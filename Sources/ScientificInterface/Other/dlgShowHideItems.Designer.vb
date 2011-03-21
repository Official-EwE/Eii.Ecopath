Namespace Ecosim

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class dlgShowHideItems
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlgShowHideItems))
            Me.OK_Button = New System.Windows.Forms.Button
            Me.Cancel_Button = New System.Windows.Forms.Button
            Me.m_clbGroups = New System.Windows.Forms.CheckedListBox
            Me.m_btnAllGroups = New System.Windows.Forms.Button
            Me.m_btnNoneGroups = New System.Windows.Forms.Button
            Me.m_btnDefaultGroups = New System.Windows.Forms.Button
            Me.m_tcDisplayBits = New System.Windows.Forms.TabControl
            Me.m_tpGroups = New System.Windows.Forms.TabPage
            Me.m_hdrFilters = New ScientificInterfaceShared.Controls.cEwEHeaderLabel
            Me.m_btnLiving = New System.Windows.Forms.Button
            Me.m_btnNonLiving = New System.Windows.Forms.Button
            Me.m_btnConsumers = New System.Windows.Forms.Button
            Me.m_btnProducers = New System.Windows.Forms.Button
            Me.m_btnFished = New System.Windows.Forms.Button
            Me.m_tpFleets = New System.Windows.Forms.TabPage
            Me.m_clbFleets = New System.Windows.Forms.CheckedListBox
            Me.m_btnAllFleets = New System.Windows.Forms.Button
            Me.m_btnNoneFleets = New System.Windows.Forms.Button
            Me.m_tcDisplayBits.SuspendLayout()
            Me.m_tpGroups.SuspendLayout()
            Me.m_tpFleets.SuspendLayout()
            Me.SuspendLayout()
            '
            'OK_Button
            '
            resources.ApplyResources(Me.OK_Button, "OK_Button")
            Me.OK_Button.Name = "OK_Button"
            '
            'Cancel_Button
            '
            resources.ApplyResources(Me.Cancel_Button, "Cancel_Button")
            Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Cancel_Button.Name = "Cancel_Button"
            '
            'm_clbGroups
            '
            resources.ApplyResources(Me.m_clbGroups, "m_clbGroups")
            Me.m_clbGroups.CheckOnClick = True
            Me.m_clbGroups.FormattingEnabled = True
            Me.m_clbGroups.Name = "m_clbGroups"
            '
            'm_btnAllGroups
            '
            resources.ApplyResources(Me.m_btnAllGroups, "m_btnAllGroups")
            Me.m_btnAllGroups.Name = "m_btnAllGroups"
            '
            'm_btnNoneGroups
            '
            resources.ApplyResources(Me.m_btnNoneGroups, "m_btnNoneGroups")
            Me.m_btnNoneGroups.Name = "m_btnNoneGroups"
            '
            'm_btnDefaultGroups
            '
            resources.ApplyResources(Me.m_btnDefaultGroups, "m_btnDefaultGroups")
            Me.m_btnDefaultGroups.Name = "m_btnDefaultGroups"
            '
            'm_tcDisplayBits
            '
            resources.ApplyResources(Me.m_tcDisplayBits, "m_tcDisplayBits")
            Me.m_tcDisplayBits.Controls.Add(Me.m_tpGroups)
            Me.m_tcDisplayBits.Controls.Add(Me.m_tpFleets)
            Me.m_tcDisplayBits.Name = "m_tcDisplayBits"
            Me.m_tcDisplayBits.SelectedIndex = 0
            '
            'm_tpGroups
            '
            Me.m_tpGroups.Controls.Add(Me.m_hdrFilters)
            Me.m_tpGroups.Controls.Add(Me.m_clbGroups)
            Me.m_tpGroups.Controls.Add(Me.m_btnLiving)
            Me.m_tpGroups.Controls.Add(Me.m_btnNonLiving)
            Me.m_tpGroups.Controls.Add(Me.m_btnConsumers)
            Me.m_tpGroups.Controls.Add(Me.m_btnProducers)
            Me.m_tpGroups.Controls.Add(Me.m_btnFished)
            Me.m_tpGroups.Controls.Add(Me.m_btnDefaultGroups)
            Me.m_tpGroups.Controls.Add(Me.m_btnAllGroups)
            Me.m_tpGroups.Controls.Add(Me.m_btnNoneGroups)
            resources.ApplyResources(Me.m_tpGroups, "m_tpGroups")
            Me.m_tpGroups.Name = "m_tpGroups"
            Me.m_tpGroups.UseVisualStyleBackColor = True
            '
            'm_hdrFilters
            '
            resources.ApplyResources(Me.m_hdrFilters, "m_hdrFilters")
            Me.m_hdrFilters.Name = "m_hdrFilters"
            '
            'm_btnLiving
            '
            resources.ApplyResources(Me.m_btnLiving, "m_btnLiving")
            Me.m_btnLiving.Name = "m_btnLiving"
            '
            'm_btnNonLiving
            '
            resources.ApplyResources(Me.m_btnNonLiving, "m_btnNonLiving")
            Me.m_btnNonLiving.Name = "m_btnNonLiving"
            '
            'm_btnConsumers
            '
            resources.ApplyResources(Me.m_btnConsumers, "m_btnConsumers")
            Me.m_btnConsumers.Name = "m_btnConsumers"
            '
            'm_btnProducers
            '
            resources.ApplyResources(Me.m_btnProducers, "m_btnProducers")
            Me.m_btnProducers.Name = "m_btnProducers"
            '
            'm_btnFished
            '
            resources.ApplyResources(Me.m_btnFished, "m_btnFished")
            Me.m_btnFished.Name = "m_btnFished"
            '
            'm_tpFleets
            '
            Me.m_tpFleets.Controls.Add(Me.m_clbFleets)
            Me.m_tpFleets.Controls.Add(Me.m_btnAllFleets)
            Me.m_tpFleets.Controls.Add(Me.m_btnNoneFleets)
            resources.ApplyResources(Me.m_tpFleets, "m_tpFleets")
            Me.m_tpFleets.Name = "m_tpFleets"
            Me.m_tpFleets.UseVisualStyleBackColor = True
            '
            'm_clbFleets
            '
            resources.ApplyResources(Me.m_clbFleets, "m_clbFleets")
            Me.m_clbFleets.CheckOnClick = True
            Me.m_clbFleets.FormattingEnabled = True
            Me.m_clbFleets.Name = "m_clbFleets"
            '
            'm_btnAllFleets
            '
            resources.ApplyResources(Me.m_btnAllFleets, "m_btnAllFleets")
            Me.m_btnAllFleets.Name = "m_btnAllFleets"
            '
            'm_btnNoneFleets
            '
            resources.ApplyResources(Me.m_btnNoneFleets, "m_btnNoneFleets")
            Me.m_btnNoneFleets.Name = "m_btnNoneFleets"
            '
            'dlgShowHideItems
            '
            Me.AcceptButton = Me.OK_Button
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.Cancel_Button
            Me.ControlBox = False
            Me.Controls.Add(Me.OK_Button)
            Me.Controls.Add(Me.m_tcDisplayBits)
            Me.Controls.Add(Me.Cancel_Button)
            Me.DoubleBuffered = True
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "dlgShowHideItems"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
            Me.m_tcDisplayBits.ResumeLayout(False)
            Me.m_tpGroups.ResumeLayout(False)
            Me.m_tpFleets.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Private WithEvents m_tcDisplayBits As System.Windows.Forms.TabControl
        Private WithEvents m_btnAllGroups As System.Windows.Forms.Button
        Private WithEvents m_btnNoneGroups As System.Windows.Forms.Button
        Private WithEvents m_clbGroups As System.Windows.Forms.CheckedListBox
        Private WithEvents m_btnDefaultGroups As System.Windows.Forms.Button
        Private WithEvents m_clbFleets As System.Windows.Forms.CheckedListBox
        Private WithEvents m_btnAllFleets As System.Windows.Forms.Button
        Private WithEvents m_btnNoneFleets As System.Windows.Forms.Button
        Private WithEvents m_tpGroups As System.Windows.Forms.TabPage
        Private WithEvents m_tpFleets As System.Windows.Forms.TabPage
        Private WithEvents m_hdrFilters As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_btnNonLiving As System.Windows.Forms.Button
        Private WithEvents m_btnConsumers As System.Windows.Forms.Button
        Private WithEvents m_btnProducers As System.Windows.Forms.Button
        Private WithEvents m_btnFished As System.Windows.Forms.Button
        Private WithEvents m_btnLiving As System.Windows.Forms.Button
        Private WithEvents OK_Button As System.Windows.Forms.Button
        Private WithEvents Cancel_Button As System.Windows.Forms.Button
    End Class

End Namespace

