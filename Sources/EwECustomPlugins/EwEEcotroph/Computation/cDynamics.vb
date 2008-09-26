'==============================================================================
'
' $Log: cDynamics.vb,v $
' Revision 1.1  2008/09/26 07:30:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.68  2008/06/05 19:37:00  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports EwECore
Imports System.Xml
Imports System.Windows.Forms

Namespace Computation

    Public Class cDynamics

#Region "Private events"
        Public Event InformCatchPastAnalysisErr()
        Public Event InformIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double)
#End Region 'Private events

#Region "Private fields"
        Private Const PRGRS_BAR_MAX As Integer = 10

        Private m_EcotrophManager As cEcotrophManager
        Private m_EPdata As cEcopathDataStructures
        Private m_InputArySize As Integer
        Private m_IdxSearchTLInf() As Integer
        Private m_IdxSearchTLSup() As Integer
#End Region 'Private fields

#Region "Public fields"
        Public InputBiomass() As Single
        Public InputAccessBiomass() As Single
        Public InputFlow() As Single
        Public InputKinetic() As Single
        Public InputCatches() As Single
        Public InputFishLossRate() As Single
        Public InputAccessFishLossRate() As Single
        Public InputNaturalLossRate() As Single
        Public InputFishMortality() As Single
        Public InputAccessFishMortality() As Single
        Public InputSelectivity() As Single
        Public InputTime() As Single
        Public InputTopD() As Single
        Public InputFormD() As Single

        Public IntrpTime2() As Single
        Public IntrpITime2() As Single
        Public IntrpTLInf() As Single
        Public IntrpTLSup() As Single
        Public IntrpTL() As Single
        Public IntrpTLMid() As Single
        Public IntrpITL() As Single
        Public IntrpBiomass() As Single
        Public IntrpAccessBiomass() As Single
        Public IntrpCatches() As Single
        Public IntrpKinetic() As Single
        Public IntrpFlow() As Single
        Public IntrpFishLossRate() As Single
        Public IntrpAccessFishLossRate() As Single
        Public IntrpNaturalLossRate() As Single
        Public IntrpFishMortality() As Single
        Public IntrpAccessFishMortality() As Single
        Public IntrpSelectivity() As Single
        Public IntrpTopD() As Single
        Public IntrpFormD() As Single
        Public IntrpAccessFlow() As Single
        Public IntrpAccessNaturalLossRate() As Single
        Public IsDynamicsParameterRun As Boolean

        Public Kinetic(,) As Single
        Public Biomass(,) As Single
        Public Flow(,) As Single
        Public AccessFlow(,) As Single
        Public FishLossRate(,) As Single
        Public Catches(,) As Single
        Public CatchMultiplier() As Single
        Public BiomassPred(,) As Single
        Public KineticRecal(,) As Single
        Public BetaFlow(,) As Single
        Public FishMortality(,) As Single
        Public EffortMultiplier() As Single
        Public AccessFishLossRate(,) As Single
        Public AccessBiomass(,) As Single

        Public SryTotalBiomass() As Single
        Public SryAccessBiomass() As Single
        Public SryPredBiomass() As Single
        Public SryTotalFlow() As Single
        Public SryAccessFlow() As Single
        Public SryPredFlow() As Single
        Public SryTotalCatch() As Single
        Public SryPredCatch() As Single
        Public IsDynamicsRun As Boolean
        Public IsIterationContinue As Boolean
#End Region 'Public fields

#Region "Constructors"
        Public Sub New(ByRef EcotrophManager As cEcotrophManager)
            m_EcotrophManager = EcotrophManager
        End Sub
#End Region 'Constructors

#Region "Public Properties"
        Public WriteOnly Property EcopathData() As cEcopathDataStructures
            Set(ByVal value As cEcopathDataStructures)
                m_EPdata = value
            End Set
        End Property
#End Region 'Public Properties

