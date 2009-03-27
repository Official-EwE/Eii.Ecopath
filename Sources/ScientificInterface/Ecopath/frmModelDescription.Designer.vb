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
        Me.m_udNumDigits = New System.Windows.Forms.NumericUpDown
        Me.lbNumDigits = New System.Windows.Forms.Label
        Me.lblColorHeader = New System.Windows.Forms.Label
        Me.m_lbDescription = New System.Windows.Forms.Label
        Me.lbScenarioName = New System.Windows.Forms.Label
        Me.lblScenario = New System.Windows.Forms.Label
        Me.m_tbName = New System.Windows.Forms.TextBox
        Me.m_lbAuthor = New System.Windows.Forms.Label
        Me.m_tbAuthor = New System.Windows.Forms.TextBox
        Me.m_lbContact = New System.Windows.Forms.Label
        Me.m_tbContact = New System.Windows.Forms.TextBox
        Me.m_tbDescription = New System.Windows.Forms.TextBox
        Me.m_txbPath = New System.Windows.Forms.Label
        Me.lblPath = New System.Windows.Forms.Label
        Me.lblArea = New System.Windows.Forms.Label
        Me.m_tbArea = New System.Windows.Forms.TextBox
        Me.lblAreaUnit = New System.Windows.Forms.Label
        Me.m_tlpUnits = New System.Windows.Forms.TableLayoutPanel
        Me.m_gbCurrencyUnit = New System.Windows.Forms.GroupBox
        Me.tbCurrencyNutrientOther = New System.Windows.Forms.TextBox
        Me.rbNitrogen = New System.Windows.Forms.RadioButton
        Me.rbNutrientOther = New System.Windows.Forms.RadioButton
        Me.rbPhosporus = New System.Windows.Forms.RadioButton
        Me.tbCurrencyEnergyOther = New System.Windows.Forms.TextBox
        Me.rbCurrencyEnergyOther = New System.Windows.Forms.RadioButton
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
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
        Me.lbNote = New System.Windows.Forms.Label
        Me.m_lblMonetaryUnit = New System.Windows.Forms.Label
        Me.m_cmbMonetaryUnit = New ScientificInterfaceShared.Controls.cMonetaryUnitComboBox
        CType(Me.m_udNumDigits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.m_tlpUnits.SuspendLayout()
        Me.m_gbCurrencyUnit.SuspendLayout()
        Me.m_gbTimeUnits.SuspendLayout()
        Me.SuspendLayout()
        '
        'm_udNumDigits
        '
        Me.m_udNumDigits.Location = New System.Drawing.Point(184, 356)
        Me.m_udNumDigits.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.m_udNumDigits.Name = "m_udNumDigits"
        Me.m_udNumDigits.Size = New System.Drawing.Size(64, 20)
        Me.m_udNumDigits.TabIndex = 16
        '
        'lbNumDigits
        '
        Me.lbNumDigits.AutoSize = True
        Me.lbNumDigits.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lbNumDigits.Location = New System.Drawing.Point(12, 358)
        Me.lbNumDigits.Name = "lbNumDigits"
        Me.lbNumDigits.Size = New System.Drawing.Size(166, 13)
        Me.lbNumDigits.TabIndex = 15
        Me.lbNumDigits.Text = "Number of relevant decimal &digits:"
        '
        'lblColorHeader
        '
        Me.lblColorHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblColorHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lblColorHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblColorHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblColorHeader.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblColorHeader.Location = New System.Drawing.Point(12, 326)
        Me.lblColorHeader.Name = "lblColorHeader"
        Me.lblColorHeader.Size = New System.Drawing.Size(647, 18)
        Me.lblColorHeader.TabIndex = 14
        Me.lblColorHeader.Text = "General options"
        Me.lblColorHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_lbDescription
        '
        Me.m_lbDescription.AutoSize = True
        Me.m_lbDescription.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lbDescription.Location = New System.Drawing.Point(12, 68)
        Me.m_lbDescription.Name = "m_lbDescription"
        Me.m_lbDescription.Size = New System.Drawing.Size(63, 13)
        Me.m_lbDescription.TabIndex = 3
        Me.m_lbDescription.Text = "D&escription:"
        '
        'lbScenarioName
        '
        Me.lbScenarioName.AutoSize = True
        Me.lbScenarioName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lbScenarioName.Location = New System.Drawing.Point(12, 42)
        Me.lbScenarioName.Name = "lbScenarioName"
        Me.lbScenarioName.Size = New System.Drawing.Size(38, 13)
        Me.lbScenarioName.TabIndex = 1
        Me.lbScenarioName.Text = "&Name:"
        '
        'lblScenario
        '
        Me.lblScenario.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
        Me.lblScenario.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.lblScenario.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblScenario.Location = New System.Drawing.Point(12, 9)
        Me.lblScenario.Name = "lblScenario"
        Me.lblScenario.Size = New System.Drawing.Size(647, 18)
        Me.lblScenario.TabIndex = 0
        Me.lblScenario.Text = "Model"
        Me.lblScenario.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'm_tbName
        '
        Me.m_tbName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbName.Location = New System.Drawing.Point(102, 39)
        Me.m_tbName.Name = "m_tbName"
        Me.m_tbName.Size = New System.Drawing.Size(557, 20)
        Me.m_tbName.TabIndex = 2
        '
        'm_lbAuthor
        '
        Me.m_lbAuthor.AutoSize = True
        Me.m_lbAuthor.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lbAuthor.Location = New System.Drawing.Point(12, 159)
        Me.m_lbAuthor.Name = "m_lbAuthor"
        Me.m_lbAuthor.Size = New System.Drawing.Size(41, 13)
        Me.m_lbAuthor.TabIndex = 5
        Me.m_lbAuthor.Text = "&Author:"
        '
        'm_tbAuthor
        '
        Me.m_tbAuthor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbAuthor.Location = New System.Drawing.Point(102, 156)
        Me.m_tbAuthor.MaxLength = 60
        Me.m_tbAuthor.Name = "m_tbAuthor"
        Me.m_tbAuthor.Size = New System.Drawing.Size(557, 20)
        Me.m_tbAuthor.TabIndex = 6
        '
        'm_lbContact
        '
        Me.m_lbContact.AutoSize = True
        Me.m_lbContact.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.m_lbContact.Location = New System.Drawing.Point(12, 185)
        Me.m_lbContact.Name = "m_lbContact"
        Me.m_lbContact.Size = New System.Drawing.Size(47, 13)
        Me.m_lbContact.TabIndex = 7
        Me.m_lbContact.Text = "&Contact:"
        '
        'm_tbContact
        '
        Me.m_tbContact.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbContact.Location = New System.Drawing.Point(102, 182)
        Me.m_tbContact.MaxLength = 250
        Me.m_tbContact.Multiline = True
        Me.m_tbContact.Name = "m_tbContact"
        Me.m_tbContact.Size = New System.Drawing.Size(557, 58)
        Me.m_tbContact.TabIndex = 8
        '
        'm_tbDescription
        '
        Me.m_tbDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tbDescription.Location = New System.Drawing.Point(102, 65)
        Me.m_tbDescription.Multiline = True
        Me.m_tbDescription.Name = "m_tbDescription"
        Me.m_tbDescription.Size = New System.Drawing.Size(557, 85)
        Me.m_tbDescription.TabIndex = 4
        '
        'm_txbPath
        '
        Me.m_txbPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_txbPath.Location = New System.Drawing.Point(102, 273)
        Me.m_txbPath.Name = "m_txbPath"
        Me.m_txbPath.Size = New System.Drawing.Size(557, 41)
        Me.m_txbPath.TabIndex = 13
        '
        'lblPath
        '
        Me.lblPath.AutoSize = True
        Me.lblPath.Location = New System.Drawing.Point(12, 273)
        Me.lblPath.Name = "lblPath"
        Me.lblPath.Size = New System.Drawing.Size(26, 13)
        Me.lblPath.TabIndex = 12
        Me.lblPath.Text = "File:"
        '
        'lblArea
        '
        Me.lblArea.AutoSize = True
        Me.lblArea.Location = New System.Drawing.Point(12, 250)
        Me.lblArea.Name = "lblArea"
        Me.lblArea.Size = New System.Drawing.Size(32, 13)
        Me.lblArea.TabIndex = 9
        Me.lblArea.Text = "A&rea:"
        '
        'm_tbArea
        '
        Me.m_tbArea.Location = New System.Drawing.Point(102, 247)
        Me.m_tbArea.MaxLength = 60
        Me.m_tbArea.Name = "m_tbArea"
        Me.m_tbArea.Size = New System.Drawing.Size(92, 20)
        Me.m_tbArea.TabIndex = 10
        '
        'lblAreaUnit
        '
        Me.lblAreaUnit.Location = New System.Drawing.Point(200, 250)
        Me.lblAreaUnit.Name = "lblAreaUnit"
        Me.lblAreaUnit.Size = New System.Drawing.Size(76, 17)
        Me.lblAreaUnit.TabIndex = 11
        Me.lblAreaUnit.Text = "km²"
        '
        'm_tlpUnits
        '
        Me.m_tlpUnits.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.m_tlpUnits.ColumnCount = 2
        Me.m_tlpUnits.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.71407!))
        Me.m_tlpUnits.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.28593!))
        Me.m_tlpUnits.Controls.Add(Me.m_gbCurrencyUnit, 0, 0)
        Me.m_tlpUnits.Controls.Add(Me.m_gbTimeUnits, 1, 0)
        Me.m_tlpUnits.Location = New System.Drawing.Point(12, 409)
        Me.m_tlpUnits.Margin = New System.Windows.Forms.Padding(0)
        Me.m_tlpUnits.Name = "m_tlpUnits"
        Me.m_tlpUnits.RowCount = 1
        Me.m_tlpUnits.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.m_tlpUnits.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 194.0!))
        Me.m_tlpUnits.Size = New System.Drawing.Size(647, 194)
        Me.m_tlpUnits.TabIndex = 19
        '
        'm_gbCurrencyUnit
        '
        Me.m_gbCurrencyUnit.Controls.Add(Me.tbCurrencyNutrientOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbNitrogen)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbNutrientOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbPhosporus)
        Me.m_gbCurrencyUnit.Controls.Add(Me.tbCurrencyEnergyOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCurrencyEnergyOther)
        Me.m_gbCurrencyUnit.Controls.Add(Me.Label2)
        Me.m_gbCurrencyUnit.Controls.Add(Me.Label1)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbWetWeight)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbJoules)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCalorie)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbCarbon)
        Me.m_gbCurrencyUnit.Controls.Add(Me.rbDryWeight)
        Me.m_gbCurrencyUnit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_gbCurrencyUnit.Location = New System.Drawing.Point(0, 3)
        Me.m_gbCurrencyUnit.Margin = New System.Windows.Forms.Padding(0, 3, 3, 3)
        Me.m_gbCurrencyUnit.Name = "m_gbCurrencyUnit"
        Me.m_gbCurrencyUnit.Size = New System.Drawing.Size(351, 188)
        Me.m_gbCurrencyUnit.TabIndex = 0
        Me.m_gbCurrencyUnit.TabStop = False
        Me.m_gbCurrencyUnit.Text = "C&urrency units"
        '
        'tbCurrencyNutrientOther
        '
        Me.tbCurrencyNutrientOther.Location = New System.Drawing.Point(243, 92)
        Me.tbCurrencyNutrientOther.Name = "tbCurrencyNutrientOther"
        Me.tbCurrencyNutrientOther.Size = New System.Drawing.Size(75, 20)
        Me.tbCurrencyNutrientOther.TabIndex = 16
        '
        'rbNitrogen
        '
        Me.rbNitrogen.AutoSize = True
        Me.rbNitrogen.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbNitrogen.Location = New System.Drawing.Point(184, 45)
        Me.rbNitrogen.Name = "rbNitrogen"
        Me.rbNitrogen.Size = New System.Drawing.Size(86, 17)
        Me.rbNitrogen.TabIndex = 14
        Me.rbNitrogen.Text = "nitrogen ({0})"
        Me.rbNitrogen.UseVisualStyleBackColor = True
        '
        'rbNutrientOther
        '
        Me.rbNutrientOther.AutoSize = True
        Me.rbNutrientOther.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbNutrientOther.Location = New System.Drawing.Point(184, 91)
        Me.rbNutrientOther.Name = "rbNutrientOther"
        Me.rbNutrientOther.Size = New System.Drawing.Size(52, 17)
        Me.rbNutrientOther.TabIndex = 15
        Me.rbNutrientOther.Text = "other:"
        Me.rbNutrientOther.UseVisualStyleBackColor = True
        '
        'rbPhosporus
        '
        Me.rbPhosporus.AutoSize = True
        Me.rbPhosporus.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbPhosporus.Location = New System.Drawing.Point(184, 68)
        Me.rbPhosporus.Name = "rbPhosporus"
        Me.rbPhosporus.Size = New System.Drawing.Size(97, 17)
        Me.rbPhosporus.TabIndex = 15
        Me.rbPhosporus.Text = "phosporus ({0})"
        Me.rbPhosporus.UseVisualStyleBackColor = True
        '
        'tbCurrencyEnergyOther
        '
        Me.tbCurrencyEnergyOther.Location = New System.Drawing.Point(82, 159)
        Me.tbCurrencyEnergyOther.Name = "tbCurrencyEnergyOther"
        Me.tbCurrencyEnergyOther.Size = New System.Drawing.Size(75, 20)
        Me.tbCurrencyEnergyOther.TabIndex = 13
        '
        'rbCurrencyEnergyOther
        '
        Me.rbCurrencyEnergyOther.AutoSize = True
        Me.rbCurrencyEnergyOther.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbCurrencyEnergyOther.Location = New System.Drawing.Point(24, 160)
        Me.rbCurrencyEnergyOther.Name = "rbCurrencyEnergyOther"
        Me.rbCurrencyEnergyOther.Size = New System.Drawing.Size(52, 17)
        Me.rbCurrencyEnergyOther.TabIndex = 12
        Me.rbCurrencyEnergyOther.Text = "other:"
        Me.rbCurrencyEnergyOther.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(166, 24)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 13)
        Me.Label2.TabIndex = 11
        Me.Label2.Text = "&Nutrient related:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 24)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(78, 13)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "&Energy related:"
        '
        'rbWetWeight
        '
        Me.rbWetWeight.AutoSize = True
        Me.rbWetWeight.Checked = True
        Me.rbWetWeight.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbWetWeight.Location = New System.Drawing.Point(24, 45)
        Me.rbWetWeight.Name = "rbWetWeight"
        Me.rbWetWeight.Size = New System.Drawing.Size(99, 17)
        Me.rbWetWeight.TabIndex = 6
        Me.rbWetWeight.TabStop = True
        Me.rbWetWeight.Text = "wet weight ({0})"
        Me.rbWetWeight.UseVisualStyleBackColor = True
        '
        'rbJoules
        '
        Me.rbJoules.AutoSize = True
        Me.rbJoules.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbJoules.Location = New System.Drawing.Point(24, 68)
        Me.rbJoules.Name = "rbJoules"
        Me.rbJoules.Size = New System.Drawing.Size(75, 17)
        Me.rbJoules.TabIndex = 7
        Me.rbJoules.Text = "joules ({0})"
        Me.rbJoules.UseVisualStyleBackColor = True
        '
        'rbCalorie
        '
        Me.rbCalorie.AutoSize = True
        Me.rbCalorie.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbCalorie.Location = New System.Drawing.Point(24, 91)
        Me.rbCalorie.Name = "rbCalorie"
        Me.rbCalorie.Size = New System.Drawing.Size(79, 17)
        Me.rbCalorie.TabIndex = 8
        Me.rbCalorie.Text = "calorie ({0})"
        Me.rbCalorie.UseVisualStyleBackColor = True
        '
        'rbCarbon
        '
        Me.rbCarbon.AutoSize = True
        Me.rbCarbon.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbCarbon.Location = New System.Drawing.Point(24, 114)
        Me.rbCarbon.Name = "rbCarbon"
        Me.rbCarbon.Size = New System.Drawing.Size(81, 17)
        Me.rbCarbon.TabIndex = 9
        Me.rbCarbon.Text = "carbon ({0})"
        Me.rbCarbon.UseVisualStyleBackColor = True
        '
        'rbDryWeight
        '
        Me.rbDryWeight.AutoSize = True
        Me.rbDryWeight.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbDryWeight.Location = New System.Drawing.Point(24, 137)
        Me.rbDryWeight.Name = "rbDryWeight"
        Me.rbDryWeight.Size = New System.Drawing.Size(96, 17)
        Me.rbDryWeight.TabIndex = 10
        Me.rbDryWeight.Text = "dry weight ({0})"
        Me.rbDryWeight.UseVisualStyleBackColor = True
        '
        'm_gbTimeUnits
        '
        Me.m_gbTimeUnits.Controls.Add(Me.txbTimeOther)
        Me.m_gbTimeUnits.Controls.Add(Me.rbTimeOther)
        Me.m_gbTimeUnits.Controls.Add(Me.rbDay)
        Me.m_gbTimeUnits.Controls.Add(Me.rbYear)
        Me.m_gbTimeUnits.Controls.Add(Me.lbNote)
        Me.m_gbTimeUnits.Dock = System.Windows.Forms.DockStyle.Fill
        Me.m_gbTimeUnits.Location = New System.Drawing.Point(357, 3)
        Me.m_gbTimeUnits.Margin = New System.Windows.Forms.Padding(3, 3, 0, 3)
        Me.m_gbTimeUnits.Name = "m_gbTimeUnits"
        Me.m_gbTimeUnits.Size = New System.Drawing.Size(290, 188)
        Me.m_gbTimeUnits.TabIndex = 1
        Me.m_gbTimeUnits.TabStop = False
        Me.m_gbTimeUnits.Text = "&Time units"
        '
        'txbTimeOther
        '
        Me.txbTimeOther.Location = New System.Drawing.Point(93, 67)
        Me.txbTimeOther.Name = "txbTimeOther"
        Me.txbTimeOther.Size = New System.Drawing.Size(75, 20)
        Me.txbTimeOther.TabIndex = 7
        '
        'rbTimeOther
        '
        Me.rbTimeOther.AutoSize = True
        Me.rbTimeOther.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbTimeOther.Location = New System.Drawing.Point(6, 68)
        Me.rbTimeOther.Name = "rbTimeOther"
        Me.rbTimeOther.Size = New System.Drawing.Size(72, 17)
        Me.rbTimeOther.TabIndex = 6
        Me.rbTimeOther.Text = "other unit:"
        Me.rbTimeOther.UseVisualStyleBackColor = True
        '
        'rbDay
        '
        Me.rbDay.AutoSize = True
        Me.rbDay.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbDay.Location = New System.Drawing.Point(6, 45)
        Me.rbDay.Name = "rbDay"
        Me.rbDay.Size = New System.Drawing.Size(42, 17)
        Me.rbDay.TabIndex = 5
        Me.rbDay.Text = "day"
        Me.rbDay.UseVisualStyleBackColor = True
        '
        'rbYear
        '
        Me.rbYear.AutoSize = True
        Me.rbYear.Checked = True
        Me.rbYear.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbYear.Location = New System.Drawing.Point(6, 22)
        Me.rbYear.Name = "rbYear"
        Me.rbYear.Size = New System.Drawing.Size(55, 17)
        Me.rbYear.TabIndex = 4
        Me.rbYear.TabStop = True
        Me.rbYear.Text = "year *)"
        Me.rbYear.UseVisualStyleBackColor = True
        '
        'lbNote
        '
        Me.lbNote.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbNote.Enabled = False
        Me.lbNote.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic)
        Me.lbNote.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lbNote.Location = New System.Drawing.Point(6, 121)
        Me.lbNote.Name = "lbNote"
        Me.lbNote.Size = New System.Drawing.Size(278, 61)
        Me.lbNote.TabIndex = 8
        Me.lbNote.Text = "*) In Ecosim/Ecospace the time unit should be 'year'"
        Me.lbNote.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'm_lblMonetaryUnit
        '
        Me.m_lblMonetaryUnit.AutoSize = True
        Me.m_lblMonetaryUnit.Location = New System.Drawing.Point(12, 385)
        Me.m_lblMonetaryUnit.Name = "m_lblMonetaryUnit"
        Me.m_lblMonetaryUnit.Size = New System.Drawing.Size(74, 13)
        Me.m_lblMonetaryUnit.TabIndex = 17
        Me.m_lblMonetaryUnit.Text = "&Monetary unit:"
        '
        'm_cmbMonetaryUnit
        '
        Me.m_cmbMonetaryUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.m_cmbMonetaryUnit.FormattingEnabled = True
        Me.m_cmbMonetaryUnit.Location = New System.Drawing.Point(184, 382)
        Me.m_cmbMonetaryUnit.Name = "m_cmbMonetaryUnit"
        Me.m_cmbMonetaryUnit.Size = New System.Drawing.Size(179, 21)
        Me.m_cmbMonetaryUnit.Sorted = True
        Me.m_cmbMonetaryUnit.TabIndex = 18
        Me.m_cmbMonetaryUnit.Unit = EwEUtils.Core.eUnitMonetaryType.Custom
        '
        'frmModelDescription
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(671, 633)
        Me.Controls.Add(Me.m_cmbMonetaryUnit)
        Me.Controls.Add(Me.m_lblMonetaryUnit)
        Me.Controls.Add(Me.m_tlpUnits)
        Me.Controls.Add(Me.lblAreaUnit)
        Me.Controls.Add(Me.lblArea)
        Me.Controls.Add(Me.lblPath)
        Me.Controls.Add(Me.m_txbPath)
        Me.Controls.Add(Me.m_tbDescription)
        Me.Controls.Add(Me.m_tbContact)
        Me.Controls.Add(Me.m_tbArea)
        Me.Controls.Add(Me.m_tbAuthor)
        Me.Controls.Add(Me.lblScenario)
        Me.Controls.Add(Me.m_tbName)
        Me.Controls.Add(Me.m_udNumDigits)
        Me.Controls.Add(Me.lbNumDigits)
        Me.Controls.Add(Me.m_lbContact)
        Me.Controls.Add(Me.lblColorHeader)
        Me.Controls.Add(Me.m_lbDescription)
        Me.Controls.Add(Me.lbScenarioName)
        Me.Controls.Add(Me.m_lbAuthor)
        Me.Name = "frmModelDescription"
        Me.Text = "frmModelParameters"
        CType(Me.m_udNumDigits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.m_tlpUnits.ResumeLayout(False)
        Me.m_gbCurrencyUnit.ResumeLayout(False)
        Me.m_gbCurrencyUnit.PerformLayout()
        Me.m_gbTimeUnits.ResumeLayout(False)
        Me.m_gbTimeUnits.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents m_udNumDigits As System.Windows.Forms.NumericUpDown
    Private WithEvents lbNumDigits As System.Windows.Forms.Label
    Private WithEvents lblColorHeader As System.Windows.Forms.Label
    Private WithEvents m_lbDescription As System.Windows.Forms.Label
    Private WithEvents lbScenarioName As System.Windows.Forms.Label
    Private WithEvents lblScenario As System.Windows.Forms.Label
    Private WithEvents m_tbName As System.Windows.Forms.TextBox
    Private WithEvents m_tbContact As System.Windows.Forms.TextBox
    Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
    Private WithEvents m_lbContact As System.Windows.Forms.Label
    Private WithEvents m_lbAuthor As System.Windows.Forms.Label
    Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
    Private WithEvents m_txbPath As System.Windows.Forms.Label
    Private WithEvents lblPath As System.Windows.Forms.Label
    Private WithEvents lblArea As System.Windows.Forms.Label
    Private WithEvents m_tbArea As System.Windows.Forms.TextBox
    Private WithEvents lblAreaUnit As System.Windows.Forms.Label
    Private WithEvents m_tlpUnits As System.Windows.Forms.TableLayoutPanel
    Private WithEvents m_gbCurrencyUnit As System.Windows.Forms.GroupBox
    Private WithEvents Label1 As System.Windows.Forms.Label
    Private WithEvents rbWetWeight As System.Windows.Forms.RadioButton
    Private WithEvents rbJoules As System.Windows.Forms.RadioButton
    Private WithEvents rbCalorie As System.Windows.Forms.RadioButton
    Private WithEvents rbCarbon As System.Windows.Forms.RadioButton
    Private WithEvents rbDryWeight As System.Windows.Forms.RadioButton
    Private WithEvents tbCurrencyEnergyOther As System.Windows.Forms.TextBox
    Private WithEvents rbCurrencyEnergyOther As System.Windows.Forms.RadioButton
    Private WithEvents rbNitrogen As System.Windows.Forms.RadioButton
    Private WithEvents rbPhosporus As System.Windows.Forms.RadioButton
    Private WithEvents Label2 As System.Windows.Forms.Label
    Private WithEvents m_gbTimeUnits As System.Windows.Forms.GroupBox
    Private WithEvents txbTimeOther As System.Windows.Forms.TextBox
    Private WithEvents rbTimeOther As System.Windows.Forms.RadioButton
    Private WithEvents rbDay As System.Windows.Forms.RadioButton
    Private WithEvents rbYear As System.Windows.Forms.RadioButton
    Private WithEvents lbNote As System.Windows.Forms.Label
    Private WithEvents m_lblMonetaryUnit As System.Windows.Forms.Label
    Private WithEvents m_cmbMonetaryUnit As ScientificInterfaceShared.Controls.cMonetaryUnitComboBox
    Private WithEvents rbNutrientOther As System.Windows.Forms.RadioButton
    Friend WithEvents tbCurrencyNutrientOther As System.Windows.Forms.TextBox
End Class
