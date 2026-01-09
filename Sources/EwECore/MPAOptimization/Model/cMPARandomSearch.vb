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


Imports System.IO
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

' ToDo: significantly improve sampling performance:
'   - Pre-build an array of water cells that can be sampled
'   - Pre-build an array of water cells for each region (if region mode is enabled)
'   - Build an array for each iteration with candidate cells
'   - If region mode is on, filter the iteration-specific array with the closure state of regions
'   - Sampling only from cells in this array to prevent trying unnecessary cells
Public Class cMPARandomSearch
    Inherits cMPAOptBaseClass

#Region "Private data"

    Private LayerSumInMPA() As Single
    Private MaxLayerSumByLayerAndPctMPA(,) As Single

    '-- For smart resampling --
    ''' <summary>Water cell numbers for resampling</summary>
    Private m_watercells As New List(Of Integer)
    ''' <summary>Region # -> cell numbers for resampling</summary>
    Private m_regionCells() As List(Of Integer)
    ''' <summary>Number of cells in a region</summary>
    Private m_regionSize() As Integer
    ''' <summary>Number of cells set in a region</summary>
    Private m_regionSet() As Integer
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cMPARandomSearch)()

#End Region

#Region "Construction and Initialization"

    Public Sub New()
        MyBase.New()
        Me.m_OutputFilename = "MPAOpt_random_Output.csv"
    End Sub

#Region "Public Properties and Methods"

    Public Overrides ReadOnly Property OKtoRun() As Boolean
        Get
            If (Me.m_data.bUseRegions) Then

                ReDim Me.m_regionCells(Me.m_SpaceData.nRegions)
                ReDim Me.m_regionSize(Me.m_SpaceData.nRegions)
                ReDim Me.m_regionSet(Me.m_SpaceData.nRegions)

                For i As Integer = 0 To Me.m_SpaceData.nRegions
                    Me.m_regionCells(i) = New List(Of Integer)
                Next

            End If

            Me.m_watercells.Clear()

            Dim iR As Integer = Me.m_SpaceData.InRow
            Dim iC As Integer = Me.m_SpaceData.InCol

            Array.Clear(Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse), 0, Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse).Length)

            'We need number of potential MPA cells, this is watercells 
            '  - (cells which are either not part of an active MPA or which already are the same kind of MPA.)

            Dim ActiveMPA(Me.m_SpaceData.MPAno) As Boolean
            For i As Integer = 1 To Me.m_SpaceData.MPAno : ActiveMPA(i) = Me.m_SpaceData.IsMPAActive(i) : Next

            'The logic below presumes that MPAs and water cells do not change during the search. Which should be ok.
            For irow As Integer = 1 To iR
                For icol As Integer = 1 To iC
                    If Me.m_SpaceData.Depth(irow, icol) > 0 Then
                        Dim iThisCell As Integer = Me.RowColToCell(irow, icol)
                        Me.m_watercells.Add(iThisCell)
                        If (Me.m_data.bUseRegions) Then
                            Dim reg As Integer = Me.m_SpaceData.Region(irow, icol)
                            Me.m_regionSize(reg) += 1
                            Me.m_regionCells(reg).Add(iThisCell)
                        End If
                    End If
                Next
            Next

            ' Assess current protection
            Dim IsProtected(iR, iC) As Boolean
            Dim nProtected As Integer = 0
            For iMPA As Integer = 1 To Me.m_SpaceData.MPAno
                Dim bIsClosed As Boolean
                Dim bIsApplied As Boolean
                For i As Integer = 1 To cCore.N_MONTHS : bIsClosed = bIsClosed Or (Me.m_SpaceData.MPAmonth(i, iMPA) = False) : Next
                For i As Integer = 1 To Me.m_SpaceData.nFleets : bIsApplied = bIsApplied Or (Me.m_SpaceData.MPAfishery(i, iMPA) = False) : Next
                If (bIsClosed And bIsApplied And iMPA <> Me.m_data.iMPAtoUse) Then
                    For irow As Integer = 1 To iR
                        For icol As Integer = 1 To iC
                            If ((Me.m_SpaceData.Depth(irow, icol) > 0) And (Me.m_SpaceData.MPA(iMPA)(irow, icol) > 0)) Then
                                If (IsProtected(irow, icol) = False) Then
                                    IsProtected(irow, icol) = True
                                    nProtected += 1
                                End If
                            End If
                        Next
                    Next
                End If
            Next

            Me.CellCount = Me.m_watercells.Count

            If (Me.CellCount = 0) Then
                m_logger.LogError("Cannot start MPA Opt without modelled cells")
                Return False
            End If

            'If ((100 * nProtected / Me.CellCount) > Me.m_data.MaxArea) Then
            '    ' Reject a run if there are no cells left to close, e.g., if the max percentage protection
            '    Me.WriteError(cStringUtils.Localize("The area is already protected by {0}%. MPA optimization will not run", (100 * nProtected / Me.CellCount)))
            '    Return False
            'End If

            If (Me.m_data.bUseRegions) Then
                For i As Integer = 1 To Me.m_SpaceData.nRegions
                    If (Me.m_regionSize(i) < 5) Then
                        Me.WriteError(My.Resources.CoreMessages.MPARND_PRERUN_CHECK_REGIONSIZE)
                        Return False
                    End If
                Next
            End If

            Return True
        End Get
    End Property

