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
' Copyright 1991- 
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.SystemUtilities

#End Region ' Imports

Public Class dlgMergeGroups

    Private m_uic As cUIContext = Nothing
    Private m_engine As cEcopathMergeGroups = Nothing
    Private m_bInUpdate As Boolean = True
    Private m_images As New ImageList()

    Public Sub New(uic As cUIContext)

        Me.m_uic = uic
        Me.m_engine = New cEcopathMergeGroups(Me.m_uic.Core)

        Me.InitializeComponent()

        Me.m_images.Images.Add(SharedResources.OK)
        Me.m_images.Images.Add(SharedResources.Warning)
        Me.m_images.Images.Add(SharedResources.Critical)
        Me.m_tcInputs.ImageList = Me.m_images

        Me.m_grid.Init(uic, Me.m_engine.Data)
        Me.m_gridDietComp.Init(uic, Me.m_engine.Data)
        Me.m_gridTaxa.Init(uic, Me.m_engine.Data)

    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Debug.Assert(Me.m_uic.Core.StateMonitor.HasEcopathRan)

        Dim core As cCore = Me.m_uic.Core

        For i As Integer = 1 To core.nGroups
            Me.m_cmbTarget.Items.Add(core.EcoPathGroupInputs(i))
        Next

        Me.m_bInUpdate = False
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

#End Region ' Overrides 

