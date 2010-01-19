#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' This class represents a group of Consumers in the Ecost economic model.
''' Consumers form the end of economic flow chains.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public Class cConsumerUnit
    Inherits cUnit

    Public Sub New()
        MyBase.New()
    End Sub

#Region " Calculations "

#End Region ' Calculations

#Region " Properties "

    Public Overrides ReadOnly Property Category() As String
        Get
            Return "Consumer"
        End Get
    End Property

    <Browsable(True), _
        Category("Consumer"), _
        DisplayName("<placeholder>"), _
        Description("Consumer unit attribute, spare"), _
        cPropertySorter.PropertyOrder(1)> _
       Public Property ConsumerAttribute1() As Single
        Get
            Return 0.0
        End Get
        Set(ByVal value As Single)
            SetChanged()
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides ReadOnly Property UnitType() As cUnitFactory.eUnitType
        Get
            Return cUnitFactory.eUnitType.Consumer
        End Get
    End Property

#End Region ' Properties

End Class
