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
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

Public Class cEcospaceValidationPlugin
    Implements IEcospaceInitRunStartedPlugin
    Implements IEcospaceEndTimestepPlugin
    Implements IEcospaceRunCompletedPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin

    Public Const PLUGIN_NAME As String = "EcospaceValidationPlugin"

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_pathdata As cEcopathDataStructures = Nothing
    Private m_spacedata As cEcospaceDataStructures = Nothing

    Private m_engine As cEcospaceValidation = Nothing

    Private m_uic As cUIContext = Nothing
    Private m_ui As frmEcospaceValidation = Nothing

#End Region ' Private vars

#Region " Generic plugin bits "

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return PLUGIN_NAME
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "EwE dev team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceOutput"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav_output
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return "Under development"
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceCompleted
        End Get
    End Property

#End Region ' Generic plugin bits

#Region " Relevant Ecospace plugin bits "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
        Me.m_pathdata = Me.m_core.EcopathDataStructures
        Me.m_spacedata = Me.m_core.EcospaceDataStructures
        Me.m_engine = New cEcospaceValidation(Me.m_core, Me.m_pathdata, Me.m_spacedata)
    End Sub

    Public Sub EcospaceInitRunStarted(EcospaceDatastructures As Object) Implements IEcospaceInitRunStartedPlugin.EcospaceInitRunStarted
        ' Reset admin at start of a run
        Me.m_engine.Clear()
    End Sub

    Public Sub EcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPlugin.EcospaceEndTimeStep
        ' Compute stats
        Me.m_engine.CalculateStats(Me.m_spacedata.Bcell, iTime)
    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        ' Nothing to do here
    End Sub

#End Region ' Relevant Ecospace plugin bits

#Region " UI bits "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Windows.Forms.Form) Implements IGUIPlugin.OnControlClick
        ' Show UI
        If Not Me.HasUI Then
            Me.m_ui = GetUI()
        End If
        frmPlugin = Me.m_ui
    End Sub

#End Region ' UI bits

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return (Me.m_ui.IsDisposed = False)
    End Function

    Private Function GetUI() As frmEcospaceValidation
        Return New frmEcospaceValidation(Me.m_uic, Me.m_engine)
    End Function

#End Region ' Internals

End Class
