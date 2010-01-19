#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports System.Text
Imports System.Reflection
Imports System.Threading
Imports EwEUtils.Core

#End Region ' Imports

Public Class cPluginPoint
    Implements EwEPlugin.Data.IDatabasePlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin
    Implements EwEPlugin.Data.IDataProducerPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.ISearchPlugin

#Region " Privates "

    Private m_core As EwECore.cCore = Nothing
    Private m_bInitOK As Boolean = False
    Private m_form As frmMain = Nothing
    Private m_data As cData = Nothing
    Private m_model As cModel = Nothing
    Private m_result As cResults = Nothing
    Private m_mhEcopath As cMessageHandler = Nothing
    Private m_syncobj As SynchronizationContext = Nothing

    ' Data exchange
    Private m_dataBroadcaster As EwEPlugin.Data.IDataBroadcaster = Nothing
    ''' <summary>Ooooh, that was long ago...</summary>
    Private m_ddx As cPluginData = Nothing

#End Region ' Privates

#Region " Plugin point implementation "

    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

        ' Sanity checks
        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        Debug.Assert(Me.m_bInitOK = False)

        ' To prevent multiple calls
        Me.m_bInitOK = False

        Try
            If TypeOf core Is EwECore.cCore Then

                Me.m_core = DirectCast(core, EwECore.cCore)
                Me.m_ddx = New cPluginData(Assembly.GetExecutingAssembly().GetName().Name, Me.Name)
                Me.m_data = New cData(Me.m_core)
                Me.m_model = New cModel()
                Me.m_result = New cResults(Me.m_data)
                Me.m_syncobj = SynchronizationContext.Current

                If (Me.m_syncobj Is Nothing) Then
                    Me.m_syncobj = New SynchronizationContext()
                End If
                Me.m_mhEcopath = New cMessageHandler(AddressOf EcopathMessageHandler, _
                                                     eCoreComponentType.EcoPath, _
                                                     eMessageType.DataAddedOrRemoved, _
                                                     Me.m_syncobj)

                ' Done initializing
                Me.m_bInitOK = True

            Else
                'some kind of a message
                Return
            End If

        Catch ex As Exception

            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return

        End Try

    End Sub

    Private Sub EcopathMessageHandler(ByRef msg As cMessage)

        Try
            ' Take care of deleted groups and fleets
        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

#Region " GUI "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Internal name of the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "ValueChain"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in description.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Dim sb As New StringBuilder()
            sb.AppendLine("ValueChain - an economic fisheries model for EwE6")
            sb.AppendLine("")
            sb.AppendLine("This plug-in calculates a range of economic and social-economic indicators based on Ecopath and Ecosim data, where users can define economic systems as value chains of desired complexity.")
            sb.AppendLine("")
            sb.AppendLine("This plug-in was developed in conjunction with the ECOST project (http://www.ird.fr/ecostproject), and was partially funded by the North Sea Centre in Hirtshals, Denmark.")
            Return sb.ToString()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Image to display for controls activating the Value Chain plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing ' My.Resources.ValueChain_32x32
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Text to be displayed for the controls activating the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Value chain"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre, ECOST project, North Sea Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:v.christensen@fisheries.ubc.ca,j.steenbeek@fisheries.ubc.ca"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Text to be displayed on tooltips for controls activating the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return Me.ControlText()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Handler of event, invoked by controls activating the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False

        'Interface item has been clicked
        'Show the Ecotroph interface
        If m_bInitOK Then

            ' Does form still exist?
            If Not Me.HasInterface() Then
                ' #No: create it
                Me.m_form = New frmMain(Me, Me.ControlText)
            End If

            ' JS 04may09: do not show form; the loading framework is responsible for this
            '' Activate the form
            ' Me.m_form.Show()

            ' Pass form reference back to calling app
            f = Me.m_form
        Else
            Debug.Assert(False, "Plugin was not initialized properly.")
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The core state that must be met for this plug-in to be enabled.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Scientific Interface menu item location for the menu item to activate
    ''' the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property MenuItemLocation() As String _
        Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return ""
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Scientific Interface navigation tree location for the tree node that 
    ''' will activate the plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NavigationTreeItemLocation() As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndParameterization|ndEcopathOutputTools"
        End Get
    End Property

#End Region ' GUI

#Region " Database integration "


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when an EwE6 model is loaded. 
    ''' Handled toload the Value Chain model corresponding to an EwE model.
    ''' </summary>
    ''' <param name="dataSource">The loaded datasource.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function LoadModel(ByVal dataSource As Object) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.LoadModel

        Dim ds As EwECore.DataSources.IEcopathDataSource = DirectCast(dataSource, EwECore.DataSources.IEcopathDataSource)

        ' Sanity checks
        Debug.Assert(Me.m_data.IsChanged() = False)

        Return Me.m_data.Load()

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when an EwE6 model is saved. 
    ''' Handled to save the Value Chain model corresponding to an EwE model.
    ''' </summary>
    ''' <param name="dataSource">The loaded datasource.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function SaveModel(ByVal dataSource As Object) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.SaveModel
        Return Me.m_data.Save()
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, no longer called but kept for compliancy.
    ''' </summary>
    ''' <param name="strName">The name of the datasource.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Open(ByVal strName As String) As Boolean _
        Implements EwEPlugin.Data.IDatabasePlugin.Open
        ' NOP
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when an EwE6 model is closed. 
    ''' Handled to terminate the Value Chain model corresponding to an EwE model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Close() _
        Implements EwEPlugin.Data.IDatabasePlugin.Close
        Me.m_data.Close()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when an EwE6 model is polled for
    ''' unsaved changes. Handled to inform the EwE6 engine that the Value Chain 
    ''' model has unsaved changes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsModified() As Boolean _
        Implements EwEPlugin.Data.IDatabasePlugin.IsModified
        Return Me.m_data.IsChanged()
    End Function

