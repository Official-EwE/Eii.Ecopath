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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

Public Class cEmissionTimeSeriesReader

    Private Shared s_groupheaders As String() = {"group", "pool"}
    Private Shared s_areaheaders As String() = {"target", "zone", "region", "mpa", "habitat"}

    Public Sub New()
    End Sub

    Public Function Load(core As cCore, strFiles() As String, data As cData) As Boolean

        Dim msg As cMessage = Nothing
        Dim records As New List(Of cEmissionTimeSeries)

        Try
            For Each strFile As String In strFiles

                Using sw As New StreamReader(strFile)
                    Dim nItems As Integer = 0
                    Dim iItem As Integer = 0
                    Dim iID As Integer = -1
                    Dim dt As Date = Nothing
                    Dim strLine As String = ""
                    Dim bits() As String = Nothing
                    Dim groups As New List(Of Integer)
                    Dim targets As New List(Of Integer)
                    strLine = sw.ReadLine

                    ' -- groups --

                    bits = cStringUtils.SplitQualified(strLine, ",")
                    iID = Me.StartsWith(bits(0).Trim().ToLower, s_groupheaders)
                    If (iID = -1) Then
                        msg = ParseError(My.Resources.PARSE_ERROR_DETAIL_GROUPROW)
                    Else
                        nItems = bits.Count - 1
                    End If

                    For i As Integer = 1 To nItems
                        If msg Is Nothing Then
                            If Not (Integer.TryParse(bits(i), iItem)) Then
                                msg = ParseError(cStringUtils.Localize(My.Resources.PARSE_ERROR_DETAIL_PARSELINE, 1))
                            Else
                                groups.Add(iItem)
                            End If
                        End If
                    Next

                    ' -- area --

                    If (msg Is Nothing) Then
                        strLine = sw.ReadLine
                        bits = cStringUtils.SplitQualified(strLine, ",")
                        iID = Me.StartsWith(bits(0).Trim().ToLower, s_areaheaders)
                        If (iID = -1) Then
                            msg = ParseError(My.Resources.PARSE_ERROR_DETAIL_REGIONROW)
                        Else
                            If (iID = 2) Then data.TargetType = eTargetType.Region
                            If (iID = 3) Then data.TargetType = eTargetType.MPA
                            If (iID = 4) Then data.TargetType = eTargetType.Habitat
                        End If

                    End If

                    If (msg Is Nothing) Then
                        If (bits.Count < nItems) Then
                            msg = ParseError(cStringUtils.Localize(My.Resources.PARSE_ERROR_DETAIL_MISSING, 2))
                        End If
                    End If

                    If (msg Is Nothing) Then
                        For i As Integer = 1 To nItems
                            If Not (Integer.TryParse(bits(i), iItem)) Then
                                msg = ParseError(cStringUtils.Localize(My.Resources.PARSE_ERROR_DETAIL_PARSELINE, 2))
                            Else
                                targets.Add(iItem)
                            End If
                        Next
                    End If

                    ' Define data
                    If (msg Is Nothing) Then

                        For i As Integer = 0 To nItems - 1
                            records.Add(New cEmissionTimeSeries(data, groups(i), targets(i)))
                        Next

                        ' -- data --

                        ' Populate data
                        Dim iLine As Integer = 3
                        While (Not sw.EndOfStream) And (msg Is Nothing)
                            strLine = sw.ReadLine()
                            bits = cStringUtils.SplitQualified(strLine, ",")
                            Try
                                If (bits(0).Length = 4) Then
                                    dt = New Date(CInt(bits(0)), 1, 1)
                                Else
                                    dt = Date.Parse(bits(0))
                                End If
                            Catch ex As Exception
                                msg = ParseError(cStringUtils.Localize(My.Resources.PARSE_ERROR_DETAIL_PARSEDATE, iLine))
                            End Try

                            For i As Integer = 1 To Math.Min(nItems, bits.Length - 1)
                                If (msg Is Nothing) Then
                                    If Not String.IsNullOrWhiteSpace(bits(i)) Then
                                        Dim v As Single = 1
                                        If Single.TryParse(bits(i), v) Then
                                            If (v <> 1) Then
                                                records(i - 1).Datapoint(dt) = v
                                            End If
                                        Else
                                            msg = ParseError(cStringUtils.Localize(My.Resources.PARSE_ERROR_DETAIL_PARSELINE, iLine))
                                        End If
                                    End If
                                End If
                            Next

                            iLine += 1
                        End While
                    End If

                End Using
            Next
        Catch ex As Exception
            msg = ParseError(ex.Message)
        End Try

        If (msg IsNot Nothing) Then
            data.Core.Messages.SendMessage(msg)
            Return False
        End If

        records.Sort(New cEmissionTimeSeriesComparer())
        data.TimeSeries.AddRange(records)

        Return True

    End Function

    Private Function StartsWith(str As String, vals As String()) As Integer

        If (String.IsNullOrWhiteSpace(str)) Then Return -1
        If (vals Is Nothing) Then Return -1

        For i As Integer = 0 To vals.Count - 1
            If str.StartsWith(vals(i)) Then Return i
        Next
        Return -1

    End Function

    Private Function ParseError(strDetail As String) As cMessage
        Return New cMessage(cStringUtils.Localize(My.Resources.PARSE_ERROR_GENERIC, strDetail), eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Critical)
    End Function

End Class
