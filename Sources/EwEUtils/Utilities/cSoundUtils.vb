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

Imports System
Imports System.IO
Imports System.Media
Imports System.Windows.Forms
Imports Microsoft.VisualBasic

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous sound-related functionalities.
    ''' </summary>
    ''' =======================================================================
    Public Class cSoundUtilities

#Region " Public access "

        Public Shared Sub PlaySound(icon As MessageBoxIcon)

            Dim sound As SystemSound

            Select Case icon
                Case MessageBoxIcon.Asterisk
                    sound = SystemSounds.Asterisk
                Case MessageBoxIcon.Exclamation
                    sound = SystemSounds.Exclamation
                Case MessageBoxIcon.Hand, MessageBoxIcon.Stop
                    sound = SystemSounds.Hand
                Case MessageBoxIcon.Information
                    sound = SystemSounds.Exclamation
                Case MessageBoxIcon.Question
                    sound = SystemSounds.Question
                Case MessageBoxIcon.Warning
                    sound = SystemSounds.Exclamation
                Case Else
                    sound = SystemSounds.Beep
            End Select

            Try
                My.Computer.Audio.PlaySystemSound(sound)
            Catch ex As Exception

            End Try

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
