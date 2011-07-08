
#Region "Import"

Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region


Public Class cEcospaceASCResultsWriter
    Inherits cEcospaceBaseResultsWriter


#Region "Private data "


#End Region


#Region "IEcospaceResultsWriter Implementation"

    Public Overrides Sub WriteResults(ByVal SpaceTimeStepResults As Object)
        Dim strm As StreamWriter
        Dim fn As String

        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName("Bomass", igrp, "ASC", tsData.iTimeStep)
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
        End If
    End Sub

    Protected Overrides ReadOnly Property OuputType() As cEcospaceBaseResultsWriter.eSpaceOutputType
        Get
            Return eSpaceOutputType.ASC
        End Get
    End Property

#End Region

#Region "Private methods"


    Private Sub saveASC(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Try
            Me.WriteHeader(strm)
            Me.WriteBody(strm, SpaceTSData, igrp)
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".WriteResults() Exception: " & ex.Message)
        End Try
    End Sub


    Protected Sub WriteHeader(ByRef writer As TextWriter)
        'ToDo compute yllcorner
        'ToDo find cell size in degrees
        writer.WriteLine("ncols       " & Me.SpaceData.InCol)
        writer.WriteLine("nrows       " & Me.SpaceData.InRow)
        'X Lower Left corner (cols)
        writer.WriteLine("xllcorner   " & Me.SpaceData.Lon1) 'org.LonOrigin)
        'Y Lower Left Corner (rows)
        writer.WriteLine("yllcorner   " & Me.SpaceData.Lat1) 'org.LatOrigin)
        writer.WriteLine("cellsize    " & Me.SpaceData.CellLength)
        writer.WriteLine("NODATAVALUE " & cCore.NULL_VALUE)
    End Sub

    Protected Sub WriteBody(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Dim buff As String

        For ir As Integer = Me.SpaceData.InRow To 1 Step -1
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then buff = buff & " "
                buff = buff & Format(SpaceTSData.BiomassMap(ir, ic, igrp), "#########0.0#####")
            Next
            strm.WriteLine(buff)
            buff = ""
        Next

    End Sub

#End Region

   
End Class
