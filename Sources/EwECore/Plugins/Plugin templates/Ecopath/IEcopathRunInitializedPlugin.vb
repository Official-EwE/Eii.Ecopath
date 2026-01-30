' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Plugins.Ecopath

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Plugin point called at the start of an Ecopath run.
    ''' After all the data has been loaded but before Ecopath has started to compute 
    ''' the missing parameters. 
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcopathRunInitializedPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Plugin point called at the start of an Ecopath run.
        ''' After all the data has been loaded but before Ecopath has started to compute the missing parameters. 
        ''' </summary>
        ''' <param name="EcopathDataAsObject">cEcopathDataStructures as an object.</param>
        ''' <param name="TaxonDataAsObject">cTanonDataStructures as an object.</param>
        ''' <param name="StanzaDataAsObject">cStanzaDataStructures as an object.</param>
        ''' -----------------------------------------------------------------------
        Sub EcopathRunInitialized(EcopathDataAsObject As Object,
                                  TaxonDataAsObject As Object,
                                  StanzaDataAsObject As Object)

    End Interface

End Namespace