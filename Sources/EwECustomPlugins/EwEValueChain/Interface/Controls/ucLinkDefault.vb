#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Database.cEwEDatabase

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' User control that reflects a default link.
''' </summary>
''' ===========================================================================
Public Class ucLinkDefault

    Private WithEvents m_linkDefault As cLinkDefault = Nothing
    Private WithEvents m_sg As cStyleGuide = Nothing

    Public Sub New()
        Me.InitializeComponent()
        ' Hook up to SG
        Me.m_sg = cStyleGuide.GetInstance()
    End Sub

    Private Sub ucLink_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed
        Me.LinkDefault(Nothing)
        Me.m_sg = Nothing
    End Sub

    Public Sub LinkDefault(ByVal link As cLinkDefault)
        Me.m_linkDefault = link
    End Sub

    Private Sub ucLink_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
        Handles Me.Paint
        If Me.Selected Then
            cArrowIndicator.DrawArrow(e.Graphics, _
                Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), _
                Me.ClientRectangle, 180, 1.0)
        Else
            cArrowIndicator.DrawArrow(e.Graphics, _
                Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT), _
                Me.ClientRectangle, 180, 1.0)
        End If
    End Sub

    Private Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType) _
        Handles m_sg.StyleGuideChanged
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Private Sub m_link_OnChanged(ByVal obj As cOOPStorable) _
        Handles m_linkDefault.OnChanged
        Me.Invalidate()
    End Sub

End Class
