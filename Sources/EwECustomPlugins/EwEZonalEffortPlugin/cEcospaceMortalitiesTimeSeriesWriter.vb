' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore

#End Region ' Imports

Friend Class cEcospaceMortalitiesTimeSeriesWriter

    Private m_strm As StreamWriter
    Private m_bInit As Boolean
    Private m_Pathdata As cEcopathDataStructures
    Private m_spaceData As cEcospaceDataStructures
    Private m_core As cCore

    Public Function Init(Filename As String, theCore As cCore, EcopathData As cEcopathDataStructures, EcospaceData As cEcospaceDataStructures) As Boolean
        Dim breturn As Boolean = True

        Try
            Me.m_Pathdata = EcopathData
            Me.m_spaceData = EcospaceData
            Me.m_core = theCore
            Me.m_strm = New StreamWriter(Filename)
            breturn = Me.WriteHeader()

        Catch ex As Exception
            breturn = False
        End Try

        Me.m_bInit = breturn
        Return breturn

    End Function


    Private Function WriteHeader() As Boolean
        Dim breturn As Boolean = True

        Try
            Me.m_strm.WriteLine("Ecospace total mortality")
            Me.m_strm.Write("Group/Timestep")
            For itime As Integer = 1 To Me.m_spaceData.nTimeSteps
                Me.m_strm.Write("," + itime.ToString)
            Next itime
            Me.m_strm.WriteLine()

        Catch ex As Exception
            breturn = False
        End Try

        Return breturn

    End Function

    Public Function Write() As Boolean
        Try
            If Not Me.m_bInit Then Return False

            For igrp As Integer = 1 To Me.m_spaceData.NGroups

                Me.m_strm.Write(Me.m_Pathdata.GroupName(igrp))
                For it As Integer = 1 To Me.m_spaceData.nTimeSteps
                    'TotalLoss is the sum of loss across all the cells
                    Me.m_strm.Write("," + Me.m_spaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.TotalLoss, igrp, it).ToString)
                Next
                Me.m_strm.WriteLine()
            Next igrp

            Me.m_strm.Close()

        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function

End Class
