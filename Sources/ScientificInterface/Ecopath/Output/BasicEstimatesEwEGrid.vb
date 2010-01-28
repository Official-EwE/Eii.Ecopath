'==============================================================================
'
' $Log: BasicEstimatesEwEGrid.vb,v $
' Revision 1.7  2009/05/28 12:36:54  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.6  2009/05/21 19:27:10  jeroens
' eCoreComponentTypes moved to EwEUtils
'
' Revision 1.5  2009/03/30 19:01:56  jeroens
' Z-column now properly hidden
'
' Revision 1.4  2009/03/12 14:11:16  jeroens
' Implemented Z/PB columns
'
' Revision 1.3  2009/01/16 18:30:07  jeroens
' eMessageSource renamed to eCoreComponentTypes
'
' Revision 1.2  2008/12/15 15:58:24  jeroens
' no message
'
' Revision 1.1  2008/09/26 07:31:32  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SourceGrid2

#End Region

Namespace Ecopath.Output

    <CLSCompliant(False)> _
    Public Class BasicEstimatesEwEGrid
        : Inherits EwEGrid

        Enum eColumnTypes As Integer
            Index = 0
            Name
            TL
            Area
            BA
            B
            Z
            PB
            QB
            EE
            GE
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()
            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)
            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.TL) = New EwEColumnHeaderCell(My.Resources.HEADER_TROPHICLEVEL)
            Me(0, eColumnTypes.Area) = New EwEColumnHeaderCell(My.Resources.HEADER_AREA)
            Me(0, eColumnTypes.BA) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSAREA_UNIT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.B) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASS_UNIT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.Z) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.PB) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.QB) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.EE) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, eColumnTypes.GE) = New EwEColumnHeaderCell(My.Resources.HEADER_GE)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim aiStanzaGroupIndex(core.nGroups) As Integer 'Hold the stanza group index
            Dim iStanzaGroupIndexPrev As Integer = -1
            Dim hgcStanza As EwEHierarchyGridCell = Nothing
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)

            For i As Integer = 1 To Me.Core.nGroups : aiStanzaGroupIndex(i) = -1 : Next

            'Tag stanza group
            For iStanzaGroup As Integer = 0 To core.nStanzas - 1
                sg = core.StanzaGroups(iStanzaGroup)

                For iStanza As Integer = 1 To sg.NStanzas
                    source = core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    aiStanzaGroupIndex(source.Index) = iStanzaGroup
                Next
            Next

            'Remove existing rows
            Me.RowsCount = 1

            'Create rows for all groups
            For groupIndex As Integer = 1 To core.nGroups
                source = core.EcoPathGroupOutputs(groupIndex)

                If aiStanzaGroupIndex(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow
                    FillInRows(iRow, source)
                Else 'Group is stanza
                    sg = core.StanzaGroups(aiStanzaGroupIndex(source.Index))
                    If aiStanzaGroupIndex(source.Index) <> iStanzaGroupIndexPrev Then 'If stanza group appears the first time Then diplay the + control

                        ' Complete row with dummy cells
                        iRow = Me.AddRow()
                        For i As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(sg, eVarNameFlags.Name)

                        iStanzaGroupIndexPrev = aiStanzaGroupIndex(source.Index)

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

        Private Sub FillInRows(ByVal iRow As Integer, ByVal source As cCoreInputOutputBase, Optional ByVal bIsStanza As Boolean = False)

            Dim cell As EwECellBase = Nothing

            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(source, eVarNameFlags.Index)
            If bIsStanza Then
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(source, eVarNameFlags.Name)
            End If

            Me(iRow, eColumnTypes.TL) = New PropertyCell(source, eVarNameFlags.TTLX)
            Me(iRow, eColumnTypes.Area) = New PropertyCell(source, eVarNameFlags.Area)
            Me(iRow, eColumnTypes.BA) = New PropertyCell(source, eVarNameFlags.BiomassAreaOutput)
            Me(iRow, eColumnTypes.B) = New PropertyCell(source, eVarNameFlags.Biomass)

            If bIsStanza Then
                Me(iRow, eColumnTypes.Z) = New PropertyCell(source, eVarNameFlags.PBOutput)
            Else
                cell = New EwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.Z) = cell
            End If

            If Not bIsStanza Then
                Me(iRow, eColumnTypes.PB) = New PropertyCell(source, eVarNameFlags.PBOutput)
            Else
                cell = New EwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.PB) = cell
            End If

            Me(iRow, eColumnTypes.QB) = New PropertyCell(source, eVarNameFlags.QBOutput)
            Me(iRow, eColumnTypes.EE) = New PropertyCell(source, eVarNameFlags.EEOutput)
            Me(iRow, eColumnTypes.GE) = New PropertyCell(source, eVarNameFlags.GEOutput)

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Dim ci As ColumnInfo = Me.Columns(eColumnTypes.Z)

            Me.Rows(0).Height = 60
            Me.Columns(eColumnTypes.Index).Width = 24
            Me.Columns(eColumnTypes.Name).Width = 120
            Me.Columns(eColumnTypes.Name).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

            ci.Visible = (core.nStanzas > 0)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
