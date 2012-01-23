#Region "Import"

Option Strict On
Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

''' <summary>
''' Implementation of <see cref="IEcospaceResultsWriter">IEcospaceResultsWriter</see> and <see cref="cEcospaceBaseResultsWriter">cEcospaceBaseResultsWriter</see> 
''' to save Ecospace results to file. 
''' </summary>
''' <remarks>There will be one CSV file for each group containing data for all the time steps.</remarks>
Public Class cEcospaceCSVResultsWriter
    Inherits cEcospaceBaseResultsWriter

#Region "IEcospaceResultsWriter Implementation"

    Public Overrides Sub StartWrite()
        If (Not Me.SpaceData.bSaveCSV) Then Return
        Me.CreateTimeStampedDir()
        Me.WriteFileHeaders(eVarNameFlags.EcospaceMapBiomass)
    End Sub

    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        If (Not Me.SpaceData.bSaveCSV) Then Return

        Dim strm As StreamWriter
        Dim fn As String
        Dim varname As eVarNameFlags = eVarNameFlags.EcospaceMapBiomass
        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving

            fn = Me.getFileName(varname, igrp, Me.getSubDirName())
            strm = New StreamWriter(fn, True)
            saveCSV(strm, tsData, igrp, varname)

            strm.Close()
            strm = Nothing
        Next

    End Sub

    Public Overrides Sub EndWrite()
        If (Not Me.SpaceData.bSaveCSV) Then Return
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
            fn = Me.getFileName(varname, igrp, "CSV")
            strm = New StreamWriter(fn, True)
            Me.WriteHeader(strm, igrp, varname)
            strm.Close()
            strm = Nothing
        Next

    End Sub


    Private Sub WriteHeader(ByRef strm As StreamWriter, ByVal igrp As Integer, ByVal varname As eVarNameFlags)

        Try
            Dim simScen As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
            Dim SpaceScen As String = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name

            strm.WriteLine("Model," & Chr(34) & Me.m_core.DataSource.FileName & Chr(34))
            strm.WriteLine("EcoSim Scenario," & Chr(34) & simScen & Chr(34))
            strm.WriteLine("EcoSpace Scenario," & Chr(34) & SpaceScen & Chr(34))
            strm.WriteLine("Map rows," & Me.SpaceData.InRow)
            strm.WriteLine("Map cols," & Me.SpaceData.InCol)
            strm.WriteLine("Map cell length," & Me.SpaceData.CellLength)
            strm.WriteLine("Map Latitude," & Me.SpaceData.Lat1)
            strm.WriteLine("Map Longitude," & Me.SpaceData.Lon1)
            strm.WriteLine("EcoSpace time step length," & Me.SpaceData.TimeStep.ToString)
            strm.WriteLine("Variable," & varname.ToString())
            strm.WriteLine("Group name," & Chr(34) & Me.PathData.GroupName(igrp) & Chr(34))

            strm.WriteLine()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region


End Class
