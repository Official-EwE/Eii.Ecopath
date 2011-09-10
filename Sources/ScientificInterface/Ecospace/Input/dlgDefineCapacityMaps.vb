#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Define Capacity Maps user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgDefineCapacityMaps

#Region " Private variables "

        Private m_uic As cUIContext = Nothing

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_grid.UIContext = Me.m_uic
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_grid.Apply() = False Then
                ' Abort! Abort!
                Return
            End If

            ' Close dialog
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnAdd.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnRemove.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_HabitatGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAdd.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemove.Enabled = Me.m_grid.IsMapRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsMapRow() And Me.m_grid.IsFlaggedForDeletionRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace
