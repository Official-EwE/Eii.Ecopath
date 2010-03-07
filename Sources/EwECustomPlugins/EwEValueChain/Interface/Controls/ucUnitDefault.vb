Option Strict On
Imports System.Drawing
Imports ScientificInterfaceShared.Style

Public Class ucUnitDefault

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.BorderStyle = Windows.Forms.BorderStyle.None
    End Sub

    Protected Overrides Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType)
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim fmt As New StringFormat()
        Dim rc As New Rectangle(Me.ClientRectangle.X, Me.ClientRectangle.Y, Me.ClientRectangle.Width, Me.ClientRectangle.Height)
        Dim clr As Color = Color.Black

        rc.Width -= 1
        rc.Height -= 1

        fmt.Alignment = StringAlignment.Center

        e.Graphics.FillRectangle(Brushes.White, rc)
        e.Graphics.DrawString(Me.Text, SystemFonts.DefaultFont, Brushes.Black, rc, fmt)

        If (Me.UIContext IsNot Nothing) Then
            If Me.Selected Then
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            Else
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End If
        End If

        Using p As New Pen(clr)
            e.Graphics.DrawRectangle(p, rc)
        End Using

    End Sub

End Class
