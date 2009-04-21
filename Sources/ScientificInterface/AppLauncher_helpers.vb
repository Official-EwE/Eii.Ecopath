'==============================================================================
'
' $Log: AppLauncher_helpers.vb,v $
' Revision 1.13  2009/04/21 15:48:22  jeroens
' Fixed TS index issue
'
' Revision 1.12  2009/04/21 14:45:16  jeroens
' Time series shown in Sim label
' Simpliefied modified display in tool tips
'
' Revision 1.11  2009/04/13 14:14:36  jeroens
' Update selection pane at startup
'
' Revision 1.10  2009/03/24 15:56:41  jeroens
' Updated to minor ScIntShared namespace changes
'
' Revision 1.9  2009/03/22 14:01:34  jeroens
' Core state monitor exec event parameters simplified
'
' Revision 1.8  2009/03/12 01:33:26  jeroens
' SAVE before you commit! SAVE!
'
' Revision 1.7  2009/03/12 01:31:35  jeroens
' ResetVisibleFlags may not distribute event
'
' Revision 1.6  2009/03/02 01:44:40  jeroens
' Changed init order in statusbarhelper
'
' Revision 1.5  2009/03/01 19:32:14  jeroens
' Changed data modified indicator
'
' Revision 1.4  2009/01/16 23:49:06  jeroens
' Status strip items no longer confusingly enabled
'
' Revision 1.3  2009/01/16 16:05:53  joeb
' Removed the Hack that closed Ecospace forms in response to any state changed
'
' Revision 1.2  2008/11/27 03:10:42  jeroens
' Group visible flags maintained by style guide, no longer by AppLauncher
'
' Revision 1.1  2008/09/26 07:31:24  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Imports EwECore
Imports WeifenLuo.WinFormsUI.Docking
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEPlugin
Imports System.Text

#End Region ' Imports

Partial Public Class AppLauncher

