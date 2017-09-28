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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Collections.Specialized
Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class ucEcoEngineerConfigUI
    Implements IOptionsPage
    Implements IUIElement

#Region " Private vars "

    Private m_ds As cComplexityDataset = Nothing
    Private m_bInUpdate As Boolean = False
    Private m_rules() As cComplexityRule = Nothing

    Private m_shapePreview As cShapePreview = Nothing

    Private m_fpFN As cEwEFormatProvider = Nothing
    Private m_fpA As cEwEFormatProvider = Nothing
    Private m_fpB As cEwEFormatProvider = Nothing
    Private m_fpC As cEwEFormatProvider = Nothing

#End Region ' Private vars 

#Region " Construction and destruction "

    Public Sub New(ds As cComplexityDataset)

        Me.InitializeComponent()

        Me.m_ds = ds
        Me.m_shapePreview = New cShapePreview()
        Me.DoubleBuffered = True

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If

            If (Me.m_fpFN IsNot Nothing) Then
                Me.OnFormClosed()
            End If

        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

#End Region ' Construction and destruction

#Region " Form overloads "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to initialize the form content right before the form gets displayed
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_bInUpdate = True

        Me.m_tbxName.Text = Me.m_ds.DisplayName
        Me.m_tbxDescription.Text = Me.m_ds.DataDescription

        ReDim Me.m_rules(Me.UIContext.Core.nGroups)

        Me.m_fpFN = New cEwEFormatProvider(Me.UIContext, Me.m_tbxFunctionName, GetType(String))
        Me.m_fpA = New cEwEFormatProvider(Me.UIContext, Me.m_tbxA, GetType(Single))
        Me.m_fpB = New cEwEFormatProvider(Me.UIContext, Me.m_tbxB, GetType(Single))
        Me.m_fpC = New cEwEFormatProvider(Me.UIContext, Me.m_tbxC, GetType(Single))

        AddHandler Me.m_fpFN.OnValueChanged, AddressOf OnCurveParamsChanged
        AddHandler Me.m_fpA.OnValueChanged, AddressOf OnCurveParamsChanged
        AddHandler Me.m_fpB.OnValueChanged, AddressOf OnCurveParamsChanged
        AddHandler Me.m_fpC.OnValueChanged, AddressOf OnCurveParamsChanged

        ' Create Template complexity rules
        ' Saachi: Template equations added (Marcus Island ecosystem engineers)
        Me.AddPreDefinedComplexityRule("Mytilus galloprovincialis", My.Resources.MASK_HIGHSHORE, -0.00003, 0.9206, -290.8)
        Me.AddPreDefinedComplexityRule("Mytilus galloprovincialis", My.Resources.MASK_LOWSHORE, -0.00004, 1.2068, 1756.3)
        Me.AddPreDefinedComplexityRule("Aulacomya atra", My.Resources.MASK_HIGHSHORE, -0.000006, 0.6221, 851.8)
        Me.AddPreDefinedComplexityRule("Aulacomya atra", My.Resources.MASK_LOWSHORE, -0.00002, 0.6337, 5683.1)
        Me.AddPreDefinedComplexityRule("Semimytilus algosus", My.Resources.MASK_HIGHSHORE, -0.000005, 0.1972, +5293.8)
        Me.AddPreDefinedComplexityRule("Balanus glandula", My.Resources.MASK_HIGHSHORE, -0.00002, 0.3201, 1224.6)

        ' Load custom pre-defined datasets from settings
        Me.LoadPreDefinedRules()

        ' Fill group list, and create a rule for each group to the UI has something to work with
        For i As Integer = 1 To Me.UIContext.Core.nGroups
            Me.m_clbGroups.Items.Add(Me.UIContext.Core.EcoPathGroupInputs(i).Name)
            Me.m_rules(i) = New cComplexityRule()
            Me.m_rules(i).Group = i
        Next
        Me.m_clbGroups.SelectedIndex = 0

        ' Make sure to use any rules in the dataset, and check the groups that these rules apply to
        For Each r As cComplexityRule In Me.m_ds.Rules
            If (r.Group > 0) Then
                Me.m_clbGroups.SetItemChecked(r.Group - 1, True)
                Me.m_rules(r.Group) = r
            End If
        Next

        ' Prettify
        Me.m_btnDeletePredefined.Text = ""
        Me.m_btnDeletePredefined.Image = ScientificInterfaceShared.My.Resources.DeleteHS
        Me.m_btnAddPredefined.Text = ""
        Me.m_btnAddPredefined.Image = ScientificInterfaceShared.My.Resources.AddTableHS

        Me.m_sketchpad.UIContext = Me.UIContext
        Me.m_sketchpad.Shape = Me.m_shapePreview

        Me.m_bInUpdate = False

        ' Connect to sponsors
        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmd As cCommand = cmdh.GetCommand(cBrowserCommand.COMMAND_NAME)

        cmd.AddControl(Me.m_pbEII, "http://ecopathinternational.org")
        cmd.AddControl(Me.m_pbMare, "http://ma-re.uct.ac.za/")

    End Sub

    Protected Sub OnFormClosed()

        Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
        Dim cmd As cCommand = cmdh.GetCommand(cBrowserCommand.COMMAND_NAME)

        cmd.RemoveControl(Me.m_pbEII)
        cmd.RemoveControl(Me.m_pbMare)

        RemoveHandler Me.m_fpFN.OnValueChanged, AddressOf OnCurveParamsChanged
        RemoveHandler Me.m_fpA.OnValueChanged, AddressOf OnCurveParamsChanged
        RemoveHandler Me.m_fpB.OnValueChanged, AddressOf OnCurveParamsChanged
        RemoveHandler Me.m_fpC.OnValueChanged, AddressOf OnCurveParamsChanged

        Me.m_fpFN.Release()
        Me.m_fpFN = Nothing
        Me.m_fpA.Release()
        Me.m_fpA = Nothing
        Me.m_fpB.Release()
        Me.m_fpB = Nothing
        Me.m_fpC.Release()
        Me.m_fpC = Nothing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to redraw the graph when the form has been resized
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnResize(e As System.EventArgs)
        MyBase.OnResize(e)
        Me.m_sketchpad.Invalidate()
    End Sub

