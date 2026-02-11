' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> 
''' and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to write Ecospace area averaged results to csv files. This class provides 
''' the framework for writing the file. The actual data is supplied by an implementation 
''' of <see cref="cEcospaceResultsWriterDataSourceBase">cEcospaceResultsWriterDataSourceBase</see> 
''' that supplies the data in a generic format.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceRegionAvgResultsWriter
    Inherits cEcospaceBaseResultsWriter

    Public Const cDATA_NAME As String = "regavg"
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cEcospaceRegionAvgResultsWriter)()

#Region " Private classes "

    ''' <summary>
    ''' Types of result objects
    ''' </summary>
    ''' <remarks></remarks>
    Private Enum eDataSourceTypes
        Biomass
        [Catch]
        RegionBiomass
        RegionCatch
        MOTotalLoss
        RegionConsumption
        RegionLandings
        RegionValue
        RegionTL
    End Enum

#End Region ' Private classes

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region ' Constructor

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.StartWrite"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub StartWrite()
        ' Do not do anything here
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.WriteResults"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub WriteResults(SpaceTimeStepResults As Object)
        ' Take no action. Ecospace results by region are populated only when Ecospace has finished running.
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.EndWrite"/>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub EndWrite()
        Dim msg As cMessage = Nothing

        Try
            ' Create output dir
            Me.CreateOutputDir()
            ' Write it all
            Me.WriteResult()

            ' Notify user
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_SAVED_AVERAGES_SUCCESS, Me.OutputDirectory),
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputDirectory
        Catch ex As Exception
            ' Notify user of error
            msg = New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSPACE_SAVED_AVERAGES_ERROR, Me.OutputDirectory, ex.Message),
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
        End Try
        ' Done
        Me.m_core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="cEcospaceBaseResultsWriter.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property DisplayName As String
        Get
            Return My.Resources.CoreDefaults.ECOSPACE_WRITER_REGAVG
        End Get
    End Property

#End Region ' Public access

#Region " Internals "

    Protected Overrides Function FileExtension() As String
        Return ".csv"
    End Function

    ''' <summary>
    ''' Make sure output directory is defined and available.
    ''' </summary>
    Protected Overrides Function CreateOutputDir() As Boolean

        If Me.m_core.m_EcospaceData.UseCoreOutputDir Then
            Me.m_OutputPath = Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace)
        Else
            If String.IsNullOrWhiteSpace(Me.EcospaceData.EcospaceAreaOutputDir) Then
                Me.m_OutputPath = Me.m_core.OutputPath
            Else
                Me.m_OutputPath = Path.Combine(Me.m_core.OutputPath, Me.EcospaceData.EcospaceAreaOutputDir)
            End If
        End If

        If (Not cFileUtils.IsDirectoryAvailable(Me.OutputDirectory, True)) Then
            Debug.Assert(False, Me.ToString & ".CreateOutputDir() cannot create directory")
            m_logger.LogError("cEcospaceRegionResultWriter failed to create directory " & Me.OutputDirectory)
            Return False
        End If

        Return True

    End Function

    ''' <summary>
    ''' Creates a new cResultsDataSourceBase object
    ''' </summary>
    ''' <param name="ResultType"></param>
    ''' <param name="RegionIndex"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function DataSourceFactory(ResultType As eDataSourceTypes, Optional RegionIndex As Integer = 0) As cEcospaceResultsWriterDataSourceBase
        Dim dataSource As cEcospaceResultsWriterDataSourceBase
        Select Case ResultType
            Case eDataSourceTypes.Biomass
                dataSource = New cBiomassResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.Catch
                dataSource = New cCatchResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.MOTotalLoss
                dataSource = New cMOTotalResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionBiomass
                dataSource = New cRegionBiomassResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionCatch
                dataSource = New cRegionCatchResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionConsumption
                dataSource = New cRegionConsuptionResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionLandings
                dataSource = New cRegionLandingsResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionValue
                dataSource = New cRegionValueResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
            Case eDataSourceTypes.RegionTL
                dataSource = New cRegionValueResultsDataSource(Me.m_core, Me.m_core.m_EcospaceData)
        End Select
        dataSource.Init(RegionIndex)
        Return dataSource
    End Function

