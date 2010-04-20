Option Strict On

''' ===========================================================================
''' <summary>
''' Plugin points for the initialization of Ecosim data cEcosimDataStructures 
''' </summary>
''' ===========================================================================
Public Interface IEcosimDataInitializedPlugin
    Inherits IPlugin

    ''' <summary>
    ''' Ecosim has loaded a scenario and is about to call cEcosimModel.Init() to initialize its data.
    ''' </summary>
    ''' <param name="EcosimDatastructures">Ecosim datastructures instance.</param>
    ''' <remarks>This can be used prior to the initialization of Ecosim data to set variables that are used by Ecosim to set derived variables. </remarks>
    Sub EcosimPreDataInitialized(ByVal EcosimDatastructures As Object)

    ''' <summary>
    ''' Ecosim is about to initailize for a run
    ''' </summary>
    ''' <param name="EcosimDatastructures">cEcosimDataStructures instance.</param>
    ''' <remarks>Call prior to initialization of run data.</remarks>
    Sub EcosimPreRunInitialized(ByVal EcosimDatastructures As Object)


End Interface
