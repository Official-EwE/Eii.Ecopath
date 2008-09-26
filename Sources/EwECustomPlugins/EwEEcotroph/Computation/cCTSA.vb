'==============================================================================
'
' $Log: cCTSA.vb,v $
' Revision 1.1  2008/09/26 07:30:36  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.50  2008/06/05 19:02:25  joeh
' no message
'
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports System.Xml

Namespace Computation

    Public Class cCTSA

#Region "Private events"
        Public Event FwdCalInformIterationInfo(ByVal KineticCriteria As Double)
        Public Event BwdCalInformIterationInfo(ByVal KineticCriteria As Double)
#End Region 'Private events

#Region "Private fields"
        Private Const TL_OUT_INIT As Double = 2.0
        Private Const TL_OUT_FINAL As Double = 7.0
        Private Const TL_INCRM As Double = 0.1
        Private Const PRGRS_BAR_MAX As Integer = 10

        Private m_EcotrophManager As cEcotrophManager
        Private m_EPdata As cEcopathDataStructures

        Private m_FwdCalFlowTL1 As Single
        Private m_FwdCalFlowTL2 As Single
        Private m_FwdCalBiomassTL1 As Single
        Private m_FwdCalBiomassTL2 As Single

        Private m_BwdCalAccessFishMortalityTTL As Single
        Private m_BwdCalFishLossRateTTL As Single
        Private m_BwdCalFlowTTL As Single
        Private m_BwdCalBiomassTTL As Single
#End Region 'Private fields