#End Region

    Private Function RowColToCell(r As Integer, c As Integer) As Integer
        Return (r - 1) * Me.m_SpaceData.InCol + c
    End Function

    Private Sub CellToRowCol(i As Integer, ByRef r As Integer, ByRef c As Integer)
        r = (i - 1) \ Me.m_SpaceData.InCol + 1
        c = (i - 1) Mod Me.m_SpaceData.InCol + 1
    End Sub

    Private Sub initForRun()

        Try

            'Ecoseed does not listen to the Ecospace time steps
            Me.m_EcoSpace.TimeStepDelegate = Nothing

            'create a new list to store the results
            Me.m_lstObjectiveResults = New List(Of cObjectiveResult)
            Me.TargetSumMax = 0

            'Clear out any values from a previous ecoseed run
            Me.m_data.Clear()

            Me.RedimSeedVariables()

        Catch ex As Exception
            Me.WriteError(ex)
            Throw New ApplicationException(Me.ToString & ".initForRun() Error: " & ex.Message, ex)
        End Try

    End Sub


#End Region

#Region "Running"

    Overrides Sub Run() 'Implements IMPASearchModel.Run

        Try

            Me.m_bRunning = True
            Me.setRunState(cMPAOptManager.eRunStates.Initializing)

            Me.m_data.StopRun = False

            Me.runSearch()

        Catch ex As Exception
            Me.WriteError("MPA Optimizatoin Random Search Error")
            Debug.Assert(False, ex.StackTrace)
        End Try

        Me.m_bRunning = False
        Me.setRunState(cMPAOptManager.eRunStates.Completed)

    End Sub


    Private Sub runSearch()

        Debug.Assert(Me.m_data.iMPAtoUse > 0, "Current MPA not set!!!.")

        'VC changes
        'Main loop for running the Random MPA optimization

        Dim StoreOptimalPct As Single = 1 'from GUI
        Dim MinimalEvaluationValue As Single = 0
        Dim writer As StreamWriter = Nothing

        If Me.m_bAutosaveResults Then
            If cFileUtils.IsDirectoryAvailable(Me.m_strOutputPath, True) Then
                Try
                    writer = New StreamWriter(Path.Combine(Me.m_strOutputPath, Me.m_OutputFilename))
                Catch ex As Exception

                End Try
            End If
        End If

        Try
            Debug.Assert(Me.m_data IsNot Nothing, "Ecoseed: data not initialized")
            Debug.Assert(Me.m_EcoSpace IsNot Nothing, "Ecoseed: Ecospace not initialized")
