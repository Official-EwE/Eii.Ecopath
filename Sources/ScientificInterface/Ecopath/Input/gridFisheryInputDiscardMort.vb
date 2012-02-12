' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2.Cells

#End Region ' Imports

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Discard Mortality user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridFisheryInputDiscardMort
        Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim src As cCoreInputOutputBase = Nothing

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(1, 2 + Core.nFleets)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            For iFleet As Integer = 1 To Me.Core.nFleets
                src = Core.FleetInputs(iFleet)
                Me(0, 1 + iFleet) = New PropertyColumnHeaderCell(Me.PropertyManager, src, eVarNameFlags.Name)
            Next

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cCoreInputOutputBase = Nothing
            Dim fleet As cFleetInput = Nothing
            Dim cell As ICell = Nothing

            ' For each group
            For iGroup As Integer = 1 To core.nGroups

                Me.AddRow()

                'Get the group info
                group = core.EcoPathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New EwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 1) = New PropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                ' Fleet cells
                For iFleet As Integer = 1 To Me.Core.nFleets
                    fleet = Core.FleetInputs(iFleet)
                    Me(iGroup, 1 + iFleet) = New PropertyCell(Me.PropertyManager, fleet, eVarNameFlags.DiscardMortality, group)
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
