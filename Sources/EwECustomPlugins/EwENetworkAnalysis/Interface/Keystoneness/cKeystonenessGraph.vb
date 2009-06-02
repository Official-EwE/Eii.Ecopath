'==============================================================================
'
' $Log: cKeystonenessGraph.vb,v $
' Revision 1.5  2009/06/02 02:44:52  jeroens
' Fixed label placement bug when displaying numbers
' Renamed keystoneness indicators
' Scaled circles to relative biomass to control max circle size
'
' Revision 1.4  2009/06/01 00:58:16  jeroens
' Hmm
'
' Revision 1.3  2009/05/30 00:08:44  jeroens
' Toolstrip usage centralized
' Added custom menu commands to style the graph
'
' Revision 1.2  2009/05/28 14:58:45  jeroens
' Styled graph
'
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

    Private Const iMAX_SYMBOL_SIZE As Integer = 100

    Private m_zgh As cZedGraphHelper = Nothing

    Public Sub New()
        '
    End Sub

    Public Overrides Function Attach(ByVal manager As cNetworkManager, _
                                     ByVal datagrid As DataGridView, _
                                     ByVal graph As ZedGraphControl, _
                                     ByVal plot As ucPlot, _
                                     ByVal toolstrip As ToolStrip) As Boolean
        Dim bSucces As Boolean = MyBase.Attach(manager, datagrid, graph, plot, toolstrip)

        Me.Graph.Visible = bSucces
        Me.Toolstrip.Visible = bSucces
        Me.ToolstripShowOptionCSV()
        Me.AddToolstrippybits()

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.NetworkManager.Core, Me.Graph, 1)
        Me.m_zgh.ShowPointValue = True

        Return bSucces

    End Function

    Public Overrides Sub Detach()

        Me.RemoveToolstrippybits()

        Me.m_zgh.Detach()
        Me.m_zgh = Nothing

        MyBase.Detach()

    End Sub

    Private Enum eGraphStyleType As Byte
        Circle
        CircleScaled
        Number
    End Enum

    Private m_graphstyle As eGraphStyleType = eGraphStyleType.Circle

    Private Property GraphStyle() As eGraphStyleType
        Get
            Return Me.m_graphstyle
        End Get
        Set(ByVal graphstyle As eGraphStyleType)
            If (Me.m_graphstyle <> graphstyle) Then
                Me.m_graphstyle = graphstyle
                Me.DisplayData()
            End If
        End Set
    End Property

    Private Enum eContentStyleType As Byte
        Keystoneness
        TotalImpactOverB
    End Enum

    Private m_contentstyle As eContentStyleType = eContentStyleType.Keystoneness

    Private Property ContentStyle() As eContentStyleType
        Get
            Return Me.m_contentstyle
        End Get
        Set(ByVal contentstyle As eContentStyleType)
            If (Me.m_contentstyle <> contentstyle) Then
                Me.m_contentstyle = contentstyle
                Me.DisplayData()
            End If
        End Set
    End Property

    Public Overrides Sub DisplayData()

        Dim pane As GraphPane = Nothing
        Dim li As LineItem = Nothing
        Dim curve As CurveItem = Nothing
        Dim ppl As PointPairList = Nothing
        Dim txt As ZedGraph.TextObj = Nothing
        Dim source As cCoreInputOutputBase = Nothing
        Dim sMaxB As Single = 0.0

        pane = Me.m_zgh.ConfigurePane("", My.Resources.LBL_RELTOTALIMPACT, My.Resources.LBL_KEYSTONENESS, False)
        pane.XAxis.Scale.Max = 1.0

        pane.CurveList.Clear()
        pane.GraphObjList.Clear()

        'Precalc max B (for CircleScaled style)
        For iGroup As Integer = 1 To Me.NetworkManager.nLivingGroups
            sMaxB = Math.Max(sMaxB, Me.NetworkManager.BiomassByGroup(iGroup))
        Next
        ' Avoid division by zero
        If sMaxB = 0 Then sMaxB = 1.0

        For iGroup As Integer = 1 To Me.NetworkManager.nLivingGroups

            ppl = New PointPairList()

            Select Case Me.ContentStyle
                Case eContentStyleType.Keystoneness
                    ppl.Add(Me.NetworkManager.RelativeTotalImpact(iGroup), Me.NetworkManager.KeystoneIndex(iGroup))
                Case eContentStyleType.TotalImpactOverB
                    ppl.Add(Me.NetworkManager.RelativeTotalImpact(iGroup), Me.NetworkManager.TotalImpactOverBiomass(iGroup))
            End Select

            source = Me.NetworkManager.Core.EcoPathGroupInputs(iGroup)

            Select Case Me.m_graphstyle

                Case eGraphStyleType.Circle

                    li = New LineItem(source.Name, ppl, Color.Black, SymbolType.Circle)
                    li.Line.Color = Color.Transparent
                    pane.CurveList.Add(li)

                Case eGraphStyleType.CircleScaled

                    li = New LineItem(source.Name, ppl, Color.Black, SymbolType.Circle)
                    li.Line.Color = Color.Transparent
                    If (Me.NetworkManager.BiomassByGroup(iGroup) > 0) Then
                        li.Symbol.Size = CSng(iMAX_SYMBOL_SIZE * Math.Sqrt(Me.NetworkManager.BiomassByGroup(iGroup) / sMaxB))
                        li.Symbol.Fill = New Fill(Me.StyleGuide.GroupColor(Me.NetworkManager.Core, source.Index))
                        pane.CurveList.Add(li)
                    End If

                Case eGraphStyleType.Number

                    ' Add hidden line for mouse value tracking
                    li = New LineItem(source.Name, ppl, Color.Transparent, SymbolType.None)
                    pane.CurveList.Add(li)

                    ' Add text label
                    txt = New ZedGraph.TextObj(CStr(source.Index), ppl(0).X, ppl(0).Y)

                    txt.ZOrder = ZOrder.E_BehindCurves
                    With txt.FontSpec
                        .Fill.IsVisible = False
                        .Border.IsVisible = False
                        '.FontColor = Me.StyleGuide.GroupColor(Me.NetworkManager.Core, source.Index)
                        .FontColor = Color.Black
                    End With

                    pane.GraphObjList.Add(txt)

            End Select

        Next

        Me.m_zgh.RescaleAndRedraw()
        Me.UpdateControls()

    End Sub

    Private m_tsStyle As ToolStripDropDownButton = Nothing
    Private m_tsmiCircles As ToolStripMenuItem = Nothing
    Private m_tsmiCirclesScaled As ToolStripMenuItem = Nothing
    Private m_tsmiNumbers As ToolStripMenuItem = Nothing

    Private m_tsContent As ToolStripDropDownButton = Nothing
    Private m_tsmiKeyst As ToolStripMenuItem = Nothing
    Private m_tsmiKeystOverB As ToolStripMenuItem = Nothing

    Private Sub AddToolstrippybits()

        Me.m_tsmiCircles = New ToolStripMenuItem(My.Resources.MNU_STYLE_CIRCLES)
        AddHandler Me.m_tsmiCircles.Click, AddressOf OnStyleCircles

        Me.m_tsmiCirclesScaled = New ToolStripMenuItem(My.Resources.MNU_STYLE_CIRCLES_SCALED)
        AddHandler Me.m_tsmiCirclesScaled.Click, AddressOf OnStyleCirclesScaled

        Me.m_tsmiNumbers = New ToolStripMenuItem(My.Resources.MNU_STYLE_NUMBERS)
        AddHandler Me.m_tsmiNumbers.Click, AddressOf OnStyleNumbers

        Me.m_tsStyle = New ToolStripDropDownButton(My.Resources.MNU_STYLE)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiCircles)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiCirclesScaled)
        Me.m_tsStyle.DropDownItems.Add(Me.m_tsmiNumbers)
        Me.Toolstrip.Items.Add(Me.m_tsStyle)

        Me.m_tsmiKeyst = New ToolStripMenuItem(My.Resources.MNU_CONTENT_KEYSTONE)
        AddHandler Me.m_tsmiKeyst.Click, AddressOf OnContentK

        Me.m_tsmiKeystOverB = New ToolStripMenuItem(My.Resources.MNU_CONTENT_TOTALIMPACT_OVER_BIOMASS)
        AddHandler Me.m_tsmiKeystOverB.Click, AddressOf OnContentKoverB

        Me.m_tsContent = New ToolStripDropDownButton(My.Resources.MNU_CONTENT)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiKeyst)
        Me.m_tsContent.DropDownItems.Add(Me.m_tsmiKeystOverB)
        Me.Toolstrip.Items.Add(Me.m_tsContent)

    End Sub

    Private Sub RemoveToolstrippybits()

        Me.Toolstrip.Items.Remove(Me.m_tsStyle)

        Me.m_tsStyle.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiCircles.Click, AddressOf OnStyleCircles
        Me.m_tsmiCircles = Nothing
        RemoveHandler Me.m_tsmiCirclesScaled.Click, AddressOf OnStyleCirclesScaled
        Me.m_tsmiCirclesScaled = Nothing
        RemoveHandler Me.m_tsmiNumbers.Click, AddressOf OnStyleNumbers
        Me.m_tsmiNumbers = Nothing
        Me.m_tsStyle = Nothing

        Me.Toolstrip.Items.Remove(Me.m_tsContent)

        Me.m_tsContent.DropDownItems.Clear()
        RemoveHandler Me.m_tsmiKeyst.Click, AddressOf OnContentK
        Me.m_tsmiKeyst = Nothing
        RemoveHandler Me.m_tsmiKeystOverB.Click, AddressOf OnContentKoverB
        Me.m_tsmiKeystOverB = Nothing
        Me.m_tsContent = Nothing

    End Sub

    Private Sub OnStyleCircles(ByVal sender As Object, ByVal arg As EventArgs)
        Me.GraphStyle = eGraphStyleType.Circle
    End Sub

    Private Sub OnStyleCirclesScaled(ByVal sender As Object, ByVal arg As EventArgs)
        Me.GraphStyle = eGraphStyleType.CircleScaled
    End Sub

    Private Sub OnStyleNumbers(ByVal sender As Object, ByVal arg As EventArgs)
        Me.GraphStyle = eGraphStyleType.Number
    End Sub

    Private Sub OnContentK(ByVal sender As Object, ByVal arg As EventArgs)
        Me.ContentStyle = eContentStyleType.Keystoneness
    End Sub

    Private Sub OnContentKoverB(ByVal sender As Object, ByVal arg As EventArgs)
        Me.ContentStyle = eContentStyleType.TotalImpactOverB
    End Sub

    Private Sub UpdateControls()

        Me.m_tsmiCircles.Checked = (Me.GraphStyle = eGraphStyleType.Circle)
        Me.m_tsmiCirclesScaled.Checked = (Me.GraphStyle = eGraphStyleType.CircleScaled)
        Me.m_tsmiNumbers.Checked = (Me.GraphStyle = eGraphStyleType.Number)

        Me.m_tsmiKeyst.Checked = (Me.ContentStyle = eContentStyleType.Keystoneness)
        Me.m_tsmiKeystOverB.Checked = (Me.ContentStyle = eContentStyleType.TotalImpactOverB)

        Select Case Me.ContentStyle

            Case eContentStyleType.Keystoneness
                Me.m_zgh.ConfigurePane("", My.Resources.LBL_RELTOTALIMPACT, My.Resources.LBL_KEYSTONENESS, False)

            Case eContentStyleType.TotalImpactOverB
                Me.m_zgh.ConfigurePane("", My.Resources.LBL_RELTOTALIMPACT, My.Resources.LBL_TOTALIMPACT_OVER_B, False)

        End Select

    End Sub

End Class
