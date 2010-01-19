
#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE

#End Region


<CLSCompliant(False)> _
Public Class gridFishingWeights
    : Inherits EwEGrid

    Private m_core As cCore


    Public Sub New()

        Me.m_core = cCore.GetInstance

    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        Dim core As cCore = cCore.GetInstance()
        Dim src As cCoreInputOutputBase = Nothing

        Me.Redim(1, 2 + core.nFleets)

        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

        For iFleet As Integer = 1 To core.nFleets
            src = core.FleetInputs(iFleet)
            Me(0, 1 + iFleet) = New PropertyColumnHeaderCell(src, _
                eVarNameFlags.Name, Nothing, _
                "{0} ({1})", cStyleGuide.eUnitType.Currency)
        Next

        Me.FixedColumns = 2
        Me.FixedColumnWidths = True

    End Sub

    Protected Overrides Sub FillData()

        Try

            Dim core As cCore = cCore.GetInstance()
            Dim mse As cMSEManager = Me.m_core.MSEManager
            If mse Is Nothing Then Exit Sub

            Dim group As cCoreInputOutputBase = Nothing
            Dim fleet As cMSEFleetInput = Nothing
            ' Dim cell As ICell = Nothing

            ' For each group
            For iGroup As Integer = 1 To core.nGroups

                Me.AddRow()

                'Get the group info
                group = core.EcoPathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New EwERowHeaderCell(iGroup)
                Me(iGroup, 1) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To core.nFleets
                    fleet = mse.FleetInputs(iFleet)
                    Me(iGroup, 1 + iFleet) = New PropertyCell(fleet, eVarNameFlags.MSEFleetWeight, group)
                Next
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
