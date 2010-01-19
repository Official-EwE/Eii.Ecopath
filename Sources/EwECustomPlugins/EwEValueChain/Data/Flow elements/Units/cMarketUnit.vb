#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public Class cMarketUnit
    Inherits cEconomicUnit

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

    Protected Overrides Function Calculate(ByVal results As cResults, _
            ByVal sInputBiomass As Single, ByVal sInputValue As Single, _
            ByVal sOutputBiomass As Single, ByVal sOutputValue As Single, _
            ByVal iTimeStep As Integer) As Boolean

        Return MyBase.Calculate(results, sInputBiomass, sInputValue, sOutputBiomass, sOutputValue, iTimeStep)

    End Function

#End Region ' Calculations

#Region " Properties "

    <Browsable(True), _
        Category("Market"), _
        DisplayName("<placeholder>"), _
        Description("Market unit attribute, spare"), _
        cPropertySorter.PropertyOrder(1)> _
    Public Property MarketAttribute1() As Single
        Get
            Return 0.0
        End Get
        Set(ByVal value As Single)
            SetChanged()
        End Set
    End Property

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Market"
        End Get
    End Property

    <Browsable(False)> _
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Market
        End Get
    End Property

#End Region ' Properties

End Class
