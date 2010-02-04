#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports ScientificInterface.Other

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Grid class for the Edit Fleets interface.
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
   Public Class EditFleetsEwEGrid
    : Inherits EwEGrid

#Region " Private vars "

    ''' <summary>A number representing the row that contains the first Fleet</summary>
    Private Const iFIRSTFLEETROW As Integer = 1

    ''' <summary>List of active Fleets.</summary>
    Private m_lfiFleets As New List(Of cFleetInfo)
    ''' <summary>List of removed Fleets.</summary>
    Private m_lfiFleetsRemoved As New List(Of cFleetInfo)
    ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
    ''' to trap cell edit events locally in this grid. These events are essential
    ''' for keeping the local Fleet administration up to date.</summary>
    Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
    ''' <summary>Update lock, used to distinguish between code updates and
    ''' user updates of grid cells. When grid cells are updated from within
    ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
    Private m_iUpdateLock As Integer = 0

    ''' <summary>Visual model to display original Fleets.</summary>
    Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display newly created Fleets.</summary>
    Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
    ''' <summary>Visual model to display Fleets that are about be deleted.</summary>
    Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        FleetIndex = 0
        FleetName
        FleetColor
        FleetStatus
    End Enum

#End Region ' Private vars

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Administrative unit representing a <see cref="cFleetInput">Fleet</see>
    ''' in the EwE model.
    ''' </summary>
    ''' <remarks>
    ''' This class can represent existing and new Fleets. If this class has its
    ''' <see cref="cFleetInfo.Fleet">Fleet</see> parameter set, a real live
    ''' Fleet is represented. If this parameter is not set, a new Fleet is
    ''' represented.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Class cFleetInfo

        ''' <summary><see cref="cFleetInput">cFleetInput</see> associated with this Fleet, if any.</summary>
        Private m_fleet As cFleetInput = Nothing
        ''' <summary>Name for this Fleet.</summary>
        Private m_strName As String = ""
        ''' <summary>Fleet color.</summary>
        Private m_iColor As Integer = 0
        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a Fleet in the interface.</summary>
        Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="Fleet">The <see cref="cFleetInput">cFleetInput</see> to
        ''' initialize this instance from. If set, this instance represents a
        ''' Fleet currently active in the EwE model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal fleet As cFleetInput)
            Debug.Assert(fleet IsNot Nothing)
            Me.m_fleet = fleet
            Me.m_strName = fleet.Name
            Me.m_iColor = fleet.PoolColor
            Me.m_status = AddRemoveItemStatus.Original
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String)
            Me.m_fleet = Nothing
            Me.m_strName = strName
            Me.m_iColor = 0
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
        ''' Get/set the <see cref="cEcopathDataStructures.FleetColor">Color</see> value of
        ''' this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property PoolColor() As Integer
            Get
                Return Me.m_iColor
            End Get
            Set(ByVal value As Integer)
                Me.m_iColor = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cFleetInput">EwE Fleet</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Fleet() As cFleetInput
            Get
                Return Me.m_fleet
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
        ''' for the fleet object.
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
        ''' States whether the underlying fleet has been changed.
        ''' </summary>
        ''' <returns>
        ''' True if the underlying fleet has been changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged() As Boolean
            If Me.m_fleet Is Nothing Then Return False
            Return (Me.m_fleet.Name <> Me.m_strName) Or _
                   (Me.m_fleet.PoolColor <> Me.m_iColor)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this fleet is flagged for deletion. Toggling this flag
        ''' will update the <see cref="Status">Status</see> of the item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = AddRemoveItemStatus.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Me.m_fleet IsNot Nothing Then
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

    End Class

#End Region ' Helper classes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the grid
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()

        MyBase.New()

        ' Set up visual models for reflecting Fleet modification status
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

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

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

        ' Fleet index cell
        Me(0, eColumnTypes.FleetIndex) = New EwEColumnHeaderCell()
        ' Fleet name cell, editable this time
        Me(0, eColumnTypes.FleetName) = New EwEColumnHeaderCell(My.Resources.HEADER_FLEETNAME)
        ' Color
        Me(0, eColumnTypes.FleetColor) = New EwEColumnHeaderCell(My.Resources.HEADER_COLOR)

        ' Fleet index cell
        Me(0, eColumnTypes.FleetStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

        ' Fix index column only; Fleet name column cannot be fixed because it must be editable
        Me.FixedColumns = 1

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to first create a snapshot of the Fleet/stanza configuration
    ''' in the current EwE model. The grid will be populated from this local
    ''' administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        Dim Fleet As cFleetInput = Nothing
        Dim fi As cFleetInfo = Nothing

        ' Populate local administration from a snapshot of the live data
        Me.m_lfiFleets.Clear()

        ' Make snapshot of Fleet configuration
        For iFleet As Integer = 1 To Me.Core.nFleets
            Fleet = Core.FleetInputs(iFleet)
            fi = New cFleetInfo(Fleet)
            Me.m_lfiFleets.Add(fi)
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

        Dim fi As cFleetInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim cells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim ewec As EwECell = Nothing

        ' Create missing rows
        For iRow As Integer = Me.Rows.Count To Me.m_lfiFleets.Count
            Me.AddRow()

            ewec = New EwECell(0, GetType(Integer))
            ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.FleetIndex) = ewec

            Me(iRow, eColumnTypes.FleetName) = New Cells.Real.Cell("", GetType(String))
            Me(iRow, eColumnTypes.FleetName).Behaviors.Add(m_bm)

            Me(iRow, eColumnTypes.FleetColor) = New Cells.Real.Cell()
            Me(iRow, eColumnTypes.FleetColor).VisualModel = New cColorCellVisualizer()
            Me(iRow, eColumnTypes.FleetColor).Behaviors.Add(m_bm)

            ' Status
            vm = New VisualModels.Common()
            vm.ImageAlignment = ContentAlignment.MiddleCenter
            Me(iRow, eColumnTypes.FleetStatus) = New Cells.Real.Cell()
            Dim dm As New DataModels.DataModelBase(GetType(String))
            dm.EditableMode = EditableMode.None
            Me(iRow, eColumnTypes.FleetStatus).DataModel = dm
        Next

        ' Delete obsolete rows
        While Me.Rows.Count > Me.m_lfiFleets.Count + 1
            Me.Rows.Remove(Me.Rows.Count - iFIRSTFLEETROW)
        End While

        ' Sanity check whether grid can accomodate all Fleets + header
        Debug.Assert(Me.Rows.Count = Me.m_lfiFleets.Count + 1)

        ' Populate rows
        For iRow As Integer = 1 To Me.m_lfiFleets.Count
            UpdateRow(iRow)
        Next iRow

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        ' Should size to fit header
        Me.Columns(eColumnTypes.FleetColor).Width = 80
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)

        Dim fi As cFleetInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim aCells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim strText As String = ""

        Me.AllowUpdates = False

        fi = DirectCast(Me.m_lfiFleets(iRow - iFIRSTFLEETROW), cFleetInfo)
        ri = Me.Rows(iRow)

        ri.Tag = fi
        aCells = ri.GetCells()

        pos = New Position(iRow, eColumnTypes.FleetIndex)
        aCells(eColumnTypes.FleetIndex).SetValue(pos, CInt(iRow))

        pos = New Position(iRow, eColumnTypes.FleetName)
        aCells(eColumnTypes.FleetName).SetValue(pos, CStr(fi.Name))

        pos = New Position(iRow, eColumnTypes.FleetColor)
        Dim clr As Color = cStyleGuide.IntToColor(fi.PoolColor)
        If clr.A = 0 Then clr = Me.StyleGuide.FleetColorDefault(iRow, Me.m_lfiFleets.Count)
        aCells(eColumnTypes.FleetColor).SetValue(pos, clr)

        Select Case fi.Status
            Case AddRemoveItemStatus.Original
                vm = Me.m_vmOriginal
                strText = My.Resources.GENERIC_ITEMSTATUS_ORIGINAL
            Case AddRemoveItemStatus.Added
                vm = Me.m_vmAdded
                strText = My.Resources.GENERIC_ITEMSTATUS_CREATEPENDING
            Case AddRemoveItemStatus.Removed
                vm = Me.m_vmRemoved
                strText = My.Resources.GENERIC_ITEMSTATUS_DELETEPENDING
        End Select

        pos = New Position(iRow, eColumnTypes.FleetStatus)
        aCells(eColumnTypes.FleetStatus).VisualModel = vm
        aCells(eColumnTypes.FleetStatus).SetValue(pos, strText)

        Me.AllowUpdates = True

    End Sub

    Private Sub UpdateColorColumn()

        Dim fi As cFleetInfo = Nothing
        Dim clr As Color = Color.Transparent

        Me.AllowUpdates = False
        For iRow As Integer = iFIRSTFLEETROW To Me.RowsCount - 1
            fi = DirectCast(Me.m_lfiFleets(iRow - iFIRSTFLEETROW), cFleetInfo)
            clr = cStyleGuide.IntToColor(fi.PoolColor)
            If clr.A = 0 Then
                clr = Me.StyleGuide.FleetColorDefault(iRow - iFIRSTFLEETROW + 1, Me.m_lfiFleets.Count)
            End If
            Me(iRow, eColumnTypes.FleetColor).Value = clr
        Next iRow
        Me.AllowUpdates = True

        Me.Invalidate()

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

        Dim fi As cFleetInfo = DirectCast(Me.m_lfiFleets(p.Row - 1), cFleetInfo)

        Select Case DirectCast(p.Column, eColumnTypes)

            Case eColumnTypes.FleetName
                ' JS: Handled in OnCellEdited()
                ' fi.Name = CStr(cell.GetValue(p))

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

        Dim fi As cFleetInfo = DirectCast(Me.m_lfiFleets(p.Row - 1), cFleetInfo)

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.FleetIndex
                ' Not possible

            Case eColumnTypes.FleetName
                Dim strName As String = CStr(cell.GetValue(p))
                ' Check if name is unique
                For iFleet As Integer = 0 To Me.m_lfiFleets.Count - 1
                    Dim giTemp As cFleetInfo = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
                    ' Does name already exist?
                    If (Not Object.ReferenceEquals(giTemp, fi)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                        ' Change is not allowed
                        Me.UpdateRow(p.Row)
                        ' Report failure
                        Return False
                    End If
                Next
                ' Allow name change
                fi.Name = strName

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
            Case eColumnTypes.FleetColor
                Me.SelectCustomColor(p.Row)
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

        Dim iFleet As Integer = iRow - iFIRSTFLEETROW
        Dim fi As cFleetInfo = Nothing
        Dim strPrompt As String = ""

        ' Validate
        If iFleet < 0 Then Return

        fi = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
        ' Toggle 'flagged for deletion' flag
        fi.FlaggedForDeletion = Not fi.FlaggedForDeletion

        ' Check to see what is to happen to the Fleet now
        Select Case fi.Status

            Case AddRemoveItemStatus.Original
                ' Clear removed status of the Fleet
                Me.m_lfiFleetsRemoved.Remove(Me.m_lfiFleets(iFleet))

            Case AddRemoveItemStatus.Added
                ' Clear removed status of the Fleet
                Me.m_lfiFleetsRemoved.Remove(Me.m_lfiFleets(iFleet))

            Case AddRemoveItemStatus.Removed
                ' Set removed status
                Me.m_lfiFleetsRemoved.Add(Me.m_lfiFleets(iFleet))

            Case AddRemoveItemStatus.Invalid
                ' Set removed status
                Me.m_lfiFleets.RemoveAt(iFleet)

        End Select

        Me.UpdateGrid()

    End Sub

    ''' <summary>
    ''' States whether a row holds a fleet.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns></returns>
    Public Function IsFleetRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (iRow >= iFIRSTFLEETROW) And (iRow < Me.RowsCount)
    End Function

    ''' <summary>
    ''' States whether the fleet on a row is flagged for deletion.
    ''' </summary>
    Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not IsFleetRow(iRow) Then Return False

        Dim iFleet As Integer = iRow - iFIRSTFLEETROW
        Dim fi As cFleetInfo = Nothing
        Dim strPrompt As String = ""

        fi = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
        Return fi.FlaggedForDeletion
    End Function

    ''' <summary>
    ''' Insert a row by creating a new fleet.
    ''' </summary>
    Public Sub InsertRow(Optional ByVal iRow As Integer = -1)
        If iRow = -1 Then iRow = Me.SelectedRow()
        If iRow = -1 Then iRow = Math.Max(iFIRSTFLEETROW, Me.RowsCount)
        If Not Me.CanInsertRow(iRow) Then Return
        Me.CreateFleet(iRow)
    End Sub

    ''' <summary>
    ''' Create a new fleet.
    ''' </summary>
    Private Sub CreateFleet(ByVal iRow As Integer)

        Dim iFleet As Integer = -1
        Dim fi As cFleetInfo = Nothing
        Dim lstrFleetNames As New List(Of String)

        ' Make fit
        iRow = Math.Max(iFIRSTFLEETROW, iRow)
        iFleet = iRow - iFIRSTFLEETROW

        ' Validate
        If iFleet < 0 Then Return

        ' Gather fleet names for generating new number
        For i As Integer = 0 To Me.m_lfiFleets.Count - 1
            lstrFleetNames.Add(Me.m_lfiFleets(i).Name)
        Next i

        fi = New cFleetInfo(String.Format(My.Resources.DEFAULT_NEWFLEET_NUM, _
                StringUtils.GetNextNumber(lstrFleetNames.ToArray, My.Resources.DEFAULT_NEWFLEET_NUM)))
        Me.m_lfiFleets.Insert(iFleet, fi)

        Me.UpdateGrid()
        Me.SelectRow(fi)
    End Sub

    ''' <summary>
    ''' States whether a row can be inserted at the indicated position.
    ''' </summary>
    Public Function CanInsertRow(Optional ByVal iRow As Integer = -1) As Boolean
        Return True
    End Function

    ''' <summary>
    ''' Move row up, switching positions with the row above it.
    ''' </summary>
    Public Sub MoveRowUp(Optional ByVal iRow As Integer = -1)
        Dim bMoveSelection As Boolean = (iRow = -1)

        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not CanMoveRowUp(iRow) Then Return
        Me.MoveRow(iRow, iRow - 1)

        If bMoveSelection Then
            Me.SelectRow(iRow - 1)
        End If
    End Sub

    ''' <summary>
    ''' States whether a row can be moved up.
    ''' </summary>
    Public Function CanMoveRowUp(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (Me.RowsCount > (iFIRSTFLEETROW + 1)) And (iRow > iFIRSTFLEETROW)
    End Function

    ''' <summary>
    ''' Move row down, switching positions with the row below it.
    ''' </summary>
    Public Sub MoveRowDown(Optional ByVal iRow As Integer = -1)
        Dim bMoveSelection As Boolean = (iRow = -1)

        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not CanMoveRowDown(iRow) Then Return
        Me.MoveRow(iRow, iRow + 1)

        If bMoveSelection Then
            Me.SelectRow(iRow + 1)
        End If
    End Sub

    ''' <summary>
    ''' States whether a row can be moved down.
    ''' </summary>
    Public Function CanMoveRowDown(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (Me.RowsCount > (iFIRSTFLEETROW + 1)) And (iRow >= iFIRSTFLEETROW) And (iRow < Me.RowsCount - 1)
    End Function

    ''' <summary>
    ''' Move one row to another position.
    ''' </summary>
    Private Sub MoveRow(ByVal iFromRow As Integer, ByVal iToRow As Integer)

        Dim objTemp As cFleetInfo = Nothing
        Dim iStep As Integer = 1
        Dim iFromFleet As Integer = iFromRow - iFIRSTFLEETROW
        Dim iToFleet As Integer = iToRow - iFIRSTFLEETROW

        ' Truncate
        iFromFleet = Math.Max(0, Math.Min(Me.m_lfiFleets.Count - 1, iFromFleet))
        iToFleet = Math.Max(0, Math.Min(Me.m_lfiFleets.Count - 1, iToFleet))

        ' Nothing to do? abort
        If iFromFleet = iToFleet Then Return
        ' Determine direction of movement
        If iFromFleet < iToFleet Then iStep = 1 Else iStep = -1

        ' Swap Fleets (but do not swap the Fleet at iTo because then we've gone 1 too far)
        For iFleet As Integer = iFromFleet To iToFleet - iStep Step iStep
            objTemp = Me.m_lfiFleets(iFleet + iStep)
            Me.m_lfiFleets(iFleet + iStep) = Me.m_lfiFleets(iFleet)
            Me.m_lfiFleets(iFleet) = objTemp
            Me.UpdateRow(iFleet + iFIRSTFLEETROW)
            Me.UpdateRow(iFleet + iFIRSTFLEETROW + iStep)
        Next iFleet

    End Sub

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

    Private Sub SelectRow(ByVal fi As cFleetInfo)
        For iFleet As Integer = 0 To Me.m_lfiFleets.Count - 1
            If Object.ReferenceEquals(Me.m_lfiFleets(iFleet), fi) Then
                Me.SelectRow(iFleet + iFIRSTFLEETROW)
            End If
        Next
    End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Colors "

    Public Sub ResetFleetColors()

        Dim fi As cFleetInfo = Nothing
        For iFleet As Integer = 0 To Me.m_lfiFleets.Count - 1
            fi = Me.m_lfiFleets(iFleet)
            fi.PoolColor = 0
        Next
        Me.UpdateColorColumn()

    End Sub

    Public Sub SelectCustomColor(Optional ByVal iRow As Integer = -1)

        Dim fi As cFleetInfo = Nothing
        Dim dlgColor As ColorDialog = Nothing

        If iRow = -1 Then iRow = Me.SelectedRow

        If Not Me.IsFleetRow(iRow) Then Return

        fi = Me.m_lfiFleets(iRow - iFIRSTFLEETROW)

        dlgColor = New ColorDialog()
        dlgColor.Color = cStyleGuide.IntToColor(fi.PoolColor)
        If dlgColor.ShowDialog() = DialogResult.OK Then
            fi.PoolColor = cStyleGuide.ColorToInt(dlgColor.Color)
            Me.UpdateRow(iRow)
        End If

    End Sub

