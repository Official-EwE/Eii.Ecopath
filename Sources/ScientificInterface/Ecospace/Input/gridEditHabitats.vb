'==============================================================================
'
' $Log: gridEditHabitats.vb,v $
' Revision 1.3  2009/05/28 12:37:40  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.2  2008/12/15 15:55:33  jeroens
' no message
'
' Revision 1.1  2008/11/04 04:58:44  jeroens
' Renamed
'
' Revision 1.2  2008/10/29 15:45:48  jeroens
' Fixed issue 562
'
' Revision 1.1  2008/09/26 07:31:56  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridEditHabitats
        : Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first Habitat</summary>
        Private Const iFIRSTHABITATROW As Integer = 1

        ''' <summary>The <see cref="cCore">Core</see> currently being modified.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>List of active Habitats.</summary>
        Private m_alHabitats As New List(Of HabitatInfo)
        ''' <summary>List of removed Habitats.</summary>
        Private m_alHabitatsRemoved As New List(Of HabitatInfo)
        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local Habitat administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Visual model to display original Habitats.</summary>
        Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display newly created Habitats.</summary>
        Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display Habitats that are about be deleted.</summary>
        Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            HabitatIndex = 0
            HabitatName
            HabitatStatus
        End Enum

#Region " Helper classes "


        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="cEcospaceHabitat">Habitat</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new Habitats. If this class has its
        ''' <see cref="HabitatInfo.Habitat">Habitat</see> parameter set, a real live
        ''' Habitat is represented. If this parameter is not set, a new Habitat is
        ''' represented.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class HabitatInfo

            ''' <summary><see cref="cEcospaceHabitat">cEcospaceHabitat</see> associated with this Habitat, if any.</summary>
            Private m_Habitat As cEcospaceHabitat = Nothing
            ''' <summary>Name for this Habitat.</summary>
            Private m_strName As String = ""
            ''' <summary>Flag stating whether a user action is confirmed</summary>
            Private m_bConfirmed As Boolean = True
            ''' <summary>The status of a Habitat in the interface.</summary>
            Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="Habitat">The <see cref="cEcospaceHabitat">cEcospaceHabitat</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' Habitat currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal Habitat As cEcospaceHabitat)
                Debug.Assert(Habitat IsNot Nothing)
                Me.m_Habitat = Habitat
                Me.m_strName = Habitat.Name
                Me.m_status = AddRemoveItemStatus.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String)
                Me.m_Habitat = Nothing
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
            ''' Get the <see cref="cEcospaceHabitat">EwE Habitat</see> associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Habitat() As cEcospaceHabitat
                Get
                    Return Me.m_Habitat
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
            ''' for the habitat object.
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
            ''' States whether the Habitat has changed.
            ''' </summary>
            ''' <returns>
            ''' True when Habitat <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged() As Boolean
                If (Me.IsNew()) Then Return False
                Return (Me.m_Habitat.Name <> Me.m_strName)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the Habitat is to be created.
            ''' </summary>
            ''' <returns>
            ''' True when Habitat <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsNew() As Boolean
                Return (Me.m_Habitat Is Nothing)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this habitat is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = AddRemoveItemStatus.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Me.m_Habitat IsNot Nothing Then
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
            Me.m_core = cCore.GetInstance()

            ' Set up visual models for reflecting Habitat modification status
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

            ' Habitat index cell
            Me(0, eColumnTypes.HabitatIndex) = New EwEColumnHeaderCell()
            ' Habitat name cell, editable this time
            Me(0, eColumnTypes.HabitatName) = New EwEColumnHeaderCell(My.Resources.HEADER_HABITAT)

            ' Habitat index cell
            Me(0, eColumnTypes.HabitatStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

            ' Fix index column only; Habitat name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.None
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the Habitat/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            ' Get the core reference
            Dim Habitat As cEcospaceHabitat = Nothing
            Dim hi As HabitatInfo = Nothing

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of Habitat configuration
            ' SKIP ALL HABITAT HERE!
            For iHabitat As Integer = 1 To Me.m_core.nHabitats - 1
                Habitat = Me.m_core.EcospaceHabitats(iHabitat)
                hi = New HabitatInfo(Habitat)
                Me.m_alHabitats.Add(hi)
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

            Dim hi As HabitatInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_alHabitats.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.HabitatIndex) = ewec

                Me(iRow, eColumnTypes.HabitatName) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.HabitatName).Behaviors.Add(m_bm)

                ' Status
                vm = New VisualModels.Common()
                vm.ImageAlignment = ContentAlignment.MiddleCenter
                Me(iRow, eColumnTypes.HabitatStatus) = New Cells.Real.Cell()
                Dim dm As New DataModels.DataModelBase(GetType(String))
                dm.EditableMode = EditableMode.None
                Me(iRow, eColumnTypes.HabitatStatus).DataModel = dm
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_alHabitats.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTHABITATROW)
            End While

            ' Sanity check whether grid can accomodate all Habitats + header
            Debug.Assert(Me.Rows.Count = Me.m_alHabitats.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_alHabitats.Count
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

            Dim hi As HabitatInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim strText As String = ""

            Me.AllowUpdates = False

            hi = DirectCast(Me.m_alHabitats(iRow - iFIRSTHABITATROW), HabitatInfo)
            ri = Me.Rows(iRow)

            ri.Tag = hi
            aCells = ri.GetCells()

            pos = New Position(iRow, eColumnTypes.HabitatIndex)
            aCells(eColumnTypes.HabitatIndex).SetValue(pos, CInt(iRow))

            pos = New Position(iRow, eColumnTypes.HabitatName)
            aCells(eColumnTypes.HabitatName).SetValue(pos, CStr(hi.Name))

            Select Case hi.Status
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

            pos = New Position(iRow, eColumnTypes.HabitatStatus)
            aCells(eColumnTypes.HabitatStatus).VisualModel = vm
            aCells(eColumnTypes.HabitatStatus).SetValue(pos, strText)

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

            Dim hi As HabitatInfo = DirectCast(Me.m_alHabitats(p.Row - 1), HabitatInfo)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.HabitatName
                    ' JS: Handled in OnCellEdited()
                    ' hi.Name = CStr(cell.GetValue(p))

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

            Dim hi As HabitatInfo = DirectCast(Me.m_alHabitats(p.Row - 1), HabitatInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.HabitatIndex
                    ' Not possible

                Case eColumnTypes.HabitatName
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iHabitat As Integer = 0 To Me.m_alHabitats.Count - 1
                        Dim giTemp As HabitatInfo = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
                        ' Does name already exist?
                        If (Not Object.ReferenceEquals(giTemp, hi)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    hi.Name = strName

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

            Dim iHabitat As Integer = iRow - iFIRSTHABITATROW
            Dim hi As HabitatInfo = Nothing
            Dim strPrompt As String = ""

            ' Validate
            If iHabitat < 0 Then Return

            hi = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
            ' Toggle 'flagged for deletion' flag
            hi.FlaggedForDeletion = Not hi.FlaggedForDeletion

            ' Check to see what is to happen to the Habitat now
            Select Case hi.Status

                Case AddRemoveItemStatus.Original
                    ' Clear removed status of the Habitat
                    Me.m_alHabitatsRemoved.Remove(Me.m_alHabitats(iHabitat))

                Case AddRemoveItemStatus.Added
                    ' Clear removed status of the Habitat
                    Me.m_alHabitatsRemoved.Remove(Me.m_alHabitats(iHabitat))

                Case AddRemoveItemStatus.Removed
                    ' Set removed status
                    Me.m_alHabitatsRemoved.Add(Me.m_alHabitats(iHabitat))

                Case AddRemoveItemStatus.Invalid
                    ' Set removed status
                    Me.m_alHabitats.RemoveAt(iHabitat)

            End Select

            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a habitat.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsHabitatRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTHABITATROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the habitat on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsHabitatRow(iRow) Then Return False

            Dim iHabitat As Integer = iRow - iFIRSTHABITATROW
            Dim hi As HabitatInfo = Nothing
            Dim strPrompt As String = ""

            hi = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
            Return hi.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new habitat.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateHabitat()
        End Sub

        ''' <summary>
        ''' Create a new habitat.
        ''' </summary>
        Private Sub CreateHabitat()
            Dim iRow As Integer = -1
            Dim iHabitat As Integer = -1
            Dim hi As HabitatInfo = Nothing
            Dim lstrHabitats As New List(Of String)

            ' Make fit
            iRow = Math.Max(iFIRSTHABITATROW, Me.RowsCount)
            iHabitat = iRow - iFIRSTHABITATROW

            ' Validate
            If iHabitat < 0 Then Return

            ' Collect all current habitat names
            For Each hi In Me.m_alHabitats
                lstrHabitats.Add(hi.Name)
            Next

            ' Format new hab with an autonumber value based on existing names
            hi = New HabitatInfo(String.Format(My.Resources.DEFAULT_NEWHABITAT_NUM, _
                    cStringUtils.GetNextNumber(lstrHabitats.ToArray(), My.Resources.DEFAULT_NEWHABITAT_NUM)))
            Me.m_alHabitats.Insert(iHabitat, hi)

            Me.UpdateGrid()
            Me.SelectRow(hi)
        End Sub

        ''' <summary>
        ''' States whether a row can be inserted at the indicated position.
        ''' </summary>
        Public Function CanAddRow() As Boolean
            Return True
        End Function

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

        Private Sub SelectRow(ByVal hi As HabitatInfo)
            For iHabitat As Integer = 0 To Me.m_alHabitats.Count - 1
                If Object.ReferenceEquals(Me.m_alHabitats(iHabitat), hi) Then
                    Me.SelectRow(iHabitat + iFIRSTHABITATROW)
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
        ''' Habitat configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean

            '' Check if the user is about to delete all fleets - one should remain
            'If Me.m_alHabitatsRemoved.Count = Me.m_alHabitats.Count Then
            '    MsgBox(My.Resources.ECOPATH_EDITHABITAT_PROMPT_CANNOTDELETEALL, _
            '            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, _
            '            My.Resources.ECOPATH_EDITHABITAT_CONFIRMDELETE_CAPTION)
            '    Return False
            'End If

            Return True

        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bHabitatsChanged As Boolean = False
            Dim hi As HabitatInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim Habitat As cEcospaceHabitat = Nothing
            Dim iHabitat As Integer = 0
            Dim bSuccess As Boolean = True

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess Habitat changes
            For iHabitat = 0 To Me.m_alHabitats.Count - 1
                hi = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
                ' Check if this habitat is newly added
                bConfigurationChanged = bConfigurationChanged Or hi.IsNew()
                ' Check if this habitat has been modified
                bHabitatsChanged = bHabitatsChanged Or hi.IsChanged()
            Next iHabitat

            If Me.m_alHabitatsRemoved.Count > 5 Then

                strPrompt = String.Format(My.Resources.ECOSPACE_EDITHABITAT_CONFIRMDELETENUM_PROMPT, Me.m_alHabitatsRemoved.Count)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.Yes
                        ' Confirm all regions
                        For Each hi In Me.m_alHabitatsRemoved
                            hi.Confirmed = True
                        Next
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            Else
                ' Assess Habitats to remove
                For iHabitat = 0 To Me.m_alHabitatsRemoved.Count - 1
                    hi = DirectCast(Me.m_alHabitatsRemoved(iHabitat), HabitatInfo)
                    If (Not hi.IsNew()) Then

                        strPrompt = String.Format(My.Resources.ECOSPACE_EDITHABITAT_CONFIRMDELETE_PROMPT, hi.Name)

                        Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                            Case MsgBoxResult.Cancel
                                ' Abort Apply process
                                Return False
                            Case MsgBoxResult.No
                                ' Do not delete this Habitat
                                hi.Confirmed = False
                            Case MsgBoxResult.Yes
                                ' Delete this Habitat
                                hi.Confirmed = True
                                bConfigurationChanged = True
                            Case Else
                                ' Unexpected anwer: assert
                                Debug.Assert(False)
                        End Select

                    End If
                Next iHabitat
            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

                cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

                ' Add new Habitats
                For iHabitat = 0 To Me.m_alHabitats.Count - 1
                    hi = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
                    If (hi.IsNew()) Then
                        bSuccess = bSuccess And Me.m_core.AddEcospaceHabitat(hi.Name, iDBID)
                    End If
                Next

                ' Remove deleted (and confirmed) Habitats
                Dim iHabitatRemove As Integer = 0
                For iHabitat = 0 To Me.m_alHabitatsRemoved.Count - 1
                    hi = DirectCast(Me.m_alHabitatsRemoved(iHabitatRemove), HabitatInfo)

                    ' Sanity check
                    Debug.Assert(Not hi.IsNew())

                    If (hi.Confirmed()) Then
                        If (Me.m_core.RemoveEcospaceHabitat(hi.Habitat)) Then
                            Me.m_alHabitats.Remove(hi)
                            Me.m_alHabitatsRemoved.Remove(hi)
                        Else
                            bSuccess = False
                            iHabitatRemove += 1
                        End If
                    End If
                Next

                ' The core will reload now
                Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)
                cApplicationStatusNotifier.SetStatusText("", TriState.False)

                ' Test whether new Habitats were loaded correctly 
                ' !! taking into account that this dialog does NOT contain the All habitat, hence the '-1'
                Debug.Assert(Me.m_alHabitats.Count = (Me.m_core.nHabitats - 1), ">> Internal panic: Dialog and core out of sync on Habitats")
            End If

            ' Update core objects
            If (bHabitatsChanged) Then
                ' For each local habitat admin unit
                For iHabitat = 0 To Me.m_alHabitats.Count - 1
                    ' Get local admin unit
                    hi = DirectCast(Me.m_alHabitats(iHabitat), HabitatInfo)
                    ' Has it changed?
                    If (hi.IsChanged()) Then
                        ' Find core habitat with same BDID (cannot use cached cEcospaceHabitat instances since the core has reloaded)
                        Dim bFound As Boolean = False
                        ' For every core habitat instance
                        For iHabTest As Integer = 1 To Me.m_core.nHabitats - 1
                            ' Get core habitat instance
                            Dim habTest As cEcospaceHabitat = Me.m_core.EcospaceHabitats(iHabTest)
                            ' Has matching ID?
                            If (habTest.getID = hi.Habitat.getID) Then
                                ' #Yes: Update
                                habTest.Name = hi.Name
                                ' Are we relieved or what!
                                bFound = True
                            End If
                        Next
                        ' All went well?
                        If Not bFound Then
                            ' #No?! Uh oh...
                            Debug.Assert(False, ">> Internal panic: Unable to apply changes to habitat id " & hi.Habitat.getID)
                        End If
                    End If
                Next
            End If

            Return bSuccess

        End Function

#End Region ' Apply changes

    End Class

End Namespace


