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

''' <summary>
''' Data for the <see cref="cMessageClientServerBase.MessageReceived">MessageReceived</see> event.
''' </summary>
Public Class cMessageReceivedEventArgs
    Inherits cConnectionEventArgs

    ''' <summary>The message that was received.</summary>
    Private ReadOnly m_strMessage As String

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Creates a new instance of the <see cref="cMessageReceivedEventArgs" /> class.
    ''' </summary>
    ''' <param name="host">The remote host.</param>
    ''' <param name="strMessage">The message that was received.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal host As sHostInfo, ByVal strMessage As String)
        MyBase.New(host)
        Me.m_strMessage = strMessage
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get the message that was received.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Message() As String
        Get
            Return Me.m_strMessage
        End Get
    End Property

End Class
