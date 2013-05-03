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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports EwECore
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Main interface for the biodiversity indicators plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmMain

#Region " Variables "

    Private m_ecosimWrapper As cEcosimGraphWrapper = Nothing
    Private m_ecospaceWrapper As cEcospaceMapWrapper = Nothing
    Private m_mcWrapper As cMCGraphWrapper = Nothing

    Private m_ppt As cEwEBioDiversityIndicatorsPlugin = Nothing

    Private m_settings As cIndicatorSettings = Nothing
    Private m_bInUpdate As Boolean = False

#End Region ' Variables

#Region " Construction "

    Public Sub New(ByVal uic As cUIContext, pluginpoint As cEwEBioDiversityIndicatorsPlugin)

        MyBase.New()
        Me.UIContext = uic

        Me.m_ppt = pluginpoint
        Me.m_settings = pluginpoint.m_settings

        Me.InitializeComponent()

        Me.m_ecosimWrapper = New cEcosimGraphWrapper()
        Me.m_ecospaceWrapper = New cEcospaceMapWrapper()
        Me.m_mcWrapper = New cMCGraphWrapper()

    End Sub

#End Region ' Construction

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form is first loaded; content to the underlying framework and intialize
    ''' the form content.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Dim ndSel As TreeNode = Nothing
        Me.m_bInUpdate = True

        Try
            Me.m_grid.Attach(Me.m_settings)
            Me.m_grid.UIContext = Me.UIContext
        Catch ex As Exception
            Debug.Assert(False, "Grid not able to attach")
        End Try

        Try
            Me.m_ecosimWrapper.Attach(Me.UIContext, Me.m_graphSim, Me.m_settings, Me.m_ppt.m_lIndEcosim, 0)
        Catch ex As Exception
            Debug.Assert(False, "Zed graph handler not able to attach")
        End Try

        Try
            Me.m_ecospaceWrapper.Attach(Me.UIContext, Me.m_ppt.m_dtIndEcospace, Me.m_settings, Me.m_tsMap, Me.m_map)
        Catch ex As Exception
            Debug.Assert(False, "Map stuff not able to attach")
        End Try

        Try
            Me.m_mcWrapper.Attach(Me.UIContext, Me.m_graphMC, Me.m_settings, Me.m_ppt.m_lIndMC, 0)
        Catch ex As Exception
            Debug.Assert(False, "Zed graph handler not able to attach")
        End Try

        Me.Text = My.Resources.CAPTION
        Me.TabText = My.Resources.CAPTION

        Try
            ' Populate tree view from indicator settings
            Me.m_tvIndicators.Nodes.Clear()
            For i As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
                ' Get indicator group from settings
                Dim grp As cIndicatorSettings.cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(i)
                ' Create treenode for this group
                Dim tnGrp As TreeNode = Me.m_tvIndicators.Nodes.Add(grp.Name)
                ' Make sure the group is attached to its node
                tnGrp.Tag = grp
                ' Show description as tooltip text
                tnGrp.ToolTipText = grp.Description

                For j As Integer = 0 To grp.NumIndicators - 1
                    ' Get indicator from group
                    Dim ind As cIndicatorSettings.cIndicatorInfo = grp.Indicator(j)
                    ' Create treenode for indicator
                    Dim tnInd As TreeNode = tnGrp.Nodes.Add(ind.Name)
                    ' Make sure the indicator is attached to its node
                    tnInd.Tag = ind
                    ' Show description as tooltip text
                    tnInd.ToolTipText = ind.Description

                    If (ndSel Is Nothing) Then ndSel = tnInd
                Next
            Next

            ' Expand all nodes in the tree
            Me.m_tvIndicators.ExpandAll()
            ' Select node
            Me.m_tvIndicators.SelectedNode = ndSel

        Catch ex As Exception
            ' Catch programming error
            Debug.Assert(False, ex.Message)
        End Try

        ' Initialize content of controls
        Me.m_cbAutoSaveCSV.Checked = My.Settings.AutoSaveCSV
        Me.m_cbRunWithEcopath.Checked = My.Settings.RunWithEcopath
        Me.m_cbRunWithEcosim.Checked = My.Settings.RunWithEcosim
        Me.m_cbRunWithEcospace.Checked = My.Settings.RunWithEcospace
        Me.m_cbRunWithMC.Checked = My.Settings.RunWithMC
        If (My.Settings.SaveToDefault) Then
            Me.m_rbDefault.Checked = True
        Else
            Me.m_rbCustom.Checked = True
        End If
        Me.m_tbxDefaultLocation.Text = Me.m_ppt.DefaultFolder
        Me.m_tbxOutputFolder.Text = My.Settings.CustomFolder

        ' Start listening to Ecopath, Ecosim, Ecospace and external messages (responses are handled in OnCoreMessage)
        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoPath, eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace, eCoreComponentType.Core}

        Me.m_bInUpdate = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form is officially closed; preserve what needs preserving and clean up.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Cleanup 
        Me.m_grid.Detach()
        Me.m_ecosimWrapper.Detach()
        Me.m_ecospaceWrapper.Detach()

        ' Stop listening to any messages
        Me.CoreComponents = Nothing

        ' Done
        MyBase.OnFormClosed(e)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' A message of one of the subscribed types has arrived. Respond accordingly.
    ''' </summary>
    ''' <param name="msg">The <see cref="cMessage"/> that arrived.</param>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
        MyBase.OnCoreMessage(msg)

        ' Weed out clutter - the EwE core produces quite a lot of progress messages while it
        ' executes. There is no need to respond to progress messages; any relevant state change
        ' is broadcasted via a proper notification message.
        If (msg.Importance = eMessageImportance.Progress) Then Return

        ' Is an external message?
        If (msg.Type = eMessageType.GlobalSettingsChanged) Then
            ' #Yes: Update default location because systemwide settings may have changed
            Me.m_bInUpdate = True
            Me.m_tbxDefaultLocation.Text = Me.m_ppt.DefaultFolder
            Me.m_cbAutoSaveCSV.Checked = My.Settings.AutoSaveCSV
            Me.m_bInUpdate = False
        End If

        ' Update controls to reflect any core state changes
        Me.UpdateControls()

    End Sub

#End Region ' Overrides

#Region " Events "

    Private Sub OnTreeNodeSelected(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tvIndicators.AfterSelect
        cApplicationStatusNotifier.StartProgress(Me.UIContext.Core)
        Try
            ' Repopulate all indicators in response to a treenode selection change
            Me.UpdateIndicators(cEwEBioDiversityIndicatorsPlugin.eComponentType.Any)
        Catch ex As Exception
            ' Whoah
        End Try
        cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)
    End Sub

    Private Sub OnSaveToCSV(sender As System.Object, e As System.EventArgs) Handles m_btnSaveToCSV.Click
        ' Save selected component (path, sim, space or ...) to CSV
        Me.m_ppt.SaveToCSV(Me.SelectedTabComponent(), False)
    End Sub

    Private Sub OnAutoSaveCSVCChanged(sender As Object, e As System.EventArgs) Handles m_cbAutoSaveCSV.CheckedChanged
        ' User toggled AutoSaveCSV checkbox; update settings
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveCSV = Me.m_cbAutoSaveCSV.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnRunWithEcopathChanged(sender As Object, e As System.EventArgs) Handles m_cbRunWithEcopath.CheckedChanged

        ' User toggled RunWithEcopath checkbox; update settings
        If Me.m_bInUpdate Then Return
        My.Settings.RunWithEcopath = Me.m_cbRunWithEcopath.Checked
        My.Settings.Save()
        Me.UpdateControls()

        ' No longer run with Ecopath?
        If (Not My.Settings.RunWithEcopath) Then
            ' #Yes: clear results
            Me.m_ppt.ClearEcopathIndicators()
        End If

    End Sub

    Private Sub OnRunWithEcosimChanged(sender As Object, e As System.EventArgs) Handles m_cbRunWithEcosim.CheckedChanged

        ' User toggled RunWithEcosim checkbox; update settings
        If Me.m_bInUpdate Then Return
        My.Settings.RunWithEcosim = Me.m_cbRunWithEcosim.Checked
        My.Settings.Save()
        Me.UpdateControls()

        ' No longer run with Ecosim?
        If (Not My.Settings.RunWithEcosim) Then
            ' #Yes: clear results
            Me.m_ppt.ClearEcosimIndicators()
        End If

    End Sub

    Private Sub OnRunWithEcospaceChanged(sender As Object, e As System.EventArgs) Handles m_cbRunWithEcospace.CheckedChanged, m_cbRunWithMC.CheckedChanged

        ' User toggled RunWithEcospace checkbox; update settings
        If Me.m_bInUpdate Then Return
        My.Settings.RunWithEcospace = Me.m_cbRunWithEcospace.Checked
        My.Settings.Save()
        Me.UpdateControls()

        ' No longer run with Ecospace?
        If (Not My.Settings.RunWithEcospace) Then
            ' #Yes: clear results
            Me.m_ppt.ClearEcospaceIndicators()
        End If

    End Sub

    Private Sub OnRunWithMCChanged(sender As Object, e As System.EventArgs) Handles m_cbRunWithMC.CheckedChanged

        ' User toggled RunWithMC checkbox; update settings
        If Me.m_bInUpdate Then Return
        My.Settings.RunWithMC = Me.m_cbRunWithMC.Checked
        My.Settings.Save()
        Me.UpdateControls()

        ' No longer run with Ecosim?
        If (Not My.Settings.RunWithMC) Then
            ' #Yes: clear results
            Me.m_ppt.ClearMCIndicators()
        End If

    End Sub

    Private Sub OnTabSelected(sender As Object, e As System.EventArgs) Handles m_tcOutput.SelectedIndexChanged
        ' User selected a different tab (settings, path, sim, space, ...)
        ' Update any controls that rely on this selection
        Me.UpdateControls()
    End Sub

    Private Sub OnSaveLocationChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_rbDefault.CheckedChanged, m_tbxOutputFolder.TextChanged

        ' User has changed the content of controls that affect the save location
        ' Update settings accordingly

        ' Note that m_rbCustom.Checked is not validated here since m_rbDefault and m_rbCustom are mutally exclusive. 
        ' Only one radio button needs to be checked. If it was not for the custom path controls this interface should
        ' have been implemented via a check box. Ah well. This is much funner.

        My.Settings.SaveToDefault = Me.m_rbDefault.Checked
        My.Settings.CustomFolder = Me.m_tbxOutputFolder.Text
        My.Settings.Save()

    End Sub

    Private Sub OnBrowseOutputFolder(sender As System.Object, e As System.EventArgs) Handles m_btnChoose.Click
        ' User wants to browse for an output folder. Let's be nice.
        Me.PickOutputFolder()
    End Sub

    Private Sub OnVisitCSIC(sender As System.Object, e As System.EventArgs) Handles m_pbCSIC.Click
        ' User wants to visit CSIC
        Me.NavigateTo("http://www.csic.es/web/guest/home")
    End Sub

    Private Sub OnVisitICM(sender As System.Object, e As System.EventArgs) Handles m_pbICM.Click
        ' User wants to visit ICM
        Me.NavigateTo("http://www.icm.csic.es/")
    End Sub

    Private Sub OnClickDefaultLocation(sender As System.Object, e As System.EventArgs) Handles m_tbxDefaultLocation.Click
        Me.NavigateTo("file://" & Me.m_tbxDefaultLocation.Text)
    End Sub

#End Region ' Events

#Region " Public methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of controls that display indicators.
    ''' </summary>
    ''' <param name="component">The component that needs updating.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub UpdateIndicators(component As cEwEBioDiversityIndicatorsPlugin.eComponentType)

        ' Optimization: only update the component that was changed
        If (component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Any Or component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecopath) Then
            Me.m_grid.RefreshContent(Me.m_ppt.m_indEcopath)
        End If
        If (component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Any Or component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecosim) Then
            Me.m_ecosimWrapper.RefreshContent(Me.GetSelectedIndicatorGroup())
        End If
        If (component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Any Or component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecospace) Then
            Me.m_ecospaceWrapper.RefreshContent(Me.GetSelectedIndicator(), Me.m_ppt.m_indEcopath)
        End If
        If (component = cEwEBioDiversityIndicatorsPlugin.eComponentType.Any Or component = cEwEBioDiversityIndicatorsPlugin.eComponentType.MC) Then
            Me.m_mcWrapper.RefreshContent(Me.GetSelectedIndicatorGroup())
        End If

        ' Update state specific controls as a precaution
        Me.UpdateControls()

    End Sub

#End Region ' Public methods

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator group selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetSelectedIndicatorGroup() As cIndicatorSettings.cIndicatorInfoGroup

        Dim nd As TreeNode = Me.m_tvIndicators.SelectedNode
        If (nd Is Nothing) Then Return Nothing

        If TypeOf nd.Tag Is cIndicatorSettings.cIndicatorInfo Then
            nd = nd.Parent
        End If

        Return DirectCast(nd.Tag, cIndicatorSettings.cIndicatorInfoGroup)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetSelectedIndicator() As cIndicatorSettings.cIndicatorInfo

        Dim nd As TreeNode = Me.m_tvIndicators.SelectedNode
        If (nd Is Nothing) Then Return Nothing

        If TypeOf nd.Tag Is cIndicatorSettings.cIndicatorInfo Then
            Return DirectCast(nd.Tag, cIndicatorSettings.cIndicatorInfo)
        End If

        Return Nothing

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function SelectedTabComponent() As cEwEBioDiversityIndicatorsPlugin.eComponentType
        Select Case Me.m_tcOutput.SelectedIndex
            Case 1 : Return cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecopath
            Case 2 : Return cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecosim
            Case 3 : Return cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecospace
        End Select
        Return cEwEBioDiversityIndicatorsPlugin.eComponentType.Any
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="UpdateControls"/>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        Dim csm As cCoreStateMonitor = Nothing
        Dim bCanSave As Boolean = False

        csm = Me.UIContext.Core.StateMonitor

        Select Case Me.SelectedTabComponent
            Case cEwEBioDiversityIndicatorsPlugin.eComponentType.Any
                bCanSave = False
            Case cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecopath
                bCanSave = My.Settings.RunWithEcopath And csm.HasEcopathRan
            Case cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecosim
                bCanSave = My.Settings.RunWithEcosim And csm.HasEcosimRan
            Case cEwEBioDiversityIndicatorsPlugin.eComponentType.Ecospace
                bCanSave = My.Settings.RunWithEcospace And csm.HasEcospaceRan
        End Select

        Me.m_btnSaveToCSV.Enabled = bCanSave

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Show a given web page in the EwE browser window
    ''' </summary>
    ''' <param name="strURL"></param>
    ''' -----------------------------------------------------------------------
    Private Sub NavigateTo(strURL As String)

        Dim cmd As cBrowserCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        If (cmd IsNot Nothing) Then
            cmd.Invoke(strURL)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Allow the user to pick an output folder
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub PickOutputFolder()

        ' Use the central EwE 'directory open' structure to have the user select an output folder.
        ' In EwE, this is centrally done the cDirectoryOpenCommand command
        Dim cmd As cDirectoryOpenCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)
        ' Got command?
        If (cmd IsNot Nothing) Then
            ' #Yes: invoke command, providing the currently selected path
            cmd.Invoke(Me.m_tbxOutputFolder.Text, My.Resources.PROMPT_OUTPUTFOLDER)
            ' Did user complete command successfully?
            If (cmd.Result = Windows.Forms.DialogResult.OK) Then
                ' #Yes: Update local output folder
                Me.m_tbxOutputFolder.Text = cmd.Directory
                ' Update settings
                My.Settings.CustomFolder = cmd.Directory
                My.Settings.Save()
            End If
        End If

    End Sub

#End Region ' Internals

End Class