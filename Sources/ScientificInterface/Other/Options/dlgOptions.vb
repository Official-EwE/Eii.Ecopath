#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

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
        ''' <summary>List of active pages.</summary>
        Private m_lPages As New List(Of IOptionsPage)
        ''' <summary>Current page.</summary>
        Private m_pageCurrent As IOptionsPage = Nothing

        ' ToDo: track changes in pages, and only show prompts after changes occurred. Not very important right now.
        Private m_bHasFiredPrompt As Boolean = False

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext)

            Me.m_uic = uic

            Me.InitializeComponent()
            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core, My.Resources.STATUS_PLEASE_WAIT)

            For Each nodeChild As TreeNode In Me.m_tvOptions.Nodes
                Me.ExpandNode(nodeChild)
            Next

            Me.m_tvOptions.ExpandAll()

            Me.AddPage(GetType(ucOptionsGeneral))
            Me.AddPage(GetType(ucOptionsPresentation))
            Me.AddPage(GetType(ucOptionsColors))
            Me.AddPage(GetType(ucOptionsPresentation))
            Me.AddPage(GetType(ucOptionsPlugins))
            Me.SelectPage("")

            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        End Sub

#End Region ' Constructor

#Region " Internals "

        Private Sub AddPage(ByVal t As Type)

            ' Sanity check
            Debug.Assert(GetType(IOptionsPage).IsAssignableFrom(t))
            Debug.Assert(GetType(Control).IsAssignableFrom(t))

            Dim optionspage As IOptionsPage = DirectCast(Activator.CreateInstance(t, New Object() {Me.m_uic}), IOptionsPage)
            DirectCast(optionspage, Control).Dock = DockStyle.Fill

            Me.m_lPages.Add(optionspage)

        End Sub

        Private Function GetPage(ByVal t As Type) As IOptionsPage

            For Each optionspage As IOptionsPage In Me.m_lPages
                If optionspage.GetType().Equals(t) Then
                    Return optionspage
                End If
            Next
            Return Nothing

        End Function

        Private Sub Apply()

            Dim msg As cMessage = Nothing
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            For Each optionspage As IOptionsPage In Me.m_lPages
                result = DirectCast(Math.Max(result, optionspage.Apply()), IOptionsPage.eApplyResultType)
            Next

            Select Case result
                Case IOptionsPage.eApplyResultType.Success
                    msg = New cMessage(SharedResources.PROMPT_OPTIONS_APPLIED_SUCCESS, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
                    Me.m_bHasFiredPrompt = False
                Case IOptionsPage.eApplyResultType.Success_restart
                    msg = New cMessage(SharedResources.PROMPT_REQUIRES_RESTART, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)
                Case IOptionsPage.eApplyResultType.Success_administrator
                    msg = New cMessage(SharedResources.PROMPT_REQUIRES_ADMINISTRATOR, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Warning)
                Case IOptionsPage.eApplyResultType.Failed
                    msg = New cMessage(SharedResources.PROMPT_OPTIONS_APPLIED_FAILED, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
                    Me.m_bHasFiredPrompt = False
            End Select

            ' Need to send message?
            If (msg IsNot Nothing) And (Me.m_bHasFiredPrompt = False) Then
                ' #Yes: notify user
                Me.m_uic.Core.Messages.SendMessage(msg)
                Me.m_bHasFiredPrompt = True
            End If

        End Sub

        Private Sub SelectPage(ByVal strPage As String)

            Dim page As IOptionsPage = Me.GetPage(GetType(ucOptionsGeneral))

            Me.SuspendLayout()

            Select Case strPage
                Case "", "ndGeneral"
                    ' NOP
                Case "ndPresentation"
                    page = Me.GetPage(GetType(ucOptionsPresentation))
                Case "ndDisplay", "ndColors"
                    page = Me.GetPage(GetType(ucOptionsColors))
                Case "ndGraphCharts"
                    page = Me.GetPage(GetType(ucOptionsGraphs))
                Case "ndPlugins"
                    page = Me.GetPage(GetType(ucOptionsPlugins))
                Case Else
                    Debug.Assert(False, "Invalid node selected")
            End Select

            ' Optimization
            If Object.ReferenceEquals(page, Me.m_pageCurrent) Then Return
            ' Set new page
            Me.m_pageCurrent = page
            ' Yo
            Me.m_scContent.Panel2.Controls.Clear()
            Me.m_scContent.Panel2.Controls.Add(DirectCast(Me.m_pageCurrent, Control))

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
            Me.m_pageCurrent = Nothing

            ' Manually dispose
            For Each optionspage As IOptionsPage In Me.m_lPages
                DirectCast(optionspage, Control).Dispose()
            Next
            Me.m_lPages.Clear()

        End Sub

#End Region ' Event handlers

    End Class

End Namespace