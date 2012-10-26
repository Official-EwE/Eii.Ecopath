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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Form from which to configure generic Ecospace parameters.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class EcospaceParameters

#Region " Private vars "

        ' Scenario generics
        Private m_fpScenarioName As cEwEFormatProvider = Nothing
        Private m_fpScenarioDescription As cEwEFormatProvider = Nothing
        Private m_fpAuthor As cEwEFormatProvider = Nothing
        Private m_fpContact As cEwEFormatProvider = Nothing

        ' Threading
        Private m_fpNumThreads As cEwEFormatProvider = Nothing
        Private m_fpNumThreads2 As cEwEFormatProvider = Nothing
        Private m_fpNumPackets As cEwEFormatProvider = Nothing

        ' Model
        Private m_fpTotalTime As cEwEFormatProvider = Nothing
        Private m_fpNumTSpYear As cEwEFormatProvider = Nothing
        Private m_fpTolerance As cEwEFormatProvider = Nothing
        Private m_fpSOR As cEwEFormatProvider = Nothing
        Private m_fpMaxIterations As cEwEFormatProvider = Nothing
        Private m_fpUseExact As cEwEFormatProvider = Nothing

        Private m_fpMovePackets As cEwEFormatProvider = Nothing
        Private WithEvents m_bpConTracing As cBooleanProperty = Nothing

        ' Properties to monitor for setting radio button check states
        Private WithEvents m_bpUseIBM As cBooleanProperty = Nothing
        Private WithEvents m_bpUseNewStanza As cBooleanProperty = Nothing
        Private WithEvents m_bpAdjustSpace As cBooleanProperty = Nothing
        Private WithEvents m_bpEffort As cBooleanProperty = Nothing

#End Region ' Private vars

#Region " Form events "

        Public Sub New()
            Me.InitializeComponent()
        End Sub
        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the form is initially loaded.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.InitContent()
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace, eCoreComponentType.Core}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_bpUseIBM = Nothing
            Me.m_bpUseNewStanza = Nothing
            Me.m_bpAdjustSpace = Nothing
            Me.m_bpConTracing = Nothing
            Me.m_bpEffort = Nothing
 
            Me.m_fpScenarioName.Release()
            Me.m_fpScenarioDescription.Release()
            Me.m_fpAuthor.Release()
            Me.m_fpContact.Release()

            Me.m_fpNumThreads.Release()
            Me.m_fpNumThreads2.Release()
            Me.m_fpNumPackets.Release()
            Me.m_fpTotalTime.Release()
            Me.m_fpNumTSpYear.Release()
            Me.m_fpTolerance.Release()
            Me.m_fpSOR.Release()
            Me.m_fpMaxIterations.Release()
            Me.m_fpUseExact.Release()
            Me.m_fpMovePackets.Release()

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub InitContent()

            Dim ecospaceModelParams As cEcospaceModelParameters = Me.Core.EcospaceModelParameters()
            Dim pm As cPropertyManager = Me.PropertyManager

            ' Start listening to props
            Me.m_bpUseIBM = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseIBM), cBooleanProperty)
            Me.m_bpUseNewStanza = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseNewMultiStanza), cBooleanProperty)
            Me.m_bpAdjustSpace = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.AdjustSpace), cBooleanProperty)
            Me.m_bpEffort = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.PredictEffort), cBooleanProperty)

            Me.m_bpConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)

            Me.UpdateControls()

            ' Hmm, connecting one control to two live properties - this could be dangerous
            Me.m_fpNumThreads = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nGridSolverThreads)
            Me.m_fpNumThreads2 = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nSpaceThreads)
            Me.m_fpNumPackets = New cPropertyFormatProvider(Me.UIContext, Me.m_tbNumPackets, ecospaceModelParams, eVarNameFlags.PacketsMultiplier)

            ' Model
            Me.m_fpTotalTime = New cPropertyFormatProvider(Me.UIContext, Me.m_tbTotalTime, ecospaceModelParams, eVarNameFlags.TotalTime)
            Me.m_fpNumTSpYear = New cPropertyFormatProvider(Me.UIContext, Me.m_tbNumTimeStepsPerYear, ecospaceModelParams, eVarNameFlags.NumTimeStepsPerYear)
            Me.m_fpTolerance = New cPropertyFormatProvider(Me.UIContext, Me.m_tbTolerance, ecospaceModelParams, eVarNameFlags.Tolerance)
            Me.m_fpSOR = New cPropertyFormatProvider(Me.UIContext, Me.m_tbSOR, ecospaceModelParams, eVarNameFlags.SOR)
            Me.m_fpMaxIterations = New cPropertyFormatProvider(Me.UIContext, Me.m_nudMaxIterations, ecospaceModelParams, eVarNameFlags.MaxIterations)
            Me.m_fpUseExact = New cPropertyFormatProvider(Me.UIContext, Me.m_cbUseExact, ecospaceModelParams, eVarNameFlags.UseExact)

            Me.m_fpMovePackets = New cPropertyFormatProvider(Me.UIContext, Me.m_cbMovePackets, ecospaceModelParams, eVarNameFlags.EcospaceIBMMovePacketOnStanza)

            Me.UpdateScenarioFormatProviders()

        End Sub

