
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region


<CLSCompliant(False)> _
Public Class gridCatchabilityIncrease
    : Inherits EwEGrid

    Private m_core As cCore

    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 2)
        Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
        Me(0, 1) = New EwEColumnHeaderCell("Max. annual increase in catchability")

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            For iflt As Integer = 1 To m_core.nFleets

                Me.Rows.Insert(iflt)

                Me(iflt, 0) = New PropertyRowHeaderCell(mse.FleetInputs(iflt), eVarNameFlags.Name)
                Me(iflt, 1) = New PropertyCell(mse.FleetInputs(iflt), eVarNameFlags.MSEQIncrease)

            Next
        Catch ex As Exception
            Debug.Assert(False)
        End Try



    End Sub

End Class
