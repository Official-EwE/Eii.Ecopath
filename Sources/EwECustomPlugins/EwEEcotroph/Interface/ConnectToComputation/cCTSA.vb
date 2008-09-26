'==============================================================================
'
' $Log: cCTSA.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.8  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace ConnectToComputation

    Public Class cCTSA

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
#End Region 'Public fields

#Region "Private fields"
        Private Shared m_CTSA As New cCTSA
#End Region 'Private fields

#Region "Public properties"
        Public Shared ReadOnly Property CTSA() As cCTSA
            Get
                Return m_CTSA
            End Get
        End Property
#End Region 'Public properties

#Region "Public methods"
        Public Shared Sub RunCTSAParameter()
            DisplayToolStripData()
            m_CTSA.m_EcotrophManager.RunCTSAParameter(m_CTSA.m_ToolStrip)
        End Sub

        Public Shared Sub RunCTSAFwdCal()
            DisplayToolStripData()
            m_CTSA.m_EcotrophManager.RunCTSAFwdCal(m_CTSA.m_ToolStrip)
        End Sub

        Public Shared Sub RunCTSABwdCal()
            DisplayToolStripData()
            m_CTSA.m_EcotrophManager.RunCTSABwdCal(m_CTSA.m_ToolStrip)
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
            cUtility.RemoveToolStrip(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl)
            cUtility.AddToolStrip(m_CTSA.m_PanelToolStrip, m_CTSA.m_ToolStrip)
            cUtility.SetToolStripPropertyDefault(m_CTSA.m_PanelToolStrip)
        End Sub

        Private Shared Sub SetUpToolStrip()
            Dim ToolStp As ToolStrip
            Dim ToolStpLblPrgBar As ToolStripLabel
            Dim ToolStpPrgBar As ToolStripProgressBar

            ToolStp = CType(m_CTSA.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
            ToolStpLblPrgBar = CType(m_CTSA.m_ToolStrip.Items("tslblProgressBar"), ToolStripLabel)
            ToolStpPrgBar = CType(m_CTSA.m_ToolStrip.Items("tspgbProgressBar"), ToolStripProgressBar)

            ToolStp.Visible = False

            ToolStpLblPrgBar.Text = My.Resources.LBL_CTSA_PRGR
            ToolStpLblPrgBar.Visible = True
            ToolStpPrgBar.Visible = True

            ToolStp.Refresh()
            ToolStp.Visible = True
            ToolStp.Update()
            'm_CTSA.m_PanelToolStrip.Controls("tsEcotroph").Update()
        End Sub
#End Region 'Helper methods

    End Class

End Namespace