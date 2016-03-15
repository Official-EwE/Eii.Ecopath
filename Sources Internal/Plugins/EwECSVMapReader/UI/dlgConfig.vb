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
#Region " Imports "

option Strict On
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class dlgConfig
    Implements IUIElement
    Implements IOptionsPage

    Private m_ds As cEwECSVMapDataset = Nothing

    Public Sub New(ds As cEwECSVMapDataset)
        MyBase.New()
        Me.m_ds = ds
        Me.InitializeComponent()
    End Sub

    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tbxName.Text = Me.m_ds.DisplayName
        Me.m_tbxDescription.Text = Me.m_ds.DataDescription
        Me.m_tbxFolder.Text = Me.m_ds.Source
        Me.UpdateFileList(Me.m_ds.Files.ToArray())

    End Sub

    Public Function Apply() As IOptionsPage.eApplyResultType _
        Implements IOptionsPage.Apply

        Me.m_ds.DisplayName = Me.m_tbxName.Text
        Me.m_ds.DataDescription = Me.m_tbxDescription.Text
        Me.m_ds.Source = Me.m_tbxFolder.Text
        Return IOptionsPage.eApplyResultType.Success

    End Function

    Public Function CanApply() As Boolean _
        Implements IOptionsPage.CanApply

        Return (Not String.IsNullOrWhiteSpace(Me.m_tbxName.Text)) And
               (Me.m_lbxFiles.Items.Count > 0)

    End Function

    Public Function CanSetDefaults() As Boolean _
        Implements IOptionsPage.CanSetDefaults

        Return False

    End Function

    Public Event OnChanged(sender As IOptionsPage, args As System.EventArgs) _
        Implements IOptionsPage.OnChanged

    Public Sub SetDefaults() _
        Implements IOptionsPage.SetDefaults
        ' NOP
    End Sub

    Private Sub OnChooseFolder(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseFolder.Click

        ' ToDo: globalize this

        Dim dlg As FolderBrowserDialog = cEwEFileDialogHelper.FolderBrowserDialog("Select folder with CSV maps", Me.m_ds.Source)
        dlg.ShowNewFolderButton = False

        If (dlg.ShowDialog(Me.UIContext.FormMain) = DialogResult.OK) Then
            Me.UpdateFileList(Me.m_ds.Read(dlg.SelectedPath))
            Me.m_tbxFolder.Text = dlg.SelectedPath()
        End If

    End Sub

    Private Sub OnTextsChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxName.TextChanged, m_tbxDescription.TextChanged

        Me.ContentChanged()

    End Sub

    Private Sub ContentChanged()
        Try
            RaiseEvent OnChanged(Me, New EventArgs())
        Catch ex As Exception

        End Try
    End Sub

    Private Sub UpdateFileList(files As String())
        Me.m_lbxFiles.BeginUpdate()
        Me.m_lbxFiles.Items.Clear()
        For i As Integer = 0 To files.Count - 1
            Me.m_lbxFiles.Items.Add(System.IO.Path.GetFileName(files(i)))
        Next
        Me.m_lbxFiles.EndUpdate()
    End Sub

End Class