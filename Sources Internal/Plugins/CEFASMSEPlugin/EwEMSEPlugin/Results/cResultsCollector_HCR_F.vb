Imports EwEUtils.Utilities

Public MustInherit Class cResultsCollector_HCR_F
    Inherits cResultsCollector_1DArray

    Public Sub New()

    End Sub

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return -9999
        End Get
    End Property

    Public Overrides ReadOnly Property Dim_Name As String
        Get
            Return "Groups"
        End Get
    End Property

    Public Overrides ReadOnly Property nElements As Integer
        Get
            Return m_MSE.Core.nGroups
        End Get
    End Property

    Public Overrides ReadOnly Property ElementName(iElement As Integer) As String
        Get
            Return m_MSE.Core.EcoPathGroupInputs(iElement).Name
        End Get
    End Property

    Public Overrides ReadOnly Property GetValue_Formatted4CSV(iStrategy As Integer, iElement As Integer, iTime As Integer) As Object
        Get
            Return cStringUtils.FormatNumber(GetValue(iStrategy, iElement, iTime))
        End Get
    End Property

End Class
