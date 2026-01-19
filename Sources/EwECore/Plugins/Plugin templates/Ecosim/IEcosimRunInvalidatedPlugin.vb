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

Namespace Plugins.Ecosim

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing a plugin point that is automatically invoked when
    ''' an Ecosim run is invalidated. This happens when an user input causes
    ''' the current Ecosim results to become invalid, or when an Ecospace scenario
    ''' is closed.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Interface IEcosimRunInvalidatedPlugin
        Inherits IPlugin

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Execute an Ecosim Run Invalidated plug-in.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Sub EcosimRunInvalidated()

    End Interface

End Namespace