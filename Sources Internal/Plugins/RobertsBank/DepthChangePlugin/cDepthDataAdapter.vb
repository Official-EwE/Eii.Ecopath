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

Option Strict On
Option Explicit On

#Region " Imports "

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities
Imports EwECore.SpatialData

#End Region ' Imports


''' <summary>
''' External defined Depth Data Adapter for the Spatial Temporal framework
''' </summary>
''' <remarks></remarks>
Public Class cDepthDataAdapter
    Inherits cSpatialDataAdapter

#Region "Private variables"

    Private m_orgDepth(,) As Integer
    Private m_Ecospace As cEcoSpace
    Private m_SpaceData As cEcospaceDataStructures

    'has the original map value changed
    Private m_bChanged(,) As Boolean
#End Region

#Region "Construction"

    Public Sub New(theCore As cCore, EcoSpace As cEcoSpace, EcoSpaceData As cEcospaceDataStructures)
        MyBase.New(theCore, eVarNameFlags.LayerDepth, eCoreCounterTypes.NotSet)

        Me.m_Ecospace = EcoSpace
        Me.m_SpaceData = EcoSpaceData

    End Sub

#End Region

#Region "Private Modeling Code"

    Private Sub InitSpatialChanges()

        'Counts and re-sets the number of water cells in the core
        WaterCells()

        Me.SpaceData.bHasCapacityChanged = True
        Me.Ecospace.SetHabCap()

        'CalcHabitatArea() assumes that we have ONLY set water cells to land. Not the other direction.
        'If we have added water cells the new cells need PHabType(row,col,habitat) set Proportion of habitat type in a cell
        'For CalcHabitatArea() to correctly set the habitat areas
        Me.Ecospace.CalcHabitatArea()
        Me.Ecospace.SetMovementParameters()

    End Sub
    Private Sub WaterCells()
        'this finds the start and end rows and columns so that solvegrid doesn't go through every one
        Dim foundRow As Boolean
        Dim waterCtr As Integer = 0
        Dim iRow As Integer, iCol As Integer
        For iCol = 1 To SpaceData.InCol
            foundRow = False
            SpaceData.iStartRow(iCol) = SpaceData.InRow + 1
            SpaceData.iEndRow(iCol) = 0
            For iRow = 1 To SpaceData.InRow
                If SpaceData.Depth(iRow, iCol) > 0 Then
                    waterCtr = waterCtr + 1
                    SpaceData.iWaterCellIndex(waterCtr) = iRow
                    SpaceData.jWaterCellIndex(waterCtr) = iCol
                    If SpaceData.iStartRow(iCol) = SpaceData.InRow + 1 Then
                        SpaceData.iStartRow(iCol) = iRow
                        foundRow = True
                    End If
                    SpaceData.iEndRow(iCol) = iRow
                End If
            Next
            'SpaceData.iStartRow(j) = 1
            'SpaceData.iEndRow(j) = SpaceData.Inrow
        Next

        SpaceData.iTotalWaterCells = waterCtr
        SpaceData.nWaterCells = waterCtr

        For iRow = 1 To SpaceData.InRow
            SpaceData.jStartCol(iRow) = SpaceData.InCol + 1
            SpaceData.jEndCol(iRow) = 0
            For iCol = 1 To SpaceData.InCol
                If SpaceData.Depth(iRow, iCol) > 0 Then
                    If SpaceData.jStartCol(iRow) = SpaceData.InCol + 1 Then
                        SpaceData.jStartCol(iRow) = iCol
                    End If
                    SpaceData.jEndCol(iRow) = iCol
                End If
            Next
        Next
    End Sub

    Private ReadOnly Property Ecospace As cEcoSpace
        Get
            Return Me.m_Ecospace
        End Get
    End Property


    Private ReadOnly Property SpaceData As cEcospaceDataStructures
        Get
            Return Me.m_SpaceData
        End Get
    End Property

#End Region

