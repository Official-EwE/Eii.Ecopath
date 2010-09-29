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

    ''' =======================================================================
    ''' <summary>
    ''' Grid clas, showing Ecopath statistics values.
    ''' </summary>
    ''' =======================================================================
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
            Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_PARAMETER)
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUE)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_UNITS)

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcoPathStats = Core.EcopathStats

            Dim aunitCurrOverTime As cStyleGuide.eUnitType() = New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}
            Dim strMask2 As String = My.Resources.GENERIC_HEADER_UNITUNIT

            Me.AddRow(My.Resources.HEADER_SUM_CONSUMPTION, source, eVarNameFlags.EcopathStatsTotalConsumption, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_EXPORTS, source, eVarNameFlags.EcopathStatsTotalExports, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_FLOW_RESP, source, eVarNameFlags.EcopathStatsTotalRespFlow, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_FLOW_DET, source, eVarNameFlags.EcopathStatsTotalFlowDetritus, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_THROUGHPUT, source, eVarNameFlags.EcopathStatsTotalThroughput, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_PROD, source, eVarNameFlags.EcopathStatsTotalProduction, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_MEAN_CATCH_TL, source, eVarNameFlags.EcopathStatsMeanTrophicLevelCatch)
            Me.AddRow(My.Resources.HEADER_GROSS_EFFICIENCY, source, eVarNameFlags.EcopathStatsGrossEfficiency)
            Me.AddRow(My.Resources.HEADER_SUM_NET_PP, source, eVarNameFlags.EcopathStatsTotalNetPP, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_PP_RESP, source, eVarNameFlags.EcopathStatsTotalPResp)
            Me.AddRow(My.Resources.HEADER_NET_PROD, source, eVarNameFlags.EcopathStatsNetSystemProduction, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_SUM_PPB, source, eVarNameFlags.EcopathStatsTotalPB)
            Me.AddRow(My.Resources.HEADER_SUM_BT, source, eVarNameFlags.EcopathStatsTotalBT)
            Me.AddRow(My.Resources.HEADER_SUM_BnonDET, source, eVarNameFlags.EcopathStatsTotalBNonDet, cStyleGuide.eUnitType.Currency)
            Me.AddRow(My.Resources.HEADER_SUM_CATCH, source, eVarNameFlags.EcopathStatsTotalCatch, aunitCurrOverTime, strMask2)
            Me.AddRow(My.Resources.HEADER_INDEX_CONNECTANCE, source, eVarNameFlags.EcopathStatsConnectanceIndex)
            Me.AddRow(My.Resources.HEADER_INDEX_ONMIVORY, source, eVarNameFlags.EcopathStatsOmnivIndex)
            Me.AddRow(My.Resources.HEADER_SUM_VALUE_MARKET, source, eVarNameFlags.EcopathStatsTotalMarketValue, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_VALUE_SHADOW, source, eVarNameFlags.EcopathStatsTotalShadowValue, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_VALUE, source, eVarNameFlags.EcopathStatsTotalValue, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_COST_FIXED, source, eVarNameFlags.EcopathStatsTotalFixedCost, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_COST_VARIABLE, source, eVarNameFlags.EcopathStatsTotalVarCost, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_COST, source, eVarNameFlags.EcopathStatsTotalCost, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_SUM_PROFIT, source, eVarNameFlags.EcopathStatsProfit, cStyleGuide.eUnitType.Monetary)
            Me.AddRow(My.Resources.HEADER_ECOPATH_PEDIGREE, source, eVarNameFlags.EcopathStatsPedigree)
            Me.AddRow(My.Resources.HEADER_MEASUREOFFIT, source, eVarNameFlags.EcopathStatsMeasureOfFit)

        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags)
            Me.AddRow(strHeader, source, vnf, Nothing, "")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal unitType As cStyleGuide.eUnitType)
            Me.AddRow(strHeader, source, vnf, New cStyleGuide.eUnitType() {unitType}, "{0}")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal aUnitTypes() As cStyleGuide.eUnitType, ByVal strUnitMask As String)

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, eColumnTypes.Header) = New EwERowHeaderCell(strHeader)
            Me(iRow, eColumnTypes.Value) = New PropertyCell(Me.PropertyManager, source, vnf)
            Me(iRow, eColumnTypes.Units) = New EwEUnitCell(strUnitMask, aUnitTypes)

        End Sub

    End Class

End Namespace
