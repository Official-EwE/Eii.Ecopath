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

Imports System.Net.Sockets
Imports System.Net
Imports System.Text
Imports System.IO

#End Region ' Imports

''' <summary>
''' Initiates a connection with a server and sends ands receives text messages over that connection.
''' </summary>
Public Class cMessageClient
    Inherits cMessageClientServerBase

#Region " Private vars "

    ''' <summary>The client to communicate with the server.</summary>
    Private ReadOnly m_client As New TcpClient()
    ''' <summary>Indicates whether the current instance has been disposed.</summary>
    Private m_bIsDisposed As Boolean = False
    ''' <summary>The port number of the local end of the connection.</summary>
    Private m_iPort As Integer = 42
    ''' <summary>The details of the server to connect to.</summary>
    Private m_server As sHostInfo = Nothing
    ''' <summary>The stream over which messages are sent and received.</summary>
    Private m_stream As NetworkStream = Nothing

#End Region ' Private vars

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the port number of the local end of the connection.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property LocalPort() As Integer
        Get
            Return Me.m_iPort
        End Get
    End Property

#End Region 'Properties

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event to signal that a connection attempt failed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Event ConnectionFailed As ConnectionEventHandler

#End Region 'Events

#Region " Public Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Connects to the server.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Connect(ByVal strHostName As String, ByVal iPort As Integer)
        Me.Initialise(strHostName, iPort)
        Me.m_client.BeginConnect(Me.m_server.HostName, Me.m_server.Port, AddressOf Connect, Nothing)
    End Sub

    Public Sub Disconnect()
        If Me.m_client.Connected Then
            Me.m_client.Client.Disconnect(True)
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Sends a message to the server.
    ''' </summary>
    ''' <param name="strMessage">The message to send.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Send(ByVal strMessage As String)
        Dim buffer As Byte() = Me.Encoding.GetBytes(strMessage)
        Me.m_stream.BeginWrite(buffer, 0, buffer.Length, AddressOf Write, Nothing)
    End Sub

#End Region 'Public Methods

#Region " Protected Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Releases all resources used by the object.
    ''' </summary>
    ''' <param name="disposing">Aargh.</param>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not Me.m_bIsDisposed Then
            If disposing Then
                'Close the connection and the underlying stream.
                If Me.m_stream IsNot Nothing Then
                    Me.m_stream.Close()
                End If

                Me.m_client.Close()
            End If

            Me.m_stream = Nothing
        End If

        MyBase.Dispose(disposing)
        Me.m_bIsDisposed = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionFailed" /> event.
    ''' </summary>
    ''' <param name="e">
    ''' Contains the data for the event.
    ''' </param>
    ''' <remarks>
    ''' The event will be raised on the thread on which the current instance was created.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnConnectionFailed(ByVal e As cConnectionEventArgs)
        Me.SynchronisingContext.Post(AddressOf RaiseConnectionFailed, e)
    End Sub

#End Region 'Protected Methods

#Region " Private Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Completes a connection to the server.
    ''' </summary>
    ''' <param name="ar">Contains state information for the connection operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Connect(ByVal ar As IAsyncResult)
        Try
            'Complete the asynchronous connection.
            Me.m_client.EndConnect(ar)

            'Get the port that was assigned to the local end point.
            Me.m_iPort = DirectCast(Me.m_client.Client.LocalEndPoint, IPEndPoint).Port

            Me.m_stream = Me.m_client.GetStream()

            Dim buffer(Me.BufferSize - 1) As Byte

            'Listen asynchronously for an incoming message.
            Me.m_stream.BeginRead(buffer, 0, Me.BufferSize, AddressOf Read, buffer)

            'Notify any listeners that the connection was successful.
            Me.OnConnectionAccepted(New cConnectionEventArgs(Me.m_server))
        Catch ex As SocketException
            'The specified server was not found.
            Me.OnConnectionFailed(New cConnectionEventArgs(Me.m_server))
        End Try
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a <see cref="sHostInfo" /> to represent the server.
    ''' </summary>
    ''' <param name="strHostName">The name or address of the server.</param>
    ''' <param name="iPort">The port number of the server.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Initialise(ByVal strHostName As String, ByVal iPort As Integer)
        Me.m_server = New sHostInfo(strHostName, iPort)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionFailed" /> event on the current thread.
    ''' </summary>
    ''' <param name="e">Contains the data for the event.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RaiseConnectionFailed(ByVal e As Object)
        RaiseEvent ConnectionFailed(Me, DirectCast(e, cConnectionEventArgs))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Receives an incoming message.
    ''' </summary>
    ''' <param name="ar">Contains state information for the read operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Read(ByVal ar As IAsyncResult)
        Try
            ' The stream will be Nothing if the client has been disposed.
            If Me.m_stream IsNot Nothing Then

                Dim abBuffer = DirectCast(ar.AsyncState, Byte())
                ' Complete the asynchronous read and get the first block of data.
                Dim iNumBytes = Me.m_stream.EndRead(ar)

                ' Did the server close the connection?
                If (iNumBytes = 0) Then
                    ' #Yes: raise event
                    Me.OnConnectionClosed(New cConnectionEventArgs(Me.m_server))
                Else
                    ' #No: Start building the message.
                    Dim sbMessage As New StringBuilder(Me.Encoding.GetString(abBuffer, 0, iNumBytes))
                    ' As long as there is more data...
                    While Me.m_stream.DataAvailable
                        ' ...read another block of data.
                        iNumBytes = Me.m_stream.Read(abBuffer, 0, Me.BufferSize)
                        ' Build the message block by block.
                        sbMessage.Append(Me.Encoding.GetString(abBuffer, 0, iNumBytes))
                    End While

                    ' Notify any listeners that a message was received.
                    Me.OnMessageReceived(New cMessageReceivedEventArgs(Me.m_server, sbMessage.ToString()))
                    ' Listen asynchronously for another incoming message.
                    Me.m_stream.BeginRead(abBuffer, 0, Me.BufferSize, AddressOf Read, abBuffer)

                End If
            End If

        Catch ex As IOException
            'The callback specified when BeginRead was called may get invoked one last time when the TcpClient is disposed.
            'This exception is thrown when EndRead is called on a disposed client stream.
        End Try

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Completes writing a message to the server.
    ''' </summary>
    ''' <param name="ar">Contains state information for the write operation.</param>
    ''' -----------------------------------------------------------------------
    Private Sub Write(ByVal ar As IAsyncResult)
        ' Complete the asynchronous write.
        Me.m_stream.EndWrite(ar)
    End Sub

#End Region 'Private Methods

End Class
