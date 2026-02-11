' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Public Class gridFishingWeights
    Inherits cEwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' Test for UI context to prevent core from being accessed
        If (Me.UIContext Is Nothing) Then Return

        Dim src As cCoreInputOutputBase = Nothing

        Me.Redim(1, 2 + Me.Core.nFleets)

        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

        For iFleet As Integer = 1 To Me.Core.nFleets
            src = Me.Core.EcopathFleetInputs(iFleet)
            Me(0, 1 + iFleet) = New cPropertyColumnHeaderCell(Me.PropertyManager, src, eVarNameFlags.Name, Nothing, cUnits.Currency)
        Next

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
        End Get
    End Property

    Protected Overrides Sub FillData()

        Try

            Dim mse As cMSEManager = Me.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            Dim group As cCoreInputOutputBase = Nothing
            Dim fleet As cMSEFleetInput = Nothing
            ' Dim cell As ICell = Nothing

            ' For each group
            For iGroup As Integer = 1 To Me.Core.nGroups

                Me.AddRow()

                'Get the group info
                group = Me.Core.EcopathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To Me.Core.nFleets
                    fleet = mse.EcopathFleetInputs(iFleet)
                    Me(iGroup, 1 + iFleet) = New cPropertyCell(Me.PropertyManager, fleet, eVarNameFlags.MSEFleetWeight, group)
                Next
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
