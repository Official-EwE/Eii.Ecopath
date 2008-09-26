'==============================================================================
'
' $Log: IGUIPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:08  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/01/29 15:56:59  jeroens
' Fixed CLS compliancy issues
'
' Revision 1.4  2007/04/25 22:43:03  jeroens
' + Extended OnControlClick with parameter that can return a reference to the form created for this plugin. This form can then be blended into the application invoking the command
'
' Revision 1.3  2007/03/17 02:10:20  jeroens
' + Added EnabledState
'
' Revision 1.2  2007/03/14 00:53:05  jeroens
' * Renamed ControlStatusText to ControlTooltipText
'
' Revision 1.1  2006/08/30 20:52:36  jeroens
' * Moved and/or created
'
' Revision 1.1  2006/08/08 14:11:50  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports System.Drawing
Imports EwEUtils.Core
Imports EwEUtils.Commands

''' ---------------------------------------------------------------------------
''' <summary>
''' IGUIPlugin, interface for implementing <see cref="IPlugin">plugins</see> that
''' must be accessible from a Windows GUI.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IGUIPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify an image to show in the control 
    ''' for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property ControlImage() As Image

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the item text to display in the control 
    ''' for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property ControlText() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to specify the tooltip text to display for the control
    ''' for this plugin.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    ReadOnly Property ControlTooltipText() As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler that will be called when the control for this plugin
    ''' is clicked or activated.
    ''' </summary>
    ''' <param name="sender">The control that was clicked or activated.</param>
    ''' <param name="e">Event parameters pertaining the control.</param>
    ''' <param name="frmPlugin">A reference to the form that the plugin creates
    ''' or activates in response to this event.</param>
    ''' -----------------------------------------------------------------------
    Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As Windows.Forms.Form)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the <see cref="eCoreExecutionState">Core Execution State</see> that the 
    ''' EwE core must meet to allow this plugin to run. All GUI controls attached
    ''' to this plug-in will be enabled and disabled in tune with this state.
    ''' </summary>
    ''' <returns>A eCoreExecutionState value, or 0 if this plugin should be accessible anytime.</returns>
    ''' <remarks>See EwECore/Core/cCoreStateMonitor.eCoreExecutionState for possible values.</remarks>
    ''' -----------------------------------------------------------------------
    ReadOnly Property EnabledState() As eCoreExecutionState

End Interface
