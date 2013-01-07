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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor base class that supports manual modification of Ecospace 
    ''' layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerEditorRegion
        Inherits cLayerEditor

#Region " Construction "

        Public Sub New()
            MyBase.New(GetType(ucLayerEditorRegion))
            Me.CellValue = 1
        End Sub

#End Region ' Construction

#Region " Public interfaces "

        Friend Sub CreateMPARegions()

            If (Me.UIContext Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim mpa As cEcospaceLayerMPA = bm.LayerMPA
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters

            parms.nRegions = Me.UIContext.Core.nMPAs

            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol
                    Me.Layer.Value(iRow, iCol) = mpa.Cell(iRow, iCol)
                Next iCol
            Next iRow
            Me.UpdateGUI()

            Me.Layer.Update(cLayer.eChangeFlags.Map)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create regions from Habitats.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub CreateHabitatRegions()

            If (Me.UIContext Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim mpa As cEcospaceLayerMPA = bm.LayerMPA
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            Dim sValMax As Single = 0
            Dim iHabMax As Integer = 0

            parms.nRegions = Me.UIContext.Core.nHabitats

            For iRow As Integer = 1 To bm.InRow
                For iCol As Integer = 1 To bm.InCol
                    sValMax = 0
                    iHabMax = 0
                    For iHab As Integer = 1 To parms.nRegions
                        Dim sVal As Single = CSng(bm.LayerHabitat(iHab).Cell(iRow, iCol))
                        If sVal > sValMax Then
                            sValMax = sVal : iHabMax = iHab
                        End If
                    Next
                    Me.Layer.Value(iRow, iCol) = iHabMax
                Next iCol
            Next iRow

            Me.UpdateGUI()
            Me.Layer.Update(cLayer.eChangeFlags.Map)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create regions from cells.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub CreateCellRegions(iClusterSize As Integer)

            If (Me.UIContext Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim depth As cEcospaceLayerDepth = bm.LayerDepth
            Dim parms As cEcospaceModelParameters = Me.UIContext.Core.EcospaceModelParameters
            Dim iRegion As Integer = 1

            For iRow As Integer = 1 To bm.InRow Step iClusterSize
                For iCol As Integer = 1 To bm.InCol Step iClusterSize
                    For i As Integer = 0 To iClusterSize
                        For j As Integer = 0 To iClusterSize
                            If depth.IsWaterCell(iRow + i, iCol + j) Then
                                Me.Layer.Value(iRow + i, iCol + j) = iRegion
                            End If
                        Next
                    Next
                    iRegion += 1
                Next iCol
            Next iRow
            parms.nRegions = iRegion

            Me.UpdateGUI()
            Me.Layer.Update(cLayer.eChangeFlags.Map)

        End Sub

#End Region ' Public interfaces

    End Class

End Namespace ' Controls.Map.Layers
