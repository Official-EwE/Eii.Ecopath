'==============================================================================
'
' $Log: IToolbarItemPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2006/09/06 16:53:19  jeroens
' + Commenting
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IToolbarItemPlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the toolbar item location for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property ToolbarItemLocation() As String

End Interface
