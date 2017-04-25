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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities.cSystemUtils
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Definitions
Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmTransectSummary

    Private WithEvents m_data As cTransectDatastructures = Nothing
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_tick As Integer = 1

    Public Sub New(uic As cUIContext)
        Me.InitializeComponent()
        Me.UIContext = uic
        Me.m_data = cTransectDatastructures.Instance(uic.Core)
        Me.Text = My.Resources.CAPTION_OUT
        Me.TabText = Me.Text
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return

        ' Make pretty
        Me.m_tsbnPlay.Image = SharedResources.PlayHS
        Me.m_tsbnStop.Image = SharedResources.StopHS

        ' ToDo: globalize this, include units, etc
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph, 4)
        Me.m_zgh.ConfigurePane("Depth", "Cell", "Depth", False, iPane:=1)
        Me.m_zgh.ConfigurePane("MPA", "Cell", "MPA", False, iPane:=2)
        Me.m_zgh.ConfigurePane("Biomass", "Cell", "Biomass", False, iPane:=3)
        Me.m_zgh.ConfigurePane("Catch", "Cell", "Catch", False, iPane:=4)

        For i As Integer = 1 To 4
            AddHandler Me.m_zgh.GetPane(i).XAxis.ScaleFormatEvent, AddressOf OnFormatXScale
        Next

        AddHandler Me.Core.StateMonitor.CoreExecutionStateEvent, AddressOf OnCoreExecutionStateChanged
        Me.FillTransectBox()
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        Me.m_timerPlay.Enabled = False
        Me.m_zgh.Detach()
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If (Me.UIContext Is Nothing) Then Return

        Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
        Dim bHasResults As Boolean = Not sm.IsBusy And sm.HasEcospaceRan
        Dim bIsPlaying As Boolean = Me.m_timerPlay.Enabled

        Me.m_tsbnPlay.Enabled = bHasResults And Not bIsPlaying
        Me.m_tsbnStop.Enabled = bIsPlaying
        Me.m_tsbnSaveToCSV.Enabled = bHasResults

    End Sub

#End Region ' Overrides 

#Region " Control events "

    Private Sub OnSelectTransect(sender As Object, e As EventArgs) _
        Handles m_tscmbTransect.SelectedIndexChanged
        Try
            Me.UpdateGraph()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnTransectAdded(sender As cTransectDatastructures, transect As cTransect) _
        Handles m_data.OnTransectAdded
        Try
            Me.m_tscmbTransect.Items.Add(transect)
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnTransectRemoved(sender As cTransectDatastructures, transect As cTransect) _
        Handles m_data.OnTransectRemoved
        Try
            Me.m_tscmbTransect.Items.Remove(transect)
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnTransectChanged(sender As cTransectDatastructures, transect As cTransect) _
        Handles m_data.OnTransectChanged
        Try
            Me.UpdateGraph()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Function OnFormatXScale(pane As GraphPane, axis As Axis, val As Double, index As Integer) As String
        Dim t As cTransect = DirectCast(Me.m_tscmbTransect.SelectedItem, cTransect)
        If (t IsNot Nothing) Then
            Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
            Dim cells As Point() = t.Cells(bm)
            If (cells.Count > 0) And ((index = 0) Or (index = t.NumCells - 1)) Then
                Dim pt As Point = cells(index)
                ' ToDo: globalize this
                Return cStringUtils.Localize("({0}, {1})", pt.X, pt.Y)
            End If
        End If
        Return ""
    End Function

    Private Sub OnCoreExecutionStateChanged(statemonitor As cCoreStateMonitor)
        Try
            Me.m_timerPlay.Enabled = False
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnPlay(sender As Object, e As EventArgs) Handles m_tsbnPlay.Click
        Try
            Me.m_timerPlay.Enabled = True
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnStop(sender As Object, e As EventArgs) Handles m_tsbnStop.Click
        Try
            Me.m_timerPlay.Enabled = False
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnTick(sender As Object, e As EventArgs) Handles m_timerPlay.Tick
        Try
            Me.m_tick += 1
            If (Me.m_tick > Core.nEcospaceTimeSteps) Then Me.m_tick = 1
            Me.UpdateGraph()
            Me.UpdateControls()
        Catch ex As Exception
            ' NOP
        End Try
    End Sub

    Private Sub OnSaveTransectsToCSV(sender As Object, e As EventArgs) _
        Handles m_tsbnSaveToCSV.Click

        ' We can take these humongous shortcuts here because we have inside information ;)
        Dim w As New cTransectResultWriterPlugin()
        w.Init(Me.Core)
        w.EndWrite()

    End Sub

#End Region ' Control events