#End Region ' Form events

#Region " Form content handling "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper enum, used to determine the threading model type from ecospace data flags.
        ''' </summary>
        ''' <remarks>MUAAAHAHAAHHAAH!</remarks>
        ''' -------------------------------------------------------------------
        Private Enum eThreadingModelType As Integer
            UseNewStanza
            UseIBM
            OldSchool
        End Enum

        Private m_bInUpdate As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update and enable controls that cannot be managed any other way.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub UpdateControls()

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Me.m_bInUpdate = True

            Dim threadingModel As eThreadingModelType = eThreadingModelType.OldSchool
            Dim bUseIBM As Boolean = CBool(Me.m_bpUseIBM.GetValue())
            Dim bUseNewStanza As Boolean = CBool(Me.m_bpUseNewStanza.GetValue())
            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

            If bUseIBM Then threadingModel = eThreadingModelType.UseIBM
            If bUseNewStanza Then threadingModel = eThreadingModelType.UseNewStanza

            Select Case threadingModel
                Case eThreadingModelType.OldSchool
                    Me.m_rbOldSchool.Checked = True
                Case eThreadingModelType.UseIBM
                    Me.m_rbIBM.Checked = True
                Case eThreadingModelType.UseNewStanza
                    Me.m_rbNewStanzaModel.Checked = True
            End Select

            Me.m_rbBaseBiomass.Checked = Not CBool(Me.m_bpAdjustSpace.GetValue())
            Me.m_rbAdjustedBiomass.Checked = CBool(Me.m_bpAdjustSpace.GetValue())

            Me.m_cbContaminantTracing.Checked = CBool(Me.m_bpConTracing.GetValue())

            Me.m_rbCapHap.Checked = (parms.CapacityCalculationType = eEcospaceCapacityCalType.CapacityAndHabitat)
            Me.m_rbCap.Checked = (parms.CapacityCalculationType = eEcospaceCapacityCalType.Capacity)
            Me.m_rbHab.Checked = (parms.CapacityCalculationType = eEcospaceCapacityCalType.Habitat)

            Me.m_cbSaveASC.Checked = Me.Core.Autosave(eAutosaveTypes.EcospaceASC)
            Me.m_cbSaveCSV.Checked = Me.Core.Autosave(eAutosaveTypes.EcospaceCSV)

            Me.m_rbPredictEffort.Checked = CBool(Me.m_bpEffort.GetValue())
            Me.m_rbEcopathEffort.Checked = Not CBool(Me.m_bpEffort.GetValue())

            Me.m_bInUpdate = False

        End Sub

#End Region ' Form content handling

#Region " cProperty events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when either of the two model state properties changes.
        ''' </summary>
        ''' <param name="prop">The property that changed.</param>
        ''' <param name="changeFlags">The extent of the change.</param>
        ''' -------------------------------------------------------------------
        Private Sub OnPropertyChanged(ByVal prop As cProperty, ByVal changeFlags As cProperty.eChangeFlags) _
                Handles m_bpUseIBM.PropertyChanged, m_bpUseNewStanza.PropertyChanged, m_bpConTracing.PropertyChanged

            Me.UpdateControls()
        End Sub

#End Region ' cProperty events

#Region " Control events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the IBM mode radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnRunIBM(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_rbIBM.CheckedChanged, m_rbNewStanzaModel.CheckedChanged, m_rbOldSchool.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            If Me.m_rbIBM.Checked Then
                Me.m_bpUseNewStanza.SetValue(False)
                Me.m_bpUseIBM.SetValue(True)
            ElseIf Me.m_rbNewStanzaModel.Checked Then
                Me.m_bpUseIBM.SetValue(False)
                Me.m_bpUseNewStanza.SetValue(True)
            ElseIf Me.m_rbOldSchool.Checked Then
                Me.m_bpUseNewStanza.SetValue(False)
                Me.m_bpUseIBM.SetValue(False)
            End If

        End Sub

        Private Sub OnBiomassOptionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_rbBaseBiomass.CheckedChanged, m_rbAdjustedBiomass.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Me.m_bpAdjustSpace.SetValue(Me.m_rbAdjustedBiomass.Checked)

        End Sub

        Private Sub OnEffortOptionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_rbPredictEffort.CheckedChanged, m_rbEcopathEffort.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Me.m_bpEffort.SetValue(Me.m_rbPredictEffort.Checked)

        End Sub

        Private Sub OnConcTracingOptionChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_cbContaminantTracing.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            If Me.m_cbContaminantTracing.Checked Then
                Dim cmdh As cCommandHandler = Me.CommandHandler
                Dim cmd As cCommand = cmdh.GetCommand("EnableEcotracer")

                If (cmd IsNot Nothing) Then
                    cmd.Tag = eTracerRunModeTypes.RunSpace
                    cmd.Invoke()
                    If (Me.Core.ActiveEcotracerScenarioIndex <= 0) Then
                        Me.m_cbContaminantTracing.Checked = False
                    End If
                End If
            End If

            ' If tracer scenario loaded turn this on
            Me.m_bpConTracing.SetValue(Me.m_cbContaminantTracing.Checked)

            Me.UpdateControls()

        End Sub

        Private Sub OnCapCalcOptionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbCapHap.CheckedChanged, m_rbCap.CheckedChanged, m_rbHab.CheckedChanged

            If (Me.UIContext Is Nothing) Then Return
            If (Me.m_bInUpdate) Then Return

            Dim capcalctype As eEcospaceCapacityCalType = eEcospaceCapacityCalType.CapacityAndHabitat
            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

            If Me.m_rbHab.Checked Then
                capcalctype = eEcospaceCapacityCalType.Habitat
            ElseIf Me.m_rbCap.Checked Then
                capcalctype = eEcospaceCapacityCalType.Capacity
            End If

            parms.CapacityCalculationType = capcalctype

        End Sub

        Private Sub OnSaveCSVClicked(sender As Object, e As EventArgs) _
            Handles m_cbSaveCSV.Click
            Try
                Me.Core.Autosave(eAutosaveTypes.EcospaceCSV) = Me.m_cbSaveCSV.Checked
            Catch ex As Exception
                ' Ouch
            End Try
        End Sub

        Private Sub OnSaveASCIIClicked(sender As Object, e As EventArgs) _
            Handles m_cbSaveASC.Click
            Try
                Me.Core.Autosave(eAutosaveTypes.EcospaceASC) = Me.m_cbSaveASC.Checked
            Catch ex As Exception
                ' Ouch
            End Try
        End Sub

#End Region ' Control events

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
                ' Reload
                Me.InitContent()
            End If
            If ((msg.Source = eCoreComponentType.Core) And (msg.Type = eMessageType.GlobalSettingsChanged)) Then
                Me.UpdateControls()
            End If
        End Sub

#End Region ' Overrides

#Region " Internals "

        Private Sub UpdateScenarioFormatProviders()

            Dim scenarioDef As cEcospaceScenario = Core.EcospaceScenarios(Core.ActiveEcospaceScenarioIndex)

            ' Connect controls to core data
            Me.m_fpScenarioName = New cPropertyFormatProvider(Me.UIContext, Me.m_tbName, scenarioDef, eVarNameFlags.Name)
            Me.m_fpScenarioDescription = New cPropertyFormatProvider(Me.UIContext, Me.m_tbDescription, scenarioDef, eVarNameFlags.Description)
            Me.m_fpAuthor = New cPropertyFormatProvider(Me.UIContext, Me.m_tbAuthor, scenarioDef, eVarNameFlags.Author)
            Me.m_fpContact = New cPropertyFormatProvider(Me.UIContext, Me.m_tbContact, scenarioDef, eVarNameFlags.Contact)

        End Sub

#End Region ' Internals

    End Class

End Namespace
