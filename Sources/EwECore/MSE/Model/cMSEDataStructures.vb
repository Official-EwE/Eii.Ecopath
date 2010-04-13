#Region " Imports "

Option Strict On
Imports Microsoft.VisualBasic

#End Region ' Imports

Public Enum eMSEEffortMode

    ''' <summary>Ecosim effort. Then effort is being regulate by the Quota.</summary>
    TrackUseQuota
    ''' <summary>Effort is being predicted via Ecosim. Then effort is being regulate by the Quota.</summary>
    PredictUseQuota
    ''' <summary>Ecosim effort.</summary>
    Tracking

End Enum

Public Class cMSEDataStructures

#Region "Public Data"

    Public NTrials As Integer

    ''' <summary>
    ''' Importance weight of a fleet on a group
    ''' </summary>
    ''' <remarks> fleets, groups</remarks>
    Public Fweight(,) As Single 'Fishing weight set by user. Weight/importance of a fleet

    ''' <summary>
    ''' Weighted relative catchablility for closed loop FWeight * RelQ
    ''' Use to update fishing effort during an Ecosim run 
    ''' </summary>
    ''' <remarks>
    ''' Fwc(ifleet, 0) initialized to Ecopath base value in Fwc(Fleet) = Fweight(iFlt, iGrp) * relQ(iFlt, iGrp)
    ''' Fwc(iFleet, 1) = Updated for each year in MSE.AccessFs
    ''' FishingEffort(iFleet) = FishingEffort(iFleet) * m_data.Fwc(iFleet, 0) / m_data.Fwc(iFleet, 1)
    ''' </remarks>
    Public Fwc(,) As Single
    Public Wftot() As Single 'sum of fishing weight for all species caught by a fleet  Wftot(iflt) = Wftot(iflt) + Fweight(iflt, igrp)

    Public Qgrow() As Single 'Max catchability increase. Catchability increase over time due to improved fishing efficiency
    Public BioRiskValue(,) As Single 'Lower and Upper boundry for Biomass risk
    Public CVbiomEst() As Single 'Biomass coefficient of variation
    Public CVFest() As Single 'Fishing effort coefficient of variation
    Public CVBiomT(,) As Single 'groups,years
    Public CVFT(,) As Single ' fleets,years

    Public VarQest() As Single 'Estimated variation in the estimation of fishing effort. Use in the first year of the simulation to vary effort. See Init
    Public KalGainQ() As Single
    Public VarQyear() As Single
    Public VarQgrow() As Single 'variation in catchability

    ''' <summary>
    ''' T/F flag tells the MSE if a trial has exceeded the lower biomasss risk boundry
    ''' </summary>
    Public BioR0() As Boolean
    ''' <summary>
    ''' T/F flag tells the MSE if a trial has exceeded the upper biomasss risk boundry
    ''' </summary>
    Public BioR1() As Boolean
    ''' <summary>
    ''' Number of trials biomass was outside the lower or upper risk boundry
    ''' </summary>
    Public BioRiskCount(,) As Integer

    Public AssessPower As Single
    Public GstockPred() As Single
    Public RstockPred() As Single
    Public KalmanGain() As Single
    Public QGrowUsed() As Single

    ''' <summary>sum of employment value over all the completed trials</summary>
    Public sumEmployVal As Single
    ''' <summary>sum of economic over all the completed trials</summary>
    Public SumTotVal As Single
    ''' <summary>sum of mandated value over all the completed trials</summary>
    Public sumManVal As Single
    ''' <summary>sum of ecological value (biomass) over all the completed trials</summary>
    Public sumEcoVal As Single
    ''' <summary>weighted sum of all values over all the completed trials</summary>
    Public sumWeightedValues As Single

    Public BestTotalValue As Single

    Public BaseEmployVal As Single
    Public BaseTotalVal As Single
    Public BaseManValue As Single
    Public BaseEcoVal As Single

    ''' <summary>
    ''' Use for EwE5 Closed Loop Fishing Rate Assessment method
    ''' </summary>
    ''' <remarks></remarks>
    Public AssessMethod As eAssessmentMethods

    Public BioStats As cMSESummaryStats
    Public CatchGroupStats As cMSESummaryStats
    Public CatchFleetStats As cMSESummaryStats
    Public EffortStats As cMSESummaryStats

    Public ProfitSum As cMSESummaryStats
    Public CostSum As cMSESummaryStats
    Public JobsSum As cMSESummaryStats

    Public Bestimate() As Single
    Public BestimateLast() As Single
    Public EffortMode As eMSEEffortMode

    Public BioBounds() As cMSEBounds
    ''' <summary>Catch by group bounds</summary>
    Public CatchGroupBounds() As cMSEBounds

    ''' <summary>Catch by fleet bounds</summary>
    Public CatchFleetBounds() As cMSEBounds

    ''' <summary>Effort by fleet bounds</summary>
    Public EffortFleetBounds() As cMSEBounds

    Public StopRun As Boolean

    Public SaveOutput As Boolean

    ''' <summary>
    ''' Time index to start the MSY search at
    ''' </summary>
    ''' <remarks></remarks>
    Public MSYStartTimeIndex As Integer
    Public MSYGroupWeight() As Single
    'VC added a boolean to ex/include fleets from MSY runs:
    Public MSYEvaluateFleet() As Boolean
    Public MSYEvaluateGroup() As Boolean

    ''' <summary>
    ''' run the MSY search without calling the interface
    ''' </summary>
    ''' <remarks></remarks>
    Public MSYRunSilent As Boolean

    ''' <summary>
    ''' If True the MSE will evalute the run using Value. If false it will use Catch Biomass.
    ''' </summary>
    ''' <remarks></remarks>
    Public MSYEvaluateValue As Boolean

    ''' <summary>
    ''' Year/Timestep to start the assessment on
    ''' </summary>
    ''' <remarks></remarks>
    Public StartYear As Integer


#End Region

#Region "Private data"

    Private m_curIter As Integer
    Private m_EPData As cEcopathDataStructures
    Private m_ESData As cEcosimDatastructures
    Private m_QuotaData As cQuotaDataStructures

#End Region

#Region " Constructor "

    Public Sub New(ByVal EPdata As cEcopathDataStructures, _
                   ByVal ESdata As cEcosimDatastructures, _
                   ByVal QuotaData As cQuotaDataStructures)
        Me.NTrials = 10 'default number of trials
        Me.EffortMode = eMSEEffortMode.TrackUseQuota
        Me.StopRun = False
        Me.m_EPData = EPdata
        Me.m_ESData = ESdata
        Me.m_QuotaData = QuotaData
    End Sub

#End Region 'Constructor

#Region "Methods and Properties"

    Public ReadOnly Property UseQuotaRegs() As Boolean
        Get
            'Quota regs are being applied if Effort is Predicting or QuotaTracking
            If Me.EffortMode = eMSEEffortMode.PredictUseQuota Or Me.EffortMode = eMSEEffortMode.TrackUseQuota Then
                Return True
            End If
            'NOT if EffortMode is Tracking 
            Return False
        End Get
    End Property

    Public Property CurrentIteration() As Integer
        Get
            Return m_curIter
        End Get
        Friend Set(ByVal value As Integer)
            m_curIter = value
        End Set
    End Property

    Public Sub clearBioRisk()
        ReDim BioRiskCount(NGroups, 1)
    End Sub

    ''' <summary>
    ''' Set default values for the Management Strategy Evaluation model cMSE
    ''' </summary>
    Public Sub Init()

        Try
            ' JS 28Jan2010: moved to datasource
            ''load the bounds /traffic light object with the values from the Quotas
            'Me.DefaultBioBounds()
            ''
            'Me.DefaultCatchBounds()

            Me.BioStats = New cMSESummaryStats(Me, Me.BioBounds, NGroups)
            Me.CatchGroupStats = New cMSESummaryStats(Me, Me.CatchGroupBounds, NGroups)
            Me.CatchFleetStats = New cMSESummaryStats(Me, Me.CatchFleetBounds, nFleets)
            Me.EffortStats = New cMSESummaryStats(Me, Me.EffortFleetBounds, nFleets)

            Me.ProfitSum = New cMSESummaryStats(Me, Nothing, 1)
            Me.JobsSum = New cMSESummaryStats(Me, Nothing, 1)
            Me.CostSum = New cMSESummaryStats(Me, Nothing, 1)

            'default values for MSY 
            'these values can be overridden by an MSE of MSY plugin
            Me.MSYRunSilent = False
            Me.MSYEvaluateValue = True
            Me.MSYStartTimeIndex = 2
            Me.StartYear = 1

        Catch ex As Exception
            cLog.Write(ex)
            Throw New ApplicationException("Init() " & ex.Message, ex)
        End Try

    End Sub

    ''' <summary>
    ''' Redimension variables and set default variable values.
    ''' </summary>
    Public Sub RedimVars()

        ReDim GstockPred(NGroups)
        ReDim RstockPred(NGroups)
        ReDim KalmanGain(NGroups)
        ReDim VarQest(nFleets), KalGainQ(nFleets), VarQyear(nFleets)
        ReDim VarQgrow(nFleets)
        ReDim Wftot(nFleets)

        ReDim Fweight(nFleets, NGroups)
        ReDim Qgrow(nFleets)
        ReDim Fwc(nFleets, 1)

        ReDim BioR0(NGroups)
        ReDim BioR1(NGroups)
        ReDim BioRiskValue(NGroups, 1)
        ReDim BioRiskCount(NGroups, 1)

        ReDim CVbiomEst(NGroups)
        ReDim CVFest(nFleets)

        ReDim QGrowUsed(nFleets)

        ReDim Bestimate(NGroups)
        ReDim BestimateLast(NGroups)

        ReDim MSYGroupWeight(NGroups)
        For iGrp As Integer = 1 To NGroups
            MSYGroupWeight(iGrp) = 1
        Next

        ReDim BioBounds(NGroups)
        ReDim Me.CatchGroupBounds(NGroups)
        ReDim Me.CatchFleetBounds(Me.nFleets)
        ReDim Me.EffortFleetBounds(Me.nFleets)

        'default assessment method
        ' Fs from biomass estimates by pool
        AssessMethod = eAssessmentMethods.CatchEstmBio

        AssessPower = 1

        'set default values
        For iGrp As Integer = 1 To NGroups

            ' CVbiomEst(iGrp) = 0.2
            GstockPred(iGrp) = 0.6
            KalmanGain(iGrp) = 0.65
            BioRiskValue(iGrp, 0) = 0.5 'lower
            BioRiskValue(iGrp, 1) = 2 'upper

            For iFlt As Integer = 1 To nFleets
                If Me.m_ESData.relQ(iFlt, iGrp) > 0 Then
                    Fweight(iFlt, iGrp) = 1
                End If
            Next
        Next

        For iFlt As Integer = 1 To nFleets
            Qgrow(iFlt) = 0.03
            ' CVFest(iFlt) = 0.3
        Next iFlt

    End Sub


    Public Sub redimTime(Optional ByVal originalNumberOfYears As Integer = cCore.NULL_VALUE)

        Try
            'if time has changed then try to preserve the values
            'if not or Preserve fails then set to defaults
            Dim bFullRedim As Boolean = True
            If CVBiomT IsNot Nothing Then

                Try
                    ReDim Preserve CVBiomT(NGroups, Me.nYears) 'groups,time
                    ReDim Preserve CVFT(nFleets, Me.nYears)  ' fleets,time
                    bFullRedim = False
                Catch ex As Exception
                    bFullRedim = True
                End Try

            End If

            If bFullRedim Then
                ReDim CVBiomT(NGroups, Me.nYears) 'groups,time
                ReDim CVFT(nFleets, Me.nYears)  ' fleets,time
            End If 'bFullRedim

            Dim firstYear As Integer = 1
            If (originalNumberOfYears <> cCore.NULL_VALUE) And (bFullRedim = False) Then
                'number of years has changed 
                'The original data was preserved just set defaults for the new values only
                firstYear = originalNumberOfYears + 1
            End If

            'set default values
            For iGrp As Integer = 1 To NGroups
                For it As Integer = firstYear To Me.nYears
                    Me.CVBiomT(iGrp, it) = 0.2
                Next
            Next

            For iFlt As Integer = 1 To nFleets
                For it As Integer = firstYear To Me.nYears
                    Me.CVFT(iFlt, it) = 0.3
                Next
            Next iFlt

        Catch ex As Exception

        End Try


    End Sub

    ''' <summary>
    ''' Load the Bounds/Traffic light objects with the values from the Quota data Blim and Bbase
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub DefaultBioBounds(ByVal igrp As Integer)

        Try
            If Me.m_QuotaData.Blim(igrp) >= 0 Then
                Me.BioBounds(igrp) = New cMSEBounds(igrp, Me.m_QuotaData.Blim(igrp), Me.m_QuotaData.Bbase(igrp))
            Else
                Me.BioBounds(igrp) = New cMSEBounds(igrp, Me.m_EPData.B(igrp) * 0.1F, Me.m_EPData.B(igrp) * 0.4F)
            End If
        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadBounds() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub DefaultCatchBoundsGroup(ByVal igrp As Integer)

        Dim LB As Single = 0.5F
        Dim UB As Single = 2.0F

        Try
            'Catch by Group
            Dim tCatch As Single = Me.m_EPData.fCatch(igrp)
            If tCatch > 0 Then
                Me.CatchGroupBounds(igrp) = New cMSEBounds(igrp, tCatch * LB, tCatch * UB)
            Else
                'no catch??? set the bounds to NULL_VALUE this will show up in the interface and we can decide what to do then
                Me.CatchGroupBounds(igrp) = New cMSEBounds(igrp, cCore.NULL_VALUE, cCore.NULL_VALUE)
            End If

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadBounds() Exception: " & ex.Message)
        End Try

    End Sub

    Public Sub DefaultCatchBoundsFleet(ByVal iflt As Integer)

        Dim LB As Single = 0.5F
        Dim UB As Single = 2.0F
        Dim sumCatch As Single

        Try
            sumCatch = 0
            'sum the ecopath catch for this fleet
            For igrp As Integer = 1 To NGroups
                sumCatch += Me.m_EPData.Landing(iflt, igrp) + Me.m_EPData.Discard(iflt, igrp)
            Next
            Me.CatchFleetBounds(iflt) = New cMSEBounds(iflt, sumCatch * LB, sumCatch * UB)
            Me.EffortFleetBounds(iflt) = New cMSEBounds(iflt, 0.5, 2)

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadBounds() Exception: " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Set variable to default values for a trial
    ''' </summary>
    ''' <remarks>Sets Wftot(), Fwc(), VarQyear(), VarQgrow(), VarQest(), KalGainQ() </remarks>
    Friend Sub InitForTrial()
        Dim iFlt As Integer, iGrp As Integer

        Array.Clear(BioR0, 0, BioR0.Length)
        Array.Clear(BioR1, 0, BioR1.Length)
        Array.Clear(Wftot, 0, Wftot.Length)
        Array.Clear(Fwc, 0, Fwc.Length)

        For iFlt = 1 To Me.nFleets
            For iGrp = 1 To Me.m_EPData.NumLiving
                Wftot(iFlt) = Wftot(iFlt) + Fweight(iFlt, iGrp)
                Fwc(iFlt, 0) = Fwc(iFlt, 0) + Fweight(iFlt, iGrp) * Me.m_ESData.relQ(iFlt, iGrp)
            Next
            If Wftot(iFlt) > 0 Then Fwc(iFlt, 0) = Fwc(iFlt, 0) / Wftot(iFlt)
            Fwc(iFlt, 1) = Fwc(iFlt, 0)
        Next iFlt

        For iFlt = 1 To Me.nFleets

            'If AssessMethod = 1 Then
            '    VarQyear(iFlt) = CSng((Fwc(iFlt, 0) * CVbiomEst(iFlt)) ^ 2.0F)
            'Else
            VarQyear(iFlt) = CSng((Fwc(iFlt, 0) * CVFest(iFlt)) ^ 2)
            'End If
            VarQgrow(iFlt) = CSng((1 / 3 - 1 / 4) * Qgrow(iFlt) ^ 2) ' var of uniform 0-qgrow
            If VarQgrow(iFlt) = 0 Then VarQgrow(iFlt) = 0.0001
            VarQest(iFlt) = VarQgrow(iFlt) * CSng((1 + Math.Sqrt(1 + 4 * VarQyear(iFlt) / VarQgrow(iFlt))) / 2)
            KalGainQ(iFlt) = VarQest(iFlt) / (VarQest(iFlt) + VarQyear(iFlt))
        Next iFlt

    End Sub

    Public Sub InitForRun()

        Try

            Me.BioStats.Init()
            Me.CatchGroupStats.Init()
            Me.CatchFleetStats.Init()
            Me.EffortStats.Init()

            Me.ProfitSum.Init()
            Me.CostSum.Init()
            Me.JobsSum.Init()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Throw New Exception("Exception in cMSEDataStructures.InitForRun() Failed to initialize statistics. " & ex.Message)
        End Try

    End Sub

    Public ReadOnly Property NGroups() As Integer
        Get
            Return Me.m_EPData.NumGroups
        End Get
    End Property

    Public ReadOnly Property nTimeSteps() As Integer
        Get
            Return Me.m_ESData.NTimes
        End Get
    End Property

    Public ReadOnly Property nYears() As Integer
        Get
            Return Me.m_ESData.NumYears
        End Get
    End Property

    Public ReadOnly Property nFleets() As Integer
        Get
            Return Me.m_EPData.NumFleet
        End Get
    End Property

#End Region

End Class

#Region "Traffic light bounds"

Public Class cMSEBounds
    Public Upper As Single
    Public Lower As Single
    Public Index As Single

    Public Sub New(ByVal ObjectIndex As Integer, ByVal LowerBound As Single, ByVal UpperBound As Single)
        Me.Lower = LowerBound
        Me.Upper = UpperBound
        Me.Index = ObjectIndex
    End Sub

End Class

#End Region

#Region "Helper class used to gather stats on output data "

Public Class cMSESummaryStats

    Private Enum eSumIndexes
        Mean
        Min
        Max
    End Enum

    Private m_mseData As cMSEDataStructures
    Private m_data(,) As Single

    ''' <summary>
    ''' Number of data points stored
    ''' </summary>
    Private m_n() As Integer

    ''' <summary>
    ''' Number of groupings(groups/fleets)
    ''' </summary>
    Private m_count As Integer
    'Private m_lstLinValues As List(Of List(Of Single))

    ''' <summary>
    ''' List of data by Group/iteration/time. Each iteration will have its own array of data points
    ''' </summary>
    Private m_lstValues As List(Of List(Of Single()))

    Private m_curIter As Integer

    'histogram bin width
    Private m_binWidth() As Single

    'number of bins
    Private m_nBins() As Integer

    'list of histogram by grouping(group/fleet...)
    Private m_lstHist As New List(Of Single())
    Private m_lstMeans As New List(Of Single())

    Private m_bounds() As cMSEBounds


    Public Sub New(ByVal MSEData As cMSEDataStructures, ByVal Bounds() As cMSEBounds, ByVal NumberOfData As Integer)

        m_mseData = MSEData
        m_count = NumberOfData - 1
        m_bounds = Bounds

    End Sub


    ''' <summary>
    ''' Add an Iteration/Model Run to the stats
    ''' </summary>
    ''' <remarks>Each iteration will have its own list of data points</remarks>
    Public Sub AddIteration()

        Try
            'Loop over the list of groupings
            'for each grouping add a new iteration (list of data points) to the existing iterations
            Me.m_curIter += 1
            For Each lst As List(Of Single()) In Me.m_lstValues
                Dim d() As Single
                ReDim d(Me.m_mseData.nTimeSteps)
                lst.Add(d)
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            cLog.Write(ex)
        End Try

    End Sub

    Public Sub Init()

        Try

            m_curIter = 0

            ReDim Me.m_data(2, Me.m_count)
            ReDim Me.m_n(Me.m_count)

            ReDim Me.m_binWidth(Me.m_count)
            ReDim Me.m_nBins(Me.m_count)

            For igrp As Integer = 0 To Me.m_count
                Me.m_data(eSumIndexes.Min, igrp) = Single.MaxValue
            Next

            Me.m_lstValues = New List(Of List(Of Single()))

            'data is stored by grouping/iteration/time
            'Each grouping contains a list of iterations List(Of List(Of Single))
            'Each iteration contains a list of data points List(Of Single)
            For i As Integer = 0 To Me.m_count

                'add a list of iteration for each grouping
                'iterations are added in AddIteration()
                Me.m_lstValues.Add(New List(Of Single()))
            Next

            'populate the histogram and the Mean values with empty data 
            'this way if they are used before ComputeStats() is called they will not return Null
            Dim values() As Single
            For i As Integer = 0 To Me.m_count
                ReDim values(Me.m_mseData.nTimeSteps)
                Me.m_lstHist.Add(values)
                Me.m_lstMeans.Add(values)
            Next

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Throw New ApplicationException(Me.ToString & ".Init() exception. " & ex.Message)
        End Try

    End Sub

    Public Sub AddValue(ByVal index As Integer, ByVal TimeIndex As Integer, ByVal Value As Single)
        Try

            index -= 1
            Me.m_data(eSumIndexes.Mean, index) += Value
            Me.m_data(eSumIndexes.Min, index) = Math.Min(Me.m_data(eSumIndexes.Min, index), Value)
            Me.m_data(eSumIndexes.Max, index) = Math.Max(Me.m_data(eSumIndexes.Max, index), Value)

            Me.m_n(index) += 1

            'data is stored in a list by grouping/iteration/time
            'each iteration will have its own list of data points added in AddIteration()
            'add a data point to the end of the list for this grouping(index), iteration(Me.m_curIter - 1) 

            'Me.m_curIter is incremented for each iteration(making it one based) see AddIteration()
            Me.m_lstValues.Item(index).Item(Me.m_curIter - 1)(TimeIndex) = Value

        Catch ex As Exception
            Debug.Assert(False, "MSE Failed to add a value to statistics data. " & ex.Message)
            cLog.Write(ex)
        End Try

    End Sub


    Public Sub ComputeStats()
        Dim means() As Single

        'Histogram
        Try
            'Clear out the old data
            'this addes a new array to the Histogram list
            Me.m_lstHist.Clear()
            For i As Integer = 0 To Me.m_count
                Me.m_lstHist.Add(Me.calcHistogram(i))
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ComputeStats() in calculation of histogram!")
        End Try


        'Mean
        Try
            Me.m_lstMeans.Clear()

            For Each lst As List(Of Single()) In Me.m_lstValues 'group/fleet
                'mean values are stored in a one based array to be consistent with the interface
                ReDim means(Me.m_mseData.nTimeSteps)
                Me.m_lstMeans.Add(means)

                For Each iterVals As Single() In lst 'iteration
                    For i As Integer = 1 To Me.m_mseData.nTimeSteps
                        'sum the iteration for this time step
                        means(i) += iterVals(i)
                    Next
                Next
            Next

            'now average the sums computed above 
            For Each means In Me.m_lstMeans
                For i As Integer = 1 To means.Length - 1
                    means(i) /= Me.m_curIter
                Next
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".ComputeStats() in calculation of Means!")
        End Try

    End Sub

    Public ReadOnly Property Mean(ByVal Index As Integer) As Single
        Get
            Index -= 1
            Return Me.m_data(eSumIndexes.Mean, Index) / Me.m_n(Index)
        End Get
    End Property

    Public ReadOnly Property Min(ByVal Index As Integer) As Single
        Get
            Index -= 1
            Return Me.m_data(eSumIndexes.Min, Index)
        End Get
    End Property

    Public ReadOnly Property Max(ByVal Index As Integer) As Single
        Get
            Index -= 1
            Return Me.m_data(eSumIndexes.Max, Index)
        End Get
    End Property

    Public ReadOnly Property Variance(ByVal Index As Integer) As Single
        Get
            Dim ss As Single
            Dim n As Single
            Try

                Debug.Assert(Index <= Me.m_lstValues.Count, "MSE Statistics Variance index out of bounds.")
                If Index > Me.m_lstValues.Count Then
                    Return 0
                End If

                Dim m As Single = Me.Mean(Index)

                Dim lstGrouping As List(Of Single()) = Me.m_lstValues.Item(Index - 1)
                For Each iterVals As Single() In lstGrouping
                    For it As Integer = 1 To Me.m_mseData.nTimeSteps
                        ss += CSng((iterVals(it) - m) ^ 2.0F)
                        n += 1
                    Next
                Next

            Catch ex As Exception
                cLog.Write(ex)
                System.Console.WriteLine(ex.ToString)
            End Try

            'Debug.Assert(n > 1, "MSE Statistics Variance No data to compute.")
            If n <= 1 Then Return 0
            Return ss / (n - 1)

        End Get

    End Property

    Public ReadOnly Property Std(ByVal Index As Integer) As Single
        Get
            Return CSng(Math.Sqrt(Me.Variance(Index)))
        End Get
    End Property

    Public ReadOnly Property CV(ByVal Index As Integer) As Single
        Get
            Return Me.Std(Index) / Me.Mean(Index)
        End Get
    End Property

    Public Function PercentageBelow(ByVal index As Integer, ByVal value As Single) As Single

        Dim hist() As Single = Me.Histogram(index)
        Dim min As Single = Me.Min(index)
        Dim binwidth As Single = Me.HistoBinWidths(index)
        Dim curBin As Single
        Dim prob As Single

        'sum the histogram values up to value
        'this assumes the histogram has been normalized
        For i As Integer = 0 To hist.Length - 1
            curBin = min + binwidth * (i + 1)

            If curBin <= value Then
                prob += hist(i)
            Else
                Exit For
            End If

        Next

        Return prob * 100

    End Function

    Public Function PercentageAbove(ByVal index As Integer, ByVal value As Single) As Single

        Dim hist() As Single = Me.Histogram(index)
        Dim min As Single = Me.Min(index)
        Dim binwidth As Single = Me.HistoBinWidths(index)
        Dim curBin As Single
        Dim prob As Single

        'this assumes the histogram has been normalized
        For i As Integer = 0 To hist.Length - 1
            curBin = min + binwidth * (i + 1)

            If curBin >= value Then
                prob += hist(i)
            End If

        Next

        Return prob * 100

    End Function

    Public ReadOnly Property BelowLimit(ByVal index As Integer) As Single
        Get
            If Me.m_bounds IsNot Nothing Then
                Return Me.PercentageBelow(index, Me.m_bounds(index).Lower)
            End If
            Return 0
        End Get
    End Property


    Public ReadOnly Property AboveLimit(ByVal index As Integer) As Single
        Get
            If Me.m_bounds IsNot Nothing Then
                Return Me.PercentageAbove(index, Me.m_bounds(index).Upper)
            End If
            Return 0
        End Get
    End Property

    Public ReadOnly Property Histogram(ByVal Index As Integer) As Single()
        Get
            Try
                Return Me.m_lstHist(Index - 1)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Histogram() Exception: " & ex.Message)
                Return Nothing
            End Try
        End Get
    End Property


    Public ReadOnly Property MeanValues(ByVal Index As Integer) As Single()
        Get
            Try
                Return Me.m_lstMeans(Index - 1)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Histogram() Exception: " & ex.Message)
                Return Nothing
            End Try
        End Get
    End Property



    Private ReadOnly Property calcHistogram(ByVal GroupingIndex As Integer) As Single()
        Get

            Dim n As Single
            Dim ibin As Integer 'index of the bin for a value
            Dim hist() As Single 'histogram array for this group

            'get the list of iterations for this Index(group, fleet...)
            Dim iterList As List(Of Single()) = Me.m_lstValues.Item(GroupingIndex)
            Dim min As Single = Me.Min(GroupingIndex + 1) 'min of this index
            Dim max As Single = Me.Max(GroupingIndex + 1) 'max value in this index

            Try

                m_nBins(GroupingIndex) = 100 'CInt(Me.m_n(GroupingIndex) / 100.0F) 'number of bins 
                m_binWidth(GroupingIndex) = (max - min) / m_nBins(GroupingIndex) 'bin width
                'alternative bin width algo some number of bins (10) for one standard deviation
                'm_binWidth(GroupingIndex) = CSng(Me.Std(GroupingIndex + 1) / 10)
                'm_nBins(GroupingIndex) = CInt((max - min) / m_binWidth(GroupingIndex))

                ReDim hist(m_nBins(GroupingIndex) - 1)

                'only calculate the histogram if there are values
                If (max - min) > 0 Then

                    'loop over all the iteration for this group
                    For Each iterVals As Single() In iterList

                        'all the data points for this iteration
                        For it As Integer = 1 To Me.m_mseData.nTimeSteps
                            ibin = CInt(Math.Truncate((iterVals(it) - min) / m_binWidth(GroupingIndex)))
                            If ibin >= m_nBins(GroupingIndex) Then
                                'this value must be the max bump it down into the last bin
                                'Debug.Assert(val = max, "MSE histogram caluclation binning problem!")
                                ibin = m_nBins(GroupingIndex) - 1
                            End If
                            hist(ibin) += 1
                            n += 1
                        Next

                    Next

                    Dim pTot As Single
                    'normalize the counts
                    For i As Integer = 0 To m_nBins(GroupingIndex) - 1
                        hist(i) /= n
                        pTot += hist(i)
                    Next

                    System.Console.WriteLine("Total probability for index " & GroupingIndex.ToString & " = " & pTot.ToString)

                End If

            Catch ex As Exception
                Debug.Assert(False, "Exception calculating histogram for index " & GroupingIndex.ToString & " " & ex.Message)
            End Try

            'Debug.Assert(p = 1)
            Return hist

        End Get

    End Property

    Public ReadOnly Property HistoBinWidths(ByVal Index As Integer) As Single
        Get
            Index -= 1
            Return m_binWidth(Index)
        End Get
    End Property

    Public ReadOnly Property HistoNBins(ByVal Index As Integer) As Integer
        Get
            Index -= 1
            Return Me.m_nBins(Index)
        End Get
    End Property


    ''' <summary>
    ''' Return the stored values for an iteration as an array
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <param name="Iteration"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property Values(ByVal Index As Integer, ByVal Iteration As Integer) As Single()
        Get
            Dim vals() As Single
            Try
                Debug.Assert(Index <= Me.m_lstValues.Count, Me.ToString & ".Values() Index out of bounds!")
                Debug.Assert(Iteration <= Me.m_lstValues.Item(Index - 1).Count, Me.ToString & ".Values() Iteration index out of bounds!")
                vals = Me.m_lstValues.Item(Index - 1).Item(Iteration - 1)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Values() Exception: " & ex.Message)
                ReDim vals(Me.m_mseData.nTimeSteps)
            End Try

            Return vals

        End Get

    End Property

    Public ReadOnly Property Count() As Integer
        Get
            Return Me.m_lstValues.Count
        End Get
    End Property

    Public ReadOnly Property nIterations(ByVal index As Integer) As Integer
        Get
            Return Me.m_lstValues.Item(index - 1).Count
        End Get
    End Property


    Public Overrides Function ToString() As String
        Dim buf As New System.Text.StringBuilder
        For i As Integer = 1 To Me.m_count
            buf.Append("Mean=" & Me.Mean(i).ToString & ", Min=" & Me.Min(i).ToString & ", " & _
                       "Max=" & Me.Max(i).ToString & ", Variance= " & Me.Variance(i).ToString & ", STD= " & Me.Std(i).ToString & ", ")
        Next
        Return buf.ToString
    End Function


End Class

#End Region


