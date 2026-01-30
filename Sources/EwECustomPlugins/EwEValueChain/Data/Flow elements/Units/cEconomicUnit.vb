' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities



<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public MustInherit Class cEconomicUnit
    Inherits cUnit

#Region " Private variables "

    Private m_WorkerFemale As Single = 0.0!
    Private m_WorkerMale As Single = 0.0!
    Private m_WorkerOther As Single = 0.0!
    Private m_WorkerMalePay As Single = 0.0!
    Private m_WorkerFemalePay As Single = 0.0!
    Private m_WorkerOtherPay As Single = 0.0!
    Private m_WorkerMaleShare As Single = 0.0!
    Private m_WorkerFemaleShare As Single = 0.0!
    Private m_WorkerOtherShare As Single = 0.0!
    Private m_WorkerMaleDependents As Single = 0.0!
    Private m_WorkerFemaleDependents As Single = 0.0!
    Private m_WorkerParttime As Single = 0.0!
    Private m_OwnerMale As Single = 0.0!
    Private m_OwnerFemale As Single = 0.0!
    Private m_OwnerMalePay As Single = 0.0!
    Private m_OwnerFemalePay As Single = 0.0!
    Private m_OwnerMaleShare As Single = 0.0!
    Private m_OwnerFemaleShare As Single = 0.0!
    Private m_OwnerMaleDependents As Single = 0
    Private m_OwnerFemaleDependents As Single = 0
    Private m_EnergyProducts As Single = 0
    Private m_IndustrialProducts As Single = 0
    Private m_ServiceProducts As Single = 0
    Private m_EnergyCost As Single = 0
    Private m_CapitalCost As Single = 0
    Private m_IndustrialCost As Single = 0
    Private m_ServiceCost As Single = 0
    Private m_ManagementCost As Single = 0
    Private m_RoyaltyCost As Single = 0
    Private m_CertificationCost As Single = 0
    Private m_TaxesLicense As Single = 0
    Private m_TaxesProfit As Single = 0
    Private m_TaxesVAT As Single = 0
    Private m_TaxesImport As Single = 0
    Private m_TaxesExport As Single = 0
    Private m_TaxesEnvironmental As Single = 0
    Private m_TaxesProduction As Single = 0
    Private m_SubsidyEnergy As Single = 0
    Private m_SubsidyOther As Single = 0

    'Public Amount As Single         'Amount in tonnes 
    'Public Benefit As Single        '
    'Public CapitalAmount As Single  'number of Capital units    VC DON'T THINK THIS IS NEEDED, ONLY PER TONNES
    'Public CapitalCost As Single    'per tons
    'Public EmployeesFemale As Single    'per tons
    'Public EmployeesMale As Single      'per tons
    'Public EmployersFemale As Single    'per tons
    'Public EmployersMale As Single      'per tons
    'Public LabourAmount As Single       'number of labour units per tons
    'Public LabourCost As Single         'per tons
    'Public ManagementCost As Single 'per unit produced?
    'Public ProductionUnits As Single     'Number of units per tons of product (boats, processors, distributors, etc)
    'Public RawAmount As Single      'Unit cost for buying a tonnes of raw material
    'Public RawCost As Single        'Unit cost for buying a tonnes of raw material
    'Public Price As Single          'Price for each product per tons
    'Public Revenue As Single        'Total revenue 
    'Public Subsidy As Single        'per tons produced
    'Public TaxEnvironmentalRate As Single   'per tonnes produced
    'Public TaxProductionRate As Single      'per tonnes produced
    'Public ProcessingCost As Single       'Cost for processing one tons (is in addition to raw cost)
    'Public Value As Single          'Value of production
    'Public WageFemale As Single     '$ per year
    'Public WageMale As Single       '$ per year

    Private m_bBroker As Boolean = False

#End Region ' Private variables

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor
    Public Overrides Sub InitRun(iSequence As Integer)
        MyBase.InitRun(iSequence)
        ' No state variables to clear here
    End Sub

