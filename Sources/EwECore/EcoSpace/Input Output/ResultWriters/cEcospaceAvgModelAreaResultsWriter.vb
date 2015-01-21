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

#Region " Imports "

Option Strict On

Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> 
''' and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to write Ecospace output by region to csv files. 
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cEcospaceAvgModelAreaResultsWriter
    Inherits cEcospaceBaseResultsWriter

    'ToDo 12-Dec-2014 This still need to implement the Catch data
    'ToDo 12-Dec-2014 It would be possible to merge this code with the cEcospaceRegionResultsWriter
    'a couple of different way.
    '1. Write a data source for both types of data that provided the annual average and time step data and wrap that in one writer
    '2. Merge the critical code of both class into one AreaAverage writer

#Region " Private classes "

    ''' <summary>
    ''' Local helper class for remembering bits of a landing record.
    ''' </summary>
    Private Class cCatch

        Public Sub New(f As cFleetInput, g As cCoreGroupBase)
            Me.FleetName = f.Name
            Me.FleetIndex = f.Index
            Me.GroupName = g.Name
            Me.GroupIndex = g.Index
        End Sub

        Public Property FleetName As String
        Public Property FleetIndex As Integer
        Public Property GroupName As String
        Public Property GroupIndex As Integer

    End Class

#End Region ' Private classes

#Region " Constructor "

    Public Sub New()
    End Sub

#End Region ' Constructor

