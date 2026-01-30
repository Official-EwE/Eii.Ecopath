' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwECore.Common
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.IO
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources



''' -----------------------------------------------------------------------
''' <summary>
''' Grid class implementing the Edit Group Taxon interface grid bit.
''' </summary>
''' -----------------------------------------------------------------------

Public Class gridDefineTaxonomy
    Inherits cEwEGrid

#Region " Privates "

    Private m_bShowCodes As Boolean = False

    ''' <summary>List of active taxa.</summary>
    Private m_lTaxonInfo As New List(Of cTaxonInfo)
    ''' <summary>List of removed taxa.</summary>
    Private m_lTaxonInfoRemoved As New List(Of cTaxonInfo)

    ''' <summary>Search term for public use.</summary>
    Private m_tiSearch As ITaxonSearchData = Nothing
    ''' <summary>Internal item linked to the search term.</summary>
    Private m_tiSearchLinked As ITaxonSearchData = Nothing

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    ''' <remarks>The logic that shows and hides code columns depends on the position 
    ''' of the status column, which is presumed to reside before the code columns.
    ''' Please do not alter the position of the status and code columns.</remarks>
    Private Enum eColumnTypes
        Hierarchy = 0
        Name
        PropB
        PropC
        Genus
        Species
        Family
        Order
        [Class]
        Phylum
        Status
        ' - Codes -
        CodeFishBase
        CodeSeaLifeBase
        CodeAquaMaps
        CodeOBIS
        CodeAphia
        CodeSAUP
        CodeFAO
        CodeLSID
    End Enum

#End Region ' Privates

#Region " Private helper classes "

    Private Class cTaxonInfo
        Inherits cTaxonSearchData
        Implements ITaxonSearchData
        Implements ITaxonDetailsData

#Region " Private vars "

        ''' <summary>The status of a Layer in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal group As cEcoPathGroupInput)
            MyBase.New("")
            Me.iGroup = group.Index
            Me.iStanza = 0
            Me.Common = group.Name
            Me.m_status = eItemStatusTypes.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing stanza.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal stanza As cStanzaGroup)
            MyBase.New("")
            Me.iGroup = 0
            Me.iStanza = stanza.Index
            Me.Common = stanza.Name
            Me.m_status = eItemStatusTypes.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an administrative unit for an existing taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal taxon As cTaxon)
            MyBase.New(taxon.Source)
            Me.TaxonID = CInt(taxon.GetVariable(eVarNameFlags.DBID))
            Me.TaxonIndex = taxon.Index
            Me.iGroup = taxon.iGroup
            Me.iStanza = taxon.iStanza
            Me.PropB = taxon.PropB
            Me.PropC = taxon.PropC
            Me.CodeSAUP = taxon.CodeSAUP
            Me.CodeFishBase = taxon.CodeFishBase
            Me.CodeSeaLifeBase = taxon.CodeSeaLifeBase
            Me.CodeAquaMaps = taxon.CodeAquaMaps
            Me.CodeOBIS = taxon.CodeOBIS
            Me.CodeAphia = taxon.CodeAphia
            Me.CodeFAO = taxon.CodeFAO
            Me.CodeLSID = taxon.CodeLSID
            Me.Common = taxon.Name
            Me.Class = taxon.Class
            Me.Order = taxon.Order
            Me.Family = taxon.Family
            Me.Genus = taxon.Genus
            Me.Species = taxon.Species
            Me.North = taxon.North
            Me.South = taxon.South
            Me.East = taxon.East
            Me.West = taxon.West
            Me.EcologyType = taxon.EcologyType
            Me.OccurrenceStatus = taxon.OccurrenceStatus
            Me.OrganismType = taxon.OrganismType
            Me.IUCNConservationStatus = taxon.IUCNConservationStatus
            Me.MeanLength = taxon.MeanLength
            Me.MaxLength = taxon.MaxLength
            Me.MeanWeight = taxon.MeanWeight
            Me.MeanLifespan = taxon.MeanLifespan
            Me.LastUpdated = taxon.LastUpdated
            Me.SourceKey = taxon.SourceKey
            Me.SearchFields = taxon.SearchFields
            Me.m_status = eItemStatusTypes.Original
        End Sub

        Public Sub New(ByVal taxon As ITaxonSearchData)
            MyBase.New(taxon.Source)
            Me.Update(taxon)
        End Sub

        ''' <summary>
        ''' Via serialization
        ''' </summary>
        Public Sub New()
            MyBase.New("")
        End Sub

