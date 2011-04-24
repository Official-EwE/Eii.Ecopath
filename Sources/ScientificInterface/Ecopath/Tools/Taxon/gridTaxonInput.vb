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

        Private m_editorEcology As EwEComboBoxCellEditor = Nothing
        Private m_editorConservation As EwEComboBoxCellEditor = Nothing
        Private m_editorOrganism As EwEComboBoxCellEditor = Nothing
        Private m_editorOccurrence As EwEComboBoxCellEditor = Nothing

        Enum eColumnTypes As Integer
            Hierarchy = 0
            Name
            Organism
            Ecology
            Occurrence
            PropCatch
            Conservation
            VulIndex
            MeanLen
            MaxLen
            MeanWeight
            MeanLifeSpan
        End Enum

        Public Sub New()
            MyBase.New()

            ' Prepare editors
            Me.m_editorEcology = New EwEComboBoxCellEditor(New cEcologyTypeFormatter())
            Me.m_editorConservation = New EwEComboBoxCellEditor(New cIUCNConservationTypeFormatter())
            Me.m_editorOrganism = New EwEComboBoxCellEditor(New cOrganismTypeFormatter())
            Me.m_editorOccurrence = New EwEComboBoxCellEditor(New cOccurrenceTypeFormatter())

        End Sub

        Protected Overrides Sub InitStyle()

            MyBase.InitStyle()

            If (Me.UIContext Is Nothing) Then Return

            Me.Redim(1, [Enum].GetValues(GetType(eColumnTypes)).Length)

            Me(0, eColumnTypes.Hierarchy) = New EwEColumnHeaderCell("")
            Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_COMMON_NAME)
            Me(0, eColumnTypes.Ecology) = New EwEColumnHeaderCell(SharedResources.HEADER_ECOLOGY)
            Me(0, eColumnTypes.Organism) = New EwEColumnHeaderCell(SharedResources.HEADER_ORGANISM)
            Me(0, eColumnTypes.PropCatch) = New EwEColumnHeaderCell(SharedResources.HEADER_PROPORTION_CATCH)
            Me(0, eColumnTypes.Conservation) = New EwEColumnHeaderCell(SharedResources.HEADER_IUCN_CONSERVATION_STATUS)
            Me(0, eColumnTypes.Occurrence) = New EwEColumnHeaderCell(SharedResources.HEADER_OCCURRENCE_STATUS)
            Me(0, eColumnTypes.MeanLen) = New EwEColumnHeaderCell(SharedResources.HEADER_MEAN_LENGTH)
            Me(0, eColumnTypes.MaxLen) = New EwEColumnHeaderCell(SharedResources.HEADER_MAX_LENGTH)
            Me(0, eColumnTypes.MeanWeight) = New EwEColumnHeaderCell(SharedResources.HEADER_MEAN_WEIGHT)
            Me(0, eColumnTypes.MeanLifeSpan) = New EwEColumnHeaderCell(SharedResources.HEADER_MEAN_LIFESPAN_UNIT, cStyleGuide.eUnitType.Time)
            Me(0, eColumnTypes.VulIndex) = New EwEColumnHeaderCell(SharedResources.HEADER_VULNERABILITY_INDEX)

            Me.FixedColumns = 2

        End Sub

        Protected Overrides Sub FillData()

            If (Me.UIContext Is Nothing) Then Return

            Dim stanza As cStanzaGroup = Nothing
            Dim group As cEcoPathGroupInput = Nothing
            Dim taxon As cTaxon = Nothing
            Dim hgcParent As EwEHierarchyGridCell = Nothing
            Dim iRow As Integer = -1
            Dim abStanzaHandled(Me.Core.nStanzas) As Boolean

            ' Remove existing rows
            Me.RowsCount = 1

            For iGroup As Integer = 1 To Me.Core.nGroups

                group = Me.Core.EcoPathGroupInputs(iGroup)
                If group.isMultiStanza Then

                    If Not abStanzaHandled(group.iStanza) Then
                        stanza = Me.Core.StanzaGroups(group.iStanza)
                        iRow = Me.AddRow()

                        hgcParent = New EwEHierarchyGridCell()
                        Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                        Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, stanza, eVarNameFlags.Name, Nothing, hgcParent)
                        For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                            Me(iRow, iCol) = New EwERowHeaderCell("")
                        Next

                        For iTaxon As Integer = 1 To Me.Core.nTaxon
                            taxon = Me.Core.Taxon(iTaxon)
                            If taxon.Stanza = stanza.Index Then
                                iRow += 1
                                Me.AddTaxonRow(taxon, iRow, hgcParent)
                            End If
                        Next
                        abStanzaHandled(group.iStanza) = True
                    End If

                Else
                    iRow = Me.AddRow()

                    hgcParent = New EwEHierarchyGridCell()
                    Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                    Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(String.Format(SharedResources.GENERIC_LABEL_INDEXED, group.Index, group.Name))
                    For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                        Me(iRow, iCol) = New EwERowHeaderCell("")
                    Next

                    For iTaxon As Integer = 1 To Me.Core.nTaxon
                        taxon = Me.Core.Taxon(iTaxon)
                        If taxon.Group = group.Index Then
                            iRow += 1
                            Me.AddTaxonRow(taxon, iRow, hgcParent)
                        End If
                    Next
                End If
            Next

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
                    taxon.IUCNConservationStatus = CType(cell.GetValue(p), eIUCNConservationStatusTypes)
                Case eColumnTypes.Ecology
                    taxon.EcologyType = CType(cell.GetValue(p), eEcologyTypes)
                Case eColumnTypes.PropCatch
                    taxon.ProportionCatch = CSng(cell.GetValue(p))
                Case eColumnTypes.Occurrence
                    taxon.OccurrenceStatus = CType(cell.GetValue(p), eOccurrenceStatusTypes)
                Case eColumnTypes.Organism
                    taxon.OrganismType = CType(cell.GetValue(p), eOrganismTypes)
                Case Else

            End Select
            Return MyBase.OnCellValueChanged(p, cell)
        End Function

        Private Sub AddTaxonRow(ByVal taxon As cTaxon, ByVal iRow As Integer, ByVal hgcParent As EwEHierarchyGridCell)

            Dim cell As EwECellBase = Nothing

            Me.Rows.Insert(iRow)
            Me(iRow, eColumnTypes.Hierarchy) = New PropertyRowHeaderCell(Me.PropertyManager, taxon, eVarNameFlags.Index)

            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderChildCell(Me.PropertyManager, taxon, eVarNameFlags.Name)
            Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)

            Me(iRow, eColumnTypes.Ecology) = New SourceGrid2.Cells.Real.Cell(taxon.EcologyType, Me.m_editorEcology)
            Me(iRow, eColumnTypes.Ecology).Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.Organism) = New SourceGrid2.Cells.Real.Cell(taxon.OrganismType, Me.m_editorOrganism)
            Me(iRow, eColumnTypes.Organism).Behaviors.Add(Me.EwEEditHandler)

            Me(iRow, eColumnTypes.Conservation) = New SourceGrid2.Cells.Real.Cell(taxon.IUCNConservationStatus, Me.m_editorConservation)
            Me(iRow, eColumnTypes.Conservation).Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.Occurrence) = New SourceGrid2.Cells.Real.Cell(taxon.OccurrenceStatus, Me.m_editorOccurrence)
            Me(iRow, eColumnTypes.Occurrence).Behaviors.Add(Me.EwEEditHandler)

            cell = New PropertyCell(Me.PropertyManager, taxon, eVarNameFlags.TaxonPropCatch)
            cell.SuppressZero = True
            Me(iRow, eColumnTypes.PropCatch) = cell

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
            Me(iRow, eColumnTypes.VulIndex) = cell

            hgcParent.AddChildRow(iRow)
            Me.Taxon(iRow) = taxon

        End Sub

    End Class

End Namespace
