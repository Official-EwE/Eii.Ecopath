Imports ScientificInterface.Ecosim

Public Class frmMSEAssessFleets
    Private m_blocks As ucPolicyColorBlocks = Nothing
    Private m_core As EwECore.cCore

    Public Sub New()
        m_core = EwECore.cCore.GetInstance

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.m_blocks = New ucPolicyColorBlocks()
    End Sub

    Private Sub frmMSEAssessFleets_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SplitContainer1.Panel1.Controls.Clear()
        SplitContainer1.Panel1.Controls.Add(m_blocks)
        m_blocks.Dock = DockStyle.Fill
        m_blocks.ParmBlockCodes.nBlockCodes = m_core.nFleets
        m_blocks.ParmBlockCodes.SelectedBlockNum = 1
    End Sub

End Class