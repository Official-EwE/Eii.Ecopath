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
    Public Class cFileUtils

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
            Dim strFile As String = ""

            If String.IsNullOrEmpty(strText) Then Return ""

            ' 1. Strip off path part
            If bProtectPath Then

                Try
                    ' Find path\file separator position
                    Dim iLastSep As Integer = strText.LastIndexOf("\"c)
                    If iLastSep = -1 Then iLastSep = strText.LastIndexOf("/"c)
                    strPath = strText.Substring(0, iLastSep + 1)
                    strFile = strText.Substring(iLastSep + 1)
                Catch ex As Exception
                    strPath = ""
                    strFile = strText
                End Try

                bProtectPath = Not String.IsNullOrEmpty(strPath)
            Else
                strFile = strText
            End If

            ' Clean up
            'strFile = strText.Replace(" ", "_") ' Spaces are definitely allowed under 32 bit ;-)
            strFile = strFile.Replace("\", "-")
            strFile = strFile.Replace("/", "-")

            ' Replace invalid file name chars with hyphens
            For Each c As Char In Path.GetInvalidFileNameChars
                If strFile.IndexOf(c) > -1 Then
                    strFile = strFile.Replace(c, "-"c)
                End If
            Next

            If bProtectPath Then
                strText = Path.Combine(strPath, strFile)
            Else
                strText = strFile
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
        ''' <param name="attributes"><see cref="FileAttributes">Attributes</see> to
        ''' assign to the backup file.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' If <paramref name="strDest"/> is left empty, a backup file name will be
        ''' created that looks like '[original name].[original ext].[short date]'.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Public Shared Function CreateBackup(ByVal strSrc As String, _
                                            ByRef strDest As String, _
                                            Optional ByVal attributes As FileAttributes = FileAttributes.Archive Or FileAttributes.NotContentIndexed) As Boolean

            If String.IsNullOrEmpty(strDest) Then
                strDest = strSrc & ".backup_" & ToValidFileName(Date.Now.ToShortDateString, False)
            End If

            Try
                ' Create backup copy
                File.Copy(strSrc, strDest, True)
                ' Apply attributes
                File.SetAttributes(strDest, attributes)
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

        Private Const cCHARS_NUMBER As String = "-0123456789E."
        Private Const cCHARS_STRING As String = "-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$."
        Private cSeparator As Char = CChar(" ")

        Public Shared Function ReadNumber(ByRef sr As System.IO.TextReader) As Single
            Dim ch(255) As Char ' Should be enough to hold one single number
            Dim readCh(1) As Char
            Dim nChar As Integer = 0

            ' Read leading spaces
            Do
                sr.Read(readCh, 0, 1)
            Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) > -1) Or (sr.Peek() < 0)

            If (sr.Peek() = -1) Then Throw New Exception("Unexpected end of file found while reading body")

            ' Read digits
            Do
                ch(nChar) = readCh(0)
                nChar += 1
                sr.Read(readCh, 0, 1)
            Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) = -1) Or (sr.Peek() < 0)

            Return Single.Parse(ch)

        End Function

    End Class

End Namespace
