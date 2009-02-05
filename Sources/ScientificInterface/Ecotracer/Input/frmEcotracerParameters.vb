'==============================================================================
'
' $Log: frmEcotracerParameters.vb,v $
' Revision 1.3  2009/02/05 17:48:40  jeroens
' MessageSources -> CoreComponents
'
' Revision 1.2  2009/01/16 18:30:39  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:32:04  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/08/10 01:43:08  jeroens
' Renamed PropertyFormatProvider
'
' Revision 1.10  2008/06/02 00:01:35  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.9  2008/05/29 22:22:55  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.8  2008/03/18 17:25:55  jeroens
' Ecotracer command handles property juggling
'
' Revision 1.7  2008/03/17 14:46:32  jeroens
' Tracer run mode controlled via command
'
' Revision 1.6  2008/03/03 16:08:18  joeb
' Started Implementation of Ecospace output
'
' Revision 1.5  2008/01/08 19:07:28  jeroens
' Contracing flags now on all parameter pages
'
' Revision 1.4  2008/01/08 11:24:25  jeroens
' Merged input parms and group grid in one screen
'
' Revision 1.3  2008/01/03 17:40:41  joeb
' Renamed excretion rate column
'
' Revision 1.2  2007/12/21 15:37:11  jeroens
' * Connected to core messages
'
' Revision 1.1  2007/12/05 03:54:04  jeroens
' * Initial version
'
'==============================================================================

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands

Namespace Ecotracer

    Public Class frmEcotracerParameters

#Region "Private data"

        Private m_core As cCore = Nothing
        Private m_fpScenarioName As cEwEFormatProvider = Nothing
        Private m_fpScenarioDescription As cEwEFormatProvider = Nothing
        Private m_fpAuthor As cEwEFormatProvider = Nothing
        Private m_fpContact As cEwEFormatProvider = Nothing
        Private m_propEcosimConTracing As cBooleanProperty = Nothing
        Private m_propEcospaceConTracing As cBooleanProperty = Nothing

#End Region

#Region "Constructor"

        Public Sub New()
            InitializeComponent()
            Me.m_core = cCore.GetInstance()
        End Sub

#End Region

#Region " Events "

        Private Sub frmEcotracerParameters_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim scenarioDef As cEcotracerScenario = m_core.EcotracerScenarios(m_core.ActiveEcotracerScenarioIndex)

            Me.m_fpScenarioName = New cPropertyFormatProvider(Me.m_tbName, scenarioDef, eVarNameFlags.Name)
            Me.m_fpScenarioDescription = New cPropertyFormatProvider(Me.m_tbDescription, scenarioDef, eVarNameFlags.Description)
            Me.m_fpAuthor = New cPropertyFormatProvider(Me.m_tbAuthor, scenarioDef, eVarNameFlags.Author)
            Me.m_fpContact = New cPropertyFormatProvider(Me.m_tbContact, scenarioDef, eVarNameFlags.Contact)

            ' Try this
            Me.ConnectToEcosim(True)
            Me.ConnectToEcospace(True)

            Me.UpdateControls()

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSim, eCoreComponentType.EcoSpace}

        End Sub

        Private Sub frmEcotracerParameters_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

            Me.CoreComponents = Nothing

            Me.ConnectToEcosim(False)
            Me.ConnectToEcospace(False)

            ' Sanity checks
            Debug.Assert(Not Me.IsConnectedToEcosim())
            Debug.Assert(Not Me.IsConnectedToEcospace())

        End Sub

        Private Sub OnConTracingChanged(ByVal p As cProperty, ByVal cf As cProperty.eChangeFlags)
            Me.UpdateControls()
        End Sub

        Private Sub rbDisabled_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbDisabled.Click
            SetTracerRunMode(eTracerRunModeTypes.Disabled)
        End Sub


        Private Sub rbSim_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbSim.Click
            SetTracerRunMode(eTracerRunModeTypes.RunSim)
        End Sub

        Private Sub rbSpace_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbSpace.Click
            SetTracerRunMode(eTracerRunModeTypes.RunSpace)
        End Sub

