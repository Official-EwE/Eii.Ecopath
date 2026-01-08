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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.Common
Imports ZedGraph
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region

Public Class frmMSEPlots

    'ToDo_jb 12-Jan-2010 frmMSEPlots Show Hide button should be disabled when Fleet data is selected

    Private m_MSE As cMSEManager
    Private m_paneMaster As MasterPane = Nothing
    Private m_plotter As cMSEPlotter
    Private m_MSEEvents As cMSEEventSource
    Private m_curPlotType As ePlotTypes
    Private m_curPlotData As ePlotData
    Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of frmMSEPlots)()

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Debug.Assert(Me.UIContext IsNot Nothing)

        Me.m_MSE = Me.UIContext.Core.MSEManager
        Me.m_plotter = New cMSEPlotter()
        Me.m_plotter.Init(Me.UIContext, Me.m_MSE, Me.m_graph)

        Me.m_MSEEvents = New cMSEEventSource
        AddHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        AddHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        ' Display Groups
        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cShowHideItemsCommand.COMMAND_NAME)
        If cmd IsNot Nothing Then
            AddHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups
        End If

        Me.m_rbHisto.Tag = ePlotTypes.Histogram
        Me.m_rbValues.Tag = ePlotTypes.Values

        Me.m_rbGroupBiomass.Tag = ePlotData.Biomass
        Me.m_rbGroupCatch.Tag = ePlotData.GroupCatch
        Me.m_rbFleetValue.Tag = ePlotData.FleetValue
        Me.m_rbEffort.Tag = ePlotData.Effort
        Me.m_rbBioEst.Tag = ePlotData.BioEst
        Me.m_rbTotFleetValue.Tag = ePlotData.FleetTotValue
        Me.m_rbFComparison.Tag = ePlotData.FishingMortalityComparison

        Me.m_curPlotData = ePlotData.Biomass
        Me.m_curPlotType = ePlotTypes.Histogram


        Try
            Me.DrawPlots()
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".Load() Exception: " & ex.Message)
        End Try

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_plotter.Detach()

        RemoveHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        RemoveHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        ' Show/Hide Groups
        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cShowHideItemsCommand.COMMAND_NAME)
        If cmd IsNot Nothing Then
            RemoveHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups
        End If

        MyBase.OnFormClosed(e)

    End Sub

    Private Sub onRefLevelsChanged()
        Me.m_plotter.AddReference()
    End Sub

    Private Sub onRunCompleted()
        Me.DrawPlots()
    End Sub

    Private Sub PlotGroupData(lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), _
                              PlotType As ePlotTypes, DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try
            'set the plot type and data before adding the data to the plotter
            'DataType is needed by isGroupVisible()
            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType

            For Each stat As cMSEStats In lstStatObjects
                If Me.m_plotter.IsGroupVisible(stat.Index) Then
                    data.Add(stat)
                End If
            Next

            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".PlotGroupData() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub PlotFleetData(lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), _
                              PlotType As ePlotTypes, DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try
            For Each stat As cMSEStats In lstStatObjects
                If Me.UIContext.StyleGuide.FleetVisible(stat.Index) Then
                    data.Add(stat)
                End If
            Next

            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType
            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".PlotFleetData() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub PlotFleetTotValData(TotFleetValue As cMSEStats, PlotType As ePlotTypes, DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try
            data.Add(TotFleetValue)

            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType
            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".PlotFleetData() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub DrawPlots()

        Select Case Me.m_curPlotData
            Case ePlotData.Biomass
                Me.PlotGroupData(Me.m_MSE.BiomassStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.BioEst
                Me.PlotGroupData(Me.m_MSE.BioEstimatesStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.GroupCatch
                Me.PlotGroupData(Me.m_MSE.GroupCatchStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.FleetValue
                Me.PlotFleetData(Me.m_MSE.FleetStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.Effort
                Me.PlotFleetData(Me.m_MSE.EffortStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.FleetTotValue
                Me.PlotFleetTotValData(Me.m_MSE.TotalFleetValueStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.FishingMortalityComparison
                Me.PlotGroupData(Me.m_MSE.FCompareStats, Me.m_curPlotType, Me.m_curPlotData)
        End Select

    End Sub

    Private Sub onDataTypeCheckedChanged(sender As Object, e As System.EventArgs) _
                Handles m_rbGroupBiomass.CheckedChanged, m_rbGroupCatch.CheckedChanged,
                        m_rbFleetValue.CheckedChanged, m_rbEffort.CheckedChanged, m_rbBioEst.CheckedChanged,
                        m_rbTotFleetValue.CheckedChanged, m_rbFComparison.CheckedChanged

        ' Ignore creation-time events
        If (Me.UIContext Is Nothing) Then Return

        Try

            If DirectCast(sender, RadioButton).Checked Then
                Dim tag As Object = DirectCast(sender, RadioButton).Tag
                If tag Is Nothing Then
                    Debug.Assert(False, "Radio button does not have a tag")
                    Return
                End If
                Me.m_curPlotData = DirectCast(tag, ePlotData)
                Me.UpdateControls()
                Me.Cursor = Cursors.WaitCursor
                Me.DrawPlots()
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onPlotTypesSelectedIndexChanged() Exception: " & ex.Message)
            m_logger.LogError(ex, "frmMSEPlots.onDataTypeCheckedChanged Exception")
        End Try

        Me.Cursor = Cursors.Default

    End Sub


    Private Sub onPlotTypeCheckedChanged(sender As Object, e As System.EventArgs) Handles m_rbHisto.CheckedChanged, m_rbValues.CheckedChanged

        Try
            If DirectCast(sender, RadioButton).Checked Then
                Dim tag As Object = DirectCast(sender, RadioButton).Tag
                If tag Is Nothing Then Exit Sub
                Me.m_curPlotType = DirectCast(tag, ePlotTypes)
                Me.Cursor = Cursors.WaitCursor
                Me.m_plotter.PlotType = Me.m_curPlotType
                Me.DrawPlots()
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onPlotTypesSelectedIndexChanged() Exception: " & ex.Message)
            m_logger.LogError(ex, "frmMSEPlots.onPlotTypeCheckedChanged Exception")
        End Try
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub OnShowHideGroups(cmd As cCommand)

        Me.m_plotter.Clear()
        Me.DrawPlots()

    End Sub

#Region "Core interactions"

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        Try
            Me.m_MSEEvents.HandleCoreMessage(msg)
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class