#Region " Events "

    Private Sub OnFormatGroupItem(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbTarget.Format, m_cmbMerge.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
            Dim grp As cCoreGroupBase = DirectCast(e.ListItem, cCoreGroupBase)

            If (Not grp.Disposed) Then
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        Catch ex As Exception
            ' mmm
        End Try

    End Sub

    Private Sub OnTargetSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbTarget.SelectedIndexChanged

        Dim core As cCore = Me.m_uic.Core
        Dim iTarget As Integer = Me.SelectedTarget()
        Dim grps As Integer() = Me.m_engine.CompatibleGroups(iTarget)

        Me.m_cmbMerge.Items.Clear()
        For i As Integer = 0 To grps.Count - 1
            Me.m_cmbMerge.Items.Add(Me.m_uic.Core.EcoPathGroupInputs(grps(i)))
        Next

        If (iTarget > 0) Then
            Me.m_tbxNewName.Text = Me.m_uic.Core.EcoPathGroupInputs(iTarget).Name
        End If

        Me.UpdatePreview(True)

    End Sub

    Private Sub OnMergeSelected(sender As Object, e As EventArgs) _
        Handles m_cmbMerge.SelectedIndexChanged

        Me.UpdatePreview(True)

    End Sub

    Private Sub OnNameChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxNewName.TextChanged

        Me.UpdatePreview(False)

    End Sub

    Private Sub OnEstimateVarChanged(sender As Object, e As EventArgs) _
        Handles m_rbB.CheckedChanged, m_rbPB.CheckedChanged, m_rbQB.CheckedChanged, m_rbEE.CheckedChanged

        Me.UpdatePreview(False)

    End Sub

    Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
        Handles m_btnOK.Click

        If (Me.m_engine.Merge()) Then
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        Else
            ' Throw some kind of error...
        End If

    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnClickGeomar(sender As Object, e As EventArgs) _
        Handles m_pbLogo.Click

        Me.OpenLink("http://www.geomar.de")

    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedTarget() As Integer

        Dim item As Object = Me.m_cmbTarget.SelectedItem

        If (item Is Nothing) Then Return cCore.NULL_VALUE
        If (Not TypeOf (item) Is cCoreGroupBase) Then Return cCore.NULL_VALUE
        Return DirectCast(item, cCoreGroupBase).Index

    End Function

    Private Function SelectedMerge() As Integer

        Dim item As Object = Me.m_cmbMerge.SelectedItem

        If (item Is Nothing) Then Return cCore.NULL_VALUE
        If (Not TypeOf (item) Is cCoreGroupBase) Then Return cCore.NULL_VALUE
        Return DirectCast(item, cCoreGroupBase).Index

    End Function

    Private Function SelectedName() As String

        Return Me.m_tbxNewName.Text

    End Function

    Private Function SelectedEstimation() As cEcopathMergeGroups.eEstimate
        If Me.m_rbB.Checked Then Return cEcopathMergeGroups.eEstimate.Biomass
        If Me.m_rbPB.Checked Then Return cEcopathMergeGroups.eEstimate.PB
        If Me.m_rbQB.Checked Then Return cEcopathMergeGroups.eEstimate.QB
        If Me.m_rbEE.Checked Then Return cEcopathMergeGroups.eEstimate.EE
        Return cEcopathMergeGroups.eEstimate.NotSet
    End Function

    'Private Function SelectedGroups() As Integer()

    '    Dim lgroups As New List(Of Integer)
    '    Dim iTarget As Integer = Me.SelectedTarget

    '    If (iTarget > 0) Then lgroups.Add(iTarget)

    '    For Each item As Object In Me.m_clbGroups.CheckedItems
    '        Dim group As cCoreGroupBase = DirectCast(item, cCoreGroupBase)
    '        If Not lgroups.Contains(group.Index) Then lgroups.Add(group.Index)
    '    Next
    '    Return lgroups.ToArray()

    'End Function

    Private Sub UpdateControls()

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Dim data As cEcopathMergeGroupsDatastructures = Me.m_engine.Data

        Me.m_rbB.Checked = (data.Estimate = cEcopathMergeGroups.eEstimate.Biomass)
        Me.m_rbPB.Checked = (data.Estimate = cEcopathMergeGroups.eEstimate.PB)
        Me.m_rbQB.Checked = (data.Estimate = cEcopathMergeGroups.eEstimate.QB)
        Me.m_rbEE.Checked = (data.Estimate = cEcopathMergeGroups.eEstimate.EE)

        Dim iTarget As Integer = Me.SelectedTarget()
        Dim iMerge As Integer = Me.SelectedMerge()
        Dim bHasEstimate As Boolean = (Me.SelectedEstimation <> cEcopathMergeGroups.eEstimate.NotSet)
        Dim bCanMerge As Boolean = iTarget > 0 And iMerge > 0

        Me.m_cmbMerge.Enabled = (iTarget > 0)
        Me.m_btnOK.Enabled = bCanMerge And bHasEstimate

        Me.m_tabBasicInput.ImageIndex = cSystemUtils.IIF(bCanMerge And bHasEstimate, 0, 2)
        Me.m_tabDiets.ImageIndex = 0
        Me.m_tabTaxonomy.ImageIndex = 0

        Me.m_bInUpdate = False

    End Sub

    Private Sub UpdatePreview(bCalcEstimation As Boolean)

        If (Me.m_bInUpdate) Then Return

        Try
            If bCalcEstimation Then
                Me.m_tbxNewName.Text = Me.m_engine.GroupName(Me.SelectedTarget(), Me.SelectedMerge())
            End If
            Me.m_engine.Calculate(Me.SelectedTarget(), Me.SelectedMerge(), Me.SelectedName(), Me.SelectedEstimation(), bCalcEstimation)
            Me.m_grid.UpdateContent()

            If bCalcEstimation Then
                Me.m_gridDietComp.RefreshContent()
            Else
                Me.m_gridDietComp.UpdateContent()
            End If
            ' Full refresh
            Me.m_gridTaxa.RefreshContent()

        Catch ex As Exception

        End Try
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Open an external link.
    ''' </summary>
    ''' <param name="strURL">The link to navigate to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OpenLink(strURL As String)

        Try
            Dim cmd As cBrowserCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            If (cmd IsNot Nothing) Then
                cmd.Invoke(strURL)
            End If
        Catch ex As Exception
            cLog.Write(ex, "dlgMergeGroups::NavigateTo(" & strURL & ")")
        End Try

    End Sub


#End Region ' Internals

End Class