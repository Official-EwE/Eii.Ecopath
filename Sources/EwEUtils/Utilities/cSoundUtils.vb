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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Imports System.Windows.Forms
Imports Microsoft.Win32

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous sound-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cSoundUtilities

#Region " Private vars "

        Private Shared s_sounds As sSystemSounds = Nothing

        Private Structure sSystemSounds
            ''' <summary>SystemAsterisk</summary>
            Dim Asterisk As String
            ''' <summary>SystemExclamation</summary>
            Dim Exclamation As String
            ''' <summary>SystemHand</summary>
            Dim Hand As String
            ''' <summary>SystemNotification</summary>
            Dim Notification As String
            ''' <summary>SystemQuestion</summary>
            Dim Question As String
            ''' <summary>A default sound</summary>
            Dim [Default] As String
        End Structure

#End Region ' Private vars

#Region " Public access "

        Public Shared Sub PlaySound(ByVal icon As MessageBoxIcon, Optional ByVal strFileName As String = "")

            If String.IsNullOrEmpty(strFileName) Then

                cSoundUtilities.InitSounds()

                With s_sounds
                    Select Case icon
                        Case MessageBoxIcon.Asterisk
                            strFileName = .Asterisk
                        Case MessageBoxIcon.Exclamation
                            strFileName = .Exclamation
                        Case MessageBoxIcon.Hand, _
                             MessageBoxIcon.Stop
                            strFileName = .Hand
                        Case MessageBoxIcon.Information
                            strFileName = .Notification
                        Case MessageBoxIcon.Question
                            strFileName = .Question
                        Case MessageBoxIcon.Warning
                            strFileName = .[Default]
                        Case Else
                            strFileName = .[Default]
                    End Select
                End With

                If String.IsNullOrWhiteSpace(strFileName) Then
                    strFileName = s_sounds.[Default]
                End If
            End If

            My.Computer.Audio.Play(strFileName, 1)

        End Sub

#End Region ' Public access

#Region " Internals "

        Private Shared Sub InitSounds()

            If Object.ReferenceEquals(s_sounds, Nothing) Then

                s_sounds = New sSystemSounds()

                With s_sounds
                    .Asterisk = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemAsterisk\.Current", "")
                    .Exclamation = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemExclamation\.Current", "")
                    .Hand = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemHand\.Current", "")
                    .Notification = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemNotification\.Current", "")
                    .Question = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\SystemQuestion\.Current", "")
                    .[Default] = cRegistryUtils.ReadKey(Registry.CurrentUser, "AppEvents\Schemes\Apps\.Default\.Default\.Current", "")
                End With
            End If
        End Sub

#End Region ' Internals

    End Class

End Namespace
