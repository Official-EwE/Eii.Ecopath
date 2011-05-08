#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceGrid2.VisualModels

#End Region ' Imports

Namespace Controls.EwEGrid

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' A EwE Grid base visualizer that aligns its content.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cEwECellVisualizer
        : Inherits cEwEGridVisualizerBase

        ''' <summary>
        ''' Create a new visualizer.
        ''' </summary>
        ''' <param name="alignment">Alignment to choose. If not specified content will
        ''' be aligned <see cref="ContentAlignment.MiddleCenter"/>.</param>
        Public Sub New(Optional ByVal alignment As ContentAlignment = ContentAlignment.MiddleCenter)
            MyBase.New()
            Me.TextAlignment = alignment
            Me.WordWrap = False
            Me.AlignTextToImage = True
        End Sub

    End Class

End Namespace