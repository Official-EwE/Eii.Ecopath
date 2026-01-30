' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class gridRiskBounds
    Inherits cEwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 4)
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_MSE_LOWERRISK)
        Me(0, 3) = New cEwEColumnHeaderCell(SharedResources.HEADER_MSE_UPPERRISK)

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

            For igrp As Integer = 1 To Me.Core.nLivingGroups

                Me.Rows.Insert(igrp)
                Me(igrp, 0) = New cEwERowHeaderCell(CStr(igrp))
                Me(igrp, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.Name)
                Me(igrp, 2) = New cPropertyCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.MSELowerRisk)
                Me(igrp, 3) = New cPropertyCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.MSEUpperRisk)

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
