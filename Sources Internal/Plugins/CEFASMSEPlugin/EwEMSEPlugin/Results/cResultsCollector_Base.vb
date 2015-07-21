Public MustInherit Class cResultsCollector_Base

    Protected m_nStrategies As Integer
    Protected m_ModelID As Integer
    Protected m_Yearly As Boolean

    Public MustOverride ReadOnly Property NumberOfTimeRecords As Integer

    Public MustOverride Sub Initialise(MSE As cMSE)

    Public MustOverride Sub Populate()

    Public MustOverride ReadOnly Property DataName As String

    Protected MustOverride ReadOnly Property DefaultValue As Object

    Protected MustOverride Sub SetDefaults(ByVal DefaultValue As Object)

    Public MustOverride ReadOnly Property Yearly As Boolean

    Public Sub New()

    End Sub

    Public ReadOnly Property nStrategies As Integer
        Get
            Return m_nStrategies
        End Get
    End Property

    Public ReadOnly Property ModelID As Integer
        Get
            Return m_ModelID
        End Get
    End Property

    Public Sub Init_for_iModel(iModel As Integer)
        m_ModelID = iModel
        SetDefaults(DefaultValue)
    End Sub

End Class
