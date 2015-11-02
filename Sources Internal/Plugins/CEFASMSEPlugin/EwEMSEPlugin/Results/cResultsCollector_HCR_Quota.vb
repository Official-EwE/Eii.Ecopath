Public MustInherit Class cResultsCollector_HCR_Quota
    Inherits cResultsCollector_2DArray

    Protected Overrides ReadOnly Property DefaultValue As Object
        Get
            Return -9999
        End Get
    End Property

    Public Overrides ReadOnly Property TotalAcrossFleets As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property TotalAcrossGroups As Boolean
        Get
            Return False
        End Get
    End Property



End Class
