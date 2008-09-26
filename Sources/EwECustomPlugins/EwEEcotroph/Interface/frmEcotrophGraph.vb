'==============================================================================
'
' $Log: frmEcotrophGraph.vb,v $
' Revision 1.1  2008/09/26 07:30:43  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.21  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================
Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports ZedGraph

Public Class frmEcotrophGraph

#Region "Public fields"
    Public m_NodeText As String
    Public m_TabPageText As String
    Public m_DataGrid As DataGridView
    Public m_ZedGraph As ZedGraphControl
    Public m_EcotrophManager As cEcotrophManager
#End Region 'Public fields

#Region "Public constructors"
    Public Sub New()
        InitializeComponent()
        m_ZedGraph = zgZedGraph
    End Sub
#End Region 'Public constructors

#Region "Public methods"
    Public Sub InitializeGraphClass()
        Dim TransposeGraph As cTransposeGraph
        Dim CTSAGraph As cCTSAGraph
        Dim DiagnosisGraph As cDiagnosisGraph
        Dim DynamicsGraph As cDynamicsGraph

        Text = m_NodeText & " - " & m_TabPageText ' form's title
        Select Case m_NodeText
            Case My.Resources.TREE_NODE_AUTO_SMOOTH, My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                TransposeGraph = New cTransposeGraph
                TransposeGraph.m_Form = Me
                TransposeGraph.m_NodeText = m_NodeText
                TransposeGraph.m_TabPageText = m_TabPageText
                TransposeGraph.m_DataGrid = m_DataGrid
                TransposeGraph.m_ZedGraph = m_ZedGraph
                TransposeGraph.PlotGraph()
            Case My.Resources.TREE_NODE_FWD_CAL, My.Resources.TREE_NODE_BWD_CAL
                CTSAGraph = New cCTSAGraph
                CTSAGraph.m_Form = Me
                CTSAGraph.m_NodeText = m_NodeText
                CTSAGraph.m_TabPageText = m_TabPageText
                CTSAGraph.m_DataGrid = m_DataGrid
                CTSAGraph.m_ZedGraph = m_ZedGraph
                CTSAGraph.m_EcotrophManager = m_EcotrophManager
                CTSAGraph.PlotGraph()
            Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR, My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                DiagnosisGraph = New cDiagnosisGraph
                DiagnosisGraph.m_Form = Me
                DiagnosisGraph.m_NodeText = m_NodeText
                DiagnosisGraph.m_TabPageText = m_TabPageText
                DiagnosisGraph.m_DataGrid = m_DataGrid
                DiagnosisGraph.m_ZedGraph = m_ZedGraph
                DiagnosisGraph.PlotGraph()
            Case My.Resources.TREE_NODE_CATCH_FORECAST, My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                DynamicsGraph = New cDynamicsGraph
                DynamicsGraph.m_Form = Me
                DynamicsGraph.m_NodeText = m_NodeText
                DynamicsGraph.m_TabPageText = m_TabPageText
                DynamicsGraph.m_DataGrid = m_DataGrid
                DynamicsGraph.m_ZedGraph = m_ZedGraph
                DynamicsGraph.PlotGraph()
        End Select

        Show() 'ShowDialog()
    End Sub
#End Region 'Public methods

End Class