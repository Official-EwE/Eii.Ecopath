' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.IO
Imports EwECore
Imports EwECore.DataSources
Imports EwECore.Plugins
Imports EwECore.Plugins.Ecospace

''' ---------------------------------------------------------------------------
''' <summary>
''' Plug-in point to manage transect persistence
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cTransectStoragePlugin
    Implements IEcospacePlugin
    Implements IEcospaceScenarioAddedOrRemovedPlugin

#Region " Private vars "

    Private m_core As cCore = Nothing
    Private m_data As cTransectDatastructures = Nothing

#End Region ' Private vars

#Region " Foundation "

    Public Sub Initialize(core As Object) Implements IPlugin.Initialize
        Me.m_core = CType(core, cCore)
        Me.m_data = cTransectDatastructures.Instance(Me.m_core)
    End Sub

    Private m_iScenarioDBID As Integer = -1

    Public Sub LoadEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.LoadEcospaceScenario

        Dim ds As IEcospaceDatasource = DirectCast(dataSource, IEcospaceDatasource)
        Dim scenario As cEcospaceScenario = Me.m_core.EcospaceScenarios(Me.m_core.ActiveEcospaceScenarioIndex)

        Me.m_iScenarioDBID = scenario.DBID
        Dim strDBFileNme As String = Me.TransectFileName(ds, Me.m_iScenarioDBID)

        If Me.m_data.FromXML(strDBFileNme) Then
            Me.m_data.IsChanged = False
        End If

    End Sub

    Public Sub SaveEcospaceScenario(dataSource As Object) Implements IEcospacePlugin.SaveEcospaceScenario

        Dim ds As IEcospaceDatasource = DirectCast(dataSource, IEcospaceDatasource)
        Dim strDBFileNme As String = Me.TransectFileName(ds, Me.m_iScenarioDBID)

        If (Me.m_data.IsChanged() And Me.m_iScenarioDBID > 0) Then
            If Me.m_data.ToXML(strDBFileNme) Then
            End If
        End If
        Me.m_data.IsChanged = False

    End Sub

    Public Sub CloseEcospaceScenario() Implements IEcospacePlugin.CloseEcospaceScenario
        Me.m_data.Clear()
        Me.m_iScenarioDBID = -1
    End Sub

    Public Sub EcospaceScenarioAdded(dataSource As Object, scenarioID As Integer) _
        Implements IEcospaceScenarioAddedOrRemovedPlugin.EcospaceScenarioAdded
        ' NOP
    End Sub

    Public Sub EcospaceScenarioRemoved(dataSource As Object, scenarioID As Integer) _
        Implements IEcospaceScenarioAddedOrRemovedPlugin.EcospaceScenarioRemoved

        Try
            Dim strFile As String = Me.TransectFileName(DirectCast(dataSource, IEcospaceDatasource), scenarioID)
            If (File.Exists(strFile)) Then File.Delete(strFile)
        Catch ex As Exception

        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IPlugin.DisplayName"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property DisplayName As String _
        Implements IPlugin.DisplayName
        Get
            Return My.Resources.CAPTION_STORAGE
        End Get
    End Property

    Public ReadOnly Property Name As String Implements IPlugin.Name
        Get
            Return "zCosmInsiteTransectStorage"
        End Get
    End Property

    Public ReadOnly Property Description As String Implements IPlugin.Description
        Get
            Return "This plug-in manages the life span of transect data"
        End Get
    End Property

    Public ReadOnly Property Author As String Implements IPlugin.Author
        Get
            Return "Jeroen Steenbeek"
        End Get
    End Property

    Public ReadOnly Property Contact As String Implements IPlugin.Contact
        Get
            Return "ewedevteam@gmail.com"
        End Get
    End Property

#End Region ' Foundation

#Region " Internals "

    Private Function TransectFileName(ds As IEcospaceDatasource, iScenarioID As Integer) As String
        Dim strDB As String = ds.ToString()
        Dim strPath As String = Path.GetDirectoryName(strDB)
        Dim strFile As String = Path.GetFileNameWithoutExtension(strDB) & "_" & iScenarioID & "_transects.xml"
        Return Path.Combine(strPath, strFile)
    End Function

#End Region ' Internals

End Class
