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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Public Class frmConfig
    Inherits frmEwEGrid

    Public Sub New(uic As cUIContext)

        Me.UIContext = uic
        Me.InitializeComponent()

        Me.Grid = m_grid

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Me.Text = My.Resources.CAPTION
        Me.TabText = Me.Text
        Me.m_tsbnAutosave.Image = ScientificInterfaceShared.My.Resources.saveHS

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Core}
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
    End Sub

    Private Sub m_tsmiGOM_Click(sender As Object, e As EventArgs) Handles m_tsmiGOM.Click

        cFishMIPcore.GetInstance().Configuration().LoadEcoOcean()
        Me.m_grid.RefreshContent()

    End Sub

    Private m_bInUpdate As Boolean = False

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)

        If (msg.Type = eMessageType.GlobalSettingsChanged) Then
            Me.UpdateControls()
        End If

    End Sub
End Class