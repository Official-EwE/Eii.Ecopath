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
Option Strict On
Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.Linq

''' <summary>
''' Receives connection requests from clients and receives and sends text messages over those connections.
''' </summary>
Public Class cMessageServer
    Inherits cMessageClientServerBase

#Region " Types "

    ''' <summary>
    ''' Contains application-specific state data for an asynchronous read.
    ''' </summary>
    Private Class cReadAsyncState
        ''' <summary>The client receiving the data.</summary>
        Public Client As TcpClient
        ''' <summary>A byte array the data is read into.</summary>
        Public Buffer As Byte()
    End Class

#End Region ' Types

#Region " Private vars "

    ''' <summary>The connected clients and the corresponding remote host information.</summary>
    Private ReadOnly m_dicClients As New Dictionary(Of TcpClient, sHostInfo)
    ''' <summary>Indicates whether or not the current instance has been disposed.</summary>
    Private m_bIsDisposed As Boolean = False
    ''' <summary>The port on which the server is listening for connections.</summary>
    Private m_iPort As Integer
    ''' <summary>The server listening for connections.</summary>
    Private m_server As TcpListener

#End Region ' Private vars

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding listening on any local IP 
    ''' address and a random port.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.Initialise(IPAddress.Any, 0)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding listening on a random port.
    ''' </summary>
    ''' <param name="strAddress">The IP address or host name on which to listen.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As String)
        Me.Initialise(Me.ParseAddress(strAddress), 0)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding listening on a random port.
    ''' </summary>
    ''' <param name="strAddress">The IP address on which to listen.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As IPAddress)
        Me.Initialise(strAddress, 0)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding listening on any local IP address.
    ''' </summary>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal iPort As Integer)
        Me.Initialise(IPAddress.Any, iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding.
    ''' </summary>
    ''' <param name="strAddress">The IP address or host name on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As String, ByVal iPort As Integer)
        Me.Initialise(Me.ParseAddress(strAddress), iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with 
    ''' the default buffer size and character encoding.
    ''' </summary>
    ''' <param name="strAddress">The IP address on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As IPAddress, ByVal iPort As Integer)
        Me.Initialise(strAddress, iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with the default character encoding.
    ''' </summary>
    ''' <param name="strAddress">The IP address or host name on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="iBufferSize">The block size in which to read incoming messages.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As String, _
                   ByVal iPort As Integer, _
                   ByVal iBufferSize As Integer)
        MyBase.New(iBufferSize)
        Me.Initialise(Me.ParseAddress(strAddress), iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with the default buffer size.
    ''' </summary>
    ''' <param name="strAddress">The IP address or host name on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As String, _
                   ByVal iPort As Integer, _
                   ByVal encoding As Text.Encoding)
        MyBase.New(encoding)
        Me.Initialise(Me.ParseAddress(strAddress), iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class.
    ''' </summary>
    ''' <param name="strAddress">The IP address or host name on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="iBufferSize">The block size in which to read incoming messages.</param>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As String, _
                   ByVal iPort As Integer, _
                   ByVal iBufferSize As Integer, _
                   ByVal encoding As Encoding)
        MyBase.New(iBufferSize, encoding)
        Me.Initialise(Me.ParseAddress(strAddress), iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with the default character encoding.
    ''' </summary>
    ''' <param name="strAddress">The IP address on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="iBufferSize">The block size in which to read incoming messages.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As IPAddress, _
                   ByVal iPort As Integer, _
                   ByVal iBufferSize As Integer)
        MyBase.New(iBufferSize)
        Me.Initialise(strAddress, iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class with the default buffer size.
    ''' </summary>
    ''' <param name="strAddress">The IP address on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As IPAddress, _
                   ByVal iPort As Integer, _
                   ByVal encoding As Encoding)
        MyBase.New(encoding)
        Me.Initialise(strAddress, iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageServer" /> class.
    ''' </summary>
    ''' <param name="strAddress">The IP address on which to listen.</param>
    ''' <param name="iPort">The port on which to listen for incoming connections.</param>
    ''' <param name="iBufferSize">The block size in which to read incoming messages.</param>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strAddress As IPAddress, _
                   ByVal iPort As Integer, _
                   ByVal iBufferSize As Integer, _
                   ByVal encoding As Encoding)
        MyBase.New(iBufferSize, encoding)
        Me.Initialise(strAddress, iPort)
    End Sub

#End Region ' Constructors

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Gets the port number on which the server is listening.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Port() As Integer
        Get
            Return Me.m_iPort
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get a list of remote hosts connected to the server.
    ''' </summary>
    ''' <remarks>
    ''' The list generated ad hoc so the property value should not be cached.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Hosts() As sHostInfo()
        Get
            Return Me.m_dicClients.Values.ToArray()
        End Get
    End Property

#End Region ' Properties

#Region " Public Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends a message to the specified host.
    ''' </summary>
    ''' <param name="strHostName">The name or address of the host to send the message to.</param>
    ''' <param name="iPort">The port number of the host to send the message to.</param>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Send(ByVal strHostName As String, ByVal iPort As Integer, ByVal strMessage As String)
        Dim client = (From c In Me.m_dicClients.Keys _
                      Let h = Me.m_dicClients(c) _
                      Where h.HostName = strHostName AndAlso h.Port = iPort _
                      Select c).First()

        Me.Send(client, strMessage)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends a message to the specified host.
    ''' </summary>
    ''' <param name="host">The remote host to send the message to.</param>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Send(ByVal host As sHostInfo, ByVal strMessage As String)
        Dim client = (From c In Me.m_dicClients.Keys _
                      Where Me.m_dicClients(c).Equals(host) _
                      Select c).First()

        Me.Send(client, strMessage)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends a message to all connected hosts.
    ''' </summary>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Send(ByVal strMessage As String)
        For Each client In Me.m_dicClients.Keys
            Me.Send(client, strMessage)
        Next
    End Sub

#End Region ' Public Methods

#Region " Protected Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Releases all resources used by the object.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not Me.m_bIsDisposed Then
            If disposing Then
                'Close all remaining connections
                For Each client In Me.m_dicClients.Keys
                    client.GetStream().Close()
                    client.Close()
                Next

                'Stop listening for connection requests.
                Me.m_server.Stop()
            End If

            Me.m_dicClients.Clear()
        End If

        MyBase.Dispose(disposing)
        Me.m_bIsDisposed = True
    End Sub

#End Region ' Protected Methods

#Region " Private Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Accepts an incoming connection request.
    ''' </summary>
    ''' <param name="ar">Contains state information for the connection operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub AcceptTcpClient(ByVal ar As IAsyncResult)
        Try
            Dim client = Me.m_server.EndAcceptTcpClient(ar)
            Dim host As sHostInfo

            With DirectCast(client.Client.RemoteEndPoint, IPEndPoint)
                host = New sHostInfo(.Address.ToString, .Port)
            End With

            'Remember the client and its host information.
            Me.m_dicClients.Add(client, host)

            'Listen asynchronously for more connections.
            Me.m_server.BeginAcceptTcpClient(AddressOf AcceptTcpClient, Nothing)

            Dim stream = client.GetStream()
            Dim buffer(Me.BufferSize - 1) As Byte

            'Listen asynchronously for incoming messages from this client.
            stream.BeginRead(buffer, 0, Me.BufferSize, AddressOf Read, New cReadAsyncState With {.Client = client, .Buffer = buffer})

            'Notify any listeners that a connection has been made.
            Me.OnConnectionAccepted(New cConnectionEventArgs(host))
        Catch ex As ObjectDisposedException
            'The callback specified when BeginAcceptTcpClient was called gets invoked one last time when the TcpListener is stopped.
            'This exception is thrown when EndAcceptTcpClient is called on a disposed server.
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates the server and starts listening for incoming connection requests.
    ''' </summary>
    ''' <param name="port">The port to listen on.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Initialise(ByVal ipAddress As IPAddress, ByVal port As Integer)
        'Listen on the first IPv4 address assigned to the local machine.
        Me.m_server = New TcpListener(ipAddress, port)
        Me.m_server.Start()

        'Get the port number from the server in case a random port was used.
        Me.m_iPort = DirectCast(Me.m_server.LocalEndpoint, IPEndPoint).Port

        'Start listen asynchronously.
        Me.m_server.BeginAcceptTcpClient(AddressOf AcceptTcpClient, Nothing)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Converts an address string to an <see cref="IPAddress" /> instance.
    ''' </summary>
    ''' <param name="address">The address to parse.</param>
    ''' <returns>An <see cref="IPAddress">IPAddress</see> corresponding to the specified address.</returns>
    ''' <remarks>
    ''' If <i>address</i> is null or empty or is equal to the machine name then <see cref="IPAddress.Any">Any</see> is returned.
    ''' If <i>address</i> is equal to "localhost" then <see cref="IPAddress.Loopback">Loopback</see> is returned.
    ''' In each case comparisons are case-insensitive.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function ParseAddress(ByVal address As String) As IPAddress
        Dim result As IPAddress

        If String.IsNullOrEmpty(address) OrElse _
           address.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase) Then
            result = IPAddress.Any
        ElseIf address.Equals("localhost", StringComparison.CurrentCultureIgnoreCase) Then
            result = IPAddress.Loopback
        Else
            result = IPAddress.Parse(address)
        End If

        Return result
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Receives an incoming message.
    ''' </summary>
    ''' <param name="ar">Contains state information for the read operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Read(ByVal ar As IAsyncResult)
        Dim asyncState = DirectCast(ar.AsyncState, cReadAsyncState)
        Dim buffer = asyncState.Buffer
        Dim client = asyncState.Client

        Try
            Dim stream = client.GetStream()

            'Complete the asynchronous read and get the first block of data.
            Dim byteCount = stream.EndRead(ar)

            If byteCount = 0 Then
                'If there is no data when an asynchronous read completes it is because the client closed the connection.
                Me.RemoveClient(client)
            Else
                'Start building the message.
                Dim message As New StringBuilder(Me.Encoding.GetString(buffer, 0, byteCount))

                'As long as there is more data...
                While stream.DataAvailable
                    '...read another block of data.
                    byteCount = stream.Read(buffer, 0, Me.BufferSize)

                    'Build the message block by block.
                    message.Append(Me.Encoding.GetString(buffer, 0, byteCount))
                End While

                'Listen asynchronously for another incoming message.
                stream.BeginRead(buffer, 0, Me.BufferSize, AddressOf Read, New cReadAsyncState With {.Client = client, .Buffer = buffer})

                'Notify any listeners that a message was received.
                Me.OnMessageReceived(New cMessageReceivedEventArgs(Me.m_dicClients(client), message.ToString()))
            End If
        Catch ex As InvalidOperationException
            'The callback specified when BeginRead was called gets invoked one last time when the TcpListener is stopped.
            'This exception is thrown when GetStream is called on a disconnected client or EndRead is called on a disposed stream.
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Removes a client from the list of connected clients.
    ''' </summary>
    ''' <param name="client">The client to remove.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RemoveClient(ByVal client As TcpClient)
        Dim host = Me.m_dicClients(client)

        Me.m_dicClients.Remove(client)

        'Notify any listeners that the host has disconnected.
        Me.OnConnectionClosed(New cConnectionEventArgs(host))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends a message to a client.
    ''' </summary>
    ''' <param name="client">The client to send the message to.</param>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Send(ByVal client As TcpClient, ByVal strMessage As String)
        Dim stream = client.GetStream()
        Dim buffer As Byte() = Me.Encoding.GetBytes(strMessage)

        'Send the message asynchronously.
        stream.BeginWrite(buffer, 0, buffer.Length, AddressOf Write, stream)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Completes writing a message to a client.
    ''' </summary>
    ''' <param name="ar">Contains state information for the write operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Write(ByVal ar As IAsyncResult)
        'Complete the asynchronous write.
        DirectCast(ar.AsyncState, NetworkStream).EndWrite(ar)
    End Sub

#End Region ' Private Methods

End Class
