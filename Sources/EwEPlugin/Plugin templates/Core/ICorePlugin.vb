Option Strict On

''' ===========================================================================
''' <summary>
''' Interface for a plug-in that is invoked when the EwE Core loads the three main
''' models Ecopath, Ecosim and Ecospace. Plug-in points in this interface
''' will allow an implementing plug-in to obtain a reference to the three models.
''' </summary>
''' ===========================================================================
Public Interface ICorePlugin
    Inherits IPlugin

    ''' <summary>
    ''' The core has loaded a model and initialized its internal data
    ''' </summary>
    ''' <param name="objEcoPath">The Ecopath model</param>
    ''' <param name="objEcoSim">The Ecosim model</param>
    ''' <param name="objEcoSpace">The Ecospace model</param>
    Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object)

End Interface
