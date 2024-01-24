' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' <summary>
''' Contaminant tracing model.
''' </summary>
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
    Public bio() As Single

    Public BypassIntegrated() As Boolean

    Public EnvConDriver() As Single

    Public ThreadID As Integer

    'references to other core data 
    Private m_EPData As cEcopathDataStructures
    Private m_ESData As cEcosimDatastructures
    Private m_Stanza As cStanzaDatastructures
    Private m_TracerData As cContaminantTracerDataStructures


    Public Sub Cupdate(Biom() As Single)
        Dim i As Integer, istep As Integer, Ceq As Single, Tst As Single, InputMult As Single
        Dim maxT As Single, nstep As Integer, Ttemp As Single, Terr As Single, tempsum As Single
        Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, Derivcon2() As Single

        ReDim Derivcon(Me.m_EPData.NumGroups), Cintotal(Me.m_EPData.NumGroups), Closs(Me.m_EPData.NumGroups), Derivcon2(Me.m_EPData.NumGroups)

        'update change in Contaminant concentrations for 1 month--call after first call to derivt
        'in adamsbasforth, rk4
        'use Closs first to calculate total uptake from environment as loss to env conc
        'Tst = 1.0# / (12 * 30)
        Me.ConcTr(Me.m_EPData.NumGroups + 1) = 0

        If Me.m_TracerData.ConForceNumber > 0 Then
            InputMult = Me.m_ESData.tval(Me.m_TracerData.ConForceNumber)
            Debug.Assert(InputMult <> 1)
        Else
            InputMult = 1
        End If

        'find the maximum allowable timestep
        Me.ConDeriv(Biom, Derivcon, Cintotal, Closs, InputMult, False)
        maxT = 1.0# / 12
        For i = 0 To Me.m_EPData.NumGroups
            'calculate equilibrium state estimate
            'Ceq = CSng(Cintotal(i) / (Closs(i)) + 1.0E-20)
            'this should be allowed to evaluate as Inf or NaN
            Ceq = CSng(Cintotal(i) / Closs(i))
            'calculate distance to equilibrium (%)
            'if the equilibrium is Inf or NaN, then this should evaluate to NaN
            Terr = CSng(2.0 * Math.Abs(Ceq - Me.ConcTr(i)) / (Ceq + Me.ConcTr(i) + 1.0E-30))
            If Terr < 0.01 Then
                'this forces the maximum timestep size to be 1/closs
                Terr = 0.01
            End If
            'minimum timestep is 0.01 times 1/closs (which is essentially the time to equilibrium at the current derivative value)
            'the timestep scales from (0.01 to 1.0) times 1/closs as ConcTr approaches Ceq
            'in the case of NaN for Terr, this evaluates to NaN, and no action will be taken (keep default timestep)
            Ttemp = CSng(0.01 / Terr / Closs(i))
            If Ttemp < maxT Then
                maxT = Ttemp
            End If
        Next
        'calculate number of ecotracer timesteps per ecosim timestep based on the max timestep
        nstep = CInt(Math.Ceiling(1.0 / 12.0 / maxT))
        'cap the max timestep at the user defined value
        nstep = Math.Min(nstep, m_TracerData.MaxTimeSteps) '
        Tst = CSng(1.0# / (12 * nstep))

        'Euler 1st step
        For i = 0 To Me.m_EPData.NumGroups
            Me.ConcTr(i) = Me.ConcTr(i) + Derivcon(i) * Tst
            Debug.Assert(Not Single.IsNegativeInfinity(Me.ConcTr(i)))
            Derivcon2(i) = Derivcon(i)
            Me.EnvConDriver(i) = 0.0
        Next

        'Adams bashford steps 2-N
        For istep = 2 To nstep
            Me.ConDeriv(Biom, Derivcon, Cintotal, Closs, InputMult, False)
            For i = 0 To Me.m_EPData.NumGroups
                'ConCtot = ConCtot + ConcTr(i)
                'Analytic solution assuming Cintotal is constant (this does not conserve mass in general)
                'Ceq = CSng(Cintotal(i) / (Closs(i) + 1.0E-20))
                'ConcTr(i) = CSng(Ceq + (ConcTr(i) - Ceq) * Math.Exp(-Closs(i) * Tst))
                'Euler
                'ConcTr(i) = ConcTr(i) + Derivcon(i) * Tst
                'Adams Bashford multistep
                Me.ConcTr(i) = CSng(Me.ConcTr(i) + (3.0 * Derivcon(i) - Derivcon2(i)) * Tst / 2.0)
                Debug.Assert(Not Single.IsNegativeInfinity(Me.ConcTr(i)))
                Derivcon2(i) = Derivcon(i)
            Next
        Next
        'Sum up the total concentration in the last ConcTr position
        tempsum = 0
        For i = 0 To Me.m_EPData.NumGroups
            Me.ConcTr(Me.m_EPData.NumGroups + 1) = Me.ConcTr(Me.m_EPData.NumGroups + 1) + Me.ConcTr(i)
            tempsum = tempsum + Derivcon(i)
            Debug.Assert(Not Single.IsNegativeInfinity(Me.ConcTr(i)))
        Next

    End Sub


    Public Sub ConDeriv(Biom() As Single, Derivcon() As Single, Cintotal() As Single, Closs() As Single, InputMult As Single, Space As Boolean)
        'calculates total derivative of contaminant concentrations given
        'rate coefficients from interface and monthly call to derivt

        Dim i As Integer, j As Integer, ii As Integer, K As Integer
        Dim ConFlow As Single, GradFlow As Single, ist As Integer, ieco As Integer
        'Dim Ceq As Single
        Dim DetToEnv As Single
        Dim ExcretToEnv As Single
        Dim InputMultT As Single
        Dim Cgradloss() As Single
        ReDim Cgradloss(Me.m_EPData.NumGroups)

        Try

            'Dim Cinflow() As Single = New Single(Me.m_ESData.nGroups) {}

            'leave the zero index with environmental inflows set by the user
            For i = 1 To Me.m_EPData.NumGroups : Me.m_TracerData.Cinflow(i) = 0 : Next

            'Cinflow(0) = Me.m_TracerData.Cinflow(0)
            'first accumulate inputs for all pools as functions of concs
            'in donor pools and rate constants

            'flows associated with trophic linkages
            For ii = 1 To Me.m_ESData.inlinks
                i = Me.m_ESData.ilink(ii) : j = Me.m_ESData.jlink(ii)
                ConFlow = Me.ConKtrophic(ii) * Me.ConcTr(i) '(ConKtrophic(ii) = eat / biomass(iPrey))
                Me.m_TracerData.Cinflow(j) += ConFlow * (1 - Me.m_TracerData.CassimProp(j))

                'flow to environment of consumed contaminant excreted over all trophic flows
                ExcretToEnv = ExcretToEnv + ConFlow * Me.m_TracerData.CassimProp(j)

                'Debug.Assert(Not Single.IsNaN(Cinflow(j)))


            Next ii

            'flows associated with detritus and discards
            For i = 1 To Me.m_EPData.NumLiving
                For j = Me.m_EPData.NumLiving + 1 To Me.m_EPData.NumGroups
                    Me.m_TracerData.Cinflow(j) = Me.m_TracerData.Cinflow(j) + Me.m_ESData.mo(i) * (1 - Me.m_ESData.MoPred(i) + Me.m_ESData.MoPred(i) * Me.m_ESData.Ftime(i)) * Me.ConcTr(i) * Me.m_EPData.DF(i, j - Me.m_EPData.NumLiving)
                    For K = 1 To Me.m_EPData.NumFleet 'nb: loop bypassed if numgear=0
                        Me.m_TracerData.Cinflow(j) += Me.ConKdet(i, j - Me.m_EPData.NumLiving, K) * Me.ConcTr(i)

                        'Debug.Assert(Not Single.IsNaN(Cinflow(j)))
                    Next
                Next
            Next

            'flows associated with graduation among stanzas
            'If Space = False Then
            'following code will fail in ecospace, since gradflow is difficult to estimate; ignore it
            'when call is from ecospace (space=true)
            For i = 1 To Me.m_Stanza.Nsplit
                For ist = 2 To Me.m_Stanza.Nstanza(i)
                    ieco = Me.m_Stanza.EcopathCode(i, ist - 1)
                    If Space = True Then
                        GradFlow = 12 * Me.m_Stanza.SplitRflow(i, ist) * Me.ConcTr(ieco)
                        Cgradloss(ieco) = 12 * Me.m_Stanza.SplitRflow(i, ist)
                        ieco = Me.m_Stanza.EcopathCode(i, ist)
                        Me.m_TracerData.Cinflow(ieco) += GradFlow

                    Else
                        GradFlow = 12 * Me.m_Stanza.NageS(i, Me.m_Stanza.Age1(i, ist)) * Me.m_Stanza.WageS(i, Me.m_Stanza.Age1(i, ist)) * Me.ConcTr(ieco) / Biom(ieco)

                        ' ieco = EcopathCode(i, ist - 1)
                        Me.m_TracerData.Cinflow(ieco) = Me.m_TracerData.Cinflow(ieco) - GradFlow
                        ieco = Me.m_Stanza.EcopathCode(i, ist)
                        Me.m_TracerData.Cinflow(ieco) = Me.m_TracerData.Cinflow(ieco) + GradFlow
                        'Cinflow(ieco) += +GradFlow
                    End If
                    'Debug.Assert(Not Single.IsNaN(Me.m_TracerData.Cinflow(ieco)))
                Next
            Next

            'other losses and flows to environment
            Closs(0) = 0
            For i = 1 To Me.m_EPData.NumGroups
                Closs(0) = Closs(0) + Me.m_TracerData.Cenv(i) * Biom(i)
                ExcretToEnv = ExcretToEnv + Me.ConcTr(i) * Me.m_TracerData.CmetabolismRate(i)
            Next
            DetToEnv = 0
            For i = Me.m_EPData.NumLiving + 1 To Me.m_EPData.NumGroups
                DetToEnv = DetToEnv + Me.m_ESData.DetritusOut(i) * Me.ConcTr(i)
            Next

            'save this result as the "loss" rate from environment to ecosystem components
            Me.loss(0) = Closs(0) : Biom(0) = 1


            For i = 0 To Me.m_EPData.NumGroups
                If i = 0 Then
                    InputMultT = InputMult
                Else
                    InputMultT = 1.0#
                End If

                'add environmental and immigration flows to get total inflow
                '(at this point, m_tracer.Cinflow already sums inflow components from biological flows (derivt)
                Cintotal(i) = InputMultT * Me.m_TracerData.Cinflow(i) + Me.m_TracerData.Cimmig(i) * Me.m_EPData.Immig(i) + Me.m_TracerData.Cenv(i) * Biom(i) * Me.ConcTr(0)
                'Added Ecospace forced contaminants
                Cintotal(i) += Me.EnvConDriver(i)
                'Debug.Assert(Not Single.IsNaN(Cintotal(i)))

                'flow to environment from detritus and trophic flows
                'this contaminant flow will not be subjected to the contaminant forcing function via InputMultT
                If i = 0 Then Cintotal(0) = Cintotal(0) + DetToEnv + ExcretToEnv

                'and set up total instantaneous loss rate (note m_tracer.CoutFlow nonzero only for i=0)
                'jb for Ecospace set loss to Ecospace loss by cell Me.lossSpace(group)(row, col)
                Closs(i) = Me.loss(i) / Biom(i) + Me.m_TracerData.cdecay(i) + Me.m_TracerData.CoutFlow(i) + Cgradloss(i) + Me.m_TracerData.CmetabolismRate(i) '+ 1E-20
                Derivcon(i) = Cintotal(i) - Closs(i) * Me.ConcTr(i)
                'Ceq = Cintotal / Closs
                'update concentration over one month assuming constant inflow and loss over month
                'ConcTr(i) = Ceq + (ConcTr(i) - Ceq) * Exp(-Closs / 12)
            Next

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try


    End Sub


    Public Sub CInitialize()
        'Public Sub CInitialize(nMapRows As Integer, nMapCols As Integer)
        'initialize contaminant concentrations at start of simulation (call from initialstate)
        Try

            ReDim Me.ConKtrophic(Me.m_ESData.inlinks)
            ReDim Me.ConcTr(Me.m_EPData.NumGroups + 1)
            'BypassIntegrated() should all be false all groups need to run the grid integration 
            ReDim Me.BypassIntegrated(Me.m_EPData.NumGroups)
            ReDim Me.EnvConDriver(Me.m_EPData.NumGroups + 1)
            ReDim Me.ConKdet(Me.m_EPData.NumGroups, Me.m_EPData.NumGroups - Me.m_EPData.NumLiving, Me.m_EPData.NumFleet)

            For i As Integer = 0 To Me.m_EPData.NumGroups
                Me.ConcTr(i) = Me.m_TracerData.Czero(i)
                If i > 0 Then Me.m_TracerData.CoutFlow(i) = 0 '(outflow from ecopath groups already accounted in m_data.loss(i) emig component
            Next
            Me.ConcTr(Me.m_EPData.NumGroups + 1) = 0   'for total in environment

            'make room for the results
            Me.m_TracerData.redimForEcosimRun(Me.m_ESData.nGroups, Me.m_ESData.NTimes)

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("Contaminant Tracer initialization error: " & ex.Message, ex)
        End Try

    End Sub


    Public Sub Init(ByRef refTracerData As cContaminantTracerDataStructures, ByRef refEcopathData As cEcopathDataStructures, ByRef refEcosimData As cEcosimDatastructures, ByRef refStanzaData As cStanzaDatastructures)

        Me.m_TracerData = refTracerData
        Me.m_EPData = refEcopathData
        Me.m_ESData = refEcosimData
        Me.m_Stanza = refStanzaData

    End Sub


    Public Sub SaveEcosimTimeStepData(iTime As Integer, Biomass() As Single, ByRef TracerData As cContaminantTracerDataStructures)
        Dim igrp As Integer
        For igrp = 0 To Me.m_EPData.NumGroups + 1
            TracerData.TracerConc(igrp, iTime) = Me.ConcTr(igrp)
            If igrp <= Me.m_EPData.NumGroups Then
                If Biomass(igrp) > 1.0E-20F Then
                    TracerData.TracerCB(igrp, iTime) = Me.ConcTr(igrp) / (Biomass(igrp) + 1.0E-20F)
                Else
                    TracerData.TracerCB(igrp, iTime) = 0.0
                End If
            End If
        Next igrp
    End Sub


End Class
