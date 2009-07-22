'==============================================================================
'
' $Log: cMSE.vb,v $
' Revision 1.10  2009/07/03 23:41:36  joeb
' MSE interface changes
'
' Revision 1.9  2009/06/01 17:07:38  joeb
' MSE debugging
'
' Revision 1.8  2009/05/26 16:45:24  joeb
' Added useEconomicPlugin and isEconomicAvailable to FPS and MSE
'
' Revision 1.7  2009/05/20 16:29:37  joeb
' Renamed eCallBackTypes.Stopped to RunCompleted
'
' Revision 1.6  2009/05/11 21:28:08  joeb
' Adding MSE data to Decision Support Tool (Multi Player Game)
'
' Revision 1.5  2009/04/02 20:54:38  jeroens
' Uses eSearchResultCriteriaTypes
'
' Revision 1.4  2008/12/09 19:49:17  joeb
' Ouput objects now use core data instead of buffering data
'
' Revision 1.3  2008/12/02 19:08:21  joeb
' Added flag for computation of EcoSim timestep ouput
'
' Revision 1.2  2008/11/28 16:54:14  joeb
' Cleaned up ToDo's
'
' Revision 1.1  2008/09/26 07:30:27  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports EwECore
Imports EwECore.Ecosim
Imports EwEUtils.Core
Imports EwEPlugin

Namespace MSE

#Region "Public definitions"

    Public Enum eCallBackTypes
        Started
        RunCompleted
        IterationCompleted
        IterationStarted
    End Enum

    Public Delegate Sub MSECallBackDelegate(ByVal CallBackType As eCallBackTypes)

#End Region

#Region "MSE Class"

    ''' <summary>
    ''' Management Strategy Evaluation
    ''' </summary>
    ''' <remarks>This was the Closed Loop Simulation in EwE5</remarks>
    Public Class cMSE

#Region "Private data"


        Private m_data As cMSEDataStructures
        Private m_Ecosim As Ecosim.cEcoSimModel
        Private m_Search As cSearchDatastructures
        Private m_esData As cEcosimDatastructures

        Private m_epdata As cEcopathDataStructures

        Private m_nTrials As Integer

        Private m_CallbackDelegate As MSECallBackDelegate

        Private BestTime() As Single
        Private EcoValueBase As Single, ManValueBase As Single
        Private TotValBase As Single, EmployBase As Single

        Private m_pluginManager As cPluginManager
        Private m_bUsePluginData As Boolean

        Private WithEvents EconomicData As cEconomicDataSource

#End Region

#Region "Modeling code"

#Region "Private Properties"

        Private ReadOnly Property UsePlugin() As Boolean
            Get
                If Me.m_Search.MSEUseEconomicPlugin And (Me.m_pluginManager IsNot Nothing) Then
                    Return True
                End If
                Return False
            End Get
        End Property

#End Region

