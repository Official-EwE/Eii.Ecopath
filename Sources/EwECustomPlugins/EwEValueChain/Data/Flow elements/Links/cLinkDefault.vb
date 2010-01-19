#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Class for holding default link properties, used when forging new links 
''' between units in the flow.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)), _
    DefaultProperty("Name"), _
    Serializable()> _
Public Class cLinkDefault
    Inherits cEwEDatabase.cOOPStorable

#Region " Shared definitions "

    Protected Const cCATEGORY_GENERIC As String = "1. Generic"
    Protected Const cCATEGORY_TRANSFER As String = "2. Transfer"

#End Region ' Shared definitions

#Region " Privates "

    Private m_linkType As cLinkFactory.eLinkType = cLinkFactory.eLinkType.ProducerToProcessing
    ''' <summary>Link output biomass ratio.</summary>
    Private m_sBiomassRatio As Single = 1.0!
    ''' <summary>Link output value per ton.</summary>
    Private m_sValuePerTon As Single = 1.0!
    ''' <summary>Link output value ratio.</summary>
    Private m_sValueRatio As Single = 1

    ''' <summary>Flag stating whether this unit is allowed to broadcast change events.</summary>
    Private m_bAllowEvents As Boolean = True

#End Region ' Privates

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region ' Constructor

#Region " Properties "

    <Browsable(True), _
       Category(cCATEGORY_GENERIC), _
       DisplayName("Name"), _
       Description("Name of this link"), _
       cPropertySorter.PropertyOrder(1)> _
    Public Overridable Property Name() As String
        Get
            Return Me.m_linkType.ToString()
        End Get
        Set(ByVal strName As String)
            '
        End Set
    End Property

    <Browsable(False)> _
    Public Property LinkType() As Integer
        Get
            Return Me.m_linkType
        End Get
        Set(ByVal value As Integer)
            Me.m_linkType = DirectCast(value, cLinkFactory.eLinkType)
        End Set
    End Property

    <Browsable(True), _
     Category(cCATEGORY_TRANSFER), _
     DisplayName("Biomass ratio"), _
     Description("Ratio of biomass change (proportion, [0-1])"), _
     DefaultValue(1.0!), _
     cPropertySorter.PropertyOrder(1)> _
    Public Property BiomassRatio() As Single
        Get
            Return Me.m_sBiomassRatio
        End Get
        Set(ByVal value As Single)
            Me.m_sBiomassRatio = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True), _
        Category(cCATEGORY_TRANSFER), _
        DisplayName("Value per ton"), _
        Description("Value per ton"), _
        DefaultValue(1.0!), _
        cPropertySorter.PropertyOrder(2)> _
    Public Property ValuePerTon() As Single
        Get
            Return Me.m_sValuePerTon
        End Get
        Set(ByVal value As Single)
            Me.m_sValuePerTon = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True), _
        Category(cCATEGORY_TRANSFER), _
        DisplayName("Value ratio"), _
        Description("Value ratio, the ratio between value of product and value of raw material (the input to the previous box)"), _
        DefaultValue(1.0!), _
        cPropertySorter.PropertyOrder(3)> _
     Public Property ValueRatio() As Single
        Get
            Return Me.m_sValueRatio
        End Get
        Set(ByVal value As Single)
            Me.m_sValueRatio = value
            Me.SetChanged()
        End Set
    End Property

#End Region ' Properties

End Class
