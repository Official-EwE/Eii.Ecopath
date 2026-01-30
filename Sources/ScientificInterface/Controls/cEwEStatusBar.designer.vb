' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports SharedResources = ScientificInterfaceShared.My.Resources

Partial Class cEwEStatusBar
    Inherits StatusStrip


    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(cEwEStatusBar))
        Me.m_tslStop = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsbProgress = New System.Windows.Forms.ToolStripProgressBar()
        Me.m_tsStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsEcopathModel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsEcosimScenario = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsEcospaceScenario = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsEcotracerScenario = New System.Windows.Forms.ToolStripStatusLabel()
        Me.m_tsIP = New System.Windows.Forms.ToolStripStatusLabel()
        Me.SuspendLayout()
        '
        'm_tslStop
        '
        Me.m_tslStop.Image = CType(resources.GetObject("m_tslStop.Image"), System.Drawing.Image)
        Me.m_tslStop.Name = "m_tslStop"
        Me.m_tslStop.Size = New System.Drawing.Size(47, 17)
        Me.m_tslStop.Text = "Stop"
        Me.m_tslStop.ToolTipText = "Interrupt the current process"
        '
        'm_tsbProgress
        '
        Me.m_tsbProgress.Name = "m_tsbProgress"
        Me.m_tsbProgress.Size = New System.Drawing.Size(100, 16)
        '
        'm_tsStatus
        '
        Me.m_tsStatus.Name = "m_tsStatus"
        Me.m_tsStatus.Size = New System.Drawing.Size(4, 17)
        Me.m_tsStatus.Spring = True
        Me.m_tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tsEcopathModel
        '
        Me.m_tsEcopathModel.AutoToolTip = True
        Me.m_tsEcopathModel.Image = CType(resources.GetObject("m_tsEcopathModel.Image"), System.Drawing.Image)
        Me.m_tsEcopathModel.Name = "m_tsEcopathModel"
        Me.m_tsEcopathModel.Size = New System.Drawing.Size(16, 17)
        '
        'm_tsEcosimScenario
        '
        Me.m_tsEcosimScenario.AutoToolTip = True
        Me.m_tsEcosimScenario.Image = CType(resources.GetObject("m_tsEcosimScenario.Image"), System.Drawing.Image)
        Me.m_tsEcosimScenario.Name = "m_tsEcosimScenario"
        Me.m_tsEcosimScenario.Size = New System.Drawing.Size(16, 17)
        '
        'm_tsEcospaceScenario
        '
        Me.m_tsEcospaceScenario.AutoToolTip = True
        Me.m_tsEcospaceScenario.Image = CType(resources.GetObject("m_tsEcospaceScenario.Image"), System.Drawing.Image)
        Me.m_tsEcospaceScenario.Name = "m_tsEcospaceScenario"
        Me.m_tsEcospaceScenario.Size = New System.Drawing.Size(16, 16)
        '
        'm_tsEcotracerScenario
        '
        Me.m_tsEcotracerScenario.AutoToolTip = True
        Me.m_tsEcotracerScenario.Image = CType(resources.GetObject("m_tsEcotracerScenario.Image"), System.Drawing.Image)
        Me.m_tsEcotracerScenario.Name = "m_tsEcotracerScenario"
        Me.m_tsEcotracerScenario.Size = New System.Drawing.Size(16, 16)
        '
        'm_tsIP
        '
        Me.m_tsIP.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left
        Me.m_tsIP.BorderStyle = System.Windows.Forms.Border3DStyle.Etched
        'Me.m_tsIP.Image = Global.ScientificInterface.My.Resources.Resources.Computer
        Me.m_tsIP.Name = "m_tsIP"
        Me.m_tsIP.Size = New System.Drawing.Size(20, 17)
        '
        'cEwEStatusBar
        '
        Me.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tslStop, Me.m_tsbProgress, Me.m_tsStatus, Me.m_tsEcopathModel, Me.m_tsEcosimScenario, Me.m_tsEcospaceScenario, Me.m_tsEcotracerScenario, Me.m_tsIP})
        Me.ShowItemToolTips = True
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents m_tslStop As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsbProgress As System.Windows.Forms.ToolStripProgressBar
    Private WithEvents m_tsStatus As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcopathModel As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcosimScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcospaceScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsEcotracerScenario As System.Windows.Forms.ToolStripStatusLabel
    Private WithEvents m_tsIP As System.Windows.Forms.ToolStripStatusLabel

End Class