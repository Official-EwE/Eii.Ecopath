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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Off-vessel price user input.
    ''' </summary>
    ''' =======================================================================
    
    Public Class gridEcosimCatchability
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(1, Core.nFleets + 1 + 1)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet names
            For fleetIndex As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcosimFleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New cPropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim grp As cEcosimGroupInput = Nothing
            Dim flt As cEcosimFleetInput = Nothing

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For iGroup As Integer = 1 To Me.Core.nGroups

                Dim iRow As Integer = Me.AddRow()
                grp = Me.Core.EcosimGroupInputs(iGroup)

                Me(iRow, 0) = New cEwECell(CStr(iGroup))
                Me(iRow, 1) = New cPropertyCell(Me.PropertyManager, grp, eVarNameFlags.Name)

                For iFleet As Integer = 1 To Me.Core.nFleets
                    flt = Me.Core.EcosimFleetInputs(iFleet)
                    Me(iRow, iFleet + 1) = New cPropertyCell(Me.PropertyManager, flt, eVarNameFlags.EcosimRelQ, grp)
                Next
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecosim
            End Get
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property
    End Class

End Namespace

