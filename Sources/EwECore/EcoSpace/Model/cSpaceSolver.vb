Imports System
Imports System.Threading

Public Class cSpaceSolver

    ''' <summary>
    ''' Signal mechanism used by the calling thread for thread Synchronization
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state SignalState.Reset()) 
    ''' calls to SignalState.WaitOne() will block until Solve() has completed (SignalState in a signaled state SignalState.Set())
    ''' </remarks>
    Public SignalState As New ManualResetEvent(True)

    Private m_ConTracer As cContaminantTracer

    Public iYear As Integer ' current year

    ''' <summary>
    ''' Delegate for posting error messages.
    ''' </summary>
    ''' <remarks>
    ''' All error handling must be done on the same thread. Errors can not be thrown from one thread to another.
    ''' A delegate must be used to cross the thread boundary. EcospaceErrorHandler is a delegate to a sub on the main Ecospace thread.
    ''' </remarks>
    Public EcospaceErrorHandler As cEcoSpace.SolverErrorDelegate

    Public isOkToRun As Boolean
    Public ThreadID As Integer

    'references
    Public m_EcospaceModel As cEcoSpace
    Public m_Data As cEcospaceDataStructures
    Public m_SimData As cEcosimDatastructures
    Public m_PathData As cEcopathDataStructures
    Public m_Stanza As cStanzaDatastructures
    Public m_Ecosim As Ecosim.cEcoSimModel
    Public m_TracerData As cContaminantTracerDataStructures

    Public Search As cSearchDatastructures

    Public Bcw(,,) As Single
    Public C(,,) As Single
    Public d(,,) As Single
    Public e(,,) As Single
    Public BEQLast(,,) As Single
    ' Public WchangeVar(,,) As Single
    Public Btime() As Single
    Public F(,,) As Single
    Public AMm(,,) As Single
    Public Ecode() As Integer
    Public HdenCell(,,) As Single
    Public RelFitness(,,) As Single
    Public FtimeCell(,,) As Single
    Public Cper(,,) As Single
    Public PconSplit() As Single
    Public RelRepStanza() As Single
    Public Tstanza() As Single
    Public PbSpace() As Single

    'needs to be set from ecospace, but not references
    'Public Tn As Integer
    Public nvar2 As Integer
    Public itt As Integer
    Public PPScale As Single
    Public TimeStep2 As Single
    Public MinChange As Single

    'locals
    'Private ebb() As Single
    Private BB() As Single
    Private loss() As Single
    Private RelPPupwell As Single
    Private RelR As Single, RelRS As Single, Rflow As Single
    Private Flowin() As Single
    Private FlowoutRate() As Single
    Private ieco As Integer
    Private isc As Integer
    Private isp As Integer
    Private ist As Integer
    Private EatEff() As Single
    Private VulPred() As Single
    Private ig As Integer
    Private pbb() As Single

    'These are total sums for every cell, so must be summed for each thread seperately, then combined after they've all run
    Public BtimeLocal() As Single
    Public TotLossThread() As Single
    Public TotEatenByThread() As Single
    Public TotBiomThread() As Single
    Public TotPredThread() As Single
    Public TotIFDweightThread() As Single

    'the ip groups to solve
    Private iFrstCell As Integer
    Private iLastCell As Integer

    'variables from m_ESData, used locally
    Private Hden() As Single
    Private Ftime() As Single
    Private Fish1() As Single
    Private FishTime() As Single
    Private FishRateGear(,) As Single
    Private pred() As Single
    Private Eatenof() As Single
    Private Eatenby() As Single
    Private RelaSwitch() As Single
    Private NutBiom As Single
    Private NutFree As Single
    Private MedVal() As Single

    'Contaminant tracing used locally
    Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single

    Private RtoNext As Single
    Private SurvRat As Single

    ''' <summary>Detritus by Group</summary>
    ''' <remarks>Added for Atlantis coupling. Local copy passes to SimDetritusMT() by each thread(this prevents cross thread corruption) then used to update map </remarks>
    Private GroupDetritus() As Single

    Public Sub Init()
        'local spatial variables
        ReDim loss(m_Data.NGroups)
        ReDim pbb(m_Data.NGroups)
        ReDim EatEff(m_Data.nvartot)
        ReDim VulPred(m_Data.nvartot)
        ReDim Flowin(m_Data.nvartot)
        ReDim FlowoutRate(m_Data.nvartot)
        'ReDim ebb(m_Data.nvartot)
        ReDim BB(m_Data.nvartot)
        ReDim pbb(m_Data.NGroups)

        'local versions of ecosim variables
        ReDim Hden(m_Data.NGroups)
        ReDim Ftime(m_Data.NGroups)
        ReDim Fish1(m_Data.NGroups)
        ReDim FishTime(m_Data.NGroups)
        ReDim FishRateGear(m_Data.nFleets, m_Data.NGroups)
        ReDim pred(m_Data.NGroups)
        ReDim Eatenof(m_Data.NGroups)
        ReDim Eatenby(m_Data.NGroups)
        ReDim MedVal(m_SimData.MediationShapes)

        'thread copy of global sums
        ReDim BtimeLocal(m_Data.NGroups)
        ReDim TotLossThread(m_Data.NGroups)
        ReDim TotEatenByThread(m_Data.NGroups)
        ReDim TotBiomThread(m_Data.NGroups)
        ReDim TotPredThread(m_Data.NGroups)
        ReDim TotIFDweightThread(m_Data.NGroups)
        ReDim GroupDetritus(m_Data.NGroups)

        'local copies are initialized from the ecosim data
        Array.Copy(m_SimData.Hden, Hden, m_Data.NGroups + 1)
        Array.Copy(m_SimData.Ftime, Ftime, m_Data.NGroups + 1)
        Array.Copy(m_SimData.Fish1, Fish1, m_Data.NGroups + 1)
        Array.Copy(m_SimData.FishTime, FishTime, m_Data.NGroups + 1)
        Array.Copy(m_SimData.pred, pred, m_Data.NGroups + 1)
        Array.Copy(m_SimData.Eatenof, Eatenof, m_Data.NGroups + 1)
        Array.Copy(m_SimData.Eatenby, Eatenby, m_Data.NGroups + 1)
        Array.Copy(m_SimData.MedVal, MedVal, m_SimData.MediationShapes + 1)

        m_ConTracer.Init(m_TracerData, m_PathData, m_SimData, m_Stanza)
        m_ConTracer.CInitialize()

    End Sub

    Public Sub Clear()

        Try
            'each solver get it's own Contaminant Tracer data and model
            Me.m_TracerData.Clear()
            Me.m_ConTracer = Nothing
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".Clear() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try

    End Sub

    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstGroup"></param>
    ''' <param name="iLastGroup"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastCells(ByVal iFirstGroup As Integer, ByVal iLastGroup As Integer)
        iFrstCell = iFirstGroup
        iLastCell = iLastGroup
    End Sub


    ''' <summary>
    ''' Do any processing necessary at the start of a new year
    ''' </summary>
    ''' <param name="iYear"></param>
    ''' <remarks></remarks>
    Public Sub YearTimeStep(ByVal iYear As Integer)
        Try
            If Search.bInSearch Then
                'Indicators need to clear out there yearly data
                ' Indic.YearTimeStep(m_EPData)

            End If
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Solve(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)

        If m_TracerData.EcoSpaceConSimOn Then
            ReDim Derivcon(m_PathData.NumGroups), Cintotal(m_PathData.NumGroups), Closs(m_PathData.NumGroups)
        End If

        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        isOkToRun = False
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            SignalState.Reset()
            Dim iGrp As Integer

            'do the processing here
            For iGrp = iFrstCell To iLastCell
                'iGrp is the linear index of the two dimensional spatial array
                'it is now converted into a row/col index for use in the rest of the algorithm
                'i = (iGrp - 1) \ m_Data.InCol + 1
                'j = (iGrp - 1) Mod m_Data.InCol + 1

                'now do the computations
                SolveCell(m_Data.iWaterCellIndex(iGrp), m_Data.jWaterCellIndex(iGrp))

            Next iGrp

            'thread has finished it is ok to run this again
            isOkToRun = True

            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            SignalState.Set()

        Catch ex As Exception
            cLog.Write(ex) 'this is dangerous clog.Write is not thread safe

            'prevent this thread from blocking forever if it throws an error
            SignalState.Set()
            isOkToRun = True

            'tell the main thread that this solver has had a problem
            'If EcospaceErrorHandler IsNot Nothing Then
            'Me.EcospaceErrorHandler(Me.ThreadID, ex.Message)
            'Else
            Debug.Assert(False, ex.Message)
            'End If

        End Try

    End Sub


#End Region

    Private Function SolveCell(ByVal i As Integer, ByVal j As Integer) As Boolean

        Dim ip As Integer

        Try

            'this changes the timestep for higher order numerical sceme.  the timestep isn't actuall different, it's a multiplier
            TimeStep2 = m_Data.TimeStep * 0.66667

            If m_TracerData.EcoSpaceConSimOn Then
                m_ConTracer.ConcTr(0) = m_Data.Ccell(i, j, 0)
                'jb ConTotal() is not used anywhere
                'For ip = 0 To m_Data.NGroups : ConTotal(ip) = ConTotal(ip) + m_Data.Ccell(i, j, ip):Next
            End If

            For ip = 1 To m_Data.NGroups
                'abmpa: at this point (after having been in solvegrid) the BCell holds
                'the long term equilibrium biomass or at least an approx to)
                'ebb(ip) = m_Data.Bcell(i, j, ip)
                '                        'in ecoseed the effort distribution needs to be calculated using ebb(),
                '                        'not the Bcell (short term biomass)
                'If m_Data.SpaceTime = False Then
                '    If m_Data.TimeNow > 0 Then
                '        m_Data.Bcell(i, j, ip) = (m_Data.Bcell(i, j, ip) + BEQLast(i, j, ip)) / 2.0#
                '    End If
                '    If m_Data.chkMPA And m_Data.EcoseedOn = False Then
                '        m_Data.Bcell(i, j, ip) = (1 - WchangeVar(i, j, ip)) * m_Data.Bcell(i, j, ip) + WchangeVar(i, j, ip) * m_Data.Blast(i, j, ip)
                '    End If
                '    BEQLast(i, j, ip) = ebb(ip)
                'End If

                'm_Data.Blast(i, j, ip) = m_Data.Bcell(i, j, ip)
                'end abmpa
                If m_Data.Depth(i, j) = 0 Then m_Data.Bcell(i, j, ip) = 0
                BB(ip) = m_Data.Bcell(i, j, ip)

                If m_TracerData.EcoSpaceConSimOn Then m_ConTracer.ConcTr(ip) = m_Data.Ccell(i, j, ip)

                'sum biomass over all the cells
                'this is now done individually for each thread, then summed outside the threads
                'Btime(ip) = Btime(ip) + BB(ip)
                BtimeLocal(ip) = BB(ip) + BtimeLocal(ip)

                If (m_SimData.NoIntegrate(ip) = ip Or m_SimData.NoIntegrate(ip) < 0) And m_SimData.SimGE(ip) > 0 Then
                    If (Cper(i, j, ip) > 0 And m_SimData.FtimeAdjust(ip) > 0) Then
                        FtimeCell(i, j, ip) = FtimeCell(i, j, ip) * (0.7 + 0.3 * m_SimData.Cbase(ip) / Cper(i, j, ip))
                    End If
                    '  FtimeCell(i, j, ip) = Cbase(ip) / Cper(i, j, ip)
                    If FtimeCell(i, j, ip) > m_SimData.FtimeMax(ip) Then FtimeCell(i, j, ip) = m_SimData.FtimeMax(ip)
                    If FtimeCell(i, j, ip) < 0.1 Then FtimeCell(i, j, ip) = 0.1
                    Ftime(ip) = FtimeCell(i, j, ip)
                End If

                Hden(ip) = HdenCell(i, j, ip)
                'VC20Aug98: This should consider fleets which can fish in the MPA
                ' The variable MPAfishery(gear, habitattype)=true indicates that
                ' the specific gear can fish in the specified habitattype
                If m_Data.Depth(i, j) > 0 Then
                    If m_Data.PredictEffort Then
                        FishTime(ip) = m_Data.Ftot(ip, i, j)
                        '****Following lines set fishrategear for Simdetritus
                        For ig = 1 To m_Data.nFleets
                            FishRateGear(ig, 0) = m_Data.EffortSpace(ig, i, j)
                            'effortspace should be 1.0 for cell with "average" effort by gear type ig
                        Next
                    Else
                        FishTime(ip) = Fish1(ip)
                        '****Following lines set fishrategear for Simdetritus
                        For ig = 1 To m_Data.nFleets
                            FishRateGear(ig, 0) = 1 ' 1 x FishMGear(ig, ip)
                        Next
                    End If 'If m_Data.PredictEffort > 0 Then

                Else
                    'depth<=0
                    FishTime(ip) = 0
                    '****Following line sets fishrategear for Simdetritus
                    For ig = 1 To m_Data.nFleets
                        FishRateGear(ig, 0) = 0
                    Next

                End If 'If m_Data.Depth(i, j) > 0 Then

                EatEff(ip) = 1
                VulPred(ip) = 1

                If m_Data.PrefHab(ip, m_Data.HabType(i, j)) = False And m_Data.PrefHab(ip, 0) = False Then
                    VulPred(ip) = m_Data.RelVulBad(ip)
                    EatEff(ip) = m_Data.EatEffBad(ip)
                End If

                If Search.bInSearch Then
                    Search.calcEcoSpaceMonthlyCatch(ip, BB, m_Data.EffortSpace, i, j, iYear, m_Data.TimeStep)
                End If
                ' m_EcospaceModel.summarizeCatchData(Tn, itt, ip, BB, i, j)

            Next ip

            m_EcospaceModel.accumCatchData(itt, BB, i, j)

            For isc = 1 To m_Data.Nvarsplit
                ieco = Ecode(isc)
                'ebb(nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)

                If m_Data.NewMultiStanza Or m_Data.UseIBM Then
                    pred(ieco) = m_Data.PredCell(i, j, ieco)
                Else
                    pred(ieco) = m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) ' Nadult(i, j, ip)
                End If
            Next isc

            RelPPupwell = 1 + m_Data.PPupWell * m_Data.UpVel(i, j) / m_Data.CellLength

            If RelPPupwell < 1 Then RelPPupwell = 1

            '  Debug.Assert(i <> 2)
            'jb compute Flowin() and FlowoutRate() for all groups for this row/col
            'If NutFile = "" Then
            derivtRed(BB, Flowin, FlowoutRate, EatEff, VulPred, m_Data.RelPP(i, j) / PPScale * RelPPupwell, i, j)
            'Else
            '    derivtRed(BB(), Flowin(), FlowoutRate(), EatEff(), VulPred(), NutForce(i, j, Inutmonth))
            'End If

            If m_TracerData.EcoSpaceConSimOn Then
                m_ConTracer.loss = loss 'set loss to ecospace loss for this cell
                m_ConTracer.ConDeriv(BB, Derivcon, Cintotal, Closs, m_Data.RelCin(i, j), True)
            End If

            'jb now populate the spatial matrixes with the data computed by derivtRed() for this cell across all groups
            For ip = 1 To m_Data.NGroups
                HdenCell(i, j, ip) = Hden(ip)
                If pred(ip) > 1.0E-30 Then
                    RelFitness(i, j, ip) = (m_SimData.SimGE(ip) * Eatenby(ip) - loss(ip)) / pred(ip) + FishTime(ip)
                Else
                    RelFitness(i, j, ip) = -2.0# * m_PathData.PB(ip)
                End If
            Next

            For ip = 1 To m_Data.NGroups

                Me.m_Data.GroupDetritus(i, j, ip) = GroupDetritus(ip)

                F(i, j, ip) = Flowin(ip)
                AMm(i, j, ip) = -FlowoutRate(ip) - Bcw(i + 1, j, ip) - C(i - 1, j, ip) - d(i, j, ip) - e(i, j, ip)
                If AMm(i, j, ip) >= 0 Then AMm(i, j, ip) = -1.0E+30
                'm_Data.deriv2(i, j, ip) = m_Data.deriv(i, j, ip)
                'm_Data.deriv(i, j, ip) = AMm(i, j, ip) * m_Data.Bcell(i, j, ip) + F(i, j, ip) + Bcw(i, j, ip) * m_Data.Bcell(i - 1, j, ip) + C(i, j, ip) * m_Data.Bcell(i + 1, j, ip) + d(i, j - 1, ip) * m_Data.Bcell(i, j - 1, ip) + e(i, j + 1, ip) * m_Data.Bcell(i, j + 1, ip)
                If m_Data.SpaceTime Then
                    AMm(i, j, ip) = AMm(i, j, ip) - 1 / TimeStep2
                    'this is for new 2nd order BDF numerical sceme (replacing backwards euler)
                    F(i, j, ip) = F(i, j, ip) + (1.3333 * m_Data.Bcell(i, j, ip) - 0.3333 * m_Data.Blast(i, j, ip)) / TimeStep2
                    m_Data.Blast(i, j, ip) = m_Data.Bcell(i, j, ip)
                End If

                If m_SimData.SimGE(ip) > 0 Then
                    Cper(i, j, ip) = Eatenby(ip) / (m_Data.Bcell(i, j, ip) + 1.0E-20)
                End If
                If Cper(i, j, ip) < 0.001 * m_SimData.Cbase(ip) Then
                    Cper(i, j, ip) = 0.001 * m_SimData.Cbase(ip)
                End If

            Next ip

            If m_TracerData.EcoSpaceConSimOn Then
                For ip = 0 To m_Data.NGroups
                    m_Data.Ftr(i, j, ip) = Cintotal(ip)
                    m_Data.AMmTr(i, j, ip) = -Closs(ip) - Bcw(i + 1, j, ip) - C(i - 1, j, ip) - d(i, j, ip) - e(i, j, ip)
                    If m_Data.AMmTr(i, j, ip) >= 0 Then m_Data.AMmTr(i, j, ip) = -1.0E+30
                    '   If m_Data.SpaceTime And FastIntegrate(ip) = False Then
                    m_Data.Ftr(i, j, ip) = m_Data.Ftr(i, j, ip) + m_Data.Ccell(i, j, ip) / TimeStep2 '/ m_Data.TimeStep
                    m_Data.AMmTr(i, j, ip) = m_Data.AMmTr(i, j, ip) - 1 / TimeStep2 '/ m_Data.TimeStep
                    '  End If
                Next
            End If

            isc = 0
            For isp = 1 To m_Stanza.Nsplit
                ieco = m_Stanza.EcopathCode(isp, m_Stanza.Nstanza(isp))
                RelR = m_Data.Bcell(i, j, ieco) * RelRepStanza(isp)
                For ist = 1 To m_Stanza.Nstanza(isp)
                    isc = isc + 1
                    ieco = m_Stanza.EcopathCode(isp, ist)
                    If m_Data.NewMultiStanza Then
                        'accumulate information needed to predict mean stanza loss, feeding, IFD weights from derivtred outputs
                        'these arrays are used in the new SpaceSplitUpdate subroutine for predicting mortality
                        'rate and growth rate averages over space by age in that update routine
                        'IFDweight is used to predict proportion of biomass of ieco stanza that will be on cell i,j
                        If (m_Data.PrefHab(ieco, m_Data.HabType(i, j)) = True Or m_Data.PrefHab(ieco, 0) = True) And m_Data.Depth(i, j) > 0 Then
                            TotLossThread(ieco) = TotLossThread(ieco) + loss(ieco)
                            TotEatenByThread(ieco) = TotEatenByThread(ieco) + Eatenby(ieco)
                            TotBiomThread(ieco) = TotBiomThread(ieco) + m_Data.Bcell(i, j, ieco)
                            TotPredThread(ieco) = TotPredThread(ieco) + pred(ieco)
                            'm_Data.IFDweight(i, j, ieco) = ((Eatenby(ieco) / pred(ieco)) / (loss(ieco) / m_Data.Bcell(i, j, ieco))) ^ m_Data.IFDPower
                            m_Data.IFDweight(i, j, ieco) = m_Data.Bcell(i, j, nvar2 + isc)
                            TotIFDweightThread(ieco) = TotIFDweightThread(ieco) + m_Data.IFDweight(i, j, ieco)
                        End If
                    ElseIf m_Data.UseIBM Then
                        m_Stanza.Zcell(i, j, ieco) = loss(ieco) / (m_Data.Bcell(i, j, ieco) + 1.0E-30)
                        If m_Data.Bcell(i, j, ieco) = 0 Then
                            m_Stanza.Zcell(i, j, ieco) = 0
                        End If
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30)
                        If m_Data.PredCell(i, j, ieco) = 0 Then
                            Cper(i, j, ieco) = m_SimData.Cbase(ieco)
                        End If
                    End If
                    SurvRat = Math.Exp(-FlowoutRate(ieco) * Tstanza(isc))
                    RelRS = RelR * SurvRat 'Math.Exp(-FlowoutRate(ieco) * Tstanza(isc))
                    If ist = 1 Then '< m_Stanza.Nstanza(isp) Then
                        Rflow = RelR - RelRS
                    Else
                        Rflow = RtoNext
                    End If
                    RtoNext = m_Data.Bcell(i, j, nvar2 + isc) * FlowoutRate(ieco) / (1 / (SurvRat + 1.0E-20) - 1)
                    RelR = RelRS
                    If m_Data.NewMultiStanza Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.PredCell(i, j, ieco) + 1.0E-30)
                        If ist > 1 Then Rflow = m_Data.Bcell(i, j, m_Stanza.EcopathCode(isp, ist - 1))
                    ElseIf m_Data.UseIBM = False And m_Data.NewMultiStanza = False Then
                        Cper(i, j, ieco) = Eatenby(ieco) / (m_Data.Bcell(i, j, nvar2 + isc) * PconSplit(isc) + 1.0E-30)
                    End If

                    F(i, j, nvar2 + isc) = Rflow
                    AMm(i, j, nvar2 + isc) = -FlowoutRate(ieco) - Bcw(i + 1, j, ieco) - C(i - 1, j, ieco) - d(i, j, ieco) - e(i, j, ieco)
                    If AMm(i, j, nvar2 + isc) >= 0 Then AMm(i, j, nvar2 + isc) = -1.0E+30

                    'm_Data.deriv2(i, j, nvar2 + isc) = m_Data.deriv(i, j, nvar2 + isc)
                    'm_Data.deriv(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) * m_Data.Bcell(i, j, nvar2 + isc) + F(i, j, nvar2 + isc) + Bcw(i, j, nvar2 + isc) * m_Data.Bcell(i - 1, j, nvar2 + isc) + C(i, j, nvar2 + isc) * m_Data.Bcell(i + 1, j, nvar2 + isc) + d(i, j - 1, nvar2 + isc) * m_Data.Bcell(i, j - 1, nvar2 + isc) + e(i, j + 1, nvar2 + isc) * m_Data.Bcell(i, j + 1, nvar2 + isc)

                    If m_Data.SpaceTime Then
                        F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + (1.3333 * m_Data.Bcell(i, j, nvar2 + isc) - 0.3333 * m_Data.Blast(i, j, nvar2 + isc)) / TimeStep2
                        'F(i, j, nvar2 + isc) = F(i, j, nvar2 + isc) + m_Data.Bcell(i, j, nvar2 + isc) / m_Data.TimeStep
                        AMm(i, j, nvar2 + isc) = AMm(i, j, nvar2 + isc) - 1 / TimeStep2
                        m_Data.Blast(i, j, nvar2 + isc) = m_Data.Bcell(i, j, nvar2 + isc)
                    End If
                Next
            Next

            '                    'For MPA Seed routine:
            '                    'At equilibrium 0 = dB = G - ZB, hence Bo = G/Z, where Bo is in    m_data.bcell() and Z=-AMm()
            '                    'For the no fishing situation: Bclose ~ Bo Z / (Z-F) or
            '                    'Bclose = -Bcell(i,j,ip) * AMm(i,j,ip) / (AMm(i,j,p) - Ftime(i,j,ip))
            '                    'This is the long-term predicted biomass in the cell from not fishing there
            '                    '   If AMm(i, j, ip) > 0 Then Bclose(i, j, ip) = -Bcell(i, j, ip) * AMm(i, j, ip) / (AMm(i, j, P) - Ftime(i, j, ip))

            Return True

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".SolveCell() Error: " & ex.Message)
        End Try

    End Function

    Private Sub derivtRed(ByVal Biomass() As Single, ByRef Flowin() As Single, ByRef FlowoutRate() As Single, ByRef EatEff() As Single, ByRef VulPred() As Single, ByVal RelProd As Single, ByVal iRow As Integer, ByVal iCol As Integer)
        'reduced derivatives for MPA equilibration procedure
        Dim i As Integer, j As Integer, ii As Integer
        Dim eat As Single, Pmult As Single
        'Dim Vprey As Single 'not used
        'Dim Shown As Boolean 'not used
        Dim SimGEt As Single
        Dim Dwe As Single
        Dim Bprey As Single

        Dim aeff() As Single, Veff() As Single
        ReDim aeff(m_SimData.inlinks), Veff(m_SimData.inlinks)

        Dim Hdent() As Single
        ReDim Hdent(m_Data.NGroups)

        'EwE5 ToDetritus() is declared at a global level
        'in EcoSpace this is the only place it is used so its scope is local to EcoSpace
        Dim ToDetritus() As Single
        ReDim ToDetritus(m_Data.NGroups)

        Try

            If m_SimData.MedIsUsed(0) Then SetMedFunctions(Biomass)

            setpred(Biomass)

            ReDim Eatenof(m_Data.NGroups)
            ReDim Eatenby(m_Data.NGroups)

            Dwe = 0.5

            'set ecosim nutrients
            NutBiom = 0
            For i = 1 To m_Data.NGroups
                NutBiom = NutBiom + Biomass(i)
            Next

            NutFree = m_SimData.NutTot * RelProd - NutBiom
            If NutFree < m_SimData.NutMin Then NutFree = m_SimData.NutMin

            '*************
            'Consumpt is NOT threadsafe
            '***********
            If m_SimData.IndicesOn Then
                ReDim m_SimData.Consumpt(m_Data.NGroups, m_Data.NGroups)
            End If

            For j = m_Data.nLiving + 1 To m_Data.NGroups
                ToDetritus(j - m_Data.nLiving) = 0
                'jb DetPassedOn() is not used anywhere
                ' DetPassedOn(j) = 0
            Next j

            SetRelaSwitch(Biomass)

            'get first estimate of denominators of predation rate disc equations
            Dim ia As Integer, Vbiom() As Single, Vdenom() As Single
            'this requires first estimates of vulnerable biomasses Vbiom by foraging arena
            ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)
            For ii = 1 To m_SimData.inlinks
                i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)

                aeff(ii) = m_Data.Aspace(ii) * Ftime(j) * RelaSwitch(ii) * EatEff(j) * VulPred(i)
                Veff(ia) = m_Data.Vspace(ia) * Ftime(i)
                ApplyAVmodifiers(aeff(ii), Veff(ia), i, m_SimData.Jarena(ia), False, iRow, iCol)  '?not sure this will work right with multiple preds in arenas
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * pred(j) / Hden(j)
            Next

            'then calculate first estimate using initial Hden estimates of vulnerable biomass in each arena
            For ia = 1 To m_SimData.Narena
                i = m_SimData.Iarena(ia)
                If m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next

            'then update hden estimates based on new vulnerable biomass estimates
            For ii = 1 To m_SimData.inlinks
                j = m_SimData.jlink(ii)
                ia = m_SimData.ArenaLink(ii)
                Hdent(j) = Hdent(j) + aeff(ii) * Vbiom(ia)
            Next

            For j = 1 To m_Data.NGroups
                Hden(j) = (1 - Dwe) * (1 + m_SimData.Htime(j) * Hdent(j)) + Dwe * Hden(j)
            Next

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'then update vulnerable biomass estimates using new Hden estimates (THIS MAY NOT BE NECESSARY?)
            ReDim Vbiom(m_SimData.Narena), Vdenom(m_SimData.Narena)
            For ii = 1 To m_SimData.inlinks
                i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)
                Vdenom(ia) = Vdenom(ia) + aeff(ii) * pred(j) / Hden(j)
            Next
            For ia = 1 To m_SimData.Narena
                i = m_SimData.Iarena(ia)
                If m_SimData.BoutFeeding Then
                    If Vdenom(ia) > 0 Then
                        Vbiom(ia) = Veff(ia) * Biomass(i) * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia)
                    Else
                        Vbiom(ia) = Veff(ia) * Biomass(i)
                    End If
                Else
                    Vbiom(ia) = Veff(ia) * Biomass(i) / (m_Data.Vspace(ia) + Veff(ia) + Vdenom(ia))
                End If
            Next
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'then predict consumption flows and cumulative consumptions using the new Vbiom estimates
            For ii = 1 To m_SimData.inlinks
                i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii) : ia = m_SimData.ArenaLink(ii)
                If m_SimData.TrophicOff Then Bprey = m_SimData.StartBiomass(i) Else Bprey = Biomass(i)

                'prey
                ' For j = 1 To N  'VC ignore detritus; CJW had NumGroups 'predator
                '    aeff = A(i, j) * tval(SeasonType(i, j)) * Ftime(j)
                '    Veff = vulrate(i, j) * Ftime(i) * MedVal(MF(i, j))
                Select Case m_SimData.FlowType(i, j) 'prey always first
                    Case 1 'donor controlled flow
                        eat = aeff(ii) * Bprey
                    Case 3 'limited total flow
                        'MsgBox ("invalid flow control type setting; edit your mdb")
                        eat = aeff(ii) * Bprey * pred(j) / (1 + aeff(ii) * pred(j) * Bprey / m_SimData.maxflow(i, j))
                    Case 2 'prey limited flow
                        'Vprey = Veff(ii) * Bprey / (vulrate(i, j) + Veff(ii) + aeff(ii) * pred(j) / Hden(j))
                        eat = aeff(ii) * Vbiom(ia) * pred(j) / Hden(j)
                    Case Else
                        eat = 0
                End Select
                Eatenof(i) = Eatenof(i) + eat
                Eatenby(j) = Eatenby(j) + eat

                'predation mort by link
                m_Data.MPred(iRow, iCol, ii) = eat / (Bprey + 1.0E-20)

                '******** 
                'THIS NEEDS TO CHANGE FOR THREADED STUFF
                '**********
                If m_SimData.IndicesOn Then m_SimData.Consumpt(i, j) = m_SimData.Consumpt(i, j) + eat

                'jb 
                If m_TracerData.EcoSpaceConSimOn = True Then
                    ' Debug.Assert(False, "Contaminant tracing not implemented in Ecospace")
                    'jb ConKtrophic will need to be local it is the rate of comsumption per unit of prey
                    If Biomass(i) > 0 Then m_ConTracer.ConKtrophic(ii) = eat / Biomass(i) Else m_ConTracer.ConKtrophic(ii) = 0
                End If

            Next

            'Make the detritus calculations here:
            m_Ecosim.SimDetritusMT(Biomass, Me.FishRateGear, Eatenby, Eatenof, ToDetritus, GroupDetritus)

            For i = 1 To m_Data.NGroups

                Eatenby(i) = Eatenby(i) + m_SimData.QBoutside(i) * Biomass(i)

                If i <= m_Data.nLiving Then      'Living group
                    Pmult = 1.0#
                    ApplyAVmodifiers(Pmult, Veff(1), i, i, False, iRow, iCol)
                    pbb(i) = Pmult * EatEff(i) * m_SimData.PBmaxs(i) * NutFree / (NutFree + m_SimData.NutFreeBase(i)) * m_SimData.pbm(i) / (1 + Biomass(i) * PbSpace(i))
                    'pbb becomes pbmaxs= pb times a max increase factor = pbm for consumers
                    loss(i) = Eatenof(i) + (m_SimData.mo(i) * (1 - m_SimData.MoPred(i) + m_SimData.MoPred(i) * Ftime(i)) + m_PathData.Emig(i) + FishTime(i)) * Biomass(i)
                    'deriv(i) = Immig(i) + Biomass(i) * pbb(i) + simGE(i) * Eatenby(i) - loss(i)
                    'biomeq(i) = (Immig(i) + simGE(i) * Eatenby(i) + pbb(i) * Biomass(i)) / (loss(i) / Biomass(i))

                    'jb change layout so I could read it
                    'SimGEt = IIf(m_ESData.UseVarPQ And m_EPdata.vbK(i) > 0, m_ESData.AssimEff(i) * loss(i) / Biomass(i) / (loss(i) / Biomass(i) + 3 * m_EPdata.vbK(i)), m_ESData.SimGE(i))
                    If m_SimData.UseVarPQ And m_PathData.vbK(i) > 0 Then
                        SimGEt = m_SimData.AssimEff(i) * loss(i) / Biomass(i) / (loss(i) / Biomass(i) + 3 * m_PathData.vbK(i))
                    Else
                        SimGEt = m_SimData.SimGE(i)
                    End If

                    Flowin(i) = m_PathData.Immig(i) + SimGEt * Eatenby(i) + pbb(i) * Biomass(i)

                    If Biomass(i) > 1.0E-20 Then
                        FlowoutRate(i) = loss(i) / Biomass(i)
                    Else
                        FlowoutRate(i) = 100
                    End If
                    'If Abs(Flowin(i) - loss(i)) > 0.1 * loss(i) Then Stop
                Else                'Detritus group
                    loss(i) = Eatenof(i) + m_PathData.Emig(i) + m_SimData.DetritusOut(i) * Biomass(i)
                    'deriv(i) = Immig(i) + ToDetritus(i - n) - loss(i)
                    If loss(i) <> 0 And Biomass(i) > 0 Then
                        'biomeq(i) = (Immig(i) + ToDetritus(i - n)) / (loss(i) / Biomass(i))
                        Flowin(i) = (m_PathData.Immig(i) + ToDetritus(i - m_Data.nLiving))
                        FlowoutRate(i) = loss(i) / Biomass(i)
                    Else
                        Flowin(i) = 1.0E-20
                        'VC160398 below FlowoutRate(i) was set to 100 before
                        If Biomass(i) > 0 Then
                            FlowoutRate(i) = Flowin(i) / Biomass(i)
                        Else
                            FlowoutRate(i) = 0.0000000001
                        End If
                    End If
                End If
            Next

        Catch ex As Exception
            '   Debug.Assert(False)
            Throw New ApplicationException(Me.ToString & ".derivtRed() Error: " & ex.Message)
        End Try
    End Sub
    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets RelaSwitch() 
    ''' </summary>
    ''' <param name="B">Biomass at this time step for this spatial cell</param>
    ''' <remarks>Sets RelaSwitch() using local B() and  Ecosim.A(), Ecosim.SwitchPower(), Ecosim.BaseTimeSwitch()  </remarks>
    Sub SetRelaSwitch(ByVal B() As Single)     'Switching
        Dim i As Integer, j As Integer, ii As Integer
        Dim PredDen() As Double

        ReDim PredDen(m_Data.NGroups)
        ReDim RelaSwitch(m_SimData.inlinks)

        'throw an error for error testing
        'PredDen(m_Data.NGroups + 1) = 0

        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii)
            PredDen(j) = PredDen(j) + m_Ecosim.A(i, j) * B(i) ^ m_SimData.SwitchPower(j)
        Next
        For ii = 1 To m_SimData.inlinks
            i = m_SimData.ilink(ii) : j = m_SimData.jlink(ii)
            If m_SimData.SwitchPower(j) = 0.0# Then
                RelaSwitch(ii) = 1
            Else
                RelaSwitch(ii) = m_Ecosim.A(i, j) * B(i) ^ m_SimData.SwitchPower(j) / (PredDen(j) + 1.0E-20) / m_SimData.BaseTimeSwitch(ii)
            End If
        Next

    End Sub
    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    Sub setpred(ByVal Biomass() As Single)
        'Routine modified 290597 VC to follow ESimII
        Dim i As Integer ', ii As Integer
        'set predator abundance measure used for predation
        'rate calculations; this is just biomass for
        'simple pools, or predator numbers for pools that
        'are split into Juv-Adult pairs
        'note below that biomass(ii) for ii>n contains
        'numbers in pools iad, iju rather than biomasses
        For i = 1 To m_Data.NGroups
            'If i > N And biomass(i) = 0 Then biomass(i) = 1
            If Biomass(i) < 1.0E-20 Then Biomass(i) = 1.0E-20 '0.00000001
            If m_SimData.NoIntegrate(i) >= 0 Then pred(i) = Biomass(i)
        Next

    End Sub

    '***********************
    'THIS FUNCTION IS COPIED FROM cEcoSimModel.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Sets MedVal() mediation value used to modify a or v. Local version for thread safety.
    ''' </summary>
    ''' <param name="Biom"></param>
    ''' <remarks>MedVal(nmediationshapes) is used in ApplyAVmodifiers()</remarks>
    Sub SetMedFunctions(ByVal Biom() As Single)
        'called from derivt, derivtred if MedIsUsed(0)=true to set
        'current Y value of each active trophic mediation function
        Dim i As Integer, j As Integer, MedX As Single, ip As Long

        For i = 1 To m_SimData.MediationShapes
            If m_SimData.MedIsUsed(i) Then
                MedX = 0.0000000001
                For j = 1 To m_SimData.NMedXused(i)
                    If m_SimData.IMedUsed(j, i) <= m_Data.NGroups Then
                        MedX = MedX + Biom(m_SimData.IMedUsed(j, i)) * m_SimData.MedWeights(m_SimData.IMedUsed(j, i), i)
                    Else    'a fleet
                        'ToDo_jb SetMedFunctions() uses timeNow as an array index for FishRateGear() this should be iMonth
                        MedX = MedX + FishRateGear(m_SimData.IMedUsed(j, i) - m_Data.NGroups, m_Data.TimeNow) * m_SimData.MedWeights(m_SimData.IMedUsed(j, i), i)
                    End If
                Next
                '060328 CJW found that without the +0.01 below it could be unstable when slope
                'was large around Ecopath base point in mediation function, causing instability.
                'This solves it. VC.
                ip = Int(m_SimData.IMedBase(i) * MedX / m_SimData.MedXbase(i) + 0.01)
                If ip < 1 Then ip = 1
                If ip > m_SimData.NMedPoints Then ip = m_SimData.NMedPoints
                MedVal(i) = m_SimData.Medpoints(ip, i) / m_SimData.MedYbase(i)
            End If
        Next

    End Sub
    '***********************
    'THIS FUNCTION IS COPIED from cSpaceSolver.vb
    'Changes here will NOT copy over to there
    '***********************
    ''' <summary>
    ''' Apply the multi function mediation functions/modifiers to 'a'(searchrate) and 'v'(vulnerability)
    ''' uses MedVal(NMediationShapes) to modify A and/or V
    ''' </summary>
    ''' <param name="A">SearchRate to modify</param>
    ''' <param name="v">Vulnerability to modify</param>
    ''' <param name="i">i Index (Prey)</param>
    ''' <param name="j">j Index (Pred)</param>
    ''' <param name="UseTime">True if the modifier is over time (Ecosim), False if not (Ecospace) </param>
    ''' <remarks>
    ''' THREADING:  MedVal() is set to the mediating value based on biomass for each map cell at each time step via cSpaceSolver.SetMedFunctions().
    ''' It is unique to this thread/cell/time-step. It was moved here to make it thread safe.
    '''</remarks>
    Sub ApplyAVmodifiers(ByRef A As Single, ByRef v As Single, ByVal i As Integer, ByVal j As Integer, ByVal UseTime As Boolean, ByVal iRow As Integer, ByVal iCol As Integer)
        Dim K As Integer, Mult As Single
        'VC Hobart Sep 2008. Added row and col numbers to the call to this routine, as they are needed for spatial fields

        'VC Hobart Sep 2008. Adding temperature and salinity fields to Ecospace,
        'for now it's just readable in code, we'll need interface and database handling as well

        If m_Data.SpatialFieldsInUse Then
            For iSF As Integer = 1 To m_Data.nSpatialFields
                m_Ecosim.ApplySalinityModifier(A, m_Data.SpatialField(iRow, iCol, i), _
                                               m_Data.SpatialFieldOptimum(i, iSF), _
                                               m_Data.SpatialFieldStdLeft(i, iSF), _
                                               m_Data.SpatialFieldStdRight(i, iSF))
            Next
        End If


        For K = 1 To m_SimData.MaxFunctions

            If m_SimData.FunctionNumber(i, j, K) = 0 Then Exit Sub

            If m_SimData.IsMedFunction(i, j, K) Then
                Mult = MedVal(m_SimData.FunctionNumber(i, j, K))
            Else
                Mult = 1
                'If UseTime = True Then Mult = m_ESData.tval(m_ESData.FunctionNumber(i, j, K)) Else Mult = 1
            End If

            Select Case m_SimData.FunctionType(i, j, K)
                Case 1 'multiply rate of search
                    A = A * Mult
                Case 2 'multiply vulnerability
                    v = v * Mult
                Case 3 'multiply foraging area
                    A = A / (Mult + 0.0000000001)
                Case 4 ' multiply foraging area and vulnerability
                    A = A / (Mult + 0.0000000001)
                    v = v * Mult
            End Select

        Next

    End Sub

    Public Sub New(ByVal ThreadNumber As Integer)

        ThreadID = ThreadNumber

        m_ConTracer = New cContaminantTracer
        'create a new tracer data structure
        'this will get a copy of the data that has been initialized by the database in cEcospace.InitSpaceSolverThreads()
        m_TracerData = New cContaminantTracerDataStructures

        isOkToRun = True

    End Sub

End Class
