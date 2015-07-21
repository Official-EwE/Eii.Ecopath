Public MustInherit Class cResultsCollector_1DArray
    Inherits cResultsCollector_Base

    Private m_DataArray(,,) As Object
    Protected m_MSE As cMSE

    Public Sub New()
        MyBase.New()
    End Sub

    Public MustOverride ReadOnly Property Dim_Name As String

    Public MustOverride ReadOnly Property nElements As Integer

    Public MustOverride ReadOnly Property ElementName(iElement As Integer) As String

    Public MustOverride ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, ByVal iElement As Integer, iTime As Integer) As Object

    Public Overrides Sub Initialise(MSE As cMSE)
        m_MSE = MSE
        SetSize(MSE.Strategies.Count, nElements, NumberOfTimeRecords)
    End Sub

    Protected ReadOnly Property GetValue(ByVal iStrategy As Integer, ByVal iElement As Integer, ByVal iTime As Integer) As Object
        Get
            Return m_DataArray(iStrategy - 1, iElement - 1, iTime - 1)
        End Get
    End Property


    Protected WriteOnly Property SetValue(ByVal iStrategy As Integer, ByVal iElement As Integer, ByVal iTime As Integer) As Object
        Set(value As Object)
            m_DataArray(iStrategy - 1, iElement - 1, iTime - 1) = value
        End Set
    End Property

    Protected Overrides Sub SetDefaults(ByVal DefaultValue As Object)
        For iStrategy = 1 To m_nStrategies
            For iElement = 1 To nElements
                For iTime = 1 To NumberOfTimeRecords
                    Me.SetValue(iStrategy, iElement, iTime) = DefaultValue
                Next
            Next
        Next
    End Sub

    Protected Sub SetSize(nStrategy As Integer, nElement As Integer, nTime As Integer)
        ReDim m_DataArray(nStrategy - 1, nElement - 1, nTime - 1)
        m_nStrategies = nStrategy
    End Sub

End Class
