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
    Inherits cResiliencePluginBase
    Implements EwEPlugin.IAutoSavePlugin
    Implements EwEPlugin.IEcopathRunInitializedPlugin
    Implements EwEPlugin.IEcosimInitializedPlugin
    Implements EwEPlugin.IEcosimEndTimestepPlugin
    Implements EwEPlugin.IEcosimRunCompletedPostPlugin
    Implements EwEPlugin.IDisposedPlugin

#Region " Private vars "

    Private m_pathds As cEcopathDataStructures = Nothing
    Private m_simds As cEcosimDatastructures = Nothing
    Private m_model As cResilienceModel = Nothing

#End Region ' Private vars

#Region " Singleton "

    Private Shared g_inst As cResiliencePlugin = Nothing

    Public Shared Function GetInstance() As cResiliencePlugin
        Return g_inst
    End Function

#End Region ' Singleton

#Region " Constructor "

    Public Sub New()
        g_inst = Me
    End Sub

#End Region ' Constructor

#Region " Autosave implementation "

    Public Property AutoSave As Boolean _
        Implements EwEPlugin.IAutoSavePlugin.AutoSave
        Get
            Return My.Settings.ResilAutosave
        End Get
        Set(value As Boolean)
            My.Settings.ResilAutosave = value
            My.Settings.Save()
        End Set
    End Property

    Public Function AutoSaveName() As String _
        Implements EwEPlugin.IAutoSavePlugin.AutoSaveName
        Return My.Resources.RESIL_AUTOSAVE_NAME
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

        If My.Settings.ResilAutosave Then
            Dim writer As New cResilienceWriter(Me.m_core, Me.m_model.Data)
            writer.SaveDataToFile()
        End If

    End Sub

#End Region ' Ecosim integration

    Public ReadOnly Property Model As cResilienceModel
        Get
            Return Me.m_model
        End Get
    End Property

    Public Overrides ReadOnly Property Name As String
        Get
            Return "ndResilience-00-Core"
        End Get
    End Property

    Public Overrides Sub Initialize(core As Object)
        MyBase.Initialize(core)
        Me.m_model = New cResilienceModel(Me.m_core)
    End Sub

    Protected Overrides Sub Dispose()

        Me.m_model.Dispose()
        Me.m_model = Nothing
        MyBase.Dispose()

    End Sub

End Class
