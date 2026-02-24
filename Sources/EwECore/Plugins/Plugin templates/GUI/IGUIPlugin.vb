' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.UI

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
        ''' Get a WinForms image to show in the control for this plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property ControlImage() As Object

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the tool tip text to display for the control for this plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        ReadOnly Property ControlTooltipText() As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event handler that will be called when the control for this plug-in
        ''' is clicked or activated.
        ''' </summary>
        ''' <param name="sender">The control that was clicked or activated.</param>
        ''' <param name="e">Event parameters pertaining the control.</param>
        ''' <param name="frmPlugin">A reference to the form that the plug-in creates
        ''' or activates in response to this event.</param>
        ''' -----------------------------------------------------------------------
        Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get must meet to allow this plug-in to run. All GUI controls attached
        ''' to this plug-in will be enabled and disabled in tune with this state.
        ''' </summary>
        ''' <returns>A eCoreExecutionState value, or 0 if this plug-in should be accessible anytime.</returns>
        ''' <remarks>See EwECore/Core/cCoreStateMonitor.eCoreExecutionState for possible values.</remarks>
        ''' -----------------------------------------------------------------------
        ReadOnly Property EnabledState() As eCoreExecutionState

    End Interface

End Namespace