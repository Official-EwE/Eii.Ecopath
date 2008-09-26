'==============================================================================
'
' $Log: cEcotrophManager.vb,v $
' Revision 1.1  2008/09/26 07:30:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.84  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports EwECore
Imports System.xml

Public Class cEcotrophManager

#Region "Events"
    Public Event RunTransposePrgrs(ByVal ToolStp As ToolStrip, ByVal BarMax As Integer)
    Public Event CTSAFwdCalIterationInfo(ByVal KineticCriteria As Double)
    Public Event CTSABwdCalIterationInfo(ByVal KineticCriteria As Double)
    Public Event DiagnosisIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double)
    Public Event DynamicsIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double)
    Public Event CatchPastAnalysisErr()
#End Region 'Events

#Region "Private fields"
    Private Enum eRunState
        CoreNotReady
        NetworkNeedsToRun
        NetworkHasRun
    End Enum

    Private m_Transpose As Computation.cTranspose
    Private WithEvents m_CTSA As Computation.cCTSA
    Private WithEvents m_Diagnosis As Computation.cDiagnosis
    Private WithEvents m_Dynamics As Computation.cDynamics
    Private m_Core As cCore
    Private m_Input As cInput
    Private m_Runstate As eRunState
    Private m_EPdata As cEcopathDataStructures
    Private m_Publisher As cMessagePublisher
    Private WithEvents m_CoreStateMonitor As cCoreStateMonitor
#End Region 'Private fields

#Region "Construction and initialization"
    Friend Function Init(ByRef Core As cCore, ByRef Input As cInput) As Boolean
        m_Transpose = New Computation.cTranspose(Me)
        m_CTSA = New Computation.cCTSA(Me)
        m_Diagnosis = New Computation.cDiagnosis(Me)
        m_Dynamics = New Computation.cDynamics(Me)
        m_Core = Core
        m_CoreStateMonitor = m_Core.StateMonitor
        m_Publisher = m_Core.Messages
        m_Input = Input
        Return True
    End Function

    Public Sub New()
        m_Runstate = eRunState.CoreNotReady
    End Sub
#End Region 'Construction and initialization

#Region "Public Properties"
#Region "Inputs"
    Public ReadOnly Property CoreData() As cCore
        Get
            Return m_Core
        End Get
    End Property

    Public Property EcopathData() As cEcopathDataStructures
        Get
            Return m_EPdata
        End Get
        Set(ByVal value As cEcopathDataStructures)
            m_EPdata = value
        End Set
    End Property

    Public ReadOnly Property InputData() As cInput
        Get
            Return m_Input
        End Get
    End Property

    'Public ReadOnly Property ProductionBiomass() As Single()
    '    Get
    '        Return m_EPdata.PB
    '    End Get
    'End Property

    'Public ReadOnly Property SmoothFactor() As Single
    '    Get
    '        Return m_Transpose.SmoothFactor
    '    End Get
    'End Property
#End Region 'Inputs

