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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'





Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Common
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug



''' ---------------------------------------------------------------------------
''' <summary>
''' Main interface for the biodiversity indicators plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmMain

#Region " Variables "

    Private m_ecosimgraph As cEcosimGraphWrapper = Nothing
    Private m_ecospacemap As cEcospaceMapWrapper = Nothing
    Private m_mcgraphPath As cMCHistogramGraphWrapper = Nothing
    Private m_mcgraphSim As cMCGraphWrapper = Nothing

    Private m_plugin As cEwEEcologicalIndicatorsPlugin = Nothing

    Private m_settings As cIndicatorSettings = Nothing
    Private m_bInUpdate As Boolean = False

    Private WithEvents m_checker As cCheckboxHierarchy = Nothing
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmMain)()

#End Region ' Variables

#Region " Construction "

    Public Sub New(ByVal uic As cUIContext, pluginpoint As cEwEEcologicalIndicatorsPlugin)

        MyBase.New()
        Me.UIContext = uic

        Me.m_plugin = pluginpoint
        Me.m_settings = pluginpoint.m_settings

        Me.InitializeComponent()

        Me.m_ecosimgraph = New cEcosimGraphWrapper()
        Me.m_ecospacemap = New cEcospaceMapWrapper()
        Me.m_mcgraphSim = New cMCGraphWrapper()
        Me.m_mcgraphPath = New cMCHistogramGraphWrapper()

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

        If (Me.UIContext Is Nothing) Then Return

        Dim il As New ImageList()
        il.Images.Add(SharedResources.Warning)
        il.Images.Add(SharedResources.OK)
        Me.m_tcOutput.ImageList = il

        Dim ndSel As TreeNode = Nothing
        Me.m_bInUpdate = True

        Me.Text = My.Resources.DISPLAYNAME
        Me.TabText = My.Resources.DISPLAYNAME

        Me.m_llStatus.UIContext = Me.UIContext
        Me.m_pbStatus.BackgroundImage = ScientificInterfaceShared.My.Resources.Warning

        Me.m_legend.UIContext = Me.UIContext
        Me.m_legend.Colors = Me.StyleGuide.DefaultColors(50)

        Try
            Me.m_grid.Attach(Me.m_settings)
            Me.m_grid.UIContext = Me.UIContext
        Catch ex As Exception
            Debug.Assert(False, "Grid not able to attach")
        End Try

        Try
            Me.m_ecosimgraph.Attach(Me.UIContext, Me.m_graphSim, Me.m_settings, Me.m_plugin.m_lIndEcosim)
        Catch ex As Exception
            Debug.Assert(False, "Zed graph handler not able to attach")
        End Try

        Try
            Me.m_ecospacemap.Attach(Me.UIContext, Me.m_plugin.m_dtIndEcospace, Me.m_settings, Me.m_pbEcospaceMap, Me.m_plugin.m_indEcopath, Me.m_legend.Colors)
        Catch ex As Exception
            Debug.Assert(False, "Map stuff not able to attach")
        End Try

        Try
            Me.m_mcgraphPath.Attach(Me.UIContext, Me.m_graphMCpath, Me.m_settings, Me.m_plugin.m_lIndMCpath)
        Catch ex As Exception
            Debug.Assert(False, "Zed graph handler not able to attach")
        End Try

        Try
            Me.m_mcgraphSim.Attach(Me.UIContext, Me.m_graphMCsim, Me.m_settings, Me.m_plugin.m_lIndMCsim)
        Catch ex As Exception
            Debug.Assert(False, "Zed graph handler not able to attach")
        End Try

        Me.m_checker = New cCheckboxHierarchy(Nothing)
        Me.m_checker.ManageCheckedStates = False

        Try
            ' Populate tree view from indicator settings
            Me.m_tvIndicators.Nodes.Clear()
            For i As Integer = 0 To Me.m_settings.NumIndicatorGroups - 1
                ' Get indicator group from settings
                Dim grp As cIndicatorInfoGroup = Me.m_settings.IndicatorGroup(i)
                ' Create treenode for this group
                Dim tnGrp As TreeNode = Me.m_tvIndicators.Nodes.Add(grp.Name)
                ' Make sure the group is attached to its node
                tnGrp.Tag = grp
                ' Show description as tooltip text
                tnGrp.ToolTipText = grp.Description

                Me.m_checker.Add(tnGrp, Nothing)

                For j As Integer = 0 To grp.NumIndicators - 1
                    ' Get indicator from group
                    Dim ind As cIndicatorInfo = grp.Indicator(j)
                    ' Create treenode for indicator
                    Dim tnInd As TreeNode = tnGrp.Nodes.Add(ind.Name)
                    ' Make sure the indicator is attached to its node
                    tnInd.Tag = ind
                    ' Show description as tooltip text
                    tnInd.ToolTipText = ind.Description
                    ' Set enabled state
                    tnInd.Checked = ind.Enabled

                    Me.m_checker.Add(tnInd, tnGrp)

                Next

                If (ndSel Is Nothing) Then ndSel = tnGrp

            Next

            ' Expand all nodes in the tree
            Me.m_tvIndicators.ExpandAll()
            ' Select node
            Me.m_tvIndicators.SelectedNode = ndSel

        Catch ex As Exception
            ' Catch programming error
            Debug.Assert(False, ex.Message)
        End Try
        Me.m_checker.ManageCheckedStates = True

        ' Initialize content of controls
        Me.m_cbAutoSaveEcopath.Checked = My.Settings.AutoSaveEcopath
        Me.m_cbAutoSaveEcosim.Checked = My.Settings.AutoSaveEcosim
        Me.m_cbAutoSaveMCMC.Checked = My.Settings.AutoSaveMCMC
        Me.m_cbAutoSaveEcospaceCSV.Checked = My.Settings.AutoSaveEcospaceCSV
        Me.m_cbAutoSaveEcospaceASCII.Checked = My.Settings.AutoSaveEcospaceMaps
        Me.m_cbPlotAtEnd.Checked = My.Settings.PlotAtEnd
        Me.m_sliderNoBins.Value = My.Settings.NunHistBins

        If (My.Settings.SaveToDefault) Then
            Me.m_rbDefault.Checked = True
        Else
            Me.m_rbCustom.Checked = True
        End If
        Me.m_tbxDefaultLocation.Text = Me.m_plugin.DefaultFolder
        Me.m_tbxOutputFolder.Text = My.Settings.CustomFolder

        Me.Icon = My.Resources.BioDiversityPluginIcon
        Me.m_tsbnEcospaceSaveImage.Image = SharedResources.saveHS

        ' Start listening to core run state changes
        AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged
        AddHandler Me.m_settings.OnSettingsChanged, AddressOf OnSettingsChanged

        ' Start listening to Ecopath, Ecosim, Ecospace and external messages (responses are handled in OnCoreMessage)
        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecosim, eCoreComponentType.Ecospace, eCoreComponentType.Core}

        Me.m_bInUpdate = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form is officially closed; preserve what needs preserving and clean up.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        ' Stop listening to core run state changes
        RemoveHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreStateChanged
        RemoveHandler Me.m_settings.OnSettingsChanged, AddressOf OnSettingsChanged

        ' Cleanup 
        Me.m_grid.Detach()
        Me.m_ecosimgraph.Detach()
        Me.m_ecospacemap.Detach()

        Me.Icon.Dispose()

        ' Stop listening to any messages
        Me.CoreComponents = Nothing

        ' Done
        MyBase.OnFormClosed(e)

    End Sub

    Private Sub OnSettingsChanged(sender As Object, e As EventArgs)
        If (Me.m_bInUpdate) Then Return
        Me.UpdateControls()
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
            Me.m_tbxDefaultLocation.Text = Me.m_plugin.DefaultFolder
            Me.m_cbAutoSaveEcopath.Checked = My.Settings.AutoSaveEcopath
            Me.m_cbAutoSaveEcosim.Checked = My.Settings.AutoSaveEcosim
            Me.m_cbAutoSaveMCMC.Checked = My.Settings.AutoSaveMCMC
            Me.m_cbAutoSaveEcospaceCSV.Checked = My.Settings.AutoSaveEcospaceCSV
            Me.m_cbAutoSaveEcospaceASCII.Checked = My.Settings.AutoSaveEcospaceMaps
            Me.m_bInUpdate = False
        End If

        ' Update controls to reflect any core state changes
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to prevent the form from locking up while running.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides ReadOnly Property IsRunForm As Boolean
        Get
            Return True
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="UpdateControls"/>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub UpdateControls()

        If (Me.UIContext Is Nothing) Then Return

        Dim csm As cCoreStateMonitor = Me.UIContext.Core.StateMonitor
        Dim bCanSave As Boolean = False
        Dim bHasTaxa As Boolean = (Me.UIContext.Core.nTaxon > 0)
        Dim bIsRunning As Boolean = csm.IsBusy

        Me.m_cbRunWithEcopath.Checked = Me.m_settings.RunWithEcopath
        Me.m_cbRunWithEcosim.Checked = Me.m_settings.RunWithEcosim
        Me.m_cbRunWithEcospace.Checked = Me.m_settings.RunWithEcospace
        Me.m_cbRunWithMC.Checked = Me.m_settings.RunWithMonteCarlo
        Me.m_cbEcospaceAnnualOnly.Checked = Me.m_settings.EcospaceAnnualOnly

        Select Case Me.SelectedTabComponent
            Case cEwEEcologicalIndicatorsPlugin.eComponentType.Any
                bCanSave = False
            Case cEwEEcologicalIndicatorsPlugin.eComponentType.Ecopath
                bCanSave = Me.m_settings.RunWithEcopath And csm.HasEcopathRan
            Case cEwEEcologicalIndicatorsPlugin.eComponentType.Ecosim
                bCanSave = Me.m_settings.RunWithEcosim And csm.HasEcosimRan
            Case cEwEEcologicalIndicatorsPlugin.eComponentType.Ecospace
                bCanSave = Me.m_settings.RunWithEcospace And csm.HasEcospaceRan
            Case cEwEEcologicalIndicatorsPlugin.eComponentType.MonteCarlo
                ' Unfortunately this cannot be asked from the state monitor. 
                ' Perhaps we should add this; I added a ToDo to the Core State Monitor file.
                bCanSave = Me.m_settings.RunWithMonteCarlo And Me.m_plugin.HasMonteCarloRan
        End Select

        Me.m_llStatus.Visible = Not bHasTaxa
        Me.m_pbStatus.Visible = Not bHasTaxa

        Me.m_cbAutoSaveEcopath.Enabled = Not bIsRunning
        Me.m_cbAutoSaveEcosim.Enabled = Not bIsRunning
        Me.m_cbAutoSaveMCMC.Enabled = Not bIsRunning
        Me.m_cbAutoSaveEcospaceCSV.Enabled = Not bIsRunning
        Me.m_cbAutoSaveEcospaceASCII.Enabled = Not bIsRunning
        Me.m_cbRunWithEcopath.Enabled = Not bIsRunning
        Me.m_cbRunWithEcosim.Enabled = Not bIsRunning
        Me.m_cbRunWithEcospace.Enabled = Not bIsRunning
        Me.m_cbRunWithMC.Enabled = Not bIsRunning
        Me.m_btnChangeDefault.Enabled = Not bIsRunning
        Me.m_btnChooseFolder.Enabled = Not bIsRunning
        Me.m_cbEcospaceAnnualOnly.Enabled = Not bIsRunning
        Me.m_cbPlotAtEnd.Enabled = Not bIsRunning
        Me.m_rbCustom.Enabled = Not bIsRunning
        Me.m_rbDefault.Enabled = Not bIsRunning
        Me.m_tbxOutputFolder.Enabled = Not bIsRunning

        Me.m_btnSaveResults.Enabled = bCanSave
        Me.m_tpEcopath.ImageIndex = If(Me.m_settings.RunWithEcopath, 1, 0)
        Me.m_tpEcosim.ImageIndex = If(Me.m_settings.RunWithEcosim, 1, 0)
        Me.m_tpEcospace.ImageIndex = If(Me.m_settings.RunWithEcospace, 1, 0)
        Me.m_tpMCpath.ImageIndex = If(Me.m_settings.RunWithMonteCarlo, 1, 0)
        Me.m_tpMCsim.ImageIndex = If(Me.m_settings.RunWithMonteCarlo, 1, 0)

        Me.m_tbxHistNoBins.Text = CStr(Me.m_mcgraphPath.NumBins(Nothing))

    End Sub

#End Region ' Overrides

#Region " Events "

    Private Sub OnTreeNodeSelected(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvIndicators.AfterSelect

        Try
            ' Repopulate all indicators in response to a treenode selection change
            Me.UpdateIndicators(cEwEEcologicalIndicatorsPlugin.eComponentType.Any)
        Catch ex As Exception
            ' Whoah
        End Try

    End Sub

    Private Sub OnTreeNodeCheckChanged(sender As Object, e As TreeViewEventArgs) _
        Handles m_tvIndicators.AfterCheck
        m_bTreenodeProcessingPending = True
        BeginInvoke(New MethodInvoker(AddressOf ProcessTreenodeStates))
    End Sub

    Private Sub OnSaveToCSV(sender As System.Object, e As System.EventArgs) _
        Handles m_btnSaveResults.Click

        ' Start CSV save process
        cApplicationStatusNotifier.StartProgress(Me.UIContext.Core)
        Try
            ' Save selected component (path, sim, space or ...) to CSV
            Me.m_plugin.SaveToCSVManual(Me.SelectedTabComponent())
        Catch ex As Exception

        End Try

        ' End CSV save process
        cApplicationStatusNotifier.EndProgress(Me.UIContext.Core)

    End Sub

    Private Sub OnAutoSaveEcopathCSVChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbAutoSaveEcopath.CheckedChanged
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveEcopath = Me.m_cbAutoSaveEcopath.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnAutoSaveEcosimCSVChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbAutoSaveEcosim.CheckedChanged
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveEcosim = Me.m_cbAutoSaveEcosim.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnAutoSaveMCMCCSVChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbAutoSaveMCMC.CheckedChanged
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveMCMC = Me.m_cbAutoSaveMCMC.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnAutoSaveEcospaceCSVChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbAutoSaveEcospaceCSV.CheckedChanged
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveEcospaceCSV = Me.m_cbAutoSaveEcospaceCSV.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnAutoSaveEcospaceMapsChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbAutoSaveEcospaceASCII.CheckedChanged
        If Me.m_bInUpdate Then Return
        My.Settings.AutoSaveEcospaceMaps = Me.m_cbAutoSaveEcospaceASCII.Checked
        My.Settings.Save()
        Me.Core.OnSettingsChanged()
    End Sub

    Private Sub OnRunWithEcopathChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbRunWithEcopath.CheckedChanged

        ' User toggled RunWithEcopath checkbox; update settings
        If Me.m_bInUpdate Then Return
        Me.m_settings.RunWithEcopath = Me.m_cbRunWithEcopath.Checked

        ' No longer run with Ecopath?
        If (Not Me.m_settings.RunWithEcopath) Then
            ' #Yes: clear results
            Me.m_plugin.ClearEcopathIndicators()
        Else
            Me.m_plugin.TryRecomputeEcopathIndicators()
        End If

    End Sub

    Private Sub OnRunWithEcosimChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbRunWithEcosim.CheckedChanged

        ' User toggled RunWithEcosim checkbox; update settings
        If Me.m_bInUpdate Then Return
        Me.m_settings.RunWithEcosim = Me.m_cbRunWithEcosim.Checked

        ' No longer run with Ecosim?
        ' - This should be handled by the plugin itself based on settings changes
        If (Not Me.m_settings.RunWithEcosim) Then
            ' #Yes: clear results
            Me.m_plugin.ClearEcosimIndicators()
        End If

    End Sub

    Private Sub OnRunWithEcospaceChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbRunWithEcospace.CheckedChanged

        ' User toggled RunWithEcospace checkbox; update settings
        If Me.m_bInUpdate Then Return
        Me.m_settings.RunWithEcospace = Me.m_cbRunWithEcospace.Checked

        ' No longer run with Ecospace?
        If (Not Me.m_settings.RunWithEcospace) Then
            ' #Yes: clear results
            Me.m_plugin.ClearEcospaceIndicators()
        End If

    End Sub

    Private Sub OnRunWithMCChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbRunWithMC.CheckedChanged

        ' User toggled RunWithMC checkbox; update settings
        If Me.m_bInUpdate Then Return
        Me.m_settings.RunWithMonteCarlo = Me.m_cbRunWithMC.Checked

        ' No longer run with Ecosim?
        If (Not Me.m_settings.RunWithMonteCarlo) Then
            ' #Yes: clear results
            Me.m_plugin.ClearMCIndicators()
        End If

    End Sub

    Private Sub OnSpaceAnnualOnlyChanged(sender As Object, e As System.EventArgs) _
        Handles m_cbEcospaceAnnualOnly.CheckedChanged

        ' User toggled EcospaceAnnualOnly checkbox; update settings
        If Me.m_bInUpdate Then Return
        Me.m_settings.EcospaceAnnualOnly = Me.m_cbEcospaceAnnualOnly.Checked

    End Sub

    Private Sub OnPlotAtEndCheckedChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbPlotAtEnd.CheckedChanged

        If Me.m_bInUpdate Then Return
        My.Settings.PlotAtEnd = m_cbPlotAtEnd.Checked
        My.Settings.Save()

    End Sub

    Private Sub OnTabSelected(sender As Object, e As System.EventArgs) _
        Handles m_tcOutput.SelectedIndexChanged

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

    Private Sub OnChangeDefaultFolder(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChangeDefault.Click

        Try
            Dim cmd As cShowOptionsCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cShowOptionsCommand.cCOMMAND_NAME), cShowOptionsCommand)
            If cmd IsNot Nothing Then
                cmd.Invoke(ScientificInterfaceShared.Definitions.eApplicationOptionTypes.Autosave)
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnChooseOutputFolder(sender As System.Object, e As System.EventArgs) _
        Handles m_btnChooseFolder.Click
        ' User wants to browse for an output folder. Let's be nice.
        Me.PickOutputFolder()
    End Sub

    Private Sub OnVisitCSIC(sender As System.Object, e As System.EventArgs) _
        Handles m_pbCSIC.Click
        ' User wants to visit CSIC
        Me.OpenLink("http://www.csic.es/")
    End Sub

    Private Sub OnVisitIRD(sender As System.Object, e As System.EventArgs) _
        Handles m_pbIRD.Click
        ' User wants to visit IRD
        Me.OpenLink("http://www.ird.fr/")
    End Sub

    Private Sub OnVisitEII(sender As System.Object, e As System.EventArgs) _
        Handles m_pbEII.Click
        ' User wants to visit EII
        Me.OpenLink("http://www.ecopathinternational.org")
    End Sub

    Private Sub OnCoreStateChanged(csm As cCoreStateMonitor)
        Try
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSliderNoBins(sender As Object, e As EventArgs) Handles m_sliderNoBins.ValueChanged
        Try
            If (Me.m_settings Is Nothing) Then Return
            If (Me.m_mcgraphPath Is Nothing) Then Return

            Me.m_mcgraphPath.NumBins(Me.m_plugin.m_indEcopath) = CInt(Me.m_sliderNoBins.Value)

            Me.UpdateControls()
            My.Settings.NunHistBins = Me.m_mcgraphPath.NumBins(Nothing)
            My.Settings.Save()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnEcospaceSaveMapImage(sender As Object, e As EventArgs) Handles m_tsbnEcospaceSaveImage.Click

        ' ToDo: globalize this
        Dim strName As String = Path.Combine(Me.m_plugin.OutputFolder(cEwEEcologicalIndicatorsPlugin.eComponentType.Ecospace), Me.m_ecospacemap.MapFileName)
        Dim sfd As SaveFileDialog = cEwEFileDialogHelper.SaveFileDialog("Save indicators map", strName, SharedResources.FILEFILTER_IMAGE)
        If sfd.ShowDialog() = DialogResult.OK Then
            Me.m_ecospacemap.SaveImage(sfd.FileName)
        End If

    End Sub

#End Region ' Events

#Region " Public methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of controls that display indicators.
    ''' </summary>
    ''' <param name="component">The component that needs updating.</param>
    ''' -----------------------------------------------------------------------
    Friend Sub UpdateIndicators(component As cEwEEcologicalIndicatorsPlugin.eComponentType)

        ' Optimization: only plot when done
        If (Me.Core.StateMonitor.IsSearching) And (My.Settings.PlotAtEnd) Then Return

        ' Optimization: only update the component that was changed
        If (component = cEwEEcologicalIndicatorsPlugin.eComponentType.Any Or component = cEwEEcologicalIndicatorsPlugin.eComponentType.Ecopath) Then
            Me.m_grid.RefreshContent(Me.m_plugin.m_indEcopath)
        End If
        If (component = cEwEEcologicalIndicatorsPlugin.eComponentType.Any Or component = cEwEEcologicalIndicatorsPlugin.eComponentType.Ecosim) Then
            Me.m_ecosimgraph.RefreshContent(Me.GetSelectedIndicator(), Me.GetSelectedIndicatorGroup())
        End If
        If (component = cEwEEcologicalIndicatorsPlugin.eComponentType.Any Or component = cEwEEcologicalIndicatorsPlugin.eComponentType.Ecospace) Then
            Me.m_ecospacemap.RefreshContent(Me.GetSelectedIndicator(), Me.GetSelectedIndicatorGroup())
        End If
        If (component = cEwEEcologicalIndicatorsPlugin.eComponentType.Any Or component = cEwEEcologicalIndicatorsPlugin.eComponentType.MonteCarlo) Then
            Me.m_mcgraphSim.RefreshContent(Me.GetSelectedIndicator(), Me.GetSelectedIndicatorGroup())
            Me.m_mcgraphPath.RefreshContent(Me.GetSelectedIndicator(), Me.GetSelectedIndicatorGroup(), Me.m_plugin.m_indEcopath)
        End If

        ' Update state specific controls as a precaution
        Me.UpdateControls()

    End Sub

    Private m_bTreenodeProcessingPending As Boolean = False

    Friend Sub ProcessTreenodeStates()
        If Not m_bTreenodeProcessingPending Then Return
        m_bTreenodeProcessingPending = False
        For Each tnGroup As TreeNode In Me.m_tvIndicators.Nodes
            For Each tnInd As TreeNode In tnGroup.Nodes
                Dim info As cIndicatorInfo = DirectCast(tnInd.Tag, cIndicatorInfo)
                info.Enabled = tnInd.Checked
            Next
        Next
        Me.UpdateIndicators(cEwEEcologicalIndicatorsPlugin.eComponentType.Any)
    End Sub

