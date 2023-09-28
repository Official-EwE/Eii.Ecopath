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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style.cStyleGuide
Imports System.Drawing
Imports System.Windows.Forms
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class frmBiomassEmitter

#Region " Constructor "

    Public Sub New(uic As cUIContext, engine As cBiomassEmitter)
        MyBase.New()
        Me.InitializeComponent()
        Me.Text = My.Resources.CAPTION_EMITTER
        Me.TabText = Me.Text
        Me.Engine = engine
        Me.UIContext = uic
    End Sub

#End Region ' Constructor

#Region " Overrides "

    ' ToDo: create DGV columns dynamically?

    Protected Overrides Sub OnLoad(e As EventArgs)

        MyBase.OnLoad(e)

        If (Me.UIContext Is Nothing) Then Return
        If (Me.Engine Is Nothing) Then Return

        Me.InUpdate = True

        ' -- Set up UI --
        Dim cmd As cBrowserCommand = DirectCast(Me.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.AddControl(Me.m_pbSafenet, "https://www.criobe.pf/recherche/safenet/")
        cmd.AddControl(Me.m_pbICM, "https://www.icm.csic.es")
        cmd.AddControl(Me.m_pbEII, "https://www.ecopathinternational.org")

        Me.m_btnBrowseTimeSeries.Image = SharedResources.openHS
        Me.m_lblVersion.Text = My.Resources.VERSION

        Me.m_cbEnabled.Checked = Me.Engine.Enabled

        Select Case Me.Data.TargetType
            Case eTargetType.Region
                Me.m_rbApplyToRegions.Checked = True
            Case eTargetType.MPA
                Me.m_rbApplyToMPAs.Checked = True
            Case eTargetType.Habitat
                Me.m_rbApplyToHabitats.Checked = True
            Case Else
                Debug.Assert(False)
        End Select

        Select Case Me.Data.ApplicationType
            Case eApplicationType.Absolute
                Me.m_rbApplyIsAbsolute.Checked = True
            Case eApplicationType.Relative
                Me.m_rbApplyIsRelative.Checked = True
            Case eApplicationType.Additive
                Me.m_rbApplyIsAdditive.Checked = True
        End Select

        Dim prots As eProtectionType() = CType([Enum].GetValues(GetType(eProtectionType)), eProtectionType())

        ' Populate MPA data
        Me.m_dgvRuleData.Rows.Clear()
        Me.m_colMPAProtection.Items.Clear()
        Me.m_colMPAProtection.ValueType = GetType(eProtectionType)
        Me.m_colMPAProtection.DataSource = prots

        ' Populate rule settings grid
        For i As Integer = 0 To prots.Count - 1
            Dim prot As eProtectionType = prots(i)
            Dim iRow As Integer = Me.m_dgvRuleSettings.Rows.Add(New Object() {prot, Me.Data.RuleMaxEffect(prot)})
            Me.m_dgvRuleSettings.Rows(iRow).Tag = prot
        Next

        Me.RefreshRuleGrid()
        Me.InUpdate = False

        Me.UpdateControls()

        ' -- Tell EwE what messages to send our way --
        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Ecospace}

    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)

        If (Me.UIContext Is Nothing) Then Return
        If (Me.Engine Is Nothing) Then Return

        Dim cmd As cBrowserCommand = DirectCast(Me.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
        cmd.RemoveControl(Me.m_pbSafenet)
        cmd.RemoveControl(Me.m_pbICM)
        cmd.RemoveControl(Me.m_pbEII)

        MyBase.OnFormClosing(e)

    End Sub

    Public Overrides Sub OnCoreMessage(msg As cMessage)
        MyBase.OnCoreMessage(msg)
        Me.UpdateControls()
    End Sub

    Protected Overrides Sub UpdateControls()

        If (Me.InUpdate) Then Return
        Me.InUpdate = True

        Dim strTarget As String = ""
        Select Case Me.Data.TargetType

            Case eTargetType.MPA
                strTarget = SharedResources.HEADER_MPA
                Me.m_rbApplyToMPAs.Checked = True

            Case eTargetType.Region
                strTarget = SharedResources.HEADER_REGION
                Me.m_rbApplyToRegions.Checked = True

            Case eTargetType.Habitat
                strTarget = SharedResources.HEADER_HABITAT
                Me.m_rbApplyToHabitats.Checked = True

            Case Else
                Debug.Assert(False)

        End Select
        Me.m_colTrendTarget.HeaderText = strTarget

        Me.UpdateStatus(Me.m_pbHasMetadata, Me.m_lblHasRules, Me.NumEnabledRules > 0 And Me.NumMPAs > 0, cStringUtils.Localize(My.Resources.CHECK_RULES_ENABLED, Me.NumEnabledRules), My.Resources.CHECK_RULES_DISABLED)
        If Not HasTrends() Then
            Me.UpdateStatus(Me.m_pbHasTrends, Me.m_lblHasTimeSeries, False, "", My.Resources.CHECK_TIMESERIES_MISSING)
        Else
            Me.UpdateStatus(Me.m_pbHasTrends, Me.m_lblHasTimeSeries, Me.HasTrendData(), My.Resources.CHECK_TIMESERIES_OK, My.Resources.CHECK_TIMESERIES_OUTOFRANGE)
        End If

        Me.InUpdate = False

    End Sub

#End Region ' Overrides

#Region " Public bits "

    Public ReadOnly Property Engine As cBiomassEmitter = Nothing

    Public ReadOnly Property Data As cData
        Get
            Return Me.Engine.Data
        End Get
    End Property

#End Region ' Public bits 

#Region " Events "

    Private Sub OnEnabledStateChanged(sender As Object, e As EventArgs) _
        Handles m_cbEnabled.CheckedChanged
        Try
            Me.Engine.Enabled = Me.m_cbEnabled.Checked
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnTargetChanged(sender As Object, e As EventArgs) _
        Handles m_rbApplyToRegions.CheckedChanged, m_rbApplyToMPAs.CheckedChanged, m_rbApplyToHabitats.CheckedChanged

        If (Me.Engine Is Nothing) Then Return
        If (Me.InUpdate) Then Return

        If Me.m_rbApplyToRegions.Checked Then Me.Data.TargetType = eTargetType.Region
        If Me.m_rbApplyToMPAs.Checked Then Me.Data.TargetType = eTargetType.MPA
        If Me.m_rbApplyToHabitats.Checked Then Me.Data.TargetType = eTargetType.Habitat

        ' Need to refresh validation status
        Me.RefreshTimeSeriesGrid()

    End Sub

    Private Sub OnDataTypeChanged(sender As Object, e As EventArgs) _
        Handles m_rbApplyIsRelative.CheckedChanged, m_rbApplyIsAbsolute.CheckedChanged, m_rbApplyIsAdditive.CheckedChanged

        If (Me.Engine Is Nothing) Then Return
        If (Me.InUpdate) Then Return

        If Me.m_rbApplyIsRelative.Checked Then Me.Data.ApplicationType = eApplicationType.Relative
        If Me.m_rbApplyIsAbsolute.Checked Then Me.Data.ApplicationType = eApplicationType.Absolute
        If Me.m_rbApplyIsAdditive.Checked Then Me.Data.ApplicationType = eApplicationType.Additive

    End Sub

    Private Sub OnLoadTrends(sender As Object, e As EventArgs) _
        Handles m_btnBrowseTimeSeries.Click

        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_SELECTTRENDFILE, "", SharedResources.FILEFILTER_CSV)
        ofd.Multiselect = True
        If (ofd.ShowDialog() = DialogResult.OK) Then
            Me.Data.LoadTimeSeries(ofd.FileNames)
            Me.RefreshTimeSeriesGrid()
        End If

    End Sub

    Private Sub OnResetTrends(sender As Object, e As EventArgs) _
        Handles m_btnResetTimeSeriesFile.Click

        Me.Data.Clear()
        Me.RefreshTimeSeriesGrid()

    End Sub

    Private Sub OnMagicButtonClicked(sender As Object, e As EventArgs) Handles m_btnBuildTrend.Click

        Dim util As New dlgBiomassEmitterTimeSeriesBuilder(Me.UIContext)
        If (util.ShowDialog(Me.UIContext.FormMain) = DialogResult.OK) Then
            If (util.LoadOnSave) Then
                Me.Data.LoadTimeSeries(New String() {util.FileName})
                Me.RefreshTimeSeriesGrid()
            End If
        End If
    End Sub

    Private Sub OnRuleSettingChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvRuleSettings.CellValueChanged

        ' Do not fire events while initializing
        If (Me.Engine Is Nothing) Or (Me.InUpdate) Then Return

        Dim prot As eProtectionType = DirectCast(Me.m_dgvRuleSettings.Rows(e.RowIndex).Tag, eProtectionType)
        Dim val As Single = CSng(Me.m_dgvRuleSettings(Me.m_colSettingsMaxEffect.Index, e.RowIndex).Value)

        Me.Data.RuleMaxEffect(prot) = val
        Me.Data.SaveModelChanges()
        Me.UpdateControls()

    End Sub

    Private Sub OnisableAllTimeSeries(sender As Object, e As EventArgs) Handles m_btnDisableAllTimeSeries.Click
        For Each timeseries As cEmissionTimeSeries In Me.Data.TimeSeries
            timeseries.Enable = False
        Next
        Me.RefreshTimeSeriesGrid()
    End Sub

    Private Sub OnEnableAllTimeSeries(sender As Object, e As EventArgs) Handles m_btnEnableAllTimeSeries.Click
        For Each timeseries As cEmissionTimeSeries In Me.Data.TimeSeries
            timeseries.Enable = (timeseries.NumDataPointsForRun > 0)
        Next
        Me.RefreshTimeSeriesGrid()
    End Sub

    Private Sub OnEnableFishedGroupTimeSeries(sender As Object, e As EventArgs) Handles m_btnEnableFishedGroupTimeSeries.Click
        For Each timeseries As cEmissionTimeSeries In Me.Data.TimeSeries
            If (timeseries.Enable) Then
                Dim group As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(timeseries.Group)
                timeseries.Enable = (timeseries.NumDataPointsForRun > 0) And group.IsFished
            End If
        Next
        Me.RefreshTimeSeriesGrid()
    End Sub

    Private Sub OnTimeSeriesEnabledStateChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvTrends.CellValueChanged

        ' Do not fire events while initializing
        If (Me.Engine Is Nothing) Or (Me.InUpdate) Then Return

        Dim timeseries As cEmissionTimeSeries = DirectCast(Me.m_dgvTrends.Rows(e.RowIndex).Tag, cEmissionTimeSeries)
        Dim val As Boolean = CBool(Me.m_dgvTrends(Me.m_colSettingsMaxEffect.Index, e.RowIndex).Value)
        timeseries.Enable = val

        Me.UpdateControls()

    End Sub

    Private Sub OnRuleSettingsChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvRuleData.CellValueChanged

        ' Do not fire events while initializing
        If (Me.Engine Is Nothing) Or (Me.InUpdate) Then Return

        Dim rule As cEmissionRule = DirectCast(Me.m_dgvRuleData.Rows(e.RowIndex).Tag, cEmissionRule)
        Dim val As Object = Me.m_dgvRuleData(e.ColumnIndex, e.RowIndex).Value

        ' Using hard-coded column indices can easily break when the grid is localized - if ever
        Select Case e.ColumnIndex
            Case 2 ' Enable
                rule.Enable = CBool(val)
            Case 3 ' Protection type
                rule.Protection = DirectCast(val, eProtectionType)

        End Select

        Me.Data.SaveModelChanges()
        Me.UpdateControls()

    End Sub

    Private Sub OnDirtyHackToMakeComboBoxCellCommitItsStuffAarghAarghAargh(sender As Object, e As EventArgs) _
        Handles m_dgvRuleData.CurrentCellDirtyStateChanged

        ' OMG
        If (Me.m_dgvRuleData.IsCurrentCellDirty) Then
            Me.m_dgvRuleData.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If

    End Sub

