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

#Region " Imports "

Option Strict On
Option Explicit On
Imports System.Threading
Imports EwECore.MSE
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace Ecosim

#Region "Public defintions"

    ''' <summary>
    ''' Reason the model completed
    ''' </summary>
    Public Enum eEcoSimCompletedReason
        ''' <summary>The model finished the run successfully</summary>
        Completed
        ''' <summary>User stopped model from completing</summary>
        UserInterupted
        ''' <summary>Error while model was running</summary>
        ErrorEncountered
    End Enum

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Delegate definitions

    ''' <summary>
    ''' Definition of Time Step Delegate use for notification of an EcoSim time step
    ''' </summary>
    ''' <param name="iTime">Index of the completed time step (1 to n time steps)</param>
    ''' <param name="data">cEcoSimResults object containing results from this time step (iTime)</param>
    ''' <remarks>This delegate will get called at the end of each time step to pass data out of the model</remarks>
    Public Delegate Sub EcoSimTimeStepDelegate(ByVal iTime As Long, ByVal data As cEcoSimResults)

    Public Delegate Sub EcoSimRunCompletedDelegate(ByVal obj As Object)

#End Region

    ''' <summary>
    ''' Class to encapsulate EcoSim Model
    ''' </summary>
    Public Class cEcosimModel

        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        'ToDo Forced F Time Series changed
        'jb 20-Feb-2018 something looks wrong with the Landings and Value plots 
        '   when an f time series is applied and the run time is extented.
        '   Values plots for the extention but not landings?? This may be OK...
        '   Intuitively I would have thought either both or neither
        'xxxxxxxxxxxxxxxxxxxxxxxxx

        'ToDo's Discards 1-Nov-2016 Check that discards to detritus are accounted for correctly when driving discard mortality rates
        'ToDo's Discards 1-Nov-2016 Are the discards survivals being account for when there is no landing and all discards survive. See PopulateResults()
        'ToDo's Discards 1-Nov-2016 How to deal with Forced F and Catches. Will the current accounting in PopulateResults() work. Are discards that survived in F (nope) and Catches (most likley not)
        'ToDo's Discards 1-Nov-2016 Null and Zero values in the dataset still need to get sorted out. 
        '                              I disabled the first version because Of an initialzation bug.
        'ToDo's Discards 1-Nov-2016 Still need to debug driving mulitple fleets on the same groups.

#Region "Data Private and Public"

        'ToDo_jb Ecosim Core comunication. Implementation needs to be completed. 
        'Ecosim comunicates with the core via Ecosim's message publisher which the core can add a handler to.
        'This keep the core out of Ecosim. 
        'This mechanism is setup but has not really been used

        Private m_ConTracer As cContaminantTracer
        Private m_TracerData As cContaminantTracerDataStructures
        Private m_MSEData As cMSEDataStructures
        'Management Strategy Evaluator 
        Private m_MSE As MSE.cMSE

        Private m_pluginManager As cPluginManager

        Private m_RefData As cTimeSeriesDataStructures

        Private m_publisher As New cMessagePublisher
        Private m_Ecofunctions As cEcoFunctions

        Private m_EnviroResponseManager As cEcosimEnviroResponseManager
        Private m_MortalityResponseManager As cEcosimMortalityResponseManager

        ' Private Ntimes As Integer
        Private StepsPerYear As Integer
        Private TimeNow As Integer
        Private YearNow As Integer
        Private DeltaT As Single 'delta time in years one month set in SetTimeSteps
        Private nvar As Integer

        Private DoingEiiSaving2Round As Boolean
        Private MakeTestData As Boolean
        Private AbortRun As Boolean
        Private IsDatWtSet As Boolean

        Friend A(,) As Single

        Private dydx() As Single

        'Private GearIncludeInEquil() As Single
        Private BaseValue As Single

        Friend BB() As Single

        'by nGroups
        Private pbbase() As Single
        Private StartEatenBy() As Single
        Private EatenByBase() As Single
        Private StartEatenOf() As Single
        Private SimQB() As Single
        Private Mtotal() As Single

        Private SimGES() As Single
        Private IadCode() As Integer, IjuCode() As Integer, IecoCode() As Integer

        Private nGroups As Integer
        Private Irun As Integer 'arrays for state checking to debug ecosim when runs don't repeat
        Private Qmult() As Single, CurrentProfit() As Single
        Private CurrentIncome() As Single, CapBase() As Single ', PcapBase() As Single
        Private EscalePar() As Single, CapTime() As Single ', Epower() As Single
        ' Private CapDepreciate() As Single
        ' Private CapBaseGrowth() As Single,
        Private CapGrowthFactor() As Single
        Private CostPenaltyConstant As Double
        Private BestTime() As Single
        '       Epower(ig)    effort response power parameter, default 3.0
        '       PcapBase(ig)  initial effort as proportion of initial capital capacity, default 0.5
        '       CapDepreciate(ig)  capital depreciation rate, default 0.06
        '       CapBaseGrowth(ig) initial capital growth rate (proportional, /year), default 0.2

        'Adding energy content to EwE: Villy 2025-03-12
        Public EnergyBase() As Single
        Public EnergyUsed As Boolean


        'Private BaseConsumption(50, 50) As Single
        'Parameter names changed as follows:
        'In CW's ECOSIM  In ECOPATH
        '==============  ==========
        'n               N1             number of groups total
        'nnd             N              number of living groups
        'poolnames$()    Specie$()
        'iprod           PP()           iprod=PP-2
        'export          EX()
        'Catch           C()
        'ipt             ImpVar()
        'cef()           GE()
        'b()             bb()           b used for biomass
        'C               CC             var in Jacobian,name in use
        'info$           inf$           name in use
        'fish()         Fish1()        name in use
        'a()             aa()           name in use
        'tzero()         TimeJuv()      Based on T0 from VBGF I calculate the TimeJuv(),
        'Thus t0<>tzero : different meanings in the two versions
        'Fileform.frm is not used in the ECOPATH/ECOSIM version
        'The following variables are scaled after npairs:
        'wjuv   wk Zadult  Zjuv
        'wzero  rzero   vbK
        'timejuv    mintimejuv  maxtimejuv
        'Prepo WtGrow   SaveSizeAd  SaveSizeJuv
        't0 LkLoo Loo AinLW     BinLW

        ' Private XplotLast() As Single, YplotLast() As Single
        Private fbasetest As Single, fstep As Single
        Private fishingrate As Single, fstepp As Single, lastharvest As Single, equilharvest As Single
        Private lastvalue As Single, equilvalue As Single, cbval As Single, equilstock As Single
        Private LinScale As Single
        Public Srec() As Single 'ww changed to public
        Private TimeSeriesFile As String

        'fitting of model to time series data
        ''' <summary>sumof log(observed/predicted) (by time series)</summary>
        Private DatSumZ() As Single
        ''' <summary>sumof log(observed/predicted)^2 (by time series)</summary>
        Private DatSumZ2() As Single
        ''' <summary>number of observation</summary>
        Private DatNobs() As Integer
        'Private eDatq() As Single
        Private NobsTime() As Single
        Private DatDev(,) As Single

        Private NutPBmax As Single
        Private maxKageMax As Object


        '''' <summary>
        '''' Feeding Time scaling value
        '''' </summary>
        '''' <remarks>Default value = one set in InitialState. Computed at the end of each time step in rk4()</remarks>
        'Public Ftime() As Single
        'Publicm_data.Hden() As Single
        Private CBlast() As Single
        Private PredPerBiomass() As Single

        ''' <summary>Boolean flag use to force biomass with the data in m_refData.PoolForceBB(ngroup,nyears). </summary>
        ''' <remarks>True means Biomass is forced with data from m_refData.PoolForceBB(). False biomass is computed.
        ''' ResetPred() is set immediately before the rk4() is run and immediately after. 
        ''' The flag is used by setPred() which is called be Derivt() and SplitUpdate() which are called by the rk4()</remarks>
        Public ResetPred() As Boolean 'ww set to public for new stanza stuff

        Private pbb() As Single
        Private SimGEtemp() As Single
        Private biomeq() As Single

        Private deriv() As Single
        Private RiskRate() As Single
        Private Qopt() As Single
        Private yt() As Single
        Private dyt() As Single
        Private dym() As Single

        Public Nrec() As Single
        Public Brec() As Single

        'changed to public
        Public AhatStanza() As Single
        Public RhatStanza() As Single

        Private Rbase() As Single
        Private BrecYear() As Single

        Private Dfitness() As Single
        Private Deatenby() As Single
        Private Deatenof() As Single

        Private Bstore(,) As Single

        'Core data structures

        'jb for know make the datastructures Friend so that the Core can acceess them to make changes
        Friend m_Data As cEcosimDatastructures
        Friend m_stanza As cStanzaDatastructures
        'EcoSim needs to be initialized by the output of EcoPath
        'the cEcoPathDatastructures object holds that data
        Private m_EPData As cEcopathDataStructures

        Private m_search As cSearchDatastructures

        'private variable
        Private m_Results As cEcoSimResults

        Private m_bInit As Boolean
        Private m_OnTimeStepDelegate As EcoSimTimeStepDelegate = Nothing
        Private m_OnRunCompletedDelegate As EcoSimRunCompletedDelegate

        ''' <summary>
        ''' Public flag to stop the currently running EcoSim model
        ''' </summary>
        ''' <remarks></remarks>
        Public bStopRunning As Boolean

        Private m_SyncObj As System.Threading.SynchronizationContext

        ''' <summary> Biomass averaged from sub timesteps to a monthly value</summary>
        Private BBAvg() As Single

        ''' <summary> Loss averaged from sub timesteps to a monthly value</summary>
        Private LossAvg() As Single

        Private EatenByAvg() As Single
        Private EatenOfAvg() As Single
        Private PredAvg() As Single

        'Private RiskRateAvg() As Single
        Private fCatch0() As Single

#End Region

#Region "Construction Destruction"

        Sub New(ByVal functions As cEcoFunctions)
            Me.m_bInit = False
            Me.m_Ecofunctions = functions
            Me.bStopRunning = False

            Me.m_SyncObj = System.Threading.SynchronizationContext.Current
            'if there is no current context then create a new one on this thread. 
            'this happens if no interface has been created yet(I think...)
            If (Me.m_SyncObj Is Nothing) Then Me.m_SyncObj = New System.Threading.SynchronizationContext()

        End Sub


        ''' <summary>
        ''' Call initialization routines for EcoSim
        ''' </summary>
        ''' <returns>True if successful. False otherwise</returns>
        ''' <remarks>
        ''' Was called StartEcoSim() in original code
        ''' </remarks>
        Public Function Init(ByVal bFullInitialization As Boolean) As Boolean

            Try

                'Ecosim data is about to initialize
                If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimPreDataInitialized(Me.m_Data)

                'jb this may need to be called every time Ecosim is Run 
                'to make sure any edits made in Ecosim are used for the initialization
                Me.m_Results = New cEcoSimResults(Me.nGroups, Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit, Me.m_EPData.NumFleet)

                Me.RedimEcoSimVars()

                'jb 12-Feb-2010 Reloading of forcing data overwrites user edits of effort
                'We just need to make sure the forcing data arrays are dimensioned big enough to handle the run length
                Me.m_RefData.nGroups = Me.nGroups
                Me.m_RefData.redimForcingData(Me.m_Data.NumYears + Me.m_search.ExtraYearsForSearch)

                Me.DefaultDF()

                'Init Propdiscardtime() and Proplandedtime() to Ecopath values
                Me.InitPropLanded()

                Me.SetStartBiomass()     'Startbiomass must be set before SimFile is opened
                Me.RemoveImportFromEcosim()

                Me.Calc_nvar()

                Me.CalcEatenOfBy()
                Me.CalcStartEatenOfBy()

                Me.SetupSimVariables()  'sets vulrate()

                Me.InitialState() 'uses vulrate() to set A()

                Me.DefaultMigrationAndToDetritus()
                Me.CalcMo()
                Me.CalcBaseAdditiveMort()

                Me.InitStanza()

                Me.BaseValueOfHarvest()
                Me.BaseValueOfFishMGear()
                Me.SetRelativeCatchabilities()

                ' Prey relative energy base calculation, Villy 2025-03-12
                Me.EnergyBaseCalculation()

#If DEBUG Then
                'Make sure FishMGear and relQ are set correctly 
                Me.debugTestRelQFishMGear()
#End If

                If bFullInitialization Then
                    Me.SetBaseFFromGear()
                End If

                Me.SetTimeSteps()

                Me.CalculateAssimilationEfficiencies()

                Me.m_ConTracer = New cContaminantTracer()

                Me.m_publisher.sendAllMessages()

                Return True
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Function

        Public Sub Clear()

            Me.A = Nothing 'nGroups, nGroups
            Me.RiskRate = Nothing 'nGroups
            'Me.BaseConsumption = Nothing

            Me.ResetPred = Nothing 'nGroups
            Me.EscalePar = Nothing 'm_EPData.NumFleet
            Me.CapGrowthFactor = Nothing 'm_EPData.NumFleet

            Me.IadCode = Nothing 'nGroups
            Me.IjuCode = Nothing 'nGroups
            Me.IecoCode = Nothing 'nGroups

            Me.Qopt = Nothing 'nGroups

            Me.BB = Nothing 'nGroups
            Me.biomeq = Nothing 'nGroups

            Me.Brec = Nothing 'nGroups
            Me.CBlast = Nothing 'nGroups
            Me.deriv = Nothing 'nGroups
            Me.Deatenby = Nothing 'nGroups
            Me.Deatenof = Nothing 'nGroups
            Me.Dfitness = Nothing 'nGroups

            Me.dydx = Nothing 'nGroups
            Me.dym = Nothing 'nGroups
            Me.dyt = Nothing 'nGroups
            Me.yt = Nothing 'nGroups

            Me.EatenByBase = Nothing 'nGroups

            Me.Nrec = Nothing 'nGroups
            Me.pbb = Nothing 'nGroups
            Me.pbbase = Nothing 'nGroups
            Me.Rbase = Nothing 'nGroups
            Me.SimGEtemp = Nothing 'nGroups

            Me.StartEatenOf = Nothing 'nGroups
            Me.Mtotal = Nothing 'nGroups
            Me.StartEatenBy = Nothing 'nGroups
            Me.SimQB = Nothing 'nGroups

            Me.BB = Nothing 'nGroups
            Me.biomeq = Nothing 'nGroups
            Me.dydx = Nothing 'nGroups
            Me.dym = Nothing 'nGroups
            Me.dyt = Nothing 'nGroups
            Me.EatenByBase = Nothing 'nGroups
            Me.yt = Nothing 'nGroups
            Me.Bstore = Nothing 'nGroups, nts

            Me.m_Results = Nothing

        End Sub


#End Region

#Region "Multi threading code"

        Private Sub RunModelThreaded(ByVal obj As Object)

            Try
                Me.RunModelValue(Me.m_Data.NumYears, Me.m_search.Frates, Me.m_search.nBlocks)
            Catch ex As Exception
                Me.Messages.AddMessage(New cMessage(ex.Message, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Critical))
            End Try


            Try
                Me.m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.fireRunCompleted), Nothing)
            Catch ex As Exception
                Debug.Assert(False, "Exception calling Ecosim.OnRunCompleted() Exception: " & ex.Message)
            End Try

        End Sub


#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Exposes the MessagePublisher instance so that the core can add message handlers
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Messages() As cMessagePublisher
            Get
                Return Me.m_publisher
            End Get
        End Property

        Public Property SearchData() As cSearchDatastructures
            Get
                Return Me.m_search
            End Get
            Set(ByVal value As cSearchDatastructures)
                Me.m_search = value
            End Set
        End Property

        ''' <summary>
        ''' Get or Set the EcoPath data
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' This is used to initialize EcoSim with the last EcoPath estimated parameters
        ''' See cModelInterface.InitEcoSim(...)
        ''' </remarks>
        Public Property EcopathData() As cEcopathDataStructures
            Get
                EcopathData = Me.m_EPData
            End Get

            Set(ByVal NewParameters As cEcopathDataStructures)
                Me.m_EPData = NewParameters
            End Set

        End Property

        ''' <summary>
        ''' Get or Set the Ecosim data
        ''' </summary>
        ''' <value></value>
        ''' <remarks>
        ''' </remarks>
        Public Property EcosimData() As cEcosimDatastructures
            Get
                Return Me.m_Data
            End Get

            Set(ByVal value As cEcosimDatastructures)
                Me.m_Data = value
            End Set
        End Property

        Public Property TimeSeriesData() As cTimeSeriesDataStructures
            Get
                Return Me.m_RefData
            End Get
            Set(ByVal newValue As cTimeSeriesDataStructures)
                Me.m_RefData = newValue
            End Set
        End Property

        Public Property MSEData() As cMSEDataStructures
            Get
                Return Me.m_MSEData
            End Get
            Set(ByVal value As cMSEDataStructures)
                Me.m_MSEData = value
            End Set
        End Property

        ''' <summary>
        ''' Get/set the time step delegate for Ecosim to call at each time step
        ''' </summary>
        Public Property TimeStepDelegate() As EcoSimTimeStepDelegate
            Get
                Return Me.m_OnTimeStepDelegate
            End Get
            Set(ByVal value As EcoSimTimeStepDelegate)
                Me.m_OnTimeStepDelegate = value
            End Set
        End Property

        Public Property RunCompletedDelegate() As EcoSimRunCompletedDelegate
            Get
                Return Me.m_OnRunCompletedDelegate
            End Get
            Set(ByVal value As EcoSimRunCompletedDelegate)
                Me.m_OnRunCompletedDelegate = value
            End Set
        End Property


        <CLSCompliant(False)>
        Public Property PluginManager() As cPluginManager
            Get
                Return Me.m_pluginManager
            End Get
            Set(ByVal pm As cPluginManager)
                Me.m_pluginManager = pm
            End Set
        End Property


        Public Property EcosimEnviroResponseManager() As cEcosimEnviroResponseManager
            Get
                Return Me.m_EnviroResponseManager
            End Get
            Set(ByVal manager As cEcosimEnviroResponseManager)
                Me.m_EnviroResponseManager = manager
            End Set
        End Property


        Public Property EcosimMortalityResponseManager() As cEcosimMortalityResponseManager
            Get
                Return Me.m_MortalityResponseManager
            End Get
            Set(ByVal manager As cEcosimMortalityResponseManager)
                Me.m_MortalityResponseManager = manager
            End Set
        End Property




        Public ReadOnly Property ConTracer() As cContaminantTracer
            Get
                Return Me.m_ConTracer
            End Get
        End Property

        Public Property TracerData() As cContaminantTracerDataStructures
            Get
                Return Me.m_TracerData
            End Get
            Set(ByVal value As cContaminantTracerDataStructures)
                Me.m_TracerData = value
            End Set
        End Property


        Private Sub initConTracer()

            Try
                'only run the initialization if contaminant tracer is turned on
                'this should also mean it has been loaded with a scenario
                If Me.m_TracerData.EcoSimConSimOn Then
                    Me.m_ConTracer.Init(Me.m_TracerData, Me.m_EPData, Me.m_Data, Me.m_stanza)
                    Me.m_ConTracer.CInitialize()
                End If

            Catch ex As Exception
                cLog.Write(ex)
                Me.m_TracerData.EcoSimConSimOn = False
            End Try

        End Sub


#End Region

#Region "Public functions"

        Public Function SetCounters() As Boolean
            'jb hack this need to be somewhere else
            Me.nGroups = Me.m_EPData.NumGroups
            Me.m_Data.nGear = Me.m_EPData.NumFleet
            Return True
        End Function


        Public Sub CalculateAssimilationEfficiencies()
            '041012 VC calculating assimilation efficiency for variable p/q
            'following Kerims African paper
            For i As Integer = 1 To Me.m_EPData.NumLiving
                If Me.m_EPData.vbK(i) > 0 Then Me.m_Data.AssimEff(i) = Me.m_EPData.GE(i) / Me.m_EPData.PB(i) * (Me.m_EPData.PB(i) + 3 * Me.m_EPData.vbK(i))
            Next
        End Sub


        ''' <summary>
        ''' Init Propdiscardtime(fleets,groups) and PropLandedTime(fleets, groups) to Ecopath landing and discards
        ''' </summary>
        ''' <remarks>This must be call before <see cref="EwECore.Ecosim.cEcosimModel.Init">Ecosim.Init(Boolean)</see> 
        ''' so Propdiscardtime() and PropLandedTime() can be used to init <see cref="cEcosimDatastructures.FishRateNo">FishRateNo()</see> (fishing mortality)</remarks>
        Private Sub InitPropLanded()
            For iflt As Integer = 1 To Me.m_Data.nGear
                For igrp As Integer = 1 To Me.nGroups
                    'jb 7-Jan-2010 addded PropDiscardMort() so the default for discards contain only the mort
                    Me.m_Data.PropDiscardMortTime(iflt, igrp) = Me.m_EPData.PropDiscardMort(iflt, igrp)
                    Me.m_Data.PropDiscardTime(iflt, igrp) = Me.m_EPData.PropDiscard(iflt, igrp) * Me.m_Data.PropDiscardMortTime(iflt, igrp)
                    Me.m_Data.PropLandedTime(iflt, igrp) = Me.m_EPData.PropLanded(iflt, igrp)

                Next
            Next

        End Sub

        'Energy calculation, just needs to be done once after Ecosim is loaded
        ' Could instead be done when Ecopath model is balanced, it only usse Ecopath input

        Private Sub EnergyBaseCalculation()
            For i As Integer = 1 To Me.m_EPData.NumLiving
                If Me.m_EPData.Energy(i) <> 1 Then Me.EnergyUsed = True
            Next

            If Me.EnergyUsed Then
                For i As Integer = 1 To Me.m_EPData.NumLiving     'predator
                    If Me.m_EPData.QB(i) > 0 Then
                        Me.EnergyBase(i) = 0
                        For j As Integer = 1 To Me.nGroups     'prey
                            Me.EnergyBase(i) = Me.EnergyBase(i) + Me.EcopathData.DC(i, j) * Me.m_EPData.Energy(j)
                        Next
                    End If
                    If (EnergyBase(i) = 0) Then EnergyBase(i) = 1 ' To avoid divisions by zero etc
                Next
            End If

        End Sub



        Friend Sub SetRelativeCatchabilities()
            Dim i As Integer
            Dim j As Integer
            'set relative catchabilities by gear type, treating effort for each gear as starting at base
            'value of 1.0 so that F for the gear (F=qE=C/B) is 1.0xq where q is relative catchability
            'this avoids measuring effort in some unnecessary data units

            For i = 1 To Me.m_Data.nGear
                For j = 1 To Me.nGroups
                    'total catch rate 
                    'Includes discards that survive
                    Me.m_Data.relQ(i, j) = (Me.m_EPData.Landing(i, j) + Me.m_EPData.Discard(i, j)) / Me.m_Data.StartBiomass(j)

                    'For it As Integer = 1 To Me.m_Data.NTimes
                    '    m_Data.relQt(i, j, it) = m_Data.relQ(i, j)
                    'Next

                Next
            Next

        End Sub

        Public Sub InitMSE(ByRef MSEModel As MSE.cMSE)
            Me.m_MSE = MSEModel
        End Sub

        'Public Sub InitFPS(ByRef FPS As cFishingPolicySearch)
        '    Me.m_FPS = FPS
        'End Sub

        ''' <summary>
        ''' Scale vulnerabilities to trophic level
        ''' </summary>
        ''' <param name="Lower">The lower vulnerability to scale from.</param>
        ''' <param name="Upper">The upport vulnerability to scale to.</param>
        ''' <returns>True if successful.</returns>
        ''' <remarks>
        ''' Ported from EwE5 frmSim1.cmdfTL_Click
        ''' </remarks>
        Public Function ScaleVulnerabilitiesToTL(ByVal Lower As Single, ByVal Upper As Single) As Boolean

            If Lower >= 1 And Upper > 1 And Upper > Lower Then

                Dim maxTL As Single = 0
                For i As Integer = 1 To Me.m_EPData.NumLiving
                    If Me.m_EPData.TTLX(i) > maxTL Then maxTL = Me.m_EPData.TTLX(i)
                Next
                If maxTL < 2.1 Then
                    ' ToDo: send notification message
                    Return False
                End If

                'Scale v's between
                For iPred As Integer = 1 To Me.m_EPData.NumLiving
                    If Me.m_EPData.QB(iPred) > 0 Then
                        Dim vul As Single = (Me.m_EPData.TTLX(iPred) - 2) / (maxTL - 2) * (Upper - Lower) + Lower
                        If vul > 1 Then vul = CSng(10 * vul) / 10 Else vul = 0
                        For iPrey As Integer = 1 To Me.m_EPData.NumGroups
                            If Me.m_EPData.DC(iPred, iPrey) > 0 Then
                                Me.m_Data.VulMult(iPrey, iPred) = vul
                            End If
                        Next
                    End If
                Next
                Return True
            End If

            'ToDo: send error message
            Return False

        End Function

#End Region

