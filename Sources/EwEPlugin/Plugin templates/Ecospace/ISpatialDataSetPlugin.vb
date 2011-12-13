Option Strict On
Imports EwEUtils.SpatialData

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for providing a spatial data set as a plugin.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ISpatialDataSetPlugin
    Inherits IPlugin
    Inherits ISpatialDataSet

End Interface
