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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports EwEUtils.Core

Namespace SpatialData

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Central class for logging spatial operations.
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' The operations log generates <see cref="cMessage">core messages</see>, 
    ''' one message for a  given layer and time step, with <see cref="cMessage.Variables"/> 
    ''' for every spatial operation applied to the data for this layer.
    ''' </para>
    ''' <para>
    ''' To do this, the log behaves in a transaction-based manner. A layer log message 
    ''' is started by calling <see cref="cSpatialOperationLog.BeginLayerLog"/>. 
    ''' Consecutive <see cref="cSpatialOperationLog.LogOperation">LogOperation</see> calls
    ''' will add <see cref="cVariableStatus"/> entries to the message. The message
    ''' is terminated and sent out by calling <see cref="cSpatialOperationLog.EndLayerLog"/>.
    ''' </para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class cSpatialOperationLog

        ' ToDo: add auto-file generation: observe core messages, start log file for space scenario name, write messages, etc

#Region " Private vars "

        Private m_core As cCore = Nothing
        Private m_vn As eVarNameFlags = eVarNameFlags.NotSet
        Private m_iIndex As Integer = cCore.NULL_VALUE

        Private m_msgCurrent As cMessage = Nothing

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize a new instance of this class.
        ''' </summary>
        ''' <param name="core">The core to use for sending messages.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

#End Region ' Construction / destruction

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Notify the log that the spatial framework has begun processing external
        ''' data for a map layer at a given time step. To finish processing
        ''' the layer call <see cref="EndLayerLog"/>.
        ''' </summary>
        ''' <param name="timestep">The time step that is currently being executed.</param>
        ''' <param name="dt">The absolute time represented by this time step.</param>
        ''' <param name="layer">The layer that is being processed.</param>
        ''' -------------------------------------------------------------------
        Friend Sub BeginLayerLog(ByVal timestep As Integer, ByVal dt As DateTime, ByVal layer As cEcospaceLayer)

            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.EndLayerLog()
            End If

            Me.m_msgCurrent = New cMessage(String.Format(My.Resources.CoreMessages.STATUS_SPATIALTEMPORAL_LOADING, layer.Name, timestep, dt), _
                                           eMessageType.GISOperation, eCoreComponentType.External, eMessageImportance.Information)
            Me.m_vn = layer.VarName
            Me.m_iIndex = layer.Index

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Notify the log that the spatial framework is finished processing
        ''' external data for the layer and time step indicated in <see cref="BeginLayerLog"/>.
        ''' </summary>
        ''' <param name="bSendMessage">Flag, stating whether a status message
        ''' should be sent out. True by default.</param>
        ''' -------------------------------------------------------------------
        Friend Sub EndLayerLog(Optional bSendMessage As Boolean = True)

            If (Me.m_msgCurrent IsNot Nothing) And (bSendMessage = True) Then
                Me.m_core.Messages.SendMessage(Me.m_msgCurrent)
            End If

            Me.m_msgCurrent = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Log a spatial operation for the current layer.
        ''' </summary>
        ''' <param name="strMsg">The status message describing the operation.</param>
        ''' <param name="status">The result of the operation.</param>
        ''' -------------------------------------------------------------------
        Public Sub LogOperation(strMsg As String, status As eStatusFlags)

            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.m_msgCurrent.AddVariable(New cVariableStatus(status, strMsg, Me.m_vn, eDataTypes.External, eCoreComponentType.External, Me.m_iIndex))
            End If

        End Sub

    End Class

End Namespace
