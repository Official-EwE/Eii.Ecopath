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

    Private m_core As cCore = Nothing
    Private m_nodeController As cTreeViewNodeController = Nothing
    Private m_pluginManager As cPluginManager = Nothing
    Private m_ntPluginHandler As cPluginNavTreeHandler = Nothing
    Private m_tnSelected As TreeNode = Nothing

#Region " Constructors "

    Public Sub New(ByRef core As cCore, ByRef pluginManager As EwEPlugin.cPluginManager)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.m_core = core
        Me.m_pluginManager = pluginManager

        AddHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

        ' Put all the list here
        Me.m_nodeController = New cTreeViewNodeController()
        Me.m_nodeController.Attach(Me.m_tvNavigation)

        'Basic Parameters
        m_nodeController.Add("ndModelDescription", eCoreExecutionState.EcopathLoaded, GetType(frmModelDescription), "Model description.htm")
        m_nodeController.Add("ndBasicInput", eCoreExecutionState.EcopathLoaded, GetType(BasicInputEwEGrid), "Basic input.htm")
        m_nodeController.Add("ndDietComposition", eCoreExecutionState.EcopathLoaded, GetType(DietComp), "Diet composition.htm")
        m_nodeController.Add("ndDetritusFate", eCoreExecutionState.EcopathLoaded, GetType(DetritusFateEwEGrid), "Detritus fate.htm")
        m_nodeController.Add("ndOtherProduction", eCoreExecutionState.EcopathLoaded, GetType(OtherProductionEwEGrid), "Other production.htm")
        m_nodeController.Add("ndDefFleets", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputFleetDefinitionEwEGrid), "Definition of fleets.htm")
        m_nodeController.Add("ndLandings", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputLandingsEwEGrid), "Landings.htm")
        m_nodeController.Add("ndDiscards", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardsEwEGrid), "Discards.htm")
        m_nodeController.Add("ndDiscardFate", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardFateEwEGrid), "Discard fate.htm")
        m_nodeController.Add("ndDiscardMortRate", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputDiscardMortGrid), "") ' ToDo: connect to help
        m_nodeController.Add("ndOffVesselPrice", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputOffVesselPriceEwEGrid), "Market price.htm")
        m_nodeController.Add("ndNonMarketPrice", eCoreExecutionState.EcopathLoaded, GetType(FisheryInputNonMarketPriceEwEGrid), "Non market price.htm")

        ' Ecopath Output
        m_nodeController.Add("ndBasicEstimates", eCoreExecutionState.EcopathCompleted, GetType(BasicEstimatesEwEGrid), "Basic estimates.htm")
        m_nodeController.Add("ndKeyIndices", eCoreExecutionState.EcopathCompleted, GetType(KeyIndicesEwEGrid), "Key indices.htm")
        m_nodeController.Add("ndMortCoef", eCoreExecutionState.EcopathCompleted, GetType(MortalityCoefficientsEwEGrid), "Mortalities.htm")
        m_nodeController.Add("ndPredMort", eCoreExecutionState.EcopathCompleted, GetType(MortalityPredationEwEGrid), "Predation mortality.htm")
        m_nodeController.Add("ndConsumption", eCoreExecutionState.EcopathCompleted, GetType(ConsumptionEwEGrid), "Consumption.htm")
        m_nodeController.Add("ndRespiration", eCoreExecutionState.EcopathCompleted, GetType(RespirationEwEGrid), "Respiration.htm")
        m_nodeController.Add("ndPreyOverlap", eCoreExecutionState.EcopathCompleted, GetType(NicheOverlapPreyEwEGrid), "Niche overlap.htm")
        m_nodeController.Add("ndPredatorOverlap", eCoreExecutionState.EcopathCompleted, GetType(NicheOverlapPredatorEwEGrid), "Niche overlap.htm")
        m_nodeController.Add("ndElectivity", eCoreExecutionState.EcopathCompleted, GetType(ElectivityEwEGrid), "Electivity.htm")
        m_nodeController.Add("ndSearchRates", eCoreExecutionState.EcopathCompleted, GetType(SearchRatesEwEGrid), "Search rates.htm")
        m_nodeController.Add("ndQuantity", eCoreExecutionState.EcopathCompleted, GetType(FisheryOutputQuantityEwEGrid), "Fishery (Ecopath parameterization).htm")
        m_nodeController.Add("ndValue", eCoreExecutionState.EcopathCompleted, GetType(FisheryOutputValueEwEGrid), "Fishery (Ecopath parameterization).htm")
        m_nodeController.Add("ndFlowDiagram", eCoreExecutionState.EcopathCompleted, GetType(FlowDiagram.FlowDiagram), "Flow diagram.htm")
        m_nodeController.Add("ndEcopathStats", eCoreExecutionState.EcopathCompleted, GetType(EcopathStatisticsEwEGrid), "")
        ' Network Analysis PlugIn: "Network%20analysis%20indices%20in.htm"

        ' PSD Input
        m_nodeController.Add("ndGrowthParameters", eCoreExecutionState.EcopathLoaded, GetType(GrowthParametersEwEGrid), "") ' ToDo: connect to help
        m_nodeController.Add("ndRunPSD", eCoreExecutionState.EcopathLoaded, GetType(RunPSD), "") ' ToDo: connect to help

        ' PSD Output
        m_nodeController.Add("ndGrowthEstimates", eCoreExecutionState.PSDCompleted, GetType(GrowthEstimatesEwEGrid), "") ' ToDo: connect to help
        m_nodeController.Add("ndPSDContributionPlot", eCoreExecutionState.PSDCompleted, GetType(PSDContributionPlot), "") ' ToDo: connect to help
        m_nodeController.Add("ndPSDContributionResult", eCoreExecutionState.PSDCompleted, GetType(PSDContributionResult), "") ' ToDo: connect to help
        m_nodeController.Add("ndPSDPlotByGroup", eCoreExecutionState.PSDCompleted, GetType(PSDPlotByGroup), "") ' ToDo: connect to help
        m_nodeController.Add("ndSizeWeightPlot", eCoreExecutionState.PSDCompleted, GetType(SizeWeightPlot), "") ' ToDo: connect to help

        ' Ecosim Input
        m_nodeController.Add("ndEcosimParameters", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.EcosimParameters), "Ecosim parameters.htm")
        m_nodeController.Add("ndGroupInfo", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.GroupInfoEwEGrid), "Group info.htm")
        m_nodeController.Add("ndVulnerabilities", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.Vulnerabilities), "Vulnerabilities flow control.htm")
        m_nodeController.Add("ndTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTimeSeries), "Time series.htm")
        m_nodeController.Add("ndMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmMediationFunction), "Mediation.htm")
        m_nodeController.Add("ndApplyMediation", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedConsumer), "Apply mediation.htm")
        m_nodeController.Add("ndApplyMediationPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyMedPrimaryProducer), "Apply mediation.htm")
        m_nodeController.Add("ndForcingFunction", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmForcingFunction), "Forcing function.htm")
        m_nodeController.Add("ndApplyFFConsumer", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFConsumer), "Apply forcing function consumer.htm")
        m_nodeController.Add("ndApplyFFPP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmApplyFFPrimaryProducer), "Apply forcing function primary.htm")
        m_nodeController.Add("ndEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmEggProduction), "Egg production.htm")
        m_nodeController.Add("ndApplyEP", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.ApplyEP), "Apply egg production.htm")
        m_nodeController.Add("ndFishingEffort", eCoreExecutionState.EcosimLoaded, GetType(frmFishingEffort)) ' ToDo: connect to help
        m_nodeController.Add("ndRegulatoryOptions", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.gridRegulatoryOptions)) ' ToDo: connect to help
        m_nodeController.Add("ndFleetQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.gridFishingQuotas)) ' ToDo: connect to help
        m_nodeController.Add("ndSpeciesQuotas", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmTargetFishingMortalityPolicy)) ' ToDo: connect to help
        m_nodeController.Add("ndFleetSizeDynamics", eCoreExecutionState.EcosimLoaded, GetType(FisheryInputFleetSizeDynamicsEwEGrid), "Fleet size dynamics.htm")

        ' Ecosim Output
        m_nodeController.Add("ndRunEcosim", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.RunEcosim), "Run Ecosim.htm")
        m_nodeController.Add("ndEcosimPlots", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimOutputPlots), "Ecosim plot.htm")
        m_nodeController.Add("ndEcosimResults", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimResults), "Ecosim results.htm")
        m_nodeController.Add("ndSRPlot", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.SRplot), "Stock recruitment S R plot.htm")
        m_nodeController.Add("ndSuitabilityPlot", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.SuitabilityPlot)) ' ToDo: connect to help
        m_nodeController.Add("ndFishingMortality", eCoreExecutionState.EcosimCompleted, GetType(frmFishingMortality)) ' ToDo: connect to help

        ' Ecosim Tools
        m_nodeController.Add("ndMCRun", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.MCRun), "Monte Carlo runs.htm") ' ToDo: connect to help
        m_nodeController.Add("ndFishingPolicySearch", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFishingPolicySearch), "Fishing policy search.htm")
        m_nodeController.Add("ndFitToTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFitToTimeSeries), "Fit to time series.htm")

        ' Ecospace
        m_nodeController.Add("ndDispersal", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.DispersalEwEGrid), "Dispersal.htm")
        m_nodeController.Add("ndEcospaceParameters", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceParameters), "Ecospace parameters.htm")
        m_nodeController.Add("ndBasemap", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.Basemap.Basemap), "Basemap.htm") ' ToDo: connect to help
        m_nodeController.Add("ndAssignHabitats", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridAssignHabits), "Assign habitats.htm")
        m_nodeController.Add("ndEcospaceFishery", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.gridFishery), "Ecospace Fishery.htm")
        m_nodeController.Add("ndEcospaceScenario", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceScenarioDlg)) ' ToDo: connect to help
        m_nodeController.Add("ndRunEcospace", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.RunEcospace), "Run Ecospace.htm")
        m_nodeController.Add("ndMPAOptimizations", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmMPAOptimizations), "EcoSeed.htm")

        ' Ecospace output
        m_nodeController.Add("ndEcospaceResults", eCoreExecutionState.EcospaceCompleted, GetType(Ecospace.cFormEcospaceResults), "") ' ToDo: connect to help

        ' ToDo_JS: Link to yet-to-be-written help text
        m_nodeController.Add("ndEcoTracer_Pram", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerParameters), "") ' ToDo: connect to help
        m_nodeController.Add("ndEcoTracer_Input", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerInput), "") ' ToDo: connect to help
        m_nodeController.Add("ndEcoTracer_Output", eCoreExecutionState.EcotracerLoaded, GetType(frmEcotracerOutput), "") ' ToDo: connect to help

        'MSE
        m_nodeController.Add("ndMSERun", eCoreExecutionState.EcosimLoaded, GetType(frmMSE), "") ' ToDo: connect to help

    End Sub

    Private Sub NavigationPanel_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed

        RemoveHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged
        Me.m_nodeController.Detach()
        Me.m_nodeController = Nothing

    End Sub