#Region " Serialization "

        Protected Property Group As String = ""
        Protected Property Stanza As String = ""

        Protected Property Name As String
            Get
                Return Me.Genus & " " & Me.Species
            End Get
            Set(value As String)
                If value.Contains(" ") Then
                    Dim bits As String() = value.Split(" "c)
                    Me.Genus = bits(0)
                    Me.Species = bits(1)
                Else
                    Me.Genus = value
                    Me.Species = ""
                End If
            End Set
        End Property

        Private Shared separators As Char() = {"-"c, "/"c}

        Protected Property Ecology As String
            Get
                Return Me.EcologyType.ToString()
            End Get
            Set(value As String)
                Try
                    Dim e As eEcologyTypes = eEcologyTypes.NotSet
                    If Not String.IsNullOrWhiteSpace(value) Then
                        For Each c As Char In separators
                            If value.Contains(c) Then
                                value = value.Split(c)(0).Trim
                                ' Simplifying value
                            End If
                        Next
                        Select Case value.ToLower
                            Case "reef" : e = eEcologyTypes.ReefAssociated
                            Case Else
                                [Enum].TryParse(value.Replace(" ", ""), e)
                        End Select
                    End If
                    Me.EcologyType = e
                Catch ex As Exception
                    Debug.Assert(False, value & " is not a recognized ecology type")
                    Me.EcologyType = eEcologyTypes.NotSet
                End Try
            End Set
        End Property

        Protected Property Organism As String
            Get
                Return Me.OrganismType.ToString()
            End Get
            Set(value As String)
                Dim s As eOrganismTypes = eOrganismTypes.NotSet
                If Not String.IsNullOrWhiteSpace(value) Then
                    If Not [Enum].TryParse(value.Replace(" ", ""), s) Then
                        ' ToDo: find partial matches
                    End If
                End If
                Me.OrganismType = CType(s, eOrganismTypes)
            End Set
        End Property

        Protected Property Occurrence As String
            Get
                Return Me.OccurrenceStatus.ToString()
            End Get
            Set(value As String)
                Dim s As eOccurrenceStatusTypes = eOccurrenceStatusTypes.NotSet
                If Not String.IsNullOrWhiteSpace(value) Then
                    [Enum].TryParse(value.Replace(" ", ""), s)
                End If
                Me.OccurrenceStatus = s
            End Set
        End Property

        Protected Property Exploitation As String
            Get
                Return Me.ExploitationStatus.ToString()
            End Get
            Set(value As String)
                Dim e As eExploitationTypes = eExploitationTypes.NotSet
                If (Not String.IsNullOrWhiteSpace(value)) Then
                    [Enum].TryParse(value.Replace(" ", ""), e)
                End If
                Me.ExploitationStatus = e
            End Set
        End Property

        Protected Property IUCN As String
            Get
                Return Me.IUCNConservationStatus.ToString
            End Get
            Set(value As String)
                Dim s As eIUCNConservationStatusTypes = eIUCNConservationStatusTypes.NotSet
                Dim i As Integer = s
                If Integer.TryParse(value, i) Then
                    s = CType(i, eIUCNConservationStatusTypes)
                Else
                    Select Case value.ToUpper
                        Case "NE" : s = eIUCNConservationStatusTypes.NotEvaluated
                        Case "DD" : s = eIUCNConservationStatusTypes.DataDeficient
                        Case "LC" : s = eIUCNConservationStatusTypes.LeastConcern
                        Case "NT" : s = eIUCNConservationStatusTypes.NearThreatened
                        Case "VU" : s = eIUCNConservationStatusTypes.Vulnerable
                        Case "EN" : s = eIUCNConservationStatusTypes.Endangered
                        Case "CR" : s = eIUCNConservationStatusTypes.CriticallyEndangered
                    End Select
                End If
                Me.IUCNConservationStatus = s
            End Set
        End Property