#End Region ' Form overloads

#Region " Options page "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Write the content of the UI to the dataset.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function Apply() As IOptionsPage.eApplyResultType _
        Implements IOptionsPage.Apply

        Me.m_ds.DisplayName = Me.m_tbxName.Text
        Me.m_ds.DataDescription = Me.m_tbxDescription.Text
        Me.m_ds.Rules.Clear()
        For Each i As Integer In Me.m_clbGroups.CheckedIndices
            Me.m_ds.Rules.Add(Me.m_rules(i + 1))
        Next
        Return IOptionsPage.eApplyResultType.Success

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' External check, replies whether the UI is ready to apply when all
    ''' expected data is in place.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return Not String.IsNullOrWhiteSpace(Me.m_tbxName.Text)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' There are no valid defaults to set.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event to notify the world that the user has made modifications to the UI.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event OnChanged(sender As IOptionsPage, args As System.EventArgs) _
        Implements IOptionsPage.OnChanged

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set default values in the form. Since there are none, this method will not 
    ''' do anything.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        ' Nothing to do here
    End Sub

#End Region ' Options page

#Region " Public config "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide the EwE UI Context to this form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property UIContext As cUIContext _
        Implements IUIElement.UIContext

#End Region ' Public config

#Region " Control events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event, sent when the name or description text fields have been changed in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnDatasetDescriptiveChanged(sender As Object, e As System.EventArgs) _
        Handles m_tbxName.TextChanged, m_tbxDescription.TextChanged

        If Me.m_bInUpdate Then Return

        Me.NotifyWorld()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event, sent when a Template rule is selected in the form
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnPreDefinedRuleSelectionChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbPreDefinedRules.SelectedIndexChanged

        If Me.m_bInUpdate Then Return
        If (Me.m_cmbPreDefinedRules.SelectedItem Is Nothing) Then Return

        ' Copy rule data
        Dim ruleTemplate As cComplexityRule = DirectCast(Me.m_cmbPreDefinedRules.SelectedItem, cComplexityRule)
        Dim ruleSelected As cComplexityRule = Me.SelectedRule()

        ruleSelected.Name = ruleTemplate.Name
        ruleSelected.A = ruleTemplate.A
        ruleSelected.B = ruleTemplate.B
        ruleSelected.C = ruleTemplate.C

        Me.UpdateControls()
        Me.UpdateGraph()

    End Sub

    Private Sub OnAddPreDefinedRule(sender As System.Object, e As System.EventArgs) _
        Handles m_btnAddPredefined.Click
        Try
            Me.SaveComplexityRule(CStr(Me.m_fpFN.Value), CSng(Me.m_fpA.Value), CSng(Me.m_fpB.Value), CSng(Me.m_fpC.Value))
        Catch ex As Exception

        End Try
    End Sub

    Private Sub OnDeletePreDefinedRule(sender As System.Object, e As System.EventArgs) _
        Handles m_btnDeletePredefined.Click
        Me.DeletePreDefinedRule()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ecent, sent when A, B or C value has been changed in the form.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnCurveParamsChanged(sender As System.Object, e As System.EventArgs)

        If Me.m_bInUpdate Then Return

        Me.m_bInUpdate = True
        Try
            Dim r As cComplexityRule = Me.SelectedRule()
            r.Name = CStr(Me.m_fpFN.Value)
            r.A = CSng(Me.m_fpA.Value)
            r.B = CSng(Me.m_fpB.Value)
            r.C = CSng(Me.m_fpC.Value)
            Me.UpdateGraph()
        Catch ex As Exception

        End Try
        Me.m_bInUpdate = False
        Me.UpdateControls()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event, sent when a group is in the process of being assigned (or unassigned) as an architect
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnGroupCheckChanged(sender As Object, e As System.Windows.Forms.ItemCheckEventArgs) _
        Handles m_clbGroups.ItemCheck
        Try
            ' This event is tricky. The check state is not set yet; it will be set after this call completes.
            ' Therefore, delay the response until the check state has been completed, and then update the UI.
            ' Lovely, no?
            Me.BeginInvoke(New MethodInvoker(AddressOf UpdateControls))
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event, sent when a group is selected
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnSelectGroup(sender As System.Object, e As System.EventArgs) _
        Handles m_clbGroups.SelectedIndexChanged
        Try
            Me.UpdateGraph()
            Me.UpdateControls()
        Catch ex As Exception

        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Paint rotated Y axis label.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub OnPaintYAxis(sender As Object, e As System.Windows.Forms.PaintEventArgs) _
        Handles m_lblYAxis.Paint

        Dim g As Graphics = e.Graphics
        Dim fmt As New StringFormat()

        fmt.Alignment = StringAlignment.Center
        fmt.LineAlignment = StringAlignment.Center
        fmt.Trimming = StringTrimming.None

        Dim rc As New Rectangle(0, 0, m_lblYAxis.Height, m_lblYAxis.Width)
        Using br As New SolidBrush(Me.BackColor)
            g.FillRectangle(br, Me.m_lblYAxis.ClientRectangle)
        End Using

        g.TranslateTransform(0, rc.Width)
        g.RotateTransform(-90)

        Using br As New SolidBrush(Me.ForeColor)
            g.DrawString(Me.m_lblYAxis.Text, Me.m_lblYAxis.Font, br, rc, fmt)
        End Using
        g.ResetTransform()

    End Sub