#End Region ' Public methods

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator group selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetSelectedIndicatorGroup() As cIndicatorInfoGroup

        Dim nd As TreeNode = Me.m_tvIndicators.SelectedNode
        If (nd Is Nothing) Then Return Nothing

        If TypeOf nd.Tag Is cIndicatorInfo Then
            nd = nd.Parent
        End If

        Return DirectCast(nd.Tag, cIndicatorInfoGroup)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetSelectedIndicator() As cIndicatorInfo

        Dim nd As TreeNode = Me.m_tvIndicators.SelectedNode
        If (nd Is Nothing) Then Return Nothing

        If TypeOf nd.Tag Is cIndicatorInfo Then
            Return DirectCast(nd.Tag, cIndicatorInfo)
        End If

        Return Nothing

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the indicator selected in the indicator navigation tree.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function SelectedTabComponent() As cEwEEcologicalIndicatorsPlugin.eComponentType
        Select Case Me.m_tcOutput.SelectedIndex
            Case 1 : Return cEwEEcologicalIndicatorsPlugin.eComponentType.Ecopath
            Case 2 : Return cEwEEcologicalIndicatorsPlugin.eComponentType.Ecosim
            Case 3 : Return cEwEEcologicalIndicatorsPlugin.eComponentType.Ecospace
            Case 4, 5 : Return cEwEEcologicalIndicatorsPlugin.eComponentType.MonteCarlo
        End Select
        Return cEwEEcologicalIndicatorsPlugin.eComponentType.Any
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Open an external link.
    ''' </summary>
    ''' <param name="strURL">The link to navigate to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OpenLink(strURL As String)

        Try
            Dim cmd As cBrowserCommand = DirectCast(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            If (cmd IsNot Nothing) Then
                cmd.Invoke(strURL)
            End If
        Catch ex As Exception
            m_logger.LogError(ex, "cEwEBioDivPlugin::NavigateTo(" & strURL & ")")
        End Try

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
            If (cmd.Result = System.Windows.Forms.DialogResult.OK) Then
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