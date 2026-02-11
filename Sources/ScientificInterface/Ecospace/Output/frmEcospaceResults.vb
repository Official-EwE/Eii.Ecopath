' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecospace

    Public Class frmEcospaceResults

#Region " Private vars "

        ' Results grid
        Private m_GridGear As cGridEcospaceResultsGear = Nothing
        Private m_GridGroup As cGridEcospaceResultsGroup = Nothing
        Private m_GridRegion As cGridEcospaceResultsRegion = Nothing

        ' Summary
        Private m_fpSumStartTime As cEwEFormatProvider = Nothing
        Private m_fpSumEndTime As cEwEFormatProvider = Nothing
        Private m_fpSumLength As cEwEFormatProvider = Nothing

#End Region ' Private vars

        Public Sub New()

            Me.InitializeComponent()

        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim ecospaceModelParams As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()

            Me.m_fpSumStartTime = New cPropertyFormatProvider(Me.UIContext, Me.m_tbSumStartTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeStart)
            Me.m_fpSumEndTime = New cPropertyFormatProvider(Me.UIContext, Me.m_tbSumEndTime, ecospaceModelParams, eVarNameFlags.EcospaceSummaryTimeEnd)
            Me.m_fpSumLength = New cPropertyFormatProvider(Me.UIContext, Me.m_nudSumLength, ecospaceModelParams, eVarNameFlags.EcospaceNumberSummaryTimeSteps)

            'Initialize the results grid
            Me.m_GridGear = New cGridEcospaceResultsGear
            Me.m_GridGear.UIContext = Me.UIContext
            Me.m_GridGear.Dock = DockStyle.Fill
            Me.m_GridGroup = New cGridEcospaceResultsGroup
            Me.m_GridGroup.UIContext = Me.UIContext
            Me.m_GridGroup.Dock = DockStyle.Fill
            Me.m_GridRegion = New cGridEcospaceResultsRegion
            Me.m_GridRegion.UIContext = Me.UIContext
            Me.m_GridRegion.Dock = DockStyle.Fill

            ' Add the result grids. 
            Me.m_plResultsGrid.Controls.Add(Me.m_GridGear)
            Me.m_plResultsGrid.Controls.Add(Me.m_GridGroup)
            Me.m_plResultsGrid.Controls.Add(Me.m_GridRegion)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.Ecospace}

            Me.FillFilterCombos()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_fpSumStartTime.Release()
            Me.m_fpSumEndTime.Release()
            Me.m_fpSumLength.Release()
            MyBase.OnFormClosed(e)
        End Sub

        ''' <summary>
        ''' Populate gear, region combo boxes.
        ''' </summary>
        Private Sub FillFilterCombos()
            Me.m_rbFleet.Checked = True

            Me.m_cmbGears.Items.Clear()
            Dim fleet As cEcospaceFleetOutput = Nothing
            For i As Integer = 0 To Me.Core.nFleets
                fleet = Me.Core.EcospaceFleetOutput(i)
                If (i = 0) Then
                    Me.m_cmbGears.Items.Add(fleet.Name)
                Else
                    Me.m_cmbGears.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, i, fleet.Name))
                End If
            Next
            Me.m_cmbGears.SelectedIndex = 0

            Me.m_cmbRegions.Items.Clear()
            Dim region As cEcospaceRegionOutput = Nothing
            For i As Integer = 0 To Me.Core.nRegions
                region = Me.Core.EcospaceRegionOutput(i)
                If (i = 0) Then
                    Me.m_cmbRegions.Items.Add(region.Name)
                Else
                    Me.m_cmbRegions.Items.Add(String.Format(SharedResources.GENERIC_LABEL_INDEXED, i, region.Name))
                End If
            Next
            Me.m_cmbRegions.SelectedIndex = 0

        End Sub

        Private Sub rbResults_CheckedChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_rbFleet.CheckedChanged, m_rbGroup.CheckedChanged, m_rbRegion.CheckedChanged

            Me.UpdateControls()

        End Sub

        Private Sub OnSelectGear(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbGears.SelectedIndexChanged

            'fleets are zero based so the zero index is ok
            Me.m_GridGroup.SelFleetIndex = Me.m_cmbGears.SelectedIndex
            Me.m_GridGroup.RefreshContent()

        End Sub

        Private Sub OnSelectRegion(sender As System.Object, e As System.EventArgs) _
            Handles m_cmbRegions.SelectedIndexChanged

            'regions are zero based so the zero index is ok
            Me.m_GridRegion.SelRegionIndex = Me.m_cmbRegions.SelectedIndex
            Me.m_GridRegion.RefreshContent()

        End Sub

        Protected Overrides Sub UpdateControls()

            ' Show grids
            Me.m_GridGear.Visible = Me.m_rbFleet.Checked
            Me.m_GridGroup.Visible = Me.m_rbGroup.Checked
            Me.m_GridRegion.Visible = Me.m_rbRegion.Checked

            Me.m_cmbGears.Enabled = Me.m_rbGroup.Checked
            Me.m_cmbRegions.Enabled = Me.m_rbRegion.Checked

        End Sub

        ''' <summary>
        ''' Message handler for core Ecosim Datachanged message
        ''' </summary>
        ''' <param name="msg"></param>
        ''' <remarks>This updates the grids with the results if the user changed the time periods</remarks>
        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            If msg.DataType = eDataTypes.EcospaceModelParameter Then
                For Each var As cVariableStatus In msg.Variables
                    If var.VarName = eVarNameFlags.EcospaceSummaryTimeStart Or var.VarName = eVarNameFlags.EcospaceSummaryTimeEnd Or var.VarName = eVarNameFlags.EcospaceNumberSummaryTimeSteps Then

                        If Me.m_GridGroup.Visible Then Me.m_GridGroup.RefreshContent()
                        If Me.m_GridRegion.Visible Then Me.m_GridRegion.RefreshContent()
                        If Me.m_GridGear.Visible Then Me.m_GridGear.RefreshContent()

                        Exit Sub
                    End If
                Next
            End If
            MyBase.OnCoreMessage(msg)
        End Sub

    End Class

End Namespace

