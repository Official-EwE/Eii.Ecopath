#Region " Imports "

Option Strict On
Imports System.Drawing

#End Region ' Imports

Namespace Controls.EwEGrid

    <CLSCompliant(False)> _
    Public Class cEwEGridRowIndexVisualizer
        : Inherits cEwEGridVisualizerBase

        Public Sub New()
            MyBase.New()
            Me.TextAlignment = ContentAlignment.MiddleCenter
            Me.WordWrap = True
        End Sub

    End Class

End Namespace