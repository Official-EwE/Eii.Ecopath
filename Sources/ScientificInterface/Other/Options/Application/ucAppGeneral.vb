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

            ' Disable button if there is nothing to clear
            Me.m_btnClear.Enabled = (My.Settings.MdbRecentlyUsedList.Count <= 1)

            Me.m_nudMRU.Value = CInt(Math.Min(Me.m_nudMRU.Maximum, _
                                     Math.Max(Me.m_nudMRU.Minimum, My.Settings.MdbRecentlyUsedCount)))

            Me.m_nudMaxNumMessages.Value = CInt(Math.Min(Me.m_nudMaxNumMessages.Maximum, _
                                                Math.Max(Me.m_nudMaxNumMessages.Minimum, My.Settings.FeedbackMessageLogSize)))

        End Sub

#End Region ' Overrides

#Region " Public access "

        Public Sub Save()
            My.Settings.MdbRecentlyUsedCount = CInt(Me.m_nudMRU.Value)
            My.Settings.FeedbackMessageLogSize = CInt(Me.m_nudMaxNumMessages.Value)
        End Sub

#End Region ' Public access

#Region " Event handlers "

        Private Sub btnClearMRU_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnClear.Click
            Me.ClearFileList(My.Settings.MdbRecentlyUsedList)
            Me.m_btnClear.Enabled = False
        End Sub

#End Region ' Event handlers

#Region " Internals "

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

#End Region ' Internals

    End Class

End Namespace
