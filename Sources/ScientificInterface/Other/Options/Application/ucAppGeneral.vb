'==============================================================================
'
' $Log: ucAppGeneral.vb,v $
' Revision 1.3  2009/03/27 13:39:25  jeroens
' Limited number of status messages
'
' Revision 1.2  2008/12/15 15:56:02  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:32:09  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.IO
Imports WeifenLuo.WinFormsUI

#End Region

Namespace Other

    Public Class ucAppGeneral

#Region "Private variables"

        Private m_bCritPop As Boolean = True
        Private m_bCritStat As Boolean = True
        Private m_bWarnPop As Boolean = False
        Private m_bWarnStat As Boolean = True
        Private m_bInfoPop As Boolean = False
        Private m_bInfoStat As Boolean = True

#End Region

#Region "Constructors"

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ' Set the initial values for displaying messages

            'Critical messages
            m_bCritPop = My.Settings.FeedbackCriticalPopup
            m_bCritStat = My.Settings.FeedbackCriticalStatusMessage

            'Warning messages
            m_bWarnPop = My.Settings.FeedbackWarningPopup
            m_bWarnStat = My.Settings.FeedbackWarningStatusMessage

            'Information messages
            m_bInfoPop = My.Settings.FeedbackInformationPopup
            m_bInfoStat = My.Settings.FeedbackInformationStatusMessage

        End Sub

#End Region

