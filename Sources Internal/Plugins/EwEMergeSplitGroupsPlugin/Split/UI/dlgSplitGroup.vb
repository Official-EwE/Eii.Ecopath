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
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Ecopath
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' <summary>
''' Dialog form class to guide users in the Ecopath group splitting process.
''' </summary>
Public Class dlgSplitGroup

    Private m_uic As cUIContext = Nothing
    Private m_engine As cEcopathSplitGroup = Nothing
    Private m_bInUpdate As Boolean = True

    Private WithEvents m_fpN1 As cEwEFormatProvider = Nothing
    Private WithEvents m_fpN2 As cEwEFormatProvider = Nothing

    Private WithEvents m_fpB1 As cEwEFormatProvider = Nothing
    Private WithEvents m_fpB2 As cEwEFormatProvider = Nothing

    Private WithEvents m_fpA1 As cEwEFormatProvider = Nothing
    Private WithEvents m_fpA2 As cEwEFormatProvider = Nothing

    Private m_biomass As Single = 0
    Private m_biomasssource As eBiomassSource = eBiomassSource.NotSet

    Private Enum eBiomassSource As Integer
        NotSet = 0
        Manual
        Stanza
        Taxonomy
    End Enum

    Public Sub New(uic As cUIContext, engine As cEcopathSplitGroup)

        Me.m_uic = uic
        Me.m_engine = engine

        Me.InitializeComponent()

    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Debug.Assert(Me.m_uic.Core.StateMonitor.HasEcopathRan)

        Dim core As cCore = Me.m_uic.Core

        For i As Integer = 1 To core.nGroups
            Dim grp As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
            If (grp.BiomassAreaInput > 0) Then
                Me.m_cmbSource.Items.Add(core.EcoPathGroupInputs(i))
            End If
        Next

        Me.m_fpN1 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxSplit1, GetType(String))
        Me.m_fpN2 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxSplit2, GetType(String))

        Me.m_fpB1 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxB1, GetType(Single))
        Me.m_fpB2 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxB2, GetType(Single))

        Me.m_fpA1 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxAge1, GetType(Integer))
        Me.m_fpA2 = New cEwEFormatProvider(Me.m_uic, Me.m_tbxAge2, GetType(Integer))

        Me.m_bInUpdate = False
        Me.B1Ratio = 0.5

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
        MyBase.OnFormClosed(e)

        Me.m_fpN1.Release()
        Me.m_fpN2.Release()

        Me.m_fpB1.Release()
        Me.m_fpB2.Release()

        Me.m_fpA1.Release()
        Me.m_fpA2.Release()

    End Sub

#End Region ' Overrides 

