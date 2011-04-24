#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin.Data
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports SourceGrid2

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Grid class implementing the Edit Group Taxon interface grid bit.
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class gridDefineTaxonomy
    Inherits EwEGrid

#Region " Privates "

    ''' <summary>List of active taxa.</summary>
    Private m_lTaxonInfo As New List(Of cTaxonInfo)
    ''' <summary>List of removed taxa.</summary>
    Private m_lTaxonInfoRemoved As New List(Of cTaxonInfo)

    ''' <summary>Search term for public use.</summary>
    Private m_tiSearch As ITaxonSearchData = Nothing
    ''' <summary>Internal item linked to the search term.</summary>
    Private m_tiSearchLinked As ITaxonSearchData = Nothing

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        Hierarchy = 0
        Name
        Proportion
        Species
        Genus
        Family
        Order
        [Class]
        Phylum
        Code
        Status
    End Enum

#End Region ' Privates

#Region " Private helper classes "

    Private Class cTaxonInfo
        Implements ITaxonSearchData
        Implements ITaxonDetailsData

#Region " Private vars "

        ' JS cannot hang on to a cTaxon because the core may reload taxa amidst applying.
        ' Private m_taxon As cTaxon = Nothing
        Private m_iDBIDTaxon As Integer = cCore.NULL_VALUE
        Private m_iTaxon As Integer = -1

        Private m_strCode3A As String = ""
        Private m_strCodeISSCAAP As String = ""
        Private m_strCodeTaxon As String = ""
        Private m_strPhylum As String = ""
        Private m_strClass As String = ""
        Private m_strOrder As String = ""
        Private m_strGenus As String = ""
        Private m_strFamily As String = ""
        Private m_strSpecies As String = ""
        Private m_strCommon As String = ""
        Private m_strSource As String = ""
        Private m_strKey As String = ""
        Private m_sNorth As Single = cCore.NULL_VALUE
        Private m_sSouth As Single = cCore.NULL_VALUE
        Private m_sWest As Single = cCore.NULL_VALUE
        Private m_sEast As Single = cCore.NULL_VALUE
        Private m_ecology As eEcologyTypes = eEcologyTypes.NotSet
        Private m_conservation As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet
        Private m_occurrence As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet
        Private m_organism As eOrganismTypes = eOrganismTypes.Fishes
        Private m_dLastUpdated As Double = cDateUtils.DateToJulian(Date.Now())
        Private m_sMaxLength As Single = cCore.NULL_VALUE
        Private m_sMeanLength As Single = cCore.NULL_VALUE
        Private m_sMeanLifespan As Single = cCore.NULL_VALUE
        Private m_sMeanWeight As Single = cCore.NULL_VALUE
        Private m_iVulnerabilityIndex As Integer = 0

        ''' <summary>Index of the ecopath group that this taxon contributes to.</summary>
        Private m_iGroup As Integer = 0
        ''' <summary>Index of the stanza configuration that this taxon contributes to.</summary>
        Private m_iStanza As Integer = 0
        Private m_sProportion As Single = 1.0!

        ''' <summary>The status of a Layer in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal group As cEcoPathGroupInput)
            Me.m_iGroup = group.Index
            Me.m_iStanza = 0
            Me.m_sProportion = 1.0!
            Me.m_strCommon = group.Name
            Me.m_status = eItemStatusTypes.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing stanza.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal stanza As cStanzaGroup)
            Me.m_iGroup = 0
            Me.m_iStanza = stanza.Index
            Me.m_sProportion = 1.0!
            Me.m_strCommon = stanza.Name
            Me.m_status = eItemStatusTypes.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an administrative unit for an existing taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal taxon As cTaxon)
            Me.m_iDBIDTaxon = CInt(taxon.GetVariable(eVarNameFlags.DBID))
            Me.m_iTaxon = taxon.Index
            Me.m_iGroup = taxon.Group
            Me.m_iStanza = taxon.Stanza
            Me.m_sProportion = taxon.Proportion
            Me.m_strCode3A = taxon.Code3A
            Me.m_strCodeISSCAAP = taxon.CodeISSCAAP
            Me.m_strCodeTaxon = taxon.CodeTaxon
            Me.m_strCommon = taxon.Name
            Me.m_strClass = taxon.Class
            Me.m_strOrder = taxon.Order
            Me.m_strFamily = taxon.Family
            Me.m_strGenus = taxon.Genus
            Me.m_strSpecies = taxon.Species
            Me.m_sNorth = taxon.North
            Me.m_sSouth = taxon.South
            Me.m_sEast = taxon.East
            Me.m_sWest = taxon.West
            Me.m_ecology = taxon.EcologyType
            Me.m_occurrence = taxon.OccurrenceStatus
            Me.m_organism = taxon.OrganismType
            Me.m_conservation = taxon.IUCNConservationStatus
            Me.m_sMeanLength = taxon.MeanLength
            Me.m_sMaxLength = taxon.MaxLength
            Me.m_sMeanWeight = taxon.MeanWeight
            Me.m_sMeanLifespan = taxon.MeanLifespan
            Me.m_dLastUpdated = taxon.LastUpdated
            Me.m_strSource = taxon.Source
            Me.m_strKey = taxon.SourceKey
            Me.m_status = eItemStatusTypes.Original
        End Sub

        Public Sub New(ByVal taxon As ITaxonSearchData)
            Me.Update(taxon)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update this unit with new Taxonomy data.
        ''' </summary>
        ''' <param name="taxon"></param>
        ''' -------------------------------------------------------------------
        Public Sub Update(ByVal taxon As ITaxonSearchData)
            Me.m_strCode3A = taxon.Code3A
            Me.m_strCodeISSCAAP = taxon.CodeISSCAAP
            Me.m_strCodeTaxon = taxon.CodeTaxon
            Me.m_strCommon = taxon.Common
            Me.m_strPhylum = taxon.Phylum
            Me.m_strClass = taxon.Class
            Me.m_strOrder = taxon.Order
            Me.m_strFamily = taxon.Family
            Me.m_strGenus = taxon.Genus
            Me.m_strSpecies = taxon.Species
            Me.m_sNorth = taxon.North
            Me.m_sSouth = taxon.South
            Me.m_sEast = taxon.East
            Me.m_sWest = taxon.West
            If TypeOf (taxon) Is ITaxonDetailsData Then
                Dim details As ITaxonDetailsData = DirectCast(taxon, ITaxonDetailsData)
                Me.m_ecology = details.EcologyType
                Me.m_occurrence = details.OccurrenceStatus
                Me.m_organism = details.OrganismType
                Me.m_conservation = details.IUCNConservationStatus
                Me.m_sMeanLength = details.MeanLength
                Me.m_sMaxLength = details.MaxLength
                Me.m_sMeanWeight = details.MeanWeight
                Me.m_sMeanLifespan = details.MeanLifespan
                Me.m_dLastUpdated = details.LastUpdated
            End If
            Me.m_strKey = taxon.SourceKey
            Me.m_strSource = taxon.Source
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the DBID of the <see cref="cTaxon">EwE Taxonomy code</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TaxonID() As Integer
            Get
                Return Me.m_iDBIDTaxon
            End Get
        End Property

        ReadOnly Property TaxonIndex() As Integer
            Get
                Return Me.m_iTaxon
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the Taxonomy code is to be created.
        ''' </summary>
        ''' <returns>
        ''' True when Layer <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.m_iDBIDTaxon = cCore.NULL_VALUE)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the administrative unit has changed.
        ''' </summary>
        ''' <returns>
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsChanged(ByVal taxon As cTaxon) As Boolean
            Get
                If (Me.IsNew()) Then Return False

                Debug.Assert(CInt(taxon.GetVariable(eVarNameFlags.DBID)) = Me.m_iDBIDTaxon)

                If (taxon.Proportion <> Me.m_sProportion) Then Return True
                If (taxon.Group <> Me.m_iGroup) Then Return True
                If (String.Compare(taxon.Name, Me.m_strCommon) <> 0) Then Return True
                If (String.Compare(taxon.Phylum, Me.m_strPhylum) <> 0) Then Return True
                If (String.Compare(taxon.Class, Me.m_strClass) <> 0) Then Return True
                If (String.Compare(taxon.Order, Me.m_strOrder) <> 0) Then Return True
                If (String.Compare(taxon.Family, Me.m_strFamily) <> 0) Then Return True
                If (String.Compare(taxon.Genus, Me.m_strGenus) <> 0) Then Return True
                If (String.Compare(taxon.Species, Me.m_strSpecies) <> 0) Then Return True
                If (String.Compare(taxon.Source, Me.m_strSource) <> 0) Then Return True
                If (String.Compare(taxon.CodeTaxon, Me.m_strCodeTaxon) <> 0) Then Return True
                If (String.Compare(taxon.CodeISSCAAP, Me.m_strCodeISSCAAP) <> 0) Then Return True
                If (String.Compare(taxon.Code3A, Me.m_strCode3A) <> 0) Then Return True
                Return False
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eItemStatusTypes">item status</see>
        ''' for the layer object.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Status() As eItemStatusTypes
            Get
                Return Me.m_status
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this layer is flagged for deletion. Toggling this flag
        ''' will update the <see cref="Status">Status</see> of the item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = eItemStatusTypes.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Not Me.IsNew Then
                    If bDelete Then
                        Me.m_status = eItemStatusTypes.Removed
                    Else
                        Me.m_status = eItemStatusTypes.Original
                    End If
                Else
                    If bDelete Then
                        Me.m_status = eItemStatusTypes.Invalid
                    Else
                        Me.m_status = eItemStatusTypes.Added
                    End If
                End If
            End Set
        End Property

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If (obj Is Nothing) Then Return False
            If (TypeOf obj Is cTaxon) Then
                Return DirectCast(obj, cTaxon).CodeTaxon = Me.CodeTaxon
            End If
            Return MyBase.Equals(obj)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group index of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Group() As Integer
            Get
                Return Me.m_iGroup
            End Get
            Set(ByVal value As Integer)
                Me.m_iGroup = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the stanza index of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Stanza() As Integer
            Get
                Return Me.m_iStanza
            End Get
            Set(ByVal value As Integer)
                Me.m_iStanza = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the proportion that this administrative unit contributes to
        ''' a functional group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Proportion() As Single
            Get
                Return Me.m_sProportion
            End Get
            Set(ByVal value As Single)
                Me.m_sProportion = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Phylum"/>
        ''' -------------------------------------------------------------------
        Public Property Phylum() As String _
            Implements ITaxonSearchData.Phylum
            Get
                Return Me.m_strPhylum
            End Get
            Set(ByVal value As String)
                Me.m_strPhylum = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.[Class]"/>
        ''' -------------------------------------------------------------------
        Public Property [Class]() As String _
            Implements ITaxonSearchData.Class
            Get
                Return m_strClass
            End Get
            Set(ByVal value As String)
                m_strClass = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Code3A"/>
        ''' -------------------------------------------------------------------
        Public Property Code3A() As String _
            Implements ITaxonSearchData.Code3A
            Get
                Return m_strCode3A
            End Get
            Set(ByVal value As String)
                m_strCode3A = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.CodeISSCAAP"/>
        ''' -------------------------------------------------------------------
        Public Property CodeISSCAAP() As String _
            Implements ITaxonSearchData.CodeISSCAAP
            Get
                Return m_strCodeISSCAAP
            End Get
            Set(ByVal value As String)
                Me.m_strCodeISSCAAP = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.CodeTaxon"/>
        ''' -------------------------------------------------------------------
        Public Property CodeTaxon() As String _
            Implements ITaxonSearchData.CodeTaxon
            Get
                Return Me.m_strCodeTaxon
            End Get
            Set(ByVal value As String)
                Me.m_strCodeTaxon = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Common"/>
        ''' -------------------------------------------------------------------
        Public Property Common() As String _
            Implements ITaxonSearchData.Common
            Get
                Return Me.m_strCommon
            End Get
            Set(ByVal value As String)
                Me.m_strCommon = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Family"/>
        ''' -------------------------------------------------------------------
        Public Property Family() As String _
            Implements ITaxonSearchData.Family
            Get
                Return Me.m_strFamily
            End Get
            Set(ByVal value As String)
                Me.m_strFamily = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Order"/>
        ''' -------------------------------------------------------------------
        Public Property Order() As String _
            Implements ITaxonSearchData.Order
            Get
                Return Me.m_strOrder
            End Get
            Set(ByVal value As String)
                Me.m_strOrder = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Genus"/>
        ''' -------------------------------------------------------------------
        Public Property Genus() As String _
            Implements ITaxonSearchData.Genus
            Get
                Return Me.m_strGenus
            End Get
            Set(ByVal value As String)
                Me.m_strGenus = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Source"/>
        ''' -------------------------------------------------------------------
        Public Property Source() As String _
            Implements ITaxonSearchData.Source
            Get
                Return Me.m_strSource
            End Get
            Set(ByVal value As String)
                Me.m_strSource = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.SourceKey"/>
        ''' -------------------------------------------------------------------
        Public Property SourceKey() As String _
            Implements ITaxonSearchData.SourceKey
            Get
                Return Me.m_strKey
            End Get
            Set(ByVal value As String)
                Me.m_strKey = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.Species"/>
        ''' -------------------------------------------------------------------
        Public Property Species() As String _
           Implements ITaxonSearchData.Species
            Get
                Return Me.m_strSpecies
            End Get
            Set(ByVal value As String)
                Me.m_strSpecies = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.North"/>
        ''' -------------------------------------------------------------------
        Public Property North() As Single _
            Implements ITaxonSearchData.North
            Get
                Return Me.m_sNorth
            End Get
            Set(ByVal value As Single)
                Me.m_sNorth = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.South"/>
        ''' -------------------------------------------------------------------
        Public Property South() As Single _
            Implements ITaxonSearchData.South
            Get
                Return Me.m_sSouth
            End Get
            Set(ByVal value As Single)
                Me.m_sSouth = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.East"/>
        ''' -------------------------------------------------------------------
        Public Property East() As Single _
            Implements ITaxonSearchData.East
            Get
                Return Me.m_sEast
            End Get
            Set(ByVal value As Single)
                Me.m_sEast = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonSearchData.West"/>
        ''' -------------------------------------------------------------------
        Public Property West() As Single _
            Implements ITaxonSearchData.West
            Get
                Return Me.m_sWest
            End Get
            Set(ByVal value As Single)
                Me.m_sWest = value
            End Set
        End Property

        Public Sub ApplyChanges(ByVal taxon As cTaxon)
            If Me.IsChanged(taxon) Then
                With taxon
                    .Name = Me.m_strCommon
                    .Group = Me.m_iGroup
                    .Proportion = Me.m_sProportion
                    .Code3A = Me.m_strCode3A
                    .CodeISSCAAP = Me.m_strCodeISSCAAP
                    .CodeTaxon = Me.m_strCodeTaxon
                    .Species = Me.m_strSpecies
                    .Family = Me.m_strFamily
                    .Genus = Me.m_strGenus
                    .Order = Me.m_strOrder
                    .Class = Me.m_strClass
                    .Source = Me.m_strSource
                    .SourceKey = Me.m_strKey
                    .North = Me.m_sNorth
                    .West = Me.m_sWest
                    .East = Me.m_sEast
                    .South = Me.m_sSouth
                    .EcologyType = Me.m_ecology
                    .IUCNConservationStatus = Me.m_conservation
                    .OrganismType = Me.m_organism
                    .OccurrenceStatus = Me.m_occurrence
                    .MaxLength = Me.m_sMaxLength
                    .MeanLength = Me.m_sMeanLength
                    .MeanWeight = Me.m_sMeanWeight
                    .MeanLifespan = Me.m_sMeanLifespan
                    .LastUpdated = cDateUtils.DateToJulian()
                End With
            End If
        End Sub

        ''' <inheritdocs cref="ITaxonDetailsData.EcologyType"/>
        Public Property EcologyType() As eEcologyTypes _
            Implements ITaxonDetailsData.EcologyType
            Get
                Return Me.m_ecology
            End Get
            Set(ByVal value As eEcologyTypes)
                Me.m_ecology = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.IUCNConservationStatus"/>
        Public Property IUCNConservationStatus() As eIUCNConservationStatusTypes _
            Implements ITaxonDetailsData.IUCNConservationStatus
            Get
                Return Me.m_conservation
            End Get
            Set(ByVal value As eIUCNConservationStatusTypes)
                Me.m_conservation = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.LastUpdated"/>
        Public Property LastUpdated() As Double _
            Implements ITaxonDetailsData.LastUpdated
            Get
                Return Me.m_dLastUpdated
            End Get
            Set(ByVal value As Double)
                Me.m_dLastUpdated = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.MaxLength"/>
        Public Property MaxLength() As Single _
            Implements ITaxonDetailsData.MaxLength
            Get
                Return Me.m_sMaxLength
            End Get
            Set(ByVal value As Single)
                Me.m_sMaxLength = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.MeanLength"/>
        Public Property MeanLength() As Single _
            Implements ITaxonDetailsData.MeanLength
            Get
                Return Me.m_sMeanLength
            End Get
            Set(ByVal value As Single)
                Me.m_sMeanLength = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.MeanLifespan"/>
        Public Property MeanLifespan() As Single _
            Implements ITaxonDetailsData.MeanLifespan
            Get
                Return Me.m_sMeanLifespan
            End Get
            Set(ByVal value As Single)
                Me.m_sMeanLifespan = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.MeanWeight"/>
        Public Property MeanWeight() As Single _
            Implements ITaxonDetailsData.MeanWeight
            Get
                Return Me.m_sMeanWeight
            End Get
            Set(ByVal value As Single)
                Me.m_sMeanWeight = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.OccurrenceStatus"/>
        Public Property OccurrenceStatus() As eOccurrenceStatusTypes _
            Implements ITaxonDetailsData.OccurrenceStatus
            Get
                Return Me.m_occurrence
            End Get
            Set(ByVal value As eOccurrenceStatusTypes)
                Me.m_occurrence = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.OrganismType"/>
        Public Property OrganismType() As eOrganismTypes _
            Implements ITaxonDetailsData.OrganismType
            Get
                Return Me.m_organism
            End Get
            Set(ByVal value As eOrganismTypes)
                Me.m_organism = value
            End Set
        End Property

        ''' <inheritdocs cref="ITaxonDetailsData.VulnerabilityIndex"/>
        Public Property VulnerabilityIndex() As Integer _
            Implements ITaxonDetailsData.VulnerabilityIndex
            Get
                Return Me.m_iVulnerabilityIndex
            End Get
            Set(ByVal value As Integer)
                Me.m_iVulnerabilityIndex = value
            End Set
        End Property

    End Class

