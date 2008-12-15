'==============================================================================
'
' $Log: FisheryInputDiscardFateEwEGrid.vb,v $
' Revision 1.2  2008/12/15 15:53:39  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.20  2008/07/29 13:06:44  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.19  2008/06/02 00:01:28  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.18  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.17  2008/05/13 18:48:06  jeroens
' Fixed bug 466
'
' Revision 1.16  2008/04/07 02:31:08  jeroens
' Cleaning up resources
'
' Revision 1.15  2008/02/13 16:44:29  jeroens
' Renamed resources
'
' Revision 1.14  2008/01/31 17:08:14  jeroens
' Made fleet column headers live updating
'
' Revision 1.13  2007/10/10 02:59:13  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.12  2007/07/03 07:08:47  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.11  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.10  2007/06/05 02:45:50  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.9  2007/05/31 13:11:21  jeroens
' * Renamed StyleGuide StyleFlags to eStyleFlags
'
' Revision 1.8  2007/04/29 03:45:11  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class FisheryInputDiscardFateEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(core.nFleets + 1, core.nDetritusGroups + 4)

            ' Grid Cell (0, 0) - Fleet name
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)

            ' Dynamic column header - Detritus groups
            For columnIndex As Integer = 1 To core.nDetritusGroups
                source = core.EcoPathGroupInputs(core.nGroups - core.nDetritusGroups + columnIndex)
                Me(0, columnIndex + 1) = New PropertyColumnHeaderCell(source, eVarNameFlags.Name)
            Next

            ' Export header cell
            Me(0, Me.ColumnsCount - 2) = New EwEColumnHeaderCell(My.Resources.GENERIC_HEADER_EXPORT)

            ' Sum header cell
            Me(0, Me.ColumnsCount - 1) = New EwEColumnHeaderCell(My.Resources.HEADER_SUM)

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim sum As New cSingleProperty("")
            sum.SetValue(1.0)
            sum.SetStyle(StyleGuide.eStyleFlags.Sum Or StyleGuide.eStyleFlags.NotEditable)

            Dim prop As cProperty = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim alSumAll As New ArrayList()

            Dim opSumAll As cMultiOperation = Nothing
            Dim propSumAll As cFormulaProperty = Nothing
            Dim opMinus As cBinaryOperation = Nothing
            Dim propExport As cFormulaProperty = Nothing

            ' For each fleet
            For rowIndex As Integer = 1 To core.nFleets
                'Get the fleet info
                source = core.FleetInputs(rowIndex)
                ' Clear the arrayList for the sum of new row
                alSumAll.Clear()
                ' Fleet name As row header
                Me(rowIndex, 0) = New EwERowHeaderCell(rowIndex)
                Me(rowIndex, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                For columnIndex As Integer = 2 To core.nDetritusGroups + 1
                    ' Get the ecopath input
                    sourceSec = core.EcoPathGroupInputs(columnIndex - 1)
                    ' Dynamic indexed Discard fate property 
                    prop = pm.GetProperty(source, eVarNameFlags.DiscardFate, sourceSec)
                    ' Add prop to the arraylist
                    alSumAll.Add(prop)
                    'assigned it to destined cell
                    Me(rowIndex, columnIndex) = New PropertyCell(prop)
                Next

                ' Get the sum of discard fate of all detritus groups
                opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray)
                propSumAll = New cFormulaProperty(DirectCast(opSumAll, cExpression))

                ' Calculate the export
                opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Substract, _
                                CObj(sum), CObj(propSumAll))

                ' Get the export property
                propExport = New cFormulaProperty(DirectCast(opMinus, cExpression))

                Me(rowIndex, Me.ColumnsCount - 2) = New PropertyCell(propExport)
                ' The property cell for the sum column, which is not editable and equal to 1
                Me(rowIndex, Me.ColumnsCount - 1) = New PropertyCell(sum)
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace

