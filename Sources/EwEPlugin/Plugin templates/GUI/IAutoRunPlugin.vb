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

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing a plugin point that automatically executes with
''' one or more of the EwE <see cref="eCoreComponentType">core components</see>.
''' Note that this plug-in point just serves to centrally identify the auto-run
''' setting in the user interface. The plug-in is responsible for triggering and
''' implementing the auto-run behaviour by implementing the desired plug-in points.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IAutoRunPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of <see cref="eCoreComponentType"/> identifiers that this
    ''' plug-in can execute with.
    ''' </summary>
    ''' <returns>An array of <see cref="eCoreComponentType"/> identifiers that this
    ''' plug-in can execute with.</returns>
    ''' -----------------------------------------------------------------------
    Function AutoRunTypes() As eCoreComponentType()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set if this plug-in is enabled to auto-run with a given <see cref="eCoreComponentType">core component</see>..
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Property AutoRun(type As eCoreComponentType) As Boolean

End Interface