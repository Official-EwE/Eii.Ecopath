' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports ZedGraph

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
