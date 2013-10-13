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

Public Class dlgHarvestControlRule

#Region "Private variables and Properties"

    Private m_Plugin As cMSE
    Private m_strategy As Strategy
    Private m_HRC As HCR_Group
    Private m_isValid As Boolean = True

    Private ReadOnly Property Core As EwECore.cCore
        Get
            Return Me.m_Plugin.Core
        End Get
    End Property

#End Region

#Region "Public Properties"

    Public ReadOnly Property HarvestControlRule As HCR_Group
        Get
            Return Me.m_HRC
        End Get
    End Property

#End Region

#Region "Initialization Construction"

    Public Sub Init(MSEPlugin As cMSE, curStrategy As Strategy)
        m_Plugin = MSEPlugin
        m_strategy = curStrategy
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        m_HRC = New HCR_Group(m_Plugin.Core)

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.m_cbBiomassGroups.Items.Add(New cCoreInputOutputControlItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.m_cbFMortGroups.Items.Add(New cCoreInputOutputControlItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next

        m_cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Target))
        m_cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Conservation))
        m_cbCostFunctions.SelectedIndex = 0

        Me.CenterToParent()

    End Sub

    Protected Overrides Sub OnFormClosing(ByVal e As FormClosingEventArgs)
        'If not a valid rule stop the form from closing to let the user correct the rule
        e.Cancel = Not Me.m_isValid
        MyBase.OnFormClosing(e)

    End Sub

#End Region

#Region "Control event handlers"

    Private Sub OnOK(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click

        ' JS 02Oct13: globalized this message
        ' JS 02Oct13: replaced message box with cMessages

        ' Think positive
        Me.m_isValid = True

        Dim validationstring As String = ""

        If Me.m_strategy.Contains(Me.HarvestControlRule) Then
            'Failed vaidation rule already exists in strategy
            Me.m_isValid = False
            Me.m_Plugin.InformUser(My.Resources.ERROR_HARVESTRULE_DUPLICATE, EwEUtils.Core.eMessageImportance.Critical)
            ' Don't bother checking the other validation. Just boot out
            Return
        End If

        If Not Me.HarvestControlRule.isValid(validationstring) Then
            'If the Harvest Rule is not valid set the DialogResult to Cancel so the rule is not used
            Me.m_isValid = False
            Me.m_Plugin.InformUser(String.Format(My.Resources.ERROR_HARVESTRULE_INVALID, validationstring), EwEUtils.Core.eMessageImportance.Critical)
            Return
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_isValid = True
        Me.Close()
    End Sub

    Private Sub cbBiomassGroups_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbBiomassGroups.SelectedIndexChanged, m_cbFMortGroups.SelectedIndexChanged
        Try
            updateHRC()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

    Private Sub updateHRC()

        Dim selItem As cCoreInputOutputControlItem = Nothing
        Dim grpOut As cEcoPathGroupOutput = Nothing

        ' Group Biomass
        selItem = DirectCast(m_cbBiomassGroups.SelectedItem, cCoreInputOutputControlItem)
        If (selItem IsNot Nothing) Then

            Me.m_HRC.GroupB = DirectCast(selItem.Source, cEcoPathGroupInput)
            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupB.Index)

            Me.m_HRC.LowerLimit = grpOut.Biomass * 0.1
            Me.m_HRC.UpperLimit = grpOut.Biomass * 0.4

        End If

        ' Fishing Mort
        selItem = DirectCast(m_cbFMortGroups.SelectedItem, cCoreInputOutputControlItem)
        If selItem IsNot Nothing Then

            Me.m_HRC.GroupF = DirectCast(selItem.Source, cEcoPathGroupInput)

            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupF.Index)
            Me.m_HRC.MaxF = grpOut.MortCoFishRate

        End If

        ' Cost function
        If (Me.m_cbCostFunctions.SelectedItem IsNot Nothing) Then
            Me.m_HRC.CostFunction = HCR_Group.toCostFunctionEnum(CStr(Me.m_cbCostFunctions.SelectedItem))
        End If

        ' Oooh
        Me.m_tbxRule.Text = Me.m_HRC.ToString()

    End Sub

    Private Sub cbCostFunctions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_cbCostFunctions.SelectedIndexChanged
        Try
            Me.updateHRC()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

#End Region

End Class
