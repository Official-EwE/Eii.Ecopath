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

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub ucLink_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed
        Me.LinkDefault(Nothing)
    End Sub

    Public Sub LinkDefault(ByVal link As cLinkDefault)
        Me.m_linkDefault = link
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        Dim clr As Color = Color.Black
        If Me.UIContext IsNot Nothing Then
            If Me.Selected Then
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
            Else
                clr = Me.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT)
            End If
        End If
        cArrowIndicator.DrawArrow(e.Graphics, clr, Me.ClientRectangle, 180, 1.0)

    End Sub

    Protected Overrides Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType) 
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

    Private Sub m_link_OnChanged(ByVal obj As cOOPStorable) _
        Handles m_linkDefault.OnChanged
        Me.Invalidate()
    End Sub

End Class
