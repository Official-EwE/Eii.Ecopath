'==============================================================================
'
' $Log: IterResultSOGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.12  2008/08/02 03:04:18  jeroens
' Renamed resources
'
' Revision 1.11  2008/06/02 00:01:40  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.10  2008/04/07 02:31:20  jeroens
' Cleaning up resources
'
' Revision 1.9  2008/02/05 03:25:25  jeroens
' Added header
'
'==============================================================================

#Region "Imports directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecosim

    <CLSCompliant(False)> _
    Public Class IterResultSOGrid
        : Inherits EwEGrid

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            'Todo: Add dynamic color cells
            Me.Redim(1, 6)
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_NUMCALLS)
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTAL)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.FPS_ITER_RESULT_SO_ECON)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_SOCIAL)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_MANDATED_ABBR)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.FPS_ITER_RESULT_SO_ECOL)

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults, ByVal nSearchBlocks As Integer, ByRef pbc As ParmBlockCodes)

            Dim cnt As Integer = Me.RowsCount
            Me.Rows.Insert(cnt)
            Me(cnt, 0) = New EwERowHeaderCell(results.nCalls.ToString)
            Me(cnt, 1) = New Cell(results.Totals.ToString)
            For icrt As Integer = 1 To 4
                Me(cnt, 1 + icrt) = New Cell(results.CriteriaValues(icrt).ToString)
            Next
            Dim bClrs() As Integer = results.BlockNumber
            Dim visClr(bClrs.Length - 1) As SourceGrid2.VisualModels.Common

            For i As Integer = 1 To bClrs.Length - 1
                Dim clr As Color = pbc.BlockColor(bClrs(i))
                visClr(i) = New SourceGrid2.VisualModels.Common
                visClr(i).BackColor = clr
            Next
            Dim bRlts() As Single = results.BlockResults
            For col As Integer = 6 To Me.ColumnsCount - 1
                Me(cnt, col) = New Cell(bRlts(col - 5))
                Me(cnt, col).VisualModel = visClr(col - 5)
            Next

            ' Scroll this row into view
            Me.ShowCell(New SourceGrid2.Position(cnt, 0))

        End Sub

        Public Sub ClearData()

            If Me.RowsCount > 1 Then
                Me.Rows.RemoveRange(1, Me.RowsCount - 1)
            End If

        End Sub

        Public Sub InsertColumns(ByVal colCnt As Integer)

            Dim cnt As Integer = Me.ColumnsCount

            For i As Integer = 0 To colCnt - 1
                Me.Columns.Insert(cnt + i)
                Me(0, cnt + i) = New EwEColumnHeaderCell((i + 1).ToString)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
        End Sub

    End Class

End Namespace


