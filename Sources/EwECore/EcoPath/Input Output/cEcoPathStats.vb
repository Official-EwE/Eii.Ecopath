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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cEcopathStats
    Inherits cCoreInputOutputBase

    Sub New(core As cCore, iDBID As Integer)
        MyBase.New(core)

        Dim val As cValue = Nothing

        Me.DBID = iDBID
        Me.m_dataType = eDataTypes.EcoPathStatistics
        Me.m_coreComponent = eCoreComponentType.Ecopath

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet)

            'TotalConsumption
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalConsumption, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalExports
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalExports, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalRespFlow
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalRespFlow, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalFlowDetritus
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalFlowDetritus, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalThroughput
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalThroughput, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalProduction
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalProduction, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'MeanTrophicLevelCatch
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'GrossEfficiency
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsGrossEfficiency, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalNetPP
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalNetPP, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalPResp
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalPResp, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'NetSystemProduction
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsNetSystemProduction, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalPB
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalPB, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalBT
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalBT, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalBNonDet
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalBNonDet, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalCatch
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalCatch, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'ConnectanceIndex
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsConnectanceIndex, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'OmnivIndex
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsOmnivIndex, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalMarketValue
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalMarketValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalShadowValue
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalShadowValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalValue
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalFixedCost
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalFixedCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalVarCost
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalVarCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalCost
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsTotalCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Profit
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsProfit, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Ecopath pedigree index
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsPedigree, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Ecopath pedigree measure of fit
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsMeasureOfFit, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Ecopath diversity index
            val = New cValue(core, New Single, eVarNameFlags.EcopathStatsDiversity, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcopathStats.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcopathStats. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In Me.m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        If CSng(value.Value) = cCore.NULL_VALUE Then
                            value.Status = eStatusFlags.Null Or eStatusFlags.NotEditable
                        Else
                            value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        End If

                End Select
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
                Return False
            End Try
        Next keyvalue
        Return True

    End Function

    Public Property TotalConsumption() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalConsumption))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalConsumption, value)
        End Set
    End Property

    Public Property TotalConsumptionStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalConsumption)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalConsumption, value)
        End Set
    End Property

    ' --

    Public Property TotalExports() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalExports))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalExports, value)
        End Set
    End Property

    Public Property TotalExportsStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalExports)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalExports, value)
        End Set
    End Property

    ' --

    Public Property TotalRespFlow() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalRespFlow))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalRespFlow, value)
        End Set
    End Property

    Public Property TotalRespFlowStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalRespFlow)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalRespFlow, value)
        End Set
    End Property

    ' --

    Public Property TotalFlowDetritus() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalFlowDetritus))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalFlowDetritus, value)
        End Set
    End Property

    Public Property TotalFlowDetritusStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalFlowDetritus)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalFlowDetritus, value)
        End Set
    End Property

    ' --

    Public Property TotalThroughput() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalThroughput))
        End Get
        Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalThroughput, value)
        End Set
    End Property

    Public Property TotalThroughputStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalThroughput)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalThroughput, value)
        End Set
    End Property

    ' --

    Public Property TotalProduction() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalProduction))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalProduction, value)
        End Set
    End Property

    Public Property TotalProductionStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalProduction)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalProduction, value)
        End Set
    End Property

    ' --

    Public Property MeanTrophicLevelCatch() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, value)
        End Set
    End Property

    Public Property MeanTrophicLevelCatchStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, value)
        End Set
    End Property

    ' --

    Public Property GrossEfficiency() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsGrossEfficiency))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsGrossEfficiency, value)
        End Set
    End Property

    Public Property GrossEfficiencyStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsGrossEfficiency)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsGrossEfficiency, value)
        End Set
    End Property

    ' --

    Public Property TotalNetPP() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalNetPP))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalNetPP, value)
        End Set
    End Property

    Public Property TotalNetPPStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalNetPP)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalNetPP, value)
        End Set
    End Property

    ' --

    Public Property TotalPResp() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalPResp))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalPResp, value)
        End Set
    End Property

    Public Property TotalPResptatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalPResp)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalPResp, value)
        End Set
    End Property

    ' --

    Public Property NetSystemProduction() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsNetSystemProduction))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsNetSystemProduction, value)
        End Set
    End Property

    Public Property NetSystemProductionStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsNetSystemProduction)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsNetSystemProduction, value)
        End Set
    End Property

    ' --

    Public Property TotalPB() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalPB))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalPB, value)
        End Set
    End Property

    Public Property TotalPBStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalPB)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalPB, value)
        End Set
    End Property

    ' --

    Public Property TotalBT() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalBT))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalBT, value)
        End Set
    End Property

    Public Property TotalBTStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalBT)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalBT, value)
        End Set
    End Property

    ' --

    Public Property TotalBNonDet() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalBNonDet))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalBNonDet, value)
        End Set
    End Property

    Public Property TotalBNonDetStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalBNonDet)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalBNonDet, value)
        End Set
    End Property

    ' --

    Public Property TotalCatch() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalCatch))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalCatch, value)
        End Set
    End Property

    Public Property TotalCatchStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalCatch)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalCatch, value)
        End Set
    End Property

    ' --

    Public Property ConnectanceIndex() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsConnectanceIndex))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsConnectanceIndex, value)
        End Set
    End Property

    Public Property ConnectanceIndexStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsConnectanceIndex)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsConnectanceIndex, value)
        End Set
    End Property

    ' --

    Public Property OmnivIndex() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsOmnivIndex))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsOmnivIndex, value)
        End Set
    End Property

    Public Property OmnivIndexStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsOmnivIndex)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsOmnivIndex, value)
        End Set
    End Property

    ' --

    Public Property TotalMarketValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalMarketValue))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalMarketValue, value)
        End Set
    End Property

    Public Property TotalMarketValueStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalMarketValue)
        End Get
        Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalMarketValue, value)
        End Set
    End Property

    ' --

    Public Property TotalShadowValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalShadowValue))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalShadowValue, value)
        End Set
    End Property

    Public Property TotalShadowValueStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalShadowValue)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalShadowValue, value)
        End Set
    End Property

    ' --

    Public Property TotalValue() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalValue))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalValue, value)
        End Set
    End Property

    Public Property TotalValueStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalValue)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalValue, value)
        End Set
    End Property

    ' --

    Public Property TotalFixedCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalFixedCost))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalFixedCost, value)
        End Set
    End Property

    Public Property TotalFixedCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalFixedCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalFixedCost, value)
        End Set
    End Property

    ' --

    Public Property TotalVarCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalVarCost))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalVarCost, value)
        End Set
    End Property

    Public Property TotalVarCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalVarCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalVarCost, value)
        End Set
    End Property

    ' --

    Public Property TotalCost() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsTotalCost))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsTotalCost, value)
        End Set
    End Property

    Public Property TotalCostStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsTotalCost)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsTotalCost, value)
        End Set
    End Property

    ' --

    Public Property Profit() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsProfit))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsProfit, value)
        End Set
    End Property

    Public Property ProfitStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsProfit)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsProfit, value)
        End Set
    End Property

    ' --

    Public Property Pedigree() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsPedigree))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsPedigree, value)
        End Set
    End Property

    Public Property PedigreeStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsPedigree)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsPedigree, value)
        End Set
    End Property

    ' --

    Public Property MeasureOfFit() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsMeasureOfFit))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsMeasureOfFit, value)
        End Set
    End Property

    Public Property MeasureOfFitStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsMeasureOfFit)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsMeasureOfFit, value)
        End Set
    End Property

    ' --

    Public Property DiversityIndex() As Single
        Get
            Return CSng(Me.GetVariable(eVarNameFlags.EcopathStatsDiversity))
        End Get
        Friend Set(value As Single)
            Me.SetVariable(eVarNameFlags.EcopathStatsDiversity, value)
        End Set
    End Property

    Public Property DiversityIndexStatus() As eStatusFlags
        Get
            Return Me.GetStatus(eVarNameFlags.EcopathStatsDiversity)
        End Get
        Friend Set(value As eStatusFlags)
            Me.SetStatus(eVarNameFlags.EcopathStatsDiversity, value)
        End Set
    End Property

End Class
