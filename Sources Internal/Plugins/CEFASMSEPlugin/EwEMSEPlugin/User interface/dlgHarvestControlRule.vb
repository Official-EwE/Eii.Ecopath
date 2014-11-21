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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'
Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Style

Public Class dlgHarvestControlRule

    Private Class cHCRTypeItem
        Private m_rule As HCRType
        Public Sub New(rule As HCRType)
            Me.m_rule = rule
        End Sub
        Public Overrides Function ToString() As String
            Return New cCostFunctionTypeFormatter().GetDescriptor(Me.m_rule)
        End Function
        Public ReadOnly Property [Function] As HCRType
            Get
                Return Me.m_rule
            End Get
        End Property
    End Class

#Region " Private vars "

    Private m_Plugin As cMSE = Nothing
    Private m_strategy As Strategy = Nothing
    Private m_HRC As HCR_Group = Nothing
    Private m_bIsValid As Boolean = True
    Private m_bInitialized As Boolean = False

    Private ReadOnly Property Core As EwECore.cCore
        Get
            Return Me.m_Plugin.Core
        End Get
    End Property

#End Region ' Private vars

#Region " Public Properties "

    Public ReadOnly Property HarvestControlRule As HCR_Group
        Get
            Return Me.m_HRC
        End Get
    End Property

#End Region ' Public Properties

#Region " Initialization Construction "

    Public Sub Init(MSEPlugin As cMSE, curStrategy As Strategy, Optional curHCR As HCR_Group = Nothing)
        Me.m_Plugin = MSEPlugin
        Me.m_strategy = curStrategy
        Me.m_HRC = curHCR
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        If (Me.m_HRC Is Nothing) Then
            Me.m_HRC = New HCR_Group(m_Plugin.Core)
        End If

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Dim grp As cEcoPathGroupInput = Core.EcoPathGroupInputs(igrp)
                Dim i As Integer = Me.m_cmbBiomassGroups.Items.Add(grp)
                If (Object.ReferenceEquals(grp, Me.m_HRC.GroupB)) Then
                    Me.m_cmbBiomassGroups.SelectedIndex = i
                End If
            End If
        Next

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Dim grp As cEcoPathGroupInput = Core.EcoPathGroupInputs(igrp)
                Dim i As Integer = Me.m_cmbFMortGroups.Items.Add(grp)
                If (Object.ReferenceEquals(grp, Me.m_HRC.GroupF)) Then
                    Me.m_cmbFMortGroups.SelectedIndex = i
                End If
            End If
        Next

        Me.m_cmbCostFunctions.Items.Add(New cHCRTypeItem(HCRType.Target))
        Me.m_cmbCostFunctions.Items.Add(New cHCRTypeItem(HCRType.Conservation))
        Me.m_cmbCostFunctions.SelectedIndex = cSystemUtils.IIF(Me.m_HRC.TypeOfHCR = HCRType.Target, 0, 1)

        Me.m_bInitialized = True
        Me.CenterToScreen()

        Me.UpdateHRC()

    End Sub

    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
        'If not a valid rule stop the form from closing to let the user correct the rule
        e.Cancel = Not Me.m_bIsValid
        MyBase.OnFormClosing(e)

    End Sub

#End Region ' Initialization Construction

#Region " Control event handlers "


    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click

        ' Think positive
        Me.m_bIsValid = True

        Dim validationstring As String = ""

        If Me.m_strategy.AlreadyContainsATarg(Me.HarvestControlRule) And Me.HarvestControlRule.TypeOfHCR = HCRType.Target Then
            'Failed vaidation rule already exists in strategy
            Me.m_bIsValid = False
            Me.m_Plugin.InformUser(My.Resources.ERROR_HARVESTRULE_DUPLICATE, EwEUtils.Core.eMessageImportance.Critical)
            ' Don't bother checking the other validation. Just boot out
            Return
        End If

        If Not Me.HarvestControlRule.isValid(validationstring) Then
            'If the Harvest Rule is not valid set the DialogResult to Cancel so the rule is not used
            Me.m_bIsValid = False
            Me.m_Plugin.InformUser(String.Format(My.Resources.ERROR_HARVESTRULE_INVALID, validationstring), EwEUtils.Core.eMessageImportance.Critical)
            Return
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_bIsValid = True
        Me.Close()
    End Sub

    Private Sub OnFormatGroupComboItem(sender As Object, e As System.Windows.Forms.ListControlConvertEventArgs) _
        Handles m_cmbBiomassGroups.Format, m_cmbFMortGroups.Format
        Dim fmt As New cCoreInterfaceFormatter()
        e.Value = fmt.GetDescriptor(e.ListItem, eDescriptorTypes.Name)
    End Sub

    Private Sub OnGroupSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbBiomassGroups.SelectedIndexChanged, m_cmbFMortGroups.SelectedIndexChanged

        If Not Me.m_bInitialized Then Return

        Try
            UpdateHRC()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub OnCostFunctionSelected(sender As System.Object, e As System.EventArgs) _
        Handles m_cmbCostFunctions.SelectedIndexChanged

        If Not Me.m_bInitialized Then Return

        Try
            Me.UpdateHRC()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

#End Region ' Control event handlers

#Region " Internals "

    Private Sub UpdateHRC()

        Dim grpOut As cEcoPathGroupOutput = Nothing
        Dim sVal As Single = 0

        ' Group Biomass
        Me.m_HRC.GroupB = DirectCast(m_cmbBiomassGroups.SelectedItem, cEcoPathGroupInput)

        If (Me.m_HRC.GroupB IsNot Nothing) Then
            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupB.Index)
            sVal = grpOut.Biomass
        Else
            sVal = 0
        End If
        Me.m_HRC.LowerLimit = sVal * 0.1
        Me.m_HRC.UpperLimit = sVal * 0.4

        ' Fishing Mort
        Me.m_HRC.GroupF = DirectCast(m_cmbFMortGroups.SelectedItem, cEcoPathGroupInput)
        If (Me.m_HRC.GroupF IsNot Nothing) Then
            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupF.Index)
            Me.m_HRC.MaxF = grpOut.MortCoFishRate
        Else
            Me.m_HRC.MaxF = 0
        End If

        ' Oooh
        Me.m_tbxRule.Text = Me.m_HRC.ToString()

    End Sub

#End Region ' Internals

End Class
