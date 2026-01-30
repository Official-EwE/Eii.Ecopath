' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports EwECore.Common
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class frmEditSurvivabilities
    Implements IDisposable

    Private m_mse As cMSE = Nothing
    Private m_survivability As cSurvivability
    Private m_bIsDirty As Boolean

    Public Sub New(MSE As cMSE)
        Me.m_mse = MSE
        Me.InitializeComponent()
        Me.Grid = Me.m_grid
    End Sub

    Public Sub Init(uic As cUIContext)
        Me.UIContext = uic
        Me.m_grid.UIContext = uic
        Me.m_grid.Init(Me.m_mse, Me.m_mse.Survivability)
        Me.m_survivability = New cSurvivability(Me.m_mse, uic.Core, Me.m_mse.Survivability.EcosimData, Me.m_mse.Survivability.EcopathData)
        Me.m_survivability.Load()
        Me.UpdateGrid(Me.m_survivability.ListofSurvDistParams, My.Resources.HEADER_SURVIVABILITIES)
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        Me.QuickEditHandler.ShowImportExport = False
        Me.QuickEditHandler.Attach(Me.m_grid, Me.UIContext, Me.m_ts)

        AddHandler Me.m_grid.onEdited, AddressOf Me.OnGridEdited

        Me.m_bIsDirty = False
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If (Me.m_bIsDirty = True) Then
            ' JS 02Oct13: globalized this method
            ' JS 02Oct13: replaced MsgBox with cFeedbackMessage
            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_UNSAVED_CHANGES,
                                 eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.Core.Messages.SendMessage(fmsg)
            e.Cancel = (fmsg.Reply <> eMessageReply.YES)
        End If

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        Me.QuickEditHandler.Detach()
        RemoveHandler Me.m_grid.onEdited, AddressOf Me.OnGridEdited
        Me.m_grid.UIContext = Nothing

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
        ' Me.m_btnOK.Enabled = Me.m_bIsDirty
    End Sub

    Private Sub UpdateGrid(data As List(Of cSurvivability.cSurvivabilityDistributonParam), strName As String)
        Me.m_grid.Data = data
        Me.m_grid.DataName = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.CAPTION, strName)
    End Sub

    Private Sub m_btnSave_Click(sender As System.Object, e As System.EventArgs) Handles m_btnSave.Click
        Dim lstrSubMessages As New List(Of String)
        Dim strFolder As String = cMSEUtils.MSEFolder(Me.m_mse.DataPath, cMSEUtils.eMSEPaths.DistrParams)

        'Saves all the parameters to csv when user clicks to save
        If Me.m_survivability.Save() Then lstrSubMessages.Add(String.Format(My.Resources.STATUS_SAVED_DETAIL, "Survivabilities_dist.csv"))

        Me.m_bIsDirty = False

        Me.m_mse.InformUser(String.Format(My.Resources.STATUS_SAVED_DISTPARMS, My.Resources.CAPTION, strFolder),
                                 eMessageImportance.Information, strFolder, lstrSubMessages.ToArray())

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnGridEdited()
        Me.m_bIsDirty = True
        Me.Invoke(New MethodInvoker(AddressOf Me.UpdateControls))
    End Sub

    Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

End Class