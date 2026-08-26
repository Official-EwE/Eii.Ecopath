' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Stock assessment / recruitment biomass estimator used by the MSE.
    ''' Extracted from <see cref="cMSE"/> and free of any core/plugin dependency.
    ''' </summary>
    Public Class cMSEStockRecruitment
        Implements IMSEStockRecruitment

        Private ReadOnly m_data As IMSEQuotaData

        Public Sub New(data As IMSEQuotaData)
            Me.m_data = data
        End Sub

        Public Function StockRecruitment(iGroup As Integer, B As Single, BioEst As Single, Blast As Single, iCurYear As Integer) As Single Implements IMSEStockRecruitment.StockRecruitment
            'B is the biomass calculated by Ecosim
            'BioEst is the observed biomass(Ecosim biomass + random variation)
            'Blast is the biomass predicted for the last timestep ( Blast = stockRecruitment(t-1) )

            Dim RstockPred As Single
            Dim vPred As Single
            Dim Best As Single
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ' What this correction basically does is to increase the year-to-year Biomass gain factor in the delaydifference model (effective GstockPred by year)
            ' for situations where F has been reduced relative to ecopath base, and reduce the factor for years when F is higher than ecopath base.  
            'In the original code, we were just doing a factor reduction based on current F (catchyeargroup/Blast), without correcting relative to the ecopath base value of GstockPred.
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Me.m_data.BestimateLast(iGroup) = Blast * CSng(Math.Exp(-Me.m_Search.CatchYearGroup(iGroup) / Blast)) 
            Me.m_data.BestimateLast(iGroup) = Blast * CSng(Math.Exp(-Me.m_data.CatchYearGroup(iGroup) / Blast + Me.m_data.Fish1(iGroup)))
            Me.m_data.CatchYearGroup(iGroup) = 0

            RstockPred = CSng(Me.m_data.Rmax(iGroup) * Me.m_data.BestimateLast(iGroup) / (Me.m_data.BhalfT(iGroup) + Me.m_data.BestimateLast(iGroup)))
            vPred = CSng((Me.m_data.RstockRatio(iGroup) * Me.m_data.cvRec(iGroup)) ^ 2 / (1 - Me.m_data.GstockPred(iGroup) ^ 2))
            Me.m_data.KalmanGain(iGroup) = CSng(vPred / (vPred + Me.m_data.CVbiomEst(iGroup) ^ 2))

            'and then we estimate a biomass from assessments, so Bestimate is what will be used for e.g., the fixed escapement policy.
            'VC091107 fixed problem in eq below
            Best = Me.m_data.KalmanGain(iGroup) * BioEst + (1 - Me.m_data.KalmanGain(iGroup)) * (Me.m_data.GstockPred(iGroup) * Me.m_data.BestimateLast(iGroup) + RstockPred)

            'store the pred/actual
            Dim val As Single
            val = Best / B
            Me.m_data.BioEstStats.AddValue(iGroup, iCurYear, val)

            Return Best

        End Function

    End Class

End Namespace
