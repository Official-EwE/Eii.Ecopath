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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Public Class cNavTreeSupplyDemand
    Inherits cNavTreeRoot

    Public Overrides ReadOnly Property Name As String
        Get
            Return "n01SupplyDemand"
        End Get
    End Property

    Public Overrides ReadOnly Property NavigationTreeItemLocation As String
        Get
            Return MyBase.NavigationTreeItemLocation & "\" & MyBase.Name
        End Get
    End Property

    Public Overrides ReadOnly Property ControlText As String
        Get
            Return My.Resources.NAVTREE_SUPPLY_DEMAND
        End Get
    End Property

    Protected Overrides Function GetUI() As System.Windows.Forms.Form
        Return New frmSupplyDemand(Me.m_uic, cResiliencePlugin.GetInstance().Model)
    End Function

    Public Overrides ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
        End Get
    End Property

End Class