#Region "Outputs"
#Region "Transpose"
    Public ReadOnly Property SigmaLN() As Single()
        Get
            Return m_Transpose.SigmaLN
        End Get
    End Property

    Public ReadOnly Property Proportion() As Single(,)
        Get
            Return m_Transpose.Proportion
        End Get
    End Property

    Public ReadOnly Property ProportionSTD() As Single(,)
        Get
            Return m_Transpose.ProportionSTD
        End Get
    End Property

    Public ReadOnly Property TransposeBiomass() As Single(,)
        Get
            Return m_Transpose.TransposeBiomass
        End Get
    End Property

    Public ReadOnly Property TLTuncated() As Single()
        Get
            Return m_Transpose.TLTruncated
        End Get
    End Property

    Public ReadOnly Property Flow() As Single(,)
        Get
            Return m_Transpose.Flow
        End Get
    End Property

    Public ReadOnly Property TransposeCatch() As Single(,,)
        Get
            Return m_Transpose.TransposeCatch
        End Get
    End Property

    Public ReadOnly Property TransposeCatchSumGp() As Single(,)
        Get
            Return m_Transpose.TransposeCatchSumGp
        End Get
    End Property

    Public ReadOnly Property AccessBiomass() As Single(,)
        Get
            Return m_Transpose.AccessBiomass
        End Get
    End Property


    Public ReadOnly Property TransposeBiomassSum() As Single()
        Get
            Return m_Transpose.TransposeBiomassSum
        End Get
    End Property

    Public ReadOnly Property AEFBiomass() As Single()
        Get
            Return m_Transpose.AEFBiomass
        End Get
    End Property

    Public ReadOnly Property OmniIdxBiomass() As Single()
        Get
            Return m_Transpose.OmniIdxBiomass
        End Get
    End Property

    Public ReadOnly Property UserDefValBiomass() As Single()
        Get
            Return m_Transpose.UserDefValBiomass
        End Get
    End Property


    Public ReadOnly Property AccessBiomassSum() As Single()
        Get
            Return m_Transpose.AccessBiomassSum
        End Get
    End Property

    Public ReadOnly Property AEFAccessBiomass() As Single()
        Get
            Return m_Transpose.AEFAccessBiomass
        End Get
    End Property

    Public ReadOnly Property OmniIdxAccessBiomass() As Single()
        Get
            Return m_Transpose.OmniIdxAccessBiomass
        End Get
    End Property

    Public ReadOnly Property UserDefValAccessBiomass() As Single()
        Get
            Return m_Transpose.UserDefValAccessBiomass
        End Get
    End Property


    Public ReadOnly Property TransposeFlowSum() As Single()
        Get
            Return m_Transpose.TransposeFlowSum
        End Get
    End Property

    Public ReadOnly Property AEFFlow() As Single()
        Get
            Return m_Transpose.AEFFlow
        End Get
    End Property

    Public ReadOnly Property OmniIdxFlow() As Single()
        Get
            Return m_Transpose.OmniIdxFlow
        End Get
    End Property

    Public ReadOnly Property UserDefValFlow() As Single()
        Get
            Return m_Transpose.UserDefValFlow
        End Get
    End Property


    Public ReadOnly Property Kinetic() As Single()
        Get
            Return m_Transpose.Kinetic
        End Get
    End Property

    Public ReadOnly Property AEFKinetic() As Single()
        Get
            Return m_Transpose.AEFKinetic
        End Get
    End Property

    Public ReadOnly Property OmniIdxKinetic() As Single()
        Get
            Return m_Transpose.OmniIdxKinetic
        End Get
    End Property

    Public ReadOnly Property UserDefValKinetic() As Single()
        Get
            Return m_Transpose.UserDefValKinetic
        End Get
    End Property


    Public ReadOnly Property TransposeCatchSumGpFlt() As Single()
        Get
            Return m_Transpose.TransposeCatchSumGpFlt
        End Get
    End Property

    Public ReadOnly Property AEFCatches() As Single()
        Get
            Return m_Transpose.AEFCatches
        End Get
    End Property

    Public ReadOnly Property OmniIdxCatches() As Single()
        Get
            Return m_Transpose.OmniIdxCatches
        End Get
    End Property

    Public ReadOnly Property UserDefValCatches() As Single()
        Get
            Return m_Transpose.UserDefValCatches
        End Get
    End Property


    Public ReadOnly Property FishLossRate() As Single()
        Get
            Return m_Transpose.FishLossRate
        End Get
    End Property

    Public ReadOnly Property AEFFishLossRate() As Single()
        Get
            Return m_Transpose.AEFFishLossRate
        End Get
    End Property

    Public ReadOnly Property OmniIdxFishLossRate() As Single()
        Get
            Return m_Transpose.OmniIdxFishLossRate
        End Get
    End Property

    Public ReadOnly Property UserDefValFishLossRate() As Single()
        Get
            Return m_Transpose.UserDefValFishLossRate
        End Get
    End Property


    Public ReadOnly Property AccessFishLossRate() As Single()
        Get
            Return m_Transpose.AccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property AEFAccessFishLossRate() As Single()
        Get
            Return m_Transpose.AEFAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property OmniIdxAccessFishLossRate() As Single()
        Get
            Return m_Transpose.OmniIdxAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property UserDefValAccessFishLossRate() As Single()
        Get
            Return m_Transpose.UserDefValAccessFishLossRate
        End Get
    End Property


    Public ReadOnly Property NaturalLossRate() As Single()
        Get
            Return m_Transpose.NaturalLossRate
        End Get
    End Property

    Public ReadOnly Property AEFNaturalLossRate() As Single()
        Get
            Return m_Transpose.AEFNaturalLossRate
        End Get
    End Property

    Public ReadOnly Property OmniIdxNaturalLossRate() As Single()
        Get
            Return m_Transpose.OmniIdxNaturalLossRate
        End Get
    End Property

    Public ReadOnly Property UserDefValNaturalLossRate() As Single()
        Get
            Return m_Transpose.UserDefValNaturalLossRate
        End Get
    End Property


    Public ReadOnly Property FishMortality() As Single()
        Get
            Return m_Transpose.FishMortality
        End Get
    End Property

    Public ReadOnly Property AEFFishMortality() As Single()
        Get
            Return m_Transpose.AEFFishMortality
        End Get
    End Property

    Public ReadOnly Property OmniIdxFishMortality() As Single()
        Get
            Return m_Transpose.OmniIdxFishMortality
        End Get
    End Property

    Public ReadOnly Property UserDefValFishMortality() As Single()
        Get
            Return m_Transpose.UserDefValFishMortality
        End Get
    End Property


    Public ReadOnly Property Selectivity() As Single()
        Get
            Return m_Transpose.Selectivity
        End Get
    End Property

    Public ReadOnly Property AEFSelectivity() As Single()
        Get
            Return m_Transpose.AEFSelectivity
        End Get
    End Property

    Public ReadOnly Property OmniIdxSelectivity() As Single()
        Get
            Return m_Transpose.OmniIdxSelectivity
        End Get
    End Property

    Public ReadOnly Property UserDefValSelectivity() As Single()
        Get
            Return m_Transpose.UserDefValSelectivity
        End Get
    End Property


    Public ReadOnly Property AccessFishMortality() As Single()
        Get
            Return m_Transpose.AccessFishMortality
        End Get
    End Property

    Public ReadOnly Property AEFAccessFishMortality() As Single()
        Get
            Return m_Transpose.AEFAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property OmniIdxAccessFishMortality() As Single()
        Get
            Return m_Transpose.OmniIdxAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property UserDefValAccessFishMortality() As Single()
        Get
            Return m_Transpose.UserDefValAccessFishMortality
        End Get
    End Property


    Public ReadOnly Property Time() As Single()
        Get
            Return m_Transpose.Time
        End Get
    End Property

    Public ReadOnly Property AEFTime() As Single()
        Get
            Return m_Transpose.AEFTime
        End Get
    End Property

    Public ReadOnly Property OmniIdxTime() As Single()
        Get
            Return m_Transpose.OmniIdxTime
        End Get
    End Property

    Public ReadOnly Property UserDefValTime() As Single()
        Get
            Return m_Transpose.UserDefValTime
        End Get
    End Property


    Public ReadOnly Property IsAEFRun() As Boolean
        Get
            Return m_Transpose.IsAEFRun
        End Get
    End Property

    Public ReadOnly Property IsOmniIdxRun() As Boolean
        Get
            Return m_Transpose.IsOmniIdxRun
        End Get
    End Property

    Public ReadOnly Property IsUserDefValRun() As Boolean
        Get
            Return m_Transpose.IsUserDefValRun
        End Get
    End Property
