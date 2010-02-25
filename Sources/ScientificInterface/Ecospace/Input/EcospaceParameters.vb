'==============================================================================
'
' $Log: EcospaceParameters.vb,v $
' Revision 1.7  2009/05/11 01:50:48  jeroens
' Renamed command classes
'
' Revision 1.6  2009/03/19 16:02:26  jeroens
' Added FormatProvider.Release
'
' Revision 1.5  2009/02/05 17:48:39  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.4  2009/01/16 18:30:07  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.3  2009/01/15 22:39:55  joeb
' Moved Ecospace start and end summary periods from Parameters form to Results form
'
' Revision 1.2  2008/12/15 15:52:26  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:55  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

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

        Private m_core As cCore = Nothing

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
        Private WithEvents m_bpConTracing As cBooleanProperty = Nothing

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
        Private Sub EcospaceParameters_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

            Me.m_core = cCore.GetInstance()
            Me.InitContent()
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace}
        End Sub

        Protected Overrides Sub OnFormClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

            Me.CoreComponents = Nothing
            Me.m_bpUseIBM = Nothing
            Me.m_bpUseNewStanza = Nothing
            Me.m_bpAdjustSpace = Nothing
            Me.m_bpConTracing = Nothing

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
            Me.m_fpPredictEffort.Release()
            Me.m_fpUseExact.Release()

            MyBase.OnFormClosed(e)
        End Sub

        Private Sub InitContent()

            Dim ecospaceModelParams As cEcospaceModelParameters = Me.m_core.EcospaceModelParameters()
            Dim pm As cPropertyManager = Me.PropertyManager

            ' Start listening to props
            Me.m_bpUseIBM = CType(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseIBM), cBooleanProperty)
            Me.m_bpUseNewStanza = CType(pm.GetProperty(ecospaceModelParams, eVarNameFlags.UseNewMultiStanza), cBooleanProperty)
            Me.m_bpAdjustSpace = CType(pm.GetProperty(ecospaceModelParams, eVarNameFlags.AdjustSpace), cBooleanProperty)

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
            Me.m_fpNumThreads = New cPropertyFormatProvider(Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nSolverThreads)
            Me.m_fpNumThreads2 = New cPropertyFormatProvider(Me.m_nudNumThreads, ecospaceModelParams, eVarNameFlags.nSpaceThreads)
            Me.m_fpNumPackets = New cPropertyFormatProvider(Me.m_tbNumPackets, ecospaceModelParams, eVarNameFlags.PacketsMultiplier)

            ' Model
            Me.m_fpTotalTime = New cPropertyFormatProvider(Me.m_tbTotalTime, ecospaceModelParams, eVarNameFlags.TotalTime)
            Me.m_fpNumTSpYear = New cPropertyFormatProvider(Me.m_tbNumTimeStepsPerYear, ecospaceModelParams, eVarNameFlags.NumTimeStepsPerYear)
            Me.m_fpTolerance = New cPropertyFormatProvider(Me.m_tbTolerance, ecospaceModelParams, eVarNameFlags.Tolerance)
            Me.m_fpSOR = New cPropertyFormatProvider(Me.m_tbSOR, ecospaceModelParams, eVarNameFlags.SOR)
            Me.m_fpMaxIterations = New cPropertyFormatProvider(Me.m_nudMaxIterations, ecospaceModelParams, eVarNameFlags.MaxIterations)
            Me.m_fpPredictEffort = New cPropertyFormatProvider(Me.m_cbPredictEffort, ecospaceModelParams, eVarNameFlags.PredictEffort)
            Me.m_fpUseExact = New cPropertyFormatProvider(Me.m_cbUseExact, ecospaceModelParams, eVarNameFlags.UseExact)
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
        Private Sub UpdateControls()

            Dim threadingModel As eThreadingModelType = eThreadingModelType.OldSchool
            Dim bUseIBM As Boolean = CBool(Me.m_bpUseIBM.GetValue())
            Dim bUseNewStanza As Boolean = CBool(Me.m_bpUseNewStanza.GetValue())

            Debug.Assert(Not (bUseIBM And bUseNewStanza), "Mutually exclusive options are both set, something is wrong in the Ecospace core")

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

            m_rbBaseBiomass.Checked = Not CBool(Me.m_bpAdjustSpace.GetValue())
            m_rbAdjustedBiomass.Checked = CBool(Me.m_bpAdjustSpace.GetValue())

            Me.m_cbContaminantTracing.Checked = CBool(Me.m_bpConTracing.GetValue())

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
        Private Sub rbIBM_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_rbIBM.Validated
            ' Set the value, let property value cascades do the rest
            Me.m_bpUseNewStanza.SetValue(False)
            Me.m_bpUseIBM.SetValue(True)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the New Stanza mode radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub rbNewStanzaModel_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_rbNewStanzaModel.Validated
            ' Set the value, let property value cascades do the rest
            Me.m_bpUseIBM.SetValue(False)
            Me.m_bpUseNewStanza.SetValue(True)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Event handler; called when the 'Old School' radio button is checked.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub rbOldSchool_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_rbOldSchool.Validated
            ' Set the value, let property value cascades do the rest
            Me.m_bpUseNewStanza.SetValue(False)
            Me.m_bpUseIBM.SetValue(False)
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
                Dim cmdh As cCommandHandler = cCommandHandler.GetInstance()
                Dim cmd As cCommand = cmdh.GetCommand("EnableEcotracer")

                If (cmd IsNot Nothing) Then
                    cmd.Tag = eTracerRunModeTypes.RunSpace
                    cmd.Invoke()
                    If (Me.m_core.ActiveEcotracerScenarioIndex <= 0) Then
                        Me.m_cbContaminantTracing.Checked = False
                    End If
                End If
            End If

            ' If tracer scenario loaded turn this on
            Me.m_bpConTracing.SetValue(Me.m_cbContaminantTracing.Checked)

            Me.UpdateControls()

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
            Dim scenarioDef As cEcospaceScenario = m_core.EcospaceScenarios(m_core.ActiveEcospaceScenarioIndex)
            ' Connect controls to core data
            Me.m_fpScenarioName = New cPropertyFormatProvider(Me.m_tbName, scenarioDef, eVarNameFlags.Name)
            Me.m_fpScenarioDescription = New cPropertyFormatProvider(Me.m_tbDescription, scenarioDef, eVarNameFlags.Description)
            Me.m_fpAuthor = New cPropertyFormatProvider(Me.m_tbAuthor, scenarioDef, eVarNameFlags.Author)
            Me.m_fpContact = New cPropertyFormatProvider(Me.m_tbContact, scenarioDef, eVarNameFlags.Contact)
        End Sub

#End Region ' Internals

    End Class

End Namespace
