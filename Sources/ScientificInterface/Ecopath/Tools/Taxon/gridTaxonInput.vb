#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterface.Other
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports 

Namespace Ecopath.Input

    ''' =======================================================================
    ''' <summary>
    ''' Grid displaying Ecopath Basic Input information.
    ''' </summary>
    ''' =======================================================================
    <CLSCompliant(False)> _
    Public Class gridTaxonInput
        : Inherits EwEGrid

        Private m_editorEcology As EwEComboBoxCellEditor(Of eEcologyTypes) = Nothing
        Private m_editorConservation As EwEComboBoxCellEditor(Of eIUCNConservationStatusTypes) = Nothing
        Private m_editorOrganism As EwEComboBoxCellEditor(Of eOrganismTypes) = Nothing
        Private m_editorOccurrence As EwEComboBoxCellEditor(Of eOccurrenceStatusTypes) = Nothing

        Enum eColumnTypes As Integer
            Index = 0
            Name
            Ecology
            Organism
            Exploited
            Conservation
            Occurrence
            MeanLen
            MaxLen
            MeanWeight
            MeanLifeSpan
            VulIndex
        End Enum

        Public Sub New()
            MyBase.new()
        End Sub

        Protected Overrides Sub InitStyle()

            ' ToDo_JS: globalize this

            MyBase.InitStyle()

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Index) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell("Common name")
            Me(0, eColumnTypes.Ecology) = New EwEColumnHeaderCell("Ecology")
            Me(0, eColumnTypes.Organism) = New EwEColumnHeaderCell("Organism")
            Me(0, eColumnTypes.Exploited) = New EwEColumnHeaderCell("Exploited")
            Me(0, eColumnTypes.Conservation) = New EwEColumnHeaderCell("Conservation status")
            Me(0, eColumnTypes.Occurrence) = New EwEColumnHeaderCell("Occurrence status")
            Me(0, eColumnTypes.MeanLen) = New EwEColumnHeaderCell("Mean length")
            Me(0, eColumnTypes.MaxLen) = New EwEColumnHeaderCell("Max length")
            Me(0, eColumnTypes.MeanWeight) = New EwEColumnHeaderCell("Mean weight")
            Me(0, eColumnTypes.MeanLifeSpan) = New EwEColumnHeaderCell("Mean life span")
            Me(0, eColumnTypes.VulIndex) = New EwEColumnHeaderCell("Vulnerability index")

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            Dim group As cCoreInputOutputBase = Nothing
            Dim taxon As cTaxon = Nothing
            Dim cell As EwECellBase = Nothing
            Dim cellParent As EwEHierarchyGridCell = Nothing
            Dim iRow As Integer = -1

            ' Prepare editors
            Me.m_editorEcology = New EwEComboBoxCellEditor(Of eEcologyTypes) _
                                                          (DirectCast([Enum].GetValues(GetType(eEcologyTypes)), eEcologyTypes()), _
                                                           New cEcologyTypeFormatter())
            Me.m_editorConservation = New EwEComboBoxCellEditor(Of eIUCNConservationStatusTypes) _
                                                          (DirectCast([Enum].GetValues(GetType(eIUCNConservationStatusTypes)), eIUCNConservationStatusTypes()), _
                                                           New cIUCNConservationTypeFormatter())
            Me.m_editorOrganism = New EwEComboBoxCellEditor(Of eOrganismTypes) _
                                                          (DirectCast([Enum].GetValues(GetType(eOrganismTypes)), eOrganismTypes()), _
                                                           New cOrganismTypeFormatter())
            Me.m_editorOccurrence = New EwEComboBoxCellEditor(Of eOccurrenceStatusTypes) _
                                                          (DirectCast([Enum].GetValues(GetType(eOccurrenceStatusTypes)), eOccurrenceStatusTypes()), _
                                                           New cOccurrenceTypeFormatter())

            ' Sort taxa by group
            Dim aiGroupTaxa(Me.Core.nGroups) As List(Of cTaxon)
            For iGroup As Integer = 0 To Me.Core.nGroups
                aiGroupTaxa(iGroup) = New List(Of cTaxon)
            Next
            For iTaxon As Integer = 1 To Me.Core.nTaxon
                taxon = Me.Core.Taxon(iTaxon)
                aiGroupTaxa(taxon.Group).Add(taxon)
            Next

            ' Remove existing rows
            Me.RowsCount = 1

            ' Create rows for all groups
            For iGroup As Integer = 1 To Core.nGroups

                group = Core.EcoPathGroupInputs(iGroup)

                ' Add group
                iRow = Me.AddRow()
                For i As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1 : Me(iRow, i) = New EwERowHeaderCell() : Next

                cellParent = New EwEHierarchyGridCell()
                Me(iRow, eColumnTypes.Index) = cellParent
                Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, group, eVarNameFlags.Name, Nothing, cellParent)

                For Each taxon In aiGroupTaxa(iGroup)

                    ' Add taxon
                    iRow = Me.AddRow()
                    Me.Taxon(iRow) = taxon

                    Me(iRow, eColumnTypes.Index) = New PropertyRowHeaderCell(Me.PropertyManager, taxon, eVarNameFlags.Index)

                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, taxon, eVarNameFlags.Name)
                    Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)

                    Me(iRow, eColumnTypes.Ecology) = New SourceGrid2.Cells.Real.Cell(taxon.EcologyType, Me.m_editorEcology)
                    Me(iRow, eColumnTypes.Ecology).Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.Organism) = New SourceGrid2.Cells.Real.Cell(taxon.OrganismType, Me.m_editorOrganism)
                    Me(iRow, eColumnTypes.Organism).Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.Conservation) = New SourceGrid2.Cells.Real.Cell(taxon.IUCNConservationType, Me.m_editorConservation)
                    Me(iRow, eColumnTypes.Conservation).Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.Occurrence) = New SourceGrid2.Cells.Real.Cell(taxon.OccurrenceType, Me.m_editorOccurrence)
                    Me(iRow, eColumnTypes.Occurrence).Behaviors.Add(Me.EwEEditHandler)

                    Me(iRow, eColumnTypes.Exploited) = New SourceGrid2.Cells.Real.CheckBox(taxon.ExploitedType)
                    Me(iRow, eColumnTypes.Exploited).Behaviors.Add(Me.EwEEditHandler)

                    cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonMeanLength)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.MeanLen) = cell

                    cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonMaxLength)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.MaxLen) = cell

                    cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonMeanWeight)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.MeanWeight) = cell

                    cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonMeanLifespan)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.MeanLifeSpan) = cell

                    cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonVulnerabilityIndex)
                    cell.SuppressZero = True
                    Me(iRow, eColumnTypes.VulIndex) = cell

                    cellParent.AddChildRow(iRow)
                Next
            Next iGroup

        End Sub

        Protected Property Taxon(ByVal iRow As Integer) As cTaxon
            Get
                Return DirectCast(Me.Rows(iRow).Tag, cTaxon)
            End Get
            Set(ByVal value As cTaxon)
                Me.Rows(iRow).Tag = value
            End Set
        End Property

        Public Overrides ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.EcoPath
            End Get
        End Property

        Protected Overrides Function OnCellValueChanged(ByVal p As SourceGrid2.Position, ByVal cell As SourceGrid2.Cells.ICellVirtual) As Boolean
            Dim taxon As cTaxon = Me.Taxon(p.Row)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.Conservation
                    taxon.IUCNConservationType = CType(cell.GetValue(p), eIUCNConservationStatusTypes)
                Case eColumnTypes.Ecology
                    taxon.EcologyType = CType(cell.GetValue(p), eEcologyTypes)
                Case eColumnTypes.Exploited
                    taxon.ExploitedType = CBool(cell.GetValue(p))
                Case eColumnTypes.Occurrence
                    taxon.OccurrenceType = CType(cell.GetValue(p), eOccurrenceStatusTypes)
                Case eColumnTypes.Organism
                    taxon.OrganismType = CType(cell.GetValue(p), eOrganismTypes)
                Case Else

            End Select
            Return MyBase.OnCellValueChanged(p, cell)
        End Function

    End Class

End Namespace
