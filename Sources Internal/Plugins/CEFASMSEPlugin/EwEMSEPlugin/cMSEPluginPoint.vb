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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

Option Strict On
Option Explicit On

#Region " Imports "

Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls

#End Region ' Imports


Public Class cMSEPluginPoint
    Implements EwEPlugin.IMenuItemPlugin
    Implements EwEPlugin.ICorePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimBeginTimestepPlugin
    Implements EwEPlugin.IMessageFilterPlugin
    Implements EwEPlugin.IEcopathPlugin
    Implements EwEPlugin.IEcosimPlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IMSEInitialized
    Implements EwEPlugin.IEcosimDataInitializedPlugin


#Region " Internal vars "

    Private m_MSE As cMSE

    Private MSEForm As frmMSE = Nothing
    Private mCore As cCore = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_ecosim As EwECore.Ecosim.cEcoSimModel = Nothing
    Private m_ecopath As Ecopath.cEcoPathModel
    Private m_simdata As cEcosimDatastructures
    Private m_pathdata As cEcopathDataStructures
    Private m_coreMSEData As EwECore.MSE.cMSEDataStructures

    Private m_EcosimTimeStepDelegate As EwECore.Ecosim.EcoSimTimeStepDelegate

    Private m_monitor As New cMSEStateMonitor(Me)

    Private m_mhSettings As cMessageHandler = Nothing
    Private m_mhEcosim As cMessageHandler = Nothing

#End Region ' Internal vars

#Region "Public Properties"

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the <see cref="cMSEStateMonitor">MSE state monitor</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Monitor As cMSEStateMonitor
        Get
            Return Me.m_monitor
        End Get
    End Property

    Public ReadOnly Property MSE As cMSE
        Get
            Return Me.m_MSE
        End Get
    End Property

    Public Sub UIContext(ByVal uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
    End Sub

    Friend ReadOnly Property Core As cCore
        Get
            Return Me.mCore
        End Get
    End Property

#End Region 'Public Properties

#Region " Construction "

    Public Sub New()
        m_MSE = New cMSE(m_monitor, Me)
        Me.InvalidateConfiguration()
    End Sub

#End Region ' Construction

#Region " Diagnostics and state management "

    Friend Sub InvalidateConfiguration()

        Me.MSE.InvalidateData()
        Me.m_monitor.Invalidate()

    End Sub

#End Region ' Diagnostics and state management

#Region " EwE app flow plugins "

    Public Sub onEcosimBeginTimeStep(ByRef BiomassAtTimestep() As Single, ByVal EcosimDatastructures As Object, ByVal iTime As Integer) Implements EwEPlugin.IEcosimBeginTimestepPlugin.EcosimBeginTimeStep

        Try
            Me.MSE.onEcosimBeginTimeStep(BiomassAtTimestep, iTime)
        Catch ex As Exception

        End Try

    End Sub

    Public Function CloseModel() As Boolean Implements EwEPlugin.IEcopathPlugin.CloseModel
        ' NOP
        Return True
    End Function

    Public Function LoadModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.LoadModel
        Me.InvalidateConfiguration()
        Return True
    End Function

    Public Function SaveModel(dataSource As Object) As Boolean Implements EwEPlugin.IEcopathPlugin.SaveModel
        Return True
    End Function

    Public Sub CloseEcosimScenario() Implements EwEPlugin.IEcosimPlugin.CloseEcosimScenario
        ' NOP
    End Sub

    Public Sub LoadEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.LoadEcosimScenario
        Me.InvalidateConfiguration()
    End Sub

    Public Sub SaveEcosimScenario(dataSource As Object) Implements EwEPlugin.IEcosimPlugin.SaveEcosimScenario
        ' NOP
    End Sub

    Public Sub onInitialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        Me.mCore = CType(core, cCore)
        Units.Init(mCore)
    End Sub

    Public Sub onCoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) Implements EwEPlugin.ICorePlugin.CoreInitialized

        m_ecopath = CType(objEcoPath, Ecopath.cEcoPathModel)
        m_ecosim = CType(objEcoSim, Ecosim.cEcoSimModel)

        Debug.Assert(Me.m_uic IsNot Nothing)

        Me.MSE.onCoreInitialized(Me.mCore, m_ecopath, m_ecosim)

        ' Set message handlers

        Me.m_mhSettings = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.Core, eMessageType.GlobalSettingsChanged, Me.m_uic.SyncObject)
        Me.m_mhEcosim = New cMessageHandler(AddressOf OnCoreMessage, eCoreComponentType.EcoSim, eMessageType.DataAddedOrRemoved, Me.m_uic.SyncObject)

#If DEBUG Then
        Me.m_mhSettings.Name = "CefasMSE_mhSettings"
        Me.m_mhEcosim.Name = "CefasMSE_mhEcosim"
#End If

    End Sub

    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) _
                Implements EwEPlugin.IEcopathRunInitializedPlugin.EcopathRunInitialized

        Me.m_pathdata = DirectCast(EcopathDataAsObject, cEcopathDataStructures)
        Me.MSE.onEcopathInitialized(Me.m_pathdata)

    End Sub

    Public Sub onEcosimInitialized(ByVal EcosimDatastructures As Object) Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Debug.Assert(TypeOf EcosimDatastructures Is cEcosimDatastructures, "EcosimInitialized() failed to pass in valid Ecosim Data!")

        If TypeOf EcosimDatastructures Is cEcosimDatastructures Then

            Me.m_simdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)
            Me.MSE.onEcosimInitialized(Me.m_simdata)

        End If

    End Sub

    Public Sub MSEInitialized(MSEModel As Object, MSEDataStructure As Object, EcosimDatastructures As Object) Implements EwEPlugin.IMSEInitialized.MSEInitialized
        Try
            m_coreMSEData = DirectCast(MSEDataStructure, MSE.cMSEDataStructures)
            Me.m_MSE.CoreMSEData = Me.m_coreMSEData
        Catch ex As Exception
            cLog.Write(ex, "MSEInitialized(...) Failed to cast MSEDataStructure to cMSEDataStructures.")
        End Try

    End Sub


    Public Sub EcosimPreDataInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreDataInitialized

    End Sub

    Public Sub EcosimPreRunInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimDataInitializedPlugin.EcosimPreRunInitialized
        Try
            Dim data As cEcosimDatastructures = DirectCast(EcosimDatastructures, cEcosimDatastructures)
            Me.m_MSE.onEcosimRunBeginning(data)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        If Not Me.HasUI Then
            MSEForm = New frmMSE(Me, Me.m_uic)
        End If

        ' Let EwE show the form
        frmPlugin = MSEForm

    End Sub

#End Region ' EwE app flow plugins

#Region "Plugin Implementations"


    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return My.Resources.CAPTION_TOOLTIP
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            'Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
            Return EwEUtils.Core.eCoreExecutionState.EcosimLoaded
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevlowestoft@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in to run CEFAS MSE"
        End Get
    End Property


    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "ndCefasMSE"
        End Get
    End Property

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

#End Region

#Region " Helper methods "

    Private Function HasUI() As Boolean
        If Me.MSEForm Is Nothing Then Return False
        Return Not Me.MSEForm.IsDisposed
    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cEcoPathGroupInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cEcoPathGroupInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present groups.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveGroup(strName As String, iIndex As Integer) As cEcoPathGroupInput
        If (iIndex < 1) Or (iIndex > Me.Core.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.Core.EcoPathGroupInputs(iIndex)
        If String.Compare(grp.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cFleetInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cFleetInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present fleets.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveFleet(strName As String, iIndex As Integer) As cFleetInput
        If (iIndex < 1) Or (iIndex > Me.Core.nFleets) Then Return Nothing
        Dim flt As cFleetInput = Me.Core.FleetInputs(iIndex)
        If String.Compare(flt.Name, strName, True) <> 0 Then
            Return Nothing
        End If
        Return flt
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Notify the user of an event.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="importance"></param>
    ''' <param name="strHyperlink"></param>
    ''' -----------------------------------------------------------------------
    Friend Sub InformUser(strMessage As String, importance As eMessageImportance, _
                          Optional strHyperlink As String = "", _
                          Optional astrSubMessages As String() = Nothing)

        If (Me.Core Is Nothing) Then Return

        Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.External, importance)
        msg.Hyperlink = strHyperlink
        If (astrSubMessages IsNot Nothing) Then
            For Each strSubMessage As String In astrSubMessages
                msg.AddVariable(New cVariableStatus(eStatusFlags.OK, strSubMessage, eVarNameFlags.NotSet, eDataTypes.External, eCoreComponentType.External, 0))
            Next
        End If
        Me.Core.Messages.SendMessage(msg)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Ask the user a question.
    ''' </summary>
    ''' <param name="strMessage"></param>
    ''' <param name="style"></param>
    ''' <param name="importance"></param>
    ''' <param name="replyDefault"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Friend Function AskUser(strMessage As String, _
                            style As eMessageReplyStyle, _
                            Optional importance As eMessageImportance = eMessageImportance.Question, _
                            Optional replyDefault As eMessageReply = eMessageReply.OK) As eMessageReply

        If (Me.Core Is Nothing) Then Return replyDefault

        Dim fmsg As New cFeedbackMessage(strMessage, eCoreComponentType.External, eMessageType.Any, importance, style)
        fmsg.Reply = replyDefault
        Me.Core.Messages.SendMessage(fmsg)
        Return fmsg.Reply

    End Function

    Private Sub OnCoreMessage(ByRef msg As cMessage)

        Dim bRefresh As Boolean = False

        ' Refresh when Core settings have changed
        If (msg.Type = eMessageType.GlobalSettingsChanged) Then
            bRefresh = True
        End If

        ' Refresh upon ecosim scenario load
        If (msg.Type = eMessageType.DataAddedOrRemoved And msg.Source = eCoreComponentType.EcoSim) Then
            bRefresh = True
        End If

        If (bRefresh = True) Then
            Me.InvalidateConfiguration()
        End If

    End Sub

    Private Sub onPreProcessMessage(ByVal msg As EwEUtils.Core.IMessage, ByRef bCancelMessage As Boolean) _
        Implements EwEPlugin.IMessageFilterPlugin.PreProcessMessage

        ' JS 03Oct13: ONLY SUPPRESS MESSAGES WHEN MSE IS RUNNING! 
        If Not Me.MSE.IsRunning Then Return

        'Plugin Point called to cancel a message
        Select Case msg.Type

            Case eMessageType.Estimate_BA, _
                 eMessageType.Estimate_Net_Migration, _
                 eMessageType.EE
                Console.WriteLine("! MSE suppressed message " & msg.Message)
                bCancelMessage = True

            Case Else
                bCancelMessage = False

        End Select

    End Sub

#End Region ' Helper methods

End Class
