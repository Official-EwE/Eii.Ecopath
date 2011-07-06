
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

        Sub Init(ByVal theCore As Object)

        Sub StartWrite()

        Sub EndWrite()


    End Interface

End Namespace
