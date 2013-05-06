
Imports EwECore
Imports EwECore.MSEBatchManager


Public Class frmMSEBatch

    Private m_Manager As cMSEBatchManager

    Private m_cmdFile As String


    Public Sub New(ByVal MSEBatchManager As MSEBatchManager.cMSEBatchManager)
        MyBase.New()

        InitializeComponent()

        Me.m_Manager = DirectCast(MSEBatchManager, cMSEBatchManager)

        Me.m_Manager.onMessageDelegate = AddressOf Me.onMessage

    End Sub



    Private Sub btSelect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btSelect.Click
        Dim fd As New Windows.Forms.OpenFileDialog()

        fd.Filter = "csv files (*.cvs)|*.csv|All files (*.*)|*.*"
        fd.RestoreDirectory = True

        If fd.ShowDialog() = System.Windows.Forms.DialogResult.OK Then

            Me.lstOutput.Items.Clear()

            Me.m_cmdFile = fd.FileName
            Me.lbCommandFile.Text = System.IO.Path.GetFileName(Me.m_cmdFile)
            Me.lbCommandFileDir.Text = System.IO.Path.GetDirectoryName(Me.m_cmdFile)

            Me.m_Manager.ReadCommandFile(Me.m_cmdFile)

        End If

    End Sub


    Private Sub btRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btRun.Click

        If Me.m_cmdFile = String.Empty Then

            Exit Sub
        End If

        Me.lstOutput.Items.Clear()

        If System.IO.File.Exists(Me.m_cmdFile) Then
            Me.m_Manager.Run()
        Else
            Me.onMessage("Sorry cannot open file " & Me.m_cmdFile)
        End If

    End Sub


    Private Sub onMessage(ByVal str As String)
        Try
            Me.lstOutput.Items.Add(str)
            Me.lstOutput.SelectedIndex = Me.lstOutput.Items.Count - 1
            Me.lstOutput.Refresh()
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onMessage() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub btStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btStop.Click
        Me.m_Manager.BatchData.StopRun = True
    End Sub
End Class