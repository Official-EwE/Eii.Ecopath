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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmResults
    Inherits WeifenLuo.WinFormsUI.Docking.DockContent

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmResults))
        Me.chkBiomass = New System.Windows.Forms.CheckBox()
        Me.chkConsumption = New System.Windows.Forms.CheckBox()
        Me.chkBiomassInteg = New System.Windows.Forms.CheckBox()
        Me.chkPredationMortality = New System.Windows.Forms.CheckBox()
        Me.chkFishingMortality = New System.Windows.Forms.CheckBox()
        Me.btnSetPredPrey = New System.Windows.Forms.Button()
        Me.btnSetPreyPred = New System.Windows.Forms.Button()
        Me.btnSaveResults = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.chkPredationPerPredator = New System.Windows.Forms.CheckBox()
        Me.btnSetParentOnly = New System.Windows.Forms.Button()
        Me.chkFishMortFleetToPrey = New System.Windows.Forms.CheckBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.chkCatch = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.chkDietProportions = New System.Windows.Forms.CheckBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.btnSetFleetPrey = New System.Windows.Forms.Button()
        Me.chkCatchFleet = New System.Windows.Forms.CheckBox()
        Me.btnSetFleetOnly = New System.Windows.Forms.Button()
        Me.chkFleetValue = New System.Windows.Forms.CheckBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.chkEffort = New System.Windows.Forms.CheckBox()
        Me.chkBasicEstimates = New System.Windows.Forms.CheckBox()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.chkInitFishMort = New System.Windows.Forms.CheckBox()
        Me.chkInitFishingValues = New System.Windows.Forms.CheckBox()
        Me.chkInitFishingQuantities = New System.Windows.Forms.CheckBox()
        Me.chkSearchRates = New System.Windows.Forms.CheckBox()
        Me.chkElectivity = New System.Windows.Forms.CheckBox()
        Me.chkPredOverlap = New System.Windows.Forms.CheckBox()
        Me.chkPreyOverlap = New System.Windows.Forms.CheckBox()
        Me.chkRespiration = New System.Windows.Forms.CheckBox()
        Me.chkInitConsumption = New System.Windows.Forms.CheckBox()
        Me.chkInitPredMort = New System.Windows.Forms.CheckBox()
        Me.chkMortalityCoefficients = New System.Windows.Forms.CheckBox()
        Me.chkKeyIndices = New System.Windows.Forms.CheckBox()
        Me.btnAllOptions = New System.Windows.Forms.Button()
        Me.prgSave = New System.Windows.Forms.ProgressBar()
        Me.lblPrgInfo = New System.Windows.Forms.Label()
        Me.chkYearly = New System.Windows.Forms.CheckBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.optCSV = New System.Windows.Forms.RadioButton()
        Me.optExcel = New System.Windows.Forms.RadioButton()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.chkresiduals = New System.Windows.Forms.CheckBox()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.chkSS = New System.Windows.Forms.CheckBox()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.Panel3.SuspendLayout()
        Me.Panel4.SuspendLayout()
        Me.Panel6.SuspendLayout()
        Me.Panel7.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel5.SuspendLayout()
        Me.Panel8.SuspendLayout()
        Me.SuspendLayout()
        '
        'chkBiomass
        '
        resources.ApplyResources(Me.chkBiomass, "chkBiomass")
        Me.chkBiomass.Name = "chkBiomass"
        Me.chkBiomass.Text = Global.EwEResultsExtractor.My.Resources.Resources.BIOMASS
        Me.chkBiomass.UseVisualStyleBackColor = True
        '
        'chkConsumption
        '
        resources.ApplyResources(Me.chkConsumption, "chkConsumption")
        Me.chkConsumption.Name = "chkConsumption"
        Me.chkConsumption.Text = Global.EwEResultsExtractor.My.Resources.Resources.CONSUMPTION
        Me.chkConsumption.UseVisualStyleBackColor = True
        '
        'chkBiomassInteg
        '
        resources.ApplyResources(Me.chkBiomassInteg, "chkBiomassInteg")
        Me.chkBiomassInteg.Name = "chkBiomassInteg"
        Me.chkBiomassInteg.Text = Global.EwEResultsExtractor.My.Resources.Resources.BIOMASSINTEGRATED
        Me.chkBiomassInteg.UseVisualStyleBackColor = True
        '
        'chkPredationMortality
        '
        resources.ApplyResources(Me.chkPredationMortality, "chkPredationMortality")
        Me.chkPredationMortality.Name = "chkPredationMortality"
        Me.chkPredationMortality.Text = Global.EwEResultsExtractor.My.Resources.Resources.PREDATIONMORT
        Me.chkPredationMortality.UseVisualStyleBackColor = True
        '
        'chkFishingMortality
        '
        resources.ApplyResources(Me.chkFishingMortality, "chkFishingMortality")
        Me.chkFishingMortality.Name = "chkFishingMortality"
        Me.chkFishingMortality.Text = Global.EwEResultsExtractor.My.Resources.Resources.FISHMORT
        Me.chkFishingMortality.UseVisualStyleBackColor = True
        '
        'btnSetPredPrey
        '
        resources.ApplyResources(Me.btnSetPredPrey, "btnSetPredPrey")
        Me.btnSetPredPrey.Name = "btnSetPredPrey"
        Me.btnSetPredPrey.UseVisualStyleBackColor = True
        '
        'btnSetPreyPred
        '
        resources.ApplyResources(Me.btnSetPreyPred, "btnSetPreyPred")
        Me.btnSetPreyPred.Name = "btnSetPreyPred"
        Me.btnSetPreyPred.Text = Global.EwEResultsExtractor.My.Resources.Resources.CHANGE_SELECTION
        Me.btnSetPreyPred.UseVisualStyleBackColor = True
        '
        'btnSaveResults
        '
        resources.ApplyResources(Me.btnSaveResults, "btnSaveResults")
        Me.btnSaveResults.Name = "btnSaveResults"
        Me.btnSaveResults.Text = Global.EwEResultsExtractor.My.Resources.Resources.SAVE_RESULTS
        Me.btnSaveResults.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        resources.ApplyResources(Me.btnCancel, "btnCancel")
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Text = Global.EwEResultsExtractor.My.Resources.Resources.CANCEL
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'chkPredationPerPredator
        '
        resources.ApplyResources(Me.chkPredationPerPredator, "chkPredationPerPredator")
        Me.chkPredationPerPredator.Name = "chkPredationPerPredator"
        Me.chkPredationPerPredator.Text = Global.EwEResultsExtractor.My.Resources.Resources.PREDATION_PER_PRED
        Me.chkPredationPerPredator.UseVisualStyleBackColor = True
        '
        'btnSetParentOnly
        '
        resources.ApplyResources(Me.btnSetParentOnly, "btnSetParentOnly")
        Me.btnSetParentOnly.Name = "btnSetParentOnly"
        Me.btnSetParentOnly.Text = Global.EwEResultsExtractor.My.Resources.Resources.CHANGE_SELECTION
        Me.btnSetParentOnly.UseVisualStyleBackColor = True
        '
        'chkFishMortFleetToPrey
        '
        resources.ApplyResources(Me.chkFishMortFleetToPrey, "chkFishMortFleetToPrey")
        Me.chkFishMortFleetToPrey.Name = "chkFishMortFleetToPrey"
        Me.chkFishMortFleetToPrey.Text = Global.EwEResultsExtractor.My.Resources.Resources.FISHMORT_FLEET2PREY
        Me.chkFishMortFleetToPrey.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.Azure
        Me.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel1.Controls.Add(Me.chkCatch)
        Me.Panel1.Controls.Add(Me.chkPredationMortality)
        Me.Panel1.Controls.Add(Me.chkBiomass)
        Me.Panel1.Controls.Add(Me.btnSetParentOnly)
        Me.Panel1.Controls.Add(Me.chkBiomassInteg)
        Me.Panel1.Controls.Add(Me.chkFishingMortality)
        resources.ApplyResources(Me.Panel1, "Panel1")
        Me.Panel1.Name = "Panel1"
        '
        'chkCatch
        '
        resources.ApplyResources(Me.chkCatch, "chkCatch")
        Me.chkCatch.Name = "chkCatch"
        Me.chkCatch.Text = Global.EwEResultsExtractor.My.Resources.Resources.TEXT_CATCH
        Me.chkCatch.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.White
        Me.Label1.Name = "Label1"
        '
        'chkDietProportions
        '
        resources.ApplyResources(Me.chkDietProportions, "chkDietProportions")
        Me.chkDietProportions.Name = "chkDietProportions"
        Me.chkDietProportions.Text = Global.EwEResultsExtractor.My.Resources.Resources.DIET_PROPS
        Me.chkDietProportions.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.BackColor = System.Drawing.Color.Azure
        Me.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel2.Controls.Add(Me.chkDietProportions)
        Me.Panel2.Controls.Add(Me.chkConsumption)
        Me.Panel2.Controls.Add(Me.btnSetPredPrey)
        resources.ApplyResources(Me.Panel2, "Panel2")
        Me.Panel2.Name = "Panel2"
        '
        'Label3
        '
        Me.Label3.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.ForeColor = System.Drawing.Color.White
        Me.Label3.Name = "Label3"
        '
        'Panel3
        '
        Me.Panel3.BackColor = System.Drawing.Color.Azure
        Me.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel3.Controls.Add(Me.btnSetPreyPred)
        Me.Panel3.Controls.Add(Me.chkPredationPerPredator)
        resources.ApplyResources(Me.Panel3, "Panel3")
        Me.Panel3.Name = "Panel3"
        '
        'Label4
        '
        Me.Label4.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.ForeColor = System.Drawing.Color.White
        Me.Label4.Name = "Label4"
        '
        'Panel4
        '
        Me.Panel4.BackColor = System.Drawing.Color.Azure
        Me.Panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel4.Controls.Add(Me.Label2)
        Me.Panel4.Controls.Add(Me.btnSetFleetPrey)
        Me.Panel4.Controls.Add(Me.chkCatchFleet)
        Me.Panel4.Controls.Add(Me.chkFishMortFleetToPrey)
        resources.ApplyResources(Me.Panel4, "Panel4")
        Me.Panel4.Name = "Panel4"
        '
        'Label2
        '
        Me.Label2.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.ForeColor = System.Drawing.Color.White
        Me.Label2.Name = "Label2"
        '
        'btnSetFleetPrey
        '
        resources.ApplyResources(Me.btnSetFleetPrey, "btnSetFleetPrey")
        Me.btnSetFleetPrey.Name = "btnSetFleetPrey"
        Me.btnSetFleetPrey.Text = Global.EwEResultsExtractor.My.Resources.Resources.CHANGE_SELECTION
        Me.btnSetFleetPrey.UseVisualStyleBackColor = True
        '
        'chkCatchFleet
        '
        resources.ApplyResources(Me.chkCatchFleet, "chkCatchFleet")
        Me.chkCatchFleet.Name = "chkCatchFleet"
        Me.chkCatchFleet.Text = Global.EwEResultsExtractor.My.Resources.Resources.CATCH_PER_FLEET
        Me.chkCatchFleet.UseVisualStyleBackColor = True
        '
        'btnSetFleetOnly
        '
        resources.ApplyResources(Me.btnSetFleetOnly, "btnSetFleetOnly")
        Me.btnSetFleetOnly.Name = "btnSetFleetOnly"
        Me.btnSetFleetOnly.Text = Global.EwEResultsExtractor.My.Resources.Resources.CHANGE_SELECTION
        Me.btnSetFleetOnly.UseVisualStyleBackColor = True
        '
        'chkFleetValue
        '
        resources.ApplyResources(Me.chkFleetValue, "chkFleetValue")
        Me.chkFleetValue.Name = "chkFleetValue"
        Me.chkFleetValue.Text = Global.EwEResultsExtractor.My.Resources.Resources.VALUE_PER_FLEET
        Me.chkFleetValue.UseVisualStyleBackColor = True
        '
        'Panel6
        '
        Me.Panel6.BackColor = System.Drawing.Color.Azure
        Me.Panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel6.Controls.Add(Me.chkEffort)
        Me.Panel6.Controls.Add(Me.btnSetFleetOnly)
        Me.Panel6.Controls.Add(Me.chkFleetValue)
        resources.ApplyResources(Me.Panel6, "Panel6")
        Me.Panel6.Name = "Panel6"
        '
        'chkEffort
        '
        resources.ApplyResources(Me.chkEffort, "chkEffort")
        Me.chkEffort.Name = "chkEffort"
        Me.chkEffort.Text = Global.EwEResultsExtractor.My.Resources.Resources.EFFORT
        Me.chkEffort.UseVisualStyleBackColor = True
        '
        'chkBasicEstimates
        '
        resources.ApplyResources(Me.chkBasicEstimates, "chkBasicEstimates")
        Me.chkBasicEstimates.Name = "chkBasicEstimates"
        Me.chkBasicEstimates.Text = Global.EwEResultsExtractor.My.Resources.Resources.BASIC_ESTIMATES
        Me.chkBasicEstimates.UseVisualStyleBackColor = True
        '
        'Panel7
        '
        Me.Panel7.BackColor = System.Drawing.Color.Azure
        Me.Panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel7.Controls.Add(Me.Label6)
        Me.Panel7.Controls.Add(Me.chkInitFishMort)
        Me.Panel7.Controls.Add(Me.chkInitFishingValues)
        Me.Panel7.Controls.Add(Me.chkInitFishingQuantities)
        Me.Panel7.Controls.Add(Me.chkSearchRates)
        Me.Panel7.Controls.Add(Me.chkElectivity)
        Me.Panel7.Controls.Add(Me.chkPredOverlap)
        Me.Panel7.Controls.Add(Me.chkPreyOverlap)
        Me.Panel7.Controls.Add(Me.chkRespiration)
        Me.Panel7.Controls.Add(Me.chkInitConsumption)
        Me.Panel7.Controls.Add(Me.chkInitPredMort)
        Me.Panel7.Controls.Add(Me.chkMortalityCoefficients)
        Me.Panel7.Controls.Add(Me.chkKeyIndices)
        Me.Panel7.Controls.Add(Me.chkBasicEstimates)
        resources.ApplyResources(Me.Panel7, "Panel7")
        Me.Panel7.Name = "Panel7"
        '
        'Label6
        '
        Me.Label6.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.ForeColor = System.Drawing.Color.White
        Me.Label6.Name = "Label6"
        '
        'chkInitFishMort
        '
        resources.ApplyResources(Me.chkInitFishMort, "chkInitFishMort")
        Me.chkInitFishMort.Name = "chkInitFishMort"
        Me.chkInitFishMort.Text = Global.EwEResultsExtractor.My.Resources.Resources.INIT_FISH_MORT
        Me.chkInitFishMort.UseVisualStyleBackColor = True
        '
        'chkInitFishingValues
        '
        resources.ApplyResources(Me.chkInitFishingValues, "chkInitFishingValues")
        Me.chkInitFishingValues.Name = "chkInitFishingValues"
        Me.chkInitFishingValues.Text = Global.EwEResultsExtractor.My.Resources.Resources.FISHING_VALUES
        Me.chkInitFishingValues.UseVisualStyleBackColor = True
        '
        'chkInitFishingQuantities
        '
        resources.ApplyResources(Me.chkInitFishingQuantities, "chkInitFishingQuantities")
        Me.chkInitFishingQuantities.Name = "chkInitFishingQuantities"
        Me.chkInitFishingQuantities.Text = Global.EwEResultsExtractor.My.Resources.Resources.FISHING_QUANT
        Me.chkInitFishingQuantities.UseVisualStyleBackColor = True
        '
        'chkSearchRates
        '
        resources.ApplyResources(Me.chkSearchRates, "chkSearchRates")
        Me.chkSearchRates.Name = "chkSearchRates"
        Me.chkSearchRates.Text = Global.EwEResultsExtractor.My.Resources.Resources.SEARCH_RATES
        Me.chkSearchRates.UseVisualStyleBackColor = True
        '
        'chkElectivity
        '
        resources.ApplyResources(Me.chkElectivity, "chkElectivity")
        Me.chkElectivity.Name = "chkElectivity"
        Me.chkElectivity.Text = Global.EwEResultsExtractor.My.Resources.Resources.ELECTIVITY
        Me.chkElectivity.UseVisualStyleBackColor = True
        '
        'chkPredOverlap
        '
        resources.ApplyResources(Me.chkPredOverlap, "chkPredOverlap")
        Me.chkPredOverlap.Name = "chkPredOverlap"
        Me.chkPredOverlap.Text = Global.EwEResultsExtractor.My.Resources.Resources.PRED_OVERLAP
        Me.chkPredOverlap.UseVisualStyleBackColor = True
        '
        'chkPreyOverlap
        '
        resources.ApplyResources(Me.chkPreyOverlap, "chkPreyOverlap")
        Me.chkPreyOverlap.Name = "chkPreyOverlap"
        Me.chkPreyOverlap.Text = Global.EwEResultsExtractor.My.Resources.Resources.PREY_OVERLAP
        Me.chkPreyOverlap.UseVisualStyleBackColor = True
        '
        'chkRespiration
        '
        resources.ApplyResources(Me.chkRespiration, "chkRespiration")
        Me.chkRespiration.Name = "chkRespiration"
        Me.chkRespiration.Text = Global.EwEResultsExtractor.My.Resources.Resources.RESPIRATION
        Me.chkRespiration.UseVisualStyleBackColor = True
        '
        'chkInitConsumption
        '
        resources.ApplyResources(Me.chkInitConsumption, "chkInitConsumption")
        Me.chkInitConsumption.Name = "chkInitConsumption"
        Me.chkInitConsumption.Text = Global.EwEResultsExtractor.My.Resources.Resources.INIT_CONSUMPTION
        Me.chkInitConsumption.UseVisualStyleBackColor = True
        '
        'chkInitPredMort
        '
        resources.ApplyResources(Me.chkInitPredMort, "chkInitPredMort")
        Me.chkInitPredMort.Name = "chkInitPredMort"
        Me.chkInitPredMort.Text = Global.EwEResultsExtractor.My.Resources.Resources.INIT_PRED_MORT
        Me.chkInitPredMort.UseVisualStyleBackColor = True
        '
        'chkMortalityCoefficients
        '
        resources.ApplyResources(Me.chkMortalityCoefficients, "chkMortalityCoefficients")
        Me.chkMortalityCoefficients.Name = "chkMortalityCoefficients"
        Me.chkMortalityCoefficients.Text = Global.EwEResultsExtractor.My.Resources.Resources.MORT_COEFFS
        Me.chkMortalityCoefficients.UseVisualStyleBackColor = True
        '
        'chkKeyIndices
        '
        resources.ApplyResources(Me.chkKeyIndices, "chkKeyIndices")
        Me.chkKeyIndices.Name = "chkKeyIndices"
        Me.chkKeyIndices.Text = Global.EwEResultsExtractor.My.Resources.Resources.KEY_INDICES
        Me.chkKeyIndices.UseVisualStyleBackColor = True
        '
        'btnAllOptions
        '
        resources.ApplyResources(Me.btnAllOptions, "btnAllOptions")
        Me.btnAllOptions.Name = "btnAllOptions"
        Me.btnAllOptions.Text = Global.EwEResultsExtractor.My.Resources.Resources.ALL_OPTIONS
        Me.btnAllOptions.UseVisualStyleBackColor = True
        '
        'prgSave
        '
        resources.ApplyResources(Me.prgSave, "prgSave")
        Me.prgSave.Name = "prgSave"
        '
        'lblPrgInfo
        '
        resources.ApplyResources(Me.lblPrgInfo, "lblPrgInfo")
        Me.lblPrgInfo.Name = "lblPrgInfo"
        '
        'chkYearly
        '
        resources.ApplyResources(Me.chkYearly, "chkYearly")
        Me.chkYearly.Name = "chkYearly"
        Me.chkYearly.Text = Global.EwEResultsExtractor.My.Resources.Resources.YEARLY
        Me.chkYearly.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'PictureBox2
        '
        resources.ApplyResources(Me.PictureBox2, "PictureBox2")
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.TabStop = False
        '
        'Label5
        '
        Me.Label5.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.ForeColor = System.Drawing.Color.White
        Me.Label5.Name = "Label5"
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.optCSV)
        Me.Panel5.Controls.Add(Me.optExcel)
        resources.ApplyResources(Me.Panel5, "Panel5")
        Me.Panel5.Name = "Panel5"
        '
        'optCSV
        '
        resources.ApplyResources(Me.optCSV, "optCSV")
        Me.optCSV.Name = "optCSV"
        Me.optCSV.TabStop = True
        Me.optCSV.Text = Global.EwEResultsExtractor.My.Resources.Resources.DOTCSV
        Me.optCSV.UseVisualStyleBackColor = True
        '
        'optExcel
        '
        resources.ApplyResources(Me.optExcel, "optExcel")
        Me.optExcel.Name = "optExcel"
        Me.optExcel.TabStop = True
        Me.optExcel.Text = Global.EwEResultsExtractor.My.Resources.Resources.EXCEL
        Me.optExcel.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.BackColor = System.Drawing.Color.RoyalBlue
        Me.Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.ForeColor = System.Drawing.Color.White
        Me.Label7.Name = "Label7"
        '
        'chkresiduals
        '
        resources.ApplyResources(Me.chkresiduals, "chkresiduals")
        Me.chkresiduals.Name = "chkresiduals"
        Me.chkresiduals.Text = Global.EwEResultsExtractor.My.Resources.Resources.RESIDUALS
        Me.chkresiduals.UseVisualStyleBackColor = True
        '
        'Panel8
        '
        Me.Panel8.BackColor = System.Drawing.Color.Azure
        Me.Panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Panel8.Controls.Add(Me.chkSS)
        Me.Panel8.Controls.Add(Me.chkresiduals)
        resources.ApplyResources(Me.Panel8, "Panel8")
        Me.Panel8.Name = "Panel8"
        '
        'chkSS
        '
        resources.ApplyResources(Me.chkSS, "chkSS")
        Me.chkSS.Name = "chkSS"
        Me.chkSS.Text = Global.EwEResultsExtractor.My.Resources.Resources.SUM_OF_SQUARES
        Me.chkSS.UseVisualStyleBackColor = True
        '
        'frmResults
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Panel8)
        Me.Controls.Add(Me.Panel5)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.chkYearly)
        Me.Controls.Add(Me.lblPrgInfo)
        Me.Controls.Add(Me.prgSave)
        Me.Controls.Add(Me.btnAllOptions)
        Me.Controls.Add(Me.Panel7)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnSaveResults)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.PictureBox2)
        Me.Controls.Add(Me.Panel3)
        Me.Name = "frmResults"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.Panel3.ResumeLayout(False)
        Me.Panel3.PerformLayout()
        Me.Panel4.ResumeLayout(False)
        Me.Panel4.PerformLayout()
        Me.Panel6.ResumeLayout(False)
        Me.Panel6.PerformLayout()
        Me.Panel7.ResumeLayout(False)
        Me.Panel7.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel5.ResumeLayout(False)
        Me.Panel5.PerformLayout()
        Me.Panel8.ResumeLayout(False)
        Me.Panel8.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkBiomass As System.Windows.Forms.CheckBox
    Friend WithEvents chkConsumption As System.Windows.Forms.CheckBox
    Friend WithEvents chkBiomassInteg As System.Windows.Forms.CheckBox
    Friend WithEvents chkPredationMortality As System.Windows.Forms.CheckBox
    Friend WithEvents chkFishingMortality As System.Windows.Forms.CheckBox
    Friend WithEvents btnSetPredPrey As System.Windows.Forms.Button
    Friend WithEvents btnSetPreyPred As System.Windows.Forms.Button
    Friend WithEvents btnSaveResults As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents chkPredationPerPredator As System.Windows.Forms.CheckBox
    Friend WithEvents btnSetParentOnly As System.Windows.Forms.Button
    Friend WithEvents chkFishMortFleetToPrey As System.Windows.Forms.CheckBox
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents Panel3 As System.Windows.Forms.Panel
    Friend WithEvents Panel4 As System.Windows.Forms.Panel
    Friend WithEvents chkDietProportions As System.Windows.Forms.CheckBox
    Friend WithEvents chkCatch As System.Windows.Forms.CheckBox
    Friend WithEvents btnSetFleetPrey As System.Windows.Forms.Button
    Friend WithEvents chkCatchFleet As System.Windows.Forms.CheckBox
    Friend WithEvents btnSetFleetOnly As System.Windows.Forms.Button
    Friend WithEvents chkFleetValue As System.Windows.Forms.CheckBox
    Friend WithEvents Panel6 As System.Windows.Forms.Panel
    Friend WithEvents chkBasicEstimates As System.Windows.Forms.CheckBox
    Friend WithEvents Panel7 As System.Windows.Forms.Panel
    Friend WithEvents chkKeyIndices As System.Windows.Forms.CheckBox
    Friend WithEvents chkMortalityCoefficients As System.Windows.Forms.CheckBox
    Friend WithEvents chkInitPredMort As System.Windows.Forms.CheckBox
    Friend WithEvents chkInitConsumption As System.Windows.Forms.CheckBox
    Friend WithEvents chkRespiration As System.Windows.Forms.CheckBox
    Friend WithEvents chkPreyOverlap As System.Windows.Forms.CheckBox
    Friend WithEvents chkPredOverlap As System.Windows.Forms.CheckBox
    Friend WithEvents chkSearchRates As System.Windows.Forms.CheckBox
    Friend WithEvents chkElectivity As System.Windows.Forms.CheckBox
    Friend WithEvents chkInitFishingQuantities As System.Windows.Forms.CheckBox
    Friend WithEvents chkInitFishingValues As System.Windows.Forms.CheckBox
    Friend WithEvents btnAllOptions As System.Windows.Forms.Button
    Friend WithEvents prgSave As System.Windows.Forms.ProgressBar
    Friend WithEvents lblPrgInfo As System.Windows.Forms.Label
    Friend WithEvents chkYearly As System.Windows.Forms.CheckBox
    Friend WithEvents chkEffort As System.Windows.Forms.CheckBox
    Friend WithEvents chkInitFishMort As System.Windows.Forms.CheckBox
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Panel5 As System.Windows.Forms.Panel
    Friend WithEvents optCSV As System.Windows.Forms.RadioButton
    Friend WithEvents optExcel As System.Windows.Forms.RadioButton
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents chkresiduals As System.Windows.Forms.CheckBox
    Friend WithEvents Panel8 As System.Windows.Forms.Panel
    Friend WithEvents chkSS As System.Windows.Forms.CheckBox
End Class
