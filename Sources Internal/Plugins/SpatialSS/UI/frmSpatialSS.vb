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

Option Strict On
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmSpatialSS

    Private m_timeseries As cSpatialTimeSeries = Nothing

    Public Sub New(uic As cUIContext, ts As cSpatialTimeSeries)
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_timeseries = ts
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub ChooseTSFile(sender As Object, e As EventArgs) Handles m_btnChoose.Click

        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Select reference CSF file", My.Settings.CSVFile, SharedResources.FILEFILTER_CSV)
        If (ofd.ShowDialog() = DialogResult.OK) Then
            If (Me.m_timeseries.Read(ofd.FileName)) Then
                ' Yippee, time series has data
                My.Settings.CSVFile = ofd.FileName
                My.Settings.Save()
            Else
                ' Error!
                Debug.Assert(False, "Failed to read time series " & ofd.FileName)
            End If
        End If

    End Sub
End Class