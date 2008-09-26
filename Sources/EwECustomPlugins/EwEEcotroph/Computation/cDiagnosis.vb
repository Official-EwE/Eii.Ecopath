'==============================================================================
'
' $Log: cDiagnosis.vb,v $
' Revision 1.1  2008/09/26 07:30:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.49  2008/06/05 19:35:36  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports EwECore
Imports System.Xml
Imports System.Windows.Forms

Namespace Computation

    Public Class cDiagnosis

#Region "Private events"
        Public Event InformIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double)
#End Region 'Private events

#Region "Private fields"
        Private Const TL_OUT_INIT As Double = 2.0
        Private Const TL_OUT_FINAL As Double = 7.0
        Private Const TL_INCRM As Double = 0.1
        Private Const NUM_EFFORT_MULTIPLIER As Integer = 11
        Private Const PRGRS_BAR_MAX As Integer = 10

        Private m_EcotrophManager As cEcotrophManager
        Private m_EPdata As cEcopathDataStructures
        Private m_InputArySize As Integer
        Private m_NumTrophicLevel As Integer
        Private m_TrophicLevel As Single()
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
        Public IsDiagnosisParameterRun As Boolean

        Public EffortMultiplier() As Single
        Public Kinetic(,) As Single
        Public Flow(,) As Single
        Public Biomass(,) As Single
        Public KineticRecal(,) As Single
        Public AccessFlow(,) As Single
        Public AccessBiomass(,) As Single
        Public Catches(,) As Single

        Public AbsTotalBiomass() As Single
        Public AbsVulnerBiomass() As Single
        Public AbsPredBiomass() As Single
        Public AbsTotalFlow() As Single
        Public AbsVulnerFlow() As Single
        Public AbsPredFlow() As Single
        Public AbsTotalCatch() As Single
        Public AbsPredCatch() As Single
        Public RelTotalBiomass() As Single
        Public RelVulnerBiomass() As Single
        Public RelPredBiomass() As Single
        Public RelTotalFlow() As Single
        Public RelVulnerFlow() As Single
        Public RelPredFlow() As Single
        Public RelTotalCatch() As Single
        Public RelPredCatch() As Single
        Public TLTotalBiomass() As Single
        Public TLVulnerBiomass() As Single
        Public TLTotalCatch() As Single
        Public IsDiagnosisRun As Boolean
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
        Public Sub RunDiagnosisParameter(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
            IsDiagnosisParameterRun = False
            m_EcotrophManager.InputData.ReadFile("DiagnosisParameter", m_EcotrophManager)
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
            If MainFrom <> My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then IsDiagnosisParameterRun = True
        End Sub

        Public Sub RunDiagnosis(ByVal ToolStp As ToolStrip, ByVal EffortMultiplierType As String)
            Dim NumFlowIteration As Integer
            Dim FlowCriteria As Double
            Dim KineticCriteria As Double

            IsDiagnosisRun = False
            FindReadEffortMultiplier(EffortMultiplierType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeKineticAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            InitializeFlowAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            NumFlowIteration = 1
            IsIterationContinue = True
            Do
                Do
                    FindFlowAry(EffortMultiplierType)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    FindBiomassAry()
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                    'Do
                    'FindKineticAry(EffortMultiplierType)
                    FindKineticRecalAry(EffortMultiplierType)
                    m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                Loop Until StablisationOneFn(KineticCriteria)
                If (NumFlowIteration Mod 500) = 0 Then RaiseEvent InformIterationInfo(KineticCriteria, FlowCriteria)
                NumFlowIteration = NumFlowIteration + 1
                If IsIterationContinue = False Then Exit Do
            Loop Until StablisationTwoFn(EffortMultiplierType, FlowCriteria)

            FindAccessFlowAry(EffortMultiplierType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindCatchesAry(EffortMultiplierType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindAbsoluteParameterAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindRelativeParameterAry(EffortMultiplierType)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTrophicLevelParameterAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsDiagnosisRun = True
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
            ReDim InputTopD(m_InputArySize)

            For Idx As Integer = 1 To m_InputArySize
                InputTopD(Idx) = m_EcotrophManager.InputData.DiagnosisTopD(Idx)
            Next
        End Sub

        Private Sub FindInputFormDAry(ByVal MainFrom As String)
            ReDim InputFormD(m_InputArySize)

            For Idx As Integer = 1 To m_InputArySize
                InputFormD(Idx) = m_EcotrophManager.InputData.DiagnosisFormD(Idx)
            Next
        End Sub

        Private Sub FindReadEffortMultiplier(ByVal EffortMultiplierType As String)
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR
                    ReDim EffortMultiplier(NUM_EFFORT_MULTIPLIER)

                    EffortMultiplier(1) = 0.0
                    EffortMultiplier(2) = 0.2
                    EffortMultiplier(3) = 0.4
                    EffortMultiplier(4) = 0.6
                    EffortMultiplier(5) = 0.8
                    EffortMultiplier(6) = 1.0
                    EffortMultiplier(7) = 1.2
                    EffortMultiplier(8) = 1.4
                    EffortMultiplier(9) = 1.6
                    EffortMultiplier(10) = 1.8
                    EffortMultiplier(11) = 2.0
                Case My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    ReDim EffortMultiplier(NUM_EFFORT_MULTIPLIER)

                    EffortMultiplier(1) = 0.0
                    EffortMultiplier(2) = 0.2
                    EffortMultiplier(3) = 0.4
                    EffortMultiplier(4) = 0.7
                    EffortMultiplier(5) = 1.0
                    EffortMultiplier(6) = 1.5
                    EffortMultiplier(7) = 2.0
                    EffortMultiplier(8) = 2.5
                    EffortMultiplier(9) = 3.0
                    EffortMultiplier(10) = 4.0
                    EffortMultiplier(11) = 5.0
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    m_EcotrophManager.InputData.ReadFile("EffortMultiplier", m_EcotrophManager)
            End Select
        End Sub

        Private Sub InitializeKineticAry()
            Dim Idx As Integer

            m_NumTrophicLevel = 1
            For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                m_NumTrophicLevel = m_NumTrophicLevel + 1
            Next
            ReDim m_TrophicLevel(m_NumTrophicLevel)
            ReDim Kinetic(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            Idx = 1
            m_TrophicLevel(Idx) = 1.0
            For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                m_TrophicLevel(Idx) = CSng(TLIn)
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                For Row As Integer = 1 To m_NumTrophicLevel
                    Kinetic(Row, Col) = InputKinetic(Row)
                Next
            Next
        End Sub

        Private Sub InitializeFlowAry()
            ReDim Flow(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                Flow(1, Col) = InputFlow(1)
            Next
        End Sub

        Private Sub FindFlowAry(ByVal EffortMultiplierType As String)
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        For Row As Integer = 2 To m_NumTrophicLevel
                            Flow(Row, Col) = CSng(Flow(Row - 1, Col) * Math.Exp(-(InputNaturalLossRate(Row - 1) + _
                              (EffortMultiplier(Col) * InputFishMortality(Row - 1) / _
                              Kinetic(Row - 1, Col))) * (m_TrophicLevel(Row) - m_TrophicLevel(Row - 1))))
                        Next
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        For Row As Integer = 2 To m_NumTrophicLevel
                            Flow(Row, Col) = CSng(Flow(Row - 1, Col) * Math.Exp(-(InputNaturalLossRate(Row - 1) + _
                              (m_EcotrophManager.InputData.EffortMultiplier(Col) * InputFishMortality(Row - 1) / _
                              Kinetic(Row - 1, Col))) * (m_TrophicLevel(Row) - m_TrophicLevel(Row - 1))))
                        Next
                    Next
            End Select
        End Sub

        Private Sub FindBiomassAry()
            ReDim Biomass(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                For Row As Integer = 1 To m_NumTrophicLevel
                    Biomass(Row, Col) = Flow(Row, Col) / Kinetic(Row, Col)
                Next
            Next
        End Sub

        'Private Sub FindKineticAry(ByVal EffortMultiplierType As String)
        '    Dim TLOut As Double
        '    Dim Row As Integer
        '    ReDim Kinetic(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

        '    TLOut = 1
        '    Row = 1
        '    For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
        '        Kinetic(Row, Col) = KineticOneFn(EffortMultiplierType, Row, Col)
        '    Next

        '    For TLOut = TL_OUT_INIT To 5.8 Step TL_INCRM
        '        Row = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
        '        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
        '            Kinetic(Row, Col) = KineticTwoFn(EffortMultiplierType, TLOut, Row, Col)
        '        Next
        '    Next

        '    For TLOut = 5.9 To TL_OUT_FINAL Step TL_INCRM
        '        Row = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
        '        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
        '            Kinetic(Row, Col) = KineticThreeFn(TLOut, Col)
        '        Next
        '    Next
        'End Sub

        Private Sub FindKineticRecalAry(ByVal EffortMultiplierType As String)
            Dim TLOut As Double
            Dim Row As Integer
            ReDim KineticRecal(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            TLOut = 1
            Row = 1
            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                KineticRecal(Row, Col) = KineticOneFn(EffortMultiplierType, Row, Col)
            Next

            For TLOut = TL_OUT_INIT To 5.8 Step TL_INCRM
                Row = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                    KineticRecal(Row, Col) = KineticTwoFn(EffortMultiplierType, TLOut, Row, Col)
                Next
            Next

            For TLOut = 5.9 To TL_OUT_FINAL Step TL_INCRM
                Row = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                    KineticRecal(Row, Col) = KineticThreeFn(TLOut, Col)
                Next
            Next
        End Sub

        Private Function KineticOneFn(ByVal EffortMultiplierType As String, ByVal Row As Integer, ByVal Col As Integer) As Single
            Dim TLOut As Double
            Dim RowVar As Integer
            Dim SumBiomass As Double
            Dim SumInputBiomass As Double

            SumBiomass = 0.0
            SumInputBiomass = 0.0
            For TLOut = TL_OUT_INIT To 2.3 Step TL_INCRM
                RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                SumBiomass = SumBiomass + Biomass(RowVar, Col)
                SumInputBiomass = SumInputBiomass + InputBiomass(RowVar)
            Next
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    Return CSng((InputKinetic(Row) - InputFishMortality(Row)) * (1 + m_EcotrophManager.InputData.DiagnosisTopD(Row) * _
                     ((Math.Pow(SumBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)) - Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row))) / _
                     Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)))) + EffortMultiplier(Col) * InputFishMortality(Row))
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    Return CSng((InputKinetic(Row) - InputFishMortality(Row)) * (1 + m_EcotrophManager.InputData.DiagnosisTopD(Row) * _
                      ((Math.Pow(SumBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)) - Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row))) / _
                      Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)))) + m_EcotrophManager.InputData.EffortMultiplier(Col) * InputFishMortality(Row))
            End Select
        End Function

        Private Function KineticTwoFn(ByVal EffortMultiplierType As String, ByVal TrophicLevelOut As Double, ByVal Row As Integer, ByVal Col As Integer) As Single
            Dim TLOut As Double
            Dim TLOutFinal As Double
            Dim RowVar As Integer
            Dim SumBiomass As Double
            Dim SumInputBiomass As Double

            If TrophicLevelOut < 5.79 Then
                TLOutFinal = CSng(TrophicLevelOut + 1.3)
            Else '=5.8
                TLOutFinal = CSng(TrophicLevelOut + 1.2)
            End If
            SumBiomass = 0.0
            SumInputBiomass = 0.0
            For TLOut = CSng(TrophicLevelOut + 0.8) To TLOutFinal Step TL_INCRM
                RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                SumBiomass = SumBiomass + Biomass(RowVar, Col)
                SumInputBiomass = SumInputBiomass + InputBiomass(RowVar)
            Next
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    Return CSng((InputKinetic(Row) - InputFishMortality(Row)) * (1 + m_EcotrophManager.InputData.DiagnosisTopD(Row) * _
                     ((Math.Pow(SumBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)) - Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row))) / _
                     Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)))) + EffortMultiplier(Col) * InputFishMortality(Row))
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    Return CSng((InputKinetic(Row) - InputFishMortality(Row)) * (1 + m_EcotrophManager.InputData.DiagnosisTopD(Row) * _
                      ((Math.Pow(SumBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)) - Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row))) / _
                      Math.Pow(SumInputBiomass, m_EcotrophManager.InputData.DiagnosisFormD(Row)))) + m_EcotrophManager.InputData.EffortMultiplier(Col) * InputFishMortality(Row))
            End Select
        End Function

        Private Function KineticThreeFn(ByVal TrophicLevelOut As Double, ByVal Col As Integer) As Single
            Dim TLOut As Double
            Dim RowVar As Integer
            Dim SumTL As Double
            Dim SumLogKinetic As Double
            Dim AvgTL As Double
            Dim AvgLogKinetic As Double
            Dim SumTLDevLogKineticDev As Double
            Dim SumTLDevSquare As Double
            Dim Slope As Double
            Dim Intercept As Double

            SumTL = 0.0
            SumLogKinetic = 0.0
            For TLOut = 5 To 5.8 Step TL_INCRM
                RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                SumTL = SumTL + TLOut
                SumLogKinetic = SumLogKinetic + Math.Log(Kinetic(RowVar, Col))
            Next
            AvgTL = SumTL / 9.0
            AvgLogKinetic = SumLogKinetic / 9.0
            SumTLDevLogKineticDev = 0.0
            SumTLDevSquare = 0.0
            For TLOut = 5 To 5.8 Step TL_INCRM
                RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                SumTLDevLogKineticDev = SumTLDevLogKineticDev + (TLOut - AvgTL) * _
                  (Math.Log(Kinetic(RowVar, Col)) - AvgLogKinetic)
                SumTLDevSquare = SumTLDevSquare + (TLOut - AvgTL) * (TLOut - AvgTL)
            Next
            Slope = SumTLDevLogKineticDev / SumTLDevSquare
            Intercept = AvgLogKinetic - Slope * AvgTL
            Return CSng(Math.Exp(Intercept + Slope * TrophicLevelOut))
        End Function

        Private Function StablisationOneFn(ByRef KineticCriteria As Double) As Boolean
            Dim SumKineticRecal As Double
            Dim SumKinetic As Double

            SumKineticRecal = 0
            For Row As Integer = 1 To KineticRecal.GetUpperBound(0)
                For Col As Integer = 1 To KineticRecal.GetUpperBound(1)
                    SumKineticRecal = SumKineticRecal + KineticRecal(Row, Col)
                Next
            Next
            SumKinetic = 0
            For Row As Integer = 1 To Kinetic.GetUpperBound(0)
                For Col As Integer = 1 To Kinetic.GetUpperBound(1)
                    SumKinetic = SumKinetic + Kinetic(Row, Col)
                Next
            Next
            If Math.Abs(SumKineticRecal - SumKinetic) > 0.000001 Then
                For Row As Integer = 1 To Kinetic.GetUpperBound(0)
                    For Col As Integer = 1 To Kinetic.GetUpperBound(1)
                        Kinetic(Row, Col) = KineticRecal(Row, Col)
                    Next
                Next
                'For Row As Integer = 1 To Kinetic.GetUpperBound(0)
                '    For Col As Integer = 1 To Kinetic.GetUpperBound(1)
                '        Biomass(Row, Col) = Flow(Row, Col) / Kinetic(Row, Col)
                '    Next
                'Next
                KineticCriteria = SumKineticRecal - SumKinetic
                Return False
            Else
                KineticCriteria = SumKineticRecal - SumKinetic
                Return True
            End If
        End Function

        'Private Function StablisationOneFn() As Boolean
        '    Dim Sum As Double

        '    Sum = 0.0
        '    Console.WriteLine("S" & Sum)
        '    For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
        '        For Row As Integer = 1 To m_NumTrophicLevel
        '            'For Row As Integer = 2 To m_NumTrophicLevel
        '            If Biomass(Row, Col) = (Flow(Row, Col) / Kinetic(Row, Col)) Then MsgBox("Hi")
        '            Sum = Sum + Biomass(Row, Col) - (Flow(Row, Col) / Kinetic(Row, Col))
        '            Console.Write("BM" & Biomass(Row, Col) & " ")
        '            Console.Write("F/K" & Flow(Row, Col) / Kinetic(Row, Col) & "")
        '            Console.Write("D" & Biomass(Row, Col) - (Flow(Row, Col) / Kinetic(Row, Col)))
        '            Console.Write("S" & Sum)
        '        Next
        '        Console.WriteLine()
        '    Next
        '    If Math.Abs(Sum) > 0.000001 Then '0.0001 Then '0.000001 Then   
        '        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
        '            For Row As Integer = 1 To m_NumTrophicLevel
        '                Biomass(Row, Col) = Flow(Row, Col) / Kinetic(Row, Col)
        '                Console.Write(Biomass(Row, Col) & " ")
        '            Next
        '            Console.WriteLine()
        '        Next
        '        Return False
        '    Else
        '        Return True
        '    End If
        'End Function

        Private Function StablisationTwoFn(ByVal EffortMultiplierType As String, ByRef FlowCriteria As Double) As Boolean
            Dim TLOut As Double
            Dim RowVar As Integer
            Dim ColVar As Integer
            Dim Sum1 As Double
            Dim Sum2 As Double
            Dim Sum3 As Double
            Dim BetaFlow(1, NUM_EFFORT_MULTIPLIER) As Single

            Sum1 = 0.0
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If m_EcotrophManager.InputData.EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
            End Select
            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                Sum1 = Sum1 + Biomass(RowVar, ColVar)
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                Sum2 = 0.0
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    RowVar = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                    Sum2 = Sum2 + Biomass(RowVar, Col)
                Next

                BetaFlow(1, Col) = CSng((1.0 - m_EcotrophManager.InputData.DiagnosisBeta) * InputFlow(1) + _
                  m_EcotrophManager.InputData.DiagnosisBeta * InputFlow(1) * Sum2 / Sum1)
            Next

            Sum3 = 0.0
            For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                Sum3 = Sum3 + (BetaFlow(1, col) - Flow(1, col))
            Next

            If Math.Abs(Sum3) > 0.000001 Then ' 0.0001 Then '0.000001 Then
                For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                    Flow(1, Col) = BetaFlow(1, Col)
                Next
                FlowCriteria = Sum3
                Return False
            Else
                FlowCriteria = Sum3
                Return True
            End If
        End Function

        Private Sub FindAccessFlowAry(ByVal EffortMultiplierType As String)
            Dim ColVar As Integer
            Dim AccessFlowRefState(m_NumTrophicLevel) As Single
            Dim AccessNaturalLossRefSate(m_NumTrophicLevel) As Single
            ReDim AccessFlow(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            'Reference state
            For Idx As Integer = 1 To m_NumTrophicLevel
                AccessFlowRefState(Idx) = InputAccessBiomass(Idx) * InputKinetic(Idx)
            Next

            For idx As Integer = 1 To m_NumTrophicLevel - 1
                AccessNaturalLossRefSate(idx) = CSng(Math.Log(AccessFlowRefState(idx) / AccessFlowRefState(idx + 1)) / _
                  (m_TrophicLevel(idx + 1) - m_TrophicLevel(idx)) - InputAccessFishLossRate(idx))
            Next

            'Access flow when TL=1
            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AccessFlow(1, Col) = AccessFlowRefState(1)
            Next

            'Access flow when TL=2
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If m_EcotrophManager.InputData.EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
            End Select
            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AccessFlow(2, Col) = AccessFlowRefState(2) * Flow(2, Col) / Flow(2, ColVar)
            Next

            'Access flow when TL>2
            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For Row As Integer = 3 To m_NumTrophicLevel
                        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                            AccessFlow(Row, Col) = CSng(AccessFlow(Row - 1, Col) * _
                              Math.Exp(-(AccessNaturalLossRefSate(Row - 1) + EffortMultiplier(Col) * _
                              InputAccessFishMortality(Row - 1) / Kinetic(Row - 1, Col)) * _
                              (m_TrophicLevel(Row) - m_TrophicLevel(Row - 1))))
                        Next
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For Row As Integer = 3 To m_NumTrophicLevel
                        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                            AccessFlow(Row, Col) = CSng(AccessFlow(Row - 1, Col) * _
                              Math.Exp(-(AccessNaturalLossRefSate(Row - 1) + m_EcotrophManager.InputData.EffortMultiplier(Col) * _
                              InputAccessFishMortality(Row - 1) / Kinetic(Row - 1, Col)) * _
                              (m_TrophicLevel(Row) - m_TrophicLevel(Row - 1))))
                        Next
                    Next
            End Select
        End Sub

        Private Sub FindAccessBiomassAry()
            ReDim AccessBiomass(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            For Row As Integer = 3 To m_NumTrophicLevel
                For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                    AccessBiomass(Row, Col) = AccessFlow(Row, Col) / Kinetic(Row, Col)
                Next
            Next
        End Sub

        Private Sub FindCatchesAry(ByVal EffortMultiplierType As String)
            ReDim Catches(m_NumTrophicLevel, NUM_EFFORT_MULTIPLIER)

            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For Row As Integer = 3 To m_NumTrophicLevel
                        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                            Catches(Row, Col) = EffortMultiplier(Col) * InputAccessFishMortality(Row) * AccessBiomass(Row, Col)
                        Next
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For Row As Integer = 3 To m_NumTrophicLevel
                        For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                            Catches(Row, Col) = m_EcotrophManager.InputData.EffortMultiplier(Col) * InputAccessFishMortality(Row) * _
                              AccessBiomass(Row, Col)
                            'Catches(Row, Col) = m_EcotrophManager.InputData.EffortMultiplier(Col) * InputFishMortality(Row) * _
                            '  Biomass(Row, Col)
                        Next
                    Next
            End Select
        End Sub

        Private Sub FindAbsoluteParameterAry()
            ReDim AbsTotalBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim AbsVulnerBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim AbsPredBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim AbsTotalFlow(NUM_EFFORT_MULTIPLIER)
            ReDim AbsVulnerFlow(NUM_EFFORT_MULTIPLIER)
            ReDim AbsPredFlow(NUM_EFFORT_MULTIPLIER)
            ReDim AbsTotalCatch(NUM_EFFORT_MULTIPLIER)
            ReDim AbsPredCatch(NUM_EFFORT_MULTIPLIER)

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsTotalBiomass(Col) = CSng(Sum(Biomass, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsVulnerBiomass(Col) = CSng(Sum(AccessBiomass, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsPredBiomass(Col) = CSng(Sum(Biomass, 3.5, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsTotalFlow(Col) = CSng(Sum(Flow, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsVulnerFlow(Col) = CSng(Sum(AccessFlow, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsPredFlow(Col) = CSng(Sum(Flow, 3.5, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsTotalCatch(Col) = CSng(Sum(Catches, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                AbsPredCatch(Col) = CSng(Sum(Catches, 3.5, 7, Col))
            Next
        End Sub

        Private Function Sum(ByVal Variable(,) As Single, ByVal TLInitial As Double, ByVal TLFinal As Double, ByVal Col As Integer) As Double
            Dim RowInitial As Integer
            Dim RowFinal As Integer
            Dim SumValue As Double

            RowInitial = CInt((Int(TLInitial) - 2) * 10 + CInt((TLInitial - Int(TLInitial)) * 10) + 2)
            RowFinal = CInt((Int(TLFinal) - 2) * 10 + CInt((TLFinal - Int(TLFinal)) * 10) + 2)
            SumValue = 0.0

            For Row As Integer = RowInitial To RowFinal
                SumValue = SumValue + Variable(Row, Col)
            Next
            Return SumValue
        End Function

        Private Sub FindRelativeParameterAry(ByVal EffortMultiplierType As String)
            Dim ColVar As Integer
            ReDim RelTotalBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim RelVulnerBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim RelPredBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim RelTotalFlow(NUM_EFFORT_MULTIPLIER)
            ReDim RelVulnerFlow(NUM_EFFORT_MULTIPLIER)
            ReDim RelPredFlow(NUM_EFFORT_MULTIPLIER)
            ReDim RelTotalCatch(NUM_EFFORT_MULTIPLIER)
            ReDim RelPredCatch(NUM_EFFORT_MULTIPLIER)

            Select Case EffortMultiplierType
                Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                    For col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                        If m_EcotrophManager.InputData.EffortMultiplier(col) = 1.0 Then ColVar = col
                    Next
            End Select

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                RelTotalBiomass(Col) = AbsTotalBiomass(Col) / AbsTotalBiomass(ColVar)
                RelVulnerBiomass(Col) = AbsVulnerBiomass(Col) / AbsVulnerBiomass(ColVar)
                RelPredBiomass(Col) = AbsPredBiomass(Col) / AbsPredBiomass(ColVar)
                RelTotalFlow(Col) = AbsTotalFlow(Col) / AbsTotalFlow(ColVar)
                RelVulnerFlow(Col) = AbsVulnerFlow(Col) / AbsVulnerFlow(ColVar)
                RelPredFlow(Col) = AbsPredFlow(Col) / AbsPredFlow(ColVar)
                RelTotalCatch(Col) = AbsTotalCatch(Col) / AbsTotalCatch(ColVar)
                RelPredCatch(Col) = AbsPredCatch(Col) / AbsPredCatch(ColVar)
            Next
        End Sub

        Private Sub FindTrophicLevelParameterAry()
            ReDim TLTotalBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim TLVulnerBiomass(NUM_EFFORT_MULTIPLIER)
            ReDim TLTotalCatch(NUM_EFFORT_MULTIPLIER)

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                TLTotalBiomass(Col) = CSng(SumTL(Biomass, 2, 7, Col) / Sum(Biomass, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                TLVulnerBiomass(Col) = CSng(SumTL(AccessBiomass, 2, 7, Col) / Sum(AccessBiomass, 2, 7, Col))
            Next

            For Col As Integer = 1 To NUM_EFFORT_MULTIPLIER
                TLTotalCatch(Col) = CSng(SumTL(Catches, 2, 7, Col) / Sum(Catches, 2, 7, Col))
            Next
        End Sub

        Private Function SumTL(ByVal Variable(,) As Single, ByVal TLInitial As Double, ByVal TLFinal As Double, ByVal Col As Integer) As Double
            Dim RowInitial As Integer
            Dim RowFinal As Integer
            Dim SumValue As Double

            RowInitial = CInt((Int(TLInitial) - 2) * 10 + CInt((TLInitial - Int(TLInitial)) * 10) + 2)
            RowFinal = CInt((Int(TLFinal) - 2) * 10 + CInt((TLFinal - Int(TLFinal)) * 10) + 2)
            SumValue = 0.0


            For Row As Integer = RowInitial To RowFinal
                SumValue = SumValue + Variable(Row, Col) * m_TrophicLevel(Row)
            Next
            Return SumValue
        End Function
#End Region 'Helper methods
    End Class

End Namespace