#End Region ' Control events

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a pre-defined complexity rule
    ''' </summary>
    ''' <param name="strSpeciesName">Name of the speies.</param>
    ''' <param name="strMask">The mask to use.</param>
    ''' <param name="A">A parameter.</param>
    ''' <param name="B">B parameter.</param>
    ''' <param name="C">C parameter.</param>
    ''' -----------------------------------------------------------------------
    Private Sub AddPreDefinedComplexityRule(ByVal strSpeciesName As String, ByVal strMask As String, _
                                            ByVal A As Single, ByVal B As Single, ByVal C As Single)

        Dim strRuleName As String = strSpeciesName
        strSpeciesName = cStringUtils.ToTitleCase(strSpeciesName)

        If Not String.IsNullOrWhiteSpace(strMask) Then
            strRuleName = cStringUtils.Localize(strMask, strSpeciesName)
        End If

        Me.m_cmbPreDefinedRules.Items.Add(New cComplexityRuleDefault(strRuleName, A, B, C))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the current function as a pre-defined rule
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="A"></param>
    ''' <param name="B"></param>
    ''' <param name="C"></param>
    ''' -----------------------------------------------------------------------
    Private Sub SaveComplexityRule(strName As String, A As Single, B As Single, C As Single)

        For Each r As cComplexityRule In Me.m_cmbPreDefinedRules.Items
            If (String.Compare(strName, r.Name, True) = 0) Then
                If (r.IsDefault) Then
                    Dim core As cCore = Me.UIContext.Core
                    Dim msg As New cMessage(My.Resources.ERROR_DEFAULT_EXISTS, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
                    core.Messages.SendMessage(msg)
                    Return
                Else
                    Me.m_cmbPreDefinedRules.Items.Remove(r)
                    Exit For
                End If
            End If
        Next

        ' Now add the rule
        Me.m_cmbPreDefinedRules.Items.Add(New cComplexityRule(strName, A, B, C))
        ' Try to select it (a bit of a round-trip but hey)
        Me.UpdatePreDefinedSelectedRule()

        ' Store settings 
        Me.SavePreDefinedRules()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delete the pre-defined rule that is currently selected
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub DeletePreDefinedRule()

        Dim item As Object = Me.m_cmbPreDefinedRules.SelectedItem
        If (item Is Nothing) Then Return
        If (TypeOf item Is cComplexityRuleDefault) Then Return
        Me.m_cmbPreDefinedRules.Items.Remove(item)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the rule for the currently selected group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function SelectedRule() As cComplexityRule
        Dim i As Integer = Me.m_clbGroups.SelectedIndex
        Return Me.m_rules(i + 1)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the graph from the selected rule.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateGraph()

        Dim rule As cComplexityRule = Me.SelectedRule()
        Dim iGroup As Integer = Me.m_clbGroups.SelectedIndex()
        Dim bIsArchitect As Boolean = False
        Dim sScale As Single = CSng(Me.m_sketchpad.MaxXValue / Me.m_shapePreview.nPoints)

        If iGroup >= 0 Then bIsArchitect = Me.m_clbGroups.GetItemChecked(iGroup)

        For x As Integer = 0 To Me.m_shapePreview.nPoints - 1
            Me.m_shapePreview.ShapeData(x) = If(bIsArchitect, rule.ArchitecturalComplexity(x * sScale), 0)
        Next

        Me.m_shapePreview.Name = rule.Name
        Me.m_sketchpad.Invalidate()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update state and content of UI controls
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateControls()

        Dim rule As cComplexityRule = Me.SelectedRule()
        Dim ruleTemplate As cComplexityRule = DirectCast(Me.m_cmbPreDefinedRules.SelectedItem, cComplexityRule)
        Dim iGroup As Integer = Me.m_clbGroups.SelectedIndex()
        Dim bIsArchitect As Boolean = False
        Dim bCanAddDefault As Boolean = False
        Dim bCanDeleteSelectedTemplate As Boolean = False

        If (ruleTemplate IsNot Nothing) Then
            bCanDeleteSelectedTemplate = (Not TypeOf ruleTemplate Is cComplexityRuleDefault)
        End If

        If iGroup >= 0 Then bIsArchitect = Me.m_clbGroups.GetItemChecked(iGroup)

        Me.m_bInUpdate = True

        If (bIsArchitect = True) And (rule IsNot Nothing) Then
            Me.m_fpFN.Style = cStyleGuide.eStyleFlags.OK
            Me.m_fpFN.Value = rule.Name
            Me.m_fpA.Style = cStyleGuide.eStyleFlags.OK
            Me.m_fpA.Value = rule.A
            Me.m_fpB.Style = cStyleGuide.eStyleFlags.OK
            Me.m_fpB.Value = rule.B
            Me.m_fpC.Style = cStyleGuide.eStyleFlags.OK
            Me.m_fpC.Value = rule.C
        Else
            Me.m_fpFN.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
            Me.m_fpA.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
            Me.m_fpB.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
            Me.m_fpC.Style = cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null
        End If

        Me.m_cmbPreDefinedRules.Enabled = bIsArchitect

        Me.m_btnAddPredefined.Enabled = bIsArchitect And Not String.IsNullOrWhiteSpace(CStr(Me.m_fpFN.Value))
        Me.m_btnDeletePredefined.Enabled = bIsArchitect And bCanDeleteSelectedTemplate

        Me.UpdatePreDefinedSelectedRule()

        Me.m_bInUpdate = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Try to select a Template shape from the value in the A, B and C parameters
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdatePreDefinedSelectedRule()

        Dim strName As String = CStr(Me.m_fpFN.Value)
        Dim A As Single = CSng(Me.m_fpA.Value)
        Dim B As Single = CSng(Me.m_fpB.Value)
        Dim C As Single = CSng(Me.m_fpC.Value)
        Dim sel As Object = Nothing

        For Each item As Object In Me.m_cmbPreDefinedRules.Items
            Dim r As cComplexityRule = DirectCast(item, cComplexityRule)
            If (String.Compare(r.Name, strName, True) = 0) And (r.A = A) And (r.B = B) And (r.C = C) Then
                sel = item
            End If
        Next

        Me.m_cmbPreDefinedRules.SelectedItem = sel

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell the world that the UI has changed
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub NotifyWorld()
        Try
            RaiseEvent OnChanged(Me, New System.EventArgs())
        Catch ex As Exception
            ' Kaboom
        End Try
    End Sub

    Private Sub SavePreDefinedRules()

        My.Settings.PreDefined = New StringCollection()

        For Each r As cComplexityRule In Me.m_cmbPreDefinedRules.Items
            If Not r.IsDefault Then
                Dim strRule As String = String.Format("{0}|{1}|{2}|{3}", _
                                                      r.Name.Replace("|", "-"), _
                                                      cStringUtils.FormatSingle(r.A), _
                                                      cStringUtils.FormatSingle(r.B), _
                                                      cStringUtils.FormatSingle(r.C))
                My.Settings.PreDefined.Add(strRule)
            End If
        Next
        My.Settings.Save()

    End Sub

    Private Sub LoadPreDefinedRules()

        If (My.Settings.PreDefined Is Nothing) Then Return

        For Each strPredefined As String In My.Settings.PreDefined
            Dim bits As String() = strPredefined.Split("|"c)
            If (bits.Length = 4) Then
                Dim r As New cComplexityRule()
                r.Name = bits(0)
                r.A = cStringUtils.ConvertToSingle(bits(1))
                r.B = cStringUtils.ConvertToSingle(bits(2))
                r.C = cStringUtils.ConvertToSingle(bits(3))
                Me.m_cmbPreDefinedRules.Items.Add(r)
            End If
        Next

    End Sub

#End Region ' Internals

End Class
