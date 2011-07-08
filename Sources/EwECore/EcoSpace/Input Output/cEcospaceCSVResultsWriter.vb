
#Region "Import"

Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

Public Class cEcospaceCSVResultsWriter
    Inherits cEcospaceBaseResultsWriter


#Region "Private data "


#End Region


#Region "IEcospaceResultsWriter Implementation"

    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)
        Dim strm As StreamWriter
        Dim fn As String

        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName("Biomass", igrp, "CSV")
            strm = New StreamWriter(fn, True)

            saveCSV(strm, tsData, igrp)

            strm.Close()
            strm = Nothing
        Next

    End Sub


    Public Overrides Sub EndWrite()

    End Sub

    Public Overrides Sub StartWrite()
        If Me.SpaceData.bSaveCSV Then
            Me.CreateTimeStampedDir()
            Me.WriteFileHeaders()
        End If
    End Sub


    Protected Overrides ReadOnly Property OuputType() As cEcospaceBaseResultsWriter.eSpaceOutputType
        Get
            Return eSpaceOutputType.CSV
        End Get
    End Property

#End Region

#Region "Private methods"

    Private Sub saveCSV(ByRef strm As StreamWriter, ByVal Results As cEcospaceTimestep, ByVal igrp As Integer)
        Dim buff As String
        strm.WriteLine("Step," & Results.iTimeStep.ToString)
        'TimeNow is the loop counter in Ecospace and is not updated until the end of the loop
        'For the Year of this time step we need to add delta T
        strm.WriteLine("Year," & (SpaceData.TimeNow + Me.SpaceData.TimeStep).ToString)
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(Results.BiomassMap(ir, ic, igrp))
            Next
            strm.WriteLine(buff)
            buff = ""
        Next

        strm.WriteLine()

    End Sub

    Private Sub saveXYZ(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Dim buff As String
        strm.WriteLine("X,Y,Z")
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                buff = ic.ToString & "," & ir.ToString & "," & cStringUtils.FormatSingle(SpaceTSData.BiomassMap(ir, ic, igrp))
                strm.WriteLine(buff)
                buff = ""
            Next
        Next
    End Sub

    Private Sub WriteFileHeaders()
        Dim strm As StreamWriter
        Dim fn As String

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName("Biomass", igrp, "CSV")
            strm = New StreamWriter(fn, True)
            Me.WriteHeader(strm, igrp, "Biomass")
            strm.Close()
            strm = Nothing
        Next

    End Sub


    Private Sub WriteHeader(ByRef strm As StreamWriter, ByVal igrp As Integer, ByVal Variable As String)

        Try
            Dim simScen As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
            Dim SpaceScen As String = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name

            strm.WriteLine("Model," & Chr(34) & Me.m_core.DataSource.FileName & Chr(34))
            strm.WriteLine("Variable," & Variable)
            strm.WriteLine("Group name," & Chr(34) & Me.PathData.GroupName(igrp) & Chr(34))
            strm.WriteLine("EcoSim Scenario," & Chr(34) & simScen & Chr(34))
            strm.WriteLine("EcoSpace Scenario," & Chr(34) & SpaceScen & Chr(34))
            strm.WriteLine("EcoSpace time step length," & Me.SpaceData.TimeStep.ToString)
            strm.WriteLine("Map rows," & Me.SpaceData.InRow)
            strm.WriteLine("Map cols," & Me.SpaceData.InCol)
            strm.WriteLine("Map cell length," & Me.SpaceData.CellLength)
            strm.WriteLine("Map Latitude," & Me.SpaceData.Lat1)
            strm.WriteLine("Map Longitude," & Me.SpaceData.Lon1)

            strm.WriteLine()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub

#End Region


End Class
