Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports System.Reflection

''' -----------------------------------------------------------------------
''' <summary>
''' Helper class; maintains content of the status strip panes in the AppLauncher.
''' </summary>
''' -----------------------------------------------------------------------
Public Class cEwEStatusBar
    Inherits StatusStrip

    ''' <summary>The core to observe.</summary>
    Private m_core As cCore = Nothing

    ''' <summary>The property selection command to listen to.</summary>
    Private WithEvents m_cmd As PropertySelectionCommand = Nothing
    ''' <summary>Selected properties.</summary>
    Private m_aprop As cProperty() = Nothing

    ''' <summary>The core state monitor offering events to observe.</summary>
    Private WithEvents m_csm As cCoreStateMonitor = Nothing
    Private WithEvents m_tsEcopathModel As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcosimScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcospaceScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcotracerScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsbProgress As System.Windows.Forms.ToolStripProgressBar
    Private WithEvents m_tslVersion As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsSelection As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsiModified As System.Windows.Forms.ToolStripStatusLabel

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor, initializes a new instance of this class.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Public Sub Connect(ByVal core As cCore)

        Dim an As AssemblyName = Assembly.GetExecutingAssembly().GetName()

        Me.m_core = core
        Me.m_cmd = CType(cCommandHandler.GetInstance().GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)
        Me.m_csm = core.StateMonitor
        Me.m_tslVersion.Text = an.Version.ToString()

        Me.UpdateSelectionPane()

    End Sub

    Public Sub Disconnect()

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
        UpdateModelPanes()
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
        UpdateModelPanes()
    End Sub

#End Region ' Events

#Region " Command handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, invoked when the <see cref="m_cmd">selection command</see>
    ''' is invoked from anywhere in the GUI.
    ''' </summary>
    ''' <param name="cmd">The <see cref="Command">Command</see> that was invoked.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnInvoke(ByVal cmd As cCommand) Handles m_cmd.OnInvoke

        Dim aprops() As cProperty = Nothing

        ' Sanity check
        If Not (cmd Is m_cmd) Then Return
        ' Get selected props
        Me.m_aprop = m_cmd.Selection()

        ' Update
        Me.UpdateSelectionPane()

    End Sub

#End Region ' Command handling 

