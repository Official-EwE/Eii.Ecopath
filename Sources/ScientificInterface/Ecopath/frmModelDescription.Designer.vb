Imports ScientificInterfaceShared.Controls

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmModelDescription
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmModelDescription))
        Me.m_udNumDigits = New System.Windows.Forms.NumericUpDown
        Me.lbNumDigits = New System.Windows.Forms.Label
        Me.m_lblOptions = New cEwEHeaderLabel
        Me.m_lbDescription = New System.Windows.Forms.Label
        Me.m_lbScenarioName = New System.Windows.Forms.Label
        Me.m_lblModel = New cEwEHeaderLabel
        Me.m_tbName = New System.Windows.Forms.TextBox
        Me.m_lbAuthor = New System.Windows.Forms.Label
        Me.m_tbAuthor = New System.Windows.Forms.TextBox
        Me.m_lbContact = New System.Windows.Forms.Label
        Me.m_lblFile = New System.Windows.Forms.Label
        Me.m_lblArea = New System.Windows.Forms.Label
        Me.m_tbArea = New System.Windows.Forms.TextBox
        Me.m_lblAreaUnit = New System.Windows.Forms.Label
        Me.m_tlpUnits = New System.Windows.Forms.TableLayoutPanel
        Me.m_gbCurrencyUnit = New System.Windows.Forms.GroupBox
        Me.tbCurrencyNutrientOther = New System.Windows.Forms.TextBox
        Me.rbNitrogen = New System.Windows.Forms.RadioButton
        Me.rbNutrientOther = New System.Windows.Forms.RadioButton
        Me.rbPhosporus = New System.Windows.Forms.RadioButton
        Me.tbCurrencyEnergyOther = New System.Windows.Forms.TextBox
        Me.rbCurrencyEnergyOther = New System.Windows.Forms.RadioButton
        Me.m_lblNutrientRelated = New System.Windows.Forms.Label
        Me.m_lblEnergyRelated = New System.Windows.Forms.Label
        Me.rbWetWeight = New System.Windows.Forms.RadioButton
        Me.rbJoules = New System.Windows.Forms.RadioButton
        Me.rbCalorie = New System.Windows.Forms.RadioButton
        Me.rbCarbon = New System.Windows.Forms.RadioButton
        Me.rbDryWeight = New System.Windows.Forms.RadioButton
        Me.m_gbTimeUnits = New System.Windows.Forms.GroupBox
        Me.txbTimeOther = New System.Windows.Forms.TextBox
        Me.rbTimeOther = New System.Windows.Forms.RadioButton
        Me.rbDay = New System.Windows.Forms.RadioButton
        Me.rbYear = New System.Windows.Forms.RadioButton
        Me.m_gbMonetaryUnits = New System.Windows.Forms.GroupBox
        Me.m_lblMonetaryUnit = New System.Windows.Forms.Label
        Me.m_cmbMonetaryUnit = New ScientificInterfaceShared.Controls.cMonetaryUnitComboBox
        Me.m_gbNumFormatting = New System.Windows.Forms.GroupBox
        Me.m_cbGroupDigits = New System.Windows.Forms.CheckBox
        Me.m_chkPSD = New System.Windows.Forms.CheckBox
        Me.m_tbxFile = New System.Windows.Forms.TextBox
        Me.m_tbContact = New System.Windows.Forms.RichTextBox
        Me.m_tbDescription = New System.Windows.Forms.RichTextBox
        Me.m_hdrExecution = New cEwEHeaderLabel
        CType(Me.m_udNumDigits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpUnits.SuspendLayout()
        Me.m_gbCurrencyUnit.SuspendLayout()
        Me.m_gbTimeUnits.SuspendLayout()
        Me.m_gbMonetaryUnits.SuspendLayout()
        Me.m_gbNumFormatting.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_udNumDigits
        '
        resources.ApplyResources(Me.m_udNumDigits, "m_udNumDigits")
        Me.m_udNumDigits.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.m_udNumDigits.Name = "m_udNumDigits"
        '
        'lbNumDigits
        '
        resources.ApplyResources(Me.lbNumDigits, "lbNumDigits")
        Me.lbNumDigits.Name = "lbNumDigits"
        '
        'm_lblOptions
        '
        resources.ApplyResources(Me.m_lblOptions, "m_lblOptions")
        Me.m_lblOptions.Name = "m_lblOptions"
        '
        'm_lbDescription
        '
        resources.ApplyResources(Me.m_lbDescription, "m_lbDescription")
        Me.m_lbDescription.Name = "m_lbDescription"
        '
        'm_lbScenarioName
        '
        resources.ApplyResources(Me.m_lbScenarioName, "m_lbScenarioName")
        Me.m_lbScenarioName.Name = "m_lbScenarioName"
        '
        'm_lblModel
        '
        resources.ApplyResources(Me.m_lblModel, "m_lblModel")
        Me.m_lblModel.Name = "m_lblModel"
        '
        'm_tbName
        '
        resources.ApplyResources(Me.m_tbName, "m_tbName")
        Me.m_tbName.Name = "m_tbName"
        '
        'm_lbAuthor
        '
        resources.ApplyResources(Me.m_lbAuthor, "m_lbAuthor")
        Me.m_lbAuthor.Name = "m_lbAuthor"
        '
        'm_tbAuthor
        '
        resources.ApplyResources(Me.m_tbAuthor, "m_tbAuthor")
        Me.m_tbAuthor.Name = "m_tbAuthor"
        '
        'm_lbContact
        '
        resources.ApplyResources(Me.m_lbContact, "m_lbContact")
        Me.m_lbContact.Name = "m_lbContact"
        '
        'm_lblFile
        '
        resources.ApplyResources(Me.m_lblFile, "m_lblFile")
        Me.m_lblFile.Name = "m_lblFile"
        '
        'm_lblArea
        '
        resources.ApplyResources(Me.m_lblArea, "m_lblArea")
        Me.m_lblArea.Name = "m_lblArea"
        '
        'm_tbArea
        '
        resources.ApplyResources(Me.m_tbArea, "m_tbArea")
        Me.m_tbArea.Name = "m_tbArea"
        '
        'm_lblAreaUnit
        '
        resources.ApplyResources(Me.m_lblAreaUnit, "m_lblAreaUnit")
        Me.m_lblAreaUnit.Name = "m_lblAreaUnit"
        '
        'm_tlpUnits
        '
        resources.ApplyResources(Me.m_tlpUnits, "m_tlpUnits")
        Me.m_tlpUnits.Controls.Add(Me.m_gbCurrencyUnit, 0, 1)
        Me.m_tlpUnits.Controls.Add(Me.m_gbTimeUnits, 1, 1)
        Me.m_tlpUnits.Controls.Add(Me.m_gbMonetaryUnits, 1, 0)
        Me.m_tlpUnits.Controls.Add(Me.m_gbNumFormatting, 0, 0)
        Me.m_tlpUnits.Name = "m_tlpUnits"
        '
        'm_gbCurrencyUnit
        '
        Me.m_gbCurrencyUnit.Controls.Add(Me.tbCurrencyNutrientOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbNitrogen)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbNutrientOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbPhosporus)
        Me.m_gbCurrencyUnit.Controls.Add(Me.tbCurrencyEnergyOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCurrencyEnergyOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.m_lblNutrientRelated)
        Me.m_gbCurrencyUnit.Controls.Add(Me.m_lblEnergyRelated)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbWetWeight)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbJoules)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCalorie)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCarbon)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbDryWeight)
        resources.ApplyResources(Me.m_gbCurrencyUnit, "m_gbCurrencyUnit")
        Me.m_gbCurrencyUnit.Name = "m_gbCurrencyUnit"
        Me.m_gbCurrencyUnit.TabStop = False
        '
        'tbCurrencyNutrientOther
        '
        resources.ApplyResources(Me.tbCurrencyNutrientOther, "tbCurrencyNutrientOther")
        Me.tbCurrencyNutrientOther.Name = "tbCurrencyNutrientOther"
        '
        'rbNitrogen
        '
        resources.ApplyResources(Me.rbNitrogen, "rbNitrogen")
        Me.rbNitrogen.Name = "rbNitrogen"
        Me.rbNitrogen.UseVisualStyleBackColor = True
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
        'tbCurrencyEnergyOther
        '
        resources.ApplyResources(Me.tbCurrencyEnergyOther, "tbCurrencyEnergyOther")
        Me.tbCurrencyEnergyOther.Name = "tbCurrencyEnergyOther"
        '
        'rbCurrencyEnergyOther
        '
        resources.ApplyResources(Me.rbCurrencyEnergyOther, "rbCurrencyEnergyOther")
        Me.rbCurrencyEnergyOther.Name = "rbCurrencyEnergyOther"
        Me.rbCurrencyEnergyOther.UseVisualStyleBackColor = True
        '
        'm_lblNutrientRelated
        '
        resources.ApplyResources(Me.m_lblNutrientRelated, "m_lblNutrientRelated")
        Me.m_lblNutrientRelated.Name = "m_lblNutrientRelated"
        '
        'm_lblEnergyRelated
        '
        resources.ApplyResources(Me.m_lblEnergyRelated, "m_lblEnergyRelated")
        Me.m_lblEnergyRelated.Name = "m_lblEnergyRelated"
        '
        'rbWetWeight
        '
        resources.ApplyResources(Me.rbWetWeight, "rbWetWeight")
        Me.rbWetWeight.Checked = True
        Me.rbWetWeight.Name = "rbWetWeight"
        Me.rbWetWeight.TabStop = True
        Me.rbWetWeight.UseVisualStyleBackColor = True
        '
        'rbJoules
        '
        resources.ApplyResources(Me.rbJoules, "rbJoules")
        Me.rbJoules.Name = "rbJoules"
        Me.rbJoules.UseVisualStyleBackColor = True
        '
        'rbCalorie
        '
        resources.ApplyResources(Me.rbCalorie, "rbCalorie")
        Me.rbCalorie.Name = "rbCalorie"
        Me.rbCalorie.UseVisualStyleBackColor = True
        '
        'rbCarbon
        '
        resources.ApplyResources(Me.rbCarbon, "rbCarbon")
        Me.rbCarbon.Name = "rbCarbon"
        Me.rbCarbon.UseVisualStyleBackColor = True
        '
        'rbDryWeight
        '
        resources.ApplyResources(Me.rbDryWeight, "rbDryWeight")
        Me.rbDryWeight.Name = "rbDryWeight"
        Me.rbDryWeight.UseVisualStyleBackColor = True
        '
        'm_gbTimeUnits
        '
        Me.m_gbTimeUnits.Controls.Add(Me.txbTimeOther)
        Me.m_gbTimeUnits.Controls.Add(Me.rbTimeOther)
        Me.m_gbTimeUnits.Controls.Add(Me.rbDay)
        Me.m_gbTimeUnits.Controls.Add(Me.rbYear)
        resources.ApplyResources(Me.m_gbTimeUnits, "m_gbTimeUnits")
        Me.m_gbTimeUnits.Name = "m_gbTimeUnits"
        Me.m_gbTimeUnits.TabStop = False
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
        'm_gbMonetaryUnits
        '
        Me.m_gbMonetaryUnits.Controls.Add(Me.m_lblMonetaryUnit)
        Me.m_gbMonetaryUnits.Controls.Add(Me.m_cmbMonetaryUnit)
        resources.ApplyResources(Me.m_gbMonetaryUnits, "m_gbMonetaryUnits")
        Me.m_gbMonetaryUnits.Name = "m_gbMonetaryUnits"
        Me.m_gbMonetaryUnits.TabStop = False
        '
        'm_lblMonetaryUnit
        '
        resources.ApplyResources(Me.m_lblMonetaryUnit, "m_lblMonetaryUnit")
        Me.m_lblMonetaryUnit.Name = "m_lblMonetaryUnit"
        '
        'm_cmbMonetaryUnit
        '
        resources.ApplyResources(Me.m_cmbMonetaryUnit, "m_cmbMonetaryUnit")
        Me.m_cmbMonetaryUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbMonetaryUnit.FormattingEnabled = True
        Me.m_cmbMonetaryUnit.Name = "m_cmbMonetaryUnit"
        Me.m_cmbMonetaryUnit.Sorted = True
        Me.m_cmbMonetaryUnit.UIContext = Nothing
        Me.m_cmbMonetaryUnit.Unit = EwEUtils.Core.eUnitMonetaryType.NotSet
        '
        'm_gbNumFormatting
        '
        Me.m_gbNumFormatting.Controls.Add(Me.m_cbGroupDigits)
        Me.m_gbNumFormatting.Controls.Add(Me.lbNumDigits)
        Me.m_gbNumFormatting.Controls.Add(Me.m_udNumDigits)
        resources.ApplyResources(Me.m_gbNumFormatting, "m_gbNumFormatting")
        Me.m_gbNumFormatting.Name = "m_gbNumFormatting"
        Me.m_gbNumFormatting.TabStop = False
        '
        'm_cbGroupDigits
        '
        resources.ApplyResources(Me.m_cbGroupDigits, "m_cbGroupDigits")
        Me.m_cbGroupDigits.Name = "m_cbGroupDigits"
        Me.m_cbGroupDigits.UseVisualStyleBackColor = True
        '
        'm_chkPSD
        '
        resources.ApplyResources(Me.m_chkPSD, "m_chkPSD")
        Me.m_chkPSD.Name = "m_chkPSD"
        Me.m_chkPSD.UseVisualStyleBackColor = True
        '
        'm_tbxFile
        '
        resources.ApplyResources(Me.m_tbxFile, "m_tbxFile")
        Me.m_tbxFile.Name = "m_tbxFile"
        Me.m_tbxFile.ReadOnly = True
        '
        'm_tbContact
        '
        resources.ApplyResources(Me.m_tbContact, "m_tbContact")
        Me.m_tbContact.Name = "m_tbContact"
        '
        'm_tbDescription
        '
        resources.ApplyResources(Me.m_tbDescription, "m_tbDescription")
        Me.m_tbDescription.Name = "m_tbDescription"
        '
        'm_hdrExecution
        '
        resources.ApplyResources(Me.m_hdrExecution, "m_hdrExecution")
        Me.m_hdrExecution.Name = "m_hdrExecution"
        '
        'frmModelDescription
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.m_tbDescription)
        Me.Controls.Add(Me.m_tbContact)
        Me.Controls.Add(Me.m_tbxFile)
        Me.Controls.Add(Me.m_chkPSD)
        Me.Controls.Add(Me.m_tlpUnits)
        Me.Controls.Add(Me.m_lblAreaUnit)
        Me.Controls.Add(Me.m_lblArea)
        Me.Controls.Add(Me.m_lblFile)
        Me.Controls.Add(Me.m_tbArea)
        Me.Controls.Add(Me.m_tbAuthor)
        Me.Controls.Add(Me.m_lblModel)
        Me.Controls.Add(Me.m_tbName)
        Me.Controls.Add(Me.m_lbContact)
        Me.Controls.Add(Me.m_hdrExecution)
        Me.Controls.Add(Me.m_lblOptions)
        Me.Controls.Add(Me.m_lbDescription)
        Me.Controls.Add(Me.m_lbScenarioName)
        Me.Controls.Add(Me.m_lbAuthor)
        Me.Name = "frmModelDescription"
        CType(Me.m_udNumDigits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpUnits.ResumeLayout(False)
        Me.m_gbCurrencyUnit.ResumeLayout(False)
        Me.m_gbCurrencyUnit.PerformLayout()
        Me.m_gbTimeUnits.ResumeLayout(False)
        Me.m_gbTimeUnits.PerformLayout()
        Me.m_gbMonetaryUnits.ResumeLayout(False)
        Me.m_gbMonetaryUnits.PerformLayout()
        Me.m_gbNumFormatting.ResumeLayout(False)
        Me.m_gbNumFormatting.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_udNumDigits As System.Windows.Forms.NumericUpDown
    Private WithEvents lbNumDigits As System.Windows.Forms.Label
    Private WithEvents m_lblOptions As cEwEHeaderLabel
    Private WithEvents m_lbDescription As System.Windows.Forms.Label
    Private WithEvents m_lbScenarioName As System.Windows.Forms.Label
    Private WithEvents m_lblModel As cEwEHeaderLabel
    Private WithEvents m_tbName As System.Windows.Forms.TextBox
    Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_lbContact As System.Windows.Forms.Label
    Private WithEvents m_lbAuthor As System.Windows.Forms.Label
    Private WithEvents m_lblFile As System.Windows.Forms.Label
    Private WithEvents m_lblArea As System.Windows.Forms.Label
    Private WithEvents m_tbArea As System.Windows.Forms.TextBox
    Private WithEvents m_lblAreaUnit As System.Windows.Forms.Label
    Private WithEvents m_tlpUnits As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_gbCurrencyUnit As System.Windows.Forms.GroupBox
    Private WithEvents m_lblEnergyRelated As System.Windows.Forms.Label
    Private WithEvents rbWetWeight As System.Windows.Forms.RadioButton
    Private WithEvents rbJoules As System.Windows.Forms.RadioButton
    Private WithEvents rbCalorie As System.Windows.Forms.RadioButton
    Private WithEvents rbCarbon As System.Windows.Forms.RadioButton
    Private WithEvents rbDryWeight As System.Windows.Forms.RadioButton
    Private WithEvents tbCurrencyEnergyOther As System.Windows.Forms.TextBox
    Private WithEvents rbCurrencyEnergyOther As System.Windows.Forms.RadioButton
    Private WithEvents rbNitrogen As System.Windows.Forms.RadioButton
    Private WithEvents rbPhosporus As System.Windows.Forms.RadioButton
    Private WithEvents m_lblNutrientRelated As System.Windows.Forms.Label
    Private WithEvents m_gbTimeUnits As System.Windows.Forms.GroupBox
    Private WithEvents txbTimeOther As System.Windows.Forms.TextBox
    Private WithEvents rbTimeOther As System.Windows.Forms.RadioButton
    Private WithEvents rbDay As System.Windows.Forms.RadioButton
    Private WithEvents rbYear As System.Windows.Forms.RadioButton
    Private WithEvents m_lblMonetaryUnit As System.Windows.Forms.Label
    Private WithEvents m_cmbMonetaryUnit As ScientificInterfaceShared.Controls.cMonetaryUnitComboBox
    Private WithEvents rbNutrientOther As System.Windows.Forms.RadioButton
    Private WithEvents tbCurrencyNutrientOther As System.Windows.Forms.TextBox
    Private WithEvents m_chkPSD As System.Windows.Forms.CheckBox
    Private WithEvents m_cbGroupDigits As System.Windows.Forms.CheckBox
    Private WithEvents m_gbMonetaryUnits As System.Windows.Forms.GroupBox
    Private WithEvents m_gbNumFormatting As System.Windows.Forms.GroupBox
    Private WithEvents m_tbxFile As System.Windows.Forms.TextBox
    Private WithEvents m_tbContact As System.Windows.Forms.RichTextBox
    Private WithEvents m_tbDescription As System.Windows.Forms.RichTextBox
    Private WithEvents m_hdrExecution As cEwEHeaderLabel
End Class
