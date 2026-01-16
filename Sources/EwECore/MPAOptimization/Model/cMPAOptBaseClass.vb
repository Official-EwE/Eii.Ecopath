' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'


Imports System.IO
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public MustInherit Class cMPAOptBaseClass
    Implements IMPASearchModel

#Region "Must Overide Methods"

    Public MustOverride Sub Run() Implements IMPASearchModel.Run

    Public MustOverride ReadOnly Property OKtoRun() As Boolean Implements IMPASearchModel.OKtoRun

#End Region

#Region "Private data"

    Const N_MAX_RESULTS As Integer = 500
    Const RESULTS_TO_KEEP As Integer = N_MAX_RESULTS \ 2

    Protected m_EcoSpace As cEcoSpace
    Friend m_SpaceData As cEcospaceDataStructures

    Protected m_data As cMPAOptDataStructures
    Protected m_search As cSearchDatastructures

    Protected m_bRunning As Boolean
    Protected m_esStartTime As Single
    Protected EcoSeedOn As Boolean

    'results of each iterations
    Protected m_lstObjectiveResults As New List(Of cObjectiveResult)

    Protected m_cellComputedCallback As cMPAOptManager.SearchIterationDelegate
    Protected m_StateCallback As cMPAOptManager.SearchRunStateDelegate
    Protected m_SendMessageDelegate As cMPAOptManager.SendMessageDelegate

    '''' <summary>Best results of the current run</summary>
    'Private m_bestResults As cObjectiveResult

    Protected CumulativeCellWeight() As Double
    Protected CellWgt() As Double

    Protected CellCount As Integer
    Protected m_nIters As Integer 'number of iteration completed

    Protected m_runstate As cMPAOptManager.eRunStates = cMPAOptManager.eRunStates.Idle

    ' -- Autosave settings --

    ''' <summary>Auto-save file name.</summary>
    Protected m_OutputFilename As String
    ''' <summary>Flag, stating whether autosave is enabled.</summary>
    Protected m_bAutosaveResults As Boolean = False
    ''' <summary>Auto-save folder.</summary>
    Protected m_strOutputPath As String = ""
    ''' <summary>Auto-save file header.</summary>
    Protected m_strHeader As String = ""

    ''' <summary>MPA lookup</summary>
    Protected IsMPA(,) As Boolean
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMPAOptBaseClass)()

#Region "Modeling data from EwE5"

    Protected BOrig(,,) As Single
    Protected FOrig(,,) As Single
    Protected WOrig(,,) As Single
    Protected Blastseed(,,) As Single

    Public StoreBtimeForEcoSeed() As Single

    Protected TotWeightedValueBase As Single
    'Protected EmployBase As Single, TotValBase As Single, ManValueBase As Single, EcoValueBase As Single, DiversityBase As Single, AreaBoundBase As Single
    Protected EmployBase As Double, ProfitBase As Double, ManValueBase As Double, EcoValueBase As Double, DiversityBase As Double, AreaBoundBase As Double

    Protected TargetSumMax As Single

    Protected AreaBoundary As Single


#End Region

#End Region

#Region "Construction and Initialization"


    Sub New()

    End Sub

    Public Overridable Function Init(ByRef EcoSpaceModel As cEcoSpace, ByRef MPAOptData As cMPAOptDataStructures) As Boolean Implements IMPASearchModel.Init

        Try

            Me.m_EcoSpace = EcoSpaceModel
            Me.m_data = MPAOptData

            Me.m_SpaceData = Me.m_EcoSpace.EcoSpaceData
            Me.m_search = Me.m_EcoSpace.SearchData

            'set EcoSpace to use this MPA optimization model
            Me.m_EcoSpace.MPAOptimization = Me

            'the seed array can be needed before the model is run
            ReDim Me.m_data.MPASeed(Me.m_SpaceData.InRow + 1, Me.m_SpaceData.InCol + 1)

        Catch ex As Exception
            m_logger.LogError(ex, "init")
            Return False
        End Try

        Return True

    End Function


    Protected Sub InitIsMPA()
        Dim nRows As Integer = Me.m_SpaceData.InRow
        Dim nCols As Integer = Me.m_SpaceData.InCol
        Me.IsMPA = New Boolean(nRows + 1, nCols + 1) {}

        For i As Integer = 1 To nRows
            For j As Integer = 1 To nCols
                ' make snapshot of MPA cell occupation for quick lookup during computations
                Me.IsMPA(i, j) = False
                For k As Integer = 1 To Me.m_SpaceData.MPAno
                    Me.IsMPA(i, j) = Me.IsMPA(i, j) Or (Me.m_SpaceData.MPA(k)(i, j) > 0)
                Next
            Next
        Next
    End Sub

    Public Overridable Sub Connect(OnSearchInteration As cMPAOptManager.SearchIterationDelegate, _
                       OnRunStateChanged As cMPAOptManager.SearchRunStateDelegate, _
                       OnSendMessage As cMPAOptManager.SendMessageDelegate) Implements IMPASearchModel.Connect
        Me.m_cellComputedCallback = OnSearchInteration
        Me.m_StateCallback = OnRunStateChanged
        Me.m_SendMessageDelegate = OnSendMessage
    End Sub


