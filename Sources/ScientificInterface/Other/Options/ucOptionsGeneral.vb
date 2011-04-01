#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports EwECore
Imports EwEUtils.Commands
Imports WeifenLuo.WinFormsUI
Imports ScientificInterfaceShared.Commands

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

            ' Output path
            Me.m_fieldpickOutput.UIContext = Me.m_uic
            Me.m_fieldpickOutput.Fields = New Object() {cPathUtility.ePathPlaceholderTypes.Desktop, _
                                                        cPathUtility.ePathPlaceholderTypes.ModelPath, _
                                                        cPathUtility.ePathPlaceholderTypes.MyAppData, _
                                                        cPathUtility.ePathPlaceholderTypes.MyDocuments}
            Me.m_tbOutput.Text = My.Settings.OutputPathMask

            ' Backup path masks
            Me.m_fieldpickBackup.UIContext = Me.m_uic
            Me.m_fieldpickBackup.Fields = [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
            Me.m_tbBackupMask.Text = My.Settings.BackupFileMask

            Me.m_cbCheckEwE6.Checked = False
            Me.m_cbDownloadUpdates.Checked = My.Settings.AutoUpdatePlugins
            Me.m_cbShowTime.Checked = My.Settings.StatusShowTime

            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Function Apply() As IOptionsPage.eApplyResultType _
            Implements IOptionsPage.Apply

            Dim bRestart As Boolean = (My.Settings.AutoUpdatePlugins <> Me.m_cbDownloadUpdates.Checked)

            My.Settings.MdbRecentlyUsedCount = CInt(Me.m_nudMRU.Value)
            My.Settings.StatusMaxMessages = CInt(Me.m_nudMaxNumMessages.Value)
            My.Settings.AutoUpdatePlugins = Me.m_cbDownloadUpdates.Checked
            My.Settings.StatusShowTime = Me.m_cbShowTime.Checked
            My.Settings.BackupFileMask = Me.m_tbBackupMask.Text
            My.Settings.OutputPathMask = Me.m_tbOutput.Text

            If bRestart Then Return IOptionsPage.eApplyResultType.Success_restart
            Return IOptionsPage.eApplyResultType.Success

        End Function

#End Region ' Public access

#Region " Event handlers "

        Private Sub m_btnResetOverwritePrompts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnResetOverwritePrompts.Click
            My.Settings.SuppressedOverwritePrompts = ""
            Me.UpdateControls()
        End Sub

        Private Sub btnClearMRU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClearMRU.Click
            Me.ClearFileList(My.Settings.MdbRecentlyUsedList)
            Me.m_btnClearMRU.Enabled = False
            Me.UpdateControls()
        End Sub

        Private Sub OnOutputDirPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickOutput.OnDirectoryPicked

            Me.ReplaceText(Me.m_tbOutput, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnOutputFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickOutput.OnFieldPicked

            Me.ReplaceText(Me.m_tbOutput, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupDirectoryPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal strDirectory As String) _
            Handles m_fieldpickBackup.OnDirectoryPicked

            Me.m_tbBackupMask.SelectionStart = 0
            Me.m_tbBackupMask.SelectionLength = Math.Max(0, Me.m_tbBackupMask.Text.LastIndexOf("\"c))
            Me.InsertText(Me.m_tbBackupMask, strDirectory)
            Me.UpdateControls()

        End Sub

        Private Sub OnFieldPicked(ByVal sender As ScientificInterfaceShared.Controls.ucFieldPicker, ByVal value As Object) _
            Handles m_fieldpickBackup.OnFieldPicked

            Me.InsertText(Me.m_tbBackupMask, "{" & value.ToString & "}")
            Me.UpdateControls()

        End Sub

        Private Sub OnBackupMaskChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tbBackupMask.TextChanged, m_tbOutput.TextChanged
            Me.UpdateControls()
        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Sub ClearFileList(ByVal fileList As ArrayList)

            If (fileList Is Nothing) Then Return

            If MessageBox.Show(My.Resources.GENERIC_PROMPT_CLEAR_MRU, Me.Text, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
                ' Clear confirmed
                fileList.Clear()
                ' This is a temporary solution to avoid returning null reference.
                fileList.Add(New System.Object)
            End If

        End Sub

        Private Sub UpdateControls()

            Dim bHasSuppressedPrompts As Boolean = (Not String.IsNullOrEmpty(My.Settings.SuppressedOverwritePrompts))
            Dim bCanCheckExEUpdate As Boolean = False
            Dim bHasMRU As Boolean = (My.Settings.MdbRecentlyUsedList.Count > 0)

            Me.m_btnResetOverwritePrompts.Enabled = bHasSuppressedPrompts
            Me.m_lblResetOverwritePrompts.Enabled = bHasSuppressedPrompts

            Me.m_cbCheckEwE6.Enabled = bCanCheckExEUpdate

            Me.m_btnClearMRU.Enabled = bHasMRU

            Dim strSample As String = ""
            If Not cPathUtility.ResolvePath(Me.m_tbBackupMask.Text, Me.m_uic.Core, strSample) Then
                cPathUtility.ResolvePath(Me.m_tbBackupMask.Text, "model", "C:\models", ".ext", "6.version", strSample)
            End If
            Me.m_lblSample.Text = strSample

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
