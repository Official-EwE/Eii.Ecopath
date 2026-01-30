' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports ValueChain



''' ===========================================================================
''' <summary>
''' User control that reflects a default link.
''' </summary>
''' ===========================================================================
Public Class ucLinkDefault

    Private WithEvents m_linkDefault As cLinkDefault = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub ucLink_Disposed(sender As Object, e As System.EventArgs) _
        Handles Me.Disposed
        Me.LinkDefault(Nothing)
    End Sub

    Public Sub LinkDefault(link As cLinkDefault)
        Me.m_linkDefault = link
    End Sub

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim clr As Color = Color.Black
        If Me.UIContext IsNot Nothing Then
            If Me.Selected Then
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            Else
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End If
        End If
        cArrowIndicator.DrawArrow(e.Graphics, clr, Me.ClientRectangle, 0, 1.0)

    End Sub

    Protected Overrides Sub OnStyleguideChanged(changeFlags As cStyleGuide.eChangeType)
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Private Sub m_link_OnChanged(obj As cValueChainEntity) _
        Handles m_linkDefault.OnChanged
        Me.Invalidate()
    End Sub

End Class
