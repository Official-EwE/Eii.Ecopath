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
Imports System.Runtime.InteropServices
Imports EwEUtils.Utilities
Imports Microsoft.Win32

#End Region ' Imports

''' <summary>
''' https://stackoverflow.com/questions/57850624/prevent-a-computer-from-entering-sleep-standby-hibernate-while-program-is-runnin
''' </summary>
Friend Class cNativeMethods

    Private Shared s_dtLastMidHour As Integer = -1
    Private Shared s_bSetActiveHoursSuccess As Boolean = True

    Public Shared Sub PreventSleep(bMonitor As Boolean)
        Dim flags As eExecutionState = eExecutionState.ES_CONTINUOUS Or eExecutionState.ES_SYSTEM_REQUIRED
        If bMonitor Then flags = flags Or eExecutionState.ES_DISPLAY_REQUIRED
        SetThreadExecutionState(flags)
    End Sub

    Public Shared Sub AllowSleep()
        SetThreadExecutionState(eExecutionState.ES_CONTINUOUS)
    End Sub

    <DllImport("Kernel32.DLL", CharSet:=CharSet.Auto, SetLastError:=True)>
    Private Shared Function SetThreadExecutionState(ByVal state As eExecutionState) As eExecutionState
    End Function

    <FlagsAttribute()>
    Public Enum eExecutionState As UInteger
        ES_SYSTEM_REQUIRED = &H1
        ES_DISPLAY_REQUIRED = &H2
        ES_CONTINUOUS = &H80000000UI
    End Enum

    Public Shared Function ShiftActiveHours() As Boolean

        If Not s_bSetActiveHoursSuccess Then Return False

        Dim basepath As String = "SOFTWARE\Microsoft\WindowsUpdate\UX\Settings"
        Dim midHour As Integer = Date.Now.Hour
        Dim bSuccess As Boolean = True

        If midHour <> s_dtLastMidHour Then

            Try
                Dim key As RegistryKey = Nothing
                key = Registry.LocalMachine.OpenSubKey(basepath, True)
                ' Start active hours at 2 hours ago
                key.SetValue("ActiveHoursStart", (midHour + 22) Mod 24)
                ' Stop active hours 6 hours in the future
                key.SetValue("ActiveHoursEnd", (midHour + 6) Mod 24)
            Catch ex As Exception
                bSuccess = False
            End Try

            s_bSetActiveHoursSuccess = bSuccess
            s_dtLastMidHour = midHour

        End If
        Return bSuccess

    End Function

End Class