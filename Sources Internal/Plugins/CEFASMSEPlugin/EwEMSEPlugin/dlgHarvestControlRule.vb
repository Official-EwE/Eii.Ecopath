Imports System.Windows.Forms

Public Class dlgHarvestControlRule

    Private Class cbItem

        Private m_item As EwECore.cCoreInputOutputBase

        Public Sub New(coreObject As EwECore.cCoreInputOutputBase)
            m_item = coreObject
        End Sub

        Public ReadOnly Property Index As Integer
            Get
                Return m_item.Index
            End Get
        End Property

        Public Overrides Function toString() As String
            Return m_item.Name
        End Function

    End Class

    Private m_Plugin As cMSE
    Private m_HRC As HCR_Group

    Public Sub Init(MSEPlugin As cMSE)
        m_Plugin = MSEPlugin
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dlgHarvestControlRule_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        m_HRC = New HCR_Group

        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.cbBiomassGroups.Items.Add(New cbItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next


        For igrp As Integer = 1 To Me.Core.nGroups
            If Core.EcoPathGroupInputs(igrp).IsFished Then
                Me.cbFMortGroups.Items.Add(New cbItem(Core.EcoPathGroupInputs(igrp)))
            End If
        Next

        cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Target))
        cbCostFunctions.Items.Add(HCR_Group.toCostFunctionString(eCostFunctionTypes.Conservation))
        cbCostFunctions.SelectedIndex = 0

    End Sub

    Private ReadOnly Property Core As EwECore.cCore
        Get
            Return Me.m_Plugin.Core
        End Get
    End Property

    Private Sub cbBiomassGroups_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbBiomassGroups.SelectedIndexChanged
        'Dim selItem As cbItem = DirectCast(cbBiomassGroups.SelectedItem, cbItem)

        'Me.m_HRC.GroupNumber4Biomass = selItem.Index
        'Me.m_HRC.GroupName4Biomass = selItem.toString

        'Me.m_HRC.LowerLimit = Me.Core.EcoPathGroupInputs(Me.m_HRC.GroupNumber4Biomass).BiomassAreaInput * 0.1
        'Me.m_HRC.UpperLimit = Me.Core.EcoPathGroupInputs(Me.m_HRC.GroupNumber4Biomass).BiomassAreaInput * 0.4

        updateHRC()

    End Sub

    Private Sub cbFMortGroups_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbFMortGroups.SelectedIndexChanged
        'Dim selItem As cbItem = DirectCast(cbFMortGroups.SelectedItem, cbItem)

        'Me.m_HRC.GroupNumber4F = selItem.Index
        'Me.m_HRC.GroupName4F = selItem.toString

        'Me.m_HRC.MaxF = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4F).MortCoFishRate

        updateHRC()
    End Sub

    Private Sub updateHRC()

        'Group Biomass
        Dim selItem As cbItem = DirectCast(cbBiomassGroups.SelectedItem, cbItem)
        Me.m_HRC.GroupNumber4Biomass = selItem.Index
        Me.m_HRC.GroupName4Biomass = selItem.toString
        Me.m_HRC.LowerLimit = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4Biomass).Biomass * 0.1
        Me.m_HRC.UpperLimit = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4Biomass).Biomass * 0.4

        'Fishing Mort
        selItem = DirectCast(cbFMortGroups.SelectedItem, cbItem)
        Me.m_HRC.GroupNumber4F = selItem.Index
        Me.m_HRC.GroupName4F = selItem.toString
        Me.m_HRC.MaxF = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4F).MortCoFishRate

        Me.m_HRC.CostFunction = Me.cbCostFunctions.SelectedItem

        Me.txRule.Text = Me.m_HRC.toDisplayString
    End Sub


    Public ReadOnly Property HarvestControlRule As HCR_Group
        Get
            Return Me.m_HRC
        End Get
    End Property
   
    Private Sub cbCostFunctions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbCostFunctions.SelectedIndexChanged
        Me.updateHRC()
        ' m_HRC.CostFunction = Me.cbCostFunctions.SelectedItem
    End Sub
End Class
