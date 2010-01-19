
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region


<CLSCompliant(False)> _
Public Class gridFishingCV
    : Inherits EwEGrid

    Private m_core As cCore

    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 4)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_CV)
        Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_INCREASEQ)

        Me.FixedColumns = 2

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            For iFlt As Integer = 1 To m_core.nFleets

                Me.Rows.Insert(iFlt)

                Me(iFlt, 0) = New EwERowHeaderCell(iFlt)
                Me(iFlt, 1) = New PropertyRowHeaderCell(mse.FleetInputs(iFlt), eVarNameFlags.Name)
                Me(iFlt, 2) = New PropertyCell(mse.FleetInputs(iFlt), eVarNameFlags.MSEFleetCV)
                Me(iFlt, 3) = New PropertyCell(mse.FleetInputs(iFlt), eVarNameFlags.MSEQIncrease)

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
