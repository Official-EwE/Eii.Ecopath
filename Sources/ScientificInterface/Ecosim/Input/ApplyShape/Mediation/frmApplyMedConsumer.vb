' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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


        Protected Overrides ReadOnly Property Grid() As gridApplyShapeBase
            Get
                Return Me.m_grid
            End Get
        End Property
#End Region

#Region " Event handlers "

        Private Sub OnClearAll(sender As System.Object, e As System.EventArgs) _
            Handles tsBtnClearAll.Click
            Me.ClearAll()
        End Sub

        Private Sub OnSetAll(sender As System.Object, e As System.EventArgs) _
            Handles tsBtnSetAll.Click
            Me.SetAll()
        End Sub

#End Region ' Event handlers

    End Class

End Namespace