#End Region

#Region "Public Properties and Methods"

    Public Overridable Property MPAOptData() As cMPAOptDataStructures Implements IMPASearchModel.MPAOptData
        Get
            Return Me.m_data
        End Get
        Set(value As cMPAOptDataStructures)
            Me.m_data = value
        End Set
    End Property

    Public Overridable ReadOnly Property EcospaceStartTime() As Single Implements IMPASearchModel.EcospaceStartTime
        Get

            If Not Me.m_bRunning Then
                'this got called even though Ecoseed is not running this should NOT happen
                'Oh well return zero this should be the default start time for ecospace
                Return 0
            End If

            If Me.nInterationCompleted > 0 Then
                'if Ecoseed has already run Ecospace 
                'then start the time loop at the start of the first summary time period
                'This should change to Ecoseed having its own start and end time instead of using the the summary time periods
                Return Me.m_data.EcoSpaceStartYear
            Else
                'This is the first time the optimization will run Ecospace
                'Ecospace needs to run for the entire time period to set the base values
                Return 0
            End If

        End Get
    End Property


    Public Overridable ReadOnly Property isRunning() As Boolean Implements IMPASearchModel.isRunning
        Get
            Return Me.m_bRunning
        End Get
    End Property

    Public Overridable Sub StopRun() Implements IMPASearchModel.StopRun
        Me.m_data.StopRun = True
    End Sub

    Public Overridable Sub clearMPAs() Implements IMPASearchModel.clearMPAs
        For ir As Integer = 1 To Me.m_SpaceData.InRow
            For ic As Integer = 1 To Me.m_SpaceData.InCol
                'm_SpaceData.MPA(ir, ic) = 0
                For impa As Integer = 1 To Me.m_SpaceData.MPAno
                    Me.m_SpaceData.MPA(impa)(ir, ic) = 0
                Next

            Next ic
        Next ir
    End Sub

    Public Overridable Sub clearSeedCells() Implements IMPASearchModel.clearSeedCells
        For ir As Integer = 1 To Me.m_SpaceData.InRow
            For ic As Integer = 1 To Me.m_SpaceData.InCol
                Me.m_data.MPASeed(ir, ic) = 0
            Next ic
        Next ir
    End Sub


    Public Overridable Function setAllCellsToMPA(MAPIndex As Integer) As Boolean Implements IMPASearchModel.setAllCellsToMPA

        'make sure the MPA index supplied by the user is in bounds
        If MAPIndex > 0 And MAPIndex <= Me.m_SpaceData.MPAno Then
            Dim impanew As Integer
            For impa As Integer = 1 To Me.m_SpaceData.MPAno
                impanew = 0
                If impa = MAPIndex Then
                    impanew = 1
                End If
                For ir As Integer = 1 To Me.m_SpaceData.InRow
                    For ic As Integer = 1 To Me.m_SpaceData.InCol
                        Me.m_SpaceData.MPA(impa)(ir, ic) = impanew
                    Next ic
                Next ir
            Next
            Return True
        Else
            'invalid MPA index
            Return False
        End If

    End Function

    Public Overridable Function setAllCellsToSeed(iMPA As Integer) As Boolean Implements IMPASearchModel.setAllCellsToSeed

        'make sure the MPA index supplied by the user is in bounds
        If iMPA > 0 And iMPA <= Me.m_SpaceData.MPAno Then
            For ir As Integer = 1 To Me.m_SpaceData.InRow
                For ic As Integer = 1 To Me.m_SpaceData.InCol
                    Me.m_data.MPASeed(ir, ic) = iMPA
                Next ic
            Next ir
            Return True
        Else
            'invalid MPA index
            Return False
        End If
    End Function


    Public Overridable ReadOnly Property Results() As System.Collections.Generic.List(Of cObjectiveResult) Implements IMPASearchModel.Results
        Get
            Return Me.m_lstObjectiveResults
        End Get
    End Property

    Public Overridable ReadOnly Property nInterationCompleted() As Integer Implements IMPASearchModel.nInterationsCompleted
        Get
            Return Me.m_nIters
        End Get
    End Property


    ''' <inheritdocs cref="IMPASearchModel.ConfigureAutosave"/>
    Public Overridable Sub ConfigureAutosave(bAutosave As Boolean, strOutputPath As String, strHeader As String) _
        Implements IMPASearchModel.ConfigureAutosave
        Me.m_bAutosaveResults = bAutosave
        Me.m_strOutputPath = strOutputPath
        Me.m_strHeader = strHeader
    End Sub

#End Region

#Region "Running the model"


    Friend Overridable Function EvaluateRun() As Single
        Dim curSum As Double 'results of the search run

        Try

            curSum = Me.m_search.ValWeight(eSearchCriteriaResultTypes.Profit) * Me.m_search.Profit / Me.ProfitBase +
                     Me.m_search.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.m_search.Employ / Me.EmployBase +
                     Me.m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.m_search.ManValue / Me.ManValueBase +
                     Me.m_search.ValWeight(eSearchCriteriaResultTypes.Ecological) * Me.m_search.EcoValue / Me.EcoValueBase +
                     Me.m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity) * Me.m_search.DiversityIndex / Me.DiversityBase


            'Calculate boundary length/area ratio
            Me.AreaBoundary = Me.CalculateAreaOverBondaryLength()
            curSum = curSum + Me.AreaBoundary * Me.m_data.BoundaryWeight
            Me.m_data.objFuncTotal = (Me.m_search.WeightedTotal + Me.AreaBoundary * Me.m_data.BoundaryWeight) / Me.TotWeightedValueBase

            'calculate the relative values in to data structures 
            'so they can be use to populate the Input/Output object for the interface
            Me.m_data.objFuncEcologicalValue = CSng(Me.m_search.EcoValue / Me.EcoValueBase)
            Me.m_data.objFuncMandatedValue = CSng(Me.m_search.ManValue / Me.ManValueBase)
            Me.m_data.objFuncSocialValue = CSng(Me.m_search.Employ / Me.EmployBase)
            Me.m_data.objFuncEconomicValue = CSng(Me.m_search.Profit / Me.ProfitBase)
            Me.m_data.objFuncBiodiversity = CSng(Me.m_search.DiversityIndex / Me.DiversityBase)
            Me.m_data.objFuncAreaBorder = Me.AreaBoundary '/ AreaBoundBase

            If curSum > Me.TargetSumMax Then
                'save the best results 
                Me.TargetSumMax = CSng(curSum)

                Me.setRunState(cMPAOptManager.eRunStates.NewBestResultFound)

            End If

            'keep the results of every search
            Me.m_lstObjectiveResults.Add(New cObjectiveResult(Me.m_data, Me.m_SpaceData))

            ''Memory management for results
            'If Me.m_lstObjectiveResults.Count >= N_MAX_RESULTS Then
            '    'sorts in decending order (biggest objFuncTotal first)
            '    Me.m_lstObjectiveResults.Sort()
            '    'remove lowest results from the end of the list
            '    Me.m_lstObjectiveResults.RemoveRange(RESULTS_TO_KEEP - 1, Me.m_lstObjectiveResults.Count - RESULTS_TO_KEEP)
            'End If

            Return CSng(curSum)

        Catch ex As Exception
            m_logger.LogError(ex, "EvaluateRun")
            Debug.Assert(False, Me.ToString & ".EvaluateRun() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".EvaluateRun() Error:", ex)
        End Try

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the current run state of the MPA search.
    ''' </summary>
    ''' <returns>The current run state of the MPA search.</returns>
    ''' -----------------------------------------------------------------------
    Public Function RunState() As cMPAOptManager.eRunStates Implements IMPASearchModel.RunState
        Return Me.m_runstate
    End Function

    ''' <summary>
    ''' Public interfaced called by Ecospace at the start of each Year
    ''' </summary>
    ''' <param name="Biomass"></param>
    ''' <param name="iYear"></param>
    ''' <remarks>This is used by Ecoseed to control the length of the Ecospace run</remarks>
    Public Overridable Sub YearTimeStep(ByRef iYear As Integer, Biomass() As Single) Implements IMPASearchModel.YearTimeStep

        If Not Me.m_bRunning Then
            'Ecoseed is not running so don't do anything
            Exit Sub
        End If

        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        'JB 1-Oct-2014 Fixed
        'WARNING BUG
        'The idea here is to store the Ecospace state variables BCell() 
        'on the first run at the user supplied timestep EcoSpaceStartYear.
        'Then restore then on the first time step of subsequent runs

        'Right now it stores on the first run 
        'But does not restore on subsequent runs because
        'iYear has already been incremented to EcoSpaceStartYear + 1 when it get here
        'bitches...
        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


        'EwE5 see  KeepOrReloadCellValues(Biomass)
        If Me.nInterationCompleted = 0 And iYear = Me.m_data.EcoSpaceStartYear Then
            'First model interation and the year to start subsequent runs on 
            'Save the Ecospace start state so we can restore for later runs
            Me.StoreEcospaceState(Biomass)
        End If

        'iYear gets incremented at the start of the timestep 
        'so it will equal EcoSpaceStartYear + 1 on the first time step when this is called
        If Me.nInterationCompleted > 0 And (iYear - 1) = Me.m_data.EcoSpaceStartYear Then
            'We we are in the first timestep of an MPA optimization run 
            'Restore the Ecospace state to the start year saved from the first MPA Opt interation
            Me.RestoreEcospaceState(Biomass)
        End If

    End Sub


    Protected Overridable Sub fireOnIteration()

        Try
            If Me.m_cellComputedCallback IsNot Nothing Then
                Me.m_cellComputedCallback.Invoke()
            End If
        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try

    End Sub

    Protected Overridable Sub dumpSearchValues(search As cSearchDatastructures)

        System.Console.WriteLine("Total Value = " & search.Profit / Me.ProfitBase &
                                    ", Employ Value = " & search.Employ / Me.EmployBase &
                                    ", Mandated Value = " & search.ManValue / Me.ManValueBase &
                                    ", Eco Value = " & search.EcoValue / Me.EcoValueBase)
    End Sub


    Protected Sub StoreEcospaceState(biomass() As Single)
        Dim i As Integer, j As Integer, ip As Integer

        Debug.Assert(Me.nInterationCompleted = 0, "Opps you can only save the Ecospace state on the first optimization run.")

        'In the first interation keep the original bcell values
        For i = 1 To Me.m_SpaceData.InRow
            For j = 1 To Me.m_SpaceData.InCol
                For ip = 1 To Me.m_SpaceData.NGroups
                    Me.BOrig(i, j, ip) = Me.m_SpaceData.Bcell(i, j, ip)
                    Me.FOrig(i, j, ip) = Me.m_EcoSpace.FtimeCell(i, j, ip)
                    '   WOrig(i, j, ip) = m_esData.WchangeVar(i, j, ip)
                    Me.Blastseed(i, j, ip) = Me.m_SpaceData.Blast(i, j, ip)
                Next
            Next
        Next
        'Btime is needed when running Ecoseed
        For i = 1 To Me.m_SpaceData.NGroups
            Me.StoreBtimeForEcoSeed(i) = biomass(i)
        Next

    End Sub


    Protected Sub RestoreEcospaceState(biomass() As Single)
        Dim i As Integer, j As Integer, ip As Integer

        For i = 1 To Me.m_SpaceData.InRow
            For j = 1 To Me.m_SpaceData.InCol
                For ip = 1 To Me.m_SpaceData.NGroups

                    Me.m_SpaceData.Blast(i, j, ip) = Me.Blastseed(i, j, ip)
                    Me.m_SpaceData.Bcell(i, j, ip) = Me.BOrig(i, j, ip) 'Bseed(i, j, ip)
                    Me.m_EcoSpace.FtimeCell(i, j, ip) = Me.FOrig(i, j, ip)
                    ' WchangeVar(i, j, ip) = Wseed(i, j, ip)
                    '    LastT = m_esData.SumStart(0) - TimeStep
                Next
            Next
        Next
        For i = 1 To Me.m_SpaceData.NGroups
            biomass(i) = Me.StoreBtimeForEcoSeed(i)
        Next

    End Sub

    Protected Sub setRunState(RunState As cMPAOptManager.eRunStates)

        Try
            Me.m_runstate = RunState
            If (Me.m_StateCallback IsNot Nothing) Then
                Me.m_StateCallback.Invoke(RunState)
            End If
        Catch ex As Exception
            m_logger.LogError(ex, "setRunState")
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try

    End Sub

    Protected Function CellsNotMPA() As Boolean

        For i As Integer = 1 To Me.m_SpaceData.InRow
            For j As Integer = 1 To Me.m_SpaceData.InCol
                If Me.m_SpaceData.Depth(i, j) > 0 Then
                    For impa As Integer = 1 To Me.m_SpaceData.MPAno
                        If Me.m_SpaceData.MPA(impa)(i, j) > 0 Then
                            Return True
                        End If
                    Next impa
                End If
            Next j
        Next i

        Return False

    End Function


    Protected Function CalculateAreaOverBondaryLength() As Single
        Dim ir As Integer
        Dim ic As Integer
        Dim Area As Single
        Dim Border As Integer
        CalculateAreaOverBondaryLength = 0
        For ir = 1 To Me.m_SpaceData.InRow
            For ic = 1 To Me.m_SpaceData.InCol

                'is there any MPA in this cell
                If Me.IsMPA(ir, ic) Then
                    'Yep 
                    'Include this cell in the area
                    Area = Area + 1
                    'Border
                    'Include it in the Border area if not an MPA and modelled water cell
                    If Not Me.IsMPA(ir - 1, ic) And Me.m_SpaceData.Depth(ir - 1, ic) > 0 Then Border = Border + 1 'cell above is not mpa
                    If Not Me.IsMPA(ir + 1, ic) And Me.m_SpaceData.Depth(ir - 1, ic) > 0 Then Border = Border + 1 'cell below is not mpa
                    If Not Me.IsMPA(ir, ic - 1) And Me.m_SpaceData.Depth(ir, ic - 1) > 0 Then Border = Border + 1 'cell left is not mpa
                    If Not Me.IsMPA(ir, ic + 1) And Me.m_SpaceData.Depth(ir, ic + 1) > 0 Then Border = Border + 1 'cell right is not mpa
                End If
            Next
        Next
        If Border > 0 Then
            Return Area / Border
        Else
            'baserun no mpa, so return 1?
            Return 0.25
        End If
    End Function

    Protected Sub getBaseValues()

        Me.m_search.RedimForRun()

        'on the first call to ecospace ecoseed makes a copy of Biomass(), FTime()... See KeepOrReloadCellValues() at the user defined start time-step
        'then on subsequient calls it starts ecospace at the user defined start time-step and copies the values from the original call back to ecospace
        Me.m_nIters = 0
        'Get economic values for the base year BaseYearCost and BaseYearEffort
        Me.m_search.bBaseYearSet = False
        Me.m_EcoSpace.Run()

        If Me.m_data.StopRun Then Exit Sub

        'this will start ecospace at the user defined timestep and copy the saved state back into Ecospace at the user defined time step
        Me.m_nIters = 1
        Me.m_EcoSpace.Run()

        'values were set in the search object by EcoSpace.Run()
        Me.EmployBase = Me.m_search.Employ
        Me.ProfitBase = Me.m_search.Profit
        Me.ManValueBase = Me.m_search.ManValue
        Me.EcoValueBase = Me.m_search.EcoValue
        Me.DiversityBase = Me.m_search.DiversityIndex
        Me.AreaBoundBase = Me.CalculateAreaOverBondaryLength()

        If Me.ProfitBase = 0 Then Me.ProfitBase = 1
        If Me.ProfitBase < 0 Then Me.ProfitBase = -Me.ProfitBase
        If Me.EmployBase = 0 Then Me.EmployBase = 1
        If Me.EmployBase < 0 Then Me.EmployBase = -Me.EmployBase
        If Me.ManValueBase = 0 Then Me.ManValueBase = 1
        If Me.EcoValueBase = 0 Then Me.EcoValueBase = 1
        If Me.AreaBoundBase = 0 Then Me.AreaBoundBase = 1
        If Me.DiversityBase = 0 Then Me.DiversityBase = 1

        Me.TotWeightedValueBase = CSng(0 + Me.m_search.ValWeight(eSearchCriteriaResultTypes.Profit) * Me.ProfitBase +
                        Me.m_search.ValWeight(eSearchCriteriaResultTypes.Employment) * Me.EmployBase +
                        Me.m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb) * Me.ManValueBase +
                        Me.m_search.ValWeight(eSearchCriteriaResultTypes.Ecological) * Me.EcoValueBase +
                        Me.m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity) * Me.DiversityBase +
                        Me.m_data.BoundaryWeight * Me.AreaBoundBase)

    End Sub

