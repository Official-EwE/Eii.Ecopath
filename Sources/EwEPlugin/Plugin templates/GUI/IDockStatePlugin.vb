#Region " Imports "

Option Strict On

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Plugin point that allows a GUI plugin to state its desired dock location.
''' </summary>
''' ===========================================================================
Public Interface IDockStatePlugin
    Inherits IGUIPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The dockstate for the form of this plugin.
    ''' </summary>
    ''' <remarks>
    ''' Values are interpreted as
    ''' WeifenLuo DockState enumerated values. This project is not linked to
    ''' WeifenLuo's DockPanel suite, but implementing plug-ins can include
    ''' such a reference and return actual DockState enumerated values here.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Function DockState() As Integer

End Interface
