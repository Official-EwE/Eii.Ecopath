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

#End Region ' Imports

Public Class dlgDefineMapResponseAssignments

#Region "Private variables"

    Private m_shape As EwECore.cEnviroResponseFunction = Nothing
    Private m_manager As cMapResponseInteractionManager = Nothing
    Private m_zgh As cZedGraphMediationHelper = Nothing
    Private m_uic As cUIContext = Nothing
    'Private m_bHasInit As Boolean
    Private m_map As cEnviroInputMap = Nothing
    'Private m_fpXMin As cEwEFormatProvider = Nothing
    'Private m_fpXMax As cEwEFormatProvider = Nothing

    Private m_ShapeGUIHandler As cShapeGUIHandler

    Private m_SD As Single

    Private m_bInInit As Boolean

#End Region

#Region "Construction Initialization"

    Public Sub New(ByVal UIC As cUIContext, ByVal ResponseShape As EwECore.cEnviroResponseFunction, ByVal Manager As EwECore.cMapResponseInteractionManager, ByVal ParentShapeGUIHandler As cShapeGUIHandler)
        Me.InitializeComponent()

        Me.m_shape = ResponseShape
        Me.m_manager = Manager
        Me.m_ShapeGUIHandler = ParentShapeGUIHandler

        Me.m_uic = UIC

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
            Me.m_bInInit = True

            Me.m_tbxXMax.Text = Me.m_shape.ResponseRightLimit.ToString
            Me.m_tbxXMin.Text = Me.m_shape.ResponseLeftLimit.ToString


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

            Me.m_lbxGroups.Attach(Me.m_uic)

            Me.calcSDFromXAxis()

            Me.loadMaps()
            Me.UpdatePlots()

            ' Me.m_bHasInit = True
            Me.updateControls()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".OnLoad() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try

        Me.m_bInInit = False

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If (Me.m_uic Is Nothing) Then Return

        Me.m_lbxGroups.Detach()

        MyBase.OnFormClosed(e)

    End Sub

#End Region