#End Region ' Database integration

#Region " Ecopath integration "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when Ecopath has ran.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) _
        Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted

        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return
        ' Abort if not allowed to run with Ecopath
        If (parms.RunWithEcopath = False) Then Return

        ' Running in auto mode?
        If (Me.m_model.IsManualRunMode = False) Then
            ' #Yes: prepare results for receiving Ecopath results
            Me.m_result.Reset(cModel.eRunTypes.Ecopath)
        End If

        ' Prepare data
        Me.m_data.InitRun()
        ' Run a single time step
        Me.m_model.RunTimeStep(Me.m_data, Me.m_result)

        ' Send out data
        Me.BroadcastResults(1)

    End Sub


#End Region ' Ecopath integration

#Region " Ecosim integration "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called just before Ecosim will run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized

        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return
        ' Abort if not allowed to run with Ecosim
        If (parms.RunWithEcosim = False) Then Return

        ' Running in auto mode?
        If (Me.m_model.IsManualRunMode = False) Then
            ' #Yes: prepare results for receiving Ecosim results
            Me.m_result.Reset(cModel.eRunTypes.Ecosim)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called at end of every Ecosim timestep.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, _
                          ByVal EcosimDatastructures As Object, _
                          ByVal iTimeStep As Integer, _
                          ByVal ecosimresults As Object) _
        Implements IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return
        ' Abort if not allowed to run with Ecosim
        If (parms.RunWithEcosim = False) Then Return

        ' Run VC model
        Me.m_model.RunTimeStep(Me.m_data, Me.m_result, iTimeStep, DirectCast(ecosimresults, cEcoSimResults))
        ' Send out data
        Me.BroadcastResults(iTimeStep)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when Ecosim has finished running.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub EcosimRunCompleted(ByVal EcosimDatastructures As Object) _
        Implements IEcosimRunCompletedPlugin.EcosimRunCompleted

        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return
        ' Abort if not allowed to run with Ecosim
        If (parms.RunWithEcosim = False) Then Return

        If (Me.m_dataBroadcaster IsNot Nothing) Then
            Me.m_dataBroadcaster.BroadcastData(Me.Name, Me.m_ddx)
        End If

    End Sub

#End Region ' Ecosim integration

