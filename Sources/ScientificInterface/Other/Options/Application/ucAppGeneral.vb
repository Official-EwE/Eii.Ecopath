#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports WeifenLuo.WinFormsUI

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control; implements the Options > General settings interface
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucAppGeneral

#Region " Constructors "

        Public Sub New(ByVal uic As cUIContext)
            Me.InitializeComponent()
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
                                                Math.Max(Me.m_nudMaxNumMessages.Minimum, My.Settings.FeedbackMessageLogSize)))

            Me.m_cbCheckEwE6.Checked = False
            Me.m_cbDownloadUpdates.Checked = My.Settings.AutoUpdatePlugins

            Me.UpdateControls()

        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Sub Save()
            My.Settings.MdbRecentlyUsedCount = CInt(Me.m_nudMRU.Value)
            My.Settings.FeedbackMessageLogSize = CInt(Me.m_nudMaxNumMessages.Value)
            My.Settings.AutoUpdatePlugins = Me.m_cbDownloadUpdates.Checked
        End Sub

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

        End Sub

#End Region ' Internals

    End Class

End Namespace
