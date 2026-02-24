' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

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
        Function Layers(Optional varName As eVarNameFlags = eVarNameFlags.NotSet) As cEcospaceLayer()

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get a single layer.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> to get layers for.</param>
        ''' <param name="iIndex">Optional one-based index of the layer to retrieve.</param>
        ''' <returns>A single layer.</returns>
        ''' -----------------------------------------------------------------------
        Function Layer(varName As eVarNameFlags, Optional iIndex As Integer = cCore.NULL_VALUE) As cEcospaceLayer

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Method that managed layers can call to request their data.
        ''' </summary>
        ''' <param name="varName">The <see cref="eVarNameFlags">variable</see> to get layer data for.</param>
        ''' <param name="iIndex">Index of the layer to obtain data for.</param>
        ''' <returns>Data in a format that the layer should understand.</returns>
        ''' -----------------------------------------------------------------------
        Function LayerData(varName As eVarNameFlags, iIndex As Integer) As Object

    End Interface

End Namespace
