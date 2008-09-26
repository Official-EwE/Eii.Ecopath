'==============================================================================
'
' $Log: BasicEstimatesEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.26  2008/08/02 03:04:10  jeroens
' Renamed resources
'
' Revision 1.25  2008/06/02 00:01:25  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.24  2008/05/29 22:22:39  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.23  2008/04/07 02:31:05  jeroens
' Cleaning up resources
'
' Revision 1.22  2008/01/11 12:33:18  jeroens
' Fixed bug 299
'
' Revision 1.21  2007/10/10 02:59:11  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.20  2007/08/03 17:17:59  jeroens
' * Uses cell-based unit support
'
' Revision 1.19  2007/07/06 20:11:17  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.18  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.17  2007/06/21 22:23:36  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.16  2007/06/13 22:36:08  fgao
' Fixed Bug 67: Relating to Grid cell alignment.
'
' Revision 1.15  2007/06/13 22:04:53  fgao
' Fixed Bug 67: Relating to Grid cell alignment.
'
' Revision 1.14  2007/05/04 23:53:05  fgao
' Add temporary Unit settings for grid header
'
' Revision 1.13  2007/04/29 03:45:09  jeroens
' * Connected to EwEGridRefresh
'
' Revision 1.12  2007/04/19 19:19:22  joeh
' *Fix the first two columns in the grid
'
' Revision 1.11  2007/04/18 19:53:07  joeh
' *Implement stanza heirarchy
'
' Revision 1.10  2006/12/01 23:31:15  jeroens
' + Added Index column again; core messages refer to Group indices
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class BasicEstimatesEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 10)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_TROPHICLEVEL)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_AREA)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSAREA_UNIT, StyleGuide.eUnitType.Currency)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS_UNIT, StyleGuide.eUnitType.Currency)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, StyleGuide.eUnitType.Time)
            Me(0, 8) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, 9) = New EwEColumnHeaderCell(My.Resources.HEADER_GE)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
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

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupOutputs(groupIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 9 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, source, True)
                End If
            Next groupIndex

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)
            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If
            Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.TTLX)
            Me(iRow, 3) = New PropertyCell(source, eVarNameFlags.Area)
            Me(iRow, 4) = New PropertyCell(source, eVarNameFlags.BiomassAreaOutput)
            Me(iRow, 5) = New PropertyCell(source, eVarNameFlags.Biomass)
            Me(iRow, 6) = New PropertyCell(source, eVarNameFlags.PBOutput)
            Me(iRow, 7) = New PropertyCell(source, eVarNameFlags.QBOutput)
            Me(iRow, 8) = New PropertyCell(source, eVarNameFlags.EEOutput)
            Me(iRow, 9) = New PropertyCell(source, eVarNameFlags.GEOutput)
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Me.Rows(0).Height = 60
            Me.Columns(0).Width = 24
            Me.Columns(1).Width = 120
            Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
            Me.Columns(2).Width = 52
            Me.Columns(3).Width = 53
            Me.Columns(4).Width = 67
            Me.Columns(5).Width = 58
            Me.Columns(6).Width = 66
            Me.Columns(7).Width = 82
            Me.Columns(8).Width = 69
            Me.Columns(9).Width = 76

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace
