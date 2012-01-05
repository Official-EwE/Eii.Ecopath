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
        ''' <param name="strStartDir">Directory to search.</param>
        ''' <param name="bRecursive">Flag stating if subdirectories should be searched recursively.</param>
        ''' <returns>The full path to the file if found, or an empty string if the file could not be located.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FindFile(ByVal strFile As String, _
                                        ByVal strStartDir As String, _
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

            If Not Directory.Exists(Path.GetDirectoryName(strDest)) Then
                Try
                    Directory.CreateDirectory(Path.GetDirectoryName(strDest))
                Catch ex As Exception
                    ' Ouch!
                    Return False
                End Try
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
        ''' <returns>True if the directory is available.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function IsDirectoryAvailable(ByVal strDirectory As String, _
                                                    Optional ByVal bCreate As Boolean = False) As Boolean

            If Not Directory.Exists(strDirectory) Then
                Try
                    If bCreate Then Return (Directory.CreateDirectory(strDirectory) IsNot Nothing)
                Catch ex As Exception
                    ' Whoah
                End Try
                Return False
            End If
            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Creates a standard file name for EwE output files.
        ''' </summary>
        ''' <param name="strModelName">Name of the model for which the output file is created.</param>
        ''' <param name="strComponentName">Name of the component for which the output file is created.</param>
        ''' <param name="strFilter">Optional filter to specify an optional subcomponent for which the file is created.</param>
        ''' <param name="strScenarioName">Optional scenario name for which the file is created.</param>
        ''' <param name="strExt">Optional extension to add.</param>
        ''' <returns>A file name of the form {<paramref name="ModelName"/>}-{<paramref name="ComponentName"/>}[-{<paramref name="Scenario"/>}][-{<paramref name="Filter"/>}][.{<paramref name="Ext"/>}].</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function ToOutputFilename(ByVal strModelName As String, _
                                                ByVal strComponentName As String, _
                                                Optional ByVal strFilter As String = "", _
                                                Optional ByVal strScenarioName As String = "", _
                                                Optional ByVal strExt As String = ".csv") As String

            ' Sanity checks
            Debug.Assert(Not String.IsNullOrEmpty(strModelName), "Model Name required")
            Debug.Assert(Not String.IsNullOrEmpty(strComponentName), "Component Name required")

            Dim cSeparator As String = "-"
            Dim sb As New StringBuilder()

            ' Add entire component as subdirectory
            sb.Append(cFileUtils.ToValidFileName(strModelName, False))
            sb.Append(cSeparator)
            sb.Append(cFileUtils.ToValidFileName(strComponentName, False))
            sb.Append(Path.DirectorySeparatorChar)

            ' Add entire scenario name as directory, if provided
            If (Not String.IsNullOrWhiteSpace(strScenarioName)) Then
                sb.Append(cFileUtils.ToValidFileName(strScenarioName, False))
                sb.Append(Path.DirectorySeparatorChar)
            End If

            ' Add entire filter as directory, if provided
            If (String.IsNullOrWhiteSpace(strFilter)) Then
                strFilter = "Output" & " " & Date.Now.ToShortDateString & " " & Date.Now.ToShortTimeString
            End If
            sb.Append(cFileUtils.ToValidFileName(strFilter, False))

            ' Add extension, if provided
            If (Not String.IsNullOrWhiteSpace(strExt)) Then
                ' Add a dot ('.') if the extension is provided without
                If Not cStringUtils.BeginsWith(strExt, ".") Then
                    sb.Append(".")
                End If
                sb.Append(strExt)
            End If

            Return sb.ToString()

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

    End Class

End Namespace
