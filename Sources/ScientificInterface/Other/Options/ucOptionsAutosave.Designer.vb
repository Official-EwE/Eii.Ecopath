' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucOptionsAutosave
        Inherits System.Windows.Forms.UserControl

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.m_cbEcosimRun = New System.Windows.Forms.CheckBox()
            Me.m_hdrEcosim = New ScientificInterfaceShared.Controls.cEwEHeaderLabel()
            Me.m_cbMonteCarlo = New System.Windows.Forms.CheckBox()
            Me.m_cbAutosaveAll = New System.Windows.Forms.CheckBox()
            Me.m_cbEcospace = New System.Windows.Forms.CheckBox()
            Me.m_cbSpaceASCII = New System.Windows.Forms.CheckBox()
            Me.m_cbSpaceCSV = New System.Windows.Forms.CheckBox()
            Me.m_cbEcosim = New System.Windows.Forms.CheckBox()
            Me.m_cbMSE = New System.Windows.Forms.CheckBox()
            Me.m_cbEcotracer = New System.Windows.Forms.CheckBox()
            Me.SuspendLayout()
            '
            'm_cbEcosimRun
            '
            Me.m_cbEcosimRun.AutoSize = True
            Me.m_cbEcosimRun.Location = New System.Drawing.Point(41, 69)
            Me.m_cbEcosimRun.Name = "m_cbEcosimRun"
            Me.m_cbEcosimRun.Size = New System.Drawing.Size(79, 17)
            Me.m_cbEcosimRun.TabIndex = 4
            Me.m_cbEcosimRun.Text = "Run results"
            Me.m_cbEcosimRun.UseVisualStyleBackColor = True
            '
            'm_hdrEcosim
            '
            Me.m_hdrEcosim.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
            Me.m_hdrEcosim.CanCollapseParent = False
            Me.m_hdrEcosim.CollapsedParentHeight = 0
            Me.m_hdrEcosim.IsCollapsed = False
            Me.m_hdrEcosim.Location = New System.Drawing.Point(0, 0)
            Me.m_hdrEcosim.Name = "m_hdrEcosim"
            Me.m_hdrEcosim.Size = New System.Drawing.Size(553, 18)
            Me.m_hdrEcosim.TabIndex = 3
            Me.m_hdrEcosim.Text = "Auto-save results"
            Me.m_hdrEcosim.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            '
            'm_cbMonteCarlo
            '
            Me.m_cbMonteCarlo.AutoSize = True
            Me.m_cbMonteCarlo.Location = New System.Drawing.Point(41, 92)
            Me.m_cbMonteCarlo.Name = "m_cbMonteCarlo"
            Me.m_cbMonteCarlo.Size = New System.Drawing.Size(83, 17)
            Me.m_cbMonteCarlo.TabIndex = 4
            Me.m_cbMonteCarlo.Text = "Monte Carlo"
            Me.m_cbMonteCarlo.UseVisualStyleBackColor = True
            '
            'm_cbAutosaveAll
            '
            Me.m_cbAutosaveAll.AutoSize = True
            Me.m_cbAutosaveAll.Location = New System.Drawing.Point(6, 23)
            Me.m_cbAutosaveAll.Name = "m_cbAutosaveAll"
            Me.m_cbAutosaveAll.Size = New System.Drawing.Size(84, 17)
            Me.m_cbAutosaveAll.TabIndex = 21
            Me.m_cbAutosaveAll.Text = "Autosave all"
            Me.m_cbAutosaveAll.UseVisualStyleBackColor = True
            '
            'm_cbEcospace
            '
            Me.m_cbEcospace.AutoSize = True
            Me.m_cbEcospace.Location = New System.Drawing.Point(23, 143)
            Me.m_cbEcospace.Name = "m_cbEcospace"
            Me.m_cbEcospace.Size = New System.Drawing.Size(74, 17)
            Me.m_cbEcospace.TabIndex = 21
            Me.m_cbEcospace.Text = "Ecospace"
            Me.m_cbEcospace.UseVisualStyleBackColor = True
            '
            'm_cbSpaceASCII
            '
            Me.m_cbSpaceASCII.AutoSize = True
            Me.m_cbSpaceASCII.Location = New System.Drawing.Point(41, 166)
            Me.m_cbSpaceASCII.Name = "m_cbSpaceASCII"
            Me.m_cbSpaceASCII.Size = New System.Drawing.Size(81, 17)
            Me.m_cbSpaceASCII.TabIndex = 4
            Me.m_cbSpaceASCII.Text = "ASCII maps"
            Me.m_cbSpaceASCII.UseVisualStyleBackColor = True
            '
            'm_cbSpaceCSV
            '
            Me.m_cbSpaceCSV.AutoSize = True
            Me.m_cbSpaceCSV.Location = New System.Drawing.Point(41, 189)
            Me.m_cbSpaceCSV.Name = "m_cbSpaceCSV"
            Me.m_cbSpaceCSV.Size = New System.Drawing.Size(75, 17)
            Me.m_cbSpaceCSV.TabIndex = 4
            Me.m_cbSpaceCSV.Text = "CSV maps"
            Me.m_cbSpaceCSV.UseVisualStyleBackColor = True
            '
            'm_cbEcosim
            '
            Me.m_cbEcosim.AutoSize = True
            Me.m_cbEcosim.Location = New System.Drawing.Point(23, 46)
            Me.m_cbEcosim.Name = "m_cbEcosim"
            Me.m_cbEcosim.Size = New System.Drawing.Size(60, 17)
            Me.m_cbEcosim.TabIndex = 21
            Me.m_cbEcosim.Text = "Ecosim"
            Me.m_cbEcosim.UseVisualStyleBackColor = True
            '
            'm_cbMSE
            '
            Me.m_cbMSE.AutoSize = True
            Me.m_cbMSE.Location = New System.Drawing.Point(41, 115)
            Me.m_cbMSE.Name = "m_cbMSE"
            Me.m_cbMSE.Size = New System.Drawing.Size(183, 17)
            Me.m_cbMSE.TabIndex = 4
            Me.m_cbMSE.Text = "Management Strategy Evaluation"
            Me.m_cbMSE.UseVisualStyleBackColor = True
            '
            'm_cbEcotracer
            '
            Me.m_cbEcotracer.AutoSize = True
            Me.m_cbEcotracer.Location = New System.Drawing.Point(23, 212)
            Me.m_cbEcotracer.Name = "m_cbEcotracer"
            Me.m_cbEcotracer.Size = New System.Drawing.Size(72, 17)
            Me.m_cbEcotracer.TabIndex = 21
            Me.m_cbEcotracer.Text = "Ecotracer"
            Me.m_cbEcotracer.UseVisualStyleBackColor = True
            '
            'ucOptionsAutosave
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_cbEcotracer)
            Me.Controls.Add(Me.m_cbEcospace)
            Me.Controls.Add(Me.m_cbEcosim)
            Me.Controls.Add(Me.m_cbAutosaveAll)
            Me.Controls.Add(Me.m_cbMSE)
            Me.Controls.Add(Me.m_cbMonteCarlo)
            Me.Controls.Add(Me.m_cbSpaceCSV)
            Me.Controls.Add(Me.m_cbSpaceASCII)
            Me.Controls.Add(Me.m_cbEcosimRun)
            Me.Controls.Add(Me.m_hdrEcosim)
            Me.Name = "ucOptionsAutosave"
            Me.Size = New System.Drawing.Size(553, 378)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Private WithEvents m_cbEcosimRun As System.Windows.Forms.CheckBox
        Private WithEvents m_hdrEcosim As ScientificInterfaceShared.Controls.cEwEHeaderLabel
        Private WithEvents m_cbMonteCarlo As System.Windows.Forms.CheckBox
        Private WithEvents m_cbAutosaveAll As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcospace As System.Windows.Forms.CheckBox
        Private WithEvents m_cbSpaceASCII As System.Windows.Forms.CheckBox
        Private WithEvents m_cbSpaceCSV As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcosim As System.Windows.Forms.CheckBox
        Private WithEvents m_cbMSE As System.Windows.Forms.CheckBox
        Private WithEvents m_cbEcotracer As System.Windows.Forms.CheckBox

    End Class

End Namespace
