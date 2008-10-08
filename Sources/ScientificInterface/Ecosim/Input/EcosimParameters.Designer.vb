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
            Me.gpbBasicParams = New System.Windows.Forms.GroupBox
            Me.m_nudNutBaseFreeProp = New System.Windows.Forms.NumericUpDown
            Me.m_nudNumberYears = New System.Windows.Forms.NumericUpDown
            Me.m_nudRelaxation = New System.Windows.Forms.NumericUpDown
            Me.cmbSalinityForcing = New System.Windows.Forms.ComboBox
            Me.cmbNutForcing = New System.Windows.Forms.ComboBox
            Me.chkPredictEffort = New System.Windows.Forms.CheckBox
            Me.chkConTracing = New System.Windows.Forms.CheckBox
            Me.m_lblRelaxation = New System.Windows.Forms.Label
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label7 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.lblScenario = New System.Windows.Forms.Label
            Me.gbDetails = New System.Windows.Forms.GroupBox
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.lblDescription = New System.Windows.Forms.Label
            Me.lbScenarioName = New System.Windows.Forms.Label
            Me.chkRegulatoryFeedbackLoop = New System.Windows.Forms.CheckBox
            Me.gpbBasicParams.SuspendLayout()
            CType(Me.m_nudNutBaseFreeProp, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudNumberYears, System.ComponentModel.ISupportInitialize).BeginInit()
            CType(Me.m_nudRelaxation, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gbDetails.SuspendLayout()
            Me.SuspendLayout()
            '
            'gpbBasicParams
            '
            resources.ApplyResources(Me.gpbBasicParams, "gpbBasicParams")
            Me.gpbBasicParams.Controls.Add(Me.m_nudNutBaseFreeProp)
            Me.gpbBasicParams.Controls.Add(Me.m_nudNumberYears)
            Me.gpbBasicParams.Controls.Add(Me.m_nudRelaxation)
            Me.gpbBasicParams.Controls.Add(Me.cmbSalinityForcing)
            Me.gpbBasicParams.Controls.Add(Me.cmbNutForcing)
            Me.gpbBasicParams.Controls.Add(Me.chkPredictEffort)
            Me.gpbBasicParams.Controls.Add(Me.chkRegulatoryFeedbackLoop)
            Me.gpbBasicParams.Controls.Add(Me.chkConTracing)
            Me.gpbBasicParams.Controls.Add(Me.m_lblRelaxation)
            Me.gpbBasicParams.Controls.Add(Me.Label2)
            Me.gpbBasicParams.Controls.Add(Me.Label7)
            Me.gpbBasicParams.Controls.Add(Me.Label6)
            Me.gpbBasicParams.Controls.Add(Me.Label1)
            Me.gpbBasicParams.Name = "gpbBasicParams"
            Me.gpbBasicParams.TabStop = False
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
            'gbDetails
            '
            resources.ApplyResources(Me.gbDetails, "gbDetails")
            Me.gbDetails.Controls.Add(Me.m_tbContact)
            Me.gbDetails.Controls.Add(Me.m_tbAuthor)
            Me.gbDetails.Controls.Add(Me.m_lbContact)
            Me.gbDetails.Controls.Add(Me.m_lbAuthor)
            Me.gbDetails.Controls.Add(Me.m_tbName)
            Me.gbDetails.Controls.Add(Me.m_tbDescription)
            Me.gbDetails.Controls.Add(Me.lblDescription)
            Me.gbDetails.Controls.Add(Me.lbScenarioName)
            Me.gbDetails.Name = "gbDetails"
            Me.gbDetails.TabStop = False
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
            'chkRegulatoryFeedbackLoop
            '
            resources.ApplyResources(Me.chkRegulatoryFeedbackLoop, "chkRegulatoryFeedbackLoop")
            Me.chkRegulatoryFeedbackLoop.Name = "chkRegulatoryFeedbackLoop"
            Me.chkRegulatoryFeedbackLoop.UseVisualStyleBackColor = True
            '
            'EcosimParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gbDetails)
            Me.Controls.Add(Me.lblScenario)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(Me.gpbBasicParams)
            Me.Name = "EcosimParameters"
            Me.gpbBasicParams.ResumeLayout(False)
            Me.gpbBasicParams.PerformLayout()
            CType(Me.m_nudNutBaseFreeProp, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudNumberYears, System.ComponentModel.ISupportInitialize).EndInit()
            CType(Me.m_nudRelaxation, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gbDetails.ResumeLayout(False)
            Me.gbDetails.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents gpbBasicParams As System.Windows.Forms.GroupBox
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents chkConTracing As System.Windows.Forms.CheckBox
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents lblScenario As System.Windows.Forms.Label
        Friend WithEvents gbDetails As System.Windows.Forms.GroupBox
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents lbScenarioName As System.Windows.Forms.Label
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

    End Class
End Namespace

