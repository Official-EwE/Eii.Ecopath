#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region

<CLSCompliant(False)> _
Public Class gridFleetRefLevels
    : Inherits EwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Redim(1, 6)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCH_LOWER)
        Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_CATCH_UPPER)
        Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFORT_LOWER)
        Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFORT_UPPER)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.UIContext.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.UIContext.Core.nFleets

                Me.Rows.Insert(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSERefFleetCatchLower)
                Me(i, 3) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSERefFleetCatchUpper)
                Me(i, 4) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSERefFleetEffortLower)
                Me(i, 5) = New PropertyCell(Me.PropertyManager, mse.FleetInputs(i), eVarNameFlags.MSERefFleetEffortUpper)

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
