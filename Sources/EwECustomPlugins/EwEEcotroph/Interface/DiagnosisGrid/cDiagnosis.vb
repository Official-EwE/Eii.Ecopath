'==============================================================================
'
' $Log: cDiagnosis.vb,v $
' Revision 1.1  2008/09/26 07:30:39  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.28  2008/06/05 19:43:47  joeh
' no message
'
'==============================================================================
Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace UserInterface

    Public Class cDiagnosis

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
        Public m_ToolStrip As ToolStrip
        Public m_Tree As TreeView
        Public m_TabPages(cUtility.NUM_TAB_PAGE_DESIGNER) As TabPage
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
        Public Shared Sub DisplayDiagnosisParameter(ByVal MainFrom As String)
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 1
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Diagnosis parameter
            DisplayBasicParamGridData(TabCntl, TabPgNum, MainFrom)

            cUtility.DisplayToolStripData(m_Diagnosis.m_PanelToolStrip, m_Diagnosis.m_PanelTabCntl, m_Diagnosis.m_ToolStrip, _
              m_Diagnosis.m_Tree.SelectedNode.Text, m_Diagnosis.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub DisplayDiagnosis(ByVal EffortMultiplierType As String)
            Dim TabCntl As TabControl
            Dim NumTabPg As Integer
            Dim TabPgNum As Integer

            'Retrieve tab control 
            NumTabPg = 7
            TabCntl = TabControlFn(NumTabPg, TabPgNum)
            'Retrieve data grid and display Summary
            DisplaySummaryGridData(TabCntl, TabPgNum, EffortMultiplierType)
            'Retrieve data grid and display Catches
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_CATCHES)
            'Retrieve data grid and display Biomass
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_BIOMASS)
            'Retrieve data grid and display AccessBiomass
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_ACCESS_BIOMASS)
            'Retrieve data grid and display Flow
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_PROD)
            'Retrieve data grid and display AccessFlow
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_ACCESS_PROD)
            'Retrieve data grid and display Kinetic
            DisplayDiagnosisGridData(TabCntl, TabPgNum, My.Resources.TAB_KINETIC)

            cUtility.DisplayToolStripData(m_Diagnosis.m_PanelToolStrip, m_Diagnosis.m_PanelTabCntl, m_Diagnosis.m_ToolStrip, _
              m_Diagnosis.m_Tree.SelectedNode.Text, m_Diagnosis.m_Tree.SelectedNode.Parent.Text, TabCntl.SelectedTab.Text)
        End Sub

        Public Shared Sub UpdateDiagnosisParameter(ByVal CbxMain As ToolStripComboBox, ByVal TbxBeta As ToolStripTextBox, _
          ByVal DataGrid As DataGridView, ByRef IsValidDiagnosisParameter As cUtility.Valid)
            Try
                Select Case TbxBeta.Visible
                    Case False
                        If CbxMain.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Then
                            IsValidDiagnosisParameter = cUtility.Valid.F
                        Else
                            IsValidDiagnosisParameter = cUtility.Valid.T
                        End If
                    Case True
                        For Row As Integer = 1 To m_Diagnosis.m_EcotrophManager.InputData.DiagnosisTopD.GetUpperBound(0)
                            m_Diagnosis.m_EcotrophManager.InputData.DiagnosisTopD(Row) = CSng(DataGrid.Item(15 - 1, Row - 1).Value)
                            m_Diagnosis.m_EcotrophManager.InputData.DiagnosisFormD(Row) = CSng(DataGrid.Item(16 - 1, Row - 1).Value)
                        Next

                        If CbxMain.Text = My.Resources.DROP_DWN_LST_ITM_PLS_SELECT Or _
                          CDbl(TbxBeta.Text) < 0.0 Or CDbl(TbxBeta.Text) > 1.0 Then
                            IsValidDiagnosisParameter = cUtility.Valid.F
                        Else
                            m_Diagnosis.m_EcotrophManager.InputData.DiagnosisBeta = CSng(TbxBeta.Text)
                            WriteFile("DiagnosisParameter")
                            IsValidDiagnosisParameter = cUtility.Valid.T
                        End If
                End Select
            Catch ex As Exception
                IsValidDiagnosisParameter = cUtility.Valid.F
            End Try
        End Sub

        Public Shared Sub UpdateEffortMultiplierAry(ByVal DataGrid As DataGridView, ByRef IsValidEffortMultiplier As cUtility.Valid)
            Try
                'Check if there is 1.0
                IsValidEffortMultiplier = cUtility.Valid.F
                For Col As Integer = 1 To m_Diagnosis.m_EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
                    If CDbl(DataGrid.Item(Col + 1, 0).Value) = 1.0 Then
                        IsValidEffortMultiplier = cUtility.Valid.T
                        Exit For
                    End If
                Next

                If IsValidEffortMultiplier = cUtility.Valid.T Then 'There is a 1
                    'Check range
                    For Col As Integer = 1 To m_Diagnosis.m_EcotrophManager.InputData.EffortMultiplier.GetUpperBound(0)
                        If CDbl(DataGrid.Item(Col + 1, 0).Value) < 0.0 Or CDbl(DataGrid.Item(Col + 1, 0).Value) > 20.0 Then 'Out of range
                            IsValidEffortMultiplier = cUtility.Valid.F
                            Exit Sub
                        Else 'Within range
                            m_Diagnosis.m_EcotrophManager.InputData.EffortMultiplier(Col) = CSng(DataGrid.Item(Col + 1, 0).Value)
                        End If
                    Next
                Else ' There is no 1
                    Exit Sub
                End If

                'There is a 1 and not out of range
                WriteFile("EffortMultiplier")
                IsValidEffortMultiplier = cUtility.Valid.T
            Catch ex As Exception
                IsValidEffortMultiplier = cUtility.Valid.F
            End Try
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Function TabControlFn(ByVal NumTabPg As Integer, ByRef TabPgNum As Integer) As TabControl
            Dim TabCntl As TabControl

            cUtility.SetTabControlPropertyDefault(m_Diagnosis.m_PanelTabCntl, m_Diagnosis.m_TabPages)

            TabCntl = CType(m_Diagnosis.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
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
                cDiagnosisBasicParam.DisplayGridData(DataGrid, m_Diagnosis)
            Else
                TabCntl.Visible = False
            End If
        End Sub

        Private Shared Sub DisplayDiagnosisGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal TabPgText As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = TabPgText
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cDiagnosisGeneral.DisplayGridData(TabPgText, DataGrid, m_Diagnosis)
        End Sub

        Private Shared Sub DisplaySummaryGridData(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, ByVal EffortMultiplierType As String)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_SUMMARY
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            cDiagnosisSummary.DisplayGridData(DataGrid, m_Diagnosis, EffortMultiplierType)
        End Sub

        Private Shared Sub WriteFile(ByVal FileName As String)
            m_Diagnosis.m_EcotrophManager.InputData.WriteFile(FileName, m_Diagnosis.m_EcotrophManager)
        End Sub
#End Region 'Helper methods

    End Class

End Namespace
