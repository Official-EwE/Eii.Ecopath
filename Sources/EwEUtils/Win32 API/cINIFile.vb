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

Option Strict On
Imports System
Imports EwEUtils.Win32Api

#End Region ' Imports

Namespace Win32Api

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Implements a simple read/write interface to a Windows INI file.<br/><br/>
    ''' Written by Karl Moore, obtained May 26/05 from http://www.developer.com/net/asp/article.php/3287991
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    <Obsolete("This class uses Win32API methods that should be discontinued. Use SystemUtilities.cXMLini instead")> _
    Public Class cINIFile

        Private m_strFilename As String

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new INI file interface.
        ''' </summary>
        ''' <param name="strFilename">The name of the INI file to access.</param>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal strFilename As String)
            Me.m_strFilename = strFilename
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the current filename
        ''' </summary>
        ''' -----------------------------------------------------------------------------
        ReadOnly Property FileName() As String
            Get
                Return m_strFilename
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns a string from an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="strDefault">Default string value</param>
        ''' <returns>The INI string, or the default value in case the key is not found in indicated section</returns>
        ''' -----------------------------------------------------------------------------
        Public Function GetString(ByVal strSection As String, ByVal strKey As String, ByVal strDefault As String) As String

            Dim strOut As String = strDefault
            Dim intCharCount As Integer
            Dim objResult As New System.Text.StringBuilder(256)
            intCharCount = Kernel32.GetPrivateProfileString(strSection, strKey, strDefault, objResult, objResult.Capacity, Me.m_strFilename)
            If intCharCount > 0 Then strOut = objResult.ToString().Substring(0, intCharCount)
            Return (strOut)

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns an integer from an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="nDefault">Default value</param>
        ''' <returns>The INI string, or the default value in case the key is not found in indicated section</returns>
        ''' -----------------------------------------------------------------------------
        Public Function GetInteger(ByVal strSection As String, ByVal strKey As String, ByVal nDefault As Integer) As Integer
            Return Kernel32.GetPrivateProfileInt(strSection, strKey, nDefault, Me.m_strFilename)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Returns a boolean from an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="bDefault">Default value</param>
        ''' <returns>The INI string, or the default value in case the key is not found in indicated section</returns>
        ''' -----------------------------------------------------------------------------
        Public Function GetBoolean(ByVal strSection As String, ByVal strKey As String, ByVal bDefault As Boolean) As Boolean
            ' Returns a boolean from your INI file
            Return (Kernel32.GetPrivateProfileInt(strSection, strKey, CInt(bDefault), Me.m_strFilename) = 1)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a string to an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="strValue ">The value to write value</param>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteString(ByVal strSection As String, ByVal strKey As String, ByVal strValue As String)
            ' Writes a string to your INI file
            Kernel32.WritePrivateProfileString(strSection, strKey, strValue, Me.m_strFilename)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a string to an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="nValue ">The value to write value</param>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteInteger(ByVal strSection As String, ByVal strKey As String, ByVal nValue As Integer)
            Me.WriteString(strSection, strKey, CStr(nValue))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Writes a string to an INI file
        ''' </summary>
        ''' <param name="strSection">INI section to access</param>
        ''' <param name="strKey">INI key to access</param>
        ''' <param name="bValue ">The value to write value</param>
        ''' -----------------------------------------------------------------------------
        Public Sub WriteBoolean(ByVal strSection As String, ByVal strKey As String, ByVal bValue As Boolean)
            ' Writes a boolean to your INI file
            WriteString(strSection, strKey, CStr(CInt(bValue)))
        End Sub

    End Class

End Namespace
