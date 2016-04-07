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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.Reflection
Imports EwECore
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region

Public Class cFishBasePlugin
    Implements IDataSearchProducerPlugin
    Implements IDisposedPlugin
    Implements IConfigurablePlugin
    Implements ITaxonSearchCapabilitiesPlugin

#Region " Private vars "

    Private m_bInitOk As Boolean = False
    Private m_core As cCore = Nothing
    Private m_fbddx As cFishBaseConnection = Nothing

    ''' <summary>Data search term.</summary>
    Friend m_dataTerm As ITaxonSearchData = Nothing
    ''' <summary>Search results for the last data term.</summary>
    Friend m_results As cFishBaseSearchResults = Nothing
    ''' <summary>Broadcaster for distributing data.</summary>
    Friend m_broadcaster As IDataBroadcaster = Nothing

    ''' <summary>Data provider enabled state.</summary>
    Private m_bEnabled As Boolean = False

#End Region ' Private vars

#Region " Plugin points implementation "

#Region " Init and disposal "

    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize

        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOk = False
        Try
            If TypeOf core Is EwECore.cCore Then
                Me.m_core = DirectCast(core, EwECore.cCore)
                Me.m_bInitOk = True
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try

    End Sub

    Public Sub Dispose() _
        Implements EwEPlugin.IDisposedPlugin.Dispose
        Me.Connection = Nothing
    End Sub

#End Region ' Init and disposal

#Region " Generic "

    Public ReadOnly Property Author() As String _
        Implements EwEPlugin.IPlugin.Author
        Get
            Return "Ecopath International Initiative"
        End Get
    End Property

    Public ReadOnly Property Contact() As String _
        Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    Public ReadOnly Property Description() As String _
        Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in for obtaining taxonomy data from FishBase"
        End Get
    End Property

    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name
        Get
            Return My.Resources.ENGINE_NAME
        End Get
    End Property

#End Region ' Generic

#Region " Data "

    Public Sub Broadcaster(ByVal broadcaster As IDataBroadcaster) _
        Implements IDataProducerPlugin.Broadcaster
        Me.m_broadcaster = broadcaster
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="aresults"></param>
    ''' <returns></returns>
    Friend Function BroadcastResults(aResults() As ITaxonSearchData) As Boolean
        ' Create new results
        Me.m_results = New cFishBaseSearchResults(Me.m_dataTerm, aResults, EwEUtils.Utilities.cTypeUtils.TypeToString(Me.GetType))
        ' Broadcast results
        If (Me.m_broadcaster IsNot Nothing) Then
            Me.m_broadcaster.BroadcastData(Me.Name, Me.m_results)
        End If
        Return True
    End Function

    Public Function GetDataByType(ByVal typeData As System.Type, ByRef data As IPluginData) As Boolean _
        Implements IDataProducerPlugin.GetDataByType
        If (TypeOf data Is ITaxonSearchData) Then data = DirectCast(Me.m_dataTerm, IPluginData)
        Return Me.IsEnabled
    End Function

    Public Function IsDataAvailable(ByVal typeData As System.Type, _
                                    Optional ByVal runType As IRunType = Nothing) As Boolean _
        Implements IDataProducerPlugin.IsDataAvailable
        Return (GetType(ITaxonSearchData).IsAssignableFrom(typeData))
    End Function

    Public Function IsEnabled() As Boolean _
        Implements IDataProducerPlugin.IsEnabled
        Return Me.m_bEnabled
    End Function

    Public Function IsEnabled(ByVal typeData As System.Type, _
                              ByVal runType As IRunType) As Boolean _
        Implements IDataProducerPlugin.IsEnabled
        Return Me.m_bEnabled
    End Function

    Public Function SetEnabled(ByVal bEnable As Boolean) As Boolean _
        Implements IDataProducerPlugin.SetEnabled
        Me.m_bEnabled = bEnable
    End Function

    Public Sub SetEnabled(ByVal typeData As System.Type, _
                          ByVal runType As IRunType, _
                          ByVal bEnable As Boolean) _
        Implements IDataProducerPlugin.SetEnabled
        ' NOP
    End Sub

#End Region ' Data

#Region " Search "

    ''' <inheritdoc cref="IDataSearchProducerPlugin.StartSearch"/>
    Public Function StartSearch(ByVal data As Object, _
                                ByVal iMaxResults As Integer) As Boolean _
        Implements IDataSearchProducerPlugin.StartSearch

        If (Me.m_fbddx Is Nothing) Then Return False

        ' Test connection
        If Not Me.m_fbddx.IsConnected Then Return False
        ' Test data type
        If Not (TypeOf data Is ITaxonSearchData) Then Return False
        ' Get ready
        Me.m_dataTerm = DirectCast(data, ITaxonSearchData)
        Me.m_results = Nothing

        If ((Me.m_dataTerm.SearchFields And eTaxonClassificationType.Latin) > 0) Then
            Me.m_dataTerm.SearchFields = Me.m_dataTerm.SearchFields Or eTaxonClassificationType.Class Or eTaxonClassificationType.Genus
        End If

        If (iMaxResults > 0) Then
            Me.MaxResults = iMaxResults
        End If

        ' Go search
        Return Me.m_fbddx.Search(Me.m_dataTerm, Me.MaxResults)

    End Function

    ''' <inheritdoc cref="IDataSearchProducerPlugin.StopSearch"/>
    Public Function StopSearch() As Boolean _
        Implements EwEPlugin.Data.IDataSearchProducerPlugin.StopSearch
        Return True
    End Function

    ''' <inheritdoc cref="IDataSearchProducerPlugin.IsSeaching"/>
    Public Function IsSeaching() As Boolean _
        Implements IDataSearchProducerPlugin.IsSeaching
        If (Me.m_fbddx Is Nothing) Then Return False
        Return Me.m_fbddx.IsSearching
    End Function

    ''' <inheritdoc cref="IDataSearchProducerPlugin.SearchResults"/>
    Public Function SearchResults(ByVal dataTerm As Object, ByRef results As IDataSearchResults) As Boolean _
        Implements IDataSearchProducerPlugin.SearchResults

        If (Object.ReferenceEquals(dataTerm, Me.m_dataTerm)) Then
            results = Me.m_results
            Return True
        End If
        Return False

    End Function

    ''' <inheritdocs cref="IDataSearchProducerPlugin.CreateSearchTerm"/>
    Public Function CreateSearchTerm() As Object _
        Implements EwEPlugin.Data.IDataSearchProducerPlugin.CreateSearchTerm
        Return New cFishBaseTaxonData(cTypeUtils.TypeToString(Me.GetType()))
    End Function

