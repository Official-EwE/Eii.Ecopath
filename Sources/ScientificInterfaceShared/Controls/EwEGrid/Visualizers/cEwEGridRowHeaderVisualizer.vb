' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A visualizer for rendering EwE row header cells.
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cEwEGridRowHeaderVisualizer
        Inherits cEwEGridVisualizerBase

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleLeft
            Me.WordWrap = True
        End Sub

    End Class

End Namespace