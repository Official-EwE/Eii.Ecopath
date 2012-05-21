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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing an Options page
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IOptionsPage

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating all possible results when applying the content
    ''' of an options page.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Enum eApplyResultType As Integer
        ''' <summary>Application was successful.</summary>
        Success
        ''' <summary>Application was successful but requires a restart.</summary>
        Success_restart
        ''' <summary>Application successful, but need administrator privileges to work.</summary>
        Success_administrator
        ''' <summary>Application failed.</summary>
        Failed
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Method to apply the content of an options page to the 'system'.
    ''' </summary>
    ''' <returns>An <see cref="eApplyResultType">apply result</see>.</returns>
    ''' -----------------------------------------------------------------------
    Function Apply() As eApplyResultType

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Revert the current page to default values
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Sub SetDefaults()

End Interface