#If DEBUG Then
            System.Console.WriteLine("-----------MPA Random Search --------------")
#End If

            Me.initForRun()

            ' make snapshot of MPA cell occupation for quick lookup during computations
            Me.InitIsMPA()

            Me.m_search.SearchMode = eSearchModes.SpatialOpt
            Me.m_search.SetMinSearchBlocks()
            Me.getBaseValues()

            Me.WriteOutputFileHeader(writer)

            Me.CalculateCellWeightings()

            Me.CellCount = Me.m_watercells.Count

            'Get the layer weights by percentage MPA coverage
            Me.sortLayersByCellWeight(Me.CellCount)

            'Step from Min area(%) (= integer) to Max area(%) (= integer) stepsize = Step (%) (=integer)
            Dim iStep As Integer = CInt((Me.m_data.MaxArea - Me.m_data.MinArea) / Me.m_data.stepSize)
            Dim nStep As Integer = 0

            Me.setRunState(cMPAOptManager.eRunStates.Searching)

            Me.m_nIters = 0

            For iPropMPA As Integer = Me.m_data.MinArea To Me.m_data.MaxArea Step Me.m_data.stepSize
                'keep track of how many times we've stepped: 
                'calculate how many cells that should be closed:
                'this is calculated based on number of water cells - number of other mpsa cells, not total number of cells:
                Dim NumberMPA As Integer = CInt(iPropMPA * Me.CellCount / 100)

                'Step through and do iterations:
                For m_iIter As Integer = 1 To Me.m_data.nIterations
                    'select the MPA cells that are to be evaluated in this run
                    Me.selectRandomCells(NumberMPA)

                    Me.fireOnIteration()

                    'Run EcoSpace
                    Me.m_EcoSpace.Run()
                    If Me.m_data.StopRun Then Exit For

                    'Evaluate the current MPA cell selection
                    Me.EvaluateRun()

                    'Store LayerSumInMPA
                    Me.calcImportanceLayersCoverageInRun()

                    ' Process results
                    Me.StoreObjectiveFunctionResults(writer)
                    ' Next
                    Me.m_nIters += 1

                Next
                If Me.m_data.StopRun Then Exit For
                nStep += 1
            Next

            Me.cleanUp()

        Catch ex As Exception
            Me.WriteError(ex)
            Me.m_bRunning = False
            Debug.Assert(False, ex.StackTrace)
        End Try

        If (writer IsNot Nothing) Then
            writer.Flush()
            writer.Close()
            writer.Dispose()
        End If

    End Sub

    Private m_generator As New Random()

    Private Sub selectRandomCells(NumberMPA As Integer)

        Dim cells As New List(Of Integer)
        Dim used As New HashSet(Of Integer)

        Try

            'clear out the last set of cells
            Me.m_data.ClearCells()

            ' Clear data cells with the currently selected MPA
            Array.Clear(Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse), 0, Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse).Length)

            If (Me.m_data.bUseRegions) Then
                For i As Integer = 1 To Me.m_SpaceData.nRegions - 1
                    Me.m_regionSet(i) = 0
                Next
            End If

            'Now start selecting the ones to make MPAs
            Dim GetOut As Integer = 0
            Dim bDone As Boolean = False

            Do While (bDone = False) And (GetOut < 100 * NumberMPA)

                Dim iThisCell As Integer = 0

                'JS 26Oct19: determine which cells to use
                If (GetOut = 0) Then
                    cells.Clear()
                    If Me.m_data.bUseRegions Then
                        Dim iNumSaturated As Integer = 0
                        Dim iNumAvailable As Integer = 0
                        For reg As Integer = 1 To Me.m_SpaceData.nRegions
                            If (Me.m_regionSize(reg) > 0) Then
                                iNumAvailable += 1
                                Dim propclosed As Double = Me.m_regionSet(reg) / Me.m_regionSize(reg)
                                If ((propclosed * 100) > Me.m_data.MaxArea) Then
                                    iNumSaturated += 1
                                Else
                                    cells.AddRange(Me.m_regionCells(reg))
                                End If
                            End If
                        Next

                        If iNumSaturated = iNumAvailable Then
                            cells.Clear()
                            cells.AddRange(Me.m_watercells)
                        Else
                            cells.Sort()
                        End If
                    Else
                        cells.AddRange(Me.m_watercells)
                    End If
                End If

                'VC 2019-11-07  there was a bug in the selection here below
                'the cells are arranged in increasing order (col-row) so once it reaches the
                'random number it should pick that cell. The bug was that the test for cumulativecellweight>= ranval
                'and the not used.contains were in the same line (and), so when reaching a cell that had already be taken
                'it would just continue to the next cell in the row and pick that one

                'For the region selection, the CumulativeCellWeight should be built by region, so need to do it again
                'VC2019/11/07 I've implemented this, so now cumulativecellweight is calculated here instead
                Dim sum As Double = 0
                Dim ix As Integer
                For jx As Integer = 0 To cells.Count - 1
                    ix = cells(jx)
                    sum += Me.CellWgt(ix)
                    Me.CumulativeCellWeight(ix) = sum
                Next


                Do While iThisCell = 0
                    Dim RanVal As Double = Me.m_generator.NextDouble()
                    For j As Integer = 0 To cells.Count - 1
                        Dim i As Integer = cells(j)
                        If Me.CumulativeCellWeight(i) >= RanVal Then
                            'use this one, if not taken already
                            If Not used.Contains(i) Then iThisCell = i
                            Exit For
                        End If
                    Next
                Loop

                If (iThisCell > 0) Then
                    Dim GetRow As Integer = (iThisCell - 1) \ Me.m_SpaceData.InCol + 1
                    Dim GetCol As Integer = (iThisCell - 1) Mod Me.m_SpaceData.InCol + 1

                    'now we know which cell to close
                    'check that the cell hasn't been made into an mpa already
                    Debug.Assert(Me.m_SpaceData.Depth(GetRow, GetCol) > 0)

                    If (Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse)(GetRow, GetCol) <= 0) Then

                        Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse)(GetRow, GetCol) = 1
                        Me.m_data.AddCell(GetRow, GetCol, Me.m_data.iMPAtoUse)
                        Me.IsMPA(GetRow, GetCol) = True

                        If (Me.m_data.bUseRegions) Then
                            Dim reg As Integer = Me.m_SpaceData.Region(GetRow, GetCol)
                            Me.m_regionSet(reg) += 1
                        End If
                        used.Add(iThisCell)
                        GetOut = 0
                        bDone = (used.Count >= NumberMPA)
                    Else
                        GetOut += 1
                    End If
                Else
                    GetOut += 1
                End If
            Loop

        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, Me.ToString & ".selectRandomCells() Error: " & ex.Message)
            Throw New ApplicationException(Me.ToString & ".selectRandomCells() Error:", ex)
        End Try

        If (Me.m_data.bUseRegions) Then
