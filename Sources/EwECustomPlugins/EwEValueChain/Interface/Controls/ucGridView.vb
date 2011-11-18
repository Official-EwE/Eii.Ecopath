Imports ScientificInterfaceShared.Controls.EwEGrid

Friend Class ucGridView

    Private m_qe As cQuickEditHandler = Nothing
    Private m_grid As EwEGrid = Nothing

    Public Sub New(grid As EwEGrid)
        MyBase.New()
        Me.InitializeComponent()
        Me.m_grid = grid
        Me.m_grid.Dock = Windows.Forms.DockStyle.Fill
        Me.m_plGrid.Controls.Add(Me.m_grid)
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.m_qe = New cQuickEditHandler()
        Me.m_qe.Attach(Me.m_grid, Me.m_grid.UIContext, Me.m_ts, "")
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If (Me.m_qe IsNot Nothing) Then
                Me.m_qe.Detach()
                Me.m_qe = Nothing
            End If

            If (Me.m_grid IsNot Nothing) Then
                Me.m_plGrid.Controls.Clear()
                Me.m_grid.UIContext = Nothing
                Me.m_grid.Dispose()
                Me.m_grid = Nothing
            End If
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try

    End Sub

End Class