#Region " Data Exchange "

    Private Sub BroadcastResults(ByVal iTimeStep As Integer)

        If (Me.m_dataBroadcaster IsNot Nothing) Then

            ' Fill exchange data based on the type of computed results
            Select Case Me.m_result.RunType
                Case cModel.eRunTypes.Ecopath
                    Me.m_ddx.m_runType = New cEcopathRunType()
                Case cModel.eRunTypes.Ecosim
                    Me.m_ddx.m_runType = New cEcosimRunType()
            End Select

            Me.m_ddx.Resize(Me.m_data.Core.nFleets)
            Me.m_ddx.m_iTimeStep = iTimeStep

            Me.Populate(DirectCast(Me.m_ddx.Total, cPluginData.cVCEconomicData), iTimeStep, 0)
            For iFleet As Integer = 1 To Me.m_data.Core.nFleets - 1
                Me.Populate(DirectCast(Me.m_ddx.Subtotal(iFleet), cPluginData.cVCEconomicData), iTimeStep, iFleet)
            Next iFleet

            Me.m_dataBroadcaster.BroadcastData(Me.Name, Me.m_ddx)
        End If

    End Sub

    Private Sub Populate(ByVal data As cPluginData.cVCEconomicData, ByVal iTimeStep As Integer, ByVal iFleet As Integer)

        data.m_sCost = Me.m_result.GetTimeStepTotal(cResults.eVariableType.Cost, iTimeStep, Nothing, iFleet)
        data.m_sCostInput = Me.m_result.GetTimeStepTotal(cResults.eVariableType.CostRawmaterial, iTimeStep, Nothing, iFleet)
        data.m_sCostLicenseObservers = Me.m_result.GetTimeStepTotal(cResults.eVariableType.CostManagementRoyaltyCertificationObservers, iTimeStep, Nothing, iFleet)
        data.m_sCostSalariesShares = Me.m_result.GetTimeStepTotal(cResults.eVariableType.CostSalariesShares, iTimeStep, Nothing, iFleet)
        data.m_sCostTaxes = Me.m_result.GetTimeStepTotal(cResults.eVariableType.CostTaxes, iTimeStep, Nothing, iFleet)
        data.m_sCostTotalInputOther = Me.m_result.GetTimeStepTotal(cResults.eVariableType.CostTotalInputOther, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfDependentsTotal = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfDependentsTotal, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfJobsFemaleTotal = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfJobsFemaleTotal, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfJobsMaleTotal = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfJobsMaleTotal, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfJobsTotal = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfJobsTotal, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfOwnerDependents = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfOwnerDependents, iTimeStep, Nothing, iFleet)
        data.m_sNumberOfWorkerDependents = Me.m_result.GetTimeStepTotal(cResults.eVariableType.NumberOfWorkerDependents, iTimeStep, Nothing, iFleet)
        data.m_sProduction = Me.m_result.GetTimeStepTotal(cResults.eVariableType.Production, iTimeStep, Nothing, iFleet)
        data.m_sProductionLive = Me.m_result.GetTimeStepTotal(cResults.eVariableType.ProductionLive, iTimeStep, Nothing, iFleet)
        data.m_sProfit = Me.m_result.GetTimeStepTotal(cResults.eVariableType.Profit, iTimeStep, Nothing, iFleet)
        data.m_sRevenueProductsMain = Me.m_result.GetTimeStepTotal(cResults.eVariableType.RevenueProductsMain, iTimeStep, Nothing, iFleet)
        data.m_sRevenueProductsOther = Me.m_result.GetTimeStepTotal(cResults.eVariableType.RevenueProductsOther, iTimeStep, Nothing, iFleet)
        data.m_sRevenueSubsidies = Me.m_result.GetTimeStepTotal(cResults.eVariableType.RevenueSubsidies, iTimeStep, Nothing, iFleet)
        data.m_sRevenueTotal = Me.m_result.GetTimeStepTotal(cResults.eVariableType.RevenueTotal, iTimeStep, Nothing, iFleet)
        data.m_sThroughput = Me.m_result.GetTimeStepTotal(cResults.eVariableType.TotalUtility, iTimeStep, Nothing, iFleet)

    End Sub

    Public Sub Broadcaster(ByVal broadcaster As EwEPlugin.Data.IDataBroadcaster) _
        Implements EwEPlugin.Data.IDataProducerPlugin.Broadcaster

        Me.m_dataBroadcaster = broadcaster

    End Sub

    Public Function IsDataAvailable(ByVal strDataName As String, ByVal runType As IRunType) As Boolean _
        Implements EwEPlugin.Data.IDataProducerPlugin.IsDataAvailable

        Dim bIsAvailable As Boolean = False
        Try
            bIsAvailable = (String.Compare(strDataName, Me.Name, True) = 0) And Me.SupportsRunType(runType)
        Catch ex As Exception
            bIsAvailable = False
        End Try
        Return bIsAvailable

    End Function

    Public Function IsDataAvailable(ByVal typeData As System.Type, ByVal runType As IRunType) As Boolean _
        Implements EwEPlugin.Data.IDataProducerPlugin.IsDataAvailable

        Dim bIsAvailable As Boolean = False
        Try
            bIsAvailable = (typeData Is GetType(IEconomicData)) And Me.SupportsRunType(runType)
        Catch ex As Exception
            bIsAvailable = False
        End Try
        Return bIsAvailable

    End Function

    Public Function GetDataByName(ByVal strDataName As String, _
                                  ByRef data As EwEPlugin.Data.IPluginData) As Boolean _
        Implements EwEPlugin.Data.IDataProducerPlugin.GetDataByName

        data = Nothing
        If String.Compare(strDataName, Me.Name, True) = 0 Then
            data = Me.m_ddx
        End If

        Return (data IsNot Nothing)

    End Function

    Public Function GetDataByType(ByVal typeData As System.Type, _
                                  ByRef data As EwEPlugin.Data.IPluginData) As Boolean _
        Implements EwEPlugin.Data.IDataProducerPlugin.GetDataByType

        data = Nothing
        If (typeData Is GetType(IEconomicData)) Then
            data = Me.m_ddx
        End If

        Return (data IsNot Nothing)

    End Function

