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
Public Class cEcospaceRegionResultWriter
    Inherits cEcospaceBaseResultsWriter

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

        ' Is this needed?
        If (Me.m_core.nRegions = 0) Then Return

        Dim msg As cMessage = Nothing

        Try
            ' Create output dir
            Me.CreateOutputDir()
            ' Write it all
            Me.WriteResultByRegion()
            'Me.WriteResultsByGroup()

            ' ToDo: globalize this method

            ' Notify user
            msg = New cMessage("Ecospace results by region have been saved to '" & Me.OutputDirectory & "'", _
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
            msg.Hyperlink = Me.OutputDirectory
        Catch ex As Exception
            ' Notify user of error
            msg = New cMessage("Ecospace results by region could not be saved to '" & Me.OutputDirectory & "'. " & ex.Message, _
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
            'Use the output directory set by the user
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

#Region " Results by group "

    ''' <summary>
    ''' Write results, each group in a separate file across regions
    ''' </summary>
    Private Sub WriteResultsByGroup()

        Dim r As cEcospaceRegionOutput = Nothing
        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        ' For all groups
        For iGroup As Integer = 1 To Me.m_core.nGroups
            ' Get name
            strName = Me.EcopathData.GroupName(iGroup)
            ' For all data (0 = biomass, 1 = catch))
            For iData As Integer = 0 To 1
                ' Define file name and data descriptor
                If (iData = 0) Then
                    strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("Biomass_{0}.csv", strName), False)
                    strDescriptor = "Biomass by region"
                Else
                    strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("Catch_{0}.csv", strName), False)
                    strDescriptor = "Catch by region"
                End If

                ' Start writing
                sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                If Me.m_core.SaveWithFileHeader Then
                    sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                    sw.WriteLine("Data," & cStringUtils.ToCSVField(strDescriptor))
                    sw.WriteLine("Group," & cStringUtils.ToCSVField(strName))
                    sw.WriteLine()
                End If

                ' Write data header
                sw.Write("TimeStep")
                For iRegion As Integer = 0 To Me.m_core.nRegions
                    r = Me.m_core.EcospaceRegionOutput(iRegion)
                    sw.Write("," & cStringUtils.ToCSVField(r.Name))
                Next iRegion
                sw.WriteLine()

                ' Write data
                For iTime As Integer = 1 To Me.m_core.nEcospaceTimeSteps
                    sw.Write(iTime)
                    For iRegion As Integer = 0 To Me.m_core.nRegions
                        r = Me.m_core.EcospaceRegionOutput(iRegion)
                        If (iData = 0) Then
                            sValue = r.BiomassByTime(iGroup, iTime)
                        Else
                            ' For now sum all catches. This needs to change so that each landing is reported by region
                            For ifleet As Integer = 1 To Me.m_core.nFleets
                                sValue += r.CatchFleetGroupTime(ifleet, iGroup, iTime)
                            Next
                        End If
                        sw.Write("," & cStringUtils.FormatNumber(sValue))
                    Next iRegion
                    sw.WriteLine()
                Next iTime

                ' Clean up
                sw.Flush()
                sw.Close()
                sw.Dispose()

            Next iData
        Next iGroup

    End Sub

#End Region ' Results by group

#Region " Results by region "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save results by file per region.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub WriteResultByRegion()

        Dim r As cEcospaceRegionOutput = Nothing
        Dim sw As StreamWriter = Nothing
        Dim strName As String = ""
        Dim strFile As String = ""
        Dim strDescriptor As String = ""
        Dim sValue As Single = 0

        ' For all groups
        For iRegion As Integer = 1 To Me.m_core.nRegions

            ' Get region name
            r = Me.m_core.EcospaceRegionOutput(iRegion)
            strName = r.Name

            ' For all data (0 = biomass, 1 = catch))
            For iData As Integer = 0 To 3
                ' Define file name and data descriptor
                Select Case iData
                    Case 0
                        strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("{0}_biomass.csv", strName), False)
                        strDescriptor = "Average biomass by region"
                    Case 1
                        strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("{0}_biomass_annual.csv", strName), False)
                        strDescriptor = "Annual average biomass by region"
                    Case 2
                        strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("{0}_catch.csv", strName), False)
                        strDescriptor = "Average catch by region"
                    Case 3
                        strFile = cFileUtils.ToValidFileName(cStringUtils.Localize("{0}_catch_annual.csv", strName), False)
                        strDescriptor = "Annual average catch by region"
                End Select

                Try
                    ' Start writing
                    sw = New StreamWriter(Path.Combine(Me.OutputDirectory, strFile))
                    If Me.m_core.SaveWithFileHeader Then
                        sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
                        sw.WriteLine("Data," & cStringUtils.ToCSVField(strDescriptor))
                        sw.WriteLine("Region," & cStringUtils.ToCSVField(strName))
                        sw.WriteLine("Region area km2," & cStringUtils.ToCSVField(Me.m_core.m_EcoSpaceData.nCellsInRegion(iRegion) * Me.m_core.EcospaceBasemap.CellLength() ^ 2))
                        sw.WriteLine("Number of cells," & cStringUtils.ToCSVField(Me.m_core.m_EcoSpaceData.nCellsInRegion(iRegion)))
                        sw.WriteLine()
                    End If

                    Select Case iData
                        Case 0
                            Me.WriteBiomassData(sw, r, False)
                        Case 1
                            Me.WriteBiomassData(sw, r, True)
                        Case 2
                            Me.WriteCatchData(sw, r, False)
                        Case 3
                            Me.WriteCatchData(sw, r, True)
                    End Select

                    ' Clean up
                    sw.Flush()
                    sw.Close()
                    sw.Dispose()
                Catch ex As Exception
                    cLog.Write(ex, "Failed to save Ecospace average biomass file for " + strDescriptor)
                End Try

            Next iData
        Next iRegion

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write biomass data block for a specific region.
    ''' </summary>
    ''' <param name="sw">The streamwriter to write to.</param>
    ''' <param name="r">The region to write for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteBiomassData(sw As StreamWriter, r As cEcospaceRegionOutput, bAnnual As Boolean)

        Dim g As cCoreGroupBase = Nothing
        Dim n As Integer = 0

        ' Write data header
        If (bAnnual) Then
            sw.Write("Year")
            n = Me.m_core.nEcospaceYears
        Else
            sw.Write("TimeStep")
            n = Me.m_core.nEcospaceTimeSteps
        End If

        For iGroup As Integer = 1 To Me.m_core.nGroups
            g = Me.m_core.EcoPathGroupInputs(iGroup)
            sw.Write("," & cStringUtils.ToCSVField(g.Name))
        Next iGroup
        sw.WriteLine()

        ' Write data block
        For iTime As Integer = 1 To n
            sw.Write(iTime)
            For iGroup As Integer = 1 To Me.m_core.nGroups
                sw.Write(",")
                If (bAnnual) Then
                    sw.Write(cStringUtils.FormatNumber(r.BiomassByYear(iGroup, iTime)))
                Else
                    sw.Write(cStringUtils.FormatNumber(r.BiomassByTime(iGroup, iTime)))
                End If
            Next iGroup
            sw.WriteLine()
        Next iTime

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write catch data block for a specific region.
    ''' </summary>
    ''' <param name="sw">The streamwriter to write to.</param>
    ''' <param name="r">The region to write for.</param>
    ''' -----------------------------------------------------------------------
    Private Sub WriteCatchData(sw As StreamWriter, r As cEcospaceRegionOutput, bAnnual As Boolean)

        Dim fleet As cFleetInput = Nothing
        Dim group As cCoreGroupBase = Nothing
        Dim lCatches As New List(Of cCatch)
        Dim [catch] As cCatch = Nothing
        Dim n As Integer = 0

        ' Gather all catches
        For iFleet As Integer = 1 To Me.m_core.nFleets
            fleet = Me.m_core.FleetInputs(iFleet)
            For iGroup As Integer = 1 To Me.m_core.nGroups
                group = Me.m_core.EcoPathGroupInputs(iGroup)
                If (fleet.Landings(iGroup) + fleet.Discards(iGroup)) > 0 Then
                    ' Remember landing
                    lCatches.Add(New cCatch(fleet, group))
                End If
            Next iGroup
        Next iFleet

        ' Write data header
        Dim sb1 As New StringBuilder()
        Dim sb2 As New StringBuilder()
        sb1.Append("Fleet")
        If (bAnnual) Then
            sb2.Append("Year")
            n = Me.m_core.nEcospaceYears
        Else
            sb2.Append("Timestep")
            n = Me.m_core.nEcospaceTimeSteps
        End If
        For iLanding As Integer = 0 To lCatches.Count - 1
            [catch] = lCatches(iLanding)
            sb1.Append("," & cStringUtils.ToCSVField([catch].FleetName))
            sb2.Append("," & cStringUtils.ToCSVField([catch].GroupName))
        Next
        sw.WriteLine(sb1.ToString)
        sw.WriteLine(sb2.ToString)
        sb1.Clear()
        sb2.Clear()

        ' Write data block
        For iTime As Integer = 1 To n
            sw.Write(iTime)
            For iLanding As Integer = 0 To lCatches.Count - 1
                [catch] = lCatches(iLanding)
                sw.Write(",")
                If (bAnnual) Then
                    sw.Write(cStringUtils.FormatNumber(r.CatchFleetGroupYear([catch].FleetIndex, [catch].GroupIndex, iTime)))
                Else
                    sw.Write(cStringUtils.FormatNumber(r.CatchFleetGroupTime([catch].FleetIndex, [catch].GroupIndex, iTime)))
                End If
            Next iLanding
            sw.WriteLine()
        Next iTime

    End Sub

#End Region ' Results by region

#End Region ' Internals

End Class
