
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region

<CLSCompliant(False)> _
Public Class gridGroupRefLevels
    : Inherits EwEGrid


    Private m_core As cCore

    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 5)
        Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 1) = New EwEColumnHeaderCell("Biomass Lower")
        Me(0, 2) = New EwEColumnHeaderCell("Biomass Upper")

        Me(0, 3) = New EwEColumnHeaderCell("Catch Lower")
        Me(0, 4) = New EwEColumnHeaderCell("Catch Upper")

        Me.FixedColumns = 1

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            For igrp As Integer = 1 To m_core.nLivingGroups

                Me.Rows.Insert(igrp)
                Me(igrp, 0) = New PropertyRowHeaderCell(mse.GroupInputs(igrp), eVarNameFlags.Name)
                Me(igrp, 1) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSERefBioLower)
                Me(igrp, 2) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSERefBioUpper)

                Me(igrp, 3) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSERefGroupCatchLower)
                Me(igrp, 4) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSERefGroupCatchUpper)

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
