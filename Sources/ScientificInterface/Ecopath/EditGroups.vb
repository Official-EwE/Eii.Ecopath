#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands

#End Region

Namespace Ecopath

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Interface to create and destroy groups, assign multi-stanza configurations
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EditGroups

#Region " Private variables "

        ''' <summary>Reference to the one core.</summary>
        Private m_Core As cCore

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()
            ' Add any initialization after the InitializeComponent() call.
            m_Core = cCore.GetInstance()

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Private Sub EditGroups_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Me.UpdateControls()
        End Sub

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

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

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnInsert.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnDelete.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnPreserve.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_GroupGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

        Private Sub m_bntColorScale_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_bntColorScale.Click
            Me.m_grid.SetScaleGroupColors()
        End Sub

        Private Sub m_btnColorDefaults_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnColorDefaults.Click
            Me.m_grid.SetAlternatingGroupColors()
        End Sub

        Private Sub m_btnCustomColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnCustomColour.Click
            Me.m_grid.SelectCustomColor()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
            Me.m_btnInsert.Enabled = Me.m_grid.CanInsertRow()
            Me.m_btnDelete.Enabled = Me.m_grid.IsGroupRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnPreserve.Enabled = Me.m_grid.IsGroupRow() And Me.m_grid.IsFlaggedForDeletionRow()
            Me.m_btnCustomColour.Enabled = Me.m_grid.IsGroupRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace

