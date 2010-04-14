#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports EwEUtils.Commands

#End Region


Public Class frmOptions

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

    End Sub

End Class