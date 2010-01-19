#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
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

    Protected Overrides Function Calculate(ByVal results As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        Dim bSucces As Boolean = MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        ' ..but adds Agricultural costs
        bSucces = bSucces And Me.CalcAgriculturalCost(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        ' ..but adds Agricultural revenue from such products, should there by any
        bSucces = bSucces And Me.CalcAgriculturalProducts(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)
        Return bSucces

    End Function

    Protected Overridable Function CalcAgriculturalCost(ByVal result As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        Dim AgriCost As Single = sOutputBiomass * Me.AgriculturalInput
        result.Store(Me, cResults.eVariableType.CostAgriculture, AgriCost, iTimeStep)
        Return True

    End Function

    Protected Overridable Function CalcAgriculturalProducts(ByVal result As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        Dim AgriRevenue As Single = sOutputBiomass * Me.AgriculturalProducts
        result.Store(Me, cResults.eVariableType.RevenueAgriculture, AgriRevenue, iTimeStep)
        Return True

    End Function

#End Region ' Calculations

#Region " Properties "

    <Browsable(True), _
        Category(sPROPCAT_PRODUCTS), _
        DisplayName("Revenue (agricultural)"), _
        Description("Revenue for agricultural products per tonnes of product"), _
        DefaultValue(0.0!), _
        cPropertySorter.PropertyOrder(101)> _
        Public Property AgriculturalProducts() As Single
        Get
            Return m_AgriculturalProducts
        End Get
        Set(ByVal value As Single)
            m_AgriculturalProducts = value
            SetChanged()
        End Set
    End Property

    <Browsable(True), _
        Category(sPROPCAT_INPUTCOST), _
        DisplayName("Cost (agricultural)"), _
        Description("Agricultural input cost per tonnes of products"), _
        DefaultValue(0.0!), _
        cPropertySorter.PropertyOrder(102)> _
        Public Property AgriculturalInput() As Single
        Get
            Return m_AgriculturalInput
        End Get
        Set(ByVal value As Single)
            m_AgriculturalInput = value
            SetChanged()
        End Set
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Processing"
        End Get
    End Property

    <Browsable(False)> _
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Processing
        End Get
    End Property

#End Region ' Properties

End Class
