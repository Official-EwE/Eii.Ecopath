#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridEditRegions
        : Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first Region</summary>
        Private Const iFIRSTREGIONROW As Integer = 1

        ''' <summary>The <see cref="cCore">Core</see> currently being modified.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>List of active Regions.</summary>
        Private m_alRegions As New List(Of RegionInfo)
        ''' <summary>List of removed Regions.</summary>
        Private m_alRegionsRemoved As New List(Of RegionInfo)
        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local Region administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Visual model to display original Regions.</summary>
        Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display newly created Regions.</summary>
        Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display Regions that are about be deleted.</summary>
        Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            RegionIndex = 0
            RegionName
            RegionStatus
        End Enum

        ''' <summary>Enumerated type defining how regions should be allocated, if at all.</summary>
        Private Enum AllocationModeType As Integer
            None = 0
            Habitat = 1
            Cell = 2
        End Enum

        Private m_allocateRegionsFlag As AllocationModeType = AllocationModeType.None

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="cEcospaceRegion">Region</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new Regions. If this class has its
        ''' <see cref="RegionInfo.Region">Region</see> parameter set, a real live
        ''' Region is represented. If this parameter is not set, a new Region is
        ''' represented.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class RegionInfo

            ''' <summary><see cref="cEcospaceRegion">cEcospaceRegion</see> associated with this Region, if any.</summary>
            Private m_Region As cEcospaceRegion = Nothing
            ''' <summary>Name for this Region.</summary>
            Private m_strName As String = ""
            ''' <summary>Flag stating whether a user action is confirmed</summary>
            Private m_bConfirmed As Boolean = True
            ''' <summary>The status of a Region in the interface.</summary>
            Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original
            ''' <summary>Habitat to allocate this region to, if any</summary>
            Private m_hab As cEcospaceHabitat = Nothing
            ''' <summary>Cell (col, row) to allocate this region to, if any</summary>
            Private m_ptCell As New Point(0, 0)

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="Region">The <see cref="cEcospaceRegion">cEcospaceRegion</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' Region currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal Region As cEcospaceRegion)
                Debug.Assert(Region IsNot Nothing)
                Me.m_Region = Region
                Me.m_strName = Region.Name
                Me.m_status = AddRemoveItemStatus.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String)
                Me.m_Region = Nothing
                Me.m_strName = strName
                Me.m_status = AddRemoveItemStatus.Added
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the name of this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Name() As String
                Get
                    Return Me.m_strName
                End Get
                Set(ByVal value As String)
                    Me.m_strName = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cEcospaceRegion">EwE Region</see> associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Region() As cEcospaceRegion
                Get
                    Return Me.m_Region
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
            ''' for the region object.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Status() As AddRemoveItemStatus
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
            ''' States whether the Region has changed.
            ''' </summary>
            ''' <returns>
            ''' True when Region <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged() As Boolean
                If Me.IsNew() Then Return False
                Return (Me.m_Region.Name <> Me.m_strName)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether a Region is to be added
            ''' </summary>
            ''' <returns>
            ''' True if new.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsNew() As Boolean
                Return (Me.Region Is Nothing)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this region is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = AddRemoveItemStatus.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Not Me.IsNew Then
                        If bDelete Then
                            Me.m_status = AddRemoveItemStatus.Removed
                        Else
                            Me.m_status = AddRemoveItemStatus.Original
                        End If
                    Else
                        If bDelete Then
                            Me.m_status = AddRemoveItemStatus.Invalid
                        Else
                            Me.m_status = AddRemoveItemStatus.Added
                        End If
                    End If
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the habitat this region is associated with.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Habitat() As cEcospaceHabitat
                Get
                    Return Me.m_hab
                End Get
                Set(ByVal value As cEcospaceHabitat)
                    Me.m_hab = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the cell position (Column, Row) this region is associated with.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Cell() As Point
                Get
                    Return Me.m_ptCell
                End Get
                Set(ByVal value As Point)
                    Me.m_ptCell = value
                End Set
            End Property

        End Class

