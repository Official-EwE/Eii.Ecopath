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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports ScientificInterfaceShared.Controls
Imports LumenWorks.Framework.IO.Csv
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class frmEditDecreaseEffort

    Private m_plugin As cMSE = Nothing

    Public Class cDecreaseInEffort

        Public Sub New(ByVal FleetIndex As Integer, ByVal FleetName As String, ByVal MaxDecreaseInEffort As Double)
            Me.FleetIndex = FleetIndex
            Me.FleetName = FleetName
            Me.MaxDecreaseInEffort = MaxDecreaseInEffort
        End Sub

        Public Property FleetIndex() As Integer
        Public Property FleetName() As String
        Public Property MaxDecreaseInEffort() As Double

    End Class

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(ByVal uic As cUIContext, ByVal Plugin As cMSE)
        Me.m_plugin = Plugin
        'Me.Grid = m_grid
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim csv As CsvReader
        Dim reader As StreamReader = Nothing
        Dim irow As Integer
        Dim strPath As String = cMSEUtils.MSEFile(m_plugin.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv")

        If File.Exists(strPath) Then

            reader = cMSEUtils.GetReader(strPath)
            csv = New CsvReader(reader, True)
            While Not csv.EndOfStream
                If csv.ReadNextRecord() Then
                    irow = dgvMaxDecreaseEffort.Rows.Add()
                    dgvMaxDecreaseEffort.Rows.Item(irow).Cells(0).Value = cStringUtils.ConvertToInteger(csv(0))
                    dgvMaxDecreaseEffort.Rows.Item(irow).Cells(1).Value = cMSEUtils.FromCSVField(csv(1))
                    dgvMaxDecreaseEffort.Rows.Item(irow).Cells(2).Value = cStringUtils.ConvertToDouble(csv(2))
                End If
            End While

            csv.Dispose()
            cMSEUtils.ReleaseReader(reader)
        End If

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Try
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        Try

            Dim csv_out As New StreamWriter(cMSEUtils.MSEFile(m_plugin.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv"), False)

            ' JS 19Oct13: Avoid spaces in CSV headers, this may confuse readers
            csv_out.WriteLine("FleetNumber,FleetName,MaxChangeEffort")
            For irow = 0 To dgvMaxDecreaseEffort.Rows.Count - 1
                Dim row As DataGridViewRow = dgvMaxDecreaseEffort.Rows.Item(irow)
                csv_out.WriteLine("{0},{1},{2}", _
                                  cStringUtils.FormatNumber(row.Cells(0).Value), _
                                  cStringUtils.ToCSVField(row.Cells(1).Value), _
                                  cStringUtils.FormatNumber(row.Cells(2).Value))
            Next

            csv_out.Dispose()

            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        Catch ex As Exception

        End Try

    End Sub

End Class