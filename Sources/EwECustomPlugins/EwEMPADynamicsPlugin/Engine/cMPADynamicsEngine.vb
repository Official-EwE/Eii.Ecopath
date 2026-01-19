' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.Drawing
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Web
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip
Imports EwECore
Imports EwECore.Auxiliary
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls



' ToDo: add leniency to date parsing (yy vs yyyy, M vs MM)

Public Class cMPADynamicsEngine

#Region " Private vars "

    ''' <summary>Cell values considered as True (e.g., for turning options on)</summary>
    Private Const s_TRUE As String = "1yv+t"
    ''' <summary>Cell values considered as False (e.g., for turning options off)</summary>
    Private Const s_FALSE As String = "0nx-f"
    ''' <summary>Cell values considered as neutral (e.g., for leaving options as they are)</summary>
    Private Const s_DEFAULT As String = "?="

    ''' <summary>Supported date formats that can be parsed.</summary>
    Private Shared sFORMATS As String() = New String() {"yyyy/MM", "yyyy-MM", "MM/yyyy", "MM-yyyy", "yyyy/M", "yyyy-M", "M/yyyy", "M-yyyy"}
    ''' <summary>Supported locales. Sorry.</summary>
    Private Shared sLOCALE As New CultureInfo("en-US")

    Private m_core As cCore = Nothing
    Private m_ds As cEcospaceDataStructures = Nothing
    Private m_dtStates As New Dictionary(Of Date, List(Of cMPAState))
    Private m_lPreserved As New List(Of cMPAState)
    Private m_bAutosaving As Boolean = False
    Private m_bAutorunning As Boolean = False

    Private m_sw As StreamWriter = Nothing

#End Region ' Private vars

    Public Shared timestampZero As New Date(1, 1, 1)

#Region " Construction "

    Public Sub New(core As cCore, ds As cEcospaceDataStructures)
        Me.m_core = core
        Me.m_ds = ds
    End Sub

#End Region ' Construction

#Region " Public access "

    Public Sub Clear()
        Me.Restore()
        Me.m_dtStates.Clear()
    End Sub

    Public Event OnSettingsChanged(sender As Object, args As EventArgs)

    Public Property Autorun As Boolean

    Public Sub Backup(bAutosave As Boolean)

        Me.m_bAutorunning = Me.Autorun And (Me.m_dtStates.Count > 0)
        If (Not Me.m_bAutorunning) Then Return

        Me.m_lPreserved.Clear()
        Dim timestamp As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(1)
        For iMPA As Integer = 1 To Me.m_ds.MPAno
            Dim state As New cMPAState(Me.m_ds, iMPA, timestamp)
            state.Load()
            Me.m_lPreserved.Add(state)
        Next
        Me.m_bAutosaving = bAutosave
        Me.StartAutosaving()
    End Sub

    Public Sub Restore()

        If (Not Me.m_bAutorunning) Then Return

        For Each state As cMPAState In Me.m_lPreserved
            state.Apply()
        Next
        Me.m_lPreserved.Clear()
        Me.StopAutosaving()
    End Sub

    Public Sub OnEcospaceTimeStep(iTime As Integer)

        If (Not Me.m_bAutorunning) Then Return

        Dim timestamp As Date = Me.m_core.EcospaceTimestepToAbsoluteTime(iTime)

        If (Me.m_ds.bInSpinUp) Then Return

        If (Me.m_dtStates.ContainsKey(timestamp)) Then
            For Each state As cMPAState In Me.m_dtStates(timestamp)
                state.Apply()
                Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_MPA_CHANGED,
                                                        timestamp.ToShortDateString(),
                                                        state.ToString(),
                                                        state.ClosureState(),
                                                        state.RegulationState()),
                                  eMessageImportance.Information)
            Next
            ' Autosave when state has changed
            Me.Autosave(iTime)
        End If

    End Sub

    ''' <summary>
    ''' Read a CSV file
    ''' </summary>
    ''' <param name="strCSV">CSV file name to read</param>
    ''' <returns></returns>
    Public Function LoadCSV(strCSV As String) As Boolean

        Dim lDetails As New List(Of String)
        Dim strText As String = ""
        Dim bSucces As Boolean = False

        Me.m_dtStates.Clear()
        strCSV = Path.GetFullPath(strCSV)
        Try
            Using sr As New StreamReader(strCSV)
                strText = sr.ReadToEnd()
            End Using
            bSucces = Me.LoadText(strText, lDetails)

            If (bSucces) Then
                Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_SUCCESS, strCSV), eMessageImportance.Information)
                Me.SavePersistent(strText)
            Else
                Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_FAILED, strCSV, ""), eMessageImportance.Critical, lDetails)
                Me.m_dtStates.Clear()
            End If

        Catch ex As Exception
            Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_FAILED, strCSV, ex.Message), eMessageImportance.Critical)
            ' NOP
        End Try

        Me.Autorun = bSucces

        Return bSucces

    End Function

    Private Function LoadText(strText As String, lDetails As List(Of String)) As Boolean

        Dim bSucces As Boolean = True

        Try
            Dim dt As DataTable = Me.TextToDatatable(strText)
            If (dt Is Nothing) Then Return False
            Dim bTimeStepMode As Boolean = dt.Columns.Contains("timestep")

            For Each drow As DataRow In dt.Rows

                Dim iMPA As Integer = Me.ToMPA(CStr(drow("MPA")))
                Dim timestamp As Date

                If (iMPA > 0 And iMPA <= Me.m_core.nMPAs) Then
                    If (bTimeStepMode) Then
                        timestamp = Me.m_core.EcospaceTimestepToAbsoluteTime(CInt(drow("timestep")))
                    Else
                        bSucces = bSucces And Date.TryParseExact(CStr(drow("date")), sFORMATS, sLOCALE, DateTimeStyles.None, timestamp)
                    End If

                    If (timestamp >= Me.m_core.EcospaceTimestepToAbsoluteTime(1) And timestamp > timestampZero) Then

                        If (iMPA >= 1) And bSucces Then
                            Dim state As New cMPAState(Me.m_ds, iMPA, timestamp)
                            For i As Integer = 1 To cCore.N_MONTHS
                                state.IsClosed(i) = Me.IsEnforced(Me.ReadSafe(drow, "m" & i, ""))
                            Next

                            For i As Integer = 1 To Me.m_core.nFleets
                                state.IsEnforced(i) = Me.IsEnforced(Me.ReadSafe(drow, "f" & i, ""))
                            Next

                            If (Not Me.m_dtStates.ContainsKey(timestamp)) Then
                                Me.m_dtStates(timestamp) = New List(Of cMPAState)
                            End If
                            Me.m_dtStates(timestamp).Add(state)
                        ElseIf (lDetails IsNot Nothing) Then
                            Dim strError As String = cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_ERROR_MPA_UNKNOWN, CStr(drow("MPA")))
                            If (lDetails.IndexOf(strError) = -1) Then
                                lDetails.Add(strError)
                            End If
                        End If
                    End If
                ElseIf (lDetails IsNot Nothing) Then
                    ' iMPA out of bounds
                    Dim strError As String = cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_ERROR_MPA_UNKNOWN, CStr(drow("MPA")))
                    If (lDetails.IndexOf(strError) = -1) Then
                        lDetails.Add(strError)
                    End If
                End If
            Next

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Public Function SaveCSV(strFile As String) As Boolean

        Dim bSucces As Boolean = True
        Dim bTimestepMode As Boolean = (Me.m_core.EcosimFirstYear < 100)

        Try
            Using sw As New StreamWriter(strFile)
                sw.Write(If(bTimestepMode, "Timestep", "Date"))
                sw.Write(",mpa")
                For i As Integer = 1 To cCore.N_MONTHS : sw.Write(",m{0}", i) : Next
                For i As Integer = 1 To Me.m_core.nFleets : sw.Write(",fleet{0}", i) : Next
                sw.WriteLine()

                For Each state As cMPAState In Me.MPAStates(True)
                    sw.Write(If(bTimestepMode, "1", String.Format("{0}-01", Me.m_core.EcosimFirstYear)))
                    sw.Write("," & state.MPA)
                    For i As Integer = 1 To cCore.N_MONTHS : sw.Write(",{0}", Me.ToString(state.IsClosed(i))) : Next
                    For i As Integer = 1 To Me.m_core.nFleets : sw.Write(",{0}", Me.ToString(state.IsEnforced(i))) : Next
                    sw.WriteLine()
                Next

                sw.Flush()
                sw.Close()
            End Using
            Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_CONFIG_SAVE_SUCCESS, strFile), eMessageImportance.Information, hyperlink:=Path.GetDirectoryName(strFile))

        Catch ex As Exception
            Me.SendStatusMessage(cStringUtils.Localize(My.Resources.STATUS_CONFIG_LOAD_FAILED, strFile, ex.Message), eMessageImportance.Critical)
            bSucces = False
        End Try
        Return bSucces

    End Function

    Public ReadOnly Property MPAStates(bIncludeStartup As Boolean) As ICollection(Of cMPAState)
        Get
            Dim lStates As New List(Of cMPAState)
            ' No double work please
            bIncludeStartup = bIncludeStartup And Not Me.m_dtStates.ContainsKey(timestampZero)

            If (bIncludeStartup) Then
                For iMPA As Integer = 1 To Me.m_ds.MPAno
                    Dim state As New cMPAState(Me.m_ds, iMPA, timestampZero)
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

