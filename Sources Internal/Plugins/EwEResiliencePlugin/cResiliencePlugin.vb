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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cResiliencePlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IAutoSavePlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPostPlugin
    Implements EwEPlugin.INavigationTreeItemPlugin
    Implements EwEPlugin.IDisposedPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_core As cCore = Nothing
    Private m_pathds As cEcopathDataStructures = Nothing
    Private m_simds As cEcosimDatastructures = Nothing
    Private m_frm As frmResilience = Nothing
    Private m_model As cResilienceModel = Nothing

#End Region ' Private vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "F. Arreguín-Sánchez, M. Zetina-Rejon, J. Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:farregui@ipn.mx"
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for the EwE6 software to estimate, save and display resilience, as demonstrated in 'Measuring resilience in aquatic trophic networks from supply–demand-of-energy relationships'"
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
        Me.m_model = New cResilienceModel(Me.m_core)
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            ' Navigation tree nodes are sorted by name. 
            ' With the name prefix 'ndX' the resilience node ends up at the bottom of the Ecosim output nodes list
            Return "ndXEcosimResilience"
        End Get
    End Property

#End Region ' Generic plug-in bits

#Region " Autosave implementation "

    Public Property AutoSave As Boolean _
        Implements EwEPlugin.IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.Autosave
        End Get
        Set(value As Boolean)
            My.Settings.Autosave = value
            My.Settings.Save()
        End Set
    End Property

    Public Function AutoSaveName() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveName
        Return My.Resources.AUTOSAVE_NAME
    End Function

    Public Function AutoSaveSubPath() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveSubPath
        ' No fancy sub-directories
        Return ""
    End Function

    Public Function AutoSaveType() As EwEUtils.Core.eAutosaveTypes _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveType
        Return eAutosaveTypes.Ecosim
    End Function

#End Region ' Autosave implementation

#Region " UIC integration "

    Public Sub UIContext(uic As Object) Implements EwEPlugin.IUIContextPlugin.UIContext
        Try
            Me.m_uic = CType(uic, cUIContext)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' UIC integration

#Region " Ecopath integration "

    Public Sub EcopathRunInitialized(EcopathDataAsObject As Object, _
                                     TaxonDataAsObject As Object, _
                                     StanzaDataAsObject As Object) Implements EwEPlugin.IEcopathRunInitializedPlugin.EcopathRunInitialized
        Try
            Me.m_pathds = CType(EcopathDataAsObject, cEcopathDataStructures)
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Ecopath integration

#Region " Ecosim integration "

    Public Sub EcosimInitialized(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimInitializedPlugin.EcosimInitialized
        Try
            Me.m_simds = CType(EcosimDatastructures, cEcosimDatastructures)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try
            Me.m_model.Compute(iTime, Me.m_simds)
        Catch ex As Exception

        End Try

    End Sub

    Public Sub EcosimRunCompletedPost(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunCompletedPostPlugin.EcosimRunCompletedPost

        If My.Settings.Autosave Then
            Dim writer As New cResilienceWriter(Me.m_core, Me.m_model.Data)
            writer.SaveDataToFile()
        End If

    End Sub

#End Region ' Ecosim integration

#Region " UI integration "

    Public ReadOnly Property ControlImage As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String _
        Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.CAPTION
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcosimCompleted
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try
            If (Not Me.HasUI()) Then
                Me.m_frm = New frmResilience(Me.m_uic, Me.m_model)
                frmPlugin = Me.m_frm
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndTimeDynamic\ndEcosimOutput"
        End Get
    End Property

#End Region ' UI integration

#Region " Disposal "

    Public Sub Dispose() Implements EwEPlugin.IDisposedPlugin.Dispose

        If (Me.m_frm IsNot Nothing) Then
            Me.m_frm.Dispose()
            Me.m_frm = Nothing
        End If
        Me.m_model = Nothing

    End Sub

#End Region ' Disposal

#Region " Internals "

    Private Function HasUI() As Boolean
        If (Me.m_frm Is Nothing) Then Return False
        Return (Not Me.m_frm.IsDisposed)
    End Function

#End Region ' Internals

End Class
