#Region " Imports "

Option Strict On
Imports EwEPlugin.Data
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Data exposed by the Ecost plug-in for consumption by the outside world.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPluginData
    Implements IPluginData
    Implements IEconomicData

    Public Class cVCEconomicData
        Implements IEData

#Region " Private vars "

        Friend m_sCost As Single = 0
        Friend m_sCostInput As Single = 0
        Friend m_sCostLicenseObservers As Single = 0
        Friend m_sCostSalariesShares As Single = 0
        Friend m_sCostTaxes As Single = 0
        Friend m_sCostTotalInputOther As Single = 0
        Friend m_sNumberOfDependentsTotal As Single = 0
        Friend m_sNumberOfJobsFemaleTotal As Single = 0
        Friend m_sNumberOfJobsMaleTotal As Single = 0
        Friend m_sNumberOfJobsTotal As Single = 0
        Friend m_sNumberOfOwnerDependents As Single = 0
        Friend m_sNumberOfWorkerDependents As Single = 0
        Friend m_sProduction As Single = 0
        Friend m_sProductionLive As Single = 0
        Friend m_sProfit As Single = 0
        Friend m_sRevenueProductsMain As Single = 0
        Friend m_sRevenueProductsOther As Single = 0
        Friend m_sRevenueSubsidies As Single = 0
        Friend m_sRevenueTotal As Single = 0
        Friend m_sThroughput As Single = 0

#End Region ' Private vars

#Region " Properties "

        Public ReadOnly Property Cost() As Single _
            Implements IEData.Cost
            Get
                Return Me.m_sCost
            End Get
        End Property

        Public ReadOnly Property CostInput() As Single _
            Implements IEData.CostInput
            Get
                Return Me.m_sCostInput
            End Get
        End Property

        Public ReadOnly Property CostLicenseObservers() As Single _
            Implements IEData.CostLicenseObservers
            Get
                Return Me.m_sCostLicenseObservers
            End Get
        End Property

        Public ReadOnly Property CostSalariesShares() As Single _
            Implements IEData.CostSalariesShares
            Get
                Return Me.m_sCostSalariesShares
            End Get
        End Property

        Public ReadOnly Property CostTaxes() As Single _
            Implements IEData.CostTaxes
            Get
                Return Me.m_sCostTaxes
            End Get
        End Property

        Public ReadOnly Property CostTotalInputOther() As Single _
            Implements IEData.CostTotalInputOther
            Get
                Return Me.m_sCostTotalInputOther
            End Get
        End Property

        Public ReadOnly Property NumberOfDependentsTotal() As Single _
            Implements IEData.NumberOfDependentsTotal
            Get
                Return Me.m_sNumberOfDependentsTotal
            End Get
        End Property

        Public ReadOnly Property NumberOfJobsFemaleTotal() As Single _
            Implements IEData.NumberOfJobsFemaleTotal
            Get
                Return Me.m_sNumberOfJobsFemaleTotal
            End Get
        End Property

        Public ReadOnly Property NumberOfJobsMaleTotal() As Single _
            Implements IEData.NumberOfJobsMaleTotal
            Get
                Return Me.m_sNumberOfJobsMaleTotal
            End Get
        End Property

        Public ReadOnly Property NumberOfJobsTotal() As Single _
            Implements IEData.NumberOfJobsTotal
            Get
                Return Me.m_sNumberOfJobsTotal
            End Get
        End Property

        Public ReadOnly Property NumberOfOwnerDependents() As Single _
            Implements IEData.NumberOfOwnerDependents
            Get
                Return Me.m_sNumberOfOwnerDependents
            End Get
        End Property

        Public ReadOnly Property NumberOfWorkerDependents() As Single _
            Implements IEData.NumberOfWorkerDependents
            Get
                Return Me.m_sNumberOfWorkerDependents
            End Get
        End Property

        Public ReadOnly Property Production() As Single _
            Implements IEData.Production
            Get
                Return Me.m_sProduction
            End Get
        End Property

        Public ReadOnly Property ProductionLive() As Single _
            Implements IEData.ProductionLive
            Get
                Return Me.m_sProductionLive
            End Get
        End Property

        Public ReadOnly Property Profit() As Single _
            Implements IEData.Profit
            Get
                Return Me.m_sProfit
            End Get
        End Property

        Public ReadOnly Property RevenueProductsMain() As Single _
            Implements IEData.RevenueProductsMain
            Get
                Return Me.m_sRevenueProductsMain
            End Get
        End Property

        Public ReadOnly Property RevenueProductsOther() As Single _
            Implements IEData.RevenueProductsOther
            Get
                Return Me.m_sRevenueProductsOther
            End Get
        End Property

        Public ReadOnly Property RevenueSubsidies() As Single _
            Implements IEData.RevenueSubsidies
            Get
                Return Me.m_sRevenueSubsidies
            End Get
        End Property

        Public ReadOnly Property RevenueTotal() As Single _
            Implements IEData.RevenueTotal
            Get
                Return Me.m_sRevenueTotal
            End Get
        End Property

        Public ReadOnly Property Throughput() As Single _
            Implements IEData.Throughput
            Get
                Return Me.m_sThroughput
            End Get
        End Property

