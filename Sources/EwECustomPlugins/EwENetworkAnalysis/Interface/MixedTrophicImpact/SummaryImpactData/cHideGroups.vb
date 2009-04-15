'==============================================================================
'
' $Log: cHideGroups.vb,v $
' Revision 1.6  2009/04/15 23:37:38  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.5  2009/04/15 18:14:54  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.4  2008/12/10 20:56:19  joeh
' Finalize the Suitability Plot
'
' Revision 1.3  2008/12/04 01:14:48  joeh
' Add ucPlotOfMixedTrophicImpact
'
' Revision 1.2  2008/12/03 20:49:19  joeh
' Incorportate Functional Response into Network Analysis - Take three
'
' Revision 1.1  2008/09/26 07:30:53  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.7  2008/06/24 00:52:27  joeh
' Ecosim NA indice plots are no longer displayed in a pop up form, rather they are displayed in the same form where  we have the NA tree view
'
' Revision 1.6  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.5  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.4  2007/06/21 23:49:35  joeh
' Move hard coded strings into the resource file
'
' Revision 1.3  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'
'==============================================================================
Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Windows.Forms

Public Class cHideGroups
    Private Shared m_HideGroupsClassInstance As cHideGroups

    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Panel


    Public Shared Function GetInstance(ByVal Panel As Windows.Forms.Panel) As cHideGroups
        m_Panel = Panel

        If m_HideGroupsClassInstance Is Nothing Then m_HideGroupsClassInstance = New cHideGroups(Panel)
        Return m_HideGroupsClassInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_Panel = Panel
    End Sub
    Public Sub SetUpPanel()
        RemoveToolStrip()

        SetUpGrid()
    End Sub

    Private Sub SetUpGrid()
        Dim DataGrid As DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), TableLayoutPanel)
        'Dim FunctRespUC As ucFunctionalResponse = _
        '    CType(m_Panel.Controls("ucFUnctionalResponse"), ucFunctionalResponse)
        Dim MixedTrophicImpactUC As ucPlotOfMixedTrophicImpact = _
            CType(m_Panel.Controls("ucPlotOfMixedTrophicImpact"), ucPlotOfMixedTrophicImpact)

        m_Panel.AutoScroll = False
        LogoPanel.Visible = False
        DataGrid.Visible = False
        GraphPane.Visible = False
        'If Not FunctRespUC Is Nothing Then FunctRespUC.Visible = False
        If Not MixedTrophicImpactUC Is Nothing Then MixedTrophicImpactUC.Visible = False
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), ToolStrip)
        Dim DataGrid As DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), DataGridView)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = DockStyle.Fill
        End If
    End Sub

End Class
