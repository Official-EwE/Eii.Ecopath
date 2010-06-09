#Region " Imports "

Option Strict On
Imports SourceGrid2
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities

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

    ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
    ''' to trap cell edit events locally in this grid. These events are essential
    ''' for keeping the local administration up to date.</summary>
    Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
    ''' <summary>Update lock, used to distinguish between code updates and
    ''' user updates of grid cells. When grid cells are updated from within
    ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
    Private m_iUpdateLock As Integer = 0
    ''' <summary>Visual model to display original groups.</summary>
    Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display newly created groups.</summary>
    Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display groups that are about be deleted.</summary>
    Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        Hierarchy = 0
        Name
        Proportion
        LastUpdated
        Status
    End Enum

#End Region ' Privates

#Region " Private helper classes "

    Private Class cTaxonInfo
        Implements ITaxonData

        Private m_taxon As cTaxon = Nothing
        Private m_strCode3A As String
        Private m_strCodeISSCAAP As String
        Private m_strCodeTaxon As String
        Private m_strClass As String
        Private m_strOrder As String
        Private m_strGenus As String
        Private m_strFamily As String
        Private m_strSpecies As String
        Private m_strCommon As String
        Private m_strSource As String
        Private m_strKey As String
        Private m_sProportion As Single
        ''' <summary>Index of the ecopath group that this taxon contributes to.</summary>
        Private m_iGroup As Integer = Nothing

        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a Layer in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original
        ''' <summary>States whether this info has been updated from an external source</summary>
        Private m_bUpdated As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an new taxon administrative unit for an existing group.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal group As cEcoPathGroupInput)
            Me.m_iGroup = group.Index
            Me.m_sProportion = 1.0!
            Me.m_strCommon = group.Name
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create an administrative unit for an existing taxon.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal taxon As cTaxon)
            Me.m_taxon = taxon
            Me.m_strClass = taxon.Class
            Me.m_strCode3A = taxon.Code3A
            Me.m_strCodeISSCAAP = taxon.CodeISSCAAP
            Me.m_strCodeTaxon = taxon.CodeTaxon
            Me.m_strCommon = taxon.Name
            Me.m_strFamily = taxon.Family
            Me.m_strGenus = taxon.Genus
            Me.m_strKey = taxon.SourceKey
            Me.m_strOrder = taxon.Order
            Me.m_strSource = taxon.Source
            Me.m_strSpecies = taxon.Species
            Me.m_iGroup = taxon.Group
            Me.m_sProportion = taxon.Proportion
            Me.m_bUpdated = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update this unit with new Taxonomy data.
        ''' </summary>
        ''' <param name="taxon"></param>
        ''' -------------------------------------------------------------------
        Public Sub Update(ByVal taxon As ITaxonData)
            Me.m_strClass = taxon.Class
            Me.m_strCode3A = taxon.Code3A
            Me.m_strCodeISSCAAP = taxon.CodeISSCAAP
            Me.m_strCodeTaxon = taxon.CodeTaxon
            Me.m_strCommon = taxon.Common
            Me.m_strFamily = taxon.Family
            Me.m_strGenus = taxon.Genus
            Me.m_strKey = taxon.SourceKey
            Me.m_strOrder = taxon.Order
            Me.m_strSource = taxon.Source
            Me.m_strSpecies = taxon.Species
            Me.m_bUpdated = True
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

        Public Property [Class]() As String Implements EwEUtils.Core.ITaxonData.Class
            Get
                Return m_strClass
            End Get
            Set(ByVal value As String)
                m_strClass = value
            End Set
        End Property

        Public Property Code3A() As String Implements EwEUtils.Core.ITaxonData.Code3A
            Get
                Return m_strCode3A
            End Get
            Set(ByVal value As String)
                m_strCode3A = value
            End Set
        End Property

        Public Property CodeISSCAAP() As String Implements EwEUtils.Core.ITaxonData.CodeISSCAAP
            Get
                Return m_strCodeISSCAAP
            End Get
            Set(ByVal value As String)
                Me.m_strCodeISSCAAP = value
            End Set
        End Property

        Public Property CodeTaxon() As String Implements EwEUtils.Core.ITaxonData.CodeTaxon
            Get
                Return Me.m_strCodeTaxon
            End Get
            Set(ByVal value As String)
                Me.m_strCodeTaxon = value
            End Set
        End Property

        Public Property Common() As String Implements EwEUtils.Core.ITaxonData.Common
            Get
                Return Me.m_strCommon
            End Get
            Set(ByVal value As String)
                Me.m_strCommon = value
            End Set
        End Property

        Public Property Family() As String Implements EwEUtils.Core.ITaxonData.Family
            Get
                Return Me.m_strFamily
            End Get
            Set(ByVal value As String)
                Me.m_strFamily = value
            End Set
        End Property

        Public Property Genus() As String Implements EwEUtils.Core.ITaxonData.Genus
            Get
                Return Me.m_strGenus
            End Get
            Set(ByVal value As String)
                Me.m_strGenus = value
            End Set
        End Property

        Public Property LastUpdated() As Single Implements EwEUtils.Core.ITaxonData.LastUpdated
            Get
                Return 0L
            End Get
            Set(ByVal value As Single)
                ' NOP
            End Set
        End Property

        Public Property Order() As String Implements EwEUtils.Core.ITaxonData.Order
            Get
                Return Me.m_strOrder
            End Get
            Set(ByVal value As String)
                Me.m_strOrder = value
            End Set
        End Property

        Public Property Source() As String Implements EwEUtils.Core.ITaxonData.Source
            Get
                Return Me.m_strSource
            End Get
            Set(ByVal value As String)
                Me.m_strSource = value
            End Set
        End Property

        Public Property SourceKey() As String Implements EwEUtils.Core.ITaxonData.SourceKey
            Get
                Return Me.m_strKey
            End Get
            Set(ByVal value As String)
                Me.m_strKey = value
            End Set
        End Property

        Public Property Species() As String Implements EwEUtils.Core.ITaxonData.Species
            Get
                Return Me.m_strSpecies
            End Get
            Set(ByVal value As String)
                Me.m_strSpecies = value
            End Set
        End Property

    End Class

