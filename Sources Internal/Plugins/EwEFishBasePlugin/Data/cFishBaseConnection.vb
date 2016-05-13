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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwEUtils.Core

Public MustInherit Class cFishBaseConnection
    Implements IDisposable

    Private m_bConnected As Boolean = False
    Private m_bLoggedOn As Boolean = False
    Private m_bIsSearching As Boolean = False
    Private m_ppt As cFishBasePlugin = Nothing
    Private m_term As ITaxonSearchData = Nothing

    Public Sub New(plugin As cFishBasePlugin)
        Me.m_ppt = plugin
    End Sub

    Public Overridable Sub Dispose() _
        Implements IDisposable.Dispose
        GC.SuppressFinalize(Me)
    End Sub

#Region " Events "

    ''' <summary>FishBase server connection state event.</summary>
    ''' <param name="sender">The FB DDX server this event originated from.</param>
    ''' <param name="bConnected">FB server connection status</param>
    Public Event OnConnected(ByVal sender As cFishBaseConnection, ByVal bConnected As Boolean)

    ''' <summary>FishBase server search results event.</summary>
    ''' <param name="sender">The FB DDX server this event originated from.</param>
    ''' <param name="results">The results.</param>
    Public Event OnSearchResults(ByVal sender As cFishBaseConnection, ByVal results() As ITaxonSearchData)

    Protected Sub FireConnectionEvent()
        Try
            RaiseEvent OnConnected(Me, Me.IsConnected)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub FireSearchResultsEvent(results() As ITaxonSearchData)
        Try
            RaiseEvent OnSearchResults(Me, results)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Events

    Public Overridable Function Connect() As Boolean
        If Me.IsConnected Then Me.Disconnect()
        Return True
    End Function

    Public Overridable Function Disconnect() As Boolean
        Return Me.IsConnected
    End Function

    Public Overridable Property IsConnected() As Boolean
        Get
            Return Me.m_bConnected
        End Get
        Set(value As Boolean)
            If value <> Me.m_bConnected Then
                Me.m_bConnected = value
                Me.FireConnectionEvent()
            End If
        End Set
    End Property

    Public Overridable Property IsSearching() As Boolean
        Get
            Return Me.m_bIsSearching
        End Get
        Set(value As Boolean)
            Me.m_bIsSearching = value
        End Set
    End Property

    Public Overridable Function Search(ByVal taxon As ITaxonSearchData, iMaxResults As Integer) As Boolean
        If (Not Me.IsConnected) Then Return False
        Me.m_term = taxon
        Return True
    End Function

    Protected ReadOnly Property SearchTerm As ITaxonSearchData
        Get
            Return Me.m_term
        End Get
    End Property

    Protected ReadOnly Property PluginPoint As cFishBasePlugin
        Get
            Return Me.m_ppt
        End Get
    End Property

    Public Function IUCNstatus(strCode As String) As eIUCNConservationStatusTypes

        Select Case strCode.ToLower.Replace(".", "")
            Case "ne" : Return eIUCNConservationStatusTypes.NotEvaluated
            Case "dd" : Return eIUCNConservationStatusTypes.DataDeficient
            Case "lc" : Return eIUCNConservationStatusTypes.LeastConcern
            Case "nt" : Return eIUCNConservationStatusTypes.NearThreatened
            Case "vu" : Return eIUCNConservationStatusTypes.Vulnerable
            Case "en" : Return eIUCNConservationStatusTypes.Endangered
            Case "cr" : Return eIUCNConservationStatusTypes.CriticallyEndangered
            Case "ew" : Return eIUCNConservationStatusTypes.ExtinctInWild
            Case "ex" : Return eIUCNConservationStatusTypes.Extinct
            Case "lr/cd" ' no idea what to do with this
            Case "lr/lc" : Return eIUCNConservationStatusTypes.LeastConcern
            Case "lr/nt" : Return eIUCNConservationStatusTypes.NearThreatened
        End Select
        Return eIUCNConservationStatusTypes.NotSet

    End Function

End Class
