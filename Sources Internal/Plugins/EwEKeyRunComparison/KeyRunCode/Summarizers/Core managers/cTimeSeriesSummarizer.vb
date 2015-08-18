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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cTimeSeriesSummarizer
    Implements IHashSummarizer

    Private m_core As cCore

    Public Sub New(Core As cCore)
        Me.m_core = Core
    End Sub

    Public Function Name() As String Implements IHashSummarizer.Name
        Return "EcosimTimeSeries"
    End Function

    Public Sub Init() Implements IHashSummarizer.Init

    End Sub

    Public Function HashValues() As System.Collections.Generic.List(Of cHashValues) Implements IHashSummarizer.HashValues

        Dim lHashValues As New List(Of cHashValues)

        If (Me.m_core.ActiveTimeSeriesDatasetIndex <= 0) Then Return lHashValues

        Dim ds As cTimeSeriesDataset = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
        Dim ts As cTimeSeries = Nothing
        Dim sbSummary As New Text.StringBuilder()

        For i As Integer = 0 To ds.Count - 1
            ts = ds(i)
            ' Only hash enabled, active time series
            If (ts.Enabled) Then
                If (sbSummary.Length > 0) Then sbSummary.Append("|")

                sbSummary.Append("p=" & cStringUtils.FormatNumber(CInt(ts.DatPool)))
                sbSummary.Append(",t=" & cStringUtils.FormatNumber(CInt(ts.DataType)))
                sbSummary.Append(",w=" & cStringUtils.FormatNumber(ts.WtType))
                sbSummary.Append(",c=" & cStringUtils.FormatNumber(ts.CV))
                sbSummary.Append(",data=" & cStringConverters.ShapeToString(ts))
            End If
        Next i

        lHashValues.Add(New cHashValues(Me.Name, "TimeSeries", sbSummary.ToString))

        Return lHashValues

    End Function

End Class
