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
        Private m_ucOptionsColors As ucOptionsColors
        ''' <summary></summary>
        Private m_ucOptionsGeneral As ucOptionsGeneral
        ''' <summary></summary>
        Private m_ucOptionsPresentation As ucOptionsPresentation
        ''' <summary></summary>
        Private m_ucOptionsPlugins As ucOptionsPlugins
        ''' <summary></summary>
        Private m_ucOptionsGraphsCharts As ucOptionsGraphs
        ''' <summary>Current page.</summary>
        Private m_ucCurrent As UserControl = Nothing

        Private m_bHasFiredRestartPrompt As Boolean = False

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_PLEASE_WAIT)

            Me.InitializeComponent()

            For Each nodeChild As TreeNode In Me.m_tvOptions.Nodes
                Me.ExpandNode(nodeChild)
            Next

            Me.m_uic = uic
            Me.m_tvOptions.ExpandAll()

            Me.m_ucOptionsColors = New ucOptionsColors(uic)
            Me.m_ucOptionsColors.Dock = DockStyle.Fill

            Me.m_ucOptionsGeneral = New ucOptionsGeneral(uic)
            Me.m_ucOptionsGeneral.Dock = DockStyle.Fill

            Me.m_ucOptionsPresentation = New ucOptionsPresentation(uic)
            Me.m_ucOptionsPresentation.Dock = DockStyle.Fill

            Me.m_ucOptionsPlugins = New ucOptionsPlugins(uic)
            Me.m_ucOptionsPlugins.Dock = DockStyle.Fill

            Me.m_ucOptionsGraphsCharts = New ucOptionsGraphs(uic)
            Me.m_ucOptionsGraphsCharts.Dock = DockStyle.Fill

            Me.SelectPage("")

            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        End Sub

#End Region ' Constructor

#Region " Internals "

        Private Sub Apply()

            Dim bRestart As Boolean = False

            bRestart = bRestart Or (Me.m_ucOptionsGeneral.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucOptionsPlugins.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucOptionsColors.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucOptionsPresentation.Apply() = IOptionsPage.eApplyResultType.Success_restart)
            bRestart = bRestart Or (Me.m_ucOptionsGraphsCharts.Apply() = IOptionsPage.eApplyResultType.Success_restart)

            ' Need to restart for changes to be effective?
            If bRestart And Not Me.m_bHasFiredRestartPrompt Then
                ' #Yeah: notify user
                MsgBox(My.Resources.PROMPT_CHANGES_RESTART, MsgBoxStyle.Information)
                Me.m_bHasFiredRestartPrompt = True
            End If

        End Sub

        Private Sub SelectPage(ByVal strPage As String)
            Dim ucPage As UserControl = Me.m_ucOptionsGeneral

            Me.SuspendLayout()

            Select Case strPage
                Case "", "ndGeneral"
                    ucPage = Me.m_ucOptionsGeneral
                Case "ndPresentation"
                    ucPage = Me.m_ucOptionsPresentation
                Case "ndDisplay", "ndColors"
                    ucPage = Me.m_ucOptionsColors
                Case "ndGraphCharts"
                    ucPage = Me.m_ucOptionsGraphsCharts
                Case "ndPlugins"
                    ucPage = Me.m_ucOptionsPlugins

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

        Private Sub ExpandNode(ByVal node As TreeNode)
            For Each nodeChild As TreeNode In node.Nodes
                Me.ExpandNode(nodeChild)
            Next
            node.Expand()
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
            Me.m_ucOptionsColors.Dispose()
            Me.m_ucOptionsGeneral.Dispose()
            Me.m_ucOptionsPresentation.Dispose()
            Me.m_ucOptionsPlugins.Dispose()
            Me.m_ucOptionsGraphsCharts.Dispose()

        End Sub

#End Region ' Event handlers

    End Class

End Namespace