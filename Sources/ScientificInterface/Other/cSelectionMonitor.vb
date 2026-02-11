' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' ---------------------------------------------------------------------------
''' <summary>
''' Monitor class for currently selected data.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cSelectionMonitor

    Private m_cmdSelect As cPropertySelectionCommand = Nothing

    Public Sub New()
        ' NOP
    End Sub

    Public Sub Attach(uic As cUIContext)

        ' Sanity checks
        Debug.Assert(Me.m_cmdSelect Is Nothing)
        Debug.Assert(uic IsNot Nothing)

        ' Start monitoring
        Me.m_cmdSelect = DirectCast(uic.CommandHandler.GetCommand(cPropertySelectionCommand.COMMAND_NAME), cPropertySelectionCommand)
        AddHandler Me.m_cmdSelect.OnPostInvoke, AddressOf Me.HandleSelectionChanged

    End Sub

    Public Sub Detach()

        ' Sanity checks
        Debug.Assert(Me.m_cmdSelect IsNot Nothing)

        ' Stop monitoring
        RemoveHandler Me.m_cmdSelect.OnPostInvoke, AddressOf Me.HandleSelectionChanged
        Me.m_cmdSelect = Nothing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of currently selected properties.
    ''' </summary>
    ''' <returns>An array of currently selected <see cref="cProperty">properties</see>.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Selection() As cProperty()
        If (Me.m_cmdSelect IsNot Nothing) Then
            Return Me.m_cmdSelect.Selection
        End If
        Return New cProperty() {}
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Selection change notification
    ''' </summary>
    ''' <param name="sender"></param>
    ''' -----------------------------------------------------------------------
    Event OnSelectionChanged(sender As cSelectionMonitor)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="OnSelectionChanged">Selection change event</see> dispatch.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub HandleSelectionChanged(cmd As cCommand)
        Try
            RaiseEvent OnSelectionChanged(Me)
        Catch ex As Exception

        End Try
    End Sub

End Class
