
Namespace Ecospace

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EcospaceParameters
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
            Dim gbBiomass As System.Windows.Forms.GroupBox
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcospaceParameters))
            Dim gbModel As System.Windows.Forms.GroupBox
            Me.rbBaseBiomass = New System.Windows.Forms.RadioButton
            Me.rbAdjustedBiomass = New System.Windows.Forms.RadioButton
            Me.rbNewStanzaModel = New System.Windows.Forms.RadioButton
            Me.rbIBM = New System.Windows.Forms.RadioButton
            Me.rbOldSchool = New System.Windows.Forms.RadioButton
            Me.cbPredictEffort = New System.Windows.Forms.CheckBox
            Me.lbNumThreads = New System.Windows.Forms.Label
            Me.udNumThreads = New System.Windows.Forms.NumericUpDown
            Me.lbPacketsMultiplier = New System.Windows.Forms.Label
            Me.lblInitializationHeader = New System.Windows.Forms.Label
            Me.Label1 = New System.Windows.Forms.Label
            Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
            Me.gbThreading = New System.Windows.Forms.GroupBox
            Me.tbNumPackets = New System.Windows.Forms.TextBox
            Me.udMaxIterations = New System.Windows.Forms.NumericUpDown
            Me.lbTotalTime = New System.Windows.Forms.Label
            Me.Label3 = New System.Windows.Forms.Label
            Me.lbNumIterations = New System.Windows.Forms.Label
            Me.lbTolerance = New System.Windows.Forms.Label
            Me.lbSOR = New System.Windows.Forms.Label
            Me.tbTotalTime = New System.Windows.Forms.TextBox
            Me.tbTimeStepsPerYear = New System.Windows.Forms.TextBox
            Me.tbTolerance = New System.Windows.Forms.TextBox
            Me.tbSOR = New System.Windows.Forms.TextBox
            Me.gbRunTime = New System.Windows.Forms.GroupBox
            Me.cbContaminantTracing = New System.Windows.Forms.CheckBox
            Me.cbUseExact = New System.Windows.Forms.CheckBox
            Me.gbDetails = New System.Windows.Forms.GroupBox
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.lblDescription = New System.Windows.Forms.Label
            Me.lbScenarioName = New System.Windows.Forms.Label
            Me.lblScenario = New System.Windows.Forms.Label
            gbBiomass = New System.Windows.Forms.GroupBox
            gbModel = New System.Windows.Forms.GroupBox
            gbBiomass.SuspendLayout()
            gbModel.SuspendLayout()
            CType(Me.udNumThreads, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.TableLayoutPanel1.SuspendLayout()
            Me.gbThreading.SuspendLayout()
            CType(Me.udMaxIterations, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.gbRunTime.SuspendLayout()
            Me.gbDetails.SuspendLayout()
            Me.SuspendLayout()
            '
            'gbBiomass
            '
            resources.ApplyResources(gbBiomass, "gbBiomass")
            gbBiomass.Controls.Add(Me.rbBaseBiomass)
            gbBiomass.Controls.Add(Me.rbAdjustedBiomass)
            gbBiomass.Name = "gbBiomass"
            gbBiomass.TabStop = False
            '
            'rbBaseBiomass
            '
            resources.ApplyResources(Me.rbBaseBiomass, "rbBaseBiomass")
            Me.rbBaseBiomass.Checked = True
            Me.rbBaseBiomass.Name = "rbBaseBiomass"
            Me.rbBaseBiomass.TabStop = True
            Me.rbBaseBiomass.UseVisualStyleBackColor = True
            '
            'rbAdjustedBiomass
            '
            resources.ApplyResources(Me.rbAdjustedBiomass, "rbAdjustedBiomass")
            Me.rbAdjustedBiomass.Name = "rbAdjustedBiomass"
            Me.rbAdjustedBiomass.UseVisualStyleBackColor = True
            '
            'gbModel
            '
            resources.ApplyResources(gbModel, "gbModel")
            gbModel.Controls.Add(Me.rbNewStanzaModel)
            gbModel.Controls.Add(Me.rbIBM)
            gbModel.Controls.Add(Me.rbOldSchool)
            gbModel.Name = "gbModel"
            gbModel.TabStop = False
            '
            'rbNewStanzaModel
            '
            resources.ApplyResources(Me.rbNewStanzaModel, "rbNewStanzaModel")
            Me.rbNewStanzaModel.Checked = True
            Me.rbNewStanzaModel.Name = "rbNewStanzaModel"
            Me.rbNewStanzaModel.TabStop = True
            Me.rbNewStanzaModel.UseVisualStyleBackColor = True
            '
            'rbIBM
            '
            resources.ApplyResources(Me.rbIBM, "rbIBM")
            Me.rbIBM.Name = "rbIBM"
            Me.rbIBM.UseVisualStyleBackColor = True
            '
            'rbOldSchool
            '
            resources.ApplyResources(Me.rbOldSchool, "rbOldSchool")
            Me.rbOldSchool.Name = "rbOldSchool"
            Me.rbOldSchool.UseVisualStyleBackColor = True
            '
            'cbPredictEffort
            '
            resources.ApplyResources(Me.cbPredictEffort, "cbPredictEffort")
            Me.cbPredictEffort.Name = "cbPredictEffort"
            Me.cbPredictEffort.UseVisualStyleBackColor = True
            '
            'lbNumThreads
            '
            resources.ApplyResources(Me.lbNumThreads, "lbNumThreads")
            Me.lbNumThreads.Name = "lbNumThreads"
            '
            'udNumThreads
            '
            resources.ApplyResources(Me.udNumThreads, "udNumThreads")
            Me.udNumThreads.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.udNumThreads.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.udNumThreads.Name = "udNumThreads"
            Me.udNumThreads.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'lbPacketsMultiplier
            '
            resources.ApplyResources(Me.lbPacketsMultiplier, "lbPacketsMultiplier")
            Me.lbPacketsMultiplier.Name = "lbPacketsMultiplier"
            '
            'lblInitializationHeader
            '
            resources.ApplyResources(Me.lblInitializationHeader, "lblInitializationHeader")
            Me.lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblInitializationHeader.Name = "lblInitializationHeader"
            '
            'Label1
            '
            resources.ApplyResources(Me.Label1, "Label1")
            Me.Label1.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.Label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.Label1.Name = "Label1"
            '
            'TableLayoutPanel1
            '
            resources.ApplyResources(Me.TableLayoutPanel1, "TableLayoutPanel1")
            Me.TableLayoutPanel1.Controls.Add(Me.gbThreading, 1, 0)
            Me.TableLayoutPanel1.Controls.Add(gbModel, 0, 0)
            Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
            '
            'gbThreading
            '
            resources.ApplyResources(Me.gbThreading, "gbThreading")
            Me.gbThreading.Controls.Add(Me.tbNumPackets)
            Me.gbThreading.Controls.Add(Me.lbNumThreads)
            Me.gbThreading.Controls.Add(Me.udNumThreads)
            Me.gbThreading.Controls.Add(Me.lbPacketsMultiplier)
            Me.gbThreading.Name = "gbThreading"
            Me.gbThreading.TabStop = False
            '
            'tbNumPackets
            '
            resources.ApplyResources(Me.tbNumPackets, "tbNumPackets")
            Me.tbNumPackets.Name = "tbNumPackets"
            '
            'udMaxIterations
            '
            resources.ApplyResources(Me.udMaxIterations, "udMaxIterations")
            Me.udMaxIterations.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.udMaxIterations.Name = "udMaxIterations"
            Me.udMaxIterations.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'lbTotalTime
            '
            resources.ApplyResources(Me.lbTotalTime, "lbTotalTime")
            Me.lbTotalTime.Name = "lbTotalTime"
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'lbNumIterations
            '
            resources.ApplyResources(Me.lbNumIterations, "lbNumIterations")
            Me.lbNumIterations.Name = "lbNumIterations"
            '
            'lbTolerance
            '
            resources.ApplyResources(Me.lbTolerance, "lbTolerance")
            Me.lbTolerance.Name = "lbTolerance"
            '
            'lbSOR
            '
            resources.ApplyResources(Me.lbSOR, "lbSOR")
            Me.lbSOR.Name = "lbSOR"
            '
            'tbTotalTime
            '
            resources.ApplyResources(Me.tbTotalTime, "tbTotalTime")
            Me.tbTotalTime.Name = "tbTotalTime"
            '
            'tbTimeStepsPerYear
            '
            resources.ApplyResources(Me.tbTimeStepsPerYear, "tbTimeStepsPerYear")
            Me.tbTimeStepsPerYear.Name = "tbTimeStepsPerYear"
            '
            'tbTolerance
            '
            resources.ApplyResources(Me.tbTolerance, "tbTolerance")
            Me.tbTolerance.Name = "tbTolerance"
            '
            'tbSOR
            '
            resources.ApplyResources(Me.tbSOR, "tbSOR")
            Me.tbSOR.Name = "tbSOR"
            '
            'gbRunTime
            '
            resources.ApplyResources(Me.gbRunTime, "gbRunTime")
            Me.gbRunTime.Controls.Add(Me.tbSOR)
            Me.gbRunTime.Controls.Add(Me.tbTolerance)
            Me.gbRunTime.Controls.Add(Me.tbTimeStepsPerYear)
            Me.gbRunTime.Controls.Add(Me.tbTotalTime)
            Me.gbRunTime.Controls.Add(Me.cbContaminantTracing)
            Me.gbRunTime.Controls.Add(Me.cbUseExact)
            Me.gbRunTime.Controls.Add(Me.cbPredictEffort)
            Me.gbRunTime.Controls.Add(Me.lbSOR)
            Me.gbRunTime.Controls.Add(Me.lbTolerance)
            Me.gbRunTime.Controls.Add(Me.lbNumIterations)
            Me.gbRunTime.Controls.Add(Me.Label3)
            Me.gbRunTime.Controls.Add(Me.lbTotalTime)
            Me.gbRunTime.Controls.Add(Me.udMaxIterations)
            Me.gbRunTime.Name = "gbRunTime"
            Me.gbRunTime.TabStop = False
            '
            'cbContaminantTracing
            '
            resources.ApplyResources(Me.cbContaminantTracing, "cbContaminantTracing")
            Me.cbContaminantTracing.Name = "cbContaminantTracing"
            Me.cbContaminantTracing.UseVisualStyleBackColor = True
            '
            'cbUseExact
            '
            resources.ApplyResources(Me.cbUseExact, "cbUseExact")
            Me.cbUseExact.Name = "cbUseExact"
            Me.cbUseExact.UseVisualStyleBackColor = True
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
            'lblScenario
            '
            resources.ApplyResources(Me.lblScenario, "lblScenario")
            Me.lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.lblScenario.Name = "lblScenario"
            '
            'EcospaceParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.gbDetails)
            Me.Controls.Add(Me.lblScenario)
            Me.Controls.Add(Me.gbRunTime)
            Me.Controls.Add(Me.TableLayoutPanel1)
            Me.Controls.Add(Me.Label1)
            Me.Controls.Add(Me.lblInitializationHeader)
            Me.Controls.Add(gbBiomass)
            Me.Name = "EcospaceParameters"
            gbBiomass.ResumeLayout(False)
            gbBiomass.PerformLayout()
            gbModel.ResumeLayout(False)
            gbModel.PerformLayout()
            CType(Me.udNumThreads, System.ComponentModel.ISupportInitialize).EndInit()
            Me.TableLayoutPanel1.ResumeLayout(False)
            Me.gbThreading.ResumeLayout(False)
            Me.gbThreading.PerformLayout()
            CType(Me.udMaxIterations, System.ComponentModel.ISupportInitialize).EndInit()
            Me.gbRunTime.ResumeLayout(False)
            Me.gbRunTime.PerformLayout()
            Me.gbDetails.ResumeLayout(False)
            Me.gbDetails.PerformLayout()
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents rbNewStanzaModel As System.Windows.Forms.RadioButton
        Friend WithEvents rbIBM As System.Windows.Forms.RadioButton
        Friend WithEvents lbNumThreads As System.Windows.Forms.Label
        Friend WithEvents udNumThreads As System.Windows.Forms.NumericUpDown
        Friend WithEvents lbPacketsMultiplier As System.Windows.Forms.Label
        Friend WithEvents rbOldSchool As System.Windows.Forms.RadioButton
        Friend WithEvents lblInitializationHeader As System.Windows.Forms.Label
        Friend WithEvents Label1 As System.Windows.Forms.Label
        Friend WithEvents rbBaseBiomass As System.Windows.Forms.RadioButton
        Friend WithEvents rbAdjustedBiomass As System.Windows.Forms.RadioButton
        Friend WithEvents cbPredictEffort As System.Windows.Forms.CheckBox
        Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents gbThreading As System.Windows.Forms.GroupBox
        Friend WithEvents tbNumPackets As System.Windows.Forms.TextBox
        Friend WithEvents udMaxIterations As System.Windows.Forms.NumericUpDown
        Friend WithEvents lbTotalTime As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents lbNumIterations As System.Windows.Forms.Label
        Friend WithEvents lbTolerance As System.Windows.Forms.Label
        Friend WithEvents lbSOR As System.Windows.Forms.Label
        Friend WithEvents tbTotalTime As System.Windows.Forms.TextBox
        Friend WithEvents tbTimeStepsPerYear As System.Windows.Forms.TextBox
        Friend WithEvents tbTolerance As System.Windows.Forms.TextBox
        Friend WithEvents tbSOR As System.Windows.Forms.TextBox
        Friend WithEvents gbRunTime As System.Windows.Forms.GroupBox
        Friend WithEvents cbUseExact As System.Windows.Forms.CheckBox
        Friend WithEvents cbContaminantTracing As System.Windows.Forms.CheckBox
        Friend WithEvents gbDetails As System.Windows.Forms.GroupBox
        Friend WithEvents m_tbContact As System.Windows.Forms.TextBox
        Friend WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Friend WithEvents m_lbContact As System.Windows.Forms.Label
        Friend WithEvents m_lbAuthor As System.Windows.Forms.Label
        Friend WithEvents m_tbName As System.Windows.Forms.TextBox
        Friend WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Friend WithEvents lblDescription As System.Windows.Forms.Label
        Friend WithEvents lbScenarioName As System.Windows.Forms.Label
        Friend WithEvents lblScenario As System.Windows.Forms.Label
    End Class

End Namespace
