Option Strict On
Imports System.Drawing
Imports ScientificInterfaceShared.Style

Public Class ucUnitDefault

    Private WithEvents m_sg As cStyleGuide = Nothing

    Private Sub ucUnitDefault_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.BorderStyle = Windows.Forms.BorderStyle.None
        ' Hook up to SG
        Me.m_sg = cStyleGuide.GetInstance()
    End Sub

    Private Sub ucUnitDefault_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Me.m_sg = Nothing
    End Sub

    Private Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType) _
            Handles m_sg.StyleGuideChanged
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Private Sub ucUnitDefault_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim fmt As New StringFormat()
        Dim rc As New Rectangle(Me.ClientRectangle.X, Me.ClientRectangle.Y, Me.ClientRectangle.Width, Me.ClientRectangle.Height)

        rc.Width -= 1
        rc.Height -= 1

        fmt.Alignment = StringAlignment.Center

        e.Graphics.FillRectangle(Brushes.White, rc)
        e.Graphics.DrawString(Me.Text, SystemFonts.DefaultFont, Brushes.Black, rc, fmt)

        If Me.Selected Then
            Using p As New Pen(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT))
                e.Graphics.DrawRectangle(p, rc)
            End Using
        Else
            Using p As New Pen(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
                e.Graphics.DrawRectangle(p, rc)
            End Using
        End If
    End Sub

End Class
