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
#Region " Imports "

Option Strict On
Imports System.Windows.Forms

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' IPluginHelpPlugin, interface for providing help information for a 
''' <see cref="IPlugin">plugin</see>.
''' </summary>
''' ---------------------------------------------------------------------------
Public Interface IHelpPlugin

    ''' <summary>URL to plug-in help file.</summary>
    ReadOnly Property HelpURL As String

    ''' <summary>URL to plug-in help page.</summary>
    ReadOnly Property HelpTopic As String

End Interface
