Imports ScientificInterfaceShared.Forms
Namespace Ecotracer

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmEcotracerInput
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmEcotracerInput))
            Me.cmbEnvInflowFF = New System.Windows.Forms.ComboBox
            Me.m_lbFFEnv = New System.Windows.Forms.Label
            Me.m_tbCLossEnv = New System.Windows.Forms.TextBox
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_tbCInflowEnv = New System.Windows.Forms.TextBox
            Me.m_lblCDecay = New System.Windows.Forms.Label
            Me.m_lblCInflowEnv = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.m_grid = New ScientificInterface.Ecotracer.EcotracerInputGrid
            Me.m_tbCZeroEnv = New System.Windows.Forms.TextBox
            Me.m_tbCDecayRateEnv = New System.Windows.Forms.TextBox
            Me.m_lbCZeroEnv = New System.Windows.Forms.Label
            Me.m_lbCDecayRateEnv = New System.Windows.Forms.Label
            Me.m_tlp = New System.Windows.Forms.TableLayoutPanel
            Me.m_plAaargh = New System.Windows.Forms.Panel
            Me.m_tlp.SuspendLayout()
            Me.m_plAaargh.SuspendLayout()
            Me.SuspendLayout()
            '
            'cmbEnvInflowFF
            '
            Me.cmbEnvInflowFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbEnvInflowFF.FormattingEnabled = True
            resources.ApplyResources(Me.cmbEnvInflowFF, "cmbEnvInflowFF")
            Me.cmbEnvInflowFF.Name = "cmbEnvInflowFF"
            '
            'm_lbFFEnv
            '
            resources.ApplyResources(Me.m_lbFFEnv, "m_lbFFEnv")
            Me.m_lbFFEnv.Name = "m_lbFFEnv"
            '
            'm_tbCLossEnv
            '
            resources.ApplyResources(Me.m_tbCLossEnv, "m_tbCLossEnv")
            Me.m_tbCLossEnv.Name = "m_tbCLossEnv"
            '
            'lblInitializationHeader
            '
            resources.ApplyResources(Me.lblInitializationHeader, "lblInitializationHeader")
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            '
            'm_tbCInflowEnv
            '
            resources.ApplyResources(Me.m_tbCInflowEnv, "m_tbCInflowEnv")
            Me.m_tbCInflowEnv.Name = "m_tbCInflowEnv"
            '
            'm_lblCDecay
            '
            resources.ApplyResources(Me.m_lblCDecay, "m_lblCDecay")
            Me.m_lblCDecay.Name = "m_lblCDecay"
            '
            'm_lblCInflowEnv
            '
            resources.ApplyResources(Me.m_lblCInflowEnv, "m_lblCInflowEnv")
            Me.m_lblCInflowEnv.Name = "m_lblCInflowEnv"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.Name = "Label1"
            '
            'm_grid
            '
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = False
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.FixedColumnWidths = False
            Me.m_grid.FocusStyle = SourceGrid2.FocusStyle.None
            Me.m_grid.GridToolTipActive = True
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
            Me.m_grid.TrackPropertySelection = True
            Me.m_grid.UIContext = Nothing
            '
            'm_tbCZeroEnv
            '
            resources.ApplyResources(Me.m_tbCZeroEnv, "m_tbCZeroEnv")
            Me.m_tbCZeroEnv.Name = "m_tbCZeroEnv"
            '
            'm_tbCDecayRateEnv
            '
            resources.ApplyResources(Me.m_tbCDecayRateEnv, "m_tbCDecayRateEnv")
            Me.m_tbCDecayRateEnv.Name = "m_tbCDecayRateEnv"
            '
            'm_lbCZeroEnv
            '
            resources.ApplyResources(Me.m_lbCZeroEnv, "m_lbCZeroEnv")
            Me.m_lbCZeroEnv.Name = "m_lbCZeroEnv"
            '
            'm_lbCDecayRateEnv
            '
            resources.ApplyResources(Me.m_lbCDecayRateEnv, "m_lbCDecayRateEnv")
            Me.m_lbCDecayRateEnv.Name = "m_lbCDecayRateEnv"
            '
            'm_tlp
            '
            resources.ApplyResources(Me.m_tlp, "m_tlp")
            Me.m_tlp.Controls.Add(Me.m_lbCZeroEnv, 0, 0)
            Me.m_tlp.Controls.Add(Me.m_lbCDecayRateEnv, 0, 1)
            Me.m_tlp.Controls.Add(Me.m_lblCInflowEnv, 3, 0)
            Me.m_tlp.Controls.Add(Me.m_lblCDecay, 3, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCDecayRateEnv, 1, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCInflowEnv, 4, 0)
            Me.m_tlp.Controls.Add(Me.m_tbCLossEnv, 4, 1)
            Me.m_tlp.Controls.Add(Me.m_tbCZeroEnv, 1, 0)
            Me.m_tlp.Name = "m_tlp"
            '
            'm_plAaargh
            '
            resources.ApplyResources(Me.m_plAaargh, "m_plAaargh")
            Me.m_plAaargh.Controls.Add(Me.m_grid)
            Me.m_plAaargh.Name = "m_plAaargh"
            '
            'frmEcotracerInput
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_plAaargh)
            Me.Controls.Add(Me.m_tlp)
            Me.Controls.Add(Me.cmbEnvInflowFF)
            Me.Controls.Add(Me.m_lbFFEnv)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Name = "frmEcotracerInput"
            Me.m_tlp.ResumeLayout(False)
            Me.m_tlp.PerformLayout()
            Me.m_plAaargh.ResumeLayout(False)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_grid As EcotracerInputGrid
        Private WithEvents m_tbCLossEnv As System.Windows.Forms.TextBox
        Private WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Private WithEvents m_tbCInflowEnv As System.Windows.Forms.TextBox
        Private WithEvents m_lblCDecay As System.Windows.Forms.Label
        Private WithEvents m_lblCInflowEnv As System.Windows.Forms.Label
        Private WithEvents m_tbCZeroEnv As System.Windows.Forms.TextBox
        Private WithEvents m_tbCDecayRateEnv As System.Windows.Forms.TextBox
        Private WithEvents m_lbCZeroEnv As System.Windows.Forms.Label
        Private WithEvents m_lbCDecayRateEnv As System.Windows.Forms.Label
        Private WithEvents cmbEnvInflowFF As System.Windows.Forms.ComboBox
        Private WithEvents m_lbFFEnv As System.Windows.Forms.Label
        Private WithEvents Label1 As System.Windows.Forms.Label
        Private WithEvents m_tlp As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_plAaargh As System.Windows.Forms.Panel
    End Class

End Namespace
