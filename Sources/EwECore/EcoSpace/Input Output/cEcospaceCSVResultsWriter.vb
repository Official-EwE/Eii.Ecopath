
Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cEcospaceCSVResultsWriter
    Implements EwEUtils.Core.IEcospaceResultsWriter

#Region "Private data "

    Private m_core As cCore
    Private m_TimeStampDirName As String

#End Region

#Region "IEcospaceResultsWriter Implementation"


    Public Sub WriteResults(ByVal SpaceTimeStepResults As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults
        Dim strm As StreamWriter
        Dim fn As String

        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName(SpaceTimeStepResults, igrp)
            strm = New StreamWriter(fn, False)


            'saveASC(strm, tsData, igrp)
            'saveCSV(strm, tsData, igrp)
            saveXYZ(strm, tsData, igrp)

            'For ir As Integer = 1 To Me.SpaceData.InRow
            '    For ic As Integer = 1 To Me.SpaceData.InCol
            '        If ic > 1 Then buff = buff & ","
            '        buff = buff & cStringUtils.FormatSingle(tsData.BiomassMap(ir, ic, igrp))
            '    Next
            '    strm.WriteLine(buff)
            '    buff = ""
            'Next

            strm.Close()
            strm = Nothing
        Next

    End Sub


    Public Sub EndWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.EndWrite

    End Sub

    Public Sub Init(ByVal theCore As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.Init
        Me.m_core = theCore
    End Sub

    Public Sub StartWrite() Implements EwEUtils.Core.IEcospaceResultsWriter.StartWrite
        If Me.SpaceData.bSave Then
            Me.CreateTimeStampedDir()
        End If
    End Sub

#End Region

#Region "Private methods"

    Private Sub CreateTimeStampedDir()

        m_TimeStampDirName = System.IO.Path.Combine(Me.m_core.OutputPath, "EcospaceMapOuput " & Me.getTimeStamp)

        If Directory.Exists(Me.TimeStampDirName) Then
            Return
        End If

        Try
            Directory.CreateDirectory(TimeStampDirName)
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".CreateTimeStampedDir() Exception: " & ex.Message)
        End Try
    End Sub

    Private Function getTimeStamp() As String
        Return Format(Date.Now, "y-M-d-H-m-s")
    End Function

    Private ReadOnly Property TimeStampDirName()
        Get
            Return Me.m_TimeStampDirName
        End Get
    End Property


    Private Function getFileName(ByVal SpaceTimeStepResults As cEcospaceTimestep, ByVal iGrp As Integer) As String

        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim time As String = SpaceTimeStepResults.iTimeStep.ToString
        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName("EcoSpaceOutput-Biomass-" & grpName & "-" & time & ".xyz", False)
        Return System.IO.Path.Combine(Me.TimeStampDirName, fn)

    End Function

    Private ReadOnly Property PathData() As cEcopathDataStructures
        Get
            Return Me.m_core.m_EcoPathData
        End Get
    End Property

    Private ReadOnly Property SpaceData() As cEcospaceDataStructures
        Get
            Return Me.m_core.m_EcoSpaceData
        End Get
    End Property

    Private Sub saveCSV(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)

        Dim buff As String
        For ir As Integer = 1 To Me.SpaceData.InRow
            For ic As Integer = 1 To Me.SpaceData.InCol
                If ic > 1 Then buff = buff & ","
                buff = buff & cStringUtils.FormatSingle(SpaceTSData.BiomassMap(ir, ic, igrp))
            Next
            strm.WriteLine(buff)
            buff = ""
        Next

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

    Private Sub saveASC(ByRef strm As StreamWriter, ByVal SpaceTSData As cEcospaceTimestep, ByVal igrp As Integer)
        Me.WriteHeader(strm)
        Me.WriteBody(strm, SpaceTSData, igrp)
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
