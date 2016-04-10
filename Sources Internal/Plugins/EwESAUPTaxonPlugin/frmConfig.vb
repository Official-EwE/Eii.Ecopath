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

Option Strict On
Imports System.Windows.Forms

''' ---------------------------------------------------------------------------
''' <summary>
''' Interface for configuring the SAUP taxon table connection.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class frmConfig

    ''' <summary>Plug-in to configure.</summary>
    Private m_plugin As cSAUPTaxonDataPlugin = Nothing

    Public Sub New(ByVal plugin As cSAUPTaxonDataPlugin)
        MyBase.New()

        ' Store refs
        Me.m_plugin = plugin
        ' Init controls
        Me.InitializeComponent()

        ' Init UI state
        Select Case Me.m_plugin.ConnectionType

            Case cSAUPTaxonDataPlugin.eConnectionType.Access
                Me.m_rbSQLServer.Checked = True
            Case cSAUPTaxonDataPlugin.eConnectionType.SQLServer
                Me.m_rbSQLServer.Checked = True
            Case Else
                Debug.Assert(False)

        End Select

        Me.m_tbAccessDatabase.Text = Me.m_plugin.AccessDatabase
        Me.m_tbSQLHost.Text = Me.m_plugin.SQLHost
        Me.m_tbSQLDatabase.Text = Me.m_plugin.SQLDatabase
        Me.m_tbSQLUsername.Text = Me.m_plugin.SQLUserName
        Me.m_tbSQLPassword.Text = Me.m_plugin.SQLPassword

    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        Me.m_plugin.ReadConfiguration()
        Me.UpdateControls()
    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)
        Me.UpdatePlugin()
        MyBase.OnFormClosed(e)
    End Sub

    Private Sub UpdateControls()

        Dim bIsConnected As Boolean = Me.m_plugin.IsConnected
        Dim bCanConnect As Boolean = False
        Dim bAccess As Boolean = (Me.m_rbAccess.Checked = True)
        Dim bSQLSrv As Boolean = (Me.m_rbSQLServer.Checked = True)

        If bAccess Then
            bCanConnect = (Me.m_tbAccessDatabase.Text <> "")
        ElseIf bSQLSrv Then
            bCanConnect = (Me.m_tbSQLHost.Text <> "") And _
                          (Me.m_tbSQLDatabase.Text <> "") And _
                          (Me.m_tbSQLPassword.Text <> "") And _
                          (Me.m_tbSQLUsername.Text <> "")
        End If

        Me.m_rbAccess.Enabled = Not bIsConnected
        Me.m_tbAccessDatabase.Enabled = Not bIsConnected
        Me.m_btnBrowseAccessDatabase.Enabled = Not bIsConnected
        Me.m_rbSQLServer.Enabled = Not bIsConnected
        Me.m_tbSQLHost.Enabled = Not bIsConnected
        Me.m_tbSQLDatabase.Enabled = Not bIsConnected
        Me.m_tbSQLUsername.Enabled = Not bIsConnected
        Me.m_tbSQLPassword.Enabled = Not bIsConnected

        Me.m_btnConnect.Enabled = Not bIsConnected And bCanConnect
        Me.m_btnDisconnect.Enabled = bIsConnected

    End Sub

    Private Sub UpdatePlugin()

        If Me.m_rbAccess.Checked Then
            Me.m_plugin.ConnectionType = cSAUPTaxonDataPlugin.eConnectionType.Access
        ElseIf Me.m_rbSQLServer.Checked Then
            Me.m_plugin.ConnectionType = cSAUPTaxonDataPlugin.eConnectionType.SQLServer
        End If
        Me.m_plugin.AccessDatabase = Me.m_tbAccessDatabase.Text
        Me.m_plugin.SQLDatabase = Me.m_tbSQLDatabase.Text
        Me.m_plugin.SQLHost = Me.m_tbSQLHost.Text
        Me.m_plugin.SQLUserName = Me.m_tbSQLUsername.Text
        Me.m_plugin.SQLPassword = Me.m_tbSQLPassword.Text
        Me.m_plugin.WriteConfiguration()

    End Sub

#Region " Access controls "

    Private Sub OnFocusAccessControl(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbAccessDatabase.GotFocus
        Me.m_rbAccess.Checked = True
    End Sub

    Private Sub m_tbAccess_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbAccessDatabase.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnBrowseAccessDatabase(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_btnBrowseAccessDatabase.Click

        Me.m_rbAccess.Checked = True

        Dim dlg As New OpenFileDialog()

        dlg.CheckFileExists = True
        dlg.CheckPathExists = True
        dlg.AutoUpgradeEnabled = True
        dlg.Multiselect = False
        dlg.Filter = "Access databases|*.mdb;*.accdb|All files|*.*"
        dlg.FilterIndex = 0
        dlg.FileName = Me.m_tbAccessDatabase.Text

        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.m_tbAccessDatabase.Text = dlg.FileName
        End If

    End Sub

#End Region ' Access controls

#Region " SQL Server controls "

    Private Sub OnFocusSQLSrvControl(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_tbSQLHost.GotFocus, _
                m_tbSQLDatabase.GotFocus, _
                m_tbSQLUsername.GotFocus, _
                m_tbSQLPassword.GotFocus
        Me.m_rbSQLServer.Checked = True
    End Sub

    Private Sub m_tbComputer_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbSQLHost.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub m_tbDatabase_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbSQLDatabase.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub m_tbUsername_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbSQLUsername.TextChanged
        Me.UpdateControls()
    End Sub

    Private Sub m_tbPassword_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_tbSQLPassword.TextChanged
        Me.UpdateControls()
    End Sub

#End Region ' SQL Server controls

#Region " Generic controls "

    Private Sub OnConnect(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnConnect.Click

        ' Sanity check
        Debug.Assert(Not Me.m_plugin.IsConnected, "Not properly used")

        Me.Cursor = Windows.Forms.Cursors.WaitCursor
        Me.UpdatePlugin()
        Me.m_plugin.Connect()
        Me.Cursor = Windows.Forms.Cursors.Default

        If Not Me.m_plugin.IsConnected Then
            MsgBox("Unable to connect", MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
        Else
            Me.DialogResult = Windows.Forms.DialogResult.OK
            Me.Close()
        End If

    End Sub

    Private Sub OnDisconnect(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles m_btnDisconnect.Click

        ' Sanity check
        Debug.Assert(Me.m_plugin.IsConnected, "Not properly used")

        Me.m_plugin.Disconnect()
        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub

    Private Sub OnDatabaseTypeChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
        Handles m_rbAccess.CheckedChanged, _
                m_rbSQLServer.CheckedChanged
        Me.UpdateControls()
    End Sub

    Private Sub OnCancel(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

#End Region ' Generic controls

End Class