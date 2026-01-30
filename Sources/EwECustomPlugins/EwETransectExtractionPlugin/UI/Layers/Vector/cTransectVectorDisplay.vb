' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Definitions

Public Class cTransectVectorDisplay
    Inherits cDisplayLayer

    Public Sub New(uic As cUIContext)
        MyBase.New(uic, New cTransectVectorRenderer(uic))
        Me.m_editor = New cTransectVectorEditor()
        Me.RenderMode = eLayerRenderType.Always
    End Sub

    Public Property Data As cTransectDatastructures = Nothing

End Class

