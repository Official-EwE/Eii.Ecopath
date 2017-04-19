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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
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

Public Class cTransectWriterPlugin
    Implements INavigationTreeItemPlugin
    Implements IEcospaceResultWriterPlugin
    Implements IUIContextPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_data As cTransectDatastructures = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_frm As frmTransectWriter = Nothing

#End Region ' Private vars

#Region " Foundation "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = CType(core, cCore)
        Me.m_data = New cTransectDatastructures(Me.m_core)
    End Sub

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "OutTransects"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
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

#End Region ' Foundation

#Region " UI "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput"
        End Get
    End Property

    Public ReadOnly Property ControlImage As Image Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav0_application_get
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

#End Region ' UI

#Region " Result writer "

    Public Sub Init(theCore As Object) Implements IEcospaceResultsWriter.Init

    End Sub

    Public ReadOnly Property DisplayName As String Implements IEcospaceResultsWriter.DisplayName
        Get
            Return "Transects"
        End Get
    End Property

    Public Property Enabled As Boolean Implements IEcospaceResultsWriter.Enabled
        Get
            Return Me.m_data.Autosaving
        End Get
        Set(value As Boolean)
            Me.m_data.Autosaving = value
        End Set
    End Property

    Public Property FirstOutputTimeStep As Integer Implements IEcospaceResultsWriter.FirstOutputTimeStep

    Public Sub StartWrite() Implements IEcospaceResultsWriter.StartWrite

    End Sub

    Public Sub WriteResults(SpaceTimeStepResults As Object) Implements IEcospaceResultsWriter.WriteResults

    End Sub

    Public Sub EndWrite() Implements IEcospaceResultsWriter.EndWrite

    End Sub

#End Region ' Result writer

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return Not Me.m_frm.IsDisposed
    End Function

    Private Function GetUI() As frmTransectWriter
        If (Not Me.HasUI()) Then Me.m_frm = New frmTransectWriter(Me.m_uic, Me.m_data)
        Return Me.m_frm
    End Function

#End Region ' Internals 

End Class
