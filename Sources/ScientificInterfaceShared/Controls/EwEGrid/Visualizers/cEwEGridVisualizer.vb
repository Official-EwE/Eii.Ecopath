' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A EwE Grid base visualizer that aligns its content.
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cEwECellVisualizer
        Inherits cEwEGridVisualizerBase

        ''' <summary>
        ''' Create a new visualizer.
        ''' </summary>
        ''' <param name="alignment">Alignment to choose. If not specified content will
        ''' be aligned <see cref="ContentAlignment.MiddleCenter"/>.</param>
        Public Sub New(Optional alignment As ContentAlignment = ContentAlignment.MiddleCenter)
            MyBase.New()
            Me.TextAlignment = alignment
            Me.AlignTextToImage = True
            Me.WordWrap = False
            Me.AlignTextToImage = True
        End Sub

    End Class

End Namespace