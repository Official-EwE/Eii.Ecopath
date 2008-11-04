
Namespace Ecospace.Basemap.Layers

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlgEditLayer
        Inherits System.Windows.Forms.Form

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
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgEditLayer))
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.Apply_Button = New System.Windows.Forms.Button
            Me.pnBasemap = New System.Windows.Forms.Panel
            Me.lbName = New System.Windows.Forms.Label
            Me.m_tbNameValue = New System.Windows.Forms.TextBox
            Me.m_lblRemarks = New System.Windows.Forms.Label
            Me.m_tbRemarks = New System.Windows.Forms.TextBox
            Me.m_btnDataImport = New System.Windows.Forms.Button
            Me.m_btnDataExport = New System.Windows.Forms.Button
            Me.m_plEditVisualStyle = New System.Windows.Forms.Panel
            Me.m_tcLayerView = New System.Windows.Forms.TabControl
            Me.m_tbAppearance = New System.Windows.Forms.TabPage
            Me.m_tlpDetails = New System.Windows.Forms.TableLayoutPanel
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.m_lblWeight = New System.Windows.Forms.Label
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_nudWeight = New System.Windows.Forms.NumericUpDown
            Me.lblAppearance = New System.Windows.Forms.Label
            Me.lblDescription = New System.Windows.Forms.Label
            Me.m_tpData = New System.Windows.Forms.TabPage
            Me.m_lblStaticData = New System.Windows.Forms.Label
            Me.m_grid = New ScientificInterface.gridLayerData
            Me.TableLayoutPanel1.SuspendLayout()
            Me.m_tcLayerView.SuspendLayout()
            Me.m_tbAppearance.SuspendLayout()
            Me.m_tlpDetails.SuspendLayout()
            CType(Me.m_nudWeight, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tpData.SuspendLayout()
            Me.SuspendLayout()
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(Me.Apply_Button, 2, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'Apply_Button
            '
            resources.ApplyResources(Me.Apply_Button, "Apply_Button")
            Me.Apply_Button.Name = "Apply_Button"
            '
            'pnBasemap
            '
            resources.ApplyResources(Me.pnBasemap, "pnBasemap")
            Me.pnBasemap.BackColor = System.Drawing.SystemColors.HighlightText
            Me.pnBasemap.Name = "pnBasemap"
            '
            'lbName
            '
            resources.ApplyResources(Me.lbName, "lbName")
            Me.lbName.Name = "lbName"
            '
            'm_tbNameValue
            '
            resources.ApplyResources(Me.m_tbNameValue, "m_tbNameValue")
            Me.m_tbNameValue.Name = "m_tbNameValue"
            '
            'm_lblRemarks
            '
            resources.ApplyResources(Me.m_lblRemarks, "m_lblRemarks")
            Me.m_lblRemarks.Name = "m_lblRemarks"
            '
            'm_tbRemarks
            '
            resources.ApplyResources(Me.m_tbRemarks, "m_tbRemarks")
            Me.m_tbRemarks.Name = "m_tbRemarks"
            '
            'm_btnDataImport
            '
            resources.ApplyResources(Me.m_btnDataImport, "m_btnDataImport")
            Me.m_btnDataImport.Name = "m_btnDataImport"
            Me.m_btnDataImport.UseVisualStyleBackColor = True
            '
            'm_btnDataExport
            '
            resources.ApplyResources(Me.m_btnDataExport, "m_btnDataExport")
            Me.m_btnDataExport.Name = "m_btnDataExport"
            Me.m_btnDataExport.UseVisualStyleBackColor = True
            '
            'm_plEditVisualStyle
            '
            resources.ApplyResources(Me.m_plEditVisualStyle, "m_plEditVisualStyle")
            Me.m_plEditVisualStyle.Name = "m_plEditVisualStyle"
            '
            'm_tcLayerView
            '
            resources.ApplyResources(Me.m_tcLayerView, "m_tcLayerView")
            Me.m_tcLayerView.Controls.Add(Me.m_tbAppearance)
            Me.m_tcLayerView.Controls.Add(Me.m_tpData)
            Me.m_tcLayerView.Name = "m_tcLayerView"
            Me.m_tcLayerView.SelectedIndex = 0
            '
            'm_tbAppearance
            '
            Me.m_tbAppearance.Controls.Add(Me.m_tlpDetails)
            Me.m_tbAppearance.Controls.Add(Me.m_plEditVisualStyle)
            Me.m_tbAppearance.Controls.Add(Me.lblAppearance)
            Me.m_tbAppearance.Controls.Add(Me.lblDescription)
            Me.m_tbAppearance.Controls.Add(Me.pnBasemap)
            resources.ApplyResources(Me.m_tbAppearance, "m_tbAppearance")
            Me.m_tbAppearance.Name = "m_tbAppearance"
            Me.m_tbAppearance.UseVisualStyleBackColor = True
            '
            'm_tlpDetails
            '
            resources.ApplyResources(Me.m_tlpDetails, "m_tlpDetails")
            Me.m_tlpDetails.Controls.Add(Me.lbName, 0, 0)
            Me.m_tlpDetails.Controls.Add(Me.m_tbNameValue, 1, 0)
            Me.m_tlpDetails.Controls.Add(Me.m_tbRemarks, 1, 3)
            Me.m_tlpDetails.Controls.Add(Me.m_tbDescription, 1, 2)
            Me.m_tlpDetails.Controls.Add(Me.m_lblRemarks, 0, 3)
            Me.m_tlpDetails.Controls.Add(Me.m_lblWeight, 0, 1)
            Me.m_tlpDetails.Controls.Add(Me.m_lblDescription, 0, 2)
            Me.m_tlpDetails.Controls.Add(Me.m_nudWeight, 1, 1)
            Me.m_tlpDetails.Name = "m_tlpDetails"
            '
            'm_tbDescription
            '
            resources.ApplyResources(Me.m_tbDescription, "m_tbDescription")
            Me.m_tbDescription.Name = "m_tbDescription"
            '
            'm_lblWeight
            '
            resources.ApplyResources(Me.m_lblWeight, "m_lblWeight")
            Me.m_lblWeight.Name = "m_lblWeight"
            '
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_nudWeight
            '
            resources.ApplyResources(Me.m_nudWeight, "m_nudWeight")
            Me.m_nudWeight.Name = "m_nudWeight"
            '
            'lblAppearance
            '
            resources.ApplyResources(Me.lblAppearance, "lblAppearance")
            Me.lblAppearance.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblAppearance.ForeColor = System.Drawing.SystemColors.ButtonHighlight
            Me.lblAppearance.Name = "lblAppearance"
            '
            'lblDescription
            '
            resources.ApplyResources(Me.lblDescription, "lblDescription")
            Me.lblDescription.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblDescription.ForeColor = System.Drawing.SystemColors.ButtonHighlight
            Me.lblDescription.Name = "lblDescription"
            '
            'm_tpData
            '
            Me.m_tpData.Controls.Add(Me.m_lblStaticData)
            Me.m_tpData.Controls.Add(Me.m_grid)
            Me.m_tpData.Controls.Add(Me.m_btnDataExport)
            Me.m_tpData.Controls.Add(Me.m_btnDataImport)
            resources.ApplyResources(Me.m_tpData, "m_tpData")
            Me.m_tpData.Name = "m_tpData"
            Me.m_tpData.UseVisualStyleBackColor = True
            '
            'm_lblStaticData
            '
            resources.ApplyResources(Me.m_lblStaticData, "m_lblStaticData")
            Me.m_lblStaticData.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblStaticData.ForeColor = System.Drawing.SystemColors.ButtonHighlight
            Me.m_lblStaticData.Name = "m_lblStaticData"
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.ContextMenuStyle = SourceGrid2.ContextMenuStyle.None
            Me.m_grid.CustomSort = False
            Me.m_grid.FixedColumnWidths = True
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
            Me.m_grid.Layer = Nothing
            Me.m_grid.Name = "m_grid"
            Me.m_grid.SpecialKeys = CType((((((((((SourceGrid2.GridSpecialKeys.Ctrl_C Or SourceGrid2.GridSpecialKeys.Ctrl_V) _
                        Or SourceGrid2.GridSpecialKeys.Ctrl_X) _
                        Or SourceGrid2.GridSpecialKeys.Delete) _
                        Or SourceGrid2.GridSpecialKeys.Arrows) _
                        Or SourceGrid2.GridSpecialKeys.Tab) _
                        Or SourceGrid2.GridSpecialKeys.PageDownUp) _
                        Or SourceGrid2.GridSpecialKeys.Enter) _
                        Or SourceGrid2.GridSpecialKeys.Escape) _
                        Or SourceGrid2.GridSpecialKeys.Backspace), SourceGrid2.GridSpecialKeys)
            '
            'DataLayerDialog
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.Controls.Add(Me.m_tcLayerView)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "DataLayerDialog"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.m_tcLayerView.ResumeLayout(False)
            Me.m_tbAppearance.ResumeLayout(False)
            Me.m_tlpDetails.ResumeLayout(False)
            Me.m_tlpDetails.PerformLayout()
            CType(Me.m_nudWeight, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tpData.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
        Private WithEvents pnBasemap As System.Windows.Forms.Panel
        Private WithEvents lbName As System.Windows.Forms.Label
        Private WithEvents m_lblRemarks As System.Windows.Forms.Label
        Private WithEvents m_tbRemarks As System.Windows.Forms.TextBox
        Private WithEvents Apply_Button As System.Windows.Forms.Button
        Private WithEvents m_plEditVisualStyle As System.Windows.Forms.Panel
        Private WithEvents m_btnDataImport As System.Windows.Forms.Button
        Private WithEvents m_btnDataExport As System.Windows.Forms.Button
        Private WithEvents m_tbNameValue As System.Windows.Forms.TextBox
        Private WithEvents m_tcLayerView As System.Windows.Forms.TabControl
        Private WithEvents m_tbAppearance As System.Windows.Forms.TabPage
        Private WithEvents m_tpData As System.Windows.Forms.TabPage
        Private WithEvents m_lblStaticData As System.Windows.Forms.Label
        Private WithEvents m_grid As gridLayerData
        Private WithEvents lblAppearance As System.Windows.Forms.Label
        Private WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents m_tlpDetails As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents m_lblDescription As System.Windows.Forms.Label
        Friend WithEvents m_lblWeight As System.Windows.Forms.Label
        Friend WithEvents m_nudWeight As System.Windows.Forms.NumericUpDown

    End Class
End Namespace