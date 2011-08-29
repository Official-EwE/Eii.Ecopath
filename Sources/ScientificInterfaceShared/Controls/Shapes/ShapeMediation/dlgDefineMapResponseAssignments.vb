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
    Private m_orgMin As Single
    Private m_orgMax As Single
    Private m_bHasInit As Boolean
    Private m_map As IEnviroInputMap

#End Region

#Region "Construciton Initialization"

    Public Sub New(ByVal UIC As cUIContext, ByVal ResponseShape As EwECore.cEnviroResponseFunction, ByVal Manager As EwECore.cMapResponseInteractionManager)
        Me.InitializeComponent()

        Me.m_shape = ResponseShape
        Me.m_manager = Manager

        Me.m_uic = UIC

        Me.m_zgh = New cZedGraphEnviroResponseHelper 'cZedGraphHelper
        Me.m_zgh.Attach(Me.m_uic, Me.ZedGraph)
        Me.m_zgh.ShowPointValue = True

    End Sub

    Private Sub dlgDefineMapResponseAssignments_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Try

            'remember the original response function min and max
            Me.m_orgMin = Me.m_shape.XAxisMin
            Me.m_orgMax = Me.m_shape.XAxisMax

            Me.m_zgh.ConfigurePane(My.Resources.RESPONSE_GRAPH_TITLE, My.Resources.RESPONSE_GRAPH_XLABEL, My.Resources.RESPONSE_GRAPH_YLABEL, True)

            If Me.m_shape.XAxisMax = 0 Then
                Me.m_shape.XAxisMax = 1.0 'some kind of bogus default if nothing has been defined
            End If

            Dim fmt As New cCoreInterfaceFormatter()
            Me.lbSeletedFunctionName.Text = String.Format(My.Resources.CAPACITY_SET_SHAPE_MINMAX, fmt.GetDescriptor(Me.m_shape, eDescriptorTypes.Name))

            Me.updateControls()
            Me.LoadGroups()
            Me.loadMaps()
            Me.PlotGraph()

        Catch ex As Exception

        End Try

        Me.m_bHasInit = True

    End Sub

#End Region

