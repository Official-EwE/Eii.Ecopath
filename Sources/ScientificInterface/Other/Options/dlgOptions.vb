'==============================================================================
'
' $Log: dlgOptions.vb,v $
' Revision 1.2  2008/12/15 15:54:30  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:32:10  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.24  2008/08/14 01:52:18  jeroens
' Not mybase!
'
' Revision 1.23  2008/07/29 23:43:48  jeroens
' Settings saved after all pages have been applied
'
' Revision 1.22  2008/07/18 18:00:48  jeroens
' Plugin changes Applied on [OK]
'
' Revision 1.21  2008/07/16 13:54:58  jeroens
' Ugh, adding pages manually did not cause them to be disposed properly
'
' Revision 1.20  2008/07/10 18:19:28  jeroens
' Removed units references
'
' Revision 1.19  2007/11/23 20:05:45  jeroens
' + Added PlugIns page
'
' Revision 1.18  2007/10/30 23:05:04  jeroens
' * Woops
'
' Revision 1.17  2007/10/30 22:51:19  jeroens
' - Discontinued Model settings
'
' Revision 1.16  2007/10/03 02:32:54  jeroens
' no message
'
' Revision 1.15  2007/10/03 01:54:29  jeroens
' * Reworked styleguide, colormanager
'
' Revision 1.14  2007/06/14 14:53:54  jeroens
' + Added error page when model not loaded
' + Added General model parameters page
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Other

    ''' <summary>
    ''' The class for setting EwE6 application scope settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Class dlgOptions

#Region " Private variables "

        ''' <summary></summary>
        Private m_core As cCore = cCore.GetInstance()
        ''' <summary></summary>
        Private m_ucAppColors As ucAppColors
        ''' <summary></summary>
        Private m_ucAppGeneral As ucAppGeneral
        ''' <summary></summary>
        Private m_ucAppPlugins As ucAppPlugins
        ''' <summary>Current page.</summary>
        Private m_ucCurrent As UserControl = Nothing

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            Me.InitializeComponent()

            'Initialize the color option control
            Me.m_ucAppColors = New ucAppColors
            Me.m_ucAppColors.Dock = DockStyle.Fill

            'Initialize the general option control
            Me.m_ucAppGeneral = New ucAppGeneral
            Me.m_ucAppGeneral.Dock = DockStyle.Fill

            Me.m_ucAppPlugins = New ucAppPlugins
            Me.m_ucAppPlugins.Dock = DockStyle.Fill

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.m_ucAppPlugins.Apply()
            Me.m_ucAppColors.SaveColorOptions()
            Me.m_ucAppGeneral.SaveGeneralOptions()

            ' Save all settings
            My.Settings.Save()
            Me.Close()

        End Sub

        Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
            ' Cancel the option setting
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        End Sub

        Private Sub tvOptions_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvOptions.AfterSelect

            Dim ucPage As UserControl = Me.m_ucAppGeneral

            Me.SuspendLayout()

            Select Case e.Node.Name
                Case "ndGeneral"
                    ucPage = Me.m_ucAppGeneral
                Case "ndColors"
                    ucPage = Me.m_ucAppColors
                Case "ndPlugins"
                    ucPage = Me.m_ucAppPlugins

                Case Else
                    Debug.Assert(False, "Invalid node selected")
            End Select

            ' Optimization
            If Object.ReferenceEquals(ucPage, Me.m_ucCurrent) Then Return
            ' Set new page
            Me.m_ucCurrent = ucPage
            ' Yo
            Me.plOption.Controls.Clear()
            Me.plOption.Controls.Add(ucPage)

            Me.ResumeLayout()

        End Sub

        Private Sub dlgOptions_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
                Handles Me.FormClosing

            ' Bye
            Me.plOption.Controls.Clear()
            ' Manually dispose
            Me.m_ucCurrent.Dispose()
            Me.m_ucAppColors.Dispose()
            Me.m_ucAppGeneral.Dispose()
            Me.m_ucAppPlugins.Dispose()

        End Sub

#End Region ' Event handlers

    End Class

End Namespace