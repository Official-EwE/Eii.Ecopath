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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

'==============================================================================
'
' $Log: ICustomPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/03/15 14:09:57  jeroens
' + Commented
'
' Revision 1.1  2006/08/30 20:52:35  jeroens
' * Moved and/or created
'
' Revision 1.1  2006/08/20 21:20:06  jeroens
' Initial version
'
'==============================================================================

Option Strict On

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for implementing your own plugin functionality that is does not
''' get invoked from a built-in EwE core or GUI plugin point.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface ICustomPlugin
    Inherits IPlugin

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Invoke this custom plug-in point.
    ''' </summary>
    ''' <param name="strMethod">A string name identifying the functionality in
    ''' this plugin to invoke. This string can contain anything; the implementation
    ''' of the plug-in can evaluate this string to implement specific behaviour.</param>
    ''' <param name="objArgs">An array of arguments to pass into the plugin.</param>
    ''' <param name="objResult">The outcome of invoking the plug-in, if any.</param>
    ''' <returns>True to indicate that this call was successful, and to stop executing 
    ''' custom plug-ins with the same name.</returns>
    ''' -----------------------------------------------------------------------
    Function Invoke(ByVal strMethod As String, ByVal objArgs() As Object, ByRef objResult As Object) As Boolean

End Interface
