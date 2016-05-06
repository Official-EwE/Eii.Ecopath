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

Imports ScientificInterfaceShared.Controls

Public Class frmProgress

    Private m_bStop As Boolean = False
    Private m_uic As cUIContext = Nothing

    Public Sub New(uic As cUIContext)
        Me.m_uic = uic
        Me.InitializeComponent()
        Me.TopLevel = True
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.CenterToParent()
        cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)
        MyBase.OnFormClosed(e)
    End Sub

    Public Sub SetStatus(ByVal strText As String, ByVal sProgress As Single)

        cApplicationStatusNotifier.UpdateProgress(Me.m_uic.Core, strText, sProgress)
        Try
            Me.m_lblInfo.Text = strText
            Me.m_progress.Value = CInt(sProgress * 100)
            Me.Refresh()
        Catch ex As Exception

        End Try
    End Sub

    Public Property [Stop]() As Boolean
        Get
            Return Me.m_bStop
        End Get
        Private Set(ByVal value As Boolean)
            Me.m_bStop = value
        End Set
    End Property

    Private Sub m_btnStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnStop.Click
        Me.m_bStop = True
    End Sub
End Class