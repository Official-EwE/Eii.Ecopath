Imports EwEUtils.Core

Public Class cTaxonDataStructures

    Private m_ecopathDS As cEcopathDataStructures = Nothing
    Private m_stanzaDS As cStanzaDatastructures = Nothing

    ''' <summary>Total number of taxonomy codes.</summary>
    Public NumTaxon As Integer = 0
    ''' <summary>Taxonomy code DBID (xNumTaxa).</summary>
    Public TaxonDBID() As Integer
    ''' <summary>Taxon assignments (xNumTaxa) -> iGroup / iStanza</summary>
    Public TaxonTarget() As Integer
    ''' <summary>Taxon assignment proportions (xNumTaxa)</summary>
    Public TaxonProp() As Single
    ''' <summary>Flag stating whether TaxonTarget(i) refers to a stanza (true) or a group (false)</summary>
    Public IsTaxonStanza() As Boolean
    ''' <summary>Taxonomy class names (xNumTaxa).</summary>
    Public TaxonClass() As String
    ''' <summary>Taxonomy order names (xNumTaxa).</summary>
    Public TaxonOrder() As String
    ''' <summary>Taxonomy family names (xNumTaxa).</summary>
    Public TaxonFamily() As String
    ''' <summary>Taxonomy genus names (xNumTaxa).</summary>
    Public TaxonGenus() As String
    ''' <summary>Taxonomy species names (xNumTaxa).</summary>
    Public TaxonSpecies() As String
    ''' <summary>Taxonomy (common) names (xNumTaxa).</summary>
    Public TaxonName() As String
    ''' <summary>Taxonomy ISCAAP codes (xNumTaxa).</summary>
    Public TaxonCodeISCAAP() As String
    ''' <summary>Taxonomy taxon names (xNumTaxa).</summary>
    Public TaxonCodeTaxon() As String
    ''' <summary>Taxonomy 3A names (xNumTaxa).</summary>
    Public TaxonCode3A() As String
    ''' <summary>Taxonomy source names where Taxon information was derived from (xNumTaxa).</summary>
    Public TaxonSource() As String
    ''' <summary>Taxonomy source keys to access Taxon information in <see cref="TaxonSource">a source</see>(xNumTaxa).</summary>
    Public TaxonSourceKey() As String
    ''' <summary>Taxonomy last updated dates (xNumTaxa) in julian day format.</summary>
    Public TaxonLastUpdated() As Double
    ''' <summary>Northern limit of taxon occurrence bounding box</summary>
    Public TaxonNorth() As Single
    ''' <summary>Southern limit of taxon occurrence bounding box</summary>
    Public TaxonSouth() As Single
    ''' <summary>Eastern limit of taxon occurrence bounding box</summary>
    Public TaxonEast() As Single
    ''' <summary>Western limit of taxon occurrence bounding box</summary>
    Public TaxonWest() As Single
    ''' <summary>Ecology types for taxa</summary>
    Public TaxonEcologyType() As eEcologyTypes
    ''' <summary>Organism types for taxa</summary>
    Public TaxonOrganismType() As eOrganismTypes
    ''' <summary>Taxa exploited status</summary>
    Public TaxonExploited() As Boolean
    ''' <summary>Taxa IUCN csonservation status</summary>
    Public TaxonIUCNConservationStatus() As eIUCNConservationStatusTypes
    ''' <summary>Taxa occurrence status</summary>
    Public TaxonOccurrenceStatus() As eOccurrenceStatusTypes
    Public TaxonMeanWeight() As Single
    Public TaxonMeanLength() As Single
    Public TaxonMaxLength() As Single
    Public TaxonMeanLifeSpan() As Single
    Public TaxonVulnerabilityIndex() As Integer

    ''' <summary>Group taxon index - may be used by model, initially designed for quick taxon access code.</summary>
    Private m_alGroupTaxa() As List(Of Integer)

    Public Sub New(ByVal ecopathDS As cEcopathDataStructures, ByVal stanzaDS As cStanzaDatastructures)
        Me.m_ecopathDS = ecopathDS
        Me.m_stanzaDS = stanzaDS
    End Sub

    Public Sub Clear()
        Me.NumTaxon = 0
    End Sub

    Public Sub RedimTaxon()

        ReDim Me.TaxonDBID(Me.NumTaxon)
        ReDim Me.TaxonTarget(Me.NumTaxon)
        ReDim Me.IsTaxonStanza(Me.NumTaxon)
        ReDim Me.TaxonProp(Me.NumTaxon)
        ReDim Me.TaxonClass(Me.NumTaxon)
        ReDim Me.TaxonCode3A(Me.NumTaxon)
        ReDim Me.TaxonCodeISCAAP(Me.NumTaxon)
        ReDim Me.TaxonCodeTaxon(Me.NumTaxon)
        ReDim Me.TaxonName(Me.NumTaxon)
        ReDim Me.TaxonFamily(Me.NumTaxon)
        ReDim Me.TaxonGenus(Me.NumTaxon)
        ReDim Me.TaxonOrder(Me.NumTaxon)
        ReDim Me.TaxonSourceKey(Me.NumTaxon)
        ReDim Me.TaxonSource(Me.NumTaxon)
        ReDim Me.TaxonSpecies(Me.NumTaxon)
        ReDim Me.TaxonNorth(Me.NumTaxon)
        ReDim Me.TaxonSouth(Me.NumTaxon)
        ReDim Me.TaxonEast(Me.NumTaxon)
        ReDim Me.TaxonWest(Me.NumTaxon)
        ReDim Me.TaxonEcologyType(Me.NumTaxon)
        ReDim Me.TaxonOrganismType(Me.NumTaxon)
        ReDim Me.TaxonExploited(Me.NumTaxon)
        ReDim Me.TaxonIUCNConservationStatus(Me.NumTaxon)
        ReDim Me.TaxonOccurrenceStatus(Me.NumTaxon)
        ReDim Me.TaxonMeanWeight(Me.NumTaxon)
        ReDim Me.TaxonMeanLength(Me.NumTaxon)
        ReDim Me.TaxonMaxLength(Me.NumTaxon)
        ReDim Me.TaxonMeanLifeSpan(Me.NumTaxon)
        ReDim Me.TaxonVulnerabilityIndex(Me.NumTaxon)
        ReDim Me.TaxonLastUpdated(Me.NumTaxon)

    End Sub

#Region " Taxon index "

    Public ReadOnly Property NumGroupTaxa(ByVal iGroup As Integer) As Integer
        Get
            If Me.m_alGroupTaxa Is Nothing Then Me.UpdateTaxonIndex()
            Try
                Return Me.m_alGroupTaxa(iGroup).Count
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            Return 0
        End Get
    End Property

    Public ReadOnly Property GroupTaxa(ByVal iGroup As Integer, ByVal iIndex As Integer) As Integer
        Get
            If Me.m_alGroupTaxa Is Nothing Then Me.UpdateTaxonIndex()
            Try
                Return Me.m_alGroupTaxa(iGroup)(iIndex)
            Catch ex As Exception
                Debug.Assert(False)
            End Try
            Return 0
        End Get
    End Property

    Public Sub UpdateTaxonIndex()

        ReDim Me.m_alGroupTaxa(Me.m_ecopathDS.NumGroups)
        For iGroup As Integer = 0 To Me.m_ecopathDS.NumGroups
            Me.m_alGroupTaxa(iGroup) = New List(Of Integer)
        Next

        For iTaxon As Integer = 1 To Me.NumTaxon
            If Me.IsTaxonStanza(iTaxon) Then
                Dim iStanza As Integer = Me.TaxonTarget(iTaxon)
                For iIndex As Integer = 1 To Me.m_stanzaDS.Nstanza(iStanza)
                    Dim iGroup As Integer = Me.m_stanzaDS.EcopathCode(iStanza, iIndex)
                    Me.m_alGroupTaxa(iGroup).Add(iTaxon)
                Next
            Else
                Dim iGroup As Integer = Me.TaxonTarget(iTaxon)
                Me.m_alGroupTaxa(iGroup).Add(iTaxon)
            End If
        Next

    End Sub

#End Region ' Taxon index

End Class
