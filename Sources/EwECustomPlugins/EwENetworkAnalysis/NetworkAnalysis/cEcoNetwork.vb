'==============================================================================
'
' $Log: cEcoNetwork.vb,v $
' Revision 1.1  2008/09/26 07:30:59  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.29  2008/06/25 02:25:21  joeh
' Compute and send ecosim NA data to csv file - Take 2
'
' Revision 1.28  2008/06/14 00:00:28  joeh
' Compute and send ecosim NA data to csv file
'
' Revision 1.27  2007/09/25 19:02:51  joeb
' Fixed frmHideGroups bug
'
' Revision 1.26  2007/06/28 19:33:30  joeh
' Add tool strip button Cancel
'
' Revision 1.25  2007/06/26 21:15:39  joeh
' Remove FindCycles() during network analysis
'
' Revision 1.24  2007/06/20 18:13:56  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Explicit On
Option Strict On
Imports EwECore

Imports System.Xml
Public Class cEcoNetwork

#Region "Private data"

    Private m_manager As cNetworkManager

    Private m_HideGroups As frmHideGroups  'joeh

    ' Private m_publisher As cMessagePublisher

    Private m_epdata As cEcopathDataStructures
    Private m_esdata As cEcosimDatastructures
    Private m_core As cCore

    Private CCD As Integer

    Private Ascend As Single

    Private im() As Single

    Private CatchSum As Single
    'Private DetIndex As Single 'Proportion of total flow originating from detritus: DetIndex.

    Private SumDC() As Single

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

    Private TrPut() As Single

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
    Private TL() As Single

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

#Region "Private Ecosim Data"

    'biomass computed by Ecosim at the current time step
    'Ecoism.BB(ngroups) is private to access it from this plugin it gets passed as an agrument at each time step (see EcosimTimestep())
    'then copied into a local BB() array
    Private BB() As Single
    Private OrigKempton As Single
    Private OrigPPR(1) As Single

#End Region

#End Region

#Region "Public data"

    ''' <summary>
    ''' Stop the network annalysis routine from running
    ''' </summary>
    ''' <remarks>In EwE5 this was called AbortRun and was global. It stopped all kinds of things.</remarks>
    Public bStopNetworkAnnalysis As Boolean

    Public PPRon As Boolean

#Region "Public Flows (Trophic level decomposition) variables"

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
#End Region

#Region "Public Ascendancy variables"

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
    Public Q() As Single


    'see EwE5 DisplayFlowIndices
    Public TruPut As Single, SumAc As Single, SumEc As Single, SumCc As Single
    Public SumEx As Single, SumResp As Single, Tc As Single, TCyc As Single, TcD As Single

#End Region

#Region "Public Flows and Biomass variables"

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
#End Region

#Region "Public Required Primary Production varaibles"
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

#End Region

#Region "Public Cycles and Pathways variables"
    Public lstPathways As New List(Of String)
    Public NoArrows As Integer 'This declared NoArrows is used only in PathPrintReqPP()  joeh
    ' The NoArrows in FindCycles(), PrintPath(), PrintCycle() and PreyProd()
    ' is NOT the NoArrows above but a variable passed from its caller.  The NoArrows in 
    ' FindCycles() is exposed as NumberArrows below
    Public NumberArrows As Integer
#End Region

#Region "Public Ecosim Variables"

    Private Kemptons() As Single
    Private TLSim() As Single

    Private TLC() As Single 'TL of catch in Ecosim
    '  Public CostSim() As Single
    ' Public ValueSim() As Single
    Private CatchSim() As Single
    Private Elect(,,) As Single
    Private ByTL(,,) As Single

    Private BiomassFish As Single
    Private BiomassInvert As Single

    Public FIB() As Single  'FIB-index in ecosim

    Public RelativeSumOfCatchPlot() As Single
    Public RelativeKemptonsPlot() As Single

    'Trophic level of catch
    Public TLCatchPlot() As Single

    Public TLSimPlot(,) As Single 'groups,time

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
    Public AscendTotal() As Single
    Public AMI() As Single
    Public Entropy() As Single

#End Region

#End Region

#Region "Constructors"

    'Public Sub New(ByRef Manager As cNetworkManager)
    Public Sub New(ByRef Manager As cNetworkManager) 'joeh
        m_manager = Manager

        OrigKempton = Single.Epsilon 'for /0 error
    End Sub

#End Region

#Region "Public Properties"

    Public WriteOnly Property EcopathData() As cEcopathDataStructures
        Set(ByVal value As cEcopathDataStructures)
            m_epdata = value
        End Set
    End Property


    Public WriteOnly Property EcosimData() As cEcosimDatastructures
        Set(ByVal value As cEcosimDatastructures)
            m_esdata = value
        End Set
    End Property


    Public WriteOnly Property HideGroupsForm() As frmHideGroups
        Set(ByVal value As frmHideGroups)
            m_HideGroups = value
        End Set
    End Property




    'Public ReadOnly Property MessagePublisher() As cMessagePublisher
    '    Get
    '        Return m_publisher
    '    End Get
    'End Property

#End Region

