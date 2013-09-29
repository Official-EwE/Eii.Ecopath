Option Strict On
Imports System.IO
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cMSEUtils

    Public Shared Function GetReader(strFile As String) As StreamReader

        Dim reader As StreamReader = Nothing

        If Not File.Exists(strFile) Then Return Nothing
        Try
            reader = New StreamReader(strFile)
        Catch ex As Exception
            cLog.Write(ex, eVerboseLevel.Detailed, "MSEplugIn(" & strFile & ")")
        End Try
        Return reader

    End Function

    Public Shared Sub ReleaseReader(ByRef reader As StreamReader)
        If (reader IsNot Nothing) Then
            reader.Close()
            reader.Dispose()
        End If
        reader = Nothing
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Obtain a stream writer to a given location relative to the <paramref name="strDatapath">data path</paramref>.
    ''' This method tries to ensure that the file output directory is available.
    ''' </summary>
    ''' <param name="strFile"></param>
    ''' <param name="bAppend"></param>
    ''' <returns>A valid Streamwriter, or nothing if the writer could not be created for any reason.</returns>
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

    Public Enum eMSEPaths As Integer
        Root = 0
        Strategies
        DistrParams
        ParamsOut
        Results
    End Enum

    Public Shared Function Folder(DataPath As String, category As eMSEPaths, Optional strSubPath As String = "", Optional bCreateIfNotExists As Boolean = True) As String
        Dim strPath As String = Path.Combine(DataPath, Subfolder(category), strSubPath)
        If Not cFileUtils.IsDirectoryAvailable(strPath, bCreateIfNotExists) Then
            Return ""
        End If
        Return strPath
    End Function

    Public Shared Function Subfolder(category As eMSEPaths) As String
        Select Case category
            Case eMSEPaths.Root : Return ""
            Case eMSEPaths.DistrParams : Return "DistributionParameters"
            Case eMSEPaths.Strategies : Return "Strategies"
            Case eMSEPaths.ParamsOut : Return "ParametersOut"
            Case eMSEPaths.Results : Return "Results"
            Case Else
                Debug.Assert(False)
        End Select
        Return ""
    End Function

End Class
