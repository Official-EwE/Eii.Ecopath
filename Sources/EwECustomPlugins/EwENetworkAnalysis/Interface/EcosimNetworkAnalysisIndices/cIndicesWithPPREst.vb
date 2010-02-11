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

<CLSCompliant(False)> _
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
        Me.m_zgh.Attach(Me.NetworkManager.UIContext, Me.Graph, 2)
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


