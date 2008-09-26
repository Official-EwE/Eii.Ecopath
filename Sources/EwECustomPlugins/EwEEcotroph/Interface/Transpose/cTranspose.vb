Option Explicit On
Option Strict On

Imports System.Windows.Forms

Namespace UserInterface

    Public Class cTranspose

#Region "Public fields"
        Public m_EcotrophManager As cEcotrophManager
        Public m_PanelToolStrip As Panel
        Public m_PanelTabCntl As Panel
#End Region 'Public fields

#Region "Private fields"
        Private Const NUM_TAB_PAGE As Integer = 6
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
            Dim TabPgNum As Integer

            'DisplaySigmaLN()
            'DisplayProportion()
            SetUpToolStrip()
            'Retrieve tab control 
            TabCntl = TabControlFn(TabPgNum)
            'Retrieve data grid and display Catches (sum over group)
            DisplayTransposeCatches(TabCntl, TabPgNum)
            For FN As Integer = 1 To m_Transpose.m_EcotrophManager.EcopathData.NumFleet
                'Retrieve data grid and display Catch
                DisplayTransposeCatch(TabCntl, TabPgNum, FN)
            Next
            'Retrieve data grid and display Flow
            DisplayTransposeFlow(TabCntl, TabPgNum)
            'Retrieve data grid and display TransposeBiomass
            DisplayTransposeBiomass(TabCntl, TabPgNum)
            'Retrieve data grid and display ProportionSTD
            DisplayProportionSTD(TabCntl, TabPgNum)
        End Sub
#End Region 'Public methods

#Region "Helper methods"
        Private Shared Sub SetUpToolStrip()
            Dim ToolStp As ToolStrip
            Dim ToolStpLbl1 As ToolStripLabel
            Dim ToolStpLbl2 As ToolStripLabel
            Dim ToolStpTxtBox As ToolStripTextBox
            Dim ToolStpPrgBar As ToolStripProgressBar
            Dim ToolStpBtn As ToolStripButton

            ToolStp = CType(m_Transpose.m_PanelToolStrip.Controls("tsEcotroph"), ToolStrip)
            ToolStpLbl1 = CType(ToolStp.Items("tslblSmoothFactor"), ToolStripLabel)
            ToolStpLbl2 = CType(ToolStp.Items("tslblProgressBar"), ToolStripLabel)
            ToolStpTxtBox = CType(ToolStp.Items("tstbxSmoothFactor"), ToolStripTextBox)
            ToolStpPrgBar = CType(ToolStp.Items("tspgbProgressBar"), ToolStripProgressBar)
            ToolStpBtn = CType(ToolStp.Items("tsbtnCalculate"), ToolStripButton)

            ToolStp.Visible = True
            ToolStpLbl1.Visible = True
            ToolStpLbl1.Text = My.Resources.LBL_SMOOTH_FACTOR
            ToolStpTxtBox.Visible = True
            ToolStpTxtBox.Text = CStr(m_Transpose.m_EcotrophManager.SmoothFactor)
            ToolStpLbl2.Visible = False
            ToolStpPrgBar.Visible = False
            ToolStpBtn.Text = My.Resources.BTN_CALCULATE
            ToolStpBtn.Visible = True

            ToolStp.Refresh()
        End Sub

        Private Shared Function TabControlFn(ByRef TabPgNum As Integer) As TabControl
            Dim TabCntl As TabControl

            TabCntl = CType(m_Transpose.m_PanelTabCntl.Controls("tcEcotroph"), TabControl)
            If TabCntl.Controls.Count > NUM_TAB_PAGE Then
                For TabPgNum = TabCntl.Controls.Count To (NUM_TAB_PAGE + 1) Step -1
                    TabCntl.Controls.RemoveByKey("tpEcotroph" & TabPgNum)
                Next
            End If
            TabCntl.Visible = True
            TabPgNum = 0
            Return TabCntl
        End Function

        Private Shared Sub DisplayTransposeCatches(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_CATCHES
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            DataGrid.ReadOnly = True
            cTransposeCatches.DisplayData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplayTransposeCatch(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer, _
          ByVal FleetNumber As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_CATCH & " (" & m_Transpose.m_EcotrophManager.EcopathData. _
              FleetName(FleetNumber) & ")"
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            DataGrid.ReadOnly = True
            Grid.cTransposeCatch.DisplayData(DataGrid, m_Transpose, FleetNumber)
        End Sub

        Private Shared Sub DisplayTransposeFlow(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_FLOW
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            DataGrid.ReadOnly = True
            cTransposeFlow.DisplayData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplayTransposeBiomass(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_TRANSP_BIOMASS
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            DataGrid.ReadOnly = True
            cTransposeBiomass.DisplayData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplayProportionSTD(ByVal TabCntl As TabControl, ByRef TabPgNum As Integer)
            Dim TabPg As TabPage
            Dim DataGrid As DataGridView

            TabPgNum = TabPgNum + 1
            TabPg = CType(TabCntl.Controls("tpEcotroph" & TabPgNum), TabPage)
            TabPg.Text = My.Resources.TAB_PROPORT_STD
            DataGrid = CType(TabPg.Controls("dgvEcotroph" & TabPgNum), DataGridView)
            DataGrid.ReadOnly = True
            cProportionSTD.DisplayData(DataGrid, m_Transpose)
        End Sub

        Private Shared Sub DisplaySigmaLN()
            Console.WriteLine("SigmaLN")
            For i As Integer = 1 To m_Transpose.m_EcotrophManager.SigmaLN.GetUpperBound(0)
                Console.Write(m_Transpose.m_EcotrophManager.SigmaLN(i) & "  ")
            Next
            Console.WriteLine()
        End Sub

        Private Shared Sub DisplayProportion()
            Console.WriteLine("Proportion")
            For i As Integer = 1 To m_Transpose.m_EcotrophManager.Proportion.GetUpperBound(0)
                For j As Integer = 1 To m_Transpose.m_EcotrophManager.Proportion.GetUpperBound(1)
                    Console.Write(m_Transpose.m_EcotrophManager.Proportion(i, j) & "  ")
                Next
                Console.WriteLine()
            Next
            Console.WriteLine()
        End Sub
#End Region

    End Class

End Namespace
