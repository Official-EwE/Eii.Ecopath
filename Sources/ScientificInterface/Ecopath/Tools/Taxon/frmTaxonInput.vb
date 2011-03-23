#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
#End Region ' Imports 

Namespace Ecopath.Input

    Public Class frmTaxonInput

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
            Me.Grid = Me.m_grid
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If Me.UIContext Is Nothing Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditTaxa")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnEditTaxa)

        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            If Me.UIContext Is Nothing Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditTaxa")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnEditTaxa)

            MyBase.OnFormClosed(e)
        End Sub

    End Class

End Namespace