'==============================================================================
'
' $Log: IEconomicData.vb,v $
' Revision 1.5  2009/03/05 17:27:07  jeroens
' Added TimeStep
'
' Revision 1.4  2009/03/05 07:27:11  jeroens
' Implemented
'
' Revision 1.3  2009/01/24 17:46:16  joeb
' Added EmploymentValueByFleet and ProfitByFleet
'
' Revision 1.2  2009/01/22 17:38:36  jeroens
' Added  2 main economic values
'
' Revision 1.1  2009/01/21 19:25:00  jeroens
' Initial version
'
'==============================================================================

Namespace Core

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for exchanging Economic data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface IEconomicData

        ReadOnly Property TimeStep() As Integer
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

End Namespace ' Core
