#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports SourceGrid2.Cells.Real
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid accepting Ecopath Detritus Fate user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class DetritusFateEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            Dim source As cCoreInputOutputBase = Nothing

            'Define grid dimensions
            Me.Redim(Core.nGroups + 1, 4 + Core.nDetritusGroups)

            'Header cell (0,0) Source \ fate
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_SOURCEFATE)

            ' Detritus column header cells
            For i As Integer = 1 To Core.nDetritusGroups
                source = Core.EcoPathGroupInputs(Core.nGroups - Core.nDetritusGroups + i)
                Me(0, i + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            ' The export header cell
            Me(0, Core.nDetritusGroups + 2) = New EwEColumnHeaderCell(SharedResources.HEADER_EXPORT)
            ' The sum header cell
            Me(0, Core.nDetritusGroups + 3) = New EwEColumnHeaderCell(SharedResources.HEADER_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Dim prop As cProperty = Nothing
            Dim propSum As cSingleProperty = Nothing
            Dim propExport As cFormulaProperty = Nothing

            Dim alProp As New ArrayList()
            Dim propSumAll As cFormulaProperty = Nothing
            Dim opSumAll As cMultiOperation = Nothing
            Dim opMinus As cBinaryOperation = Nothing

            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim blnStanza(Core.nGroups) As Boolean
            Dim aiStanza(Core.nGroups) As Integer 'Hold the stanza group number
            Dim iStanzaPrev As Integer = -1

            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Core.nGroups : aiStanza(i) = -1 : Next

            'Remove existing rows
            Me.RowsCount = 1

            'Tag stanza group first
            For iStanzaGroup As Integer = 0 To Core.nStanzas - 1
                sg = Core.StanzaGroups(iStanzaGroup)
                For iStanza As Integer = 1 To sg.NStanzas
                    source = Core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    blnStanza(source.Index) = True
                    aiStanza(source.Index) = iStanzaGroup
                Next
            Next

            ' Configure static SUM prop
            propSum = New cSingleProperty()
            propSum.SetValue(1.0)
            propSum.SetStyle(cStyleGuide.eStyleFlags.Sum Or cStyleGuide.eStyleFlags.NotEditable)

            ' Create rows for all groups
            For rowIndex As Integer = 1 To Core.nGroups

                source = Core.EcoPathGroupInputs(rowIndex)
                alProp.Clear()

                If aiStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    For iCol As Integer = 1 To Core.nDetritusGroups

                        sourceSec = Core.EcoPathGroupInputs(Core.nGroups - Core.nDetritusGroups + iCol)

                        Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                        Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                        prop = Me.PropertyManager.GetProperty(source, eVarNameFlags.DetritusFate, sourceSec, True, Core.nGroups - Core.nDetritusGroups)
                        Me(iRow, iCol + 1) = New PropertyCell(prop)
                        alProp.Add(prop)
                    Next

                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alProp.ToArray)
                    propSumAll = Me.Formula(opSumAll)
                    opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Subtract, propSum, propSumAll)
                    propExport = Me.Formula(opMinus)

                    ' Export column 
                    Me(iRow, Me.ColumnsCount - 2) = New PropertyCell(propExport)

                    ' JS 140606: Use static single property here. Seems overkill where a simple Cell(1.0) would have
                    '            been sufficient, but this way the cell inherits StyleGuide colour and decimals feedback.
                    Me(iRow, Me.ColumnsCount - 1) = New PropertyCell(propSum)

                Else ' Group is stanza

                    sg = Core.StanzaGroups(aiStanza(source.Index))
                    If aiStanza(source.Index) <> iStanzaPrev Then 'If stanza group appears the first time Then display + control
                        iRow = Me.AddRow()
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)
                        'Complete row with dummy cells
                        For i As Integer = 2 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        iStanzaPrev = aiStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow()
                    hgcStanza.AddChildRow(iRow)
                    For iCol As Integer = 1 To Core.nDetritusGroups

                        sourceSec = Core.EcoPathGroupInputs(Core.nGroups - Core.nDetritusGroups + iCol)

                        Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                        Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
                        prop = Me.PropertyManager.GetProperty(source, eVarNameFlags.DetritusFate, sourceSec, True, Core.nGroups - Core.nDetritusGroups)
                        Me(iRow, iCol + 1) = New PropertyCell(prop)
                        alProp.Add(prop)
                    Next

                    opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alProp.ToArray)
                    propSumAll = Me.Formula(opSumAll)
                    opMinus = New cBinaryOperation(cBinaryOperation.eOperatorType.Subtract, propSum, propSumAll)
                    propExport = Me.Formula(opMinus)

                    ' Export column 
                    Me(iRow, Me.ColumnsCount - 2) = New PropertyCell(propExport)
                    Me(iRow, Me.ColumnsCount - 1) = New PropertyCell(propSum)
                End If
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
