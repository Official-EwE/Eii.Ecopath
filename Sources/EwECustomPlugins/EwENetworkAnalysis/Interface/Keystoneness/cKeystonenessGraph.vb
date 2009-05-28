'==============================================================================
'
' $Log: cKeystonenessGraph.vb,v $
' Revision 1.1  2009/05/28 13:40:11  jeroens
' Added keystoneness graph, renamed table content manager to prevent confusion
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

Public Class cKeystonenessGraph
    Inherits cContentManager

    Private m_zgh As cZedGraphHelper = Nothing

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot) As Boolean

        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot)
        Me.Graph.Visible = bSucces

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.NetworkManager.Core, Me.Graph, 1)
        Me.m_zgh.ShowPointValue = True

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
        Dim curve As CurveItem = Nothing
        Dim ppl As PointPairList = Nothing
        Dim source As cCoreInputOutputBase = Nothing

        ' ToDo: localize this
        pane = Me.m_zgh.ConfigurePane("", "Keystoneness", "Scaled impact", False)

        pane.CurveList.Clear()
        For iGroup As Integer = 1 To Me.NetworkManager.nLivingGroups

            ppl = New PointPairList()
            ppl.Add(Me.NetworkManager.ScaledImpact(iGroup), Me.NetworkManager.KeystoneIndex(iGroup))

            source = Me.NetworkManager.Core.EcoPathGroupInputs(iGroup)

            curve = pane.AddCurve(source.Name, ppl, _
                      Me.StyleGuide.GroupColor(Me.NetworkManager.Core, source.Index), _
                      SymbolType.Circle)

        Next

        Me.m_zgh.RescaleAndRedraw()

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
