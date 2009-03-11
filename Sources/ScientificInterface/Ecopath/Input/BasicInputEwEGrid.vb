'==============================================================================
'
' $Log: BasicInputEwEGrid.vb,v $
' Revision 1.4  2009/03/11 19:32:11  jeroens
' Switched Z, P/B columns
'
' Revision 1.3  2009/03/09 15:05:07  jeroens
' Split P/B col into P/B (non stanza), Z (stanza)
'
' Revision 1.2  2009/01/16 18:30:09  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:31:30  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports SourceGrid2
Imports EwEUtils.Core

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class BasicInputEwEGrid
        : Inherits EwEGrid

#Region " Private variables "

        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)

#End Region ' Private variables

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, 11)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_AREA)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSAREA_UNIT, StyleGuide.eUnitType.Currency)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_GE)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_UNASSIMILCONSUMPTION)
            Me(0, 10) = New EwEColumnHeaderCell(My.Resources.HEADER_DETIMP_UNIT, New StyleGuide.eUnitType() {StyleGuide.eUnitType.Currency, StyleGuide.eUnitType.Time})

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim cell As EwECellBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim bInStanza(core.nGroups) As Boolean
            Dim intStanza(core.nGroups) As Integer 'Hold the stanza group number
            Dim intStanzaPrev As Integer = -1
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)
            Dim hgcStanza As EwEHierarchyGridCell = Nothing

            For i As Integer = 1 To core.nGroups : intStanza(i) = -1 : Next

            ' Create stanza groups first
            ' Oh yes, this core exposed list(!) is zero-based
            For stanzaIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaIndex)

                ' The group list of a StanzaGroup is one-based! Are you confused yet?
                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    bInStanza(source.Index) = True
                    intStanza(source.Index) = stanzaIndex
                Next
            Next

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For groupIndex As Integer = 1 To core.nGroups

                source = core.EcoPathGroupInputs(groupIndex)

                If intStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow()
                    Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.Area)

                    cell = New PropertyCell(source, eVarNameFlags.BiomassAreaInput)
                    cell.SuppressZero = True
                    Me(iRow, 3) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    Me(iRow, 4) = cell

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.SuppressZero = True
                    Me(iRow, 5) = cell

                    cell = New PropertyCell(source, eVarNameFlags.QBInput)
                    cell.SuppressZero = True
                    Me(iRow, 6) = cell

                    Me(iRow, 7) = New PropertyCell(source, eVarNameFlags.EEInput)
                    Me(iRow, 8) = New PropertyCell(source, eVarNameFlags.GEInput)
                    Me(iRow, 9) = New PropertyCell(source, eVarNameFlags.GS)
                    Me(iRow, 10) = New PropertyCell(source, eVarNameFlags.DetImp)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanza(source.Index))
                    If intStanza(source.Index) <> intStanzaPrev Then 'If stanza group appears the first time Then diplay the + control

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)

                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells. We'll do something fancy here one day
                        For i As Integer = 2 To 9 : Me(iRow, i) = New EwERowHeaderCell() : Next

                        intStanzaPrev = intStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If

                    'display group info
                    iRow = Me.AddRow()
                    Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.Area)

                    cell = New PropertyCell(source, eVarNameFlags.BiomassAreaInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, 3) = cell

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, 4) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    cell.Behaviors.Add(m_bm)
                    Me(iRow, 5) = cell

                    cell = New PropertyCell(source, eVarNameFlags.QBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, 6) = cell

                    Me(iRow, 7) = New PropertyCell(source, eVarNameFlags.EEInput)
                    Me(iRow, 8) = New PropertyCell(source, eVarNameFlags.GEInput)
                    Me(iRow, 8).Behaviors.Add(m_bm)
                    Me(iRow, 9) = New PropertyCell(source, eVarNameFlags.GS)
                    Me(iRow, 10) = New PropertyCell(source, eVarNameFlags.DetImp)

                    hgcStanza.AddChildRow(iRow)

                End If

            Next groupIndex

        End Sub

        Friend Sub OnCellDoubleClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)
            Dim dlgEditMultiStanza As EditMultiStanza = Nothing
            Dim propStanzaDoubleClicked As cProperty = Nothing
            Dim objStanzaDoubleClicked As cEcoPathGroupInput = Nothing

            If Not TypeOf cell Is PropertyCell Then Return
            propStanzaDoubleClicked = DirectCast(cell, PropertyCell).GetProperty()
            objStanzaDoubleClicked = DirectCast(propStanzaDoubleClicked.Source, cEcoPathGroupInput)

            dlgEditMultiStanza = New EditMultiStanza(objStanzaDoubleClicked)
            dlgEditMultiStanza.ShowDialog(Me)
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Me.Rows(0).Height = 60
            Me.Columns(0).Width = 24
            Me.Columns(1).Width = 120
            Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            'Me.Columns(2).Width = 58
            'Me.Columns(3).Width = 59
            'Me.Columns(4).Width = 67
            'Me.Columns(5).Width = 78
            'Me.Columns(6).Width = 66
            'Me.Columns(7).Width = 77
            'Me.Columns(8).Width = 78
            'Me.Columns(9).Width = 69

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
