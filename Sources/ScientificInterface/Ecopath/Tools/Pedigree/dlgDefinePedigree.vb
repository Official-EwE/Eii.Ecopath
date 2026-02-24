' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports EwEUtils.Utilities

Namespace Ecopath

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog class, implements the user interface to add/remove/reorder 
    ''' pedigree levels in the EwE Scientific Interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgEditPedigree

        Private m_uic As cUIContext = Nothing
        Private m_varInitial As eVarNameFlags = eVarNameFlags.NotSet

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
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, Optional varInitial As eVarNameFlags = eVarNameFlags.NotSet)

            Me.InitializeComponent()
            Me.m_uic = uic
            Me.m_grid.UIContext = uic
            Me.m_varInitial = varInitial

        End Sub

#End Region

#Region " Event handlers "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim var As eVarNameFlags = eVarNameFlags.NotSet
            Dim descr As cVarnameTypeFormatter = Nothing
            Dim iSelection As Integer = 0

            Me.m_ilPretty.Images.Add(ScientificInterfaceShared.My.Resources.CommentHS)
            Me.m_tpRemarks.ImageIndex = 0

            ' Clear drop down
            Me.m_cmbVariable.Items.Clear()
            ' For all pedigree vars
            For iVariable As Integer = 1 To Me.m_uic.Core.nPedigreeVariables
                ' Get variable
                var = Me.m_uic.Core.PedigreeVariable(iVariable)
                ' Get descriptor
                descr = New cVarnameTypeFormatter()
                ' Add to combo
                Me.m_cmbVariable.Items.Add(descr.ToString(var, eDescriptorTypes.Name))

                If (var = Me.m_varInitial) Then iSelection = iVariable - 1
            Next
            ' Select 
            Me.m_cmbVariable.SelectedIndex = iSelection

            ' Done
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
            Me.DialogResult = DialogResult.OK
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(sender As System.Object, e As System.EventArgs) _
            Handles Cancel_Button.Click
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub OnVariableSelected(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbVariable.SelectedIndexChanged
            Dim iIndex As Integer = Me.m_cmbVariable.SelectedIndex
            Me.m_grid.VarName = Me.m_uic.Core.PedigreeVariable(iIndex + 1)
        End Sub

        Private Sub m_btnInsert_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnInsert.Click
            Me.m_grid.InsertRow()
            Me.UpdateControls()
        End Sub

        Private Sub OnSort(sender As System.Object, e As System.EventArgs) _
            Handles m_btnSort.Click
            Me.m_grid.Sort()
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

        Private Sub m_tbDescription_Validated(sender As Object, e As System.EventArgs)

            Me.m_grid.SelectedLevelDescription = Me.m_tbDescription.Text
        End Sub

        Private Sub OnDefaultAllColors(sender As System.Object, e As System.EventArgs) _
            Handles m_btnColorDefaultAll.Click
            Me.m_grid.SetDefaultColors()
        End Sub

        Private Sub OnDefaultColor(sender As System.Object, e As System.EventArgs) _
            Handles m_btnColorDefaultCurrent.Click
            Me.m_grid.SetDefaultColor()
        End Sub

        Private Sub OnSelectCustomColor(sender As System.Object, e As System.EventArgs) _
            Handles m_btnColorCustom.Click
            Me.m_grid.SelectCustomColor()
        End Sub

#End Region ' Event handlers 

#Region " Updating "

        Private Sub UpdateControls()

            Dim bIsDataRow As Boolean = Me.m_grid.IsDataRow()
            Dim bIsFlaggedForDeletion As Boolean = Me.m_grid.IsFlaggedForDeletionRow()

            Me.m_btnInsert.Enabled = Me.m_grid.CanInsertRow()
            Me.m_btnDelete.Enabled = bIsDataRow And (Not bIsFlaggedForDeletion)
            Me.m_btnKeep.Enabled = bIsDataRow And bIsFlaggedForDeletion
            Me.m_btnSort.Enabled = Me.m_grid.CanSort()

            Me.m_btnColorDefaultCurrent.Enabled = bIsDataRow
            Me.m_btnColorCustom.Enabled = bIsDataRow

            Me.m_tbDescription.Enabled = bIsDataRow
            Me.m_tbDescription.Text = Me.m_grid.SelectedLevelDescription

        End Sub

#End Region ' Updating

    End Class

End Namespace