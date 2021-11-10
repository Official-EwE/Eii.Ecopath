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
Imports EwEEcologicalIndicatorsPlugin
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmConfig

    Private m_bInUpdate As Boolean = True

    Private Shared _inst_ As frmConfig

    Public Shared Function GetUI(uic As cUIContext, data As cConfiguration) As frmConfig
        Dim bHasUI As Boolean = False
        If (_inst_ IsNot Nothing) Then
            bHasUI = Not _inst_.IsDisposed
        End If
        If Not bHasUI Then
            _inst_ = New frmConfig(uic, data)
        End If
        Return _inst_
    End Function

    Protected Sub New()
        Me.InitializeComponent()
        Me.Text = My.Resources.CAPTION
        Me.TabText = Me.Text
        Me.Grid = Me.m_grid
    End Sub

    Protected Sub New(uic As cUIContext, config As cConfiguration)
        Me.New()
        Me.UIContext = uic
        Me.Config = config
    End Sub

    Public ReadOnly Property Config As cConfiguration

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Me.m_tsbnLoadProtocol.Image = SharedResources.star_blue
        Me.m_tsbnEcoOceanDefaults.Image = My.Resources.ecoocean_768px
        Me.m_tsbnTaxonDefaults.Image = SharedResources.taxon
        Me.m_tsbnSaveEcosim.Image = SharedResources.Ecosim_32x32
        Me.m_tsbnSaveEcospace.Image = SharedResources.Ecospace_32x32
        Me.m_tsbnSaveEcoInd.Image = My.Resources.BioDiversityPlugin
        Me.m_tsbnEcoIndTriatlas.Image = My.Resources.triatlas_fish
        Me.m_tsbnCalculateScaling.Image = SharedResources.CalculatorHS

        If (Me.Config Is Nothing) Then Return

        Me.m_grid.UIContext = Me.UIContext
        Me.m_grid.Configuration = Me.Config
        Me.m_grid.RefreshContent()

        Me.InitUI()

        Me.m_bInUpdate = False
        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        Me.Config.SaveChanges()
        MyBase.OnFormClosing(e)
        Me.m_grid.UIContext = Nothing
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)
        frmConfig._inst_ = Nothing
    End Sub

    Protected Overrides Sub UpdateControls()
        MyBase.UpdateControls()

        Dim bhasProtocol As Boolean = Not String.IsNullOrWhiteSpace(Me.Config.ProtocolName)
        Dim bHasArea As Boolean = Not String.IsNullOrWhiteSpace(Me.Config.OceanRegion)
        Dim bHasESM As Boolean = (Me.m_cmbESM.SelectedIndex >= 0)
        Dim bIsConfigured As Boolean = bHasArea And bHasESM And bhasProtocol

        Me.m_tsbnCalculateScaling.Enabled = bhasProtocol
        Me.m_tsbnSaveEcosim.Enabled = False ' bHasArea And sm.HasEcosimLoaded
        Me.m_tsbnSaveEcospace.Enabled = bIsConfigured
        Me.m_tsbnSaveEcoInd.Enabled = bIsConfigured
        Me.m_btnApply.Enabled = bIsConfigured

    End Sub

    Private Sub InitUI()

        Me.m_bInUpdate = True

        ' Populate variables grid
        Me.m_grid.RefreshContent()

        Dim iSel As Integer = -1

        ' Populate experiments combo
        Me.m_cmbExperiment.Items.Clear() : iSel = 0
        Me.m_cmbExperiment.Items.Add(SharedResources.GENERIC_VALUE_CUSTOM)
        For Each ex As cExperiment In Me.Config.Experiments
            Me.m_cmbExperiment.Items.Add(ex)
            If (Object.ReferenceEquals(ex, Me.Config.Experiment)) Then
                iSel = Me.m_cmbESM.Items.Count
            End If
        Next
        Me.m_cmbExperiment.SelectedIndex = iSel

        ' Populate GCMs combo
        Me.m_cmbESM.Items.Clear() : iSel = -1
        For Each gcm As cGCM In Me.Config.GlobalClimateModels
            If (String.Compare(gcm.Name, Me.Config.ClimateModel, True) = 0) Then
                iSel = Me.m_cmbESM.Items.Count
            End If
            Me.m_cmbESM.Items.Add(gcm)
        Next
        Me.m_cmbESM.SelectedIndex = iSel

        ' Populate periods box
        Me.m_dgvExperimentDrivers.Rows.Clear()
        For Each p As cPeriod In Me.Config.Periods
            Dim spec As String = p.Name
            Dim i As Integer = Me.m_dgvExperimentDrivers.Rows.Add(New Object() {spec, CStr(p.StartYear), CStr(p.EndYear), Me.Config.ClimateScenarioForPeriod(spec), Me.Config.SocioEconomicScenarioForPeriod(p.Name)})
            Me.m_dgvExperimentDrivers.Rows(i).Tag = p.Name
        Next

        ' Populate layer index grid
        Me.m_dgvDriversPos.Rows.Clear()
        Me.m_colDIIndex.ValueType = GetType(Integer)
        Dim names As String() = Me.Config.DriverLayerNames
        For i As Integer = 1 To names.Count
            Me.m_dgvDriversPos.Rows.Add(New Object() {1, names(i - 1), Me.Config.GCMVarDriverLayerMapping(names(i - 1))})
        Next

        ' Populate layer scaling grid
        Me.m_dgvDriverScaling.Rows.Clear()
        Me.m_colDSScaling.ValueType = GetType(Single)
        Dim n As Integer = 1
        For Each esm As cGCM In Me.Config.GlobalClimateModels
            For Each var As String In Me.Config.PhyVariables
                If Me.Config.IncludeScaling(esm.Name, var) Then
                    Dim i As Integer = Me.m_dgvDriverScaling.Rows.Add(New Object() {n, esm.Description, var, Me.Config.LayerScaling(esm.Name, var)})
                    Me.m_dgvDriverScaling.Rows(i).Tag = esm.Name
                    n += 1
                End If
            Next
        Next

        ' Populate time series grid
        Me.m_dgvFishing.Rows.Clear()
        For i As Integer = 1 To Me.Config.SocioEnconomicScenarios.Count
            Dim soc As cSocioEconomicScenario = Me.Config.SocioEnconomicScenarios(i - 1)
            Dim iRow As Integer = Me.m_dgvFishing.Rows.Add(New Object() {1, soc.Name, Me.Config.SocScenarioTSIndex(soc.Name)})
            Me.m_dgvFishing.Rows(iRow).Tag = soc.Name
        Next

        ' Populate indicator grid with all available EcoIND indicators, checking selected indicators
        Me.m_dgvIndicators.Rows.Clear()
        Dim ecoind As cEwEEcologicalIndicatorsPlugin = Me.Config.EcoIND
        Dim settings As cIndicatorSettings = ecoind.Settings
        For ig As Integer = 0 To settings.NumIndicatorGroups - 1
            Dim grp As cIndicatorInfoGroup = settings.IndicatorGroup(ig)
            For i As Integer = 0 To grp.NumIndicators - 1
                Dim ind As cIndicatorInfo = grp.Indicator(i)
                Dim var As String = Me.Config.EcoIndVariable(ind)
                Dim iRow As Integer = Me.m_dgvIndicators.Rows.Add(New Object() {Me.Config.Indicators.Contains(var), ind, var})
                Me.m_dgvIndicators.Rows(iRow).Tag = ind
            Next
        Next

        Me.Config.DiscoverGCMVarDriverLayerMapping()
        Me.m_tscmbArea.Items.Clear()
        For Each reg As cOceanRegion In Me.Config.OceanRegions
            Me.m_tscmbArea.Items.Add(reg.Name)
        Next
        Me.m_tscmbArea.Text = Me.Config.OceanRegion

        Me.m_tsbnSaveEcosim.Checked = Me.Config.SaveWithEcosim
        Me.m_tsbnSaveEcospace.Checked = Me.Config.SaveWithEcospace
        Me.m_tsbnSaveEcoInd.Checked = Me.Config.SaveWithEcoIND

        Me.m_bInUpdate = False

        Me.UpdateExperimentSelections()

    End Sub

    Private Sub UpdateExperimentSelections()

        Me.m_bInUpdate = True

        Try
            For Each dgv As DataGridViewRow In Me.m_dgvExperimentDrivers.Rows
                Dim period As String = CStr(dgv.Tag)
                dgv.Cells(3).Value = Me.Config.ClimateScenarioForPeriod(period)
                dgv.Cells(4).Value = Me.Config.SocioEconomicScenarioForPeriod(period)
            Next
        Catch ex As Exception

        End Try

        Me.m_bInUpdate = False

    End Sub

    Private Sub CommitIndexChanges()

        Try
            For Each row As DataGridViewRow In Me.m_dgvDriversPos.Rows
                Me.Config.GCMVarDriverLayerMapping(CStr(row.Cells(1).Value)) = CInt(row.Cells(2).Value)
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub CommitScalingChanges()

        Try
            For Each row As DataGridViewRow In Me.m_dgvDriverScaling.Rows
                Dim gcm As String = CStr(row.Tag)
                Me.Config.LayerScaling(gcm, CStr(row.Cells(2).Value)) = CSng(row.Cells(3).Value)
            Next
        Catch ex As Exception

        End Try

    End Sub

    Private Sub CommitChanges()

        Me.m_bInUpdate = True

        Me.Config.OceanRegion = Me.m_tscmbArea.Text

        If (Me.m_cmbESM.SelectedItem IsNot Nothing) Then
            Me.Config.ClimateModel = DirectCast(Me.m_cmbESM.SelectedItem, cGCM).Name
        Else
            Me.Config.ClimateModel = ""
        End If

        For Each dgv As DataGridViewRow In Me.m_dgvExperimentDrivers.Rows
            Dim period As String = CStr(dgv.Tag)
            Me.Config.ClimateScenarioForPeriod(period) = CStr(dgv.Cells(3).Value)
            Me.Config.SocioEconomicScenarioForPeriod(period) = CStr(dgv.Cells(4).Value)
        Next

        For Each dgv As DataGridViewRow In Me.m_dgvFishing.Rows
            Dim soc As String = CStr(dgv.Tag)
            Me.Config.SocScenarioTSIndex(soc) = CInt(dgv.Cells(2).Value)
        Next

        Me.Config.SaveWithEcosim = Me.m_tsbnSaveEcosim.Checked
        Me.Config.SaveWithEcospace = Me.m_tsbnSaveEcospace.Checked
        Me.Config.SaveWithEcoIND = Me.m_tsbnSaveEcoInd.Checked

        Me.Config.Indicators.Clear()
        For Each dgv As DataGridViewRow In Me.m_dgvIndicators.Rows
            Dim ind As cIndicatorInfo = DirectCast(dgv.Cells(1).Value, cIndicatorInfo)
            ind.Enabled = CBool(dgv.Cells(0).Value)
            If ind.Enabled Then
                Me.Config.Indicators.Add(Me.Config.EcoIndVariable(ind))
            End If
        Next
        Me.Config.SaveChanges()

        If (Me.Config.Experiment Is Nothing) Then
            Me.m_cmbExperiment.SelectedIndex = 0
        Else
            Me.m_cmbExperiment.SelectedItem = Me.Config.Experiment
        End If

        Me.m_bInUpdate = False
        Me.UpdateControls()

    End Sub

