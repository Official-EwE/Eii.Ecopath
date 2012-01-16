#Region " Imports "

Option Strict On

Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of habitat layers. Setting
    ''' a cell value in one habitat will clear the cell values in another.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorHabitat
        Inherits cLayerEditorRange

        Public Sub New()
            MyBase.New()
        End Sub

        Protected Overrides Sub SetCellValue(ptSet As System.Drawing.Point, _
                                             value As Object, _
                                             e As System.Windows.Forms.MouseEventArgs, _
                                             ptClick As System.Drawing.Point)

            If (Me.UIContext Is Nothing) Then Return

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim iHab As Integer = CInt(Me.Layer.Data.Index)
            Dim sValue As Single = Math.Min(Math.Max(0.0!, CSng(value)), 1.0!)
            Dim sTotal As Single = 0

            ' Hackerdihack: scale cell for all other habitat layers
            For i As Integer = 1 To core.nHabitats
                If (i = iHab) Then
                    sTotal += sValue
                Else
                    sTotal += CSng(bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X))
                End If
            Next

            If (sTotal > 1) Then
                Dim sRemainer As Single = (1 - sValue)
                For i As Integer = 1 To core.nHabitats
                    If (i <> iHab) Then
                        ' Scale down other habitat capacities
                        bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X) = CSng(bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X)) * sRemainer
                    End If
                Next
            End If

            MyBase.SetCellValue(ptSet, sValue, e, ptClick)

        End Sub

    End Class

End Namespace