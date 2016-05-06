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

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucAcknowledgements
    Inherits System.Windows.Forms.UserControl

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.m_tlpText = New System.Windows.Forms.TableLayoutPanel()
        Me.m_lblAckVal = New System.Windows.Forms.Label()
        Me.m_lblRefVal = New System.Windows.Forms.Label()
        Me.m_llContactVal = New System.Windows.Forms.LinkLabel()
        Me.m_lblAck = New System.Windows.Forms.Label()
        Me.m_lblRef = New System.Windows.Forms.Label()
        Me.m_lblContact = New System.Windows.Forms.Label()
        Me.m_lblGrant = New System.Windows.Forms.Label()
        Me.m_lblGrantVal = New System.Windows.Forms.Label()
        Me.m_tlpImages = New System.Windows.Forms.TableLayoutPanel()
        Me.m_pbIPN = New System.Windows.Forms.PictureBox()
        Me.m_pbCicimar = New System.Windows.Forms.PictureBox()
        Me.m_pbConacyt = New System.Windows.Forms.PictureBox()
        Me.m_tlpText.SuspendLayout()
        Me.m_tlpImages.SuspendLayout()
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'm_tlpText
        '
        Me.m_tlpText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpText.ColumnCount = 2
        Me.m_tlpText.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.m_tlpText.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpText.Controls.Add(Me.m_lblAckVal, 1, 0)
        Me.m_tlpText.Controls.Add(Me.m_lblRefVal, 1, 1)
        Me.m_tlpText.Controls.Add(Me.m_llContactVal, 1, 2)
        Me.m_tlpText.Controls.Add(Me.m_lblAck, 0, 0)
        Me.m_tlpText.Controls.Add(Me.m_lblRef, 0, 1)
        Me.m_tlpText.Controls.Add(Me.m_lblContact, 0, 2)
        Me.m_tlpText.Controls.Add(Me.m_lblGrant, 0, 3)
        Me.m_tlpText.Controls.Add(Me.m_lblGrantVal, 1, 3)
        Me.m_tlpText.Location = New System.Drawing.Point(3, 3)
        Me.m_tlpText.Name = "m_tlpText"
        Me.m_tlpText.RowCount = 5
        Me.m_tlpText.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpText.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpText.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpText.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.m_tlpText.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpText.Size = New System.Drawing.Size(551, 121)
        Me.m_tlpText.TabIndex = 10
        '
        'm_lblAckVal
        '
        Me.m_lblAckVal.AutoSize = True
        Me.m_lblAckVal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblAckVal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblAckVal.Location = New System.Drawing.Point(112, 3)
        Me.m_lblAckVal.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblAckVal.Name = "m_lblAckVal"
        Me.m_lblAckVal.Size = New System.Drawing.Size(436, 26)
        Me.m_lblAckVal.TabIndex = 6
        Me.m_lblAckVal.Text = "Instituto Politécnico Nacional -Centro Interdisciplinario de Ciencias Marinas (IP" & _
    "N-CICIMAR)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Consejo Nacional de Ciencia y Tecnología (CONACyT)"
        '
        'm_lblRefVal
        '
        Me.m_lblRefVal.AutoSize = True
        Me.m_lblRefVal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblRefVal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblRefVal.Location = New System.Drawing.Point(112, 35)
        Me.m_lblRefVal.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblRefVal.Name = "m_lblRefVal"
        Me.m_lblRefVal.Size = New System.Drawing.Size(436, 39)
        Me.m_lblRefVal.TabIndex = 7
        Me.m_lblRefVal.Text = "Arreguı́n-Sánchez, F., 2014. Measuring resilience in aquatic trophic networks fro" & _
    "m supply-demand of energy relationships. Ecological Modelling 272, 271-276." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'm_llContactVal
        '
        Me.m_llContactVal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_llContactVal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_llContactVal.LinkArea = New System.Windows.Forms.LinkArea(28, 44)
        Me.m_llContactVal.Location = New System.Drawing.Point(112, 80)
        Me.m_llContactVal.Margin = New System.Windows.Forms.Padding(3)
        Me.m_llContactVal.Name = "m_llContactVal"
        Me.m_llContactVal.Size = New System.Drawing.Size(436, 17)
        Me.m_llContactVal.TabIndex = 8
        Me.m_llContactVal.TabStop = True
        Me.m_llContactVal.Text = "Dr. Manuel J. Zetina-Rejon, mzetina@ipn.mx" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        Me.m_llContactVal.UseCompatibleTextRendering = True
        '
        'm_lblAck
        '
        Me.m_lblAck.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblAck.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblAck.Location = New System.Drawing.Point(3, 3)
        Me.m_lblAck.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblAck.Name = "m_lblAck"
        Me.m_lblAck.Size = New System.Drawing.Size(103, 26)
        Me.m_lblAck.TabIndex = 9
        Me.m_lblAck.Text = "Acknowledgements:"
        '
        'm_lblRef
        '
        Me.m_lblRef.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblRef.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblRef.Location = New System.Drawing.Point(3, 35)
        Me.m_lblRef.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblRef.Name = "m_lblRef"
        Me.m_lblRef.Size = New System.Drawing.Size(103, 39)
        Me.m_lblRef.TabIndex = 9
        Me.m_lblRef.Text = "Reference:"
        '
        'm_lblContact
        '
        Me.m_lblContact.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblContact.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblContact.Location = New System.Drawing.Point(3, 80)
        Me.m_lblContact.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblContact.Name = "m_lblContact"
        Me.m_lblContact.Size = New System.Drawing.Size(103, 17)
        Me.m_lblContact.TabIndex = 9
        Me.m_lblContact.Text = "Contact:"
        '
        'm_lblGrant
        '
        Me.m_lblGrant.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblGrant.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblGrant.Location = New System.Drawing.Point(3, 103)
        Me.m_lblGrant.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblGrant.Name = "m_lblGrant"
        Me.m_lblGrant.Size = New System.Drawing.Size(103, 13)
        Me.m_lblGrant.TabIndex = 9
        Me.m_lblGrant.Text = "Project grant:"
        '
        'm_lblGrantVal
        '
        Me.m_lblGrantVal.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_lblGrantVal.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lblGrantVal.Location = New System.Drawing.Point(112, 103)
        Me.m_lblGrantVal.Margin = New System.Windows.Forms.Padding(3)
        Me.m_lblGrantVal.Name = "m_lblGrantVal"
        Me.m_lblGrantVal.Size = New System.Drawing.Size(436, 13)
        Me.m_lblGrantVal.TabIndex = 10
        Me.m_lblGrantVal.Text = "CONACyT project 155900 & grant 206762"
        '
        'm_tlpImages
        '
        Me.m_tlpImages.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpImages.ColumnCount = 3
        Me.m_tlpImages.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpImages.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpImages.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.m_tlpImages.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.m_tlpImages.Controls.Add(Me.m_pbIPN, 0, 0)
        Me.m_tlpImages.Controls.Add(Me.m_pbCicimar, 1, 0)
        Me.m_tlpImages.Controls.Add(Me.m_pbConacyt, 2, 0)
        Me.m_tlpImages.Location = New System.Drawing.Point(0, 127)
        Me.m_tlpImages.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tlpImages.Name = "m_tlpImages"
        Me.m_tlpImages.RowCount = 1
        Me.m_tlpImages.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.m_tlpImages.Size = New System.Drawing.Size(557, 114)
        Me.m_tlpImages.TabIndex = 9
        '
        'm_pbIPN
        '
        Me.m_pbIPN.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.IPN_color
        Me.m_pbIPN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.m_pbIPN.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbIPN.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbIPN.Location = New System.Drawing.Point(5, 5)
        Me.m_pbIPN.Margin = New System.Windows.Forms.Padding(5)
        Me.m_pbIPN.Name = "m_pbIPN"
        Me.m_pbIPN.Padding = New System.Windows.Forms.Padding(5)
        Me.m_pbIPN.Size = New System.Drawing.Size(175, 104)
        Me.m_pbIPN.TabIndex = 2
        Me.m_pbIPN.TabStop = False
        '
        'm_pbCicimar
        '
        Me.m_pbCicimar.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.cicimar_color
        Me.m_pbCicimar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.m_pbCicimar.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbCicimar.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbCicimar.Location = New System.Drawing.Point(190, 5)
        Me.m_pbCicimar.Margin = New System.Windows.Forms.Padding(5)
        Me.m_pbCicimar.Name = "m_pbCicimar"
        Me.m_pbCicimar.Padding = New System.Windows.Forms.Padding(5)
        Me.m_pbCicimar.Size = New System.Drawing.Size(175, 104)
        Me.m_pbCicimar.TabIndex = 3
        Me.m_pbCicimar.TabStop = False
        '
        'm_pbConacyt
        '
        Me.m_pbConacyt.BackgroundImage = Global.EwEResiliencePlugin.My.Resources.Resources.CONACYT
        Me.m_pbConacyt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.m_pbConacyt.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_pbConacyt.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_pbConacyt.Location = New System.Drawing.Point(375, 5)
        Me.m_pbConacyt.Margin = New System.Windows.Forms.Padding(5)
        Me.m_pbConacyt.Name = "m_pbConacyt"
        Me.m_pbConacyt.Padding = New System.Windows.Forms.Padding(5)
        Me.m_pbConacyt.Size = New System.Drawing.Size(177, 104)
        Me.m_pbConacyt.TabIndex = 5
        Me.m_pbConacyt.TabStop = False
        '
        'ucAcknowledgements
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpText)
        Me.Controls.Add(Me.m_tlpImages)
        Me.Name = "ucAcknowledgements"
        Me.Size = New System.Drawing.Size(557, 241)
        Me.m_tlpText.ResumeLayout(False)
        Me.m_tlpText.PerformLayout()
        Me.m_tlpImages.ResumeLayout(False)
        CType(Me.m_pbIPN, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbCicimar, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_pbConacyt, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_tlpText As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_lblAckVal As System.Windows.Forms.Label
    Private WithEvents m_lblRefVal As System.Windows.Forms.Label
    Private WithEvents m_llContactVal As System.Windows.Forms.LinkLabel
    Private WithEvents m_lblAck As System.Windows.Forms.Label
    Private WithEvents m_lblRef As System.Windows.Forms.Label
    Private WithEvents m_lblContact As System.Windows.Forms.Label
    Private WithEvents m_lblGrant As System.Windows.Forms.Label
    Private WithEvents m_lblGrantVal As System.Windows.Forms.Label
    Private WithEvents m_pbIPN As System.Windows.Forms.PictureBox
    Private WithEvents m_pbCicimar As System.Windows.Forms.PictureBox
    Private WithEvents m_pbConacyt As System.Windows.Forms.PictureBox
    Private WithEvents m_tlpImages As System.Windows.Forms.TableLayoutPanel

End Class
