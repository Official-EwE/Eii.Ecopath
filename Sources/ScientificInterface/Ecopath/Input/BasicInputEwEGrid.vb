'==============================================================================
'
' $Log: BasicInputEwEGrid.vb,v $
' Revision 1.8  2009/05/21 19:27:14  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.7  2009/03/30 19:01:57  jeroens
' Z-column now properly hidden
'
' Revision 1.6  2009/03/12 14:11:16  jeroens
' Implemented Z/PB columns
'
' Revision 1.5  2009/03/12 01:32:44  jeroens
' SAVE before you commit! SAVE!
'
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

        Enum eColumnTypes As Integer
            Index = 0
            Name
            Area
            BA
            Z
            PB
            QB
            EE
            GE
            GS
            DetImp
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.Area) = New EwEColumnHeaderCell(My.Resources.HEADER_AREA)
            Me(0, eColumnTypes.BA) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSAREA_UNIT, StyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.Z) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, StyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.PB) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.QB) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.EE) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, eColumnTypes.GE) = New EwEColumnHeaderCell(My.Resources.HEADER_GE)
            Me(0, eColumnTypes.GS) = New EwEColumnHeaderCell(My.Resources.HEADER_UNASSIMILCONSUMPTION)
            Me(0, eColumnTypes.DetImp) = New EwEColumnHeaderCell(My.Resources.HEADER_DETIMP_UNIT, New StyleGuide.eUnitType() {StyleGuide.eUnitType.Currency, StyleGuide.eUnitType.Time})

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim cell As EwECellBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim abInStanza(core.nGroups) As Boolean
            Dim aiStanza(core.nGroups) As Integer 'Hold the stanza group number
            Dim iStanzaPrev As Integer = -1
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)
            Dim hgcStanza As EwEHierarchyGridCell = Nothing

            For i As Integer = 1 To core.nGroups : aiStanza(i) = -1 : Next

            ' Create stanza groups first
            ' Oh yes, this core exposed list(!) is zero-based
            For stanzaIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaIndex)

                ' The group list of a StanzaGroup is one-based! Are you confused yet?
                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    abInStanza(source.Index) = True
                    aiStanza(source.Index) = stanzaIndex
                Next
            Next

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For groupIndex As Integer = 1 To core.nGroups

                source = core.EcoPathGroupInputs(groupIndex)

                If aiStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Area) = New PropertyCell(source, eVarNameFlags.Area)

                    cell = New PropertyCell(source, eVarNameFlags.BiomassAreaInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.BA) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    Me(iRow, eColumnTypes.Z) = cell

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.PB) = cell

                    cell = New PropertyCell(source, eVarNameFlags.QBInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.QB) = cell

                    Me(iRow, eColumnTypes.EE) = New PropertyCell(source, eVarNameFlags.EEInput)
                    Me(iRow, eColumnTypes.GE) = New PropertyCell(source, eVarNameFlags.GEInput)
                    Me(iRow, eColumnTypes.GS) = New PropertyCell(source, eVarNameFlags.GS)
                    Me(iRow, eColumnTypes.DetImp) = New PropertyCell(source, eVarNameFlags.DetImp)

                Else 'Group is stanza

                    sg = core.StanzaGroups(aiStanza(source.Index))
                    If aiStanza(source.Index) <> iStanzaPrev Then 'If stanza group appears the first time Then diplay the + control

                        ' Fill row with dummy cells. We'll do something fancy here one day
                        iRow = Me.AddRow()
                        For i As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)

                        iStanzaPrev = aiStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If

                    'display group info
                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Area) = New PropertyCell(source, eVarNameFlags.Area)

                    cell = New PropertyCell(source, eVarNameFlags.BiomassAreaInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.BA) = cell

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.Z) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    cell.Behaviors.Add(m_bm)
                    Me(iRow, eColumnTypes.PB) = cell

                    cell = New PropertyCell(source, eVarNameFlags.QBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.QB) = cell

                    Me(iRow, eColumnTypes.EE) = New PropertyCell(source, eVarNameFlags.EEInput)
                    Me(iRow, eColumnTypes.GE) = New PropertyCell(source, eVarNameFlags.GEInput)
                    Me(iRow, eColumnTypes.GE).Behaviors.Add(m_bm)
                    Me(iRow, eColumnTypes.GS) = New PropertyCell(source, eVarNameFlags.GS)
                    Me(iRow, eColumnTypes.DetImp) = New PropertyCell(source, eVarNameFlags.DetImp)

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

            Dim core As cCore = cCore.GetInstance()
            Dim ci As ColumnInfo = Me.Columns(eColumnTypes.Z)

            Me.Rows(0).Height = 60
            Me.Columns(0).Width = 24
            Me.Columns(1).Width = 120
            Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

            ci.Visible = (core.nStanzas > 0)

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
