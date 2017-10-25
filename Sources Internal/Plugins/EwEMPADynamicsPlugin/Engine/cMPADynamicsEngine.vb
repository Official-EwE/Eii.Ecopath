Option Strict On
Imports System.Globalization
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
#Region " Imports "

Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class cMPADynamicsEngine

    Private m_core As cCore = Nothing
    Private m_ds As cEcospaceDataStructures = Nothing
    Private m_dtStates As New Dictionary(Of Date, List(Of cMPAState))
    Private m_lPreserved As New List(Of cMPAState)

    Public Sub New(core As cCore, ds As cEcospaceDataStructures)
        Me.m_core = core
        Me.m_ds = ds
    End Sub

    Public Sub Clear()
        Me.Restore()
        Me.m_dtStates.Clear()
    End Sub

    Public Sub Backup()

        Me.m_lPreserved.Clear()
        Dim timestamp As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(1)
        For iMPA As Integer = 1 To Me.m_ds.MPAno
            Dim state As New cMPAState(Me.m_ds, iMPA, timestamp)
            state.Load()
            Me.m_lPreserved.Add(state)
        Next

    End Sub

    Public Sub Restore()
        For Each state As cMPAState In Me.m_lPreserved
            state.Apply()
        Next
        Me.m_lPreserved.Clear()
    End Sub

    Public Sub OnEcospaceTimeStep(iTime As Integer)

        ' ToDo: globalize this method

        Dim timestamp As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)
        If (Me.m_dtStates.ContainsKey(timestamp)) Then
            For Each state As cMPAState In Me.m_dtStates(timestamp)
                state.Apply()
                SendStatusMessage(cStringUtils.Localize("{0}: Updated {1}: {2}; {3}",
                                                        timestamp.ToShortDateString(),
                                                        state.ToString(),
                                                        state.ClosureState(),
                                                        state.RegulationState()),
                                  eMessageImportance.Information)
            Next
        End If

    End Sub

    Private Shared sFORMATS As String() = New String() {"yyyy/MM", "yyyy-MM"}
    Private Shared sLOCALE As New CultureInfo("en-US")

    ' Hack 'n slash
    Public Function LoadCSV(strCSV As String) As Boolean

        ' ToDo: globalize this method

        Me.m_dtStates.Clear()

        strCSV = Path.GetFullPath(strCSV)
        Try
            Dim strText As String = ""
            Using sr As New StreamReader(strCSV)
                strText = sr.ReadToEnd()
            End Using

            Dim dt As DataTable = Me.LoadText(strText)
            If (dt Is Nothing) Then Return False
            Dim bTimeStepMode As Boolean = dt.Columns.Contains("timestep")

            For Each drow As DataRow In dt.Rows

                Dim timestamp As Date
                If (bTimeStepMode) Then
                    timestamp = Me.m_core.EcospaceTimestepToAbsoluteTime(CInt(drow("timestep")))
                Else
                    Date.TryParseExact(CStr(drow("date")), sFORMATS, sLOCALE, DateTimeStyles.None, timestamp)
                End If

                Dim state As New cMPAState(Me.m_ds, Me.ToMPA(CStr(drow("MPA"))), timestamp)
                For i As Integer = 1 To cCore.N_MONTHS
                    state.IsClosed(i) = ToCheckState(Me.ReadSafe(drow, "m" & i, ""))
                Next

                For i As Integer = 1 To Me.m_core.nFleets
                    state.IsEnforced(i) = ToCheckState(Me.ReadSafe(drow, "f" & i, ""))
                Next

                If (Not Me.m_dtStates.ContainsKey(timestamp)) Then
                    Me.m_dtStates(timestamp) = New List(Of cMPAState)
                End If
                Me.m_dtStates(timestamp).Add(state)

            Next

            SendStatusMessage(cStringUtils.Localize("MPA Dynamics CSV file {0} loaded", strCSV), eMessageImportance.Information)
            Return True

        Catch ex As Exception
            SendStatusMessage(cStringUtils.Localize("MPA Dynamics CSV file {0} faied to load. {1}", strCSV, ex.Message), eMessageImportance.Information)
        End Try

        Return False

    End Function

    Public ReadOnly Property MPAStates(bIncludeStartup As Boolean) As ICollection(Of cMPAState)
        Get
            Dim lStates As New List(Of cMPAState)

            If (bIncludeStartup) Then
                Dim timestamp As New Date(1, 1, 1)
                For iMPA As Integer = 1 To Me.m_ds.MPAno
                    Dim state As New cMPAState(Me.m_ds, iMPA, timestamp)
                    state.Load()
                    lStates.Add(state)
                Next
            End If

            For Each value As List(Of cMPAState) In Me.m_dtStates.Values
                lStates.AddRange(value)
            Next
            lStates.Sort(New cMPAStateComparer)
            Return lStates
        End Get
    End Property

    'Public Function LoadExcel(strExcel As String) As Boolean
    '    Me.Clear()

    '    Dim bOK As Boolean = True
    '    Dim dt As New DataTable()

    '    strExcelFile = Path.GetFullPath(strExcelFile)

    '    Using pck As New ExcelPackage()
    '        Try
    '            Using strm As Stream = File.OpenRead(strExcelFile)
    '                pck.Load(strm)
    '            End Using
    '        Catch ex As Exception
    '            StatusHandler.Log("Unable to load Excel file '" & strExcelFile & "': " & ex.Message, eAlert.Error)
    '            Return Nothing
    '        End Try

    '        Dim ws As ExcelWorksheet = Nothing
    '        If (Not String.IsNullOrWhiteSpace(strWorksheet)) Then
    '            For Each wsTemp As ExcelWorksheet In pck.Workbook.Worksheets
    '                If (String.Compare(wsTemp.Name, strWorksheet, True) = 0) Then ws = wsTemp
    '            Next
    '            If (ws Is Nothing) Then
    '                StatusHandler.Log("Excel file does Not contain worksheet name '" & strWorksheet & "'", eAlert.Error)
    '                Return Nothing
    '            End If
    '        Else
    '            ws = pck.Workbook.Worksheets.First
    '        End If

    '        Dim nCols As Integer = ws.Dimension.End.Column

    '        For iCol As Integer = 1 To nCols
    '            Dim cell As ExcelRange = ws.Cells(1, iCol, 1, iCol)
    '            Dim col As String = cell.Text
    '            dt.Columns.Add(col)
    '        Next

    '        For iRow As Integer = 2 To ws.Dimension.End.Row
    '            Dim drow As DataRow = dt.NewRow()
    '            For iCol As Integer = 1 To nCols
    '                Dim cell As ExcelRange = ws.Cells(iRow, iCol, iRow, iCol)
    '                drow(iCol - 1) = cell.Value
    '            Next
    '            dt.Rows.Add(drow)
    '        Next

    '    End Using

    '    MapColumnNames(dt)

    '    StatusHandler.Log("Excel file '" & strExcelFile & "', " & If(String.IsNullOrWhiteSpace(strWorksheet), "first worksheet", "worksheet '" & strWorksheet & "'") & " loaded", eAlert.OK)

    '    Return dt
    'End Function

