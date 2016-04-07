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
Partial Class ucConfig
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucConfig))
        Me.m_lblWebUser = New System.Windows.Forms.Label()
        Me.m_lblWebPwd = New System.Windows.Forms.Label()
        Me.m_tbxWebAccount = New System.Windows.Forms.TextBox()
        Me.m_tbxWebPwd = New System.Windows.Forms.TextBox()
        Me.m_tlpAll = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbBlueBridge = New System.Windows.Forms.PictureBox()
        Me.m_plBits = New System.Windows.Forms.Panel()
        Me.m_tlpConnection = New System.Windows.Forms.TableLayoutPanel()
        Me.m_btnConnect = New System.Windows.Forms.Button()
        Me.m_btnDisconnect = New System.Windows.Forms.Button()
        Me.m_cmbMaxResults = New System.Windows.Forms.ComboBox()
        Me.m_lblNumResults = New System.Windows.Forms.Label()
        Me.m_hdrSearch = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_hdrConnection = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
        Me.m_tbxWebPort = New System.Windows.Forms.TextBox()
        Me.m_tbxWebServer = New System.Windows.Forms.TextBox()
        Me.m_lblWebPort = New System.Windows.Forms.Label()
        Me.m_tbxAccess = New System.Windows.Forms.TextBox()
        Me.m_lblWebServer = New System.Windows.Forms.Label()
        Me.m_rbWebService = New System.Windows.Forms.RadioButton()
        Me.m_rbAccess = New System.Windows.Forms.RadioButton()
        Me.m_btnToggleViewChars = New System.Windows.Forms.Button()
        Me.m_btnPickAccess = New System.Windows.Forms.Button()
        Me.m_pbFishBase = New System.Windows.Forms.PictureBox()
        Me.m_tlpAll.SuspendLayout()
        CType(Me.m_pbBlueBridge, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_plBits.SuspendLayout()
        Me.m_tlpConnection.SuspendLayout()
        CType(Me.m_pbFishBase, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_lblWebUser
        '
        resources.ApplyResources(Me.m_lblWebUser, "m_lblWebUser")
        Me.m_lblWebUser.Name = "m_lblWebUser"
        '
        'm_lblWebPwd
        '
        resources.ApplyResources(Me.m_lblWebPwd, "m_lblWebPwd")
        Me.m_lblWebPwd.Name = "m_lblWebPwd"
        '
        'm_tbxWebAccount
        '
        resources.ApplyResources(Me.m_tbxWebAccount, "m_tbxWebAccount")
        Me.m_tbxWebAccount.Name = "m_tbxWebAccount"
        '
        'm_tbxWebPwd
        '
        resources.ApplyResources(Me.m_tbxWebPwd, "m_tbxWebPwd")
        Me.m_tbxWebPwd.Name = "m_tbxWebPwd"
        '
        'm_tlpAll
        '
        Me.m_tlpAll.BackColor = System.Drawing.Color.Transparent
        resources.ApplyResources(Me.m_tlpAll, "m_tlpAll")
        Me.m_tlpAll.Controls.Add(Me.m_pbBlueBridge, 0, 2)
        Me.m_tlpAll.Controls.Add(Me.m_plBits, 0, 1)
        Me.m_tlpAll.Controls.Add(Me.m_pbFishBase, 0, 0)
        Me.m_tlpAll.Name = "m_tlpAll"
        '
        'm_pbBlueBridge
        '
        Me.m_pbBlueBridge.BackgroundImage = Global.EwEFishBasePlugin.My.Resources.Resources.BlueBridge_xparent
        resources.ApplyResources(Me.m_pbBlueBridge, "m_pbBlueBridge")
        Me.m_pbBlueBridge.Name = "m_pbBlueBridge"
        Me.m_pbBlueBridge.TabStop = False
        '
        'm_plBits
        '
        Me.m_plBits.BackColor = System.Drawing.SystemColors.Control
        Me.m_plBits.Controls.Add(Me.m_tlpConnection)
        Me.m_plBits.Controls.Add(Me.m_cmbMaxResults)
        Me.m_plBits.Controls.Add(Me.m_lblNumResults)
        Me.m_plBits.Controls.Add(Me.m_hdrSearch)
        Me.m_plBits.Controls.Add(Me.m_hdrConnection)
        Me.m_plBits.Controls.Add(Me.m_tbxWebPort)
        Me.m_plBits.Controls.Add(Me.m_tbxWebServer)
        Me.m_plBits.Controls.Add(Me.m_lblWebPort)
        Me.m_plBits.Controls.Add(Me.m_tbxAccess)
        Me.m_plBits.Controls.Add(Me.m_lblWebServer)
        Me.m_plBits.Controls.Add(Me.m_rbWebService)
        Me.m_plBits.Controls.Add(Me.m_rbAccess)
        Me.m_plBits.Controls.Add(Me.m_btnToggleViewChars)
        Me.m_plBits.Controls.Add(Me.m_btnPickAccess)
        Me.m_plBits.Controls.Add(Me.m_lblWebUser)
        Me.m_plBits.Controls.Add(Me.m_tbxWebAccount)
        Me.m_plBits.Controls.Add(Me.m_tbxWebPwd)
        Me.m_plBits.Controls.Add(Me.m_lblWebPwd)
        resources.ApplyResources(Me.m_plBits, "m_plBits")
        Me.m_plBits.Name = "m_plBits"
        '
        'm_tlpConnection
        '
        resources.ApplyResources(Me.m_tlpConnection, "m_tlpConnection")
        Me.m_tlpConnection.Controls.Add(Me.m_btnConnect, 0, 0)
        Me.m_tlpConnection.Controls.Add(Me.m_btnDisconnect, 1, 0)
        Me.m_tlpConnection.Name = "m_tlpConnection"
        '
        'm_btnConnect
        '
        resources.ApplyResources(Me.m_btnConnect, "m_btnConnect")
        Me.m_btnConnect.Name = "m_btnConnect"
        '
        'm_btnDisconnect
        '
        resources.ApplyResources(Me.m_btnDisconnect, "m_btnDisconnect")
        Me.m_btnDisconnect.Name = "m_btnDisconnect"
        '
        'm_cmbMaxResults
        '
        resources.ApplyResources(Me.m_cmbMaxResults, "m_cmbMaxResults")
        Me.m_cmbMaxResults.FormattingEnabled = True
        Me.m_cmbMaxResults.Items.AddRange(New Object() {resources.GetString("m_cmbMaxResults.Items"), resources.GetString("m_cmbMaxResults.Items1"), resources.GetString("m_cmbMaxResults.Items2"), resources.GetString("m_cmbMaxResults.Items3"), resources.GetString("m_cmbMaxResults.Items4"), resources.GetString("m_cmbMaxResults.Items5")})
        Me.m_cmbMaxResults.Name = "m_cmbMaxResults"
        '
        'm_lblNumResults
        '
        resources.ApplyResources(Me.m_lblNumResults, "m_lblNumResults")
        Me.m_lblNumResults.Name = "m_lblNumResults"
        '
        'm_hdrSearch
        '
        resources.ApplyResources(Me.m_hdrSearch, "m_hdrSearch")
        Me.m_hdrSearch.CanCollapseParent = False
        Me.m_hdrSearch.CollapsedParentHeight = 0
        Me.m_hdrSearch.IsCollapsed = False
        Me.m_hdrSearch.Name = "m_hdrSearch"
        '
        'm_hdrConnection
        '
        resources.ApplyResources(Me.m_hdrConnection, "m_hdrConnection")
        Me.m_hdrConnection.CanCollapseParent = False
        Me.m_hdrConnection.CollapsedParentHeight = 0
        Me.m_hdrConnection.IsCollapsed = False
        Me.m_hdrConnection.Name = "m_hdrConnection"
        '
        'm_tbxWebPort
        '
        resources.ApplyResources(Me.m_tbxWebPort, "m_tbxWebPort")
        Me.m_tbxWebPort.Name = "m_tbxWebPort"
        '
        'm_tbxWebServer
        '
        resources.ApplyResources(Me.m_tbxWebServer, "m_tbxWebServer")
        Me.m_tbxWebServer.Name = "m_tbxWebServer"
        '
        'm_lblWebPort
        '
        resources.ApplyResources(Me.m_lblWebPort, "m_lblWebPort")
        Me.m_lblWebPort.Name = "m_lblWebPort"
        '
        'm_tbxAccess
        '
        resources.ApplyResources(Me.m_tbxAccess, "m_tbxAccess")
        Me.m_tbxAccess.Name = "m_tbxAccess"
        '
        'm_lblWebServer
        '
        resources.ApplyResources(Me.m_lblWebServer, "m_lblWebServer")
        Me.m_lblWebServer.Name = "m_lblWebServer"
        '
        'm_rbWebService
        '
        resources.ApplyResources(Me.m_rbWebService, "m_rbWebService")
        Me.m_rbWebService.Name = "m_rbWebService"
        Me.m_rbWebService.TabStop = True
        Me.m_rbWebService.UseVisualStyleBackColor = True
        '
        'm_rbAccess
        '
        resources.ApplyResources(Me.m_rbAccess, "m_rbAccess")
        Me.m_rbAccess.Name = "m_rbAccess"
        Me.m_rbAccess.TabStop = True
        Me.m_rbAccess.UseVisualStyleBackColor = True
        '
        'm_btnToggleViewChars
        '
        resources.ApplyResources(Me.m_btnToggleViewChars, "m_btnToggleViewChars")
        Me.m_btnToggleViewChars.Name = "m_btnToggleViewChars"
        '
        'm_btnPickAccess
        '
        resources.ApplyResources(Me.m_btnPickAccess, "m_btnPickAccess")
        Me.m_btnPickAccess.Name = "m_btnPickAccess"
        '
        'm_pbFishBase
        '
        Me.m_pbFishBase.BackColor = System.Drawing.Color.White
        Me.m_pbFishBase.BackgroundImage = Global.EwEFishBasePlugin.My.Resources.Resources.fblogo_new
        resources.ApplyResources(Me.m_pbFishBase, "m_pbFishBase")
        Me.m_pbFishBase.Name = "m_pbFishBase"
        Me.m_pbFishBase.TabStop = False
        '
        'ucConfig
        '
        Me.Controls.Add(Me.m_tlpAll)
        Me.Name = "ucConfig"
        resources.ApplyResources(Me, "$this")
        Me.m_tlpAll.ResumeLayout(False)
        CType(Me.m_pbBlueBridge, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_plBits.ResumeLayout(False)
        Me.m_plBits.PerformLayout()
        Me.m_tlpConnection.ResumeLayout(False)
        CType(Me.m_pbFishBase, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_lblWebUser As System.Windows.Forms.Label
    Private WithEvents m_tbxWebAccount As System.Windows.Forms.TextBox
    Private WithEvents m_lblWebPwd As System.Windows.Forms.Label
    Private WithEvents m_tbxWebPwd As System.Windows.Forms.TextBox
    Private WithEvents m_tlpAll As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_rbWebService As System.Windows.Forms.RadioButton
    Private WithEvents m_tbxWebPort As System.Windows.Forms.TextBox
    Private WithEvents m_tbxWebServer As System.Windows.Forms.TextBox
    Private WithEvents m_lblWebPort As System.Windows.Forms.Label
    Private WithEvents m_lblWebServer As System.Windows.Forms.Label
    Private WithEvents m_cmbMaxResults As System.Windows.Forms.ComboBox
    Private WithEvents m_lblNumResults As System.Windows.Forms.Label
    Private WithEvents m_hdrSearch As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_hdrConnection As ScientificInterfaceShared.Controls.cEwEHeaderLabel
    Private WithEvents m_plBits As System.Windows.Forms.Panel
    Private WithEvents m_tbxAccess As System.Windows.Forms.TextBox
    Private WithEvents m_rbAccess As System.Windows.Forms.RadioButton
    Private WithEvents m_btnPickAccess As System.Windows.Forms.Button
    Private WithEvents m_btnToggleViewChars As System.Windows.Forms.Button
    Private WithEvents m_tlpConnection As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_btnConnect As System.Windows.Forms.Button
    Private WithEvents m_btnDisconnect As System.Windows.Forms.Button
    Private WithEvents m_pbFishBase As System.Windows.Forms.PictureBox
    Private WithEvents m_pbBlueBridge As System.Windows.Forms.PictureBox
End Class