#End Region

#Region "Saving Ouput CSV file and memory"

    ''' <summary>
    ''' Store the best row and col for this search interation
    ''' </summary>
    ''' <remarks>Right now this is writing the results file and memory</remarks>
    Protected Sub StoreObjectiveFunctionResults(writer As StreamWriter)

        Try

            'write the data to file
            Me.WriteOutputData(writer)

            'keep the results in memory
            '  m_lstObjectiveResults.Add(New cObjectiveResult(m_data))

        Catch ex As Exception
            Debug.Assert(False, "Ecoseed Error in StoreObjectiveFunctionResults(). " & ex.Message)
            m_logger.LogError(ex, "StoreObjectiveFunctionResults")
            'Just Blunder On????????????????????

        End Try

    End Sub

    ''' <summary>
    ''' Write header information to an output writer.
    ''' </summary>
    ''' <param name="writer">The writer to write to.</param>
    Protected Sub WriteOutputFileHeader(writer As StreamWriter)

        If (writer Is Nothing) Then Return

        'EwE5
        'Write #fnum, "row", "col", "econ", "social", "mandated", "ecosystem", "Area/Border"
        'Write #fnum, "", "", ValWeight(1), ValWeight(2), ValWeight(3), ValWeight(4), BoundaryWeight

        writer.WriteLine("MPA Optimization output")
        writer.WriteLine(Me.m_strHeader)
        writer.WriteLine("Objective weights for run")
        writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Boundary")
        writer.WriteLine()
        writer.WriteLine(String.Format("{0},{1},{2},{3},{4}",
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.Profit)),
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.Employment)),
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.MandateReb)),
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.Ecological)),
                cStringUtils.FormatNumber(Me.m_search.ValWeight(eSearchCriteriaResultTypes.BioDiversity)),
                cStringUtils.FormatNumber(Me.m_data.BoundaryWeight)))
        writer.WriteLine()
        writer.WriteLine("Base Values")
        writer.WriteLine("Economic, Social, Mandated, Ecosystem, Biomass diversity, Area/Boundary")
        writer.WriteLine(String.Format("{0},{1},{2},{3},{4},{5}",
                cStringUtils.FormatNumber(Me.ProfitBase),
                cStringUtils.FormatNumber(Me.EmployBase),
                cStringUtils.FormatNumber(Me.ManValueBase),
                cStringUtils.FormatNumber(Me.EcoValueBase),
                cStringUtils.FormatNumber(Me.DiversityBase),
                cStringUtils.FormatNumber(Me.AreaBoundBase)))
        writer.WriteLine()
        'writer.WriteLine("Data Format")
        'writer.WriteLine("Number of Rows and Columns")
        'writer.WriteLine("Row, Column, MPAIndex")
        'writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Border")

        Dim msg As New cMessage(cStringUtils.Localize(My.Resources.CoreMessages.MPAOPT_SAVE_SUCCESS, Path.Combine(Me.m_strOutputPath, Me.m_OutputFilename)),
                                eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        msg.Hyperlink = Me.m_strOutputPath
        Me.SendMessage(msg)

    End Sub

    ''' <summary>
    ''' Write the objective function values to file
    ''' </summary>
    ''' <param name="writer">The writer to write to.</param>
    Protected Sub WriteOutputData(writer As StreamWriter)

        If (writer Is Nothing) Then Return

        Try
            'EwE5
            'Write #fnum, bestrow, bestcol, ObjF(0), ObjF(1), ObjF(2), ObjF(3), ObjF(4)
            writer.WriteLine("Iteration," & Me.m_data.nIterations)
            writer.WriteLine("MPA cells," & cStringUtils.FormatNumber(Me.m_data.Cells.Count))
            For Each cell As cMPACell In Me.m_data.Cells
                writer.WriteLine("{0},{1},{2}", cell.Row, cell.Col, cell.iMPA)
            Next
            writer.WriteLine("Economic,Social,Mandated,Ecosystem,Biodiversity,Area/Border")
            writer.WriteLine(String.Format("{0},{1},{2},{3},{4}", _
                   cStringUtils.FormatNumber(Me.m_data.objFuncEconomicValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncSocialValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncMandatedValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncEcologicalValue), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncBiodiversity), _
                   cStringUtils.FormatNumber(Me.m_data.objFuncAreaBorder)))

        Catch ex As Exception
            m_logger.LogError(ex, "cMPARandomSearch::WriteOutputData")
        End Try

    End Sub

