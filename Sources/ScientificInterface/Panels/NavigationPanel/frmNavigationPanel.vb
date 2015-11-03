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
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterface.Ecopath.Controls
Imports ScientificInterface.Ecopath.Input
Imports ScientificInterface.Ecopath.Output
Imports ScientificInterface.Ecopath.Tools
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Integration

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Navigation tree panel; contains the navigation structure that provides uniform
''' access to the screens of the EwE user interface. All interaction with the
''' tree is standardized and handled in this class.
''' </summary>
''' <remarks>
''' <para>The Navigation Panel will not actually create or highlight the GUI items 
''' that it provides access to. Instead, the panel will outsource this functionality 
''' via the central <see cref="ScientificInterfaceShared.Commands.cCommandHandler">CommandHandler</see> 
''' and the <see cref="ScientificInterfaceShared.Commands.cNavigationCommand">Navigation command</see>.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class frmNavigationPanel

    Private m_uic As cUIContext = Nothing
    Private m_nodeController As cTreeViewNodeController = Nothing
    Private m_pluginManager As cPluginManager = Nothing
    Private m_ntPluginHandler As cPluginNavTreeHandler = Nothing
    Private m_tnSelected As TreeNode = Nothing

#Region " Construction / destruction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor; initializes a new instance of the navigation panel.
    ''' </summary>
    ''' <param name="uic">The UI contezt to connect to.</param>
    ''' <param name="pluginManager">The plug-in manager to obtain tree 
    ''' extensions for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext, _
                   ByVal pluginManager As EwEPlugin.cPluginManager)

        ' Sanity check
        Debug.Assert(uic IsNot Nothing)

        ' Store refs
        Me.m_uic = uic
        Me.m_pluginManager = pluginManager

        Try
            ' Hit 'em, Jimmy
            Me.InitializeComponent()
        Catch ex As Exception

        End Try

    End Sub

    Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)

        If bDisposing Then
            If components IsNot Nothing Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(bDisposing)

    End Sub

#End Region ' Construction / destruction

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.Icon = Icon.FromHandle(ScientificInterfaceShared.My.Resources.NavHS.GetHicon)

        ' Put all the list here
        Me.m_nodeController = New cTreeViewNodeController()
        Me.m_nodeController.Attach(Me.m_uic, Me.m_tvNavigation)

#If Not Debug Then
        Me.RemoveNode("ndMSEBatch")
        Console.writeline("Removed MSE Batch node in release mode")
         Me.RemoveNode("ndRefMSY")
        Console.writeline("Removed MSE MSY node in release mode")
