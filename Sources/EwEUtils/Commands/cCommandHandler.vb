#Region " Imports "

Option Strict On
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

#End Region ' Imports

Namespace Commands

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' The CommandHandler is the central repository for storing and retrieving
    ''' <see cref="cCommand">Commands</see> in a User Interface. Additionally, this
    ''' class serves as a central registry point for <see cref="ControlHandler">ControlHandlers</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cCommandHandler

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            ' Create storages
            Me.m_dictCommands = New Dictionary(Of String, cCommand)
            Me.m_dictHandlerTypes = New Dictionary(Of String, Type)

            ' Register predefined command handler types
            Me.AddControlHandlerType("System.Windows.Forms.Button", GetType(ButtonControlHandler))
            Me.AddControlHandlerType("System.Windows.Forms.ToolStripMenuItem", GetType(ToolStripMenuItemControlHandler))
            Me.AddControlHandlerType("System.Windows.Forms.ToolStripButton", GetType(ToolStripButtonControlHandler))
            Me.AddControlHandlerType("System.Windows.Forms.ToolStripDropDownButton", GetType(ToolStripButtonDropDownControlHandler))
            Me.AddControlHandlerType("System.Windows.Forms.ToolStripSplitButton", GetType(ToolStripSplitButtonHandler))
        End Sub

#End Region ' Construction

#Region " Command administration "

        ''' <summary>Dictionary of <see cref="cCommand">Commands</see>.</summary>
        Private m_dictCommands As Dictionary(Of String, cCommand) = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add a <see cref="cCommand">Command</see> to the handler.
        ''' </summary>
        ''' <param name="c">The command to add.</param>
        ''' -----------------------------------------------------------------------
        Friend Sub Add(ByVal c As cCommand)
            Try
                Me.m_dictCommands.Add(c.Name.ToLower(), c)
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Remove a <see cref="cCommand">Command</see> from the handler.
        ''' </summary>
        ''' <param name="c">The command to remove.</param>
        ''' -----------------------------------------------------------------------
        Public Sub Remove(ByVal c As cCommand)
            Try
                If (c IsNot Nothing) Then
                    c.Clear()
                    Me.m_dictCommands.Remove(c.Name.ToLower())
                End If
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Retrieve a <see cref="cCommand">Command</see> by its name.
        ''' </summary>
        ''' <param name="strName">The name of the Command to find.</param>
        ''' <returns>
        ''' A <see cref="cCommand">Command</see>, or Nothing if the command could not 
        ''' be found.
        ''' </returns>
        ''' -----------------------------------------------------------------------
        Public Function GetCommand(ByVal strName As String) As cCommand
            Try
                Return Me.m_dictCommands(strName.ToLower())
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Public Sub Clear()
            Try
                Dim lcmds As New List(Of cCommand)
                For Each cmd As cCommand In Me.m_dictCommands.Values : lcmds.Add(cmd) : Next
                For Each cmd As cCommand In lcmds : Me.Remove(cmd) : Next
                lcmds.Clear()
            Catch ex As Exception

            End Try
            Debug.Assert(Me.m_dictCommands.Count = 0)
        End Sub

#End Region ' Command administration 

#Region " Command idle time updating "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Application idle time handler, makes sure that every registered command 
        ''' is updated.
        ''' </summary>
        ''' <remarks>
        ''' This method should be invoked in response to the
        ''' .NET Application.OnIdle event.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub OnIdle(ByVal sender As Object, ByVal e As EventArgs)
            For Each cmd As cCommand In Me.m_dictCommands.Values
                cmd.Update()
            Next
        End Sub

#End Region ' Command idle time updating 

#Region " ControlHandler administration "

        ''' <summary>Dictionary of registered GUI control handler types.</summary>
        Private m_dictHandlerTypes As Dictionary(Of String, Type) = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Register a GUI control handler type.
        ''' </summary>
        ''' <param name="obj">An instance of a GUI object to add a handler type for.</param>
        ''' <param name="t">The handler type that will handle events for GUI objects
        ''' of the same type.</param>
        ''' -----------------------------------------------------------------------
        Public Sub AddControlHandlerType(ByVal obj As Object, ByVal t As Type)
            Try
                Me.AddControlHandlerType(obj.GetType().ToString(), t)
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Register a GUI control handler type.
        ''' </summary>
        ''' <param name="str">A Type indicator of a GUI object to add a handler type for.</param>
        ''' <param name="t">The handler type that will handle events for GUI objects
        ''' of the same type.</param>
        ''' -----------------------------------------------------------------------
        Public Sub AddControlHandlerType(ByVal str As String, ByVal t As Type)
            Try
                Me.m_dictHandlerTypes.Add(str, t)
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns a registered GUI control handler type.
        ''' </summary>
        ''' <param name="obj">An instance of a GUI object to return the handler type for.</param>
        ''' -----------------------------------------------------------------------
        Public Function GetControlHandlerType(ByVal obj As Object) As Type
            Try
                Return GetControlHandlerType(obj.GetType().ToString())
            Catch ex As Exception
                ' Kaboom
                Return Nothing
            End Try
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns a registered GUI control handler type.
        ''' </summary>
        ''' <param name="str">A Type indicator of a GUI object to return 
        ''' the handler type for.</param>
        ''' -----------------------------------------------------------------------
        Public Function GetControlHandlerType(ByVal str As String) As Type
            Try
                Return Me.m_dictHandlerTypes(str)
            Catch ex As Exception
                ' Kaboom
                Return Nothing
            End Try
        End Function

#End Region ' ControlHandler administration

    End Class

End Namespace
