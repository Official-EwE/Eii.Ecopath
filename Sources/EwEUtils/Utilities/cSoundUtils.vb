#Region " Imports "

Imports System.Threading
Imports Microsoft.VisualBasic
Imports EwEUtils.Utilities
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

                If String.IsNullOrEmpty(strFileName) Then
                    strFileName = s_sounds.[Default]
                End If
            End If

            If Not String.IsNullOrEmpty(strFileName) Then
                My.Computer.Audio.Play(strFileName, AudioPlayMode.Background)
            Else
                Beep()
            End If

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
