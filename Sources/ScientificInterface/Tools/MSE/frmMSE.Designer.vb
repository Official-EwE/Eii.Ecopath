Imports WeifenLuo.WinFormsUI.Docking

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSE
    Inherits frmEwE


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
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSE))
        Me.m_btRun = New System.Windows.Forms.Button
        Me.m_lblNumTrials = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.btStop = New System.Windows.Forms.Button
        Me.zdGraph = New ZedGraph.ZedGraphControl
        Me.btShowHide = New System.Windows.Forms.Button
        Me.ckSave = New System.Windows.Forms.CheckBox
        Me.m_lblStartYear = New System.Windows.Forms.Label
        Me.m_tlpTop = New System.Windows.Forms.TableLayoutPanel
        Me.m_nudStartYear = New System.Windows.Forms.NumericUpDown
        Me.m_nudNumTrials = New System.Windows.Forms.NumericUpDown
        Me.m_tlpTop.SuspendLayout()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).BeginInit()
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
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.Label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.Label3.Name = "Label3"
        '
        'btStop
        '
        resources.ApplyResources(Me.btStop, "btStop")
        Me.btStop.Name = "btStop"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'zdGraph
        '
        resources.ApplyResources(Me.zdGraph, "zdGraph")
        Me.zdGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.zdGraph.Name = "zdGraph"
        Me.zdGraph.ScrollGrace = 0
        Me.zdGraph.ScrollMaxX = 0
        Me.zdGraph.ScrollMaxY = 0
        Me.zdGraph.ScrollMaxY2 = 0
        Me.zdGraph.ScrollMinX = 0
        Me.zdGraph.ScrollMinY = 0
        Me.zdGraph.ScrollMinY2 = 0
        '
        'btShowHide
        '
        resources.ApplyResources(Me.btShowHide, "btShowHide")
        Me.btShowHide.Name = "btShowHide"
        Me.btShowHide.UseVisualStyleBackColor = True
        '
        'ckSave
        '
        resources.ApplyResources(Me.ckSave, "ckSave")
        Me.ckSave.Name = "ckSave"
        Me.ckSave.UseVisualStyleBackColor = True
        '
        'm_lblStartYear
        '
        resources.ApplyResources(Me.m_lblStartYear, "m_lblStartYear")
        Me.m_lblStartYear.Name = "m_lblStartYear"
        '
        'm_tlpTop
        '
        resources.ApplyResources(Me.m_tlpTop, "m_tlpTop")
        Me.m_tlpTop.Controls.Add(Me.m_lblStartYear, 9, 0)
        Me.m_tlpTop.Controls.Add(Me.btStop, 1, 0)
        Me.m_tlpTop.Controls.Add(Me.btShowHide, 2, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btRun, 0, 0)
        Me.m_tlpTop.Controls.Add(Me.m_nudStartYear, 10, 0)
        Me.m_tlpTop.Controls.Add(Me.m_lblNumTrials, 6, 0)
        Me.m_tlpTop.Controls.Add(Me.ckSave, 4, 0)
        Me.m_tlpTop.Controls.Add(Me.m_nudNumTrials, 7, 0)
        Me.m_tlpTop.Name = "m_tlpTop"
        '
        'm_nudStartYear
        '
        resources.ApplyResources(Me.m_nudStartYear, "m_nudStartYear")
        Me.m_nudStartYear.Name = "m_nudStartYear"
        '
        'm_nudNumTrials
        '
        resources.ApplyResources(Me.m_nudNumTrials, "m_nudNumTrials")
        Me.m_nudNumTrials.Name = "m_nudNumTrials"
        '
        'frmMSE
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tlpTop)
        Me.Controls.Add(Me.zdGraph)
        Me.Controls.Add(Me.Label3)
        Me.Name = "frmMSE"
        Me.m_tlpTop.ResumeLayout(False)
        Me.m_tlpTop.PerformLayout()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btRun As System.Windows.Forms.Button
    Private WithEvents m_lblNumTrials As System.Windows.Forms.Label
    Private WithEvents Label3 As System.Windows.Forms.Label
    Private WithEvents btStop As System.Windows.Forms.Button
    Private WithEvents zdGraph As ZedGraph.ZedGraphControl
    Private WithEvents btShowHide As System.Windows.Forms.Button
    Private WithEvents m_lblStartYear As System.Windows.Forms.Label
    Private WithEvents m_tlpTop As System.Windows.Forms.TableLayoutPanel
    Private WithEvents ckSave As System.Windows.Forms.CheckBox
    Private WithEvents m_nudNumTrials As System.Windows.Forms.NumericUpDown
    Private WithEvents m_nudStartYear As System.Windows.Forms.NumericUpDown
End Class