#End Region 'Transpose
#Region "CTSA"
    Public ReadOnly Property CTSAKinetic() As Single()
        Get
            Return m_CTSA.CTSAKinetic
        End Get
    End Property

    Public ReadOnly Property CTSANaturalLossRate() As Single()
        Get
            Return m_CTSA.CTSANaturalLossRate
        End Get
    End Property

    Public ReadOnly Property TopD() As Single()
        Get
            Return m_CTSA.TopD
        End Get
    End Property

    Public ReadOnly Property FormD() As Single()
        Get
            Return m_CTSA.FormD
        End Get
    End Property

    Public ReadOnly Property CTSASelectivity() As Single()
        Get
            Return m_CTSA.CTSASelectivity
        End Get
    End Property

    Public ReadOnly Property IsCTSAParameterRun() As Boolean
        Get
            Return m_CTSA.IsCTSAParameterRun
        End Get
    End Property

    Public ReadOnly Property FwdCalKinetic() As Single()
        Get
            Return m_CTSA.FwdCalKinetic
        End Get
    End Property

    Public ReadOnly Property FwdCalFlow() As Single()
        Get
            Return m_CTSA.FwdCalFlow
        End Get
    End Property

    Public ReadOnly Property FwdCalBiomass() As Single()
        Get
            Return m_CTSA.FwdCalBiomass
        End Get
    End Property

    Public ReadOnly Property FwdCalFishLossRate() As Single()
        Get
            Return m_CTSA.FwdCalFishLossRate
        End Get
    End Property

    Public ReadOnly Property FwdCalVirginFlow() As Single()
        Get
            Return m_CTSA.FwdCalVirginFlow
        End Get
    End Property

    Public ReadOnly Property FwdCalVirginBiomass() As Single()
        Get
            Return m_CTSA.FwdCalVirginBiomass
        End Get
    End Property

    Public ReadOnly Property FwdCalKineticRecal() As Single()
        Get
            Return m_CTSA.FwdCalKineticRecal
        End Get
    End Property

    Public ReadOnly Property FwdCalAccessBiomass() As Single()
        Get
            Return m_CTSA.FwdCalAccessBiomass
        End Get
    End Property

    Public ReadOnly Property FwdCalAccessFishLossRate() As Single()
        Get
            Return m_CTSA.FwdCalAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property FwdCalFishMortality() As Single()
        Get
            Return m_CTSA.FwdCalFishMortality
        End Get
    End Property

    Public ReadOnly Property FwdCalAccessFishMortality() As Single()
        Get
            Return m_CTSA.FwdCalAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property FwdCalSelectivity() As Single()
        Get
            Return m_CTSA.FwdCalSelectivity
        End Get
    End Property

    Public ReadOnly Property FwdCalTime() As Single()
        Get
            Return m_CTSA.FwdCalTime
        End Get
    End Property

    Public ReadOnly Property IsFwdCalRun() As Boolean
        Get
            Return m_CTSA.IsFwdCalRun
        End Get
    End Property

    Public WriteOnly Property IsFwdCalIterationContinue() As Boolean
        Set(ByVal value As Boolean)
            m_CTSA.IsFwdCalIterationContinue = value
        End Set
    End Property

    Public ReadOnly Property BwdCalKinetic() As Single()
        Get
            Return m_CTSA.BwdCalKinetic
        End Get
    End Property

    Public ReadOnly Property BwdCalFlow() As Single()
        Get
            Return m_CTSA.BwdCalFlow
        End Get
    End Property

    Public ReadOnly Property BwdCalBiomass() As Single()
        Get
            Return m_CTSA.BwdCalBiomass
        End Get
    End Property

    Public ReadOnly Property BwdCalFishLossRate() As Single()
        Get
            Return m_CTSA.BwdCalFishLossRate
        End Get
    End Property

    Public ReadOnly Property BwdCalAccessFishMortality() As Single()
        Get
            Return m_CTSA.BwdCalAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property BwdCalVirginFlow() As Single()
        Get
            Return m_CTSA.BwdCalVirginFlow
        End Get
    End Property

    Public ReadOnly Property BwdCalVirginBiomass() As Single()
        Get
            Return m_CTSA.BwdCalVirginBiomass
        End Get
    End Property

    Public ReadOnly Property BwdCalKineticRecal() As Single()
        Get
            Return m_CTSA.BwdCalKineticRecal
        End Get
    End Property

    Public ReadOnly Property BwdCalAccessBiomass() As Single()
        Get
            Return m_CTSA.BwdCalAccessBiomass
        End Get
    End Property

    Public ReadOnly Property BwdCalAccessFishLossRate() As Single()
        Get
            Return m_CTSA.BwdCalAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property BwdCalFishMortality() As Single()
        Get
            Return m_CTSA.BwdCalFishMortality
        End Get
    End Property

    Public ReadOnly Property BwdCalSelectivity() As Single()
        Get
            Return m_CTSA.BwdCalSelectivity
        End Get
    End Property

    Public ReadOnly Property BwdCalTime() As Single()
        Get
            Return m_CTSA.BwdCalTime
        End Get
    End Property

    Public ReadOnly Property IsBwdCalRun() As Boolean
        Get
            Return m_CTSA.IsBwdCalRun
        End Get
    End Property

    Public WriteOnly Property IsBwdCalIterationContinue() As Boolean
        Set(ByVal value As Boolean)
            m_CTSA.IsBwdCalIterationContinue = value
        End Set
    End Property