#End Region ' Data exchange

#Region " Search "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when a search is initialized.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub SearchInitialized(ByVal SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.SearchInitialized

        Dim ds As cSearchDatastructures = DirectCast(SearchDatastructures, cSearchDatastructures)
        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return

        ' Only respond to fishing policy search when allowed to respond
        If (parms.RunWithFishingPolicySearch = False) Then Return
        ' Only respond to fishing policy search
        If (ds.SearchMode <> eSearchModes.FishingPolicy) Then Return

        ' Reset values that this plug-in will (hopefully) deliver.
        'VC090402: updated the blowe to use the value chain searchDS parameters (which is what I need)
        ds.profit = 0
        ds.totval = 0
        ds.Employ = 0
        'ds.SectorTotalValue = 0
        'ds.SectorProfit = 0
        'ds.SectorJobs = 0

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when a search is starting.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub SearchIterationsStarting() Implements _
          EwEPlugin.ISearchPlugin.SearchIterationsStarting

        ' NOP

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point implementation, called when a search step has been 
    ''' performed. Implemented to provide economic search results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub PostRunSearchResults(ByVal SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.PostRunSearchResults

        Dim ds As cSearchDatastructures = DirectCast(SearchDatastructures, cSearchDatastructures)
        Dim parms As cParameters = Me.m_data.Parameters

        ' Abort if no params
        If (parms Is Nothing) Then Return

        ' Only respond to fishing policy search when allowed to respond
        If (parms.RunWithFishingPolicySearch = False) Then Return
        ' Only respond to fishing policy search
        If (ds.SearchMode <> eSearchModes.FishingPolicy) Then Return

        'ds.SectorProfit = Me.Results.GetTotal(cEcostResults.eVariableType.Profit)
        'ds.SectorTotalValue = Me.Results.GetTotal(cEcostResults.eVariableType.RevenueTotal)
        'ds.SectorJobs = Me.Results.GetTotal(cEcostResults.eVariableType.NumberOfJobsTotal)

    End Sub

#End Region ' Search

#End Region ' Plugin point implementation

#Region " Exhibitionism "

    Public ReadOnly Property Data() As cData
        Get
            Return Me.m_data
        End Get
    End Property

    Public ReadOnly Property Model() As cModel
        Get
            Return Me.m_model
        End Get
    End Property

    Public ReadOnly Property Results() As cResults
        Get
            Return Me.m_result
        End Get
    End Property

    Public ReadOnly Property Core() As cCore
        Get
            Return Me.m_core
        End Get
    End Property

#End Region ' Exhibitionism

#Region " Helpers "

    Private Function HasInterface() As Boolean
        If Me.m_form Is Nothing Then Return False
        If Me.m_form.IsDisposed Then Return False
        Return True
    End Function

    Private Function SupportsRunType(ByVal runType As IRunType) As Boolean

        Dim parms As cParameters = Me.m_data.Parameters

        If parms IsNot Nothing Then
            If TypeOf (runType) Is cEcopathRunType Then
                Return parms.RunWithEcopath
            ElseIf TypeOf (runType) Is cEcosimRunType Then
                Return parms.RunWithEcosim
            ElseIf TypeOf (runType) Is cFishingPolicySearchRunType Then
                Return parms.RunWithFishingPolicySearch
            End If
        End If
        Return False

    End Function

#End Region

End Class
