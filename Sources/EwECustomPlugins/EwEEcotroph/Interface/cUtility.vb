'==============================================================================
'
' $Log: cUtility.vb,v $
' Revision 1.1  2008/09/26 07:30:43  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.60  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================
Imports System.Windows.Forms
Imports System.Drawing

Public Class cUtility

    Public Enum Valid
        F = 0
        T = 1
        NA = 2
    End Enum

    Public Shared ReadOnly NUM_TAB_PAGE_DESIGNER As Integer = 13

    Public Shared ReadOnly DEFAULT_COL_WIDTH As Integer = 70
    Public Shared ReadOnly ID_COL_WIDTH As Integer = 25
    Public Shared ReadOnly TRP_LVL_COL_WIDTH As Integer = 70 '110
    Public Shared ReadOnly GRP_NAME_TRP_LVL_COL_WIDTH As Integer = 110

    Public Shared ReadOnly DEFAULT_ROW_HEIGHT As Integer = 22
    Public Shared ReadOnly SIGMA_TRP_LVLOUT_ROW_HEIGHT As Integer = 22
    Public Shared ReadOnly ACCESS_TRP_LVLOUT_ROW_HEIGHT As Integer = 22
    Public Shared ReadOnly EFF_MTPLR_TRP_LVLOUT_ROW_HEIGHT As Integer = 22

    Public Shared ReadOnly DEFAULT_CTSA_WATER_TEMP As Single = 18.0
    Public Shared ReadOnly DEFAULT_CTSA_TE_TL12 As Single = 10.0
    Public Shared ReadOnly DEFAULT_CTSA_TE_TL2 As Single = 10.0
    Public Shared ReadOnly DEFAULT_CTSA_TOPD As Single = 0.4
    Public Shared ReadOnly DEFAULT_CTSA_FORMD As Single = 0.5
    Public Shared ReadOnly DEFAULT_CTSA_ASYMPTOTE As Single = 1.0
    Public Shared ReadOnly DEFAULT_CTSA_TL50 As Single = 2.8
    Public Shared ReadOnly DEFAULT_CTSA_SLOPE As Single = 8
    Public Shared ReadOnly DEFAULT_CTSA_CATCHES As Single = 0.01

    Public Shared ReadOnly DEFAULT_DIAGNOSIS_BETA As Single = 0.0
    Public Shared ReadOnly DEFAULT_DIAGNOSIS_FORMD As Single = 0.5
    Public Shared ReadOnly DEFAULT_DIAGNOSIS_TOPD As Single = 0.4

    Public Shared ReadOnly DEFAULT_DYNAMICS_BETA As Single = 0.0
    Public Shared ReadOnly DEFAULT_DYNAMICS_FORMD As Single = 0.5
    Public Shared ReadOnly DEFAULT_DYNAMICS_TOPD As Single = 0.4

    'Public Shared ReadOnly AEF_ALGOR As String = "Automatic empirical function"
    'Public Shared ReadOnly OMNI_IDX_ALGOR As String = "Omnivory index"
    'Public Shared ReadOnly USER_DEF_VAL_ALGOR As String = "User defined values"

    Public Shared Sub RemoveToolStrip(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel)
        Dim ToolStp As ToolStrip
        Dim TabCntl As TabControl

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        TabCntl = CType(PanelTabCntl.Controls("tcEcotroph"), TabControl)
        If Not ToolStp Is Nothing Then
            PanelToolStrip.Controls.RemoveByKey("tsEcotroph")
            TabCntl.Dock = DockStyle.Fill
        End If
    End Sub

    Public Shared Sub AddToolStrip(ByVal PanelToolStrip As Panel, ByVal ToolStp As ToolStrip)
        PanelToolStrip.Controls.Add(ToolStp)
    End Sub

    Public Shared Sub SetToolStripPropertyDefault(ByVal PanelToolStrip As Panel)
        Dim ToolStp As ToolStrip

        ToolStp = CType(PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
        ToolStp.Visible = False
        For Idx As Integer = 1 To ToolStp.Items.Count
            ToolStp.Items(Idx - 1).Visible = False
        Next
        ToolStp.Refresh()
        ToolStp.Visible = True
        ToolStp.Update()
    End Sub

    Public Shared Sub SetTabControlPropertyDefault(ByVal PanelTabControl As Panel, ByVal TabPgs() As TabPage)
        Dim TabCntl As TabControl

        TabCntl = CType(PanelTabControl.Controls("tcEcotroph"), TabControl)
        For TabPgNum As Integer = TabCntl.Controls.Count + 1 To cUtility.NUM_TAB_PAGE_DESIGNER
            TabCntl.Controls.Add(TabPgs(TabPgNum))
            'TabCntl.TabPages.Add("tpEcotroph" & TabPgNum, "")
            'DataView = New DataGridView
            'DataView.Name = "dgvEcotroph" & TabPgNum
            'DataView.Anchor = AnchorStyles.Bottom
            'DataView.Anchor = AnchorStyles.Top
            'DataView.Anchor = AnchorStyles.Left
            'DataView.Anchor = AnchorStyles.Right
            'DataView.Dock = DockStyle.Fill
            'DataView.BackgroundColor = Drawing.Color.White
            'DataView.Location = New Point(0, 0)
            'TabCntl.TabPages("tpEcotroph" & TabPgNum).Controls.Add(DataView)
        Next
    End Sub

    Public Shared Sub SetGridColumnPropertyDefault(ByVal DataGrid As DataGridView)
        DataGrid.ReadOnly = True
        DataGrid.ColumnHeadersVisible = True
        For ColIndex As Integer = 0 To DataGrid.ColumnCount - 1
            DataGrid.Columns(ColIndex).HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(ColIndex).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            DataGrid.Columns(ColIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Columns(ColIndex).Width = DEFAULT_COL_WIDTH
            DataGrid.Columns(ColIndex).Frozen = False
            DataGrid.Columns(ColIndex).SortMode = DataGridViewColumnSortMode.NotSortable
        Next
        DataGrid.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect
        'DataGrid.EditMode = DataGridViewEditMode.EditOnEnter 'DataGrid.ReadOnly will be set to False in other classes
    End Sub

    Public Shared Sub SetGridRowPropertyDefault(ByVal DataGrid As DataGridView)
        DataGrid.RowHeadersVisible = True
        For RowIndex As Integer = 0 To DataGrid.RowCount - 1
            'DataGrid.Rows(RowIndex).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            'DataGrid.Rows(RowIndex).DefaultCellStyle.BackColor = Drawing.Color.White
            DataGrid.Rows(RowIndex).Height = DEFAULT_ROW_HEIGHT
            DataGrid.Rows(RowIndex).Frozen = False
        Next
    End Sub

    Public Shared Sub SetGridCellPropertyDefault(ByVal DataGrid As DataGridView) ', ByVal EcotrophManager As cEcotrophManager)
        Dim CellStyle As DataGridViewCellStyle

        CellStyle = New DataGridViewCellStyle
        CellStyle.Alignment = DataGridViewContentAlignment.MiddleRight '???
        DataGrid.Item(1, 0).Style = CellStyle
        DataGrid.Item(1, 1).Style = CellStyle

        CellStyle = New DataGridViewCellStyle
        CellStyle.BackColor = Drawing.Color.White
        For Col As Integer = 1 To DataGrid.ColumnCount - 2 'EcotrophManager.AccessBiomass.GetUpperBound(1)
            For Row As Integer = 0 To DataGrid.RowCount - 1
                'DataGrid.Item(Col + 1, 0).Style = CellStyle
                'DataGrid.Item(Col + 1, 1).Style = CellStyle
                DataGrid.Item(Col + 1, Row).Style = CellStyle
            Next
        Next
    End Sub

    Public Shared Sub DisplayToolStripData(ByVal PanelToolStrip As Panel, ByVal PanelTabCntl As Panel, ByVal ToolStp As ToolStrip, _
      ByVal SelectedNodeName As String, ByVal SelectedNodeParentName As String, ByVal EnteredTabPageName As String)
        Select Case SelectedNodeName
            Case My.Resources.TREE_NODE_AUTO_SMOOTH, My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_PROPORT_STD) Then cProportionSTD.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_PROD) Then cTransposeFlow.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_TRANSP_BIOMASS) Then cTransposeBiomass.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_CATCH) Then cTransposeCatch.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_CATCHES) Then cTransposeCatches.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_ACCESS_BIOMASS) Then cAccessBiomass.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose, SelectedNodeName)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_MAIN) Then cTransposeMain.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, _
                  UserInterface.cTranspose.Transpose)
            Case My.Resources.TREE_NODE_BASIC_PARAM
                Select Case SelectedNodeParentName
                    Case My.Resources.TREE_NODE_CTSA
                        cCTSABasicParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cCTSA.CTSA)
                    Case My.Resources.TREE_NODE_DIAGNOSIS
                        cDiagnosisBasicParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cDiagnosis.Diagnosis)
                    Case My.Resources.TREE_NODE_DYNAMICS
                        If EnteredTabPageName.ToString.Contains(My.Resources.TAB_BASIC_PARAM) Then cDynamicsBasicParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cDynamics.Dynamics)
                        If EnteredTabPageName.ToString.Contains(My.Resources.TAB_INTRP_PARAM) Then cDynamicsIntrpParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cDynamics.Dynamics)
                End Select
            Case My.Resources.TREE_NODE_FWD_CAL
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_MAIN) Then cCTSAFwdCalMain.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_UNEXPLOITED) Then cCTSAFwdCalParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cCTSA.CTSA)
            Case My.Resources.TREE_NODE_BWD_CAL
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_MAIN) Then cCTSAFwdCalMain.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp)
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_UNEXPLOITED) Then cCTSABwdCalParam.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cCTSA.CTSA)
            Case My.Resources.TREE_NODE_EVEN_EFF_MTPLR, My.Resources.TREE_NODE_UNEVEN_EFF_MTPLR, My.Resources.TREE_NODE_USER_DEF_EFF_MTPLR
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_SUMMARY) Then
                    cDiagnosisSummary.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp)
                Else
                    cDiagnosisGeneral.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, SelectedNodeName)
                End If
            Case My.Resources.TREE_NODE_CATCH_FORECAST, My.Resources.TREE_NODE_CATCH_PAST_ANALYSIS
                If EnteredTabPageName.ToString.Contains(My.Resources.TAB_SUMMARY) Then
                    cDynamicsSummary.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp)
                Else
                    cDynamicsGeneral.DisplayToolStripData(PanelToolStrip, PanelTabCntl, ToolStp, UserInterface.cDynamics.Dynamics, SelectedNodeName)
                End If
        End Select
    End Sub

End Class
