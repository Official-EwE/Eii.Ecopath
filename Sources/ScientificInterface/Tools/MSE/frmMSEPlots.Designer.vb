Imports ScientificInterfaceShared.Forms
Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSEPlots
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSEPlots))
        Me.ZedGraph = New ZedGraph.ZedGraphControl
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.rbBioEst = New System.Windows.Forms.RadioButton
        Me.btShowHide = New System.Windows.Forms.Button
        Me.m_hdrPlots = New cEwEHeaderLabel
        Me.rbEffort = New System.Windows.Forms.RadioButton
        Me.rbFleetValue = New System.Windows.Forms.RadioButton
        Me.rbGroupCatch = New System.Windows.Forms.RadioButton
        Me.rbGroupBiomass = New System.Windows.Forms.RadioButton
        Me.Panel2 = New System.Windows.Forms.Panel
        Me.m_hdrType = New cEwEHeaderLabel
        Me.rbValues = New System.Windows.Forms.RadioButton
        Me.rbHisto = New System.Windows.Forms.RadioButton
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ZedGraph
        '
        resources.ApplyResources(Me.ZedGraph, "ZedGraph")
        Me.ZedGraph.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.ZedGraph.Name = "ZedGraph"
        Me.ZedGraph.ScrollGrace = 0
        Me.ZedGraph.ScrollMaxX = 0
        Me.ZedGraph.ScrollMaxY = 0
        Me.ZedGraph.ScrollMaxY2 = 0
        Me.ZedGraph.ScrollMinX = 0
        Me.ZedGraph.ScrollMinY = 0
        Me.ZedGraph.ScrollMinY2 = 0
        '
        'Panel1
        '
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Controls.Add(Me.rbBioEst)
        Me.Panel1.Controls.Add(Me.btShowHide)
        Me.Panel1.Controls.Add(Me.m_hdrPlots)
        Me.Panel1.Controls.Add(Me.rbEffort)
        Me.Panel1.Controls.Add(Me.rbFleetValue)
        Me.Panel1.Controls.Add(Me.rbGroupCatch)
        Me.Panel1.Controls.Add(Me.rbGroupBiomass)
        Me.Panel1.Name = "Panel1"
        '
        'rbBioEst
        '
        resources.ApplyResources(Me.rbBioEst, "rbBioEst")
        Me.rbBioEst.Name = "rbBioEst"
        Me.rbBioEst.TabStop = True
        Me.rbBioEst.UseVisualStyleBackColor = True
        '
        'btShowHide
        '
        resources.ApplyResources(Me.btShowHide, "btShowHide")
        Me.btShowHide.Name = "btShowHide"
        Me.btShowHide.UseVisualStyleBackColor = True
        '
        'm_hdrPlots
        '
        resources.ApplyResources(Me.m_hdrPlots, "m_hdrPlots")
        Me.m_hdrPlots.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.m_hdrPlots.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrPlots.Name = "m_hdrPlots"
        '
        'rbEffort
        '
        resources.ApplyResources(Me.rbEffort, "rbEffort")
        Me.rbEffort.Name = "rbEffort"
        Me.rbEffort.UseVisualStyleBackColor = True
        '
        'rbFleetValue
        '
        resources.ApplyResources(Me.rbFleetValue, "rbFleetValue")
        Me.rbFleetValue.Name = "rbFleetValue"
        Me.rbFleetValue.UseVisualStyleBackColor = True
        '
        'rbGroupCatch
        '
        resources.ApplyResources(Me.rbGroupCatch, "rbGroupCatch")
        Me.rbGroupCatch.Name = "rbGroupCatch"
        Me.rbGroupCatch.UseVisualStyleBackColor = True
        '
        'rbGroupBiomass
        '
        resources.ApplyResources(Me.rbGroupBiomass, "rbGroupBiomass")
        Me.rbGroupBiomass.Checked = True
        Me.rbGroupBiomass.Name = "rbGroupBiomass"
        Me.rbGroupBiomass.TabStop = True
        Me.rbGroupBiomass.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.m_hdrType)
        Me.Panel2.Controls.Add(Me.rbValues)
        Me.Panel2.Controls.Add(Me.rbHisto)
        resources.ApplyResources(Me.Panel2, "Panel2")
        Me.Panel2.Name = "Panel2"
        '
        'm_hdrType
        '
        Me.m_hdrType.BackColor = System.Drawing.SystemColors.ButtonShadow
        resources.ApplyResources(Me.m_hdrType, "m_hdrType")
        Me.m_hdrType.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.m_hdrType.Name = "m_hdrType"
        '
        'rbValues
        '
        resources.ApplyResources(Me.rbValues, "rbValues")
        Me.rbValues.Name = "rbValues"
        Me.rbValues.Tag = ""
        Me.rbValues.UseVisualStyleBackColor = True
        '
        'rbHisto
        '
        resources.ApplyResources(Me.rbHisto, "rbHisto")
        Me.rbHisto.Checked = True
        Me.rbHisto.Name = "rbHisto"
        Me.rbHisto.TabStop = True
        Me.rbHisto.Tag = ""
        Me.rbHisto.UseVisualStyleBackColor = True
        '
        'frmMSEPlots
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.ZedGraph)
        Me.Name = "frmMSEPlots"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents ZedGraph As ZedGraph.ZedGraphControl
    Private WithEvents Panel1 As System.Windows.Forms.Panel
    Private WithEvents rbEffort As System.Windows.Forms.RadioButton
    Private WithEvents rbFleetValue As System.Windows.Forms.RadioButton
    Private WithEvents rbGroupCatch As System.Windows.Forms.RadioButton
    Private WithEvents rbGroupBiomass As System.Windows.Forms.RadioButton
    Private WithEvents Panel2 As System.Windows.Forms.Panel
    Private WithEvents rbValues As System.Windows.Forms.RadioButton
    Private WithEvents rbHisto As System.Windows.Forms.RadioButton
    Private WithEvents m_hdrType As cEwEHeaderLabel
    Private WithEvents btShowHide As System.Windows.Forms.Button
    Private WithEvents rbBioEst As System.Windows.Forms.RadioButton
    Private WithEvents m_hdrPlots As cEwEHeaderLabel
End Class
