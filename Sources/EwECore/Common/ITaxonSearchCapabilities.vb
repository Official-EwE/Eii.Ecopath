' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Common

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Template for reporting taxonomic search capabilities
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Interface ITaxonSearchCapabilities

        ''' <summary>
        ''' Returns a bitwise pattern of <see cref="eTaxonClassificationType"/> enumerated
        ''' values stating which taxonomic classification fields can be searched.
        ''' </summary>
        ''' <returns>A bitwise pattern of <see cref="eTaxonClassificationType"/> enumerated
        ''' values stating which taxonomic classification fields can be searched.</returns>
        Function TaxonSearchCapabilities() As eTaxonClassificationType

        ''' <summary>
        ''' Returns whether the taxonomic search engine can search by spatial bounding box.
        ''' </summary>
        ''' <returns>True if the taxonomic search engine can search by spatial bounding box</returns>
        Function HasSpatialSearchCapabilities() As Boolean

        ''' <summary>
        ''' Returns whether the taxonomic search engine can search by depth range.
        ''' </summary>
        ''' <returns>True if the taxonomic search engine can search by depth range.</returns>
        Function HasDepthRangeSearchCapabilities() As Boolean

    End Interface

End Namespace
