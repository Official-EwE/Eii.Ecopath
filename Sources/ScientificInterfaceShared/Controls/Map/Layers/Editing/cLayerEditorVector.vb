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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor that supports interactions with <see cref="cEcospaceLayerVector">vector layers</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorVector
        Inherits cLayerEditor

#Region " Private vars "

        Private m_ptfDelta As PointF = Nothing
        Private m_szfCell As SizeF = Nothing

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            Me.New(Nothing)
        End Sub

        Public Sub New(ByVal t As Type)
            MyBase.New(t)
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.StartEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub StartEdit(ByVal ptClick As Point, ByVal buttons As MouseEventArgs)
            MyBase.StartEdit(ptClick, buttons)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.Edit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Edit(ByVal ptFrom As Point, _
                                  ByVal ptTo As Point, _
                                  ByVal ptDeltaMouse As Point, _
                                  ByVal szfCell As SizeF, _
                                  ByVal args As MouseEventArgs, _
                                  ByRef ptUpdateMin As Point, _
                                  ByRef ptUpdateMax As Point)

            Me.m_ptfDelta = New PointF(ptDeltaMouse.X, ptDeltaMouse.Y)
            Me.m_szfCell = New SizeF(szfCell.Width, szfCell.Height)

            MyBase.Edit(ptFrom, ptTo, ptDeltaMouse, szfCell, args, ptUpdateMin, ptUpdateMax)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="cLayerEditor.EndEdit"/>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndEdit()
            MyBase.EndEdit()
        End Sub

#End Region ' Public interfaces

#Region " Internal overrides "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overidden to set a vector into a single cell.
        ''' </summary>
        ''' <param name="ptSet">The cell location (Col, Row) to set.</param>
        ''' <param name="value">A array of 2 Single values</param>
        ''' <param name="e">Mouse event args accompanying this action.</param>
        ''' <param name="ptClick">The cell location (Col, Row) in the cursor.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub SetCellValue(ByVal ptSet As Point, _
                                             ByVal value As Object, _
                                             ByVal e As MouseEventArgs, _
                                             ByVal ptClick As Point)

            If (Not Me.IsEditable) Then Return

            ' Calc the distance the mouse has travelled
            Dim dx As Single = CSng(Math.Sqrt(Me.m_ptfDelta.X * Me.m_ptfDelta.X + Me.m_ptfDelta.Y * Me.m_ptfDelta.Y))
            ' Only process significant changes
            If dx <= 2 Then Return

            Dim sVal As Single = CSng(Me.CellValue)
            Me.Layer.Value(ptSet.Y, ptSet.X) = New Single() {Me.m_ptfDelta.X * sVal / dx, _
                                                             Me.m_ptfDelta.Y * sVal / dx}

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick up the cell value at a given point, and store this value in the
        ''' layer editor as the next value that will be set.
        ''' Overridden to pick up the scale factor at a given location.
        ''' </summary>
        ''' <param name="pt">The cell location to pick up a value from.</param>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Pickup(ByVal pt As System.Drawing.Point)

            Try
                ' JS: pt(X,Y) translated to value(row, col); it never fails to confuse me. Even if I wrote this code...
                Dim asValue As Single() = DirectCast(Me.Layer.Value(pt.Y, pt.X), Single())
                Me.CellValue = CSng(Math.Sqrt(asValue(0) * asValue(0) + asValue(1) * asValue(1)))

                ' Notify the editor GUI, if any
                If Me.GUI IsNot Nothing Then
                    Me.GUI.UpdateContent(Me)
                End If

            Catch ex As Exception
            End Try

        End Sub

#End Region ' Internal overrides

        Public Overrides Sub Fill()

            If (Not Me.IsEditable) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim l As cEcospaceLayerVector = CType(Me.Layer.Data, cEcospaceLayerVector)
            Dim v As Single = CSng(Me.CellValue)

            For i As Integer = 1 To bm.InRow
                For j As Integer = 1 To bm.InCol
                    If layerDepth.IsWaterCell(i, j) Then
                        l.XVelocity(i, j) = v
                        l.YVelocity(i, j) = v
                    End If
                Next j
            Next i
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

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