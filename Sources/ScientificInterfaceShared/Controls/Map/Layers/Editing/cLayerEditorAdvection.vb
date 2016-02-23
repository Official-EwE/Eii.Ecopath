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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports manual modification of Ecospace advection data.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorAdvection
        Inherits cLayerEditorVector

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorRange))
        End Sub

#End Region ' Construction

        Public Overrides Sub Initialize(uic As cUIContext, layer As cDisplayRasterLayer)
            MyBase.Initialize(uic, layer)
            Me.CellValueMin = 0
            Me.CellValueMax = 1000
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Smooth layer data across water cells.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Smooth()

            If (Not Me.IsEditable) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim layer As EwECore.cEcospaceLayerVector = CType(Me.Layer.Data, cEcospaceLayerVector)
            Dim cnewx(,) As Single
            Dim cnewy(,) As Single
            Dim i As Integer, j As Integer
            Dim tx As Single
            Dim ty As Single
            Dim n As Integer

            ReDim cnewx(bm.InRow, bm.InCol)
            ReDim cnewy(bm.InRow, bm.InCol)

            For i = 1 To bm.InRow
                For j = 1 To bm.InCol
                    tx = 0
                    ty = 0
                    n = 0
                    For ii As Integer = i - 1 To i + 1
                        For jj As Integer = j - 1 To j + 1
                            If Not (ii = 0 Or jj = 0 Or ii = bm.InRow + 1 Or jj = bm.InCol + 1) And layerDepth.IsWaterCell(ii, jj) Then
                                tx += layer.XVelocity(ii, jj)
                                ty += layer.YVelocity(ii, jj)
                                n += 1
                            End If
                        Next jj
                    Next ii
                    If n > 0 Then
                        cnewx(i, j) = tx / n
                        cnewy(i, j) = ty / n
                    End If
                Next j
            Next i

            For i = 1 To bm.InRow
                For j = 1 To bm.InCol
                    If layerDepth.IsWaterCell(i, j) Then
                        layer.XVelocity(i, j) = cnewx(i, j)
                        layer.YVelocity(i, j) = cnewy(i, j)
                    End If
                Next
            Next
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub
    End Class

End Namespace