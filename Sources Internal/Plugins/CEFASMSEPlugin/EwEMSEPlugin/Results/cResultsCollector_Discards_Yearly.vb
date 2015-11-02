Public Class cResultsCollector_Discards_Yearly
    Inherits cResultsCollector_Catch

    Public Overrides ReadOnly Property ResultsThroughProjection(iGrp As Integer, iFleet As Integer, iTime As Integer) As Double
        Get
            Dim TempTotalGroupFleetLandingRate As Double = 0
            For iMonth = 1 To 12
                TempTotalGroupFleetLandingRate += m_MSE.DiscardsThroughoutProjection(iGrp, iFleet, (iTime - 1) * 12 + iMonth)
            Next
            TempTotalGroupFleetLandingRate /= 12
            Return TempTotalGroupFleetLandingRate
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get

            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Discard Rate (" & ScientificInterfaceShared.My.Resources.UNIT_CURRENCY_WETWEIGHT & "/year)"
        End Get
    End Property

    'Public Overrides Sub Populate()

    'Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
    'For igrp = 1 To m_MSE.Core.nGroups
    '    For iFleet = 1 To m_MSE.Core.nFleets
    '        For iTime = 1 To NumberOfTimeRecords
    '            Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = m_MSE.DiscardsThroughoutProjection(igrp, iFleet, iTime)
    '        Next
    '    Next
    'Next
    'End Sub

    Public Overrides ReadOnly Property FileNamePrefix As String
        Get
            Return "DiscardsYearly_"
        End Get
    End Property

End Class
