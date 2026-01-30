' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwECore.Database
Imports EwEUtils.Utilities



''' ===========================================================================
''' <summary>
''' Class for holding default link properties, used when forging new links 
''' between units in the flow.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
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

    <Browsable(True),
       Category(cCATEGORY_GENERIC),
       DisplayName("Name"),
       Description("Name of this link"),
       cPropertySorter.PropertyOrder(1)>
    Public Overridable Property Name() As String
        Get
            Return ""
        End Get
        Set(strName As String)
            '
        End Set
    End Property

    <Browsable(False)>
    Public Property LinkType() As Integer
        Get
            Return Me.m_linkType
        End Get
        Set(value As Integer)
            Me.m_linkType = DirectCast(value, cLinkFactory.eLinkType)
        End Set
    End Property

    <Browsable(True),
     Category(cCATEGORY_TRANSFER),
     DisplayName("Biomass ratio"),
     Description("Ratio of biomass change (proportion, [0-1])"),
     DefaultValue(1.0!),
     cPropertySorter.PropertyOrder(1)>
    Public Overridable Property BiomassRatio() As Single
        Get
            Return Me.m_sBiomassRatio
        End Get
        Set(value As Single)
            Me.m_sBiomassRatio = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(cCATEGORY_TRANSFER),
        DisplayName("Value per ton"),
        Description("Value per ton"),
        DefaultValue(1.0!),
        cPropertySorter.PropertyOrder(2)>
    Public Overridable Property ValuePerTon() As Single
        Get
            Return Me.m_sValuePerTon
        End Get
        Set(value As Single)
            Me.m_sValuePerTon = value
            Me.SetChanged()
        End Set
    End Property

    <Browsable(True),
        Category(cCATEGORY_TRANSFER),
        DisplayName("Value ratio"),
        Description("Value ratio, the ratio between value of product and value of raw material (the input to the previous box)"),
        DefaultValue(1.0!),
        cPropertySorter.PropertyOrder(3)>
    Public Overridable Property ValueRatio() As Single
        Get
            Return Me.m_sValueRatio
        End Get
        Set(value As Single)
            Me.m_sValueRatio = value
            Me.SetChanged()
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a link is visible in the interface.
    ''' </summary>
    ''' <returns>True by default.</returns>
    ''' -----------------------------------------------------------------------
    Public Overridable Function IsVisible() As Boolean
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function ToString() As String
        Return Me.m_linkType.ToString()
    End Function

#End Region ' Properties

End Class
