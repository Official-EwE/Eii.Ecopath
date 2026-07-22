# MSE Technical Architecture

Management Strategy Evaluation (MSE) — also referred to as the *Closed Loop Simulator* in EwE5 — is a stochastic Monte-Carlo framework that wraps the Ecosim simulation engine. It repeatedly runs Ecosim through a set of *trials*, each time perturbing catchability and biomass estimates with configurable noise, applies a regulatory model (quota/effort controls), and collects statistics across all trials.

---

## High-level component overview

```mermaid
graph TD
    UI["ScientificInterface<br>(MSE forms & grids)"]
    Manager["cMSEManager<br>(Input Output layer)"]
    Core["cCore<br>(EwECore)"]
    MSE["cMSE<br>(Simulation engine)"]
    Data["cMSEDataStructures<br>(State & parameters)"]
    Ecosim["cEcosimModel<br>(Ecosim engine)"]
    Batch["cMSEBatchManager<br>(Batch analysis)"]
    Plugin["cPluginManager<br>(Economic plug-in)"]
    OutputCSV["cMSECSVOutputWriter<br>(CSV output)"]
    OutputBatch["cMSEBatchOutputWriter<br>(Batch output)"]
    LPSolver["cLPSolver<br>(Linear programming)"]

    UI -->|"RunMSE / RunMSY"| Manager
    Manager -->|"Init / Connect / Run"| MSE
    Manager -->|"exposes I/O objects"| Core
    MSE -->|"reads/writes"| Data
    MSE -->|"Run / SetFtimeFromGear"| Ecosim
    MSE -->|"onEcosimTimestep callback"| Ecosim
    MSE -->|"batch mode"| Batch
    MSE -->|"PostRunSearchResults"| Plugin
    MSE -->|"saveIteration"| OutputCSV
    Batch -->|"saveIteration"| OutputBatch
    MSE -->|"RegulateLPEffort"| LPSolver
    Core -->|"owns"| Manager
```

---

## Package / namespace map

| Namespace / folder | Key classes | Responsibility |
|---|---|---|
| `EwECore.MSE` (Model) | `cMSE` | Core closed-loop simulation loop, assessment, regulations |
| `EwECore.MSE` (Model) | `cMSEDataStructures` | All run-time state arrays (biomass, effort, quota, statistics) |
| `EwECore.MSE` (Model) | `cMSECSVOutputWriter` | Writes per-iteration CSV files to the autosave folder |
| `EwECore.MSE` (Input Output) | `cMSEManager` | Orchestrates threading, exposes I/O objects to the UI layer |
| `EwECore.MSE` (Input Output) | `cMSEParameters` | Scalar run parameters (assessment method, n-trials, effort mode …) |
| `EwECore.MSE` (Input Output) | `cMSEGroupInput` | Per-group inputs (biomass CV, reference levels, HCR parameters) |
| `EwECore.MSE` (Input Output) | `cMSEFleetInput` | Per-fleet inputs (effort CV, quota type, LP bounds, quota shares) |
| `EwECore.MSE` (Input Output) | `cMSEOutput` | Exposes statistics back to the UI |
| `EwECore.MSEBatchManager` | `cMSEBatchManager` | **Not used anymore.** Runs a sequence of MSE configurations from a command file |
| `EwECore.MSEBatchManager` | `cMSEBatchDataStructures` | Batch-specific state (TFM groups, TAC groups, FixedF groups) |
| `EwECore.MSEBatchManager` | `cMSEBatchOutputWriter` | Output writer used in batch mode |
| `EwECore.MSECommandFile` | `cMSECommandFileReader` | Parses the batch command file |

---

## Regulation modes and effort sources

```mermaid
flowchart LR
    subgraph RegulationMode
        NR["NoRegulations<br>(Ecosim-driven effort)"]
        UR["UseRegulations<br>(Quota / HCR controls)"]
    end

    subgraph EffortSource["Effort Source (when UseRegulations)"]
        ES["EcosimEffort<br>(track Ecosim scenario)"]
        NC["NoCap<br>(effort set to MSEMaxEffort)"]
        PR["Predicted<br>(Ecosim PredictSimEffort)"]
    end

    UR --> ES
    UR --> NC
    UR --> PR
```

