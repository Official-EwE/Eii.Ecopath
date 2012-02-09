Option Strict On
Imports System.Reflection
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.SystemUtilities

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class; maintains content of the status strip panes in the AppLauncher.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cEwEStatusBar

    ''' <summary>The ui context to use.</summary>
    Private m_uic As cUIContext = Nothing

    ''' <summary>The core state monitor offering events to observe.</summary>
    Private WithEvents m_csm As cCoreStateMonitor = Nothing

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        ' Load
        Me.InitializeComponent()
        ' At runtime set visible state of controls
        If (Not Me.DesignMode) Then
            ' Hide all items at startup
            For Each item As ToolStripItem In Me.Items
                item.Visible = False
            Next
            ' .. except for springy status label, which needs to push the model and scenario controls to the right
            Me.m_tsStatus.Visible = True
            ' Configure host IP
            If My.Settings.ShowHostInfo Then
                Try
                    Me.m_tsIP.Text = String.Format(SharedResources.GENERIC_LABEL_DOUBLE, cSystemUtils.GetHostName(), cSystemUtils.GetHostIP())
                Catch ex As Exception
                    '  Hmm
                End Try
                Me.m_tsIP.Visible = True
            End If
        End If
    End Sub

    Public Sub Attach(ByVal uic As cUIContext)

        Dim an As AssemblyName = Assembly.GetExecutingAssembly().GetName()

        Me.m_uic = uic
        Me.m_csm = Me.m_uic.Core.StateMonitor

    End Sub

    Public Sub Detach()
        Me.m_csm = Nothing
    End Sub

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Core state monitor data changed event handler; handled to update the
    ''' content of the status panes.
    ''' </summary>
    ''' <remarks>
    ''' Refer to <see cref="cCoreStateMonitor.CoreDataStateEvent">data change event</see>
    ''' for a detailed description of this event.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub m_csm_CoreDataStateEvent(ByVal csm As EwECore.cCoreStateMonitor) Handles m_csm.CoreDataStateEvent
        Me.UpdateModelPanes()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Core state monitor execution state changed event handler; handled to 
    ''' update the content of the status panes.
    ''' </summary>
    ''' <remarks>
    ''' Refer to <see cref="cCoreStateMonitor.CoreDataStateEvent">data change event</see>
    ''' for a detailed description of this event.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub m_csm_CoreExecutionStateEvent(ByVal csm As cCoreStateMonitor) _
        Handles m_csm.CoreExecutionStateEvent
        Me.UpdateModelPanes()
    End Sub

    Private Sub OnStopRun(sender As Object, e As System.EventArgs) Handles m_tslStop.Click
        Try
            Me.m_uic.Core.StopRun()
        Catch ex As Exception
        End Try
    End Sub

#End Region ' Events

