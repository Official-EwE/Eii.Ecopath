' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing Egg Production shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridEggProduction
    Inherits gridForcingBase

    Private m_handler As cEggProductionShapeGUIHandler = Nothing

    Public Sub New()
        MyBase.New()
        Me.IsSeasonal = True
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            If (Me.m_handler Is Nothing) Then
                Me.m_handler = New cEggProductionShapeGUIHandler(Me.UIContext)
            End If
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.Core.EggProdShapeManager
        End Get
    End Property

End Class