#End Region ' Serialization

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update this unit with new Taxonomy data.
        ''' </summary>
        ''' <param name="taxon"></param>
        ''' -------------------------------------------------------------------
        Public Sub Update(ByVal taxon As ITaxonSearchData)
            Me.CodeSAUP = taxon.CodeSAUP
            Me.CodeFishBase = taxon.CodeFishBase
            Me.CodeSeaLifeBase = taxon.CodeSeaLifeBase
            Me.CodeAquaMaps = taxon.CodeAquaMaps
            Me.CodeAphia = taxon.CodeAphia
            Me.CodeOBIS = taxon.CodeOBIS
            Me.CodeFAO = taxon.CodeFAO
            Me.CodeLSID = taxon.CodeLSID
            Me.Common = taxon.Common
            Me.Phylum = taxon.Phylum
            Me.Class = taxon.Class
            Me.Order = taxon.Order
            Me.Family = taxon.Family
            Me.Genus = taxon.Genus
            Me.Species = taxon.Species
            Me.North = taxon.North
            Me.South = taxon.South
            Me.East = taxon.East
            Me.West = taxon.West
            If TypeOf (taxon) Is ITaxonDetailsData Then
                Dim details As ITaxonDetailsData = DirectCast(taxon, ITaxonDetailsData)
                Me.EcologyType = details.EcologyType
                Me.OccurrenceStatus = details.OccurrenceStatus
                Me.OrganismType = details.OrganismType
                Me.IUCNConservationStatus = details.IUCNConservationStatus
                Me.MeanLength = details.MeanLength
                Me.MaxLength = details.MaxLength
                Me.MeanWeight = details.MeanWeight
                Me.MeanLifespan = details.MeanLifespan
                Me.SearchFields = details.SearchFields
                Me.LastUpdated = details.LastUpdated
            End If
            Me.SourceKey = taxon.SourceKey
            Me.Source = taxon.Source
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the DBID of the <see cref="cTaxon">EwE Taxonomy code</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property TaxonID() As Integer = cCore.NULL_VALUE

        Public ReadOnly Property TaxonIndex() As Integer = -1

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the Taxonomy code is to be created.
        ''' </summary>
        ''' <returns>
        ''' True when Layer <see cref="Name">Name</see> value has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.TaxonID = cCore.NULL_VALUE)
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

                Debug.Assert(CInt(taxon.GetVariable(eVarNameFlags.DBID)) = Me.TaxonID)

                If (Math.Round(taxon.PropB, 5) <> Math.Round(Me.PropB, 5)) Then Return True
                If (Math.Round(taxon.PropC, 5) <> Math.Round(Me.PropC, 5)) Then Return True
                If (taxon.iGroup <> Me.iGroup) Then Return True
                If (taxon.iStanza <> Me.iStanza) Then Return True
                If (taxon.CodeFishBase <> Me.CodeFishBase) Then Return True
                If (taxon.CodeSeaLifeBase <> Me.CodeSeaLifeBase) Then Return True
                If (taxon.CodeAquaMaps <> Me.CodeAquaMaps) Then Return True
                If (taxon.CodeOBIS <> Me.CodeOBIS) Then Return True
                If (taxon.CodeAphia <> Me.CodeAphia) Then Return True
                If (taxon.CodeSAUP <> Me.CodeSAUP) Then Return True
                If (String.Compare(taxon.CodeLSID, Me.CodeLSID) <> 0) Then Return True
                If (String.Compare(taxon.CodeFAO, Me.CodeFAO) <> 0) Then Return True

                If (String.Compare(taxon.Name, Me.Common) <> 0) Then Return True
                If (String.Compare(taxon.Phylum, Me.Phylum) <> 0) Then Return True
                If (String.Compare(taxon.Class, Me.Class) <> 0) Then Return True
                If (String.Compare(taxon.Order, Me.Order) <> 0) Then Return True
                If (String.Compare(taxon.Family, Me.Family) <> 0) Then Return True
                If (String.Compare(taxon.Genus, Me.Genus) <> 0) Then Return True
                If (String.Compare(taxon.Species, Me.Species) <> 0) Then Return True
                If (String.Compare(taxon.Source, Me.Source) <> 0) Then Return True

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
        <Browsable(False)>
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
            If (TypeOf obj Is ITaxonSearchData) Then
                Dim t As ITaxonSearchData = DirectCast(obj, ITaxonSearchData)
                Return (t.CodeSAUP > 0 And t.CodeSAUP = Me.CodeSAUP) Or
                       (t.CodeFishBase > 0 And t.CodeFishBase = Me.CodeFishBase) Or
                       (t.CodeSeaLifeBase > 0 And t.CodeSeaLifeBase = Me.CodeSeaLifeBase) Or
                       (t.CodeOBIS > 0 And t.CodeOBIS = Me.CodeOBIS) Or
                       (Not String.IsNullOrWhiteSpace(t.CodeLSID) And String.Compare(t.CodeLSID, Me.CodeLSID, True) = 0) Or
                       (Not String.IsNullOrWhiteSpace(t.CodeAquaMaps) And String.Compare(t.CodeAquaMaps, Me.CodeAquaMaps, True) = 0) Or
                       (Not String.IsNullOrWhiteSpace(t.CodeAphia) And String.Compare(t.CodeAphia, Me.CodeAphia, True) = 0) Or
                       (Not String.IsNullOrWhiteSpace(t.CodeFAO) And String.Compare(t.CodeFAO, Me.CodeFAO, True) = 0)
            End If
            Return MyBase.Equals(obj)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group index of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property iGroup() As Integer = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the stanza index of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property iStanza() As Integer = 0

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group/stanza biomass proportion for this taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property PropB() As Single = 1.0!

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group/stanza catch proportion for this taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property PropC() As Single = 1.0

        Public Sub ApplyChanges(ByVal taxon As cTaxon)
            If Me.IsChanged(taxon) Then
                With taxon
                    .Name = Me.Common
                    .iGroup = Me.iGroup
                    .iStanza = Me.iStanza
                    .PropB = Me.PropB
                    .PropC = Me.PropC
                    .CodeSAUP = Me.CodeSAUP
                    .CodeFishBase = Me.CodeFishBase
                    .CodeSeaLifeBase = Me.CodeSeaLifeBase
                    .CodeAquaMaps = Me.CodeAquaMaps
                    .CodeOBIS = Me.CodeOBIS
                    .CodeAphia = Me.CodeAphia
                    .CodeLSID = Me.CodeLSID
                    .CodeFAO = Me.CodeFAO
                    .Species = Me.Species
                    .Family = Me.Family
                    .Genus = Me.Genus
                    .Order = Me.Order
                    .Class = Me.Class
                    .Source = Me.Source
                    .SourceKey = Me.SourceKey
                    .North = Me.North
                    .West = Me.West
                    .East = Me.East
                    .South = Me.South
                    .EcologyType = Me.EcologyType
                    .IUCNConservationStatus = Me.IUCNConservationStatus
                    .OrganismType = Me.OrganismType
                    .OccurrenceStatus = Me.OccurrenceStatus
                    .MaxLength = Me.MaxLength
                    .MeanLength = Me.MeanLength
                    .MeanWeight = Me.MeanWeight
                    .MeanLifespan = Me.MeanLifespan
                    .LastUpdated = cDateUtils.DateToJulian()
                End With
            End If
        End Sub

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

    Public Overrides Property UIContext As ScientificInterfaceShared.Controls.cUIContext
        Get
            Return MyBase.UIContext
        End Get
        Set(value As ScientificInterfaceShared.Controls.cUIContext)
            Me.m_lTaxonInfo.Clear()
            If (value IsNot Nothing) Then
                ' Make snapshot of configuration 
                For iTaxon As Integer = 1 To value.Core.nTaxon
                    Dim ti As New cTaxonInfo(value.Core.Taxon(iTaxon))
                    Me.m_lTaxonInfo.Add(ti)
                Next
            End If
            MyBase.UIContext = value
        End Set
    End Property

