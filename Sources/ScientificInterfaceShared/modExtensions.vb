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
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Namespace Extensions

    Public Module modExtensions

        <Runtime.InteropServices.DllImportAttribute("user32.dll")>
        Private Function DestroyIcon(handle As IntPtr) As Boolean
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Properly destroy an icon by releasing the GDI+ resource. This Extension 
        ''' method works around a known issue in the .NET framework, see 
        ''' http://msdn.microsoft.com/en-us/library/system.drawing.icon.fromhandle.aspx
        ''' </summary>
        ''' <param name="icon">The icon to destroy.</param>
        ''' <remarks>...and I still firmly believe that Extensions are a most
        ''' horrendous addition to the .NET languages. Believe me, if Icon were
        ''' inheritable... Yuck!</remarks>
        ''' -----------------------------------------------------------------------
        <Runtime.CompilerServices.Extension()> _
        Public Sub Destroy(ByRef icon As System.Drawing.Icon)
            ' Encapsualted in Windows check until we have an idea how this were to behave on non-windows systems.
            If cSystemUtils.IsWindows Then
                ' Ugh! Urgh! Yuck! Blah!
                DestroyIcon(icon.Handle)
            End If
            icon.Dispose()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' String truncation method, blatantly copied from 
        ''' http://www.codeproject.com/KB/vb/NewPathCompactPath.aspx
        ''' </summary>
        ''' <param name="strSrc">The string to truncate with path ellipses.</param>
        ''' <param name="iWidth">Allowed width of the string in pixels.</param>
        ''' <param name="ft">The font to measure the string with.</param>
        ''' <param name="tfFlags">Optional string format flags</param>
        ''' <returns>A truncated string.</returns>
        ''' <remarks>Note that this method does not modify the original string.</remarks>
        ''' -----------------------------------------------------------------------
        <Runtime.CompilerServices.Extension()>
        Public Function CompactString(ByVal strSrc As String,
                                             ByVal iWidth As Integer,
                                             ByVal ft As Font,
                                             Optional ByVal tfFlags As TextFormatFlags = TextFormatFlags.SingleLine Or TextFormatFlags.PathEllipsis Or TextFormatFlags.ModifyString) As String

            If (String.IsNullOrWhiteSpace(strSrc)) Then Return ""

            Dim strResult As String = String.Copy(strSrc)
            TextRenderer.MeasureText(strResult, ft, New Size(iWidth, 0), tfFlags Or TextFormatFlags.ModifyString)
            Return strResult

        End Function
    End Module

End Namespace
