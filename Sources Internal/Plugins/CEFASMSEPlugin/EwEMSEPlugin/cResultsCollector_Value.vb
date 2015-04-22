Public Class cResultsCollector_Value
    Inherits cResultsCollector_2DArray

    Public Overrides ReadOnly Property DataName As String
        Get
            Return "Value"
        End Get
    End Property

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return 0
        End Get
    End Property

    Public Overrides Sub Populate()

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iFleet = 1 To m_MSE.Core.nFleets
                For iTime = 1 To NumberOfTimeRecords
                    Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) * m_MSE.Core.FleetInputs(iFleet).OffVesselValue(igrp)
                    Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) * m_MSE.Core.FleetInputs(iFleet).OffVesselValue(igrp) 'Summing across fleets
                    Me.SetValue(StrategyIndex, 0, iFleet, iTime) = Me.GetValue(StrategyIndex, 0, iFleet, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) * m_MSE.Core.FleetInputs(iFleet).OffVesselValue(igrp) 'Summing across groups
                    Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + m_MSE.LandingsThroughoutProjection(igrp, iFleet, iTime) * m_MSE.Core.FleetInputs(iFleet).OffVesselValue(igrp) ' summ across both fleets and groups
                Next
            Next
        Next

    End Sub

    Public Overrides ReadOnly Property TotalAcrossFleets As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TotalAcrossGroups As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject * m_MSE.EcosimData.NumStepsPerYear
        End Get
    End Property

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return False
        End Get
    End Property
End Class
