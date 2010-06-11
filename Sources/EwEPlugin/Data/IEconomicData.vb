#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Economic data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IEData

        ReadOnly Property Production() As Single
        ReadOnly Property ProductionLive() As Single
        ReadOnly Property RevenueProductsMain() As Single
        ReadOnly Property RevenueProductsOther() As Single
        ReadOnly Property RevenueSubsidies() As Single
        ReadOnly Property RevenueTotal() As Single
        ReadOnly Property CostSalariesShares() As Single
        ReadOnly Property CostInput() As Single
        ReadOnly Property CostTotalInputOther() As Single
        ReadOnly Property CostTaxes() As Single
        ReadOnly Property CostLicenseObservers() As Single
        ReadOnly Property Cost() As Single
        ReadOnly Property Profit() As Single
        ReadOnly Property Throughput() As Single
        ReadOnly Property NumberOfJobsFemaleTotal() As Single
        ReadOnly Property NumberOfJobsMaleTotal() As Single
        ReadOnly Property NumberOfJobsTotal() As Single
        ReadOnly Property NumberOfWorkerDependents() As Single
        ReadOnly Property NumberOfOwnerDependents() As Single
        ReadOnly Property NumberOfDependentsTotal() As Single

    End Interface

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Economic data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IEconomicData

        ReadOnly Property TimeStep() As Integer

        ''' <summary>Get total economic values.</summary>
        ReadOnly Property Total() As IEData
        ''' <summary>Get a subtotal block.</summary>
        ReadOnly Property Subtotal(ByVal iFleet As Integer) As IEData
        ''' <summary>Get the number of subtotal blocks.</summary>
        ReadOnly Property NumSubtotals() As Integer
        ''' <summary>Get the core counter that represents the grouping of subtotals.</summary>
        ReadOnly Property SubtotalCounter() As eCoreCounterTypes

    End Interface

End Namespace ' Core
