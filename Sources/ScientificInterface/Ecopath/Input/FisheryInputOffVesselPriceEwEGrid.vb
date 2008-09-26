'==============================================================================
'
' $Log: FisheryInputOffVesselPriceEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.5  2008/08/04 02:27:45  jeroens
' Renamed varname MarketPrice to OffVesselPrice
'
' Revision 1.4  2008/07/29 13:06:45  jeroens
' Propery renamed 'IsStatic' method
'
' Revision 1.3  2008/06/02 00:01:29  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.2  2008/05/29 22:22:42  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.1  2008/05/07 00:55:18  jeroens
' Renamed market price grid
'
' Revision 1.18  2008/04/07 02:31:09  jeroens
' Cleaning up resources
'
' Revision 1.17  2008/01/31 17:08:15  jeroens
' Made fleet column headers live updating
'
' Revision 1.16  2008/01/11 12:33:18  jeroens
' Fixed bug 299
'
' Revision 1.15  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.14  2007/07/06 20:11:18  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.13  2007/07/03 07:08:47  jeroens
' * Fixed member naming inconsistencies
'
' Revision 1.12  2007/06/22 17:33:34  fgao
' Fixed a bug: Indent the multistanza group display.
'
' Revision 1.11  2007/06/21 22:23:37  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.10  2007/04/29 03:45:12  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region "Imports Directive"

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class FisheryInputOffVesselPriceEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
            Me.FixedColumnWidths = True
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing

            Me.Redim(1, core.nFleets + 1 + 1)


            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

            ' Dynamic column header - fleet names
            For fleetIndex As Integer = 1 To core.nFleets
                source = core.FleetInputs(fleetIndex)
                Me(0, fleetIndex + 1) = New PropertyColumnHeaderCell(source, eVarNameFlags.Name, Nothing, _
                        My.Resources.GENERIC_HEADER_PROP_A_PER_B, New StyleGuide.eUnitType() {StyleGuide.eUnitType.Monetary, StyleGuide.eUnitType.Currency})
            Next

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
            For rowIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupInputs(rowIndex)

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
                        For i As Integer = 2 To core.nFleets + 1 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, source, True)
                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal isIndented As Boolean = False)
            Dim core As cCore = cCore.GetInstance()
            Dim sourceSec As cCoreInputOutputBase = Nothing

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If
            ' For each fleet
            For fleetIndex As Integer = 1 To core.nFleets
                ' Get the fleet info
                sourceSec = core.FleetInputs(fleetIndex)
                ' The market price is indexed by (fleetIndex, groupIndex)
                ' Add the dynamic property to the destined cell
                Me(iRow, fleetIndex + 1) = New PropertyCell(sourceSec, eVarNameFlags.OffVesselPrice, source)
            Next
        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

    End Class

End Namespace

