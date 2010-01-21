
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
            Dim m_gbModel As System.Windows.Forms.GroupBox
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EcospaceParameters))
            Me.m_rbNewStanzaModel = New System.Windows.Forms.RadioButton
            Me.m_rbIBM = New System.Windows.Forms.RadioButton
            Me.m_rbOldSchool = New System.Windows.Forms.RadioButton
            Me.m_rbBaseBiomass = New System.Windows.Forms.RadioButton
            Me.m_rbAdjustedBiomass = New System.Windows.Forms.RadioButton
            Me.m_cbPredictEffort = New System.Windows.Forms.CheckBox
            Me.m_lbNumThreads = New System.Windows.Forms.Label
            Me.m_nudNumThreads = New System.Windows.Forms.NumericUpDown
            Me.lbPacketsMultiplier = New System.Windows.Forms.Label
            Me.m_lblInitializationHeader = New System.Windows.Forms.Label
            Me.m_lblModelHeader = New System.Windows.Forms.Label
            Me.m_tlpModelTop = New System.Windows.Forms.TableLayoutPanel
            Me.m_gbThreading = New System.Windows.Forms.GroupBox
            Me.m_tbNumPackets = New System.Windows.Forms.TextBox
            Me.m_nudMaxIterations = New System.Windows.Forms.NumericUpDown
            Me.m_lbTotalTime = New System.Windows.Forms.Label
            Me.m_lblNumTimstepsPerYear = New System.Windows.Forms.Label
            Me.m_lbNumIterations = New System.Windows.Forms.Label
            Me.m_lbTolerance = New System.Windows.Forms.Label
            Me.m_lbSOR = New System.Windows.Forms.Label
            Me.m_tbTotalTime = New System.Windows.Forms.TextBox
            Me.m_tbNumTimeStepsPerYear = New System.Windows.Forms.TextBox
            Me.m_tbTolerance = New System.Windows.Forms.TextBox
            Me.m_tbSOR = New System.Windows.Forms.TextBox
            Me.m_gbRunTime = New System.Windows.Forms.GroupBox
            Me.m_cbContaminantTracing = New System.Windows.Forms.CheckBox
            Me.m_cbUseExact = New System.Windows.Forms.CheckBox
            Me.m_tbContact = New System.Windows.Forms.TextBox
            Me.m_tbAuthor = New System.Windows.Forms.TextBox
            Me.m_lbContact = New System.Windows.Forms.Label
            Me.m_lbAuthor = New System.Windows.Forms.Label
            Me.m_tbName = New System.Windows.Forms.TextBox
            Me.m_tbDescription = New System.Windows.Forms.TextBox
            Me.m_lblDescription = New System.Windows.Forms.Label
            Me.m_lbScenarioName = New System.Windows.Forms.Label
            Me.m_lblScenario = New System.Windows.Forms.Label
            Me.m_plBiomass = New System.Windows.Forms.Panel
            m_gbModel = New System.Windows.Forms.GroupBox
            m_gbModel.SuspendLayout()
            CType(Me.m_nudNumThreads, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_tlpModelTop.SuspendLayout()
            Me.m_gbThreading.SuspendLayout()
            CType(Me.m_nudMaxIterations, System.ComponentModel.ISupportInitialize).BeginInit()
            Me.m_gbRunTime.SuspendLayout()
            Me.m_plBiomass.SuspendLayout()
            Me.SuspendLayout()
            '
            'm_gbModel
            '
            resources.ApplyResources(m_gbModel, "m_gbModel")
            m_gbModel.Controls.Add(Me.m_rbNewStanzaModel)
            m_gbModel.Controls.Add(Me.m_rbIBM)
            m_gbModel.Controls.Add(Me.m_rbOldSchool)
            m_gbModel.Name = "m_gbModel"
            m_gbModel.TabStop = False
            '
            'm_rbNewStanzaModel
            '
            resources.ApplyResources(Me.m_rbNewStanzaModel, "m_rbNewStanzaModel")
            Me.m_rbNewStanzaModel.Checked = True
            Me.m_rbNewStanzaModel.Name = "m_rbNewStanzaModel"
            Me.m_rbNewStanzaModel.TabStop = True
            Me.m_rbNewStanzaModel.UseVisualStyleBackColor = True
            '
            'm_rbIBM
            '
            resources.ApplyResources(Me.m_rbIBM, "m_rbIBM")
            Me.m_rbIBM.Name = "m_rbIBM"
            Me.m_rbIBM.UseVisualStyleBackColor = True
            '
            'm_rbOldSchool
            '
            resources.ApplyResources(Me.m_rbOldSchool, "m_rbOldSchool")
            Me.m_rbOldSchool.Name = "m_rbOldSchool"
            Me.m_rbOldSchool.UseVisualStyleBackColor = True
            '
            'm_rbBaseBiomass
            '
            resources.ApplyResources(Me.m_rbBaseBiomass, "m_rbBaseBiomass")
            Me.m_rbBaseBiomass.Checked = True
            Me.m_rbBaseBiomass.Name = "m_rbBaseBiomass"
            Me.m_rbBaseBiomass.TabStop = True
            Me.m_rbBaseBiomass.UseVisualStyleBackColor = True
            '
            'm_rbAdjustedBiomass
            '
            resources.ApplyResources(Me.m_rbAdjustedBiomass, "m_rbAdjustedBiomass")
            Me.m_rbAdjustedBiomass.Name = "m_rbAdjustedBiomass"
            Me.m_rbAdjustedBiomass.UseVisualStyleBackColor = True
            '
            'm_cbPredictEffort
            '
            resources.ApplyResources(Me.m_cbPredictEffort, "m_cbPredictEffort")
            Me.m_cbPredictEffort.Name = "m_cbPredictEffort"
            Me.m_cbPredictEffort.UseVisualStyleBackColor = True
            '
            'm_lbNumThreads
            '
            resources.ApplyResources(Me.m_lbNumThreads, "m_lbNumThreads")
            Me.m_lbNumThreads.Name = "m_lbNumThreads"
            '
            'm_nudNumThreads
            '
            resources.ApplyResources(Me.m_nudNumThreads, "m_nudNumThreads")
            Me.m_nudNumThreads.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
            Me.m_nudNumThreads.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
            Me.m_nudNumThreads.Name = "m_nudNumThreads"
            Me.m_nudNumThreads.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'lbPacketsMultiplier
            '
            resources.ApplyResources(Me.lbPacketsMultiplier, "lbPacketsMultiplier")
            Me.lbPacketsMultiplier.Name = "lbPacketsMultiplier"
            '
            'm_lblInitializationHeader
            '
            resources.ApplyResources(Me.m_lblInitializationHeader, "m_lblInitializationHeader")
            Me.m_lblInitializationHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblInitializationHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblInitializationHeader.Name = "m_lblInitializationHeader"
            '
            'm_lblModelHeader
            '
            resources.ApplyResources(Me.m_lblModelHeader, "m_lblModelHeader")
            Me.m_lblModelHeader.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblModelHeader.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblModelHeader.Name = "m_lblModelHeader"
            '
            'm_tlpModelTop
            '
            resources.ApplyResources(Me.m_tlpModelTop, "m_tlpModelTop")
            Me.m_tlpModelTop.Controls.Add(Me.m_gbThreading, 1, 0)
            Me.m_tlpModelTop.Controls.Add(m_gbModel, 0, 0)
            Me.m_tlpModelTop.Name = "m_tlpModelTop"
            '
            'm_gbThreading
            '
            resources.ApplyResources(Me.m_gbThreading, "m_gbThreading")
            Me.m_gbThreading.Controls.Add(Me.m_tbNumPackets)
            Me.m_gbThreading.Controls.Add(Me.m_lbNumThreads)
            Me.m_gbThreading.Controls.Add(Me.m_nudNumThreads)
            Me.m_gbThreading.Controls.Add(Me.lbPacketsMultiplier)
            Me.m_gbThreading.Name = "m_gbThreading"
            Me.m_gbThreading.TabStop = False
            '
            'm_tbNumPackets
            '
            resources.ApplyResources(Me.m_tbNumPackets, "m_tbNumPackets")
            Me.m_tbNumPackets.Name = "m_tbNumPackets"
            '
            'm_nudMaxIterations
            '
            resources.ApplyResources(Me.m_nudMaxIterations, "m_nudMaxIterations")
            Me.m_nudMaxIterations.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
            Me.m_nudMaxIterations.Name = "m_nudMaxIterations"
            Me.m_nudMaxIterations.Value = New Decimal(New Integer() {1, 0, 0, 0})
            '
            'm_lbTotalTime
            '
            resources.ApplyResources(Me.m_lbTotalTime, "m_lbTotalTime")
            Me.m_lbTotalTime.Name = "m_lbTotalTime"
            '
            'm_lblNumTimstepsPerYear
            '
            resources.ApplyResources(Me.m_lblNumTimstepsPerYear, "m_lblNumTimstepsPerYear")
            Me.m_lblNumTimstepsPerYear.Name = "m_lblNumTimstepsPerYear"
            '
            'm_lbNumIterations
            '
            resources.ApplyResources(Me.m_lbNumIterations, "m_lbNumIterations")
            Me.m_lbNumIterations.Name = "m_lbNumIterations"
            '
            'm_lbTolerance
            '
            resources.ApplyResources(Me.m_lbTolerance, "m_lbTolerance")
            Me.m_lbTolerance.Name = "m_lbTolerance"
            '
            'm_lbSOR
            '
            resources.ApplyResources(Me.m_lbSOR, "m_lbSOR")
            Me.m_lbSOR.Name = "m_lbSOR"
            '
            'm_tbTotalTime
            '
            resources.ApplyResources(Me.m_tbTotalTime, "m_tbTotalTime")
            Me.m_tbTotalTime.Name = "m_tbTotalTime"
            '
            'm_tbNumTimeStepsPerYear
            '
            resources.ApplyResources(Me.m_tbNumTimeStepsPerYear, "m_tbNumTimeStepsPerYear")
            Me.m_tbNumTimeStepsPerYear.Name = "m_tbNumTimeStepsPerYear"
            '
            'm_tbTolerance
            '
            resources.ApplyResources(Me.m_tbTolerance, "m_tbTolerance")
            Me.m_tbTolerance.Name = "m_tbTolerance"
            '
            'm_tbSOR
            '
            resources.ApplyResources(Me.m_tbSOR, "m_tbSOR")
            Me.m_tbSOR.Name = "m_tbSOR"
            '
            'm_gbRunTime
            '
            resources.ApplyResources(Me.m_gbRunTime, "m_gbRunTime")
            Me.m_gbRunTime.Controls.Add(Me.m_tbSOR)
            Me.m_gbRunTime.Controls.Add(Me.m_tbTolerance)
            Me.m_gbRunTime.Controls.Add(Me.m_tbNumTimeStepsPerYear)
            Me.m_gbRunTime.Controls.Add(Me.m_tbTotalTime)
            Me.m_gbRunTime.Controls.Add(Me.m_cbContaminantTracing)
            Me.m_gbRunTime.Controls.Add(Me.m_cbUseExact)
            Me.m_gbRunTime.Controls.Add(Me.m_cbPredictEffort)
            Me.m_gbRunTime.Controls.Add(Me.m_lbSOR)
            Me.m_gbRunTime.Controls.Add(Me.m_lbTolerance)
            Me.m_gbRunTime.Controls.Add(Me.m_lbNumIterations)
            Me.m_gbRunTime.Controls.Add(Me.m_lblNumTimstepsPerYear)
            Me.m_gbRunTime.Controls.Add(Me.m_lbTotalTime)
            Me.m_gbRunTime.Controls.Add(Me.m_nudMaxIterations)
            Me.m_gbRunTime.Name = "m_gbRunTime"
            Me.m_gbRunTime.TabStop = False
            '
            'm_cbContaminantTracing
            '
            resources.ApplyResources(Me.m_cbContaminantTracing, "m_cbContaminantTracing")
            Me.m_cbContaminantTracing.Name = "m_cbContaminantTracing"
            Me.m_cbContaminantTracing.UseVisualStyleBackColor = True
            '
            'm_cbUseExact
            '
            resources.ApplyResources(Me.m_cbUseExact, "m_cbUseExact")
            Me.m_cbUseExact.Name = "m_cbUseExact"
            Me.m_cbUseExact.UseVisualStyleBackColor = True
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
            'm_lblScenario
            '
            resources.ApplyResources(Me.m_lblScenario, "m_lblScenario")
            Me.m_lblScenario.BackColor = System.Drawing.SystemColors.ButtonShadow
            Me.m_lblScenario.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
            Me.m_lblScenario.Name = "m_lblScenario"
            '
            'm_plBiomass
            '
            resources.ApplyResources(Me.m_plBiomass, "m_plBiomass")
            Me.m_plBiomass.Controls.Add(Me.m_rbBaseBiomass)
            Me.m_plBiomass.Controls.Add(Me.m_rbAdjustedBiomass)
            Me.m_plBiomass.Name = "m_plBiomass"
            '
            'EcospaceParameters
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.m_plBiomass)
            Me.Controls.Add(Me.m_tbContact)
            Me.Controls.Add(Me.m_tbAuthor)
            Me.Controls.Add(Me.m_lblScenario)
            Me.Controls.Add(Me.m_lbContact)
            Me.Controls.Add(Me.m_gbRunTime)
            Me.Controls.Add(Me.m_lbAuthor)
            Me.Controls.Add(Me.m_tlpModelTop)
            Me.Controls.Add(Me.m_tbName)
            Me.Controls.Add(Me.m_lblModelHeader)
            Me.Controls.Add(Me.m_tbDescription)
            Me.Controls.Add(Me.m_lblInitializationHeader)
            Me.Controls.Add(Me.m_lblDescription)
            Me.Controls.Add(Me.m_lbScenarioName)
            Me.Name = "EcospaceParameters"
            m_gbModel.ResumeLayout(False)
            m_gbModel.PerformLayout()
            CType(Me.m_nudNumThreads, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_tlpModelTop.ResumeLayout(False)
            Me.m_gbThreading.ResumeLayout(False)
            Me.m_gbThreading.PerformLayout()
            CType(Me.m_nudMaxIterations, System.ComponentModel.ISupportInitialize).EndInit()
            Me.m_gbRunTime.ResumeLayout(False)
            Me.m_gbRunTime.PerformLayout()
            Me.m_plBiomass.ResumeLayout(False)
            Me.m_plBiomass.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents lbPacketsMultiplier As System.Windows.Forms.Label
        Private WithEvents m_plBiomass As System.Windows.Forms.Panel
        Private WithEvents m_lbScenarioName As System.Windows.Forms.Label
        Private WithEvents m_lblScenario As System.Windows.Forms.Label
        Private WithEvents m_tbName As System.Windows.Forms.TextBox
        Private WithEvents m_tbDescription As System.Windows.Forms.TextBox
        Private WithEvents m_tbContact As System.Windows.Forms.TextBox
        Private WithEvents m_tbAuthor As System.Windows.Forms.TextBox
        Private WithEvents m_lbContact As System.Windows.Forms.Label
        Private WithEvents m_lbAuthor As System.Windows.Forms.Label
        Private WithEvents m_lblDescription As System.Windows.Forms.Label
        Private WithEvents m_lblInitializationHeader As System.Windows.Forms.Label
        Private WithEvents m_rbBaseBiomass As System.Windows.Forms.RadioButton
        Private WithEvents m_rbAdjustedBiomass As System.Windows.Forms.RadioButton
        Private WithEvents m_lblModelHeader As System.Windows.Forms.Label
        Private WithEvents m_tlpModelTop As System.Windows.Forms.TableLayoutPanel
        Private WithEvents m_gbThreading As System.Windows.Forms.GroupBox
        Private WithEvents m_rbNewStanzaModel As System.Windows.Forms.RadioButton
        Private WithEvents m_rbIBM As System.Windows.Forms.RadioButton
        Private WithEvents m_rbOldSchool As System.Windows.Forms.RadioButton
        Private WithEvents m_lbNumThreads As System.Windows.Forms.Label
        Private WithEvents m_nudNumThreads As System.Windows.Forms.NumericUpDown
        Private WithEvents m_tbNumPackets As System.Windows.Forms.TextBox
        Private WithEvents m_gbRunTime As System.Windows.Forms.GroupBox
        Private WithEvents m_lbTotalTime As System.Windows.Forms.Label
        Private WithEvents m_tbTotalTime As System.Windows.Forms.TextBox
        Private WithEvents m_lblNumTimstepsPerYear As System.Windows.Forms.Label
        Private WithEvents m_tbNumTimeStepsPerYear As System.Windows.Forms.TextBox
        Private WithEvents m_lbNumIterations As System.Windows.Forms.Label
        Private WithEvents m_nudMaxIterations As System.Windows.Forms.NumericUpDown
        Private WithEvents m_lbTolerance As System.Windows.Forms.Label
        Private WithEvents m_tbTolerance As System.Windows.Forms.TextBox
        Private WithEvents m_tbSOR As System.Windows.Forms.TextBox
        Private WithEvents m_lbSOR As System.Windows.Forms.Label
        Private WithEvents m_cbPredictEffort As System.Windows.Forms.CheckBox
        Private WithEvents m_cbUseExact As System.Windows.Forms.CheckBox
        Private WithEvents m_cbContaminantTracing As System.Windows.Forms.CheckBox
    End Class

End Namespace
