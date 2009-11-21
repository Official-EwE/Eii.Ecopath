Imports WeifenLuo.WinFormsUI.Docking
Imports ScientificInterfaceShared

Namespace Ecopath.Output

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class PSDContributionPlot
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
            Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
            Me.zgcZedGraphCntl = New ZedGraph.ZedGraphControl
            Me.m_lbGroups = New ScientificInterfaceShared.Controls.cGroupListBox
            Me.m_lblGroups = New System.Windows.Forms.Label
            Me.SplitContainer1.Panel1.SuspendLayout()
            Me.SplitContainer1.Panel2.SuspendLayout()
            Me.SplitContainer1.SuspendLayout()
            Me.SuspendLayout()
            '
            'SplitContainer1
            '
            Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
            Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
            Me.SplitContainer1.Name = "SplitContainer1"
            '
            'SplitContainer1.Panel1
            '
            Me.SplitContainer1.Panel1.Controls.Add(Me.zgcZedGraphCntl)
            '
            'SplitContainer1.Panel2
            '
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_lblGroups)
            Me.SplitContainer1.Panel2.Controls.Add(Me.m_lbGroups)
            Me.SplitContainer1.Size = New System.Drawing.Size(607, 382)
            Me.SplitContainer1.SplitterDistance = 465
            Me.SplitContainer1.TabIndex = 0
            '
            'zgcZedGraphCntl
            '
            Me.zgcZedGraphCntl.Dock = System.Windows.Forms.DockStyle.Fill
            Me.zgcZedGraphCntl.Location = New System.Drawing.Point(0, 0)
            Me.zgcZedGraphCntl.Margin = New System.Windows.Forms.Padding(0)
            Me.zgcZedGraphCntl.Name = "zgcZedGraphCntl"
            Me.zgcZedGraphCntl.ScrollGrace = 0
            Me.zgcZedGraphCntl.ScrollMaxX = 0
            Me.zgcZedGraphCntl.ScrollMaxY = 0
            Me.zgcZedGraphCntl.ScrollMaxY2 = 0
            Me.zgcZedGraphCntl.ScrollMinX = 0
            Me.zgcZedGraphCntl.ScrollMinY = 0
            Me.zgcZedGraphCntl.ScrollMinY2 = 0
            Me.zgcZedGraphCntl.Size = New System.Drawing.Size(465, 382)
            Me.zgcZedGraphCntl.TabIndex = 0
            '
            'm_lbGroups
            '
            Me.m_lbGroups.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                        Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lbGroups.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
            Me.m_lbGroups.FormattingEnabled = True
            Me.m_lbGroups.GroupDisplayStyle = ScientificInterfaceShared.Controls.cGroupListBox.eGroupDisplayStyleTypes.DisplayVisibleOnly
            Me.m_lbGroups.GroupListTracking = ScientificInterfaceShared.Controls.cGroupListBox.eGroupTrackingType.AllGroups
            Me.m_lbGroups.IntegralHeight = False
            Me.m_lbGroups.Location = New System.Drawing.Point(0, 18)
            Me.m_lbGroups.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lbGroups.Name = "m_lbGroups"
            Me.m_lbGroups.ShowAllGroupsItem = False
            Me.m_lbGroups.Size = New System.Drawing.Size(138, 364)
            Me.m_lbGroups.SortThreshold = -9999.0!
            Me.m_lbGroups.SortType = ScientificInterfaceShared.Controls.cGroupListBox.eSortType.GroupIndexDesc
            Me.m_lbGroups.TabIndex = 0
            '
            'm_lblGroups
            '
            Me.m_lblGroups.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                        Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_lblGroups.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblGroups.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.m_lblGroups.ForeColor = System.Drawing.SystemColors.ButtonHighlight
            Me.m_lblGroups.Location = New System.Drawing.Point(0, 0)
            Me.m_lblGroups.Margin = New System.Windows.Forms.Padding(0)
            Me.m_lblGroups.Name = "m_lblGroups"
            Me.m_lblGroups.Size = New System.Drawing.Size(138, 18)
            Me.m_lblGroups.TabIndex = 1
            Me.m_lblGroups.Text = "Groups"
            Me.m_lblGroups.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'PSDContributionPlot
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.ClientSize = New System.Drawing.Size(607, 382)
            Me.Controls.Add(Me.SplitContainer1)
            Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
            Me.Name = "PSDContributionPlot"
            Me.Text = "PSDContributionPlot"
            Me.SplitContainer1.Panel1.ResumeLayout(False)
            Me.SplitContainer1.Panel2.ResumeLayout(False)
            Me.SplitContainer1.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
        Private WithEvents zgcZedGraphCntl As ZedGraph.ZedGraphControl
        Private WithEvents m_lbGroups As ScientificInterfaceShared.Controls.cGroupListBox
        Private WithEvents m_lblGroups As System.Windows.Forms.Label
    End Class

End Namespace