#End Region ' Constructor

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub InitStyle()

        MyBase.InitStyle()

        Me.Selection.EnableMultiSelection = False

        Dim iNumCols As Integer = CInt(If(Me.m_bShowCodes, System.Enum.GetValues(GetType(eColumnTypes)).Length, eColumnTypes.Status + 1))

        ' Redim columns
        Me.Redim(1, iNumCols)

        ' Group index cell
        Me(0, eColumnTypes.Hierarchy) = New cEwEColumnHeaderCell()
        Me(0, eColumnTypes.Name) = New cEwEColumnHeaderCell(eVarNameFlags.Name)
        Me(0, eColumnTypes.PropB) = New cEwEColumnHeaderCell(eVarNameFlags.TaxonPropBiomass)
        Me(0, eColumnTypes.PropC) = New cEwEColumnHeaderCell(eVarNameFlags.TaxonPropCatch)
        Me(0, eColumnTypes.Phylum) = New cEwEColumnHeaderCell(eVarNameFlags.Phylum)
        Me(0, eColumnTypes.Class) = New cEwEColumnHeaderCell(eVarNameFlags.Class)
        Me(0, eColumnTypes.Order) = New cEwEColumnHeaderCell(eVarNameFlags.Order)
        Me(0, eColumnTypes.Family) = New cEwEColumnHeaderCell(eVarNameFlags.Family)
        Me(0, eColumnTypes.Genus) = New cEwEColumnHeaderCell(eVarNameFlags.Genus)
        Me(0, eColumnTypes.Species) = New cEwEColumnHeaderCell(eVarNameFlags.Species)
        Me(0, eColumnTypes.Status) = New cEwEColumnHeaderCell(SharedResources.HEADER_STATUS)

        If (Me.m_bShowCodes) Then
            Me(0, eColumnTypes.CodeFishBase) = New cEwEColumnHeaderCell(eVarNameFlags.CodeFB)
            Me(0, eColumnTypes.CodeSeaLifeBase) = New cEwEColumnHeaderCell(eVarNameFlags.CodeSLB)
            Me(0, eColumnTypes.CodeSAUP) = New cEwEColumnHeaderCell(eVarNameFlags.CodeSAUP)
            Me(0, eColumnTypes.CodeAquaMaps) = New cEwEColumnHeaderCell(eVarNameFlags.CodeAquaMaps)
            Me(0, eColumnTypes.CodeOBIS) = New cEwEColumnHeaderCell(eVarNameFlags.CodeOBIS)
            Me(0, eColumnTypes.CodeAphia) = New cEwEColumnHeaderCell(eVarNameFlags.CodeAphia)
            Me(0, eColumnTypes.CodeFAO) = New cEwEColumnHeaderCell(eVarNameFlags.CodeFAO)
            Me(0, eColumnTypes.CodeLSID) = New cEwEColumnHeaderCell(eVarNameFlags.CodeLSID)
        End If

        Me.FixedColumns = 1
        Me.FixedColumnWidths = True

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
        Dim hgcParent As cEwEHierarchyGridCell = Nothing
        Dim taxon As cTaxon = Nothing
        Dim ti As cTaxonInfo = Nothing

        ' Populate local administration from a snapshot of the live data

        ' Me.NormalizeProportions()

        ' Create rows
        Me.RowsCount = 1

        For iGroup As Integer = 1 To Me.Core.nGroups

            grp = Me.Core.EcopathGroupInputs(iGroup)
            If grp.IsMultiStanza Then

                If Not abStanzaHandled(grp.iStanza) Then
                    ' Create row for multi-stanza group
                    iRow = Me.AddRow()
                    stz = Me.Core.StanzaGroups(grp.iStanza)

                    hgcParent = New cEwEHierarchyGridCell()
                    hgcParent.Tag = stz

                    Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                    Me(iRow, eColumnTypes.Name) = New cPropertyRowHeaderParentCell(Me.PropertyManager, stz, eVarNameFlags.Name, Nothing, hgcParent)
                    For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                        Me(iRow, iCol) = New cEwERowHeaderCell("")
                    Next

                    For iTaxon As Integer = 0 To Me.m_lTaxonInfo.Count - 1
                        ti = Me.m_lTaxonInfo(iTaxon)
                        If ti.iStanza = stz.Index Then
                            Me.AddTaxonRow(ti, iRow)
                        End If
                    Next
                    abStanzaHandled(grp.iStanza) = True

                End If
            Else
                ' Add regular group row
                iRow = Me.AddRow()

                hgcParent = New cEwEHierarchyGridCell()
                hgcParent.Tag = grp

                Me(iRow, eColumnTypes.Hierarchy) = hgcParent
                Me(iRow, eColumnTypes.Name) = New cEwERowHeaderCell(String.Format(SharedResources.GENERIC_LABEL_INDEXED, grp.Index, grp.Name))
                For iCol As Integer = eColumnTypes.Name + 1 To Me.ColumnsCount - 1
                    Me(iRow, iCol) = New cEwERowHeaderCell("")
                Next

                For iTaxon As Integer = 0 To Me.m_lTaxonInfo.Count - 1
                    ti = Me.m_lTaxonInfo(iTaxon)
                    If ti.iGroup = grp.Index Then
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

        Dim bShowCol As Boolean = True

        For iCol As Integer = 1 To Me.ColumnsCount - 1
            Select Case DirectCast(iCol, eColumnTypes)
                Case eColumnTypes.Hierarchy
                    Me.Columns(iCol).Width = 20
                Case Else
                    Me.Columns(iCol).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                    Me.AutoSizeColumn(iCol, 40)
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
        Me(iRow, eColumnTypes.PropB).Value = ti.PropB
        Me(iRow, eColumnTypes.PropC).Value = ti.PropC

        If (Me.m_bShowCodes) Then
            Me(iRow, eColumnTypes.CodeSAUP).Value = ti.CodeSAUP
            Me(iRow, eColumnTypes.CodeFishBase).Value = ti.CodeFishBase
            Me(iRow, eColumnTypes.CodeSeaLifeBase).Value = ti.CodeSeaLifeBase
            Me(iRow, eColumnTypes.CodeFAO).Value = ti.CodeFAO
            Me(iRow, eColumnTypes.CodeLSID).Value = ti.CodeLSID
            Me(iRow, eColumnTypes.CodeAquaMaps).Value = ti.CodeAquaMaps
            Me(iRow, eColumnTypes.CodeOBIS).Value = ti.CodeOBIS
            Me(iRow, eColumnTypes.CodeAphia).Value = ti.CodeAphia
        End If

        Me(iRow, eColumnTypes.Status).Value = ti.Status

    End Sub

    Private Function FindParentRow(ByVal iRow As Integer) As Integer
        If iRow < 1 Then Return -1
        While (iRow > 0) And Not (TypeOf Me(iRow, eColumnTypes.Hierarchy) Is cEwEHierarchyGridCell)
            iRow -= 1
        End While
        Return iRow
    End Function

    Private Function AddTaxonRow(ByVal ti As cTaxonInfo, Optional ByVal iRow As Integer = -1) As Integer

        Dim cell As cEwECell = Nothing

        If iRow = -1 Then
            iRow = Me.FindParentRow(Me.SelectedRow)
        End If

        Dim hgcParent As cEwEHierarchyGridCell = DirectCast(Me(iRow, eColumnTypes.Hierarchy), cEwEHierarchyGridCell)
        iRow += hgcParent.NumChildRows + 1
        Me.Rows.Insert(iRow)
        Me(iRow, eColumnTypes.Hierarchy) = New cEwERowHeaderCell()
        Me(iRow, eColumnTypes.Hierarchy).Tag = ti
        Me(iRow, eColumnTypes.Name) = New cEwECell(ti.Common, GetType(String))
        Me(iRow, eColumnTypes.Name).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Species) = New cEwECell(ti.Species, GetType(String), cStyleGuide.eStyleFlags.Taxon)
        Me(iRow, eColumnTypes.Species).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Genus) = New cEwECell(ti.Genus, GetType(String), cStyleGuide.eStyleFlags.Taxon)
        Me(iRow, eColumnTypes.Genus).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Family) = New cEwECell(ti.Family, GetType(String))
        Me(iRow, eColumnTypes.Family).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Order) = New cEwECell(ti.Order, GetType(String))
        Me(iRow, eColumnTypes.Order).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Class) = New cEwECell(ti.Class, GetType(String))
        Me(iRow, eColumnTypes.Class).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Phylum) = New cEwECell(ti.Phylum, GetType(String))
        Me(iRow, eColumnTypes.Phylum).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.PropB) = New cEwECell(ti.PropB, GetType(Single))
        Me(iRow, eColumnTypes.PropB).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.PropC) = New cEwECell(ti.PropC, GetType(Single))
        Me(iRow, eColumnTypes.PropC).Behaviors.Add(Me.EwEEditHandler)
        Me(iRow, eColumnTypes.Status) = New cEwEStatusCell(eItemStatusTypes.Original)

        If (Me.m_bShowCodes) Then

            ' == CODE cells
            'Me(iRow, eColumnTypes.Code) = New EwECell(ti.SourceKey, GetType(String), cStyleGuide.eStyleFlags.NotEditable)

            cell = New cEwECell(ti.CodeFishBase, GetType(Long))
            cell.SuppressZero(CLng(cCore.NULL_VALUE)) = True
            Me(iRow, eColumnTypes.CodeFishBase) = cell

            cell = New cEwECell(ti.CodeSeaLifeBase, GetType(Long))
            cell.SuppressZero(CLng(cCore.NULL_VALUE)) = True
            Me(iRow, eColumnTypes.CodeSeaLifeBase) = cell

            cell = New cEwECell(ti.CodeSAUP, GetType(Long))
            cell.SuppressZero(CLng(cCore.NULL_VALUE)) = True
            Me(iRow, eColumnTypes.CodeSAUP) = cell

            cell = New cEwECell(ti.CodeOBIS, GetType(Long))
            cell.SuppressZero(CLng(cCore.NULL_VALUE)) = True
            Me(iRow, eColumnTypes.CodeOBIS) = cell

            Me(iRow, eColumnTypes.CodeFAO) = New cEwECell(ti.CodeFAO, GetType(String))
            Me(iRow, eColumnTypes.CodeLSID) = New cEwECell(ti.CodeLSID, GetType(String))
            Me(iRow, eColumnTypes.CodeAquaMaps) = New cEwECell(ti.CodeAquaMaps, GetType(String))
            Me(iRow, eColumnTypes.CodeAphia) = New cEwECell(ti.CodeAphia, GetType(String))

        End If

        hgcParent.AddChildRow(iRow)
        Me.UpdateRow(iRow)

    End Function

    Private Sub RemoveTaxonRow(ByVal iRow As Integer)
        If iRow <= 0 Then iRow = Me.SelectedRow
        Dim iRowParent As Integer = Me.FindParentRow(iRow)
        If iRowParent >= 1 Then
            Dim hgcParent As cEwEHierarchyGridCell = DirectCast(Me(iRowParent, eColumnTypes.Hierarchy), cEwEHierarchyGridCell)
            hgcParent.RemoveChildRow(iRow)
            Me.Rows.Remove(iRow)
        End If
    End Sub

    Public Sub UpdateProportions()
        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim ti As cTaxonInfo = Me.TaxonInfo(iRow)
            If ti IsNot Nothing Then
                Me(iRow, eColumnTypes.PropB).Value = ti.PropB
                Me(iRow, eColumnTypes.PropC).Value = ti.PropC
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
            Case eColumnTypes.PropB : ti.PropB = CSng(val)
            Case eColumnTypes.PropC : ti.PropC = CSng(val)
            Case eColumnTypes.CodeFishBase : ti.CodeFishBase = CLng(val)
            Case eColumnTypes.CodeSeaLifeBase : ti.CodeSeaLifeBase = CLng(val)
            Case eColumnTypes.CodeAquaMaps : ti.CodeAquaMaps = CStr(val)
            Case eColumnTypes.CodeOBIS : ti.CodeOBIS = CLng(val)
            Case eColumnTypes.CodeAphia : ti.CodeAphia = CStr(val)
            Case eColumnTypes.CodeSAUP : ti.CodeSAUP = CLng(val)
            Case eColumnTypes.CodeFAO : ti.CodeFAO = CStr(val)
            Case eColumnTypes.CodeLSID : ti.CodeLSID = CStr(val)
                'Case eColumnTypes.Code : ti.CodeTaxon = CStr(val)
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

