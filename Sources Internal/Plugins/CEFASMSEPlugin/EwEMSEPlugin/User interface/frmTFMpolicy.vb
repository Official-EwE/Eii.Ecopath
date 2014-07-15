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
Public Class frmTFMpolicy

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
    Private m_MSE As cMSE
    Private m_qeh As cQuickEditHandler
    Private m_SelectedStrategy As Strategy
    Private m_HCR As HCR_Group

    Private m_strategies As Strategies
    Private m_bStrategiesSaved As Boolean = True

#End Region ' Internals

#Region " Construction Initialization "

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Public Sub Init(UI As cUIContext, MSE As cMSE)
        Me.UIContext = UI
        Me.m_MSE = MSE

        Me.m_strategies = New Strategies(MSE, Me.UIContext.Core)
        Me.m_strategies.Load()
    End Sub

#End Region

#Region " Form overrides "

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph)
        Me.m_zgh.ConfigurePane("", String.Format(My.Resources.LABEL_BIOMASS_UNIT, "kt"), SharedResources.HEADER_TFM, True)

        Me.m_zgh.AllowZoom = False
        Me.m_zgh.AllowPan = False
        Me.m_zgh.AllowEdit = True
        Me.m_zgh.ShowHoverMenu = False

        Me.m_grid.UIContext = Me.UIContext
        Me.m_gridRegulations.UIContext = Me.UIContext

        Me.m_qeh = New cQuickEditHandler()
        Me.m_qeh.ShowImportExport = False
        Me.m_qeh.Attach(Me.m_grid, Me.UIContext, Me.m_tsHCR)
        Me.m_grid.DataName = "HarvestControlRules"
        AddHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged
        AddHandler Me.m_grid.onEdited, AddressOf OnGridEdited

        Me.UpdateStrategies()

        If (Me.m_tscmStrategies.Items.Count > 0) Then
            Me.m_tscmStrategies.SelectedIndex = 0
        End If

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As System.Windows.Forms.FormClosingEventArgs)

        If m_bStrategiesSaved = False Then
            e.Cancel = (Me.m_MSE.AskUser(My.Resources.PROMPT_UNSAVED_CHANGES, eMessageReplyStyle.YES_NO) = eMessageReplyStyle.OK)
        End If
        Me.m_qeh.Detach()

        MyBase.OnFormClosing(e)

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_grid.OnSelectionChanged, AddressOf OnGridSelectionChanged
        RemoveHandler Me.m_grid.onEdited, AddressOf OnGridEdited

        If (Me.m_zgh IsNot Nothing) Then
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
        End If
        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Form overrides

