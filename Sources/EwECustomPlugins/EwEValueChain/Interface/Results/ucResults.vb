#Region " Imports "

Option Strict On

Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Commands
Imports ZedGraph
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Utilities

#End Region ' Imports

Public Class ucResults

#Region " Helper classes "

    Private Class cCoreComboItem

        Private m_source As cCoreInputOutputBase

        Public Sub New(ByVal source As cCoreInputOutputBase)
            Me.m_source = source
        End Sub

        Public Overrides Function ToString() As String
            If Me.m_source Is Nothing Then Return "<all fleets>"
            Return Me.m_source.Name
        End Function

        Public ReadOnly Property Source() As cCoreInputOutputBase
            Get
                Return Me.m_source
            End Get
        End Property

    End Class

#End Region ' Helper classes

#Region " Private bits "

    Private Shared g_iLastFleet As Integer = 0

    Private Enum eViewModeType As Integer
        Ecopath = 0
        Ecosim
        Equilibrium
    End Enum

    Enum eGraphDataType As Integer
        CostRevenue = 0
        Cost
        Revenue
        Jobs
        Dependents
    End Enum

    ''' <summary>Ecoat data that provides, unit, links and other dynamic bits and pieces.</summary>
    Private m_data As cData = Nothing
    ''' <summary>Instance of the Ecost model to poke and prod.</summary>
    Private m_model As cModel = Nothing
    ''' <summary>Instance of model results to reflect.</summary>
    Private m_result As cResults = Nothing
    ''' <summary>UI context to operate on.</summary>
    Private m_uic As cUIContext = Nothing

    ''' <summary>Viewmode dictates what type of result screen the user sees.</summary>
    Private m_viewMode As eViewModeType = eViewModeType.Ecosim
    ''' <summary>Graphmode dictates what data is viewed in result graphs.</summary>
    Private m_graphmode As eGraphDataType = eGraphDataType.CostRevenue
    ''' <summary>Current view to update when triggers arrive.</summary>
    Private m_view As IResultView = Nothing
    ''' <summary>Update feedback prevention flaggibit.</summary>
    Private m_bInUpdate As Boolean = False

    ''' <summary>Local command to for running Ecopath.</summary>
    Private m_cmdRunEcopath As cCommand = Nothing
    ''' <summary>Local command to for running Ecosim.</summary>
    Private m_cmdRunEcosim As cCommand = Nothing
    ''' <summary>Local command to for running Equilibrium.</summary>
    Private m_cmdRunEqulibrium As cCommand = Nothing

#End Region ' Private bits

#Region " Constructor "

    Public Sub New(ByVal uic As cUIContext, _
                   ByVal data As cData, _
                   ByVal model As cModel, _
                   ByVal result As cResults)

        Me.InitializeComponent()

        Me.m_uic = uic
        Me.m_data = data
        Me.m_model = model
        Me.m_result = result

        Dim cmdH As cCommandHandler = Me.m_uic.CommandHandler

        ' Start listening for model events
        AddHandler Me.m_model.OnRunCompleted, AddressOf OnModelRunCompleted

        ' Set up commands
        Me.m_cmdRunEcopath = New cCommand(cmdH, "VC_RunEcopath")
        Me.m_cmdRunEcopath.AddControl(Me.m_btnRunEcopath)
        AddHandler Me.m_cmdRunEcopath.OnInvoke, AddressOf OnInvokeRunEcopath
        AddHandler Me.m_cmdRunEcopath.OnUpdate, AddressOf OnUpdateRunEcopath

        Me.m_cmdRunEcosim = New cCommand(cmdH, "VC_RunEcosim")
        Me.m_cmdRunEcosim.AddControl(Me.m_btnRunEcosim)
        AddHandler Me.m_cmdRunEcosim.OnInvoke, AddressOf OnInvokeRunEcosim
        AddHandler Me.m_cmdRunEcosim.OnUpdate, AddressOf OnUpdateRunEcosim

        Me.m_cmdRunEqulibrium = New cCommand(cmdH, "VC_RunEqulibrium")
        Me.m_cmdRunEqulibrium.AddControl(Me.m_btnRunEquilibrium)
        AddHandler Me.m_cmdRunEqulibrium.OnInvoke, AddressOf OnInvokeRunEquilibrium
        AddHandler Me.m_cmdRunEqulibrium.OnUpdate, AddressOf OnUpdateRunEquilibrium

        Me.Initialize()

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing Then
                Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler

                cmdh.Remove(Me.m_cmdRunEcopath)
                RemoveHandler Me.m_cmdRunEcopath.OnInvoke, AddressOf OnInvokeRunEcosim
                RemoveHandler Me.m_cmdRunEcopath.OnUpdate, AddressOf OnUpdateRunEcosim
                Me.m_cmdRunEcopath = Nothing

                cmdh.Remove(Me.m_cmdRunEcosim)
                RemoveHandler Me.m_cmdRunEcosim.OnInvoke, AddressOf OnInvokeRunEcosim
                RemoveHandler Me.m_cmdRunEcosim.OnUpdate, AddressOf OnUpdateRunEcosim
                Me.m_cmdRunEcosim = Nothing

                cmdh.Remove(Me.m_cmdRunEqulibrium)
                RemoveHandler Me.m_cmdRunEqulibrium.OnInvoke, AddressOf OnInvokeRunEquilibrium
                RemoveHandler Me.m_cmdRunEqulibrium.OnUpdate, AddressOf OnUpdateRunEquilibrium
                Me.m_cmdRunEcosim = Nothing

                RemoveHandler Me.m_model.OnRunCompleted, AddressOf OnModelRunCompleted
                If components IsNot Nothing Then
                    components.Dispose()
                End If
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region ' Constructor

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_tscmbGraphData.Items.Clear()
        For Each gd As eGraphDataType In [Enum].GetValues(GetType(eGraphDataType))
            Me.m_tscmbGraphData.Items.Add(gd)
        Next
        Me.m_tscmbGraphData.SelectedIndex = 0

        ' Restore last selection
        Me.m_tscmbFleets.SelectedIndex = Math.Min(Me.m_tscmbFleets.Items.Count - 1, Math.Max(-1, ucResults.g_iLastFleet))
    End Sub

    Private Sub OnFilterByFleet(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbFleets.SelectedIndexChanged

        ' Filter by fleet
        Dim item As cCoreComboItem = Nothing
        Dim fleet As cFleetInput = Nothing

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        ucResults.g_iLastFleet = Me.m_tscmbFleets.SelectedIndex
        item = DirectCast(Me.m_tscmbFleets.SelectedItem, cCoreComboItem)
        If item IsNot Nothing Then fleet = DirectCast(item.Source, cFleetInput)

        Me.m_plFlow.FleetFilter = fleet

        Me.UpdateControls()
        Me.UpdateFilter()

        Me.m_bInUpdate = False

    End Sub

    Private Sub OnDoubleClickedFlow(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_scResults.DoubleClick
        Me.m_tsbShowFlow.Checked = Not m_tsbShowFlow.Checked
        Me.UpdateControls()
    End Sub

    Private Sub m_tsbShowFlow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbShowFlow.Click
        Me.m_tsbShowFlow.Checked = Not m_tsbShowFlow.Checked
        Me.UpdateControls()
    End Sub

    Private Sub OnShowEcopath(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbEcopath.Click
        Me.SetViewMode(eViewModeType.Ecopath)
    End Sub

    Private Sub OnShowEcosim(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbEcosim.Click
        Me.SetViewMode(eViewModeType.Ecosim)
    End Sub

    Private Sub OnShowEquilibrium(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbEquilibrium.Click
        Me.SetViewMode(eViewModeType.Equilibrium)
    End Sub

    Private Sub OnModelRunCompleted(ByVal iTimeStep As Integer)
        ' NOP
    End Sub

    Private Sub OnGraphDataSelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tscmbGraphData.SelectedIndexChanged
        Me.SetGraphData(DirectCast(Me.m_tscmbGraphData.SelectedItem, eGraphDataType))
        Me.UpdateResults()
    End Sub

#Region " Commands "

    Private Sub OnInvokeRunEcopath(ByVal cmd As EwEUtils.Commands.cCommand)

        Dim bOldRunFlag As Boolean = Me.m_data.Parameters.RunWithEcopath

        ' Switch to manual run mode
        Me.m_model.IsManualRunMode = True
        Me.m_data.Parameters.RunWithEcopath = True

        cApplicationStatusNotifier.SetStatusText("Running value chain for Ecopath, please wait...", TriState.True)

        Try

            ' Reset results and prepare for receiving Ecopath results
            '    In manual mode the calling process becomes responsible for resetting results
            Me.m_result.Reset(cModel.eRunTypes.Ecopath)
            ' Prepare to display Ecopath results
            Me.SetViewMode(eViewModeType.Ecopath)
            ' Run Ecopath
            Me.m_data.Core.RunEcoPath()
            ' Reflect results
            Me.UpdateResults()

        Catch ex As Exception

        End Try

        ' Switch back to auto run mode
        Me.m_model.IsManualRunMode = False
        Me.m_data.Parameters.RunWithEcopath = bOldRunFlag

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

    End Sub

    Private Sub OnUpdateRunEcopath(ByVal cmd As EwEUtils.Commands.cCommand)
        Dim csm As cCoreStateMonitor = Me.m_data.Core.StateMonitor
        cmd.Enabled = csm.HasEcopathLoaded And (Not csm.IsEcopathRunning)
    End Sub

    Private Sub OnInvokeRunEcosim(ByVal cmd As EwEUtils.Commands.cCommand)

        Dim bOldRunFlag As Boolean = Me.m_data.Parameters.RunWithEcosim

        cApplicationStatusNotifier.SetStatusText("Running value chain for Ecosim, please wait...", TriState.True)

        ' Switch to manual run mode
        Me.m_model.IsManualRunMode = True
        Me.m_data.Parameters.RunWithEcosim = True

        Try

            ' Reset cached results
            '    In manual mode the calling process becomes responsible for resetting results
            Me.m_result.Reset(cModel.eRunTypes.Ecosim)
            ' Prepare view
            Me.SetViewMode(eViewModeType.Ecosim)
            ' Run Ecosim
            Me.m_data.Core.RunEcoSim(AddressOf EcosimTimestepHandler)
            ' Update results
            Me.UpdateResults()

        Catch ex As Exception

        End Try

        ' Switch back to auto run mode
        Me.m_model.IsManualRunMode = False
        Me.m_data.Parameters.RunWithEcosim = bOldRunFlag

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

    End Sub

    Private Sub OnUpdateRunEcosim(ByVal cmd As EwEUtils.Commands.cCommand)
        Dim csm As cCoreStateMonitor = Me.m_data.Core.StateMonitor
        cmd.Enabled = csm.HasEcosimLoaded And (Not csm.IsEcosimRunning)
    End Sub

    Private Sub OnInvokeRunEquilibrium(ByVal cmd As EwEUtils.Commands.cCommand)

        cApplicationStatusNotifier.SetStatusText("Running value chain equilibrium, please wait...", TriState.True)

        ' Switch to manual run mode
        Me.m_model.IsManualRunMode = True

        ' Reset cached results
        '    In manual mode the calling process becomes responsible for resetting results
        Me.m_result.Reset(cModel.eRunTypes.Equilibrium)
        ' Prepare view
        Me.SetViewMode(eViewModeType.Equilibrium)
        ' Run
        Me.m_model.RunEquilibrium(Me.m_data, Me.m_result)
        ' Process results
        Me.UpdateResults()

        ' Switch back to auto run mode
        Me.m_model.IsManualRunMode = False

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

    End Sub

    Private Sub OnUpdateRunEquilibrium(ByVal cmd As EwEUtils.Commands.cCommand)
        Dim csm As cCoreStateMonitor = Me.m_data.Core.StateMonitor
        cmd.Enabled = csm.HasEcosimLoaded And (Not csm.IsEcosimRunning)
    End Sub

#End Region ' Commands

#Region " Core messages "

    Private Sub EcosimTimestepHandler(ByVal iTime As Long, ByVal results As cEcoSimResults)
        ' JS 25Feb09: plug-in will handle this
        'Me.m_model.Run(Me.m_data, Me.m_result, CInt(iTime), results)
        'Me.UpdateResults()
    End Sub

#End Region ' Core messages

#End Region ' Events

#Region " Internals "

    Private m_bInitializing As Boolean = False

    Private Sub Initialize()

        Dim item As cCoreInputOutputBase = Nothing

        Me.m_bInitializing = True

        Me.m_plFlow.Init(Me.m_uic, Me.m_data, Nothing, Nothing)

        ' Populate fleet combo
        Me.m_tscmbFleets.Items.Clear()
        For iFleet As Integer = 0 To Me.m_data.Core.nFleets
            If iFleet = 0 Then
                item = Nothing
            Else
                item = Me.m_data.Core.FleetInputs(iFleet)
            End If
            Me.m_tscmbFleets.Items.Add(New cCoreComboItem(item))
        Next
        Me.m_tscmbFleets.SelectedIndex = 0

        Me.SetViewMode(Me.m_viewMode)

        Me.UpdateFilter()
        Me.UpdateControls()

        Me.m_bInitializing = False

        Me.UpdateResults()

    End Sub

    Private Sub UpdateFilter()
        Me.m_plFlow.SuspendLayout()
        Me.m_plFlow.RebuildFlow()
        Me.m_plFlow.Arrange()
        Me.m_plFlow.ResumeLayout()

        Me.UpdateResults()
    End Sub

    Private Sub UpdateResults()

        If Me.m_bInitializing Then Return

        Dim fleet As cFleetInput = Me.m_plFlow.FleetFilter
        Dim iFleet As Integer = 0

        If (fleet IsNot Nothing) Then
            iFleet = fleet.Index
        End If

        If Me.m_view IsNot Nothing Then
            Me.m_view.ShowResults(iFleet, Me.m_plFlow.GetFlowUnits(), Me.m_result)
        End If

    End Sub

    Private Sub UpdateControls()

        Me.m_bInUpdate = True

        Me.m_scResults.Panel1Collapsed = (Me.m_tsbShowFlow.Checked = False)
        Me.m_tsbEcopath.Checked = (Me.m_viewMode = eViewModeType.Ecopath)
        Me.m_tsbEcosim.Checked = (Me.m_viewMode = eViewModeType.Ecosim)
        Me.m_tsbEquilibrium.Checked = (Me.m_viewMode = eViewModeType.Equilibrium)

        Me.m_tscmbFleets.Enabled = (Me.m_data.Parameters.ResultsByFleet = True)
        Me.m_tscmbFleets.SelectedItem = Me.GetCoreComboItem(Me.m_plFlow.FleetFilter, Me.m_tscmbFleets)

        Me.m_tslblData.Visible = (Me.m_viewMode <> eViewModeType.Ecopath)
        Me.m_tscmbGraphData.Visible = (Me.m_viewMode <> eViewModeType.Ecopath)

        Me.m_bInUpdate = False

    End Sub

    Private Sub SetViewMode(ByVal viewMode As eViewModeType)

        Dim ctrl As ScrollableControl = Nothing

        ' Store view mode type
        Me.m_viewMode = viewMode

        ' Create new view
        Me.m_scResults.SuspendLayout()
        Me.m_scResults.Panel2.SuspendLayout()
        Me.m_scResults.Panel2.Controls.Clear()

        Select Case viewMode

            Case eViewModeType.Ecopath
                ctrl = New gridEcopathResult(Me.m_uic)

            Case eViewModeType.Ecosim
                ctrl = New ucEcosimGraph(Me.m_data, Me.m_uic)

            Case eViewModeType.Equilibrium
                ctrl = New ucEquilibriumGraph(Me.m_uic)

            Case Else
                Debug.Assert(False, "View mode {0} not supported", viewMode.ToString())

        End Select

        Debug.Assert(ctrl IsNot Nothing)
        Debug.Assert(TypeOf ctrl Is IResultView)
        Me.m_view = DirectCast(ctrl, IResultView)

        Debug.Assert(TypeOf ctrl Is Control)
        ctrl.Dock = Windows.Forms.DockStyle.Fill
        Me.m_scResults.Panel2.Controls.Add(ctrl)

        Me.m_scResults.Panel2.ResumeLayout()
        Me.m_scResults.ResumeLayout()

        ' Yippee
        Me.SetGraphData(Me.m_graphmode)
        Me.UpdateResults()
        Me.UpdateControls()

    End Sub

    Private Sub SetGraphData(ByVal graphmode As eGraphDataType)

        Me.m_graphmode = graphmode
        Me.UpdateControls()

        If Not TypeOf (Me.m_view) Is IGraphView Then Return

        Dim gv As IGraphView = DirectCast(Me.m_view, IGraphView)
        Dim strGraphTitle As String = ""
        Dim strXAxisLabel As String = CStr(IIf(Me.m_viewMode = eViewModeType.Equilibrium, "Effort", "Year"))
        Dim strYAxisLabel As String = ""
        Dim aUnitsYAxis() As cStyleGuide.eUnitType = New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Monetary}
        Dim avars() As cResults.eVariableType = Nothing

        Select Case graphmode

            Case eGraphDataType.CostRevenue
                strGraphTitle = "Revenue and Cost"
                strYAxisLabel = "Revenue and Cost ({0})"
                avars = New cResults.eVariableType() {cResults.eVariableType.RevenueTotal, _
                                                      cResults.eVariableType.Cost, _
                                                      cResults.eVariableType.Profit}

            Case eGraphDataType.Cost
                strGraphTitle = "Cost"
                strYAxisLabel = "Cost breakdown ({0})"
                avars = New cResults.eVariableType() {cResults.eVariableType.CostAgriculture, _
                                                      cResults.eVariableType.CostInput, _
                                                      cResults.eVariableType.CostManagementRoyaltyCertification, _
                                                      cResults.eVariableType.CostManagementRoyaltyCertificationObservers, _
                                                      cResults.eVariableType.CostRawmaterial}

            Case eGraphDataType.Revenue
                strGraphTitle = "Revenue"
                strYAxisLabel = "Revenue breakdown ({0})"
                avars = New cResults.eVariableType() {cResults.eVariableType.RevenueTickets, _
                                                      cResults.eVariableType.RevenueSubsidies, _
                                                      cResults.eVariableType.RevenueProductsMain, _
                                                      cResults.eVariableType.RevenueProductsOther, _
                                                      cResults.eVariableType.RevenueAgriculture}

            Case eGraphDataType.Jobs
                strGraphTitle = "Jobs"
                strYAxisLabel = "Jobs"
                avars = New cResults.eVariableType() {cResults.eVariableType.NumberOfJobsTotal, _
                                                      cResults.eVariableType.NumberOfJobsMaleTotal, _
                                                      cResults.eVariableType.NumberOfJobsFemaleTotal}
            Case eGraphDataType.Dependents
                strGraphTitle = "Dependents"
                strYAxisLabel = "Dependents"
                avars = New cResults.eVariableType() {cResults.eVariableType.NumberOfDependentsTotal, _
                                                      cResults.eVariableType.NumberOfWorkerDependents, _
                                                      cResults.eVariableType.NumberOfWorkerFemales, _
                                                      cResults.eVariableType.NumberOfWorkerMales, _
                                                      cResults.eVariableType.NumberOfOwnerMales, _
                                                      cResults.eVariableType.NumberOfOwnerFemales, _
                                                      cResults.eVariableType.NumberOfOwnerDependents}

            Case Else
                Debug.Assert(False)

        End Select

        gv.SetData(strGraphTitle, strXAxisLabel, Nothing, strYAxisLabel, aUnitsYAxis, avars)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, returns a cCoreComboItem for a given cCoreInputOutputBase
    ''' instance from a given combo box.
    ''' </summary>
    ''' <param name="source">The source to locate.</param>
    ''' <param name="cmb">The combo box to plunder.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function GetCoreComboItem(ByVal source As cCoreInputOutputBase, ByVal cmb As ToolStripComboBox) As cCoreComboItem
        Dim item As cCoreComboItem = Nothing
        For i As Integer = 0 To cmb.Items.Count - 1
            item = DirectCast(cmb.Items(i), cCoreComboItem)
            If Object.ReferenceEquals(source, item.Source) Then
                Return item
            End If
        Next
        Return Nothing
    End Function

#End Region ' Internals 

End Class
