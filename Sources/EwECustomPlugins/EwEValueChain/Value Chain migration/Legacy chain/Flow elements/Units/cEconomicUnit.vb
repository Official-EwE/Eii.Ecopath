' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace ValueChainMigrator.LegacyData

    <Obsolete(), Serializable()>
    Public MustInherit Class cEconomicUnit
        Inherits cUnit

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
        End Sub

        Public Property EnergyProducts() As Single

        Public Property IndustrialProducts() As Single

        Public Property ServiceProducts() As Single

        Public Property SubsidyEnergy() As Single

        Public Property SubsidyOther() As Single

        Public Overridable Property Broker() As Boolean

        Public Property WorkerFemalePay() As Single

        Public Property WorkerMalePay() As Single

        Public Property OwnerFemalePay() As Single

        Public Property OwnerMalePay() As Single

        Public Property WorkerOtherPay() As Single

        Public Property WorkerFemaleshare() As Single

        Public Property WorkerMaleshare() As Single

        Public Property OwnerFemaleshare() As Single

        Public Property OwnerMaleshare() As Single

        Public Property CapitalInput() As Single

        Public Property EnergyCost() As Single

        Public Property IndustrialCost() As Single

        Public Property ServiceCost() As Single

        Public Property ManagementCost() As Single

        Public Property RoyaltyCost() As Single

        Public Property CertificationCost() As Single

        Public Property TaxEnvironmental() As Single

        Public Property TaxExport() As Single

        Public Property TaxImport() As Single

        Public Property TaxProduction() As Single

        Public Property TaxVAT() As Single

        Public Property ProfitTax() As Single

        Public Property LicenseTax() As Single

        Public Property WorkerFemale() As Single


        Public Property WorkerMale() As Single

        Public Property WorkerParttime() As Single

        Public Property WorkerOther() As Single

        Public Property OwnerFemale() As Single

        Public Property OwnerMale() As Single

        Public Property WorkerFemaleDependents() As Single

        Public Property WorkerMaleDependents() As Single

        Public Property OwnerFemaleDependents() As Single

        Public Property OwnerMaleDependents() As Single

    End Class

End Namespace