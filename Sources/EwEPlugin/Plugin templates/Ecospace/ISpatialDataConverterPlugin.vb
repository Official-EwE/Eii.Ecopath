Option Strict On
Imports EwEUtils.SpatialData

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for providing a spatial data converter as a plugin.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ISpatialDataConverterPlugin
    Inherits IPlugin
    Inherits ISpatialDataConverter

End Interface
