Option Strict On

Imports System.IO
Imports EwECore
Imports EwECore.Ecosim
Imports EwECore.ExternalData
Imports EwEUtils.Core
Imports EwEPlugin

Namespace MSE

#Region "Public definitions"

    Public Class cMSYProgressArgs
        Public Iteration As Integer
        Public FleetIndex As Integer
        Public CurrentEffort As Single
        Public Sub New(ByVal curIteration As Integer, ByVal iFleet As Integer, ByVal Effort As Single)
            Iteration = curIteration
            FleetIndex = iFleet
            CurrentEffort = Effort
        End Sub
    End Class


    Public Enum eCallBackTypes
        Started
        RunCompleted
        IterationCompleted
        IterationStarted
    End Enum

    Public Delegate Sub MSEProgressDelegate(ByVal CallBackType As eCallBackTypes)
    Public Delegate Sub MSYProgressDelegate(ByVal MSYProgress As cMSYProgressArgs)

#End Region

#Region "MSE Class"

    ''' <summary>
    ''' Management Strategy Evaluation
    ''' </summary>
    ''' <remarks>This was the Closed Loop Simulation in EwE5</remarks>
    Public Class cMSE

        'ToDo_jb 30-Dec-09 MSE note Number of years the MSE runs Ecosim for. At one point we had it running for an extra number of years(cSearchDataStructures.ExtraYearsForSearch) 
        'like the Fishing Policy search does.
        'I have removed this from cSearchDataStructures.InitSearch(). Now Ecosim is only run for the number of years it is set for in the interface. 
        'If we want to run it for some invisible number of years we need to sort out 
        ' ValueChain does it need to know about the extra years? Now it uses nEcosimYears which does not include the extra years.
        ' Histogram How does it bin the data from the extra years that is not displayed. If the data is not display the histogram can look wrong.
        ' Fishing effort figure out what happens to the effort for the extra years in the different modes the MSE runs in.

        'ToDo_jb 18-Jan-2010 cMSE output files need to have header and maybe more outputs

        'ToDo_jb 18-Jan-2010 cMSE database save reference values. This should get done in conjunction with the MSE Trunk merge and release

        Private Enum eResultsData
            GroupQuota
            FleetQuota
        End Enum


#Region "Private data"

        Private m_core As cCore
        Private m_data As cMSEDataStructures
        Private m_Ecosim As Ecosim.cEcoSimModel
        Private m_Search As cSearchDatastructures
        Private m_esData As cEcosimDatastructures

        Private m_epdata As cEcopathDataStructures
        Private m_quota As cQuotaDataStructures

        Private m_nTrials As Integer

        Private m_CallbackDelegate As MSEProgressDelegate

        Private BestTime() As Single
        Private EcoValueBase As Single, ManValueBase As Single
        Private TotValBase As Single, EmployBase As Single

        Private m_pluginManager As cPluginManager
        Private m_orgPredictEffort As Boolean
        Private m_orgUsePlugin As Boolean = False

        'fishing mortality at the current time step
        'calc in UpdateQuotas() using the estimated biomass
        Private FtargetT() As Single

        Private WithEvents m_EconomicData As cEconomicDataSource

        Private m_MSYCallBack As MSYProgressDelegate
        Private m_baseEffort(,) As Single 'base value relative effort FishRateGear()

        Private m_DataDir As String

        'Filenames prefixes for output file
        Private Const BIOMASS_DATA As String = "MSE_Biomass_"
        Private Const CATCH_DATA As String = "MSE_CatchByGroup_"
        Private Const EFFORT_DATA As String = "MSE_Effort_"
        Private Const FLEETCATCH_DATA As String = "MSE_CatchByFleet_"
        Private Const QUOTAGROUP_DATA As String = "MSE_QuotaByGroup_"

        ''' <summary>Dictionary of arrays that are use to store results that are gathered by the MSE.</summary>
        ''' <remarks>Use to store results that are not computed by Ecosim.</remarks>
        Private m_lstData As Dictionary(Of eResultsData, Single(,))

        ''' <summary>Current time index(cumulative month) computed by Ecosim</summary>
        ''' <remarks>set when Ecosim calls SetTime()</remarks>
        Private m_curT As Integer

        ''' <summary>Current year index computed by Ecosim</summary>
        ''' <remarks>set when Ecosim calls SetTime()</remarks>
        Private m_curYear As Integer

#End Region

#Region "Public Properties"

        Public ReadOnly Property Data() As cMSEDataStructures
            Get
                Return Me.m_data
            End Get
        End Property

#End Region

#Region "Modeling code"

#Region "Private Properties"

        Private ReadOnly Property UsePlugin() As Boolean
            Get
                If (Me.m_EconomicData IsNot Nothing) Then
                    Return (Me.m_Search.MSEUseEconomicPlugin = True) And _
                           (Me.m_EconomicData.EnableData(New cEcosimRunType) = True)
                End If
                Return False
            End Get
        End Property

#End Region