#Region "Running the model"



        ''' <summary>
        ''' Overloaded RunModelValue() provided so that older search code will run without a major over haul
        ''' </summary>
        ''' <param name="NumberOfYears"></param>
        ''' <param name="totval"></param>
        ''' <param name="Employ"></param>
        ''' <param name="manvalue"></param>
        ''' <param name="ecovalue"></param>
        ''' <param name="frateopt"></param>
        ''' <param name="nopt"></param>
        ''' <remarks>Hopefully this is temporary once all the existing code is changed to use RunModelValue(NumberOfYears,frateopt(),nopt) this can be removed</remarks>
        Friend Sub RunModelValue(ByVal NumberOfYears As Integer, ByRef totval As Double, ByRef Employ As Double,
                       ByRef manvalue As Double, ByRef ecovalue As Double, ByRef frateopt() As Double, ByVal nopt As Integer)

            Me.RunModelValue(NumberOfYears, frateopt, nopt)

            totval = Me.m_search.totval
            Employ = Me.m_search.Employ
            manvalue = Me.m_search.manvalue
            ecovalue = Me.m_search.ecovalue

        End Sub

        Friend Sub RunModelValue(ByVal NumberOfYears As Integer, ByRef frateopt() As Double, ByVal nopt As Integer)
            'jb Differences between EwE5 and EwE6:
            'Removal of all npairs code. Adult/Juvenile pairs has been replaced with multi stanza life stages.
            'Removal of Imethod flag. EwE6 only uses the rk4 routine for numeric intergration AdamsBasforth()integration has been removed.
            'Removal of StepsPerMonth. EwE6 the user could set the number of time steps in a month this has been discontinued.
            'Removal of Integrate flag. All integration is handled by the rk4. Integration of individual groups is handled by the NoIntergrate(ngroups) flag only
            'All Search optimization calculations have been moved to the cSearchDataStructures e.g. totval, Employ.....

            Dim itime As Integer
            Dim i As Integer
            Dim ipct As Integer
            Dim j As Integer
            Dim t As Single
            Dim iyr As Integer, iyf As Integer
            Dim RelFopt() As Single, QGrowUsed() As Single
            Dim Fgear() As Single
            Dim ExtraTime As Integer = Me.m_search.ExtraYearsForSearch

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'For testing error handling
            'Throw New Exception("Ecosim test exception!")
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'QYear() Max catchability increase. Catchability increase over time due to improved fishing efficiency
            'Used to modify relQ()
            'Set to one for normal run has no effect on catchability (relQ)
            'For a managment strategy evalution MSE varied as a function of Qgrow (user input) in MSE.YearTimeStep()
            Dim QYear() As Single

            'Ecosim is about to be initialized for a run
            If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimPreRunInitialized(Me.m_Data)

            Me.InitializeStanzaVars()

            If Me.m_Data.bTimestepOutput Then
                Me.redimTime(NumberOfYears + ExtraTime, True)
                Me.m_Data.dimResults(NumberOfYears + ExtraTime)
            Else
                Me.m_Data.eraseResults()
            End If

            Me.AbortRun = False
            Me.m_Data.FirstTime = True

            'This pluginmanager case is being used by VC to get EcoBio modify the timeseries repetitively, then run Ecosim
            If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimModifyTimeseries(Me.m_RefData)

            Me.InitializeDataInfo()
            Me.m_Data.ClearSummaryResults()
            If Me.m_Data.PredictSimEffort Then Me.InitializeCapacity()

            ReDim Fgear(Me.m_EPData.NumFleet)
            ReDim QYear(Me.m_EPData.NumFleet)
            ReDim QGrowUsed(Me.m_EPData.NumFleet)
            ReDim RelFopt(nopt)

            'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
            '*
            If Me.m_search.bInSearch Then
                'NEEDS TO RECALCULATE BASECOST ETC
                Me.m_search.redimForRun()
                Me.m_search.bBaseYearSet = False

                ReDim Preserve Me.m_Data.FishRateNo(Me.m_EPData.NumGroups, 12 * (NumberOfYears + ExtraTime))
                ReDim Preserve Me.m_Data.FishRateGear(Me.m_EPData.NumFleet + 1, 12 * (NumberOfYears + ExtraTime))

                'If m_search.MaxEffort < 60 Then m_search.MaxEffort = 60
                For i = 1 To nopt
                    If frateopt(i) < Math.Log(Me.m_search.MaxEffort) Then
                        RelFopt(i) = CSng(Math.Exp(frateopt(i)))
                    Else
                        RelFopt(i) = Me.m_search.MaxEffort
                    End If
                Next

            End If 'If m_search.bDoSearch Then
            '*
            'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
            '*

            For i = 1 To Me.m_EPData.NumFleet : QYear(i) = 1 : Next
            Me.m_search.initForRun(Me.m_EPData, Me.m_Data)

            'calculate pbbiomass parameter from pbbase and pbm
            Me.Set_pbm_pbbiomass()

            'xxxxxxxxxxxxxxxxxxx
            'get initial derivative to define runge-kutta time step deltat
            Me.SetFishTimetoFish1()
            For i = 1 To Me.m_EPData.NumFleet
                Me.m_Data.FishRateGear(i, 0) = Me.m_Data.FishRateGear(i, 1)
            Next
            Me.CalcStartEatenOfBy()
            Me.InitialState()
            Me.SetTimeSteps()
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            Me.SetBBtoStartBiomass(Me.nvar)

            t = 0
            itime = 0
            Me.TimeNow = 1 : Me.YearNow = 1
            Me.m_search.Ecodistance = 0
            Me.m_search.ExistValue = 0

            Me.m_Data.FirstTime = True

            ' JS 08Jan10: moved NA calculations to Ecosim
            Me.EstimateTLofCatch(0)

            If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimRunInitialized(Me.m_Data)

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ' START OF TIME LOOP
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            For iyr = 1 To NumberOfYears + ExtraTime
                'Constrain the Ecosim year index to the run length passed in as the argument NumberOfYears
                iyf = Math.Min(iyr, NumberOfYears)

                'set Fgear() fishing effort at timestep that can be modified by a search routine
                Me.SetFGear(Fgear, RelFopt, QYear, iyr)

                'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
                '*
                Dim FSearch As Single
                If Me.m_search.bInSearch Then

                    'Calculates NetCost() 
                    Me.m_search.calcNetCost(Fgear, iyr)
                    Me.m_search.calcYearlySummaryValues(Me.BB)

                    If Me.m_search.SearchMode = eSearchModes.FishingPolicy Or Me.m_search.SearchMode = eSearchModes.MSE Then
                        Array.Clear(Me.m_search.FishYear, 0, Me.m_search.FishYear.Length)
                        'calculate fishing mortality if in Fishing policy or MSE
                        'used to overwrite FishRateNo() inside the month time loop
                        For iFlt As Integer = 1 To Me.m_EPData.NumFleet
                            For j = 1 To Me.m_EPData.NumGroups
                                'Don't include discards that survived  Propdiscardtime() does not include survivors
                                FSearch = Me.m_Data.relQ(iFlt, j) * (Me.m_Data.PropLandedTime(iFlt, j) + Me.m_Data.PropDiscardTime(iFlt, j))
                                Me.m_search.FishYear(j) += Fgear(iFlt) * FSearch * QYear(iFlt)
                                '********following line stops gear overwrite for cases where
                                'model has been fit to historical data by using species F forcing
                                'for years 1 to NYRDAT (policy impact allowed only for future years)
                                If Me.m_Data.FisForced(j) And iyr < Me.m_RefData.nYears Then Me.m_search.FishYear(j) = Me.m_Data.FishRateNo(j, 12 * iyr - 11)
                            Next j
                        Next iFlt
                    End If 'If m_search.SearchMode = eSearchModes.FishingPolicy Or m_search.SearchMode = eSearchModes.MSE Then

                End If
                '*
                'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------

                'xxxxxxxxxxxxxxxxxxxxxxxxx
                'START OF MONTHLY TIME LOOP
                'xxxxxxxxxxxxxxxxxxxxxxxx
                For ipct = 1 To 12

                    itime = itime + 1
                    Me.TimeNow = itime
                    Me.YearNow = iyr
                    Dim itt As Integer = Math.Min(itime, NumberOfYears * Me.StepsPerYear)

                    'set QMult() multiplier (density dependent catchability) as a function of the current biomass for this timestep
                    Me.setDenDepCatchMult(Me.BB)

                    Me.setForcedCatchabilities(itt, iyr, QYear)
                    Me.m_Data.SetRelQToT(itime, True)

                    If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimBeginTimeStep(Me.BB, Me.m_Data, itime)

                    Me.AccumulateDataInfo(ipct, itime, iyr, Me.BB, Me.m_Data.loss)
                    'If ipct = 6 Then AccumulateDataInfo(CInt(Math.Truncate(itime / 12)), BB, m_Data.loss)

                    Me.setEffortFromPlugin(QYear, itime, iyr)
                    'Set FishTime() (fishing mort at timestep)
                    Me.setFishTime(itime, iyr, QYear)

                    If Me.m_search.SearchMode = eSearchModes.MSE Then
                        'MSE Quota regulations
                        'Overwrites FishTime() set above in setFishTime()
                        Me.m_MSE.DoRegulations(Me.BB, Fgear, Me.Qmult, QYear, itime, ipct, iyr)
                    End If

                    If Me.m_Data.PredictSimEffort Then

                        'Predict Effort
                        Me.PredictCurrentEffort(itime)
                        If Me.m_search.SearchMode = eSearchModes.MSE Then
                            'if MSE is running then regulate the predicted Effort 
                            Me.m_MSE.RegulateEffort(Me.BB, Me.Qmult, QYear, itime, ipct)
                        End If
                        'set the F using the Predicted and Regulated (if MSE is running) effort
                        Me.SetFtimeFromGear(itime, QYear, True)

                        Me.FindCurrentProfit(Me.BB, itime, iyr)
                        Me.PredictCapacityChange()
                    End If

                    If Me.m_search.SearchMode = eSearchModes.FishingPolicy Then
                        For i = 1 To Me.m_EPData.NumFleet
                            If Me.m_search.FblockCode(i, iyf) > 0 Then
                                Me.m_Data.FishRateGear(i, itime) = Fgear(i)
                            End If
                        Next
                    End If

                    'Copy FishRateGear() for this time step into the zero time element for SimDetritus
                    For i = 1 To Me.m_Data.nGear : Me.m_Data.FishRateGear(i, 0) = Me.m_Data.FishRateGear(i, itime) : Next

                    'Set tval(nForcingShapes) to the forcing function values/multipliers for this timestep
                    Me.settval(itt)

                    Me.m_MSE.VaryForcing(Me.m_Data.tval)

                    'CJW email 04May00: in runmodel, m_refData.PoolForceBB loop has to be put in twice, 
                    'both before and after call to rk4 (otherwise Z calculation as loss/bb) 
                    'is wrong later in time step
                    Me.setForcedBiomass(itt, iyr)

                    Me.setForcedDiscards(itt, iyr, QYear)

                    Me.clearMonthlyStanzaVars()
                    For irk4 As Integer = 1 To Me.m_Data.StepsPerMonth

                        If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimSubTimestepBegin(Me.BB, t, Me.DeltaT, irk4, Me.m_Data)

                        'update stanza groups on the last iteration
                        Dim UpdateStanza As Boolean = (irk4 = Me.m_Data.StepsPerMonth)
                        Me.rk4(Me.BB, t, Me.DeltaT, itt, UpdateStanza)
                        t += Me.DeltaT
                        Me.setForcedBiomass(itt, iyr)

                        If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimSubTimestepEnd(Me.BB, t, Me.DeltaT, irk4, Me.m_Data)

                    Next irk4

                    If Me.AbortRun Then
                        'rk4() sent a message
                        'm_publisher.AddMessage(New cMessage("Ecosim run aborted.", eMessageType.ErrorEncountered, eCoreComponentType.EcoSim, eMessageImportance.Critical))
                        Exit Sub
                    End If

                    'AccumulateDataInfo(ipct, itime, iyf, BB, m_Data.loss)

                    Me.CheckIfSmall(1, Me.nvar, Me.BB)

                    'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
                    '*
                    If Me.m_search.bInSearch Then
                        Me.CalcCatchForSearch(itime, iyr, ipct, Me.BB, Fgear, QYear)
                    End If
                    '*
                    'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------

                    If Me.m_search.SearchMode = eSearchModes.MSE Then
                        Me.m_MSE.AccessBioRisk(Me.BB)
                    End If 'If m_search.PlotOn = True Then

                    ' JS 09Sep13: Can run TL calculations with searches upon request
                    If ((Me.m_search.SearchMode = eSearchModes.NotInSearch) Or (Me.m_Data.bAlwaysCalcTLc)) And Me.m_Data.bTimestepOutput Then
                        Me.EstimateTLs(itime)
                        Me.EstimateTLofCatch(itime)
                        Me.ComputeMoreStats(itime)
                    End If

                    'Compute time step results if the calling routine set bTimestepOutput to True
                    If Me.m_Data.bTimestepOutput Then
                        Me.PopulateResults(itime, ipct, iyr)
                        If Not Me.m_search.SearchMode = eSearchModes.FitToTimeSeries Then
                            Me.FireOnTimeStep(itime)
                        End If
                    End If

                    If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimEndTimeStep(Me.BB, Me.m_Data, itime, Me.m_Results)

                    If Me.bStopRunning Then Exit For

                Next ipct 'Month
                'xxxxxxxxxxxxxxxxxxxxxxxxx
                'END OF MONTHLY TIME LOOP
                'xxxxxxxxxxxxxxxxxxxxxxxx

                'perform assessment and update relative efficiency (m_search.Fwc) estimates if assessment is on
                If Me.m_search.SearchMode = eSearchModes.MSE Then
                    Me.m_MSE.AssessFs(Fgear, Me.BB)
                End If

                'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
                '*
                If Me.m_search.bInSearch And iyr = Me.m_search.BaseYear Then
                    'Ecosim has one spatial cell
                    Me.m_search.calcBaseYearCost(iyr, 1)
                End If
                '*
                'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------

                If Me.bStopRunning Then Exit For

            Next iyr 'Year
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'END OF TIME LOOP
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            If Me.m_Data.bTimestepOutput Then
                'summarize any data for output
                Me.m_Data.SummarizeResults(Me.m_EPData.CostPct, Me.m_search.Jobs)
            End If

            'VC080523 Subtracts baseyear from the calc's below as we only need to look from that point on
            'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------
            '*
            If Me.m_search.bInSearch Then
                'only go in here if searching
                Me.m_search.EcosimSummarizeIndicators(Me.biomeq, Fgear, NumberOfYears, NumberOfYears + ExtraTime - Me.m_search.BaseYear, Me.m_Data.PriceMedData)
            End If
            '*
            'Search--Search--Search--Search--Search--Search--Search--Search--Search--Search--Search----------

            Me.PlotDataInfo(False, Me.m_Data.SS, Me.m_Data.SSGroup)
            ' System.Console.WriteLine("Ecosim SS = " & m_Data.SS.ToString)

            If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimRunCompleted(Me.m_Data)

            'Set values back to defaults... StepsPerMonth = 1
            Me.m_Data.onEcosimRunCompleted()

            'TimeNow needs to be set back to the first timestep 
            'so it can be used by derivt() outside RunModelValue() and rk4()
            Me.TimeNow = 1 : Me.YearNow = 1

        End Sub

#End Region

#Region "Private modeling code"

        ''' <summary>
        ''' Calculates Ecosim yearly catch, sum of catch for year, for search data.
        ''' </summary>
        ''' <param name="iTime">Current time step</param>
        ''' <param name="iYear">Current year</param>
        ''' <param name="iMonth">Current month in year (12)</param>
        ''' <param name="biomass">Biomass of current time step</param>
        ''' <param name="Fgear">Effort set by searches</param>
        ''' <param name="QYear">Catchability increase for year</param>
        ''' <remarks>Uses <see cref="cEcosimModel.CalcLandings"> CalcCatch to compute catch.</see></remarks>
        Private Sub CalcCatchForSearch(ByVal iTime As Integer, ByVal iYear As Integer, ByVal iMonth As Integer, ByVal biomass() As Single, ByVal Fgear() As Single, ByVal QYear() As Single)
            Dim iflt As Integer
            Dim LandingsForValue(,) As Single
            If Not Me.m_search.bInSearch Then
                'Not in a search so just bump out
                Exit Sub
            End If

            If iMonth = 1 Then
                'start of a year clear out yearly data
                Me.m_search.ClearYearlyData()
            End If

            'MSE stores effort in FishRateGear(fleet,time) when applying harvest rule (TFM, TAC...)
            'all other searches store effort in Fgear()
            If Me.m_search.SearchMode = eSearchModes.MSE Then

                If Me.m_MSEData.UseQuotaRegs Then
                    'Populate Fgear(fleet) with effort set by MSE for the harvest rule
                    For iflt = 1 To Me.m_Data.nGear
                        Fgear(iflt) = Me.m_Data.FishRateGear(iflt, iTime)
                    Next

                End If 'Me.m_MSEData.UseQuotaRegs 
            End If 'Me.m_search.SearchMode = eSearchModes.MSE 

            ReDim LandingsForValue(Me.m_Data.nGroups, Me.m_Data.nGear)
            'Calculate the catch
            Dim landings As Single
            For igrp As Integer = 1 To Me.m_EPData.NumLiving
                For iflt = 1 To Me.m_Data.nGear

                    ' JS 210308: used correct number of steps per year here

                    'landings calculated by ecosim
                    'discards are not included
                    landings = Me.CalcLandings(igrp, iflt, biomass(igrp), Fgear(iflt), Me.m_Data.relQ(iflt, igrp), QYear(iflt)) / Me.m_Data.NumStepsPerYear
                    'sum into yearly value for searches
                    Me.m_search.CatchYear(iflt, igrp) += landings
                    Me.m_search.CatchYearGroup(igrp) += landings

                    'Price Elasticity function are based on the yearly Ecopath Landings
                    'Searches use average annual value so they sum the monthly values
                    'so we need to convert this back to Ecopath yearly values for the Price Elasticity to return the correct value
                    LandingsForValue(igrp, iflt) += landings * Me.m_Data.NumStepsPerYear
                Next
            Next


            'Now get Value of the landings 
            If iYear > Me.m_search.BaseYear Then
                'Value of catch for the search includes discount factor

                'set PriceMedData.MedVal() as a function of landings(supply) for all applied price elasticity functions
                Me.m_Data.PriceMedData.SetPriceMedFunctions(LandingsForValue)

                'now get the value for all Group, Fleets based on the price elasticity function
                For igrp As Integer = 1 To Me.nGroups
                    For iflt = 1 To Me.m_Data.nGear
                        'Monthly Value = Landings * [mediated price] * [Discount Factor]
                        Me.m_search.ValCatch(iflt, igrp) += LandingsForValue(igrp, iflt) * Me.MarketValue(igrp, iflt, iTime, iYear) * Me.m_search.DF / 12.0F

                    Next
                Next

            End If

        End Sub

        ''' <summary>
        ''' Return Landings for the Group, Fleet, Biomass, Effort, Q, QYear and Density dependant catchability
        ''' </summary>
        ''' <param name="iGrp">Group index</param>
        ''' <param name="iFlt">Fleet index</param>
        ''' <param name="B">Biomass of the group</param>
        ''' <param name="Effort">Effort</param>
        ''' <param name="Qrate">Catchability rate</param>
        ''' <param name="QYear">Catchability multiplier used for increase in catchability over time (QYear())</param>
        ''' <returns>Landings</returns>
        ''' <remarks>Qmult() must be set via <see cref="cEcosimModel.setDenDepCatchMult">setDenDepCatchMult</see> before calling CalcCatch(). </remarks>
        Public Function CalcLandings(ByVal iGrp As Integer, ByVal iFlt As Integer,
                                    ByVal B As Single, ByVal Effort As Single, ByVal Qrate As Single,
                                     ByVal QYear As Single) As Single

            Return B * Qrate * Effort * Me.Qmult(iGrp) * Me.m_Data.PropLandedTime(iFlt, iGrp) * QYear

        End Function

        ''' <summary>
        ''' Calculate the Price Elasticity value of the landings from Ecosim Results for a time step
        ''' </summary>
        ''' <param name="iTime"></param>
        ''' <remarks></remarks>
        Public Sub CalcValueFromLandings(ByVal iTime As Integer, ByVal iYear As Integer)
            Dim baseGroupVal As Single

            'set PriceMedData.MedVal() for all applied price elasticity functions
            'using landings at the current time step Me.m_Data.ResultsLandings(groups,fleets)
            Me.m_Data.PriceMedData.SetPriceMedFunctions(Me.m_Data.ResultsLandings)

            'now get the value for all Group, Fleets based on the price elasticity function
            For igrp As Integer = 1 To Me.nGroups
                baseGroupVal = 0
                For iflt As Integer = 1 To Me.m_Data.nGear
                    'Landings are the "Ecopath" landings (discards not included) which is the annual landings
                    Dim value As Single = Me.m_Data.ResultsLandings(igrp, iflt) * Me.MarketValue(igrp, iflt, iTime, iYear)

                    Me.m_Data.ResultsSumValueByGroupGear(igrp, iflt, iTime) += value
                    Me.m_Data.ResultsSumValueByGear(iflt, iTime) += value

                    'sum all value into the zero fleet index
                    Me.m_Data.ResultsSumValueByGroupGear(igrp, 0, iTime) += value
                    Me.m_Data.ResultsSumValueByGear(0, iTime) += value

                    'Ecopath value of this group for relative value
                    baseGroupVal += Me.m_EPData.Landing(iflt, igrp) * Me.m_EPData.Market(iflt, igrp)
                Next
                If baseGroupVal > 0 Then
                    'Zero index contains the sum of landing across all fleets
                    Me.m_Data.ResultsSumRelValueByGroup(igrp, iTime) = Me.m_Data.ResultsSumValueByGroupGear(igrp, 0, iTime) / baseGroupVal
                End If
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the Ecopath market value (off vessel price) as a function of 
        ''' time series forcing and any applied price elasticity function.
        ''' </summary>
        ''' <param name="iGrp">One-based index of the affected group. This is the group 
        ''' that the price function is applied to in the application grid.</param>
        ''' <param name="iFlt">One-based index of the affected fleet. This is the fleet 
        ''' that the price function is applied to in the application grid.</param>
        ''' <param name="iTime">Cummulative time step.</param>
        ''' <param name="iYear">Current year.</param>
        ''' <returns>([Ecopath market value] or [OffVesselPrice time series]) * [PES multiplier]</returns>
        ''' <remarks>
        ''' <para>Any <see cref="eTimeSeriesType.OffVesselPrice">absolute landings time series</see> are 
        ''' used if provided. if not, Ecopath base off-vessel prices are multiplied with 
        ''' <see cref="eTimeSeriesType.OffVesselPriceRel">relative time series</see> 
        ''' and applied price elasticity functions.</para>
        ''' <para>Note that <see cref="eTimeSeriesType.OffVesselPriceRel">relative time series</see>
        ''' can vary prices for all caught groups by the same magnitude by specifying group code 0.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function MarketValue(ByVal iGrp As Integer, ByVal iFlt As Integer, ByVal iTime As Integer, ByVal iYear As Integer) As Single

            Dim value As Single = Me.m_EPData.Market(iFlt, iGrp)
            Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iTime, iYear)
            If Me.m_RefData.PoolForceOffVesselPriceAbs(iFlt, iGrp, iForcing) > 0 Then
                Return Me.m_RefData.PoolForceOffVesselPriceAbs(iFlt, iGrp, iForcing)
            End If

            Dim f As Single = Me.m_RefData.PoolForceOffVesselPriceRel(iFlt, iGrp, iForcing)
            If (f <= 0) Then f = Me.m_RefData.PoolForceOffVesselPriceRel(iFlt, 0, iForcing)
            If f > 0 Then
                value = value * f
            End If

            'apply the price elasticity multiplier to market value for this Group/Fleet
            'if there is no PES function for this group fleet getPESMult(group,fleet) will return 1
            Return value * Me.m_Data.PriceMedData.getPESMult(iGrp, iFlt)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the Ecopath effort cost value as a function of time series forcing.
        ''' </summary>
        ''' <param name="iFlt">One-based index of the fleet.</param>
        ''' <param name="iTime">Cummulative time step.</param>
        ''' <param name="iYear">Current year.</param>
        ''' <returns>Ecopath effort cost value as a function of time series forcing</returns>
        ''' <remarks>Any <see cref="eTimeSeriesType.EffortCost">absolute landings time series</see> are 
        ''' used if provided. if not, Ecopath base effort costs are multiplied with 
        ''' <see cref="eTimeSeriesType.EffortCostRel">relative time series</see>.</remarks>
        ''' -------------------------------------------------------------------
        Public Function EffortCost(ByVal iFlt As Integer, ByVal iTime As Integer, ByVal iYear As Integer) As Single

            Dim value As Single = Me.m_EPData.cost(iFlt, eCostIndex.CUPE)
            Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iTime, iYear)
            If Me.m_RefData.PoolForceEffortCostAbs(iFlt, iForcing) > 0 Then
                value = Me.m_RefData.PoolForceEffortCostAbs(iFlt, iForcing)
            ElseIf Me.m_RefData.PoolForceEffortCostRel(iFlt, iForcing) > 0 Then
                value *= Me.m_RefData.PoolForceEffortCostRel(iFlt, iForcing)
            End If
            Return value

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the Ecopath sail cost value as a function of time series forcing.
        ''' </summary>
        ''' <param name="iFlt">One-based index of the fleet.</param>
        ''' <param name="iTime">Cummulative time step.</param>
        ''' <param name="iYear">Current year.</param>
        ''' <returns>Ecopath sail cost value as a function of time series forcing</returns>
        ''' <remarks>Any <see cref="eTimeSeriesType.SailCost">absolute landings time series</see> are 
        ''' used if provided. if not, Ecopath base sail costs are multiplied with 
        ''' <see cref="eTimeSeriesType.SailCostRel">relative time series</see>.</remarks>
        ''' -------------------------------------------------------------------
        Public Function SailCost(ByVal iFlt As Integer, ByVal iTime As Integer, ByVal iYear As Integer) As Single

            Dim value As Single = Me.m_EPData.cost(iFlt, eCostIndex.Sail)
            Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iTime, iYear)
            If Me.m_RefData.PoolForceSailCostAbs(iFlt, iForcing) > 0 Then
                value = Me.m_RefData.PoolForceSailCostAbs(iFlt, iForcing)
            ElseIf Me.m_RefData.PoolForceSailCostRel(iFlt, iForcing) > 0 Then
                value *= Me.m_RefData.PoolForceSailCostRel(iFlt, iForcing)
            End If
            Return value

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return the Ecopath fixed cost value as a function of time series forcing.
        ''' </summary>
        ''' <param name="iFlt">One-based index of the fleet.</param>
        ''' <param name="iTime">Cummulative time step.</param>
        ''' <param name="iYear">Current year.</param>
        ''' <returns>Ecopath fixed cost value as a function of time series forcing</returns>
        ''' <remarks>Any <see cref="eTimeSeriesType.FixedCost">absolute landings time series</see> are 
        ''' used if provided. if not, Ecopath base fixed costs are multiplied with 
        ''' <see cref="eTimeSeriesType.FixedCost">relative time series</see>.</remarks>
        ''' -------------------------------------------------------------------
        Public Function FixedCost(ByVal iFlt As Integer, ByVal iTime As Integer, ByVal iYear As Integer) As Single

            Dim value As Single = Me.m_EPData.cost(iFlt, eCostIndex.Fixed)
            Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iTime, iYear)
            If Me.m_RefData.PoolForceSailCostAbs(iFlt, iForcing) > 0 Then
                value = Me.m_RefData.PoolForceSailCostAbs(iFlt, iForcing)
            ElseIf Me.m_RefData.PoolForceSailCostRel(iFlt, iForcing) > 0 Then
                value *= Me.m_RefData.PoolForceSailCostRel(iFlt, iForcing)
            End If
            Return value

        End Function


        Private Sub setForcedBiomass(ByVal iModelTimeStep As Integer, iYear As Integer)
            Dim iGrp As Integer
            'Get the correct forcing data timestep for this model time step
            Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iModelTimeStep, iYear)

            For iGrp = 1 To Me.m_EPData.NumGroups
                Me.ResetPred(iGrp) = False
            Next

            If iForcing <= Me.m_RefData.AppliedDatPoints Then  'Force the biomass if such a dataseries exists
                For iGrp = 1 To Me.m_EPData.NumGroups
                    If Me.m_RefData.PoolForceBB(iGrp, iForcing) > 0 Then
                        Me.ResetPred(iGrp) = True
                        Me.BB(iGrp) = Me.m_RefData.PoolForceBB(iGrp, iForcing)
                    End If
                Next
            End If 'If iyr <= m_refData.NdatYear Then

        End Sub


        ''' <summary>
        ''' Set FishTime(Group) total fishing mortality used by Ecosim in Deritt()
        ''' </summary>
        ''' <param name="iTime">Index of the current time step.</param>
        '''  <param name="iYear">Index of the current year.</param>
        ''' <remarks>
        '''  If runnning in a regular mode (no search) then use FishRateNo(ngroups,ntime) * density dependant catchability, if forced catch or F (fishing mortality rates) then don't use density dependance.
        '''  If in a FP search use FishYear() to set FishRateNo(ngroups,ntime)  and FishTime(ngroups). 
        '''  MSE sets FishRateNo(ngroups,ntime) in <see cref="cMSE.DoRegulations">DoRegulations()</see>
        ''' </remarks>
        Private Sub setFishTime(ByVal iTime As Integer, ByVal iYear As Integer, QYear() As Single)

            If Me.m_search.SearchMode = eSearchModes.FishingPolicy Then
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Fishing Policy 
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                For igrp As Integer = 1 To Me.nvar
                    'overwrite FishRateNo() with computed mortality rates (FishYear(group)) if in Fishing Policy 
                    'see EwE5 RunModelValue()
                    Me.m_Data.FishRateNo(igrp, iTime) = Me.m_search.FishYear(igrp) 'fish year was computed or set to FishRateNo(j, 12 * iyr - 11) if FisForced() = True (forcing data loaded)
                    Me.m_Data.FishTime(igrp) = Me.m_Data.FishRateNo(igrp, iTime) * Me.Qmult(igrp)

                Next igrp

            Else



                'Get the correct forcing data timestep for this model time step
                Dim iForcing As Integer = Me.m_RefData.toForcingTimeStep(iTime, iYear)
                'is this group timestep forced
                Dim isForced As Boolean = False
                'xxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Normal run
                'xxxxxxxxxxxxxxxxxxxxxxxxxxx
                'FishRateNo(group,time) fishing mortality at the current effort 
                'Qmult(group) density dependent catchability at the current biomass set in setDenDepCatchMult()
                For iGrp As Integer = 1 To Me.nvar

                    ''Is this group forced in some way
                    'isForced = (Me.m_RefData.isTimeStepValid(iForcing)) And ((Me.m_RefData.PoolForceCatch(iGrp, iForcing) > 0) Or Me.m_Data.FisForced(iGrp))

                    'Is this group forced in some way
                    isForced = (Me.m_RefData.isTimeStepValid(iForcing)) And ((Me.m_RefData.PoolForceCatch(iGrp, iForcing) > 0) Or Me.m_RefData.ForcedFs(iGrp, iTime) >= 0)

                    'If (Not Me.m_RefData.isTimeStepValid(iTime)) And Me.m_Data.FisForced(iGrp) Then
                    '    ' 
                    '    'boo uaaa
                    '    Me.m_Data.FisForced(iGrp) = False
                    '    isForced = False
                    '    Me.SetFtimeFromGear(BB, iTime, QYear, False)
                    '    Me.m_Data.FisForced(iGrp) = True

                    'End If


                    If isForced Then
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        'WARNING: If both forced F's and forced Catches are loaded
                        'Forced Catches will take precedents
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        'YES Forced don't included Density Dependant Catchability on any type of forced F
                        ' If Me.m_Data.FisForced(iGrp) Then
                        If Me.m_RefData.ForcedFs(iGrp, iTime) >= 0 Then
                            'Forced F
                            Me.m_Data.FishTime(iGrp) = Me.m_RefData.ForcedFs(iGrp, iTime)
                            Me.m_Data.FishRateNo(iGrp, iTime) = Me.m_RefData.ForcedFs(iGrp, iTime)
                        End If

                        'If Forced Catches are loaded for this time step overwrite Forced F's
                        If Me.m_RefData.PoolForceCatch(iGrp, iForcing) > 0 Then
                            'Forced Catches
                            Me.m_Data.FishTime(iGrp) = Me.m_RefData.PoolForceCatch(iGrp, iForcing) / Me.BB(iGrp)
                            'VC Sep 2008. Based on discussion with CJW, we'll cap the FishTime to be a logistic function 
                            'Carl suggests the following:
                            'predict F from
                            'F=w*Fmax+(1-w)C/B
                            'Where the �weight� w is given by the logistic function, and K = 5
                            'w = 1 / (1 + exp(-K * (C / B - Fmax)))
                            'Fmax is the Flimit from the search
                            'Dim w As Double = 1 / (1 + Math.Exp(-5 * (CBratio - FLimit(iGrp))))
                            'FishTime(iGrp) = w * FLimit(iGrp) + (1 - w) * CBratio

                            'The next is easier, and gives almost the same answer:
                            If Me.m_Data.FishTime(iGrp) > 3 Then Me.m_Data.FishTime(iGrp) = 3

                        End If

                    Else
                        'NOT Forced
                        'Include density dependant catchability
                        'FishRateNo() is fishing mortality at the current effort by group,time

                        'Debug.Assert(iGrp <> 17 And iTime < 601)
                        Me.m_Data.FishTime(iGrp) = Me.m_Data.FishRateNo(iGrp, iTime) * Me.Qmult(iGrp)
                    End If

                    'PoolForceZ(iGroup,0) is used in Derivt() to force mortality
                    If iForcing <= Me.m_RefData.nYears And Me.m_RefData.PoolForceZ(iGrp, iForcing) > 0 Then
                        Me.m_RefData.PoolForceZ(iGrp, 0) = Me.m_RefData.PoolForceZ(iGrp, iForcing)
                    End If

                Next iGrp

            End If

        End Sub


        Private Sub setForcedDiscards(ByVal iModelTimeStep As Integer, iYear As Integer, QYear() As Single)
            Dim bForced As Boolean = False
            Dim bFChanged As Boolean = False
            Dim totCatch As Single

            Dim iForcedTime As Integer = Me.m_RefData.toForcingTimeStep(iModelTimeStep, iYear)

            For igrp As Integer = 1 To Me.m_Data.nGroups
                For iflt As Integer = 1 To Me.m_Data.nGear

                    Dim discardmortForced As Single = Me.m_RefData.PoolForceDiscardMort(iflt, igrp, iForcedTime)
                    If (discardmortForced < 0) Then discardmortForced = Me.m_RefData.PoolForceDiscardMort(iflt, 0, iForcedTime)

                    If discardmortForced >= 0.0 Then
                        'Discard Mortality has changed
                        'Save the discard mortality rate for this timestep
                        Me.m_Data.PropDiscardMortTime(iflt, igrp) = discardmortForced
                        'Propdiscardtime() does NOT include discards that survived
                        Me.m_Data.PropDiscardTime(iflt, igrp) = (1 - Me.m_Data.PropLandedTime(iflt, igrp)) * Me.m_Data.PropDiscardMortTime(iflt, igrp)

                        bFChanged = True
                        bForced = True
                    End If

                    Dim discardprop As Single = Me.m_RefData.PoolForceDiscardProp(iflt, igrp, iForcedTime)
                    If (discardprop < 0) Then discardprop = Me.m_RefData.PoolForceDiscardProp(iflt, 0, iForcedTime)

                    If discardprop >= 0.0 Then
                        'Propdiscardtime does not include discards that survived
                        Me.m_Data.PropDiscardTime(iflt, igrp) = discardprop * Me.m_Data.PropDiscardMortTime(iflt, igrp)
                        Me.m_Data.PropLandedTime(iflt, igrp) = 1 - discardprop

                        bForced = True
                        bFChanged = True
                    End If

                    If bFChanged Then
                        Debug.Assert((Me.m_Data.PropLandedTime(iflt, igrp) + Me.m_Data.PropDiscardTime(iflt, igrp)) <= 1.0001, "Opps cEcosimModel.setForcedDiscards() may have calculated an incorrect PropLandedTime() or Propdiscardtime()")
                        'FishMGear() only contains catch that incure mortality
                        'Changing the discard mortality rate changes F
                        'Changing the proportion of landings and discards changes F if discard mort rate is not 1
                        'Calulate the new F from base values 
                        totCatch = (Me.m_EPData.Landing(iflt, igrp) + Me.m_EPData.Discard(iflt, igrp)) * (Me.m_Data.PropLandedTime(iflt, igrp) + Me.m_Data.PropDiscardTime(iflt, igrp))
                        'Debug.Assert(Me.m_Data.FishMGear(iflt, igrp) = (totCatch / Me.m_EPData.B(igrp)))
                        Me.m_Data.FishMGear(iflt, igrp) = totCatch / Me.m_EPData.B(igrp)


                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        'for debugging
                        'FishMGear should equal relQ * [proportion of catch mort] 
                        'debugTestRelQFishMGear will test this assumption
                        'FishMGear() = relQ() * (PropLandedTime() + Propdiscardtime())
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        bFChanged = False
                    End If 'bFChanged

                Next iflt
            Next igrp

            If bForced Then
                Me.SetFtimeFromGear(iModelTimeStep, QYear, PredEffort:=False, ForcedDiscards:=True)
                'Debugging check that FishMGear and relQ are still in sync
                'Me.debugTestRelQFishMGear()
            End If

        End Sub

        Private Sub setForcedCatchabilities(ByVal iModelTimeStep As Integer, iYear As Integer, QYear() As Single)

            Dim bForced As Boolean = False
            Dim iForcedTime As Integer = Me.m_RefData.toForcingTimeStep(iModelTimeStep, iYear)

            For igrp As Integer = 1 To Me.m_Data.nGroups
                For iflt As Integer = 1 To Me.m_Data.nGear
                    ' JS: 0 is not a valid value (yet)
                    If Me.m_RefData.PoolForceCatchabilities(iflt, igrp, iForcedTime) > 0.0 Then
                        Me.m_Data.relQt(iflt, igrp, iModelTimeStep) = Me.m_RefData.PoolForceCatchabilities(iflt, igrp, iForcedTime)
                        bForced = True
                    End If
                Next iflt
            Next igrp

            If bForced Then
                'Me.SetFtimeFromGear(iModelTimeStep, QYear, False)
            End If

        End Sub

        Private Sub setEffortFromPlugin(ByVal QYear() As Single, ByVal iTime As Integer, ByVal iYear As Integer)
            Dim bModified As Boolean
            Dim effort(Me.m_Data.nGear) As Single
            Try

                For iflt As Integer = 1 To Me.m_Data.nGear
                    effort(iflt) = Me.m_Data.FishRateGear(iflt, iTime)
                Next

                If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimModifyEffort(bModified, effort, Me.BB, iTime, iYear, Me.m_Data)

                If bModified Then
                    'Ok the plugin modified Effort for this time step
                    'so set Effort in the core data 
                    For iflt As Integer = 1 To Me.m_Data.nGear
                        Me.m_Data.FishRateGear(iflt, iTime) = effort(iflt)
                    Next

                    'And update F from Effort
                    Me.SetFtimeFromGear(iTime, QYear, Me.m_Data.PredictSimEffort)

                End If

            Catch ex As Exception
                cLog.Write(ex)
            End Try

        End Sub


        Private Sub CheckIfSmall(ByVal Start As Integer, ByVal last As Integer, ByRef ArrayX() As Single)
            Dim i As Integer
            For i = Start To last
                If Single.IsNaN(ArrayX(i)) Then ArrayX(i) = 1.0E-20
                If ArrayX(i) < 1.0E-20 Then ArrayX(i) = 1.0E-20
            Next
        End Sub


        ''' <summary>
        ''' Set fishing effort to a value supplied by a search routine or FishRateGear()
        ''' </summary>
        ''' <param name="Fgear">Fishing Effort</param>
        ''' <param name="RelFopt"></param>
        ''' <param name="iYear"></param>
        ''' <remarks>At the start of a year Fgear(nFleets)(fishing effort) is set to a user entered value FishRateGear(nFleets,nTime) or effort set by a search. 
        ''' If in a search Fgear() is then used to compute FishYear() which is used to set FishRateNo() and compute FishTime(). FishTime() is what is used by Derivt() Confused yet???? </remarks>
        Public Sub SetFGear(ByRef Fgear() As Single, ByVal RelFopt() As Single, ByRef QYear() As Single, ByVal iYear As Integer)


            'Constrain the year index to the Ecosim run length
            'When run for the Fishing Policy Search Ecosim is run past the end of the run length
            'This makes sure the iYear index is not out of bounds
            Dim iyf As Integer = CInt(If(iYear <= Me.m_Data.NumYears, iYear, Me.m_Data.NumYears))

            'Set Fgear() to values entered by the user
            For iFlt As Integer = 1 To Me.m_EPData.NumFleet
                Fgear(iFlt) = Me.m_Data.FishRateGear(iFlt, 12 * iyf - 11)
            Next iFlt


            'If in some kind of a search overwrite the Fgear() with the values from the search routine
            If Me.m_search.SearchMode = eSearchModes.FishingPolicy Then
                'xxxxxxxxxxxxxxxx
                'Fishing Policy search
                'xxxxxxxxxxxxxxxxxxxxxx

                'get the Fgear values set by the optimization routine
                Me.m_search.SetFGear(Fgear, RelFopt, iYear)

            ElseIf Me.m_search.SearchMode = eSearchModes.MSE Then
                'xxxxxxxxxxxxxxxxxxxxxx
                'MSE
                'xxxxxxxxxxxxxxxxxxxxxxx
                'vary Fgear(nFleets) and Qyear(nFleets)
                Me.m_MSE.VaryEffortCatchability(Fgear, QYear, iYear)

            End If

            ' Invoke FGear override plug-in point
            If (Me.m_pluginManager IsNot Nothing) Then Me.m_pluginManager.EcosimModifyFGear(Fgear, Me.BB, Me.m_Data, Me.TimeNow)

        End Sub



        Friend Sub rk4(ByRef B() As Single, ByRef t As Single, ByRef DeltaT As Single, iTimeStepIndex As Integer, ByVal UpdateStanzaGroups As Boolean)
            'this version taken from CJW's simII 290597 vc
            'runge-kutta integration from Press et al 1992 ed p 707
            'jb the runge-kutta integration method looks like it came directly from Numerical Recipies in C
            Dim dh As Single, d6 As Single, th As Single
            Dim LossSt(Me.m_Data.nGroups) As Single
            Dim i As Integer

            '090905VC&JB: discussion with Carl:
            'can we run Ecosim at different time scales? Joe and I are looking at the code, 
            'thinking of running multistanza once per month, but updating feeding calculations at finer time scales. 
            'No changes in interfaces if this can be done, only accessible from code
            'Carl: "Certainly. �You need to pass the mean rate over the month to the multistanza growth equation, 
            'likewise mean (or total) mortality rate to the multistanza survival equation. �
            'Note the mortality rates caused on prey by the multistanza group will be approximated by rate at start of the month, 
            'not updated during month. �Otherwise, all continuous variables can be upated with shorter time step. �
            'Watch out for very fast pools (very high PB), note these are not updated but modeled as tracking equiilibrium."
            '[05/09/09 12:00:14 PM]  But as i told Beth Thursday AM, i do not think that is the right approach. �
            'Instead, would take two steps: (1) fix the ERSEM/GTM state initialization using Ecopath-type calculation, 
            'so don't get initial instability; and (2) do all the fast variables only in the 
            'ERSEM/GTM model (phyto, zoops, detritus), pass only the monthly mean biomasses to Ecosim 
            'and pass back only the monthly mean mortality rates and fluxes to detritus from the Ecosim pools. �
            'Joe has added array in Derivt to calculate all mortality rate components (Mij), so can pass just the correct ones to ERSEM/GTM.
            '[05/09/09 11:57:59 AM] Carl Walters: Also, we already calculate in global variable ToDetritus the flows 
            'from ecopath pools to detritus, just need to make sure only the flows are used in ERSEM/GTM 
            'that are from pools represented only in Ecosim.
            'Note you should not be afraid to demand that the ERSEM/GTM models 
            'be modified to add fast dynamic pools where/when we have identified such pools as 
            'important in Ecopath but they have been overlooked or omitted for some reason in the ERSEM/GTM models. �
            'That would make a clean split: fast variables all done in the detailed models, slower variables all done in Ecosim.
            'Push hard for what i just recommended.  
            'Avoid changing Ecosim time step (not needed for our important variables), 
            'instead change the linkage variables and add fast variables as needed into the GTM models

            dh = CSng(DeltaT / 2.0)
            d6 = CSng(DeltaT / 6.0)
            th = t + dh

            Me.Derivt(t, B, Me.dydx, iTimeStepIndex)

            '  cLog.WriteArrayToFile("dydx EwE6.csv", dydx, t.ToString)

            If Me.m_TracerData.EcoSimConSimOn = True Then
                Me.m_ConTracer.loss = Me.m_Data.loss 'Ecosim loss computed in Derivt
                Me.m_ConTracer.Cupdate(B)
            End If

            For i = 1 To Me.nGroups
                LossSt(i) = Me.m_Data.loss(i)

                'averages use to update Ftime() at end of month
                Me.EatenByAvg(i) += Me.m_Data.Eatenby(i)
                Me.EatenOfAvg(i) += Me.m_Data.Eatenof(i)
                Me.PredAvg(i) += Me.m_Data.pred(i)
            Next

            For i = 1 To Me.nvar
                If Me.m_Data.NoIntegrate(i) <> 0 Then 'b(nGroups) b(nGroups-1)
                    Me.yt(i) = B(i) + dh * Me.dydx(i)
                ElseIf Me.m_Data.NoIntegrate(i) = 0 Then
                    Me.yt(i) = (1 - Me.m_Data.SorWt) * Me.biomeq(i) + Me.m_Data.SorWt * B(i)
                End If
            Next
            'yt is new biomass
            Me.Derivt(th, Me.yt, Me.dyt, iTimeStepIndex)

            For i = 1 To Me.nvar
                If Me.m_Data.NoIntegrate(i) <> 0 Then
                    Me.yt(i) = B(i) + Me.dyt(i) * dh
                ElseIf Me.m_Data.NoIntegrate(i) = 0 Then
                    Me.yt(i) = (1 - Me.m_Data.SorWt) * Me.biomeq(i) + Me.m_Data.SorWt * B(i)
                End If
            Next

            Me.Derivt(th, Me.yt, Me.dym, iTimeStepIndex)

            For i = 1 To Me.nvar
                If Me.m_Data.NoIntegrate(i) <> 0 Then
                    Me.yt(i) = B(i) + DeltaT * Me.dym(i)
                    Me.dym(i) = Me.dyt(i) + Me.dym(i)
                ElseIf Me.m_Data.NoIntegrate(i) = 0 Then
                    Me.yt(i) = (1 - Me.m_Data.SorWt) * Me.biomeq(i) + Me.m_Data.SorWt * B(i)
                    If Me.yt(i) < 0 Then
                        Me.m_publisher.SendMessage(New cMessage("Error in Runge Kutta.", eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Critical))
                        Debug.Assert(False, "yt(i) < 0 for i=" & i.ToString & ", t=" & t.ToString)
                        Me.AbortRun = True
                        Exit Sub
                    End If

                End If
            Next

            Me.Derivt(t + DeltaT, Me.yt, Me.dyt, iTimeStepIndex)

            For i = 1 To Me.nGroups 'N
                'jb set loss to the loss after the first call the Derivt()
                'loss is then used by SplitUpdate()
                Me.m_Data.loss(i) = LossSt(i)

                If Me.m_Data.NoIntegrate(i) > 0 Then
                    If Me.m_Data.NoIntegrate(i) = i Then 'pool not split case
                        B(i) = B(i) + d6 * (Me.dydx(i) + Me.dyt(i) + 2 * Me.dym(i))
                    End If
                ElseIf Me.m_Data.NoIntegrate(i) = 0 Then
                    B(i) = (1 - Me.m_Data.SorWt) * Me.biomeq(i) + Me.m_Data.SorWt * B(i)
                End If
            Next

            'Are we running with more than 12 timesteps per year
            If Me.m_Data.StepsPerMonth > 1 Then
                'Yes call Derivt() to recompute mPred() with biomass at end of timestep
                Me.Derivt(t + DeltaT, B, Me.dydx, iTimeStepIndex)
                'reset loss to start value for SplitUpdate() after Derivt()
                For i = 1 To Me.nGroups : Me.m_Data.loss(i) = LossSt(i) : Next
            End If

            'average biomass and loss to a monthly value for SplitUpdate
            Me.avgMonthlyStanzaVars(B, UpdateStanzaGroups)

            'only update biomass for stanza groups and feeding time (Ftime()) on the last sub timestep
            If UpdateStanzaGroups Then

                'SplitUpdate(,,) updates Biomass of the current timestep via the last B(ngroups) argument
                Me.SplitUpdate(Me.BBAvg, Me.LossAvg, B)

                For i = 1 To Me.nGroups 'N

                    'Feeding time update at the end of the month using the monthly average values
                    If (Me.m_Data.NoIntegrate(i) = i Or Me.m_Data.NoIntegrate(i) < 0) Then
                        Me.CBlast(i) = Me.EatenByAvg(i) / Me.PredAvg(i)
                    End If

                    Me.RiskRate(i) = CSng(Me.EatenOfAvg(i) / Me.BBAvg(i) + Me.m_Data.mo(i) + 0.0000000001)
                    Me.Qopt(i) = Me.m_Data.Qmain(i) + Me.m_Data.Qrisk(i) / Me.RiskRate(i)

                    If Me.CBlast(i) > 0 And (Me.m_Data.NoIntegrate(i) = i Or Me.m_Data.NoIntegrate(i) < 0) Then
                        '20150126, Arrow lake model, big increase in prey, top predator foraging time can't go below 0.1, 
                        'so changed bount to 0.01 
                        Me.m_Data.Ftime(i) = CSng(Me.m_Data.ForagingTimeLowerLimit + (1 - Me.m_Data.ForagingTimeLowerLimit) * Me.m_Data.Ftime(i) * (1 - Me.m_Data.FtimeAdjust(i) + Me.m_Data.FtimeAdjust(i) * Me.Qopt(i) / Me.CBlast(i)))
                    End If
                    If Me.m_Data.Ftime(i) > Me.m_Data.FtimeMax(i) Then Me.m_Data.Ftime(i) = Me.m_Data.FtimeMax(i)
                Next

            End If

            'System.Console.WriteLine("--------------Detritus----------------")
            'System.Console.Write("T=" + t.ToString)
            'For idet As Integer = m_EPData.NumLiving + 1 To m_EPData.NumGroups
            '    System.Console.Write("," + m_EPData.GroupName(idet) + "," + B(idet).ToString)
            'Next
            'System.Console.WriteLine()

        End Sub

        Friend Sub InitializeStanzaVars()
            Array.Clear(Me.m_stanza.RecStanzaScalar, 0, Me.m_stanza.Nsplit)
        End Sub

        Friend Sub clearMonthlyStanzaVars()
            Array.Clear(Me.BBAvg, 0, Me.nGroups + 1)
            Array.Clear(Me.LossAvg, 0, Me.nGroups + 1)

            Array.Clear(Me.EatenByAvg, 0, Me.nGroups + 1)
            Array.Clear(Me.EatenOfAvg, 0, Me.nGroups + 1)
            Array.Clear(Me.PredAvg, 0, Me.nGroups + 1)
        End Sub

        ''' <summary>
        ''' Average biomass and loss from sub timesteps(more then one per month) to monthly values 
        ''' </summary>
        ''' <param name="BB"></param>
        ''' <param name="UpdateStanza"></param>
        ''' <remarks>Multistanza calculations SplitUpdate() need to run on a monthly timestep using the monthly average values for biomass and loss</remarks>
        Private Sub avgMonthlyStanzaVars(ByVal BB() As Single, ByVal UpdateStanza As Boolean)

            For igrp As Integer = 1 To Me.nGroups

                'sum Biomass and Loss
                Me.BBAvg(igrp) += BB(igrp)
                Me.LossAvg(igrp) += Me.m_Data.loss(igrp)

                'EatenByAvg, EatenOfAvg and PredAvg were summed after the first call to derivt

                If UpdateStanza Then
                    Me.BBAvg(igrp) = Me.BBAvg(igrp) / Me.m_Data.StepsPerMonth
                    Me.LossAvg(igrp) = Me.LossAvg(igrp) / Me.m_Data.StepsPerMonth
                    Me.EatenByAvg(igrp) = Me.EatenByAvg(igrp) / Me.m_Data.StepsPerMonth
                    Me.EatenOfAvg(igrp) = Me.EatenOfAvg(igrp) / Me.m_Data.StepsPerMonth
                    Me.PredAvg(igrp) = Me.PredAvg(igrp) / Me.m_Data.StepsPerMonth
                End If
            Next

        End Sub

        ''' <summary>
        ''' Updates numbers, weight, and biomass for multiple stanza species
        ''' </summary>
        ''' <param name="BAvg">Average biomass over all the sub timesteps</param>
        ''' <param name="LossAvg">Average loss over all the sub timesteps</param>
        ''' <param name="BtoUpdate">Biomass array to update to multistanza biomass</param>
        ''' <remarks>BAvg and LossAvg are temporary variables any changes to them will be lost. To update biomass use BtoUpdate(ngroups)</remarks>
        Private Sub SplitUpdate(ByVal BAvg() As Single, ByVal LossAvg() As Single, ByVal BtoUpdate() As Single)
            'updates numbers, weight, and biomass for multiple stanza species
            Dim isp As Integer, ist As Integer, ieco As Integer, ia As Integer
            Dim Su As Single, Gf As Single, Nt As Single
            Dim Agemax As Integer, AgeMin As Integer, Be As Single

            For isp = 1 To Me.m_stanza.Nsplit

                'update numbers and body weights
                ieco = Me.m_stanza.EcopathCode(isp, Me.m_stanza.Nstanza(isp))
                If Me.ResetPred(ieco) = False Then

                    Be = 0
                    For ist = 1 To Me.m_stanza.Nstanza(isp)
                        ieco = Me.m_stanza.EcopathCode(isp, ist)
                        'jb 16-Feb-2010 changed to use monthly averaged biomass and loss 
                        Su = CSng(Math.Exp(-LossAvg(ieco) / 12.0# / BAvg(ieco)))
                        Gf = Me.m_Data.Eatenby(ieco) / Me.m_Data.pred(ieco)  '(month factor here included in splitalpha scaling setup)
                        For ia = Me.m_stanza.Age1(isp, ist) To Me.m_stanza.Age2(isp, ist)
                            Me.m_stanza.NageS(isp, ia) = Me.m_stanza.NageS(isp, ia) * Su
                            Me.m_stanza.WageS(isp, ia) = Me.m_stanza.vBM(isp) * Me.m_stanza.WageS(isp, ia) + Gf * Me.m_stanza.SplitAlpha(isp, ia)
                            If Me.m_stanza.FixedFecundity(isp) Then
                                Be = Be + Me.m_stanza.NageS(isp, ia) * Me.m_stanza.EggsSplit(isp, ia) * Me.m_stanza.SpawnProp(isp, ist)
                            Else
                                ' JS 11-Sep-19 only for spawning life stages
                                If (Me.m_stanza.WageS(isp, ia) > Me.m_stanza.WmatWinf(isp)) Then
                                    Be = Be + Me.m_stanza.NageS(isp, ia) * (Me.m_stanza.WageS(isp, ia) - Me.m_stanza.WmatWinf(isp)) * Me.m_stanza.SpawnProp(isp, ist)
                                End If
                            End If
                        Next ia
                    Next ist

                    Me.m_stanza.WageS(isp, Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp))) = (Su * Me.AhatStanza(isp) + (1 - Su) * Me.m_stanza.WageS(isp, Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp)) - 1)) / (1 - Me.RhatStanza(isp) * Su)
                    If Be = 0 Then Be = 1.0E-20F
                    Me.m_stanza.EggsStanza(isp) = Be
                    'WageS(iSp, 0) = 0
                    'update ages looping backward over age
                    For ist = Me.m_stanza.Nstanza(isp) To 1 Step -1
                        Agemax = Me.m_stanza.Age2(isp, ist)
                        If ist > 1 Then AgeMin = Me.m_stanza.Age1(isp, ist) Else AgeMin = 1
                        If ist = Me.m_stanza.Nstanza(isp) Then
                            Nt = Me.m_stanza.NageS(isp, Agemax) + Me.m_stanza.NageS(isp, Agemax - 1)
                            If Nt = 0 Then Nt = 1.0E-30 'watch for zero numbers of older animals
                            'WageS(isp, Agemax) = (WageS(isp, Agemax) * NageS(isp, Agemax) + WageS(isp, Agemax - 1) * NageS(isp, Agemax - 1)) / Nt
                            Me.m_stanza.NageS(isp, Agemax) = Nt
                            Agemax = Agemax - 1
                        End If
                        For ia = Agemax To AgeMin Step -1
                            Me.m_stanza.NageS(isp, ia) = Me.m_stanza.NageS(isp, ia - 1)
                            Me.m_stanza.WageS(isp, ia) = Me.m_stanza.WageS(isp, ia - 1)
                        Next
                        ieco = Me.m_stanza.EcopathCode(isp, ist)
                        If ist < Me.m_stanza.Nstanza(isp) Then
                            Me.Brec(ieco) = Me.m_stanza.NageS(isp, Me.m_stanza.Age2(isp, ist) + 1) * Me.m_stanza.WageS(isp, Me.m_stanza.Age2(isp, ist) + 1)
                        End If
                    Next

                    'finally set abundance at youngest age to recruitment rate
                    ieco = Me.m_stanza.EcopathCode(isp, Me.m_stanza.Nstanza(isp)) 'code for adult biomass for sp isp
                    'VILLY: note following assumes we extend pair list for egg prod and recpower to add multistanza options  at end of pair lists
                    Me.Srec(ieco) = BAvg(ieco)

                    If Me.m_stanza.BaseEggsStanza(isp) > 0 Then
                        Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, 1)) = Me.m_stanza.RscaleSplit(isp) * Me.m_Data.tval(Me.m_stanza.EggProdShapeSplit(isp)) * Me.m_stanza.RzeroS(isp) * Me.m_Data.tval(Me.m_stanza.HatchCode(isp))
                    End If
                    If Me.m_stanza.HatchCode(isp) = 0 And Me.m_stanza.BaseEggsStanza(isp) > 0 Then
                        Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, 1)) = CSng(Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, 1)) * (Me.m_stanza.EggsStanza(isp) / Me.m_stanza.BaseEggsStanza(isp)) ^ Me.m_stanza.RecPowerSplit(isp))
                    End If
                    Me.m_stanza.WageS(isp, Me.m_stanza.Age1(isp, 1)) = 0
                End If
            Next isp

            ' Update linked recruitments
            For isp = 1 To Me.m_stanza.Nsplit
                If (Me.m_stanza.RecStanza(isp) > 0) And (Me.m_stanza.RecStanza(isp) <= Me.m_stanza.Nsplit) Then
                    Dim sFrom As Single = Me.m_stanza.NageS(Me.m_stanza.RecStanza(isp), Me.m_stanza.Age1(Me.m_stanza.RecStanza(isp), 1))
                    Dim sTo As Single = Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, 1))
                    If (Me.m_stanza.RecStanzaScalar(isp) = 0) Then Me.m_stanza.RecStanzaScalar(isp) = sTo / sFrom
                    Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, 1)) = sFrom * Me.m_stanza.RecStanzaScalar(isp)
                End If
            Next

            ' finally update biomass, pred and NumSplit information for all multistanza species
            'BAvg(averaged biomass) is a temporary variable and will be destroyed 
            'use(BtoUpdate) this is the actual biomass array from the last sub timestep
            Me.SplitSetPred(BtoUpdate)

        End Sub

#If DEBUG Then

        ''' <summary>
        ''' For Debugging. Test that relQ() and FishMGear have been set correctly. 
        ''' </summary>
        ''' <remarks>Only called during initialization of Ecosim. At other times during the run relQ() and FishMGear() may not be equal.</remarks>
        Private Sub debugTestRelQFishMGear()

            'fishing mortality at the current effort
            For iGrp As Integer = 1 To Me.m_EPData.NumLiving
                If (Not Me.m_Data.FisForced(iGrp)) Then

                    For iFlt As Integer = 1 To Me.m_Data.nGear
                        If Me.m_Data.relQ(iFlt, iGrp) > 0 Then
                            'Make sure PropLandedTime() and Propdiscardtime() don't add up to greater than zero
                            Debug.Assert(Math.Round(Me.m_Data.PropLandedTime(iFlt, iGrp) + Me.m_Data.PropDiscardTime(iFlt, iGrp), 3) <= 1.0!,
                                        Me.ToString & ".SetFtimeFromGear() PropLanded + PropDiscarded should not be greater than 1!")
                            'Fishing Mortality "F" computed from base catch rate "q"
                            Dim FfromQ0 As Single = Me.m_Data.relQ(iFlt, iGrp) * (Me.m_Data.PropLandedTime(iFlt, iGrp) + Me.m_Data.PropDiscardTime(iFlt, iGrp))
                            Dim dif As Double = Math.Round(FfromQ0 - Me.m_Data.FishMGear(iFlt, iGrp), 3)
                            'Debug.Assert(dif = 0.0, "relQ and FishMGear are not equal. Something is wrong!")
                        End If
                    Next iFlt

                End If

            Next iGrp

        End Sub

#End If


        'Private Sub Cupdate(ByVal Biom() As Single)
        '    Dim i As Integer, Ceq As Single, istep, Tst As Single, InputMult As Single
        '    Dim Derivcon() As Single, Cintotal() As Single, Closs() As Single, ConCtot As Single
        '    ReDim Derivcon(nGroups), Cintotal(nGroups), Closs(nGroups)
        '    'update change in Contaminant concentrations for 1 month--call after first call to derivt
        '    'in adamsbasforth, rk4
        '    'use Closs first to calculate total uptake from environment as loss to env conc
        '    Tst = 1.0# / (12 * 3)
        '    m_Data.ConcTr(nGroups + 1) = 0
        '    If i = 0 And ConForceNumber > 0 Then InputMult = m_Data.tval(ConForceNumber) Else InputMult = 1
        '    For istep = 1 To 3
        '        ConDeriv(Biom, Derivcon, Cintotal, Closs, InputMult, False)
        '        ConCtot = 0
        '        For i = 0 To nGroups
        '            ConCtot = ConCtot + m_Data.ConcTr(i)
        '            Ceq = Cintotal(i) / (Closs(i) + 1.0E-20)
        '            m_Data.ConcTr(i) = Ceq + (m_Data.ConcTr(i) - Ceq) * Math.Exp(-Closs(i) * Tst)
        '            'm_data.m_data.ConcTr(i) = m_data.ConcTr(i) + Derivcon(i) * Tst
        '            If istep = 3 Then m_Data.ConcTr(nGroups + 1) = m_Data.ConcTr(nGroups + 1) + m_Data.ConcTr(i)
        '        Next
        '    Next
        'End Sub
        ''Private Sub ConDeriv(ByVal Biom() As Single, ByVal Derivcon() As Single, ByVal Cintotal() As Single, ByVal Closs() As Single, ByVal InputMult As Single, ByVal Space As Boolean)
        '    'calculates total derivative of contaminant concentrations given
        '    'rate coefficients from interface and monthly call to derivt


        '    'TODo_jb ConDeriv() port to EcoSpace
        '    'ConDeriv() make a local copy of this for Ecospace Space solver
        '    'm_Data.loss(i) will need to be ecospace.loss
        '    'ConKtrophic(ii) rate of biomass flow to pred from prey for each spatial unit set in DerivtRed (eat / biomass(iPrey))
        '    Dim i As Integer, j As Integer, ii As Integer, K As Integer
        '    Dim ConFlow As Single, GradFlow As Single, ist As Integer, ieco As Integer
        '    'Dim Ceq As Single
        '    Dim DetToEnv As Single
        '    Dim InputMultT As Single
        '    Dim Cgradloss() As Single
        '    ReDim Cgradloss(nGroups)

        '    Array.Clear(Cinflow, 0, Cinflow.Length)

        '    'first accumulate inputs for all pools as functions of concs
        '    'in donor pools and rate constants

        '    'flows associated with trophic linkages
        '    For ii = 1 To m_Data.inlinks
        '        i = m_Data.ilink(ii) : j = m_Data.jlink(ii)
        '        ConFlow = ConKtrophic(ii) * m_Data.ConcTr(i)
        '        Cinflow(j) = Cinflow(j) + ConFlow * (1 - m_EPData.GS(j)) ' (conktrophic ii =eat/biomass(iPrey))
        '        For K = m_EPData.NumLiving + 1 To nGroups
        '            Cinflow(K) = Cinflow(K) + m_EPData.GS(j) * ConFlow * m_EPData.DF(j, K - m_EPData.NumLiving)
        '        Next
        '    Next

        '    'flows associated with detritus and discards
        '    For i = 1 To m_EPData.NumLiving
        '        For j = m_EPData.NumLiving + 1 To nGroups
        '            Cinflow(j) = Cinflow(j) + m_Data.mo(i) * (1 - m_Data.MoPred(i) + m_Data.MoPred(i) * m_Data.Ftime(i)) * m_Data.ConcTr(i) * m_EPData.DF(i, j - m_EPData.NumLiving)
        '            For K = 1 To m_EPData.NumFleet 'nb: loop bypassed if numgear=0
        '                Cinflow(j) = Cinflow(j) + ConKdet(i, j, K) * m_Data.ConcTr(i)
        '            Next
        '        Next
        '    Next

        '    'flows associated with graduation among stanzas
        '    'If Space = False Then
        '    'following code will fail in ecospace, since gradflow is difficult to estimate; ignore it
        '    'when call is from ecospace (space=true)
        '    For i = 1 To m_stanza.Nsplit
        '        For ist = 2 To m_stanza.Nstanza(i)
        '            ieco = m_stanza.EcopathCode(i, ist - 1)
        '            If Space = True Then
        '                GradFlow = 12 * m_stanza.SplitRflow(i, ist) * m_Data.ConcTr(ieco)
        '                Cgradloss(ieco) = 12 * m_stanza.SplitRflow(i, ist)
        '                ieco = m_stanza.EcopathCode(i, ist)
        '                Cinflow(ieco) = Cinflow(ieco) + GradFlow
        '            Else
        '                GradFlow = 12 * m_stanza.NageS(i, m_stanza.Age1(i, ist)) * m_stanza.WageS(i, m_stanza.Age1(i, ist)) * m_Data.ConcTr(ieco) / Biom(ieco)

        '                ' ieco = EcopathCode(i, ist - 1)
        '                Cinflow(ieco) = Cinflow(ieco) - GradFlow
        '                ieco = m_stanza.EcopathCode(i, ist)
        '                Cinflow(ieco) = Cinflow(ieco) + GradFlow
        '            End If
        '        Next
        '    Next

        '    'other losses and flows to environment
        '    Closs(0) = 0
        '    For i = 1 To nGroups
        '        Closs(0) = Closs(0) + Cenv(i) * Biom(i)
        '    Next
        '    DetToEnv = 0
        '    For i = m_EPData.NumLiving + 1 To nGroups
        '        DetToEnv = DetToEnv + m_Data.DetritusOut(i) * m_Data.ConcTr(i)
        '    Next
        '    'save this result as the "loss" rate from environment to ecosystem components
        '    m_Data.loss(0) = Closs(0) : Biom(0) = 1


        '    For i = 0 To nGroups
        '        If i = 0 Then InputMultT = InputMult Else InputMultT = 1.0#
        '        'add environmental and immigration flows to get total inflow
        '        '(at this point, cinflow already sums inflow components from biological flows (derivt)
        '        Cintotal(i) = InputMultT * Cinflow(i) + Cimmig(i) * m_EPData.Immig(i) + Cenv(i) * Biom(i) * m_Data.ConcTr(0)

        '        If i = 0 Then Cintotal(i) = Cintotal(i) + DetToEnv
        '        'and set up total instantaneous loss rate (note coutflow nonzero only for i=0)
        '        'jb for Ecospace loss will need to be ecospace loss 
        '        Closs(i) = m_Data.loss(i) / Biom(i) + Cdecay(i) + CoutFlow(i) + Cgradloss(i) '+ 1E-20
        '        Derivcon(i) = Cintotal(i) - Closs(i) * m_Data.ConcTr(i)
        '        'Ceq = Cintotal / Closs
        '        'update concentration over one month assuming constant inflow and loss over month
        '        'm_data.ConcTr(i) = Ceq + (m_data.ConcTr(i) - Ceq) * Exp(-Closs / 12)
        '    Next
        'End Sub


        'Private Sub SaveEiiDataFromEcosim(ByVal BB() As Single)
        '    Dim i As Integer, j As Integer, ii As Integer

        '    For i = 1 To nGroups
        '        m_Data.DCPct(i, 0) = BB(i)
        '    Next i 'save the biomass

        '    'At this point
        '    'simDCAtT(pred,prey) contains the biomass consumed by a pred on a prey
        '    'DC(pred,prey) is the proportion of biomass that makes up a predators diet
        '    'So we need to convert simDCAtT() from biomass to proportion of total diet
        '    For ii = 1 To m_Data.inlinks
        '        i = m_Data.ilink(ii) : j = m_Data.jlink(ii)
        '        If m_Data.Eatenby(j) > 0 Then
        '            m_Data.simDCAtT(j, i) = m_Data.simDCAtT(j, i) / m_Data.Eatenby(j) 'simDCAtT just used for convenience to store the sim diets
        '        End If
        '    Next ii

        '    For j = 1 To nGroups
        '        m_Data.DCPct(j, 1) = BB(j)
        '    Next j

        '    For j = 1 To m_EPData.NumLiving
        '        m_Data.DCPct(j, 2) = m_Data.Eatenby(j) / BB(j)
        '    Next j

        'End Sub

        ''' <summary>
        ''' Public Function to run the Ecosim Model 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' The EcoSim model must be Initialized before this is call.
        ''' </remarks>
        Public Function Run() As Boolean
            Dim bsuccess As Boolean

            Try
                Dim msg As cMessage = Me.checkOKToRun()
                Me.bStopRunning = False

                ' ToDo: support feedback messages here
                If (msg IsNot Nothing) Then
                    Me.m_publisher.AddMessage(msg)
                    Me.m_publisher.sendAllMessages()
                    Return False
                End If

                If Me.m_TracerData.EcoSimConSimOn Then
                    If Me.m_ConTracer Is Nothing Then
                        Me.m_TracerData.EcoSimConSimOn = False
                        Debug.Assert(False, "Ecosim warning: Contaminant Tracer is turned on but has not been initialized properly.")
                    End If
                End If

                If Me.m_Data.bMultiThreaded Then
                    ThreadPool.QueueUserWorkItem(AddressOf Me.RunModelThreaded)
                Else
                    Me.RunModelValue(Me.m_Data.NumYears, Me.m_search.Frates, Me.m_search.nBlocks)
                End If

                bsuccess = True
                If Me.AbortRun Or Me.bStopRunning Then
                    'Ecosim Failed or was stopped return False
                    bsuccess = False
                End If

            Catch ex As Exception

                cLog.Write(ex)
                Me.m_publisher.AddMessage(New cMessage("Ecosim Run() Error: " & ex.Message, eMessageType.ErrorEncountered,
                                        eCoreComponentType.Ecosim, eMessageImportance.Critical, eDataTypes.NotSet))
                bsuccess = False
            End Try

            Me.m_publisher.sendAllMessages()
            Return bsuccess

        End Function


        Private Sub fireRunCompleted(ByVal Obj As Object)
            If Me.m_OnRunCompletedDelegate <> Nothing Then
                Me.m_OnRunCompletedDelegate.Invoke(Obj)
            End If
        End Sub

        ''' <summary>
        ''' Turn off all searches
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub setSearchOff()
            Me.m_Data.bTimestepOutput = True
            Me.m_search.SearchMode = eSearchModes.NotInSearch 'turn off the fishing policy search
            Me.m_search.setMinSearchBlocks()
            '  m_search.clearBaseYear() 'sets baseyear to zero
        End Sub

        ''' <summary>
        ''' Wraps all the calls to the TimeStep delegate into on method
        ''' </summary>
        ''' <param name="iTime"></param>
        ''' <remarks></remarks>
        Private Sub FireOnTimeStep(ByVal iTime As Integer)

            If Me.m_OnTimeStepDelegate IsNot Nothing Then

                Try

                    If Me.m_Data.bMultiThreaded Then
                        'Marshall the call to an interface onto the thread that created Ecosim via the SynchronizationContext
                        Me.MarshallTimeStep(iTime)
                    Else
                        'Call the TimeStepDelegate directly
                        Me.callTimeStep(iTime)
                    End If

                Catch ex As Exception
                    'swallow any errors so ecosim can keep running
                    cLog.Write(ex)
                    Debug.Assert(False, "Ecosim Error: the interface Ecosim Time Step handler threw an error that was not handled.")
                End Try

            End If
        End Sub

        Private Sub callTimeStep(ByVal Args As Object)
            Dim itime As Integer = CInt(Args)
            Me.m_OnTimeStepDelegate(itime, Me.m_Results)
        End Sub

        ''' <summary>
        ''' Marshall the TimeStep delegate call onto the thread that created Ecosim
        ''' </summary>
        ''' <param name="Args"></param>
        ''' <remarks></remarks>
        Private Sub MarshallTimeStep(ByVal Args As Object)

            Debug.Assert(TypeOf Args Is Integer)
            Me.m_SyncObj.Send(New System.Threading.SendOrPostCallback(AddressOf Me.callTimeStep), Args)

        End Sub

        ''' <summary>
        ''' Processes the current EcoSim Time step
        ''' </summary>
        ''' <param name="iTime">Current time step</param>
        ''' <remarks>
        ''' This is call by RunModelValue() at the end of a time step.
        ''' To package/summarize data from the model into an cEcoSimResults object which 
        ''' then gets passed to the Delegate function (mProgressDelegate) that was initialized in InitMultiThreading(...)
        ''' </remarks>
        Private Sub PopulateResults(ByVal iTime As Integer, ByVal imonth As Integer, ByVal iYear As Integer)
            Try
                Dim ist As Integer
                Dim ia As Integer
                Dim ijuv As Integer, iecojuv As Integer
                Dim SumEf As Single
                Dim iflt As Integer
                Dim igrp As Integer
                Dim totMort As Single
                Dim startCatch As Single

                'Clear out data from last timestep
                Me.m_Results.clear()
                Array.Clear(Me.m_Data.ResultsLandings, 0, Me.m_Data.ResultsLandings.Length)
                Array.Clear(Me.m_Data.ResultsDiscardsMort, 0, Me.m_Data.ResultsDiscardsMort.Length)
                Array.Clear(Me.m_Data.ResultsDiscardsSurvived, 0, Me.m_Data.ResultsDiscardsSurvived.Length)

                Me.m_Results.CurrentT = iTime
                'increment the number of time steps in the summary data
                Me.m_Data.nSumTimeSteps += 1

                Me.calcFunctionalResponse(iTime)

                For igrp = 1 To Me.m_Results.nGroups
                    'output biomass is relative to the Ecopath baseline biomass
                    'StartBiomass() is set at initialization to Ecopath baseline values
                    Me.m_Results.Biomass(igrp) = Me.BB(igrp) / Me.m_Data.StartBiomass(igrp)

                    startCatch = Me.m_Data.StartBiomass(igrp) * Me.m_Data.Fish1(igrp)
                    If startCatch <> 0 Then
                        Me.m_Results.Yield(igrp) = Me.BB(igrp) * Me.m_Data.FishTime(igrp) / startCatch
                        Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.YieldRel, igrp, iTime) = Me.BB(igrp) * Me.m_Data.FishTime(igrp) / startCatch
                    End If

                    'save results over time for output
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Biomass, igrp, iTime) = Me.BB(igrp)
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.BiomassRel, igrp, iTime) = Me.BB(igrp) / Me.m_Data.StartBiomass(igrp)
                    'Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, iTime) = Me.BB(igrp) * Me.m_Data.FishTime(igrp)
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FeedingTime, igrp, iTime) = Me.m_Data.Ftime(igrp)
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.ConsumpBiomass, igrp, iTime) = Me.m_Data.Eatenby(igrp) / Me.BB(igrp)
                    ' JS 25Mar10: preserve TL
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.TL, igrp, iTime) = Me.m_Data.TLSim(igrp)

                    If Me.m_EPData.PP(igrp) < 1 Then
                        totMort = Me.m_Data.loss(igrp) / Me.BB(igrp)
                    Else
                        totMort = Me.pbb(igrp)
                    End If

                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.TotalMort, igrp, iTime) = totMort
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.PredMort, igrp, iTime) = Me.m_Data.Eatenof(igrp) / Me.BB(igrp)
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.FishMort, igrp, iTime) = Me.m_Data.FishTime(igrp) + Me.m_Data.Eatenof(igrp) / Me.BB(igrp)

                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.EcoSysStructure, igrp, iTime) = Me.m_search.BGoalValue(igrp) * Me.BB(igrp)

                    If totMort <> 0 Then
                        Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.MortVPred, igrp, iTime) = Me.m_Data.Eatenof(igrp) / totMort
                        Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.MortVFishing, igrp, iTime) = Me.BB(igrp) * Me.m_Data.FishTime(igrp) / totMort
                    End If

                    For ipred As Integer = 1 To Me.m_Results.nGroups
                        'sum of consumption for pred and prey
                        Me.m_Data.ResultsAvgByPreyPred(cEcosimDatastructures.eEcosimPreyPredResults.Pred, igrp, ipred) += Me.m_Data.Consumpt(igrp, ipred) 'm_Data.Eatenby(i) '
                        Me.m_Data.ResultsAvgByPreyPred(cEcosimDatastructures.eEcosimPreyPredResults.Prey, igrp, ipred) += Me.m_Data.Consumpt(ipred, igrp) 'm_Data.Eatenof(i) '

                        Me.m_Data.PredPreyResultsOverTime(cEcosimDatastructures.eEcosimPreyPredResults.Consumption, igrp, ipred, iTime) = Me.m_Data.Consumpt(igrp, ipred)
                        Me.m_Data.PredPreyResultsOverTime(cEcosimDatastructures.eEcosimPreyPredResults.Pred, igrp, ipred, iTime) = Me.m_Data.Consumpt(igrp, ipred) / Me.BB(igrp)
                        Me.m_Data.PredPreyResultsOverTime(cEcosimDatastructures.eEcosimPreyPredResults.Prey, igrp, ipred, iTime) = Me.m_Data.Consumpt(ipred, igrp) / (Me.BB(igrp) * ((Me.m_Data.Eatenby(igrp) + 1.0E-20F) / Me.BB(igrp))) 'm_Data.Eatenby(igrp) + 1.0E-20F
                    Next ipred

                    SumEf = 0
                    For iflt = 1 To Me.m_Data.nGear    'Discarded fish has no value
                        SumEf = SumEf + Me.m_Data.FishRateGear(iflt, iTime) * Me.m_Data.FishMGear(iflt, igrp)
                    Next
                    If SumEf = 0 Then SumEf = 1

                    'jb Nov-2-2016 remove the check for F=0 so we can calculated discards where all catch survived
                    'If m_Data.FishTime(igrp) > 0 Then
                    Dim CatchMort As Single
                    Dim PropFleet As Single 'proportion of total F(fishing mortality) on group by a fleet
                    Dim TotCatchScalar As Single
                    Dim TotCatch As Single


                    If Me.m_RefData.ForcedFs(igrp, iTime) < 0 Then
                        For iflt = 1 To Me.m_Data.nGear
                            If Me.m_EPData.Landing(iflt, igrp) + Me.m_EPData.Discard(iflt, igrp) > 0 Then

                                'FishTime() and FishMGear() only contain mortality. Discards that survived are not included.
                                PropFleet = Me.m_Data.FishRateGear(iflt, iTime) * Me.m_Data.FishMGear(iflt, igrp) / SumEf

                                'multiplier to scale F [fishing mort rate] to Total Catch [total catch including discards that survived]
                                TotCatchScalar = 1 / (Me.m_Data.PropLandedTime(iflt, igrp) + Me.m_Data.PropDiscardTime(iflt, igrp) + 1.0E-20F)

                                CatchMort = Me.BB(igrp) * Me.m_Data.FishTime(igrp) * PropFleet

                                'Debug warning if fishing mort calcualtion have gotten out of sync
                                'Debug.Assert(Math.Abs(Me.m_Data.FishRateGear(iflt, iTime) * Me.m_Data.FishMGear(iflt, igrp) - Me.m_Data.FishTime(igrp) * PropFleet) < 0.0001, "Ecosim Fishing Mortality is out of sync. Check FishTime() and FishMGear()")

                                'total catch including discard that survived
                                TotCatch = CatchMort * TotCatchScalar

                                Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, iTime) = TotCatch

                                'Total catch that was landed
                                Me.m_Data.ResultsLandings(igrp, iflt) = TotCatch * Me.m_Data.PropLandedTime(iflt, igrp)

                                'Catch mortality that is discarded
                                Me.m_Data.ResultsDiscardsMort(igrp, iflt) = CatchMort * Me.m_Data.PropDiscardTime(iflt, igrp) / (Me.m_Data.PropLandedTime(iflt, igrp) + Me.m_Data.PropDiscardTime(iflt, igrp) + 1.0E-20F)

                                'Total catch that survived = [total catch] - [total catch mortality] 
                                If Me.m_Data.FishTime(igrp) > 0 Or Me.m_Data.FisForced(igrp) Then
                                    'WARNING ResultsDiscardsSurvived() can contain rounding error if the catch and discard numbers are small enough 
                                    'just stripping off the rounding error will not work for all cases...
                                    Me.m_Data.ResultsDiscardsSurvived(igrp, iflt) = CSng(TotCatch * (1 - Me.m_Data.PropLandedTime(iflt, igrp)) * (1 - Me.m_Data.PropDiscardMortTime(iflt, igrp)))
                                Else
                                    'F = 0 all catch was discarded and survived
                                    'Ok in this case calculate discards that survived from the base values
                                    'this only works if the f has not been forced
                                    Me.m_Data.ResultsDiscardsSurvived(igrp, iflt) = Me.m_Data.relQ(iflt, igrp) * Me.m_Data.FishRateGear(iflt, iTime) * Me.BB(igrp) * (1 - Me.m_Data.PropDiscardMortTime(iflt, igrp))
                                End If

                                Me.m_Data.ResultsDiscardsSurvived(igrp, 0) += Me.m_Data.ResultsDiscardsSurvived(igrp, iflt)
                                Me.m_Data.ResultsDiscardsMort(igrp, 0) += Me.m_Data.ResultsDiscardsMort(igrp, iflt)

                                Me.m_Data.ResultsTimeLandingsGroupGear(igrp, iflt, iTime) = Me.m_Data.ResultsLandings(igrp, iflt)
                                Me.m_Data.ResultsTimeDiscardsSurvivedGroupGear(igrp, iflt, iTime) = Me.m_Data.ResultsDiscardsSurvived(igrp, iflt)
                                Me.m_Data.ResultsTimeDiscardsMortGroupGear(igrp, iflt, iTime) = Me.m_Data.ResultsDiscardsMort(igrp, iflt)
                                ' This is rocket science
                                Me.m_Data.ResultsTimeDiscardsGroupGear(igrp, iflt, iTime) = Me.m_Data.ResultsDiscardsMort(igrp, iflt) + Me.m_Data.ResultsDiscardsSurvived(igrp, iflt)

                                'sum catch across all the groups into this fleet
                                'Does not contain the discards that survived
                                Me.m_Data.ResultsSumCatchByGear(iflt, iTime) += TotCatch 'CatchMort

                                'By group gear
                                Me.m_Data.ResultsSumCatchByGroupGear(igrp, iflt, iTime) = TotCatch 'CatchMort
                                Me.m_Data.ResultsSumFMortByGroupGear(igrp, iflt, iTime) = CatchMort / Me.BB(igrp)

                                'sum all fleets into zero index
                                Me.m_Data.ResultsSumCatchByGear(0, iTime) += Me.m_Data.ResultsSumCatchByGear(iflt, iTime)
                                Me.m_Data.ResultsSumFMortByGroupGear(igrp, 0, iTime) += Me.m_Data.ResultsSumFMortByGroupGear(igrp, iflt, iTime)
                                Me.m_Data.ResultsSumCatchByGroupGear(igrp, 0, iTime) += Me.m_Data.ResultsSumCatchByGroupGear(igrp, iflt, iTime)
                                'Discards
                                Me.m_Data.ResultsTimeDiscardsGroupGear(igrp, 0, iTime) += Me.m_Data.ResultsTimeDiscardsGroupGear(igrp, iflt, iTime)
                                Me.m_Data.ResultsTimeDiscardsSurvivedGroupGear(igrp, 0, iTime) += Me.m_Data.ResultsTimeDiscardsSurvivedGroupGear(igrp, iflt, iTime)
                                Me.m_Data.ResultsTimeDiscardsMortGroupGear(igrp, 0, iTime) += Me.m_Data.ResultsTimeDiscardsMortGroupGear(igrp, iflt, iTime)

                                'Results for this time step by group, gear
                                Me.m_Results.BCatch(igrp, iflt) = TotCatch
                                Me.m_Results.Landings(igrp, iflt) = Me.m_Data.ResultsLandings(igrp, iflt)

                            End If ' If m_EPData.Landing(iflt, igrp) + m_EPData.Discard(iflt, igrp) > 0 Then
                        Next iflt

                    Else 'Me.m_RefData.ForcedFs(igrp, iTime) F is forced
                        'F is forced by group
                        'in this case we don't know what the fleet breakdown is
                        Dim CatchByGroup As Single

                        CatchByGroup = Me.BB(igrp) * Me.m_Data.FishTime(igrp)

                        Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.Yield, igrp, iTime) = CatchByGroup
                        Me.m_Data.ResultsLandings(igrp, 0) = CatchByGroup ' * Me.m_Data.PropLandedTime(0, igrp)

                        Me.m_Results.BCatch(igrp, 0) = CatchByGroup
                        Me.m_Results.Landings(igrp, 0) = Me.m_Data.ResultsLandings(igrp, 0)

                        Me.m_Data.ResultsSumCatchByGroupGear(igrp, 0, iTime) = CatchByGroup
                        Me.m_Data.ResultsSumFMortByGroupGear(igrp, 0, iTime) = CatchByGroup / Me.BB(igrp)

                        Me.m_Data.ResultsTimeLandingsGroupGear(igrp, 0, iTime) = Me.m_Data.ResultsLandings(igrp, 0)

                    End If

                    'Average weight is only for multi stanza groups it will be -9999 for all other groups
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.AvgWeight, igrp, iTime) = cCore.NULL_VALUE
                    Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.ProdConsump, igrp, iTime) = Me.SimGEtemp(igrp)

                Next igrp

                Me.CalcValueFromLandings(iTime, iYear)

                'effort
                For iflt = 1 To Me.m_Data.nGear
                    Me.m_Data.ResultsEffort(iflt, iTime) = Me.m_Data.ResultsEffort(iflt, iTime) + Me.m_Data.FishRateGear(iflt, iTime)
                    Me.m_Results.Effort(iflt) = Me.m_Data.ResultsEffort(iflt, iTime)
                Next

                'now set average weight for all multi stanza groups
                For i As Integer = 1 To Me.m_stanza.Nsplit
                    For j As Integer = 1 To Me.m_stanza.Nstanza(i)
                        Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.AvgWeight, Me.m_stanza.EcopathCode(i, j), iTime) = Me.BB(Me.m_stanza.EcopathCode(i, j)) / Me.m_stanza.NumSplit(i, j)
                    Next
                Next

                'sum biomass for each life stage over this year
                'use for stock recruitment
                For ist = 1 To Me.m_stanza.Nsplit
                    For iLf As Integer = 1 To Me.m_stanza.Nstanza(ist) - 1
                        'Brec() is number * weight for the first monthly age group of this ecopath group
                        'this is the biomass that recruited into this ecopath group
                        Me.BrecYear(Me.m_stanza.EcopathCode(ist, iLf)) = Me.BrecYear(Me.m_stanza.EcopathCode(ist, iLf)) + Me.Brec(Me.m_stanza.EcopathCode(ist, iLf))
                    Next
                Next

                'Stock Recruitment plot data
                For ist = 1 To Me.m_stanza.Nsplit
                    'ecopath index of the stock/adult
                    ia = Me.m_stanza.EcopathCode(ist, Me.m_stanza.Nstanza(ist))

                    'relative biomass of the adult life stage stored over time
                    'Srec() contains BB(igroup) at the current time step
                    Me.Bstore(ia, iTime) = Me.Srec(ia) / Me.m_Data.StartBiomass(ia)
                    Me.m_Results.hasSRData = False

                    If imonth = 12 Then
                        'if the month = 12 then send the Stock/Recruitment data to the interface

                        Me.m_Results.hasSRData = True

                        For ijuv = 1 To Me.m_stanza.Nstanza(ist) - 1

                            'no data by default for all groups/life stages
                            Me.m_Results.hasSRData(ist, ijuv) = False

                            If iTime > Me.m_stanza.Age2(ist, ijuv) Then
                                'there is data for this group/life stage
                                Me.m_Results.hasSRData(ist, ijuv) = True

                                'ecopath index of the juvenile
                                iecojuv = Me.m_stanza.EcopathCode(ist, ijuv)

                                'biomass of the stock when the juvenile was age zero
                                Me.m_Results.BStock(ist, ijuv) = Me.Bstore(ia, iTime - Me.m_stanza.Age2(ist, ijuv))
                                Me.m_Results.BRecruitment(ist, ijuv) = Me.BrecYear(iecojuv) / 12 / Me.Rbase(iecojuv)

                            End If 'If iTime > m_stanza.Age2(ist, ijuv) Then
                        Next ijuv

                    End If 'If imonth = 12 Then

                Next 'For ist As Integer = 1 To m_stanza.Nsplit

                If Me.m_TracerData.EcoSimConSimOn Then
                    Me.m_ConTracer.SaveEcosimTimeStepData(iTime, Me.BB, Me.TracerData)
                End If

                'ToDo_jb find a better way to do this
                If imonth = 12 Then
                    'clear out the sum for the next year
                    Array.Clear(Me.BrecYear, 0, Me.m_Data.nGroups)
                End If

                If (iTime < Me.m_Data.NTimes) Then
                    ' JS 11Jan09: Exposed former Network Analysis vars
                    Me.m_Results.TLCatch = Me.m_Data.TLC(iTime)
                    Me.m_Results.FIB = Me.m_Data.FIB(iTime)
                End If

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False)
            End Try

        End Sub

        ''' <summary>
        ''' Encapsulate all logic that validates the model before it is run.
        ''' </summary>
        ''' <returns >
        ''' A message to indicate that something is wrong and the run cannot continue, or 
        ''' nothing if Ecosim is ready to run.
        ''' </returns>
        Private Function checkOKToRun() As cMessage

            If (Me.m_EPData Is Nothing) Then
                Return New cMessage("Ecosim cannot run. Internal eror: Ecopath data is not initialized",
                                    eMessageType.Any, eCoreComponentType.Ecosim, eMessageImportance.Critical)
            End If

            If (Me.m_Data Is Nothing) Then
                Return New cMessage("Ecosim cannot run. Internal eror:  Ecosim data is not initialized",
                                    eMessageType.Any, eCoreComponentType.Ecosim, eMessageImportance.Critical)
            End If

            If (Me.m_search Is Nothing) Then
                Return New cMessage("Ecosim cannot run. Internal eror: Search data is not initialized",
                                    eMessageType.Any, eCoreComponentType.Ecosim, eMessageImportance.Critical)
            End If

            'next check to make sure PeatArena(arena,k) accounts for all i,j consumption rates
            Dim Tcon(,) As Single
            ReDim Tcon(Me.nGroups, Me.nGroups)
            For iii As Integer = 1 To Me.m_Data.NlinksSet
                Dim i As Integer = Me.m_Data.IlinkSet(iii)
                Dim j As Integer = Me.m_Data.KlinkSet(iii)
                Tcon(i, j) = Tcon(i, j) + Me.m_Data.PeatArena(Me.m_Data.ArenaNo(i, Me.m_Data.JlinkSet(iii)), j)
            Next

            ' Validate arena use
            Dim msg As New cFeedbackMessage(My.Resources.CoreMessages.ECOSIM_RUN_ERROR_MISSINGPREDATION, eCoreComponentType.Ecosim, eMessageType.Any, eMessageImportance.Question)
            For i As Integer = 1 To Me.nGroups
                For j As Integer = 1 To Me.nGroups
                    If Me.m_Data.Consumption(i, j) > 0 Then
                        If Tcon(i, j) < 1 Then
                            Dim vs As New cVariableStatus(eStatusFlags.MissingParameter,
                                                          cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_RUN_ERROR_MISSINGPREDATION_DETAIL, Me.m_EPData.GroupName(i), Me.m_EPData.GroupName(j), Tcon(i, j)),
                                                          eVarNameFlags.EcosimArenaShare, eDataTypes.EcosimArenaShare, eCoreComponentType.Ecosim, i, j)
                            msg.AddVariable(vs, True)
                            'assign remaining consumption by j of i to the i,j arena
                            Me.m_Data.PeatArena(Me.m_Data.ArenaNo(i, j), j) = Me.m_Data.PeatArena(Me.m_Data.ArenaNo(i, j), j) + 1 - Tcon(i, j)
                        End If
                    End If
                Next
            Next

            If (msg.Variables.Count > 0) Then
                Me.m_publisher.SendMessage(msg)
                Return Nothing
            End If

            If (Me.m_Data.inlinks < Me.m_Data.Narena) Then
                Return New cMessage("Arena feeding proportions not set properly", eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning)
            End If

            Return Nothing

        End Function


        Friend Sub SetBBtoStartBiomass(ByVal num As Integer)
            Dim i As Integer
            For i = 1 To Me.nGroups 'num
                Me.BB(i) = Me.m_Data.StartBiomass(i)
            Next
        End Sub

        Private Function RandomNormal() As Single
            Dim i As Integer, X As Double
            Dim rnd As New Random(CInt(Date.Now.Ticks))
            X = -6
            For i = 1 To 12
                X = X + rnd.NextDouble
            Next
            Return CSng(X)
        End Function

        ''' <summary>
        ''' Compute stats used for SS for this Ecosim year
        ''' </summary>
        ''' <param name="iyear">Ecosim year index, zero based, first year = zero</param>
        ''' <param name="BB">Ecosim predicted biomass for this year</param>
        ''' <param name="loss">Ecosim predicted loss for this year</param>
        ''' <remarks>Called once a year in the middle of the year </remarks>
        Public Sub AccumulateDataInfo(ByVal iMonth As Integer, ByVal iTimeStep As Integer, iYear As Integer, ByVal BB() As Single, ByRef loss() As Single)
            'accumulates statistical information for comparing model to data
            'for simulation year iyear (0=first simulation year)
            'assumes first simulation year is first calendar year in data csv file

            'ToDo_jb AccumulateDataInfo needs to be made callable by Ecospace
            'ToDo_jb AccumulateDataInfo eTimeSeriesType.AverageWeight is not computed by Ecospace 

            'ToDo_jb AccumulateDataInfo MakeTestData is only set to True from EwE5 Ecoranger EwE6 does not contain Ecoranger so MakeTestData is never True
            Dim iDType As Integer, iDYear As Integer, Zstat As Single
            Dim Zest As Single, SDtest As Single
            SDtest = 0.05

            Try
                If Not Me.m_RefData.HasData(iTimeStep, iMonth, iYear) Then
                    'Nope no reference data
                    Return
                End If

                'now accumulate z statistics for any observations available this year
                For iDType = 1 To Me.m_RefData.AppliedNdatType

                    'If there is reference data for this timestep, month, year get the iDYear index(time step index of the data)
                    If Me.m_RefData.setRefDataIndex(iDYear, iTimeStep, iMonth, iYear) Then

                        ' JS16Mar23: Catches forcing is not a reference TS?!
                        ' JS22Jan24: Relative fishing mort (104) is not included here

                        Debug.Assert(iDYear <> cCore.NULL_VALUE, "Warning: Ecosim.AccumulateDataInfo() failed to find a valid reference data index.")
                        If Me.m_RefData.AppliedDatVal(iDYear, iDType) > 0 And (Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.BiomassRel Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.BiomassAbs Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.TotalMortality Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.AverageWeight Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Catches Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.CatchesForcing Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.CatchesRel Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Discards Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.DiscardsTotalAbs Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.DiscardsTotalRel Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Landings Or
                            Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.FishingMortalityRef) Then

                            Zstat = 0
                            Me.m_RefData.Iobs += 1

                            'data type 0,1,5,6,-6,7
                            Select Case Me.m_RefData.AppliedDatType(iDType)

                                Case eTimeSeriesType.BiomassRel, eTimeSeriesType.BiomassAbs '0, 1 Abundance Data
                                    If Me.MakeTestData Then Me.m_RefData.AppliedDatVal(iDYear, iDType) = CSng(BB(Me.m_RefData.AppliedDatPool(iDType)) * Math.Exp(SDtest * Me.RandomNormal())) ' to test with random error data
                                    Zstat = CSng(Math.Log(Me.m_RefData.AppliedDatVal(iDYear, iDType) / BB(Me.m_RefData.AppliedDatPool(iDType))))
                                    Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(BB(Me.m_RefData.AppliedDatPool(iDType))))

                                Case eTimeSeriesType.TotalMortality      '5 Total mortality Data
                                    Zest = loss(Me.m_RefData.AppliedDatPool(iDType)) / BB(Me.m_RefData.AppliedDatPool(iDType))
                                    If Me.MakeTestData Then Me.m_RefData.AppliedDatVal(iDYear, iDType) = CSng(Zest * Math.Exp(SDtest * Me.RandomNormal())) ' to test with random error data
                                    Zstat = CSng(Math.Log(Me.m_RefData.AppliedDatVal(iDYear, iDType) / Zest))
                                    Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(Zest))

                                Case eTimeSeriesType.Catches, eTimeSeriesType.CatchesForcing, eTimeSeriesType.CatchesRel   '6, -6, 61 Absolute Catch Data, Martell, iDatTypean 02

                                    If Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType)) > 0 Then
                                        Zstat = CSng(Math.Log(Me.m_RefData.AppliedDatVal(iDYear, iDType) / (BB(Me.m_RefData.AppliedDatPool(iDType)) * Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType)))))
                                        If Me.MakeTestData Then Me.m_RefData.AppliedDatVal(iDYear, iDType) = BB(Me.m_RefData.AppliedDatPool(iDType)) * Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType))
                                        Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(BB(Me.m_RefData.AppliedDatPool(iDType)) * Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType))))
                                    End If

                                Case eTimeSeriesType.AverageWeight    '7 Mean body weith data Martell, iDatTypean 02
                                    'Assuming user knows this data type is for split pools only.
                                    'and is treated as a relative index
                                    If Me.m_Data.ResultsOverTime IsNot Nothing Then
                                        Dim iti As Integer = iDYear * 12 - 7

                                        ' iDatTypeS 11Aug10: EcopathCode maps a stanza group to a functional group, and is not related to the number of TS
                                        '             Instead, the average weight of DatPool(iDatType), the targer group, should be obtained here.
                                        'Zest = m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.AvgWeight, m_stanza.EcopathCode(i, iDatType), iti)
                                        Zest = Me.m_Data.ResultsOverTime(cEcosimDatastructures.eEcosimResults.AvgWeight, Me.m_RefData.AppliedDatPool(iDType), iti)
                                        If Zest > 0 Then
                                            Zstat = CSng(Math.Log(Me.m_RefData.AppliedDatVal(iDYear, iDType) / Zest))
                                            Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(Zest))
                                        End If

                                        If Me.MakeTestData Then
                                            Me.m_RefData.AppliedDatVal(iDYear, iDType) = CSng(Zest * Math.Exp(SDtest * Me.RandomNormal()))
                                        End If

                                    End If

                                Case eTimeSeriesType.FishingMortalityRef

                                    If Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType)) > 0 Then
                                        Zstat = CSng(Math.Log(Me.m_RefData.AppliedDatVal(iDYear, iDType) / Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType))))
                                        Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(Me.m_Data.FishTime(Me.m_RefData.AppliedDatPool(iDType))))
                                    End If

                                Case eTimeSeriesType.Discards

                                    Dim iflt As Integer, igrp As Integer
                                    Dim predDiscard As Single
                                    Dim obsDiscard As Single = Me.m_RefData.AppliedDatVal(iDYear, iDType)
                                    If obsDiscard = 0.0 Then obsDiscard = 1.0E-20
                                    iflt = Me.m_RefData.AppliedDatPool(iDType)
                                    igrp = Me.m_RefData.AppliedDatPoolSec(iDType)
                                    predDiscard = Me.m_Data.ResultsDiscardsMort(igrp, iflt) + Me.m_Data.ResultsDiscardsSurvived(igrp, iflt)
                                    If predDiscard = 0 Then predDiscard = 1.0E-20

                                    Zstat = CSng(Math.Log(obsDiscard / predDiscard))
                                    Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(obsDiscard))

                                Case eTimeSeriesType.DiscardsTotalAbs, eTimeSeriesType.DiscardsTotalRel

                                    Dim igrp As Integer
                                    Dim predDiscard As Single
                                    Dim obsDiscard As Single = Me.m_RefData.AppliedDatVal(iDYear, iDType)
                                    If obsDiscard = 0.0 Then obsDiscard = 1.0E-20
                                    igrp = Me.m_RefData.AppliedDatPool(iDType)
                                    predDiscard = Me.m_Data.ResultsDiscardsMort(igrp, 0) + Me.m_Data.ResultsDiscardsSurvived(igrp, 0)
                                    If predDiscard = 0 Then predDiscard = 1.0E-20

                                    Zstat = CSng(Math.Log(obsDiscard / predDiscard))
                                    Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(obsDiscard))

                                Case eTimeSeriesType.Landings

                                    Dim iflt As Integer, igrp As Integer
                                    Dim predLanded As Single
                                    Dim obsLanded As Single = Me.m_RefData.AppliedDatVal(iDYear, iDType)
                                    If obsLanded = 0.0 Then obsLanded = 1.0E-20
                                    iflt = Me.m_RefData.AppliedDatPool(iDType)
                                    igrp = Me.m_RefData.AppliedDatPoolSec(iDType)
                                    predLanded = Me.m_Data.ResultsLandings(igrp, iflt)
                                    If predLanded = 0 Then predLanded = 1.0E-20

                                    Zstat = CSng(Math.Log(obsLanded / predLanded))
                                    Me.m_RefData.Yhat(Me.m_RefData.Iobs) = CSng(Math.Log(obsLanded))

                            End Select

                            'increment counters
                            Me.NobsTime(iDYear) += 1
                            Me.DatNobs(iDType) += 1

                            Me.m_RefData.Wt(Me.m_RefData.Iobs) = Me.m_RefData.AppliedWtType(iDType)
                            'log prediction error by observation
                            Me.m_RefData.Erpred(Me.m_RefData.Iobs) = Zstat
                            'sum of log prediction error by datatype
                            Me.DatSumZ(iDType) = Me.DatSumZ(iDType) + Zstat
                            'squared sum of log prediction error by datatype
                            Me.DatSumZ2(iDType) = Me.DatSumZ2(iDType) + Zstat * Zstat

                            'System.Console.WriteLine(iDType.ToString + "," + iDYear.ToString + "," + Zstat.ToString + "," + BB(m_RefData.DatPool(iDType)).ToString)

                            'ElseIf (m_TracerData.EcoSimConSimOn And m_RefData.DatVal(iDYear, iDType) > 0) And _
                            '                    (m_RefData.DatType(iDType) = eTimeSeriesType.EcotracerConcRel Or m_RefData.DatType(iDType) = eTimeSeriesType.EcotracerConcAbs) Then

                            'ToDo_iDatTypeb AccumulateDataInfo contaminant tracing
                            'If TracerData.ConcTr(DatPool(iDatType)) > 0 Then
                            '    TraceObs = TraceObs + 1
                            '    'NobsTime(iDyear) = NobsTime(iDyear) + 1
                            '    'NobsTime is for testing significance, don't need a similar one for tracer, for now at least
                            '    'Wt(m_refdata.Iobs) = WtType(iDatType)
                            '    Zstat = Log(DatVal(iDyear, iDatType) / ConcTr(DatPool(iDatType)))
                            '    'YTraceHat(m_refdata.Iobs) = Log(ConcTr(DatPool(iDatType)))
                            '    ErTrace(TraceObs) = Zstat
                            '    DatTraceObs(iDatType) = DatTraceObs(iDatType) + 1
                            '    DatTraceZ(iDatType) = DatTraceZ(iDatType) + Zstat
                            '    DatTraceZ2(iDatType) = DatTraceZ2(iDatType) + Zstat * Zstat
                            'End If

                        End If
                    End If 'HasData()
                Next iDType

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.StackTrace)
                Throw New ApplicationException(Me.ToString & ".AccumulateDataInfo() ", ex)
            End Try

        End Sub


        ''' <summary>
        ''' Sets DeltaT and StepsPerMonth.
        ''' Turns off numeric integration if rate of change to great [loss]/[Ecopath base biomass]
        ''' </summary>
        ''' <remarks>This makes a call to Derivt() to compute loss() for numeric integration checking </remarks>
        Public Sub SetTimeSteps()
            'Updated 290507 VC to match Ecosim II
            'sets number of months to simulate=stepsperyear*totaltime
            'and runge-kutta time step to even fraction of a
            'month based on maximum rates
            Dim i As Integer
            Dim rrate As Single

            Me.m_Data.FirstTime = True

            'set values need for Derivt()
            Me.SetFishTimetoFish1()
            For i = 0 To Me.nGroups
                Me.m_Data.Ftime(i) = 1
                Me.m_Data.Hden(i) = Me.m_Data.CmCo(i) / (Me.m_Data.CmCo(i) + 1)
            Next

            Me.Derivt(0, Me.m_Data.StartBiomass, Me.dydx, 1)

            'turn off numeric intergration for groups where rate of change is to big
            For i = 1 To Me.nGroups

                'jb 3-Oct-2013 Allow Detritus to use rk4 integration if rate is low enough
                'This is part of detritus BA, Immig and Emig changes
                'If i > m_EPData.NumLiving Then
                '    m_Data.NoIntegrate(i) = 0 'VC following CJWs email of 05dec97
                'End If

                '17-Feb-2014 Added reset of NoIntegrate if integration of non-stanza group is turned off (NoIntegrate(i) = 0)
                'Stanza groups have a NoIntegrate(group) value < 0. That can never be set to zero.
                'This fixes a bug that prevents the integration from being turn back On once it gets Off 
                If Me.m_Data.NoIntegrate(i) = 0 Then Me.m_Data.NoIntegrate(i) = i

                rrate = Math.Abs(Me.m_Data.loss(i)) / Me.m_Data.StartBiomass(i)
                If rrate > 24 And Me.m_Data.NoIntegrate(i) = i Then
                    'if the rate of loss [total biomass loss]/[ecopath biomass]is greater then 24(?) then turn off the numeric integration 
                    Me.m_Data.NoIntegrate(i) = 0
                End If
            Next

            Me.m_Data.StepsPerMonth = 1
            'StepsPerMonth =  30.41666667 = 356 days per year

            Me.DeltaT = CSng(1 / (12 * Me.m_Data.StepsPerMonth))

        End Sub

        Public Sub Derivt(ByVal t As Single, ByVal Biomass() As Single, ByRef deriv() As Single, ByVal iTimeStepIndex As Integer)

            'calculates volterra  derivatives for each ecopath biomass pool
            'pool loss rate for pool i is stored in m_data.loss(i) for step size checking
            'using biomass vector biomass(i) with n pools
            'aij matrix from MakeAMatrix routine where aij=consumption of i by j/ (biomassi*biomassj)
            'and following parameters for each pool
            'pbm and pbbiomass are p/b parameters defined as:
            'assume pbm=maximum p / b has been entered and
            'base p/b=pbbase at initial biomass equilibrium has been used
            'before entering this routing to
            'calculate pbbiomass= (pbm/pbbase-1)/(initial biomass)
            'cef=GE(i) is consumption efficiency (=P/B divided by Q/B for non primary producers)
            'mo is instantaneous "other" mortality rate
            'emig is emigration rate (estimated at base biomass as immig/biomass
            'immig is biomass immigration rate (amount per time, NOT amount per biomass
            'fish is instantaneous fishing rate
            Dim i As Integer, j As Integer, ii As Integer
            Dim eat As Single, Bprey As Single
            Dim aeff() As Single, Veff() As Single
            Dim Hdent() As Single
            'Primary Production forcing function multiplier
            Dim Pmult As Single
            Dim MoMult As Single
            'Imported Detritus forcing function multiplier
            Dim DtImpMult As Single
            'Imported Detritus after forcing function has been applied
            Dim DetInFlow As Single

            'Adding relative prey energy content to EwE: Villy 2025-03-12
            Dim Eaten(Me.nGroups) As Single
            Dim EnergyRel(Me.nGroups) As Single
            'End of energy

            ReDim aeff(Me.m_Data.inlinks)
            ReDim Veff(Me.m_Data.inlinks)
            ReDim Hdent(Me.nGroups)

            Dim ia As Integer, Vbiom() As Single, Vdenom() As Single
            Try

                ReDim Me.m_Data.ToDetritus(Me.nGroups - Me.m_EPData.NumLiving)

                If Me.m_Data.BioMedData.MedIsUsed(0) Then Me.SetMedFunctions(Biomass)

                Me.setpred(Biomass)

                ReDim Me.m_Data.Eatenof(Me.nGroups)
                ReDim Me.m_Data.Eatenby(Me.nGroups)
                ReDim Me.m_Data.Consumpt(Me.nGroups, Me.nGroups)

                Dim Dwe As Single
                Dwe = 0.5

                'set free nutrient concentration
                Me.m_Data.NutBiom = 0
                For i = 1 To Me.nGroups
                    Me.m_Data.NutBiom = Me.m_Data.NutBiom + Biomass(i)
                Next i
                'jb m_data.NutTot was set in InitState
                'Nf = Nt-sum(Bi)
                Me.m_Data.NutFree = Me.m_Data.NutTot * Me.m_Data.tval(Me.m_Data.NutForceNumber) - Me.m_Data.NutBiom
                If Me.m_Data.NutFree < Me.m_Data.NutMin Then Me.m_Data.NutFree = Me.m_Data.NutMin

                'For debugging the Relative Nutrient Dumping from Ecospace 
                'Debug.Print(m_Data.NutBiom.ToString + ", " + m_Data.tval(m_Data.NutForceNumber).ToString + ", " + (2 * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(1))).ToString)

                'ADDED CODE FOR CONTAMINANT
                'initialize inflow rate to each group, preserving user value of cinflow(0)
                'Dim Ceat As Single

                For j = Me.m_EPData.NumLiving + 1 To Me.nGroups
                    Me.m_Data.ToDetritus(j - Me.m_EPData.NumLiving) = 0
                Next j

                'get switching parameters
                Me.SetRelaSwitch(Biomass)

                'get first estimate of denominators of predation rate disc equations

                'this requires first estimates of vulnerable biomasses Vbiom by foraging arena
                ReDim Vbiom(Me.m_Data.Narena), Vdenom(Me.m_Data.Narena)
                For ii = 1 To Me.m_Data.inlinks

                    i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii) : ia = Me.m_Data.ArenaLink(ii)
                    aeff(ii) = Me.m_Data.Alink(ii) * Me.m_Data.Ftime(j) * Me.m_Data.RelaSwitch(ii)
                    Veff(ia) = Me.m_Data.VulArena(ia) * Me.m_Data.Ftime(i)
                    Me.ApplyAVmodifiers(iTimeStepIndex, aeff(ii), Veff(ia), MoMult, i, Me.m_Data.Jarena(ia), True)  '?not sure this will work right with multiple preds in arenas
                    Vdenom(ia) = Vdenom(ia) + aeff(ii) * Me.m_Data.pred(j) / Me.m_Data.Hden(j)
                Next

                'then calculate first estimate using initial Hden estimates of vulnerable biomass in each arena
                For ia = 1 To Me.m_Data.Narena
                    i = Me.m_Data.Iarena(ia)

                    If Me.m_Data.TrophicOff Then
                        Bprey = Me.m_Data.StartBiomass(i)
                    Else
                        Bprey = Biomass(i)
                    End If
                    If Me.m_Data.BoutFeeding Then
                        If Vdenom(ia) > 0 Then
                            Vbiom(ia) = CSng(Veff(ia) * Bprey * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia))
                        Else
                            Vbiom(ia) = Veff(ia) * Bprey
                        End If
                    Else
                        Vbiom(ia) = Veff(ia) * Bprey / (Me.m_Data.VulArena(ia) + Veff(ia) + Vdenom(ia))
                    End If
                Next

                'then update hden estimates based on new vulnerable biomass estimates
                For ii = 1 To Me.m_Data.inlinks
                    j = Me.m_Data.jlink(ii)
                    ia = Me.m_Data.ArenaLink(ii)
                    Hdent(j) = Hdent(j) + aeff(ii) * Vbiom(ia)
                Next

                For j = 1 To Me.nGroups
                    Me.m_Data.Hden(j) = (1 - Dwe) * CSng((1 + Me.m_Data.Htime(j) * Hdent(j))) + Dwe * Me.m_Data.Hden(j)
                Next

                ReDim Vbiom(Me.m_Data.Narena), Vdenom(Me.m_Data.Narena)
                For ii = 1 To Me.m_Data.inlinks
                    i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii) : ia = Me.m_Data.ArenaLink(ii)
                    Vdenom(ia) = Vdenom(ia) + aeff(ii) * Me.m_Data.pred(j) / Me.m_Data.Hden(j)
                Next

                For ia = 1 To Me.m_Data.Narena
                    i = Me.m_Data.Iarena(ia)

                    If Me.m_Data.TrophicOff Then
                        Bprey = Me.m_Data.StartBiomass(i)
                    Else
                        Bprey = Biomass(i)
                    End If 'If m_Data.TrophicOff Then

                    If Me.m_Data.BoutFeeding Then
                        If Vdenom(ia) > 0 Then
                            Vbiom(ia) = CSng(Veff(ia) * Bprey * (1 - Math.Exp(-Vdenom(ia))) / Vdenom(ia))
                        Else
                            Vbiom(ia) = Veff(ia) * Bprey
                        End If 'If Vdenom(ia) > 0 Then
                    Else
                        Vbiom(ia) = Veff(ia) * Bprey / (Me.m_Data.VulArena(ia) + Veff(ia) + Vdenom(ia))
                    End If 'If m_Data.BoutFeeding Then

                Next

                'then predict consumption flows and cumulative consumptions using the new Vbiom estimates
                For ii = 1 To Me.m_Data.inlinks
                    i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii) : ia = Me.m_Data.ArenaLink(ii)
                    If Me.m_Data.TrophicOff Then Bprey = Me.m_Data.StartBiomass(i) Else Bprey = Biomass(i)
                    Select Case Me.m_Data.FlowType(i, j) 'prey always first
                        Case 1 'donor controlled flow
                            eat = aeff(ii) * Bprey
                        Case 3 'limited total flow
                            'MsgBox ("invalid flow control type setting; edit your mdb")
                            eat = aeff(ii) * Bprey * Me.m_Data.pred(j) / (1 + aeff(ii) * Me.m_Data.pred(j) * Bprey / Me.m_Data.maxflow(i, j))
                        Case 2 'prey limited flow
                            'Vprey = Veff(ii) * Bprey / (vulrate(i, j) + Veff(ii) + aeff(ii) * pred(j) / Hden(j))
                            eat = aeff(ii) * Vbiom(ia) * Me.m_Data.pred(j) / Me.m_Data.Hden(j)
                        Case Else
                            eat = 0
                    End Select

                    'predation mort by link
                    Me.m_Data.MPred(ii) = CSng(eat / (Bprey + 1.0E-20))

                    Me.m_Data.Eatenof(i) = Me.m_Data.Eatenof(i) + eat
                    Me.m_Data.Eatenby(j) = Me.m_Data.Eatenby(j) + eat
                    Me.m_Data.Consumpt(i, j) = eat   'VILLY WHAT IS THIS USED FOR?  WRONG FOR COMPLEX ARENA CASES!
                    Me.m_Data.simDCAtT(j, i) = eat 'DCmean just used for convenience to store the sim diets

                    'ADDED CODE FOR CONTAMINANT ACCOUNTING
                    If Me.m_TracerData.EcoSimConSimOn = True Then
                        If Biomass(i) > 0 Then
                            Me.m_ConTracer.ConKtrophic(ii) = eat / Biomass(i)
                        Else
                            Me.m_ConTracer.ConKtrophic(ii) = 0
                        End If
                    End If

                Next 'For ii = 1 To m_Data.inlinks

                If Me.m_Data.TrophicOff Then
                    For i = 1 To Me.m_EPData.NumLiving
                        Me.m_Data.Eatenof(i) = (Me.Mtotal(i) - Me.m_Data.mo(i) * (1 - Me.m_Data.MoPred(i) + Me.m_Data.MoPred(i) * Me.m_Data.Ftime(i))) * Biomass(i)
                    Next
                End If

                'Added relative energy content, Villy 2025-03-12
                'calc total eaten by each consumer group
                If Me.EnergyUsed Then
                    For j = 1 To Me.m_EPData.NumLiving
                        For i = 1 To Me.nGroups
                            Eaten(j) = Eaten(j) + Me.m_Data.simDCAtT(j, i)
                        Next
                    Next
                    'Calc relative energy in food
                    For j = 1 To Me.m_EPData.NumLiving
                        If Eaten(j) > 0 Then        'a consumer
                            For i = 1 To Me.nGroups
                                EnergyRel(j) = EnergyRel(j) + Me.m_Data.simDCAtT(j, i) / Eaten(j) * Me.m_EPData.Energy(i)
                            Next
                        End If
                    Next
                End If
                'End of energy

                For i = 1 To Me.nGroups
                    Me.m_Data.Eatenby(i) = Me.m_Data.Eatenby(i) + Biomass(i) * Me.m_Data.QBoutside(i)
                Next

                'Make the detritus calculations here:
                Me.SimDetritusMT(iTimeStepIndex, Biomass, Me.m_Data.FishRateGear, Me.m_Data.Eatenby, Me.m_Data.Eatenof, Me.m_Data.ToDetritus, Me.m_Data.GroupDetritus)

                For i = 1 To Me.nGroups

                    If i <= Me.m_EPData.NumLiving Then      'Living group

                        'ToDetritus = ToDetritus + m_data.mo(i) * biomass(i)
                        'pbbiomass is 0 for consumers
                        Pmult = 1.0
                        MoMult = 1.0
                        Me.ApplyAVmodifiers(iTimeStepIndex, Pmult, Veff(1), MoMult, i, i, True)
                        'pbm(i) = 0 for all non PP groups
                        'pbb becomes pbmaxs= pb times a max increase factor = pbm for consumers

                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        'pbb(i) = m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) * Pmult * m_Data.pbm(i) / (1 + Biomass(i) * m_Data.pbbiomass(i))
                        'Changed 3-Mar-2017
                        'Carl Walters email "fixing nutrient effects on primary production in ecosim, and bug in modifying producers with forcing functions and mediation functions"
                        'There is a bad setup in derivt that couples nutrient response effects to the biomass shading effects; these need to vary independently. 
                        '1)      There is a line that calculates pbb(i):
                        'pbb(i) = m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) * Pmult * m_Data.pbm(i) / (1 + Biomass(i) * m_Data.pbbiomass(i))
                        'change the term m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) in this line to just
                        '2.0* m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i))
                        '(this allows primary production rate to as much as double as nutrient concentrations increase)
                        '2)      This necessitates a change in the calculation of NutFreeBase(i) in InitialState:
                        'Debug.Assert(m_Data.pbbiomass(i) = 0)
                        Me.pbb(i) = 2 * Me.m_Data.NutFree / (Me.m_Data.NutFree + Me.m_Data.NutFreeBase(i)) * Pmult * Me.m_Data.pbm(i) / (1 + Biomass(i) * Me.m_Data.pbbiomass(i))
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        'VC051011: To accomodate constant Z policies I've included a recalculation of F = Z - Pred - Other Mortality - Emigration:
                        If Me.m_RefData.PoolForceZ(i, 0) > 0 Then 'constant Z for this group, saved in array 0 for convenience
                            Me.m_Data.FishTime(i) = Me.m_RefData.PoolForceZ(i, 0) - Me.m_Data.Eatenof(i) / Biomass(i) - (Me.m_Data.mo(i) * MoMult * (1 - Me.m_Data.MoPred(i) + Me.m_Data.MoPred(i) * Me.m_Data.Ftime(i)) + Me.m_Data.Emig(i))
                            If Me.m_Data.FishTime(i) < 0 Then Me.m_Data.FishTime(i) = 0
                        End If

                        If Me.StartEatenOf(i) > 0 Then
                            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            ''jb Alternatively just scale Mo as a proportion of the change in q/q0 and PaddP [proportion of additive mortality]
                            'Dim MoPredRatio As Single = m_Data.Eatenof(i) / (StartEatenOf(i) + 1.0E-20F)
                            'm_Data.moTot(i) = m_Data.mo(i) + m_Data.MoPredBase(i) * (1 - MoPredRatio) * (1.0F - m_Data.PaddP(i))
                            ''Debug.Assert(Not Single.IsNaN(m_Data.moTot(i)))
                            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-
                            'jb org formulation
                            ' PredMult = (m_Data.Eatenof(i) / StartEatenOf(i)) / (m_Data.PhHalf(i) + m_Data.Eatenof(i) / StartEatenOf(i))
                            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                            ''From Spreadsheet "simple nonadditive predation model.xls"
                            'When PaddP() is one(all predation mortality is additive) moTot() = mo() 
                            Me.m_Data.moTot(i) = Me.m_Data.moMax(i) / (1 + Me.m_Data.Qh(i) * (Me.m_Data.Eatenof(i) / Me.StartEatenOf(i)))
                            ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        Else
                            Me.m_Data.moTot(i) = Me.m_Data.mo(i)
                        End If

                        Me.m_Data.loss(i) = Me.m_Data.Eatenof(i) + (Me.m_Data.moTot(i) * MoMult * (1 - Me.m_Data.MoPred(i) + Me.m_Data.MoPred(i) * Me.m_Data.Ftime(i)) + Me.m_Data.Emig(i) + Me.m_Data.FishTime(i)) * Biomass(i)

                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                        '27-Nov-2019 Original loss equation not including non additive mortality
                        'm_Data.loss(i) = m_Data.Eatenof(i) + (m_Data.mo(i) * MoMult * (1 - m_Data.MoPred(i) + m_Data.MoPred(i) * m_Data.Ftime(i)) + m_Data.Emig(i) + m_Data.FishTime(i)) * Biomass(i)
                        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

                        ''for debugging nonadditive predation mortality
                        'If m_Data.PaddP(i) < 1 Then
                        '    Dim orgLoss As Single = m_Data.Eatenof(i) + (m_Data.mo(i) * (1 - m_Data.MoPred(i) + m_Data.MoPred(i) * m_Data.Ftime(i)) + m_Data.Emig(i) + m_Data.FishTime(i)) * Biomass(i)
                        '    Debug.Print("Group = " + i.ToString + ", new loss = " + m_Data.loss(i).ToString + ", org loss = " + orgLoss.ToString + ", mo max = " + m_Data.moMax(i).ToString + ", mo used = " + m_Data.moTot(i).ToString + ", mo original = " + m_Data.mo(i).ToString + ", rel pred ratio = " + (m_Data.Eatenof(i) / StartEatenOf(i)).ToString)
                        'End If

                        'on the use of variable GE CJW wrote to VC on 041210: just need to modify derivt to calculate GE for each time step
                        'from GE=0.6Z/(Z+3K*), where Z=loss/B, in the last loop over groups.  That calculation will automatically be overwritten
                        '(dB/dt from it is ignored anyway) for split groups, so not worth avoiding doing it for them.
                        If (Me.m_Data.UseVarPQ And Me.m_EPData.vbK(i) > 0) Then
                            Me.SimGEtemp(i) = Me.m_Data.AssimEff(i) * Me.m_Data.loss(i) / Biomass(i) / (Me.m_Data.loss(i) / Biomass(i) + 3 * Me.m_EPData.vbK(i))
                        Else
                            Me.SimGEtemp(i) = Me.m_Data.SimGE(i)
                        End If

                        'Added relative energy content, Villy 2025-01-14
                        If Me.EnergyUsed Then
                            If Me.EnergyBase(i) > 0 Then
                                Me.SimGEtemp(i) = SimGEtemp(i) * EnergyRel(i) / Me.EnergyBase(i)
                            End If
                        End If
                        'End energy


                        deriv(i) = Me.m_EPData.Immig(i) + Biomass(i) * Me.pbb(i) + Me.SimGEtemp(i) * Me.m_Data.Eatenby(i) - Me.m_Data.loss(i)
                        Me.biomeq(i) = (Me.m_EPData.Immig(i) + Me.m_Data.SimGE(i) * Me.m_Data.Eatenby(i) + Me.pbb(i) * Biomass(i)) / (Me.m_Data.loss(i) / Biomass(i))
                    Else
                        'Detritus group
                        'Flow to detritus from imports and immigration
                        'jb 3-Oct-2013 added immig
                        DtImpMult = 1
                        Me.ApplyAVmodifiers(iTimeStepIndex, DtImpMult, 0, MoMult, i, i, True)
                        DetInFlow = Me.m_EPData.DtImp(i) * DtImpMult + Me.m_EPData.Immig(i)

                        Me.m_Data.loss(i) = Me.m_Data.Eatenof(i) + (Me.m_Data.Emig(i) + Me.m_Data.DetritusOut(i)) * Biomass(i)

                        deriv(i) = DetInFlow + Me.m_Data.ToDetritus(i - Me.m_EPData.NumLiving) - Me.m_Data.loss(i)

                        If Me.m_Data.loss(i) <> 0 And Biomass(i) > 0 And DetInFlow + Me.m_Data.ToDetritus(i - Me.m_EPData.NumLiving) > 0 Then
                            Me.biomeq(i) = (DetInFlow + Me.m_Data.ToDetritus(i - Me.m_EPData.NumLiving)) / (Me.m_Data.loss(i) / Biomass(i))
                        Else
                            Me.biomeq(i) = 1.0E-20
                        End If
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.StackTrace)
                Throw New ApplicationException("Derivt() Error.", ex)
            End Try
            ' cLog.WriteArrayToFile("Deriv EwE6.csv", deriv, t.ToString)
        End Sub

        ''' <summary>
        ''' Thread safe version of SimDetritus
        ''' </summary>
        Public Sub SimDetritusMT(iTime As Integer, ByVal Biomass() As Single, ByVal FishRateGear(,) As Single, ByVal Eatenby() As Single, ByVal EatenOf() As Single, ByRef ToDetritus() As Single, ByRef DetritusByGroup() As Single)
            ' Dim Surplus As Single
            Dim i As Integer, j As Integer, K As Integer
            Dim ToDet As Single, DetFlowN As Single
            Dim AMult As Single, VMult As Single, MortMult As Single
            DetFlowN = 0

            'DetritusByGroup() needs to be cleared because the values are summed into it
            Array.Clear(DetritusByGroup, 0, Me.nGroups)

            For i = 1 To Me.m_EPData.NumLiving
                For j = Me.m_EPData.NumLiving + 1 To Me.nGroups
                    'First take egestion
                    ToDet = Me.m_EPData.GS(i) * Eatenby(i) * Me.m_EPData.DF(i, j - Me.m_EPData.NumLiving)

                    'get the other mortality multiplier/driver from the forcing functions
                    'this will only affect other mort if the multiplier is non-zero
                    MortMult = 1
                    Me.ApplyAVmodifiers(iTime, AMult, VMult, MortMult, i, i, True)

                    'Add dying organisms-including other mort forcing/driver
                    ToDet = ToDet + Me.m_Data.mo(i) * MortMult * Biomass(i) * Me.m_EPData.DF(i, j - Me.m_EPData.NumLiving)

                    For K = 1 To Me.m_EPData.NumFleet
                        'proportion of fishing mortality that was discarded and died
                        Dim PropDiscMort As Single = (Me.m_Data.PropDiscardTime(K, i) / (Me.m_Data.PropLandedTime(K, i) + Me.m_Data.PropDiscardTime(K, i) + 1.0E-20F))
                        'Debug.Assert(PropDiscMort = 0.0)
                        If Me.m_Data.FirstTime = True Then
                            DetFlowN = Me.m_EPData.DiscardFate(K, j - Me.m_EPData.NumLiving) * Biomass(i) * Me.m_Data.FishMGear(K, i) * PropDiscMort
                        Else
                            'Debug.Assert(m_Data.FishMGear(K, i) = 0)
                            'jb 07-Jan-2010 Changed to use Propdiscardtime(fleets,groups) (% discarded for this time step) initialized to ecopath PropDiscard() or set in MSE.RegulateEffort() 
                            DetFlowN = Me.m_EPData.DiscardFate(K, j - Me.m_EPData.NumLiving) * Biomass(i) * FishRateGear(K, 0) * Me.m_Data.FishMGear(K, i) * PropDiscMort
                        End If
                        ToDet = ToDet + DetFlowN

                        If Me.m_TracerData.EcoSimConSimOn = True Then
                            Me.m_ConTracer.ConKdet(i, j - Me.m_EPData.NumLiving, K) = DetFlowN / Biomass(i)
                        End If

                    Next K

                    ToDetritus(j - Me.m_EPData.NumLiving) = ToDetritus(j - Me.m_EPData.NumLiving) + ToDet

                    DetritusByGroup(i) += ToDet

                Next j
            Next i

            'Next add flow from other detritus groups
            For i = Me.m_EPData.NumLiving + 1 To Me.nGroups
                For j = Me.m_EPData.NumLiving + 1 To Me.nGroups
                    If i <> j Then
                        ToDetritus(j - Me.m_EPData.NumLiving) = ToDetritus(j - Me.m_EPData.NumLiving) + Me.m_EPData.DetPassedProp(i) * Biomass(i) * Me.m_EPData.DF(i, j - Me.m_EPData.NumLiving)
                    End If
                Next
            Next

            If Me.m_Data.FirstTime = True Then
                For i = Me.m_EPData.NumLiving + 1 To Me.nGroups
                    'jb 3-Oct-2013 include immigration and import
                    'm_Data.DetritusOut(i) = (ToDetritus(i - m_EPData.NumLiving) - m_EPData.BA(i) + m_EPData.Immig(i) - EatenOf(i)) / Biomass(i) - m_Data.Emig(i)
                    Me.m_Data.DetritusOut(i) = (ToDetritus(i - Me.m_EPData.NumLiving) - Me.m_EPData.BA(i) + Me.m_EPData.Immig(i) + Me.m_EPData.DtImp(i) - EatenOf(i)) / Biomass(i) - Me.m_Data.Emig(i)
                    If Me.m_Data.DetritusOut(i) < 1.0E-20 Then Me.m_Data.DetritusOut(i) = 1.0E-20
                Next i
            End If
            Me.m_Data.FirstTime = False

        End Sub


        '***********************
        'THIS FUNCTION IS COPIED IN cSpaceSolver.vb
        'Changes here will NOT copy over to there
        '***********************
        Public Sub SetRelaSwitch(ByVal B() As Single)     'Switching
            Dim i As Integer, j As Integer, ii As Integer
            Dim PredDen() As Double

            ReDim PredDen(Me.nGroups)
            ReDim Me.m_Data.RelaSwitch(Me.m_Data.inlinks)

            For ii = 1 To Me.m_Data.inlinks
                i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii)
                PredDen(j) = PredDen(j) + Me.A(i, j) * B(i) ^ Me.m_Data.SwitchPower(j)
                Me.m_Data.RelaSwitch(ii) = 1
            Next
            For ii = 1 To Me.m_Data.inlinks
                i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii)
                If Me.m_Data.SwitchPower(j) > 0 Then
                    '    m_Data.RelaSwitch(ii) = 1
                    'Else
                    Me.m_Data.RelaSwitch(ii) = CSng(Me.A(i, j) * B(i) ^ Me.m_Data.SwitchPower(j) / (PredDen(j) + 1.0E-20) / Me.m_Data.BaseTimeSwitch(ii))
                End If
            Next
        End Sub


        Public Sub InitialState()
            'VC changed tzero in CJWs version to TimeJuv()
            'find initial state for delay-difference model pools
            Dim i As Integer
            Me.TimeNow = 1 : Me.YearNow = 1

            ReDim Me.Srec(Me.nGroups)
            ReDim Me.SimGES(Me.nGroups)

            'set up total and free base nutrient concentrations
            Me.m_Data.NutBiom = 0
            For i = 1 To Me.nGroups
                Me.m_Data.NutBiom = Me.m_Data.NutBiom + Me.m_Data.StartBiomass(i)
            Next

            If Me.m_Data.NutBaseFreeProp < 0.1 Then Me.m_Data.NutBaseFreeProp = 0.1
            If Me.m_Data.NutPBmax < 1.1 Then Me.m_Data.NutPBmax = 1.1
            'Nt0 = sum(Bi)/(1-pf)
            Me.m_Data.NutTot = Me.m_Data.NutBiom / (1 - Me.m_Data.NutBaseFreeProp)
            'Nf = Nt0 - sum(Bi)
            Me.m_Data.NutFree = Me.m_Data.NutTot - Me.m_Data.NutBiom

            ReDim Me.m_Data.NutFreeBase(Me.nGroups)
            For i = 1 To Me.m_EPData.NumLiving

                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'm_Data.NutFreeBase(i) = (m_Data.PBmaxs(i) - 1) * m_Data.NutFree
                'Changed 3-Mar-2017
                'Carl Walters email "fixing nutrient effects on primary production in ecosim, and bug in modifying producers with forcing functions and mediation functions"
                'There is a bad setup in derivt that couples nutrient response effects to the biomass shading effects; these need to vary independently. 
                '1)      There is a line that calculates pbb(i):
                'pbb(i) = m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) * Pmult * m_Data.pbm(i) / (1 + Biomass(i) * m_Data.pbbiomass(i))
                'change the term m_Data.PBmaxs(i) * m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i)) in this line to just
                '2.0* m_Data.NutFree / (m_Data.NutFree + m_Data.NutFreeBase(i))
                '(this allows primary production rate to as much as double as nutrient concentrations increase)

                '2)      This necessitates a change in the calculation of NutFreeBase(i) in InitialState:
                Me.m_Data.NutFreeBase(i) = Me.m_Data.NutFree
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            Next
            Me.m_Data.NutMin = CSng(0.00101 * Me.m_Data.NutFree)

            If Me.m_TracerData.EcoSimConSimOn = True Then Me.initConTracer()

            For i = 0 To Me.nGroups
                Me.m_Data.Ftime(i) = 1
                Me.m_Data.Hden(i) = Me.m_Data.CmCo(i) / (Me.m_Data.CmCo(i) - 1)
            Next

            For i = 1 To Me.nGroups
                Me.m_Data.Cbase(i) = Me.StartEatenBy(i) / Me.m_Data.StartBiomass(i)
                '  If No(i, KageMax(i)) > 1.0E-20 Then m_data.Cbase(i) = StartEatenBy(i) / No(i, KageMax(i))
                If Me.m_Data.Cbase(i) = 0 Then Me.m_Data.Cbase(i) = 1 : Me.m_Data.FtimeMax(i) = 1
                Me.CBlast(i) = Me.m_Data.Cbase(i)
            Next

            'jb Split Pool initialization code removed from here See EwE5 InitialState()

            'Mediation initialization:
            Me.InitializeMedFunctions()

            Me.InitializePriceFunctions()

            'multiple stanza initialize
            Dim B() As Single
            ReDim B(Me.nGroups)
            Me.SplitInitialize(B)
            Me.setpred(Me.m_Data.StartBiomass)

            For i = 1 To Me.nGroups
                Me.m_Data.Cbase(i) = Me.StartEatenBy(i) / Me.m_Data.pred(i)
                Me.CBlast(i) = Me.m_Data.Cbase(i)
                Me.m_Data.Qmain(i) = (1 - Me.m_Data.RiskTime(i)) * Me.m_Data.Cbase(i)
                Me.m_Data.Qrisk(i) = CSng(Me.m_Data.RiskTime(i) * Me.m_Data.Cbase(i) * (Me.StartEatenOf(i) / Me.m_Data.StartBiomass(i) + Me.m_Data.mo(i) + 0.0000000001))
            Next

            ReDim Me.PredPerBiomass(Me.nGroups)
            ReDim Me.ResetPred(Me.nGroups)
            For i = 1 To Me.nGroups
                Me.PredPerBiomass(i) = Me.m_Data.pred(i) / Me.m_Data.StartBiomass(i)
            Next
            '
            Me.MakeAMatrix()
            'Switching:
            Me.InitRelaSwitch()

            If cCore.USE_SHARED_ARENAS Then
                Me.DefineFlowList()
            Else
                DefineArenasAndFlowList()
            End If
            Me.SetArenaVulandSearchRates()

        End Sub

        Private Sub InitRelaSwitch()     'Switching
            Dim i As Integer, j As Integer, ii As Integer
            Dim PredDen() As Double
            ReDim PredDen(Me.nGroups)
            ReDim Me.m_Data.BaseTimeSwitch(Me.m_Data.inlinks)

            For ii = 1 To Me.m_Data.inlinks
                i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii)
                PredDen(j) = PredDen(j) + Me.A(i, j) * Me.m_Data.StartBiomass(i) ^ Me.m_Data.SwitchPower(j)
            Next

            For ii = 1 To Me.m_Data.inlinks
                i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii)
                Me.m_Data.BaseTimeSwitch(ii) = CSng(Me.A(i, j) * Me.m_Data.StartBiomass(i) ^ Me.m_Data.SwitchPower(j) / (PredDen(j) + 1.0E-20))
            Next

        End Sub

        Private Sub Calc_nvar()
            Me.nvar = Me.nGroups '+ 3 * npairs
            'For numbers
        End Sub


        Private Sub MakeAMatrix()
            'calculate volterra rates of effective search ai,j=cons/(bibj)
            Dim i As Integer, j As Integer
            For i = 1 To Me.nGroups  'N1 'prey
                For j = 1 To Me.nGroups  'N1
                    Me.MakeAMatrixCell(i, j) 'prey, consumer
                Next
            Next

        End Sub

        Private Sub MakeAMatrixCell(ByVal i As Integer, ByVal j As Integer) 'here prey is first
            Dim Dzero As Single, Denv As Single

            ' Debug.Assert((i = 5 And j = 10) = False)

            If Me.SimQB(j) > 0 Then
                Me.m_Data.Htime(j) = CSng(Me.m_Data.pred(j) / (Me.m_Data.CmCo(j) * Me.m_Data.StartBiomass(j) * Me.SimQB(j)))
            Else
                Me.m_Data.Htime(j) = 0
            End If
            Dzero = Me.m_Data.CmCo(j) / (Me.m_Data.CmCo(j) - 1)
            Me.A(i, j) = 0.0#
            If Me.m_Data.StartBiomass(i) > 0 Then
                Select Case Me.m_Data.FlowType(i, j) '
                    Case 1 'donor controlled flow

                        Me.A(i, j) = Me.m_Data.Consumption(i, j) / Me.m_Data.StartBiomass(i)

                    Case 3 'total flow limited

                        If Me.m_Data.pred(j) > 0 And Me.m_Data.Consumption(i, j) > 0 And Me.m_Data.Consumption(i, j) <> Me.m_Data.maxflow(i, j) Then
                            Me.A(i, j) = Me.m_Data.Consumption(i, j) / (Me.m_Data.StartBiomass(i) * Me.m_Data.pred(j) * (1 - Me.m_Data.Consumption(i, j) / Me.m_Data.maxflow(i, j)))
                        End If

                        'pred is predator biomass
                    Case 2 'prey avail limited 'And pred(j) > 10 ^ -10

                        If Me.m_Data.Consumption(i, j) > 0 Then
                            Denv = (Me.m_Data.StartBiomass(i) * Me.m_Data.pred(j) * Me.m_Data.vulrate(i, j) - Me.m_Data.Consumption(i, j) * Me.m_Data.pred(j))
                            If Denv < 1.0E-20 Then Denv = 1.0E-20
                            Me.A(i, j) = Dzero * 2 * Me.m_Data.Consumption(i, j) * Me.m_Data.vulrate(i, j) / Denv
                        Else
                            Me.A(i, j) = 0
                        End If

                End Select
            End If 'If m_data.StartBiomass(i) > 0 Then

        End Sub




        Friend Sub setpred(ByVal Biomass() As Single)
            'Routine modified 290597 VC to follow ESimII
            Dim i As Integer ', ii As Integer
            'set predator abundance measure used for predation
            'rate calculations; this is just biomass for simple pools
            For i = 1 To Me.nGroups
                'If i > N And biomass(i) = 0 Then biomass(i) = 1
                If Biomass(i) < 1.0E-20 Then Biomass(i) = 1.0E-20 '0.00000001
                If Me.m_Data.NoIntegrate(i) >= 0 Then
                    Me.m_Data.pred(i) = Biomass(i)
                End If
            Next

            For i = 1 To Me.nGroups
                If Me.ResetPred(i) Then
                    Me.m_Data.pred(i) = Biomass(i) * Me.PredPerBiomass(i)
                End If
            Next

        End Sub


        Private Sub SplitInitialize(ByVal B() As Single)
            ' intiializes dynamic state variables for multistanza species
            Dim isp As Integer, ist As Integer, ieco As Integer, ia As Integer

            'VERIFY_JS: 060918 remote m_stanza ReDim should NOT be necessary!
            ReDim Me.m_stanza.NumSplit(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            ReDim Me.m_stanza.NageS(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            ReDim Me.m_stanza.WageS(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            ReDim Me.m_stanza.SplitAlpha(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            ReDim Me.m_stanza.SplitRflow(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            Dim Be As Single
            'ReDim NageSsaved(Nsplit, MaxAgeSplit) As Single, WageSsaved(Nsplit, MaxAgeSplit) As Single 'VILLY: THIS NEEDS TO BE  MOVED TO DO ONLY ON ECOSIM LOAD WHERE OTHER SAVED ARRAYS DIMENSIONED
            ReDim Me.m_stanza.RscaleSplit(Me.m_stanza.Nsplit)
            ReDim Me.m_stanza.EggsSplit(Me.m_stanza.Nsplit, Me.m_stanza.MaxAgeSplit)
            Dim Agem As Integer, Rtot As Single, its As Integer ' , nsc As Integer

            For isp = 1 To Me.m_stanza.Nsplit

                Me.m_stanza.EggProdIsSeasonal(isp) = False
                If Me.m_Data.isSeasonal(Me.m_stanza.EggProdShapeSplit(isp)) Then
                    Me.m_stanza.EggProdIsSeasonal(isp) = True
                End If

                Be = 0 : ist = 1
                For ia = Me.m_stanza.Age1(isp, 1) To Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp))
                    Me.m_stanza.NageS(isp, ia) = Me.m_stanza.SplitNo(isp, ia)
                    Me.m_stanza.WageS(isp, ia) = Me.m_stanza.SplitWage(isp, ia)
                    ' Keep track which life stage this age belongs to
                    If (ia > Me.m_stanza.Age2(isp, ist)) Then
                        ist += 1
                    End If
                    ' JS 11-Sep-19 only for spawning life stages
                    If (Me.m_stanza.WageS(isp, ia) > Me.m_stanza.WmatWinf(isp)) Then
                        Me.m_stanza.EggsSplit(isp, ia) = (Me.m_stanza.WageS(isp, ia) - Me.m_stanza.WmatWinf(isp)) * Me.m_stanza.SpawnProp(isp, ist)
                        Be = Be + Me.m_stanza.NageS(isp, ia) * Me.m_stanza.EggsSplit(isp, ia) ': Stop
                    End If
                Next ia

                ' If Be = 0.0F Then Be = 1.0E-20F
                Me.m_stanza.BaseEggsStanza(isp) = Be
                Me.m_stanza.EggsStanza(isp) = Be
                For ist = 1 To Me.m_stanza.Nstanza(isp)
                    ieco = Me.m_stanza.EcopathCode(isp, ist)
                    'turn of the integration for all multi stanza groups
                    'see rk4() splitupdate()
                    Me.m_Data.NoIntegrate(ieco) = -ieco
                    If ist < Me.m_stanza.Nstanza(isp) Then
                        Me.Rbase(ieco) = Me.m_stanza.NageS(isp, Me.m_stanza.Age2(isp, ist) + 1) * Me.m_stanza.WageS(isp, Me.m_stanza.Age2(isp, ist) + 1)
                    End If
                Next ist

                'set rescaling factor for seasonal recruitment to give same annual average
                'as if recruitment were even over months
                Me.m_stanza.RscaleSplit(isp) = 1
                If Me.m_stanza.EggProdIsSeasonal(isp) Then
                    'mean value for the seasonal Egg Prod shape
                    Rtot = 0
                    'jb the number of months is fixed at 12
                    For its = 1 To 12
                        Rtot = Rtot + Me.m_Data.zscale(its, Me.m_stanza.EggProdShapeSplit(isp))
                    Next
                    Me.m_stanza.RscaleSplit(isp) = 12 / Rtot
                End If 'If m_stanza.EggProdIsSeasonal(isp) Then
            Next isp

            'cLog.WriteMatrixToFile("N At Age EwE6.csv", m_stanza.NageS, "EwE6")
            'cLog.WriteMatrixToFile("W At Age EwE6.csv", m_stanza.WageS, "EwE6")

            Me.SplitSetPred(B)
            'initialize splitalpha growth coefficients using pred information
            For isp = 1 To Me.m_stanza.Nsplit
                For ist = 1 To Me.m_stanza.Nstanza(isp)
                    ieco = Me.m_stanza.EcopathCode(isp, ist)

                    Agem = Me.m_stanza.Age2(isp, ist) : If ist = Me.m_stanza.Nstanza(isp) Then Agem = Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp)) - 1
                    For ia = Me.m_stanza.Age1(isp, 1) To Agem
                        Me.m_stanza.SplitAlpha(isp, ia) = (Me.m_stanza.SplitWage(isp, ia + 1) - Me.m_stanza.vBM(isp) * Me.m_stanza.SplitWage(isp, ia)) * Me.m_Data.pred(ieco) / Me.StartEatenBy(ieco)
                    Next ia
                Next ist
                Me.m_stanza.SplitAlpha(isp, Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp))) = Me.m_stanza.SplitAlpha(isp, Me.m_stanza.Age2(isp, Me.m_stanza.Nstanza(isp)) - 1)
            Next isp
            'initialize splitgroup flux rates among stanzas for ecospace
            For isp = 1 To Me.m_stanza.Nsplit
                For ist = 2 To Me.m_stanza.Nstanza(isp)
                    Me.m_stanza.SplitRflow(isp, ist) = Me.m_stanza.NageS(isp, Me.m_stanza.Age1(isp, ist)) * Me.m_stanza.WageS(isp, Me.m_stanza.Age1(isp, ist)) / B(Me.m_stanza.EcopathCode(isp, ist - 1))
                Next ist
            Next isp

            'cLog.WriteMatrixToFile("N At Age EwE6.csv", m_stanza.NageS, "EwE6")
            'cLog.WriteMatrixToFile("W At Age EwE6.csv", m_stanza.WageS, "EwE6")
            'cLog.WriteMatrixToFile("SplitAlpha 6.csv", m_stanza.SplitAlpha, "EwE6")
            'jb
            'pred in EwE5 for detritus is 1e-20 EwE6 it is zero 
            'which is effectivly zero so it should not matter

            'cLog.WriteMatrixToFile("SplitRflow 6.csv", m_stanza.SplitRflow, "EwE6")
            'cLog.WriteArrayToFile("Pred 6.csv", m_Data.pred, "EwE6")
            'cLog.WriteMatrixToFile("NumSplit 6.csv", m_stanza.NumSplit, "EwE6")
            'cLog.WriteMatrixToFile("EggsSplit 6.csv", m_stanza.EggsSplit, "EwE6")

        End Sub

        Public Sub SplitSetPred(ByVal B() As Single)
            'sets total biomass B and Preds predation index by multistanza species
            Dim isp As Integer, ist As Integer, ieco As Integer, ia As Integer
            Dim Bt As Single, pt As Single, Nt As Single

            For isp = 1 To Me.m_stanza.Nsplit
                For ist = 1 To Me.m_stanza.Nstanza(isp)
                    Bt = 1.0E-30
                    pt = 1.0E-30
                    Nt = 1.0E-30
                    For ia = Me.m_stanza.Age1(isp, ist) To Me.m_stanza.Age2(isp, ist)
                        Bt = Bt + Me.m_stanza.NageS(isp, ia) * Me.m_stanza.WageS(isp, ia)
                        pt = pt + Me.m_stanza.NageS(isp, ia) * Me.m_stanza.WWa(isp, ia) 'VILLY: wwa should be w^2/3 from yer ecopath initialization setup
                        Nt = Nt + Me.m_stanza.NageS(isp, ia)
                    Next ia

                    ieco = Me.m_stanza.EcopathCode(isp, ist)
                    B(ieco) = Bt
                    Me.m_Data.pred(ieco) = pt  'VILLY: note this avoids using setpred routine for multi stanza species, hope it will work ok
                    Me.m_stanza.NumSplit(isp, ist) = Nt

                Next ist
            Next isp

        End Sub


        ''' <summary>
        ''' called from InitialState to set up trophic mediation functions
        ''' and determine which ones are active for a simulation
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub InitializeMedFunctions()

            Dim ii As Integer, i As Integer, j As Integer, jj As Integer ', MedX As Single
            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing
            Dim medData As cMediationDataStructures = Me.m_Data.BioMedData

            'Clear out the old data set all mediation functions to false
            'SetMedFunctions is only called from derivt if
            'MedIsUsed(0) is set to true
            For i = 0 To medData.MediationShapes
                medData.MedIsUsed(i) = False
                medData.MedVal(i) = 1
            Next

            'now set MedIsUsed() to True for all trophic links that have had IsMedFunction() set to True 
            For ii = 1 To Me.m_Data.inlinks
                i = Me.m_Data.ilink(ii) : j = Me.m_Data.jlink(ii)
                For jj = 1 To cMediationDataStructures.MAXFUNCTIONS
                    If Me.m_Data.BioMedData.IsMedFunction(i, j, jj) Then        'MF() ranges from 0 to MediationShapes (=9)
                        medData.MedIsUsed(0) = True
                        medData.MedIsUsed(Me.m_Data.BioMedData.FunctionNumber(i, j, jj)) = True
                    End If
                Next
            Next

            'now check all the Primary Producers they where not included in the inlinks loop above
            For ii = 1 To Me.m_Data.nGroups
                If Me.m_EPData.PP(ii) = 1 Then
                    For jj = 1 To cMediationDataStructures.MAXFUNCTIONS
                        If Me.m_Data.BioMedData.IsMedFunction(ii, ii, jj) Then
                            'The zero index = True if any mediation funtion has been applied
                            medData.MedIsUsed(0) = True
                            medData.MedIsUsed(Me.m_Data.BioMedData.FunctionNumber(ii, ii, jj)) = True
                        End If
                    Next
                End If
            Next

            For i = 1 To medData.MediationShapes
                'jb removed the MedIsUsed() so that all mediation functions get initialized
                'this is for the Single Player Game it needs to use the mediation functions even if they have not been assigned e.g. MedIsUsed(SinglePlayerMedFunction) = false
                'this should not matter as all operations on the med function have to check MedIsUsed() before use... I hope...
                ' If m_Data.MedIsUsed(i) = True Then

                medData.MedXbase(i) = 0
                jj = 0
                For j = 1 To Me.nGroups + Me.m_EPData.NumFleet
                    If medData.MedWeights(j, i) > 0 Then
                        jj = jj + 1
                        If j <= Me.nGroups Then
                            medData.MedXbase(i) = medData.MedXbase(i) + medData.MedWeights(j, i) * Me.m_Data.StartBiomass(j)
                        Else
                            'medData.MedXbase(i) = medData.MedXbase(i) + medData.MedWeights(j, i) * m_Data.FishRateGear(j - nGroups, 0)
                            'Ecosim effort at Ecopath base effort is always one
                            medData.MedXbase(i) = medData.MedXbase(i) + medData.MedWeights(j, i) * 1
                        End If
                        medData.IMedUsed(jj, i) = j
                    End If
                Next

                medData.NMedXused(i) = jj
                medData.MedYbase(i) = medData.Medpoints(medData.IMedBase(i), i)
                If medData.MedYbase(i) = 0 Then

                    ' Create base message
                    If (msg Is Nothing) Then
                        msg = New cMessage(My.Resources.CoreMessages.MEDIATION_ZERO_BASE, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning)
                        Me.m_publisher.AddMessage(msg)
                    End If

                    ' Add detail
                    vs = New cVariableStatus(eStatusFlags.ErrorEncountered,
                                             cStringUtils.Localize(My.Resources.CoreMessages.MEDIATION_ZERO_BASE_DETAIL, medData.MediationTitles(i)),
                                             eVarNameFlags.MedFunctNumber, eDataTypes.Mediation, eCoreComponentType.Ecosim, i)
                    msg.AddVariable(vs)
                    ' Flag med fn as unusable
                    medData.MedIsUsed(i) = False
                End If

                If jj = 0 Or medData.MedXbase(i) = 0 Then
                    medData.MedIsUsed(i) = False
                End If

            Next

        End Sub

        Public Sub InitializePriceFunctions()

            Dim iShp As Integer, iGrp As Integer, nGrps As Integer ', MedX As Single
            Dim msg As cMessage = Nothing
            Dim vs As cVariableStatus = Nothing
            Dim PriceMedData As cMediationDataStructures = Me.m_Data.PriceMedData

            Try

                'PriceMedData.MedPriceWeights(5, 1, 1) = 1

                'Clear out the old data set all mediation functions to false
                'SetMedFunctions is only called from derivt if
                'MedIsUsed(0) is set to true
                For iShp = 0 To PriceMedData.MediationShapes
                    PriceMedData.MedIsUsed(iShp) = False
                    PriceMedData.MedVal(iShp) = 1
                Next

                For iGrp = 1 To Me.nGroups
                    For iflt As Integer = 1 To Me.m_Data.nGear
                        For iFnt As Integer = 1 To cMediationDataStructures.MAXFUNCTIONS
                            If (Me.m_Data.PriceMedData.PriceMedFuncNum(iGrp, iflt, iFnt) > 0) Then        'MF() ranges from 0 to MediationShapes (=9)
                                PriceMedData.MedIsUsed(Me.m_Data.PriceMedData.PriceMedFuncNum(iGrp, iflt, iFnt)) = True
                            End If
                        Next
                    Next
                Next

                For iShp = 1 To PriceMedData.MediationShapes
                    'jb removed the MedIsUsed() so that all mediation functions get initialized
                    'this is for the Single Player Game it needs to use the mediation functions even if they have not been assigned e.g. MedIsUsed(SinglePlayerMedFunction) = false
                    'this should not matter as all operations on the med function have to check MedIsUsed() before use... I hope...
                    ' If m_Data.MedIsUsed(i) = True Then
                    PriceMedData.MedXbase(iShp) = 0
                    nGrps = 0
                    For iGrp = 1 To Me.nGroups
                        For iflt As Integer = 1 To Me.m_EPData.NumFleet
                            If PriceMedData.MedPriceWeights(iGrp, iflt, iShp) > 0 Then
                                'sum the weighted landings across all the fleets on this group 
                                'for baseline X on this price elasticity function

                                nGrps = nGrps + 1
                                PriceMedData.MedXbase(iShp) += Me.m_EPData.Landing(iflt, iGrp) * PriceMedData.MedPriceWeights(iGrp, iflt, iShp)
                                PriceMedData.IMedUsed(nGrps, iShp) = iGrp
                                PriceMedData.IMedFltUsed(nGrps, iShp) = iflt
                            End If
                        Next
                    Next

                    PriceMedData.NMedXused(iShp) = nGrps
                    PriceMedData.MedYbase(iShp) = PriceMedData.Medpoints(PriceMedData.IMedBase(iShp), iShp)
                    If PriceMedData.MedYbase(iShp) = 0 Then

                        ' Create base message
                        If (msg Is Nothing) Then
                            msg = New cMessage(My.Resources.CoreMessages.MEDIATION_ZERO_BASE, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning)
                            Me.m_publisher.AddMessage(msg)
                        End If

                        ' Add detail
                        vs = New cVariableStatus(eStatusFlags.ErrorEncountered,
                                                 cStringUtils.Localize(My.Resources.CoreMessages.MEDIATION_ZERO_BASE_DETAIL, PriceMedData.MediationTitles(iShp)),
                                                 eVarNameFlags.MedFunctNumber, eDataTypes.Mediation, eCoreComponentType.Ecosim, iShp)
                        msg.AddVariable(vs)
                        ' Flag med fn as unusable
                        PriceMedData.MedIsUsed(iShp) = False
                    End If

                    If nGrps = 0 Or PriceMedData.MedXbase(iShp) = 0 Then
                        PriceMedData.MedIsUsed(iShp) = False
                    End If

                Next

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".InitializePriceFunctions() Exception: " & ex.Message)
            End Try


        End Sub
        '***********************
        'THIS FUNCTION IS COPIED IN cSpaceSolver.vb
        'Changes here will NOT copy over to there
        '***********************
        Friend Sub SetMedFunctions(ByVal Biom() As Single)
            'called from derivt, derivtred if MedIsUsed(0)=true to set
            'current Y value of each active trophic mediation function
            Me.m_Data.BioMedData.SetMedFunctions(Biom, Me.m_Data.FishRateGear, Me.TimeNow)

        End Sub

        ''' <summary>
        ''' Sets pbbiomass() and clears pbm() for Primary Producer groups
        ''' </summary>
        ''' <remarks>
        ''' pbm() and pbbiomass() must be zero for groups that are not primary producers. 
        ''' This prevents Derivt() from calculating primary production on non PP groups.  
        ''' </remarks>
        Public Sub Set_pbm_pbbiomass()
            Dim i As Integer
            'VC: Only used for p.producers
            For i = 1 To Me.nGroups
                If Me.m_Data.SimGE(i) > 0 Then
                    'Clear pbm for consumer and detritus groups
                    'This stops the calculation of PP on Non PP groups in Derivt
                    Me.m_Data.pbm(i) = 0
                ElseIf Me.pbbase(i) > 0 Then
                    'set pbbiomass for primary produces only
                    'm_Data.SimGE(i) will be zero for primary producers and detritus groups
                    Me.m_Data.pbbiomass(i) = (Me.m_Data.pbm(i) / Me.pbbase(i) - 1) / Me.m_Data.StartBiomass(i)
                Else
                    Me.m_Data.pbbiomass(i) = 0
                End If
            Next

        End Sub

        Public Sub SetFishTimetoFish1()
            Dim i As Integer
            For i = 1 To Me.m_EPData.NumGroups
                Me.m_Data.FishTime(i) = Me.m_Data.Fish1(i)
            Next
        End Sub

        Public Sub CalcStartEatenOfBy()
            'VC 160797
            Dim i As Integer, j As Integer

            For i = 1 To Me.m_EPData.NumGroups
                Me.StartEatenBy(i) = Me.m_Data.StartBiomass(i) * Me.SimQB(i)
                Me.EatenByBase(i) = Me.StartEatenBy(i)
                Me.StartEatenOf(i) = 0
                For j = 1 To Me.m_EPData.NumGroups
                    If j <= Me.m_EPData.NumLiving And Me.SimQB(j) > 0 And Me.m_Data.SimDC(j, i) > 0 Then
                        Me.StartEatenOf(i) = Me.StartEatenOf(i) + Me.m_Data.StartBiomass(j) * Me.SimQB(j) * Me.m_Data.SimDC(j, i)
                    End If

                Next
                Me.Mtotal(i) = Me.StartEatenOf(i) / Me.m_Data.StartBiomass(i) + Me.m_Data.mo(i)

            Next i

            'CalcBaseAdditiveMort()

        End Sub


        Public Sub CalcBaseAdditiveMort()
            Dim i As Integer

            For i = 1 To Me.m_EPData.NumGroups

                If Me.StartEatenOf(i) > 0 And Me.m_Data.mo(i) > 0 Then
                    Me.m_Data.MoPredBase(i) = Me.StartEatenOf(i) / Me.m_Data.StartBiomass(i)
                    Me.m_Data.moMax(i) = Me.m_Data.mo(i) + Me.m_Data.MoPredBase(i) * (1.0F - Me.m_Data.PaddP(i))
                    '=Mmax/Mo-1
                    Me.m_Data.Qh(i) = CSng(Me.m_Data.moMax(i) / Me.m_Data.mo(i) - 1.0F)

                Else
                    Me.m_Data.moMax(i) = 1.0E-20
                    Me.m_Data.Qh(i) = 0
                End If

                ''jb 11-jan-2024 PhHalf is not use in the new implementation of additive mortality
                'If Me.m_Data.PaddP(i) > 0 Then
                '    Me.m_Data.PhHalf(i) = 1.0F / Me.m_Data.PaddP(i) - 1.0F
                'Else
                '    Me.m_Data.PhHalf(i) = 1.0F
                'End If

            Next i

        End Sub

        Public Sub InitializeDataInfo()
            'initializes arrays used to estimate catchability coefficients
            'and measures of goodness of fit to reference data
            ReDim Me.DatSumZ(Me.m_RefData.AppliedNdatType)
            ReDim Me.DatSumZ2(Me.m_RefData.AppliedNdatType)
            ReDim Me.DatNobs(Me.m_RefData.AppliedNdatType)
            ReDim Me.NobsTime(Me.m_RefData.AppliedDatPoints)
            ReDim Me.DatDev(Me.m_RefData.AppliedNdatType, Me.m_RefData.AppliedDatPoints)

            'data managed by cTimeSeriesDataStructures
            'Dimensioned across all the data even if these data are forcing and not used for stats in AccumulateDataInfo()
            'This is because AppliedDatVal(,) contains data this is both forcing and reference
            'and it easier to loop over all the data rather then keeping at seperate dimensions
            ReDim Me.m_RefData.Erpred(Me.m_RefData.AppliedNdatType * Me.m_RefData.AppliedDatPoints)
            ReDim Me.m_RefData.Yhat(Me.m_RefData.AppliedNdatType * Me.m_RefData.AppliedDatPoints)
            ReDim Me.m_RefData.Wt(Me.m_RefData.AppliedNdatType * Me.m_RefData.AppliedDatPoints)
            Me.m_RefData.Iobs = 0

        End Sub

        Public Sub PlotDataInfo(ByVal PlotOn As Boolean, ByRef Ss As Single, Optional ByRef SSgroup() As Single = Nothing)
            'called at end of runmodel to calculate catchabilities and plot
            'relative abundance index data

            ' VC+JB June20 2008: we're adding a calculation of SS by group to the SS calculations,
            ' We're only doing it from Ecosim now, that's what the optional SSgroup is doing

            ' Set a flag to populte SSGroup 
            Dim bSSgrp As Boolean = (SSgroup IsNot Nothing)

            'ToDo_jb PlotDataInfo AverageBodyWeight
            Dim iDatPt As Integer, iDType As Integer, iYear As Integer ', bplot As Single

            ReDim Me.m_RefData.AppliedDatSS(Me.m_RefData.AppliedNdatType)
            ReDim Me.m_RefData.AppliedDatQ(Me.m_RefData.AppliedNdatType)
            ReDim Me.m_RefData.AppliedeDatQ(Me.m_RefData.AppliedNdatType)
            ReDim Me.m_RefData.AppliedSSPredErr(Me.m_RefData.AppliedNdatType)

            For iDType = 1 To Me.m_RefData.AppliedNdatType
                If Me.DatNobs(iDType) > 0 Then

                    ' CW April 2016: It is incorrect to be overriding the first ss calculation with the second one 
                    ' (corrected for mean Zstat) for catches and forced catches; it is necessary to estimate q for 
                    ' mean weight data, since the ecosim internal mean weight is not in the same units as users are 
                    ' likely to enter for mean weight data. So the OR condition needs to be removed for etimeseriestype 
                    ' Catch and Catch forcing.

                    ' JB April 2016: DataQ() is now only calculated for relative biomass, total mortality and average weight. 
                    ' For all other data types it is set to zero so it will not have any scaling effect. I checked this 
                    ' with a few different catch time series and it does appear to be working correctly. 

                    ' VC June 2016: We've run into an issue with catches: Chiara has a model where we can only trust the 
                    ' trend in the catch data, not the actual values. This may actually be a general problem, so we've added
                    ' a new time series type, 61 (6 for catches and 1 for relative) catches, relative (reference)

                    'JB Oct 2019 Relative catches (61) were not included in the relative calculations places
                    'REALLY it took that long to figure that out....

                    'Calculate DatQ() for data types that are relative.
                    'These data may not have been entered in the same units as the internal ecosim mean weight
                    'DatQ() is used to normalize/scale the time series data to model units
                    If Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.BiomassRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.TotalMortality Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.CatchesRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.AverageWeight Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.OffVesselPriceRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.SailCostRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.FishingMortalityRef Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.DiscardsTotalRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.FixedCostRel Then

                        'If cTimeSeries.IsRelative(Me.m_RefData.AppliedDatType(iDType)) Then
                        Me.m_RefData.AppliedDatSS(iDType) = CSng(Me.DatSumZ2(iDType) - Me.DatSumZ(iDType) ^ 2 / Me.DatNobs(iDType))
                        Me.m_RefData.AppliedDatQ(iDType) = Me.DatSumZ(iDType) / Me.DatNobs(iDType)
                        Me.m_RefData.AppliedeDatQ(iDType) = CSng(Math.Exp(Me.m_RefData.AppliedDatQ(iDType)))
                    Else
                        'all other data types are used as is
                        Me.m_RefData.AppliedDatSS(iDType) = Me.DatSumZ2(iDType)
                        Me.m_RefData.AppliedDatQ(iDType) = 0
                    End If

                End If
            Next

            'Reset all counters and sums
            Me.m_RefData.Iobs = 0
            Ss = 0
            If bSSgrp Then
                Array.Clear(SSgroup, 0, SSgroup.Length)
            End If

            For iDatPt = 1 To Me.m_RefData.AppliedDatPoints
                iYear = Me.m_RefData.AppliedDatYear(iDatPt) - Me.m_RefData.AppliedDatYear(1)
                For iDType = 1 To Me.m_RefData.AppliedNdatType
                    ' JS 15Feb24: if below are reference time series, then CatchesForcing is misplaced
                    If Me.m_RefData.AppliedDatVal(iDatPt, iDType) > 0 And iYear < Me.m_Data.NumYears + 1 And
                        (Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.BiomassRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.BiomassAbs Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.TotalMortality Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Catches Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.CatchesForcing Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.AverageWeight Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Discards Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.DiscardsTotalAbs Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.DiscardsTotalRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.CatchesRel Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.FishingMortalityRef Or
                        Me.m_RefData.AppliedDatType(iDType) = eTimeSeriesType.Landings) Then
                        'cTimeSeries.IsReference(Me.m_RefData.AppliedDatType(iDType)) Then

                        Me.m_RefData.Iobs = Me.m_RefData.Iobs + 1
                        'following debug.print checks to insure m_refdata.Iobs data alignment has been
                        'maintained during the simulation: prints logY(m_refdata.Iobs),logYij then
                        'logN(m_refdata.Iobs) in er, -m_RefData.Yhat(m_refdata.Iobs) before adding lnq
                        'Debug.Print m_RefData.Erpred(m_refdata.Iobs) + m_RefData.Yhat(m_refdata.Iobs), Log(m_refData.DatVal(i, j))
                        'Debug.Print m_RefData.Erpred(m_refdata.Iobs) - Log(m_refData.DatVal(i, j)), -m_RefData.Yhat(m_refdata.Iobs)
                        'following remove conditional mle of q from prediction error, add to
                        'prediction of logN
                        Me.m_RefData.Erpred(Me.m_RefData.Iobs) = Me.m_RefData.Erpred(Me.m_RefData.Iobs) - Me.m_RefData.AppliedDatQ(iDType)
                        Me.DatDev(iDType, iDatPt) = Me.m_RefData.Erpred(Me.m_RefData.Iobs)
                        Ss = CSng(Ss + Me.m_RefData.Wt(Me.m_RefData.Iobs) * Me.m_RefData.Erpred(Me.m_RefData.Iobs) ^ 2)
                        Me.m_RefData.AppliedSSPredErr(iDType) = CSng(Me.m_RefData.AppliedSSPredErr(iDType) + (Me.m_RefData.Wt(Me.m_RefData.Iobs) * Me.m_RefData.Erpred(Me.m_RefData.Iobs) ^ 2))
                        'Next is to calculate the SS by group:
                        If bSSgrp Then
                            Dim iGroup As Integer = 0
                            Select Case Me.m_RefData.AppliedDatType(iDType)
                                Case eTimeSeriesType.Discards, eTimeSeriesType.Landings
                                    ' Discards and landings data is indexed by fleet x group
                                    iGroup = Me.m_RefData.AppliedDatPoolSec(iDType)
                                Case Else
                                    ' All other reference time series are indexed by group
                                    iGroup = Me.m_RefData.AppliedDatPool(iDType)
                            End Select
                            SSgroup(iGroup) += CSng(Me.m_RefData.Wt(Me.m_RefData.Iobs) * Me.m_RefData.Erpred(Me.m_RefData.Iobs) ^ 2)
                        End If
                        Me.m_RefData.Yhat(Me.m_RefData.Iobs) = Me.m_RefData.Yhat(Me.m_RefData.Iobs) + Me.m_RefData.AppliedDatQ(iDType)
                    End If

                Next
            Next


            ' JS 02Aug10: LogL is not used anymore
            'Dim LogL As Single
            'For j = 1 To m_RefData.NdatType
            '    If m_RefData.DatSS(j) > 0 Then LogL = LogL + m_RefData.WtType(j) * (DatNobs(j) - 1) * Math.Log(m_RefData.DatSS(j))
            'Next
            'LogL = LogL / 2

            'vc sep 2008, adding an option for increasing SS with a fishing mortality penalty
            'if doing a fit to time series, and there are any fmax in the fishing policy screen,
            'then use them here:
            'VC instead of changing SS, we now change the calculation of F, see SetFishTime

            'If m_search.bInSearch And m_search.bUseFishingMortalityPenality Then
            '    For Grp As Integer = 1 To m_EPData.NumGroups
            '        Dim maxF As Single = 0
            '        For iYr As Integer = 1 To m_Data.NumYears * m_Data.NumStepsPerYear Step m_Data.NumStepsPerYear
            '            'the next is just a place keeper awaiting access to fmax
            '            ' If m_Data.FishRateNo(Grp, iYr) > 3 * m_EPData.PB(Grp) Then '  m_search.FLimit(Grp) Then
            '            If m_Data.FishRateNo(Grp, iYr) > m_search.FLimit(Grp) Then
            '                If m_Data.FishRateNo(Grp, iYr) > maxF Then maxF = m_Data.FishRateNo(Grp, iYr)
            '            End If
            '        Next
            '        If maxF > 0 Then Ss = Ss * (maxF / (3 * m_EPData.PB(Grp))) ^ 2 ' : Stop
            '        ' If maxF > 0 And m_search.FLimit(Grp) > 0 Then Ss = Ss * (maxF / m_search.FLimit(Grp)) ^ 2 ' : Stop
            '    Next
            'End If

        End Sub


        Friend Sub RedimForSearchRun()
            ReDim Me.BB(Me.nGroups)
            ReDim Me.biomeq(Me.nGroups)
            ReDim Me.m_Data.Consumption(Me.nGroups, Me.nGroups)
            ReDim Me.m_Data.Consumpt(Me.nGroups, Me.nGroups)
            ReDim Me.dydx(Me.nGroups)
            ReDim Me.dym(Me.nGroups)
            ReDim Me.dyt(Me.nGroups)
            ReDim Me.m_Data.Eatenby(Me.nGroups)
            ReDim Me.m_Data.Eatenof(Me.nGroups)
            ReDim Me.EatenByBase(Me.nGroups)
            ReDim Me.m_Data.FishTime(Me.nGroups)
            ReDim Me.m_Data.loss(Me.nGroups)
            ReDim Me.m_Data.pred(Me.nGroups)
            ReDim Me.yt(Me.nGroups)

        End Sub

        Friend Sub RedimEcoSimVars()

            ReDim Me.m_Data.maxflow(Me.nGroups, Me.nGroups)
            ReDim Me.m_Data.Ftime(Me.nGroups)
            ReDim Me.m_Data.Hden(Me.nGroups)

            ReDim Me.A(Me.nGroups, Me.nGroups)
            ReDim Me.m_Data.AssimEff(Me.nGroups)
            ReDim Me.RiskRate(Me.nGroups)

            ReDim Me.m_Data.BaseTimeSwitch(Me.nGroups)

            ReDim Me.ResetPred(Me.nGroups)
            ReDim Me.EscalePar(Me.m_EPData.NumFleet)
            ReDim Me.CapGrowthFactor(Me.m_EPData.NumFleet)

            'jb moved to SetDefaultParameters
            'ReDim GearIncludeInEquil(m_EPData.NumFleet)

            ReDim Me.IadCode(Me.nGroups), Me.IjuCode(Me.nGroups), Me.IecoCode(Me.nGroups)
            Me.NutPBmax = 1.5

            ReDim Me.Qopt(Me.nGroups)

            ReDim Me.BB(Me.nGroups)
            ReDim Me.biomeq(Me.nGroups)

            ReDim Me.Brec(Me.nGroups)
            ReDim Me.CBlast(Me.nGroups)
            ReDim Me.deriv(Me.nGroups)
            ReDim Me.m_Data.DetritusOut(Me.nGroups)
            ReDim Me.Deatenby(Me.nGroups)
            ReDim Me.Deatenof(Me.nGroups)
            ReDim Me.Dfitness(Me.nGroups)

            ReDim Me.dydx(Me.nGroups)
            ReDim Me.dym(Me.nGroups)
            ReDim Me.dyt(Me.nGroups)
            ReDim Me.yt(Me.nGroups)

            ReDim Me.m_Data.loss(Me.nGroups)

            ReDim Me.EatenByBase(Me.nGroups)
            ReDim Me.m_Data.FishTime(Me.nGroups)
            ReDim Me.m_Data.mo(Me.nGroups)

            ReDim Me.Nrec(Me.nGroups)
            ReDim Me.pbb(Me.nGroups)
            ReDim Me.pbbase(Me.nGroups)
            ReDim Me.m_Data.pbbiomass(Me.nGroups)
            ReDim Me.Rbase(Me.nGroups)
            ReDim Me.m_Data.SimGE(Me.nGroups)
            ReDim Me.SimGEtemp(Me.nGroups)

            ReDim Me.StartEatenOf(Me.nGroups)
            ReDim Me.Mtotal(Me.nGroups)
            ReDim Me.StartEatenBy(Me.nGroups)
            ReDim Me.m_Data.StartBiomass(Me.nGroups)
            ReDim Me.SimQB(Me.nGroups)

            ReDim Me.m_search.LastYearIncomeSpecies(Me.m_EPData.NumFleet, Me.m_EPData.NumGroups)
            ReDim Me.BestTime(Me.m_EPData.NumLiving)
            ReDim Me.BrecYear(Me.nGroups)
            ReDim Me.BBAvg(Me.nGroups)
            ReDim Me.LossAvg(Me.nGroups)
            ReDim Me.EatenByAvg(Me.nGroups)
            ReDim Me.EatenOfAvg(Me.nGroups)
            ReDim Me.PredAvg(Me.nGroups)
            ReDim Me.fCatch0(Me.nGroups)

            'energy VC 2025-03-12
            ReDim Me.EnergyBase(Me.m_EPData.NumLiving)

        End Sub

        Private Sub BaseValueOfHarvest()
            Dim i As Integer
            Dim j As Integer
            Me.BaseValue = 0
            For i = 1 To Me.nGroups
                For j = 1 To Me.m_EPData.NumFleet
                    'If Landing(j, i) + Discard(j, i) > 0 Then BaseValue = BaseValue +m_Data.Fish1(i) * m_data.StartBiomass(i) * Market(j, i) * Landing(j, i) / (Landing(j, i) + Discard(j, i))
                    '040106VC: Was as above, but this is wrong, it doesn't have landing by gear, need to use catch by gear, or average value by group
                    If Me.m_EPData.fCatch(i) > 0 Then
                        Me.BaseValue = Me.BaseValue + Me.m_Data.Fish1(i) * Me.m_Data.StartBiomass(i) * Me.m_EPData.Market(j, i) * Me.m_EPData.Landing(j, i) / Me.m_EPData.fCatch(i)
                    End If
                Next
            Next
        End Sub

        Private Sub BaseValueOfFishMGear()
            'both Fish1(nGroups) (fishing mortality by group) and fCatch(nGroups) (total biomass of catch) must be populated before this is called

            Dim i As Integer, j As Integer
            For i = 1 To Me.m_EPData.NumFleet
                For j = 1 To Me.m_EPData.NumGroups
                    Me.m_Data.FishMGear(i, j) = 0
                    If Me.m_EPData.fCatch(j) > 0 Then
                        'JB 12-Aug-2015 don't include the discards that survived in FishMGear() 
                        'm_Data.FishMGear(i, j) = m_Data.Fish1(j) * (m_EPData.Landing(i, j) + m_EPData.Discard(i, j)) / m_EPData.fCatch(j)
                        Me.m_Data.FishMGear(i, j) = Me.m_Data.Fish1(j) * (Me.m_EPData.Landing(i, j) + (Me.m_EPData.Discard(i, j) * Me.m_EPData.PropDiscardMort(i, j))) / Me.m_EPData.fCatch(j)
                    End If
                Next
            Next

            'Also set FishMGear for all gear combined: m_EPData.NumFleet + 1
            For j = 1 To Me.m_EPData.NumGroups
                Me.m_Data.FishMGear(Me.m_EPData.NumFleet + 1, j) = Me.m_Data.Fish1(j)
            Next

        End Sub
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' call calculatestanzaparameters whenever going from ecopath to ecosim
        ''' to set up multistanza initial state variables
        ''' </remarks>
        Public Function InitStanza() As Boolean
            ' call calculatestanzaparameters whenever going from ecopath to ecosim
            'to set up multistanza initial state variables

            Try
                Dim isp As Integer
                Dim i As Integer
                Dim ieco As Integer
                Dim Bio() As Single
                Dim Bat() As Single
                Dim first() As Integer
                Dim Z() As Single
                Dim cb() As Single
                Dim second() As Integer

                ReDim Bio(Me.m_stanza.MaxStanza)
                ReDim Z(Me.m_stanza.MaxStanza)
                ReDim cb(Me.m_stanza.MaxStanza)
                ReDim Bat(Me.m_stanza.MaxStanza)
                ReDim first(Me.m_stanza.MaxStanza)
                ReDim second(Me.m_stanza.MaxStanza)
                ' JS060918: redimensioned by Nsplit rather than by hard-coded 50
                ReDim Me.AhatStanza(Me.m_stanza.Nsplit)
                ReDim Me.RhatStanza(Me.m_stanza.Nsplit)

                For isp = 1 To Me.m_stanza.Nsplit
                    For i = 1 To Me.m_stanza.Nstanza(isp)
                        ieco = Me.m_stanza.EcopathCode(isp, i)
                        first(i) = Me.m_stanza.Age1(isp, i)
                        If i > 1 Then second(i - 1) = first(i) - 1
                        Bio(i) = Me.m_EPData.B(ieco)
                        Z(i) = Me.m_EPData.PB(ieco)
                        cb(i) = Me.m_EPData.QB(ieco)

                        Me.m_stanza.SpeciesCode(i, 0) = isp
                        Me.m_stanza.SpeciesCode(i, 1) = ieco
                        Me.m_stanza.SpeciesCode(i, 2) = ieco
                    Next

                    ' Make sure default base group is specified
                    If (Me.m_stanza.BaseStanza(isp) <= 0) Then
                        Me.m_stanza.BaseStanza(isp) = Me.m_stanza.Nstanza(isp) 'last lifestage for this stanza group
                    End If
                    ' Make sure default base group is specified
                    If (Me.m_stanza.BaseStanzaCB(isp) <= 0) Then
                        Me.m_stanza.BaseStanzaCB(isp) = Me.m_stanza.Nstanza(isp) 'last lifestage for this stanza group
                    End If

                    ieco = Me.m_stanza.EcopathCode(isp, Me.m_stanza.BaseStanza(isp))

                    Me.CalculateStanzaParameters(isp, Me.m_stanza.Nstanza(isp), Me.m_stanza.BaseStanza(isp), first, second, Bio,
                                                Me.m_EPData.vbK(ieco), Z, Me.m_stanza.BaseStanzaCB(isp), cb, Me.m_stanza.BABsplit(isp), Bat)

                    'jb added set Age2() In EwE5 this was done by the database when Age1() was read in
                    'Computed Stanza variables can not be used before InitStanza is called
                    'get how many month this group is to reach 90% of rel. Winf; K is monthly
                    For i = 1 To Me.m_stanza.Nstanza(isp)
                        Me.m_stanza.Age2(isp, i) = second(i)
                        'Explicitly copy the computed values into the ecopath inputs
                        'these values should never be different during the initialization
                        'if the user has changed a parameter from the interface the core should have done this OnChanged()
                        'this could create the problem that modeling data has changed but the core has no idea this has happened
                        'the user should really then save the model this way at least the data in memory is corect
                        ieco = Me.m_stanza.EcopathCode(isp, i)
                        Me.m_EPData.Binput(ieco) = Bio(i)
                        Me.m_EPData.PBinput(ieco) = Z(i)
                        Me.m_EPData.QBinput(ieco) = cb(i)

                        'Added for external programs that update Ecopath variables
                        'but don't have access to internal calls that coordinate updating of variables
                        Me.m_EPData.B(ieco) = Bio(i)
                        Me.m_EPData.PB(ieco) = Z(i)
                        Me.m_EPData.QB(ieco) = cb(i)

                    Next i

                Next isp


            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("InitStanza()", ex)
            End Try


            'cLog.WriteMatrixToFile("splitWage 6.csv", m_stanza.SplitWage, "6")
            'cLog.WriteMatrixToFile("WWA 6.csv", m_stanza.WWa, "6")
            'cLog.WriteMatrixToFile("SplitNo 6.csv", m_stanza.SplitNo, "6")

        End Function

        ''' <summary>
        ''' Compute initial state stanza variables for a split group argument #1 isp
        ''' 
        ''' </summary>
        ''' <param name="isp">index of the split group to compute variables for</param>
        ''' <param name="Stanza">Number of Stanzas (life stages) in this split group Nstanza(isp)</param>
        ''' <param name="BaseStanza">index of the base stanza for this split group this is the i of the oldest (base) stanza/life stage for this split group</param>
        ''' <param name="first">index of Age1 for all stanzas in this split group</param>
        ''' <param name="Second">Output Variable index of Age2 for all stanzas in this split group</param>
        ''' <param name="Bio">biomass for the base stanza of this split group. 
        ''' During initialization of ecosim this is the Ecopath biomass for this base stanza.</param>
        ''' <param name="vbK">Growth curvature parameter for the base stanza of the split group</param>
        ''' <param name="Z">mortality of each stanza for this split. Ecopath PB</param>
        ''' <param name="BaseCB">Consumption Biomass (Ecopath QB) for the base stanza of this split group BaseStanzaCB(isp)</param>
        ''' <param name="cb">Consumption Biomass (Ecopath QB) for each stanza in this split group </param>
        ''' <param name="BaB">BiomassAccumulation over Biomass biomass acumulation rate for this split group</param>
        ''' <param name="Bat">Output variable BaB * Bio(iStanza)</param>
        ''' <remarks>calculates 
        ''' vBM(isp), SplitWage(isp, MaxYears),WWa(isp, MaxYears), AhatStanza(isp),RhatStanza(isp),RzeroS(isp),SplitNo(isp, Age)
        '''</remarks>
        Public Function CalculateStanzaParameters(ByVal isp As Integer, ByVal Stanza As Integer, ByVal BaseStanza As Integer, ByRef first() As Integer,
                                                ByRef Second() As Integer, ByRef Bio() As Single, ByRef vbK As Single, ByRef Z() As Single,
                                                ByRef BaseCB As Integer, ByRef cb() As Single, ByRef BaB As Single, ByRef Bat() As Single) As Boolean
            'isp above is split species code number
            Try
                Dim Age As Integer
                '   Dim i As Integer
                Dim Grp As Integer
                Dim Surv As Single
                Dim PrevSurv As Single
                'Dim NoMonths
                Dim SumB As Single
                'Dim age2(isp,) As Integer
                'These will be dimensioned by stanza's:
                Dim Consumption() As Single

                'These ones are by month:
                Dim Survive() As Single
                Dim Recruits As Single
                Dim K As Single

                'throw an error for error testing
                'isp = isp / 0

                'input parameters have not been set 
                'don't try to calculate the paramaters
                If Bio(BaseStanza) < 0 Or cb(BaseCB) < 0 Then
                    System.Console.WriteLine("Missing parameters for Stanza Group '" & Me.m_stanza.StanzaName(isp) & "' CalculateStanzaParameters() will not be run.")
                    Exit Function
                End If

                If vbK = 0 Then
                    '  MsgBox("Enter K of VBGF") : Exit Sub
                    Me.m_publisher.SendMessage(
                            New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.STANZA_KinVGBF_MISSING, Me.m_stanza.StanzaName(isp)),
                            eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning))
                    Return False
                End If

                'Calculate how many month this group is to reach 90% of rel. Winf; K is monthly
                'Since wa = (1-exp(-Ka))^3 we have a =
                'Second(Stanza) = Log(1 - 0.9 ^ (1 / 3)) / (-vbK / 12)
                Second(Stanza) = CInt(40.3978 / vbK)   'this is the same as above, to avoid integer division overrun
                If Second(Stanza) < first(Stanza) + 12 Then Second(Stanza) = first(Stanza) + 12
                If Second(Stanza) > cCore.MAX_AGE Then Second(Stanza) = cCore.MAX_AGE
                'above limit ok for vbk>=0.1 (most cases)
                'However when calculating treat the last as a plus group

                ReDim Consumption(Stanza)
                'Dimension based on number of months included for this group:
                ReDim Survive(Second(Stanza))

                'Calculate weight at age
                Me.m_stanza.vBM(isp) = 1 - 3 * vbK / 12
                For Age = 0 To Second(Stanza)         'weight will be zero if age is zero!!!!!
                    'm_stanza.SplitWage(isp, Age) = (1 - Math.Exp(-vbK * (Age + 0.5F) / 12)) ^ 3
                    Me.m_stanza.SplitWage(isp, Age) = CSng((1 - Math.Exp(-vbK * Age / 12)) ^ 3)
                    Me.m_stanza.WWa(isp, Age) = CSng(Me.m_stanza.SplitWage(isp, Age) ^ (2 / 3))
                Next

                'Estimate monthly survival rate, Sa, for age a, from Z estimates by age range
                Survive(0) = 1
                PrevSurv = 1
                For Grp = 1 To Stanza
                    'get the mortality rate that is to be used for the previous age cohort
                    Surv = CSng(Math.Exp(-(Z(Grp) + BaB) / 12))
                    If Surv > 0 Then
                        'For the first month use the survival for the previous stanza:
                        If first(Grp) > 0 Then
                            Survive(first(Grp)) = Survive(first(Grp) - 1) * PrevSurv
                            'Then use this one for the rest of the months in the stanza:
                        End If
                        For Age = first(Grp) + 1 To Second(Grp)
                            Survive(Age) = Survive(Age - 1) * Surv
                        Next
                        PrevSurv = Surv
                    End If
                Next
                'VILLY: DO NOT REMOVE FOLLOWING ACCUMULATOR CALCULATION
                'FOR LAST AGE
                If Surv < 1 Then Survive(Age - 1) = Survive(Age - 1) / (1 - Surv)
                Dim Ahat As Single, Rhat As Single, AhatC As Single, RhatC As Single
                Rhat = (Me.m_stanza.SplitWage(isp, Second(Stanza)) - Me.m_stanza.SplitWage(isp, Second(Stanza) - 1)) / (Me.m_stanza.SplitWage(isp, Second(Stanza) - 1) - Me.m_stanza.SplitWage(isp, Second(Stanza) - 2))
                Ahat = Me.m_stanza.SplitWage(isp, Second(Stanza)) - Rhat * Me.m_stanza.SplitWage(isp, Second(Stanza) - 1)
                Me.m_stanza.SplitWage(isp, Second(Stanza)) = (Surv * Ahat + (1 - Surv) * Me.m_stanza.SplitWage(isp, Second(Stanza))) / (1 - Rhat * Surv)
                Me.AhatStanza(isp) = Ahat
                Me.RhatStanza(isp) = Rhat
                RhatC = (Me.m_stanza.WWa(isp, Second(Stanza)) - Me.m_stanza.WWa(isp, Second(Stanza) - 1)) / (Me.m_stanza.WWa(isp, Second(Stanza) - 1) - Me.m_stanza.WWa(isp, Second(Stanza) - 2))
                AhatC = Me.m_stanza.WWa(isp, Second(Stanza)) - Rhat * Me.m_stanza.WWa(isp, Second(Stanza) - 1)
                Me.m_stanza.WWa(isp, Second(Stanza)) = (Surv * AhatC + (1 - Surv) * Me.m_stanza.WWa(isp, Second(Stanza))) / (1 - RhatC * Surv)

                ' System.Console.WriteLine(isp.ToString & ", " & Stanza.ToString & ", " & "Ahat,Rhat=" & Ahat.ToString & ", " & Rhat.ToString)

                'So now we have the monthly survival rate
                'Get the recruitment, start with base stanza
                SumB = 0
                For Age = first(BaseStanza) To Second(BaseStanza)
                    SumB = SumB + Survive(Age) * Me.m_stanza.SplitWage(isp, Age)
                Next
                'If BaseStanza = Stanza Then 'make the plus group up to 99% of Winf:
                '    i = -Log(0.01) / (3 * vbK / 12)
                '    For Age = Age2(isp, BaseStanza) To i
                '        SumB = SumB + Survive(Age2(isp, BaseStanza)) * (1 - Exp(-vbK * Age / 12)) ^ 3
                '    Next
                'End If

                'jb was
                ' If SumB <= 0 Then Stop : GoTo exitSub
                ' Debug.Assert(SumB > 0, "CalculateStanzaParameters SumB = 0")
                If SumB <= 0 Then
                    Dim msg As String = "Biomass for one of your stanza groups < 0, Please check"
                    Me.m_publisher.SendMessage(New cMessage(msg, eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Critical, eDataTypes.Stanza))
                    Return False
                End If

                Recruits = Bio(BaseStanza) / SumB
                Me.m_stanza.RzeroS(isp) = CSng(Recruits * Math.Exp(BaB / 12))
                'Get the number at age for each monthly cohort:
                For Age = 0 To Second(Stanza)
                    Me.m_stanza.SplitNo(isp, Age) = Recruits * Survive(Age)
                Next
                'Calculate the biomass by stanza:
                Dim biot As Single
                For Grp = 1 To Stanza
                    If Grp <> BaseStanza Then   'already calculated biomass for the basestanza
                        biot = 0
                        For Age = first(Grp) To Second(Grp)
                            biot = biot + Me.m_stanza.SplitNo(isp, Age) * Me.m_stanza.SplitWage(isp, Age)
                        Next
                        Bio(Grp) = biot
                        'If DoWhat = "Insert split" Then
                        '    SetCellValue(frmGrpStanza.vaStanza, 3, Grp, Format(Bio(Grp), GenNum))
                        '    SetGridBlockFormat(frmGrpStanza.vaStanza, 3, Grp, 3, Grp, True, BlockColor, HighlightColor)
                        'End If
                    End If
                Next
                'Then calculate consumption for the group where Cons/Biomass has been entered:
                K = 0   'temporarily use k to sure the sum:
                For Age = first(BaseCB) To Second(BaseCB)
                    K = K + Me.m_stanza.SplitNo(isp, Age) * Me.m_stanza.WWa(isp, Age)
                Next


                If K > 0 Then K = cb(BaseCB) * Bio(BaseCB) / K 'THIS IS THE REAL CONSTANT k
                For Grp = 1 To Stanza
                    Bat(Grp) = BaB * Bio(Grp)
                    If Grp <> BaseCB Then
                        Consumption(Grp) = 0
                        For Age = first(Grp) To Second(Grp)
                            Consumption(Grp) = Consumption(Grp) + Me.m_stanza.SplitNo(isp, Age) * Me.m_stanza.WWa(isp, Age)
                        Next
                        Consumption(Grp) = K * Consumption(Grp)
                        'VC: We don't really need to store the Consumption(), but I'm keeping it as a separate variable in case CJW needs it in Ecosim
                        If Bio(Grp) > 0 Then cb(Grp) = Consumption(Grp) / Bio(Grp)
                        'If DoWhat = "Insert split" Then
                        '    SetCellValue(frmGrpStanza.vaStanza, 5, Grp, Format(cb(Grp), GenNum))
                        '    SetGridBlockFormat(frmGrpStanza.vaStanza, 5, Grp, 5, Grp, True, BlockColor, HighlightColor)
                        'End If
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Throw New ApplicationException("CalculateStanzaParameters() iStanza=" & isp & " Error: " & ex.Message, ex)
            End Try

            Return True
            'VC: Carl had something about conversion efficiency here, but can't make head/tails.

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="iGroup"></param>
        ''' <param name="PotGrowth">Potential growth values to use.</param>
        ''' <param name="FWMax">FWMax values to use</param>
        ''' <param name="estimated">Two-dimensional array with estmated vulnerabilities.</param>
        Public Function EstimateVulnerabilities(ByVal iGroup As Integer,
                                                ByRef PotGrowth As Single, ByRef FWMax As Single,
                                                ByVal estimated As Single()) As Boolean

            Try

                ' Calculate PotGrowth if needed
                If PotGrowth <= 0 Then
                    If Me.m_Data.Fish1(iGroup) = 0 Then
                        PotGrowth = cCore.NULL_VALUE
                    Else
                        PotGrowth = 2.0!
                    End If
                End If

                ' Calculate FWMax if needed
                If (FWMax <= 0) Then
                    If Me.m_Data.Fish1(iGroup) = 0 Then
                        FWMax = 1.2!
                    Else
                        If (Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) > 0) Then
                            FWMax = CSng(1.1 * Me.m_Data.Fish1(iGroup) / (Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)))
                        Else
                            FWMax = cCore.NULL_VALUE
                        End If
                    End If
                End If

                ' Estimate Vs
                If (Me.m_Data.Fish1(iGroup) > 0) And (Me.m_Data.SimGE(iGroup) > 0) Then
                    estimated(0) = Me.VulBo(PotGrowth, iGroup, True)
                    estimated(1) = Me.VulBo(PotGrowth, iGroup, False)
                Else
                    estimated(0) = cCore.NULL_VALUE
                    estimated(1) = cCore.NULL_VALUE
                End If

                If (FWMax > 0) Then
                    estimated(2) = Me.VulFmax(FWMax, iGroup, True)
                    estimated(3) = Me.VulFmax(FWMax, iGroup, False)
                Else
                    estimated(2) = cCore.NULL_VALUE
                    estimated(3) = cCore.NULL_VALUE
                End If

            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

        Private Sub CalcMo()
            Dim i As Integer ', gro
            'mo should be close to zero for juvenile groups;
            'this is not considered here.
            For i = 1 To Me.m_EPData.NumLiving
                Me.m_Data.mo(i) = (1 - Me.m_EPData.EE(i)) * Me.pbbase(i)
            Next
            For i = Me.m_EPData.NumLiving + 1 To Me.nGroups  'N1
                Me.m_Data.mo(i) = 0
            Next
        End Sub


        Private Sub DefaultMigrationAndToDetritus()
            Dim i As Integer, j As Integer
            Dim ToDetritus As Single, deteat As Single
            Dim eat As Single

            ToDetritus = 0
            For i = 1 To Me.nGroups
                If Me.m_EPData.GS(i) < 0 Then Me.m_EPData.GS(i) = 0.2 'Assimilation loss
                ToDetritus = ToDetritus + Me.m_Data.mo(i) * Me.m_Data.StartBiomass(i)
                eat = 0
                For j = 1 To Me.nGroups
                    eat = eat + Me.m_Data.Consumption(j, i)   'eaten by i of j
                Next
                ToDetritus = ToDetritus + eat * Me.m_EPData.GS(i)
            Next
            deteat = 0 : For i = 1 To Me.nGroups : deteat = deteat + Me.m_Data.Consumption(Me.nGroups, i) : Next

        End Sub


        Public Sub SetupSimVariables()
            Dim i As Integer, j As Integer

            'jb SEmult not used in EcoSim
            'ReDim SEmult(mEPData.NumGear)
            'For i = 1 To mEPData.NumGear : SEmult(i) = 1 : Next

            For i = 1 To Me.nGroups
                Me.pbbase(i) = Me.m_EPData.PB(i)
                If Me.m_EPData.PP(i) = 1 Then Me.m_Data.SimGE(i) = 0 'Else pbbase(i) = 0
                If Me.m_Data.SimGE(i) < 0 Then Me.m_Data.SimGE(i) = 0
                If i >= Me.m_EPData.NumLiving + 1 Then Me.pbbase(i) = 0 : Me.m_Data.SimGE(i) = 1
                If Me.m_Data.PBmaxs(i) <= 0 Then Me.m_Data.PBmaxs(i) = 2

                Me.m_Data.pbm(i) = Me.m_Data.PBmaxs(i) * Me.pbbase(i)

                If i <= Me.m_EPData.NumLiving Then
                    If Me.m_Data.StartBiomass(i) > 0 Then
                        Me.m_Data.Fish1(i) = Me.m_EPData.fCatch(i) / Me.m_Data.StartBiomass(i)
                        If Me.m_Data.FishRateMax(i) = 0 Then Me.m_Data.FishRateMax(i) = 4 * Me.m_Data.Fish1(i)
                        If Me.m_Data.FishRateMax(i) = 0 Then Me.m_Data.FishRateMax(i) = 1 'PB(i)   '1
                        Me.m_Data.Emig(i) = Me.m_EPData.Emigration(i) / Me.m_Data.StartBiomass(i)
                    End If
                    'Immig(i) is already defined in Ecopath
                    'is a flow not a rate relative to biomass
                Else    'for detriuts
                    Me.m_Data.Fish1(i) = 0
                    Me.m_Data.FishRateMax(i) = 1
                    Me.m_Data.Emig(i) = If(Me.m_EPData.Emig(i) > 0 And Me.m_EPData.Emigration(i) = 0, Me.m_EPData.Emig(i), Me.m_EPData.Emigration(i) / Me.m_Data.StartBiomass(i))
                    'jb Don't overwrite Immigration with Imported Detritus
                    ' m_EPData.Immig(i) = m_EPData.DtImp(i)
                End If
            Next
            'If there are fishing rates over time then use them for scaling
            For i = 1 To Me.m_EPData.NumLiving
                For j = 1 To Me.m_Data.NTimes
                    If Me.m_Data.FishRateNo(i, j) > Me.m_Data.FishRateMax(i) Then Me.m_Data.FishRateMax(i) = CSng(1.2 * Me.m_Data.FishRateNo(i, j))
                Next
            Next
            For i = Me.m_EPData.NumLiving + 1 To Me.nGroups
                Me.m_Data.pbm(i) = 0 : Me.pbbase(i) = 0.0000000001
            Next i

            For i = 1 To Me.nGroups     'prey
                For j = 1 To Me.nGroups 'pred
                    'If simDC(i, j) > 0 Then
                    Me.setvulratecell(i, j, Me.m_Data.VulMult(i, j))
                Next
            Next

            If cCore.USE_SHARED_ARENAS Then
                ' Datasource may have missed entries for newly created diet links
                If (Not Me.m_Data.ValidateSharedArenas()) Then
                    Dim msg As cMessage = New cMessage("Shared arenas have been reset", eMessageType.DataModified, eCoreComponentType.Core, eMessageImportance.Warning)
                    Me.m_publisher.SendMessage(msg)
                End If
            Else
                Me.DefaultPeatArena()
            End If

            ' cLog.WriteMatrixToFile("VulRate EwE6.csv", m_Data.vulrate, "Vul rate")

        End Sub

        Public Sub setvulratecell(ByVal i As Integer, ByVal j As Integer, ByVal multiplier As Single)
            ' Dim RescaledMultiplier As Single
            'i=prey, j = predator
            'Rescale the multiplier VC 11 May 98 from the [0,1] range
            'to CJW's range of 1 to 20 or more with 4 as mixed flowcontrol
            'If multiplier < 1.1947055 Then
            '    RescaledMultiplier = Exp(2 * (Exp(multiplier + 0) - 1))
            'Else
            '    RescaledMultiplier = 100#
            'End If
            'RescaledMultiplier = Exp(2.1 * (Exp(CDbl(multiplier)) - 1.3)) + 0.47

            ' Debug.Assert((j = 12 And i = 5) = False)

            If Me.m_Data.SimDC(j, i) > 0 Or (i = j And Me.m_EPData.PP(i) = 1) Then
                'maxflow(i, j) = RescaledMultiplier * Consumption(i, j)
                Me.m_Data.maxflow(i, j) = multiplier * Me.m_Data.Consumption(i, j)
                If Me.m_Data.maxflow(i, j) = 0 Then Me.m_Data.maxflow(i, j) = 1
                'If StartBiomass(i) > 0 Then vulrate(i, j) = RescaledMultiplier * Consumption(i, j) / StartBiomass(i)
                'If StartBiomass(i) > 0 Then vulrate(i, j) = multiplier * Consumption(i, j) / StartBiomass(i)
                'prey
                'VC040708: if no biomass for detritus is given, then the below may explode, will put a trap
                If multiplier > 100 And Me.m_Data.StartBiomass(i) < 10 ^ -10 And i > Me.m_EPData.NumLiving Then
                    Me.m_Data.vulrate(i, j) = 1

                ElseIf Me.m_Data.StartBiomass(i) > 0 Then
                    Me.m_Data.vulrate(i, j) = CSng(multiplier * Me.m_Data.Consumption(i, j) / Me.m_Data.StartBiomass(i))

                End If

                If Me.m_Data.vulrate(i, j) = 0 Then Me.m_Data.vulrate(i, j) = 2

            Else
                Me.m_Data.vulrate(i, j) = 1
            End If
        End Sub

        ''' <summary>
        ''' Initialization routine for: Eatenby, Eatenof, Consumption, inlinks, ilinks and jlinks
        ''' </summary>
        ''' <remarks>Eatenby, Eatenof and Consumption are initialized as a function of StartBiomass QB and DC</remarks>
        Friend Sub CalcEatenOfBy()

            Dim i As Integer, j As Integer, ii As Integer

            'The Diet Matrix could have changed since the last call to this
            'so count the links before populating them

            ReDim Me.m_Data.Eatenby(Me.nGroups)
            ReDim Me.m_Data.Eatenof(Me.nGroups)

            'Compute Consumption and count the number of links
            Me.m_Data.inlinks = 0
            For j = 1 To Me.m_EPData.NumLiving      'all living groups; consumers
                For i = 0 To Me.nGroups  'prey
                    Me.m_Data.Consumption(i, j) = Me.m_Data.StartBiomass(j) * Me.m_EPData.QB(j) * Me.m_EPData.DC(j, i)

                    If Me.m_Data.Consumption(i, j) > 0 And i > 0 Then
                        Me.m_Data.inlinks = Me.m_Data.inlinks + 1
                    End If
                    Me.m_Data.Eatenof(i) = Me.m_Data.Eatenof(i) + Me.m_Data.Consumption(i, j)
                    Me.m_Data.Eatenby(j) = Me.m_Data.Eatenby(j) + Me.m_Data.Consumption(i, j)
                Next i
            Next j


            'Now populate the i and j links
            'With the number of links from above
            Me.m_Data.ilink = New Integer(Me.m_Data.inlinks) {}
            Me.m_Data.jlink = New Integer(Me.m_Data.inlinks) {}

            Me.m_Data.inlinks = 0


            'For j = 1 To m_EPData.NumLiving      'all living groups; consumers
            '    For i = 1 To nGroups  'prey
            '    jb swaped the loading order To match all other prey, pred loops
            'Debug.Assert(False, "JB CalcEatenOfBy")
            For i = 1 To Me.nGroups     'all living groups; consumers
                For j = 1 To Me.nGroups  'prey

                    If Me.m_Data.Consumption(i, j) > 0 Then
                        Me.m_Data.inlinks = Me.m_Data.inlinks + 1
                        Me.m_Data.ilink(Me.m_Data.inlinks) = i
                        Me.m_Data.jlink(Me.m_Data.inlinks) = j
                    End If
                Next
            Next

            ' Set arenas here. They do not change anymore
            ' Set up list of foraging arenas defined by nonzero trophic flows
            ' First count number of arenas
            Me.m_Data.Narena = 0
            For i = 1 To Me.nGroups : For j = 1 To Me.m_EPData.NumLiving
                    If Me.m_Data.Consumption(i, j) > 0 Then Me.m_Data.Narena = Me.m_Data.Narena + 1
                Next
            Next
            ReDim Me.m_Data.Iarena(Me.m_Data.Narena), Me.m_Data.Jarena(Me.m_Data.Narena), Me.m_Data.ArenaNo(Me.nGroups, Me.nGroups)

            'then assign arenas to linear list
            ii = 0
            For i = 1 To Me.nGroups
                For j = 1 To Me.nGroups
                    If Me.m_Data.Consumption(i, j) > 0 Then
                        ii = ii + 1
                        Me.m_Data.Iarena(ii) = i
                        Me.m_Data.Jarena(ii) = j
                        Me.m_Data.ArenaNo(i, j) = ii
                    End If
                Next
            Next
            Me.m_Data.Narena = ii

        End Sub

        Friend Sub redimTime(ByVal nYears As Integer, ByVal DontPreserve As Boolean)
            'Dim MaxTime As Integer
            Debug.Assert(nYears <> 0, Me.ToString & ".RedimTotalTimeVariables() TotalTime = 0 Something is very wrong......")
            Dim nts As Integer = nYears * Me.m_Data.NumStepsPerYear
            'Ntimes = MaxTime
            If DontPreserve Then
                ReDim Me.Bstore(Me.nGroups, nts)   'was 1200
            Else
                ReDim Preserve Me.Bstore(Me.nGroups, nts) 'was 1200
            End If

        End Sub

        ''' <summary>
        ''' Hardwire some default values for the EcoSim Model
        ''' </summary>
        ''' <remarks>
        ''' In the original code this was called "EcoSimFileOpen()"
        ''' this has been restructured to just set Defaults for the EcoSim model
        ''' defaults that belong to eEcoSimDatastructures are set in cEcoSimDatastructures.SetDefaultParameters()
        ''' </remarks>
        Public Sub SetDefaultParameters()
            'read ini file stored defaults here 
            'at this time there is no mechanisim for storing defaults 
            'so I have just hardwired the same values as are in the default ini file 
            'see original code "SetupParametersRead()"

            Me.StepsPerYear = 12

            Dim i As Integer, j As Integer
            Me.IsDatWtSet = False

            'redim variables that need defaults before they are read from the database
            'this means these variables are not dimensioned with the other data during the database read
            ReDim Me.m_Data.FlowType(Me.nGroups, Me.nGroups)
            ReDim Me.m_Data.Cbase(Me.nGroups)
            ReDim Me.m_Data.FtimeMax(Me.nGroups)
            ReDim Me.m_search.FLimit(Me.nGroups)

            ReDim Me.m_Data.PaddP(Me.nGroups)
            ReDim Me.m_Data.MoPredBase(Me.nGroups)
            ReDim Me.m_Data.moTot(Me.nGroups)
            ReDim Me.m_Data.moMax(Me.nGroups)
            ReDim Me.m_Data.Qh(Me.nGroups)
            'jb 11-jan-2024 PhHalf is not use in the new implementation of additive mortality
            'ReDim Me.m_Data.PhHalf(Me.nGroups)


            'default from frmOptF.Form_Load()
            For igrp As Integer = 1 To Me.nGroups
                Me.m_search.FLimit(igrp) = 1000
            Next

            'ReDim GearIncludeInEquil(m_EPData.NumFleet)
            ''vc What if no fishery? If mEPData.NumGear < 1 Then mEPData.NumGear = 1
            'For i = 1 To m_EPData.NumFleet
            '    GearIncludeInEquil(i) = True
            'Next

            For i = 1 To Me.nGroups     'prey
                If (Me.m_EPData.vbK(i) <= 0 Or Me.m_EPData.vbK(i) = CSng(0.3)) And Me.m_EPData.PP(i) < 1 And Me.m_EPData.StanzaGroup(i) = False Then 'vbK(i) = vbK(0) '0.1
                    '041210VC: Carl wrote:
                    'if we "hardwire" the typical A=0.6 into the relationship, need not make user enter K for every species,
                    'since can use an apparent K*=Zo/3[0.6/GEo-1] where Zo and GEo are the present ecopath base input values.
                    Me.m_EPData.vbK(i) = CSng(Me.m_EPData.PB(i) / 3 * (0.6 / Me.m_EPData.GE(i) - 1))
                End If
                For j = 1 To Me.nGroups 'consumer; Doesn't make much sense for detritus
                    Me.m_Data.FlowType(i, j) = 2
                    'jb this was in the original code but it does not make sense here as VulMult() has been read from the database
                    'and this will overwrite the values with the default
                    'If m_Data.SimDC(j, i) > 0 Or (i = j And m_EPData.PP(i) = 1) Then
                    '    m_Data.VulMult(i, j) = m_Data.VulMultAll
                    'End If

                Next
            Next

            ' Loop for Groups
            For i = 1 To Me.nGroups
                If i <= Me.m_EPData.NumLiving Then
                    'SimQB(i) = m_EPData.QB(i) 'SimQB() will be set again in RemoveImportFromEcosim
                    Me.m_Data.Cbase(i) = Me.m_EPData.QB(i)
                End If
                Me.m_Data.PaddP(i) = 1.0
                If Me.m_Data.Cbase(i) <= 0 Then
                    Me.m_Data.Cbase(i) = 1
                    Me.m_Data.FtimeMax(i) = 1
                    'percent of predation mortality that is additive

                    Me.m_Data.MoPredBase(i) = 1

                End If
            Next

        End Sub



        Friend Sub InitAssessment()
            'Dim totalQuota() As Single
            'Dim iFlt As Integer, iGrp As Integer
            'Dim ngear As Integer = m_Data.nGear

            'Dim mseData As cMSEDataStructures = Me.m_MSE.Data

            'For iGrp = 1 To nGroups
            '    mseData.Bestimate(iGrp) = m_Data.StartBiomass(iGrp) * Math.Exp(Me.m_Quota.CVest(iGrp) * RandomNormal())
            '    mseData.BestimateLast(iGrp) = mseData.Bestimate(iGrp)
            'Next iGrp

            'ReDim totalQuota(nGroups)
            'For iFlt = 1 To ngear
            '    For iGrp = 1 To nGroups
            '        If (m_EPData.Landing(iFlt, iGrp) + m_EPData.Discard(iFlt, iGrp)) > 0 Then
            '            totalQuota(iGrp) = totalQuota(iGrp) + Me.m_Quota.Quota(iFlt, iGrp)
            '        End If
            '    Next
            'Next

            'For iFlt = 1 To ngear
            '    For iGrp = 1 To nGroups
            '        If (m_EPData.Landing(iFlt, iGrp) + m_EPData.Discard(iFlt, iGrp)) > 0 Then
            '            Me.m_Quota.QuotaTime(iFlt, iGrp) = Me.m_Quota.Quota(iFlt, iGrp)
            '            Me.m_Quota.Quotashare(iFlt, iGrp) = Me.m_Quota.Quota(iFlt, iGrp) / (totalQuota(iGrp) + 0.0000000001)
            '        End If
            '    Next
            'Next

        End Sub



        Private Sub DefaultDF()
            Dim i As Integer
            'In OpenFile the statements below are included
            'but not read if no BA matrix (will go to clos)
            '***VC remember above
            If Me.nGroups = Me.m_EPData.NumLiving + 1 Then
                For i = 1 To Me.m_EPData.NumLiving
                    Me.m_EPData.DF(i, 1) = 1              'All detritus sent to the single det. box
                Next i
                Me.m_EPData.DF(Me.nGroups, 1) = 0            'All for export
            End If

        End Sub

        ''' <summary>
        ''' Init Biomass to Ecopath values during initialization
        ''' </summary>
        ''' <remarks>Warning: <see cref="cMSEDataStructures.setDefaultRegValues"> </see>cMSEDataStructures.setDefaultRegValues() uses Ecopath.B() instead of Ecosim.StartBiomass() because of initialization order. </remarks>
        Public Sub SetStartBiomass()
            Dim i As Integer ', TotalBiomass As Single

            ' Warning: if you make any changes to StartBiomass() you must make the same changes to QuotaDataStructures.setDefaultRegValues()
            'where it uses Ecopath.B() instead of Ecosim.StartBiomass()
            For i = 1 To Me.nGroups
                Me.m_Data.StartBiomass(i) = Me.m_EPData.B(i)
                Me.BB(i) = Me.m_EPData.B(i)
            Next i

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            ''jb from original code
            ''TotalBiomass is local and it's not used to update anything after it is set 
            ''so the commented out code had no effect

            ''set default parameters to assure mass balance in detritus pool
            'TotalBiomass = 0
            'For i = 1 To mEPData.NumLiving
            '    TotalBiomass = TotalBiomass + StartBiomass(i)
            'Next i
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'set a reasonably large value for detritus biomass if a very small or zero value has been entered by ecopath user
            For i = Me.m_EPData.NumLiving + 1 To Me.nGroups
                If Me.m_Data.StartBiomass(i) < 1.0E-20 Then
                    Me.m_Data.StartBiomass(i) = 1.0E-20
                    Me.BB(i) = Me.m_Data.StartBiomass(i)
                End If
            Next

        End Sub

        Friend Sub RemoveImportFromEcosim()
            Dim i As Integer, j As Integer
            Dim FractionWOimport As Single
            Try
                ReDim Me.m_Data.QBoutside(Me.nGroups)
                'VC210898   To take care of import (a part of the diet composition):
                '           Rescale diet so as not to incorporate import:
                For j = 1 To Me.m_EPData.NumLiving      'all living groups

                    If Me.m_EPData.DC(j, 0) > 0 Then
                        FractionWOimport = (1 - Me.m_EPData.DC(j, 0) / 1) 'There is import
                    Else
                        FractionWOimport = 1
                    End If

                    For i = 1 To Me.nGroups 'prey
                        'Next cause an overflow if Fraction WOImport=0, therefore a trap
                        If FractionWOimport = 0 Then
                            Me.m_Data.SimDC(j, i) = 0
                        Else
                            Me.m_Data.SimDC(j, i) = Me.m_EPData.DC(j, i) '/ FractionWOimport
                        End If
                    Next
                    'Also rescale the QB = consumption /biomass ratio
                    Me.SimQB(j) = Me.m_EPData.QB(j) ' * FractionWOimport
                    Me.m_Data.QBoutside(j) = Me.m_EPData.QB(j) * (1 - FractionWOimport)
                    If Me.SimQB(j) > 0 Then
                        Me.m_Data.SimGE(j) = Me.m_EPData.PB(j) / Me.m_EPData.QB(j)
                    Else
                        Me.m_Data.SimGE(j) = 0
                    End If
                    'If only import there will be no QB
                Next

            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

        End Sub


        Public Sub ApplySalinityModifier(ByRef A As Single, ByVal CurVal As Single,
                                         ByVal Optim As Single, ByVal StdLeft As Single, ByVal StdRight As Single)
            Dim Mult As Single

            If CurVal < Optim Then
                Mult = CSng(Math.Exp(-0.5 * ((CurVal - Optim) / (StdLeft + 0.0000001)) ^ 2))
            Else
                Mult = CSng(Math.Exp(-0.5 * ((CurVal - Optim) / (StdRight + 0.0000001)) ^ 2))
            End If

            ' Mult = Math.Exp(-0.5 * ((Sal - m_Data.EnvResponseOpt(1, j)) / (m_Data.SdSal(j) + 0.0000001)) ^ 2)
            A = A * Mult

        End Sub


        '***********************
        'THIS FUNCTION IS COPIED IN cSpaceSolver.vb
        'Changes here will NOT copy over to there
        '***********************
        ''' <summary>
        ''' Apply the multi function forcing or mediation functions to 'a'(searchrate) and 'v'(vulnerability)
        ''' </summary>
        ''' <param name="A">SearchRate to modify</param>
        ''' <param name="v">Vulnerability to modify</param>
        ''' <param name="i">i Index</param>
        ''' <param name="j">j Index</param>
        ''' <param name="UseTime">True if the modifier is over time (Ecosim), False if not (Ecospace) </param>
        Public Sub ApplyAVmodifiers(ByVal iTime As Integer, ByRef A As Single, ByRef v As Single, ByRef Mo As Single, ByVal i As Integer, ByVal j As Integer, ByVal UseTime As Boolean)
            Dim K As Integer, Mult As Single

            Me.ApplyEnvironmentalResponse(A, j, iTime)

            Me.ApplyMortalityResponse(Mo, j, iTime)

            For K = 1 To cMediationDataStructures.MAXFUNCTIONS

                If Me.m_Data.BioMedData.FunctionNumber(i, j, K) <= 0 Then Exit Sub

                If Me.m_Data.BioMedData.IsMedFunction(i, j, K) Then
                    Mult = Me.m_Data.BioMedData.MedVal(Me.m_Data.BioMedData.FunctionNumber(i, j, K))
                Else
                    If UseTime = True Then
                        Mult = Me.m_Data.tval(Me.m_Data.BioMedData.FunctionNumber(i, j, K))
                    Else
                        Mult = 1
                    End If
                End If

                Select Case Me.m_Data.BioMedData.ApplicationType(i, j, K)
                    Case eForcingFunctionApplication.SearchRate,
                         eForcingFunctionApplication.ProductionRate
                        A = A * Mult
                        'Debug.Print(Mult.ToString + ", " + A.ToString)
                    Case eForcingFunctionApplication.Vulnerability
                        v = v * Mult
                    Case eForcingFunctionApplication.ArenaArea
                        A = CSng(A / (Mult + 0.0000000001))
                    Case eForcingFunctionApplication.VulAndArea
                        A = CSng(A / (Mult + 0.0000000001))
                        v = v * Mult
                    Case eForcingFunctionApplication.Import
                        A = A * Mult
                    Case eForcingFunctionApplication.MortOther
                        'only drive mortality if the multiplier is not zero
                        If Mult <> 0 Then
                            Mo = Mo * Mult
                        End If
                End Select

            Next

        End Sub

        Private Sub TempDebugMortOther(ByVal i As Integer, ByVal j As Integer)
            'WARNING SET ALL FORCING FUNCTIONS TO MORTOTHER for testing!!!!
            Dim bfound As Boolean
            Dim iIndex As Integer

            For K As Integer = 1 To cMediationDataStructures.MAXFUNCTIONS
                bfound = False
                For ii As Integer = 1 To Me.nGroups

                    If Me.m_Data.BioMedData.FunctionNumber(i, ii, K) > 0 Then
                        bfound = True
                        iIndex = ii
                        Exit For
                    End If
                Next

                If bfound Then
                    System.Console.WriteLine("HACK WARNING: Setting forcing function to MortOther")
                    Me.m_Data.BioMedData.FunctionNumber(i, j, K) = Me.m_Data.BioMedData.FunctionNumber(i, iIndex, K)
                    Me.m_Data.BioMedData.ApplicationType(i, j, K) = eForcingFunctionApplication.MortOther
                End If

            Next K

        End Sub

        Private Sub ApplyEnvironmentalResponse(ByRef A As Single, iPredIndex As Integer, ByVal iTimeStep As Integer)

            For iRes As Integer = 1 To Me.EcosimEnviroResponseManager.nInputData  ' As IEnviroInputData In Me.EcosimEnviroResponseManager.InputData
                Dim ResponseFunction As IEnviroInputData = Me.EcosimEnviroResponseManager.InputData(iRes)
                If ResponseFunction.IsCapacityEnabled Then

                    If ResponseFunction.ResponseIndexForGroup(iPredIndex) > 0 Then
                        A *= ResponseFunction.ResponseFunction(iPredIndex, iTimeStep)
                    End If ' map.ResponseIndexForGroup(igrp) > 0

                End If

            Next iRes

        End Sub


        Private Sub ApplyMortalityResponse(ByRef Mo As Single, iPredIndex As Integer, ByVal iTimeStep As Integer)
            Dim propMort As Single

            Try

                If Me.m_Data.mo(iPredIndex) = 0.0F Then
                    'if mo is zero applying a multiplier will have no effect
                    'so just save time and return 1.0
                    Mo = 1.0F
                    Return
                End If

                'Get the proportion of total mortality as a function of environmental driver
                For iRes As Integer = 1 To Me.EcosimMortalityResponseManager.nInputData  ' As IEnviroInputData In Me.EcosimEnviroResponseManager.InputData
                    Dim ResponseFunction As IEnviroInputData = Me.EcosimMortalityResponseManager.InputData(iRes)
                    If ResponseFunction.IsCapacityEnabled Then

                        If ResponseFunction.ResponseIndexForGroup(iPredIndex) > 0 Then
                            propMort += ResponseFunction.ResponseFunction(iPredIndex, iTimeStep)
                            'Exit For
                        End If ' map.ResponseIndexForGroup(igrp) > 0

                    End If

                Next iRes

                'convert proportion to instantaneous mortality
                If propMort >= 1.0 Then propMort = 0.9999F
                'instantaneous mortality in Ecopath annual units, this should be the same as Ecopath mo units
                Dim lnPropMort As Single = CSng(-Math.Log(1 - propMort)) * 12
                'multiplier to scale mo up to the new value
                Mo = (lnPropMort + Me.m_Data.mo(iPredIndex)) / Me.m_Data.mo(iPredIndex)

            Catch ex As Exception
                Mo = 1.0F
            End Try


        End Sub

        ''' <summary>
        ''' Populates FishRateNo(group,time) with base fishing mortality rates for all time steps
        ''' </summary>
        ''' <remarks>
        ''' Fishing mortality rates include fishing effort, density dependent catchability and discard mortality rates. 
        ''' Called to initialize <see cref="cEcosimDatastructures.FishRateNo">FishRateNo(group,time)</see> or when effort changes.
        ''' </remarks>
        Public Sub SetBaseFFromGear()
            'calculates Fishrateno by group from fishing efforts except for groups flagged
            'to be forced by group F over time using csv file (fisforced(i)=true if i is forced in csv)
            Dim t As Integer

            Dim QYear() As Single
            ReDim QYear(Me.EcopathData.NumFleet)
            For i As Integer = 1 To Me.EcopathData.NumFleet
                'Dec 2011: VC changed dimentsion from me.nGroups to Numfleet
                QYear(i) = 1
            Next

            Me.setDenDepCatchMult(Me.m_Data.StartBiomass)

            For t = 1 To Me.m_Data.NTimes
                Me.SetFtimeFromGear(t, QYear, True)
            Next

        End Sub

        ''' <summary>
        ''' Sets Qmult(ngroups) (density dependent catchability multiplier) to biomass(ngroups) 
        ''' </summary>
        ''' <param name="BB">Biomass use to set Qmult()</param>
        ''' <remarks>Called at each time step to set density dependent catchability to biomass at the current time step</remarks>
        Friend Sub setDenDepCatchMult(ByVal BB() As Single)

            ReDim Me.Qmult(Me.m_Data.nGroups)
            'Density dependent catchability recalculation
            For igrp As Integer = 1 To Me.m_Data.nGroups
                Me.Qmult(igrp) = Me.m_Data.QmQo(igrp) / (1 + (Me.m_Data.QmQo(igrp) - 1) * BB(igrp) / Me.m_Data.StartBiomass(igrp))
            Next

        End Sub

        ''' <summary>
        ''' Populates FishTime(ngroups) and FishRateNo(group,time) with fishing mortality rates for a timestep from the current Biomass, Effort
        ''' </summary>
        ''' <param name="BB">Biomass(group).</param>
        ''' <param name="t">Timestep.</param>
        ''' <param name="PredEffort">Boolean flag True if effort being predicted, False otherwise.</param>
        ''' <remarks>
        ''' FishTime() is the total fishing mortality used by Ecosim in Deritt
        ''' </remarks>
        <Obsolete("Replaced by SetFtimeFromGear(t, q(), PredEffort) still in place to support legacy code.")>
        Friend Sub SetFtimeFromGear(ByVal BB() As Single, ByVal t As Integer, ByVal QYear() As Single, ByVal PredEffort As Boolean)

            Me.SetFtimeFromGear(t, QYear, PredEffort)

        End Sub



        ''' <summary>
        ''' Populates FishTime(ngroups) and FishRateNo(group,time) with fishing mortality rates for a timestep from the current Biomass, Effort
        ''' </summary>
        ''' <param name="t">Timestep.</param>
        ''' <param name="PredEffort">Boolean flag True if effort being predicted, False otherwise.</param>
        ''' <remarks>
        ''' FishTime() is the total fishing mortality used by Ecosim in Deritt
        ''' </remarks>
        Friend Sub SetFtimeFromGear(ByVal t As Integer, ByVal QYear() As Single, ByVal PredEffort As Boolean, Optional ForcedDiscards As Boolean = False)
            Dim i As Integer, ig As Integer, Ft As Single

            'ToDo FishMGear() Aug 2015 Bug Fix
            'FishMGear contains only Fishing Mortality. Discards that survived are not included.
            'xxxxxxxxxxxxxxxxxxxxx
            'DONE
            'Make sure Me.m_Data.Propdiscardtime has been adjusted for discards that survive
            '   This happens in InitPropLanded() 
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'Check SimDetritus to make sure it uses discards correctly

            Me.m_Data.SetRelQToT(t, True)

            'fishing mortality at the current effort
            For i = 1 To Me.m_Data.nGroups
                If ((Me.m_RefData.ForcedFs(i, t) < 0) And PredEffort) Or ForcedDiscards Then

                    Ft = 0
                    For ig = 1 To Me.m_Data.nGear
                        Debug.Assert(Math.Round(Me.m_Data.PropLandedTime(ig, i) + Me.m_Data.PropDiscardTime(ig, i), 3) <= 1.0!,
                                    Me.ToString & ".SetFtimeFromGear() PropLanded + PropDiscarded should not be greater than 1!")
                        'jb 27-June-2014  Propdiscardtime(fleet,group) does not include fish that survived discarding
                        'Debug.Assert(m_Data.relQ(ig, i) = 0)

                        ' js 12-dec-2023: when driving dicard mort, relQ(ig, i) may return -9999
                        Ft = Ft + QYear(ig) * Math.Max(Me.m_Data.relQ(ig, i), 0) * Me.m_Data.FishRateGear(ig, t) * (Me.m_Data.PropLandedTime(ig, i) + Me.m_Data.PropDiscardTime(ig, i))
                        'Ft = Ft + QYear(ig) * m_Data.FishMGear(ig, i) * m_Data.FishRateGear(ig, t) * (Me.m_Data.PropLandedTime(ig, i) + Me.m_Data.Propdiscardtime(ig, i))
                    Next

                    'Save F for this time step 
                    'NOT including Density Dependant Catchability.
                    'This is because Density Dependant Catchability is dependant on B(t) B(0) ratio which we may not know for given t
                    'Density Dependant Catchability will need to be applied during the timestep when FishTime() is populated In SetFishTime()
                    Me.m_Data.FishRateNo(i, t) = Ft

                    'Include Density Dependant Catchability in the F that is applied to the current timestep
                    Me.m_Data.FishTime(i) = Me.m_Data.FishRateNo(i, t) * Me.Qmult(i)
                End If

            Next i

        End Sub

        ''' <summary>
        ''' Populate cEcosimDataStructures.tval(nForcingShapes) with forcing function values/multiplers for this time step
        ''' </summary>
        ''' <param name="iTime">Cumulative monthly index of this timestep</param>
        Friend Sub settval(ByVal iTime As Integer)
            'Set current values for time shape functions
            Try
                For iShp As Integer = 1 To Me.m_Data.NumForcingShapes
                    'jb changed all forcing function data are stored by time 
                    ' m_Data.tval(i) = if(i <= 6 / 2, m_Data.zscale(its, i), m_Data.zscale(itt, i))
                    Me.m_Data.tval(iShp) = Me.m_Data.zscale(iTime, iShp)
                Next

            Catch ex As Exception
                cLog.Write(ex)
                System.Console.WriteLine(Me.ToString & ".settval() Exception: " & ex.Message)
            End Try

        End Sub

        Sub DefineFlowList()

            Debug.Assert(cCore.USE_SHARED_ARENAS)

            ' set up list of foraging arenas defined by nonzero trophic flows
            Dim i As Integer, K As Integer, ii As Integer, iii As Integer


            ' JS 12Jun20: moved arena validation to a pre-run Sim check

            'next count number of nonzero trophic links
            Me.m_Data.inlinks = 0
            For iii = 1 To Me.m_Data.NlinksSet
                'find arena number for this link
                i = Me.m_Data.IlinkSet(iii)
                ii = Me.m_Data.ArenaNo(i, Me.m_Data.JlinkSet(iii))
                K = Me.m_Data.KlinkSet(iii)
                If Me.m_Data.PeatArena(ii, K) > 0 Then 'predator k takes part of its consumption of i from this arena
                    Me.m_Data.inlinks = Me.m_Data.inlinks + 1
                End If
            Next

            ReDim Me.m_Data.Qlink(Me.m_Data.inlinks), Me.m_Data.ilink(Me.m_Data.inlinks), Me.m_Data.jlink(Me.m_Data.inlinks), Me.m_Data.ArenaLink(Me.m_Data.inlinks)
            ReDim Me.m_Data.MPred(Me.m_Data.inlinks)

            'then set list variables by feeding link (note must be at least as many links as arenas)
            Dim Il As Integer = 0
            For iii = 1 To Me.m_Data.NlinksSet
                i = Me.m_Data.IlinkSet(iii)
                ii = Me.m_Data.ArenaNo(i, Me.m_Data.JlinkSet(iii))
                K = Me.m_Data.KlinkSet(iii)
                If Me.m_Data.PeatArena(ii, K) > 0 Then 'predator k takes part of its consumption of i from this arena
                    'Debug.Assert(m_Data.PeatArena(ii, K) = 1.0F)
                    'If m_Data.PeatArena(ii, K) <> 1.0F Then
                    '    Debug.WriteLine("ShArenas: DefineFlowList - Prop: {0}, Arena: {1}, Pred {2}", m_Data.PeatArena(ii, K), ii, K)
                    'End If
                    ''Debug.Assert(i <> 8)
                    Il = Il + 1
                    Me.m_Data.ilink(Il) = i 'Prey for this arena link
                    Me.m_Data.jlink(Il) = K 'Pred for this arena link
                    Me.m_Data.ArenaLink(Il) = ii
                    'Consumption(iPrey,iPred), PeatArena(Arena, Pred)
                    Me.m_Data.Qlink(Il) = Me.m_Data.Consumption(i, K) * Me.m_Data.PeatArena(ii, K)
                End If 'm_Data.PeatArena(ii, K) > 0 
            Next iii

        End Sub

        Sub DefaultPeatArena()

            Debug.Assert(Not cCore.USE_SHARED_ARENAS)

            'sets default peatarena array for models that have not had multiple predators defined to
            'eat in any foraging arenas and no peatarena values have yet been stored in database
            Dim i As Integer, j As Integer, ii As Integer
            ii = 0

            For i = 1 To Me.nGroups
                For j = 1 To Me.nGroups
                    If Me.m_Data.Consumption(i, j) > 0 Then ii = ii + 1
                Next j
            Next i

            Me.m_Data.NlinksSet = ii

            ' JS 13Sep18: Redim already done in cEcosimDataStructures to max no of links
            'ReDim m_Data.PeatArena(m_Data.NlinksSet, nGroups)
            'ReDim m_Data.IlinkSet(m_Data.NlinksSet)
            'ReDim m_Data.JlinkSet(m_Data.NlinksSet)
            'ReDim m_Data.KlinkSet(m_Data.NlinksSet)
            ii = 0

            For i = 1 To Me.nGroups
                For j = 1 To Me.nGroups
                    If Me.m_Data.Consumption(i, j) > 0 Then
                        ii = ii + 1
                        Me.m_Data.IlinkSet(ii) = i
                        Me.m_Data.JlinkSet(ii) = j
                        Me.m_Data.KlinkSet(ii) = j
                        Me.m_Data.PeatArena(ii, j) = 1
                    End If
                Next j
            Next i

        End Sub

        Sub DefineArenasAndFlowList()

            Debug.Assert(Not cCore.USE_SHARED_ARENAS)
            ' set up list of foraging arenas defined by nonzero trophic flows
            Dim i As Integer, j As Integer, K As Integer, ii As Integer, iii As Integer
            'IlinkSet(2) = 3: JlinkSet(2) = 1: KlinkSet(2) = 2: PeatArena(2, 1) = 1  'test inputs for accounting
            'first count number of arenas
            m_Data.Narena = 0
            For i = 1 To nGroups : For j = 1 To m_EPData.NumLiving
                    If m_Data.Consumption(i, j) > 0 Then m_Data.Narena = m_Data.Narena + 1
                Next
            Next
            ReDim m_Data.Iarena(m_Data.Narena), m_Data.Jarena(m_Data.Narena), m_Data.ArenaNo(nGroups, nGroups)

            'then assign arenas to linear list
            ii = 0
            For i = 1 To nGroups
                For j = 1 To nGroups
                    If m_Data.Consumption(i, j) > 0 Then
                        ii = ii + 1
                        m_Data.Iarena(ii) = i
                        m_Data.Jarena(ii) = j
                        m_Data.ArenaNo(i, j) = ii
                    End If
                Next
            Next

            'next check to make sure PeatArena(arena,k) accounts for all i,j consumption rates
            Dim Tcon(,) As Single
            ReDim Tcon(nGroups, nGroups)
            For iii = 1 To m_Data.NlinksSet
                i = m_Data.IlinkSet(iii)
                j = m_Data.KlinkSet(iii)
                Tcon(i, j) = Tcon(i, j) + m_Data.PeatArena(m_Data.ArenaNo(i, m_Data.JlinkSet(iii)), j)
            Next

            For i = 1 To nGroups
                For j = 1 To nGroups
                    If m_Data.Consumption(i, j) > 0 Then
                        If Tcon(i, j) < 1 Then
                            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.ECOSIM_RUN_ERROR_MISSINGPREDATION, m_EPData.GroupName(i), m_EPData.GroupName(j)),
                                                    eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning)
                            Me.m_publisher.AddMessage(msg)
                            'assign remaining consumption by j of i to the i,j arena
                            m_Data.PeatArena(m_Data.ArenaNo(i, j), j) = m_Data.PeatArena(m_Data.ArenaNo(i, j), j) + 1 - Tcon(i, j)
                        End If
                    End If
                Next
            Next

            'next count number of nonzero trophic links
            m_Data.inlinks = 0
            For iii = 1 To m_Data.NlinksSet
                'find arena number for this link
                i = m_Data.IlinkSet(iii)
                ii = m_Data.ArenaNo(i, m_Data.JlinkSet(iii))
                K = m_Data.KlinkSet(iii)
                If m_Data.PeatArena(ii, K) > 0 Then 'predator k takes part of its consumption of i from this arena
                    m_Data.inlinks = m_Data.inlinks + 1
                End If
            Next

            If m_Data.inlinks < m_Data.Narena Then
                ' ToDo: globalize this
                Me.m_publisher.AddMessage(New cMessage("feeding proportions by arenas not set properly",
                                            eMessageType.ErrorEncountered, eCoreComponentType.Ecosim, eMessageImportance.Warning))
                ' ToDo: Handle this properly
                Stop
            End If

            ReDim m_Data.Qlink(m_Data.inlinks), m_Data.ilink(m_Data.inlinks), m_Data.jlink(m_Data.inlinks), m_Data.ArenaLink(m_Data.inlinks)
            ReDim m_Data.MPred(m_Data.inlinks)

            'then set list variables by feeding link (note must be at least as many links as arenas
            Dim Il As Integer
            Il = 0
            For iii = 1 To m_Data.NlinksSet
                i = m_Data.IlinkSet(iii)
                ii = m_Data.ArenaNo(i, m_Data.JlinkSet(iii))
                K = m_Data.KlinkSet(iii)
                If m_Data.PeatArena(ii, K) > 0 Then 'predator k takes part of its consumption of i from this arena
                    Il = Il + 1
                    m_Data.ilink(Il) = i
                    m_Data.jlink(Il) = K
                    m_Data.ArenaLink(Il) = ii
                    m_Data.Qlink(Il) = m_Data.Consumption(i, K) * m_Data.PeatArena(ii, K)
                End If
            Next
        End Sub

        Sub SetArenaVulandSearchRates()
            'this routine sets vulrates by arena and feeding a's by trophic link after arena and trophic link lists have been set
            'by routine DefineArenaVulandSearcRates
            Dim i As Integer, j As Integer, ii As Integer, ia As Integer
            Dim Qarena() As Single, VulBiom() As Single

            ReDim Me.m_Data.VulArena(Me.m_Data.Narena), Me.m_Data.Alink(Me.m_Data.inlinks)

            'find total consumptions of prey type for each arena, added over predators
            ReDim Qarena(Me.m_Data.Narena), VulBiom(Me.m_Data.Narena)
            For ii = 1 To Me.m_Data.inlinks
                ia = Me.m_Data.ArenaLink(ii)
                Qarena(ia) = Qarena(ia) + Me.m_Data.Qlink(ii)
            Next

            'then set initial vulnerable biomasses (V) by arena
            For ii = 1 To Me.m_Data.Narena
                i = Me.m_Data.Iarena(ii)
                j = Me.m_Data.Jarena(ii)
                'Debug.Assert(i <> 8)
                If Me.m_Data.VulMult(i, j) > Me.m_Data.VulnerabilityCap Then
                    Me.m_Data.VulMult(i, j) = Me.m_Data.VulnerabilityCap
                End If
                Me.m_Data.VulArena(ii) = CSng((Me.m_Data.VulMult(i, j) + 0.0000000001) * Qarena(ii) / Me.m_Data.StartBiomass(i))
                If Me.m_Data.VulArena(ii) = 0 Then Me.m_Data.VulArena(ii) = 1
                If Me.m_Data.BoutFeeding Then
                    VulBiom(ii) = CSng(-Qarena(ii) / Math.Log(1 - 1 / (Me.m_Data.VulMult(i, j) + 0.0000000001)))
                Else
                    VulBiom(ii) = CSng((Me.m_Data.VulMult(i, j) + 0.0000000001 - 1.0#) * Qarena(ii) / (2 * Me.m_Data.VulArena(ii)))
                End If
                If VulBiom(ii) = 0 Then VulBiom(ii) = 1

                'note above calculation will give wrong result if vulmult(i,j)=1, i.e. vulmult must be strictly
                'greater than 1.0
                'set nonzero value for vularena to avoid divides by zero if no feeding in it
            Next
            'then set predator search rates (a) by trophic link

            'System.Console.WriteLine("i Prey, j Pred, Prey Name, Pred Name, A, V")
            Dim Dzero As Single
            For ii = 1 To Me.m_Data.inlinks
                ia = Me.m_Data.ArenaLink(ii)
                j = Me.m_Data.jlink(ii)
                If VulBiom(ia) > 0 Then
                    Dzero = Me.m_Data.CmCo(j) / (Me.m_Data.CmCo(j) - 1)
                    Me.m_Data.Alink(ii) = Dzero * Me.m_Data.Qlink(ii) / (VulBiom(ia) * Me.m_Data.pred(j))
                Else
                    Me.m_Data.Alink(ii) = 0
                End If

                'System.Console.WriteLine(m_Data.Iarena(m_Data.ArenaLink(ii)).ToString + " , " + m_Data.Jarena(m_Data.ArenaLink(ii)).ToString + ",  " + m_Data.ilink(ii).ToString + " , " + m_Data.jlink(ii).ToString + ",  " + m_EPData.GroupName(m_Data.Iarena(ii)) + ", " + m_EPData.GroupName(j) + ", " + m_Data.Alink(ii).ToString() + ",  " + m_Data.VulArena(ii).ToString())

            Next


        End Sub


        Sub PredictCurrentEffort(ByVal t As Integer)
            'predicts fishing effort by gear from currentincome (value of landings per unit effort)
            'and current effort capacity Captime(ig)
            Dim ig As Integer, Ipower As Single
            For ig = 1 To Me.m_Data.nGear
                Ipower = CSng(Me.CurrentIncome(ig) ^ Me.m_Data.Epower(ig))
                Me.m_Data.FishRateGear(ig, t) = Me.CapTime(ig) * Ipower / (Me.EscalePar(ig) + Ipower)
                'jb m_Data.FishRateGear(gear,time) is bounded by MaxEffort() in SetFtimeFromGear()
            Next

        End Sub

        Sub FindCurrentProfit(BB() As Single, t As Integer, y As Integer)

            Dim i As Integer, ig As Integer, TotIncome As Single, Fg As Single
            Dim LandingsForValue(Me.m_Data.nGroups, Me.m_Data.nGear) As Single
            For i = 1 To Me.m_EPData.NumLiving
                For ig = 1 To Me.m_Data.nGear
                    LandingsForValue(i, ig) += Me.CalcLandings(i, ig, BB(i), m_Data.FishRateGear(ig, t), Me.m_Data.relQ(ig, i), 1)
                Next ig
            Next i
            Me.m_Data.PriceMedData.SetPriceMedFunctions(LandingsForValue)

            Dim TotCost As Single
            ReDim CurrentProfit(m_Data.nGear), CurrentIncome(m_Data.nGear)
            For ig = 1 To m_Data.nGear
                TotIncome = 0
                For i = 1 To m_Data.nGroups
                    Fg = CSng(Qmult(i) * m_Data.FishMGear(ig, i) * (m_Data.FishRateGear(ig, t) + 1.0E-20))
                    TotIncome += Fg * BB(i) * Me.MarketValue(i, ig, t, y) * Me.m_Data.PropLandedTime(ig, i)
                Next

                TotCost = m_Data.FishRateGear(ig, t) * (Me.EffortCost(ig, t, y) + Me.SailCost(ig, t, y))
                CurrentProfit(ig) = TotIncome - TotCost
                'If CurrentProfit(ig) <> CurrentProfit(ig) Then Stop
                If CurrentProfit(ig) < 0 Then CurrentProfit(ig) = 0
                CurrentIncome(ig) = CSng(TotIncome / (m_Data.FishRateGear(ig, t) + 1.0E-20))
            Next
            'Debug.Print CurrentProfit(1), CurrentProfit(2), CurrentProfit(3)
        End Sub


        Sub PredictCapacityChange()
            'predicts changes in fleet capacity caused by profits and depreciation
            Dim ig As Integer, Cg As Single
            For ig = 1 To Me.m_Data.nGear
                Cg = Me.CapGrowthFactor(ig) * Me.CurrentProfit(ig) : If Cg < 0 Then Cg = 0
                Me.CapTime(ig) = Me.CapTime(ig) * (1 - Me.m_Data.CapDepreciate(ig) / 12) + Cg / 12
                '   If CapTime(ig) <> CapTime(ig) Or CapTime(ig) > 1000 Then Stop
            Next
        End Sub


        ''' <summary>
        ''' Initialize fishing capacities by fleet for dynamic effort model, and set effort dynamic response parameters for simulation
        ''' </summary>
        Sub InitializeCapacity()
            'initialize fishing capacities by fleet for dynamic effort model, and
            'set effort dynamic response parameters for simulation

            If Me.m_search.bInSearch And Me.m_search.SearchMode <> eSearchModes.MSE Then
                'If in search mode then only MSE
                Return
            End If

            ReDim Me.CapTime(Me.m_Data.nGear), Me.CapBase(Me.m_Data.nGear), Me.EscalePar(Me.m_Data.nGear)
            ReDim Me.Qmult(Me.m_Data.nGroups)

            Dim ig As Integer, i As Integer, t As Integer
            For i = 1 To Me.m_Data.nGroups : Me.Qmult(i) = 1 : Next
            For ig = 1 To Me.m_Data.nGear
                Me.m_Data.FishRateGear(ig, 0) = 1
                Me.m_Data.FishRateGear(ig, 1) = 1
                '****VILLY: following parameters need to be set in interface/database*****
                '    Epower(ig) = 3
                '    PcapBase(ig) = 0.5
                '    CapDepreciate(ig) = 0.06
                '    CapBaseGrowth(ig) = 0.2
                '*****end of temporary parameter set block************
                'VC: the above is done when opening a model, Aug 2002
                Me.CapBase(ig) = 1 / Me.m_Data.PcapBase(ig)
                Me.CapTime(ig) = Me.CapBase(ig)
                For t = 0 To Me.m_Data.NTimes
                    Me.m_Data.FishRateGear(ig, t) = 1
                Next
            Next
            Me.FindCurrentProfit(Me.m_Data.StartBiomass, 0, 0)
            For ig = 1 To Me.m_Data.nGear
                Me.EscalePar(ig) = CSng((Me.CapBase(ig) - 1) * Me.CurrentIncome(ig) ^ Me.m_Data.Epower(ig))
                Me.CapGrowthFactor(ig) = CSng(Me.CapBase(ig) * (Me.m_Data.CapBaseGrowth(ig) + Me.m_Data.CapDepreciate(ig)) / (Me.CurrentProfit(ig) + 1.0E-20))
            Next

        End Sub


        ''' <summary>
        ''' Summarize data for the Summary Results grid. This is data that is summarized into two time windows defined by the user.
        ''' </summary>
        ''' <param name="iTime"></param>
        ''' <remarks></remarks>
        Private Sub getSummaryResults(ByVal iTime As Integer)
            Dim iTimeWindow As Integer = -1

            'iTimeWindow is the index that the average is on in the sumxxxx arrays ex. SumBiomass(iSummaryIndex,igrp)
            'iTimeWindow = -1 then this is not a summary time period
            'iTimeWindow = 0 this is the first summary time period
            'iTimeWindow = 1 this is the last summary time period

            If iTime >= Me.m_Data.SumStart(0) And Me.m_Data.NumStep0 <= Me.m_Data.NumStep - 1 Then
                iTimeWindow = 0
                Me.m_Data.NumStep0 = Me.m_Data.NumStep0 + 1
            ElseIf iTime >= Me.m_Data.SumStart(1) And Me.m_Data.NumStep1 <= Me.m_Data.NumStep - 1 Then
                iTimeWindow = 1
                Me.m_Data.NumStep1 = Me.m_Data.NumStep1 + 1
            End If


            If iTimeWindow > -1 Then

                For igrp As Integer = 1 To Me.m_Data.nGroups

                    Me.m_Data.SumBiomass(iTimeWindow, igrp) = Me.m_Data.SumBiomass(iTimeWindow, igrp) + Me.BB(igrp)
                    Me.m_Data.SumCatch(iTimeWindow, igrp) = Me.m_Data.SumCatch(iTimeWindow, igrp) + Me.BB(igrp) * Me.m_Data.FishTime(igrp)

                Next

            End If

        End Sub


        Public Sub copyTo(ByRef d As cEcosimModel)
            Try
                d.m_ConTracer = Me.m_ConTracer
                d.TracerData = Me.TracerData
                d.m_pluginManager = Me.m_pluginManager
                d.m_RefData = Me.m_RefData
                d.m_publisher = Me.m_publisher

                ' Ntimes 
                d.StepsPerYear = Me.StepsPerYear
                d.TimeNow = Me.TimeNow
                d.YearNow = Me.YearNow
                d.DeltaT = Me.DeltaT
                d.nvar = Me.nvar
                d.DoingEiiSaving2Round = Me.DoingEiiSaving2Round
                d.MakeTestData = Me.MakeTestData
                d.AbortRun = Me.AbortRun
                d.IsDatWtSet = Me.IsDatWtSet

                d.BaseValue = Me.BaseValue
                d.A = DirectCast(Me.A.Clone, Single(,))
                d.dydx = DirectCast(Me.dydx.Clone, Single())

                d.BB = DirectCast(Me.BB.Clone, Single())
                d.pbbase = DirectCast(Me.pbbase.Clone, Single())
                d.StartEatenBy = DirectCast(Me.StartEatenBy.Clone, Single())
                d.EatenByBase = DirectCast(Me.EatenByBase.Clone, Single())
                d.StartEatenOf = DirectCast(Me.StartEatenOf.Clone, Single())
                d.SimQB = DirectCast(Me.SimQB.Clone, Single())
                d.Mtotal = DirectCast(Me.Mtotal.Clone, Single())

                d.SimGES = DirectCast(Me.SimGES.Clone, Single())
                d.IadCode = DirectCast(Me.IadCode.Clone, Integer())
                d.IjuCode = DirectCast(Me.IjuCode.Clone, Integer())
                d.IecoCode = DirectCast(Me.IecoCode.Clone, Integer())

                d.EnergyBase = DirectCast(Me.EnergyBase.Clone, Single())   'VC 2025-03-12 Adding Energy
                d.EnergyUsed = Me.EnergyUsed             'VC 2025-03-12 Adding Energy

                d.nGroups = Me.nGroups
                d.Irun = Me.Irun
                d.Qmult = DirectCast(Me.Qmult.Clone, Single())
                d.EscalePar = DirectCast(Me.EscalePar.Clone, Single())
                d.CapGrowthFactor = DirectCast(Me.CapGrowthFactor.Clone, Single())
                d.CostPenaltyConstant = Me.CostPenaltyConstant
                d.BestTime = DirectCast(Me.BestTime.Clone, Single())

                d.fbasetest = Me.fbasetest
                d.fstep = Me.fstep
                d.fishingrate = Me.fishingrate
                d.fstepp = Me.fstepp
                d.lastharvest = Me.lastharvest
                d.equilharvest = Me.equilharvest
                d.lastvalue = Me.lastvalue
                d.equilvalue = Me.equilvalue
                d.cbval = Me.cbval
                d.equilstock = Me.equilstock
                d.LinScale = Me.LinScale
                d.Srec = DirectCast(Me.Srec.Clone, Single())
                d.TimeSeriesFile = Me.TimeSeriesFile

                d.DatSumZ = DirectCast(Me.DatSumZ.Clone, Single())
                d.DatSumZ2 = DirectCast(Me.DatSumZ2.Clone, Single())
                d.DatNobs = DirectCast(Me.DatNobs.Clone, Integer())

                d.NobsTime = DirectCast(Me.NobsTime.Clone, Single())
                d.m_RefData.Erpred = DirectCast(Me.m_RefData.Erpred.Clone, Single())
                d.m_RefData.Yhat = DirectCast(Me.m_RefData.Yhat.Clone, Single())
                d.DatDev = DirectCast(Me.DatDev.Clone, Single(,))

                d.NutPBmax = Me.NutPBmax
                d.maxKageMax = Me.maxKageMax

                d.CBlast = DirectCast(Me.CBlast.Clone, Single())
                d.PredPerBiomass = DirectCast(Me.PredPerBiomass.Clone, Single())

                ' ConKtrophic.clone 
                d.pbb = DirectCast(Me.pbb.Clone, Single())
                d.SimGEtemp = DirectCast(Me.SimGEtemp.Clone, Single())
                d.biomeq = DirectCast(Me.biomeq.Clone, Single())

                d.deriv = DirectCast(Me.deriv.Clone, Single())
                d.RiskRate = DirectCast(Me.RiskRate.Clone, Single())
                d.Qopt = DirectCast(Me.Qopt.Clone, Single())
                d.yt = DirectCast(Me.yt.Clone, Single())
                d.dyt = DirectCast(Me.dyt.Clone, Single())
                d.dym = DirectCast(Me.dym.Clone, Single())
                d.Nrec = DirectCast(Me.Nrec.Clone, Single())
                d.Brec = DirectCast(Me.Brec.Clone, Single())

                'changed to public
                d.AhatStanza = DirectCast(Me.AhatStanza.Clone, Single())
                d.RhatStanza = DirectCast(Me.RhatStanza.Clone, Single())
                d.Rbase = DirectCast(Me.Rbase.Clone, Single())
                d.BrecYear = DirectCast(Me.BrecYear.Clone, Single())
                d.Dfitness = DirectCast(Me.Dfitness.Clone, Single())
                d.Deatenby = DirectCast(Me.Deatenby.Clone, Single())
                d.Deatenof = DirectCast(Me.Deatenof.Clone, Single())
                '  DetPassedProp.clone 
                d.Bstore = DirectCast(Me.Bstore.Clone, Single(,))

                '=m_Data As cEcosimDatastructures
                '=m_stanza As cStanzaDatastructures
                '=m_Results As cEcoSimResults
                '=m_EPData As cEcopathDataStructures
                d.m_bInit = Me.m_bInit
                '=m_ProgressDelegate As EcoSimTimeStepDelegate
                '=m_SynEcoSim As System.ComponentModel.ISynchronizeInvoke
                '=m_search As cSearchDatastructures
                d.bStopRunning = Me.bStopRunning
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try
        End Sub

        Private Sub EstimateTLofCatch(ByVal TimeStep As Integer)

            Dim i As Integer
            Dim fCatch As Single
            Dim totalTL As Single
            Dim sFiBT, sFiB0 As Double

            Try
                ' Reset running totals for a time step
                Me.m_Data.CatchSim(TimeStep) = 0

                'js13oct10: implemented FiB calcs
                ''vc100522: Calculation of FiB index is wrong. it should be based on the catch for each functional group as follows:
                ''FiB = [Catch(iGrp, iTime) ^ (TL(igrp)-1)] / [Catch(iGrp, 0) ^ (TL(igrp)-1)] assuming first timestep is 0
                ''I presume that we somewhere later convert to math.log10(FiB) as it has to be scaled this way.
                sFiB0 = 0 : sFiBT = 0

                For i = 1 To Me.m_EPData.NumGroups
                    ' Determine catch
                    fCatch = Me.m_Data.FishTime(i) * Me.BB(i)
                    ' Total sim catch
                    Me.m_Data.CatchSim(TimeStep) += fCatch
                    ' Determine running TLC
                    totalTL += Me.m_Data.TLSim(i) * fCatch

                    ' Is first time step?
                    If TimeStep = 0 Then
                        ' #Yes: preserve catch for each group
                        Me.fCatch0(i) = fCatch
                        ' Init TLSim
                        Me.m_Data.TLSim(i) = Me.m_EPData.TTLX(i)
                    ElseIf fCatch > 0 Then ' Apparently, 0 ^ 0 = 1?!
                        ' #No: total FiB terms for all groups
                        sFiBT = sFiBT + (fCatch ^ (Me.m_Data.TLSim(i) - 1))
                        sFiB0 = sFiB0 + (Me.fCatch0(i) ^ (Me.m_Data.TLSim(i) - 1))
                    End If
                Next

                ' Calculate FiB
                If TimeStep = 0 Then
                    Me.m_Data.FIB(TimeStep) = 1
                Else
                    Me.m_Data.FIB(TimeStep) = CSng(Math.Log10(sFiBT / sFiB0))
                End If

                ' Calculate kemptons Q
                If TimeStep = 0 Then
                    'Baseline value
                    'VC added Shannon June 2016
                    Me.m_Data.Kemptons(0) = Me.m_Ecofunctions.KemptonsQ(Me.m_EPData.NumLiving, Me.m_EPData.TTLX, Me.m_Data.StartBiomass, 0.25)
                    Me.m_Data.ShannonDiversity(0) = Me.m_Ecofunctions.ShannonDiversityIndex(Me.m_EPData.NumLiving, Me.m_Data.StartBiomass)
                Else
                    Me.m_Data.Kemptons(TimeStep) = Me.m_Ecofunctions.KemptonsQ(Me.m_EPData.NumLiving, Me.m_Data.TLSim, Me.BB, 0.25)
                    Me.m_Data.ShannonDiversity(TimeStep) = Me.m_Ecofunctions.ShannonDiversityIndex(Me.m_EPData.NumLiving, Me.BB)
                End If
                If Me.m_Data.CatchSim(0) > 0 Then
                    ' Calculate TL of catch
                    Me.m_Data.TLC(TimeStep) = CSng(totalTL / (Me.m_Data.CatchSim(TimeStep) + 1.0E-20))
                Else
                    ' ?!
                    Me.m_EPData.TLcatch = CSng(totalTL / (Me.m_Data.CatchSim(TimeStep) + 1.0E-20))
                End If

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, ex.ToString)
                Throw New ApplicationException(Me.ToString & ".EstimateTLofCatch() Error: " & ex.Message, ex)
            End Try

        End Sub

        Private Sub EstimateTLs(ByVal time As Integer)

            Dim i As Integer
            Dim j As Integer
            Dim Diet(Me.m_EPData.NumGroups, Me.m_EPData.NumGroups) As Single
            Dim SumDiet As Single
            Dim SumR() As Single
            Dim Alpha(,) As Single
            Dim SumBio() As Single

            Try

                'System.Windows.Forms.Application.DoEvents()

                For i = 1 To Me.m_EPData.NumLiving  'consumer
                    If Me.m_Data.Eatenby(i) > 0 Then
                        SumDiet = 0
                        For j = 1 To Me.m_EPData.NumGroups  'food
                            Diet(i, j) = Me.m_Data.Consumpt(j, i) / Me.m_Data.Eatenby(i)
                            SumDiet = SumDiet + Diet(i, j)
                        Next
                        If SumDiet > 0 Then
                            For j = 1 To Me.m_EPData.NumGroups  'food
                                Diet(i, j) = Diet(i, j) / SumDiet
                            Next
                        End If
                    End If
                Next

                Me.m_Ecofunctions.EstimateTrophicLevels(Me.m_EPData.NumGroups, Me.m_EPData.NumLiving, Me.m_EPData.PP, Diet, Me.m_Data.TLSim)

                ReDim SumBio(Me.m_EPData.NumLiving)
                ReDim SumR(Me.m_EPData.NumLiving)
                ReDim Alpha(Me.m_EPData.NumLiving, Me.m_EPData.NumGroups)
                For i = 1 To Me.m_EPData.NumLiving
                    If Me.m_EPData.QB(i) > 0 Then    'Estimate Chesson from Sim
                        SumBio(i) = 0
                        For j = 1 To Me.m_EPData.NumGroups
                            SumBio(i) = SumBio(i) + Me.m_EPData.B(j)
                        Next
                        SumR(i) = 0
                        For j = 1 To Me.m_EPData.NumGroups              'FOLLOWING CHESSON (1983)
                            If Me.m_EPData.B(j) > 0 Then Alpha(i, j) = Diet(i, j) / (Me.BB(j) / SumBio(i))
                            SumR(i) = SumR(i) + Alpha(i, j)
                        Next

                        If SumR(i) > 0 Then
                            For j = 1 To Me.m_EPData.NumGroups
                                Alpha(i, j) = Alpha(i, j) / SumR(i)
                            Next                'THIS ALPHA IS THE SAME AS CHESSONS ALPHA
                        End If

                        For j = 1 To Me.m_EPData.NumGroups
                            Me.m_Data.Elect(i, j, time) = Alpha(i, j) '(NumGroups * Alpha(j) - 1) / ((NumGroups - 2) * Alpha(j) + 1)
                        Next
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".EstimateTLsInEcosim() Error: " & ex.Message)
                Throw New ApplicationException(Me.ToString & ".EstimateTLsInEcosim() Error: " & ex.Message, ex)
            End Try

        End Sub

        Private Sub ComputeMoreStats(itime As Integer)

            Dim Diet(Me.m_EPData.NumGroups, Me.m_EPData.NumGroups) As Single
            Dim BQB(Me.m_EPData.NumGroups) As Single

            For i As Integer = 1 To Me.m_EPData.NumLiving  'consumer
                If Me.m_Data.Eatenby(i) > 0 Then
                    Dim SumDiet As Single = 0
                    For j As Integer = 1 To Me.m_EPData.NumGroups  'food
                        Diet(i, j) = Me.m_Data.Consumpt(j, i) / Me.m_Data.Eatenby(i)
                        SumDiet += Diet(i, j)
                    Next
                    If SumDiet > 0 Then
                        For j As Integer = 1 To Me.m_EPData.NumGroups  'food
                            Diet(i, j) /= SumDiet
                        Next
                    End If
                End If
            Next

            Me.m_Ecofunctions.Omniv(Diet, Me.m_Data.TLSim, BQB, Me.m_EPData.NumGroups)
            For i As Integer = 1 To Me.m_EPData.NumGroups
                Me.m_Data.BQB(i, itime) = BQB(i)
            Next

            Dim min_B_QB As Single = 0
            For i As Integer = 1 To Me.m_EPData.NumGroups
                If Me.m_EPData.QB(i) > 0 Then
                    Dim x As Single = Me.BB(i) * Me.m_EPData.QB(i)
                    If (min_B_QB = 0) Then
                        min_B_QB = x
                    Else
                        min_B_QB = Math.Min(min_B_QB, x)
                    End If
                End If
            Next i

            Debug.Assert(min_B_QB > 0)

            Dim SysOm As Single = 0
            Dim SysOmDen As Single = 0

            For i As Integer = 1 To Me.m_EPData.NumLiving
                If Me.BB(i) * Me.m_EPData.QB(i) / min_B_QB > 0 Then    ' *** CONSUMERS ONLY
                    SysOm += CSng(Math.Log(Me.BB(i) * Me.m_EPData.QB(i) / min_B_QB) * BQB(i))
                    SysOmDen += CSng(Math.Log(Me.BB(i) * Me.m_EPData.QB(i) / min_B_QB))
                End If
            Next i

            If SysOmDen > 0 Then
                SysOm /= SysOmDen
                Me.m_Data.SysOm(itime) = SysOm
            End If

        End Sub

        Public Sub calcFunctionalResponse(ByVal time As Integer)
            Dim i As Integer
            Dim j As Integer
            Dim Diet(,) As Single
            Dim SumDiet As Single
            Dim SumR() As Single
            Dim Alpha(,) As Single
            'On Local Error Resume Next
            Dim SumBio() As Single

            If Me.m_search.SearchMode = eSearchModes.FitToTimeSeries Then
                Return
            End If

            Try

                ReDim Diet(Me.m_EPData.NumGroups, Me.m_EPData.NumGroups)

                For i = 1 To Me.m_EPData.NumLiving  'consumer
                    If Me.m_Data.Eatenby(i) > 0 Then
                        SumDiet = 0
                        For j = 1 To Me.m_EPData.NumGroups  'food
                            Diet(i, j) = Me.m_Data.Consumpt(j, i) / Me.m_Data.Eatenby(i)
                            SumDiet = SumDiet + Diet(i, j)
                        Next
                        If SumDiet > 0 Then
                            For j = 1 To Me.m_EPData.NumGroups  'food
                                Diet(i, j) = Diet(i, j) / SumDiet
                            Next
                        End If
                    End If
                Next

                ReDim SumBio(Me.m_EPData.NumLiving)
                ReDim SumR(Me.m_EPData.NumLiving)
                ReDim Alpha(Me.m_EPData.NumLiving, Me.m_EPData.NumGroups)
                For i = 1 To Me.m_EPData.NumLiving
                    If Me.m_EPData.QB(i) > 0 Then    'Estimate Chesson from Sim
                        SumBio(i) = 0
                        For j = 1 To Me.m_EPData.NumGroups
                            SumBio(i) = SumBio(i) + Me.m_EPData.B(j)
                        Next
                        SumR(i) = 0
                        For j = 1 To Me.m_EPData.NumGroups              'FOLLOWING CHESSON (1983)
                            If Me.m_EPData.B(j) > 0 Then Alpha(i, j) = Diet(i, j) / (Me.BB(j) / SumBio(i))
                            SumR(i) = SumR(i) + Alpha(i, j)
                        Next

                        If SumR(i) > 0 Then
                            For j = 1 To Me.m_EPData.NumGroups
                                Alpha(i, j) = Alpha(i, j) / SumR(i)
                            Next                'THIS ALPHA IS THE SAME AS CHESSONS ALPHA
                        End If

                        For j = 1 To Me.m_EPData.NumGroups
                            Me.m_Data.Elect(i, j, time) = Alpha(i, j) '(NumGroups * Alpha(j) - 1) / ((NumGroups - 2) * Alpha(j) + 1)
                        Next
                    End If
                Next

            Catch ex As Exception
                cLog.Write(ex)
                Debug.Assert(False, Me.ToString & ".calcFunctionalResponse() Error: " & ex.Message)
            End Try

        End Sub


        Friend Function VulBo(ByVal BmaxBo As Single,
                              ByVal iGroup As Integer,
                              ByVal FtimeOn As Boolean) As Single

            Dim Q As Single
            Dim gc As Single
            Dim M As Single
            Dim mp As Single
            Dim v As Single

            If BmaxBo > 1 And Me.m_Data.SimGE(iGroup) > 0 And Me.m_Data.Fish1(iGroup) > 0 Then
                If FtimeOn Then
                    M = Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)
                    mp = (Me.m_Data.MoPred(iGroup) * Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)) / M
                    gc = Me.m_Data.SimGE(iGroup) * Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup)
                    If M <> gc Then v = (-gc * BmaxBo + (1 - mp) * M * BmaxBo + mp * M) / (M - gc)
                Else
                    M = Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)
                    Q = M / Me.m_Data.SimGE(iGroup)
                    If Q <> Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup) Then v = Q * (1 - BmaxBo) / (Q - Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup))
                End If
                If v > 1 Then Return v Else Return -1 'was Log(Log(v) / 2 + 1) where it's v now
            End If
            Return -1

        End Function

        Friend Function VulFmax(ByVal Fpo As Single,
                                ByVal iGroup As Integer,
                                ByVal FtimeOn As Boolean) As Single

            Dim Q As Single
            Dim gc As Single
            Dim M As Single
            Dim mp As Single
            Dim v As Single

            If Fpo > 0 And Me.m_Data.SimGE(iGroup) > 0 Then
                If FtimeOn Then
                    M = Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)
                    mp = (Me.m_Data.MoPred(iGroup) * Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)) / M
                    gc = mp * M / (M + Fpo * M - Me.m_Data.SimGE(iGroup) * Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup))
                    v = mp * M / (M - Me.m_Data.SimGE(iGroup) * Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup) + Fpo * M)
                Else
                    M = Me.m_Data.mo(iGroup) + Me.StartEatenOf(iGroup) / Me.m_Data.StartBiomass(iGroup)
                    Q = M / Me.m_Data.SimGE(iGroup) * (1 + Fpo)
                    If Q <> Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup) Then v = Q / (Q - Me.StartEatenBy(iGroup) / Me.m_Data.StartBiomass(iGroup))
                End If
                If v > 1 Then Return v Else Return -1 'was Log(Log(v) / 2 + 1) where it's v now
            End If
            Return -1

        End Function

#End Region

#Region "Evolve functions stored for reference"

#If EVOLVE Then

            Private Sub evolve(ByVal Biomass() As Single)
            Dim FMove As Single, Ftimecheck() As Single, DFtime() As Single
            Dim DfitCheck() As Single, DfitBase() As Single, i As Integer
            Dim DeltaTime As Single
            ReDim Ftimecheck(nGroups), DFtime(nGroups), DfitCheck(nGroups), DfitBase(nGroups)
            Dim dydxbase(100) As Single, dydxnew(100) As Single, DfitNum(100) As Single

            FMove = 0.5

            'obtain base estimate of dfitness/dftime at current ftime
            'note DfitDFtime gives exact answer only when there is no cannibalism
            '(derivative should be larger when there is?)
            DfitDFtime(Biomass, DfitBase, m_Data.Ftime)
            'then obtain second estimate at increased ftime
            '  Derivt 0, Biomass(), dydxbase()

            For i = 1 To nGroups
                '     Ftime(i) = Ftime(i) + 0.001
                '     Derivt 0, Biomass(), dydxnew()
                '     Ftime(i) = Ftime(i) - 0.001
                '     DfitNum(i) = (dydxnew(i) - dydxbase(i)) / Biomass(i) / 0.001
                DFtime(i) = 0.1 * m_Data.Ftime(i)
                Ftimecheck(i) = m_Data.Ftime(i) + DFtime(i)
                Dfitness(i) = DfitBase(i)
            Next
            DfitDFtime(Biomass, DfitCheck, Ftimecheck)
            'project ftime that would make dfitness/dftime=0, change ftime partway to this value
            For i = 1 To m_EPData.NumLiving
                If Math.Abs(Ftimecheck(i) - m_Data.Ftime(i)) > 0.0000000001 And m_Data.SimGE(i) > 0 Then
                    'DeltaTime = Sgn(DfitBase(i)) * Abs(FMove * DfitBase(i) * DFtime(i) / (Ftimecheck(i) -  m_data.ftime(i)))
                    'following is second derivative method for moving to optimum ftime
                    'it doesn't work in most cases, due to pos 2nd deriv at ftime=1
                    'DeltaTime = -FMove * DfitBase(i) * DFtime(i) / (DfitCheck(i) - DfitBase(i))
                    'below is simpler gradient move (equal to derivative, constrained)
                    DeltaTime = DfitBase(i)
                    If Math.Abs(DeltaTime) > 0.01 Then DeltaTime = 0.01 * Math.Sign(DeltaTime)
                    ' If Abs(DeltaTime) > 0.1 *  m_data.ftime(i) Then DeltaTime = Sgn(DeltaTime) * 0.1 *  m_data.ftime(i)
                    m_Data.Ftime(i) = m_Data.Ftime(i) + DeltaTime
                    If m_Data.Ftime(i) > m_Data.FtimeMax(i) Then m_Data.Ftime(i) = m_Data.FtimeMax(i)
                    If m_Data.Ftime(i) < 0.01 Then m_Data.Ftime(i) = 0.01
                End If
            Next
        End Sub

        Private Sub DfitDFtime(ByVal Biomass() As Single, ByVal Dfit() As Single, ByVal Fti() As Single)
            'This routine calculates derivative of dB/B dt with respect to the time
            '(relative) spent foraging (Ftime) by pool which affects eatenof and eatenby
            'components of dB/dt
            'the derivative of "fitness" dB/Bdt with respect to ftime is stored in the
            'array Dfitness(pool)
            Dim i As Integer, j As Integer, ii As Integer
            Dim eat As Single
            Dim aeff As Single
            Dim Den As Single

            setpred(Biomass)
            ReDim Deatenby(nGroups)
            ReDim Deatenof(nGroups)
            For ii = 1 To m_Data.inlinks
                i = m_Data.ilink(ii) : j = m_Data.jlink(ii)
                'prey
                ' For j = 1 To N  'VC ignore detritus; CJW had nGroups 'predator
                aeff = A(i, j) * Fti(j)
                Select Case m_Data.FlowType(i, j) 'prey always first
                    Case 1 'donor controlled flow
                        eat = aeff * Biomass(i)
                    Case 3 'limited total flow
                        eat = aeff * Biomass(i) * m_Data.pred(j) / (1 + aeff * m_Data.pred(j) * Biomass(i) / m_Data.maxflow(i, j))
                    Case 2 'prey limited flow
                        Den = (m_Data.vulrate(i, j) + m_Data.vulrate(i, j) * Fti(i) + aeff * m_Data.pred(j))
                        eat = aeff * Biomass(i) * m_Data.pred(j) * m_Data.vulrate(i, j) * Fti(i) / Den
                    Case Else
                        eat = 0
                End Select
                'Eatenof(i) = Eatenof(i) + eat
                'Eatenby(j) = Eatenby(j) + eat
                If m_Data.FlowType(i, j) = 2 Then
                    Deatenof(i) = Deatenof(i) + eat / Fti(i) - m_Data.vulrate(i, j) * eat / Den
                    Deatenby(j) = Deatenby(j) + eat / Fti(j) - A(i, j) * m_Data.pred(j) * eat / Den
                End If
            Next
            For i = 1 To m_EPData.NumLiving
                If m_Data.SimGE(i) > 0 Then
                    Dfit(i) = (m_Data.SimGE(i) * Deatenby(i) - Deatenof(i)) / m_EPData.B(i) - m_Data.MoPred(i) * m_Data.mo(i)
                End If
            Next
        End Sub

#End If

#End Region

    End Class

End Namespace
