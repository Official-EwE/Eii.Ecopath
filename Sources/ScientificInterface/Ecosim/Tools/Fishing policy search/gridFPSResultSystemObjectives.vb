'==============================================================================
'
' $Log: gridFPSResultSystemObjectives.vb,v $
' Revision 1.7  2009/04/03 20:27:10  jeroens
' Prepared for more objective results
'
' Revision 1.6  2009/03/26 22:47:00  jeroens
' ClearData -> RemoveDataRows, uses new ClearRow method to properly clean up
'
' Revision 1.5  2009/01/31 00:57:02  joeb
' Fixed bug Diversity cell missing from output grid
'
' Revision 1.4  2008/12/15 15:55:35  jeroens
' no message
'
' Revision 1.3  2008/12/02 17:23:14  joeb
' Resized the fishing block selector to fit on the form
'
' Revision 1.2  2008/11/19 19:21:34  jeroens
' Fixed crash
'
' Revision 1.1  2008/11/19 14:40:35  jeroens
' Moved and renamed
'
' Revision 1.1  2008/09/26 07:31:52  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper grid for Fishing Policy Search interface, displaying iterations
    ''' by system objective.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class gridFPSResultSystemObjectives
        : Inherits EwEGrid

        Private m_iColDynamic As Integer = 0

        Private Enum eColumnTypes As Integer
            Iteration = 0
            Total
        End Enum

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Add dynamic cols manually
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length + _
                        [Enum].GetValues(GetType(eSearchCriteriaResultTypes)).Length)

            Me(0, eColumnTypes.Iteration) = New EwEColumnHeaderCell(My.Resources.HEADER_NUMCALLS)
            Me(0, eColumnTypes.Total) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTAL)

            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.TotalValue) = New EwEColumnHeaderCell(My.Resources.HEADER_NET_ECONOMIC_VALUE_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.Employment) = New EwEColumnHeaderCell(My.Resources.HEADER_SOCIAL)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.MandateReb) = New EwEColumnHeaderCell(My.Resources.HEADER_MANDATED_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.Ecological) = New EwEColumnHeaderCell(My.Resources.HEADER_ECOSYSTEM_STRUCTURE_ABBR)
            Me(0, eColumnTypes.Total + eSearchCriteriaResultTypes.BioDiversity) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS_DIVERSITY_ABBR)

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults, ByVal nSearchBlocks As Integer, ByRef pbc As ucParmBlockCodes)

            Dim aiBlocks() As Integer = results.BlockNumber
            Dim asResults() As Single = results.BlockResults
            Dim avm(aiBlocks.Length - 1) As SourceGrid2.VisualModels.Common
            Dim clr As Color
            Dim cnt As Integer = Me.RowsCount

            Me.Rows.Insert(cnt)
            Me(cnt, 0) = New EwERowHeaderCell(CStr(results.nCalls))
            Me(cnt, 1) = New Cell(CStr(results.Totals))

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

        Public Sub InsertColumns(ByVal colCnt As Integer)

            Me.m_iColDynamic = Me.ColumnsCount

            For i As Integer = 0 To colCnt - 1
                Me.Columns.Insert(Me.m_iColDynamic + i)
                Me(0, Me.m_iColDynamic + i) = New EwEColumnHeaderCell((i + 1).ToString)
            Next

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
        End Sub

    End Class

End Namespace


