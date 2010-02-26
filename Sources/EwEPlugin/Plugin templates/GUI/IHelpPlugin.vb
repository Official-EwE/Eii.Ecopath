#Region " Imports "

Option Strict On
Imports System.Windows.Forms

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' IPluginHelpPlugin, interface for providing help information for a 
''' <see cref="IPlugin">plugin</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IHelpPlugin

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <param name="frmPlugin"></param>
    ''' <remarks></remarks>
    Sub OnShowHelp(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As Form)

End Interface
