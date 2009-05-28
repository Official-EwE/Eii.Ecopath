'==============================================================================
'
' $Log: cEwEEcotrophPlugin.vb,v $
' Revision 1.3  2009/05/28 14:43:44  jeroens
' Updated to build
'
' Revision 1.2  2009/01/16 18:30:27  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:30:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.10  2008/06/05 19:43:31  joeh
' no message
'
'==============================================================================
Option Strict On
Option Explicit On

Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core

Public Class cEwEEcotrophPlugin
    Implements EwEPlugin.IEcopathRunCompletedPlugin
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin

    Private m_core As EwECore.cCore
    Private m_Input As cInput
    Private m_bInitOK As Boolean
    Private m_EcotrophManager As cEcotrophManager
    Private m_EcotrophInterface As frmEcotrophGrid

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
                m_Input = New cInput
                m_EcotrophManager = New cEcotrophManager
                m_EcotrophManager.Init(m_core, m_Input)
                m_bInitOK = True
                System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
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

    ''' <summary>
    ''' Called by the core when Ecopath has run successfuly 
    ''' </summary>
    ''' <param name="EcopathDataStructures"></param>
    ''' <remarks></remarks>
    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted
        Debug.Assert(TypeOf EcopathDataStructures Is EwECore.cEcopathDataStructures, Me.ToString & _
                             ".EcopathRan() argument EcopathDataStructure is not a cEcopathDataStructures object.")
        Try
            If TypeOf EcopathDataStructures Is EwECore.cEcopathDataStructures Then
                'set the Ecopath data in the ecotroph manager object
                'this is the data the ecotroph analysis will be run on
                m_EcotrophManager.EcopathData = DirectCast(EcopathDataStructures, EwECore.cEcopathDataStructures)
                'm_EcotrophManager.IsMainNetworkRun = False
                'm_EcotrophManager.IsRequiredPrimaryProdRun = False
                'm_EcotrophManager.IsEcosimNetworkWithoutPPREstRun = False
                'm_EcotrophManager.IsEcosimNetworkWithPPREstRun = False
                System.Console.WriteLine(Me.ToString & ".EcopathRan() Successfull.")
            Else
                'some kind of a message
                m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwEEcotroph.EcopathRunCompleted() argument EcopathDataStructure is not a cEcopathDataStructures object." _
                                            , EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
            End If
        Catch ex As Exception
            Debug.Assert(False, ex.Message)

            m_core.Messages.AddMessage(New EwECore.cMessage("Plugin EwEEcotroph.EcopathRunCompleted() Error: " & ex.Message _
                            , EwECore.eMessageType.ErrorEncountered, eCoreComponentType.Core, EwECore.eMessageImportance.Warning))
        End Try
    End Sub


#Region "Plugin implementation"
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "EcoTroph plug-in"
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
        'Show the Ecotroph interface
        If m_bInitOK Then
            ' Test if form still exists
            If Me.m_EcotrophInterface IsNot Nothing Then
                ' Form is ready to be used if it has not been disposed yet
                bIsFormReady = (Me.m_EcotrophInterface.IsDisposed = False)
            End If
            ' Create form when not ready
            If Not bIsFormReady Then
                Me.m_EcotrophInterface = New frmEcotrophGrid(m_EcotrophManager)
            End If

            ' Activate the form
            Me.m_EcotrophInterface.Show()

            ' Pass form reference back to calling app
            f = Me.m_EcotrophInterface

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel

            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu

            End If
        Else
            Debug.Assert(False, "Ecotroph plugin was not initialized properly.")
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

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Ecopath Ecotroph Plug-in"
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

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ControlText()
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
            Return "mailto:s.lai@fisheries.ubc.ca"
        End Get
    End Property
#End Region 'Plugin implementation

End Class