#End If

        With Me.m_nodeController

            'Basic Parameters
            .Add("ndModelParameters", eCoreExecutionState.EcopathLoaded, GetType(frmModelParameters), "Model description.htm")
            .Add("ndBasicInput", eCoreExecutionState.EcopathLoaded, GetType(frmBasicInput), "Basic input.htm", True)
            .Add("ndDietComposition", eCoreExecutionState.EcopathLoaded, GetType(frmDietComp), "Diet composition.htm")
            .Add("ndDetritusFate", eCoreExecutionState.EcopathLoaded, GetType(gridDetritusFate), "Detritus fate.htm")
            .Add("ndOtherProduction", eCoreExecutionState.EcopathLoaded, GetType(gridOtherProduction), "Other production.htm")
            .Add("ndDefFleets", eCoreExecutionState.EcopathLoaded, GetType(frmFisheryBasicInput), "Definition of fleets.htm")
            .Add("ndLandings", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryInputLandings), "Landings.htm")
            .Add("ndDiscards", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryInputDiscards), "Discards.htm")
            .Add("ndDiscardFate", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryInputDiscardFate), "Discard fate.htm")
            .Add("ndDiscardMortRate", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryInputDiscardMort), "") ' ToDo: connect to help
            .Add("ndOffVesselPrice", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryOffVesselValue), "Market price.htm")
            .Add("ndNonMarketPrice", eCoreExecutionState.EcopathLoaded, GetType(gridFisheryInputNonMarketPrice), "Non market price.htm")
            .Add("ndPedigree", eCoreExecutionState.EcopathLoaded, GetType(frmPedigree), "pedigree.htm")
            .Add("ndEcopathInputTraits", eCoreExecutionState.EcopathLoaded, GetType(frmTaxonInput), "") ' ToDo: connect to help

            ' Ecopath Output
            .Add("ndBasicEstimates", eCoreExecutionState.EcopathCompleted, GetType(gridBasicEstimates), "Basic estimates.htm")
            .Add("ndKeyIndices", eCoreExecutionState.EcopathCompleted, GetType(gridKeyIndices), "Key indices.htm")
            .Add("ndMortCoef", eCoreExecutionState.EcopathCompleted, GetType(gridMortalityCoefficients), "Mortalities.htm")
            .Add("ndPredMort", eCoreExecutionState.EcopathCompleted, GetType(gridMortalityPredation), "Predation mortality.htm")
            .Add("ndFleetFishingMortality", eCoreExecutionState.EcopathCompleted, GetType(gridFleetFishingMortality), "")
            .Add("ndConsumption", eCoreExecutionState.EcopathCompleted, GetType(gridConsumption), "Consumption.htm")
            .Add("ndRespiration", eCoreExecutionState.EcopathCompleted, GetType(gridRespiration), "Respiration.htm")
            .Add("ndPreyOverlap", eCoreExecutionState.EcopathCompleted, GetType(gridNicheOverlapPrey), "Niche overlap.htm")
            .Add("ndPredatorOverlap", eCoreExecutionState.EcopathCompleted, GetType(gridNicheOverlapPredator), "Niche overlap.htm")
            .Add("ndElectivity", eCoreExecutionState.EcopathCompleted, GetType(gridElectivity), "Electivity.htm")
            .Add("ndSearchRates", eCoreExecutionState.EcopathCompleted, GetType(gridSearchRates), "Search rates.htm")
            .Add("ndQuantity", eCoreExecutionState.EcopathCompleted, GetType(gridFisheryOutputQuantity), "Fishery (Ecopath parameterization).htm")
            .Add("ndValue", eCoreExecutionState.EcopathCompleted, GetType(gridFisheryOutputValue), "Fishery (Ecopath parameterization).htm")
            .Add("ndFlowDiagram", eCoreExecutionState.EcopathCompleted, GetType(FlowDiagram.frmFlowDiagram), "Flow diagram.htm")
            .Add("ndEcopathStats", eCoreExecutionState.EcopathCompleted, GetType(gridEcopathStatistics), "")
            .Add("ndNichePredPreyPlot", eCoreExecutionState.EcopathCompleted, GetType(frmNichePredPreyPlot), "")
            ' Network Analysis PlugIn: "Network%20analysis%20indices%20in.htm"

            ' PSD Input
            .Add("ndGrowthParameters", eCoreExecutionState.EcopathLoaded, GetType(gridGrowthParameters), "") ' ToDo: connect to help
            .Add("ndRunPSD", eCoreExecutionState.EcopathLoaded, GetType(RunPSD), "") ' ToDo: connect to help

            ' PSD Output
            .Add("ndGrowthEstimates", eCoreExecutionState.PSDCompleted, GetType(gridPSDGrowthEstimates), "") ' ToDo: connect to help
            .Add("ndPSDContributionPlot", eCoreExecutionState.PSDCompleted, GetType(PSDContributionPlot), "") ' ToDo: connect to help
            .Add("ndPSDContributionResult", eCoreExecutionState.PSDCompleted, GetType(gridPSDContributionResult), "") ' ToDo: connect to help
            .Add("ndPSDPlotByGroup", eCoreExecutionState.PSDCompleted, GetType(PSDPlotByGroup), "") ' ToDo: connect to help
            .Add("ndSizeWeightPlot", eCoreExecutionState.PSDCompleted, GetType(SizeWeightPlot), "") ' ToDo: connect to help

            ' Ecosim Input
            .Add("ndEcosimParameters", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmEcosimParameters), "Ecosim parameters.htm")
            .Add("ndGroupInfo", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.gridEcosimGroupInput), "Group info.htm")
            .Add("ndVulnerabilities", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmVulnerabilities), "Vulnerabilities flow control.htm")
            .Add("ndTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTimeSeries), "Time series.htm")
            .Add("ndMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmMediationFunction), "Mediation.htm")
            .Add("ndApplyMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedConsumer), "Apply mediation.htm")
            .Add("ndApplyMediationPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedPP), "Apply mediation.htm")
            .Add("ndApplyMediationDetritus", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedDetritus)) ' ToDo: connect to help
            .Add("ndForcingFunction", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmForcingFunction), "Forcing function.htm")
            .Add("ndApplyFFConsumer", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFConsumer), "Apply forcing function consumer.htm")
            .Add("ndApplyFFPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFPrimaryProducer), "Apply forcing function primary.htm")
            .Add("ndApplyFFDetritus", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFDetritus)) ' ToDo: connect to help
            .Add("ndEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmEggProduction), "Egg production.htm")
            .Add("ndApplyEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.ApplyEP), "Apply egg production.htm")
            .Add("ndFishingEffort", eCoreExecutionState.EcosimLoaded, GetType(frmFishingEffort)) ' ToDo: connect to help
            .Add("ndFleetQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.gridQuotaShare)) ' ToDo: connect to help
            '.Add("ndSpeciesQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTargetFishingMortalityPolicy)) ' ToDo: connect to help
            .Add("ndFleetSizeDynamics", eCoreExecutionState.EcosimLoaded, GetType(gridEcosimFleetSizeDynamics), "Fleet size dynamics.htm")
            .Add("ndFishingMortality", eCoreExecutionState.EcosimLoaded, GetType(frmFishingMortality)) ' ToDo: connect to help
            .Add("ndPriceElasticity", eCoreExecutionState.EcosimLoaded, GetType(frmPriceElasticity)) ' ToDo: connect to help
            .Add("ndApplyPriceElasticity", eCoreExecutionState.EcosimLoaded, GetType(frmApplyPriceElasticy)) ' ToDo: connect to help

            ' Ecosim Output
            .Add("ndRunEcosim", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmRunEcosim), "Run Ecosim.htm")
            .Add("ndEcosimPlots", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.frmEcosimOutputPlots), "Ecosim plot.htm")
            .Add("ndEcosimResults", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.frmEcosimResults), "Ecosim results.htm")
            .Add("ndEcosimAllFits", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.frmShowAllFits), "Ecosim results.htm")
            .Add("ndSRPlot", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmStockRecruitmentPlot), "Stock recruitment S R plot.htm")
            .Add("ndSuitabilityPlot", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.SuitabilityPlot)) ' ToDo: connect to help
            .Add("ndFDSliderPlugin", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.frmEcosimFD))

            ' Ecosim Tools
            .Add("ndMCRun", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.MCRun), "Monte Carlo runs.htm") ' ToDo: connect to help
            .Add("ndFishingPolicySearch", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFishingPolicySearch), "Fishing policy search.htm")
            .Add("ndFitToTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFitToTimeSeries), "Fit to time series.htm")
            .Add("ndMSY", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmMSY), "") ' ToDo: connect to help

            ' Ecospace
            .Add("ndDispersal", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridEcospaceDispersal), "Dispersal.htm")
            .Add("ndEcospaceParameters", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmEcospaceParameters), "Ecospace parameters.htm")
            .Add("ndBasemap", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.Basemap.frmEcospaceMap), "Basemap.htm") ' ToDo: connect to help
            .Add("ndEcospaceFishery", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridEcospaceFishery), "Ecospace Fishery.htm")
            .Add("ndEcospaceMPA", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmMPAs))
            .Add("ndEcospaceScenario", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.dlgEcospaceScenario)) ' ToDo: connect to help
            .Add("ndRunEcospace", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmRunEcospace), "Run Ecospace.htm")
            .Add("ndAdvection", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.Advection.frmAdvection), "")
            .Add("ndMPAOptimizations", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmMPAOptimizations), "EcoSeed.htm")
            .Add("ndEcospaceExtData", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmSpatialTimeSeries), "")

            ' Ecospace output
            .Add("ndEcospaceResults", eCoreExecutionState.EcospaceCompleted, GetType(Ecospace.frmEcospaceResults), "") ' ToDo: connect to help

            ' ToDo_JS: Link to yet-to-be-written help text
            .Add("ndEcoTracer_Pram", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerParameters), "") ' ToDo: connect to help
            .Add("ndEcoTracer_Input", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerInput), "") ' ToDo: connect to help
            .Add("ndEcoTracer_Output", eCoreExecutionState.EcotracerLoaded, GetType(frmEcotracerOutput), "") ' ToDo: connect to help

            'MSE
            .Add("ndOptions", eCoreExecutionState.EcosimLoaded, GetType(frmMSEOptions), "") ' ToDo: connect to help
            ' .Add("ndControlType", eCoreExecutionState.EcosimLoaded, GetType(gridRegulatoryOptions), "") ' ToDo: connect to help
            .Add("ndRefFixedEscape", eCoreExecutionState.EcosimLoaded, GetType(gridFixedEscapement), "") ' ToDo: connect to help
            .Add("ndQuotaShare", eCoreExecutionState.EcosimLoaded, GetType(frmQuotaShare), "") ' ToDo: connect to help
            .Add("ndRegFishingMort", eCoreExecutionState.EcosimLoaded, GetType(frmTargetFishingMortalityPolicy), "") ' ToDo: connect to help
            .Add("ndAssessGroup", eCoreExecutionState.EcosimLoaded, GetType(frmMSEAssessGroups), "") ' ToDo: connect to help
            .Add("ndAssessFleet", eCoreExecutionState.EcosimLoaded, GetType(frmMSEAssessFleets), "") ' ToDo: connect to help
            .Add("ndMSERecruitment", eCoreExecutionState.EcosimLoaded, GetType(frmMSERecruitment), "") ' ToDo: connect to help
            'jb march-8-2010 removed Group objectives and Objective weights as they are not being used by the MSE
            '.Add("ndEfTrackObjectives", eCoreExecutionState.EcosimLoaded, GetType(gridMSEOjectiveWeights), "") ' ToDo: connect to help
            '.Add("ndEfTrackEcoObjectives", eCoreExecutionState.EcosimLoaded, GetType(gridMSEGroupObjectives), "") ' ToDo: connect to help
            .Add("ndRefMSY", eCoreExecutionState.EcosimLoaded, GetType(frmMSY), "") ' ToDo: connect to help
            .Add("ndRefBiomass", eCoreExecutionState.EcosimLoaded, GetType(frmGroupRefLevels), "") ' ToDo: connect to help
            .Add("ndRefCatch", eCoreExecutionState.EcosimLoaded, GetType(gridFleetRefLevels), "") ' ToDo: connect to help
            .Add("ndEfTrackFleetWeights", eCoreExecutionState.EcosimLoaded, GetType(gridFishingWeights), "") ' ToDo: connect to help
            .Add("ndMSERun", eCoreExecutionState.EcosimLoaded, GetType(frmMSE), "") ' ToDo: connect to help
            .Add("ndMSEResults", eCoreExecutionState.EcosimLoaded, GetType(frmMSEResults), "") ' ToDo: connect to help
            .Add("ndMSEPlots", eCoreExecutionState.EcosimLoaded, GetType(frmMSEPlots), "") ' ToDo: connect to help

            'MSE Batch
            'Not ready for release yet
            '.Add("ndRunBatch", eCoreExecutionState.EcosimLoaded, GetType(frmMSERunBatch), "") ' ToDo: connect to help
            '.Add("ndMSEBatchTFM", eCoreExecutionState.EcosimLoaded, GetType(frmMSEBatchTFM), "") ' ToDo: connect to help
            '.Add("ndMSEBatchFixedF", eCoreExecutionState.EcosimLoaded, GetType(frmMSEBatchFixedF), "") ' ToDo: connect to help
            ''jb Form not done yet
            ' '' .Add("ndMSEBatchTAC", eCoreExecutionState.EcosimLoaded, GetType(frmMSEBatchTAC), "") ' ToDo: connect to help
            '.Add("ndMSEBatchParameters", eCoreExecutionState.EcosimLoaded, GetType(frmMSEBatchParameters), "") ' ToDo: connect to help

            'Ecospace habitat capacity
            .Add("ndHabCap", eCoreExecutionState.EcospaceLoaded, GetType(frmForagingResponse), "") ' ToDo: connect to help
            .Add("ndHabCapModel", eCoreExecutionState.EcospaceLoaded, GetType(frmCapacityCalcType), "") ' ToDo: connect to help
            '.Add("ndHabCapDrivers", eCoreExecutionState.EcospaceLoaded, GetType(frmCapacityDrivers), "") ' ToDo: connect to help
            .Add("ndHabCapApply", eCoreExecutionState.EcospaceLoaded, GetType(frmApplyCapacity), "") ' ToDo: connect to help
            .Add("ndHabitatPrefs", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridHabitatPreference), "Assign habitats.htm")

        End With

        ' JS 19Mar2010: now why was this necessary?
        If (Me.m_tvNavigation.SelectedNode IsNot Nothing) Then
            If cSystemUtils.IsRightToLeft Then
                Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
            Else
                Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft
            End If
        End If

        ' Integrate plug-ins
        If Me.m_pluginManager IsNot Nothing Then
            Me.m_ntPluginHandler = New cPluginNavTreeHandler(Me.m_tvNavigation, Me.m_pluginManager, Me.m_uic.CommandHandler)
        End If

        AddHandler Me.m_uic.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_uic.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

        Me.Icon.Dispose()

        Me.m_nodeController.Detach()
        Me.m_nodeController = Nothing

        Me.m_ntPluginHandler.Dispose()
        Me.m_ntPluginHandler = Nothing

        Me.m_pluginManager = Nothing
        Me.m_uic = Nothing

        MyBase.OnFormClosed(e)
    End Sub

    Public Overrides Function PanelType() As frmEwEDockContent.ePanelType
        Return ePanelType.SystemPanel
    End Function

