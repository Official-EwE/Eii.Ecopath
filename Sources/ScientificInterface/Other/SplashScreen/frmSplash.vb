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
Imports System.ComponentModel
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Friend Class frmSplash

    Private Shared s_splash As frmSplash

    Public Sub New()
        Me.InitializeComponent()
        s_splash = Me
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.BackgroundImage = My.Resources.splash_01
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Stretch

        Me.m_pbIcon.BackgroundImage = cDrawingUtils.BitmapFromIcon(cEwEIcon.Current())
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Zoom

        Me.m_lblEwE.Text = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.GENERIC_CAPTION, cCore.Version(False))

        Dim dt As DateTime = cAssemblyUtils.GetCompileDate(System.Reflection.Assembly.GetAssembly(GetType(cCore)))
        Dim strMask As String = ""

        Select Case My.Application.ReleaseMode
            Case eReleaseMode.Beta
                strMask = My.Resources.VERSION_BETA
            Case eReleaseMode.Dev
                strMask = My.Resources.VERSION_DEVELOPMENT
            Case eReleaseMode.Release
                ' ToDo: show pro / free
                strMask = My.Resources.VERSION_RELEASE
        End Select

        Me.m_lblReleaseMode.Text = cStringUtils.Localize(strMask, dt.ToShortDateString)

        Me.CenterToScreen()
        Me.TopMost = True

    End Sub

    Public Shared Sub BuggerOff()
        If (s_splash Is Nothing) Then Return
        If (s_splash.InvokeRequired) Then
            If s_splash.InvokeRequired Then
                s_splash.Invoke(New MethodInvoker(AddressOf s_splash.Close))
            Else
                s_splash.Close()
            End If
            s_splash = Nothing
        End If
    End Sub

    Public Shared Function IsAlive() As Boolean
        Return (s_splash IsNot Nothing)
    End Function

End Class