#End Region 'CTSA
#Region "Diagnosis"
    Public ReadOnly Property DiagnosisInputBiomass() As Single()
        Get
            Return m_Diagnosis.InputBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputAccessBiomass() As Single()
        Get
            Return m_Diagnosis.InputAccessBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputFlow() As Single()
        Get
            Return m_Diagnosis.InputFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputKinetic() As Single()
        Get
            Return m_Diagnosis.InputKinetic
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputCatches() As Single()
        Get
            Return m_Diagnosis.InputCatches
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputFishLossRate() As Single()
        Get
            Return m_Diagnosis.InputFishLossRate
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputAccessFishLossRate() As Single()
        Get
            Return m_Diagnosis.InputAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputNaturalLossRate() As Single()
        Get
            Return m_Diagnosis.InputNaturalLossRate
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputFishMortality() As Single()
        Get
            Return m_Diagnosis.InputFishMortality
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputAccessFishMortality() As Single()
        Get
            Return m_Diagnosis.InputAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputSelectivity() As Single()
        Get
            Return m_Diagnosis.InputSelectivity
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputTime() As Single()
        Get
            Return m_Diagnosis.InputTime
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputTopD() As Single()
        Get
            Return m_Diagnosis.InputTopD
        End Get
    End Property

    Public ReadOnly Property DiagnosisInputFormD() As Single()
        Get
            Return m_Diagnosis.InputFormD
        End Get
    End Property

    Public ReadOnly Property IsDiagnosisParameterRun() As Boolean
        Get
            Return m_Diagnosis.IsDiagnosisParameterRun
        End Get
    End Property

    Public ReadOnly Property DiagnosisEffortMultiplier() As Single()
        Get
            Return m_Diagnosis.EffortMultiplier
        End Get
    End Property

    Public ReadOnly Property DiagnosisKinetic() As Single(,)
        Get
            Return m_Diagnosis.Kinetic
        End Get
    End Property

    Public ReadOnly Property DiagnosisFlow() As Single(,)
        Get
            Return m_Diagnosis.Flow
        End Get
    End Property

    Public ReadOnly Property DiagnosisBiomass() As Single(,)
        Get
            Return m_Diagnosis.Biomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisAccessFlow() As Single(,)
        Get
            Return m_Diagnosis.AccessFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisAccessBiomass() As Single(,)
        Get
            Return m_Diagnosis.AccessBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisCatches() As Single(,)
        Get
            Return m_Diagnosis.Catches
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsTotalBiomass() As Single()
        Get
            Return m_Diagnosis.AbsTotalBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsVulnerBiomass() As Single()
        Get
            Return m_Diagnosis.AbsVulnerBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsPredBiomass() As Single()
        Get
            Return m_Diagnosis.AbsPredBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsTotalFlow() As Single()
        Get
            Return m_Diagnosis.AbsTotalFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsVulnerFlow() As Single()
        Get
            Return m_Diagnosis.AbsVulnerFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsPredFlow() As Single()
        Get
            Return m_Diagnosis.AbsPredFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsTotalCatch() As Single()
        Get
            Return m_Diagnosis.AbsTotalCatch
        End Get
    End Property

    Public ReadOnly Property DiagnosisAbsPredCatch() As Single()
        Get
            Return m_Diagnosis.AbsPredCatch
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelTotalBiomass() As Single()
        Get
            Return m_Diagnosis.RelTotalBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelVulnerBiomass() As Single()
        Get
            Return m_Diagnosis.RelVulnerBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelPredBiomass() As Single()
        Get
            Return m_Diagnosis.RelPredBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelTotalFlow() As Single()
        Get
            Return m_Diagnosis.RelTotalFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelVulnerFlow() As Single()
        Get
            Return m_Diagnosis.RelVulnerFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelPredFlow() As Single()
        Get
            Return m_Diagnosis.RelPredFlow
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelTotalCatch() As Single()
        Get
            Return m_Diagnosis.RelTotalCatch
        End Get
    End Property

    Public ReadOnly Property DiagnosisRelPredCatch() As Single()
        Get
            Return m_Diagnosis.RelPredCatch
        End Get
    End Property

    Public ReadOnly Property DiagnosisTLTotalBiomass() As Single()
        Get
            Return m_Diagnosis.TLTotalBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisTLVulnerBiomass() As Single()
        Get
            Return m_Diagnosis.TLVulnerBiomass
        End Get
    End Property

    Public ReadOnly Property DiagnosisTLTotalCatch() As Single()
        Get
            Return m_Diagnosis.TLTotalCatch
        End Get
    End Property

    Public ReadOnly Property IsDiagnosisRun() As Boolean
        Get
            Return m_Diagnosis.IsDiagnosisRun
        End Get
    End Property

    Public WriteOnly Property IsDiagnosisIterationContinue() As Boolean
        Set(ByVal value As Boolean)
            m_Diagnosis.IsIterationContinue = value
        End Set
    End Property
