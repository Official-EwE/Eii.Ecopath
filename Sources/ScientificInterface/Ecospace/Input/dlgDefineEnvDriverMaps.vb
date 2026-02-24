' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Dialog, implementing the Ecospace Edit Input Maps user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class dlgDefineEnvDriverMaps

#Region " Private variables "

        Private m_uic As cUIContext = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Events "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.m_grid.UIContext = Me.m_uic
            Me.UpdateControls()
        End Sub

        Private Sub OnMoveUp(sender As System.Object, e As System.EventArgs) _
            Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub OnMoveDown(sender As System.Object, e As System.EventArgs) _
            Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(sender As System.Object, e As System.EventArgs) _
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

        Private Sub Cancel_Button_Click(sender As System.Object, e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnAddLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAdd.Click
            Me.m_grid.AppendRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnRemoveLayer(sender As System.Object, e As System.EventArgs) _
            Handles m_btnRemoveHabitat.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_HabitatGrid_OnSelectionChanged() _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnAdd.Enabled = True
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
            Me.m_btnRemoveHabitat.Enabled = Me.m_grid.IsLayerRow() And (Not Me.m_grid.IsFlaggedForDeletionRow()) And (Me.m_grid.CanRemoveRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsLayerRow() And Me.m_grid.IsFlaggedForDeletionRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace
