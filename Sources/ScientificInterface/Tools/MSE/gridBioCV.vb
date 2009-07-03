
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

    Private m_core As cCore

    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()
        Me.Redim(1, 2)
        Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUP)
        Me(0, 1) = New EwEColumnHeaderCell("C.V.")

    End Sub

    Protected Overrides Sub FillData()
        Dim mse As cMSEManager = Me.m_core.MSEManager
        If mse Is Nothing Then Exit Sub

        For igrp As Integer = 1 To m_core.nLivingGroups

            Me.Rows.Insert(igrp)

            Me(igrp, 0) = New PropertyRowHeaderCell(mse.GroupInputs(igrp), eVarNameFlags.Name)
            Me(igrp, 1) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSEBioCV)

        Next

    End Sub

End Class
