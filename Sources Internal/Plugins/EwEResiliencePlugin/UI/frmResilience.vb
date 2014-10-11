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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmResilience

    Private m_data As cResilienceData = Nothing

    Public Sub New(uic As cUIContext, data As cResilienceData)
        MyBase.New()
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_data = data
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.Text = My.Resources.CAPTION
        AddHandler Me.m_data.OnUpdated, AddressOf OnDataUpdated
    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        RemoveHandler Me.m_data.OnUpdated, AddressOf OnDataUpdated
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub OnDataUpdated(sender As cResilienceData, iTime As Integer)
        Console.WriteLine("Resilience {0}: supply {1}, demand {2}", iTime, m_data.Supply(iTime), Me.m_data.Demand(iTime))
    End Sub

End Class