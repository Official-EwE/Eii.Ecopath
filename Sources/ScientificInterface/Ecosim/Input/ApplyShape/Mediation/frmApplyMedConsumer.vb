#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Form implementing the Ecosim 'Apply Mediation to Consumer' interface.
    ''' </summary>
    ''' =======================================================================
    Public Class frmApplyMedConsumer
        Inherits frmApplyShapeBase

#Region " Constructor "

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides ReadOnly Property Grid() As ApplyShapeGrid
            Get
                Return Me.m_grid
            End Get
        End Property
#End Region

#Region " Event handlers "

        Private Sub tsBtnClearAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsBtnClearAll.Click
            Me.ClearAllPairs()
        End Sub

        Private Sub tsBtnSetAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles tsBtnSetAll.Click
            Me.SetAllPairs()
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
