Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditMultiStanza
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
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditMultiStanza))
            Me.m_btnCalculate = New System.Windows.Forms.Button
            Me.m_btnOK = New System.Windows.Forms.Button
            Me.m_btnCancel = New System.Windows.Forms.Button
            Me.m_grid = New ScientificInterface.EditMultiStanzaEwEGrid
            Me.m_lblStanzaGroups = New System.Windows.Forms.Label
            Me.m_lblK = New System.Windows.Forms.Label
            Me.m_lblRecPwr = New System.Windows.Forms.Label
            Me.m_lblBAB = New System.Windows.Forms.Label
            Me.m_lblWmatWinf = New System.Windows.Forms.Label
            Me.m_lblFF = New System.Windows.Forms.Label
            Me.m_txtK = New System.Windows.Forms.TextBox
            Me.m_txtRecPwr = New System.Windows.Forms.TextBox
            Me.m_txtBAB = New System.Windows.Forms.TextBox
            Me.m_txtWmatWinf = New System.Windows.Forms.TextBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.m_cbFFecun = New System.Windows.Forms.CheckBox
            Me.m_cmbStanzaGroups = New System.Windows.Forms.ComboBox
            Me.m_cmbFF = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'm_btnCalculate
            '
            resources.ApplyResources(Me.m_btnCalculate, "m_btnCalculate")
            Me.m_btnCalculate.Name = "m_btnCalculate"
            Me.m_btnCalculate.UseVisualStyleBackColor = True
            '
            'm_btnOK
            '
            resources.ApplyResources(Me.m_btnOK, "m_btnOK")
            Me.m_btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.m_btnOK.Name = "m_btnOK"
            Me.m_btnOK.UseVisualStyleBackColor = True
            '
            'm_btnCancel
            '
            resources.ApplyResources(Me.m_btnCancel, "m_btnCancel")
            Me.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.m_btnCancel.Name = "m_btnCancel"
            Me.m_btnCancel.UseVisualStyleBackColor = True
            '
            'm_grid
            '
            resources.ApplyResources(Me.m_grid, "m_grid")
            Me.m_grid.AutoSizeMinHeight = 10
            Me.m_grid.AutoSizeMinWidth = 10
            Me.m_grid.AutoStretchColumnsToFitWidth = True
            Me.m_grid.AutoStretchRowsToFitHeight = False
            Me.m_grid.BackColor = System.Drawing.Color.White
            Me.m_grid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.m_grid.ContextMenuStyle = CType((((SourceGrid2.ContextMenuStyle.ColumnResize Or SourceGrid2.ContextMenuStyle.AutoSize) _
                        Or SourceGrid2.ContextMenuStyle.CopyPasteSelection) _
                        Or SourceGrid2.ContextMenuStyle.CellContextMenu), SourceGrid2.ContextMenuStyle)
            Me.m_grid.CustomSort = False
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
            Me.m_grid.StanzaGroup = Nothing
            Me.m_grid.TabStop = True
            '
            'm_lblStanzaGroups
            '
            resources.ApplyResources(Me.m_lblStanzaGroups, "m_lblStanzaGroups")
            Me.m_lblStanzaGroups.Name = "m_lblStanzaGroups"
            '
            'm_lblK
            '
            resources.ApplyResources(Me.m_lblK, "m_lblK")
            Me.m_lblK.Name = "m_lblK"
            '
            'm_lblRecPwr
            '
            resources.ApplyResources(Me.m_lblRecPwr, "m_lblRecPwr")
            Me.m_lblRecPwr.Name = "m_lblRecPwr"
            '
            'm_lblBAB
            '
            resources.ApplyResources(Me.m_lblBAB, "m_lblBAB")
            Me.m_lblBAB.Name = "m_lblBAB"
            '
            'm_lblWmatWinf
            '
            resources.ApplyResources(Me.m_lblWmatWinf, "m_lblWmatWinf")
            Me.m_lblWmatWinf.Name = "m_lblWmatWinf"
            '
            'm_lblFF
            '
            resources.ApplyResources(Me.m_lblFF, "m_lblFF")
            Me.m_lblFF.Name = "m_lblFF"
            '
            'm_txtK
            '
            resources.ApplyResources(Me.m_txtK, "m_txtK")
            Me.m_txtK.Name = "m_txtK"
            '
            'm_txtRecPwr
            '
            resources.ApplyResources(Me.m_txtRecPwr, "m_txtRecPwr")
            Me.m_txtRecPwr.Name = "m_txtRecPwr"
            '
            'm_txtBAB
            '
            resources.ApplyResources(Me.m_txtBAB, "m_txtBAB")
            Me.m_txtBAB.Name = "m_txtBAB"
            '
            'm_txtWmatWinf
            '
            resources.ApplyResources(Me.m_txtWmatWinf, "m_txtWmatWinf")
            Me.m_txtWmatWinf.Name = "m_txtWmatWinf"
            '
            'm_zgc
            '
            resources.ApplyResources(Me.m_zgc, "m_zgc")
            Me.m_zgc.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
            Me.m_zgc.Name = "m_zgc"
            Me.m_zgc.ScrollGrace = 0
            Me.m_zgc.ScrollMaxX = 0
            Me.m_zgc.ScrollMaxY = 0
            Me.m_zgc.ScrollMaxY2 = 0
            Me.m_zgc.ScrollMinX = 0
            Me.m_zgc.ScrollMinY = 0
            Me.m_zgc.ScrollMinY2 = 0
            Me.m_zgc.TabStop = False
            '
            'm_cbFFecun
            '
            resources.ApplyResources(Me.m_cbFFecun, "m_cbFFecun")
            Me.m_cbFFecun.Name = "m_cbFFecun"
            Me.m_cbFFecun.UseVisualStyleBackColor = True
            '
            'm_cmbStanzaGroups
            '
            resources.ApplyResources(Me.m_cmbStanzaGroups, "m_cmbStanzaGroups")
            Me.m_cmbStanzaGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbStanzaGroups.FormattingEnabled = True
            Me.m_cmbStanzaGroups.Name = "m_cmbStanzaGroups"
            '
            'm_cmbFF
            '
            resources.ApplyResources(Me.m_cmbFF, "m_cmbFF")
            Me.m_cmbFF.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbFF.FormattingEnabled = True
            Me.m_cmbFF.Name = "m_cmbFF"
            '
            'EditMultiStanza
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.m_btnCancel
            Me.Controls.Add(Me.m_cmbFF)
            Me.Controls.Add(Me.m_cmbStanzaGroups)
            Me.Controls.Add(Me.m_cbFFecun)
            Me.Controls.Add(Me.m_zgc)
            Me.Controls.Add(Me.m_txtWmatWinf)
            Me.Controls.Add(Me.m_txtBAB)
            Me.Controls.Add(Me.m_txtRecPwr)
            Me.Controls.Add(Me.m_txtK)
            Me.Controls.Add(Me.m_lblFF)
            Me.Controls.Add(Me.m_lblWmatWinf)
            Me.Controls.Add(Me.m_lblBAB)
            Me.Controls.Add(Me.m_lblRecPwr)
            Me.Controls.Add(Me.m_lblK)
            Me.Controls.Add(Me.m_lblStanzaGroups)
            Me.Controls.Add(Me.m_grid)
            Me.Controls.Add(Me.m_btnCancel)
            Me.Controls.Add(Me.m_btnOK)
            Me.Controls.Add(Me.m_btnCalculate)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "EditMultiStanza"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cmbStanzaGroups As System.Windows.Forms.ComboBox
        Private WithEvents m_txtK As System.Windows.Forms.TextBox
        Private WithEvents m_txtRecPwr As System.Windows.Forms.TextBox
        Private WithEvents m_txtBAB As System.Windows.Forms.TextBox
        Private WithEvents m_txtWmatWinf As System.Windows.Forms.TextBox
        Private WithEvents m_cmbFF As System.Windows.Forms.ComboBox
        Private WithEvents m_zgc As ZedGraph.ZedGraphControl
        Private WithEvents m_cbFFecun As System.Windows.Forms.CheckBox
        Private WithEvents m_grid As EditMultiStanzaEwEGrid
        Private WithEvents m_btnCalculate As System.Windows.Forms.Button
        Private WithEvents m_btnOK As System.Windows.Forms.Button
        Private WithEvents m_btnCancel As System.Windows.Forms.Button
        Private WithEvents m_lblStanzaGroups As System.Windows.Forms.Label
        Private WithEvents m_lblK As System.Windows.Forms.Label
        Private WithEvents m_lblRecPwr As System.Windows.Forms.Label
        Private WithEvents m_lblBAB As System.Windows.Forms.Label
        Private WithEvents m_lblWmatWinf As System.Windows.Forms.Label
        Private WithEvents m_lblFF As System.Windows.Forms.Label
    End Class

End Namespace
