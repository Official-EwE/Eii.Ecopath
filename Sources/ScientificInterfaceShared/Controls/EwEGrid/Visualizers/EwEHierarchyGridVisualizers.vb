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

        Public Sub New()
            MyBase.new()
            Me.ImageAlignment = ContentAlignment.MiddleCenter
        End Sub

        Public WriteOnly Property Expanded() As Boolean
            Set(ByVal bExpanded As Boolean)
                If bExpanded Then
                    Me.Image = My.Resources.Expanded
                Else
                    Me.Image = My.Resources.Collapsed
                End If
            End Set
        End Property

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
