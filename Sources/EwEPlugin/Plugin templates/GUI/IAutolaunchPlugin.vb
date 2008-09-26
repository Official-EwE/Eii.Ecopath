'==============================================================================
'
' $Log: IAutolaunchPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
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
    ''' Plug-in point to auto-launch the plug-in.
    ''' </summary>
    ''' <param name="frmPlugin">The form produced by the plug-in.</param>
    ''' -----------------------------------------------------------------------
    Function Autolaunch(ByRef frmPlugin As Windows.Forms.Form) As Boolean

End Interface
