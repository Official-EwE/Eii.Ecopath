#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.FishingPolicy
Imports SourceGrid2.Cells.Real

#End Region

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper grid for Fishing Policy Search interface, displaying ...
    ''' </summary>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
     Public Class gridFPSResultFleetValue
        : Inherits EwEGrid

        Private m_Core As cCore
        Private m_FPManager As cFishingPolicyManager

        Public Sub New()

            MyBase.New()
            m_Core = cCore.GetInstance
            m_FPManager = m_Core.FishingPolicyManager

        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(m_Core.nFleets + 1, m_Core.nFleets + 3)
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.FPS_FV_RESULT_COL0)
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_INCOME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_PROFIT)

            For i As Integer = 1 To m_Core.nFleets
                Dim fltName As String = m_Core.FleetInputs(i).Name
                Me(i, 0) = New EwERowHeaderCell(fltName)
                Me(0, i + 2) = New EwEColumnHeaderCell(fltName)
            Next

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults)

            For i As Integer = 1 To m_Core.nFleets
                Me(i, 1) = New Cell(results.Income(i).ToString)
                Me(i, 2) = New Cell(results.Profitability(i).ToString)

                For j As Integer = 1 To m_Core.nFleets
                    Me(i, j + 2) = New Cell(results.CompensationMatrix(i, j).ToString)
                Next
            Next
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()
            Me.FixedColumns = 1
            Me.FixedColumnWidths = False
        End Sub

    End Class

End Namespace
