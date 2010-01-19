#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwEUtils.Utilities
Imports EwEUtils.Database.cEwEDatabase
Imports System.Reflection

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Position of a single unit in a flow diagram.
''' </summary>
''' ===========================================================================
Public Class cFlowPosition
    Inherits cOOPStorable

#Region " Private vars "

    Private m_diagram As cFlowDiagram = Nothing
    Private m_unit As cUnit = Nothing

    Private m_iX As Integer = 0
    Private m_iY As Integer = 0
    Private m_iWidth As Integer = 0
    Private m_iHeight As Integer = 0

#End Region ' Private vars

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the diagram this flow position belongs to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Diagram() As cFlowDiagram
        Get
            Return Me.m_diagram
        End Get
        Set(ByVal value As cFlowDiagram)
            If (Not Object.ReferenceEquals(value, Me.m_diagram)) Then
                Me.m_diagram = value
                Me.SetChanged()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the unit that this position belongs to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Unit() As cUnit
        Get
            Return Me.m_unit
        End Get
        Set(ByVal value As cUnit)
            If (Not Object.ReferenceEquals(value, Me.m_unit)) Then
                Me.m_unit = value
                Me.SetChanged()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the X position.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Xpos() As Integer
        Get
            Return Me.m_iX
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iX) Then
                Me.m_iX = value
                Me.SetChanged()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the Y position.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Ypos() As Integer
        Get
            Return Me.m_iY
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iY) Then
                Me.m_iY = value
                Me.SetChanged()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the width.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Width() As Integer
        Get
            Return Me.m_iWidth
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iWidth) Then
                Me.m_iWidth = value
                Me.SetChanged()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the height.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property Height() As Integer
        Get
            Return Me.m_iHeight
        End Get
        Set(ByVal value As Integer)
            If (value <> Me.m_iHeight) Then
                Me.m_iHeight = value
                Me.SetChanged()
            End If
        End Set
    End Property

#End Region ' Properties

End Class