#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin.Data
Imports EwEUtils.Utilities
Imports EwEUtils.Core
Imports SourceGrid2

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Grid class implementing the Edit Group Taxon interface grid bit.
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class gridEditGroupTaxon
    Inherits EwEGrid

#Region " Privates "

    ''' <summary>List of active taxa.</summary>
    Private m_lTaxonInfo As New List(Of cTaxonInfo)
    ''' <summary>List of removed taxa.</summary>
    Private m_lTaxonInfoRemoved As New List(Of cTaxonInfo)

    ''' <summary>Search term for public use.</summary>
    Private m_tiSearch As ITaxonData = Nothing
    ''' <summary>Internal item linked to the search term.</summary>
    Private m_tiSearchLinked As ITaxonData = Nothing

    Private m_vizPropNormalized As New cEwEGridProportionVisualizer()
    Private m_lUsedKeys As New List(Of String)

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        Hierarchy = 0
        Name
        Proportion
        PropNorm
        LastUpdated
        Status
    End Enum

#End Region ' Privates

#Region " Private helper classes "

    Private Class cTaxonInfo
        Implements ITaxonData

#Region " Private vars "

        Private m_taxon As cTaxon = Nothing
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
        Private m_sProportion As Single = 1.0!
        Private m_sPropNorm As Single = 1.0!
        ''' <summary>Index of the ecopath group that this taxon contributes to.</summary>
        Private m_iGroup As Integer = Nothing

        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a Layer in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original
        Private m_dLastUpdated As Double = 0.0

#End Region ' Private vars

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal group As cEcoPathGroupInput)
            Me.m_iGroup = group.Index
            Me.m_sProportion = 1.0!
            Me.m_strCommon = group.Name
            Me.m_status = eItemStatusTypes.Added
            Me.m_dLastUpdated = cDateUtils.DateToJulian()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an administrative unit for an existing taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal taxon As cTaxon)
            Me.m_taxon = taxon
            Me.m_iGroup = taxon.Group
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
            Me.m_strSource = taxon.Source
            Me.m_strKey = taxon.SourceKey
            Me.m_status = eItemStatusTypes.Original
            Me.m_dLastUpdated = taxon.LastUpdated
        End Sub

        Public Sub New(ByVal taxon As ITaxonData)
            Me.Update(taxon)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update this unit with new Taxonomy data.
        ''' </summary>
        ''' <param name="taxon"></param>
        ''' -------------------------------------------------------------------
        Public Sub Update(ByVal taxon As ITaxonData)
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
            Me.m_strKey = taxon.SourceKey
            Me.m_strSource = taxon.Source
            Me.m_dLastUpdated = taxon.LastUpdated
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cTaxon">EwE Taxonomy code</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Taxon() As cTaxon
            Get
                Return Me.m_taxon
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
            Return (Me.m_taxon Is Nothing)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the administrative unit has changed.
        ''' </summary>
        ''' <returns>
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property IsChanged() As Boolean
            Get
                If (Me.IsNew()) Then Return False
                If (Me.m_taxon.Proportion <> Me.m_sProportion) Then Return True
                If (Me.m_taxon.Group <> Me.m_iGroup) Then Return True
                If (String.Compare(Me.Taxon.Name, Me.m_strCommon) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Phylum, Me.m_strPhylum) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Class, Me.m_strClass) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Order, Me.m_strOrder) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Family, Me.m_strFamily) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Genus, Me.m_strGenus) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Species, Me.m_strSpecies) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Source, Me.m_strSource) <> 0) Then Return True
                If (String.Compare(Me.Taxon.CodeTaxon, Me.m_strCodeTaxon) <> 0) Then Return True
                If (String.Compare(Me.Taxon.CodeISSCAAP, Me.m_strCodeISSCAAP) <> 0) Then Return True
                If (String.Compare(Me.Taxon.Code3A, Me.m_strCode3A) <> 0) Then Return True
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
        ''' Get/set whether the user has confirmed an action on this object.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Confirmed() As Boolean
            Get
                Return Me.m_bConfirmed
            End Get
            Set(ByVal value As Boolean)
                Me.m_bConfirmed = value
            End Set
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the group of this administrative unit.
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

        Public Property PropNormalized() As Single
            Get
                Return Me.m_sPropNorm
            End Get
            Set(ByVal value As Single)
                Me.m_sPropNorm = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Phylum"/>
        ''' -------------------------------------------------------------------
        Public Property Phylum() As String _
            Implements ITaxonData.Phylum
            Get
                Return Me.m_strPhylum
            End Get
            Set(ByVal value As String)
                Me.m_strPhylum = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.[Class]"/>
        ''' -------------------------------------------------------------------
        Public Property [Class]() As String _
            Implements ITaxonData.Class
            Get
                Return m_strClass
            End Get
            Set(ByVal value As String)
                m_strClass = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Code3A"/>
        ''' -------------------------------------------------------------------
        Public Property Code3A() As String _
            Implements ITaxonData.Code3A
            Get
                Return m_strCode3A
            End Get
            Set(ByVal value As String)
                m_strCode3A = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.CodeISSCAAP"/>
        ''' -------------------------------------------------------------------
        Public Property CodeISSCAAP() As String _
            Implements ITaxonData.CodeISSCAAP
            Get
                Return m_strCodeISSCAAP
            End Get
            Set(ByVal value As String)
                Me.m_strCodeISSCAAP = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.CodeTaxon"/>
        ''' -------------------------------------------------------------------
        Public Property CodeTaxon() As String _
            Implements ITaxonData.CodeTaxon
            Get
                Return Me.m_strCodeTaxon
            End Get
            Set(ByVal value As String)
                Me.m_strCodeTaxon = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Common"/>
        ''' -------------------------------------------------------------------
        Public Property Common() As String _
            Implements ITaxonData.Common
            Get
                Return Me.m_strCommon
            End Get
            Set(ByVal value As String)
                Me.m_strCommon = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Family"/>
        ''' -------------------------------------------------------------------
        Public Property Family() As String _
            Implements ITaxonData.Family
            Get
                Return Me.m_strFamily
            End Get
            Set(ByVal value As String)
                Me.m_strFamily = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Order"/>
        ''' -------------------------------------------------------------------
        Public Property Order() As String _
            Implements ITaxonData.Order
            Get
                Return Me.m_strOrder
            End Get
            Set(ByVal value As String)
                Me.m_strOrder = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Genus"/>
        ''' -------------------------------------------------------------------
        Public Property Genus() As String _
            Implements ITaxonData.Genus
            Get
                Return Me.m_strGenus
            End Get
            Set(ByVal value As String)
                Me.m_strGenus = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.LastUpdated"/>
        ''' -------------------------------------------------------------------
        Public Property LastUpdated() As Double _
            Implements ITaxonData.LastUpdated
            Get
                Return Me.m_dLastUpdated
            End Get
            Private Set(ByVal value As Double)
                ' NOP
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Source"/>
        ''' -------------------------------------------------------------------
        Public Property Source() As String _
            Implements ITaxonData.Source
            Get
                Return Me.m_strSource
            End Get
            Set(ByVal value As String)
                Me.m_strSource = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.SourceKey"/>
        ''' -------------------------------------------------------------------
        Public Property SourceKey() As String _
            Implements ITaxonData.SourceKey
            Get
                Return Me.m_strKey
            End Get
            Set(ByVal value As String)
                Me.m_strKey = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.Species"/>
        ''' -------------------------------------------------------------------
        Public Property Species() As String _
           Implements ITaxonData.Species
            Get
                Return Me.m_strSpecies
            End Get
            Set(ByVal value As String)
                Me.m_strSpecies = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.North"/>
        ''' -------------------------------------------------------------------
        Public Property North() As Single _
            Implements ITaxonData.North
            Get
                Return Me.m_sNorth
            End Get
            Set(ByVal value As Single)
                Me.m_sNorth = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.South"/>
        ''' -------------------------------------------------------------------
        Public Property South() As Single _
            Implements ITaxonData.South
            Get
                Return Me.m_sSouth
            End Get
            Set(ByVal value As Single)
                Me.m_sSouth = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.East"/>
        ''' -------------------------------------------------------------------
        Public Property East() As Single _
            Implements ITaxonData.East
            Get
                Return Me.m_sEast
            End Get
            Set(ByVal value As Single)
                Me.m_sEast = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="ITaxonData.West"/>
        ''' -------------------------------------------------------------------
        Public Property West() As Single _
            Implements ITaxonData.West
            Get
                Return Me.m_sWest
            End Get
            Set(ByVal value As Single)
                Me.m_sWest = value
            End Set
        End Property

        Public Sub ApplyChanges()
            If Me.IsChanged Then
                With Me.Taxon
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
        Me(0, eColumnTypes.Name) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
        Me(0, eColumnTypes.Proportion) = New EwEColumnHeaderCell(My.Resources.HEADER_PROPORTION)
        Me(0, eColumnTypes.PropNorm) = New EwEColumnHeaderCell(My.Resources.HEADER_PROPORTION)
        Me(0, eColumnTypes.LastUpdated) = New EwEColumnHeaderCell(My.Resources.HEADER_LASTUPDATED)
        Me(0, eColumnTypes.Status) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

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

        ' Brute-force update grid
        Me.UpdateGrid()

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()

        For iCol As Integer = 1 To Me.ColumnsCount - 1
            Select Case DirectCast(iCol, eColumnTypes)
                Case eColumnTypes.Hierarchy
                    Me.Columns(iCol).Width = 20
                Case eColumnTypes.PropNorm
                    Me.Columns(iCol).Width = 100
                Case Else
                    Me.Columns(iCol).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                    Me.AutoSizeColumn(iCol, 100)
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
    ''' Brute-force resize the gird if necessary, and repopulate with data from 
    ''' the local administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateGrid()

        Dim grp As cEcoPathGroupInput = Nothing
        Dim ti As cTaxonInfo = Nothing
        Dim iRow As Integer = 0
        Dim hgcGroup As EwEHierarchyGridCell = Nothing
        Dim dt As Date = Nothing

        Me.m_lUsedKeys.Clear()

        ' Create rows
        Me.RowsCount = 1
        For iGroup As Integer = 1 To Me.Core.nGroups

            iRow = Me.AddRow()

            grp = Me.Core.EcoPathGroupInputs(iGroup)

            hgcGroup = New EwEHierarchyGridCell()
            hgcGroup.Tag = grp

            Me(iRow, eColumnTypes.Hierarchy) = hgcGroup
            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, grp, eVarNameFlags.Name)
            Me(iRow, eColumnTypes.PropNorm) = New EwERowHeaderCell("")
            Me(iRow, eColumnTypes.Proportion) = New EwERowHeaderCell("")
            Me(iRow, eColumnTypes.LastUpdated) = New EwERowHeaderCell("")
            Me(iRow, eColumnTypes.Status) = New EwERowHeaderCell("")

            For iTaxon As Integer = 0 To Me.m_lTaxonInfo.Count - 1

                ti = Me.m_lTaxonInfo(iTaxon)
                If ti.Group = grp.Index Then

                    iRow = Me.AddRow()

                    hgcGroup.AddChildRow(iRow)
                    Me(iRow, eColumnTypes.Hierarchy) = New EwERowHeaderCell(hgcGroup.NumChildRows)
                    Me(iRow, eColumnTypes.Hierarchy).Tag = ti
                    Me(iRow, eColumnTypes.Name) = New EwERowHeaderCell(ti.Common)
                    Me(iRow, eColumnTypes.PropNorm) = New EwECell(ti.PropNormalized, GetType(Single), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.PropNorm).VisualModel = Me.m_vizPropNormalized
                    Me(iRow, eColumnTypes.Proportion) = New EwECell(ti.Proportion, GetType(Single))
                    Me(iRow, eColumnTypes.Proportion).Behaviors.Add(Me.EwEEditHandler)
                    Me(iRow, eColumnTypes.LastUpdated) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.Status) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)

                    Me.m_lUsedKeys.Add(ti.CodeTaxon)

                End If
            Next
        Next

        ' Populate rows
        For iRow = 1 To Me.RowsCount - 1
            Me.UpdateRow(iRow)
        Next iRow

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)

        Dim tag As Object = Me(iRow, eColumnTypes.Hierarchy).Tag
        Dim ti As cTaxonInfo = Nothing
        Dim vm As VisualModels.IVisualModel = Nothing
        Dim dt As Date = Nothing
        Dim strText As String = ""
        Dim iNumOpen As Integer = 0

        If Not TypeOf tag Is cTaxonInfo Then Return

        ti = DirectCast(tag, cTaxonInfo)

        Me(iRow, eColumnTypes.Name).Value = ti.Common
        Me(iRow, eColumnTypes.PropNorm).Value = ti.PropNormalized
        Me(iRow, eColumnTypes.Proportion).Value = ti.Proportion
        ' Last updated
        If (ti.LastUpdated > 0) Then
            strText = String.Format("{0:g}", cDateUtils.JulianToDate(ti.LastUpdated))
        Else
            strText = ""
        End If
        Me(iRow, eColumnTypes.LastUpdated).Value = strText

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

        ' Can only be proportion
        Dim ti As cTaxonInfo = Me.TaxonInfo(p.Row)
        If ti Is Nothing Then Return False
        ti.Proportion = CSng(Me(p.Row, p.Column).Value)
        Me.NormalizeProportions()

        For iRow As Integer = 1 To Me.RowsCount - 1
            Me.UpdateRow(iRow)
        Next
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

    Public Property SelectedTaxon() As ITaxonData
        Get
            Return Me.TaxonInfo(Me.SelectedRow)
        End Get
        Set(ByVal taxon As ITaxonData)
            If Not (TypeOf taxon Is cTaxonInfo) Then Return
            For iRow As Integer = 1 To Me.RowsCount - 1
                If Object.ReferenceEquals(TaxonInfo(iRow), taxon) Then
                    Me.SelectRow(iRow)
                    Return
                End If
            Next
        End Set
    End Property

    Public Property SelectedGroup() As cEcoPathGroupInput
        Get
            Dim iRow As Integer = Me.SelectedRow
            Dim tag As Object = Nothing

            If (iRow < 1) Then Return Nothing
            tag = Me(iRow, eColumnTypes.Hierarchy).Tag

            If (TypeOf tag Is cTaxonInfo) Then Return Me.Core.EcoPathGroupInputs(DirectCast(tag, cTaxonInfo).Group)
            If (TypeOf tag Is cEcoPathGroupInput) Then Return DirectCast(tag, cEcoPathGroupInput)
            Return Nothing
        End Get
        Set(ByVal value As cEcoPathGroupInput)
            Dim tag As Object = Nothing
            For iRow As Integer = 1 To RowsCount - 1
                tag = Me(iRow, eColumnTypes.Hierarchy).Tag
                If (TypeOf tag Is cEcoPathGroupInput) Then Me.SelectRow(iRow) : Return
            Next
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns an array of all available taxa.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Taxa() As ITaxonData()
        Get
            Return Me.m_lTaxonInfo.ToArray
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a taxon for the selected group.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub AddTaxon(Optional ByVal taxon As ITaxonData = Nothing)

        Dim ti As cTaxonInfo = Nothing
        Dim iRow As Integer = Nothing

        If Me.m_lUsedKeys.Contains(taxon.CodeTaxon) Then Return

        If (taxon Is Nothing) Then
            ti = New cTaxonInfo(Me.SelectedGroup)
            Me.m_lTaxonInfo.Add(ti)
        Else
            ti = New cTaxonInfo(taxon)
            ti.Group = Me.SelectedGroup.Index
            Me.m_lTaxonInfo.Add(ti)
            Me.NormalizeProportions()
        End If

        Me.m_lUsedKeys.Add(taxon.CodeTaxon)

        Me.UpdateGrid()
        Me.SelectedTaxon = ti

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move a taxon to a different group.
    ''' </summary>
    ''' <param name="iDirection"></param>
    ''' -----------------------------------------------------------------------
    Public Sub MoveTaxon(ByVal iDirection As Integer)
        Dim ti As cTaxonInfo = Me.TaxonInfo(Me.SelectedRow)
        If (ti Is Nothing) Then Return
        ti.Group += iDirection
        Me.UpdateGrid()
        Me.SelectedTaxon = ti
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the delete state of all selected rows
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ToggleDeleteRow()

        Dim sel As Selection = Me.Selection
        Dim ti As cTaxonInfo = Nothing

        For iRow As Integer = 1 To Me.RowsCount - 1
            If Me.Selection.ContainsRow(iRow) Then
                ti = Me.TaxonInfo(iRow)

                If (ti IsNot Nothing) Then
                    ti.FlaggedForDeletion = Not ti.FlaggedForDeletion

                    ' Check to see what is to happen to the MPA now
                    Select Case ti.Status

                        Case eItemStatusTypes.Original
                            ' Clear removed status 
                            Me.m_lTaxonInfoRemoved.Remove(ti)

                        Case eItemStatusTypes.Added
                            ' Remove new item
                            Me.m_lTaxonInfo.Remove(ti)

                        Case eItemStatusTypes.Removed
                            ' Set removed status
                            Me.m_lTaxonInfoRemoved.Add(ti)

                        Case eItemStatusTypes.Invalid
                            ' Set removed status
                            Me.m_lTaxonInfo.Remove(ti)

                    End Select

                    Me.NormalizeProportions()

                End If
            End If
        Next

        Me.UpdateGrid()
        Me.SelectedTaxon = ti

    End Sub

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
    Public Sub UpdateSelectedTaxon(ByVal taxon As ITaxonData)
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
    Public Function GetSearchTerm(Optional ByVal taxonSearch As ITaxonData = Nothing) As ITaxonData

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
    Public Function IsSearchTerm(ByVal taxon As ITaxonData) As Boolean
        Return (Object.ReferenceEquals(taxon, Me.m_tiSearch)) And _
               (Object.ReferenceEquals(Me.SelectedTaxon, Me.m_tiSearchLinked))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Search the grid for a data row with the same <see cref="ITaxonData.SourceKey">source key</see>.
    ''' </summary>
    ''' <param name="taxon">The <see cref="ITaxonData">taxon data</see> to find</param>
    ''' <returns>A row number, or -1 if no such key was found.</returns>
    ''' -----------------------------------------------------------------------
    Public Function FindTaxonRow(ByVal taxon As ITaxonData) As Integer

        For iRow As Integer = 1 To Me.RowsCount - 1
            Dim ti As cTaxonInfo = Me.TaxonInfo(iRow)
            If ti IsNot Nothing Then
                If ti.Taxon IsNot Nothing Then
                    If ti.SourceKey = taxon.SourceKey Then Return iRow
                End If
            End If
        Next
        Return -1
    End Function

#End Region ' Search

#Region " Apply changes "

    Public Sub NormalizeProportions()

        Dim asTotal(Me.Core.nGroups) As Single
        Dim aiTotal(Me.Core.nGroups) As Integer
        Dim ti As cTaxonInfo = Nothing
        Dim iTaxon As Integer = 0

        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = Me.m_lTaxonInfo(iTaxon)
            If (ti.Status <> eItemStatusTypes.Removed) Then
                asTotal(ti.Group) += ti.Proportion
                aiTotal(ti.Group) += 1
            End If
        Next

        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = Me.m_lTaxonInfo(iTaxon)
            If (ti.Status <> eItemStatusTypes.Removed) Then
                ' Has a total of 0?
                If (asTotal(ti.Group) = 0.0!) Then
                    ' #Yes: redistribute values
                    ti.PropNormalized = 1.0! / aiTotal(ti.Group)
                Else
                    ti.PropNormalized = ti.Proportion / asTotal(ti.Group)
                End If
            End If
        Next

    End Sub

    Public Function Apply() As Boolean

        Dim strPrompt As String = ""
        Dim bConfigurationChanged As Boolean = False
        Dim ti As cTaxonInfo = Nothing
        Dim taxon As cTaxon = Nothing
        Dim iTaxon As Integer = 0
        Dim bSuccess As Boolean = True

        Me.NormalizeProportions()

        ' Assess Taxon changes
        For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
            ti = DirectCast(Me.m_lTaxonInfo(iTaxon), cTaxonInfo)
            ' Check this Taxon is newly added
            If Object.ReferenceEquals(ti.Taxon, Nothing) Then
                bConfigurationChanged = True
            End If
            ' Check if this Taxon is an existing Taxon that has been moved
            If Not Object.ReferenceEquals(ti.Taxon, Nothing) Then
                If ((iTaxon + 1) <> ti.Taxon.Index) Then
                    bConfigurationChanged = True
                End If
            End If
        Next iTaxon

        ' Assess Taxons to remove
        strPrompt = ""
        For iTaxon = 0 To Me.m_lTaxonInfoRemoved.Count - 1
            ti = DirectCast(Me.m_lTaxonInfoRemoved(iTaxon), cTaxonInfo)
            If (Not Object.ReferenceEquals(ti.Taxon, Nothing)) Then

                strPrompt = String.Format("Are you sure you want to delete taxonomy entry '{0}'?", ti.Common)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.No
                        ' Do not delete this Taxon
                        ti.Confirmed = False
                    Case MsgBoxResult.Yes
                        ' Delete this Taxon
                        ti.Confirmed = True
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            End If
        Next iTaxon

        ' Handle added and removed items
        If (bConfigurationChanged) Then

            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

            cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

            Dim htTaxonID As New Dictionary(Of cTaxonInfo, Integer)
            Dim iDBID As Integer = Nothing

            ' Add new Taxons
            For iTaxon = 0 To Me.m_lTaxonInfo.Count - 1
                ti = Me.m_lTaxonInfo(iTaxon)
                If (ti.IsNew) Then
                    Dim igt As Integer = iTaxon + 1
                    bSuccess = bSuccess And Me.Core.AddTaxon(ti.Group, ti, ti.Proportion, iDBID)
                    ' Map this new ID during update
                    htTaxonID.Add(ti, iDBID)
                End If
            Next

            ' Remove deleted (and confirmed) Taxons
            Dim iTaxonRemove As Integer = 0
            For iTaxon = 0 To Me.m_lTaxonInfoRemoved.Count - 1
                ti = DirectCast(Me.m_lTaxonInfoRemoved(iTaxonRemove), cTaxonInfo)
                If (Not Object.ReferenceEquals(ti.Taxon, Nothing)) And (ti.Confirmed = True) Then
                    If (Me.Core.RemoveTaxon(ti.Taxon.Index)) Then
                        Me.m_lTaxonInfo.Remove(ti)
                        Me.m_lTaxonInfoRemoved.Remove(ti)
                    Else
                        bSuccess = False
                        iTaxonRemove += 1
                    End If
                End If
            Next

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

            ' Test whether new Taxons were loaded correctly
            Debug.Assert(Me.m_lTaxonInfo.Count = Me.Core.nTaxon, "Dialog and core out of sync on Taxons")
        End If

        ' Update any changed taxa
        For Each ti In Me.m_lTaxonInfo
            ti.ApplyChanges()
        Next

        Return bSuccess

    End Function

#End Region ' Apply changes

#End Region ' Public bits

End Class
