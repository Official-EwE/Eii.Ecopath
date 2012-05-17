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

    Public Class cSpatialOperationLog

        Private m_core As cCore
        Private m_msgCurrent As cMessage = Nothing
        Private m_vn As eVarNameFlags = eVarNameFlags.NotSet
        Private m_iIndex As Integer = cCore.NULL_VALUE

        Public Sub New(core As cCore)
            Me.m_core = core
        End Sub

        Friend Sub BeginProcessLayer(timestep As Integer, dt As DateTime, strLayerName As String, vn As eVarNameFlags, iIndex As Integer)
            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.EndProcessLayer()
            End If
            Me.m_vn = vn
            Me.m_iIndex = iIndex
            Me.m_msgCurrent = New cMessage("Spatial data for timestep " & timestep & " (" & dt.ToString("MMM yyyy") & "), layer " & strLayerName, eMessageType.GISOperation, eCoreComponentType.External, eMessageImportance.Information)
        End Sub

        Friend Sub EndProcessLayer()
            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.m_core.Messages.SendMessage(Me.m_msgCurrent)
                Me.m_msgCurrent = Nothing
            End If
        End Sub

        Public Sub AddMessage(strMsg As String, status As eStatusFlags)
            If (Me.m_msgCurrent IsNot Nothing) Then
                Me.m_msgCurrent.AddVariable(New cVariableStatus(status, strMsg, Me.m_vn, eDataTypes.External, eCoreComponentType.External, Me.m_iIndex))
            End If
        End Sub

    End Class

End Namespace
