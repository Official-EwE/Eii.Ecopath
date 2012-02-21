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

Namespace Ecospace

    Public Class frmSpatialTimeSeries

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
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.EcoSpace, eCoreComponentType.External}
        End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            MyBase.OnFormClosed(e)
        End Sub

        Private Sub OnSelectedDatasetChanged(owner As Object, ds As EwEUtils.SpatialData.ISpatialDataSet) Handles m_ucDatasets.OnSelectedDatasetChanged
            If (ds IsNot Nothing) Then MsgBox(ds.DisplayName)
        End Sub

        Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)
            MyBase.OnCoreMessage(msg)

            ' Dataset changes are passed on via core layer changes
            If (msg.DataType = eDataTypes.EcospaceSpatialDataConnection) Then
                Me.m_ucDatasets.RefreshContent()
            End If

        End Sub

    End Class

End Namespace
