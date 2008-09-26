'==============================================================================
'
' $Log: NavigationPanel.vb,v $
' Revision 1.1  2008/09/26 07:32:11  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.102  2008/08/11 22:15:18  jeroens
' Die! Die! Die! *froth*
'
' Revision 1.101  2008/08/02 03:04:20  jeroens
' Renamed resources
'
' Revision 1.100  2008/07/11 01:00:39  jeroens
' Fixed issue 509
'
' Revision 1.99  2008/06/25 00:26:23  sherman
' Moved Ecospace Results into Node... still has refresh issues
'
' Revision 1.98  2008/06/04 15:33:40  jeroens
' Renamed Ecoseed to MPAOptimizations
'
' Revision 1.97  2008/06/02 00:01:43  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.96  2008/05/23 15:55:47  jeroens
' Added ApplyMed PP form
'
' Revision 1.95  2008/05/07 00:52:50  jeroens
' Renamed market price grid
'
' Revision 1.94  2008/03/24 02:24:20  jeroens
' Renamed Ecoseed to frmEcoseed
'
' Revision 1.93  2008/01/27 16:59:17  jeroens
' Updated help links
'
' Revision 1.92  2008/01/25 03:01:00  jeroens
' Fixed sim output form core states
'
' Revision 1.91  2008/01/23 18:17:10  sherman
' Added Ecoseed Node
'
' Revision 1.90  2008/01/22 16:28:30  jeroens
' Subclassed applyFF forms
'
' Revision 1.89  2008/01/08 11:26:11  jeroens
' Merged tracer input parms and group grid in one screen
'
' Revision 1.88  2007/12/21 15:31:50  jeroens
' * Fixed tracer grid nav link
'
' Revision 1.87  2007/12/18 21:29:21  sherman
' Added Ecotracer Output
'
' Revision 1.86  2007/12/10 02:30:15  sherman
' Re-organized Ecosim Plots and Ecosim Results.  Moved monte carlo run to tools.
'
' Revision 1.85  2007/12/05 03:45:32  jeroens
' * Disabled ecotracer outputs until outputs are available
'
' Revision 1.84  2007/11/28 23:52:07  sherman
' Fixed Tracer Navigation bugs
'
' Revision 1.83  2007/11/28 23:27:04  sherman
' Added EcoTracer Treenode
'
' Revision 1.82  2007/11/06 03:14:47  jeroens
' + Added Sim > tools > tracer
'
' Revision 1.81  2007/10/30 22:52:06  jeroens
' + Added Ecopath model description
'
' Revision 1.80  2007/10/30 02:43:45  jeroens
' + Added fishing effort shape pages
'
' Revision 1.79  2007/10/29 14:14:51  jeroens
' Reworked shape controls
'
' Revision 1.78  2007/10/13 23:27:24  jeroens
' * Partially solved bug 159
'
' Revision 1.77  2007/10/10 04:24:03  jeroens
' * Node 'Biomass' renamed to 'Run Ecosim'
'
' Revision 1.76  2007/09/11 18:56:37  jeroens
' - Removed basemap controller
'
' Revision 1.75  2007/09/05 21:29:51  fgao
' Update ApplyFF nav node
'
' Revision 1.74  2007/08/14 19:37:51  joeh
' Add FitToTimeSeries form
'
' Revision 1.73  2007/08/09 02:16:14  jeroens
' * Fixed potential crash in SelectedNodeName
'
' Revision 1.72  2007/08/02 02:56:33  jeroens
' + Linked to available help pages
'
'==============================================================================

#Region "Imports Directive"

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
''' via the central <see cref="CommandHandler">CommandHandler</see> and its
''' <see cref="NavigationCommand">NavigationCommand</see>.</para>
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class NavigationPanel

    Private m_core As cCore = Nothing
    Private m_nodeController As cTreeViewNodeController = Nothing
    Private m_pluginManager As cPluginManager = Nothing
    Private m_ntPluginHandler As cPluginNavTreeHandler = Nothing
    Private m_tnSelected As TreeNode = Nothing