#If DEBUG Then
            Console.WriteLine("cMPARandomSearch region random cell assessment:")
            For i As Integer = 1 To Me.m_SpaceData.nRegions
                If (Me.m_regionSize(i) > 0) Then
                    Dim propclosed As Double = Me.m_regionSet(i) / Me.m_regionSize(i)
                    Console.WriteLine(" Region {0:D3}: {1} cells, {2} closed, {3}%", i, Me.m_regionSize(i), Me.m_regionSet(i), propclosed)
                End If
            Next
#End If
        End If

    End Sub

    Protected Sub CalculateCellWeightings()
        Dim iC As Integer       'used to count the cells

        Try

            Dim inRow As Integer = Me.m_SpaceData.InRow
            Dim inCol As Integer = Me.m_SpaceData.InCol
            Dim NoCells As Integer = inRow * inCol

            ReDim Me.CumulativeCellWeight(NoCells)
            ReDim Me.CellWgt(NoCells)
            Dim CellWeight(inRow, inCol) As Double

            'If on the GUI the "Group weighting" is checked then calculate cellweight, otherwise, set to 1
            'use guidance function
            'cell contribution to objectivity function at the ecopath base case 
            '1. equal prob
            '2. biomass or habitat proportional
            '3. inverse objectivity function 
            'evt 4 mcmc search, start with a given number of closed cells, replace a cell (based on probability), evaluate, 

            'develop a measure including
            '1. spatial cost of fishing (distance from port): this becomes and "importance" layer, we can just cut and paste it in
            '2. depth factor (deeper  = more costly): this also becomes an importance layer
            '3. Any "importance" layer, i.e. Jeroen, we need to be able to store "importance" layers, which for now can be cut and pasted into ecospace. 
            '   The "importance" layers will need to have a title and description, plus a value for each cell. 
            '4. How much does the cell contribute to fishing pressure for the cells to be protected


            'Scan through the spreadsheet with the importance layers, and set up the likelihood function.

            'If Me.m_data.bUseCellWeight Then
            '    ''Get the ecosystem structure weightings from the GUI (needs to be added)
            '    ''for now hard-coded to 1
            '    'Dim GroupWeight(m_SpaceData.NGroups) As Single
            '    'For ip As Integer = 1 To m_SpaceData.NGroups
            '    '    GroupWeight(ip) = 1
            '    'Next

            '    For i As Integer = 1 To inRow
            '        For j As Integer = 1 To inCol
            '            For ip As Integer = 1 To m_SpaceData.NGroups
            '                '    CellWeight(i, j) += GroupWeight(ip) * BOrig(i, j, ip)
            '                CellWeight(i, j) += Me.m_search.BGoalValue(ip) * BOrig(i, j, ip)
            '            Next
            '        Next
            '    Next
            'Else
            'iC = 0

            Dim data()(,) As Single = Me.m_SpaceData.ImportanceLayerMap
            Dim weight As Double
            Dim LayerSum(Me.m_SpaceData.nImportanceLayers) As Double

            'VC2008Nov11, scaling each of the importance layers to have average 1
            For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
                'weight = Me.m_SpaceData.ImportanceLayers(iL).sWeight
                Dim Count As Integer = 0
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        If data(iL)(i, j) > 0 Then
                            Count += 1
                            LayerSum(iL) += data(iL)(i, j)
                        End If
                    Next j
                Next i
                'This will make the average for each layer 1, but then a layer that only has values 
                'in a few cells will count much less, than one with values in many cells
                'If Count > 0 Then AverageLayer(iL) /= Count
                'So instead making the layers SUM to 1
                If LayerSum(iL) = 0 Then LayerSum(iL) = 1 'just to avoid division with 0, if a layer is empty
            Next iL

            Dim minCellWeight As Double = Double.MaxValue
            For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
                weight = Me.m_SpaceData.ImportanceLayerWeight(iL)
                For i As Integer = 1 To inRow
                    For j As Integer = 1 To inCol
                        CellWeight(i, j) += weight * data(iL)(i, j) / LayerSum(iL)
                        If CellWeight(i, j) < minCellWeight And CellWeight(i, j) > 0 Then minCellWeight = CellWeight(i, j)
                    Next j
                Next i
            Next iL

            'now make sure all cells can be selected:
            For i As Integer = 1 To inRow
                For j As Integer = 1 To inCol
                    If CellWeight(i, j) = 0 Then 'give it a value
                        CellWeight(i, j) = 0.01 * minCellWeight
                    End If
                Next j
            Next i

            'VC2019/11/07 I've moved the calculatoin of the cumulative cell weight to where it's used
            'as it needs to be calculated by region
            'Now calculate cumulative weighted importance over all cells:
            iC = 0
            Dim Sum As Double = 0
            For i As Integer = 1 To inRow
                For j As Integer = 1 To inCol
                    iC += 1
                    If CellWeight(i, j) < 0 Then CellWeight(i, j) = 0
                    Sum += CellWeight(i, j)
                    'CumulativeCellWeight(iC) = Sum
                    Me.CellWgt(iC) = CellWeight(i, j)
                Next
            Next

            'Finally scale the cellweights so that they sum to 1
            If Sum > 0 Then
                For i As Integer = 1 To NoCells
                    'CumulativeCellWeight(i) /= Sum
                    Me.CellWgt(i) /= Sum
                Next
            Else
                'if there are no values in any of the importance layer
                'set CumulativeCellWeight() to an even gradient so that the cell selection will not be weighted
                Dim g As Single = CSng(1 / NoCells)
                For i As Integer = 1 To NoCells
                    'CumulativeCellWeight(i) += g * i
                    Me.CellWgt(i) += g * i
                Next
            End If

        Catch ex As Exception
            Me.WriteError(ex)
            Debug.Assert(False, ex.StackTrace)
            Throw New ApplicationException(Me.ToString & ".CalculateCellWeightings() " & ex.Message, ex)
        End Try

    End Sub

    Protected Sub sortLayersByCellWeight(CellCount As Integer)
        Dim NoCells As Integer = Me.m_SpaceData.InRow * Me.m_SpaceData.InCol
        ReDim Me.MaxLayerSumByLayerAndPctMPA(Me.m_SpaceData.nImportanceLayers, 100)

        For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
            Dim Cnt As Integer = 0
            Dim ArrayVal(NoCells) As Single

            For i As Integer = 1 To Me.m_SpaceData.InRow
                For j As Integer = 1 To Me.m_SpaceData.InCol
                    Cnt = Cnt + 1
                    'Make a copy of the data
                    ArrayVal(Cnt) = Me.m_SpaceData.ImportanceLayerMap(iL)(i, j)
                Next j
            Next i
            'now we have all the layer values in ArrayVal, so sort them:
            System.Array.Sort(ArrayVal)
            System.Array.Reverse(ArrayVal)
            'We can now store the layerweight for each percentage coverage:
            For iMPA As Integer = 1 To 100
                'we want to store this for 100 levels (%) of protection
                For iC As Integer = 0 To CInt(CellCount * iMPA / 100) - 1
                    Me.MaxLayerSumByLayerAndPctMPA(iL, iMPA) += ArrayVal(iC)
                Next
            Next
        Next iL
    End Sub

    Protected Sub calcImportanceLayersCoverageInRun()
        Dim Data()(,) As Single = Me.m_SpaceData.ImportanceLayerMap
        ReDim Me.LayerSumInMPA(Me.m_SpaceData.nImportanceLayers)

        For iL As Integer = 1 To Me.m_SpaceData.nImportanceLayers
            For iR As Integer = 1 To Me.m_SpaceData.InRow
                For iC As Integer = 1 To Me.m_SpaceData.InCol
                    If Me.m_SpaceData.MPA(Me.m_data.iMPAtoUse)(iR, iC) > 0 Then 'this is a protected cell, so check what 
                        Me.LayerSumInMPA(iL) += Data(iL)(iR, iC)
                    End If
                Next iC
            Next iR
        Next iL
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Convert 'iAreaPercentToClose' cells in the map to MPA 'iMPA'
    ''' </summary>
    ''' <param name="hitcountmap">The best count map to convert.</param>
    ''' <param name="iAreaPercentToClose">Percent of water cells 
    ''' to close in addition to the current MPAs.</param>
    ''' <param name="iMPA">The MPA to assign new cells to.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>
    ''' Cells are selected from the best count map, aiMap, by descending
    ''' value until either the requested percentage is met or there are no 
    ''' convertable cells left.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Function ConvertToMPA(hitcountmap As Integer(,),
                                 iAreaPercentToClose As Integer,
                                 iMPA As Integer) As Boolean

        ' Dictionary with list of points, sorted by hit count
        Dim dtMapSorted As New Dictionary(Of Integer, List(Of Integer))
        ' List of hit count values, keys to the dictionary
        Dim lKeys As New List(Of Integer)
        ' Helper var to reference lists in the dictionary
        Dim lPoints As List(Of Integer) = Nothing
        ' Number of water cells
        Dim iNumWaterCells As Integer = 0
        ' Number of cells to close
        Dim iNumCellsToClose As Integer = 0
        ' Row, col iterators
        Dim iRow, iCol As Integer
        ' Always handy
        Dim iIndex As Integer = 0

        Dim cR As Integer = Me.m_SpaceData.InRow
        Dim cC As Integer = Me.m_SpaceData.InCol

        Dim iFrom As Integer = 1
        Dim iTo As Integer = If(Me.m_data.bUseRegions, Me.m_SpaceData.nRegions, 1)

        For iIter As Integer = iFrom To iTo

            dtMapSorted.Clear()
            lKeys.Clear()
            iNumWaterCells = 0

            ' Gather conversion info
            For iRow = 1 To cR
                For iCol = 1 To cC

                    Dim bUseCell As Boolean = (Me.m_SpaceData.Depth(iRow, iCol) > 0)
                    If (Me.m_data.bUseRegions) Then bUseCell = bUseCell And Me.m_SpaceData.Region(iRow, iCol) = iIter

                    If (bUseCell) Then

                        ' Clear existing target MPA cells
                        Me.m_SpaceData.MPA(iMPA)(iRow, iCol) = 0
                        ' Get hit count value for this cell
                        iIndex = hitcountmap(iRow, iCol)

                        ' Add it to the dictionary
                        If Not dtMapSorted.ContainsKey(iIndex) Then
                            ' #Yes: create point list and add it to dictionary
                            lPoints = New List(Of Integer)
                            dtMapSorted(iIndex) = lPoints
                            lKeys.Add(iIndex)
                        Else
                            ' #No: get point list
                            lPoints = dtMapSorted(iIndex)
                        End If
                        ' Add point as candidate cell
                        lPoints.Add(Me.RowColToCell(iRow, iCol))

                        ' Count water cells
                        iNumWaterCells += 1

                    End If ' Is water cell
                Next iCol
            Next iRow

            ' Need to bail out?
            If (lKeys.Count = 0) Then Return True

            ' Calculate #cells to close
            iNumCellsToClose = CInt(Math.Ceiling(iNumWaterCells * iAreaPercentToClose / 100))

            ' Sort keys in reverse order (highest hit count value first)
            lKeys.Sort()
            lKeys.Reverse()

            ' VC, JS 14nov08: Instead of randomizing cells when hit counts are identical,
            '                 cells could be selected based on total weighted score
            For Each lPoints In dtMapSorted.Values
                Me.Shuffle(lPoints)
            Next

            ' Get first cell list to iterate over
            lPoints = dtMapSorted(lKeys(0))
            lKeys.RemoveAt(0)
            iIndex = 0

            ' Can we go home now?
            While (iNumCellsToClose > 0)
                ' Next point list, if applicable
                ' Bug fix: point list are now allowed to be empty
                While (lPoints.Count = 0)
                    lPoints = dtMapSorted(lKeys(0))
                    lKeys.RemoveAt(0)
                End While

                iIndex = lPoints(0)
                Me.CellToRowCol(iIndex, iRow, iCol)
                Me.m_SpaceData.MPA(iMPA)(iRow, iCol) = 1
                lPoints.RemoveAt(0)

                ' One less to close
                iNumCellsToClose -= 1
            End While
        Next

        Return True

    End Function

    Private Sub Shuffle(pts As List(Of Integer))
        Dim n As Integer = pts.Count - 1
        For i As Integer = 0 To pts.Count - 1
            Dim t As Integer = pts(i)
            pts.RemoveAt(i)
            pts.Insert(CInt(Me.m_generator.NextDouble * n), t)
        Next
    End Sub

#End Region

End Class
