' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing fishing mortality shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridFishingMortality
    Inherits gridForcingBase

    Private m_handler As cFishingMortalityShapeGUIHandler = Nothing

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Public Overrides ReadOnly Property Handler() As ScientificInterfaceShared.Controls.cShapeGUIHandler
        Get
            If (Me.m_handler Is Nothing) Then
                Me.m_handler = New cFishingMortalityShapeGUIHandler(Me.UIContext)
            End If

            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.UIContext.Core.FishMortShapeManager
        End Get
    End Property

End Class
