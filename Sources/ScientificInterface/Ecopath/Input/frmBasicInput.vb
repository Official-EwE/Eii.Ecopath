#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Basic Input interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmBasicInput

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Dim cmd As cCommand = Nothing
            MyBase.OnLoad(e)

            If (Me.CommandHandler Is Nothing) Then Return

            cmd = Me.CommandHandler.GetCommand("EditGroups")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditGroups)
            cmd = Me.CommandHandler.GetCommand("EditMultiStanza")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditMultiStanza)
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            Dim cmd As cCommand = Nothing

            MyBase.OnFormClosed(e)

            If (Me.CommandHandler Is Nothing) Then Return

            cmd = Me.CommandHandler.GetCommand("EditGroups")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditGroups)
            cmd = Me.CommandHandler.GetCommand("EditMultiStanza")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditMultiStanza)
        End Sub

    End Class

End Namespace
