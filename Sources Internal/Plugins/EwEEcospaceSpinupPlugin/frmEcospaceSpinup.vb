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
Imports ScientificInterfaceShared.Properties
Imports System.Windows.Forms

#End Region ' Imports

''' <summary>
''' Main user interface for Ecospace spin-up logic
''' </summary>
Public Class frmEcospaceSpinup

#Region " Private vars "

    Private m_plugin As cEcospaceSpinupPlugin = Nothing
    Private m_bInitializing As Boolean = False
    Private WithEvents m_fpSpinupEnabled As cEwEFormatProvider = Nothing
    Private WithEvents m_fpSpinupYears As cEwEFormatProvider = Nothing

#End Region ' Private vars

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

#Region " Overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)

        MyBase.OnLoad(e)

        Me.m_gridSpinUpDif.UIContext = Me.UIContext
        Me.m_gridSpinUpDif.Init(Me.m_plugin)

        Me.m_bInitializing = True

        Dim pm As cPropertyManager = Me.PropertyManager
        Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

        Me.m_fpSpinupEnabled = New cPropertyFormatProvider(Me.UIContext, Me.m_chkUseSpinup, parms, eVarNameFlags.EcospaceSpinupEnabled)
        Me.m_fpSpinupYears = New cPropertyFormatProvider(Me.UIContext, Me.m_tbxSpinUpYears, parms, eVarNameFlags.EcospaceSpinupYears)

        AddHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
        'AddHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
        'AddHandler Me.m_plugin.OnEcospaceRunCompleted, AddressOf Me.OnRunCompleted

        Me.m_bInitializing = False

        Me.UpdateControls()

    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

        RemoveHandler Me.m_plugin.OnEcospaceTimeStep, AddressOf Me.OnTimeStep
        'RemoveHandler Me.m_plugin.OnEcospaceRunStarting, AddressOf Me.OnRunStarted
        'RemoveHandler Me.m_plugin.OnEcospaceRunCompleted, AddressOf Me.OnRunCompleted

        Me.m_fpSpinupEnabled.Release()
        Me.m_fpSpinupYears.Release()

        Me.m_gridSpinUpDif.UIContext = Nothing
        MyBase.OnFormClosed(e)

    End Sub

    Public Overrides ReadOnly Property IsRunForm() As Boolean
        Get
            Return False
        End Get
    End Property

    Public Sub SettingsChanged()
        Me.UpdateControls()
    End Sub

    Protected Overrides Sub UpdateControls()

        MyBase.UpdateControls()

        If (Me.m_plugin Is Nothing) Then Return
        If (Me.m_bInitializing) Then Return

        Me.m_bInitializing = True
        Try
            Me.m_chkUseBaseBio.Checked = Me.m_plugin.UseSpinUpBaseBio
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try
        Me.m_bInitializing = False

    End Sub

#End Region ' Overrides

#Region " Public access "

    Friend Sub Init(plugin As cEcospaceSpinupPlugin)
        Me.m_plugin = plugin
    End Sub

#End Region ' Public access

#Region " Events "

    'Private Sub OnRunStarted()
    '    ' NOP
    'End Sub

    Private Sub OnTimeStep()
        Try
            Me.m_gridSpinUpDif.OnTimeStep()
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try
    End Sub

    'Private Sub OnRunCompleted()
    '    ' NOP
    'End Sub

    Private Sub OnUseBaseBiohanged(sender As System.Object, e As System.EventArgs) _
        Handles m_chkUseBaseBio.CheckedChanged

        If (Me.m_bInitializing) Then Return
        Try
            Me.m_plugin.UseSpinUpBaseBio = Me.m_chkUseBaseBio.Checked
        Catch ex As Exception
            cLog.Write(ex, "frmEwESpinupPlugin.UpdateControls")
        End Try

    End Sub

#End Region ' Events

End Class