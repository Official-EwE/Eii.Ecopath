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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwECore.Style
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports System.Windows.Forms

#End Region ' Imports

''' <summary>
''' Main user interface for Ecospace spin-up logic
''' </summary>
Public Class frmEcospaceSpinup

#Region " Private vars "

    Private m_plugin As cEcospaceSpinupPlugin = Nothing
    Private m_bInitializing As Boolean = False
    Private m_fpSpinupYears As cEwEFormatProvider = Nothing

#End Region ' Private vars

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
        Me.Grid = Me.m_gridSpinUpDif
        Me.m_gridSpinUpDif.Init(Me.m_plugin)
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        Me.m_bInitializing = True

        Dim mdYears As New cVariableMetaData(0, 100, cOperatorManager.getOperator(eOperators.GreaterThanOrEqualTo), cOperatorManager.getOperator(eOperators.LessThanOrEqualTo), 0, cUnits.Year)
        Me.m_fpSpinupYears = New cEwEFormatProvider(Me.UIContext, Me.m_tbxSpinUpYears, GetType(Single), mdYears)

        AddHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
        AddHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
        AddHandler Me.m_plugin.OnEcospaceRunCompleted, AddressOf Me.OnRunCompleted

        Me.m_bInitializing = False

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As FormClosedEventArgs)
        RemoveHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
        RemoveHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
        RemoveHandler Me.m_plugin.OnEcospaceRunCompleted, AddressOf Me.OnRunCompleted
        Me.m_fpSpinupYears.Release()
    End Sub

    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub UpdateControls()

        MyBase.UpdateControls()

        If (Me.m_plugin Is Nothing) Then Return
        If (Me.m_bInitializing) Then Return

        Me.m_bInitializing = True
        Try
            Me.m_chkUseSpinup.Checked = Me.m_plugin.UseSpinUp
            Me.m_fpSpinupYears.Value = Me.m_plugin.SpinUpYears
            Me.m_chkUseBaseBio.Checked = Me.m_plugin.UseSpinUpBaseBio
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try
        Me.m_bInitializing = False

    End Sub

#End Region ' Overrides

#Region " Public access "

    Friend Sub Init(ByVal plugin As cEcospaceSpinupPlugin)
        Me.m_plugin = plugin
    End Sub

#End Region ' Public access

#Region " Events "

    Private Sub OnRunStarted()
        Me.UpdateCore()
    End Sub

    Private Sub OnTimeStep()
        Try
            Me.m_gridSpinUpDif.OnTimeStep()
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try
    End Sub

    Private Sub OnRunCompleted()
        ' NOP
    End Sub

    Private Sub OnUseSpinupChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_chkUseSpinup.CheckedChanged

        If (Me.m_bInitializing) Then Return
        Try
            Me.m_plugin.UseSpinUp = Me.m_chkUseSpinup.Checked
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try

    End Sub

    Private Sub OnUseBaseBiohanged(sender As System.Object, e As System.EventArgs) _
        Handles m_chkUseBaseBio.CheckedChanged

        If (Me.m_bInitializing) Then Return
        Try
            Me.m_plugin.UseSpinUpBaseBio = Me.m_chkUseBaseBio.Checked
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try

    End Sub

    Private Sub OnSpinupYearsChanged(sender As System.Object, e As System.EventArgs) _
        Handles m_tbxSpinUpYears.TextChanged

        If (Me.m_bInitializing) Then Return
        Try
            Me.m_plugin.SpinUpYears = CSng(Me.m_fpSpinupYears.Value)
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try

    End Sub

#End Region ' Events

#Region " Internals "

    Private Sub UpdateCore()
        If (Me.m_plugin Is Nothing) Then Return
        Try
            'Update the Plugin State
            Me.m_plugin.UseSpinUp = Me.m_chkUseSpinup.Checked
            Single.TryParse(Me.m_tbxSpinUpYears.Text, Me.m_plugin.SpinUpYears)
            Me.m_plugin.UseSpinUpBaseBio = Me.m_chkUseBaseBio.Checked
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateCore")
        End Try
    End Sub

#End Region ' Internals

End Class