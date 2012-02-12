' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports SourceGrid2

#End Region ' Imports

Namespace Properties

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' This class implements a selection change <see cref="Command">Command</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cPropertySelectionCommand
        Inherits cCommand

        ''' <summary>Public available name for this command</summary>
        Public Shared COMMAND_NAME As String = "~SelectedProperties"

        ''' <summary>The properties broadcasted by this command</summary>
        Private m_lprop As New List(Of cProperty)
        ''' <summary>The event that occurred.</summary>
        Private m_event As SelectionChangeEventType = SelectionChangeEventType.Clear

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes and names an instance of this class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.New(cmdh, COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="source">The <see cref="cCoreInputOutputBase">cCoreInputOutput</see> 
        ''' object that was selected.</param>
        ''' <param name="varName">The <see cref="eVarNameFlags">VarName</see> of
        ''' the field that was selected.</param>
        ''' <param name="sourceSec">The <see cref="cCoreInputOutputBase">cCoreInputOutput</see> 
        ''' object that acts as secundary index to the selection.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal pm As cPropertyManager, _
                                    ByVal source As cCoreInputOutputBase, _
                                    ByVal varName As eVarNameFlags, _
                                    Optional ByVal sourceSec As cCoreInputOutputBase = Nothing)

            Dim prop As cProperty = Nothing

            If Not Object.ReferenceEquals(source, Nothing) Then
                ' Get property
                prop = pm.GetProperty(source, varName, sourceSec)
            End If

            Me.Invoke(prop)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke()
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="prop">The <see cref="cProperty">cProperty</see> that 
        ''' was selected.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal prop As cProperty)
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.Add(prop)
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="aprop">Array of <see cref="cProperty">cProperty</see> 
        ''' instances that were selected.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal aprop() As cProperty)
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.AddRange(aprop)
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="lprop">List of <see cref="cProperty">cProperty</see> 
        ''' instances that were selected.</param>
        ''' <param name="event">The Sourcegrid event that fired this command.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal lprop As List(Of cProperty), _
                                    ByVal [event] As SelectionChangeEventType)
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.AddRange(lprop)
            Me.m_event = [event]
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get and array of currently selected <see cref="cProperty">cProperty</see> 
        ''' instances.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Selection() As cProperty()
            Get
                Return Me.m_lprop.ToArray()
            End Get
        End Property

        Public ReadOnly Property EventType() As SelectionChangeEventType
            Get
                Return Me.m_event
            End Get
        End Property
    End Class

End Namespace ' Properties
