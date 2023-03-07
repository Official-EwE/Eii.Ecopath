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

Option Explicit On
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Window
Imports EwECore
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class frmEcospaceValidation

#Region " Private vars "

    Private m_bInUpdate As Boolean = True

#End Region ' Private vars

#Region " Construction / destruction "

    Friend Sub New(uic As cUIContext, engine As cEcospaceValidation)
        MyBase.New()

        Me.Engine = engine

        Me.InitializeComponent()

        Me.UIContext = uic
        Me.Text = My.Resources.CAPTION
        Me.TabText = My.Resources.CAPTION

    End Sub

#End Region ' Construction / destruction

#Region " Overrides "

    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Me.m_grid.UIContext = Me.UIContext

        Me.m_bInUpdate = True

        Me.m_nudTimeStep.Value = 1
        Me.m_nudTimeStep.Minimum = 1
        Me.m_nudTimeStep.Maximum = Me.Core.nEcospaceTimeSteps

        Me.m_slTimestep.Value = Me.m_nudTimeStep.Value
        Me.m_slTimestep.Minimum = Me.m_nudTimeStep.Minimum
        Me.m_slTimestep.Maximum = Me.m_nudTimeStep.Maximum

        Me.m_bInUpdate = False

        Me.UpdateGrid()

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        Me.m_grid.UIContext = Nothing
        MyBase.OnFormClosed(e)

    End Sub

#End Region ' Overrides

#Region " Control events "

    Private Sub OnNudValueChanged(sender As Object, e As EventArgs) Handles m_nudTimeStep.ValueChanged

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Me.m_slTimestep.Value = Me.m_nudTimeStep.Value
        Me.m_bInUpdate = False

        Me.UpdateGrid()

    End Sub

    Private Sub OnSliderValueChanged(sender As Object, e As EventArgs) Handles m_slTimestep.ValueChanged

        If (Me.m_bInUpdate) Then Return
        Me.m_bInUpdate = True
        Me.m_nudTimeStep.Value = Me.m_slTimestep.Value
        Me.m_bInUpdate = False

        Me.UpdateGrid()

    End Sub

    Friend Sub Poke(iTime As Integer)
        Dim iTimeStep As Integer = CInt(Me.m_nudTimeStep.Value)
        If (iTime = iTimeStep) Then UpdateGrid()
    End Sub

#End Region ' Control events

#Region " UI updates "

    Friend ReadOnly Property Engine As cEcospaceValidation

    Private Sub UpdateGrid()

        Dim iTimeStep As Integer = CInt(Me.m_nudTimeStep.Value)
        Me.m_grid.UpdateData(Me.Engine.MeanBwPrey(iTimeStep))

    End Sub

#End Region ' UI updates

End Class