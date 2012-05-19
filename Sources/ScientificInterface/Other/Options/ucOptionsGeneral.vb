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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports WeifenLuo.WinFormsUI
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Utilities
Imports System.Configuration
Imports EwEUtils.SystemUtilities

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > General settings interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucOptionsGeneral
        Implements IOptionsPage

        Private m_uic As cUIContext = Nothing
        Private m_fpVerboseLevel As cEwEFormatProvider = Nothing

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.m_uic = uic
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            'Me.m_tsddFields.DropDown.Items.Clear()
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Constructors

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            Dim bHasMRU As Boolean = False

            If (My.Settings.MdbRecentlyUsedList IsNot Nothing) Then
                bHasMRU = (My.Settings.MdbRecentlyUsedList.Count <= 1)
            End If

            ' Enable button if there is something to clear
            Me.m_btnClearMRU.Enabled = bHasMRU

            Me.m_nudMRU.Value = CInt(Math.Min(Me.m_nudMRU.Maximum, _
                                     Math.Max(Me.m_nudMRU.Minimum, My.Settings.MdbRecentlyUsedCount)))

            Me.m_nudMaxNumMessages.Value = CInt(Math.Min(Me.m_nudMaxNumMessages.Maximum, _
                                                Math.Max(Me.m_nudMaxNumMessages.Minimum, My.Settings.StatusMaxMessages)))
            Me.m_fpVerboseLevel = New cEwEFormatProvider(Me.m_uic, Me.m_cmbLogLevel, New cVerboseLevelTypeFormatter(), Nothing)
            Me.m_fpVerboseLevel.Value = cLog.VerboseLevel

            ' Output path
            Me.m_fieldpickOutput.UIContext = Me.m_uic
            Me.m_fieldpickOutput.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbOutputMask.Text = My.Settings.OutputPathMask

            ' Backup path masks
            Me.m_fieldpickBackup.UIContext = Me.m_uic
            Me.m_fieldpickBackup.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbBackupMask.Text = My.Settings.BackupFileMask

            Me.m_cbDownloadUpdates.Checked = My.Settings.AutoUpdatePlugins
            Me.m_nudTimeOut.Value = CDec(Math.Max(1, Math.Round(My.Settings.UpdatePluginsTimeout / 1000)))
            Me.m_cbShowHost.Checked = My.Settings.ShowHostInfo
            Me.m_cbShowTime.Checked = My.Settings.StatusShowTime

            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            ' Only when toggling this option on
            If (Me.m_cbDownloadUpdates.Checked And My.Settings.AutoUpdatePlugins = False) Then
                If (Not cSystemUtils.IsAdministrator()) Then
                    result = IOptionsPage.eApplyResultType.Success_administrator
                Else
                    result = IOptionsPage.eApplyResultType.Success_restart
                End If
            End If

            Try

                My.Settings.MdbRecentlyUsedCount = CInt(Me.m_nudMRU.Value)
                My.Settings.StatusMaxMessages = CInt(Me.m_nudMaxNumMessages.Value)
                My.Settings.AutoUpdatePlugins = Me.m_cbDownloadUpdates.Checked
                My.Settings.UpdatePluginsTimeout = CInt(Me.m_nudTimeOut.Value * 1000)
                My.Settings.StatusShowTime = Me.m_cbShowTime.Checked
                My.Settings.BackupFileMask = Me.m_tbBackupMask.Text
                My.Settings.OutputPathMask = Me.m_tbOutputMask.Text
                My.Settings.ShowHostInfo = Me.m_cbShowHost.Checked
                My.Settings.LogVerboseLevel = DirectCast(Me.m_fpVerboseLevel.Value, cLog.eVerboseLevel)

            Catch ex As Exception
                result = IOptionsPage.eApplyResultType.Failed
            End Try

            Return result

        End Function

#End Region ' Public access

