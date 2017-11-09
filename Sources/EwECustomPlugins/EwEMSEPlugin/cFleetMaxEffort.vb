Public Class cFleetMaxEffort

    Private m_iFleet As Integer
    Private m_max_effort As Single
    Private m_max_percentage_change_in_max_effort As Single

    Public ReadOnly Property MaxEffort() As Single
        Get
            Return m_max_effort
        End Get
    End Property

    Public Sub New(iFleet As Integer, start_effort As Single, max_percentage_change_in_max_effort As Single)

        m_iFleet = iFleet
        m_max_effort = start_effort + start_effort * max_percentage_change_in_max_effort
        m_max_percentage_change_in_max_effort = max_percentage_change_in_max_effort

    End Sub

    Public Sub UpdateLimit(end_previous_year_effort As Single)

        Dim max_reduction_in_max_effort As Single
        Dim min_increase_in_max_effort As Single

        max_reduction_in_max_effort = m_max_effort - m_max_effort * m_max_percentage_change_in_max_effort
        min_increase_in_max_effort = end_previous_year_effort + end_previous_year_effort * m_max_percentage_change_in_max_effort

        m_max_effort = Math.Max(max_reduction_in_max_effort, min_increase_in_max_effort)

    End Sub

End Class
