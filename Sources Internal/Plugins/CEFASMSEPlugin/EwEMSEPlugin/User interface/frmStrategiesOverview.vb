Imports ScientificInterfaceShared.Controls
Imports SourceGrid2

Public Class frmStrategiesOverview

    Private m_mse As cMSE = Nothing
    Private m_data As Strategies = Nothing

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(ByVal uic As cUIContext, ByVal mse As cMSE)
        Me.m_mse = mse
        Me.m_data = mse.Strategies
        'Me.m_data = New Strategies(mse, mse.Core)
        Me.m_data.Load()
        Me.Grid = m_grid
        'Me.m_grid.Init(Me.m_data)
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.QuickEditHandler.ShowImportExport = False
        Me.QuickEditHandler.Attach(Me.m_grid, Me.UIContext, Me.m_ts)

        Me.m_grid.Init(Me.m_data)

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Me.QuickEditHandler.Detach()
        MyBase.OnFormClosed(e)
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
    Handles m_btnSave.Click

        Try
            ' Save to default location
            If Me.m_data.Save("") Then
                Me.DialogResult = Windows.Forms.DialogResult.OK
                Me.Close()
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub m_btnCheckAll_Click(sender As Object, e As EventArgs) Handles m_btnCheckAll.Click

        m_grid.CheckAll()

    End Sub

    Private Sub m_btnCheckNone_Click(sender As Object, e As EventArgs) Handles m_btnCheckNone.Click

        m_grid.UncheckAll()

    End Sub
End Class