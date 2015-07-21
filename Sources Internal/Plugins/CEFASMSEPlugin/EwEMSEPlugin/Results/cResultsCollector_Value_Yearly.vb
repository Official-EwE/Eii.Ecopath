Public Class cResultsCollector_Value_Yearly
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

    Public Overrides ReadOnly Property NumberOfTimeRecords As Integer
        Get
            Return m_MSE.NYearsProject
        End Get
    End Property

    Public Overrides Sub Populate()
        Dim TempTotalValue As Double

        Dim StrategyIndex = m_MSE.Strategies.IndexOf(m_MSE.currentStrategy) + 1 'Adding 1 to make it a non-zero index
        For igrp = 1 To m_MSE.Core.nGroups
            For iFleet = 1 To m_MSE.Core.nFleets
                For iTime = 1 To NumberOfTimeRecords

                    TempTotalValue = 0
                    For iMonth = 1 To 12
                        TempTotalValue += m_MSE.LandingsThroughoutProjection(igrp, iFleet, (iTime - 1) * 12 + iMonth) * m_MSE.Core.FleetInputs(iFleet).OffVesselValue(igrp)
                    Next
                    TempTotalValue /= 12
                    Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = TempTotalValue

                    Me.SetValue(StrategyIndex, igrp, iFleet, iTime) = TempTotalValue
                    Me.SetValue(StrategyIndex, igrp, 0, iTime) = Me.GetValue(StrategyIndex, igrp, 0, iTime) + TempTotalValue 'Summing across fleets
                    Me.SetValue(StrategyIndex, 0, iFleet, iTime) = Me.GetValue(StrategyIndex, 0, iFleet, iTime) + TempTotalValue 'Summing across groups
                    Me.SetValue(StrategyIndex, 0, 0, iTime) = Me.GetValue(StrategyIndex, 0, 0, iTime) + TempTotalValue ' summ across both fleets and groups
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

    Public Overrides ReadOnly Property Yearly As Boolean
        Get
            Return True
        End Get
    End Property
End Class
