' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Core

    ''' ===========================================================================
    ''' <summary>
    ''' Interface for a plug-in that is invoked when the EwE Core has initialized 
    ''' its main data structures. Plug-in points in this interface
    ''' will allow an implementing plug-in to obtain a reference to the data structures.
    ''' </summary>
    ''' ===========================================================================
    Public Interface ICoreDataPlugin
        Inherits IPlugin

        ''' <summary>
        ''' The core has loaded a model and initialized its internal data
        ''' </summary>
        ''' <param name="objEcopathData">The Ecopath data structures</param>
        ''' <param name="objStanzaData">The stanza data structures</param>
        ''' <param name="objTaxonData">The taxon data structures</param>
        ''' <param name="objEcosamplerData">The ecosampler data structures</param>
        ''' <param name="objPDSdata">Particle size distribution data structures</param>
        ''' <param name="objEcosimData">The Ecosim data structures</param>
        ''' <param name="objEcosimTimeSeriesData">The Ecosim time series data structures</param>
        ''' <param name="objSearchData">The search data structures</param>
        ''' <param name="objEcoSpaceData">The Ecospace data structures</param>
        Sub CoreDataInitialized(objEcopathData As Object, objStanzaData As Object, objTaxonData As Object, objEcosamplerData As Object, objPDSdata As Object,
                            objEcosimData As Object, objEcosimTimeSeriesData As Object, objSearchData As Object, objEcoSpaceData As Object)

    End Interface

End Namespace
