'==============================================================================
'
' $Log: cEcoPathStats.vb,v $
' Revision 1.1  2009/01/29 17:29:55  jeroens
' Initial version
'
'==============================================================================

Option Strict On
Imports EwECore.ValueWrapper
Imports EwEUtils.Core

Public Class cEcoPathStats
    Inherits cCoreInputOutputBase

    Sub New(ByRef theCore As cCore, ByVal iDBID As Integer)
        MyBase.New(theCore)

        Dim val As cValue = Nothing

        Me.DBID = iDBID
        Me.m_dataType = eDataTypes.EcoPathStatistics
        Me.m_coreComponent = eCoreComponentType.EcoPath

        Try

            Me.m_ValidationStatus = New cVariableStatus(Me, eStatusFlags.OK, "", eVarNameFlags.NotSet, eDataTypes.EcoSimStatistics, eCoreComponentType.EcoSim, Index, cCore.NULL_VALUE)

            'TotalConsumption
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalConsumption, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalExports
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalExports, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalRespFlow
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalRespFlow, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalFlowDetritus
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalFlowDetritus, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalThroughput
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalThroughput, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalProduction
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalProduction, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'EcopathStatsMeanTrophicLevelCatch
            val = New cValue(New Single, eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'GrossEfficiency
            val = New cValue(New Single, eVarNameFlags.EcopathStatsGrossEfficiency, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalNetPP
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalNetPP, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalPResp
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalPResp, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'NetSystemProduction
            val = New cValue(New Single, eVarNameFlags.EcopathStatsNetSystemProduction, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalPB
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalPB, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalBT
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalBT, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalBNonDet
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalBNonDet, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalCatch
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalCatch, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'ConnectanceIndex
            val = New cValue(New Single, eVarNameFlags.EcopathStatsConnectanceIndex, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'OmnivIndex
            val = New cValue(New Single, eVarNameFlags.EcopathStatsOmnivIndex, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalMarketValue
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalMarketValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalShadowValue
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalShadowValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalValue
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalValue, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalFixedCost
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalFixedCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalVarCost
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalVarCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'TotalCost
            val = New cValue(New Single, eVarNameFlags.EcopathStatsTotalCost, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)
            'Profit
            val = New cValue(New Single, eVarNameFlags.EcopathStatsProfit, eStatusFlags.NotEditable Or eStatusFlags.ValueComputed, eValueTypes.Sng)
            Me.m_values.Add(val.varName, val)

            'set status flags to their default values
            Me.ResetStatusFlags()

        Catch ex As Exception
            Debug.Assert(False, "Error creating new cEcopathStats.")
            cLog.Write(Me.ToString & ".New(nGroups) Error creating new cEcopathStats. Error: " & ex.Message)
        End Try

    End Sub

    Friend Overrides Function ResetStatusFlags(Optional ByVal bForceReset As Boolean = False) As Boolean
        Dim i As Integer

        'tell the base class to do the default values
        MyBase.ResetStatusFlags(bForceReset)

        Dim keyvalue As KeyValuePair(Of eVarNameFlags, cValue)
        Dim value As cValue
        For Each keyvalue In m_values
            Try
                value = keyvalue.Value

                Select Case value.varType
                    Case eValueTypes.SingleArray, eValueTypes.IntArray, eValueTypes.PointArray, eValueTypes.BoolArray
                        For i = 0 To value.Length
                            value.Status(i) = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed
                        Next i

                    Case eValueTypes.Sng, eValueTypes.Int
                        value.Status = eStatusFlags.NotEditable Or eStatusFlags.ValueComputed

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
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalConsumption))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalConsumption, value)
        End Set
    End Property

    Public Property TotalConsumptionStatus() As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalConsumption)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalConsumption, value)
        End Set
    End Property

    ' --

    Public Property TotalExports(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalExports))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalExports, value)
        End Set
    End Property

    Public Property TotalExportsStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalExports)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalExports, value)
        End Set
    End Property

    ' --

    Public Property TotalRespFlow(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalRespFlow))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalRespFlow, value)
        End Set
    End Property

    Public Property TotalRespFlowStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalRespFlow)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalRespFlow, value)
        End Set
    End Property

    ' --

    Public Property TotalFlowDetritus(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalFlowDetritus))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalFlowDetritus, value)
        End Set
    End Property

    Public Property TotalFlowDetritusStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalFlowDetritus)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalFlowDetritus, value)
        End Set
    End Property

    ' --

    Public Property TotalThroughput(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalThroughput))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalThroughput, value)
        End Set
    End Property

    Public Property TotalThroughputStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalThroughput)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalThroughput, value)
        End Set
    End Property

    ' --

    Public Property TotalProduction(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalProduction))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalProduction, value)
        End Set
    End Property

    Public Property TotalProductionStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalProduction)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalProduction, value)
        End Set
    End Property

    ' --

    Public Property MeanTrophicLevelCatch(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, value)
        End Set
    End Property

    Public Property MeanTrophicLevelCatchStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsMeanTrophicLevelCatch, value)
        End Set
    End Property

    ' --

    Public Property GrossEfficiency(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsGrossEfficiency))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsGrossEfficiency, value)
        End Set
    End Property

    Public Property GrossEfficiencyStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsGrossEfficiency)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsGrossEfficiency, value)
        End Set
    End Property

    ' --

    Public Property TotalNetPP(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalNetPP))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalNetPP, value)
        End Set
    End Property

    Public Property TotalNetPPStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalNetPP)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalNetPP, value)
        End Set
    End Property

    ' --

    Public Property TotalPResp(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalPResp))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalPResp, value)
        End Set
    End Property

    Public Property TotalPResptatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalPResp)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalPResp, value)
        End Set
    End Property

    ' --

    Public Property NetSystemProduction(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsNetSystemProduction))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsNetSystemProduction, value)
        End Set
    End Property

    Public Property NetSystemProductionStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsNetSystemProduction)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsNetSystemProduction, value)
        End Set
    End Property

    ' --

    Public Property TotalPB(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalPB))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalPB, value)
        End Set
    End Property

    Public Property TotalPBStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalPB)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalPB, value)
        End Set
    End Property

    ' --

    Public Property TotalBT(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalBT))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalBT, value)
        End Set
    End Property

    Public Property TotalBTStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalBT)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalBT, value)
        End Set
    End Property

    ' --

    Public Property TotalBNonDet(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalBNonDet))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalBNonDet, value)
        End Set
    End Property

    Public Property TotalBNonDetStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalBNonDet)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalBNonDet, value)
        End Set
    End Property

    ' --

    Public Property TotalCatch(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalCatch))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalCatch, value)
        End Set
    End Property

    Public Property TotalCatchStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalCatch)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalCatch, value)
        End Set
    End Property

    ' --

    Public Property ConnectanceIndex(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsConnectanceIndex))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsConnectanceIndex, value)
        End Set
    End Property

    Public Property ConnectanceIndexStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsConnectanceIndex)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsConnectanceIndex, value)
        End Set
    End Property

    ' --

    Public Property OmnivIndex(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsOmnivIndex))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsOmnivIndex, value)
        End Set
    End Property

    Public Property OmnivIndexStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsOmnivIndex)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsOmnivIndex, value)
        End Set
    End Property

    ' --

    Public Property TotalMarketValue(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalMarketValue))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalMarketValue, value)
        End Set
    End Property

    Public Property TotalMarketValueStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalMarketValue)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalMarketValue, value)
        End Set
    End Property

    ' --

    Public Property TotalShadowValue(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalShadowValue))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalShadowValue, value)
        End Set
    End Property

    Public Property TotalShadowValueStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalShadowValue)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalShadowValue, value)
        End Set
    End Property

    ' --

    Public Property TotalValue(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalValue))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalValue, value)
        End Set
    End Property

    Public Property TotalValueStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalValue)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalValue, value)
        End Set
    End Property

    ' --

    Public Property TotalFixedCost(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalFixedCost))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalFixedCost, value)
        End Set
    End Property

    Public Property TotalFixedCostStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalFixedCost)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalFixedCost, value)
        End Set
    End Property

    ' --

    Public Property TotalVarCost(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalVarCost))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalVarCost, value)
        End Set
    End Property

    Public Property TotalVarCostStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalVarCost)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalVarCost, value)
        End Set
    End Property

    ' --

    Public Property TotalCost(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsTotalCost))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsTotalCost, value)
        End Set
    End Property

    Public Property TotalCostStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsTotalCost)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsTotalCost, value)
        End Set
    End Property

    ' --

    Public Property Profit(ByVal iGroup As Integer) As Single
        Get
            Return CSng(GetVariable(eVarNameFlags.EcopathStatsProfit))
        End Get
        Set(ByVal value As Single)
            SetVariable(eVarNameFlags.EcopathStatsProfit, value)
        End Set
    End Property

    Public Property ProfitStatus(ByVal iGroup As Integer) As eStatusFlags
        Get
            Return GetStatus(eVarNameFlags.EcopathStatsProfit)
        End Get
        Set(ByVal value As eStatusFlags)
            SetStatus(eVarNameFlags.EcopathStatsProfit, value)
        End Set
    End Property

End Class
