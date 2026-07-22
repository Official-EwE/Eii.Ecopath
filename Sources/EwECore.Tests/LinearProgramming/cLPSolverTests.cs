// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).
// Copyright © 1991– Ecopath International Initiative (EII)

using EwECore;
using EwECore.Common;
using FluentAssertions;

namespace EwECore.Tests.LinearProgramming;

/// <summary>
/// Unit tests for <see cref="cLPSolver"/>, the lp_solve 5.5 wrapper that
/// implements <see cref="ILPSolver"/>.
///
/// The tests document the <em>contract</em> of the solver so that, when the
/// underlying implementation is later replaced by the managed lp_solve .NET
/// package, every test still compiles and the results must match.
///
/// Each test guards with <see cref="IsSolverAvailable"/> and returns early
/// (without failing) when the native lpsolve55.dll is absent, e.g. on Linux CI.
/// </summary>
public sealed class cLPSolverTests
{
    // ------------------------------------------------------------------
    // Helpers
    // ------------------------------------------------------------------

    /// <summary>True when the native lpsolve55.dll can be loaded.</summary>
    private static bool IsSolverAvailable => new cLPSolver().IsSupported();

    private static cLPSolver CreateSolver() => new cLPSolver();

    // ------------------------------------------------------------------
    // IsSupported
    // ------------------------------------------------------------------

    [Fact]
    public void IsSupported_OnWindowsWithDll_ReturnsTrue()
    {
        // Arrange
        if (!System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                System.Runtime.InteropServices.OSPlatform.Windows))
            return;

        var solver = new cLPSolver();

        // Act
        bool result = solver.IsSupported();

        // Assert
        result.Should().BeTrue("lpsolve55.dll should be loadable on Windows");
    }

    // ------------------------------------------------------------------
    // AddVariable / AddRow – index assignment
    // ------------------------------------------------------------------

    [Fact]
    public void AddVariable_FirstCall_AssignsIndexGreaterThanZero()
    {
        // Arrange
        if (!IsSolverAvailable) return;
        var solver = CreateSolver();
        int idx = 0;

        // Act
        bool ok = solver.AddVariable("x", ref idx);

        // Assert
        ok.Should().BeTrue();
        idx.Should().BeGreaterThan(0, "indices are 1-based in the shared definition list");
    }

    [Fact]
    public void AddVariable_SecondCall_AssignsHigherIndexThanFirst()
    {
        // Arrange
        if (!IsSolverAvailable) return;
        var solver = CreateSolver();
        int iX = 0, iY = 0;

        // Act
        solver.AddVariable("x", ref iX);
        solver.AddVariable("y", ref iY);

        // Assert
        iY.Should().BeGreaterThan(iX);
    }

    [Fact]
    public void AddRow_AssignsIndexHigherThanLastVariable()
    {
        // Arrange
        if (!IsSolverAvailable) return;
        var solver = CreateSolver();
        int iX = 0, iRow = 0;
        solver.AddVariable("x", ref iX);

        // Act
        solver.AddRow("row1", ref iRow);

        // Assert
        iRow.Should().BeGreaterThan(iX);
    }

    // ------------------------------------------------------------------
    // Solve – maximize using only variable bounds (no constraint rows)
    //
    //   Maximize:  x + y
    //   Subject to: 0 ≤ x ≤ 4
    //               0 ≤ y ≤ 6
    //
    //   Optimal: x = 4, y = 6, objective = 10
    // ------------------------------------------------------------------

    /// <summary>
    /// Builds the simple 2-variable maximisation problem.
    /// Returns (null,0,0,0) when the solver is unavailable so callers can guard.
    /// </summary>
    private static (cLPSolver? solver, int iX, int iY, int iObj) BuildSimpleMaxProblem()
    {
        if (!IsSolverAvailable) return (null, 0, 0, 0);
        var solver = CreateSolver();

        int iX = 0, iY = 0, iObj = 0;
        solver.AddVariable("x", ref iX);
        solver.SetBounds(iX, 0.0, 4.0);

        solver.AddVariable("y", ref iY);
        solver.SetBounds(iY, 0.0, 6.0);

        // Objective row – bounds on the goal row are never used as constraints.
        solver.AddRow("obj", ref iObj);
        solver.SetCoefficient(iObj, iX, 1.0);
        solver.SetCoefficient(iObj, iY, 1.0);
        solver.SetBounds(iObj, 0.0, double.PositiveInfinity);
        solver.AddGoal(iObj, 1, bMinimize: false);

        return (solver, iX, iY, iObj);
    }

    [Fact]
    public void Solve_MaximizeWithVariableBoundsOnly_ReturnsOptimal()
    {
        // Arrange
        var (solver, _, _, _) = BuildSimpleMaxProblem();
        if (solver is null) return;

        // Act
        var result = solver.Solve(0);

        // Assert
        result.Should().Be(eSolverReturnValues.OPTIMAL);
    }

    [Fact]
    public void Solve_MaximizeWithVariableBoundsOnly_VariableX_IsAtUpperBound()
    {
        // Arrange
        var (solver, iX, _, _) = BuildSimpleMaxProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iX);

        // Assert
        value.Should().BeApproximately(4.0, precision: 1e-6);
    }

    [Fact]
    public void Solve_MaximizeWithVariableBoundsOnly_VariableY_IsAtUpperBound()
    {
        // Arrange
        var (solver, _, iY, _) = BuildSimpleMaxProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iY);

        // Assert
        value.Should().BeApproximately(6.0, precision: 1e-6);
    }

    [Fact]
    public void Solve_MaximizeWithVariableBoundsOnly_ObjectiveValue_IsCorrect()
    {
        // Arrange
        var (solver, _, _, iObj) = BuildSimpleMaxProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iObj);

        // Assert
        value.Should().BeApproximately(10.0, precision: 1e-6);
    }

    // ------------------------------------------------------------------
    // Solve – minimize using variable lower bounds (no constraint rows)
    //
    //   Minimize:  x + 2·y
    //   Subject to: 2 ≤ x ≤ 10
    //               3 ≤ y ≤ 10
    //
    //   Optimal: x = 2, y = 3, objective = 8
    // ------------------------------------------------------------------

    private static (cLPSolver? solver, int iX, int iY, int iObj) BuildSimpleMinProblem()
    {
        if (!IsSolverAvailable) return (null, 0, 0, 0);
        var solver = CreateSolver();

        int iX = 0, iY = 0, iObj = 0;
        solver.AddVariable("x", ref iX);
        solver.SetBounds(iX, 2.0, 10.0);

        solver.AddVariable("y", ref iY);
        solver.SetBounds(iY, 3.0, 10.0);

        solver.AddRow("cost", ref iObj);
        solver.SetCoefficient(iObj, iX, 1.0);
        solver.SetCoefficient(iObj, iY, 2.0);
        solver.SetBounds(iObj, 0.0, double.PositiveInfinity);
        solver.AddGoal(iObj, 1, bMinimize: true);

        return (solver, iX, iY, iObj);
    }

    [Fact]
    public void Solve_MinimizeWithVariableLowerBounds_ReturnsOptimal()
    {
        // Arrange
        var (solver, _, _, _) = BuildSimpleMinProblem();
        if (solver is null) return;

        // Act
        var result = solver.Solve(0);

        // Assert
        result.Should().Be(eSolverReturnValues.OPTIMAL);
    }

    [Fact]
    public void Solve_MinimizeWithVariableLowerBounds_VariableX_IsAtLowerBound()
    {
        // Arrange
        var (solver, iX, _, _) = BuildSimpleMinProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iX);

        // Assert
        value.Should().BeApproximately(2.0, precision: 1e-6);
    }

    [Fact]
    public void Solve_MinimizeWithVariableLowerBounds_VariableY_IsAtLowerBound()
    {
        // Arrange
        var (solver, _, iY, _) = BuildSimpleMinProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iY);

        // Assert
        value.Should().BeApproximately(3.0, precision: 1e-6);
    }

    [Fact]
    public void Solve_MinimizeWithVariableLowerBounds_ObjectiveValue_IsCorrect()
    {
        // Arrange
        var (solver, _, _, iObj) = BuildSimpleMinProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iObj);

        // Assert
        value.Should().BeApproximately(8.0, precision: 1e-6);
    }

    // ------------------------------------------------------------------
    // Solve – maximize with LE constraint rows (mirrors MSE fleet pattern)
    //
    //   Two fleets (variables), two bio-groups (LE-only constraint rows,
    //   matching SetBounds(row, 0, FTarget) as used in cMSE).
    //
    //   Maximize: 3·f1 + 2·f2
    //   Subject to:
    //     0.5·f1 + 0.3·f2  ≤  2      (group1 F-target)
    //     0.2·f1 + 0.4·f2  ≤  1.5    (group2 F-target)
    //     0 ≤ f1 ≤ 10
    //     0 ≤ f2 ≤ 10
    //
    //   Optimal: f1 = 2.5, f2 = 2.5, objective = 12.5  (both constraints binding)
    // ------------------------------------------------------------------

    private static (cLPSolver? solver, int iF1, int iF2, int iG1, int iG2, int iVal)
        BuildMseStyleProblem()
    {
        if (!IsSolverAvailable) return (null, 0, 0, 0, 0, 0);
        var solver = CreateSolver();

        int iF1 = 0, iF2 = 0, iG1 = 0, iG2 = 0, iVal = 0;

        solver.AddVariable("fleet1", ref iF1);
        solver.SetBounds(iF1, 0.0, 10.0);

        solver.AddVariable("fleet2", ref iF2);
        solver.SetBounds(iF2, 0.0, 10.0);

        // Group constraint rows: dMin = 0 so only LE constraints are added.
        solver.AddRow("group1", ref iG1);
        solver.SetCoefficient(iG1, iF1, 0.5);
        solver.SetCoefficient(iG1, iF2, 0.3);
        solver.SetBounds(iG1, 0.0, 2.0);

        solver.AddRow("group2", ref iG2);
        solver.SetCoefficient(iG2, iF1, 0.2);
        solver.SetCoefficient(iG2, iF2, 0.4);
        solver.SetBounds(iG2, 0.0, 1.5);

        // Goal (VALUE) – maximize total fishing value.
        solver.AddRow("VALUE", ref iVal);
        solver.SetCoefficient(iVal, iF1, 3.0);
        solver.SetCoefficient(iVal, iF2, 2.0);
        solver.SetBounds(iVal, 0.0, double.PositiveInfinity);
        solver.AddGoal(iVal, 1, bMinimize: false);

        return (solver, iF1, iF2, iG1, iG2, iVal);
    }

    [Fact]
    public void Solve_MseStyleMaximize_ReturnsOptimal()
    {
        // Arrange
        var (solver, _, _, _, _, _) = BuildMseStyleProblem();
        if (solver is null) return;

        // Act
        var result = solver.Solve(0);

        // Assert
        result.Should().Be(eSolverReturnValues.OPTIMAL);
    }

    [Fact]
    public void Solve_MseStyleMaximize_Fleet1_IsCorrect()
    {
        // Arrange
        var (solver, iF1, _, _, _, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iF1);

        // Assert
        value.Should().BeApproximately(2.5, precision: 1e-4);
    }

    [Fact]
    public void Solve_MseStyleMaximize_Fleet2_IsCorrect()
    {
        // Arrange
        var (solver, _, iF2, _, _, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iF2);

        // Assert
        value.Should().BeApproximately(2.5, precision: 1e-4);
    }

    [Fact]
    public void Solve_MseStyleMaximize_ObjectiveValue_IsCorrect()
    {
        // Arrange
        var (solver, _, _, _, _, iVal) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iVal);

        // Assert
        value.Should().BeApproximately(12.5, precision: 1e-4);
    }

    [Fact]
    public void Solve_MseStyleMaximize_Group1ConstraintValue_DoesNotExceedFTarget()
    {
        // Arrange
        var (solver, _, _, iG1, _, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iG1);

        // Assert
        // The row value (0.5·f1 + 0.3·f2) must stay within its LE bound.
        value.Should().BeLessOrEqualTo(2.0 + 1e-6);
    }

    [Fact]
    public void Solve_MseStyleMaximize_Group2ConstraintValue_DoesNotExceedFTarget()
    {
        // Arrange
        var (solver, _, _, _, iG2, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double value = solver.GetValue(iG2);

        // Assert
        value.Should().BeLessOrEqualTo(1.5 + 1e-6);
    }

    // ------------------------------------------------------------------
    // Dual (shadow) values
    //
    //   Both group constraints are binding at the optimum, so their dual
    //   values (shadow prices) must be strictly positive – relaxing the
    //   F-target would allow the objective to increase.
    // ------------------------------------------------------------------

    [Fact]
    public void GetDualValue_MseStyleMaximize_BindingGroup1Constraint_IsNonZero()
    {
        // Arrange
        var (solver, _, _, iG1, _, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double dual = solver.GetDualValue(iG1);

        // Assert
        Math.Abs(dual).Should().BeGreaterThan(1e-9,
            "the group-1 F-target constraint is binding and must have a non-zero shadow price");
    }

    [Fact]
    public void GetDualValue_MseStyleMaximize_BindingGroup2Constraint_IsNonZero()
    {
        // Arrange
        var (solver, _, _, _, iG2, _) = BuildMseStyleProblem();
        if (solver is null) return;
        solver.Solve(0);

        // Act
        double dual = solver.GetDualValue(iG2);

        // Assert
        Math.Abs(dual).Should().BeGreaterThan(1e-9,
            "the group-2 F-target constraint is binding and must have a non-zero shadow price");
    }

    // ------------------------------------------------------------------
    // Infeasible problem
    //
    //   Variable lower bounds force x + y ≥ 10, but the LE constraint caps
    //   x + y ≤ 3 → the solver must report INFEASIBLE.
    // ------------------------------------------------------------------

    [Fact]
    public void Solve_Infeasible_ReturnsInfeasible()
    {
        // Arrange
        if (!IsSolverAvailable) return;
        var solver = CreateSolver();

        int iX = 0, iY = 0, iCon = 0, iObj = 0;

        solver.AddVariable("x", ref iX);
        solver.SetBounds(iX, 5.0, 10.0);   // x ≥ 5

        solver.AddVariable("y", ref iY);
        solver.SetBounds(iY, 5.0, 10.0);   // y ≥ 5

        // x + y ≤ 3  →  contradicts x ≥ 5, y ≥ 5
        solver.AddRow("sum_con", ref iCon);
        solver.SetCoefficient(iCon, iX, 1.0);
        solver.SetCoefficient(iCon, iY, 1.0);
        solver.SetBounds(iCon, 0.0, 3.0);

        solver.AddRow("obj", ref iObj);
        solver.SetCoefficient(iObj, iX, 1.0);
        solver.SetCoefficient(iObj, iY, 1.0);
        solver.SetBounds(iObj, 0.0, double.PositiveInfinity);
        solver.AddGoal(iObj, 1, bMinimize: false);

        // Act
        var result = solver.Solve(0);

        // Assert
        result.Should().Be(eSolverReturnValues.INFEASIBLE);
    }

    // ------------------------------------------------------------------
    // Single-variable problem
    //
    //   Maximize: 5·z
    //   Subject to: 0 ≤ z ≤ 7
    //   Optimal: z = 7, objective = 35
    // ------------------------------------------------------------------

    [Fact]
    public void Solve_SingleVariable_ReturnsOptimalAtUpperBound()
    {
        // Arrange
        if (!IsSolverAvailable) return;
        var solver = CreateSolver();

        int iZ = 0, iObj = 0;
        solver.AddVariable("z", ref iZ);
        solver.SetBounds(iZ, 0.0, 7.0);

        solver.AddRow("obj", ref iObj);
        solver.SetCoefficient(iObj, iZ, 5.0);
        solver.SetBounds(iObj, 0.0, double.PositiveInfinity);
        solver.AddGoal(iObj, 1, bMinimize: false);

        // Act
        var result = solver.Solve(0);

        // Assert
        result.Should().Be(eSolverReturnValues.OPTIMAL);
        solver.GetValue(iZ).Should().BeApproximately(7.0, precision: 1e-6);
        solver.GetValue(iObj).Should().BeApproximately(35.0, precision: 1e-6);
    }
}
