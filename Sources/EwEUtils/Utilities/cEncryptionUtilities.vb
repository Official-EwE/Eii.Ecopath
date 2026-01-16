' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.Security.Cryptography
Imports System.Text



Namespace Utilities

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Class providing encryption utility methods.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cEncryptionUtilities

        Public Shared Function MD5(strToHash As String) As String

            Return cEncryptionUtilities.MD5(System.Text.Encoding.ASCII.GetBytes(strToHash))

        End Function

        Public Shared Function MD5(data As Byte()) As String

            If (data Is Nothing) Then Return ""

            Dim md5Obj As New System.Security.Cryptography.MD5CryptoServiceProvider()
            Dim hash() As Byte = md5Obj.ComputeHash(data)
            Dim sbHash As New StringBuilder()

            For Each b As Byte In hash
                sbHash.Append(b.ToString("x2"))
            Next

            Return sbHash.ToString

        End Function

    End Class

End Namespace
