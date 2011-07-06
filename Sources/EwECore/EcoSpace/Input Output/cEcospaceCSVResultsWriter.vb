
Imports System.IO
Imports System.Text
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cEcospaceCSVResultsWriter
    Implements EwEUtils.Core.IEcospaceResultsWriter

#Region "Private data "

    Private m_core As cCore

#End Region

#Region "IEcospaceResultsWriter Implementation"


    Public Sub WriteResults(ByVal SpaceTimeStepResults As Object) Implements EwEUtils.Core.IEcospaceResultsWriter.WriteResults
        Dim strm As StreamWriter
        Dim fn As String
        Dim buff As String

        Dim tsData As cEcospaceTimestep = DirectCast(SpaceTimeStepResults, cEcospaceTimestep)

        For igrp As Integer = 1 To Me.m_core.m_EcoPathData.NumLiving
            fn = Me.getFileName(SpaceTimeStepResults, igrp)
            strm = New StreamWriter(fn, False)

            For ir As Integer = 1 To Me.SpaceData.InRow
                For ic As Integer = 1 To Me.SpaceData.InCol
                    If ic > 1 Then buff = buff & ","
                    buff = buff & cStringUtils.FormatSingle(tsData.BiomassMap(ir, ic, igrp))
                Next
                strm.WriteLine(buff)
                buff = ""
            Next

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

    End Sub

#End Region

#Region "Private methods"

    Private Function getFileName(ByVal SpaceTimeStepResults As cEcospaceTimestep, ByVal iGrp As Integer) As String
        Dim grpName As String = Me.m_core.m_EcoPathData.GroupName(iGrp)
        Dim time As String = SpaceTimeStepResults.iTimeStep.ToString
        Dim fn As String = EwEUtils.Utilities.cFileUtils.ToValidFileName("EcoSpaceOutput-Biomass-" & grpName & "-" & time & ".csv", False)
        Return System.IO.Path.Combine(Me.m_core.OutputPath, fn)


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



#End Region

End Class
