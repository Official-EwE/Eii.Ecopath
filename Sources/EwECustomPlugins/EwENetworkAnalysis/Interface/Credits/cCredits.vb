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

Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports ZedGraph

<CLSCompliant(False)>
Public Class cCredits
    Inherits cContentManager

    Public Overrides Function Attach(manager As cNetworkManager, datagrid As DataGridView, graph As ZedGraphControl, plot As ucPlot, toolstrip As ToolStrip, info As Control, uic As cUIContext) As Boolean
        If MyBase.Attach(manager, datagrid, graph, plot, toolstrip, info, uic) Then
            Me.InfoPanel.Visible = True
            Return True
        End If
        Return False
    End Function

    Public Overrides Sub DisplayData()
        ' NOP
    End Sub

    Public Overrides Function PageTitle() As String
        Return My.Resources.PAGE_CREDITS
    End Function

End Class