#End Region ' Public access

#Region " Persistence "

    Private Const KEY_GENERAL As String = "General"
    Private Const SETTING_CSV As String = "CSV"

    Friend Sub SavePersistent(text As String)
        Dim ad As cAuxiliaryData = Me.m_core.AuxillaryData(Me.DataName)
        Dim textC As String = cStringUtils.Compress(text, Compression.CompressionLevel.Optimal)
        ad.Settings.WriteSetting(KEY_GENERAL, SETTING_CSV, textC)
    End Sub

    Friend Sub LoadPersistent()

        Dim ad As cAuxiliaryData = Me.m_core.AuxillaryData(Me.DataName)
        Dim textC As String = ad.Settings.ReadSetting(KEY_GENERAL, SETTING_CSV, "")
        If Not String.IsNullOrWhiteSpace(textC) Then
            Dim textUC As String = cStringUtils.Decompress(textC)
            Me.LoadText(textUC, Nothing)
        End If

    End Sub

    Private Function DataName() As String
        Dim sc As cEcospaceScenario = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex)
        Return String.Format("MPADynamics_{0}", sc.DBID)
    End Function

    Private Function TimestepName(dt As DateTime) As String
        Return String.Format("time_{0}-{1}-{2}", dt.Year, dt.Month, dt.Day)
    End Function

