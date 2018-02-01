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
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports RDotNet

#End Region ' Imports

Public Class cNetworkD3RWriterPlugin
    Implements IMenuItemPlugin
    Implements IEwEOptionsPlugin

    Private m_core As cCore = Nothing
    Private m_engine As REngine = Nothing

#Region " Generic "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "EwEEcopathExportDietToNetworkD3"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "Utility plug-in for EwE to export a food web to a NetworkD3 R script (https://christophergandrud.github.io/networkD3/)"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

#End Region ' Generic

#Region " UI integration "

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuFile\ExportModel"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements IGUIPlugin.ControlText
        Get
            Return "To NetworkD3 simple network"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Label As String Implements IEwEOptionsPlugin.Label
        Get
            Return "NetworkD3"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Windows.Forms.Form) Implements IGUIPlugin.OnControlClick
        Try
            Me.CreateNetworkD3RScript()
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Generic

#Region " Internals "

    ''' <summary>
    ''' Generates the R script and copies it to the clipboard.
    ''' </summary>
    Private Sub CreateNetworkD3RScript()

        If (Me.m_engine Is Nothing) Then
            REngine.SetEnvironmentVariables()
            Me.m_engine = REngine.GetInstance()
        End If

        Dim network As cNetwork = Nothing
        Dim msg As cMessage = Nothing

        Select Case My.Settings.NetworkType
            Case 0
                network = New cSimpleNetwork(Me.m_core)
            Case 1
                network = New cForceNetwork(Me.m_core)
            Case Else
                network = New cSimpleNetwork(Me.m_core)
        End Select

        Try
            Clipboard.SetText(network.GenerateScript())
            msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_SUCCESS, network.Name),
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        Catch ex As Exception
            msg = New cMessage(cStringUtils.Localize(My.Resources.PROMPT_ERROR, network.Name, ex.Message),
                               eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
        End Try
        Me.m_core.Messages.SendMessage(msg)

    End Sub

    Public Function IsConfigured() As Boolean Implements IConfigurable.IsConfigured
        Return True
    End Function

    Public Function GetConfigUI() As Control Implements IConfigurable.GetConfigUI
        Return New ucNetworkD3Options()
    End Function

#End Region ' Internals

End Class
