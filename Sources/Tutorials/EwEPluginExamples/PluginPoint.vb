
Imports EwECore
Imports EwEPlugin

Public Class PluginPoint
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin


    Private m_core As EwECore.cCore
    Private m_bInitOK As Boolean
    Private m_PluginInterface As frmEwEPlugin


    ''' <summary>
    ''' Initialize the Plugin. This is called when the core loads the Plugin. It will only be called once.
    ''' </summary>
    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOK = False
        Try
            If TypeOf core Is EwECore.cCore Then
                m_core = DirectCast(core, EwECore.cCore)
                m_PluginInterface = New frmEwEPlugin(m_core)
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

#Region "Plugin implementation"
    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.MenuItem1
        End Get
    End Property

    ''' <summary>
    ''' Text to be displayed for the plugin
    ''' </summary>
    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "EwE Examples Plug-in"
        End Get
    End Property

    ''' <summary>
    ''' Menu Item or Tree node clicked
    ''' </summary>
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef f As Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick
        ' Flag stating whether form is ready to be used. If so, we don't need to create it, do we?
        Dim bIsFormReady As Boolean = False

        'Interface item has been clicked
        'Show the Ecotroph interface
        If m_bInitOK Then
            ' Test if form still exists

            If Me.m_PluginInterface IsNot Nothing And Me.m_PluginInterface.IsDisposed Then
                Me.m_PluginInterface = New frmEwEPlugin(m_core)
            End If


            ' Activate the form
            Me.m_PluginInterface.Show()

            ' Pass form reference back to calling app
            f = Me.m_PluginInterface

            If TypeOf sender Is System.Windows.Forms.TreeView Then
                'from the navigation panel

            ElseIf TypeOf sender Is System.Windows.Forms.ToolStripMenuItem Then
                'from the menu

            End If
        Else
            Debug.Assert(False, "Plugin was not initialized properly.")
        End If
    End Sub

    ''' <summary>
    ''' Location where the menu item should go
    ''' </summary>
    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "EwE Example Plug-in"
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            'this will put the navigation item at the end of the tree as top level node 
            'Not the best place there should be a Plugins node and all plugins should go under it
            Return "ndTools"
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwE Plugin Examples"
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