#End Region ' Persistence

#Region " Internals "

    ''' <summary>
    ''' Convert comma-separated text into a datatable.
    ''' </summary>
    ''' <param name="strText"></param>
    ''' <returns></returns>
    Private Function TextToDatatable(strText As String) As DataTable

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
            Me.SendStatusMessage(ex.Message, eMessageImportance.Critical)
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

        Dim iTest As Integer = 0
        If Integer.TryParse(strName, iTest) Then
            If iTest > 0 Then
                Return iTest
            End If
        End If

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

    Private Function IsEnforced(strVal As String) As TriState

        If (String.IsNullOrWhiteSpace(strVal)) Then Return TriState.UseDefault
        strVal = strVal.Trim().ToLower()(0)
        If (s_DEFAULT.Contains(strVal)) Then Return TriState.UseDefault
        If (s_TRUE.Contains(strVal)) Then Return TriState.True
        If (s_FALSE.Contains(strVal)) Then Return TriState.False
        Return TriState.UseDefault

    End Function

    Private Overloads Function ToString(ts As TriState) As String
        If (ts = TriState.True) Then Return s_TRUE(0)
        If (ts = TriState.False) Then Return s_FALSE(0)
        Return ""
    End Function

    Private Sub SendStatusMessage(strMessage As String, importance As eMessageImportance, Optional lDetails As ICollection(Of String) = Nothing, Optional hyperlink As String = "")
        Dim msg As New cMessage(strMessage, eMessageType.DataImport, eCoreComponentType.External, importance)
        If (lDetails IsNot Nothing) Then
            For Each strDetail As String In lDetails
                Dim vs As New cVariableStatus(Nothing, eStatusFlags.ErrorEncountered, strDetail, eVarNameFlags.NotSet)
                msg.Variables.Add(vs)
            Next
        End If
        If (Not String.IsNullOrWhiteSpace(hyperlink)) Then msg.Hyperlink = hyperlink
        Me.m_core.Messages.SendMessage(msg)
    End Sub

    Private Function AutosaveFileName() As String
        Return Path.Combine(Me.m_core.DefaultOutputPath(eAutosaveTypes.Ecospace), "MPADynamicsStats.csv")
    End Function

    ''' <summary>
    ''' Create streamwriter, and write out the initial state
    ''' </summary>
    ''' <returns>True if successful.</returns>
    Private Function StartAutosaving() As Boolean

        If (Me.m_bAutosaving = False) Then Return False

        Dim fout As String = Me.AutosaveFileName()
        Dim pout As String = Path.GetDirectoryName(fout)
        If Not cFileUtils.IsDirectoryAvailable(pout, True) Then
            Me.SendStatusMessage(cStringUtils.Localize(My.Resources.NOTIFICATION_AUTOSAVE_FAILED, My.Resources.DISPLAYNAME, fout, ""), eMessageImportance.Critical)
            Return False
        End If

        Try
            Me.m_sw = New StreamWriter(fout)
            If (Me.m_core.SaveWithFileHeader) Then
                Me.m_sw.WriteLine(Me.m_core.DefaultFileHeader(eAutosaveTypes.Ecospace))
            End If

            Me.m_sw.Write("Timestep,Date,Area,Cells,AreaClosed,CellsClosed")
            For iMPA As Integer = 1 To Me.m_ds.MPAno
                Me.m_sw.Write(",MPA_{0}_AreaClosed,MPA_{0}_CellsClosed", iMPA)
            Next
            Me.m_sw.WriteLine()

        Catch ex As Exception
            Me.m_sw = Nothing
            Me.m_bAutosaving = False

            Me.SendStatusMessage(cStringUtils.Localize(My.Resources.NOTIFICATION_AUTOSAVE_FAILED, My.Resources.DISPLAYNAME, fout, ex.Message), eMessageImportance.Critical)

            Return False
        End Try
        Return Me.Autosave(1)

    End Function

    Private Function Autosave(iTime As Integer) As Boolean

        If (Me.m_bAutosaving = False) Then Return False

        Dim nClosed(Me.m_ds.MPAno) As Integer
        Dim szClosed(Me.m_ds.MPAno) As Double
        Dim nArea As Integer = 0
        Dim szArea As Double = 0

        ' This code goes the long way to count cells closed to any form of fishing
        For irow As Integer = 1 To Me.m_ds.InRow
            For icol As Integer = 1 To Me.m_ds.InCol
                If (Me.m_ds.Depth(irow, icol) > 0) Then
                    Dim bClosed As Boolean = False
                    Dim sz As Single = Me.m_ds.CellArea(irow, icol)
                    For iMPA As Integer = 1 To Me.m_ds.MPAno
                        If (Me.m_ds.IsMPAActive(iMPA)) Then
                            If (Me.m_ds.MPA(iMPA)(irow, icol) > 0) Then
                                nClosed(iMPA) += 1
                                szClosed(iMPA) += sz
                                bClosed = True
                            End If
                        End If
                    Next
                    If bClosed Then
                        nClosed(0) += 1
                        szClosed(0) += sz
                    End If

                    szArea += sz
                    nArea += 1
                End If
            Next
        Next

        'Me.m_sw.Write("Timestep,Date,Area,NumCells")
        Me.m_sw.Write("{0},{1},{2},{3}",
                      iTime,
                      cStringUtils.ToCSVField(cStringUtils.FormatDate(Me.m_core.EcospaceTimestepToAbsoluteTime(iTime))),
                      cStringUtils.ToCSVField(szArea),
                      cStringUtils.ToCSVField(nArea))
        For iMPA As Integer = 0 To Me.m_ds.MPAno
            'Me.m_sw.Write(",MPA_{0}_AreaClosed,MPA_{0}_CellsClosed", iMPA)
            Me.m_sw.Write(",{0},{1}",
                      cStringUtils.ToCSVField(szClosed(iMPA)),
                      cStringUtils.ToCSVField(nClosed(iMPA)))
        Next
        Me.m_sw.WriteLine()

        Return True

    End Function

    ''' <summary>
    ''' Close streamwriter
    ''' </summary>
    Private Sub StopAutosaving()
        If (Me.m_bAutosaving = False) Then Return

        Dim fout As String = Me.AutosaveFileName()
        Dim pout As String = Path.GetDirectoryName(fout)
        Me.SendStatusMessage(cStringUtils.Localize(My.Resources.NOTIFICATION_AUTOSAVE_SUCCESS, My.Resources.DISPLAYNAME, fout), eMessageImportance.Information, hyperlink:=pout)

        Me.m_sw.Flush()
        Me.m_sw.Close()
        Me.m_sw.Dispose()
        Me.m_sw = Nothing
        Me.m_bAutosaving = False

    End Sub

#End Region ' Internals

End Class
