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
Imports System.Text
Imports System.Threading

#End Region ' Imports

''' <summary>
''' A base class for both server and client types who can send text messages to each other.
''' </summary>
Public MustInherit Class cMessageClientServerBase
    Implements IDisposable

#Region " Private vars "

    ''' <summary>
    ''' The block size in which to read incoming messages.
    ''' </summary>
    Private ReadOnly m_iBufferSize As Integer = 1024
    ''' <summary>
    ''' The encoding to use when converting between binary data to text.
    ''' </summary>
    Private ReadOnly m_encoding As Encoding = System.Text.Encoding.ASCII
    ''' <summary>
    ''' Indicates whether or not the current instance has been disposed.
    ''' </summary>
    Private m_bIsDisposed As Boolean = False
    ''' <summary>
    ''' The object used to marshal event handler calls.
    ''' </summary>
    ''' <remarks>
    ''' All events will be raised on the thread on which the current instance was created.
    ''' </remarks>
    Private m_synchronisingContext As SynchronizationContext = SynchronizationContext.Current

#End Region ' Private vars

#Region " Constructors "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageClientServerBase" /> 
    ''' class with a default buffer size and character encoding.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageClientServerBase" /> 
    ''' class with default character encoding.
    ''' </summary>
    ''' <param name="bufferSize">The block size in which to read incoming messages.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal bufferSize As Integer)
        Me.m_iBufferSize = bufferSize
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageClientServerBase" /> 
    ''' class with a default buffer size.
    ''' </summary>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal encoding As Encoding)
        Me.m_encoding = encoding
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageClientServerBase" /> class.
    ''' </summary>
    ''' <param name="bufferSize">The block size in which to read incoming messages.</param>
    ''' <param name="encoding">The encoding to use when converting between binary data to text.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal bufferSize As Integer, ByVal encoding As Encoding)
        Me.m_iBufferSize = bufferSize
        Me.m_encoding = encoding
    End Sub

#End Region ' Constructors

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the size of the blocks in which incoming messages are read.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property BufferSize() As Integer
        Get
            Return Me.m_iBufferSize
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the encoding used to convert text messages to binary data for transmission.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property Encoding() As Encoding
        Get
            Return Me.m_encoding
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the encoding used to convert text messages to binary data for transmission.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected ReadOnly Property SynchronisingContext() As SynchronizationContext
        Get
            Return Me.m_synchronisingContext
        End Get
    End Property

#End Region ' Properties

#Region " Events "

    ''' -----------------------------------------------------------------------
    ''' <summary>Event to notify that a connection attempt was accepted.</summary>
    ''' -----------------------------------------------------------------------
    Public Event ConnectionAccepted As ConnectionEventHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>Event to notify that a connection was closed.</summary>
    ''' -----------------------------------------------------------------------
    Public Event ConnectionClosed As ConnectionEventHandler

    ''' -----------------------------------------------------------------------
    ''' <summary>Event to notify that a message was received.</summary>
    ''' -----------------------------------------------------------------------
    Public Event MessageReceived As MessageReceivedEventHandler

#End Region ' Events

#Region " Public Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Releases all resources used by the object.
    ''' </summary>
    ''' <remarks>
    ''' This method should be called when the object is no longer needed.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region ' Public Methods

#Region " Protected Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Releases all resources used by the object.
    ''' </summary>
    ''' <param name="bDisposing">Raah!</param>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub Dispose(ByVal bDisposing As Boolean)
        If Not Me.m_bIsDisposed Then
            If bDisposing Then
            End If

            Me.m_synchronisingContext = Nothing
        End If

        Me.m_bIsDisposed = True
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionAccepted" /> event.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' <remarks>
    ''' The event will be raised on the thread on which the current instance was created.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnConnectionAccepted(ByVal e As cConnectionEventArgs)
        Me.m_synchronisingContext.Post(AddressOf RaiseConnectionAccepted, e)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionClosed" /> event.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' <remarks>
    ''' The event will be raised on the thread on which the current instance was created.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnConnectionClosed(ByVal e As cConnectionEventArgs)
        Me.m_synchronisingContext.Post(AddressOf RaiseConnectionClosed, e)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="MessageReceived" /> event.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' <remarks>
    ''' The event will be raised on the thread on which the current instance was created.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overridable Sub OnMessageReceived(ByVal e As cMessageReceivedEventArgs)
        Me.m_synchronisingContext.Post(AddressOf RaiseMessageReceived, e)
    End Sub

#End Region ' Protected Methods

#Region " Private Methods "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionAccepted" /> event on the current thread.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RaiseConnectionAccepted(ByVal e As Object)
        RaiseEvent ConnectionAccepted(Me, DirectCast(e, cConnectionEventArgs))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="ConnectionClosed" /> event on the current thread.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RaiseConnectionClosed(ByVal e As Object)
        RaiseEvent ConnectionClosed(Me, DirectCast(e, cConnectionEventArgs))
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Raises the <see cref="MessageReceived" /> event on the current thread.
    ''' </summary>
    ''' <param name="e">The data for the event.</param>
    ''' -----------------------------------------------------------------------
    Private Sub RaiseMessageReceived(ByVal e As Object)
        RaiseEvent MessageReceived(Me, DirectCast(e, cMessageReceivedEventArgs))
    End Sub

#End Region ' Private Methods

End Class
