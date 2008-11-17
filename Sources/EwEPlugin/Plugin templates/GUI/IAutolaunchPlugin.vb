'==============================================================================
'
' $Log: IAutolaunchPlugin.vb,v $
' Revision 1.2  2008/11/17 13:06:00  jeroens
' Fixed auto-launch behaviour
'
' Revision 1.1  2008/09/05 16:08:36  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.Drawing
Imports EwEUtils.Core
Imports EwEUtils.Commands

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
