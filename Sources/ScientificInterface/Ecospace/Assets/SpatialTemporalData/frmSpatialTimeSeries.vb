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
#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEUtils.SpatialData
Imports EwECore.SpatialData

#End Region ' Imports

Namespace Ecospace

    Public Class frmSpatialTimeSeries

#Region " Private vars "

        Private m_thread As Threading.Thread = Nothing
        Private m_ds As ISpatialDataSet = Nothing

#End Region ' Private vars

#Region " Private helper classes "

        Private Class cSpatialDataAdapterItem

            Private m_adt As cSpatialDataAdapter
            Private m_fmt As New cSpatialDataAdapterFormatter()

            Public Sub New(adt As cSpatialDataAdapter)
                Me.m_adt = adt
            End Sub

            Public Overrides Function ToString() As String
                If (Me.m_adt Is Nothing) Then Return ScientificInterfaceShared.My.Resources.GENERIC_VALUE_ALL
                Return Me.m_fmt.GetDescriptor(Me.m_adt)
            End Function

            Public ReadOnly Property Adapter As cSpatialDataAdapter
                Get
                    Return Me.m_adt
                End Get
            End Property
        End Class

#End Region ' Private helper classes

#Region " Form overrides "

        Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
            Get
                Return MyBase.UIContext
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                MyBase.UIContext = value
                Me.m_toolbox.UIContext = value
                Me.m_map.UIContext = value
            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            ' Connect to edit command
            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditSpatialTemporalDataConnections")
            If (cmd IsNot Nothing) Then cmd.AddControl(Me.m_tsbnConnections)

            ' Fill filter combo
            Me.m_tscmTypes.Items.Add(New cSpatialDataAdapterItem(Nothing))
            For Each adt As cSpatialDataAdapter In Me.Core.SpatialDataConnectionManager.Adapters
                Me.m_tscmTypes.Items.Add(New cSpatialDataAdapterItem(adt))
            Next
            Me.m_tscmTypes.SelectedIndex = 0

            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace, eCoreComponentType.External}

        End Sub

        Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)

            If (Me.UIContext Is Nothing) Then Return

            Dim cmd As cCommand = Me.CommandHandler.GetCommand("EditSpatialTemporalDataConnections")
            If (cmd IsNot Nothing) Then cmd.RemoveControl(Me.m_tsbnConnections)

            If Me.HasThread Then
                Me.m_thread.Abort()
                Me.m_thread = Nothing
            End If

            MyBase.OnFormClosed(e)

        End Sub

        Protected Overrides Sub OnStyleGuideChanged(ct As ScientificInterfaceShared.Style.cStyleGuide.eChangeType)
            MyBase.OnStyleGuideChanged(ct)

            If ((ct And cStyleGuide.eChangeType.Colours) > 0) Then
                Me.m_map.Invalidate()
                Me.m_toolbox.Invalidate()
            End If

        End Sub

        Protected Overrides Sub UpdateControls()
            MyBase.UpdateControls()
            Me.m_tsbnZoomData.Checked = (Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Data)
            Me.m_tsbnZoomMap.Checked = (Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Map)
            Me.m_tsbnZoomBoth.Checked = (Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Both)
        End Sub

#End Region ' Form overrides

#Region " Control events "

        Private Sub OnSelectedDatasetChanged(owner As Object, ds As EwEUtils.SpatialData.ISpatialDataSet) _
            Handles m_toolbox.OnSelectedDatasetChanged

            If (Object.ReferenceEquals(ds, Me.m_ds)) Then Return

            If Me.HasThread Then
                Me.m_thread.Abort()
                Me.m_thread = Nothing
            End If

            If (Me.m_ds IsNot Nothing) Then
                ' Clear selection
            End If

            Me.m_ds = ds
            Me.m_map.SelectedDataset = ds

            If (Me.m_ds IsNot Nothing) Then
                Me.m_thread = New Threading.Thread(AddressOf IndexDataset)
                Me.m_thread.Priority = Threading.ThreadPriority.BelowNormal
                Me.m_thread.Start()
            End If

        End Sub

        Private Sub OnSelectType(sender As System.Object, e As System.EventArgs) _
            Handles m_tscmTypes.SelectedIndexChanged

            Dim t As cSpatialDataAdapterItem = DirectCast(Me.m_tscmTypes.SelectedItem, cSpatialDataAdapterItem)
            Dim vn As eVarNameFlags = eVarNameFlags.NotSet

            If (t IsNot Nothing) Then
                If (t.Adapter IsNot Nothing) Then
                    vn = t.Adapter.VarName
                End If
            End If

            Me.m_toolbox.VarName = vn

        End Sub

        Private Sub OnSelectedTimestepChanged(owner As Object, iTimeStep As Integer, dt As Date) _
            Handles m_toolbox.OnSelectedTimestepChanged
            Me.m_map.SelectedTimeStep = iTimeStep
        End Sub

        Private Sub OnZoom(sender As System.Object, e As System.EventArgs) _
            Handles m_tsbnZoomMap.Click, m_tsbnZoomData.Click, m_tsbnZoomBoth.Click
            If (sender Is Me.m_tsbnZoomData) Then
                Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Data
            ElseIf (sender Is Me.m_tsbnZoomMap) Then
                Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Map
            Else
                Me.m_map.ZoomLevel = ucSpatialTimeSeriesMap.eZoomLevel.Both
            End If
            Me.UpdateControls()
        End Sub

#End Region ' Control events

#Region " Callbacks "

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            ' Dataset changes are passed on via core layer changes
            If (msg.DataType = eDataTypes.EcospaceSpatialDataConnection) Then
                Me.m_toolbox.RefreshContent()
                Me.m_map.RefreshContent()
            End If

        End Sub

        Private Delegate Sub OnSpatialIndexUpdatedDelegate(ds As ISpatialDataSet)

        Private Sub OnSpatialIndexUpdated(ds As ISpatialDataSet)
            If (Object.ReferenceEquals(ds, Me.m_ds)) Then
                If Me.InvokeRequired Then
                    Me.Invoke(New OnSpatialIndexUpdatedDelegate(AddressOf OnSpatialIndexUpdated), New Object() {ds})
                Else
                    Me.m_map.RefreshContent()
                    Me.m_toolbox.Invalidate()
                End If
            End If
        End Sub

#End Region ' Callbacks

#Region " Threaded indexing of datasets "

        Private Sub IndexDataset()
            Me.m_ds.BuildIndex(AddressOf OnSpatialIndexUpdated)
        End Sub

        Private Function HasThread() As Boolean
            If (Me.m_thread Is Nothing) Then Return False
            Return Me.m_thread.IsAlive
        End Function

#End Region ' Threaded indexing of datasets

    End Class

End Namespace
