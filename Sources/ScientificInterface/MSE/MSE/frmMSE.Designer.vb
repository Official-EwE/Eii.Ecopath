' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

Partial Class frmMSE
    Inherits frmEwE

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSE))
        Me.m_btRun = New System.Windows.Forms.Button()
        Me.m_lblNumTrials = New System.Windows.Forms.Label()
        Me.m_btnStop = New System.Windows.Forms.Button()
        Me.m_zgc = New ZedGraph.ZedGraphControl()
        Me.m_ckSave = New System.Windows.Forms.CheckBox()
        Me.m_lblStartYear = New System.Windows.Forms.Label()
        Me.m_nudStartYear = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_nudNumTrials = New ScientificInterfaceShared.Controls.cEwENumericUpDown()
        Me.m_scMain = New System.Windows.Forms.SplitContainer()
        Me.m_tlpRuns = New System.Windows.Forms.TableLayoutPanel()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_scMain.Panel1.SuspendLayout()
        Me.m_scMain.Panel2.SuspendLayout()
        Me.m_scMain.SuspendLayout()
        Me.m_tlpRuns.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_btRun
        '
        resources.ApplyResources(Me.m_btRun, "m_btRun")
        Me.m_btRun.Name = "m_btRun"
        Me.m_btRun.UseVisualStyleBackColor = True
        '
        'm_lblNumTrials
        '
        resources.ApplyResources(Me.m_lblNumTrials, "m_lblNumTrials")
        Me.m_lblNumTrials.Name = "m_lblNumTrials"
        '
        'm_btnStop
        '
        Me.m_btnStop.DialogResult = System.Windows.Forms.DialogResult.Cancel
        resources.ApplyResources(Me.m_btnStop, "m_btnStop")
        Me.m_btnStop.Name = "m_btnStop"
        Me.m_btnStop.UseVisualStyleBackColor = True
        '
        'm_zgc
        '
        resources.ApplyResources(Me.m_zgc, "m_zgc")
        Me.m_zgc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0R
        Me.m_zgc.ScrollMaxX = 0R
        Me.m_zgc.ScrollMaxY = 0R
        Me.m_zgc.ScrollMaxY2 = 0R
        Me.m_zgc.ScrollMinX = 0R
        Me.m_zgc.ScrollMinY = 0R
        Me.m_zgc.ScrollMinY2 = 0R
        '
        'm_ckSave
        '
        resources.ApplyResources(Me.m_ckSave, "m_ckSave")
        Me.m_ckSave.Name = "m_ckSave"
        Me.m_ckSave.UseVisualStyleBackColor = True
        '
        'm_lblStartYear
        '
        resources.ApplyResources(Me.m_lblStartYear, "m_lblStartYear")
        Me.m_lblStartYear.Name = "m_lblStartYear"
        '
        'm_nudStartYear
        '
        resources.ApplyResources(Me.m_nudStartYear, "m_nudStartYear")
        Me.m_nudStartYear.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        Me.m_nudStartYear.Name = "m_nudStartYear"
        '
        'm_nudNumTrials
        '
        resources.ApplyResources(Me.m_nudNumTrials, "m_nudNumTrials")
        Me.m_nudNumTrials.InterceptMouseWheel = ScientificInterfaceShared.Controls.cEwENumericUpDown.eInterceptMouseWheelType.WhenMouseOver
        Me.m_nudNumTrials.Name = "m_nudNumTrials"
        '
        'm_scMain
        '
        resources.ApplyResources(Me.m_scMain, "m_scMain")
        Me.m_scMain.Name = "m_scMain"
        '
        'm_scMain.Panel1
        '
        Me.m_scMain.Panel1.Controls.Add(Me.m_tlpRuns)
        Me.m_scMain.Panel1.Controls.Add(Me.m_lblStartYear)
        Me.m_scMain.Panel1.Controls.Add(Me.m_ckSave)
        Me.m_scMain.Panel1.Controls.Add(Me.m_nudStartYear)
        Me.m_scMain.Panel1.Controls.Add(Me.m_lblNumTrials)
        Me.m_scMain.Panel1.Controls.Add(Me.m_nudNumTrials)
        '
        'm_scMain.Panel2
        '
        Me.m_scMain.Panel2.Controls.Add(Me.m_zgc)
        '
        'm_tlpRuns
        '
        resources.ApplyResources(Me.m_tlpRuns, "m_tlpRuns")
        Me.m_tlpRuns.Controls.Add(Me.m_btRun, 0, 0)
        Me.m_tlpRuns.Controls.Add(Me.m_btnStop, 1, 0)
        Me.m_tlpRuns.Name = "m_tlpRuns"
        '
        'frmMSE
        '
        Me.AcceptButton = Me.m_btRun
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.CancelButton = Me.m_btnStop
        Me.Controls.Add(Me.m_scMain)
        Me.Name = "frmMSE"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.TabText = ""
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.Panel1.ResumeLayout(False)
        Me.m_scMain.Panel1.PerformLayout()
        Me.m_scMain.Panel2.ResumeLayout(False)
        CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_scMain.ResumeLayout(False)
        Me.m_tlpRuns.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btRun As System.Windows.Forms.Button
    Private WithEvents m_lblNumTrials As System.Windows.Forms.Label
    Private WithEvents m_btnStop As System.Windows.Forms.Button
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_lblStartYear As System.Windows.Forms.Label
    Private WithEvents m_ckSave As System.Windows.Forms.CheckBox
    Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
    Private WithEvents m_tlpRuns As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_nudNumTrials As ScientificInterfaceShared.Controls.cEwENumericUpDown
    Private WithEvents m_nudStartYear As ScientificInterfaceShared.Controls.cEwENumericUpDown
End Class
