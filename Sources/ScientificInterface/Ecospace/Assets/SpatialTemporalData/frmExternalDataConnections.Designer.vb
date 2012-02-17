Imports ScientificInterfaceShared.Forms

Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmExternalDataConnections
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmExternalDataConnections))
            Me.m_scMain = New System.Windows.Forms.SplitContainer()
            Me.m_ucDatasets = New ScientificInterface.Ecospace.Controls.ucExternalDataConnections()
            Me.m_tsDatasets = New ScientificInterfaceShared.Controls.cEwEToolstrip()
            Me.m_tslData = New System.Windows.Forms.ToolStripLabel()
            Me.m_tscmAdapters = New System.Windows.Forms.ToolStripComboBox()
            Me.m_tsbnConnections = New System.Windows.Forms.ToolStripButton()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.m_tsDatasets.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_scMain
            '
            Me.m_scMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_scMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMain.Location = New System.Drawing.Point(0, 0)
            Me.m_scMain.Name = "m_scMain"
            Me.m_scMain.Orientation = System.Windows.Forms.Orientation.Horizontal
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_ucDatasets)
            Me.m_scMain.Panel2.Controls.Add(Me.m_tsDatasets)
            Me.m_scMain.Size = New System.Drawing.Size(441, 442)
            Me.m_scMain.SplitterDistance = 244
            Me.m_scMain.TabIndex = 0
            '
            'm_ucDatasets
            '
            Me.m_ucDatasets.BackColor = System.Drawing.SystemColors.Window
            Me.m_ucDatasets.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucDatasets.Location = New System.Drawing.Point(0, 25)
            Me.m_ucDatasets.Name = "m_ucDatasets"
            Me.m_ucDatasets.Size = New System.Drawing.Size(437, 165)
            Me.m_ucDatasets.TabIndex = 1
            Me.m_ucDatasets.UIContext = Nothing
            Me.m_ucDatasets.VarName = EwEUtils.Core.eVarNameFlags.NotSet
            '
            'm_tsDatasets
            '
            Me.m_tsDatasets.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
            Me.m_tsDatasets.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.m_tscmAdapters, Me.m_tslData, Me.m_tsbnConnections})
            Me.m_tsDatasets.Location = New System.Drawing.Point(0, 0)
            Me.m_tsDatasets.Name = "m_tsDatasets"
            Me.m_tsDatasets.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
            Me.m_tsDatasets.Size = New System.Drawing.Size(437, 25)
            Me.m_tsDatasets.TabIndex = 0
            '
            'm_tslData
            '
            Me.m_tslData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tslData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tslData.Name = "m_tslData"
            Me.m_tslData.Size = New System.Drawing.Size(34, 22)
            Me.m_tslData.Text = "Data:"
            '
            'm_tscmAdapters
            '
            Me.m_tscmAdapters.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
            Me.m_tscmAdapters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_tscmAdapters.Name = "m_tscmAdapters"
            Me.m_tscmAdapters.Size = New System.Drawing.Size(121, 25)
            '
            'm_tsbnConnections
            '
            Me.m_tsbnConnections.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
            Me.m_tsbnConnections.Image = CType(resources.GetObject("m_tsbnConnections.Image"), System.Drawing.Image)
            Me.m_tsbnConnections.ImageTransparentColor = System.Drawing.Color.Magenta
            Me.m_tsbnConnections.Name = "m_tsbnConnections"
            Me.m_tsbnConnections.Size = New System.Drawing.Size(87, 22)
            Me.m_tsbnConnections.Text = "&Connections..."
            '
            'frmExternalDataConnections
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(441, 442)
            Me.Controls.Add(Me.m_scMain)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "frmExternalDataConnections"
            Me.Text = "External data connections"
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.Panel2.PerformLayout()
            CType(Me.m_scMain, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_scMain.ResumeLayout(False)
            Me.m_tsDatasets.ResumeLayout(False)
            Me.m_tsDatasets.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_ucDatasets As ScientificInterface.Ecospace.Controls.ucExternalDataConnections
        Private WithEvents m_tsDatasets As ScientificInterfaceShared.Controls.cEwEToolstrip
        Private WithEvents m_tscmAdapters As System.Windows.Forms.ToolStripComboBox
        Private WithEvents m_tslData As System.Windows.Forms.ToolStripLabel
        Private WithEvents m_tsbnConnections As System.Windows.Forms.ToolStripButton
    End Class

End Namespace
