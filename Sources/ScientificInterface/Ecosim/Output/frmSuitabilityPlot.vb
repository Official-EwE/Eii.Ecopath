' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Ecosim

    Public Class SuitabilityPlot

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As cUIContext)
                MyBase.UIContext = value
                Me.m_plot.UIContext = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

    End Class

End Namespace

