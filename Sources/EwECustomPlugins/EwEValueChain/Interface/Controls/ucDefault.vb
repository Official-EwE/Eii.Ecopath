' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Windows.Forms
Imports EwECore.Database.cEwEDatabase
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

Public Class ucDefault
    Inherits UserControl
    Implements IUIElement

    Private m_bSelected As Boolean = False
    Private m_obj As cOOPStorable = Nothing
    Private m_uic As cUIContext = Nothing

    Public Property Selected() As Boolean
        Get
            Return Me.m_bSelected
        End Get
        Set(value As Boolean)
            Me.m_bSelected = value
            Me.Invalidate()
        End Set
    End Property

    Public Property ObjDefault() As cOOPStorable
        Get
            Return Me.m_obj
        End Get
        Set(value As cOOPStorable)
            Me.m_obj = value
            Me.Invalidate()
        End Set
    End Property

    Public Property UIContext() As cUIContext _
        Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(value As cUIContext)
            If (Me.m_uic IsNot Nothing) Then
                RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
            End If
            Me.m_uic = value
            If (Me.m_uic IsNot Nothing) Then
                AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
            End If
        End Set
    End Property

    Protected ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_uic.StyleGuide
        End Get
    End Property

    Protected Overridable Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
        ' NOP
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ucDefault
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Name = "ucDefault"
        Me.ResumeLayout(False)

    End Sub
End Class