#Region "Control Event Handlers"

    Private Sub OnGroupSelectionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_lbxGroups.SelectedValueChanged
        Try
            Me.updateControls()
        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Add the selected groups to the currently selected map
    ''' </summary>
    Private Sub OnAddGroup(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btnAdd.Click

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

    Private Sub OnRemove(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_btnRemove.Click
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

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnOk.Click

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub trvMapTree_AfterExpand(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tvMaps.AfterExpand
        Me.m_map = Me.GetSelectedMap(e.Node)
    End Sub

    Private Sub trvMapTree_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles m_tvMaps.AfterSelect
        Try
            Me.m_map = GetSelectedMap(e.Node)
            Me.updateControls()
            Me.UpdatePlots()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnMinMaxTextChanged(ByVal sender As Object, args As EventArgs)
        ' Format providers changed: update the map
        Me.UpdatePlots()
    End Sub

    Private Sub OnSetDefaultMinMax(ByVal sender As Object, ByVal e As EventArgs) Handles m_btnDefaultMinMax.Click
        Me.setDefaultMinMax()
    End Sub

    Private Sub txMean_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_txMean.TextChanged
        Try
            If Me.m_bInInit Then Return
            'Mean is stored in the Steep variable
            Debug.Assert(Me.m_shape.ShapeFunctionType = eShapeFunctionType.Normal, "Oppss BUG! should not be setting the Mean for shapes that are not Normal.")
            Me.m_shape.Steep = Single.Parse(Me.m_txMean.Text)
            Me.calcXFromMeanAndSD(Me.m_SD, Me.m_shape.ResponseLeftLimit, Me.m_shape.ResponseRightLimit)
            UpdatePlots()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub txSD_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_txSD.TextChanged
        Try
            If Me.m_bInInit Then Return
            Debug.Assert(Me.m_shape.ShapeFunctionType = eShapeFunctionType.Normal, "Oppss BUG! should not be setting the SD for shapes that are not Normal.")
            Me.m_SD = Single.Parse(Me.m_txSD.Text)
            Me.calcXFromMeanAndSD(Me.m_SD, Me.m_shape.ResponseLeftLimit, Me.m_shape.ResponseRightLimit)
            UpdatePlots()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub onMinMax_TextChanged(sender As System.Object, e As System.EventArgs) Handles m_tbxXMin.TextChanged, m_tbxXMax.TextChanged

        If Me.m_bInInit Then Return
        Try
            'Not all shapes use the Min and Mix data range
            If Me.CanUpdateMinMax() Then
                Me.m_shape.ResponseLeftLimit = Single.Parse(Me.m_tbxXMin.Text)
                Me.m_shape.ResponseRightLimit = Single.Parse(Me.m_tbxXMax.Text)
            End If
            Me.UpdatePlots()
        Catch ex As Exception

        End Try

    End Sub


    Private Sub m_btChangeShape_Click(sender As System.Object, e As System.EventArgs) Handles m_btChangeShape.Click
        Try
            Me.ChangeFFShape()
            Me.UpdatePlots()
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Private Methods"

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

    Private Sub updateControls()

        Try

            Dim bCanAddGroup As Boolean = (Me.m_lbxGroups.SelectedItems.Count > 0)
            Dim bCanRemoveGroup As Boolean = (Me.m_tvMaps.SelectedNode IsNot Nothing)

            Me.m_btnAdd.Enabled = bCanAddGroup
            Me.m_btnRemove.Enabled = bCanRemoveGroup

            Select Case Me.m_shape.ShapeFunctionType

                Case eShapeFunctionType.Normal

                    Me.m_tbxXMax.Enabled = True
                    Me.m_tbxXMin.Enabled = True

                    Me.m_lblXMin.Enabled = True
                    Me.m_lblXMax.Enabled = True

                    Me.m_lblXMin.Text = "Plot min:"
                    Me.m_lblXMax.Text = "Plot max:"

                    Me.m_txMean.Enabled = True
                    Me.m_txSD.Enabled = True
                    Me.m_txMean.Text = Me.m_shape.Steep.ToString
                    Me.m_txSD.Text = Me.m_SD.ToString

                Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder

                    Me.m_tbxXMax.Enabled = True
                    Me.m_tbxXMin.Enabled = True

                    Me.m_lblXMin.Enabled = True
                    Me.m_lblXMax.Enabled = True

                    Me.m_lblXMin.Text = "Plot min:"
                    Me.m_lblXMax.Text = "Plot max:"

                    Me.m_txMean.Enabled = False
                    Me.m_txSD.Enabled = False
                    Me.m_txMean.Text = ""
                    Me.m_txSD.Text = ""

                    Me.m_lbMean.Enabled = False
                    Me.m_lbSD.Enabled = False



                Case Else

                    Me.m_tbxXMax.Enabled = True
                    Me.m_tbxXMin.Enabled = True

                    Me.m_lblXMin.Enabled = True
                    Me.m_lblXMax.Enabled = True

                    'the space after the text are so the label will line up when the text is swapped 
                    Me.m_lblXMin.Text = "X min:  "
                    Me.m_lblXMax.Text = "X max:  "

                    Me.m_txMean.Enabled = False
                    Me.m_txSD.Enabled = False
                    Me.m_txMean.Text = ""
                    Me.m_txSD.Text = ""

                    Me.m_lbMean.Enabled = False
                    Me.m_lbSD.Enabled = False

            End Select

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

    Private Sub calcXFromMeanAndSD(SD As Single, ByRef XMin As Single, ByRef XMax As Single)

        Dim mean As Single = Me.m_shape.Steep
        Dim widthSD As Single = Me.m_shape.YBase

        'Compute half the width in the same units as SD (x axis units)
        Dim halfwidth As Single = SD * widthSD / 2.0F
        XMin = mean - halfwidth
        XMax = mean + halfwidth

    End Sub


    Private Sub calcSDFromXAxis()
        Debug.Assert(Not (Me.m_shape.ResponseLeftLimit = 0 And Me.m_shape.ResponseRightLimit = 0), "Opps X Axis has not been set!")
        Dim mean As Single = Me.m_shape.Steep
        Dim widthSD As Single = Me.m_shape.YBase

        Dim range As Single = mean - Me.m_shape.ResponseLeftLimit
        Me.m_SD = range / (widthSD / 2)
        'If this is a new response function then 
        'SD will be calculated as 0 give it a default of 1
        If Me.m_SD = 0 Then Me.m_SD = 1
    End Sub

    Private Function CanUpdateMinMax() As Boolean
        If Me.m_shape.ShapeFunctionType <> eShapeFunctionType.Normal Or _
            Me.m_shape.ShapeFunctionType <> eShapeFunctionType.LeftShoulder Or _
            Me.m_shape.ShapeFunctionType <> eShapeFunctionType.RightShoulder Then
            Return False
        End If
        Return True
    End Function


    Private Sub ChangeFFShape()
        Dim dlg As New dlgChangeShape(Me.m_uic, DirectCast(Me.m_shape, cForcingFunction), Me.m_ShapeGUIHandler)
        dlg.ShowDialog(Me.m_uic.FormMain)
    End Sub


    Private Sub getPlotMinMax(ByRef ShapeMin As Single, ByRef ShapeMax As Single, ByRef PlotMin As Single, ByRef PlotMax As Single)

        Select Case Me.m_shape.ShapeFunctionType

            Case eShapeFunctionType.Normal
                'Normal distribution shape min and max are set from the Mean SD

                'Use the Min Max on the interface to set the plot window size
                PlotMin = Single.Parse(Me.m_tbxXMin.Text)
                PlotMax = Single.Parse(Me.m_tbxXMax.Text)

                'Get the Min and Max of the data from Mean, SD and SDWidth
                Me.calcXFromMeanAndSD(Me.m_SD, ShapeMin, ShapeMax)


            Case eShapeFunctionType.LeftShoulder, eShapeFunctionType.RightShoulder
                'Shoulder shape min and max can not be set here
                'They only get set from the ChangeShape dialogue

                'Min and Max of the plot window NOT the data
                PlotMax = Single.Parse(Me.m_tbxXMax.Text)
                PlotMin = Single.Parse(Me.m_tbxXMin.Text)

                'The min and max of the data cannot be changed here
                ShapeMin = Me.m_shape.ResponseLeftLimit
                ShapeMax = Me.m_shape.ResponseRightLimit


            Case Else
                'For all other shape the Min Max get set for the Min and Max textbox on this form
                ShapeMin = CSng(Me.m_tbxXMin.Text)
                ShapeMax = CSng(Me.m_tbxXMax.Text)
                PlotMin = ShapeMin
                PlotMax = ShapeMax

        End Select
    End Sub


    Private Sub PlotShape()

        Try

            'Min Max of the response function
            Dim Xmin As Single
            Dim Xmax As Single

            'Min and Max of the plot window NOT the response function
            Dim XmaxWin As Single
            Dim XminWin As Single

            Me.getPlotMinMax(Xmin, Xmax, XminWin, XmaxWin)

            'set the Min and Max on the response function
            'this is what the core will use to find the x value
            Me.m_shape.ResponseLeftLimit = Xmin
            Me.m_shape.ResponseRightLimit = Xmax

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

    Private Sub loadMaps()

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


    Private Sub setDefaultMinMax()

        If (Me.m_map Is Nothing) Then
            'some kind of a warning
            Exit Sub
        End If

        Me.m_tbxXMax.Text = Me.m_map.Max.ToString
        Me.m_tbxXMin.Text = Me.m_map.Min.ToString

    End Sub

    Private Function GetSelectedMap(ByVal node As TreeNode) As cEnviroInputMap
        Try

            Dim ob As Object

            'No node has been selected just return nothing
            If node Is Nothing Then Return Nothing

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

#End Region


End Class




