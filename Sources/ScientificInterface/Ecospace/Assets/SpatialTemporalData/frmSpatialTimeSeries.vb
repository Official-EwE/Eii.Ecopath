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
Imports EwEUtils.Commands
Imports EwEUtils.SpatialData

Namespace Ecospace

    Public Class frmSpatialTimeSeries

        Private m_thread As Threading.Thread = Nothing
        Private m_ds As ISpatialDataSet = Nothing

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_ucDatasets.UIContext = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return
            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditSpatialTemporalDataConnections")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnConnections)

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace, eCoreComponentType.External}
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            If (Me.UIContext Is Nothing) Then Return
            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditSpatialTemporalDataConnections")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnConnections)
            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnSelectedDatasetChanged(owner As Object, ds As EwEUtils.SpatialData.ISpatialDataSet) _
            Handles m_ucDatasets.OnSelectedDatasetChanged

            If (Object.ReferenceEquals(ds, Me.m_ds)) Then Return

            If Me.HasThread Then
                Me.m_thread.Abort()
                Me.m_thread = Nothing
            End If

            If (Me.m_ds IsNot Nothing) Then
                ' Clear selection
            End If

            Me.m_ds = ds

            If (Me.m_ds IsNot Nothing) Then
                Me.m_thread = New Threading.Thread(AddressOf IndexDataset)
                Me.m_thread.Priority = Threading.ThreadPriority.BelowNormal
                Me.m_thread.Start()
            End If

        End Sub

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            ' Dataset changes are passed on via core layer changes
            If (msg.DataType = eDataTypes.EcospaceSpatialDataConnection) Then
                Me.m_ucDatasets.RefreshContent()
            End If

        End Sub

#Region " Threaded indexing of datasets "

        Private Sub IndexDataset()
            Me.m_ds.BuildIndex()
        End Sub

        Private Function HasThread() As Boolean
            If (Me.m_thread Is Nothing) Then Return False
            Return Me.m_thread.IsAlive
        End Function

#End Region ' Threaded indexing of datasets

    End Class

End Namespace
