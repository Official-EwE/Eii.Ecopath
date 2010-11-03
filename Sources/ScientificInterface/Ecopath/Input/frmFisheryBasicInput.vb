#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecopath Fisheries fleet definitions interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmFisheryBasicInput

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.CommandHandler Is Nothing) Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditFleets")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditFleets)
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
            MyBase.OnFormClosed(e)
            If (Me.CommandHandler Is Nothing) Then Return
            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditFleets")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditFleets)
        End Sub

    End Class

End Namespace
