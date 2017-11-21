' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports ZedGraph
Imports ScientificInterfaceShared.Commands
Imports EwEUtils.Utilities

#End Region ' Imports

''' <summary>
''' Main interface to define the functional responses of groups to environmental drivers.
''' </summary>
Public NotInheritable Class dlgDefineEcospaceForagingResponse

    'ToDo  update graph interface from edit dialog 

#Region " Private variables "

    Protected m_uic As cUIContext = Nothing
    Protected m_shape As EwECore.cEnviroResponseFunction = Nothing
    Protected m_shapefunction As IShapeFunction = Nothing
    Protected m_manager As IEnvironmentalResponseManager = Nothing

    Private m_zgh As cZedGraphMediationHelper = Nothing
    Private m_map As IEnviroInputData = Nothing
    Private m_fpMin As cEwEFormatProvider = Nothing
    Private m_fpMax As cEwEFormatProvider = Nothing
    Private m_fpMean As cEwEFormatProvider = Nothing

    Private m_bInUpdate As Boolean = False

#End Region ' Private variables

#Region " Construction Initialization "

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="uic"></param>
    ''' <param name="shape"></param>
    ''' <param name="manager"></param>
    ''' <remarks></remarks>
    Public Sub New(ByVal uic As cUIContext,
                   ByVal shape As EwECore.cEnviroResponseFunction,
                   ByVal manager As EwECore.IEnvironmentalResponseManager)
        Me.InitializeComponent()

        Me.m_shape = shape
        Me.m_shapefunction = cShapeFunctionFactory.GetShapeFunction(shape)
        Me.m_manager = manager

        Me.m_uic = uic

        Me.m_zgh = New cZedGraphMediationHelper()
        Me.m_zgh.Attach(Me.m_uic, Me.m_graph)
        Me.m_zgh.ShowPointValue = True

        Debug.Print("Load dialogue " + Me.m_shape.ToCSVString())

        Try
            Me.Text = cStringUtils.Localize(Me.Text, New cShapeDataFormatter().GetDescriptor(Me.m_shape))
        Catch ex As Exception
            ' Whoah!
        End Try

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.m_uic Is Nothing) Then Return

        Try
            Me.m_bInUpdate = True

            Me.m_zgh.ConfigurePane(My.Resources.RESPONSE_GRAPH_TITLE, My.Resources.RESPONSE_GRAPH_XLABEL, My.Resources.RESPONSE_GRAPH_YLABEL, True)

            'Yaxis (left) grid lines
            Me.m_zgh.GetPane(1).YAxis.MajorGrid.IsVisible = True

            ' JB: the cool thing to do here would be to only show the 1.0 grid line;not all the grid line....
            ' JS: This should help
            Me.m_zgh.GetPane(1).YAxis.MajorTic.IsAllTics = False

            Me.m_zgh.GetPane(1).Y2Axis.IsVisible = True

            Me.m_zgh.GetPane(1).Y2Axis.Title.Text = My.Resources.HEADER_MAP_HISTOGRAM
            Me.m_zgh.GetPane(1).Y2Axis.Title.IsVisible = True
            Me.m_zgh.GetPane(1).Y2Axis.Title.FontSpec = Me.m_zgh.GetPane(1).YAxis.Title.FontSpec

            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsAllTics = False
            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsOpposite = False
            Me.m_zgh.GetPane(1).Y2Axis.MajorTic.IsOpposite = False

            'somehow set the Y2Axis label font size
            Me.m_zgh.GetPane(1).Y2Axis.Scale.MaxAuto = True

            Me.m_lbxGroups.Attach(Me.m_uic)
            Me.m_lbxGroups.Populate(Me.GetGroupList())

            Me.m_fpMin = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMin, GetType(Single))
            Me.m_fpMax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMax, GetType(Single))
            Me.m_fpMean = New cEwEFormatProvider(Me.m_uic, Me.m_tbxMean, GetType(Single))

            ' Set min and max
            Me.m_fpMin.Value = Me.m_shape.ResponseLeftLimit
            Me.m_fpMax.Value = Me.m_shape.ResponseRightLimit

        Catch ex As Exception

        End Try

        Me.m_bInUpdate = False
        Me.InitToShapeType()

    End Sub

    Protected Function GetGroupList() As Integer()
        Dim lstGroups As New List(Of Integer)
        For iGrp As Integer = 1 To Me.m_uic.Core.nGroups
            Dim grp As cEcospaceGroupInput = Me.m_uic.Core.EcospaceGroupInputs(iGrp)
            If ((grp.CapacityCalculationType And eEcospaceCapacityCalType.EnvResponses) = eEcospaceCapacityCalType.EnvResponses) Then
                lstGroups.Add(iGrp)
            End If
        Next
        Return lstGroups.ToArray()
    End Function

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.m_uic Is Nothing) Then Return

        Me.m_lbxGroups.Detach()

        ' Clear out shape to de-init UI
        Me.m_shape = Nothing
        Me.InitToShapeType()

        Me.m_fpMin.Release()
        Me.m_fpMax.Release()
        Me.m_fpMean.Release()

        MyBase.OnFormClosed(e)

    End Sub

    Protected Sub InitToShapeType()

        Me.m_shapefunction = cShapeFunctionFactory.GetShapeFunction(Me.m_shape)

        If (Me.ShowMinMax) Then
            RemoveHandler Me.m_fpMin.OnValueChanged, AddressOf OnMinMaxValueChanged
            RemoveHandler Me.m_fpMax.OnValueChanged, AddressOf OnMinMaxValueChanged
        End If

        If (Me.CanEditMean) Then
            RemoveHandler Me.m_fpMean.OnValueChanged, AddressOf OnMeanValueChanged
        End If

        If (Me.m_shape Is Nothing) Then Return

        If (Me.ShowMinMax) Then
            AddHandler Me.m_fpMin.OnValueChanged, AddressOf OnMinMaxValueChanged
            AddHandler Me.m_fpMax.OnValueChanged, AddressOf OnMinMaxValueChanged
        End If

        If (Me.CanEditMean) Then

            Dim normdist As cNormalShapeFunction = DirectCast(Me.m_shapefunction, cNormalShapeFunction)
            Me.m_fpMean.Value = normdist.Mean

            AddHandler Me.m_fpMean.OnValueChanged, AddressOf OnMeanValueChanged
        End If

        Me.LoadDrivers()
        Me.UpdatePlots()
        Me.UpdateControls()

    End Sub

