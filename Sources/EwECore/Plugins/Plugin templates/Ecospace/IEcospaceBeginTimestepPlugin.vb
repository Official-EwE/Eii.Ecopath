' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Namespace Plugins.Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for extending the Ecospace begin time step logic. Plug-ins of this
    ''' type are invoked as soon as the EwE Core is about to begin its calculatios
    ''' of an Ecospace time step.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcospaceBeginTimestepPlugin
        Inherits IPlugin

        ''' <summary>
        ''' Begin of an Ecospace time step.
        ''' </summary>
        ''' <param name="EcospaceDatastructures">Ecospace data structures.</param>
        ''' <param name="iTime">Cumulative time step.</param>
        Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer)

    End Interface

End Namespace