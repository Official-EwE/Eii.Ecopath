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

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cEwEMPADynamicsPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcospacePlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceRunCompletedPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_ui As frmMPADynamics = Nothing
    Private m_engine As cMPADynamicsEngine = Nothing
    Private m_spaceDS As cEcospaceDataStructures = Nothing

#End Region ' Private vars

#Region " Generic plug-in bits "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndEcospaceMPAzzzzDynamics"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "A plug-in to control MPA open/closed dynamics over time"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Generic plug-in bits

#Region " UI integration "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav_input
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return My.Resources.PLUGIN_TITLE
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput\ndEcospaceFishery"
        End Get
    End Property

#End Region ' Navigation tree

#Region " Ecospace integration "

    Public Sub LoadEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.LoadEcospaceScenario
        ' NOP
    End Sub

    Public Sub SaveEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.SaveEcospaceScenario
        ' NOP
    End Sub

    Public Sub CloseEcospaceScenario() Implements IEcospacePlugin.CloseEcospaceScenario
        If (Me.m_engine IsNot Nothing) Then
            'Me.CloseUI()
            Me.m_engine.Clear()
            Me.m_engine = Nothing
        End If
    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_spaceDS = DirectCast(EcospaceDatastructures, cEcospaceDataStructures)
        Me.m_engine = New cMPADynamicsEngine(Me.m_core, Me.m_spaceDS)
    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        ' Make snapshot of MPA states
        Me.m_engine.Backup()
    End Sub

    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        ' Update MPA states
        Me.m_engine.OnEcospaceTimeStep(iTime)
    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        ' Restore snapshot of MPA states
        Me.m_engine.Restore()
    End Sub

#End Region ' Ecospace integration

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_ui IsNot Nothing) Then Return Not Me.m_ui.IsDisposed()
        Return False
    End Function

    Private Function GetUI() As frmMPADynamics
        If (Not Me.HasUI()) Then
            Me.m_ui = New frmMPADynamics(Me.m_uic, Me.m_engine)
        End If
        Return Me.m_ui
    End Function

    Private Sub CloseUI()
        If Me.HasUI() Then
            Me.m_ui.Close()
            Me.m_ui.Dispose()
            Me.m_ui = Nothing
        End If
    End Sub

#End Region ' Internals

End Class
