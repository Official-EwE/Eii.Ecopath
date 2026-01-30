' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

'Option Explicit On

Imports EwECore.Ecosim
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Namespace FishingPolicy

    ''' <summary>
    ''' A Fishing Policy search has completed all it's runs.
    ''' </summary>
    ''' <remarks>If there are multiple runs they have all been completed or an Error has occured and the runs could not be completed.</remarks>
    Public Delegate Sub SearchCompletedDelegate()

    ''' <summary>
    ''' A run of the Fishing Policy search has completed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Delegate Sub RunCompletedDelegate()

    ''' <summary>
    ''' A Fishing Policy Search run has started.
    ''' </summary>
    ''' <remarks>When this is called the results object will be initialized and dimensioned but it will not contain any values.</remarks>
    Public Delegate Sub RunStartedDelegate()

    ''' <summary>
    ''' Progress of the current Fishing Policy run
    ''' </summary>
    ''' <remarks>The Results object will contain the results of the current interation</remarks>
    Public Delegate Sub ProgressDelegate()

    Public Delegate Sub AddMessageDelegate(ByRef message As cMessage)

#Region "Fishing Policy Search model"

    Public Class cFishingPolicySearch

        'ToDo_jb cFishingPolicySearch What is the message from EwE5 in UseCostPenalty() should this change the InitOption if it fails the test

#Region "Public variables"

        Public SearchCompletedCallBack As SearchCompletedDelegate
        Public RunCompletedCallBack As RunCompletedDelegate
        Public SearchStartedCallBack As RunStartedDelegate
        Public AddMessageCallBack As AddMessageDelegate
        Public ProgressCallBack As ProgressDelegate

        Public Results As cFPSSearchResults

        Public MaxRuns As Integer
        Public PrintOn As Boolean
        Public TotalTime As Integer
        Public ProfitBase As Double
        Public EmployBase As Double
        Public ManValueBase As Double
        Public EcoValueBase As Double
        Public ExistValue As Single
        Public BioDivBase As Double

        ''' <summary>
        ''' Force a running search to exit
        ''' </summary>
        ''' <remarks></remarks>
        Public SearchFailed As Boolean
        Public StopEstimation As Boolean

        'Count of the current run at the start of a run
        'the first run will be one
        Public iRun As Integer
#End Region

#Region "Private modeling variables"

        Private Resline As Integer
        Private CritValue(cSearchDatastructures.N_CRIT_RESULTS) As Single
        'Dim X() As Double
        Private G() As Double, Xm() As Double ', Nam$(Nmax)
        Private H() As Double, W() As Double    'was 1000 when nmax was 100
        'Dim X(Nmax) As Double, G(Nmax) As Double, Xm(Nmax) As Double, Nam$(Nmax)
        'Dim H(10000) As Double, W(10000) As Double    'was 1000 when nmax was 100
        Private ColrNo() As Long
        Private VlocalPenalty As Double
        Private MaxNoOfIterations As Integer
        Private ifn As Integer

        Dim ncom As Integer, pcom(50) As Double, xicom(50) As Double

        Dim PaidToJbyI(,) As Single
        Dim Profitability() As Single

        'used by SearchForBaseProfitability
        Dim PropToPlaintiff As Single 'this never get set to anything other than zero

#End Region

#Region "Private Core variables"

        Private m_core As cCore
        Private m_ecosim As cEcosimModel
        Private m_searchData As cSearchDatastructures
        Private m_pluginManager As cPluginManager
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cFishingPolicySearch)()

#End Region

#Region "Construction and Initialization"

        Public Sub New()

        End Sub

        Friend Sub init(ByRef theCore As cCore)

            Debug.Assert(theCore IsNot Nothing, Me.ToString & ".init(cCore) cCore must not be NULL.")

            Try

                Me.m_core = theCore
                Me.m_ecosim = Me.m_core.m_Ecosim
                Me.m_searchData = Me.m_core.m_SearchData

                Me.m_pluginManager = Me.m_core.PluginManager

                Me.MaxNoOfIterations = 2000 'from EwE5 frmSim1.load() why it is intialized in ecosim I have no idea
                Me.m_searchData.InitOption = eInitOption.EcopathBaseF
                Me.m_searchData.SearchMethod = eSearchOptionTypes.Fletch

            Catch ex As Exception
                m_logger.LogError(ex, "{0}.init(cCore) Failed to initialize.", Me.ToString)
                Throw New ApplicationException(Me.ToString & ".init(cCore) Failed to initialize.", ex)
            End Try

            ' Fire plug-in point
            If (Me.m_pluginManager IsNot Nothing) Then
                Me.m_core.m_SearchData.SearchMode = eSearchModes.FishingPolicy
                Me.m_pluginManager.SearchInitialized(Me.m_searchData)
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            End If

        End Sub

#End Region

#Region "Running the search"

        Public Sub Run()

            'run the model
            Try

                Me.runSearch()
            Catch ex As Exception
                'add a message to the manager
                Me.AddMessage(ex.Message)
            End Try

            If Me.SearchCompletedCallBack IsNot Nothing Then
                Me.SearchCompletedCallBack()
            End If

        End Sub

        Private Sub runSearch()
            'Hi Villy if econ is 'net economic value' it is totval calculated in cSearchDatastructures.EcosimSummarizeIndicators()
            '[14:46:10] Joe Buszowski says: if econ is Ecosystem structure then it is ecovalue calculated in cSearchDatastructures.calcYearlySummaryValues()
            '[14:48:52] Joe Buszowski says: The actual values that appear in the interface are calculated in cFishingPolicySearch.FUNC()
            ' VC 20250407 I changed it so that profit = totval-cost is used for profit instead of totval 

            Try

                Dim nBlocksUsed As Integer

                Me.SearchFailed = False
                Me.StopEstimation = False

                Me.m_searchData = Me.m_core.m_SearchData
                Me.m_searchData.SearchMode = eSearchModes.FishingPolicy 'make sure the search is turned on this will also set some default values based on the flag
                Me.m_searchData.initForRun(Me.m_core.m_EcopathData, Me.m_core.m_EcoSimData)

                Me.TotalTime = Me.m_core.nEcosimYears
                Me.m_searchData.RedimForRun()

                Me.m_searchData.setLimitFishingMortality()

                'In EwE5
                'BaseYear can be zero in the interface however once the zero baseyear is used to set the searchblocks it is set back to one
                'EwE5 Code frmOptf.CmdSearch_Click()
                'FblockCode(ifleet, BaseYear) = 0 'set baseyear in search blocks to zero
                'If BaseYear <= 0 Then BaseYear = 1'make sure baseyear is not zero 
                'If BaseYear is zero this allows the optimization to vary the baseyear but still get the baseyear values from one
                'In EwE6 we constrain baseyear 1 to nEcosimYears right from the start
                If Me.m_searchData.BaseYear < 1 Then
                    Me.m_searchData.BaseYear = 1
                End If

                If Me.m_searchData.BaseYear > Me.m_core.nEcosimYears Then
                    Me.m_searchData.BaseYear = Me.m_core.nEcosimYears
                End If

                Me.m_searchData.bBaseYearSet = False

                'get the number of blocks, sets ParNumber() and BlockNumber()
                nBlocksUsed = Me.m_searchData.SetFletchPars()

                'setting the number of blocks will set Frates to default values, for the new number of blocks
                Me.m_searchData.nBlocks = nBlocksUsed

                'set a new results object with the number of blocks
                Me.Results = New cFPSSearchResults(Me.m_searchData.nBlocks, Me.m_searchData.NumFleets)

                'the length of cSearchDataStructures.BlockNumber can/will be greater then cFPSSearchResults.BlockNumber
                'see SetFletchPars for why this is
                Debug.Assert(Me.m_searchData.BlockNumber.Length >= Me.Results.BlockNumber.Length, Me.ToString & " Number of search blocks is to big. This is a bug!")
                'copy the BlockNumber set in SetFletchPars into the results object
                Array.Copy(Me.m_searchData.BlockNumber, Me.Results.BlockNumber, Me.Results.BlockNumber.Length)

                Me.m_searchData.saveInitialFishingRate(Me.m_core.m_EcoSimData)

                Me.m_ecosim.Init(False)

                Me.checkUseCostPenalty(nBlocksUsed)

                'set Frates() for base values for the different Search Initialization Options
                'Ecopath, Current and Random
                Dim baseFrate As Single
                If Me.m_searchData.InitOption = eInitOption.EcopathBaseF Then
                    'EwE5 base Frates is always zero for Base values this may be a bug but it's copied here
                    baseFrate = 0
                Else
                    'Current F's or Random F's
                    baseFrate = 0.01
                End If

                For i As Integer = 1 To Me.m_searchData.nBlocks
                    Me.m_searchData.Frates(i) = baseFrate
                Next

                Me.m_searchData.setMaxEffort(nBlocksUsed)
                'get the base values for the objective function by running ecosim 
                Me.getBaseValues(nBlocksUsed)

                For Iter As Integer = 1 To Me.m_searchData.nRuns
                    If Me.SearchFailed Or Me.StopEstimation Then
                        Exit For
                    End If

                    'set the fishing rate to initial values (Frates(nBlocks)) base on the initialization option (m_searchData.InitOption )
                    Me.m_searchData.restoreSavedFishingRates()

                    'set maxEffort base on the initial fishing rates, maxEffort is used to constrain the fishing rates
                    Me.m_searchData.setMaxEffort(nBlocksUsed)

                    'tell the world that a search 'Run' has started info about the run is available via properties of the manager and results objects
                    Me.SearchStarted(Iter)

                    Me.Minimize(nBlocksUsed, Me.m_searchData.Frates, Me.m_searchData.SearchMethod)

                    If Me.RunCompletedCallBack IsNot Nothing Then
                        Me.RunCompletedCallBack()
                    End If

                Next Iter

            Catch ex As Exception
                m_logger.LogError(ex, "{0}.runSearch() Error running Fishing Policy Search.", Me.ToString)
                Throw New ApplicationException("Error running Fishing Policy Search.", ex)
            End Try

            ' Done
            If Me.m_pluginManager IsNot Nothing Then
                Me.m_core.m_SearchData.SearchMode = eSearchModes.FishingPolicy
                Me.m_pluginManager.SearchCompleted(Me.m_searchData)
                Me.m_core.m_SearchData.SearchMode = eSearchModes.NotInSearch
            End If

        End Sub

        Private Sub getBaseValues(nBlocksUsed As Integer)

            If Me.m_pluginManager IsNot Nothing Then
                Me.m_pluginManager.SearchIterationsStarting()
            End If

            Me.m_ecosim.bStopRunning = False

            'get the base values used by FUNC to tell the change between the current run and the base run
            Me.m_ecosim.RunModelValue(Me.TotalTime, Me.m_searchData.Frates, nBlocksUsed)

            If Me.m_searchData.FPSUseEconomicPlugin And (Me.m_pluginManager IsNot Nothing) Then
                Me.m_pluginManager.PostRunSearchResults(Me.m_searchData)
            End If

            Me.ProfitBase = Me.m_searchData.Profit
            Me.EmployBase = Me.m_searchData.Employ
            Me.ManValueBase = Me.m_searchData.ManValue
            Me.EcoValueBase = Me.m_searchData.EcoValue
            Me.BioDivBase = Me.m_searchData.DiversityIndex

            If Me.ProfitBase = 0 Then Me.ProfitBase = 1
            If Me.ProfitBase < 0 Then Me.ProfitBase = -Me.ProfitBase
            If Me.EmployBase = 0 Then Me.EmployBase = 1
            If Me.EmployBase < 0 Then Me.EmployBase = -Me.EmployBase
            If Me.ManValueBase = 0 Then Me.ManValueBase = 1
            If Me.EcoValueBase = 0 Then Me.EcoValueBase = 1
            If Me.BioDivBase = 0 Then Me.BioDivBase = 1

        End Sub

        Private Sub SearchStarted(iIteration As Integer)

            Try

                Me.iRun = iIteration
                'clear out the results from the last run
                Me.Results.Clear()

                If Me.SearchStartedCallBack IsNot Nothing Then
                    Me.SearchStartedCallBack()
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "SearchStarted")
            End Try

        End Sub

#End Region

#Region "Message handling"

        Private Sub AddMessage(strMessage As String, Optional msgType As eMessageType = eMessageType.ErrorEncountered, Optional msgImportance As eMessageImportance = eMessageImportance.Critical)

            Try

                Me.addMessage(New cMessage(strMessage, msgType, eCoreComponentType.Ecosim, msgImportance))

            Catch ex As Exception
                m_logger.LogError(ex, "{0}.addMessage() Error adding message.", Me.ToString)
                Debug.Assert(False, Me.ToString & ".addMessage() Error: " & ex.Message)
            End Try

        End Sub

        Private Sub addMessage(ByRef msg As cMessage)

            Debug.Assert(Me.AddMessageCallBack IsNot Nothing, Me.ToString & " Missing AddMessageCallBack().")

            Try
                If Me.AddMessageCallBack IsNot Nothing Then
                    Me.AddMessageCallBack(msg)
                End If
            Catch ex As Exception
                m_logger.LogError(ex, "{0}.addMessage() Error adding message.", Me.ToString)
                Debug.Assert(False, Me.ToString & ".addMessage() Error: " & ex.Message)
            End Try

        End Sub

        Sub printstats(Xtime As Double, itn As Integer, ifn As Integer, F As Double, n As Integer, X() As Double, G() As Double)

            Try

                'process any data for output
                Me.Results.nCalls = ifn

                For iblk As Integer = 1 To n
                    If X(iblk) < Math.Log(Me.m_searchData.MaxEffort) Then
                        Me.Results.BlockResults(iblk) = CSng(Math.Exp(X(iblk)))
                    Else
                        Me.Results.BlockResults(iblk) = 60
                    End If
                Next

                Dim WeightCorrection As Single
                'VC 20250419 the calculations below only included 4 of the 5 ValWeights, so added no 5
                If Me.m_searchData.ValWeight(1) + Me.m_searchData.ValWeight(2) + Me.m_searchData.ValWeight(3) + Me.m_searchData.ValWeight(4) + Me.m_searchData.ValWeight(5) > 0 Then
                    WeightCorrection = Me.m_searchData.ValWeight(1) + Me.m_searchData.ValWeight(2) + Me.m_searchData.ValWeight(3) + Me.m_searchData.ValWeight(4) + Me.m_searchData.ValWeight(5)
                End If
                If WeightCorrection <= 0 Then WeightCorrection = 1

                Me.Results.Totals = CSng((-F + Me.VlocalPenalty) / WeightCorrection)

                For icrit As Integer = 1 To cSearchDatastructures.N_CRIT_RESULTS
                    Me.Results.CriteriaValues(icrit) = Me.CritValue(icrit)
                Next

                If Me.ProgressCallBack IsNot Nothing Then
                    Me.ProgressCallBack()
                End If

#If DEBUG Then
                ''debuging output
                'System.Console.WriteLine("FPS iterations: " & Results.nCalls.ToString)
                'For icr As Integer = 1 To cSearchDatastructures.N_CRIT_RESULTS
                '    System.Console.Write(Results.CriteriaValues(icr).ToString & ", ")
                'Next

                'For iblk As Integer = 1 To n
                '    System.Console.Write(Math.Exp(X(iblk)).ToString & ", ")
                'Next
                'System.Console.WriteLine()
#End If

            Catch ex As Exception
                m_logger.LogError(ex, "{0}.printstats() Error printing stats.", Me.ToString)
                Debug.Assert(False, Me.ToString & ".printstats() Error: " & ex.Message)
            End Try

            ''defint I-N
            ''defdbl A-H, O-Z
            'Dim lines As Integer
            'Dim FormStr As String
            'Dim WeightCorrection As Single
            'If ValWeight(1) + ValWeight(2) + ValWeight(3) + ValWeight(4) > 0 Then
            '    WeightCorrection = ValWeight(1) + ValWeight(2) + ValWeight(3) + ValWeight(4)
            'End If
            'If WeightCorrection <= 0 Then WeightCorrection = 1

            ''If PrintOn = True Then frmOptF.Res.Print itn; " evals "; ifn; " ";
            'If itn = 0 Then
            '    For lines = 1 To n : optVal(lines, 0) = 1 : Next
            'End If
            'If PrintOn = True Then 'frmOptF.Res.Print "func: "; Format(-f, "####.##"); " ";
            '    frmOptF.vaRes.maxRows = frmOptF.vaRes.maxRows + 1
            '    ReDim Preserve optVal(n, frmOptF.vaRes.maxRows - 1)
            '    SetBlock(frmOptF.vaRes, 0, 0, frmOptF.vaRes.maxCols, frmOptF.vaRes.maxRows)
            '    frmOptF.vaRes.TypeHAlign = 2
            '    frmOptF.vaRes.BlockMode = False
            '    SetCellValue(frmOptF.vaRes, 1, frmOptF.vaRes.maxRows, Format(ifn, "###0"))
            '    SetCellValue(frmOptF.vaRes, 2, frmOptF.vaRes.maxRows, Format((-F + VlocalPenalty) / WeightCorrection, GenNum))
            '    'frmOptF.Res.Print "Fs: ";
            '    For i = 1 To 4
            '        'If CritValue(i) < 0 Then Stop
            '        If CritValue(i) > 1000 Then
            '            SetCellText(frmOptF.vaRes, 2 + i, frmOptF.vaRes.maxRows, "")
            '        Else
            '            SetCellValue(frmOptF.vaRes, 2 + i, frmOptF.vaRes.maxRows, Format(CritValue(i), GenNum))
            '        End If
            '    Next
            '    For i = 1 To n
            '        'frmOptF.Res.Print Format(Exp(X(i)), "#.#"); "  ";
            '        '061129VC: the maxeffort is to make it possible to go beyond the max 60x effort
            '        'we've had to do that for models starting in 1950 where effort was 200x by 2003
            '        'MaxEffort = if(frmOptF.Option1(0).value, 5 * Frates(i), Log(60))
            '        'If MaxEffort < 60 Then MaxEffort = 60
            '        FormStr = if(X(i) > 4.6, "0", GenNum)  'no decimals if bigger than 100
            '        If X(i) < Log(MaxEffort) Then   '3.4011 Then   'exp(4.1)=60
            '            SetCellValue(frmOptF.vaRes, i + 6, frmOptF.vaRes.maxRows, Format(Exp(X(i)), FormStr))
            '            optVal(i, frmOptF.vaRes.maxRows - 1) = Exp(X(i))
            '        Else
            '            If frmOptF.chkBatch Then
            '                SetCellValue(frmOptF.vaRes, i + 6, frmOptF.vaRes.maxRows, 60)
            '            Else
            '                SetCellText(frmOptF.vaRes, i + 6, frmOptF.vaRes.maxRows, ">60")
            '            End If
            '            optVal(i, frmOptF.vaRes.maxRows - 1) = 60
            '        End If
            '        frmOptF.vaRes.BackColor = ColrNo(i)
            '    Next i
            '    If frmOptF.chkBatch Then
            '        SetCellValue(frmOptF.vaRes, i + 6, frmOptF.vaRes.maxRows, Format(ValWeight(1), GenNum))
            '        SetCellValue(frmOptF.vaRes, i + 7, frmOptF.vaRes.maxRows, Format(ValWeight(2), GenNum))
            '        SetCellValue(frmOptF.vaRes, i + 8, frmOptF.vaRes.maxRows, Format(ValWeight(3), GenNum))
            '        SetCellValue(frmOptF.vaRes, i + 9, frmOptF.vaRes.maxRows, Format(ValWeight(4), GenNum))
            '    Else
            '        frmOptF.UpdatePlot(CritValue())
            '    End If
            '    lines = frmOptF.vaRes.Height / 275 + 1
            '    If frmOptF.vaRes.maxRows >= lines Then frmOptF.vaRes.TopRow = frmOptF.vaRes.maxRows - lines + 2
            'End If
            'If StopEstimation = True Then SearchFailed = True : frmOptF.MousePointer = vbDefault : DoEvents()
        End Sub

#End Region

#Region "Private modeling code"

        Private Sub checkUseCostPenalty(nSearchBlocks As Integer)
            '  Dim TempTotVal As Double, TempEmploy As Double, TempManVal As Double, TempEcoVal As Double

            'jb Logic copied from EwE5 I'm not sure what the point of this 
            'it tells the user it is resetting the InitOption to Ecopath F's but it never resets InitOption flag
            If Me.m_searchData.InitOption <> eInitOption.EcopathBaseF Then

                Me.m_ecosim.RunModelValue(Me.TotalTime, Me.m_searchData.Frates, nSearchBlocks)

                For iflt As Integer = 1 To Me.m_searchData.NumFleets
                    If Me.m_searchData.CostRatio(iflt) > 1.15 And Me.m_searchData.UseCostPenalty = True Then
                        'EwE5 message
                        'MsgBox("Cost exceeds income for fleet " + m_core.m_EcoPathData.FleetName(iflt) + " so initial fishing efforts violate earnings > cost constraint; restarting with Ecopath base efforts", vbOKOnly, "Ecosim policy search")

                        Me.addMessage(New cFeedbackMessage("Cost exceeds income for fleet " + Me.m_core.m_EcopathData.FleetName(iflt) +
                                        " so initial fishing efforts violate earnings > cost constraint; restarting with Ecopath base efforts",
                                        eCoreComponentType.Ecosim, eMessageType.Any, eMessageImportance.Critical))
                        Exit For
                    End If  'Villy: Carl had introduced the clause above, omitting the calculation of basevalues
                Next

            End If

        End Sub

        Sub Minimize(n As Integer, X() As Double, SearchMethod As eSearchOptionTypes)
            'Sub Minimize(n As Integer, X() As Double, SearchMethod As Integer, ColorN() As Long, CritVa() As Single)
            '****************   NOTE TO FLETCH USERS   *****************************

            '       To use this program for fitting data to models, you must do the
            '       following:
            '         (1) Modify lines below as noted to name variables used by your
            '             model and data used in fitting, to set initial values
            '             for your parameter estimates x(1)...x(n), and number of
            '             parameters n to be estimated.
            '         (2) Fill in the subroutine called ReadData to name and read in
            '             any data that you want to use in the fitting; note your
            '             data variable name(s) must be declared in the SHARED statement
            '             below in this mainline program.  Note also that you cannot put
            '             DATA and READ statements in that subroutine; if you want to use
            '             that input approach, such statements must be in this mainline
            '             program, just below this starred instruction section.
            '         (3) Fill in the subroutine called func to generate your model
            '             predicted values and the value of the fitting criterion (eg,
            '             value of the sum of squares of deviations) given any values
            '             passed to the subroutine for the parameters x(1),...,x(n)
            '             by the Fletch subroutine; Fletch will call func with various
            '             values of the x's during its search for the x values that will
            '             minimize the fitting criterion.  Note that the last line in
            '             your func subroutine must be of the form func=xxx, where xxx is
            '             the calculated value of your fitting criterion.
            '         (4) the last call to func (after fitting is finished) will be with
            '             a variable called iprintresid set equal to 1 (it will be 0 for
            '             all other calls to func).  You might want to set up func so
            '             that it prints the observed and predicted values (or residuals)
            '             if iprintresid=1, perhaps to a file so that you can plot them.
            '             Alternatively, you might want to create a subroutine to print or
            '             plot them; in this case, call that routine right at the end of
            '             this main program.
            '
            '***********************************************************************

            'dimension variables to be passed between ReadData and Func here
            'replace example statements with your own variables
            'Dim x, y, z
            'Dim a, B, c(100)

            'ReDim X(n) As Double
            Try

                ReDim Me.G(n), Me.Xm(n) ', Nam$(Nmax)
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'Dimensioning strangeness
                'In flet H() dimensions are accessed in two ways 
                'Via NN which =             
                'Np = n + 1
                'NN = n * Np / 2
                'and via ib + i
                'iv = n + n
                'ib = iv + n
                'for example
                'For i = 1 To n
                '    W(ib + i) = temp * Sig / Z
                'Next i

                Dim ndims As Integer
                ndims = n * (n + 1) \ 2
                If ndims < n * 4 Then
                    ndims = n * 4
                End If
                'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                'EwE5 code
                'ReDim H(UBound(X) ^ 2 / 2 + 2), W(UBound(X) ^ 2 / 2 + 2)  'was 1000 when nmax was 100
                ReDim Me.H(ndims), Me.W(ndims)

                '061205VC: I had a model where the W would go out of bounds and bomb the optimization. it was with x = 3 letting
                'the expression be = 5.5, which caused it to bomb when the index was 6 !, so now adding 2 instead of 1 as before

                Dim F As Double, StepSize As Double, eps As Double, mode As Integer, maxfn As Integer, iprint As Integer, iexit As Integer
                Dim dfn As Double, iter As Integer, Gtol As Double
                Dim i As Integer, Estfn As Double

                'do not mess with the following parameters-used by Fletch
                'jb 24-Mar-2021 flet() was not varying the results of each search iteration enough to find any sort of a difference/gradient for each search variable.
                'Setting the step size to larger (from StepSize = 0.0001 to StepSize = 0.001) fixes this and it was able to search through the varaible space.
                'This does not affect the DFPmin() fuunction. It has it's own way of setting the step size
                StepSize = 0.0001 'default
                'StepSize = 0.000001
                Gtol = 0.0000000001 'Default

                ' VC 20250419 eps is tolerance for when to stop optimization
                ' trying to make it a bit more tolerant to speed up runs a bit
                'eps = 0.000001
                eps = 0.001

                mode = 1
                maxfn = Me.MaxNoOfIterations
                iprint = 1

                For i = 1 To X.Length - 1 : Me.Xm(i) = X(i) : Next

                Estfn = Me.FUNC(X, n)
                Me.printstats(0.0, 0, 0, Estfn, n, X, Me.G)

                If SearchMethod = eSearchOptionTypes.Fletch Then
                    Me.flet(F, X, n, Me.G, Me.H, dfn, Me.Xm, StepSize, eps, mode, Me.m_searchData.nInterations, iprint, Me.W, iexit)
                ElseIf SearchMethod = eSearchOptionTypes.DFPmin Then
                    Me.DFPmin(X, n, Gtol, iter, Me.ifn, F)
                ElseIf SearchMethod = eSearchOptionTypes.BaseProfitability Then 'search for base profitability
                    Me.SearchForBaseProfitability(X, n)
                End If

            Catch ex As Exception
                m_logger.LogError(ex, "Minimize() Error minimizing function.")
                Debug.Assert(False, ex.Message)
                Throw New ApplicationException("Minimize() Error: " & ex.Message, ex)
            End Try

        End Sub

        Sub flet(F As Double, X() As Double, n As Integer, G() As Double, H() As Double, dfn As Double, Xm() As Double,
                     hh As Double, eps As Double, mode As Integer, maxfn As Integer, iprint As Integer, W() As Double, iexit As Integer)
            '      subroutine flet(f,x,n,g,h,dfn,xm,hh,eps,
            '     *                 mode,maxfn,iprint,w,iexit,func,*)
            '      implicit real*8 (a-h,o-z)
            '      dimension x(20),g(20),h(100),w(100),xm(20)

            Dim NN As Integer
            Dim llog As Integer
            Dim Np As Integer
            Dim N1 As Integer
            Dim iss As Integer
            Dim iu As Integer
            Dim iv As Integer

            Dim ib As Integer
            Dim idiff As Integer
            Dim ij As Integer
            Dim i As Integer
            Dim j As Integer
            Dim Z As Double
            Dim i1 As Integer
            Dim zz As Double
            Dim jk As Integer
            Dim ik As Integer
            Dim K As Integer
            Dim dmin As Double
            Dim itn As Integer
            Dim link As Integer
            Dim itime As Integer
            Dim Xtime As Double
            Dim gs0 As Double
            Dim aeps As Double
            Dim Alpha As Double
            Dim FF As Double
            Dim tot As Double
            Dim intt As Integer
            Dim DF As Double
            Dim f1 As Double
            Dim f2 As Double
            Dim dgs As Double
            Dim gys As Double
            Dim Sig As Double
            Dim temp As Double

            Me.SearchFailed = False
            Me.Resline = 0
            llog = 1
            Np = n + 1
            N1 = n - 1
            NN = n * Np \ 2
            iss = n
            iu = n
            iv = n + n
            ib = iv + n
            idiff = 1
            iexit = 0
            If mode = 3 Then GoTo pta
            If mode = 2 Then GoTo ptb
            ij = CInt(NN + 1)

            For i = 1 To n
                For j = 1 To i
                    ij = ij - 1
                    H(ij) = 0
                Next j
                H(ij) = 1
            Next i

            GoTo pta

ptb:        '    continue
            ij = 1
            For i = 2 To n
                Z = H(ij)
                If Z <= 0.0! Then GoTo Ptc
                ij = ij + 1
                i1 = ij
                For j = 1 To n
                    zz = H(ij)
                    H(ij) = H(ij) / Z
                    jk = ij
                    ik = i1
                    For K = i To j
                        jk = jk + Np - K
                        H(jk) = H(jk) - H(ik) * zz
                        ik = ik + 1
                    Next K
                    ij = ij + 1
                Next j
            Next i

            If H(ij) <= 0.0! Then GoTo Ptc
pta:        '    continue

            ij = Np
            dmin = H(1)

            For i = 2 To n
                If H(ij) <= dmin Then
                    dmin = H(ij)
                End If
                ij = ij + Np - i

            Next i

            If dmin <= 0.0! Then GoTo Ptc
            Z = F
            itn = 0
            F = Me.FUNC(X, n) : If Me.SearchFailed = True Then Exit Sub
            Me.ifn = 1
            DF = dfn
            If (dfn = 0.0!) Then DF = F - Z
            If (dfn < 0.0!) Then DF = Math.Abs(DF * F)
            If (DF <= 0.0!) Then DF = 1
pte:        ' continue
            For i = 1 To n
                W(i) = X(i)
            Next i
            link = 1

            If (idiff - 1) > 0 Then
                GoTo 110
            Else
                GoTo 100
            End If

18:         '    continue
            If (Me.ifn >= maxfn) Then GoTo 90
20:         '    continue
            If (iprint = 0) Then GoTo 21
            If (itn = 0) Then GoTo 7000
            ' skip for now      IF (amod(itn, iprint) <> 0) THEN GOTO 21
            If (llog = 1) Then GoTo 7010
7003:       itime = 1
            Xtime = itime / 1000.0!
            'If MaxRuns = 1 Then
            Me.printstats(Xtime, itn, Me.ifn, F, n, X, G)
            '  If frmOptF.chkBatch.value = False Then Call printstats(Xtime, itn, ifn, F, n, X(), G())
            '    printstats(Xtime, itn, ifn, F, n, X, G)
21:         itn = itn + 1
            W(1) = -G(1)

            For i = 2 To n
                ij = i
                i1 = i - 1
                Z = -G(i)
                For j = 1 To i1
                    Z = Z - H(ij) * W(j)
                    ij = ij + n - j
                Next j
                W(i) = Z
            Next i

            W(iss + n) = W(n) / H(NN)
            ij = NN

            For i = 1 To N1
                ij = ij - 1
                Z = 0
                For j = 1 To i
                    Z = Z + H(ij) * W(iss + Np - j)
                    ij = ij - 1
                Next j
                W(iss + n - i) = W(n - i) / H(ij) - Z
            Next i

            Z = 0
            gs0 = 0

            For i = 1 To n
                If (Z * Xm(i) >= Math.Abs(W(iss + i))) Then GoTo 29
                Z = Math.Abs(W(iss + i)) / Xm(i)
29:             gs0 = gs0 + G(i) * W(iss + i)
            Next i

            iexit = 2
            If (gs0 >= 0.0!) Then GoTo 92
            aeps = eps / Z
            Alpha = -2 * DF / gs0
            If (Alpha > 1) Then Alpha = 1
            FF = F
            tot = 0
            intt = 0
            iexit = 1
30:         '    continue
            If (Me.ifn >= maxfn Or Alpha < 1.0E-20) Then GoTo 90

            For i = 1 To n
                W(i) = X(i) + Alpha * W(iss + i)
            Next i

            f1 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
            Me.ifn = Me.ifn + 1
            If (f1 >= F) Then GoTo 40
            f2 = F
            tot = tot + Alpha
32:         '    continue

            For i = 1 To n
                X(i) = W(i)
            Next i

            F = f1
            If intt - 1 > 0 Then GoTo 50
            If intt - 1 = 0 Then GoTo 49

35:         '   continue
            If (Me.ifn >= maxfn Or Alpha < 1.0E-20) Then GoTo 90

            For i = 1 To n
                W(i) = X(i) + Alpha * W(iss + i)
            Next i

            f1 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
            Me.ifn = Me.ifn + 1
            If (f1 >= F) Then GoTo 50
            If (f1 + f2 >= F + F And 7 * f1 + 5 * f2 > 12 * F) Then intt = 2
            tot = tot + Alpha
            Alpha = 2 * Alpha
            GoTo 32
40:         '    continue
            If (Alpha < aeps) Then
                GoTo 92
            End If

            If (Me.ifn >= maxfn) Then GoTo 90
            Alpha = 0.5 * Alpha

            For i = 1 To n
                W(i) = X(i) + Alpha * W(iss + i)
            Next i

            f2 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
            Me.ifn = Me.ifn + 1
            If (f2 >= F) Then GoTo 45
            tot = tot + Alpha
            F = f2

            For i = 1 To n
                X(i) = W(i)
            Next i

            GoTo 49
45:         '   continue
            Z = 0.1
            If (f1 + F > f2 + f2) Then Z = 1 + 0.5 * (F - f1) / (F + f1 - f2 - f2)
            If (Z < 0.1) Then Z = 0.1
            Alpha = Z * Alpha
            intt = 1
            GoTo 30
49:         '  continue
            If (tot < aeps) Then
                GoTo 92
            End If

50:         '   continue
            Alpha = tot

            For i = 1 To n
                W(i) = X(i)
                W(ib + i) = G(i)
            Next i

            link = 2
            If idiff > 1 Then GoTo 110
            GoTo 100
54:         '   continue
            If (Me.ifn >= maxfn) Then GoTo 90
            gys = 0

            For i = 1 To n
                W(i) = W(ib + i)
                gys = gys + G(i) * W(iss + i)
            Next i

            DF = FF - F
            dgs = gys - gs0
            If (dgs <= 0) Then GoTo 20
            link = 1
            If (dgs + Alpha * gs0 > 0.0!) Then GoTo 52

            For i = 1 To n
                W(iu + i) = G(i) - W(i)
            Next i

            Sig = 1 / (Alpha * dgs)
            GoTo 70
52:         '   continue
            zz = Alpha / (dgs - Alpha * gs0)
            Z = dgs * zz - 1

            For i = 1 To n
                W(iu + i) = Z * W(i) + G(i)
            Next i

            Sig = 1 / (zz * dgs * dgs)
            GoTo 70
60:         '    continue
            link = 2

            For i = 1 To n
                W(iu + i) = W(i)
            Next i

            If (dgs + Alpha * gs0 > 0) Then GoTo 62
            Sig = 1 / gs0
            GoTo 70
62:         '    continue
            Sig = -zz
70:         '    continue
            W(iv + 1) = W(iu + 1)

            For i = 2 To n
                ij = i
                i1 = i - 1
                Z = W(iu + i)

                For j = 1 To i1
                    Z = Z - H(ij) * W(iv + j)
                    ij = ij + n - j
                Next j
                W(iv + i) = Z
            Next i

            ij = 1

            For i = 1 To n
                temp = W(iv + i)
                Z = H(ij) + Sig * temp * temp
                If (Z <= 0) Then Z = dmin
                If (Z < dmin) Then dmin = Z
                H(ij) = Z
                W(ib + i) = temp * Sig / Z
                Sig = Sig - Z * W(ib + i) * W(ib + i)
                ij = ij + Np - i
            Next i

            ij = 1

            For i = 1 To N1
                ij = ij + 1
                i1 = i + 1

                For j = i1 To n
                    W(iu + j) = W(iu + j) - H(ij) * W(iv + i)
                    H(ij) = H(ij) + W(ib + i) * W(iu + j)
                    ij = ij + 1
                Next j
            Next i
            If link = 1 Then GoTo 60
            If link = 2 Then GoTo 20
            'go to (60,20),link
90:         '   continue
            iexit = 3
            If Me.PrintOn And Me.MaxRuns = 1 Then
                'If Alpha > 1E-20 Then frmOptF.Res.Print "maximum number of evaluations exceeded " Else frmOptF.Res.Print "can't find improving step"
                If Alpha > 1.0E-20 Then
                    Me.AddMessage("maximum number of evaluations exceeded ")
                    ' MsgBox("maximum number of evaluations exceeded ")
                Else
                    Me.AddMessage("can't find improving step")
                    ' MsgBox("can't find improving step")
                End If
            End If
            GoTo 94
92:         '    continue
            If (idiff = 2) Then GoTo 94
            idiff = 2
            GoTo pte
94:         '   continue
            If (iexit = 2) Then
                If Me.PrintOn And Me.MaxRuns = 1 Then
                    Me.AddMessage("fletch grad transpose times delta x greater than or equal zero --- eps set too small?")
                    '    MsgBox("fletch grad transpose times delta x greater than or equal zero --- eps set too small?")
                End If
                'If PrintOn = True Then frmOptF.Res.Print "fletch  grad transpose times delta x greater than or"
                'If PrintOn = True Then frmOptF.Res.Print "equal zero ---   eps set too small?"
            End If
            If (iprint = 0) Then
                Debug.Assert(False, "Exiting flet().")
                Me.AddMessage("Exiting optimization.")
                Return
            End If

            itime = 1
            Xtime = itime / 1000.0!
            'frmOptF.Res.Print "final statistics"
            'frmOptF.vaRes.maxRows = frmOptF.vaRes.maxRows + 1
            'SetCellText frmOptF.vaRes, 2, frmOptF.vaRes.maxRows, "Final statistics"
            '    If MaxRuns > 1 Then frmOptF.vaRes.maxRows = TotalRuns
            '      Call printstats(Xtime, itn, ifn, F, n, X(), G())
            'If (MaxRuns = 1 Or DoWhat = "LastRun") And ifn <= MaxNoOfIterations And frmOptF.chkBatch = False Then
            '    If PrintOn Then MsgBox("Optimization done", vbOKOnly, "EwE: optimum fishing strategy")
            '    DoWhat = ""
            'End If

            Me.printstats(Xtime, itn, Me.ifn, F, n, X, G)

            Me.AddMessage(My.Resources.CoreMessages.FPS_RUN_SUCCESS, eMessageType.Any, eMessageImportance.Information)
            ' MsgBox("Optimization done", vbOKOnly, "EwE: optimum fishing strategy")
            GoTo endline

100:        ' continue
            'VC 20250419 Carl is suggesting to set Z = 0.000001
            'Z = 0.000001  'hh * Xm(i)

            For i = 1 To n
                Z = hh * Xm(i)
                W(i) = W(i) + Z
                f1 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
                G(i) = (f1 - F) / Z
                W(i) = W(i) - Z
            Next i

            Me.ifn = Me.ifn + n
            If link = 1 Then GoTo 18
            If link = 2 Then GoTo 54
            ' go to (18,54),link
110:        '  continue

            For i = 1 To n
                Z = hh * Xm(i)
                W(i) = W(i) + Z
                f1 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
                W(i) = W(i) - Z - Z
                f2 = Me.FUNC(W, n) : If Me.SearchFailed = True Then Exit Sub
                G(i) = (f1 - f2) / (2 * Z)
                W(i) = W(i) + Z
            Next i

            Me.ifn = Me.ifn + n + n
            If link = 1 Then GoTo 18
            If link = 2 Then GoTo 54
            'go to (18,54),link
            'c *** print headings **
7000:       'If PrintOn = True Then frmOptF.Res.Print "initial statistics"
            GoTo 7003
7010:       'If PrintOn = True Then frmOptF.Res.Print "intermediate statistics"
            llog = 0
            GoTo 7003
Ptc:        If Me.PrintOn = True Then
                Me.AddMessage("fletch hessian not positive definate")
                'MsgBox("fletch hessian not positive definate")
            End If

endline:    ' '

        End Sub

        Function FUNC(X() As Double, n As Integer) As Double
            Dim i As Integer
            'Dim totval As Double, Employ As Double,ecovalue As Double, manvalue As Double,
            Dim LogUtil As Double
            Dim returnvalue As Double

            'then generate your predictions here and calculate the fitting criterion,
            'for example set sumdev=sum over observations of squared deviations between
            'predicted and observed values

            'For i = 1 To n
            '    System.Console.WriteLine(X(i))
            'Next
            'following is -log likelihood for variable linfinity growth fitting from tag data
            ' where x(1)=est linfinity, x(2)=est K

            Try

                Me.m_ecosim.RunModelValue(Me.TotalTime, X, n)

                If Me.m_searchData.FPSUseEconomicPlugin And (Me.m_pluginManager IsNot Nothing) Then
                    Me.m_pluginManager.PostRunSearchResults(Me.m_searchData)
                End If

                Me.VlocalPenalty = 0
                For i = 1 To n
                    Me.VlocalPenalty = Me.VlocalPenalty + 0.001 * X(i) ^ 2
                Next

                If Me.ProfitBase <> 0 Then Me.CritValue(eSearchCriteriaResultTypes.Profit) = CSng(Me.m_searchData.Profit / Me.ProfitBase)
                If Me.EmployBase <> 0 Then Me.CritValue(eSearchCriteriaResultTypes.Employment) = CSng(Me.m_searchData.Employ / Me.EmployBase)
                If Me.ManValueBase <> 0 Then Me.CritValue(eSearchCriteriaResultTypes.MandateReb) = CSng(Me.m_searchData.ManValue / Me.ManValueBase)
                If Me.EcoValueBase <> 0 Then Me.CritValue(eSearchCriteriaResultTypes.Ecological) = CSng(Me.m_searchData.EcoValue / Me.EcoValueBase)
                If Me.BioDivBase <> 0 Then Me.CritValue(eSearchCriteriaResultTypes.BioDiversity) = CSng(Me.m_searchData.DiversityIndex / Me.BioDivBase)

                returnvalue = Me.VlocalPenalty - Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.Profit) * Me.m_searchData.Profit / Me.ProfitBase -
                        Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.m_searchData.Employ / Me.EmployBase -
                        Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.m_searchData.ManValue / Me.ManValueBase -
                        Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.Ecological) * Me.m_searchData.EcoValue / Me.EcoValueBase -
                        Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.BioDiversity) * Me.m_searchData.DiversityIndex / Me.BioDivBase

                If Me.m_searchData.MinimizeEffortChange Then
                    If returnvalue < 0 Then
                        returnvalue = returnvalue * Me.EffortChangePenalty()
                    Else
                        returnvalue = returnvalue * (1 / Me.EffortChangePenalty())
                    End If
                End If

                If Me.m_searchData.LimitFishingMortality Then returnvalue = returnvalue * Me.LimitFPenalty()

                If Me.m_searchData.PortFolio = True Then
                    'calculate general log utility for net economic value
                    'sets to quadratic function with continuous derivative if critvalue is <-.5
                    If Me.CritValue(1) + 1 > 0.5 Then
                        LogUtil = Math.Log(Me.CritValue(1) + 1)
                    Else
                        LogUtil = Math.Log(0.5) + 1 / 0.5 * (Me.CritValue(1) + 1 - 0.5) - 1 / 0.25 * (Me.CritValue(1) + 1 - 0.5) ^ 2
                    End If
                    returnvalue = -Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.Profit) * LogUtil +
                                   Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.m_searchData.Ecodistance -
                                   Me.m_searchData.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.ExistValue
                End If

                'Is the objective function value a valid number
                If Double.IsNaN(returnvalue) Or Double.IsInfinity(returnvalue) Then
                    'Nope...
                    'figure out which criteria value is an invalid number
                    'and dump it to the log
                    Dim enumNames As String
                    For icrt As Integer = 0 To Me.CritValue.Length - 1
                        If Double.IsNaN(Me.CritValue(icrt)) Or Double.IsInfinity(Me.CritValue(icrt)) Then
                            Dim enumname As String = [Enum].GetName(GetType(eSearchCriteriaResultTypes), icrt)
                            enumNames += enumname + " "
                            m_logger.LogError("Fishing Policy Search criteria value " + enumname + " is invalid.")
                        End If
                    Next

                    'If there was an error the returnvalue will not matter
                    'SearchFailed will force the search to stop
                    returnvalue = 1.0E+20
                    Me.SearchFailed = True
                    Me.AddMessage("Fishing Policy Search Error: Invalid optimization value for " + enumNames, eMessageType.ErrorEncountered)
                End If

                Return returnvalue

            Catch ex As Exception
                'If there was an error the returnvalue will not matter
                'SearchFailed will force the search to stop
                FUNC = 1.0E+20
                Me.SearchFailed = True

                m_logger.LogError(ex, "Fishing Policy Search Aborted due to Error.")
                Me.AddMessage("Fishing Policy Search Error: " & ex.Message, eMessageType.ErrorEncountered)

            End Try

        End Function

        Sub DFPmin(P() As Double, n As Integer, FTOL As Double, iter As Integer, ift As Integer, FRET As Double)
            Dim Itmax As Integer, eps As Double, Fp As Double, i As Integer, j As Integer
            Dim its As Integer, Fac As Double, Fae As Double, Fad As Double, Dum As Double
            Me.SearchFailed = False
            Me.ifn = 0
            Itmax = Me.MaxNoOfIterations '200
            eps = 0.0000000001
            Dim Hessin(,) As Double, xi() As Double, G() As Double, dg() As Double, hdg() As Double
            ReDim Hessin(n, n), xi(n), G(n), dg(n), hdg(n)

            Fp = Me.FUNC2(P, n)
            Me.DFUNC(P, G, n)

            For i = 1 To n
                For j = 1 To n
                    Hessin(i, j) = 0.0!
                Next j
                Hessin(i, i) = 1.0!
                xi(i) = -G(i)
            Next i

            For its = 1 To Itmax
                iter = its
                Me.LINMIN(P, xi, n, FRET)
                If 2.0! * Math.Abs(FRET - Fp) <= FTOL * (Math.Abs(FRET) + Math.Abs(Fp) + eps) Then
                    Erase hdg, dg, G, xi, Hessin
                    ift = Me.ifn
                    Exit Sub
                End If
                Fp = FRET
                For i = 1 To n
                    dg(i) = G(i)
                Next i
                FRET = Me.FUNC2(P, n)
                Me.DFUNC(P, G, n)
                For i = 1 To n
                    dg(i) = G(i) - dg(i)
                Next i
                For i = 1 To n
                    hdg(i) = 0.0!
                    For j = 1 To n
                        hdg(i) = hdg(i) + Hessin(i, j) * dg(j)
                    Next j
                Next i
                Fac = 0.0!
                Fae = 0.0!
                For i = 1 To n
                    Fac = Fac + dg(i) * xi(i)
                    Fae = Fae + dg(i) * hdg(i)
                Next i
                Fac = 1.0! / Fac
                Fad = 1.0! / Fae
                For i = 1 To n
                    dg(i) = Fac * xi(i) - Fad * hdg(i)
                Next i
                For i = 1 To n
                    For j = 1 To n
                        Dum = Fac * xi(i) * xi(j) - Fad * hdg(i) * hdg(j) + Fae * dg(i) * dg(j)
                        Hessin(i, j) = Hessin(i, j) + Dum
                    Next j
                Next i
                For i = 1 To n
                    xi(i) = 0.0!
                    For j = 1 To n
                        xi(i) = xi(i) - Hessin(i, j) * G(j)
                    Next j
                Next i
                Me.printstats(0, its, Me.ifn, FRET, n, P, G)
                ift = Me.ifn
                If Me.SearchFailed = True Then Exit Sub

            Next its
            ift = Me.ifn
            'frmOptF.Res.Print "too many iterations in DFPMIN"
            Me.AddMessage("too many iterations in DFPMIN")
            '  MsgBox("too many iterations in DFPMIN")
        End Sub


        Sub DFUNC(X() As Double, ByRef DF() As Double, n As Integer)
            Dim Dstep As Double, Fbase As Double, i As Integer
            'DF(1) = 2 * X(1) - 0.9 * X(2)
            'DF(2) = 2 * X(2) + -0.9 * X(1)
            Dstep = 0.000001
            Fbase = Me.FUNC2(X, n)
            For i = 1 To n
                X(i) = X(i) + Dstep
                DF(i) = (Me.FUNC2(X, n) - Fbase) / Dstep
                X(i) = X(i) - Dstep
            Next
        End Sub

        Function FUNC2(X() As Double, n As Integer) As Double
            'FUNC2 = X(1) ^ 2 + X(2) ^ 2 - 0.9 * X(1) * X(2)
            FUNC2 = Me.FUNC(X, n)
            Me.ifn = Me.ifn + 1
        End Function

        Sub LINMIN(ByRef P() As Double, ByRef xi() As Double, n As Integer, ByRef FRET As Double)
            Dim Tol As Double, j As Integer, Ax As Double, XX As Double, Fa As Double
            Dim Fb As Double, Fx As Double, Dum As Double, Bx As Double
            Dim Xmin As Double
            Tol = 0.0001
            Me.ncom = n
            For j = 1 To n
                Me.pcom(j) = P(j)
                Me.xicom(j) = xi(j)
            Next j
            Ax = 0.0!
            XX = 1.0!
            Me.MNBRAK(Ax, XX, Bx, Fa, Fx, Fb, Dum)
            FRET = Me.BRENT(Ax, XX, Bx, Dum, Tol, Xmin)
            For j = 1 To n
                xi(j) = Xmin * xi(j)
                P(j) = P(j) + xi(j)
            Next j
        End Sub

        Sub MNBRAK(ByRef Ax As Double, ByRef Bx As Double, ByRef cx As Double, ByRef Fa As Double, ByRef Fb As Double, ByRef FC As Double, ByRef Dum As Double)
            Dim Q As Double, R As Double, Gold As Double, Glimit As Double, Tiny As Double
            Dim U As Double, Ulim As Double, Fu As Double, done As Boolean
            Gold = 1.618034
            Glimit = 100.0!
            Tiny = 1.0E-20
            Fa = Me.FUNC(Ax)
            Fb = Me.FUNC(Bx)
            If Fb > Fa Then
                Dum = Ax
                Ax = Bx
                Bx = Dum
                Dum = Fb
                Fb = Fa
                Fa = Dum
            End If
            cx = Bx + Gold * (Bx - Ax)
            FC = Me.FUNC(cx)
            Do
                If Fb < FC Then Exit Do
                done = True '-1
                R = (Bx - Ax) * (Fb - FC)
                Q = (Bx - cx) * (Fb - Fa)
                Dum = Q - R
                If Math.Abs(Dum) < Tiny Then Dum = Tiny
                U = Bx - ((Bx - cx) * Q - (Bx - Ax) * R) / (2.0! * Dum)
                Ulim = Bx + Glimit * (cx - Bx)
                If (Bx - U) * (U - cx) > 0.0! Then
                    Fu = Me.FUNC(U)
                    If Fu < FC Then
                        Ax = Bx
                        Fa = Fb
                        Bx = U
                        Fb = Fu
                        Exit Sub
                    ElseIf Fu > Fb Then
                        cx = U
                        FC = Fu
                        Exit Sub
                    End If
                    U = cx + Gold * (cx - Bx)
                    Fu = Me.FUNC(U)
                ElseIf (cx - U) * (U - Ulim) > 0.0! Then
                    Fu = Me.FUNC(U)
                    If Fu < FC Then
                        Bx = cx
                        cx = U
                        U = cx + Gold * (cx - Bx)
                        Fb = FC
                        FC = Fu
                        Fu = Me.FUNC(U)
                    End If
                ElseIf (U - Ulim) * (Ulim - cx) >= 0.0! Then
                    U = Ulim
                    Fu = Me.FUNC(U)
                Else
                    U = cx + Gold * (cx - Bx)
                    Fu = Me.FUNC(U)
                End If
                If done Then
                    Ax = Bx
                    Bx = cx
                    cx = U
                    Fa = Fb
                    Fb = FC
                    FC = Fu
                Else
                    done = False '0
                End If
            Loop While Not done
        End Sub

        Function BRENT(ByRef Ax As Double, ByRef Bx As Double, ByRef cx As Double, ByRef Dum As Double, ByRef Tol As Double, ByRef Xmin As Double) As Double
            Dim Itmax As Integer, Cgold As Double, Zeps As Double, A As Double, B As Double
            Dim v As Double, W As Double, X As Double, E As Double, Fx As Double
            Dim Fval As Double, Fw As Double, iter As Integer, done As Boolean
            Dim Xm As Double, Tol1 As Double, Tol2 As Double, R As Double, P As Double, Q As Double
            Dim d As Double, Etemp As Double, U As Double, Fu As Double
            Itmax = 100
            Cgold = 0.381966
            Zeps = 0.0000000001
            A = Ax
            If cx < Ax Then A = cx
            B = Ax
            If cx > Ax Then B = cx
            v = Bx
            W = v
            X = v
            E = 0.0!
            Fx = Me.FUNC(X)
            Fval = Fx
            Fw = Fx
            For iter = 1 To Itmax
                Xm = 0.5 * (A + B)
                Tol1 = Tol * Math.Abs(X) + Zeps
                Tol2 = 2.0! * Tol1
                If Math.Abs(X - Xm) <= Tol2 - 0.5 * (B - A) Then Exit For
                done = True '-1
                If Math.Abs(E) > Tol1 Then
                    R = (X - W) * (Fx - Fval)
                    Q = (X - v) * (Fx - Fw)
                    P = (X - v) * Q - (X - W) * R
                    Q = 2.0! * (Q - R)
                    If Q > 0.0! Then P = -P
                    Q = Math.Abs(Q)
                    Etemp = E
                    E = d
                    Dum = Math.Abs(0.5 * Q * Etemp)
                    If Math.Abs(P) < Dum And P > Q * (A - X) And P < Q * (B - X) Then
                        d = P / Q
                        U = X + d
                        If U - A < Tol2 Or B - U < Tol2 Then d = Math.Abs(Tol1) * Math.Sign(Xm - X)
                        done = False '0
                    End If
                End If
                If done Then
                    If X >= Xm Then
                        E = A - X
                    Else
                        E = B - X
                    End If
                    d = Cgold * E
                End If
                If Math.Abs(d) >= Tol1 Then
                    U = X + d
                Else
                    U = X + Math.Abs(Tol1) * Math.Sign(d)
                End If
                Fu = Me.FUNC(U)
                If Fu <= Fx Then
                    If U >= X Then
                        A = X
                    Else
                        B = X
                    End If
                    v = W
                    Fval = Fw
                    W = X
                    Fw = Fx
                    X = U
                    Fx = Fu
                Else
                    If U < X Then
                        A = U
                    Else
                        B = U
                    End If
                    If Fu <= Fw Or W = X Then
                        v = W
                        Fval = Fw
                        W = U
                        Fw = Fu
                    ElseIf Fu <= Fval Or v = X Or v = W Then
                        v = U
                        Fval = Fu
                    End If
                End If
            Next iter
            'If iter > Itmax Then frmOptF.Res.Print "Brent exceed maximum iterations.": End
            If iter > Itmax Then Me.AddMessage("Brent exceed maximum iterations.") : Exit Function 'End
            Xmin = X
            BRENT = Fx
        End Function

        Private Function FUNC(X As Double) As Double
            FUNC = Me.F1DIM(X)
        End Function

        Function F1DIM(X As Double) As Double
            Dim XT(50) As Double, j As Integer
            For j = 1 To Me.ncom
                XT(j) = Me.pcom(j) + X * Me.xicom(j)
            Next j
            F1DIM = Me.FUNC2(XT, Me.ncom)
            Erase XT
        End Function

        Sub SearchForBaseProfitability(X() As Double, n As Integer)
            ' Dim totval As Double, Employ As Double, manvalue As Double, ecovalue As Double
            Dim BaseIncome() As Single, Temp As Double
            Dim CostToI() As Single, GainToJ(,) As Single, iter As Integer
            Dim PaidToJ() As Single
            Dim tcost As Single
            Dim Xtime As Double
            Dim RelaxWt As Single
            Dim GroMax As Single
            Dim Delp() As Single, LastX As Double, DelX() As Double, LastP() As Single, DpDx() As Double
            Dim i As Integer, j As Integer, K As Integer, SpGaintoJ As Single
            Dim gro As Double, SumGro As Double

            Dim epdata As cEcopathDataStructures = Me.m_core.m_EcopathData

            'exit if search is not over gear types
            If n <> Me.m_searchData.NumFleets Then
                Me.m_core.Messages.SendMessage(New cMessage("This search method only allows you to search over all fleets. You must set the search blocks to one block per fleet.",
                                                    eMessageType.ErrorEncountered, eCoreComponentType.FishingPolicySearch, eMessageImportance.Warning))
                Exit Sub
            End If

            RelaxWt = 0.5
            GroMax = 0.3
            Me.PropToPlaintiff = 0.0

            Dim BaseIncomeSpecies(,) As Single
            ReDim BaseIncome(Me.m_searchData.NumFleets), BaseIncomeSpecies(Me.m_searchData.NumFleets, Me.m_searchData.NumGroups) ', BaseEffort(m_searchData.NumFleets)
            ReDim CostToI(Me.m_searchData.NumFleets), GainToJ(Me.m_searchData.NumFleets, Me.m_searchData.NumFleets), PaidToJ(Me.m_searchData.NumFleets), Me.PaidToJbyI(Me.m_searchData.NumFleets, Me.m_searchData.NumFleets)
            ReDim Me.Profitability(Me.m_searchData.NumFleets), Me.G(Me.m_searchData.NumFleets)
            Dim tincome As Single, Dummy As Double, nch As Integer
            ReDim Delp(Me.m_searchData.NumFleets), DelX(Me.m_searchData.NumFleets), LastP(Me.m_searchData.NumFleets), DpDx(Me.m_searchData.NumFleets)

            'varies fishing efforts so as to try and achieve baseprofitability for each fleet, while accounting
            'for transfer costs from fleets that cause reduced income to the fleets impacted by such reductions

            Do
                iter = iter + 1
                'get base incomes and costs for this iteration
                Dummy = Me.FUNC(X, n)

                'LastYearIncome() and LastYearIncomeSpecies() set by Ecosim.RunModelValue called by FUNC()
                For i = 1 To Me.m_searchData.NumFleets
                    BaseIncome(i) = Me.m_searchData.LastYearIncome(i)
                    For K = 1 To Me.m_searchData.NumGroups
                        BaseIncomeSpecies(i, K) = Me.m_searchData.LastYearIncomeSpecies(i, K)
                    Next
                Next

                'then get gains to each gear j of eliminating gear i, while accumulating negative gains
                'as costs to gear i
                ReDim PaidToJ(Me.m_searchData.NumFleets), Me.PaidToJbyI(Me.m_searchData.NumFleets, Me.m_searchData.NumFleets)
                For i = 1 To Me.m_searchData.NumFleets
                    Temp = X(i)
                    'turn off gear i temporarily and make a run
                    X(i) = -5
                    Me.m_ecosim.RunModelValue(Me.TotalTime, X, n)
                    CostToI(i) = 0
                    For j = 1 To Me.m_searchData.NumFleets
                        GainToJ(i, j) = Me.m_searchData.LastYearIncome(j) - BaseIncome(j)

                        If Me.m_searchData.IncludeCompetitiveImpact Then

                            If GainToJ(i, j) > 0 Then
                                CostToI(i) = CostToI(i) + GainToJ(i, j)
                                PaidToJ(j) = PaidToJ(j) + GainToJ(i, j)
                                Me.PaidToJbyI(i, j) = GainToJ(i, j)
                            End If

                        Else
                            For K = 1 To Me.m_searchData.NumGroups
                                If BaseIncomeSpecies(i, K) = 0 Then
                                    SpGaintoJ = Me.m_searchData.LastYearIncomeSpecies(j, K) - BaseIncomeSpecies(j, K)
                                    If SpGaintoJ > 0 Then
                                        CostToI(i) = CostToI(i) + SpGaintoJ
                                        PaidToJ(j) = PaidToJ(j) + SpGaintoJ
                                        Me.PaidToJbyI(i, j) = Me.PaidToJbyI(i, j) + SpGaintoJ
                                    End If
                                End If
                            Next
                        End If
                    Next
                    'restore log effort for gear i
                    X(i) = Temp
                Next
                For i = 1 To Me.m_searchData.NumFleets
                    Me.m_searchData.LastYearIncome(i) = BaseIncome(i)
                Next

                Me.updateBaseProfitabilityResults()
                'If StopEstimation = False Then ShowFleetCosts()

                'now calculate profitabilities if all gears were charged for their costs to other gears
                'and increment/decrement log effort in proportion to excess of profitability over target
                SumGro = 0
                nch = 0
                For i = 1 To Me.m_searchData.NumFleets
                    ' JS_todo: incorporate ecsim EffortCost and SailCost here?
                    tcost = CSng((epdata.cost(i, eCostIndex.CUPE) + epdata.cost(i, eCostIndex.Sail)) * Math.Exp(X(i)) + CostToI(i) + 0.0000000001)
                    tincome = CSng(BaseIncome(i) + Me.PropToPlaintiff * PaidToJ(i) + 0.0000000001)

                    Me.Profitability(i) = (tincome - tcost) / tincome - Me.m_searchData.TargetProfitability(i)
                    LastX = X(i)
                    If Math.Abs(DelX(i)) < 0.1 Then
                        'use simple step based on profitability
                        gro = Me.Profitability(i)
                        If gro > GroMax Then gro = GroMax
                        If gro < -GroMax Then gro = -GroMax

                        X(i) = X(i) + RelaxWt * gro
                    Else
                        'use linear projection step based on dprofitability/dX
                        Delp(i) = Me.Profitability(i) - LastP(i)
                        DpDx(i) = Delp(i) / DelX(i)
                        If Math.Abs(DpDx(i)) > 0.01 Then
                            gro = -LastP(i) / (DpDx(i))
                            If gro > GroMax Then gro = GroMax
                            If gro < -GroMax Then gro = -GroMax
                            X(i) = X(i) + RelaxWt * gro
                        Else
                            gro = 0.000001
                            X(i) = X(i) + gro
                        End If
                    End If
                    If X(i) < -5 Then X(i) = -5
                    DelX(i) = X(i) - LastX
                    SumGro = SumGro + Math.Abs(DelX(i))
                    If Math.Abs(gro) > 0.01 Then nch = nch + 1
                    LastP(i) = Me.Profitability(i)
                Next

                Me.printstats(Xtime, iter, Me.m_searchData.NumFleets * iter, Dummy, n, X, Me.G)

                If SumGro < 0.01 Or nch = 0 Or iter > 100 Or Me.StopEstimation Then Exit Do
            Loop

            'do one last model run to set fishing rates to final optimum found
            Dummy = Me.FUNC(X, n)

        End Sub

        Private Sub updateBaseProfitabilityResults()

            Try

                For iflt As Integer = 1 To Me.Results.nFleets

                    Me.Results.Income(iflt) = Me.m_searchData.LastYearIncome(iflt)
                    Me.Results.Profitability(iflt) = Me.Profitability(iflt)

                    For iflt2 As Integer = 1 To Me.Results.nFleets
                        Me.Results.CompensationMatrix(iflt, iflt2) = CSng(Me.PaidToJbyI(iflt, iflt2) / (Me.m_searchData.LastYearIncome(iflt) + 1.0E-20))
                    Next iflt2

                Next iflt

            Catch ex As Exception
                m_logger.LogError(ex, "updateBaseProfitabilityResults. Error updating Fishing Policy Search Base Profitability results.")
                Debug.Assert(False)
            End Try

        End Sub

        ''' <summary>
        ''' Compute a penalty multiplier if change in effort from the last year to this year is greater than MaxEffortChange
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>The penalty is the sum of change ratio from year to year  penalty = penalty * (CurEffort(i) / LastEffort(i))</remarks>
        Private Function EffortChangePenalty() As Single
            Dim i As Integer
            Dim iyr As Integer
            Dim CurEffort() As Single
            Dim LastEffort() As Single
            Dim penalty As Single
            ReDim CurEffort(Me.m_searchData.NumFleets)
            ReDim LastEffort(Me.m_searchData.NumFleets)

            penalty = 1 'default return value 

            For i = 1 To Me.m_searchData.NumFleets
                For iyr = 2 To Me.TotalTime
                    If Me.m_searchData.FblockCode(i, iyr) > 0 Then
                        LastEffort(i) = Me.m_core.m_EcoSimData.FishRateGear(i, 12 * iyr - 23)
                        CurEffort(i) = Me.m_core.m_EcoSimData.FishRateGear(i, 12 * iyr - 11)

                        If CurEffort(i) > 0 And LastEffort(i) > 0 Then

                            If CurEffort(i) > LastEffort(i) * Me.m_searchData.MaxEffortChange Then
                                penalty = penalty * (LastEffort(i) / CurEffort(i))
                            ElseIf LastEffort(i) > CurEffort(i) * Me.m_searchData.MaxEffortChange Then
                                penalty = penalty * (CurEffort(i) / LastEffort(i))
                            End If

                        End If

                    End If
                Next
            Next

            Return penalty

