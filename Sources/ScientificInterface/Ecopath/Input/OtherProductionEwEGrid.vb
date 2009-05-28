'==============================================================================
'
' $Log: OtherProductionEwEGrid.vb,v $
' Revision 1.5  2009/05/28 12:37:00  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.4  2009/05/21 19:27:17  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.3  2009/01/16 18:30:11  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:55:38  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.16  2008/08/02 03:04:13  jeroens
' Renamed resources
'
' Revision 1.15  2008/06/02 00:01:29  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.14  2008/05/29 22:22:42  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.13  2008/04/07 02:31:10  jeroens
' Cleaning up resources
'
' Revision 1.12  2008/01/11 12:33:18  jeroens
' Fixed bug 299
'
' Revision 1.11  2007/10/10 02:59:14  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.10  2007/08/03 17:18:10  jeroens
' * Uses cell-based unit support
'
' Revision 1.9  2007/07/06 20:11:18  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.8  2007/06/21 22:23:37  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.7  2007/05/04 23:53:05  fgao
' Add temporary Unit settings for grid header
'
' Revision 1.6  2007/04/29 03:45:12  jeroens
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
    Public Class OtherProductionEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Dim aUnitType As cStyleGuide.eUnitType() = {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time}

            Me.Redim(1, 7)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_IMMIGRATION_UNIT, aUnitType)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_EMIGRATION_UNIT, aUnitType)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_EMIGRATIONRATE_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMACCU_UNIT, aUnitType)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMACCURATE_UNIT, cStyleGuide.eUnitType.Time)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            'Add by joeh
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim blnStanza(core.nLivingGroups) As Boolean
            Dim intStanza(core.nLivingGroups) As Integer 'Hold the stanza group number
            Dim intStanzaPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nLivingGroups : intStanza(i) = -1 : Next

            'Remove existing rows
            Me.RowsCount = 1

            'Tag stanza group first
            For stanzaGroupIndex As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(stanzaGroupIndex)
                For stanzaIndex As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(stanzaIndex))
                    blnStanza(source.Index) = True
                    intStanza(source.Index) = stanzaGroupIndex
                Next
            Next
            'End add by joeh

            'Change by joeh
            'For groupIndex As Integer = 1 To core.nLivingGroups

            '    Me.Rows.Insert(groupIndex)
            '    source = core.EcoPathGroupInputs(groupIndex)

            '    Me(groupIndex, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            '    Me(groupIndex, 1) = New PropertyCell(source, eVarNameFlags.Immig)
            '    Me(groupIndex, 2) = New PropertyCell(source, eVarNameFlags.Emig)
            '    Me(groupIndex, 3) = New PropertyCell(source, eVarNameFlags.EmigRate)
            '    Me(groupIndex, 4) = New PropertyCell(source, eVarNameFlags.BioAccum)
            '    Me(groupIndex, 5) = New PropertyCell(source, eVarNameFlags.BioAccumRate)
            'Next groupIndex

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupInputs(groupIndex)

                If intStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.Immig)
                    Me(iRow, 3) = New PropertyCell(source, eVarNameFlags.Emig)
                    Me(iRow, 4) = New PropertyCell(source, eVarNameFlags.EmigRate)
                    Me(iRow, 5) = New PropertyCell(source, eVarNameFlags.BioAccum)
                    Me(iRow, 6) = New PropertyCell(source, eVarNameFlags.BioAccumRate)
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanza(source.Index))
                    If intStanza(source.Index) <> intStanzaPrev Then 'If stanza group appears the first time Then display + control
                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        'Complete row with dummy cells
                        For i As Integer = 2 To 6 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaPrev = intStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    hgcStanza.AddChildRow(iRow)
                    Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
                    Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
                    Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.Immig)
                    Me(iRow, 3) = New PropertyCell(source, eVarNameFlags.Emig)
                    Me(iRow, 4) = New PropertyCell(source, eVarNameFlags.EmigRate)
                    Me(iRow, 5) = New PropertyCell(source, eVarNameFlags.BioAccum)
                    Me(iRow, 6) = New PropertyCell(source, eVarNameFlags.BioAccumRate)
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
