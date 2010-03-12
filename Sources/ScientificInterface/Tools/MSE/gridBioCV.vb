#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region

<CLSCompliant(False)> _
Public Class gridBioCV
    : Inherits EwEGrid

    Public Sub New()
        MyBase.New()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 3)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_CV)
'
        Me.FixedColumns = 1

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For i As Integer = 1 To Me.Core.nLivingGroups

                Me.AddRow()

                Me(i, 0) = New EwERowHeaderCell(i)
                Me(i, 1) = New PropertyRowHeaderCell(Me.PropertyManager, mse.GroupInputs(i), eVarNameFlags.Name)
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
