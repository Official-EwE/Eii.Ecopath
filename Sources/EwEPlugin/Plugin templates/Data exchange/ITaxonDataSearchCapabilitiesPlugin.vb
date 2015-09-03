' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System
Imports EwEUtils.Core

#End Region ' Imports

Namespace Data

    ''' <summary>
    ''' Plug-in point allowing taxonomy <see cref="IDataSearchProducerPlugin">search
    ''' plug-ins</see> to report their search capabilities.
    ''' </summary>
    Public Interface ITaxonDataSearchCapabilitiesPlugin
        Inherits IDataSearchProducerPlugin

        ''' <summary>
        ''' Returns a bitwise pattern of <see cref="eTaxonClassificationType"/> enumerated
        ''' values stating which taxonomic classification fields can be searched.
        ''' </summary>
        ''' <returns>A bitwise pattern of <see cref="eTaxonClassificationType"/> enumerated
        ''' values stating which taxonomic classification fields can be searched.</returns>
        Function TaxonSearchCapabilities() As eTaxonClassificationType

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function SpatialSearchCapabilities() As Boolean

    End Interface

End Namespace
