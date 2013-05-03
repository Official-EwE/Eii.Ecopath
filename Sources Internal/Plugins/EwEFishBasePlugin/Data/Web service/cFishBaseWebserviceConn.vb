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
#Region " Imports "

Option Strict On
Imports System.Text
Imports System.Xml
Imports EwEUtils.Core
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Buffer class for interfacing with the FishBase web server.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFishBaseWebserviceConn
    Inherits cFishBaseConnection

#Region " Private vars "

    Private Enum eSocketRequest As Integer
        None = 0
        Connecting
        LogOn
        Searching
        Details
        LogOff
        Disconnecting
    End Enum

    Private m_socket As cMessageClient = Nothing
    Private m_request As eSocketRequest = eSocketRequest.None

    Private m_strHost As String = ""
    Private m_iPort As Integer = 0
    Private m_strPwdMD5 As String = ""
    Private m_strUnMD5 As String = ""

    ''' <summary>Number of milliseconds for attempting a web service reconnect.</summary>
    Private m_iAutoConnectTimeOut As Integer = 5000

#End Region ' Private vars 

#Region " Construction / destruction "

    Public Sub New(ppt As cFishBasePlugin)
        MyBase.New(ppt)

        Me.m_socket = New cMessageClient()

        AddHandler Me.m_socket.ConnectionAccepted, AddressOf OnSocketConnectionAccepted
        AddHandler Me.m_socket.ConnectionClosed, AddressOf OnSocketConnectionClosed
        AddHandler Me.m_socket.ConnectionFailed, AddressOf OnSocketConnectionFailed
        AddHandler Me.m_socket.MessageReceived, AddressOf OnSocketMessageReceived

    End Sub

    Public Overrides Sub Dispose()
        If (Me.m_socket IsNot Nothing) Then

            Me.Disconnect()

            RemoveHandler Me.m_socket.ConnectionAccepted, AddressOf OnSocketConnectionAccepted
            RemoveHandler Me.m_socket.ConnectionClosed, AddressOf OnSocketConnectionClosed
            RemoveHandler Me.m_socket.ConnectionFailed, AddressOf OnSocketConnectionFailed
            RemoveHandler Me.m_socket.MessageReceived, AddressOf OnSocketMessageReceived

            Me.m_socket.Dispose()
            Me.m_socket = Nothing

        End If
    End Sub

#End Region ' Construction / destruction

#Region " Properties "

    Public Overrides Property IsConnected() As Boolean
        Get
            Return MyBase.IsConnected
        End Get
        Set(ByVal value As Boolean)
            MyBase.IsConnected = value
        End Set
    End Property

    Private Property PendingRequest() As eSocketRequest
        Get
            Return Me.m_request
        End Get
        Set(ByVal value As eSocketRequest)
            Me.m_request = value
            Me.IsSearching = (Me.PendingRequest <> cFishBaseWebserviceConn.eSocketRequest.None)
        End Set
    End Property

#End Region ' Properties

    Public Overrides Function Connect() As Boolean
        'Return Me.Connect("localhost", 8001)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to connect the FishBase server.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overloads Function Connect(ByVal strIP As String, iPort As Integer, ByVal strName As String, ByVal strPwd As String) As Boolean
        If Not MyBase.Connect() Then Return False
        Try
            Me.m_socket.Connect(strIP, iPort)
        Catch ex As Exception
            Return False
        End Try

        Try
            Dim doc = Me.CreateXMLDocument()

            If String.IsNullOrEmpty(strName) Then
                doc.AppendChild(doc.CreateElement("logoff"))
                Me.m_strUnMD5 = ""
                Me.m_strPwdMD5 = ""

                Me.PendingRequest = eSocketRequest.LogOff
            Else
                Dim nd As XmlNode = doc.CreateElement("logon")
                Dim att As XmlAttribute = Nothing

                Me.m_strUnMD5 = cStringUtils.GenerateHash(strName)
                att = doc.CreateAttribute("username")
                att.Value = Me.m_strUnMD5
                nd.Attributes.Append(att)

                Me.m_strPwdMD5 = cStringUtils.GenerateHash(strPwd)
                att = doc.CreateAttribute("pwd")
                att.Value = Me.m_strPwdMD5
                nd.Attributes.Append(att)

                doc.AppendChild(nd)

                Me.PendingRequest = eSocketRequest.LogOn
            End If

            Me.m_socket.Send(doc.OuterXml)
        Catch ex As Exception

        End Try
        Return True
    End Function

    Public Overrides Function Disconnect() As Boolean
        If Not MyBase.Disconnect Then Return False
        Try
            Me.m_socket.Disconnect()
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start executing a FIshBase search
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Search(ByVal taxon As ITaxonSearchData, iMaxResults As Integer) As Boolean
        If (Not Me.IsConnected) Then Return False
        Me.PendingRequest = eSocketRequest.Searching
        ' ToDo: create search on request
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start executing a FishBase detail request.
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function Details(ByVal taxon As ITaxonSearchData) As Boolean

    End Function

#Region " Internals "

#Region " XML "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Parse the FishBase webserver message.
    ''' </summary>
    ''' <param name="strMessage">The message to parse.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ParseMessage(ByVal strMessage As String)

        Dim doc As New XmlDocument()
        Try
            doc.LoadXml(strMessage)
        Catch ex As Exception

        End Try

        Dim lNodes = doc.SelectNodes("logon")
        If lNodes.Count = 1 Then
            Me.ParseLogonResults(lNodes(0))
        End If

        Me.PendingRequest = eSocketRequest.None

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Simple log-on result parser
    ''' </summary>
    ''' <param name="xn">Node to parse</param>
    ''' <remarks>
    ''' The node is expected to have the following layout:
    ''' <code>
    '''     <logon status="true|false" message="..."/>
    ''' </code>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub ParseLogonResults(ByVal xn As XmlNode)

        Dim att As XmlAttribute = Nothing
        Dim strMessage As String = ""
        Dim bLoggedOn As Boolean = False

        ' Assume the worst

        ' Obtain status
        att = xn.Attributes("status")
        If att IsNot Nothing Then bLoggedOn = (String.Compare(xn.Attributes(0).InnerText, "true", True) = 0)

        ' Obtain optional message
        att = xn.Attributes("message")
        If att IsNot Nothing Then strMessage = att.InnerText

        Dim bError As Boolean = (Me.PendingRequest = eSocketRequest.LogOn And bLoggedOn = False)

        Me.FireConnectionEvent()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Simple search results parser
    ''' </summary>
    ''' <param name="xn"></param>
    ''' -----------------------------------------------------------------------
    Private Sub ParseSearchResults(ByVal xn As XmlNode)

        Dim lResults As New List(Of ITaxonSearchData)
        MyBase.FireSearchResultsEvent(lResults.ToArray)

    End Sub

    Private Sub ParseDetailResults(ByVal xn As XmlNode)

    End Sub

    Private Function CreateXMLDocument() As XmlDocument
        Dim doc As New XmlDocument()
        Dim dec As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", Nothing)
        doc.AppendChild(dec)
        doc.PreserveWhitespace = False
        Return doc
    End Function

#End Region ' XML

#Region " Socket event handling "

    Private Sub OnSocketConnectionAccepted(ByVal sender As Object, ByVal args As cConnectionEventArgs)
        Me.IsConnected = True
    End Sub

    Private Sub OnSocketConnectionClosed(ByVal sender As Object, ByVal args As cConnectionEventArgs)
        Me.IsConnected = False
    End Sub

    Private Sub OnSocketConnectionFailed(ByVal sender As Object, ByVal args As cConnectionEventArgs)
        Me.IsConnected = False
    End Sub

    Private Sub OnSocketMessageReceived(ByVal sender As Object, ByVal args As cMessageReceivedEventArgs)
        Try
            Me.ParseMessage(args.Message)
        Catch ex As Exception

        End Try
    End Sub

#End Region ' Socket event handling

#End Region ' Internals

End Class
