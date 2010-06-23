#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Other

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Dialog; implements the shell for the Options interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class dlgOptions

#Region " Private variables "

        ''' <summary></summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary></summary>
        Private m_ucAppColors As ucOptionsColors
        ''' <summary></summary>
        Private m_ucAppGeneral As ucOptionsGeneral
        ''' <summary></summary>
        Private m_ucAppPlugins As ucOptionsPlugins
        ''' <summary></summary>
        Private m_ucAppGraphsCharts As ucOptionsGraphs
        ''' <summary>Current page.</summary>
        Private m_ucCurrent As UserControl = Nothing

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

            Me.InitializeComponent()

            Me.m_uic = uic
            Me.m_tvOptions.ExpandAll()

            Me.m_ucAppColors = New ucOptionsColors(uic)
            Me.m_ucAppColors.Dock = DockStyle.Fill

            Me.m_ucAppGeneral = New ucOptionsGeneral(uic)
            Me.m_ucAppGeneral.Dock = DockStyle.Fill

            Me.m_ucAppPlugins = New ucOptionsPlugins(uic)
            Me.m_ucAppPlugins.Dock = DockStyle.Fill

            Me.m_ucAppGraphsCharts = New ucOptionsGraphs(uic)
            Me.m_ucAppGraphsCharts.Dock = DockStyle.Fill

            Me.SelectPage("")

            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        End Sub

#End Region ' Constructor

#Region " Internals "

        Private Sub Apply()

            Dim bRestart As Boolean = False

            bRestart = bRestart Or (Me.m_ucAppGeneral.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucAppPlugins.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucAppColors.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucAppGraphsCharts.Apply() = IOptionsPage.eApplyResultType.Success_restart)

            My.Settings.Save()

            ' Need to restart for changes to be effective?
            If bRestart Then
                ' #Yeah: notify user
                MsgBox(My.Resources.PROMPT_CHANGES_RESTART, MsgBoxStyle.Information)
            End If

        End Sub

        Private Sub SelectPage(ByVal strPage As String)
            Dim ucPage As UserControl = Me.m_ucAppGeneral

            Me.SuspendLayout()

            Select Case strPage
                Case "", "ndGeneral"
                    ucPage = Me.m_ucAppGeneral
                Case "ndDisplay", "ndColors"
                    ucPage = Me.m_ucAppColors
                Case "ndGraphCharts"
                    ucPage = Me.m_ucAppGraphsCharts
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
            Me.m_scContent.Panel2.Controls.Clear()
            Me.m_scContent.Panel2.Controls.Add(ucPage)

            Me.ResumeLayout()
        End Sub

#End Region ' Internals

#Region " Event handlers "

        Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnOk.Click

            Me.Apply()
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnApply(ByVal sender As System.Object, ByVal e As System.EventArgs) _
                Handles m_btnApply.Click

            Me.Apply()

        End Sub

        Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnCancel.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnSelectedNode(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
            Handles m_tvOptions.AfterSelect

            Me.SelectPage(e.Node.Name)

        End Sub

        Private Sub dlgOptions_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
                Handles Me.FormClosing

            ' Bye
            Me.m_scContent.Panel2.Controls.Clear()
            ' Manually dispose
            Me.m_ucCurrent = Nothing
            Me.m_ucAppColors.Dispose()
            Me.m_ucAppGeneral.Dispose()
            Me.m_ucAppPlugins.Dispose()
            Me.m_ucAppGraphsCharts.Dispose()

        End Sub

#End Region ' Event handlers

    End Class

End Namespace