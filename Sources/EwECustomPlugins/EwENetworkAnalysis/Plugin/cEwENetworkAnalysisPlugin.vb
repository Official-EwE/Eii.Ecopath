'==============================================================================
'
' $Log: cEwENetworkAnalysisPlugin.vb,v $
' Revision 1.9  2009/05/19 13:42:21  jeroens
' Uses IDisposePlugin, remote controller
'
' Revision 1.8  2009/05/04 22:26:29  jeroens
' Loading framework will show form
'
' Revision 1.7  2009/05/02 03:07:49  jeroens
' Minor housekeeping
'
' Revision 1.6  2009/05/01 17:50:37  jeroens
' Greatly simplified content management
'
' Revision 1.5  2009/04/16 19:31:32  jeroens
' Added IsDataAvailable
'
' Revision 1.4  2009/01/21 19:11:01  jeroens
' Uses changed plug-in data exchange structure
'
' Revision 1.3  2009/01/16 18:30:27  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/10/07 21:20:56  jeroens
' Implemented data exchange plugin structure
'
' Revision 1.1  2008/09/26 07:31:00  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports EwEUtils.Commands

#End Region ' Imports

Public Class cEwENetworkAnalysisPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements IDataProducerPlugin
    Implements IDisposedPlugin
    'at this time we do not need these plugin points
    'Implements EwEPlugin.IEcosimRunCompletedPlugin
    'Implements EwEPlugin.IEcosimBeginTimestepPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_bInitOK As Boolean = False

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

#End Region ' Private vars

#Region " Core "

    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    ''' <param name="core"></param>
    ''' <remarks></remarks>
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize

        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                Me.m_core = DirectCast(core, EwECore.cCore)

                Me.m_manager = New cNetworkManager
                Me.m_manager.Init(m_core)

                Me.m_remote = New cNetworkAnalysisRemote()
                Me.m_remote.Attach(Me.m_manager)

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
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted

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
                m_manager.IsEcosimNetworkRan = False
                'End Add

                'System.Console.WriteLine(Me.ToString & ".EcopathRan() Successfull.")
            Else

                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() argument EcopathDataStructure is not a cEcopathDataStructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, EwECore.eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, EwECore.eCoreComponentType.Core, EwECore.eMessageImportance.Warning))

        End Try

    End Sub

#End Region ' Ecopath

#Region " Generic "

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ControlText()
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Network analysis plug-in"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property

#End Region ' Generic

