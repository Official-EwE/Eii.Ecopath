
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

    Private m_MSE As cMSEManager
    Private m_paneMaster As MasterPane = Nothing
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_plotter As cMSEPlotter
    Private m_MSEEvents As cMSEEventSource
    Private m_curPlotType As ePlotTypes
    Private m_curPlotData As ePlotData

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        MyBase.OnLoad(e)

        Debug.Assert(Me.UIContext IsNot Nothing)

        Me.m_MSE = Me.UIContext.Core.MSEManager
        Me.m_zgh = New cZedGraphHelper()
        Me.m_plotter = New cMSEPlotter()
        Me.m_plotter.Init(Me.UIContext, Me.m_MSE, Me.m_zgh, Me.ZedGraph)

        Me.m_MSEEvents = New cMSEEventSource
        AddHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        AddHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        ' Display Groups
        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.AddControl(Me.btShowHide)
        End If

        AddHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups

        Me.rbHisto.Tag = ePlotTypes.Histogram
        Me.rbValues.Tag = ePlotTypes.Values

        Me.rbGroupBiomass.Tag = ePlotData.Biomass
        Me.rbGroupCatch.Tag = ePlotData.GroupCatch
        Me.rbFleetValue.Tag = ePlotData.FleetValue
        Me.rbEffort.Tag = ePlotData.Effort
        Me.rbBioEst.Tag = ePlotData.BioEst

        Me.m_curPlotData = ePlotData.Biomass
        Me.m_curPlotType = ePlotTypes.Histogram

        Try
            Me.DrawPlots()
        Catch ex As Exception
            System.Console.WriteLine(Me.ToString & ".Load() Exception: " & ex.Message)
        End Try

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RemoveHandler Me.m_MSEEvents.onRefLevelsChanged, AddressOf Me.onRefLevelsChanged
        RemoveHandler Me.m_MSEEvents.onRunCompleted, AddressOf Me.onRunCompleted

        ' Show/Hide Groups
        Dim cmd As cCommand = Me.UIContext.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
        If Not Object.ReferenceEquals(cmd, Nothing) Then
            cmd.RemoveControl(Me.btShowHide)
        End If

        RemoveHandler cmd.OnPostInvoke, AddressOf Me.OnShowHideGroups
        MyBase.OnFormClosed(e)

    End Sub

    Private Sub onRefLevelsChanged()
        Me.m_plotter.AddReference()
    End Sub

    Private Sub onRunCompleted()
        Me.DrawPlots()
    End Sub

    Private Sub PlotGroupData(ByVal lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), _
                              ByVal PlotType As ePlotTypes, ByVal DataType As ePlotData)
        Dim data As New List(Of cCoreGroupBase)

        Try
            For Each stat As cMSEStats In lstStatObjects
                If Me.UIContext.StyleGuide.GroupVisible(stat.Index) Then
                    data.Add(stat)
                End If
            Next

            Me.m_plotter.PlotType = PlotType
            Me.m_plotter.DataType = DataType
            Me.m_plotter.AddData(data)
            Me.m_plotter.Draw()

        Catch ex As Exception
            Debug.Assert(False, Me.ToString & ".PlotGroupData() Exception: " & ex.Message)
        End Try

    End Sub


    Private Sub PlotFleetData(ByVal lstStatObjects As EwECore.cCoreInputOutputList(Of cCoreInputOutputBase), _
                              ByVal PlotType As ePlotTypes, ByVal DataType As ePlotData)
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

    Private Sub DrawPlots()

        Select Case Me.m_curPlotData
            Case ePlotData.Biomass
                PlotGroupData(Me.m_MSE.BiomassStats, Me.m_curPlotType, Me.m_curPlotData)

            Case ePlotData.BioEst
                PlotGroupData(Me.m_MSE.BioEstimatesStats, Me.m_curPlotType, Me.m_curPlotData)

            Case ePlotData.GroupCatch
                PlotGroupData(Me.m_MSE.GroupCatchStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.FleetValue
                PlotFleetData(Me.m_MSE.FleetStats, Me.m_curPlotType, Me.m_curPlotData)
            Case ePlotData.Effort
                PlotFleetData(Me.m_MSE.EffortStats, Me.m_curPlotType, Me.m_curPlotData)
        End Select

    End Sub

    Private Sub onDataTypeCheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
                Handles rbGroupBiomass.CheckedChanged, rbGroupCatch.CheckedChanged, _
                rbFleetValue.CheckedChanged, rbEffort.CheckedChanged, rbBioEst.CheckedChanged
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
        'Me.btShowHideGroups.Visible = (Me.m_curPlotData <> ePlotData.FleetValue) And _
        '                              (Me.m_curPlotData <> ePlotData.Effort)
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