#Region "Public methods"
        Public Sub RunDynamicsParameter(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
            IsDynamicsParameterRun = False
            m_EcotrophManager.InputData.ReadFile("DynamicsParameter", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindInputBiomassAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputAccessBiomassAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputFlowAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputKineticAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputCatchesAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputFishLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputAccessFishLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputNaturalLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputFishMortalityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputAccessFishMortalityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputSelectivityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputTimeAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputTopDAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindInputFormDAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindIntrpTime2Ary(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpITime2Ary(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeIntrpTLInfAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeIntrpTLSupAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeIntrpTLAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            If MainFrom <> My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then
                Do
                    FindIntrpTLInfAry(MainFrom)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindIntrpTLSupAry(MainFrom)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindIntrpTLAry(MainFrom)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                Loop Until IntrpTL(IntrpTL.GetUpperBound(0)) >= 5.5 '6.0
            End If

            FindIntrpTLMidAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpITLAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpBiomassAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpAccessBiomassAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpCatchesAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpFlowAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindIntrpKineticAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpFishLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpAccessFishLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpNaturalLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpFishMortalityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpAccessFishMortalityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpSelectivityAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpTopDAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpFormDAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpAccessFlowAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindIntrpAccessNaturalLossRateAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            If MainFrom <> My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then IsDynamicsParameterRun = True
        End Sub

        Public Sub RunDynamics(ByVal ToolStp As ToolStrip, ByVal CatchHistoryType As String, Optional ByVal CatchPastAnalysisFilePath As String = "")
            Dim IsStabilisationOneChecked As Boolean
            Dim NumKineticIteration As Integer
            Dim FlowCriteria As Double
            Dim KineticCriteria As Double

            IsDynamicsRun = False
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    m_EcotrophManager.InputData.ReadFile("ForecastYear", m_EcotrophManager)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    m_EcotrophManager.InputData.ReadFile("CatchMultiplier", m_EcotrophManager)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    m_EcotrophManager.InputData.ReadFile("IndexPPForecast", m_EcotrophManager)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    If m_EcotrophManager.InputData.ReadFile("CatchPastAnalysis", m_EcotrophManager, CatchPastAnalysisFilePath) = False Then
                        RaiseEvent InformCatchPastAnalysisErr()
                        Exit Sub
                    End If
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    m_EcotrophManager.InputData.ReadFile("IndexPPPastAnalysis", m_EcotrophManager)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            End Select

            InitializeKineticAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeBiomassAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeFlowAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeAccessFlowAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            NumKineticIteration = 1
            IsIterationContinue = True
            Do
                IsStabilisationOneChecked = False
                FindFishLossRateAry(CatchHistoryType)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                Select Case CatchHistoryType
                    Case My.Resources.TREE_NODE_CATCH_FORECAST
                        'FindFishLossRateAry(CatchHistoryType)
                        'm_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                        FindCatchesAry()
                        m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                        'FindFishLossRateAry(CatchHistoryType)
                        'm_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                        FindCatchMultiplierAry()
                        m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                End Select
                Do
                    FindFlowAry(CatchHistoryType, IsStabilisationOneChecked)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindBiomassAry()
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindBiomassPredAry(CatchHistoryType)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindKineticRecalAry(CatchHistoryType)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    'Do
                    FindBetaFlowAry(CatchHistoryType)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                Loop Until StablisationOneFn(IsStabilisationOneChecked, FlowCriteria)
                If (NumKineticIteration Mod 500) = 0 Then RaiseEvent InformIterationInfo(KineticCriteria, FlowCriteria)
                NumKineticIteration = NumKineticIteration + 1
                If IsIterationContinue = False Then Exit Do
            Loop Until StablisationTwoFn(KineticCriteria)

            FindFishMortality(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindEffortMultiplierAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessParameterAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindSummaryParameterAry(CatchHistoryType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsDynamicsRun = True
        End Sub

        Public Sub RunDynamicsCatches(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
            FindInputCatchesAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            'The call to FindInputTimeAry is used to check if the transpose algor
            'in the first line of the catch file has been chosen in the 'Main from'
            'combo box.  If yes, process continue.  If no, an error box will pop up
            FindInputTimeAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindIntrpCatchesAry(MainFrom)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Sub FindInputBiomassAry(ByVal MainFrom As String)
            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    m_InputArySize = m_EcotrophManager.AEFBiomass.GetUpperBound(0)
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    m_InputArySize = m_EcotrophManager.OmniIdxBiomass.GetUpperBound(0)
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    m_InputArySize = m_EcotrophManager.UserDefValBiomass.GetUpperBound(0)
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    m_InputArySize = m_EcotrophManager.FwdCalBiomass.GetUpperBound(0)
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    m_InputArySize = m_EcotrophManager.BwdCalBiomass.GetUpperBound(0)
            End Select
            ReDim InputBiomass(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputBiomass(Idx) = m_EcotrophManager.AEFBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputBiomass(Idx) = m_EcotrophManager.OmniIdxBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputBiomass(Idx) = m_EcotrophManager.UserDefValBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputBiomass(Idx) = m_EcotrophManager.FwdCalBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputBiomass(Idx) = m_EcotrophManager.BwdCalBiomass(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputAccessBiomassAry(ByVal MainFrom As String)
            ReDim InputAccessBiomass(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessBiomass(Idx) = m_EcotrophManager.AEFAccessBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessBiomass(Idx) = m_EcotrophManager.OmniIdxAccessBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessBiomass(Idx) = m_EcotrophManager.UserDefValAccessBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessBiomass(Idx) = m_EcotrophManager.FwdCalAccessBiomass(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessBiomass(Idx) = m_EcotrophManager.BwdCalAccessBiomass(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputFlowAry(ByVal MainFrom As String)
            ReDim InputFlow(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputFlow(Idx) = m_EcotrophManager.AEFFlow(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputFlow(Idx) = m_EcotrophManager.OmniIdxFlow(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputFlow(Idx) = m_EcotrophManager.UserDefValFlow(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFlow(Idx) = m_EcotrophManager.FwdCalFlow(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFlow(Idx) = m_EcotrophManager.BwdCalFlow(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputKineticAry(ByVal MainFrom As String)
            ReDim InputKinetic(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputKinetic(Idx) = m_EcotrophManager.AEFKinetic(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputKinetic(Idx) = m_EcotrophManager.OmniIdxKinetic(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputKinetic(Idx) = m_EcotrophManager.UserDefValKinetic(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputKinetic(Idx) = m_EcotrophManager.FwdCalKinetic(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputKinetic(Idx) = m_EcotrophManager.BwdCalKinetic(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputCatchesAry(ByVal MainFrom As String)
            ReDim InputCatches(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputCatches(Idx) = m_EcotrophManager.AEFCatches(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputCatches(Idx) = m_EcotrophManager.OmniIdxCatches(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputCatches(Idx) = m_EcotrophManager.UserDefValCatches(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputCatches(Idx) = m_EcotrophManager.InputData.Catches(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputCatches(Idx) = m_EcotrophManager.InputData.Catches(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputFishLossRateAry(ByVal MainFrom As String)
            ReDim InputFishLossRate(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishLossRate(Idx) = m_EcotrophManager.AEFFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishLossRate(Idx) = m_EcotrophManager.OmniIdxFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishLossRate(Idx) = m_EcotrophManager.UserDefValFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishLossRate(Idx) = m_EcotrophManager.FwdCalFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishLossRate(Idx) = m_EcotrophManager.BwdCalFishLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputAccessFishLossRateAry(ByVal MainFrom As String)
            ReDim InputAccessFishLossRate(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishLossRate(Idx) = m_EcotrophManager.AEFAccessFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishLossRate(Idx) = m_EcotrophManager.OmniIdxAccessFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishLossRate(Idx) = m_EcotrophManager.UserDefValAccessFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishLossRate(Idx) = m_EcotrophManager.FwdCalAccessFishLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishLossRate(Idx) = m_EcotrophManager.BwdCalAccessFishLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputNaturalLossRateAry(ByVal MainFrom As String)
            ReDim InputNaturalLossRate(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputNaturalLossRate(Idx) = m_EcotrophManager.AEFNaturalLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputNaturalLossRate(Idx) = m_EcotrophManager.OmniIdxNaturalLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputNaturalLossRate(Idx) = m_EcotrophManager.UserDefValNaturalLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize '-1
                        InputNaturalLossRate(Idx) = m_EcotrophManager.CTSANaturalLossRate(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize '-1
                        InputNaturalLossRate(Idx) = m_EcotrophManager.CTSANaturalLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputFishMortalityAry(ByVal MainFrom As String)
            ReDim InputFishMortality(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishMortality(Idx) = m_EcotrophManager.AEFFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishMortality(Idx) = m_EcotrophManager.OmniIdxFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishMortality(Idx) = m_EcotrophManager.UserDefValFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishMortality(Idx) = m_EcotrophManager.FwdCalFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputFishMortality(Idx) = m_EcotrophManager.BwdCalFishMortality(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputAccessFishMortalityAry(ByVal MainFrom As String)
            ReDim InputAccessFishMortality(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishMortality(Idx) = m_EcotrophManager.AEFAccessFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishMortality(Idx) = m_EcotrophManager.OmniIdxAccessFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishMortality(Idx) = m_EcotrophManager.UserDefValAccessFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishMortality(Idx) = m_EcotrophManager.FwdCalAccessFishMortality(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputAccessFishMortality(Idx) = m_EcotrophManager.BwdCalAccessFishMortality(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputSelectivityAry(ByVal MainFrom As String)
            ReDim InputSelectivity(m_InputArySize)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize
                        InputSelectivity(Idx) = m_EcotrophManager.AEFSelectivity(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize
                        InputSelectivity(Idx) = m_EcotrophManager.OmniIdxSelectivity(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize
                        InputSelectivity(Idx) = m_EcotrophManager.UserDefValSelectivity(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputSelectivity(Idx) = m_EcotrophManager.FwdCalSelectivity(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize
                        InputSelectivity(Idx) = m_EcotrophManager.BwdCalSelectivity(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputTimeAry(ByVal MainFrom As String)
            ReDim InputTime(m_InputArySize - 1)

            Select Case MainFrom
                Case My.Resources.DROP_DWN_LST_ITM_PLS_SELECT
                    Exit Sub
                Case My.Resources.DROP_DWN_LST_ITM_TP_AUTO_SMOOTH
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputTime(Idx) = m_EcotrophManager.AEFTime(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_OMNI_IDX
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputTime(Idx) = m_EcotrophManager.OmniIdxTime(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_TP_USER_DEF_SIGMA
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputTime(Idx) = m_EcotrophManager.UserDefValTime(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_FWD_CAL
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputTime(Idx) = m_EcotrophManager.FwdCalTime(Idx)
                    Next
                Case My.Resources.DROP_DWN_LST_ITM_CTSA_BWD_CAL
                    For Idx As Integer = 1 To m_InputArySize - 1
                        InputTime(Idx) = m_EcotrophManager.BwdCalTime(Idx)
                    Next
            End Select
        End Sub

        Private Sub FindInputTopDAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim InputTopD(m_InputArySize)

            For Idx As Integer = 1 To m_InputArySize
                InputTopD(Idx) = m_EcotrophManager.InputData.DynamicsTopD(Idx)
            Next
        End Sub

        Private Sub FindInputFormDAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim InputFormD(m_InputArySize)

            For Idx As Integer = 1 To m_InputArySize
                InputFormD(Idx) = m_EcotrophManager.InputData.DynamicsFormD(Idx)
            Next
        End Sub

        Private Sub FindIntrpTime2Ary(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTime2(m_InputArySize - 1)

            For Idx As Integer = 1 To m_InputArySize - 1
                IntrpTime2(Idx) = InputTime(Idx) - InputTime(2)
            Next
        End Sub

        Private Sub FindIntrpITime2Ary(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpITime2(m_InputArySize - 1)

            IntrpITime2(1) = 0.0
            IntrpITime2(2) = 0.0
            For Idx As Integer = 3 To m_InputArySize - 1
                IntrpITime2(Idx) = CSng(IntrpITime2(Idx - 1) + 0.1)
            Next
        End Sub

        Private Sub InitializeIntrpTLInfAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTLInf(1)

            IntrpTLInf(1) = 0 / 0
        End Sub

        Private Sub InitializeIntrpTLSupAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTLSup(1)

            IntrpTLSup(1) = 0 / 0
        End Sub

        Private Sub InitializeIntrpTLAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTL(1)
            'ReDim IntrpTL(2)

            IntrpTL(1) = 1.0
            'IntrpTL(2) = 2.0
        End Sub

        Private Sub FindIntrpTLInfAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            Dim Idx As Integer

            Idx = IntrpTLInf.GetUpperBound(0) + 1
            ReDim Preserve IntrpTLInf(Idx)
            ReDim Preserve m_IdxSearchTLInf(Idx)

            For IdxSearch As Integer = 1 To IntrpTime2.GetUpperBound(0)
                Select Case IntrpTime2(IdxSearch)
                    Case Is < IntrpITime2(Idx)
                        Exit Select 'continue to search
                    Case Is = IntrpITime2(Idx)
                        IntrpTLInf(Idx) = CSng(0.1 * (IdxSearch - 2) + 2) 'compute Input TL
                        m_IdxSearchTLInf(Idx) = IdxSearch
                        Exit For
                    Case Is > IntrpITime2(Idx)
                        IdxSearch = IdxSearch - 1 'go back one since TLInf is located
                        IntrpTLInf(Idx) = CSng(0.1 * (IdxSearch - 2) + 2) 'compute Input TL
                        m_IdxSearchTLInf(Idx) = IdxSearch
                        Exit For
                End Select
                'If IntrpTime2(IdxSearch) >= IntrpITime2(Idx) Then
                '    IdxSearch = IdxSearch - 1 'go back one since TLInf is located
                '    IntrpTLInf(Idx) = CSng(0.1 * (IdxSearch - 2) + 2) 'compute Input TL
                '    m_IdxSearchTLInf(Idx) = IdxSearch
                '    Exit For
                'End If
            Next
        End Sub

        Private Sub FindIntrpTLSupAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            Dim Idx As Integer

            Idx = IntrpTLSup.GetUpperBound(0) + 1
            ReDim Preserve IntrpTLSup(Idx)
            ReDim Preserve m_IdxSearchTLSup(Idx)

            For IdxSearch As Integer = 1 To IntrpTime2.GetUpperBound(0)
                'If IntrpTime2(IdxSearch) >= IntrpITime2(Idx) Then
                If IntrpTime2(IdxSearch) > IntrpITime2(Idx) Then
                    IntrpTLSup(Idx) = CSng(0.1 * (IdxSearch - 2) + 2) 'compute Input TL
                    m_IdxSearchTLSup(Idx) = IdxSearch
                    Exit For
                End If
            Next
        End Sub

        Private Sub FindIntrpTLAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            Dim Idx As Integer

            Idx = IntrpTL.GetUpperBound(0) + 1
            ReDim Preserve IntrpTL(Idx)

            IntrpTL(Idx) = IntrpTLInf(Idx) + ((IntrpTLSup(Idx) - IntrpTLInf(Idx)) * (IntrpITime2(Idx) - IntrpTime2(m_IdxSearchTLInf(Idx))) _
              / (IntrpTime2(m_IdxSearchTLSup(Idx)) - IntrpTime2(m_IdxSearchTLInf(Idx))))
        End Sub

        Private Sub FindIntrpTLMidAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTLMid(IntrpTL.GetUpperBound(0) - 1)

            IntrpTLMid(1) = 0 / 0 ' NaN
            For Idx As Integer = 2 To IntrpTLMid.GetUpperBound(0)
                IntrpTLMid(Idx) = CSng(IntrpTL(Idx) + (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 2.0)
            Next
        End Sub

        Private Sub FindIntrpITLAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpITL(IntrpTLMid.GetUpperBound(0))

            IntrpITL(1) = 0 / 0 'NaN
            For Idx As Integer = 2 To IntrpITL.GetUpperBound(0)
                IntrpITL(Idx) = CSng(Int(IntrpTLMid(Idx) * 10 - 0.5) / 10)
            Next
        End Sub

        Private Sub FindIntrpBiomassAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            'Dim AryIdx1 As Integer
            'Dim AryIdx2 As Integer
            ReDim IntrpBiomass(IntrpTLMid.GetUpperBound(0))

            IntrpBiomass(1) = InputBiomass(1)
            For Idx As Integer = 2 To IntrpBiomass.GetUpperBound(0)
                'AryIdx1 = CInt((Int(IntrpITL(Idx)) - 2) * 10 + CInt((IntrpITL(Idx) - Int(IntrpITL(Idx))) * 10) + 2)
                'AryIdx2 = CInt((Int(IntrpITL(Idx) + 0.1) - 2) * 10 + CInt((IntrpITL(Idx) + 0.1 - Int(IntrpITL(Idx) + 0.1)) * 10) + 2)
                'IntrpBiomass(Idx) = CSng((InputBiomass(AryIdx1) + (InputBiomass(AryIdx2) - InputBiomass(AryIdx1)) * _
                '  (IntrpTLMid(Idx) - IntrpITL(Idx) - 0.05) / 0.1) * (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 0.1)
                IntrpBiomass(Idx) = Interpolate(InputBiomass, Idx)
            Next
        End Sub

        Private Function Interpolate(ByVal InputVariable() As Single, ByVal Idx As Integer) As Single
            Dim Sum As Double
            Dim AryIdx As Integer

            Sum = 0.0
            If IntrpTL(Idx + 1) < IntrpTLSup(Idx) Then
                AryIdx = CInt((Int(IntrpTLInf(Idx)) - 2) * 10 + CInt((IntrpTLInf(Idx) - Int(IntrpTLInf(Idx))) * 10) + 2)
                Return CSng(InputVariable(AryIdx) * (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 0.1)
            Else
                AryIdx = CInt((Int(IntrpTLInf(Idx)) - 2) * 10 + CInt((IntrpTLInf(Idx) - Int(IntrpTLInf(Idx))) * 10) + 2)
                Sum = Sum + InputVariable(AryIdx) * (IntrpTLSup(Idx) - IntrpTL(Idx)) / 0.1
                For TLOut As Single = IntrpTLSup(Idx) To CSng(IntrpTLInf(Idx + 1) - 0.1) Step 0.1
                    AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                    Sum = Sum + InputVariable(AryIdx)
                Next
                AryIdx = CInt((Int(IntrpTLInf(Idx + 1)) - 2) * 10 + CInt((IntrpTLInf(Idx + 1) - Int(IntrpTLInf(Idx + 1))) * 10) + 2)
                Sum = Sum + InputVariable(AryIdx) * (IntrpTL(Idx + 1) - IntrpTLInf(Idx + 1)) / 0.1
                Return CSng(Sum)
            End If
        End Function

        Private Sub FindIntrpAccessBiomassAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            'Dim AryIdx1 As Integer
            'Dim AryIdx2 As Integer
            ReDim IntrpAccessBiomass(IntrpTLMid.GetUpperBound(0))

            IntrpAccessBiomass(1) = InputAccessBiomass(1)
            For Idx As Integer = 2 To IntrpAccessBiomass.GetUpperBound(0)
                'AryIdx1 = CInt((Int(IntrpITL(Idx)) - 2) * 10 + CInt((IntrpITL(Idx) - Int(IntrpITL(Idx))) * 10) + 2)
                'AryIdx2 = CInt((Int(IntrpITL(Idx) + 0.1) - 2) * 10 + CInt((IntrpITL(Idx) + 0.1 - Int(IntrpITL(Idx) + 0.1)) * 10) + 2)
                'IntrpAccessBiomass(Idx) = CSng((InputAccessBiomass(AryIdx1) + (InputAccessBiomass(AryIdx2) - InputAccessBiomass(AryIdx1)) * _
                '  (IntrpTLMid(Idx) - IntrpITL(Idx) - 0.05) / 0.1) * (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 0.1)
                IntrpAccessBiomass(Idx) = Interpolate(InputAccessBiomass, Idx)
            Next
        End Sub

        Private Sub FindIntrpCatchesAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            'Dim AryIdx1 As Integer
            'Dim AryIdx2 As Integer
            ReDim IntrpCatches(IntrpTLMid.GetUpperBound(0))

            IntrpCatches(1) = InputCatches(1)
            For Idx As Integer = 2 To IntrpCatches.GetUpperBound(0)
                'AryIdx1 = CInt((Int(IntrpITL(Idx)) - 2) * 10 + CInt((IntrpITL(Idx) - Int(IntrpITL(Idx))) * 10) + 2)
                'AryIdx2 = CInt((Int(IntrpITL(Idx) + 0.1) - 2) * 10 + CInt((IntrpITL(Idx) + 0.1 - Int(IntrpITL(Idx) + 0.1)) * 10) + 2)
                'IntrpCatches(Idx) = CSng((InputCatches(AryIdx1) + (InputCatches(AryIdx2) - InputCatches(AryIdx1)) * _
                '  (IntrpTLMid(Idx) - IntrpITL(Idx) - 0.05) / 0.1) * (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 0.1)
                IntrpCatches(Idx) = Interpolate(InputCatches, Idx)
            Next
        End Sub

        Private Sub FindIntrpFlowAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpFlow(IntrpTLMid.GetUpperBound(0))

            IntrpFlow(1) = InputFlow(1)
            For Idx As Integer = 2 To IntrpFlow.GetUpperBound(0)
                IntrpFlow(Idx) = Interpolate(InputFlow, Idx)
            Next
        End Sub

        Private Sub FindIntrpKineticAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpKinetic(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpKinetic.GetUpperBound(0)
                IntrpKinetic(Idx) = IntrpFlow(Idx) / IntrpBiomass(Idx)
            Next
        End Sub

        'Private Sub FindIntrpKineticAry(ByVal MainFrom As String)
        '    If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
        '    Dim AryIdx1 As Integer
        '    Dim AryIdx2 As Integer
        '    ReDim IntrpKinetic(IntrpTLMid.GetUpperBound(0))

        '    IntrpKinetic(1) = InputKinetic(1)
        '    For Idx As Integer = 2 To IntrpKinetic.GetUpperBound(0)
        '        AryIdx1 = CInt((Int(IntrpITL(Idx)) - 2) * 10 + CInt((IntrpITL(Idx) - Int(IntrpITL(Idx))) * 10) + 2)
        '        AryIdx2 = CInt((Int(IntrpITL(Idx) + 0.1) - 2) * 10 + CInt((IntrpITL(Idx) + 0.1 - Int(IntrpITL(Idx) + 0.1)) * 10) + 2)
        '        IntrpKinetic(Idx) = CSng((InputKinetic(AryIdx1) + (InputKinetic(AryIdx2) - InputKinetic(AryIdx1)) * _
        '          (IntrpTLMid(Idx) - IntrpITL(Idx) - 0.05) / 0.1)) '* (IntrpTL(Idx + 1) - IntrpTL(Idx)) / 0.1)
        '    Next
        'End Sub

        'Private Sub FindIntrpFlowAry(ByVal MainFrom As String)
        '    If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
        '    ReDim IntrpFlow(IntrpTLMid.GetUpperBound(0))

        '    For Idx As Integer = 1 To IntrpFlow.GetUpperBound(0)
        '        IntrpFlow(Idx) = IntrpBiomass(Idx) * IntrpKinetic(Idx)
        '    Next
        'End Sub

        Private Sub FindIntrpFishLossRateAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpFishLossRate(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpFishLossRate.GetUpperBound(0)
                IntrpFishLossRate(Idx) = IntrpCatches(Idx) / IntrpFlow(Idx)
            Next
        End Sub

        Private Sub FindIntrpAccessFishLossRateAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpAccessFishLossRate(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpAccessFishLossRate.GetUpperBound(0)
                IntrpAccessFishLossRate(Idx) = IntrpFishLossRate(Idx) * IntrpBiomass(Idx) / IntrpAccessBiomass(Idx)
            Next
        End Sub

        Private Sub FindIntrpNaturalLossRateAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpNaturalLossRate(IntrpTLMid.GetUpperBound(0) - 1)

            For Idx As Integer = 1 To IntrpNaturalLossRate.GetUpperBound(0)
                IntrpNaturalLossRate(Idx) = CSng(Math.Log((IntrpFlow(Idx) / IntrpFlow(Idx + 1)) * ((IntrpTL(Idx + 2) - IntrpTL(Idx + 1)) / (IntrpTL(Idx + 1) - IntrpTL(Idx)))) / _
                  (IntrpTL(Idx + 1) - IntrpTL(Idx)) - IntrpFishLossRate(Idx))
            Next
        End Sub

        Private Sub FindIntrpFishMortalityAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpFishMortality(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpFishMortality.GetUpperBound(0)
                IntrpFishMortality(Idx) = IntrpFishLossRate(Idx) * IntrpKinetic(Idx)
            Next
        End Sub

        Private Sub FindIntrpAccessFishMortalityAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpAccessFishMortality(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpAccessFishMortality.GetUpperBound(0)
                IntrpAccessFishMortality(Idx) = IntrpAccessFishLossRate(Idx) * IntrpKinetic(Idx)
            Next
        End Sub

        Private Sub FindIntrpSelectivityAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpSelectivity(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpSelectivity.GetUpperBound(0)
                IntrpSelectivity(Idx) = IntrpAccessBiomass(Idx) / IntrpBiomass(Idx)
            Next
        End Sub

        Private Sub FindIntrpTopDAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpTopD(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpTopD.GetUpperBound(0)
                IntrpTopD(Idx) = m_EcotrophManager.InputData.DynamicsTopD(Idx)
            Next
        End Sub

        Private Sub FindIntrpFormDAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpFormD(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpFormD.GetUpperBound(0)
                IntrpFormD(Idx) = m_EcotrophManager.InputData.DynamicsFormD(Idx)
            Next
        End Sub

        Private Sub FindIntrpAccessFlowAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpAccessFlow(IntrpTLMid.GetUpperBound(0))

            For Idx As Integer = 1 To IntrpAccessFlow.GetUpperBound(0)
                IntrpAccessFlow(Idx) = IntrpAccessBiomass(Idx) * IntrpKinetic(Idx)
            Next
        End Sub

        Private Sub FindIntrpAccessNaturalLossRateAry(ByVal MainFrom As String)
            If MainFrom = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then Exit Sub
            ReDim IntrpAccessNaturalLossRate(IntrpTLMid.GetUpperBound(0) - 1)

            For Idx As Integer = 1 To IntrpAccessNaturalLossRate.GetUpperBound(0)
                IntrpAccessNaturalLossRate(Idx) = CSng(Math.Log((IntrpAccessFlow(Idx) / IntrpAccessFlow(Idx + 1)) * ((IntrpTL(Idx + 2) - IntrpTL(Idx + 1)) / (IntrpTL(Idx + 1) - IntrpTL(Idx)))) / _
                  (IntrpTL(Idx + 1) - IntrpTL(Idx)) - IntrpAccessFishLossRate(Idx))
            Next
        End Sub

        Private Sub InitializeKineticAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim Kinetic(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim Kinetic(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Col As Integer = 1 To Kinetic.GetUpperBound(1)
                For Row As Integer = 1 To Kinetic.GetUpperBound(0)
                    Kinetic(Row, Col) = IntrpKinetic(Row)
                Next
            Next
        End Sub

        Private Sub InitializeBiomassAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim Biomass(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim Biomass(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Col As Integer = 1 To Biomass.GetUpperBound(1)
                For Row As Integer = 1 To Biomass.GetUpperBound(0)
                    Biomass(Row, Col) = IntrpBiomass(Row)
                Next
            Next
        End Sub

        Private Sub InitializeFlowAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim Flow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim Flow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Col As Integer = 1 To Flow.GetUpperBound(1)
                For Row As Integer = 1 To Flow.GetUpperBound(0)
                    Flow(Row, Col) = IntrpFlow(Row)
                Next
            Next
        End Sub

        Private Sub InitializeAccessFlowAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim AccessFlow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim AccessFlow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Col As Integer = 1 To AccessFlow.GetUpperBound(1)
                For Row As Integer = 1 To AccessFlow.GetUpperBound(0)
                    AccessFlow(Row, Col) = IntrpAccessFlow(Row)
                Next
            Next
        End Sub

        Private Sub FindFishLossRateAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim FishLossRate(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                    For Col As Integer = 1 To FishLossRate.GetUpperBound(1)
                        For Row As Integer = 1 To FishLossRate.GetUpperBound(0)
                            FishLossRate(Row, Col) = m_EcotrophManager.InputData.CatchMultiplier(Col) * IntrpFishMortality(Row) / Kinetic(Row, Col)
                        Next
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim FishLossRate(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
                    For Col As Integer = 1 To FishLossRate.GetUpperBound(1)
                        For Row As Integer = 1 To FishLossRate.GetUpperBound(0)
                            FishLossRate(Row, Col) = m_EcotrophManager.InputData.CatchPastAnalysis(Row, Col) / Flow(Row, Col) 'Catches(Row, Col) / Flow(Row, Col)
                        Next
                    Next
            End Select
        End Sub

        Private Sub FindCatchesAry()
            ReDim Catches(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)

            For Col As Integer = 1 To Catches.GetUpperBound(1)
                For Row As Integer = 1 To Catches.GetUpperBound(0)
                    Catches(Row, Col) = AccessFlow(Row, Col) * FishLossRate(Row, Col) / IntrpSelectivity(Row)
                Next
            Next

        End Sub

        Private Sub FindCatchMultiplierAry()
            Dim Sum1 As Double
            Dim Sum2 As Double
            ReDim CatchMultiplier(m_EcotrophManager.InputData.NumPastAnalysisYear)

            Sum1 = 0.0
            For Row As Integer = 1 To m_EcotrophManager.InputData.CatchPastAnalysis.GetUpperBound(0)
                Sum1 = Sum1 + m_EcotrophManager.InputData.CatchPastAnalysis(Row, 1)
            Next
            For Col As Integer = 1 To CatchMultiplier.GetUpperBound(0)
                Sum2 = 0.0
                For Row As Integer = 1 To m_EcotrophManager.InputData.CatchPastAnalysis.GetUpperBound(0)
                    Sum2 = Sum2 + m_EcotrophManager.InputData.CatchPastAnalysis(Row, Col)
                Next
                CatchMultiplier(Col) = CSng(Sum2 / Sum1)
            Next
        End Sub

        Private Sub FindFlowAry(ByVal CatchHistoryType As String, ByVal IsStabilisationOneChecked As Boolean)
            Dim Row As Integer
            Dim Col As Integer
            Dim ColFinal As Integer
            Dim Sum As Double

            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ColFinal = m_EcotrophManager.InputData.NumForecastYear
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ColFinal = m_EcotrophManager.InputData.NumPastAnalysisYear
            End Select

            If IsStabilisationOneChecked = False Then
                Row = 1
                Select Case CatchHistoryType
                    Case My.Resources.TREE_NODE_CATCH_FORECAST
                        For Col = 1 To ColFinal
                            Flow(Row, Col) = IntrpFlow(Row) * m_EcotrophManager.InputData.IndexPPForecast(Col)
                        Next
                    Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                        For Col = 1 To ColFinal
                            Flow(Row, Col) = IntrpFlow(Row) * m_EcotrophManager.InputData.IndexPPPastAnalysis(Col)
                        Next
                End Select
            End If


            For Row = 2 To 11
                For Col = 1 To ColFinal
                    Flow(Row, Col) = CSng(Flow(Row - 1, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row) - IntrpTL(Row - 1))) * _
                      Math.Exp(-(IntrpNaturalLossRate(Row - 1) + FishLossRate(Row - 1, Col)) * (IntrpTL(Row) - IntrpTL(Row - 1))))
                Next
            Next

            For Row = 12 To Flow.GetUpperBound(0)
                Col = 1
                Sum = 0.0
                For Idx As Integer = Row - 10 To Row - 1
                    Sum = Sum + (IntrpNaturalLossRate(Idx) + FishLossRate(Idx, Col)) * _
                      (IntrpTL(Idx + 1) - IntrpTL(Idx))
                Next
                Flow(Row, Col) = CSng(Flow(Row - 10, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                  Math.Exp(-Sum))

                For Col = 2 To ColFinal
                    Sum = 0.0
                    For Idx As Integer = Row - 10 To Row - 1
                        Sum = Sum + (IntrpNaturalLossRate(Idx) + FishLossRate(Idx, Col - 1)) * _
                          (IntrpTL(Idx + 1) - IntrpTL(Idx))
                    Next
                    Flow(Row, Col) = CSng(Flow(Row - 10, Col - 1) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                      Math.Exp(-Sum))
                Next
            Next
        End Sub

        Private Sub FindBiomassAry()
            For Col As Integer = 1 To Biomass.GetUpperBound(1)
                For Row As Integer = 1 To Biomass.GetUpperBound(0)
                    Biomass(Row, Col) = Flow(Row, Col) / Kinetic(Row, Col)
                Next
            Next
        End Sub

        Private Sub FindBiomassPredAry(ByVal CatchHistoryType As String)
            Dim IntrpTLPred(IntrpTL.GetUpperBound(0) - 1) As Single
            Dim ForcedExit As Boolean

            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim BiomassPred(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim BiomassPred(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            'For col As Integer = 1 To BiomassPred.GetUpperBound(1)
            '    BiomassPred(1, col) = 0 / 0
            'Next

            'For Row As Integer = 2 To BiomassPred.GetUpperBound(0)
            For Row As Integer = 1 To BiomassPred.GetUpperBound(0)
                ForcedExit = False
                For P As Integer = 1 To IntrpTLMid.GetUpperBound(0)
                    If IntrpTLMid(P) > IntrpTLMid(Row) + 1 Then
                        'If (IntrpTLMid(Row) + 1) - IntrpTLMid(P - 1) < IntrpTLMid(P) - (IntrpTLMid(Row) + 1) Then
                        '    'IntrpTLMid(P - 1)is closer to (IntrpTLMid(Row) + 1)
                        '    IntrpTLPred(Row) = IntrpTL(P - 1)
                        'Else
                        '    'IntrpTLMid(P)is closer to (IntrpTLMid(Row) + 1)
                        '    IntrpTLPred(Row) = IntrpTL(P)
                        'End If
                        IntrpTLPred(Row) = IntrpTL(P - 1)
                        ForcedExit = True
                        Exit For
                    End If
                Next
                'cannot find P
                If ForcedExit = False Then
                    IntrpTLPred(Row) = IntrpTL(IntrpTL.GetUpperBound(0) - 1)
                End If
            Next

            For Col As Integer = 1 To BiomassPred.GetUpperBound(1)
                'For Row As Integer = 2 To BiomassPred.GetUpperBound(0)
                For Row As Integer = 1 To BiomassPred.GetUpperBound(0)
                    'If Row > 2 Then
                    If Row > 1 Then
                        For P As Integer = 1 To IntrpTL.GetUpperBound(0) - 1
                            If IntrpTL(P) = IntrpTLPred(Row - 1) Then
                                BiomassPred(Row, Col) = BiomassPred(Row, Col) + Biomass(P, Col)
                                Exit For
                            End If
                        Next
                    End If
                    For P As Integer = 1 To IntrpTL.GetUpperBound(0) - 1
                        If IntrpTL(P) = IntrpTLPred(Row) Then
                            BiomassPred(Row, Col) = BiomassPred(Row, Col) + Biomass(P, Col)
                            Exit For
                        End If
                    Next
                    If Row < BiomassPred.GetUpperBound(0) Then
                        For P As Integer = 1 To IntrpTL.GetUpperBound(0) - 1
                            If IntrpTL(P) = IntrpTLPred(Row + 1) Then
                                BiomassPred(Row, Col) = BiomassPred(Row, Col) + Biomass(P, Col)
                                Exit For
                            End If
                        Next
                    End If
                Next
            Next

            ''For Col As Integer = 1 To BiomassPred.GetUpperBound(1)
            ''    For Row As Integer = 2 To BiomassPred.GetUpperBound(0)
            ''        'For Row As Integer = 1 To BiomassPred.GetUpperBound(0)
            ''        ForcedExit = False
            ''        For P As Integer = 1 To IntrpTLMid.GetUpperBound(0)
            ''            If IntrpTLMid(P) > IntrpTLMid(Row) + 1 Then
            ''                If (IntrpTLMid(Row) + 1) - IntrpTLMid(P - 1) < IntrpTLMid(P) - (IntrpTLMid(Row) + 1) Then
            ''                    'IntrpTLMid(P - 1)is closer to (IntrpTLMid(Row) + 1)
            ''                    BiomassPred(Row, Col) = Biomass(P - 1 - 1, Col) + Biomass(P - 1, Col) + Biomass(P - 1 + 1, Col)
            ''                Else
            ''                    'IntrpTLMid(P)is closer to (IntrpTLMid(Row) + 1)
            ''                    BiomassPred(Row, Col) = Biomass(P - 1, Col) + Biomass(P, Col) + Biomass(P + 1, Col)
            ''                End If
            ''                ForcedExit = True
            ''                Exit For
            ''            End If
            ''        Next
            ''        'cannot find P
            ''        If ForcedExit = False Then BiomassPred(Row, Col) = Biomass(IntrpTLMid.GetUpperBound(0) - 2, Col) + _
            ''          Biomass(IntrpTLMid.GetUpperBound(0) - 1, Col) + Biomass(IntrpTLMid.GetUpperBound(0), Col)
            ''    Next
            ''Next
        End Sub

        Private Sub FindKineticRecalAry(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim KineticRecal(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim KineticRecal(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Col As Integer = 1 To KineticRecal.GetUpperBound(1)
                For Row As Integer = 1 To KineticRecal.GetUpperBound(0)
                    KineticRecal(Row, Col) = CSng((IntrpKinetic(Row) - FishLossRate(Row, 1) * Kinetic(Row, 1)) * _
                      (1.0 + IntrpTopD(Row) * (Math.Pow(BiomassPred(Row, Col), IntrpFormD(Row)) - _
                      Math.Pow(BiomassPred(Row, 1), IntrpFormD(Row))) / Math.Pow(BiomassPred(Row, 1), IntrpFormD(Row))) + _
                      FishLossRate(Row, Col) * Kinetic(Row, Col))
                Next
            Next
        End Sub

        Private Sub FindBetaFlowAry(ByVal CatchHistoryType As String)
            Dim Sum1 As Double
            Dim Sum2 As Double
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim BetaFlow(1, m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim BetaFlow(1, m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            Sum1 = 0.0
            For Row As Integer = 2 To Biomass.GetUpperBound(0)
                Sum1 = Sum1 + Biomass(Row, 1)
            Next

            For Col As Integer = 1 To BetaFlow.GetUpperBound(1)
                Sum2 = 0.0
                For Row As Integer = 2 To Biomass.GetUpperBound(0)
                    Sum2 = Sum2 + Biomass(Row, Col)
                Next
                'BetaFlow(1, Col) = CSng((1.0 - m_EcotrophManager.InputData.DynamicsBeta) * IntrpFlow(1) + _
                '  m_EcotrophManager.InputData.DynamicsBeta * IntrpFlow(1) * Sum2 / Sum1)
                BetaFlow(1, Col) = CSng((1.0 - m_EcotrophManager.InputData.DynamicsBeta) * Flow(1, Col) + _
                  m_EcotrophManager.InputData.DynamicsBeta * Flow(1, Col) * Sum2 / Sum1)
            Next
        End Sub

        Private Function StablisationOneFn(ByRef IsStablisationOneChecked As Boolean, ByRef FlowCriteria As Double) As Boolean
            Dim Sum As Double

            Sum = 0.0
            For Col As Integer = 1 To BetaFlow.GetUpperBound(1)
                Sum = Sum + BetaFlow(1, Col) - Flow(1, Col)
            Next

            If Math.Abs(Sum) > 0.000001 Then
                For Col As Integer = 1 To BetaFlow.GetUpperBound(1)
                    Flow(1, Col) = BetaFlow(1, Col)
                Next
                IsStablisationOneChecked = True
                FlowCriteria = Sum
                Return False
            Else
                FlowCriteria = Sum
                Return True
            End If
        End Function

        Private Function StablisationTwoFn(ByRef KineticCriteria As Double) As Boolean
            Dim Sum1 As Double
            Dim Sum2 As Double

            Sum1 = 0.0
            For Row As Integer = 1 To Kinetic.GetUpperBound(0)
                For Col As Integer = 1 To Kinetic.GetUpperBound(1)
                    Sum1 = Sum1 + Kinetic(Row, Col)
                Next
            Next

            Sum2 = 0.0
            For Row As Integer = 1 To KineticRecal.GetUpperBound(0)
                For Col As Integer = 1 To KineticRecal.GetUpperBound(1)
                    Sum2 = Sum2 + KineticRecal(Row, Col)
                Next
            Next

            If Math.Abs(Sum1 - Sum2) > 0.000001 Then
                For Row As Integer = 1 To KineticRecal.GetUpperBound(0)
                    For Col As Integer = 1 To KineticRecal.GetUpperBound(1)
                        Kinetic(Row, Col) = KineticRecal(Row, Col)
                    Next
                Next
                KineticCriteria = Sum1 - Sum2
                Return False
            Else
                KineticCriteria = Sum1 - Sum2
                Return True
            End If
        End Function

        Private Sub FindFishMortality(ByVal CatchHistoryType As String)
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim FishMortality(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim FishMortality(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            For Row As Integer = 1 To FishMortality.GetUpperBound(0)
                For Col As Integer = 1 To FishMortality.GetUpperBound(1)
                    FishMortality(Row, Col) = FishLossRate(Row, Col) * Kinetic(Row, Col)
                Next
            Next
        End Sub

        Private Sub FindEffortMultiplierAry(ByVal CatchHistoryType As String)
            Dim Sum1 As Double
            Dim Sum2 As Double
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim EffortMultiplier(m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim EffortMultiplier(m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            Sum1 = 0.0
            For Row As Integer = 1 To FishMortality.GetUpperBound(0)
                Sum1 = Sum1 + FishMortality(Row, 1)
            Next
            For Col As Integer = 1 To EffortMultiplier.GetUpperBound(0)
                Sum2 = 0.0
                For Row As Integer = 1 To FishMortality.GetUpperBound(0)
                    Sum2 = Sum2 + FishMortality(Row, Col)
                Next
                EffortMultiplier(Col) = CSng(Sum2 / Sum1)
            Next
        End Sub

        Private Sub FindAccessParameterAry(ByVal CatchHistoryType As String)
            Dim Row As Integer
            Dim Col As Integer
            Dim Sum As Double

            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ReDim AccessFlow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                    ReDim AccessFishLossRate(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                    ReDim AccessBiomass(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumForecastYear)
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ReDim AccessFlow(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
                    ReDim AccessFishLossRate(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
                    ReDim AccessBiomass(IntrpTLMid.GetUpperBound(0), m_EcotrophManager.InputData.NumPastAnalysisYear)
            End Select

            Row = 2
            For Col = 1 To AccessFlow.GetUpperBound(1)
                AccessFlow(Row, Col) = IntrpAccessFlow(Row) * Flow(Row, Col) / Flow(Row, 1)
            Next

            'Row 3 to 11
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    For Row = 3 To 11
                        For Col = 1 To m_EcotrophManager.InputData.NumForecastYear
                            AccessFishLossRate(Row - 1, Col) = Catches(Row - 1, Col) / AccessFlow(Row - 1, Col)
                            AccessBiomass(Row - 1, Col) = AccessFlow(Row - 1, Col) / Kinetic(Row - 1, Col)
                            AccessFlow(Row, Col) = CSng(AccessFlow(Row - 1, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row) - IntrpTL(Row - 1))) * _
                              Math.Exp(-((IntrpAccessNaturalLossRate(Row - 1) + AccessFishLossRate(Row - 1, Col)) * (IntrpTL(Row) - IntrpTL(Row - 1)))))
                        Next
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    For Row = 3 To 11
                        For Col = 1 To m_EcotrophManager.InputData.NumPastAnalysisYear
                            AccessFishLossRate(Row - 1, Col) = m_EcotrophManager.InputData.CatchPastAnalysis(Row - 1, Col) / AccessFlow(Row - 1, Col)
                            AccessBiomass(Row - 1, Col) = AccessFlow(Row - 1, Col) / Kinetic(Row - 1, Col)
                            AccessFlow(Row, Col) = CSng(AccessFlow(Row - 1, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row) - IntrpTL(Row - 1))) * _
                              Math.Exp(-((IntrpAccessNaturalLossRate(Row - 1) + AccessFishLossRate(Row - 1, Col)) * (IntrpTL(Row) - IntrpTL(Row - 1)))))
                        Next
                    Next
            End Select

            'Row 12 to end
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    For Row = 12 To IntrpTLMid.GetUpperBound(0)
                        For Col = 1 To m_EcotrophManager.InputData.NumForecastYear
                            AccessFishLossRate(Row - 1, Col) = Catches(Row - 1, Col) / AccessFlow(Row - 1, Col)
                            AccessBiomass(Row - 1, Col) = AccessFlow(Row - 1, Col) / Kinetic(Row - 1, Col)
                            If Col = 1 Then
                                Sum = 0.0
                                For Idx As Integer = Row - 10 To Row - 1
                                    Sum = Sum + (IntrpAccessNaturalLossRate(Idx) + AccessFishLossRate(Idx, 1)) * _
                                      (IntrpTL(Idx + 1) - IntrpTL(Idx))
                                Next
                                AccessFlow(Row, Col) = CSng(AccessFlow(Row - 10, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                                  Math.Exp(-Sum))

                            Else
                                Sum = 0.0
                                For Idx As Integer = Row - 10 To Row - 1
                                    Sum = Sum + (IntrpAccessNaturalLossRate(Idx) + AccessFishLossRate(Idx, Col - 1)) * _
                                      (IntrpTL(Idx + 1) - IntrpTL(Idx))
                                Next
                                AccessFlow(Row, Col) = CSng(AccessFlow(Row - 10, Col - 1) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                                  Math.Exp(-Sum))
                            End If
                        Next
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    For Row = 12 To IntrpTLMid.GetUpperBound(0)
                        For Col = 1 To m_EcotrophManager.InputData.NumPastAnalysisYear
                            AccessFishLossRate(Row - 1, Col) = m_EcotrophManager.InputData.CatchPastAnalysis(Row - 1, Col) / AccessFlow(Row - 1, Col)
                            AccessBiomass(Row - 1, Col) = AccessFlow(Row - 1, Col) / Kinetic(Row - 1, Col)
                            If Col = 1 Then
                                Sum = 0.0
                                For Idx As Integer = Row - 10 To Row - 1
                                    Sum = Sum + (IntrpAccessNaturalLossRate(Idx) + AccessFishLossRate(Idx, 1)) * _
                                      (IntrpTL(Idx + 1) - IntrpTL(Idx))
                                Next
                                AccessFlow(Row, Col) = CSng(AccessFlow(Row - 10, Col) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                                  Math.Exp(-Sum))

                            Else
                                Sum = 0.0
                                For Idx As Integer = Row - 10 To Row - 1
                                    Sum = Sum + (IntrpAccessNaturalLossRate(Idx) + AccessFishLossRate(Idx, Col - 1)) * _
                                      (IntrpTL(Idx + 1) - IntrpTL(Idx))
                                Next
                                AccessFlow(Row, Col) = CSng(AccessFlow(Row - 10, Col - 1) * ((IntrpTL(Row + 1) - IntrpTL(Row)) / (IntrpTL(Row - 10 + 1) - IntrpTL(Row - 10))) * _
                                  Math.Exp(-Sum))
                            End If
                        Next
                    Next
            End Select

            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    For Col = 1 To m_EcotrophManager.InputData.NumForecastYear
                        AccessFishLossRate(IntrpTLMid.GetUpperBound(0), Col) = Catches(IntrpTLMid.GetUpperBound(0), Col) / AccessFlow(IntrpTLMid.GetUpperBound(0), Col)
                        AccessBiomass(IntrpTLMid.GetUpperBound(0), Col) = AccessFlow(IntrpTLMid.GetUpperBound(0), Col) / Kinetic(IntrpTLMid.GetUpperBound(0), Col)
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    For Col = 1 To m_EcotrophManager.InputData.NumPastAnalysisYear
                        AccessFishLossRate(IntrpTLMid.GetUpperBound(0), Col) = m_EcotrophManager.InputData.CatchPastAnalysis(IntrpTLMid.GetUpperBound(0), Col) / AccessFlow(IntrpTLMid.GetUpperBound(0), Col)
                        AccessBiomass(IntrpTLMid.GetUpperBound(0), Col) = AccessFlow(IntrpTLMid.GetUpperBound(0), Col) / Kinetic(IntrpTLMid.GetUpperBound(0), Col)
                    Next
            End Select
        End Sub

        Private Sub FindSummaryParameterAry(ByVal CatchHistoryType As String)
            Dim RowInitial As Integer
            Dim RowFinal As Integer
            Dim ColFinal As Integer

            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    ColFinal = m_EcotrophManager.InputData.NumForecastYear
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    ColFinal = m_EcotrophManager.InputData.NumPastAnalysisYear
            End Select
            ReDim SryTotalBiomass(ColFinal)
            ReDim SryAccessBiomass(ColFinal)
            ReDim SryPredBiomass(ColFinal)
            ReDim SryTotalFlow(ColFinal)
            ReDim SryAccessFlow(ColFinal)
            ReDim SryPredFlow(ColFinal)
            ReDim SryTotalCatch(ColFinal)
            ReDim SryPredCatch(ColFinal)

            RowInitial = 2 'TL=2
            RowFinal = IntrpTL.GetUpperBound(0) - 1
            For Col As Integer = 1 To ColFinal
                SryTotalBiomass(Col) = CSng(Sum(Biomass, RowInitial, RowFinal, Col))
            Next

            RowInitial = 2 'TL=2
            For Col As Integer = 1 To ColFinal
                SryAccessBiomass(Col) = CSng(Sum(AccessBiomass, RowInitial, RowFinal, Col))
            Next

            RowInitial = 8 'TL=3.59
            For Col As Integer = 1 To ColFinal
                SryPredBiomass(Col) = CSng(Sum(Biomass, RowInitial, RowFinal, Col))
            Next

            RowInitial = 2 'TL=2
            For Col As Integer = 1 To ColFinal
                SryTotalFlow(Col) = CSng(Sum(Flow, RowInitial, RowFinal, Col))
            Next

            RowInitial = 2 'TL=2
            For Col As Integer = 1 To ColFinal
                SryAccessFlow(Col) = CSng(Sum(AccessFlow, RowInitial, RowFinal, Col))
            Next

            RowInitial = 8 'TL=3.59
            For Col As Integer = 1 To ColFinal
                SryPredFlow(Col) = CSng(Sum(Flow, RowInitial, RowFinal, Col))
            Next

            RowInitial = 2 'TL=2
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    For Col As Integer = 1 To ColFinal
                        SryTotalCatch(Col) = CSng(Sum(Catches, RowInitial, RowFinal, Col))
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    For Col As Integer = 1 To ColFinal
                        SryTotalCatch(Col) = CSng(Sum(m_EcotrophManager.InputData.CatchPastAnalysis, _
                          RowInitial, RowFinal, Col))
                    Next
            End Select

            RowInitial = 8 'TL=3.59
            Select Case CatchHistoryType
                Case My.Resources.TREE_NODE_CATCH_FORECAST
                    For Col As Integer = 1 To ColFinal
                        SryPredCatch(Col) = CSng(Sum(Catches, RowInitial, RowFinal, Col))
                    Next
                Case My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                    For Col As Integer = 1 To ColFinal
                        SryPredCatch(Col) = CSng(Sum(m_EcotrophManager.InputData.CatchPastAnalysis, _
                          RowInitial, RowFinal, Col))
                    Next
            End Select
        End Sub

        Private Function Sum(ByVal Variable(,) As Single, ByVal RowInitial As Integer, ByVal RowFinal As Integer, ByVal Col As Integer) As Double
            Dim SumValue As Double

            SumValue = 0.0
            For Row As Integer = RowInitial To RowFinal
                SumValue = SumValue + Variable(Row, Col)
            Next
            Return SumValue
        End Function
#End Region 'Helper methods

    End Class

End Namespace
