#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports WeifenLuo.WinFormsUI
Imports EwECore

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
            Me.m_tsddFields.DropDown.Items.Clear()
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

            ' Backup path masks
            For Each ph As cPathUtility.ePathPlaceholderTypes In [Enum].GetValues(GetType(cPathUtility.ePathPlaceholderTypes))
                Me.m_tsddFields.DropDown.Items.Add(ph.ToString, Nothing, AddressOf OnInsertField)
            Next
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

        Private Sub OnInsertField(ByVal sender As Object, ByVal e As EventArgs)

            Dim strSrc As String = Me.m_tbBackupMask.Text
            Dim strDest As String
            Dim iSelStart As Integer = Me.m_tbBackupMask.SelectionStart
            Dim iSelLen As Integer = Me.m_tbBackupMask.SelectionLength
            Dim item As ToolStripItem = DirectCast(sender, ToolStripItem)
            Dim strItemText As String = "{" & item.Text & "}"
            Dim iItemLen As Integer = strItemText.Length

            If (iSelLen = 0) Then
                strDest = strSrc & strItemText
                iSelStart = strDest.Length
            Else
                strDest = strSrc.Substring(0, iSelStart) & strItemText & strSrc.Substring(iSelStart + iSelLen)
                iSelStart += iItemLen
            End If

            Me.m_tbBackupMask.Text = strDest
            Me.m_tbBackupMask.SelectionStart = iSelStart
            Me.m_tbBackupMask.SelectionLength = 0

            Me.UpdateControls()

        End Sub

        Private Sub OnBackupMaskChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_tbBackupMask.TextChanged
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

#End Region ' Internals

    End Class

End Namespace
