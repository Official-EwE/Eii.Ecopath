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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Option Strict On

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
    ''' <param name="objEcoSpaceData">The Ecospace data structures</param>
    Sub CoreDataInitialized(objEcopathData As Object, objStanzaData As Object, objTaxonData As Object, objEcosamplerData As Object, objPDSdata As Object,
                            objEcosimData As Object, objEcosimTimeSeriesData As Object, objEcoSpaceData As Object)

End Interface
