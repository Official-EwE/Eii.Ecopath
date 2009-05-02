'==============================================================================
'
' $Log: cIndicesWithPPREst.vb,v $
' Revision 1.12  2009/05/02 03:00:11  jeroens
' Scenario name included in default file name
' ExtractData uses string builder
'
' Revision 1.11  2009/05/01 17:43:00  jeroens
' Inherited from cContentManager
'
' Revision 1.10  2009/04/28 16:24:25  jeroens
' Fixed graph max axis
' Graph styling done with ZedGraphHelper
'
' Revision 1.9  2009/04/17 01:07:05  joeh
' Make MixedTrophicImpactUC not visible when needed
'
' Revision 1.8  2009/04/16 00:11:56  joeh
' Add "Imports System.Windows.Forms" statement
'
' Revision 1.7  2009/04/15 18:14:55  joeh
' Set m_Panel.AutoScroll = False
'
' Revision 1.6  2009/04/14 18:21:11  joeh
' Add separator to tool strip
'
' Revision 1.5  2009/04/09 20:04:48  joeh
' Add "Bar graph" button to plot bar graph for MTI
'
' Revision 1.4  2008/12/02 23:31:55  joeh
' Remove Zed graph control from the parameters of CreatePlot( )
'
' Revision 1.3  2008/11/28 01:58:33  joeh
' Implement new MTI plot and save MTI plot as emf file
'
' Revision 1.2  2008/11/10 05:34:54  jeroens
' Renamed file command
'
' Revision 1.1  2008/09/26 07:30:51  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports ZedGraph
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Text
Imports EwEUtils
Imports EwECore
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cIndicesWithPPREst
    Inherits cContentManager

    Private m_zgh As ZedGraphHelper = Nothing

    Public Overrides Sub Attach(ByVal manager As cNetworkManager, _
                                ByVal datagrid As DataGridView, _
                                ByVal graph As ZedGraphControl, _
                                ByVal plot As ucPlot)
        MyBase.Attach(manager, datagrid, graph, plot)
        Me.Graph.Visible = True
    End Sub

    Public Overrides Sub Detach()

        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        MyBase.Detach()

    End Sub

    Public Overrides Sub DisplayData()

        Dim zgc As ZedGraphControl = Me.Graph
        Dim paneMaster As MasterPane = zgc.MasterPane
        Dim pane As GraphPane = Nothing
        Dim g As Graphics = Nothing

        Me.m_zgh = New ZedGraphHelper()
        Me.m_zgh.Attach(Me.NetworkManager.Core, zgc, 2)
        Me.m_zgh.ShowPointValue = True

        'Pane1
        pane = Me.m_zgh.ConfigurePane("", My.Resources.LBL_TIME_STEP, My.Resources.LBL_NA_INDIC, True, LegendPos.TopCenter, 1)
        'Add curves
        pane.CurveList.Clear()
        'FIB
        AddCurve(My.Resources.LBL_FIB_INDX, Me.NetworkManager.FIB, pane, Color.Green)
        'Relative sum of catch
        AddCurve(My.Resources.LBL_TOTAL_CATCH, Me.NetworkManager.RelativeSumOfCatchPlot, pane, Color.Red)
        'Relative Kemptons
        AddCurve(My.Resources.LBL_KEMPTONS_Q, Me.NetworkManager.RelativeKemptonsPlot, pane, Color.Blue)
        'TL catch
        AddCurve(My.Resources.LBL_TL_CATCH, Me.NetworkManager.TLCatchPlot, pane, Color.Black)
        'FCI
        AddCurve(My.Resources.LBL_FCI, Me.NetworkManager.FCIEcosim, pane, Color.Brown)
        'Catch PPR
        AddCurve(My.Resources.LBL_CATCH_PPR, Me.NetworkManager.RelativeCatchPPRPlot, pane, Color.Violet)
        'Catch detritus required
        AddCurve(My.Resources.LBL_CATCH_DET_REQ, Me.NetworkManager.RelativeDetritusReqPlot, pane, Color.Orange)

        'Pane2
        pane = Me.m_zgh.ConfigurePane("", My.Resources.LBL_TIME_STEP, My.Resources.LBL_NA_INDIC, True, LegendPos.TopCenter, 2)
        'Add curves
        pane.CurveList.Clear()
        'Ascendency on flow
        AddCurve(My.Resources.LBL_ASCEND_FLOW, Me.NetworkManager.AscendFlowEcosim, pane, Color.Gold)

        zgc.AxisChange()
        zgc.Refresh()

        g = Me.Graph.Parent.CreateGraphics
        paneMaster.AxisChange(g)
        paneMaster.SetLayout(g, PaneLayout.SingleColumn)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Append base class file name with scenario name and NA form-specific tag.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Filename() As String

        Dim core As cCore = Me.NetworkManager.Core
        Dim scenario As cEcoSimScenario = Nothing
        Dim strFileName As String = MyBase.Filename()

        Try
            scenario = core.EcosimScenarios(core.ActiveEcosimScenarioIndex)
            strFileName &= "-" & EwEUtils.Utilities.FileUtilities.ToValidFileName(scenario.Name, False)
        Catch ex As Exception

        End Try

        Return strFileName & "-NA-PPR"

    End Function

    Public Overrides Sub SaveToCSV(ByVal strFileName As String)

        Dim sw As New StreamWriter(strFileName)
        If (sw IsNot Nothing) Then
            sw.Write(ExtractData)
            sw.Close()
        End If

    End Sub

    Public Overrides Function RequiresToolstrip() As Boolean
        Return True
    End Function

    Public Overrides Sub SetUpToolStrip(ByVal ts As ToolStrip)

        MyBase.SetupToolstrip(ts)

        Dim tsbtnExport As ToolStripButton = DirectCast(ts.Items("tsbtnOutputIndicesCSV"), ToolStripButton)
        tsbtnExport.Visible = True
        ts.Refresh()

    End Sub

    Private Function ExtractData() As String

        Dim sb As New StringBuilder()

        sb.Append(My.Resources.COL_HDR_THROUGHPUT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CAPACITY_ECOSIM)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_IMPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_FLOW)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_RESP)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_IMPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_FLOW)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_OVERHEAD_RESP)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PCI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_FCI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PATH_LEN)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_EXPORT)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_RESP_ECOSIM)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PRIM_PROD)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROD)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_BIOMASS)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_PROP_FLOW_DET)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH_PPR)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_CATCH_DET_REQ)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ASCEND_TOTAL)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_AMI)
        sb.Append(", ")
        sb.Append(My.Resources.COL_HDR_ENTROPY)
        sb.AppendLine("")

        For i As Integer = 1 To Me.NetworkManager.nEcosimTimesteps
            sb.Append(Me.NetworkManager.ThroughputEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.CapacityEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AscendImportEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AscendFlowEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AscendExportEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AscendRespEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.OverheadImportEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.OverheadFlowEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.OverheadExportEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.OverheadRespEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.PCIEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.FCIEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.PathLengthEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.ExportEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.RespEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.PrimaryProdEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.ProdEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.BiomassEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.CatchEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.PropFlowDetEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.RaiseToPPEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.RaiseToDetEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AscendTotalEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.AMIEcosim(i).ToString)
            sb.Append(", ")
            sb.Append(Me.NetworkManager.EntropyEcosim(i).ToString)
            sb.AppendLine("")
        Next

        Return sb.ToString()

    End Function

End Class


