'==============================================================================
'
' $Log: MortalityCoefficientsEwEGrid.vb,v $
' Revision 1.1  2008/09/26 07:31:33  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.22  2008/08/03 01:03:21  jeroens
' Fixed issue 178
'
' Revision 1.21  2008/08/02 03:04:11  jeroens
' Renamed resources
'
' Revision 1.20  2008/06/02 00:01:26  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.19  2008/05/29 22:22:40  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.18  2008/04/07 02:31:06  jeroens
' Cleaning up resources
'
' Revision 1.17  2008/01/11 12:33:17  jeroens
' Fixed bug 299
'
' Revision 1.16  2007/10/10 02:59:12  jeroens
' * Updated to new EwEGrid MessageSource interface
'
' Revision 1.15  2007/08/10 01:00:15  joeb
' Fixed BiomassAccum bug
'
' Revision 1.14  2007/07/06 20:11:17  jeroens
' * Core stanza group list no longer exposed
'
' Revision 1.13  2007/06/21 23:57:20  fgao
' Add grid selection, autosize..etc features..
'
' Revision 1.12  2007/06/04 23:20:58  fgao
' Fixed a title display error
'
' Revision 1.11  2007/04/29 03:45:10  jeroens
' * Connected to EwEGridRefresh
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
    Public Class MortalityCoefficientsEwEGrid
        : Inherits EwEGrid

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            Me.Redim(1, 8)
            Me(0, 0) = New EwEColumnHeaderCell("")
            Me(0, 1) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_PBZ)
            Me(0, 3) = New EwEColumnHeaderCell(My.Resources.HEADER_FISHINGMORTRATE)
            Me(0, 4) = New EwEColumnHeaderCell(My.Resources.HEADER_PREDMORTRATE)
            Me(0, 5) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMACCURATE2)
            Me(0, 6) = New EwEColumnHeaderCell(My.Resources.HEADER_NETMIGRATE)
            Me(0, 7) = New EwEColumnHeaderCell(My.Resources.HEADER_OTHERMORTRATE)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim core As cCore = cCore.GetInstance()
            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim intStanzaGroupIndex(core.nLivingGroups) As Integer 'Hold the stanza group index
            Dim intStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To core.nLivingGroups : intStanzaGroupIndex(i) = -1 : Next

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
            For groupIndex As Integer = 1 To core.nLivingGroups
                source = core.EcoPathGroupOutputs(groupIndex)

                If intStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, DirectCast(source, cEcoPathGroupOutput))
                Else 'Group is stanza
                    sg = core.StanzaGroups(intStanzaGroupIndex(source.Index))
                    If intStanzaGroupIndex(source.Index) <> intStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)

                        iRow = Me.AddRow()
                        Me(iRow, 0) = hgcStanza
                        Me(iRow, 1) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)
                        ' Complete row with dummy cells
                        For i As Integer = 2 To 7 : Me(iRow, i) = New EwERowHeaderCell() : Next
                        intStanzaGroupIndexPrev = intStanzaGroupIndex(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If
                    'Display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, DirectCast(source, cEcoPathGroupOutput), True)

                    hgcStanza.AddChildRow(iRow)

                End If
            Next

        End Sub

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cEcoPathGroupOutput, Optional ByVal isIndented As Boolean = False)

            Dim cell As PropertyCell = Nothing
            Dim bMortAlert As Boolean = (source.MortCoOtherMort < 0)
            Dim bCatchAlert As Boolean = (source.MortCoFishRate > source.PBOutput)

            Me(iRow, 0) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If isIndented Then
                Me(iRow, 1) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, 1) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If
            Me(iRow, 2) = New PropertyCell(source, eVarNameFlags.PBOutput)
            Me(iRow, 3) = New PropertyCell(source, eVarNameFlags.MortCoFishRate)
            Me(iRow, 4) = New PropertyCell(source, eVarNameFlags.MortCoPredMort)
            Me(iRow, 5) = New PropertyCell(source, eVarNameFlags.BioAccumRatePerYear)
            Me(iRow, 6) = New PropertyCell(source, eVarNameFlags.MortCoNetMig)
            Me(iRow, 7) = New PropertyCell(source, eVarNameFlags.MortCoOtherMort)

            Me.SetCellAlert(DirectCast(Me(iRow, 1), EwECellBase), bMortAlert)
            Me.SetCellAlert(DirectCast(Me(iRow, 3), EwECellBase), bMortAlert And bCatchAlert)
            Me.SetCellAlert(DirectCast(Me(iRow, 7), EwECellBase), bMortAlert)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As EwECore.eMessageSource
            Get
                Return eMessageSource.EcoPath
            End Get
        End Property

        Private Sub SetCellAlert(ByVal cell As EwECellBase, ByVal bSetAlert As Boolean)
            If bSetAlert Then
                cell.Style = cell.Style Or StyleGuide.eStyleFlags.Checked
            Else
                cell.Style = cell.Style And (Not StyleGuide.eStyleFlags.Checked)
            End If
        End Sub

    End Class

End Namespace
