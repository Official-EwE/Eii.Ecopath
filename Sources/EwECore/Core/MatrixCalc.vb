''' <summary>
''' Module containing routines for Matrix equation solver
''' </summary>
''' <remarks></remarks>
Public Module MatrixCalc
    'MatSEqnS and matluS variables
    'array dimensions used by MatSEqnS and matluS
    Public Lo As Integer
    Public Up As Integer

    Public rpvt() As Integer, cpvt() As Integer

    '========================MatSEqnS==================================
    'MatSEqnS solves a system of n linear equations, Ax=b, and puts the
    'answer in b. A is first put in LU form by matluS, then matbsS is called
    'to solve the system.  matrices A,b are single precision.
    '
    'Parameters: A(n x n) contains coefficient matrix, b(N) contains the right side
    '
    'Returns: A in LU form, solution in b
    '===================================================================
    Public Function MatSEqnS(ByRef A(,) As Single, ByRef B() As Single) As Integer
        'MsgBox "MatSeqns"
        Dim ErrCode As Integer, bserrcode As Integer, row As Integer
        Dim X() As Single
        Dim OkToContinue As Boolean

        ErrCode = 0

        Try
            'On Local Error GoTo sseqnerr: 
            'Lo = LBound(A, 1)
            'jb in EwE5 lo boundary of the arrays was set to 1 we can not do that here so hard wire this value
            Lo = 1
            Up = UBound(A, 1)
            ReDim X(Up)
            ReDim rpvt(Up)
            ReDim cpvt(Up)

            ErrCode = matluS(A, OkToContinue)                      'Get LU matrix
            'If Not OkToContinue Then Error ErrCode
            If Not OkToContinue Then
                Debug.Assert(False, "matluS returned false.")
                Return ErrCode
            End If
            'check dimensions of b
            'If (Lo <> LBound(B)) Or (Up <> UBound(B)) Then Error 197
            If (Up <> UBound(B)) Then
                Debug.Assert(False)
                Return 197
            End If

            bserrcode = matbsS(A, B, X)          'Backsolve system
            'If bserrcode Then Error bserrcode
            If bserrcode Then
                Debug.Assert(False)
                Return bserrcode
            End If

            For row = Lo To Up
                B(row) = X(row)                         'Put solution in b for return
            Next row

        Catch ex As Exception

            Debug.Assert(False)
            Return 0
        End Try



        '        If ErrCode Then Error ErrCode
        'sseqnexit:
        '        Erase X, rpvt, cpvt
        '        MatSEqnS = ErrCode
        '        Exit Function
        'sseqnerr:
        '        ErrCode = (Err() + 5) Mod 200 - 5
        '        Resume sseqnexit
    End Function
    '========================matluS====================================
    'matluS does Gaussian elimination with total pivoting to put a square, single
    'precision matrix in LU form. The multipliers used in the row operations to
    'create zeroes below the main diagonal are saved in the zero spaces.
    '
    'Parameters: A(n x n) matrix, rpvt(n) and cpvt(n) permutation vectors
    '            used to index the row and column pivots
    '
    'Returns: A in LU form with corresponding pivot vectors; the total number of
    '         pivots in count, which is used to find the sign of the determinant.
    '===================================================================
    Public Function matluS(ByRef A(,) As Single, ByRef OkToContinue As Boolean) As Integer

        Dim ErrCode As Integer, row As Integer, col As Integer, pvt As Integer, max As Single, R As Integer
        Dim CCC As Integer, bestrow As Integer, bestcol As Integer
        Dim rownorm() As Single
        Dim seps As Single, oldmax As Single, rp As Integer, cp As Integer
        Dim count As Integer
        Dim Temp As Single
        'On Local Error GoTo sluerr: ErrCode = 0

        Try
            'Checks if A is square, returns error code if not
            If Up <> UBound(A, 2) Then
                'If Not (Lo = LBound(A, 2) And Up = UBound(A, 2)) Then

                Debug.Assert(False)
                Return 198
            End If

            ReDim rownorm(Up)
            count = 0                            'initialize count, OkToContinue
            OkToContinue = True

            For row = Lo To Up                  'initialize rpvt and cpvt
                rpvt(row) = row
                cpvt(row) = row
                rownorm(row) = 0.0                'find the row norms of A()
                For col = Lo To Up
                    rownorm(row) = rownorm(row) + Math.Abs(A(row, col))
                Next col
                'If rownorm(Row) = 0! Then        'if any rownorm is zero, the matrix
                '    OkToContinue = 0                   'is singular, set error, exit and do
                '    Error 199                      'not OkToContinue
                'End If
            Next row

            For pvt = Lo To (Up - 1)
                'Find best available pivot
                max = 0.0                         'checks all values in rows and columns not
                For row = pvt To Up             'already used for pivoting and finds the
                    R = rpvt(row)                'number largest in absolute value relative
                    For col = pvt To Up          'to its row norm
                        CCC = cpvt(col)
                        If (rownorm(R) <> 0) Then
                            Temp = Math.Abs(A(R, CCC)) / rownorm(R)
                        End If

                        If Temp > max Then
                            max = Temp
                            bestrow = row          'save the position of new max!
                            bestcol = col
                        End If
                    Next col
                Next row

                If max = 0.0 Then                 'if no nonzero number is found, A is
                    Debug.Assert(False)
                    OkToContinue = False                   'singular, send back error, do not OkToContinue
                    Return 199
                ElseIf pvt > 1 Then              'check if drop in pivots is too much
                    If max < (seps * oldmax) Then
                        OkToContinue = False
                        Return 199
                    End If
                End If

                oldmax = max
                If rpvt(pvt) <> rpvt(bestrow) Then
                    count = count + 1                    'if a row or column pivot is
                    'SWAP rpvt(pvt), rpvt(bestrow)      'necessary, count it and permute
                    Temp = rpvt(pvt)
                    rpvt(pvt) = rpvt(bestrow)
                    rpvt(bestrow) = Temp
                End If                                  'rpvt or cpvt. Note: the rows and
                If cpvt(pvt) <> cpvt(bestcol) Then    'columns are not actually switched,
                    count = count + 1                    'only the order in which they are
                    'SWAP cpvt(pvt), cpvt(bestcol)      'used.
                    Temp! = cpvt(pvt)
                    cpvt(pvt) = cpvt(bestrow)
                    cpvt(bestrow) = Temp!
                End If
                'Eliminate all values below the pivot
                rp = rpvt(pvt)
                cp = cpvt(pvt)
                For row = (pvt + 1) To Up
                    R = rpvt(row)

                    If (A(rp, cp) <> 0) Then
                        A(R, cp) = -A(R, cp) / A(rp, cp)  'save multipliers
                    End If
                    For col = (pvt + 1) To Up
                        CCC = cpvt(col)                      'complete row operations
                        A(R, CCC) = A(R, CCC) + A(R, cp) * A(rp, CCC)
                    Next col
                Next row
            Next pvt

            If A(rpvt(Up), cpvt(Up)) = 0.0 Then
                'if last pivot is zero or pivot drop is
                'too large, A is singular, send back error
                OkToContinue = 0
                'DispError 0, "Last pivot is zero or pivot drop is too large."
                Debug.Assert(False, "matlus: Last pivot is zero or pivot drop is too large.")
                Return 199
            ElseIf (Math.Abs(A(rpvt(Up), cpvt(Up))) / rownorm(rpvt(Up))) < (seps * oldmax) Then
                'if pivot is not identically zero then
                'OkToContinue remains TRUE
                'Debug.Assert(False, "matlus: Last pivot is zero or pivot drop is too large.")
                Return 199
            End If

            If ErrCode Then
                'DispError 0, "pivot is not identically zero."
                'Error ErrCode
                'jb I don't think this can happen
                Debug.Assert(False)
                Return ErrCode
            End If

        Catch ex As Exception
            OkToContinue = False
            Debug.Assert(False, ex.Message)
            Return 199
        End Try

        'sluexit:
        '        matluS = ErrCode
        '        Exit Function

        'sluerr:
        '        ErrCode = Err()
        '        If ErrCode < 199 Then OkToContinue = False
        '        Resume sluexit
    End Function
    '========================matbsS=====================================
    'matbsS takes a matrix in LU form, found by matluS, and a vector b
    'and solves the system Ux=Lb for x. matrices A,b,x are single precision.
    '
    'Parameters: LU matrix in A, corresponding pivot vectors in rpvt and cpvt,
    '            right side in b
    '
    'Returns: solution in x, b is modified, rest unchanged
    '===================================================================
    Public Function matbsS(ByRef A(,) As Single, ByRef B() As Single, ByRef X() As Single) As Integer
        Dim pvt As Integer, CCC As Integer, col As Integer, row As Integer, R As Integer

        Try

            'On Local Error GoTo sbserr: matbsS = 0
            'do row operations on b using the multipliers in L to find Lb
            For pvt = Lo To (Up - 1)
                CCC = cpvt(pvt)
                For row = (pvt + 1) To Up
                    R = rpvt(row)
                    B(R) = B(R) + A(R, CCC) * B(rpvt(pvt))
                Next row
            Next pvt

            'backsolve Ux=Lb to find x
            For row = Up To Lo Step -1
                CCC = cpvt(row)
                R = rpvt(row)
                X(CCC) = B(R)
                For col = (row + 1) To Up
                    X(CCC) = X(CCC) - A(R, cpvt(col)) * X(cpvt(col))
                Next col

                If A(R, CCC) <> 0 Then
                    X(CCC) = X(CCC) / A(R, CCC)
                End If
            Next row

        Catch ex As Exception
            'any return value other the zero is considered an error
            Return 1
            Exit Function

        End Try

        'no error
        Return 0

        'sbsexit:
        '        Exit Function
        'sbserr:
        '        matbsS = Err()
        '        Resume sbsexit
    End Function


End Module
