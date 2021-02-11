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

Option Explicit On
Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cEcoNetwork

#Region " Private data "

    Private m_manager As cNetworkManager

    Private m_epdata As cEcopathDataStructures
    Private m_esdata As cEcosimDatastructures
    Private m_core As cCore

    Private CCD As Integer

    Private Ascend As Single

    Private ImpDet() As Single

    Private CatchSum As Single
    'Private DetIndex As Single 'Proportion of total flow originating from detritus: DetIndex.

    'Private SumDC() As Single

    Private RaiEm1() As Single 'This is for raising flows
    Private RaiEm2() As Single  'This is for raising emergy flows

    'edited: 013197 VC
    Private BenificialPredation(,) As Boolean

    'Private TrEm1() As Single
    Private TrEm2() As Single

    '************************************************
    'edited as VC's advice eli 052396.
    'private  TRP()  As Single
    'private  TRPD()  As Single
    'Private TrpShow() As Single
    'end of modif.....
    '************************************************

    Private TrPut() As Double

    Private F(,) As Single

    'From Lindeman
    Private ADj() As Single
    Private APj() As Single
    Private A1() As Single
    Private A2() As Single
    'Private CA() As Single
    'Private CAD() As Single
    Private CycDC(,) As Single
    'This array contains the amount that is to be subtracted
    'from diet compositions after conversion to proportion of diet
    'in order to break all cycles by removing the min amount circulated.
    'The amount is calculated in the FindCycles module
    'private  Cons(0 To N) As Single
    Private DCNoCyc(,) As Single
    Private G2(,) As Single
    Private G3(,) As Single
    'private  TrPut(0 To  ) As Double
    Private HNoC() As Single
    Private LastComp() As Integer
    Private last() As Integer
    Private Path() As Integer
    Private PredatOn() As Single
    Private PredNoC() As Single
    Private SumCycDC() As Single
    Private TropLvl() As Single
    'Private TL() As Single

    'added by eli 052396
    Public Ad(,) As Single
    Private Ap(,) As Single
    Private TLsort() As Single

    Private Cons() As Single

    Private AUL(,) As Single

    Private E() As Single

    Private DCC(,) As Single 'DCC is "Diet Composition of Catch" by gear

    Dim pivot As Integer
    Dim Level As Integer

    Dim aa As Integer

    Private DoneAlready As Boolean
    Private MinCons As Single
    Private FoundCycles As Boolean

    Private ThruPut As Single

    'for requiredPP
    Private Flow(,) As Single
    Private FlowCyc(,) As Single
    Private SumRaise(,) As Single
    'Private NumDetPath As Integer
    'Private NumLivPath As Integer
    Private NumOfPaths As Integer
    Private Sel As Integer 'jb I don't think this is used anymore In EwE5 it is set from PrepareReqPPDetails()
    Private m_GroupsToShow() As Boolean

    Private m_AbortTimer As System.Timers.Timer

    ''' <summary>Abort Timer timed out. Used to post a message at the end of a run.</summary>
    Private m_timedOut As Boolean

    ''' <summary>
    ''' Number of milliseconds to wait for the Network Analysis to complete before it times out.
    ''' </summary>
    ''' <remarks>This is only effective if <see cref="bUseAbortTimer">bUseAbortTimer</see> = True. Default of 30 minutes</remarks> 
    Public Property TimeOutMilSecs As Long = 30 * 60 * 1000 '30min * 60sec * 1000milsec

#Region " Ecosim "

    'biomass computed by Ecosim at the current time step
    'Ecoism.BB(ngroups) is private to access it from this plugin it gets passed as an agrument at each time step (see EcosimTimestep())
    'then copied into a local BB() array
    Private BB() As Single
    Private OrigDiversityIndex As Single
    Private OrigPPR(1) As Single

#End Region ' Ecosim

#End Region ' Private data

#Region " Public data "

    ''' <summary>
    ''' Stop the network annalysis routine from running
    ''' </summary>
    ''' <remarks>In EwE5 this was called AbortRun and was global. It stopped all kinds of things.</remarks>
    Public bStopNetworkAnnalysis As Boolean

    Public PPRon As Boolean

#Region " Flows (Trophic level decomposition) "

    'See DisplayTransMatrix()
    Public AM(,) As Single 'Trophic level decomposition / Relative Flows tab
    Public AM_Abs(,) As Single 'Trophic level decomposition / Absolute Flows Tab
    Public QTL() As Single 'sum of absolute flow across all the groups for a trophic level AM_Abs(grp,trophiclevel)
    Public CA() As Single 'Trophic level decomposition / Used in computing Transfer Efficiency
    Public CAD() As Single 'Trophic level decomposition / Used in computing Transfer Efficiency
    Public DetIndex As Single 'Proportion of total flow originating from detritus: DetIndex.
    Public BbyTL() As Single 'Trophic level decomposition / Biomass by trophic level
    'Public BbyGp() As Single 'Trophic level decomposition / Biomass by group
    Public CbyTL() As Single 'Trophic level decomposition / Catch by trophic level
    'Public CbyGp() As Single 'Trophic level decomposition / Catch by group
    Public NoTL As Integer 'number of trophic levels

#End Region ' Flows (Trophic level decomposition)

#Region " Ascendancy "

    'see EwE5 DisplayAscendency()
    Public Ao As Single
    Public Ai As Single
    Public Ae As Single
    Public Ar As Single
    Public Aop As Single
    Public Aip As Single
    Public Aep As Single
    Public Arp As Single
    Public Eo As Single
    Public Ei As Single
    Public Eee As Single
    Public er As Single
    Public Eop As Single
    Public Eip As Single
    Public Eep As Single
    Public Erp As Single
    Public Co As Single
    Public Ci As Single
    Public Ce As Single
    Public Cr As Single
    Public Cop As Single
    Public Cip As Single
    Public Cep As Single
    Public Crp As Single
    Public Ascen As Single
    Public Overhead As Single
    Public Capacity As Single
    Public Ascp As Single
    Public Overp As Single
    Public Capp As Single

    Public Ac() As Single
    Public Ec() As Single
    Public CC() As Single

    Public Q() As Double

    'see EwE5 DisplayFlowIndices
    Public TruPut As Single, SumAc As Single, SumEc As Single, SumCc As Single
    Public SumEx As Single, SumResp As Single, Tc As Single, TCyc As Single, TcD As Single

#End Region ' Ascendancy

#Region " Keystoneness "

    Public KeystoneIndex1() As Double
    Public KeystoneIndex2() As Double
    Public KeystoneIndex3() As Double
    Public RelTotalImpact() As Double

#End Region ' Keystoneness

#Region " Flows and Biomass "

    'see EwE5 DisplayFlows
    'Flows and Biomass /From primary producer (tabs)
    Public Impo() As Single 'Import 
    Public DTA() As Single 'flow to det
    Public EXA() As Single 'Export
    Public TRP() As Single 'throughput
    Public Predat() As Single 'Cons by pred
    Public RSP() As Single 'respiration

    Public TrpShow() As Single 'throughput shown
    Public TrEm1() As Single

    'edited: 013097 VC
    Public FC(,) As Single
    'FC is the hostmatrix(host, predator)=predator composition

    'Flows and Biomass /From detritus (tabs)
    Public ImpD() As Single 'import 
    Public PredatD() As Single 'Cons to pred

    Public RSPD() As Single 'respiration
    Public DTAD() As Single 'Flow to det
    Public EXAD() As Single 'Export
    Public TRPD() As Single 'throughput

    Public SumIm As Single 'Input TL II+ (not in throughput)
    Public AmCyc As Single 'Extracted to break cycles
    Public TotalTrp As Single

    Public MTI(,) As Single 'mixed trophic impact (tab)

#End Region ' Flows and Biomass

#Region " PPR "

    Public NumDetPath As Integer
    Public NumLivPath As Integer

    'these varables are calculated in CalculateRequiredPP

    Public SumDetRequired(,) As Single
    Public SumPPRequired(,) As Single
    Public RaiseToDet(1) As Single
    Public RaiseToPP(1) As Single

    Public totalPP As Single
    Public DetFlow As Single
    Public totalCatch As Single

    Public NumPath() As Integer

    Public Topic As Integer

#End Region ' PPR

#Region " Cycles and Pathways "

    Public lstPathways As New List(Of String)
    Public NoArrows As Long 'This declared NoArrows is used only in PathPrintReqPP()  joeh
    ' The NoArrows in FindCycles(), PrintPath(), PrintCycle() and PreyProd()
    ' is NOT the NoArrows above but a variable passed from its caller.  The NoArrows in 
    ' FindCycles() is exposed as NumberArrows below
    Public NumberArrows As Integer

#End Region ' Cycles and Pathways

#Region " Ecosim "

    Private ByTL(,,) As Single

    Public RelativeSumOfCatch() As Single
    Public RelativeDiversityIndex() As Single
    Public RelativeLIndex() As Single
    Public AbsoluteLIndex() As Single
    Public RelativePsust() As Single
    Public AbsolutePsust() As Single

    'Trophic level of catch
    Public TLCatch() As Single

    Public TLSim(,) As Single 'groups,time

    Public RelativeCatchPPR() As Single
    Public RelativeCatchDetReq() As Single

    Public Throughput() As Single
    Public CapacityEcosim() As Single
    Public AscendImport() As Single
    Public AscendFlow() As Single
    Public AscendExport() As Single
    Public AscendResp() As Single
    Public OverheadImport() As Single
    Public OverheadFlow() As Single
    Public OverheadExport() As Single
    Public OverheadResp() As Single
    Public PCI() As Single
    Public FCI() As Single
    Public PathLength() As Single
    Public Export() As Single
    Public Resp() As Single
    Public PrimaryProd() As Single
    Public Prod() As Single
    Public Biomass() As Single
    Public CatchEcosim() As Single
    Public PropFlowDet() As Single
    Public RaiseToPPEcosim() As Single
    Public RaiseToDetEcosim() As Single
    Public Ascendency() As Single
    Public AMI() As Single
    Public Entropy() As Single
    Public TotTransferEfficiency() As Single
    Public DetTransferEfficiency() As Single
    Public PPTransferEfficiency() As Single

    Public PPTransferEfficiencyWeighted() As Single
    Public DetTransferEfficiencyWeighted() As Single
    Public TotTransferEfficiencyWeighted() As Single

#End Region ' Ecosim

#End Region ' Public data

#Region " Constructor "

    Public Sub New(ByRef Manager As cNetworkManager) 'joeh
        Me.m_manager = Manager
        Me.m_core = Me.m_manager.Core
        Me.OrigDiversityIndex = Single.Epsilon 'for /0 error
    End Sub

#End Region ' Constructor

#Region " Public Properties "

    Public Property GroupsToShow() As Boolean()
        Get
            Return Me.m_GroupsToShow
        End Get
        Set(value As Boolean())
            Me.m_GroupsToShow = value
        End Set
    End Property

    Public Property EcopathData() As cEcopathDataStructures
        Get
            Return Me.m_epdata
        End Get
        Set(value As cEcopathDataStructures)
            Me.m_epdata = value
        End Set
    End Property

    Public Property EcosimData() As cEcosimDatastructures
        Get
            Return Me.m_esdata
        End Get
        Set(value As cEcosimDatastructures)
            Me.m_esdata = value
        End Set
    End Property

    ''' <summary>Use the Abort Timer to abort a run after <see cref="TimeOutMilSecs">time out in Milliseconds</see></summary>
    ''' <remarks>
    ''' False by default. The AbortTimer works in the Scientific interface but needs an interface to turn it on/off and set the TimeOutMilSecs. 
    ''' At this time this can only be used from code.
    ''' </remarks>
    Public Property bUseAbortTimer As Boolean = My.Settings.UseAbortTimer

#End Region ' Public Properties

#Region " Network Analysis "

    Private Sub startAbortTimer()

        If Not Me.bUseAbortTimer Then Exit Sub

        Try

            'Create or start a new timer
            If Me.m_AbortTimer Is Nothing Then
                Me.m_AbortTimer = New System.Timers.Timer(Me.TimeOutMilSecs)
                Me.m_AbortTimer.AutoReset = False
                Me.m_AbortTimer.Start()
            Else
                Me.m_AbortTimer.Stop()
                Me.m_AbortTimer.Interval = Me.TimeOutMilSecs
                Me.m_AbortTimer.Start()
            End If

            Me.m_timedOut = False
            AddHandler Me.m_AbortTimer.Elapsed, AddressOf Me.OnAbortTimerEvent

        Catch ex As Exception

        End Try

    End Sub

    Private Sub stopAbortTimer()

        If Not Me.bUseAbortTimer Then Exit Sub

        Try
            Debug.Assert(Me.m_AbortTimer IsNot Nothing, Me.ToString + " abort timer has not been set!")

            If Me.m_timedOut Then
                Me.SendMessage(New cMessage("Sorry Network Analysis timed out after " + (Me.TimeOutMilSecs / 60 / 1000).ToString + " minutes. Results will not be displayed.",
                                            eMessageType.ErrorEncountered, EwEUtils.Core.eCoreComponentType.EcoPath, eMessageImportance.Warning))
            End If

            Me.m_AbortTimer.Close()
            RemoveHandler Me.m_AbortTimer.Elapsed, AddressOf Me.OnAbortTimerEvent

        Catch ex As Exception

        End Try
    End Sub


    Private Sub OnAbortTimerEvent(source As Object, e As System.Timers.ElapsedEventArgs)

        If Not Me.bUseAbortTimer Then Exit Sub

        Me.bStopNetworkAnnalysis = True
        Me.m_timedOut = True
        Console.WriteLine("The abortTimer.Elapsed event was raised at {0}", e.SignalTime)
    End Sub

    Public Function IsTimedOut() As Boolean
        Return Me.m_timedOut
    End Function

    ''' <summary>
    ''' Run the Main Network Analysis routine
    ''' </summary>
    ''' <remarks>In EwE5 this was called EconetMain(). 
    ''' This runs the initial routines need for all the Networking output that is not part of the EwE5 'Cycles and pathway' menu. 
    ''' Required PP is computed from <see>CalculateRequiredPP()</see>  </remarks>
    Public Function RunNetworkAnalysis() As Boolean
        'this computes the values for the following tabs
        'Trophic level decomposition
        'Flows and biomass
        'Mixed trophic impact
        'Ascencdency
        'Flow from detritus

#If DEBUG Then
        ' Profiler bit
        Console.WriteLine("NA started at " & Date.Now.ToLongTimeString)
