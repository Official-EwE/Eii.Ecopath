Option Strict On

#Region "Contaminant tracing model"

Public Class cContaminantTracer

    ''' <summary>
    ''' Flow rate of biomass to pred from prey for each spatial unit set in Ecosim.Derivt and SpaceSolver.DerivtRed (eaten / biomass(iPrey))
    ''' </summary>
    ''' <remarks>[biomass consumed by pred]/[biomass of prey]</remarks>
    Public ConKtrophic() As Single

    ''' <summary>
    ''' Flow rate of detritus from a living group to a detrius group by a fishing fleet
    ''' ConKdet(iLivingGroup,iDetritusGroup,iFleet)
    ''' </summary>
    ''' <remarks>
    ''' see <see>eEcosim.SimDetritus</see> for how this is populated
    ''' [sum of discarded detritus from group for fleet]/[biomass of group]
    ''' </remarks>
    Public ConKdet(,,) As Single 'ngroups,ngroups,nfleets

    ''' <summary>
    ''' Concentration of contaminants at each time step
    ''' </summary>
    ''' <remarks>
    ''' updated in Cupdate() for each time step in Ecosim. 
    ''' In Ecospace this will remain constant based on the value set by Ecoism.Derivt() during initialzation of Ecospace.
    '''  </remarks>
    Public ConcTr() As Single

    ''' <summary>
    ''' Loss computed by Ecosim.Derivt or Ecospace.DerivtRed for each time step 
    ''' </summary>
    ''' <remarks>This must be set to the local loss by the calling routine for each time step</remarks>
    Public loss() As Single

    Public BypassIntegrated() As Boolean

    'references to other core data 
    Private m_EPData As cEcopathDataStructures
    Private m_ESData As cEcosimDatastructures
    Private m_Stanza As cStanzaDatastructures
    Private m_TracerData As cContaminantTracerDataStructures

    Public Sub Cupdate(ByVal Biom() As Single)
        Dim i As Integer, istep As Integer, Ceq As Single, Tst As Single, InputMult As Single
        Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single

        ReDim Derivcon(m_EPData.NumGroups), Cintotal(m_EPData.NumGroups), Closs(m_EPData.NumGroups)

        'update change in Contaminant concentrations for 1 month--call after first call to derivt
        'in adamsbasforth, rk4
        'use Closs first to calculate total uptake from environment as loss to env conc
        Tst = 1.0# / (12 * 3)
        ConcTr(m_EPData.NumGroups + 1) = 0

        If m_TracerData.ConForceNumber > 0 Then
            ' If i = 0 And m_TracerData.ConForceNumber > 0 Then
            InputMult = m_ESData.tval(m_TracerData.ConForceNumber)
        Else
            InputMult = 1
        End If

        For istep = 1 To 3
            ConDeriv(Biom, Derivcon, Cintotal, Closs, InputMult, False)
            ConCtot = 0
            For i = 0 To m_EPData.NumGroups
                ConCtot = ConCtot + ConcTr(i)
                Ceq = CSng(Cintotal(i) / (Closs(i) + 1.0E-20))
                ConcTr(i) = CSng(Ceq + (ConcTr(i) - Ceq) * Math.Exp(-Closs(i) * Tst))
                'ESData.ConcTr(i) =ConcTr(i) + Derivcon(i) * Tst
                If istep = 3 Then
                    ConcTr(m_EPData.NumGroups + 1) = ConcTr(m_EPData.NumGroups + 1) + ConcTr(i)
                End If
            Next
        Next
    End Sub


    Public Sub ConDeriv(ByVal Biom() As Single, ByVal Derivcon() As Single, ByVal Cintotal() As Single, ByVal Closs() As Single, ByVal InputMult As Single, ByVal Space As Boolean)
        'calculates total derivative of contaminant concentrations given
        'rate coefficients from interface and monthly call to derivt

        Dim i As Integer, j As Integer, ii As Integer, K As Integer
        Dim ConFlow As Single, GradFlow As Single, ist As Integer, ieco As Integer
        'Dim Ceq As Single
        Dim DetToEnv As Single
        Dim ExcretToEnv As Single
        Dim InputMultT As Single
        Dim Cgradloss() As Single
        ReDim Cgradloss(m_EPData.NumGroups)

        'leave the zero index with environmental inflows set by the user
        For i = 1 To m_EPData.NumGroups : m_TracerData.Cinflow(i) = 0 : Next

        'first accumulate inputs for all pools as functions of concs
        'in donor pools and rate constants

        'flows associated with trophic linkages
        For ii = 1 To m_ESData.inlinks
            i = m_ESData.ilink(ii) : j = m_ESData.jlink(ii)
            ConFlow = ConKtrophic(ii) * ConcTr(i) '(ConKtrophic(ii) = eat / biomass(iPrey))
            ' m_TracerData.Cinflow(j) = m_TracerData.Cinflow(j) + ConFlow * (1 - m_EPData.GS(j)) 
            m_TracerData.Cinflow(j) = m_TracerData.Cinflow(j) + ConFlow * (1 - m_EPData.GS(j)) * (1 - m_TracerData.CexcretionRate(j))

            'flow to environment of consumed contaminant excreted over all trophic flows
            ExcretToEnv = ExcretToEnv + ConFlow * (1 - m_EPData.GS(j)) * m_TracerData.CexcretionRate(j)

            For K = m_EPData.NumLiving + 1 To m_EPData.NumGroups
                m_TracerData.Cinflow(K) = m_TracerData.Cinflow(K) + m_EPData.GS(j) * ConFlow * m_EPData.DF(j, K - m_EPData.NumLiving)
            Next

        Next

        'flows associated with detritus and discards
        For i = 1 To m_EPData.NumLiving
            For j = m_EPData.NumLiving + 1 To m_EPData.NumGroups
                m_TracerData.Cinflow(j) = m_TracerData.Cinflow(j) + m_ESData.mo(i) * (1 - m_ESData.MoPred(i) + m_ESData.MoPred(i) * m_ESData.Ftime(i)) * ConcTr(i) * m_EPData.DF(i, j - m_EPData.NumLiving)
                For K = 1 To m_EPData.NumFleet 'nb: loop bypassed if numgear=0
                    m_TracerData.Cinflow(j) = m_TracerData.Cinflow(j) + ConKdet(i, j, K) * ConcTr(i)
                Next
            Next
        Next

        'flows associated with graduation among stanzas
        'If Space = False Then
        'following code will fail in ecospace, since gradflow is difficult to estimate; ignore it
        'when call is from ecospace (space=true)
        For i = 1 To m_Stanza.Nsplit
            For ist = 2 To m_Stanza.Nstanza(i)
                ieco = m_Stanza.EcopathCode(i, ist - 1)
                If Space = True Then
                    GradFlow = 12 * m_Stanza.SplitRflow(i, ist) * ConcTr(ieco)
                    Cgradloss(ieco) = 12 * m_Stanza.SplitRflow(i, ist)
                    ieco = m_Stanza.EcopathCode(i, ist)
                    m_TracerData.Cinflow(ieco) = m_TracerData.Cinflow(ieco) + GradFlow
                Else
                    GradFlow = 12 * m_Stanza.NageS(i, m_Stanza.Age1(i, ist)) * m_Stanza.WageS(i, m_Stanza.Age1(i, ist)) * ConcTr(ieco) / Biom(ieco)

                    ' ieco = EcopathCode(i, ist - 1)
                    m_TracerData.Cinflow(ieco) = m_TracerData.Cinflow(ieco) - GradFlow
                    ieco = m_Stanza.EcopathCode(i, ist)
                    m_TracerData.Cinflow(ieco) = m_TracerData.Cinflow(ieco) + GradFlow
                End If
            Next
        Next

        'other losses and flows to environment
        Closs(0) = 0
        For i = 1 To m_EPData.NumGroups
            Closs(0) = Closs(0) + m_TracerData.Cenv(i) * Biom(i)
        Next
        DetToEnv = 0
        For i = m_EPData.NumLiving + 1 To m_EPData.NumGroups
            DetToEnv = DetToEnv + m_ESData.DetritusOut(i) * ConcTr(i)
        Next

        'save this result as the "loss" rate from environment to ecosystem components
        loss(0) = Closs(0) : Biom(0) = 1

        For i = 0 To m_EPData.NumGroups
            If i = 0 Then
                InputMultT = InputMult
            Else
                InputMultT = 1.0#
            End If

            'add environmental and immigration flows to get total inflow
            '(at this point, m_tracer.Cinflow already sums inflow components from biological flows (derivt)
            Cintotal(i) = InputMultT * m_TracerData.Cinflow(i) + m_TracerData.Cimmig(i) * m_EPData.Immig(i) + m_TracerData.Cenv(i) * Biom(i) * ConcTr(0)

            'flow to environment from detritus and trophic flows
            'this contaminant flow will not be subjected to the contaminant forcing function via InputMultT
            If i = 0 Then Cintotal(0) = Cintotal(0) + DetToEnv + ExcretToEnv

            'and set up total instantaneous loss rate (note m_tracer.CoutFlow nonzero only for i=0)
            'jb for Ecospace loss will need to be ecospace loss 
            Closs(i) = loss(i) / Biom(i) + m_TracerData.cdecay(i) + m_TracerData.CoutFlow(i) + Cgradloss(i) '+ 1E-20
            Derivcon(i) = Cintotal(i) - Closs(i) * ConcTr(i)
            'Ceq = Cintotal / Closs
            'update concentration over one month assuming constant inflow and loss over month
            'ConcTr(i) = Ceq + (ConcTr(i) - Ceq) * Exp(-Closs / 12)
        Next

    End Sub


    Public Sub CInitialize()
        'initialize contaminant concentrations at start of simulation (call from initialstate)
        Try

            ReDim ConKtrophic(m_ESData.inlinks)
            ReDim ConcTr(m_EPData.NumGroups + 1)
            'BypassIntegrated() should all be false all groups need to run the grid integration 
            ReDim BypassIntegrated(m_EPData.NumGroups)

            'jb EwE5
            ' ReDim ConKdet(EPData.NumGroups, NumLiving + 1 To EPData.NumGroups, NumGear) 
            ReDim ConKdet(m_EPData.NumGroups, m_EPData.NumGroups, m_EPData.NumFleet)

            For i As Integer = 0 To m_EPData.NumGroups
                ConcTr(i) = m_TracerData.Czero(i)
                If i > 0 Then m_TracerData.CoutFlow(i) = 0 '(outflow from ecopath groups already accounted in m_data.loss(i) emig component
            Next
            ConcTr(m_EPData.NumGroups + 1) = 0   'for total in environment

            'make room for the results
            m_TracerData.redimForEcosimRun(m_ESData.nGroups, m_ESData.NTimes)

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("Contaminant Tracer initialization error: " & ex.Message, ex)
        End Try

    End Sub


    Public Sub Init(ByRef refTracerData As cContaminantTracerDataStructures, ByRef refEcopathData As cEcopathDataStructures, ByRef refEcosimData As cEcosimDatastructures, ByRef refStanzaData As cStanzaDatastructures)

        m_TracerData = refTracerData
        m_EPData = refEcopathData
        m_ESData = refEcosimData
        m_Stanza = refStanzaData

    End Sub


    Public Sub SaveEcosimTimeStepData(ByVal iTime As Integer, ByVal Biomass() As Single, ByRef TracerData As cContaminantTracerDataStructures)
        Dim igrp As Integer
        For igrp = 0 To m_EPData.NumGroups + 1
            TracerData.TracerConc(igrp, iTime) = Me.ConcTr(igrp)
            If igrp <= m_EPData.NumGroups Then
                If Biomass(igrp) > 0 Then TracerData.TracerConc(igrp, iTime) = TracerData.TracerConc(igrp, iTime) / Biomass(igrp)
            End If
        Next igrp
    End Sub


End Class

#End Region

#Region "Data structures for contaminant tracer"


''' <summary>
''' Public variables need by the contaminant tracing model cContaminantTracer
''' </summary>
''' <remarks></remarks>
Public Class cContaminantTracerDataStructures

    'EwE5 ReadEcoTracer() for variables that are saved to database
    Private m_nGroups As Integer = 0

    ''' <summary>
    ''' Decay rate per year
    ''' </summary>
    ''' <remarks>
    ''' The zero element holds the Enviroment value 
    ''' 1 to ngroups is for the groups
    ''' </remarks>
    Public cdecay() As Single

    ''' <summary>
    ''' Initial concentration of contaminants in t/km2
    ''' </summary>
    ''' <remarks>
    ''' The zero element holds the Environment value 
    ''' 1 to ngroups is for the groups
    ''' </remarks>
    Public Czero() As Single

    ''' <summary>
    ''' Base inflow rate to environment t/km2/year in Zero element
    ''' </summary>
    Public Cinflow() As Single

    ''' <summary>
    ''' Absorption rate t/t/t/year (concentration in prey/consumption/year ???)
    ''' </summary>
    Public Cenv() As Single

    ''' <summary>
    ''' Concentration in immigrating groups t/t
    ''' </summary>
    Public Cimmig() As Single

    ''' <summary>
    ''' Volume exchange loss rate zero element for environment
    ''' </summary>
    Public CoutFlow() As Single

    ''' <summary>
    ''' Pool excretion rate.
    ''' </summary>
    ''' <remarks>
    ''' <para>C.J.Walters, per email 25Nov2007:</para>
    ''' <para>
    ''' One way to improve ecotracer would be to add a pool-specific instantaneous
    ''' excretion rate, with the excretion flow going back into the 0 (environment)
    ''' pool. Such flows should be very low for lipophilic compounds, close to the
    ''' metabolic rate for compounds that are lost in proportion to routine tissue
    ''' metabolism (eg conversion of carbon 14 organic form to c14-02).
    ''' </para>
    ''' </remarks>
    Public CexcretionRate() As Single

    Public ConForceNumber As Integer

    ''' <summary>
    ''' Plot contr/biomass
    ''' </summary>
    ''' <remarks></remarks>
    Public TracePlotCB As Boolean

    ''' <summary>Results over time</summary>
    Public TracerConc(,) As Single
    ''' <summary>Concentration over time by region.</summary>
    Public TracerConcByRegion(,,) As Single ' by region, group, time
    ''' <summary>Concentration over biomass over time by region.</summary>
    Public TracerCBRegion(,,) As Single ' by region, group, time

    ''' <summary>Max concentration of contaminant at the current time step by group</summary>
    Public ConcMax() As Single

    ''' <summary>Ecosim tracer enabled state flag.</summary>
    Friend m_bEcoSimConSimOn As Boolean
    ''' <summary>Ecospace tracer enabled state flag.</summary>
    Friend m_bEcoSpaceConSimOn As Boolean

    Friend Sub RedimByNGroups(ByVal nGroups As Integer)
        'redimension variables needed for contaminant tracking
        ReDim Czero(nGroups)
        ReDim Cimmig(nGroups)
        ReDim Cenv(nGroups)
        ReDim cdecay(nGroups)
        ReDim Cinflow(nGroups)
        ReDim CoutFlow(nGroups)
        ReDim CexcretionRate(nGroups)
        Me.m_nGroups = nGroups
    End Sub

    Public Sub redimForEcosimRun(ByVal nGroups As Integer, ByVal nTime As Integer)
        ReDim TracerConc(nGroups + 1, nTime)
    End Sub

    Public Sub redimForEcospaceRun(ByVal nRegions As Integer, ByVal nGroups As Integer, ByVal nTime As Integer)
        ReDim TracerConcByRegion(nRegions, nGroups + 1, nTime)
        ReDim TracerCBRegion(nRegions, nGroups + 1, nTime)
    End Sub

    Public Sub Clear()
        Me.m_nGroups = 0
        Me.RedimByNGroups(0)
    End Sub

    Public Sub CopyTo(ByRef d As cContaminantTracerDataStructures)
        'EwE5 ReadEcoTracer() for variables that are saved to database

        d.cdecay = CType(cdecay.Clone, Single())
        d.Czero = CType(Czero.Clone, Single())
        d.Cinflow = CType(Cinflow.Clone, Single())
        d.Cenv = CType(Cenv.Clone, Single())
        d.Cimmig = CType(Cimmig.Clone, Single())
        d.CoutFlow = CType(CoutFlow.Clone, Single())
        d.CexcretionRate = CType(CexcretionRate.Clone, Single())
        d.ConForceNumber = ConForceNumber
        d.EcoSimConSimOn = EcoSimConSimOn
        d.EcoSpaceConSimOn = EcoSpaceConSimOn
    End Sub

    Public Property EcoSimConSimOn() As Boolean
        Get
            Return ((Me.m_bEcoSimConSimOn = True) And (Me.m_nGroups > 0))
        End Get
        Set(ByVal value As Boolean)
            m_bEcoSimConSimOn = value
        End Set
    End Property


    Public Property EcoSpaceConSimOn() As Boolean
        Get
            Return m_bEcoSpaceConSimOn
            'jb this is not valid for Ecospace
            'because of the mulithreading Ecospace gets a copy of the data initialized be the database
            'RedimByNGroups() is never called on the actual object so m_nGroups is never set
            '  Return ((Me.m_bEcoSpaceConSimOn = True) And (Me.m_nGroups > 0))
        End Get
        Set(ByVal value As Boolean)
            m_bEcoSpaceConSimOn = value
        End Set
    End Property


End Class

#End Region

