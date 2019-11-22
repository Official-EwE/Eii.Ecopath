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

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.BackgroundImage = My.Resources.splash_01
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Stretch

        Me.m_pbIcon.BackgroundImage = cDrawingUtils.BitmapFromIcon(cEwEIcon.Current())
        Me.m_pbIcon.BackgroundImageLayout = ImageLayout.Zoom

        Dim strBitApp As String = If(cSystemUtils.Is64BitProcess, SharedResources.ABOUT_64BIT, SharedResources.ABOUT_32BIT)
        Dim strVer As String = cStringUtils.Localize(SharedResources.GENERIC_LABEL_DOUBLE, cCore.Version(False), strBitApp)

        Dim dt As DateTime = cAssemblyUtils.GetCompileDate(System.Reflection.Assembly.GetAssembly(GetType(cCore)))
        Dim strMask As String = ""

        Select Case EwE6ApplicationFramework.ReleaseMode
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
#If DEBUG Then
        Me.TopMost = False
#Else
        Me.TopMost = True
#End If

    End Sub

End Class