

''' <summary>
''' Interface for MSE initialization plugin points that are invoked once the MSE model has been loaded
''' </summary>
''' <remarks></remarks>
Public Interface IMSEInitialized
    Inherits IPlugin

    ''' <summary>
    ''' MSE model has been initialized
    ''' </summary>
    ''' <param name="MSEModel">MSE model</param>
    ''' <param name="MSEDataStructure">MSE data structures</param>
    ''' <param name="EcosimDatastructures">Ecosim data structures</param>
    ''' <remarks></remarks>
    Sub MSEInitialized(ByVal MSEModel As Object, ByVal MSEDataStructure As Object, ByVal EcosimDatastructures As Object)

End Interface


