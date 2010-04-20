

''' <summary>
''' Plugin points for the initialization of Ecosim data cEcosimDataStructures 
''' </summary>
''' <remarks></remarks>
Public Interface IEcosimDataInitializedPlugin
    Inherits IPlugin

    ''' <summary>
    ''' Ecosim has loaded a scenario and is about to call cEcosimModel.Init() to initialize its data.
    ''' </summary>
    ''' <param name="EcosimDatastructures">Ecosim datastructures as an object</param>
    ''' <remarks>This can be used prior to the initialization of Ecosim data to set variables that are used by Ecosim to set derived variables. </remarks>
    Sub EcosimPreDataInitialized(ByVal EcosimDatastructures As Object)

    ''' <summary>
    ''' Ecosim has loaded a scenario and has completed its call to cEcosimModel.Init().
    ''' </summary>
    ''' <param name="EcosimDatastructures">cEcosimDataStructers as an object</param>
    ''' <remarks>Call after data has been initialized but before run initialization</remarks>
    Sub EcosimPostDataInitialized(ByVal EcosimDatastructures As Object)


    ''' <summary>
    ''' Ecosim is about to initailize for a run
    ''' </summary>
    ''' <param name="EcosimDatastructures">cEcosimDataStructers as an object</param>
    ''' <remarks>Call prior to initialization of run data.</remarks>
    Sub EcosimPreRunInitialized(ByVal EcosimDatastructures As Object)


End Interface