#Region " Events "

    Private Sub OnSourceSelected(sender As Object, e As EventArgs) _
        Handles m_cmbSource.SelectedIndexChanged

        Dim grp As cEcoPathGroupInput = Me.SelectedSource()
        Dim core As cCore = Me.m_uic.Core

        If (grp IsNot Nothing) Then
            Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(grp.Index)
            Me.m_biomass = grpOut.Biomass

            ' Set biomass source type
            If grp.isMultiStanza Then
                Me.m_biomasssource = eBiomassSource.Stanza
                Dim stanza As cStanzaGroup = Me.SelectedStanza()
                Debug.Assert(stanza IsNot Nothing)
                Dim iLifeStage As Integer = stanza.iLifeStage(grp.Index)
                Me.m_fpA1.Value = stanza.StartAge(iLifeStage)
                Me.m_fpA2.Value = stanza.StartAge(iLifeStage)
            ElseIf grp.NTaxon > 0 Then
                Me.m_biomasssource = eBiomassSource.Taxonomy
            Else
                Me.m_biomasssource = eBiomassSource.Manual
            End If

            Me.m_fpN1.Value = grp.Name
            Me.m_fpN2.Value = grp.Name
        Else
            Me.m_biomass = cCore.NULL_VALUE
            Me.m_biomasssource = eBiomassSource.NotSet
        End If

        Me.PopulateStanzaList()
        Me.OnSliderValueChanged(Me, Nothing)

        Me.UpdateControls()

    End Sub

    Private Sub OnFormatGroupItem(sender As Object, e As Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbSource.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
            Dim grp As cEcoPathGroupInput = DirectCast(e.ListItem, cEcoPathGroupInput)

            If (Not grp.Disposed) Then
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        Catch ex As Exception
            ' mmm
        End Try

    End Sub

    Private Sub OnFormatTaxonItem(sender As Object, e As Windows.Forms.ListControlConvertEventArgs) _
        Handles m_lbxTaxa1.Format, m_lbxTaxa2.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
            Dim taxon As cTaxon = DirectCast(e.ListItem, cTaxon)

            If (Not taxon.Disposed) Then
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        Catch ex As Exception
            ' mmm
        End Try

    End Sub

    Private Sub OnSplitNameChanged(sender As Object, e As EventArgs) _
        Handles m_fpN2.OnValueChanged

        Me.UpdateControls()

    End Sub

    Private Sub OnSliderValueChanged(sender As Object, e As EventArgs) _
        Handles m_sliderB.ValueChanged

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        Me.B1 = Me.m_biomass * (1 - B1Ratio)
        Me.B2 = Me.m_biomass * B1Ratio

        Me.m_bInUpdate = False

    End Sub

    Private Sub OnB1Changed(sender As Object, e As EventArgs) _
        Handles m_fpB1.OnValueChanged

        If Me.m_bInUpdate Then Return

        Dim b1 As Single = Math.Min(Me.m_biomass, CSng(Me.m_fpB1.Value))
        Me.B1Ratio = (b1 / Me.m_biomass)

    End Sub

    Private Sub OnB2Changed(sender As Object, e As EventArgs) _
        Handles m_fpB2.OnValueChanged

        If Me.m_bInUpdate Then Return

        Dim b2 As Single = Math.Min(Me.m_biomass, CSng(Me.m_fpB2.Value))
        Me.B1Ratio = 1 - (b2 / Me.m_biomass)

    End Sub

    Private Sub OnMoveTaxaToGroup2(sender As Object, e As EventArgs) _
        Handles m_btn2to1.Click
        MoveSelectedTaxa(Me.m_lbxTaxa2, Me.m_lbxTaxa1)
    End Sub

    Private Sub OnMoveTaxaToGroup1(sender As Object, e As EventArgs) _
        Handles m_btn1to2.Click
        MoveSelectedTaxa(Me.m_lbxTaxa1, Me.m_lbxTaxa2)
    End Sub

    Private Sub OnTaxaSelectionChanged(sender As Object, e As EventArgs) _
        Handles m_lbxTaxa1.SelectedIndexChanged, m_lbxTaxa2.SelectedIndexChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnClickGeomar(sender As Object, e As EventArgs) _
        Handles m_pbLogo.Click

        Me.OpenLink("http://www.geomar.de")

    End Sub

    Private Sub OnOK(sender As Object, e As EventArgs) _
        Handles m_btnOK.Click

        Dim bSucces As Boolean = False
        Dim grp As cEcoPathGroupInput = Me.SelectedSource()

        If (grp Is Nothing) Then Debug.Assert(False) : Return

        If (Me.m_biomasssource = eBiomassSource.Stanza) Then
            bSucces = Me.m_engine.SplitLifeStage(grp.Index, Me.Name2, Me.Age2)
        Else
            Dim taxa As New List(Of Integer)
            bSucces = Me.m_engine.SplitGroup(grp.Index, Me.Name2, B1, B2, Me.Taxa(Me.m_lbxTaxa2, False))
        End If

        If bSucces Then
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub OnCancel(sender As Object, e As EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedSource() As cEcoPathGroupInput

        Dim item As Object = Me.m_cmbSource.SelectedItem

        If (item Is Nothing) Then Return Nothing
        If (Not TypeOf (item) Is cEcoPathGroupInput) Then Return Nothing
        Return DirectCast(item, cEcoPathGroupInput)

    End Function

    Private Function SelectedStanza() As cStanzaGroup

        Dim grp As cEcoPathGroupInput = Me.SelectedSource()
        If (grp Is Nothing) Then Return Nothing
        If (Not grp.isMultiStanza) Then Return Nothing

        Return Me.m_uic.Core.StanzaGroups(grp.iStanza)

    End Function

    Private Property B1 As Single
        Get
            Return CSng(Me.m_fpB1.Value)
        End Get
        Set(value As Single)
            Me.m_fpB1.Value = value
        End Set
    End Property

    Private Property B2 As Single
        Get
            Return CSng(Me.m_fpB2.Value)
        End Get
        Set(value As Single)
            Me.m_fpB2.Value = value
        End Set
    End Property

    Private Property B1Ratio As Single
        Get
            Return Me.m_sliderB.Value / 1000.0!
        End Get
        Set(value As Single)
            If (Me.m_bInUpdate) Then Return
            If (Me.B1Ratio <> value) Then
                Me.m_sliderB.Value = CInt(Math.Max(0, Math.Min(1000, (1 - value) * 1000.0!)))
            End If
        End Set
    End Property

    Private Property Name1 As String
        Get
            If (Me.m_fpN1.Value Is Nothing) Then Return ""
            Return CStr(Me.m_fpN1.Value).Trim
        End Get
        Set(value As String)
            Me.m_fpN1.Value = value.Trim
        End Set
    End Property

    Private Property Name2 As String
        Get
            If (Me.m_fpN2.Value Is Nothing) Then Return ""
            Return CStr(Me.m_fpN2.Value).Trim
        End Get
        Set(value As String)
            Me.m_fpN2.Value = value.Trim
        End Set
    End Property

    Private Property Age1 As Integer
        Get
            Return CInt(Me.m_fpN1.Value)
        End Get
        Set(value As Integer)
            Me.m_fpN1.Value = value
        End Set
    End Property

    Private Property Age2 As Integer
        Get
            Return CInt(Me.m_fpN2.Value)
        End Get
        Set(value As Integer)
            Me.m_fpN2.Value = value
        End Set
    End Property

    Private Function Taxa1(bSelectedOnly As Boolean) As cTaxon()

    End Function

    Private Function Taxa(lb As ListBox, bSelectedOnly As Boolean) As cTaxon()

        Dim lTaxa As New List(Of cTaxon)
        Dim coll As ICollection = Nothing
        If (bSelectedOnly) Then coll = lb.SelectedItems Else coll = lb.Items

        For Each item As Object In coll
            If (item IsNot Nothing) Then
                If (TypeOf item Is cTaxon) Then
                    lTaxa.Add(DirectCast(item, cTaxon))
                End If
            End If
        Next
        Return lTaxa.ToArray()

    End Function

    Private Sub PopulateStanzaList()

        Me.m_lbxTaxa1.Items.Clear()
        Me.m_lbxTaxa2.Items.Clear()

        Dim grp As cEcoPathGroupInput = Me.SelectedSource()
        Dim core As cCore = Me.m_uic.Core

        If (grp Is Nothing) Then Return

        For i As Integer = 1 To grp.NTaxon
            Dim taxon As cTaxon = core.Taxon(grp.iTaxon(i))
            Me.m_lbxTaxa1.Items.Add(taxon)
            If (Me.m_biomasssource = eBiomassSource.Stanza) Then Me.m_lbxTaxa2.Items.Add(taxon)
        Next
        Me.RecalcTaxaBiomass()

    End Sub

    Private Sub MoveSelectedTaxa(lbFrom As ListBox, lbTo As ListBox)

        lbFrom.SuspendLayout()
        lbTo.SuspendLayout()

        For Each item In Me.Taxa(lbFrom, True)
            lbFrom.Items.Remove(item)
            lbTo.Items.Add(item)
        Next

        lbFrom.ResumeLayout()
        lbTo.ResumeLayout()

        Me.RecalcTaxaBiomass()

    End Sub

    Private Sub RecalcTaxaBiomass()

        If (Me.m_biomasssource <> eBiomassSource.Taxonomy) Then Return

        Me.B1Ratio = 1 - (Me.BiomassFromTaxa(Me.Taxa(Me.m_lbxTaxa1, False)) / Me.m_biomass)

    End Sub

    Private Function BiomassFromTaxa(taxa As cTaxon()) As Single

        Dim bTot As Single = 0
        For Each taxon As cTaxon In taxa
            bTot += taxon.Proportion() * Me.m_biomass
        Next
        Return bTot

    End Function

    Private Sub UpdateControls()

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Dim bHasSource As Boolean = (Me.SelectedSource IsNot Nothing)
        Dim bHasTargets As Boolean = True ' Validate unique target names
        Dim bCanSplit As Boolean = False

        If (bHasSource) Then
            bCanSplit = Me.m_engine.CanSplitGroups(Me.SelectedSource.Index, Me.Name2)
        End If

        Me.m_sliderB.Enabled = bHasSource
        Me.m_tbxSplit1.Enabled = bHasSource
        Me.m_tbxSplit2.Enabled = bHasSource

        Dim bEditBiomass As Boolean = False
        Dim bEditAges As Boolean = False
        Dim bEditTaxa As Boolean = False

        Select Case Me.m_biomasssource
            Case eBiomassSource.NotSet
                ' NOP
            Case eBiomassSource.Manual
                bEditBiomass = True
            Case eBiomassSource.Stanza
                bEditAges = True
            Case eBiomassSource.Taxonomy
                bEditTaxa = True
        End Select

        Me.m_fpN1.Style = cStyleGuide.eStyleFlags.NotEditable

        Me.m_fpB1.Style = If(bEditBiomass, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)
        Me.m_fpB2.Style = If(bEditBiomass, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)

        Me.m_fpA1.Style = If(bEditAges, cStyleGuide.eStyleFlags.NotEditable, cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null)
        Me.m_fpA2.Style = If(bEditAges, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Null)

        Me.m_btn1to2.Enabled = bEditTaxa And (Me.m_lbxTaxa1.SelectedIndices.Count > 0)
        Me.m_btn2to1.Enabled = bEditTaxa And (Me.m_lbxTaxa2.SelectedIndices.Count > 0)

        Me.m_btnOK.Enabled = bCanSplit

        Me.m_bInUpdate = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Open an external link.
    ''' </summary>
    ''' <param name="strURL">The link to navigate to.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OpenLink(strURL As String)

        Try
            Dim cmd As cBrowserCommand = DirectCast(Me.m_uic.CommandHandler.GetCommand(cBrowserCommand.COMMAND_NAME), cBrowserCommand)
            If (cmd IsNot Nothing) Then
                cmd.Invoke(strURL)
            End If
        Catch ex As Exception
            cLog.Write(ex, "dlgSplitGroup::OpenLink(" & strURL & ")")
        End Try

    End Sub

#End Region ' Internals

End Class