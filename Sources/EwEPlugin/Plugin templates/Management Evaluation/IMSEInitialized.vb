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

''' <summary>
''' Interface for MSE initialization plugin points that are invoked once the MSE model has been loaded
''' </summary>
''' <remarks></remarks>
Public Interface IMSEInitialized
    Inherits IPlugin

    ''' <summary>
    ''' MSE model has been initialized
    ''' </summary>
    ''' <param name="MSEModel">MSE model</param>
    ''' <param name="MSEDataStructure">MSE data structures</param>
    ''' <param name="EcosimDatastructures">Ecosim data structures</param>
    ''' <remarks></remarks>
    Sub MSEInitialized(MSEModel As Object, MSEDataStructure As Object, EcosimDatastructures As Object)

End Interface


