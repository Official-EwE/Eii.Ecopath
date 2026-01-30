' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.Utilities



<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cProcessingUnit
    Inherits cEconomicUnit

#Region " Private variables "

    Protected m_AgriculturalProducts As Single = 0.0
    Protected m_AgriculturalInput As Single = 0

#End Region

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

    Protected Overrides Function Calculate(results As cResults,
            sInputBiomass As Single, sInputValue As Single,
            sOutputBiomass As Single, sOutputValue As Single,
            iTimeStep As Integer) As Boolean

        Dim bSucces As Boolean = MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        ' ..but adds Agricultural costs
        bSucces = bSucces And Me.CalcAgriculturalCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        ' ..but adds Agricultural revenue from such products, should there by any
        bSucces = bSucces And Me.CalcAgriculturalProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

        ' JS 23 Apr 25: debugging for inconsistencies. No issues found here.
        'Console.WriteLine("{0} @{1} -> {2} > B > {3}, {4} > V {5}", Me.Name, iTimeStep, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue)

        Return bSucces

    End Function

    Protected Overridable Function CalcAgriculturalCost(result As cResults,
            sInputBiomass As Single, sInputValue As Single,
            sOutputBiomass As Single, sOutputValue As Single,
            iTimeStep As Integer) As Boolean

        Dim AgriCost As Single = sOutputBiomass * Me.AgriculturalInput
        result.Store(Me, cResults.eVariableType.CostAgriculture, AgriCost, iTimeStep)
        Return True

    End Function

    Protected Overridable Function CalcAgriculturalProducts(result As cResults,
            sInputBiomass As Single, sInputValue As Single,
            sOutputBiomass As Single, sOutputValue As Single,
            iTimeStep As Integer) As Boolean

        Dim AgriRevenue As Single = sOutputBiomass * Me.AgriculturalProducts
        result.Store(Me, cResults.eVariableType.RevenueAgriculture, AgriRevenue, iTimeStep)
        Return True

    End Function

#End Region ' Calculations

#Region " Properties "

    <Browsable(True),
        Category(sPROPCAT_PRODUCTS),
        DisplayName("Revenue (agricultural)"),
        Description("Revenue for agricultural products per tonnes of product"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(101)>
    Public Property AgriculturalProducts() As Single
        Get
            Return Me.m_AgriculturalProducts
        End Get
        Set(value As Single)
            Me.m_AgriculturalProducts = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(sPROPCAT_INPUTCOST),
        DisplayName("Cost (agricultural)"),
        Description("Agricultural input cost per tonnes of products"),
        DefaultValue(0.0!),
        cPropertySorter.PropertyOrder(102)>
    Public Property AgriculturalInput() As Single
        Get
            Return Me.m_AgriculturalInput
        End Get
        Set(value As Single)
            Me.m_AgriculturalInput = value
            Me.SetChanged()
        End Set
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Processing"
        End Get
    End Property

    <Browsable(False)>
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Processing
        End Get
    End Property

#End Region ' Properties

End Class