#End Region ' Helper classes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()

            MyBase.New()
            Me.m_core = cCore.GetInstance()

            ' Set up visual models for reflecting Region modification status
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

            ' JS 15Apr07: there will be no context menu item until we have a better idea
            Me.ContextMenu = Nothing

            ' Redim columns
            Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

            ' Region index cell
            Me(0, eColumnTypes.RegionIndex) = New EwEColumnHeaderCell()
            ' Region name cell, editable this time
            Me(0, eColumnTypes.RegionName) = New EwEColumnHeaderCell(My.Resources.HEADER_REGION)

            ' Region index cell
            Me(0, eColumnTypes.RegionStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

            ' Fix index column only; Region name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.None
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the Region/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            ' Get the core reference
            Dim core As cCore = cCore.GetInstance()
            Dim Region As cEcospaceRegion = Nothing
            Dim ri As RegionInfo = Nothing

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of Region configuration 
            For iRegion As Integer = 1 To core.nRegions
                Region = core.EcospaceRegions(iRegion)
                ri = New RegionInfo(Region)
                Me.m_alRegions.Add(ri)
            Next

            ' Brute-force update grid
            UpdateGrid()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Brute-force resize the gird if necessary, and repopulate with data from 
        ''' the local administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub UpdateGrid()

            Dim ri As RegionInfo = Nothing
            Dim rowInfo As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_alRegions.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.RegionIndex) = ewec

                Me(iRow, eColumnTypes.RegionName) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.RegionName).Behaviors.Add(m_bm)

                ' Status
                vm = New VisualModels.Common()
                vm.ImageAlignment = ContentAlignment.MiddleCenter
                Me(iRow, eColumnTypes.RegionStatus) = New Cells.Real.Cell()
                Dim dm As New DataModels.DataModelBase(GetType(String))
                dm.EditableMode = EditableMode.None
                Me(iRow, eColumnTypes.RegionStatus).DataModel = dm
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_alRegions.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTREGIONROW)
            End While

            ' Sanity check whether grid can accomodate all Regions + header
            Debug.Assert(Me.Rows.Count = Me.m_alRegions.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_alRegions.Count
                UpdateRow(iRow)
            Next iRow

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of the Row with the given index.
        ''' </summary>
        ''' <param name="iRow">The index of the row to refresh.</param>
        ''' -----------------------------------------------------------------------
        Private Sub UpdateRow(ByVal iRow As Integer)

            Dim ri As RegionInfo = Nothing
            Dim rowInfo As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim strText As String = ""

            Me.AllowUpdates = False

            ri = DirectCast(Me.m_alRegions(iRow - iFIRSTREGIONROW), RegionInfo)
            rowInfo = Me.Rows(iRow)

            rowInfo.Tag = ri
            aCells = rowInfo.GetCells()

            pos = New Position(iRow, eColumnTypes.RegionIndex)
            aCells(eColumnTypes.RegionIndex).SetValue(pos, CInt(iRow))

            pos = New Position(iRow, eColumnTypes.RegionName)
            aCells(eColumnTypes.RegionName).SetValue(pos, CStr(ri.Name))

            Select Case ri.Status
                Case AddRemoveItemStatus.Original
                    vm = Me.m_vmOriginal
                    strText = ""
                Case AddRemoveItemStatus.Added
                    vm = Me.m_vmAdded
                    strText = My.Resources.GENERIC_ITEMSTATUS_CREATEPENDING
                Case AddRemoveItemStatus.Removed
                    vm = Me.m_vmRemoved
                    strText = My.Resources.GENERIC_ITEMSTATUS_DELETEPENDING
            End Select

            pos = New Position(iRow, eColumnTypes.RegionStatus)
            aCells(eColumnTypes.RegionStatus).VisualModel = vm
            aCells(eColumnTypes.RegionStatus).SetValue(pos, strText)

            Me.AllowUpdates = True

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

            If Not Me.AllowUpdates Then Return True

            Dim ri As RegionInfo = DirectCast(Me.m_alRegions(p.Row - 1), RegionInfo)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.RegionName
                    ' JS: Handled in OnCellEdited()
                    ' ri.Name = CStr(cell.GetValue(p))

            End Select

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

            If Not Me.AllowUpdates Then Return True

            Dim ri As RegionInfo = DirectCast(Me.m_alRegions(p.Row - 1), RegionInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.RegionIndex
                    ' Not possible

                Case eColumnTypes.RegionName
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iRegion As Integer = 0 To Me.m_alRegions.Count - 1
                        Dim giTemp As RegionInfo = DirectCast(Me.m_alRegions(iRegion), RegionInfo)
                        ' Does name already exist?
                        If (Not Object.ReferenceEquals(giTemp, ri)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    ri.Name = strName

            End Select

            Return True

        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Cell click handler, called in response to clicking button-like cells.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnCellClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)

            Select Case DirectCast(p.Column, eColumnTypes)
            End Select

        End Sub

#End Region ' Grid interaction

#Region " Row manipulation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Delete a row from the grid
        ''' </summary>
        ''' <param name="iRow">The index of the row to delete.</param>
        ''' -----------------------------------------------------------------------
        Public Sub ToggleDeleteRow(Optional ByVal iRow As Integer = -1)

            If iRow = -1 Then iRow = Me.SelectedRow

            Dim iRegion As Integer = iRow - iFIRSTREGIONROW
            Dim ri As RegionInfo = Nothing

            ' Validate
            If iRegion < 0 Then Return

            ri = DirectCast(Me.m_alRegions(iRegion), RegionInfo)
            Me.RemoveRegion(ri, Not ri.FlaggedForDeletion, True)

        End Sub

        ''' <summary>
        ''' States whether a row holds a region.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsRegionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTREGIONROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the region on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsRegionRow(iRow) Then Return False

            Dim iRegion As Integer = iRow - iFIRSTREGIONROW
            Dim ri As RegionInfo = DirectCast(Me.m_alRegions(iRegion), RegionInfo)

            Return ri.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new region.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return

            Dim lstrRegions As New List(Of String)
            Dim ri As RegionInfo = Nothing

            ' Collect all current region names
            For Each ri In Me.m_alRegions
                lstrRegions.Add(ri.Name)
            Next

            Me.CreateRegion(String.Format(My.Resources.DEFAULT_NEWREGION_NUM, _
                    cStringUtils.GetNextNumber(lstrRegions.ToArray(), My.Resources.DEFAULT_NEWREGION_NUM)))
        End Sub

        ''' <summary>
        ''' States whether a row can be inserted at the indicated position.
        ''' </summary>
        Public Function CanAddRow() As Boolean
            Return True
        End Function

        Public Sub CreateHabitatRegions()

            Dim iHab As Integer = 0
            Dim hab As cEcospaceHabitat = Nothing
            Dim ri As RegionInfo = Nothing

            ' Delete all existing regions
            Dim ari As RegionInfo() = Me.m_alRegions.ToArray
            For Each ri In ari
                Me.RemoveRegion(ri, False, False)
            Next

            ' Create new regions for each habitat
            For iHab = 1 To Me.m_core.nHabitats - 1
                hab = Me.m_core.EcospaceHabitats(iHab)
                ri = Me.CreateRegion(hab.Name, False)
                ri.Habitat = hab
            Next

            Me.m_allocateRegionsFlag = AllocationModeType.Habitat

            Me.UpdateGrid()

        End Sub

        Public Sub CreateCellRegions()

            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim ri As RegionInfo = Nothing

            ' ToDo_JS: Prompt for confirmation if this will create an insane pile of regions?

            ' Delete all existing regions
            Dim ari As RegionInfo() = Me.m_alRegions.ToArray
            For Each ri In ari
                Me.RemoveRegion(ri, False, False)
            Next

            ' Create a region for each cell
            For iCol = 1 To bm.InCol
                For iRow = 1 To bm.InRow
                    ri = Me.CreateRegion(String.Format("({0}, {1})", iCol, iRow), False)
                    ri.Cell = New Point(iCol, iRow)
                Next iRow
            Next iCol

            Me.m_allocateRegionsFlag = AllocationModeType.Cell

            Me.UpdateGrid()

        End Sub

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a new region.
        ''' </summary>
        ''' <returns>
        ''' The created <see cref="RegionInfo">region</see>.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function CreateRegion(ByVal strName As String, Optional ByVal bUpdate As Boolean = True) As RegionInfo

            Dim ri As RegionInfo = New RegionInfo(strName)
            Me.m_alRegions.Add(ri)
            If (bUpdate = True) Then
                Me.UpdateGrid()
                Me.SelectRow(ri)
            End If
            Return ri

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a region.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub RemoveRegion(ByVal ri As RegionInfo, ByVal bRemove As Boolean, Optional ByVal bUpdate As Boolean = True)

            ' Toggle 'flagged for deletion' flag
            ri.FlaggedForDeletion = Not ri.FlaggedForDeletion

            ' Check to see what is to happen to the Region now
            Select Case ri.Status

                Case AddRemoveItemStatus.Original
                    ' Clear removed status of the Region
                    Me.m_alRegionsRemoved.Remove(ri)

                Case AddRemoveItemStatus.Added
                    ' Clear removed status of the Region
                    Me.m_alRegionsRemoved.Remove(ri)

                Case AddRemoveItemStatus.Removed
                    ' Set removed status
                    Me.m_alRegionsRemoved.Add(ri)

                Case AddRemoveItemStatus.Invalid
                    ' Set removed status
                    Me.m_alRegions.Remove(ri)

            End Select

            If bUpdate Then Me.UpdateGrid()

        End Sub

#End Region ' Internals

#End Region ' Row manipulation 

#Region " Admin "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update lock, should be set when modifying cell values from the code
        ''' to prevent recursive update/notification loops.
        ''' </summary>
        ''' <returns>True when no update lock is active.</returns>
        ''' <remarks>
        ''' Update locks are cumulative: setting this lock twice will require 
        ''' clearing it twice to allow updates to happen.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Property AllowUpdates() As Boolean
            Get
                Return (Me.m_iUpdateLock = 0)
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    Me.m_iUpdateLock += 1
                Else
                    Me.m_iUpdateLock -= 1
                End If
            End Set
        End Property

#Region " Selection extension "

        Public Function SelectedRow() As Integer

            Dim iSelectedRow As Integer = -1
            Dim selection As SourceGrid2.Selection = Me.Selection
            Dim arSelection As SourceGrid2.Range = Nothing

            If selection Is Nothing Then Return iSelectedRow
            If selection.Count = 0 Then Return iSelectedRow

            arSelection = selection.Item(0)
            iSelectedRow = arSelection.Start.Row
            Return iSelectedRow

        End Function

        Public Sub SelectRow(ByVal iRow As Integer)

            ' Clear current selection
            If Me.Selection IsNot Nothing Then
                Dim r As SourceGrid2.Range = Me.Selection.GetRange()
                If Not r.IsEmpty Then
                    Me.Selection.RemoveRange(r)
                End If
            End If
            Me.Selection.AddRange(New SourceGrid2.Range(iRow, 0, iRow, Me.ColumnsCount))

            ' Make sure selected row is visible
            Me.ShowCell(New Position(iRow, 0))
        End Sub

        Private Sub SelectRow(ByVal ri As RegionInfo)
            For iRegion As Integer = 0 To Me.m_alRegions.Count - 1
                If Object.ReferenceEquals(Me.m_alRegions(iRegion), ri) Then
                    Me.SelectRow(iRegion + iFIRSTREGIONROW)
                End If
            Next
        End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Validation "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; validates the content of the grid.
        ''' </summary>
        ''' <returns>True when the content of the grid depicts a valid
        ''' Region configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean

            '' Check if the user is about to delete all fleets - one should remain
            'If Me.m_alRegionsRemoved.Count = Me.m_alRegions.Count Then
            '    MsgBox(My.Resources.ECOPATH_EDITREGION_PROMPT_CANNOTDELETEALL, _
            '            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, _
            '            My.Resources.ECOPATH_EDITREGION_CONFIRMDELETE_CAPTION)
            '    Return False
            'End If

            Return True

        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bRegionsChanged As Boolean = False
            Dim ri As RegionInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim Region As cEcospaceRegion = Nothing
            Dim iRegion As Integer = 0
            Dim bSuccess As Boolean = True

            ' Habitat to region mapping
            Dim dtHabitatIDToRegionID As New Dictionary(Of Integer, Integer)
            Dim dtCellToRegionID As New Dictionary(Of Point, Integer)

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess Region changes
            For iRegion = 0 To Me.m_alRegions.Count - 1
                ri = DirectCast(Me.m_alRegions(iRegion), RegionInfo)
                bConfigurationChanged = bConfigurationChanged Or ri.IsNew()
                bRegionsChanged = bRegionsChanged Or ri.IsChanged()
            Next iRegion

            If Me.m_alRegionsRemoved.Count > 5 Then

                strPrompt = String.Format(My.Resources.ECOSPACE_EDITREGION_CONFIRMDELETENUM_PROMPT, Me.m_alRegionsRemoved.Count)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.Yes
                        ' Confirm all regions
                        For Each ri In Me.m_alRegionsRemoved
                            ri.Confirmed = True
                        Next
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            Else
                ' Assess Regions to remove
                For iRegion = 0 To Me.m_alRegionsRemoved.Count - 1
                    ri = DirectCast(Me.m_alRegionsRemoved(iRegion), RegionInfo)
                    If (Not ri.IsNew()) Then

                        strPrompt = String.Format(My.Resources.ECOSPACE_EDITREGION_CONFIRMDELETE_PROMPT, ri.Name)

                        Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                            Case MsgBoxResult.Cancel
                                ' Abort Apply process
                                Return False
                            Case MsgBoxResult.No
                                ' Do not delete this Region
                                ri.Confirmed = False
                            Case MsgBoxResult.Yes
                                ' Delete this Region
                                ri.Confirmed = True
                                bConfigurationChanged = True
                            Case Else
                                ' Unexpected anwer: assert
                                Debug.Assert(False)
                        End Select

                    End If
                Next iRegion
            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

                cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

                ' Add new Regions
                For iRegion = 0 To Me.m_alRegions.Count - 1
                    ri = DirectCast(Me.m_alRegions(iRegion), RegionInfo)
                    If (ri.IsNew()) Then
                        bSuccess = bSuccess And Me.m_core.AddEcospaceRegion(ri.Name, iDBID)

                        ' Prepare mapping
                        Select Case Me.m_allocateRegionsFlag
                            Case AllocationModeType.None
                            Case AllocationModeType.Habitat
                                Dim hab As cEcospaceHabitat = ri.Habitat
                                If (hab IsNot Nothing) Then dtHabitatIDToRegionID(CInt(hab.GetVariable(eVarNameFlags.DBID))) = iDBID
                            Case AllocationModeType.Cell
                                Dim pt As Point = ri.Cell
                                If ((pt.X > 0) And (pt.Y > 0)) Then dtCellToRegionID(pt) = iDBID
                        End Select
                    End If
                Next

                ' Remove deleted (and confirmed) Regions
                Dim iRegionRemove As Integer = 0
                For iRegion = 0 To Me.m_alRegionsRemoved.Count - 1
                    ri = DirectCast(Me.m_alRegionsRemoved(iRegionRemove), RegionInfo)

                    ' Sanity check
                    Debug.Assert(Not ri.IsNew())

                    If (ri.Confirmed()) Then
                        ' Find region to remove
                        If (Me.m_core.RemoveEcospaceRegion(ri.Region)) Then
                            Me.m_alRegions.Remove(ri)
                            Me.m_alRegionsRemoved.Remove(ri)
                        Else
                            bSuccess = False
                            iRegionRemove += 1
                        End If
                    End If
                Next iRegion

                Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)
                cApplicationStatusNotifier.SetStatusText("", TriState.False)

                ' Test whether new Regions were loaded correctly
                Debug.Assert(Me.m_alRegions.Count = Me.m_core.nRegions, "Dialog and core out of sync on Regions")
            End If

            ' Update core objects
            If (bRegionsChanged) Then
                ' For each local region admin unit
                For iRegion = 0 To Me.m_alRegions.Count - 1
                    ' Get local admin unit
                    ri = DirectCast(Me.m_alRegions(iRegion), RegionInfo)
                    ' Has it changed?
                    If ri.IsChanged() Then
                        ' Find core region with same BDID (cannot use cached cEcospaceRegion instances since the core has reloaded)
                        Dim bFound As Boolean = False
                        ' For every core region instance
                        For iRegionTest As Integer = 1 To Me.m_core.nRegions - 1
                            ' Get core region instance
                            Dim RegionTest As cEcospaceRegion = Me.m_core.EcospaceRegions(iRegionTest)
                            ' Has matching ID?
                            If (RegionTest.getID = ri.Region.getID) Then
                                ' #Yes: Update
                                RegionTest.Name = ri.Name
                                ' Oh yes! YES! YESSS!
                                bFound = True
                            End If
                        Next
                        ' All went well?
                        If Not bFound Then
                            ' #No?! Uh oh...
                            Debug.Assert(False, ">> Internal panic: Unable to apply changes to region id " & ri.Region.getID)
                        End If
                    End If
                Next

                '' Apply all changes
                'Me.m_core.SaveEcospaceScenario()
            End If

            Select Case Me.m_allocateRegionsFlag
                Case AllocationModeType.None
                Case AllocationModeType.Habitat
                    bSuccess = bSuccess And Me.AllocateRegionsFromHabitats(dtHabitatIDToRegionID)
                Case AllocationModeType.Cell
                    bSuccess = bSuccess And Me.AllocateRegionsFromCells(dtCellToRegionID)
            End Select

            Return bSuccess

        End Function

        Private Function AllocateRegionsFromHabitats(ByVal dtHabitatIDToRegionID As Dictionary(Of Integer, Integer)) As Boolean

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim bmlRegions As cEcospaceLayer = bm.LayerRegion()
            Dim bmlHabitats As cEcospaceLayer = bm.LayerHabitat()
            Dim iHab As Integer = 0
            Dim iHabDBID As Integer = 0
            Dim hab As cEcospaceHabitat = Nothing
            Dim iReg As Integer = 0
            Dim iRegDBID As Integer = 0
            Dim reg As cEcospaceRegion = Nothing

            ' Ugh
            Dim drm As Dictionary(Of Integer, cEcospaceRegion) = Me.GetRegionMappings()

            ' For each row
            For iRow As Integer = 1 To bm.InRow
                ' For each col
                For iCol As Integer = 1 To bm.InCol
                    ' Get habitat for cell
                    iHab = CInt(bmlHabitats.Cell(iRow, iCol))
                    ' Get default region
                    iReg = 0
                    ' Is habitat present at this cell?
                    If (iHab > 0) Then
                        ' #Yes: get habitat
                        hab = Me.m_core.EcospaceHabitats(iHab)
                        ' Get DBID for habitat
                        iHabDBID = CInt(hab.GetVariable(eVarNameFlags.DBID))
                        ' Find if there is a region mapping
                        If (dtHabitatIDToRegionID.ContainsKey(iHabDBID)) Then
                            ' #Yes: get mapped region DBID
                            iRegDBID = dtHabitatIDToRegionID(iHabDBID)
                            ' Try to get region (this should work but hey, still good to check)
                            If drm.ContainsKey(iRegDBID) Then
                                ' #Yes: found a region
                                reg = drm(iRegDBID)
                                ' Finally get region index
                                iReg = reg.Index
                            End If
                        End If
                    End If

                    ' Sanity check
                    Debug.Assert((iHab <> 0) = (iReg <> 0))

                    ' Assign or clear region, depending on what has been found
                    bmlRegions.Cell(iRow, iCol) = iReg

                Next iCol
            Next iRow

            Return True

        End Function

        Private Function AllocateRegionsFromCells(ByVal dtCellToRegionID As Dictionary(Of Point, Integer)) As Boolean

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim bmlRegions As cEcospaceLayer = bm.LayerRegion()
            Dim ptCell As Point = Nothing
            Dim iReg As Integer = 0
            Dim iRegDBID As Integer = 0
            Dim reg As cEcospaceRegion = Nothing

            ' Ugh
            Dim drm As Dictionary(Of Integer, cEcospaceRegion) = Me.GetRegionMappings()

            ' For each row
            For iRow As Integer = 1 To bm.InRow
                ' For each col
                For iCol As Integer = 1 To bm.InCol
                    ' Get point for cell
                    ptCell = New Point(iCol, iRow)
                    ' Get default region
                    iReg = 0
                    ' Find if there is a region mapping
                    If (dtCellToRegionID.ContainsKey(ptCell)) Then
                        ' #Yes: get mapped region DBID
                        iRegDBID = dtCellToRegionID(ptCell)
                        ' Try to get region (this should work but hey, still good to check)
                        If drm.ContainsKey(iRegDBID) Then
                            ' #Yes: found a region
                            reg = drm(iRegDBID)
                            ' Finally get region index
                            iReg = reg.Index
                        End If
                    End If

                    ' Assign or clear region, depending on what has been found
                    bmlRegions.Cell(iRow, iCol) = iReg

                Next iCol
            Next iRow

            Return True

        End Function

        Private Function GetRegionMappings() As Dictionary(Of Integer, cEcospaceRegion)
            Dim d As New Dictionary(Of Integer, cEcospaceRegion)
            Dim r As cEcospaceRegion = Nothing
            For iReg As Integer = 1 To Me.m_core.nRegions
                r = Me.m_core.EcospaceRegions(iReg)
                d(CInt(r.GetVariable(eVarNameFlags.DBID))) = r
            Next
            Return d
        End Function

#End Region ' Apply changes

    End Class

End Namespace


