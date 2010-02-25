#Region " Imports "

Option Strict On

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Plugin interface that defines all functionality required to add a menu
''' item to the EwE main menu.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IMenuItemPlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the menu item location for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property MenuItemLocation() As String

End Interface
