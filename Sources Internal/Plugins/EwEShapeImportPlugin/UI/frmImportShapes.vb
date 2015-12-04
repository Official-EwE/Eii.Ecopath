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
Imports System.IO
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core
Imports System.Windows.Forms

#End Region ' Imports

Public Class frmImportShapes

    Private m_uic As cUIContext = Nothing
    Private m_data As cData = Nothing

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
        Me.m_data = New cData(Me.m_uic.Core)
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_grid.UIContext = Me.m_uic

        Me.m_cmbTarget.Items.Add(eDataTypes.CapacityMediation)
        Me.m_cmbTarget.Items.Add(eDataTypes.PriceMediation)
        Me.m_cmbTarget.Items.Add(eDataTypes.Mediation)
        Me.m_cmbTarget.Items.Add(eDataTypes.Forcing)
        Me.m_cmbTarget.Items.Add(eDataTypes.EggProd)
        Me.m_cmbTarget.SelectedIndex = 0

        Me.CenterToScreen()
        Me.UpdateControls()

    End Sub

#Region " Events "

    Private Sub OnImportBrowseFile(sender As System.Object, e As System.EventArgs) _
        Handles m_btnImportBrowse.Click

        Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
        Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

        cmdFO.Invoke(Me.m_tbImportFileName.Text, SharedResources.FILEFILTER_CSV & "|" & SharedResources.FILEFILTER_TEXT)

        If (cmdFO.Result = DialogResult.OK) Then
            Me.m_tbImportFileName.Text = cmdFO.FileName
            Me.Read()
        End If

    End Sub

    Private Sub OnFileNameTextChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbImportFileName.TextChanged
        ' NOP
    End Sub

    Private Sub OnDelimiterChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbImportDelimiter.TextChanged
        Me.m_data.Delimiter = Me.m_tbImportDelimiter.Character
        Me.Read()
    End Sub

    Private Sub OnSeparatorChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbImportSeparator.TextChanged
        Me.m_data.DecimalSeparator = Me.m_tbImportSeparator.Character
        Me.Read()
    End Sub

    Private Sub OnFormatTarget(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbTarget.Format

        Dim fmt As New cDataTypeFormatter()
        e.Value = fmt.GetDescriptor(e.ListItem)

    End Sub

    Private Sub OnTargetSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbTarget.SelectedIndexChanged
        Me.m_data.DataType = DirectCast(Me.m_cmbTarget.SelectedItem, eDataTypes)
        Me.Read()
    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOk.Click

        Dim importer As New cImporter(Me.m_uic.Core, Me.m_data)
        Dim bSuccess As Boolean = True

        cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, "", -1)
        Try
            bSuccess = importer.Import()
        Catch ex As Exception

        End Try
        cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        If (bSuccess = True) Then
            Me.DialogResult = DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = DialogResult.Cancel
        Me.Close()

    End Sub

#End Region ' Events


#Region " Drag and drop "

    Protected Overrides Sub OnDragOver(e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim astrFiles() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
            If astrFiles.Length = 1 Then
                e.Effect = DragDropEffects.All
            End If
        End If
        MyBase.OnDragOver(e)
    End Sub

    Protected Overrides Sub OnDragDrop(e As DragEventArgs)
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Try
                Dim astrFiles() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                If astrFiles.Length = 1 Then
                    Me.m_tbImportFileName.Text = astrFiles(0)
                    Me.Read()
                End If
            Catch ex As Exception

            End Try
        End If
        MyBase.OnDragDrop(e)
    End Sub

#End Region ' Drag and drop

#Region " Internals "

    Private Sub UpdateControls()

        Dim bCanImport As Boolean = (Me.m_grid.RowsCount > 1)
        Me.m_btnOk.Enabled = bCanImport

    End Sub

    Private Sub Read()

        If Not File.Exists(Me.m_tbImportFileName.Text) Then Return

        Using reader As New StreamReader(Me.m_tbImportFileName.Text)
            Me.m_data.Read(reader)
        End Using
        Me.m_grid.Functions = Me.m_data.FunctionDefinitions()

        Me.UpdateControls()

    End Sub

#End Region ' Internals

End Class