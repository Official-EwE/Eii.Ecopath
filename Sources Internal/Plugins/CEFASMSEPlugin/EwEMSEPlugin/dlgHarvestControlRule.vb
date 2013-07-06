Imports System.Windows.Forms

Public Class dlgHarvestControlRule

#Region "Private helper class"

    ''' <summary>
    ''' Wrapper around a EwECore.cCoreInputOutputBase item used for selecting a combobox item
    ''' </summary>
    ''' <remarks></remarks>
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

#End Region

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

#End Region
    
#Region "Control event handlers"

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Dim validationstring As String
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

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
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

        'Group Biomass
        Dim selItem As cbItem = DirectCast(cbBiomassGroups.SelectedItem, cbItem)
        If selItem IsNot Nothing Then
            Me.m_HRC.GroupNumber4Biomass = selItem.Index
            Me.m_HRC.GroupName4Biomass = selItem.toString
            Me.m_HRC.LowerLimit = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4Biomass).Biomass * 0.1
            Me.m_HRC.UpperLimit = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4Biomass).Biomass * 0.4
        End If

        'Fishing Mort
        selItem = DirectCast(cbFMortGroups.SelectedItem, cbItem)
        If selItem IsNot Nothing Then
            Me.m_HRC.GroupNumber4F = selItem.Index
            Me.m_HRC.GroupName4F = selItem.toString
            Me.m_HRC.MaxF = Me.Core.EcoPathGroupOutputs(Me.m_HRC.GroupNumber4F).MortCoFishRate
        End If

        If Me.cbCostFunctions.SelectedItem IsNot Nothing Then
            Me.m_HRC.CostFunction = Me.cbCostFunctions.SelectedItem
        End If

        Me.txRule.Text = Me.m_HRC.toDisplayString

    End Sub

    Private Sub cbCostFunctions_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbCostFunctions.SelectedIndexChanged
        Me.updateHRC()
    End Sub



#End Region

    Private Sub dlgHarvestControlRule_Validated(sender As Object, e As System.EventArgs) Handles Me.Validated

    End Sub

    Private Sub dlgHarvestControlRule_Validating(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles Me.Validating

    End Sub


    Protected Overrides Sub OnFormClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)
        MyBase.OnFormClosing(e)

        If Not Me.m_isValid Then
            'Not a valid rule
            'so stop the form from closing 
            'to let the user correct the rule
            e.Cancel = True
        End If

    End Sub

   

End Class
