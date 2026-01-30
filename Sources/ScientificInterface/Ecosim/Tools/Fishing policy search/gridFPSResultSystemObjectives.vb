' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.FishingPolicy
Imports SourceGrid2.Cells.Real
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper grid for Fishing Policy Search interface, displaying iterations
    ''' by system objective.
    ''' </summary>
    ''' -----------------------------------------------------------------------

    Public Class gridFPSResultSystemObjectives
        Inherits cEwEGrid

        Private m_iColDynamic As Integer = 0

        Private Enum eColumnTypes As Integer
            Iteration = 0
            Total
        End Enum

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Add dynamic cols manually
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length +
                        [Enum].GetValues(GetType(eSearchCriteriaResultTypes)).Length)

            Me(0, eColumnTypes.Iteration) = New cEwEColumnHeaderCell(SharedResources.HEADER_NUMCALLS)
            Me(0, eColumnTypes.Total) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTAL)

            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.Profit) = New cEwEColumnHeaderCell(SharedResources.HEADER_NET_ECONOMIC_VALUE_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.Employment) = New cEwEColumnHeaderCell(SharedResources.HEADER_SOCIAL)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.MandateReb) = New cEwEColumnHeaderCell(SharedResources.HEADER_MANDATED_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.Ecological) = New cEwEColumnHeaderCell(SharedResources.HEADER_ECOSYSTEM_STRUCTURE_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.BioDiversity) = New cEwEColumnHeaderCell(SharedResources.HEADER_BIODIVERSITY)

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults, nSearchBlocks As Integer, ByRef pbc As ucParmBlockCodes)

            Dim aiBlocks() As Integer = results.BlockNumber
            Dim asResults() As Single = results.BlockResults
            Dim avm(aiBlocks.Length - 1) As SourceGrid2.VisualModels.Common
            Dim clr As Color
            Dim cnt As Integer = Me.RowsCount

            Me.Rows.Insert(cnt)
            Me(cnt, eColumnTypes.Iteration) = New cEwERowHeaderCell(CStr(results.nCalls))
            Me(cnt, eColumnTypes.Total) = New Cell(CStr(results.Totals))

            For Each result As eSearchCriteriaResultTypes In [Enum].GetValues(GetType(eSearchCriteriaResultTypes))
                Me(cnt, eColumnTypes.Total + result) = New Cell(results.CriteriaValues(result).ToString)
            Next

            For i As Integer = 1 To aiBlocks.Length - 1
                clr = pbc.BlockColor(aiBlocks(i))
                avm(i) = New SourceGrid2.VisualModels.Common
                avm(i).BackColor = clr
            Next

            For iCol As Integer = Me.m_iColDynamic To Me.ColumnsCount - 1
                Me(cnt, iCol) = New Cell(asResults(iCol - Me.m_iColDynamic + 1))
                Me(cnt, iCol).VisualModel = avm(iCol - Me.m_iColDynamic + 1)
            Next

            ' Scroll this row into view
            Me.ShowCell(New SourceGrid2.Position(cnt, 0))

        End Sub

        Public Sub RemoveDataRows()

            Me.SuspendLayout()
            While Me.RowsCount > 1
                Me.ClearRow(1)
                Me.Rows.Remove(1)
            End While
            Me.ResumeLayout()

        End Sub

        Public Sub InsertColumns(colCnt As Integer)

            Me.m_iColDynamic = Me.ColumnsCount

            For i As Integer = 0 To colCnt - 1
                Me.Columns.Insert(Me.m_iColDynamic + i)
                Me(0, Me.m_iColDynamic + i) = New cEwEColumnHeaderCell((i + 1).ToString)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
        End Sub

    End Class

End Namespace


