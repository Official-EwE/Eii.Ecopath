
Namespace Core

    ''' <summary>
    ''' Interface for writing Ecospace time step results to file
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IEcospaceResultsWriter

        ''' <summary>
        ''' Save time step data to file
        ''' </summary>
        ''' <param name="SpaceTimeStepResults">cEcospaceTimestep as object containing the data to save.</param>
        ''' <remarks></remarks>
        Sub WriteResults(ByVal SpaceTimeStepResults As Object)

        ''' <summary>
        ''' Init to the current cCore
        ''' </summary>
        ''' <param name="theCore"></param>
        ''' <remarks></remarks>
        Sub Init(ByVal theCore As Object)

        ''' <summary>
        ''' Called when as Ecospace model run is about to start
        ''' </summary>
        ''' <remarks>This can be used to initialized and file data at the start of a run</remarks>
        Sub StartWrite()

        ''' <summary>
        ''' Called at the end of an Ecospace model run
        ''' </summary>
        ''' <remarks>Cleanup after an Ecospace run has completed</remarks>
        Sub EndWrite()


    End Interface

End Namespace
