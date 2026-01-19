' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecosim

    
    Public Class gridEcosimFleetSizeDynamics
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            ' Redim the grid dimension
            Me.Redim(1, 6)

            ' Define column header
            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
            Me(0, 2) = New cEwEColumnHeaderCell(eVarNameFlags.EPower)
            Me(0, 3) = New cEwEColumnHeaderCell(eVarNameFlags.PcapBase)
            Me(0, 4) = New cEwEColumnHeaderCell(eVarNameFlags.CapDepreciate)
            Me(0, 5) = New cEwEColumnHeaderCell(eVarNameFlags.CapBaseGrowth)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing

            For iRow As Integer = 1 To Me.Core.nFleets
                source = Me.Core.EcosimFleetInputs(iRow)
                Me.Rows.Insert(iRow)
                Me(iRow, 0) = New cEwERowHeaderCell(CStr(iRow))
                Me(iRow, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                Me(iRow, 2) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.EPower)
                Me(iRow, 3) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.PcapBase)
                Me(iRow, 4) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.CapDepreciate)
                Me(iRow, 5) = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.CapBaseGrowth)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecosim
            End Get
        End Property

    End Class

End Namespace

