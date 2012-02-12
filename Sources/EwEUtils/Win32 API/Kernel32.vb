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
Option Strict On
Imports System.Text
Imports System.Runtime.InteropServices

Namespace Win32Api

    ''' <summary>
    ''' Helper class providing access to a selection of Win32 kernel32 API calls.
    ''' </summary>
    Public Class Kernel32

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
        Public Shared Function GetShortPathName(ByVal strLongPath As String, <MarshalAs(UnmanagedType.LPTStr)> ByVal strShortPath As String, <MarshalAs(UnmanagedType.U4)> ByVal bufferSize As Integer) As Integer
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function GetPrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As StringBuilder, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function WritePrivateProfileString(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Boolean
        End Function

        <DllImport("kernel32.dll", SetLastError:=True)> _
        Public Shared Function GetPrivateProfileInt(ByVal lpAppName As String, ByVal lpKeyName As String, ByVal nDefault As Integer, ByVal lpFileName As String) As Integer
        End Function

    End Class

End Namespace ' Win32Api