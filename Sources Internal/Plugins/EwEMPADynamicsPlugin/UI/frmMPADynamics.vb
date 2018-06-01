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

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmMPADynamics

#Region " Private vars "

    Private m_engine As cMPADynamicsEngine = Nothing
    Private m_bGridInvalid As Boolean = True

#End Region ' Private vars

    Public Sub New(uic As cUIContext, engine As cMPADynamicsEngine)

        Me.UIContext = uic
        Me.m_engine = engine

        Me.InitializeComponent()

        Me.Text = My.Resources.PLUGIN_TITLE
        Me.TabText = Me.Text
        Me.m_tsbnLoadCSV.Image = SharedResources.openHS

    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim fmt As New cCoreInterfaceFormatter()
        Dim col As DataGridViewColumn = Nothing

        ' Create grid cols
        For i As Integer = 1 To cCore.N_MONTHS
            col = New DataGridViewImageColumn()
            col.Name = "m_colM" & i
            col.HeaderText = cDateUtils.GetMonthName(i, False)
            col.ReadOnly = True
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            Me.m_dgvStates.Columns.Add(col)
        Next

        For i As Integer = 1 To Me.Core.nFleets
            Dim fleet As cEcopathFleetInput = Me.Core.EcopathFleetInputs(i)
            col = New DataGridViewImageColumn()
            col.Name = "m_colF" & i
            col.HeaderText = fmt.GetDescriptor(fleet)
            col.ReadOnly = True
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader
            Me.m_dgvStates.Columns.Add(col)
        Next

        Me.m_tsbnShowMonths.Image = SharedResources.CalendarHS
        Me.m_tsbnShowFleets.Image = SharedResources.fishing_gear
        Me.m_tsbnExport.Image = SharedResources.ExportHS

        For i As Integer = 0 To Me.Core.nFleets
            If (i = 0) Then
                Me.m_tscmbFleets.Items.Add(SharedResources.GENERIC_VALUE_ALLFLEETS)
            Else
                Me.m_tscmbFleets.Items.Add(fmt.GetDescriptor(Me.Core.EcopathFleetInputs(i)))
            End If
        Next

        Me.m_tsbnShowMonths.Checked = My.Settings.ShowMonths
        Me.m_tsbnShowFleets.Checked = My.Settings.ShowFleets
        Me.m_tscmbFleets.SelectedIndex = 0
        Me.UpdateGrid()
        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}

    End Sub

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)

        If (msg.Source = eCoreComponentType.EcoSpace And msg.Type = eMessageType.DataValidation) Then
            If (msg.DataType = eDataTypes.EcospaceMPA Or msg.DataType = eDataTypes.EcospaceFleet) Then
                Me.InvalidateGrid()
            End If
        End If

    End Sub

#End Region ' Overrides

#Region " Event handlers "

    Private Sub OnDropFile(sender As Object, e As DragEventArgs) Handles m_dgvStates.DragDrop
        Try
            Dim files As String() = CType(e.Data.GetData(DataFormats.FileDrop), String())
            Me.m_engine.Clear()
            For Each file As String In files
                Me.m_engine.LoadCSV(file, False)
            Next
            Me.UpdateGrid()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnLoadCSV(sender As Object, e As EventArgs) Handles m_tsbnLoadCSV.Click
        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_SELECT_FILE_LOAD, "", SharedResources.FILEFILTER_CSV)
        If (ofd.ShowDialog() = DialogResult.OK) Then
            Me.m_engine.LoadCSV(ofd.FileName)
            Me.UpdateGrid()
        End If
    End Sub

    Private Sub OnExportFile(sender As Object, e As EventArgs) Handles m_tsbnExport.Click
        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog(My.Resources.PROMPT_SELECT_FILE_SAVE, "", SharedResources.FILEFILTER_CSV)
        If (sfd.ShowDialog() = DialogResult.OK) Then
            Me.m_engine.SaveCSV(sfd.FileName)
        End If
    End Sub

    Private Sub OnShowMonthsChanged(sender As Object, e As EventArgs) Handles m_tsbnShowMonths.Click
        Me.UpdateGrid()
    End Sub

    Private Sub OnShowFleetsChanged(sender As Object, e As EventArgs) Handles m_tsbnShowFleets.Click
        Me.UpdateGrid()
    End Sub

    Private Sub OnShowFleetIndexChanged(sender As Object, e As EventArgs) Handles m_tscmbFleets.SelectedIndexChanged
        Me.UpdateGrid()
    End Sub

#End Region ' Event handlers

#Region " Internals "

    Private Sub InvalidateGrid()
        ' Bundle multiple messages into one update
        If (m_bGridInvalid = False) Then
            Me.m_bGridInvalid = True
            Me.BeginInvoke(New MethodInvoker(AddressOf UpdateGrid))
        End If
    End Sub

    Private Sub UpdateGrid()

        Me.m_bGridInvalid = False
        If (Me.IsDisposed) Then Return

        Dim states As ICollection(Of cMPAState) = Me.m_engine.MPAStates(True)
        Dim fmt As New cCoreInterfaceFormatter()

        Me.m_dgvStates.Enabled = False
        Me.m_dgvStates.Rows.Clear()

        ' Show/hide columns
        Dim bShowMonths As Boolean = Me.m_tsbnShowMonths.Checked
        Dim iShowFleets As Integer = If(Me.m_tsbnShowFleets.Checked, Me.m_tscmbFleets.SelectedIndex, -1)

        For j As Integer = 1 To cCore.N_MONTHS
            Me.m_dgvStates.Columns("m_colM" & j).Visible = bShowMonths
        Next
        For j As Integer = 1 To Me.Core.nFleets
            Me.m_dgvStates.Columns("m_colF" & j).Visible = (iShowFleets = j) Or (iShowFleets = 0)
        Next

        If (states.Count > 0) Then

            Me.m_dgvStates.Rows.Add(states.Count)
            For i As Integer = 0 To states.Count - 1

                Dim state As cMPAState = states(i)
                Dim row As DataGridViewRow = Me.m_dgvStates.Rows(i)
                Dim mpa As cEcospaceMPA = Me.Core.EcospaceMPAs(state.MPA)

                Dim timestamp As Date = state.TimeStamp
                If (timestamp = New Date(1, 1, 1)) Then
                    row.Cells("m_colTime").Value = My.Resources.GENERIC_VALUE_INITIAL
                    row.DefaultCellStyle.BackColor = Drawing.Color.FromArgb(255, 230, 230, 230)
                Else
                    row.Cells("m_colTime").Value = state.TimeStamp.ToShortDateString()
                End If
                row.Cells("m_colMPA").Value = fmt.GetDescriptor(mpa)
                For j As Integer = 1 To cCore.N_MONTHS
                    row.Cells("m_colM" & j).Value = ToCellValue(state.IsClosed(j))
                Next
                For j As Integer = 1 To Me.Core.nFleets
                    row.Cells("m_colF" & j).Value = ToCellValue(state.IsEnforced(j))
                Next
            Next
        End If

        Me.m_dgvStates.Enabled = True

    End Sub

    Private Function ToCellValue(state As TriState) As Object
        Select Case state
            Case TriState.True
                Return My.Resources.enforced
            Case TriState.False
                Return SharedResources.fishing_gear
            Case TriState.UseDefault
                Return My.Resources.none
        End Select
        Return Nothing
    End Function

#End Region ' Internals

End Class