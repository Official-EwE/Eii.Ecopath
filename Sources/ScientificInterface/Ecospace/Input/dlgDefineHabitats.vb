' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit Habitats user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgEditHabitats

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
            Me.CenterToScreen()
            Me.m_grid.UIContext = Me.m_uic
            Me.UpdateControls()
        End Sub

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles OK_Button.Click

            ' Try to apply grid changes
            If Me.m_grid.Apply() = False Then
                ' Abort! Abort!
                Return
            End If

            ' Close dialog
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnInsert(sender As System.Object, e As System.EventArgs) Handles m_btnAddHabitat.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnDelete(sender As System.Object, e As System.EventArgs) Handles m_btnRemoveHabitat.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnKeep(sender As System.Object, e As System.EventArgs) Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnMoveUp(sender As Object, e As EventArgs) Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub OnMoveDown(sender As Object, e As EventArgs) Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub OnHabitatSelected() Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAddHabitat.Enabled = Me.m_grid.CanAddRow()
            Me.m_btnRemoveHabitat.Enabled = Me.m_grid.IsHabitatRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsHabitatRow() And Me.m_grid.IsFlaggedForDeletionRow()
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
        End Sub

#End Region ' Updating

    End Class

End Namespace
