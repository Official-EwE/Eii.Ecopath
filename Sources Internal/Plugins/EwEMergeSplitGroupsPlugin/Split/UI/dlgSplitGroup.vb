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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Ecopath
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Public Class dlgSplitGroup

    Private m_uic As cUIContext = Nothing
    Private m_engine As cEcopathSplitGroup = Nothing
    Private m_bInUpdate As Boolean = True

    Private m_fpN1 As cEwEFormatProvider = Nothing
    Private m_fpN2 As cEwEFormatProvider = Nothing

    Private m_fpB1 As cEwEFormatProvider = Nothing
    Private m_fpB2 As cEwEFormatProvider = Nothing

    Private m_fpA1 As cEwEFormatProvider = Nothing
    Private m_fpA2 As cEwEFormatProvider = Nothing

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
            Me.m_cmbSource.Items.Add(core.EcoPathGroupInputs(i))
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

    Private Sub OnOK(sender As Object, e As EventArgs) _
        Handles m_btnOK.Click

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnCancel(sender As Object, e As EventArgs) _
        Handles m_btnCancel.Click

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()

    End Sub

    Private Sub OnSourceSelected(sender As Object, e As EventArgs) _
        Handles m_cmbSource.SelectedIndexChanged

        Dim i As Integer = Me.SelectedSource
        Dim core As cCore = Me.m_uic.Core

        If (i > 0) Then
            Dim grpIn As cEcoPathGroupInput = core.EcoPathGroupInputs(i)
            Dim grpOut As cEcoPathGroupOutput = core.EcoPathGroupOutputs(i)
            Me.m_biomass = grpOut.Biomass

            ' Set biomass source type
            If grpIn.isMultiStanza Then
                Me.m_biomasssource = eBiomassSource.Stanza
            ElseIf grpIn.NTaxon > 0 Then
                Me.m_biomasssource = eBiomassSource.Taxonomy
            Else
                Me.m_biomasssource = eBiomassSource.Manual
            End If

            Me.m_fpN1.Value = grpIn.Name
            Me.m_fpN2.Value = grpIn.Name

        Else
            Me.m_biomass = cCore.NULL_VALUE
            Me.m_biomasssource = eBiomassSource.NotSet
        End If

        Me.OnSliderValueChanged(Me, Nothing)

        Me.UpdateControls()

    End Sub

    Private Sub OnFormatGroupItem(sender As Object, e As Windows.Forms.ListControlConvertEventArgs) Handles m_cmbSource.Format

        Try
            Dim fmt As New ScientificInterfaceShared.Style.cCoreInterfaceFormatter()
            Dim grp As cCoreGroupBase = DirectCast(e.ListItem, cCoreGroupBase)

            If (Not grp.Disposed) Then
                e.Value = fmt.GetDescriptor(e.ListItem)
            End If
        Catch ex As Exception
            ' mmm
        End Try

    End Sub

    Private Sub OnSplitNameChanged(sender As Object, e As EventArgs) _
        Handles m_tbxSplit1.TextChanged, m_tbxSplit2.TextChanged

        Me.UpdateControls()

    End Sub

    Private Sub OnSliderValueChanged(sender As Object, e As EventArgs) Handles m_sliderB.ValueChanged

        If Me.m_bInUpdate Then Return
        Me.m_bInUpdate = True

        Me.B1 = Me.m_biomass * B1Ratio
        Me.B2 = Me.m_biomass * (1 - B1Ratio)

        Me.m_bInUpdate = False

    End Sub

    Private Sub OnB1Changed(sender As Object, e As EventArgs) Handles m_tbxB1.TextChanged

        Dim b1 As Single = Math.Min(Me.m_biomass, CSng(Me.m_fpB1.Value))
        If (b1 = 0) Then
            Me.B1Ratio = 0
        Else
            Me.B1Ratio = Me.m_biomass / b1
        End If

    End Sub

    Private Sub OnB2Changed(sender As Object, e As EventArgs) Handles m_tbxB2.TextChanged

        Dim b2 As Single = Math.Min(Me.m_biomass, CSng(Me.m_fpB1.Value))
        If (b2 = Me.m_biomass) Then
            Me.B1Ratio = 1
        Else
            Me.B1Ratio = Me.m_biomass / (1 - b2)
        End If
    End Sub

#End Region ' Events

#Region " Internals "

    Private Function SelectedSource() As Integer

        Dim item As Object = Me.m_cmbSource.SelectedItem

        If (item Is Nothing) Then Return cCore.NULL_VALUE
        If (Not TypeOf (item) Is cCoreGroupBase) Then Return cCore.NULL_VALUE
        Return DirectCast(item, cCoreGroupBase).Index

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
                Me.m_sliderB.Value = CInt(Math.Max(0, Math.Min(1000, value * 1000.0!)))
            End If
        End Set
    End Property

    Private Sub UpdateControls()

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True

        Dim bHasSource As Boolean = (Me.SelectedSource > 0)
        Dim bHasTargets As Boolean = True ' Validate unique target names

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

        Me.m_fpB1.Style = cSystemUtils.IIF(bEditBiomass, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)
        Me.m_fpB2.Style = cSystemUtils.IIF(bEditBiomass, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)

        Me.m_fpA1.Style = cSystemUtils.IIF(bEditAges, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)
        Me.m_fpA2.Style = cSystemUtils.IIF(bEditAges, cStyleGuide.eStyleFlags.OK, cStyleGuide.eStyleFlags.NotEditable)

        Me.m_btn1to2.Enabled = bEditTaxa And (Me.m_lbxTaxa1.SelectedIndices.Count > 0)
        Me.m_btn2to1.Enabled = bEditTaxa And (Me.m_lbxTaxa2.SelectedIndices.Count > 0)

        Me.m_bInUpdate = False

    End Sub

    Private Sub m_btn2to1_Click(sender As Object, e As EventArgs) Handles m_btn2to1.Click

    End Sub

    Private Sub m_btn1to2_Click(sender As Object, e As EventArgs) Handles m_btn1to2.Click

    End Sub

#End Region ' Internals

End Class