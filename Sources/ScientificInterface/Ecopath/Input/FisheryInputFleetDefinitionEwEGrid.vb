#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath fleet definitions input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
       Public Class FisheryInputFleetDefinitionEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 7)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_FIXEDCOST)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_EFFORTRELATEDCOST)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_SAILINGRELATEDCOST)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_PROFIT_PERC)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALVALUE_PERC)

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

            Dim propTotal As New cSingleProperty("")
            propTotal.SetValue(100.0)
            propTotal.SetStyle(cStyleGuide.eStyleFlags.NotEditable Or cStyleGuide.eStyleFlags.Sum)

            For rowIndex As Integer = 1 To Core.nFleets

                Me.Rows.Insert(rowIndex)
                ' Clear the arrayList for the new row
                alSumAll.Clear()

                source = Core.FleetInputs(rowIndex)
                Me(rowIndex, 0) = New EwERowHeaderCell(rowIndex)
                ' Fleet name column
                Me(rowIndex, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)

                'Fixed cost column
                prop = pm.GetProperty(source, eVarNameFlags.FixedCost)
                Me(rowIndex, 2) = New PropertyCell(prop)
                alSumAll.Add(prop)

                'Effort related cost
                prop = pm.GetProperty(source, eVarNameFlags.CPUECost)
                Me(rowIndex, 3) = New PropertyCell(prop)
                alSumAll.Add(prop)

                'Sailing related cost
                prop = pm.GetProperty(source, eVarNameFlags.SailCost)
                Me(rowIndex, 4) = New PropertyCell(prop)
                alSumAll.Add(prop)

                ' Get the dynamic profit cell by using MultiOperation and binaryOperation
                opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
                propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
                opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, _
                                                CType(propTotal, Object), CType(propSumAll, Object))
                propProfit = New cFormulaProperty(CType(opMinus, cExpression))

                Me(rowIndex, 5) = New PropertyCell(CType(propProfit, cProperty))

                ' Set the constant total 100.0
                Me(rowIndex, 6) = New PropertyCell(propTotal)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

