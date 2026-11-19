// SPDX-License-Identifier: EUPL-1.2
// This file is part of Ecopath with Ecosim (EwE).

using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace EwECore.Tests.EcoSpace;

// Core metadata initializes globally; worker overlap is controlled explicitly within each test.
[Collection("Ecospace worker batches")]
public sealed class cEcoSpaceSolverBatchTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(15);

    [Theory]
    [InlineData("Grid", "Grid")]
    [InlineData("Space", "Space")]
    [InlineData("Contaminant", "Contaminant")]
    [InlineData("Space", "Contaminant")]
    [InlineData("Grid", "Space")]
    public async Task OverlappingSolverStages_DrainAndReportFailuresIndependently(string stageA, string stageB)
    {
        // Arrange
        using var coreA = new cCore();
        using var coreB = new cCore();
        using var started = new CountdownEvent(4);
        using var releaseA = new ManualResetEventSlim();
        using var releaseB = new ManualResetEventSlim();
        var spaceA = GetSpace(coreA);
        var spaceB = GetSpace(coreB);
        ConfigureFailingStage(spaceA, stageA, new BlockingLogger(started, releaseA));
        ConfigureFailingStage(spaceB, stageB, new BlockingLogger(started, releaseB));
        Task<Exception?>? runA = null;
        Task<Exception?>? runB = null;
        try
        {
            // Act
            runA = Task.Run<Exception?>(() => Record.Exception(() => RunStage(spaceA, stageA)));
            runB = Task.Run<Exception?>(() => Record.Exception(() => RunStage(spaceB, stageB)));
            Assert.True(started.Wait(TestTimeout), "All four solver workers must overlap.");
            releaseA.Set();
            var failureA = await runA.WaitAsync(TestTimeout);

            // Assert
            AssertWorkerFailures(failureA, 2);
            Assert.False(runB.IsCompleted);
            AssertPartitions(spaceA, stageA);
            AssertPartitions(spaceB, stageB);
            Assert.Equal(17f, spaceA.EcoSpaceData.ResultsByFleet[0, 0, 0]);
            releaseB.Set();
            AssertWorkerFailures(await runB.WaitAsync(TestTimeout), 2);
            Assert.Equal(17f, spaceB.EcoSpaceData.ResultsByFleet[0, 0, 0]);
        }
        finally
        {
            releaseA.Set();
            releaseB.Set();
            if (runA is not null) await runA.WaitAsync(TestTimeout);
            if (runB is not null) await runB.WaitAsync(TestTimeout);
        }
    }

    [Theory]
    [InlineData("Grid")]
    [InlineData("Space")]
    [InlineData("Contaminant")]
    public async Task SetupFailure_DrainsPreviouslyQueuedSolver(string stage)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        using var started = new CountdownEvent(1);
        using var release = new ManualResetEventSlim();
        ConfigureFailingStage(space, stage, new BlockingLogger(started, release));
        if (stage == "Grid")
            GetField<List<cGridSolver>>(space, "m_gridSolvers")[1] = null!;
        else
            GetField<List<cSpaceSolver>>(space, "m_spaceSolvers")[1] = null!;
        Task<Exception?>? running = null;
        try
        {
            // Act
            running = Task.Run<Exception?>(() => Record.Exception(() => RunStage(space, stage)));
            Assert.True(started.Wait(TestTimeout));

            // Assert
            Assert.False(running.IsCompleted, "Caller cleanup must wait for the already queued solver.");
            release.Set();
            var failure = Assert.IsType<ApplicationException>(await running.WaitAsync(TestTimeout));
            Assert.IsType<NullReferenceException>(failure.InnerException);
        }
        finally
        {
            release.Set();
            if (running is not null) await running.WaitAsync(TestTimeout);
        }
    }

    [Theory]
    [InlineData(false, true, false)]
    [InlineData(false, false, false)]
    [InlineData(true, false, false)]
    [InlineData(false, true, true)]
    [InlineData(false, false, true)]
    [InlineData(true, false, true)]
    public async Task GridStage_PreservesSolutionOrReportsCalculationFailure(bool localMemory, bool exact, bool failCalculation)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        var solver = new cGridSolver(1) { bUseLocalMemory = localMemory };
        var biomass = new float[3, 3, 2];
        var loss = new float[3, 3, 2];
        var forcing = new float[3, 3, 2];
        loss[1, 1, 1] = -2f;
        forcing[1, 1, 1] = 8f;
        SetField(solver, "X", biomass);
        SetField(solver, "Aloc", failCalculation ? null! : loss);
        SetField(solver, "Floc", forcing);
        foreach (string field in new[] { "Bcw", "C", "d", "e" })
            SetField(solver, field, new float[3, 3, 2]);
        SetField(solver, "M", 1);
        SetField(solver, "NomCols", 1);
        var depth = new float[3, 3];
        depth[1, 1] = 1f;
        SetField(solver, "Depth", depth);
        SetField(solver, "W", 1f);
        SetField(solver, "Tol", 0.0001f);
        SetField(solver, "timeStep", 1f);
        SetField(solver, "maxIter", 10);
        foreach (string field in new[] { "iStartRow", "iEndRow", "jStartCol", "jEndCol", "jord" })
            SetField(solver, field, new[] { 0, 1 });
        SetField(solver, "threadGroups", new[,] { { 0, 0 }, { 0, 1 } });
        SetField(solver, "ByPassIntegrate", new bool[2]);
        SetField(solver, "useExact", exact);
        SetField(space, "m_gridSolvers", new List<cGridSolver> { solver });
        SetField(space, "nGroupsInThread", new[] { 0, 1 });

        // Act
        var failure = await Task.Run<Exception?>(() => Record.Exception(() => RunStage(space, "Grid"))).WaitAsync(TestTimeout);

        // Assert
        if (failCalculation)
        {
            AssertWorkerFailures(failure, 1);
            Assert.Equal(0f, biomass[1, 1, 1]);
        }
        else
        {
            Assert.Null(failure);
            Assert.Equal(4f, biomass[1, 1, 1]);
        }
        Assert.Equal(1, solver.nGroupsComputed);
        Assert.True(solver.CPUTime >= 0);
    }

    [Theory]
    [InlineData("Grid")]
    [InlineData("Space")]
    [InlineData("Contaminant")]
    public async Task EmptySolverStage_Completes(string stage)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        InitializeStageData(space);
        SetField(space, "m_gridSolvers", new List<cGridSolver>());
        SetField(space, "m_spaceSolvers", new List<cSpaceSolver>());
        space.EcoSpaceData.iTotalWaterCells = 0;
        space.EcoSpaceData.nSpaceSolverThreads = 0;

        // Act
        var failure = await Task.Run<Exception?>(() => Record.Exception(() => RunStage(space, stage))).WaitAsync(TestTimeout);

        // Assert
        Assert.Null(failure);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Contaminant")]
    public async Task WorkerSetupFailure_IsCollectedByBatch(string stage)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        InitializeStageData(space);
        space.EcoSpaceData.nSpaceSolverThreads = 1;
        var solver = new cSpaceSolver(1);
        if (stage == "Space") solver.m_TracerData = null!;
        SetField(space, "m_spaceSolvers", new List<cSpaceSolver> { solver });

        // Act
        var failure = await Task.Run<Exception?>(() => Record.Exception(() => RunStage(space, stage))).WaitAsync(TestTimeout);

        // Assert
        AssertWorkerFailures(failure, 1);
        Assert.Equal(17f, space.EcoSpaceData.ResultsByFleet[0, 0, 0]);
    }

    [Theory]
    [InlineData("Space")]
    [InlineData("Contaminant")]
    public async Task SuccessfulSpaceStage_FinalizesWorkersBeforeAggregatingResults(string stage)
    {
        // Arrange
        using var core = new cCore();
        var space = GetSpace(core);
        InitializeStageData(space);
        space.EcoSpaceData.iTotalWaterCells = 0;
        int effort = (int)eSpaceResultsFleets.FishingEffort;
        space.EcoSpaceData.ResultsByFleet[effort, 0, 0] = 17f;
        var solvers = new List<cSpaceSolver>();
        for (int i = 1; i <= 2; i++)
        {
            var solver = new cSpaceSolver(i)
            {
                m_Data = space.EcoSpaceData,
                m_PathData = space.EcoPathData,
                Landings = new float[1, 1],
                ResultsByGroup = new float[Enum.GetValues<eSpaceResultsGroups>().Length, 1],
                ResultsByFleet = new float[Enum.GetValues<eSpaceResultsFleets>().Length, 1],
                ResultsByFleetGroup = new float[Enum.GetValues<eSpaceResultsFleetsGroups>().Length, 1, 1],
                ResultsCatchRegionGearGroup = new float[1, 1, 1],
                ResultsLandingsRegionGearGroup = new float[1, 1, 1],
                ResultsValueRegionGearGroup = new float[1, 1, 1],
                ResultsConsumptionRegionPredPrey = new float[1, 1, 1],
                TotLossThread = new float[1],
                TotEatenByThread = new float[1],
                TotBiomThread = new float[1],
                TotPredThread = new float[1],
                TotIFDweightThread = new float[1]
            };
            solver.ResultsByFleet[effort, 0] = i;
            SetField(solver, "m_stpWatch", new Stopwatch());
            solvers.Add(solver);
        }
        SetField(space, "m_spaceSolvers", solvers);

        // Act
        await Task.Run(() => RunStage(space, stage)).WaitAsync(TestTimeout);

        // Assert
        Assert.Equal(stage == "Space" ? 17f : 20f, space.EcoSpaceData.ResultsByFleet[effort, 0, 0]);
        Assert.All(solvers, solver =>
        {
            Assert.False(GetField<Stopwatch>(solver, "m_stpWatch").IsRunning);
            Assert.Equal(1, solver.iFrstCell);
            Assert.Equal(0, solver.iLstCell);
            Assert.Equal(stage == "Space" ? 0f : solver.ThreadID, solver.ResultsByFleet[effort, 0]);
        });
    }

    private static void ConfigureFailingStage(cEcoSpace space, string stage, ILogger logger)
    {
        InitializeStageData(space);
        if (stage == "Grid")
        {
            var solvers = new List<cGridSolver> { new(1), new(2) };
            foreach (var solver in solvers) SetField(solver, "m_logger", logger);
            SetField(space, "m_gridSolvers", solvers);
            SetField(space, "nGroupsInThread", new[] { 0, 1, 1 });
        }
        else
        {
            var solvers = new List<cSpaceSolver> { new(1), new(2) };
            foreach (var solver in solvers)
            {
                solver.m_Data = space.EcoSpaceData;
                solver.m_PathData = space.EcoPathData;
                SetField(solver, "m_stpWatch", new Stopwatch());
                SetField(solver, "m_logger", logger);
            }
            SetField(space, "m_spaceSolvers", solvers);
        }
    }

    private static void InitializeStageData(cEcoSpace space)
    {
        foreach (string field in new[] { "Btime", "TotLoss", "TotEatenBy", "TotBiom", "TotPred", "TotIFDweight" })
            SetField(space, field, new float[1]);
        space.EcoSpaceData.iTotalWaterCells = 5;
        space.EcoSpaceData.nSpaceSolverThreads = 2;
        space.EcoSpaceData.nWaterCells = 1;
        space.EcoSpaceData.ResultsByFleet = new float[Enum.GetValues<eSpaceResultsFleets>().Length, 1, 1];
        space.EcoSpaceData.ResultsByFleet[0, 0, 0] = 17f;
    }

    private static void AssertPartitions(cEcoSpace space, string stage)
    {
        if (stage == "Grid")
        {
            Assert.All(GetField<List<cGridSolver>>(space, "m_gridSolvers"), solver =>
            {
                Assert.Equal(1, solver.iFirstIndex);
                Assert.Equal(1, solver.nGroupsComputed);
            });
        }
        else
        {
            var solvers = GetField<List<cSpaceSolver>>(space, "m_spaceSolvers");
            Assert.Equal(1, solvers[0].iFrstCell);
            Assert.Equal(solvers[0].iLstCell + 1, solvers[1].iFrstCell);
            Assert.Equal(5, solvers[1].iLstCell);
        }
    }

    private static void AssertWorkerFailures(Exception? failure, int count)
    {
        var callerFailure = Assert.IsType<ApplicationException>(failure);
        var workerFailures = Assert.IsType<AggregateException>(callerFailure.InnerException);
        Assert.Equal(count, workerFailures.InnerExceptions.Count);
        Assert.All(workerFailures.InnerExceptions, error => Assert.IsType<NullReferenceException>(error));
    }

    private static void RunStage(cEcoSpace space, string stage)
    {
        if (stage == "Contaminant")
        {
            typeof(cEcoSpace).GetMethod("runSpaceCSolverThreads", BindingFlags.Instance | BindingFlags.NonPublic)!
                .CreateDelegate<Action<float>>(space)(2f);
        }
        else
        {
            string name = stage == "Grid" ? "runGridSolverThreads" : "runSpaceSolverThreads";
            typeof(cEcoSpace).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!
                .CreateDelegate<Action>(space)();
        }
    }

    private static cEcoSpace GetSpace(cCore core) => GetField<cEcoSpace>(core, "m_Ecospace");
    private static T GetField<T>(object target, string name) =>
        (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(target)!;
    private static void SetField(object target, string name, object value) =>
        target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.SetValue(target, value);

    // Hold actual solver workers at their error boundary to force overlapping production batches.
    private sealed class BlockingLogger(CountdownEvent started, ManualResetEventSlim release) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            started.Signal();
            if (!release.Wait(TestTimeout)) throw new TimeoutException("Test solver was not released.");
        }
    }
}
