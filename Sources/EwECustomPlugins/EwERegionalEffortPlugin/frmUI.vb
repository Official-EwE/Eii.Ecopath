' ===============================================================================
' This file is part of the EcoOcean toolkit.
'
' To use EcoOceanUtils please contact the EcoOcean core team at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmUI

#Region " Private vars "

    Private m_plugin As cRegionalEffortPlugin = Nothing
    Private m_bInUpdate As Boolean = True

    Private WithEvents m_fpUseCostThreshold As cEwEFormatProvider = Nothing
    Private WithEvents m_fpSailingCostThreshold As cEwEFormatProvider = Nothing

#End Region ' Private vars

    Public Sub New(plugin As cRegionalEffortPlugin, uic As cUIContext)
        MyBase.New()

        Me.m_plugin = plugin
        Me.UIContext = uic

        Me.InitializeComponent()

    End Sub

#Region " Form overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Try
            AddHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Me.Text = My.Resources.CAPTION
        Me.TabText = Me.Text

        Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

        Me.m_btnCalc.Text = ""
        Me.m_btnCalc.Image = SharedResources.CalculatorHS

        Me.m_fpUseCostThreshold = New cPropertyFormatProvider(Me.UIContext, Me.m_cbOnlyFishBelowCostThreshold, parms, eVarNameFlags.UseEffortDistThreshold)
        Me.m_fpSailingCostThreshold = New cPropertyFormatProvider(Me.UIContext, Me.m_tbxEffortDistThreshold, parms, eVarNameFlags.EffortDistThreshold)

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.TimeSeries, eCoreComponentType.Ecospace}
        Me.m_bInUpdate = False

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        Try
            RemoveHandler Me.m_plugin.OnChanged, AddressOf Me.OnChanged
            Me.m_fpUseCostThreshold.Release()
            Me.m_fpSailingCostThreshold.Release()
        Catch ex As Exception

        End Try
        MyBase.OnFormClosed(e)
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        If Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True

        Try
            Dim bOK As Boolean = Me.m_plugin.OverwriteEffort

            Me.m_tbxEffortFile.Text = Me.m_plugin.EffortFileName
            Me.m_tbxZoneName.Text = Me.m_plugin.EffortZoneName

            Me.m_cbAutoModeEnabled.Checked = Me.m_plugin.Enabled
            Me.m_cbNormalizeZonalEffort.Checked = Me.m_plugin.NormalizeEffort
            Me.m_cbWriteCatches.Checked = Me.m_plugin.WriteCatcheTimeSeries
            Me.m_cbWriteMortalities.Checked = Me.m_plugin.WriteMortalitiesTimeSeries
            Me.m_cbWriteEffort.Checked = Me.m_plugin.WriteEffortTimeSeries

            Me.m_pbStatus.Image = If(bOK, SharedResources.OK, SharedResources.Warning)
            Me.m_lblStatus.Text = If(bOK, My.Resources.STATUS_RUN, My.Resources.STATUS_NO_RUN)
            Me.m_lblZoneInfo2.Text = String.Format("{0} zone(s) defined", Me.m_plugin.NumZones)

        Catch ex As Exception

        End Try

        Me.m_bInUpdate = False

    End Sub

#End Region ' Form overrides

#Region " Public bits "

    Public Sub RefreshContent()
        Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
    End Sub

#End Region ' Public bits

#Region " Event handlers "

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)

        If (msg.Source = eCoreComponentType.TimeSeries Or msg.Source = eCoreComponentType.Ecospace) Then
            ' Lazy update UI
            Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateControls))
        End If

    End Sub

    Private Sub OnChanged()
        Me.UpdateControls()
    End Sub

    Private Sub OnLoadZonesCSV(sender As Object, e As EventArgs) _
        Handles m_btnLoad.Click

        Dim dlg As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog("Load zonal file with cell areas", "", ScientificInterfaceShared.My.Resources.FILEFILTER_CSV)
        If (dlg.ShowDialog() = DialogResult.OK) Then
            Me.m_plugin.LoadZonesAndCellAreas(dlg.FileName)
        End If

        Me.Apply()
        Me.UpdateControls()

    End Sub

    Private Sub OnLoadZonesMap(sender As Object, e As EventArgs) _
        Handles m_btnLoadMap.Click

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmd As cImportLayerCommand = DirectCast(cmdh.GetCommand(cImportLayerCommand.cCOMMAND_NAME), cImportLayerCommand)
        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap

        cmd.Invoke({bm.LayerEffortZone}, eNativeLayerFileFormatTypes.ASCII)

        Me.CalcZones()

    End Sub

    Private Sub OnChooseEffortFile(sender As Object, e As EventArgs) _
        Handles m_btnChoosePath.Click

        Dim dlg As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_EFFORTFILE, Me.m_plugin.EffortFileName, ScientificInterfaceShared.My.Resources.FILEFILTER_CSV)
        If (dlg.ShowDialog() = DialogResult.OK) Then
            Me.m_plugin.EffortFileName = dlg.FileName
        End If

        Me.Apply()
        Me.UpdateControls()

    End Sub

    Private Sub OnCalcZOnes(sender As Object, e As EventArgs) Handles m_btnCalc.Click

        Me.CalcZones()

    End Sub

    Private Sub OnSettingsChanged(sender As Object, e As EventArgs) _
        Handles m_fpSailingCostThreshold.OnValueChanged, m_cbAutoModeEnabled.CheckedChanged, m_cbNormalizeZonalEffort.CheckedChanged,
                m_cbWriteCatches.CheckedChanged, m_cbWriteMortalities.CheckedChanged, m_cbWriteEffort.CheckedChanged

        If Me.m_bInUpdate Then Return

        Me.Apply()
        Me.UpdateControls()

    End Sub

#End Region ' Event handlers

#Region " Internals "

    Private Sub CalcZones()

        Dim bm As cEcospaceBasemap = Me.Core.EcospaceBasemap
        Dim d As cEcospaceLayerDepth = bm.LayerDepth
        Dim z As cEcospaceLayerEffortZone = bm.LayerEffortZone
        Dim nMax As Integer = 0

        For r As Integer = 1 To bm.InRow
            For c As Integer = 1 To bm.InCol
                If d.IsWaterCell(r, c) Then nMax = Math.Max(nMax, CInt(z.Cell(r, c)))
            Next
        Next

        Me.Core.EcospaceModelParameters.nEffortZones = nMax
        Me.UpdateControls()

    End Sub

    Private Sub Apply()

        Me.m_plugin.Enabled = Me.m_cbAutoModeEnabled.Checked
        Me.m_plugin.NormalizeEffort = Me.m_cbNormalizeZonalEffort.Checked
        Me.m_plugin.WriteCatcheTimeSeries = Me.m_cbWriteCatches.Checked
        Me.m_plugin.WriteMortalitiesTimeSeries = Me.m_cbWriteMortalities.Checked
        Me.m_plugin.WriteEffortTimeSeries = Me.m_cbWriteEffort.Checked
        Me.m_plugin.EffortZoneName = Me.m_tbxZoneName.Text

        Me.Core.EcospaceModelParameters.EffortDistThreshold = CSng(Me.m_fpSailingCostThreshold.Value)
        Me.Core.EcospaceModelParameters.UseEffortDistThreshold = Me.m_cbOnlyFishBelowCostThreshold.Checked

    End Sub


#End Region ' Internals

End Class