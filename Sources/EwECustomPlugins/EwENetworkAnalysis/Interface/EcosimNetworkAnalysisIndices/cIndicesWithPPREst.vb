'==============================================================================
'
' $Log: cIndicesWithPPREst.vb,v $
' Revision 1.1  2008/09/26 07:30:51  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.13  2008/09/09 14:44:49  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.12  2008/06/25 02:25:22  joeh
' Compute and send ecosim NA data to csv file - Take 2
'
' Revision 1.11  2008/06/25 01:53:41  joeh
' Ecosim NA indice plots are displayed in the same form where we have the NA tree view - Take 2
'
' Revision 1.10  2008/06/24 00:52:27  joeh
' Ecosim NA indice plots are no longer displayed in a pop up form, rather they are displayed in the same form where  we have the NA tree view
'
' Revision 1.9  2008/06/18 20:16:02  joeh
' Plot Ascendency on flow in a second pane
'
' Revision 1.8  2008/06/14 03:47:25  joeh
' Compute and send Ecosim NA data to csv file
'
' Revision 1.7  2007/07/06 00:44:59  joeh
' Move hard coded strings to resource file
'
' Revision 1.6  2007/06/28 19:23:28  joeh
' Switch to wait cursor when displaying data
'
' Revision 1.5  2007/06/22 19:12:46  joeh
' Modify GetInstance()
'
' Revision 1.4  2007/06/22 00:35:30  joeh
' Add Option Strict On and Option Explicit On
'
' Revision 1.3  2007/06/20 23:34:01  joeh
' Add Panel as a new argument in GetInstance() and New()
'
' Revision 1.2  2007/06/20 18:13:58  joeh
' add header to the top of the file so that CVS will log the file with every update
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports EwEUtils.Commands

#End Region ' Imports

