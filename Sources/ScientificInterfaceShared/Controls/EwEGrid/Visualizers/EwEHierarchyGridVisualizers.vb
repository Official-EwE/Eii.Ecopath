Option Strict On

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEParentRowHeaderVisualizer implements a EwERowHeaderVisualizer visualizer
    ''' for rendering EwE hierarchical parent row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
     Public Class cVisualizerEwECollapseExpandRowHeader
        : Inherits cEwEGridRowHeaderVisualizer

        Public Enum eCollapsedState As Integer
            NoChildren = 0
            Collapsed
            Expanded
        End Enum

        Public Sub New()
            MyBase.new()
            Me.ImageAlignment = ContentAlignment.MiddleCenter
        End Sub

        Public Sub SetCollapsedState(ByVal state As eCollapsedState)
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
    <CLSCompliant(False)> _
     Public Class cVisualizerEwEParentRowHeader
        : Inherits cEwEGridRowHeaderVisualizer

        Public Sub New()
            MyBase.new()
        End Sub

    End Class

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' EwEChildRowHeaderVisualizer implements a EwERowHeaderVisualizer visualizer
    ''' for rendering EwE hierarchical child row header cells
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
     Public Class cVisualizerEwEChildRowHeader
        : Inherits cEwEGridRowHeaderVisualizer

        ''' <summary>Size of label indentation</summary>
        Private Const cINDENT_SIZE As Integer = 20

        Public Sub New()
            MyBase.new()
            Me.Indentation = cVisualizerEwEChildRowHeader.cINDENT_SIZE
        End Sub

    End Class

End Namespace