#If EWE5_CODE Then
'EwE5 code could return zero in some cases
            EffortChangePenalty = 1
'Exit Function
For i = 1 To NumGear
    LastEffort(i) = FishRateGear(i, 12 * 1 - 11)
Next
For i = 1 To NumGear
    For iyr = 2 To TotalTime
        If FblockCode(i, iyr) > 0 Then
            LastEffort(i) = FishRateGear(i, 12 * iyr - 23)
            CurEffort(i) = FishRateGear(i, 12 * iyr - 11)
            If CurEffort(i) > LastEffort(i) * MaxEffortChange And CurEffort(i) > 0 Then
                EffortChangePenalty = EffortChangePenalty * (LastEffort(i) / CurEffort(i)) '^ (1 / 2)
                'FishRateGear(i, 12 * iyr - 11) = LastEffort(i) / MaxEffortChange
            ElseIf LastEffort(i) > CurEffort(i) * MaxEffortChange And LastEffort(i) > 0 Then
                EffortChangePenalty = EffortChangePenalty * (CurEffort(i) / LastEffort(i)) '^ (1 / 2)
                'FishRateGear(i, 12 * iyr - 11) = LastEffort(i) * MaxEffortChange
            End If
            LastEffort(i) = CurEffort(i)
        End If
    Next
