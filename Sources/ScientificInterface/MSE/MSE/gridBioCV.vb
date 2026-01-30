' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources


Public Class gridBioCV
    Inherits cEwEGrid

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 3)
        Me(0, 0) = New cEwEColumnHeaderCell("")
        Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
        Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_CV)
        '
        Me.FixedColumns = 1

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

            For i As Integer = 1 To Me.Core.nLivingGroups

                Me.AddRow()

                Me(i, 0) = New cEwERowHeaderCell(CStr(i))
                Me(i, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, mse.GroupInputs(i), eVarNameFlags.Name)
                '  Me(i, 2) = New PropertyCell(Me.PropertyManager, mse.GroupInputs(i), eVarNameFlags.MSEBioCV)
                '
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
