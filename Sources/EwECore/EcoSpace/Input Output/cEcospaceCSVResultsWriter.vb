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
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to save Ecospace results to file. 
''' </summary>
''' <remarks>There will be one CSV file for each group containing data for all the time steps.</remarks>
Public Class cEcospaceCSVResultsWriter
    Inherits cEcospaceBaseResultsWriter

#Region "IEcospaceResultsWriter Implementation"

    Public Overrides Sub StartWrite()
        Try
            Me.CreateOutputDir()
            Me.WriteFileHeaders(eVarNameFlags.EcospaceMapBiomass)
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

                    fn = Me.getGroupFileName(varname, igrp, Me.getSubDirName())
                    strm = New StreamWriter(fn, True)
                    saveCSV(strm, tsData, igrp, varname)

                    strm.Close()
                    strm = Nothing
                Next

            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".WriteResults Exception: " & ex.Message)
        End Try

    End Sub

    Public Overrides Sub EndWrite()
    End Sub

#End Region

#Region "Private methods"

    Protected Overrides ReadOnly Property OuputType() As cEcospaceBaseResultsWriter.eSpaceOutputType
        Get
            Return eSpaceOutputType.CSV
        End Get
    End Property

    Private Sub saveCSV(ByRef strm As StreamWriter, ByVal timestep As cEcospaceTimestep, ByVal iIndex As Integer, varname As eVarNameFlags)

        Dim map As cEcospaceLayer = timestep.Layer(varname, iIndex)
        Dim sbBuff As New StringBuilder()

        Debug.Assert(map IsNot Nothing)

        strm.WriteLine("Step," & timestep.iTimeStep.ToString)
        'TimeNow is the loop counter in Ecospace and is not updated until the end of the loop
        'For the Year of this time step we need to add delta T
        strm.WriteLine("Year," & timestep.TimeStepinYears.ToString)
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then sbBuff.Append(",")
                sbBuff.Append(cStringUtils.FormatSingle(CSng(map.Cell(ir, ic))))
            Next
            strm.WriteLine(sbBuff.ToString)
            sbBuff.Length = 0
        Next
        strm.WriteLine()

    End Sub

    ''' <summary>
    ''' Not used here but saves the data to an XYZ formatted file
    ''' </summary>
    ''' <param name="strm"></param>
    ''' <param name="SpaceTSData"></param>
    ''' <param name="iIndex"></param>
    ''' <remarks></remarks>
    Private Sub saveXYZ(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal iIndex As Integer, varname As eVarNameFlags)

        Dim map As cEcospaceLayer = SpaceTSData.Layer(varname, iIndex)

        Debug.Assert(map IsNot Nothing)

        ' Write header
        strm.WriteLine("X,Y,Z")
        ' Write data
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                strm.WriteLine("{0},{1},{2}", ic, ir, cStringUtils.FormatSingle(CSng(map.Cell(ir, ic))))
            Next
        Next

    End Sub

    Private Sub WriteFileHeaders(ByVal varname As eVarNameFlags)

        Dim strm As StreamWriter
        Dim fn As String

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getGroupFileName(varname, igrp, "CSV")
            'Create a new file when writting the header
            'this overwrites the data in the current directory
            strm = New StreamWriter(fn)
            Me.WriteHeader(strm, igrp, varname)
            strm.Close()
            strm = Nothing
        Next

    End Sub


    Private Sub WriteHeader(ByRef strm As StreamWriter, ByVal igrp As Integer, ByVal varname As eVarNameFlags)

        Try
            Dim simScen As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
            Dim SpaceScen As String = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name
            Dim ver As String = cCore.Version

            strm.WriteLine("Model," & cStringUtils.ToCSVField(Me.m_core.DataSource.FileName))
            strm.WriteLine("EwE version," & ver)
            strm.WriteLine("Run date," & Date.Now.ToLongDateString & " " & Date.Now.ToLongTimeString)

            strm.WriteLine("EcoSim Scenario," & cStringUtils.ToCSVField(simScen))
            strm.WriteLine("EcoSpace Scenario," & cStringUtils.ToCSVField(SpaceScen))
            strm.WriteLine("Map rows," & Me.SpaceData.InRow)
            strm.WriteLine("Map cols," & Me.SpaceData.InCol)
            strm.WriteLine("Map cell length," & Me.SpaceData.CellLength)
            strm.WriteLine("Map Latitude," & Me.SpaceData.Lat1)
            strm.WriteLine("Map Longitude," & Me.SpaceData.Lon1)
            strm.WriteLine("EcoSpace time step length," & Me.SpaceData.TimeStep.ToString)
            strm.WriteLine("Variable," & varname.ToString())
            strm.WriteLine("Group name," & cStringUtils.ToCSVField(Me.PathData.GroupName(igrp)))

            strm.WriteLine()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region


End Class
