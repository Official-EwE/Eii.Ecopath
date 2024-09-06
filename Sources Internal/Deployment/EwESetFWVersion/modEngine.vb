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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Option Strict On
Imports System.IO
Imports EwEUtils.Utilities

Module modEngine

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Main entry point.  
    ''' </summary>
    ''' <remarks>
    ''' <para>Requires 2 arguments:</para>
    ''' <list>
    ''' <item>1) starting search path</item>
    ''' <item>2) the .net framework version to set (e.g. v4.7)</item>
    ''' </list>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Sub Main()

        ' Run this with the following command line arguments in the project properties > debug > command line parameters (or hard-code them here)
        ' "D:\Sources\Ecopath6\" "v4.7"

        Dim arrArgs() As String = cStringUtils.SplitQualified(Command, ",")
        Dim strPathSource As String = "D:\Sources\Ecopath6\" ' arrArgs(0).Replace("""", "").Trim
        Dim fwversion As String = "v4.7" ' arrArgs(1).Replace("""", "").Trim

        ' Find all the files with Assembly Name
        Dim lstrFiles As List(Of String) = GetFilesRecursive(strPathSource, "*.vbproj")

        ' Change the assembly names in directories.
        For Each strFile As String In lstrFiles
            ChangeFWVersion(strFile, fwversion)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Goes through the all files in a given list of files, and replaces 
    ''' the framework version in lines that contain one of the following:</para>
    ''' <list>
    ''' <item>VB: &lt;TargetFrameworkVersion("0.0.0.0")&gt;</item>
    ''' <item>C#: [TargetFrameworkVersion("1.0.0.0")]</item>
    ''' </list>
    ''' <para>This works for both C# and VB.</para>
    ''' </summary>
    ''' <param name="strFile">The exact file including the filename and extension</param>
    ''' <param name="strFWNew">The FW number in the format of "#[.#]".</param>
    ''' -----------------------------------------------------------------------
    Sub ChangeFWVersion(ByVal strFile As String, ByVal strFWNew As String)

        ' Check for jewels first
        If IO.File.Exists(strFile) Then

            Dim astrLines() As String
            Dim strLine As String
            Dim bChanged As Boolean = False

            Try
                astrLines = IO.File.ReadAllLines(strFile)

                If (astrLines.Length = 0) Then Return

                ' loop through each line and look for the a version indicator.
                For i = 0 To astrLines.Length - 1
                    strLine = astrLines(i).Trim
                    If strLine.Length > 2 Then
                        ' Is not a comment?
                        If (Not strLine.Substring(0, 1) = "'") And (Not strLine.Substring(0, 2) = "//") Then
                            ' #Yes: is assembly version or file version line?
                            If strLine.Contains("TargetFrameworkVersion") Then
                                ' #Yes: Build new line
                                Dim strLineNew As String = "<TargetFrameworkVersion>" & strFWNew & "</TargetFrameworkVersion>"
                                If (strLine <> strLineNew) Then
                                    astrLines(i) = strLineNew
                                    bChanged = True
                                End If
                            End If ' Containing Assembly
                        End If ' Check comments
                    End If ' Check length
                Next

                If bChanged Then
                    IO.File.WriteAllLines(strFile, astrLines)
                End If

            Catch ex As Exception
                Debug.Assert(False, "Whoopsy renaming assembly" & strFile)
            End Try
        End If

    End Sub

    Private EXCLUDED_PATHS As String() = New String() {".svn", "bin", "obj", "documentation", "user guide", "include", "database", "resources"}

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Goes through the directory and find all the files with a given file name.
    ''' </summary>
    ''' <param name="strPath">Starting Search directory.</param>
    ''' <param name="strFilter">File to be found including extension.</param>
    ''' -----------------------------------------------------------------------
    Private Function GetFilesRecursive(ByVal strPath As String, ByVal strFilter As String) As List(Of String)

        Dim result As New List(Of String)()
        If (Array.IndexOf(EXCLUDED_PATHS, Path.GetFileName(strPath).ToLower()) > -1) Then Return result

        Try
            result.AddRange(Directory.GetFiles(strPath, strFilter))
            For Each strSubDir As String In Directory.GetDirectories(strPath)
                result.AddRange(GetFilesRecursive(strSubDir, strFilter))
            Next
        Catch ex As Exception
            Console.WriteLine("Error searching directories. " & ex.Message)
        End Try
        Return result

    End Function

End Module

