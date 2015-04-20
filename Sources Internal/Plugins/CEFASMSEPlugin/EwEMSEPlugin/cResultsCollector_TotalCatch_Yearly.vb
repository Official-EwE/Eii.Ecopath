Public Class cResultsCollector_TotalCatch_Yearly
    Inherits cResultsCollector_Catch

    'Public Overrides Sub Populate()

    '    Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
    '    Dim TempTotalGroupFleetCatchRate As Double

    '    For iTime = 1 To m_nTimeRecords
    '        For igrp = 1 To m_MSE.Core.nGroups
    '            For iFleet = 1 To m_MSE.Core.nFleets
    '                TempTotalGroupFleetCatchRate = 0

    '                For iMonth = 1 To 12
    '                    TempTotalGroupFleetCatchRate += m_MSE.CatchesThroughoutProjection(igrp, iFleet, (iTime - 1) * 12 + iMonth)
    '                Next
    '                TempTotalGroupFleetCatchRate /= 12

    '                Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = TempTotalGroupFleetCatchRate
    '                Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + TempTotalGroupFleetCatchRate 'Summing across fleets
    '                Me.SetValue(StrategyIndex, 0, iFleet, iTime) = Me.GetValue(StrategyIndex, 0, iFleet, iTime) + TempTotalGroupFleetCatchRate 'Summing across groups
    '                Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + TempTotalGroupFleetCatchRate ' summ across both fleets and groups
    '            Next
    '        Next
    '    Next

    'End Sub

    Public Overloads Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupFleetDiscardRate As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupFleetDiscardRate += m_MSE.CatchesThroughoutProjection(iGrp, iFleet, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupFleetDiscardRate /= 12
            Return TempTotalGroupFleetDiscardRate
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Catch Rate (" & ScientificInterfaceShared.My.Resources.UNIT_CURRENCY_WETWEIGHT & "/year)"
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property
End Class
