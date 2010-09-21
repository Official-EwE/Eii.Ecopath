#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Windows.Forms
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region

Namespace Ecopath

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog class, implements the user interface to add/remove/reorder 
    ''' pedigree levels in the EwE Scientific Interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgEditPedigree

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext)

            Me.InitializeComponent()
            Me.m_grid.UIContext = uic

        End Sub

#End Region

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.m_cmbVariable.Items.Clear()
            For Each vn As eVarNameFlags In cPedigreeManager.SupportVariables
                Me.m_cmbVariable.Items.Add(vn)
            Next
            Me.m_cmbVariable.SelectedIndex = 0
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

        Private Sub OnVariableSelected(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_cmbVariable.SelectedIndexChanged
            Me.m_grid.VarName = DirectCast(Me.m_cmbVariable.SelectedItem, eVarNameFlags)
        End Sub

        Private Sub m_btnInsert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnInsert.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnSort(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSort.Click
            Me.m_grid.Sort()
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

        Private Sub m_grid_OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) _
            Handles m_grid.OnSelectionChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()

            Me.m_btnSort.Enabled = Me.m_grid.CanSort()
            Me.m_btnInsert.Enabled = Me.m_grid.CanInsertRow()
            Me.m_btnDelete.Enabled = Me.m_grid.IsDataRow() And (Not Me.m_grid.IsFlaggedForDeletionRow())
            Me.m_btnKeep.Enabled = Me.m_grid.IsDataRow() And Me.m_grid.IsFlaggedForDeletionRow()

        End Sub

#End Region ' Updating

    End Class

End Namespace