#Region "Initialization and Connection"

        Public Sub Init(ByRef MSEData As cMSEDataStructures, ByRef Ecosim As Ecosim.cEcoSimModel, ByRef SearchData As cSearchDatastructures, ByVal EcopathData As cEcopathDataStructures, ByVal PluginManager As cPluginManager)

            Me.m_data = MSEData
            Me.m_Ecosim = Ecosim
            Me.m_Search = SearchData

            Me.m_esData = m_Ecosim.m_Data
            Me.m_epdata = EcopathData
            Me.m_pluginManager = PluginManager

            Me.EconomicData = cEconomicDataSource.getInstance()

        End Sub

        Public Sub Connect(ByRef CallbackDelegate As MSECallBackDelegate)
            m_CallbackDelegate = CallbackDelegate
        End Sub


        Public Sub InitForRun()

            Try
                Dim iflt As Integer
                ReDim BestTime(m_epdata.NumGroups)

                Me.m_data.clearBioRisk()

                For iflt = 1 To m_epdata.NumFleet
                    'save qgrowth parameter so as not to interfere with value fitting simulations
                    Me.m_data.QGrowUsed(iflt) = m_data.Qgrow(iflt)
                Next

                'init RstockPred from GstockPred
                'GstockPred could have been altered by an interface
                For iflt = 1 To Me.m_epdata.NumLiving
                    Me.m_data.RstockPred(iflt) = (1 - Me.m_data.GstockPred(iflt)) * Me.m_esData.StartBiomass(iflt)
                Next

                Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onEcosimTimestep

                'initialize Ecosim
                m_Ecosim.Init(False)

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".InitForRun() Error:" & ex.Message, ex)
            End Try

        End Sub


        Private Sub setBestTotalValue()

            Try
                'Run Ecosim
                Me.m_Ecosim.Run()

                'get the base values from the search data
                Me.m_data.BaseTotalVal = Me.m_Search.totval
                Me.m_data.BaseEmployVal = Me.m_Search.Employ
                Me.m_data.BaseManValue = Me.m_Search.manvalue
                Me.m_data.BaseEcoVal = Me.m_Search.ecovalue

                'cal base BestTotalValue (TotValBase,EmployBase... were set in SetBaseValues()
                Me.m_data.BestTotalValue = Me.m_Search.ValWeight(eSearchCriteriaResultTypes.TotalValue) * Me.m_Search.totval / TotValBase + _
                                 Me.m_Search.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.m_Search.Employ / EmployBase + _
                                 Me.m_Search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.m_Search.manvalue / ManValueBase + _
                                 Me.m_Search.ValWeight(eSearchCriteriaResultTypes.Ecological) * Me.m_Search.ecovalue / EcoValueBase

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("MSE.setBestTotalValue() Error: " & ex.Message, ex)
            End Try


        End Sub


        Private Sub SetBaseValues()
            Dim i As Integer, j As Integer, Cval As Single

            'RunModelValue TotalTime, TotValBase, EmployBase, EcoValueBase, lnF(), N, False
            EcoValueBase = 0
            ManValueBase = 0
            TotValBase = 0
            EmployBase = 0

            For i = 1 To m_epdata.NumLiving
                EcoValueBase = EcoValueBase + Me.m_Search.BGoalValue(i)
                ManValueBase = ManValueBase + Me.m_Search.MGoalValue(i)
            Next

            EcoValueBase = EcoValueBase * Me.m_esData.NumYears
            ManValueBase = ManValueBase * Me.m_esData.NumYears
            If ManValueBase = 0 Then ManValueBase = 1 'to avoid division with zero

            For i = 1 To m_epdata.NumLiving
                For j = 1 To m_epdata.NumFleet
                    Cval = m_esData.StartBiomass(i) * Me.m_esData.relQ(j, i) * m_epdata.Market(j, i)
                    TotValBase = TotValBase + Cval  '.5 here assumes cost likely 80% of income
                    EmployBase = EmployBase + Cval * Me.m_Search.Jobs(j)
                Next
            Next

            If Me.m_Search.DiscountFactor > 0 Then
                TotValBase = Math.Abs(TotValBase) / Me.m_Search.DiscountFactor
                EmployBase = Math.Abs(EmployBase) / Me.m_Search.DiscountFactor
            End If

            ManValueBase = Math.Abs(ManValueBase)
            EcoValueBase = Math.Abs(EcoValueBase)

            If TotValBase < 1.0E-20 Then TotValBase = 1.0E-20
            If EmployBase < 1.0E-20 Then EmployBase = 1.0E-20
            If ManValueBase < 1.0E-20 Then ManValueBase = 1.0E-20
            If EcoValueBase < 1.0E-20 Then EcoValueBase = 1.0E-20

        End Sub

#End Region