#Region " Calculations "

    Protected Overrides Function Calculate(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        'The production unit needs to do the same calculations as the MyBase=cEconomicUnit, but:
        Dim bSucces As Boolean = MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        'Production in weight
        results.Store(Me, cResults.eVariableType.Production, sOutputBiomass, iTimeStep)

        bSucces = bSucces And Me.CalcProductionLiveWeight(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        'Revenue
        bSucces = bSucces And Me.CalcProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcSubsidy(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        'Cost
        bSucces = bSucces And Me.CalcRawmaterialCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcInputCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcManagementRoyaltyCertificationCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        bSucces = bSucces And Me.CalcTax(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcWorkerPay(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcOwnerPay(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        'Social
        bSucces = bSucces And Me.CalcWorkerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcWorkerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcWorkerParttime(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcWorkerOther(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcOwnerFemales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcOwnerMales(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcWorkerDependents(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        bSucces = bSucces And Me.CalcOwnerDependents(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        Return bSucces

    End Function

#Region " Production (weight)"

#End Region

    Protected Overridable Function CalcProductionLiveWeight(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim ToBeCalculated As Single = 0
        results.Store(Me, cResults.eVariableType.ProductionLive, ToBeCalculated, iTimeStep)

        Return True
    End Function

#Region " Revenue "

    Protected Overridable Function CalcProducts(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.EnergyProducts + Me.IndustrialProducts + Me.ServiceProducts)

        results.Store(Me, cResults.eVariableType.RevenueProductsOther, sSum, iTimeStep)
        If Me.Broker = False Then
            results.Store(Me, cResults.eVariableType.RevenueProductsMain, sOutputValue, iTimeStep)
        End If
        Return True
    End Function

    Protected Overridable Function CalcSubsidy(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.SubsidyEnergy + Me.SubsidyOther)
        results.Store(Me, cResults.eVariableType.RevenueSubsidies, sSum, iTimeStep)
        Return True
    End Function

#End Region ' Revenue

#Region " Cost "

    Protected Overridable Function CalcRawmaterialCost(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        If Me.Broker = False Then
            'Dim sSum As Single = sInputBiomass * sInputValue
            results.Store(Me, cResults.eVariableType.CostRawmaterial, sInputValue, iTimeStep)
        End If
        Return True
    End Function

    Protected Overridable Function CalcInputCost(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.CapitalInput + Me.EnergyCost + Me.IndustrialCost + Me.ServiceCost)
        results.Store(Me, cResults.eVariableType.CostInput, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcManagementRoyaltyCertificationCost(results As cResults,
               sInputBiomass As Single, sInputValue As Single,
               sOutputBiomass As Single, sOutputValue As Single,
               iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.m_ManagementCost + Me.m_RoyaltyCost + Me.m_CertificationCost)
        results.Store(Me, cResults.eVariableType.CostManagementRoyaltyCertification, sSum, iTimeStep)
        Return True
    End Function



    Protected Overridable Function CalcTax(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.TaxEnvironmental + Me.TaxExport + Me.TaxProduction + Me.TaxVAT + Me.m_TaxesImport + Me.LicenseTax)
        ' profit tax is calculated later, after all revenue and (other) cost is known (VC111117)
        results.Store(Me, cResults.eVariableType.CostTaxes, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcWorkerPay(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single
        If (Me.m_WorkerMalePay + Me.m_WorkerFemalePay) > 0 Then
            sSum = sOutputBiomass * (Me.m_WorkerMalePay + Me.m_WorkerFemalePay)
        Else
            sSum = sOutputValue * (Me.m_WorkerMaleShare + Me.m_WorkerFemaleShare) / 100
        End If
        results.Store(Me, cResults.eVariableType.CostWorker, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcOwnerPay(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single
        If (Me.m_OwnerMalePay + Me.m_OwnerFemalePay) > 0 Then
            sSum = sOutputBiomass * (Me.m_OwnerMalePay + Me.m_OwnerFemalePay)
        Else
            sSum = sOutputValue * (Me.m_OwnerMaleShare + Me.m_OwnerFemaleShare) / 100
        End If
        results.Store(Me, cResults.eVariableType.CostOwner, sSum, iTimeStep)
        Return True
    End Function

#End Region ' Cost

#Region " Social "

    Protected Overridable Function CalcWorkerFemales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_WorkerFemale
        results.Store(Me, cResults.eVariableType.NumberOfWorkerFemales, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcWorkerMales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_WorkerMale
        results.Store(Me, cResults.eVariableType.NumberOfWorkerMales, sSum, iTimeStep)

        Return True
    End Function

    Protected Overridable Function CalcWorkerParttime(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_WorkerParttime
        results.Store(Me, cResults.eVariableType.NumberOfWorkerPartTime, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcWorkerOther(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_WorkerOther
        results.Store(Me, cResults.eVariableType.NumberOfWorkerOther, sSum, iTimeStep)

        Return True
    End Function

    Protected Overridable Function CalcOwnerMales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_OwnerMale
        results.Store(Me, cResults.eVariableType.NumberOfOwnerMales, sSum, iTimeStep)

        Return True
    End Function

    Protected Overridable Function CalcOwnerFemales(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * Me.m_OwnerFemale
        results.Store(Me, cResults.eVariableType.NumberOfOwnerFemales, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcWorkerDependents(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.m_WorkerFemaleDependents * Me.m_WorkerFemale + Me.m_WorkerMaleDependents * Me.m_WorkerMale)
        results.Store(Me, cResults.eVariableType.NumberOfWorkerDependents, sSum, iTimeStep)
        Return True
    End Function

    Protected Overridable Function CalcOwnerDependents(results As cResults,
                sInputBiomass As Single, sInputValue As Single,
                sOutputBiomass As Single, sOutputValue As Single,
                iTimeStep As Integer) As Boolean

        Dim sSum As Single = sOutputBiomass * (Me.m_OwnerFemaleDependents * Me.m_OwnerFemale + Me.m_OwnerMaleDependents * Me.m_OwnerMale)
        results.Store(Me, cResults.eVariableType.NumberOfOwnerDependents, sSum, iTimeStep)
        Return True
    End Function

#End Region ' Social

#End Region ' Calculations

#Region " Properties "

#Region " Products "

    <Browsable(True),
        Category(sPROPCAT_PRODUCTS),
        DisplayName("Energy products"),
        Description("Energy products per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property EnergyProducts() As Single
        Get
            Return Me.m_EnergyProducts
        End Get
        Set(value As Single)
            Me.m_EnergyProducts = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PRODUCTS),
        DisplayName("Industrial products"),
        Description("Revenue of industrial products per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property IndustrialProducts() As Single
        Get
            Return Me.m_IndustrialProducts
        End Get
        Set(value As Single)
            Me.m_IndustrialProducts = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PRODUCTS),
        DisplayName("Service products"),
        Description("Revenue of services per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(4)>
    Public Property ServiceProducts() As Single
        Get
            Return Me.m_ServiceProducts
        End Get
        Set(value As Single)
            Me.m_ServiceProducts = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SUBSIDIES),
        DisplayName("Energy subsidy"),
        Description("Energy subsidy per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(1)>
    Public Property SubsidyEnergy() As Single
        Get
            Return Me.m_SubsidyEnergy
        End Get
        Set(value As Single)
            Me.m_SubsidyEnergy = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SUBSIDIES),
        DisplayName("Other subsidies"),
        Description("Other subsidies per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property SubsidyOther() As Single
        Get
            Return Me.m_SubsidyOther
        End Get
        Set(value As Single)
            Me.m_SubsidyOther = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
     Category(sPROPCAT_GENERAL),
     DisplayName("Broker"),
     Description("States whether this unit functions as a broker"),
     cPropertySorter.PropertyOrder(5)>
    Public Overridable Property Broker() As Boolean
        Get
            Return Me.m_bBroker
        End Get
        Set(value As Boolean)
            Me.m_bBroker = value
            Me.SetChanged()
        End Set
    End Property
#End Region ' Products

#Region " Pay "

    <Browsable(True),
         Category(sPROPCAT_PAY),
         DisplayName("Female worker pay"),
         Description("Female worker pay per tonnes of product"),
         DefaultValue(0.0!),
         cPropertySorter.PropertyOrder(1)>
    Public Property WorkerFemalePay() As Single
        Get
            Return Me.m_WorkerFemalePay
        End Get
        Set(value As Single)
            Me.m_WorkerFemalePay = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PAY),
        DisplayName("Male worker pay"),
        Description("Male worker pay per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property WorkerMalePay() As Single
        Get
            Return Me.m_WorkerMalePay
        End Get
        Set(value As Single)
            Me.m_WorkerMalePay = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PAY),
        DisplayName("Female owners pay"),
        Description("Female owners pay per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property OwnerFemalePay() As Single
        Get
            Return Me.m_OwnerFemalePay
        End Get
        Set(value As Single)
            Me.m_OwnerFemalePay = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PAY),
        DisplayName("Male owners pay"),
        Description("Male owners pay per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(4)>
    Public Property OwnerMalePay() As Single
        Get
            Return Me.m_OwnerMalePay
        End Get
        Set(value As Single)
            Me.m_OwnerMalePay = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_PAY),
        DisplayName("Other worker pay"),
        Description("Other worker pay per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(10)>
    Public Property WorkerOtherPay() As Single
        Get
            Return Me.m_WorkerOtherPay
        End Get
        Set(value As Single)
            Me.m_WorkerOtherPay = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Pay

#Region " Share "

    <Browsable(True),
         Category(sPROPCAT_SHARE),
         DisplayName("Female worker share"),
         Description("Female worker share in % of revenue"),
         DefaultValue(0.0!),
         cPropertySorter.PropertyOrder(1)>
    Public Property WorkerFemaleshare() As Single
        Get
            Return Me.m_WorkerFemaleShare
        End Get
        Set(value As Single)
            Me.m_WorkerFemaleShare = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SHARE),
        DisplayName("Male worker share"),
        Description("Male worker share in % of revenue"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property WorkerMaleshare() As Single
        Get
            Return Me.m_WorkerMaleShare
        End Get
        Set(value As Single)
            Me.m_WorkerMaleShare = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SHARE),
        DisplayName("Female owners share"),
        Description("Female owners share in % of revenue"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property OwnerFemaleshare() As Single
        Get
            Return Me.m_OwnerFemaleShare
        End Get
        Set(value As Single)
            Me.m_OwnerFemaleShare = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SHARE),
        DisplayName("Male owners share"),
        Description("Male owners share in % of revenue"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(4)>
    Public Property OwnerMaleshare() As Single
        Get
            Return Me.m_OwnerMaleShare
        End Get
        Set(value As Single)
            Me.m_OwnerMaleShare = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Share

#Region " Input cost "

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Capital cost"),
        Description("Capital cost per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property CapitalInput() As Single
        Get
            Return Me.m_CapitalCost
        End Get
        Set(value As Single)
            Me.m_CapitalCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Energy cost"),
        Description("Energy cost per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property EnergyCost() As Single
        Get
            Return Me.m_EnergyCost
        End Get
        Set(value As Single)
            Me.m_EnergyCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Industrial cost"),
        Description("Industrial cost per tonnes of product"),
        DefaultValue(0),
        cPropertySorter.PropertyOrder(4)>
    Public Property IndustrialCost() As Single
        Get
            Return Me.m_IndustrialCost
        End Get
        Set(value As Single)
            Me.m_IndustrialCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Services cost"),
        Description("Services cost per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(5)>
    Public Property ServiceCost() As Single
        Get
            Return Me.m_ServiceCost
        End Get
        Set(value As Single)
            Me.m_ServiceCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Management cost"),
        Description("Management cost per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(6)>
    Public Property ManagementCost() As Single
        Get
            Return Me.m_ManagementCost
        End Get
        Set(value As Single)
            Me.m_ManagementCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
    Category(sPROPCAT_INPUTCOST),
    DisplayName("Royalty cost"),
    Description("Royalty cost per tonnes of product"),
    DefaultValue(0.0!),
    cPropertySorter.PropertyOrder(7)>
    Public Property RoyaltyCost() As Single
        Get
            Return Me.m_RoyaltyCost
        End Get
        Set(value As Single)
            Me.m_RoyaltyCost = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
    Category(sPROPCAT_INPUTCOST),
    DisplayName("Certification cost"),
    Description("Certification cost per tonnes of product"),
    DefaultValue(0.0!),
    cPropertySorter.PropertyOrder(8)>
    Public Property CertificationCost() As Single
        Get
            Return Me.m_CertificationCost
        End Get
        Set(value As Single)
            Me.m_CertificationCost = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Input

#Region " Taxes "

    <Browsable(True),
        Category(sPROPCAT_TAXES),
        DisplayName("Environmental tax"),
        Description("Environmental tax per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(1)>
    Public Property TaxEnvironmental() As Single
        Get
            Return Me.m_TaxesEnvironmental
        End Get
        Set(value As Single)
            Me.m_TaxesEnvironmental = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
         Category(sPROPCAT_TAXES),
        DisplayName("Export tax"),
         Description("Export tax per tonnes of product"),
         DefaultValue(0.0!),
         cPropertySorter.PropertyOrder(2)>
    Public Property TaxExport() As Single
        Get
            Return Me.m_TaxesExport
        End Get
        Set(value As Single)
            Me.m_TaxesExport = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_TAXES),
        DisplayName("Import tax"),
        Description("Import tax per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property TaxImport() As Single
        Get
            Return Me.m_TaxesImport
        End Get
        Set(value As Single)
            Me.m_TaxesImport = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_TAXES),
        DisplayName("Production tax"),
        Description("Production tax per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(4)>
    Public Property TaxProduction() As Single
        Get
            Return Me.m_TaxesProduction
        End Get
        Set(value As Single)
            Me.m_TaxesProduction = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
         Category(sPROPCAT_TAXES),
         DisplayName("VAT tax"),
         Description("VAT tax per tonnes of product"),
         DefaultValue(0.0!),
         cPropertySorter.PropertyOrder(6)>
    Public Property TaxVAT() As Single
        Get
            Return Me.m_TaxesVAT
        End Get
        Set(value As Single)
            Me.m_TaxesVAT = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_TAXES),
        DisplayName("Profit tax (prop.)"),
        Description("Tax as proportion of profit"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(6)>
    Public Property ProfitTax() As Single
        Get
            Return Me.m_TaxesProfit
        End Get
        Set(value As Single)
            Me.m_TaxesProfit = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_TAXES),
        DisplayName("License tax"),
        Description("License tax per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(7)>
    Public Property LicenseTax() As Single
        Get
            Return Me.m_TaxesLicense
        End Get
        Set(value As Single)
            Me.m_TaxesLicense = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Taxes

#Region " Social "

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No. female workers"),
        Description("Number of female workers per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(1)>
    Public Property WorkerFemale() As Single
        Get
            Return Me.m_WorkerFemale
        End Get
        Set(value As Single)
            Me.m_WorkerFemale = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No. male workers"),
        Description("Number of male workers per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Property WorkerMale() As Single
        Get
            Return Me.m_WorkerMale
        End Get
        Set(value As Single)
            Me.m_WorkerMale = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No part-time workers"),
        Description("Number of part-time workers per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Property WorkerParttime() As Single
        Get
            Return Me.m_WorkerParttime
        End Get
        Set(value As Single)
            Me.m_WorkerParttime = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No. other workers"),
        Description("Number of other workers per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(4)>
    Public Property WorkerOther() As Single
        Get
            Return Me.m_WorkerOther
        End Get
        Set(value As Single)
            Me.m_WorkerOther = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No. female owners"),
        Description("Number of female owners per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(10)>
    Public Property OwnerFemale() As Single
        Get
            Return Me.m_OwnerFemale
        End Get
        Set(value As Single)
            Me.m_OwnerFemale = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("No. male owners"),
        Description("Number of male owners per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(11)>
    Public Property OwnerMale() As Single
        Get
            Return Me.m_OwnerMale
        End Get
        Set(value As Single)
            Me.m_OwnerMale = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("Female worker dependents"),
        Description("Number of dependents per female worker"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(20)>
    Public Property WorkerFemaleDependents() As Single
        Get
            Return Me.m_WorkerFemaleDependents
        End Get
        Set(value As Single)
            Me.m_WorkerFemaleDependents = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("Male worker dependents"),
        Description("Number of dependents per male worker"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(21)>
    Public Property WorkerMaleDependents() As Single
        Get
            Return Me.m_WorkerMaleDependents
        End Get
        Set(value As Single)
            Me.m_WorkerMaleDependents = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("Female owner dependents"),
        Description("Number of dependents per female owner"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(30)>
    Public Property OwnerFemaleDependents() As Single
        Get
            Return Me.m_OwnerFemaleDependents
        End Get
        Set(value As Single)
            Me.m_OwnerFemaleDependents = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_SOCIAL),
        DisplayName("Male owner dependents"),
        Description("Number of dependents per male owner"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(31)>
    Public Property OwnerMaleDependents() As Single
        Get
            Return Me.m_OwnerMaleDependents
        End Get
        Set(value As Single)
            Me.m_OwnerMaleDependents = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Social

#End Region ' Properties

End Class
