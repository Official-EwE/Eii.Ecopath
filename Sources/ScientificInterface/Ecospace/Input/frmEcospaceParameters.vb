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
        Private m_fpPredictEffort As cEwEFormatProvider = Nothing
        Private m_fpUseExact As cEwEFormatProvider = Nothing

        Private m_fpMovePackets As cEwEFormatProvider = Nothing

        Private WithEvents m_bpConTracing As cBooleanProperty = Nothing

        Private m_fpSaveCSV As cEwEFormatProvider = Nothing
        Private m_fpSaveASC As cEwEFormatProvider = Nothing

        ' Properties to monitor for setting radio button check states
        Private WithEvents m_bpUseIBM As cBooleanProperty = Nothing
        Private WithEvents m_bpUseNewStanza As cBooleanProperty = Nothing
        Private WithEvents m_bpAdjustSpace As cBooleanProperty = Nothing


#End Region ' Private vars

#Region " Form events "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the form is initially loaded.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.InitContent()
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.m_bpUseIBM = Nothing
            Me.m_bpUseNewStanza = Nothing
            Me.m_bpAdjustSpace = Nothing
            Me.m_bpConTracing = Nothing
 
            Me.m_fpScenarioName.Release()
            Me.m_fpScenarioDescription.Release()
            Me.m_fpAuthor.Release()
            Me.m_fpContact.Release()
            Me.m_fpSaveCSV.Release()
            Me.m_fpSaveASC.Release()

            Me.m_fpNumThreads.Release()
            Me.m_fpNumThreads2.Release()
            Me.m_fpNumPackets.Release()
            Me.m_fpTotalTime.Release()
            Me.m_fpNumTSpYear.Release()
            Me.m_fpTolerance.Release()
            Me.m_fpSOR.Release()
            Me.m_fpMaxIterations.Release()
            Me.m_fpPredictEffort.Release()
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

            Me.m_bpConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)



            ' Initialize
            ' OK, the positioning of this code requires some explanation. Consider the following facts:
            ' - Form.Load sets the focus to the first control in the tab order.
            ' - Setting the checked state of a radio button will give it the focus.
            ' - EwE format providers typically update code data via a Control.Leave event. 
            ' If UpdateControls were called AFTER the format providers were initialized, and the first control 
            ' in the tab order were EwEFormatProvider controller, this control looses its focus as a result of
            ' the UpdateControls call. This will then result in a data change in the core, flagging the
            ' underlying Ecospace model as dirty.
            ' 
            ' This is not allowed. Therefore, all format providers are initialized last in this method.
            Me.UpdateControls()

            ' Hmm, connecting one control to two live properties - this could be dangerous
            Me.m_fpNumThreads = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nSolverThreads)
            Me.m_fpNumThreads2 = New cPropertyFormatProvider(Me.UIContext, Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nSpaceThreads)
            Me.m_fpNumPackets = New cPropertyFormatProvider(Me.UIContext, Me.m_tbNumPackets, ecospaceModelParams, eVarNameFlags.PacketsMultiplier)

            ' Model
            Me.m_fpTotalTime = New cPropertyFormatProvider(Me.UIContext, Me.m_tbTotalTime, ecospaceModelParams, eVarNameFlags.TotalTime)
            Me.m_fpNumTSpYear = New cPropertyFormatProvider(Me.UIContext, Me.m_tbNumTimeStepsPerYear, ecospaceModelParams, eVarNameFlags.NumTimeStepsPerYear)
            Me.m_fpTolerance = New cPropertyFormatProvider(Me.UIContext, Me.m_tbTolerance, ecospaceModelParams, eVarNameFlags.Tolerance)
            Me.m_fpSOR = New cPropertyFormatProvider(Me.UIContext, Me.m_tbSOR, ecospaceModelParams, eVarNameFlags.SOR)
            Me.m_fpMaxIterations = New cPropertyFormatProvider(Me.UIContext, Me.m_nudMaxIterations, ecospaceModelParams, eVarNameFlags.MaxIterations)
            Me.m_fpPredictEffort = New cPropertyFormatProvider(Me.UIContext, Me.m_cbPredictEffort, ecospaceModelParams, eVarNameFlags.PredictEffort)
            Me.m_fpUseExact = New cPropertyFormatProvider(Me.UIContext, Me.m_cbUseExact, ecospaceModelParams, eVarNameFlags.UseExact)

            Me.m_fpMovePackets = New cPropertyFormatProvider(Me.UIContext, Me.m_cbMovePackets, ecospaceModelParams, eVarNameFlags.EcospaceIBMMovePacketOnStanza)

            Me.m_fpSaveCSV = New cPropertyFormatProvider(Me.UIContext, Me.m_cbSaveCSV, ecospaceModelParams, eVarNameFlags.EcospaceSaveCSV)
            Me.m_fpSaveASC = New cPropertyFormatProvider(Me.UIContext, Me.m_cbSaveASC, ecospaceModelParams, eVarNameFlags.EcospaceSaveASC)

            'Me.m_fpUseRelTime = New cPropertyFormatProvider(Me.UIContext, Me.m_chkUseRelativeTime, ecospaceModelParams, eVarNameFlags.UseRelativeTime)

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update and enable controls that cannot be managed any other way.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub UpdateControls()

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
            Handles m_rbIBM.CheckedChanged

            If Me.UIContext Is Nothing Then Return

            If Me.m_rbIBM.Checked Then
                ' Set the value, let property value cascades do the rest
                Me.m_bpUseNewStanza.SetValue(False)
                Me.m_bpUseIBM.SetValue(True)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the New Stanza mode radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnRunNewStanza(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_rbNewStanzaModel.CheckedChanged

            If Me.UIContext Is Nothing Then Return

            If Me.m_rbNewStanzaModel.Checked Then
                ' Set the value, let property value cascades do the rest
                Me.m_bpUseIBM.SetValue(False)
                Me.m_bpUseNewStanza.SetValue(True)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the 'Old School' radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnRunOldSchool(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_rbOldSchool.CheckedChanged

            If Me.UIContext Is Nothing Then Return

            If Me.m_rbOldSchool.Checked Then
                ' Set the value, let property value cascades do the rest
                Me.m_bpUseNewStanza.SetValue(False)
                Me.m_bpUseIBM.SetValue(False)
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the Ecopath habitat radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub rbBaseBiomass_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_rbBaseBiomass.Validated
            ' Set the value, let property value cascades do the rest
            Me.m_bpAdjustSpace.SetValue(False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the Ecopath habitat radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub rbAdjustedBiomass_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_rbAdjustedBiomass.Validated
            ' Set the value, let property value cascades do the rest
            Me.m_bpAdjustSpace.SetValue(True)
        End Sub

        Private Sub cbContaminantTracing_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_cbContaminantTracing.Click

            If m_cbContaminantTracing.Checked Then
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

            Dim capcalctype As eEcospaceCapacityCalType = eEcospaceCapacityCalType.CapacityAndHabitat
            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters

            If Me.m_rbHab.Checked Then
                capcalctype = eEcospaceCapacityCalType.Habitat
            ElseIf Me.m_rbCap.Checked Then
                capcalctype = eEcospaceCapacityCalType.Capacity
            End If

            parms.CapacityCalculationType = capcalctype

        End Sub

#End Region ' Control events

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If ((msg.Source = eCoreComponentType.EcoSpace) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
                ' Reload
                Me.InitContent()
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
