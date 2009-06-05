
'==============================================================================
'
' $Log: frmMSE.vb,v $
' Revision 1.2  2009/06/05 19:01:50  joeb
' Added MSE node to navigation tree
'
'
'=============================================================================
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core

#End Region

Public Class frmMSE


#Region "Construction Initialization and Distruction"

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()
        ' Add any initialization after the InitializeComponent() call.

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE, eCoreComponentType.SearchObjective}

    End Sub

    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)

        Me.CoreComponents = Nothing

    End Sub

    Private Sub OnfrmMSELoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

#End Region


#Region "Core interactions"



    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)


    End Sub

#End Region


End Class