#Region " Write Results "

    Private Function initDataSources() As cEcospaceResultsWriterDataSourceBase()

        Dim l As New List(Of cEcospaceResultsWriterDataSourceBase)

        l.Add(Me.DataSourceFactory(eDataSourceTypes.Biomass))
        l.Add(Me.DataSourceFactory(eDataSourceTypes.Catch))
        l.Add(Me.DataSourceFactory(eDataSourceTypes.MOTotalLoss))
        For irgn As Integer = 0 To Me.m_core.nRegions
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionBiomass, irgn))
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionCatch, irgn))
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionLandings, irgn))
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionValue, irgn))
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionConsumption, irgn))
            l.Add(Me.DataSourceFactory(eDataSourceTypes.RegionTL, irgn))
        Next

        Return l.ToArray()

    End Function

    Private Sub WriteResult()

        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        Dim datasources() As cEcospaceResultsWriterDataSourceBase = Me.initDataSources()

        For Each ds As cEcospaceResultsWriterDataSourceBase In datasources

            For Each AvgType As eEcospaceResultsAverageType In System.Enum.GetValues(GetType(eEcospaceResultsAverageType))

                strFile = Me.GetFileName(AvgType, ds)

                Try

                    ' Start writing
                    sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                    If Me.m_core.SaveWithFileHeader Then

                        Dim extra As New Dictionary(Of String, String)
                        extra("Data") = ds.DataDescriptor
                        extra("Area") = ds.AreaDescriptor
                        extra("Number_of_cells") = CStr(ds.nWaterCells)
                        sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace, extraFields:=extra))
                    End If

                    Me.WriteData(sw, ds, AvgType)

                    ' Clean up
                    sw.Flush()
                    sw.Close()
                    sw.Dispose()

                Catch ex As Exception
                    m_logger.LogError(ex, "Failed to write Ecospace average biomass to file for data " + strDescriptor)
                End Try

            Next
        Next

    End Sub

    Private Function GetFileName(AverageType As eEcospaceResultsAverageType, ds As cEcospaceResultsWriterDataSourceBase) As String
        Dim fn As String

        'get the file name from a plugin
        If Me.m_core.PluginManager.EcospaceResultsModelAreaFileName(fn, ds, AverageType) Then
            Return fn
        Else
            'No plugin with the filename
            'So use the default
            Select Case AverageType
                Case eEcospaceResultsAverageType.TimeStep
                    fn = "Ecospace_Average_"
                Case eEcospaceResultsAverageType.Annual
                    fn = "Ecospace_Annual_Average_"
            End Select

            'System.Console.WriteLine(ds.FileNameAbbreviation)

            Return cFileUtils.ToValidFileName(fn + ds.FilenameIdentifier + ".csv", False)
        End If

    End Function

    Private Sub WriteData(sw As StreamWriter, dataSource As cEcospaceResultsWriterDataSourceBase, AvgType As eEcospaceResultsAverageType)
        Dim nYrs As Integer = 0
        Dim spaceData As cEcospaceDataStructures = Me.m_core.m_EcospaceData

        nYrs = Me.m_core.nEcospaceTimeSteps

        ' Write data header
        If AvgType = eEcospaceResultsAverageType.Annual Then
            sw.Write("Year")
        Else
            sw.Write("TimeStep")
        End If

        For iGroup As Integer = 1 To dataSource.nResults
            sw.Write("," & cStringUtils.ToCSVField(dataSource.FieldName(iGroup)))
        Next iGroup
        sw.WriteLine()

        'Dim nTsYr As Integer = CInt(1.0 / spaceData.TimeStep)
        Dim value() As Single = New Single(dataSource.nResults) {}
        Dim bSave As Boolean
        Dim TSLabel As String, Year As Integer
        Dim nAvg(dataSource.nResults) As Integer

        Year = CInt(Math.Truncate((Me.FirstOutputTimeStep - 1) / Me.m_core.m_EcospaceData.nTimeStepsPerYear))

        'Loop over all the time steps
        'If in Annual mode then sum and average the at the end of the year
        For iTime As Integer = Me.FirstOutputTimeStep To Me.m_core.nEcospaceTimeSteps
            For iRslt As Integer = 1 To dataSource.nResults

                If AvgType = eEcospaceResultsAverageType.Annual Then
                    value(iRslt) += dataSource.GetResult(iRslt, iTime)
                    nAvg(iRslt) += 1
                    If ((iTime Mod Me.m_core.m_EcospaceData.nTimeStepsPerYear) = 0) Then
                        'End of the year
                        'Average the results and
                        'Save to file
                        value(iRslt) /= nAvg(iRslt) 'average over the number of data points/timesteps
                        bSave = True
                        nAvg(iRslt) = 0
                    End If
                Else
                    'Save every time step
                    value(iRslt) = dataSource.GetResult(iRslt, iTime)
                    bSave = True
                End If

            Next iRslt

            If bSave Then
                'Grab the label based on the Average Type
                If AvgType = eEcospaceResultsAverageType.Annual Then
                    Year += 1
                    TSLabel = CStr(Year)
                Else
                    TSLabel = CStr(iTime)
                End If

                sw.Write(TSLabel)
                For igrp As Integer = 1 To dataSource.nResults
                    sw.Write(",")
                    sw.Write(cStringUtils.FormatNumber(value(igrp)))
                    bSave = False
                    value(igrp) = 0
                Next igrp

                sw.WriteLine()
            End If

        Next iTime

    End Sub

#End Region ' Write Results

#End Region ' Internals

End Class
