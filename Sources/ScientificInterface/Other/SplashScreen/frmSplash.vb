' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Text
Imports EwELicense
Imports EwEUtils.Utilities

#End Region ' Imports

Friend Class frmSplash

    Public Sub New()

        Me.InitializeComponent()

        Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)
        Me.SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.Opaque, False)

        Me.AllowTransparency = False

        Dim bInvertText As Boolean = False
        Dim now As Date = Date.Now
        Select Case now.Hour Mod 4
            Case 0 : Me.BackgroundImage = My.Resources.splash_01
            Case 1 : Me.BackgroundImage = My.Resources.splash_02 : bInvertText = True
            Case 2 : Me.BackgroundImage = My.Resources.splash_03
            Case 3 : Me.BackgroundImage = My.Resources.splash_04 : bInvertText = True
        End Select

        ' Tee hee hee
        If ((now.Month = 4) And (now.Day = 1)) Then
            Me.BackgroundImage = My.Resources.splash_xx
            bInvertText = True
        End If

        Me.Text = My.Resources.GENERIC_CAPTION
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Stretch

        Me.m_pbIcon.BackgroundImage = cDrawingUtils.BitmapFromIcon(cEwEIcon.Current())
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Zoom

        Me.m_lblEwE.Text = EwEVersion(False, True, False)
        Me.ScaleFont(Me.m_lblEwE)

        Dim sb As New StringBuilder()
        Dim strRelease As String = EwERelease()
        Dim strRegistration As String = EwERegistration(New cLicense())

        If (Not String.IsNullOrWhiteSpace(strRelease)) Then
            sb.Append(strRelease)
        End If
        If (Not String.IsNullOrWhiteSpace(strRegistration)) Then
            If (sb.Length > 0) Then sb.AppendLine()
            sb.Append(strRegistration)
        End If
        Me.m_lblReleaseMode.Text = sb.ToString()

        If (bInvertText) Then
            Me.m_lblEwE.ForeColor = Color.White
            Me.m_lblReleaseMode.ForeColor = Color.White
            Me.m_lblText.ForeColor = Color.White
        End If
        Me.UpdateStatus(My.Resources.STATUS_LOADING)

#If DEBUG Then
        Me.TopMost = False
#Else
        Me.TopMost = True
#End If
    End Sub

    Public Sub UpdateStatus(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateStatusDelegate(AddressOf Me.UpdateStatus), message)
        Else
            Me.m_lblText.Text = message
        End If
    End Sub

    Private Delegate Sub UpdateStatusDelegate(message As String)

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        Me.CenterToScreen()


        MyBase.OnLoad(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        Me.m_pbIcon.BackgroundImage.Dispose()
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub ScaleFont(lab As Label)

        Dim extent As SizeF = TextRenderer.MeasureText(lab.Text, lab.Font)

        Dim hRatio As Single = lab.Height / extent.Height
        Dim wRatio As Single = lab.Width / extent.Width
        Dim ratio As Single = If(hRatio < wRatio, hRatio, wRatio)
        Dim newSize As Single = CSng(Math.Floor(lab.Font.Size * ratio))
        lab.Font = New Font(lab.Font.FontFamily, newSize, lab.Font.Style)

    End Sub

End Class