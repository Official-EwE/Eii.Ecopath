'==============================================================================
'
' $Log: cPluginAutolaunchHandler.vb,v $
' Revision 1.1  2008/09/26 07:31:04  sherman
' --== DELETED HISTORY ==--
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
        Dim collPlugins As ICollection(Of IPlugin) = Me.PluginManager.GetPlugins(GetType(IAutolaunchPlugin))
        For Each ip As IPlugin In collPlugins
            Me.RunPlugin(DirectCast(ip, IAutolaunchPlugin), Nothing, Nothing)
        Next
    End Sub

#End Region ' Internals

End Class
