'==============================================================================
'
' $Log: cIndicesWithPPREst.vb,v $
' Revision 1.18  2009/05/30 00:00:54  jeroens
' Toolstrip usage centralized
'
' Revision 1.17  2009/05/28 13:59:57  jeroens
' Fixed annual averages option in CSV export
'
' Revision 1.16  2009/05/28 12:37:14  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.15  2009/05/19 13:41:11  jeroens
' Content manager derived pages will take care of updating NA run state
'
' Revision 1.14  2009/05/11 20:34:36  jeroens
' Added monthly / annual averages CVS export
'
' Revision 1.13  2009/05/11 02:12:39  jeroens
' Simplified default file name use for CSV files
' Uses new cDirectoryOpenCommand
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

    Private m_zgh As cZedGraphHelper = Nothing

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean

        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)

        ' PPR not on yet?
        If (Me.NetworkManager.EcosimPPROn = False) Then
            ' #Yes: prompt user if need to run
            bSucces = bSucces And (MsgBox(My.Resources.PROMPT_ESTIMATE_PPR, MsgBoxStyle.YesNo, My.Resources.CAPTION) = MsgBoxResult.Yes)
        End If

        ' Need to run?
        If bSucces Then
            ' #Yes: run std PP
            Me.NetworkManager.RunRequiredPrimaryProd()
            ' Switch on PPR in Ecosim
            Me.NetworkManager.UseEcosimNetwork = True
            Me.NetworkManager.EcosimPPROn = True
            ' Ecosim NA run succesful?
            bSucces = bSucces And Me.NetworkManager.RunEcosimNetwork()
            Me.NetworkManager.UseEcosimNetwork = False
        Else
            bSucces = False
        End If

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.NetworkManager.Core, Me.Graph, 2)
        Me.m_zgh.ShowPointValue = True

        Me.Graph.Visible = bSucces
        Me.Toolstrip.Visible = bSucces
        Me.ToolstripShowOptionCSV()

        Return bSucces

    End Function

    Public Overrides Sub Detach()

        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        MyBase.Detach()

    End Sub

    Public Overrides ReadOnly Property IsDataOverTime() As Boolean
        Get
            Return True
        End Get
    End Property

    Public Overrides Sub DisplayData()

        Dim paneMaster As MasterPane = Me.Graph.MasterPane
        Dim pane As GraphPane = Nothing
        Dim g As Graphics = Nothing

        'Pane1
        pane = Me.m_zgh.ConfigurePane("", My.Resources.LBL_MONTHS, My.Resources.LBL_NA_INDIC, True, LegendPos.TopCenter, 1)
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
        pane = Me.m_zgh.ConfigurePane("", My.Resources.LBL_MONTHS, My.Resources.LBL_NA_INDIC, True, LegendPos.TopCenter, 2)
        'Add curves
        pane.CurveList.Clear()
        'Ascendency on flow
        AddCurve(My.Resources.LBL_ASCEND_FLOW, Me.NetworkManager.AscendFlowEcosim, pane, Color.Gold)

        Me.m_zgh.RescaleAndRedraw()

        g = Me.Graph.Parent.CreateGraphics
        paneMaster.AxisChange(g)
        paneMaster.SetLayout(g, PaneLayout.SingleColumn)

    End Sub

End Class


