' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Enum eMPAOptimizationModels
    EcoSeed
    RandomSearch
End Enum


Public Class cMPAOptDataStructures

    Const MIN_RUN_LENGTH As Integer = 3

    Public CurRow As Integer
    Public CurCol As Integer
    Public bestrow As Integer
    Public bestcol As Integer
    Public StopRun As Boolean
    Public BoundaryWeight As Single
    Public MPASeed(,) As Integer
    Public SeedBlockSize2 As Integer

    'value of objective function  relative to the base value
    Public objFuncEconomicValue As Single
    Public objFuncMandatedValue As Single
    Public objFuncSocialValue As Single
    Public objFuncEcologicalValue As Single
    Public objFuncAreaBorder As Single

    Public objFuncBiodiversity As Single

    Public objFuncTotal As Single

    Public SearchType As eMPAOptimizationModels

    Public stepSize As Integer
    Public MaxArea As Integer
    Public MinArea As Integer
    Public nIterations As Integer

    Public iMPAtoUse As Integer
    Public bUseCellWeight As Boolean
    Public bUseRegions As Boolean

    Public EcoSpaceStartYear As Integer = 3
    Public EcoSpaceEndYear As Integer

    Private m_cells As List(Of cMPACell)

    Public Sub New()

        Me.SearchType = eMPAOptimizationModels.RandomSearch

        Me.nIterations = 100
        Me.stepSize = 10
        Me.MaxArea = 20
        Me.MinArea = 20
        Me.iMPAtoUse = 1

        Me.m_cells = New List(Of cMPACell)

    End Sub

    ''' <summary>
    ''' Clear out the current Ecoseed values
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Clear()
        Me.CurRow = 0
        Me.CurCol = 0
        Me.bestrow = 0
        Me.bestcol = 0

        Me.objFuncEconomicValue = 0
        Me.objFuncMandatedValue = 0
        Me.objFuncSocialValue = 0
        Me.objFuncEcologicalValue = 0
        Me.objFuncAreaBorder = 0
        Me.objFuncBiodiversity = 0
        Me.objFuncTotal = 0

    End Sub

    Public Sub setObjectiveValues(SearchData As cSearchDatastructures)

    End Sub

    Public Sub AddCell(Row As Integer, col As Integer, iMPA As Integer)
        Me.m_cells.Add(New cMPACell(Row, col, iMPA))
    End Sub

    Public Sub ClearCells()
        Me.m_cells.Clear()
    End Sub

    Public Function Cells() As List(Of cMPACell)
        Return Me.m_cells
    End Function

    Public ReadOnly Property MinRunLength() As Integer
        Get
            Return MIN_RUN_LENGTH
        End Get
    End Property

End Class



''' <summary>
''' MPA cell selected during a trial
''' </summary>
''' <remarks></remarks>
Public Class cMPACell
    Public Row As Integer
    Public Col As Integer
    Public iMPA As Integer

    Public Sub New(theRow As Integer, theCol As Integer, theMPAIndex As Integer)
        Me.Row = theRow
        Me.Col = theCol
        Me.iMPA = theMPAIndex
    End Sub

End Class