#Region " Event handlers "

        Private Sub btnClearOverwrite_click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClearOVerwritePrompts.Click
            My.Settings.SuppressedOverwritePrompts = ""
        End Sub

        Private Sub btnClearMRU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClearMRU.Click
            Me.ClearFileList(My.Settings.MdbRecentlyUsedList)
            Me.m_btnClearMRU.Enabled = False
            Me.UpdateControls()
        End Sub

        Private Sub OnOutputFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickOutput.OnFieldPicked

            Me.InsertText(Me.m_tbOutputMask, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnOutputDirectoryPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickOutput.OnDirectoryPicked

            Me.m_tbOutputMask.SelectionStart = 0
            Me.m_tbOutputMask.SelectionLength = Math.Max(0, Me.m_tbOutputMask.Text.LastIndexOf("\"c))
            Me.InsertText(Me.m_tbOutputMask, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupDirectoryPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickBackup.OnDirectoryPicked

            Me.m_tbBackupMask.SelectionStart = 0
            Me.m_tbBackupMask.SelectionLength = Math.Max(0, Me.m_tbBackupMask.Text.LastIndexOf("\"c))
            Me.InsertText(Me.m_tbBackupMask, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickBackup.OnFieldPicked

            Me.InsertText(Me.m_tbBackupMask, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnMaskChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tbBackupMask.TextChanged, m_tbOutputMask.TextChanged

            Me.UpdateControls()

        End Sub

        Private Sub OnDefaults(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnDefaults.Click

            ' Better protect this code in case settings property names change
            Try
                Me.m_nudMRU.Value = CInt(My.Settings.GetDefaultValue("MdbRecentlyUsedCount"))
                Me.m_tbOutputMask.Text = CStr(My.Settings.GetDefaultValue("OutputPathMask"))
                Me.m_tbBackupMask.Text = CStr(My.Settings.GetDefaultValue("BackupFileMask"))
            Catch ex As Exception

            End Try

        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub ClearFileList(ByVal fileList As ArrayList)

            If (fileList Is Nothing) Then Return

            Dim fmsg As New cFeedbackMessage(My.Resources.GENERIC_PROMPT_CLEAR_MRU, EwEUtils.Core.eCoreComponentType.Core, eMessageType.Any, eMessageImportance.Question, cFeedbackMessage.eReplyStyle.YES_NO)
            Me.m_uic.Core.Messages.SendMessage(fmsg)

            If (fmsg.Reply = cFeedbackMessage.eReply.YES) Then
                ' Clear confirmed
                fileList.Clear()
                ' This is a temporary solution to avoid returning null reference.
                fileList.Add(New System.Object)
            End If

        End Sub

        Private Sub UpdateControls()

            Dim bHasSuppressedPrompts As Boolean = (Not String.IsNullOrEmpty(My.Settings.SuppressedOverwritePrompts))
            Dim bHasMRU As Boolean = (My.Settings.MdbRecentlyUsedList.Count > 0)

            Me.m_btnClearOVerwritePrompts.Enabled = bHasSuppressedPrompts
            Me.m_btnClearMRU.Enabled = bHasMRU

            Me.UpdateSample(Me.m_lblSampleOutput, Me.m_tbOutputMask.Text)
            Me.UpdateSample(Me.m_lblSampleBackup, Me.m_tbBackupMask.Text)

        End Sub

        Private Sub UpdateSample(ByVal lbl As Label, ByVal strMask As String)

            Dim strVersion As String = Application.ProductVersion.ToString
            Dim strDocDir As String = Environment.GetFolderPath(Environment.SpecialFolder.Personal)
            Dim strSample As String = ""

            If Not cPathUtility.ResolvePath(strMask, Me.m_uic.Core, strSample) Then
                cPathUtility.ResolvePath(strMask, "model", strDocDir, ".eweaccdb", strVersion, strSample)
            End If
            lbl.Text = cStringUtils.CompactString(strSample, lbl.ClientRectangle.Width, lbl.Font, TextFormatFlags.PathEllipsis)

        End Sub

        Private Sub InsertText(ByVal tb As TextBox, ByVal strText As String)
            Dim strSrc As String = tb.Text
            Dim strDest As String
            Dim iSelStart As Integer = tb.SelectionStart
            Dim iSelLen As Integer = tb.SelectionLength
            Dim iItemLen As Integer = strText.Length

            If (iSelLen = 0) Then
                strDest = strSrc & strText
                iSelStart = strDest.Length
            Else
                strDest = strSrc.Substring(0, iSelStart) & strText & strSrc.Substring(iSelStart + iSelLen)
                iSelStart += iItemLen
            End If

            tb.Text = strDest
            tb.SelectionStart = iSelStart
            tb.SelectionLength = 0
        End Sub

        Private Sub ReplaceText(ByVal tb As TextBox, ByVal strText As String)
            tb.Text = strText
        End Sub

#End Region ' Internals

    End Class

End Namespace
