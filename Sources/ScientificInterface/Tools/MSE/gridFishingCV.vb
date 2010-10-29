#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.MSE
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells.Real

#End Region

<CLSCompliant(False)> _
Public Class gridFishingCV
    : Inherits EwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 3)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
        Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_INCREASEQ)
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

                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSEQIncrease)
                ' Me(i, 2) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSEFleetCV)

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
