' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecopath

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog class, implements the user interface to add/remove/reorder fleets 
    ''' in the EwE Scientific Interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EditFleets

#Region " Constructor "

        ''' <summary>
        ''' Parameterless constructor added for the benefit of LSA Creator. Do not use.
        ''' </summary>
        <Obsolete("Do not use parameterless constructor")>
        Public Sub New()
            Me.InitializeComponent()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
        ''' <param name="fleet">A fleet to select, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext,
                       Optional fleet As cEcopathFleetInput = Nothing)

            Me.InitializeComponent()

            Me.m_grid.UIContext = uic
            Me.m_grid.SelectFleet(fleet)

        End Sub

#End Region

#Region " Event handlers "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
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

        Private Sub m_btnInsert_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnInsert.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveUp_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveDown_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDelete.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_grid_OnSelectionChanged() _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnDefaultAll(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDefaultAll.Click
            Me.m_grid.SetDefaultFleetColors()
        End Sub

        Private Sub OnDefaultCurrent(sender As System.Object, e As System.EventArgs) _
            Handles m_btnDefaultCurrent.Click
            Me.m_grid.SetDefaultFleetColor(Me.m_grid.SelectedRow)
        End Sub

        Private Sub OnCustomColor(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCustom.Click
            Me.m_grid.SelectCustomFleetColor(Me.m_grid.SelectedRow)
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
            Me.m_btnInsert.Enabled = Me.m_grid.CanInsertRow()
            Me.m_btnDelete.Enabled = Me.m_grid.IsFleetRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsFleetRow() And Me.m_grid.IsFlaggedForDeletionRow()

            Me.m_btnDefaultCurrent.Enabled = Me.m_grid.IsFleetRow()
            Me.m_btnCustom.Enabled = Me.m_grid.IsFleetRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace