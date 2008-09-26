'==============================================================================
'
' $Log: EcosimParameters.vb,v $
' Revision 1.1  2008/09/26 07:31:34  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.29  2008/08/14 19:06:10  jeroens
' Year and propertion boxes NumericUpDowns
'
' Revision 1.28  2008/08/10 01:43:07  jeroens
' Renamed PropertyFormatProvider
'
' Revision 1.27  2008/08/02 03:04:15  jeroens
' Renamed resources
'
' Revision 1.26  2008/07/21 18:26:44  jeroens
' Added Relaxation parameter
'
' Revision 1.25  2008/07/02 17:39:09  jeroens
' Fixed tracer checkbox handling
'
' Revision 1.24  2008/06/02 00:01:33  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.23  2008/05/29 22:22:54  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.22  2008/05/13 16:23:27  jeroens
' Tracer enabled via generic command
'
' Revision 1.21  2008/05/08 17:51:33  jeroens
' Fixed bug 467
'
' Revision 1.20  2008/04/07 02:31:14  jeroens
' Cleaning up resources
'
' Revision 1.19  2008/03/03 16:06:40  joeb
' Made ConSim flag specific to Ecosim and Ecospace
'
' Revision 1.18  2008/01/08 19:07:28  jeroens
' Contracing flags now on all parameter pages
'
' Revision 1.17  2007/12/21 16:06:07  jeroens
' * PropertyFormatProvider offers refresh trigger
'
' Revision 1.16  2007/12/05 03:48:11  jeroens
' - Cleaned-up
'
' Revision 1.15  2007/12/04 02:21:47  jeroens
' * Added salinity FF
'
' Revision 1.14  2007/10/30 18:44:18  jeroens
' + Added Author, contact
'
' Revision 1.13  2007/10/19 15:29:24  jeroens
' * Fixed bug 319
'
' Revision 1.12  2007/10/15 15:25:51  jeroens
' * Filters incoming messages for appropriate source and type
'
' Revision 1.11  2007/10/12 16:06:15  jeroens
' + Original message passed to OnCoreDataChanged
'
' Revision 1.10  2007/10/12 15:21:53  joeb
' Changed N time to a property
'
' Revision 1.9  2007/10/10 19:31:53  jeroens
' * Restyled
' * FF number replaced by drop down combo
' * Form responds to proper code DataAddedOrRemoved triggers
'
' Revision 1.8  2007/09/28 19:33:21  joeb
' Number of years the model can run for
'
' Revision 1.7  2007/09/20 16:06:15  joeb
' Moved Summary time period data to Ecosim results
'
' Revision 1.6  2007/09/19 22:14:49  joeb
' Added Ecosim Summary time periods
'
' Revision 1.5  2007/09/17 21:08:06  joeb
' Added Predict Effort
'
' Revision 1.4  2007/09/12 16:05:30  jeroens
' + Added scenario name, description
'
' Revision 1.3  2007/07/30 17:40:37  jeroens
' - Removed apply TS button
' * Fixed layout, keyboard shortcuts
'
' Revision 1.2  2007/06/11 04:21:20  jeroens
' * Uses renamed dlgApplyTimeSeries
'
' Revision 1.1  2007/06/06 01:42:43  jeroens
' * Renamed Ecosim "Run info" to "Ecosim parameters"
'
' Revision 1.10  2007/05/16 19:52:54  joeb
' Added Contaminant tracing On/Off check box
'
' Revision 1.9  2007/04/24 23:15:43  fgao
' Add temporary Read Time Series button..
'
' Revision 1.8  2007/04/11 17:08:15  jeroens
' * Replaced EwETextBox by EwEFormatProvider
'
'==============================================================================

#Region "Imports Directive"

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    Public Class EcosimParameters

        Private m_core As cCore = Nothing
        Private m_fpScenarioName As cEwEFormatProvider = Nothing
        Private m_fpScenarioDescription As cEwEFormatProvider = Nothing
        Private m_fpAuthor As cEwEFormatProvider = Nothing
        Private m_fpContact As cEwEFormatProvider = Nothing
        Private m_fpNumYears As cEwEFormatProvider = Nothing
        Private m_fpNutBaseFreeProp As cEwEFormatProvider = Nothing
        Private m_fpNutrientForceNumber As cEwEFormatProvider = Nothing
        Private m_fpSalinityForceNumber As cEwEFormatProvider = Nothing
        Private m_fpPredictEffort As cEwEFormatProvider = Nothing
        Private m_fpRelaxation As cEwEFormatProvider = Nothing

        Private m_propConTracing As cBooleanProperty = Nothing

        Public Sub New()
            InitializeComponent()
            ' Set core
            Me.m_core = cCore.GetInstance()
        End Sub

        Public Sub New(ByVal strText As String)

            Me.New()

            'Set tab text
            Me.TabText = strText
            ' Set the windows text
            Me.Text = strText

        End Sub

#Region " Events "

        Private Sub EcosimParams_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim ecosimModelParams As cEcoSimModelParameters = m_core.EcoSimModelParameters()
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            Me.m_fpNumYears = New cPropertyFormatProvider(Me.m_nudNumberYears, ecosimModelParams, eVarNameFlags.EcoSimNYears)
            Me.m_fpNutBaseFreeProp = New cPropertyFormatProvider(Me.m_nudNutBaseFreeProp, ecosimModelParams, eVarNameFlags.NutBaseFreeProp)

            Me.m_fpNutrientForceNumber = New cPropertyFormatProvider(Me.cmbNutForcing, ecosimModelParams, eVarNameFlags.NutForceFunctionNumber)
            Me.m_fpSalinityForceNumber = New cPropertyFormatProvider(Me.cmbSalinityForcing, ecosimModelParams, eVarNameFlags.SalinityForceFunctionNumber)
            Me.m_fpPredictEffort = New cPropertyFormatProvider(Me.chkPredictEffort, ecosimModelParams, eVarNameFlags.PredictEffort)
            Me.m_fpRelaxation = New cPropertyFormatProvider(Me.m_nudRelaxation, ecosimModelParams, eVarNameFlags.Relaxation)

            Me.m_propConTracing = DirectCast(pm.GetProperty(ecosimModelParams, eVarNameFlags.ConSimOnEcoSim), cBooleanProperty)
            AddHandler Me.m_propConTracing.PropertyChanged, AddressOf OnConTracingChanged

            ' Listen to shapes data added or removed messages
            Me.MessageSources = New eMessageSource() {eMessageSource.ShapesManager, eMessageSource.EcoSim}

            Me.UpdateFFFormatProviders()
            Me.RebuildScenarioFormatProviders()
            Me.UpdateControls()

        End Sub

        Private Sub EcosimParameters_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            ' Clean up
            Me.MessageSources = Nothing

            RemoveHandler Me.m_propConTracing.PropertyChanged, AddressOf OnConTracingChanged
            Me.m_propConTracing = Nothing

            Me.m_fpScenarioName = Nothing
            Me.m_fpScenarioDescription = Nothing
            Me.m_fpAuthor = Nothing
            Me.m_fpContact = Nothing
            Me.m_fpNumYears = Nothing
            Me.m_fpNutBaseFreeProp = Nothing
            Me.m_fpNutrientForceNumber = Nothing
            Me.m_fpSalinityForceNumber = Nothing
            Me.m_fpPredictEffort = Nothing

        End Sub

        Dim m_bInUpdate As Boolean = False

        Private Sub chkConTracing_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkConTracing.Click

            If m_bInUpdate = True Then Return

            m_bInUpdate = True

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("EnableEcotracer")
            If (cmd IsNot Nothing) Then
                If (Me.chkConTracing.Checked) Then
                    cmd.Tag = eTracerRunModeTypes.RunSim
                Else
                    cmd.Tag = eTracerRunModeTypes.Disabled
                End If
                cmd.Invoke()
                If (Me.m_core.ActiveEcotracerScenarioIndex <= 0) Then
                    Me.chkConTracing.Checked = False
                End If
            End If

            ' If tracer scenario loaded turn this on
            Me.m_propConTracing.SetValue(Me.chkConTracing.Checked)

            m_bInUpdate = False

        End Sub

        Private Sub OnConTracingChanged(ByVal p As cProperty, ByVal cf As cProperty.eChangeFlags)
            Me.UpdateControls()
        End Sub

#End Region ' Events

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As cMessage)
            If ((msg.Source = eMessageSource.ShapesManager) And (msg.Type = eMessageType.DataAddedOrRemoved)) Then
                Me.UpdateFFFormatProviders()
            End If

            If msg.Source = eMessageSource.EcoSim And msg.Type = eMessageType.DataAddedOrRemoved Then
                Me.RebuildScenarioFormatProviders()
            End If
        End Sub

#End Region ' Overrides

#Region " Internals "

        Private Sub RebuildScenarioFormatProviders()
            Dim scenarioDef As cEcoSimScenario = m_core.EcosimScenarios(m_core.ActiveEcosimScenarioIndex)
            Me.m_fpScenarioName = New cPropertyFormatProvider(Me.m_tbName, scenarioDef, eVarNameFlags.Name)
            Me.m_fpScenarioDescription = New cPropertyFormatProvider(Me.m_tbDescription, scenarioDef, eVarNameFlags.Description)
            Me.m_fpAuthor = New cPropertyFormatProvider(Me.m_tbAuthor, scenarioDef, eVarNameFlags.Author)
            Me.m_fpContact = New cPropertyFormatProvider(Me.m_tbContact, scenarioDef, eVarNameFlags.Contact)
        End Sub

        Private Sub UpdateFFFormatProviders()
            ' Assemble list of FFs
            Dim ffm As cForcingFunctionManager = Me.m_core.ForcingShapeManager()
            Dim aItems(ffm.Count) As Object

            aItems(0) = My.Resources.GENERIC_VALUE_NONE
            For iFF As Integer = 0 To ffm.Count - 1
                aItems(iFF + 1) = ffm(iFF)
            Next
            Me.m_fpNutrientForceNumber.Items = aItems
            Me.m_fpSalinityForceNumber.Items = aItems
        End Sub

        Private Sub UpdateControls()
            Me.chkConTracing.Checked = CBool(Me.m_propConTracing.GetValue())
        End Sub

#End Region ' Internals

    End Class

End Namespace
