
#Region "Import"

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

    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)
        Dim strm As StreamWriter
        Dim fn As String

        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName("Biomass", igrp, "ASC", tsData.iTimeStep)
            strm = New StreamWriter(fn, False)

            saveASC(strm, tsData, igrp)

            strm.Close()
            strm = Nothing
        Next

    End Sub


    Public Overrides Sub EndWrite()

    End Sub

    Public Overrides Sub StartWrite()
        If Me.SpaceData.bSaveASC Then
            Me.CreateTimeStampedDir()
            Me.WriteInfoFile()
        End If
    End Sub

    Protected Overrides ReadOnly Property OuputType() As cEcospaceBaseResultsWriter.eSpaceOutputType
        Get
            Return eSpaceOutputType.ASC
        End Get
    End Property

#End Region

#Region "Private methods"


    Private Sub WriteInfoFile()
        Try
            Dim fn As String
            Dim strm As StreamWriter
            fn = Path.Combine(Me.TimeStampDirName, ".Ecospace RunInfo.txt")
            strm = New StreamWriter(fn, False)

            Dim simScen As String = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex).Name
            Dim SpaceScen As String = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex).Name
            Dim ver As String = System.Reflection.Assembly.GetAssembly(GetType(cCore)).GetName.Version.ToString

            strm.WriteLine("EcoSpace ASC map output")
            strm.WriteLine("EwE version," & ver)
            strm.WriteLine("Run date," & Date.Now.ToLongDateString & " " & Date.Now.ToLongTimeString)

            strm.WriteLine("Model," & Chr(34) & Me.m_core.DataSource.FileName & Chr(34))
            strm.WriteLine("EcoSim Scenario," & Chr(34) & simScen & Chr(34))
            strm.WriteLine("EcoSpace Scenario," & Chr(34) & SpaceScen & Chr(34))
            strm.WriteLine("Map rows," & Me.SpaceData.InRow)
            strm.WriteLine("Map cols," & Me.SpaceData.InCol)
            strm.WriteLine("Map cell length," & Me.SpaceData.CellLength)
            strm.WriteLine("Map Latitude," & Me.SpaceData.Lat1)
            strm.WriteLine("Map Longitude," & Me.SpaceData.Lon1)
            strm.WriteLine("EcoSpace time step length," & Me.SpaceData.TimeStep.ToString)

            strm.Close()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub saveASC(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Try
            Me.WriteHeader(strm)
            Me.WriteBody(strm, SpaceTSData, igrp)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub


    Protected Sub WriteHeader(ByRef writer As TextWriter)

        Dim cellSizeDegrees As Single = Me.SpaceData.CellLength * cEcospaceDataStructures.KM_TO_DEGRESS
        Dim latLL As Single = Me.SpaceData.Lat1 + (Me.SpaceData.InRow + 1) * cellSizeDegrees

        writer.WriteLine("ncols       " & Me.SpaceData.InCol)
        writer.WriteLine("nrows       " & Me.SpaceData.InRow)
        'X Lower Left corner (cols)
        writer.WriteLine("xllcorner   " & Me.SpaceData.Lon1) 'org.LonOrigin)
        'Y Lower Left Corner (rows)
        writer.WriteLine("yllcorner   " & latLL) 'org.LatOrigin)
        writer.WriteLine("cellsize    " & cellSizeDegrees)
        writer.WriteLine("NODATAVALUE " & cCore.NULL_VALUE)
    End Sub

    Protected Sub WriteBody(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Dim buff As String
        Dim bcell As Single
        For ir As Integer = Me.SpaceData.InRow To 1 Step -1
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then buff = buff & " "
                If Me.SpaceData.Depth(ir, ic) > 0 Then
                    bcell = SpaceTSData.BiomassMap(ir, ic, igrp)
                Else
                    'land as NODATAVALUE
                    bcell = cCore.NULL_VALUE
                End If
                buff = buff & Format(bcell, "#########0.0#####")
            Next
            strm.WriteLine(buff)
            buff = ""
        Next

    End Sub

#End Region

End Class
