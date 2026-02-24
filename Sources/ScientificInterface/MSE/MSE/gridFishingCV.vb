' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridFishingCV
    Inherits cEwEGrid

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
