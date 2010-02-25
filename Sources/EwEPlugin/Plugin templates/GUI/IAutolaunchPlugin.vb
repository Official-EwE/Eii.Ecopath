#Region " Imports "

Option Strict On

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plug-in that should auto-launch when a consuming GUI is loaded
''' </summary>
''' ===========================================================================
Public Interface IAutolaunchPlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Plug-in point to state whether auto-launch is active.
    ''' </summary>
    ''' <remarks>True if active.</remarks>
    ''' -----------------------------------------------------------------------
    Function Autolaunch() As Boolean

End Interface
