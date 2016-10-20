Option Strict On
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

Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cSpatialSSPluginPoint
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin
    Implements EwEPlugin.IEcospaceEndTimestepPostPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_timeseries As cSpatialTimeSeries = Nothing

    Private m_frm As frmSpatialSS = Nothing

#End Region ' Private vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "VC, JS, KT"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return "Spatial SS"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Implements simple Ecospace Sum of Squares calculations from CSV reference data"
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "Calculate SS"
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Form) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Generic plug-in bits

#Region " Ecospace integration "

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_timeseries = New cSpatialTimeSeries(Me.m_core)
    End Sub

    Public Sub EcospaceEndTimeStepPost(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceEndTimestepPostPlugin.EcospaceEndTimeStepPost

        Dim pts As cSpatialTimeSeries.cDataPoint() = Me.m_timeseries.DataPoints(iTime)
        ' Update SS

    End Sub

#End Region ' Ecospace integration

#Region " Internals "

    Private Function GetUI() As Form
        If Not Me.HasUI() Then
            Me.m_frm = New frmSpatialSS(Me.m_uic, Me.m_timeseries)
        End If
        Return Me.m_frm
    End Function

    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return Not Me.m_frm.IsDisposed
    End Function

#End Region ' Internals 

End Class