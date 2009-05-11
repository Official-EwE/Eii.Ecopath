'==============================================================================
'
' $Log: cControlHandler.vb,v $
' Revision 1.1  2009/05/11 01:43:25  jeroens
' Renamed
'
' Revision 1.1  2008/09/26 07:31:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2007/08/30 16:24:41  jeroens
' + Added support for toolstrip split button
'
' Revision 1.2  2007/01/20 03:18:32  jeroens
' + Added handler for ToolStripButtonDropDown controls
'
' Revision 1.1  2006/09/18 15:41:00  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports System.Windows.Forms

Namespace Commands

#Region " ControlHandler base class "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Base class for connecting a Command to a User Interface Control
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public MustInherit Class ControlHandler

        ''' <summary>The associated Command.</summary>
        Private m_cmd As cCommand = Nothing

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of a ControlHandler. Connects a
        ''' Command instance to a User Interface control.
        ''' </summary>
        ''' <param name="objCmd">The Command instance to attach.</param>
        ''' <param name="objGUI">The User Interface instance to attach.</param>
        ''' ---------------------------------------------------------------------------
        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            Debug.Assert(TypeOf objCmd Is cCommand)
            m_cmd = DirectCast(objCmd, cCommand)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; exposes the attached Command to derived classes.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected ReadOnly Property Command() As cCommand
            Get
                Return Me.m_cmd
            End Get
        End Property

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Override this to implement how the User Interface control will reflect
        ''' the Command state.
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public MustOverride Sub Update()

    End Class

#End Region ' ControlHandler base class 

#Region " ToolStripMenuItemControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a Command and a ToolStripMenuItem.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ToolStripMenuItemControlHandler
        Inherits ControlHandler

        Private WithEvents m_tsi As ToolStripMenuItem = Nothing

        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            MyBase.New(objCmd, objGUI)
            Debug.Assert(TypeOf objGUI Is ToolStripMenuItem)
            Me.m_tsi = DirectCast(objGUI, ToolStripMenuItem)
        End Sub

        Public Overrides Sub Update()
            Me.m_tsi.Enabled = Me.Command.Enabled
            Me.m_tsi.Checked = Me.Command.Checked
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsi.Click
            Me.Command.Invoke()
        End Sub

    End Class

#End Region ' ToolStripMenuItemControlHandler

#Region " ToolStripButtonControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a Command and a ToolStripButton.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ToolStripButtonControlHandler
        Inherits ControlHandler

        Private WithEvents m_tsb As ToolStripButton = Nothing

        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            MyBase.New(objCmd, objGUI)
            Debug.Assert(TypeOf objGUI Is ToolStripButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripButton)
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Enabled = Me.Command.Enabled
            Me.m_tsb.Checked = Me.Command.Checked
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.Click
            Me.Command.Invoke()
        End Sub

    End Class

#End Region ' ToolStripButtonControlHandler

#Region " ToolStripButtonDropDownControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a Command and a ToolStripButton.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ToolStripButtonDropDownControlHandler
        Inherits ControlHandler

        Private WithEvents m_tsb As ToolStripDropDownButton = Nothing

        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            MyBase.New(objCmd, objGUI)
            Debug.Assert(TypeOf objGUI Is ToolStripDropDownButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripDropDownButton)
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.Click
            Me.Command.Invoke()
        End Sub

    End Class

#End Region ' ToolStripButtonControlHandler

#Region " ToolStripSplitButtonHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a Command and a ToolStripSplitButton.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ToolStripSplitButtonHandler
        Inherits ControlHandler

        Private WithEvents m_tsb As ToolStripSplitButton = Nothing

        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            MyBase.New(objCmd, objGUI)
            Debug.Assert(TypeOf objGUI Is ToolStripSplitButton)
            Me.m_tsb = DirectCast(objGUI, ToolStripSplitButton)
        End Sub

        Public Overrides Sub Update()
            Me.m_tsb.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_tsb.ButtonClick
            Me.Command.Invoke()
        End Sub

    End Class

#End Region ' ToolStripButtonControlHandler

#Region " ButtonControlHandler "

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Implementation of a connecting between a Command and a Button.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ButtonControlHandler
        Inherits ControlHandler

        Private WithEvents m_btn As Button = Nothing

        Public Sub New(ByRef objCmd As Object, ByRef objGUI As Object)
            MyBase.New(objCmd, objGUI)
            Debug.Assert(TypeOf objGUI Is Button)
            Me.m_btn = DirectCast(objGUI, Button)
        End Sub

        Public Overrides Sub Update()
            Me.m_btn.Enabled = Me.Command.Enabled
        End Sub

        Private Sub OnClick(ByVal sender As Object, ByVal e As EventArgs) Handles m_btn.Click
            Me.Command.Invoke()
        End Sub

    End Class

#End Region ' ButtonControlHandler

End Namespace