Public Class cIndicesWithPPREst
    Public Event AddToolStrip()

    Private Shared m_IndicesWithPPREstInstance As cIndicesWithPPREst
    Private m_NetworkManager As cNetworkManager
    'Private m_Panel As Windows.Forms.Panel
    Private Shared m_Panel As Windows.Forms.Panel

    Public Shared Function GetInstance(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel) As cIndicesWithPPREst
        m_Panel = Panel

        If m_IndicesWithPPREstInstance Is Nothing Then m_IndicesWithPPREstInstance = New cIndicesWithPPREst(NetworkManager, Panel)
        Return m_IndicesWithPPREstInstance
    End Function

    Private Sub New()
        '
    End Sub

    Private Sub New(ByVal NetworkManager As cNetworkManager, ByVal Panel As Windows.Forms.Panel)
        Me.New()
        m_NetworkManager = NetworkManager
        m_Panel = Panel
    End Sub

    Public Sub SetUpPanel(ByVal IsEcosimNetworkAnalysisSuccess As Boolean)
        SetUpToolStrip(IsEcosimNetworkAnalysisSuccess)

        SetUpGrid(IsEcosimNetworkAnalysisSuccess)
    End Sub

    Public Sub CreatePlot(ByVal Frm As Form, ByVal Zgc As ZedGraphControl)
        Dim Panes As MasterPane = Zgc.MasterPane
        Dim Pane1 As GraphPane = New ZedGraph.GraphPane
        Dim Pane2 As GraphPane = New ZedGraph.GraphPane
        Dim Graphic As Graphics

        Panes.PaneList.Clear()
        Panes.Add(Pane1)
        Panes.Add(Pane2)

        'Pane1
        InitializePane(Pane1, My.Resources.LBL_TIME_STEP, My.Resources.LBL_NA_INDIC)
        'Add curves
        Zgc.MasterPane(0).CurveList.Clear()
        'FIB
        AddCurve(My.Resources.LBL_FIB_INDX, m_NetworkManager.FIB, Pane1, Color.Green)
        'Relative sum of catch
        AddCurve(My.Resources.LBL_TOTAL_CATCH, m_NetworkManager.RelativeSumOfCatchPlot, Pane1, Color.Red)
        'Relative Kemptons
        AddCurve(My.Resources.LBL_KEMPTONS_Q, m_NetworkManager.RelativeKemptonsPlot, Pane1, Color.Blue)
        'TL catch
        AddCurve(My.Resources.LBL_TL_CATCH, m_NetworkManager.TLCatchPlot, Pane1, Color.Black)
        'FCI
        AddCurve(My.Resources.LBL_FCI, m_NetworkManager.FCIEcosim, Pane1, Color.Brown)
        'Catch PPR
        AddCurve(My.Resources.LBL_CATCH_PPR, m_NetworkManager.RelativeCatchPPRPlot, Pane1, Color.Violet)
        'Catch detritus required
        AddCurve(My.Resources.LBL_CATCH_DET_REQ, m_NetworkManager.RelativeDetritusReqPlot, Pane1, Color.Orange)

        'Pane2
        InitializePane(Pane2, My.Resources.LBL_TIME_STEP, My.Resources.LBL_NA_INDIC)
        'Add curves
        Zgc.MasterPane(1).CurveList.Clear()
        'Ascendency on flow
        AddCurve(My.Resources.LBL_ASCEND_FLOW, m_NetworkManager.AscendFlowEcosim, Pane2, Color.Gold)

        Zgc.AxisChange()
        Zgc.Refresh()

        Graphic = Frm.CreateGraphics
        Panes.AxisChange(Graphic)
        Panes.SetLayout(Graphic, PaneLayout.SingleColumn)

        Cursor.Current = Cursors.Default
    End Sub

    Public Sub ExtractToCSV()
        Dim myStream As StreamWriter
        Dim cmdh As CommandHandler = CommandHandler.GetInstance()
        Dim cmdFS As FileSaveCommand = DirectCast(cmdh.GetCommand(FileSaveCommand.COMMAND_NAME), FileSaveCommand)

        cmdFS.Invoke("csv files (*.csv)|*.csv|text files (*.txt)|*.txt|All files (*.*)|*.*", 1)

        If cmdFS.Result = Windows.Forms.DialogResult.OK Then
            m_Panel.Refresh()
            myStream = New StreamWriter(cmdFS.FileName)
            If (myStream IsNot Nothing) Then
                myStream.Write(ExtractData)
                myStream.Close()
            End If
        End If
    End Sub

    Private Sub SetUpGrid(ByVal IsEcosimNetworkAnalysisSuccess As Boolean)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
            CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)
        Dim LogoPanel As Windows.Forms.TableLayoutPanel = _
            CType(m_Panel.Controls("tlpNetworkAnalysis"), Windows.Forms.TableLayoutPanel)

        LogoPanel.Visible = False
        DataGrid.Visible = False

        If IsEcosimNetworkAnalysisSuccess = True Then
            GraphPane.Visible = True
        Else
            GraphPane.Visible = False
        End If
    End Sub

    Private Sub SetUpToolStrip(ByVal IsEcosimNetworkAnalysisSuccess As Boolean)
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim ToolStripLabel1 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel2 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripLabel3 As Windows.Forms.ToolStripLabel = New Windows.Forms.ToolStripLabel
        Dim ToolStripCombo1 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripCombo2 As Windows.Forms.ToolStripComboBox = New Windows.Forms.ToolStripComboBox
        Dim ToolStripPrgBar As Windows.Forms.ToolStripProgressBar = New Windows.Forms.ToolStripProgressBar
        Dim ToolStripButton1 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton
        Dim ToolStripButton2 As Windows.Forms.ToolStripButton = New Windows.Forms.ToolStripButton

        RemoveToolStrip()

        If IsEcosimNetworkAnalysisSuccess = True Then
            RaiseEvent AddToolStrip()

            ToolStrip = CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
            ToolStripLabel1 = CType(ToolStrip.Items("tslblSelection1"), Windows.Forms.ToolStripLabel)
            ToolStripLabel2 = CType(ToolStrip.Items("tslblSelection2"), Windows.Forms.ToolStripLabel)
            ToolStripLabel3 = CType(ToolStrip.Items("tslblProgressBar"), Windows.Forms.ToolStripLabel)
            ToolStripCombo1 = CType(ToolStrip.Items("tscmbSelection1"), Windows.Forms.ToolStripComboBox)
            ToolStripCombo2 = CType(ToolStrip.Items("tscmbSelection2"), Windows.Forms.ToolStripComboBox)
            ToolStripPrgBar = CType(ToolStrip.Items("tspgbProgressBar"), Windows.Forms.ToolStripProgressBar)
            ToolStripButton1 = CType(ToolStrip.Items("tsbtnCancel"), Windows.Forms.ToolStripButton)
            ToolStripButton2 = CType(ToolStrip.Items("tsbtnOutputIndicesCSV"), Windows.Forms.ToolStripButton)

            ToolStripLabel1.Visible = False
            ToolStripCombo1.Visible = False

            ToolStripLabel2.Visible = False
            ToolStripCombo2.Visible = False

            ToolStripLabel3.Visible = False
            ToolStripPrgBar.Visible = False
            ToolStripButton1.Visible = False
            ToolStripButton2.Visible = True

            ToolStrip.Refresh()
        End If
    End Sub

    Private Sub RemoveToolStrip()
        Dim ToolStrip As Windows.Forms.ToolStrip = _
            CType(m_Panel.Controls("tsNetworkAnalysis"), Windows.Forms.ToolStrip)
        Dim DataGrid As Windows.Forms.DataGridView = _
            CType(m_Panel.Controls("dgvNetworkAnalysis"), Windows.Forms.DataGridView)
        Dim GraphPane As ZedGraphControl = _
                    CType(m_Panel.Controls("zgcNetworkAnalysis"), ZedGraphControl)

        If Not ToolStrip Is Nothing Then
            m_Panel.Controls.RemoveByKey("tsNetworkAnalysis")
            DataGrid.Dock = Windows.Forms.DockStyle.Fill
            GraphPane.Dock = DockStyle.Fill
        End If
    End Sub

    Private Function ExtractData() As String
        Dim str As String = ""

        str = str + My.Resources.COL_HDR_THROUGHPUT + ", "
        str = str + My.Resources.COL_HDR_CAPACITY_ECOSIM + ", "
        str = str + My.Resources.COL_HDR_ASCEND_IMPORT + ", "
        str = str + My.Resources.COL_HDR_ASCEND_FLOW + ", "
        str = str + My.Resources.COL_HDR_ASCEND_EXPORT + ", "
        str = str + My.Resources.COL_HDR_ASCEND_RESP + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_IMPORT + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_FLOW + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_EXPORT + ", "
        str = str + My.Resources.COL_HDR_OVERHEAD_RESP + ", "
        str = str + My.Resources.COL_HDR_PCI + ", "
        str = str + My.Resources.COL_HDR_FCI + ", "
        str = str + My.Resources.COL_HDR_PATH_LEN + ", "
        str = str + My.Resources.COL_HDR_EXPORT + ", "
        str = str + My.Resources.COL_HDR_RESP_ECOSIM + ", "
        str = str + My.Resources.COL_HDR_PRIM_PROD + ", "
        str = str + My.Resources.COL_HDR_PROD + ", "
        str = str + My.Resources.COL_HDR_BIOMASS + ", "
        str = str + My.Resources.COL_HDR_CATCH + ", "
        str = str + My.Resources.COL_HDR_PROP_FLOW_DET + ", "
        str = str + My.Resources.COL_HDR_CATCH_PPR + ", "
        str = str + My.Resources.COL_HDR_CATCH_DET_REQ + ", "
        str = str + My.Resources.COL_HDR_ASCEND_TOTAL + ", "
        str = str + My.Resources.COL_HDR_AMI + ", "
        str = str + My.Resources.COL_HDR_ENTROPY + ", "
        str = str + vbCrLf

        For i As Integer = 1 To m_NetworkManager.nEcosimTimesteps
            str = str + m_NetworkManager.ThroughputEcosim(i).ToString + ", "
            str = str + m_NetworkManager.CapacityEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendImportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendFlowEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendRespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadImportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadFlowEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.OverheadRespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PCIEcosim(i).ToString + ", "
            str = str + m_NetworkManager.FCIEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PathLengthEcosim(i).ToString + ", "
            str = str + m_NetworkManager.ExportEcosim(i).ToString + ", "
            str = str + m_NetworkManager.RespEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PrimaryProdEcosim(i).ToString + ", "
            str = str + m_NetworkManager.ProdEcosim(i).ToString + ", "
            str = str + m_NetworkManager.BiomassEcosim(i).ToString + ", "
            str = str + m_NetworkManager.CatchEcosim(i).ToString + ", "
            str = str + m_NetworkManager.PropFlowDetEcosim(i).ToString + ", "
            str = str + m_NetworkManager.RaiseToPPEcosim(i).ToString + ", "
            str = str + m_NetworkManager.RaiseToDetEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AscendTotalEcosim(i).ToString + ", "
            str = str + m_NetworkManager.AMIEcosim(i).ToString + ", "
            str = str + m_NetworkManager.EntropyEcosim(i).ToString + ", "
            str = str + vbCrLf
        Next

        Return str
    End Function

End Class


