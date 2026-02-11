' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
            MyBase.New(GetType(ucLayerEditorHabitat))
            Me.CellValue = 1.0!
        End Sub

        Public Property UseHabitatAreaCorrection As Boolean = False

        Protected Overrides Function SetCellValue(ptSet As System.Drawing.Point,
                                             value As Object,
                                             e As System.Windows.Forms.MouseEventArgs,
                                             ptClick As System.Drawing.Point) As Boolean

            If (Me.UIContext Is Nothing) Then Return False

            Dim core As cCore = Me.UIContext.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim iHab As Integer = CInt(Me.Layer.Data.Index)
            Dim sValue As Single = Math.Min(Math.Max(0.0!, CSng(value)), 1.0!)
            Dim sTotal As Single = 0

            If (Me.UseHabitatAreaCorrection) Then

                For i As Integer = 1 To core.nHabitats - 1
                    If (i = iHab) Then
                        sTotal += sValue
                    Else
                        sTotal += CSng(bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X))
                    End If
                Next

                If (sTotal > 1) Then
                    Dim sRemainer As Single = (1 - sValue)
                    For i As Integer = 1 To core.nHabitats - 1
                        If (i <> iHab) Then
                            ' Scale down other habitat capacities
                            bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X) = CSng(bm.LayerHabitat(i).Cell(ptSet.Y, ptSet.X)) * sRemainer
                        End If
                    Next
                End If

            End If

            Return MyBase.SetCellValue(ptSet, sValue, e, ptClick)

        End Function

    End Class

End Namespace