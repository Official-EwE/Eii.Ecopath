#Region " Imports "

Option Strict On

Imports System.Reflection
Imports EwECore
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cEwENetworkAnalysisPlugin
    Inherits cNavTreeControlPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPlugin
    Implements EwEPlugin.Data.IDataProducerPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements IDisposedPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_core As cCore = Nothing
    Private m_bInitOK As Boolean = False
    Private m_bDataEnabled As Boolean = True

    ''' <summary>
    ''' Network Analysis manager. Provide access to Network Analysis methods.
    ''' </summary>
    ''' <remarks>Because the plugin handles interactions with the core it manages the life span of the network manager and the interface. 
    ''' The plugin is responsible for telling the network manager when a plugin point has been invoked by the core. 
    ''' The plugin will pass a network manager reference to the interface when the user has clicked the plugins menu item or tree node in the main interface.</remarks>
    Private m_manager As cNetworkManager = Nothing

    ''' <summary>Interface form.</summary>
    Private m_frmNA As frmNetworkAnalysis = Nothing
    ''' <summary>NA remote control.</summary>
    Private m_remote As cNetworkAnalysisRemote = Nothing
    ''' <summary>Ooooh, that was long ago...</summary>
    Private m_ddx As cEwENetworkAnalysisData = Nothing

#End Region ' Private vars

#Region " Singleton "

    Private Shared _inst_ As cEwENetworkAnalysisPlugin = Nothing

    Public Sub New()
        If _inst_ Is Nothing Then
            _inst_ = Me
        End If
    End Sub

    Public Shared Function SwitchForm(ByVal page As frmNetworkAnalysis.eNetworkAnalysisPageTypes) As frmNetworkAnalysis

        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False
        Dim frm As frmNetworkAnalysis = Nothing

        'Interface item has been clicked
        'Show the Ecotroph interface
        If cEwENetworkAnalysisPlugin._inst_.m_bInitOK Then

            ' Does form still exist?
            If Not cEwENetworkAnalysisPlugin._inst_.HasUI() Then
                ' #No: create it
                frm = New frmNetworkAnalysis(cEwENetworkAnalysisPlugin._inst_.m_manager, cEwENetworkAnalysisPlugin._inst_.m_uic)
                cEwENetworkAnalysisPlugin._inst_.m_frmNA = frm
            Else
                frm = cEwENetworkAnalysisPlugin._inst_.m_frmNA
            End If
            frm.ShowPage(page)
        Else
            Debug.Assert(False, "Plugin was not initialized properly.")
        End If
        Return frm

    End Function

#End Region ' Singleton

#Region " Generic "

    Public Overrides ReadOnly Property Name() As String 
        Get
            Return "nwa00"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As System.Drawing.Image
        Get
            Return SharedResources.nav4_output_extend
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText() As String
        Get
            Return My.Resources.NAVITEM_ROOT
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot
        End Get
    End Property

    Public Overrides Function FormPage() As frmNetworkAnalysis.eNetworkAnalysisPageTypes
        Return frmNetworkAnalysis.eNetworkAnalysisPageTypes.Credits
    End Function

#End Region ' Generic

#Region " Core "

    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    ''' <param name="core"></param>
    Public Overrides Sub Initialize(ByVal core As Object)

        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                Me.m_core = DirectCast(core, EwECore.cCore)
                Me.m_ddx = New cEwENetworkAnalysisData(Assembly.GetExecutingAssembly().GetName().Name, Me.Name)

                Me.m_manager = New cNetworkManager
                Me.m_manager.Init(m_core)

                Me.m_bInitOK = True
                'System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try

    End Sub

    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose

        If Me.HasUI() Then
            Me.m_frmNA.Close()
            Me.m_frmNA = Nothing
        End If

        If (Me.m_remote IsNot Nothing) Then
            Me.m_remote.Detach()
        End If

        Me.m_manager = Nothing
        Me.m_bInitOK = False

    End Sub

#End Region ' Core

