' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is invoked whenever an EwE
    ''' Ecopath model has been loaded or has been saved, but before the datasource is
    ''' closed.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecopath model has been loaded, 
        ''' exposing the data source that the Ecopath model was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source from which
        ''' data is being loaded.</param>
        ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
        ''' plug-ins can respond to this event.</remarks>
        ''' <returns>True if loaded successful.</returns>
        ''' -----------------------------------------------------------------------
        Function LoadModel(dataSource As Object) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecopath model has been saved, 
        ''' exposing the data source that the Ecopath model was loaded from.
        ''' </summary>
        ''' <param name="dataSource">A reference to the EwE data source to which
        ''' data is being saved.</param>
        ''' <remarks>This plug-in point is non-exclusive, meaning that multiple
        ''' plug-ins can respond to this event.</remarks>
        ''' -----------------------------------------------------------------------
        Function SaveModel(dataSource As Object) As Boolean

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plug-in point that is called when an Ecopath model has been closed.
        ''' </summary>
        ''' <returns>True if closed successful.</returns>
        ''' -----------------------------------------------------------------------
        Function CloseModel() As Boolean

    End Interface

End Namespace