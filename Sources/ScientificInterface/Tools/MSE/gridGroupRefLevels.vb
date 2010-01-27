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

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()

        ' ToDo: localize this method

        MyBase.InitStyle()
        Me.Redim(1, 6)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 2) = New EwEColumnHeaderCell("Biomass Lower")
        Me(0, 3) = New EwEColumnHeaderCell("Biomass Upper")
        Me(0, 4) = New EwEColumnHeaderCell("Catch Lower")
        Me(0, 5) = New EwEColumnHeaderCell("Catch Upper")

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.UIContext.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.UIContext.Core.nLivingGroups

                Me.Rows.Insert(i)
                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New PropertyRowHeaderCell(mse.GroupInputs(i), eVarNameFlags.Name)
                Me(i, 2) = New PropertyCell(mse.GroupInputs(i), eVarNameFlags.MSERefBioLower)
                Me(i, 3) = New PropertyCell(mse.GroupInputs(i), eVarNameFlags.MSERefBioUpper)
                Me(i, 4) = New PropertyCell(mse.GroupInputs(i), eVarNameFlags.MSERefGroupCatchLower)
                Me(i, 5) = New PropertyCell(mse.GroupInputs(i), eVarNameFlags.MSERefGroupCatchUpper)

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
