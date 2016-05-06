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
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

Public Class cConsWriterPlugin
    Implements EwEPlugin.IUIContextPlugin
    Implements EwEPlugin.IAutoSavePlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimRunInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPostPlugin
    Implements EwEPlugin.IMenuItemPlugin

#Region " Private vars "

    Private m_uic As cUIContext = Nothing
    Private m_core As cCore = Nothing
    Private m_pathds As cEcopathDataStructures = Nothing
    Private m_simds As cEcosimDatastructures = Nothing
    Private m_writer As cConsumptionWriter = Nothing

#End Region ' Private vars

#Region " Generic plug-in bits "

    Public ReadOnly Property Author As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Contact As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property Description As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return ""
        End Get
    End Property

    Public Sub Initialize(core As Object) _
        Implements EwEPlugin.IPlugin.Initialize
        Me.m_core = DirectCast(core, cCore)
    End Sub

    Public ReadOnly Property Name As String Implements EwEPlugin.IPlugin.Name
        Get
            Return "MenuOptionsConsWriter"
        End Get
    End Property

#End Region ' Generic plug-in bits

#Region " Autosave implementation "

    Public Property AutoSave As Boolean _
        Implements EwEPlugin.IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.ConsAutosave
        End Get
        Set(value As Boolean)
            My.Settings.ConsAutosave = value
            My.Settings.Save()
        End Set
    End Property

    Public Function AutoSaveName() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveName
        Return My.Resources.CONSWR_AUTOSAVE_ITEM
    End Function

    Public Function AutoSaveSubPath() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveOutputPath
        Return Me.m_core.DefaultOutputPath(Me.AutoSaveType)
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

    Public Sub EcosimRunInitialized(EcosimDatastructures As Object) Implements EwEPlugin.IEcosimRunInitializedPlugin.EcosimRunInitialized
        Try
            If My.Settings.ConsAutosave Then
                Me.m_writer = New cConsumptionWriter(Me.m_core, m_pathds, m_simds)
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub EcosimRunCompletedPost(EcosimDatastructures As Object) _
        Implements EwEPlugin.IEcosimRunCompletedPostPlugin.EcosimRunCompletedPost

        'ToDo: globalize this

        If (Me.m_writer IsNot Nothing) Then
            Dim msg As cMessage = Nothing
            If (Me.m_writer.Success) Then
                msg = New cMessage(String.Format(My.Resources.CONSWR_STATUS_SAVE_SUCCESS, Me.m_writer.OutputPath), _
                                   eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Information)
                msg.Hyperlink = Me.m_writer.OutputPath
            Else
                msg = New cMessage(String.Format(My.Resources.CONSWR_STATUS_SAVE_FAILED, Me.m_writer.OutputPath), _
                                   eMessageType.DataExport, eCoreComponentType.External, eMessageImportance.Warning)
            End If
            Me.m_core.Messages.SendMessage(msg)
            Me.m_writer = Nothing
        End If
    End Sub

    Public Sub EcosimEndTimeStep(ByRef BiomassAtTimestep() As Single, EcosimDatastructures As Object, iTime As Integer, Ecosimresults As Object) _
        Implements EwEPlugin.IEcosimEndTimestepPlugin.EcosimEndTimeStep

        Try
            If (Me.m_writer IsNot Nothing) Then
                Me.m_writer.SaveDataToFile(iTime, True)
                Me.m_writer.SaveDataToFile(iTime, False)
            End If
        Catch ex As Exception

        End Try

    End Sub

#End Region ' Ecosim integration

#Region " UI integration "

    Public ReadOnly Property ControlImage As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return My.Resources.CONSWR_MENU_ITEM
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.Idle
        End Get
    End Property

    Public Sub OnControlClick(sender As Object, e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        Try
            Dim dlg As New frmConfig(Me.m_uic)
            dlg.ShowDialog()
        Catch ex As Exception

        End Try
    End Sub

    Public ReadOnly Property MenuItemLocation As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

#End Region ' UI integration

 End Class
