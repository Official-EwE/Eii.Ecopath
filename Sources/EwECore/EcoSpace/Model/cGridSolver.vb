' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Threading
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

Public Class cGridSolver

#Region "Public data"

    ''' <summary>
    ''' Signal mechanism used by the calling thread for thread Synchronization
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state WaitHandle.Reset()) 
    ''' calls to WaitHandle.WaitOne() will block until Solve() has completed (SignalState in a signaled state WaitHandle.Set())
    ''' </remarks>
    Public WaitHandle As New ManualResetEvent(True)

    Public Shared ThreadIncrementer As Integer

    Public iterThread As Integer 'total iterations 

    Public CPUTime As Single


#End Region

#Region "Private data"

    Public ThreadID As Integer

    'arguments from original code
    Private X(,,) As Single
    Private Aloc(,,) As Single
    Private Floc(,,) As Single
    Private jord() As Integer
    Private W As Single
    Private Bcw(,,) As Single
    Private C(,,) As Single
    Private d(,,) As Single
    Private e(,,) As Single
    Private M As Integer
    Private NomCols As Integer
    Private Tol As Single
    Private Depth(,) As Single

    Private iFrstGrp As Integer
    Private iLastGrp As Integer

    Private ByPassIntegrate() As Boolean

    Private iStartRow() As Integer
    Private iEndRow() As Integer
    Private jStartCol() As Integer
    Private jEndCol() As Integer

    Private timeStep As Single

    Private maxIter As Integer

    Private alternateRowCol As Boolean = False

    Private isMigratory() As Boolean

    Public threadTime As Double

    Private threadGroups(,) As Integer

    Private useExact As Boolean

    Private m_rand As Random

    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
    'Local copies of the core arrays used by SolveGridLocalMemoryOnly()
    'this is for Hungabee and are only used if bUseLocalMemory = True
    Public bUseLocalMemory As Boolean

    Private Xloc(,) As Single
    Private AlocLoc(,) As Single
    Private FlocLoc(,) As Single
    Private bloc(,) As Single
    Private cloc(,) As Single
    Private dloc(,) As Single
    Private eloc(,) As Single

    Private DepthLoc(,) As Single

    Private iStartRowLoc() As Integer
    Private iEndRowLoc() As Integer
    Private jStartColLoc() As Integer
    Private jEndColLoc() As Integer
    Private jordLoc() As Integer
    'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    Private m_stpwCopy As Stopwatch
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cGridSolver)()


#End Region

