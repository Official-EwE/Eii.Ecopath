' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class dlgImportLayerStyles

    Private m_uic As cUIContext = Nothing
    Private m_io As cImportExportStyle = Nothing
    Private m_checker As cCheckboxHierarchy = Nothing

    Public Sub New(uic As cUIContext, io As cImportExportStyle)
        Me.m_uic = uic
        Me.m_io = io
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim vnf As New EwECore.Style.cVarnameTypeFormatter()

        Dim styleHeader As New DataGridViewCellStyle()
        styleHeader.BackColor = SystemColors.ButtonFace

        Me.m_checker = New cCheckboxHierarchy(Nothing)
        Me.m_checker.ManageCheckedStates = False

        Dim dgVar As DataGridViewRow = Nothing
        Dim varname As eVarNameFlags = eVarNameFlags.NotSet

        For Each entry As cImportExportStyle.cStyleEntry In Me.m_io.Entries

            entry.HasLayer = (Me.m_io.FindMatchingLayer(entry) IsNot Nothing)
            entry.CanCreate = (Me.m_io.CanCreate(entry))

            If (entry.HasLayer Or entry.CanCreate) Then

                If (entry.VarName <> varname) Then

                    ' Create new paretnt row
                    dgVar = New DataGridViewRow()
                    dgVar.DefaultCellStyle = styleHeader
                    dgVar.CreateCells(Me.m_dgLayers)
                    dgVar.Cells(Me.m_colUsed.Index).Value = CheckState.Unchecked
                    dgVar.Cells(Me.m_colIndex.Index).Value = ""
                    dgVar.Cells(Me.m_colName.Index).Value = vnf.ToString(entry.VarName, eDescriptorTypes.Name)
                    dgVar.Cells(Me.m_colStatus.Index).Value = ""

                    Me.m_dgLayers.Rows.Add(dgVar)
                    Me.m_checker.Add(dgVar.Cells(Me.m_colUsed.Index), Nothing)

                    varname = entry.VarName
                End If

                Dim strName As String = ""
                If entry.Index < 1 Then
                    strName = entry.Name
                Else
                    strName = cStringUtils.Localize(SharedResources.GENERIC_LABEL_INDEXED, entry.Index, entry.Name)
                End If

                Dim dgLayer As New DataGridViewRow()
                dgLayer.CreateCells(Me.m_dgLayers)
                dgLayer.Cells(Me.m_colUsed.Index).Value = If(entry.Enabled, CheckState.Checked, CheckState.Unchecked)
                dgLayer.Cells(Me.m_colIndex.Index).Value = If(entry.Index < 1, "", CStr(entry.Index))
                dgLayer.Cells(Me.m_colName.Index).Value = strName
                dgLayer.Cells(Me.m_colStatus.Index).Value = ""
                dgLayer.Tag = entry
                Me.m_dgLayers.Rows.Add(dgLayer)
                Me.m_checker.Add(dgLayer.Cells(Me.m_colUsed.Index), dgVar.Cells(Me.m_colUsed.Index))

            End If
        Next

        Me.UpdateStatus()
        Me.m_checker.ManageCheckedStates = True

        Dim cmd As cBrowserCommand = CType(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.AddControl(Me.m_pbCredits, "www.criobe.pf/recherche/safenet/")

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        Dim cmd As cBrowserCommand = CType(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.RemoveControl(Me.m_pbCredits)

        MyBase.OnFormClosed(e)
    End Sub

    Private Sub UpdateStatus()

        Dim styleDefault As New DataGridViewCellStyle()

        Dim styleMissing As New DataGridViewCellStyle()
        styleMissing.ForeColor = Color.Red

        Dim styleCreate As New DataGridViewCellStyle()
        styleCreate.ForeColor = Color.DarkGreen

        ' ToDo: update 'include' status
        ' Excluded layers will be "(ignored)"
        For Each dgr As DataGridViewRow In Me.m_dgLayers.Rows
            If (dgr.Tag IsNot Nothing) Then

                Dim entry As cImportExportStyle.cStyleEntry = DirectCast(dgr.Tag, cImportExportStyle.cStyleEntry)
                Dim label As String = My.Resources.GENERIC_VALUE_PRESENT
                Dim style As DataGridViewCellStyle = styleDefault

                If (Not entry.HasLayer) Then
                    If (Me.m_cbCreateMissingLayers.Checked) Then
                        label = SharedResources.GENERIC_VALUE_CREATE_PENDING
                        style = styleCreate
                    Else
                        label = SharedResources.GENERIC_VALUE_NOTAVAILABLE
                        style = styleMissing
                    End If
                End If
                dgr.Cells(3).Value = label
                dgr.Cells(3).Style = style
            End If
        Next

    End Sub

    Private Sub OnOK(sender As Object, e As EventArgs) Handles m_btnOK.Click

        Me.UpdateEntries()

        If Me.m_io.MergeToLayers(Me.m_cbCreateMissingLayers.Checked) Then
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub UpdateEntries()
        For Each dgr As DataGridViewRow In Me.m_dgLayers.Rows
            If (dgr.Tag IsNot Nothing) Then
                Dim entry As cImportExportStyle.cStyleEntry = DirectCast(dgr.Tag, cImportExportStyle.cStyleEntry)
                Dim state As CheckState = DirectCast(dgr.Cells(Me.m_colUsed.Index).Value, CheckState)
                entry.Enabled = (state = CheckState.Checked)
            End If
        Next
    End Sub

    Private Sub OnToggleCreateMissing(sender As Object, e As EventArgs) Handles m_cbCreateMissingLayers.CheckedChanged
        Me.UpdateStatus()
    End Sub

    Private m_bInUpdate As Boolean = False

    Private Sub OnCancel(sender As Object, e As EventArgs) Handles m_btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class