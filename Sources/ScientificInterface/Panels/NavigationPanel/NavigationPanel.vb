#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Globalization

Imports EwECore
Imports ScientificInterface.Ecopath.Controls
Imports ScientificInterface.Ecopath.Input
Imports ScientificInterface.Ecopath.Output
Imports ScientificInterface.Ecosim
Imports ScientificInterface.Ecospace
Imports EwEPlugin
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports WeifenLuo.WinFormsUI.Docking

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
''' via the central <see cref="cCommandHandler">CommandHandler</see> and its
''' <see cref="cNavigationCommand">NavigationCommand</see>.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class NavigationPanel

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

        ' Hit 'em, Jimmy
        Me.InitializeComponent()

    End Sub

    Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
        RemoveHandler Me.m_uic.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

        If bDisposing Then

            Me.m_nodeController.Detach()
            Me.m_nodeController = Nothing

            Me.m_ntPluginHandler = Nothing
            Me.m_pluginManager = Nothing
            Me.m_uic = Nothing

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

        ' Put all the list here
        Me.m_nodeController = New cTreeViewNodeController()
        Me.m_nodeController.Attach(Me.m_uic, Me.m_tvNavigation)

        With Me.m_nodeController

            'Basic Parameters
            .Add("ndModelDescription", eCoreExecutionState.EcopathLoaded, GetType(frmModelDescription), "Model description.htm")
            .Add("ndBasicInput", eCoreExecutionState.EcopathLoaded, GetType(BasicInputEwEGrid), "Basic input.htm")
            .Add("ndDietComposition", eCoreExecutionState.EcopathLoaded, GetType(DietComp), "Diet composition.htm")
            .Add("ndDetritusFate", eCoreExecutionState.EcopathLoaded, GetType(DetritusFateEwEGrid), "Detritus fate.htm")
            .Add("ndOtherProduction", eCoreExecutionState.EcopathLoaded, GetType(OtherProductionEwEGrid), "Other production.htm")
            .Add("ndDefFleets", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputFleetDefinitionEwEGrid), "Definition of fleets.htm")
            .Add("ndLandings", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputLandingsEwEGrid), "Landings.htm")
            .Add("ndDiscards", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardsEwEGrid), "Discards.htm")
            .Add("ndDiscardFate", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardFateEwEGrid), "Discard fate.htm")
            .Add("ndDiscardMortRate", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardMortGrid), "") ' ToDo: connect to help
            .Add("ndOffVesselPrice", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputOffVesselPriceEwEGrid), "Market price.htm")
            .Add("ndNonMarketPrice", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputNonMarketPriceEwEGrid), "Non market price.htm")

            ' Ecopath Output
            .Add("ndBasicEstimates", eCoreExecutionState.EcopathCompleted, GetType(BasicEstimatesEwEGrid), "Basic estimates.htm")
            .Add("ndKeyIndices", eCoreExecutionState.EcopathCompleted, GetType(KeyIndicesEwEGrid), "Key indices.htm")
            .Add("ndMortCoef", eCoreExecutionState.EcopathCompleted, GetType(MortalityCoefficientsEwEGrid), "Mortalities.htm")
            .Add("ndPredMort", eCoreExecutionState.EcopathCompleted, GetType(MortalityPredationEwEGrid), "Predation mortality.htm")
            .Add("ndFleetFishingMortality", eCoreExecutionState.EcopathCompleted, GetType(FleetFishingMortalityGrid), "")
            .Add("ndConsumption", eCoreExecutionState.EcopathCompleted, GetType(ConsumptionEwEGrid), "Consumption.htm")
            .Add("ndRespiration", eCoreExecutionState.EcopathCompleted, GetType(RespirationEwEGrid), "Respiration.htm")
            .Add("ndPreyOverlap", eCoreExecutionState.EcopathCompleted, GetType(NicheOverlapPreyEwEGrid), "Niche overlap.htm")
            .Add("ndPredatorOverlap", eCoreExecutionState.EcopathCompleted, GetType(NicheOverlapPredatorEwEGrid), "Niche overlap.htm")
            .Add("ndElectivity", eCoreExecutionState.EcopathCompleted, GetType(ElectivityEwEGrid), "Electivity.htm")
            .Add("ndSearchRates", eCoreExecutionState.EcopathCompleted, GetType(SearchRatesEwEGrid), "Search rates.htm")
            .Add("ndQuantity", eCoreExecutionState.EcopathCompleted, GetType(FisheryOutputQuantityEwEGrid), "Fishery (Ecopath parameterization).htm")
            .Add("ndValue", eCoreExecutionState.EcopathCompleted, GetType(FisheryOutputValueEwEGrid), "Fishery (Ecopath parameterization).htm")
            .Add("ndFlowDiagram", eCoreExecutionState.EcopathCompleted, GetType(FlowDiagram.FlowDiagram), "Flow diagram.htm")
            .Add("ndEcopathStats", eCoreExecutionState.EcopathCompleted, GetType(EcopathStatisticsEwEGrid), "")
            ' Network Analysis PlugIn: "Network%20analysis%20indices%20in.htm"

            ' PSD Input
            .Add("ndGrowthParameters", eCoreExecutionState.EcopathLoaded, GetType(GrowthParametersEwEGrid), "") ' ToDo: connect to help
            .Add("ndRunPSD", eCoreExecutionState.EcopathLoaded, GetType(RunPSD), "") ' ToDo: connect to help

            ' PSD Output
            .Add("ndGrowthEstimates", eCoreExecutionState.PSDCompleted, GetType(GrowthEstimatesEwEGrid), "") ' ToDo: connect to help
            .Add("ndPSDContributionPlot", eCoreExecutionState.PSDCompleted, GetType(PSDContributionPlot), "") ' ToDo: connect to help
            .Add("ndPSDContributionResult", eCoreExecutionState.PSDCompleted, GetType(PSDContributionResult), "") ' ToDo: connect to help
            .Add("ndPSDPlotByGroup", eCoreExecutionState.PSDCompleted, GetType(PSDPlotByGroup), "") ' ToDo: connect to help
            .Add("ndSizeWeightPlot", eCoreExecutionState.PSDCompleted, GetType(SizeWeightPlot), "") ' ToDo: connect to help

            ' Ecosim Input
            .Add("ndEcosimParameters", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.EcosimParameters), "Ecosim parameters.htm")
            .Add("ndGroupInfo", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.GroupInfoEwEGrid), "Group info.htm")
            .Add("ndVulnerabilities", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.Vulnerabilities), "Vulnerabilities flow control.htm")
            .Add("ndTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTimeSeries), "Time series.htm")
            .Add("ndMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmMediationFunction), "Mediation.htm")
            .Add("ndApplyMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedConsumer), "Apply mediation.htm")
            .Add("ndApplyMediationPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedPrimaryProducer), "Apply mediation.htm")
            .Add("ndForcingFunction", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmForcingFunction), "Forcing function.htm")
            .Add("ndApplyFFConsumer", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFConsumer), "Apply forcing function consumer.htm")
            .Add("ndApplyFFPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFPrimaryProducer), "Apply forcing function primary.htm")
            .Add("ndEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmEggProduction), "Egg production.htm")
            .Add("ndApplyEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.ApplyEP), "Apply egg production.htm")
            .Add("ndFishingEffort", eCoreExecutionState.EcosimLoaded, GetType(frmFishingEffort)) ' ToDo: connect to help
            .Add("ndFleetQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.gridQuotaShare)) ' ToDo: connect to help
            '.Add("ndSpeciesQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTargetFishingMortalityPolicy)) ' ToDo: connect to help
            .Add("ndFleetSizeDynamics", eCoreExecutionState.EcosimLoaded, GetType(FisheryInputFleetSizeDynamicsEwEGrid), "Fleet size dynamics.htm")

            ' Ecosim Output
            .Add("ndRunEcosim", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.RunEcosim), "Run Ecosim.htm")
            .Add("ndEcosimPlots", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimOutputPlots), "Ecosim plot.htm")
            .Add("ndEcosimResults", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimResults), "Ecosim results.htm")
            .Add("ndEcosimAllFits", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.frmShowAllFits), "Ecosim results.htm")
            .Add("ndSRPlot", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.SRplot), "Stock recruitment S R plot.htm")
            .Add("ndSuitabilityPlot", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.SuitabilityPlot)) ' ToDo: connect to help
            .Add("ndFishingMortality", eCoreExecutionState.EcosimCompleted, GetType(frmFishingMortality)) ' ToDo: connect to help

            ' Ecosim Tools
            .Add("ndMCRun", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.MCRun), "Monte Carlo runs.htm") ' ToDo: connect to help
            .Add("ndFishingPolicySearch", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFishingPolicySearch), "Fishing policy search.htm")
            .Add("ndFitToTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFitToTimeSeries), "Fit to time series.htm")

            ' Ecospace
            .Add("ndDispersal", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.DispersalEwEGrid), "Dispersal.htm")
            .Add("ndEcospaceParameters", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceParameters), "Ecospace parameters.htm")
            .Add("ndBasemap", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.Basemap.Basemap), "Basemap.htm") ' ToDo: connect to help
            .Add("ndAssignHabitats", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridAssignHabits), "Assign habitats.htm")
            .Add("ndEcospaceFishery", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridFishery), "Ecospace Fishery.htm")
            .Add("ndEcospaceScenario", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceScenarioDlg)) ' ToDo: connect to help
            .Add("ndRunEcospace", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.RunEcospace), "Run Ecospace.htm")
            .Add("ndMPAOptimizations", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmMPAOptimizations), "EcoSeed.htm")

            ' Ecospace output
            .Add("ndEcospaceResults", eCoreExecutionState.EcospaceCompleted, GetType(Ecospace.cFormEcospaceResults), "") ' ToDo: connect to help

            ' ToDo_JS: Link to yet-to-be-written help text
            .Add("ndEcoTracer_Pram", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerParameters), "") ' ToDo: connect to help
            .Add("ndEcoTracer_Input", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerInput), "") ' ToDo: connect to help
            .Add("ndEcoTracer_Output", eCoreExecutionState.EcotracerLoaded, GetType(frmEcotracerOutput), "") ' ToDo: connect to help

            'MSE
            .Add("ndMSERun", eCoreExecutionState.EcosimLoaded, GetType(frmMSE), "") ' ToDo: connect to help
            .Add("ndRefFixedEscape", eCoreExecutionState.EcosimLoaded, GetType(gridFixedEscapement), "") ' ToDo: connect to help
            .Add("ndOptions", eCoreExecutionState.EcosimLoaded, GetType(frmOptions), "") ' ToDo: connect to help
            .Add("ndControlType", eCoreExecutionState.EcosimLoaded, GetType(gridRegulatoryOptions), "") ' ToDo: connect to help
            .Add("ndQuotaShare", eCoreExecutionState.EcosimLoaded, GetType(frmQuotaShare), "") ' ToDo: connect to help
            .Add("ndRegFishingMort", eCoreExecutionState.EcosimLoaded, GetType(frmTargetFishingMortalityPolicy), "") ' ToDo: connect to help

            'jb march-8-2010 removed Group objectives and Objective weights as they are not being used by the MSE
            '.Add("ndEfTrackObjectives", eCoreExecutionState.EcosimLoaded, GetType(gridMSEOjectiveWeights), "") ' ToDo: connect to help
            '.Add("ndEfTrackEcoObjectives", eCoreExecutionState.EcosimLoaded, GetType(gridMSEGroupObjectives), "") ' ToDo: connect to help
            .Add("ndEfTrackFleetWeights", eCoreExecutionState.EcosimLoaded, GetType(gridFishingWeights), "") ' ToDo: connect to help

            .Add("ndAssessGroup", eCoreExecutionState.EcosimLoaded, GetType(frmMSEAssessGroups), "") ' ToDo: connect to help
            .Add("ndAssessFleet", eCoreExecutionState.EcosimLoaded, GetType(frmMSEAssessFleets), "") ' ToDo: connect to help
            .Add("ndRefMSY", eCoreExecutionState.EcosimLoaded, GetType(frmMSY), "") ' ToDo: connect to help
            .Add("ndRefBiomass", eCoreExecutionState.EcosimLoaded, GetType(frmGroupRefLevels), "") ' ToDo: connect to help
            .Add("ndRefCatch", eCoreExecutionState.EcosimLoaded, GetType(gridFleetRefLevels), "") ' ToDo: connect to help
            .Add("ndMSEPlots", eCoreExecutionState.EcosimLoaded, GetType(frmMSEPlots), "") ' ToDo: connect to help
            .Add("ndMSEResults", eCoreExecutionState.EcosimLoaded, GetType(frmMSEResults), "") ' ToDo: connect to help

            .Add("ndMSERecruitment", eCoreExecutionState.EcosimLoaded, GetType(gridMSERecruitment), "") ' ToDo: connect to help
            'ndMSERecruitment

        End With

        ' JS 19Mar2010: now why was this necessary?
        If (Me.m_tvNavigation.SelectedNode IsNot Nothing) Then
            If CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft Then
                Me.ShowHint = DockState.DockRight
            Else
                Me.ShowHint = DockState.DockLeft
            End If
        End If

        ' Integrate plug-ins
        If Me.m_pluginManager IsNot Nothing Then
            Me.m_ntPluginHandler = New cPluginNavTreeHandler(Me.m_tvNavigation, Me.m_pluginManager, Me.m_uic.CommandHandler)
        End If

        AddHandler Me.m_uic.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

    End Sub

