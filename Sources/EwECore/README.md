# Eii.Ecopath.EwECore

The Core functionality for Ecopath with Ecosim (EwE)

https://ecopath.org/

## Ecospace worker batches

Habitat-capacity, effort-distribution, fishing-mortality, grid-solver, space-solver,
and contaminant space-solver workers use independent batches. Completion tracking,
worker errors, and load-shared fleet allocation are owned by each batch, not shared
between cores. Solver results are consumed only after all workers succeed. Grid and
space solver batches retain their existing unbounded waits, and already-started
workers are drained even if setup or scheduling a later worker fails.

Worker failures are logged and propagated to the caller after all workers in the
batch have finished. The habitat batch retains its five-minute timeout threshold:
a timeout is logged immediately, but the caller waits for remaining workers before
throwing `TimeoutException`. This prevents subsequent model processing or cleanup
from racing with workers still accessing model arrays. A permanently stuck worker
therefore still blocks the caller; the timeout does not cancel workers. This does
not make concurrent operations on the same core supported.

Regression tests: `dotnet test .\Sources\EwECore.Tests\EwECore.Tests.csproj --filter "FullyQualifiedName~cEcoSpaceWorkerBatchTests|FullyQualifiedName~cEcoSpaceSolverBatchTests"`


dd