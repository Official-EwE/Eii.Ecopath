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
        ''' <summary></summary>
        Private m_ucAppGraphsCharts As ucAppGraphs
        ''' <summary>Current page.</summary>
        Private m_ucCurrent As UserControl = Nothing

#End Region ' Private variables

#Region " Constructor "

        Public Sub New()

            cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_PLEASE_WAIT, TriState.True)

            Me.InitializeComponent()

            Me.m_tvOptions.ExpandAll()

            Me.m_ucAppColors = New ucAppColors()
            Me.m_ucAppColors.Dock = DockStyle.Fill

            Me.m_ucAppGeneral = New ucAppGeneral()
            Me.m_ucAppGeneral.Dock = DockStyle.Fill

            Me.m_ucAppPlugins = New ucAppPlugins()
            Me.m_ucAppPlugins.Dock = DockStyle.Fill

            Me.m_ucAppGraphsCharts = New ucAppGraphs()
            Me.m_ucAppGraphsCharts.Dock = DockStyle.Fill

            Me.SelectPage("")

            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        End Sub

#End Region ' Constructor

#Region " Internals "

        Private Sub Apply()

            Me.m_ucAppPlugins.Save()
            Me.m_ucAppColors.Save()
            Me.m_ucAppGraphsCharts.Save()
            Me.m_ucAppGeneral.Save()
            My.Settings.Save()

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