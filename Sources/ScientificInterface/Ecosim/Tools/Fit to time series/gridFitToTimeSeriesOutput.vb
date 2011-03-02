#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2

#End Region ' Imports

Namespace Ecosim

    ''' <summary>
    ''' Grid for displaying Fit to time series run results
    ''' </summary>
    <CLSCompliant(False)> _
    Public Class gridFitToTimeSeriesOutput
        Inherits EwEGrid

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()
        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
        End Sub

    End Class

End Namespace
