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
    ''' A visualizer for rendering EwE row header cells.
    ''' </summary>
    ''' -------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class cEwEGridRowHeaderVisualizer
        : Inherits cEwEGridVisualizerBase

        Public Sub New()
            MyBase.new()
            Me.TextAlignment = ContentAlignment.MiddleLeft
            Me.WordWrap = True
        End Sub

    End Class

End Namespace