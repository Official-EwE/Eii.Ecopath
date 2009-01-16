'==============================================================================
'
' $Log: FisheryInputNonMarketPriceEwEGrid.vb,v $
' Revision 1.3  2009/01/16 18:30:11  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:53:40  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.18  2008/08/02 03:04:13  jeroens
' Renamed resources
'
' Revision 1.17  2008/06/02 00:01:29  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.16  2008/05/29 22:22:42  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.15  2008/04/07 02:31:09  jeroens
' Cleaning up resources
'
' Revision 1.14  2008/01/11 12:33:18  jeroens
' Fixed bug 299
'
' Revision 1.13  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.12  2007/08/23 14:50:51  jeroens
' * Moved NonMarketValue from fleet to groupinput
'
' Revision 1.11  2007/07/06 20:11:18  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.10  2007/06/22 17:33:34  fgao
' Fixed a bug: Indent the multistanza group display.
'
' Revision 1.9  2007/04/29 03:45:12  jeroens
' * Connected to EwEGridRefresh
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecopath.Input

    <CLSCompliant(False)> _
    Public Class FisheryInputNonMarketPriceEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, 3)

            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_VALUEBIOMASS)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim group As cEcoPathGroupInput = Nothing
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
                    group = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    intStanzaGroupIndex(group.Index) = stanzaGroupIndex
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For rowIndex As Integer = 1 To core.nGroups
                group = core.EcoPathGroupInputs(rowIndex)
                ' Is group stanza?
                If intStanzaGroupIndex(group.Index) = -1 Then
                    ' #No: display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, group)
                Else
                    '#Yes: Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(group.Index))
                    If intStanzaGroupIndex(group.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 2 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(group.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    FillInRows(iRow, group, True)
                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal group As cEcoPathGroupInput, Optional ByVal isIndented As Boolean = False)
            ' Get the group name from EcopathInput
            Me(iRow, 0) = New PropertyRowHeaderCell(group, eVarNameFlags.Index)

            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(group, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(group, eVarNameFlags.Name)
            End If

            Me(iRow, 2) = New PropertyCell(group, eVarNameFlags.NonMarketValue)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
