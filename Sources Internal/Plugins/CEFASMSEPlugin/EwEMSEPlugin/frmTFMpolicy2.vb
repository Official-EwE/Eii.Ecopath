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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
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
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls.EwEGrid

#End Region ' Imports

''' =======================================================================
''' <summary>
''' Form, implementing the Cefas MSE Fishing policy mortality (a.k.a hockey stick) 
''' interface.
''' </summary>
''' =======================================================================
Public Class frmTFMpolicy2

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
    Private m_plugin As cMSE
    Private m_qeh As cQuickEditHandler
    Private m_SelectedStrategy As Strategy

    Private m_HCR As HCR_Group

    Private StrategiesSaved As Boolean = True

#End Region ' Internals

#Region " Construction Initialization "

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(UI As cUIContext, Plugin As cMSE)
        Me.UIContext = UI
        Me.m_plugin = Plugin
    End Sub

#End Region

#Region " Events "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
        Me.m_zgh.ConfigurePane("", String.Format(My.Resources.LABEL_BIOMASS_UNIT, "kt"), SharedResources.HEADER_TFM, True)

        Me.m_zgh.AllowZoom = False
        Me.m_zgh.AllowPan = False
        Me.m_zgh.AllowEdit = True

        Me.m_grid.Init(Me.m_plugin)
        Me.m_grid.UIContext = Me.UIContext

        Me.m_qeh = New cQuickEditHandler()
        Me.m_qeh.Attach(Me.m_grid, Me.UIContext, Me.m_tsHCR)
        Me.m_grid.DataName = "HarvestControlRules"

        Me.UpdateControls()

        If (Me.m_plugin.Strategies.Count > 0) Then
            Me.m_tscmStrategies.SelectedIndex = 0
        End If

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If StrategiesSaved = False Then
            e.Cancel = (Me.m_plugin.AskUser(My.Resources.PROMPT_UNSAVED_CHANGES, eMessageReplyStyle.YES_NO) <> eMessageReplyStyle.OK)
        End If

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        If Me.m_zgh IsNot Nothing Then
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
        End If
        MyBase.OnFormClosed(e)

    End Sub

    Private Sub HandleGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)
        ' Update group selection according to user actions in the grid
        Me.HCRGroup = Me.m_grid.HarvestControlRule
    End Sub

    Private Sub OnGridEdited()
        Try
            Me.Redraw()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub tsbDefaultTFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles tsbDefaultTFM.Click
        'Try
        '    Me.UIContext.Core.SetDefaultTFM()
        'Catch ex As Exception
        '    Debug.Assert(False, ex.Message)
        'End Try
    End Sub


    Private Sub OnSelectedStrategyChanged(sender As Object, e As System.EventArgs) _
        Handles m_tscmStrategies.SelectedIndexChanged

        If Me.m_tscmStrategies.SelectedIndex >= 0 Then
            Me.changeSelectedStrategy(Me.m_tscmStrategies.SelectedIndex)
        End If

    End Sub


    Private Sub changeSelectedStrategy(iSelectedIndex As Integer)

        m_SelectedStrategy = Me.m_plugin.Strategies(iSelectedIndex)
        Me.m_grid.SelectedStrategyIndex = iSelectedIndex
        Me.Redraw()

    End Sub

    Private Sub OnAddHCR(sender As Object, e As System.EventArgs) Handles m_tsbnAddHCR.Click

        'Ask the user to create a new HCR_Group
        Dim HRCDialogue As dlgHarvestControlRule = New dlgHarvestControlRule
        HRCDialogue.Init(Me.m_plugin, Me.m_SelectedStrategy)
        HRCDialogue.ShowDialog()

        If HRCDialogue.DialogResult = Windows.Forms.DialogResult.OK Then
            'add the newly created harvest control rule to the current strategy
            Me.m_SelectedStrategy.Add(HRCDialogue.HarvestControlRule)
            Me.m_grid.RefreshContent()
        End If


    End Sub

    Private Sub OnAddStrategy(sender As Object, e As System.EventArgs) _
        Handles m_tsbnAddStrategy.Click

        ' JS 30Sep13: Globalized
        ' JS 30Sep13: Strategy file name is safe
        ' JS 13Oct13: Replaced use of InputBox

        Try
            Dim StratName As String = ""
            Dim box As New frmInputBox()

            If box.Show(Me, My.Resources.PROMPT_ENTERNAME, My.Resources.PROMPT_ENTERNAME_CAPTION) = Windows.Forms.DialogResult.OK Then
                StratName = box.Value
            End If

            If String.IsNullOrWhiteSpace(StratName) Then Return

            'Build the filename out of the strategy name
            Dim StartFilename As String = Path.Combine(Me.m_plugin.Strategies.DataDirectory, cFileUtils.ToValidFileName(StratName + ".csv", False))
            Dim strategy As Strategy = New Strategy(StratName, StartFilename)

            ' JS 30Sep13: Strategies class validates both strategy name and file. VERY GOOD!!
            If (Not Me.m_plugin.Strategies.Contains(strategy)) Then
                Me.m_plugin.Strategies.Add(strategy)
                Me.UpdateControls()
                Me.changeSelectedStrategy(Me.m_tscmStrategies.Items.Count - 1)
            Else
                Me.m_plugin.InformUser(My.Resources.ERROR_ENTERNAME, eMessageImportance.Warning)
            End If

        Catch ex As Exception

        End Try

    End Sub


    Private Sub btnSaveStrategies_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tsbnSaveToCSV.Click

        ' JS 30Sep13: CSV file written in fixed digit format
        ' JS 30Sep13: Uses safe streamwriter

        Dim csvStrategyFile As StreamWriter = Nothing

        For Each iStrategy In Me.m_plugin.Strategies
            csvStrategyFile = cMSEUtils.GetWriter(iStrategy.FileName, False)
            If (csvStrategyFile IsNot Nothing) Then

                csvStrategyFile.WriteLine("GroupNameForBiomass,GroupNumberForBiomass,LowerLimit,UpperLimit,GroupNameForF,GroupNumberForF,MaxF,CostFunctionType")
                For Each iHCR In iStrategy
                    csvStrategyFile.WriteLine(cStringUtils.ToCSVField(iHCR.GroupB.Name) & "," & _
                                              cStringUtils.ToCSVField(iHCR.GroupB.Index) & "," & _
                                              cStringUtils.ToCSVField(iHCR.LowerLimit) & "," & _
                                              cStringUtils.ToCSVField(iHCR.UpperLimit) & "," & _
                                              cStringUtils.ToCSVField(iHCR.GroupF.Name) & "," & _
                                              cStringUtils.ToCSVField(iHCR.GroupF.Index) & "," & _
                                              cStringUtils.ToCSVField(iHCR.MaxF) & "," & _
                                              cStringUtils.ToCSVField(HCR_Group.toCostFunctionString(iHCR.CostFunction)))
                Next
                cMSEUtils.ReleaseWriter(csvStrategyFile)
            End If
        Next

    End Sub

    Private Sub OnDeleteStrategy(sender As System.Object, e As System.EventArgs) Handles m_tsbnDeleteStrategy.Click
        Dim selStrategy As Integer = m_tscmStrategies.SelectedIndex

        'ToDo this needs to delete the Strategy file as well as removing it from the list
        'that should happen from the Strategies object itself
        'Also there should be an isDirty flag
        If selStrategy >= 0 Then
            Me.m_plugin.Strategies.RemoveAt(selStrategy)
            Me.UpdateControls()
            Me.m_tscmStrategies.SelectedIndex = 0
        End If

    End Sub

    Private Sub OnDeleteHCR(sender As System.Object, e As System.EventArgs) Handles m_tsbnAddHCR.Click
        Dim selHCRIndex As Integer = Me.m_grid.SelectedRow
        Dim curStratIndex As Integer = Me.m_tscmStrategies.SelectedIndex

        If selHCRIndex > 0 Then
            If Me.m_SelectedStrategy IsNot Nothing Then
                'ToDo Like the Deleted Strategy this should be handled by the Strategy object
                'that way there can be an isDirty flag
                Me.m_SelectedStrategy.RemoveAt(selHCRIndex - 1)
                Me.UpdateControls()
                If curStratIndex > -1 And curStratIndex < Me.m_tscmStrategies.Items.Count Then
                    Me.m_tscmStrategies.SelectedIndex = curStratIndex
                End If
            End If
        End If

    End Sub

