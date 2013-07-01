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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
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

    Public Sub New(ByVal uic As cUIContext, _
                   ByVal man As cNetworkManager)

        Me.m_uic = uic
        Me.m_man = man

        Me.InitializeComponent()

    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_cbCalcCyclesPathways.Checked = Me.m_man.UseCyclesPathways
        Me.UpdateControls()
    End Sub

    Private Sub m_cbCalcCyclesPathways_CheckedChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbCalcCyclesPathways.CheckedChanged
        Try
            Me.m_man.UseCyclesPathways = m_cbCalcCyclesPathways.Checked
            Me.UpdateControls()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub UpdateControls()
        Me.m_nudTimeOut.Enabled = Me.m_man.UseCyclesPathways
        Me.m_lblTimeout.Enabled = Me.m_man.UseCyclesPathways
        Me.m_lblTimeOutUnit.Enabled = Me.m_man.UseCyclesPathways
    End Sub

End Class
