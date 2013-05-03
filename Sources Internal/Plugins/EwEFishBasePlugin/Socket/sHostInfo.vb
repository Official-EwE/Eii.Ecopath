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

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Structure that contains information about a remote host.
''' </summary>
''' -----------------------------------------------------------------------
Public Structure sHostInfo

#Region " Private vars "

    ''' <summary>The name of the remote host.</summary>
    Private ReadOnly m_strHostName As String
    ''' <summary>The port number of the remote host.</summary>
    Private ReadOnly m_iPort As Integer

#End Region ' Private vars

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="sHostInfo"/> class.
    ''' </summary>
    ''' <param name="strHostName">The name of the remote host.</param>
    ''' <param name="iPort">The port number of the remote host.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal strHostName As String, ByVal iPort As Integer)
        Me.m_strHostName = strHostName
        Me.m_iPort = iPort
    End Sub


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Compares the current instance to another object for value equality.
    ''' </summary>
    ''' <param name="host">
    ''' The object to which the current instance is compared.
    ''' </param>
    ''' <returns>
    ''' <b>true</b> if both the host name and port of both instances are the same; otherwise, <b>False</b>.
    ''' </returns>
    ''' <remarks>
    ''' The comparison of host names is case-insensitive.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overloads Function Equals(ByVal host As sHostInfo) As Boolean
        Return Me.HostName.Equals(host.HostName, StringComparison.CurrentCultureIgnoreCase) AndAlso _
               Me.Port = host.Port
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Compares the current instance to another object for value equality.
    ''' </summary>
    ''' <param name="obj">
    ''' The object to which the current instance is compared.
    ''' </param>
    ''' <returns>
    ''' True if the host name port of both instances are the same.
    ''' </returns>
    ''' <remarks>
    ''' Comparison of host names is case-insensitive.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        Return TypeOf obj Is sHostInfo AndAlso _
               Me.Equals(DirectCast(obj, sHostInfo))
    End Function

#Region " Public properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the remote host.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property HostName() As String
        Get
            Return Me.m_strHostName
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the port number of the remote host.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Port() As Integer
        Get
            Return m_iPort
        End Get
    End Property

#End Region ' Public properties

End Structure
