'==============================================================================
'
' $Log: cPluginGUIHandler.vb,v $
' Revision 1.2  2009/04/02 19:00:44  jeroens
' Minor changes
'
' Revision 1.1  2008/09/26 07:31:04  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/09/06 16:17:47  jeroens
' Uses IDockStatePlugin
'
' Revision 1.7  2008/09/05 16:13:40  jeroens
' PluginManager set/get via Property
'
' Revision 1.6  2007/04/26 00:41:56  jeroens
' + Extended OnControlClick with parameter that can return a reference to the form created for this plugin. This form can then be blended into the application invoking the command
'
' Revision 1.5  2007/04/25 22:43:19  jeroens
' + Extended OnControlClick with parameter that can return a reference to the form created for this plugin. This form can then be blended into the application invoking the command
'
' Revision 1.4  2007/03/19 14:23:22  jeroens
' + Newly placed plug-ins are properly initialized
'
' Revision 1.3  2007/03/19 02:26:37  jeroens
' + Added comments
' + Added RunPlugin
' + Added PluginGUICommand
'
' Revision 1.2  2007/03/17 02:52:35  jeroens
' * Uses Plugins(t) capability of cPluginAssembly
'
' Revision 1.1  2006/08/31 15:20:33  jeroens
' * Moved
'
' Revision 1.2  2006/08/23 02:28:31  jeroens
' + Empty string will append menu item to the main menu, rather than fail
'
' Revision 1.1  2006/08/08 14:11:50  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports System.Windows.Forms
Imports EwEUtils.Commands
Imports EwEUtils.Core

''' -----------------------------------------------------------------------
''' <summary>
''' GUI utility class, handles the placement of <see cref="IGUIPlugin">IGUIPlugin</see>-
''' derived plugins in the menu structure of a <see cref="Form">Form</see>.
''' </summary>
''' -----------------------------------------------------------------------
Public MustInherit Class cPluginGUIHandler

#Region " Private parts "

    ''' <summary>The plugin manager that holds the plugins to manage.</summary>
    Private m_pm As cPluginManager = Nothing

#End Region ' Private parts

#Region " Plugin assembly handling "

    Public Property PluginManager() As cPluginManager
        Get
            Return Me.m_pm
        End Get
        Set(ByVal pm As cPluginManager)
            If (pm Is Me.m_pm) Then Return

            If (Me.m_pm IsNot Nothing) Then
                ' Stop observing events originating from current plugin manager:
                ' - Assemblies added event
                RemoveHandler m_pm.AssemblyAdded, AddressOf OnAssemblyAdded
                ' - Assemblies removed event
                RemoveHandler m_pm.AssemblyRemoved, AddressOf OnAssemblyRemoved
                ' - Plugin enabled state event
                RemoveHandler m_pm.PluginEnabled, AddressOf EnablePlugin
                ' Manually remove existing assemblies
                For Each pa As cPluginAssembly In Me.m_pm.PluginAssemblies
                    Me.OnAssemblyRemoved(pa)
                Next
            End If

            Me.m_pm = pm

            If (Me.m_pm IsNot Nothing) Then
                ' Manually add existing assemblies
                For Each pa As cPluginAssembly In Me.m_pm.PluginAssemblies
                    Me.OnAssemblyAdded(pa)
                Next
                ' Start observing events originating from new plugin manager
                ' - Assemblies added event
                AddHandler m_pm.AssemblyAdded, AddressOf OnAssemblyAdded
                ' - Assemblies removed event
                AddHandler m_pm.AssemblyRemoved, AddressOf OnAssemblyRemoved
                ' - Plugin enabled state event
                AddHandler m_pm.PluginEnabled, AddressOf EnablePlugin
            End If

        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when a <see cref="cPluginAssembly">plugin assembly</see>
    ''' is added to the <see cref="cPluginManager">plugin manager</see>.
    ''' </summary>
    ''' <param name="pa">The added plugin assembly.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyAdded(ByVal pa As cPluginAssembly)
        ' Start listening to events originating from this assembly
        ' - Assembly enabled state changed event
        AddHandler pa.AssemblyEnabled, AddressOf OnAssemblyEnabled
        ' Manually fire event to place plug-ins
        Me.OnAssemblyEnabled(pa, pa.Enabled)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when a <see cref="cPluginAssembly">plugin assembly</see>
    ''' is removed from the <see cref="cPluginManager">plugin manager</see>.
    ''' </summary>
    ''' <param name="pa">The removed plugin assembly.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyRemoved(ByVal pa As cPluginAssembly)
        ' Simulate disabling of the assembly. This will cause all GUI items from this assembly to be removed.
        Me.OnAssemblyEnabled(pa, False)
        ' Stop listening to assemble enabled events
        ' - Assembly enabled state changed event
        RemoveHandler pa.AssemblyEnabled, AddressOf OnAssemblyEnabled
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, responds to a plugin assembly enabled state change.
    ''' </summary>
    ''' <param name="pa">The <see cref="cPluginAssembly">plugin assembly</see>
    ''' that changed enabled state.</param>
    ''' <param name="bEnabled">The new <see cref="cPluginAssembly.Enabled">Enabled</see>
    ''' state.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyEnabled(ByVal pa As cPluginAssembly, ByVal bEnabled As Boolean)
        Dim ctrl As Control = Nothing
        For Each ip As IPlugin In pa.Plugins(GetType(IGUIPlugin), True)
            ' Position the plugin
            Me.PlacePlugin(DirectCast(ip, IGUIPlugin), bEnabled)
            ' Update its enabled state
            Me.m_pm.UpdatePluginEnabledStates(DirectCast(ip, IGUIPlugin))
        Next
    End Sub

