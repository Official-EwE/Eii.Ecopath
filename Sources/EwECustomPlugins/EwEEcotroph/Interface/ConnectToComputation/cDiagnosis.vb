'==============================================================================
'
' $Log: cDiagnosis.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.9  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace ConnectToComputation

    Public Class cDiagnosis

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
#End Region 'Public fields

#Region "Private fields"
        Private Shared m_Diagnosis As New cDiagnosis
#End Region 'Private fields

#Region "Public properties"
        Public Shared ReadOnly Property Diagnosis() As cDiagnosis
            Get
                Return m_Diagnosis
            End Get
        End Property
#End Region 'Public properties

#Region "Public methods"
        Public Shared Sub RunDiagnosisParameter(ByVal MainFrom As String)
            DisplayToolStripData()
            m_Diagnosis.m_EcotrophManager.RunDiagnosisParameter(m_Diagnosis.m_ToolStrip, MainFrom)
        End Sub

        Public Shared Sub RunDiagnosis(ByVal EffortMultiplierType As String)
            DisplayToolStripData()
            m_Diagnosis.m_EcotrophManager.RunDiagnosis(m_Diagnosis.m_ToolStrip, EffortMultiplierType)
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
            cUtility.RemoveToolStrip(m_Diagnosis.m_PanelToolStrip, m_Diagnosis.m_PanelTabCntl)
            cUtility.AddToolStrip(m_Diagnosis.m_PanelToolStrip, m_Diagnosis.m_ToolStrip)
            cUtility.SetToolStripPropertyDefault(m_Diagnosis.m_PanelToolStrip)
        End Sub

        Private Shared Sub SetUpToolStrip()
            Dim ToolStp As ToolStrip
            Dim ToolStpLblPrgBar As ToolStripLabel
            Dim ToolStpPrgBar As ToolStripProgressBar

            ToolStp = CType(m_Diagnosis.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
            ToolStpLblPrgBar = CType(m_Diagnosis.m_ToolStrip.Items("tslblProgressBar"), ToolStripLabel)
            ToolStpPrgBar = CType(m_Diagnosis.m_ToolStrip.Items("tspgbProgressBar"), ToolStripProgressBar)

            ToolStp.Visible = False

            ToolStpLblPrgBar.Text = My.Resources.LBL_DIAGNOSIS_PRGR
            ToolStpLblPrgBar.Visible = True
            ToolStpPrgBar.Visible = True

            ToolStp.Refresh()
            ToolStp.Visible = True
            ToolStp.Update()
        End Sub
#End Region 'Helper methods
    End Class

End Namespace
