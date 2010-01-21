Imports ScientificInterface.Controls
Imports WeifenLuo.WinFormsUI.Docking

Namespace Ecosim
    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated(), CLSCompliant(False)> _
    Partial Class EcosimParameters
        : Inherits frmEwE

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
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcosimParameters))
            Me.m_nudNutBaseFreeProp = New System.Windows.Forms.NumericUpDown
            Me.m_nudNumberYears = New System.Windows.Forms.NumericUpDown
            Me.m_nudRelaxation = New System.Windows.Forms.NumericUpDown
            Me.m_cmbSalinityForcing = New System.Windows.Forms.ComboBox
            Me.m_cmbNutForcing = New System.Windows.Forms.ComboBox
            Me.m_chkPredictEffort = New System.Windows.Forms.CheckBox
            Me.m_chkConTracing = New System.Windows.Forms.CheckBox
            Me.m_lblRelaxation = New System.Windows.Forms.Label
            Me.m_lblSalinityForcing = New System.Windows.Forms.Label
            Me.m_lblNutForcing = New System.Windows.Forms.Label
            Me.m_lblNutBaseFreeProp = New System.Windows.Forms.Label
            Me.m_lblNumberYears = New System.Windows.Forms.Label
            Me.m_lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_lblScenario = New System.Windows.Forms.Label
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lbScenarioName = New System.Windows.Forms.Label
            Me.m_chkUseVarPQ = New System.Windows.Forms.CheckBox
            Me.cmbTempLoading = New System.Windows.Forms.ComboBox
            Me.m_lblTempLoading = New System.Windows.Forms.Label
            CType(Me.m_nudNutBaseFreeProp, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudNumberYears, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRelaxation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.SuspendLayout()
            '
            'm_nudNutBaseFreeProp
            '
            resources.ApplyResources(Me.m_nudNutBaseFreeProp, "m_nudNutBaseFreeProp")
            Me.m_nudNutBaseFreeProp.Name = "m_nudNutBaseFreeProp"
            '
            'm_nudNumberYears
            '
            resources.ApplyResources(Me.m_nudNumberYears, "m_nudNumberYears")
            Me.m_nudNumberYears.Name = "m_nudNumberYears"
            '
            'm_nudRelaxation
            '
            resources.ApplyResources(Me.m_nudRelaxation, "m_nudRelaxation")
            Me.m_nudRelaxation.Name = "m_nudRelaxation"
            '
            'm_cmbSalinityForcing
            '
            Me.m_cmbSalinityForcing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbSalinityForcing.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbSalinityForcing, "m_cmbSalinityForcing")
            Me.m_cmbSalinityForcing.Name = "m_cmbSalinityForcing"
            '
            'm_cmbNutForcing
            '
            Me.m_cmbNutForcing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.m_cmbNutForcing.FormattingEnabled = True
            resources.ApplyResources(Me.m_cmbNutForcing, "m_cmbNutForcing")
            Me.m_cmbNutForcing.Name = "m_cmbNutForcing"
            '
            'm_chkPredictEffort
            '
            resources.ApplyResources(Me.m_chkPredictEffort, "m_chkPredictEffort")
            Me.m_chkPredictEffort.Name = "m_chkPredictEffort"
            Me.m_chkPredictEffort.UseVisualStyleBackColor = True
            '
            'm_chkConTracing
            '
            resources.ApplyResources(Me.m_chkConTracing, "m_chkConTracing")
            Me.m_chkConTracing.Name = "m_chkConTracing"
            Me.m_chkConTracing.UseVisualStyleBackColor = True
            '
            'm_lblRelaxation
            '
            resources.ApplyResources(Me.m_lblRelaxation, "m_lblRelaxation")
            Me.m_lblRelaxation.Name = "m_lblRelaxation"
            '
            'm_lblSalinityForcing
            '
            resources.ApplyResources(Me.m_lblSalinityForcing, "m_lblSalinityForcing")
            Me.m_lblSalinityForcing.Name = "m_lblSalinityForcing"
            '
            'm_lblNutForcing
            '
            resources.ApplyResources(Me.m_lblNutForcing, "m_lblNutForcing")
            Me.m_lblNutForcing.Name = "m_lblNutForcing"
            '
            'm_lblNutBaseFreeProp
            '
            resources.ApplyResources(Me.m_lblNutBaseFreeProp, "m_lblNutBaseFreeProp")
            Me.m_lblNutBaseFreeProp.Name = "m_lblNutBaseFreeProp"
            '
            'm_lblNumberYears
            '
            resources.ApplyResources(Me.m_lblNumberYears, "m_lblNumberYears")
            Me.m_lblNumberYears.Name = "m_lblNumberYears"
            '
            'm_lblInitializationHeader
            '
            resources.ApplyResources(Me.m_lblInitializationHeader, "m_lblInitializationHeader")
            Me.m_lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblInitializationHeader.Name = "m_lblInitializationHeader"
            '
            'm_lblScenario
            '
            resources.ApplyResources(Me.m_lblScenario, "m_lblScenario")
            Me.m_lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblScenario.Name = "m_lblScenario"
            '
            'm_tbContact
            '
            resources.ApplyResources(Me.m_tbContact, "m_tbContact")
            Me.m_tbContact.Name = "m_tbContact"
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
            'm_lbAuthor
            '
            resources.ApplyResources(Me.m_lbAuthor, "m_lbAuthor")
            Me.m_lbAuthor.Name = "m_lbAuthor"
            '
            'm_tbName
            '
            resources.ApplyResources(Me.m_tbName, "m_tbName")
            Me.m_tbName.Name = "m_tbName"
            '
            'm_tbDescription
            '
            resources.ApplyResources(Me.m_tbDescription, "m_tbDescription")
            Me.m_tbDescription.Name = "m_tbDescription"
            '
            'm_lblDescription
            '
            resources.ApplyResources(Me.m_lblDescription, "m_lblDescription")
            Me.m_lblDescription.Name = "m_lblDescription"
            '
            'm_lbScenarioName
            '
            resources.ApplyResources(Me.m_lbScenarioName, "m_lbScenarioName")
            Me.m_lbScenarioName.Name = "m_lbScenarioName"
            '
            'm_chkUseVarPQ
            '
            resources.ApplyResources(Me.m_chkUseVarPQ, "m_chkUseVarPQ")
            Me.m_chkUseVarPQ.Name = "m_chkUseVarPQ"
            Me.m_chkUseVarPQ.UseVisualStyleBackColor = True
            '
            'cmbTempLoading
            '
            Me.cmbTempLoading.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbTempLoading.FormattingEnabled = True
            resources.ApplyResources(Me.cmbTempLoading, "cmbTempLoading")
            Me.cmbTempLoading.Name = "cmbTempLoading"
            '
            'm_lblTempLoading
            '
            resources.ApplyResources(Me.m_lblTempLoading, "m_lblTempLoading")
            Me.m_lblTempLoading.Name = "m_lblTempLoading"
            '
            'EcosimParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_lblTempLoading)
            Me.Controls.Add(Me.cmbTempLoading)
            Me.Controls.Add(Me.m_nudNutBaseFreeProp)
            Me.Controls.Add(Me.m_tbContact)
            Me.Controls.Add(Me.m_nudNumberYears)
            Me.Controls.Add(Me.m_tbAuthor)
            Me.Controls.Add(Me.m_nudRelaxation)
            Me.Controls.Add(Me.m_lblScenario)
            Me.Controls.Add(Me.m_cmbSalinityForcing)
            Me.Controls.Add(Me.m_lbContact)
            Me.Controls.Add(Me.m_cmbNutForcing)
            Me.Controls.Add(Me.m_lblInitializationHeader)
            Me.Controls.Add(Me.m_chkPredictEffort)
            Me.Controls.Add(Me.m_lbAuthor)
            Me.Controls.Add(Me.m_chkUseVarPQ)
            Me.Controls.Add(Me.m_chkConTracing)
            Me.Controls.Add(Me.m_tbName)
            Me.Controls.Add(Me.m_lblRelaxation)
            Me.Controls.Add(Me.m_lbScenarioName)
            Me.Controls.Add(Me.m_lblSalinityForcing)
            Me.Controls.Add(Me.m_tbDescription)
            Me.Controls.Add(Me.m_lblNutForcing)
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_lblNutBaseFreeProp)
            Me.Controls.Add(Me.m_lblNumberYears)
            Me.Name = "EcosimParameters"
            CType(Me.m_nudNutBaseFreeProp, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumberYears, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRelaxation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents m_tbName As System.Windows.Forms.TextBox
        Friend WithEvents m_tbContact As System.Windows.Forms.TextBox
        Friend WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Friend WithEvents m_nudNutBaseFreeProp As System.Windows.Forms.NumericUpDown
        Private WithEvents m_chkUseVarPQ As System.Windows.Forms.CheckBox
        Private WithEvents m_lbScenarioName As System.Windows.Forms.Label
        Private WithEvents m_lbContact As System.Windows.Forms.Label
        Private WithEvents m_lbAuthor As System.Windows.Forms.Label
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents m_lblNumberYears As System.Windows.Forms.Label
        Private WithEvents m_nudNumberYears As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblNutBaseFreeProp As System.Windows.Forms.Label
        Private WithEvents m_cmbNutForcing As System.Windows.Forms.ComboBox
        Private WithEvents m_lblNutForcing As System.Windows.Forms.Label
        Private WithEvents m_cmbSalinityForcing As System.Windows.Forms.ComboBox
        Private WithEvents m_lblSalinityForcing As System.Windows.Forms.Label
        Private WithEvents cmbTempLoading As System.Windows.Forms.ComboBox
        Private WithEvents m_nudRelaxation As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lblRelaxation As System.Windows.Forms.Label
        Private WithEvents m_lblTempLoading As System.Windows.Forms.Label
        Private WithEvents m_chkPredictEffort As System.Windows.Forms.CheckBox
        Private WithEvents m_chkConTracing As System.Windows.Forms.CheckBox
        Private WithEvents m_lblScenario As System.Windows.Forms.Label
        Private WithEvents m_lblInitializationHeader As System.Windows.Forms.Label

    End Class
End Namespace