#End Region ' Events

#Region " Internals "

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

        If (Me.m_zgh Is Nothing) Then Return

        Dim lpts As New PointPairList
        Dim line As LineItem = Nothing
        Dim lLines As New List(Of LineItem)
        Dim fmt As New cCoreInterfaceFormatter()

        Try

            If Me.m_HCR IsNot Nothing Then

                ' #Yes: plot stick
                Dim bsum As Double = Me.m_HCR.LowerLimit + Me.m_HCR.UpperLimit
                If bsum > 0 Then
                    ' Add points
                    lpts.Add(0, 0)
                    lpts.Add(Units.Convert(eConvertTypes.ToDisplayBio, Me.m_HCR.LowerLimit), 0)
                    lpts.Add(Units.Convert(eConvertTypes.ToDisplayBio, Me.m_HCR.UpperLimit), Me.m_HCR.MaxF) ' Point order?
                    lpts.Add(Units.Convert(eConvertTypes.ToDisplayBio, Me.m_HCR.UpperLimit) * 4, Me.m_HCR.MaxF) ' Max X value?
                Else
                    'Zero biomass values user has only entered F
                    'draw a square line at zero up to F
                    lpts.Add(-1, 0)
                    lpts.Add(0, 0)
                    lpts.Add(0, Me.m_HCR.MaxF) ' Point order?
                    lpts.Add(4, Me.m_HCR.MaxF) ' Max X value?
                End If

                line = New LineItem(fmt.GetDescriptor(Me.m_HCR.GroupB), lpts, Me.StyleGuide.GroupColor(Me.Core, Me.m_HCR.GroupB.Index), SymbolType.Circle)
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
            Return Me.m_plugin
        End Get
    End Property

    Public Shadows Sub UpdateControls()
        MyBase.UpdateControls()

        Try
            Dim i As Integer
            Me.m_tscmStrategies.Items.Clear()
            For Each strategy As Strategy In Me.m_plugin.Strategies
                i += 1
                Me.m_tscmStrategies.Items.Add(strategy.Name)
            Next
        Catch ex As Exception

        End Try

    End Sub

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
                    Me.m_HCR.LowerLimit = Math.Max(0, Math.Min(Units.Convert(eConvertTypes.ToEcopathBio, dX), Me.m_HCR.UpperLimit))
                Case eDragType.BUpperFMax
                    Me.m_HCR.UpperLimit = Math.Max(Me.m_HCR.LowerLimit, Units.Convert(eConvertTypes.ToEcopathBio, dX))
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
                Case eDragType.FMax
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
            End Select
            Me.Redraw()
            Me.m_grid.UpdateContent()
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

End Class


