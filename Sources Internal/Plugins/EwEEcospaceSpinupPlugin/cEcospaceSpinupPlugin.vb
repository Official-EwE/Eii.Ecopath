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
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Ecopath
Imports EwECore.Ecosim
Imports EwEPlugin
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Controls

#End Region

#Const USE_LICENSE_LIB = 1

Public Class cEcospaceSpinupPlugin
    Implements EwEPlugin.IPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IEcospaceInitializedPlugin
    Implements EwEPlugin.IEcospaceEndTimestepPlugin
    Implements EwEPlugin.IEcospaceInitRunStartedPlugin
    Implements EwEPlugin.IEcospaceInitRunCompletedPlugin
    Implements EwEPlugin.IEcospaceRunCompletedPlugin
    Implements EwEPlugin.IAutoRunPlugin
    Implements EwEPlugin.ILicensePlugin

#Region "Events sent out by Plugin to an Interface"

    Public Event OnEcospaceTimeStep()
    Public Event OnEcospaceRunStarting()
    Public Event OnEcospaceRunCompleted()

#End Region

#Region " Local variables"

    ''' <summary>The core that this plug-in can use</summary>
    Private m_core As cCore

    Private m_EcoPath As cEcopathModel
    Private m_EcoSim As cEcosimModel
    Private m_EcoSpace As cEcoSpace
    Private m_EcoSpaceData As cEcospaceDataStructures

    Private m_uic As cUIContext = Nothing
    Private m_form As frmEcospaceSpinup = Nothing

    Private m_bUseSpinUpBase As Boolean = False

#End Region

#Region " Public Methods and properties "

    ' ToDo: move this field to datasource too, and make persistent

    Public Property UseSpinUpBaseBio As Boolean
        Get
            Try
                Return Me.m_bUseSpinUpBase
            Catch ex As Exception
                Me.LogMessage(ex)
            End Try
            Return False
        End Get
        Set(value As Boolean)
            Try
                Me.m_bUseSpinUpBase = value
                If Me.HasMainForm() Then
                    Me.m_form.SettingsChanged()
                End If
            Catch ex As Exception
                Me.LogMessage(ex)
            End Try
        End Set
    End Property

    Public nTimeSteps As Integer
    Public BtBtMinus1() As Single
    ' Public SS As Double
    Public BtB0() As Single
    Public BioAtTime() As Single
    Public BioAtBase() As Single

#End Region ' Public Methods and properties

#Region " Private methods "

    Public Sub LogMessage(ex As Exception, Optional msg As String = "")
        Try
            cLog.Write(ex, msg)
            Me.LogMessage(ex.Message)
        Catch x As Exception

        End Try
    End Sub

    Public Sub LogMessage(msg As String)
        Try
            System.Console.WriteLine(Me.ToString + " " + msg)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub fireOnTimeStep()
        Try
            RaiseEvent OnEcospaceTimeStep()
        Catch ex As Exception
            Me.LogMessage(ex, "Failed to send OnEcospaceTimeStep() Event to interface.")
        End Try
    End Sub


    Private Sub fireOnRunCompleted()
        Try
            ' Done
            RaiseEvent OnEcospaceRunCompleted()
        Catch ex As Exception
            Me.LogMessage(ex, "Failed to send OnEcospaceTimeStep() Event to interface.")
        End Try
    End Sub

    Private Sub fireOnRunStarting()
        Try
            RaiseEvent OnEcospaceRunStarting()
        Catch ex As Exception
            Me.LogMessage(ex, "Failed to send fireOnRunStarting() Event to interface.")
        End Try
    End Sub

#End Region ' Private methods

#Region "Ecopath, Ecosim and Ecospace events"

    Public Sub OnEcospaceInitRunStarted(EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitRunStartedPlugin.EcospaceInitRunStarted

        Try
            ' This is the correct moment to tell Ecospace to start using the SpinUp period
            Me.EcoSpaceData.UseSpinUpBase = Me.UseSpinUpBaseBio

        Catch ex As Exception
            Me.LogMessage(ex, "Exception initializing OnEcospaceInitRunStarted.")
        End Try

    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Try
            'Me.SS = 0
            ReDim Me.BtBtMinus1(Me.EcoSpaceData.NGroups)
            ReDim Me.BtB0(Me.EcoSpaceData.NGroups)
            ReDim Me.BioAtTime(Me.EcoSpaceData.NGroups)
            ReDim Me.BioAtBase(Me.EcoSpaceData.NGroups)

            Me.fireOnRunStarting()

        Catch ex As Exception
            Me.LogMessage(ex, "Exception initializing EcospaceSpinupPlugin.")
        End Try
    End Sub

    Public Sub OnEcospaceEndTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements EwEPlugin.IEcospaceEndTimestepPlugin.EcospaceEndTimeStep

        Try

            If Me.EcoSpaceData.bInSpinUp Then
                'In a SpinUp Period

                Array.Clear(Me.BioAtTime, 0, Me.BioAtTime.Length)
                Array.Clear(Me.BioAtBase, 0, Me.BioAtBase.Length)

                Array.Clear(Me.BtBtMinus1, 0, Me.BtBtMinus1.Length)
                Array.Clear(Me.BtB0, 0, Me.BtB0.Length)

                'Squared log relative error
                Dim BtBt1 As Single
                'Biomass at the current timestep
                Dim Bt As Single
                Dim BtMinus1 As Single = 1
                'Biomass at base timestep
                Dim B0 As Single
                For igrp As Integer = 1 To Me.EcoSpaceData.NGroups
                    'Biomass at the current time step
                    Bt = Me.EcoSpaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, igrp, iTime)
                    If iTime > 1 Then
                        BtMinus1 = Me.EcoSpaceData.ResultsByGroup(EwECore.eSpaceResultsGroups.Biomass, igrp, iTime - 1)
                    Else
                        BtMinus1 = Bt
                    End If
                    'Biomass at zero time step
                    B0 = Me.EcoSpaceData.SpinUpBBase(igrp)

                    Me.BtB0(igrp) = Bt / B0
                    Me.BtB0(0) += Me.BtB0(igrp)

                    BtBt1 = Bt / BtMinus1  'Math.Log(Bt / B0) ^ 2

                    Me.BtBtMinus1(igrp) += BtBt1
                    'slre summed across all the groups
                    Me.BtBtMinus1(0) += BtBt1

                    Me.BioAtBase(igrp) = B0
                    Me.BioAtTime(igrp) = Bt

                    'sum into the zero index
                    Me.BioAtBase(0) += B0
                    Me.BioAtTime(0) += Bt


                Next

                'sum across all the groups into zero index
                Me.BtB0(0) = Me.BtB0(0) / Me.EcoSpaceData.NGroups ' (BioAtTime(0) - BioAtBase(0)) / BioAtBase(0) * 100
                Me.BtBtMinus1(0) = Me.BtBtMinus1(0) / Me.EcoSpaceData.NGroups
                Me.fireOnTimeStep()

            End If

        Catch ex As Exception
            Me.LogMessage(ex, "Exception on Ecospace timestep.")
        End Try

    End Sub

    Public Sub EcospaceRunCompleted(EcoSpaceDatastructures As Object) Implements EwEPlugin.IEcospaceRunCompletedPlugin.EcospaceRunCompleted
        Try
            Me.fireOnRunCompleted()
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Every plug-in is told to initialize to the EwE core as soon as it is loaded. 
    ''' Typically, plug-ins use this opportunity to store a reference to the core
    ''' for later use.
    ''' </summary>
    ''' <param name="CoreAsObject">The core, casted to a generic object</param>
    Public Sub Initialize(CoreAsObject As Object) Implements EwEPlugin.IPlugin.Initialize
        Try
            Me.m_core = DirectCast(CoreAsObject, cCore)
        Catch ex As Exception
            Me.LogMessage(ex)
        End Try
    End Sub

    ''' <summary>
    ''' Plug-in point that is called when the core has initialized its models
    ''' Ecopath, Ecosim and Ecospace. This is the only opportunity for plug-ins to grab 
    ''' references to these models.
    ''' </summary>
    ''' <param name="EcopathAsObject"></param>
    ''' <param name="EcoSimAsObject"></param>
    ''' <param name="EcoSpaceAsObject"></param>
    Public Sub CoreInitialized(ByRef EcopathAsObject As Object, ByRef EcoSimAsObject As Object, ByRef EcoSpaceAsObject As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized
        Try

            Me.m_EcoPath = TryCast(EcopathAsObject, cEcopathModel)
            Me.m_EcoSim = TryCast(EcoSimAsObject, cEcosimModel)
            Me.m_EcoSpace = TryCast(EcoSpaceAsObject, cEcoSpace)

            Debug.Assert((Me.m_EcoPath IsNot Nothing) And (Me.m_EcoSim IsNot Nothing) And (Me.m_EcoSpace IsNot Nothing),
                         Me.ToString + ".CoreInitialized() Failed to initialize data.")

        Catch ex As Exception
            Me.LogMessage(ex)
        End Try

    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements EwEPlugin.IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_EcoSpaceData = TryCast(EcospaceDatastructures, cEcospaceDataStructures)
        Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcospaceInitialized() Failed to get EcosimDataStructures.")
    End Sub

#End Region

#Region " Datastructure access"

    Public ReadOnly Property Core As cCore
        Get
            Debug.Assert(Me.m_core IsNot Nothing, Me.ToString + ".Core() EwE Core has not been initialized correctly.")
            Return Me.m_core
        End Get
    End Property


    Public ReadOnly Property EcoSpaceData As cEcospaceDataStructures
        Get
            Debug.Assert(Me.m_EcoSpaceData IsNot Nothing, Me.ToString + ".EcoSpaceData() EcoSpace has not been initialized correctly.")
            Return Me.m_EcoSpaceData
        End Get
    End Property

#End Region

#Region " Autorun plug-in implementation "

    Public Function AutoRunTypes() As eCoreComponentType() Implements IAutoRunPlugin.AutoRunTypes
        Return New eCoreComponentType() {eCoreComponentType.Ecospace}
    End Function

    Public Property AutoRun(type As eCoreComponentType) As Boolean Implements IAutoRunPlugin.AutoRun
        Get
            Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
            If Not sm.HasEcospaceLoaded Then Return False
            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters
            If (parms Is Nothing) Then Return False
            Return parms.SpinupEnabled
        End Get
        Set(value As Boolean)
            Dim sm As cCoreStateMonitor = Me.Core.StateMonitor
            If Not sm.HasEcospaceLoaded Then Return
            Dim parms As cEcospaceModelParameters = Me.Core.EcospaceModelParameters
            If (parms Is Nothing) Then Return
            parms.SpinupEnabled = value
        End Set
    End Property

#End Region

#Region " User Interface plug-in implementation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User Interfaces require a UIContext, which provides not only access to
    ''' a running core, but also to a styleguide, command handler, and other
    ''' aspects that binds user interface elements in the EwE 6 application. 
    ''' </summary>
    ''' <param name="uic">The <see cref="cUIContext"/> to connect to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Try
            Me.m_uic = DirectCast(uic, cUIContext)
        Catch ex As Exception
            Me.m_uic = Nothing
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display in controls that provide access to 
    ''' this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DisplayName() As String Implements EwEPlugin.IGUIPlugin.DisplayName
        Get
            Return "Ecospace Spin-Up"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what image to show for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlImage() As Object Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            ' Use an image from the pool of shared resources
            Return Nothing 'ScientificInterfaceShared.My.Resources.fish
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 what text to display when the user hovers the mouse cursor
    ''' over a user interface element for this plug-in.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            ' Show the description as a tooltip text
            Return Me.Description
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Provide EwE6 with a method to execute when a user interface control for 
    ''' this plug-in is clicked by the user.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef form As Object) Implements EwEPlugin.IGUIPlugin.OnControlClick
        form = Me.GetMainForm
    End Sub

    Private Function HasMainForm() As Boolean
        If (Me.m_form IsNot Nothing) Then
            Return Not Me.m_form.IsDisposed
        End If
        Return False
    End Function

    Private Function GetMainForm() As frmEcospaceSpinup

#If USE_LICENSE_LIB Then
        Try
            If Not Me.m_core.License.IsRegistered Then
                Me.PromptForLicense()
            End If
            If Not Me.m_core.License.IsRegistered Then Return Nothing
        Catch ex As Exception
            cLog.Write(ex, "cEcospaceSpinUpPlugin.GetMainForm")
            Return Nothing
        End Try
#End If

        If Not Me.HasMainForm() Then
            Me.m_form = New frmEcospaceSpinup()
            Me.m_form.UIContext = Me.m_uic
            Me.m_form.Init(Me)
            ' Me.m_form.Text = "Ecospace fit"
        End If

        Return Me.m_form

    End Function

    Public Sub Expiry(ByRef dt As Date) Implements ILicensePlugin.Expiry
#If USE_LICENSE_LIB Then
        Try
            dt = Me.m_core.License.Expiry
        Catch ex As Exception
            cLog.Write(ex, "cEcospaceSpinUpPlugin.Expiry")
        End Try
#Else
        dt = Date.Now().AddDays(1)
#End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 when during application execution this plug-in should be accessible 
    ''' to users.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Tell EwE6 where to place an item in its navigation tree.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceOutput"
        End Get
    End Property

#End Region ' User Interface plug-in implementation

#Region "IPlugin implementation"

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Global Ocean Modeling unit, UBC Institute of the Oceans and Fisheries"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevtea@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Run Ecospace with a spin-up (or burn-in) period."
        End Get
    End Property

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "EwEEcospaceSpinUpPlugin"
        End Get
    End Property

#End Region

#Region " License "

    Friend Sub PromptForLicense()
        If (Me.m_uic Is Nothing) Then Return
        Try
            Dim cmd As cEnterLicenseCommand = CType(Me.m_uic.CommandHandler.GetCommand(cEnterLicenseCommand.cCOMMAND_NAME), cEnterLicenseCommand)
            If (cmd IsNot Nothing) Then cmd.Invoke()
        Catch ex As Exception
            cLog.Write(ex)
        End Try
    End Sub

#End Region ' License

End Class

