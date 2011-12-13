#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Data adapters are responsible for nesting external spatial data into Ecospace. 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ISpatialDataAdapter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the varname that this data source interacts with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property VarName() As eVarNameFlags

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate underlying Ecospace data from a spatial raster.
        ''' </summary>
        ''' <param name="iTime">The Ecospace time step to populate data for.</param>
        ''' <remarks>
        ''' The provided raster must match the spatial extent and
        ''' projection of the underlying Ecospace grid. This should have been
        ''' ensured by the underlying <see cref="ISpatialDataConverter"/>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Function Populate(ByVal iTime As Integer) As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get whether this data adapter is connected to an external data set.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property IsConnected() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ISpatialDataConverter"/> to use for converting
        ''' the data to an <see cref="ISpatialRaster">Ecospace-compatible raster</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property Converter() As ISpatialDataConverter

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="ISpatialDataSet"/> to physically access the data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property Dataset() As ISpatialDataSet

    End Interface

End Namespace

