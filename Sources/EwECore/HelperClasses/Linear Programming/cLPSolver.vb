' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Common
Imports LpSolveDotNet
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

''' ---------------------------------------------------------------------------
''' <summary>
''' LP solver interface to the unmanaged lp_solve engine version 5.5
''' </summary>
''' <remarks>
''' Please refer to the Microsoft Solver Foundation API reference for using the
''' methods in this class. Note that this solver wraps unmanaged code; this class
''' will only work on Windows.
''' </remarks>
''' ---------------------------------------------------------------------------
Public Class cLPSolver
    Implements ILPSolver

#Region " Private classes "
    Private Class cDef
        Public m_key As Object
        Public m_ord As Integer
        Public m_dMin As Double
        Public m_dMax As Double
        Public m_dResult As Double
        Public m_DualValue As Double
        Public Sub New(key As Object, ord As Integer)
            Me.m_key = key
            Me.m_ord = ord
        End Sub
    End Class

    Private Class cVarDef
        Inherits cDef
        Public Sub New(key As Object, ord As Integer)
            MyBase.New(key, ord)
        End Sub
    End Class

    Private Class cRowDef
        Inherits cDef
        Public m_dVals As New Dictionary(Of Object, Double)
        Public Sub New(key As Object, ord As Integer)
            MyBase.New(key, ord)
        End Sub
    End Class

#End Region ' Private classes

#Region " Private vars "

    Private m_lDefs As New List(Of cDef)
    Private m_iGoal As Integer = -1
    Private m_bMinimize As Boolean = False
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of cLPSolver)()

#End Region ' Private vars

#Region " Public access "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.m_lDefs.Add(Nothing) ' One-based index
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddVariable"/>
    ''' -----------------------------------------------------------------------
    Public Function AddVariable(key As Object, ByRef iIndex As Integer) As Boolean _
        Implements ILPSolver.AddVariable
        iIndex = Me.m_lDefs.Count
        Me.m_lDefs.Add(New cVarDef(key, iIndex))
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.SetBounds"/>
    ''' -----------------------------------------------------------------------
    Public Sub SetBounds(iVar As Integer, dMin As Double, dMax As Double) _
          Implements ILPSolver.SetBounds
        Dim vd As cDef = Me.m_lDefs(iVar)
        vd.m_dMin = dMin
        vd.m_dMax = dMax
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddRow"/>
    ''' -----------------------------------------------------------------------
    Public Function AddRow(key As Object, ByRef iIndex As Integer) As Boolean _
          Implements ILPSolver.AddRow
        iIndex = Me.m_lDefs.Count
        Me.m_lDefs.Add(New cRowDef(key, iIndex))
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.AddVariable"/>
    ''' -----------------------------------------------------------------------
    Public Function AddGoal(iRow As Integer, ip As Integer, bMinimize As Boolean) As Boolean _
         Implements ILPSolver.AddGoal
        ' ip (priority) is ignored
        Me.m_iGoal = iRow
        Me.m_bMinimize = bMinimize
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.SetCoefficient"/>
    ''' -----------------------------------------------------------------------
    Public Sub SetCoefficient(iRow As Integer, iVar As Integer, dVal As Double) _
         Implements ILPSolver.SetCoefficient
        Dim rd As cRowDef = DirectCast(Me.m_lDefs(iRow), cRowDef)
        Dim vd As cVarDef = DirectCast(Me.m_lDefs(iVar), cVarDef)
        rd.m_dVals(vd.m_key) = dVal
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.Solve"/>
    ''' <remarks>
    ''' This method creates the unmanaged solver, populates and runs it, extracts
    ''' results and destroys the unmanaged solver.
    ''' </remarks>
    ''' <returns>
    ''' True if ran successful. Remember to check whether this particular solver 
    ''' <see cref="IsSupported">is supported by the operating system</see>.
    ''' </returns>
    ''' -----------------------------------------------------------------------
    Public Function Solve(iTimeStepIndex As Integer) As eSolverReturnValues _
          Implements ILPSolver.Solve
        Dim rv As eSolverReturnValues

        Debug.Assert(Me.m_iGoal > 0, "Goal not defined")

        ' Safety check
        If Not Me.IsSupported Then
            Debug.Assert(False, "LpSolveDotNet did not initialize")
            Return eSolverReturnValues.ERROR
        End If

        Dim vars() As cVarDef = Me.Vars
        Dim rows() As cRowDef = Me.Rows
        Dim lp As LpSolve = Nothing

        Try
            lp = LpSolve.make_lp(0, vars.Length)
        Catch ex As Exception
            m_logger.LogError(ex, "cLPSolver.Solve() Failed on make_lp(,)")
            Return eSolverReturnValues.ERROR
        End Try

        Try

            For v As Integer = 0 To vars.Length - 1
                Dim vd As cVarDef = vars(v)
                lp.set_bounds(vd.m_ord, vd.m_dMin, vd.m_dMax)
                lp.set_col_name(vd.m_ord, vd.m_key.ToString())
            Next

            For r As Integer = 0 To rows.Length - 1
                Dim dRow(vars.Length) As Double
                Dim rd As cRowDef = rows(r)
                For v As Integer = 0 To vars.Length - 1
                    Dim vd As cVarDef = vars(v)
                    If rd.m_dVals.ContainsKey(vd.m_key) Then
                        dRow(v + 1) = rd.m_dVals(vd.m_key)
                    End If
                Next v
                Dim bAdded As Boolean

                'only add a lower constraint if it is not equal to zero
                If rd.m_dMin <> 0 Then
                    bAdded = lp.add_constraint(dRow, lpsolve_constr_types.GE, rd.m_dMin)
                End If

                'Always add the upper constraint! I think LPSolve will ignore constraints that are zero! Maybe...
                lp.add_constraint(dRow, lpsolve_constr_types.LE, rd.m_dMax)

                lp.set_row_name(rd.m_ord, rd.m_key.ToString())
            Next r

            If True Then
                Dim dRow(vars.Length) As Double
                Dim rd As cRowDef = Me.Goal()
                For v As Integer = 0 To vars.Length - 1
                    Dim vd As cVarDef = vars(v)
                    If rd.m_dVals.ContainsKey(vd.m_key) Then
                        dRow(v + 1) = rd.m_dVals(vd.m_key)
                    End If
                Next v
                lp.set_obj_fn(dRow)
                If Me.m_bMinimize Then
                    lp.set_minim()
                Else
                    lp.set_maxim()
                End If
            End If

            Dim lpResult As lpsolve_return
            lpResult = lp.solve()

            'this works because there is a one to one mapping for LpSolveDotNet_return and eSolverReturnValues
            rv = CType(lpResult, eSolverReturnValues)

            If rv <> eSolverReturnValues.OPTIMAL Then

