'==============================================================================
'
' $Log: cTranspose.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.21  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace ConnectToComputation

    Public Class cTranspose

        'Private m_EcotrophManager As cEcotrophManager
        'Private m_Panel As Panel

        'Public Sub New(ByVal EcotrophManager As cEcotrophManager, ByVal Panel As Panel)
        '    m_EcotrophManager = EcotrophManager
        '    m_Panel = Panel
        'End Sub

        'Public Sub RunTransposeUsingAutomaticEmpiricalFunction()
        '    SetUpToolStrip()
        '    'SetUpTabControl()
        '    'SetUpGrid()
        '    'Computation.cTranspose.RunTransposeUsingAutomaticEmpiricalFunction()
        'End Sub

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
#End Region 'Public fields

#Region "Private fields"
        Private Shared m_Transpose As New cTranspose
#End Region 'Private fields

#Region "Public properties"
        Public Shared ReadOnly Property Transpose() As cTranspose
            Get
                Return m_Transpose
            End Get
        End Property
#End Region 'Public properties

#Region "Public methods"
        Public Shared Sub RunTransposeAEF()
            DisplayToolStripData()
            m_Transpose.m_EcotrophManager.RunTransposeAEF(m_Transpose.m_ToolStrip)
        End Sub

        Public Shared Sub RunTransposeOmniIdx()
            DisplayToolStripData()
            'SetUpToolStrip()
            m_Transpose.m_EcotrophManager.RunTransposeOmniIdx(m_Transpose.m_ToolStrip)
        End Sub

        Public Shared Sub RunTransposeUserDefVal()
            DisplayToolStripData()
            'SetUpToolStrip()
            m_Transpose.m_EcotrophManager.RunTransposeUserDefVal(m_Transpose.m_ToolStrip)
        End Sub

        Public Shared Sub RunTransposeAEFCatches()
            DisplayToolStripData()
            m_Transpose.m_EcotrophManager.RunTransposeAEFCatches(m_Transpose.m_ToolStrip)
        End Sub

        Public Shared Sub RunTransposeUserDefValCatches()
            DisplayToolStripData()
            m_Transpose.m_EcotrophManager.RunTransposeUserDefValCatches(m_Transpose.m_ToolStrip)
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
            cUtility.RemoveToolStrip(m_Transpose.m_PanelToolStrip, m_Transpose.m_PanelTabCntl)
            cUtility.AddToolStrip(m_Transpose.m_PanelToolStrip, m_Transpose.m_ToolStrip)
            cUtility.SetToolStripPropertyDefault(m_Transpose.m_PanelToolStrip)
        End Sub

        Private Shared Sub SetUpToolStrip()
            Dim ToolStp As ToolStrip
            Dim ToolStpLblPrgBar As ToolStripLabel
            Dim ToolStpPrgBar As ToolStripProgressBar

            ToolStp = CType(m_Transpose.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
            ToolStpLblPrgBar = CType(ToolStp.Items("tslblProgressBar"), ToolStripLabel)
            ToolStpPrgBar = CType(ToolStp.Items("tspgbProgressBar"), ToolStripProgressBar)

            ToolStp.Visible = False

            ToolStpLblPrgBar.Text = My.Resources.LBL_TRANSP_PRGR
            ToolStpLblPrgBar.Visible = True
            ToolStpPrgBar.Visible = True

            ToolStp.Refresh()
            ToolStp.Visible = True
            ToolStp.Update()
            'm_Transpose.m_PanelToolStrip.Controls("tsEcotroph").Update()
        End Sub
        'Private Shared Sub SetUpToolStripOld()
        '    Dim ToolStp As ToolStrip
        '    Dim ToolStpLbl1 As ToolStripLabel
        '    Dim ToolStpLbl2 As ToolStripLabel
        '    Dim ToolStpLbl3 As ToolStripLabel
        '    Dim ToolStpTxtBox As ToolStripTextBox
        '    Dim ToolStpTxtBox2 As ToolStripTextBox
        '    Dim ToolStpPrgBar As ToolStripProgressBar
        '    Dim ToolStpBtn As ToolStripButton

        '    RemoveToolStrip()
        '    AddToolStrip()

        '    ToolStp = CType(m_Transpose.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        '    ToolStpLbl1 = CType(ToolStp.Items("tslblSmoothFactor"), ToolStripLabel)
        '    ToolStpLbl2 = CType(ToolStp.Items("tslblProgressBar"), ToolStripLabel)
        '    ToolStpLbl3 = CType(ToolStp.Items("tslblWaterTemp"), ToolStripLabel)
        '    ToolStpTxtBox = CType(ToolStp.Items("tstbxSmoothFactor"), ToolStripTextBox)
        '    ToolStpTxtBox2 = CType(ToolStp.Items("tstbxWaterTemp"), ToolStripTextBox)
        '    ToolStpPrgBar = CType(ToolStp.Items("tspgbProgressBar"), ToolStripProgressBar)
        '    ToolStpBtn = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)

        '    ToolStp.Visible = True
        '    ToolStpLbl1.Visible = False
        '    ToolStpTxtBox.Visible = False
        '    ToolStpLbl2.Visible = True
        '    ToolStpLbl2.Text = My.Resources.LBL_TRANSP_PRGR
        '    ToolStpPrgBar.Visible = True
        '    ToolStpBtn.Visible = False
        '    ToolStpLbl3.Visible = False
        '    ToolStpTxtBox2.Visible = False

        '    ToolStp.Refresh()
        'End Sub

        'Private Shared Sub RemoveToolStrip()
        '    Dim TabCntl As TabControl

        '    TabCntl = CType(m_Transpose.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
        '    If Not m_Transpose.m_ToolStrip Is Nothing Then
        '        m_Transpose.m_PanelToolStrip.Controls.RemoveByKey("tsEcotroph")
        '        TabCntl.Dock = DockStyle.Fill
        '    End If
        'End Sub

        'Private Shared Sub AddToolStrip()
        '    m_Transpose.m_PanelToolStrip.Controls.Add(m_Transpose.m_ToolStrip)
        'End Sub
#End Region 'Helper methods

    End Class

End Namespace
