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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls.EwEGrid
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region

Namespace Ecopath.Output

    ''' =======================================================================
    ''' <summary>
    ''' Grid clas, showing Ecopath statistics values.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridEcopathStatistics
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
            Me(0, 0) = New EwEColumnHeaderCell(SharedResources.HEADER_PARAMETER)
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_VALUE)
            Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_UNITS)

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcoPathStats = Core.EcopathStats

            Dim aunitCurrOverTime As eUnitType() = New eUnitType() {eUnitType.Currency, eUnitType.Time}
            Dim strMaskCurrOverTime As String = SharedResources.HEADER_A_PER_B

            Dim aunitCurrTime As eUnitType() = New eUnitType() {eUnitType.Time}
            Dim strMaskCurrTime As String = "/{0}" ' Ah well

            Me.AddRow(SharedResources.HEADER_SUM_CONSUMPTION, source, eVarNameFlags.EcopathStatsTotalConsumption, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_EXPORTS, source, eVarNameFlags.EcopathStatsTotalExports, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_FLOW_RESP, source, eVarNameFlags.EcopathStatsTotalRespFlow, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_FLOW_DET, source, eVarNameFlags.EcopathStatsTotalFlowDetritus, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_THROUGHPUT, source, eVarNameFlags.EcopathStatsTotalThroughput, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_PROD, source, eVarNameFlags.EcopathStatsTotalProduction, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_MEAN_CATCH_TL, source, eVarNameFlags.EcopathStatsMeanTrophicLevelCatch)
            Me.AddRow(SharedResources.HEADER_GROSS_EFFICIENCY, source, eVarNameFlags.EcopathStatsGrossEfficiency)
            Me.AddRow(SharedResources.HEADER_SUM_NET_PP, source, eVarNameFlags.EcopathStatsTotalNetPP, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_PP_RESP, source, eVarNameFlags.EcopathStatsTotalPResp)
            Me.AddRow(SharedResources.HEADER_NET_PROD, source, eVarNameFlags.EcopathStatsNetSystemProduction, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_SUM_PPB, source, eVarNameFlags.EcopathStatsTotalPB)
            Me.AddRow(SharedResources.HEADER_SUM_BT, source, eVarNameFlags.EcopathStatsTotalBT, aunitCurrTime, strMaskCurrTime)
            Me.AddRow(SharedResources.HEADER_SUM_BnonDET, source, eVarNameFlags.EcopathStatsTotalBNonDet, eUnitType.Currency)
            Me.AddRow(SharedResources.HEADER_SUM_CATCH, source, eVarNameFlags.EcopathStatsTotalCatch, aunitCurrOverTime, strMaskCurrOverTime)
            Me.AddRow(SharedResources.HEADER_INDEX_CONNECTANCE, source, eVarNameFlags.EcopathStatsConnectanceIndex)
            Me.AddRow(SharedResources.HEADER_INDEX_ONMIVORY, source, eVarNameFlags.EcopathStatsOmnivIndex)
            Me.AddRow(SharedResources.HEADER_SUM_VALUE_MARKET, source, eVarNameFlags.EcopathStatsTotalMarketValue, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_VALUE_SHADOW, source, eVarNameFlags.EcopathStatsTotalShadowValue, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_VALUE, source, eVarNameFlags.EcopathStatsTotalValue, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_COST_FIXED, source, eVarNameFlags.EcopathStatsTotalFixedCost, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_COST_VARIABLE, source, eVarNameFlags.EcopathStatsTotalVarCost, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_COST, source, eVarNameFlags.EcopathStatsTotalCost, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_SUM_PROFIT, source, eVarNameFlags.EcopathStatsProfit, eUnitType.Monetary)
            Me.AddRow(SharedResources.HEADER_ECOPATH_PEDIGREE, source, eVarNameFlags.EcopathStatsPedigree)
            Me.AddRow(SharedResources.HEADER_MEASUREOFFIT, source, eVarNameFlags.EcopathStatsMeasureOfFit)

        End Sub

        Protected Overrides Sub FinishStyle()
            Me.Columns(eColumnTypes.Header).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.Units).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.Value).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            MyBase.FinishStyle()
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags)
            Me.AddRow(strHeader, source, vnf, Nothing, "")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal unitType As eUnitType)
            Me.AddRow(strHeader, source, vnf, New eUnitType() {unitType}, "{0}")
        End Sub

        Private Overloads Sub AddRow(ByVal strHeader As String, _
                        ByVal source As cEcoPathStats, ByVal vnf As eVarNameFlags, _
                        ByVal aUnitTypes() As eUnitType, ByVal strUnitMask As String)

            Dim iRow As Integer = Me.AddRow()
            Me(iRow, eColumnTypes.Header) = New EwERowHeaderCell(strHeader)
            Me(iRow, eColumnTypes.Value) = New PropertyCell(Me.PropertyManager, source, vnf)
            Me(iRow, eColumnTypes.Units) = New EwEUnitCell(strUnitMask, aUnitTypes)

        End Sub

    End Class

End Namespace