#If DEBUG Then
                'Need to find a better way to do this
                Dim tmpPath As String = System.IO.Path.GetTempPath
                Dim solverFile As String = System.IO.Path.Combine(tmpPath, "EWE6_LPSolve_model_" & iTimeStepIndex.ToString & ".txt")
                System.Console.WriteLine("cLPSolver.Solve() Non Optimal Solution: " & lpResult.ToString & " Timestep " & iTimeStepIndex.ToString & " file saved to ")
                System.Console.WriteLine(solverFile)
                lp.write_lp(solverFile)
#End If
            End If

            ' This looks incredibly fragile...
            Dim n As Integer = 1 + Me.Vars.Length + Me.Rows.Length
            Debug.Assert(n = 1 + lp.get_Ncolumns() + lp.get_Nrows(), "cLPSolver number of variables and rows does not match.")

            Dim dualValues(n) As Double
            Dim dSol(n) As Double

            Dim iSol As Integer = 0
            lp.get_primal_solution(dSol)
            lp.get_dual_solution(dualValues)
            Me.Goal.m_dResult = dSol(iSol)
            iSol += 1

            For iRow As Integer = 0 To rows.Length - 1
                rows(iRow).m_dResult = dSol(iSol)
                rows(iRow).m_DualValue = dualValues(iSol)
                iSol += 1
            Next

            For iVar As Integer = 0 To vars.Length - 1
                vars(iVar).m_dResult = dSol(iSol)
                vars(iVar).m_DualValue = dualValues(iSol)
                iSol += 1
            Next

            '  LpSolveDotNet.write_lp(lp, "cLPSolver.txt")

        Catch ex As Exception
            rv = eSolverReturnValues.ERROR
        End Try

        'LpSolveDotNet.write_lp(lp, "cLPSolver.txt")

        lp.delete_lp()

        Return rv

    End Function

    Public Sub SolveLPSolve()

        'SimplexSolver solver = new SimplexSolver();
        Dim lp As LpSolve = LpSolve.make_lp(0, 2)

        ' - Vars already defined in constructor
        'int savid, vzvid;
        'solver.AddVariable("Saudi Arabia", out savid);
        'solver.SetBounds(savid, 0, 9000);
        lp.set_bounds(1, 0, 9000)
        'solver.AddVariable("Venezuela", out vzvid);
        'solver.SetBounds(vzvid, 0, 6000);
        lp.set_bounds(2, 0, 6000)

        'int gasoline, jetfuel, machinelubricant, cost;
        Dim drow As Double()

        'solver.AddRow("gasoline", out gasoline);
        'solver.SetCoefficient(gasoline, savid, 0.3);
        'solver.SetCoefficient(gasoline, vzvid, 0.4);
        'solver.SetBounds(gasoline, 2000, Rational.PositiveInfinity);
        drow = New Double() {0, 0.3, 0.4}
        lp.add_constraint(drow, lpsolve_constr_types.GE, 2000)

        'solver.AddRow("jetfuel", out jetfuel);
        'solver.SetCoefficient(jetfuel, savid, 0.4);
        'solver.SetCoefficient(jetfuel, vzvid, 0.2);
        'solver.SetBounds(jetfuel, 1500, Rational.PositiveInfinity);
        drow = New Double() {0, 0.4, 0.2}
        lp.add_constraint(drow, lpsolve_constr_types.GE, 1500)

        'solver.AddRow("machinelubricant", out machinelubricant);
        'solver.SetCoefficient(machinelubricant, savid, 0.2);
        'solver.SetCoefficient(machinelubricant, vzvid, 0.3);
        'solver.SetBounds(machinelubricant, 500, Rational.PositiveInfinity);
        drow = New Double() {0, 0.2, 0.3}
        lp.add_constraint(drow, lpsolve_constr_types.GE, 500)

        'solver.AddRow("cost", out cost);
        'solver.SetCoefficient(cost, savid, 20);
        'solver.SetCoefficient(cost, vzvid, 15);
        'solver.AddGoal(cost, 1, true);
        drow = New Double() {0, 20, 15}
        lp.set_obj_fn(drow)

        'solver.Solve(new SimplexSolverParams());
        lp.set_minim()

        'LpSolveDotNet.print_lp(lp)
        lp.solve()

        'Console.WriteLine("SA {0}, VZ {1}, Gasoline {2}, Jet Fuel {3}, Machine Lubricant {4}, Cost {5}",
        '    solver.GetValue(savid).ToDouble(),
        '    solver.GetValue(vzvid).ToDouble(),
        '    solver.GetValue(gasoline).ToDouble(),
        '    solver.GetValue(jetfuel).ToDouble(),
        '    solver.GetValue(machinelubricant).ToDouble(),
        '    solver.GetValue(cost).ToDouble());

        'LpSolveDotNet.print_objective(lp)
        'LpSolveDotNet.print_solution(lp, 1)
        'LpSolveDotNet.print_constraints(lp, 1)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.GetValue"/>
    ''' -----------------------------------------------------------------------
    Public Function GetValue(iData As Integer) As Double _
          Implements ILPSolver.GetValue
        Return Me.m_lDefs(iData).m_dResult
    End Function

    Public Function GetDualValue(iData As Integer) As Double Implements ILPSolver.GetDualValue
        Return Me.m_lDefs(iData).m_DualValue
    End Function

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="ILPSolver.IsSupported"/>
    ''' -----------------------------------------------------------------------
    Public Function IsSupported() As Boolean Implements ILPSolver.IsSupported
        LpSolve.Init()
        Return LpSolve.Init()
    End Function

#End Region ' Public access

#Region " Internals "

    Private Function Vars() As cVarDef()
        Dim lvars As New List(Of cVarDef)
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cVarDef Then
                Dim vd As cVarDef = DirectCast(def, cVarDef)
                lvars.Add(vd)
            End If
        Next
        Return lvars.ToArray
    End Function

    Private Function Rows() As cRowDef()
        Dim lrows As New List(Of cRowDef)
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cRowDef Then
                Dim rd As cRowDef = DirectCast(def, cRowDef)
                If rd.m_ord <> Me.m_iGoal Then
                    lrows.Add(rd)
                End If
            End If
        Next
        Return lrows.ToArray()
    End Function

    Private Function Goal() As cRowDef
        For Each def As cDef In Me.m_lDefs
            If TypeOf def Is cRowDef Then
                Dim rd As cRowDef = DirectCast(def, cRowDef)
                If rd.m_ord = Me.m_iGoal Then Return rd
            End If
        Next
        Return Nothing
    End Function

#End Region ' Internals

End Class