#Region "Public fields"
        Public CTSAKinetic() As Single
        Public CTSANaturalLossRate() As Single
        Public TopD() As Single
        Public FormD() As Single
        Public CTSASelectivity() As Single
        Public IsCTSAParameterRun As Boolean

        Public FwdCalKinetic() As Single
        Public FwdCalFlow() As Single
        Public FwdCalBiomass() As Single
        Public FwdCalFishLossRate() As Single
        Public FwdCalVirginFlow() As Single
        Public FwdCalVirginBiomass() As Single
        Public FwdCalKineticRecal() As Single
        Public FwdCalAccessBiomass() As Single
        Public FwdCalAccessFishLossRate() As Single
        Public FwdCalFishMortality() As Single
        Public FwdCalAccessFishMortality() As Single
        Public FwdCalSelectivity() As Single
        Public FwdCalTime() As Single
        Public IsFwdCalRun As Boolean
        Public IsFwdCalIterationContinue As Boolean

        Public BwdCalKinetic() As Single
        Public BwdCalFlow() As Single
        Public BwdCalBiomass() As Single
        Public BwdCalFishLossRate() As Single
        Public BwdCalAccessFishMortality() As Single
        Public BwdCalVirginFlow() As Single
        Public BwdCalVirginBiomass() As Single
        Public BwdCalKineticRecal() As Single
        Public BwdCalAccessBiomass() As Single
        Public BwdCalAccessFishLossRate() As Single
        Public BwdCalFishMortality() As Single
        Public BwdCalSelectivity() As Single
        Public BwdCalTime() As Single
        Public IsBwdCalRun As Boolean
        Public IsBwdCalIterationContinue As Boolean
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
        Public Sub RunCTSAParameter(ByVal ToolStp As ToolStrip)
            IsCTSAParameterRun = False
            m_EcotrophManager.InputData.ReadFile("CTSAParameter", m_EcotrophManager)
            m_EcotrophManager.InputData.ReadFile("KineticParameter", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindCTSAKineticAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindCTSANaturalLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTopDAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFormDAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindCTSASelectivityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsCTSAParameterRun = True
        End Sub

        Public Sub RunCTSAFwdCal(ByVal ToolStp As ToolStrip)
            Dim NumKineticIteration As Integer
            Dim KineticCriteria As Double

            IsFwdCalRun = False
            m_EcotrophManager.InputData.ReadFile("CTSAFwdCalParameter", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindFwdCalKineticAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            NumKineticIteration = 1
            IsFwdCalIterationContinue = True
            Do
                FindFwdCalSeeds()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalFlowAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalBiomassAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalFishLossRateAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalVirginFlowAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalVirginBiomassAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindFwdCalKineticRecalAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

                If (NumKineticIteration Mod 500) = 0 Then RaiseEvent FwdCalInformIterationInfo(KineticCriteria)
                NumKineticIteration = NumKineticIteration + 1
                If IsFwdCalIterationContinue = False Then Exit Do
            Loop Until FwdCalKineticStablisationFn(KineticCriteria)
            FindFwdCalAccessBiomassAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFwdCalAccessFishLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFwdCalFishMortalityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFwdCalAccessFishMortalityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFwdCalSelectivityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFwdCalTimeAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsFwdCalRun = True
        End Sub

        Public Sub RunCTSABwdCal(ByVal ToolStp As ToolStrip)
            Dim NumKineticIteration As Integer
            Dim KineticCriteria As Double
            'Dim IsIterated As Boolean

            IsBwdCalRun = False
            m_EcotrophManager.InputData.ReadFile("CTSABwdCalParameter", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindBwdCalKineticAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            'IsIterated = False
            NumKineticIteration = 1
            IsBwdCalIterationContinue = True
            Do
                'FindBwdCalSeeds(IsIterated)
                FindBwdCalSeeds()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                'Do
                FindBwdCalFlowAryTLLeTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalBiomassAryTLLeTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalFishLossRateAryTLLeTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalAccessFishMortalityAryTLLeTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalVirginFlowAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalVirginBiomassAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalAccessFishMortalityAryTLGtTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalFishLossRateAryTLGtTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalFlowAryTLGtTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalBiomassAryTLGtTTL()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
                FindBwdCalKineticRecalAry()
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

                If (NumKineticIteration Mod 500) = 0 Then RaiseEvent BwdCalInformIterationInfo(KineticCriteria)
                NumKineticIteration = NumKineticIteration + 1
                If IsBwdCalIterationContinue = False Then Exit Do
            Loop Until BwdCalKineticStablisationFn(KineticCriteria)
            'Loop Until BwdCalAccessFishMortalityStablisationFn(IsIterated)
            FindBwdCalAccessBiomassAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindBwdCalAccessFishLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindBwdCalFishMortalityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            'FindBwdCalAccessFishMortalityAry()
            'm_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindBwdCalSelectivityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindBwdCalTimeAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsBwdCalRun = True
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Sub FindCTSAKineticAry()
            Dim Idx As Integer
            Dim IdxMax As Integer
            Dim TLOut As Double

            IdxMax = 1
            For TLIn As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                IdxMax = IdxMax + 1
            Next
            ReDim CTSAKinetic(IdxMax)

            Idx = 1
            TLOut = 1
            CTSAKinetic(Idx) = CTSAKineticFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                CTSAKinetic(Idx) = CTSAKineticFn(TLOut)
            Next
        End Sub

        Private Function CTSAKineticFn(ByVal TrophicLevelOut As Double) As Single
            'Return CSng(m_EcotrophManager.InputData.KineticParameter(1) * Math.Exp( _
            '  -m_EcotrophManager.InputData.KineticParameter(2) * TrophicLevelOut) _
            '  + m_EcotrophManager.InputData.KineticParameter(3) * m_EcotrophManager.InputData.WaterTemp)
            Return CSng(m_EcotrophManager.InputData.KineticParameter(1) * _
            Math.Pow(TrophicLevelOut, m_EcotrophManager.InputData.KineticParameter(2)) * _
            Math.Exp(m_EcotrophManager.InputData.KineticParameter(3) * m_EcotrophManager.InputData.WaterTemp))
        End Function

        Private Sub FindCTSANaturalLossRateAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim CTSANaturalLossRate(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            CTSANaturalLossRate(Idx) = CTSANaturalLossRateFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                CTSANaturalLossRate(Idx) = CTSANaturalLossRateFn(TLOut)
            Next
            'CTSANaturalLossRate(1) = CTSANaturalLossRateFn(m_EcotrophManager.InputData.TETL12)

            'For Idx As Integer = 2 To CTSAKinetic.GetUpperBound(0)
            '    CTSANaturalLossRate(Idx) = CTSANaturalLossRateFn(m_EcotrophManager.InputData.TETL2)
            'Next
        End Sub

        Private Function CTSANaturalLossRateFn(ByVal TrophicLevelOut As Double) As Single 'ByVal TrophicEfficiency As Single) As Single
            Select Case TrophicLevelOut
                Case Is < 2
                    Return CSng(-Math.Log(m_EcotrophManager.InputData.TETL12 / 100.0))
                Case Else
                    Return CSng(-Math.Log(m_EcotrophManager.InputData.TETL2 / 100.0))
            End Select
            'Return CSng(-Math.Log(TrophicEfficiency / 100.0))
        End Function

        Private Sub FindTopDAry()
            ReDim TopD(CTSAKinetic.GetUpperBound(0))

            For Idx As Integer = 1 To CTSAKinetic.GetUpperBound(0)
                TopD(Idx) = TopDFn(Idx)
            Next
        End Sub

        Private Function TopDFn(ByVal Idx As Integer) As Single
            Return m_EcotrophManager.InputData.CTSATopD(Idx)
        End Function

        Private Sub FindFormDAry()
            ReDim FormD(CTSAKinetic.GetUpperBound(0))

            For Idx As Integer = 1 To CTSAKinetic.GetUpperBound(0)
                FormD(Idx) = FormDFn(Idx)
            Next
        End Sub

        Private Function FormDFn(ByVal Idx As Integer) As Single
            Return m_EcotrophManager.InputData.CTSAFormD(Idx)
        End Function

        Private Sub FindCTSASelectivityAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim CTSASelectivity(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            CTSASelectivity(Idx) = CTSASelectivityFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                CTSASelectivity(Idx) = CTSASelectivityFn(TLOut)
            Next
        End Sub

        Private Function CTSASelectivityFn(ByVal TrophicLevelOut As Double) As Single
            Return CSng(m_EcotrophManager.InputData.Asymptote / (1 + Math.Exp(-m_EcotrophManager.InputData.Slope _
              * (TrophicLevelOut - m_EcotrophManager.InputData.TL50))))
        End Function

        Private Sub FindFwdCalKineticAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FwdCalKinetic(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FwdCalKinetic(Idx) = CTSAKineticFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalKinetic(Idx) = CTSAKineticFn(TLOut)
            Next
        End Sub

        Private Sub FindFwdCalSeeds()
            Select Case m_EcotrophManager.InputData.SeedNameFwdCal
                Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL1
                    m_FwdCalBiomassTL1 = m_EcotrophManager.InputData.SeedValueFwdCal
                    m_FwdCalFlowTL1 = m_FwdCalBiomassTL1 * FwdCalKinetic(1)
                    m_FwdCalFlowTL2 = CSng(m_FwdCalFlowTL1 / Math.Exp(CTSANaturalLossRateFn(1)))
                    m_FwdCalBiomassTL2 = m_FwdCalFlowTL2 / FwdCalKinetic(2)
                Case My.Resources.DROP_DWN_LST_ITM_PROD_TL1
                    m_FwdCalFlowTL1 = m_EcotrophManager.InputData.SeedValueFwdCal
                    m_FwdCalFlowTL2 = CSng(m_FwdCalFlowTL1 / Math.Exp(CTSANaturalLossRateFn(1)))
                    m_FwdCalBiomassTL2 = m_FwdCalFlowTL2 / FwdCalKinetic(2)
                    m_FwdCalBiomassTL1 = m_FwdCalFlowTL1 / FwdCalKinetic(1)
                Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL2
                    m_FwdCalBiomassTL2 = m_EcotrophManager.InputData.SeedValueFwdCal
                    m_FwdCalFlowTL2 = FwdCalKinetic(2) * m_FwdCalBiomassTL2
                    m_FwdCalFlowTL1 = CSng(m_FwdCalFlowTL2 * Math.Exp(CTSANaturalLossRateFn(1)))
                    m_FwdCalBiomassTL1 = m_FwdCalFlowTL1 / FwdCalKinetic(1)
                Case My.Resources.DROP_DWN_LST_ITM_PROD_TL2
                    m_FwdCalFlowTL2 = m_EcotrophManager.InputData.SeedValueFwdCal
                    m_FwdCalFlowTL1 = CSng(m_FwdCalFlowTL2 * Math.Exp(CTSANaturalLossRateFn(1)))
                    m_FwdCalBiomassTL1 = m_FwdCalFlowTL1 / FwdCalKinetic(1)
                    m_FwdCalBiomassTL2 = m_FwdCalFlowTL2 / FwdCalKinetic(2)
            End Select
        End Sub

        Private Sub FindFwdCalFlowAry()
            Dim Idx As Integer
            ReDim FwdCalFlow(CTSAKinetic.GetUpperBound(0))

            FwdCalFlow(1) = m_FwdCalFlowTL1
            FwdCalFlow(2) = m_FwdCalFlowTL2

            Idx = 3
            For TLOut As Double = TL_OUT_INIT + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                FwdCalFlow(Idx) = FwdCalFlowFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function FwdCalFlowFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double
            Dim AryIdxPrevious As Integer

            Debug.Assert(TrophicLevelOut > 2)
            TLOutPrevious = TrophicLevelOut - TL_INCRM
            AryIdxPrevious = AryIdx - 1
            Return CSng(FwdCalFlow(AryIdxPrevious) * Math.Exp(-CTSANaturalLossRateFn(TLOutPrevious) * TL_INCRM) - _
              m_EcotrophManager.InputData.Catches(AryIdxPrevious) * TL_INCRM * _
              Math.Exp(-CTSANaturalLossRateFn(TLOutPrevious) * TL_INCRM / 2))
        End Function

        Private Sub FindFwdCalBiomassAry()
            Dim Idx As Integer
            ReDim FwdCalBiomass(CTSAKinetic.GetUpperBound(0))

            FwdCalBiomass(1) = m_FwdCalBiomassTL1
            FwdCalBiomass(2) = m_FwdCalBiomassTL2

            Idx = 3
            For TLOut As Double = TL_OUT_INIT + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                FwdCalBiomass(Idx) = FwdCalBiomassFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function FwdCalBiomassFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Debug.Assert(TrophicLevelOut > 2)
            'Return FwdCalFlow(AryIdx) / FwdCalKinetic(AryIdx)
            Return FwdCalFlowFn(TrophicLevelOut, AryIdx) / FwdCalKinetic(AryIdx)
        End Function

        Private Sub FindFwdCalFishLossRateAry()
            Dim Idx As Integer
            ReDim FwdCalFishLossRate(CTSAKinetic.GetUpperBound(0))

            FwdCalFishLossRate(1) = CSng(Math.Log(FwdCalFlow(1) / FwdCalFlow(2)) / (2 - 1) - _
              CTSANaturalLossRateFn(1))
            FwdCalFishLossRate(2) = CSng(Math.Log(FwdCalFlow(2) / FwdCalFlowFn(2.1, 3)) / (2.1 - 2) - _
              CTSANaturalLossRateFn(2))

            Idx = 3
            For TLOut As Double = TL_OUT_INIT + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                FwdCalFishLossRate(Idx) = FwdCalFishLossRateFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function FwdCalFishLossRateFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutNext As Double
            Dim AryIdxNext As Integer

            Debug.Assert(TrophicLevelOut > 2)
            TLOutNext = TrophicLevelOut + TL_INCRM
            AryIdxNext = AryIdx + 1
            Return CSng(Math.Log(FwdCalFlowFn(TrophicLevelOut, AryIdx) / FwdCalFlowFn(TLOutNext, AryIdxNext)) / TL_INCRM - _
              CTSANaturalLossRateFn(TrophicLevelOut))
        End Function

        Private Sub FindFwdCalVirginFlowAry()
            Dim Idx As Integer
            ReDim FwdCalVirginFlow(CTSAKinetic.GetUpperBound(0))

            FwdCalVirginFlow(1) = FwdCalFlow(1)

            Idx = 2
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                FwdCalVirginFlow(Idx) = FwdCalVirginFlowFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function FwdCalVirginFlowFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double
            Dim AryIdxPrevious As Integer

            Debug.Assert(TrophicLevelOut > 1)
            Select Case TrophicLevelOut
                Case 2
                    TLOutPrevious = 1
                Case Is > 2
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
            End Select
            AryIdxPrevious = AryIdx - 1
            Return CSng(FwdCalVirginFlow(AryIdxPrevious) * Math.Exp(-CTSANaturalLossRateFn(TLOutPrevious) * _
              (TrophicLevelOut - TLOutPrevious)))
        End Function

        Private Sub FindFwdCalVirginBiomassAry()
            Dim Idx As Integer
            ReDim FwdCalVirginBiomass(CTSAKinetic.GetUpperBound(0))

            'FwdCalVirginBiomass(1) = FwdCalVirginFlow(1) / FwdCalKinetic(1)
            FwdCalVirginBiomass(1) = FwdCalVirginFlow(1) / CTSAKineticFn(1)

            Idx = 2
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                FwdCalVirginBiomass(Idx) = FwdCalVirginBiomassFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function FwdCalVirginBiomassFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Debug.Assert(TrophicLevelOut > 1)
            'Return FwdCalVirginFlow(AryIdx) / FwdCalKinetic(AryIdx)
            Return FwdCalVirginFlowFn(TrophicLevelOut, AryIdx) / CTSAKineticFn(TrophicLevelOut)
        End Function

        Private Sub FindFwdCalKineticRecalAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FwdCalKineticRecal(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FwdCalKineticRecal(Idx) = FwdCalKineticRecalFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalKineticRecal(Idx) = FwdCalKineticRecalFn(TLOut)
            Next
        End Sub

        Private Function FwdCalKineticRecalFn(ByVal TrophicLevelOut As Double) As Single
            Dim AryIdx As Integer
            Dim TLOut As Double
            Dim TLOutFinal As Double
            Dim FwdCalBiomassTmp As Single
            Dim SumFwdCalBiomass As Double
            Dim SumFwdCalVirginBiomass As Double
            Dim FwdCalFishLossRateTmp As Single

            Dim SumTL As Double
            Dim SumLogFwdCalKineticRecal As Double
            Dim AvgTL As Double
            Dim AvgLogFwdCalKineticRecal As Double
            Dim SumTLDevLogFwdCalKineticRecalDev As Double
            Dim SumTLDevSquare As Double
            Dim Slope As Double
            Dim Intercept As Double

            Select Case TrophicLevelOut
                Case 1
                    SumFwdCalBiomass = 0.0
                    SumFwdCalVirginBiomass = 0.0
                    For TLOut = TL_OUT_INIT To 2.5 Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        If TLOut > 2 Then
                            FwdCalBiomassTmp = FwdCalBiomassFn(TLOut, AryIdx)
                        Else '=2
                            FwdCalBiomassTmp = FwdCalBiomass(2)
                        End If
                        SumFwdCalBiomass = SumFwdCalBiomass + FwdCalBiomassTmp
                        SumFwdCalVirginBiomass = SumFwdCalVirginBiomass + FwdCalVirginBiomassFn(TLOut, AryIdx)
                    Next
                    AryIdx = 1 'CInt((Int(TrophicLevelOut) - 2) * 10 + CInt((TrophicLevelOut - Int(TrophicLevelOut)) * 10) + 2)
                    Return CSng(CTSAKineticFn(TrophicLevelOut) * (1 + m_EcotrophManager.InputData.CTSATopD(AryIdx) * _
                      ((Math.Pow(SumFwdCalBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)) - Math.Pow(SumFwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx))) / _
                      Math.Pow(SumFwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)))) + FwdCalFishLossRate(1) * FwdCalKinetic(1))
                Case 2 To 5.8
                    If TrophicLevelOut < 5.79 Then
                        TLOutFinal = TrophicLevelOut + 1.3 'CSng(TrophicLevelOut + 1.3)
                    Else '=5.8
                        TLOutFinal = TrophicLevelOut + 1.2 'CSng(TrophicLevelOut + 1.2)
                    End If
                    SumFwdCalBiomass = 0.0
                    SumFwdCalVirginBiomass = 0.0
                    For TLOut = (TrophicLevelOut + 0.8) To TLOutFinal Step TL_INCRM 'CSng(TrophicLevelOut + 0.8) To TLOutFinal Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        SumFwdCalBiomass = SumFwdCalBiomass + FwdCalBiomassFn(TLOut, AryIdx)
                        SumFwdCalVirginBiomass = SumFwdCalVirginBiomass + FwdCalVirginBiomassFn(TLOut, AryIdx)
                    Next
                    AryIdx = CInt((Int(TrophicLevelOut) - 2) * 10 + CInt((TrophicLevelOut - Int(TrophicLevelOut)) * 10) + 2)
                    If TrophicLevelOut > 2 Then
                        FwdCalFishLossRateTmp = FwdCalFishLossRateFn(TrophicLevelOut, AryIdx)
                    Else '=2
                        FwdCalFishLossRateTmp = FwdCalFishLossRate(2)
                    End If
                    Return CSng(CTSAKineticFn(TrophicLevelOut) * (1 + m_EcotrophManager.InputData.CTSATopD(AryIdx) * _
                      ((Math.Pow(SumFwdCalBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)) - Math.Pow(SumFwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx))) / _
                      Math.Pow(SumFwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)))) + FwdCalFishLossRateTmp * FwdCalKinetic(AryIdx))
                Case 5.89 To 7 ' 5.9 to 7
                    SumTL = 0.0
                    SumLogFwdCalKineticRecal = 0.0
                    For TLOut = 5 To 5.8 Step TL_INCRM
                        SumTL = SumTL + TLOut
                        SumLogFwdCalKineticRecal = SumLogFwdCalKineticRecal + LogFwdCalKineticRecalFn(TLOut)
                    Next
                    AvgTL = SumTL / 9.0
                    AvgLogFwdCalKineticRecal = SumLogFwdCalKineticRecal / 9.0
                    SumTLDevLogFwdCalKineticRecalDev = 0.0
                    SumTLDevSquare = 0.0
                    For TLOut = 5 To 5.8 Step TL_INCRM
                        SumTLDevLogFwdCalKineticRecalDev = SumTLDevLogFwdCalKineticRecalDev + (TLOut - AvgTL) * _
                          (LogFwdCalKineticRecalFn(TLOut) - AvgLogFwdCalKineticRecal)
                        SumTLDevSquare = SumTLDevSquare + (TLOut - AvgTL) * (TLOut - AvgTL)
                    Next
                    Slope = SumTLDevLogFwdCalKineticRecalDev / SumTLDevSquare
                    Intercept = AvgLogFwdCalKineticRecal - Slope * AvgTL
                    Return CSng(Math.Exp(Intercept + Slope * TrophicLevelOut))
            End Select
        End Function

        Private Function LogFwdCalKineticRecalFn(ByVal TrophicLevelOut As Double) As Single
            'Return CSng(Math.Log(CTSAKineticFn(TrophicLevelOut)))
            Return CSng(Math.Log(FwdCalKineticRecalFn(TrophicLevelOut)))
        End Function

        Private Function FwdCalKineticStablisationFn(ByRef KineticCriteria As Double) As Boolean
            Dim SumFwdCalKineticRecal As Double
            Dim SumFwdCalKinetic As Double

            SumFwdCalKineticRecal = 0
            For Idx As Integer = 2 To FwdCalKineticRecal.GetUpperBound(0)
                SumFwdCalKineticRecal = SumFwdCalKineticRecal + FwdCalKineticRecal(Idx)
            Next
            SumFwdCalKinetic = 0
            For Idx As Integer = 2 To FwdCalKinetic.GetUpperBound(0)
                SumFwdCalKinetic = SumFwdCalKinetic + FwdCalKinetic(Idx)
            Next
            If Math.Abs(SumFwdCalKineticRecal - SumFwdCalKinetic) > 0.000001 Then
                For Idx As Integer = 1 To FwdCalKineticRecal.GetUpperBound(0)
                    FwdCalKinetic(Idx) = FwdCalKineticRecal(Idx)
                Next
                KineticCriteria = SumFwdCalKineticRecal - SumFwdCalKinetic
                Return False
            Else
                KineticCriteria = SumFwdCalKineticRecal - SumFwdCalKinetic
                Return True
            End If
        End Function

        Private Sub FindFwdCalAccessBiomassAry()
            Dim Idx As Integer
            ReDim FwdCalAccessBiomass(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            FwdCalAccessBiomass(Idx) = FwdCalBiomass(Idx) * CTSASelectivity(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalAccessBiomass(Idx) = FwdCalBiomass(Idx) * CTSASelectivity(Idx)
            Next
        End Sub

        Private Sub FindFwdCalAccessFishLossRateAry()
            Dim Idx As Integer
            ReDim FwdCalAccessFishLossRate(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            FwdCalAccessFishLossRate(Idx) = m_EcotrophManager.InputData.Catches(Idx) / FwdCalAccessBiomass(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalAccessFishLossRate(Idx) = m_EcotrophManager.InputData.Catches(Idx) / FwdCalAccessBiomass(Idx)
            Next
        End Sub

        Private Sub FindFwdCalFishMortalityAry()
            Dim Idx As Integer
            ReDim FwdCalFishMortality(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            FwdCalFishMortality(Idx) = FwdCalFishLossRate(Idx) * FwdCalKinetic(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalFishMortality(Idx) = FwdCalFishLossRate(Idx) * FwdCalKinetic(Idx)
            Next
        End Sub

        Private Sub FindFwdCalAccessFishMortalityAry()
            Dim Idx As Integer
            ReDim FwdCalAccessFishMortality(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            FwdCalAccessFishMortality(Idx) = FwdCalAccessFishLossRate(Idx) * FwdCalKinetic(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalAccessFishMortality(Idx) = FwdCalAccessFishLossRate(Idx) * FwdCalKinetic(Idx)
            Next
        End Sub

        Private Sub FindFwdCalSelectivityAry()
            Dim Idx As Integer
            ReDim FwdCalSelectivity(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            FwdCalSelectivity(Idx) = FwdCalAccessBiomass(Idx) / FwdCalBiomass(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FwdCalSelectivity(Idx) = FwdCalAccessBiomass(Idx) / FwdCalBiomass(Idx)
            Next
        End Sub

        Private Sub FindFwdCalTimeAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FwdCalTime(CTSAKinetic.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            FwdCalTime(Idx) = FwdCalTimeFn(TLOut, Idx)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                FwdCalTime(Idx) = FwdCalTimeFn(TLOut, Idx)
            Next
        End Sub

        Private Function FwdCalTimeFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double

            Select Case TrophicLevelOut
                Case 1
                    Return 0
                Case 2
                    TLOutPrevious = 1
                Case Is > 2
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
            End Select
            Return CSng(FwdCalTime(AryIdx - 1) + ((TrophicLevelOut - TLOutPrevious) / FwdCalKinetic(AryIdx - 1)))
        End Function

        Private Sub FindBwdCalKineticAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalKinetic(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            BwdCalKinetic(Idx) = CTSAKineticFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalKinetic(Idx) = CTSAKineticFn(TLOut)
            Next
        End Sub

        'Private Sub FindBwdCalSeeds(ByVal IsIterated As Boolean)
        Private Sub FindBwdCalSeeds()
            Dim TLOut As Double
            Dim AryIdx As Integer

            'If IsIterated = False Then m_BwdCalAccessFishMortalityTTL = 0.1
            TLOut = m_EcotrophManager.InputData.TTL
            AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            Select Case m_EcotrophManager.InputData.SeedNameBwdCal
                Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                    m_BwdCalAccessFishMortalityTTL = m_EcotrophManager.InputData.SeedValueBwdCal '0.15
                    m_BwdCalFishLossRateTTL = m_BwdCalAccessFishMortalityTTL * CTSASelectivityFn(TLOut) / _
                       BwdCalKinetic(AryIdx)
                Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                    m_BwdCalFishLossRateTTL = m_EcotrophManager.InputData.SeedValueBwdCal
                    m_BwdCalAccessFishMortalityTTL = m_BwdCalFishLossRateTTL * BwdCalKinetic(AryIdx) / _
                      CTSASelectivityFn(TLOut)
            End Select

            m_BwdCalFlowTTL = CSng((m_EcotrophManager.InputData.Catches(AryIdx) * (m_BwdCalFishLossRateTTL + CTSANaturalLossRateFn(TLOut)) * 0.1) / _
              (m_BwdCalFishLossRateTTL * (1 - Math.Exp(-(m_BwdCalFishLossRateTTL + CTSANaturalLossRateFn(TLOut)) * 0.1))))
            m_BwdCalBiomassTTL = m_BwdCalFlowTTL / BwdCalKinetic(AryIdx)
        End Sub

        Private Sub FindBwdCalFlowAryTLLeTTL()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalFlow(CTSAKinetic.GetUpperBound(0))

            'BwdCalFlow when 2<=TLOut<=TTL
            For TLOut = m_EcotrophManager.InputData.TTL To TL_OUT_INIT Step -TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalFlow(Idx) = BwdCalFlowFn(TLOut, Idx)
            Next

            'BwdCalFlow when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalFlow(Idx) = BwdCalFlowFn(TLOut, Idx)

            'BwdCalFlow when TLOut>TTL
            'For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
            '    Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            '    BwdCalFlow(Idx) = BwdCalFlowFn(TLOut, Idx)
            'Next
        End Sub

        Private Function BwdCalFlowFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            'Dim TLOutNext As Double
            Dim TLOutPrevious As Double
            Dim TLOutIncrement As Double
            Dim AryIdxNext As Integer

            Dim AccessFishMortality As Single
            Dim FishLossRate As Single

            'Debug.Assert(TrophicLevelOut > 2)
            Select Case TrophicLevelOut
                Case 1.0
                    TLOutIncrement = 1.0
                    'TLOutNext = TrophicLevelOut + TLOutIncrement
                    AryIdxNext = AryIdx + 1
                    Return CSng(BwdCalFlow(AryIdxNext) * Math.Exp(CTSANaturalLossRateFn(TrophicLevelOut) * TLOutIncrement) + _
                      m_EcotrophManager.InputData.Catches(AryIdx) * TLOutIncrement * _
                      Math.Exp(CTSANaturalLossRateFn(TrophicLevelOut) * TLOutIncrement / 2))
                Case 2 To m_EcotrophManager.InputData.TTL - TL_INCRM + CSng(0.01) '2 to TTL-0.1 inclusive
                    TLOutIncrement = TL_INCRM
                    'TLOutNext = TrophicLevelOut + TLOutIncrement
                    AryIdxNext = AryIdx + 1
                    Return CSng(BwdCalFlow(AryIdxNext) * Math.Exp(CTSANaturalLossRateFn(TrophicLevelOut) * TLOutIncrement) + _
                      m_EcotrophManager.InputData.Catches(AryIdx) * TLOutIncrement * _
                      Math.Exp(CTSANaturalLossRateFn(TrophicLevelOut) * TLOutIncrement / 2))
                Case m_EcotrophManager.InputData.TTL
                    Return m_BwdCalFlowTTL
                Case Is > m_EcotrophManager.InputData.TTL
                    'AccessFishMortality = m_BwdCalAccessFishMortalityTTL
                    'FishLossRate = AccessFishMortality * CTSASelectivityFn(TrophicLevelOut) * BwdCalKinetic(AryIdx)
                    AccessFishMortality = 0.0
                    FishLossRate = AccessFishMortality * CTSASelectivityFn(TrophicLevelOut) / BwdCalKinetic(AryIdx)
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
                    Return CSng(BwdCalFlowFn(TLOutPrevious, AryIdx) * Math.Exp(-(CTSANaturalLossRateFn(TrophicLevelOut) + FishLossRate) * TL_INCRM))
            End Select
        End Function

        Private Sub FindBwdCalBiomassAryTLLeTTL()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalBiomass(CTSAKinetic.GetUpperBound(0))

            'BwdCalBiomass when 2<=TLOut<=TTL
            For TLOut = m_EcotrophManager.InputData.TTL To TL_OUT_INIT Step -TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalBiomass(Idx) = BwdCalBiomassFn(TLOut, Idx)
            Next

            'BwdCalBiomass when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalBiomass(Idx) = BwdCalBiomassFn(TLOut, Idx)

            'BwdCalBiomass when TLOut>TTL
            'For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
            '    Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            '    BwdCalBiomass(Idx) = BwdCalBiomassFn(TLOut, Idx)
            'Next
        End Sub

        Private Function BwdCalBiomassFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            'Debug.Assert(TrophicLevelOut > 2)
            Select Case TrophicLevelOut
                Case 1, 2 To m_EcotrophManager.InputData.TTL - TL_INCRM + CSng(0.01), Is > m_EcotrophManager.InputData.TTL '2 to TTL-0.1 inclusive
                    'Return BwdCalFlow(AryIdx) / BwdCalKinetic(AryIdx)
                    Return BwdCalFlowFn(TrophicLevelOut, AryIdx) / BwdCalKinetic(AryIdx)
                Case m_EcotrophManager.InputData.TTL
                    Return m_BwdCalBiomassTTL
            End Select
        End Function

        Private Sub FindBwdCalFishLossRateAryTLLeTTL()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalFishLossRate(CTSAKinetic.GetUpperBound(0))

            'BwdCalFishLossRate when 2<=TLOut<=TTL
            For TLOut = m_EcotrophManager.InputData.TTL To TL_OUT_INIT Step -TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalFishLossRate(Idx) = BwdCalFishLossRateFn(TLOut, Idx)
            Next

            'BwdCalFishLossRate when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalFishLossRate(Idx) = BwdCalFishLossRateFn(TLOut, Idx)

            'BwdCalFishLossRate when TLOut>TTL
            'For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
            '    Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            '    BwdCalFishLossRate(Idx) = BwdCalFishLossRateFn(TLOut, Idx)
            'Next
        End Sub

        Private Function BwdCalFishLossRateFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutNext As Double
            Dim TLOutIncrement As Double
            Dim AryIdxNext As Integer

            Dim AccessFishMortality As Single

            'Debug.Assert(TrophicLevelOut > 2)
            Select Case TrophicLevelOut
                Case 1
                    TLOutIncrement = 1.0
                    TLOutNext = TrophicLevelOut + TLOutIncrement
                    AryIdxNext = AryIdx + 1
                    Return CSng(Math.Log(BwdCalFlowFn(TrophicLevelOut, AryIdx) / BwdCalFlowFn(TLOutNext, AryIdxNext)) / TLOutIncrement - _
                      CTSANaturalLossRateFn(TrophicLevelOut))
                    'Return CSng(Math.Log(BwdCalFlow(AryIdx) / BwdCalFlow(AryIdxNext)) / TLOutIncrement - _
                    ' CTSANaturalLossRate(AryIdx))
                Case 2 To m_EcotrophManager.InputData.TTL - TL_INCRM + CSng(0.01) '2 to TTL-0.1 inclusive
                    TLOutIncrement = TL_INCRM
                    TLOutNext = TrophicLevelOut + TLOutIncrement
                    AryIdxNext = AryIdx + 1
                    Return CSng(Math.Log(BwdCalFlowFn(TrophicLevelOut, AryIdx) / BwdCalFlowFn(TLOutNext, AryIdxNext)) / TLOutIncrement - _
                      CTSANaturalLossRateFn(TrophicLevelOut))
                    'Return CSng(Math.Log(BwdCalFlow(AryIdx) / BwdCalFlow(AryIdxNext)) / TLOutIncrement - _
                    '  CTSANaturalLossRate(AryIdx))
                Case m_EcotrophManager.InputData.TTL
                    Return m_BwdCalFishLossRateTTL
                Case Is > m_EcotrophManager.InputData.TTL
                    'AccessFishMortality = m_BwdCalAccessFishMortalityTTL
                    'Return AccessFishMortality * CTSASelectivityFn(TrophicLevelOut) * BwdCalKinetic(AryIdx)
                    AccessFishMortality = 0.0
                    Return AccessFishMortality * CTSASelectivityFn(TrophicLevelOut) / BwdCalKinetic(AryIdx)
            End Select
        End Function

        Private Sub FindBwdCalAccessFishMortalityAryTLLeTTL()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalAccessFishMortality(CTSAKinetic.GetUpperBound(0))

            'BwdCalAccessFishMortality when 2<=TLOut<=TTL
            For TLOut = m_EcotrophManager.InputData.TTL To TL_OUT_INIT Step -TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalAccessFishMortality(Idx) = BwdCalAccessFishMortalityFn(TLOut, Idx)
            Next

            'BwdCalAccessFishMortality when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalAccessFishMortality(Idx) = BwdCalAccessFishMortalityFn(TLOut, Idx)

            'BwdCalAccessFishMortality when TLOut>TTL
            'For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
            '    Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            '    BwdCalAccessFishMortality(Idx) = BwdCalAccessFishMortalityFn(TLOut, Idx)
            'Next
        End Sub

        Private Function BwdCalAccessFishMortalityFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            'Debug.Assert(TrophicLevelOut > 2)
            Select Case TrophicLevelOut
                Case 1, 2 To m_EcotrophManager.InputData.TTL - TL_INCRM + CSng(0.01) '2 to TTL-0.1 inclusive
                    'Return BwdCalFishLossRate(AryIdx) / CTSASelectivity(AryIdx) * BwdCalKinetic(AryIdx)
                    Return BwdCalFishLossRateFn(TrophicLevelOut, AryIdx) / CTSASelectivityFn(TrophicLevelOut) * BwdCalKinetic(AryIdx)
                Case m_EcotrophManager.InputData.TTL ', Is > m_EcotrophManager.InputData.TTL
                    Return m_BwdCalAccessFishMortalityTTL
                Case Is > m_EcotrophManager.InputData.TTL
                    Return 0.0
            End Select
        End Function

        Private Sub FindBwdCalVirginFlowAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalVirginFlow(CTSAKinetic.GetUpperBound(0))

            'BwdCalVirginFlow when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalVirginFlow(Idx) = BwdCalVirginFlowFn(TLOut, Idx)

            'BwdCalVirginFlow when TLOut>=2
            Idx = 2
            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                BwdCalVirginFlow(Idx) = BwdCalVirginFlowFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function BwdCalVirginFlowFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double
            Dim TLOutIncrement As Double
            Dim AryIdxPrevious As Integer

            'Debug.Assert(TrophicLevelOut > 1)
            Select Case TrophicLevelOut
                Case 1
                    Return BwdCalFlowFn(TrophicLevelOut, AryIdx)
                Case 2
                    TLOutIncrement = 1.0
                    TLOutPrevious = TrophicLevelOut - TLOutIncrement
                    AryIdxPrevious = AryIdx - 1
                    Return CSng(BwdCalVirginFlow(AryIdxPrevious) * Math.Exp(-CTSANaturalLossRateFn(TLOutPrevious) * _
                      TLOutIncrement))
                Case Is > 2
                    TLOutIncrement = TL_INCRM
                    TLOutPrevious = TrophicLevelOut - TLOutIncrement
                    AryIdxPrevious = AryIdx - 1
                    Return CSng(BwdCalVirginFlow(AryIdxPrevious) * Math.Exp(-CTSANaturalLossRateFn(TLOutPrevious) * _
                      TLOutIncrement))
            End Select
        End Function

        Private Sub FindBwdCalVirginBiomassAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalVirginBiomass(CTSAKinetic.GetUpperBound(0))

            'BwdCalVirginBiomass when TLOut=1
            TLOut = 1.0
            Idx = 1
            BwdCalVirginBiomass(Idx) = BwdCalVirginBiomassFn(TLOut, Idx)

            'BwdCalVirginBiomass when TLOut>=2
            Idx = 2
            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                BwdCalVirginBiomass(Idx) = BwdCalVirginBiomassFn(TLOut, Idx)
                Idx = Idx + 1
            Next
        End Sub

        Private Function BwdCalVirginBiomassFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            'Debug.Assert(TrophicLevelOut > 1)
            Return BwdCalVirginFlowFn(TrophicLevelOut, AryIdx) / CTSAKineticFn(TrophicLevelOut)
        End Function

        Private Sub FindBwdCalAccessFishMortalityAryTLGtTTL()
            Dim TLOut As Double
            Dim Idx As Integer

            For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                If (2.0 * BwdCalAccessFishMortality(Idx - 1) - BwdCalAccessFishMortality(Idx - 2)) > 0.0 Then
                    BwdCalAccessFishMortality(Idx) = CSng(2.0 * BwdCalAccessFishMortality(Idx - 1) - BwdCalAccessFishMortality(Idx - 2))
                Else
                    BwdCalAccessFishMortality(Idx) = 0.0
                End If
            Next
        End Sub

        Private Sub FindBwdCalFishLossRateAryTLGtTTL()
            Dim TLOut As Double
            Dim Idx As Integer

            For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalFishLossRate(Idx) = BwdCalAccessFishMortality(Idx) * CTSASelectivityFn(TLOut) / BwdCalKinetic(Idx)
            Next
        End Sub

        Private Sub FindBwdCalFlowAryTLGtTTL()
            Dim TLOut As Double
            Dim Idx As Integer

            For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalFlow(Idx) = CSng(BwdCalFlow(Idx - 1) * Math.Exp(-(CTSANaturalLossRateFn(TLOut) + BwdCalFishLossRate(Idx - 1)) * TL_INCRM))
            Next
        End Sub

        Private Sub FindBwdCalBiomassAryTLGtTTL()
            Dim TLOut As Double
            Dim Idx As Integer

            For TLOut = m_EcotrophManager.InputData.TTL + TL_INCRM To TL_OUT_FINAL Step TL_INCRM
                Idx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                BwdCalBiomass(Idx) = BwdCalFlow(Idx) / BwdCalKinetic(Idx)
            Next
        End Sub

        Private Sub FindBwdCalKineticRecalAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalKineticRecal(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            BwdCalKineticRecal(Idx) = BwdCalKineticRecalFn(TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalKineticRecal(Idx) = BwdCalKineticRecalFn(TLOut)
            Next
        End Sub

        Private Function BwdCalKineticRecalFn(ByVal TrophicLevelOut As Double) As Single
            Dim AryIdx As Integer
            Dim TLOut As Double
            Dim TLOutFinal As Double
            Dim SumBwdCalBiomass As Double
            Dim SumBwdCalVirginBiomass As Double

            Dim SumTL As Double
            Dim SumLogBwdCalKineticRecal As Double
            Dim AvgTL As Double
            Dim AvgLogBwdCalKineticRecal As Double
            Dim SumTLDevLogBwdCalKineticRecalDev As Double
            Dim SumTLDevSquare As Double
            Dim Slope As Double
            Dim Intercept As Double

            Select Case TrophicLevelOut
                Case 1
                    SumBwdCalBiomass = 0.0
                    SumBwdCalVirginBiomass = 0.0
                    For TLOut = TL_OUT_INIT To 2.5 Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        SumBwdCalBiomass = SumBwdCalBiomass + BwdCalBiomass(AryIdx)
                        SumBwdCalVirginBiomass = SumBwdCalVirginBiomass + BwdCalVirginBiomass(AryIdx)
                    Next
                    AryIdx = 1 'CInt((Int(TrophicLevelOut) - 2) * 10 + CInt((TrophicLevelOut - Int(TrophicLevelOut)) * 10) + 2)
                    Return CSng(CTSAKineticFn(TrophicLevelOut) * (1 + m_EcotrophManager.InputData.CTSATopD(AryIdx) * _
                      ((Math.Pow(SumBwdCalBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)) - Math.Pow(SumBwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx))) / _
                      Math.Pow(SumBwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)))) + BwdCalFishLossRate(1) * BwdCalKinetic(1))
                Case 2 To 5.8
                    If TrophicLevelOut < 5.79 Then
                        TLOutFinal = TrophicLevelOut + 1.3 'CSng(TrophicLevelOut + 1.3)
                    Else '=5.8
                        TLOutFinal = TrophicLevelOut + 1.2 'CSng(TrophicLevelOut + 1.2)
                    End If
                    SumBwdCalBiomass = 0.0
                    SumBwdCalVirginBiomass = 0.0
                    For TLOut = (TrophicLevelOut + 0.8) To TLOutFinal Step TL_INCRM 'CSng(TrophicLevelOut + 0.8) To TLOutFinal Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        SumBwdCalBiomass = SumBwdCalBiomass + BwdCalBiomass(AryIdx)
                        SumBwdCalVirginBiomass = SumBwdCalVirginBiomass + BwdCalVirginBiomass(AryIdx)
                    Next
                    AryIdx = CInt((Int(TrophicLevelOut) - 2) * 10 + CInt((TrophicLevelOut - Int(TrophicLevelOut)) * 10) + 2)
                    Return CSng(CTSAKineticFn(TrophicLevelOut) * (1 + m_EcotrophManager.InputData.CTSATopD(AryIdx) * _
                      ((Math.Pow(SumBwdCalBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)) - Math.Pow(SumBwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx))) / _
                      Math.Pow(SumBwdCalVirginBiomass, m_EcotrophManager.InputData.CTSAFormD(AryIdx)))) + BwdCalFishLossRate(AryIdx) * BwdCalKinetic(AryIdx))
                Case 5.89 To 7 ' 5.9 to 7
                    SumTL = 0.0
                    SumLogBwdCalKineticRecal = 0.0
                    For TLOut = 5 To 5.8 Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        SumTL = SumTL + TLOut
                        SumLogBwdCalKineticRecal = SumLogBwdCalKineticRecal + Math.Log(BwdCalKineticRecal(AryIdx))
                    Next
                    AvgTL = SumTL / 9.0
                    AvgLogBwdCalKineticRecal = SumLogBwdCalKineticRecal / 9.0
                    SumTLDevLogBwdCalKineticRecalDev = 0.0
                    SumTLDevSquare = 0.0
                    For TLOut = 5 To 5.8 Step TL_INCRM
                        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
                        SumTLDevLogBwdCalKineticRecalDev = SumTLDevLogBwdCalKineticRecalDev + (TLOut - AvgTL) * _
                          (Math.Log(BwdCalKineticRecal(AryIdx)) - AvgLogBwdCalKineticRecal)
                        SumTLDevSquare = SumTLDevSquare + (TLOut - AvgTL) * (TLOut - AvgTL)
                    Next
                    Slope = SumTLDevLogBwdCalKineticRecalDev / SumTLDevSquare
                    Intercept = AvgLogBwdCalKineticRecal - Slope * AvgTL

                    AryIdx = CInt((Int(TrophicLevelOut) - 2) * 10 + CInt((TrophicLevelOut - Int(TrophicLevelOut)) * 10) + 2)
                    If CSng(Math.Exp(Intercept + Slope * TrophicLevelOut)) > CTSAKinetic(AryIdx) Then
                        Return CSng(Math.Exp(Intercept + Slope * TrophicLevelOut))
                    Else
                        Return CTSAKinetic(AryIdx)
                    End If
            End Select
        End Function

        Private Function BwdCalKineticStablisationFn(ByRef KineticCriteria As Double) As Boolean
            Dim TLOut As Double
            Dim AryIdx As Integer
            Dim SumBwdCalKineticRecal As Double
            Dim SumBwdCalKinetic As Double

            TLOut = 5.8
            AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
            SumBwdCalKineticRecal = 0.0
            For Idx As Integer = 2 To AryIdx
                SumBwdCalKineticRecal = SumBwdCalKineticRecal + BwdCalKineticRecal(Idx)
            Next
            SumBwdCalKinetic = 0.0
            For Idx As Integer = 2 To AryIdx
                SumBwdCalKinetic = SumBwdCalKinetic + BwdCalKinetic(Idx)
            Next
            If Math.Abs(SumBwdCalKineticRecal - SumBwdCalKinetic) > 0.000001 Then
                For Idx As Integer = 1 To BwdCalKineticRecal.GetUpperBound(0)
                    BwdCalKinetic(Idx) = BwdCalKineticRecal(Idx)
                Next
                KineticCriteria = SumBwdCalKineticRecal - SumBwdCalKinetic
                Return False
            Else
                KineticCriteria = SumBwdCalKineticRecal - SumBwdCalKinetic
                Return True
            End If
        End Function

        'Private Function BwdCalAccessFishMortalityStablisationFn(ByRef IsIterated As Boolean) As Boolean
        '    Dim AryIdx As Integer
        '    Dim SumAccessFishMortality As Double
        '    Dim AvgAccessFishMortality As Double

        '    SumAccessFishMortality = 0.0
        '    For TLOut As Double = m_EcotrophManager.InputData.TTL - CSng(0.5) To m_EcotrophManager.InputData.TTL - CSng(0.1) Step TL_INCRM
        '        AryIdx = CInt((Int(TLOut) - 2) * 10 + CInt((TLOut - Int(TLOut)) * 10) + 2)
        '        SumAccessFishMortality = SumAccessFishMortality + BwdCalAccessFishMortality(AryIdx)
        '    Next
        '    AvgAccessFishMortality = SumAccessFishMortality / 5.0
        '    If Math.Abs(m_EcotrophManager.InputData.SlopeSelectivityTTL * AvgAccessFishMortality - _
        '      m_BwdCalAccessFishMortalityTTL) > 0.000001 Then
        '        m_BwdCalAccessFishMortalityTTL = CSng(m_EcotrophManager.InputData.SlopeSelectivityTTL * AvgAccessFishMortality)
        '        IsIterated = True
        '        Return False
        '    Else
        '        Return True
        '    End If
        'End Function

        Private Sub FindBwdCalAccessBiomassAry()
            Dim Idx As Integer
            ReDim BwdCalAccessBiomass(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            BwdCalAccessBiomass(Idx) = BwdCalBiomass(Idx) * CTSASelectivity(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalAccessBiomass(Idx) = BwdCalBiomass(Idx) * CTSASelectivity(Idx)
            Next
        End Sub

        Private Sub FindBwdCalAccessFishLossRateAry()
            Dim Idx As Integer
            ReDim BwdCalAccessFishLossRate(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            BwdCalAccessFishLossRate(Idx) = m_EcotrophManager.InputData.Catches(Idx) / BwdCalAccessBiomass(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalAccessFishLossRate(Idx) = m_EcotrophManager.InputData.Catches(Idx) / BwdCalAccessBiomass(Idx)
            Next
        End Sub

        Private Sub FindBwdCalFishMortalityAry()
            Dim Idx As Integer
            ReDim BwdCalFishMortality(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            BwdCalFishMortality(Idx) = BwdCalFishLossRate(Idx) * BwdCalKinetic(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalFishMortality(Idx) = BwdCalFishLossRate(Idx) * BwdCalKinetic(Idx)
            Next
        End Sub

        Private Sub FindBwdCalSelectivityAry()
            Dim Idx As Integer
            ReDim BwdCalSelectivity(CTSAKinetic.GetUpperBound(0))

            Idx = 1
            BwdCalSelectivity(Idx) = BwdCalAccessBiomass(Idx) / BwdCalBiomass(Idx)

            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                BwdCalSelectivity(Idx) = BwdCalAccessBiomass(Idx) / BwdCalBiomass(Idx)
            Next
        End Sub

        Private Sub FindBwdCalTimeAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim BwdCalTime(CTSAKinetic.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            BwdCalTime(Idx) = BwdCalTimeFn(TLOut, Idx)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                BwdCalTime(Idx) = BwdCalTimeFn(TLOut, Idx)
            Next
        End Sub

        Private Function BwdCalTimeFn(ByVal TrophicLevelOut As Double, ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double

            Select Case TrophicLevelOut
                Case 1
                    Return 0
                Case 2
                    TLOutPrevious = 1
                Case Is > 2
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
            End Select
            Return CSng(BwdCalTime(AryIdx - 1) + ((TrophicLevelOut - TLOutPrevious) / BwdCalKinetic(AryIdx - 1)))
        End Function
#End Region 'Helper methods

    End Class

End Namespace