#End Region 'Diagnosis
#Region "Dynamics"
    Public ReadOnly Property DynamicsInputBiomass() As Single()
        Get
            Return m_Dynamics.InputBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsInputAccessBiomass() As Single()
        Get
            Return m_Dynamics.InputAccessBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsInputFlow() As Single()
        Get
            Return m_Dynamics.InputFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsInputKinetic() As Single()
        Get
            Return m_Dynamics.InputKinetic
        End Get
    End Property

    Public ReadOnly Property DynamicsInputCatches() As Single()
        Get
            Return m_Dynamics.InputCatches
        End Get
    End Property

    Public ReadOnly Property DynamicsInputFishLossRate() As Single()
        Get
            Return m_Dynamics.InputFishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsInputAccessFishLossRate() As Single()
        Get
            Return m_Dynamics.InputAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsInputNaturalLossRate() As Single()
        Get
            Return m_Dynamics.InputNaturalLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsInputFishMortality() As Single()
        Get
            Return m_Dynamics.InputFishMortality
        End Get
    End Property

    Public ReadOnly Property DynamicsInputAccessFishMortality() As Single()
        Get
            Return m_Dynamics.InputAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property DynamicsInputSelectivity() As Single()
        Get
            Return m_Dynamics.InputSelectivity
        End Get
    End Property

    Public ReadOnly Property DynamicsInputTime() As Single()
        Get
            Return m_Dynamics.InputTime
        End Get
    End Property

    Public ReadOnly Property DynamicsInputTopD() As Single()
        Get
            Return m_Dynamics.InputTopD
        End Get
    End Property

    Public ReadOnly Property DynamicsInputFormD() As Single()
        Get
            Return m_Dynamics.InputFormD
        End Get
    End Property

    Public ReadOnly Property IsDynamicsParameterRun() As Boolean
        Get
            Return m_Dynamics.IsDynamicsParameterRun
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpITime2() As Single()
        Get
            Return m_Dynamics.IntrpITime2
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpTL() As Single()
        Get
            Return m_Dynamics.IntrpTL
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpBiomass() As Single()
        Get
            Return m_Dynamics.IntrpBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpAccessBiomass() As Single()
        Get
            Return m_Dynamics.IntrpAccessBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpCatches() As Single()
        Get
            Return m_Dynamics.IntrpCatches
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpKinetic() As Single()
        Get
            Return m_Dynamics.IntrpKinetic
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpFlow() As Single()
        Get
            Return m_Dynamics.IntrpFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpFishLossRate() As Single()
        Get
            Return m_Dynamics.IntrpFishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpAccessFishLossRate() As Single()
        Get
            Return m_Dynamics.IntrpAccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpNaturalLossRate() As Single()
        Get
            Return m_Dynamics.IntrpNaturalLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpFishMortality() As Single()
        Get
            Return m_Dynamics.IntrpFishMortality
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpAccessFishMortality() As Single()
        Get
            Return m_Dynamics.IntrpAccessFishMortality
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpSelectivity() As Single()
        Get
            Return m_Dynamics.IntrpSelectivity
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpTopD() As Single()
        Get
            Return m_Dynamics.IntrpTopD
        End Get
    End Property

    Public ReadOnly Property DynamicsIntrpFormD() As Single()
        Get
            Return m_Dynamics.IntrpFormD
        End Get
    End Property

    Public ReadOnly Property DynamicsKinetic() As Single(,)
        Get
            Return m_Dynamics.Kinetic
        End Get
    End Property

    Public ReadOnly Property DynamicsBiomass() As Single(,)
        Get
            Return m_Dynamics.Biomass
        End Get
    End Property

    Public ReadOnly Property DynamicsFlow() As Single(,)
        Get
            Return m_Dynamics.Flow
        End Get
    End Property

    Public ReadOnly Property DynamicsAccessFlow() As Single(,)
        Get
            Return m_Dynamics.AccessFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsFishLossRate() As Single(,)
        Get
            Return m_Dynamics.FishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsCatches() As Single(,)
        Get
            Return m_Dynamics.Catches
        End Get
    End Property

    Public ReadOnly Property DynamicsCatchMultiplier() As Single()
        Get
            Return m_Dynamics.CatchMultiplier
        End Get
    End Property

    Public ReadOnly Property DynamicsFishMortality() As Single(,)
        Get
            Return m_Dynamics.FishMortality
        End Get
    End Property

    Public ReadOnly Property DynamicsEffortMultiplier() As Single()
        Get
            Return m_Dynamics.EffortMultiplier
        End Get
    End Property

    Public ReadOnly Property DynamicsAccessBiomass() As Single(,)
        Get
            Return m_Dynamics.AccessBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsAccessFishLossRate() As Single(,)
        Get
            Return m_Dynamics.AccessFishLossRate
        End Get
    End Property

    Public ReadOnly Property DynamicsKineticRecal() As Single(,)
        Get
            Return m_Dynamics.KineticRecal
        End Get
    End Property

    Public ReadOnly Property DynamicsBiomassPred() As Single(,)
        Get
            Return m_Dynamics.BiomassPred
        End Get
    End Property

    Public ReadOnly Property DynamicsSryTotalBiomass() As Single()
        Get
            Return m_Dynamics.SryTotalBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsSryAccessBiomass() As Single()
        Get
            Return m_Dynamics.SryAccessBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsSryPredBiomass() As Single()
        Get
            Return m_Dynamics.SryPredBiomass
        End Get
    End Property

    Public ReadOnly Property DynamicsSryTotalFlow() As Single()
        Get
            Return m_Dynamics.SryTotalFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsSryAccessFlow() As Single()
        Get
            Return m_Dynamics.SryAccessFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsSryPredFlow() As Single()
        Get
            Return m_Dynamics.SryPredFlow
        End Get
    End Property

    Public ReadOnly Property DynamicsSryTotalCatch() As Single()
        Get
            Return m_Dynamics.SryTotalCatch
        End Get
    End Property

    Public ReadOnly Property DynamicsSryPredCatch() As Single()
        Get
            Return m_Dynamics.SryPredCatch
        End Get
    End Property

    Public ReadOnly Property IsDynamicsRun() As Boolean
        Get
            Return m_Dynamics.IsDynamicsRun
        End Get
    End Property

    Public WriteOnly Property IsDynamicsIterationContinue() As Boolean
        Set(ByVal value As Boolean)
            m_Dynamics.IsIterationContinue = value
        End Set
    End Property
