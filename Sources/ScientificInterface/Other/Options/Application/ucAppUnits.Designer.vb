Namespace Other

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class ucAppUnits
        Inherits System.Windows.Forms.UserControl

        'UserControl overrides dispose to clean up the component list.
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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucAppUnits))
            Me.rbNutrientOther = New System.Windows.Forms.RadioButton
            Me.rbPhosporus = New System.Windows.Forms.RadioButton
            Me.rbNitrogen = New System.Windows.Forms.RadioButton
            Me.rbEnergyOther = New System.Windows.Forms.RadioButton
            Me.rbDryWeight = New System.Windows.Forms.RadioButton
            Me.rbCarbon = New System.Windows.Forms.RadioButton
            Me.rbCalorie = New System.Windows.Forms.RadioButton
            Me.rbJoules = New System.Windows.Forms.RadioButton
            Me.rbWetWeight = New System.Windows.Forms.RadioButton
            Me.gbpTime = New System.Windows.Forms.GroupBox
            Me.lbNote = New System.Windows.Forms.Label
            Me.txbTimeOther = New System.Windows.Forms.TextBox
            Me.rbTimeOther = New System.Windows.Forms.RadioButton
            Me.rbDay = New System.Windows.Forms.RadioButton
            Me.rbYear = New System.Windows.Forms.RadioButton
            Me.lblTitle = New System.Windows.Forms.Label
            Me.gbEnergy = New System.Windows.Forms.GroupBox
            Me.txbEnergyOther = New System.Windows.Forms.TextBox
            Me.gbNutrients = New System.Windows.Forms.GroupBox
            Me.txbNutrientOther = New System.Windows.Forms.TextBox
            Me.m_tlbOuter = New System.Windows.Forms.TableLayoutPanel
            Me.m_tlpLeftCol = New System.Windows.Forms.TableLayoutPanel
            Me.gbpTime.SuspendLayout()
            Me.gbEnergy.SuspendLayout()
            Me.gbNutrients.SuspendLayout()
            Me.m_tlbOuter.SuspendLayout()
            Me.m_tlpLeftCol.SuspendLayout()
            Me.SuspendLayout()
            '
            'rbNutrientOther
            '
            resources.ApplyResources(Me.rbNutrientOther, "rbNutrientOther")
            Me.rbNutrientOther.Name = "rbNutrientOther"
            Me.rbNutrientOther.UseVisualStyleBackColor = True
            '
            'rbPhosporus
            '
            resources.ApplyResources(Me.rbPhosporus, "rbPhosporus")
            Me.rbPhosporus.Name = "rbPhosporus"
            Me.rbPhosporus.UseVisualStyleBackColor = True
            '
            'rbNitrogen
            '
            resources.ApplyResources(Me.rbNitrogen, "rbNitrogen")
            Me.rbNitrogen.Name = "rbNitrogen"
            Me.rbNitrogen.UseVisualStyleBackColor = True
            '
            'rbEnergyOther
            '
            resources.ApplyResources(Me.rbEnergyOther, "rbEnergyOther")
            Me.rbEnergyOther.Name = "rbEnergyOther"
            Me.rbEnergyOther.UseVisualStyleBackColor = True
            '
            'rbDryWeight
            '
            resources.ApplyResources(Me.rbDryWeight, "rbDryWeight")
            Me.rbDryWeight.Name = "rbDryWeight"
            Me.rbDryWeight.UseVisualStyleBackColor = True
            '
            'rbCarbon
            '
            resources.ApplyResources(Me.rbCarbon, "rbCarbon")
            Me.rbCarbon.Name = "rbCarbon"
            Me.rbCarbon.UseVisualStyleBackColor = True
            '
            'rbCalorie
            '
            resources.ApplyResources(Me.rbCalorie, "rbCalorie")
            Me.rbCalorie.Name = "rbCalorie"
            Me.rbCalorie.UseVisualStyleBackColor = True
            '
            'rbJoules
            '
            resources.ApplyResources(Me.rbJoules, "rbJoules")
            Me.rbJoules.Name = "rbJoules"
            Me.rbJoules.UseVisualStyleBackColor = True
            '
            'rbWetWeight
            '
            resources.ApplyResources(Me.rbWetWeight, "rbWetWeight")
            Me.rbWetWeight.Checked = True
            Me.rbWetWeight.Name = "rbWetWeight"
            Me.rbWetWeight.TabStop = True
            Me.rbWetWeight.UseVisualStyleBackColor = True
            '
            'gbpTime
            '
            Me.gbpTime.Controls.Add(Me.lbNote)
            Me.gbpTime.Controls.Add(Me.txbTimeOther)
            Me.gbpTime.Controls.Add(Me.rbTimeOther)
            Me.gbpTime.Controls.Add(Me.rbDay)
            Me.gbpTime.Controls.Add(Me.rbYear)
            resources.ApplyResources(Me.gbpTime, "gbpTime")
            Me.gbpTime.Name = "gbpTime"
            Me.gbpTime.TabStop = False
            '
            'lbNote
            '
            resources.ApplyResources(Me.lbNote, "lbNote")
            Me.lbNote.Name = "lbNote"
            '
            'txbTimeOther
            '
            resources.ApplyResources(Me.txbTimeOther, "txbTimeOther")
            Me.txbTimeOther.Name = "txbTimeOther"
            '
            'rbTimeOther
            '
            resources.ApplyResources(Me.rbTimeOther, "rbTimeOther")
            Me.rbTimeOther.Name = "rbTimeOther"
            Me.rbTimeOther.UseVisualStyleBackColor = True
            '
            'rbDay
            '
            resources.ApplyResources(Me.rbDay, "rbDay")
            Me.rbDay.Name = "rbDay"
            Me.rbDay.UseVisualStyleBackColor = True
            '
            'rbYear
            '
            resources.ApplyResources(Me.rbYear, "rbYear")
            Me.rbYear.Checked = True
            Me.rbYear.Name = "rbYear"
            Me.rbYear.TabStop = True
            Me.rbYear.UseVisualStyleBackColor = True
            '
            'lblTitle
            '
            Me.lblTitle.BackColor = System.Drawing.SystemColors.ButtonShadow
            resources.ApplyResources(Me.lblTitle, "lblTitle")
            Me.lblTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblTitle.Name = "lblTitle"
            '
            'gbEnergy
            '
            Me.gbEnergy.Controls.Add(Me.txbEnergyOther)
            Me.gbEnergy.Controls.Add(Me.rbWetWeight)
            Me.gbEnergy.Controls.Add(Me.rbJoules)
            Me.gbEnergy.Controls.Add(Me.rbCalorie)
            Me.gbEnergy.Controls.Add(Me.rbCarbon)
            Me.gbEnergy.Controls.Add(Me.rbDryWeight)
            Me.gbEnergy.Controls.Add(Me.rbEnergyOther)
            resources.ApplyResources(Me.gbEnergy, "gbEnergy")
            Me.gbEnergy.Name = "gbEnergy"
            Me.gbEnergy.TabStop = False
            '
            'txbEnergyOther
            '
            resources.ApplyResources(Me.txbEnergyOther, "txbEnergyOther")
            Me.txbEnergyOther.Name = "txbEnergyOther"
            '
            'gbNutrients
            '
            Me.gbNutrients.Controls.Add(Me.txbNutrientOther)
            Me.gbNutrients.Controls.Add(Me.rbNitrogen)
            Me.gbNutrients.Controls.Add(Me.rbPhosporus)
            Me.gbNutrients.Controls.Add(Me.rbNutrientOther)
            resources.ApplyResources(Me.gbNutrients, "gbNutrients")
            Me.gbNutrients.Name = "gbNutrients"
            Me.gbNutrients.TabStop = False
            '
            'txbNutrientOther
            '
            resources.ApplyResources(Me.txbNutrientOther, "txbNutrientOther")
            Me.txbNutrientOther.Name = "txbNutrientOther"
            '
            'm_tlbOuter
            '
            resources.ApplyResources(Me.m_tlbOuter, "m_tlbOuter")
            Me.m_tlbOuter.Controls.Add(Me.gbpTime, 1, 0)
            Me.m_tlbOuter.Controls.Add(Me.m_tlpLeftCol, 0, 0)
            Me.m_tlbOuter.Name = "m_tlbOuter"
            '
            'm_tlpLeftCol
            '
            resources.ApplyResources(Me.m_tlpLeftCol, "m_tlpLeftCol")
            Me.m_tlpLeftCol.Controls.Add(Me.gbNutrients, 0, 1)
            Me.m_tlpLeftCol.Controls.Add(Me.gbEnergy, 0, 0)
            Me.m_tlpLeftCol.Name = "m_tlpLeftCol"
            '
            'ucAppUnits
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_tlbOuter)
            Me.Controls.Add(Me.lblTitle)
            Me.Name = "ucAppUnits"
            Me.gbpTime.ResumeLayout(False)
            Me.gbpTime.PerformLayout()
            Me.gbEnergy.ResumeLayout(False)
            Me.gbEnergy.PerformLayout()
            Me.gbNutrients.ResumeLayout(False)
            Me.gbNutrients.PerformLayout()
            Me.m_tlbOuter.ResumeLayout(False)
            Me.m_tlpLeftCol.ResumeLayout(False)
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents rbNutrientOther As System.Windows.Forms.RadioButton
        Friend WithEvents rbPhosporus As System.Windows.Forms.RadioButton
        Friend WithEvents rbNitrogen As System.Windows.Forms.RadioButton
        Friend WithEvents rbEnergyOther As System.Windows.Forms.RadioButton
        Friend WithEvents rbDryWeight As System.Windows.Forms.RadioButton
        Friend WithEvents rbCarbon As System.Windows.Forms.RadioButton
        Friend WithEvents rbCalorie As System.Windows.Forms.RadioButton
        Friend WithEvents rbJoules As System.Windows.Forms.RadioButton
        Friend WithEvents rbWetWeight As System.Windows.Forms.RadioButton
        Friend WithEvents gbpTime As System.Windows.Forms.GroupBox
        Friend WithEvents txbTimeOther As System.Windows.Forms.TextBox
        Friend WithEvents rbTimeOther As System.Windows.Forms.RadioButton
        Friend WithEvents rbDay As System.Windows.Forms.RadioButton
        Friend WithEvents rbYear As System.Windows.Forms.RadioButton
        Friend WithEvents lblTitle As System.Windows.Forms.Label
        Friend WithEvents lbNote As System.Windows.Forms.Label
        Friend WithEvents gbEnergy As System.Windows.Forms.GroupBox
        Friend WithEvents gbNutrients As System.Windows.Forms.GroupBox
        Friend WithEvents txbNutrientOther As System.Windows.Forms.TextBox
        Friend WithEvents txbEnergyOther As System.Windows.Forms.TextBox
        Friend WithEvents m_tlbOuter As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents m_tlpLeftCol As System.Windows.Forms.TableLayoutPanel

    End Class

End Namespace