#Region " Import / export "

    Public Function Import(file As String) As Boolean

        Dim newData As New List(Of cTaxonInfo)
        Dim msg As cMessage = Nothing
        If cGenericDataReader.Read(file, "", newData) Then
            If (newData.Count > 0 And Me.m_lTaxonInfo.Count > 0) Then
                ' Ask for confirmation
                Dim fmsg As New cFeedbackMessage(My.Resources.ECOPATH_IMPORTTAXA_DELETE_PROMPT, eCoreComponentType.External, eMessageType.DataImport, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
                fmsg.Reply = eMessageReply.NO
                Core.Messages.SendMessage(fmsg)
                If (fmsg.Reply <> eMessageReply.YES) Then Return False
            End If

            ' Flag all orignals for deletion, trash all new items
            Me.m_lTaxonInfoRemoved.Clear()

            For Each ti As cTaxonInfo In Me.m_lTaxonInfo
                If (ti IsNot Nothing) Then
                    ti.FlaggedForDeletion = True
                    Select Case ti.Status
                        Case eItemStatusTypes.Original
                            Me.m_lTaxonInfoRemoved.Add(ti)
                    End Select
                End If
            Next
            Me.m_lTaxonInfo.Clear()

            ' Filter out bad records
            For Each ti As cTaxonInfo In newData
                If (ti.iGroup > 0 Or ti.iStanza > 0) Then
                    Me.m_lTaxonInfo.Add(ti)
                End If
            Next
            Me.RefreshContent()

            msg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_FILEIMPORT_SUCCESS, file), eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Information)
        Else
            msg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_FILEIMPORT_FAILURE, file, ""), eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Critical)
        End If

        Me.Core.Messages.SendMessage(msg)

        Return True
    End Function

    Public Function Export(file As String) As Boolean

        Dim excl As String() = New String() {"FlaggedForDeletion", "LastUpdated", "SearchFields"}
        Dim dt As DataTable = cDataTableConverter.ToDatatable(Me.m_lTaxonInfo, excl)
        Dim msg As cMessage = Nothing

        If cGenericDataWriter.Write(dt, file, "") Then
            Dim wr As New cFieldInfoWriter(Me.Core)
            wr.Write(GetType(cTaxonInfo), cFileUtils.AddToName(file, "_fieldinfo", ".txt"), excl)

            msg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_FILESAVE_SUCCES, "Taxonomy", file), eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Information)
            msg.Hyperlink = System.IO.Path.GetDirectoryName(file)
        Else
            msg = New cMessage(cStringUtils.Localize(My.Resources.GENERIC_FILESAVE_FAILURE, "Taxonomy", file, ""), eMessageType.DataExport, eCoreComponentType.Ecopath, eMessageImportance.Critical)
        End If
        Me.Core.Messages.SendMessage(msg)

    End Function

#End Region ' Import / export

#Region " Data "

    Public Property SelectedTaxon() As ITaxonSearchData
        Get
            Return Me.TaxonInfo(Me.SelectedRow)
        End Get
        Set(ByVal taxon As ITaxonSearchData)
            If Not (TypeOf taxon Is cTaxonInfo) Then Return
            For iRow As Integer = 1 To Me.RowsCount - 1
                If ReferenceEquals(TaxonInfo(iRow), taxon) Then
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
    ''' Get/set whether the various keys need to be shown in the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ShowCodes As Boolean
        Get
            Return Me.m_bShowCodes
        End Get
        Set(value As Boolean)
            If (Me.m_bShowCodes <> value) Then
                Me.m_bShowCodes = value
                Me.RefreshContent()
            End If
        End Set
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

    Public Overrides ReadOnly Property SuppressQuickEdits As Boolean
        Get
            Return False
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
        Else
            ti = New cTaxonInfo(taxon)
            If (grp Is Nothing) Then
                ti.iStanza = stz.Index
            Else
                ti.iGroup = grp.Index
            End If
        End If

        Me.m_lTaxonInfo.Add(ti)
        Me.AddTaxonRow(ti)
        Me.SelectedTaxon = ti

    End Sub

    ''' -----------------------------------------------------------------------
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
    ''' -----------------------------------------------------------------------
    Public Function CanAddTaxon(Optional ByVal taxon As ITaxonSearchData = Nothing) As Boolean

        Dim grp As cEcoPathGroupInput = Me.SelectedGroup
        Dim stz As cStanzaGroup = Me.SelectedStanza

        Dim bIsTaxonUsed As Boolean = False
        Dim bStanzaHasTaxon As Boolean = False

        If (grp Is Nothing) And (stz Is Nothing) Then Return False

        For Each ti As cTaxonInfo In Me.m_lTaxonInfo
            'bIsTaxonUsed = bIsTaxonUsed Or (ti.Equals(taxon))
            If (stz IsNot Nothing) Then
                bStanzaHasTaxon = bStanzaHasTaxon Or (stz.Index = ti.iStanza)
            End If
        Next

        Return (Not bIsTaxonUsed) And (Not bStanzaHasTaxon)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the delete state of all selected rows.
    ''' </summary>
    ''' <param name="bDelete">The state to set rows to.</param>
    ''' -----------------------------------------------------------------------
    Public Sub DeleteRows(bDelete As Boolean)

        Dim sel As Selection = Me.Selection
        Dim rows As RowInfo() = sel.SelectedRows
        Dim ti As cTaxonInfo = Nothing

        For Each row As RowInfo In rows
            Dim iRow As Integer = row.Index

            ti = Me.TaxonInfo(iRow)

            If (ti IsNot Nothing) Then
                ti.FlaggedForDeletion = bDelete

                ' Check to see what is to happen to the MPA now
                If (bDelete) Then
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
                Else
                    Me.UpdateRow(iRow)
                End If

            End If
        Next

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether a taxon has already been used.
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function IsTaxonUsed(taxon As ITaxonSearchData) As Boolean
        For Each ti As cTaxonInfo In Me.m_lTaxonInfo
            If ti.Equals(taxon) Then
                Return True
            End If
        Next
        Return False
    End Function

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
        Return (ReferenceEquals(taxon, Me.m_tiSearch)) And
               (ReferenceEquals(Me.SelectedTaxon, Me.m_tiSearchLinked))
    End Function

#End Region ' Search

#Region " Apply changes "

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
        If (Me.m_lTaxonInfoRemoved.Count > 0) Then

            Dim fmsg As New cFeedbackMessage(My.Resources.TAXON_DELETE_CONFIRMATION, eCoreComponentType.Ecopath, eMessageType.Any, eMessageImportance.Question, eMessageReplyStyle.YES_NO)
            Me.UIContext.Core.Messages.SendMessage(fmsg)

            Select Case fmsg.Reply
                Case eMessageReply.NO
                    ' Abort
                    Return False
                Case eMessageReply.YES
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

            cApplicationStatusNotifier.StartProgress(Me.Core, SharedResources.GENERIC_STATUS_APPLYCHANGES)

            Dim htTaxonID As New Dictionary(Of cTaxonInfo, Integer)
            Dim iDBID As Integer = Nothing

            Try

                ' Add new Taxons
                For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
                    ti = Me.m_lTaxonInfo(iTaxon)
                    If (ti.IsNew) Then
                        bSuccess = bSuccess And Me.Core.AddTaxon(Math.Max(ti.iGroup, ti.iStanza), (ti.iStanza > 0), ti, ti.PropB, ti.PropC, iDBID)
                        ' Map this new ID during update
                        htTaxonID.Add(ti, iDBID)
                    End If
                Next

                ' Remove deleted Taxons
                For Each ti In Me.m_lTaxonInfoRemoved
                    bSuccess = bSuccess And Me.Core.RemoveTaxon(ti.TaxonIndex)
                Next
                Me.m_lTaxonInfoRemoved.Clear()

            Catch ex As Exception

            End Try

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.EndProgress(Me.Core)

        End If


        ' Update any changed taxa
        Dim dtTaxa As New Dictionary(Of Integer, cTaxon)
        For i As Integer = 1 To Me.Core.nTaxon
            taxon = Me.Core.Taxon(i)
            dtTaxa(CInt(taxon.GetVariable(eVarNameFlags.DBID))) = taxon
        Next

        Me.Core.SetBatchLock(cCore.eBatchLockType.Update)
        For Each ti In Me.m_lTaxonInfo
            If (Not ti.IsNew And Not ti.FlaggedForDeletion) Then
                ti.ApplyChanges(dtTaxa(ti.TaxonID))
            End If
        Next
        Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)

        Return bSuccess

    End Function

#End Region ' Apply changes

#End Region ' Public bits

End Class