#End Region ' Private helper classes

#Region " Constructor "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        MyBase.New()

    End Sub

#End Region ' Constructor

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.Selection.EnableMultiSelection = False

        ' Redim columns
        Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

        ' Group index cell
        Me(0, eColumnTypes.Hierarchy) = New EwEColumnHeaderCell()
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
        Me(0, eColumnTypes.Proportion) = New EwEColumnHeaderCell(SharedResources.HEADER_PROPORTION_B)
        Me(0, eColumnTypes.Phylum) = New EwEColumnHeaderCell(SharedResources.HEADER_PHYLUM)
        Me(0, eColumnTypes.Class) = New EwEColumnHeaderCell(SharedResources.HEADER_CLASS)
        Me(0, eColumnTypes.Order) = New EwEColumnHeaderCell(SharedResources.HEADER_ORDER)
        Me(0, eColumnTypes.Family) = New EwEColumnHeaderCell(SharedResources.HEADER_FAMILY)
        Me(0, eColumnTypes.Genus) = New EwEColumnHeaderCell(SharedResources.HEADER_GENUS)
        Me(0, eColumnTypes.Species) = New EwEColumnHeaderCell(SharedResources.HEADER_SPECIES)
        Me(0, eColumnTypes.Code) = New EwEColumnHeaderCell(SharedResources.HEADER_CODE)
        Me(0, eColumnTypes.Status) = New EwEColumnHeaderCell(SharedResources.HEADER_STATUS)

        Me.FixedColumns = 1
        Me.FixedColumnWidths = False

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to first create a snapshot of the group taxon configuration
    ''' in the current EwE model. The grid will be populated from this local
    ''' administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        Dim stz As cStanzaGroup = Nothing
        Dim grp As cEcoPathGroupInput = Nothing
        Dim abStanzaHandled(Me.Core.nStanzas) As Boolean
        Dim iRow As Integer = 0
        Dim hgcParent As EwEHierarchyGridCell = Nothing
        Dim taxon As cTaxon = Nothing
        Dim ti As cTaxonInfo = Nothing

        ' Populate local administration from a snapshot of the live data

        ' Make snapshot of configuration 
        For iTaxon As Integer = 1 To Me.Core.nTaxon
            taxon = Me.Core.Taxon(iTaxon)
            ti = New cTaxonInfo(taxon)
            Me.m_lTaxonInfo.Add(ti)
        Next

        Me.NormalizeProportions()

        ' Create rows
        Me.RowsCount = 1

        For iGroup As Integer = 1 To Me.Core.nGroups

            grp = Me.Core.EcoPathGroupInputs(iGroup)
            If grp.isMultiStanza Then

                If Not abStanzaHandled(grp.iStanza) Then
                    iRow = Me.AddRow()
                    stz = Me.Core.StanzaGroups(grp.iStanza)

                    hgcParent = New EwEHierarchyGridCell()
                    hgcParent.Tag = stz

                    Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                    Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderParentCell(Me.PropertyManager, stz, eVarNameFlags.Name, Nothing, hgcParent)
                    For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                        Me(iRow, iCol) = New EwERowHeaderCell("")
                    Next

                    For iTaxon As Integer = 0 To Me.m_lTaxonInfo.Count - 1
                        ti = Me.m_lTaxonInfo(iTaxon)
                        If ti.Stanza = stz.Index Then
                            Me.AddTaxonRow(ti, iRow)
                        End If
                    Next
                    abStanzaHandled(grp.iStanza) = True

                End If
            Else

                iRow = Me.AddRow()

                hgcParent = New EwEHierarchyGridCell()
                hgcParent.Tag = grp

                Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(String.Format(SharedResources.GENERIC_LABEL_INDEXED, grp.Index, grp.Name))
                For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                    Me(iRow, iCol) = New EwERowHeaderCell("")
                Next

                For iTaxon As Integer = 0 To Me.m_lTaxonInfo.Count - 1
                    ti = Me.m_lTaxonInfo(iTaxon)
                    If ti.Group = grp.Index Then
                        Me.AddTaxonRow(ti, iRow)
                    End If
                Next
            End If
        Next

        ' Populate rows
        For iRow = 1 To Me.RowsCount - 1
            Me.UpdateRow(iRow)
        Next iRow
    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        For iCol As Integer = 1 To Me.ColumnsCount - 1
            Select Case DirectCast(iCol, eColumnTypes)
                Case eColumnTypes.Hierarchy
                    Me.Columns(iCol).Width = 20
                Case Else
                    Me.Columns(iCol).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                    Me.AutoSizeColumn(iCol, 80)
            End Select
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; called when a cell has received focus. Overriden to notify
    ''' our parent that the selection has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnCellGotFocus(ByVal e As SourceGrid2.PositionCancelEventArgs)
        MyBase.OnCellGotFocus(e)
        Me.RaiseSelectionChangeEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; called when a cell has lost focus. Overriden to notify
    ''' our parent that the selection has changed.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub OnCellLostFocus(ByVal e As SourceGrid2.PositionCancelEventArgs)
        MyBase.OnCellLostFocus(e)
        Me.Selection.Clear()
        Me.RaiseSelectionChangeEvent()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)

        Dim ti As cTaxonInfo = Me.TaxonInfo(iRow)
        Dim vm As VisualModels.IVisualModel = Nothing
        Dim dt As Date = Nothing
        Dim strText As String = ""
        Dim iNumOpen As Integer = 0

        If ti Is Nothing Then Return

        Me(iRow, eColumnTypes.Name).Value = ti.Common
        Me(iRow, eColumnTypes.Class).Value = ti.Class
        Me(iRow, eColumnTypes.Order).Value = ti.Order
        Me(iRow, eColumnTypes.Family).Value = ti.Family
        Me(iRow, eColumnTypes.Genus).Value = ti.Genus
        Me(iRow, eColumnTypes.Species).Value = ti.Species
        Me(iRow, eColumnTypes.Proportion).Value = ti.Proportion

        Select Case ti.Status
            Case eItemStatusTypes.Original
                vm = Me.DefaultVisualOriginal
                strText = ""
            Case eItemStatusTypes.Added
                vm = Me.DefaultVisualAdded
                strText = My.Resources.GENERIC_ITEMSTATUS_CREATEPENDING
            Case eItemStatusTypes.Removed
                vm = Me.DefaultVisualRemoved
                strText = My.Resources.GENERIC_ITEMSTATUS_DELETEPENDING
        End Select

        Me(iRow, eColumnTypes.Status).VisualModel = vm
        Me(iRow, eColumnTypes.Status).Value = strText

    End Sub

    Private Function FindParentRow(ByVal iRow As Integer) As Integer
        If iRow < 1 Then Return -1
        While (iRow > 0) And Not (TypeOf Me(iRow, eColumnTypes.Hierarchy) Is EwEHierarchyGridCell)
            iRow -= 1
        End While
        Return iRow
    End Function

    Private Function AddTaxonRow(ByVal ti As cTaxonInfo, Optional ByVal iRow As Integer = -1) As Integer

        Dim cell As EwECell = Nothing

        If iRow = -1 Then
            iRow = Me.FindParentRow(Me.SelectedRow)
        End If

        Dim hgcParent As EwEHierarchyGridCell = DirectCast(Me(iRow, eColumnTypes.Hierarchy), EwEHierarchyGridCell)
        iRow += 1
        Me.Rows.Insert(iRow)
        Me(iRow, eColumnTypes.Hierarchy) = New EwERowHeaderCell()
        Me(iRow, eColumnTypes.Hierarchy).Tag = ti
        Me(iRow, eColumnTypes.Name) = New EwECell(ti.Common, GetType(String))
        Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Species) = New EwECell(ti.Species, GetType(String), cStyleGuide.eStyleFlags.TaxonItalics)
        Me(iRow, eColumnTypes.Species).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Genus) = New EwECell(ti.Genus, GetType(String), cStyleGuide.eStyleFlags.TaxonItalics)
        Me(iRow, eColumnTypes.Genus).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Family) = New EwECell(ti.Family, GetType(String))
        Me(iRow, eColumnTypes.Family).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Order) = New EwECell(ti.Order, GetType(String))
        Me(iRow, eColumnTypes.Order).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Class) = New EwECell(ti.Class, GetType(String))
        Me(iRow, eColumnTypes.Class).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Phylum) = New EwECell(ti.Phylum, GetType(String))
        Me(iRow, eColumnTypes.Phylum).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Proportion) = New EwECell(ti.Proportion, GetType(Single))
        Me(iRow, eColumnTypes.Proportion).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Status) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)

        ' == CODE cell
        ' Allow custom taxon code when not obtained from external source, e.g. when no source key provided
        If String.IsNullOrEmpty(ti.SourceKey) Then
            cell = New EwECell(ti.CodeTaxon, GetType(String))
            cell.Behaviors.Add(Me.EwEEditHandler)
        Else
            cell = New EwECell(ti.CodeTaxon, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
        End If
        Me(iRow, eColumnTypes.Code) = cell

        hgcParent.AddChildRow(iRow)
        Me.UpdateRow(iRow)

    End Function

    Private Sub RemoveTaxonRow(ByVal iRow As Integer)
        If iRow <= 0 Then iRow = Me.SelectedRow
        Dim iRowParent As Integer = Me.FindParentRow(iRow)
        If iRowParent >= 1 Then
            Dim hgcParent As EwEHierarchyGridCell = DirectCast(Me(iRowParent, eColumnTypes.Hierarchy), EwEHierarchyGridCell)
            hgcParent.RemoveChildRow(iRow)
            Me.Rows.Remove(iRow)
        End If
    End Sub

    Public Sub UpdateProportions()
        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim ti As cTaxonInfo = Me.TaxonInfo(iRow)
            If ti IsNot Nothing Then
                Me(iRow, eColumnTypes.Proportion).Value = ti.Proportion
            End If
        Next
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Called when the user has finished editing a cell. Handled to update 
    ''' local admin based on cell value changes.
    ''' </summary>
    ''' <returns>
    ''' True if the edit operation is allowed, False to cancel the edit operation.
    ''' </returns>
    ''' <remarks>
    ''' This method differs from OnCellValueChanged; at the end of an edit
    ''' operation it is once again safe to alter the value of the cell that was
    ''' just edited for text and combo box controls. *sigh*
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Dim ti As cTaxonInfo = Me.TaxonInfo(p.Row)
        If ti Is Nothing Then Return False

        Dim val As Object = Me(p.Row, p.Column).Value

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.Name : ti.Common = CStr(val)
            Case eColumnTypes.Class : ti.Class = CStr(val)
            Case eColumnTypes.Family : ti.Family = CStr(val)
            Case eColumnTypes.Order : ti.Order = CStr(val)
            Case eColumnTypes.Genus : ti.Genus = CStr(val)
            Case eColumnTypes.Species : ti.Species = CStr(val)
            Case eColumnTypes.Phylum : ti.Phylum = CStr(val)
            Case eColumnTypes.Proportion : ti.Proportion = CSng(val)
            Case eColumnTypes.Code : ti.CodeTaxon = CStr(val)
        End Select

        ' Perhaps redundant but hey
        Me.UpdateRow(p.Row)

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, obtains the taxon info for a given row.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns>A cTaxonInfo instance, or nothing if the row did not contain
    ''' a taxoninfo link.</returns>
    ''' -----------------------------------------------------------------------
    Private Function TaxonInfo(ByVal iRow As Integer) As cTaxonInfo
        Dim tag As Object = Nothing
        If (iRow <= 1) Then Return Nothing
        tag = Me(iRow, eColumnTypes.Hierarchy).Tag
        If Not (TypeOf tag Is cTaxonInfo) Then Return Nothing
        Return DirectCast(tag, cTaxonInfo)
    End Function

#End Region ' Internals

#Region " Public bits "

#Region " Data "

    Public Property SelectedTaxon() As ITaxonSearchData
        Get
            Return Me.TaxonInfo(Me.SelectedRow)
        End Get
        Set(ByVal taxon As ITaxonSearchData)
            If Not (TypeOf taxon Is cTaxonInfo) Then Return
            For iRow As Integer = 1 To Me.RowsCount - 1
                If Object.ReferenceEquals(TaxonInfo(iRow), taxon) Then
                    Me.SelectRow(iRow)
                    Return
                End If
            Next
        End Set
    End Property

    Public ReadOnly Property SelectedGroup() As cEcoPathGroupInput
        Get
            Dim iRowParent As Integer = Me.FindParentRow(Me.SelectedRow)
            Dim tag As Object = Nothing

            If (iRowParent < 1) Then Return Nothing

            tag = Me(iRowParent, eColumnTypes.Hierarchy).Tag
            If (TypeOf tag Is cEcoPathGroupInput) Then
                Return DirectCast(tag, cEcoPathGroupInput)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property SelectedStanza() As cStanzaGroup
        Get
            Dim iRowParent As Integer = Me.FindParentRow(Me.SelectedRow)
            Dim tag As Object = Nothing

            If (iRowParent < 1) Then Return Nothing

            tag = Me(iRowParent, eColumnTypes.Hierarchy).Tag
            If (TypeOf tag Is cStanzaGroup) Then
                Return DirectCast(tag, cStanzaGroup)
            End If
            Return Nothing
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of all available taxa.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Taxa() As ITaxonSearchData()
        Get
            Return Me.m_lTaxonInfo.ToArray
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a taxon for the selected group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub AddTaxon(Optional ByVal taxon As ITaxonSearchData = Nothing)

        If Not Me.CanAddTaxon(taxon) Then Return

        Dim ti As cTaxonInfo = Nothing
        Dim iRow As Integer = Nothing
        Dim grp As cEcoPathGroupInput = Me.SelectedGroup
        Dim stz As cStanzaGroup = Me.SelectedStanza

        If (taxon Is Nothing) Then
            If (grp Is Nothing) Then
                ti = New cTaxonInfo(stz)
            Else
                ti = New cTaxonInfo(grp)
            End If
            Me.m_lTaxonInfo.Add(ti)
        Else
            ti = New cTaxonInfo(taxon)
            ti.Group = Me.SelectedGroup.Index
            Me.m_lTaxonInfo.Add(ti)
        End If

        Me.AddTaxonRow(ti)
        Me.SelectedTaxon = ti

    End Sub

    ''' <summary>
    ''' States whether a taxon can be added to the current selected row.
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' <para>The following rules are checked:</para>
    ''' <list type="bullet">
    ''' <item><description>A row must be selected</description></item>
    ''' <item><description>A stanza group can have only ONE taxon assigned</description></item>
    ''' <item><description>A taxon code can be used multiple times</description></item>
    ''' </list>
    ''' </remarks>
    Public Function CanAddTaxon(Optional ByVal taxon As ITaxonSearchData = Nothing) As Boolean

        Dim grp As cEcoPathGroupInput = Me.SelectedGroup
        Dim stz As cStanzaGroup = Me.SelectedStanza

        Dim bIsTaxonUsed As Boolean = False
        Dim bStanzaHasTaxon As Boolean = False

        If (grp Is Nothing) And (stz Is Nothing) Then Return False

        For Each ti As cTaxonInfo In Me.m_lTaxonInfo
            'bIsTaxonUsed = bIsTaxonUsed Or (ti.Equals(taxon))
            If (stz IsNot Nothing) Then
                bStanzaHasTaxon = bStanzaHasTaxon Or (stz.Index = ti.Stanza)
            End If
        Next

        Return (Not bIsTaxonUsed) And (Not bStanzaHasTaxon)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the delete state of all selected rows
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ToggleDeleteRow()

        Dim sel As Selection = Me.Selection
        Dim ti As cTaxonInfo = Nothing

        Dim iRow As Integer = Me.SelectedRow
        ti = Me.TaxonInfo(iRow)

        If (ti IsNot Nothing) Then
            ti.FlaggedForDeletion = Not ti.FlaggedForDeletion

            ' Check to see what is to happen to the MPA now
            Select Case ti.Status

                Case eItemStatusTypes.Original
                    ' Clear removed status 
                    Me.m_lTaxonInfoRemoved.Remove(ti)
                    Me.UpdateRow(iRow)

                Case eItemStatusTypes.Added
                    ' Remove new item
                    Me.m_lTaxonInfo.Remove(ti)
                    Me.RemoveTaxonRow(iRow)

                Case eItemStatusTypes.Removed
                    ' Set removed status
                    Me.m_lTaxonInfoRemoved.Add(ti)
                    Me.UpdateRow(iRow)

                Case eItemStatusTypes.Invalid
                    ' Set removed status
                    Me.m_lTaxonInfo.Remove(ti)
                    Me.RemoveTaxonRow(iRow)

            End Select

        End If

    End Sub

    Public Function CanDeleteTaxon() As Boolean
        Return (Me.SelectedTaxon IsNot Nothing)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether the taxon info row is flagged for deletion.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Function IsFlaggedForDeletionRow() As Boolean
        Dim ti As cTaxonInfo = Me.TaxonInfo(Me.SelectedRow)
        If (ti Is Nothing) Then Return False
        Return ti.FlaggedForDeletion
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the grid row for the current selected taxon.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateSelectedTaxonRow()
        Me.UpdateRow(Me.SelectedRow())
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Populate the selected taxon with new data.
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' -----------------------------------------------------------------------
    Public Sub UpdateSelectedTaxon(ByVal taxon As ITaxonSearchData)
        Dim ti As cTaxonInfo = Me.TaxonInfo(Me.SelectedRow)
        If (ti Is Nothing) Then Return
        ti.Update(taxon)
    End Sub

#End Region ' Data

#Region " Search "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns a search term for the current selected taxon.
    ''' </summary>
    ''' <param name="taxonSearch">Taxon to create a search term for.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function GetSearchTerm(Optional ByVal taxonSearch As ITaxonSearchData = Nothing) As ITaxonSearchData

        Me.m_tiSearchLinked = Me.SelectedTaxon

        If taxonSearch Is Nothing Then taxonSearch = Me.m_tiSearchLinked
        Me.m_tiSearch = New cTaxonInfo(taxonSearch)

        Return Me.m_tiSearch

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a specific taxon is the last created search term.
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function IsSearchTerm(ByVal taxon As ITaxonSearchData) As Boolean
        Return (Object.ReferenceEquals(taxon, Me.m_tiSearch)) And _
               (Object.ReferenceEquals(Me.SelectedTaxon, Me.m_tiSearchLinked))
    End Function

#End Region ' Search

#Region " Apply changes "

    Public Sub NormalizeProportions()

        Dim asTotalGroup(Me.Core.nGroups) As Single
        Dim aiTotalGroup(Me.Core.nGroups) As Integer
        Dim asTotalTaxon(Me.Core.nStanzas) As Single
        Dim aiTotalTaxon(Me.Core.nStanzas) As Integer

        Dim ti As cTaxonInfo = Nothing
        Dim iTaxon As Integer = 0

        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = Me.m_lTaxonInfo(iTaxon)
            If (ti.Status <> eItemStatusTypes.Removed) Then
                If ti.Stanza > 0 Then
                    asTotalTaxon(ti.Stanza) += ti.Proportion
                    aiTotalTaxon(ti.Stanza) += 1
                Else
                    asTotalGroup(ti.Group) += ti.Proportion
                    aiTotalGroup(ti.Group) += 1
                End If
            End If
        Next

        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = Me.m_lTaxonInfo(iTaxon)
            If (ti.Status <> eItemStatusTypes.Removed) Then
                If ti.Stanza > 0 Then
                    ' Has a total of 0?
                    If (asTotalTaxon(ti.Stanza) = 0.0!) Then
                        ' #Yes: redistribute values
                        ti.Proportion = 1.0! / aiTotalTaxon(ti.Stanza)
                    Else
                        ti.Proportion = ti.Proportion / asTotalTaxon(ti.Stanza)
                    End If
                Else
                    ' Has a total of 0?
                    If (asTotalGroup(ti.Group) = 0.0!) Then
                        ' #Yes: redistribute values
                        ti.Proportion = 1.0! / aiTotalGroup(ti.Group)
                    Else
                        ti.Proportion = ti.Proportion / asTotalGroup(ti.Group)
                    End If
                End If
            End If
        Next
        Me.UpdateProportions()

    End Sub

    Public Function Apply() As Boolean

        Dim bConfigurationChanged As Boolean = False
        Dim ti As cTaxonInfo = Nothing
        Dim taxon As cTaxon = Nothing
        Dim iTaxon As Integer = 0
        Dim bSuccess As Boolean = True

        ' Assess Taxon changes
        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = DirectCast(Me.m_lTaxonInfo(iTaxon), cTaxonInfo)
            ' Check this Taxon is newly added
            If ti.IsNew Then
                bConfigurationChanged = True
                Exit For
            Else
                ' Check if this Taxon is an existing Taxon that has been moved
                If ((iTaxon + 1) <> ti.TaxonIndex) Then
                    bConfigurationChanged = True
                    Exit For
                End If
            End If
        Next iTaxon

        ' Assess Taxons to remove
        If Me.m_lTaxonInfoRemoved.Count > 0 Then
            Select Case MsgBox(My.Resources.TAXON_DELETE_CONFIRMATION, MsgBoxStyle.Question Or MsgBoxStyle.YesNo)
                Case MsgBoxResult.No
                    ' Abort
                    Return False
                Case MsgBoxResult.Yes
                    ' Delete this Taxon
                    bConfigurationChanged = True
                Case Else
                    ' Unexpected anwer: assert
                    Debug.Assert(False)
            End Select
        End If

        ' Handle added and removed items
        If (bConfigurationChanged) Then

            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

            cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.GENERIC_STATUS_APPLYCHANGES)

            Dim htTaxonID As New Dictionary(Of cTaxonInfo, Integer)
            Dim iDBID As Integer = Nothing

            Try

                ' Add new Taxons
                For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
                    ti = Me.m_lTaxonInfo(iTaxon)
                    If (ti.IsNew) Then
                        bSuccess = bSuccess And Me.Core.AddTaxon(Math.Max(ti.Group, ti.Stanza), (ti.Stanza > 0), ti, ti.Proportion, iDBID)
                        ' Map this new ID during update
                        htTaxonID.Add(ti, iDBID)
                    End If
                Next

                ' Remove deleted Taxons
                Dim iTaxonRemove As Integer = 0
                For iTaxon = 0 To Me.m_lTaxonInfoRemoved.Count - 1
                    ti = DirectCast(Me.m_lTaxonInfoRemoved(iTaxonRemove), cTaxonInfo)
                    If (Not ti.IsNew) Then
                        If (Me.Core.RemoveTaxon(ti.TaxonIndex)) Then
                            Me.m_lTaxonInfo.Remove(ti)
                            Me.m_lTaxonInfoRemoved.Remove(ti)
                        Else
                            bSuccess = False
                            iTaxonRemove += 1
                        End If
                    End If
                Next

            Catch ex As Exception

            End Try

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.EndProgress(Me.Core)

            ' Test whether new Taxons were loaded correctly
            Debug.Assert(Me.m_lTaxonInfo.Count = Me.Core.nTaxon, "Dialog and core out of sync on Taxons")
        End If

        ' Update any changed taxa
        Dim dtTaxa As New Dictionary(Of Integer, cTaxon)
        For i As Integer = 1 To Me.Core.nTaxon
            taxon = Me.Core.Taxon(i)
            dtTaxa(CInt(taxon.GetVariable(eVarNameFlags.DBID))) = taxon
        Next

        For Each ti In Me.m_lTaxonInfo
            If Not ti.IsNew Then ti.ApplyChanges(dtTaxa(ti.TaxonID))
        Next

        Return bSuccess

    End Function

#End Region ' Apply changes

#End Region ' Public bits

End Class
