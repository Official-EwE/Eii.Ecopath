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

Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Commands

#End Region ' Imports

Public Class ucAcknowledgements
    Implements IUIElement

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext _
        Implements ScientificInterfaceShared.Controls.IUIElement.UIContext

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub m_pbIPN_Click(sender As System.Object, e As System.EventArgs) Handles m_pbIPN.Click
        Me.VisitSponsor("http://www.ipn.mx")
    End Sub

    Private Sub m_pbCicimar_Click(sender As System.Object, e As System.EventArgs) Handles m_pbCicimar.Click
        Me.VisitSponsor("http://www.cicimar.ipn.mx")
    End Sub

    Private Sub m_pbConacyt_Click(sender As System.Object, e As System.EventArgs) Handles m_pbConacyt.Click
        Me.VisitSponsor("http://www.conacyt.mx")
    End Sub

    Private Sub VisitSponsor(strURL As String)
        Try
            Dim cmd As cBrowserCommand = CType(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke(strURL)
        Catch ex As Exception

        End Try
    End Sub

End Class
