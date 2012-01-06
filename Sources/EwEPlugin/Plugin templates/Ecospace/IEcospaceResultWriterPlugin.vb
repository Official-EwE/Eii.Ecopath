Option Strict On
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for providing an Ecospace result writer as a plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IEcospaceResultWriterPlugin
    Inherits IPlugin
    Inherits IEcospaceResultsWriter

End Interface
