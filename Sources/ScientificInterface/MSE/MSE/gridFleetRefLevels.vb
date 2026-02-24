' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridFleetRefLevels
    Inherits cEwEGrid

    Public Sub New()
        ' Set text to use in dock panel
        Me.Text = My.Resources.CAPTION_MSEFLEETREF
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Redim(1, 6)
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
        Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_CATCH_LOWER)
        Me(0, 3) = New cEwEColumnHeaderCell(SharedResources.HEADER_CATCH_UPPER)
        Me(0, 4) = New cEwEColumnHeaderCell(SharedResources.HEADER_EFFORT_LOWER)
        Me(0, 5) = New cEwEColumnHeaderCell(SharedResources.HEADER_EFFORT_UPPER)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.UIContext.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.UIContext.Core.nFleets

                Me.Rows.Insert(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSERefFleetCatchLower)
                Me(i, 3) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSERefFleetCatchUpper)
                Me(i, 4) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSERefFleetEffortLower)
                Me(i, 5) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSERefFleetEffortUpper)

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