#End Region ' Private helper classes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        MyBase.New()

        ' Set up visual models for reflecting group modification status
        With Me.m_vmOriginal
            .ForeColor = Color.FromArgb(255, 0, 0, 0)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

        With Me.m_vmAdded
            .ForeColor = Color.FromArgb(255, 8, 128, 12)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

        With Me.m_vmRemoved
            .ForeColor = Color.FromArgb(255, 255, 22, 12)
            .TextAlignment = ContentAlignment.MiddleCenter
            .MakeReadOnly()
        End With

    End Sub

#Region " Grid interaction "

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
        Me(0, eColumnTypes.Proportion) = New EwEColumnHeaderCell("Proportion")
        Me(0, eColumnTypes.LastUpdated) = New EwEColumnHeaderCell("Last updated")
        Me(0, eColumnTypes.Status) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

        Me.FixedColumns = 1

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to first create a snapshot of the group/stanza configuration
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

        ' Brute-force update grid
        Me.UpdateGrid()

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.AutoSizeColumnRange(1, Me.ColumnsCount - 1, 1, Me.RowsCount - 1)
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
    Public Sub UpdateGrid()

        Dim grp As cEcoPathGroupInput = Nothing
        Dim ti As cTaxonInfo = Nothing
        Dim iRow As Integer = 0
        Dim hgcGroup As EwEHierarchyGridCell = Nothing
        Dim dt As Date = Nothing

        ' Create rows
        Me.RowsCount = 1
        For iGroup As Integer = 1 To Me.Core.nGroups

            iRow = Me.AddRow()

            grp = Me.Core.EcoPathGroupInputs(iGroup)

            hgcGroup = New EwEHierarchyGridCell()
            hgcGroup.Tag = grp

            Me(iRow, eColumnTypes.Hierarchy) = hgcGroup
            Me(iRow, eColumnTypes.Name) = New PropertyRowHeaderCell(Me.PropertyManager, grp, eVarNameFlags.Name)
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
                    Me(iRow, eColumnTypes.Name) = New EwECell(ti.Common, GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.Proportion) = New EwECell(ti.Proportion, GetType(Single))
                    Me(iRow, eColumnTypes.LastUpdated) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
                    Me(iRow, eColumnTypes.Status) = New EwECell("", GetType(String), cStyleGuide.eStyleFlags.NotEditable)
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
        Dim ri As RowInfo = Nothing
        Dim aCells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim dt As Date = Nothing
        Dim strText As String = ""
        Dim iNumOpen As Integer = 0

        If Not TypeOf tag Is cTaxonInfo Then Return

        'Me.AllowUpdates = False

        ti = DirectCast(tag, cTaxonInfo)
        ri = Me.Rows(iRow)

        aCells = ri.GetCells()

        ' Set name
        pos = New Position(iRow, eColumnTypes.Name)
        aCells(eColumnTypes.Name).SetValue(pos, ti.Common)

        ' Set proportion
        pos = New Position(iRow, eColumnTypes.Proportion)
        aCells(eColumnTypes.Proportion).SetValue(pos, ti.Proportion)

        ' Lst updated
        If (ti.LastUpdated > 0) Then
            strText = String.Format("{0:g}", cDateUtils.FromJulianDate(ti.LastUpdated))
        Else
            strText = ""
        End If
        aCells(eColumnTypes.LastUpdated).SetValue(pos, strText)

        Select Case ti.Status
            Case eItemStatusTypes.Original
                vm = Me.m_vmOriginal
                strText = ""
            Case eItemStatusTypes.Added
                vm = Me.m_vmAdded
                strText = My.Resources.GENERIC_ITEMSTATUS_CREATEPENDING
            Case eItemStatusTypes.Removed
                vm = Me.m_vmRemoved
                strText = My.Resources.GENERIC_ITEMSTATUS_DELETEPENDING
        End Select

        ' Set modification status
        pos = New Position(iRow, eColumnTypes.Status)
        aCells(eColumnTypes.Status).VisualModel = vm
        aCells(eColumnTypes.Status).SetValue(pos, strText)

        'Me.AllowUpdates = True

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Called Update local admin based on cell value changes.
    ''' </summary>
    ''' <returns>
    ''' True if the value change is allowed, False to block the value change.
    ''' </returns>
    ''' <remarks>
    ''' This method differs from OnCellValueEdited; during a cell value 
    ''' change notification (at the end of an edit operation) it is unsafe
    ''' to modify the value of the cell being edited. However, the end edit 
    ''' event will not be triggered for particular specialized cells which
    ''' makes this method mandatory. We once again apologize for the confusion; )
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Overrides Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean

        Return True

    End Function

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

        Return True

    End Function

    Private Function TaxonInfo(ByVal iRow As Integer) As cTaxonInfo
        Dim tag As Object = Nothing
        If (iRow <= 1) Then Return Nothing
        tag = Me(iRow, eColumnTypes.Hierarchy).Tag
        If Not (TypeOf tag Is cTaxonInfo) Then Return Nothing
        Return DirectCast(tag, cTaxonInfo)
    End Function

