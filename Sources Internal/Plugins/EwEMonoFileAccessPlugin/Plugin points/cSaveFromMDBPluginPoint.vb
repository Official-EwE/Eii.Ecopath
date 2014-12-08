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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports System.Windows.Forms
Imports EwECore
Imports EwECore.Database
Imports EwECore.DataSources
Imports EwEPlugin
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports

Public Class cSavePluginPoint
    Implements IMenuItemPlugin

    Private m_core As cCore = Nothing

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Save an Ecopath model database to a flat text file for MONO."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "m_tsmiFileSaveAs3"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.mono
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Save Model Database To .eiixml File..."
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return Me.ControlText
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.Idle
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        Dim ofd As New OpenFileDialog()
        Dim msg As cMessage = Nothing

        ofd.Filter = My.Resources.FILEFILTER_DB
        ofd.CheckFileExists = True
        ofd.RestoreDirectory = True

        If (ofd.ShowDialog() <> DialogResult.OK) Then Return
        Dim ds As IEwEDataSource = cDataSourceFactory.Create(ofd.FileName)
        If Not (TypeOf ds Is cDBDataSource) Then Return
        Dim dbds As cDBDataSource = DirectCast(ds, cDBDataSource)
        If Not (TypeOf dbds.Connection Is cEwEAccessDatabase) Then Return
        Dim db As cEwEAccessDatabase = DirectCast(dbds.Connection, cEwEAccessDatabase)

        If (db.Open(ofd.FileName) <> EwEUtils.Core.eDatasourceAccessType.Opened) Then Return

        Try
            Dim strPath As String = Path.Combine( _
                Path.GetDirectoryName(ofd.FileName), _
                Path.GetFileNameWithoutExtension(ofd.FileName) & ".eiixml")

            ds = cDataSourceFactory.Create(EwEUtils.Core.eDataSourceTypes.EIIXML)
            If DirectCast(ds, cEIIXMLDataSource).SaveFromDB(db, strPath) Then
                ' ToDo: globalize this
                msg = New cMessage("EIIXML database saved to " & strPath, eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strPath)
                Me.m_core.Messages.SendMessage(msg)
            End If
            ds.Close()

        Catch ex As Exception

        End Try
        db.Close()

    End Sub

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuFile"
        End Get
    End Property

End Class
