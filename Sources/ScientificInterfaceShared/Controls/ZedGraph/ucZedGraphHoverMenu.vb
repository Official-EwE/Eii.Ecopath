#Region " Imports "

Option Strict On

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Hover menu for zed graph controls
    ''' </summary>
    ''' =======================================================================
    Public Class ucZedGraphHoverMenu

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event for notifying the world that the user executed a command.
        ''' </summary>
        ''' <param name="cmd"></param>
        ''' -------------------------------------------------------------------
        Public Event OnUserCommand(ByVal cmd As eCommandTypes)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type stating possible menu hover commands
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eCommandTypes As Integer
            ''' <summary>User wants to zoom in.</summary>
            ZoomIn
            ''' <summary>User wants to zoom out.</summary>
            ZoomOut
        End Enum

#End Region ' Public interfaces

#Region " Framework overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.Size = Me.ClientSize
        End Sub

#End Region ' Framework overrides

#Region " Event handling "

        Private Sub m_btnIn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnIn.Click
            Me.InvokeCallback(eCommandTypes.ZoomIn)
        End Sub

        Private Sub m_btnOut_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOut.Click
            Me.InvokeCallback(eCommandTypes.ZoomOut)
        End Sub

#End Region ' Event handling

#Region " Internals "

        Private Sub InvokeCallback(ByVal cmd As eCommandTypes)
            RaiseEvent OnUserCommand(cmd)
        End Sub

#End Region ' Internals

    End Class

End Namespace
