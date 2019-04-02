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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class ucOptions
    Implements IOptionsPage

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_man As cNetworkManager = Nothing
    Private m_cbh As cCheckboxHierarchy = Nothing
    Private m_bInUpdate As Boolean = False

#End Region ' Private vars

    Public Sub New(uic As cUIContext)
        Me.UIContext = uic
        Me.InitializeComponent()
    End Sub

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        Me.m_bInUpdate = True

        Me.m_man = cEwENetworkAnalysisPlugin.thePlugin.Manager
        Me.m_cbUseTimeout.Checked = Me.m_man.UseAbortTimer
        Me.m_nudTimeOut.Value = CInt(Me.m_man.TimeOutMilSecs / (1000 * 60))
        Me.m_cbSkipCyclesPathways.Checked = Me.m_man.SkipCyclesPathways

        Me.m_cbh = New cCheckboxHierarchy(Me.m_cbAutosaveRoot)
        Me.m_cbh.Add(Me.m_cbAutosaveEcopath, Me.m_cbAutosaveRoot)
        Me.m_cbh.Add(Me.m_cbAutosaveEcosimWoPPR, Me.m_cbAutosaveRoot)
        Me.m_cbh.Add(Me.m_cbAutosaveEcosimWithPPR, Me.m_cbAutosaveRoot)
        Me.m_cbh.ManageCheckedStates = True

        Me.m_cbAutosaveEcopath.Checked = My.Settings.AutosaveEcopath
        Me.m_cbAutosaveEcosimWoPPR.Checked = My.Settings.AutosaveEcosimWoPPR
        Me.m_cbAutosaveEcosimWithPPR.Checked = My.Settings.AutosaveEcosimWithPPR

        Me.m_bInUpdate = False

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing And Me.components IsNot Nothing Then
                Me.m_cbh.Dispose()
                Me.components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private Sub UpdateControls()

        Me.m_nudTimeOut.Enabled = Me.m_cbUseTimeout.Checked
        Me.m_lblSkipCyclesPathways.Visible = Me.m_cbSkipCyclesPathways.Checked

    End Sub

#Region " Event handlers "

    Private Sub OnNoCyclesPathwaysChanged(sender As Object, e As EventArgs) Handles m_cbSkipCyclesPathways.CheckedChanged

        If Me.m_bInUpdate Then Return
        Me.UpdateControls()

    End Sub

    Private Sub OnCheckChanged(sender As Object, e As EventArgs) Handles m_cbUseTimeout.CheckedChanged

        If Me.m_bInUpdate Then Return
        Me.UpdateControls()

    End Sub


    Private Sub OnSaveEcosimWoPPRChecked(sender As Object, e As EventArgs) Handles m_cbAutosaveEcosimWoPPR.CheckedChanged

        If Me.m_bInUpdate Then Return
        If (Me.m_cbAutosaveEcosimWoPPR.Checked) Then
            Me.m_cbAutosaveEcosimWithPPR.Checked = False
        End If
        Me.UpdateControls()

    End Sub

    Private Sub OnSaveEcosimWithPPRChecked(sender As Object, e As EventArgs) Handles m_cbAutosaveEcosimWithPPR.CheckedChanged

        If Me.m_bInUpdate Then Return
        If (Me.m_cbAutosaveEcosimWithPPR.Checked) Then
            Me.m_cbAutosaveEcosimWoPPR.Checked = False
        End If
        Me.UpdateControls()

    End Sub

#End Region ' Event handlers

#Region " Options page implementation "

    Public Property UIContext As cUIContext Implements IUIElement.UIContext

    Public Event OnChanged As IOptionsPage.OnChangedEventHandler Implements IOptionsPage.OnChanged

    Public Function CanApply() As Boolean Implements IOptionsPage.CanApply
        Return True
    End Function

    Public Function Apply() As IOptionsPage.eApplyResultType Implements IOptionsPage.Apply

        ' If not initialized
        If (Me.m_man Is Nothing) Then Return IOptionsPage.eApplyResultType.Success

        Me.m_man.TimeOutMilSecs = CInt(Me.m_nudTimeOut.Value * 1000 * 60)
        Me.m_man.UseAbortTimer = m_cbUseTimeout.Checked
        Me.m_man.SkipCyclesPathways = Me.m_cbSkipCyclesPathways.Checked

        My.Settings.AutosaveEcosimWoPPR = Me.m_cbAutosaveEcosimWoPPR.Checked
        My.Settings.AutosaveEcosimWithPPR = Me.m_cbAutosaveEcosimWithPPR.Checked
        My.Settings.AbortTimoutMins = CInt(Me.m_nudTimeOut.Value)
        My.Settings.Save()

        Return IOptionsPage.eApplyResultType.Success

    End Function

    Public Function CanSetDefaults() As Boolean Implements IOptionsPage.CanSetDefaults
        Return True
    End Function

    Public Sub SetDefaults() Implements IOptionsPage.SetDefaults
        Me.m_cbUseTimeout.Checked = True
        Me.m_nudTimeOut.Value = 30
        Me.m_cbAutosaveRoot.Checked = False
        Me.m_cbSkipCyclesPathways.Checked = False
    End Sub


#End Region ' Options page implementation 

End Class
