#Region " Imports "

Option Strict On
Imports System
Imports System.Windows.Forms
Imports System.Diagnostics
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports System.Collections.Generic

#End Region ' Imports

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
    ''' <summary>The command handler to interact with.</summary>
    Private m_cmdh As cCommandHandler = Nothing

#End Region ' Private parts

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Construct a new cPluginGUIHandler.
    ''' </summary>
    ''' <param name="pm"></param>
    ''' <param name="cmdh"></param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal pm As cPluginManager, _
                   ByVal cmdh As cCommandHandler)
        Me.PluginManager = pm
        Me.CommandHandler = cmdh
    End Sub

#Region " Plugin assembly handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="cPluginManager">plug-in manager</see> for this handler.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property PluginManager() As cPluginManager
        Get
            Return Me.m_pm
        End Get
        Set(ByVal pm As cPluginManager)
            If (pm Is Me.m_pm) Then Return

            If (Me.m_pm IsNot Nothing) Then
                '' Stop observing events originating from current plugin manager:
                '' - Assemblies added event
                'RemoveHandler m_pm.AssemblyAdded, AddressOf OnAssemblyAdded
                '' - Assemblies removed event
                'RemoveHandler m_pm.AssemblyRemoved, AddressOf OnAssemblyRemoved
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
    ''' Get/set the <see cref="cCommandHandler">command handler</see> for this 
    ''' handler to use.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Property CommandHandler() As cCommandHandler
        Get
            Return Me.m_cmdh
        End Get
        Set(ByVal value As cCommandHandler)
            Me.m_cmdh = value
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
        '' Start listening to events originating from this assembly
        '' - Assembly enabled state changed event
        'AddHandler pa.AssemblyEnabled, AddressOf ActivateAssembly
        Me.ActivateAssembly(pa, pa.Enabled)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, called when a <see cref="cPluginAssembly">plugin assembly</see>
    ''' is removed from the <see cref="cPluginManager">plugin manager</see>.
    ''' </summary>
    ''' <param name="pa">The removed plugin assembly.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnAssemblyRemoved(ByVal pa As cPluginAssembly)
        ' Remove the assembly
        Me.ActivateAssembly(pa, False)
        '' Stop listening to assemble enabled events
        '' - Assembly enabled state changed event
        'RemoveHandler pa.AssemblyEnabled, AddressOf ActivateAssembly
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
    Private Sub ActivateAssembly(ByVal pa As cPluginAssembly, ByVal bEnabled As Boolean)

        Dim ctrl As Control = Nothing
        Dim lPlugins As New List(Of IGUIPlugin)
        Dim aSorted() As IGUIPlugin = Nothing
        Dim iStart, iEnd, iStep As Integer

        ' Gather list of plug-ins
        For Each ip As IPlugin In pa.Plugins(GetType(IGUIPlugin), True)
            lPlugins.Add(DirectCast(ip, IGUIPlugin))
        Next

        ' Sort
        aSorted = Me.SortPlugins(lPlugins.ToArray())

        If bEnabled Then
            iStart = 0 : iEnd = aSorted.Length - 1 : iStep = 1
        Else
            iStart = aSorted.Length - 1 : iEnd = 0 : iStep = -1
        End If

        ' Enable or disable in sorted order
        For iPlugin As Integer = iStart To iEnd Step iStep
            ' Position the plugin
            Me.PlacePlugin(aSorted(iPlugin), bEnabled)
            ' Update its enabled state
            Me.m_pm.UpdatePluginEnabledStates(aSorted(iPlugin))
        Next iPlugin

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sort all plug-ins in a plug-in assembly for proper ordering.
    ''' </summary>
    ''' <param name="aip">An array of plug-ins to sort.</param>
    ''' <returns>An array of sorted plug-ins.</returns>
    ''' <remarks>
    ''' This method is useful when adding or removing a series of hierarchical 
    ''' UI plug-ins with a hierarchical structure, such as menu items or navigation
    ''' tree nodes.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Function SortPlugins(ByVal aip() As IGUIPlugin) As IGUIPlugin()
        Return aip
    End Function

#End Region ' Plugin placement

#Region " Plugin execution "

    Protected Sub RunPlugin(ByVal ip As IGUIPlugin, ByVal sender As Object, ByVal e As EventArgs)

        Dim cmd As cCommand = Nothing
        Dim pcmd As cPluginGUICommand = Nothing
        Dim frm As Form = Nothing

        Debug.Assert(Me.m_cmdh IsNot Nothing)

        ' Try to get the reserved GUI command from the central command handler
        cmd = Me.m_cmdh.GetCommand(cPluginGUICommand.COMMAND_NAME)
        ' Got a result?
        If cmd IsNot Nothing Then
            ' #Yes: verify if correct class?
            If TypeOf cmd Is cPluginGUICommand Then
                ' #Yes: type-cast
                pcmd = DirectCast(cmd, cPluginGUICommand)
            End If
        End If

        Try

            ' Found a valid GUI command?
            If (pcmd IsNot Nothing) Then
                ' #Yes: invoke the plug-in via this command
                pcmd.Invoke(ip, sender, e)
            Else
                ' #No: activate plugin directly
                ip.OnControlClick(sender, e, frm)
            End If

        Catch ex As Exception
            System.Console.WriteLine("Error in OnPluginMenuItemClick()" & ex.Message)
            Debug.Assert(False, "Error in OnPluginMenuItemClick()" & ex.Message)
        End Try

    End Sub

#End Region ' Plugin execution

End Class
