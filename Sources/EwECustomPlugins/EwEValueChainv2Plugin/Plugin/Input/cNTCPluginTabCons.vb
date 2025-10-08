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

Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cNTCPluginTabCons
    Inherits cNavTreeControlPlugin

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "vcV2Node24Consumers"
        End Get
    End Property

    Public Overrides ReadOnly Property DisplayName() As String
        Get
            Return My.Resources.NAVTREE_INPUT_TABLE_CONSUMERS
        End Get
    End Property

    Public Overrides Function FormPage() As eValueChainPageTypes
        Return eValueChainPageTypes.TableConsumers
    End Function

    Public Overrides ReadOnly Property Description() As String
        Get
            Return "Value chain 'Consumers table' navigation element"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation() As String
        Get
            Return Me.NavTreeNodeRoot() & "|vcV2Node00|vcNode10Tables"
        End Get
    End Property

    Public Overrides ReadOnly Property ControlImage() As Object
        Get
            Return SharedResources.nav_output
        End Get
    End Property

End Class
