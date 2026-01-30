' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    Public Class gridFleetFishingMortality
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = True
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim group As cCoreGroupBase = Nothing
            Dim fleet As cEcopathFleetInput = Nothing
            Dim iGroup As Integer = 0

            Me.Redim(Me.Core.nLivingGroups + 1, 2 + Me.Core.nFleets)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEET_GROUP)

            For iFleet As Integer = 1 To Me.Core.nFleets
                fleet = Me.Core.EcopathFleetInputs(iFleet)
                Me(0, 1 + iFleet) = New cPropertyColumnHeaderCell(Me.PropertyManager,
                                                                 fleet, eVarNameFlags.Name, Nothing,
                                                                 cUnits.OverTime)
            Next iFleet

            For iGroup = 1 To Me.Core.nLivingGroups
                group = Me.Core.EcopathGroupOutputs(iGroup)
                Me(iGroup, 0) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)
            Next iGroup

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cEcopathGroupOutput = Nothing
            Dim fleet As cEcopathFleetInput = Nothing
            Dim cell As cEwECell = Nothing
            Dim sLandings As Single = 0.0!
            Dim sDiscards As Single = 0.0!
            Dim sBiomass As Single = 0.0!
            Dim style As cStyleGuide.eStyleFlags = (cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.ValueComputed)

            For iFleet As Integer = 1 To Me.Core.nFleets
                ' Get fleet
                fleet = Me.Core.EcopathFleetInputs(iFleet)
                For iGroup As Integer = 1 To Me.Core.nLivingGroups
                    group = Me.Core.EcopathGroupOutputs(iGroup)
                    ' Get values 
                    sLandings = fleet.Landings(iGroup)
                    'Only discards the suffer mortality
                    sDiscards = fleet.Discards(iGroup) * fleet.DiscardMortality(iGroup)
                    sBiomass = group.Biomass()

                    ' Create cell
                    If sBiomass > 0 Then
                        cell = New cEwECell((sLandings + sDiscards) / sBiomass, GetType(Single), style)
                    Else
                        cell = New cEwECell(0.0!, GetType(Single), style Or cStyleGuide.eStyleFlags.Null)
                    End If

                    ' Value cells suppress zeroes to increase legibility of the grid
                    cell.SuppressZero(0) = True

                    ' Activate the cell
                    Me(iGroup, 1 + iFleet) = cell
                    ' Next
                Next
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property
    End Class

End Namespace
