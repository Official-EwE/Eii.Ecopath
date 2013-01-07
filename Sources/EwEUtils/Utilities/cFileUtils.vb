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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System
Imports System.IO
Imports System.Text
Imports System.Collections.Generic
Imports System.Security.AccessControl
Imports System.Diagnostics

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
            For Each c As Char In Path.GetInvalidPathChars
                If strPath.IndexOf(c) > -1 Then
                    strPath = strPath.Replace(c, "")
                End If
            Next

            ' Replace invalid file name chars with hyphens
            For Each c As Char In Path.GetInvalidFileNameChars
                If strFile.IndexOf(c) > -1 Then
                    strFile = strFile.Replace(c, "-"c)
                End If
            Next

            If bProtectPath Then
                strText = Path.Combine(strPath, strFile)
                ' Replace all accidental 'double dots'
                strText = cStringUtils.ReplaceAll(strText, "..", ".")
            Else
                strText = strFile
            End If

            Return strText
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a text into a valid file extension.
        ''' </summary>
        ''' <param name="strText">Text to convert into a file extension.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToValidFileExt(ByVal strText As String, strDefault As String) As String

            If (String.IsNullOrWhiteSpace(strText)) Then strText = strDefault
            If (String.IsNullOrWhiteSpace(strText)) Then Return ""

            If strText(0) <> "."c Then Return "." & strText
            Return strText

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Find a file in a directory.
        ''' </summary>
        ''' <param name="strFile">Name of the file to locate.</param>
        ''' <param name="strPath">Directory to search.</param>
        ''' <param name="bRecursive">Flag stating if subdirectories should be searched recursively.</param>
        ''' <returns>The full path to the file if found, or an empty string if the file could not be located.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FindFile(ByVal strFile As String, _
                                        ByVal strPath As String, _
                                        Optional ByVal bRecursive As Boolean = False) As String

            Dim strFullPath As String = Path.Combine(strPath, strFile)
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
                For Each strDirectory As String In Directory.GetDirectories(strPath)
                    strFullPath = FindFile(strFile, strDirectory, bRecursive)
                    If Not String.IsNullOrEmpty(strFullPath) Then Return strFullPath
                Next
            End If
            Return ""

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get all files that match a given <see cref="System.Windows.Forms.FileDialog.Filter">dialog filter</see>.
        ''' </summary>
        ''' <param name="astrFiles">The array of files to filter.</param>
        ''' <param name="astrExtensions">Array of file extensions to text.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FilesByDialogFilter(astrFiles() As String, astrExtensions() As String) As String()

            If (astrExtensions Is Nothing) Then Return astrFiles

            Dim hash As New HashSet(Of String)
            Dim lstrFiles As New List(Of String)

            For i As Integer = 0 To astrExtensions.Length - 1
                If Not hash.Contains(astrExtensions(i)) Then hash.Add(astrExtensions(i))
                If astrExtensions(i).Contains(".*") Then Return astrFiles
            Next

            For i As Integer = 0 To astrFiles.Length - 1
                If hash.Contains("*" & Path.GetExtension(astrFiles(i)).ToLower()) Then
                    lstrFiles.Add(astrFiles(i))
                End If
            Next

            Return lstrFiles.ToArray()

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
        ''' <returns>True if successful.</returns>
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

            If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strDest)) Then
                Return False
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

        Private Shared g_files As New List(Of String)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a random file in the %TEMP% folder, and return the path to the file.
        ''' </summary>
        ''' <param name="strExt">An optional file extension to use.</param>
        ''' <returns>The full path to the file.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function MakeTempFile(Optional ByVal strExt As String = "") As String

            ' TODO: Check if file is writeable!!!

            Dim strFileName As String = Path.GetRandomFileName() & strExt

            Dim strFile As String = Path.Combine(System.IO.Path.GetTempPath(), strFileName)
            ' Add to temp file registry
            If Not cFileUtils.g_files.Contains(strFile) Then cFileUtils.g_files.Add(strFile)
            ' Done
            Return strFile

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Purge all files created by <see cref="MakeTempFile"/>
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Shared Sub PurgeTempFiles()
            Dim astrFiles As String() = cFileUtils.g_files.ToArray
            For Each strTempFile As String In astrFiles
                cFileUtils.PurgeTempFile(strTempFile)
            Next
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Purge a single file created by <see cref="MakeTempFile"/>
        ''' </summary>
        ''' <param name="strTempFile"></param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub PurgeTempFile(ByVal strTempFile As String)
            Try
                If File.Exists(strTempFile) Then File.Delete(strTempFile)
                cFileUtils.g_files.Remove(strTempFile)
            Catch ex As Exception
                ' Hmm
            End Try
        End Sub

        Private Const cCHARS_NUMBER As String = "-0123456789E."
        Private Const cCHARS_STRING As String = "-0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_$."
        Private cSeparator As Char = CChar(" ")

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Read a number from a <see cref="TextReader"/> and advances the read pointer.
        ''' </summary>
        ''' <param name="reader">The reader to read the number from.</param>
        ''' <returns>The read number in the form of a <see cref="Single"/></returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function ReadNumber(ByRef reader As TextReader) As Single
            Dim ch(255) As Char ' Should be enough to hold one single number
            Dim readCh(1) As Char
            Dim nChar As Integer = 0

            ' Read leading spaces
            Do
                reader.Read(readCh, 0, 1)
            Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) > -1) Or (reader.Peek() < 0)

            If (reader.Peek() = -1) Then Throw New Exception("Unexpected end of file found while reading body")

            ' Read digits
            Do
                ch(nChar) = readCh(0)
                nChar += 1
                reader.Read(readCh, 0, 1)
            Loop Until (cCHARS_NUMBER.IndexOfAny(readCh) = -1) Or (reader.Peek() < 0)

            Return Single.Parse(ch)

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Checks if a directory is available, and optionally tries to create the directory if missing.
        ''' </summary>
        ''' <param name="strDirectory">The directory to check.</param>
        ''' <param name="bCreate">Optional flag, stating whether the directory 
        ''' should be created if it does not exist yet.</param>
        ''' <param name="bClear">Optional flag, stating whether any content of
        ''' the directory should be cleared out.</param>
        ''' <returns>True if the directory is available.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function IsDirectoryAvailable(ByVal strDirectory As String, _
                                                    Optional ByVal bCreate As Boolean = False, _
                                                    Optional ByVal bClear As Boolean = False) As Boolean

            ' Test if already exists as a file
            If File.Exists(strDirectory) Then Return False

            Dim bExists As Boolean = Directory.Exists(strDirectory)

            If bExists And bClear Then
                Try
                    Directory.Delete(strDirectory, True)
                    bCreate = True
                    bExists = False
                Catch ex As Exception
                    ' Ouch
                End Try
            End If

            If Not bExists Then
                Try
                    If bCreate Then bExists = (Directory.CreateDirectory(strDirectory) IsNot Nothing)
                Catch ex As Exception
                    ' Whoah
                End Try
            End If

            Return bExists

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert an absolute path to a relative path.
        ''' </summary>
        ''' <param name="strRoot">The root path to translate the absolute path to.</param>
        ''' <param name="strAbs">The absolute path to translate.</param>
        ''' <returns>A path relative to <paramref name="strRoot"/></returns>
        ''' -----------------------------------------------------------------------
        Shared Function RelativePath(ByVal strRoot As String, ByVal strAbs As String) As String

            Dim astrRoot As String() = Path.GetFullPath(strRoot).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar)
            Dim astrAbs As String() = Path.GetFullPath(strAbs).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar)

            Dim nShared As Integer = 0
            For i As Integer = 0 To Math.Min(astrRoot.Length, astrAbs.Length) - 1
                If String.Compare(astrRoot(i), astrAbs(i), True) = 0 Then
                    nShared += 1
                Else
                    Exit For
                End If
            Next i

            If nShared = 0 Then Return strAbs

            Dim sbPathRel As New StringBuilder()
            For i As Integer = nShared To astrRoot.Length - 1
                If (i > nShared) Then sbPathRel.Append(Path.DirectorySeparatorChar)
                sbPathRel.Append("..")
            Next

            If sbPathRel.Length = 0 Then
                sbPathRel.Append(".")
            End If

            For i As Integer = nShared To astrAbs.Length - 1
                sbPathRel.Append(Path.DirectorySeparatorChar)
                sbPathRel.Append(astrAbs(i))
            Next

            Return sbPathRel.ToString
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Recursively delete a directory and everything in it. Dangerous!
        ''' </summary>
        ''' <param name="strPath">The folder to delete.</param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function DeleteDirectory(strPath As String) As Boolean

            ' Path indicates a file: abort
            If File.Exists(strPath) Then Return False

            Dim bSucces As Boolean = True
            Try

                ' Recursively get rid off all subfolders
                For Each strSubFolder As String In Directory.GetDirectories(strPath)
                    bSucces = bSucces And cFileUtils.DeleteDirectory(strSubFolder)
                Next strSubFolder

                ' Now trash the content of this directory
                For Each strFile As String In Directory.GetFiles(strPath)
                    File.Delete(strFile)
                Next strFile

                ' Lastly trash directory itself
                Directory.Delete(strPath)

            Catch ex As Exception
                bSucces = False
            End Try
            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Obtain a streamwriter to a file. Any target directory is created if
        ''' if does not yet exist. 
        ''' </summary>
        ''' <param name="strPath">The path to stream writer.</param>
        ''' <param name="bAppend">Optional flag, stating whether the streamwriter
        ''' should be opened for appending or overwriting.</param>
        ''' <returns>A streamwriter, or Nothing if an error occurred.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function GetStreamWriter(ByVal strPath As String, _
                                               Optional ByVal bAppend As Boolean = False) As StreamWriter

            Dim sw As StreamWriter = Nothing

            If cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strPath), True, False) Then
                Try
                    sw = New StreamWriter(strPath, bAppend)
                Catch ex As Exception
                    ' Whoopy
                End Try
            End If
            Return sw

        End Function

    End Class

End Namespace
