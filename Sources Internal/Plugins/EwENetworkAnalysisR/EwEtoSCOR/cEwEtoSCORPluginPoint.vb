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
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEwEtoSCORPluginPoint
    Implements IMenuItemPlugin
    Implements IEcopathRunCompletedPlugin

    Private m_core As cCore = Nothing
    Private m_epData As cEcopathDataStructures = Nothing

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Save SCOR file..."
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "Export Ecopath data to a SCOR file for Ulanowitz' NETWRK"
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Debug.Assert(Me.m_core IsNot Nothing)

        Dim sfd As New SaveFileDialog()

        sfd.Title = "Select SCOR output file"
        sfd.CheckPathExists = True
        sfd.CheckFileExists = False
        sfd.OverwritePrompt = True
        sfd.FileName = Path.GetFileNameWithoutExtension(Me.m_core.DataSource.ToString())
        sfd.InitialDirectory = Path.GetDirectoryName(Me.m_core.DataSource.ToString())
        sfd.Filter = "NETWRK files|*.dat|SCOR files|*.scor"
        sfd.FilterIndex = 0

        If (sfd.ShowDialog() <> DialogResult.OK) Then Return
        If String.IsNullOrWhiteSpace(sfd.FileName) Then Return

        Me.WriteFile(sfd.FileName)
    End Sub

    Public ReadOnly Property MenuItemLocation As String _
        Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuFile"
        End Get
    End Property

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Stuart Borrett, Sheila Heymans, Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for EwE6 that writes Ecopath values to a NETWRK-compatible SCOR file"
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(core, cCore)
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property Name As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return "m_tsmiFileSaveAs2"
        End Get
    End Property

    Public Sub EcopathRunCompleted(ByRef EcopathDataStructures As Object) _
        Implements EwEPlugin.IEcopathRunCompletedPlugin.EcopathRunCompleted
        Me.m_epData = DirectCast(EcopathDataStructures, cEcopathDataStructures)
    End Sub

#Region " Internals "

    Private Sub WriteFile(ByVal strFileName As String)

        Dim wr As New cSCORWriter(Me.m_epData)
        Dim msg As cMessage = Nothing

        If wr.Write(strFileName) Then
            msg = New cMessage(String.Format("SCOR file saved to '{0}'", strFileName), _
                               eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.EcoPath, eMessageImportance.Information)
            msg.Hyperlink = Path.GetDirectoryName(strFileName)
        Else
            msg = New cMessage(String.Format("SCOR file could not be saved to '{0}'", strFileName), _
                               eMessageType.DataExport, EwEUtils.Core.eCoreComponentType.EcoPath, eMessageImportance.Warning)
        End If
        Me.m_core.Messages.SendMessage(msg)

    End Sub

#End Region ' Internals

End Class
