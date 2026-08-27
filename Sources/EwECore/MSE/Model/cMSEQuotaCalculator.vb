' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright � 1991� Ecopath International Initiative (EII)

Namespace MSE

    ''' <summary>
    ''' Computes the yearly MSE quota per group and shares it across fleets.
    ''' Extracted from <see cref="cMSE"/> and free of any core/plugin dependency.
    ''' </summary>
    Public Class cMSEQuotaCalculator
        Implements IMSEQuotaCalculator

        Private m_data As IMSEQuotaData 'This data is not passed in the constructor to make it possible to use this class with Dependency Injection (DI)

        Private ReadOnly m_stockRecruitment As IMSEStockRecruitment

        Public Sub New(stockRecruitment As IMSEStockRecruitment)
            Me.m_stockRecruitment = stockRecruitment
        End Sub

        Public WriteOnly Property Data() As IMSEQuotaData Implements IMSEQuotaCalculator.Data
            Set(value As IMSEQuotaData)
                Me.m_data = value
            End Set
        End Property

        ''' <summary>
        ''' Estimate biomass per living group via the stock-recruitment model and store it in <see cref="cMSEDataStructures.Bestimate"/>.
        ''' </summary>
        ''' <param name="Biomass">Biomass by group calculated by Ecosim.</param>
        ''' <param name="curYear">Current MSE year index.</param>
        ''' <param name="randomNormal">Supplies a normally distributed random number (mean 0, std 1).</param>
        Public Sub DoAssessment(Biomass() As Single, curYear As Integer, randomNormal As Func(Of Single)) Implements IMSEQuotaCalculator.DoAssessment

            Dim Bobs() As Single
            ReDim Bobs(Me.m_data.nGroups)
            For i As Integer = 1 To Me.m_data.nLiving
                Bobs(i) = Biomass(i) * CSng(Math.Exp(Me.m_data.CVbiomEst(i) * randomNormal()))
                Me.m_data.Bestimate(i) = Me.m_stockRecruitment.StockRecruitment(i, Biomass(i), Bobs(i), Me.m_data.Bestimate(i), curYear)
            Next i

        End Sub

        ''' <summary>
        ''' Set the quota, apply uncertainty and share it between the fleets. Returns the quota by group.
        ''' </summary>
        ''' <param name="randomNormal">Supplies a normally distributed random number (mean 0, std 1).</param>
        Public Function UpdateQuotas(randomNormal As Func(Of Single)) As Single() Implements IMSEQuotaCalculator.UpdateQuotas
            Dim iflt As Integer, igrp As Integer
            Dim tQuota() As Single

            ReDim tQuota(Me.m_data.nGroups)
            Array.Clear(Me.m_data.FTarget, 0, Me.m_data.nGroups)
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'HACK WARNING
            'BatchMode (cMSEBatchManager) needs to be able to set FixedF() and TAC() values to zero and still have them considered a valid value
            'It does this by setting values to Epsilon 1.401298E-45 when the user enters zero
            'This is interpreted as >0 then rounded off to zero
            'this allows the interface and database to remain the same Zero means TAC() and FixedF() are NOT USED.
            'It would be tricky to fix this with a flag and not break existing models.
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            '
            '1 Set the quota via Fixed Escapement, Fixed Fishing Mortality or Target Fishing Mortality(hockey stick)
            '2 Apply uncertainty to the Quota
            '3 Share the Quota between the fleets
            For igrp = 1 To Me.m_data.nLiving

                If Me.m_data.TAC(igrp) > 0 Then
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'Total Allowable Catch
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    Dim tac As Single = CSng(Math.Round(Me.m_data.TAC(igrp), 5))
                    tQuota(igrp) = tac

                ElseIf Me.m_data.FixedEscapement(igrp) > 0 Then
                    'xxxxxxxxxxxxxxxxxxxxxxx
                    'Fixed Escapement
                    'xxxxxxxxxxxxxxxxxxxxxxx

                    tQuota(igrp) = Me.m_data.Bestimate(igrp) - Me.m_data.FixedEscapement(igrp)
                    If tQuota(igrp) < 0 Then tQuota(igrp) = 0

                ElseIf Me.m_data.FixedF(igrp) > 0 Then
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxx
                    'Fixed Mortality
                    'xxxxxxxxxxxxxxxxxxxxxxxxxxx
                    Dim f As Single = CSng(Math.Round(Me.m_data.FixedF(igrp), 5))
                    tQuota(igrp) = f * Me.m_data.Bestimate(igrp)
                    Me.m_data.FTarget(igrp) = f

                Else
                    'xxxxxxxxxxxxxxxxxxxxxxxx
                    'Target Fishing Mortality
                    'xxxxxxxxxxxxxxxxxxxxxxxx
                    Dim brange As Single = Me.m_data.Bbase(igrp) - Me.m_data.Blim(igrp)
                    If brange <= 0 Then brange = 1.0E-20

                    'VC to JB: I think the Biomass below should be Bestimate instead; talked to Carl and he agrees. will be a double wham, which is OK.
                    Me.m_data.FTarget(igrp) = Me.m_data.Fopt(igrp) * (Me.m_data.Bestimate(igrp) - Me.m_data.Blim(igrp)) / brange

                    'constrain the fishing mortality to min and max values. 
                    'Fmin(igrp) only gets set by the MSEBatchManager for all other runs it must be zero. 
                    If Me.m_data.FTarget(igrp) < Me.m_data.Fmin(igrp) Then Me.m_data.FTarget(igrp) = Me.m_data.Fmin(igrp)
                    If Me.m_data.FTarget(igrp) > Me.m_data.Fopt(igrp) Then Me.m_data.FTarget(igrp) = Me.m_data.Fopt(igrp)

                    tQuota(igrp) = Me.m_data.FTarget(igrp) * Me.m_data.Bestimate(igrp)

                End If

                'Add uncertainty to the Quota set above
                'VC091104 There will also be uncertainty on how well this quota is implemented so add this:
                'but assume uncertainty is smaller?????? not done here
                tQuota(igrp) = tQuota(igrp) * CSng(Math.Exp(Me.m_data.CVbiomEst(igrp) * randomNormal() - 0.5 * Me.m_data.CVbiomEst(igrp) ^ 2))

            Next igrp

            'Share the Quota across the fleets for this timestep
            For iflt = 1 To Me.m_data.nFleets   'nFleets holds the same value as cEcoSimDataStructures.nGear. It's set in cEcoSimModel.SetCounters
                For igrp = 1 To Me.m_data.nGroups
                    Me.m_data.QuotaTime(iflt, igrp) = tQuota(igrp) * Me.m_data.Quotashare(iflt, igrp)
                Next
            Next

            Return tQuota

        End Function

    End Class

End Namespace