#End Region ' Search

#Region " Search capabiities "

    Public Function TaxonSearchCapabilities() As eTaxonClassificationType _
        Implements ITaxonSearchCapabilitiesPlugin.TaxonSearchCapabilities
        Return eTaxonClassificationType.Common Or _
               eTaxonClassificationType.Class Or _
               eTaxonClassificationType.Family Or _
               eTaxonClassificationType.Order Or _
               eTaxonClassificationType.Genus Or _
               eTaxonClassificationType.Species Or _
               eTaxonClassificationType.Latin
    End Function

    Public Function HasDepthRangeSearchCapabilities() As Boolean _
        Implements ITaxonSearchCapabilities.HasDepthRangeSearchCapabilities
        Return False
    End Function

    Public Function HasSpatialSearchCapabilities() As Boolean Implements ITaxonSearchCapabilities.HasSpatialSearchCapabilities
        Return True
    End Function

#End Region ' Search capabiities

#End Region ' Plugin points implementation

#Region " Friendly bits "

    Friend Function GetConfigUI() As System.Windows.Forms.Control _
        Implements EwEPlugin.IConfigurablePlugin.GetConfigUI
        Try
            Return New ucConfig(Me)
        Catch ex As Exception
            cLog.Write(ex, "cFishBasePlugin.GetConfigUI()")
        End Try
        Return Nothing
    End Function

    Friend Function IsConfigured() As Boolean _
        Implements EwEPlugin.IConfigurablePlugin.IsConfigured
        If (Me.m_fbddx Is Nothing) Then Return False
        Return Me.m_fbddx.IsConnected
    End Function

    Friend Property Connection As cFishBaseConnection
        Get
            Return Me.m_fbddx
        End Get
        Set(value As cFishBaseConnection)
            If (Me.m_fbddx IsNot Nothing) Then
                ' ToDo: Cleanup
            End If
            Me.m_fbddx = value
            If (Me.m_fbddx IsNot Nothing) Then
                ' ToDo: Initialize
            End If
        End Set
    End Property

    Friend Property MaxResults As Integer = 100

#End Region ' Friendly bits

#Region " Event handling "

    ''' <summary>
    ''' Handler to send a message to the core
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="bLoggedOn"></param>
    ''' <param name="bError"></param>
    ''' <param name="strMessage"></param>
    Private Sub OnAuthenticated(ByVal sender As cFishBaseConnection, _
                                ByVal bLoggedOn As Boolean, _
                                ByVal bError As Boolean, _
                                ByVal strMessage As String)

        Dim msg As New cMessage("", eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)

        If bLoggedOn Then
            msg.Message = My.Resources.STATUS_LOGGED_ON
        Else
            If String.IsNullOrWhiteSpace(strMessage) Then
                msg.Message = My.Resources.STATUS_LOGGED_OFF
            Else
                msg.Message = String.Format(My.Resources.STATUS_LOGON_ERROR, strMessage)
                msg.Importance = eMessageImportance.Warning
            End If
        End If
        Me.m_core.Messages.SendMessage(msg)

    End Sub

#End Region ' Event handling

End Class