#Region " Internals "

    Private Function LoadText(strText As String) As DataTable

        Try
            Dim sr As New StringReader(strText)
            Dim strLine As String = sr.ReadLine()
            Dim strArray() As String = cStringUtils.SplitQualified(strLine, ",")
            Dim dt As New DataTable()
            Dim row As DataRow = Nothing

            For Each s As String In strArray
                dt.Columns.Add(New DataColumn(Me.ToSimpleColumnName(s), GetType(String)))
            Next

            strLine = sr.ReadLine
            While Not String.IsNullOrEmpty(strLine)
                row = dt.NewRow()
                row.ItemArray = cStringUtils.SplitQualified(strLine, ",")
                dt.Rows.Add(row)
                strLine = sr.ReadLine
            End While

            sr.Close()
            sr.Dispose()
            Return dt

        Catch ex As Exception
            SendStatusMessage(ex.Message, eMessageImportance.Critical)
        End Try
        Return Nothing

    End Function

    Private Function ToSimpleColumnName(strColName As String) As String

        Dim strTest As String = strColName.ToLower()
        Dim n As Integer = 0

        If (Not Integer.TryParse(strColName, n)) Then
            For i As Integer = 1 To cCore.N_MONTHS
                If strTest.StartsWith(cDateUtils.GetMonthName(i, False).ToLower()) Then
                    n = i
                End If
            Next
        End If
        If (n > 0) Then Return "m" & n

        If strTest.StartsWith("fleet") Then
            strTest = strTest.Substring(5).Trim()
            If (Not Integer.TryParse(strTest, n)) Then
                For i As Integer = 1 To Me.m_core.nFleets
                    Dim fleet As cEcopathFleetInput = Me.m_core.EcopathFleetInputs(i)
                    If (String.Compare(strTest, fleet.Name, True) = 0) Then
                        n = i
                    End If
                Next
            End If
        End If
        If (n > 0) Then Return "f" & n

        Return strColName

    End Function

    Private Function ToMPA(strName As String) As Integer

        For i As Integer = 1 To Me.m_core.nMPAs
            Dim mpa As cEcospaceMPA = Me.m_core.EcospaceMPAs(i)
            If (String.Compare(mpa.Name, strName, True) = 0) Then
                Return i
            End If
        Next
        Return Nothing

    End Function

    Private Function ReadSafe(drow As DataRow, strField As String, strDefault As String) As String

        If Not drow.Table.Columns.Contains(strField) Then Return strDefault

        Dim val As Object = drow(strField)
        If Convert.IsDBNull(val) Then Return strDefault

        Return CStr(val)

    End Function

    Private Const s_TRUE As String = "1yv+t"
    Private Const s_FALSE As String = "0nx-f"
    Private Const s_DEFAULT As String = "?="

    Private Function ToCheckState(strVal As String) As CheckState

        If (String.IsNullOrWhiteSpace(strVal)) Then Return CheckState.Indeterminate
        strVal = strVal.Trim().ToLower()(0)
        If (s_DEFAULT.Contains(strVal)) Then Return CheckState.Indeterminate
        If (s_TRUE.Contains(strVal)) Then Return CheckState.Checked
        If (s_FALSE.Contains(strVal)) Then Return CheckState.Unchecked
        ' Unknown?!
        Return CheckState.Indeterminate
    End Function

    Private Sub SendStatusMessage(strMessage As String, importance As eMessageImportance)
        Dim msg As New cMessage(strMessage, eMessageType.DataImport, eCoreComponentType.External, importance)
        Me.m_core.Messages.SendMessage(msg)
    End Sub

#End Region ' Internals

End Class
