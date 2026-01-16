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

Imports System.ComponentModel
Imports EwECore.Database.cEwEDatabase
Imports EwEUtils.Utilities

''' ===========================================================================
''' <summary>
''' One single flow diagram.
''' </summary>
''' ===========================================================================
<TypeConverter(GetType(cPropertySorter)),
    DefaultProperty("Name"),
    Serializable()>
Public Class cFlowDiagram
    : Inherits cOOPStorable

#Region " Properties "

    <Browsable(True), _
       DisplayName("Name"), _
       Description("Name of this diagram"),
       cPropertySorter.PropertyOrder(1)>
    Public Overridable Property Name() As String = "Default"

#End Region ' Properties

End Class
