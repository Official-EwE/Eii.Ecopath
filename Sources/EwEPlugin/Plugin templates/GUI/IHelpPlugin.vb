'==============================================================================
'
' $Log: IHelpPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/11/01 16:08:17  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.Drawing
Imports EwEUtils.Core
Imports EwEUtils.Commands

''' ---------------------------------------------------------------------------
''' <summary>
''' IPluginHelpPlugin, interface for providing help information for a 
''' <see cref="IPlugin">plugin</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IHelpPlugin

    Sub OnShowHelp(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As Windows.Forms.Form)

End Interface