#Region " Public access "

    Public Overrides Function FileExtension() As String
        Return ".csv"
    End Function

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

            ' ToDo: globalize this method

            ' Notify user
            msg = New cMessage("Ecospace average results have been saved to '" & Me.OutputDirectory & "'", _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputDirectory
        Catch ex As Exception
            ' Notify user of error
            msg = New cMessage("Ecospace average results could not be saved to '" & Me.OutputDirectory & "'. " & ex.Message, _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
        End Try
        ' Done
        Me.m_core.Messages.SendMessage(msg)

    End Sub

#End Region ' Public access

#Region " Internals "

    ''' <summary>
    ''' Make sure output directory is defined and available.
    ''' </summary>
    Protected Overrides Function CreateOutputDir() As Boolean

        If Me.m_core.m_EcoSpaceData.UseCoreOutputDir Then
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
            cLog.Write("cEcospaceRegionResultWriter failed to create directory " & Me.OutputDirectory)
            Return False
        End If

        Return True

    End Function

#Region " Write Results  "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save results by file per region.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub WriteResult()

        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        ' For all data (0 = biomass, 1 = catch))
        For iData As Integer = 0 To 1
            ' Define file name and data descriptor
            Select Case iData
                Case 0
                    strFile = cFileUtils.ToValidFileName("Ecospace_Average_Biomass.csv", False)
                    strDescriptor = "Average biomass across modeled area"
                Case 1
                    strFile = cFileUtils.ToValidFileName("Ecospace_Annual_Average_Biomass.csv", False)
                    strDescriptor = "Annual average biomass across modeled area"
                Case 2
                    strFile = cFileUtils.ToValidFileName("Ecospace_Average_Catch.csv", False)
                    strDescriptor = "Average catch across modeled area"
                Case 3
                    strFile = cFileUtils.ToValidFileName("Ecospace_Annual_Average_Biomass.csv", False)
                    strDescriptor = "Annual average catch across modeled area"
            End Select

            Try

                ' Start writing
                sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                If Me.m_core.SaveWithFileHeader Then
                    sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                    sw.WriteLine("Data," & cStringUtils.ToCSVField(strDescriptor))
                    sw.WriteLine("Modeled area km2," & cStringUtils.ToCSVField(Me.m_core.m_EcoSpaceData.nWaterCells * Me.m_core.EcospaceBasemap.CellLength() ^ 2))
                    sw.WriteLine("Number of cells," & cStringUtils.ToCSVField(Me.m_core.m_EcoSpaceData.nWaterCells))
                    sw.WriteLine()
                End If

                Select Case iData
                    Case 0
                        Me.WriteBiomassData(sw, False)
                    Case 1
                        Me.WriteBiomassData(sw, True)
                    Case 2
                        'ToDo
                        '  Me.WriteCatchData(sw, r, False)
                    Case 3
                        'ToDo
                        '   Me.WriteCatchData(sw, r, True)
                End Select

                ' Clean up
                sw.Flush()
                sw.Close()
                sw.Dispose()

            Catch ex As Exception
                cLog.Write(ex, "Failed to write Ecospace average biomass to file for data " + strDescriptor)
            End Try

        Next iData

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write biomass data block for a specific region.
    ''' </summary>
    ''' <param name="sw">The streamwriter to write to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteBiomassData(sw As StreamWriter, bAnnual As Boolean)

        Dim g As cCoreGroupBase = Nothing
        Dim nYrs As Integer = 0
        Dim spaceData As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        nYrs = Me.m_core.nEcospaceTimeSteps


        ' Write data header
        If (bAnnual) Then
            sw.Write("Year")
        Else
            sw.Write("TimeStep")
        End If

        For iGroup As Integer = 1 To Me.m_core.nGroups
            g = Me.m_core.EcoPathGroupInputs(iGroup)
            sw.Write("," & cStringUtils.ToCSVField(g.Name))
        Next iGroup
        sw.WriteLine()

        ' Write data block
        If bAnnual Then
            'ANNUAL Data
            'Number of timesteps per year
            Dim nTsYr As Integer = CInt(1.0 / spaceData.TimeStep)

            Dim sumB(Me.m_core.nGroups) As Single
            Dim iCumTime As Integer
            For iYr As Integer = 1 To Me.m_core.nEcospaceYears
                'Sum the biomass for the current year
                For its As Integer = 1 To nTsYr
                    iCumTime += 1
                    For igrp As Integer = 1 To Me.m_core.nGroups
                        sumB(igrp) += spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, igrp, iCumTime)
                    Next

                Next its

                sw.Write(iYr)
                For igrp As Integer = 1 To Me.m_core.nGroups
                    sw.Write(",")
                    sw.Write(cStringUtils.FormatNumber(sumB(igrp) / nTsYr))
                    sumB(igrp) = 0
                Next
                sw.WriteLine()

            Next iYr

        Else 'Monthly

            'Same time step as Ecospace (Monthly by default)
            For iTime As Integer = 1 To Me.m_core.nEcospaceTimeSteps
                sw.Write(iTime)
                For igrp As Integer = 1 To Me.m_core.nGroups
                    sw.Write(",")
                    sw.Write(cStringUtils.FormatNumber(spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, igrp, iTime)))
                Next igrp
                sw.WriteLine()
            Next iTime

        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write catch data block for a specific region.
    ''' </summary>
    ''' <param name="sw">The streamwriter to write to.</param>
    ''' <param name="r">The region to write for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteCatchData(sw As StreamWriter, r As cEcospaceRegionOutput, bAnnual As Boolean)

        Debug.Assert(False, "Oppss " + Me.ToString + ".WriteCatchData() not implemented yet.")
        'Dim fleet As cFleetInput = Nothing
        'Dim group As cCoreGroupBase = Nothing
        'Dim lCatches As New List(Of cCatch)
        'Dim [catch] As cCatch = Nothing
        'Dim n As Integer = 0

        '' Gather all catches
        'For iFleet As Integer = 1 To Me.m_core.nFleets
        '    fleet = Me.m_core.FleetInputs(iFleet)
        '    For iGroup As Integer = 1 To Me.m_core.nGroups
        '        group = Me.m_core.EcoPathGroupInputs(iGroup)
        '        If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) > 0 Then
        '            ' Remember landing
        '            lCatches.Add(New cCatch(fleet, group))
        '        End If
        '    Next iGroup
        'Next iFleet

        '' Write data header
        'Dim sb1 As New StringBuilder()
        'Dim sb2 As New StringBuilder()
        'sb1.Append("Fleet")
        'If (bAnnual) Then
        '    sb2.Append("Year")
        '    n = Me.m_core.nEcospaceYears
        'Else
        '    sb2.Append("Timestep")
        '    n = Me.m_core.nEcospaceTimeSteps
        'End If
        'For iLanding As Integer = 0 To lCatches.Count - 1
        '    [catch] = lCatches(iLanding)
        '    sb1.Append("," & cStringUtils.ToCSVField([catch].FleetName))
        '    sb2.Append("," & cStringUtils.ToCSVField([catch].GroupName))
        'Next
        'sw.WriteLine(sb1.ToString)
        'sw.WriteLine(sb2.ToString)
        'sb1.Clear()
        'sb2.Clear()

        '' Write data block
        'For iTime As Integer = 1 To n
        '    sw.Write(iTime)
        '    For iLanding As Integer = 0 To lCatches.Count - 1
        '        [catch] = lCatches(iLanding)
        '        sw.Write(",")
        '        If (bAnnual) Then
        '            sw.Write(cStringUtils.FormatNumber(r.CatchFleetGroupYear([catch].FleetIndex, [catch].GroupIndex, iTime)))
        '        Else
        '            sw.Write(cStringUtils.FormatNumber(r.CatchFleetGroupTime([catch].FleetIndex, [catch].GroupIndex, iTime)))
        '        End If
        '    Next iLanding
        '    sw.WriteLine()
        'Next iTime

    End Sub


#End Region ' Internals

#End Region

End Class
