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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class frmEditAssessmentError
    Implements IDisposable

    Private m_mse As cMSE = Nothing
    Private m_bIsDirty As Boolean
    Private m_StockAssess As cStockAssessmentModel

    Private Class cComboItem

        Public Property ErrorDataType As eErrorType
        Public Property Text As String

        Public Sub New(ItemTest As String, ErorrTypeEnum As eErrorType)
            ErrorDataType = ErorrTypeEnum
            Text = ItemTest
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Text
        End Function
    End Class


    Public Enum eErrorType As Integer
        GroupObervationError
        FleetImplementationError
    End Enum

    Public Sub New(ByVal uic As cUIContext, MSE As cMSE)
        Me.m_mse = MSE
        Me.UIContext = uic
        Me.InitializeComponent()
        Me.Grid = Me.m_grdError
    End Sub


    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return
        Me.QuickEditHandler.ShowImportExport = False
        Me.QuickEditHandler.Attach(Me.m_grdError, Me.UIContext, Me.m_ts)

        Me.m_StockAssess = New cStockAssessmentModel(Me.m_mse)
        Me.m_StockAssess.Load()

        Me.m_grdError.Init(Me.m_StockAssess)
        Me.m_grdError.UIContext = Me.UIContext
        Me.m_grdError.ErrorDataType = eErrorType.GroupObervationError

        Me.m_tscbTypes.Items.Add(New cComboItem("Biomass obervation error", eErrorType.GroupObervationError))
        Me.m_tscbTypes.Items.Add(New cComboItem("Fleet implementation error", eErrorType.FleetImplementationError))
        Me.m_tscbTypes.SelectedIndex = 0

        Me.m_bIsDirty = False
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If (Me.m_bIsDirty = True) Then
            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_UNSAVED_CHANGES, _
                                 eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            fmsg.Reply = eMessageReply.YES
            Me.Core.Messages.SendMessage(fmsg)
            e.Cancel = (fmsg.Reply <> eMessageReply.YES)
        End If

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        Me.QuickEditHandler.Detach()
        Me.m_grdError.UIContext = Nothing

        MyBase.OnFormClosed(e)

    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()
        ' Me.m_btnOK.Enabled = Me.m_bIsDirty
    End Sub

    'Private Sub UpdateGrid(data As cDiets, strName As String)
    '    Me.m_grid.Init(data)
    '    Me.m_grid.DataName = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, My.Resources.CAPTION, strName)
    'End Sub

    Private Sub m_btnSave_Click(sender As System.Object, e As System.EventArgs) Handles m_btnSave.Click
        Dim lstrSubMessages As New List(Of String)
        Dim strFolder As String = cMSEUtils.MSEFolder(Me.m_mse.DataPath, cMSEUtils.eMSEPaths.StockAssessment)

        Me.m_StockAssess.Save()

        Me.m_bIsDirty = False

        'Me.m_mse.InformUser(String.Format(My.Resources.STATUS_SAVED_DISTPARMS, My.Resources.CAPTION, strFolder), _
        '                         eMessageImportance.Information, strFolder, lstrSubMessages.ToArray())

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub OnGridEdited()
        Me.m_bIsDirty = True
        Me.Invoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub m_tscbTypes_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles m_tscbTypes.SelectedIndexChanged

        Try

            Dim comboItem As cComboItem = DirectCast(Me.m_tscbTypes.SelectedItem, cComboItem)
            Me.m_grdError.ErrorDataType = comboItem.ErrorDataType

        Catch ex As Exception

        End Try

    End Sub
End Class