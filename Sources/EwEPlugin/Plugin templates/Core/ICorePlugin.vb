Option Strict On

Public Interface ICorePlugin
    Inherits IPlugin

    ''' <summary>
    ''' The core has loaded a model and initialized its internal data
    ''' </summary>
    ''' <param name="objEcoPath">The Ecopath model</param>
    ''' <param name="objEcoSim">The Ecosim model</param>
    ''' <param name="objEcoSpace">The Ecospace model</param>
    ''' <remarks></remarks>
    Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object)


End Interface
