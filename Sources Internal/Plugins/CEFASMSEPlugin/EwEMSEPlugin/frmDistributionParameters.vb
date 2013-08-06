#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls
Imports SourceGrid2
Imports System.IO

#End Region ' Imports

Public Class frmDistributionParameters

    Private m_MSEPlugin As cMSE

    Public Sub Init(ByVal UI As cUIContext, ByVal Plugin As cMSE)
        Me.m_MSEPlugin = Plugin
    End Sub


    Private Sub frmDistributionParameters_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        cboPathOrSim.SelectedIndex = 0
    End Sub

    Private Sub cboPathOrSim_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboPathOrSim.SelectedIndexChanged
        MsgBox("You have selected: " & cboPathOrSim.SelectedIndex)

    End Sub
End Class