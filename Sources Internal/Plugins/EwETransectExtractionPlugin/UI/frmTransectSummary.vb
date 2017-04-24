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
Imports ScientificInterfaceShared.Controls
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

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.UIContext, Me.m_graph, 4)
        Me.m_zgh.ConfigurePane("Depth", "Cell", "Depth", False, iPane:=1)
        Me.m_zgh.ConfigurePane("MPA", "Cell", "MPA", False, iPane:=2)
        Me.m_zgh.ConfigurePane("Biomass at T", "Cell", "Biomass", False, iPane:=3)
        Me.m_zgh.ConfigurePane("Catch at T", "Cell", "Catch", False, iPane:=4)

        For i As Integer = 1 To 4
            AddHandler Me.m_zgh.GetPane(i).XAxis.ScaleFormatEvent, AddressOf OnFormatXScale
        Next

        Me.m_tsbnPlay.Image = SharedResources.PlayHS
        Me.m_tsbnStop.Image = SharedResources.StopHS

        Me.FillTransectBox()
        Me.UpdateControls()

    End Sub

    Private Function OnFormatXScale(pane As GraphPane, axis As Axis, val As Double, index As Integer) As String
        Dim t As cTransect = DirectCast(Me.m_tscmbTransect.SelectedItem, cTransect)
        If (t IsNot Nothing) Then
            If (index = 0) Then Return "top left"
            If (index = t.NumCells - 1) Then Return "bottom right"
        End If
        Return ""
    End Function

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        Me.m_zgh.Detach()
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If (Me.UIContext Is Nothing) Then Return

        Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
        Dim bHasResults As Boolean = Not sm.IsBusy And sm.HasEcospaceRan

        Me.m_tsbnPlay.Enabled = bHasResults
        Me.m_tsbnStop.Enabled = Me.m_timerPlay.Enabled

    End Sub

#End Region ' Overrides 

#Region " Control events "

    Private Sub OnSelectTransect(sender As Object, e As EventArgs) Handles m_tscmbTransect.SelectedIndexChanged

        Try
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub m_data_OnTransectAdded(sender As cTransectDatastructures, transect As cTransect) Handles m_data.OnTransectAdded
        Me.m_tscmbTransect.Items.Add(transect)
    End Sub

    Private Sub m_data_OnTransectRemoved(sender As cTransectDatastructures, transect As cTransect) Handles m_data.OnTransectRemoved
        Me.m_tscmbTransect.Items.Remove(transect)
    End Sub

    Private Sub m_data_OnTransectChanged(sender As cTransectDatastructures, transect As cTransect) _
        Handles m_data.OnTransectChanged
        Try
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnPlay(sender As Object, e As EventArgs) Handles m_tsbnPlay.Click
        Me.m_timerPlay.Enabled = True
    End Sub

    Private Sub OnStop(sender As Object, e As EventArgs) Handles m_tsbnStop.Click
        Me.m_timerPlay.Enabled = False
    End Sub

    Private Sub OnTick(sender As Object, e As EventArgs) Handles m_timerPlay.Tick
        Me.UpdateGraph()
        Me.m_tick += 1
        If (Me.m_tick > Core.nEcospaceTimeSteps) Then Me.m_tick = 1
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
                    For i As Integer = 0 To t.NumCells - 1
                        Dim pt As Point = cells(i)
                        Dim sVal As Single = s.Value(i)
                        If ((bm.IsModelledCell(pt.Y, pt.X)) Or (var = eVarNameFlags.LayerDepth)) And (sVal >= 0) Then
                            ppl.Add(i, sVal)
                        End If
                    Next
                    Dim li As LineItem = Me.m_zgh.CreateLineItem(s.Name, ScientificInterfaceShared.Definitions.eSketchDrawModeTypes.Line, Color.Black, ppl)
                    gp.CurveList.Add(li)
                Next

                With gp.XAxis.Scale
                    .Min = 0
                    .MinAuto = False
                    .MinGrace = 0
                    .Max = t.NumCells
                    .MaxAuto = False
                    .MaxGrace = 0
                    .MinorStep = 0
                    .MajorStep = 1
                    .MajorStepAuto = False
                End With
            Catch ex As Exception

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
                    If (s IsNot Nothing) Then
                        For i As Integer = 0 To t.NumCells - 1
                            Dim pt As Point = cells(i)
                            Dim sVal As Single = s.Value(i)
                            If (bm.IsModelledCell(pt.Y, pt.X) And (sVal >= 0)) Then
                                ppl.Add(i, sVal)
                            End If
                        Next
                    End If
                    Dim li As LineItem = Me.m_zgh.CreateLineItem(Me.Core.EcoPathGroupInputs(iGroup), ppl)
                    gp.CurveList.Add(li)
                Next

                With gp.XAxis.Scale
                    .Min = 0
                    .MinAuto = False
                    .MinGrace = 0
                    .Max = t.NumCells
                    .MaxAuto = False
                    .MaxGrace = 0
                    .MinorStep = 0
                    .MajorStep = 1
                    .MajorStepAuto = False
                End With
            Catch ex As Exception

            End Try
            Me.m_zgh.RescaleAndRedraw()
        End If
    End Sub

#End Region ' Internals

End Class