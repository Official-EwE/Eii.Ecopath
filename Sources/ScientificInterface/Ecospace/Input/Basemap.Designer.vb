Imports ScientificInterfaceShared.Forms

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
            Me.m_plLayers = New ScientificInterfaceShared.Controls.ucSmoothPanel
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.m_tlpControls = New System.Windows.Forms.TableLayoutPanel
            Me.m_hdrLayers = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tsEditBasemapThingies = New ScientificInterfaceShared.Controls.cEwEToolstrip
            Me.tsbEditBasemap = New System.Windows.Forms.ToolStripButton
            Me.tsbEditHabitats = New System.Windows.Forms.ToolStripButton
            Me.tsbEditMPA = New System.Windows.Forms.ToolStripButton
            Me.tsbEditRegion = New System.Windows.Forms.ToolStripButton
            Me.m_plEditor = New System.Windows.Forms.Panel
            Me.m_zoomToolbar = New ScientificInterface.Ecospace.ucMapZoomToolbar
            Me.m_zoomContainer = New ScientificInterface.Ecospace.ucMapZoom
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.m_tlpControls.SuspendLayout()
            Me.m_tsEditBasemapThingies.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_plLayers
            '
            resources.ApplyResources(Me.m_plLayers, "m_plLayers")
            Me.m_plLayers.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_plLayers.Name = "m_plLayers"
            '
            'SplitContainer1
            '
            resources.ApplyResources(Me.SplitContainer1, "SplitContainer1")
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_zoomToolbar)
            Me.SplitContainer1.Panel1.Controls.Add(Me.m_zoomContainer)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_tlpControls)
            '
            'm_tlpControls
            '
            resources.ApplyResources(Me.m_tlpControls, "m_tlpControls")
            Me.m_tlpControls.Controls.Add(Me.m_plLayers, 0, 3)
            Me.m_tlpControls.Controls.Add(Me.m_hdrLayers, 0, 2)
            Me.m_tlpControls.Controls.Add(Me.m_tsEditBasemapThingies, 0, 0)
            Me.m_tlpControls.Controls.Add(Me.m_plEditor, 0, 1)
            Me.m_tlpControls.Name = "m_tlpControls"
            '
            'm_hdrLayers
            '
            resources.ApplyResources(Me.m_hdrLayers, "m_hdrLayers")
            Me.m_hdrLayers.Name = "m_hdrLayers"
            '
            'm_tsEditBasemapThingies
            '
            resources.ApplyResources(Me.m_tsEditBasemapThingies, "m_tsEditBasemapThingies")
            Me.m_tsEditBasemapThingies.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsEditBasemapThingies.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbEditBasemap, Me.tsbEditHabitats, Me.tsbEditMPA, Me.tsbEditRegion})
            Me.m_tsEditBasemapThingies.Name = "m_tsEditBasemapThingies"
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
            'm_plEditor
            '
            resources.ApplyResources(Me.m_plEditor, "m_plEditor")
            Me.m_plEditor.Name = "m_plEditor"
            '
            'm_zoomToolbar
            '
            resources.ApplyResources(Me.m_zoomToolbar, "m_zoomToolbar")
            Me.m_zoomToolbar.MinimumSize = New System.Drawing.Size(100, 25)
            Me.m_zoomToolbar.Name = "m_zoomToolbar"
            Me.m_zoomToolbar.PositionMode = ScientificInterface.Ecospace.ucMapZoom.ePositionModeTypes.Center
            Me.m_zoomToolbar.UIContext = Nothing
            '
            'm_zoomContainer
            '
            resources.ApplyResources(Me.m_zoomContainer, "m_zoomContainer")
            Me.m_zoomContainer.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_zoomContainer.Name = "m_zoomContainer"
            Me.m_zoomContainer.PositionMode = ScientificInterface.Ecospace.ucMapZoom.ePositionModeTypes.Center
            Me.m_zoomContainer.UIContext = Nothing
            Me.m_zoomContainer.ZoomPercentage = 100.0!
            '
            'Basemap
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.SplitContainer1)
            Me.Name = "Basemap"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.TabText = "Define habitats"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel1.PerformLayout()
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.m_tlpControls.ResumeLayout(False)
            Me.m_tsEditBasemapThingies.ResumeLayout(False)
            Me.m_tsEditBasemapThingies.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_plLayers As ScientificInterfaceShared.Controls.ucSmoothPanel
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents m_tlpControls As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_tsEditBasemapThingies As cEwEToolstrip
        Private WithEvents tsbEditBasemap As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbEditHabitats As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbEditRegion As System.Windows.Forms.ToolStripButton
        Private WithEvents tsbEditMPA As System.Windows.Forms.ToolStripButton
        Private WithEvents m_plEditor As System.Windows.Forms.Panel
        Private WithEvents m_zoomContainer As ScientificInterface.Ecospace.ucMapZoom
        Private WithEvents m_hdrLayers As cEwEHeaderLabel
        Private WithEvents m_zoomToolbar As ScientificInterface.Ecospace.ucMapZoomToolbar

    End Class

End Namespace