#Region " Events "

    Private Sub OnLoadEcoOceanDefaultsMappings(sender As Object, e As EventArgs) _
        Handles m_tsbnEcoOceanDefaults.Click

        Me.Config.LoadEcoOceanDefaultMappings()
        Me.m_grid.RefreshContent()

    End Sub

    Private Sub OnLoadFromTaxonomicBreakdown(sender As Object, e As EventArgs) _
        Handles m_tsbnTaxonDefaults.Click

        Me.Config.DecipherGroupOutputMappingsFromTaxa()
        Me.m_grid.RefreshContent()

    End Sub

    Private Sub OnExperimentSelectionChanged(sender As Object, e As EventArgs) _
        Handles m_cmbExperiment.SelectedIndexChanged

        If (Me.m_bInUpdate) Then Return
        Dim sel As Object = Me.m_cmbExperiment.SelectedItem
        If (Not TypeOf sel Is cExperiment) Then
            Me.Config.Experiment = Nothing
        Else
            Me.Config.Experiment = DirectCast(sel, cExperiment)
        End If
        Me.UpdateExperimentSelections()

    End Sub

    Private Sub OnSelectionChanged(sender As Object, e As EventArgs) _
        Handles m_cmbESM.SelectedIndexChanged

        If (Me.m_bInUpdate) Then Return
        Me.CommitChanges()

    End Sub

    Private Sub OnInputChanged(sender As Object, e As EventArgs) _
        Handles m_tsbnSaveEcospace.CheckedChanged, m_tsbnSaveEcosim.CheckedChanged, m_cmbESM.SelectedIndexChanged, m_tscmbArea.TextChanged

        If (Me.m_bInUpdate) Then Return
        Me.CommitChanges()

    End Sub

    Private Sub OnDriverSettingsChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvDriverScaling.CellValueChanged, m_dgvDriversPos.CellValueChanged, m_dgvFishing.CellValueChanged, m_dgvIndicators.CellValueChanged

        If (Me.m_bInUpdate) Then Return
        Me.CommitIndexChanges()
        Me.CommitScalingChanges()
        Me.Config.SaveChanges()

    End Sub

    Private Sub OnConfigureEwE(sender As Object, e As EventArgs) _
        Handles m_btnApply.Click

        Me.CommitChanges()
        Dim ctrl As New cEwEController(Me.Core, Me.Config)
        ctrl.ConfigEwE()

    End Sub

    Private Sub OnLoadProtocol(sender As Object, e As EventArgs) Handles m_tsbnLoadProtocol.Click

        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.CAPTION_LOADPROTOCOL, "", My.Resources.FILEFILTER)
        If ofd.ShowDialog() = DialogResult.OK Then
            If Me.Config.LoadProtocol(ofd.FileName) Then
                Me.m_tsbnLoadProtocol.Text = Me.Config.ProtocolName
            End If
            Me.InitUI()
            Me.UpdateControls()
        End If

    End Sub

    Private Sub OnFormatGCM(sender As Object, e As ListControlConvertEventArgs) Handles m_cmbESM.Format
        If (TypeOf e.ListItem Is cGCM) Then
            e.Value = DirectCast(e.ListItem, cGCM).Description
        End If
    End Sub

    Private Sub OnGridEdited(sender As Object, e As DataGridViewCellEventArgs) Handles m_dgvIndicators.CellEndEdit
        If (Me.m_bInUpdate) Then Return
        Me.Config.SaveChanges()
    End Sub

    Private Sub OnSelectEcoIndNone(sender As Object, e As EventArgs) Handles m_tsbnEcoIndNone.Click
        Me.m_bInUpdate = True
        For Each drow As DataGridViewRow In Me.m_dgvIndicators.Rows
            drow.Cells(0).Value = False
        Next
        Me.m_bInUpdate = False
        Me.Config.SaveChanges()
    End Sub

    Private Sub OnSelectEcoIndAll(sender As Object, e As EventArgs) Handles m_tsbnEcoIndAll.Click
        Me.m_bInUpdate = True
        For Each drow As DataGridViewRow In Me.m_dgvIndicators.Rows
            drow.Cells(0).Value = True
        Next
        Me.m_bInUpdate = False
        Me.Config.SaveChanges()
    End Sub

    Private Sub OnSelectEcoIndTriatlas(sender As Object, e As EventArgs) Handles m_tsbnEcoIndTriatlas.Click
        Me.m_bInUpdate = True
        For Each drow As DataGridViewRow In Me.m_dgvIndicators.Rows
            Dim ind As cIndicatorInfo = DirectCast(drow.Tag, cIndicatorInfo)
            Dim var As String = Me.Config.EcoIndVariable(ind)
            drow.Cells(0).Value = cConfiguration.EcoIndTriatlasVariables.Contains(var)
        Next
        Me.m_bInUpdate = False
        Me.Config.SaveChanges()
    End Sub

    Private Sub OnComputePPLayerScaling(sender As Object, e As EventArgs) Handles m_tsbnCalculateScaling.Click
        Me.m_bInUpdate = True
        Me.CommitChanges()

        Dim controller As New cEwEController(Me.Core, Me.Config)
        For Each gcm As cGCM In Me.Config.GlobalClimateModels
            For Each var As String In Me.Config.PhyVariables
                Dim scalar As Single = controller.CalculatePhyScalar(gcm.Name, var)
                If (scalar > 0) Then
                    Me.Config.LayerScaling(gcm.Name, var) = scalar
                End If
            Next
        Next
        Me.Config.SaveChanges()
        Me.InitUI()
        Me.m_bInUpdate = False
    End Sub

#End Region ' Events

End Class