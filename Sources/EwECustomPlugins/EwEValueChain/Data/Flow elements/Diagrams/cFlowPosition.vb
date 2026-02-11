' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Database.cEwEDatabase

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
        Set(value As cFlowDiagram)
            If (Not ReferenceEquals(value, Me.m_diagram)) Then
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
        Set(value As cUnit)
            If (Not ReferenceEquals(value, Me.m_unit)) Then
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
        Set(value As Integer)
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
        Set(value As Integer)
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
        Set(value As Integer)
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
        Set(value As Integer)
            If (value <> Me.m_iHeight) Then
                Me.m_iHeight = value
                Me.SetChanged()
            End If
        End Set
    End Property

#End Region ' Properties

End Class