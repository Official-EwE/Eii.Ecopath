#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports EwECore.MSE
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

<CLSCompliant(False)> _
Public Class gridFishingWeights
    : Inherits EwEGrid

    Public Sub New()
    End Sub

    Protected Overrides Sub InitStyle()
        MyBase.InitStyle()

        ' Test for UI context to prevent core from being accessed
        If (Me.UIContext Is Nothing) Then Return

        Dim src As cCoreInputOutputBase = Nothing

        Me.Redim(1, 2 + Me.Core.nFleets)

        Me(0, 0) = New EwEColumnHeaderCell("")
        Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

        For iFleet As Integer = 1 To Me.Core.nFleets
            src = Me.Core.FleetInputs(iFleet)
            Me(0, 1 + iFleet) = New PropertyColumnHeaderCell(Me.PropertyManager, _
                                                             src, eVarNameFlags.Name, Nothing, _
                                                             SharedResources.GENERIC_LABEL_DETAILED, cStyleGuide.eUnitType.Currency)
        Next

        Me.FixedColumns = 2
        Me.FixedColumnWidths = False

    End Sub

    Protected Overrides Sub FillData()

        Try

            Dim mse As cMSEManager = Me.Core.MSEManager
            If mse Is Nothing Then Exit Sub

            Dim group As cCoreInputOutputBase = Nothing
            Dim fleet As cMSEFleetInput = Nothing
            ' Dim cell As ICell = Nothing

            ' For each group
            For iGroup As Integer = 1 To Me.Core.nGroups

                Me.AddRow()

                'Get the group info
                group = Core.EcoPathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New EwERowHeaderCell(iGroup)
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To Me.Core.nFleets
                    fleet = mse.FleetInputs(iFleet)
                    Me(iGroup, 1 + iFleet) = New PropertyCell(Me.PropertyManager, fleet, eVarNameFlags.MSEFleetWeight, group)
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
