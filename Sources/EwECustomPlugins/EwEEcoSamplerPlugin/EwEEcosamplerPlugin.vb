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

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Samples
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Logging
Imports Microsoft.Extensions.Logging
Imports Debug = System.Diagnostics.Debug

#End Region ' Imports

Public Class EwEEcosamplerPlugin
    Implements IMonteCarloPlugin
    Implements IEcosimInitializedPlugin
    Implements IUIContextPlugin
    Implements INavigationTreeItemPlugin
    Implements IMenuItemPlugin
    Implements IDockStatePlugin
    Implements ISaveFilterPlugin
    Implements ISearchPlugin
    Implements IHelpPlugin

#Region " Internal vars "

    Private m_uic As cUIContext = Nothing
    Private m_sampleman As cEcopathSampleManager = Nothing
    Private m_montecarlo As cEcosimMonteCarlo = Nothing
    Private m_esdata As cEcosimDatastructures = Nothing
    Private m_iNumRecordedSamples As Integer = 0
    Private m_iNumRejectedSamples As Integer = 0
    Private m_ui As frmSamples = Nothing

    Private m_strBaseHash As String = ""

    Private m_bValidateRespirationOrg As Boolean = False

#End Region ' Internal vars

#Region " Generic plugin "

    Public ReadOnly Property Author As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "EwE development team"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property DisplayName As String Implements EwEPlugin.IPlugin.DisplayName
        Get
            Return My.Resources.DISPLAYNAME
        End Get
    End Property

    Public ReadOnly Property Description As String Implements EwEPlugin.IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public Sub Initialize(core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' NOP
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "MCRecorder"
        End Get
    End Property

#End Region ' Generic plugin

#Region " UI plugin "

    Public Sub UIContext(uic As Object) _
        Implements EwEPlugin.IUIContextPlugin.UIContext
        Me.m_uic = DirectCast(uic, cUIContext)
        If (uic IsNot Nothing) Then
            Me.m_sampleman = Me.m_uic.Core.SampleManager()
        ElseIf (Me.m_sampleman IsNot Nothing) Then
            Me.m_sampleman.Dispose()
        End If
    End Sub

#End Region ' UI plugin

#Region " Dockstate plugin "

    Public Function DockState() As Integer _
        Implements EwEPlugin.IDockStatePlugin.DockState
        Return If(cSystemUtils.IsRightToLeft,
                                WeifenLuo.WinFormsUI.Docking.DockState.DockLeft,
                                WeifenLuo.WinFormsUI.Docking.DockState.DockRight)
    End Function

#End Region ' Dockstate plugin

#Region " Save filter plugin "

    Public Sub CoreInitialized(ByRef objEcoPath As Object, ByRef objEcoSim As Object, ByRef objEcoSpace As Object) _
        Implements EwEPlugin.ICorePlugin.CoreInitialized
        ' NOP
    End Sub

    Public Function DiscardChanges(ByRef bCancel As Boolean) As Boolean _
        Implements EwEPlugin.ISaveFilterPlugin.DiscardChanges
        ' NOP
        Return True
    End Function

    Public Function SaveChanges(ByRef bCancel As Boolean) As Boolean _
        Implements EwEPlugin.ISaveFilterPlugin.SaveChanges

        ' Restore original model. Should the user be asked?
        If (Me.m_sampleman.IsLoaded) Then
            Me.m_sampleman.Load(Nothing, True)
        End If

        ' JS 5 April 16: Disabled automatic sample invalidation while hash keys differ due to minute numerical differences
        ' bCancel = (Me.m_sampleman.CanSaveModel() = False)

        Return True

    End Function

#End Region ' Save filter plugin

#Region " Menu plugin "

    Public ReadOnly Property ControlImage As Object _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.DeltaHS
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathLoaded
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As Object) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick

        frmPlugin = Me.GetUI()

    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTools"
        End Get
    End Property

#End Region ' Menu plugin

#Region " Ecosim plugin "

    Public Sub EcosimInitialized(EcosimDatastructures As Object) _
        Implements IEcosimInitializedPlugin.EcosimInitialized

        Me.m_esdata = DirectCast(EcosimDatastructures, cEcosimDatastructures)

    End Sub

#End Region ' Ecosim plug-in

#Region " Monte Carlo plugin "

    Public Sub MontCarloInitialized(MonteCarloAsObject As Object) _
        Implements EwEPlugin.IMonteCarloPlugin.MontCarloInitialized

        Me.m_montecarlo = DirectCast(MonteCarloAsObject, cEcosimMonteCarlo)

    End Sub

    Public Sub MonteCarloRunInitialized() _
        Implements EwEPlugin.IMonteCarloPlugin.MonteCarloRunInitialized

        If (Not Me.IsRecording) Then Return

        ' Force MCMC to validate respiration. Remember the old flag though
        Me.m_bValidateRespirationOrg = Me.m_montecarlo.ValidateRespiration
        Me.m_montecarlo.ValidateRespiration = True

        Me.m_sampleman.StartRecording()

    End Sub

    Private m_sampleCurrent As cEcopathSample = Nothing

    Public Sub MonteCarloBalancedEcopathModel(TrialNumber As Integer, nIterations As Integer) _
        Implements EwEPlugin.IMonteCarloPlugin.MonteCarloBalancedEcopathModel

        If (Not Me.IsRecording) Then Return

        Try
            ' Note that the SS has not been calculated here!!
            Me.m_sampleCurrent = Me.m_sampleman.Record(Me.m_strBaseHash)
            If (Me.m_sampleCurrent IsNot Nothing) Then
                Me.m_iNumRecordedSamples += 1
            Else
                Me.m_iNumRejectedSamples += 1
            End If
        Catch ex As Exception
            Dim msg As New cMessage(My.Resources.RECORD_ERROR,
                                eMessageType.DataExport, eCoreComponentType.DataSource,
                                eMessageImportance.Warning)
            Me.m_uic.Core.Messages.AddMessage(msg)
            Me.IsRecording = False
            cLog.Write(ex, "EwESampleRecorderPlugin.MonteCarloBalancedEcopathModel")
        End Try

    End Sub

    Public Sub MonteCarloEcosimRunCompleted() _
        Implements EwEPlugin.IMonteCarloPlugin.MonteCarloEcosimRunCompleted

        If (Not Me.IsRecording) Then Return

        ' Capture SS and store it
        If (Me.m_sampleCurrent IsNot Nothing) Then
            Me.m_sampleman.StoreEcosimDiagnostics(Me.m_sampleCurrent, Me.m_montecarlo, Me.m_esdata)
            Me.m_sampleCurrent = Nothing
        End If


    End Sub

    Public Sub MonteCarloRunCompleted() _
        Implements EwEPlugin.IMonteCarloPlugin.MonteCarloRunCompleted

        If (Not Me.IsRecording) Then Return

        ' Restore MCMC respiration validation to original state
        Me.m_montecarlo.ValidateRespiration = Me.m_bValidateRespirationOrg
        Me.m_bValidateRespirationOrg = False
        Me.m_uic.Core.SaveChanges(True)
        Me.m_sampleman.StopRecording()

        Try
            Dim msg As New cMessage(cStringUtils.Localize(My.Resources.RECORD_REPORT, Me.m_iNumRecordedSamples, Me.m_iNumRejectedSamples),
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)
        Catch ex As Exception
            cLog.Write(ex, "EwESampleRecorderPlugin.MonteCarloRunCompleted")
        End Try

        Me.m_iNumRecordedSamples = 0
        Me.m_iNumRejectedSamples = 0

    End Sub

#End Region ' Monte Carlo plugin

#Region " Search plugin "

    Public Sub SearchInitialized(SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.SearchInitialized
        ' NOP
    End Sub

    Public Sub SearchIterationsStarting() _
        Implements EwEPlugin.ISearchPlugin.SearchIterationsStarting

        ' Make sure to unload any loaded sample
        Me.m_sampleman.Load(Nothing, False)

        If (Not Me.IsRecording) Then Return

        ' Reset samples count
        Me.m_iNumRecordedSamples = 0
        Me.m_iNumRejectedSamples = 0
        ' Grab model hash key that will be stored with recorded samples
        Me.m_strBaseHash = Me.m_sampleman.ModelHash

    End Sub

    Public Sub PostRunSearchResults(SearchDatastructures As Object) _
        Implements EwEPlugin.ISearchPlugin.PostRunSearchResults
        ' NOP
    End Sub

    Public Sub SearchCompleted(SearchDatastructures As Object) Implements EwEPlugin.ISearchPlugin.SearchCompleted

        If (Not Me.IsRecording) Then Return

        '#If DEBUG Then
        '        If (Me.m_sampleman.ModelHash <> Me.m_strBaseHash) Then
        '            Debug.Assert(False, "Significant differents in Ecopath params before and after MC run")
        '        End If
        '#End If

    End Sub

#End Region ' Search plugin

#Region " Help plugin "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IHelpPlugin.HelpTopic"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpTopic As String _
        Implements EwEPlugin.IHelpPlugin.HelpTopic
        Get
            Return ".\UserGuide\EcoSampler.pdf"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IHelpPlugin.HelpURL"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HelpURL As String _
        Implements EwEPlugin.IHelpPlugin.HelpURL
        Get
            Return Me.HelpTopic
        End Get
    End Property

#End Region ' Help plugin

#Region " Regional bits "

    Friend Property IsRecording As Boolean = False

    Friend ReadOnly Property NumRecordedSamples As Integer
        Get
            Return Me.m_iNumRecordedSamples
        End Get
    End Property

    Friend ReadOnly Property SampleManager As cEcopathSampleManager
        Get
            Return Me.m_sampleman
        End Get
    End Property

    Public ReadOnly Property MenuItemLocation As String Implements IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' Regional bits

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_ui Is Nothing) Then Return False
        Return Not Me.m_ui.IsDisposed
    End Function

    Private Function GetUI() As System.Windows.Forms.Form
        If Not Me.HasUI() Then
            Me.m_ui = New frmSamples(Me.m_uic, Me)
        End If
        Return Me.m_ui
    End Function

#End Region ' Internals

End Class