#End If

        Dim i As Integer ', chk As Integer
        Dim strErr As String = ""
        Dim bSucces As Boolean = True

        Debug.Assert(Me.m_epdata IsNot Nothing, Me.ToString & ".RunNetworkAnalysis() Ecopath data has not been initialized.")
        Try

            If Me.m_epdata Is Nothing Then
                Return False
            End If

            cApplicationStatusNotifier.StartProgress(Me.m_core, My.Resources.STATUS_RUNNING_NA)
            Me.FoundCycles = False
            Me.NetworkDimensioning()

            ' EconetMain_g% = 1
            'jb this is weird I can’t put a Try Catch statement into either Ulanow or Lindeman   
            ' JS: that's because of the presence of labels. We *really* should get rid of those
            Try
                cApplicationStatusNotifier.UpdateProgress(Me.m_core, My.Resources.STATUS_RUNNING_ULANOW, 0.2)
                strErr = "Ulanow()"
                Me.Ulanow(Me.m_epdata.B, Me.m_epdata.PB, Me.m_epdata.QB, Me.m_epdata.EE, Me.m_epdata.DC, Me.ImpDet, Me.m_epdata.Ex, Me.m_epdata.Resp)

                ' JS: Lindeman should not be part of the main ENA run, but should be requested separately. Cycle computations are killers
                Me.startAbortTimer()

                cApplicationStatusNotifier.UpdateProgress(Me.m_core, My.Resources.STATUS_RUNNING_LINDEMAN, 0.3)
                strErr = "Lindeman()"
                Me.Lindeman(Me.m_epdata.B, Me.m_epdata.PB, Me.m_epdata.QB, Me.m_epdata.EE, Me.m_epdata.DC, Me.ImpDet, Me.m_epdata.Ex, Me.m_epdata.Resp) '
                cApplicationStatusNotifier.UpdateProgress(Me.m_core, My.Resources.STATUS_RUNNING_NA, 0.4)

                Me.stopAbortTimer()

            Catch ex As Exception
                cLog.Write(ex)
                bSucces = False
            End Try

            'if Lindeman timed out then don't do the processing and return False
            'Me.stopAbortTimer() will send a message
            bSucces = bSucces And (Not Me.m_timedOut)

            If (bSucces) Then

                ' PROCEED TO THE MIXED TROPHIC IMPACT ROUTINE
                Me.CatchSum = 0


                For i = 1 To Me.m_epdata.NumLiving
                    Me.CatchSum = Me.CatchSum + Me.m_epdata.Landing(0, i) + Me.m_epdata.Discard(0, i) 'Catch(i%)
                Next i
                If Me.CatchSum > 0 Or Me.m_epdata.NumFleet > 0 Then
                    Me.CCD = 1
                    ReDim Me.MTI(Me.m_epdata.NumGroups + Me.m_epdata.NumFleet, Me.m_epdata.NumGroups + Me.m_epdata.NumFleet)
                Else
                    Me.CCD = 0
                    ReDim Me.MTI(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
                End If

                Me.Impacts()

                'VC090527 Adding calculation of keystoneness based on Libralato et al 2006:
                Me.Keystoneness()

                cApplicationStatusNotifier.UpdateProgress(Me.m_core, My.Resources.STATUS_RUNNING_EQPP, 0.6)
                Me.InitReqPP()

                'asn.SetStatusText("Finalizing Network Analysis run...", 0.8)
                'activate the textStream to save the output to file
                'Dim textStream As New System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory() & "NetworkOutput.csv")
                'DumpResultsToStream(Nothing)
                'Me.m_manager.UpdateNetworkAnalysis(6)

            End If

        Catch ex As Exception
#If DEBUG Then
            ' Profiler bit
            Console.WriteLine("NA screwed up at " & Date.Now.ToLongTimeString)
#End If
            ' Debug.Assert(False, ex.Message)
            cLog.Write(ex)
            bSucces = False
        End Try
#If DEBUG Then
        ' Profiler bit
        Console.WriteLine("NA ended at " & Date.Now.ToLongTimeString)
#End If

        cApplicationStatusNotifier.EndProgress(Me.m_core)

        Return bSucces
        'Count = 2
    End Function


    ''' <summary>
    ''' Initialization of the Required PP routines
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Renamed from ReqPPMain() in EwE5. This needs to run after RunNetworkAnalysis and before CalculateRequiredPP</remarks>
    Private Function InitReqPP() As Boolean
        'Function ReqPPMain() As Integer 'EwE5 name

        Dim SumDC(Me.m_epdata.NumGroups) As Single

        Try

            'prepares the buffers.
            ReDim Me.DCNoCyc(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
            ReDim Me.CycDC(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
            ReDim Me.ImpDet(Me.m_epdata.NumGroups)
            ReDim Me.LastComp(2 * Me.m_epdata.NumGroups + 1)
            ReDim Me.PredatOn(Me.m_epdata.NumGroups)
            ReDim Me.SumCycDC(Me.m_epdata.NumGroups)
            ReDim Me.TrPut(Me.m_epdata.NumGroups + 3)
            ReDim Me.Path(2 * Me.m_epdata.NumGroups + 2)
            ReDim Me.Cons(Me.m_epdata.NumLiving)

            ReDim Me.Flow(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            ReDim Me.FlowCyc(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            ReDim Me.NumPath(Me.m_epdata.NumLiving)
            ReDim Me.SumRaise(1, Me.m_epdata.NumLiving)

            'jb CheckIfPPisInput() uses the missing() array that is private to Ecopath
            'this is the number of missing parameters in the Ecopath inputs
            'If there are any missing parameters this can not be run so calling CheckIfPPisInput() is pointless in this version
            'CheckIfPPisInput(NoPP)
            Me.CalcTotalPP()

            'default return value.

            'validates data.
            If Me.totalPP = 0 Then
                Me.SendMessage(New cMessage(My.Resources.PROMPT_ERROR_PP, eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Warning))
                Return False
            End If

            'continue computing.
            Me.CalcDetFlow(Me.DetFlow)
            Me.FlowInternal(Me.Flow, Me.FlowCyc)
            Me.ImportAmount()
            Me.FlowExternal(Me.Flow, Me.FlowCyc)
            Me.SumDiet(SumDC)
            Me.ThruputByGroup()
            'If FoundCycles = False Then FindCycles(Cons)  'Joeh
            Me.FoundCycles = True
            Me.CalcCycDC(Me.CycDC)
            Me.CalcSumCycDC(Me.CycDC)
            Me.CalcDCNoCyc(Me.CycDC, SumDC)
            Me.EstimTotalCatch()
            Return True

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("InitReqPP()" & ex.Message, ex)
        End Try

    End Function


    ''' <summary>
    ''' Dump the ouput for debugging to the Console or a text stream
    ''' </summary>
    ''' <param name="textStream">Nothing for output to the console. A valid text stream for writting to file</param>
    ''' <remarks>This can write to the console or a file stream</remarks>
    Public Sub DumpResultsToStream(Optional ByRef textStream As System.IO.TextWriter = Nothing)

        Dim textStreamOrg As System.IO.TextWriter = System.Console.Out

        If textStream IsNot Nothing Then
            System.Console.SetOut(textStream)
        End If

        System.Console.WriteLine("---------------Network Analysis-------------------")
        System.Console.WriteLine("Trophic level decomposition / Absolute flows")

        For igrp As Integer = 1 To Me.m_epdata.NumGroups
            System.Console.Write(Me.m_epdata.GroupName(igrp))
            For iTl As Integer = 1 To Me.NoTL
                System.Console.Write(", " & Me.AM(igrp, iTl).ToString("0.000"))
            Next
            System.Console.Write(vbNewLine)
        Next

        System.Console.WriteLine("--------Mixed trophic impacts----------")
        For row As Integer = 1 To Me.m_epdata.NumGroups
            System.Console.Write(Me.m_epdata.GroupName(row))
            For col As Integer = 1 To Me.m_epdata.NumGroups 'CCD%
                System.Console.Write(", " & Me.MTI(row, col).ToString("0.000"))
            Next
            System.Console.Write(vbNewLine)
        Next

        ' Restore original text stream
        If (textStream IsNot Nothing) Then
            System.Console.SetOut(textStreamOrg)
        End If

    End Sub


    Private Sub NetworkDimensioning()
        Try
            ReDim Me.Cons(Me.m_epdata.NumLiving)
            ReDim Me.RaiEm1(Me.m_epdata.NumGroups)  'This is for raising flows
            ReDim Me.RaiEm2(Me.m_epdata.NumGroups)   'This is for raising emergy flows

            'edited: 013197 VC
            ReDim Me.BenificialPredation(Me.m_epdata.NumGroups + Me.m_epdata.NumFleet, Me.m_epdata.NumGroups + Me.m_epdata.NumFleet)
            If Me.m_epdata.NumFleet > 3 Then
                ReDim Me.MTI(Me.m_epdata.NumGroups + Me.m_epdata.NumFleet, Me.m_epdata.NumGroups + Me.m_epdata.NumFleet)
            Else
                ReDim Me.MTI(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            End If

            ReDim Me.AM(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
            ReDim Me.AM_Abs(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
            ReDim Me.DTA(Me.m_epdata.NumGroups + 1)
            ReDim Me.DTAD(Me.m_epdata.NumGroups + 1)
            'ReDim DTD( m_epdata.NumGroups + 1) 
            ReDim Me.EXA(Me.m_epdata.NumGroups)
            ReDim Me.EXAD(Me.m_epdata.NumGroups)

            'edited: 013097 VC
            ReDim Me.FC(Me.m_epdata.NumGroups + Me.m_epdata.NumFleet, Me.m_epdata.NumGroups + Me.m_epdata.NumFleet)
            'FC is the hostmatrix(host, predator)=predator composition
            ReDim Me.RSP(Me.m_epdata.NumGroups)
            ReDim Me.RSPD(Me.m_epdata.NumGroups)
            ReDim Me.TrEm1(Me.m_epdata.NumGroups)
            ReDim Me.TrEm2(Me.m_epdata.NumGroups)

            '************************************************
            'edited as VC's advice eli 052396.
            'ReDim TRP( m_epdata.NumGroups)  
            'ReDim TRPD( m_epdata.NumGroups)  
            ReDim Me.TRP(Me.m_epdata.NumGroups + 2)
            ReDim Me.TRPD(Me.m_epdata.NumGroups + 2)
            ReDim Me.TrpShow(Me.m_epdata.NumGroups + 2)
            'end of modif.....
            '************************************************

            ReDim Me.TrPut(Me.m_epdata.NumGroups + 3)

            ReDim Me.Q(Me.m_epdata.NumGroups + 10)
            ReDim Me.F(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
            'ReDim SumDC(m_epdata.NumGroups)

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            Throw New ApplicationException("NetworkDimensioning() " & ex.Message)
        End Try

    End Sub

    Private Sub Lindeman(ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef im() As Single, ByRef Ex() As Single, ByRef Resp() As Single)
        'mess$ = "The following is a trophic aggregation routine"
        'mess$ = mess$ & " For description see Ulanowicz, R.E., Ecosystem Trophic"
        'mess$ = mess$ & " Foundations: Lindeman Exonerata. in E.A. Halfon (ed.)."
        'mess$ = mess$ & " Theoretical Systems Ecology. Acad. Press, New York. (in press),"
        'mess$ = mess$ & " and the ECOPATH II Manual (Version 2.0)"
        'MsgBox mess$

        Dim i As Integer
        Dim j As Integer
        Dim K As Integer
        Dim ii As Integer
        Dim SumDC(Me.m_epdata.NumGroups) As Single

        Dim Consu As Single, T1 As Double, T2 As Double, Tr1 As Single, TotTr As Single, TotalB As Single

        Dim ErrCode As Integer, SumTrp As Single, Sum As Single
        Dim SumTrpD As Single, DTDSum As Single, DDT As Single
        Dim lines As Integer, TrpNoIM As Single, Dead As Single
        Dim TLChk As Integer
        'jb not used
        ' dim  PriVar, num, Dim MinTL  Dim Grp As Integer   Dim kk As Integer
        '*************************************************
        'modified as VC's advice eli 052396
        'ReDim Ad( m_epdata.NumGroups, m_epdata.NumGroups) As Single
        'ReDim Ap( m_epdata.NumGroups, m_epdata.NumGroups) As Single
        'ReDim BbyTL( m_epdata.NumGroups)
        'ReDim TLsort( m_epdata.NumGroups) As Single

        ReDim Me.ADj(Me.m_epdata.NumGroups)
        ReDim Me.APj(Me.m_epdata.NumGroups)
        ReDim Me.A1(Me.m_epdata.NumGroups)
        ReDim Me.A2(Me.m_epdata.NumGroups)
        ReDim Me.CA(Me.m_epdata.NumGroups)
        ReDim Me.CAD(Me.m_epdata.NumGroups)
        ReDim Me.CycDC(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
        'This array contains the amount that is to be subtracted
        'from diet compositions after conversion to proportion of diet
        'in order to break all cycles by removing the min amount circulated.
        'The amount is calculated in the FindCycles module
        'ReDim Cons(0 To N) As Single
        ReDim Me.DCNoCyc(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
        ReDim Me.G2(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
        ReDim Me.G3(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups + 1)
        'ReDim TrPut(0 To m_epdata.NumGroups) As Double
        ReDim Me.HNoC(Me.m_epdata.NumGroups)
        ReDim im(Me.m_epdata.NumGroups)
        ReDim Me.Impo(Me.m_epdata.NumGroups)
        ReDim Me.ImpD(Me.m_epdata.NumGroups)
        ReDim Me.LastComp(2 * Me.m_epdata.NumGroups + 1)
        ReDim Me.last(2 * Me.m_epdata.NumLiving)
        ReDim Me.Path(2 * Me.m_epdata.NumGroups + 2)
        ReDim Me.Predat(Me.m_epdata.NumGroups)
        ReDim Me.PredatD(Me.m_epdata.NumGroups)
        ReDim Me.PredatOn(Me.m_epdata.NumGroups)
        ReDim Me.PredNoC(Me.m_epdata.NumGroups)
        ReDim Me.QTL(Me.m_epdata.NumGroups)
        ReDim Me.SumCycDC(Me.m_epdata.NumGroups)
        'ReDim SumDC( m_epdata.NumGroups) As Single
        ReDim Me.TropLvl(Me.m_epdata.NumGroups)
        'ReDim TL(m_epdata.NumGroups)

        'added by eli 052396
        ReDim Me.Ad(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups)
        ReDim Me.Ap(Me.m_epdata.NumGroups + 1, Me.m_epdata.NumGroups)
        ReDim Me.TLsort(Me.m_epdata.NumGroups + 1)
        ReDim Me.BbyTL(Me.m_epdata.NumGroups + 1)
        'ReDim BbyGp(m_epdata.NumGroups + 1)
        ReDim Me.CbyTL(Me.m_epdata.NumGroups + 1)
        'ReDim CbyGp(m_epdata.NumGroups + 1)
        'end of modif. eli 052396
        '*************************************************

        'For i = 1 To m_epdata.NumGroups
        '    BbyGp(i) = m_epdata.B(i)
        'Next

        'For i = 1 To m_epdata.NumGroups
        '    CbyGp(i) = m_epdata.fCatch(i)
        'Next

        For i = 1 To Me.m_epdata.NumGroups
            Me.A1(i) = 0 : Me.A2(i) = 0 : Me.PredatOn(i) = 0
            Me.Predat(i) = 0 : Me.PredatD(i) = 0 : im(i) = 0
            Me.DTA(i) = 0 : Me.TRP(i) = 0 : Me.EXA(i) = 0
            Me.EXAD(i) = 0 : Me.RSP(i) = 0 : Me.RSPD(i) = 0
            Me.TrEm1(i) = 0 : Me.TrEm2(i) = 0
            Me.BbyTL(i) = 0
            Me.CbyTL(i) = 0
            Me.TrpShow(i) = 0
            For j = 1 To Me.m_epdata.NumGroups
                Me.DCNoCyc(i, j) = 0 : Me.G2(i, j) = 0 : Me.G3(i, j) = 0
                Me.Ap(i, j) = 0 : Me.Ad(i, j) = 0 : SumDC(j) = 0
                Me.AM(i, j) = 0 : Me.CycDC(i, j) = 0
            Next j
        Next i

        Me.ImportAmount()
        Me.SumDiet(SumDC)
        Me.ThruputByGroup()

        If Me.FoundCycles = False Then
            Me.FindCycles(Me.Cons)
            Me.FoundCycles = True
        End If

        'CycDC contains the proportion of the diet that is the minimum
        'amount in a cycle and should be removed to break the cycle.
        'This amount is subtracted from all flows in the cycle.

        Me.CalcCycDC(Me.CycDC)
        Me.CalcSumCycDC(Me.CycDC)
        Me.CalcDCNoCyc(Me.CycDC, SumDC)

        'Diet compositions for consumers are made to sum to one.
        'Diet compositions for strict producers are unaffected.
        'DCNoCyc is the feeding coefficients matrix, with cycles excluded.
        'Next calculates PredNoC(i) - with cycles excluded

        For i = 1 To Me.m_epdata.NumGroups
            Me.PredNoC(i) = 0                     'Predation on group i
            For j = 1 To Me.m_epdata.NumLiving
                If DC(j, i) > 0 Then
                    'Cons = B(j) * QB(j) * (SumDC(j) - SumCycDC(j)) / SumDC(j) * DCNoCyc(j, i)
                    Consu = B(j) * QB(j) * (SumDC(j) - Me.SumCycDC(j)) * Me.DCNoCyc(j, i)
                    Me.PredNoC(i) = Me.PredNoC(i) + Consu
                End If
            Next j
            'TrPut is called H() by Ulanowics
            Me.HNoC(i) = Me.PredNoC(i) + Ex(i) + Me.m_epdata.FlowToDet(i) + Resp(i)
            'Next for primary producers add import:
            If Me.m_epdata.PP(i) > 0 And i <= Me.m_epdata.NumLiving Then Me.HNoC(i) = Me.HNoC(i) + im(i) * Me.m_epdata.PP(i)
            'VC Jan 97: Above changed. Before it did not have the condition that it was
            'only for living groups. Therefore import to detritus was added twice.
            'I also think that the m_epdata.pp(i) shold be multiplied so that only the part of the import
            'which relates to TL I is included. Hence, this is assumed to be proportional to m_epdata.pp.
            'For detritus groups add import:
            If i > Me.m_epdata.NumLiving Then
                Me.HNoC(i) = Me.HNoC(i) + im(i)
            End If
        Next i

        For i = 1 To Me.m_epdata.NumGroups
            Me.A1(i) = CSng(1 - CInt(10000 * SumDC(i)) / 10000)
            'If m_epdata.pp(i) = 0 Then A1(i) = 0
            If Me.m_epdata.PP(i) = 1 Then Me.A1(i) = 1
            Me.A1(i) = Me.m_epdata.PP(i)
            Me.Ap(1, i) = Me.A1(i)
        Next i

        For i = 1 To Me.m_epdata.NumGroups
            For j = 1 To Me.m_epdata.NumGroups + 1
                Me.G2(i, j) = Me.DCNoCyc(i, j)
            Next j
        Next i

        Me.NoTL = Me.m_epdata.NumGroups
        For K = 1 To Me.m_epdata.NumGroups             'Max no of trophic levels is m_epdata.NumGroups
            T1 = 0 : T2 = 0
            ErrCode = Me.MatMultS(Me.G2, Me.DCNoCyc, Me.G3)     'g3 = g2 * dcnocyc
            If ErrCode <> 0 Then
                'TODO_jb message box??
                'MsgBox("There is an error in Lindeman module, (Code = " & ErrCode & "),", vbOKOnly & vbCritical, "Module aborted. ")
                Return
                'GoTo EndOfLinde
            End If
            'MsgBox "No of trophic levels: " & k

            For i = 1 To Me.m_epdata.NumLiving
                Me.A2(i) = 0
                For j = 1 To Me.m_epdata.NumLiving
                    Me.A2(i) = Me.A2(i) + Me.A1(j) * Me.G2(i, j)  'Matrix multiplication
                Next j
            Next i

            For i = 1 To Me.m_epdata.NumLiving
                Me.Ap(K + 1, i) = Me.A2(i)                    '... p refers to prim producer
                'Ad(k + 1, i) = G2(i, m_epdata.NumGroups)        '... d refers to detritus
                'VC311099 moved line above to within new fornext below to sum over all detritus
                'If i <= m_epdata.NumGroups Then
                T1 = T1 + Me.A2(i)
                For j = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
                    Me.Ad(K + 1, i) = Me.Ad(K + 1, i) + Me.G2(i, j)
                    T2 = T2 + Me.G3(i, j)    'was G3(i, numgrups)
                Next
                'End If
                For j = 1 To Me.m_epdata.NumGroups
                    If Me.G3(i, j) > 0.000001 Then
                        Me.G2(i, j) = Me.G3(i, j)
                    Else
                        Me.G2(i, j) = 0
                    End If
                Next j
            Next i
            If T1 + T2 < 0.00001 Then
                Me.NoTL = K + 1 : Exit For
            End If

        Next K

        'MsgBox "NOTL=" & NoTL

        For i = 1 To Me.m_epdata.NumGroups
            Me.APj(i) = 0
            Me.ADj(i) = 0
            For j = 1 To Me.m_epdata.NumGroups
                Me.APj(i) = Me.APj(i) + Me.Ap(j, i)
                Me.ADj(i) = Me.ADj(i) + Me.Ad(j, i)
            Next j
        Next i

        For i = 1 To Me.m_epdata.NumLiving                   'This it to make AP and AD sum to 1
            If Me.APj(i) + Me.ADj(i) < 1 And Me.APj(i) + Me.ADj(i) > 0 Then
                For j = 1 To Me.m_epdata.NumLiving
                    Me.Ap(j, i) = Me.Ap(j, i) / (Me.APj(i) + Me.ADj(i))
                    Me.Ad(j, i) = Me.Ad(j, i) / (Me.APj(i) + Me.ADj(i))
                Next j
            End If
        Next i
        'If AP and AD are not made to sum to 1 the sum of the columns in
        'the A-matrix (in Ulanowicz' Lindeman paper) does not add up to 1.

        'VC 28 October 1999: Next doesn't seem to make any sense so remarked out.
        'PriVar = 0
        '  If PriVar = 1 Then
        '  For i = 1 To m_epdata.NumLiving
        '      For j = 1 To m_epdata.NumLiving
        '          num = Ap(i, j)
        '      Next j
        '  Next i
        '  For i = 1 To m_epdata.NumLiving
        '      For j = 1 To m_epdata.NumLiving
        '       num = Ad(i, j)
        '      Next j
        '  Next i
        'End If

        TLChk = 1

        '************************
        'modified as VC's advice eli 052396
        'For i = 1 To NoTL
        'For j = 1 To N
        'AM(i, j) = Ap(i, j) + Ad(i, j)
        'Next j
        'Next i
        'AM(1, m_epdata.NumGroups) = 1                      'All activity of detritus is on
        'trophic level I.
        'Ad =  FlowFromDetritus
        'Ap =  FlowFromPrimProd

        For i = 1 To Me.NoTL
            For j = 1 To Me.m_epdata.NumLiving
                Me.AM(i, j) = Me.Ap(i, j) + Me.Ad(i, j)
            Next j
        Next i
        For j = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
            Me.AM(1, j) = 1                      'All activity of detritus is on
        Next j                                    'trophic level I.
        'end of modif.
        '*****************************************

        For i = 1 To Me.m_epdata.NumGroups
            For j = 1 To Me.NoTL
                If Me.AM(j, i) = 0 Then
                Else
                    If i <= Me.m_epdata.NumLiving Then
                        Me.BbyTL(j) = Me.BbyTL(j) + Me.AM(j, i) * B(i)
                        Me.CbyTL(j) = Me.CbyTL(j) + Me.AM(j, i) * Me.m_epdata.fCatch(i)
                    End If
                End If
            Next j
        Next i
        'Start printing absolute flows

        'Sorts grp's after trophic level and stores ranking in TLsort(m_epdata.NumGroups)
        Me.SortTL()
        ' GoSub SortTL 'EwE5 code      

        TLChk = 2
        For i = 1 To Me.m_epdata.NumGroups
            ii = CInt(Me.TLsort(Me.m_epdata.NumGroups + 1 - i))       'TLsort(i) gives a reference to the
            'groups sorted after trophic level
            For j = 1 To Me.NoTL
                If Me.AM(j, ii) = 0 Then

                Else
                    Me.AM_Abs(j, ii) = CSng(Me.AM(j, ii) * Me.Q(ii))
                    Me.QTL(j) = Me.QTL(j) + Me.AM_Abs(j, ii)
                End If
            Next j
        Next i
        '***

        SumTrp = 0
        For i = 1 To Me.NoTL
            Sum = 0
            For j = 1 To Me.m_epdata.NumLiving
                If Me.APj(j) > 0 Then
                    Me.Predat(i) = Me.Predat(i) + Me.PredNoC(j) * Me.Ap(i, j)
                    Me.TRP(i) = Me.TRP(i) + Me.HNoC(j) * Me.Ap(i, j)
                    If Me.m_GroupsToShow(j) Then Me.TrpShow(i) = Me.TrpShow(i) + Me.HNoC(j) * Me.Ap(i, j)
                    'TrpShow(i) = TrpShow(i) + HNoC(j) * Ap(i, j) 'joeh
                    Me.Impo(i) = Me.Impo(i) + im(j) * Me.Ap(i, j)
                    Me.CA(i) = Me.CA(i) + Me.m_epdata.fCatch(j) * Me.Ap(i, j)
                    Me.EXA(i) = Me.EXA(i) + Ex(j) * Me.Ap(i, j)
                    Me.DTA(i) = Me.DTA(i) + Me.m_epdata.FlowToDet(j) * Me.Ap(i, j)
                    Me.RSP(i) = Me.RSP(i) + Resp(j) * Me.Ap(i, j)
                End If
            Next j
            SumTrp = SumTrp + Me.TRP(i)
        Next i

        'Init
        SumTrpD = 0
        Me.PredatD(1) = 0
        Me.TRPD(1) = 0
        Me.ImpD(1) = 0
        Me.CAD(1) = 0
        Me.EXAD(1) = 0
        'Sum for all detritus groups
        For i = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
            SumTrpD = SumTrpD + Me.HNoC(i)
            Me.PredatD(1) = Me.PredatD(1) + Me.PredNoC(i)
            Me.TRPD(1) = Me.TRPD(1) + Me.HNoC(i)
            Me.ImpD(1) = Me.ImpD(1) + im(i)
            Me.CAD(1) = Me.CAD(1) + Me.m_epdata.fCatch(i)
            Me.EXAD(1) = Me.EXAD(1) + Ex(i)
            Me.RSPD(1) = Me.RSPD(1) + Resp(i)
        Next i
        Me.DTAD(1) = 0                         'Cont to detr from detr is zero
        'RSPD(1) = 0                         'RESP(m_epdata.NumGroups) is zero
        For i = 2 To Me.NoTL
            For j = 1 To Me.m_epdata.NumLiving
                If Me.ADj(j) > 0 Then
                    Me.PredatD(i) += Me.PredNoC(j) * Me.Ad(i, j)
                    Me.TRPD(i) += Me.HNoC(j) * Me.Ad(i, j)

                    If Me.m_GroupsToShow(j) Then Me.TrpShow(i) += Me.HNoC(j) * Me.Ad(i, j)
                    'TrpShow(i) = TrpShow(i) + HNoC(j) * Ad(i, j) 'joeh
                    Me.ImpD(i) += im(j) * Me.Ad(i, j)
                    Me.CAD(i) += Me.m_epdata.fCatch(j) * Me.Ad(i, j)
                    Me.EXAD(i) += Ex(j) * Me.Ad(i, j)
                    Me.DTAD(i) += Me.m_epdata.FlowToDet(j) * Me.Ad(i, j)
                    Me.RSPD(i) += Resp(j) * Me.Ad(i, j)
                End If
            Next j
            SumTrpD = SumTrpD + Me.TRPD(i)
        Next i

        If SumTrp + SumTrpD > 0 Then
            Me.DetIndex = SumTrpD / (SumTrp + SumTrpD)
        End If
        'Proportion of total flow originating from detritus: DetIndex.

        DTDSum = 0 : DDT = 0
        For i = 1 To Me.NoTL + 1
            DTDSum = DTDSum + Me.DTA(i) + Me.DTAD(i)
            DDT = DDT + Me.DTAD(i)
        Next i

        For i = 1 To Me.NoTL + 1
            lines = lines + 1
            If i <= Me.m_epdata.NumLiving Then
                If Me.TRP(i) < 0.0001 And Me.TRP(i + 1) < 0.0001 Then i = Me.NoTL + 1
            Else
                i = Me.NoTL + 1
            End If
        Next i

        For i = 1 To Me.NoTL + 1
            If i <= Me.m_epdata.NumGroups Then
                lines = lines + 1
                If i <= Me.m_epdata.NumLiving Then
                    If Me.TRPD(i) < 0 And Me.TRPD(i + 1) < 0 Then i = Me.NoTL + 1
                End If
            End If
        Next i

        Me.TotalTrp = 0
        Me.SumIm = 0
        For i = 1 To Me.NoTL + 1
            If i <= Me.m_epdata.NumGroups Then
                If i = 1 Then
                    'MsgBox Cstr$(Impo(i) + ImpD(i))
                Else
                    Me.SumIm += (Me.Impo(i) + Me.ImpD(i))
                End If
                Me.TotalTrp = Me.TotalTrp + Me.TRP(i) + Me.TRPD(i)
                If i <= Me.m_epdata.NumLiving Then
                    If (Me.TRP(i) + Me.TRPD(i)) < 0.0001 And (Me.TRP(i + 1) + Me.TRPD(i + 1)) < 0.0001 Then i = Me.NoTL + 1
                Else
                    i = Me.NoTL + 1
                End If
            End If
        Next i

        TrpNoIM = Me.Predat(1) + Me.EXA(1) + Me.DTA(1) + Me.RSP(1)
        TrpNoIM = Me.PredatD(1) + Me.EXAD(1) + Me.DTAD(1) + Me.RSPD(1) + TrpNoIM
        If TrpNoIM > 0 Then
            Dead = 0
            For j = 1 To Me.m_epdata.NumLiving
                If EE(j) < 1 Then Dead = Dead + B(j) * PB(j) * (1 - EE(j)) * Me.AM(1, j)
            Next j
            If TrpNoIM <> 0 Then
                Me.TrEm2(1) = (Dead + Me.EXA(1) + Me.EXAD(1) + Me.Predat(1) + Me.PredatD(1)) / TrpNoIM
                Me.TrEm1(1) = (Me.Predat(1) + Me.PredatD(1)) / TrpNoIM
            End If
        End If

        For i = 2 To Me.NoTL
            If i <= Me.m_epdata.NumGroups Then
                Tr1 = Me.Predat(i) + Me.PredatD(i)
                If Tr1 > 0 Then
                    If Me.TRP(i) + Me.TRPD(i) > 0 Then
                        Me.TrEm1(i) = Tr1 / (Me.TRP(i) + Me.TRPD(i))
                    End If
                End If
                TotTr = Me.Predat(i) + Me.PredatD(i) + Me.CA(i) + Me.CAD(i)
                If Me.TRP(i) + Me.TRPD(i) > 0 Then
                    TotTr = TotTr / (Me.TRP(i) + Me.TRPD(i))
                Else
                    Me.TrEm1(i) = 0
                End If
            End If
        Next i

        For i = 2 To Me.NoTL
            If i <= Me.m_epdata.NumGroups And (Me.TRP(i) + Me.TRPD(i)) > 0 Then
                Dead = 0
                For j = 1 To Me.m_epdata.NumGroups
                    If EE(i) < 1 And i <= Me.m_epdata.NumLiving Then Dead = Dead + B(j) * PB(j) * (1 - EE(j)) * Me.AM(i, j)
                Next j
                TotTr = Me.Predat(i) + Me.PredatD(i) + Me.EXA(i) + Me.EXAD(i) + Dead
                Me.TrEm2(i) = TotTr / (Me.TRP(i) + Me.TRPD(i))
            End If
        Next i

        For i = 1 To Me.NoTL + 1
            If i <= Me.m_epdata.NumLiving Then
                TotalB += Me.BbyTL(i)
                If Me.BbyTL(i) < 0.0001 Then i = Me.NoTL + 1
            End If
        Next i

    End Sub

    Private Sub SortTL()

        Dim K As Integer
        Dim Grp As Integer
        Dim MinTL As Integer

        'This routine sorts group after
        'trophic level
        For K = 1 To Me.m_epdata.NumGroups
            Me.TropLvl(K) = Me.m_epdata.TTLX(K)
        Next K

        For kk As Integer = 1 To Me.m_epdata.NumGroups
            MinTL = 100
            Grp = -2
            For K = 1 To Me.m_epdata.NumGroups
                If Me.TropLvl(K) <= MinTL And Me.TropLvl(K) > 0 Then
                    MinTL = CInt(Me.TropLvl(K))
                    Grp = K
                End If
            Next K
            Me.TLsort(kk) = Grp             'Contains list with sorted grpnumbers
            Me.TropLvl(Grp) = -1
        Next kk

        Return

    End Sub

    Private Sub Ulanow(ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef im() As Single, ByRef Ex() As Single, ByRef Resp() As Single)
        Dim i As Integer, j As Integer, K As Integer
        Dim infoc As Single
        Dim L As Integer
        Dim MM As Integer
        Dim chk As Integer
        Dim Dead As Single, Tmix As Single
        '
        ' Dim bit As Single, ImpEn, ImpEm1, ImpEm2, told
        'Dim X, Yy, E1, C1, ErrCode, ErrC, Ptc, Mark
        Dim bit As Single, ImpEn As Single, ImpEm1 As Single, ImpEm2 As Single, told As Single
        Dim X As Single, Yy As Single, E1 As Single, C1 As Single, ErrCode As Integer, ErrC As Integer, Ptc As Single ', Mark
        '   Dim TLi
        Dim Au As Single = 0.0!
        Dim TotalCatchForThisGear As Single
        Dim Hold() As Single = Nothing
        Dim SumWgt() As Single = Nothing
        Dim CumTrEm1() As Single = Nothing
        Dim CumTrEm2() As Single = Nothing
        Dim TrpTLen() As Single = Nothing
        Dim TrpTLEm1() As Single = Nothing
        Dim TrpTLEm2() As Single = Nothing
        Dim Qold() As Single = Nothing
        Dim Wgt(,) As Single = Nothing
        Dim fi(,) As Single = Nothing
        Dim Fcyc(,) As Single = Nothing
        Dim Acyc(,) As Single = Nothing
        Dim HCyc() As Single = Nothing
        Dim Qcyc() As Single = Nothing
        Dim NoResp() As Single = Nothing
        Dim NoRespC() As Single = Nothing
        Dim Aold(,) As Single = Nothing

        Dim DCnt As Integer

        Static ToldYou As Boolean

        'for debugging exception handling
        'Throw New ApplicationException("Test Exception in Ulanow")

        chk = 0
        For i = 1 To Me.m_epdata.NumGroups
            If Me.TrEm2(i) > 0 Then chk = i
            'Chk becomes highest trophic level with a positive transfer efficiency
        Next i

        If chk > 0 Then
            ReDim Aold(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            'Aold is for pyramid and bank screen
            ReDim CumTrEm1(chk + 1)
            ReDim CumTrEm2(chk + 1)
            ReDim Hold(Me.m_epdata.NumGroups + 3)
            ReDim SumWgt(Me.m_epdata.NumGroups)
            ReDim TrpTLen(chk)
            ReDim TrpTLEm1(chk)
            ReDim TrpTLEm2(chk)
            'ReDim Qem( m_epdata.NumGroups + 3)
            ReDim Qold(Me.m_epdata.NumGroups + 3)
            ReDim Wgt(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
        Else
            Debug.Assert(chk = 0)  'First run of Ulanow
            ReDim Me.AUL(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            ReDim fi(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
            ReDim Fcyc(Me.m_epdata.NumLiving, Me.m_epdata.NumLiving)
            ReDim Acyc(Me.m_epdata.NumGroups + 3, Me.m_epdata.NumGroups + 3)
            ReDim HCyc(Me.m_epdata.NumGroups + 3)
            ReDim Qcyc(Me.m_epdata.NumGroups + 3)
            ReDim NoResp(Me.m_epdata.NumGroups)
            ReDim NoRespC(Me.m_epdata.NumGroups + 1)
            ReDim im(Me.m_epdata.NumGroups + 2)
            ReDim Me.Ac(Me.m_epdata.NumGroups)
            ReDim Me.Ec(Me.m_epdata.NumGroups)
            ReDim Me.CC(Me.m_epdata.NumGroups)
            'ReDim Lap( m_epdata.NumGroups, m_epdata.NumGroups)
        End If

        K = Me.m_epdata.NumGroups + 1 : L = Me.m_epdata.NumGroups + 2 : MM = Me.m_epdata.NumGroups + 3
        bit = 1.442695
        Me.Ao = 0 : Me.Eo = 0 : Me.Co = 0
        Me.Ai = 0 : Me.Ei = 0 : Me.Ci = 0
        Me.Ae = 0 : Me.Eee = 0 : Me.Ce = 0
        Me.Ar = 0 : Me.er = 0 : Me.Cr = 0
        Me.SumEx = 0
        Me.SumResp = 0

        If chk = 0 Then
            For i = 1 To MM
                Me.Q(i) = 0
                Me.TrPut(i) = 0
                Qcyc(i) = 0 : HCyc(i) = 0
                For j = 1 To Me.m_epdata.NumGroups
                    Me.AUL(i, j) = 0
                    Acyc(i, j) = 0
                    If i <= Me.m_epdata.NumGroups Then
                        Me.F(i, j) = 0
                        fi(i, j) = 0
                    End If
                    If i <= Me.m_epdata.NumLiving And j <= Me.m_epdata.NumLiving Then
                        Fcyc(i, j) = 0
                    End If
                Next j
            Next i
            For j = 1 To Me.m_epdata.NumGroups
                NoResp(j) = 0 : im(j) = 0
                Me.Ac(j) = 0 : Me.Ec(j) = 0 : Me.CC(j) = 0
            Next
            For i = 1 To Me.m_epdata.NumGroups
                For j = 1 To Me.m_epdata.NumGroups
                    If j <= Me.m_epdata.NumLiving Then   '041022, accommodating m_epdata.pp with diets
                        '070104VC: the line below had m_epdata.pp(i) instead of m_epdata.pp(j) this caused flow to detritus to be calculated
                        'erroneously. Gave problems with MTI, Absolute flows, ascendency which are fixed now.
                        Me.AUL(i, j) = If(Me.m_epdata.PP(j) < 1, B(j) * QB(j) * DC(j, i), 0)
                    Else
                        Me.AUL(i, j) = CSng(Me.m_epdata.det(i, j))
                    End If
                    If i <= Me.m_epdata.NumLiving And j <= Me.m_epdata.NumLiving Then Acyc(i, j) = Me.AUL(i, j)
                Next j
            Next i                         'Aij is amount eaten of
            'group i by predator j
            Me.CalcImport()

            For i = 1 To MM
                If i <= Me.m_epdata.NumGroups Then
                    Me.AUL(K, i) = im(i)      'Amount imported
                    Me.AUL(i, L) = Ex(i)
                    Me.AUL(i, MM) = Resp(i)
                    Me.SumEx = Me.SumEx + Ex(i)
                    Me.SumResp = Me.SumResp + Resp(i)
                    If i <= Me.m_epdata.NumLiving Then
                        Acyc(K, i) = im(i)
                        Acyc(i, L) = Ex(i)
                        Acyc(i, MM) = Resp(i)
                    End If
                End If
            Next i
        End If                          'Only in first run

        If chk > 0 Then
            '===============                SECOND RUN - EMERGY
            ImpEn = 0
            ImpEm1 = 0
            ImpEm2 = 0
            CumTrEm1(1) = 1
            CumTrEm2(1) = 1
            If Me.TrEm1(1) <> 0 Then CumTrEm1(2) = 1 / Me.TrEm1(1)
            If Me.TrEm2(1) <> 0 Then CumTrEm2(2) = 1 / Me.TrEm2(1)

            For i = 3 To chk
                TrpTLen(i) = 0
                TrpTLEm1(i) = 0
                TrpTLEm2(i) = 0
                If Me.TrEm1(i - 1) > 0 Then CumTrEm1(i) = CumTrEm1(i - 1) / Me.TrEm1(i - 1)
                If Me.TrEm2(i - 1) > 0 Then CumTrEm2(i) = CumTrEm2(i - 1) / Me.TrEm2(i - 1)
                'PRINT TAB(3); USING "##.###  ##########.### "; TrEm1(i); CumTrEm1(i)
            Next i

            ' This transfer efficiency is a cumulative" transfer efficiency
            ' i.e. it is a factor that can be used for raising a flow on a
            ' given trophic level to what it corresponds to of primary prod.

            For i = 1 To Me.m_epdata.NumGroups
                Me.RaiEm1(i) = 0                'For Emergy-1
                Me.RaiEm2(i) = 0                'For Emergy-2
            Next i

            For i = 1 To Me.m_epdata.NumGroups              'i is for groups included in AM
                ImpEn = ImpEn + im(i)
                For j = 1 To chk            'j is for trophic level'
                    Me.RaiEm1(i) = Me.RaiEm1(i) + Me.AM(j, i) * CumTrEm1(j)
                    Me.RaiEm2(i) = Me.RaiEm2(i) + Me.AM(j, i) * CumTrEm2(j)
                Next j
                'PRINT USING "######.## ######.## "; RaiEm1(i); RaiEm2(i)
            Next i


            For i = 1 To MM
                For j = 1 To MM
                    Aold(i, j) = Me.AUL(i, j)
                Next j
            Next i
            For i = 1 To MM
                For j = 1 To MM
                    Qold(i) = Qold(i) + Aold(i, j)
                    Hold(i) = Hold(i) + Aold(j, i)
                Next j                      'Aold is for pyramid (and bank) screen
                told = told + Hold(i)       'Total flow using energy
            Next i




EmergyRun:
            DCnt = DCnt + 1
            For i = 1 To Me.m_epdata.NumGroups
                Me.Ac(i) = 0 : Me.Ec(i) = 0 : Me.CC(i) = 0
                For j = 1 To MM    'm_epdata.NumGroups
                    If DCnt = 1 Then
                        Me.AUL(i, j) = Aold(i, j) * Me.RaiEm1(i)
                    ElseIf DCnt = 2 Then
                        Me.AUL(i, j) = Aold(i, j) * Me.RaiEm2(i)
                    End If
                Next j
                If EE(i) < 1 And i <= Me.m_epdata.NumLiving Then
                    Dead = B(i) * PB(i) * (1 - EE(i))
                Else
                    Dead = 0
                End If
                If DCnt = 1 Then
                    ImpEm1 = ImpEm1 + Me.AUL(K, i)
                Else
                    ImpEm2 = ImpEm2 + Me.AUL(K, i)
                End If
            Next i

        End If                          'Second run - emergy
        '****                           'emergy end

        'PrintAMatr

        Me.TruPut = 0
        Me.Ao = 0 : Me.Eo = 0 : Me.Co = 0
        Me.Ai = 0 : Me.Ei = 0 : Me.Ci = 0
        Me.Ae = 0 : Me.Eee = 0 : Me.Ce = 0
        Me.Ar = 0 : Me.er = 0 : Me.Cr = 0

        Me.TCyc = 0
        For i = 1 To MM
            Me.Q(i) = 0
            Me.TrPut(i) = 0
            For j = 1 To MM
                Me.Q(i) = Me.Q(i) + Me.AUL(i, j)
                'Row sum - out of boxes
                Me.TrPut(i) = Me.TrPut(i) + Me.AUL(j, i)
                'Col sum - into boxes
                'Notice that Q(i) will be larger
                'than TrPut(i) for a producer with QB=0
                Qcyc(i) = Qcyc(i) + Acyc(i, j)
                HCyc(i) = HCyc(i) + Acyc(j, i)
            Next j
            Me.TruPut = CSng(Me.TruPut + Me.Q(i))            'Total flow=throughput
            Me.TCyc = Me.TCyc + Qcyc(i)
        Next i
        If DCnt = 1 Then Tmix = Me.TruPut

        For i = 1 To Me.m_epdata.NumGroups               'Imports
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If Me.Q(K) > 0 And Me.Q(i) > 0 Then
                X = CSng(Me.AUL(K, i) * Me.TruPut / Me.Q(K) / Me.Q(i))
                Yy = CSng(Me.AUL(K, i) ^ 2 / Me.Q(K) / Me.Q(i))
            End If
            If X > 0 Then Au = CSng(Me.AUL(K, i) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-Me.AUL(K, i) * Math.Log(Yy))
            If Me.TruPut > 0 Then X = (Me.AUL(K, i) / Me.TruPut)

            If X > 0 Then C1 = CSng(-Me.AUL(K, i) * Math.Log(X))
            Me.Ao = Me.Ao + Au
            Me.Eo = Me.Eo + E1
            Me.Co = Me.Co + C1
            Me.AddUp_i_j(i, Au, E1, C1)  'Sum the group specific ascendency etc
        Next i

        For i = 1 To Me.m_epdata.NumGroups               'Trophic interactions
            For j = 1 To Me.m_epdata.NumGroups
                X = 0 : Yy = 0
                Au = 0 : E1 = 0 : C1 = 0
                If Me.AUL(i, j) > 0 Then
                    If chk = 0 Then
                        If Me.Q(i) > 0 And Me.TrPut(j) > 0 Then
                            X = CSng(Me.AUL(i, j) * Me.TruPut / Me.Q(i) / Me.TrPut(j))
                            Yy = CSng(Me.AUL(i, j) ^ 2 / Me.Q(i) / Me.TrPut(j))
                        End If
                    Else
                        If Me.Q(i) > 0 And Me.Q(j) > 0 And Me.TrPut(j) <> 0 Then
                            X = CSng(Me.AUL(i, j) * Me.TruPut / Me.Q(i) / Me.Q(j))
                            Yy = CSng(Me.AUL(i, j) ^ 2 / Me.Q(i) / Me.Q(j))
                        End If
                    End If
                End If
                If X > 0 Then Au = CSng(Me.AUL(i, j) * Math.Log(X))
                If Yy > 0 Then E1 = CSng(-Me.AUL(i, j) * Math.Log(Yy))
                X = (Me.AUL(i, j) / Me.TruPut)
                If X > 0 Then C1 = CSng(-Me.AUL(i, j) * Math.Log(X))
                Me.Ai = Me.Ai + Au
                Me.Ei = Me.Ei + E1
                Me.Ci = Me.Ci + C1
                Me.AddUp_i_j(i, Au, E1, C1)
            Next j
        Next i

        For j = 1 To Me.m_epdata.NumGroups               'Exports
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If chk = 0 Then            'Energy run
                If Me.TrPut(j) > 0 And Me.TrPut(L) > 0 Then
                    X = CSng(Me.AUL(j, L) * Me.TruPut / Me.TrPut(L) / Me.TrPut(j))
                    Yy = CSng(Me.AUL(j, L) ^ 2 / Me.TrPut(L) / Me.TrPut(j))
                End If
            Else                        'Emergy run 1 or 2
                If Me.Q(j) > 0 And Me.TrPut(L) > 0 Then
                    X = CSng(Me.AUL(j, L) * Me.TruPut / Me.TrPut(L) / Me.Q(j))
                    Yy = CSng(Me.AUL(j, L) ^ 2 / Me.TrPut(L) / Me.Q(j))
                End If
            End If
            If X > 0 Then Au = CSng(Me.AUL(j, L) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-Me.AUL(j, L) * Math.Log(Yy))
            X = (Me.AUL(j, L) / Me.TruPut)
            If X > 0 Then C1 = CSng(-Me.AUL(j, L) * Math.Log(X))
            Me.Ae = Me.Ae + Au
            Me.Eee = Me.Eee + E1
            Me.Ce = Me.Ce + C1
            Me.AddUp_i_j(j, Au, E1, C1)
        Next j

        For j = 1 To Me.m_epdata.NumGroups               'Respiration
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If chk = 0 Then
                If Me.TrPut(MM) > 0 And Me.TrPut(j) > 0 Then
                    X = CSng(Me.AUL(j, MM) * Me.TruPut / Me.TrPut(MM) / Me.TrPut(j))
                    Yy = CSng(Me.AUL(j, MM) ^ 2 / Me.TrPut(MM) / Me.TrPut(j))
                End If
            Else
                If Me.TrPut(MM) > 0 And Me.Q(j) > 0 Then
                    X = CSng(Me.AUL(j, MM) * Me.TruPut / Me.TrPut(MM) / Me.Q(j))
                    Yy = CSng(Me.AUL(j, MM) ^ 2 / Me.TrPut(MM) / Me.Q(j))
                End If
            End If
            If X > 0 Then Au = CSng(Me.AUL(j, MM) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-Me.AUL(j, MM) * Math.Log(Yy))
            X = (Me.AUL(j, MM) / Me.TruPut)
            If X > 0 Then
                C1 = CSng(-Me.AUL(j, MM) * Math.Log(X))
            End If
            Me.Ar = Me.Ar + Au
            Me.er = Me.er + E1
            Me.Cr = Me.Cr + C1
            Me.AddUp_i_j(j, Au, E1, C1)
        Next j
        '     If Chk = 0 Then                'First run
        '      'Print Tab(30); "Network flow indices by group"
        '      'MsgBox "Network flow indices by group"
        '     End If
        '     If Chk > 0 Then                'Second run = emergy calculation
        '      If DCnt = 1 Then
        '       MsgBox "Emergy1 based ascendency by group"
        '      ElseIf DCnt = 2 Then
        '       MsgBox "Emergy2 based ascendency by group"
        '      End If
        '     End If
        Me.SumAc = 0
        Me.SumEc = 0
        Me.SumCc = 0
        For i = 1 To Me.m_epdata.NumGroups
            Me.Ac(i) = Me.Ac(i) * bit
            Me.Ec(i) = Me.Ec(i) * bit
            Me.CC(i) = Me.CC(i) * bit
            Me.SumAc = Me.SumAc + Me.Ac(i)
            Me.SumEc = Me.SumEc + Me.Ec(i)
            Me.SumCc = Me.SumCc + Me.CC(i)
        Next i

        If chk > 0 Then                'pyramid and bank
            For j = 1 To Me.m_epdata.NumGroups
                SumWgt(j) = 0
            Next j
            For i = 1 To chk
                For j = 1 To Me.m_epdata.NumGroups
                    If DCnt = 1 Then
                        If i > 1 Then
                            Wgt(i, j) = CumTrEm1(i) * Me.AM(i, j)
                        Else
                            Wgt(i, j) = Me.AM(i, j)
                        End If
                    ElseIf DCnt = 2 Then
                        If i > 1 Then
                            Wgt(i, j) = CumTrEm2(i) * Me.AM(i, j)
                        Else
                            Wgt(i, j) = Me.AM(i, j)
                        End If
                    End If
                    SumWgt(j) = SumWgt(j) + Wgt(i, j)
                Next j
            Next i

            For i = 1 To chk
                For j = 1 To Me.m_epdata.NumGroups
                    If SumWgt(j) = 0 Then SumWgt(j) = 1
                    If DCnt = 1 Then
                        If SumWgt(j) > 0 Then TrpTLEm1(i) = CSng(TrpTLEm1(i) + Me.Q(j) * Wgt(i, j) / SumWgt(j))
                        TrpTLen(i) = TrpTLen(i) + Qold(j) * Me.AM(i, j)
                    ElseIf DCnt = 2 Then
                        If SumWgt(j) > 0 Then TrpTLEm2(i) = CSng(TrpTLEm2(i) + Me.Q(j) * Wgt(i, j) / SumWgt(j))
                    End If
                Next j
            Next i
        End If

        Me.Ao = Me.Ao * bit : Me.Eo = Me.Eo * bit : Me.Co = Me.Co * bit
        Me.Ai = Me.Ai * bit : Me.Ei = Me.Ei * bit : Me.Ci = Me.Ci * bit
        Me.Ae = Me.Ae * bit : Me.Eee = Me.Eee * bit : Me.Ce = Me.Ce * bit
        Me.Ar = Me.Ar * bit : Me.er = Me.er * bit : Me.Cr = Me.Cr * bit

        For i = 1 To Me.m_epdata.NumGroups                 'FOR HOST-MATRIX FOR IMPACT
            'For j = 1 To m_epdata.NumGroups             'EXCLUD. RESP
            For j = 1 To Me.m_epdata.NumLiving             'EXCLUD. RESP and flow to detritus
                NoResp(i) = NoResp(i) + Me.AUL(i, j)
            Next j
            NoRespC(i) = Me.m_epdata.fCatch(i) + NoResp(i)  'Including catches
        Next i

        'NO FISHERY
        For i = 1 To Me.m_epdata.NumGroups               'FOR HOST-MATRIX FOR IMPACT
            If NoResp(i) > 0 Then  'Host: a group i , recipient: living j
                For j = 1 To Me.m_epdata.NumLiving    'Not detritus as recipient  'EXCL RESP
                    If j <= Me.m_epdata.NumLiving Then Me.F(i, j) = Me.AUL(i, j) / NoResp(i)
                Next j
            End If
        Next i
        'WITH FISHERY


        For i = 1 To Me.m_epdata.NumGroups               'FOR HOST-MATRIX FOR IMPACT
            If NoRespC(i) > 0 Then
                For j = 1 To Me.m_epdata.NumLiving  'Groups + m_epdata.NumFleet         'EXCL RESP
                    Me.FC(i, j) = Me.AUL(i, j) / NoRespC(i) 'Includes fishery
                Next
                For j = 1 To Me.m_epdata.NumFleet 'Host: a group i , recipient: fishery j
                    Me.FC(i, j + Me.m_epdata.NumGroups) = (Me.m_epdata.Landing(j, i) + Me.m_epdata.Discard(j, i)) / NoRespC(i)
                Next
            End If
        Next i

        'Host matrix for m_epdata.Discards going to detritus  (from fleets)
        If Me.m_epdata.NumFleet > 0 Then
            For i = 1 To Me.m_epdata.NumFleet
                For j = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups     'Host: fleet i, recipient: Detritus j
                    TotalCatchForThisGear = Me.m_epdata.Landing(i, 0) + Me.m_epdata.Discard(i, 0)
                    If TotalCatchForThisGear > 0 Then Me.FC(Me.m_epdata.NumGroups + i, j) = CSng(Me.m_epdata.det(Me.m_epdata.NumGroups + i, Me.m_epdata.NumLiving) / TotalCatchForThisGear)
                Next
            Next
        End If

        'A with detritus"
        For i = 1 To Me.m_epdata.NumGroups               'FOR CYCLING INDEX
            If Me.Q(i) > 0 Then
                For j = 1 To Me.m_epdata.NumGroups
                    fi(i, j) = CSng(-Me.AUL(i, j) / Me.Q(i))
                    If i = j Then fi(i, j) = 1 + fi(i, j)
                Next j
            End If
        Next i

        'A without detritus"
        For i = 1 To Me.m_epdata.NumLiving                'FOR CYCLING INDEX
            If Qcyc(i) > 0 Then
                For j = 1 To Me.m_epdata.NumLiving
                    Fcyc(i, j) = -Acyc(i, j) / Qcyc(i)
                    If i = j Then Fcyc(i, j) = 1 + Fcyc(i, j)
                Next j
            End If
        Next i

        If chk = 0 Then                  'Only find cycling index in first run
            Me.TcD = 0
            Me.Tc = 0
            'MsgBox "Making Matrix Inversion #1"
            ErrCode = Me.MatInvS(fi)        'FOR CYCLING INDEX
            If ErrCode <> 0 Then
                'Mess$ = "         There is an error, ( Code = " & ERRCode & " ) "
                'Mess$ = Mess&  "    Finn's cycling index cannot be calculated "
                'MsgBox Mess$
            Else
                For i = 1 To Me.m_epdata.NumGroups
                    If fi(i, i) > 0 Then
                        Me.AmCyc = CSng(Me.Q(i) * (fi(i, i) - 1) / fi(i, i))   'Amount cycled
                        Me.TcD = Me.TcD + Me.AmCyc      'For cycling index including detritus
                    End If
                Next i
            End If

            'MsgBox "Making Matrix Inversion #2"
            ErrC = Me.MatInvS(Fcyc)         'FOR CYCLING INDEX excl detr
            If ErrC <> 0 Then
                'mess$ = "         There is an error, ( Code = " & ERRC & " ) "
                'mess$ = mess$ & "    Predatory cycling index cannot be calculated "
                'MsgBox mess$
            Else
                For i = 1 To Me.m_epdata.NumLiving
                    If Fcyc(i, i) > 0 Then
                        Me.AmCyc = Qcyc(i) * (Fcyc(i, i) - 1) / Fcyc(i, i)
                        'Amount cycled
                        Me.Tc = Me.Tc + Me.AmCyc
                        'For cycling index excluding detritus
                    End If
                Next i
            End If
        End If
        If ErrCode <> 0 Then ' Or ERRC <> 0 Then
        End If
        Me.Ascen = Me.Ao + Me.Ai + Me.Ae + Me.Ar
        If chk = 0 Then Me.Ascend = Me.Ascen 'It is Ascend that will be saved
        Me.Overhead = Me.Eo + Me.Ei + Me.Eee + Me.er
        Me.Capacity = Me.Co + Me.Ci + Me.Ce + Me.Cr
        If Me.TruPut > 0 Then
            infoc = Me.Ascen / Me.TruPut
            Ptc = 100 / Me.Capacity
            Me.Aop = Me.Ao * Ptc : Me.Eop = Me.Eo * Ptc : Me.Cop = Me.Co * Ptc
            Me.Aip = Me.Ai * Ptc : Me.Eip = Me.Ei * Ptc : Me.Cip = Me.Ci * Ptc
            Me.Aep = Me.Ae * Ptc : Me.Eep = Me.Eee * Ptc : Me.Cep = Me.Ce * Ptc
            Me.Arp = Me.Ar * Ptc : Me.Erp = Me.er * Ptc : Me.Crp = Me.Cr * Ptc
            Me.Ascp = Me.Ascen * Ptc
            Me.Overp = Me.Overhead * Ptc
            Me.Capp = Me.Capacity * Ptc
        End If
        'Save to Database
        'Dim SQL As String
        'On Local Error Resume Next
        'SQL = "SELECT * from [Summary Statistics] where modelName='" & lastModel & "'"
        'Set g_Recordset = CCG.UpdatableRecords(SQL)  '  g_databas.OpenRecordset(SQL)
        'If g_Recordset.RecordCount = 0 Then g_Recordset.AddNew
        'g_Recordset.Fields("modelName").value = lastModel
        'g_Recordset.Fields("Ascendency").value = Ascen
        'g_Recordset.Fields("Capacity").value = Capacity
        'g_Recordset.Fields("RelAscendency").value = Ascen / Capacity
        'g_Recordset.Fields("InternalAscendency").value = Aip
        'g_Recordset.Fields("InternalOverhead").value = Eip
        'g_Recordset.Fields("ExportOverhead").value = Eep
        'g_Recordset.Fields("Information").value = infoc
        'g_Recordset.Update

        'If chk > 0 Then
        'MsgBox "CALCULATION OF NETWORK FLOW INDICES BASED ON EMERGY"
        'MsgBox " (Emergy) "
        'End If
        If chk = 0 Then                  'Only print cycling in first run
            If ErrC = 0 Then
            Else
                If ToldYou = False Then Me.SendMessage(New cMessage(My.Resources.PROMPT_ERROR_CYCLING, eMessageType.Any, EwEUtils.Core.eCoreComponentType.External, eMessageImportance.Warning))
                ToldYou = True
            End If
        Else                              'if Chk > 0
            '     MsgBox "Raising factors used for each group to raise flows to emergy units:"
            '        For i = 1 To m_epdata.NumGroups
            '        If DCnt = 1 Then
            '          If RaiEm1(i) > 99999 Then
            '            MsgBox CStr(RaiEm1(i))
            '          Else
            '            MsgBox CStr(RaiEm1(i))
            '          End If
            '        Else
            '          If RaiEm2(i) > 99999 Then
            '            MsgBox CStr(RaiEm2(i))
            '          Else
            '           MsgBox CStr(RaiEm2(i))
            '          End If
            '        End If
            '       Next i
        End If                            'End of print cycling
        ' Mark = 0


nextroute:
        If chk > 0 And DCnt = 1 Then GoTo EmergyRun

        '     If chk > 0 Then                'Print pyramid and bank here
        '      MsgBox "             Throughput by trophic level"
        '      MsgBox "Trophic"
        '      For TLi = chk To 1 Step -1
        '       MsgBox trophic(TLi)
        '       MsgBox TrpTLEn(TLi)
        '       MsgBox TrpTLEm1(TLi)
        '       MsgBox TrpTLEm2(TLi)
        '      Next TLi
        '      MsgBox "   Import: "
        '      MsgBox ImpEn
        '      MsgBox ImpEm1
        '      MsgBox ImpEm2
        '      MsgBox "Total: "
        '      MsgBox told
        '      MsgBox Tmix
        '      MsgBox TruPut
        '     End If                          'End of pyramid and bank screen




    End Sub


    '=======================MatMultS%===================================
    'MatMultS% multiplies two single precision matrices and places the
    'product in a result matrix
    '
    'Parameters: matrices Alpha,Beta,Gamma
    '
    'Returns: Gamma() = Alpha() * Beta()
    '===================================================================
    Private Function MatMultS(Alpha(,) As Single, Beta(,) As Single, Gamma(,) As Single) As Integer
        Dim col As Integer, row As Integer, inside As Integer
        On Error GoTo smulterr : MatMultS = 0
        If (LBound(Alpha, 2) <> LBound(Beta, 1)) Or (UBound(Alpha, 2) <> UBound(Beta, 1)) Then
            Error 197                   'check inside dimensions
        ElseIf (LBound(Alpha, 1) <> LBound(Gamma, 1)) Or (UBound(Alpha, 1) <> UBound(Gamma, 1)) Or (LBound(Beta, 2) <> LBound(Gamma, 2)) Or (UBound(Beta, 2) <> UBound(Gamma, 2)) Then
            Error 195                   'check dimensions of result matrix
        End If
        'loop through, Gamma(row,col)=inner product of Alpha(row,*) and Beta(*,col)
        For row = LBound(Gamma, 1) To UBound(Gamma, 1)
            For col = LBound(Gamma, 2) To UBound(Gamma, 2)
                Gamma(row, col) = 0.0!
                For inside = LBound(Alpha, 2) To UBound(Alpha, 2)
                    Gamma(row, col) += Alpha(row, inside) * Beta(inside, col)
                Next inside
            Next col
        Next row
smultexit:
        Exit Function
smulterr:
        MatMultS = (Err.Number + 5) Mod 200 - 5
        Resume smultexit
    End Function


    Private Function MatInvS(A(,) As Single) As Integer
        Dim bserrcode As Integer, col As Integer, ErrCode As Integer, row As Integer
        Dim Ain(,) As Single, Ein() As Single, X() As Single
        On Error GoTo sinverr
        ErrCode = 0

        'MatrixCalc.matluS uses up and lo 
        Me.m_core.EcoFunction.MatrixCalc.Lo = 1 'LBound(A, 1)
        Me.m_core.EcoFunction.MatrixCalc.Up = UBound(A, 1)
        Dim Lo As Integer = Me.m_core.EcoFunction.MatrixCalc.Lo
        Dim Up As Integer = Me.m_core.EcoFunction.MatrixCalc.Up
        ReDim Me.m_core.EcoFunction.MatrixCalc.rpvt(Up)
        ReDim Me.m_core.EcoFunction.MatrixCalc.cpvt(Up)


        ReDim Ain(Up, Up)
        ReDim Ein(Up)
        ReDim X(Up)

        ErrCode = Me.m_core.EcoFunction.MatrixCalc.matluS(A, True)                     'Get LU matrix
        'nong 'stop If Not continue Then Error ErrCode
        For col = Lo To Up                         'find A^-1 one column at a time
            Ein(col) = 1.0!
            bserrcode = Me.m_core.EcoFunction.MatrixCalc.matbsS(A, Ein, X)
            If bserrcode <> 0 Then Error bserrcode
            For row = Lo To Up
                Ain(row, col) = X(row)
                Ein(row) = 0.0!
            Next row
        Next col
        For col = Lo To Up                         'put A^-1 in A
            For row = Lo To Up
                A(row, col) = Ain(row, col)
            Next row
        Next col
        If ErrCode <> 0 Then Error ErrCode
sinvexit:
        Erase Me.E, X, Ain, Me.m_core.EcoFunction.MatrixCalc.rpvt, Me.m_core.EcoFunction.MatrixCalc.cpvt
        MatInvS = ErrCode
        Exit Function
sinverr:
        ErrCode = (Err.Number + 5) Mod 200 - 5
        Resume sinvexit
    End Function


    Private Sub CalcImport()
        Dim i As Integer
        For i = 1 To Me.m_epdata.NumGroups
            If i > Me.m_epdata.NumLiving Then
                Me.ImpDet(i) = Me.m_epdata.DtImp(i)
            Else
                Me.ImpDet(i) = Me.m_epdata.DC(i, 0) * Me.m_epdata.QB(i) * Me.m_epdata.B(i)
            End If
        Next i
    End Sub


    Private Sub AddUp_i_j(i As Integer, Au As Single, E1 As Single, C1 As Single)

        Me.Ac(i) = Me.Ac(i) + Au
        Me.Ec(i) = Me.Ec(i) + E1
        Me.CC(i) = Me.CC(i) + C1
    End Sub



    Private Sub Impacts()

        Try

            'Mantis issue 551
            'From ecopath.org user:Alberto Barausse
            'This is not precisely a "bug" according to the standard meaning, but in the MTI analysis discarded groups 
            'are positively impacting the fleet that discards them, i.e. I believe they are treated as landings making '
            'up the fleet diet matrix. If I am right (this is what clearly appears from my model) '
            'I think the routine should be corrected.

            'VC Oct 2008, fixed the above by excluding discards from DCCnoDiscard while it is still included in the FC
            'DCC is now used for calculation of beneficial predation only
            ReDim Me.DCC(Me.m_epdata.NumFleet, Me.m_epdata.NumGroups)
            Dim i As Integer, j As Integer, ErrCode As Integer
            Dim CatchByGear As Single
            Dim DCCnoDiscard(Me.m_epdata.NumFleet, Me.m_epdata.NumGroups) As Single

            For i = 1 To Me.m_epdata.NumFleet
                CatchByGear = 0       'catch by gear
                For j = 1 To Me.m_epdata.NumGroups
                    CatchByGear = CatchByGear + Me.m_epdata.Landing(i, j) '+ m_epdata.Discard(i, j)
                Next j
                For j = 1 To Me.m_epdata.NumGroups
                    If CatchByGear > 0 Then   'DCC is "Diet Composition of Catch" by gear
                        Me.DCC(i, j) = (Me.m_epdata.Landing(i, j) + Me.m_epdata.Discard(i, j)) / CatchByGear
                        DCCnoDiscard(i, j) = Me.m_epdata.Landing(i, j) / CatchByGear
                    End If
                Next j
            Next

            For i = 1 To Me.m_epdata.NumGroups            'Detritus is not considered a predator
                For j = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
                    Me.F(i, j) = 0
                    Me.FC(i, j) = 0
                Next
            Next i


            For i = 1 To Me.m_epdata.NumGroups      'Impacting
                For j = 1 To Me.m_epdata.NumGroups  'Impacted=host
                    If Me.m_epdata.NumFleet >= 1 Then   'Then fishery is included as group(s)
                        If j > Me.m_epdata.NumLiving Then  'Detritus
                            Me.MTI(i, j) = -(0 - Me.FC(j, i))
                        Else
                            Me.MTI(i, j) = -(Me.m_epdata.DC(j, i) - Me.FC(j, i))       'Host matrix incl C
                        End If
                    Else                'no fishery
                        If j > Me.m_epdata.NumLiving Then  'Detritus
                            Me.MTI(i, j) = -(0 - Me.F(j, i)) 'Detritus is not considered a predator
                        Else
                            Me.MTI(i, j) = -(Me.m_epdata.DC(j, i) - Me.F(j, i))
                            '= proportion i contributes to the diet of j less the proportion i takes from j
                        End If
                    End If              'FEEDING MATRIX - HOST MATRIX
                Next j
                Me.MTI(i, i) = 1 + Me.MTI(i, i)   '[Ulanowicz' M -- here called MTI, is the [I]-[Q] MATRIX
            Next i
            If Me.m_epdata.NumFleet > 0 Then  'Then fishery is included as group(s)
                'First do impact of groups on fishery
                For i = 1 To Me.m_epdata.NumGroups ' + m_epdata.NumFleet
                    For j = 1 To Me.m_epdata.NumFleet
                        Me.MTI(i, Me.m_epdata.NumGroups + j) = -(DCCnoDiscard(j, i) - 0)
                        '-(m_epdata.dcC(j, i) - FC(m_epdata.NumGroups + j, i))
                        '= How much fishery takes of group i - how much the groups takes from the fishery (0)
                    Next
                Next i
                'Next impact of fishery on groups       'FC(host, predator)
                For i = 1 To Me.m_epdata.NumFleet ' + m_epdata.NumFleet
                    For j = 1 To Me.m_epdata.NumGroups
                        Me.MTI(Me.m_epdata.NumGroups + i, j) = -(0 - Me.FC(j, Me.m_epdata.NumGroups + i))
                        'if part of the prey is dying due to M0 then FC for fishery will be lower
                        'and the impact of the fishery will be lower
                    Next
                Next i

                'Next to interaction between fleets
                'For i = 1 To m_epdata.NumGroups
                '    For j = 1 To m_epdata.NumFleet
                'There is no direct interaction (unless one fisher steals from another :-)
                '  MTI(m_epdata.NumGroups+i, m_epdata.NumGroups+j) = -(m_epdata.dcC(j, i) - FC(m_epdata.NumGroups + j, i))
                '  MTI(m_epdata.NumGroups + j, m_epdata.NumGroups+i) = -(0 - FC(m_epdata.NumGroups+i, m_epdata.NumGroups + j))
                '    Next
                'Next

                'Add 1 to diagonal for fleets -- just like for living groups above
                For j = 1 To Me.m_epdata.NumFleet
                    Me.MTI(Me.m_epdata.NumGroups + j, Me.m_epdata.NumGroups + j) = 1 + Me.MTI(Me.m_epdata.NumGroups + j, Me.m_epdata.NumGroups + j)
                Next
            End If

            'Set impact of detritus on detritus to zero
            'For i = N + 1 To m_epdata.NumGroups
            '    For j = N + 1 To m_epdata.NumGroups
            '        MTI(i, j) = 0
            '    Next
            'Next
            'With above activated matrix cannot be inverted/VC Jan 97


            'For i = 1 To m_epdata.NumGroups + m_epdata.NumFleet
            '    For j = 1 To m_epdata.NumGroups + m_epdata.NumFleet
            '        If MTI(i, j) > 0 Then MsgBox Cstr$(i) + "  " + Cstr$(j) + "  " + Cstr$(MTI(i, j))
            '    Next
            'Next

            ErrCode = Me.MatInvS(Me.MTI)
            If ErrCode% <> 0 Then
                MsgBox("There is an error, (Code = " & ErrCode% & ")")
                If ErrCode% = -1 Then
                    MsgBox("MTI Matrix not invertible")
                End If
                For i = 1 To Me.m_epdata.NumGroups + Me.m_epdata.NumFleet
                    For j = 1 To Me.m_epdata.NumGroups + Me.m_epdata.NumFleet
                        Me.MTI(i, j) = 0
                    Next
                Next
                GoTo EndOfImp
            End If

RETURNED:

            For i = 1 To Me.m_epdata.NumGroups + Me.m_epdata.NumFleet
                If i > Me.m_epdata.NumLiving And i <= Me.m_epdata.NumGroups Then  'for detritus
                    Me.MTI(i, i) = 0
                Else
                    Me.MTI(i, i) = Me.MTI(i, i) - 1   'for living and fishery
                End If
            Next i

            'The Mixed Tropic Impact Matrix has now been calculated"
            Me.CheckForBenificialPredation()

EndOfImp:

        Catch ex As Exception
            Debug.Assert(False)
            cLog.Write(ex)
            Throw New ApplicationException("Impacts() ", ex)
        End Try


    End Sub


    Private Sub CheckForBenificialPredation()
        Dim i As Integer
        Dim j As Integer
        Try

            For i = 1 To Me.m_epdata.NumGroups + Me.m_epdata.NumFleet
                For j = 1 To Me.m_epdata.NumGroups + Me.m_epdata.NumFleet
                    Me.BenificialPredation(i, j) = False
                Next
            Next
            'First check for groups
            For i = 1 To Me.m_epdata.NumLiving
                For j = 1 To Me.m_epdata.NumGroups
                    If Me.m_epdata.DC(i, j) > 0 And Me.MTI(i, j) > 0 Then
                        Me.BenificialPredation(i, j) = True
                    End If
                Next
            Next
            'Then check for impact of fishery
            For i = 1 To Me.m_epdata.NumFleet
                For j = 1 To Me.m_epdata.NumGroups
                    If Me.DCC(i, j) > 0 And Me.MTI(i + Me.m_epdata.NumGroups, j) > 0 Then
                        Me.BenificialPredation(i + Me.m_epdata.NumGroups, j) = True
                    End If
                Next
            Next
        Catch ex As Exception
            Debug.Assert(False)
            Throw New ApplicationException("CheckForBenificialPredation()", ex)
        End Try

    End Sub


    Private Sub Assign1DimInteg(Start As Integer, slut As Integer, value As Integer, vectorGetValue() As Integer)
        For i As Integer = Start To slut
            vectorGetValue(i) = value
        Next
    End Sub


    Private Sub CyclePrint(CycDC(,) As Single, Cons() As Single) 'Have identified a cycle
        Dim arrow As Integer, bib As Integer
        Dim K As Integer
        arrow = 1
        Me.aa = 0
        For K = Me.pivot - 1 To Me.Level
            If Me.Path(K) > 0 Then
                bib = Me.aa
                Me.aa = Me.Path(K)
                If arrow = 0 Then
                    'mess$ = mess$ & "<---"
                    If CycDC(bib, Me.aa) < 0 Then
                        Cons(bib) = 0
                    Else
                        Cons(bib) = -Me.m_epdata.B(bib) * Me.m_epdata.QB(bib) * Me.m_epdata.DC(bib, Me.aa)
                    End If
                End If
                'mess$ = mess$ & Cstr$(Path(k))
                arrow = 0
            End If
        Next K
        'MsgBox mess$
        'mess$ = ""
    End Sub

    Private Sub FindMinConsump(ByRef Cons() As Single, ByRef MinCons As Single)
        'Find min consumption (i.e. least negative Cons)
        Try
            Dim Mini As Integer
            MinCons = 1
            For Mini = Me.pivot - 1 To Me.m_epdata.NumLiving
                If Cons(Me.Path(Mini)) <= 0 And Me.m_epdata.QB(Me.Path(Mini)) > 0 Then
                    If MinCons = 1 Then MinCons = Cons(Me.Path(Mini))
                    If Cons(Me.Path(Mini)) > MinCons Then MinCons = Cons(Me.Path(Mini))
                End If
            Next Mini
            If MinCons = 1 Then MinCons = 0
            For Mini = 1 To Me.m_epdata.NumLiving : Cons(Mini) = 0 : Next Mini
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("FindComsump() " & ex.Message, ex)
        End Try

    End Sub


    Private Sub ImportAmount()
        'This subroutine gives the total import (as an amount) to each group
        Try
            Dim i As Integer
            For i = 1 To Me.m_epdata.NumLiving
                Me.ImpDet(i) = Me.m_epdata.DC(i, 0) * Me.m_epdata.QB(i) * Me.m_epdata.B(i)
            Next i
            For i = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
                Me.ImpDet(i) = Me.m_epdata.DtImp(i)
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("ImportAmount() " & ex.Message, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Scary stuff to pass large model deadlocks
    ''' </summary>
    Public Property SkipCyclesPathways As Boolean = False

    Private Sub FindCycles(ByRef Cons() As Single)

        ' Wow!
        If Me.SkipCyclesPathways Then Return

        'CycDC [previously called CD] contains the proportion
        'of the diet that is the minimum
        'amount in a cycle and should be removed to break the cycle.
        'This amount is subtracted from all flows in the cycle.
        Dim APred As Integer, Comp As Integer, pred As Integer, prey As Integer
        Dim Answer As Object = Nothing
        Dim Cnt As Integer


        'Dim iProg As Integer

        Me.bStopNetworkAnnalysis = False

        'DoWhat = "PPR"
        'frmWait.Caption = "Identification of pathways in progress, (cancel if pathways not needed)"
        'frmWait.Frame1.Visible = False
        'frmWait.PBar.max = 1000 'm_epdata.NumLiving
        'frmWait.ZOrder()
        'frmWait.Show() '0
        'frmWait.Refresh()
        Cnt = 0
        ' Init1Dim(1, m_epdata.NumLiving, Cons())
        Array.Clear(Cons, 0, Me.m_epdata.NumLiving)
        'Initialize
        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ''Timing code
        'Dim stpwRunTime As Stopwatch
        'Dim stpwCompTime As Stopwatch
        'stpwCompTime = New Stopwatch
        'stpwRunTime = Stopwatch.StartNew
        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        'If m_epdata.NumLiving > 20 And DoneAlready = False Then
        ' If DoneAlready = False Then DoneAlready = True
        '    Answer = MsgBox("Your model has many groups, this procedure may take a long time to complete. Do you want to continue?", vbInformation + vbYesNo, "Find pathways: tedious for large models")
        '    If Answer = vbNo Then AbortRun = True: Exit Sub
        'End If

        For Me.pivot = 1 To Me.m_epdata.NumLiving
            If Me.m_epdata.QB(Me.pivot) <= 0 Then
                GoTo NextPivot
            End If

            Array.Clear(Me.Path, 0, 2 * Me.m_epdata.NumGroups + 2)
            'Init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path)
            Me.Assign1DimInteg(1, 2 * Me.m_epdata.NumGroups + 1, Me.pivot, Me.LastComp)

            Me.Path(Me.pivot - 1) = Me.pivot           ' Path's limits are Pivot-1 to Level
            '*** FOR Level = Pivot TO m_epdata.NumLiving
            For Me.Level = Me.pivot To 2 * Me.m_epdata.NumLiving
                If Me.Path(Me.Level - 1) > 0 Then
                    pred = Me.Path(Me.Level - 1)
                Else
                    pred = Me.pivot
                End If
                For Comp = Me.LastComp(Me.Level) To Me.m_epdata.NumLiving
                    If Me.m_epdata.DC(pred, Comp) > 0 Then

                        prey = Comp
                        Me.Path(Me.Level) = 0
                        Me.CheckPath(Me.pivot, prey)
                        If prey = 0 And Comp <> Me.pivot Then
                            GoTo NextComp 'In Path already
                        End If

                        'Only the time spent in the computations
                        'stpwCompTime.Start()

                        If Me.pivot = Comp Then

                            Me.Path(Me.Level) = Comp
                            Me.CyclePrint(Me.CycDC, Cons)
                            Cnt = Cnt + 1

                            'Me.m_manager.updateFoundCycle(Cnt)
                            'prgmsg.Progress = prgmsg.Progress + 1
                            'm_publisher.SendMessage(prgmsg)

                            '     frmWait.Label1(0).Caption = "No of cycles: " + CStr(Cnt)
                            '    UpdateWait()
                            If Me.bStopNetworkAnnalysis Then Exit Sub 'Have identified a cycle
                            Me.FindMinConsump(Cons, Me.MinCons)
                            Me.MinCycDC(Me.CycDC, Me.MinCons)
                            Me.Path(Me.Level) = 0

                            'stpwCompTime.Stop()
                        Else
                            Me.Path(Me.Level) = Comp                     'Include group in Path
                            Me.Path(Me.Level + 1) = 0
                            Me.LastComp(Me.Level) = Comp
                            Me.LastComp(Me.Level + 1) = Me.pivot
                            APred = 1

                            'stpwCompTime.Stop()
                            Exit For              'exit Comp for loop when path found
                            'and continue to next Level
                        End If

                    End If
                    APred = 0          'if program doesn't use EXIT FOR it will reset APred
NextComp:

                    ' DoEvents()
                Next Comp
                If Me.bStopNetworkAnnalysis Then Exit Sub 'Have identified a cycle
                If APred = 0 Then                   'Start backtracking
                    'For Answer = 1 To Level: Debug.Print Format(Path(Answer), "  ##");: Next
                    'Debug.Print
                    If Me.Level > Me.pivot Then Me.LastComp(Me.Level - 1) = Me.Path(Me.Level - 1) + 1
                    Me.Path(Me.Level) = 0
                    Me.Level = Me.Level - 2
                    If Me.Level = Me.pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                End If

            Next Me.Level
NextPivot:

            Me.m_manager.UpdateProgress(My.Resources.STATUS_FINDING_PATHWAY_CYCLES, CSng(Me.pivot / Me.m_epdata.NumLiving))

        Next Me.pivot
        '   Unload(frmWait)
        '   frmWait = Nothing
        'all done
        'prgmsg.ProgressState = eProgressState.Finished
        'm_publisher.SendMessage(prgmsg)

        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ''Timing code
        'Try
        '    stpwRunTime.Stop()
        '    Dim InComp As Single = CSng(stpwCompTime.Elapsed.TotalMinutes / stpwRunTime.Elapsed.TotalMinutes)
        '    System.Console.WriteLine("FindCycles Total run time (sec) = " & stpwRunTime.Elapsed.TotalSeconds.ToString)
        '    System.Console.WriteLine("FindCycles Computation run time (sec) = " & stpwCompTime.Elapsed.TotalSeconds.ToString)
        '    System.Console.WriteLine("FindCycles Percentage in computation (sec) = " & (InComp * 100).ToString)
        'Catch ex As Exception
        '    'opppssss
        '    Debug.Assert(False)
        'End Try
        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        System.Console.WriteLine("FindCycles Done")

    End Sub


    Private Sub CheckPath(Pivot As Integer, ByRef prey As Integer)
        Dim K As Integer
        Try
            For K = Pivot - 1 To Me.Level + 1
                If prey = Me.Path(K) Then
                    prey = 0
                    Exit For
                End If

            Next K
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CheckPath() " & ex.Message, ex)
        End Try


    End Sub

    Private Sub MinCycDC(ByRef CycDC(,) As Single, ByRef MinCons As Single)
        Dim arrow As Integer
        Dim K As Integer, bib As Integer, aa As Integer
        Try
            arrow = 1
            aa = 0
            For K = Me.pivot - 1 To Me.Level
                If Me.Path(K) > 0 Then
                    bib = aa
                    aa = Me.Path(K)
                    If arrow = 0 Then
                        If MinCons < 0 And CycDC(aa, bib) = 0 Then CycDC(aa, bib) = MinCons
                        If CycDC(aa, bib) > MinCons Then CycDC(aa, bib) = MinCons
                    End If
                    arrow = 0
                End If
            Next K
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("MinCycDC() " & ex.Message, ex)
        End Try

    End Sub


    Private Sub SumDiet(ByRef SumDC() As Single)
        Dim i As Integer, j As Integer
        'VC 280ct99: below summed up with dc(i, N1+1) for import but it is in dc(i,0)?????
        Try
            For i = 1 To Me.m_epdata.NumGroups
                SumDC(i) = 0
                For j = 0 To Me.m_epdata.NumGroups '+ 1                             'Import is in 0  (NOT N1+1
                    SumDC(i) = SumDC(i) + Me.m_epdata.DC(i, j)            ' 0 <= SUMDC <= 1
                Next j
                '050113VC: remarked out the line below as import was already added to diets just above
                'If PP(i) > 0 And PP(i) < 1 Then SumDC(i) = SumDC(i) + DC(i, 0)
                If Me.m_epdata.PP(i) = 1 Then SumDC(i) = 0
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("SumDiet() " & ex.Message, ex)
        End Try

        ' Diet is to be made to sum to 1
    End Sub


    Private Sub ThruputByGroup()
        Dim i As Integer, j As Integer
        Try
            Me.ThruPut = 0
            For i = 1 To Me.m_epdata.NumGroups
                Me.PredatOn(i) = 0
                For j = 1 To Me.m_epdata.NumGroups + 1               'Import is in N1+1
                    If j <= Me.m_epdata.NumLiving Then Me.PredatOn(i) = Me.PredatOn(i) + Me.m_epdata.B(j) * Me.m_epdata.QB(j) * Me.m_epdata.DC(j, i)
                Next j

                Me.TrPut(i) = Me.PredatOn(i) + Me.m_epdata.Ex(i) + Me.m_epdata.FlowToDet(i) + Me.m_epdata.Resp(i)
                'vcm_epdata. resp for pp If PP(i) > 0 Then TrPut(i) = TrPut(i) + Im(i)  'For primary prod
                If Me.m_epdata.PP(i) > 0 Then Me.TrPut(i) = Me.TrPut(i) + Me.ImpDet(i) + Me.m_epdata.Resp(i) 'For primary prod
                'vc resp for detr  If i > m_epdata.NumLiving Then TrPut(i) = TrPut(i) + Im(i)     'For detritus
                If i > Me.m_epdata.NumLiving Then Me.TrPut(i) = Me.TrPut(i) + Me.ImpDet(i) + Me.m_epdata.Resp(i) 'For detritus
                Me.ThruPut = CSng(Me.ThruPut + Me.TrPut(i))
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("ThruputBYGroup() " & ex.Message, ex)
        End Try


        'TrPut(i) [called H() by Ulanowics] gives the throughput
        'for each group - including cycles.
        'If there is import to a consumer group then it is assumed
        'that the import is not primary production, and that the
        'trophic level of the import has the same distribution as
        'the trophic levels of the rest of the diet. Therefore the feeding
        'coefficients (Gij, 1<=j<=N1) are made to sum to one, see below and in
        'SumDiet routine.
        'For a mixed producer/consumer (e.g. corals, giant clams) it
        'is assumed that all import is primary production and
        'thus the diet need not sum to 1. For a producer the
        'diet composition must sum to 0 if there is no import (i.e.
        'QB=0, and to 1 if there is import (QB>0).

    End Sub


    Private Sub CalcCycDC(ByRef CycDC(,) As Single)
        Dim i As Integer, j As Integer
        Try
            For i = 1 To Me.m_epdata.NumLiving
                'vc280ct99
                'Import is in 0 not in N1+1
                'For j = 1 To NumGroups + 1
                For j = 0 To Me.m_epdata.NumGroups '+ 1
                    Me.AmCyc = Me.AmCyc - CycDC(i, j)       'CycDC is negative
                    'CycDC is negative
                    If Me.TrPut(i) > 0 Then
                        CycDC(i, j) = CSng(-CycDC(i, j) / Me.TrPut(i))
                        If Me.m_epdata.DC(i, j) < CycDC(i, j) Then CycDC(i, j) = Me.m_epdata.DC(i, j)
                        '***VC*** made this check 07dec95 to avoid negative DC's
                        '041022VC to accomodate PP with diets:
                        If Me.m_epdata.PP(i) = 1 Then CycDC(i, j) = 0
                    End If
                Next j
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcCycDC() " & ex.Message, ex)
        End Try

        'CycDC is the part of the diet composition that needs to be
        'removed to get rid of cycles
    End Sub

    Private Sub CalcDCNoCyc(ByRef CycDC(,) As Single, ByRef SumDC() As Single)
        'DCNoCyc() [Called G by Ulanowicz] is the
        'feeding coefficient matrix, with cycles excluded.
        Dim i As Integer
        Dim j As Integer
        Try
            For i = 1 To Me.m_epdata.NumLiving
                'vc 28oct99: Import is in 0 not in N1+1
                'For j = 1 To NumGroups + 1
                For j = 1 To Me.m_epdata.NumGroups
                    If Me.m_epdata.QB(i) > 0 Then
                        If SumDC(i) > 1.00001 * Me.SumCycDC(i) Then
                            Me.DCNoCyc(i, j) = (Me.m_epdata.DC(i, j) - CycDC(i, j)) / (SumDC(i) - Me.SumCycDC(i)) * (1 - Me.m_epdata.PP(i))
                            '050114VC, multiplied with (1-pp(i)) above to consider groups that
                            'are partly producers. Didn't work for such groups before this is done
                            'This actually makes the line below superfluous:
                            '041022VC below to accomodate PP with diets:
                            If Me.m_epdata.PP(i) = 1 Then Me.DCNoCyc(i, j) = 0
                        End If
                    End If
                Next j
                'vc280ct99 looks like dcnocyc needs to have import in the last element:
                If SumDC(i) > 1.00001 * Me.SumCycDC(i) Then
                    Me.DCNoCyc(i, Me.m_epdata.NumGroups + 1) = (Me.m_epdata.DC(i, 0) - CycDC(i, 0)) / (SumDC(i) - Me.SumCycDC(i))
                End If
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcDCNoCyc() " & ex.Message, ex)
        End Try


        'Diet compositions for consumers are made to sum to one,
        'subtracting what's needed to break all cycles.
        '
        'DCNoCyc() [Called G by Ulanowicz] is the
        'feeding coefficients matrix, with cycles excluded.
        'When the DC is changed to DCNoCyc the unknown parameter
        'for affected groups will also be affected.
        'Probably nothing to do about that.

    End Sub

    Private Sub CalcSumCycDC(ByRef CycDC(,) As Single)

        ' SumCycDC(i) is the proportion of group i's diet
        ' that is attributed to cycling
        Dim i As Integer, j As Integer
        Try

            For i = 1 To Me.m_epdata.NumGroups
                Me.SumCycDC(i) = 0
                'vc280oct99         'import is in element 0 not in n1+1
                'For j = 1 To NumGroups + 1
                For j = 0 To Me.m_epdata.NumGroups
                    Me.SumCycDC(i) = Me.SumCycDC(i) + CycDC(i, j)
                Next j
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcSumCycDC() " & ex.Message, ex)
        End Try


    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculation of keystoneness following Libralato et al 2006.
    ''' </summary>
    ''' <remarks>
    ''' <para>Implemented by VC in May 2009, leaving the interface to JS.</para>
    ''' <para>Doesn't require any saving, but the index needs to be exposed so 
    ''' that we can pick it up for meta analysis.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub Keystoneness()

        'First calculate the relative biomass sum for each LIVING group
        'The keystoneindex is always calculated, just after the MTI has been done
        'it requires no input, we have all

        Dim RelBi(Me.m_epdata.NumLiving) As Double
        Dim TotalImpact(Me.m_epdata.NumLiving) As Double
        Dim sSumB As Single = 0
        Dim dMaxImpact As Double = cCore.NULL_VALUE

        ReDim Me.KeystoneIndex1(Me.m_epdata.NumLiving)
        ReDim Me.KeystoneIndex2(Me.m_epdata.NumLiving)
        ReDim Me.KeystoneIndex3(Me.m_epdata.NumLiving)
        ReDim Me.RelTotalImpact(Me.m_epdata.NumLiving)

        ' Calc max sSumB
        For i As Integer = 1 To Me.m_epdata.NumLiving
            sSumB += Me.m_epdata.B(i)
        Next

        Try 'try is just because biomass could be 0 if model is really stupid

            'next we need the total impact for each LIVING group,
            For i As Integer = 1 To Me.m_epdata.NumLiving
                ' JS note to VC: iterate groups to NumLiving, NOT to NumGroups? Detritus does not predate well ;)
                ' VC response: only include living groups in the calculations (as consumers and as prey)
                'For j As Integer = 1 To m_epdata.NumGroups
                For j As Integer = 1 To Me.m_epdata.NumLiving
                    If i <> j Then
                        TotalImpact(i) += (Me.MTI(i, j) ^ 2)
                    End If
                Next
                TotalImpact(i) = Math.Sqrt(TotalImpact(i))
                dMaxImpact = Math.Max(dMaxImpact, TotalImpact(i))
            Next

            For i As Integer = 1 To Me.m_epdata.NumLiving
                RelBi(i) = Me.m_epdata.B(i) / sSumB
            Next

            For i As Integer = 1 To Me.m_epdata.NumLiving
                ' JS note to VC: The publication states Log, not LN, for the calculations below. 
                '                Since Math.Log implements LN, the calculations below should 
                '                use the Math.Log10 operator
                ' VC response: log10 OK

                ' Keystone index 1 is described in Libralato et al (2006)
                Me.KeystoneIndex1(i) = Math.Log10(TotalImpact(i) * (1 - RelBi(i)))
                ' Keystone index 2 is described in Power et al (1996)
                Me.KeystoneIndex2(i) = Math.Log10(TotalImpact(i) / RelBi(i))
                Me.RelTotalImpact(i) = TotalImpact(i) / dMaxImpact
            Next

            cKeystone3.Calculate(Me.m_epdata, Me)

        Catch ex As Exception
            Debug.Assert(False, "Exception in Keystoneness: " & ex.Message)
        End Try
    End Sub

#End Region ' Network Analysis

#Region " Required PP "

    ''' <summary>
    ''' Calculate public variables needed for required primary production
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Calculates values for "Primary prod. required" tab in EwE5. This was done in DisplayPPReq() in EwE5.</remarks>
    Public Function CalculateRequiredPP() As Boolean

        Dim numPaths As Integer, TabNo As Integer
        Dim bSucces As Boolean = True

        cApplicationStatusNotifier.StartProgress(Me.m_core, "Calculating PPR...", -1)
        'this really has to change 
        Try
            TabNo = 1 'in EwE5 this is called from Tab number one
            Me.Topic = 4 ' if(TabNo = 0, 2, 4)
            Me.FindPaths(numPaths, Me.m_epdata.B, Me.m_epdata.PB, Me.m_epdata.QB, Me.m_epdata.EE, Me.m_epdata.DC, Me.m_epdata.fCatch)

        Catch ex As Exception
            cLog.Write(ex)
            bSucces = False
        Finally

        End Try
        cApplicationStatusNotifier.EndProgress(Me.m_core)

        Return bSucces

    End Function

    Private Sub FindPaths(ByRef NumOfPaths As Integer, ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef fCatch() As Single)

        Me.NumDetPath = 0
        Me.NumLivPath = 0

        '*** DIM LastComp(1 TO m_epdata.NumGroups + 1)
        Dim Answer As Object = Nothing
        Dim Pass As Long  'Integer Found 290598 thanks to Eni / VC
        Dim APred As Integer, Comp As Integer, pred As Integer, prey As Integer, NewPath As Integer
        Dim T1 As Long = 0

        Try

            ReDim Me.SumDetRequired(1, Me.m_epdata.NumGroups)
            ReDim Me.SumPPRequired(1, Me.m_epdata.NumGroups)
            For pred = 0 To 1
                Me.RaiseToDet(pred) = 0
                Me.RaiseToPP(pred) = 0
            Next
            If Me.DoneAlready = False Then Me.DoneAlready = True

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'jb March-16-07 I'm still working on this stuff
            For Me.pivot = 1 To Me.m_epdata.NumLiving         '... prey
                'The pivot element is the starting element
                'If (Topic < 2.5 And Catch(Pivot) > 0) Or ((Topic = 4 Or Topic = 14) And QB(Pivot) > 0) Or Sel = Pivot Then
                If (Me.Topic < 2.5) Or ((Me.Topic = 4 Or Me.Topic = 14) And QB(Me.pivot) > 0) Or Me.Sel = Me.pivot Then
                    'If Topic is 1 or 2 then all groups with catches, if Topic is 4 or 14 then
                    'look only at consumers, otherwise only look at the Sel(ected) group

                    Array.Clear(Me.Path, 0, Me.Path.Length)
                    'init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path)
                    Me.Assign1DimInteg(1, 2 * Me.m_epdata.NumGroups + 1, 1, Me.LastComp)

                    Me.Path(Me.pivot - 1) = Me.pivot    ' Path's limits are Pivot-1 to Level
                    Pass = 0
                    'Dim count As Integer = 0
                    For Me.Level = Me.pivot To 2 * Me.m_epdata.NumGroups
                        'count = count + 1
                        'If count = 111135 Then MsgBox("ready to debug")
                        Pass = Pass + 1
                        If Pass > 10 ^ 7 Then GoTo NextPivot 'MsgBox "Too many pathways, results incomplete": Exit For
                        If Me.Path(Me.Level - 1) > 0 Then
                            pred = Me.Path(Me.Level - 1)
                        Else
                            pred = Me.pivot
                        End If
                        For Comp = Me.LastComp(Me.Level) To Me.m_epdata.NumGroups
                            If QB(pred) = 0 Or (Me.Topic <> 14 And pred > Me.m_epdata.NumLiving) Then     'VCJan97 was = m_epdata.NumGroups
                                APred = 0      ' Here it use EXIT FOR and also reset APred
                                Exit For
                            End If
                            If Me.DCNoCyc(pred, Comp) > 0.00001 Then
                                If pred <= Me.m_epdata.NumLiving Or Me.Topic = 14 Then
                                    prey = Comp
                                    Me.Path(Me.Level) = 0
                                    Me.CheckPath2(prey)         ' Check if Comp is in Path already
                                    If prey = 0 Then        ' Then it is in Path
                                        If Comp <> 0 Then GoTo NextComp2
                                    End If
                                    If Me.pivot = Comp Then
                                        Me.Path(Me.Level) = Comp
                                        Me.Path(Me.Level) = 0
                                    Else
                                        Me.Path(Me.Level) = Comp             ' Include group in Path
                                        Me.Path(Me.Level + 1) = 0
                                        Me.LastComp(Me.Level) = Comp
                                        Me.LastComp(Me.Level + 1) = 1         'Pivot
                                        NewPath = 1
                                        APred = 1
                                        Exit For ' exit Comp for loop when path found                                                   ' and continue to next Level
                                    End If
                                End If
                            End If             ' End DC
                            APred = 0          ' if program doesn't use EXIT FOR it will reset APred
NextComp2:
                        Next Comp

                        If APred = 0 Then                   'Start backtracking
                            If NewPath = 1 Then
                                Me.PathPrintReqPP()
                                Me.CalcRequiredPP(B, PB, QB, EE, DC, fCatch)
                                If prey > Me.m_epdata.NumLiving Then Me.NumDetPath = Me.NumDetPath + 1 Else Me.NumLivPath = Me.NumLivPath + 1
                                NewPath = 0
                            End If
                            If Me.Level > Me.pivot Then Me.LastComp(Me.Level - 1) = Me.Path(Me.Level - 1) + 1
                            Me.Path(Me.Level) = 0
                            Me.Level = Me.Level - 2
                            If Me.Level = Me.pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                        End If

                        'm_manager.UpdateCalculateRequiredPP(NumDetPath + NumLivPath)

                        'If DoWhat <> "Ecosim PPR" And (NumDetPath + NumLivPath) Mod 1000 = 0 Then
                        '    frmWait.Label1(0).Caption = "No of pathways: " + CStr(NumDetPath + NumLivPath)
                        '    UpdateWait()
                        'End If
                        'If AbortRun Then
                        '    Unload(frmWait)
                        '    frmWait = Nothing
                        '    DoWhat = ""
                        '    Screen.MousePointer = vbDefault
                        '    Exit Sub
                        'End If
                        'If m_manager.CancelRequiredPrimaryProdRun Then Exit Sub
                    Next Me.Level

                End If                     'End If for groups with no catches
NextPivot:
            Next Me.pivot
            '        DoWhat = ""
            '        Unload(frmWait)
            '        frmWait = Nothing
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        Catch ex As Exception
            'cLog.Write(ex)
            Throw New ApplicationException("FindPaths()", ex)
        End Try

    End Sub

    Private Sub CheckPath2(ByRef prey As Integer)
        'Private Sub CheckPath2(prey As Integer)
        Try
            Dim K As Integer
            For K = 1 To Me.Level + 1              ' Pivot% - 1 TO Level% + 1
                If prey = Me.Path(K) Then prey = 0 : Exit For
            Next K
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CheckPath2() " & ex.Message, ex)
        End Try
    End Sub


    Private Sub CalcTotalPP()
        Try
            Me.totalPP = 0
            For i As Integer = 1 To Me.m_epdata.NumLiving
                If Me.m_epdata.PP(i) > 0 Then Me.totalPP = Me.totalPP + Me.m_epdata.B(i) * Me.m_epdata.PB(i) * Me.m_epdata.PP(i) '* EE(i%)
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcTotalPP() " & ex.Message, ex)
        End Try
    End Sub


    'Sub CheckIfPPisInput(NoPP As Boolean)
    '    NoPP = False
    '    For K As Integer = 1 To m_epdata.NumGroups
    '        If m_epdata.PP(K) > 0.01 And (m_epdata.missing(K, 1) = True Or m_epdata.missing(K, 2) = True) Then NoPP = True
    '    Next K

    'End Sub


    Private Sub CalcDetFlow(DetFlow As Single)
        Dim i As Integer

        Try
            DetFlow = 0
            For i = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
                If Me.m_epdata.DtImp(i) <> cCore.NULL_VALUE Then
                    DetFlow = DetFlow + Me.m_epdata.DtImp(i) '+ det(0, i)
                End If
            Next i
            For i = 1 To Me.m_epdata.NumLiving
                If Me.m_epdata.PP(i) > 0 Then DetFlow = DetFlow + Me.m_epdata.B(i) * Me.m_epdata.PB(i) * (1 - Me.m_epdata.EE(i)) * Me.m_epdata.PP(i)
            Next
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcDetFlow() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub FlowInternal(Flow(,) As Single, FlowCyc(,) As Single)
        Dim i As Integer, j As Integer

        Try
            For i = 1 To Me.m_epdata.NumGroups
                For j = 1 To Me.m_epdata.NumGroups
                    If j <= Me.m_epdata.NumLiving Then
                        Flow(i, j) = Me.m_epdata.B(j) * Me.m_epdata.QB(j) * Me.m_epdata.DC(j, i)
                    Else
                        Flow(i, j) = CSng(Me.m_epdata.det(0, i))
                    End If
                    If i <= Me.m_epdata.NumLiving And j <= Me.m_epdata.NumLiving Then FlowCyc(i, j) = Flow(i, j)
                Next j
            Next i             'Flow ij (formerly Aij) is amount eaten of
            'group i  by predator j 
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("FlowInternal() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub FlowExternal(Flow(,) As Single, FlowCyc(,) As Single)
        'TCyc = 0
        Try
            For i As Integer = 1 To Me.m_epdata.NumGroups + 3
                If i <= Me.m_epdata.NumGroups Then
                    Flow(Me.m_epdata.NumGroups + 1, i) = Me.ImpDet(i)    'Amount imported
                    Flow(i, Me.m_epdata.NumGroups + 2) = Me.m_epdata.Ex(i)
                    Flow(i, Me.m_epdata.NumGroups + 3) = Me.m_epdata.Resp(i)
                    If i <= Me.m_epdata.NumLiving Then
                        FlowCyc(Me.m_epdata.NumGroups + 1, i) = Me.ImpDet(i)
                        FlowCyc(i, Me.m_epdata.NumGroups + 2) = Me.m_epdata.Ex(i)
                        FlowCyc(i, Me.m_epdata.NumGroups + 3) = Me.m_epdata.Resp(i)
                    End If
                End If
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("FlowExternal() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub EstimTotalCatch()
        Try

            Me.totalCatch = 0
            For i As Integer = 1 To Me.m_epdata.NumGroups
                Me.totalCatch = Me.totalCatch + Me.m_epdata.fCatch(i)
            Next i%
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("EstimTotalCatch() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub CalcRequiredPP(B() As Single, PB() As Single, QB() As Single, EE() As Single, DC(,) As Single, fCatch() As Single)
        Dim pred As Integer
        Dim prey As Integer
        Dim i As Integer
        Dim Si As Integer
        Dim K As Integer
        Dim Raise(1) As Single
        Try
            Si = Me.Path(Me.pivot - 1)
            If (Me.Topic < 2.5) Or ((Me.Topic = 4 Or Me.Topic = 14) And QB(Si) > 0) Or Me.Sel = Si Then
                'If (Topic < 2.5 And Catch(Si) > 0) Or ((Topic = 4 Or Topic = 14) And QB(Si) > 0) Or Sel = Si Then
                ' If Topic is 1 or 2 then all groups with catches.
                ' If Topic is 3 then look only at consumers
                ' otherwise only look at the Sel(ected) group
                For K = Me.pivot - 1 To Me.Level - 2
                    If QB(Me.Path(K)) > 0 And PB(Me.Path(K)) > 0 Then
                        pred = Me.Path(K)
                        prey = Me.Path(K + 1)
                        If K = Me.pivot - 1 Then
                            'If Topic > 3.5 Then 'Start with the total consumption of the predator
                            'Raise(1) refers to the calculation of PPR based on consumption
                            Raise(1) = B(pred) * QB(pred) * DC(pred, prey) '* (SumDC(pred) - SumCycDC(pred)) 'DCNoCyc(pred, prey)
                            'Else        'catch in Raise(0)
                            Raise(0) = fCatch(pred) / (B(pred) * PB(pred)) * Raise(1)
                            'End If
                        Else
                            For i = 0 To 1
                                Raise(i) = Raise(i) * QB(pred) / (PB(pred) * EE(pred)) * DC(pred, prey)  '* (SumDC(pred) - SumCycDC(pred))'DCNoCyc(pred, prey)
                            Next
                        End If
                    End If     'end QB>0
                Next K
            End If

            If prey > Me.m_epdata.NumLiving Then                     'Detritus path
                For i = 0 To 1
                    Me.RaiseToDet(i) = Me.RaiseToDet(i) + Raise(i)
                    Me.SumDetRequired(i, Si) = Me.SumDetRequired(i, Si) + Raise(i)
                Next
            Else                                    'PP path
                For i = 0 To 1
                    Me.RaiseToPP(i) = Me.RaiseToPP(i) + Raise(i)
                    Me.SumPPRequired(i, Si) = Me.SumPPRequired(i, Si) + Raise(i)
                Next
            End If
            'Add up all flow
            For i = 0 To 1
                'If prey <= m_epdata.NumLiving Then
                Me.SumRaise(i, Si) = Me.SumRaise(i, Si) + Raise(i)
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcRequiredPP() " & ex.Message, ex)
        End Try
    End Sub

    Private Sub PathPrintReqPP()
        Dim K As Integer
        'SHARED NumOfPaths
        Dim arrow As Integer
        Dim FirstInPath As Integer

        arrow = 1
        '    PrintC = 0
        For K = Me.pivot - 1 To Me.Level              'PreyOK
            If Me.Path(K) > 0 Then
                If arrow = 1 Then FirstInPath = Me.Path(K)
                If arrow = 0 Then
                    If FirstInPath > 0 Then
                        '                   PRINT USING "Path number: #######"; NumOfPaths + 1;
                        '                   Print FirstInPath;
                        Me.NumPath(FirstInPath) = Me.NumPath(FirstInPath) + 1
                        FirstInPath = 0
                    End If
                    '                Print "<--";
                End If
                '             If Arrow = 0 Then Print Path(k);
                '             PrintC = 1
                Me.NoArrows = Me.NoArrows + 1
                arrow = 0
            End If
        Next K
        If arrow = 0 Then Me.NumOfPaths = Me.NumOfPaths + 1

    End Sub

#End Region ' Required PP

#Region " Pathway Cycles "

    Public Sub FindCycles(ByRef DC(,) As Single, Topic As Integer, Sel As Integer, Sel2 As Integer, ByRef NoPath As Integer, ByRef NoArrows As Integer)
        'Public Sub FindCycles1(DC() As Single, Topic As Integer, Sel As Integer, Sel2 As Integer, NoPath As Integer, NoArrows As Integer)
        Dim Mess As String = ""
        Dim arrow As Integer = 0
        Dim Comp As Integer = 0
        Dim kStart As Integer = 0
        Dim K As Integer = 0
        Dim Pass As Long = 0
        Dim prey As Integer = 0
        Dim PreyOK As Integer = 0
        Dim ProdOK As Integer = 0
        Dim PrintC As Integer = 0
        Dim NewPath As Integer = 0
        Dim NumP As Long = 0

        'jb EwE5 these were at a module level
        Dim pred As Integer
        Dim APred As Integer

        'clear out the results from the last run 
        Me.lstPathways.Clear()

        NumP = 0
        If Topic = 3 Then
            Me.pivot = Sel2
            Array.Clear(Me.Path, 0, Me.Path.Length)
            Me.Assign1DimInteg(1, 2 * Me.m_epdata.NumGroups + 1, 1, Me.LastComp)
            Me.Path(0) = Me.pivot                        ' Path's limits are Pivot-1 to Level
            For Me.Level = 1 To Me.m_epdata.NumLiving                    ' In this case Pred is the prey and
                If Me.Path(Me.Level - 1) > 0 Then        ' Prey the predator
                    pred = Me.Path(Me.Level - 1)
                Else
                    pred = Me.pivot
                End If
                For Comp = Me.LastComp(Me.Level) To Me.m_epdata.NumLiving
                    If Me.bStopNetworkAnnalysis Then Exit Sub
                    If DC(Comp, pred) > 0 Then
                        prey = Comp
                        Me.Path(Me.Level) = 0
                        Me.CheckPath2(prey)
                        If prey = 0 Then
                            If Comp = Me.pivot Then
                            Else
                                GoTo NextComp3
                            End If
                        End If
                        If Me.pivot = Comp Then
                        Else
                            Me.Path(Me.Level) = Comp                      'Include group in Path
                            Me.Path(Me.Level + 1) = 0
                            Me.LastComp(Me.Level) = Comp
                            Me.LastComp(Me.Level + 1) = 1  'Pivot
                            NewPath = 1
                            APred = 1
                            Exit For                'exit Comp for loop when path found
                        End If
                    End If
                    APred = 0          'if program doesn't use EXIT FOR it will reset APred
NextComp3:
                Next Comp
                If APred = 0 Then                   'Start backtracking
                    If NewPath = 1 Then
                        '  GoSub PathPrint
                        'printpath(Topic, Mess, arrow, NoPath)
                        Me.printpath(Topic, Mess, NoArrows, NoPath) 'joeh
                        NewPath = 0
                    End If
                    If Me.Level > 1 Then Me.LastComp(Me.Level - 1) = Me.Path(Me.Level - 1) + 1
                    Me.Path(Me.Level) = 0
                    Me.Level = Me.Level - 2
                    If Me.Level = -1 Then Exit For 'Exit the Level for next and try new pivot
                End If
NextLevel3:
            Next Me.Level
        Else                                             '... for topics 1,2,4,5, 14
            For Me.pivot = 1 To Me.m_epdata.NumLiving                             '... prey
                If Me.m_epdata.QB(Me.pivot) = 0 Then
                    GoTo NextPivot       '... excludes producers
                Else
                    If Topic = 1 Or Topic = 2 Then
                        If Sel = Me.pivot Then
                        Else
                            GoTo NextPivot
                        End If
                    End If
                    Array.Clear(Me.Path, 0, Me.Path.Length)
                    ' Init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path())
                    If Topic = 4 Or Topic = 14 Then
                        Me.Assign1DimInteg(1, 2 * Me.m_epdata.NumGroups + 1, Me.pivot, Me.LastComp)
                    Else
                        Me.Assign1DimInteg(1, 2 * Me.m_epdata.NumGroups + 1, 1, Me.LastComp)
                    End If
                    Me.Path(Me.pivot - 1) = Me.pivot                 ' Path's limits are Pivot-1 to Level
                    Pass = 0

                    '*** changed counter below to go to 2 * N in  March 94, vc
                    '*** This was to avoid problem with too few levels

                    For Me.Level = Me.pivot To 2 * Me.m_epdata.NumLiving
                        Pass = Pass + 1
                        If Pass > 10 ^ 7 Then GoTo NextPivot
                        If Me.Path(Me.Level - 1) > 0 Then
                            pred = Me.Path(Me.Level - 1)
                        Else
                            pred = Me.pivot
                        End If
                        For Comp = Me.LastComp(Me.Level) To Me.m_epdata.NumGroups
                            If DC(pred, Comp) > 0 Then
                                If (Topic <> 14 And pred <= Me.m_epdata.NumLiving) Or Topic = 14 Then
                                    'For topic = 14 also cycles through detritus are OK
                                    prey = Comp
                                    Me.Path(Me.Level) = 0
                                    Me.CheckPath2(prey) '...1
                                    If prey = 0 Then
                                        If Comp = Me.pivot Then
                                        Else
                                            GoTo NextComp
                                        End If
                                    End If
                                    If Me.pivot = Comp Then
                                        Me.Path(Me.Level) = Comp
                                        If Topic = 4 Or Topic = 14 Then
                                            'GoSub CyclePrint1   ' Have identified a cycle
                                            'PrintCycle(Topic, Mess, arrow, NoPath)
                                            Me.PrintCycle(Topic, Mess, NoArrows, NoPath)
                                        End If
                                        Me.Path(Me.Level) = 0
                                    Else
                                        Me.Path(Me.Level) = Comp                  ' Include group in Path
                                        Me.Path(Me.Level + 1) = 0
                                        Me.LastComp(Me.Level) = Comp
                                        If Topic = 5 Then
                                            Me.LastComp(Me.Level + 1) = 1        'Pivot
                                        Else
                                            Me.LastComp(Me.Level + 1) = 1        'Pivot
                                        End If
                                        NewPath = 1
                                        APred = 1
                                        Exit For                              ' exit Comp for loop when path found
                                        ' and continue to next Level
                                    End If
                                End If
                            End If
                            APred = 0  'if program doesn't use EXIT FOR it will reset APred
NextComp:
                        Next Comp
Backtrack:
                        If APred = 0 Then                   'Start backtracking
                            If NewPath = 1 Then
                                'If Topic = 1 Or Topic = 2 Then PreyProd(Topic, Sel2, Mess, arrow, NoPath) 'GoSub PreyProd
                                If Topic = 1 Or Topic = 2 Then Me.PreyProd(Topic, Sel2, Mess, NoArrows, NoPath) 'GoSub PreyProd  'joeh
                                If Topic = 5 Then Me.CountPath() 'GoSub CountPath
                                NewPath = 0
                            End If
                            If Me.Level > Me.pivot Then Me.LastComp(Me.Level - 1) = Me.Path(Me.Level - 1) + 1
                            Me.Path(Me.Level) = 0
                            Me.Level = Me.Level - 2
                            If Me.Level = Me.pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                        End If
NextLevel:
                    Next Me.Level
NextPivot:
                End If
            Next Me.pivot

        End If
        Me.NumberArrows = NoArrows 'joeh
        '   GoTo EndOfFINDCYCLES1
        '-----------------------------------------
        '...***  sub-routines *** ....



        'CountPath:
        '        arrow = 1
        '        For K = pivot - 1 To Level
        '            If Path(K) > 0 Then
        '                If arrow = 0 And Topic > 0 Then NoArrows = NoArrows + 1
        '                arrow = 0
        '            End If
        '        Next K
        '        If arrow = 0 Then NoPath = NoPath + 1
        '        Return

        '        'CyclePrint1:
        '        Mess$ = ""
        '        arrow = 0
        '        'First check if in path already
        '        '(then the cycle doesn't start with the lowest group number in the path
        '        For K = pivot To Level - 1
        '            If Path(pivot - 1) > Path(K) Then arrow = -1
        '            'If so the grpnumber in Path(k) is < than the starting point for the cycle
        '            'and it should not be included in the path. It is there already
        '        Next
        '        If arrow = 0 Then
        '            arrow = 1
        '            For K = pivot - 1 To Level
        '                If Path(K) > 0 Then
        '                    If arrow = 1 Then aa = Path(K)
        '                    If arrow = 0 And Topic > 0 Then
        '                        If aa > 0 Then Mess$ = Mess$ & LTrim(CStr(aa)) : aa = 0
        '                        Mess$ = Mess$ & Chr(171) & "-"
        '                        Mess$ = Mess$ & LTrim(CStr(Path(K)))
        '                        NoArrows = NoArrows + 1
        '                    End If
        '                    arrow = 0
        '                End If
        '            Next
        '            If (Topic = 4 Or Topic = 14) And NoPath <= 50000 Then
        '                'frmCycles.vaEcocycle.maxRows = NoPath
        '                'frmCycles.vaEcocycle.row = NoPath
        '                'frmCycles.vaEcocycle.Text = Mess
        '                ''Print #fnum, Mess
        '            End If
        '            If arrow = 0 Then NoPath = NoPath + 1
        '        End If
        '        Return


        'PathPrint:
        '        Mess$ = ""
        '        arrow = 1
        '        PrintC = 0

        '        kStart = if(Topic = 3, 0, pivot - 1)
        '        For K = kStart To Level              'PreyOK
        '            If Path(K) > 0 Then
        '                If arrow = 1 Then aa = Path(K)
        '                If arrow = 0 And Topic > 0 Then
        '                    If aa > 0 Then
        '                        Mess$ = Mess$ & LTrim(CStr(aa))
        '                        aa = 0
        '                    End If
        '                    If Topic = 3 Then
        '                        Mess$ = Mess$ & "-" & Chr(187)
        '                    Else
        '                        Mess$ = Mess$ & Chr(171) & "-"
        '                    End If
        '                    Mess$ = Mess$ & LTrim(CStr(Path(K)))

        '                    PrintC = 1
        '                    NoArrows = NoArrows + 1
        '                End If
        '                arrow = 0
        '            End If
        '        Next K

        '        If arrow = 0 Then NoPath = NoPath + 1
        '        If ((Topic >= 1 And Topic <= 4) Or Topic = 14) And NoPath <= 50000 Then
        '            ''Print #fnum, Mess
        '            'frmCycles.vaEcocycle.maxRows = NoPath
        '            'frmCycles.vaEcocycle.row = NoPath
        '            'frmCycles.vaEcocycle.Text = Mess
        '        End If
        '        If NoPath = 0 Then MsgBox("No cycle(s) in this system.", 48)
        '        If m_epdata.NumGroups > 10 Then
        '            NumP = NumP + 1
        '            If NumP Mod 1000 = 0 Then
        '                'frmWait.Label1(0).Caption = "No of pathways: " + CStr(NumP)
        '                'UpdateWait()
        '            End If
        '            ' If bStopNetworkAnnalysis Then Exit Sub
        '        End If

        '        Return

        'PreyProd:
        '        'IF PP(Pivot) < 2 THEN GOTO Vers1.0
        '        PreyOK = -1
        '        ProdOK = -1
        '        For K = pivot - 1 To Level
        '            If Sel2 = Path(K) Then PreyOK = K
        '            If PP(Path(K)) > 0 Then ProdOK = K
        '        Next K
        '        If ProdOK = -1 Then Return
        '        If Topic = 2 And PreyOK = -1 Then Return
        '          GoSub PathPrint
        '        Return

        'EndOfFINDCYCLES1:

    End Sub

    'Private Sub PrintCycle(Topic As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub PrintCycle(Topic As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim K As Integer
        Dim arrow As Integer

        mess = ""
        arrow = 0
        'First check if in path already
        '(then the cycle doesn't start with the lowest group number in the path
        For K = Me.pivot To Me.Level - 1
            If Me.Path(Me.pivot - 1) > Me.Path(K) Then arrow = -1
            'If so the grpnumber in Path(k) is < than the starting point for the cycle
            'and it should not be included in the path. It is there already
        Next
        If arrow = 0 Then
            arrow = 1
            For K = Me.pivot - 1 To Me.Level
                If Me.Path(K) > 0 Then
                    If arrow = 1 Then Me.aa = Me.Path(K)
                    If arrow = 0 And Topic > 0 Then
                        If Me.aa > 0 Then mess = mess & LTrim(CStr(Me.aa)) : Me.aa = 0
                        mess = mess & Chr(171) & "-"
                        mess = mess & LTrim(CStr(Me.Path(K)))
                        NoArrows = NoArrows + 1
                    End If
                    arrow = 0
                End If
            Next
            If (Topic = 4 Or Topic = 14) And NoPath <= 50000 Then
                'frmCycles.vaEcocycle.maxRows = NoPath
                'frmCycles.vaEcocycle.row = NoPath
                'frmCycles.vaEcocycle.Text = Mess
                ''Print #fnum, Mess
                'Me.m_manager.UpdatePrintCycle(NoPath)
                Me.lstPathways.Add(mess)
            End If
            If arrow = 0 Then NoPath = NoPath + 1
        End If
        Return

    End Sub


    'Private Sub printpath(Topic As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub printpath(Topic As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim K As Integer
        Dim KStart As Integer
        'Dim numP As Integer
        Dim arrow As Integer

        mess = ""
        arrow = 1
        '    PrintC = 0

        KStart = If(Topic = 3, 0, Me.pivot - 1)
        For K = KStart To Me.Level              'PreyOK
            If Me.Path(K) > 0 Then
                If arrow = 1 Then Me.aa = Me.Path(K)
                If arrow = 0 And Topic > 0 Then
                    If Me.aa > 0 Then
                        mess$ = mess$ & LTrim(CStr(Me.aa))
                        Me.aa = 0
                    End If
                    If Topic = 3 Then
                        mess$ = mess$ & "-" & Chr(187)
                    Else
                        mess$ = mess$ & Chr(171) & "-"
                    End If
                    mess$ = mess$ & LTrim(CStr(Me.Path(K)))

                    '  PrintC = 1
                    NoArrows = NoArrows + 1
                End If
                arrow = 0
            End If
        Next K

        If arrow = 0 Then NoPath = NoPath + 1
        If ((Topic >= 1 And Topic <= 4) Or Topic = 14) And NoPath <= 50000 Then
            ''Print #fnum, Mess
            'frmCycles.vaEcocycle.maxRows = NoPath
            'frmCycles.vaEcocycle.row = NoPath
            'frmCycles.vaEcocycle.Text = Mess
        End If
        If NoPath = 0 Then
            MsgBox("No cycle(s) in this system.", MsgBoxStyle.Information)
        End If
        'If m_epdata.NumGroups > 10 Then
        '    numP = numP + 1
        '    If numP Mod 1000 = 0 Then
        '        'frmWait.Label1(0).Caption = "No of pathways: " + CStr(NumP)
        '        'UpdateWait()
        '    End If
        '    ' If bStopNetworkAnnalysis Then Exit Sub
        'End If
        'Me.m_manager.UpdatePrintPath(NoPath)
        'jb 
        Me.lstPathways.Add(mess)
        Return

    End Sub


    'Private Sub PreyProd(Topic As Integer, sel2 As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub PreyProd(Topic As Integer, sel2 As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim PreyOK As Integer = -1
        Dim ProdOK As Integer = -1
        For K As Integer = Me.pivot - 1 To Me.Level
            If sel2 = Me.Path(K) Then PreyOK = K
            If Me.m_epdata.PP(Me.Path(K)) > 0 Then ProdOK = K
        Next K
        If ProdOK = -1 Then Return
        If Topic = 2 And PreyOK = -1 Then Return
        'printpath(Topic, mess, arrow, NoPath)
        Me.printpath(Topic, mess, NoArrows, NoPath)  'joeh
        Return

    End Sub


    Private Sub CountPath()
        'arrow = 1
        'For K = pivot - 1 To Level
        '    If Path(K) > 0 Then
        '        If arrow = 0 And Topic > 0 Then NoArrows = NoArrows + 1
        '        arrow = 0
        '    End If
        'Next K
        'If arrow = 0 Then NoPath = NoPath + 1
        'Return


    End Sub

#End Region ' Pathway Cycles

#Region " Ecosim "

    Public Function InitForEcosim() As Boolean

        ' RetVal = MsgBox("Estimate PPR? (very time consuming for bigger model)", vbQuestion + vbYesNo, "PPR?")
        '  PPRon = if(RetVal = vbYes, True, False)
        Try

            ' ReDim ValueSim(m_esdata.NTimes)  'value of catch in Ecosim
            ' ReDim CostSim(m_esdata.NTimes)  'cost of catch in Ecosim
            'ReDim CatchSim(m_esdata.NTimes)
            'ReDim Kemptons(m_esdata.NTimes)

            ReDim Me.RelativeSumOfCatch(Me.m_esdata.NTimes)
            ReDim Me.RelativeDiversityIndex(Me.m_esdata.NTimes)
            ReDim Me.TLSim(Me.m_epdata.NumGroups, Me.m_esdata.NTimes)
            ReDim Me.TLCatch(Me.m_esdata.NTimes)
            ReDim Me.RelativeLIndex(Me.m_esdata.NTimes)
            ReDim Me.AbsoluteLIndex(Me.m_esdata.NTimes)
            ReDim Me.RelativePsust(Me.m_esdata.NTimes)
            ReDim Me.AbsolutePsust(Me.m_esdata.NTimes)

            'ReDim Elect(m_epdata.NumLiving, m_epdata.NumGroups, m_esdata.NTimes)
            'ReDim TLSim(m_epdata.NumGroups)
            'ReDim SumDC(m_epdata.NumGroups)

            ReDim Me.BB(Me.m_epdata.NumGroups)

            ReDim Me.ByTL(Me.m_esdata.NTimes, 7, 4)

            'summary for PPRon flag
            ReDim Me.RelativeCatchPPR(Me.m_esdata.NTimes)
            ReDim Me.RelativeCatchDetReq(Me.m_esdata.NTimes)

            'RaiseToPP() and RaiseToDet() were computed in the network anylysis 
            'which has to have been run before the network analysis for ecosim can be initialized
            Me.OrigPPR(0) = Me.RaiseToPP(0)
            Me.OrigPPR(1) = Me.RaiseToDet(0)


            'If Inrow > 0 Then
            '    ReDim ByTLspace(Ntimes, 7, 4, HalfDegreeRow, HalfDegreeCol)
            '    ReDim SAUP_C_Space(NumCatchCodes, HalfDegreeRow, HalfDegreeCol)
            'End If

            'Dim OldCatch As Single
            'Dim OldTL As Single

            'Get the inital starting biomass for EstimateTLofCatch() in Ecosim this is done in SetBBtoStartBiomass()
            'BB() is the biomass at the current time step and not exposed by ecosim so cEcoNetwork uses a local BB
            Array.Copy(Me.m_esdata.StartBiomass, Me.BB, Me.m_esdata.StartBiomass.Length)
            'EstimateTLofCatch(0, BB, CatchSim, OldTL, OldCatch, True)

            Me.OrigDiversityIndex = Me.DiversityIndex(0)

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
            Return False
        End Try

        Return True

    End Function

    Public Function EcosimTimestep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer) As Boolean

        'TLCatch gets computed for each time step
        'it will need to be stored
        Dim relCatch As Single 'relative catch 
        Dim esData As cEcosimDatastructures = DirectCast(EcosimDatastructures, cEcosimDatastructures)

        Try

            'get a local copy of the biomass computed by ecosim at this time step
            Array.Copy(BiomassAtTimestep, Me.BB, BiomassAtTimestep.Length)

            ' If ipct = 6 Then Estimate_Taxon_indices(CInt((iTime - ipct) / 12))
            'EstimateTLsInEcosim(iTime, True)
            'EstimateTLofCatch(iTime, BB, CatchSim, TLCatch, relCatch, True)         'Orig total catch =catchsum

            ' JS 08Jan10: this used to be in EstimateTLofCatch
            If iTime = 0 Then
                relCatch = 0
            ElseIf iTime = 1 Then
                relCatch = 1
            Else
                relCatch = esData.CatchSim(iTime) / esData.CatchSim(1)
            End If

            Me.PrepareUlanowForCallFromEcosim(iTime)

            'Summary data
            'see EwE5 RunModel() "If IndicesOn Then"
            Me.RelativeSumOfCatch(iTime) = relCatch
            Me.RelativeDiversityIndex(iTime) = Me.DiversityIndex(iTime) / Me.OrigDiversityIndex

            Me.TLCatch(iTime) = esData.TLC(iTime)

            For igrp As Integer = 1 To Me.m_epdata.NumGroups
                Me.TLSim(igrp, iTime) = esData.TLSim(igrp)
            Next igrp

            If Me.PPRon Then
                Me.RelativeCatchPPR(iTime) = Me.RaiseToPP(0) / Me.OrigPPR(0)
                Me.RelativeCatchDetReq(iTime) = Me.RaiseToDet(0) / Me.OrigPPR(1)

                Dim LIndexTot As Single = 0
                For i As Integer = 1 To Me.m_core.nLivingGroups
                    LIndexTot += Me.m_manager.LindexSim(i)
                Next

                Me.AbsoluteLIndex(iTime) = LIndexTot
                Me.RelativeLIndex(iTime) = Me.AbsoluteLIndex(iTime) / Me.AbsoluteLIndex(1)

                Me.AbsolutePsust(iTime) = Me.m_manager.CalcPsust(LIndexTot)
                Me.RelativePsust(iTime) = Me.AbsolutePsust(iTime) / Me.AbsolutePsust(1)
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Return False
        End Try

        Return True

    End Function

    Private Function DiversityIndex(t As Integer) As Single

        Select Case Me.m_epdata.DiversityIndexType
            Case eDiversityIndexType.Shannon
                Return Me.m_esdata.ShannonDiversity(t)
            Case eDiversityIndexType.KemptonsQ
                Return Me.m_esdata.Kemptons(t)
            Case Else
                Debug.Assert(False, "Diversity index type not supported")
        End Select
        Return 1
    End Function

    Private Sub PrepareUlanowForCallFromEcosim(Round As Integer)
        '040218VC for Sheila's Network in Ecosim calc's
        '060915VC updated this sub, added more indices, and also added call to Lindeman (plus passing parameters to that sub)

        'ToDo_jb PrepareUlanowForCallFromEcosim PPRon
        'ToDo_jb PrepareUlanowForCallFromEcosim File Saving

        Dim i As Integer, j As Integer, ii As Integer
        Dim Biom As Single
        Dim TotPProd As Single
        Dim TotProd As Single
        Dim fCatch As Single
        Dim BEmig As Single
        Dim SimCatch() As Single
        Dim SimB() As Single
        Dim SimPB() As Single
        Dim SimQB() As Single
        Dim SimEE() As Single
        Dim SDiet(,) As Single
        Dim SimIm() As Single
        Dim SimEx() As Single
        Dim SimResp() As Single
        'Static StopThis As Boolean
        ' Dim RetVal As Object

        ' Static FF As Long
        Try

            Me.NetworkDimensioning()
            ReDim SimCatch(Me.m_epdata.NumGroups)
            ReDim SimB(Me.m_epdata.NumGroups)
            ReDim SimPB(Me.m_epdata.NumGroups)
            ReDim SimQB(Me.m_epdata.NumGroups)
            ReDim SimEE(Me.m_epdata.NumGroups)
            ReDim SDiet(Me.m_epdata.NumGroups, Me.m_epdata.NumGroups)
            ReDim SimIm(Me.m_epdata.NumGroups)
            ReDim SimEx(Me.m_epdata.NumGroups)
            ReDim SimResp(Me.m_epdata.NumGroups)

            'VC: here using a localvariable called PProd, not same as in Ecopath
            Biom = 0
            TotPProd = 0  ' Calculated primary production
            TotProd = 0
            fCatch = 0

            For i = 1 To Me.m_epdata.NumLiving
                SimB(i) = Me.BB(i)
                SimPB(i) = Me.m_esdata.loss(i) / Me.BB(i)

                SimCatch(i) = Me.BB(i) * Me.m_esdata.FishTime(i)
                BEmig = Me.m_epdata.Emig(i) * Me.BB(i)

                'ToDo_jb 5-Jan-2010 NetworkAnalysis PrepareUlanowForCallFromEcosim calculation of simEE uses fishtime() as catch
                'm_esdata.FishTime(i) is a rate not an amount it should use SimCatch() BB(i) * m_esdata.FishTime(i)
                ' SimEE(i) = 1 - (m_esdata.loss(i) - m_esdata.Eatenof(i) - m_esdata.FishTime(i)) / (SimPB(i) * BB(i))
                SimEE(i) = 1 - (Me.m_esdata.loss(i) - Me.m_esdata.Eatenof(i) - SimCatch(i)) / (SimPB(i) * Me.BB(i))

                If Me.m_epdata.PP(i) < 1 Then 'only for consumers
                    If Me.m_epdata.GE(i) > 0 Then SimQB(i) = SimPB(i) / Me.m_epdata.GE(i)
                    SimIm(i) = Me.m_epdata.DC(i, 0) * SimQB(i)

                    'jb 4-Jan-2021 Calculation of Respiration changed to match Ecopath 
                    'Ecosim model that runs flat should have the same NA outputs as Ecopath
                    'This fixes a bug that caused Ecosim Network Analysis outputs to be different from Ecopath values 
                    'Bug Reported by Georgi Daskalov 
                    'SimResp(i) = BB(i) * (SimQB(i) - SimPB(i) - m_epdata.GS(i))

                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    ''Respiration in Ecopath
                    Dim Prod As Single
                    Dim Consump As Single
                    Dim UnAssimConsump As Single
                    'Ecopath Prod = m_epdata.B(i) * m_epdata.PB(i)
                    Prod = Me.BB(i) * SimPB(i)

                    'Ecopath Consump = m_epdata.B(i) * m_epdata.QB(i)
                    Consump = Me.BB(i) * SimQB(i)

                    'Ecopath UnAssimConsump = GS(i) * Consump
                    UnAssimConsump = Me.m_epdata.GS(i) * Consump

                    SimResp(i) = Consump - Prod - UnAssimConsump
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxx

                End If
                SimEx(i) = SimCatch(i)

                fCatch += SimEx(i)
                Biom += Me.BB(i)
                TotProd += Me.BB(i) * SimPB(i)
                If Me.m_epdata.PP(i) > 0 Then TotPProd += SimPB(i) * Me.BB(i) * Me.m_epdata.PP(i)
            Next

            For i = Me.m_epdata.NumLiving + 1 To Me.m_epdata.NumGroups
                SimB(i) = Me.BB(i)
                If Me.m_esdata.ToDetritus(i - Me.m_epdata.NumLiving) > 0 Then SimEE(i) = Me.m_esdata.loss(i) / Me.m_esdata.ToDetritus(i - Me.m_epdata.NumLiving)
                'Emig and Imig removed from below, zero for detritus
                SimEx(i) = (Me.m_esdata.ToDetritus(i - Me.m_epdata.NumLiving) - Me.m_epdata.BA(i) - Me.m_esdata.Eatenof(i)) / Me.BB(i)
                'SimEx(i) = (m_esdata.ToDetritus(i - m_epdata.NumLiving) - m_epdata.BA(i) - m_esdata.Eatenof(i)) ' / BB(i)
            Next i

            For ii = 1 To Me.m_esdata.inlinks
                i = Me.m_esdata.ilink(ii) : j = Me.m_esdata.jlink(ii)
                ' If m_esdata.Eatenby(j) > 0 Then SDiet(j, i) = m_esdata.DCMean(j, i) / m_esdata.Eatenby(j)
                'jb Consumpt() contains the same values as DCMean 
                'DCmean is not used anywhere else so use Consumpt() instead
                If Me.m_esdata.Eatenby(j) > 0 Then SDiet(j, i) = Me.m_esdata.Consumpt(i, j) / Me.m_esdata.Eatenby(j)
            Next

            ''jb SimEx and SimResp are dirrerent then the Ecopath versions calcualted from the same inputs
            Me.Ulanow(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, SimEx, SimResp)
            Me.Lindeman(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, SimEx, SimResp)

            'With Ecopath variables
            'Ulanow(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, m_epdata.Ex, m_epdata.Resp)
            'Lindeman(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, m_epdata.Ex, m_epdata.Resp)


            If Me.PPRon Then
                ' DoWhat = "Ecosim PPR"
                'Estimate PPR: slow
                ReDim Me.NumPath(Me.m_epdata.NumLiving)
                ReDim Me.SumRaise(1, Me.m_epdata.NumLiving)
                Me.NumOfPaths = 0
                'g_num_ppr_gr1 = 0
                'g_num_ppr_gr2 = 0
                ' AbortRun = False
                'EstimateHostMatrixWithNoCycles
                'If Round = 258 Then MsgBox("ready to debug")
                Me.FindPaths(Me.NumOfPaths, SimB, SimPB, SimQB, SimEE, SDiet, SimCatch)
                ' DoEvents()
                '  If AbortRun = True Then Exit Sub
            End If
            '    DoWhat = ""
            'If Round = 1 Then
            '    FF = FreeFile()
            '    Open dbFilepath + "Network.csv" For Output As #FF
            '    Write #FF, "Trput", "Capacity", "Asc import", "Asc flow", "Asc exp", "Asc resp", "Ovh import",
            '    Write #FF, "Ovh flow", "Ovh exp", "Ovh resp", "PCI", "FCI", "PathLength",
            '    '060916VC: Up to now PathLength was the last index, adding more indices after Cape Town IncoFish Workshop
            '    Write #FF, "Export", "Respiration", "Prim prod", "Production", "Biomass", "Catch", "Prop Flow Det",
            '    'Write #FF, "Trans Eff PP", "Trans Eff Det", "Trans Eff Total",
            '    If PPRon Then Write #FF, "Catch PPR", "Catch detritus req" Else Write #FF,
            'End If
            'Write #FF, TruPut, Capacity, Aop, Aip, Aep, Arp, Eop, Eip, Eep, Erp, 100 * Tc / TCyc, 100 * TcD / TruPut,
            'If SumEx + SumResp > 0 Then
            'Write #FF, TruPut / (SumEx + SumResp),
            'Else
            'Write #FF, "",
            'End If
            'Write #FF, SumEx, SumResp, PProd, Production, Biom, m_epdata.fCatch, DetIndex,   'TRavgP(0), TRavgD(0), TRavgT(0),
            ReDim Preserve Me.Throughput(Round)
            ReDim Preserve Me.CapacityEcosim(Round)
            ReDim Preserve Me.AscendImport(Round)
            ReDim Preserve Me.AscendFlow(Round)
            ReDim Preserve Me.AscendExport(Round)
            ReDim Preserve Me.AscendResp(Round)
            ReDim Preserve Me.OverheadImport(Round)
            ReDim Preserve Me.OverheadFlow(Round)
            ReDim Preserve Me.OverheadExport(Round)
            ReDim Preserve Me.OverheadResp(Round)
            ReDim Preserve Me.PCI(Round)
            ReDim Preserve Me.FCI(Round)
            ReDim Preserve Me.PathLength(Round)
            ReDim Preserve Me.Export(Round)
            ReDim Preserve Me.Resp(Round)
            ReDim Preserve Me.PrimaryProd(Round)
            ReDim Preserve Me.Prod(Round)
            ReDim Preserve Me.Biomass(Round)
            ReDim Preserve Me.CatchEcosim(Round)
            ReDim Preserve Me.PropFlowDet(Round)
            ReDim Preserve Me.Ascendency(Round)
            ReDim Preserve Me.AMI(Round)
            ReDim Preserve Me.Entropy(Round)
            ReDim Preserve Me.TotTransferEfficiency(Round)
            ReDim Preserve Me.DetTransferEfficiency(Round)
            ReDim Preserve Me.PPTransferEfficiency(Round)
            ReDim Preserve Me.TotTransferEfficiencyWeighted(Round)
            ReDim Preserve Me.DetTransferEfficiencyWeighted(Round)
            ReDim Preserve Me.PPTransferEfficiencyWeighted(Round)
            Me.Throughput(Round) = Me.TruPut
            Me.CapacityEcosim(Round) = Me.Capacity
            Me.AscendImport(Round) = Me.Aop
            Me.AscendFlow(Round) = Me.Aip
            Me.AscendExport(Round) = Me.Aep
            Me.AscendResp(Round) = Me.Arp
            Me.OverheadImport(Round) = Me.Eop
            Me.OverheadFlow(Round) = Me.Eip
            Me.OverheadExport(Round) = Me.Eep
            Me.OverheadResp(Round) = Me.Erp
            Me.PCI(Round) = 100 * Me.Tc / Me.TCyc
            Me.FCI(Round) = 100 * Me.TcD / Me.TruPut
            Me.PathLength(Round) = Me.TruPut / (Me.SumEx + Me.SumResp)
            Me.Export(Round) = Me.SumEx
            Me.Resp(Round) = Me.SumResp
            Me.PrimaryProd(Round) = TotPProd
            Me.Prod(Round) = TotProd
            Me.Biomass(Round) = Biom
            Me.CatchEcosim(Round) = fCatch
            Me.PropFlowDet(Round) = Me.DetIndex

            Me.Ascendency(Round) = (Me.AscendImport(Round) + Me.AscendFlow(Round) + Me.AscendExport(Round) + Me.AscendResp(Round)) * Me.CapacityEcosim(Round) / 100
            Me.AMI(Round) = Me.Ascendency(Round) / Me.Throughput(Round)
            Me.Entropy(Round) = Me.CapacityEcosim(Round) / Me.Throughput(Round)

            Me.TotTransferEfficiency(Round) = 0
            Me.DetTransferEfficiency(Round) = 0
            Me.PPTransferEfficiency(Round) = 0
            For i = 1 To Me.m_manager.nTrophicLevels
                Me.DetTransferEfficiency(Round) += Me.m_manager.DetTransferEfficiency(i)
                Me.PPTransferEfficiency(Round) += Me.m_manager.PPTransferEfficiency(i)
                Me.TotTransferEfficiency(Round) += Me.m_manager.TotTransferEfficiency(i)
            Next

            Me.PPTransferEfficiencyWeighted(Round) = CSng((Me.m_manager.PPTransferEfficiency(2) * Me.m_manager.PPTransferEfficiency(3) * Me.m_manager.PPTransferEfficiency(4)) ^ (1 / 3))
            Me.DetTransferEfficiencyWeighted(Round) = CSng((Me.m_manager.DetTransferEfficiency(2) * Me.m_manager.DetTransferEfficiency(3) * Me.m_manager.DetTransferEfficiency(4)) ^ (1 / 3))
            Me.TotTransferEfficiencyWeighted(Round) = CSng((Me.m_manager.TotTransferEfficiency(2) * Me.m_manager.TotTransferEfficiency(3) * Me.m_manager.TotTransferEfficiency(4)) ^ (1 / 3))

            'If PPRon Then Write #FF, RaiseToPP(0), RaiseToDet(0) Else Write #FF,
            If Me.PPRon Then
                ReDim Preserve Me.RaiseToPPEcosim(Round)
                ReDim Preserve Me.RaiseToDetEcosim(Round)
                Me.RaiseToPPEcosim(Round) = Me.RaiseToPP(0)
                Me.RaiseToDetEcosim(Round) = Me.RaiseToDet(0)
            End If
            'If Round = Ntimes Then
            '    Close #FF
            '    If StopThis = False Then
            '        RetVal = MsgBox("Network indices saved to " + dbFilepath + "Network.csv" + vbNewLine + vbNewLine + "Press Cancel to avoid this message in this session.", vbOKCancel, "Ecosim: network indices")
            '        If RetVal <> "" Then If RetVal = vbCancel Then StopThis = True
            '    End If
            'End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".PrepareUlanowForCallFromEcosim() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".PrepareUlanowForCallFromEcosim() Error: " & ex.Message, ex)
        End Try

    End Sub

    'Private Sub Estimate_Taxon_indices(Year As Integer)
    '    'Dim i As Integer
    '    'Dim j As Integer
    '    'Dim L As Integer
    '    'Dim row As Integer
    '    'Dim col As Integer
    '    'Dim Code As Integer
    '    'Dim Grp As Integer
    '    ''Villy 14Jan2001
    '    ''Used to estimate biomass, pb, qb for each Taxon group
    '    'If TaxonCount > 0 Or NoTL > 0 Then
    '    '    For i = 1 To TaxonCount
    '    '        Grp = TaxonGrp(i)
    '    '        For L = 1 To TaxonCode(0, 1, i) '=TaxLevel
    '    '            'TaxLevel is 1=ISSCAAP, 2 Cla, 3 Ord, 4 Fam, 5, Gen, 6, spe
    '    '            'TaxonCode Stores:
    '    '            '1Dim: 0 is for codes, 1 is a ref to where code occured first time
    '    '            '2Dim: 0 TaxonKey, 1 TaxLevel, 2 ISSCAAP, 3 ClaCode, 4 OrdCode, 5 FamCode, 6 GenCode, 7 SpeCode
    '    '            '3Dim: Ref to number of taxo-records in model
    '    '            Code = TaxonCode(1, L + 1, i)
    '    '            If Code > 0 Then
    '    '                '  Code is the first occurence of the Taxoncode, if the code is < than i
    '    '                '  it has occurred before, and it need not be plotted
    '    '                'Biomass
    '    '                TaxonValue(L, Year, Code, 0) = TaxonValue(L, Year, Code, 0) + BB(Grp) * TaxonProp(i)
    '    '                'Production = P/B * B
    '    '                TaxonValue(L, Year, Code, 1) = TaxonValue(L, Year, Code, 1) + loss(Grp) * TaxonProp(i)
    '    '                'Consumption
    '    '                TaxonValue(L, Year, Code, 2) = TaxonValue(L, Year, Code, 2) + Eatenby(Grp) * TaxonProp(i)
    '    '                'Catch
    '    '                TaxonValue(L, Year, Code, 3) = TaxonValue(L, Year, Code, 3) + FishTime(Grp) * BB(Grp) * TaxonProp(i)
    '    '                'Value
    '    '                For j = 1 To NumGear        'Discarded fish has no value
    '    '                    If Landing(j, Grp) + Discard(j, Grp) > 0 Then
    '    '                        TaxonValue(L, Year, Code, 4) = TaxonValue(L, Year, Code, 4) + BB(Grp) * FishTime(Grp) * Market(j, Grp) * Landing(j, Grp) / (Landing(j, Grp) + Discard(j, Grp))
    '    '                    End If
    '    '                Next
    '    '            End If
    '    '        Next
    '    '        'If HalfDegreeRow > 0 Then 'Do a whole lot of calculations:
    '    '        '    L = TaxonCode(0, 1, i)
    '    '        '    Code = TaxonCode(1, L + 1, i)
    '    '        '    For row = 1 To HalfDegreeRow : For col = 1 To HalfDegreeCol
    '    '        '            If Depth(row, col) > 0 Then
    '    '        '                TaxonSpace(Year, Code, 0, row, col) = TaxonSpace(Year, Code, 0, row, col) + HalfDegreeBcell(row, col, Grp) * BB(Grp) * TaxonProp(i)
    '    '        '                TaxonSpace(Year, Code, 1, row, col) = TaxonSpace(Year, Code, 1, row, col) + HalfDegreeBcell(row, col, Grp) * loss(Grp) * TaxonProp(i)
    '    '        '                TaxonSpace(Year, Code, 2, row, col) = TaxonSpace(Year, Code, 2, row, col) + HalfDegreeBcell(row, col, Grp) * Eatenby(Grp) * TaxonProp(i)
    '    '        '                TaxonSpace(Year, Code, 3, row, col) = TaxonSpace(Year, Code, 3, row, col) + HalfDegreeBcell(row, col, Grp) * FishTime(Grp) * BB(Grp) * TaxonProp(i)
    '    '        '                For j = 1 To NumGear        'Discarded fish has no value
    '    '        '                    'If Landing(j, Grp) + Discard(j, Grp) > 0 Then
    '    '        '            If Catch(j) > 0 Then
    '    '        '                TaxonSpace(Year, Code, 4, row, col) = TaxonSpace(Year, Code, 4, row, col) + HalfDegreeBcell(row, col, 0) * BB(Grp) * FishTime(Grp) * Market(j, Grp) * Landing(j, Grp) / Catch(j) '(Landing(j, Grp) + Discard(j, Grp))
    '    '        '                    End If
    '    '        '                Next
    '    '        '            End If
    '    '        '        Next : Next
    '    '        'End If
    '    '    Next
    '    'End If

    'End Sub

#End Region ' Ecosim

#Region " Panic "

    Private Sub SendMessage(msg As cMessage)
        If (Me.m_core IsNot Nothing) Then
            Try
                Me.m_core.Messages.SendMessage(msg)
            Catch ex As Exception

            End Try
        End If
    End Sub

#End Region ' Panic

End Class
