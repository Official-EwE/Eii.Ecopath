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
Imports System
Imports System.Windows.Forms
Imports System.Diagnostics

#End Region ' Imports

' ToDo_JS: Make menu items toggle automatically

Namespace Commands

#Region " cControlHandler - base class "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Base class for connecting a Command to a User Interface Control
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public MustInherit Class cControlHandler
        Implements IDisposable

        ''' <summary>The associated Command.</summary>
        Private m_cmd As cCommand = Nothing
        ''' <summary>Optional tag</summary>
        Private m_objTag As Object = Nothing

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of a ControlHandler. Connects a
        ''' Command instance to a User Interface control.
        ''' </summary>
        ''' <param name="objCmd">The Command instance to attach.</param>
        ''' <param name="objGUI">The User Interface instance to attach.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, ByVal objTag As Object)
            Debug.Assert(TypeOf objCmd Is cCommand)
            Me.m_cmd = DirectCast(objCmd, cCommand)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="IDisposable.Dispose"/>
        ''' ---------------------------------------------------------------------------
        Public Overridable Sub Dispose() _
            Implements IDisposable.Dispose

            Me.m_cmd = Nothing
            Me.m_objTag = Nothing
            GC.SuppressFinalize(Me)

        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; exposes the attached cCommand to derived classes.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected ReadOnly Property Command() As cCommand
            Get
                Return Me.m_cmd
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; exposes an attached launch parameter to derived classes.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected ReadOnly Property Tag As Object
            Get
                Return Me.m_objTag
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Invoke the underlying command.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Sub Invoke()
            Me.m_cmd.Tag = Me.m_objTag
            Me.m_cmd.Invoke()
            Me.m_cmd.Tag = Nothing
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Override this to implement how the User Interface control will reflect
        ''' the Command state.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public MustOverride Sub Update()

    End Class

#End Region ' cControlHandler 

#Region " cToolStripMenuItemControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a <see cref="cCommand"/> and a 
    ''' <see cref="ToolStripMenuItem"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cToolStripMenuItemControlHandler
        Inherits cControlHandler

        Private WithEvents m_tsi As ToolStripMenuItem = Nothing

        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, ByVal tag As Object)
            MyBase.New(objCmd, objGUI, tag)
            Debug.Assert(TypeOf objGUI Is ToolStripMenuItem)
            Me.m_tsi = DirectCast(objGUI, ToolStripMenuItem)
        End Sub

        Public Overrides Sub Dispose()
            Me.m_tsi = Nothing
            MyBase.Dispose()
        End Sub

        Public Overrides Sub Update()
            Me.m_tsi.Available = Me.Command.IsAvailable
            Me.m_tsi.Enabled = Me.Command.Enabled
            Me.m_tsi.Checked = Me.Command.Checked
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsi.Click
            Try
                Me.Invoke()
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

    End Class

#End Region ' ToolStripMenuItemControlHandler

#Region " cToolStripButtonControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a <see cref="cCommand"/> and a 
    ''' <see cref="ToolStripButton"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cToolStripButtonControlHandler
        Inherits cControlHandler

        Private WithEvents m_tsb As ToolStripButton = Nothing

        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, ByVal objTag As Object)
            MyBase.New(objCmd, objGUI, objTag)
            Debug.Assert(TypeOf objGUI Is ToolStripButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripButton)
        End Sub

        Public Overrides Sub Dispose()
            Me.m_tsb = Nothing
            MyBase.Dispose()
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Available = Me.Command.IsAvailable
            Me.m_tsb.Enabled = Me.Command.Enabled
            Me.m_tsb.Checked = Me.Command.Checked
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.Click
            Try
                Me.Invoke()
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

    End Class

#End Region ' cToolStripButtonControlHandler

#Region " cToolStripButtonDropDownControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a <see cref="cCommand"/> and a 
    ''' <see cref="ToolStripButton"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ToolStripButtonDropDownControlHandler
        Inherits cControlHandler

        Private WithEvents m_tsb As ToolStripDropDownButton = Nothing

        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, objTag As Object)
            MyBase.New(objCmd, objGUI, objTag)
            Debug.Assert(TypeOf objGUI Is ToolStripDropDownButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripDropDownButton)
        End Sub

        Public Overrides Sub Dispose()
            Me.m_tsb = Nothing
            MyBase.Dispose()
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Available = Me.Command.IsAvailable
            Me.m_tsb.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.Click
            Try
                Me.Invoke()
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

    End Class

#End Region ' cToolStripButtonDropDownControlHandler

#Region " ToolStripSplitButtonHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a <see cref="cCommand"/> and a 
    ''' <see cref="ToolStripSplitButton"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cToolStripSplitButtonHandler
        Inherits cControlHandler

        Private WithEvents m_tsb As ToolStripSplitButton = Nothing

        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, ByVal objTag As Object)
            MyBase.New(objCmd, objGUI, objTag)
            Debug.Assert(TypeOf objGUI Is ToolStripSplitButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripSplitButton)
        End Sub

        Public Overrides Sub Dispose()
            Me.m_tsb = Nothing
            MyBase.Dispose()
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Available = Me.Command.IsAvailable
            Me.m_tsb.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.ButtonClick
            Try
                Me.Invoke()
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

    End Class

#End Region ' ToolStripSplitButtonHandler

#Region " cButtonControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a <see cref="cCommand"/> and a <see cref="Button"/>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cButtonControlHandler
        Inherits cControlHandler

        Private WithEvents m_btn As Button = Nothing

        Public Sub New(ByVal objCmd As Object, ByVal objGUI As Object, ByVal objTag As Object)
            MyBase.New(objCmd, objGUI, objTag)
            Debug.Assert(TypeOf objGUI Is Button)
            Me.m_btn = DirectCast(objGUI, Button)
        End Sub

        Public Overrides Sub Dispose()
            Me.m_btn = Nothing
            MyBase.Dispose()
        End Sub

        Public Overrides Sub Update()
            Me.m_btn.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_btn.Click
            Try
                Me.Invoke()
            Catch ex As Exception
                ' Kaboom
            End Try
        End Sub

    End Class

#End Region ' cButtonControlHandler

End Namespace
