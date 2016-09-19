Option Strict On
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

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmSailCost

#Region " Private vars "

    Private m_plugin As cSailCostPlugin = Nothing
    Private m_bInitialized As Boolean = False

#End Region ' Private vars

    Public Sub New(plugin As cSailCostPlugin, uic As cUIContext)
        MyBase.New()

        Me.m_plugin = plugin
        Me.UIContext = uic

        Me.InitializeComponent()

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Try
            AddHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
        Catch ex As Exception
            System.Console.WriteLine("WARNING: GOM LME Configuration interface failed to loaded correctly. Exception " + ex.Message)
        End Try

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries}
        Me.m_bInitialized = True

        Me.UpdateControls()
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Try
            RemoveHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
        Catch ex As Exception

        End Try
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If Not Me.m_bInitialized Then Return

        Dim bConfigOK As Boolean = False
        Dim bActive As Boolean = False

        Dim strEffort As String = Path.GetFileName(Me.m_plugin.EffortFile)
        Dim strCells As String = Path.GetFileName(Me.m_plugin.LMECellsFile)

        If (Not strEffort.EndsWith(".csv")) Then strEffort = ""

        Try
            bConfigOK = Me.m_plugin.IsInputdataValid()
            bActive = Me.m_plugin.OverwriteEffort And bConfigOK

            Me.m_tbxPath.Text = Me.m_plugin.DataPath
            Me.m_chkUseSailCost.Checked = bActive
            Me.m_chkUseSailCost.Enabled = bConfigOK

            Dim lviEffort As New ListViewItem("Effort")
            lviEffort.SubItems.Add(strEffort)
            lviEffort.SubItems.Add(CStr(File.Exists(Me.m_plugin.EffortFile)))

            Dim lviCells As New ListViewItem("LME cells")
            lviCells.SubItems.Add(strCells)
            lviCells.SubItems.Add(CStr(File.Exists(Me.m_plugin.LMECellsFile)))

            Me.m_lvValidation.SuspendLayout()
            Me.m_lvValidation.Items.Clear()
            Me.m_lvValidation.Items.AddRange(New ListViewItem() {lviCells, lviEffort})

            Me.m_lvValidation.ResumeLayout()

            Select Case Me.m_plugin.RunMode
                Case cSailCostPlugin.eRunMode.Org : Me.m_rbRunModeOrg.Checked = True
                Case cSailCostPlugin.eRunMode.FixedEffort : Me.m_rbRunModeFixed.Checked = True
                Case cSailCostPlugin.eRunMode.NoFishing : Me.m_rbRunModeNone.Checked = True
            End Select
            Me.m_tbxRunModeFixedYear.Text = CStr(Me.m_plugin.FixedEffortYear)

            Me.BackColor = cSystemUtils.IIF(Me.m_plugin.OverwriteEffort, Color.LightGreen, SystemColors.Control)

        Catch ex As Exception

        End Try

    End Sub

#End Region ' Form overrides

#Region " Event handlers "

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)

        If (msg.Source = eCoreComponentType.TimeSeries) Then
            ' Lazy update UI
            Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
        End If

    End Sub

    Private Sub OnChanged()
        Me.UpdateControls()
    End Sub

    Private Sub OnChoosePath(sender As Object, e As EventArgs) _
        Handles m_btnChoosePath.Click

        Dim dlg As FolderBrowserDialog = cEwEFileDialogHelper.FolderBrowserDialog("Select GOM LME Effort data path",
                                                                                  Me.m_plugin.DataPath)
        If (dlg.ShowDialog() = DialogResult.OK) Then
            Me.m_plugin.DataPath = dlg.SelectedPath
        End If

        Me.UpdateControls()

    End Sub

    Private Sub OnUseSailCostToggled(sender As System.Object, e As System.EventArgs) _
        Handles m_chkUseSailCost.CheckedChanged

        If Not Me.m_bInitialized Then Return
        Try
            Me.m_plugin.OverwriteEffort = Me.m_chkUseSailCost.Checked
        Catch ex As Exception

        End Try
        Me.UpdateControls()

    End Sub

    Private Sub OnUseMortalitiesWriterToggled(sender As Object, e As EventArgs) _
        Handles m_cbUseMortalitiesWriter.CheckedChanged

        If Not Me.m_bInitialized Then Return
        Try
            Me.m_plugin.OverwriteEffort = Me.m_chkUseSailCost.Checked
        Catch ex As Exception

        End Try
        Me.UpdateControls()
    End Sub

    Private Sub OnRunModeChanged(sender As Object, e As EventArgs) _
        Handles m_rbRunModeOrg.CheckedChanged, m_rbRunModeFixed.CheckedChanged, m_rbRunModeNone.CheckedChanged

        If Not Me.m_bInitialized Then Return
        Try
            Me.m_plugin.RunMode = DirectCast(CInt(DirectCast(sender, Control).Tag), cSailCostPlugin.eRunMode)
        Catch ex As Exception
            Debug.Assert(False, "RadioButton tag most likely malformed, please check")
        End Try
    End Sub

    Private Sub OnFixedEffortYearChanged(sender As Object, e As EventArgs) Handles m_tbxRunModeFixedYear.TextChanged

        If Not Me.m_bInitialized Then Return
        Try
            Integer.TryParse(Me.m_tbxRunModeFixedYear.Text, Me.m_plugin.FixedEffortYear)
        Catch ex As Exception
        End Try
    End Sub

#End Region ' Event handlers

End Class