#Region "Running and computational code"

        Public Sub Run()
            Dim itr As Integer

            Try

                CallBack(eCallBackTypes.Started)

                'turn off regulatory models for initialization
                Me.m_esData.PredictSimEffort = False
                Me.m_esData.DoClosedLoop = False


                'put the search mode to initialization for setting of base values
                Me.m_Search.SearchMode = eSearchModes.InitializingSearch
                Me.m_esData.bTimestepOutput = True

                'init the MSE data
                Me.InitForRun()
                Me.m_data.InitForRun()

                Me.m_Search.initForRun(Me.m_epdata, Me.m_esData)
                Me.m_Search.setMinSearchBlocks() 'set number of search blocks to one and dim FblockCodes()
                If Me.m_Search.BaseYear = 0 Then Me.m_Search.BaseYear = 1
                Me.m_Search.setBaseYearEffort(Me.m_esData)

                'sets MeanEmploy, MeanVal, MeanManVal, MeanEcoVal, MeanTotalValue
                Me.SetBaseValues()

                'runs Ecosim and gets the base values
                Me.setBestTotalValue()

                'turn the evaluator on for the trials
                'this will vary Effort (Ecosim.Fgear) and Catability (Ecosim.Qyear) via MSE.YearTimeStep() and MSE.AccessFs
                Me.m_Search.SearchMode = eSearchModes.MSE

                For itr = 1 To m_data.NTrials

                    m_data.CurrentIteration = itr
                    Me.CallBack(eCallBackTypes.IterationStarted)

                    'Set MSE data back to initial values for a new run
                    m_data.InitForTrial()

                    'run ecosim
                    Me.m_Ecosim.Run()

                    Me.summarizeEcosimEconomicData()

                    'post the search data to plugins
                    Me.PostPluginData()

                    Me.SumValues()
                    Me.CallBack(eCallBackTypes.IterationCompleted)

                Next

                'mean values are computed by the manager from the sums
                '      Me.getMeanValues(m_data.CurrentIteration)

                Me.dumpStats()

                CallBack(eCallBackTypes.RunCompleted)

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".Run() Error: " & ex.Message)
            End Try

        End Sub

        Private Sub dumpStats()

            System.Console.WriteLine("Biomass ranges")
            System.Console.Write(Me.m_data.BioSum.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Catch by group ranges")
            System.Console.Write(Me.m_data.CatchGroupSum.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Catch by fleet ranges")
            System.Console.Write(Me.m_data.CatchFleetSum.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Profit")
            System.Console.Write(Me.m_data.ProfitSum.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Cost")
            System.Console.Write(Me.m_data.CostSum.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Jobs")
            System.Console.Write(Me.m_data.JobsSum.ToString)
            System.Console.WriteLine()

        End Sub

        ''' <summary>
        ''' Tell any plugin that a search interation has completed
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub PostPluginData()
            If Me.m_Search.MSEUseEconomicPlugin And (Me.m_pluginManager IsNot Nothing) Then
                Me.m_pluginManager.PostRunSearchResults(Me.m_Search)
            End If
        End Sub

        Private Sub getMeanValues(ByVal NTrials As Integer)

            Me.m_data.sumEmployVal = Me.m_data.sumEmployVal / NTrials
            Me.m_data.SumTotVal = Me.m_data.SumTotVal / NTrials
            Me.m_data.sumManVal = Me.m_data.sumManVal / NTrials
            Me.m_data.sumEcoVal = Me.m_data.sumEcoVal / NTrials
            Me.m_data.sumWeightedValues = Me.m_data.sumWeightedValues / NTrials

        End Sub


        ''' <summary>
        ''' Sum results of Model run into Mean values
        ''' </summary>
        ''' <remarks>Once the trials have been finished the mean will be calculated from the sums in getMeanValues() (e.g. MeanEmploy) </remarks>
        Private Sub SumValues()

            m_data.sumEmployVal += Me.m_Search.Employ
            m_data.SumTotVal += Me.m_Search.totval
            m_data.sumManVal += Me.m_Search.manvalue
            m_data.sumEcoVal += Me.m_Search.ecovalue

            m_data.sumWeightedValues = m_data.sumWeightedValues + _
                    m_Search.ValWeight(eSearchCriteriaResultTypes.TotalValue) * Me.m_Search.totval / TotValBase + _
                    m_Search.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.m_Search.Employ / EmployBase + _
                    m_Search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.m_Search.manvalue / ManValueBase + _
                    m_Search.ValWeight(eSearchCriteriaResultTypes.Ecological) * Me.m_Search.ecovalue / EcoValueBase

        End Sub

  

        Private Sub CallBack(ByVal CallBackType As eCallBackTypes)
            Try
                System.Console.WriteLine("MSE Callback = " & CallBackType.ToString)
                m_CallbackDelegate(CallBackType)
            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        ''' <summary>
        ''' Count the number of times the Biomass is outside the lower or upper risk boundry
        ''' </summary>
        ''' <param name="Biomass"></param>
        ''' <remarks>The biomass risk count can only be one per trial</remarks>
        Friend Sub AccessBioRisk(ByVal Biomass() As Single)

            Try

                For i As Integer = 1 To m_epdata.NumLiving

                    If m_data.BioR0(i) = False And Biomass(i) < m_data.BioRiskValue(i, 0) * Me.m_esData.StartBiomass(i) Then
                        m_data.BioRiskCount(i, 0) = m_data.BioRiskCount(i, 0) + 1
                        m_data.BioR0(i) = True
                    End If

                    If m_data.BioR1(i) = False And Biomass(i) > m_data.BioRiskValue(i, 1) * Me.m_esData.StartBiomass(i) Then
                        m_data.BioRiskCount(i, 1) = m_data.BioRiskCount(i, 1) + 1
                        m_data.BioR1(i) = True
                    End If

                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".AccessBioRisk() Error: " & ex.Message, ex)
            End Try

        End Sub


        ''' <summary>
        ''' Set Fgear() and QYear() for a management strategy evaluation
        ''' </summary>
        ''' <param name="Fgear">Fishing Effort</param>
        ''' <param name="QYear">Catability growth per year</param>
        ''' <param name="iYear"></param>
        ''' <remarks>Called from Ecosim during a management strategy evaluation</remarks>
        Friend Sub VaryEffortCatchability(ByRef Fgear() As Single, ByRef QYear() As Single, ByVal iYear As Integer)

            Try

                For i As Integer = 1 To Me.m_epdata.NumFleet
                    If iYear > 1 Then
                        QYear(i) = QYear(i) * (1 + Me.m_data.QGrowUsed(i) * Rnd())
                        If m_data.Fwc(i, 1) > 0 Then Fgear(i) = Fgear(i) * m_data.Fwc(i, 0) / m_data.Fwc(i, 1)
                        If Fgear(i) < 1.0E-20 Then Fgear(i) = 1.0E-20
                    End If

                    If iYear = 1 Then Fgear(i) = CSng(Fgear(i) * (1 + Me.Normal * Math.Sqrt(m_data.VarQest(i))))

                Next i

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".YearTimeStep() Error: " & ex.Message, ex)
            End Try


        End Sub


        Friend Sub AssessFs(ByVal Fgear() As Single, ByVal Bbar() As Single)
            'does assessment at end of simulated year in runmodelvalue if ploton=true,
            'returns fwc(i,1)=updated relative catchability for gear i to be used next year
            'for relative effort setting
            'uses: relative fishing efforts this year (Fgear(i)),end-year biomass bb(j)
            'wftot(i)=total fishing importance weight for gear i (computed at start of simulation)
            'yearly catch catchyear(i,j).  Note Fwc(i,0) has ecopath base weighted fishing impacts
            Dim i As Integer, j As Integer, Fwt As Single, Fest(,) As Single, Best() As Single
            Dim Fpred As Single, Bp As Single

            Try

                ReDim Fest(m_epdata.NumFleet, m_epdata.NumLiving), Best(m_epdata.NumLiving)

                'first estimate fishing rates actually achieved by gear and species, Fest(i,j)
                Select Case m_data.AssessMethod

                    Case eAssessmentMethods.Exact 'biomasses and catch known exactly

                        For i = 1 To m_epdata.NumFleet
                            For j = 1 To m_epdata.NumLiving
                                If Bbar(j) > 0 Then Fest(i, j) = Me.m_Search.CatchYear(i, j) / Bbar(j) Else Fest(i, j) = 0
                            Next
                        Next

                    Case eAssessmentMethods.CatchEstmBio ' Fs from biomass estimates by pool
                        For j = 1 To m_epdata.NumLiving
                            Best(j) = CSng(Math.Exp(Normal2() * m_data.CVbiomEst(j)) * Me.m_esData.StartBiomass(j) * (Bbar(j) / Me.m_esData.StartBiomass(j)) ^ m_data.AssessPower)

                            If BestTime(j) > 0 Then  'have previous biomass estimate for this run
                                Bp = m_data.GstockPred(j) * BestTime(j) + m_data.RstockPred(j)
                                BestTime(j) = Bp + m_data.KalmanGain(j) * (Best(j) - Bp)
                            Else
                                BestTime(j) = Best(j)
                            End If

                            Best(j) = BestTime(j)

                            For i = 1 To m_epdata.NumFleet
                                If Best(j) > 0 Then Fest(i, j) = Me.m_Search.CatchYear(i, j) / Best(j) Else Fest(i, j) = 0
                            Next i

                        Next j

                    Case eAssessmentMethods.DirectExploitation ' Fs from direct exploitation method (eg tagging)

                        For i = 1 To m_epdata.NumFleet
                            For j = 1 To m_epdata.NumLiving
                                Fest(i, j) = (m_Search.CatchYear(i, j) / Bbar(j)) * CSng(Math.Exp(Normal2() * m_data.CVFest(j)))
                            Next
                        Next

                    Case Else
                        Debug.Assert(False, "Assessment Method index set incorrectly")
                        '  MsgBox("Assessment Method index set incorrectly")
                End Select

                'then update relative catchability estimates by gear
                For i = 1 To m_epdata.NumFleet
                    Fwt = 0
                    'If Fgear(i) = 0 Then Fgear(i) = 0.0000000001
                    For j = 1 To m_epdata.NumLiving
                        Fwt = Fwt + Fest(i, j) * m_data.Fweight(i, j)
                    Next
                    Fpred = m_data.Fwc(i, 1) * (1 + m_data.Qgrow(i) / 2)
                    If Fgear(i) > 0 And Fwt > 0 Then
                        m_data.Fwc(i, 1) = Fpred + m_data.KalGainQ(i) * (Fwt / (m_data.Wftot(i) * Fgear(i)) - Fpred)
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".AssessFs() Error: " & ex.Message, ex)
            End Try


        End Sub


        Private Function Normal2() As Single
            Dim R As Single
            'R = -6
            'For i = 1 To 12
            '    R = R + Rnd
            'Next
            R = 2 * Rnd() - 1
            Normal2 = CSng(Math.Log((1 + R) / (1 - R)) / 1.82)

        End Function

        Private Function Normal() As Single
            Dim V1 As Single, V2 As Single
            Do
                V1 = Rnd()
                V2 = Rnd()
            Loop Until V1 > 0
            Normal = CSng(Math.Sqrt(-2 * Math.Log(V1)) * Math.Cos(2 * 3.14159 * V2))
        End Function

#End Region

#End Region

#Region "Time step data summary"
        ''' <summary>
        ''' Ecosim Timestep delegate handler 
        ''' </summary>
        ''' <param name="iTime"></param>
        ''' <param name="data"></param>
        ''' <remarks></remarks>
        Private Sub onEcosimTimestep(ByVal iTime As Long, ByVal data As cEcoSimResults)
            'ToDo_jb get the Mean Min and Max values of all variables that will have stats from the MSE
            Try

                If Me.m_Search.SearchMode <> eSearchModes.MSE Then
                    Exit Sub
                End If

                For igrp As Integer = 1 To Me.m_esData.nGroups
                    Me.m_data.BioSum.AddValue(igrp, Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, CInt(iTime)))
                    Me.m_data.CatchGroupSum.AddValue(igrp, Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, CInt(iTime)))
                Next igrp

                For iflt As Integer = 1 To Me.m_esData.nGear
                    Me.m_data.CatchFleetSum.AddValue(iflt, Me.m_esData.ResultsSumCatchByGear(iflt, CInt(iTime)))
                Next iflt

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".onEcosimTimestep() Error: " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' Event handler for Plugin Economic data
        ''' </summary>
        ''' <param name="EconomicData"></param>
        ''' <remarks></remarks>
        Private Sub onEconomicData(ByVal EconomicData As EwEUtils.Core.IEconomicData) Handles EconomicData.onEconomicData

            Try

                'Is there plugin economic data
                If Me.UsePlugin Then
                    'Plugin economic data from the ValueChain pluging is sent out every timestep
                    'Store the data in cMSESummaryStats objects

                    Me.m_data.ProfitSum.AddValue(1, EconomicData.Profit)
                    Me.m_data.JobsSum.AddValue(1, EconomicData.NumberOfJobsTotal)
                    Me.m_data.CostSum.AddValue(1, EconomicData.Cost)

                End If

            Catch ex As Exception
                'make sure all exceptions are handled here and not back in the cEconomicDataSource object
                System.Console.WriteLine(Me.ToString & ".onEconomicData() Error: " & ex.Message)
                cLog.Write(ex)
            End Try

        End Sub


        ''' <summary>
        ''' Summarize the economic data gathered by Ecosim at the end of a trial
        ''' </summary>
        ''' <remarks>Economic data caculated by ecosim at the end of a run</remarks>
        Private Sub summarizeEcosimEconomicData()

            'ToDo_jb cMSE.summarizeEconomicData() figure out how to compute Economic data from the Ecosim data
            If Not Me.UsePlugin Then

                Dim sumValue As Single, sumEffort As Single, sumProfit As Single, sumJobs As Single, sumCost As Single
                For iflt As Integer = 0 To Me.m_esData.nGear

                    For it As Integer = 1 To Me.m_esData.nSumTimeSteps
                        sumValue += Me.m_esData.ResultsSumValueByGear(iflt, it)
                    Next
                    For it As Integer = 1 To Me.m_esData.nSumTimeSteps
                        sumEffort += Me.m_esData.ResultsEffort(iflt, it)
                    Next

                    sumCost += Me.m_Search.NetCost(iflt)

                    ' profit
                    '[sum of value] * [ecopath profit (percentage of catch value that is profit /per unit of effort)]
                    sumProfit = sumValue * (Me.m_epdata.cost(iflt, eCostIndex.Profit) / 100) * sumEffort

                    'TEMP just for something to work with until we have ECost up and running
                    '[value of catch] * [Jobs(fleet) from the search forms]
                    sumJobs = sumValue * Me.m_Search.Jobs(iflt) 'Jobs(Fleet) percentage of value that goes to Jobs default=1

                Next iflt

                Me.m_data.ProfitSum.AddValue(1, Me.m_Search.totval)
                Me.m_data.JobsSum.AddValue(1, sumJobs)
                Me.m_data.CostSum.AddValue(1, sumCost)

            End If

        End Sub


#End Region

    End Class

#End Region

End Namespace
