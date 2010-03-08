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

            Dim strPath As String = My.Settings.ContentLayoutSaveDirectory

            ' Disable button if there is nothing to clear
            m_btnClear.Enabled = (My.Settings.MdbRecentlyUsedList.Count <= 1)

            ' Set up the content layout checkbox status
            m_cbSaveLayout.Checked = (My.Settings.SaveContentLayout = True)

            'Get the directory where the layout files are stored
            If Not Directory.Exists(strPath) Then
                strPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            End If

            My.Settings.ContentLayoutSaveDirectory = strPath

            ' Get the directory name from setting and display it in the text box..
            m_txbSaveDirectory.Text = strPath

            ' Enable/disable the RemoveAll button
            m_btnRemoveAll.Enabled = False
            For Each f As String In Directory.GetFiles(strPath)
                If f.EndsWith(".config") Then
                    m_btnRemoveAll.Enabled = True
                    Exit For
                End If
            Next

            Me.m_nudMaxNumMessages.Value = CInt(Math.Min(Me.m_nudMaxNumMessages.Maximum, _
                                                Math.Max(Me.m_nudMaxNumMessages.Minimum, My.Settings.FeedbackMessageLogSize)))

        End Sub

#End Region ' Overrides

#Region "Event handlers"

        Private Sub btnClearMRU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClear.Click
            Me.ClearFileList(My.Settings.MdbRecentlyUsedList)
            Me.m_btnClear.Enabled = False
        End Sub

        Private Sub ClearFileList(ByVal fileList As ArrayList)

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
            My.Settings.SaveContentLayout = m_cbSaveLayout.Checked
            My.Settings.ContentLayoutSaveDirectory = m_txbSaveDirectory.Text
            My.Settings.FeedbackMessageLogSize = CInt(Me.m_nudMaxNumMessages.Value)
        End Sub

        Private Sub btnSaveLocation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnSaveLocation.Click

            Dim folderBrowerDlg As New FolderBrowserDialog()

            folderBrowerDlg.Description = My.Resources.SELECT_DEFAULT_LAYOUT_DIRECTORY_MSG
            folderBrowerDlg.ShowNewFolderButton = True
            'folderBrowerDlg.RootFolder = Environment.SpecialFolder.Personal

            Dim result As DialogResult = folderBrowerDlg.ShowDialog()

            If result = DialogResult.OK Then
                m_txbSaveDirectory.Text = folderBrowerDlg.SelectedPath
            End If

        End Sub

        Private Sub btnRemoveAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnRemoveAll.Click

            Dim dstr As String = My.Settings.ContentLayoutSaveDirectory

            ' The directory is not a valid path
            If Not Directory.Exists(dstr) Then Return

            For Each f As String In Directory.GetFiles(dstr)
                If f.EndsWith(".config") Then
                    File.Delete(f)
                End If
            Next
            m_btnRemoveAll.Enabled = False

        End Sub

#End Region

    End Class

End Namespace