#Region "Initialization and Connection"

        Public Sub New(ByVal theCore As cCore)
            Me.m_core = theCore
        End Sub

        Public Sub Init(ByVal MSEData As cMSEDataStructures, ByVal QuotaData As cQuotaDataStructures, ByVal Ecosim As Ecosim.cEcoSimModel, ByVal SearchData As cSearchDatastructures, ByVal EcopathData As cEcopathDataStructures, ByVal PluginManager As cPluginManager)

            Me.m_data = MSEData
            Me.m_Ecosim = Ecosim
            Me.m_Search = SearchData
            Me.m_quota = QuotaData
            Me.m_esData = m_Ecosim.m_Data
            Me.m_epdata = EcopathData
            Me.m_pluginManager = PluginManager

            Me.m_EconomicData = cEconomicDataSource.getInstance()
            Me.m_data.InitForRun()

            'VC added a boolean to ex/include fleets from MSY runs
            ReDim Data.MSYEvaluateFleet(m_epdata.NumFleet)
            For i As Integer = 1 To m_epdata.NumFleet
                Data.MSYEvaluateFleet(i) = True 'that's the default value
            Next
            ReDim Data.MSYEvaluateGroup(m_epdata.NumGroups)
            For i As Integer = 1 To m_epdata.NumGroups
                Data.MSYEvaluateGroup(i) = True
            Next

        End Sub

        Public Sub Connect(ByRef MSECallBack As MSEProgressDelegate, ByRef MSYCallBack As MSYProgressDelegate)
            Me.m_CallbackDelegate = MSECallBack
            Me.m_MSYCallBack = MSYCallBack
        End Sub


        Friend Sub InitAssessment()
            Dim iGrp As Integer
            Dim ngear As Integer = Me.m_esData.nGear

            ReDim Me.FtargetT(Me.m_esData.nGroups)

            For iGrp = 1 To Me.m_esData.nGroups
                m_data.Bestimate(iGrp) = Me.m_esData.StartBiomass(iGrp) * CSng(Math.Exp(Me.m_data.CVbiomEst(iGrp) * Me.m_Ecosim.RandomNormal()))
                m_data.BestimateLast(iGrp) = m_data.Bestimate(iGrp)
            Next iGrp

            'QuotaTime() get set in the first timestep by UpdateQuota() before it is used in RegulateEffort()
            ' Dim totalQuota() As Single
            ' Dim iFlt As Integer

            'ReDim totalQuota(Me.m_esData.nGroups)
            'For iFlt = 1 To ngear
            '    For iGrp = 1 To Me.m_esData.nGroups
            '        If (m_epdata.Landing(iFlt, iGrp) + m_epdata.Discard(iFlt, iGrp)) > 0 Then
            '            totalQuota(iGrp) = totalQuota(iGrp) + Me.m_quota.Quota(iFlt, iGrp)
            '        End If
            '    Next
            'Next

            'For iFlt = 1 To ngear
            '    For iGrp = 1 To Me.m_esData.nGroups
            '        If (m_epdata.Landing(iFlt, iGrp) + m_epdata.Discard(iFlt, iGrp)) > 0 Then
            '            Me.m_quota.QuotaTime(iFlt, iGrp) = Me.m_quota.Quota(iFlt, iGrp)
            '        End If
            '    Next
            'Next

        End Sub

        Public Sub InitForRun()

            Try
                Dim iflt As Integer
                ReDim BestTime(m_epdata.NumGroups)

                Dim ds As cEconomicDataSource = cEconomicDataSource.getInstance()
                If (ds IsNot Nothing) Then
                    Me.m_orgUsePlugin = ds.EnableData(New cEcosimRunType)
                    ds.EnableData(New cEcosimRunType) = Me.UsePlugin
                End If

                Me.m_data.StopRun = False

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

                Me.DisableFTimeSeries()

                'initialize Ecosim
                m_Ecosim.Init(False)

                Me.InitOutputFiles()

                Me.InitResults()

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException(Me.ToString & ".InitForRun() Error:" & ex.Message, ex)
            End Try

        End Sub

        Public Sub FinalizeRun()

            'set the ecosim predict effort flag back to its original value
            Me.m_esData.PredictSimEffort = Me.m_orgPredictEffort

            Dim ds As cEconomicDataSource = cEconomicDataSource.getInstance()
            If (ds IsNot Nothing) Then
                ds.EnableData(New cEcosimRunType) = Me.m_orgUsePlugin
            End If

            If Me.m_data.SaveOutput Then
                If Me.m_lstData IsNot Nothing Then
                    Me.m_lstData.Clear()
                    Me.m_lstData = Nothing
                End If
            End If

        End Sub

        Private Function getOutputDirectory() As String

            Try

                Dim modelPath As String = DirectCast(Me.m_core.DataSource.Connection, Database.cEwEAccessDatabase).Name
                If File.Exists(modelPath) Then
                    Return Path.Combine(Path.GetDirectoryName(modelPath), "MSE\")
                Else
                    System.Console.WriteLine("MSE Failed to find database directory from the currently loaded model.")
                    Return (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSE\"))
                End If
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".getOutputDirectory() Exception: " & ex.Message)
            End Try

            Return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MSE\")

        End Function


        Private Sub InitOutputFiles()

            If Not Me.m_data.SaveOutput Then Exit Sub

            'get the directory to dump the data to
            Me.m_DataDir = Me.getOutputDirectory

            Try

                If Not Directory.Exists(Me.m_DataDir) Then
                    Directory.CreateDirectory(Me.m_DataDir)
                End If

                'clear out any existing data files
                For igrp As Integer = 1 To Me.m_data.NGroups
                    Try
                        File.Delete(Me.buildFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                        File.Delete(Me.buildFilename(CATCH_DATA, Me.m_epdata.GroupName(igrp)))
                        File.Delete(Me.buildFilename(QUOTAGROUP_DATA, Me.m_epdata.GroupName(igrp)))
                    Catch ex As Exception
                        System.Console.WriteLine(ex.Message)
                    End Try
                Next igrp

                For iflt As Integer = 1 To Me.m_data.nFleets
                    Try
                        File.Delete(Me.buildFilename(FLEETCATCH_DATA, Me.m_epdata.FleetName(iflt)))
                        File.Delete(Me.buildFilename(EFFORT_DATA, Me.m_epdata.FleetName(iflt)))
                    Catch ex As Exception
                        System.Console.WriteLine()
                    End Try
                Next iflt

                'Write output file headers

                For igrp As Integer = 1 To Me.m_data.NGroups
                    Me.WriteOutputHeader("Biomass", Me.m_epdata.GroupName(igrp), BIOMASS_DATA)
                    If Me.m_epdata.fCatch(igrp) > 0 Then
                        Me.WriteOutputHeader("Catch by Group", Me.m_epdata.GroupName(igrp), CATCH_DATA)
                        Me.WriteOutputHeader("Quota by Group", Me.m_epdata.GroupName(igrp), QUOTAGROUP_DATA)
                    End If
                Next

                For iflt As Integer = 1 To Me.m_data.nFleets
                    Me.WriteOutputHeader("Catch by Fleet", Me.m_epdata.FleetName(iflt), FLEETCATCH_DATA)
                    Me.WriteOutputHeader("Effort by Fleet", Me.m_epdata.FleetName(iflt), EFFORT_DATA)
                Next iflt

            Catch ex As Exception

            End Try

        End Sub

        Private Sub InitResults()
            Try
                If Not Me.m_data.SaveOutput Then Return

                If Me.m_lstData IsNot Nothing Then
                    Me.m_lstData.Clear()
                    Me.m_lstData = Nothing
                End If

                Me.m_lstData = New Dictionary(Of eResultsData, Single(,))
                Dim d(,) As Single
                ReDim d(m_epdata.NumGroups, Me.m_esData.NTimes)
                Me.m_lstData.Add(eResultsData.GroupQuota, d)

                'redim will create a new array
                ReDim d(m_epdata.NumFleet, Me.m_esData.NTimes)
                Me.m_lstData.Add(eResultsData.FleetQuota, d)

            Catch ex As Exception
                System.Console.WriteLine(ex.Message)
            End Try

        End Sub


        Private Sub WriteOutputHeader(ByVal DataDescription As String, ByVal GroupFleet As String, ByVal DataFileName As String)

            Try

                Dim header As System.Text.StringBuilder
                Dim strm As StreamWriter

                header = New System.Text.StringBuilder
                Dim d As DateTime = Date.Now

                header.Append("MSE " & DataDescription & vbCrLf)
                header.Append("Date, '" & d.ToLongDateString & " " & d.ToLongTimeString & vbCrLf)
                header.Append("Group, '" & GroupFleet & "'" & vbCrLf)
                header.Append("Rows = MSE Run, Columns = Time" & vbCrLf)

                For it As Integer = 1 To Me.m_core.nEcosimTimeSteps
                    header.Append(it.ToString & ", ")
                Next
                header.Remove(header.Length - 2, 2)

                strm = New StreamWriter(buildFilename(DataFileName, GroupFleet), True)
                strm.WriteLine(header)
                strm.Close()

            Catch ex As Exception

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

            ReDim Me.m_baseEffort(Me.m_esData.nGear, Me.m_esData.NTimes)
            For iflt As Integer = 1 To m_core.nFleets
                For it As Integer = 1 To Me.m_esData.NTimes
                    m_baseEffort(iflt, it) = Me.m_esData.FishRateGear(iflt, it)
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


        ''' <summary>
        ''' Disable/Unload and F (fishing mortality) timeseries 
        ''' </summary>
        ''' <remarks>Fishing mortality timeseries prevent the current effort from being used to set fishing mortality FishTime(group) on a group.</remarks>
        Private Sub DisableFTimeSeries()
            Dim bDataUnloaded As Boolean

            Try

                If Me.m_core.ActiveTimeSeriesDatasetIndex > -1 Then
                    Dim DS As cTimeSeriesDataset
                    DS = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
                    For Each ts As cTimeSeries In DS
                        If ts.TimeSeriesType = eTimeSeriesType.FishingMortality Then
                            'there is an effort in time series, so turn it off. 
                            ts.Enabled = False
                            bDataUnloaded = True
                        End If
                    Next

                    If bDataUnloaded Then
                        m_core.UpdateTimeSeries()
                    End If

                End If 'If Me.m_core.ActiveTimeSeriesDatasetIndex > -1 Then

            Catch ex As Exception
                System.Console.WriteLine(ex.Message)
                cLog.Write(ex)
            End Try

        End Sub

#End Region

#Region "Running and computational code"

        Public Sub Run()
            Dim itr As Integer

            Try

                CallBack(eCallBackTypes.Started)

                'keep the original value of PredictEffort so we can set it back at the end of the run
                m_orgPredictEffort = Me.m_esData.PredictSimEffort

                'turn off regulatory models for initialization
                Me.m_esData.PredictSimEffort = False

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

                'if we are predicting effort then make sure it is turned on in Ecosim
                Me.m_esData.PredictSimEffort = False
                If Me.m_data.EffortMode = eMSEEffortMode.PredictUseQuota Then Me.m_esData.PredictSimEffort = True

                For itr = 1 To m_data.NTrials

                    m_data.CurrentIteration = itr
                    Me.AddIteration()
                    Me.CallBack(eCallBackTypes.IterationStarted)

                    'Set MSE data back to initial values for a new run
                    m_data.InitForTrial()

                    'run ecosim
                    Me.m_Ecosim.Run()

                    Me.summarizeEcosimEconomicData()

                    Me.SaveIteration()
                    'post the search data to plugins
                    Me.PostPluginData()

                    Me.SumValues()
                    Me.CallBack(eCallBackTypes.IterationCompleted)

                    SetEffortToBaseValue()

                    If Me.m_data.StopRun Then Exit For

                Next itr

                Me.ComputeStats()

                'Me.dumpStats()

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, "MSE Exception: " & ex.Message)
                Me.m_core.Messages.SendMessage(New cMessage("Error while calculating MSE. " & ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.MSE, eMessageImportance.Critical))
            End Try

            CallBack(eCallBackTypes.RunCompleted)

            'turn off the search
            Me.m_Search.SearchMode = eSearchModes.NotInSearch

        End Sub

        Private Sub ComputeStats()

            Me.m_data.BioStats.ComputeStats()
            Me.m_data.CatchFleetStats.ComputeStats()
            Me.m_data.CatchGroupStats.ComputeStats()
            Me.m_data.EffortStats.ComputeStats()

        End Sub

        ''' <summary>
        ''' Add an iteration to the stats data
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub AddIteration()

            Me.m_data.BioStats.AddIteration()
            Me.m_data.CatchFleetStats.AddIteration()
            Me.m_data.CatchGroupStats.AddIteration()
            Me.m_data.EffortStats.AddIteration()

            Me.m_data.ProfitSum.AddIteration()
            Me.m_data.JobsSum.AddIteration()
            Me.m_data.CostSum.AddIteration()

        End Sub

        Private Sub SetEffortToBaseValue(Optional ByVal DoIt As Boolean = False)

            If Me.m_data.EffortMode = eMSEEffortMode.TrackUseQuota Or DoIt Then
                'if we are tracking the Ecosim effort and regulating it via the quota 
                'then we need to set effort to something for each iteration
                For iflt As Integer = 1 To m_core.nFleets
                    For it As Integer = 1 To Me.m_esData.NTimes
                        Me.m_esData.FishRateGear(iflt, it) = Me.m_baseEffort(iflt, it)
                    Next
                Next
            End If

        End Sub

        Private Sub dumpStats()

            For i As Integer = 1 To Me.m_esData.nGroups
                Dim histo() As Single = Me.m_data.BioStats.Histogram(i)
                'histogram stuff for debugging
                System.Console.WriteLine()
                For ihist As Integer = 1 To Me.m_data.BioStats.HistoNBins(i)
                    System.Console.Write(histo(ihist).ToString & ", ")
                Next

            Next

            System.Console.WriteLine("----P------")
            For i As Integer = 1 To Me.m_esData.nGroups
                Dim Pless As Single = Me.m_data.BioStats.PercentageBelow(i, Me.m_data.BioBounds(i).Lower)
                Dim Pgreater As Single = Me.m_data.BioStats.PercentageAbove(i, Me.m_data.BioBounds(i).Upper)
                ' Debug.Assert(Pless + Pgreater <= 100, "MSE Probability calculation!!!!")
                System.Console.WriteLine("Group = " & Me.m_core.m_EcoPathData.GroupName(i) & ", less = " & Pless.ToString & ", greater = " & Pgreater.ToString)
            Next


            System.Console.WriteLine()

            System.Console.WriteLine("Biomass ranges")
            System.Console.Write(Me.m_data.BioStats.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Catch by group ranges")
            System.Console.Write(Me.m_data.CatchGroupStats.ToString)
            System.Console.WriteLine()

            System.Console.WriteLine("Catch by fleet ranges")
            System.Console.Write(Me.m_data.CatchFleetStats.ToString)
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

        Private Sub SaveIteration()

            If Not Me.m_data.SaveOutput Then Exit Sub

            Dim buff As System.Text.StringBuilder
            Dim strm As StreamWriter

            Try
                'We could set this up so each type had a seperate flag for dumping

                'Biomass
                For igrp As Integer = 1 To Me.m_data.NGroups
                    Try
                        buff = New System.Text.StringBuilder
                        For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                            buff.Append(m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, its).ToString & ", ")
                        Next

                        strm = New StreamWriter(buildFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)), True)
                        strm.WriteLine(buff)
                        strm.Close()
                        buff = Nothing
                    Catch ex As Exception
                        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)) & " Exception: " & ex.Message)
                    End Try
                Next

                'Catch by group
                For igrp As Integer = 1 To Me.m_data.NGroups
                    Try
                        If Me.m_epdata.fCatch(igrp) > 0 Then
                            buff = New System.Text.StringBuilder
                            For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                                buff.Append(m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, its).ToString & ", ")
                            Next

                            strm = New StreamWriter(buildFilename(CATCH_DATA, Me.m_epdata.GroupName(igrp)), True)
                            strm.WriteLine(buff)
                            strm.Close()
                            buff = Nothing
                        End If
                    Catch ex As Exception
                        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)) & " Exception: " & ex.Message)
                    End Try
                Next


                'Quota by group
                For igrp As Integer = 1 To Me.m_data.NGroups
                    Try
                        Dim data(,) As Single = Me.m_lstData.Item(eResultsData.GroupQuota)
                        If Me.m_epdata.fCatch(igrp) > 0 Then
                            buff = New System.Text.StringBuilder
                            For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                                buff.Append(data(igrp, its).ToString & ", ")
                            Next

                            strm = New StreamWriter(buildFilename(QUOTAGROUP_DATA, Me.m_epdata.GroupName(igrp)), True)
                            strm.WriteLine(buff)
                            strm.Close()
                            buff = Nothing
                        End If
                    Catch ex As Exception
                        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)) & " Exception: " & ex.Message)
                    End Try
                Next




                'Catch by fleet
                For iflt As Integer = 1 To Me.m_data.nFleets
                    Try
                        buff = New System.Text.StringBuilder
                        For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                            buff.Append(m_esData.ResultsSumCatchByGear(iflt, its).ToString & ", ")
                        Next

                        strm = New StreamWriter(buildFilename(FLEETCATCH_DATA, Me.m_epdata.FleetName(iflt)), True)
                        strm.WriteLine(buff)
                        strm.Close()
                        buff = Nothing

                    Catch ex As Exception
                        'Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(CATCH_DATA, Me.m_epdata.FleetName(iflt)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename(FLEETCATCH_DATA, Me.m_epdata.FleetName(iflt)) & " Exception: " & ex.Message)
                    End Try
                Next

                'Effort by fleet
                For iflt As Integer = 1 To Me.m_data.nFleets
                    Try
                        buff = New System.Text.StringBuilder
                        For its As Integer = 1 To Me.m_core.GetCoreCounter(eCoreCounterTypes.nEcosimTimeSteps)
                            buff.Append(m_esData.ResultsEffort(iflt, its).ToString & ", ")
                        Next

                        strm = New StreamWriter(buildFilename(EFFORT_DATA, Me.m_epdata.FleetName(iflt)), True)
                        strm.WriteLine(buff)
                        strm.Close()
                        buff = Nothing

                    Catch ex As Exception
                        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(EFFORT_DATA, Me.m_epdata.GroupName(iflt)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename(EFFORT_DATA, Me.m_epdata.FleetName(iflt)) & " Exception: " & ex.Message)
                    End Try
                Next

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".SaveIteration() Exception: " & ex.Message)
            End Try

        End Sub

        Private Function buildFilename(ByVal DataType As String, ByVal DataName As String) As String
            Return Me.m_DataDir & DataType & DataName & ".csv"
        End Function



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

                Select Case CallBackType
                    Case eCallBackTypes.RunCompleted
                        Me.FinalizeRun()
                End Select

                System.Console.WriteLine("MSE Callback = " & CallBackType.ToString)
                Me.m_CallbackDelegate(CallBackType)

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

                For i As Integer = 1 To m_epdata.NumGroups

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
        ''' <param name="QYear">Catchability growth per year</param>
        ''' <param name="iYear"></param>
        ''' <remarks>Called from Ecosim during a management strategy evaluation</remarks>
        Friend Sub VaryEffortCatchability(ByRef Fgear() As Single, ByRef QYear() As Single, ByVal iYear As Integer)

            Try
                ' increase catchability with the annual growth factor, irrespective of regulation or closed loop type
                For i As Integer = 1 To Me.m_epdata.NumFleet
                    If iYear > 1 Then
                        QYear(i) = QYear(i) * (1 + Me.m_data.QGrowUsed(i) * Rnd())
                    End If
                Next i

                If Not Me.m_data.EffortMode = eMSEEffortMode.Tracking Then
                    'Only vary effort here if we are in the Tracking mode(effort is set by the current Ecosim Effort). 
                    Exit Sub
                End If

                For i As Integer = 1 To Me.m_epdata.NumFleet
                    If iYear > 1 Then
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

                If Not Me.m_data.EffortMode = eMSEEffortMode.Tracking Then
                    'Predicting effort this means we are running the regulatory code to regulate effort
                    'so don't vary effort or catchability here
                    'When?
                    '= whenever using regulatory options

                    Exit Sub
                    'if not we're tracking effort, so now that's what we're doing, using user input effort,
                    'it will use the effort from ecosim. 
                End If

                Debug.Assert(Me.m_data.EffortMode = eMSEEffortMode.Tracking, "MSE EffortMode incorrectly set!")

                ReDim Fest(m_epdata.NumFleet, m_epdata.NumLiving), Best(m_epdata.NumLiving)

                'first estimate fishing rates actually achieved by gear and species, Fest(ifleet,igroup) = catch(fleet,group)/biomass(group)
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


        Friend Sub RegulateEffort(ByVal Biomass() As Single, ByVal QMult() As Single, ByVal t As Integer)
            Dim i As Integer, ig As Integer, Elim As Single, Emax As Single
            Dim ci As Single

            'does regulatory reduction in FishRateGear(ig,t) for each ig (gear)
            For ig = 1 To m_esData.nGear

                ' If m_esData.FishRateGear(ig, t) > Me.m_quota.MaxEffort(ig) Then m_esData.FishRateGear(ig, t) = Me.m_quota.MaxEffort(ig)

                Select Case Me.m_quota.QuotaType(ig)

                    Case eQuotaTypes.Effort
                        'NOT IMPLEMENTED at this time
                        Debug.Assert(Me.m_quota.QuotaType(ig) = eQuotaTypes.Effort, "Effort regulations have not been implemented at this time!")

                        'QMult will need to be computed using Bestimate()
                        'F(igrp) = fTarget(igrp) / qmult(igrp)

                    Case eQuotaTypes.Weakest 'limit effort to weakest stock

                        For i = 1 To m_data.NGroups
                            If (m_epdata.Landing(ig, i) + m_epdata.Discard(ig, i)) > 0 Then
                                'Calculate the effort limitation, has quote been exceeded?
                                Elim = CSng(Me.m_quota.QuotaTime(ig, i) / (1.0E-20 + QMult(i) * m_esData.FishMGear(ig, i) * Biomass(i)))
                                If m_esData.FishRateGear(ig, t) > Elim Then
                                    m_esData.FishRateGear(ig, t) = Elim
                                End If
                            End If
                        Next i

                    Case eQuotaTypes.Strongest, eQuotaTypes.Selective 'limit effort to strongest stock but discard overages on weaker stocks

                        Emax = 0
                        For i = 1 To m_data.NGroups
                            If (m_epdata.Landing(ig, i)) > 0 Then
                                'Calculate the effort limitation, has quote for strongest stock (calling for biggest effort) been exceeded?
                                Elim = CSng(Me.m_quota.QuotaTime(ig, i) / (1.0E-20 + QMult(i) * m_esData.FishMGear(ig, i) * Biomass(i)))
                                If Elim > Emax Then Emax = Elim

                            End If
                        Next i

                        If Emax < m_esData.FishRateGear(ig, t) Then m_esData.FishRateGear(ig, t) = Emax
                        For i = 1 To m_data.NGroups
                            If (m_epdata.Landing(ig, i)) > 0 Then
                                ci = m_esData.FishRateGear(ig, t) * QMult(i) * m_esData.FishMGear(ig, i) * Biomass(i)

                                If ci > Me.m_quota.QuotaTime(ig, i) Then
                                    'fishing mortality exceeds quota
                                    Me.m_quota.PropLandedTime(ig, i) = CSng(Me.m_quota.QuotaTime(ig, i) / (ci + 1.0E-20))
                                    If Me.m_quota.QuotaType(ig) = eQuotaTypes.Strongest Then
                                        'QuotaType = Strongest 
                                        'excess catch discarded and included in the fishing mortailtiy
                                        Me.m_quota.Propdiscardtime(ig, i) = (1 - Me.m_quota.PropLandedTime(ig, i)) * m_epdata.PropDiscardMort(ig, i)
                                    Else
                                        'QuotaType = Selective 
                                        'excess catch is NOT included in fishing mortaility all discards survive
                                        Me.m_quota.Propdiscardtime(ig, i) = 0
                                    End If

                                Else
                                    'ci < QuotaTime
                                    Me.m_quota.PropLandedTime(ig, i) = m_epdata.PropLanded(ig, i)
                                    Me.m_quota.Propdiscardtime(ig, i) = m_epdata.PropDiscard(ig, i)
                                End If

                            End If
                        Next i

                End Select
            Next ig

        End Sub

        ''' <summary>
        ''' Populates Bestimate() for regulated fisheries
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub DoAssessment(ByVal Biomass() As Single)

            Dim Bobs() As Single
            ReDim Bobs(Me.m_epdata.NumGroups)

            For i As Integer = 1 To Me.m_epdata.NumGroups
                Me.m_data.BestimateLast(i) = Me.m_data.Bestimate(i)
                'the true biomass is the actual Ecosim biomass = Biomass()
                'Bobs is the observed biomass which is the true biomass with a random factor added
                Bobs(i) = Biomass(i) * CSng(Math.Exp(Me.m_data.CVbiomEst(i) * Me.m_Ecosim.RandomNormal() - 0.5 * Me.m_data.CVbiomEst(i) ^ 2))
                'and then we estimate a biomass from assessments, so Bestimate is what will be used for e.g., the fixed escapement policy.
                'VC091107 fixed problem in eq below
                Me.m_data.Bestimate(i) = Me.m_data.KalmanGain(i) * Bobs(i) + (1 - Me.m_data.KalmanGain(i)) * (m_data.GstockPred(i) * Me.m_data.BestimateLast(i) + m_data.RstockPred(i))

            Next i

        End Sub


        ''' <summary>
        ''' Update fishing quotas for regulated fisheries
        ''' </summary>
        ''' <remarks></remarks>
        Friend Sub UpdateQuotas(ByVal Biomass() As Single)
            Dim iflt As Integer, igrp As Integer
            Dim tQuota() As Single

            ReDim tQuota(Me.m_epdata.NumGroups)
            ReDim FtargetT(Me.m_epdata.NumGroups)

            For igrp = 1 To Me.m_epdata.NumGroups

                If Me.m_quota.FixedEscapement(igrp) > 0 Then

                    tQuota(igrp) = m_data.Bestimate(igrp) - m_quota.FixedEscapement(igrp)

                    'VC091104 There will also be uncertainty on how well this quota is implemented so add this:
                    'but assume uncertaint is smaller?????? not done here
                    tQuota(igrp) = tQuota(igrp) * CSng(Math.Exp(Me.m_data.CVbiomEst(igrp) * Me.m_Ecosim.RandomNormal() - 0.5 * Me.m_data.CVbiomEst(igrp) ^ 2))

                    If tQuota(igrp) < 0 Then tQuota(igrp) = 0

                Else    'not using fixed escapement, so calculate 

                    If m_quota.Bbase(igrp) > 0 Then

                        'Debug.Assert(Me.m_quota.Bbase(igrp) > Me.m_quota.Blim(igrp), "MSE UpdateQuotas() Bbase must be greater than Blim.")

                        'note here that Bbase has to be set larger than Blim
                        'VC to JB: I think the Biomass below should be Bestimate instead; talked to Carl and he agrees. will be a double wham, which is OK.
                        FtargetT(igrp) = Me.m_quota.Fopt(igrp) * (Me.m_data.Bestimate(igrp) - Me.m_quota.Blim(igrp)) / (Me.m_quota.Bbase(igrp) - Me.m_quota.Blim(igrp))
                        If FtargetT(igrp) < 0 Then FtargetT(igrp) = 0
                        If FtargetT(igrp) > Me.m_quota.Fopt(igrp) Then FtargetT(igrp) = Me.m_quota.Fopt(igrp)
                        tQuota(igrp) = FtargetT(igrp) * Me.m_data.Bestimate(igrp)

                        'VC091104 There will also be uncertainty on how well this quota is implemented so add this:
                        'but assume uncertaint is smaller?????? not done here
                        tQuota(igrp) = tQuota(igrp) * CSng(Math.Exp(Me.m_data.CVbiomEst(igrp) * Me.m_Ecosim.RandomNormal() - 0.5 * Me.m_data.CVbiomEst(igrp) ^ 2))
                    End If

                End If
            Next igrp

            For iflt = 1 To Me.m_esData.nGear
                For igrp = 1 To Me.m_epdata.NumGroups
                    Me.m_quota.QuotaTime(iflt, igrp) = tQuota(igrp) * Me.m_quota.Quotashare(iflt, igrp)
                Next
            Next

            Me.StoreQuotas(tQuota)

        End Sub


        ''' <summary>
        ''' Save the quota values to memory
        ''' </summary>
        ''' <param name="QuotaT">Quota array by group</param>
        Private Sub StoreQuotas(ByVal QuotaT() As Single)
            Dim igrp As Integer

            If Not Me.m_data.SaveOutput Then Exit Sub

            Try

                Dim d(,) As Single = Me.m_lstData.Item(eResultsData.GroupQuota)
                For igrp = 1 To Me.m_epdata.NumGroups
                    For it As Integer = 1 To 12
                        d(igrp, it + (Me.m_curYear - 1) * 12) = QuotaT(igrp)
                    Next
                Next

                Dim sumb As Single
                d = Me.m_lstData.Item(eResultsData.FleetQuota)
                For iflt As Integer = 1 To Me.m_epdata.NumFleet
                    sumb = 0
                    For igrp = 1 To Me.m_epdata.NumGroups
                        sumb += QuotaT(igrp) * Me.m_quota.Quotashare(iflt, igrp)
                    Next
                    For it As Integer = 1 To 12
                        d(iflt, it + (Me.m_curYear - 1) * 12) = sumb
                    Next

                Next

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".saveQuotas() Exception" & ex.Message)
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

        ''' <summary>
        ''' Box-Muller normally distributed random number with a standard deviation of one
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function Normal() As Single
            Dim V1 As Single, V2 As Single
            Do
                V1 = Rnd()
                V2 = Rnd()
            Loop Until V1 > 0
            Return CSng(Math.Sqrt(-2 * Math.Log(V1)) * Math.Cos(2 * 3.14159 * V2))
        End Function

#End Region

#Region "MSY"


        Public Sub RunMSYSearch()
            'WE'll run Ecosim for an additional 25 years to avoid the effort not being sustainable

            Me.m_data.StopRun = False

            Dim NumberOfYears As Integer = Me.m_esData.NumYears
            Dim extraYears As Integer = 25

            'Setup Ecosim 
            'timestep handler that ecosim will call where we can grab data during the run
            'see the Private Sub onMSYEcosimTimestep(...)
            Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onMSYEcosimTimestep

            Me.m_esData.bTimestepOutput = True

            Dim MSYeffort(Me.m_esData.nGear) As Single
            Dim MSYF(Me.m_epdata.NumGroups) As Single
            Dim bMSY(Me.m_esData.nGear, Me.m_epdata.NumGroups) As Double
            Dim fMSY(Me.m_esData.nGear, Me.m_epdata.NumGroups) As Double

            'Dim GroupEffort() As Single

            Dim iDataset As Integer = Me.m_core.ActiveTimeSeriesDatasetIndex
            Dim DS As cTimeSeriesDataset

            If iDataset > -1 Then DS = Me.m_core.TimeSeriesDataset(iDataset)

            'this is required to set the base effort values :
            m_core.EcoSimModelParameters.NumberYears = NumberOfYears + extraYears
            SetBaseValues()

            If Me.m_core.PluginManager IsNot Nothing Then
                Me.m_pluginManager.MSYRunStarted(Me.m_data, Me.m_quota, Me.m_esData)
            End If

            'next is a vc temp fix for debugging
            'Data.MSYStartTimeIndex = 649
            Dim FinestEffortStep As Double = 0.01


            Try

                For iFlt As Integer = 1 To Me.m_esData.nGear
                    If Data.MSYEvaluateFleet(iFlt) Then
                        MSYeffort(iFlt) = 0
                        'ReDim GroupEffort(m_data.NGroups)
                        'For iGroup As Integer = 1 To Me.m_data.NGroups
                        'If Data.MSYEvaluateGroup(iGroup) Then
                        'If m_epdata.Landing(iFlt, iGroup) > 0 Then 'only if this fleet catches this group
                        Dim Done As Boolean = False
                        Dim CurValue As Double = 0
                        Dim lastValue As Double = 0
                        Dim maxValue As Double = 0

                        Dim lastEffort As Double = 0
                        Dim TooBigEffort As Double = -99
                        Dim TooLowEffort As Double = -0.5
                        Dim tryEffort As Double = 0.5

                        Dim TSdisabled As cTimeSeries = Nothing
                        Dim NumberOfSteps As Integer = 0
                        If iDataset > -1 Then
                            For Each ts As cTimeSeries In DS
                                'jb changed to FishingMortality it's forced FishingMortality that prevents the effort from being set...I think...
                                'If ts.TimeSeriesType = eTimeSeriesType.FishingEffort Then
                                If ts.TimeSeriesType = eTimeSeriesType.FishingMortality Then
                                    ' If DirectCast(ts, cFleetTimeSeries).FleetIndex = iflt And ts.Enabled = True Then
                                    'there is an effort in time series, so turn it off. 
                                    ts.Enabled = False
                                    TSdisabled = ts
                                    m_core.UpdateTimeSeries()
                                    'DS.Update()
                                    'End If
                                End If
                            Next

                        End If

                        'UpdateTimeSeries() will reset the NumberYears to the number of years in the timeseries
                        'make sure this did not make NumberYears smaller
                        If m_core.EcoSimModelParameters.NumberYears < NumberOfYears + extraYears Then
                            m_core.EcoSimModelParameters.NumberYears = m_core.EcoSimModelParameters.NumberYears + extraYears
                        End If

                        'when projecting the time series, the forcing functions shuld be set to the average over the ecosim run, not to 1

                        System.Console.WriteLine()
                        Dim EffortFactor As Integer = 2
                        Dim CheckTangent As Boolean = False
                        Dim LastLowerEffort As Double = 0
                        Do While Done = False

                            NumberOfSteps += 1

                            Me.SetFishingEffort(iFlt, CSng(tryEffort))

                            'let ecosim init to the new values
                            Me.m_Ecosim.Init(True)

                            'run ecosim with the current effort
                            Me.m_Ecosim.Run()

                            'evaluate the ecosim output for this fleet/effort combination
                            CurValue = Me.EvaluateMSY(iFlt)

                            'if a fishery catches a group with low catch but high biomass, it may cause the effort to skyrocket
                            'to avoid this: set a limit on the F values for the exploited groups:
                            'if that happens set the value to a low value, so that it may try a lower effort
                            ' If CheckIfFishingMortalitiesTooHigh(iflt) Then CurValue = CurValue / 2


                            If CurValue = 0 Then Done = True 'no effort or no value
                            If CurValue < 0 Then Stop

                            If CheckTangent Then   'only get in here if we are checking the neighbourhood of the max value
                                If CurValue < maxValue Then
                                    'go downward
                                    TooLowEffort = LastLowerEffort
                                    TooBigEffort = lastEffort
                                Else
                                    'move upward
                                    LastLowerEffort = TooLowEffort
                                    ' TooLowEffort = lastEffort
                                    ' maxValue = CurValue
                                End If
                                CheckTangent = False
                            ElseIf CurValue > maxValue Then   'this is the normal entrypont when moving up
                                LastLowerEffort = TooLowEffort
                                TooLowEffort = lastEffort
                                If maxValue > 0 And TooBigEffort < 0 Then  'move faster if just starting out, and long way to go
                                    If CurValue / maxValue > 0.97 * EffortFactor Then EffortFactor = 4 Else EffortFactor = 2
                                End If
                                maxValue = CurValue
                                MSYeffort(iFlt) = CSng(tryEffort)
                            Else
                                If TooBigEffort < 0 Then
                                    TooBigEffort = tryEffort
                                    CheckTangent = True
                                Else
                                    'we are now somewhere below the msy effort, but at what side?
                                    If tryEffort > MSYeffort(iFlt) Then  'on the right side
                                        'reduce the toobigeffor to the current
                                        TooBigEffort = tryEffort
                                    Else   'below MSY
                                        TooLowEffort = tryEffort
                                    End If
                                End If
                            End If

                            If CheckTangent Then
                                tryEffort = 1.001 * TooLowEffort
                                'LastLowerEffort = TooLowEffort / 2
                            Else
                                If TooBigEffort < 0 Then 'NOT YET FOUND THE TOP, SO DOUBLE UP
                                    tryEffort = tryEffort * EffortFactor
                                Else  'have previously found a bigger effort that gave lower value, so now we have bounds
                                    tryEffort = (TooBigEffort - TooLowEffort) / 2 + TooLowEffort
                                End If
                            End If

                            lastValue = CurValue

                            If tryEffort > 0 And CheckTangent = False Then
                                If Math.Abs(1 - lastEffort / tryEffort) < FinestEffortStep Then Done = True
                            End If
                            lastEffort = tryEffort

                            System.Console.WriteLine(NumberOfSteps.ToString & ", Fleet = " & iFlt.ToString & _
                                                     ", MSY effort = " & MSYeffort(iFlt).ToString & _
                                                     ", Cur effort = " & tryEffort.ToString & _
                                                     ", toolow = " & TooLowEffort.ToString & _
                                                     ", toobig = " & TooBigEffort.ToString & _
                                                     ", maxvalue = " & maxValue.ToString & _
                                                     ", curvalue = " & CurValue.ToString)

                            'tell the interface an iteration has been completed
                            Me.fireMSYProgress(New cMSYProgressArgs(NumberOfSteps, iFlt, CSng(MSYeffort(iFlt))))

                            If Me.m_data.StopRun Then Exit Do
                        Loop

                        If TSdisabled IsNot Nothing Then
                            TSdisabled.Enabled = True
                            DS.Update()
                        End If


                        '==================this part not needed for teeb ---------------------------
                        'We now know the MSY effort, so can estimate, oeh, something
                        Me.SetFishingEffort(iFlt, MSYeffort(iFlt))
                        'let ecosim init to the new values
                        Me.m_Ecosim.Init(True)
                        'run ecosim with the current effort
                        Me.m_Ecosim.Run()

                        'now store the average biomasses from this run as the "MSY-biomass" for this fleet run
                        Dim SumBio As Single
                        Dim SumCatch As Single
                        For igrp As Integer = 1 To Me.m_esData.nGroups
                            SumBio = 0
                            SumCatch = 0
                            If Me.m_epdata.Landing(iFlt, igrp) > 0 Then
                                For it As Integer = 1 To Me.m_esData.NTimes
                                    'get data storted by ecosim over time  
                                    SumBio += Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, it)
                                    SumCatch += Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, it)
                                Next
                            End If
                            bMSY(iFlt, igrp) = SumBio / m_esData.NTimes
                            If SumBio > 0 Then fMSY(iFlt, igrp) = SumCatch / SumBio
                        Next igrp
                        '==================this part not needed for teeb ---------------------------

                        If Me.m_data.StopRun Then Exit For

                        'Finally reset the effort to the original effort (for all fleets)
                        SetEffortToBaseValue(True)
                        'End If
                        'End If
                        'Next iGroup

                        'We now have the effort by group, and can look for the median value:
                        'Evaluate which effort to use, there will be one for each fleet/group, but some of them are 0
                        'Dim NonZeroValues As Integer = 0
                        'For iGrp As Integer = 1 To m_data.NGroups
                        '    If GroupEffort(iGrp) > 0 Then NonZeroValues += 1
                        'Next
                        'Dim GroupEffortText As String = ""
                        'If NonZeroValues > 0 Then
                        '    Dim EffortValues(NonZeroValues) As Single
                        '    Dim iC As Integer = 0

                        '    For iGrp As Integer = 1 To m_data.NGroups
                        '        If GroupEffort(iGrp) > 0 Then
                        '            iC += 1
                        '            EffortValues(iC) = GroupEffort(iGrp)
                        '            GroupEffortText += EffortValues(iC).ToString
                        '        End If
                        '    Next
                        '    'we now have a numeric array with all the non-zero values, so find the median value:
                        '    Array.Sort(EffortValues)

                        '    Dim iLength As Integer = EffortValues.Length
                        '    If iLength Mod 2 = 1 Then
                        '        'uneven so pick this one
                        '        Dim iNo As Integer = iLength \ 2 + 1
                        '        MSYeffort(iFlt) = EffortValues(iNo)
                        '    Else
                        '        'average of the one below and above
                        '        Dim iNo As Integer = iLength \ 2
                        '        MSYeffort(iFlt) = (EffortValues(iNo) + EffortValues(iNo + 1)) / 2
                        '    End If
                        '    'MSYeffort(iFlt) = CSng(Median(EffortValues))

                        'End If

                        'System.Console.WriteLine(GroupEffortText)
                        'Me.fireMSYProgress(New cMSYProgressArgs(NumberOfSteps, iFlt, MSYeffort(iFlt)))
                        'Me.fireMSYProgress(New cMSYProgressArgs(0, iFlt, MSYeffort(iFlt)))
                    End If
                Next iFlt

                'done plugin

                'VC091103: What MSY biomass to use? a group may be caught by several fleets
                'as a first approach I will use the MSY biomass for the fleet that catches most of the species
                For igrp As Integer = 1 To Me.m_esData.nGroups
                    Dim BiggestCatch As Single = -1
                    Dim BiggestFleet As Integer = -1
                    For iflt As Integer = 1 To Me.m_esData.nGear
                        If m_epdata.Landing(iflt, igrp) + m_epdata.Discard(iflt, igrp) > 0 Then
                            'this fleet is catching this group
                            If m_epdata.Landing(iflt, igrp) + m_epdata.Discard(iflt, igrp) > BiggestCatch Then
                                BiggestCatch = m_epdata.Landing(iflt, igrp) + m_epdata.Discard(iflt, igrp)
                                BiggestFleet = iflt
                            End If
                        End If
                    Next
                    'now we know the biggestcatch, so save the biomass from there to the Bmsy:
                    If BiggestFleet > 0 Then
                        m_quota.Bbase(igrp) = CSng(bMSY(BiggestFleet, igrp))
                        'assume there's no fishing if the B is below half of the Bmsy
                        m_quota.Blim(igrp) = CSng(bMSY(BiggestFleet, igrp) * 0.5)
                        m_quota.Fopt(igrp) = CSng(fMSY(BiggestFleet, igrp))

                        'set the reference levels to Blim and Bbase
                        Me.m_data.BioBounds(igrp).Lower = m_quota.Blim(igrp)
                        Me.m_data.BioBounds(igrp).Upper = m_quota.Bbase(igrp)
                    End If
                Next

                'reset the number of years that Ecosim will run
                m_core.EcoSimModelParameters.NumberYears = NumberOfYears

                If Me.m_core.PluginManager IsNot Nothing Then
                    Me.m_pluginManager.MSYEffortCompleted(MSYeffort, MSYF)
                End If

                'MsgBox("MSY reference levels calculated")

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".RunMSYSearch() Exception: " & ex.Message)
                Me.m_core.Messages.SendMessage(New cMessage("Error while calculating MSY. " & ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.MSE, eMessageImportance.Critical))
            End Try


        End Sub


        Public Sub RunMSYSearchUsingFishingMortalityInsteadOfEffort()
            'WE'll run Ecosim for an additional 25 years to avoid the effort not being sustainable

            Me.m_data.StopRun = False

            Dim NumberOfYears As Integer = Me.m_esData.NumYears
            Dim extraYears As Integer = 25
            Dim nGroups As Integer = Me.m_data.NGroups

            'Setup Ecosim 
            'timestep handler that ecosim will call where we can grab data during the run
            'see the Private Sub onMSYEcosimTimestep(...)
            Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onMSYEcosimTimestep

            Me.m_esData.bTimestepOutput = True

            Dim MSYF(nGroups) As Single
            Dim MSYEffort(m_epdata.NumFleet) As Single

            'Dim GroupEffort() As Single

            'this is required to set the base effort values :
            m_core.EcoSimModelParameters.NumberYears = NumberOfYears + extraYears
            SetBaseValues()

            If Me.m_core.PluginManager IsNot Nothing Then
                Me.m_pluginManager.MSYRunStarted(Me.m_data, Me.m_quota, Me.m_esData)
            End If

            'next is a vc temp fix for debugging
            'Data.MSYStartTimeIndex = 649

            Try

                For iGrp As Integer = 1 To nGroups

                    If Data.MSYEvaluateGroup(iGrp) And Me.m_epdata.fCatch(iGrp) > 0 Then
                        'fCatch is the Ecopath sum of landings and discards 
                        Dim maxF As Single = 0
                        For iT As Integer = 1 To Data.MSYStartTimeIndex
                            If m_esData.FishRateNo(iGrp, iT) > maxF Then
                                maxF = Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FishMort, iGrp, iT)
                                '.m_esData.FishRateNo(iGrp, iT)
                            End If
                        Next

                        MSYF(iGrp) = 0
                        'ReDim GroupEffort(m_data.NGroups)
                        'For iGroup As Integer = 1 To Me.m_data.NGroups
                        'If Data.MSYEvaluateGroup(iGroup) Then
                        'If m_epdata.Landing(iFlt, iGroup) > 0 Then 'only if this fleet catches this group
                        Dim Done As Boolean = False
                        Dim CurValue As Double = 0
                        Dim lastValue As Double = 0
                        Dim maxValue As Double = 0

                        Dim lastF As Double = 0
                        Dim TooBigF As Double = -99
                        Dim TooLowF As Double = -0.5
                        Dim tryF As Double = 0.01

                        Dim NumberOfSteps As Integer = 0

                        If m_core.EcoSimModelParameters.NumberYears < NumberOfYears + extraYears Then
                            m_core.EcoSimModelParameters.NumberYears = NumberOfYears + extraYears
                        End If

                        System.Console.WriteLine()
                        Dim FFactor As Integer = 2
                        Dim CheckTangent As Boolean = False
                        Dim LastLowerF As Double = 0

                        Dim StoreF(12 * m_core.EcoSimModelParameters.NumberYears) As Single
                        SetFishingMortalityOverTime(iGrp, StoreF, False)

                        Do While Done = False

                            NumberOfSteps += 1

                            'let ecosim init to the new values
                            Me.m_Ecosim.Init(True)

                            Me.SetFishingMortality(iGrp, CSng(tryF))


                            'run ecosim with the current effort
                            Me.m_Ecosim.Run()

                            'evaluate the ecosim output for this fleet/effort combination
                            CurValue = Me.EvaluateMSYF(iGrp)

                            'if a fishery catches a group with low catch but high biomass, it may cause the effort to skyrocket
                            'to avoid this: set a limit on the F values for the exploited groups:
                            'if that happens set the value to a low value, so that it may try a lower effort
                            ' If CheckIfFishingMortalitiesTooHigh(iflt) Then CurValue = CurValue / 2


                            If CurValue = 0 Then Done = True 'no effort or no value
                            If CurValue < 0 Then Stop

                            If CheckTangent Then   'only get in here if we are checking the neighbourhood of the max value
                                If CurValue < maxValue Then
                                    'go downward
                                    TooLowF = LastLowerF
                                    TooBigF = lastF
                                Else
                                    'move upward
                                    LastLowerF = TooLowF
                                    ' TooLowEffort = lastEffort
                                    ' maxValue = CurValue
                                End If
                                CheckTangent = False
                            ElseIf CurValue > maxValue Then   'this is the normal entrypont when moving up
                                LastLowerF = TooLowF
                                TooLowF = lastF
                                If maxValue > 0 And TooBigF < 0 Then  'move faster if just starting out, and long way to go
                                    If CurValue / maxValue > 0.97 * FFactor Then FFactor = 4 Else FFactor = 2
                                End If
                                maxValue = CurValue
                                MSYF(iGrp) = CSng(tryF)
                            Else
                                If TooBigF < 0 Then
                                    TooBigF = tryF
                                    CheckTangent = True
                                Else
                                    'we are now somewhere below the msy effort, but at what side?
                                    If tryF > MSYF(iGrp) Then  'on the right side
                                        'reduce the toobigeffor to the current
                                        TooBigF = tryF
                                    Else   'below MSY
                                        TooLowF = tryF
                                    End If
                                End If
                            End If

                            If CheckTangent Then
                                tryF = 1.001 * TooLowF
                                'LastLowerEffort = TooLowEffort / 2
                            Else
                                If TooBigF < 0 Then 'NOT YET FOUND THE TOP, SO DOUBLE UP
                                    tryF = tryF * FFactor
                                Else  'have previously found a bigger effort that gave lower value, so now we have bounds
                                    tryF = (TooBigF - TooLowF) / 2 + TooLowF
                                End If
                            End If
                            'Cap the max F:
                            If tryF > maxF Then
                                tryF = maxF
                                TooBigF = tryF
                            End If


                            lastValue = CurValue
                            If tryF > 0 And CheckTangent = False Then
                                If Math.Abs(1 - lastF / tryF) < 0.01 Then Done = True
                            End If
                            lastF = tryF

                            System.Console.WriteLine(NumberOfSteps.ToString & ", Group = " & iGrp.ToString & _
                                                  ", MSY F = " & MSYF(iGrp).ToString & _
                                                  ", Cur F = " & tryF.ToString & _
                                                  ", toolow = " & TooLowF.ToString & _
                                                  ", toobig = " & TooBigF.ToString & _
                                                  ", maxvalue = " & maxValue.ToString & _
                                                  ", curvalue = " & CurValue.ToString)

                            'tell the interface an iteration has been completed
                            Me.fireMSYProgress(New cMSYProgressArgs(NumberOfSteps, iGrp, CSng(MSYF(iGrp))))


                            If Me.m_data.StopRun Then Exit Do
                        Loop


                        If Me.m_data.StopRun Then Exit For

                        'Finally reset the F to the original value 
                        SetFishingMortalityOverTime(iGrp, StoreF, True)
                    End If
                Next iGrp

                'done plugin

                'reset the number of years that Ecosim will run
                m_core.EcoSimModelParameters.NumberYears = NumberOfYears

                If Me.m_core.PluginManager IsNot Nothing Then
                    Me.m_pluginManager.MSYEffortCompleted(MSYEffort, MSYF)
                End If

                'MsgBox("MSY reference levels calculated")

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".RunMSYSearchUsingFishingMortalityInsteadOfEffort() Exception: " & ex.Message)
                Me.m_core.Messages.SendMessage(New cMessage("Error while calculating MSYF. " & ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.MSE, eMessageImportance.Critical))
            End Try


        End Sub



        Private Function EvaluateMSY(ByVal curFleet As Integer) As Single
            'MSY Search has just completed a run 
            'evaluate the value of the catch for this fleet with this effort level
            'Dim sumbio As Single

            'VC wants to change this so that it can calc Value or Biomass
            Dim FleetCatchValue As Single = 0
            Dim marketPrice As Single

            'System.Console.WriteLine()

            For igrp As Integer = 1 To Me.m_esData.nGroups

                If Me.m_epdata.Landing(curFleet, igrp) > 0 Then
                    If Me.m_data.MSYEvaluateValue Then
                        marketPrice = Me.m_epdata.Market(curFleet, igrp)
                    Else
                        marketPrice = 1
                    End If

                    'VC temp fix for debugging:
                    'marketPrice = 1

                    Dim GroupCatch As Single = 0
                    For it As Integer = Me.m_core.nEcosimTimeSteps To Me.m_core.nEcosimTimeSteps  'just one time step should do
                        'only evaluate for the last 25 years: LAST TIMESTEP
                        'For it As Integer = Me.m_esData.NTimes - 25 To Me.m_esData.NTimes
                        'get data stored by ecosim over time  
                        'Dim bio As Single = Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, it)
                        'sumbio += Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, it)
                        'FleetCatchValue += Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, it) * Me.m_epdata.Market(curFleet, igrp) ' * PropCaughtByThisGear
                        GroupCatch += m_esData.ResultsSumCatchByGroupGear(igrp, curFleet, it)
                        'System.Console.Write("Group " & igrp.ToString & " = " & FleetCatchValue.ToString & ", ")
                    Next
                    'average over the 25 years:
                    FleetCatchValue += (GroupCatch * m_data.MSYGroupWeight(igrp) * marketPrice)
                End If
            Next igrp
            Return FleetCatchValue

        End Function

        Private Function SetFishingEffort(ByVal Fleet As Integer, ByVal Val As Single) As Boolean

            Dim Manager As cFishingEffortManger = Me.m_core.FishingEffortShapeManager
            Dim Shape As cShapeData = Nothing

            Dim StartStep As Integer
            Dim EndStep As Integer
            If Fleet = 0 Then
                StartStep = 0
                EndStep = Me.m_core.nFleets - 1
            Else
                StartStep = Fleet - 1
                EndStep = Fleet - 1
            End If

            For iFl As Integer = StartStep To EndStep
                Shape = Manager.Item(iFl)
                Shape.LockUpdates()
                Try
                    Shape.ShapeData(1) = 1
                    For iTimeStep As Integer = Me.m_data.MSYStartTimeIndex To Me.m_core.nEcosimTimeSteps 'Step cCore.N_MONTHS
                        Shape.ShapeData(iTimeStep) = Val
                        'set effort to unity 
                    Next
                Catch ex As Exception
                    'Return False
                End Try
                Shape.UnlockUpdates()
            Next
            Manager.Update()
            Return True

        End Function

        Private Function EvaluateMSYF(ByVal curGroup As Integer) As Single
            'MSY Search has just completed a run 
            'evaluate the value of the catch for this group with this Fishing mortality

            Dim GroupCatch As Single = 0
            For it As Integer = Me.m_core.nEcosimTimeSteps To Me.m_core.nEcosimTimeSteps
                'only evaluate for the last timestep
                GroupCatch += m_esData.FishRateNo(curGroup, it) * m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, curGroup, it)
                '.Fish1.ResultsOverTime.ResultsSumCatchByGroupGear(igrp, iflt, it)
                'System.Console.Write("Group " & igrp.ToString & " = " & FleetCatchValue.ToString & ", ")
            Next
            'average over the 25 years:
            Return GroupCatch

        End Function


        Private Function SetFishingMortality(ByVal Group As Integer, ByVal Val As Single) As Boolean

            'Dim FManager As cFishingMortalityManger = Me.m_core.FishMortShapeManager
            'Dim Shape As cShapeData = Nothing
            Dim bSucces As Boolean = True
            'Shape = FManager.Item(Group)
            'Shape.LockUpdates()

            Try
                For iTimeStep As Integer = Me.m_data.MSYStartTimeIndex To Me.m_core.nEcosimTimeSteps 'Step cCore.N_MONTHS
                    '    Shape.ShapeData(iTimeStep) = Val
                    m_esData.FishRateNo(Group, iTimeStep) = Val 'FishRateNo(nGroups,nTime)
                Next
            Catch ex As Exception
                bSucces = False
            End Try

            'Shape.UnlockUpdates()
            'FManager.Update()

            Return bSucces

        End Function


        Private Function SetFishingMortalityOverTime(ByVal Group As Integer, ByVal Ftime() As Single, ByVal Setting As Boolean) As Boolean

            'Could not get direct setting of cfishingmortalitymanger to work:
            'Dim FManager As cFishingMortalityManger = Me.m_core.FishMortShapeManager
            'Dim Shape As cShapeData = Nothing
            Dim bSucces As Boolean = True
            'Shape = FManager.Item(Group)
            'Shape.LockUpdates()

            Try
                For iTimeStep As Integer = Me.m_data.MSYStartTimeIndex To Me.m_core.nEcosimTimeSteps
                    If Setting Then
                        m_esData.FishRateNo(Group, iTimeStep) = Ftime(iTimeStep)
                    Else
                        Ftime(iTimeStep) = m_esData.FishRateNo(Group, iTimeStep)
                    End If
                Next
                'For iTimeStep As Integer = Me.m_data.MSYStartTimeIndex To Me.m_core.nEcosimTimeSteps 'Step cCore.N_MONTHS
                '    If Setting Then
                '        Shape.ShapeData(iTimeStep) = Ftime(iTimeStep)
                '    Else
                '        Ftime(iTimeStep) = Shape.ShapeData(iTimeStep)
                '    End If
                'Next

            Catch ex As Exception
                bSucces = False
            End Try

            'Shape.UnlockUpdates()
            'FManager.Update()

            Return bSucces

        End Function


        Private Function CheckIfFishingMortalitiesTooHigh(ByVal curFleet As Integer) As Boolean

            CheckIfFishingMortalitiesTooHigh = False

            For iGrp As Integer = 1 To Me.m_esData.nGroups
                If Me.m_epdata.Landing(curFleet, iGrp) > 0 Then
                    'need to limit the fishing mortality to avoid groups being crashed completely, making a temp fix here
                    'm_esData.FishRateMax(iGrp) = m_epdata.PB(iGrp)
                    'If Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FishMort, iGrp, m_core.nEcosimTimeSteps) > 10 * m_epdata.PB(iGrp) Then
                    If Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, iGrp, m_core.nEcosimTimeSteps) < 0.000001 * m_epdata.B(iGrp) Then
                        ' should really use m_esData.FishRateMax(iGrp) Then
                        Return True
                    End If
                End If
            Next

        End Function

        Public Sub RunBoEstimation()

            'VC091202: We don't need the Bo reference now, using MSY levels instead
            'but leaving the code here for possible use later. 

            'We need a group-specific parameter Bo (unfished biomass). 
            'Its default value is obtained from the fitted model.  
            'We run  the  model for another 50 years. 
            'Then set the fishery for  the species  in question  to 0, 
            'leave other fisheries constant at  the last year’s  effort level. 
            'The  biomass for the species at the  end of the simulation  is our default Bo.      

            'Setup Ecosim 
            'timestep handler that ecosim will call where we can grab data during the run
            'see the Private Sub onMSYEcosimTimestep(...)

            'Try

            '    Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onMSYEcosimTimestep

            '    Me.m_esData.bTimestepOutput = True

            '    Dim iDataset As Integer = Me.m_core.ActiveTimeSeriesDatasetIndex
            '    Dim DS As cTimeSeriesDataset

            '    If iDataset > -1 Then DS = Me.m_core.TimeSeriesDataset(iDataset)

            '    Dim NumberOfYears As Integer = Me.m_esData.NumYears
            '    m_core.EcoSimModelParameters.NumberYears = NumberOfYears + 100
            '    SetBaseValues()


            '    'Setup Ecosim 
            '    'timestep handler that ecosim will call where we can grab data during the run
            '    'see the Private Sub onMSYEcosimTimestep(...)
            '    Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onMSYEcosimTimestep

            '    Me.m_esData.bTimestepOutput = True

            '    For igrp As Integer = 1 To Me.m_esData.nGroups

            '    Next

            '    'Dim MSYeffort(Me.m_esData.nGear) As Single
            '    'Dim MSYbiomass(Me.m_epdata.NumGroups, Me.m_esData.nGear) As Single
            '    'Finally reset the effort to the original effort (for all fleets)
            '    SetEffortToBaseValue(True)


            '    'reset the number of years that Ecosim will run
            '    m_core.EcoSimModelParameters.NumberYears = NumberOfYears

            'Catch ex As Exception

            'End Try


        End Sub

        Private Sub fireMSYProgress(ByVal MYSProgress As cMSYProgressArgs)

            If Me.m_data.MSYRunSilent Then Exit Sub

            Try
                If Me.m_MSYCallBack IsNot Nothing Then
                    Me.m_MSYCallBack(MYSProgress)
                End If
            Catch ex As Exception

            End Try

        End Sub


        ''' <summary>
        ''' Ecosim Timestep delegate handler for the MSY Search
        ''' </summary>
        ''' <param name="iTime"></param>
        ''' <param name="data"></param>
        ''' <remarks></remarks>
        Private Sub onMSYEcosimTimestep(ByVal iTime As Long, ByVal data As cEcoSimResults)

            Try

                'Ecosim has run a time step for the MSY search
                'grab up anything you need during the time step

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".onEcosimTimestep() Error: " & ex.Message)
            End Try

        End Sub

#End Region

#End Region

#Region "Time step data summary"
        ''' <summary>
        ''' Set Biomass and F cv's to use for this year.
        ''' </summary>
        ''' <remarks>CV's can vary on a yearly timestep set by the interface. This sets the CV to use for this year. </remarks>
        Public Sub setTime(ByVal itime As Integer, ByVal iyr As Integer)

            If Me.m_Search.SearchMode <> eSearchModes.MSE Then Return

            Me.m_curT = itime
            Me.m_curYear = iyr

            'CVbiomEst(ngroups) and CVFest(nfleets) are the cv that are used to vary biomass and fishing mortality
            For igrp As Integer = 1 To Me.m_data.NGroups
                Me.m_data.CVbiomEst(igrp) = Me.m_data.CVBiomT(igrp, iyr)
            Next

            For iflt As Integer = 1 To Me.m_data.nFleets
                Me.m_data.CVFest(iflt) = Me.m_data.CVFT(iflt, iyr)
            Next

        End Sub

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
                    Me.m_data.BioStats.AddValue(igrp, CInt(iTime), Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, CInt(iTime)))
                    Me.m_data.CatchGroupStats.AddValue(igrp, CInt(iTime), Me.m_esData.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, CInt(iTime)))
                Next igrp

                For iflt As Integer = 1 To Me.m_esData.nGear
                    Me.m_data.CatchFleetStats.AddValue(iflt, CInt(iTime), Me.m_esData.ResultsSumCatchByGear(iflt, CInt(iTime)))
                    Me.m_data.EffortStats.AddValue(iflt, CInt(iTime), Me.m_esData.ResultsEffort(iflt, CInt(iTime)))
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
        Private Sub onEconomicData(ByVal EconomicData As EwEUtils.Core.IEconomicData) Handles m_EconomicData.onEconomicData

            Try

                'Is there plugin economic data
                If Me.UsePlugin Then
                    'Plugin economic data from the ValueChain pluging is sent out every timestep
                    'Store the data in cMSESummaryStats objects

                    Me.m_data.ProfitSum.AddValue(1, EconomicData.TimeStep, EconomicData.Total.Profit)
                    Me.m_data.JobsSum.AddValue(1, EconomicData.TimeStep, EconomicData.Total.NumberOfJobsTotal)
                    Me.m_data.CostSum.AddValue(1, EconomicData.TimeStep, EconomicData.Total.Cost)

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

                'Me.m_data.ProfitSum.AddValue(1, Me.m_Search.totval)
                'Me.m_data.JobsSum.AddValue(1, sumJobs)
                'Me.m_data.CostSum.AddValue(1, sumCost)

            End If

        End Sub

        Public Sub RunFleetTradeoffs()

            Me.m_data.StopRun = False
            Dim buff As System.Text.StringBuilder
            Dim strm As StreamWriter
            Try

                'Exit Sub

                'no need to set time just use default: m_core.EcoSimModelParameters.NumberYears = NumberOfYears + 25
                'this is required to set the base effort values :
                SetBaseValues()

                Me.m_Ecosim.TimeStepDelegate = AddressOf Me.onMSYEcosimTimestep

                Me.m_esData.bTimestepOutput = True
                'let ecosim init to the new values
                Me.m_Ecosim.Init(True)
                'run ecosim with the current effort
                Me.m_Ecosim.Run()

                Dim nFleets As Integer = Me.m_esData.nGear
                Dim FleetBaseValue(nFleets) As Single
                Dim CurValue() As Single
                'Store the total base value obtained by each fishery 
                For iFlt As Integer = 1 To nFleets
                    'For iGrp As Integer = 1 To m_epdata.NumGroups
                    For it As Integer = 1 To Me.m_esData.NTimes
                        FleetBaseValue(iFlt) += m_esData.ResultsSumValueByGear(iFlt, it)  'm_esData.ResultsSumCatchByGroupGear(iGrp, iFlt, it) * Me.m_epdata.Market(iFlt, iGrp)
                    Next
                    'all of these values are annual values (even if they are by time step), so divide by number of months:
                    FleetBaseValue(iFlt) /= m_esData.NTimes
                    'Next
                Next
                Dim ValueDifferenceFromTo(nFleets, nFleets) As Single


                For iFlt As Integer = 1 To nFleets
                    Dim Manager As cFishingEffortManger = Me.m_core.FishingEffortShapeManager
                    Dim Shape As cShapeData = Nothing

                    Shape = Manager.Item(iFlt - 1)
                    Shape.LockUpdates()

                    For iT As Integer = 1 To Me.m_esData.NTimes
                        Shape.ShapeData(iT) = CSng(0.9 * m_baseEffort(iFlt, iT))
                    Next
                    Shape.UnlockUpdates()
                    Manager.Update()


                    'For it As Integer = 1 To Me.m_esData.NTimes
                    '    Me.m_esData.FishRateGear(iFlt, it) = CSng(1.1 * m_baseEffort(iFlt, it))
                    'Next
                    'let ecosim init to the new values ------ no init will overwrite the effort!!!!!
                    ' Me.m_Ecosim.Init(True)
                    'run ecosim with the current effort
                    Me.m_Ecosim.Run()

                    ReDim CurValue(nFleets)
                    For iTo As Integer = 1 To nFleets
                        For it As Integer = 1 To Me.m_esData.NTimes
                            CurValue(iTo) += m_esData.ResultsSumValueByGear(iTo, it)      'm_esData.ResultsSumCatchByGroupGear(iGrp, iFlt, it) * Me.m_epdata.Market(iFlt, iGrp)
                        Next
                        'divide by no months to get the average, which is the annual value:
                        CurValue(iTo) /= m_esData.NTimes
                    Next


                    'If MoreMoney = 0 Then Stop

                    For iTo As Integer = 1 To nFleets
                        ValueDifferenceFromTo(iFlt, iTo) = (CurValue(iTo) - FleetBaseValue(iTo)) '/ MoreMoney
                    Next


                    'get the directory to dump the data to
                    'Me.m_DataDir = AppDomain.CurrentDomain.BaseDirectory & "MSE\"
                    'strm = New StreamWriter(getFilename("FleetTradeOff", "_Effort"), True)
                    'For iFrom As Integer = 1 To nFleets
                    '    Try
                    '        buff = New System.Text.StringBuilder
                    '        For iT As Integer = 1 To m_esData.NTimes Step 12
                    '            buff.Append(Me.m_esData.FishRateGear(iFrom, iT).ToString & ", ")
                    '        Next
                    '        strm.WriteLine(buff)

                    '        buff = Nothing
                    '    Catch ex As Exception
                    '        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                    '        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & getFilename("FleetTradeOff", Me.m_epdata.FleetName(iFrom)) & " Exception: " & ex.Message)
                    '    End Try
                    'Next
                    'strm.Close()


                    'Finally reset the effort to the original effort
                    'SetEffortToBaseValue(True)
                    Dim Manager2 As cFishingEffortManger = Me.m_core.FishingEffortShapeManager
                    Dim Shape2 As cShapeData = Nothing
                    Shape2 = Manager2.Item(iFlt - 1)
                    'Reset the fishing values
                    Shape2.LockUpdates()
                    For iT As Integer = 1 To Me.m_esData.NTimes
                        Shape2.ShapeData(iT) = m_baseEffort(iFlt, iT)
                    Next
                    Shape2.UnlockUpdates()
                    Manager2.Update()

                Next


                'get the directory to dump the data to
                Me.m_DataDir = AppDomain.CurrentDomain.BaseDirectory & "Tradeoff\"
                Dim mName As String = m_core.m_EwEModelName

                strm = New StreamWriter(buildFilename("FleetTradeOff_", mName), False)
                buff = New System.Text.StringBuilder
                'First a line with a blank, then the fleet names
                'buff.Append("From\to ,")
                'For iTo As Integer = 1 To nFleets
                '   buff.Append(Me.m_epdata.FleetName(iTo).ToString & ", ")
                'Next
                'strm.WriteLine(buff)
                For iFrom As Integer = 1 To nFleets
                    Try
                        buff = New System.Text.StringBuilder

                        buff.Append(Me.m_epdata.FleetName(iFrom).ToString & ", ")
                        Dim vSum As Single = 0
                        For iTo As Integer = 1 To nFleets
                            buff.Append(ValueDifferenceFromTo(iFrom, iTo).ToString & ", ")
                            vSum += ValueDifferenceFromTo(iFrom, iTo)
                        Next
                        buff.Append(vSum.ToString)
                        strm.WriteLine(buff)

                        buff = Nothing
                    Catch ex As Exception
                        ' Debug.Assert(False, Me.ToString & " Exception saving results to file " & getFilename(BIOMASS_DATA, Me.m_epdata.GroupName(igrp)))
                        System.Console.WriteLine(Me.ToString & " Failed to write data to file " & buildFilename("FleetTradeOff", Me.m_epdata.FleetName(iFrom)) & " Exception: " & ex.Message)
                    End Try
                Next




            Catch ex As Exception

            End Try
            strm.Close()
        End Sub



#End Region




    End Class

#End Region

End Namespace
