Namespace Ecospace.Advection

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class frmAdvection
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
            Me.m_tlpMaps = New System.Windows.Forms.TableLayoutPanel
            Me.m_ucTransportRate = New ScientificInterface.Ecospace.Advection.ucTransportRate
            Me.m_ucTansportVelocity = New System.Windows.Forms.Panel
            Me.m_ucMixedLayerDepths = New System.Windows.Forms.Panel
            Me.m_ucUpwellingVelocities = New System.Windows.Forms.Panel
            Me.m_scMain = New System.Windows.Forms.SplitContainer
            Me.m_hdrOptions = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_tlpMaps.SuspendLayout()
            Me.m_scMain.Panel1.SuspendLayout()
            Me.m_scMain.Panel2.SuspendLayout()
            Me.m_scMain.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_tlpMaps
            '
            Me.m_tlpMaps.ColumnCount = 2
            Me.m_tlpMaps.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.Controls.Add(Me.m_ucTransportRate, 1, 0)
            Me.m_tlpMaps.Controls.Add(Me.m_ucTansportVelocity, 0, 0)
            Me.m_tlpMaps.Controls.Add(Me.m_ucMixedLayerDepths, 0, 1)
            Me.m_tlpMaps.Controls.Add(Me.m_ucUpwellingVelocities, 1, 1)
            Me.m_tlpMaps.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_tlpMaps.Location = New System.Drawing.Point(0, 0)
            Me.m_tlpMaps.Name = "m_tlpMaps"
            Me.m_tlpMaps.RowCount = 2
            Me.m_tlpMaps.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
            Me.m_tlpMaps.Size = New System.Drawing.Size(626, 568)
            Me.m_tlpMaps.TabIndex = 0
            '
            'm_ucTransportRate
            '
            Me.m_ucTransportRate.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucTransportRate.Location = New System.Drawing.Point(316, 3)
            Me.m_ucTransportRate.Name = "m_ucTransportRate"
            Me.m_ucTransportRate.Size = New System.Drawing.Size(307, 278)
            Me.m_ucTransportRate.TabIndex = 1
            Me.m_ucTransportRate.UIContext = Nothing
            '
            'm_ucTansportVelocity
            '
            Me.m_ucTansportVelocity.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucTansportVelocity.Location = New System.Drawing.Point(3, 3)
            Me.m_ucTansportVelocity.Name = "m_ucTansportVelocity"
            Me.m_ucTansportVelocity.Size = New System.Drawing.Size(307, 278)
            Me.m_ucTansportVelocity.TabIndex = 0
            '
            'm_ucMixedLayerDepths
            '
            Me.m_ucMixedLayerDepths.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucMixedLayerDepths.Location = New System.Drawing.Point(3, 287)
            Me.m_ucMixedLayerDepths.Name = "m_ucMixedLayerDepths"
            Me.m_ucMixedLayerDepths.Size = New System.Drawing.Size(307, 278)
            Me.m_ucMixedLayerDepths.TabIndex = 2
            '
            'm_ucUpwellingVelocities
            '
            Me.m_ucUpwellingVelocities.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_ucUpwellingVelocities.Location = New System.Drawing.Point(316, 287)
            Me.m_ucUpwellingVelocities.Name = "m_ucUpwellingVelocities"
            Me.m_ucUpwellingVelocities.Size = New System.Drawing.Size(307, 278)
            Me.m_ucUpwellingVelocities.TabIndex = 3
            '
            'm_scMain
            '
            Me.m_scMain.Dock = System.Windows.Forms.DockStyle.Fill
            Me.m_scMain.Location = New System.Drawing.Point(3, 3)
            Me.m_scMain.Name = "m_scMain"
            '
            'm_scMain.Panel1
            '
            Me.m_scMain.Panel1.Controls.Add(Me.m_hdrOptions)
            '
            'm_scMain.Panel2
            '
            Me.m_scMain.Panel2.Controls.Add(Me.m_tlpMaps)
            Me.m_scMain.Size = New System.Drawing.Size(798, 568)
            Me.m_scMain.SplitterDistance = 168
            Me.m_scMain.TabIndex = 0
            '
            'm_hdrOptions
            '
            Me.m_hdrOptions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrOptions.Location = New System.Drawing.Point(3, 3)
            Me.m_hdrOptions.Name = "m_hdrOptions"
            Me.m_hdrOptions.Size = New System.Drawing.Size(162, 18)
            Me.m_hdrOptions.TabIndex = 0
            Me.m_hdrOptions.Text = "Options"
            Me.m_hdrOptions.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'frmAdvection
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(804, 574)
            Me.Controls.Add(Me.m_scMain)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "frmAdvection"
            Me.Padding = New System.Windows.Forms.Padding(3)
            Me.ShowInTaskbar = False
            Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
            Me.Text = "frmAdvection"
            Me.m_tlpMaps.ResumeLayout(False)
            Me.m_scMain.Panel1.ResumeLayout(False)
            Me.m_scMain.Panel2.ResumeLayout(False)
            Me.m_scMain.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tlpMaps As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_ucTansportVelocity As System.Windows.Forms.Panel
        Private WithEvents m_ucTransportRate As ucTransportRate
        Private WithEvents m_ucMixedLayerDepths As System.Windows.Forms.Panel
        Private WithEvents m_ucUpwellingVelocities As System.Windows.Forms.Panel
        Private WithEvents m_scMain As System.Windows.Forms.SplitContainer
        Private WithEvents m_hdrOptions As ScientificInterfaceShared.Controls.cEwEHeaderLabel

    End Class

End Namespace
