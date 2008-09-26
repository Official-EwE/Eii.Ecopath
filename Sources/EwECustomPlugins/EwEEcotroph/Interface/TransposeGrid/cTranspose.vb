'==============================================================================
'
' $Log: cTranspose.vb,v $
' Revision 1.1  2008/09/26 07:30:41  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.33  2008/06/05 19:43:45  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace UserInterface

    Public Class cTranspose

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
        Public m_Tree As TreeView
        Public m_TabPages(cUtility.NUM_TAB_PAGE_DESIGNER) As TabPage
#End Region 'Public fields

#Region "Private fields"
        Private Const NUM_TAB_PAGE_EXCLUDE_FLEET_PAGES As Integer = 6
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
        Public Shared Sub DisplayTransposeAEF()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'DisplaySigmaLN()
            'DisplayProportion()
            'SetUpToolStrip(My.Resources.TREE_NODE_AUTO_EMPIR_FUNCT)
            'Retrieve tab control
            NumTabPg = NUM_TAB_PAGE_EXCLUDE_FLEET_PAGES + m_Transpose.m_EcotrophManager.EcopathData.NumFleet
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Biomass (sum over group)
            DisplayTransposeMainGridData(TabCntl, TabPgNum)
            'Retrieve data grid and display AccessBiomass
            DisplayAccessBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_AUTO_SMOOTH)
            'Retrieve data grid and display Flow
            DisplayTransposeFlowGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_AUTO_SMOOTH)
            'Retrieve data grid and display TransposeBiomass
            DisplayTransposeBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_AUTO_SMOOTH)
            'Retrieve data grid and display Catches (sum over group)
            DisplayTransposeCatchSumGridData(TabCntl, TabPgNum)
            For FN As Integer = 1 To m_Transpose.m_EcotrophManager.EcopathData.NumFleet
                'Retrieve data grid and display Catch
                DisplayTransposeCatchGridData(TabCntl, TabPgNum, FN, My.Resources.TREE_NODE_AUTO_SMOOTH)
            Next
            'Retrieve data grid and display ProportionSTD
            DisplayProportionSTDGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_AUTO_SMOOTH)

            cUtility.DisplayToolStripData(m_Transpose.m_PanelToolStrip, m_Transpose.m_PanelTabCntl, m_Transpose.m_ToolStrip, _
              m_Transpose.m_Tree.SelectedNode.Text, m_Transpose.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayTransposeOmniIdx()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'DisplayProportion()
            'SetUpToolStrip(My.Resources.TREE_NODE_OMIN_IDX)
            'Retrieve tab control 
            NumTabPg = NUM_TAB_PAGE_EXCLUDE_FLEET_PAGES + m_Transpose.m_EcotrophManager.EcopathData.NumFleet
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Biomass (sum over group)
            DisplayTransposeMainGridData(TabCntl, TabPgNum)
            'Retrieve data grid and display AccessBiomass
            DisplayAccessBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_OMNI_IDX)
            'Retrieve data grid and display Flow
            DisplayTransposeFlowGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_OMNI_IDX)
            'Retrieve data grid and display TransposeBiomass
            DisplayTransposeBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_OMNI_IDX)
            'Retrieve data grid and display Catches (sum over group)
            DisplayTransposeCatchSumGridData(TabCntl, TabPgNum)
            For FN As Integer = 1 To m_Transpose.m_EcotrophManager.EcopathData.NumFleet
                'Retrieve data grid and display Catch
                DisplayTransposeCatchGridData(TabCntl, TabPgNum, FN, My.Resources.TREE_NODE_OMNI_IDX)
            Next
            'Retrieve data grid and display ProportionSTD
            DisplayProportionSTDGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_OMNI_IDX)

            cUtility.DisplayToolStripData(m_Transpose.m_PanelToolStrip, m_Transpose.m_PanelTabCntl, m_Transpose.m_ToolStrip, _
              m_Transpose.m_Tree.SelectedNode.Text, m_Transpose.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayTransposeUserDefVal()
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'DisplayProportion()
            'SetUpToolStrip(My.Resources.TREE_NODE_USER_DEF_SIGMA)
            'Retrieve tab control 
            NumTabPg = NUM_TAB_PAGE_EXCLUDE_FLEET_PAGES + m_Transpose.m_EcotrophManager.EcopathData.NumFleet
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Biomass (sum over group)
            DisplayTransposeMainGridData(TabCntl, TabPgNum)
            'Retrieve data grid and display AccessBiomass
            DisplayAccessBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_USER_DEF_SIGMA)
            'Retrieve data grid and display Flow
            DisplayTransposeFlowGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_USER_DEF_SIGMA)
            'Retrieve data grid and display TransposeBiomass
            DisplayTransposeBiomassGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_USER_DEF_SIGMA)
            'Retrieve data grid and display Catches (sum over group)
            DisplayTransposeCatchSumGridData(TabCntl, TabPgNum)
            For FN As Integer = 1 To m_Transpose.m_EcotrophManager.EcopathData.NumFleet
                'Retrieve data grid and display Catch
                DisplayTransposeCatchGridData(TabCntl, TabPgNum, FN, My.Resources.TREE_NODE_USER_DEF_SIGMA)
            Next
            'Retrieve data grid and display ProportionSTD
            DisplayProportionSTDGridData(TabCntl, TabPgNum, My.Resources.TREE_NODE_USER_DEF_SIGMA)

            cUtility.DisplayToolStripData(m_Transpose.m_PanelToolStrip, m_Transpose.m_PanelTabCntl, m_Transpose.m_ToolStrip, _
              m_Transpose.m_Tree.SelectedNode.Text, m_Transpose.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub UpdateSmoothFactor(ByVal ToolStpTbx As ToolStripTextBox, ByRef IsValidSmoothFactor As cUtility.Valid)
            Try
                If CDbl(ToolStpTbx.Text) < 0.03 Or CDbl(ToolStpTbx.Text) > 1.0 Then
                    IsValidSmoothFactor = cUtility.Valid.F
                Else
                    m_Transpose.m_EcotrophManager.InputData.SmoothFactor = CSng(ToolStpTbx.Text)
                    WriteFile("SmoothFactor")
                    IsValidSmoothFactor = cUtility.Valid.T
                End If
            Catch ex As Exception
                IsValidSmoothFactor = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateSigmaAry(ByVal DataGrid As DataGridView, ByRef IsValidSigma As cUtility.Valid)
            If CStr(DataGrid.Item(1, 0).Value).Contains(My.Resources.CELL_SIGMA) Then
                Try
                    For Col As Integer = 1 To m_Transpose.m_EcotrophManager.TransposeBiomass.GetUpperBound(1)
                        m_Transpose.m_EcotrophManager.InputData.Sigma(Col) = CSng(DataGrid.Item(Col + 1, 0).Value)
                    Next
                    WriteFile("Sigma")
                    IsValidSigma = cUtility.Valid.T
                Catch ex As Exception
                    IsValidSigma = cUtility.Valid.F
                End Try
            Else
                'The selected data grid has no Sigma values 
                IsValidSigma = cUtility.Valid.NA
            End If
        End Sub

        Public Shared Sub UpdateAccessAry(ByVal SelectedNode As String, ByVal DataGrid As DataGridView, _
          ByRef IsValidAccess As cUtility.Valid)
            Dim RowNumAccess As Integer

            Select Case SelectedNode
                Case My.Resources.TREE_NODE_AUTO_SMOOTH
                    RowNumAccess = 0
                Case My.Resources.TREE_NODE_OMNI_IDX, My.Resources.TREE_NODE_USER_DEF_SIGMA
                    RowNumAccess = 1
            End Select

            If CStr(DataGrid.Item(1, RowNumAccess).Value).Contains(My.Resources.CELL_ACCESS) Then
                Try
                    For Col As Integer = 1 To m_Transpose.m_EcotrophManager.AccessBiomass.GetUpperBound(1)
                        m_Transpose.m_EcotrophManager.InputData.Access(Col) = CSng(DataGrid.Item(Col + 1, RowNumAccess).Value)
                    Next
                    WriteFile("Access")
                    IsValidAccess = cUtility.Valid.T
                Catch ex As Exception
                    IsValidAccess = cUtility.Valid.F
                End Try
            Else
                'The selected data grid has no Access values 
                IsValidAccess = cUtility.Valid.NA
            End If
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Function TabControlFn(ByVal NumTabPg As Integer, ByRef TabPgNum As Integer) As TabControl
            Dim TabCntl As TabControl

            cUtility.SetTabControlPropertyDefault(m_Transpose.m_PanelTabCntl, m_Transpose.m_TabPages)

            TabCntl = CType(m_Transpose.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
            If TabCntl.Controls.Count > NumTabPg Then
                For TabPgNum = TabCntl.Controls.Count To (NumTabPg + 1) Step -1
                    TabCntl.Controls.RemoveByKey("tpEcotroph" & TabPgNum)
                Next
            End If
            TabCntl.Visible = True
            TabPgNum = 0
            Return TabCntl
        End Function

        Private Shared Sub DisplayTransposeMainGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            'cTransposeMain.DisplayToolStripData(m_Transpose.m_PanelToolStrip, m_Transpose.m_PanelTabCntl, _
            '  m_Transpose.m_ToolStrip, m_Transpose)

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_MAIN
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cTransposeMain.DisplayGridData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplayAccessBiomassGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
                  ByVal Algor As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_ACCESS_BIOMASS
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cAccessBiomass.DisplayGridData(DataGrid, m_Transpose, Algor)
        End Sub

        Private Shared Sub DisplayTransposeCatchSumGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_CATCHES
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cTransposeCatches.DisplayGridData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplayTransposeCatchGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
          ByVal FleetNumber As Integer, ByVal Algor As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_CATCH & " (" & m_Transpose.m_EcotrophManager.EcopathData. _
              FleetName(FleetNumber) & ")"
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cTransposeCatch.DisplayGridData(DataGrid, m_Transpose, FleetNumber, Algor)
        End Sub

        Private Shared Sub DisplayTransposeFlowGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
          ByVal Algor As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_PROD
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cTransposeFlow.DisplayGridData(DataGrid, m_Transpose, Algor)
        End Sub

        Private Shared Sub DisplayTransposeBiomassGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
          ByVal Algor As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_TRANSP_BIOMASS
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cTransposeBiomass.DisplayGridData(DataGrid, m_Transpose, Algor)
        End Sub

        Private Shared Sub DisplayProportionSTDGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
          ByVal Algor As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_PROPORT_STD
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cProportionSTD.DisplayGridData(DataGrid, m_Transpose, Algor)
        End Sub

        Private Shared Sub DisplaySigmaLNGridData()
            Console.WriteLine("SigmaLN")
            For i As Integer = 1 To m_Transpose.m_EcotrophManager.SigmaLN.GetUpperBound(0)
                Console.Write(m_Transpose.m_EcotrophManager.SigmaLN(i) & "  ")
            Next
            Console.WriteLine()
        End Sub

        Private Shared Sub DisplayProportionGridData()
            Console.WriteLine("Proportion")
            For i As Integer = 1 To m_Transpose.m_EcotrophManager.Proportion.GetUpperBound(0)
                For j As Integer = 1 To m_Transpose.m_EcotrophManager.Proportion.GetUpperBound(1)
                    Console.Write(m_Transpose.m_EcotrophManager.Proportion(i, j) & "  ")
                Next
                Console.WriteLine()
            Next
            Console.WriteLine()
        End Sub

        Private Shared Sub WriteFile(ByVal FileName As String)
            m_Transpose.m_EcotrophManager.InputData.WriteFile(FileName, m_Transpose.m_EcotrophManager)
        End Sub
#End Region 'Helper methods

    End Class

End Namespace
