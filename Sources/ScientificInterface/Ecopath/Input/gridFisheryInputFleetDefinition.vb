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

Imports EwECore.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath fleet definitions input.
    ''' </summary>
    ''' =======================================================================
    
    Public Class FisheryInputFleetDefinitionEwEGrid
        Inherits cEwEGrid

        Public Sub New()
            MyBase.New()
        End Sub

        Private Enum eColumnTypes As Integer
            Index = 0
            Name
            'NominalEffort
            FixedCost
            EffCost
            SailCost
            Profit
            TotalVal
        End Enum

        Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
            Get
                Return False
            End Get
        End Property

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New cEwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(eVarNameFlags.Name)
            'Me(0, eColumnTypes.NominalEffort) = New cEwEColumnHeaderCell(eVarNameFlags.NominalEffort)
            Me(0, eColumnTypes.FixedCost) = New cEwEColumnHeaderCell(eVarNameFlags.FixedCost)
            Me(0, eColumnTypes.EffCost) = New cEwEColumnHeaderCell(eVarNameFlags.EffortCost)
            Me(0, eColumnTypes.SailCost) = New cEwEColumnHeaderCell(eVarNameFlags.SailCost)
            Me(0, eColumnTypes.Profit) = New cEwEColumnHeaderCell(SharedResources.HEADER_PROFIT, cUnits.Percentage)
            Me(0, eColumnTypes.TotalVal) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTALVALUE, cUnits.Percentage)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing

            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = Me.PropertyManager

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim opMinus As cBinaryOperation = Nothing
            Dim propProfit As cFormulaProperty = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            Dim propTotal As New cSingleProperty()
            propTotal.SetValue(100.0)
            propTotal.SetStyle(cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Sum)

            For iRow As Integer = 1 To Me.Core.nFleets

                Me.Rows.Insert(iRow)
                ' Clear the arrayList for the new row
                alSumAll.Clear()

                source = Me.Core.EcopathFleetInputs(iRow)
                Me(iRow, eColumnTypes.Index) = New cEwERowHeaderCell(CStr(iRow))

                ' Fleet name column
                Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                '' Fleet nominal effort column
                'prop = pm.GetProperty(source, eVarNameFlags.NominalEffort)
                'Me(iRow, eColumnTypes.NominalEffort) = New cPropertyCell(prop)

                'Fixed cost column
                prop = pm.GetProperty(source, eVarNameFlags.FixedCost)
                Me(iRow, eColumnTypes.FixedCost) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                'Effort related cost
                prop = pm.GetProperty(source, eVarNameFlags.EffortCost)
                Me(iRow, eColumnTypes.EffCost) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                'Sailing related cost
                prop = pm.GetProperty(source, eVarNameFlags.SailCost)
                Me(iRow, eColumnTypes.SailCost) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                ' Get the dynamic profit cell by using MultiOperation and binaryOperation
                opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
                propSumAll = Me.Formula(opSumAll)
                opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Subtract, propTotal, propSumAll)
                propProfit = Me.Formula(opMinus)

                Me(iRow, eColumnTypes.Profit) = New cPropertyCell(propProfit)

                ' Set the constant total 100.0
                Me(iRow, eColumnTypes.TotalVal) = New cPropertyCell(propTotal)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.Ecopath
            End Get
        End Property

    End Class

End Namespace

