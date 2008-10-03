'==============================================================================
'
' $Log: gridQuotaOptions.vb,v $
' Revision 1.2  2008/10/03 21:55:03  jeroens
' Mock-up improved
'
' Revision 1.1  2008/10/02 18:48:49  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports directive

Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    <CLSCompliant(False)> _
    Public Class gridQuotaOptions
        Inherits EwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            MaxEffort
            OptionNone
            OptionWeakestStock
            OptionStrongsetStockPlusDiscards
            OptionAsumeSelectiveFishing
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim iNumCols As Integer = [Enum].GetValues(GetType(eColumnTypes)).Length
            Dim src As cCoreInputOutputBase = Nothing

            Me.Redim(1, iNumCols)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            Me(0, eColumnTypes.MaxEffort) = New EwEColumnHeaderCell("Max Effort", StyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.OptionNone) = New EwEColumnHeaderCell("None")
            Me(0, eColumnTypes.OptionWeakestStock) = New EwEColumnHeaderCell("Weakest Stock")
            Me(0, eColumnTypes.OptionStrongsetStockPlusDiscards) = New EwEColumnHeaderCell("Strongest Stock + Discards")
            Me(0, eColumnTypes.OptionAsumeSelectiveFishing) = New EwEColumnHeaderCell("Assume Selective Fishing")

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim fleet As cCoreInputOutputBase = Nothing

            ' For each fleet
            For iRow As Integer = 1 To core.nFleets

                Me.AddRow()

                'Get the fleet info
                fleet = core.FleetInputs(iRow)

                Me(iRow, eColumnTypes.Index) = New EwERowHeaderCell(iRow)
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(fleet, eVarNameFlags.Name)
                Me(iRow, eColumnTypes.MaxEffort) = New SourceGrid2.Cells.Real.Cell(42.0!, GetType(Single))
                Me(iRow, eColumnTypes.OptionNone) = New SourceGrid2.Cells.Real.CheckBox(True)
                Me(iRow, eColumnTypes.OptionWeakestStock) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iRow, eColumnTypes.OptionStrongsetStockPlusDiscards) = New SourceGrid2.Cells.Real.CheckBox(False)
                Me(iRow, eColumnTypes.OptionAsumeSelectiveFishing) = New SourceGrid2.Cells.Real.CheckBox(False)

            Next iRow

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoSim
            End Get
        End Property

    End Class

End Namespace ' Ecosim
