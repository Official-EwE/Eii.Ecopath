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
        cmd.AddControl(Me.m_pbSafenet, "http://www.criobe.pf/recherche/safenet/")
        cmd.AddControl(Me.m_pbCSIC, "http://www.icm.csic.es")
        cmd.AddControl(Me.m_pbEII, "http://www.ecopathinternational.org")

        Me.m_btnTrendLoad.Image = SharedResources.openHS
        Me.m_lblVersion.Text = My.Resources.VERSION

        Me.m_cbEnabled.Checked = Me.Engine.Enabled

        Select Case Me.Data.TargetType
            Case eTargetType.Region
                Me.m_rbApplyToRegions.Checked = True
            Case eTargetType.MPA
                Me.m_rbApplyToMPAs.Checked = True
            Case Else
                Debug.Assert(False)
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
        cmd.RemoveControl(Me.m_pbCSIC)
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

            Case Else
                Debug.Assert(False)

        End Select
        Me.m_colTrendTarget.HeaderText = strTarget

        Me.UpdateStatus(Me.m_pbHasMetadata, Me.m_lblHasMetadata, Me.NumEnabledRules > 0 And Me.NumMPAs > 0, cStringUtils.Localize(My.Resources.CHECK_RULES_ENABLED, Me.NumEnabledRules), My.Resources.CHECK_RULES_DISABLED)
        If Not HasTrends() Then
            Me.UpdateStatus(Me.m_pbHasTrends, Me.m_lblHasTrends, False, "", My.Resources.CHECK_TRENDS_MISSING)
        Else
            Me.UpdateStatus(Me.m_pbHasTrends, Me.m_lblHasTrends, Me.HasTrendData(), My.Resources.CHECK_TRENDS_OK, My.Resources.CHECK_TRENDS_OUTOFRANGE)
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
        Handles m_rbApplyToRegions.CheckedChanged, m_rbApplyToMPAs.CheckedChanged

        If (Me.Engine Is Nothing) Then Return
        If (Me.InUpdate) Then Return

        If Me.m_rbApplyToRegions.Checked Then Me.Data.TargetType = eTargetType.Region
        If Me.m_rbApplyToMPAs.Checked Then Me.Data.TargetType = eTargetType.MPA

        ' Need to refresh validation status
        Me.RefreshModelTrendGrid()

    End Sub

    Private Sub OnDataTypeChanged(sender As Object, e As EventArgs) _
        Handles m_rbApplyIsRelative.CheckedChanged, m_rbApplyIsAbsolute.CheckedChanged

        If (Me.Engine Is Nothing) Then Return
        If (Me.InUpdate) Then Return

        If Me.m_rbApplyIsRelative.Checked Then Me.Data.ApplicationType = eApplicationType.Relative
        If Me.m_rbApplyIsAbsolute.Checked Then Me.Data.ApplicationType = eApplicationType.Absolute

    End Sub

    Private Sub OnLoadTrends(sender As Object, e As EventArgs) _
        Handles m_btnTrendLoad.Click

        Dim ofd As OpenFileDialog = cEwEFileDialogHelper.OpenFileDialog(My.Resources.PROMPT_SELECTTRENDFILE, "", SharedResources.FILEFILTER_CSV)
        ofd.Multiselect = True
        If (ofd.ShowDialog() = DialogResult.OK) Then
            Me.Data.LoadTrends(ofd.FileNames)
            Me.RefreshModelTrendGrid()
        End If

    End Sub

    Private Sub OnResetTrends(sender As Object, e As EventArgs) _
        Handles m_btnTrendReset.Click

        Me.Data.Clear()
        Me.RefreshModelTrendGrid()

    End Sub

    Private Sub OnMagicButtonClicked(sender As Object, e As EventArgs) Handles m_btnTrendMagic.Click

        Dim util As New dlgBiomassEmitterTimeSeriesBuilder(Me.UIContext)
        If (util.ShowDialog(Me.UIContext.FormMain) = DialogResult.OK) Then
            If (util.LoadOnSave) Then
                Me.Data.LoadTrends(New String() {util.FileName})
                Me.RefreshModelTrendGrid()
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

    Private Sub m_btnTrendNone_Click(sender As Object, e As EventArgs) Handles m_btnTrendNone.Click
        For Each trend As cModelTrend In Me.Data.ModelTrends
            trend.Enable = False
        Next
        Me.RefreshModelTrendGrid()
    End Sub

    Private Sub m_btnTrendAll_Click(sender As Object, e As EventArgs) Handles m_btnTrendAll.Click
        For Each trend As cModelTrend In Me.Data.ModelTrends
            trend.Enable = (trend.NumTrendPointsForRun > 0)
        Next
        Me.RefreshModelTrendGrid()
    End Sub

    Private Sub m_btnTrendFished_Click(sender As Object, e As EventArgs) Handles m_btnTrendFished.Click
        For Each trend As cModelTrend In Me.Data.ModelTrends
            If (trend.CanRun) Then
                Dim group As cEcoPathGroupInput = Me.Core.EcopathGroupInputs(trend.Group)
                trend.Enable = (trend.NumTrendPointsForRun > 0) And group.IsFished
            End If
        Next
        Me.RefreshModelTrendGrid()
    End Sub

    Private Sub OnTrendsEnabled(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvTrends.CellValueChanged

        ' Do not fire events while initializing
        If (Me.Engine Is Nothing) Or (Me.InUpdate) Then Return

        Dim trend As cModelTrend = DirectCast(Me.m_dgvTrends.Rows(e.RowIndex).Tag, cModelTrend)
        Dim val As Boolean = CBool(Me.m_dgvTrends(Me.m_colSettingsMaxEffect.Index, e.RowIndex).Value)
        trend.Enable = val

        Me.UpdateControls()

    End Sub

    Private Sub OnMetaDataChanged(sender As Object, e As DataGridViewCellEventArgs) _
        Handles m_dgvRuleData.CellValueChanged

        ' Do not fire events while initializing
        If (Me.Engine Is Nothing) Or (Me.InUpdate) Then Return

        Dim rule As cRuleTrend = DirectCast(Me.m_dgvRuleData.Rows(e.RowIndex).Tag, cRuleTrend)
        Dim val As Object = Me.m_dgvRuleData(e.ColumnIndex, e.RowIndex).Value

        ' Using hard-coded column indices can easily break when the grid is localized - if ever
        Select Case e.ColumnIndex
            Case 2 ' Enable
                rule.CanRun = CBool(val)
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

    Private Sub RefreshModelTrendGrid()

        If (Me.InUpdate) Then Return
        Me.InUpdate = True

        Me.m_tbxTrendFile.Text = Me.Data.TrendFileName

        Dim entries As ICollection(Of cModelTrend) = Me.Data.ModelTrends
        Me.m_dgvTrends.Rows.Clear()
        For i As Integer = 0 To entries.Count - 1
            Dim e As cModelTrend = entries(i)
            Dim img As Image = If(e.CanRun, SharedResources.OK, SharedResources.Critical)
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
            Dim rule As cRuleTrend = Data.RuleTrends(i - 1)
            Dim iRow As Integer = Me.m_dgvRuleData.Rows.Add(New Object() {rule.Index, rule.Name, rule.CanRun, rule.Protection})
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
        For Each md As cRuleTrend In Me.Data.RuleTrends
            If (md.CanRun) Then n += 1
        Next
        Return n
    End Function

    Private Function HasTrends() As Boolean
        Return (Me.Data.ModelTrends.Count + Me.Data.RuleTrends.Count > 0)
    End Function

    Private Function HasTrendData() As Boolean
        If Not Me.HasTrends() Then Return False
        Dim bHasData As Boolean = False
        For Each tr As cModelTrend In Me.Data.ModelTrends
            bHasData = bHasData Or (tr.NumTrendPointsForRun() > 0)
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