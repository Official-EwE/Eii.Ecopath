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
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath fleet definitions input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
       Public Class FisheryInputFleetDefinitionEwEGrid
        : Inherits cEwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 7)

            Me(0, 0) = New cEwEColumnHeaderCell("")
            Me(0, 1) = New cEwEColumnHeaderCell(SharedResources.HEADER_FLEETNAME)
            Me(0, 2) = New cEwEColumnHeaderCell(SharedResources.HEADER_FIXEDCOST)
            Me(0, 3) = New cEwEColumnHeaderCell(SharedResources.HEADER_EFFORTRELATEDCOST)
            Me(0, 4) = New cEwEColumnHeaderCell(SharedResources.HEADER_SAILINGRELATEDCOST)
            Me(0, 5) = New cEwEColumnHeaderCell(SharedResources.HEADER_PROFIT_PERC)
            Me(0, 6) = New cEwEColumnHeaderCell(SharedResources.HEADER_TOTALVALUE_PERC)

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
                Me(iRow, 0) = New cEwERowHeaderCell(CStr(iRow))
                ' Fleet name column
                Me(iRow, 1) = New cPropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                'Fixed cost column
                prop = pm.GetProperty(source, eVarNameFlags.FixedCost)
                Me(iRow, 2) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                'Effort related cost
                prop = pm.GetProperty(source, eVarNameFlags.CPUECost)
                Me(iRow, 3) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                'Sailing related cost
                prop = pm.GetProperty(source, eVarNameFlags.SailCost)
                Me(iRow, 4) = New cPropertyCell(prop)
                alSumAll.Add(prop)

                ' Get the dynamic profit cell by using MultiOperation and binaryOperation
                opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
                propSumAll = Me.Formula(opSumAll)
                opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Subtract, propTotal, propSumAll)
                propProfit = Me.Formula(opMinus)

                Me(iRow, 5) = New cPropertyCell(propProfit)

                ' Set the constant total 100.0
                Me(iRow, 6) = New cPropertyCell(propTotal)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

