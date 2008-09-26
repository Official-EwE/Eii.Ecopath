'==============================================================================
'
' $Log: cTranspose.vb,v $
' Revision 1.1  2008/09/26 07:30:37  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.50  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports System.Xml

Namespace Computation

    Public Class cTranspose

#Region "Private fields"
        Private Const TL_IN_INIT As Double = 2.0
        Private Const TL_IN_FINAL As Double = 5.0
        Private Const TL_OUT_INIT As Double = 2.0
        Private Const TL_OUT_FINAL As Double = 7.0
        Private Const TL_INCRM As Double = 0.1
        Private Const PRGRS_BAR_MAX As Integer = 10

        Private m_EcotrophManager As cEcotrophManager
        Private m_EPdata As cEcopathDataStructures
#End Region 'Private fields

#Region "Public fields"
        'Public SmoothFactor As Single
        Public SigmaLN() As Single
        Public Proportion(,) As Single
        Public ProportionSTD(,) As Single
        Public TransposeBiomass(,) As Single
        Public TLTruncated() As Single
        Public Flow(,) As Single
        Public TransposeCatch(,,) As Single
        Public TransposeCatchSumGp(,) As Single
        Public AccessBiomass(,) As Single

        Public TransposeBiomassSum() As Single
        Public AEFBiomass() As Single
        Public OmniIdxBiomass() As Single
        Public UserDefValBiomass() As Single

        Public AccessBiomassSum() As Single
        Public AEFAccessBiomass() As Single
        Public OmniIdxAccessBiomass() As Single
        Public UserDefValAccessBiomass() As Single

        Public TransposeFlowSum() As Single
        Public AEFFlow() As Single
        Public OmniIdxFlow() As Single
        Public UserDefValFlow() As Single

        Public Kinetic() As Single
        Public AEFKinetic() As Single
        Public OmniIdxKinetic() As Single
        Public UserDefValKinetic() As Single

        Public TransposeCatchSumGpFlt() As Single
        Public AEFCatches() As Single
        Public OmniIdxCatches() As Single
        Public UserDefValCatches() As Single

        Public FishLossRate() As Single
        Public AEFFishLossRate() As Single
        Public OmniIdxFishLossRate() As Single
        Public UserDefValFishLossRate() As Single

        Public AccessFishLossRate() As Single
        Public AEFAccessFishLossRate() As Single
        Public OmniIdxAccessFishLossRate() As Single
        Public UserDefValAccessFishLossRate() As Single

        Public NaturalLossRate() As Single
        Public AEFNaturalLossRate() As Single
        Public OmniIdxNaturalLossRate() As Single
        Public UserDefValNaturalLossRate() As Single

        Public FishMortality() As Single
        Public AEFFishMortality() As Single
        Public OmniIdxFishMortality() As Single
        Public UserDefValFishMortality() As Single

        Public Selectivity() As Single
        Public AEFSelectivity() As Single
        Public OmniIdxSelectivity() As Single
        Public UserDefValSelectivity() As Single

        Public AccessFishMortality() As Single
        Public AEFAccessFishMortality() As Single
        Public OmniIdxAccessFishMortality() As Single
        Public UserDefValAccessFishMortality() As Single

        Public Time() As Single
        Public AEFTime() As Single
        Public OmniIdxTime() As Single
        Public UserDefValTime() As Single

        Public IsAEFRun As Boolean
        Public IsOmniIdxRun As Boolean
        Public IsUserDefValRun As Boolean
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
        Public Sub RunTransposeAEF(ByVal ToolStp As ToolStrip)
            IsAEFRun = False
            m_EcotrophManager.InputData.ReadFile("SmoothFactor", m_EcotrophManager)
            m_EcotrophManager.InputData.ReadFile("Access", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            'FindSmoothFactor()
            'm_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp,PRGRS_BAR_MAX)
            FindSigmaLNAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionSTDAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeBiomassAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            For FN As Integer = 1 To m_EPdata.NumFleet
                FindTransposeCatchAry(FN)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            Next
            FindTransposeCatchSumGpAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindTransposeBiomassSumAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassSumAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowSumAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindKineticAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeCatchSumGpFltAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindNaturalLossRateAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishMortalityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindSelectivityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishMortalityAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTimeAry()
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsAEFRun = True
        End Sub

        Public Sub RunTransposeOmniIdx(ByVal ToolStp As ToolStrip)
            IsOmniIdxRun = False
            m_EcotrophManager.InputData.ReadFile("Access", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindProportionAry(My.Resources.TREE_NODE_OMNI_IDX) 'My.Resources.TREE_NODE_OMIN_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionSTDAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeBiomassAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            For FN As Integer = 1 To m_EPdata.NumFleet
                FindTransposeCatchAry(FN, My.Resources.TREE_NODE_OMNI_IDX)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            Next FN
            FindTransposeCatchSumGpAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindTransposeBiomassSumAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassSumAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowSumAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindKineticAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeCatchSumGpFltAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishLossRateAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishLossRateAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindNaturalLossRateAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishMortalityAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindSelectivityAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishMortalityAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTimeAry(My.Resources.TREE_NODE_OMNI_IDX)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsOmniIdxRun = True
        End Sub

        Public Sub RunTransposeUserDefVal(ByVal ToolStp As ToolStrip)
            IsUserDefValRun = False
            m_EcotrophManager.InputData.ReadFile("Sigma", m_EcotrophManager)
            m_EcotrophManager.InputData.ReadFile("Access", m_EcotrophManager)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindProportionAry(My.Resources.TREE_NODE_USER_DEF_SIGMA) ' My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionSTDAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeBiomassAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            For FN As Integer = 1 To m_EPdata.NumFleet
                FindTransposeCatchAry(FN, My.Resources.TREE_NODE_USER_DEF_SIGMA)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            Next FN
            FindTransposeCatchSumGpAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindTransposeBiomassSumAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessBiomassSumAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeFlowSumAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindKineticAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTransposeCatchSumGpFltAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishLossRateAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishLossRateAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindNaturalLossRateAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindFishMortalityAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindSelectivityAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindAccessFishMortalityAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindTimeAry(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            IsUserDefValRun = True
        End Sub

        Public Sub RunTransposeAEFCatches(ByVal ToolStp As ToolStrip)
            Dim IsCatchImport As Boolean

            IsCatchImport = True
            FindSigmaLNAry(IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionAry(IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionSTDAry(IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            For FN As Integer = 1 To m_EcotrophManager.InputData.NumFleetImport
                FindTransposeCatchAry(FN, IsCatchImport)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            Next
            FindTransposeCatchSumGpAry(IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindTransposeCatchSumGpFltAry(IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
        End Sub

        Public Sub RunTransposeUserDefValCatches(ByVal ToolStp As ToolStrip)
            Dim IsCatchImport As Boolean

            IsCatchImport = True
            FindProportionAry(My.Resources.TREE_NODE_USER_DEF_SIGMA, IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            FindProportionSTDAry(My.Resources.TREE_NODE_USER_DEF_SIGMA, IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            For FN As Integer = 1 To m_EcotrophManager.InputData.NumFleetImport
                FindTransposeCatchAry(FN, My.Resources.TREE_NODE_USER_DEF_SIGMA, IsCatchImport)
                m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
            Next FN
            FindTransposeCatchSumGpAry(My.Resources.TREE_NODE_USER_DEF_SIGMA, IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)

            FindTransposeCatchSumGpFltAry(My.Resources.TREE_NODE_USER_DEF_SIGMA, IsCatchImport)
            m_EcotrophManager.UpdatePrgrsRunTranspose(ToolStp, PRGRS_BAR_MAX)
        End Sub
#End Region 'Public methods

#Region "Helper methods"
#Region "Using automatic empirical function"
        'Private Sub FindSmoothFactor()
        '    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
        'End Sub

        Private Sub FindSigmaLNAry(Optional ByVal IsCatchImport As Boolean = False)
            Dim Idx As Integer
            Dim IdxMax As Integer
            Dim SmoothFactor As Single

            IdxMax = 0
            For TLIn As Double = TL_IN_INIT To TL_IN_FINAL Step TL_INCRM
                IdxMax = IdxMax + 1
            Next
            ReDim SigmaLN(IdxMax)

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
            End Select

            Idx = 1
            For TLIn As Double = TL_IN_INIT To TL_IN_FINAL Step TL_INCRM
                SigmaLN(Idx) = SigmaLNFn(SmoothFactor, TLIn)
                Idx = Idx + 1
            Next
        End Sub

        Private Function SigmaLNFn(ByVal SmoothFactor As Single, ByVal TrophicLevel As Double) _
          As Single
            Return CSng(SmoothFactor * Math.Log(TrophicLevel - 0.05))
        End Function

        Private Sub FindProportionAry(Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim RowMax As Integer
            Dim ColMax As Integer
            Dim SmoothFactor As Single

            RowMax = 0
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                RowMax = RowMax + 1
            Next
            ColMax = SigmaLN.GetUpperBound(0)
            ReDim Proportion(RowMax, ColMax)

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
            End Select

            Col = 1
            For TLIn As Double = TL_IN_INIT To TL_IN_FINAL Step TL_INCRM
                Row = 1
                For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    Proportion(Row, Col) = ProportionFn(SmoothFactor, TLIn, TLOut)
                    Row = Row + 1
                Next
                Col = Col + 1
            Next
        End Sub

        Private Function ProportionFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
          ByVal TrophicLevelOut As Double) As Single
            Return CSng(Math.Exp(-0.5 * ((Math.Log(TrophicLevelOut - 0.95) - Math.Log(TrophicLevelIn - 0.95)) / _
              SigmaLNFn(SmoothFactor, TrophicLevelIn)) ^ 2) / _
              (SigmaLNFn(SmoothFactor, TrophicLevelIn) * Math.Sqrt(2.0 * Math.PI)))
        End Function

        Private Function ProportionSumOverTLOutFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double) As Single
            Dim ProportionSumOverTLOut As Single

            ProportionSumOverTLOut = 0
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                ProportionSumOverTLOut = ProportionSumOverTLOut + ProportionFn(SmoothFactor, TrophicLevelIn, TLOut)
            Next
            Return ProportionSumOverTLOut
        End Function

        Private Sub FindProportionSTDAry(Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim SmoothFactor As Single
            ReDim ProportionSTD(Proportion.GetUpperBound(0) + 1, Proportion.GetUpperBound(1) + 1)

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
            End Select

            'TLIn=1 & TLOut=1
            Col = 1
            Row = 1
            ProportionSTD(Row, Col) = ProportionSTDFn(SmoothFactor, 1, 1, 0)

            'TLIn=1 
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Row = Row + 1
                ProportionSTD(Row, Col) = ProportionSTDFn(SmoothFactor, 1, TLOut, 0)
            Next

            'TLOut=1
            Row = 1
            Col = 1
            For TLIn As Double = TL_IN_INIT To TL_IN_FINAL Step TL_INCRM
                Col = Col + 1
                ProportionSTD(Row, Col) = ProportionSTDFn(SmoothFactor, TLIn, 1, 0)
            Next

            'TLIn<>1 & TLOut<>1
            Col = 2
            For TLIn As Double = TL_IN_INIT To TL_IN_FINAL Step TL_INCRM
                Row = 2
                For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    ProportionSTD(Row, Col) = ProportionSTDFn(SmoothFactor, TLIn, TLOut, _
                      ProportionSumOverTLOutFn(SmoothFactor, TLIn))
                    Row = Row + 1
                Next
                Col = Col + 1
            Next
        End Sub

        Private Function ProportionSTDFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
          ByVal TrophicLevelOut As Double, ByVal ProportionSumOverTLOut As Single) As Single
            Select Case TrophicLevelIn * TrophicLevelOut
                Case 1
                    Return 1
                Case TrophicLevelOut
                    Return 0
                Case TrophicLevelIn
                    Return 0
                Case Else
                    Return ProportionFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut) / ProportionSumOverTLOut
            End Select
        End Function

        Private Sub FindTransposeBiomassAry()
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim TransposeBiomass(ProportionSTD.GetUpperBound(0), m_EPdata.NumGroups)
            ReDim TLTruncated(m_EPdata.NumGroups)

            For Col = 1 To m_EPdata.NumGroups
                Row = 1
                TLOut = 1
                TLTruncated(Col) = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(m_EcotrophManager.InputData.SmoothFactor, _
                  TLTruncated(Col))
                TransposeBiomass(Row, Col) = TransposeBiomassFn(m_EcotrophManager.InputData.SmoothFactor, _
                  TLTruncated(Col), TLOut, ProportionSumOverTLOut, Col)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    TransposeBiomass(Row, Col) = TransposeBiomassFn(m_EcotrophManager.InputData.SmoothFactor, _
                      TLTruncated(Col), TLOut, ProportionSumOverTLOut, Col)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function TransposeBiomassFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
          ByVal TrophicLevelOut As Double, ByVal ProportionSumOverTLOut As Single, _
          ByVal GroupNumber As Integer) As Single
            Return ProportionSTDFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut, ProportionSumOverTLOut) * _
              BiomassFn(GroupNumber)
        End Function

        Private Function BiomassFn(ByVal GroupNum As Integer) As Single
            Return m_EPdata.B(GroupNum)
        End Function

        Private Function TrophicLevelTruncatedFn(ByVal GroupNum As Integer, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return Int(m_EPdata.TTLX(GroupNum) * 10) / 10
                Case True
                    Return Int(m_EcotrophManager.InputData.TLImport(GroupNum) * 10) / 10
            End Select
        End Function

        Private Sub FindTransposeFlowAry()
            Dim Row As Integer
            Dim Col As Integer
            Dim TLIn As Double
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim Flow(ProportionSTD.GetUpperBound(0), m_EPdata.NumLiving)

            For Col = 1 To m_EPdata.NumLiving
                Row = 1
                TLOut = 1
                TLIn = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(m_EcotrophManager.InputData.SmoothFactor, _
                  TLIn)
                Flow(Row, Col) = TransposeFlowFn(m_EcotrophManager.InputData.SmoothFactor, TLIn, TLOut, _
                  ProportionSumOverTLOut, Col)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    Flow(Row, Col) = TransposeFlowFn(m_EcotrophManager.InputData.SmoothFactor, TLIn, TLOut, _
                      ProportionSumOverTLOut, Col)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function TransposeFlowFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
          ByVal TrophicLevelOut As Double, ByVal ProportionSumOverTLOut As Single, _
          ByVal GroupNumber As Integer) As Single
            Return ProportionSTDFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut, ProportionSumOverTLOut) * _
              BiomassFn(GroupNumber) * ProdBiomFn(GroupNumber)
        End Function

        Private Function ProdBiomFn(ByVal GroupNum As Integer) As Single
            Return m_EPdata.PB(GroupNum)
        End Function

        Private Sub FindTransposeCatchAry(ByVal FleetNumber As Integer, Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLIn As Double
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            Dim SmoothFactor As Single
            Dim NumLiving As Integer
            Dim NumFleet As Integer

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                    NumLiving = m_EPdata.NumLiving
                    NumFleet = m_EPdata.NumFleet
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
            End Select

            If FleetNumber = 1 Then
                ReDim TransposeCatch(ProportionSTD.GetUpperBound(0), NumLiving, NumFleet)
            Else
                ReDim Preserve TransposeCatch(ProportionSTD.GetUpperBound(0), NumLiving, NumFleet)
            End If

            Select Case IsCatchImport
                Case False
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        TLIn = TrophicLevelTruncatedFn(Col)
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                        TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(SmoothFactor, TLIn, TLOut, _
                          ProportionSumOverTLOut, Col, FleetNumber)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(SmoothFactor, TLIn, TLOut, _
                              ProportionSumOverTLOut, Col, FleetNumber)
                            Row = Row + 1
                        Next
                    Next
                Case True
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        TLIn = TrophicLevelTruncatedFn(Col, IsCatchImport)
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                        TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(SmoothFactor, TLIn, TLOut, _
                          ProportionSumOverTLOut, Col, FleetNumber, IsCatchImport)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(SmoothFactor, TLIn, TLOut, _
                              ProportionSumOverTLOut, Col, FleetNumber, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function TransposeCatchFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
          ByVal TrophicLevelOut As Double, ByVal ProportionSumOverTLOut As Single, ByVal GroupNumber As Integer, _
          ByVal FleetNumber As Integer, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return ProportionSTDFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut, ProportionSumOverTLOut) * _
                      CatchFn(FleetNumber, GroupNumber)
                Case True
                    Return ProportionSTDFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut, ProportionSumOverTLOut) * _
                      CatchFn(FleetNumber, GroupNumber, IsCatchImport)
            End Select
        End Function

        Private Function CatchFn(ByVal FleetNum As Integer, ByVal GroupNum As Integer, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return m_EPdata.Landing(FleetNum, GroupNum) + m_EPdata.Discard(FleetNum, GroupNum)
                Case True
                    Return m_EcotrophManager.InputData.CatchesImport(FleetNum, GroupNum)
            End Select
        End Function

        Private Sub FindTransposeCatchSumGpAry(Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim SmoothFactor As Single
            Dim NumFleet As Integer

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                    NumFleet = m_EPdata.NumFleet
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
            End Select
            ReDim TransposeCatchSumGp(ProportionSTD.GetUpperBound(0), NumFleet)

            Select Case IsCatchImport
                Case False
                    For Col = 1 To NumFleet
                        Row = 1
                        TLOut = 1
                        TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(SmoothFactor, TLOut, Col)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(SmoothFactor, TLOut, Col)
                            Row = Row + 1
                        Next
                    Next
                Case True
                    For Col = 1 To NumFleet
                        Row = 1
                        TLOut = 1
                        TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(SmoothFactor, TLOut, Col, IsCatchImport)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(SmoothFactor, TLOut, Col, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function TransposeCatchSumGpFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double, _
          ByVal FleetNumber As Integer, Optional ByVal IsCatchImport As Boolean = False) As Single
            Dim Col As Integer
            Dim TLIn As Double
            Dim ProportionSumOverTLOut As Single
            Dim TransposeCatchSumOverGroup As Single
            Dim NumLiving As Integer

            Select Case IsCatchImport
                Case False
                    NumLiving = m_EPdata.NumLiving
                Case True
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
            End Select

            Select Case IsCatchImport
                Case False
                    TransposeCatchSumOverGroup = 0
                    For Col = 1 To NumLiving
                        TLIn = TrophicLevelTruncatedFn(Col)
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                        TransposeCatchSumOverGroup = TransposeCatchSumOverGroup + TransposeCatchFn(SmoothFactor, TLIn, _
                          TrophicLevelOut, ProportionSumOverTLOut, Col, FleetNumber)
                    Next
                    Return TransposeCatchSumOverGroup
                Case True
                    TransposeCatchSumOverGroup = 0
                    For Col = 1 To NumLiving
                        TLIn = TrophicLevelTruncatedFn(Col, IsCatchImport)
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                        TransposeCatchSumOverGroup = TransposeCatchSumOverGroup + TransposeCatchFn(SmoothFactor, TLIn, _
                          TrophicLevelOut, ProportionSumOverTLOut, Col, FleetNumber, IsCatchImport)
                    Next
                    Return TransposeCatchSumOverGroup
            End Select
        End Function

        Private Sub FindAccessBiomassAry()
            Dim Row As Integer
            Dim Col As Integer
            Dim TLIn As Double
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim AccessBiomass(ProportionSTD.GetUpperBound(0), m_EPdata.NumGroups)

            For Col = 1 To m_EPdata.NumGroups
                Row = 1
                TLOut = 1
                TLIn = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(m_EcotrophManager.InputData.SmoothFactor, TLIn)
                AccessBiomass(Row, Col) = AccessBiomassFn(m_EcotrophManager.InputData.SmoothFactor, TLIn, TLOut, _
                  ProportionSumOverTLOut, Col)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    AccessBiomass(Row, Col) = AccessBiomassFn(m_EcotrophManager.InputData.SmoothFactor, TLIn, _
                      TLOut, ProportionSumOverTLOut, Col)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function AccessBiomassFn(ByVal SmoothFactor As Single, ByVal TrophicLevelIn As Double, _
        ByVal TrophicLevelOut As Double, ByVal ProportionSumOverTLOut As Single, _
        ByVal GroupNumber As Integer) As Single
            Return TransposeBiomassFn(SmoothFactor, TrophicLevelIn, TrophicLevelOut, _
              ProportionSumOverTLOut, GroupNumber) * m_EcotrophManager.InputData.Access(GroupNumber)
        End Function

        Private Sub FindTransposeBiomassSumAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim TransposeBiomassSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            TransposeBiomassSum(Idx) = TransposeBiomassSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                TransposeBiomassSum(Idx) = TransposeBiomassSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFBiomass(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFBiomass(Idx) = TransposeBiomassSum(Idx)
            Next
        End Sub

        Private Function TransposeBiomassSumFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
          As Single
            Dim Col As Integer
            Dim TLIn As Double
            Dim ProportionSumOverTLOut As Single
            Dim TransposeBiomassSumOverGroup As Single

            TransposeBiomassSumOverGroup = 0
            For Col = 1 To m_EPdata.NumGroups
                TLIn = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                TransposeBiomassSumOverGroup = TransposeBiomassSumOverGroup + TransposeBiomassFn(SmoothFactor, _
                  TLIn, TrophicLevelOut, ProportionSumOverTLOut, Col)
            Next
            Return TransposeBiomassSumOverGroup
        End Function

        Private Sub FindAccessBiomassSumAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessBiomassSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessBiomassSum(Idx) = AccessBiomassSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessBiomassSum(Idx) = AccessBiomassSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFAccessBiomass(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFAccessBiomass(Idx) = AccessBiomassSum(Idx)
            Next
        End Sub

        Private Function AccessBiomassSumFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
          As Single
            Dim Col As Integer
            Dim TLIn As Double
            Dim ProportionSumOverTLOut As Single
            Dim AccessBiomassSumOverGroup As Single

            AccessBiomassSumOverGroup = 0
            For Col = 1 To m_EPdata.NumGroups
                TLIn = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                AccessBiomassSumOverGroup = AccessBiomassSumOverGroup + AccessBiomassFn(SmoothFactor, _
                  TLIn, TrophicLevelOut, ProportionSumOverTLOut, Col)
            Next
            Return AccessBiomassSumOverGroup
        End Function

        Private Sub FindTransposeFlowSumAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim TransposeFlowSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            TransposeFlowSum(Idx) = TransposeFlowSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                TransposeFlowSum(Idx) = TransposeFlowSumFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFFlow(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFFlow(Idx) = TransposeFlowSum(Idx)
            Next
        End Sub

        Private Function TransposeFlowSumFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
          As Single
            Dim Col As Integer
            Dim TLIn As Double
            Dim ProportionSumOverTLOut As Single
            Dim TransposeFlowSumOverGroup As Single

            TransposeFlowSumOverGroup = 0
            For Col = 1 To m_EPdata.NumLiving
                TLIn = TrophicLevelTruncatedFn(Col)
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(SmoothFactor, TLIn)
                TransposeFlowSumOverGroup = TransposeFlowSumOverGroup + TransposeFlowFn(SmoothFactor, TLIn, _
                  TrophicLevelOut, ProportionSumOverTLOut, Col)
            Next
            Return TransposeFlowSumOverGroup
        End Function

        Private Sub FindKineticAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Kinetic(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            Kinetic(Idx) = KineticFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                Kinetic(Idx) = KineticFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFKinetic(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFKinetic(Idx) = Kinetic(Idx)
            Next
        End Sub

        Private Function KineticFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return TransposeFlowSumFn(SmoothFactor, TrophicLevelOut) / _
              TransposeBiomassSumFn(SmoothFactor, TrophicLevelOut)
        End Function

        Private Sub FindTransposeCatchSumGpFltAry(Optional ByVal IsCatchImport As Boolean = False)
            Dim Idx As Integer
            Dim TLOut As Double
            Dim SmoothFactor As Single
            ReDim TransposeCatchSumGpFlt(ProportionSTD.GetUpperBound(0))

            Select Case IsCatchImport
                Case False
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactor
                Case True
                    SmoothFactor = m_EcotrophManager.InputData.SmoothFactorImport
            End Select

            Select Case IsCatchImport
                Case False
                    Idx = 1
                    TLOut = 1
                    TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(SmoothFactor, TLOut)

                    For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Idx = Idx + 1
                        TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(SmoothFactor, TLOut)
                    Next
                Case True
                    Idx = 1
                    TLOut = 1
                    TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(SmoothFactor, TLOut, IsCatchImport)

                    For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Idx = Idx + 1
                        TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(SmoothFactor, TLOut, IsCatchImport)
                    Next
            End Select

            ReDim AEFCatches(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFCatches(Idx) = TransposeCatchSumGpFlt(Idx)
            Next
        End Sub

        Private Function TransposeCatchSumGpFltFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double, _
          Optional ByVal IsCatchImport As Boolean = False) As Single
            Dim TransposeCatchSumOverGroupFleet As Single
            Dim NumFleet As Integer

            Select Case IsCatchImport
                Case False
                    NumFleet = m_EPdata.NumFleet
                Case True
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
            End Select

            Select Case IsCatchImport
                Case False
                    TransposeCatchSumOverGroupFleet = 0
                    For FN As Integer = 1 To NumFleet
                        TransposeCatchSumOverGroupFleet = TransposeCatchSumOverGroupFleet + _
                          TransposeCatchSumGpFn(SmoothFactor, TrophicLevelOut, FN)
                    Next
                    Return TransposeCatchSumOverGroupFleet
                Case True
                    TransposeCatchSumOverGroupFleet = 0
                    For FN As Integer = 1 To NumFleet
                        TransposeCatchSumOverGroupFleet = TransposeCatchSumOverGroupFleet + _
                          TransposeCatchSumGpFn(SmoothFactor, TrophicLevelOut, FN, IsCatchImport)
                    Next
                    Return TransposeCatchSumOverGroupFleet
            End Select
        End Function

        Private Sub FindFishLossRateAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FishLossRate(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FishLossRate(Idx) = FishLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FishLossRate(Idx) = FishLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFFishLossRate(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFFishLossRate(Idx) = FishLossRate(Idx)
            Next
        End Sub

        Private Function FishLossRateFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return TransposeCatchSumGpFltFn(SmoothFactor, TrophicLevelOut) / _
              TransposeFlowSumFn(SmoothFactor, TrophicLevelOut)
        End Function

        Private Sub FindAccessFishLossRateAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessFishLossRate(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessFishLossRate(Idx) = AccessFishLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessFishLossRate(Idx) = AccessFishLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFAccessFishLossRate(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFAccessFishLossRate(Idx) = AccessFishLossRate(Idx)
            Next
        End Sub

        Private Function AccessFishLossRateFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return TransposeCatchSumGpFltFn(SmoothFactor, TrophicLevelOut) / _
              (AccessBiomassSumFn(SmoothFactor, TrophicLevelOut) * KineticFn(SmoothFactor, TrophicLevelOut))
        End Function

        Private Sub FindNaturalLossRateAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim NaturalLossRate(ProportionSTD.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            NaturalLossRate(Idx) = NaturalLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                NaturalLossRate(Idx) = NaturalLossRateFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFNaturalLossRate(ProportionSTD.GetUpperBound(0) - 1)
            For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                AEFNaturalLossRate(Idx) = NaturalLossRate(Idx)
            Next
        End Sub

        Private Function NaturalLossRateFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
          As Single
            Dim TLOutNext As Double

            If TrophicLevelOut = 1 Then
                TLOutNext = 2
            Else
                TLOutNext = TrophicLevelOut + TL_INCRM
            End If
            Return CSng(Math.Log(TransposeFlowSumFn(SmoothFactor, TrophicLevelOut) / _
              TransposeFlowSumFn(SmoothFactor, TLOutNext)) / _
              (TLOutNext - TrophicLevelOut) - FishLossRateFn(SmoothFactor, TrophicLevelOut))
        End Function

        Private Sub FindFishMortalityAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FishMortality(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FishMortality(Idx) = FishMortalityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FishMortality(Idx) = FishMortalityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFFishMortality(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFFishMortality(Idx) = FishMortality(Idx)
            Next
        End Sub

        Private Function FishMortalityFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return TransposeCatchSumGpFltFn(SmoothFactor, TrophicLevelOut) / _
              TransposeBiomassSumFn(SmoothFactor, TrophicLevelOut)
        End Function

        Private Sub FindSelectivityAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Selectivity(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            Selectivity(Idx) = SelectivityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                Selectivity(Idx) = SelectivityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFSelectivity(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFSelectivity(Idx) = Selectivity(Idx)
            Next
        End Sub

        Private Function SelectivityFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return AccessBiomassSumFn(SmoothFactor, TrophicLevelOut) / _
              TransposeBiomassSumFn(SmoothFactor, TrophicLevelOut)
        End Function

        Private Sub FindAccessFishMortalityAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessFishMortality(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessFishMortality(Idx) = AccessFishMortalityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessFishMortality(Idx) = AccessFishMortalityFn(m_EcotrophManager.InputData.SmoothFactor, TLOut)
            Next

            ReDim AEFAccessFishMortality(ProportionSTD.GetUpperBound(0))
            For Idx = 1 To ProportionSTD.GetUpperBound(0)
                AEFAccessFishMortality(Idx) = AccessFishMortality(Idx)
            Next
        End Sub

        Private Function AccessFishMortalityFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double) _
              As Single
            Return FishMortalityFn(SmoothFactor, TrophicLevelOut) / _
              SelectivityFn(SmoothFactor, TrophicLevelOut)
        End Function

        Private Sub FindTimeAry()
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Time(ProportionSTD.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            Time(Idx) = TimeFn(m_EcotrophManager.InputData.SmoothFactor, TLOut, Idx)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                Time(Idx) = TimeFn(m_EcotrophManager.InputData.SmoothFactor, TLOut, Idx)
            Next

            ReDim AEFTime(ProportionSTD.GetUpperBound(0) - 1)
            For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                AEFTime(Idx) = Time(Idx)
            Next
        End Sub

        Private Function TimeFn(ByVal SmoothFactor As Single, ByVal TrophicLevelOut As Double, _
        ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double

            Select Case TrophicLevelOut
                Case 1
                    Return 0
                Case 2
                    TLOutPrevious = 1
                Case Is > 2
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
            End Select
            Return CSng(Time(AryIdx - 1) + ((TrophicLevelOut - TLOutPrevious) / KineticFn(SmoothFactor, TLOutPrevious)))
        End Function
#End Region 'Using automatic empirical function

#Region "Using omnivory index or user defined values"
        Private Function SigmaFn(ByVal GroupNum As Integer, ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    Return m_EPdata.BQB(GroupNum)
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    Select Case IsCatchImport
                        Case False
                            Return m_EcotrophManager.InputData.Sigma(GroupNum)
                        Case True
                            Return m_EcotrophManager.InputData.SigmaImport(GroupNum)
                    End Select
            End Select
        End Function

        Private Sub FindProportionAry(ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim RowMax As Integer
            Dim NumLiving As Integer

            Select Case IsCatchImport
                Case False
                    NumLiving = m_EPdata.NumLiving
                Case True
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
            End Select

            RowMax = 1
            For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                RowMax = RowMax + 1
            Next
            ReDim Proportion(RowMax, NumLiving)

            'TLOut=1
            Select Case IsCatchImport
                Case False
                    Row = 1
                    For GN As Integer = 1 To NumLiving
                        Proportion(Row, GN) = ProportionFn(GN, 1, Algor)
                    Next
                Case True
                    Row = 1
                    For GN As Integer = 1 To NumLiving
                        Proportion(Row, GN) = ProportionFn(GN, 1, Algor, IsCatchImport)
                    Next
            End Select

            'TLOut<>1
            Select Case IsCatchImport
                Case False
                    For GN As Integer = 1 To NumLiving
                        Row = 2
                        For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            Proportion(Row, GN) = ProportionFn(GN, TLOut, Algor)
                            Row = Row + 1
                        Next
                    Next
                Case True
                    For GN As Integer = 1 To NumLiving
                        Row = 2
                        For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            Proportion(Row, GN) = ProportionFn(GN, TLOut, Algor, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function ProportionFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return CSng(Math.Exp(-0.5 * ((Math.Log(TrophicLevelOut) - Math.Log(TrophicLevelTruncatedFn(GroupNumber))) * _
                      TrophicLevelTruncatedFn(GroupNumber) / SigmaFn(GroupNumber, Algor)) ^ 2) / _
                      (SigmaFn(GroupNumber, Algor) * Math.Sqrt(2.0 * Math.PI) / TrophicLevelTruncatedFn(GroupNumber)))
                Case True
                    Return CSng(Math.Exp(-0.5 * ((Math.Log(TrophicLevelOut) - Math.Log(TrophicLevelTruncatedFn(GroupNumber, IsCatchImport))) * _
                    TrophicLevelTruncatedFn(GroupNumber, IsCatchImport) / SigmaFn(GroupNumber, Algor, IsCatchImport)) ^ 2) / _
                    (SigmaFn(GroupNumber, Algor, IsCatchImport) * Math.Sqrt(2.0 * Math.PI) / TrophicLevelTruncatedFn(GroupNumber, IsCatchImport)))
            End Select
        End Function

        Private Function ProportionSumOverTLOutFn(ByVal GroupNumber As Integer, ByVal Algor As String, _
          Optional ByVal IsCatchImport As Boolean = False) As Single
            Dim Proportion As Single
            Dim ProportionSumOverTLOut As Single

            Select Case IsCatchImport
                Case False
                    Proportion = ProportionFn(GroupNumber, 1, Algor)
                    If Not Single.IsNaN(Proportion) Then
                        ProportionSumOverTLOut = Proportion
                    End If
                    For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Proportion = ProportionFn(GroupNumber, TLOut, Algor)
                        If Not Single.IsNaN(Proportion) Then
                            ProportionSumOverTLOut = ProportionSumOverTLOut + Proportion
                        End If
                    Next
                    Return ProportionSumOverTLOut
                Case True
                    Proportion = ProportionFn(GroupNumber, 1, Algor, IsCatchImport)
                    If Not Single.IsNaN(Proportion) Then
                        ProportionSumOverTLOut = Proportion
                    End If
                    For TLOut As Double = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Proportion = ProportionFn(GroupNumber, TLOut, Algor, IsCatchImport)
                        If Not Single.IsNaN(Proportion) Then
                            ProportionSumOverTLOut = ProportionSumOverTLOut + Proportion
                        End If
                    Next
                    Return ProportionSumOverTLOut
            End Select
        End Function

        Private Sub FindProportionSTDAry(ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            Dim NumLiving As Integer
            ReDim ProportionSTD(Proportion.GetUpperBound(0), Proportion.GetUpperBound(1))

            Select Case IsCatchImport
                Case False
                    NumLiving = m_EPdata.NumLiving
                Case True
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
            End Select

            Select Case IsCatchImport
                Case False
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                        ProportionSTD(Row, Col) = ProportionSTDFn(Col, TLOut, _
                          ProportionSumOverTLOut, Algor)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            ProportionSTD(Row, Col) = ProportionSTDFn(Col, TLOut, _
                              ProportionSumOverTLOut, Algor)
                            Row = Row + 1
                        Next
                    Next
                Case True
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor, IsCatchImport)
                        ProportionSTD(Row, Col) = ProportionSTDFn(Col, TLOut, _
                          ProportionSumOverTLOut, Algor, IsCatchImport)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            ProportionSTD(Row, Col) = ProportionSTDFn(Col, TLOut, _
                              ProportionSumOverTLOut, Algor, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function ProportionSTDFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal ProportionSumOverTLOut As Single, ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return ProportionFn(GroupNumber, TrophicLevelOut, Algor) / ProportionSumOverTLOut
                Case True
                    Return ProportionFn(GroupNumber, TrophicLevelOut, Algor, IsCatchImport) / ProportionSumOverTLOut
            End Select
        End Function

        Private Sub FindTransposeBiomassAry(ByVal Algor As String)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim TransposeBiomass(ProportionSTD.GetUpperBound(0), ProportionSTD.GetUpperBound(1))
            ReDim TLTruncated(m_EPdata.NumLiving)

            For Col = 1 To m_EPdata.NumLiving
                Row = 1
                TLOut = 1
                TLTruncated(Col) = TrophicLevelTruncatedFn(Col) 'Need this even though not used in this sub
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                TransposeBiomass(Row, Col) = TransposeBiomassFn(Col, TLOut, ProportionSumOverTLOut, Algor)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    TransposeBiomass(Row, Col) = TransposeBiomassFn(Col, TLOut, ProportionSumOverTLOut, Algor)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function TransposeBiomassFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal ProportionSumOverTLOut As Single, ByVal Algor As String) As Single
            Return ProportionSTDFn(GroupNumber, TrophicLevelOut, ProportionSumOverTLOut, Algor) * _
              BiomassFn(GroupNumber)
        End Function

        Private Sub FindTransposeFlowAry(ByVal Algor As String)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim Flow(ProportionSTD.GetUpperBound(0), ProportionSTD.GetUpperBound(1))

            For Col = 1 To m_EPdata.NumLiving
                Row = 1
                TLOut = 1
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                Flow(Row, Col) = TransposeFlowFn(Col, TLOut, ProportionSumOverTLOut, Algor)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    Flow(Row, Col) = TransposeFlowFn(Col, TLOut, ProportionSumOverTLOut, Algor)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function TransposeFlowFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal ProportionSumOverTLOut As Single, ByVal Algor As String) As Single
            Return ProportionSTDFn(GroupNumber, TrophicLevelOut, ProportionSumOverTLOut, Algor) * _
              BiomassFn(GroupNumber) * ProdBiomFn(GroupNumber)
        End Function

        Private Sub FindTransposeCatchAry(ByVal FleetNumber As Integer, ByVal Algor As String, _
          Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            Dim NumFleet As Integer
            Dim NumLiving As Integer

            Select Case IsCatchImport
                Case False
                    NumFleet = m_EPdata.NumFleet
                    NumLiving = m_EPdata.NumLiving
                Case True
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
            End Select

            If FleetNumber = 1 Then
                ReDim TransposeCatch(ProportionSTD.GetUpperBound(0), ProportionSTD.GetUpperBound(1), _
                  NumFleet)
            Else
                ReDim Preserve TransposeCatch(ProportionSTD.GetUpperBound(0), ProportionSTD.GetUpperBound(1), _
                  NumFleet)
            End If

            Select Case IsCatchImport
                Case False
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                        TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(Col, TLOut, _
                          ProportionSumOverTLOut, FleetNumber, Algor)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(Col, TLOut, _
                          ProportionSumOverTLOut, FleetNumber, Algor)
                            Row = Row + 1
                        Next
                    Next
                Case True
                    For Col = 1 To NumLiving
                        Row = 1
                        TLOut = 1
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor, IsCatchImport)
                        TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(Col, TLOut, _
                          ProportionSumOverTLOut, FleetNumber, Algor, IsCatchImport)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatch(Row, Col, FleetNumber) = TransposeCatchFn(Col, TLOut, _
                          ProportionSumOverTLOut, FleetNumber, Algor, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function TransposeCatchFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal ProportionSumOverTLOut As Single, ByVal FleetNumber As Integer, ByVal Algor As String, _
          Optional ByVal IsCatchImport As Boolean = False) As Single
            Select Case IsCatchImport
                Case False
                    Return ProportionSTDFn(GroupNumber, TrophicLevelOut, ProportionSumOverTLOut, Algor) * _
                      CatchFn(FleetNumber, GroupNumber)
                Case True
                    Return ProportionSTDFn(GroupNumber, TrophicLevelOut, ProportionSumOverTLOut, Algor, IsCatchImport) * _
                      CatchFn(FleetNumber, GroupNumber, IsCatchImport)
            End Select
        End Function

        Private Sub FindTransposeCatchSumGpAry(ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim NumFleet As Integer

            Select Case IsCatchImport
                Case False
                    NumFleet = m_EPdata.NumFleet
                Case True
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
            End Select
            ReDim TransposeCatchSumGp(ProportionSTD.GetUpperBound(0), NumFleet)

            Select Case IsCatchImport
                Case False
                    For Col = 1 To NumFleet
                        Row = 1
                        TLOut = 1
                        TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(TLOut, Col, Algor)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(TLOut, Col, Algor)
                            Row = Row + 1
                        Next
                    Next
                Case (True)
                    For Col = 1 To NumFleet
                        Row = 1
                        TLOut = 1
                        TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(TLOut, Col, Algor, IsCatchImport)

                        Row = Row + 1
                        For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                            TransposeCatchSumGp(Row, Col) = TransposeCatchSumGpFn(TLOut, Col, Algor, IsCatchImport)
                            Row = Row + 1
                        Next
                    Next
            End Select
        End Sub

        Private Function TransposeCatchSumGpFn(ByVal TrophicLevelOut As Double, ByVal FleetNumber As Integer, _
          ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False) As Single
            Dim Col As Integer
            Dim ProportionSumOverTLOut As Single
            Dim TransposeCatch As Single
            Dim TransposeCatchSumOverGroup As Single
            Dim NumLiving As Integer

            Select Case IsCatchImport
                Case False
                    NumLiving = m_EPdata.NumLiving
                Case True
                    NumLiving = m_EcotrophManager.InputData.NumLivingImport
            End Select

            Select Case IsCatchImport
                Case False
                    TransposeCatchSumOverGroup = 0
                    For Col = 1 To NumLiving
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                        TransposeCatch = TransposeCatchFn(Col, TrophicLevelOut, ProportionSumOverTLOut, _
                          FleetNumber, Algor)
                        If Not Single.IsNaN(TransposeCatch) Then
                            TransposeCatchSumOverGroup = TransposeCatchSumOverGroup + TransposeCatch
                        End If
                    Next
                    Return TransposeCatchSumOverGroup
                Case True
                    TransposeCatchSumOverGroup = 0
                    For Col = 1 To NumLiving
                        ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor, IsCatchImport)
                        TransposeCatch = TransposeCatchFn(Col, TrophicLevelOut, ProportionSumOverTLOut, _
                          FleetNumber, Algor, IsCatchImport)
                        If Not Single.IsNaN(TransposeCatch) Then
                            TransposeCatchSumOverGroup = TransposeCatchSumOverGroup + TransposeCatch
                        End If
                    Next
                    Return TransposeCatchSumOverGroup
            End Select
        End Function

        Private Sub FindAccessBiomassAry(ByVal Algor As String)
            Dim Row As Integer
            Dim Col As Integer
            Dim TLOut As Double
            Dim ProportionSumOverTLOut As Single
            ReDim AccessBiomass(ProportionSTD.GetUpperBound(0), ProportionSTD.GetUpperBound(1))

            For Col = 1 To m_EPdata.NumLiving
                Row = 1
                TLOut = 1
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                AccessBiomass(Row, Col) = AccessBiomassFn(Col, TLOut, ProportionSumOverTLOut, Algor)

                Row = Row + 1
                For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                    AccessBiomass(Row, Col) = AccessBiomassFn(Col, TLOut, ProportionSumOverTLOut, Algor)
                    Row = Row + 1
                Next
            Next
        End Sub

        Private Function AccessBiomassFn(ByVal GroupNumber As Integer, ByVal TrophicLevelOut As Double, _
          ByVal ProportionSumOverTLOut As Single, ByVal Algor As String) As Single
            Return TransposeBiomassFn(GroupNumber, TrophicLevelOut, ProportionSumOverTLOut, Algor) * _
              m_EcotrophManager.InputData.Access(GroupNumber)
        End Function

        Private Sub FindTransposeBiomassSumAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim TransposeBiomassSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            TransposeBiomassSum(Idx) = TransposeBiomassSumFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                TransposeBiomassSum(Idx) = TransposeBiomassSumFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxBiomass(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxBiomass(Idx) = TransposeBiomassSum(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValBiomass(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValBiomass(Idx) = TransposeBiomassSum(Idx)
                    Next
            End Select
        End Sub

        Private Function TransposeBiomassSumFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Dim Col As Integer
            Dim ProportionSumOverTLOut As Single
            Dim TransposeBiomass As Single
            Dim TransposeBiomassSumOverGroup As Single

            TransposeBiomassSumOverGroup = 0
            For Col = 1 To m_EPdata.NumGroups
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                TransposeBiomass = TransposeBiomassFn(Col, TrophicLevelOut, ProportionSumOverTLOut, Algor)
                If Not Single.IsNaN(TransposeBiomass) Then
                    TransposeBiomassSumOverGroup = TransposeBiomassSumOverGroup + TransposeBiomass
                End If
            Next
            Return TransposeBiomassSumOverGroup
        End Function

        Private Sub FindAccessBiomassSumAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessBiomassSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessBiomassSum(Idx) = AccessBiomassSumFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessBiomassSum(Idx) = AccessBiomassSumFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxAccessBiomass(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxAccessBiomass(Idx) = AccessBiomassSum(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValAccessBiomass(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValAccessBiomass(Idx) = AccessBiomassSum(Idx)
                    Next
            End Select
        End Sub

        Private Function AccessBiomassSumFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Dim Col As Integer
            Dim ProportionSumOverTLOut As Single
            Dim AccessBiomass As Single
            Dim AccessBiomassSumOverGroup As Single

            AccessBiomassSumOverGroup = 0
            For Col = 1 To m_EPdata.NumGroups
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                AccessBiomass = AccessBiomassFn(Col, TrophicLevelOut, ProportionSumOverTLOut, Algor)
                If Not Single.IsNaN(AccessBiomass) Then
                    AccessBiomassSumOverGroup = AccessBiomassSumOverGroup + AccessBiomass
                End If
            Next
            Return AccessBiomassSumOverGroup
        End Function

        Private Sub FindTransposeFlowSumAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim TransposeFlowSum(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            TransposeFlowSum(Idx) = TransposeFlowSumFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                TransposeFlowSum(Idx) = TransposeFlowSumFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxFlow(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxFlow(Idx) = TransposeFlowSum(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValFlow(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValFlow(Idx) = TransposeFlowSum(Idx)
                    Next
            End Select
        End Sub

        Private Function TransposeFlowSumFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Dim Col As Integer
            Dim ProportionSumOverTLOut As Single
            Dim TransposeFlow As Single
            Dim TransposeFlowSumOverGroup As Single

            TransposeFlowSumOverGroup = 0
            For Col = 1 To m_EPdata.NumLiving
                ProportionSumOverTLOut = ProportionSumOverTLOutFn(Col, Algor)
                TransposeFlow = TransposeFlowFn(Col, TrophicLevelOut, ProportionSumOverTLOut, Algor)
                If Not Single.IsNaN(TransposeFlow) Then
                    TransposeFlowSumOverGroup = TransposeFlowSumOverGroup + TransposeFlow
                End If
            Next
            Return TransposeFlowSumOverGroup
        End Function

        Private Sub FindKineticAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Kinetic(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            Kinetic(Idx) = KineticFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                Kinetic(Idx) = KineticFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxKinetic(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxKinetic(Idx) = Kinetic(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValKinetic(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValKinetic(Idx) = Kinetic(Idx)
                    Next
            End Select
        End Sub

        Private Function KineticFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return TransposeFlowSumFn(TrophicLevelOut, Algor) / _
            TransposeBiomassSumFn(TrophicLevelOut, Algor)
        End Function

        Private Sub FindTransposeCatchSumGpFltAry(ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim TransposeCatchSumGpFlt(ProportionSTD.GetUpperBound(0))


            Select Case IsCatchImport
                Case False
                    Idx = 1
                    TLOut = 1
                    TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(TLOut, Algor)

                    For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Idx = Idx + 1
                        TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(TLOut, Algor)
                    Next
                Case True
                    Idx = 1
                    TLOut = 1
                    TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(TLOut, Algor, IsCatchImport)

                    For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                        Idx = Idx + 1
                        TransposeCatchSumGpFlt(Idx) = TransposeCatchSumGpFltFn(TLOut, Algor, IsCatchImport)
                    Next
            End Select

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxCatches(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxCatches(Idx) = TransposeCatchSumGpFlt(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValCatches(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValCatches(Idx) = TransposeCatchSumGpFlt(Idx)
                    Next
            End Select
        End Sub

        Private Function TransposeCatchSumGpFltFn(ByVal TrophicLevelOut As Double, _
          ByVal Algor As String, Optional ByVal IsCatchImport As Boolean = False) As Single
            Dim TransposeCatchSumOverGroup As Single
            Dim TransposeCatchSumOverGroupFleet As Single
            Dim NumFleet As Integer

            Select Case IsCatchImport
                Case False
                    NumFleet = m_EPdata.NumFleet
                Case True
                    NumFleet = m_EcotrophManager.InputData.NumFleetImport
            End Select

            Select Case IsCatchImport
                Case False
                    TransposeCatchSumOverGroupFleet = 0
                    For FN As Integer = 1 To NumFleet
                        TransposeCatchSumOverGroup = TransposeCatchSumGpFn(TrophicLevelOut, FN, Algor)
                        If Not Single.IsNaN(TransposeCatchSumOverGroup) Then
                            TransposeCatchSumOverGroupFleet = TransposeCatchSumOverGroupFleet + TransposeCatchSumOverGroup
                        End If
                    Next
                    Return TransposeCatchSumOverGroupFleet
                Case True
                    TransposeCatchSumOverGroupFleet = 0
                    For FN As Integer = 1 To NumFleet
                        TransposeCatchSumOverGroup = TransposeCatchSumGpFn(TrophicLevelOut, FN, Algor, IsCatchImport)
                        If Not Single.IsNaN(TransposeCatchSumOverGroup) Then
                            TransposeCatchSumOverGroupFleet = TransposeCatchSumOverGroupFleet + TransposeCatchSumOverGroup
                        End If
                    Next
                    Return TransposeCatchSumOverGroupFleet
            End Select
        End Function

        Private Sub FindFishLossRateAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FishLossRate(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FishLossRate(Idx) = FishLossRateFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FishLossRate(Idx) = FishLossRateFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxFishLossRate(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxFishLossRate(Idx) = FishLossRate(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValFishLossRate(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValFishLossRate(Idx) = FishLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Function FishLossRateFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return TransposeCatchSumGpFltFn(TrophicLevelOut, Algor) / _
            TransposeFlowSumFn(TrophicLevelOut, Algor)
        End Function

        Private Sub FindAccessFishLossRateAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessFishLossRate(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessFishLossRate(Idx) = AccessFishLossRateFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessFishLossRate(Idx) = AccessFishLossRateFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxAccessFishLossRate(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxAccessFishLossRate(Idx) = AccessFishLossRate(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValAccessFishLossRate(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValAccessFishLossRate(Idx) = AccessFishLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Function AccessFishLossRateFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return TransposeCatchSumGpFltFn(TrophicLevelOut, Algor) / _
            (AccessBiomassSumFn(TrophicLevelOut, Algor) * KineticFn(TrophicLevelOut, Algor))
        End Function

        Private Sub FindNaturalLossRateAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim NaturalLossRate(ProportionSTD.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            NaturalLossRate(Idx) = NaturalLossRateFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                NaturalLossRate(Idx) = NaturalLossRateFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxNaturalLossRate(ProportionSTD.GetUpperBound(0) - 1)
                    For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                        OmniIdxNaturalLossRate(Idx) = NaturalLossRate(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValNaturalLossRate(ProportionSTD.GetUpperBound(0) - 1)
                    For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                        UserDefValNaturalLossRate(Idx) = NaturalLossRate(Idx)
                    Next
            End Select
        End Sub

        Private Function NaturalLossRateFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Dim TLOutNext As Double

            If TrophicLevelOut = 1 Then
                TLOutNext = 2
            Else
                TLOutNext = TrophicLevelOut + TL_INCRM
            End If
            Return CSng(Math.Log(TransposeFlowSumFn(TrophicLevelOut, Algor) / _
              TransposeFlowSumFn(TLOutNext, Algor)) / _
              (TLOutNext - TrophicLevelOut) - FishLossRateFn(TrophicLevelOut, Algor))
        End Function

        Private Sub FindFishMortalityAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim FishMortality(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            FishMortality(Idx) = FishMortalityFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                FishMortality(Idx) = FishMortalityFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxFishMortality(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxFishMortality(Idx) = FishMortality(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValFishMortality(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValFishMortality(Idx) = FishMortality(Idx)
                    Next
            End Select
        End Sub

        Private Function FishMortalityFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return TransposeCatchSumGpFltFn(TrophicLevelOut, Algor) / _
            TransposeBiomassSumFn(TrophicLevelOut, Algor)
        End Function

        Private Sub FindSelectivityAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Selectivity(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            Selectivity(Idx) = SelectivityFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                Selectivity(Idx) = SelectivityFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxSelectivity(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxSelectivity(Idx) = Selectivity(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValSelectivity(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValSelectivity(Idx) = Selectivity(Idx)
                    Next
            End Select
        End Sub

        Private Function SelectivityFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return AccessBiomassSumFn(TrophicLevelOut, Algor) / _
            TransposeBiomassSumFn(TrophicLevelOut, Algor)
        End Function

        Private Sub FindAccessFishMortalityAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim AccessFishMortality(ProportionSTD.GetUpperBound(0))

            Idx = 1
            TLOut = 1
            AccessFishMortality(Idx) = AccessFishMortalityFn(TLOut, Algor)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL Step TL_INCRM
                Idx = Idx + 1
                AccessFishMortality(Idx) = AccessFishMortalityFn(TLOut, Algor)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxAccessFishMortality(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        OmniIdxAccessFishMortality(Idx) = AccessFishMortality(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValAccessFishMortality(ProportionSTD.GetUpperBound(0))
                    For Idx = 1 To ProportionSTD.GetUpperBound(0)
                        UserDefValAccessFishMortality(Idx) = AccessFishMortality(Idx)
                    Next
            End Select
        End Sub

        Private Function AccessFishMortalityFn(ByVal TrophicLevelOut As Double, ByVal Algor As String) As Single
            Return FishMortalityFn(TrophicLevelOut, Algor) / _
            SelectivityFn(TrophicLevelOut, Algor)
        End Function

        Private Sub FindTimeAry(ByVal Algor As String)
            Dim Idx As Integer
            Dim TLOut As Double
            ReDim Time(ProportionSTD.GetUpperBound(0) - 1)

            Idx = 1
            TLOut = 1
            Time(Idx) = TimeFn(TLOut, Algor, Idx)

            For TLOut = TL_OUT_INIT To TL_OUT_FINAL - TL_INCRM Step TL_INCRM
                Idx = Idx + 1
                Time(Idx) = TimeFn(TLOut, Algor, Idx)
            Next

            Select Case Algor
                Case My.Resources.TREE_NODE_OMNI_IDX
                    ReDim OmniIdxTime(ProportionSTD.GetUpperBound(0) - 1)
                    For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                        OmniIdxTime(Idx) = Time(Idx)
                    Next
                Case My.Resources.TREE_NODE_USER_DEF_SIGMA
                    ReDim UserDefValTime(ProportionSTD.GetUpperBound(0) - 1)
                    For Idx = 1 To ProportionSTD.GetUpperBound(0) - 1
                        UserDefValTime(Idx) = Time(Idx)
                    Next
            End Select
        End Sub

        Private Function TimeFn(ByVal TrophicLevelOut As Double, ByVal Algor As String, _
          ByVal AryIdx As Integer) As Single
            Dim TLOutPrevious As Double

            Select Case TrophicLevelOut
                Case 1
                    Return 0
                Case 2
                    TLOutPrevious = 1
                Case Is > 2
                    TLOutPrevious = TrophicLevelOut - TL_INCRM
            End Select
            Return CSng(Time(AryIdx - 1) + ((TrophicLevelOut - TLOutPrevious) / KineticFn(TLOutPrevious, Algor)))
        End Function
#End Region 'Using omnivory index or user defined values
#End Region 'Helper methods

    End Class

End Namespace
