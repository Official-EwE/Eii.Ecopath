
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region


<CLSCompliant(False)> _
Public Class gridRiskBounds
    : Inherits EwEGrid

    Private m_core As cCore

    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 4)
        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
        Me(0, 2) = New EwEColumnHeaderCell("Min. biomass/org. biomass")
        Me(0, 3) = New EwEColumnHeaderCell("Max. biomass/org. biomass")

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            For igrp As Integer = 1 To m_core.nLivingGroups

                Me.Rows.Insert(igrp)
                Me(igrp, 0) = New EwERowHeaderCell(igrp)
                Me(igrp, 1) = New PropertyRowHeaderCell(mse.GroupInputs(igrp), eVarNameFlags.Name)
                Me(igrp, 2) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSELowerRisk)
                Me(igrp, 3) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSEUpperRisk)

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