#Region "Constructor and Initialization"

    Public Sub New(ThreadNumber As Integer)
        'isOkToRunning = True
        Me.ThreadID = ThreadNumber
        Me.m_rand = New Random
    End Sub

    ''' <summary>
    ''' Set all references to data used for calculation
    ''' </summary>
    ''' <remarks>This needs a bunch more data</remarks>
    ''' SolveGrid(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
    ''' SolveGridRow(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
    Public Function Init(ByRef AMm(,,) As Single, ByRef F(,,) As Single, ByRef BCell(,,) As Single, ByRef Inrow As Integer, ByRef InCol As Integer, ByRef Tol1 As Single, ByRef jord1() As Integer, ByRef W1 As Single, ByRef Bcw1(,,) As Single, ByRef C1(,,) As Single, ByRef d1(,,) As Single, ByRef e1(,,) As Single, ByRef Depth1(,) As Single, BPIntegrate() As Boolean, ByRef iStartRow1() As Integer, ByRef iEndRow1() As Integer, timeStep1 As Single, maxIter1 As Integer, ByRef jStartCol1() As Integer, ByRef jEndCol1() As Integer, ByRef isMigratory1() As Boolean, ByRef threadGroups1(,) As Integer, bUseExact As Boolean) As Boolean
        Me.Aloc = AMm
        Me.Floc = F
        Me.X = BCell
        Me.jord = jord1
        Me.W = W1
        Me.Bcw = Bcw1
        Me.C = C1
        Me.d = d1
        Me.e = e1
        Me.M = Inrow
        Me.NomCols = InCol
        Me.Tol = Tol1
        Me.Depth = Depth1
        Me.ByPassIntegrate = BPIntegrate
        Me.iStartRow = iStartRow1
        Me.iEndRow = iEndRow1
        Me.timeStep = timeStep1
        Me.maxIter = maxIter1
        Me.jStartCol = jStartCol1
        Me.jEndCol = jEndCol1
        Me.isMigratory = isMigratory1
        Me.threadGroups = threadGroups1
        Me.useExact = bUseExact
    End Function

#End Region

#Region "Group Counters"

    Public ReadOnly Property iFirstIndex() As Integer
        Get
            Return Me.iFrstGrp
        End Get
    End Property

    Public ReadOnly Property nGroupsComputed As Integer
        Get
            Return Me.iLastGrp - Me.iFirstIndex + 1
        End Get
    End Property


    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstGroup"></param>
    ''' <param name="iLastGroup"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastGroups(iFirstGroup As Integer, iLastGroup As Integer)
        Me.iFrstGrp = iFirstGroup
        Me.iLastGrp = iLastGroup
    End Sub

#End Region

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Solve(obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)
        ' Dim timeTemp As Double = Timer
        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        'isOkToRunning = False
        Me.iterThread = 0
        Dim iGrp As Integer
        Dim i As Integer
        Me.CPUTime = 0
        Dim stpw As Stopwatch = Stopwatch.StartNew
        Me.m_stpwCopy = New Stopwatch
        Try

            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            Me.WaitHandle.Reset()
            Me.alternateRowCol = True

            Me.CopyStartEndRowCol()

            'do the processing here
            'System.Console.WriteLine("Solve() " & iFrstGrp.ToString & "," & iLastGrp.ToString)
            For i = Me.iFrstGrp To Me.iLastGrp
                iGrp = Me.threadGroups(Me.ThreadID, i)

                If Me.ByPassIntegrate(iGrp) = False Then
                    'If useExact And isMigratory(iGrp) Then
                    If Me.useExact Then
                        Me.solveExact(iGrp)
                    Else
                        If Me.bUseLocalMemory Then
                            'Copies core memory to local arrays for running on Hungabee
                            'otherwise the same as SolveGrid_SharedMemory
                            Me.SolveGrid_LocalMemory(iGrp)
                        Else
                            'Uses core map arrays directly
                            Me.SolveGrid_SharedMemory(iGrp)
                        End If

                        If Not Me.alternateRowCol Then
                            Me.SolveGridRow(iGrp)
                        End If
                    End If
                End If

            Next i

        Catch ex As Exception
            m_logger.LogError(ex, "cGridSolver.Solve() ThreadID={ThreadID}", Me.ThreadID)
            Debug.Assert(False, ex.Message)
        End Try

        'set signal state to 'signaled' 
        'the processing has finished SignalState.WaitOne() will return immediately
        If Interlocked.Decrement(cGridSolver.ThreadIncrementer) = 0 Then
            Me.WaitHandle.Set()
        End If

        If Me.bUseLocalMemory Then
            '  System.Console.WriteLine("Grid copy CPU time (sec), " + Me.m_stpwCopy.Elapsed.TotalSeconds.ToString)
        End If

        Me.CPUTime = CSng(stpw.Elapsed.TotalSeconds)

    End Sub

    Private Sub solveExact(ip As Integer)
        'this solves the linearized implicit biomass flux equations directly rather that iteratively
        'this is takes longer than a few iterations in solvegrid, but shorter than a lot
        'if the species requires a lot of iterations (e.g. migratory), this should be better than s.g.
        'bandec and bandks are taken from numerical recipes (press et al.), translated to vb
        Dim a(,) As Double 'reduced band form of Bcell
        Dim al(,) As Double 'reduced band form of L (from LU decomp)
        Dim de As Single
        Dim b() As Single 'input vector b (=-F)
        Dim indx() As Integer 'index table for row subsitutions

        ReDim al(Me.NomCols * Me.M + 1, 2 * Me.NomCols + 1)
        ReDim b(Me.M * Me.NomCols)
        ReDim indx(Me.M * Me.NomCols)

        'get b vector from F
        Dim k As Integer = 0
        For i As Integer = 1 To Me.M
            For j As Integer = 1 To Me.NomCols
                k += 1
                b(k) = -Me.Floc(i, j, ip)
            Next
        Next

        Try
            'for the eq. Ax=b
            'get the reduced band form of a from the fluxes (see press et al. 2-4)
            a = Me.arrangeMatrix(Me.Aloc, Me.Bcw, Me.C, Me.d, Me.e, ip, Me.M, Me.NomCols)

            'get the LU decomposition of a (now a=L and al=L)
            Me.bandec(a, Me.M * Me.NomCols, Me.NomCols, al, indx, de)

            'back substitution to get x (now stored in b)
            Me.bandks(a, Me.M * Me.NomCols, Me.NomCols, al, indx, b)

            'refill the Bcell array with the values in the vector x
            Me.refillX(Me.X, b, ip, Me.M, Me.NomCols)

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub


#End Region

#Region "Grid solving computational code"

    ''' <summary>
    ''' Solve the grid diffusion matrix on directly on core memory.
    ''' </summary>
    ''' <param name="ip">Index of group</param>
    ''' <remarks>
    ''' Run if <see cref="bUseLocalMemory"></see> = False. 
    ''' This will run faster on small to medium sized models.
    ''' </remarks>
    Private Sub SolveGrid_SharedMemory(ip As Integer)
        'this routine solves for equilibrium field of concentrations x over a grid
        ' x(i,j) is equilibrium concentration of x in grid cell i,j
        'am(i,j) is total loss rate of x from cell i,j...NB:am(i,j)<0 !!!!!!
        'b(i,j) is loss rate from element i-1 to i in column j of grid
        'c(i,j) is loss rate from element i+1 to i in column j of grid
        'd(i,j) is loss rate from element j to element j+1 in row i of grid
        'e(i,j) is loss rate from element j to element j-1 in row i of grid
        'f(i,j) is forcing input to element i,j from sources outside the grid
        'm is number of rows (i) in grid
        'NomCols is number of columns (j) in grid
        'tol is tolerance limit for change in iterative solution
        'jord(k) is which column j to do as k=1, k=2,...,k=n (iteration order)
        'w is SOR overrelaxation parameter-found 1.25 to be good for typical problems
        Dim iter As Integer, j As Integer, i As Integer, jj As Integer, ic As Integer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(Me.M + 1, Me.NomCols + 1)
        ReDim gam(Me.M + 1, Me.NomCols + 1)
        ReDim rhs(Me.M + 1, Me.NomCols + 1)
        ReDim G(Me.M + 1)
        ReDim Xold(Me.M + 1, Me.NomCols + 1)
        Dim alfa2(,) As Single
        Dim gam2(,) As Single
        Dim G2() As Single
        ReDim G2(Me.NomCols + 1)
        ReDim alfa2(Me.M + 1, Me.NomCols + 1)
        ReDim gam2(Me.M + 1, Me.NomCols + 1)

        Dim totDiff As Single
        Dim totOld As Single
        Dim totdiff2 As Single

        Dim Wold As Single = Me.W

        'System.Console.WriteLine("SolveGrid() " & ip.ToString)
        Try
            'first compute LU decomposition elements for each column j
            'If StopRun = 1 Then Exit Sub
            For i = 0 To Me.M + 1
                For j = 0 To Me.NomCols + 1
                    Xold(i, j) = Me.X(i, j, ip)
                Next
            Next
            For j = 1 To Me.NomCols
                'Xold(1, j) = X(1, j, ip)
                If Me.Aloc(Me.iStartRow(j), j, ip) = 0 Then Me.Aloc(Me.iStartRow(j), j, ip) = -1.0 'E+30
                alfa(Me.iStartRow(j), j) = Me.Aloc(Me.iStartRow(j), j, ip)
                gam(Me.iStartRow(j), j) = Me.C(Me.iStartRow(j), j, ip) / alfa(Me.iStartRow(j), j)
                'For i = 1 To M
                'Xold(i, j) = X(i, j, ip)
                'Next
                For i = Me.iStartRow(j) + 1 To Me.iEndRow(j)
                    If Me.Aloc(i, j, ip) = 0 Then Me.Aloc(i, j, ip) = -1.0 'E+30
                    alfa(i, j) = Me.Aloc(i, j, ip) - Me.Bcw(i, j, ip) * gam(i - 1, j)
                    gam(i, j) = Me.C(i, j, ip) / alfa(i, j)
                Next
            Next
            If Me.alternateRowCol Then
                For i = 1 To Me.M
                    'Xold(i, 1) = X(i, 1, ip)
                    If Me.jStartCol(i) <= Me.jEndCol(i) Then 'if the row is not all land
                        If Me.Aloc(i, Me.jStartCol(i), ip) = 0 Then Me.Aloc(i, Me.jStartCol(i), ip) = -1.0 'E+30
                        alfa2(i, Me.jStartCol(i)) = Me.Aloc(i, Me.jStartCol(i), ip)
                        gam2(i, Me.jStartCol(i)) = Me.e(i, Me.jStartCol(i) + 1, ip) / alfa2(i, Me.jStartCol(i))
                    End If
                    For j = Me.jStartCol(i) + 1 To Me.jEndCol(i)
                        'Xold(i, j) = X(i, j, ip)
                        If Me.Aloc(i, j, ip) = 0 Then Me.Aloc(i, j, ip) = -1.0 'E+30
                        alfa2(i, j) = Me.Aloc(i, j, ip) - Me.d(i, j - 1, ip) * gam2(i, j - 1)
                        gam2(i, j) = Me.e(i, j + 1, ip) / alfa2(i, j)
                    Next
                Next
            End If
            'now begin block Gauss-Seidel/SOR iteration over columns of grid
            'at each iteration, solve explicitly for values in each column given
            'current estimates of "forcing" input from other columns based on their
            'current estimates
            iter = 0
iterate:
            For jj = 1 To Me.NomCols

                j = Me.jord(jj)
                For i = Me.iStartRow(j) To Me.iEndRow(j)
                    rhs(i, j) = -Me.Floc(i, j, ip) - Me.d(i, j - 1, ip) * Me.X(i, j - 1, ip) - Me.e(i, j + 1, ip) * Me.X(i, j + 1, ip)
                Next
                rhs(Me.iStartRow(j), j) = rhs(Me.iStartRow(j), j) - Me.Bcw(Me.iStartRow(j), j, ip) * Me.X(Me.iStartRow(j) - 1, j, ip)
                rhs(Me.iEndRow(j), j) = rhs(Me.iEndRow(j), j) - Me.C(Me.iEndRow(j), j, ip) * Me.X(Me.iEndRow(j) + 1, j, ip)
                'now solve for x(i,j) over i using these forcing inputs to one dimensional
                'tridiagonal solver
                G(Me.iStartRow(j)) = rhs(Me.iStartRow(j), j) / alfa(Me.iStartRow(j), j)
                'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
                For i = Me.iStartRow(j) + 1 To Me.iEndRow(j)
                    G(i) = (rhs(i, j) - Me.Bcw(i, j, ip) * G(i - 1)) / alfa(i, j)
                Next

                Me.X(Me.iEndRow(j), j, ip) = G(Me.iEndRow(j))
                For i = Me.iEndRow(j) - 1 To Me.iStartRow(j) Step -1
                    Me.X(i, j, ip) = G(i) - gam(i, j) * Me.X(i + 1, j, ip)
                Next

                For i = Me.iStartRow(j) To Me.iEndRow(j)
                    Me.X(i, j, ip) = (1 - Me.W) * Xold(i, j) + Me.W * Me.X(i, j, ip)
                Next
            Next

            ic = 0
            totDiff = 0
            totdiff2 = 0
            totOld = 0
            For j = 1 To Me.NomCols
                For i = Me.iStartRow(j) To Me.iEndRow(j)
                    If Me.Depth(i, j) > 0 Then

                        If Me.X(i, j, ip) > 0.0000000001 And Math.Abs((Me.X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Me.Tol * Me.timeStep Then
                            ic = ic + 1
                        End If
                        totDiff = totDiff + Math.Abs(Me.X(i, j, ip) - Xold(i, j))
                        totdiff2 = totdiff2 + Me.X(i, j, ip) - Xold(i, j)

                        Xold(i, j) = Me.X(i, j, ip)
                        If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                            Xold(i, j) = 0
                        End If
                        totOld = totOld + Xold(i, j)
                    End If
                Next
            Next

            If Me.alternateRowCol Then
                For i = 1 To Me.M
                    ' If StopRun = 1 Then Exit Sub
                    'j = jord(jj)
                    For j = Me.jStartCol(i) To Me.jEndCol(i)
                        rhs(i, j) = -Me.Floc(i, j, ip) - Me.Bcw(i, j, ip) * Me.X(i - 1, j, ip) - Me.C(i, j, ip) * Me.X(i + 1, j, ip)
                    Next
                    rhs(i, Me.jStartCol(i)) = rhs(i, Me.jStartCol(i)) - Me.d(i, Me.jStartCol(i) - 1, ip) * Me.X(i, Me.jStartCol(i) - 1, ip)
                    rhs(i, Me.jEndCol(i)) = rhs(i, Me.jEndCol(i)) - Me.e(i, Me.jEndCol(i) + 1, ip) * Me.X(i, Me.jEndCol(i) + 1, ip)
                    'now solve for x(i,j) over i using these forcing inputs to one dimensional
                    'tridiagonal solver
                    G2(Me.jStartCol(i)) = rhs(i, Me.jStartCol(i)) / alfa2(i, Me.jStartCol(i))
                    For j = Me.jStartCol(i) To Me.jEndCol(i)
                        G2(j) = (rhs(i, j) - Me.d(i, j - 1, ip) * G2(j - 1)) / alfa2(i, j)
                    Next

                    Me.X(i, Me.jEndCol(i), ip) = G2(Me.jEndCol(i))
                    For j = Me.jEndCol(i) - 1 To Me.jStartCol(i) Step -1
                        Me.X(i, j, ip) = G2(j) - gam2(i, j) * Me.X(i, j + 1, ip)
                    Next

                    For j = Me.jStartCol(i) To Me.jEndCol(i)
                        Me.X(i, j, ip) = (1 - Me.W) * Xold(i, j) + Me.W * Me.X(i, j, ip)
                    Next
                Next

                totDiff = 0
                totdiff2 = 0
                totOld = 0
                ic = 0
                For i = 1 To Me.M
                    For j = Me.jStartCol(i) To Me.jEndCol(i)
                        If Me.Depth(i, j) > 0 Then

                            If Me.X(i, j, ip) > 0.0000000001 And Math.Abs((Me.X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Me.Tol * Me.timeStep Then
                                ic = ic + 1
                            End If

                            totDiff = totDiff + Math.Abs(Me.X(i, j, ip) - Xold(i, j))
                            totdiff2 = totdiff2 + Me.X(i, j, ip) - Xold(i, j)

                            Xold(i, j) = Me.X(i, j, ip)
                            If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                                Xold(i, j) = 0 ': Stop
                            End If
                            totOld = totOld + Xold(i, j)
                        End If
                    Next j
                Next i

                'If Math.Abs(totdiff2 / totDiff) > 0.95 Then
                '    W = 1.9
                'Else
                '    W = Wold
                'End If

            End If

            iter = iter + 1
            If ic > 0 And iter < Me.maxIter Then GoTo iterate
exitline:

            Erase alfa, gam, rhs, G, Xold
            Me.iterThread = Me.iterThread + iter
            If Me.alternateRowCol Then
                iter = iter * 2
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub




    Private Sub CopyToLocal(ip As Integer)

        Debug.Assert(Me.bUseLocalMemory, Me.ToString + ".CopyToLocal() Called when local memory is not being used.")

        Me.m_stpwCopy.Start()

        Me.Xloc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.AlocLoc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.FlocLoc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.bloc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.cloc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.dloc = New Single(Me.M + 1, Me.NomCols + 1) {}
        Me.eloc = New Single(Me.M + 1, Me.NomCols + 1) {}

        'ReDim Xloc(M + 1, NomCols + 1)
        'ReDim AlocLoc(M + 1, NomCols + 1)
        'ReDim FlocLoc(M + 1, NomCols + 1)
        'ReDim bloc(M + 1, NomCols + 1)
        'ReDim cloc(M + 1, NomCols + 1)
        'ReDim dloc(M + 1, NomCols + 1)
        'ReDim eloc(M + 1, NomCols + 1)


        For i As Integer = 0 To Me.M + 1
            For j As Integer = 0 To Me.NomCols + 1
                Me.Xloc(i, j) = Me.X(i, j, ip)
                Me.AlocLoc(i, j) = Me.Aloc(i, j, ip)
                Me.FlocLoc(i, j) = Me.Floc(i, j, ip)
                Me.cloc(i, j) = Me.C(i, j, ip)
                Me.dloc(i, j) = Me.d(i, j, ip)
                Me.eloc(i, j) = Me.e(i, j, ip)
                Me.bloc(i, j) = Me.Bcw(i, j, ip)
            Next
        Next

        Me.m_stpwCopy.Stop()

    End Sub


    Private Sub CopyStartEndRowCol()

        If Me.bUseLocalMemory Then
            Me.m_stpwCopy.Start()

            Me.iStartRowLoc = New Integer(Me.NomCols) {}
            Me.iEndRowLoc = New Integer(Me.NomCols) {}
            Me.jordLoc = New Integer(Me.NomCols) {}
            Me.jStartColLoc = New Integer(Me.M) {}
            Me.jEndColLoc = New Integer(Me.M) {}

            Me.DepthLoc = New Single(Me.M + 1, Me.NomCols + 1) {}

            'ReDim iStartRowLoc(NomCols)
            'ReDim iEndRowLoc(NomCols)
            'ReDim jordLoc(NomCols)
            'ReDim jStartColLoc(M)
            'ReDim jEndColLoc(M)

            'ReDim DepthLoc(M + 1, NomCols + 1)

            'Dim j As Integer
            'Dim i As Integer
            'For i = 1 To M
            '    jStartColLoc(i) = jStartCol(i)
            '    jEndColLoc(i) = jEndCol(i)
            'Next

            'For j = 1 To NomCols
            '    Me.iStartRowLoc(j) = iStartRow(j)
            '    iEndRowLoc(j) = iEndRow(j)
            '    jordLoc(j) = jord(j)
            'Next

            'For i = 0 To M + 1
            '    For j = 0 To NomCols + 1
            '        DepthLoc(i, i) = Depth(i, j)
            '    Next
            'Next
            'ReDim DepthLoc(M + 1, NomCols + 1)
            Array.Copy(Me.Depth, Me.DepthLoc, Me.Depth.Length)

            Array.Copy(Me.iStartRow, Me.iStartRowLoc, Me.NomCols + 1)
            Array.Copy(Me.iEndRow, Me.iEndRowLoc, Me.NomCols + 1)
            Array.Copy(Me.jord, Me.jordLoc, Me.NomCols + 1)

            Array.Copy(Me.jStartCol, Me.jStartColLoc, Me.M + 1)
            Array.Copy(Me.jEndCol, Me.jEndColLoc, Me.M + 1)

            Me.m_stpwCopy.Stop()
        End If


    End Sub


    Private Sub UpdateCoreData(ip As Integer)
        Me.m_stpwCopy.Start()
        For i As Integer = 0 To Me.M + 1
            For j As Integer = 0 To Me.NomCols + 1
                Me.X(i, j, ip) = Me.Xloc(i, j)
                '  Aloc(i, j, ip) = Alocloc(i, j)
                '  Floc(i, j, ip) = Flocloc(i, j)
            Next
        Next
        Me.m_stpwCopy.Stop()
    End Sub

    ''' <summary>
    ''' Solve the grid diffusion matrix on local arrays copied from the core data
    ''' </summary>
    ''' <param name="ip">Index of group</param>
    ''' <remarks>
    ''' Run if <see cref="bUseLocalMemory"></see> = true. Use for big models run on a NUMA (non uniform memory access) computer. 
    ''' This can run faster on a normal multicore computer if the model is big enough i.e. One Degree World model.
    ''' </remarks>
    Private Sub SolveGrid_LocalMemory(ip As Integer)
        'this routine solves for equilibrium field of concentrations x over a grid
        ' Xloc(i,j) is equilibrium concentration of x in grid cell i,j
        'am(i,j) is total loss rate of x from cell i,j...NB:am(i,j)<0 !!!!!!
        'b(i,j) is loss rate from element i-1 to i in column j of grid
        'c(i,j) is loss rate from element i+1 to i in column j of grid
        'd(i,j) is loss rate from element j to element j+1 in row i of grid
        'e(i,j) is loss rate from element j to element j-1 in row i of grid
        'f(i,j) is forcing input to element i,j from sources outside the grid
        'm is number of rows (i) in grid
        'NomCols is number of columns (j) in grid
        'tol is tolerance limit for change in iterative solution
        'jord(k) is which column j to do as k=1, k=2,...,k=n (iteration order)
        'w is SOR overrelaxation parameter-found 1.25 to be good for typical problems
        Dim iter As Integer, j As Integer, i As Integer, jj As Integer, ic As Integer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(Me.M + 1, Me.NomCols + 1)
        ReDim gam(Me.M + 1, Me.NomCols + 1)
        ReDim rhs(Me.M + 1, Me.NomCols + 1)
        ReDim G(Me.M + 1)
        ReDim Xold(Me.M + 1, Me.NomCols + 1)
        Dim alfa2(,) As Single
        Dim gam2(,) As Single
        Dim G2() As Single
        ReDim G2(Me.NomCols + 1)
        ReDim alfa2(Me.M + 1, Me.NomCols + 1)
        ReDim gam2(Me.M + 1, Me.NomCols + 1)

        Dim totDiff As Single
        Dim totOld As Single
        Dim totdiff2 As Single

        Dim Wold As Single = Me.W

        'System.Console.WriteLine("SolveGrid() " & ip.ToString)
        Try

            Me.CopyToLocal(ip)

            'first compute LU decomposition elements for each column j
            'If StopRun = 1 Then Exit Sub
            For i = 0 To Me.M + 1
                For j = 0 To Me.NomCols + 1
                    Xold(i, j) = Me.Xloc(i, j)
                Next
            Next
            For j = 1 To Me.NomCols
                'Xold(1, j) = Xloc(1, j, ip)
                If Me.AlocLoc(Me.iStartRowLoc(j), j) = 0 Then Me.AlocLoc(Me.iStartRowLoc(j), j) = -1.0 'E+30
                alfa(Me.iStartRowLoc(j), j) = Me.AlocLoc(Me.iStartRowLoc(j), j)
                gam(Me.iStartRowLoc(j), j) = Me.cloc(Me.iStartRowLoc(j), j) / alfa(Me.iStartRowLoc(j), j)
                'For i = 1 To M
                'Xold(i, j) = Xloc(i, j, ip)
                'Next
                For i = Me.iStartRowLoc(j) + 1 To Me.iEndRowLoc(j)
                    If Me.AlocLoc(i, j) = 0 Then Me.AlocLoc(i, j) = -1.0 'E+30
                    alfa(i, j) = Me.AlocLoc(i, j) - Me.bloc(i, j) * gam(i - 1, j)
                    gam(i, j) = Me.cloc(i, j) / alfa(i, j)
                Next
            Next
            If Me.alternateRowCol Then
                For i = 1 To Me.M
                    'Xold(i, 1) = Xloc(i, 1, ip)
                    If Me.jStartColLoc(i) <= Me.jEndColLoc(i) Then 'if the row is not all land
                        If Me.AlocLoc(i, Me.jStartColLoc(i)) = 0 Then Me.AlocLoc(i, Me.jStartColLoc(i)) = -1.0 'E+30
                        alfa2(i, Me.jStartColLoc(i)) = Me.AlocLoc(i, Me.jStartColLoc(i))
                        gam2(i, Me.jStartColLoc(i)) = Me.e(i, Me.jStartColLoc(i) + 1, ip) / alfa2(i, Me.jStartColLoc(i))
                    End If
                    For j = Me.jStartColLoc(i) + 1 To Me.jEndColLoc(i)
                        'Xold(i, j) = Xloc(i, j, ip)
                        If Me.AlocLoc(i, j) = 0 Then Me.AlocLoc(i, j) = -1.0 'E+30
                        alfa2(i, j) = Me.AlocLoc(i, j) - Me.dloc(i, j - 1) * gam2(i, j - 1)
                        gam2(i, j) = Me.eloc(i, j + 1) / alfa2(i, j)
                    Next
                Next
            End If
            'now begin block Gauss-Seidel/SOR iteration over columns of grid
            'at each iteration, solve explicitly for values in each column given
            'current estimates of "forcing" input from other columns based on their
            'current estimates
            iter = 0
iterate:
            For jj = 1 To Me.NomCols

                j = Me.jordLoc(jj)
                For i = Me.iStartRowLoc(j) To Me.iEndRowLoc(j)
                    rhs(i, j) = -Me.FlocLoc(i, j) - Me.dloc(i, j - 1) * Me.Xloc(i, j - 1) - Me.eloc(i, j + 1) * Me.Xloc(i, j + 1)
                Next
                rhs(Me.iStartRowLoc(j), j) = rhs(Me.iStartRowLoc(j), j) - Me.bloc(Me.iStartRowLoc(j), j) * Me.Xloc(Me.iStartRowLoc(j) - 1, j)
                rhs(Me.iEndRowLoc(j), j) = rhs(Me.iEndRowLoc(j), j) - Me.cloc(Me.iEndRowLoc(j), j) * Me.Xloc(Me.iEndRowLoc(j) + 1, j)
                'now solve for Xloc(i,j) over i using these forcing inputs to one dimensional
                'tridiagonal solver
                G(Me.iStartRowLoc(j)) = rhs(Me.iStartRowLoc(j), j) / alfa(Me.iStartRowLoc(j), j)
                'IF iflag > 0 THEN FOR i = 1 TO m: PRINT Xloc(i, j), xold(i, j): NEXT: STOP
                For i = Me.iStartRowLoc(j) + 1 To Me.iEndRowLoc(j)
                    G(i) = (rhs(i, j) - Me.bloc(i, j) * G(i - 1)) / alfa(i, j)
                Next

                Me.Xloc(Me.iEndRowLoc(j), j) = G(Me.iEndRowLoc(j))
                For i = Me.iEndRowLoc(j) - 1 To Me.iStartRowLoc(j) Step -1
                    Me.Xloc(i, j) = G(i) - gam(i, j) * Me.Xloc(i + 1, j)
                Next

                For i = Me.iStartRowLoc(j) To Me.iEndRowLoc(j)
                    Me.Xloc(i, j) = (1 - Me.W) * Xold(i, j) + Me.W * Me.Xloc(i, j)
                Next
            Next

            ic = 0
            totDiff = 0
            totdiff2 = 0
            totOld = 0
            For j = 1 To Me.NomCols
                For i = Me.iStartRowLoc(j) To Me.iEndRowLoc(j)
                    If Me.DepthLoc(i, j) > 0 Then

                        If Me.Xloc(i, j) > 0.0000000001 And Math.Abs((Me.Xloc(i, j) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Me.Tol * Me.timeStep Then
                            ic = ic + 1
                        End If
                        totDiff = totDiff + Math.Abs(Me.Xloc(i, j) - Xold(i, j))
                        totdiff2 = totdiff2 + Me.Xloc(i, j) - Xold(i, j)

                        Xold(i, j) = Me.Xloc(i, j)
                        If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                            Xold(i, j) = 0
                        End If
                        totOld = totOld + Xold(i, j)
                    End If
                Next
            Next

            If Me.alternateRowCol Then
                For i = 1 To Me.M
                    ' If StopRun = 1 Then Exit Sub
                    'j = jord(jj)
                    For j = Me.jStartColLoc(i) To Me.jEndColLoc(i)
                        rhs(i, j) = -Me.FlocLoc(i, j) - Me.bloc(i, j) * Me.Xloc(i - 1, j) - Me.cloc(i, j) * Me.Xloc(i + 1, j)
                    Next
                    rhs(i, Me.jStartColLoc(i)) = rhs(i, Me.jStartColLoc(i)) - Me.dloc(i, Me.jStartColLoc(i) - 1) * Me.Xloc(i, Me.jStartColLoc(i) - 1)
                    rhs(i, Me.jEndColLoc(i)) = rhs(i, Me.jEndColLoc(i)) - Me.eloc(i, Me.jEndColLoc(i) + 1) * Me.Xloc(i, Me.jEndColLoc(i) + 1)
                    'now solve for Xloc(i,j) over i using these forcing inputs to one dimensional
                    'tridiagonal solver
                    G2(Me.jStartColLoc(i)) = rhs(i, Me.jStartColLoc(i)) / alfa2(i, Me.jStartColLoc(i))
                    For j = Me.jStartColLoc(i) To Me.jEndColLoc(i)
                        G2(j) = (rhs(i, j) - Me.dloc(i, j - 1) * G2(j - 1)) / alfa2(i, j)
                    Next

                    Me.Xloc(i, Me.jEndColLoc(i)) = G2(Me.jEndColLoc(i))
                    For j = Me.jEndColLoc(i) - 1 To Me.jStartColLoc(i) Step -1
                        Me.Xloc(i, j) = G2(j) - gam2(i, j) * Me.Xloc(i, j + 1)
                    Next

                    For j = Me.jStartColLoc(i) To Me.jEndColLoc(i)
                        Me.Xloc(i, j) = (1 - Me.W) * Xold(i, j) + Me.W * Me.Xloc(i, j)
                    Next
                Next

                totDiff = 0
                totdiff2 = 0
                totOld = 0
                ic = 0
                For i = 1 To Me.M
                    For j = Me.jStartColLoc(i) To Me.jEndColLoc(i)
                        If Me.DepthLoc(i, j) > 0 Then

                            If Me.Xloc(i, j) > 0.0000000001 And Math.Abs((Me.Xloc(i, j) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Me.Tol * Me.timeStep Then
                                ic = ic + 1
                            End If

                            totDiff = totDiff + Math.Abs(Me.Xloc(i, j) - Xold(i, j))
                            totdiff2 = totdiff2 + Me.Xloc(i, j) - Xold(i, j)

                            Xold(i, j) = Me.Xloc(i, j)
                            If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                                Xold(i, j) = 0 ': Stop
                            End If
                            totOld = totOld + Xold(i, j)
                        End If
                    Next j
                Next i

                'If Math.Abs(totdiff2 / totDiff) > 0.95 Then
                '    W = 1.9
                'Else
                '    W = Wold
                'End If

            End If

            iter = iter + 1
            If ic > 0 And iter < Me.maxIter Then GoTo iterate
exitline:

            Erase alfa, gam, rhs, G, Xold
            Me.iterThread = Me.iterThread + iter
            If Me.alternateRowCol Then
                iter = iter * 2
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Me.UpdateCoreData(ip)

    End Sub

    Private Sub SolveGridRow(ip As Integer)
        'this routine solves for equilibrium field of concentrations x over a grid
        ' x(i,j) is equilibrium concentration of x in grid cell i,j
        'am(i,j) is total loss rate of x from cell i,j...NB:am(i,j)<0 !!!!!!
        'b(i,j) is loss rate from element i-1 to i in column j of grid
        'c(i,j) is loss rate from element i+1 to i in column j of grid
        'd(i,j) is loss rate from element j to element j+1 in row i of grid
        'e(i,j) is loss rate from element j to element j-1 in row i of grid
        'f(i,j) is forcing input to element i,j from sources outside the grid
        'm is number of rows (i) in grid
        'NomCols is number of columns (j) in grid
        'tol is tolerance limit for change in iterative solution
        'jord(k) is which column j to do as k=1, k=2,...,k=n (iteration order)
        'w is SOR overrelaxation parameter-found 1.25 to be good for typical problems
        Dim iter As Integer, j As Integer, i As Integer, ic As Integer ', ii As Integer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(Me.M + 1, Me.NomCols + 1)
        ReDim gam(Me.M + 1, Me.NomCols + 1)
        ReDim rhs(Me.M + 1, Me.NomCols + 1)
        ReDim G(Me.NomCols + 1)
        ReDim Xold(Me.M + 1, Me.NomCols + 1)

        Dim totDiff As Single
        Dim totOld As Single

        Dim iindex As Integer
        Dim jindex As Integer

        'first compute LU decomposition elements for each column j
        'If StopRun = 1 Then Exit Sub
        For i = 0 To Me.M + 1
            For j = 0 To Me.NomCols + 1
                Xold(i, j) = Me.X(i, j, ip)
            Next
        Next
        For i = 1 To Me.M
            'Xold(i, 1) = X(i, 1, ip)
            If Me.jStartCol(i) < Me.jEndCol(i) Then 'if the row has any water
                If Me.Aloc(i, Me.jStartCol(i), ip) = 0 Then Me.Aloc(i, Me.jStartCol(i), ip) = -1.0E+30
                alfa(i, Me.jStartCol(i)) = Me.Aloc(i, Me.jStartCol(i), ip)
                gam(i, Me.jStartCol(i)) = Me.e(i, Me.jStartCol(i) + 1, ip) / alfa(i, Me.jStartCol(i))
            End If
            For j = Me.jStartCol(i) + 1 To Me.jEndCol(i)
                'Xold(i, j) = X(i, j, ip)
                If Me.Aloc(i, j, ip) = 0 Then Me.Aloc(i, j, ip) = -1.0E+30
                alfa(i, j) = Me.Aloc(i, j, ip) - Me.d(i, j - 1, ip) * gam(i, j - 1)
                gam(i, j) = Me.e(i, j + 1, ip) / alfa(i, j)
            Next
        Next
        'now begin block Gauss-Seidel/SOR iteration over columns of grid
        'at each iteration, solve explicitly for values in each column given
        'current estimates of "forcing" input from other columns based on their
        'current estimates
        iter = 0
iterate:
        For i = 1 To Me.M
            ' If StopRun = 1 Then Exit Sub
            'j = jord(jj)
            For j = Me.jStartCol(i) To Me.jEndCol(i)
                rhs(i, j) = -Me.Floc(i, j, ip) - Me.Bcw(i, j, ip) * Me.X(i - 1, j, ip) - Me.C(i, j, ip) * Me.X(i + 1, j, ip)
            Next
            rhs(i, Me.jStartCol(i)) = rhs(i, Me.jStartCol(i)) - Me.d(i, Me.jStartCol(i) - 1, ip) * Me.X(i, Me.jStartCol(i) - 1, ip)
            rhs(i, Me.jEndCol(i)) = rhs(i, Me.jEndCol(i)) - Me.e(i, Me.jEndCol(i) + 1, ip) * Me.X(i, Me.jEndCol(i) + 1, ip)

            'now solve for x(i,j) over i using these forcing inputs to one dimensional
            'tridiagonal solver
            G(Me.jStartCol(i)) = rhs(i, Me.jStartCol(i)) / alfa(i, Me.jStartCol(i))
            For j = Me.jStartCol(i) To Me.jEndCol(i)
                G(j) = (rhs(i, j) - Me.d(i, j - 1, ip) * G(j - 1)) / alfa(i, j)
            Next

            Me.X(i, Me.jEndCol(i), ip) = G(Me.jEndCol(i))
            For j = Me.jEndCol(i) - 1 To Me.jStartCol(i) Step -1
                Me.X(i, j, ip) = G(j) - gam(i, j) * Me.X(i, j + 1, ip)
            Next

            For j = Me.jStartCol(i) To Me.jEndCol(i)
                Me.X(i, j, ip) = (1 - Me.W) * Xold(i, j) + Me.W * Me.X(i, j, ip)
            Next
        Next

        ic = 0
        totDiff = 0
        totOld = 0
        For i = 1 To Me.M
            For j = Me.jStartCol(i) To Me.jEndCol(i)
                If Me.Depth(i, j) > 0 Then

                    If Me.X(i, j, ip) > 0.0000000001 And Math.Abs((Me.X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Me.Tol * Me.timeStep Then
                        ic = ic + 1
                        iindex = i
                        jindex = j
                        totDiff = totDiff + Math.Abs(Me.X(i, j, ip) - Xold(i, j))
                        totOld = totOld + Xold(i, j)
                    End If
                    If iter = Me.maxIter - 1 Then

                    End If
                    Xold(i, j) = Me.X(i, j, ip)
                    If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                        Xold(i, j) = 0 ': Stop
                    End If

                End If
            Next j
        Next i

        iter = iter + 1
        If ic > 0 And iter < Me.maxIter Then GoTo iterate

exitline:
        Erase alfa, gam, rhs, G, Xold
        Me.iterThread = Me.iterThread + iter
    End Sub

    Private Function arrangeMatrix(ByRef Amm(,,) As Single, ByRef Bcw(,,) As Single, ByRef C(,,) As Single, ByRef d(,,) As Single, ByRef e(,,) As Single, ip As Integer, M As Integer, N As Integer) As Double(,)
        'takes the Amm, Bcw, C, d and e arrays and puts them into a single compressed band diagonal array (a)

        Dim a(,) As Double
        ReDim a(N * M + 1, 2 * N + 1)
        Dim i As Integer, j As Integer
        Dim row As Integer

        For i = 1 To M
            For j = 1 To N
                row = j + N * (i - 1)
                a(row, 2 * N + 1) = C(i, j, ip)
                a(row - 1, N + 2) = e(i, j, ip)
                If Amm(i, j, ip) < 0 Then
                    a(row, N + 1) = Amm(i, j, ip)
                Else
                    a(row, N + 1) = -1
                End If
                a(row, 1) = Bcw(i, j, ip)
                a(row + 1, N) = d(i, j, ip)
            Next
        Next

        'Dim str As String
        'Dim tempstr As String
        ''Dim temp As Single
        'For i = 1 To N * M + 1
        '    str = ""
        '    For j = 1 To 2 * N + 1
        '        tempstr = Math.Round(a(i, j)).ToString
        '        str = str + tempstr + " "
        '    Next
        '    'Debug.Print(str) : Stop
        'Next
        'Debug.Print(vbCr)
        'Dim temp(,) As Single
        'ReDim temp(N * M, N * M)
        'For i = 1 To M
        '    For j = 1 To N
        '        row = j + M * (i - 1)
        '        temp(row, row) = Amm(i, j, ip)
        '        If row < N * M Then
        '            temp(row, row + 1) = e(i, j + 1, ip)
        '        End If
        '        If row < N * M - N + 1 Then
        '            temp(row, row + N) = C(i, j, ip)
        '        End If
        '        If row > 1 Then
        '            temp(row, row - 1) = d(i, j - 1, ip)
        '        End If
        '        If row > N Then
        '            temp(row, row - N) = Bcw(i, j, ip)
        '        End If
        '    Next
        'Next
        'For i = 1 To N * M
        '    str = ""
        '    For j = 1 To N * M
        '        str = str + temp(i, j).ToString + " "
        '    Next
        '    'Debug.Print(str)
        'Next
        arrangeMatrix = a

    End Function

    Private Sub bandec(ByRef a(,) As Double, totCells As Integer, N As Integer,
        ByRef al(,) As Double, ByRef indx() As Integer, ByRef d As Single)

        Dim i As Integer, j As Integer, k As Integer, l As Integer
        Dim mm As Integer
        Dim dum As Double
        Dim TINY As Single = 1.0E-20
        Try
            mm = 2 * N + 1
            l = N

            For i = 1 To N
                For j = N + 2 - i To mm ' rearrange storage a bit
                    a(i, j - l) = a(i, j)
                Next
                l = l - 1
                For j = mm - l To mm
                    a(i, j) = 0.0
                Next
            Next
            d = 1.0
            l = N
            For k = 1 To totCells 'for each row
                dum = a(k, 1)
                i = k
                If l < totCells Then l = l + 1
                For j = k + 1 To l 'find the pivot element
                    If Math.Abs(a(j, 1)) > Math.Abs(dum) Then
                        dum = a(j, 1)
                        i = j
                    End If
                Next
                indx(k) = i
                If dum = 0.0 Then
                    a(k, 1) = TINY 'matrix is algorithmically singular
                    Debug.Assert(False, "Matrix  algorithm failed: 0 pivot found - matrix appears to be singular")
                    Throw New Exception("Matrix  algorithm failed: 0 pivot found - matrix appears to be singular")
                End If
                'displaymatrix(a, N, N)
                If i <> k Then 'interchange rows
                    d = -d
                    For j = 1 To mm 'swap elements
                        dum = a(k, j)
                        a(k, j) = a(i, j)
                        a(i, j) = dum
                    Next
                End If
                'displaymatrix(a, N, N)
                For i = k + 1 To l 'do the elimination
                    dum = a(i, 1) / (a(k, 1))
                    al(k, i - k) = dum
                    For j = 2 To mm
                        a(i, j - 1) = a(i, j) - dum * a(k, j)
                    Next
                    a(i, mm) = 0.0
                    'displaymatrix(a, N, N)
                Next
            Next
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub displaymatrix(ByRef a(,) As Double, M As Integer, N As Integer)
        Dim str As String
        Dim tempstr As String
        'Dim temp As Single
        Try
            For i As Integer = 1 To N * M
                str = ""
                For j As Integer = 1 To 2 * N + 1
                    If a(i, j) > 0 Then
                        tempstr = Math.Ceiling(a(i, j)).ToString
                    Else
                        tempstr = Math.Floor(a(i, j)).ToString
                    End If
                    str = str + tempstr + " "
                Next
                Debug.Print(str)
            Next
            Debug.Print(Environment.NewLine)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub bandks(ByRef a(,) As Double, totCells As Integer, N As Integer, ByRef al(,) As Double, ByRef indx() As Integer, ByRef b() As Single)
        Dim i As Integer, k As Integer, l As Integer
        Dim mm As Integer
        Dim dum As Single

        mm = 2 * N + 1
        l = N

        For k = 1 To totCells 'forward substitution, unscramblings rows as we go
            i = indx(k)
            If i <> k Then 'swap
                dum = b(k)
                b(k) = b(i)
                b(i) = dum
            End If
            If l < totCells Then l += 1
            For i = k + 1 To l
                b(i) -= CSng(al(k, i - k) * b(k))
            Next
        Next
        l = 1
        For i = totCells To 1 Step -1
            dum = b(i)
            For k = 2 To l
                dum -= CSng(a(i, k) * b(k + i - 1))
            Next
            b(i) = CSng(dum / (a(i, 1) + 1.0E-20))
            If l < mm Then l += 1
        Next
    End Sub

    Private Sub refillX(ByRef X(,,) As Single, ByRef b() As Single, ip As Integer, M As Integer, N As Integer)
        Dim i As Integer, j As Integer

        For i = 1 To M
            For j = 1 To N
                If b(j + N * (i - 1)) > 1.0E-20 Then
                    X(i, j, ip) = b(j + N * (i - 1))
                Else
                    X(i, j, ip) = 1.0E-21
                End If

            Next
        Next
    End Sub

    'Private Function randMatrix(M As Integer, N As Integer) As Double(,)
    '    Dim a(,) As Double
    '    Dim i As Integer
    '    ReDim a(M * N, 2 * N + 1)
    '    For i = 1 To M * N
    '        a(i, 1) = Me.m_rand.NextDouble()
    '        a(i, N) = Me.m_rand.NextDouble()
    '        a(i, N + 2) = Me.m_rand.NextDouble()
    '        a(i, 2 * N + 1) = Me.m_rand.NextDouble()
    '        a(i, N + 1) = (-1.0 - Me.m_rand.NextDouble() / 10) * (a(i, 2 * N + 1) + a(i, N + 2) + a(i, N) + a(i, 1)) 'Rnd()
    '    Next
    '    displaymatrix(a, M, N)
    '    randMatrix = a
    'End Function
#End Region

End Class
