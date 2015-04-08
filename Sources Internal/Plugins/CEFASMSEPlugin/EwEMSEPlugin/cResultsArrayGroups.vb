Public MustInherit Class cResultsArrayGroups
    Inherits cBaseResultsArray

    Private test As Array
    Private m_DataArray(,,) As Object
    Protected m_MSE As cMSE
    Protected m_NumberOfTimeRecords As Integer
    Private m_nStrategy As Integer
    Private m_nGroup As Integer
    Private m_nTime As Integer

    Public Const Dim_Name_1 = "Strategy"
    Public Const Dim_Name_2 = "Group"
    Public Const Dim_Name_3 = "Time"

    Protected Property Value(ByVal iStrategy As Integer, ByVal iGroup As Integer, ByVal iTime As Integer) As Object
        Get
            Return m_DataArray(iStrategy - 1, iGroup - 1, iTime - 1)
        End Get
        Set(value As Object)
            m_DataArray(iStrategy - 1, iGroup - 1, iTime - 1) = value
        End Set
    End Property

    Protected Sub SetDefaults(ByVal DefaultValue As Object)
        For iStrategy = 1 To m_nStrategy
            For iGroup = 1 To m_nGroup
                For iTime = 1 To m_nTime
                    Me.Value(iStrategy, iGroup, iTime) = DefaultValue
                Next
            Next
        Next
    End Sub

    Protected Sub SetSize(nStrategy As Integer, nGroup As Integer, nTime As Integer)
        ReDim m_DataArray(nStrategy - 1, nGroup - 1, nTime - 1)
        m_nStrategy = nStrategy
        m_nGroup = nGroup
        m_nTime = nTime
    End Sub

End Class
