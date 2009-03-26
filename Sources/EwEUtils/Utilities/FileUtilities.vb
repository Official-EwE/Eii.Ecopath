'==============================================================================
'
' $Log: FileUtilities.vb,v $
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

Namespace Utilities

    Public Class FileUtilities

        Public Shared Function ToValidFileName(ByVal strFileName As String, ByVal bProtectPath As Boolean) As String

            Dim strPath As String = ""

            ' 1. Strip off path part
            If bProtectPath Then
                strPath = Path.GetDirectoryName(strFileName)
                If String.IsNullOrEmpty(strPath) Then
                    bProtectPath = False
                Else
                    strFileName = strFileName.Substring(strPath.Length + 1)
                End If
            End If

            ' Clean up
            strFileName = strFileName.Replace(" ", "_")
            strFileName = strFileName.Replace("\", "-")
            strFileName = strFileName.Replace("/", "-")

            ' Replace invalid file name chars with hyphens
            For Each c As Char In Path.GetInvalidFileNameChars
                If strFileName.IndexOf(c) > -1 Then
                    strFileName = strFileName.Replace(c, "-"c)
                End If
            Next

            If (bProtectPath And Not String.IsNullOrEmpty(strPath)) Then
                strFileName = Path.Combine(strPath, strFileName)
            End If

            Return strFileName
        End Function

        Public Shared Function FindFile(ByVal strFile As String, ByVal strStartDir As String, _
                                        Optional ByVal bRecursive As Boolean = False) As String

            Dim strFullPath As String = Path.Combine(strStartDir, strFile)
            If File.Exists(strFullPath) Then Return strFullPath

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
