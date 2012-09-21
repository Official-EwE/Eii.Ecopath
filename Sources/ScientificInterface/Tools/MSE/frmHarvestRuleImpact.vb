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
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ZedGraph
Imports ScientificInterfaceShared.Commands

#End Region

''' <summary>
''' In order to understand how the various harvest policy rules, including the LP, 
''' are behaving and impacting value, it is critical that we be able to do a plot 
''' for each group showing the target and realized fishing mortality rates over time, 
''' i.e. Ftarget(group,time) and FishTime(group,time).  Comparison of these values 
''' will tell users which group(s) are constraining or limiting fishing, e.g. which 
''' LP group constraints are at the Ftarget limit.  This is critical for identifying 
''' which group(s) could be allowed to suffer some overfishing in the interest of 
''' increasing total value, by increasing the target F for such groups.
''' </summary>
Public Class frmHarvestRuleImpact

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
        MyBase.OnCoreMessage(msg)
    End Sub

End Class