#End Region ' Events

#Region " Overrides "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            If msg.Source = eCoreComponentType.EcoSim Then
                Me.ConnectToEcosim(Me.m_core.ActiveEcosimScenarioIndex > 0)
            End If
            If msg.Source = eCoreComponentType.EcoSpace Then
                Me.ConnectToEcospace(Me.m_core.ActiveEcospaceScenarioIndex > 0)
            End If
        End Sub

#End Region ' Overrides

#Region " Internals "

        Private Sub UpdateControls()
            'UpdateControls() is called by the Ecosim and Ecospace properties PropertyChanged event handler 
            'This means anytime another interface or the core changes one of these flags this will be called 

            If Me.IsConnectedToEcosim Then
                If CBool(Me.m_propEcosimConTracing.GetValue()) = True Then
                    Me.rbSim.Checked = True
                    Exit Sub
                End If
            End If

            If Me.IsConnectedToEcospace Then
                If CBool(Me.m_propEcospaceConTracing.GetValue()) = True Then
                    Me.rbSpace.Checked = True
                    Exit Sub
                End If
            End If

            Me.rbDisabled.Checked = True

        End Sub

        Private Sub ConnectToEcosim(ByVal bConnect As Boolean)

            If bConnect Then

                ' Already connected? Abort
                If (Me.IsConnectedToEcosim() = True) Then Return
                If (Me.m_core.ActiveEcosimScenarioIndex <= 0) Then Return

                Dim pm As cPropertyManager = cPropertyManager.GetInstance()
                Dim ecosimModelParams As cEcoSimModelParameters = m_core.EcoSimModelParameters()

                Me.m_propEcosimConTracing = DirectCast(pm.GetProperty(ecosimModelParams, eVarNameFlags.ConSimOnEcoSim), cBooleanProperty)
                AddHandler Me.m_propEcosimConTracing.PropertyChanged, AddressOf OnConTracingChanged

            Else

                ' Already disconnected? Abort
                If Not Me.IsConnectedToEcosim() Then Return

                RemoveHandler Me.m_propEcosimConTracing.PropertyChanged, AddressOf OnConTracingChanged
                Me.m_propEcosimConTracing = Nothing

            End If

        End Sub

        Private Sub ConnectToEcospace(ByVal bConnect As Boolean)

            If bConnect Then

                ' Already connected? Abort
                If Me.IsConnectedToEcospace() Then Return
                If (Me.m_core.ActiveEcospaceScenarioIndex <= 0) Then Return

                Dim pm As cPropertyManager = cPropertyManager.GetInstance()
                Dim ecospaceModelParams As cEcospaceModelParameters = m_core.EcospaceModelParameters()

                Me.m_propEcospaceConTracing = DirectCast(pm.GetProperty(ecospaceModelParams, eVarNameFlags.ConSimOnEcoSpace), cBooleanProperty)
                AddHandler Me.m_propEcospaceConTracing.PropertyChanged, AddressOf OnConTracingChanged

            Else

                ' Already disconnected? Abort
                If Not Me.IsConnectedToEcospace() Then Return

                RemoveHandler Me.m_propEcospaceConTracing.PropertyChanged, AddressOf OnConTracingChanged
                Me.m_propEcospaceConTracing = Nothing

            End If

        End Sub

        Private Function IsConnectedToEcosim() As Boolean
            Return (Me.m_propEcosimConTracing IsNot Nothing)
        End Function

        Private Function IsConnectedToEcospace() As Boolean
            Return (Me.m_propEcospaceConTracing IsNot Nothing)
        End Function

        Private Sub SetTracerRunMode(ByVal tracerRunMode As eTracerRunModeTypes)

            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("EnableEcotracer")

            cmd.Tag = tracerRunMode
            cmd.Invoke()

        End Sub

#End Region ' Internals

    End Class

End Namespace ' Ecotracer
