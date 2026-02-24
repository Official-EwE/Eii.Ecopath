' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit MPAs user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEditMPAs

#Region " Private variables "

        Private m_uic As cUIContext = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Event handlers "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_grid.UIContext = Me.m_uic
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveUp_Click(sender As Object, e As EventArgs) Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowsUp()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveDown_Click(sender As Object, e As EventArgs) Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowsDown()
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(sender As System.Object, e As System.EventArgs) Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_grid.Apply() = False Then
                ' Abort! Abort!
                Return
            End If

            ' Close dialog
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(sender As System.Object, e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub m_btnInsert_Click(sender As System.Object, e As System.EventArgs) Handles m_btnAddMPA.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(sender As System.Object, e As System.EventArgs) Handles m_btnRemoveMPA.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(sender As System.Object, e As System.EventArgs) Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_grid_OnSelectionChanged() Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAddMPA.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemoveMPA.Enabled = Me.m_grid.IsMPARow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsMPARow() And Me.m_grid.IsFlaggedForDeletionRow()
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
        End Sub

#End Region ' Updating

    End Class

End Namespace
