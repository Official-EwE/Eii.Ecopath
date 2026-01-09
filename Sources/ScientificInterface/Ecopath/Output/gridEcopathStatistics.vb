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

Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    ''' =======================================================================
    ''' <summary>
    ''' Grid clas, showing Ecopath statistics values.
    ''' </summary>
    ''' =======================================================================
    
    Public Class gridEcopathStatistics
        Inherits cEwEGrid

        ''' <summary>The columns to show</summary>
        Private Enum eColumnTypes As Byte
            Header = 0
            Value
            Units
        End Enum

        ''' <summary>The rows to show</summary>
        Private m_vars() As eVarNameFlags = {
                eVarNameFlags.EcopathStatsTotalConsumption,
                eVarNameFlags.EcopathStatsTotalExports,
                eVarNameFlags.EcopathStatsTotalRespFlow,
                eVarNameFlags.EcopathStatsTotalFlowDetritus,
                eVarNameFlags.EcopathStatsTotalThroughput,
                eVarNameFlags.EcopathStatsTotalProduction,
                eVarNameFlags.EcopathStatsMeanTrophicLevelCatch,
                eVarNameFlags.EcopathStatsGrossEfficiency,
                eVarNameFlags.EcopathStatsTotalNetPP,
                eVarNameFlags.EcopathStatsTotalPResp,
                eVarNameFlags.EcopathStatsNetSystemProduction,
                eVarNameFlags.EcopathStatsTotalPB,
                eVarNameFlags.EcopathStatsTotalBT,
                eVarNameFlags.EcopathStatsTotalBNonDet,
                eVarNameFlags.EcopathStatsTotalCatch,
                eVarNameFlags.EcopathStatsConnectanceIndex,
                eVarNameFlags.EcopathStatsOmnivIndex,
                eVarNameFlags.EcopathStatsTotalMarketValue,
                eVarNameFlags.EcopathStatsTotalShadowValue,
                eVarNameFlags.EcopathStatsTotalValue,
                eVarNameFlags.EcopathStatsTotalFixedCost,
                eVarNameFlags.EcopathStatsTotalVarCost,
                eVarNameFlags.EcopathStatsTotalCost,
                eVarNameFlags.EcopathStatsProfit,
                eVarNameFlags.EcopathStatsPedigree,
                eVarNameFlags.EcopathStatsMeasureOfFit
        }

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 3)
            Me(0, 0) = New cEwEColumnHeaderCell(SharedResources.HEADER_PARAMETER)
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_VALUE)
            Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_UNITS)

            Me.FixedColumns = 1
            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcopathStats = Me.Core.EcopathStats
            Dim fmtVar As New cVarnameTypeFormatter()

            For i As Integer = 0 To m_vars.Count - 1
                Dim var As eVarNameFlags = m_vars(i)
                Me.AddRow(fmtVar.ToString(var), source, var)
            Next

            Dim model As cEwEModel = Me.Core.EwEModel
            Dim fmtDiv As New cDiversityIndexTypeFormatter()
            Me.AddRow(fmtDiv.ToString(model.DiversityIndexType), source, eVarNameFlags.EcopathStatsDiversity)

        End Sub

        Protected Overrides Sub FinishStyle()
            Me.Columns(eColumnTypes.Header).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.Units).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(eColumnTypes.Value).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            MyBase.FinishStyle()
        End Sub

        Private Overloads Sub AddRow(strHeader As String, source As cEcopathStats, vnf As eVarNameFlags)
            Dim iRow As Integer = Me.AddRow()
            Dim md As cVariableMetaData = source.GetVariableMetadata(vnf)

            Me(iRow, eColumnTypes.Header) = New cEwERowHeaderCell(strHeader)
            Me(iRow, eColumnTypes.Value) = New cPropertyCell(Me.PropertyManager, source, vnf)
            Me(iRow, eColumnTypes.Units) = New cEwEUnitCell(md.Units)

        End Sub

    End Class

End Namespace
