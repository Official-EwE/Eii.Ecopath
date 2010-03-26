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
    ''' <para>A location is a '\' separated series of menu item names, starting 
    ''' at the root node of the menu that the plug-in is nested into.</para>
    ''' <para>Use of the '|' character to separate menu item names is deprecated.</para>
    ''' -----------------------------------------------------------------------
    ReadOnly Property MenuItemLocation() As String

End Interface
