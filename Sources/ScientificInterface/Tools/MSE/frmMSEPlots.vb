
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwECore.SearchObjectives
Imports ScientificInterface.Controls
Imports EwEUtils.Core
Imports ScientificInterface.Ecosim
Imports EwEUtils.Commands

Imports ZedGraph

#End Region

Public Class frmMSEPlots

    'ToDo_jb 12-Jan-2010 frmMSEPlots Show Hide button should be disabled when Fleet data is selected

    Dim m_core As cCore
    Dim m_MSE As cMSEManager

    Private m_paneMaster As MasterPane = Nothing
    Private m_sg As cStyleGuide = Nothing
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_plotter As cMSEPlotter
    Private m_MSEEvents As cMSEEventSource
    Private m_curPlotType As ePlotTypes
    Private m_curPlotData As ePlotData

    Private Sub frmMSEPlots_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        RemoveHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        RemoveHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        ' Show/Hide Groups
        Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
        Dim cmd As cCommand = cmdh.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.RemoveControl(Me.btShowHideGroups)
        End If

        RemoveHandler cmd.OnInvoke, AddressOf Me.OnShowHideGroups

    End Sub

    Private Sub frmMSEPlots_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.m_core = cCore.GetInstance()
        Me.m_MSE = Me.m_core.MSEManager
        m_sg = cStyleGuide.GetInstance

        m_zgh = New cZedGraphHelper
        Me.m_plotter = New cMSEPlotter
        Me.m_plotter.Init(Me.m_core, Me.m_MSE, Me.m_zgh, Me.ZedGraph, Me.m_sg)

        m_MSEEvents = New cMSEEventSource
        AddHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        AddHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        ' Display Groups
        Dim cmd As cCommand = cCommandHandler.GetInstance().GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.AddControl(Me.btShowHideGroups)
        End If

        AddHandler cmd.OnInvoke, AddressOf Me.OnShowHideGroups


        Me.rbHisto.Tag = ePlotTypes.Histogram
        Me.rbValues.Tag = ePlotTypes.Values

        Me.rbGroupBiomass.Tag = ePlotData.Biomass
        Me.rbGroupCatch.Tag = ePlotData.GroupCatch
        Me.rbFleetValue.Tag = ePlotData.FleetValue
        Me.rbEffort.Tag = ePlotData.Effort

        Me.m_curPlotData = ePlotData.Biomass
        Me.m_curPlotType = ePlotTypes.Histogram

        Try
            Me.DrawPlots()
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".Load() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub onRefLevelsChanged()

        Me.m_plotter.AddReference()

    End Sub


    Private Sub onRunCompleted()

        Me.DrawPlots()

    End Sub

    Private Sub PlotGroupData(ByVal lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), ByVal PlotType As ePlotTypes, ByVal DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try

            For Each stat As cMSEStats In lstStatObjects
                If Me.m_sg.GroupVisible(stat.Index) Then
                    data.Add(stat)
                End If
            Next

            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType
            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".AddGroupDataToPlots() Exception: " & ex.Message)
        End Try

    End Sub


    Private Sub PlotFleetData(ByVal lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), ByVal PlotType As ePlotTypes, ByVal DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try
            'There is no fleet filtering 
            'all the fleet data gets added to the plots
            For Each stat As cMSEStats In lstStatObjects
                data.Add(stat)
            Next

            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType
            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".AddGroupDataToPlots() Exception: " & ex.Message)
        End Try

    End Sub

    Private Sub DrawPlots()

        Select Case Me.m_curPlotData
            Case ePlotData.Biomass
                PlotGroupData(Me.m_MSE.BiomassStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.GroupCatch
                PlotGroupData(Me.m_MSE.GroupCatchStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.FleetValue
                PlotFleetData(Me.m_MSE.FleetStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.Effort
                PlotFleetData(Me.m_MSE.EffortStats, Me.m_curPlotType, Me.m_curPlotData)
        End Select

    End Sub

    Private Sub onDataTypeCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles rbGroupBiomass.CheckedChanged, rbGroupCatch.CheckedChanged, rbFleetValue.CheckedChanged, rbEffort.CheckedChanged
        Try

            If DirectCast(sender, RadioButton).Checked Then
                Dim tag As Object = DirectCast(sender, RadioButton).Tag
                If tag Is Nothing Then Exit Sub
                Me.m_curPlotData = DirectCast(tag, ePlotData)
                Me.updateControls()
                Me.Cursor = Cursors.WaitCursor
                Me.DrawPlots()
            End If

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onPlotTypesSelectedIndexChanged() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try

        Me.Cursor = Cursors.Default

    End Sub


    Private Sub onPlotTypeCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbHisto.CheckedChanged, rbValues.CheckedChanged

        Try
            If DirectCast(sender, RadioButton).Checked Then
                Dim tag As Object = DirectCast(sender, RadioButton).Tag
                If tag Is Nothing Then Exit Sub
                m_curPlotType = DirectCast(tag, ePlotTypes)
                Me.Cursor = Cursors.WaitCursor
                Me.m_plotter.PlotType = Me.m_curPlotType
                Me.DrawPlots()
            End If
        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".onPlotTypesSelectedIndexChanged() Exception: " & ex.Message)
            cLog.Write(ex)
        End Try
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub OnShowHideGroups(ByVal cmd As cCommand)

        Me.m_plotter.Clear()
        Me.DrawPlots()

    End Sub

    Private Sub updateControls()
        'this does not seem to disable the showhide button
        'maybe because it belongs to a command???
        Me.btShowHideGroups.Enabled = True
        If Me.m_curPlotData = ePlotData.FleetValue Or Me.m_curPlotData = ePlotData.Effort Then
            Me.btShowHideGroups.Enabled = False
        End If
    End Sub

#Region "Core interactions"

    Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
        Try
            Me.m_MSEEvents.HandleCoreMessage(msg)
        Catch ex As Exception

        End Try
    End Sub

#End Region

End Class

