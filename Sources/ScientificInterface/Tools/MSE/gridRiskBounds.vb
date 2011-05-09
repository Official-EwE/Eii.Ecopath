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
Public Class gridRiskBounds
    : Inherits EwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 4)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUP)
        Me(0, 2) = New EwEColumnHeaderCell(SharedResources.HEADER_MSE_LOWERRISK)
        Me(0, 3) = New EwEColumnHeaderCell(SharedResources.HEADER_MSE_UPPERRISK)

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            For igrp As Integer = 1 To Me.Core.nLivingGroups

                Me.Rows.Insert(igrp)
                Me(igrp, 0) = New EwERowHeaderCell(CStr(igrp))
                Me(igrp, 1) = New PropertyRowHeaderCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.Name)
                Me(igrp, 2) = New PropertyCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.MSELowerRisk)
                Me(igrp, 3) = New PropertyCell(Me.PropertyManager, mse.GroupInputs(igrp), eVarNameFlags.MSEUpperRisk)

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