#End Region ' Form overrides

#Region " Properties "

    ''' <summary>
    ''' Get or set the current selected node name in the nav structure
    ''' </summary>
    ''' <param name="bUseDefault">States whether default node may be considered.</param>
    ''' <remarks>In order to highlight the selection</remarks>
    Public Property SelectedNodeName(Optional ByVal bUseDefault As Boolean = False) As String

        Get
            If Me.m_tvNavigation.SelectedNode Is Nothing Then Return ""
            Return Me.m_tvNavigation.SelectedNode.Name
        End Get

        Set(ByVal value As String)

            Dim bSelected As Boolean = False
            Dim nd As TreeNode = Nothing

            If Me.m_tvNavigation.Nodes.Count = 0 Then Return

            If (String.IsNullOrEmpty(value) And bUseDefault) Then
                value = Me.m_nodeController.DefaultNodeName
            End If

            ' Try to find node to select
            If Not String.IsNullOrEmpty(value) Then
                nd = Me.FindNode(Me.m_tvNavigation.Nodes, value)
            End If

            If Not Object.ReferenceEquals(nd, Me.m_tvNavigation.SelectedNode) Then
                Me.m_tvNavigation.SelectedNode = nd
            End If

        End Set

    End Property

    Public Sub Reset()
        For Each node As TreeNode In Me.m_tvNavigation.Nodes
            node.Collapse()
        Next
    End Sub

#End Region ' Properties

#Region " Event handlers "

    Private Sub OnCoreEcecutionStateChanged(ByVal csm As cCoreStateMonitor)
        With Me.m_tvNavigation
            If csm.HasEcopathLoaded Then
                .Visible = True
                .Dock = DockStyle.Fill
            Else
                .Visible = False
                .Dock = DockStyle.None
                .Width = 0
                .Height = 0
            End If
        End With
    End Sub

#End Region ' Event handlers

#Region " Internals "

    Protected Sub RemoveNode(strNode As String)
        Dim tn As TreeNode = Me.FindNode(m_tvNavigation.Nodes, strNode)
        If (tn IsNot Nothing) Then
            Me.m_tvNavigation.Nodes.Remove(tn)
            cLog.Write(String.Format("NavPanel: Removed BETA node '{0}' from navigation tree", strNode))
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Find a node, registered in the node controller tree, using the 
    ''' <see cref="TreeNode.Text">node text</see> as the key.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function FindNode(ByVal nodes As TreeNodeCollection, ByVal strText As String) As TreeNode

        Dim nodeFound As TreeNode = Nothing

        For Each nodeSearch As TreeNode In nodes

            ' Does either node text or name compare to requested text?
            If (String.Compare(nodeSearch.Text, strText) = 0) Or (String.Compare(nodeSearch.Name, strText) = 0) Then
                ' #Yes: got it
                nodeFound = nodeSearch
                Exit For
            End If

            ' Search all child nodes
            If nodeSearch.GetNodeCount(False) <> 0 Then
                nodeFound = FindNode(nodeSearch.Nodes, strText)
                If Not nodeFound Is Nothing Then Exit For
            End If
        Next
        Return nodeFound
    End Function

#End Region ' Internals

End Class