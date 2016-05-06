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

Option Strict On
Option Explicit On

#Region "Imports"

Imports EwEPlugin
Imports EwECore

Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports ScientificInterfaceShared.Controls

#End Region


Public Class cEcospaceFitPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IEcospaceInitRunCompletedPlugin
    Implements EwEPlugin.IEcospaceEndTimestepPlugin
    Implements EwEPlugin.IEcospaceRunCompletedPlugin

    Private m_core As EwECore.cCore

    Private m_fit As cEcospaceFit

    Private m_EcospaceData As cEcospaceDataStructures
    Private m_EcoPathData As cEcopathDataStructures

    Private m_uic As cUIContext
    Private m_frmUI As frmEcospaceFit


    Public ReadOnly Property Fit As cEcospaceFit
        Get
            Return Me.m_fit
        End Get
    End Property


#Region "Plugin initialization and construction"

    Private Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
            m_fit = New cEcospaceFit
        Catch ex As Exception

        End Try

    End Sub

    Private Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) _
                Implements EwEPlugin.ICorePlugin.CoreInitialized

        Try
            Me.m_EcoPathData = DirectCast(objEcoPath, EwECore.Ecopath.cEcoPathModel).EcopathData
            Me.m_EcospaceData = DirectCast(objEcoSpace, EwECore.cEcoSpace).EcoSpaceData
            Me.m_fit.Init(Me.m_core, Me.m_EcoPathData, Me.m_EcospaceData)
        Catch ex As Exception

        End Try

    End Sub

#End Region

#Region "Ecospace Events"

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        Me.m_fit.RunCompleted()
    End Sub

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep
        Me.m_fit.EcospaceTimeStep(iTime)
    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Me.m_fit.RunInitialized()
    End Sub

#End Region

#Region "Private methods"

    Private Function getMainForm() As frmEcospaceFit
        Dim bHasUI As Boolean = False

        If (Me.m_frmUI IsNot Nothing) Then
            bHasUI = Not Me.m_frmUI.IsDisposed
        End If

        If Not bHasUI Then
            Me.m_frmUI = New frmEcospaceFit()
            Me.m_frmUI.UIContext = Me.m_uic
            Me.m_frmUI.Init(Me)
            Me.m_frmUI.Text = "Ecospace fit"
        End If

        Return Me.m_frmUI

    End Function

#End Region

#Region "Plugin interface implementation"

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Name">IPlugin.Name</see> implementation.</summary>
    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "EcospaceFit"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Description">IPlugin.Description</see> implementation.</summary>
    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Ecospace fit to Ecopath biomass."
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Author">IPlugin.Author</see> implementation.</summary>
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Institute for the Oceans and Fisheries"
        End Get
    End Property

    ''' <summary>Generic <see cref="EwEPlugin.IPlugin.Contact">IPlugin.Contact</see> implementation.</summary>
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:support@ecopath.org"
        End Get
    End Property


    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Ecospace fit"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Fit to Ecopath biomass"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceInitialized
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        frmPlugin = Me.getMainForm

    End Sub


    Public ReadOnly Property NavigationTreeItemLocation As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceTools"
        End Get
    End Property

    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

#End Region


End Class
