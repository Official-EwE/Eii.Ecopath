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

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Core

#End Region ' Imports

Public Class ucOptions

    Private m_uic As cUIContext = Nothing
    Private m_man As cNetworkManager = Nothing
    Private m_bInUpdate As Boolean = False

    Public Sub New(ByVal uic As cUIContext, _
                   ByVal man As cNetworkManager)

        Me.m_uic = uic
        Me.m_man = man

        Me.InitializeComponent()

    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_bInUpdate = True
        Me.m_cbUseTimeout.Checked = Me.m_man.UseAbortTimer
        Me.m_nudTimeOut.Value = CInt(Me.m_man.TimeOutMilSecs / (1000 * 60))
        Me.m_bInUpdate = False

        Me.UpdateControls()
    End Sub

    Private Sub OnTimeOutCheckChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbUseTimeout.CheckedChanged

        If Me.m_bInUpdate Then Return

        Try
            Me.m_man.UseAbortTimer = m_cbUseTimeout.Checked
            My.Settings.UseAbortTimer = m_cbUseTimeout.Checked
            My.Settings.Save()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
        Me.UpdateControls()

    End Sub

    Private Sub OnTimeOutChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_nudTimeOut.Validated

        If Me.m_bInUpdate Then Return

        Try
            Me.m_man.TimeOutMilSecs = CInt(Me.m_nudTimeOut.Value * 1000 * 60)
            My.Settings.AbortTimoutMins = CInt(Me.m_nudTimeOut.Value)
            My.Settings.Save()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
        Me.UpdateControls()

    End Sub

    Private Sub UpdateControls()
        Me.m_nudTimeOut.Enabled = Me.m_man.UseAbortTimer
        Me.m_lblTimeout.Enabled = Me.m_man.UseAbortTimer
        Me.m_lblTimeOutUnit.Enabled = Me.m_man.UseAbortTimer
    End Sub

End Class