#Region " Internals "

    Private Sub FillTransectBox()
        Me.m_tscmbTransect.Items.Clear()
        For Each t As cTransect In Me.m_data.Transects
            Me.m_tscmbTransect.Items.Add(t)
        Next
    End Sub

    Private Sub UpdateGraph(Optional bOutOnly As Boolean = False)

        Dim t As cTransect = DirectCast(Me.m_tscmbTransect.SelectedItem, cTransect)

        If (Not bOutOnly) Then
            Me.FillInputPane(1, t, eVarNameFlags.LayerDepth)
            Me.FillInputPane(2, t, eVarNameFlags.LayerMPA)
        End If

        Me.FillOutputPane(3, t, cTransect.eSummaryType.Biomass)
        Me.FillOutputPane(4, t, cTransect.eSummaryType.Catch)

        Me.m_zgh.RescaleAndRedraw()

    End Sub

    ' ToDo: fix redundancy between FillInputPane and FillOutputPane

    Private Sub FillInputPane(iPane As Integer, t As cTransect, var As eVarNameFlags)

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim gp As GraphPane = Me.m_zgh.GetPane(iPane)

        gp.CurveList.Clear()

        If (t IsNot Nothing) Then

            Dim cells As Point() = t.Cells(Me.Core.EcospaceBasemap)

            Try
                For Each l As cEcospaceLayer In bm.Layers(var)

                    Dim s As cTransectSummary = t.Summary(bm, l, -1)
                    Dim ppl As New PointPairList()
                    Dim bIsMissing As Boolean = False

                    For i As Integer = 0 To t.NumCells - 1

                        Dim pt As Point = cells(i)
                        Dim sVal As Single = s.Value(i)

                        If ((bm.IsModelledCell(pt.Y, pt.X)) Or (var = eVarNameFlags.LayerDepth)) And (sVal >= 0) Then
                            If bIsMissing Then
                                ppl.Add(i, 0)
                                bIsMissing = False
                            End If
                            ppl.Add(i, sVal)
                            ppl.Add(i + 1, sVal)
                        Else
                            If Not bIsMissing Then
                                ppl.Add(i, 0)
                                bIsMissing = True
                            End If
                            ppl.Add(i, PointPair.Missing)
                            ppl.Add(i + 1, PointPair.Missing)
                        End If
                    Next

                    gp.CurveList.Add(Me.m_zgh.CreateLineItem(s.Name, eSketchDrawModeTypes.Line, Color.Black, ppl))
                Next

                With gp.XAxis.Scale
                    .Min = 0
                    .MinAuto = False
                    .MinGrace = 0
                    .Max = t.NumCells - 1
                    .MaxAuto = False
                    .MaxGrace = 0
                    .MinorStep = 0
                    .MajorStep = 1
                    .MajorStepAuto = False
                End With

            Catch ex As Exception
                ' NOP
            End Try
            Me.m_zgh.RescaleAndRedraw()
        End If

    End Sub

    Private Sub FillOutputPane(iPane As Integer, t As cTransect, var As cTransect.eSummaryType)

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim gp As GraphPane = Me.m_zgh.GetPane(iPane)

        gp.CurveList.Clear()

        If (t IsNot Nothing) Then

            Dim cells As Point() = t.Cells(Me.Core.EcospaceBasemap)

            Try
                For iGroup As Integer = 1 To Me.Core.nGroups

                    Dim s As cTransectSummary = t.Summary(Me.m_tick, iGroup, var)
                    Dim ppl As New PointPairList()
                    Dim bIsMissing As Boolean = False

                    If (s IsNot Nothing) Then
                        For i As Integer = 0 To t.NumCells - 1

                            Dim pt As Point = cells(i)
                            Dim sVal As Single = s.Value(i)

                            If (bm.IsModelledCell(pt.Y, pt.X) And (sVal >= 0)) Then
                                If bIsMissing Then
                                    ppl.Add(i, 0)
                                    bIsMissing = False
                                End If
                                ppl.Add(i, sVal)
                                ppl.Add(i + 1, sVal)
                            Else
                                If Not bIsMissing Then
                                    ppl.Add(i, 0)
                                    bIsMissing = True
                                End If
                                ppl.Add(i, PointPair.Missing)
                                ppl.Add(i + 1, PointPair.Missing)
                            End If

                        Next
                    End If

                    gp.CurveList.Add(Me.m_zgh.CreateLineItem(Me.Core.EcoPathGroupInputs(iGroup), ppl))
                    ' ToDo: globalize this
                    gp.Title.Text = cStringUtils.Localize("{0} at timestep {1}/{2}", IIF(var = cTransect.eSummaryType.Biomass, "Biomass", "Catch"), Me.m_tick, Me.Core.nEcospaceTimeSteps)

                Next

                With gp.XAxis.Scale
                    .Min = 0
                    .MinAuto = False
                    .MinGrace = 0
                    .Max = t.NumCells - 1
                    .MaxAuto = False
                    .MaxGrace = 0
                    .MinorStep = 0
                    .MajorStep = 1
                    .MajorStepAuto = False
                End With

            Catch ex As Exception
                ' NOP
            End Try
            Me.m_zgh.RescaleAndRedraw()

        End If

    End Sub

#End Region ' Internals

End Class