#Region " Pane content handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Bluntly updates the content of all status strip panes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateModelPanes()

        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim eweModel As cEwEModel = Me.m_uic.Core.EwEModel
        Dim simScenario As cEcoSimScenario = Nothing
        Dim tsds As cTimeSeriesDataset = Nothing
        Dim spaceScenario As cEcospaceScenario = Nothing
        Dim tracerScenario As cEcotracerScenario = Nothing
        Dim strName As String = ""
        Dim strTooltip As String = ""

        ' Is Ecopath model loaded?
        If Me.m_csm.IsExecutionStateSuperceded(eCoreExecutionState.EcopathLoaded) Then
            ' #Yes: set content for status panes

            ' ----------------------
            ' Datasource and Ecopath
            ' ----------------------
            strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOPATH_TOOLTIP, _
                                       eweModel.Name, _
                                       appl.SelectedFileName)
            Me.UpdateToolstripItem(Me.m_tsEcopathModel, eweModel.Name, strTooltip)

            ' -------
            ' Ecosim
            ' -------
            If Me.m_uic.Core.ActiveEcosimScenarioIndex >= 0 Then
                simScenario = Me.m_uic.Core.EcosimScenarios(Me.m_uic.Core.ActiveEcosimScenarioIndex)

                If Me.m_uic.Core.ActiveTimeSeriesDatasetIndex > 0 Then
                    tsds = Me.m_uic.Core.TimeSeriesDataset(Me.m_uic.Core.ActiveTimeSeriesDatasetIndex)
                    strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSIM_TOOLTIP, _
                                               simScenario.Name, _
                                               tsds.Name, _
                                               Me.ToTooltipLabel(simScenario.Description))
                    strName = String.Format(SharedResources.GENERIC_LABEL_DETAILED, simScenario.Name, tsds.Name)
                Else
                    strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSIM_TOOLTIP, _
                                               simScenario.Name, _
                                               Me.ToTooltipLabel(""), _
                                               Me.ToTooltipLabel(simScenario.Description))
                    strName = simScenario.Name
                End If
                Me.UpdateToolstripItem(Me.m_tsEcosimScenario, strName, strTooltip)
            Else
                Me.UpdateToolstripItem(Me.m_tsEcosimScenario)
            End If

            ' -------
            ' Ecospace
            ' -------
            If (Me.m_uic.Core.ActiveEcospaceScenarioIndex >= 0) Then
                spaceScenario = Me.m_uic.Core.EcospaceScenarios(Me.m_uic.Core.ActiveEcospaceScenarioIndex)
                strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSPACE_TOOLTIP, _
                                           spaceScenario.Name, _
                                           Me.ToTooltipLabel(spaceScenario.Description))
                Me.UpdateToolstripItem(Me.m_tsEcospaceScenario, spaceScenario.Name, strTooltip)
            Else
                Me.UpdateToolstripItem(Me.m_tsEcospaceScenario)
            End If

            ' -------
            ' Ecotracer
            ' -------
            If (Me.m_uic.Core.ActiveEcotracerScenarioIndex >= 0) Then
                tracerScenario = Me.m_uic.Core.EcotracerScenarios(Me.m_uic.Core.ActiveEcotracerScenarioIndex)
                strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOTRACER_TOOLTIP, _
                                           vbNewLine, _
                                           tracerScenario.Name, _
                                           Me.ToTooltipLabel(tracerScenario.Description))
                Me.UpdateToolstripItem(Me.m_tsEcotracerScenario, tracerScenario.Name, strTooltip)
            Else
                Me.UpdateToolstripItem(Me.m_tsEcotracerScenario)
            End If

        Else
            ' #No: clear all status panes
            Me.UpdateToolstripItem(Me.m_tsEcopathModel)
            Me.UpdateToolstripItem(Me.m_tsEcosimScenario)
            Me.UpdateToolstripItem(Me.m_tsEcospaceScenario)
            Me.UpdateToolstripItem(Me.m_tsEcotracerScenario)
        End If

    End Sub

    Private Function ToTooltipLabel(ByVal str As String) As String
        If String.IsNullOrEmpty(str) Then Return SharedResources.GENERIC_VALUE_NONE
        Return str
    End Function

    Private Function ToTooltipNameLabel(ByVal strName As String, ByVal bModified As Boolean) As String
        If bModified Then Return String.Format(SharedResources.GENERIC_LABEL_DETAILED, strName, My.Resources.STATUSSTRIP_MODIFIED)
        Return strName
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the content of a single tool strip item.
    ''' </summary>
    ''' <param name="tsi">The item to update.</param>
    ''' <param name="strText">Text to assign to the item. If no text is provided the
    ''' item will not be displayed.</param>
    ''' <param name="strTooltipText">Tooltip text to assign to the item. If this value
    ''' is an empty string no tooltip will appear.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateToolstripItem(ByVal tsi As ToolStripItem, _
            Optional ByVal strText As String = "", _
            Optional ByVal strTooltipText As String = "")

        ' Abort if something went wrong
        If tsi Is Nothing Then Return

        ' Configure the item that was found
        With tsi
            .Text = strText
            .ToolTipText = strTooltipText
            ' Hide item if item has no text
            .Visible = (Not String.IsNullOrEmpty(strText))
        End With
    End Sub

    Private m_strLastStatusText As String = ""
    Private m_iLastProgress As Integer = 0

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Set the text of the main status strip item.
    ''' </summary>
    ''' <param name="strText">The text to set.</param>
    ''' <param name="sProgress">Progress ([0, 1] or -1 )to set in a continuous progress bar,
    ''' 0.0 to hide progress bar, or -1 to show a marquee progress bar.</param>
    ''' -------------------------------------------------------------------
    Public Sub SetStatusText(ByVal strText As String, Optional ByVal sProgress As Single = 0.0)

        If (Me.m_tsStatus Is Nothing) Then Return

        Dim iProgress As Integer = Math.Max(Math.Min(100, CInt(CInt(sProgress * 25) * 4)), 0)

        ' Optimization
        If (String.Compare(Me.m_strLastStatusText, strText) = 0) And _
           (iProgress = Me.m_iLastProgress) Then
            Return
        End If

        Me.m_strLastStatusText = strText
        Me.m_iLastProgress = iProgress

        ' Update
        Me.m_tsStatus.Text = strText
        Select Case sProgress
            Case 0
                Me.m_tsbProgress.Visible = False
                Me.m_tslStop.Visible = False
            Case -1
                Me.m_tsbProgress.Style = ProgressBarStyle.Marquee
                Me.m_tsbProgress.Visible = True
                Me.m_tslStop.Visible = Me.m_uic.Core.CanStopRun
            Case Else
                Me.m_tsbProgress.Style = ProgressBarStyle.Continuous
                Me.m_tsbProgress.Visible = True
                Me.m_tsbProgress.Value = CInt(Math.Max(Math.Min(100, sProgress * 100), 0))
                Me.m_tslStop.Visible = Me.m_uic.Core.CanStopRun
        End Select

        ' Redraw status bar immediately
        '   This is a known performace killer (issue #937)
        Me.Refresh()

    End Sub

#End Region ' Pane content handling

End Class
