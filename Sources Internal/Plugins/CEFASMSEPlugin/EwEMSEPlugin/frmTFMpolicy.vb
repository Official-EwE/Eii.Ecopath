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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports System.Windows.Forms
Imports ZedGraph
Imports ScientificInterfaceShared.Controls
Imports SourceGrid2
Imports System.IO

#End Region ' Imports


''' =======================================================================
''' <summary>
''' Form, implementing the Ecosim Fishing policy mortality (a.k.a hockey stick) 
''' interface.
''' </summary>
''' =======================================================================
Public Class frmTFMpolicy


    'ToDo 28-June-2013 Make grid editable
    'ToDo 28-June-2013 Implement adding of HCR to current strategy
    'This will require a way to select the Biomass and F groups
    'ToDo 28-June-2013 Saving of Strategies



#Region " Internals "

    Private Enum eDragType As Integer
        None = 0
        BLower
        BUpperFMax
        FMax
    End Enum

    ''' <summary><see cref="cZedGraphHelper">Helper</see> to manipulate the graph.</summary>
    Private m_zgh As cZedGraphHelper
    ''' <summary>Graph drag mode.</summary>
    Private m_dragtype As eDragType = eDragType.None

    ''' <summary>MSE Plugin initialized in me.Init(cUIContext,cMSE)</summary>
    ''' <remarks>Provides access to data.</remarks>
    Private m_MSEPlugin As cMSE

    Private m_SelectedStrategy As Strategy

    Private m_HCR As HCR_Group

#End Region ' Internals

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(UI As cUIContext, Plugin As cMSE)
        Me.UIContext = UI
        Me.m_MSEPlugin = Plugin
    End Sub

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If Me.UIContext Is Nothing Then Return

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
        Me.m_zgh.ConfigurePane("", SharedResources.HEADER_BIOMASS, SharedResources.HEADER_TFM, True)

        Me.m_zgh.AllowZoom = False
        Me.m_zgh.AllowPan = False
        Me.m_zgh.AllowEdit = True

        Me.m_grid.Init(Me.m_MSEPlugin)
        Me.m_grid.UIContext = Me.UIContext

        populateStrategies()

        Me.cbStrategies.SelectedIndex = 0

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If Me.m_zgh IsNot Nothing Then
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
        End If

        MyBase.OnFormClosed(e)
    End Sub

    Private Sub HandleGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection) Handles m_grid.OnSelectionChanged
        ' Update group selection according to user actions in the grid
        Me.HCRGroup = Me.m_grid.HarvestControlRule
    End Sub

    Private Sub OnGridEdited() Handles m_grid.onEdited
        Try
            Me.Redraw()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub tsbDefaultTFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tsbDefaultTFM.Click
        'Try
        '    Me.UIContext.Core.SetDefaultTFM()
        'Catch ex As Exception
        '    Debug.Assert(False, ex.Message)
        'End Try
    End Sub

#End Region ' Events

#Region " Internals "

    Sub populateStrategies()

        Try
            Dim i As Integer
            Me.cbStrategies.Items.Clear()
            For Each strategy As Strategy In Me.m_MSEPlugin.Strategies
                i += 1
                Me.cbStrategies.Items.Add(strategy.Name)
            Next
        Catch ex As Exception

        End Try
    End Sub


    Private Property HCRGroup() As HCR_Group
        Get
            Return m_HCR
        End Get
        Set(value As HCR_Group)
            Me.m_HCR = value
            If Me.m_HCR IsNot Nothing Then
                Redraw()
            End If
        End Set
    End Property


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Redraw the quota curve.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub Redraw()

        If Me.m_zgh Is Nothing Then Return

        Dim lpts As New PointPairList
        Dim line As LineItem = Nothing
        Dim lLines As New List(Of LineItem)
        Try

            If Me.m_HCR IsNot Nothing Then



                ' #Yes: plot stick
                Dim bsum As Double = Me.m_HCR.LowerLimit + Me.m_HCR.UpperLimit
                If bsum > 0 Then
                    ' Add points
                    lpts.Add(0, 0)
                    lpts.Add(Me.m_HCR.LowerLimit, 0)
                    lpts.Add(Me.m_HCR.UpperLimit, Me.m_HCR.MaxF) ' Point order?
                    lpts.Add(Me.m_HCR.UpperLimit * 4, Me.m_HCR.MaxF) ' Max X value?
                Else
                    'Zero biomass values user has only entered F
                    'draw a square line at zero up to F
                    lpts.Add(-1, 0)
                    lpts.Add(0, 0)
                    lpts.Add(0, Me.m_HCR.MaxF) ' Point order?
                    lpts.Add(4, Me.m_HCR.MaxF) ' Max X value?
                End If

                line = New LineItem(Me.m_HCR.GroupName4Biomass, lpts, Me.StyleGuide.GroupColor(Me.Core, Me.m_HCR.GroupNumber4Biomass), SymbolType.Circle)
                line.Line.Width = 2.0

                lLines.Add(line)
            End If

            If lLines.Count > 0 Then
                ' Plot graph, but rescale ONLY when not dragging
                Me.m_zgh.PlotLines(lLines.ToArray, 1, (Me.m_dragtype = eDragType.None))
                Me.m_graph.Cursor = Cursors.Default
            Else
                ' Clear graph
                Me.m_zgh.PlotLines(Nothing)
                Me.m_graph.Cursor = Cursors.No
            End If

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

    End Sub


    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_MSEPlugin
        End Get
    End Property

