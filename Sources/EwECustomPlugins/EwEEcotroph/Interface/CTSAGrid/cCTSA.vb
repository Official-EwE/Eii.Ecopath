'==============================================================================
'
' $Log: cCTSA.vb,v $
' Revision 1.1  2008/09/26 07:30:38  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.37  2008/06/05 19:43:46  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace UserInterface

    Public Class cCTSA

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        'Public m_TreeView As TreeView
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
        'Public m_TabCntl As TabControl
        Public m_Tree As TreeView
        Public m_TabPages(cUtility.NUM_TAB_PAGE_DESIGNER) As TabPage
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
        Public Shared Sub DisplayCTSAParameter()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 1
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display CTSA parameter
            DisplayBasicParamGridData(TabCntl, TabPgNum)

            cUtility.DisplayToolStripData(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl, m_CTSA.m_ToolStrip, _
              m_CTSA.m_Tree.SelectedNode.Text, m_CTSA.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayCTSAFwdCal()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 2
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display main data
            DisplayFwdCalMainGridData(TabCntl, TabPgNum)
            'Retrieve data grid and display forward calculation data
            DisplayFwdCalParamGridData(TabCntl, TabPgNum)

            cUtility.DisplayToolStripData(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl, m_CTSA.m_ToolStrip, _
              m_CTSA.m_Tree.SelectedNode.Text, m_CTSA.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayCTSABwdCal()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 2
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display main data
            DisplayBwdCalMainGridData(TabCntl, TabPgNum)
            'Retrieve data grid and display forward calculation data
            DisplayBwdCalParamGridData(TabCntl, TabPgNum)

            cUtility.DisplayToolStripData(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl, m_CTSA.m_ToolStrip, _
              m_CTSA.m_Tree.SelectedNode.Text, m_CTSA.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub UpdateCTSAParameter(ByVal TbxWaterTemp As ToolStripTextBox, ByVal TbxTETL12 As ToolStripTextBox, _
          ByVal TbxTETL2 As ToolStripTextBox, ByVal TbxAsymptote As ToolStripTextBox, ByVal TbxTL50 As ToolStripTextBox, _
          ByVal TbxSlope As ToolStripTextBox, ByVal DataGrid As DataGridView, ByRef IsValidCTSAParameter As cUtility.Valid)
            Try
                For Row As Integer = 1 To m_CTSA.m_EcotrophManager.InputData.Catches.GetUpperBound(0)
                    m_CTSA.m_EcotrophManager.InputData.Catches(Row) = CSng(DataGrid.Item(2, Row - 1).Value)
                Next

                m_CTSA.m_EcotrophManager.InputData.WaterTemp = CSng(TbxWaterTemp.Text)
                m_CTSA.m_EcotrophManager.InputData.TETL12 = CSng(TbxTETL12.Text)
                m_CTSA.m_EcotrophManager.InputData.TETL2 = CSng(TbxTETL2.Text)
                For Row As Integer = 1 To m_CTSA.m_EcotrophManager.InputData.Catches.GetUpperBound(0)
                    m_CTSA.m_EcotrophManager.InputData.CTSATopD(Row) = CSng(DataGrid.Item(6, Row - 1).Value)
                    m_CTSA.m_EcotrophManager.InputData.CTSAFormD(Row) = CSng(DataGrid.Item(7, Row - 1).Value)
                Next

                If CDbl(TbxAsymptote.Text) < 0.0 Or CDbl(TbxAsymptote.Text) > 1.0 Or _
                  CDbl(TbxTL50.Text) < 2.0 Or CDbl(TbxTL50.Text) > 4.0 Or _
                  CDbl(TbxSlope.Text) < 0.0 Or CDbl(TbxSlope.Text) > 30.0 Then
                    IsValidCTSAParameter = cUtility.Valid.F
                Else
                    m_CTSA.m_EcotrophManager.InputData.Asymptote = CSng(TbxAsymptote.Text)
                    m_CTSA.m_EcotrophManager.InputData.TL50 = CSng(TbxTL50.Text)
                    m_CTSA.m_EcotrophManager.InputData.Slope = CSng(TbxSlope.Text)
                    WriteFile("CTSAParameter")
                    IsValidCTSAParameter = cUtility.Valid.T
                End If
            Catch ex As Exception
                IsValidCTSAParameter = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateFwdCalParameter(ByVal CbxInitialization As ToolStripComboBox, ByVal DataGrid As DataGridView, _
          ByRef IsValidFwdCalParameter As cUtility.Valid)
            Try
                m_CTSA.m_EcotrophManager.InputData.SeedNameFwdCal = CbxInitialization.Text
                Select Case CbxInitialization.Text
                    Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL1
                        m_CTSA.m_EcotrophManager.InputData.SeedValueFwdCal = CSng(DataGrid.Item(3, 0).Value)
                    Case My.Resources.DROP_DWN_LST_ITM_BIOM_TL2
                        m_CTSA.m_EcotrophManager.InputData.SeedValueFwdCal = CSng(DataGrid.Item(3, 1).Value)
                    Case My.Resources.DROP_DWN_LST_ITM_PROD_TL1
                        m_CTSA.m_EcotrophManager.InputData.SeedValueFwdCal = CSng(DataGrid.Item(2, 0).Value)
                    Case My.Resources.DROP_DWN_LST_ITM_PROD_TL2
                        m_CTSA.m_EcotrophManager.InputData.SeedValueFwdCal = CSng(DataGrid.Item(2, 1).Value)
                End Select
                WriteFile("CTSAFwdCalParameter")
                IsValidFwdCalParameter = cUtility.Valid.T
            Catch ex As Exception
                IsValidFwdCalParameter = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateBwdCalParameter(ByVal CbxTerminalTL As ToolStripComboBox, ByVal CbxInitialization As ToolStripComboBox, _
          ByVal DataGrid As DataGridView, ByRef IsValidBwdCalParameter As cUtility.Valid)
            Dim RowTTL As Integer

            Try
                m_CTSA.m_EcotrophManager.InputData.SeedNameBwdCal = CbxInitialization.Text
                'm_CTSA.m_EcotrophManager.InputData.SlopeSelectivityTTL = CSng(TbxSlopeSelectivityTTL.Text) 
                m_CTSA.m_EcotrophManager.InputData.TTL = CSng(CbxTerminalTL.Text)
                RowTTL = CInt((Int(CSng(CbxTerminalTL.Text)) - 2) * 10 + CInt((CSng(CbxTerminalTL.Text) - Int(CSng(CbxTerminalTL.Text))) * 10) + 2) _
                  - 1
                Select Case CbxInitialization.Text
                    Case My.Resources.DROP_DWN_LST_ITM_FISH_LOSS_RATE_TLL
                        m_CTSA.m_EcotrophManager.InputData.SeedValueBwdCal = CSng(DataGrid.Item(4, RowTTL).Value)
                    Case My.Resources.DROP_DWN_LST_ITM_ACCESS_FISH_MORTALITY_TTL
                        m_CTSA.m_EcotrophManager.InputData.SeedValueBwdCal = CSng(DataGrid.Item(5, RowTTL).Value)
                End Select
                WriteFile("CTSABwdCalParameter")
                IsValidBwdCalParameter = cUtility.Valid.T
            Catch ex As Exception
                IsValidBwdCalParameter = cUtility.Valid.F
            End Try
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Function TabControlFn(ByVal NumTabPg As Integer, ByRef TabPgNum As Integer) As TabControl
            Dim TabCntl As TabControl
            'Dim NumTabPage As Integer

            'cUtility.RemoveTabControl(m_CTSA.m_PanelTabCntl, m_CTSA.m_TabCntl)
            'cUtility.AddTabControl(m_CTSA.m_PanelTabCntl, m_CTSA.m_TabCntl)
            cUtility.SetTabControlPropertyDefault(m_CTSA.m_PanelTabCntl, m_CTSA.m_TabPages)

            'Select Case m_CTSA.m_TreeView.SelectedNode.Text
            '    Case My.Resources.TREE_NODE_BASIC_PARAM
            '        NumTabPage = 1
            '    Case My.Resources.TREE_NODE_FWD_CAL, My.Resources.TREE_NODE_BWD_CAL
            '        NumTabPage = 2
            'End Select
            TabCntl = CType(m_CTSA.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
            If TabCntl.Controls.Count > NumTabPg Then
                For TabPgNum = TabCntl.Controls.Count To (NumTabPg + 1) Step -1
                    TabCntl.Controls.RemoveByKey("tpEcotroph" & TabPgNum)
                Next
            End If
            TabCntl.Visible = True
            TabPgNum = 0
            Return TabCntl
        End Function

        Private Shared Sub DisplayBasicParamGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            'cCTSAParameter.DisplayToolStripData(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl, _
            '  m_CTSA.m_ToolStrip, m_CTSA)

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_BASIC_PARAM
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cCTSABasicParam.DisplayGridData(DataGrid, m_CTSA)
        End Sub

        Private Shared Sub DisplayFwdCalMainGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            'cCTSAMain.DisplayToolStripData(m_CTSA.m_PanelToolStrip, m_CTSA.m_PanelTabCntl, m_CTSA.m_ToolStrip)

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_MAIN
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cCTSAFwdCalMain.DisplayGridData(DataGrid, m_CTSA)
        End Sub

        Private Shared Sub DisplayFwdCalParamGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_UNEXPLOITED
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cCTSAFwdCalParam.DisplayGridData(DataGrid, m_CTSA)
        End Sub

        Private Shared Sub DisplayBwdCalMainGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_MAIN
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cCTSABwdCalMain.DisplayGridData(DataGrid, m_CTSA)
        End Sub

        Private Shared Sub DisplayBwdCalParamGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_UNEXPLOITED
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cCTSABwdCalParam.DisplayGridData(DataGrid, m_CTSA)
        End Sub

        Private Shared Sub WriteFile(ByVal FileName As String)
            m_CTSA.m_EcotrophManager.InputData.WriteFile(FileName, m_CTSA.m_EcotrophManager)
        End Sub
#End Region 'Helper methods

    End Class

End Namespace
