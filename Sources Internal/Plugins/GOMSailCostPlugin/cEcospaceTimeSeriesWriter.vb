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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Explicit On
Option Strict On

Imports System.IO

Imports EwECore
Imports EwEPlugin
Imports EwEUtils
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities

Public Class cEcospaceTimeSeriesWriter

    Private m_strm As StreamWriter
    Private m_bInit As Boolean
    Private m_Pathdata As cEcopathDataStructures
    Private m_spaceData As cEcospaceDataStructures
    Private m_core As cCore

    Public Function Init(Filename As String, theCore As cCore, EcopathData As cEcopathDataStructures, EcospaceData As cEcospaceDataStructures) As Boolean
        Dim breturn As Boolean = True

        Try
            m_Pathdata = EcopathData
            m_spaceData = EcospaceData
            m_core = theCore
            m_strm = New StreamWriter(Filename)
            breturn = Me.WriteHeader()

        Catch ex As Exception
            breturn = False
        End Try

        m_bInit = breturn
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