#End Region ' Plugin assembly handling 

#Region " Plugin placement "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Place or remove a GUI plugin item.
    ''' </summary>
    ''' <param name="ip">The <see cref="IGUIPlugin">IGUIPlugin</see> to place.</param>
    ''' <param name="bPlace">States whether the item for the plugin should be placed (True)
    ''' or removed (False).</param>
    ''' -----------------------------------------------------------------------
    Protected MustOverride Sub PlacePlugin(ByVal ip As IGUIPlugin, ByVal bPlace As Boolean)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the enabled state of a GUI plugin item.
    ''' </summary>
    ''' <param name="ip">The <see cref="IGUIPlugin">IGUIPlugin</see> to affect.</param>
    ''' <param name="bEnable">States whether the plugin should be enabled (True) or
    ''' or disabled (False).</param>
    ''' -----------------------------------------------------------------------
    Protected MustOverride Sub EnablePlugin(ByVal ip As IGUIPlugin, ByVal bEnable As Boolean)

#End Region ' Plugin placement

#Region " Plugin execution "

    Protected Sub RunPlugin(ByVal ip As IGUIPlugin, ByVal sender As Object, ByVal e As EventArgs)

        Dim cmd As Command = Nothing
        Dim pcmd As PluginGUICommand = Nothing

        ' Try to get the reserved GUI command from the central command handler
        cmd = CommandHandler.GetInstance().GetCommand(PluginGUICommand.COMMAND_NAME)
        ' Got a result?
        If cmd IsNot Nothing Then
            ' #Yes: verify if correct class?
            If TypeOf cmd Is PluginGUICommand Then
                ' #Yes: type-cast
                pcmd = DirectCast(cmd, PluginGUICommand)
            End If
        End If

        Try

            ' Found a valid GUI command?
            If (pcmd IsNot Nothing) Then
                ' #Yes: invoke the plug-in via this command
                pcmd.Invoke(ip, sender, e)
            Else
                ' #No: activate plugin directly
                ip.OnControlClick(sender, e, New Form())
            End If

        Catch ex As Exception
            System.Console.WriteLine("Error in OnPluginMenuItemClick()" & ex.Message)
            Debug.Assert(False, "Error in OnPluginMenuItemClick()" & ex.Message)
        End Try

    End Sub

#End Region ' Plugin execution

End Class

#Region " PluginGUICommand class "

Public Class PluginGUICommand
    Inherits Command

    Public Shared COMMAND_NAME As String = "~launchguiplugin"

    Private m_ip As IGUIPlugin = Nothing
    Private m_sender As Object = Nothing
    Private m_e As EventArgs = Nothing
    Private m_form As Windows.Forms.Form = Nothing
    Private m_iDockState As Integer = 0 ' Unknown
    Private m_bHasRun As Boolean = False

    Public Sub New()
        MyBase.New(PluginGUICommand.COMMAND_NAME)
    End Sub

    Friend Overloads Sub Invoke(ByVal ip As IGUIPlugin, ByVal sender As Object, ByVal e As EventArgs)
        Me.m_ip = ip
        Me.m_sender = sender
        Me.m_e = e
        Me.m_bHasRun = False
        Me.m_form = Nothing
        ' Try to launch plugin via command structure first
        MyBase.Invoke()
        ' Try to run the plug-in manually
        Me.RunPlugin()
    End Sub

    Public ReadOnly Property CoreExecutionState() As eCoreExecutionState
        Get
            If Me.m_ip Is Nothing Then Return eCoreExecutionState.Idle
            Return Me.m_ip.EnabledState
        End Get
    End Property

    Public Property Form() As Windows.Forms.Form
        Get
            Return Me.m_form
        End Get
        Friend Set(ByVal value As Windows.Forms.Form)
            Me.m_form = value
        End Set
    End Property

    Public Property DockState() As Integer
        Get
            Return Me.m_iDockState
        End Get
        Set(ByVal iDockState As Integer)
            Me.m_iDockState = iDockState
        End Set
    End Property

    Public Sub RunPlugin()

        If Me.m_ip Is Nothing Then Return
        If Me.m_bHasRun Then Return

        ' Get dockstate, if possible
        If TypeOf Me.m_ip Is IDockStatePlugin Then
            Me.DockState = DirectCast(Me.m_ip, IDockStatePlugin).DockState
        End If

        Try
            Me.m_ip.OnControlClick(Me.m_sender, Me.m_e, Me.m_form)
        Catch ex As Exception
            Debug.Assert(False, String.Format("Error {0} occurred while running plugin {1}", ex.Message, Me.m_ip.Name))
        Finally
            Me.m_bHasRun = True
        End Try

    End Sub

End Class

#End Region ' PluginGUICommand class