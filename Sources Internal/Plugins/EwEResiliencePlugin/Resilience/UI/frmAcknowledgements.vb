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
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmAcknowledgements

    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        Me.TabText = Me.Text
        Me.UIContext = uic
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
        cmd.AddControl(Me.m_pbIPN, "http://www.ipn.mx")
        cmd.AddControl(Me.m_pbCicimar, "http://www.cicimar.ipn.mx")
        cmd.AddControl(Me.m_pbConacyt, "http://www.conacyt.mx")

        ' No command handler for label controls
        'cmd.AddControl(Me.m_llAcknowledgements, "mailto:mzetina@ipn.mx")

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.UIContext Is Nothing) Then Return

        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME)
        cmd.RemoveControl(Me.m_pbIPN)
        cmd.RemoveControl(Me.m_pbCicimar)
        cmd.RemoveControl(Me.m_pbConacyt)

        MyBase.OnFormClosed(e)

    End Sub

    Private Sub OnContact(sender As Object, e As System.EventArgs) _
        Handles m_lblAcknowledgements.Click

        Try
            Dim cmd As cBrowserCommand = CType(Me.UIContext.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            cmd.Invoke("mailto:mzetina@ipn.mx")
        Catch ex As Exception

        End Try
    End Sub

End Class