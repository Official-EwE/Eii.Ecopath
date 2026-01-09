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
' This plug-in was developed under the Safenet project, and has been contributed
' to the EwE approach by the Safenet project.
' 
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'
Imports EwECore
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecopath
Imports EwECore.Plugins.Ecospace
Imports EwECore.Plugins.UI
Imports ScientificInterfaceShared.Controls
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class cEwEBiomassEmitterPlugin
    Implements IEcopathRunInitializedPlugin
    Implements IEcospacePlugin
    Implements IEcospaceInitializedPlugin
    Implements IEcospaceInitRunCompletedPlugin
    Implements IEcospaceBeginTimestepPlugin
    Implements INavigationTreeItemPlugin
    Implements IAutoRunPlugin
    Implements IUIContextPlugin

    Private m_data As cData = Nothing
    Private m_engine As cBiomassEmitter = Nothing
    Private m_core As cCore = Nothing
    Private m_ecopathDS As cEcopathDataStructures = Nothing
    Private m_ecospaceDS As cEcospaceDataStructures = Nothing
    Private m_uic As cUIContext = Nothing
    Private m_ui As frmBiomassEmitter = Nothing

#Region " Generic "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = CType(core, cCore)
        Me.m_data = New cData(Me, Me.m_core)
        Me.m_engine = New cBiomassEmitter(Me.m_data)
    End Sub

    Public ReadOnly Property DisplayName As String Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION_EMITTER
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "ndEwEBiomassEmitterPlugin"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Ecopath International Initiative Research Association, Spain"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property NavigationTreeItemLocation As String Implements INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndSpatialDynamic\ndEcospaceInput"
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As EventArgs, ByRef frmPlugin As Object) Implements IGUIPlugin.OnControlClick
        frmPlugin = Me.GetUI()
    End Sub

    Public ReadOnly Property ControlImage As Object Implements IGUIPlugin.ControlImage
        Get
            Return SharedResources.nav_input
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements IGUIPlugin.ControlTooltipText
        Get
            Return "Launch the Ecospace biomass emitter plug-in"
        End Get
    End Property

    Public ReadOnly Property EnabledState As eCoreExecutionState Implements IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcospaceLoaded
        End Get
    End Property

#End Region ' Generic

#Region " AutoRun "

    Public Function AutoRunTypes() As eCoreComponentType() Implements IAutoRunPlugin.AutoRunTypes
        Return New eCoreComponentType() {eCoreComponentType.Ecospace}
    End Function

    Public Property AutoRun(type As eCoreComponentType) As Boolean Implements IAutoRunPlugin.AutoRun
        Get
            Return Me.m_engine.Enabled
        End Get
        Set(value As Boolean)
            Me.m_engine.Enabled = value
        End Set
    End Property

#End Region ' AutoRun

#Region " Ecopath "

    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, TaxonDataAsObject As Object, StanzaDataAsObject As Object) _
        Implements IEcopathRunInitializedPlugin.EcopathRunInitialized
        Me.m_ecopathDS = DirectCast(EcopathDataAsObject, cEcopathDataStructures)
    End Sub

#End Region ' Ecopath

#Region " Ecospace "

    Public Sub LoadEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.LoadEcospaceScenario

        If (Me.m_engine Is Nothing) Then Return
        ' Tag along
        Me.m_data.LoadEcospaceScenario()

    End Sub

    Public Sub SaveEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.SaveEcospaceScenario

        If (Me.m_engine Is Nothing) Then Return
        ' Tag along
        'Me.m_data.SaveEcospaceScenario()

    End Sub

    Public Sub CloseEcospaceScenario() Implements IEcospacePlugin.CloseEcospaceScenario

        If (Me.m_engine Is Nothing) Then Return
        Me.m_data.Clear()

    End Sub

    Public Sub EcospaceInitialized(EcospaceDatastructures As Object) Implements IEcospaceInitializedPlugin.EcospaceInitialized
        Me.m_ecospaceDS = CType(EcospaceDatastructures, cEcospaceDataStructures)
    End Sub

    Public Sub EcospaceInitRunCompleted(EcospaceDatastructures As Object) Implements IEcospaceInitRunCompletedPlugin.EcospaceInitRunCompleted
        Try
            Me.m_engine.InitForRun()
        Catch ex As Exception

        End Try
    End Sub

    Public Sub EcospaceBeginTimeStep(EcospaceDatastructures As Object, iTime As Integer) Implements IEcospaceBeginTimestepPlugin.EcospaceBeginTimeStep
        Try
            Me.m_engine.Apply(iTime)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Ecospace

#Region " Accessors "

    Friend ReadOnly Property EcopathDS As cEcopathDataStructures
        Get
            Return Me.m_ecopathDS
        End Get
    End Property

    Friend ReadOnly Property EcospaceDS As cEcospaceDataStructures
        Get
            Return Me.m_ecospaceDS
        End Get
    End Property

#End Region ' Accessors

#Region " UI "

    Public Sub UIContext(uic As Object) Implements IUIContextPlugin.UIContext
        Me.m_uic = CType(uic, cUIContext)
    End Sub

    Private Function HasUI() As Boolean
        If (Me.m_ui IsNot Nothing) Then Return Not Me.m_ui.IsDisposed
        Return False
    End Function

    Private Function GetUI() As frmBiomassEmitter
        If Not Me.HasUI Then
            Me.m_ui = New frmBiomassEmitter(Me.m_uic, Me.m_engine)
        End If
        Return Me.m_ui
    End Function

#End Region ' UI

End Class
