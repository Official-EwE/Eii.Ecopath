
Imports System.IO

Public Class frmFLEMReader

    Private plugin As cFLEMPluginPoint
    Public Sub Init(ByVal FLEMPlugin As cFLEMPluginPoint)
        plugin = FLEMPlugin
    End Sub

    Private Sub frmFLEMReader_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        Try
            Me.chkForcePP.Checked = plugin.ForcePPSalinity
            Me.chkForceHabCap.Checked = plugin.VaryHabCapWithCultch
            Me.lbForceFile.Text = plugin.ForceFile

            LoadGroups(Me.cbHabCap)

            If plugin.iHabCapModGroup <= Me.Core.nGroups Then
                cbHabCap.SelectedIndex = plugin.iHabCapModGroup - 1
            End If

        Catch ex As Exception

        End Try

    End Sub


    Private Sub OnchkForcePP_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkForcePP.CheckedChanged
        plugin.ForcePPSalinity = Me.chkForcePP.Checked
    End Sub

    Private Sub OnchkForceHabCap_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles chkForceHabCap.CheckedChanged
        plugin.VaryHabCapWithCultch = chkForceHabCap.Checked
    End Sub



    Private Sub btForcingFile_Click(sender As System.Object, e As System.EventArgs) Handles btForcingFile.Click
        Dim FileDialog As New System.Windows.Forms.OpenFileDialog
        FileDialog.DefaultExt = "nuo"
        FileDialog.Filter = "FLEM files (*.nuo)|*.nuo|All files (*.*)|*.*"
        Dim result As System.Windows.Forms.DialogResult = FileDialog.ShowDialog()

        If result = Windows.Forms.DialogResult.OK Then
            Dim newForcingFile As String = FileDialog.FileName
            If File.Exists(newForcingFile) Then
                lbForceFile.Text = newForcingFile
                plugin.ForceFile = newForcingFile
            End If
        End If

    End Sub


    Private Sub LoadGroups(cbBox As Windows.Forms.ComboBox)

        For igrp As Integer = 1 To plugin.core.nGroups
            cbBox.Items.Add(plugin.core.EcoPathGroupInputs(igrp).Name)
        Next

    End Sub

   
    Private Sub cbHabCap_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbHabCap.SelectedIndexChanged
        plugin.iHabCapModGroup = cbHabCap.SelectedIndex + 1
    End Sub

End Class