#End Region ' Construction Initialization

#Region " Control Event Handlers "

    Private Sub OnGroupSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_lbxGroups.SelectedValueChanged
        Try
            Me.UpdateControls()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Add the selected groups to the currently selected map
    ''' </summary>
    Private Sub OnAddGroup(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnAdd.Click

        Try
            ' Abort if no selected map
            If Me.m_map Is Nothing Then Return

            'Yes add all the groups 
            For Each i As Integer In Me.m_lbxGroups.SelectedIndices
                Me.m_map.ResponseIndexForGroup(Me.m_lbxGroups.GetGroupIndexAt(i)) = Me.m_shape.Index
            Next

            'bluntly reload the map tree
            Me.LoadDrivers()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub OnRemoveGroup(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click

        Try
            If (Me.m_map Is Nothing) Then Return

            Dim node As TreeNode
            node = Me.m_tvDrivers.SelectedNode
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

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnOk.Click

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnMapTreeExpanded(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvDrivers.AfterExpand

        Try
            Me.m_map = Me.GetSelectedMap(e.Node)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnMapTreeSelected(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvDrivers.AfterSelect
        Try
            Me.m_map = GetSelectedMap(e.Node)
            Me.UpdateControls()
            Me.UpdatePlots()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnMinMaxValueChanged(ByVal sender As Object, args As EventArgs)
        Me.ApplyMinMax()
    End Sub

    Private Sub OnSetDefaultMinMax(ByVal sender As Object, ByVal e As EventArgs) _
        Handles m_btnDefaultMinMax.Click
        Me.SetDefaultMinMax()
    End Sub

    Private Sub OnMeanValueChanged(sender As System.Object, e As System.EventArgs)
        Try
            If Me.m_bInUpdate Then Return

            Debug.Assert(Me.CanEditMean(), "Oppss BUG! should not be setting the Mean for this type of shape.")
            'Mean is stored in the Steep variable

            Dim normdist As cNormalShapeFunction = DirectCast(Me.m_shapefunction, cNormalShapeFunction)
            normdist.Mean = CSng(Me.m_fpMean.Value)
            normdist.Apply(Me.m_shape)

            Me.UpdatePlots()
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnChangeShape(sender As System.Object, e As System.EventArgs) _
        Handles m_btChangeShape.Click
        Try
            Me.ChangeFFShape()
            ' Type of shape may have changed
            Me.InitToShapeType()
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Control Event Handlers

#Region " Private Methods "

    Private Sub UpdatePlots()

        Try
            'Always clear out the old data????
            'Maybe not!!!
            Me.m_zgh.GetPane(1).CurveList.Clear()

            Me.PlotShape()
            Me.PlotMap()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub UpdateControls()

        ' ToDo JS: this must be connected to IShapeFunction behaviour

        Dim bCanAddGroup As Boolean = (Me.m_lbxGroups.SelectedItems.Count > 0)
        Dim bCanRemoveGroup As Boolean = (Me.m_tvDrivers.SelectedNode IsNot Nothing)
        Dim bCanSetMinMax As Boolean = Me.ShowMinMax() Or True
        Dim bCanSetMeanSD As Boolean = Me.CanEditMean()

        Dim strXMin As String = My.Resources.HEADER_X_MIN
        Dim strXMax As String = My.Resources.HEADER_X_MAX
        Dim strMean As String = My.Resources.HEADER_MEAN
        Dim strSD As String = My.Resources.HEADER_STANDARDDEVIATION

        Select Case Me.m_shape.ShapeFunctionType

            Case eShapeFunctionType.Normal
                strXMin = My.Resources.HEADER_PLOT_MIN
                strXMax = My.Resources.HEADER_PLOT_MAX

                'Me.m_tbxMean.Text = Me.m_shape.Steep.ToString
                'Me.m_tbxSD.Text = Me.m_SD.ToString

            Case eShapeFunctionType.LeftShoulder,
                 eShapeFunctionType.RightShoulder,
                 eShapeFunctionType.Trapezoid
                strXMin = My.Resources.HEADER_PLOT_MIN
                strXMax = My.Resources.HEADER_PLOT_MAX

            Case Else
                ' NOP
        End Select

        Me.m_btnAdd.Enabled = bCanAddGroup
        Me.m_btnRemove.Enabled = bCanRemoveGroup

        Me.m_lblXMin.Text = cStyleGuide.ToControlLabel(strXMin)
        Me.m_lblXMax.Text = cStyleGuide.ToControlLabel(strXMax)
        Me.m_fpMin.Enabled = bCanSetMinMax
        Me.m_fpMax.Enabled = bCanSetMinMax

        Me.m_lblMean.Text = cStyleGuide.ToControlLabel(strMean)
        '    Me.m_lblSD.Text = cStyleGuide.ToControlLabel(strSD)
        Me.m_fpMean.Enabled = bCanSetMeanSD
        '  Me.m_fpSD.Enabled = bCanSetMeanSD

    End Sub


    Private Function ShowMinMax() As Boolean

        Return (Me.m_shape IsNot Nothing)

    End Function

    Private Function CanEditMinMax() As Boolean

        If (Me.m_shape Is Nothing) Then Return False

        If ((Me.m_shape.ShapeFunctionType = eShapeFunctionType.Normal) Or
            (Me.m_shape.ShapeFunctionType = eShapeFunctionType.LeftShoulder) Or
            (Me.m_shape.ShapeFunctionType = eShapeFunctionType.RightShoulder) Or
            (Me.m_shape.ShapeFunctionType = eShapeFunctionType.Trapezoid) Or
            (Me.m_shape.ShapeFunctionType = eShapeFunctionType.Sigmoid)) Then
            Return False
        End If

        Return True

    End Function

    Private Function CanEditMean() As Boolean

        If (Me.m_shape Is Nothing) Then Return False
        If (Me.m_shapefunction Is Nothing) Then Return False

        Return (TypeOf Me.m_shapefunction Is cNormalShapeFunction)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Launch EwE 'change shape' interface.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub ChangeFFShape()
        Try
            Dim cmd As cCommand = Me.m_uic.CommandHandler.GetCommand("ChangeEcosimShape")
            cmd.Tag = Me.m_shape
            cmd.Invoke()
            cmd.Tag = Nothing
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' ToDo: document this
    ''' </summary>
    ''' <param name="sShapeMin"></param>
    ''' <param name="sShapeMax"></param>
    ''' <param name="sPlotMin"></param>
    ''' <param name="sPlotMax"></param>
    ''' -----------------------------------------------------------------------
    Private Sub GetPlotMinMax(ByRef sShapeMin As Single, ByRef sShapeMax As Single,
                              ByRef sPlotMin As Single, ByRef sPlotMax As Single)

        Select Case Me.m_shape.ShapeFunctionType

            Case eShapeFunctionType.Normal
                'Normal distribution shape min and max are set from the Mean and SD values

                'Use the Min Max on the interface to set the plot window size
                sPlotMin = CSng(Me.m_fpMin.Value)
                sPlotMax = CSng(Me.m_fpMax.Value)

            Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder, eShapeFunctionType.Trapezoid, eShapeFunctionType.Sigmoid
                'Shoulder shape min and max can not be set here
                'They only get set from the ChangeShape dialogue

                'Min and Max of the plot window NOT the data
                sPlotMin = CSng(Me.m_fpMin.Value)
                sPlotMax = CSng(Me.m_fpMax.Value)

                'The min and max of the data cannot be changed here
                sShapeMin = Me.m_shape.ResponseLeftLimit
                sShapeMax = Me.m_shape.ResponseRightLimit

            Case Else
                'For all other shape the Min Max get set for the Min and Max textbox on this form
                sShapeMin = CSng(Me.m_tbxXMin.Text)
                sShapeMax = CSng(Me.m_tbxXMax.Text)
                sPlotMin = sShapeMin
                sPlotMax = sShapeMax

        End Select

    End Sub

    Private Sub PlotShape()

        Try
            ' Obtain Min and Max from the response function
            ' this is what the core will use to find the x value
            Dim XDataMin As Single = Me.m_shape.ResponseLeftLimit
            Dim XDataMax As Single = Me.m_shape.ResponseRightLimit

            Dim XWinMax As Single
            Dim XWinMin As Single

            Me.GetPlotMinMax(XDataMin, XDataMax, XWinMin, XWinMax)

            Dim Xrange As Single = XDataMax - XDataMin
            Dim fmt As New cCoreInterfaceFormatter()

            Dim dx As Single = Xrange / Me.m_shape.nPoints

            Dim YScale As Single = 1
            Dim lstPts As New PointPairList

            Dim x As Double
            For ipt As Integer = 1 To Me.m_shape.nPoints
                x = XDataMin + dx * (ipt - 1)
                lstPts.Add(x, Me.m_shape.ShapeData(ipt) * YScale)
            Next

            'add the last point out at the end of the graph
            lstPts.Add(XDataMax, Me.m_shape.ShapeData(Me.m_shape.nPoints) * YScale)

            Dim il As LineItem = Me.m_zgh.CreateLineItem(cStringUtils.Localize(My.Resources.HEADER_RESPONSE_TARGET, fmt.GetDescriptor(Me.m_shape)),
                                                         lstPts, cZedGraphMediationHelper.eEnvResponseLineType.Response)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            'X axis for plotting
            Me.m_zgh.XScaleMin = XWinMin
            Me.m_zgh.XScaleMax = XWinMax
            Me.m_zgh.YScaleMax = Me.m_shape.YMax + Me.m_shape.YMax * 0.1
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Protected Sub LoadDrivers()

        Dim map As IEnviroInputData = Nothing
        Dim fmt As New cCoreInterfaceFormatter()

        Try
            Me.m_tvDrivers.Nodes.Clear()

            For imap As Integer = 1 To Me.m_manager.nEnviroData

                map = Me.m_manager.EnviroData(imap)
                Dim ndApply As TreeNode = Me.m_tvDrivers.Nodes.Add(map.Name)
                ndApply.Tag = map

                For igrp As Integer = 1 To Me.m_uic.Core.nGroups
                    'Is the current shape selected as the response function for any group
                    If Me.m_shape.Index = map.ResponseIndexForGroup(igrp) Then
                        'Yes this shape is set for this group
                        'add a group node
                        Dim grp As cEcospaceGroupInput = Me.m_uic.Core.EcospaceGroupInputs(igrp)
                        If ((grp.CapacityCalculationType And eEcospaceCapacityCalType.EnvResponses) = eEcospaceCapacityCalType.EnvResponses) Then

                            Dim ndgrp As TreeNode = ndApply.Nodes.Add(fmt.GetDescriptor(grp))
                            ndgrp.Tag = grp

                            If Not ndApply.IsExpanded Then
                                'if there are groups assigned to this Map/Node then expand it the tree to this point
                                ndApply.ExpandAll()
                            End If
                        End If
                    End If
                Next
            Next

        Catch ex As Exception
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".LoadDrivers() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub SetDefaultMinMax()

        If (Me.m_map Is Nothing) Then Return

        Me.m_bInUpdate = True

        Me.m_fpMin.Value = Me.m_map.Min
        Me.m_fpMax.Value = Me.m_map.Max

        Me.m_bInUpdate = False
        Me.ApplyMinMax()

    End Sub

    Private Sub ApplyMinMax()
        If Me.m_bInUpdate Then Return

        Debug.Assert(Me.ShowMinMax())

        'Not all shapes use the Min and Mix data range
        If Me.CanEditMinMax() Then
            Try
                Me.m_shape.LockUpdates()
                Me.m_shape.ResponseLeftLimit = CSng(Me.m_fpMin.Value)
                Me.m_shape.ResponseRightLimit = CSng(Me.m_fpMax.Value)
                Me.m_shape.UnlockUpdates(True)
            Catch ex As Exception

            End Try
        End If ' If Me.CanEditMinMax() Then

        Me.UpdatePlots()

    End Sub

    Private Function GetSelectedMap(ByVal node As TreeNode) As IEnviroInputData
        Try

            Dim ob As Object = Nothing

            'No node has been selected just return nothing
            If (node Is Nothing) Then Return Nothing

            Do While node.Parent IsNot Nothing
                node = node.Parent
            Loop
            ob = node.Tag

            If ob IsNot Nothing Then
                If TypeOf ob Is IEnviroInputData Then
                    Return DirectCast(ob, IEnviroInputData)
                End If
            End If

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Private Sub PlotMap()
        Try
            If (Me.m_map Is Nothing) Then Return

            Dim histPts() As Drawing.PointF = Me.m_map.Histogram()
            Dim binWidth As Single = Me.m_map.HistogramBinWidth
            Dim lstPts As New PointPairList()
            Dim fmt As New cCoreInterfaceFormatter()

            'The X value in the histogram is the max value of the bin, right hand side of the bin
            'So an input value of 1.0 will be in the .X = 1.0 bin
            For ipt As Integer = 1 To histPts.Length - 1
                lstPts.Add(histPts(ipt).X - binWidth, histPts(ipt).Y)
                lstPts.Add(histPts(ipt).X, histPts(ipt).Y)
            Next

            Dim il As LineItem = Me.m_zgh.CreateLineItem(cStringUtils.Localize(My.Resources.HEADER_HISTOGRAM_TARGET, Me.m_map.Name),
                                                         lstPts, cZedGraphMediationHelper.eEnvResponseLineType.Histogram)

            il.IsY2Axis = True
            il.Line.Fill = New Fill(System.Drawing.Color.Gray)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            'Let the response function decide the plot window size
            'Me.m_zgh.XScaleMax = Me.m_map.Max
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception
            Debug.Assert(False, "PlotMap " & ex.Message)
            cLog.Write(ex)
        End Try

    End Sub

#End Region ' Private Methods

End Class