#End Region 'Dynamics
#End Region 'Outputs
#End Region 'Public Properties

#Region "Public methods"
    Public Sub RunTransposeAEF(ByVal ToolStp As ToolStrip)
        m_Transpose.EcopathData = m_EPdata
        m_Transpose.RunTransposeAEF(ToolStp)
    End Sub

    Public Sub RunTransposeOmniIdx(ByVal ToolStp As ToolStrip)
        m_Transpose.EcopathData = m_EPdata
        m_Transpose.RunTransposeOmniIdx(ToolStp)
    End Sub

    Public Sub RunTransposeUserDefVal(ByVal ToolStp As ToolStrip)
        m_Transpose.EcopathData = m_EPdata
        m_Transpose.RunTransposeUserDefVal(ToolStp)
    End Sub

    Public Sub RunTransposeAEFCatches(ByVal ToolStp As ToolStrip)
        m_Transpose.RunTransposeAEFCatches(ToolStp)
    End Sub

    Public Sub RunTransposeUserDefValCatches(ByVal ToolStp As ToolStrip)
        m_Transpose.RunTransposeUserDefValCatches(ToolStp)
    End Sub

    Public Sub RunCTSAParameter(ByVal ToolStp As ToolStrip)
        m_CTSA.EcopathData = m_EPdata
        m_CTSA.RunCTSAParameter(ToolStp)
    End Sub

    Public Sub RunCTSAFwdCal(ByVal ToolStp As ToolStrip)
        m_CTSA.EcopathData = m_EPdata
        m_CTSA.RunCTSAFwdCal(ToolStp)
    End Sub

    Public Sub RunCTSABwdCal(ByVal ToolStp As ToolStrip)
        m_CTSA.EcopathData = m_EPdata
        m_CTSA.RunCTSABwdCal(ToolStp)
    End Sub

    Public Sub RunDiagnosisParameter(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
        m_Diagnosis.EcopathData = m_EPdata
        m_Diagnosis.RunDiagnosisParameter(ToolStp, MainFrom)
    End Sub

    Public Sub RunDiagnosis(ByVal ToolStp As ToolStrip, ByVal EffortMultiplierType As String)
        m_Diagnosis.EcopathData = m_EPdata
        m_Diagnosis.RunDiagnosis(ToolStp, EffortMultiplierType)
    End Sub

    Public Sub RunDynamicsParameter(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
        m_Dynamics.EcopathData = m_EPdata
        m_Dynamics.RunDynamicsParameter(ToolStp, MainFrom)
    End Sub

    Public Sub RunDynamics(ByVal ToolStp As ToolStrip, ByVal CatchHistoryType As String, Optional ByVal CatchPastAnalysisFilePath As String = "")
        m_Dynamics.EcopathData = m_EPdata
        If CatchPastAnalysisFilePath = "" Then
            m_Dynamics.RunDynamics(ToolStp, CatchHistoryType)
        Else
            m_Dynamics.RunDynamics(ToolStp, CatchHistoryType, CatchPastAnalysisFilePath)
        End If
    End Sub

    Public Sub RunDynamicsCatches(ByVal ToolStp As ToolStrip, ByVal MainFrom As String)
        m_Dynamics.EcopathData = m_EPdata
        m_Dynamics.RunDynamicsCatches(ToolStp, MainFrom)
    End Sub
#End Region 'Public methods

#Region "Friend methods"
    Friend Sub UpdatePrgrsRunTranspose(ByVal ToolStp As ToolStrip, ByVal BarMax As Integer)
        RaiseEvent RunTransposePrgrs(ToolStp, BarMax)
    End Sub
#End Region 'Friend methods

#Region "Private events"
    Private Sub m_CTSA_FwdCalInformIterationInfo(ByVal KineticCriteria As Double) Handles m_CTSA.FwdCalInformIterationInfo
        RaiseEvent CTSAFwdCalIterationInfo(KineticCriteria)
    End Sub

    Private Sub m_CTSA_BwdCalInformIterationInfo(ByVal KineticCriteria As Double) Handles m_CTSA.BwdCalInformIterationInfo
        RaiseEvent CTSABwdCalIterationInfo(KineticCriteria)
    End Sub

    Private Sub m_Diagnosis_InformIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double) Handles m_Diagnosis.InformIterationInfo
        RaiseEvent DiagnosisIterationInfo(KineticCriteria, FlowCriteria)
    End Sub

    Private Sub m_Dynamics_InformIterationInfo(ByVal KineticCriteria As Double, ByVal FlowCriteria As Double) Handles m_Dynamics.InformIterationInfo
        RaiseEvent DynamicsIterationInfo(KineticCriteria, FlowCriteria)
    End Sub

    Private Sub m_Dynamics_InformCatchPastAnalysisErr() Handles m_Dynamics.InformCatchPastAnalysisErr
        RaiseEvent CatchPastAnalysisErr()
    End Sub
#End Region 'Private events

End Class
