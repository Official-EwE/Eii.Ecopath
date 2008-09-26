'==============================================================================
'
' $Log: IMenuItemPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/06/27 02:33:16  jeroens
' Added header
'
'==============================================================================

Public Interface IMenuItemPlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the menu item location for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property MenuItemLocation() As String

End Interface
