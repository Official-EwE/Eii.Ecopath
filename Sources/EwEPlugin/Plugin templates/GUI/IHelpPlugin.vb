Option Strict On
Imports System.Windows.Forms

''' ---------------------------------------------------------------------------
''' <summary>
''' IPluginHelpPlugin, interface for providing help information for a 
''' <see cref="IPlugin">plugin</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IHelpPlugin

    Sub OnShowHelp(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As Form)

End Interface