#Region " StatusStripHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; maintains content of the status strip panes in the AppLauncher.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StatusStripHelper

        ''' <summary>The core to observe.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>The status strip that is being controlled.</summary>
        Private m_ss As StatusStrip = Nothing

        ''' <summary>The property selection command to listen to.</summary>
        Private WithEvents m_cmd As PropertySelectionCommand = Nothing
        ''' <summary>Selected properties.</summary>
        Private m_aprop As cProperty() = Nothing
        ''' <summary>Selection pane.</summary>
        Private m_tsSelection As ToolStripItem = Nothing

        ''' <summary>The core state monitor offering events to observe.</summary>
        Private WithEvents m_csm As cCoreStateMonitor = Nothing
        ''' <summary>Ecopath model state pane offering events to observe.</summary>
        Private m_tsEcopathModel As ToolStripItem = Nothing
        ''' <summary>Ecosim scenario state pane offering events to observe.</summary>
        Private m_tsEcosimScenario As ToolStripItem = Nothing
        ''' <summary>Ecospace scenario state pane offering events to observe.</summary>
        Private m_tsEcospaceScenario As ToolStripItem = Nothing
        ''' <summary>Ecospace scenario state pane offering events to observe.</summary>
        Private m_tsEcotracerScenario As ToolStripItem = Nothing
        ''' <summary>Main status item.</summary>
        Private m_tsStatus As ToolStripItem = Nothing
        ''' <summary>Main progress bar.</summary>
        Private m_tsbProgress As ToolStripProgressBar = Nothing
        ''' <summary>Modified state pane.</summary>
        Private m_tsiModified As ToolStripItem = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="core">The EwE core to attach to</param>
        ''' <param name="ss"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByRef core As cCore, ByRef ss As StatusStrip)
            ' Store relevant references
            Me.m_core = core
            Me.m_ss = ss

            Me.m_tsEcopathModel = ss.Items("m_tsEcopathModel")
            Me.m_tsEcosimScenario = ss.Items("m_tsEcosimScenario")
            Me.m_tsEcospaceScenario = ss.Items("m_tsEcospaceScenario")
            Me.m_tsEcotracerScenario = ss.Items("m_tsEcotracerScenario")
            Me.m_tsiModified = ss.Items("m_tsiModified")
            Me.m_tsStatus = ss.Items("m_tsStatus")
            Me.m_tsbProgress = CType(ss.Items("m_tsbProgress"), ToolStripProgressBar)

            Me.m_tsSelection = ss.Items("m_tsSelection")

            ' Get property selection command
            Me.m_cmd = CType(CommandHandler.GetInstance().GetCommand(PropertySelectionCommand.COMMAND_NAME), PropertySelectionCommand)

            ' Hook up to relevant event sources
            Me.m_csm = core.StateMonitor

            Me.UpdateSelectionPane()
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
        Private Sub OnInvoke(ByVal cmd As Command) Handles m_cmd.OnInvoke

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
        Private Sub UpdateToolstripItem(ByRef tsi As ToolStripItem, _
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
            Me.m_ss.Refresh()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update the state and contents of the controls in the panel.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateSelectionPane()

            Dim strSelection As String = My.Resources.SELECTION_NONE
            Dim srcPrim As cCoreInputOutputBase = Nothing
            Dim bPrimMixed As Boolean = False
            Dim strPrim As String = ""
            Dim srcSec As cCoreInputOutputBase = Nothing
            Dim bSecMixed As Boolean = False
            Dim strSec As String = ""

            If Not Me.m_csm.HasEcopathLoaded() Then m_aprop = Nothing

            ' Find all prim and sec props
            If m_aprop IsNot Nothing Then

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
                    If m_aprop.Length > 0 Then
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

    End Class

#End Region ' StatusStripHelper

#Region " FormStateHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class; maintains form enabled / availability states in the AppLauncher.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class EwEFormStateHelper

        Private WithEvents m_csm As cCoreStateMonitor
        Private m_dp As DockPanel

        Public Sub New(ByVal csm As cCoreStateMonitor, ByVal dp As DockPanel)
            Me.m_dp = dp
            Me.m_csm = csm
        End Sub

        Private Sub m_csm_CoreExecutionStateEvent(ByVal csm As cCoreStateMonitor) _
            Handles m_csm.CoreExecutionStateEvent
            Me.UpdateFormStates()
        End Sub

        Public Function OpenEwEForms() As List(Of frmEwE)
            Dim l As New List(Of frmEwE)

            If (Me.m_dp IsNot Nothing) Then
                For Each idc As IDockContent In Me.m_dp.Documents
                    If (TypeOf idc Is frmEwE) Then
                        l.Add(DirectCast(idc, frmEwE))
                    End If
                Next
            End If

            Return l
        End Function

        Private Sub UpdateFormStates()

            Dim stateForm As eCoreExecutionState = eCoreExecutionState.Idle
            Dim bMustCloseForm As Boolean = False

            For Each f As frmEwE In Me.OpenEwEForms()
                ' Think positive
                bMustCloseForm = False

                ' Get form state
                stateForm = f.CoreExecutionState

                ' Check if form should be disabled
                bMustCloseForm = ((Not Me.m_csm.IsExecutionStateSuperceded(stateForm)) And frmEwE.IsOutputForm(stateForm))

                If bMustCloseForm Then
                    ' #Yes: Close the form
                    f.Close()
                End If
            Next
        End Sub

    End Class

#End Region ' FormStateHelper

#Region " StyleGuideUpdater "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' On-board helper class that actively updates model-derived settings in the style guide.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StyleGuideUpdater

        Private m_core As cCore = Nothing
        Private m_sg As StyleGuide = Nothing
        Private m_bIsEcopathLoaded As Boolean = False

        Private m_sm As cCoreStateMonitor = Nothing
        Private m_propNumDigits As cProperty = Nothing
        Private m_propUnitTime As cIntegerProperty = Nothing
        Private m_propUnitTimeText As cStringProperty = Nothing
        Private m_propUnitCurrency As cIntegerProperty = Nothing
        Private m_propUnitCurrencyText As cStringProperty = Nothing
        Private m_propUnitMonetary As cIntegerProperty = Nothing
        Private m_propUnitMonetaryText As cStringProperty = Nothing

        Public Sub New(ByVal core As cCore, ByVal sg As StyleGuide)

            Me.m_core = core
            Me.m_sm = core.StateMonitor
            Me.m_sg = sg

            AddHandler m_sm.CoreExecutionStateEvent, AddressOf OnCoreStateEvent

        End Sub

        Private Sub OnCoreStateEvent(ByVal csm As cCoreStateMonitor)
            If Me.m_bIsEcopathLoaded <> csm.HasEcopathLoaded Then
                Me.m_bIsEcopathLoaded = csm.HasEcopathLoaded
                Me.Update()
            End If
        End Sub

        Private Sub Update()

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            Me.m_sg.SuspendEvents()

            If Me.m_bIsEcopathLoaded Then

                Me.m_propNumDigits = pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.NumDigits)
                AddHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumDigitsChanged

                Me.m_propUnitCurrency = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitCurrency), cIntegerProperty)
                Me.m_propUnitCurrencyText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitCurrencyCustomText), cStringProperty)
                AddHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
                AddHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged

                Me.m_propUnitTime = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitTime), cIntegerProperty)
                Me.m_propUnitTimeText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitTimeCustomText), cStringProperty)
                AddHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
                AddHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged

                Me.m_propUnitMonetary = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitMonetary), cIntegerProperty)
                Me.m_propUnitMonetaryText = DirectCast(pm.GetProperty(Me.m_core.EwEModel, eVarNameFlags.UnitMonetaryCustomText), cStringProperty)
                AddHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged
                AddHandler Me.m_propUnitMonetaryText.PropertyChanged, AddressOf OnMonetaryUnitChanged

                Me.OnCurrencyUnitChanged(m_propUnitCurrency, cProperty.eChangeFlags.All)
                Me.OnTimeUnitChanged(m_propUnitTime, cProperty.eChangeFlags.All)
                Me.OnMonetaryUnitChanged(m_propUnitMonetary, cProperty.eChangeFlags.All)
                Me.OnNumDigitsChanged(m_propNumDigits, cProperty.eChangeFlags.All)
            Else
                RemoveHandler Me.m_propNumDigits.PropertyChanged, AddressOf OnNumDigitsChanged
                Me.m_propNumDigits = Nothing

                RemoveHandler Me.m_propUnitCurrency.PropertyChanged, AddressOf OnCurrencyUnitChanged
                RemoveHandler Me.m_propUnitCurrencyText.PropertyChanged, AddressOf OnCurrencyUnitChanged
                Me.m_propUnitCurrency = Nothing
                Me.m_propUnitCurrencyText = Nothing

                RemoveHandler Me.m_propUnitTime.PropertyChanged, AddressOf OnTimeUnitChanged
                RemoveHandler Me.m_propUnitTimeText.PropertyChanged, AddressOf OnTimeUnitChanged
                Me.m_propUnitTime = Nothing
                Me.m_propUnitTimeText = Nothing

                RemoveHandler Me.m_propUnitMonetary.PropertyChanged, AddressOf OnMonetaryUnitChanged
                RemoveHandler Me.m_propUnitMonetaryText.PropertyChanged, AddressOf OnMonetaryUnitChanged
                Me.m_propUnitMonetary = Nothing
                Me.m_propUnitMonetaryText = Nothing
            End If

            Me.m_sg.ResetVisibleFlags(False)
            Me.m_sg.ResumeEvents()

        End Sub

        Private Sub OnCurrencyUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.CurrencyUnit = DirectCast(Me.m_propUnitCurrency.GetValue(), eUnitCurrencyType)
            Me.m_sg.CustomCurrencyUnitText = CStr(Me.m_propUnitCurrencyText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnTimeUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.TimeUnit = DirectCast(Me.m_propUnitTime.GetValue(), eUnitTimeType)
            Me.m_sg.CustomTimeUnitText = CStr(Me.m_propUnitTimeText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnMonetaryUnitChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.MonetaryUnit = DirectCast(Me.m_propUnitMonetary.GetValue(), eUnitMonetaryType)
            Me.m_sg.CustomMonetaryUnitText = CStr(Me.m_propUnitMonetaryText.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Private Sub OnNumDigitsChanged(ByVal prop As cProperty, ByVal ct As cProperty.eChangeFlags)
            Me.m_sg.SuspendEvents()
            Me.m_sg.NumDigits = CInt(Me.m_propNumDigits.GetValue())
            Me.m_sg.ResumeEvents()
        End Sub

        Public Sub Load()

            Me.m_sg.SuspendEvents()

            Me.m_sg.LoadDefaultApplicationColors()

            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.DEFAULT_TEXT) = My.Settings.ColorDefaultText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.DEFAULT_BACKGROUND) = My.Settings.ColorDefaultBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.NAMES_TEXT) = My.Settings.ColorNameText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.NAMES_BACKGROUND) = My.Settings.ColorNameBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT) = My.Settings.ColorFailedResultText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT) = My.Settings.ColorFailedValidationText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.GENERICERROR_TEXT) = My.Settings.ColorErrorText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.COMPUTED_TEXT) = My.Settings.ColorComputedValuesText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT) = My.Settings.ColorESPressureText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PROFIT_TEXT) = My.Settings.ColorESProfitsText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.TOTALCATCH_TEXT) = My.Settings.ColorESTotalCatchText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.TROPHICLINK_TEXT) = My.Settings.ColorTrophicLinkText
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.REMARKS_BACKGROUND) = My.Settings.ColorRemarksBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.SUM_BACKGROUND) = My.Settings.ColorSumBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.READONLY_BACKGROUND) = My.Settings.ColorReadOnlyBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.CHECKED_BACKGROUND) = My.Settings.ColorCheckedBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND) = My.Settings.ColorMissingParamBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.IMAGE_BACKGROUND) = My.Settings.ColorImageBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND) = My.Settings.ColorPlotsBackground
            Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.MAP_BACKGROUND) = My.Settings.ColorMapBackground

            Me.m_sg.ResumeEvents()

        End Sub

        Public Sub Save()

            My.Settings.ColorDefaultText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.DEFAULT_TEXT)
            My.Settings.ColorDefaultBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.DEFAULT_BACKGROUND)
            My.Settings.ColorNameText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.NAMES_TEXT)
            My.Settings.ColorNameBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.NAMES_BACKGROUND)
            My.Settings.ColorFailedResultText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.INVALIDMODELRESULT_TEXT)
            My.Settings.ColorFailedValidationText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.FAILEDVALIDATION_TEXT)
            My.Settings.ColorErrorText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.GENERICERROR_TEXT)
            My.Settings.ColorComputedValuesText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.COMPUTED_TEXT)
            My.Settings.ColorESPressureText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.FISHINGPRESSURE_TEXT)
            My.Settings.ColorESProfitsText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PROFIT_TEXT)
            My.Settings.ColorESTotalCatchText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.TOTALCATCH_TEXT)
            My.Settings.ColorTrophicLinkText = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.TROPHICLINK_TEXT)
            My.Settings.ColorRemarksBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
            My.Settings.ColorSumBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.SUM_BACKGROUND)
            My.Settings.ColorReadOnlyBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.READONLY_BACKGROUND)
            My.Settings.ColorCheckedBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.CHECKED_BACKGROUND)
            My.Settings.ColorMissingParamBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.MISSINGPARAMETER_BACKGROUND)
            My.Settings.ColorImageBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            My.Settings.ColorPlotsBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            My.Settings.ColorMapBackground = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.MAP_BACKGROUND)

            My.Settings.Save()
        End Sub

    End Class

#End Region ' StyleGuideUpdater

#Region " MRUHelper "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, generates and analyses MRU strings
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class MRUHelper

        Public Enum eModuleType
            Ecosim
            Ecospace
            Ecotracer
            Dataset
        End Enum

        Private Shared Function ModuleKey(ByVal moduleType As eModuleType) As String
            Select Case moduleType
                Case eModuleType.Ecosim : Return ",Ecosim_scenario:"
                Case eModuleType.Ecospace : Return ",Ecospace_scenario:"
                Case eModuleType.Ecotracer : Return ",Ecotracer_scenario:"
                Case eModuleType.Dataset : Return ",Ecosim_dataset:"
            End Select
            Return ""
        End Function

        Public Shared Function GetMRUString(ByVal alstrMRU As ArrayList, ByVal strModelName As String, ByVal moduleType As eModuleType) As String

            Dim strModuleKey As String = MRUHelper.ModuleKey(moduleType)
            Dim strMRU As String = ""
            Dim iKeyPos As Integer = -1
            Dim iNextTerminatorPos As Integer = -1
            Dim iNameStartPos As Integer = -1

            ' For almost each MRU entry (..wtf..)
            For i As Integer = 0 To alstrMRU.Count - 2
                strMRU = CStr(alstrMRU.Item(i))
                If strMRU.StartsWith(strModelName) Then

                    ' Search scenario key
                    iKeyPos = strMRU.IndexOf(strModuleKey)
                    ' Found it?
                    If iKeyPos <> -1 Then
                        ' #Yes: try to extract scenario name
                        ' Find first pos of scenario name
                        iNameStartPos = iKeyPos + strModuleKey.Length
                        ' Find next terminator, if any
                        iNextTerminatorPos = strMRU.IndexOf(CChar(","), iKeyPos + 1)
                        ' Terminator not found?
                        If iNextTerminatorPos = -1 Then
                            ' #No terminator: name must be the rest of the string
                            Return strMRU.Substring(iNameStartPos)
                        Else
                            ' #Terminator: name must be all chars up to terminator
                            Return strMRU.Substring(iNameStartPos, iNextTerminatorPos - iNameStartPos)
                        End If
                    End If
                    ' No scenario name for this MRU entry
                    Return ""

                End If
            Next
            Return ""

        End Function

        Public Shared Sub UpdateMRUString(ByVal alstrMRU As ArrayList, ByVal strValue As String, ByVal mt As eModuleType)

            ' Item does not exist, abort!
            If (alstrMRU.Count = 1) Then Return

            Dim strMRU As String = CStr(alstrMRU.Item(0))
            Dim strModuleKey As String = MRUHelper.ModuleKey(mt)
            Dim iKeyPos As Integer = strMRU.IndexOf(strModuleKey)
            Dim iTerminatorPos As Integer = strMRU.IndexOf(CChar(","), iKeyPos + 1)
            Dim strLeft As String = String.Empty
            Dim strRight As String = String.Empty

            If iKeyPos = -1 Then
                If iTerminatorPos = -1 Then
                    strLeft = strMRU
                Else
                    strLeft = strMRU.Substring(0, iTerminatorPos)
                End If
            Else
                strLeft = strMRU.Substring(0, iKeyPos)
            End If
            If iTerminatorPos <> -1 Then
                strRight = strMRU.Substring(iTerminatorPos)
            End If
            ' Update MRU item
            alstrMRU.Item(0) = strLeft & strModuleKey & strValue & strRight

        End Sub

    End Class

#End Region ' MRUHelper

End Class
