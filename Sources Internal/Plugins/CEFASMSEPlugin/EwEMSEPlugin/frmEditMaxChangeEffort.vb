Imports System.IO
Imports ScientificInterfaceShared.Controls
Imports LumenWorks.Framework.IO.Csv

Public Class frmEditDecreaseEffort

    Private m_plugin As cMSE = Nothing

    Public Sub Init(ByVal uic As cUIContext, ByVal Plugin As cMSE)

        Me.m_plugin = Plugin

    End Sub

    Private Sub frmEditDecreaseEffort_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim csv As CsvReader
        Dim reader As StreamReader = Nothing
        Dim irow As Integer

        If File.Exists(cMSEUtils.MSEFile(m_plugin.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv")) Then

            reader = cMSEUtils.GetReader(cMSEUtils.MSEFile(m_plugin.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv"))
            csv = New CsvReader(reader, True)
            While Not csv.EndOfStream
                csv.ReadNextRecord()
                irow = dgvMaxDecreaseEffort.Rows.Add()
                dgvMaxDecreaseEffort.Rows.Item(irow).Cells(0).Value = csv(0)
                dgvMaxDecreaseEffort.Rows.Item(irow).Cells(1).Value = csv(1)
                dgvMaxDecreaseEffort.Rows.Item(irow).Cells(2).Value = csv(2)
            End While

            csv.Dispose()

        End If



    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnOK_Click(sender As System.Object, e As System.EventArgs) Handles btnOK.Click

        Dim csv_out As New StreamWriter(cMSEUtils.MSEFile(m_plugin.DataPath, cMSEUtils.eMSEPaths.Fleet, "ChangesInEffortLimits.csv"), False)

        csv_out.WriteLine("FleetNumber, FleetName, MaxChangeEffort")
        For irow = 0 To dgvMaxDecreaseEffort.Rows.Count - 1
            csv_out.WriteLine(dgvMaxDecreaseEffort.Rows.Item(irow).Cells(0).Value & "," & _
                                dgvMaxDecreaseEffort.Rows.Item(irow).Cells(1).Value & "," & _
                                dgvMaxDecreaseEffort.Rows.Item(irow).Cells(2).Value)
        Next

        csv_out.Dispose()

        Me.Close()

    End Sub
End Class