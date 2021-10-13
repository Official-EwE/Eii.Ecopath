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

Imports System.Windows.Forms
Imports Microsoft.Win32
Imports System
Imports System.IO
Imports Microsoft.VisualBasic

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous sound-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cSoundUtilities

#Region " Private vars "

        Private Class cWindowsSystemSounds
            ''' <summary>SystemAsterisk</summary>
            Public Shared ReadOnly Property Asterisk As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemAsterisk\.Current", "")
                End Get
            End Property
            ''' <summary>SystemExclamation</summary>
            Public Shared ReadOnly Property Exclamation As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemExclamation\.Current", "")
                End Get
            End Property
            ''' <summary>SystemHand</summary>
            Public Shared ReadOnly Property Hand As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemHand\.Current", "")
                End Get
            End Property
            ''' <summary>SystemNotification</summary>
            Public Shared ReadOnly Property Notification As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemNotification\.Current", "")
                End Get
            End Property
            ''' <summary>SystemQuestion</summary>
            Public Shared ReadOnly Property Question As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemQuestion\.Current", "")
                End Get
            End Property
            ''' <summary>A default sound</summary>
            Public Shared ReadOnly Property [Default] As String
                Get
                    Return cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\.Default\.Current", "")
                End Get
            End Property
        End Class

#End Region ' Private vars

#Region " Public access "

        Public Shared Sub PlaySound(icon As MessageBoxIcon)

            Dim strFileName As String = ""

            Select Case icon
                Case MessageBoxIcon.Asterisk
                    strFileName = cWindowsSystemSounds.Asterisk
                Case MessageBoxIcon.Exclamation
                    strFileName = cWindowsSystemSounds.Exclamation
                Case MessageBoxIcon.Hand,
                     MessageBoxIcon.Stop
                    strFileName = cWindowsSystemSounds.Hand
                Case MessageBoxIcon.Information
                    strFileName = cWindowsSystemSounds.Notification
                Case MessageBoxIcon.Question
                    strFileName = cWindowsSystemSounds.Question
                Case MessageBoxIcon.Warning
                    strFileName = cWindowsSystemSounds.[Default]
                Case Else
                    strFileName = cWindowsSystemSounds.[Default]
            End Select

            PlaySound(strFileName)

        End Sub

        Public Shared Sub PlaySound(strFileName As String)

            If String.IsNullOrWhiteSpace(strFileName) Then Return
            Try
                My.Computer.Audio.Play(strFileName, AudioPlayMode.Background)
            Catch ex As Exception
                ' Whoah
            End Try

        End Sub

        Public Shared Sub PlaySound(stream As Stream)

            Try
                My.Computer.Audio.Play(stream, AudioPlayMode.Background)
            Catch ex As Exception
                ' Whoah
            End Try

        End Sub

#End Region ' Public access

    End Class

End Namespace
