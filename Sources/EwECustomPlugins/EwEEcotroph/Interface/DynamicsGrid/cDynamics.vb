'==============================================================================
'
' $Log: cDynamics.vb,v $
' Revision 1.1  2008/09/26 07:30:40  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.24  2008/06/05 19:43:48  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace UserInterface

    Public Class cDynamics

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
        Public m_Tree As TreeView
        Public m_TabPages(cUtility.NUM_TAB_PAGE_DESIGNER) As TabPage
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
        Public Shared Sub DisplayDynamicsParameter(ByVal MainFrom As String)
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 2
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Diagnosis parameter
            DisplayBasicParamGridData(TabCntl, TabPgNum, MainFrom)
            'Retrieve data grid and display Interpolated parameter
            DisplayIntrpParamGridData(TabCntl, TabPgNum, MainFrom)

            cUtility.DisplayToolStripData(m_Dynamics.m_PanelToolStrip, m_Dynamics.m_PanelTabCntl, m_Dynamics.m_ToolStrip, _
              m_Dynamics.m_Tree.SelectedNode.Text, m_Dynamics.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayDynamics(ByVal CatchHistoryType As String)
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 12
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Summary
            DisplaySummaryGridData(TabCntl, TabPgNum, CatchHistoryType)
            'Retrieve data grid and display Catches
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_CATCHES, CatchHistoryType)
            'Retrieve data grid and display Biomass
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_BIOMASS, CatchHistoryType)
            'Retrieve data grid and display Flow
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_PROD, CatchHistoryType)
            'Retrieve data grid and display Kinetic
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_KINETIC, CatchHistoryType)
            'Retrieve data grid and display FishLossRate
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_FISH_LOSS_RATE, CatchHistoryType)
            'Retrieve data grid and display FishMortality
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_FISH_MORTALITY, CatchHistoryType)
            'Retrieve data grid and display AccessBiomass
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_ACCESS_BIOMASS, CatchHistoryType)
            'Retrieve data grid and display AccessFishLossRate
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_ACCESS_FISH_LOSS_RATE, CatchHistoryType)
            'Retrieve data grid and display AccessFlow
            DisplayDynamicsGridData(TabCntl, TabPgNum, My.Resources.TAB_ACCESS_PROD, CatchHistoryType)
            'Retrieve data grid and display Kinetic_Recal
            DisplayDynamicsGridData(TabCntl, TabPgNum, "Kinetic_Recal", CatchHistoryType)
            'Retrieve data grid and display Bpred
            DisplayDynamicsGridData(TabCntl, TabPgNum, "Bpred", CatchHistoryType)

            cUtility.DisplayToolStripData(m_Dynamics.m_PanelToolStrip, m_Dynamics.m_PanelTabCntl, m_Dynamics.m_ToolStrip, _
              m_Dynamics.m_Tree.SelectedNode.Text, m_Dynamics.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub UpdateDynamicsParameter(ByVal CbxMain As ToolStripComboBox, ByVal TbxBeta As ToolStripTextBox, _
        ByVal SelTabPg As TabPage, ByVal DataGrid As DataGridView, ByRef IsValidDynamicsParameter As cUtility.Valid)
            Try
                Select Case TbxBeta.Visible
                    Case False
                        If CbxMain.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then
                            IsValidDynamicsParameter = cUtility.Valid.F
                        Else
                            IsValidDynamicsParameter = cUtility.Valid.T
                        End If
                    Case True
                        For Row As Integer = 1 To DataGrid.RowCount 'm_Dynamics.m_EcotrophManager.InputData.DynamicsTopD.GetUpperBound(0)
                            m_Dynamics.m_EcotrophManager.InputData.DynamicsTopD(Row) = CSng(DataGrid.Item(15 - 1, Row - 1).Value)
                            m_Dynamics.m_EcotrophManager.InputData.DynamicsFormD(Row) = CSng(DataGrid.Item(16 - 1, Row - 1).Value)
                        Next

                        If CbxMain.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Or _
                          CDbl(TbxBeta.Text) < 0.0 Or CDbl(TbxBeta.Text) > 1.0 Then
                            IsValidDynamicsParameter = cUtility.Valid.F
                        Else
                            m_Dynamics.m_EcotrophManager.InputData.DynamicsBeta = CSng(TbxBeta.Text)
                            WriteFile("DynamicsParameter")
                            IsValidDynamicsParameter = cUtility.Valid.T
                        End If
                End Select
            Catch ex As Exception
                IsValidDynamicsParameter = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateForecastYear(ByVal TbxRefYear As ToolStripTextBox, ByVal TbxNumYear As ToolStripTextBox, _
          ByRef IsValidForecastYear As cUtility.Valid)
            Try
                m_Dynamics.m_EcotrophManager.InputData.ReferenceYear = CInt((TbxRefYear.Text))
                m_Dynamics.m_EcotrophManager.InputData.NumForecastYear = CInt((TbxNumYear.Text))
                WriteFile("ForecastYear")
                IsValidForecastYear = cUtility.Valid.T
            Catch ex As Exception
                IsValidForecastYear = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateCatchMultiplierAry(ByVal DataGrid As DataGridView, ByRef IsValidCatchMultiplier As cUtility.Valid)
            'If CStr(DataGrid.Item(1, RowNumAccess).Value).Contains(My.Resources.CELL_ACCESS) Then
            Try
                If m_Dynamics.m_EcotrophManager.InputData.NumForecastYear <= DataGrid.Rows(0).Cells.Count - 2 Then
                    For Col As Integer = 1 To m_Dynamics.m_EcotrophManager.InputData.NumForecastYear
                        m_Dynamics.m_EcotrophManager.InputData.CatchMultiplier(Col) = CSng(DataGrid.Item(Col + 1, 0).Value)
                    Next
                Else
                    ReDim Preserve m_Dynamics.m_EcotrophManager.InputData.CatchMultiplier(m_Dynamics.m_EcotrophManager.InputData.NumForecastYear)
                    For Col As Integer = 1 To DataGrid.Rows(0).Cells.Count - 2
                        m_Dynamics.m_EcotrophManager.InputData.CatchMultiplier(Col) = CSng(DataGrid.Item(Col + 1, 0).Value)
                    Next
                    For col As Integer = DataGrid.Rows(0).Cells.Count - 2 + 1 To m_Dynamics.m_EcotrophManager.InputData.NumForecastYear
                        m_Dynamics.m_EcotrophManager.InputData.CatchMultiplier(col) = 1.5 'default value
                    Next
                End If
                WriteFile("CatchMultiplier")
                IsValidCatchMultiplier = cUtility.Valid.T
            Catch ex As Exception
                IsValidCatchMultiplier = cUtility.Valid.F
            End Try
            'Else
            ''The selected data grid has no Access values 
            'IsValidAccess = cUtility.Valid.NA
            'End If
        End Sub

        Public Shared Sub UpdateIndexPPForecastAry(ByVal DataGrid As DataGridView, ByRef IsValidIndexPPForecast As cUtility.Valid)
            Try
                If m_Dynamics.m_EcotrophManager.InputData.NumForecastYear <= DataGrid.Rows(1).Cells.Count - 2 Then
                    For Col As Integer = 1 To m_Dynamics.m_EcotrophManager.InputData.NumForecastYear
                        m_Dynamics.m_EcotrophManager.InputData.IndexPPForecast(Col) = CSng(DataGrid.Item(Col + 1, 1).Value)
                    Next
                Else
                    ReDim Preserve m_Dynamics.m_EcotrophManager.InputData.IndexPPForecast(m_Dynamics.m_EcotrophManager.InputData.NumForecastYear)
                    For Col As Integer = 1 To DataGrid.Rows(1).Cells.Count - 2
                        m_Dynamics.m_EcotrophManager.InputData.IndexPPForecast(Col) = CSng(DataGrid.Item(Col + 1, 1).Value)
                    Next
                    For col As Integer = DataGrid.Rows(1).Cells.Count - 2 + 1 To m_Dynamics.m_EcotrophManager.InputData.NumForecastYear
                        m_Dynamics.m_EcotrophManager.InputData.IndexPPForecast(col) = 1.0 'default value
                    Next
                End If
                WriteFile("IndexPPForecast")
                IsValidIndexPPForecast = cUtility.Valid.T
            Catch ex As Exception
                IsValidIndexPPForecast = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateIndexPPPastAnalysisAry(ByVal DataGrid As DataGridView, ByRef IsValidIndexPPPastAnalysis As cUtility.Valid)
            Try
                For Col As Integer = 1 To m_Dynamics.m_EcotrophManager.InputData.IndexPPPastAnalysis.GetUpperBound(0)
                    m_Dynamics.m_EcotrophManager.InputData.IndexPPPastAnalysis(Col) = CSng(DataGrid.Item(Col + 1, 1).Value)
                Next
                WriteFile("IndexPPPastAnalysis")
                IsValidIndexPPPastAnalysis = cUtility.Valid.T
            Catch ex As Exception
                IsValidIndexPPPastAnalysis = cUtility.Valid.F
            End Try
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Function TabControlFn(ByVal NumTabPg As Integer, ByRef TabPgNum As Integer) As TabControl
            Dim TabCntl As TabControl

            cUtility.SetTabControlPropertyDefault(m_Dynamics.m_PanelTabCntl, m_Dynamics.m_TabPages)

            TabCntl = CType(m_Dynamics.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
            If TabCntl.Controls.Count > NumTabPg Then
                For TabPgNum = TabCntl.Controls.Count To (NumTabPg + 1) Step -1
                    TabCntl.Controls.RemoveByKey("tpEcotroph" & TabPgNum)
                Next
            End If
            TabCntl.Visible = True
            TabPgNum = 0
            Return TabCntl
        End Function

        Private Shared Sub DisplayBasicParamGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal MainFrom As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_BASIC_PARAM
            If MainFrom <> My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then
                DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
                cDynamicsBasicParam.DisplayGridData(DataGrid, m_Dynamics)
            Else
                TabCntl.Visible = False
            End If
        End Sub

        Private Shared Sub DisplayIntrpParamGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal MainFrom As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_INTRP_PARAM
            If MainFrom <> My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then
                DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
                cDynamicsIntrpParam.DisplayGridData(DataGrid, m_Dynamics)
            Else
                TabCntl.Visible = False
            End If
        End Sub

        Private Shared Sub DisplaySummaryGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal CatchHistoryType As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_SUMMARY
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cDynamicsSummary.DisplayGridData(DataGrid, m_Dynamics, CatchHistoryType)
        End Sub

        Private Shared Sub DisplayDynamicsGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal TabPgText As String, _
          ByVal CatchHistoryType As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = TabPgText
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cDynamicsGeneral.DisplayGridData(TabPgText, DataGrid, m_Dynamics, CatchHistoryType)
        End Sub

        Private Shared Sub WriteFile(ByVal FileName As String)
            m_Dynamics.m_EcotrophManager.InputData.WriteFile(FileName, m_Dynamics.m_EcotrophManager)
        End Sub
#End Region 'Helper methods


    End Class

End Namespace
