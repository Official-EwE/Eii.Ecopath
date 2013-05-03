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

''' ---------------------------------------------------------------------------
''' <summary>
''' Represents the method that will handle the 
''' <see cref="cMessageClientServerBase.ConnectionAccepted">ConnectionAccepted</see> and
''' <see cref="cMessageClientServerBase.ConnectionClosed">ConnectionClosed</see> events of a 
''' <see cref="cMessageClientServerBase" />.
''' </summary>
''' <param name="sender">The source of the event.</param>
''' <param name="e">A <see cref="cConnectionEventArgs" /> that contains the event data.</param>
''' ---------------------------------------------------------------------------
Public Delegate Sub ConnectionEventHandler(ByVal sender As Object, ByVal e As cConnectionEventArgs)

''' ---------------------------------------------------------------------------
''' <summary>
''' Represents the method that will handle the 
''' <see cref="cMessageClientServerBase.MessageReceived">MessageReceived</see> event of a 
''' <see cref="cMessageClientServerBase" />.
''' </summary>
''' <param name="sender">The source of the event.</param>
''' <param name="e">A <see cref="cMessageReceivedEventArgs" /> that contains the event data.</param>
''' ---------------------------------------------------------------------------
Public Delegate Sub MessageReceivedEventHandler(ByVal sender As Object, ByVal e As cMessageReceivedEventArgs)
