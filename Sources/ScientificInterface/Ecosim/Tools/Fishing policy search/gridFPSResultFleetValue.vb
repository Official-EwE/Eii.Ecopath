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

        Private m_FPManager As cFishingPolicyManager

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Property UIContext() As cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(ByVal value As cUIContext)
                MyBase.UIContext = value
                If value IsNot Nothing Then
                    Me.m_FPManager = Core.FishingPolicyManager
                End If
            End Set
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(Core.nFleets + 1, Core.nFleets + 3)
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.FPS_FV_RESULT_COL0)
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_INCOME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_PROFIT)

            For i As Integer = 1 To Core.nFleets
                Dim fltName As String = Core.FleetInputs(i).Name
                Me(i, 0) = New EwERowHeaderCell(fltName)
                Me(0, i + 2) = New EwEColumnHeaderCell(fltName)
            Next

        End Sub

        Protected Overrides Sub FillData()

        End Sub

        Public Sub InsertOneIterResult(ByRef results As cFPSSearchResults)

            Debug.Assert(Me.m_FPManager IsNot Nothing)
            Debug.Assert(Me.UIContext IsNot Nothing)

            For i As Integer = 1 To Core.nFleets
                Me(i, 1) = New Cell(results.Income(i).ToString)
                Me(i, 2) = New Cell(results.Profitability(i).ToString)

                For j As Integer = 1 To Core.nFleets
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
