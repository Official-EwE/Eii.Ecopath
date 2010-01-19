
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
        MyBase.New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Me.Redim(1, 2)
        Me(0, 0) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_CV)
'
        Me.FixedColumns = 1

    End Sub

    Protected Overrides Sub FillData()
        Try

            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            For igrp As Integer = 1 To m_core.nLivingGroups

                Me.AddRow()

                Me(igrp, 0) = New PropertyRowHeaderCell(mse.GroupInputs(igrp), eVarNameFlags.Name)
                Me(igrp, 1) = New PropertyCell(mse.GroupInputs(igrp), eVarNameFlags.MSEBioCV)
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