Next

#End If

        End Function

        Private Function LimitFPenalty() As Single
            Dim i As Integer
            Dim maxF As Single
            Dim Grp As Integer
            LimitFPenalty = 1
            Dim tSteps As Integer = Me.m_core.nEcosimTimeSteps
            For Grp = 1 To Me.m_core.nLivingGroups
                If Me.m_searchData.FLimit(Grp) < 1000 And Me.m_searchData.FLimit(Grp) > 0 Then
                    maxF = 0
                    '080610VC: changed the time loop below to start at baseyear+1, 
                    'we're not interested in what happened earlier when doing a search
                    'Also, changed it to annual steps, since effort is annual; faster!
                    For i = 12 * (Me.m_searchData.BaseYear + 1) To tSteps Step 12
                        If Me.m_core.m_EcoSimData.FishRateNo(Grp, i) > Me.m_searchData.FLimit(Grp) Then
                            If Me.m_core.m_EcoSimData.FishRateNo(Grp, i) > maxF Then
                                maxF = Me.m_core.m_EcoSimData.FishRateNo(Grp, i)
                            End If
                        End If
                    Next
                    If maxF > 0 Then LimitFPenalty = CSng(LimitFPenalty * (Me.m_searchData.FLimit(Grp) / maxF) ^ 2) ': Stop
                End If
            Next

        End Function

#End Region

    End Class

#End Region

#Region "Results Object"

    ''' <summary>
    ''' This is a wrapper for the Fishing Policy Search time step results
    ''' </summary>
    ''' <remarks></remarks>
    Public Class cFPSSearchResults

        Public BlockResults() As Single
        Public BlockNumber() As Integer

        Public CriteriaValues(cSearchDatastructures.N_CRIT_RESULTS) As Single
        Public Totals As Single
        Public nCalls As Integer

        'output variables for base profitability
        Public Income() As Single
        Public Profitability() As Single
        Public CompensationMatrix(,) As Single

        Private m_nblocks As Integer
        Private m_nFleets As Integer

        Friend Sub New(NumberOfBlocks As Integer, NumberOfFleets As Integer)

            Me.m_nblocks = NumberOfBlocks
            Me.m_nFleets = NumberOfFleets

            Me.RedimBlocks()
            Me.RedimBaseProfitability()

        End Sub

        Private Sub RedimBlocks()
            ReDim Me.BlockResults(Me.m_nblocks)
            ReDim Me.BlockNumber(Me.m_nblocks)
        End Sub

        Private Sub RedimBaseProfitability()

            ReDim Me.Income(Me.m_nFleets)
            ReDim Me.Profitability(Me.m_nFleets)
            ReDim Me.CompensationMatrix(Me.m_nFleets, Me.m_nFleets)

        End Sub

        Public Property nBlocks() As Integer
            Get
                Return Me.m_nblocks
            End Get
            Friend Set(value As Integer)
                Me.m_nblocks = value
                Me.RedimBlocks()
            End Set
        End Property

        Public ReadOnly Property nFleets() As Integer
            Get
                Return Me.m_nFleets
            End Get
        End Property

        Friend Sub Clear()
            Me.Totals = 0
            Me.nCalls = 0

            Array.Clear(Me.Income, 0, Me.Income.Length)
            Array.Clear(Me.Profitability, 0, Me.Profitability.Length)
            Array.Clear(Me.BlockResults, 0, Me.BlockResults.Length)

            Array.Clear(Me.CompensationMatrix, 0, Me.CompensationMatrix.Length)

        End Sub

    End Class

#End Region

End Namespace