#Region "Event handlers"

        Private Sub ucGeneral_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim strPath As String = My.Settings.ContentLayoutSaveDirectory

            ' Disable button if there is nothing to clear
            btnClear.Enabled = (My.Settings.MdbRecentlyUsedList.Count <= 1 )

            ' Set up the content layout checkbox status
            cbSaveLayout.Checked = (My.Settings.SaveContentLayout = True)

            'Get the directory where the layout files are stored
            If Not Directory.Exists(strPath) Then
                strPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            End If

            My.Settings.ContentLayoutSaveDirectory = strPath

            ' Get the directory name from setting and display it in the text box..
            txbSaveDirectory.Text = strPath

            ' Enable/disable the RemoveAll button
            btnRemoveAll.Enabled = False
            For Each f As String In Directory.GetFiles(strPath)
                If f.EndsWith(".config") Then
                    btnRemoveAll.Enabled = True
                    Exit For
                End If
            Next

            InitMessageControls()
            UpdateMessageControls()

        End Sub

        Private Sub btnClearMRU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClear.Click
            ClearFileList(My.Settings.MdbRecentlyUsedList)
            btnClear.Enabled = False
        End Sub

        Private Sub ClearFileList(ByRef fileList As ArrayList)

            If MessageBox.Show(My.Resources.GENERIC_PROMPT_CLEAR_MRU, Me.Text, _
                MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
                ' Clear confirmed
                fileList.Clear()

                ' This is a temporary solution to avoid returning null reference.
                fileList.Add(New System.Object)

                'delete the configuration files in the folder
            End If

        End Sub

        Public Sub Save()
            My.Settings.SaveContentLayout = cbSaveLayout.Checked
            My.Settings.ContentLayoutSaveDirectory = txbSaveDirectory.Text
            Me.SaveMsgSettings()
        End Sub

        Private Sub SaveMsgSettings()

            'Critical messages
            My.Settings.FeedbackCriticalPopup = m_bCritPop
            My.Settings.FeedbackCriticalStatusMessage = m_bCritStat

            'Warning messages
            My.Settings.FeedbackWarningPopup = m_bWarnPop
            My.Settings.FeedbackWarningStatusMessage = m_bWarnStat

            'Information messages
            My.Settings.FeedbackInformationPopup = m_bInfoPop
            My.Settings.FeedbackInformationStatusMessage = m_bInfoStat

            My.Settings.FeedbackMessageLogSize = CInt(Me.m_nudMaxNumMessages.Value)

        End Sub

        Private Sub btnSaveLocation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveLocation.Click

            Dim folderBrowerDlg As New FolderBrowserDialog
            folderBrowerDlg.Description = My.Resources.SELECT_DEFAULT_LAYOUT_DIRECTORY_MSG
            folderBrowerDlg.ShowNewFolderButton = True
            'folderBrowerDlg.RootFolder = Environment.SpecialFolder.Personal

            Dim result As DialogResult = folderBrowerDlg.ShowDialog()

            If result = DialogResult.OK Then
                txbSaveDirectory.Text = folderBrowerDlg.SelectedPath
            End If

        End Sub

        Private Sub btnRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveAll.Click

            Dim dstr As String = My.Settings.ContentLayoutSaveDirectory

            ' The directory is not a valid path
            If Not Directory.Exists(dstr) Then Return

            For Each f As String In Directory.GetFiles(dstr)
                If f.EndsWith(".config") Then
                    File.Delete(f)
                End If
            Next

            btnRemoveAll.Enabled = False
        End Sub

        Private Sub InitMessageControls()

            Me.cbCritical.Sorted = False
            With Me.cbCritical.Items
                .Clear()
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP)
                .Add(My.Resources.OPTIONS_FEEDBACK_STATUS)
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP_AND_STATUS)
            End With

            Me.cbWarning.Sorted = False
            With Me.cbWarning.Items
                .Clear()
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP)
                .Add(My.Resources.OPTIONS_FEEDBACK_STATUS)
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP_AND_STATUS)
            End With

            Me.cbInformation.Sorted = False
            With Me.cbInformation.Items
                .Clear()
                .Add(My.Resources.OPTIONS_FEEDBACK_NONE)
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP)
                .Add(My.Resources.OPTIONS_FEEDBACK_STATUS)
                .Add(My.Resources.OPTIONS_FEEDBACK_POPUP_AND_STATUS)
            End With

        End Sub

        Private Sub UpdateMessageControls()

            Dim iIndex As Integer = 0

            ' Critical
            If m_bCritStat Then iIndex = CInt(IIf(m_bCritPop, 2, 1)) Else m_bCritPop = True : iIndex = 0
            Me.cbCritical.SelectedIndex = iIndex

            ' Warning
            If m_bWarnStat Then iIndex = CInt(IIf(m_bWarnPop, 2, 1)) Else m_bWarnPop = True : iIndex = 0
            Me.cbWarning.SelectedIndex = iIndex

            ' Information
            If m_bInfoStat Then iIndex = CInt(IIf(m_bInfoPop, 3, 2)) Else iIndex = CInt(IIf(m_bInfoPop, 1, 0))
            Me.cbInformation.SelectedIndex = iIndex

            Me.m_nudMaxNumMessages.Value = CInt(Math.Min(Me.m_nudMaxNumMessages.Maximum, _
                                                Math.Max(Me.m_nudMaxNumMessages.Minimum, My.Settings.FeedbackMessageLogSize)))

        End Sub

        Private Sub cbCritical_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbCritical.SelectedIndexChanged
            Select Case cbCritical.SelectedIndex
                Case 0
                    m_bCritPop = True
                    m_bCritStat = False
                Case 1
                    m_bCritPop = False
                    m_bCritStat = True
                Case 2
                    m_bCritPop = True
                    m_bCritStat = True
                Case Else
                    Debug.Assert(False)
            End Select
        End Sub

        Private Sub cbWarning_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbWarning.SelectedIndexChanged
            Select Case cbWarning.SelectedIndex
                Case 0
                    m_bWarnPop = True
                    m_bWarnStat = False
                Case 1
                    m_bWarnPop = False
                    m_bWarnStat = True
                Case 2
                    m_bWarnPop = True
                    m_bWarnStat = True
                Case Else
                    Debug.Assert(False)
            End Select
        End Sub

        Private Sub cbInformation_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbInformation.SelectedIndexChanged
            Select Case cbInformation.SelectedIndex
                Case 0
                    m_bInfoPop = False
                    m_bInfoStat = False
                Case 1
                    m_bInfoPop = True
                    m_bInfoStat = False
                Case 2
                    m_bInfoPop = False
                    m_bInfoStat = True
                Case 3
                    m_bInfoPop = True
                    m_bInfoStat = True
                Case Else
                    Debug.Assert(False)
            End Select
        End Sub

#End Region

    End Class

End Namespace
