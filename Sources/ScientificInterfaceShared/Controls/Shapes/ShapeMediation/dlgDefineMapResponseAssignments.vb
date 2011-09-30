#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ZedGraph
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region

Public Class dlgDefineMapResponseAssignments

#Region "Private variables"

    Private m_shape As EwECore.cEnviroResponseFunction
    Private m_manager As cMapResponseInteractionManager
    Private m_zgh As cZedGraphEnviroResponseHelper 'cZedGraphHelper
    Private m_uic As cUIContext
    Private m_bHasInit As Boolean
    Private m_map As IEnviroInputMap
    Private m_fpXMin As cEwEFormatProvider = Nothing
    Private m_fpXMax As cEwEFormatProvider = Nothing

    Public Enum eLines As Integer
        Response
        Histogram
    End Enum

#End Region

#Region "Construction Initialization"

    Public Sub New(ByVal UIC As cUIContext, ByVal ResponseShape As EwECore.cEnviroResponseFunction, ByVal Manager As EwECore.cMapResponseInteractionManager)
        Me.InitializeComponent()

        Me.m_shape = ResponseShape
        Me.m_manager = Manager

        Me.m_uic = UIC

        Me.m_zgh = New cZedGraphEnviroResponseHelper 'cZedGraphHelper
        Me.m_zgh.Attach(Me.m_uic, Me.m_graph)
        Me.m_zgh.ShowPointValue = True

        Try
            Me.Text = String.Format(Me.Text, New cShapeDataFormatter().GetDescriptor(Me.m_shape))
        Catch ex As Exception
            ' Whoah!
        End Try

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_uic Is Nothing) Then Return

        Try

            'remember the original response function min and max
            Me.m_fpXMax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMax, GetType(Single))
            Me.m_fpXMin = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMin, GetType(Single))
            Me.m_fpXMin.Value = Me.m_shape.XAxisMin
            Me.m_fpXMax.Value = Me.m_shape.XAxisMax

            If (CSng(Me.m_fpXMax.Value) = 0) Then
                Me.m_fpXMax.Value = 1.0 'some kind of bogus default if nothing has been defined
            End If

            AddHandler Me.m_fpXMin.OnValueChanged, AddressOf OnMinMaxTextChanged
            AddHandler Me.m_fpXMax.OnValueChanged, AddressOf OnMinMaxTextChanged

            Me.m_zgh.ConfigurePane(My.Resources.RESPONSE_GRAPH_TITLE, My.Resources.RESPONSE_GRAPH_XLABEL, My.Resources.RESPONSE_GRAPH_YLABEL, True)

            Me.m_zgh.GetPane(1).Y2Axis.IsVisible = True
            Me.m_zgh.GetPane(1).Y2Axis.Title.Text = "Map histogram"
            Me.m_zgh.GetPane(1).Y2Axis.Title.IsVisible = True
            Me.m_zgh.GetPane(1).Y2Axis.Title.FontSpec = Me.m_zgh.GetPane(1).YAxis.Title.FontSpec

            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsAllTics = False
            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsOpposite = False
            Me.m_zgh.GetPane(1).Y2Axis.MajorTic.IsOpposite = False
            'somehow set the Y2Axis label font size

            Me.m_zgh.GetPane(1).Y2Axis.Scale.MaxAuto = True

            Me.m_lbxGroups.Attach(Me.m_uic)

            Me.loadMaps()
            Me.PlotGraph()

            Me.m_bHasInit = True
            Me.updateControls()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnLoad() Exception: " & ex.Message)
            cLog.Write(ex)
            Throw New Exception(ex.Message)
        End Try

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.m_uic Is Nothing) Then Return

        Me.m_lbxGroups.Detach()

        RemoveHandler Me.m_fpXMin.OnValueChanged, AddressOf OnMinMaxTextChanged
        RemoveHandler Me.m_fpXMax.OnValueChanged, AddressOf OnMinMaxTextChanged

        Me.m_fpXMax.Release()
        Me.m_fpXMax = Nothing
        Me.m_fpXMin.Release()
        Me.m_fpXMin = Nothing

        MyBase.OnFormClosed(e)

    End Sub

