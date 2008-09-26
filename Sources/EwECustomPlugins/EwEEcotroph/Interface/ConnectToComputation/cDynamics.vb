'==============================================================================
'
' $Log: cDynamics.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.6  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace ConnectToComputation

    Public Class cDynamics

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
#End Region 'Public fields

#Region "Private fields"
        Private Shared m_Dynamics As New cDynamics
#End Region 'Private fields

#Region "Public properties"
        Public Shared ReadOnly Property Dynamics() As cDynamics
            Get
                Return m_Dynamics
            End Get
        End Property
#End Region 'Public properties

#Region "Public methods"
        Public Shared Sub RunDynamicsParameter(ByVal MainFrom As String)
            DisplayToolStripData()
            m_Dynamics.m_EcotrophManager.RunDynamicsParameter(m_Dynamics.m_ToolStrip, MainFrom)
        End Sub

        Public Shared Sub RunDynamics(ByVal CatchHistoryType As String, Optional ByVal CatchPastAnalysisFilePath As String = "")
            DisplayToolStripData()
            If CatchPastAnalysisFilePath = "" Then
                m_Dynamics.m_EcotrophManager.RunDynamics(m_Dynamics.m_ToolStrip, CatchHistoryType)
            Else
                m_Dynamics.m_EcotrophManager.RunDynamics(m_Dynamics.m_ToolStrip, CatchHistoryType, CatchPastAnalysisFilePath)
            End If
        End Sub

        Public Shared Sub RunDynamicsCatches(ByVal MainFrom As String)
            DisplayToolStripData()
            m_Dynamics.m_EcotrophManager.RunDynamicsCatches(m_Dynamics.m_ToolStrip, MainFrom)
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Sub DisplayToolStripData()
            Cursor.Current = Cursors.WaitCursor
            SetUpToolStripPropertyDefault()
            SetUpToolStrip()
            Cursor.Current = Cursors.Default
        End Sub

        Private Shared Sub SetUpToolStripPropertyDefault()
            cUtility.RemoveToolStrip(m_Dynamics.m_PanelToolStrip, m_Dynamics.m_PanelTabCntl)
            cUtility.AddToolStrip(m_Dynamics.m_PanelToolStrip, m_Dynamics.m_ToolStrip)
            cUtility.SetToolStripPropertyDefault(m_Dynamics.m_PanelToolStrip)
        End Sub

        Private Shared Sub SetUpToolStrip()
            Dim ToolStp As ToolStrip
            Dim ToolStpLblPrgBar As ToolStripLabel
            Dim ToolStpPrgBar As ToolStripProgressBar

            ToolStp = CType(m_Dynamics.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
            ToolStpLblPrgBar = CType(m_Dynamics.m_ToolStrip.Items("tslblProgressBar"), ToolStripLabel)
            ToolStpPrgBar = CType(m_Dynamics.m_ToolStrip.Items("tspgbProgressBar"), ToolStripProgressBar)

            ToolStp.Visible = False

            ToolStpLblPrgBar.Text = My.Resources.LBL_DYNAMICS_PRGR
            ToolStpLblPrgBar.Visible = True
            ToolStpPrgBar.Visible = True

            ToolStp.Refresh()
            ToolStp.Visible = True
            ToolStp.Update()
        End Sub
#End Region 'Helper methods

    End Class

End Namespace
