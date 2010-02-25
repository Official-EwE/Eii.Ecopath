#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Discard Fate user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class FisheryInputDiscardFateEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(Core.nFleets + 1, Core.nDetritusGroups + 4)

            ' Grid Cell (0, 0) - Fleet name
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)

            ' Dynamic column header - Detritus groups
            For columnIndex As Integer = 1 To Core.nDetritusGroups
                source = Core.EcoPathGroupInputs(Core.nGroups - Core.nDetritusGroups + columnIndex)
                Me(0, columnIndex + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            ' Export header cell
            Me(0, Me.ColumnsCount - 2) = New EwEColumnHeaderCell(My.Resources.GENERIC_HEADER_EXPORT)

            ' Sum header cell
            Me(0, Me.ColumnsCount - 1) = New EwEColumnHeaderCell(My.Resources.HEADER_SUM)

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim sum As New cSingleProperty("")
            sum.SetValue(1.0)
            sum.SetStyle(cStyleGuide.eStyleFlags.Sum Or cStyleGuide.eStyleFlags.NotEditable)

            Dim prop As cProperty = Nothing
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
                Me(rowIndex, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                For columnIndex As Integer = 2 To core.nDetritusGroups + 1
                    ' Get the ecopath input
                    sourceSec = Me.Core.EcoPathGroupInputs(columnIndex - 1)
                    ' Dynamic indexed Discard fate property 
                    prop = Me.PropertyManager.GetProperty(source, eVarNameFlags.DiscardFate, sourceSec)
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

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

