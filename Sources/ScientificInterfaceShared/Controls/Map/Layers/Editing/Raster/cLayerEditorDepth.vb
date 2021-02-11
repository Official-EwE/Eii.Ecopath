' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore

#End Region

Namespace Controls.Map.Layers

    Public Class cLayerEditorDepth
        Inherits cLayerEditorRaster

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorDepth))
        End Sub

        Public Property ProtectCoastLine() As Boolean

        Protected Overrides Function SetCellValue(ptSet As Point,
                                                  value As Object,
                                                  e As MouseEventArgs,
                                                  ptClick As System.Drawing.Point) As Boolean

            If (Not Me.IsEditable) Then Return False

            Dim layerDepth As cEcospaceLayerDepth = DirectCast(Me.Layer.Data, cEcospaceLayerDepth)
            Dim bIsLandCell As Boolean = (layerDepth.IsLandCell(ptSet.Y, ptSet.X))
            Dim bIsLandValue As Boolean = (CInt(value) = 0)

            If Me.ProtectCoastLine Then
                If (bIsLandCell <> bIsLandValue) Then Return False
            End If

            Return MyBase.SetCellValue(ptSet, value, e, ptClick)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the layer with the current <see cref="CellValue"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Reset()

            If (Not Me.IsEditable) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth

            For i As Integer = 1 To bm.InRow
                For j As Integer = 1 To bm.InCol
                    ' JS 01Feb16: Allow depth layer to be filled when protect coastline is OFF
                    If (layerDepth.IsWaterCell(i, j) Or Me.ProtectCoastLine = False) Then
                        Me.Layer.Value(i, j) = Me.CellValue
                    End If
                Next j
            Next i
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub
    End Class

End Namespace