#End Region ' Grid interaction

#Region " Public bits "

    Public Property SelectedTaxon() As ITaxonData
        Get
            Return Me.TaxonInfo(Me.SelectedRow)
        End Get
        Set(ByVal taxon As ITaxonData)
            If Not (TypeOf taxon Is cTaxonInfo) Then Return
            For iRow As Integer = 1 To Me.RowsCount
                If Object.ReferenceEquals(TaxonInfo(iRow), taxon) Then
                    Me.SelectRow(iRow)
                    Return
                End If
            Next
        End Set
    End Property

    Public ReadOnly Property SelectedGroup() As cEcoPathGroupInput
        Get
            Dim iRow As Integer = Me.SelectedRow
            Dim tag As Object = Nothing

            If (iRow < 1) Then Return Nothing
            tag = Me(iRow, eColumnTypes.Hierarchy).Tag

            If (TypeOf tag Is cTaxonInfo) Then Return Me.Core.EcoPathGroupInputs(DirectCast(tag, cTaxonInfo).Group)
            If (TypeOf tag Is cEcoPathGroupInput) Then Return DirectCast(tag, cEcoPathGroupInput)
            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' Add a taxon for the selected group.
    ''' </summary>
    Public Sub AddTaxon()
        Dim ti As New cTaxonInfo(Me.SelectedGroup)
        Me.m_lTaxonInfo.Add(ti)
        Me.UpdateGrid()
        Me.SelectedTaxon = ti
    End Sub

#End Region ' Public bits

End Class
