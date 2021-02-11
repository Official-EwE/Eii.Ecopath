' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

' ToDo: integrate option pages provided through plug-ins

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEPlugin
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
        ''' <summary>Page to show upon startup</summary>
        Private m_strVerb As String = ""

        ' ToDo: track changes in pages, and only show prompts after changes occurred. Not very important right now.
        Private m_bHasFiredPrompt As Boolean = False

        Private m_dtNodes As New Dictionary(Of String, Type)

#End Region ' Private variables

#Region " Constructor "

        Public Sub New(uic As cUIContext, Optional verb As String = "")

            Me.m_uic = uic
            Me.InitializeComponent()
            Me.m_strVerb = verb

        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Create nodes
            Me.m_tvOptions.Nodes.Clear()

            Me.m_tvOptions.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_GENERAL, eApplicationOptionTypes.General.ToString(), GetType(ucOptionsGeneral)))
            Me.m_tvOptions.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_FILEMANAGEMENT, eApplicationOptionTypes.Autosave.ToString(), GetType(ucOptionsFileManagement)))
            Me.m_tvOptions.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_SPATTEMP, eApplicationOptionTypes.SpatialTemporal.ToString(), GetType(ucOptionsSpatialTemporal)))

            Dim tnAppearance As New TreeNode(My.Resources.OPTIONS_PAGE_APPEARANCE)
            tnAppearance.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_WINDOW, eApplicationOptionTypes.Window.ToString(), GetType(ucOptionsPresentation)))
            tnAppearance.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_COLORS, eApplicationOptionTypes.Colours.ToString(), GetType(ucOptionsStatusColors)))
            tnAppearance.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_GRAPHS, eApplicationOptionTypes.Fonts.ToString(), GetType(ucOptionsGraphs)))
            tnAppearance.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_MAPS, eApplicationOptionTypes.ReferenceMaps.ToString(), GetType(ucOptionsMap)))
            tnAppearance.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_PEDIGREE, eApplicationOptionTypes.Pedigree.ToString(), GetType(ucOptionsPedigree)))
            Me.m_tvOptions.Nodes.Add(tnAppearance)

            Dim tnPlugins As New TreeNode(My.Resources.OPTIONS_PAGE_PLUGINS)
            tnPlugins.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_INSTALLED, eApplicationOptionTypes.Plugins.ToString(), GetType(ucOptionsPlugins)))
            tnPlugins.Nodes.Add(Me.CreateNode(My.Resources.OPTIONS_PAGE_AUTORUN, eApplicationOptionTypes.AutoRun.ToString(), GetType(ucOptionsAutoRun)))

            ' Add options pages provided by plug-ins
            Dim pm As cPluginManager = Me.m_uic.Core.PluginManager
            If (pm IsNot Nothing) Then
                ' ToDo: sort
                For Each pi As IPlugin In pm.GetPlugins(GetType(IEwEOptionsPlugin))
                    Dim opt As IEwEOptionsPlugin = DirectCast(pi, IEwEOptionsPlugin)
                    Dim page As Control = opt.GetConfigUI()
                    Me.m_lPages.Add(DirectCast(page, IOptionsPage))
                    tnPlugins.Nodes.Add(Me.CreateNode(opt.Label, pi.Name, page.GetType()))
                Next
            End If
            Me.m_tvOptions.Nodes.Add(tnPlugins)

            ' Done
            Me.m_tvOptions.ExpandAll()

            'Me.SelectPage(Me.GetPage(Me.m_strVerb))
            Me.SelectNode(Me.m_strVerb)

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            ' Bye
            Me.m_scContent.Panel2.Controls.Clear()
            Me.m_pageCurrent = Nothing

            ' Manually dispose
            For Each optionspage As IOptionsPage In Me.m_lPages
                DirectCast(optionspage, Control).Dispose()
            Next
            Me.m_lPages.Clear()

            MyBase.OnFormClosed(e)

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Private Sub OnOK(sender As System.Object, e As System.EventArgs) _
            Handles m_btnOk.Click

            Try
                Me.Apply()
            Catch ex As Exception
                cLog.Write(ex, "dlgOptions::OnOK")
            End Try
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()

        End Sub

        Private Sub OnSetDefaults(sender As System.Object, e As System.EventArgs) _
                Handles m_btnSetDefaults.Click

            Try
                Me.SetDefaults()
            Catch ex As Exception
                cLog.Write(ex, "dlgOptions::OnSetDefaults")
            End Try

        End Sub

        Private Sub OnApply(sender As System.Object, e As System.EventArgs) _
                Handles m_btnApply.Click

            Try
                Me.Apply()
            Catch ex As Exception
                cLog.Write(ex, "dlgOptions::OnApply")
            End Try

        End Sub

        Private Sub OnCancel(sender As System.Object, e As System.EventArgs) _
            Handles m_btnCancel.Click

            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()

        End Sub

        Private Sub OnSelectedNode(sender As System.Object, e As System.Windows.Forms.TreeViewEventArgs) _
            Handles m_tvOptions.AfterSelect

            If (e.Node Is Nothing) Then Return

            Dim n As TreeNode = e.Node

            Try
                Dim bDone As Boolean = False
                Dim page As IOptionsPage = Nothing

                ' Select node, or first child node with content
                While Not bDone
                    Dim strVerb As String = CStr(n.Tag)
                    page = Me.GetPage(strVerb)
                    bDone = True
                    If (page Is Nothing) Then
                        If n.Nodes.Count > 0 Then
                            n = n.Nodes(0)
                            bDone = False
                        End If
                    End If
                End While
                Me.SelectPage(page)

            Catch ex As Exception
                cLog.Write(ex, "dlgOptions::OnSelectedNode(" & e.Node.Name & ")")
            End Try

        End Sub

#End Region ' Event handlers

#Region " Internals "

        Private Function CreateNode(strLabel As String, strVerb As String, type As Type) As TreeNode
            Dim tn As New TreeNode(strLabel) With {.Tag = strVerb}
            Me.m_dtNodes(strVerb) = type
            Return tn
        End Function

        Private Sub SelectNode(strVerb As String)

            If (Me.m_tvOptions.GetNodeCount(False) = 0) Then Return

            Dim n As TreeNode = Me.FindNodeByVerb(strVerb)
            If (n Is Nothing) Then n = Me.m_tvOptions.Nodes(0)
            Me.m_tvOptions.SelectedNode = n

        End Sub

        ''' <summary>
        ''' Recursively locates a node for the given verb
        ''' </summary>
        ''' <param name="strVerb"></param>
        ''' <param name="nStart"></param>
        ''' <returns></returns>
        Private Function FindNodeByVerb(strVerb As String, Optional nStart As TreeNode = Nothing) As TreeNode

            Dim nodes As TreeNodeCollection = If(nStart Is Nothing, Me.m_tvOptions.Nodes, nStart.Nodes)
            For Each n As TreeNode In nodes
                If (TypeOf n.Tag Is String) Then
                    If (String.Compare(CStr(n.Tag), strVerb, True) = 0) Then Return n
                End If
                If (n.Nodes.Count > 0) Then
                    Dim n2 As TreeNode = Me.FindNodeByVerb(strVerb, n)
                    If (n2 IsNot Nothing) Then Return n2
                End If
            Next
            Return Nothing
        End Function

        Private Function CreatePage(t As Type) As IOptionsPage

            ' Sanity check
            Debug.Assert(GetType(IOptionsPage).IsAssignableFrom(t))
            Debug.Assert(GetType(Control).IsAssignableFrom(t))

            Dim optionspage As IOptionsPage = Nothing

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            Try
                optionspage = DirectCast(Activator.CreateInstance(t, New Object() {Me.m_uic}), IOptionsPage)
                DirectCast(optionspage, Control).Dock = DockStyle.Fill
                Me.m_lPages.Add(optionspage)
            Catch ex As Exception
                cLog.Write(ex, "dlgOptions::CreatePage " & t.ToString())
            End Try
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

            Return optionspage

        End Function

        Private Function GetPage(strVerb As String) As IOptionsPage

            If (String.IsNullOrWhiteSpace(strVerb)) Then Return Nothing

            Dim t As Type = GetType(ucOptionsGeneral)
            If (Me.m_dtNodes.ContainsKey(strVerb)) Then
                t = Me.m_dtNodes(strVerb)
            End If

            ' Sanity check - if this fails something is really wrong
            Debug.Assert(t IsNot Nothing, "Page type not know, cannot continue")

            For Each optionspage As IOptionsPage In Me.m_lPages
                If optionspage.GetType().Equals(t) Then
                    Return optionspage
                End If
            Next
            Return Me.CreatePage(t)

        End Function

        Private Sub SetDefaults()

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            Try
                If Me.m_pageCurrent IsNot Nothing Then
                    Me.m_pageCurrent.SetDefaults()
                End If
            Catch ex As Exception
                cLog.Write(ex, "dlgOptions.SetDefaults(" & Me.m_pageCurrent.GetType().ToString & ")")
            End Try
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

        End Sub

        Private Sub Apply()

            Dim msgs As cMessagePublisher = Me.m_uic.Core.Messages
            Dim msg As cMessage = Nothing
            Dim result As IOptionsPage.eApplyResultType = IOptionsPage.eApplyResultType.Success

            cApplicationStatusNotifier.StartProgress(Me.m_uic.Core)
            msgs.SetMessageLock()
            Try
                For Each optionspage As IOptionsPage In Me.m_lPages
                    result = DirectCast(Math.Max(result, optionspage.Apply()), IOptionsPage.eApplyResultType)
                Next
            Catch ex As Exception
                ' Whoah
                cLog.Write(ex, "dlgOptions::Apply")
            End Try
            msgs.RemoveMessageLock()
            cApplicationStatusNotifier.EndProgress(Me.m_uic.Core)

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

        Private Sub SelectPage(page As IOptionsPage)

            Me.SuspendLayout()

            ' Optimization
            If Object.ReferenceEquals(page, Me.m_pageCurrent) Then Return
            ' Set new page
            Me.m_pageCurrent = page
            ' Yo
            Me.m_scContent.Panel2.Controls.Clear()
            Dim ctrl As Control = DirectCast(Me.m_pageCurrent, Control)
            ctrl.Dock = DockStyle.Fill
            Me.m_scContent.Panel2.Controls.Add(ctrl)

            Me.ResumeLayout()

        End Sub

        Private Sub ExpandNode(node As TreeNode)
            For Each nodeChild As TreeNode In node.Nodes
                Me.ExpandNode(nodeChild)
            Next
            node.Expand()
        End Sub

#End Region ' Internals

    End Class

End Namespace