When `UseRegulations` is active, a per-fleet `QuotaType` determines how effort is constrained each year:

| `eQuotaTypes` value | Behaviour |
|---|---|
| `NoControls` | Fleet is unregulated |
| `Weakest` | Effort capped to protect the most vulnerable stock |
| `HighestValue` | Effort maximises highest-value stock |
| `Selective` | Per-group selectivity quota |
| `Effort` | Direct effort cap (not yet fully implemented) |

---

## Run lifecycle

```mermaid
sequenceDiagram
    participant UI
    participant Manager as cMSEManager
    participant MSE as cMSE
    participant Ecosim as cEcosimModel

    UI->>Manager: RunMSE() [background thread]
    Manager->>MSE: Init(…)
    Manager->>MSE: Connect(MSECallback, MSYCallback)
    Manager->>MSE: InitForRun()
    note over MSE: SetBaseValues()<br/>setBestTotalValue()<br/>InitLPSolver()
    MSE->>Ecosim: Run() [base run for value baseline]

    loop N Trials
        MSE->>MSE: InitForTrial()
        note over MSE: InitAssessment()<br/>reset FisForced()
        MSE->>Ecosim: Run()
        note over Ecosim: calls onEcosimTimestep<br/>each month
        loop Each timestep (month)
            Ecosim-->>MSE: onEcosimTimestep(t, month, year)
            MSE->>MSE: DoRegulations(Biomass, Effort, …)
            alt month == 1
                MSE->>MSE: DoAssessment(Biomass)
                MSE->>MSE: UpdateQuotas(Biomass)
            end
            MSE->>MSE: RegulateEffort / setFishTime
            MSE->>Ecosim: SetFtimeFromGear(t, QYear)
        end
        MSE->>MSE: summarizeEcosimEconomicData()
        MSE->>MSE: SaveIteration()
        MSE->>MSE: PostPluginData()
        MSE->>MSE: SumValues()
        MSE->>MSE: resetEffort()
        MSE-->>Manager: MSEProgressDelegate(IterationCompleted)
    end

    MSE->>MSE: ComputeStats()
    MSE->>MSE: FinalizeRun()
    MSE-->>Manager: MSEProgressDelegate(RunCompleted)
    Manager-->>UI: notify
```

---

## Per-timestep regulation decision tree

```mermaid
flowchart TD
    A[Ecosim calls onEcosimTimestep] --> B{UseQuotaRegs?}
    B -- No --> C[setFishTime<br/>use timeseries / FishYear]
    B -- Yes --> D{isTStepRegulated?}
    D -- No --> C
    D -- Yes --> E{month == 1?}
    E -- Yes --> F[DoAssessment<br/>UpdateQuotas]
    F --> G[RegulateEffort]
    E -- No --> G
    G --> H{UseLPSolution?}
    H -- Yes --> I[RegulateLPEffort<br/>via cLPSolver]
    H -- No --> J{QuotaType per fleet}
    J --> K["Weakest / HighestValue /<br/>Selective / NoControls"]
    I --> L[SetFtimeFromGear]
    K --> L
    C --> L
    L --> M[Ecosim continues integration]
```

---

## Stock assessment methods

Three methods are available (`eAssessmentMethods`), selected per run:

```mermaid
flowchart LR
    A[AssessFs called<br/>end of simulated year] --> B{AssessMethod}
    B --> C["Exact<br/>Biomass & catch known exactly<br/>Fest = Catch / Bbar"]
    B --> D["CatchEstmBio<br/>Biomass estimated with log-normal noise<br/>+ stock-recruitment model<br/>+ Kalman filter update"]
    B --> E["DirectExploitation<br/>Exploitation rate with CV noise<br/>Fest = Catch/Bbar × exp(N(0,CVFest))"]
```

The `CatchEstmBio` method uses a Beverton–Holt stock-recruitment model and a Kalman filter (`KalmanGain`, `GstockPred`, `RStock0`) to propagate the biomass estimate through time.

---