#End Region ' Events

#Region " Internals "

    ''' <summary>(Wo)men-at-work.</summary>
    Private Property InUpdate As Boolean = False

    Private Sub RefreshTimeSeriesGrid()

        If (Me.InUpdate) Then Return
        Me.InUpdate = True

        Me.m_tbxTimeSeriesFile.Text = Me.Data.TrendFileName

        Dim entries As ICollection(Of cEmissionTimeSeries) = Me.Data.TimeSeries
        Me.m_dgvTrends.Rows.Clear()
        For i As Integer = 0 To entries.Count - 1
            Dim e As cEmissionTimeSeries = entries(i)
            Dim img As Image = If(e.Enable, SharedResources.OK, SharedResources.Critical)
            Dim iRow As Integer = Me.m_dgvTrends.Rows.Add(New Object() {e.Group, e.Target, e.ToString, img, e.Enable})
            Me.m_dgvTrends.Rows(iRow).Tag = e
        Next

        Me.InUpdate = False
        Me.UpdateControls()

    End Sub

    Private Sub RefreshRuleGrid()
        ' Populate rule grid
        Me.m_dgvRuleData.Rows.Clear()
        For i As Integer = 1 To Me.Core.nMPAs
            Dim rule As cEmissionRule = Data.RuleTrends(i - 1)
            Dim iRow As Integer = Me.m_dgvRuleData.Rows.Add(New Object() {rule.Index, rule.Name, rule.Enable, rule.Protection})
            Me.m_dgvRuleData.Rows(iRow).Tag = rule
        Next
    End Sub

    Private Sub UpdateStatus(pb As PictureBox, lb As Label, test As Boolean, strSucces As String, strFail As String)
        Dim sg = Me.StyleGuide
        pb.Image = If(test, SharedResources.OK, SharedResources.Warning)
        lb.Text = If(test, strSucces, strFail)
        lb.ForeColor = If(test, sg.ApplicationColor(eApplicationColorType.DEFAULT_TEXT), sg.ApplicationColor(eApplicationColorType.FAILEDVALIDATION_TEXT))
    End Sub

    Private Function NumMPAs() As Integer
        Return Me.Data.RuleTrends.Count
    End Function

    Private Function NumEnabledRules() As Integer
        Dim n As Integer = 0
        For Each md As cEmissionRule In Me.Data.RuleTrends
            If (md.Enable) Then
                n += 1
            End If
        Next
        Return n
    End Function

    Private Function HasTrends() As Boolean
        Return (Me.Data.TimeSeries.Count + Me.Data.RuleTrends.Count > 0)
    End Function

    Private Function HasTrendData() As Boolean
        If Not Me.HasTrends() Then Return False
        Dim bHasData As Boolean = False
        For Each tr As cEmissionTimeSeries In Me.Data.TimeSeries
            bHasData = bHasData Or (tr.NumDataPointsForRun() > 0)
        Next
        Return bHasData Or (Me.Data.RuleTrends.Count > 0)
    End Function

    Private Sub m_tsbnRecalc_Click(sender As Object, e As EventArgs)
        InUpdate = True
        RefreshRuleGrid()
        InUpdate = False
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles m_rbApplyIsRelative.CheckedChanged

    End Sub

#End Region ' Internals

End Class