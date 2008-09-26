'==============================================================================
'
' $Log: INavigationTreeItemPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2006/09/20 00:50:02  jeroens
' - No longer exposes a Form class. This plug-in type is invoked just like any other. The form should be launched via the central CommandHandler in OnInvoke
'
' Revision 1.2  2006/09/06 16:53:37  jeroens
' + Commenting
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Plugin interface that defines all functionality required to add a custom
''' item to the EwE navigation tree.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface INavigationTreeItemPlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the navigation tree item location for this plugin.
    ''' </summary>
    ''' <remarks>
    ''' A location is a '|' separated series of TreeNode names, starting at the
    ''' root node. 
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    ReadOnly Property NavigationTreeItemLocation() As String

End Interface