#End Region ' Properties

    End Class

#Region " Privates "

    Private m_strAssemblyName As String = ""
    Private m_strPluginName As String = ""
    Private m_totals As New cVCEconomicData()

    Friend m_runType As IRunType = Nothing
    Friend m_iTimeStep As Integer = 0
    Friend m_subtotals As New List(Of cVCEconomicData)

#End Region ' Privates

#Region " Constructor "

    Public Sub New(ByVal strAssemblyName As String, ByVal strPluginName As String)
        Me.m_strAssemblyName = strAssemblyName
        Me.m_strPluginName = strPluginName
    End Sub

#End Region ' Constructor

#Region " Helper methods "

    Friend Sub Resize(ByVal nFleets As Integer)
        Me.m_subtotals.Clear()
        For iFleet As Integer = 0 To nFleets - 1
            Me.m_subtotals.Add(New cVCEconomicData)
        Next
    End Sub

#End Region ' Helper methods

#Region " IPluginData implementation "

    Public ReadOnly Property AssemblyName() As String _
        Implements IPluginData.AssemblyName
        Get
            Return Me.m_strAssemblyName
        End Get
    End Property

    Public ReadOnly Property PluginName() As String _
        Implements IPluginData.PluginName
        Get
            Return Me.m_strPluginName
        End Get
    End Property

    Public ReadOnly Property RunType() As EwEUtils.Core.IRunType _
        Implements IPluginData.RunType
        Get
            Return m_runType
        End Get
    End Property

#End Region ' IPluginData implementation

#Region " IEconomicData implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the time step that this data represents.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property TimeStep() As Integer _
        Implements IEconomicData.TimeStep
        Get
            Return Me.m_iTimeStep
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a subtotal block.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Subtotal(ByVal iFleet As Integer) As EwEUtils.Core.IEData _
        Implements IEconomicData.Subtotal
        Get
            Debug.Assert(iFleet >= 0 And iFleet < Me.m_subtotals.Count)
            Return Me.m_subtotals(iFleet)
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the number of subtotal blocks.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NumSubtotals() As Integer _
        Implements EwEUtils.Core.IEconomicData.NumSubtotals
        Get
            Return Me.m_subtotals.Count
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get total economic values.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Total() As EwEUtils.Core.IEData _
    Implements EwEUtils.Core.IEconomicData.Total
        Get
            Return Me.m_totals
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the core counter that represents the grouping of subtotals.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property SubtotalCounter() As EwEUtils.Core.eCoreCounterTypes _
         Implements EwEUtils.Core.IEconomicData.SubtotalCounter
        Get
            Return eCoreCounterTypes.nFleets
        End Get
    End Property

#End Region ' IEconomicData implementation

End Class