#End Region ' Internals

#Region " Dragging "

    Private Function HandleGraphMouseDownEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
            Handles m_graph.MouseDownEvent

        Dim pane As GraphPane = sender.GraphPane
        Dim pt As PointF = New PointF(e.X, e.Y)
        Dim curve As CurveItem = Nothing
        Dim iIndex As Integer = 0

        ' Find the point that was clicked, and make sure the point list is editable
        If (pane.FindNearestPoint(pt, curve, iIndex)) Then
            If (curve IsNot Nothing) Then
                If (TypeOf curve.Points Is PointPairList) Then
                    ' Set drag operation type
                    Me.m_dragtype = DirectCast(iIndex, eDragType)
                End If
            End If
        End If

        Return False

    End Function

    Private Function m_graph_MouseMoveEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
        Handles m_graph.MouseMoveEvent

        Dim pane As GraphPane = sender.GraphPane
        Dim pt As PointF = New PointF(e.X, e.Y)
        Dim curve As CurveItem = Nothing
        Dim iIndex As Integer = 0
        Dim bIsNear As Boolean = False

        ' Find the point that was clicked, and make sure the point list is editable
        If (pane.FindNearestPoint(pt, curve, iIndex)) Then
            bIsNear = (curve IsNot Nothing)
        End If

        If bIsNear Then
            Me.m_graph.Cursor = Cursors.Hand
        Else
            Me.m_graph.Cursor = Cursors.Default
        End If
        Return True

    End Function

    Private Function HandleGraphMouseMoveEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
            Handles m_graph.MouseMoveEvent

        If Me.m_HCR Is Nothing Then Return False

        Dim pane As GraphPane = sender.GraphPane
        Dim pt As PointF = New PointF(e.X, e.Y)
        Dim dX As Double = 0.0
        Dim dy As Double = 0.0

        ' Dragging?
        If (Me.m_dragtype <> eDragType.None) Then
            ' Translate value
            pane.ReverseTransform(pt, dX, dy)

            Select Case Me.m_dragtype
                Case eDragType.BLower
                    Me.m_HCR.LowerLimit = Math.Max(0, Math.Min(CSng(dX), Me.m_HCR.UpperLimit))
                Case eDragType.BUpperFMax
                    Me.m_HCR.UpperLimit = Math.Max(Me.m_HCR.LowerLimit, CSng(dX))
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
                Case eDragType.FMax
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
            End Select
            Me.Redraw()
            Me.m_grid.Update()
        End If
        Return True

    End Function

    Private Function HandleGraphMouseUpEvent(ByVal sender As ZedGraphControl, ByVal e As MouseEventArgs) As Boolean _
            Handles m_graph.MouseUpEvent

        Me.m_dragtype = eDragType.None
        Me.m_zgh.RescaleAndRedraw()
        Return True

    End Function

