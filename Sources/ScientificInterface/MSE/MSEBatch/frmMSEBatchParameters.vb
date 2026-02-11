' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Public Class frmMSEBatchParameters

    ' ToDo: Add XML comments

    ' Properties to monitor for setting radio button check states
    Private WithEvents m_fpBiomass As cPropertyFormatProvider = Nothing
    Private WithEvents m_fpCatch As cPropertyFormatProvider = Nothing
    Private WithEvents m_fpQB As cPropertyFormatProvider = Nothing
    Private WithEvents m_fpF As cPropertyFormatProvider = Nothing
    Private WithEvents m_fpPred As cPropertyFormatProvider = Nothing
    Private WithEvents m_fpFeeding As cPropertyFormatProvider = Nothing

    Private m_batchManager As EwECore.MSEBatchManager.cMSEBatchManager

    Public Sub New()
        MyBase.New()
        Me.InitializeComponent()
    End Sub

    Private Sub EcospaceParameters_Load(sender As Object, e As System.EventArgs) _
          Handles Me.Load

        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.MSE}

        Me.m_batchManager = Me.UIContext.Core.MSEBatchManager

        Dim pm As cPropertyManager = Me.PropertyManager

        'Biomass()
        'QB() 'consumption/biomass
        'FeedingTime()
        'FishingMortRate()
        'PredRate()
        'CatchByGroup()
        Me.m_fpBiomass = New cPropertyFormatProvider(Me.UIContext, Me.chkSaveBiomass, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputBiomass)
        Me.m_fpCatch = New cPropertyFormatProvider(Me.UIContext, Me.chkCatch, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputCatch)
        Me.m_fpF = New cPropertyFormatProvider(Me.UIContext, Me.chkFishingMort, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputFishingMortRate)
        Me.m_fpPred = New cPropertyFormatProvider(Me.UIContext, Me.chkPredMort, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputPredRate)
        Me.m_fpQB = New cPropertyFormatProvider(Me.UIContext, Me.chkQB, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputConBio)
        Me.m_fpFeeding = New cPropertyFormatProvider(Me.UIContext, Me.chkFeedingTime, Me.m_batchManager.Parameters, eVarNameFlags.MSEBatchOutputFeedingTime)

        Me.m_lbOutputDir.Text = Me.m_batchManager.Parameters.OutputDir

    End Sub

    Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_fpBiomass = Nothing

        MyBase.OnFormClosed(e)
    End Sub

End Class