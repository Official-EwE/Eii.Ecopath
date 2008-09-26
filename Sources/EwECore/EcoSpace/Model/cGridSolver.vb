'==============================================================================
'
' $Log: cGridSolver.vb,v $
' Revision 1.1  2008/09/26 07:30:23  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.19  2007/12/26 19:14:55  joeb
' removed dead code
'
' Revision 1.18  2007/07/06 20:27:02  willw
' added comments
'
' Revision 1.17  2007/07/03 19:19:43  willw
' useExact is now set from the interface
'
' Revision 1.16  2007/06/29 00:26:19  jeroens
' + Added UseExact to Init
'
'==============================================================================

Imports System
Imports System.Threading

Public Class cGridSolver

#Region "Public data"

    ''' <summary>
    ''' Signal mechanism used by the calling thread for thread Synchronization
    ''' </summary>
    ''' <remarks>
    ''' When the Solve() thread is running (SignalState in a non-signaled state SignalState.Reset()) 
    ''' calls to SignalState.WaitOne() will block until Solve() has completed (SignalState in a signaled state SignalState.Set())
    ''' </remarks>
    Public SignalState As New ManualResetEvent(True)
    Public isOkToRunning As Boolean

    Public iterThread As Integer 'total iterations 
#End Region

#Region "Private data"

    Public ThreadID As Integer

    'arguments from original code
    'Sub SolveGrid(ByVal ip As Integer, ByVal Aloc(,,) As Single, ByVal Floc(,,) As Single, ByVal X(,,) As Single, ByVal M As Integer, ByVal NomCols As Integer, ByVal Tol As Single, ByVal jord() As Integer, ByVal W As Single)
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
    Private Depth(,) As Integer

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

    Public threadTime As Single

    Private threadGroups(,) As Integer

    Private useExact As Boolean

#End Region

#Region "Constructor and Initialization"

    Public Sub New(ByVal ThreadNumber As Integer)
        isOkToRunning = True
        ThreadID = ThreadNumber
    End Sub

    ''' <summary>
    ''' Set all references to data used for calculation
    ''' </summary>
    ''' <remarks>This needs a bunch more data</remarks>
    ''' SolveGrid(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
    ''' SolveGridRow(ip, AMm, F, m_Data.Bcell, m_Data.Inrow, m_Data.InCol, Tol, jord, m_Data.W)
    Public Function Init(ByRef AMm(,,) As Single, ByRef F(,,) As Single, ByRef BCell(,,) As Single, ByRef Inrow As Integer, ByRef InCol As Integer, ByRef Tol1 As Single, ByRef jord1() As Integer, ByRef W1 As Single, ByRef Bcw1(,,) As Single, ByRef C1(,,) As Single, ByRef d1(,,) As Single, ByRef e1(,,) As Single, ByRef Depth1(,) As Integer, ByVal BPIntegrate() As Boolean, ByRef iStartRow1() As Integer, ByRef iEndRow1() As Integer, ByVal timeStep1 As Single, ByVal maxIter1 As Integer, ByRef jStartCol1() As Integer, ByRef jEndCol1() As Integer, ByRef isMigratory1() As Boolean, ByRef threadGroups1(,) As Integer, ByVal bUseExact As Boolean) As Boolean
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
            Return iFrstGrp
        End Get
    End Property


    ''' <summary>
    ''' Set the groups to iterate over.
    ''' </summary>
    ''' <param name="iFirstGroup"></param>
    ''' <param name="iLastGroup"></param>
    ''' <remarks>Call for each thread, before the thread is started, to set the groups to solve.</remarks>
    Public Sub FirstLastGroups(ByVal iFirstGroup As Integer, ByVal iLastGroup As Integer)
        iFrstGrp = iFirstGroup
        iLastGrp = iLastGroup
    End Sub

#End Region

#Region "Public 'Solve'"

    ''' <summary>
    ''' This is the method that the ThreadPool calls. 
    ''' It must have the object argument to match the Delegate signature required by ThreadPool.QueueUserWorkItem()
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Solve(ByVal obParam As Object)
        'For our purposes here we are ignoring the obParam argument 
        'this sub signature is required by the ThreadPool.QueueUserWorkItem(...)
        Dim timeTemp As Single = Microsoft.VisualBasic.Timer
        'if this is running on a thread this may not work
        'all flags need to be set outside the thread
        isOkToRunning = False
        iterThread = 0
        Dim iGrp As Integer
        Dim i As Integer
        Try
            'set signal state to 'non-signaled' SignalState.WaitOne() will block
            SignalState.Reset()


            alternateRowCol = True
            'useExact = True

            'do the processing here
            'System.Console.WriteLine("Solve() " & iFrstGrp.ToString & "," & iLastGrp.ToString)
            For i = iFrstGrp To iLastGrp
                iGrp = threadGroups(ThreadID, i)
                'System.Console.WriteLine("Solve() " & iGrp.ToString)
                ' imig = iGrp
                'If imig > isMigratory.Length - 1 Then imig = 0

                If ByPassIntegrate(iGrp) = False Then
                    If useExact And isMigratory(iGrp) Then
                        solveExact(iGrp)
                    Else
                        SolveGrid(iGrp)
                        If Not alternateRowCol Then
                            SolveGridRow(iGrp)
                        End If
                    End If
                End If

            Next i

            'set signal state to 'signaled' 
            'the processing has finished SignalState.WaitOne() will return immediately
            threadTime = threadTime + Microsoft.VisualBasic.Timer - timeTemp
            'thread has finished it is ok to run this again
            isOkToRunning = True
            SignalState.Set()

        Catch ex As Exception
            cLog.Write(ex) 'this is dangerous clog.Write is not thread safe

            Debug.Assert(False, ex.Message)
            'prevent this thread from blocking forever if it throws an error
            SignalState.Reset()
            'not sure about this
            '  Throw New ApplicationException("Error in " & Me.ToString & ".Solve()", ex)
            isOkToRunning = True

        End Try


    End Sub

    Private Sub solveExact(ByVal ip As Integer)
        'this solves the linearized implicit biomass flux equations directly rather that iteratively
        'this is takes longer than a few iterations in solvegrid, but shorter than a lot
        'if the species requires a lot of iterations (e.g. migratory), this should be better than s.g.
        'bandec and bandks are taken from numerical recipes (press et al.), translated to vb
        Dim a(,) As Double 'reduced band form of Bcell
        Dim al(,) As Double 'reduced band form of L (from LU decomp)
        Dim de As Single
        Dim b() As Single 'input vector b (=-F)
        Dim indx() As Integer 'index table for row subsitutions

        ReDim al(NomCols * M + 1, 2 * NomCols + 1)
        ReDim b(M * NomCols)
        ReDim indx(M * NomCols)

        'get b vector from F
        Dim k As Integer = 0
        For i As Integer = 1 To M
            For j As Integer = 1 To NomCols
                k += 1
                b(k) = -Floc(i, j, ip)
            Next
        Next

        Try
            'for the eq. Ax=b
            'get the reduced band form of a from the fluxes (see press et al. 2-4)
            a = arrangeMatrix(Aloc, Bcw, C, d, e, ip, M, NomCols)

            'get the LU decomposition of a (now a=L and al=L)
            bandec(a, M * NomCols, NomCols, al, indx, de)

            'back substitution to get x (now stored in b)
            bandks(a, M * NomCols, NomCols, al, indx, b)

            'refill the Bcell array with the values in the vector x
            refillX(X, b, ip, M, NomCols)

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub


#End Region

#Region "Grid solving computational code"

    Private Sub SolveGrid(ByVal ip As Integer)
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
        Dim xx As Single = Microsoft.VisualBasic.Timer

        Dim alfa(,) As Single
        Dim gam(,) As Single
        Dim rhs(,) As Single
        Dim G() As Single
        Dim Xold(,) As Single
        ReDim alfa(M + 1, NomCols + 1)
        ReDim gam(M + 1, NomCols + 1)
        ReDim rhs(M + 1, NomCols + 1)
        ReDim G(M + 1)
        ReDim Xold(M + 1, NomCols + 1)
        Dim alfa2(,) As Single
        Dim gam2(,) As Single
        Dim G2() As Single
        ReDim G2(NomCols + 1)
        ReDim alfa2(M + 1, NomCols + 1)
        ReDim gam2(M + 1, NomCols + 1)

        Dim totDiff As Single
        Dim totOld As Single
        Dim totdiff2 As Single

        Dim Wold As Single = W

        'System.Console.WriteLine("SolveGrid() " & ip.ToString)
        'Debug.Assert(ip < 28, False)
        Try
            'first compute LU decomposition elements for each column j
            'If StopRun = 1 Then Exit Sub
            For i = 0 To M + 1
                For j = 0 To NomCols + 1
                    Xold(i, j) = X(i, j, ip)
                Next
            Next
            For j = 1 To NomCols
                'Xold(1, j) = X(1, j, ip)
                If Aloc(iStartRow(j), j, ip) = 0 Then Aloc(iStartRow(j), j, ip) = -1.0 'E+30
                alfa(iStartRow(j), j) = Aloc(iStartRow(j), j, ip)
                gam(iStartRow(j), j) = C(iStartRow(j), j, ip) / alfa(iStartRow(j), j)
                'For i = 1 To M
                'Xold(i, j) = X(i, j, ip)
                'Next
                For i = iStartRow(j) + 1 To iEndRow(j)
                    If Aloc(i, j, ip) = 0 Then Aloc(i, j, ip) = -1.0 'E+30
                    alfa(i, j) = Aloc(i, j, ip) - Bcw(i, j, ip) * gam(i - 1, j)
                    gam(i, j) = C(i, j, ip) / alfa(i, j)
                Next
            Next
            If alternateRowCol Then
                For i = 1 To M
                    'Xold(i, 1) = X(i, 1, ip)
                    If jStartCol(i) <= jEndCol(i) Then 'if the row is not all land
                        If Aloc(i, jStartCol(i), ip) = 0 Then Aloc(i, jStartCol(i), ip) = -1.0 'E+30
                        alfa2(i, jStartCol(i)) = Aloc(i, jStartCol(i), ip)
                        gam2(i, jStartCol(i)) = e(i, jStartCol(i) + 1, ip) / alfa2(i, jStartCol(i))
                    End If
                    For j = jStartCol(i) + 1 To jEndCol(i)
                        'Xold(i, j) = X(i, j, ip)
                        If Aloc(i, j, ip) = 0 Then Aloc(i, j, ip) = -1.0 'E+30
                        alfa2(i, j) = Aloc(i, j, ip) - d(i, j - 1, ip) * gam2(i, j - 1)
                        gam2(i, j) = e(i, j + 1, ip) / alfa2(i, j)
                    Next
                Next
            End If
            'now begin block Gauss-Seidel/SOR iteration over columns of grid
            'at each iteration, solve explicitly for values in each column given
            'current estimates of "forcing" input from other columns based on their
            'current estimates
            iter = 0
iterate:
            For jj = 1 To NomCols

                'ToDo_jb SolveGrid StopRun needs to be implemented this flag is in here and SolveGridRow
                'If StopRun = 1 Then Exit Sub

                j = jord(jj)
                For i = iStartRow(j) To iEndRow(j)
                    rhs(i, j) = -Floc(i, j, ip) - d(i, j - 1, ip) * X(i, j - 1, ip) - e(i, j + 1, ip) * X(i, j + 1, ip)
                Next
                rhs(iStartRow(j), j) = rhs(iStartRow(j), j) - Bcw(iStartRow(j), j, ip) * X(iStartRow(j) - 1, j, ip)
                rhs(iEndRow(j), j) = rhs(iEndRow(j), j) - C(iEndRow(j), j, ip) * X(iEndRow(j) + 1, j, ip)
                'now solve for x(i,j) over i using these forcing inputs to one dimensional
                'tridiagonal solver
                G(iStartRow(j)) = rhs(iStartRow(j), j) / alfa(iStartRow(j), j)
                'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
                For i = iStartRow(j) + 1 To iEndRow(j)
                    G(i) = (rhs(i, j) - Bcw(i, j, ip) * G(i - 1)) / alfa(i, j)
                Next
                X(iEndRow(j), j, ip) = G(iEndRow(j))
                For i = iEndRow(j) - 1 To iStartRow(j) Step -1
                    X(i, j, ip) = G(i) - gam(i, j) * X(i + 1, j, ip)
                Next
                'IF iflag > 0 THEN
                '        FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT
                '        PRINT FRE(-1), FRE(-2)
                '        : STOP
                'END IF
                For i = iStartRow(j) To iEndRow(j)
                    X(i, j, ip) = (1 - W) * Xold(i, j) + W * X(i, j, ip)
                Next
            Next

            ic = 0
            totDiff = 0
            totdiff2 = 0
            totOld = 0
            For j = 1 To NomCols
                For i = iStartRow(j) To iEndRow(j)
                    If Depth(i, j) > 0 Then

                        If X(i, j, ip) > 0.0000000001 And Math.Abs((X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Tol * timeStep Then
                            ic = ic + 1

                        End If
                        totDiff = totDiff + Math.Abs(X(i, j, ip) - Xold(i, j))
                        totdiff2 = totdiff2 + X(i, j, ip) - Xold(i, j)

                        Xold(i, j) = X(i, j, ip)
                        If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                            Xold(i, j) = 0
                        End If
                        totOld = totOld + Xold(i, j)
                    End If
                Next
            Next
            If (ip = 3 Or ip = 17) Then 'And iter > maxIter / 4 Then
                'Debug.Print("SG   iter:  " + iter.ToString + "  ip: " + ip.ToString + "   ic:  " + ic.ToString + "   totDiff:  " + (totDiff / totOld).ToString + "   " + (totdiff2 / totOld).ToString)
            End If
            'LOCATE 1, 1: Print "SOR it="; iter;: LOCATE 2, 1: Print "    nc="; ic;
            ' Label12.Caption = iter : Label13.Caption = ic 'DoEvents
            If alternateRowCol Then
                For i = 1 To M
                    ' If StopRun = 1 Then Exit Sub
                    'j = jord(jj)
                    For j = jStartCol(i) To jEndCol(i)
                        rhs(i, j) = -Floc(i, j, ip) - Bcw(i, j, ip) * X(i - 1, j, ip) - C(i, j, ip) * X(i + 1, j, ip)
                    Next
                    rhs(i, jStartCol(i)) = rhs(i, jStartCol(i)) - d(i, jStartCol(i) - 1, ip) * X(i, jStartCol(i) - 1, ip)
                    rhs(i, jEndCol(i)) = rhs(i, jEndCol(i)) - e(i, jEndCol(i) + 1, ip) * X(i, jEndCol(i) + 1, ip)
                    'now solve for x(i,j) over i using these forcing inputs to one dimensional
                    'tridiagonal solver
                    G2(jStartCol(i)) = rhs(i, jStartCol(i)) / alfa2(i, jStartCol(i))
                    'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
                    For j = jStartCol(i) To jEndCol(i)
                        G2(j) = (rhs(i, j) - d(i, j - 1, ip) * G2(j - 1)) / alfa2(i, j)
                    Next
                    X(i, jEndCol(i), ip) = G2(jEndCol(i))
                    For j = jEndCol(i) - 1 To jStartCol(i) Step -1
                        X(i, j, ip) = G2(j) - gam2(i, j) * X(i, j + 1, ip)
                    Next
                    'IF iflag > 0 THEN
                    '        FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT
                    '        PRINT FRE(-1), FRE(-2)
                    '        : STOP
                    'END IF
                    For j = jStartCol(i) To jEndCol(i)
                        X(i, j, ip) = (1 - W) * Xold(i, j) + W * X(i, j, ip)
                    Next
                Next


                totDiff = 0
                totdiff2 = 0
                totOld = 0
                ic = 0
                For i = 1 To M
                    For j = jStartCol(i) To jEndCol(i)
                        If Depth(i, j) > 0 Then

                            If X(i, j, ip) > 0.0000000001 And Math.Abs((X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Tol * timeStep Then
                                ic = ic + 1

                            End If
                            totDiff = totDiff + Math.Abs(X(i, j, ip) - Xold(i, j))
                            totdiff2 = totdiff2 + X(i, j, ip) - Xold(i, j)

                            Xold(i, j) = X(i, j, ip)
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

                If (ip = 3 Or ip = 17) Then 'And iter > maxIter / 4 Then
                    'Debug.Print("SGR  iter:  " + iter.ToString + "  ip: " + ip.ToString + "   ic:  " + ic.ToString + "   totDiff:  " + (totDiff / totOld).ToString + "   " + (totdiff2 / totOld).ToString)
                End If
            End If

            iter = iter + 1
            If ic > 0 And iter < maxIter Then GoTo iterate
            'CLS
            'LOCATE 1, 1
            'FOR i = 1 TO 20: PRINT USING "## "; i; : FOR j = 1 TO nomcols: PRINT USING " .##"; x(i, j); : NEXT: PRINT : NEXT
            'WHILE INKEY$ = "": WEND
exitline:
            xx = Microsoft.VisualBasic.Timer - xx
            Erase alfa, gam, rhs, G, Xold
            If alternateRowCol Then
                iter = iter * 2
            End If
            iterThread = iterThread + iter
            'If (ip = 3 Or ip = 17) And iter > maxIter / 4 Then
            '    'Debug.Print("SG  iter:  " + iter.ToString + "  ip: " + ip.ToString + "   ic:  " + ic.ToString + "   totDiff:  " + (totDiff / totOld).ToString)
            'End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub SolveGridRow(ByVal ip As Integer)
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
        ReDim alfa(M + 1, NomCols + 1)
        ReDim gam(M + 1, NomCols + 1)
        ReDim rhs(M + 1, NomCols + 1)
        ReDim G(NomCols + 1)
        ReDim Xold(M + 1, NomCols + 1)

        Dim totDiff As Single
        Dim totOld As Single

        Dim iindex As Integer
        Dim jindex As Integer

        'first compute LU decomposition elements for each column j
        'If StopRun = 1 Then Exit Sub
        For i = 0 To M + 1
            For j = 0 To NomCols + 1
                Xold(i, j) = X(i, j, ip)
            Next
        Next
        For i = 1 To M
            'Xold(i, 1) = X(i, 1, ip)
            If jStartCol(i) < jEndCol(i) Then 'if the row has any water
                If Aloc(i, jStartCol(i), ip) = 0 Then Aloc(i, jStartCol(i), ip) = -1.0E+30
                alfa(i, jStartCol(i)) = Aloc(i, jStartCol(i), ip)
                gam(i, jStartCol(i)) = e(i, jStartCol(i) + 1, ip) / alfa(i, jStartCol(i))
            End If
            For j = jStartCol(i) + 1 To jEndCol(i)
                'Xold(i, j) = X(i, j, ip)
                If Aloc(i, j, ip) = 0 Then Aloc(i, j, ip) = -1.0E+30
                alfa(i, j) = Aloc(i, j, ip) - d(i, j - 1, ip) * gam(i, j - 1)
                gam(i, j) = e(i, j + 1, ip) / alfa(i, j)
            Next
        Next
        'now begin block Gauss-Seidel/SOR iteration over columns of grid
        'at each iteration, solve explicitly for values in each column given
        'current estimates of "forcing" input from other columns based on their
        'current estimates
        iter = 0
iterate:
        For i = 1 To M
            ' If StopRun = 1 Then Exit Sub
            'j = jord(jj)
            For j = jStartCol(i) To jEndCol(i)
                rhs(i, j) = -Floc(i, j, ip) - Bcw(i, j, ip) * X(i - 1, j, ip) - C(i, j, ip) * X(i + 1, j, ip)
            Next
            rhs(i, jStartCol(i)) = rhs(i, jStartCol(i)) - d(i, jStartCol(i) - 1, ip) * X(i, jStartCol(i) - 1, ip)
            rhs(i, jEndCol(i)) = rhs(i, jEndCol(i)) - e(i, jEndCol(i) + 1, ip) * X(i, jEndCol(i) + 1, ip)
            'now solve for x(i,j) over i using these forcing inputs to one dimensional
            'tridiagonal solver
            G(jStartCol(i)) = rhs(i, jStartCol(i)) / alfa(i, jStartCol(i))
            'IF iflag > 0 THEN FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT: STOP
            For j = jStartCol(i) To jEndCol(i)
                G(j) = (rhs(i, j) - d(i, j - 1, ip) * G(j - 1)) / alfa(i, j)
            Next
            X(i, jEndCol(i), ip) = G(jEndCol(i))
            For j = jEndCol(i) - 1 To jStartCol(i) Step -1
                X(i, j, ip) = G(j) - gam(i, j) * X(i, j + 1, ip)
            Next
            'IF iflag > 0 THEN
            '        FOR i = 1 TO m: PRINT x(i, j), xold(i, j): NEXT
            '        PRINT FRE(-1), FRE(-2)
            '        : STOP
            'END IF
            For j = jStartCol(i) To jEndCol(i)
                X(i, j, ip) = (1 - W) * Xold(i, j) + W * X(i, j, ip)
            Next
        Next

        ic = 0
        totDiff = 0
        totOld = 0
        For i = 1 To M
            For j = jStartCol(i) To jEndCol(i)
                If Depth(i, j) > 0 Then

                    If X(i, j, ip) > 0.0000000001 And Math.Abs((X(i, j, ip) - Xold(i, j)) / (Xold(i, j) + 1.0E-20)) > Tol * timeStep Then
                        ic = ic + 1
                        iindex = i
                        jindex = j
                        totDiff = totDiff + Math.Abs(X(i, j, ip) - Xold(i, j))
                        totOld = totOld + Xold(i, j)
                    End If
                    If iter = maxIter - 1 Then

                    End If
                    Xold(i, j) = X(i, j, ip)
                    If Math.Abs(Xold(i, j)) < 1.0E-20 Then
                        Xold(i, j) = 0 ': Stop
                    End If

                End If
            Next j
        Next i
        If (ip = 3 Or ip = 17) Then 'And iter > maxIter / 4 Then
            'Debug.Print("SGR  iter:  " + iter.ToString + "  ip: " + ip.ToString + "   ic:  " + ic.ToString + "   totDiff:  " + (totDiff / totOld).ToString)
        End If
        'LOCATE 1, 1: Print "SOR it="; iter;: LOCATE 2, 1: Print "    nc="; ic;
        ' Label12.Caption = iter: Label13.Caption = ic: 'DoEvents
        iter = iter + 1
        If ic > 0 And iter < maxIter Then GoTo iterate
        'CLS
        'LOCATE 1, 1
        'FOR i = 1 TO 20: PRINT USING "## "; i; : FOR j = 1 TO nomcols: PRINT USING " .##"; x(i, j); : NEXT: PRINT : NEXT
        'WHILE INKEY$ = "": WEND
exitline:
        Erase alfa, gam, rhs, G, Xold
        iterThread = iterThread + iter
        If (ip = 3 Or ip = 17) And iter > maxIter / 4 Then
            'Debug.Print("SGR:   ip: " + ip.ToString + "   ic:  " + ic.ToString + "   totDiff:  " + (totDiff / totOld).ToString)
        End If
    End Sub

    Private Function arrangeMatrix(ByRef Amm(,,) As Single, ByRef Bcw(,,) As Single, ByRef C(,,) As Single, ByRef d(,,) As Single, ByRef e(,,) As Single, ByVal ip As Integer, ByVal M As Integer, ByVal N As Integer) As Double(,)
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

    Private Sub bandec(ByRef a(,) As Double, ByVal totCells As Integer, ByVal N As Integer, _
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

    Private Sub displaymatrix(ByRef a(,) As Double, ByVal M As Integer, ByVal N As Integer)
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
            Debug.Print(vbCr)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try
    End Sub

    Private Sub bandks(ByRef a(,) As Double, ByVal totCells As Integer, ByVal N As Integer, ByRef al(,) As Double, ByRef indx() As Integer, ByRef b() As Single)
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
                b(i) -= al(k, i - k) * b(k)
            Next
        Next
        l = 1
        For i = totCells To 1 Step -1
            dum = b(i)
            For k = 2 To l
                dum -= a(i, k) * b(k + i - 1)
            Next
            b(i) = dum / (a(i, 1) + 1.0E-20)
            If l < mm Then l += 1
        Next
    End Sub

    Private Sub refillX(ByRef X(,,) As Single, ByRef b() As Single, ByVal ip As Integer, ByVal M As Integer, ByVal N As Integer)
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

    Private Function randMatrix(ByVal M As Integer, ByVal N As Integer) As Double(,)
        Dim a(,) As Double
        Dim i As Integer
        ReDim a(M * N, 2 * N + 1)
        For i = 1 To M * N
            a(i, 1) = Rnd()
            a(i, N) = Rnd()
            a(i, N + 2) = Rnd()
            a(i, 2 * N + 1) = Rnd()
            a(i, N + 1) = (-1.0 - Rnd() / 10) * (a(i, 2 * N + 1) + a(i, N + 2) + a(i, N) + a(i, 1)) 'Rnd()
        Next
        displaymatrix(a, M, N)
        randMatrix = a
    End Function
#End Region

End Class
