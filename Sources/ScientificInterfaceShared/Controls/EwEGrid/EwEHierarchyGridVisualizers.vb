'==============================================================================
'
' $Log: EwEHierarchyGridVisualizers.vb,v $
' Revision 1.1  2008/09/26 07:31:16  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:08  jeroens
' Separated from Scientific Interface
'
' Revision 1.1  2006/10/18 15:51:28  jeroens
' Initial version
'
'==============================================================================

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
        : Inherits cVisualizerEwERowHeader

        Public Sub New()
            MyBase.new()
            Me.ImageAlignment = ContentAlignment.MiddleCenter
        End Sub

        Public WriteOnly Property Expanded() As Boolean
            Set(ByVal bExpanded As Boolean)
                If bExpanded Then
                    Me.Image = My.Resources.minus
                Else
                    Me.Image = My.Resources.plus
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
        : Inherits cVisualizerEwERowHeader

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
        : Inherits cVisualizerEwERowHeader

        ''' <summary>Size of label indentation</summary>
        Private Const cINDENT_SIZE As Integer = 20

        Public Sub New()
            MyBase.new()
            Me.Indentation = cVisualizerEwEChildRowHeader.cINDENT_SIZE
        End Sub

    End Class

End Namespace