#End Region ' Colors

#Region " Validation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; validates the content of the grid.
    ''' </summary>
    ''' <returns>True when the content of the grid depicts a valid
    ''' Fleet configuration for a model.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ValidateContent() As Boolean

        '' Check if the user is about to delete all fleets - one should remain
        'If Me.m_alFleetsRemoved.Count = Me.m_alFleets.Count Then
        '    MsgBox(My.Resources.ECOPATH_EDITFLEET_PROMPT_CANNOTDELETEALL, _
        '            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, _
        '            My.Resources.ECOPATH_EDITFLEET_CONFIRMDELETE_CAPTION)
        '    Return False
        'End If

        Return True

    End Function

#End Region ' Validation

#Region " Apply changes "

    Public Function Apply() As Boolean

        Dim strPrompt As String = ""
        Dim bConfigurationChanged As Boolean = False
        Dim bFleetsChanged As Boolean = False
        Dim fi As cFleetInfo = Nothing
        Dim fleet As cFleetInput = Nothing
        Dim iFleet As Integer = 0
        Dim bColorsChanged As Boolean = False
        Dim bSuccess As Boolean = True

        ' Validate content of the grid
        If Not Me.ValidateContent() Then Return False

        ' Assess Fleet changes
        For iFleet = 0 To Me.m_lfiFleets.Count - 1
            fi = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
            ' Check this Fleet is newly added
            If Object.ReferenceEquals(fi.Fleet, Nothing) Then
                bConfigurationChanged = True
            End If
            ' Check if this Fleet is an existing Fleet that has been moved
            If Not Object.ReferenceEquals(fi.Fleet, Nothing) Then
                If ((iFleet + 1) <> fi.Fleet.Index) Then
                    bConfigurationChanged = True
                End If
            End If
            bFleetsChanged = bFleetsChanged Or fi.IsChanged()
        Next iFleet

        ' Assess Fleets to remove
        strPrompt = ""
        For iFleet = 0 To Me.m_lfiFleetsRemoved.Count - 1
            fi = DirectCast(Me.m_lfiFleetsRemoved(iFleet), cFleetInfo)
            If (Not Object.ReferenceEquals(fi.Fleet, Nothing)) Then

                strPrompt = String.Format(My.Resources.ECOPATH_EDITFLEET_CONFIRMDELETE_PROMPT, fi.Name)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.No
                        ' Do not delete this Fleet
                        fi.Confirmed = False
                    Case MsgBoxResult.Yes
                        ' Delete this Fleet
                        fi.Confirmed = True
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            End If
        Next iFleet

        ' Handle added and removed items
        If (bConfigurationChanged) Then

            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

            cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

            Dim htFleetID As New Dictionary(Of cFleetInfo, Integer)
            Dim iDBID As Integer = Nothing

            ' Add new Fleets
            For iFleet = 0 To Me.m_lfiFleets.Count - 1

                fi = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
                If (Object.ReferenceEquals(fi.Fleet, Nothing)) Then
                    Dim igt As Integer = iFleet + 1
                    bSuccess = bSuccess And Me.Core.AddFleet(fi.Name, igt, iDBID)
                    ' Map this new ID during update
                    htFleetID.Add(fi, iDBID)
                Else
                    If ((iFleet + 1) <> fi.Fleet.Index) Then
                        bSuccess = bSuccess And Me.Core.MoveFleet(fi.Fleet.Index, iFleet + 1)
                    End If
                End If
            Next

            ' Remove deleted (and confirmed) Fleets
            Dim iFleetRemove As Integer = 0
            For iFleet = 0 To Me.m_lfiFleetsRemoved.Count - 1
                fi = DirectCast(Me.m_lfiFleetsRemoved(iFleetRemove), cFleetInfo)
                If (Not Object.ReferenceEquals(fi.Fleet, Nothing)) And (fi.Confirmed = True) Then
                    If (Me.Core.RemoveFleet(fi.Fleet.Index)) Then
                        Me.m_lfiFleets.Remove(fi)
                        Me.m_lfiFleetsRemoved.Remove(fi)
                    Else
                        bSuccess = False
                        iFleetRemove += 1
                    End If
                End If
            Next

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

            ' Test whether new Fleets were loaded correctly
            Debug.Assert(Me.m_lfiFleets.Count = Me.Core.nFleets, "Dialog and core out of sync on Fleets")
        End If

        ' Update core objects
        If (bFleetsChanged) Then
            For iFleet = 0 To Me.m_lfiFleets.Count - 1
                fi = DirectCast(Me.m_lfiFleets(iFleet), cFleetInfo)
                If fi.IsChanged() Then
                    fleet = Me.Core.FleetInputs(iFleet + 1)
                    If fleet.Name <> fi.Name Then fleet.Name = fi.Name
                    If fleet.PoolColor <> fi.PoolColor Then
                        ' Is gi.poolcolor the default color? 
                        If fi.PoolColor = cStyleGuide.ColorToInt(Me.StyleGuide.FleetColorDefault(fleet.Index, Me.m_lfiFleets.Count)) Then
                            ' #Yes: Set color to transparent to allow group to show up as true default colour
                            fleet.PoolColor = 0
                        Else
                            ' #No: Assign new color
                            fleet.PoolColor = fi.PoolColor
                        End If
                        bColorsChanged = True
                    End If
                End If
            Next
            If bColorsChanged Then cStyleGuide.GetInstance().ColorsChanged()
        End If

        Return bSuccess

    End Function

#End Region ' Apply changes

End Class
