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

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Contains data for connection-related events.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cConnectionEventArgs
    Inherits EventArgs

#Region " Private vars "

    ''' <summary>Information about the remote host.</summary>
    Private ReadOnly m_host As sHostInfo = Nothing

#End Region ' Private vars

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cConnectionEventArgs" /> class.
    ''' </summary>
    ''' <param name="host">The remote host.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal host As sHostInfo)
        Me.m_host = host
    End Sub

#End Region ' Constructor

#Region " Properties "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get information about the remote host.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Host() As sHostInfo
        Get
            Return Me.m_host
        End Get
    End Property

#End Region ' Properties

End Class
