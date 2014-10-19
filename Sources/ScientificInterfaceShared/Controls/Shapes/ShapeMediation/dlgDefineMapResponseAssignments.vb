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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
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
Imports EwEUtils.Commands

#End Region ' Imports

''' <summary>
''' Main interface to define the functional responses of groups to environmental drivers.
''' </summary>
Public Class dlgDefineMapResponseAssignments

    'ToDo  update graph interface from edit dialog 
    'ToDo Localize text

#Region " Private variables "

    Private m_shape As EwECore.cEnviroResponseFunction = Nothing
    Private m_manager As cMapResponseInteractionManager = Nothing
    Private m_zgh As cZedGraphMediationHelper = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_map As cEnviroInputMap = Nothing

    Private m_fpMin As cEwEFormatProvider = Nothing
    Private m_fpMax As cEwEFormatProvider = Nothing
    Private m_fpMean As cEwEFormatProvider = Nothing
    Private m_fpSD As cEwEFormatProvider = Nothing

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
    Public Sub New(ByVal uic As cUIContext, _
                   ByVal shape As EwECore.cEnviroResponseFunction, _
                   ByVal manager As EwECore.cMapResponseInteractionManager)
        Me.InitializeComponent()

        Me.m_shape = shape
        Me.m_manager = manager

        Me.m_uic = uic

        Me.m_zgh = New cZedGraphMediationHelper()
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
            Me.m_bInUpdate = True

            Me.m_zgh.ConfigurePane(My.Resources.RESPONSE_GRAPH_TITLE, My.Resources.RESPONSE_GRAPH_XLABEL, My.Resources.RESPONSE_GRAPH_YLABEL, True)

            'Yaxis (left) grid lines
            'the cool thing to do here would be to only show the 1.0 grid line
            'not all the grid line....
            Me.m_zgh.GetPane(1).YAxis.MajorGrid.IsVisible = True

            Me.m_zgh.GetPane(1).Y2Axis.IsVisible = True

            ' ToDo: Globalize this
            Me.m_zgh.GetPane(1).Y2Axis.Title.Text = "Map histogram"
            Me.m_zgh.GetPane(1).Y2Axis.Title.IsVisible = True
            Me.m_zgh.GetPane(1).Y2Axis.Title.FontSpec = Me.m_zgh.GetPane(1).YAxis.Title.FontSpec

            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsAllTics = False
            Me.m_zgh.GetPane(1).Y2Axis.MinorTic.IsOpposite = False
            Me.m_zgh.GetPane(1).Y2Axis.MajorTic.IsOpposite = False

            'somehow set the Y2Axis label font size
            Me.m_zgh.GetPane(1).Y2Axis.Scale.MaxAuto = True

            Dim liGroups As New List(Of Integer)
            For iGrp As Integer = 1 To Me.m_uic.Core.nGroups
                Dim grp As cEcospaceGroup = Me.m_uic.Core.EcospaceGroups(iGrp)
                If (grp.CapacityCalculationType = eEcospaceCapacityCalType.Capacity) Then
                    liGroups.Add(iGrp)
                End If
            Next
            Me.m_lbxGroups.Attach(Me.m_uic)
            Me.m_lbxGroups.Populate(liGroups.toArray())

            Me.m_fpMin = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMin, GetType(Single))
            Me.m_fpMax = New cEwEFormatProvider(Me.m_uic, Me.m_tbxXMax, GetType(Single))
            Me.m_fpMean = New cEwEFormatProvider(Me.m_uic, Me.m_tbxMean, GetType(Single))
            Me.m_fpSD = New cEwEFormatProvider(Me.m_uic, Me.m_tbxSD, GetType(Single))

            ' Set min and max
            Me.m_fpMin.Value = Me.m_shape.ResponseLeftLimit
            Me.m_fpMax.Value = Me.m_shape.ResponseRightLimit

        Catch ex As Exception

        End Try

        Me.m_bInUpdate = False
        Me.InitToShapeType()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.m_uic Is Nothing) Then Return

        Me.m_lbxGroups.Detach()

        ' Clear out shape to de-init UI
        Me.m_shape = Nothing
        Me.InitToShapeType()

        Me.m_fpMin.Release()
        Me.m_fpMax.Release()
        Me.m_fpMean.Release()
        Me.m_fpSD.Release()

        MyBase.OnFormClosed(e)

    End Sub

    Private Sub InitToShapeType()

        If (Me.CanEditMinMax) Then
            RemoveHandler Me.m_fpMin.OnValueChanged, AddressOf OnMinMaxValueChanged
            RemoveHandler Me.m_fpMax.OnValueChanged, AddressOf OnMinMaxValueChanged
        End If

        If (Me.CanEditMeanSD) Then
            RemoveHandler Me.m_fpMean.OnValueChanged, AddressOf OnMeanValueChanged
            RemoveHandler Me.m_fpSD.OnValueChanged, AddressOf OnSDValueChanged
        End If

        If (Me.m_shape Is Nothing) Then Return

        If (Me.CanEditMinMax) Then
            AddHandler Me.m_fpMin.OnValueChanged, AddressOf OnMinMaxValueChanged
            AddHandler Me.m_fpMax.OnValueChanged, AddressOf OnMinMaxValueChanged
        End If

        If (Me.CanEditMeanSD) Then
            Me.m_fpMean.Value = Me.m_shape.Steep
            Me.m_fpSD.Value = Me.CalcSDFromXAxis()

            AddHandler Me.m_fpMean.OnValueChanged, AddressOf OnMeanValueChanged
            AddHandler Me.m_fpSD.OnValueChanged, AddressOf OnSDValueChanged
        End If

        Me.LoadMaps()
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
            Me.LoadMaps()

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Private Sub OnRemoveGroup(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnRemove.Click

        Try
            If (Me.m_map Is Nothing) Then Return

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

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnMapTreeExpanded(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvMaps.AfterExpand

        Try
            Me.m_map = Me.GetSelectedMap(e.Node)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnMapTreeSelected(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) _
        Handles m_tvMaps.AfterSelect
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
            Debug.Assert(Me.CanEditMeanSD(), "Oppss BUG! should not be setting the Mean for this type of shape.")
            'Mean is stored in the Steep variable
            Me.m_shape.Steep = CSng(Me.m_fpMean.Value)
            Me.CalcXFromMeanAndSD(Me.m_shape.ResponseLeftLimit, Me.m_shape.ResponseRightLimit)
            Me.UpdatePlots()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnSDValueChanged(sender As System.Object, e As System.EventArgs)
        Try
            If Me.m_bInUpdate Then Return
            Debug.Assert(Me.CanEditMeanSD(), "Oppss BUG! should not be setting the SD for this type of shape.")
            Me.CalcXFromMeanAndSD(Me.m_shape.ResponseLeftLimit, Me.m_shape.ResponseRightLimit)
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

        ' ToDo JS: this will be connected to IShapeFunction behaviour

        Dim bCanAddGroup As Boolean = (Me.m_lbxGroups.SelectedItems.Count > 0)
        Dim bCanRemoveGroup As Boolean = (Me.m_tvMaps.SelectedNode IsNot Nothing)
        Dim bCanSetMinMax As Boolean = Me.CanEditMinMax() Or True
        Dim bCanSetMeanSD As Boolean = Me.CanEditMeanSD()

        ' ToDo: globalize this
        Dim strXMin As String = "x min"
        Dim strXMax As String = "x max"
        Dim strMean As String = "mean"
        Dim strSD As String = "SD" ' All caps

        Select Case Me.m_shape.ShapeFunctionType

            Case eShapeFunctionType.Normal
                ' ToDo: globalize this
                strXMin = "plot min"
                strXMax = "plot max"

                'Me.m_tbxMean.Text = Me.m_shape.Steep.ToString
                'Me.m_tbxSD.Text = Me.m_SD.ToString

            Case eShapeFunctionType.LeftShoulder, _
                 eShapeFunctionType.RightShoulder, _
                 eShapeFunctionType.Trapezoid
                ' ToDo: globalize this
                strXMin = "plot min"
                strXMax = "plot max"

            Case Else
                ' NOP
        End Select

        Me.m_btnAdd.Enabled = bCanAddGroup
        Me.m_btnRemove.Enabled = bCanRemoveGroup

        Me.m_lblXMin.Text = cStyleGuide.ToLabel(strXMin)
        Me.m_lblXMax.Text = cStyleGuide.ToLabel(strXMax)
        Me.m_fpMin.Enabled = bCanSetMinMax
        Me.m_fpMax.Enabled = bCanSetMinMax

        Me.m_lblMean.Text = cStyleGuide.ToLabel(strMean)
        Me.m_lblSD.Text = cStyleGuide.ToLabel(strSD)
        Me.m_fpMean.Enabled = bCanSetMeanSD
        Me.m_fpSD.Enabled = bCanSetMeanSD

    End Sub

    Private Sub CalcXFromMeanAndSD(ByRef XMin As Single, ByRef XMax As Single)

        Dim mean As Single = Me.m_shape.Steep
        Dim widthSD As Single = Me.m_shape.YBase
        Dim sd As Single = CSng(Me.m_fpSD.Value)

        'Compute half the width in the same units as SD (x axis units)
        Dim halfwidth As Single = SD * widthSD / 2.0F
        XMin = mean - halfwidth
        XMax = mean + halfwidth

    End Sub

    Private Function CalcSDFromXAxis() As Single

        Debug.Assert(Not (Me.m_shape.ResponseLeftLimit = 0 And Me.m_shape.ResponseRightLimit = 0), "Opps X Axis has not been set!")

        Dim mean As Single = Me.m_shape.Steep
        Dim widthSD As Single = Me.m_shape.YBase
        Dim range As Single = mean - Me.m_shape.ResponseLeftLimit
        Dim SD As Single = range / (widthSD / 2)
        'If this is a new response function then 
        'SD will be calculated as 0 give it a default of 1
        If SD = 0 Then SD = 1
        Return SD

    End Function

    Private Function CanEditMinMax() As Boolean

        If (Me.m_shape Is Nothing) Then Return False

        Return True

        'If Me.m_shape.ShapeFunctionType <> eShapeFunctionType.Normal Or _
        '    Me.m_shape.ShapeFunctionType <> eShapeFunctionType.LeftShoulder Or _
        '    Me.m_shape.ShapeFunctionType <> eShapeFunctionType.RightShoulder Or _
        '     Me.m_shape.ShapeFunctionType <> eShapeFunctionType.Trapezoid Then
        '    Return False
        'End If
        'Return True

    End Function

    Private Function CanEditMeanSD() As Boolean

        If (Me.m_shape Is Nothing) Then Return False

        Select Case Me.m_shape.ShapeFunctionType
            Case eShapeFunctionType.Normal : Return True
        End Select
        Return False

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
    Private Sub GetPlotMinMax(ByRef sShapeMin As Single, ByRef sShapeMax As Single, _
                              ByRef sPlotMin As Single, ByRef sPlotMax As Single)

        Select Case Me.m_shape.ShapeFunctionType

            Case eShapeFunctionType.Normal
                'Normal distribution shape min and max are set from the Mean SD

                'Use the Min Max on the interface to set the plot window size
                sPlotMin = CSng(Me.m_fpMin.Value)
                sPlotMax = CSng(Me.m_fpMax.Value)

                'Get the Min and Max of the data from Mean, SD and SDWidth
                Me.CalcXFromMeanAndSD(sShapeMin, sShapeMax)

            Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder, eShapeFunctionType.Trapezoid
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
            Dim Xmin As Single = Me.m_shape.ResponseLeftLimit
            Dim Xmax As Single = Me.m_shape.ResponseRightLimit

            Dim XmaxWin As Single
            Dim XminWin As Single

            Me.GetPlotMinMax(Xmin, Xmax, XminWin, XmaxWin)

            '' this is what the core will use to find the x value
            'Xmin = Me.m_shape.ResponseLeftLimit
            'Xmax = Me.m_shape.ResponseRightLimit

            Dim Xrange As Single = Xmax - Xmin
            Dim fmt As New cCoreInterfaceFormatter()

            Dim dx As Single = Xrange / Me.m_shape.nPoints

            Dim YScale As Single = 1
            Dim lstPts As New PointPairList

            Dim x As Double
            For ipt As Integer = 1 To Me.m_shape.nPoints
                x = Xmin + dx * (ipt - 1)
                lstPts.Add(x, Me.m_shape.ShapeData(ipt) * YScale)
            Next

            'add the last point out at the end of the graph
            lstPts.Add(Xmax, Me.m_shape.ShapeData(Me.m_shape.nPoints) * YScale)

            Dim il As LineItem = Me.m_zgh.CreateLineItem(String.Format(My.Resources.HEADER_RESPONSE_TARGET, fmt.GetDescriptor(Me.m_shape)), _
                                                         lstPts, cZedGraphMediationHelper.eEnvResponseLineType.Response)
            Me.m_zgh.GetPane(1).CurveList.Add(il)

            'X axis for plotting
            Me.m_zgh.XScaleMin = XminWin
            Me.m_zgh.XScaleMax = XmaxWin
            Me.m_zgh.YScaleMax = Me.m_shape.YMax + Me.m_shape.YMax * 0.1
            Me.m_zgh.YScaleMin = 0

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub LoadMaps()

        Dim map As IEnviroInputMap = Nothing
        Dim fmt As New cCoreInterfaceFormatter()

        Try
            Me.m_tvMaps.Nodes.Clear()

            For imap As Integer = 1 To Me.m_manager.nMaps

                map = Me.m_manager.Map(imap)
                Dim ndApply As TreeNode = Me.m_tvMaps.Nodes.Add(fmt.GetDescriptor(DirectCast(map, cEnviroInputMap).Layer))
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
            cLog.Write(ex)
            Debug.Assert(False, Me.ToString & ".loadMaps() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub SetDefaultMinMax()

        If (Me.m_map Is Nothing) Then Return

        Me.m_bInUpdate = True

        '' Never use blunt string parsing in EwE6 UI to adhere to EwE number formatting behaviour
        'Me.m_tbxXMax.Text = Me.m_map.Max.ToString
        'Me.m_tbxXMin.Text = Me.m_map.Min.ToString

        Me.m_fpMin.Value = Me.m_map.Min
        Me.m_fpMax.Value = Me.m_map.Max

        Me.m_bInUpdate = False
        Me.ApplyMinMax()

    End Sub

    Private Sub ApplyMinMax()
        If Me.m_bInUpdate Then Return

        'Not all shapes use the Min and Mix data range
        Debug.Assert(Me.CanEditMinMax())

        Try
            Me.m_shape.LockUpdates()
            Me.m_shape.ResponseLeftLimit = CSng(Me.m_fpMin.Value)
            Me.m_shape.ResponseRightLimit = CSng(Me.m_fpMax.Value)
            Me.m_shape.UnlockUpdates(True)
            Me.UpdatePlots()
        Catch ex As Exception

        End Try
        Me.UpdatePlots()

    End Sub

    Private Function GetSelectedMap(ByVal node As TreeNode) As cEnviroInputMap
        Try

            Dim ob As Object = Nothing

            'No node has been selected just return nothing
            If (node Is Nothing) Then Return Nothing

            Do While node.Parent IsNot Nothing
                node = node.Parent
            Loop
            ob = node.Tag

            If ob IsNot Nothing Then
                If TypeOf ob Is cEnviroInputMap Then
                    Return DirectCast(ob, cEnviroInputMap)
                End If
            End If

        Catch ex As Exception

        End Try

        Return Nothing

    End Function

    Private Sub PlotMap()
        Try
            If (Me.m_map Is Nothing) Then Return

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

            Dim il As LineItem = Me.m_zgh.CreateLineItem(String.Format(My.Resources.HEADER_HISTOGRAM_TARGET, fmt.GetDescriptor(Me.m_map.Layer)), _
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




