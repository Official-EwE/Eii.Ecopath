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
Imports System.Windows.Forms
Imports ScientificInterfaceShared.Controls
Imports EwECore

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

    Private Sub dlgHarvestControlRule_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        m_HRC = New HCR_Group(m_Plugin.Core)

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.cbBiomassGroups.Items.Add(New cCoreInputOutputControlItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next


        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.cbFMortGroups.Items.Add(New cCoreInputOutputControlItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next

        cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Target))
        cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Conservation))
        cbCostFunctions.SelectedIndex = 0

    End Sub

#End Region

#Region "Control event handlers"

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles OK_Button.Click

        ' ToDo_JS: globalize this

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Dim validationstring As String = ""
        Me.m_isValid = True

        If Me.m_strategy.Contains(Me.HarvestControlRule) Then
            Me.m_isValid = False
            'Failed vaidation rule already exists in strategy
            MsgBox("Sorry this harvest Control Rule already exists in the current Strategy.", MsgBoxStyle.Critical, "Please fix any errors and try again.")
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            '  Me.Close()
            'Don't bother checking the other validation
            'Just boot out
            Return
        End If

        If Not Me.HarvestControlRule.isValid(validationstring) Then
            Me.m_isValid = False
            'If the Harvest Rule is not valid set the DialogResult to Cancel so the rule is not used
            MsgBox("Sorry invalid Harvest Control Rule." + Environment.NewLine + validationstring, MsgBoxStyle.Critical, "Please fix any errors and try again.")
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Return
        End If

        Me.Close()

    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.m_isValid = True
        Me.Close()
    End Sub


    Private Sub cbBiomassGroups_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbBiomassGroups.SelectedIndexChanged

        updateHRC()

    End Sub

    Private Sub cbFMortGroups_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbFMortGroups.SelectedIndexChanged

        updateHRC()

    End Sub

    Private Sub updateHRC()

        Dim selItem As cCoreInputOutputControlItem = Nothing
        Dim grpOut As cEcoPathGroupOutput = Nothing

        ' Group Biomass
        selItem = DirectCast(cbBiomassGroups.SelectedItem, cCoreInputOutputControlItem)
        If (selItem IsNot Nothing) Then

            Me.m_HRC.GroupB = DirectCast(selItem.Source, cEcoPathGroupInput)
            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupB.Index)

            Me.m_HRC.LowerLimit = grpOut.Biomass * 0.1
            Me.m_HRC.UpperLimit = grpOut.Biomass * 0.4

        End If

        ' Fishing Mort
        selItem = DirectCast(cbFMortGroups.SelectedItem, cCoreInputOutputControlItem)
        If selItem IsNot Nothing Then

            Me.m_HRC.GroupF = DirectCast(selItem.Source, cEcoPathGroupInput)

            grpOut = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupF.Index)
            Me.m_HRC.MaxF = grpOut.MortCoFishRate

        End If

        ' Cost function
        If (Me.cbCostFunctions.SelectedItem IsNot Nothing) Then
            Me.m_HRC.CostFunction = HCR_Group.toCostFunctionEnum(CStr(Me.cbCostFunctions.SelectedItem))
        End If

        ' Oooh
        Me.txRule.Text = Me.m_HRC.ToString()

    End Sub

    Private Sub cbCostFunctions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbCostFunctions.SelectedIndexChanged
        Me.updateHRC()
    End Sub

#End Region

    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

        If Not Me.m_isValid Then
            'Not a valid rule
            'so stop the form from closing 
            'to let the user correct the rule
            e.Cancel = True
        End If
        MyBase.OnFormClosing(e)

    End Sub

End Class
