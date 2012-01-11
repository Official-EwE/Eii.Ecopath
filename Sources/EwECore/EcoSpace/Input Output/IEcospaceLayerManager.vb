Imports EwEUtils.Core

Namespace Core

    Public Interface IEcospaceLayerManager

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get managed layers.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> to get layers for.
        ''' If <see cref="eVarNameFlags.NotSet"/> is provided this manager will return
        ''' all maintained layers.</param>
        ''' <returns>An array of all managed layers.</returns>
        ''' -----------------------------------------------------------------------
        Function Layers(Optional ByVal varName As eVarNameFlags = eVarNameFlags.NotSet) As cEcospaceLayer()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a single layer.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> to get layers for.</param>
        ''' <param name="iIndex">Optional index of the layer to retrieve.</param>
        ''' <returns>A single layer.</returns>
        ''' -----------------------------------------------------------------------
        Function Layer(ByVal varName As eVarNameFlags, Optional ByVal iIndex As Integer = cCore.NULL_VALUE) As cEcospaceLayer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Method that managed layers can call to request their data.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> to get layer data for.</param>
        ''' <returns>Data in a format that the layer should understand.</returns>
        ''' -----------------------------------------------------------------------
        Function LayerData(ByVal varName As eVarNameFlags) As Object

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Callback for layers to report that they have been <see cref="cEcospaceLayer.Invalidate">invalidated</see>.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> of the layer that was invalidated.</param>
        ''' <param name="iIndex">Optional index of the layer that invalidated..</param>
        ''' -----------------------------------------------------------------------
        Sub LayerChanged(varName As eVarNameFlags, Optional iIndex As Integer = cCore.NULL_VALUE)

    End Interface

End Namespace