#End Region ' Constructors

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

    ''' <summary>
    ''' Return a temporary navigation command for transferring information.
    ''' </summary>
    ''' <param name="ndType"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Firing this command will not result in navigation changes.
    ''' </remarks>
    Public Function GetTemporaryNavCommand(ByVal ndType As String) As cNavigationCommand

        Dim ni As cNodeInfo = m_nodeController.SearchNodeByType(ndType)

        If ni Is Nothing Then Return Nothing

        Dim nodes() As TreeNode = Me.m_tvNavigation.Nodes.Find(ni.NodeName, True)
        Debug.Assert(nodes.Length = 1)
        Return New cNavigationCommand(nodes(0).Text, ni.NodeName, ni.ExecutionState, ni.Type)

    End Function

    Public Sub Reset()
        For Each node As TreeNode In Me.m_tvNavigation.Nodes
            node.Collapse()
        Next
    End Sub

#End Region ' Properties

#Region " Helper methods "

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
                ' #Yes: is this a registered node?
                If (Me.m_nodeController.SearchNodeByName(nodeSearch.Name) IsNot Nothing) Then
                    ' #Yes: got it
                    nodeFound = nodeSearch
                    Exit For
                End If
            End If

            ' Search all child nodes
            If nodeSearch.GetNodeCount(False) <> 0 Then
                nodeFound = FindNode(nodeSearch.Nodes, strText)
                If Not nodeFound Is Nothing Then Exit For
            End If
        Next
        Return nodeFound
    End Function

#End Region ' Helper methods

#Region " Event handlers "

    Private Sub NavigationPanel_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If m_tvNavigation.SelectedNode IsNot Nothing Then
            ' Whoohooo!
            If CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft Then
                Me.ShowHint = DockState.DockRight
            Else
                Me.ShowHint = DockState.DockLeft
            End If
        End If

        ' Integrate plug-ins
        If Me.m_pluginManager IsNot Nothing Then
            m_ntPluginHandler = New cPluginNavTreeHandler(Me.m_tvNavigation, Me.m_pluginManager)
        End If

    End Sub

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

End Class