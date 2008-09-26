'==============================================================================
'
' $Log: cEwENetworkAnalysisPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:00  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.27  2008/08/08 23:14:18  jeroens
' Disabled console traces
'
' Revision 1.26  2008/07/08 14:37:45  jeroens
' Exposes AscendancyTotal
'
' Revision 1.25  2008/07/07 02:23:22  jeroens
' Grouped and reorganized functionality
'
' Revision 1.24  2008/07/06 17:28:07  jeroens
' Implemented as IDataExchangePlugin
'
' Revision 1.23  2008/06/19 18:48:46  joeh
' Change Network Analysis Plug-in to Network analysis plug-in
'
' Revision 1.22  2008/03/03 23:40:59  sherman
' Removed EwE from names.
'
' Revision 1.21  2007/10/30 19:21:09  jeroens
' + Plugins need Author, contact
'
' Revision 1.20  2007/10/10 16:52:29  jeroens
' * Fixed textual representations
'
' Revision 1.19  2007/09/25 22:46:11  joeh
' Fix bug 252
'
' Revision 1.18  2007/09/25 00:01:01  joeh
' Fix bug 252
'
' Revision 1.17  2007/06/22 00:35:29  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.16  2007/06/10 00:46:39  joeh
' Add progress bars
'
' Revision 1.15  2007/06/06 01:22:54  jeroens
' * Fixed nav tree node location
' * Changed tooltip text
'
' Revision 1.14  2007/06/05 14:48:50  joeb
' Changes to initialization of Ecosim Network
'
' Revision 1.13  2007/06/01 17:31:59  joeb
' More Ecosim Network Analysis
'
' Revision 1.12  2007/05/29 21:19:36  joeb
' Network Analysis from Ecosim
'
' Revision 1.11  2007/05/28 21:37:15  joeb
' Added Ecosim interfaces
'
' Revision 1.10  2007/05/24 18:24:20  joeb
' Added Ecosim pluging interfaces
'
' Revision 1.9  2007/05/23 00:04:47  joeh
' Add new classes for the UI
'
' Revision 1.8  2007/05/02 01:17:52  joeh
' Second shot at Network Analysis Plugin UI
'
' Revision 1.7  2007/04/30 17:37:16  joeh
' *First shot at the EwENetwork Analysis UI
'
' Revision 1.6  2007/04/26 15:06:46  joeb
' Minor comments
'
' Revision 1.5  2007/04/26 14:01:40  jeroens
' + Form only created when necessary
'
'==============================================================================

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEPlugin

Public Class cEwENetworkAnalysisPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IDataExchangePlugin
    'at this time we do not need these plugin points
    'Implements EwEPlugin.IEcosimRunCompletedPlugin
    'Implements EwEPlugin.IEcosimBeginTimestepPlugin

    Private m_core As EwECore.cCore
    Private m_bInitOK As Boolean

    ''' <summary>
    ''' Network Analysis manager. Provide access to Network Analysis methods.
    ''' </summary>
    ''' <remarks>Because the plugin handles interactions with the core it manages the life span of the network manager and the interface. 
    ''' The plugin is responsible for telling the network manager when a plugin point has been invoked by the core. 
    ''' The plugin will pass a network manager reference to the interface when the user has clicked the plugins menu item or tree node in the main interface.</remarks>
    Private m_NetworkManager As cNetworkManager

    ''' <summary>
    ''' Interface form
    ''' </summary>
    ''' <remarks></remarks>
    Private m_frmNetInterface As frmNetworkAnalysis

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
                m_core = DirectCast(core, EwECore.cCore)
                m_NetworkManager = New cNetworkManager
                m_NetworkManager.Init(m_core)
                m_bInitOK = True
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
                m_NetworkManager.EcopathData = DirectCast(EcopathDataStructures, EwECore.cEcopathDataStructures)
                'Bug 252 fix by joeh
                'Add
                m_NetworkManager.IsMainNetworkRun = False
                m_NetworkManager.IsRequiredPrimaryProdRun = False
                m_NetworkManager.IsEcosimNetworkWithoutPPREstRun = False
                m_NetworkManager.IsEcosimNetworkWithPPREstRun = False
                'End Add
                'System.Console.WriteLine(Me.ToString & ".EcopathRan() Successfull.")
            Else

                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() argument EcopathDataStructure is not a cEcopathDataStructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, EwECore.eMessageSource.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcopathRunCompleted() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, EwECore.eMessageSource.Core, EwECore.eMessageImportance.Warning))

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

        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False

        'Interface item has been clicked
        'Show the Main Network interface
        If m_bInitOK Then

            ' Test if form still exists
            If Me.m_frmNetInterface IsNot Nothing Then
                ' Form is ready to be used if it has not been disposed yet
                bIsFormReady = (Me.m_frmNetInterface.IsDisposed = False)
            End If
            ' Create form when not ready
            If Not bIsFormReady Then
                Me.m_frmNetInterface = New frmNetworkAnalysis(m_NetworkManager)
            End If

            ' Activate the form
            Me.m_frmNetInterface.Show()

            ' Pass form reference back to calling app
            f = Me.m_frmNetInterface

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel

            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu

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

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcopathCompleted
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
        If Not m_NetworkManager.bEcosimNetwork Then
            Return
        End If

        Try
            If TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures Then
                'set the EcosimData data in the network manager object
                'this is the data the Network analysis will be run on
                m_NetworkManager.EcosimData = DirectCast(EcosimDatastructures, EwECore.cEcosimDatastructures)

                'm_NetworkManager.bEcoismNetwork = True

                'Initialize the Network Analysis for Ecosim
                m_NetworkManager.InitNetworkForEcosim()

                'System.Console.WriteLine(Me.ToString & ".EcosimRunInitialized() called.")
            Else

                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcosimRunInitialized() argument EcosimDatastructures is not a cEcosimDatastructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, EwECore.eMessageSource.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwENetworkAnalysis.EcosimRunInitialized() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, EwECore.eMessageSource.Core, EwECore.eMessageImportance.Warning))

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
            If Not m_NetworkManager.bEcosimNetwork Then
                Return
            End If

            If TypeOf EcosimDatastructures Is EwECore.cEcosimDatastructures Then
                'set the EcosimData data in the network manager object
                'this is the data the Network analysis will be run on
                Dim esData As cEcosimDatastructures = DirectCast(EcosimDatastructures, EwECore.cEcosimDatastructures)
                m_NetworkManager.EcosimTimeStep(BiomassAtTimestep, esData, iTime)
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

    Public Sub Manager(ByVal manager As EwEPlugin.cPluginManager) Implements EwEPlugin.IDataExchangePlugin.Manager
        ' Do not need to consume data
    End Sub

    Public Function GetData(ByVal varname As EwEUtils.Core.eVarNameFlags, Optional ByVal iIndex As Integer = -9999) As Object _
            Implements EwEPlugin.IDataExchangePlugin.GetData
        Return Nothing
    End Function

    Public Function GetData(ByVal strVarName As String, Optional ByVal iIndex As Integer = -9999) As Object _
            Implements EwEPlugin.IDataExchangePlugin.GetData
        Dim objData As Object = Nothing

        ' Run network if needed
        If Not Me.m_NetworkManager.IsMainNetworkRun Then
            m_NetworkManager.RunMainNetwork()
        End If

        Try
            Select Case strVarName
                Case "AscendancyTotal"
                    Dim asData(6, 5) As Single

                    asData(1, 1) = m_NetworkManager.AscendancyImportTotal
                    asData(2, 1) = m_NetworkManager.AscendancyImportPer
                    asData(3, 1) = m_NetworkManager.OverheadImportTotal
                    asData(4, 1) = m_NetworkManager.OverheadImportPer
                    asData(5, 1) = m_NetworkManager.CapacityImportTotal
                    asData(6, 1) = m_NetworkManager.CapacityImportPer

                    asData(1, 2) = m_NetworkManager.AscendancyInternalFlowTotal
                    asData(2, 2) = m_NetworkManager.AscendancyInternalFlowPer
                    asData(3, 2) = m_NetworkManager.OverheadFlowTotal
                    asData(4, 2) = m_NetworkManager.OverheadFlowPer
                    asData(5, 2) = m_NetworkManager.CapacityFlowTotal
                    asData(6, 2) = m_NetworkManager.CapacityFlowPer

                    asData(1, 3) = m_NetworkManager.AscendancyExportTotal
                    asData(2, 3) = m_NetworkManager.AscendancyExportPer
                    asData(3, 3) = m_NetworkManager.OverheadExportTotal
                    asData(4, 3) = m_NetworkManager.OverheadExportPer
                    asData(5, 3) = m_NetworkManager.CapacityExportTotal
                    asData(6, 3) = m_NetworkManager.CapacityExportPer

                    asData(1, 4) = m_NetworkManager.AscendancyRespTotal
                    asData(2, 4) = m_NetworkManager.AscendancyRespPer
                    asData(3, 4) = m_NetworkManager.OverheadRespTotal
                    asData(4, 4) = m_NetworkManager.OverheadRespPer
                    asData(5, 4) = m_NetworkManager.CapacityRespTotal
                    asData(6, 4) = m_NetworkManager.CapacityRespPer

                    asData(1, 5) = m_NetworkManager.AscendancyTotalsTotal
                    asData(2, 5) = m_NetworkManager.AscendancyTotalsPer
                    asData(3, 5) = m_NetworkManager.OverheadTotalsTotal
                    asData(4, 5) = m_NetworkManager.OverheadTotalsPer
                    asData(5, 5) = m_NetworkManager.CapacityTotalsTotal
                    asData(6, 5) = m_NetworkManager.CapacityTotalsPer

                    objData = asData
            End Select
        Catch ex As Exception
            objData = Nothing
        End Try
        Return objData
    End Function

#End Region ' Data exchange

End Class
