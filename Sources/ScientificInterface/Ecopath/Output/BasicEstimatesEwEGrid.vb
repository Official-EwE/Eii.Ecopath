#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
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
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_GROUPNAME)
            Me(0, eColumnTypes.TL) = New EwEColumnHeaderCell(SharedResources.HEADER_TROPHICLEVEL)
            Me(0, eColumnTypes.Area) = New EwEColumnHeaderCell(SharedResources.HEADER_AREA)
            Me(0, eColumnTypes.BA) = New EwEColumnHeaderCell(eVarNameFlags.BiomassAreaInput, SharedResources.GENERIC_LABEL_UNIT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.B) = New EwEColumnHeaderCell(eVarNameFlags.Biomass, SharedResources.GENERIC_LABEL_UNIT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.Z) = New EwEColumnHeaderCell(SharedResources.HEADER_TOTALMORTALITY_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.PB) = New EwEColumnHeaderCell(SharedResources.HEADER_PB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.QB) = New EwEColumnHeaderCell(SharedResources.HEADER_QB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.EE) = New EwEColumnHeaderCell(SharedResources.HEADER_EE)
            Me(0, eColumnTypes.GE) = New EwEColumnHeaderCell(SharedResources.HEADER_GE)

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
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name, Nothing, hgcStanza)

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

            Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
            If bIsStanza Then
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
            Else
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
            End If

            Me(iRow, eColumnTypes.TL) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.TTLX)
            Me(iRow, eColumnTypes.Area) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Area)
            Me(iRow, eColumnTypes.BA) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BiomassAreaOutput)
            Me(iRow, eColumnTypes.B) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Biomass)

            If bIsStanza Then
                Me(iRow, eColumnTypes.Z) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PBOutput)
            Else
                cell = New EwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.Z) = cell
            End If

            If Not bIsStanza Then
                Me(iRow, eColumnTypes.PB) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PBOutput)
            Else
                cell = New EwECell("", GetType(String))
                cell.Style = cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.PB) = cell
            End If

            Me(iRow, eColumnTypes.QB) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.QBOutput)
            Me(iRow, eColumnTypes.EE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EEOutput)
            Me(iRow, eColumnTypes.GE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.GEOutput)

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

            If (Me.Core Is Nothing) Then Return

            ci.Visible = (Me.Core.nStanzas > 0)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
