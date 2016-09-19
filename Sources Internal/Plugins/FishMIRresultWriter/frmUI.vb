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

Public Class frmUI
    Inherits frmEwEGrid

    Public Sub New(uic As cUIContext, plugin As cFishMIPResultWriterPlugin)

        Me.UIContext = uic
        Me.InitializeComponent()

        Me.Plugin = plugin
        Me.m_grid.Plugin = plugin
        Me.Grid = m_grid

    End Sub

    Public Property Plugin As cFishMIPResultWriterPlugin = Nothing

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
        Me.m_tsbnAutosave.Checked = Me.Core.Autosave(eAutosaveTypes.EcospaceResults, Me.Plugin.DataName)
    End Sub

    Private Sub m_tsmiGOM_Click(sender As Object, e As EventArgs) Handles m_tsmiGOM.Click

        Me.Plugin.InitEcoOcean()
        Me.m_grid.RefreshContent()

    End Sub

    Private m_bInUpdate As Boolean = False

    Private Sub OnAutosaveToggle(sender As Object, e As EventArgs) Handles m_tsbnAutosave.Click
        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Try
            Me.Plugin.Enabled = Not m_tsbnAutosave.Checked
            Me.Core.Autosave(eAutosaveTypes.EcospaceResults, Me.Plugin.DataName) = Me.Plugin.Enabled
        Catch ex As Exception

        End Try
        Me.m_bInUpdate = False
    End Sub

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)

        If (msg.Type = eMessageType.GlobalSettingsChanged) Then
            Me.UpdateControls()
        End If

    End Sub
End Class