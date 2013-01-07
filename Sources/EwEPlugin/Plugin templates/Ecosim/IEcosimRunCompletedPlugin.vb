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
' $Log: IEcosimRunCompletedPlugin.vb,v $
' Revision 1.1  2008/09/26 07:31:07  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2007/06/20 17:45:21  sherman
' Put CVS header
'
'
'
'==============================================================================

Public Interface IEcosimRunCompletedPlugin
    Inherits IPlugin

    Sub EcosimRunCompleted(ByVal EcosimDatastructures As Object)

End Interface
