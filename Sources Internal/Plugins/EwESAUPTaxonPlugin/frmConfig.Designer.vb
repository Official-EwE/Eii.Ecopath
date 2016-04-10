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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConfig
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConfig))
        Me.m_lbHost = New System.Windows.Forms.Label
        Me.m_lbDatabase = New System.Windows.Forms.Label
        Me.m_lbUsername = New System.Windows.Forms.Label
        Me.m_lbPassword = New System.Windows.Forms.Label
        Me.m_tbSQLHost = New System.Windows.Forms.TextBox
        Me.m_tbSQLDatabase = New System.Windows.Forms.TextBox
        Me.m_tbSQLUsername = New System.Windows.Forms.TextBox
        Me.m_tbSQLPassword = New System.Windows.Forms.TextBox
        Me.m_btnConnect = New System.Windows.Forms.Button
        Me.m_btnDisconnect = New System.Windows.Forms.Button
        Me.m_rbAccess = New System.Windows.Forms.RadioButton
        Me.m_lbFile = New System.Windows.Forms.Label
        Me.m_tbAccessDatabase = New System.Windows.Forms.TextBox
        Me.m_btnBrowseAccessDatabase = New System.Windows.Forms.Button
        Me.m_rbSQLServer = New System.Windows.Forms.RadioButton
        Me.m_tlpButtons = New System.Windows.Forms.TableLayoutPanel
        Me.m_plLogo = New System.Windows.Forms.Panel
        Me.m_tlpButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_lbHost
        '
        resources.ApplyResources(Me.m_lbHost, "m_lbHost")
        Me.m_lbHost.Name = "m_lbHost"
        '
        'm_lbDatabase
        '
        resources.ApplyResources(Me.m_lbDatabase, "m_lbDatabase")
        Me.m_lbDatabase.Name = "m_lbDatabase"
        '
        'm_lbUsername
        '
        resources.ApplyResources(Me.m_lbUsername, "m_lbUsername")
        Me.m_lbUsername.Name = "m_lbUsername"
        '
        'm_lbPassword
        '
        resources.ApplyResources(Me.m_lbPassword, "m_lbPassword")
        Me.m_lbPassword.Name = "m_lbPassword"
        '
        'm_tbSQLHost
        '
        resources.ApplyResources(Me.m_tbSQLHost, "m_tbSQLHost")
        Me.m_tbSQLHost.Name = "m_tbSQLHost"
        '
        'm_tbSQLDatabase
        '
        resources.ApplyResources(Me.m_tbSQLDatabase, "m_tbSQLDatabase")
        Me.m_tbSQLDatabase.Name = "m_tbSQLDatabase"
        '
        'm_tbSQLUsername
        '
        resources.ApplyResources(Me.m_tbSQLUsername, "m_tbSQLUsername")
        Me.m_tbSQLUsername.Name = "m_tbSQLUsername"
        '
        'm_tbSQLPassword
        '
        resources.ApplyResources(Me.m_tbSQLPassword, "m_tbSQLPassword")
        Me.m_tbSQLPassword.Name = "m_tbSQLPassword"
        '
        'm_btnConnect
        '
        resources.ApplyResources(Me.m_btnConnect, "m_btnConnect")
        Me.m_btnConnect.Name = "m_btnConnect"
        Me.m_btnConnect.UseVisualStyleBackColor = True
        '
        'm_btnDisconnect
        '
        Me.m_btnDisconnect.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.m_btnDisconnect, "m_btnDisconnect")
        Me.m_btnDisconnect.Name = "m_btnDisconnect"
        Me.m_btnDisconnect.UseVisualStyleBackColor = True
        '
        'm_rbAccess
        '
        resources.ApplyResources(Me.m_rbAccess, "m_rbAccess")
        Me.m_rbAccess.Name = "m_rbAccess"
        Me.m_rbAccess.TabStop = True
        Me.m_rbAccess.UseVisualStyleBackColor = True
        '
        'm_lbFile
        '
        resources.ApplyResources(Me.m_lbFile, "m_lbFile")
        Me.m_lbFile.Name = "m_lbFile"
        '
        'm_tbAccessDatabase
        '
        resources.ApplyResources(Me.m_tbAccessDatabase, "m_tbAccessDatabase")
        Me.m_tbAccessDatabase.Name = "m_tbAccessDatabase"
        '
        'm_btnBrowseAccessDatabase
        '
        resources.ApplyResources(Me.m_btnBrowseAccessDatabase, "m_btnBrowseAccessDatabase")
        Me.m_btnBrowseAccessDatabase.Name = "m_btnBrowseAccessDatabase"
        Me.m_btnBrowseAccessDatabase.UseVisualStyleBackColor = True
        '
        'm_rbSQLServer
        '
        resources.ApplyResources(Me.m_rbSQLServer, "m_rbSQLServer")
        Me.m_rbSQLServer.Name = "m_rbSQLServer"
        Me.m_rbSQLServer.TabStop = True
        Me.m_rbSQLServer.UseVisualStyleBackColor = True
        '
        'm_tlpButtons
        '
        resources.ApplyResources(Me.m_tlpButtons, "m_tlpButtons")
        Me.m_tlpButtons.Controls.Add(Me.m_btnConnect, 1, 0)
        Me.m_tlpButtons.Controls.Add(Me.m_btnDisconnect, 2, 0)
        Me.m_tlpButtons.Name = "m_tlpButtons"
        '
        'm_plLogo
        '
        resources.ApplyResources(Me.m_plLogo, "m_plLogo")
        Me.m_plLogo.BackColor = System.Drawing.Color.White
        Me.m_plLogo.BackgroundImage = Global.EwESAUPTaxonPlugin.My.Resources.Resources.sauplogo_vert
        Me.m_plLogo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.m_plLogo.Name = "m_plLogo"
        '
        'frmConfig
        '
        Me.AcceptButton = Me.m_btnConnect
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_plLogo)
        Me.Controls.Add(Me.m_tlpButtons)
        Me.Controls.Add(Me.m_rbSQLServer)
        Me.Controls.Add(Me.m_btnBrowseAccessDatabase)
        Me.Controls.Add(Me.m_tbAccessDatabase)
        Me.Controls.Add(Me.m_lbFile)
        Me.Controls.Add(Me.m_rbAccess)
        Me.Controls.Add(Me.m_tbSQLPassword)
        Me.Controls.Add(Me.m_tbSQLUsername)
        Me.Controls.Add(Me.m_tbSQLDatabase)
        Me.Controls.Add(Me.m_tbSQLHost)
        Me.Controls.Add(Me.m_lbPassword)
        Me.Controls.Add(Me.m_lbUsername)
        Me.Controls.Add(Me.m_lbDatabase)
        Me.Controls.Add(Me.m_lbHost)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConfig"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.m_tlpButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_lbHost As System.Windows.Forms.Label
    Private WithEvents m_tbSQLHost As System.Windows.Forms.TextBox
    Private WithEvents m_lbDatabase As System.Windows.Forms.Label
    Private WithEvents m_tbSQLDatabase As System.Windows.Forms.TextBox
    Private WithEvents m_tbSQLUsername As System.Windows.Forms.TextBox
    Private WithEvents m_lbUsername As System.Windows.Forms.Label
    Private WithEvents m_tbSQLPassword As System.Windows.Forms.TextBox
    Private WithEvents m_lbPassword As System.Windows.Forms.Label
    Private WithEvents m_btnConnect As System.Windows.Forms.Button
    Private WithEvents m_btnDisconnect As System.Windows.Forms.Button
    Friend WithEvents m_lbFile As System.Windows.Forms.Label
    Private WithEvents m_tbAccessDatabase As System.Windows.Forms.TextBox
    Private WithEvents m_btnBrowseAccessDatabase As System.Windows.Forms.Button
    Private WithEvents m_tlpButtons As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_rbAccess As System.Windows.Forms.RadioButton
    Private WithEvents m_rbSQLServer As System.Windows.Forms.RadioButton
    Private WithEvents m_plLogo As System.Windows.Forms.Panel
End Class
