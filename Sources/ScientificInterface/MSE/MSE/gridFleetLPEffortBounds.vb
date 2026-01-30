' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridFleetLPEffortBounds
    Inherits cEwEGrid

    Public Sub New()
        ' Set text to use in dock panel
        Me.Text = My.Resources.CAPTION_MSEFLEETREF
    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return True
        End Get
    End Property

    Protected Overrides Sub InitStyle()

        ' ToDo: globalize this

        MyBase.InitStyle()

        Me.Redim(1, 4)
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
        Me(0, 2) = New cEwEColumnHeaderCell("Lower Effort Bound")
        Me(0, 3) = New cEwEColumnHeaderCell("Upper Effort Bound")

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.UIContext.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.UIContext.Core.nFleets

                Me.Rows.Insert(i)
                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSELowerLPEffort)
                Me(i, 3) = New cPropertyCell(Me.PropertyManager, mse.EcopathFleetInputs(i), eVarNameFlags.MSEUpperLPEffort)

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
