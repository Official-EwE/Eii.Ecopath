' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports System.IO
Imports EwECore
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cMSEUtils

    Private Shared ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMSEUtils)()

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
            m_logger.LogError(ex, "MSEplugIn(" & strFile & ")")
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

        If Not cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then Return Nothing

        Try
            writer = New StreamWriter(strFile, bAppend)
            writer.AutoFlush = True
        Catch ex As Exception
            m_logger.LogError(ex, "MSEplugIn(" & strFile & ")")
        End Try
        Return writer

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Fail-safe method to release stream writer previously obtained via
    ''' <see cref="GetWriter"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Shared Sub ReleaseWriter(writer As StreamWriter)
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
        ' ''' <summary>The NaturalMortaility subfolder under 'Root'.</summary>
        'NaturalMort
        ''' <summary>The Distributions subfolder under 'Root'.</summary>
        DistrParams
        ''' <summary>The ParametersOut subfolder under 'Root'.</summary>
        ParamsOut
        ''' <summary>The Results subfolder under 'Root'.</summary>
        Results
        ''' <summary>The Results\HCRF_Targ subfolder under 'Root'.</summary>
        HCRF_Targ
        ''' <summary>The Results\HCRF_Cons subfolder under 'Root'.</summary>
        HCRF_Cons
        ''' <summary>The Results\HCRQuota_Targ subfolder under 'Root'.</summary>
        HCRQuota_Targ
        ''' <summary>The Results\HCRQuota_Cons subfolder under 'Root'.</summary>
        HCRQuota_Cons
        ''' <summary>The Results\Biomass subfolder under 'Root'.</summary>
        Biomass
        ''' <summary>The Results\Effort subfolder under 'Root'.</summary>
        Effort
        ''' <summary>The Results\HighestValueGroup subfolder under 'Root'.</summary>
        HighestValueGroup
        ''' <summary>The Results\ChokeGroup subfolder under 'Root'.</summary>
        ChokeGroup
        ''' <summary>The Results\RealisedF subfolder under 'Root'.</summary>
        RealisedF
        ''' <summary>The Results\RealisedF subfolder under 'Root'.</summary>
        RealisedLandedF
        ''' <summary>The Results\RealisedF subfolder under 'Root'.</summary>
        RealisedDiscardF
        ''' <summary>The Results\LandingsTrajectories subfolder under 'Root'.</summary>
        LandingsTrajectories
        ''' <summary>The Results\DiscardsTrajectories subfolder under 'Root'.</summary>
        DiscardsTrajectories
        ''' <summary>The Results\CatchTrajectories subfolder under 'Root'.</summary>
        CatchTrajectories
        ''' <summary>The Results\ValueTrajectories subfolder under 'Root'.</summary>
        ValueTrajectories
        ''' <summary>The Results\PredationMortalityTrajectories subfolder under 'Root'.</summary>
        PredationMortality
        ''' <summary>The Results\PredationMortalityPreyOnlyTrajectories subfolder under 'Root'.</summary>
        PredationMortalityPreyOnly
        ''' <summary>The Strategies subfolder under 'Root'.</summary>
        Strategies
        ''' <summary>The regulations subfolder under 'Root'.</summary>
        Regulations

        BiomassLimits

        StockAssessment
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the full path to a MSE file.
    ''' </summary>
    ''' <param name="strPath">The MSE datapath</param>
    ''' <param name="category">The <see cref="eMSEPaths">category</see> subfolder for the file.</param>
    ''' <param name="strFile">The name of the file.</param>
    ''' <returns>A path to a file, or an empty string if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function MSEFile(strPath As String, category As eMSEPaths, strFile As String) As String
        Return Path.Combine(MSEFolder(strPath, category), cFileUtils.ToValidFileName(strFile, False))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the full path to an MSE folder.
    ''' </summary>
    ''' <param name="strPath">The MSE datapath</param>
    ''' <param name="category">The <see cref="eMSEPaths">category</see> subfolder.</param>
    ''' <returns>A path to a folder.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function MSEFolder(strPath As String, category As eMSEPaths) As String
        Return Path.Combine(strPath, Subfolder(category))
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
            Case eMSEPaths.Fleet : Return "Fleet"
                'Case eMSEPaths.NaturalMort : Return "NaturalMortalities"
            Case eMSEPaths.ParamsOut : Return "ParametersOut"
            Case eMSEPaths.Results : Return "Results"
            Case eMSEPaths.HCRF_Targ : Return "Results\HCRF_Targ"
            Case eMSEPaths.HCRF_Cons : Return "Results\HCRF_Cons"
            Case eMSEPaths.HCRQuota_Targ : Return "Results\HCRQuota_Targ"
            Case eMSEPaths.HCRQuota_Cons : Return "Results\HCRQuota_Cons"
            Case eMSEPaths.Biomass : Return "Results\Biomass"
            Case eMSEPaths.Effort : Return "Results\Effort"
            Case eMSEPaths.HighestValueGroup : Return "Results\HighestValueGroup"
            Case eMSEPaths.ChokeGroup : Return "Results\ChokeGroup"
            Case eMSEPaths.RealisedF : Return "Results\RealisedF"
            Case eMSEPaths.RealisedLandedF : Return "Results\RealisedLandedF"
            Case eMSEPaths.RealisedDiscardF : Return "Results\RealisedDiscardedF"
            Case eMSEPaths.LandingsTrajectories : Return "Results\LandingsTrajectories"
            Case eMSEPaths.DiscardsTrajectories : Return "Results\DiscardsTrajectories"
            Case eMSEPaths.CatchTrajectories : Return "Results\CatchTrajectories"
            Case eMSEPaths.ValueTrajectories : Return "Results\ValueTrajectories"
            Case eMSEPaths.PredationMortality : Return "Results\PredationMortalityTrajectories"
            Case eMSEPaths.PredationMortalityPreyOnly : Return "Results\PredationMortalityPreyOnlyTrajectories"
            Case eMSEPaths.Strategies : Return "HCRs"
            Case eMSEPaths.Regulations : Return "Regulations"
            Case eMSEPaths.BiomassLimits : Return "BiomassLimits"
            Case eMSEPaths.StockAssessment : Return "StockAssessment"
            Case Else
                Debug.Assert(False)
        End Select
        Return ""
    End Function

    Public Shared Function readToTag(strm As StreamReader, Tag As String) As Boolean
        Dim buff As String
        Dim recs() As String

        Try
            Do Until strm.EndOfStream
                buff = strm.ReadLine
                recs = buff.Split(","c)
                If recs(0).Contains(Tag) Then
                    Return True
                End If
            Loop
        Catch ex As Exception
            Debug.Assert(False, "cMSEUtils.readToTag(" + Tag + ") Exception: " + ex.Message)
        End Try

        'Failed to find the tag in the stream
        Return False
    End Function

    ''' <summary>
    ''' Generate a <see cref="cVariableStatus">error flag</see> for use in MSE error messages.
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <param name="status"></param>
    ''' <remarks></remarks>
    Public Shared Sub LogError(msg As cMessage,
                               strMsg As String,
                               Optional status As eStatusFlags = eStatusFlags.ErrorEncountered)
        If (msg IsNot Nothing) Then
            msg.AddVariable(New cVariableStatus(status, strMsg, eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0))
        End If
        m_logger.LogError("LogError. CefasMSE: " & strMsg, eVerboseLevel.Standard)
    End Sub

End Class
