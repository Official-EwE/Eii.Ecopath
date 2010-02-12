#Region " Imports "

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Security.AccessControl

#End Region ' Imports

Namespace Utilities

    ''' =======================================================================
    ''' <summary>
    ''' Helper class offering miscellaneous file-related functionalities.
    ''' </summary>
    ''' =======================================================================
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a backup copy of a file.
        ''' </summary>
        ''' <param name="strSrc">Source file to copy.</param>
        ''' <param name="strDest">Destination to copy file to. Leave this destination empty 
        ''' to backup to a default location. This parameter will return the backup 
        ''' destination file name.</param>
        ''' <returns>True if succesful.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function CreateBackup(ByVal strSrc As String, ByRef strDest As String) As Boolean

            If String.IsNullOrEmpty(strDest) Then

                Dim strDir As String = Path.GetDirectoryName(strSrc)
                Dim strFile As String = Path.GetFileNameWithoutExtension(strSrc)
                Dim strExt As String = Path.GetExtension(strSrc)
                Dim strDate As String = Date.Now.ToShortDateString
                Dim strFileNameNew As String = FileUtilities.ToValidFileName(String.Format("{0}_{1}", strFile, strDate), False)

                strDest = Path.Combine(strDir, strFileNameNew + strExt)
            End If

            Try
                ' Create backup copy
                File.Copy(strSrc, strDest, True)
                Return True
            Catch ex As Exception
            End Try
            Return False

        End Function

        Public Shared Function MakeTempFile(ByVal strFileName As String) As String

            If String.IsNullOrEmpty(strFileName) Then
                strFileName = System.IO.Path.GetTempFileName()
            End If

            ' TODO: Check if file is writeable!!!

            Return Path.Combine(System.IO.Path.GetTempPath(), strFileName)

        End Function

    End Class

End Namespace
