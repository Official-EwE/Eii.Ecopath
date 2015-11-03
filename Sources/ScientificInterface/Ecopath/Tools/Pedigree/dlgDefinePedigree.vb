' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Windows.Forms
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

        Private m_uic As cUIContext = Nothing
        Private m_varInitial As eVarNameFlags = eVarNameFlags.NotSet

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new instance of this class.
        ''' </summary>
        ''' <param name="uic">The <see cref="cUIContext">UI context</see> to connect to.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal uic As cUIContext, Optional ByVal varInitial As eVarNameFlags = eVarNameFlags.NotSet)

            Me.InitializeComponent()
            Me.m_uic = uic
            Me.m_grid.UIContext = uic
            Me.m_varInitial = varInitial

        End Sub

#End Region

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
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
                Me.m_cmbVariable.Items.Add(descr.GetDescriptor(var, eDescriptorTypes.Name))

                If (var = Me.m_varInitial) Then iSelection = iVariable - 1
            Next
            ' Select 
            Me.m_cmbVariable.SelectedIndex = iSelection

            ' Done
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
            Dim iIndex As Integer = Me.m_cmbVariable.SelectedIndex
            Me.m_grid.VarName = Me.m_uic.Core.PedigreeVariable(iIndex + 1)
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

        Private Sub m_tbDescription_Validated(ByVal sender As Object, ByVal e As System.EventArgs)

            Me.m_grid.SelectedLevelDescription = Me.m_tbDescription.Text
        End Sub

        Private Sub OnSetDefaults(ByVal sender As Object, ByVal e As EventArgs) _
            Handles m_btnCreateDefaultLevels.Click
            Me.m_grid.CreateDefaults()
        End Sub

        Private Sub OnDefaultAllColors(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnColorDefaultAll.Click
            Me.m_grid.SetDefaultColors()
        End Sub

        Private Sub OnDefaultColor(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnColorDefaultCurrent.Click
            Me.m_grid.SetDefaultColor()
        End Sub

        Private Sub OnSelectCustomColor(ByVal sender As System.Object, ByVal e As System.EventArgs) _
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