#Region " Imports "

Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports SourceGrid2
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid displaying Ecopath Basic Input information.
    ''' </summary>
    ''' =======================================================================
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
            Me(0, eColumnTypes.BA) = New EwEColumnHeaderCell(My.Resources.HEADER_BIOMASSAREA_UNIT, cStyleGuide.eUnitType.Currency)
            Me(0, eColumnTypes.Z) = New EwEColumnHeaderCell(My.Resources.HEADER_TOTALMORTALITY_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.PB) = New EwEColumnHeaderCell(My.Resources.HEADER_PB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.QB) = New EwEColumnHeaderCell(My.Resources.HEADER_QB_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.EE) = New EwEColumnHeaderCell(My.Resources.HEADER_EE)
            Me(0, eColumnTypes.GE) = New EwEColumnHeaderCell(My.Resources.HEADER_GE)
            Me(0, eColumnTypes.GS) = New EwEColumnHeaderCell(My.Resources.HEADER_UNASSIMILCONSUMPTION)
            Me(0, eColumnTypes.DetImp) = New EwEColumnHeaderCell(My.Resources.HEADER_DETIMP_UNIT, New cStyleGuide.eUnitType() {cStyleGuide.eUnitType.Currency, cStyleGuide.eUnitType.Time})

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim source As cCoreInputOutputBase = Nothing
            Dim cell As EwECellBase = Nothing
            Dim sg As cStanzaGroup = Nothing
            Dim iRow As Integer = -1
            Dim abInStanza(Core.nGroups) As Boolean
            Dim aiStanza(Core.nGroups) As Integer 'Hold the stanza group number
            Dim iStanzaPrev As Integer = -1
            Dim dtStanzaCells As New Dictionary(Of cStanzaGroup, EwEHierarchyGridCell)
            Dim hgcStanza As EwEHierarchyGridCell = Nothing

            For i As Integer = 1 To Core.nGroups : aiStanza(i) = -1 : Next

            ' Create stanza groups first
            ' Oh yes, this core exposed list(!) is zero-based
            For stanzaIndex As Integer = 0 To Core.nStanzas - 1
                sg = Core.StanzaGroups(stanzaIndex)

                ' The group list of a StanzaGroup is one-based! Are you confused yet?
                For iStanza As Integer = 1 To sg.NStanzas
                    source = Core.EcoPathGroupInputs(sg.iGroups(iStanza))
                    abInStanza(source.Index) = True
                    aiStanza(source.Index) = stanzaIndex
                Next
            Next

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For groupIndex As Integer = 1 To Core.nGroups

                source = Core.EcoPathGroupInputs(groupIndex)

                If aiStanza(source.Index) = -1 Then 'If group is non-stanza Then display group info
                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Area) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Area)

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BiomassAreaInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.BA) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    Me(iRow, eColumnTypes.Z) = cell

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PBInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.PB) = cell

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.QBInput)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.QB) = cell

                    Me(iRow, eColumnTypes.EE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EEInput)
                    Me(iRow, eColumnTypes.GE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.GEInput)
                    Me(iRow, eColumnTypes.GS) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.GS)
                    Me(iRow, eColumnTypes.DetImp) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.DetImp)

                Else 'Group is stanza

                    sg = Core.StanzaGroups(aiStanza(source.Index))
                    If aiStanza(source.Index) <> iStanzaPrev Then 'If stanza group appears the first time Then diplay the + control

                        ' Fill row with dummy cells. We'll do something fancy here one day
                        iRow = Me.AddRow()
                        For i As Integer = 0 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next

                        hgcStanza = New EwEHierarchyGridCell()
                        dtStanzaCells.Add(sg, hgcStanza)
                        Me(iRow, eColumnTypes.Index) = hgcStanza
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, sg, eVarNameFlags.Name)

                        iStanzaPrev = aiStanza(source.Index)
                    Else
                        hgcStanza = dtStanzaCells(sg)
                    End If

                    'display group info
                    iRow = Me.AddRow()
                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, source, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Area) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Area)

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BiomassAreaInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.BA) = cell

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.PBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.Z) = cell

                    cell = New EwECell("", GetType(String))
                    cell.Style = cStyleGuide.eStyleFlags.NotEditable
                    cell.Behaviors.Add(m_bm)
                    Me(iRow, eColumnTypes.PB) = cell

                    cell = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.QBInput)
                    cell.Behaviors.Add(m_bm)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.QB) = cell

                    Me(iRow, eColumnTypes.EE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.EEInput)
                    Me(iRow, eColumnTypes.GE) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.GEInput)
                    Me(iRow, eColumnTypes.GE).Behaviors.Add(m_bm)
                    Me(iRow, eColumnTypes.GS) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.GS)
                    Me(iRow, eColumnTypes.DetImp) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.DetImp)

                    hgcStanza.AddChildRow(iRow)

                End If

            Next groupIndex

        End Sub

        Friend Sub OnCellDoubleClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)
            Dim dlg As EditMultiStanza = Nothing
            Dim prop As cProperty = Nothing
            Dim group As cEcoPathGroupInput = Nothing

            If Not TypeOf cell Is PropertyCell Then Return
            prop = DirectCast(cell, PropertyCell).GetProperty()
            group = DirectCast(prop.Source, cEcoPathGroupInput)

            dlg = New EditMultiStanza(Me.UIContext, group)
            dlg.ShowDialog(Me)
        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Dim ci As ColumnInfo = Me.Columns(eColumnTypes.Z)

            Me.Rows(0).Height = 60
            Me.Columns(0).Width = 24
            Me.Columns(1).Width = 120
            Me.Columns(1).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize

            For i As Integer = 2 To Me.ColumnsCount - 1
                Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
            Next

            If Me.UIContext Is Nothing Then Return

            ci.Visible = (Me.Core.nStanzas > 0)

        End Sub

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

    End Class

End Namespace
