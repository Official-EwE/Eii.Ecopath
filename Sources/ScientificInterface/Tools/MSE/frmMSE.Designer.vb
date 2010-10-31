Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Forms

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
        Me.m_hdrOutputs = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Me.m_btnStop = New System.Windows.Forms.Button
        Me.m_zgc = New ZedGraph.ZedGraphControl
        Me.m_btnShowHide = New System.Windows.Forms.Button
        Me.m_ckSave = New System.Windows.Forms.CheckBox
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
        'm_hdrOutputs
        '
        resources.ApplyResources(Me.m_hdrOutputs, "m_hdrOutputs")
        Me.m_hdrOutputs.Name = "m_hdrOutputs"
        '
        'm_btnStop
        '
        resources.ApplyResources(Me.m_btnStop, "m_btnStop")
        Me.m_btnStop.Name = "m_btnStop"
        Me.m_btnStop.UseVisualStyleBackColor = True
        '
        'm_zgc
        '
        resources.ApplyResources(Me.m_zgc, "m_zgc")
        Me.m_zgc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.m_zgc.Name = "m_zgc"
        Me.m_zgc.ScrollGrace = 0
        Me.m_zgc.ScrollMaxX = 0
        Me.m_zgc.ScrollMaxY = 0
        Me.m_zgc.ScrollMaxY2 = 0
        Me.m_zgc.ScrollMinX = 0
        Me.m_zgc.ScrollMinY = 0
        Me.m_zgc.ScrollMinY2 = 0
        '
        'm_btnShowHide
        '
        resources.ApplyResources(Me.m_btnShowHide, "m_btnShowHide")
        Me.m_btnShowHide.Name = "m_btnShowHide"
        Me.m_btnShowHide.UseVisualStyleBackColor = True
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
        'm_tlpTop
        '
        resources.ApplyResources(Me.m_tlpTop, "m_tlpTop")
        Me.m_tlpTop.Controls.Add(Me.m_lblStartYear, 9, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btnStop, 1, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btnShowHide, 2, 0)
        Me.m_tlpTop.Controls.Add(Me.m_btRun, 0, 0)
        Me.m_tlpTop.Controls.Add(Me.m_nudStartYear, 10, 0)
        Me.m_tlpTop.Controls.Add(Me.m_lblNumTrials, 6, 0)
        Me.m_tlpTop.Controls.Add(Me.m_ckSave, 4, 0)
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
        Me.Controls.Add(Me.m_zgc)
        Me.Controls.Add(Me.m_hdrOutputs)
        Me.Name = "frmMSE"
        Me.m_tlpTop.ResumeLayout(False)
        Me.m_tlpTop.PerformLayout()
        CType(Me.m_nudStartYear, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.m_nudNumTrials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents m_btRun As System.Windows.Forms.Button
    Private WithEvents m_lblNumTrials As System.Windows.Forms.Label
    Private WithEvents m_hdrOutputs As cEwEHeaderLabel
    Private WithEvents m_btnStop As System.Windows.Forms.Button
    Private WithEvents m_zgc As ZedGraph.ZedGraphControl
    Private WithEvents m_btnShowHide As System.Windows.Forms.Button
    Private WithEvents m_lblStartYear As System.Windows.Forms.Label
    Private WithEvents m_tlpTop As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_ckSave As System.Windows.Forms.CheckBox
    Private WithEvents m_nudNumTrials As System.Windows.Forms.NumericUpDown
    Private WithEvents m_nudStartYear As System.Windows.Forms.NumericUpDown
End Class
