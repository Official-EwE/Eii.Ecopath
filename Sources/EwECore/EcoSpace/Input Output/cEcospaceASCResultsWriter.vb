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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region "Import"

Option Strict On
Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to write Ecospace output a ESRI ASC files. 
''' </summary>
''' <remarks>Each ASC file will contain Biomass of a group for a time step</remarks>
Public Class cEcospaceASCResultsWriter
    Inherits cEcospaceBaseResultsWriter

#Region "IEcospaceResultsWriter Implementation"

    Public Overrides Sub StartWrite()
        Try
            Me.CreateOutputDir()
            Me.WriteInfoFile()
        Catch ex As Exception
            Me.m_core.Messages.SendMessage(New cMessage(String.Format(My.Resources.CoreMessages.ECOSPACE_SAVEMAP_FAILED, ex.Message), _
                                                        eMessageType.ErrorEncountered, eCoreComponentType.EcoSpace, eMessageImportance.Warning))
        End Try
    End Sub

    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        Try

            Dim vars() As eVarNameFlags = New eVarNameFlags() {eVarNameFlags.EcospaceMapBiomass, eVarNameFlags.EcospaceMapCatch}
            Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)
            Dim strm As StreamWriter
            Dim fn As String

            For Each varname As eVarNameFlags In vars

                For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
                    fn = Me.getGroupFileName(varname, igrp, Me.GetFileExtension(), tsData.iTimeStep)
                    strm = New StreamWriter(fn, False)

                    saveASC(strm, tsData, igrp, varname)

                    strm.Close()
                    strm = Nothing
                Next
            Next

            ' Sum space effort
            fn = Me.getFleetFileName(eVarNameFlags.EcospaceMapSumEffort, 0, Me.GetFileExtension(), tsData.iTimeStep)
            strm = New StreamWriter(fn, False)
            saveASC(strm, tsData, 0, eVarNameFlags.EcospaceMapSumEffort)
            strm.Close()
            strm = Nothing

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".WriteResults Exception: " & ex.Message)
        End Try


    End Sub

    Public Overrides Sub EndWrite()
        ' ToDo_JS: globalize this message
        Dim msg As New cMessage("Ecospace result ASCII files have been written to " & Me.m_TimeStampDirName, _
                                eMessageType.DataExport, eCoreComponentType.EcoSpace, eMessageImportance.Information)
        ' Provide hyperlink to the directory with the files
        msg.Hyperlink = Me.m_TimeStampDirName
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region

#Region "Private methods"

    Protected Overrides Function GetFileExtension() As String
        Return ".asc"
    End Function

    Protected Function CellSize() As Single
        Dim cellSizeDegrees As Single = Me.SpaceData.CellSize
        If cellSizeDegrees = 0 Then cellSizeDegrees = cEcospaceBasemap.ToCellSize(Me.SpaceData.CellLength)
        Return cellSizeDegrees
    End Function

    Private Sub WriteInfoFile()
        Try
            Dim fn As String
            Dim strm As StreamWriter
            fn = Path.Combine(Me.OutputDirectory, ".Ecospace RunInfo.txt")
            strm = New StreamWriter(fn, False)

            Dim simScen As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
            Dim SpaceScen As String = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name
            Dim ver As String = cCore.Version

            strm.WriteLine("EcoSpace .asc map output")
            strm.WriteLine("EwE version," & cStringUtils.ToCSVField(ver))
            strm.WriteLine("Run date," & Date.Now.ToLongDateString & " " & Date.Now.ToLongTimeString)

            strm.WriteLine("Model," & cStringUtils.ToCSVField(Me.m_core.DataSource.FileName))
            strm.WriteLine("EcoSim Scenario," & cStringUtils.ToCSVField(simScen))
            strm.WriteLine("EcoSpace Scenario," & cStringUtils.ToCSVField(SpaceScen))
            strm.WriteLine("Map rows," & Me.SpaceData.InRow)
            strm.WriteLine("Map cols," & Me.SpaceData.InCol)
            strm.WriteLine("Map cell length," & Me.SpaceData.CellLength)
            strm.WriteLine("Map cell size," & Me.CellSize())
            strm.WriteLine("Map Latitude," & Me.SpaceData.Lat1)
            strm.WriteLine("Map Longitude," & Me.SpaceData.Lon1)
            strm.WriteLine("EcoSpace time step length," & Me.SpaceData.TimeStep.ToString)

            strm.Close()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub saveASC(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer, varName As eVarNameFlags)
        Try
            Me.WriteHeader(strm)
            Me.WriteBody(strm, SpaceTSData, igrp, varName)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub

    Protected Sub WriteHeader(ByRef writer As StreamWriter)

        Dim latLL As Single = Me.SpaceData.Lat1 - (Me.SpaceData.InRow + 1) * Me.CellSize()

        writer.WriteLine("ncols         " & Me.SpaceData.InCol)
        writer.WriteLine("nrows         " & Me.SpaceData.InRow)
        writer.WriteLine("xllcorner     " & Me.SpaceData.Lon1)
        writer.WriteLine("yllcorner     " & latLL)
        'writer.WriteLine("xllcenter     " & (Me.SpaceData.Lon1 + 0.5 * cellSizeDegrees))
        'writer.WriteLine("yllcenter     " & (latLL + 0.5 * cellSizeDegrees))
        writer.WriteLine("cellsize      " & Me.CellSize())
        writer.WriteLine("NODATA_value  " & cCore.NULL_VALUE)

    End Sub

    Protected Sub WriteBody(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal iIndex As Integer, varname As eVarNameFlags)

        Dim map As cEcospaceLayer = SpaceTSData.Layer(varname, iIndex)
        Dim value As Single

        Debug.Assert(map IsNot Nothing)

        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then strm.Write(" ")
                If Me.SpaceData.Depth(ir, ic) > 0 Then
                    value = CSng(map.Cell(ir, ic))
                Else
                    'land as NODATAVALUE
                    value = cCore.NULL_VALUE
                End If
                strm.Write(value)
            Next
            strm.WriteLine("")
        Next

    End Sub

#End Region

End Class
