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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.IO
Imports System.Text
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> 
''' and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to save Ecospace results to a series of CSV files.
''' </summary>
''' <remarks>There will be one CSV file for each group containing data for all the time steps.</remarks>
''' ---------------------------------------------------------------------------
Public Class cEcospaceXYZTResultsWriter
    Inherits cEcospaceBaseResultsWriter

    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceXYZTResultsWriter)()

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_XYZT
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.StartWrite"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub StartWrite()
        Try
            If Not Me.CreateOutputDir() Then
                ' ToDo: fail
            End If
            Me.WriteGroupFileHeaders(eVarNameFlags.EcospaceMapBiomass)
            Me.WriteGroupFileHeaders(eVarNameFlags.EcospaceMapCatch)
            Me.WriteFleetFileHeaders(eVarNameFlags.EcospaceMapEffort)

            If Me.m_core.EcospaceModelParameters.ContaminantTracing Then
                Me.WriteGroupFileHeaders(eVarNameFlags.Concentration)
            End If

        Catch ex As Exception
            Me.m_core.Messages.SendMessage(New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_SAVEMAP_FAILED, ex.Message),
                                                        eMessageType.ErrorEncountered, eCoreComponentType.Ecospace, eMessageImportance.Warning))
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.WriteResults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)

        Dim vars() As eVarNameFlags
        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)
        Dim strm As StreamWriter = Nothing
        Dim strFile As String = ""
        Dim ifirstGroup As Integer

        If Me.m_core.EcospaceModelParameters.ContaminantTracing Then
            vars = New eVarNameFlags() {eVarNameFlags.EcospaceMapBiomass, eVarNameFlags.EcospaceMapCatch, eVarNameFlags.Concentration}
            ifirstGroup = 0
        Else
            vars = New eVarNameFlags() {eVarNameFlags.EcospaceMapBiomass, eVarNameFlags.EcospaceMapCatch}
            ifirstGroup = 1
        End If

        Try

            If tsData.iTimeStep < Me.FirstOutputTimeStep Then
                Return
            End If

            For Each varname As eVarNameFlags In vars
                For igrp As Integer = ifirstGroup To Me.m_core.nGroups
                    strFile = Me.GetGroupFileName(varname, igrp, Me.FileExtension())
                    If Not String.IsNullOrWhiteSpace(strFile) AndAlso cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFile), True) Then
                        'Handle file exceptions on a per file basis
                        'this way only the offending file will be skipped
                        'all other files will be written 
                        Try
                            strm = New StreamWriter(strFile, True)
                            If (strm IsNot Nothing) Then
                                Me.SaveCSV(strm, tsData, igrp, varname)
                                strm.Flush()
                                strm.Close()
                                strm = Nothing
                            End If
                        Catch ex As IOException
                            m_logger.LogError(ex, "Ecospace XYZT Export: Failed to write {varname} for group {igrp} to file {strFile}",
                                                varname.ToString(), igrp, strFile)
                            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_EXPORT_FAILED, strFile, ex.Message),
                                                   eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Warning)
                            Me.m_core.Messages.SendMessage(msg)
                        End Try

                    End If
                Next
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".WriteResults Exception: " & ex.Message)
        End Try

        ' Space effort
        For iFlt As Integer = 1 To Me.m_core.m_EcopathData.NumFleet
            strFile = Me.GetFleetFileName(eVarNameFlags.EcospaceMapEffort, iFlt, Me.FileExtension())
            Try
                strm = New StreamWriter(strFile, True)
                If (strm IsNot Nothing) Then
                    Me.SaveCSV(strm, tsData, iFlt, eVarNameFlags.EcospaceMapEffort)
                    strm.Flush()
                    strm.Close()
                    strm = Nothing
                End If
            Catch ex As IOException
                m_logger.LogError(ex, "Ecospace XYZT Export: Failed to write effort for fleet {iFlt} to file {strFile}", iFlt, strFile)
                Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_EXPORT_FAILED, strFile, ex.Message),
                                       eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Warning)
                Me.m_core.Messages.SendMessage(msg)
            End Try
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.EndWrite"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub EndWrite()
        Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_EXPORT_CSV_SUCCESS, Me.m_OutputPath),
                                eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Information)
        ' Provide hyperlink to the directory with the files
        msg.Hyperlink = Me.m_OutputPath
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region ' Overrides

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.FileExtension"/>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function FileExtension() As String
        Return ".csv"
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write run info header.
    ''' </summary>
    ''' <param name="strm">The writer to write to.</param>
    ''' <param name="varname">The variable name to write the header for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteHeader(ByRef strm As StreamWriter, varname As eVarNameFlags, TypeLabel As String, Type As String)

        Try
            Me.WriteRunInfo(strm)
            strm.WriteLine("Variable," + varname.ToString())
            strm.WriteLine(TypeLabel + "," + Type)
            strm.WriteLine()
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the data to a CSV file.
    ''' </summary>
    ''' <param name="strm"></param>
    ''' <param name="timestep">Time step results providing the values to write.</param>
    ''' <param name="iIndex"></param>
    ''' <param name="varname"></param>
    ''' -----------------------------------------------------------------------
    Private Sub SaveCSV(strm As StreamWriter,
                        timestep As cEcospaceTimestep,
                        iIndex As Integer,
                        varname As eVarNameFlags)

        Dim map As cEcospaceLayer = timestep.Layer(varname, iIndex)
        Dim sbBuff As New StringBuilder()
        Dim value As Double = 0

        Debug.Assert(map IsNot Nothing)

        strm.WriteLine("Step," & timestep.iTimeStep.ToString)
        strm.WriteLine("Year," & timestep.TimeStepinYears.ToString)
        For ir As Integer = 1 To Me.EcospaceData.InRow
            For ic As Integer = 1 To Me.EcospaceData.InCol
                If ic > 1 Then sbBuff.Append(",")
                If Me.EcospaceData.Depth(ir, ic) > 0 Then
                    value = CDbl(map.Cell(ir, ic))
                Else
                    value = cCore.NULL_VALUE
                End If
                If (value <> cCore.NULL_VALUE) Then
                    value = Me.ScaleValue(value, timestep, iIndex, varname)
                End If
                sbBuff.Append(cStringUtils.FormatNumber(value))
            Next
            strm.WriteLine(sbBuff.ToString)
            sbBuff.Length = 0
        Next
        strm.WriteLine()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the data to a XYZ formatted file.
    ''' </summary>
    ''' <param name="strm"></param>
    ''' <param name="SpaceTSData"></param>
    ''' <param name="iIndex"></param>
    ''' -----------------------------------------------------------------------
    Private Sub saveXYZ(strm As StreamWriter,
                        SpaceTSData As cEcospaceTimestep,
                        iIndex As Integer,
                        varname As eVarNameFlags)

        Dim map As cEcospaceLayer = SpaceTSData.Layer(varname, iIndex)

        Debug.Assert(map IsNot Nothing)

        ' Write header
        strm.WriteLine("X,Y,Z")
        ' Write data
        For ir As Integer = 1 To Me.EcospaceData.InRow
            For ic As Integer = 1 To Me.EcospaceData.InCol
                strm.WriteLine("{0},{1},{2}", ic, ir, cStringUtils.FormatSingle(CSng(map.Cell(ir, ic))))
            Next
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write headers for the all groups for the given variable.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteGroupFileHeaders(varname As eVarNameFlags)

        Dim strm As StreamWriter
        Dim strFN As String
        Dim grp As String

        Dim ifirst As Integer = 1
        If varname = eVarNameFlags.Concentration Then
            ifirst = 0
        End If


        For igrp As Integer = ifirst To Me.m_core.m_EcopathData.NumGroups
            strFN = Me.GetGroupFileName(varname, igrp, "csv")
            If Not String.IsNullOrWhiteSpace(strFN) AndAlso cFileUtils.IsDirectoryAvailable(Path.GetDirectoryName(strFN), True) Then
                grp = cStringUtils.ToCSVField(Me.m_core.m_EcopathData.GroupName(igrp))
                If igrp = 0 Then grp = "Environment"
                'Create a new file when writing the header
                'this overwrites the data in the current directory
                strm = New StreamWriter(strFN)
                'If Me.m_core.SaveWithFileHeader Then
                Me.WriteHeader(strm, varname, "Contaminant Concentrations", grp)
                'End If
                strm.Close()
                strm = Nothing
            End If
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write headers for the all groups for the given variable.
    ''' </summary>
    ''' <param name="varname"></param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteFleetFileHeaders(varname As eVarNameFlags)

        Dim strm As StreamWriter
        Dim strFN As String

        For iflt As Integer = 1 To Me.m_core.m_EcopathData.NumFleet
            strFN = Me.GetFleetFileName(varname, iflt, "csv")
            'Create a new file when writing the header
            'this overwrites the data in the current directory
            strm = New StreamWriter(strFN)
            'always write the header
            'If Me.m_core.SaveWithFileHeader Then
            Me.WriteHeader(strm, varname, "Fleet name", cStringUtils.ToCSVField(Me.EcopathData.FleetName(iflt)))
            'End If
            strm.Close()
            strm = Nothing
        Next

    End Sub

#End Region ' Internals

End Class
