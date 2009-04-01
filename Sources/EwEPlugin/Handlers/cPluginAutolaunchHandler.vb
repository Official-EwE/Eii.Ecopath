'==============================================================================
'
' $Log: cPluginAutolaunchHandler.vb,v $
' Revision 1.4  2009/04/01 12:50:07  jeroens
' Fixed incomplete port, woops
'
' Revision 1.3  2009/03/31 14:53:04  jeroens
' Updated to GetPlugins interface changes
'
' Revision 1.2  2008/11/17 13:05:59  jeroens
' Fixed auto-launch behaviour
'
' Revision 1.1  2008/09/05 16:13:02  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ===========================================================================
''' <summary>
''' Helper class, launches all Auto-launchable plug-ins.
''' </summary>
''' ===========================================================================
Public Class cPluginAutolaunchHandler
    Inherits cPluginGUIHandler

#Region " Construction "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of a cPluginAutolaunchHandler.
    ''' </summary>
    ''' <param name="pm"><see cref="cPluginManager">Plugin manager</see>
    ''' that holds the plugins to launch.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByRef pm As cPluginManager)
        MyBase.new()
        Me.PluginManager = pm
        Me.LaunchPlugins()
    End Sub

#End Region ' Construction 

#Region " Overrides "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden with emtpy method to comply to base class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub EnablePlugin(ByVal ip As IGUIPlugin, ByVal bEnable As Boolean)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden with emtpy method to comply to base class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub PlacePlugin(ByVal ip As IGUIPlugin, ByVal bPlace As Boolean)
    End Sub

#End Region ' Overrides 

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch all <see cref="IAutolaunchPlugin">Auto-launchable plug-ins.</see>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub LaunchPlugins()
        Dim collPlugins As ICollection(Of cPluginManager.cPluginContext) = Me.PluginManager.GetPlugins(GetType(IAutolaunchPlugin))
        For Each ipc As cPluginManager.cPluginContext In collPlugins
            Dim ip As IAutolaunchPlugin = DirectCast(ipc.Plugin, IAutolaunchPlugin)
            If ip.Autolaunch Then
                Me.RunPlugin(ip, Nothing, Nothing)
            End If
        Next
    End Sub

#End Region ' Internals

End Class
