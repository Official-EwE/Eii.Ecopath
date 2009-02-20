'==============================================================================
'
' $Log: EcopathStatisticsEwEGrid.vb,v $
' Revision 1.2  2009/02/20 17:58:31  jeroens
' Renamed UnitCell to EwEUnitCell
'
' Revision 1.1  2009/01/29 23:37:21  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SourceGrid2

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class EcopathStatisticsEwEGrid
        : Inherits EwEGrid

        Private Enum eColumnTypes As Byte
            Header = 0
            Value
            Units
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 3)
            Me(0, 0) = New EwEColumnHeaderCell("Parameter")
            Me(0, 1) = New EwEColumnHeaderCell("Value")
            Me(0, 2) = New EwEColumnHeaderCell("Units")

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim sg As StyleGuide = StyleGuide.GetInstance()
            Dim source As cEcoPathStats = core.EcopathStats

            Dim aunitCurrOverTime As StyleGuide.eUnitType() = New StyleGuide.eUnitType() {StyleGuide.eUnitType.Currency, StyleGuide.eUnitType.Time}
            Dim strMask2 As String = "{0}/{1}"

            ' ToDo_JS: globalize this
            Me.AddRow("Sum of all consumption", source, eVarNameFlags.EcopathStatsTotalConsumption, aunitCurrOverTime, strMask2)
            Me.AddRow("Sum of all exports", source, eVarNameFlags.EcopathStatsTotalExports, aunitCurrOverTime, strMask2)
            Me.AddRow("Sum of all respiratory flows", source, eVarNameFlags.EcopathStatsTotalRespFlow, aunitCurrOverTime, strMask2)
            Me.AddRow("Sum of all flows into detritus", source, eVarNameFlags.EcopathStatsTotalFlowDetritus, aunitCurrOverTime, strMask2)
            Me.AddRow("Total system throughput", source, eVarNameFlags.EcopathStatsTotalThroughput, aunitCurrOverTime, strMask2)
            Me.AddRow("Sum of all production", source, eVarNameFlags.EcopathStatsTotalProduction, aunitCurrOverTime, strMask2)
            Me.AddRow("Mean trophic level of the catch", source, eVarNameFlags.EcopathStatsMeanTrophicLevelCatch)
            Me.AddRow("Gross efficiency (catch/net p.p.)", source, eVarNameFlags.EcopathStatsGrossEfficiency)
            Me.AddRow("Calculated total net primary production", source, eVarNameFlags.EcopathStatsTotalNetPP, aunitCurrOverTime, strMask2)
            Me.AddRow("Total primary production/total respiration", source, eVarNameFlags.EcopathStatsTotalFlowDetritus)
            Me.AddRow("Net system production", source, eVarNameFlags.EcopathStatsNetSystemProduction, aunitCurrOverTime, strMask2)
            Me.AddRow("Total primary production/total biomass", source, eVarNameFlags.EcopathStatsTotalPB)
            Me.AddRow("Total biomass/total throughput", source, eVarNameFlags.EcopathStatsTotalBT)
            Me.AddRow("Total biomass (excluding detritus)", source, eVarNameFlags.EcopathStatsTotalBNonDet, StyleGuide.eUnitType.Currency)
            Me.AddRow("Total catches", source, eVarNameFlags.EcopathStatsTotalCatch, aunitCurrOverTime, strMask2)
            Me.AddRow("Connectance Index", source, eVarNameFlags.EcopathStatsConnectanceIndex)
            Me.AddRow("System Omnivory Index", source, eVarNameFlags.EcopathStatsOmnivIndex)
            Me.AddRow("Total market value", source, eVarNameFlags.EcopathStatsTotalMarketValue, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Total shadow value", source, eVarNameFlags.EcopathStatsTotalShadowValue, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Total value", source, eVarNameFlags.EcopathStatsTotalValue, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Total fixed cost", source, eVarNameFlags.EcopathStatsTotalFixedCost, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Total variable cost", source, eVarNameFlags.EcopathStatsTotalVarCost, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Total cost", source, eVarNameFlags.EcopathStatsTotalCost, StyleGuide.eUnitType.Monetary)
            Me.AddRow("Profit", source, eVarNameFlags.EcopathStatsProfit, StyleGuide.eUnitType.Monetary)

        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags)
            Me.AddRow(strHeader, source, vnf, Nothing, "")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal unitType As StyleGuide.eUnitType)
            Me.AddRow(strHeader, source, vnf, New StyleGuide.eUnitType() {unitType}, "{0}")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal aUnitTypes() As StyleGuide.eUnitType, ByVal strUnitMask As String)

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, eColumnTypes.Header) = New EwERowHeaderCell(strHeader)
            Me(iRow, eColumnTypes.Value) = New PropertyCell(source, vnf)
            Me(iRow, eColumnTypes.Units) = New EwEUnitCell(strUnitMask, aUnitTypes)

        End Sub

    End Class

End Namespace
