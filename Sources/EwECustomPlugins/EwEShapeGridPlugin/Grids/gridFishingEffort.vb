' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports ScientificInterfaceShared.Controls

''' ---------------------------------------------------------------------------
''' <summary>
''' Grid for showing fishing effort shapes.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class gridFishingEffort
    Inherits gridForcingBase

    Private m_handler As cFishingEffortShapeGUIHandler = Nothing

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
                Me.m_handler = New cFishingEffortShapeGUIHandler(Me.UIContext)
            End If
            Return Me.m_handler
        End Get
    End Property

    Public Overrides ReadOnly Property Manager() As System.Collections.IEnumerable
        Get
            Return Me.UIContext.Core.FishingEffortShapeManager
        End Get
    End Property

    Protected Overrides Function Include(shape As cShapeData) As Boolean
        ' Exclude 'all fleets' shape, which has a NULL DBID
        Return MyBase.Include(shape) And (shape.DBID > 0)
    End Function

End Class