#Region " GUI "

    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Network analysis plug-in"
        End Get
    End Property

    ''' <summary>
    ''' Menu Item or Tree node clicked
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>This will handle click events from all interface controls</remarks>
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        'Interface item has been clicked
        'Show the Main Network interface
        If m_bInitOK Then

            ' Create form when not ready
            If Not Me.HasUI() Then
                Me.m_frmNA = New frmNetworkAnalysis(m_manager)
            End If

            ' JS 05may09: do not show form; the loading framework should take care of this
            '' Activate the form
            'Me.m_frmNA.Show()

            ' Pass form reference back to calling app
            f = Me.m_frmNA

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel
                'NOP
            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu
                'NOP
            End If

        Else
            Debug.Assert(False, "Network Analysis plugin was not initialized properly.")
        End If

    End Sub

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            'this will add a new menu item to the menu bar instead of having this as a sub menu of a Plugins menu
            'not what we want but good enough for testing
            Return "EcopathToolStripMenuItem"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState() As eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            'this will put the navigation item at the end of the tree as top level node 
            'Not the best place there should be a Plugins node and all plugins should go under it
            Return "ndParameterization|ndEcopathOutputTools"
        End Get
    End Property

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
                                            , EwECore.eMessageType.ErrorEncountered, EwECore.eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcosimRunInitialized() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, EwECore.eCoreComponentType.Core, EwECore.eMessageImportance.Warning))

        End Try


    End Sub

    ''' <summary>
    ''' Ecosim has completed the time step 'iTime' all computations related to this time step have been completed.
    ''' </summary>
    ''' <param name="EcosimDatastructures"></param>
    ''' <param name="iTime"></param>
    ''' <param name="Ecosimresults"></param>
    ''' <remarks></remarks>
    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer, ByVal Ecosimresults As Object) Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep
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

#End Region ' Ecosim

#Region " Data exchange "

    Private Class cNetworkAnalysisData

        Implements IPluginData
        Implements INetworkAnalysisData

        Private m_strAssemblyName As String = ""
        Private m_strPluginName As String = ""
        Private m_assAscendancy As Single(,) = Nothing

        Public Sub New(ByVal strAssemblyName As String, ByVal strPluginName As String, ByVal assAscendancy As Single(,))
            Me.m_strAssemblyName = strAssemblyName
            Me.m_strPluginName = strPluginName
            Me.m_assAscendancy = assAscendancy
        End Sub

        Public ReadOnly Property AssemblyName() As String _
            Implements IPluginData.AssemblyName
            Get
                Return Me.m_strAssemblyName
            End Get
        End Property

        Public ReadOnly Property PluginName() As String _
            Implements IPluginData.PluginName
            Get
                Return Me.m_strPluginName
            End Get
        End Property

        Public ReadOnly Property Ascendancy() As Single(,) _
            Implements INetworkAnalysisData.Ascendancy
            Get
                Return Me.m_assAscendancy
            End Get
        End Property

        Public ReadOnly Property RunType() As IRunType _
            Implements IPluginData.RunType
            Get
                Return Nothing
            End Get
        End Property

    End Class

    Private m_broadcaster As IDataBroadcaster = Nothing

    Public Sub Broadcaster(ByVal broadcaster As IDataBroadcaster) _
        Implements IDataProducerPlugin.Broadcaster

        Me.m_broadcaster = broadcaster

    End Sub

    Public Function ProducesData(ByVal strDataName As String, ByVal runType As IRunType) As Boolean _
        Implements IDataProducerPlugin.IsDataAvailable
        Return (strDataName = "AscendancyTotal")
    End Function

    Public Function ProducesData(ByVal typeData As System.Type, ByVal runType As IRunType) As Boolean _
        Implements IDataProducerPlugin.IsDataAvailable
        Return (typeData Is GetType(INetworkAnalysisData))
    End Function

    Public Function GetDataByName(ByVal strDataName As String, ByRef data As IPluginData) As Boolean _
            Implements IDataProducerPlugin.GetDataByName

        Try
            Select Case strDataName
                Case "AscendancyTotal"
                    data = Me.GetData()
            End Select
        Catch ex As Exception
            data = Nothing
        End Try

        Return (data IsNot Nothing)

    End Function

    Public Function GetDataByType(ByVal typeData As System.Type, ByRef data As IPluginData) As Boolean _
        Implements IDataProducerPlugin.GetDataByType

        Try
            If typeData Is GetType(INetworkAnalysisData) Then
                data = Me.GetData()
            End If
        Catch ex As Exception
            data = Nothing
        End Try
        Return (data IsNot Nothing)
    End Function

    Private Function GetData() As cNetworkAnalysisData

        Dim asData(6, 5) As Single

        ' Run network if needed
        If Not Me.m_manager.IsMainNetworkRun Then
            Me.m_manager.RunMainNetwork()
        End If

        asData(1, 1) = m_manager.AscendancyImportTotal
        asData(2, 1) = m_manager.AscendancyImportPer
        asData(3, 1) = m_manager.OverheadImportTotal
        asData(4, 1) = m_manager.OverheadImportPer
        asData(5, 1) = m_manager.CapacityImportTotal
        asData(6, 1) = m_manager.CapacityImportPer

        asData(1, 2) = m_manager.AscendancyInternalFlowTotal
        asData(2, 2) = m_manager.AscendancyInternalFlowPer
        asData(3, 2) = m_manager.OverheadFlowTotal
        asData(4, 2) = m_manager.OverheadFlowPer
        asData(5, 2) = m_manager.CapacityFlowTotal
        asData(6, 2) = m_manager.CapacityFlowPer

        asData(1, 3) = m_manager.AscendancyExportTotal
        asData(2, 3) = m_manager.AscendancyExportPer
        asData(3, 3) = m_manager.OverheadExportTotal
        asData(4, 3) = m_manager.OverheadExportPer
        asData(5, 3) = m_manager.CapacityExportTotal
        asData(6, 3) = m_manager.CapacityExportPer

        asData(1, 4) = m_manager.AscendancyRespTotal
        asData(2, 4) = m_manager.AscendancyRespPer
        asData(3, 4) = m_manager.OverheadRespTotal
        asData(4, 4) = m_manager.OverheadRespPer
        asData(5, 4) = m_manager.CapacityRespTotal
        asData(6, 4) = m_manager.CapacityRespPer

        asData(1, 5) = m_manager.AscendancyTotalsTotal
        asData(2, 5) = m_manager.AscendancyTotalsPer
        asData(3, 5) = m_manager.OverheadTotalsTotal
        asData(4, 5) = m_manager.OverheadTotalsPer
        asData(5, 5) = m_manager.CapacityTotalsTotal
        asData(6, 5) = m_manager.CapacityTotalsPer

        ' ToDo: look up plugin assembly name dynamically
        ' Dim s As String = System.Reflection.Assembly.GetAssembly(GetType(cEwENetworkAnalysisPlugin)).FullName)
        Return New cNetworkAnalysisData("EwENetworkAnalysis", Me.Name, asData)

    End Function

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
