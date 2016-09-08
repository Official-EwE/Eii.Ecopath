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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'


Imports System.IO
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls

Public Class frmSailCost

    Private m_plugin As cSailCostPlugin
    Private m_bInInit As Boolean

    Public Sub New()
        MyBase.New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_bInInit = True
        Try
            Me.UpdateControls()
            AddHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
        Catch ex As Exception
            System.Console.WriteLine("WARNING: GOM LME Configuration interface failed to loaded correctly. Exception " + ex.Message)
        End Try
        Me.m_bInInit = False

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Try
            RemoveHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
        Catch ex As Exception

        End Try
        MyBase.OnFormClosed(e)
    End Sub

    Friend Sub Init(SailCostPlugin As cSailCostPlugin)
        Me.m_plugin = SailCostPlugin
    End Sub

    Private Sub OnChanged()
        Me.UpdateControls()
    End Sub

    Private Sub OnCheckedChanged_chkUseSailCost(sender As System.Object, e As System.EventArgs) Handles m_chkUseSailCost.CheckedChanged
        Try
            If Not Me.m_bInInit Then
                Me.m_plugin.UseSailCostPlugin = Me.m_chkUseSailCost.Checked
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        ' ToDo: globalize this

        Dim bConfigOK As Boolean = False
        Dim bActive As Boolean = False

        Dim strEffort As String = Path.GetFileName(Me.m_plugin.EffortFile)
        Dim strCells As String = Path.GetFileName(Me.m_plugin.LMECellsFile)

        If (Not strEffort.EndsWith(".csv")) Then strEffort = ""

        Try
            bConfigOK = Me.m_plugin.IsInputdataValid()
            bActive = Me.m_plugin.UseSailCostPlugin And bConfigOK

            Me.m_tbxPath.Text = Me.m_plugin.DataPath
            Me.m_chkUseSailCost.Checked = bActive
            Me.m_chkUseSailCost.Enabled = bConfigOK

            Me.m_clbValidation.SuspendLayout()
            Me.m_clbValidation.Items.Clear()
            Me.m_clbValidation.Items.Add("Effort file (" & strEffort & ")", File.Exists(Me.m_plugin.EffortFile))
            Me.m_clbValidation.Items.Add("LME cells file (" & strCells & ")", File.Exists(Me.m_plugin.LMECellsFile))
            Me.m_clbValidation.ResumeLayout()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub m_btnChoosePath_Click(sender As Object, e As EventArgs) Handles m_btnChoosePath.Click


        Dim dlg As FolderBrowserDialog = cEwEFileDialogHelper.FolderBrowserDialog("Select GOM LME Effort data path",
                                                                                  Me.m_plugin.DataPath)
        If (dlg.ShowDialog() = DialogResult.OK) Then
            Me.m_plugin.DataPath = dlg.SelectedPath
        End If

        Me.UpdateControls()

    End Sub

End Class