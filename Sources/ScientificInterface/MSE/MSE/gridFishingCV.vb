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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class gridFishingCV
    : Inherits cEwEGrid

    Public Sub New()
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 3)
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
        Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_INCREASEQ)
        'Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_CV)

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.Core.nFleets

                Me.Rows.Insert(i)

                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSEQIncrease)
                ' Me(i, 2) = New PropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSEFleetCV)

            Next

        Catch ex As Exception
            Debug.Assert(False)
        End Try

    End Sub

    Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
        Get
            Return eCoreComponentType.MSE
        End Get
    End Property

End Class