#Region " Ecopath "

    ''' <summary>
    ''' Called by the core when Ecopath has run successfuly 
    ''' </summary>
    ''' <param name="EcopathDataStructures"></param>
    ''' <remarks></remarks>
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) _
        Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted

        'test the error handling 
        ' Throw New Exception("Error Test from Network Analysis Plugin.")

        Debug.Assert(TypeOf EcopathDataStructures Is EwECore.cEcopathDataStructures, Me.ToString & _
                            ".EcopathRan() argument EcopathDataStructure is not a cEcopathDataStructures object.")
        Try
            If TypeOf EcopathDataStructures Is EwECore.cEcopathDataStructures Then
                'set the Ecopath data in the network manager object
                'this is the data the Network analysis will be run on
                m_manager.EcopathData = DirectCast(EcopathDataStructures, EwECore.cEcopathDataStructures)
                'Bug 252 fix by joeh
                'Add
                m_manager.IsMainNetworkRun = False
                m_manager.IsRequiredPrimaryProdRun = False
                m_manager.IsEcosimNetworkRun = False
                'End Add

                If Me.m_manager.RunWithEcopath Then
                    Me.m_manager.RunMainNetwork()
                    Me.BroadcastResults()
                End If

                'System.Console.WriteLine(Me.ToString & ".EcopathRan() Successfull.")
            Else

                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() argument EcopathDataStructure is not a cEcopathDataStructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() Error: " & ex.Message, _
                            EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))

        End Try

    End Sub

#End Region ' Ecopath

#Region " GUI "

    Public Sub UIContext(ByVal uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        ' Cleanup
        If Me.m_uic IsNot Nothing Then
            Me.m_remote.Detach()
            Me.m_remote = Nothing
        End If
        ' Update
        Me.m_uic = DirectCast(uic, cUIContext)
        ' Apply
        If Me.m_uic IsNot Nothing Then
            Me.m_remote = New cNetworkAnalysisRemote()
            Me.m_remote.Attach(Me.m_uic, Me.m_manager)
        End If
    End Sub

#End Region ' GUI

#Region " Ecosim "

    ''' <summary>
    ''' Ecosim is about to enter the time loop. All the data has been initialized
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks></remarks>
    Public Sub EcosimRunInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized

        Debug.Assert(TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures, Me.ToString & _
                            ".EcosimRunInitialized() argument EcosimDatastructures is not a cEcosimDatastructures object.")

        'Only initialize the Ecosim Network Analysis if it is turned on
        If Not m_manager.UseEcosimNetwork Then
            Return
        End If

        Try
            If TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures Then
                'set the EcosimData data in the network manager object
                'this is the data the Network analysis will be run on
                m_manager.EcosimData = DirectCast(EcosimDatastructures, EwECore.cEcosimDatastructures)

                'm_NetworkManager.bEcoismNetwork = True

                'Initialize the Network Analysis for Ecosim
                m_manager.InitNetworkForEcosim()

                'System.Console.WriteLine(Me.ToString & ".EcosimRunInitialized() called.")
            Else

                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcosimRunInitialized() argument EcosimDatastructures is not a cEcosimDatastructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcosimRunInitialized() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))

        End Try


    End Sub

    ''' <summary>
    ''' Ecosim has completed the time step 'iTime' all computations related to this time step have been completed.
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <param name="iTime"></param>
    ''' <param name="Ecosimresults"></param>
    ''' <remarks></remarks>
    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, _
                                 ByVal EcosimDatastructures As Object, _
                                 ByVal iTime As Integer, ByVal Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try
            'Only run the Ecosim Network Analysis if it is turned on
            If Not m_manager.UseEcosimNetwork Then
                Return
            End If

            If TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures Then
                'set the EcosimData data in the network manager object
                'this is the data the Network analysis will be run on
                Dim esData As cEcosimDatastructures = DirectCast(EcosimDatastructures, EwECore.cEcosimDatastructures)
                m_manager.EcosimTimeStep(BiomassAtTimestep, esData, iTime)
            Else
                Debug.Assert(False, Me.ToString & ".EcosimEndTimeStep() ")
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, ex.StackTrace)
            'eat the exception
        End Try

    End Sub

    Public Sub EcosimRunCompleted(ByVal EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunCompletedPlugin.EcosimRunCompleted

        Try

            If Me.m_manager.RunWithEcosim Then
                If Me.m_manager.RunEcosimNetwork() Then
                    Me.BroadcastResults()
                End If
            End If

        Catch ex As Exception

        End Try
    End Sub

#End Region ' Ecosim

#Region " Data exchange "

    Private m_broadcaster As IDataBroadcaster = Nothing

    Public Sub Broadcaster(ByVal broadcaster As IDataBroadcaster) _
        Implements IDataProducerPlugin.Broadcaster
        Me.m_broadcaster = broadcaster
    End Sub

    Public Function ProducesData(ByVal typeData As System.Type, Optional ByVal runType As IRunType = Nothing) As Boolean _
        Implements IDataProducerPlugin.IsDataAvailable
        Return (typeData Is GetType(INetworkAnalysisData))
    End Function

    Public Function IsEnabled1() As Boolean Implements EwEPlugin.Data.IDataProducerPlugin.IsEnabled
        Return m_bDataEnabled
    End Function

    Public Function SetEnabled(ByVal bEnable As Boolean) As Boolean Implements EwEPlugin.Data.IDataProducerPlugin.SetEnabled
        Me.m_bDataEnabled = bEnable
    End Function

    Public Sub SetEnabled(ByVal typeData As System.Type, ByVal runType As IRunType, ByVal bEnabled As Boolean) _
        Implements EwEPlugin.Data.IDataProducerPlugin.SetEnabled

        If Not (typeData Is GetType(INetworkAnalysisData)) Then Return

        If TypeOf runType Is cEcopathRunType Then
            Me.m_manager.RunWithEcopath = bEnabled
        End If

        If TypeOf runType Is cEcosimRunType Then
            Me.m_manager.RunWithEcosim = bEnabled
        End If

    End Sub

    Public Function IsEnabled(ByVal typeData As System.Type, ByVal runType As IRunType) As Boolean _
        Implements EwEPlugin.Data.IDataProducerPlugin.IsEnabled

        If Not (typeData Is GetType(INetworkAnalysisData)) Then Return False

        If TypeOf runType Is cEcopathRunType Then
            Return Me.m_manager.RunWithEcopath
        End If

        If TypeOf runType Is cEcosimRunType Then
            Return Me.m_manager.RunWithEcosim
        End If

    End Function

    Public Function GetDataByType(ByVal typeData As System.Type, ByRef data As IPluginData) As Boolean _
        Implements IDataProducerPlugin.GetDataByType

        Try
            If typeData Is GetType(INetworkAnalysisData) Then
                Me.PopulateData()
                data = Me.m_ddx
            End If
        Catch ex As Exception
            data = Nothing
        End Try
        Return (data IsNot Nothing)

    End Function

    Private Sub PopulateData()

        ' Run network if needed
        If Not Me.m_manager.IsMainNetworkRun Then
            Me.m_manager.RunMainNetwork()
        End If

        Me.m_ddx.Ascendancy(1, 1) = m_manager.AscendancyImportTotal
        Me.m_ddx.Ascendancy(2, 1) = m_manager.AscendancyImportPer
        Me.m_ddx.Ascendancy(3, 1) = m_manager.OverheadImportTotal
        Me.m_ddx.Ascendancy(4, 1) = m_manager.OverheadImportPer
        Me.m_ddx.Ascendancy(5, 1) = m_manager.CapacityImportTotal
        Me.m_ddx.Ascendancy(6, 1) = m_manager.CapacityImportPer

        Me.m_ddx.Ascendancy(1, 2) = m_manager.AscendancyInternalFlowTotal
        Me.m_ddx.Ascendancy(2, 2) = m_manager.AscendancyInternalFlowPer
        Me.m_ddx.Ascendancy(3, 2) = m_manager.OverheadFlowTotal
        Me.m_ddx.Ascendancy(4, 2) = m_manager.OverheadFlowPer
        Me.m_ddx.Ascendancy(5, 2) = m_manager.CapacityFlowTotal
        Me.m_ddx.Ascendancy(6, 2) = m_manager.CapacityFlowPer

        Me.m_ddx.Ascendancy(1, 3) = m_manager.AscendancyExportTotal
        Me.m_ddx.Ascendancy(2, 3) = m_manager.AscendancyExportPer
        Me.m_ddx.Ascendancy(3, 3) = m_manager.OverheadExportTotal
        Me.m_ddx.Ascendancy(4, 3) = m_manager.OverheadExportPer
        Me.m_ddx.Ascendancy(5, 3) = m_manager.CapacityExportTotal
        Me.m_ddx.Ascendancy(6, 3) = m_manager.CapacityExportPer

        Me.m_ddx.Ascendancy(1, 4) = m_manager.AscendancyRespTotal
        Me.m_ddx.Ascendancy(2, 4) = m_manager.AscendancyRespPer
        Me.m_ddx.Ascendancy(3, 4) = m_manager.OverheadRespTotal
        Me.m_ddx.Ascendancy(4, 4) = m_manager.OverheadRespPer
        Me.m_ddx.Ascendancy(5, 4) = m_manager.CapacityRespTotal
        Me.m_ddx.Ascendancy(6, 4) = m_manager.CapacityRespPer

        Me.m_ddx.Ascendancy(1, 5) = m_manager.AscendancyTotalsTotal
        Me.m_ddx.Ascendancy(2, 5) = m_manager.AscendancyTotalsPer
        Me.m_ddx.Ascendancy(3, 5) = m_manager.OverheadTotalsTotal
        Me.m_ddx.Ascendancy(4, 5) = m_manager.OverheadTotalsPer
        Me.m_ddx.Ascendancy(5, 5) = m_manager.CapacityTotalsTotal
        Me.m_ddx.Ascendancy(6, 5) = m_manager.CapacityTotalsPer

    End Sub

    Private Sub BroadcastResults()

        If (Me.m_broadcaster IsNot Nothing) And (Me.m_bDataEnabled = True) Then
            Me.PopulateData()
            Me.m_broadcaster.BroadcastData(Me.Name, Me.m_ddx)
        End If

    End Sub

#End Region ' Data exchange

#Region " Internal helpers "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; states if the plug-in Form has been initialized as is
    ''' ready to be used.
    ''' </summary>
    ''' <returns>True if form is initialized and is ready to be used.</returns>
    ''' -----------------------------------------------------------------------
    Private Function HasUI() As Boolean

        ' No form? Whoah!
        If (Me.m_frmNA Is Nothing) Then Return False
        ' Form is ready to be used if it has not been disposed yet
        Return (Me.m_frmNA.IsDisposed = False)

    End Function

#End Region ' Internal helpers

End Class
