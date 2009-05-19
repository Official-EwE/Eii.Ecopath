'==============================================================================
'
' $Log: cIndicesWithoutPPREst.vb,v $
' Revision 1.16  2009/05/19 13:41:11  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.15  2009/05/11 20:34:35  jeroens
' Added monthly / annual averages CVS export
'
' Revision 1.14  2009/05/11 02:12:39  jeroens
' Simplified default file name use for CSV files
' Uses new cDirectoryOpenCommand
'
' Revision 1.13  2009/05/11 00:33:55  jeroens
' Fixed extra comma in CSV results
'
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
' Revision 1.6  2009/04/14 18:21:10  joeh
' Add separator to tool strip
'
' Revision 1.5  2009/04/09 20:04:47  joeh
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
Imports EwECore
Imports EwEUtils
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cIndicesWithoutPPREst
    Inherits cContentManager

    Private m_zgh As ZedGraphHelper = Nothing

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot) As Boolean

        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot)

        Me.NetworkManager.UseEcosimNetwork = True
        Me.NetworkManager.EcosimPPROn = False
        bSucces = bSucces and Me.NetworkManager.RunEcosimNetwork() 
        Me.NetworkManager.UseEcosimNetwork = False

        Me.Graph.Visible = bSucces
        Return bSucces

    End Function

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

    Public Overrides Function RequiresToolstrip() As Boolean
        Return True
    End Function

    Public Overrides Sub SetUpToolStrip(ByVal ts As ToolStrip)

        MyBase.SetupToolstrip(ts)

        Dim tsbtnExport As ToolStripButton = DirectCast(ts.Items("tsbtnOutputIndicesCSV"), ToolStripButton)
        tsbtnExport.Visible = True
        ts.Refresh()

    End Sub

End Class