#Region " Events "

    ' -----------------------------
    ' Strategies 
    ' -----------------------------

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
            Dim StartFilename As String = Path.Combine(cMSEUtils.MSEFolder(Me.m_MSE.DataPath, cMSEUtils.eMSEPaths.Strategies), cFileUtils.ToValidFileName(StratName + ".csv", False))
            Dim NumberOfStrategies As Integer = Me.m_strategies.Count
            Dim strategy As Strategy = New Strategy(StratName, NumberOfStrategies + 1, StartFilename, Me.Core, Me.m_MSE)

            ' JS 30Sep13: Strategies class validates both strategy name and file. VERY GOOD!!
            If (Not Me.m_strategies.Contains(strategy)) Then
                Me.m_strategies.Add(strategy)
                Me.UpdateStrategies()
                Me.SelectedStrategy = Me.m_tscmStrategies.Items.Count - 1
                Me.m_bStrategiesSaved = False
            Else
                Me.m_MSE.InformUser(My.Resources.ERROR_ENTERNAME, eMessageImportance.Warning)
            End If

        Catch ex As Exception

        End Try

        Me.UpdateControls()

    End Sub

    Private Sub OnDeleteStrategy(sender As System.Object, e As System.EventArgs) _
        Handles m_tsbnDeleteStrategy.Click

        Try
            Dim selStrategy As Integer = m_tscmStrategies.SelectedIndex

            'ToDo this needs to delete the Strategy file as well as removing it from the list
            'that should happen from the Strategies object itself
            'Also there should be an isDirty flag
            If selStrategy >= 0 Then
                Me.m_strategies.RemoveAt(selStrategy)
                Me.UpdateStrategies()
                Me.SelectedStrategy = Me.m_tscmStrategies.Items.Count - 1
                Me.m_bStrategiesSaved = False
            End If
        Catch ex As Exception

        End Try
        Me.UpdateControls()

        'Remove quotashares for groups no longer with a hcr
        Me.m_MSE.QuotaShares.RemoveUnnecessaryShares()
        'Save the quotashares to csv
        Me.m_MSE.QuotaShares.Save()

    End Sub

    Private Sub OnSelectedStrategyChanged(sender As Object, e As System.EventArgs) _
        Handles m_tscmStrategies.SelectedIndexChanged

        Try
            If Me.m_tscmStrategies.SelectedIndex >= 0 Then
                Me.SelectedStrategy = Me.m_tscmStrategies.SelectedIndex
            End If
        Catch ex As Exception

        End Try
        Me.UpdateControls()

    End Sub

    ' -----------------------------
    ' Controls
    ' -----------------------------

    Private Sub OnSave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles m_btnSave.Click

        Try
            If Me.m_strategies.Save() Then
                Me.m_MSE.Strategies.Load()
                Me.m_MSE.InvalidateData()
            End If

            Me.m_bStrategiesSaved = Me.m_strategies.Save()
            Me.UpdateControls()

            Me.m_MSE.Strategies.Clear()
            Me.m_MSE.Strategies.AddRange(Me.m_strategies.ToArray)
            Me.m_MSE.InvalidateData()

            Me.m_bStrategiesSaved = True
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()

            'Update quotashares to include default values for new groups with hcr and remove them for groups no longer with a hcr
            Me.m_MSE.QuotaShares.ModifyWithNewDefaults()
            Me.m_MSE.QuotaShares.RemoveUnnecessaryShares()
            'Save the quotashares to csv
            Me.m_MSE.QuotaShares.Save()

        Catch ex As Exception

        End Try

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    ' -----------------------------
    ' Grid
    ' -----------------------------

    ''' <summary>
    ''' Update group selection according to user actions in the grid.
    ''' </summary>
    Private Sub OnGridSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)
        Me.HCRGroup = Me.m_grid.HarvestControlRule
        Me.UpdateControls()
    End Sub

    ''' <summary>
    ''' Update to changed grid contents.
    ''' </summary>
    Private Sub OnGridEdited()
        Try
            Me.UpdatePlot()
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub tsbDefaultTFM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

        'Try
        '    Me.UIContext.Core.SetDefaultTFM()
        'Catch ex As Exception
        '    Debug.Assert(False, ex.Message)
        'End Try
    End Sub

    ' -----------------------------
    ' HCR
    ' -----------------------------

    Private Sub OnAddHCR(sender As Object, e As System.EventArgs) Handles m_tsbnAddHCR.Click

        'Ask the user to create a new HCR_Group
        Dim HRCDialogue As dlgHarvestControlRule = New dlgHarvestControlRule
        HRCDialogue.Init(Me.m_MSE, Me.m_SelectedStrategy)
        HRCDialogue.ShowDialog()

        If HRCDialogue.DialogResult = Windows.Forms.DialogResult.OK Then
            'add the newly created harvest control rule to the current strategy
            Me.m_SelectedStrategy.Add(HRCDialogue.HarvestControlRule)
            Me.m_grid.RefreshContent()

            Me.m_bStrategiesSaved = False
        End If

        Me.UpdateControls()

    End Sub

    Private Sub OnDeleteHCR(sender As System.Object, e As System.EventArgs) Handles m_tsbnDeleteHCR.Click
        Dim selHCRIndex As Integer = Me.m_grid.SelectedRow
        Dim curStratIndex As Integer = Me.m_tscmStrategies.SelectedIndex

        If selHCRIndex > 0 Then
            If Me.m_SelectedStrategy IsNot Nothing Then
                'ToDo Like the Deleted Strategy this should be handled by the Strategy object
                'that way there can be an isDirty flag
                Me.m_SelectedStrategy.RemoveAt(selHCRIndex - 1)
                If curStratIndex > -1 And curStratIndex < Me.m_tscmStrategies.Items.Count Then
                    Me.m_tscmStrategies.SelectedIndex = curStratIndex
                End If

                Me.m_grid.RefreshContent()
                Me.HCRGroup = Me.m_grid.HarvestControlRule
                Me.m_bStrategiesSaved = False
                Me.UpdateControls()

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
            Me.UpdatePlot()
        End Set
    End Property

    Private ReadOnly Property MSE As cMSE
        Get
            Return Me.m_MSE
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Redraw the quota curve.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub UpdatePlot()

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

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bHasStrategy As Boolean = (Me.m_tscmStrategies.SelectedIndex >= 0)
        Dim bHasHCR As Boolean = (Me.m_grid.SelectedStrategy IsNot Nothing)

        Me.m_tsbnDeleteStrategy.Enabled = bHasStrategy
        Me.m_tsbnAddHCR.Enabled = bHasStrategy
        Me.m_tsbnDeleteHCR.Enabled = bHasHCR

        Me.m_btnSave.Enabled = Me.m_bStrategiesSaved

    End Sub

    Public Sub UpdateStrategies()
        Me.m_tscmStrategies.Items.Clear()
        For Each strategy As Strategy In Me.m_strategies
            Me.m_tscmStrategies.Items.Add(strategy.Name)
        Next
    End Sub

    Private Property SelectedStrategy As Integer
        Get
            Return Me.m_tscmStrategies.SelectedIndex
        End Get
        Set(value As Integer)
            Dim strat As Strategy = Nothing
            If (0 <= value And value < Me.m_strategies.Count) Then
                strat = Me.m_strategies(value)
                Me.m_tscmStrategies.SelectedIndex = value
            Else
                Me.m_tscmStrategies.SelectedIndex = -1
            End If

            Me.m_SelectedStrategy = strat
            Me.m_grid.SelectedStrategy = strat
            Me.m_gridRegulations.SelectedStrategy = strat
            Me.UpdatePlot()
        End Set
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
                    Me.m_HCR.LowerLimit = Math.Max(0, Math.Min(Units.Convert(eConvertTypes.ToEcopathBio, dX), Me.m_HCR.UpperLimit))
                Case eDragType.BUpperFMax
                    Me.m_HCR.UpperLimit = Math.Max(Me.m_HCR.LowerLimit, Units.Convert(eConvertTypes.ToEcopathBio, dX))
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
                Case eDragType.FMax
                    Me.m_HCR.MaxF = Math.Max(0, CSng(dy))
            End Select
            Me.UpdatePlot()
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


