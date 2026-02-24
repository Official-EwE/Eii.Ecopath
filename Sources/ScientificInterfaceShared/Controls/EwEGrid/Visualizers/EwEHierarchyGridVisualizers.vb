' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEParentRowHeaderVisualizer implements a EwERowHeaderVisualizer visualizer
    ''' for rendering EwE hierarchical parent row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cVisualizerEwECollapseExpandRowHeader
        Inherits cEwEGridRowHeaderVisualizer

        Public Enum eCollapsedState As Integer
            NoChildren = 0
            Collapsed
            Expanded
        End Enum

        Public Sub New()
            MyBase.New()
            Me.ImageAlignment = ContentAlignment.MiddleCenter
        End Sub

        Public Sub SetCollapsedState(state As eCollapsedState)
            Select Case state
                Case eCollapsedState.Collapsed
                    Me.Image = My.Resources.Collapsed
                Case eCollapsedState.Expanded
                    Me.Image = My.Resources.Expanded
                Case eCollapsedState.NoChildren
                    Me.Image = Nothing
            End Select
        End Sub

    End Class

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEParentRowHeaderVisualizer implements a EwERowHeaderVisualizer visualizer
    ''' for rendering EwE hierarchical parent row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cVisualizerEwEParentRowHeader
        Inherits cEwEGridRowHeaderVisualizer

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEChildRowHeaderVisualizer implements a EwERowHeaderVisualizer visualizer
    ''' for rendering EwE hierarchical child row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------

    Public Class cVisualizerEwEChildRowHeader
        Inherits cEwEGridRowHeaderVisualizer

        ''' <summary>Size of label indentation</summary>
        Private Const cINDENT_SIZE As Integer = 20

        Public Sub New()
            MyBase.New()
            Me.Indentation = cVisualizerEwEChildRowHeader.cINDENT_SIZE
        End Sub

    End Class

End Namespace
