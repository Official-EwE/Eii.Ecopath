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
Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

Partial Class frmOceanViz
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOceanViz))
        Me.m_tsMain = New ScientificInterfaceShared.Controls.cEwEToolstrip()
        Me.m_tsbnSnapshot = New System.Windows.Forms.ToolStripButton()
        Me.m_tsbnFish = New System.Windows.Forms.ToolStripButton()
        Me.m_tscmbFish = New System.Windows.Forms.ToolStripComboBox()
        Me.m_tsbnSettings = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.m_tsbnSend = New System.Windows.Forms.ToolStripButton()
        Me.m_cmdCommand = New System.Windows.Forms.ToolStripComboBox()
        Me.m_pbStub = New System.Windows.Forms.PictureBox()
        Me.m_tsMain.SuspendLayout()
        CType(Me.m_pbStub, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tsMain
        '
        Me.m_tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.m_tsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tsbnSnapshot, Me.m_tsbnFish, Me.m_tscmbFish, Me.m_tsbnSettings, Me.ToolStripSeparator1, Me.m_tsbnSend, Me.m_cmdCommand})
        Me.m_tsMain.Location = New System.Drawing.Point(0, 0)
        Me.m_tsMain.Name = "m_tsMain"
        Me.m_tsMain.Size = New System.Drawing.Size(757, 25)
        Me.m_tsMain.TabIndex = 0
        '
        'm_tsbnSnapshot
        '
        Me.m_tsbnSnapshot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbnSnapshot.Image = CType(resources.GetObject("m_tsbnSnapshot.Image"), System.Drawing.Image)
        Me.m_tsbnSnapshot.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnSnapshot.Name = "m_tsbnSnapshot"
        Me.m_tsbnSnapshot.Size = New System.Drawing.Size(23, 22)
        Me.m_tsbnSnapshot.Text = "Snapshot"
        '
        'm_tsbnFish
        '
        Me.m_tsbnFish.Image = CType(resources.GetObject("m_tsbnFish.Image"), System.Drawing.Image)
        Me.m_tsbnFish.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnFish.Name = "m_tsbnFish"
        Me.m_tsbnFish.Size = New System.Drawing.Size(30, 22)
        Me.m_tsbnFish.Text = ":"
        '
        'm_tscmbFish
        '
        Me.m_tscmbFish.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_tscmbFish.Items.AddRange(New Object() {"(None)"})
        Me.m_tscmbFish.Name = "m_tscmbFish"
        Me.m_tscmbFish.Size = New System.Drawing.Size(121, 25)
        '
        'm_tsbnSettings
        '
        Me.m_tsbnSettings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_tsbnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.m_tsbnSettings.Image = CType(resources.GetObject("m_tsbnSettings.Image"), System.Drawing.Image)
        Me.m_tsbnSettings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnSettings.Name = "m_tsbnSettings"
        Me.m_tsbnSettings.Size = New System.Drawing.Size(29, 22)
        Me.m_tsbnSettings.Text = "ToolStripDropDownButton1"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'm_tsbnSend
        '
        Me.m_tsbnSend.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_tsbnSend.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.m_tsbnSend.Image = CType(resources.GetObject("m_tsbnSend.Image"), System.Drawing.Image)
        Me.m_tsbnSend.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.m_tsbnSend.Name = "m_tsbnSend"
        Me.m_tsbnSend.Size = New System.Drawing.Size(37, 22)
        Me.m_tsbnSend.Text = "Send"
        '
        'm_cmdCommand
        '
        Me.m_cmdCommand.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.m_cmdCommand.Name = "m_cmdCommand"
        Me.m_cmdCommand.Size = New System.Drawing.Size(121, 25)
        '
        'm_pbStub
        '
        Me.m_pbStub.BackgroundImage = Global.EwEOceanVizPlugin.My.Resources.Resources.screencap
        Me.m_pbStub.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbStub.Location = New System.Drawing.Point(0, 25)
        Me.m_pbStub.Name = "m_pbStub"
        Me.m_pbStub.Size = New System.Drawing.Size(757, 472)
        Me.m_pbStub.TabIndex = 1
        Me.m_pbStub.TabStop = False
        '
        'frmOceanViz
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(757, 497)
        Me.Controls.Add(Me.m_pbStub)
        Me.Controls.Add(Me.m_tsMain)
        Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmOceanViz"
        Me.TabText = ""
        Me.Text = "frmOceanViz"
        Me.m_tsMain.ResumeLayout(False)
        Me.m_tsMain.PerformLayout()
        CType(Me.m_pbStub, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_tsbnSnapshot As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tsbnFish As System.Windows.Forms.ToolStripButton
    Private WithEvents m_tscmbFish As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_tsbnSettings As System.Windows.Forms.ToolStripDropDownButton
    Private WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Private WithEvents m_tsbnSend As System.Windows.Forms.ToolStripButton
    Private WithEvents m_cmdCommand As System.Windows.Forms.ToolStripComboBox
    Private WithEvents m_pbStub As System.Windows.Forms.PictureBox
    Private WithEvents m_tsMain As cEwEToolstrip
End Class
