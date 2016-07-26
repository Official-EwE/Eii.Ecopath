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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports System.IO

Namespace EcospaceTimeSeries

    Public Class cEcospaceTimeSeriesXYZReader

        Public FileName As String

        Private m_Manager As cEcospaceTimeSeriesManager
        Private m_StartDate As Date
        Private m_EndDate As Date

        Private m_MaxRow As Integer
        Private m_MaxCol As Integer

        Public Sub New(TimeSeriesFile As String, TSManager As cEcospaceTimeSeriesManager)
            Me.FileName = TimeSeriesFile
            Me.m_Manager = TSManager
        End Sub

        Public Function Read() As Boolean

            Me.Init()

            Try
                Dim strm As New StreamReader(Me.FileName)
                Dim header As String
                Dim RecBuffer As String
                Dim rec As cEcospaceTimeSeriesRec

                header = strm.ReadLine()
                'do something with the header???
                'figure out the data format???

                Do While Not strm.EndOfStream
                    RecBuffer = strm.ReadLine()
                    rec = New cEcospaceTimeSeriesRec(RecBuffer)
                    Me.getMinMaxDates(rec)
                    Me.getExtent(rec)
                    Me.m_Manager.Add(rec)
                Loop

                strm.Close()

            Catch ex As Exception
                EwEUtils.Core.cLog.Write(ex, "Ecospace failed to read time series file '" + Me.FileName + "'")
                Return False
            End Try

            Return True
        End Function

        Private Sub Init()
            m_StartDate = New Date(6666, 6, 6)
            m_EndDate = New Date(1, 1, 1)

            Me.m_MaxRow = 0
            Me.m_MaxCol = 0
        End Sub

        Private Sub getMinMaxDates(rec As cEcospaceTimeSeriesRec)

            If Date.Compare(m_StartDate, rec.TimeStamp) > 0 Then
                m_StartDate = rec.TimeStamp
            End If


            If Date.Compare(m_EndDate, rec.TimeStamp) < 0 Then
                m_EndDate = rec.TimeStamp
            End If

        End Sub


        Private Sub getExtent(rec As cEcospaceTimeSeriesRec)
            Me.m_MaxRow = Math.Max(rec.Row, Me.m_MaxRow)
            Me.m_MaxCol = Math.Max(rec.Col, Me.m_MaxCol)
        End Sub


        Public ReadOnly Property StartDate As Date
            Get
                Return Me.m_StartDate
            End Get
        End Property

        Public ReadOnly Property EndDate As Date
            Get
                Return Me.m_EndDate
            End Get
        End Property

        Public ReadOnly Property MaxRow As Integer
            Get
                Return Me.m_MaxRow
            End Get
        End Property


        Public ReadOnly Property MaxCol As Integer
            Get
                Return Me.m_MaxCol
            End Get
        End Property

    End Class

End Namespace
