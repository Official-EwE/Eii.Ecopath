' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.EwEGrid

    Public Class cEwEGridRowIndexVisualizer
        Inherits cEwEGridVisualizerBase

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleCenter
            Me.WordWrap = True
        End Sub

    End Class

End Namespace