#Region "Adapter Overrides "


    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="cSpatialDataAdapter.InitRun"/>
    ''' <remarks>
    ''' Overridden to clear the PP scale factor.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Overrides Sub InitRun()
        MyBase.InitRun()
        'ToDo save the initial state
        Try
            Me.m_orgDepth = New Integer(Me.SpaceData.InRow + 1, Me.SpaceData.InCol + 1) {}
            Array.Copy(Me.SpaceData.Depth, Me.m_orgDepth, Me.SpaceData.Depth.Length)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="cSpatialScalarDataAdapter.Adapt"/>
    ''' <remarks>
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Protected Overrides Function Adapt(ByVal bm As cEcospaceBasemap, _
                                              ByVal layer As cEcospaceLayer, _
                                              ByVal iTime As Integer, _
                                              ByVal dt As Date, _
                                              ByVal dataExternal As ISpatialRaster, _
                                              ByVal dNullValue As Double) As Boolean
        Dim bReturn As Boolean = False
        Try

            System.Console.WriteLine(Me.ToString + ".Adapt()")

            'This can only be used to convert water cells to land. Not the other direction
            'If a cell has been converted to water it needs habitats, PP and capacity set
            'This has no way of knowing what these data should be
            If Me.setDepthCells(bm, layer, iTime, dt, dataExternal, dNullValue) Then
                'Test set the capacity of adjacent cells 
                'Me.AdjustCapacity()
                Me.InitSpatialChanges()
                bReturn = True
            End If

        Catch ex As Exception
            System.Console.WriteLine("Exception: " & Me.ToString & ".Adapt() " & ex.Message)
            bReturn = False
        End Try

        Return bReturn

    End Function

    Private Sub AdjustCapacity()

        'JUST PROOF OF CONCEPT
        'This needs to have a distance weighting scheme
        For irow As Integer = 1 To Me.SpaceData.InRow
            For icol As Integer = 1 To Me.SpaceData.InCol
                If Me.m_bChanged(irow, icol) Then
                    'set capacity for all the groups
                    For igrp As Integer = 1 To Me.SpaceData.NGroups
                        'Me.SpaceData.HabCap() contains boundry cells 
                        'so don't worry about being out of bounds
                        For iirow As Integer = irow - 1 To irow + 1
                            For iicol As Integer = icol - 1 To icol + 1
                                'set the Habitat Capacity on the input map
                                'this will get transfered to the HabCap by the Capacity Model
                                Me.SpaceData.HabCapInput(iirow, iicol, igrp) = 0 'Me.SpaceData.HabCapInput(iirow, iicol, igrp) * 0.5F
                            Next iicol
                        Next iirow
                    Next igrp
                End If

            Next icol
        Next irow

    End Sub


    Private Function setDepthCells(ByVal bm As cEcospaceBasemap, _
                                              ByVal layer As cEcospaceLayer, _
                                              ByVal iTime As Integer, _
                                              ByVal dt As Date, _
                                              ByVal dataExternal As ISpatialRaster, _
                                              ByVal dNullValue As Double) As Boolean
        Debug.Assert(bm IsNot Nothing)
        Debug.Assert(layer IsNot Nothing)
        Debug.Assert(dataExternal IsNot Nothing)

        Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
        Dim msg As cMessage = Nothing
        Dim CellValue As Double = 0
        Dim bSuccess As Boolean = True ' Think positive. Really
        Dim iNumRows As Integer = bm.InRow
        Dim iNumCols As Integer = bm.InCol
        Dim iRow As Integer
        Dim iCol As Integer
        Dim dNoData As Integer = -999

        Try

            Me.initChangedArray(bm)

            iRow = 1
            For iRow = 1 To bm.InRow
                For iCol = 1 To bm.InCol
                    CellValue = dataExternal.Cell(iRow, iCol, dNoData)

                    Me.m_bChanged(iRow, iCol) = False
                    If CellValue <> CSng(layerDepth.Cell(iRow, iCol)) Then
                        'Depth has changed
                        'Set the new depth
                        If Me.SetCell(layer, iRow, iCol, CellValue) Then
                            'Keep track of which cells have changed
                            Me.m_bChanged(iRow, iCol) = True
                        Else
                            'Failed to set the value of this cell because of an exception in SetCell()
                            Return False
                        End If

                    End If 'CellValue <> CSng(layerDepth.Cell(iRow, iCol))
                Next iCol
            Next iRow

            If bSuccess Then
                '   Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_APPLIED, dataExternal.ToString()), eStatusFlags.OK)
            End If

        Catch ex As Exception
            '  Me.m_core.SpatialOperationLog.LogOperation(String.Format(My.Resources.CoreMessages.STATUS_EXCEPTION, ex.Message), eStatusFlags.ErrorEncountered)
            cLog.Write(ex, "cSpatialDataAdapter::Adapt(" & layer.ToString() & ")")
            bSuccess = False
        End Try

        Return bSuccess
    End Function

    Private Sub initChangedArray(ByVal bm As cEcospaceBasemap)
        If Me.m_bChanged Is Nothing Then
            Me.m_bChanged = New Boolean(bm.InRow + 1, bm.InCol + 1) {}
        End If
        Array.Clear(Me.m_bChanged, 0, Me.m_bChanged.Length)
    End Sub

    ''' -------------------------------------------------------------------
    ''' <inheritdocs cref="cSpatialDataAdapter.EndRun"/>
    ''' <summary>
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Overrides Sub EndRun()
        'EndRun Throws an exception in the base class for the Depth Layer
        'MyBase.EndRun()
        Try
            'ToDo restore the initial state
            Array.Copy(Me.m_orgDepth, Me.SpaceData.Depth, Me.SpaceData.Depth.Length)

            '
            Me.InitSpatialChanges()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub



#End Region ' Overrides

End Class