## Statistics collection

After all trials, `cMSEDataStructures` holds several `cMSESummaryStats` instances that accumulate values across trials and years:

| Statistics object | Dimension | Content |
|---|---|---|
| `BioStats` | groups × years | Biomass |
| `CatchGroupStats` | groups × years | Catch by group |
| `CatchFleetStats` | fleets × years | Catch by fleet |
| `EffortStats` | fleets × years | Fishing effort |
| `BioEstStats` | groups × years | Estimated biomass |
| `FLPDualValue` | fleets × years | LP dual (shadow) values |
| `ValueFleetStats` | fleets × years | Economic value per fleet |
| `ProfitSum` / `CostSum` / `JobsSum` | fleets | Economic summary |

`ComputeStats()` is called once after all trials to derive mean, standard deviation, CV, histograms, and percentage above/below reference levels.

---

## Batch analysis

```mermaid
graph TD
    BM["cMSEBatchManager"] -->|"reads"| CF["Command file<br>(cMSECommandFileReader)"]
    BM -->|"configures"| MSE["cMSE"]
    BM -->|"iterates run types"| RT["eMSEBatchRunTypes<br>FixedF | TAC | TFM | NotManaged"]
    BM -->|"writes"| BOW["cMSEBatchOutputWriter<br/><br>(implements IMSEOutputWriter)"]
    BM -->|"varies forcing"| FM["ForcingMultTime()<br>(environmental scenarios)"]
```

The batch manager reads a command file that defines sets of FixedF, TAC, and TFM fishing scenarios. For each configuration it seeds `cMSEDataStructures`, calls `cMSE.Run()`, and routes output through `cMSEBatchOutputWriter` instead of the default CSV writer.

---

## Output writers

The `IMSEOutputWriter` interface decouples the simulation from persistence:

```mermaid
classDiagram
    class IMSEOutputWriter {
        <<interface>>
        +Init()
        +saveIteration(data)
    }
    class cMSECSVOutputWriter {
        +DataDir : String
        +getOutputFileName(type, name) String
    }
    class cMSEBatchOutputWriter {
    }
    IMSEOutputWriter <|.. cMSECSVOutputWriter
    IMSEOutputWriter <|.. cMSEBatchOutputWriter
```

The active writer is selected by `OutputWriterFactory()` in `cMSE`: batch runs use `cMSEBatchOutputWriter`; interactive runs use `cMSECSVOutputWriter` which writes one CSV file per group/fleet to the configured autosave folder.

Output file prefixes (constants on `cMSE`):

| Constant | File prefix |
|---|---|
| `BIOMASS_DATA` | `MSE_Biomass` |
| `CATCH_DATA` | `MSE_CatchByGroup` |
| `EFFORT_DATA` | `MSE_Effort` |
| `FLEETCATCH_DATA` | `MSE_CatchByFleet` |
| `QUOTAGROUP_DATA` | `MSE_QuotaByGroup` |

---

## Randomness and reproducibility

- In **interactive** mode, the random seed is derived from `Date.Now.Ticks`, giving a different stochastic sequence every run.
- In **batch** mode, the seed is fixed to `42`, ensuring reproducible results across batch iterations.
- A single `System.Random` instance (`m_rndGen`) is created at the start of each run and used throughout for biomass noise (`CVbiomEst`), effort noise (`VarQest`), recruitment noise (`cvRec`), and catchability growth noise (`VarQgrow`).

---

## Key data flows

```mermaid
flowchart LR
    subgraph Inputs
        EP[Ecopath base parameters]
        ES["Ecosim scenario<br/>(effort, time-series)"]
        GI["cMSEGroupInput<br/>(CV, HCR ref levels)"]
        FI["cMSEFleetInput<br/>(quota type, weights)"]
        PM["cMSEParameters<br/>(nTrials, assessment method)"]
    end

    subgraph MSE Core
        DS[cMSEDataStructures]
        SIM[cMSE.Run loop]
    end

    subgraph Outputs
        CSV[CSV files per group/fleet]
        STATS["Summary statistics<br/>(mean, CV, histogram)"]
        UI2["UI output objects<br/>(cMSEOutput)"]
    end

    EP --> DS
    ES --> DS
    GI --> DS
    FI --> DS
    PM --> DS
    DS --> SIM
    SIM --> CSV
    SIM --> STATS
    STATS --> UI2
```