#End Region ' Form overrides

#Region " Properties "

    ''' <summary>
    ''' Get or set the current selected node name in the nav structure
    ''' </summary>
    ''' <remarks>In order to highlight the selection</remarks>
    Public Property SelectedNodeName() As String

        Get
            If Me.m_tvNavigation.SelectedNode Is Nothing Then Return ""
            Return Me.m_tvNavigation.SelectedNode.Name
        End Get

        Set(ByVal value As String)

            Dim bSelected As Boolean = False
            Dim nd As TreeNode = Nothing

            If m_tvNavigation.Nodes.Count = 0 Then Return

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Find a node, registered in the node controller tree, using the 
    ''' <see cref="TreeNode.Text">node text</see> as the key.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function FindNode(ByVal nodes As TreeNodeCollection, ByVal strText As String) As TreeNode

        Dim nodeFound As TreeNode = Nothing

        For Each nodeSearch As TreeNode In nodes

            ' Does node text compare to requested text?
            If (String.Compare(nodeSearch.Text, strText) = 0) Then
                ' JS 26Feb2010: do not search registered nodes since this will not find nodes added via plug-ins.
                '' #Yes: is this a registered node?
                'If (Me.m_nodeController.SearchNodeByName(nodeSearch.Name) IsNot Nothing) Then
                ' #Yes: got it
                nodeFound = nodeSearch
                Exit For
                'End If
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