#End Region ' Dragging

    Private Sub OnSelectedStrategyChanged(sender As Object, e As System.EventArgs) Handles cbStrategies.SelectedIndexChanged

        If Me.cbStrategies.SelectedIndex >= 0 Then
            Me.changeSelectedStrategy(Me.cbStrategies.SelectedIndex)
        End If
    End Sub


    Private Sub changeSelectedStrategy(iSelectedIndex As Integer)

        m_SelectedStrategy = Me.m_MSEPlugin.Strategies(iSelectedIndex)
        Me.m_grid.SelectedStrategyIndex = iSelectedIndex
        Me.Redraw()

        'figure out how to pass the selected group up from the grid
        'repopulate the grid
        'select a group
        'redraw the graph for the selected group

    End Sub

    Private Sub btAddHCR_Click(sender As Object, e As System.EventArgs) Handles btAddHCR.Click

        'Create a new HCR_Group
        Dim HRCDialogue As dlgHarvestControlRule = New dlgHarvestControlRule
        HRCDialogue.Init(Me.m_MSEPlugin)
        HRCDialogue.ShowDialog()

        If HRCDialogue.DialogResult = Windows.Forms.DialogResult.OK Then
            'add the newly created harvest control rule to the current strategy
            Me.m_SelectedStrategy.HCRules.Add(HRCDialogue.HarvestControlRule)
            Me.m_grid.RefreshContent()
        End If

        'assign a Biomass and F group to it, not sure how  this should look
        'Set default lower and upper biomass limits
        'set default F (possible the current Ecopath F)
        'Added it to the current strategy
        'Update the grid it should appear


    End Sub

    Private Sub btAddStrategy_Click(sender As Object, e As System.EventArgs) Handles btAddStrategy.Click

        'Get a Strategy name from the user
        Dim StratName As String
        StratName = InputBox("Select a new for the new Strategy", "Add new Strategy.")

        If String.IsNullOrEmpty(StratName) Then
            Return
        End If

        Dim StartFilename As String = Path.Combine(Me.m_MSEPlugin.Strategies.DataDirectory, StratName + ".csv")
        Dim strategy As Strategy = New Strategy(StratName, StartFilename)
        Me.m_MSEPlugin.Strategies.Add(strategy)

        Me.populateStrategies()
        Me.changeSelectedStrategy(Me.cbStrategies.Items.Count - 1)

    End Sub


    Private Sub btnSaveStrategies_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveStrategies.Click

        Dim csvStrategyFile As StreamWriter

        For Each iStrategy In Me.m_MSEPlugin.Strategies
            csvStrategyFile = New StreamWriter(iStrategy.FileName, False)
            csvStrategyFile.WriteLine("GroupNameForBiomass,GroupNumberForBiomass,LowerLimit,UpperLimit,GroupNameForF,GroupNumberForF,MaxF,CostFunctionType")
            For Each iHCR In iStrategy.HCRules
                csvStrategyFile.WriteLine(iHCR.GroupName4Biomass & "," & iHCR.GroupNumber4Biomass & "," & iHCR.LowerLimit & "," & _
                                            iHCR.UpperLimit & "," & iHCR.GroupName4F & "," & iHCR.GroupNumber4F & "," & iHCR.MaxF & "," & iHCR.CostFunction)
            Next
            csvStrategyFile.Dispose()
        Next

    End Sub

    Private Sub btDeleteStrategy_Click(sender As System.Object, e As System.EventArgs) Handles btDeleteStrategy.Click
        Dim selStrategy As Integer = cbStrategies.SelectedIndex

        'ToDo this needs to delete the Strategy file as well as removing it from the list
        'that should happen from the Strategies object itself
        'Also there should be an isDirty flag
        If selStrategy >= 0 Then
            Me.m_MSEPlugin.Strategies.RemoveAt(selStrategy)

            populateStrategies()
            Me.cbStrategies.SelectedIndex = 0
        End If

    End Sub

    Private Sub btDeleteHCR_Click(sender As System.Object, e As System.EventArgs) Handles btDeleteHCR.Click
        Dim selHCRIndex As Integer = Me.m_grid.SelectedRow
        Dim curStratIndex As Integer = Me.cbStrategies.SelectedIndex

        If selHCRIndex > 0 Then
            If Me.m_SelectedStrategy IsNot Nothing Then
                'ToDo Like the Deleted Strategy this should be handled by the Strategy object
                'that way there can be an isDirty flag
                Me.m_SelectedStrategy.HCRules.RemoveAt(selHCRIndex - 1)
                populateStrategies()
                If curStratIndex > -1 And curStratIndex < Me.cbStrategies.Items.Count Then
                    Me.cbStrategies.SelectedIndex = curStratIndex
                End If
            End If
        End If

    End Sub

End Class


