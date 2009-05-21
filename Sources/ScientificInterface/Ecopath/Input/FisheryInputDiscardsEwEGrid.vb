'==============================================================================
'
' $Log: FisheryInputDiscardsEwEGrid.vb,v $
' Revision 1.4  2009/05/21 19:27:16  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:10  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:53:39  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:31  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/07/29 13:06:45  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.21  2008/06/02 00:01:28  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.20  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.19  2008/04/07 02:31:08  jeroens
' Cleaning up resources
'
' Revision 1.18  2008/01/31 17:08:14  jeroens
' Made fleet column headers live updating
'
' Revision 1.17  2008/01/11 12:33:19  jeroens
' Fixed bug 299
'
' Revision 1.16  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.15  2007/07/06 20:11:18  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.14  2007/07/03 07:08:47  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.13  2007/06/22 17:33:34  fgao
' Fixed a bug: Indent the multistanza group display.
'
' Revision 1.12  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.11  2007/06/15 16:35:49  jeroens
' * Capable of handling a model without fleets
'
' Revision 1.10  2007/06/05 02:45:50  jeroens
' * Renamed cMultiOperation Add ->Sum
'
' Revision 1.9  2007/04/29 03:45:11  jeroens
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
    Public Class FisheryInputDiscardsEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = False
        End Sub

        Protected Overrides Sub InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            MyBase.InitStyle()

            'Define grid dimensions
            Me.Redim(1, core.nFleets + 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet name
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(source, eVarNameFlags.Name)
            Next

            ' Total column
            Me(0, core.nFleets + 2) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTAL)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1

            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
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

            For i As Integer = 1 To core.nGroups : intStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(source.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            ' Done?
            If core.nFleets = 0 Then Return

            'Create rows for all groups
            For rowIndex As Integer = 1 To core.nGroups

                ' Clear the arrayList for the new row
                alSumRow.Clear()
                ' Get the Ecopath input for this specific group
                source = core.EcoPathGroupInputs(rowIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source, alSumRow, alSumAll)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To core.nFleets + 2 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
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
            Me(iRow, 1) = New EwERowHeaderCell(My.Resources.HEADER_SUM)
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                alSumCol.Clear()

                For rowIndex As Integer = 1 To core.nGroups
                    sourceSec = core.EcoPathGroupInputs(rowIndex)
                    prop = pm.GetProperty(source, eVarNameFlags.Discards, sourceSec)
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
            Dim core As cCore = cCore.GetInstance()
            Dim sourceSec As cCoreInputOutputBase = Nothing
            Dim pm As cPropertyManager = cPropertyManager.GetInstance()
            Dim prop As cProperty = Nothing
            Dim propCell As PropertyCell = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If
            ' For each fleet (each column) 
            For fleetIndex As Integer = 1 To core.nFleets
                ' Get the fleet object 
                sourceSec = core.FleetInputs(fleetIndex)
                ' Get the index landing property
                prop = pm.GetProperty(sourceSec, eVarNameFlags.Discards, source)

                propCell = New PropertyCell(prop)
                propCell.SuppressZero = True
                ' Set the property to the cell
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

