' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modifications of layers where cells
    ''' have two values: set or cleared.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorTwoState
        Inherits cLayerEditorRaster

        Public Sub New()
            Me.New(Nothing, True)
        End Sub

        Public Sub New(typeGUI As Type, bAutoToggleCellValue As Boolean)
            MyBase.New(typeGUI)
            Me.AutoToggleCellValue = bAutoToggleCellValue
        End Sub

        Protected Property AutoToggleCellValue As Boolean = True

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Initialize"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Initialize(uic As cUIContext, layer As cDisplayLayer)
            MyBase.Initialize(uic, layer)
            Dim rl As cDisplayLayerRaster = DirectCast(layer, cDisplayLayerRaster)
            Me.CellValueMax = CSng(Math.Max(rl.ValueSet, rl.ValueClear))
            Me.CellValueMin = CSng(Math.Min(rl.ValueSet, rl.ValueClear))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.StartEdit"/>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub StartEdit(e As MouseEventArgs, map As ucMap)

            If (Not Me.IsEditable) Then Return

            MyBase.StartEdit(e, map)

            Dim ptClick As Point = map.PointToColRow(e.Location)

            If (Me.AutoToggleCellValue) Then

                ' Clicked on an empty cell?
                If Decimal.Equals(CSng(Me.Layer.Value(ptClick.Y, ptClick.X)), CSng(Me.Layer.ValueClear)) Then
                    ' #Yes: start setting values
                    Me.CellValue = CSng(Me.Layer.ValueSet)
                Else
                    ' #No: start clearing values
                    Me.CellValue = CSng(Me.Layer.ValueClear)
                End If

                If Me.GUI IsNot Nothing Then
                    ' Trigger GUI to update to the changes
                    Me.GUI.UpdateContent(Me)
                End If

            End If
        End Sub

    End Class

End Namespace