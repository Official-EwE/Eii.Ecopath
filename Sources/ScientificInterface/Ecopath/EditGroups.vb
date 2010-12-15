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
    ''' Dialog class, implements the user interface to add/remove/reorder groups, 
    ''' and change multi-stanza compositions, in the EwE Scientific Interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EditGroups

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
        ''' <param name="group">A group to select, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, _
                              Optional ByVal group As cEcoPathGroupInput = Nothing)

            Me.InitializeComponent()

            Me.m_grid.UIContext = uic
            Me.m_grid.SelectGroup(group)

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
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

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnInsert.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnMoveUp.Click
            Me.m_grid.MoveRowUp()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnMoveDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnMoveDown.Click
            Me.m_grid.MoveRowDown()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnDelete.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_btnPreserve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnKeep.Click
            Me.m_grid.ToggleDeleteRow()
            Me.UpdateControls()
        End Sub

        Private Sub m_GroupGrid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

        Private Sub OnColourDefaultAll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_bntColorDefaultAll.Click
            Me.m_grid.SetDefaultGroupColors()
        End Sub

        Private Sub OnColourAlternateAll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnColorAlternateAll.Click
            Me.m_grid.SetAlternatingGroupColors()
        End Sub

        Private Sub OnColourRandomAll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnRandomAll.Click
            Me.m_grid.SetRandomGroupColors()
        End Sub

        Private Sub OnColourDefaultCurrent(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnColourDefaultCurrent.Click
            Me.m_grid.SetDefaultGroupColor(Me.m_grid.SelectedRow)
        End Sub

        Private Sub OnColourCustomCurrent(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnColourCustomCurrent.Click
            Me.m_grid.SelectCustomColor()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()
            Me.m_btnMoveUp.Enabled = Me.m_grid.CanMoveRowUp()
            Me.m_btnMoveDown.Enabled = Me.m_grid.CanMoveRowDown()
            Me.m_btnInsert.Enabled = Me.m_grid.CanInsertRow()
            Me.m_btnDelete.Enabled = Me.m_grid.IsGroupRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsGroupRow() And Me.m_grid.IsFlaggedForDeletionRow()
            Me.m_btnColourCustomCurrent.Enabled = Me.m_grid.IsGroupRow()
            Me.m_btnColourDefaultCurrent.Enabled = Me.m_grid.IsGroupRow()
        End Sub

#End Region ' Updating

    End Class

End Namespace

