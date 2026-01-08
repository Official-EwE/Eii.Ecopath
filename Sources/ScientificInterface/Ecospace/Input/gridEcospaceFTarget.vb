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

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwECore.Common
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2
Imports SourceGrid2.BehaviorModels

#End Region

Namespace Ecospace

    ''' =======================================================================
    ''' <summary>
    ''' Grid control, implements the Ecospace interface to set dispersal rates.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)>
    Public Class gridEcospaceFTarget
        Inherits cEwEGrid

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            FTarget
        End Enum

#Region " Construction / destruction "

        Public Sub New()
            MyBase.New()
        End Sub

#End Region ' Construction / destruction

        Protected Overrides Sub InitStyle()
            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            'Add column headers
            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.FTarget) = New cEwEColumnHeaderCell(eVarNameFlags.EcospaceFTarget)

            Me.FixedColumnWidths = False

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cEcospaceGroupInput = Nothing
            Dim cell As cEwECellBase = Nothing

            For iGroup As Integer = 1 To Me.Core.nGroups
                Me.Rows.Insert(iGroup)

                source = Me.Core.EcospaceGroupInputs(iGroup)
                Me(iGroup, eColumnTypes.Index) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                Me(iGroup, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                cell = New cPropertyCell(Me.PropertyManager, source, eVarNameFlags.EcospaceFTarget)
                cell.SuppressZero = False
                Me(iGroup, eColumnTypes.FTarget) = cell
            Next

        End Sub

        Public Overrides ReadOnly Property CoreComponents() As eCoreComponentType()
            Get
                ' Refresh on Ecopath notifications
                Return New eCoreComponentType() {eCoreComponentType.Ecopath, eCoreComponentType.Ecospace}
            End Get
        End Property

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                '??? What why...
                Return False
            End Get
        End Property
    End Class

End Namespace
