#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Other

#End Region


Public Class frmApplyCapacity
    Inherits Ecosim.frmApplyShapeBase

#Region " Constructor "

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides ReadOnly Property Grid() As Ecosim.ApplyShapeGrid
        Get
            Return Me.m_grid
        End Get
    End Property

#End Region


End Class