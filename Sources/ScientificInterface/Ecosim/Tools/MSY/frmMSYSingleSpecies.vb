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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Commands
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Commands
Imports EwEUtils.Core
Imports EwECore

#End Region ' Imports

Namespace Ecosim

    Public Class frmMSYSingleSpecies

#Region " Privates "

        Private Enum eSelectionModeType As Integer
            Fleets
            Groups
        End Enum

        Private m_selectionMode As eSelectionModeType = eSelectionModeType.Groups
        Private m_zgh As cZedGraphHelper = Nothing
        Private m_bFullAssessment As Boolean = True
        Private m_bStatAssessment As Boolean = False

#End Region ' Privates

        Public Sub New()
            MyBase.new()
            Me.InitializeComponent()
        End Sub

#Region " Form overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            ' Style controls in here to prevent form designer from embedding images
            Me.m_tsbnGroup.Image = SharedResources.fish
            Me.m_tsbnFleet.Image = SharedResources.fishing_gear

            If (Me.UIContext Is Nothing) Then Return

            ' Invoke 'display groups' command with selective options
            Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
            cmd.AddControl(Me.m_tsbnShowHide, New Object() {cDisplayGroupsCommand.eGroupDisplayOptions.Fished})

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.m_graph)

            Me.PopulateTargetComboBox()
            Me.UpdateControls()

        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

            If (Me.UIContext IsNot Nothing) Then

                Dim cmd As cCommand = Me.CommandHandler.GetCommand(cDisplayGroupsCommand.cCOMMAND_NAME)
                cmd.RemoveControl(Me.m_tsbnShowHide)

                Me.m_zgh.Detach()
                Me.m_zgh = Nothing

            End If

            MyBase.OnFormClosed(e)
        End Sub

        Protected Overrides Sub UpdateControls()

            If (Me.UIContext Is Nothing) Then Return

            Me.m_tsbnGroup.Checked = (Me.SelectionMode = eSelectionModeType.Groups)
            Me.m_tsbnFleet.Checked = (Me.SelectionMode = eSelectionModeType.Fleets)

            Me.m_tsbnFull.Checked = Me.m_bFullAssessment
            Me.m_tsbnStationary.Checked = Me.m_bStatAssessment

            Me.m_tsbnSaveOutput.Checked = (Me.Core.Autosave(eAutosaveTypes.MSY))

        End Sub

#End Region ' Form overrides

#Region " Events "

        Private Sub OnSelectTarget(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_tsbnFleet.Click, m_tsbnGroup.Click
            Try
                If Object.ReferenceEquals(sender, Me.m_tsbnFleet) Then
                    Me.SelectionMode = eSelectionModeType.Fleets
                Else
                    Me.SelectionMode = eSelectionModeType.Groups
                End If
                Me.UpdateControls()
                Me.PopulateTargetComboBox()
            Catch ex As Exception
            End Try
        End Sub

        Private Sub OnSaveOutput(sender As System.Object, e As System.EventArgs) Handles m_tsbnSaveOutput.Click
            Try
                Me.Core.Autosave(eAutosaveTypes.MSY) = Me.m_tsbnSaveOutput.Checked
            Catch ex As Exception
                ' Plop
            End Try
        End Sub

        Private Sub OnAssessmentChosen(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnFull.Click, m_tsbnStationary.Click
            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Internals "

        Private Property SelectionMode() As eSelectionModeType
            Get
                Return Me.m_selectionMode
            End Get
            Set(ByVal value As eSelectionModeType)

                If (value <> Me.SelectionMode) Then
                    Me.m_selectionMode = value
                    Me.PopulateTargetComboBox()
                    Me.UpdateControls()
                End If
            End Set
        End Property

        Private Sub PopulateTargetComboBox()

            Me.m_tscmbItem.Items.Clear()

            Select Case Me.SelectionMode
                Case eSelectionModeType.Fleets
                    Me.m_tscmbItem.Items.Add(New cCoreInputOutputControlItem(SharedResources.GENERIC_VALUE_ALL))
                    For i As Integer = 1 To Me.Core.nFleets
                        Dim fleet As cFleetInput = Me.Core.FleetInputs(i)
                        Me.m_tscmbItem.Items.Add(New cCoreInputOutputControlItem(fleet))
                    Next

                Case eSelectionModeType.Groups
                    For i As Integer = 1 To Me.Core.nGroups
                        Dim group As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(i)
                        If (group.IsFished) Then
                            Me.m_tscmbItem.Items.Add(New cCoreInputOutputControlItem(group))
                        End If
                    Next

                Case Else
                    Debug.Assert(False)

            End Select

            If (Me.m_tscmbItem.Items.Count > 0) Then
                Me.m_tscmbItem.SelectedIndex = 0
            End If

        End Sub

#End Region ' Internals

    End Class

End Namespace
