
''' <summary>
''' Interface for implementing MSY search plugin points that are invoked from the EwE core.
''' </summary>
''' <remarks></remarks>
Public Interface IMSYPlugin
    Inherits ICorePlugin

    ''' <summary>
    ''' MSY has been initialized
    ''' </summary>
    ''' <param name="MSEDataStructure">MSE data structures</param>
    ''' <param name="QuotaDataStructures">Quota data structures</param>
    ''' <param name="EcosimDatastructures">Ecosim data structures</param>
    ''' <remarks></remarks>
    Sub MSYInitialized(ByVal MSEDataStructure As Object, ByVal QuotaDataStructures As Object, ByVal EcosimDatastructures As Object)

    ''' <summary>
    ''' The MSY variables have been initialized and search is about to start.
    ''' </summary>
    ''' <param name="MSEDataStructure"></param>
    ''' <param name="QuotaDataStructures"></param>
    ''' <param name="EcosimDatastructures"></param>
    ''' <remarks></remarks>
    Sub MSYRunStarted(ByVal MSEDataStructure As Object, ByVal QuotaDataStructures As Object, ByVal EcosimDatastructures As Object)

    ''' <summary>
    ''' MSY search has completed all its iteration and computed effort for all fleets. Interface objects have not been populated at this time.
    ''' </summary>
    ''' <param name="MSYEffortByFleet">MSY effort for all fleets</param>
    ''' <remarks></remarks>
    Sub MSYEffortCompleted(ByVal MSYEffortByFleet() As Single)

    ''' <summary>
    ''' MSY search is completed all iterface object have been populated.
    ''' </summary>
    ''' <remarks></remarks>
    Sub MSYRunCompleted()


End Interface