---

## Linear programming with lpsolve55.dll

When `UseLPSolution = True` (the default when `UseRegulations` is active), the MSE replaces the simple rule-based effort cap with a full linear program that maximises the total landed value across all fleets subject to per-group fishing-mortality ceilings.

### Native library integration

`lpsolve55.dll` is an unmanaged C library (lp_solve 5.5). EwECore ships two copies of it side-by-side with the managed assembly:

```
Sources/EwECore/Includes/LPSolve/win32/lpsolve55.dll   (32-bit)
Sources/EwECore/Includes/LPSolve/win64/lpsolve55.dll   (64-bit)
```

The inner class `cLPSolver.lpsolve55` declares every P/Invoke entry point with `Declare Function - Lib "lpsolve55.dll"`. At startup, `lpsolve55.Init()` calls `SetDllDirectoryA` (kernel32) to point the loader at the correct architecture subfolder, then verifies the DLL exists before setting an `IsUsable` flag. The solver is therefore **Windows-only**; `IsSupported()` returns `False` on any other platform.

```mermaid
flowchart LR
    A["cLPSolver.IsSupported()"] --> B{Windows?}
    B -- No --> C["return False<br>(LP disabled)"]
    B -- Yes --> D["lpsolve55.Init()<br>SetDllDirectoryA(win32|win64)"]
    D --> E{DLL found?}
    E -- No --> C
    E -- Yes --> F["g_bUsable = True<br>return True"]
```

### Class structure

```mermaid
classDiagram
    class ILPSolver {
        <<interface>>
        +AddVariable(key, iVar) bool
        +AddRow(key, iRow) bool
        +SetBounds(iVar, min, max)
        +SetCoefficient(iRow, iVar, val)
        +AddGoal(iRow, priority, minimize) bool
        +Solve(timeStepIndex) eSolverReturnValues
        +GetValue(iItem) double
        +GetDualValue(iItem) double
        +IsSupported() bool
    }
    class cLPSolver {
        -m_lDefs : List~cDef~
        -m_iGoal : int
        -m_bMinimize : bool
        +Solve(t) eSolverReturnValues
    }
    class lpsolve55 {
        <<static P/Invoke wrapper>>
        +Init()
        +IsUsable() bool
        +make_lp(rows, cols) int
        +add_constraint(lp, row, type, rh) bool
        +set_obj_fn(lp, row) bool
        +set_maxim(lp)
        +set_minim(lp)
        +solve(lp) lpsolve_return
        +get_primal_solution(lp, pv)
        +get_dual_solution(lp, rc)
        +delete_lp(lp)
    }
    ILPSolver <|.. cLPSolver
    cLPSolver *-- lpsolve55 : nested class
```

`cLPSolver` builds an in-memory model (variables and constraint rows stored as `cVarDef` / `cRowDef` objects), then on each call to `Solve()`:
1. Creates a fresh unmanaged LP model via `make_lp`.
2. Sets variable bounds and row coefficients.
3. Runs the solver.
4. Extracts primal and dual solutions.
5. Destroys the unmanaged model with `delete_lp`.

The unmanaged model is **recreated from scratch every timestep** rather than being warm-started.

### How MSE uses the LP solver

`InitLPSolver()` is called once during `InitForRun()` and builds the persistent `cLPSolver` instance:

| LP element | Maps to | Bound |
|---|---|---|
| **Variable** per fleet | `FishRateGear(fleet, t)` - the decision variable | `[LowLPEffort(fleet), UpperLPEffort(fleet)]` |
| **Constraint row** per living group | Total fishing mortality on that group | `[0, FTarget(group)]` |
| **Goal row** ("VALUE") | Maximise total landed value | Unbounded above |

`RegulateLPEffort()` is called at the **first month of each regulated year** and repopulates the LP with current-year data before solving:

```mermaid
sequenceDiagram
    participant MSE as cMSE
    participant LP as cLPSolver
    participant DLL as lpsolve55.dll

    MSE->>MSE: compute Qest(group, fleet)<br>Kalman-filter catchability update
    MSE->>MSE: compute QStar = Qest x mortality-selectivity factor
    MSE->>MSE: compute VPerEffort(fleet)<br>= sum_group QStar x Biomass x MarketPrice x PropLanded

    loop each living group
        MSE->>LP: SetCoefficient(groupRow, fleetVar, QStar)
        MSE->>LP: SetBounds(groupRow, 0, FTarget)
    end
    loop each fleet
        MSE->>LP: SetCoefficient(goalRow, fleetVar, VPerEffort)
        MSE->>LP: SetBounds(fleetVar, LowLPEffort, UpperLPEffort)
    end

    MSE->>LP: Solve(t)
    LP->>DLL: make_lp / set_bounds / add_constraint / set_obj_fn / set_maxim / solve
    DLL-->>LP: lpsolve_return + primal + dual solution
    LP-->>MSE: eSolverReturnValues

    alt OPTIMAL
        MSE->>Ecosim: FishRateGear(fleet, t) = GetValue(fleetVar)
    else non-optimal
        MSE->>MSE: record t in lstNonOptSolutions
        MSE->>Ecosim: FishRateGear(fleet, t) = FishRateGear(fleet, t-1)
    end

    loop each living group, months t to t+11
        MSE->>Data: FLPDualValue.AddValue(group, month, |DualValue|)
    end
```

### The LP problem formulation

```
Maximise:   sum_f  VPerEffort(f) * E(f)

Subject to:
  For each living group g:
    sum_f  QStar(g, f) * E(f)  <=  FTarget(g)       [F-cap per group]

  For each fleet f:
    LowLPEffort(f) <= E(f) <= UpperLPEffort(f)        [effort bounds]
```

| Symbol | Description |
|---|---|
| `E(f)` | Effort for fleet `f` - the decision variable |
| `VPerEffort(f)` | Value density: `sum_g QStar(g,f) * B(g) * Market(f,g) * PropLanded(f,g)` |
| `QStar(g,f)` | Effective catchability: `Qest(g,f) * (PropLanded + (1-PropLanded) * PropDiscardMort)` |
| `FTarget(g)` | Target fishing-mortality ceiling for group `g` (from `cMSEDataStructures`) |
| `Qest(g,f)` | Kalman-filtered catchability estimate, updated each year |

### Dual values (shadow prices)

After each solve, `GetDualValue()` is called for every group row. The absolute value of the dual is stored in `FLPDualValue` for all 12 months of the regulated year and exposed in the statistics output. These shadow prices indicate how much the objective (total landed value) would increase if the F-cap for a given group were relaxed by one unit - identifying which groups are the **binding constraints** on fleet value.

### Solver return values and fallback behaviour

| `eSolverReturnValues` | Meaning | MSE action |
|---|---|---|
| `OPTIMAL` | Unique optimal solution found | Apply solved effort to `FishRateGear(fleet, t)` |
| `SUBOPTIMAL` | Feasible but not proven optimal | Timestep added to `lstNonOptSolutions`; previous-timestep effort reused |
| `INFEASIBLE` | No feasible solution exists | Same fallback |
| `UNBOUNDED` | Objective is unbounded | Same fallback |
| `NUMFAILURE`, `DEGENERATE` | Numerical problems | Same fallback |
| `ERROR` | DLL not available or exception | Same fallback |

In debug builds, any non-optimal result causes the full LP model to be written to a `.txt` file in the system temp folder (`EWE6_LPSolve_model_<t>.txt`) for diagnosis.

### Platform constraints

Because `lpsolve55.dll` uses VB6-style `Declare Function - Lib` P/Invoke, the LP solver is **only functional on Windows**. On non-Windows platforms `IsSupported()` returns `False` and `Solve()` returns `ERROR` immediately, causing the MSE to fall back to previous-timestep effort for every regulated year. All other MSE functionality (rule-based regulation, stock assessment, statistics) continues to work normally.
