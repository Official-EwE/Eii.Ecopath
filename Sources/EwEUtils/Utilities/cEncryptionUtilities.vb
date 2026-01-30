' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
