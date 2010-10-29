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
    ''' Grid accepting Ecopath Landings user input.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
       Public Class FisheryInputLandingsEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.New()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            Dim source As cCoreInputOutputBase = Nothing

            MyBase.InitStyle()

            ' Test for UI context to prevent core from being accessed
            If (Me.UIContext Is Nothing) Then Return

            'Define grid dimensions
            Me.Redim(Core.nGroups + 2, Core.nFleets + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To Core.nFleets
                source = Me.Core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Next

            ' Total column
            Me(0, Core.nFleets + 2) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTAL)
            ' Sum row
            Me(Core.nGroups + 1, 1) = New EwERowHeaderCell(SharedResources.HEADER_SUM)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim aiStanzaGroups(Core.nGroups) As Integer 'Hold the stanza group index
            Dim iStanzaGroupPrev As Integer = -1

            Dim pm As cPropertyManager = Me.PropertyManager
            Dim prop As cProperty = Nothing

            Dim alSumRow As New ArrayList()
            Dim alSumAll As New ArrayList()
            Dim alSumCol As New ArrayList()

            Dim opSumAll As cMultiOperation = Nothing
            Dim opSumRow As cMultiOperation = Nothing
            Dim opSumCol As cMultiOperation = Nothing

            Dim propSumRow As cFormulaProperty = Nothing
            Dim propSumAll As cFormulaProperty = Nothing
            Dim propSumCol As cFormulaProperty = Nothing

            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Core.nGroups : aiStanzaGroups(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To Me.Core.nStanzas - 1
                sg = Me.Core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = Me.Core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    aiStanzaGroups(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If Core.nFleets = 0 Then Return

            'Create rows for all groups
            For rowIndex As Integer = 1 To Me.Core.nGroups

                ' Clear the arrayList for the new row
                alSumRow.Clear()
                ' Get the Ecopath input for this specific group
                source = Core.EcoPathGroupInputs(rowIndex)

                If aiStanzaGroups(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source, alSumRow, alSumAll)
                Else 'Group is stanza
                    sg = Core.StanzaGroups(aiStanzaGroups(source.Index))
                    If aiStanzaGroups(source.Index) <> iStanzaGroupPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To Core.nFleets + 2 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        iStanzaGroupPrev = aiStanzaGroups(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, source, alSumRow, alSumAll, True)
                End If

                ' Set the property to the last cell of the row, which is the sum of the row
                opSumRow = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumRow.ToArray())
                propSumRow = New cFormulaProperty(CType(opSumRow, cExpression))
                Me(iRow, Me.ColumnsCount - 1) = New PropertyCell(CType(propSumRow, cProperty))
            Next

            ' Sum row
            iRow = Me.AddRow()
            Me(iRow, 0) = New EwERowHeaderCell(iRow)
            Me(iRow, 1) = New EwERowHeaderCell(SharedResources.HEADER_SUM)
            For fleetIndex As Integer = 1 To Core.nFleets
                source = Core.FleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To Core.nGroups
                    sourceSec = Core.EcoPathGroupInputs(rowIndex)
                    prop = pm.GetProperty(source, eVarNameFlags.Landings, sourceSec)
                    alSumCol.Add(prop)
                Next
                opSumCol = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumCol.ToArray())
                propSumCol = New cFormulaProperty(CType(opSumCol, cExpression))
                ' Set the property to the last cell of the column, which is the sum of the column
                Me(Me.RowsCount - 1, fleetIndex + 1) = New PropertyCell(CType(propSumCol, cProperty))
            Next


            opSumAll = New cMultiOperation(cMultiOperation.eOperatorType.Sum, alSumAll.ToArray())
            propSumAll = New cFormulaProperty(CType(opSumAll, cExpression))
            ' Set the property to the bottom-right cell, which is the sum of all cells
            Me(Me.RowsCount - 1, Me.ColumnsCount - 1) = New PropertyCell(CType(propSumAll, cProperty))

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, _
            ByRef alSumRow As ArrayList, ByRef alSumAll As ArrayList, Optional ByVal isIndented As Boolean = False)

            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim pm As cPropertyManager = Me.PropertyManager
            Dim prop As cProperty = Nothing
            Dim propCell As PropertyCell = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To Core.nFleets
                ' Get the fleet object 
                sourceSec = Core.FleetInputs(fleetIndex)
                ' Get the index landing property
                prop = pm.GetProperty(sourceSec, eVarNameFlags.Landings, source)
                ' Set the property to the cell
                propCell = New PropertyCell(prop)
                ' Configure the cell
                propCell.SuppressZero = True
                ' Set the cell
                Me(iRow, fleetIndex + 1) = propCell
                ' Add the property to ArrayList; it is used for the sum
                alSumRow.Add(prop)
                alSumAll.Add(prop)
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace

