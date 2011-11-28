
#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports


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

    Private Sub EcospaceParameters_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
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

        Me.m_lbOutputDir.Text = m_batchManager.Parameters.OutputDir

    End Sub

    Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        Me.m_fpBiomass = Nothing


        MyBase.OnFormClosed(e)
    End Sub

End Class