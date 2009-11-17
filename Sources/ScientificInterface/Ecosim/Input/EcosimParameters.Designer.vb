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
            Me.cmbSalinityForcing = New System.Windows.Forms.ComboBox
            Me.cmbNutForcing = New System.Windows.Forms.ComboBox
            Me.chkPredictEffort = New System.Windows.Forms.CheckBox
            Me.chkRegulatoryFeedbackLoop = New System.Windows.Forms.CheckBox
            Me.chkConTracing = New System.Windows.Forms.CheckBox
            Me.m_lblRelaxation = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label7 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.lblScenario = New System.Windows.Forms.Label
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.lblDescription = New System.Windows.Forms.Label
            Me.lbScenarioName = New System.Windows.Forms.Label
            Me.m_chkUseVarPQ = New System.Windows.Forms.CheckBox
            Me.cmbTempLoading = New System.Windows.Forms.ComboBox
            Me.Label3 = New System.Windows.Forms.Label
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
            'cmbSalinityForcing
            '
            Me.cmbSalinityForcing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbSalinityForcing.FormattingEnabled = True
            resources.ApplyResources(Me.cmbSalinityForcing, "cmbSalinityForcing")
            Me.cmbSalinityForcing.Name = "cmbSalinityForcing"
            '
            'cmbNutForcing
            '
            Me.cmbNutForcing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cmbNutForcing.FormattingEnabled = True
            resources.ApplyResources(Me.cmbNutForcing, "cmbNutForcing")
            Me.cmbNutForcing.Name = "cmbNutForcing"
            '
            'chkPredictEffort
            '
            resources.ApplyResources(Me.chkPredictEffort, "chkPredictEffort")
            Me.chkPredictEffort.Name = "chkPredictEffort"
            Me.chkPredictEffort.UseVisualStyleBackColor = True
            '
            'chkRegulatoryFeedbackLoop
            '
            resources.ApplyResources(Me.chkRegulatoryFeedbackLoop, "chkRegulatoryFeedbackLoop")
            Me.chkRegulatoryFeedbackLoop.Name = "chkRegulatoryFeedbackLoop"
            Me.chkRegulatoryFeedbackLoop.UseVisualStyleBackColor = True
            '
            'chkConTracing
            '
            resources.ApplyResources(Me.chkConTracing, "chkConTracing")
            Me.chkConTracing.Name = "chkConTracing"
            Me.chkConTracing.UseVisualStyleBackColor = True
            '
            'm_lblRelaxation
            '
            resources.ApplyResources(Me.m_lblRelaxation, "m_lblRelaxation")
            Me.m_lblRelaxation.Name = "m_lblRelaxation"
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'Label7
            '
            resources.ApplyResources(Me.Label7, "Label7")
            Me.Label7.Name = "Label7"
            '
            'Label6
            '
            resources.ApplyResources(Me.Label6, "Label6")
            Me.Label6.Name = "Label6"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.Name = "Label1"
            '
            'lblInitializationHeader
            '
            resources.ApplyResources(Me.lblInitializationHeader, "lblInitializationHeader")
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            '
            'lblScenario
            '
            resources.ApplyResources(Me.lblScenario, "lblScenario")
            Me.lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblScenario.Name = "lblScenario"
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
            'lblDescription
            '
            resources.ApplyResources(Me.lblDescription, "lblDescription")
            Me.lblDescription.Name = "lblDescription"
            '
            'lbScenarioName
            '
            resources.ApplyResources(Me.lbScenarioName, "lbScenarioName")
            Me.lbScenarioName.Name = "lbScenarioName"
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
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'EcosimParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.cmbTempLoading)
            Me.Controls.Add(Me.m_nudNutBaseFreeProp)
            Me.Controls.Add(Me.m_tbContact)
            Me.Controls.Add(Me.m_nudNumberYears)
            Me.Controls.Add(Me.m_tbAuthor)
            Me.Controls.Add(Me.m_nudRelaxation)
            Me.Controls.Add(Me.lblScenario)
            Me.Controls.Add(Me.cmbSalinityForcing)
            Me.Controls.Add(Me.m_lbContact)
            Me.Controls.Add(Me.cmbNutForcing)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(Me.chkPredictEffort)
            Me.Controls.Add(Me.m_lbAuthor)
            Me.Controls.Add(Me.chkRegulatoryFeedbackLoop)
            Me.Controls.Add(Me.m_chkUseVarPQ)
            Me.Controls.Add(Me.chkConTracing)
            Me.Controls.Add(Me.m_tbName)
            Me.Controls.Add(Me.m_lblRelaxation)
            Me.Controls.Add(Me.lbScenarioName)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.m_tbDescription)
            Me.Controls.Add(Me.Label7)
            Me.Controls.Add(Me.lblDescription)
            Me.Controls.Add(Me.Label6)
            Me.Controls.Add(Me.Label1)
            Me.Name = "EcosimParameters"
            CType(Me.m_nudNutBaseFreeProp, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumberYears, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRelaxation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents chkConTracing As System.Windows.Forms.CheckBox
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents lblScenario As System.Windows.Forms.Label
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents chkPredictEffort As System.Windows.Forms.CheckBox
        Friend WithEvents cmbNutForcing As System.Windows.Forms.ComboBox
        Friend WithEvents m_tbName As System.Windows.Forms.TextBox
        Friend WithEvents m_tbContact As System.Windows.Forms.TextBox
        Friend WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Friend WithEvents m_lbContact As System.Windows.Forms.Label
        Friend WithEvents m_lbAuthor As System.Windows.Forms.Label
        Friend WithEvents cmbSalinityForcing As System.Windows.Forms.ComboBox
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents m_nudRelaxation As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_lblRelaxation As System.Windows.Forms.Label
        Friend WithEvents m_nudNumberYears As System.Windows.Forms.NumericUpDown
        Friend WithEvents m_nudNutBaseFreeProp As System.Windows.Forms.NumericUpDown
        Private WithEvents chkRegulatoryFeedbackLoop As System.Windows.Forms.CheckBox
        Private WithEvents m_chkUseVarPQ As System.Windows.Forms.CheckBox
        Private WithEvents lbScenarioName As System.Windows.Forms.Label
        Friend WithEvents cmbTempLoading As System.Windows.Forms.ComboBox
        Friend WithEvents Label3 As System.Windows.Forms.Label

    End Class
End Namespace