#Region "Network Analysis"



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

        Dim i As Integer ', chk As Integer
        Dim strErr As String = ""
        'ReadF:

        Debug.Assert(m_epdata IsNot Nothing, Me.ToString & ".RunNetworkAnalysis() Ecopath data has not been initialized.")
        Try

            If m_epdata Is Nothing Then
                Throw New ApplicationException("Ecopath data has not been initialized properly.")
                Return False
            End If

            FoundCycles = False
            NetworkDimensioning()

            ' EconetMain_g% = 1
            'jb this is weird I can’t put a Try Catch statement into either Ulanow or Lindeman           
            Try
                Me.m_manager.UpdateNetworkAnalysis(1)
                strErr = "Ulanow()"
                Ulanow(m_epdata.B, m_epdata.PB, m_epdata.QB, m_epdata.EE, m_epdata.DC, im, m_epdata.Ex, m_epdata.Resp)

                Me.m_manager.UpdateNetworkAnalysis(2)
                strErr = "Lindeman()"
                Lindeman(m_epdata.B, m_epdata.PB, m_epdata.QB, m_epdata.EE, m_epdata.DC, im, m_epdata.Ex, m_epdata.Resp) '
                Me.m_manager.UpdateNetworkAnalysis(3)
            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & "." & strErr, ex)
            End Try

            ' PROCEED TO THE MIXED TROPHIC IMPACT ROUTINE
            CatchSum = 0

            For i = 1 To m_epdata.NumLiving
                CatchSum = CatchSum + m_epdata.Landing(0, i) + m_epdata.Discard(0, i) 'Catch(i%)
            Next i
            If CatchSum > 0 Or m_epdata.NumFleet > 0 Then
                CCD = 1
                ReDim MTI(m_epdata.NumGroups + m_epdata.NumFleet, m_epdata.NumGroups + m_epdata.NumFleet)
            Else
                CCD = 0
                ReDim MTI(m_epdata.NumGroups, m_epdata.NumGroups)
            End If

            Impacts()

            Me.m_manager.UpdateNetworkAnalysis(4)
            InitReqPP()
            Me.m_manager.UpdateNetworkAnalysis(5)

            'activate the textStream to save the output to file
            'Dim textStream As New System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory() & "NetworkOutput.csv")
            DumpResultsToStream(Nothing)
            Me.m_manager.UpdateNetworkAnalysis(6)

        Catch ex As Exception
            ' Debug.Assert(False, ex.Message)
            cLog.Write(ex)
            Throw New ApplicationException("RunNetworkAnalysis()", ex)
        End Try


        'Count = 2
    End Function


    ''' <summary>
    ''' Initialization of the Required PP routines
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Renamed from ReqPPMain() in EwE5. This needs to run after RunNetworkAnalysis and before CalculateRequiredPP</remarks>
    Private Function InitReqPP() As Boolean
        'Function ReqPPMain() As Integer 'EwE5 name

        Try

            'prepares the buffers.
            ReDim DCNoCyc(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
            ReDim CycDC(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
            ReDim im(m_epdata.NumGroups)
            ReDim LastComp(2 * m_epdata.NumGroups + 1)
            ReDim PredatOn(m_epdata.NumGroups)
            ReDim SumCycDC(m_epdata.NumGroups)
            ReDim TrPut(m_epdata.NumGroups + 3)
            ReDim Path(2 * m_epdata.NumGroups + 2)
            ReDim Cons(m_epdata.NumLiving)

            ReDim Flow(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            ReDim FlowCyc(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            ReDim NumPath(m_epdata.NumLiving)
            ReDim SumRaise(1, m_epdata.NumLiving)

            'jb CheckIfPPisInput() uses the missing() array that is private to Ecopath
            'this is the number of missing parameters in the Ecopath inputs
            'If there are any missing parameters this can not be run so calling CheckIfPPisInput() is pointless in this version
            'CheckIfPPisInput(NoPP)
            CalcTotalPP()

            'default return value.

            'validates data.
            If totalPP = 0 Then
                MsgBox("No utilized primary producers.")
                Debug.Assert(False)
                Return False
                ' Exit Function
            End If

            'continue computing.
            CalcDetFlow(DetFlow)
            FlowInternal(Flow, FlowCyc)
            ImportAmount()
            FlowExternal(Flow, FlowCyc)
            SumDiet(SumDC)
            ThruputByGroup()
            'If FoundCycles = False Then FindCycles(Cons)  'Joeh
            FoundCycles = True
            CalcCycDC(CycDC)
            CalcSumCycDC(CycDC)
            CalcDCNoCyc(CycDC, SumDC)
            EstimTotalCatch()
            Return True

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("InitReqPP()" & ex.Message, ex)
            Return False
        End Try

    End Function


    ''' <summary>
    ''' Dump the ouput for debugging to the Console or a text stream
    ''' </summary>
    ''' <param name="textStream">Nothing for output to the console. A valid text stream for writting to file</param>
    ''' <remarks>This can write to the console or a file stream</remarks>
    Public Sub DumpResultsToStream(Optional ByRef textStream As System.IO.TextWriter = Nothing)

        If textStream IsNot Nothing Then
            System.Console.SetOut(textStream)
        End If

        System.Console.WriteLine("---------------Network Analysis-------------------")
        System.Console.WriteLine("Trophic level decomposition / Absolute flows")

        For igrp As Integer = 1 To m_epdata.NumGroups
            System.Console.Write(m_epdata.GroupName(igrp))
            For iTl As Integer = 1 To NoTL
                System.Console.Write(", " & AM(igrp, iTl).ToString("0.000"))
            Next
            System.Console.Write(vbNewLine)
        Next

        System.Console.WriteLine("--------Mixed trophic impacts----------")
        For row As Integer = 1 To m_epdata.NumGroups
            System.Console.Write(m_epdata.GroupName(row))
            For col As Integer = 1 To m_epdata.NumGroups 'CCD%
                System.Console.Write(", " & MTI(row, col).ToString("0.000"))
            Next
            System.Console.Write(vbNewLine)
        Next

        System.Console.Out.Close()

    End Sub


    Private Sub NetworkDimensioning()
        Try
            ReDim Cons(m_epdata.NumLiving)
            ReDim RaiEm1(m_epdata.NumGroups)  'This is for raising flows
            ReDim RaiEm2(m_epdata.NumGroups)   'This is for raising emergy flows

            'edited: 013197 VC
            ReDim BenificialPredation(m_epdata.NumGroups + m_epdata.NumFleet, m_epdata.NumGroups + m_epdata.NumFleet)
            If m_epdata.NumFleet > 3 Then
                ReDim MTI(m_epdata.NumGroups + m_epdata.NumFleet, m_epdata.NumGroups + m_epdata.NumFleet)
            Else
                ReDim MTI(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            End If

            ReDim AM(m_epdata.NumGroups, m_epdata.NumGroups)
            ReDim AM_Abs(m_epdata.NumGroups, m_epdata.NumGroups)
            ReDim DTA(m_epdata.NumGroups + 1)
            ReDim DTAD(m_epdata.NumGroups + 1)
            'ReDim DTD( m_epdata.NumGroups + 1) 
            ReDim EXA(m_epdata.NumGroups)
            ReDim EXAD(m_epdata.NumGroups)

            'edited: 013097 VC
            ReDim FC(m_epdata.NumGroups + m_epdata.NumFleet, m_epdata.NumGroups + m_epdata.NumFleet)
            'FC is the hostmatrix(host, predator)=predator composition
            ReDim RSP(m_epdata.NumGroups)
            ReDim RSPD(m_epdata.NumGroups)
            ReDim TrEm1(m_epdata.NumGroups)
            ReDim TrEm2(m_epdata.NumGroups)

            '************************************************
            'edited as VC's advice eli 052396.
            'ReDim TRP( m_epdata.NumGroups)  
            'ReDim TRPD( m_epdata.NumGroups)  
            ReDim TRP(m_epdata.NumGroups + 2)
            ReDim TRPD(m_epdata.NumGroups + 2)
            ReDim TrpShow(m_epdata.NumGroups + 2)
            'end of modif.....
            '************************************************

            ReDim TrPut(m_epdata.NumGroups + 3)

            ReDim Q(m_epdata.NumGroups + 10)
            ReDim F(m_epdata.NumGroups, m_epdata.NumGroups)
            ReDim SumDC(m_epdata.NumGroups)

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

        Dim Consu As Single, T1 As Single, T2 As Single, Tr1 As Single, TotTr As Single, TotalB As Single

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

        ReDim ADj(m_epdata.NumGroups)
        ReDim APj(m_epdata.NumGroups)
        ReDim A1(m_epdata.NumGroups)
        ReDim A2(m_epdata.NumGroups)
        ReDim CA(m_epdata.NumGroups)
        ReDim CAD(m_epdata.NumGroups)
        ReDim CycDC(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
        'This array contains the amount that is to be subtracted
        'from diet compositions after conversion to proportion of diet
        'in order to break all cycles by removing the min amount circulated.
        'The amount is calculated in the FindCycles module
        'ReDim Cons(0 To N) As Single
        ReDim DCNoCyc(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
        ReDim G2(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
        ReDim G3(m_epdata.NumGroups + 1, m_epdata.NumGroups + 1)
        'ReDim TrPut(0 To m_epdata.NumGroups) As Double
        ReDim HNoC(m_epdata.NumGroups)
        ReDim im(m_epdata.NumGroups)
        ReDim Impo(m_epdata.NumGroups)
        ReDim ImpD(m_epdata.NumGroups)
        ReDim LastComp(2 * m_epdata.NumGroups + 1)
        ReDim last(2 * m_epdata.NumLiving)
        ReDim Path(2 * m_epdata.NumGroups + 2)
        ReDim Predat(m_epdata.NumGroups)
        ReDim PredatD(m_epdata.NumGroups)
        ReDim PredatOn(m_epdata.NumGroups)
        ReDim PredNoC(m_epdata.NumGroups)
        ReDim QTL(m_epdata.NumGroups)
        ReDim SumCycDC(m_epdata.NumGroups)
        'ReDim SumDC( m_epdata.NumGroups) As Single
        ReDim TropLvl(m_epdata.NumGroups)
        ReDim TL(m_epdata.NumGroups)

        'added by eli 052396
        ReDim Ad(m_epdata.NumGroups + 1, m_epdata.NumGroups)
        ReDim Ap(m_epdata.NumGroups + 1, m_epdata.NumGroups)
        ReDim TLsort(m_epdata.NumGroups + 1)
        ReDim BbyTL(m_epdata.NumGroups + 1)
        'ReDim BbyGp(m_epdata.NumGroups + 1)
        ReDim CbyTL(m_epdata.NumGroups + 1)
        'ReDim CbyGp(m_epdata.NumGroups + 1)
        'end of modif. eli 052396
        '*************************************************

        'For i = 1 To m_epdata.NumGroups
        '    BbyGp(i) = m_epdata.B(i)
        'Next

        'For i = 1 To m_epdata.NumGroups
        '    CbyGp(i) = m_epdata.fCatch(i)
        'Next

        For i = 1 To m_epdata.NumGroups
            A1(i) = 0 : A2(i) = 0 : PredatOn(i) = 0
            Predat(i) = 0 : PredatD(i) = 0 : im(i) = 0
            DTA(i) = 0 : TRP(i) = 0 : EXA(i) = 0
            EXAD(i) = 0 : RSP(i) = 0 : RSPD(i) = 0
            TrEm1(i) = 0 : TrEm2(i) = 0
            BbyTL(i) = 0
            CbyTL(i) = 0
            TrpShow(i) = 0
            For j = 1 To m_epdata.NumGroups
                DCNoCyc(i, j) = 0 : G2(i, j) = 0 : G3(i, j) = 0
                Ap(i, j) = 0 : Ad(i, j) = 0 : SumDC(j) = 0
                AM(i, j) = 0 : CycDC(i, j) = 0
            Next j
        Next i

        ImportAmount()
        SumDiet(SumDC)
        ThruputByGroup()
        'If FoundCycles = False Then FindCycles(Cons) 'Joeh
        FoundCycles = True
        'CycDC contains the proportion of the diet that is the minimum
        'amount in a cycle and should be removed to break the cycle.
        'This amount is subtracted from all flows in the cycle.

        CalcCycDC(CycDC)
        CalcSumCycDC(CycDC)
        CalcDCNoCyc(CycDC, SumDC)

        'Diet compositions for consumers are made to sum to one.
        'Diet compositions for strict producers are unaffected.
        'DCNoCyc is the feeding coefficients matrix, with cycles excluded.
        'Next calculates PredNoC(i) - with cycles excluded

        For i = 1 To m_epdata.NumGroups
            PredNoC(i) = 0                     'Predation on group i
            For j = 1 To m_epdata.NumLiving
                If DC(j, i) > 0 Then
                    'Cons = B(j) * QB(j) * (SumDC(j) - SumCycDC(j)) / SumDC(j) * DCNoCyc(j, i)
                    Consu = B(j) * QB(j) * (SumDC(j) - SumCycDC(j)) * DCNoCyc(j, i)
                    PredNoC(i) = PredNoC(i) + Consu
                End If
            Next j
            'TrPut is called H() by Ulanowics
            HNoC(i) = PredNoC(i) + Ex(i) + m_epdata.FlowToDet(i) + Resp(i)
            'Next for primary producers add import:
            If m_epdata.PP(i) > 0 And i <= m_epdata.NumLiving Then HNoC(i) = HNoC(i) + im(i) * m_epdata.PP(i)
            'VC Jan 97: Above changed. Before it did not have the condition that it was
            'only for living groups. Therefore import to detritus was added twice.
            'I also think that the m_epdata.pp(i) shold be multiplied so that only the part of the import
            'which relates to TL I is included. Hence, this is assumed to be proportional to m_epdata.pp.
            'For detritus groups add import:
            If i > m_epdata.NumLiving Then HNoC(i) = HNoC(i) + im(i)
        Next i

        For i = 1 To m_epdata.NumGroups
            A1(i) = CSng(1 - CInt(10000 * SumDC(i)) / 10000)
            'If m_epdata.pp(i) = 0 Then A1(i) = 0
            If m_epdata.PP(i) = 1 Then A1(i) = 1
            A1(i) = m_epdata.PP(i)
            Ap(1, i) = A1(i)
        Next i

        For i = 1 To m_epdata.NumGroups
            For j = 1 To m_epdata.NumGroups + 1
                G2(i, j) = DCNoCyc(i, j)
            Next j
        Next i

        NoTL = m_epdata.NumGroups
        For K = 1 To m_epdata.NumGroups             'Max no of trophic levels is m_epdata.NumGroups
            T1 = 0 : T2 = 0
            ErrCode = MatMultS(G2, DCNoCyc, G3)     'g3 = g2 * dcnocyc
            If ErrCode <> 0 Then
                'TODO_jb message box??
                'MsgBox("There is an error in Lindeman module, (Code = " & ErrCode & "),", vbOKOnly & vbCritical, "Module aborted. ")
                Return
                'GoTo EndOfLinde
            End If
            'MsgBox "No of trophic levels: " & k

            For i = 1 To m_epdata.NumLiving
                A2(i) = 0
                For j = 1 To m_epdata.NumLiving
                    A2(i) = A2(i) + A1(j) * G2(i, j)  'Matrix multiplication
                Next j
            Next i

            For i = 1 To m_epdata.NumLiving
                Ap(K + 1, i) = A2(i)                    '... p refers to prim producer
                'Ad(k + 1, i) = G2(i, m_epdata.NumGroups)        '... d refers to detritus
                'VC311099 moved line above to within new fornext below to sum over all detritus
                'If i <= m_epdata.NumGroups Then
                T1 = T1 + A2(i)
                For j = m_epdata.NumLiving + 1 To m_epdata.NumGroups
                    Ad(K + 1, i) = Ad(K + 1, i) + G2(i, j)
                    T2 = T2 + G3(i, j)    'was G3(i, numgrups)
                Next
                'End If
                For j = 1 To m_epdata.NumGroups
                    If G3(i, j) > 0.000001 Then
                        G2(i, j) = G3(i, j)
                    Else
                        G2(i, j) = 0
                    End If
                Next j
            Next i
            If T1 + T2 < 0.00001 Then NoTL = K + 1 : Exit For
        Next K

        'MsgBox "NOTL=" & NoTL

        For i = 1 To m_epdata.NumGroups
            APj(i) = 0
            ADj(i) = 0
            For j = 1 To m_epdata.NumGroups
                APj(i) = APj(i) + Ap(j, i)
                ADj(i) = ADj(i) + Ad(j, i)
            Next j
        Next i

        For i = 1 To m_epdata.NumLiving                   'This it to make AP and AD sum to 1
            If APj(i) + ADj(i) < 1 And APj(i) + ADj(i) > 0 Then
                For j = 1 To m_epdata.NumLiving
                    Ap(j, i) = Ap(j, i) / (APj(i) + ADj(i))
                    Ad(j, i) = Ad(j, i) / (APj(i) + ADj(i))
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

        For i = 1 To NoTL
            For j = 1 To m_epdata.NumLiving
                AM(i, j) = Ap(i, j) + Ad(i, j)
            Next j
        Next i
        For j = m_epdata.NumLiving + 1 To m_epdata.NumGroups
            AM(1, j) = 1                      'All activity of detritus is on
        Next j                                    'trophic level I.
        'end of modif.
        '*****************************************

        For i = 1 To m_epdata.NumGroups
            For j = 1 To NoTL
                If AM(j, i) = 0 Then
                Else
                    If i <= m_epdata.NumLiving Then
                        BbyTL(j) = BbyTL(j) + AM(j, i) * B(i)
                        CbyTL(j) = CbyTL(j) + AM(j, i) * m_epdata.fCatch(i)
                    End If
                End If
            Next j
        Next i
        'Start printing absolute flows

        'Sorts grp's after trophic level and stores ranking in TLsort(m_epdata.NumGroups)
        SortTL()
        ' GoSub SortTL 'EwE5 code      

        TLChk = 2
        For i = 1 To m_epdata.NumGroups
            ii = CInt(TLsort(m_epdata.NumGroups + 1 - i))       'TLsort(i) gives a reference to the
            'groups sorted after trophic level
            For j = 1 To NoTL
                If AM(j, ii) = 0 Then

                Else
                    AM_Abs(j, ii) = AM(j, ii) * Q(ii)
                    QTL(j) = QTL(j) + AM_Abs(j, ii)
                End If
            Next j
        Next i
        '***

        SumTrp = 0
        For i = 1 To NoTL
            Sum = 0
            For j = 1 To m_epdata.NumLiving
                If APj(j) > 0 Then
                    Predat(i) = Predat(i) + PredNoC(j) * Ap(i, j)
                    TRP(i) = TRP(i) + HNoC(j) * Ap(i, j)
                    'If GrpsToShow(j) Then TrpShow(i) = TrpShow(i) + HNoC(j) * Ap(i, j)
                    If m_HideGroups.IsGroupShown(m_manager.GroupName(j)) Then TrpShow(i) = TrpShow(i) + HNoC(j) * Ap(i, j) 'joeh
                    'TrpShow(i) = TrpShow(i) + HNoC(j) * Ap(i, j) 'joeh
                    Impo(i) = Impo(i) + im(j) * Ap(i, j)
                    CA(i) = CA(i) + m_epdata.fCatch(j) * Ap(i, j)
                    EXA(i) = EXA(i) + Ex(j) * Ap(i, j)
                    DTA(i) = DTA(i) + m_epdata.FlowToDet(j) * Ap(i, j)
                    RSP(i) = RSP(i) + Resp(j) * Ap(i, j)
                End If
            Next j
            SumTrp = SumTrp + TRP(i)
        Next i

        'Init
        SumTrpD = 0
        PredatD(1) = 0
        TRPD(1) = 0
        ImpD(1) = 0
        CAD(1) = 0
        EXAD(1) = 0
        'Sum for all detritus groups
        For i = m_epdata.NumLiving + 1 To m_epdata.NumGroups
            SumTrpD = SumTrpD + HNoC(i)
            PredatD(1) = PredatD(1) + PredNoC(i)
            TRPD(1) = TRPD(1) + HNoC(i)
            ImpD(1) = ImpD(1) + im(i)
            CAD(1) = CAD(1) + m_epdata.fCatch(i)
            EXAD(1) = EXAD(1) + Ex(i)
            RSPD(1) = RSPD(1) + Resp(i)
        Next i
        DTAD(1) = 0                         'Cont to detr from detr is zero
        'RSPD(1) = 0                         'RESP(m_epdata.NumGroups) is zero
        For i = 2 To NoTL
            For j = 1 To m_epdata.NumLiving
                If ADj(j) > 0 Then
                    PredatD(i) = PredatD(i) + PredNoC(j) * Ad(i, j)
                    TRPD(i) = TRPD(i) + HNoC(j) * Ad(i, j)

                    ' If GrpsToShow(j) Then TrpShow(i) = TrpShow(i) + HNoC(j) * Ad(i, j)
                    If m_HideGroups.IsGroupShown(m_manager.GroupName(j)) Then TrpShow(i) = TrpShow(i) + HNoC(j) * Ad(i, j) 'joeh
                    'TrpShow(i) = TrpShow(i) + HNoC(j) * Ad(i, j) 'joeh
                    ImpD(i) = ImpD(i) + im(j) * Ad(i, j)
                    CAD(i) = CAD(i) + m_epdata.fCatch(j) * Ad(i, j)
                    EXAD(i) = EXAD(i) + Ex(j) * Ad(i, j)
                    DTAD(i) = DTAD(i) + m_epdata.FlowToDet(j) * Ad(i, j)
                    RSPD(i) = RSPD(i) + Resp(j) * Ad(i, j)
                End If
            Next j
            SumTrpD = SumTrpD + TRPD(i)
        Next i

        If SumTrp + SumTrpD > 0 Then
            DetIndex = SumTrpD / (SumTrp + SumTrpD)
        End If
        'Proportion of total flow originating from detritus: DetIndex.

        DTDSum = 0 : DDT = 0
        For i = 1 To NoTL + 1
            DTDSum = DTDSum + DTA(i) + DTAD(i)
            DDT = DDT + DTAD(i)
        Next i

        For i = 1 To NoTL + 1
            lines = lines + 1
            If i <= m_epdata.NumLiving Then
                If TRP(i) < 0.0001 And TRP(i + 1) < 0.0001 Then i = NoTL + 1
            Else
                i = NoTL + 1
            End If
        Next i
        'MsgBox "FLOWS ORIGINATING FROM THE DETRITUS"
        For i = 1 To NoTL + 1
            If i <= m_epdata.NumGroups Then
                lines = lines + 1
                If i <= m_epdata.NumLiving Then
                    If TRPD(i) < 0 And TRPD(i + 1) < 0 Then i = NoTL + 1
                End If
            End If
        Next i
        'MsgBox "FLOWS ALL COMBINED"
        TotalTrp = 0
        SumIm = 0
        For i = 1 To NoTL + 1
            If i <= m_epdata.NumGroups Then
                If i = 1 Then
                    'MsgBox Cstr$(Impo(i) + ImpD(i))
                Else
                    SumIm = SumIm + Impo(i) + ImpD(i)
                End If
                TotalTrp = TotalTrp + TRP(i) + TRPD(i)
                If i <= m_epdata.NumLiving Then
                    If TRP(i) + TRPD(i) < 0.0001 And TRP(i + 1) + TRPD(i + 1) < 0.0001 Then i = NoTL + 1
                Else
                    i = NoTL + 1
                End If
            End If
        Next i

        TrpNoIM = Predat(1) + EXA(1) + DTA(1) + RSP(1)
        TrpNoIM = PredatD(1) + EXAD(1) + DTAD(1) + RSPD(1) + TrpNoIM
        If TrpNoIM > 0 Then
            Dead = 0
            For j = 1 To m_epdata.NumLiving
                If EE(j) < 1 Then Dead = Dead + B(j) * PB(j) * (1 - EE(j)) * AM(1, j)
            Next j
            If TrpNoIM <> 0 Then
                TrEm2(1) = (Dead + EXA(1) + EXAD(1) + Predat(1) + PredatD(1)) / TrpNoIM
                TrEm1(1) = (Predat(1) + PredatD(1)) / TrpNoIM
            End If
        End If

        For i = 2 To NoTL
            If i <= m_epdata.NumGroups Then
                Tr1 = Predat(i) + PredatD(i)
                If Tr1 > 0 Then
                    If TRP(i) + TRPD(i) > 0 Then
                        TrEm1(i) = Tr1 / (TRP(i) + TRPD(i))
                    End If
                End If
                TotTr = Predat(i) + PredatD(i) + CA(i) + CAD(i)
                If TRP(i) + TRPD(i) > 0 Then
                    TotTr = TotTr / (TRP(i) + TRPD(i))
                Else
                    TrEm1(i) = 0
                End If
            End If
        Next i

        For i = 2 To NoTL
            If i <= m_epdata.NumGroups And (TRP(i) + TRPD(i)) > 0 Then
                Dead = 0
                For j = 1 To m_epdata.NumGroups
                    If EE(i) < 1 And i <= m_epdata.NumLiving Then Dead = Dead + B(j) * PB(j) * (1 - EE(j)) * AM(i, j)
                Next j
                TotTr = Predat(i) + PredatD(i) + EXA(i) + EXAD(i) + Dead
                TrEm2(i) = TotTr / (TRP(i) + TRPD(i))
            End If
        Next i

        For i = 1 To NoTL + 1
            If i <= m_epdata.NumLiving Then
                TotalB = TotalB + BbyTL(i)
                If BbyTL(i) < 0.0001 Then i = NoTL + 1
            End If
        Next i


        '      GoTo EndOfLinde

        '===========
        'Subroutines
        '===========
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'jb This has been moved to   Private Sub SortTL()
        'SortTL:  'This routine sorts group after
        '        'trophic level
        '        For K = 1 To m_epdata.NumGroups
        '            TropLvl(K) = m_epdata.TTLX(K)
        '        Next K

        '        For kk = 1 To m_epdata.NumGroups
        '            MinTL = 100
        '            Grp = -2
        '            For K = 1 To m_epdata.NumGroups
        '                If TropLvl(K) <= MinTL And TropLvl(K) > 0 Then
        '                    MinTL = TropLvl(K)
        '                    Grp = K
        '                End If
        '            Next K
        '            TLsort(kk) = Grp             'Contains list with sorted grpnumbers
        '            TropLvl(Grp) = -1
        '        Next kk
        '        Return
        'xxxxxxxxxxxxxxxxxxxxxxxxx

        'EndOfLinde:
    End Sub


    Private Sub SortTL()
        'ToDo_jb cNetWork.SortTL this was a gosub in the EwE5 code make sure this does the same thing
        Dim K As Integer
        Dim Grp As Integer
        Dim MinTL As Integer

        'This routine sorts group after
        'trophic level
        For K = 1 To m_epdata.NumGroups
            TropLvl(K) = m_epdata.TTLX(K)
        Next K

        For kk As Integer = 1 To m_epdata.NumGroups
            MinTL = 100
            Grp = -2
            For K = 1 To m_epdata.NumGroups
                If TropLvl(K) <= MinTL And TropLvl(K) > 0 Then
                    MinTL = CInt(TropLvl(K))
                    Grp = K
                End If
            Next K
            TLsort(kk) = Grp             'Contains list with sorted grpnumbers
            TropLvl(Grp) = -1
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
        For i = 1 To m_epdata.NumGroups
            If TrEm2(i) > 0 Then chk = i
            'Chk becomes highest trophic level with a positive transfer efficiency
        Next i

        If chk > 0 Then
            ReDim Aold(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            'Aold is for pyramid and bank screen
            ReDim CumTrEm1(chk + 1)
            ReDim CumTrEm2(chk + 1)
            ReDim Hold(m_epdata.NumGroups + 3)
            ReDim SumWgt(m_epdata.NumGroups)
            ReDim TrpTLen(chk)
            ReDim TrpTLEm1(chk)
            ReDim TrpTLEm2(chk)
            'ReDim Qem( m_epdata.NumGroups + 3)
            ReDim Qold(m_epdata.NumGroups + 3)
            ReDim Wgt(m_epdata.NumGroups, m_epdata.NumGroups)
        Else
            Debug.Assert(chk = 0)  'First run of Ulanow
            ReDim AUL(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            ReDim fi(m_epdata.NumGroups, m_epdata.NumGroups)
            ReDim Fcyc(m_epdata.NumLiving, m_epdata.NumLiving)
            ReDim Acyc(m_epdata.NumGroups + 3, m_epdata.NumGroups + 3)
            ReDim HCyc(m_epdata.NumGroups + 3)
            ReDim Qcyc(m_epdata.NumGroups + 3)
            ReDim NoResp(m_epdata.NumGroups)
            ReDim NoRespC(m_epdata.NumGroups + 1)
            ReDim im(m_epdata.NumGroups + 2)
            ReDim Ac(m_epdata.NumGroups)
            ReDim Ec(m_epdata.NumGroups)
            ReDim CC(m_epdata.NumGroups)
            'ReDim Lap( m_epdata.NumGroups, m_epdata.NumGroups)
        End If

        K = m_epdata.NumGroups + 1 : L = m_epdata.NumGroups + 2 : MM = m_epdata.NumGroups + 3
        bit = 1.442695
        Ao = 0 : Eo = 0 : Co = 0
        Ai = 0 : Ei = 0 : Ci = 0
        Ae = 0 : Eee = 0 : Ce = 0
        Ar = 0 : er = 0 : Cr = 0
        SumEx = 0
        SumResp = 0

        If chk = 0 Then
            For i = 1 To MM
                Q(i) = 0
                TrPut(i) = 0
                Qcyc(i) = 0 : HCyc(i) = 0
                For j = 1 To m_epdata.NumGroups
                    AUL(i, j) = 0
                    Acyc(i, j) = 0
                    If i <= m_epdata.NumGroups Then
                        F(i, j) = 0
                        fi(i, j) = 0
                    End If
                    If i <= m_epdata.NumLiving And j <= m_epdata.NumLiving Then
                        Fcyc(i, j) = 0
                    End If
                Next j
            Next i
            For j = 1 To m_epdata.NumGroups
                NoResp(j) = 0 : im(j) = 0
                Ac(j) = 0 : Ec(j) = 0 : CC(j) = 0
            Next
            For i = 1 To m_epdata.NumGroups
                For j = 1 To m_epdata.NumGroups
                    If j <= m_epdata.NumLiving Then   '041022, accommodating m_epdata.pp with diets
                        '070104VC: the line below had m_epdata.pp(i) instead of m_epdata.pp(j) this caused flow to detritus to be calculated
                        'erroneously. Gave problems with MTI, Absolute flows, ascendency which are fixed now.
                        AUL(i, j) = CSng(IIf(m_epdata.PP(j) < 1, B(j) * QB(j) * DC(j, i), 0))
                    Else
                        AUL(i, j) = CSng(m_epdata.det(i, j))
                    End If
                    If i <= m_epdata.NumLiving And j <= m_epdata.NumLiving Then Acyc(i, j) = AUL(i, j)
                Next j
            Next i                         'Aij is amount eaten of
            'group i by predator j
            CalcImport()

            For i = 1 To MM
                If i <= m_epdata.NumGroups Then
                    AUL(K, i) = im(i)      'Amount imported
                    AUL(i, L) = Ex(i)
                    AUL(i, MM) = Resp(i)
                    SumEx = SumEx + Ex(i)
                    SumResp = SumResp + Resp(i)
                    If i <= m_epdata.NumLiving Then
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
            If TrEm1(1) <> 0 Then CumTrEm1(2) = 1 / TrEm1(1)
            If TrEm2(1) <> 0 Then CumTrEm2(2) = 1 / TrEm2(1)

            For i = 3 To chk
                TrpTLen(i) = 0
                TrpTLEm1(i) = 0
                TrpTLEm2(i) = 0
                If TrEm1(i - 1) > 0 Then CumTrEm1(i) = CumTrEm1(i - 1) / TrEm1(i - 1)
                If TrEm2(i - 1) > 0 Then CumTrEm2(i) = CumTrEm2(i - 1) / TrEm2(i - 1)
                'PRINT TAB(3); USING "##.###  ##########.### "; TrEm1(i); CumTrEm1(i)
            Next i

            ' This transfer efficiency is a cumulative" transfer efficiency
            ' i.e. it is a factor that can be used for raising a flow on a
            ' given trophic level to what it corresponds to of primary prod.

            For i = 1 To m_epdata.NumGroups
                RaiEm1(i) = 0                'For Emergy-1
                RaiEm2(i) = 0                'For Emergy-2
            Next i

            For i = 1 To m_epdata.NumGroups              'i is for groups included in AM
                ImpEn = ImpEn + im(i)
                For j = 1 To chk            'j is for trophic level'
                    RaiEm1(i) = RaiEm1(i) + AM(j, i) * CumTrEm1(j)
                    RaiEm2(i) = RaiEm2(i) + AM(j, i) * CumTrEm2(j)
                Next j
                'PRINT USING "######.## ######.## "; RaiEm1(i); RaiEm2(i)
            Next i


            For i = 1 To MM
                For j = 1 To MM
                    Aold(i, j) = AUL(i, j)
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
            For i = 1 To m_epdata.NumGroups
                Ac(i) = 0 : Ec(i) = 0 : CC(i) = 0
                For j = 1 To MM    'm_epdata.NumGroups
                    If DCnt = 1 Then
                        AUL(i, j) = Aold(i, j) * RaiEm1(i)
                    ElseIf DCnt = 2 Then
                        AUL(i, j) = Aold(i, j) * RaiEm2(i)
                    End If
                Next j
                If EE(i) < 1 And i <= m_epdata.NumLiving Then
                    Dead = B(i) * PB(i) * (1 - EE(i))
                Else
                    Dead = 0
                End If
                If DCnt = 1 Then
                    ImpEm1 = ImpEm1 + AUL(K, i)
                Else
                    ImpEm2 = ImpEm2 + AUL(K, i)
                End If
            Next i

        End If                          'Second run - emergy
        '****                           'emergy end

        'PrintAMatr

        TruPut = 0
        Ao = 0 : Eo = 0 : Co = 0
        Ai = 0 : Ei = 0 : Ci = 0
        Ae = 0 : Eee = 0 : Ce = 0
        Ar = 0 : er = 0 : Cr = 0

        TCyc = 0
        For i = 1 To MM
            Q(i) = 0
            TrPut(i) = 0
            For j = 1 To MM
                Q(i) = Q(i) + AUL(i, j)
                'Row sum - out of boxes
                TrPut(i) = TrPut(i) + AUL(j, i)
                'Col sum - into boxes
                'Notice that Q(i) will be larger
                'than TrPut(i) for a producer with QB=0
                Qcyc(i) = Qcyc(i) + Acyc(i, j)
                HCyc(i) = HCyc(i) + Acyc(j, i)
            Next j
            TruPut = TruPut + Q(i)             'Total flow=throughput
            TCyc = TCyc + Qcyc(i)
        Next i
        If DCnt = 1 Then Tmix = TruPut

        For i = 1 To m_epdata.NumGroups               'Imports
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If Q(K) > 0 And Q(i) > 0 Then
                X = AUL(K, i) * TruPut / Q(K) / Q(i)
                Yy = CSng(AUL(K, i) ^ 2 / Q(K) / Q(i))
            End If
            If X > 0 Then Au = CSng(AUL(K, i) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-AUL(K, i) * Math.Log(Yy))
            If TruPut > 0 Then X = (AUL(K, i) / TruPut)

            If X > 0 Then C1 = CSng(-AUL(K, i) * Math.Log(X))
            Ao = Ao + Au
            Eo = Eo + E1
            Co = Co + C1
            AddUp_i_j(i, Au, E1, C1)  'Sum the group specific ascendency etc
        Next i

        For i = 1 To m_epdata.NumGroups               'Trophic interactions
            For j = 1 To m_epdata.NumGroups
                X = 0 : Yy = 0
                Au = 0 : E1 = 0 : C1 = 0
                If AUL(i, j) > 0 Then
                    If chk = 0 Then
                        If Q(i) > 0 And TrPut(j) > 0 Then
                            X = CSng(AUL(i, j) * TruPut / Q(i) / TrPut(j))
                            Yy = CSng(AUL(i, j) ^ 2 / Q(i) / TrPut(j))
                        End If
                    Else
                        If Q(i) > 0 And Q(j) > 0 And TrPut(j) <> 0 Then
                            X = AUL(i, j) * TruPut / Q(i) / Q(j)
                            Yy = CSng(AUL(i, j) ^ 2 / Q(i) / Q(j))
                        End If
                    End If
                End If
                If X > 0 Then Au = CSng(AUL(i, j) * Math.Log(X))
                If Yy > 0 Then E1 = CSng(-AUL(i, j) * Math.Log(Yy))
                X = (AUL(i, j) / TruPut)
                If X > 0 Then C1 = CSng(-AUL(i, j) * Math.Log(X))
                Ai = Ai + Au
                Ei = Ei + E1
                Ci = Ci + C1
                AddUp_i_j(i, Au, E1, C1)
            Next j
        Next i

        For j = 1 To m_epdata.NumGroups               'Exports
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If chk = 0 Then            'Energy run
                If TrPut(j) > 0 And TrPut(L) > 0 Then
                    X = CSng(AUL(j, L) * TruPut / TrPut(L) / TrPut(j))
                    Yy = CSng(AUL(j, L) ^ 2 / TrPut(L) / TrPut(j))
                End If
            Else                        'Emergy run 1 or 2
                If Q(j) > 0 And TrPut(L) > 0 Then
                    X = CSng(AUL(j, L) * TruPut / TrPut(L) / Q(j))
                    Yy = CSng(AUL(j, L) ^ 2 / TrPut(L) / Q(j))
                End If
            End If
            If X > 0 Then Au = CSng(AUL(j, L) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-AUL(j, L) * Math.Log(Yy))
            X = (AUL(j, L) / TruPut)
            If X > 0 Then C1 = CSng(-AUL(j, L) * Math.Log(X))
            Ae = Ae + Au
            Eee = Eee + E1
            Ce = Ce + C1
            AddUp_i_j(j, Au, E1, C1)
        Next j

        For j = 1 To m_epdata.NumGroups               'Respiration
            X = 0 : Yy = 0
            Au = 0 : E1 = 0 : C1 = 0
            If chk = 0 Then
                If TrPut(MM) > 0 And TrPut(j) > 0 Then
                    X = CSng(AUL(j, MM) * TruPut / TrPut(MM) / TrPut(j))
                    Yy = CSng(AUL(j, MM) ^ 2 / TrPut(MM) / TrPut(j))
                End If
            Else
                If TrPut(MM) > 0 And Q(j) > 0 Then
                    X = CSng(AUL(j, MM) * TruPut / TrPut(MM) / Q(j))
                    Yy = CSng(AUL(j, MM) ^ 2 / TrPut(MM) / Q(j))
                End If
            End If
            If X > 0 Then Au = CSng(AUL(j, MM) * Math.Log(X))
            If Yy > 0 Then E1 = CSng(-AUL(j, MM) * Math.Log(Yy))
            X = (AUL(j, MM) / TruPut)
            If X > 0 Then
                C1 = CSng(-AUL(j, MM) * Math.Log(X))
            End If
            Ar = Ar + Au
            er = er + E1
            Cr = Cr + C1
            AddUp_i_j(j, Au, E1, C1)
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
        SumAc = 0
        SumEc = 0
        SumCc = 0
        For i = 1 To m_epdata.NumGroups
            Ac(i) = Ac(i) * bit
            Ec(i) = Ec(i) * bit
            CC(i) = CC(i) * bit
            SumAc = SumAc + Ac(i)
            SumEc = SumEc + Ec(i)
            SumCc = SumCc + CC(i)
        Next i

        If chk > 0 Then                'pyramid and bank
            For j = 1 To m_epdata.NumGroups
                SumWgt(j) = 0
            Next j
            For i = 1 To chk
                For j = 1 To m_epdata.NumGroups
                    If DCnt = 1 Then
                        If i > 1 Then
                            Wgt(i, j) = CumTrEm1(i) * AM(i, j)
                        Else
                            Wgt(i, j) = AM(i, j)
                        End If
                    ElseIf DCnt = 2 Then
                        If i > 1 Then
                            Wgt(i, j) = CumTrEm2(i) * AM(i, j)
                        Else
                            Wgt(i, j) = AM(i, j)
                        End If
                    End If
                    SumWgt(j) = SumWgt(j) + Wgt(i, j)
                Next j
            Next i

            For i = 1 To chk
                For j = 1 To m_epdata.NumGroups
                    If SumWgt(j) = 0 Then SumWgt(j) = 1
                    If DCnt = 1 Then
                        If SumWgt(j) > 0 Then TrpTLEm1(i) = TrpTLEm1(i) + Q(j) * Wgt(i, j) / SumWgt(j)
                        TrpTLen(i) = TrpTLen(i) + Qold(j) * AM(i, j)
                    ElseIf DCnt = 2 Then
                        If SumWgt(j) > 0 Then TrpTLEm2(i) = TrpTLEm2(i) + Q(j) * Wgt(i, j) / SumWgt(j)
                    End If
                Next j
            Next i
        End If

        Ao = Ao * bit : Eo = Eo * bit : Co = Co * bit
        Ai = Ai * bit : Ei = Ei * bit : Ci = Ci * bit
        Ae = Ae * bit : Eee = Eee * bit : Ce = Ce * bit
        Ar = Ar * bit : er = er * bit : Cr = Cr * bit

        For i = 1 To m_epdata.NumGroups                 'FOR HOST-MATRIX FOR IMPACT
            'For j = 1 To m_epdata.NumGroups             'EXCLUD. RESP
            For j = 1 To m_epdata.NumLiving             'EXCLUD. RESP and flow to detritus
                NoResp(i) = NoResp(i) + AUL(i, j)
            Next j
            NoRespC(i) = m_epdata.fCatch(i) + NoResp(i)  'Including catches
        Next i

        'NO FISHERY
        For i = 1 To m_epdata.NumGroups               'FOR HOST-MATRIX FOR IMPACT
            If NoResp(i) > 0 Then  'Host: a group i , recipient: living j
                For j = 1 To m_epdata.NumLiving    'Not detritus as recipient  'EXCL RESP
                    If j <= m_epdata.NumLiving Then F(i, j) = AUL(i, j) / NoResp(i)
                Next j
            End If
        Next i
        'WITH FISHERY
        For i = 1 To m_epdata.NumGroups               'FOR HOST-MATRIX FOR IMPACT
            If NoRespC(i) > 0 Then
                For j = 1 To m_epdata.NumLiving  'Groups + m_epdata.NumFleet         'EXCL RESP
                    FC(i, j) = AUL(i, j) / NoRespC(i) 'Includes fishery
                Next
                For j = 1 To m_epdata.NumFleet 'Host: a group i , recipient: fishery j
                    FC(i, j + m_epdata.NumGroups) = (m_epdata.Landing(j, i) + m_epdata.Discard(j, i)) / NoRespC(i)
                Next
            End If
        Next i

        'Host matrix for m_epdata.Discards going to detritus  (from fleets)
        If m_epdata.NumFleet > 0 Then
            For i = 1 To m_epdata.NumFleet
                For j = m_epdata.NumLiving + 1 To m_epdata.NumGroups     'Host: fleet i, recipient: Detritus j
                    TotalCatchForThisGear = m_epdata.Landing(i, 0) + m_epdata.Discard(i, 0)
                    If TotalCatchForThisGear > 0 Then FC(m_epdata.NumGroups + i, j) = CSng(m_epdata.det(m_epdata.NumGroups + i, m_epdata.NumLiving) / TotalCatchForThisGear)
                Next
            Next
        End If

        'A with detritus"
        For i = 1 To m_epdata.NumGroups               'FOR CYCLING INDEX
            If Q(i) > 0 Then
                For j = 1 To m_epdata.NumGroups
                    fi(i, j) = -AUL(i, j) / Q(i)
                    If i = j Then fi(i, j) = 1 + fi(i, j)
                Next j
            End If
        Next i

        'A without detritus"
        For i = 1 To m_epdata.NumLiving                'FOR CYCLING INDEX
            If Qcyc(i) > 0 Then
                For j = 1 To m_epdata.NumLiving
                    Fcyc(i, j) = -Acyc(i, j) / Qcyc(i)
                    If i = j Then Fcyc(i, j) = 1 + Fcyc(i, j)
                Next j
            End If
        Next i

        If chk = 0 Then                  'Only find cycling index in first run
            TcD = 0
            Tc = 0
            'MsgBox "Making Matrix Inversion #1"
            ErrCode = MatInvS(fi)        'FOR CYCLING INDEX
            If ErrCode <> 0 Then
                'Mess$ = "         There is an error, ( Code = " & ERRCode & " ) "
                'Mess$ = Mess&  "    Finn's cycling index cannot be calculated "
                'MsgBox Mess$
            Else
                For i = 1 To m_epdata.NumGroups
                    If fi(i, i) > 0 Then
                        AmCyc = Q(i) * (fi(i, i) - 1) / fi(i, i)   'Amount cycled
                        TcD = TcD + AmCyc      'For cycling index including detritus
                    End If
                Next i
            End If

            'MsgBox "Making Matrix Inversion #2"
            ErrC = MatInvS(Fcyc)         'FOR CYCLING INDEX excl detr
            If ErrC <> 0 Then
                'mess$ = "         There is an error, ( Code = " & ERRC & " ) "
                'mess$ = mess$ & "    Predatory cycling index cannot be calculated "
                'MsgBox mess$
            Else
                For i = 1 To m_epdata.NumLiving
                    If Fcyc(i, i) > 0 Then
                        AmCyc = Qcyc(i) * (Fcyc(i, i) - 1) / Fcyc(i, i)
                        'Amount cycled
                        Tc = Tc + AmCyc
                        'For cycling index excluding detritus
                    End If
                Next i
            End If
        End If
        If ErrCode <> 0 Then ' Or ERRC <> 0 Then
        End If
        Ascen = Ao + Ai + Ae + Ar
        If chk = 0 Then Ascend = Ascen 'It is Ascend that will be saved
        Overhead = Eo + Ei + Eee + er
        Capacity = Co + Ci + Ce + Cr
        If TruPut > 0 Then
            infoc = Ascen / TruPut
            Ptc = 100 / Capacity
            Aop = Ao * Ptc : Eop = Eo * Ptc : Cop = Co * Ptc
            Aip = Ai * Ptc : Eip = Ei * Ptc : Cip = Ci * Ptc
            Aep = Ae * Ptc : Eep = Eee * Ptc : Cep = Ce * Ptc
            Arp = Ar * Ptc : Erp = er * Ptc : Crp = Cr * Ptc
            Ascp = Ascen * Ptc
            Overp = Overhead * Ptc
            Capp = Capacity * Ptc
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
                If ToldYou = False Then MsgBox("Predatory cycling index cannot be calculated. ")
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
    Private Function MatMultS(ByVal Alpha(,) As Single, ByVal Beta(,) As Single, ByVal Gamma(,) As Single) As Integer
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
                    Gamma(row, col) = Gamma(row, col) + Alpha(row, inside) * Beta(inside, col)
                Next inside
            Next col
        Next row
smultexit:
        Exit Function
smulterr:
        MatMultS = (Err.Number + 5) Mod 200 - 5
        Resume smultexit
    End Function


    Private Function MatInvS(ByVal A(,) As Single) As Integer
        Dim bserrcode As Integer, col As Integer, ErrCode As Integer, row As Integer
        Dim Ain(,) As Single, Ein() As Single, X() As Single
        On Error GoTo sinverr
        ErrCode = 0

        'MatrixCalc.matluS uses up and lo 
        MatrixCalc.Lo = 1 'LBound(A, 1)
        MatrixCalc.Up = UBound(A, 1)
        Dim Lo As Integer = MatrixCalc.Lo
        Dim Up As Integer = MatrixCalc.Up
        ReDim MatrixCalc.rpvt(Up)
        ReDim MatrixCalc.cpvt(Up)


        ReDim Ain(Up, Up)
        ReDim Ein(Up)
        ReDim X(Up)

        ErrCode = MatrixCalc.matluS(A, True)                     'Get LU matrix
        'nong 'stop If Not continue Then Error ErrCode
        For col = Lo To Up                         'find A^-1 one column at a time
            Ein(col) = 1.0!
            bserrcode = MatrixCalc.matbsS(A, Ein, X)
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
        Erase E, X, Ain, rpvt, cpvt
        MatInvS = ErrCode
        Exit Function
sinverr:
        ErrCode = (Err.Number + 5) Mod 200 - 5
        Resume sinvexit
    End Function


    Private Sub CalcImport()
        Dim i As Integer
        For i = 1 To m_epdata.NumGroups
            If i > m_epdata.NumLiving Then
                im(i) = m_epdata.DtImp(i)
            Else
                im(i) = m_epdata.DC(i, 0) * m_epdata.QB(i) * m_epdata.B(i)
            End If
        Next i
    End Sub


    Private Sub AddUp_i_j(ByVal i As Integer, ByVal Au As Single, ByVal E1 As Single, ByVal C1 As Single)

        Ac(i) = Ac(i) + Au
        Ec(i) = Ec(i) + E1
        CC(i) = CC(i) + C1
    End Sub



    Private Sub Impacts()

        Try

            ReDim DCC(m_epdata.NumFleet, m_epdata.NumGroups)
            Dim i As Integer, j As Integer, ErrCode As Integer
            Dim CatchByGear As Single

            For i = 1 To m_epdata.NumFleet
                CatchByGear = 0       'catch by gear
                For j = 1 To m_epdata.NumGroups
                    CatchByGear = CatchByGear + m_epdata.Landing(i, j) + m_epdata.Discard(i, j)
                Next j
                For j = 1 To m_epdata.NumGroups
                    If CatchByGear > 0 Then   'DCC is "Diet Composition of Catch" by gear
                        DCC(i, j) = (m_epdata.Landing(i, j) + m_epdata.Discard(i, j)) / CatchByGear
                    End If
                Next j
            Next

            For i = 1 To m_epdata.NumGroups            'Detritus is not considered a predator
                For j = m_epdata.NumLiving + 1 To m_epdata.NumGroups
                    F(i, j) = 0
                    FC(i, j) = 0
                Next
            Next i


            For i = 1 To m_epdata.NumGroups      'Impacting
                For j = 1 To m_epdata.NumGroups  'Impacted=host
                    If m_epdata.NumFleet >= 1 Then   'Then fishery is included as group(s)
                        If j > m_epdata.NumLiving Then  'Detritus
                            MTI(i, j) = -(0 - FC(j, i))
                        Else
                            MTI(i, j) = -(m_epdata.DC(j, i) - FC(j, i))       'Host matrix incl C
                        End If
                    Else                'no fishery
                        If j > m_epdata.NumLiving Then  'Detritus
                            MTI(i, j) = -(0 - F(j, i)) 'Detritus is not considered a predator
                        Else
                            MTI(i, j) = -(m_epdata.DC(j, i) - F(j, i))
                            '= proportion i contributes to the diet of j less the proportion i takes from j
                        End If
                    End If              'FEEDING MATRIX - HOST MATRIX
                Next j
                MTI(i, i) = 1 + MTI(i, i)   '[Ulanowicz' M -- here called MTI, is the [I]-[Q] MATRIX
            Next i
            If m_epdata.NumFleet > 0 Then  'Then fishery is included as group(s)
                'First do impact of groups on fishery
                For i = 1 To m_epdata.NumGroups ' + m_epdata.NumFleet
                    For j = 1 To m_epdata.NumFleet
                        MTI(i, m_epdata.NumGroups + j) = -(DCC(j, i) - 0) '-(m_epdata.dcC(j, i) - FC(m_epdata.NumGroups + j, i))
                        '= How much fishery takes of group i - how much the groups takes from the fishery (0)
                    Next
                Next i
                'Next impact of fishery on groups       'FC(host, predator)
                For i = 1 To m_epdata.NumFleet ' + m_epdata.NumFleet
                    For j = 1 To m_epdata.NumGroups
                        MTI(m_epdata.NumGroups + i, j) = -(0 - FC(j, m_epdata.NumGroups + i))
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
                For j = 1 To m_epdata.NumFleet
                    MTI(m_epdata.NumGroups + j, m_epdata.NumGroups + j) = 1 + MTI(m_epdata.NumGroups + j, m_epdata.NumGroups + j)
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

            ErrCode = MatInvS(MTI)
            If ErrCode% <> 0 Then
                MsgBox("There is an error, (Code = " & ErrCode% & ")")
                If ErrCode% = -1 Then
                    MsgBox("MTI Matrix not invertible")
                End If
                For i = 1 To m_epdata.NumGroups + m_epdata.NumFleet
                    For j = 1 To m_epdata.NumGroups + m_epdata.NumFleet
                        MTI(i, j) = 0
                    Next
                Next
                GoTo EndOfImp
            End If

RETURNED:

            For i = 1 To m_epdata.NumGroups + m_epdata.NumFleet
                If i > m_epdata.NumLiving And i <= m_epdata.NumGroups Then  'for detritus
                    MTI(i, i) = 0
                Else
                    MTI(i, i) = MTI(i, i) - 1   'for living and fishery
                End If
            Next i

            'The Mixed Tropic Impact Matrix has now been calculated"
            CheckForBenificialPredation()

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

            For i = 1 To m_epdata.NumGroups + m_epdata.NumFleet
                For j = 1 To m_epdata.NumGroups + m_epdata.NumFleet
                    BenificialPredation(i, j) = False
                Next
            Next
            'First check for groups
            For i = 1 To m_epdata.NumLiving
                For j = 1 To m_epdata.NumGroups
                    If m_epdata.DC(i, j) > 0 And MTI(i, j) > 0 Then
                        BenificialPredation(i, j) = True
                    End If
                Next
            Next
            'Then check for impact of fishery
            For i = 1 To m_epdata.NumFleet
                For j = 1 To m_epdata.NumGroups
                    If DCC(i, j) > 0 And MTI(i + m_epdata.NumGroups, j) > 0 Then
                        BenificialPredation(i + m_epdata.NumGroups, j) = True
                    End If
                Next
            Next
        Catch ex As Exception
            Debug.Assert(False)
            Throw New ApplicationException("CheckForBenificialPredation()", ex)
        End Try

    End Sub


    'Sub FindCycles2(ByVal Cons() As Single)
    '    'CycDC [previously called CD] contains the proportion
    '    'of the diet that is the minimum
    '    'amount in a cycle and should be removed to break the cycle.
    '    'This amount is subtracted from all flows in the cycle.
    '    Dim APred, Comp, pred, prey
    '    Dim Answer As Object
    '    Dim Cnt As Long

    '    '        AbortRun = False
    '    '        'DoWhat = "PPR"
    '    '        'frmWait.Caption = "Identification of pathways in progress, (cancel if pathways not needed)"
    '    '        'frmWait.Frame1.Visible = False
    '    '        'frmWait.PBar.max = 1000 'm_epdata.NumLiving
    '    '        'frmWait.ZOrder()
    '    '        'frmWait.Show() '0
    '    '        'frmWait.Refresh()

    '    '        Cnt = 0
    '    '        Array.Clear(Cons, 0, Cons.Length)

    '    '        'If m_epdata.NumLiving > 20 And DoneAlready = False Then
    '    '        '   If DoneAlready = False Then DoneAlready = True
    '    '        '    Answer = MsgBox("Your model has many groups, this procedure may take a long time to complete. Do you want to continue?", vbInformation + vbYesNo, "Find pathways: tedious for large models")
    '    '        '    If Answer = vbNo Then AbortRun = True: Exit Sub
    '    '        'End If

    '    '        For pivot = 1 To m_epdata.NumLiving
    '    '            If m_epdata.QB(pivot) = 0 Then GoTo NextPivot

    '    '            Array.Clear(Path, 0, Path.Length)
    '    '            ' Init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path())


    '    '            Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, pivot, LastComp)

    '    '            Path(pivot - 1) = pivot           ' Path's limits are Pivot-1 to Level
    '    '            '*** FOR Level = Pivot TO m_epdata.NumLiving
    '    '            For Level = pivot To 2 * m_epdata.NumLiving
    '    '                If Path(Level - 1) > 0 Then
    '    '                    pred = Path(Level - 1)
    '    '                Else
    '    '                    pred = pivot
    '    '                End If
    '    '                For Comp = LastComp(Level) To m_epdata.NumLiving
    '    '                    If m_epdata.DC(pred, Comp) > 0 Then
    '    '                        prey = Comp
    '    '                        Path(Level) = 0
    '    '                        CheckPath(pivot, prey)
    '    '                        If prey = 0 And Comp <> pivot Then GoTo NextComp 'In Path already
    '    '                        If pivot = Comp Then
    '    '                            Path(Level) = Comp
    '    '                            CyclePrint(CycDC, Cons)
    '    '                            Cnt = Cnt + 1
    '    '                            'frmWait.Label1(0).Caption = "No of cycles: " + CStr(Cnt)
    '    '                            'UpdateWait()
    '    '                            If AbortRun Then Exit Sub 'Have identified a cycle
    '    '                            FindMinConsump(Cons(), MinCons)
    '    '                            MinCycDC(CycDC(), MinCons)
    '    '                            Path(Level) = 0
    '    '                        Else
    '    '                            Path(Level) = Comp                     'Include group in Path
    '    '                            Path(Level + 1) = 0
    '    '                            LastComp(Level) = Comp
    '    '                            LastComp(Level + 1) = pivot
    '    '                            APred = 1
    '    '                            Exit For              'exit Comp for loop when path found
    '    '                            'and continue to next Level
    '    '                        End If
    '    '                    End If
    '    '                    APred = 0          'if program doesn't use EXIT FOR it will reset APred
    '    'NextComp:

    '    '                    ' DoEvents()
    '    '                Next Comp
    '    '                If AbortRun Then Exit Sub 'Have identified a cycle
    '    '                If APred = 0 Then                   'Start backtracking
    '    '                    'For Answer = 1 To Level: Debug.Print Format(Path(Answer), "  ##");: Next
    '    '                    'Debug.Print
    '    '                    If Level > pivot Then LastComp(Level - 1) = Path(Level - 1) + 1
    '    '                    Path(Level) = 0
    '    '                    Level = Level - 2
    '    '                    If Level = pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
    '    '                End If
    '    '                '         frmWait.Label1(0).Caption = "Cycles: " + CStr(Cnt) + ", Pivot: " + CStr(pivot) + ", Level: " + CStr(Level)
    '    '            Next Level
    '    'NextPivot:
    '    '        Next pivot
    '    '        'Unload(frmWait)
    '    '        'frmWait = Nothing
    'End Sub



    '    Sub FindCycles1(ByVal DC() As Single, ByVal Topic, ByVal Sel%, ByVal Sel2%, ByVal NoPath, ByVal NoArrows, ByVal fnum As Integer)
    '        Dim Mess As String
    '        Dim arrow As Integer
    '        Dim Comp As Integer
    '        Dim kStart As Integer
    '        Dim K As Integer
    '        Dim Pass As Long
    '        Dim prey As Integer
    '        Dim PreyOK As Integer
    '        Dim ProdOK As Integer
    '        Dim PrintC As Integer
    '        Dim NewPath As Integer
    '        Dim NumP As Long
    '        NumP = 0
    '        If Topic = 3 Then
    '            Pivot% = Sel2%
    '            Init1DimInteg(0, 2 * NumGroups + 2, Path())
    '            Assign1DimInteg(1, 2 * NumGroups + 1, 1, LastComp%())
    '            Path(0) = Pivot%                        ' Path's limits are Pivot%-1 to Level%
    '            For Level% = 1 To m_epdata.NumLiving                    ' In this case Pred is the prey and
    '                If Path(Level% - 1) > 0 Then        ' Prey the predator
    '                    pred = Path(Level% - 1)
    '                Else
    '                    pred = Pivot%
    '                End If
    '                For Comp% = LastComp(Level%) To m_epdata.NumLiving
    '                    If AbortRun Then Exit Sub
    '                    If DC(Comp%, pred) > 0 Then
    '                        prey% = Comp%
    '                        Path(Level%) = 0
    '                        CheckPath2(prey%)
    '                        If prey% = 0 Then
    '                            If Comp% = Pivot% Then
    '                            Else
    '                                GoTo NextComp3
    '                            End If
    '                        End If
    '                        If Pivot% = Comp% Then
    '                        Else
    '                            Path(Level%) = Comp%                      'Include group in Path
    '                            Path(Level% + 1) = 0
    '                            LastComp(Level%) = Comp%
    '                            LastComp(Level% + 1) = 1  'Pivot%
    '                            NewPath% = 1
    '                            APred = 1
    '                            Exit For                'exit Comp% for loop when path found
    '                        End If
    '                    End If
    '                    APred = 0          'if program doesn't use EXIT FOR it will reset APred
    'NextComp3:
    '                Next Comp%
    '                If APred = 0 Then                   'Start backtracking
    '                    If NewPath% = 1 Then
    '                    GoSub PathPrint
    '                        NewPath% = 0
    '                    End If
    '                    If Level% > 1 Then LastComp(Level% - 1) = Path(Level% - 1) + 1
    '                    Path(Level%) = 0
    '                    Level% = Level% - 2
    '                    If Level% = -1 Then Exit For 'Exit the Level for next and try new pivot
    '                End If
    'NextLevel3:
    '            Next Level%
    '        Else                                             '... for topics 1,2,4,5, 14
    '            For Pivot% = 1 To m_epdata.NumLiving                             '... prey
    '                If QB(Pivot%) = 0 Then
    '                    GoTo NextPivot       '... excludes producers
    '                Else
    '                    If Topic = 1 Or Topic = 2 Then
    '                        If Sel% = Pivot% Then
    '                        Else
    '                            GoTo NextPivot
    '                        End If
    '                    End If
    '                    Init1DimInteg(0, 2 * NumGroups + 2, Path())
    '                    If Topic = 4 Or Topic = 14 Then
    '                        Assign1DimInteg(1, 2 * NumGroups + 1, Pivot%, LastComp%())
    '                    Else
    '                        Assign1DimInteg(1, 2 * NumGroups + 1, 1, LastComp%())
    '                    End If
    '                    Path(Pivot% - 1) = Pivot%                 ' Path's limits are Pivot%-1 to Level%
    '                    Pass = 0

    '                    '*** changed counter below to go to 2 * N in  March 94, vc
    '                    '*** This was to avoid problem with too few levels

    '                    For Level% = Pivot% To 2 * m_epdata.NumLiving
    '                        Pass = Pass + 1
    '                        If Pass > 10 ^ 7 Then GoTo NextPivot
    '                        If Path(Level% - 1) > 0 Then
    '                            pred = Path(Level% - 1)
    '                        Else
    '                            pred = Pivot%
    '                        End If
    '                        For Comp% = LastComp(Level%) To NumGroups
    '                            If DC(pred, Comp%) > 0 Then
    '                                If (Topic <> 14 And pred <= m_epdata.NumLiving) Or Topic = 14 Then
    '                                    'For topic = 14 also cycles through detritus are OK
    '                                    prey% = Comp%
    '                                    Path(Level%) = 0
    '                                    CheckPath2(prey%) '...1
    '                                    If prey% = 0 Then
    '                                        If Comp% = Pivot% Then
    '                                        Else
    '                                            GoTo NextComp
    '                                        End If
    '                                    End If
    '                                    If Pivot% = Comp% Then
    '                                        Path(Level%) = Comp%
    '                                    If Topic = 4 Or Topic = 14 Then GoSub CyclePrint1   ' Have identified a cycle
    '                                        Path(Level%) = 0
    '                                    Else
    '                                        Path(Level%) = Comp%                  ' Include group in Path
    '                                        Path(Level% + 1) = 0
    '                                        LastComp(Level%) = Comp%
    '                                        If Topic = 5 Then
    '                                            LastComp(Level% + 1) = 1        'Pivot%
    '                                        Else
    '                                            LastComp(Level% + 1) = 1        'Pivot%
    '                                        End If
    '                                        NewPath% = 1
    '                                        APred = 1
    '                                        Exit For                              ' exit Comp% for loop when path found
    '                                        ' and continue to next Level%
    '                                    End If
    '                                End If
    '                            End If
    '                            APred = 0  'if program doesn't use EXIT FOR it will reset APred
    'NextComp:
    '                        Next Comp%
    'Backtrack:
    '                        If APred = 0 Then                   'Start backtracking
    '                            If NewPath% = 1 Then
    '                            If Topic = 1 Or Topic = 2 Then GoSub PreyProd
    '                            If Topic = 5 Then GoSub CountPath
    '                                NewPath% = 0
    '                            End If
    '                            If Level% > Pivot% Then LastComp(Level% - 1) = Path(Level% - 1) + 1
    '                            Path(Level%) = 0
    '                            Level% = Level% - 2
    '                            If Level% = Pivot% - 2 Then Exit For 'Exit the Level for next and try new pivot
    '                        End If
    'NextLevel:
    '                    Next Level%
    'NextPivot:
    '                End If
    '            Next Pivot%

    '        End If
    '        GoTo EndOfFINDCYCLES1
    '        '-----------------------------------------
    '        '...***  sub-routines *** ....



    'CountPath:
    '        arrow% = 1
    '        For K% = Pivot% - 1 To Level%
    '            If Path(K%) > 0 Then
    '                If arrow% = 0 And Topic > 0 Then NoArrows = NoArrows + 1
    '                arrow% = 0
    '            End If
    '        Next K%
    '        If arrow% = 0 Then NoPath = NoPath + 1
    '        Return

    'CyclePrint1:
    '        Mess$ = ""
    '        arrow = 0
    '        'First check if in path already
    '        '(then the cycle doesn't start with the lowest group number in the path
    '        For K% = Pivot% To Level% - 1
    '            If Path(Pivot% - 1) > Path(K%) Then arrow = -1
    '            'If so the grpnumber in Path(k) is < than the starting point for the cycle
    '            'and it should not be included in the path. It is there already
    '        Next
    '        If arrow = 0 Then
    '            arrow% = 1
    '            For K% = Pivot% - 1 To Level%
    '                If Path(K%) > 0 Then
    '                    If arrow% = 1 Then aa% = Path(K%)
    '                    If arrow% = 0 And Topic > 0 Then
    '                        If aa% > 0 Then Mess$ = Mess$ & LTrim(CStr(aa%)) : aa% = 0
    '                        Mess$ = Mess$ & Chr(171) & "-"
    '                        Mess$ = Mess$ & LTrim(CStr(Path(K%)))
    '                        NoArrows = NoArrows + 1
    '                    End If
    '                    arrow% = 0
    '                End If
    '            Next
    '            If (Topic = 4 Or Topic = 14) And NoPath <= 50000 Then
    '                frmCycles.vaEcocycle.maxRows = NoPath
    '                frmCycles.vaEcocycle.row = NoPath
    '                frmCycles.vaEcocycle.Text = Mess
    '                'Print #fnum, Mess
    '            End If
    '            If arrow% = 0 Then NoPath = NoPath + 1
    '        End If
    '        Return


    'PathPrint:
    '        Mess$ = ""
    '        arrow% = 1
    '        PrintC% = 0

    '        kStart = IIf(Topic = 3, 0, Pivot% - 1)
    '        For K% = kStart% To Level%              'PreyOK%
    '            If Path(K%) > 0 Then
    '                If arrow% = 1 Then aa% = Path(K%)
    '                If arrow% = 0 And Topic > 0 Then
    '                    If aa% > 0 Then
    '                        Mess$ = Mess$ & LTrim(CStr(aa%))
    '                        aa% = 0
    '                    End If
    '                    If Topic = 3 Then
    '                        Mess$ = Mess$ & "-" & Chr(187)
    '                    Else
    '                        Mess$ = Mess$ & Chr(171) & "-"
    '                    End If
    '                    Mess$ = Mess$ & LTrim(CStr(Path(K%)))

    '                    PrintC% = 1
    '                    NoArrows = NoArrows + 1
    '                End If
    '                arrow% = 0
    '            End If
    '        Next K%

    '        If arrow% = 0 Then NoPath = NoPath + 1
    '        If ((Topic >= 1 And Topic <= 4) Or Topic = 14) And NoPath <= 50000 Then
    '            'Print #fnum, Mess
    '            frmCycles.vaEcocycle.maxRows = NoPath
    '            frmCycles.vaEcocycle.row = NoPath
    '            frmCycles.vaEcocycle.Text = Mess
    '        End If
    '        If NoPath = 0 Then MsgBox("No cycle(s) in this system.", 48)
    '        If NumGroups > 10 Then
    '            NumP = NumP + 1
    '            If NumP Mod 1000 = 0 Then
    '                frmWait.Label1(0).Caption = "No of pathways: " + CStr(NumP)
    '                UpdateWait()
    '            End If
    '            If AbortRun Then Exit Sub
    '        End If

    '        Return

    'PreyProd:
    '        'IF PP(Pivot%) < 2 THEN GOTO Vers1.0
    '        PreyOK% = -1
    '        ProdOK% = -1
    '        For K% = Pivot% - 1 To Level%
    '            If Sel2% = Path(K%) Then PreyOK% = K%
    '            If PP(Path(K%)) > 0 Then ProdOK% = K%
    '        Next K%
    '        If ProdOK% = -1 Then Return
    '        If Topic = 2 And PreyOK% = -1 Then Return
    '  GoSub PathPrint
    '        Return

    'EndOfFINDCYCLES1:

    '    End Sub




    Private Sub Assign1DimInteg(ByVal Start As Integer, ByVal slut As Integer, ByVal value As Integer, ByVal vectorGetValue() As Integer)
        For i As Integer = Start To slut
            vectorGetValue(i) = value
        Next
    End Sub


    Private Sub CyclePrint(ByVal CycDC(,) As Single, ByVal Cons() As Single) 'Have identified a cycle
        Dim arrow As Integer, bib As Integer
        Dim K As Integer
        arrow = 1
        aa = 0
        For K = pivot - 1 To Level
            If Path(K) > 0 Then
                bib = aa
                aa = Path(K)
                If arrow = 0 Then
                    'mess$ = mess$ & "<---"
                    If CycDC(bib, aa) < 0 Then
                        Cons(bib) = 0
                    Else
                        Cons(bib) = -m_epdata.B(bib) * m_epdata.QB(bib) * m_epdata.DC(bib, aa)
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
            For Mini = pivot - 1 To m_epdata.NumLiving
                If Cons(Path(Mini)) <= 0 And m_epdata.QB(Path(Mini)) > 0 Then
                    If MinCons = 1 Then MinCons = Cons(Path(Mini))
                    If Cons(Path(Mini)) > MinCons Then MinCons = Cons(Path(Mini))
                End If
            Next Mini
            If MinCons = 1 Then MinCons = 0
            For Mini = 1 To m_epdata.NumLiving : Cons(Mini) = 0 : Next Mini
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("FindComsump() " & ex.Message, ex)
        End Try

    End Sub


    Private Sub ImportAmount()
        'This subroutine gives the total import (as an amount) to each group
        Try
            Dim i As Integer
            For i = 1 To m_epdata.NumLiving
                im(i) = m_epdata.DC(i, 0) * m_epdata.QB(i) * m_epdata.B(i)
            Next i
            For i = m_epdata.NumLiving + 1 To m_epdata.NumGroups
                im(i) = m_epdata.DtImp(i)
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("ImportAmount() " & ex.Message, ex)
        End Try

    End Sub



    Private Sub FindCycles(ByRef Cons() As Single)
        'CycDC [previously called CD] contains the proportion
        'of the diet that is the minimum
        'amount in a cycle and should be removed to break the cycle.
        'This amount is subtracted from all flows in the cycle.
        Dim APred As Integer, Comp As Integer, pred As Integer, prey As Integer
        Dim Answer As Object = Nothing
        Dim Cnt As Integer

        Dim iProg As Integer

        bStopNetworkAnnalysis = False

        'DoWhat = "PPR"
        'frmWait.Caption = "Identification of pathways in progress, (cancel if pathways not needed)"
        'frmWait.Frame1.Visible = False
        'frmWait.PBar.max = 1000 'm_epdata.NumLiving
        'frmWait.ZOrder()
        'frmWait.Show() '0
        'frmWait.Refresh()
        Cnt = 0
        ' Init1Dim(1, m_epdata.NumLiving, Cons())
        Array.Clear(Cons, 0, m_epdata.NumLiving)
        'Initialize

        'If m_epdata.NumLiving > 20 And DoneAlready = False Then
        ' If DoneAlready = False Then DoneAlready = True
        '    Answer = MsgBox("Your model has many groups, this procedure may take a long time to complete. Do you want to continue?", vbInformation + vbYesNo, "Find pathways: tedious for large models")
        '    If Answer = vbNo Then AbortRun = True: Exit Sub
        'End If

        For pivot = 1 To m_epdata.NumLiving
            If m_epdata.QB(pivot) <= 0 Then
                GoTo NextPivot
            End If

            Array.Clear(Path, 0, 2 * m_epdata.NumGroups + 2)
            'Init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path)
            Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, pivot, LastComp)

            Path(pivot - 1) = pivot           ' Path's limits are Pivot-1 to Level
            '*** FOR Level = Pivot TO m_epdata.NumLiving
            For Level = pivot To 2 * m_epdata.NumLiving
                If Path(Level - 1) > 0 Then
                    pred = Path(Level - 1)
                Else
                    pred = pivot
                End If
                For Comp = LastComp(Level) To m_epdata.NumLiving
                    If m_epdata.DC(pred, Comp) > 0 Then
                        prey = Comp
                        Path(Level) = 0
                        CheckPath(pivot, prey)
                        If prey = 0 And Comp <> pivot Then
                            GoTo NextComp 'In Path already
                        End If

                        If pivot = Comp Then
                            Path(Level) = Comp
                            CyclePrint(CycDC, Cons)
                            Cnt = Cnt + 1

                            Me.m_manager.updateFoundCycle(Cnt)
                            'prgmsg.Progress = prgmsg.Progress + 1
                            'm_publisher.SendMessage(prgmsg)

                            '     frmWait.Label1(0).Caption = "No of cycles: " + CStr(Cnt)
                            '    UpdateWait()
                            If bStopNetworkAnnalysis Then Exit Sub 'Have identified a cycle
                            FindMinConsump(Cons, MinCons)
                            MinCycDC(CycDC, MinCons)
                            Path(Level) = 0
                        Else
                            Path(Level) = Comp                     'Include group in Path
                            Path(Level + 1) = 0
                            LastComp(Level) = Comp
                            LastComp(Level + 1) = pivot
                            APred = 1
                            Exit For              'exit Comp for loop when path found
                            'and continue to next Level
                        End If
                    End If
                    APred = 0          'if program doesn't use EXIT FOR it will reset APred
NextComp:

                    ' DoEvents()
                Next Comp
                If bStopNetworkAnnalysis Then Exit Sub 'Have identified a cycle
                If APred = 0 Then                   'Start backtracking
                    'For Answer = 1 To Level: Debug.Print Format(Path(Answer), "  ##");: Next
                    'Debug.Print
                    If Level > pivot Then LastComp(Level - 1) = Path(Level - 1) + 1
                    Path(Level) = 0
                    Level = Level - 2
                    If Level = pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                End If

                iProg += 1
                Me.m_manager.updateProgressFindCycle(iProg)

                '   frmWait.Label1(0).Caption = "Cycles: " + CStr(Cnt) + ", Pivot: " + CStr(pivot) + ", Level: " + CStr(Level)
            Next Level
NextPivot:
        Next pivot
        '   Unload(frmWait)
        '   frmWait = Nothing
        'all done
        'prgmsg.ProgressState = eProgressState.Finished
        'm_publisher.SendMessage(prgmsg)

    End Sub


    Private Sub CheckPath(ByVal Pivot As Integer, ByRef prey As Integer)
        Dim K As Integer
        Try
            For K = Pivot - 1 To Level + 1
                If prey = Path(K) Then
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
            For K = pivot - 1 To Level
                If Path(K) > 0 Then
                    bib = aa
                    aa = Path(K)
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
            For i = 1 To m_epdata.NumGroups
                SumDC(i) = 0
                For j = 0 To m_epdata.NumGroups '+ 1                             'Import is in 0  (NOT N1+1
                    SumDC(i) = SumDC(i) + m_epdata.DC(i, j)            ' 0 <= SUMDC <= 1
                Next j
                '050113VC: remarked out the line below as import was already added to diets just above
                'If PP(i) > 0 And PP(i) < 1 Then SumDC(i) = SumDC(i) + DC(i, 0)
                If m_epdata.PP(i) = 1 Then SumDC(i) = 0
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
            ThruPut = 0
            For i = 1 To m_epdata.NumGroups
                PredatOn(i) = 0
                For j = 1 To m_epdata.NumGroups + 1               'Import is in N1+1
                    If j <= m_epdata.NumLiving Then PredatOn(i) = PredatOn(i) + m_epdata.B(j) * m_epdata.QB(j) * m_epdata.DC(j, i)
                Next j

                TrPut(i) = PredatOn(i) + m_epdata.Ex(i) + m_epdata.FlowToDet(i) + m_epdata.Resp(i)
                'vcm_epdata. resp for pp If PP(i) > 0 Then TrPut(i) = TrPut(i) + Im(i)  'For primary prod
                If m_epdata.PP(i) > 0 Then TrPut(i) = TrPut(i) + im(i) + m_epdata.Resp(i) 'For primary prod
                'vc resp for detr  If i > m_epdata.NumLiving Then TrPut(i) = TrPut(i) + Im(i)     'For detritus
                If i > m_epdata.NumLiving Then TrPut(i) = TrPut(i) + im(i) + m_epdata.Resp(i) 'For detritus
                ThruPut = ThruPut + TrPut(i)
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
            For i = 1 To m_epdata.NumLiving
                'vc280ct99
                'Import is in 0 not in N1+1
                'For j = 1 To NumGroups + 1
                For j = 0 To m_epdata.NumGroups '+ 1
                    AmCyc = AmCyc - CycDC(i, j)       'CycDC is negative
                    'CycDC is negative
                    If TrPut(i) > 0 Then
                        CycDC(i, j) = -CycDC(i, j) / TrPut(i)
                        If m_epdata.DC(i, j) < CycDC(i, j) Then CycDC(i, j) = m_epdata.DC(i, j)
                        '***VC*** made this check 07dec95 to avoid negative DC's
                        '041022VC to accomodate PP with diets:
                        If m_epdata.PP(i) = 1 Then CycDC(i, j) = 0
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
            For i = 1 To m_epdata.NumLiving
                'vc 28oct99: Import is in 0 not in N1+1
                'For j = 1 To NumGroups + 1
                For j = 1 To m_epdata.NumGroups
                    If m_epdata.QB(i) > 0 Then
                        If SumDC(i) > 1.00001 * SumCycDC(i) Then
                            DCNoCyc(i, j) = (m_epdata.DC(i, j) - CycDC(i, j)) / (SumDC(i) - SumCycDC(i)) * (1 - m_epdata.PP(i))
                            '050114VC, multiplied with (1-pp(i)) above to consider groups that
                            'are partly producers. Didn't work for such groups before this is done
                            'This actually makes the line below superfluous:
                            '041022VC below to accomodate PP with diets:
                            If m_epdata.PP(i) = 1 Then DCNoCyc(i, j) = 0
                        End If
                    End If
                Next j
                'vc280ct99 looks like dcnocyc needs to have import in the last element:
                If SumDC(i) > 1.00001 * SumCycDC(i) Then
                    DCNoCyc(i, m_epdata.NumGroups + 1) = (m_epdata.DC(i, 0) - CycDC(i, 0)) / (SumDC(i) - SumCycDC(i))
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

            For i = 1 To m_epdata.NumGroups
                SumCycDC(i) = 0
                'vc280oct99         'import is in element 0 not in n1+1
                'For j = 1 To NumGroups + 1
                For j = 0 To m_epdata.NumGroups
                    SumCycDC(i) = SumCycDC(i) + CycDC(i, j)
                Next j
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcSumCycDC() " & ex.Message, ex)
        End Try


    End Sub
#End Region

#Region "Required PP"
    ''' <summary>
    ''' Calcualte public varaibles need for required primary production
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>Calculates values for "Primary prod. required" tab in EwE5. This was done in DisplayPPReq() in EwE5.</remarks>
    Public Function CalculateRequiredPP() As Boolean
        Dim numPaths As Integer, TabNo As Integer
        'this really has to change 
        Try
            TabNo = 1 'in EwE5 this is called from Tab number one
            Topic = 4 ' IIf(TabNo = 0, 2, 4)
            FindPaths(numPaths, m_epdata.B, m_epdata.PB, m_epdata.QB, m_epdata.EE, m_epdata.DC, m_epdata.fCatch)

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalculateRequiredPP()", ex)
        Finally

        End Try

    End Function

    'Private Sub FindPaths(ByRef NumOfPaths As Long, ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef fCatch() As Single)
    Public Sub FindPaths(ByRef NumOfPaths As Integer, ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef fCatch() As Single)

        'EwE5 definition
        'Private Sub FindPaths(ByRef NumOfPaths As Long, ByRef TabNo As Integer, ByRef B() As Single, ByRef PB() As Single, ByRef QB() As Single, ByRef EE() As Single, ByRef DC(,) As Single, ByRef fCatch() As Single)

        '*** DIM LastComp(1 TO m_epdata.NumGroups + 1)
        Dim Answer As Object = Nothing
        Dim Pass As Long  'Integer Found 290598 thanks to Eni / VC
        Dim APred As Integer, Comp As Integer, pred As Integer, prey As Integer, NewPath As Integer
        Dim T1 As Long = 0

        Try

            ReDim SumDetRequired(1, m_epdata.NumGroups)
            ReDim SumPPRequired(1, m_epdata.NumGroups)
            For pred = 0 To 1
                RaiseToDet(pred) = 0
                RaiseToPP(pred) = 0
            Next
            If DoneAlready = False Then DoneAlready = True
            '    Answer = MsgBox("Your model has many groups, this procedure may take a long time to complete. Do you want to continue?", vbInformation + vbYesNo, "Find pathways: tedious for large models")
            '    If Answer = vbNo Then AbortRun = True: Exit Sub
            'End If


            'AbortRun = False
            'If DoWhat <> "Ecosim PPR" Then
            '    DoWhat = "PPR"
            '    frmWait.Caption = "Identification of pathways in progress, (cancel if pathways not needed)"
            '    frmWait.Frame1.Visible = False
            '    frmWait.PBar.max = 1000 'm_epdata.NumLiving
            '    frmWait.ZOrder()
            '    frmWait.Show() '0
            '    frmWait.Refresh()
            'End If

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'jb March-16-07 I'm still working on this stuff
            For pivot = 1 To m_epdata.NumLiving         '... prey
                'The pivot element is the starting element
                'If (Topic < 2.5 And Catch(Pivot) > 0) Or ((Topic = 4 Or Topic = 14) And QB(Pivot) > 0) Or Sel = Pivot Then
                If (Topic < 2.5) Or ((Topic = 4 Or Topic = 14) And QB(pivot) > 0) Or Sel = pivot Then
                    'If Topic is 1 or 2 then all groups with catches, if Topic is 4 or 14 then
                    'look only at consumers, otherwise only look at the Sel(ected) group

                    Array.Clear(Path, 0, Path.Length)
                    'init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path)
                    Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, 1, LastComp)

                    Path(pivot - 1) = pivot    ' Path's limits are Pivot-1 to Level
                    Pass = 0
                    For Level = pivot To 2 * m_epdata.NumGroups
                        Pass = Pass + 1
                        If Pass > 10 ^ 7 Then GoTo NextPivot 'MsgBox "Too many pathways, results incomplete": Exit For
                        If Path(Level - 1) > 0 Then
                            pred = Path(Level - 1)
                        Else
                            pred = pivot
                        End If
                        For Comp = LastComp(Level) To m_epdata.NumGroups
                            If QB(pred) = 0 Or (Topic <> 14 And pred > m_epdata.NumLiving) Then     'VCJan97 was = m_epdata.NumGroups
                                APred = 0      ' Here it use EXIT FOR and also reset APred
                                Exit For
                            End If
                            If DCNoCyc(pred, Comp) > 0.00001 Then
                                If pred <= m_epdata.NumLiving Or Topic = 14 Then
                                    prey = Comp
                                    Path(Level) = 0
                                    CheckPath2(prey)         ' Check if Comp is in Path already
                                    If prey = 0 Then        ' Then it is in Path
                                        If Comp <> 0 Then GoTo NextComp2
                                    End If
                                    If pivot = Comp Then
                                        Path(Level) = Comp
                                        Path(Level) = 0
                                    Else
                                        Path(Level) = Comp             ' Include group in Path
                                        Path(Level + 1) = 0
                                        LastComp(Level) = Comp
                                        LastComp(Level + 1) = 1         'Pivot
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
                                PathPrintReqPP()
                                CalcRequiredPP(B, PB, QB, EE, DC, fCatch)
                                If prey > m_epdata.NumLiving Then NumDetPath = NumDetPath + 1 Else NumLivPath = NumLivPath + 1
                                NewPath = 0
                            End If
                            If Level > pivot Then LastComp(Level - 1) = Path(Level - 1) + 1
                            Path(Level) = 0
                            Level = Level - 2
                            If Level = pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                        End If

                        m_manager.UpdateCalculateRequiredPP(NumDetPath + NumLivPath)
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
                        If m_manager.CancelRequiredPrimaryProdRun Then Exit Sub
                    Next Level

                End If                     'End If for groups with no catches
NextPivot:
            Next pivot
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
        'Private Sub CheckPath2(ByVal prey As Integer)
        Try
            Dim K As Integer
            For K = 1 To Level + 1              ' Pivot% - 1 TO Level% + 1
                If prey = Path(K) Then prey = 0 : Exit For
            Next K
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CheckPath2() " & ex.Message, ex)
        End Try
    End Sub


    Private Sub CalcTotalPP()
        Try
            totalPP = 0
            For i As Integer = 1 To m_epdata.NumLiving
                If m_epdata.PP(i) > 0 Then totalPP = totalPP + m_epdata.B(i) * m_epdata.PB(i) * m_epdata.PP(i) '* EE(i%)
            Next i
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcTotalPP() " & ex.Message, ex)
        End Try
    End Sub


    'Sub CheckIfPPisInput(ByVal NoPP As Boolean)
    '    NoPP = False
    '    For K As Integer = 1 To m_epdata.NumGroups
    '        If m_epdata.PP(K) > 0.01 And (m_epdata.missing(K, 1) = True Or m_epdata.missing(K, 2) = True) Then NoPP = True
    '    Next K

    'End Sub


    Private Sub CalcDetFlow(ByVal DetFlow As Single)
        Dim i As Integer

        Try
            DetFlow = 0
            For i = m_epdata.NumLiving + 1 To m_epdata.NumGroups
                DetFlow = DetFlow + m_epdata.DtImp(i) '+ det(0, i)
            Next i
            For i = 1 To m_epdata.NumLiving
                If m_epdata.PP(i) > 0 Then DetFlow = DetFlow + m_epdata.B(i) * m_epdata.PB(i) * (1 - m_epdata.EE(i)) * m_epdata.PP(i)
            Next
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("CalcDetFlow() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub FlowInternal(ByVal Flow(,) As Single, ByVal FlowCyc(,) As Single)
        Dim i As Integer, j As Integer

        Try
            For i = 1 To m_epdata.NumGroups
                For j = 1 To m_epdata.NumGroups
                    If j <= m_epdata.NumLiving Then
                        Flow(i, j) = m_epdata.B(j) * m_epdata.QB(j) * m_epdata.DC(j, i)
                    Else
                        Flow(i, j) = CSng(m_epdata.det(0, i))
                    End If
                    If i <= m_epdata.NumLiving And j <= m_epdata.NumLiving Then FlowCyc(i, j) = Flow(i, j)
                Next j
            Next i             'Flow ij (formerly Aij) is amount eaten of
            'group i  by predator j 
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("FlowInternal() " & ex.Message, ex)
        End Try


    End Sub

    Private Sub FlowExternal(ByVal Flow(,) As Single, ByVal FlowCyc(,) As Single)
        'TCyc = 0
        Try
            For i As Integer = 1 To m_epdata.NumGroups + 3
                If i <= m_epdata.NumGroups Then
                    Flow(m_epdata.NumGroups + 1, i) = im(i)    'Amount imported
                    Flow(i, m_epdata.NumGroups + 2) = m_epdata.Ex(i)
                    Flow(i, m_epdata.NumGroups + 3) = m_epdata.Resp(i)
                    If i <= m_epdata.NumLiving Then
                        FlowCyc(m_epdata.NumGroups + 1, i) = im(i)
                        FlowCyc(i, m_epdata.NumGroups + 2) = m_epdata.Ex(i)
                        FlowCyc(i, m_epdata.NumGroups + 3) = m_epdata.Resp(i)
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

            totalCatch = 0
            For i As Integer = 1 To m_epdata.NumGroups
                totalCatch = totalCatch + m_epdata.fCatch(i)
            Next i%
        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("EstimTotalCatch() " & ex.Message, ex)
        End Try

    End Sub

    Private Sub CalcRequiredPP(ByVal B() As Single, ByVal PB() As Single, ByVal QB() As Single, ByVal EE() As Single, ByVal DC(,) As Single, ByVal fCatch() As Single)
        Dim pred As Integer
        Dim prey As Integer
        Dim i As Integer
        Dim Si As Integer
        Dim K As Integer
        Dim Raise(1) As Single
        Try
            Si = Path(pivot - 1)
            If (Topic < 2.5) Or ((Topic = 4 Or Topic = 14) And QB(Si) > 0) Or Sel = Si Then
                'If (Topic < 2.5 And Catch(Si) > 0) Or ((Topic = 4 Or Topic = 14) And QB(Si) > 0) Or Sel = Si Then
                ' If Topic is 1 or 2 then all groups with catches.
                ' If Topic is 3 then look only at consumers
                ' otherwise only look at the Sel(ected) group
                For K = pivot - 1 To Level - 2
                    If QB(Path(K)) > 0 And PB(Path(K)) > 0 Then
                        pred = Path(K)
                        prey = Path(K + 1)
                        If K = pivot - 1 Then
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

            If prey > m_epdata.NumLiving Then                     'Detritus path
                For i = 0 To 1
                    RaiseToDet(i) = RaiseToDet(i) + Raise(i)
                    SumDetRequired(i, Si) = SumDetRequired(i, Si) + Raise(i)
                Next
            Else                                    'PP path
                For i = 0 To 1
                    RaiseToPP(i) = RaiseToPP(i) + Raise(i)
                    SumPPRequired(i, Si) = SumPPRequired(i, Si) + Raise(i)
                Next
            End If
            'Add up all flow
            For i = 0 To 1
                'If prey <= m_epdata.NumLiving Then
                SumRaise(i, Si) = SumRaise(i, Si) + Raise(i)
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
        For K = pivot - 1 To Level              'PreyOK
            If Path(K) > 0 Then
                If arrow = 1 Then FirstInPath = Path(K)
                If arrow = 0 Then
                    If FirstInPath > 0 Then
                        '                   PRINT USING "Path number: #######"; NumOfPaths + 1;
                        '                   Print FirstInPath;
                        NumPath(FirstInPath) = NumPath(FirstInPath) + 1
                        FirstInPath = 0
                    End If
                    '                Print "<--";
                End If
                '             If Arrow = 0 Then Print Path(k);
                '             PrintC = 1
                NoArrows = NoArrows + 1
                arrow = 0
            End If
        Next K
        If arrow = 0 Then NumOfPaths = NumOfPaths + 1

    End Sub

#End Region

#Region "Pathway Cycles"

    Public Sub FindCycles(ByRef DC(,) As Single, ByVal Topic As Integer, ByVal Sel As Integer, ByVal Sel2 As Integer, ByRef NoPath As Integer, ByRef NoArrows As Integer)
        'Public Sub FindCycles1(ByVal DC() As Single, ByVal Topic As Integer, ByVal Sel As Integer, ByVal Sel2 As Integer, ByVal NoPath As Integer, ByVal NoArrows As Integer)
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
        lstPathways.Clear()

        NumP = 0
        If Topic = 3 Then
            pivot = Sel2
            Array.Clear(Path, 0, Path.Length)
            Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, 1, LastComp)
            Path(0) = pivot                        ' Path's limits are Pivot-1 to Level
            For Level = 1 To m_epdata.NumLiving                    ' In this case Pred is the prey and
                If Path(Level - 1) > 0 Then        ' Prey the predator
                    pred = Path(Level - 1)
                Else
                    pred = pivot
                End If
                For Comp = LastComp(Level) To m_epdata.NumLiving
                    If bStopNetworkAnnalysis Then Exit Sub
                    If DC(Comp, pred) > 0 Then
                        prey = Comp
                        Path(Level) = 0
                        CheckPath2(prey)
                        If prey = 0 Then
                            If Comp = pivot Then
                            Else
                                GoTo NextComp3
                            End If
                        End If
                        If pivot = Comp Then
                        Else
                            Path(Level) = Comp                      'Include group in Path
                            Path(Level + 1) = 0
                            LastComp(Level) = Comp
                            LastComp(Level + 1) = 1  'Pivot
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
                        printpath(Topic, Mess, NoArrows, NoPath) 'joeh
                        NewPath = 0
                    End If
                    If Level > 1 Then LastComp(Level - 1) = Path(Level - 1) + 1
                    Path(Level) = 0
                    Level = Level - 2
                    If Level = -1 Then Exit For 'Exit the Level for next and try new pivot
                End If
NextLevel3:
            Next Level
        Else                                             '... for topics 1,2,4,5, 14
            For pivot = 1 To m_epdata.NumLiving                             '... prey
                If m_epdata.QB(pivot) = 0 Then
                    GoTo NextPivot       '... excludes producers
                Else
                    If Topic = 1 Or Topic = 2 Then
                        If Sel = pivot Then
                        Else
                            GoTo NextPivot
                        End If
                    End If
                    Array.Clear(Path, 0, Path.Length)
                    ' Init1DimInteg(0, 2 * m_epdata.NumGroups + 2, Path())
                    If Topic = 4 Or Topic = 14 Then
                        Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, pivot, LastComp)
                    Else
                        Assign1DimInteg(1, 2 * m_epdata.NumGroups + 1, 1, LastComp)
                    End If
                    Path(pivot - 1) = pivot                 ' Path's limits are Pivot-1 to Level
                    Pass = 0

                    '*** changed counter below to go to 2 * N in  March 94, vc
                    '*** This was to avoid problem with too few levels

                    For Level = pivot To 2 * m_epdata.NumLiving
                        Pass = Pass + 1
                        If Pass > 10 ^ 7 Then GoTo NextPivot
                        If Path(Level - 1) > 0 Then
                            pred = Path(Level - 1)
                        Else
                            pred = pivot
                        End If
                        For Comp = LastComp(Level) To m_epdata.NumGroups
                            If DC(pred, Comp) > 0 Then
                                If (Topic <> 14 And pred <= m_epdata.NumLiving) Or Topic = 14 Then
                                    'For topic = 14 also cycles through detritus are OK
                                    prey = Comp
                                    Path(Level) = 0
                                    CheckPath2(prey) '...1
                                    If prey = 0 Then
                                        If Comp = pivot Then
                                        Else
                                            GoTo NextComp
                                        End If
                                    End If
                                    If pivot = Comp Then
                                        Path(Level) = Comp
                                        If Topic = 4 Or Topic = 14 Then
                                            'GoSub CyclePrint1   ' Have identified a cycle
                                            'PrintCycle(Topic, Mess, arrow, NoPath)
                                            PrintCycle(Topic, Mess, NoArrows, NoPath)
                                        End If
                                        Path(Level) = 0
                                    Else
                                        Path(Level) = Comp                  ' Include group in Path
                                        Path(Level + 1) = 0
                                        LastComp(Level) = Comp
                                        If Topic = 5 Then
                                            LastComp(Level + 1) = 1        'Pivot
                                        Else
                                            LastComp(Level + 1) = 1        'Pivot
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
                                If Topic = 1 Or Topic = 2 Then PreyProd(Topic, Sel2, Mess, NoArrows, NoPath) 'GoSub PreyProd  'joeh
                                If Topic = 5 Then CountPath() 'GoSub CountPath
                                NewPath = 0
                            End If
                            If Level > pivot Then LastComp(Level - 1) = Path(Level - 1) + 1
                            Path(Level) = 0
                            Level = Level - 2
                            If Level = pivot - 2 Then Exit For 'Exit the Level for next and try new pivot
                        End If
NextLevel:
                    Next Level
NextPivot:
                End If
            Next pivot

        End If
        NumberArrows = NoArrows 'joeh
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

        '        kStart = IIf(Topic = 3, 0, pivot - 1)
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

    'Private Sub PrintCycle(ByVal Topic As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub PrintCycle(ByVal Topic As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim K As Integer
        Dim arrow As Integer

        mess = ""
        arrow = 0
        'First check if in path already
        '(then the cycle doesn't start with the lowest group number in the path
        For K = pivot To Level - 1
            If Path(pivot - 1) > Path(K) Then arrow = -1
            'If so the grpnumber in Path(k) is < than the starting point for the cycle
            'and it should not be included in the path. It is there already
        Next
        If arrow = 0 Then
            arrow = 1
            For K = pivot - 1 To Level
                If Path(K) > 0 Then
                    If arrow = 1 Then aa = Path(K)
                    If arrow = 0 And Topic > 0 Then
                        If aa > 0 Then mess = mess & LTrim(CStr(aa)) : aa = 0
                        mess = mess & Chr(171) & "-"
                        mess = mess & LTrim(CStr(Path(K)))
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
                Me.m_manager.UpdatePrintCycle(NoPath)
                lstPathways.Add(mess)
            End If
            If arrow = 0 Then NoPath = NoPath + 1
        End If
        Return

    End Sub


    'Private Sub printpath(ByVal Topic As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub printpath(ByVal Topic As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim K As Integer
        Dim KStart As Integer
        'Dim numP As Integer
        Dim arrow As Integer

        mess = ""
        arrow = 1
        '    PrintC = 0

        KStart = CInt(IIf(Topic = 3, 0, pivot - 1))
        For K = KStart To Level              'PreyOK
            If Path(K) > 0 Then
                If arrow = 1 Then aa = Path(K)
                If arrow = 0 And Topic > 0 Then
                    If aa > 0 Then
                        mess$ = mess$ & LTrim(CStr(aa))
                        aa = 0
                    End If
                    If Topic = 3 Then
                        mess$ = mess$ & "-" & Chr(187)
                    Else
                        mess$ = mess$ & Chr(171) & "-"
                    End If
                    mess$ = mess$ & LTrim(CStr(Path(K)))

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
        Me.m_manager.UpdatePrintPath(NoPath)
        'jb 
        lstPathways.Add(mess)
        Return

    End Sub


    'Private Sub PreyProd(ByVal Topic As Integer, ByVal sel2 As Integer, ByRef mess As String, ByRef arrow As Integer, ByRef NoPath As Integer)
    Private Sub PreyProd(ByVal Topic As Integer, ByVal sel2 As Integer, ByRef mess As String, ByRef NoArrows As Integer, ByRef NoPath As Integer) 'joeh
        Dim PreyOK As Integer = -1
        Dim ProdOK As Integer = -1
        For K As Integer = pivot - 1 To Level
            If sel2 = Path(K) Then PreyOK = K
            If m_epdata.PP(Path(K)) > 0 Then ProdOK = K
        Next K
        If ProdOK = -1 Then Return
        If Topic = 2 And PreyOK = -1 Then Return
        'printpath(Topic, mess, arrow, NoPath)
        printpath(Topic, mess, NoArrows, NoPath)  'joeh
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

#End Region

#Region "Ecosim"


    Public Sub InitForEcosim()

        ' RetVal = MsgBox("Estimate PPR? (very time consuming for bigger model)", vbQuestion + vbYesNo, "PPR?")
        '  PPRon = IIf(RetVal = vbYes, True, False)
        Try

            ReDim FIB(m_esdata.NTimes)   'FIB-index in ecosim
            ReDim TLC(m_esdata.NTimes)  'TL of catch in Ecosim
            ' ReDim ValueSim(m_esdata.NTimes)  'value of catch in Ecosim
            ' ReDim CostSim(m_esdata.NTimes)  'cost of catch in Ecosim
            ReDim CatchSim(m_esdata.NTimes)
            ReDim Kemptons(m_esdata.NTimes)

            ReDim RelativeSumOfCatchPlot(m_esdata.NTimes)
            ReDim RelativeKemptonsPlot(m_esdata.NTimes)
            ReDim TLSimPlot(m_epdata.NumGroups, m_esdata.NTimes)
            ReDim TLCatchPlot(m_esdata.NTimes)


            ReDim Elect(m_epdata.NumLiving, m_epdata.NumGroups, m_esdata.NTimes)
            ReDim TLSim(m_epdata.NumGroups)
            ReDim SumDC(m_epdata.NumGroups)

            ReDim BB(m_epdata.NumGroups)

            ReDim ByTL(m_esdata.NTimes, 7, 4)

            'summary for PPRon flag
            ReDim RelativeCatchPPR(m_esdata.NTimes)
            ReDim RelativeCatchDetReq(m_esdata.NTimes)

            'RaiseToPP() and RaiseToDet() where computed in the network anylysis 
            'that has to have been run before the network analysis for ecosim can be initialized
            OrigPPR(0) = RaiseToPP(0)
            OrigPPR(1) = RaiseToDet(0)


            'If Inrow > 0 Then
            '    ReDim ByTLspace(Ntimes, 7, 4, HalfDegreeRow, HalfDegreeCol)
            '    ReDim SAUP_C_Space(NumCatchCodes, HalfDegreeRow, HalfDegreeCol)
            'End If

            Dim OldCatch As Single
            Dim OldTL As Single

            'Get the inital starting biomass for EstimateTLofCatch() in Ecosim this is done in SetBBtoStartBiomass()
            'BB() is the biomass at the current time step and not exposed by ecosim so cEcoNetwork uses a local BB
            Array.Copy(m_esdata.StartBiomass, BB, m_esdata.StartBiomass.Length)
            EstimateTLofCatch(0, OldTL, OldCatch, True)

            OrigKempton = Kemptons(0)

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False)
            Throw New ApplicationException(Me.ToString & ".InitForEcosim() Error: " & ex.Message)
        End Try

    End Sub

    Public Function EcosimTimestep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) As Boolean
        'TLCatch gets computed for each time step
        'it will need to be stored
        Dim TLCatch As Single
        Dim relCatch As Single 'relative catch computed in EstimateTLofCatch()

        Try

            'get a local copy of the biomass computed by ecosim at this time step
            Array.Copy(BiomassAtTimestep, BB, BiomassAtTimestep.Length)

            ' If ipct = 6 Then Estimate_Taxon_indices(CInt((iTime - ipct) / 12))
            EstimateTLsInEcosim(iTime, True)
            EstimateTLofCatch(iTime, TLCatch, relCatch, True)         'Orig total catch =catchsum
            Kemptons(iTime) = FunctionKemptonsQ(BB, 0.25)

            PrepareUlanowForCallFromEcosim(iTime)

            'Summary data
            'see EwE5 RunModel() "If IndicesOn Then"
            RelativeSumOfCatchPlot(iTime) = relCatch
            RelativeKemptonsPlot(iTime) = Kemptons(iTime) / OrigKempton
            'EwE5 code for plotting trophic level of catch it Indicies plot
            'If Solving = False Then
            '    frmSim1.PlotTime.Line (told, OldTL)-(t, TLcatch - IIf(frmSim1.optTL, 0, 2)), QBColor(0)
            TLCatchPlot(iTime) = TLCatch - 2 'subtract two from TLCatch for plotting I have no idea why

            For igrp As Integer = 1 To m_epdata.NumGroups
                TLSimPlot(igrp, iTime) = TLSim(igrp)
            Next igrp

            If PPRon Then
                RelativeCatchPPR(iTime) = RaiseToPP(0) / OrigPPR(0)
                RelativeCatchDetReq(iTime) = RaiseToDet(0) / OrigPPR(1)
            End If


        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".EcosimTimestep()", ex)
        End Try




    End Function

    'Called during the intialization of Ecosim NA
    Public Sub EstimateTLofCatch(ByVal TimeStep As Integer, ByRef TL As Single, ByRef Cat As Single, ByVal DoAll As Boolean)
        Dim i As Integer
        Dim fCatch As Single
        Dim totalTL As Single
        'Static StartFIB As Single
        'Static StartTL As Single
        'Static StartCatch As Single
        'Static MaxFactor As Single
        'Static DecreaseMaxFactor As Boolean

        Try
            BiomassFish = 0
            BiomassInvert = 0
            CatchSim(TimeStep) = 0
            If TimeStep = 0 Then
                For i = 1 To m_epdata.NumGroups
                    TLSim(i) = m_epdata.TTLX(i)
                Next
            End If
            For i = 1 To m_epdata.NumGroups
                'jb all groups here
                '        If GrpsToShow(i) Then 'only visible groups
                fCatch = m_esdata.FishTime(i) * BB(i)
                If m_epdata.GroupIsFish(i) Then BiomassFish = BiomassFish + BB(i)
                If m_epdata.GroupIsInvert(i) Then BiomassInvert = BiomassInvert + BB(i)
                CatchSim(TimeStep) = CatchSim(TimeStep) + fCatch
                totalTL = totalTL + TLSim(i) * fCatch
                '      End If
            Next
            If CatchSim(0) > 0 Then
                If DoAll Then
                    TLC(TimeStep) = totalTL / CatchSim(TimeStep)
                    'Calculate FIB-index:
                    If TimeStep = 0 Then
                        'StartTL = TL
                        'StartCatch = CatchSim
                        'If DecreaseMaxFactor Then MaxFactor = MaxFactor / 2
                        'If MaxFactor < 1 Then MaxFactor = 10
                        FIB(0) = 1
                        Cat = 0
                        'jb what the fuck is this
                        ' TL = TLC(0) - IIf(frmSim1.optTL, 0, 2) '=1
                        Kemptons(0) = FunctionKemptonsQ(m_esdata.StartBiomass, 0.25)
                        'DecreaseMaxFactor = False
                        'TLC(0) = 2
                        'CatchSim(0) = 1
                    Else
                        'TL = MaxFactor * (TLC(TimeStep) - TLC(1)) + 1
                        TL = TLC(TimeStep)
                        'If Abs(TL) > 2 Then DecreaseMaxFactor = True
                        'If TL < 0.1 Then Stop
                        If TimeStep = 1 Then
                            FIB(1) = 1 'CatchSim(1) * 10 ^ (TLC(1) - 1)
                            Cat = 1
                        Else
                            FIB(TimeStep) = CSng((CatchSim(TimeStep) * 10 ^ (TLC(TimeStep) - 1)) / (CatchSim(1) * 10 ^ (TLC(1) - 1)))
                            Cat = CatchSim(TimeStep) / CatchSim(1)
                        End If
                        'Debug.Print TimeStep, CatchSim(TimeStep), FIB(TimeStep), TLC(TimeStep)
                        'FIB = log[(Catch(y) · 10TL(y)-1) / (Catch(0) · 10TL(0)-1)]
                    End If
                Else
                    m_epdata.TLcatch = totalTL / CatchSim(TimeStep)
                End If
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.ToString)
            Throw New ApplicationException(Me.ToString & ".EstimateTLofCatch() Error: " & ex.Message, ex)
        End Try


    End Sub


    Public Sub EstimateTLsInEcosim(ByVal time As Integer, ByVal DoAll As Boolean)
        Dim i As Integer
        Dim j As Integer
        Dim Diet(,) As Single
        Dim SumDiet As Single
        Dim SumR() As Single
        Dim Alpha(,) As Single
        'On Local Error Resume Next
        Dim SumBio() As Single

        Try

            ReDim Diet(m_epdata.NumGroups, m_epdata.NumGroups)

            For i = 1 To m_epdata.NumLiving  'consumer
                If m_esdata.Eatenby(i) > 0 Then
                    SumDiet = 0
                    For j = 1 To m_epdata.NumGroups  'food
                        Diet(i, j) = m_esdata.Consumpt(j, i) / m_esdata.Eatenby(i)
                        SumDiet = SumDiet + Diet(i, j)
                    Next
                    If SumDiet > 0 Then
                        For j = 1 To m_epdata.NumGroups  'food
                            Diet(i, j) = Diet(i, j) / SumDiet
                        Next
                    End If
                End If
            Next

            EstimateTrophicLevels(Diet, TLSim)

            If DoAll Then
                ReDim SumBio(m_epdata.NumLiving)
                ReDim SumR(m_epdata.NumLiving)
                ReDim Alpha(m_epdata.NumLiving, m_epdata.NumGroups)
                For i = 1 To m_epdata.NumLiving
                    If m_epdata.QB(i) > 0 Then    'Estimate Chesson from Sim
                        SumBio(i) = 0
                        For j = 1 To m_epdata.NumGroups
                            SumBio(i) = SumBio(i) + m_epdata.B(j)
                        Next
                        SumR(i) = 0
                        For j = 1 To m_epdata.NumGroups              'FOLLOWING CHESSON (1983)
                            If m_epdata.B(j) > 0 Then Alpha(i, j) = Diet(i, j) / (BB(j) / SumBio(i))
                            SumR(i) = SumR(i) + Alpha(i, j)
                        Next

                        If SumR(i) > 0 Then
                            For j = 1 To m_epdata.NumGroups
                                Alpha(i, j) = Alpha(i, j) / SumR(i)
                            Next                'THIS ALPHA IS THE SAME AS CHESSONS ALPHA
                        End If

                        For j = 1 To m_epdata.NumGroups
                            Elect(i, j, time) = Alpha(i, j) '(NumGroups * Alpha(j) - 1) / ((NumGroups - 2) * Alpha(j) + 1)
                        Next
                    End If
                Next
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".EstimateTLsInEcosim() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".EstimateTLsInEcosim() Error: " & ex.Message, ex)
        End Try

    End Sub


    Public Sub EstimateTrophicLevels(ByVal Diet(,) As Single, ByVal TLreturn() As Single)
        Dim i As Integer, j As Integer
        Dim ErrCode As Integer
        Dim TL() As Single

        Try
            ReDim TL(m_epdata.NumGroups)
            For i = 1 To m_epdata.NumGroups
                'TTLX(i) = 1
                TL(i) = 1
                For j = 1 To m_epdata.NumGroups
                    m_epdata.LHS(i, j) = 0
                Next j
            Next i

            For i = 1 To m_epdata.NumGroups
                SumDC(i) = 0
                For j = 1 To m_epdata.NumGroups
                    SumDC(i) = SumDC(i) + Diet(i, j)
                Next j
            Next i

            'Estimation of trophic levels: TTLX
            'The DC is made to sum to one, this means that it is assumed
            'that import to strict consumers has the same trophic level as
            'other prey for the group
            For i = 1 To m_epdata.NumGroups
                For j = 1 To m_epdata.NumGroups
                    If m_epdata.PP(i) = 1 Then            'Strict Primary producer, so no diet composition (even if it may have in carbon model)
                        m_epdata.LHS(i, j) = 0
                    ElseIf m_epdata.PP(i) > 0 Then            'partly a primary producer
                        m_epdata.LHS(i, j) = -Diet(i, j)
                        'ElseIf SumDC(i) > 0 And SumDC(i) < 1 Then 'Consumer with import
                    ElseIf SumDC(i) > 0 And Math.Abs(SumDC(i) - 1) > 0.0001 Then 'Consumer with import
                        m_epdata.LHS(i, j) = -Diet(i, j) / SumDC(i)
                    Else                          'Consumer
                        m_epdata.LHS(i, j) = -Diet(i, j)
                    End If
                    If m_epdata.PP(i) > 0 And m_epdata.PP(i) < 1 Then
                        'Mixed producer / consumer: TTLX should reflect both roles
                        m_epdata.LHS(i, j) = -Diet(i, j) * (1 - m_epdata.PP(i))
                    End If
                Next j
                m_epdata.LHS(i, i) = 1 - Diet(i, i)
            Next i

            For i = m_epdata.NumLiving + 1 To m_epdata.NumGroups          'multidet version for
                For j = 1 To m_epdata.NumGroups
                    m_epdata.LHS(i, j) = 0
                Next j
                m_epdata.LHS(i, i) = 1
            Next i

            ErrCode = MatSEqnS(m_epdata.LHS, TL)   'Inverses matrix to find
            If ErrCode = 0 Then 'no error
                For i = 1 To m_epdata.NumGroups : TLreturn(i) = TL(i) : Next
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".EstimateTrophicLevels() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".EstimateTrophicLevels() Error: " & ex.Message, ex)
        End Try

    End Sub


    Public Function FunctionKemptonsQ(ByVal Bio() As Single, ByVal Quan As Single) As Single
        'VC programmed this function 23 October 2002 from Tony Pitcher's description
        Dim BLower As Single
        Dim BUpper As Single
        Dim i As Integer
        Dim j As Integer
        Dim minB As Single
        Dim Smallest As Integer
        Dim Rank() As Integer
        Dim Used() As Boolean
        Dim Lower As Single
        Dim upper As Single
        Dim NumGr As Integer

        Try

            'We now know the current biomasses for each group = bb(i) the biomass for each group at the end of the simulation
            'Find the min and max biomass, only look at theliving groups
            FunctionKemptonsQ = 0
            ReDim Rank(m_epdata.NumLiving)
            ReDim Used(m_epdata.NumLiving)
            NumGr = 0
            For i = 1 To m_epdata.NumLiving
                If m_epdata.TTLX(i) < 3 Then
                    Used(i) = True 'don't include low trophic level species in diversity index
                Else
                    NumGr = NumGr + 1
                End If
            Next
            For i = 1 To NumGr
                minB = 1000000
                Smallest = 0
                For j = 1 To m_epdata.NumLiving
                    If Used(j) = False And Bio(j) < minB Then
                        minB = Bio(j)
                        Smallest = j
                    End If
                Next
                'After each round we have the smallest remaining biomass
                If Smallest > 0 Then    'there will be some where it won't
                    Used(Smallest) = True
                    Rank(i) = Smallest
                End If
            Next
            'after i rounds we have sorted all groups after biomasses in Rank()
            'Now we can find the percentiles:
            Lower = Quan * NumGr    'm_epdata.NumLiving           'e.g., 0.25* m_epdata.NumLiving
            upper = (1 - Quan) * NumGr  'm_epdata.NumLiving
            BLower = (Lower - CInt(Lower - 0.5)) * Bio(Rank(CInt(Lower - 0.5))) + (1 - (Lower - CInt(Lower - 0.5))) * Bio(Rank(CInt(Lower - 0.5) + 1))
            BUpper = (1 - (upper - CInt(upper - 0.5))) * Bio(Rank(CInt(upper - 0.5))) + (upper - CInt(upper - 0.5)) * Bio(Rank(CInt(upper - 0.5) + 1))
            'We can now calculate Kemptons Q-index:
            FunctionKemptonsQ = CSng(NumGr / Math.Log(BUpper / BLower) / 2)
            'Using the equation from Kemptons Species diversity index:
            'Q= St / [ 2 log(Pi0.25ST/Pi0.75St)] wher Piq is the proportional abundance of the qth most abundant species
            'exitFunction:

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".FunctionKemptonsQ() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".FunctionKemptonsQ() Error: " & ex.Message, ex)
        End Try


    End Function


    Public Sub EstimateTL_Indices(ByVal Year As Integer)
        Dim i As Integer
        Dim j As Integer
        Dim G As Integer
        ' Dim col As Integer
        '  Dim row As Integer
        Try

            For i = 1 To m_epdata.NumLiving
                ' If GrpsToShow(i) Then
                For j = 2 To 7  'use the first tl for the tl-plus group
                    If AM(j, i) > 0.01 Then
                        ByTL(Year, j, 0) = ByTL(Year, j, 0) + AM(j, i) * BB(i)
                        ByTL(Year, j, 1) = ByTL(Year, j, 1) + AM(j, i) * m_esdata.loss(i)
                        ByTL(Year, j, 2) = ByTL(Year, j, 2) + AM(j, i) * m_esdata.Eatenby(i)
                        ByTL(Year, j, 3) = ByTL(Year, j, 3) + AM(j, i) * m_esdata.FishTime(i) * BB(i)
                        For G = 1 To m_epdata.NumFleet
                            If m_epdata.Landing(G, i) + m_epdata.Discard(G, i) > 0 Then
                                ByTL(Year, j, 4) = ByTL(Year, j, 4) + BB(i) * m_esdata.FishTime(i) * m_epdata.Market(G, i) * m_epdata.Landing(G, i) / (m_epdata.Landing(G, i) + m_epdata.Discard(G, i))
                            End If
                        Next
                        'For row = 1 To Inrow : For col = 1 To Incol
                        '        If Depth(row, col) > 0 Then
                        '            ByTLspace(Year, j, 0, row, col) = ByTLspace(Year, j, 0, row, col) + AM(j, i) * BB(i) * HalfDegreeBcell(row, col, i)
                        '            ByTLspace(Year, j, 1, row, col) = ByTLspace(Year, j, 1, row, col) + AM(j, i) * loss(i) * HalfDegreeBcell(row, col, i)
                        '            ByTLspace(Year, j, 2, row, col) = ByTLspace(Year, j, 2, row, col) + AM(j, i) * Eatenby(i) * HalfDegreeBcell(row, col, i)
                        '            ByTLspace(Year, j, 3, row, col) = ByTLspace(Year, j, 3, row, col) + AM(j, i) * FishTime(i) * BB(i) * HalfDegreeBcell(row, col, i)
                        '        End If
                        '    Next : Next
                    End If
                Next j
                '     If TL_cut_off > 0 Then
                j = 1   'use the spot for the first trophic level for saving
                'If m_epdata.TTLX(i) >= TL_cut_off Then
                ByTL(Year, j, 0) = ByTL(Year, j, 0) + BB(i)
                ByTL(Year, j, 1) = ByTL(Year, j, 1) + m_esdata.loss(i)
                ByTL(Year, j, 2) = ByTL(Year, j, 2) + m_esdata.Eatenby(i)
                ByTL(Year, j, 3) = ByTL(Year, j, 3) + m_esdata.FishTime(i) * BB(i)
                For G = 1 To m_epdata.NumFleet
                    'If Landing(G, i) + Discard(G, i) > 0 Then
                    '040106 VC fixed below, wrong scaling
                    If m_epdata.fCatch(i) > 0 Then
                        ByTL(Year, j, 4) = ByTL(Year, j, 4) + BB(i) * m_esdata.FishTime(i) * m_epdata.Market(G, i) * m_epdata.Landing(G, i) / m_epdata.fCatch(i) '(Landing(G, i) + Discard(G, i))
                    End If
                Next
                'For row = 1 To Inrow
                '    For col = 1 To Incol
                '        'If row = 29 And col = 23 Then Stop
                '        If Depth(row, col) > 0 Then       'Here HalfDegreeBcell is the cell biomass relative to the average biomass, ie a scaling factor
                '            ByTLspace(Year, j, 0, row, col) = ByTLspace(Year, j, 0, row, col) + BB(i) * HalfDegreeBcell(row, col, i)
                '            ByTLspace(Year, j, 1, row, col) = ByTLspace(Year, j, 1, row, col) + loss(i) * HalfDegreeBcell(row, col, i)
                '            ByTLspace(Year, j, 2, row, col) = ByTLspace(Year, j, 2, row, col) + Eatenby(i) * HalfDegreeBcell(row, col, i)
                '            ByTLspace(Year, j, 3, row, col) = ByTLspace(Year, j, 3, row, col) + FishTime(i) * BB(i) * HalfDegreeBcell(row, col, i)
                '        End If
                '    Next
                'Next
                '  End If
                '  End If
                '  End If
                '    'Sum up all catches by SAUP groupings
                '    If m_epdata.fCatch(i) > 0 Then
                '        For j = 1 To NumCatchCodes
                '            If CatchCode(j, i) > 0 And CatchCode(0, i) > 0 Then 'then CatchCode(0,i) will also be > 0
                '                For row = 1 To Inrow : For col = 1 To Incol
                '                        If Depth(row, col) > 0 Then
                '                            SAUP_C_Space(j, row, col) = SAUP_C_Space(j, row, col) + FishTime(i) * BB(i) * HalfDegreeBcell(row, col, i) * CatchCode(j, i) / CatchCode(0, i)
                '                        End If
                '                    Next : Next
                '            End If
                '        Next
                '    End If
            Next i

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".EstimateTL_Indices() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".EstimateTL_Indices() Error: " & ex.Message, ex)
        End Try


    End Sub


    Public Sub PrepareUlanowForCallFromEcosim(ByVal Round As Integer)
        '040218VC for Sheila's Network in Ecosim calc's
        '060915VC updated this sub, added more indices, and also added call to Lindeman (plus passing parameters to that sub)

        'ToDo_jb PrepareUlanowForCallFromEcosim PPRon
        'ToDo_jb PrepareUlanowForCallFromEcosim File Saving

        Dim i As Integer, j As Integer, ii As Integer
        Dim Biom As Single
        Dim PProd As Single
        Dim Production As Single
        Dim fCatch As Single
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

            NetworkDimensioning()
            ReDim SimCatch(m_epdata.NumGroups)
            ReDim SimB(m_epdata.NumGroups)
            ReDim SimPB(m_epdata.NumGroups)
            ReDim SimQB(m_epdata.NumGroups)
            ReDim SimEE(m_epdata.NumGroups)
            ReDim SDiet(m_epdata.NumGroups, m_epdata.NumGroups)
            ReDim SimIm(m_epdata.NumGroups)
            ReDim SimEx(m_epdata.NumGroups)
            ReDim SimResp(m_epdata.NumGroups)

            'VC: here using a localvariable called PProd, not same as in Ecopath
            Biom = 0
            PProd = 0  ' Calculated primary production
            Production = 0
            fCatch = 0

            For i = 1 To m_epdata.NumLiving
                SimB(i) = BB(i)
                SimPB(i) = m_esdata.loss(i) / BB(i)
                SimEE(i) = 1 - (m_esdata.loss(i) - m_esdata.Eatenof(i) - m_esdata.FishTime(i)) / (SimPB(i) * BB(i))
                If m_epdata.PP(i) < 1 Then 'only for consumers
                    If m_epdata.GE(i) > 0 Then SimQB(i) = SimPB(i) / m_epdata.GE(i)
                    SimIm(i) = m_epdata.DC(i, 0) * SimQB(i)
                    SimResp(i) = BB(i) * (SimQB(i) - SimPB(i) - m_epdata.GS(i))
                End If
                SimCatch(i) = BB(i) * m_esdata.FishTime(i)
                SimEx(i) = SimCatch(i)

                fCatch = fCatch + SimEx(i)
                Biom = Biom + BB(i)
                Production = Production + BB(i) * SimPB(i)
                If m_epdata.PP(i) > 0 Then PProd = PProd + SimPB(i) * BB(i) * m_epdata.PP(i)
            Next
            For i = m_epdata.NumLiving + 1 To m_epdata.NumGroups
                SimB(i) = BB(i)
                If m_esdata.ToDetritus(i - m_epdata.NumLiving) > 0 Then SimEE(i) = m_esdata.loss(i) / m_esdata.ToDetritus(i - m_epdata.NumLiving)
                'Emig and Imig removed from below, zero for detritus
                SimEx(i) = (m_esdata.ToDetritus(i - m_epdata.NumLiving) - m_epdata.BA(i) - m_esdata.Eatenof(i)) / BB(i)
            Next i

            For ii = 1 To m_esdata.inlinks
                i = m_esdata.ilink(ii) : j = m_esdata.jlink(ii)
                ' If m_esdata.Eatenby(j) > 0 Then SDiet(j, i) = m_esdata.DCMean(j, i) / m_esdata.Eatenby(j)
                'jb Consumpt() contains the same values as DCMean 
                'DCmean is not used anywhere else so use Consumpt() instead
                If m_esdata.Eatenby(j) > 0 Then SDiet(j, i) = m_esdata.Consumpt(i, j) / m_esdata.Eatenby(j)
            Next

            'ToDo_jb PrepareUlanowForCallFromEcosim m_esdata.Consumpt can not be used instead of DCMean for some reason it does not contain the same values
            Ulanow(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, SimEx, SimResp)
            Lindeman(SimB, SimPB, SimQB, SimEE, SDiet, SimIm, SimEx, SimResp)


            If PPRon Then
                ' DoWhat = "Ecosim PPR"
                'Estimate PPR: slow
                ReDim NumPath(m_epdata.NumLiving)
                ReDim SumRaise(1, m_epdata.NumLiving)
                NumOfPaths = 0
                'g_num_ppr_gr1 = 0
                'g_num_ppr_gr2 = 0
                ' AbortRun = False
                'EstimateHostMatrixWithNoCycles
                FindPaths(NumOfPaths, SimB, SimPB, SimQB, SimEE, SDiet, SimCatch)
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
            ReDim Preserve Throughput(Round)
            ReDim Preserve CapacityEcosim(Round)
            ReDim Preserve AscendImport(Round)
            ReDim Preserve AscendFlow(Round)
            ReDim Preserve AscendExport(Round)
            ReDim Preserve AscendResp(Round)
            ReDim Preserve OverheadImport(Round)
            ReDim Preserve OverheadFlow(Round)
            ReDim Preserve OverheadExport(Round)
            ReDim Preserve OverheadResp(Round)
            ReDim Preserve PCI(Round)
            ReDim Preserve FCI(Round)
            ReDim Preserve PathLength(Round)
            ReDim Preserve Export(Round)
            ReDim Preserve Resp(Round)
            ReDim Preserve PrimaryProd(Round)
            ReDim Preserve Prod(Round)
            ReDim Preserve Biomass(Round)
            ReDim Preserve CatchEcosim(Round)
            ReDim Preserve PropFlowDet(Round)
            ReDim Preserve AscendTotal(Round)
            ReDim Preserve AMI(Round)
            ReDim Preserve Entropy(Round)
            Throughput(Round) = TruPut
            CapacityEcosim(Round) = Capacity
            AscendImport(Round) = Aop
            AscendFlow(Round) = Aip
            AscendExport(Round) = Aep
            AscendResp(Round) = Arp
            OverheadImport(Round) = Eop
            OverheadFlow(Round) = Eip
            OverheadExport(Round) = Eep
            OverheadResp(Round) = Erp
            PCI(Round) = 100 * Tc / TCyc
            FCI(Round) = 100 * TcD / TruPut
            PathLength(Round) = TruPut / (SumEx + SumResp)
            Export(Round) = SumEx
            Resp(Round) = SumResp
            PrimaryProd(Round) = PProd
            Prod(Round) = Production
            Biomass(Round) = Biom
            CatchEcosim(Round) = fCatch
            PropFlowDet(Round) = DetIndex
            AscendTotal(Round) = AscendImport(Round) + AscendFlow(Round) + AscendExport(Round) + AscendResp(Round)
            AMI(Round) = AscendTotal(Round) / Throughput(Round)
            Entropy(Round) = CapacityEcosim(Round) / Throughput(Round)

            'If PPRon Then Write #FF, RaiseToPP(0), RaiseToDet(0) Else Write #FF,
            If PPRon Then
                ReDim Preserve RaiseToPPEcosim(Round)
                ReDim Preserve RaiseToDetEcosim(Round)
                RaiseToPPEcosim(Round) = RaiseToPP(0)
                RaiseToDetEcosim(Round) = RaiseToDet(0)
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


    Public Sub Estimate_Taxon_indices(ByVal Year As Integer)
        'Dim i As Integer
        'Dim j As Integer
        'Dim L As Integer
        'Dim row As Integer
        'Dim col As Integer
        'Dim Code As Integer
        'Dim Grp As Integer
        ''Villy 14Jan2001
        ''Used to estimate biomass, pb, qb for each Taxon group
        'If TaxonCount > 0 Or NoTL > 0 Then
        '    For i = 1 To TaxonCount
        '        Grp = TaxonGrp(i)
        '        For L = 1 To TaxonCode(0, 1, i) '=TaxLevel
        '            'TaxLevel is 1=ISSCAAP, 2 Cla, 3 Ord, 4 Fam, 5, Gen, 6, spe
        '            'TaxonCode Stores:
        '            '1Dim: 0 is for codes, 1 is a ref to where code occured first time
        '            '2Dim: 0 TaxonKey, 1 TaxLevel, 2 ISSCAAP, 3 ClaCode, 4 OrdCode, 5 FamCode, 6 GenCode, 7 SpeCode
        '            '3Dim: Ref to number of taxo-records in model
        '            Code = TaxonCode(1, L + 1, i)
        '            If Code > 0 Then
        '                '  Code is the first occurence of the Taxoncode, if the code is < than i
        '                '  it has occurred before, and it need not be plotted
        '                'Biomass
        '                TaxonValue(L, Year, Code, 0) = TaxonValue(L, Year, Code, 0) + BB(Grp) * TaxonProp(i)
        '                'Production = P/B * B
        '                TaxonValue(L, Year, Code, 1) = TaxonValue(L, Year, Code, 1) + loss(Grp) * TaxonProp(i)
        '                'Consumption
        '                TaxonValue(L, Year, Code, 2) = TaxonValue(L, Year, Code, 2) + Eatenby(Grp) * TaxonProp(i)
        '                'Catch
        '                TaxonValue(L, Year, Code, 3) = TaxonValue(L, Year, Code, 3) + FishTime(Grp) * BB(Grp) * TaxonProp(i)
        '                'Value
        '                For j = 1 To NumGear        'Discarded fish has no value
        '                    If Landing(j, Grp) + Discard(j, Grp) > 0 Then
        '                        TaxonValue(L, Year, Code, 4) = TaxonValue(L, Year, Code, 4) + BB(Grp) * FishTime(Grp) * Market(j, Grp) * Landing(j, Grp) / (Landing(j, Grp) + Discard(j, Grp))
        '                    End If
        '                Next
        '            End If
        '        Next
        '        'If HalfDegreeRow > 0 Then 'Do a whole lot of calculations:
        '        '    L = TaxonCode(0, 1, i)
        '        '    Code = TaxonCode(1, L + 1, i)
        '        '    For row = 1 To HalfDegreeRow : For col = 1 To HalfDegreeCol
        '        '            If Depth(row, col) > 0 Then
        '        '                TaxonSpace(Year, Code, 0, row, col) = TaxonSpace(Year, Code, 0, row, col) + HalfDegreeBcell(row, col, Grp) * BB(Grp) * TaxonProp(i)
        '        '                TaxonSpace(Year, Code, 1, row, col) = TaxonSpace(Year, Code, 1, row, col) + HalfDegreeBcell(row, col, Grp) * loss(Grp) * TaxonProp(i)
        '        '                TaxonSpace(Year, Code, 2, row, col) = TaxonSpace(Year, Code, 2, row, col) + HalfDegreeBcell(row, col, Grp) * Eatenby(Grp) * TaxonProp(i)
        '        '                TaxonSpace(Year, Code, 3, row, col) = TaxonSpace(Year, Code, 3, row, col) + HalfDegreeBcell(row, col, Grp) * FishTime(Grp) * BB(Grp) * TaxonProp(i)
        '        '                For j = 1 To NumGear        'Discarded fish has no value
        '        '                    'If Landing(j, Grp) + Discard(j, Grp) > 0 Then
        '        '            If Catch(j) > 0 Then
        '        '                TaxonSpace(Year, Code, 4, row, col) = TaxonSpace(Year, Code, 4, row, col) + HalfDegreeBcell(row, col, 0) * BB(Grp) * FishTime(Grp) * Market(j, Grp) * Landing(j, Grp) / Catch(j) '(Landing(j, Grp) + Discard(j, Grp))
        '        '                    End If
        '        '                Next
        '        '            End If
        '        '        Next : Next
        '        'End If
        '    Next
        'End If

    End Sub



#End Region

End Class
