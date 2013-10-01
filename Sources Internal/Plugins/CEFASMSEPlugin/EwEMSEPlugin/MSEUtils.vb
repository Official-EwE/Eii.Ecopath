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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
Option Strict On
Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cMSEUtils

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fail-safe method to obtain a stream reader to a file.
    ''' </summary>
    ''' <param name="strFile">The path to the file to obtain.</param>
    ''' <returns>A stream reader if successful, or Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetReader(strFile As String) As StreamReader

        Dim reader As StreamReader = Nothing

        ' Capture errors
        If String.IsNullOrWhiteSpace(strFile) Then Return reader
        If Not File.Exists(strFile) Then Return reader

        Try
            ' Try to create file reader
            reader = New StreamReader(strFile)
        Catch ex As Exception
            ' Report error
            cLog.Write(ex, eVerboseLevel.Detailed, "MSEplugIn(" & strFile & ")")
        End Try

        ' Done
        Return reader

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fail-safe method to release stream reader previously obtained via
    ''' <see cref="GetReader"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Sub ReleaseReader(ByRef reader As StreamReader)
        If (reader IsNot Nothing) Then
            reader.Close()
            reader.Dispose()
        End If
        reader = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fail-safe method to obtain a stream writer to a file.
    ''' </summary>
    ''' <param name="strFile">The path to the file to obtain.</param>
    ''' <param name="bAppend">Append to the content if True, or overwrite content
    ''' if false.</param>
    ''' <returns>A stream writer if successful, or Nothing if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function GetWriter(strFile As String, Optional bAppend As Boolean = False) As StreamWriter

        Dim writer As StreamWriter = Nothing

        If Not File.Exists(strFile) Then Return Nothing
        If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then Return Nothing

        Try
            writer = New StreamWriter(strFile, bAppend)
        Catch ex As Exception
            cLog.Write(ex, eVerboseLevel.Detailed, "MSEplugIn(" & strFile & ")")
        End Try
        Return writer

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fail-safe method to release stream writer previously obtained via
    ''' <see cref="GetReader"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Sub ReleaseWriter(ByRef writer As StreamWriter)
        If (writer IsNot Nothing) Then
            writer.Flush()
            writer.Close()
            writer.Dispose()
        End If
        writer = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a stripped and trimmed value obtained from a CSV file.
    ''' </summary>
    ''' <param name="strValue"></param>
    ''' <returns></returns>
    ''' <remarks>Removes quotes, and trims whitespace.</remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function FromCSVField(strValue As String) As String
        Return strValue.Replace("""", "").Trim
    End Function

    ''' <summary>
    ''' Data path categories for the MSE plug-in.
    ''' </summary>
    Public Enum eMSEPaths As Integer
        ''' <summary>The root folder that the plug-in is configured to use.</summary>
        Root = 0
        ''' <summary>The Fleet subfolder under 'Root'.</summary>
        Fleet
        ''' <summary>The NaturalMortaility subfolder under 'Root'.</summary>
        NaturalMort
        ''' <summary>The Distributions subfolder under 'Root'.</summary>
        DistrParams
        ''' <summary>The ParametersOut subfolder under 'Root'.</summary>
        ParamsOut
        ''' <summary>The Results subfolder under 'Root'.</summary>
        Results
        ''' <summary>The Results\Trajectories1 subfolder under 'Root'.</summary>
        ResultsTrajectories
        ''' <summary>The Results\Trajectories2 subfolder under 'Root'.</summary>
        ResultsTraj2
        ''' <summary>The Strategies subfolder under 'Root'.</summary>
        Strategies
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the full path to a MSE file.
    ''' </summary>
    ''' <param name="DataPath">The MSE datapath</param>
    ''' <param name="category">The <see cref="eMSEPaths">category</see> subfolder for the file.</param>
    ''' <param name="strFile">The name of the file.</param>
    ''' <param name="bCreateFolderIfNotExists">Flag that states that the folder should be created if it does not exist.</param>
    ''' <returns>A path to a file, or an empty string if an error occurred.</returns>
    ''' <remarks>
    ''' This method takes care of a few common headaches related to file access, such
    ''' as ensuring that a directory exists (and optionally creating the directory),
    ''' and validating whether the file name is valid to be used with the 
    ''' Operating System.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function MSEFile(DataPath As String, category As eMSEPaths, strFile As String, _
                                Optional bCreateFolderIfNotExists As Boolean = False) As String
        Dim strPath As String = MSEFolder(DataPath, category, bCreateFolderIfNotExists)
        ' Abort if folder not present
        If String.IsNullOrWhiteSpace(strPath) Then Return ""
        ' Make sure we're using a safe file name
        strFile = cFileUtils.ToValidFileName(strFile, False)
        ' Concat!
        Return Path.Combine(strPath, strFile)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the full path to an MSE folder.
    ''' </summary>
    ''' <param name="DataPath">The MSE datapath</param>
    ''' <param name="category">The <see cref="eMSEPaths">category</see> subfolder.</param>
    ''' <param name="bCreateIfNotExists">Flag that states that the folder should be created if it does not exist.</param>
    ''' <returns>A path to a folder, or an empty string if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function MSEFolder(DataPath As String, category As eMSEPaths, _
                                  Optional bCreateIfNotExists As Boolean = True) As String
        Dim strPath As String = Path.Combine(DataPath, Subfolder(category))
        If Not cFileUtils.IsDirectoryAvailable(strPath, bCreateIfNotExists) Then
            Return ""
        End If
        Return strPath
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the subfolder path for a folder category.
    ''' </summary>
    ''' <param name="category">The <see cref="eMSEPaths">category</see> subfolder.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function Subfolder(category As eMSEPaths) As String
        Select Case category
            Case eMSEPaths.Root : Return ""
            Case eMSEPaths.DistrParams : Return "DistributionParameters"
            Case eMSEPaths.Strategies : Return "Strategies"
            Case eMSEPaths.ParamsOut : Return "ParametersOut"
            Case eMSEPaths.Results : Return "Results"
            Case eMSEPaths.ResultsTrajectories : Return "Results\Trajectories"
            Case eMSEPaths.ResultsTraj2 : Return "Results\Trajectories2"
            Case Else
                Debug.Assert(False)
        End Select
        Return ""
    End Function

End Class
