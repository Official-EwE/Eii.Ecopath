' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports ScientificInterfaceShared.Style

Public Class ucUnitDefault

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.BorderStyle = System.Windows.Forms.BorderStyle.None
    End Sub

    Protected Overrides Sub OnStyleguideChanged(changeFlags As cStyleGuide.eChangeType)
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Public Property UnitType As cUnitFactory.eUnitType

    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim fmt As New StringFormat()
        Dim rc As New Rectangle(Me.ClientRectangle.X, Me.ClientRectangle.Y, Me.ClientRectangle.Width, Me.ClientRectangle.Height)
        Dim clr As Color = Color.White

        If (Me.UIContext IsNot Nothing) Then
            If Me.Selected Then clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
        End If

        rc.Width -= 1
        rc.Height -= 1

        fmt.Alignment = StringAlignment.Center
        fmt.LineAlignment = StringAlignment.Near

        Using br As New SolidBrush(clr) : e.Graphics.FillRectangle(br, rc) : End Using

        Dim img As Image = cValueChainStyleGuide.GetImage(Me.UnitType)
        e.Graphics.DrawImage(img, rc.Width / 2.0! - 8, rc.Height - 18, 16, 16)
        img = Nothing

        e.Graphics.DrawString(Me.Text, SystemFonts.DefaultFont, SystemBrushes.ControlText, rc, fmt)
        e.Graphics.DrawRectangle(Pens.Black, rc)

    End Sub

End Class