#Region " Constructors "

    Public Sub New(ByRef p_core As cCore, ByRef p_pluginManager As EwEPlugin.cPluginManager)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.m_core = p_core
        Me.m_pluginManager = p_pluginManager

        AddHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged

        ' Put all the list here
        Me.m_nodeController = New cTreeViewNodeController(Me.m_tvNavigation)

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
        ' Network Analysis PlugIn: "Network%20analysis%20indices%20in.htm"

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
        m_nodeController.Add("ndFishingRate", eCoreExecutionState.EcosimLoaded, GetType(frmFishingRate))
        m_nodeController.Add("ndFishingMortality", eCoreExecutionState.EcosimLoaded, GetType(frmFishingMortality))
        m_nodeController.Add("ndFleetSizeDynamics", eCoreExecutionState.EcosimLoaded, GetType(FisheryInputFleetSizeDynamicsEwEGrid), "Fleet size dynamics.htm")

        ' Ecosim Output
        m_nodeController.Add("ndRunEcosim", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.RunEcosim), "Run Ecosim.htm")
        m_nodeController.Add("ndEcosimPlots", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimOutputPlots), "Ecosim plot.htm")
        m_nodeController.Add("ndEcosimResults", eCoreExecutionState.EcosimCompleted, GetType(Ecosim.EcosimResults), "Ecosim results.htm")
        m_nodeController.Add("ndSRPlot", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.SRplot), "Stock recruitment S R plot.htm")
        m_nodeController.Add("ndFunctionalResponse", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.frmFunctionalResponse), "")

        ' Ecosim Tools
        m_nodeController.Add("ndMCRun", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.MCRun), "Monte Carlo runs.htm")
        m_nodeController.Add("ndFishingPolicySearch", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.FishingPolicySearch), "Fishing policy search.htm")
        m_nodeController.Add("ndFitToTimeSeries", eCoreExecutionState.EcosimLoaded, GetType(Ecosim.FitToTimeSeries), "Fit to time series.htm")

        ' Ecospace
        m_nodeController.Add("ndDispersal", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.DispersalEwEGrid), "Dispersal.htm")
        m_nodeController.Add("ndEcospaceParameters", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceParameters), "Ecospace parameters.htm")
        m_nodeController.Add("ndBasemap", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.Basemap), "Basemap.htm")
        m_nodeController.Add("ndAssignHabitats", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.AssignHabitsEwEGrid), "Assign habitats.htm")
        m_nodeController.Add("ndEcospaceFishery", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.FisheryEwEGrid), "Ecospace Fishery.htm")
        m_nodeController.Add("ndEcospaceScenario", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceScenarioDlg))
        m_nodeController.Add("ndEcospaceResults", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.EcospaceResults), "")
        m_nodeController.Add("ndRunEcospace", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.RunEcospace), "Run Ecospace.htm")
        m_nodeController.Add("ndMPAOptimizations", eCoreExecutionState.EcospaceLoaded, GetType(Ecospace.frmMPAOptimizations), "EcoSeed.htm")

        ' ToDo_JS: Link to yet-to-be-written help text
        m_nodeController.Add("ndEcoTracer_Pram", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerParameters), "")
        m_nodeController.Add("ndEcoTracer_Input", eCoreExecutionState.EcotracerLoaded, GetType(Ecotracer.frmEcotracerInput), "")
        m_nodeController.Add("ndEcoTracer_Output", eCoreExecutionState.EcotracerLoaded, GetType(frmEcotracerOutput), "")

    End Sub

    Private Sub NavigationPanel_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        RemoveHandler Me.m_core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreEcecutionStateChanged
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
        'This value is the text or tabtext of the form
        'FG - Nov 22, 2006: This is a bug for passing text as the key for the same names.
        ' Right now, each form is not designed to have a unique name..To be updated.
        Set(ByVal value As String)
            If m_tvNavigation.Nodes.Count = 0 Then Return

            ' Need to clear selected node?
            If value = String.Empty Then
                ' Whoohoo
                Me.m_tvNavigation.SelectedNode = Nothing
            Else
                Dim nd As TreeNode = Me.FindNode(Me.m_tvNavigation.Nodes, value)
                If Not nd Is Nothing And Not Object.ReferenceEquals(nd, Me.m_tvNavigation.SelectedNode) Then
                    Me.m_tvNavigation.SelectedNode = nd
                End If
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
    <CLSCompliant(False)> _
    Public Function GetTemporaryNavCommand(ByVal ndType As String) As NavigationCommand

        Dim ni As cNodeInfo = m_nodeController.SearchNodeByType(ndType)

        If ni Is Nothing Then Return Nothing

        Dim nodes() As TreeNode = Me.m_tvNavigation.Nodes.Find(ni.NodeName, True)
        Debug.Assert(nodes.Length = 1)
        Return New NavigationCommand(nodes(0).Text, ni.NodeName, ni.ExecutionState, ni.Type)

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

    Private Sub OnCoreEcecutionStateChanged(ByVal core As cCore, ByVal state As eCoreExecutionState)
        With Me.m_tvNavigation
            If core.StateMonitor.HasEcopathLoaded Then
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