#End Region

#Region "Memory Managment"

    Protected Overridable Sub cleanUp()

        Erase Me.BOrig
        Erase Me.FOrig
        Erase Me.WOrig
        Erase Me.Blastseed

    End Sub

    Protected Overridable Sub RedimSeedVariables()
        Dim nvartot As Integer = Me.m_SpaceData.NGroups + 2

        ReDim Me.BOrig(Me.m_SpaceData.InRow + 1, Me.m_SpaceData.InCol + 1, nvartot)
        ReDim Me.FOrig(Me.m_SpaceData.InRow + 1, Me.m_SpaceData.InCol + 1, nvartot)
        ReDim Me.WOrig(Me.m_SpaceData.InRow + 1, Me.m_SpaceData.InCol + 1, nvartot)
        ReDim Me.Blastseed(Me.m_SpaceData.InRow + 1, Me.m_SpaceData.InCol + 1, nvartot)
        ReDim Me.StoreBtimeForEcoSeed(Me.m_SpaceData.NGroups)
        ' new
        ' ReDim IsMPA(m_SpaceData.InRow + 1, m_SpaceData.InCol + 1)

    End Sub


#End Region

#Region " Message handling "

    Protected Sub WriteError(ex As Exception)
        Try
            m_logger.LogError(ex, "WriteError MPA Optimization Error")
            System.Console.WriteLine(Me.ToString & " Error: " & ex.Message)
            System.Console.WriteLine("Stack trace " & ex.StackTrace)
        Catch newEx As Exception
            Debug.Assert(False, newEx.Message)
        End Try
    End Sub

    Protected Sub WriteError(message As String, ex As Exception)
        Try
            m_logger.LogError(ex, message)
            Me.WriteError(ex)
        Catch newEx As Exception
            Debug.Assert(False, newEx.Message)
        End Try
    End Sub

    Protected Sub WriteError(message As String)
        Dim msg As New cMessage(message, eMessageType.ErrorEncountered, eCoreComponentType.MPAOptimization, eMessageImportance.Critical)
        Me.SendMessage(msg)
    End Sub

    Protected Sub SendMessage(msg As cMessage)
        Try
            If (Me.m_SendMessageDelegate IsNot Nothing) Then Me.m_SendMessageDelegate.Invoke(msg)
        Catch ex As Exception
            m_logger.LogError(ex, "SendMessage")
            Debug.Assert(False, Me.ToString & ".setRunState() " & ex.Message)
        End Try
    End Sub

#End Region ' Message handling

End Class
