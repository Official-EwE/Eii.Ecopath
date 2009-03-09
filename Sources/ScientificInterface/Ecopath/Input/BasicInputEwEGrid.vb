'==============================================================================
'
' $Log: BasicInputEwEGrid.vb,v $
' Revision 1.3  2009/03/09 15:05:07  jeroens
' Split P/B col into P/B (non stanza), Z (stanza)
'
' Revision 1.2  2009/01/16 18:30:09  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.1  2008/09/26 07:31:30  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.32  2008/09/16 01:29:02  jeroens
' Suppressed a whack of zeroes
'
' Revision 1.31  2008/08/11 16:13:55  jeroens
' Generalized EndEditHandler
'
' Revision 1.30  2008/08/02 03:04:12  jeroens
' Renamed resources
'
' Revision 1.29  2008/06/02 00:01:27  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.28  2008/05/29 22:22:41  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.27  2008/04/07 02:31:07  jeroens
' Cleaning up resources
'
' Revision 1.26  2008/01/11 12:33:19  jeroens
' Fixed bug 299
'
' Revision 1.25  2007/10/10 02:59:13  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.24  2007/08/03 16:30:25  jeroens
' * Uses cell-based unit support
'
' Revision 1.23  2007/07/06 20:11:17  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.22  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.21  2007/06/13 22:04:53  fgao
' Fixed Bug 67: Relating to Grid cell alignment.
'
' Revision 1.20  2007/05/04 23:53:05  fgao
' Add temporary Unit settings for grid header
'
' Revision 1.19  2007/04/29 03:45:11  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.18  2007/04/18 01:06:47  joeh
' *Fine tune EditMultiStanza UI
'
' Revision 1.17  2007/04/14 15:24:14  jeroens
' - Removed stanza group count diagnostics
'
' Revision 1.16  2007/04/14 00:19:53  joeh
' *add "Edit Multi Stanza" submenu
'
' Revision 1.15  2007/04/06 01:04:40  joeh
' no message
'
' Revision 1.14  2007/04/03 23:17:39  joeh
' *"Edit multi stanza groups" button enabled upfront
'
' Revision 1.13  2007/03/31 01:09:06  joeh
' *First shot to add "Edit Multi Stanza" button at the top of Basic Input grid
'
' Revision 1.12  2007/03/30 17:16:45  joeh
' *Double click, instead of single click, on the cell of Basic Input to invoke the Edit Multi Stanza UI
'
' Revision 1.11  2007/03/26 22:04:05  joeh
' *Second shot at Edit Multi Stanza UI
'
' Revision 1.10  2007/03/19 23:13:31  joeh
' First shot at edit multi stanza ui
'
' Revision 1.9  2007/03/09 20:38:54  joeh
' * Implement second shot at stanza hierarchy
' * The groups, stanza or not, will be listed in the user's input order
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
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, StyleGuide.eUnitType.Time)
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

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.SuppressZero = True
                    Me(iRow, 4) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
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

                    cell = New EwECell("", GetType(String))
                    cell.Style = StyleGuide.eStyleFlags.NotEditable
                    cell.Behaviors.Add(m_bm)
                    Me(iRow, 4) = cell

                    cell = New PropertyCell(source, eVarNameFlags.PBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
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