#End Region

#Region "Control Event Handlers"

    Private Sub OnGroupSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_lbxGroups.SelectedValueChanged
        Try
            Me.updateControls()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Add the selected groups to the currently selected map
    ''' </summary>
    Private Sub OnAddGroup(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnAdd.Click

        Try

            'Dim Map As IEnviroInputMap = Me.getSelMap
            'Is there a selected map
            If Me.m_map Is Nothing Then Return

            'Yes add all the groups 
            For Each i As Integer In Me.m_lbxGroups.SelectedIndices
                Me.m_map.ResponseIndexForGroup(Me.m_lbxGroups.GetGroupIndexAt(i)) = Me.m_shape.Index
            Next

            'bluntly reload the map tree
            Me.loadMaps()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub OnRemove(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click
        Try

            ' Dim map As IEnviroInputMap = Me.getSelMap
            If Me.m_map Is Nothing Then Return

            Dim node As TreeNode
            node = Me.m_tvMaps.SelectedNode
            If (node IsNot Nothing) Then
                ' Is group node?
                If (TypeOf (node.Tag) Is cCoreGroupBase) Then
                    ' #Yes: group was put in the tag when the tree was populated
                    Dim grp As cCoreGroupBase = DirectCast(node.Tag, cCoreGroupBase)
                    Me.m_map.ResponseIndexForGroup(grp.Index) = cCore.NULL_VALUE
                    node.Remove()
                Else
                    Dim lGroupNodes As New List(Of TreeNode)
                    For Each ndChild As TreeNode In node.Nodes
                        lGroupNodes.Add(ndChild)
                    Next
                    For Each ndChild As TreeNode In lGroupNodes
                        Dim grp As cCoreGroupBase = DirectCast(ndChild.Tag, cCoreGroupBase)
                        Me.m_map.ResponseIndexForGroup(grp.Index) = cCore.NULL_VALUE
                        ndChild.Remove()
                    Next
                End If
            End If

            ' Me.loadMaps()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOk.Click

        ' Apply changes
        Me.m_shape.XAxisMin = CSng(Me.m_fpXMin.Value)
        Me.m_shape.XAxisMax = CSng(Me.m_fpXMax.Value)

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub trvMapTree_AfterExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvMaps.AfterExpand
        Me.m_map = Me.GetSelectedMap(e.Node)
    End Sub

    Private Sub trvMapTree_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvMaps.AfterSelect
        Try
            Me.m_map = GetSelectedMap(e.Node)
            Me.updateControls()
            Me.PlotGraph()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnMinMaxTextChanged(ByVal sender As cEwEFormatProvider)

        ' Format providers changed: update the map
        Me.PlotGraph()

    End Sub

    Private Sub OnSetDefaultMinMax(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnDefaultMinMax.Click
        Me.setDefaultMinMax()
    End Sub

#End Region

#Region "Private Methods"

    Private Sub PlotGraph()

        Try
            'Always clear out the old data????
            'Maybe not!!!
            Me.m_zgh.GetPane(1).CurveList.Clear()

            Me.PlotShape()
            Me.PlotMap()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub updateControls()

        Dim bCanAddGroup As Boolean = (Me.m_lbxGroups.SelectedItems.Count > 0)
        Dim bCanRemoveGroup As Boolean = (Me.m_tvMaps.SelectedNode IsNot Nothing)

        Me.m_btnAdd.Enabled = bCanAddGroup
        Me.m_btnRemove.Enabled = bCanRemoveGroup

    End Sub

    Private Sub PlotShape()

        Try
            Dim Xmax As Single = CSng(Me.m_fpXMax.Value)
            Dim Xmin As Single = CSng(Me.m_fpXMin.Value)
            Dim Xrange As Single = Xmax - Xmin
            Dim fmt As New cCoreInterfaceFormatter()

            'if there is a selected map then use that to set the x axis
            'Dim map As IEnviroInputMap = Me.getSelMap()
            If (Me.m_map IsNot Nothing) Then
                Xmax = Me.m_map.Max '+ map.BinWidth
            End If

            Dim dx As Single = Xrange / Me.m_shape.XMax
            Dim YScale As Single = 1 '/ Me.m_shape.YMax
            Dim lstPts As New PointPairList

            'First point from shape at the zero X axis
            lstPts.Add(0, Me.m_shape.ShapeData(1) * YScale)
            For ipt As Integer = 1 To Me.m_shape.XMax
                lstPts.Add(Xmin + dx * (ipt - 1), Me.m_shape.ShapeData(ipt) * YScale)
            Next

            'add the last point out at the end of the graph
            lstPts.Add(Xmax, Me.m_shape.ShapeData(Me.m_shape.XMax) * YScale)

            Dim il As LineItem = Me.m_zgh.CreateLineItem(String.Format(My.Resources.HEADER_RESPONSE_TARGET, fmt.GetDescriptor(Me.m_shape)), _
                                                         Definitions.eLineType.NotSet, Color.SandyBrown, lstPts, eLines.Response)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Me.m_zgh.XScaleMax = Xmax
            Me.m_zgh.YScaleMax = Me.m_shape.YMax + Me.m_shape.YMax * 0.1
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception

        End Try

    End Sub

    Private Sub loadMaps()

        Dim map As IEnviroInputMap = Nothing
        Dim fmt As New cCoreInterfaceFormatter()

        Try
            Me.m_tvMaps.Nodes.Clear()

            For imap As Integer = 1 To Me.m_manager.nMaps

                map = Me.m_manager.Map(imap)
                Dim ndApply As TreeNode = Me.m_tvMaps.Nodes.Add(fmt.GetDescriptor(map))
                ndApply.Tag = map

                For igrp As Integer = 1 To Me.m_uic.Core.nGroups
                    'Is the current shape selected as the response function for any group
                    If Me.m_shape.Index = map.ResponseIndexForGroup(igrp) Then
                        'Yes this shape is set for this group
                        'add a group node
                        Dim grp As cCoreGroupBase = Me.m_uic.Core.EcoPathGroupInputs(igrp)
                        Dim ndgrp As TreeNode = ndApply.Nodes.Add(fmt.GetDescriptor(grp))
                        ndgrp.Tag = grp

                        If Not ndApply.IsExpanded Then
                            'if there are groups assigned to this Map/Node then expand it the tree to this point
                            ndApply.ExpandAll()
                        End If
                    End If
                Next
            Next

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".loadMaps() Exception: " & ex.Message)
        End Try

    End Sub


    Private Sub setDefaultMinMax()

        If (Me.m_map Is Nothing) Then
            'some kind of a warning
            Exit Sub
        End If

        Me.m_fpXMax.Value = Me.m_map.Max
        Me.m_fpXMin.Value = Me.m_map.Min

        Me.updateControls()

        Me.PlotGraph()

    End Sub

    Private Function GetSelectedMap(ByVal node As TreeNode) As IEnviroInputMap
        Try

            Dim ob As Object

            'No node has been selected just return nothing
            If node Is Nothing Then Return Nothing

            Do While node.Parent IsNot Nothing
                node = node.Parent
            Loop
            ob = node.Tag

            If ob IsNot Nothing Then
                If TypeOf ob Is IEnviroInputMap Then
                    System.Console.WriteLine("Selected map " & DirectCast(ob, IEnviroInputMap).Name)
                    Return DirectCast(ob, IEnviroInputMap)
                End If
            End If

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    'Private Function getSelMap() As IEnviroInputMap
    '    Try

    '        '  Dim ob As Object
    '        Dim node As TreeNode
    '        node = Me.trvMapTree.SelectedNode

    '        Me.getSelMap(node)

    '        ''No node has been selected just return nothing
    '        'If node Is Nothing Then Return Nothing

    '        'Do While node.Parent IsNot Nothing
    '        '    node = node.Parent
    '        'Loop
    '        'ob = node.Tag

    '        'If ob IsNot Nothing Then
    '        '    If TypeOf ob Is IEnviroInputMap Then
    '        '        System.Console.WriteLine("Seleted map " & DirectCast(ob, IEnviroInputMap).Name)
    '        '        Return DirectCast(ob, IEnviroInputMap)
    '        '    End If
    '        'End If

    '    Catch ex As Exception

    '    End Try

    '    Return Nothing

    'End Function

    Private Sub PlotMap()
        Try
            'Dim map As IEnviroInputMap = Me.getSelMap
            If Me.m_map Is Nothing Then
                'no map to plot
                Return
            End If

            Dim y2max As Single
            Dim histPts() As Drawing.PointF = Me.m_map.Histogram()
            Dim binWidth As Single = Me.m_map.HistogramBinWidth
            Dim lstPts As New PointPairList()
            Dim fmt As New cCoreInterfaceFormatter()

            'The X value in the histogram is the max value of the bin, right hand side of the bin
            'So an input value of 1.0 will be in the .X = 1.0 bin
            For ipt As Integer = 1 To histPts.Length - 1
                lstPts.Add(histPts(ipt).X - binWidth, histPts(ipt).Y)
                lstPts.Add(histPts(ipt).X, histPts(ipt).Y)
                y2max = Math.Max(histPts(ipt).Y, y2max)
            Next

            Dim il As LineItem = Me.m_zgh.CreateLineItem(String.Format(My.Resources.HEADER_HISTOGRAM_TARGET, fmt.GetDescriptor(Me.m_map)), _
                                                         Definitions.eLineType.NotSet, Color.RoyalBlue, lstPts, eLines.Histogram)

            il.IsY2Axis = True
            il.Line.Fill = New Fill(System.Drawing.Color.Gray)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Me.m_zgh.XScaleMax = Me.m_map.Max
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception

        End Try

    End Sub

#End Region

End Class

#Region "ZedGraph helper for Response tool tips"


'ToDo_jb cZedGraphEnviroResponseHelper is used by both dlgDefineMapResponseAssignments and ucMediationAssignments
'it should be located in it's own file some place in the SI Shared...


''' <summary>
''' Derived Zedgraph helper class that just overrides the ToolTip formating for the EnvironmentalResponse graphs
''' </summary>
''' <remarks></remarks>
<CLSCompliant(False)> _
Public Class cZedGraphEnviroResponseHelper
    Inherits cZedGraphHelper

    Protected Overrides Function FormatTooltip(ByVal pane As ZedGraph.GraphPane, ByVal curve As ZedGraph.CurveItem, ByVal iPoint As Integer) As String

        ' ToDo: localize this

        'This is not a very good way to do this 
        'It may be better to not use a tool tip at all 
        'instead pass out the X and Y Axis value(s) and let the container figure out how to show the data
        Try

            'WARNING this only works if the curve is labeled "Response"
            Dim bUseBase As Boolean = True

            If curve.Tag IsNot Nothing Then
                If TypeOf curve.Tag Is cCurveInfo Then
                    Dim ci As cCurveInfo = DirectCast(curve.Tag, cCurveInfo)
                    Dim tag As dlgDefineMapResponseAssignments.eLines = DirectCast(ci.Tag, dlgDefineMapResponseAssignments.eLines)

                    Select Case tag
                        Case dlgDefineMapResponseAssignments.eLines.Response
                            bUseBase = False
                        Case dlgDefineMapResponseAssignments.eLines.Histogram
                            Return ""
                        Case Else
                            Debug.Assert(False, "Unsupported line type")
                    End Select
                End If ' If TypeOf curve.Tag Is cCurveInfo Then
            End If ' If curve.Tag IsNot Nothing Then

            If bUseBase Then
                Return MyBase.FormatTooltip(pane, curve, iPoint)
            End If

            Debug.Assert(curve.IsLine, "ToolTip wrong line type.")

            ' ToDo: localize this
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("Capacity for Map input.")

            Dim pp As PointPair = curve(iPoint)
            sb.AppendLine("Map input " & Me.StyleGuide.FormatNumber(pp.X))
            sb.AppendLine("Capacity " & Me.StyleGuide.FormatNumber(pp.Y))
            Return sb.ToString
        Catch ex As Exception

        End Try
        Return ""

    End Function

End Class

#End Region




