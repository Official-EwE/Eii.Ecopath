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




Imports EwECore.MSE
Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources



Namespace Ecosim

    ''' ===========================================================================
    ''' <summary>
    ''' 
    ''' </summary>
    ''' ===========================================================================
    
    Public Class gridQuotaShare
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim src As cCoreInputOutputBase = Nothing

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(1, 2 + Me.Core.nFleets + 1) ' Include sum column

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            For iFleet As Integer = 1 To Me.Core.nFleets
                src = Me.Core.EcopathFleetInputs(iFleet)
                Me(0, 1 + iFleet) = New cPropertyColumnHeaderCell(Me.PropertyManager,
                                                                 src, eVarNameFlags.Name, Nothing,
                                                                 cUnits.Currency)
            Next
            Me(0, 1 + Me.Core.nFleets + 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_SUM)

            Me.FixedColumns = 2
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub FillData()

            Dim fleet As cMSEFleetInput = Nothing
            Dim group As cCoreInputOutputBase = Nothing
            Dim prop As cProperty = Nothing
            Dim propSum As cFormulaProperty = Nothing
            Dim opSum As cMultiOperation = Nothing
            Dim alPropSum As New ArrayList()

            ' For each group
            For iGroup As Integer = 1 To Me.Core.nLivingGroups

                Me.AddRow()

                'Get the group info
                group = Me.Core.EcopathGroupInputs(iGroup)

                ' Fleet name As row header
                Me(iGroup, 0) = New cEwERowHeaderCell(CStr(iGroup))
                Me(iGroup, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, group, eVarNameFlags.Name)

                ' Clear row sum
                alPropSum.Clear()

                ' Fleet cells
                For iFleet As Integer = 1 To Me.Core.nFleets
                    fleet = Me.Core.MSEManager.EcopathFleetInputs(iFleet)
                    prop = Me.PropertyManager.GetProperty(fleet, eVarNameFlags.QuotaShare, group)
                    Me(iGroup, 1 + iFleet) = New cPropertyCell(prop)

                    ' Add to sum row
                    alPropSum.Add(prop)
                Next

                ' Now create the formula property that will calculate the sum of all cells in the row
                opSum = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alPropSum.ToArray())
                ' Create sum property
                propSum = Me.Formula(opSum)
                ' Define sum cell
                Me(iGroup, 1 + Me.Core.nFleets + 1) = New cPropertyCell(propSum)

            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecosim
            End Get
        End Property

    End Class

End Namespace ' Ecosim