#Region " Pane content handling "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Bluntly updates the content of all status strip panes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateModelPanes()

        Dim appl As AppLauncher = AppLauncher.GetInstance()
        Dim eweModel As cEwEModel = Me.m_core.EwEModel
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
                                       vbNewLine, _
                                       eweModel.Name, _
                                       appl.SelectedFileName)
            Me.UpdateToolstripItem(Me.m_tsEcopathModel, eweModel.Name, strTooltip)

            ' -------
            ' Ecosim
            ' -------
            If Me.m_core.ActiveEcosimScenarioIndex >= 0 Then
                simScenario = Me.m_core.EcosimScenarios(Me.m_core.ActiveEcosimScenarioIndex)

                If Me.m_core.ActiveTimeSeriesDatasetIndex > 0 Then
                    tsds = Me.m_core.TimeSeriesDataset(Me.m_core.ActiveTimeSeriesDatasetIndex)
                    strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSIM_TOOLTIP, _
                                               vbNewLine, _
                                               simScenario.Name, _
                                               tsds.Name, _
                                               Me.ToTooltipLabel(simScenario.Description))
                    strName = String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, simScenario.Name, tsds.Name)
                Else
                    strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSIM_TOOLTIP, _
                                               vbNewLine, _
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
            If (Me.m_core.ActiveEcospaceScenarioIndex >= 0) Then
                spaceScenario = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex)
                strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOSPACE_TOOLTIP, _
                                           vbNewLine, _
                                           spaceScenario.Name, _
                                           Me.ToTooltipLabel(spaceScenario.Description))
                Me.UpdateToolstripItem(Me.m_tsEcospaceScenario, spaceScenario.Name, strTooltip)
            Else
                Me.UpdateToolstripItem(Me.m_tsEcospaceScenario)
            End If

            ' -------
            ' Ecotracer
            ' -------
            If (Me.m_core.ActiveEcotracerScenarioIndex >= 0) Then
                tracerScenario = Me.m_core.EcotracerScenarios(Me.m_core.ActiveEcotracerScenarioIndex)
                strTooltip = String.Format(My.Resources.STATUSSTRIP_ECOTRACER_TOOLTIP, _
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

        ' Update modified indicator
        Me.m_tsiModified.Visible = Me.m_csm.IsModified()

    End Sub

    Private Function ToTooltipLabel(ByVal str As String) As String
        If String.IsNullOrEmpty(str) Then Return My.Resources.GENERIC_VALUE_NONE
        Return str
    End Function

    Private Function ToTooltipNameLabel(ByVal strName As String, ByVal bModified As Boolean) As String
        If bModified Then Return String.Format(My.Resources.GENERIC_LABEL_DETAILEDLABEL, strName, My.Resources.STATUSSTRIP_MODIFIED)
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
            .Height = 18
            .Text = strText
            .ToolTipText = strTooltipText
            ' Hide item if item has no text
            .Visible = (Not String.IsNullOrEmpty(strText))
        End With
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Set the text of the main status strip item.
    ''' </summary>
    ''' <param name="strText">The text to set.</param>
    ''' <param name="sProgress">Progress ([0, 1] or -1 )to set in a continuous progress bar,
    ''' 0.0 to hide progress bar, or -1 to show a marquee progress bar.</param>
    ''' -------------------------------------------------------------------
    Public Sub SetStatusText(ByVal strText As String, Optional ByVal sProgress As Single = 0.0)
        If Me.m_tsStatus Is Nothing Then Return
        Me.m_tsStatus.Text = strText

        Select Case sProgress
            Case 0
                Me.m_tsbProgress.Visible = False
            Case -1
                Me.m_tsbProgress.Style = ProgressBarStyle.Marquee
                Me.m_tsbProgress.Visible = True
            Case Else
                Me.m_tsbProgress.Style = ProgressBarStyle.Continuous
                Me.m_tsbProgress.Visible = True
                Me.m_tsbProgress.Value = CInt(Math.Max(Math.Min(100, sProgress * 100), 0))
        End Select

        ' Redraw status bar immediately
        Me.Refresh()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the state and contents of the controls in the panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateSelectionPane()

        Dim strSelection As String = ""
        Dim srcPrim As cCoreInputOutputBase = Nothing
        Dim bPrimMixed As Boolean = False
        Dim strPrim As String = ""
        Dim srcSec As cCoreInputOutputBase = Nothing
        Dim bSecMixed As Boolean = False
        Dim strSec As String = ""

        If Not Me.m_csm.HasEcopathLoaded() Then
            ' Clear selection
            strSelection = ""
            Me.m_aprop = Nothing
        Else
            ' Start with default selection
            strSelection = My.Resources.SELECTION_NONE
        End If

        ' Find all prim and sec props
        If Me.m_aprop IsNot Nothing Then

            For Each prop As cProperty In Me.m_aprop

                If (Not Object.ReferenceEquals(prop.Source, Nothing)) Then
                    If (Not Object.ReferenceEquals(srcPrim, Nothing)) Then
                        bPrimMixed = bPrimMixed Or (Not Object.ReferenceEquals(prop.Source, srcPrim))
                    End If
                    srcPrim = prop.Source
                End If

                If (Not Object.ReferenceEquals(prop.SourceSec, Nothing)) Then
                    If (Not Object.ReferenceEquals(srcSec, Nothing)) Then
                        bSecMixed = bSecMixed Or (Not Object.ReferenceEquals(prop.SourceSec, srcSec))
                    End If
                    srcSec = prop.SourceSec
                End If

            Next

            ' Assess the damage
            ' 1. No primary source selected?
            If Object.ReferenceEquals(srcPrim, Nothing) Then
                ' #Yes: were there properties?
                If Me.m_aprop.Length > 0 Then
                    ' #Yes: unable to determine content, must be derived
                    strSelection = My.Resources.SELECTION_DERIVED
                End If
            Else
                ' #No: format prim string 
                If (bPrimMixed = False) Then
                    strPrim = srcPrim.Name
                Else
                    strPrim = My.Resources.SELECTION_MULTIPLE
                End If

                ' No secundary source selected?
                If Object.ReferenceEquals(srcSec, Nothing) Then
                    strSelection = strPrim
                Else
                    ' #Yes: is this a mixed selection?
                    If (bSecMixed = False) Then
                        strSec = srcSec.Name
                    Else
                        strSec = My.Resources.SELECTION_MULTIPLE
                    End If

                    ' Format as multiple
                    strSelection = String.Format("{0} - {1}", strPrim, strSec)
                End If
            End If
        End If

        Me.UpdateToolstripItem(Me.m_tsSelection, strSelection)

    End Sub

#End Region ' Pane content handling

    Private Sub InitializeComponent()
        Me.m_tsbProgress = New System.Windows.Forms.ToolStripProgressBar
        Me.m_tsStatus = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsSelection = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tslVersion = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcopathModel = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcosimScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcospaceScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsEcotracerScenario = New System.Windows.Forms.ToolStripStatusLabel
        Me.m_tsiModified = New System.Windows.Forms.ToolStripStatusLabel
        Me.SuspendLayout()
        '
        'm_tsbProgress
        '
        Me.m_tsbProgress.Name = "m_tsbProgress"
        Me.m_tsbProgress.Step = 1
        Me.m_tsbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.m_tsbProgress.Visible = False
        '
        'm_tsStatus
        '
        Me.m_tsStatus.Name = "m_tsStatus"
        Me.m_tsStatus.Spring = True
        Me.m_tsStatus.Text = ""
        Me.m_tsStatus.Visible = True
        '
        'm_tsSelection
        '
        Me.m_tsSelection.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsSelection.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsSelection.Name = "m_tsSelection"
        Me.m_tsSelection.Visible = False
        '
        'm_tsEcopathModel
        '
        Me.m_tsEcopathModel.AutoToolTip = True
        Me.m_tsEcopathModel.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcopathModel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcopathModel.Image = Global.ScientificInterface.My.Resources.Resources.Ecopath_32x32
        Me.m_tsEcopathModel.Name = "m_tsEcopathModel"
        Me.m_tsEcopathModel.Visible = False
        '
        'm_tsEcosimScenario
        '
        Me.m_tsEcosimScenario.AutoToolTip = True
        Me.m_tsEcosimScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcosimScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcosimScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecosim_32x32
        Me.m_tsEcosimScenario.Name = "m_tsEcosimScenario"
        Me.m_tsEcosimScenario.Visible = False
        '
        'm_tsEcospaceScenario
        '
        Me.m_tsEcospaceScenario.AutoToolTip = True
        Me.m_tsEcospaceScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcospaceScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcospaceScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecospace_32x32
        Me.m_tsEcospaceScenario.Name = "m_tsEcospaceScenario"
        Me.m_tsEcospaceScenario.Visible = False
        '
        'm_tsEcotracerScenario
        '
        Me.m_tsEcotracerScenario.AutoToolTip = True
        Me.m_tsEcotracerScenario.BorderSides = CType((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.m_tsEcotracerScenario.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
        Me.m_tsEcotracerScenario.Image = Global.ScientificInterface.My.Resources.Resources.Ecotracer_32x32
        Me.m_tsEcotracerScenario.Name = "m_tsEcotracerScenario"
        Me.m_tsEcotracerScenario.Visible = False
        '
        'm_tsiModified
        '
        Me.m_tsiModified.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsiModified.Image = Global.ScientificInterface.My.Resources.Resources.Modified
        Me.m_tsiModified.Name = "m_tsiModified"
        Me.m_tsiModified.Visible = False
        '
        'm_tslVersion
        '
        Me.m_tslVersion.Name = "m_tslVersion"
        Me.m_tslVersion.Size = New System.Drawing.Size(39, 21)
        Me.m_tslVersion.Text = "<ver>"
        Me.m_tslVersion.Visible = False
        Me.ResumeLayout(False)
        '
        'moi
        '
        Me.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbProgress, Me.m_tsStatus, Me.m_tsSelection, Me.m_tsEcopathModel, Me.m_tsEcosimScenario, Me.m_tsEcospaceScenario, Me.m_tsEcotracerScenario, Me.m_tsiModified, Me.m_tslVersion})
        Me.ShowItemToolTips = True
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
