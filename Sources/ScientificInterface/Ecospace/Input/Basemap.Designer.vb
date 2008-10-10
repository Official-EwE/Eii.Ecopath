Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecospace.Basemap

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Basemap
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        '<System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Basemap))
            Me.plBasemap = New ScientificInterface.Ecospace.ucZoomBaseMap
            Me.plLayers = New System.Windows.Forms.Panel
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.ucBrushPicker = New ScientificInterfaceShared.ucBrushPicker
            Me.lblLayers = New System.Windows.Forms.Label
            Me.lblBrush = New System.Windows.Forms.Label
            Me.tsEditBasemapThingies = New System.Windows.Forms.ToolStrip
            Me.tsbEditBasemap = New System.Windows.Forms.ToolStripButton
            Me.tsbEditHabitats = New System.Windows.Forms.ToolStripButton
            Me.tsbEditMPA = New System.Windows.Forms.ToolStripButton
            Me.tsbEditRegion = New System.Windows.Forms.ToolStripButton
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.tsEditBasemapThingies.SuspendLayout()
            Me.SuspendLayout()
            '
            'plBasemap
            '
            Me.plBasemap.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.plBasemap, "plBasemap")
            Me.plBasemap.Name = "plBasemap"
            Me.plBasemap.PositionMode = ScientificInterface.Ecospace.ucZoomBaseMap.ePositionModeTypes.Center
            '
            'plLayers
            '
            resources.ApplyResources(Me.plLayers, "plLayers")
            Me.plLayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.plLayers.Name = "plLayers"
            '
            'SplitContainer1
            '
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.plBasemap)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.TableLayoutPanel1)
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.ucBrushPicker, 0, 4)
            Me.TableLayoutPanel1.Controls.Add(Me.plLayers, 0, 2)
            Me.TableLayoutPanel1.Controls.Add(Me.lblLayers, 0, 1)
            Me.TableLayoutPanel1.Controls.Add(Me.lblBrush, 0, 3)
            Me.TableLayoutPanel1.Controls.Add(Me.tsEditBasemapThingies, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'ucBrushPicker
            '
            resources.ApplyResources(Me.ucBrushPicker, "ucBrushPicker")
            Me.ucBrushPicker.BackColor = System.Drawing.SystemColors.Control
            Me.ucBrushPicker.BrushMaxSize = 5
            Me.ucBrushPicker.BrushMaxValue = 100.0!
            Me.ucBrushPicker.BrushMinSize = 1
            Me.ucBrushPicker.BrushMinValue = 0.0!
            Me.ucBrushPicker.BrushSize = 1
            Me.ucBrushPicker.BrushValue = 0.0!
            Me.ucBrushPicker.ForeColor = System.Drawing.SystemColors.ControlText
            Me.ucBrushPicker.Name = "ucBrushPicker"
            '
            'lblLayers
            '
            resources.ApplyResources(Me.lblLayers, "lblLayers")
            Me.lblLayers.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblLayers.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblLayers.Name = "lblLayers"
            '
            'lblBrush
            '
            resources.ApplyResources(Me.lblBrush, "lblBrush")
            Me.lblBrush.BackColor = System.Drawing.SystemColors.ControlDark
            Me.lblBrush.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblBrush.Name = "lblBrush"
            '
            'tsEditBasemapThingies
            '
            resources.ApplyResources(Me.tsEditBasemapThingies, "tsEditBasemapThingies")
            Me.tsEditBasemapThingies.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbEditBasemap, Me.tsbEditHabitats, Me.tsbEditMPA, Me.tsbEditRegion})
            Me.tsEditBasemapThingies.Name = "tsEditBasemapThingies"
            '
            'tsbEditBasemap
            '
            Me.tsbEditBasemap.Image = Global.ScientificInterface.My.Resources.Resources.Raster1
            resources.ApplyResources(Me.tsbEditBasemap, "tsbEditBasemap")
            Me.tsbEditBasemap.Name = "tsbEditBasemap"
            '
            'tsbEditHabitats
            '
            Me.tsbEditHabitats.Image = Global.ScientificInterface.My.Resources.Resources.Habitat1
            resources.ApplyResources(Me.tsbEditHabitats, "tsbEditHabitats")
            Me.tsbEditHabitats.Name = "tsbEditHabitats"
            '
            'tsbEditMPA
            '
            Me.tsbEditMPA.Image = Global.ScientificInterface.My.Resources.Resources.MPA1
            resources.ApplyResources(Me.tsbEditMPA, "tsbEditMPA")
            Me.tsbEditMPA.Name = "tsbEditMPA"
            '
            'tsbEditRegion
            '
            Me.tsbEditRegion.Image = Global.ScientificInterface.My.Resources.Resources.Regions
            resources.ApplyResources(Me.tsbEditRegion, "tsbEditRegion")
            Me.tsbEditRegion.Name = "tsbEditRegion"
            '
            'Basemap
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "Basemap"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.TableLayoutPanel1.PerformLayout()
            Me.tsEditBasemapThingies.ResumeLayout(False)
            Me.tsEditBasemapThingies.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents plBasemap As ucZoomBaseMap
        Friend WithEvents plLayers As System.Windows.Forms.Panel
        Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents lblLayers As System.Windows.Forms.Label
        Friend WithEvents lblBrush As System.Windows.Forms.Label
        Friend WithEvents ucBrushPicker As ScientificInterfaceShared.ucBrushPicker
        Friend WithEvents tsEditBasemapThingies As System.Windows.Forms.ToolStrip
        Friend WithEvents tsbEditBasemap As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsbEditHabitats As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsbEditRegion As System.Windows.Forms.ToolStripButton
        Friend WithEvents tsbEditMPA As System.Windows.Forms.ToolStripButton

    End Class

End Namespace

