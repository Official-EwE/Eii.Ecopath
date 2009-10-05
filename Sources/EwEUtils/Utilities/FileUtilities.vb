'==============================================================================
'
' $Log: FileUtilities.vb,v $
' Revision 1.3  2009/05/02 01:48:07  jeroens
' Added comments
'
' Revision 1.2  2009/03/26 15:51:01  jeroens
' Added FindFile
'
' Revision 1.1  2008/09/26 07:31:12  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/07/24 18:06:46  jeroens
' ToValidFileName can be told to preserve path chars
'
' Revision 1.1  2008/05/07 19:52:14  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.IO
Imports System.Security.AccessControl

Namespace Utilities

    Public Class FileUtilities

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a text into a string that would be accepted by the OS as a
        ''' valid file name.
        ''' </summary>
        ''' <param name="strText">Text to convert into a file name.</param>
        ''' <param name="bProtectPath">Flag stating whether any path information
        ''' included in <paramref name="strText">strText</paramref> should be
        ''' preserved. If False, an path information is stripped off.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToValidFileName(ByVal strText As String, ByVal bProtectPath As Boolean) As String

            Dim strPath As String = ""

            ' 1. Strip off path part
            If bProtectPath Then
                Try
                    strPath = Path.GetDirectoryName(strText)
                Catch ex As Exception
                    strPath = ""
                End Try
                If String.IsNullOrEmpty(strPath) Then
                    bProtectPath = False
                Else
                    strText = strText.Substring(strPath.Length + 1)
                End If
            End If

            ' Clean up
            strText = strText.Replace(" ", "_")
            strText = strText.Replace("\", "-")
            strText = strText.Replace("/", "-")

            ' Replace invalid file name chars with hyphens
            For Each c As Char In Path.GetInvalidFileNameChars
                If strText.IndexOf(c) > -1 Then
                    strText = strText.Replace(c, "-"c)
                End If
            Next

            If (bProtectPath And Not String.IsNullOrEmpty(strPath)) Then
                strText = Path.Combine(strPath, strText)
            End If

            Return strText
        End Function

        Public Shared Function FindFile(ByVal strFile As String, ByVal strStartDir As String, _
                                        Optional ByVal bRecursive As Boolean = False) As String

            Dim strFullPath As String = Path.Combine(strStartDir, strFile)
            Dim fsec As FileSecurity = Nothing

            Try
                ' Try to be nice
                If File.Exists(strFullPath) Then Return strFullPath
                ' Ok, maybe the file is hidden. Let's be less nice.
                fsec = File.GetAccessControl(strFullPath, AccessControlSections.Group)
                If fsec IsNot Nothing Then
                    Return strFullPath
                End If
            Catch ex As FileNotFoundException
                ' Woops
            End Try

            If bRecursive Then
                For Each strDirectory As String In Directory.GetDirectories(strStartDir)
                    strFullPath = FindFile(strFile, strDirectory, bRecursive)
                    If Not String.IsNullOrEmpty(strFullPath) Then Return strFullPath
                Next
            End If
            Return ""

        End Function

    End Class

End Namespace
