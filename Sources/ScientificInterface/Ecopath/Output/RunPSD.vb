' =============================================================================
'
' $Log: RunPSD.vb,v $
' Revision 1.2  2009/02/21 00:24:14  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ZedGraph

#End Region

Namespace Ecopath.Output

    Public Class RunPSD

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

        End Sub

        Private Sub mnuItmGroupPB_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmGroupPB.CheckedChanged
            mnuItmLorenzen.Checked = Not mnuItmGroupPB.Checked
        End Sub

        Private Sub mnuItmLorenzen_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmLorenzen.CheckedChanged
            mnuItmGroupPB.Checked = Not mnuItmLorenzen.Checked
        End Sub
    End Class

End Namespace