#Region "Control Event Handlers"

    ''' <summary>
    ''' Add the selected groups to the currently selected map
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAdd.Click

        Try

            'Dim Map As IEnviroInputMap = Me.getSelMap
            'Is there a selected map
            If Me.m_map Is Nothing Then Return

            'Yes add all the groups 
            For Each item As GroupListItem In Me.lstGroups.SelectedItems
                Me.m_map.ResponseIndexForGroup(item.Index) = Me.m_shape.Index
            Next

            'bluntly reload the map tree
            Me.loadMaps()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub



    Private Sub btRemove_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRemove.Click
        Try

            ' Dim map As IEnviroInputMap = Me.getSelMap
            If Me.m_map Is Nothing Then Return

            Dim node As TreeNode
            node = Me.trvMapTree.SelectedNode
            If node IsNot Nothing Then
                'last node this must be a selected group node
                If node.Nodes.Count = 0 Then
                    'Group index was put in the tag when the tree was populated
                    Dim iGrp As Integer = DirectCast(node.Tag, Integer)
                    Me.m_map.ResponseIndexForGroup(iGrp) = cCore.NULL_VALUE
                    'now remove the node
                    Me.trvMapTree.SelectedNode.Remove()
                End If
            End If

            ' Me.loadMaps()

        Catch ex As Exception

        End Try
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub trvMapTree_AfterExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvMapTree.AfterExpand
        Me.m_map = Me.getSelMap(e.Node)
    End Sub

    Private Sub trvMapTree_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles trvMapTree.Click
        'Me.m_map = Me.getSelMap
    End Sub


    Private Sub trvMapTree_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles trvMapTree.AfterSelect
        Try
            Me.m_map = getSelMap(e.Node)
            Me.PlotGraph()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub OnMinMaxTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txXMax.TextChanged, txXMin.TextChanged

        Try
            Dim txb As TextBox = DirectCast(sender, TextBox)
            'is this really user input
            If Not txb.Focused Then
                'No bump out of here
                Exit Sub
            End If

            Dim maxX As Single = Single.Parse(Me.txXMax.Text)
            Dim minX As Single = Single.Parse(Me.txXMin.Text)
            Me.m_shape.XAxisMin = minX
            Me.m_shape.XAxisMax = maxX

            Me.PlotGraph()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub btDefaultMinMax_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btDefaultMinMax.Click
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
        Try
            Me.txXMax.Text = Me.m_shape.XAxisMax.ToString
            Me.txXMin.Text = Me.m_shape.XAxisMin.ToString
        Catch ex As Exception

        End Try
    End Sub

    Private Sub PlotShape()

        Try
            Dim Xmax As Single = Me.m_shape.XAxisMax
            Dim Xmin As Single = Me.m_shape.XAxisMin
            Dim Xrange As Single = Me.m_shape.XAxisMax - Me.m_shape.XAxisMin

            'if there is a selected map then use that to set the x axis
            'Dim map As IEnviroInputMap = Me.getSelMap()
            If Me.m_map IsNot Nothing Then
                Xmax = Me.m_map.Max '+ map.BinWidth
            End If

            Dim dx As Single = Xrange / Me.m_shape.XMax
            Dim YScale As Single = 1 / Me.m_shape.YMax
            Dim lstPts As New PointPairList

            'First point from shape at the zero X axis
            lstPts.Add(0, Me.m_shape.ShapeData(1) * YScale)
            For ipt As Integer = 1 To Me.m_shape.XMax
                lstPts.Add(Xmin + dx * (ipt - 1), Me.m_shape.ShapeData(ipt) * YScale)
            Next

            'add the last point out at the end of the graph
            lstPts.Add(Xmax, Me.m_shape.ShapeData(Me.m_shape.XMax) * YScale)

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Response", Definitions.eLineType.NotSet, Color.SandyBrown, lstPts)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Me.m_zgh.XScaleMax = Xmax
            Me.m_zgh.YScaleMax = 1.2
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception

        End Try

    End Sub


    Private Sub LoadGroups()
        Dim core As cCore = Me.m_uic.Core
        For igrp As Integer = 1 To core.nGroups
            Me.lstGroups.Items.Add(New GroupListItem(core.EcoPathGroupInputs(igrp)))
        Next
    End Sub

    Private Sub loadMaps()
        Try

            Me.trvMapTree.Nodes.Clear()
            Dim shapeLabel As String = "Groups using '" & Me.m_shape.Name & "'"
            Dim map As IEnviroInputMap
            For imap As Integer = 1 To Me.m_manager.nMaps
                map = Me.m_manager.Map(imap)
                Dim ndApply As TreeNode
                Dim ndGrps As TreeNode
                ndApply = Me.trvMapTree.Nodes.Add(map.Name)
                'add the Map to the node tag
                ndApply.Tag = map
                ndGrps = ndApply.Nodes.Add(shapeLabel)

                For igrp As Integer = 1 To Me.m_uic.Core.nGroups
                    'Is the current shape selected as the response function for any group
                    If Me.m_shape.Index = map.ResponseIndexForGroup(igrp) Then
                        'Yes this shape is set for this group
                        'add a group node
                        Dim ndgrp As TreeNode = ndGrps.Nodes.Add(Me.m_uic.Core.EcoPathGroupInputs(igrp).Name)
                        ndgrp.Tag = igrp
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
        ' Dim map As IEnviroInputMap = Me.getSelMap
        If Me.m_map Is Nothing Then
            'some kind of a warning
            Exit Sub
        End If

        Me.m_shape.XAxisMax = Me.m_map.Max
        Me.m_shape.XAxisMin = Me.m_map.Min

        Me.updateControls()

        Me.PlotGraph()

    End Sub

    Private Function getSelMap(ByVal node As TreeNode) As IEnviroInputMap
        Try

            Dim ob As Object
            'Dim node As TreeNode
            'node = Me.trvMapTree.SelectedNode

            'No node has been selected just return nothing
            If node Is Nothing Then Return Nothing

            Do While node.Parent IsNot Nothing
                node = node.Parent
            Loop
            ob = node.Tag

            If ob IsNot Nothing Then
                If TypeOf ob Is IEnviroInputMap Then
                    System.Console.WriteLine("Seleted map " & DirectCast(ob, IEnviroInputMap).Name)
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
                Exit Sub
            End If

            Dim histPts() As Drawing.PointF = Me.m_map.Histogram()
            Dim lstPts As New PointPairList
            'The X value in the histogram is the max value, right hand side, in the bin
            'So an input value of 1.0 will be in the .X = 1.0 bin
            lstPts.Add(0, histPts(1).Y)
            For ipt As Integer = 1 To histPts.Length - 2
                lstPts.Add(histPts(ipt).X, histPts(ipt).Y)
                lstPts.Add(histPts(ipt).X, histPts(ipt + 1).Y)
            Next

            lstPts.Add(histPts(histPts.Length - 1).X, histPts(histPts.Length - 1).Y)

            Dim il As LineItem = Me.m_zgh.CreateLineItem("Histogram", Definitions.eLineType.NotSet, Color.RoyalBlue, lstPts)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            Me.m_zgh.XScaleMax = Me.m_map.Max
            Me.m_zgh.YScaleMax = 1.2
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Helper class Ecopath Group ListBox Item used of group name and index"

    Private Class GroupListItem
        Public Group As cEcoPathGroupInput
        Public Sub New(ByVal theGroup As cEcoPathGroupInput)
            Group = theGroup
        End Sub

        Public Overrides Function ToString() As String
            Return Group.Name
        End Function

        Public ReadOnly Property Index() As Integer
            Get
                Return Group.Index
            End Get
        End Property
    End Class

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
        'This is not a very good way to do this 
        'It may be better to not use a tool tip at all 
        'instead pass out the X and Y Axis value(s) and let the container figure out how to show the data
        Try

            'WARNING this only works if the curve is labeled "Response"
            Dim bUseBase As Boolean = True
            If curve.Tag IsNot Nothing Then
                If TypeOf curve.Tag Is cCurveInfo Then
                    Dim ci As cCurveInfo = DirectCast(curve.Tag, cCurveInfo)
                    If String.Compare(ci.Label, "Response") = 0 Then
                        'format the tooltip here
                        bUseBase = False
                    ElseIf String.Compare(ci.Label, "Histogram") = 0 Then
                        'this is the Histogram Curve
                        'so don't show anything
                        Return ""
                    End If '  If String.Compare(ci.Label, "Response") = 0 Then
                End If ' If TypeOf curve.Tag Is cCurveInfo Then
            End If ' If curve.Tag IsNot Nothing Then

            If bUseBase Then
                Return MyBase.FormatTooltip(pane, curve, iPoint)
            End If

            Debug.Assert(curve.IsLine, "ToolTip wrong line type.")

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




