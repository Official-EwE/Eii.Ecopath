'==============================================================================
'
' $Log: FisheryInputFleetDefinitionEwEGrid.vb,v $
' Revision 1.3  2009/01/16 18:30:10  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:53:39  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.19  2008/08/02 03:04:12  jeroens
' Renamed resources
'
' Revision 1.18  2008/06/02 00:01:29  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.17  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.16  2008/04/07 02:31:09  jeroens
' Cleaning up resources
'
' Revision 1.15  2008/02/13 16:44:29  jeroens
' Renamed resources
'
' Revision 1.14  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.13  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.12  2007/06/05 02:45:50  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.11  2007/05/31 13:11:21  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.10  2007/04/29 03:45:11  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.9  2006/08/23 00:15:36  shermanl
' Update class name based on core changes.......
'
' Revision 1.8  2006/08/15 15:40:29  jeroens
' * Fixed spelling error
'
' Revision 1.7  2006/07/12 16:10:11  jeroens
' - Reverted silly enum bit, sorry guys!
'
' Revision 1.6  2006/07/10 18:45:17  jeroens
' + Added sec. indexes on Cost vars
'
' Revision 1.5  2006/07/07 02:10:40  jeroens
' + Added cFleetOutput
' * Renamed cFleet to cFleetInput
'
' Revision 1.4  2006/06/20 22:55:47  fgao
' Grids update
'
' Revision 1.3  2006/06/08 23:35:12  fgao
' Added more grid..
'
' Revision 1.2  2006/06/07 03:40:56  jeroens
' + Updated to cCoreInputOutput / ICoreInterface changes
'
' Revision 1.1  2006/05/31 22:05:00  cvsuser
' Updating to dockable panels.
'
' Revision 1.4  2006/05/20 01:41:02  jeroens
' * Fixed row header in FillData
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

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

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()

            Dim alSumAll As New ArrayList()
            Dim opSumAll As cMultiOperation = Nothing
            Dim opMinus As cBinaryOperation = Nothing
            Dim propProfit As cFormulaProperty = Nothing
            Dim propSumAll As cFormulaProperty = Nothing

            Dim propTotal As New cSingleProperty("")
            propTotal.SetValue(100.0)
            propTotal.SetStyle(StyleGuide.eStyleFlags.NotEditable Or StyleGuide.eStyleFlags.Sum)

            For rowIndex As Integer = 1 To core.nFleets

                Me.Rows.Insert(rowIndex)
                ' Clear the arrayList for the new row
                alSumAll.Clear()

                source = core.FleetInputs(rowIndex)
                Me(rowIndex, 0) = New EwERowHeaderCell(rowIndex)
                ' Fleet name column
                Me(rowIndex, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)

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

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

