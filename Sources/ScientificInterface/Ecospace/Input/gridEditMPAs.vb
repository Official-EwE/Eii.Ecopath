'==============================================================================
'
' $Log: gridEditMPAs.vb,v $
' Revision 1.4  2009/05/28 12:37:42  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.3  2009/03/23 02:25:04  jeroens
' No longer uses month resource strings; uses OS date formatting options instead
'
' Revision 1.2  2008/12/15 15:55:34  jeroens
' no message
'
' Revision 1.1  2008/11/04 04:58:44  jeroens
' Renamed
'
' Revision 1.2  2008/10/29 15:45:49  jeroens
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
    Public Class gridEditMPA
        : Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first MPA</summary>
        Private Const iFIRSTMPAROW As Integer = 1

        ''' <summary>The <see cref="cCore">Core</see> currently being modified.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>List of active MPAs.</summary>
        Private m_alMPAs As New List(Of MPAInfo)
        ''' <summary>List of removed MPAs.</summary>
        Private m_alMPAsRemoved As New List(Of MPAInfo)
        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local MPA administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Visual model to display original MPAs.</summary>
        Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display newly created MPAs.</summary>
        Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display MPAs that are about be deleted.</summary>
        Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            MPAIndex = 0
            MPAName
            MPAAll
            MPAJan
            MPAFeb
            MPAMar
            MPAApr
            MPAMay
            MPAJun
            MPAJul
            MPAAug
            MPASep
            MPAOct
            MPANov
            MPADec
            MPAStatus
        End Enum

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="cEcospaceMPA">MPA</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new MPAs. If this class has its
        ''' <see cref="MPAInfo.MPA">MPA</see> parameter set, a real live
        ''' MPA is represented. If this parameter is not set, a new MPA is
        ''' represented.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class MPAInfo

            ''' <summary><see cref="cEcospaceMPA">cEcospaceMPA</see> associated with this MPA, if any.</summary>
            Private m_MPA As cEcospaceMPA = Nothing
            ''' <summary>Name for this MPA.</summary>
            Private m_strName As String = ""
            ''' <summary>Months this MPA is closed.</summary>
            Private m_bOpenMonths(cCore.N_MONTHS) As Boolean
            ''' <summary>The status of a MPA in the interface.</summary>
            Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="MPA">The <see cref="cEcospaceMPA">cEcospaceMPA</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' MPA currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal MPA As cEcospaceMPA)
                Debug.Assert(MPA IsNot Nothing)
                Me.m_MPA = MPA
                Me.m_strName = MPA.Name
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    Me.m_bOpenMonths(iMonth) = MPA.MPAMonth(iMonth)
                Next
                Me.m_status = AddRemoveItemStatus.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String)
                Me.m_MPA = Nothing
                Me.m_strName = strName
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    Me.m_bOpenMonths(iMonth) = False
                Next
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
            ''' Get the <see cref="cEcospaceMPA">EwE MPA</see> associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property MPA() As cEcospaceMPA
                Get
                    Return Me.m_MPA
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the months that an MPA is open for fishing.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property MPAMonths() As Boolean()
                Get
                    Return Me.m_bOpenMonths
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set open months in this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property IsOpen(ByVal iMonth As Integer) As Boolean
                Get
                    Return Me.m_bOpenMonths(iMonth)
                End Get
                Set(ByVal value As Boolean)
                    Me.m_bOpenMonths(iMonth) = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
            ''' for the MPA object.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Status() As AddRemoveItemStatus
                Get
                    Return Me.m_status
                End Get
            End Property

            Public Function IsNew() As Boolean
                Return (Me.m_MPA Is Nothing)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the MPA has changed.
            ''' </summary>
            ''' <returns>
            ''' True when MPA <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged() As Boolean
                Dim bChanged As Boolean = False
                If Me.m_MPA Is Nothing Then Return False
                bChanged = (Me.m_MPA.Name <> Me.m_strName)
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    bChanged = bChanged Or (Me.m_bOpenMonths(iMonth) <> Me.m_MPA.MPAMonth(iMonth))
                Next
                Return bChanged
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this MPA is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = AddRemoveItemStatus.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Me.m_MPA IsNot Nothing Then
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
            Me.FixedColumnWidths = False

            ' Set up visual models for reflecting MPA modification status
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

            ' MPA index cell
            Me(0, eColumnTypes.MPAIndex) = New EwEColumnHeaderCell()
            ' MPA name cell, editable this time
            Me(0, eColumnTypes.MPAName) = New EwEColumnHeaderCell(My.Resources.HEADER_MPA)
            Me(0, eColumnTypes.MPAAll) = New EwEColumnHeaderCell("Closed")
            'Define column header Jan - Dec
            For iCol As Integer = eColumnTypes.MPAJan To eColumnTypes.MPADec
                Dim d As New Date(1, (iCol - eColumnTypes.MPAJan) + 1, 1)
                Me(0, iCol) = New EwEColumnHeaderCell(d.ToString("MMM"))
            Next

            ' MPA index cell
            Me(0, eColumnTypes.MPAStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

            ' Fix index column only; MPA name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

        End Sub

        Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
            Return DockStyle.None
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the MPA/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            ' Get the core reference
            Dim core As cCore = cCore.GetInstance()
            Dim MPA As cEcospaceMPA = Nothing
            Dim mi As MPAInfo = Nothing

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of MPA configuration 
            For iMPA As Integer = 1 To core.nMPAs
                MPA = core.EcospaceMPAs(iMPA)
                mi = New MPAInfo(MPA)
                Me.m_alMPAs.Add(mi)
            Next

            ' Brute-force update grid
            UpdateGrid()

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Finish the style
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Me.Columns(eColumnTypes.MPAIndex).Width = 20
            Me.Columns(eColumnTypes.MPAName).Width = 80
            Me.Columns(eColumnTypes.MPAAll).Width = 60
            For col As eColumnTypes = eColumnTypes.MPAJan To eColumnTypes.MPADec
                Me.Columns(col).Width = 40
            Next
            Me.Columns(eColumnTypes.MPAStatus).Width = 80

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Brute-force resize the gird if necessary, and repopulate with data from 
        ''' the local administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub UpdateGrid()

            Dim mi As MPAInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_alMPAs.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.MPAIndex) = ewec

                Me(iRow, eColumnTypes.MPAName) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.MPAName).Behaviors.Add(m_bm)

                Me(iRow, eColumnTypes.MPAAll) = New Cells.Real.CheckBox(False)
                Me(iRow, eColumnTypes.MPAAll).Behaviors.Add(m_bm)

                For iMonth As Integer = 1 To cCore.N_MONTHS
                    Me(iRow, eColumnTypes.MPAJan - 1 + iMonth) = New Cells.Real.CheckBox(False)
                    Me(iRow, eColumnTypes.MPAJan - 1 + iMonth).Behaviors.Add(m_bm)
                Next iMonth

                ' Status
                vm = New VisualModels.Common()
                vm.ImageAlignment = ContentAlignment.MiddleCenter
                Me(iRow, eColumnTypes.MPAStatus) = New Cells.Real.Cell()
                Dim dm As New DataModels.DataModelBase(GetType(String))
                dm.EditableMode = EditableMode.None
                Me(iRow, eColumnTypes.MPAStatus).DataModel = dm
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_alMPAs.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTMPAROW)
            End While

            ' Sanity check whether grid can accomodate all MPAs + header
            Debug.Assert(Me.Rows.Count = Me.m_alMPAs.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_alMPAs.Count
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

            Dim mi As MPAInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim strText As String = ""
            Dim iNumOpen As Integer = 0

            Me.AllowUpdates = False

            mi = DirectCast(Me.m_alMPAs(iRow - iFIRSTMPAROW), MPAInfo)
            ri = Me.Rows(iRow)

            ri.Tag = mi
            aCells = ri.GetCells()

            ' Set index
            pos = New Position(iRow, eColumnTypes.MPAIndex)
            aCells(eColumnTypes.MPAIndex).SetValue(pos, CInt(iRow))

            ' Set name
            pos = New Position(iRow, eColumnTypes.MPAName)
            aCells(eColumnTypes.MPAName).SetValue(pos, CStr(mi.Name))

            ' Set montly states
            For iMonth As Integer = 1 To cCore.N_MONTHS
                pos = New Position(iRow, eColumnTypes.MPAJan - 1 + iMonth)
                ' Display a check when the MPA is NOT open for fishing
                aCells(eColumnTypes.MPAJan - 1 + iMonth).SetValue(pos, Not mi.IsOpen(iMonth))
                If mi.IsOpen(iMonth) Then iNumOpen += 1
            Next

            ' Display a check when the MPA is NOT open for fishing
            aCells(eColumnTypes.MPAAll).SetValue(pos, (iNumOpen = 0))

            Select Case mi.Status
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

            ' Set modification status
            pos = New Position(iRow, eColumnTypes.MPAStatus)
            aCells(eColumnTypes.MPAStatus).VisualModel = vm
            aCells(eColumnTypes.MPAStatus).SetValue(pos, strText)

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

            Dim mi As MPAInfo = DirectCast(Me.m_alMPAs(p.Row - 1), MPAInfo)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.MPAName
                    ' JS: Handled in OnCellEdited()
                    ' mi.Name = CStr(cell.GetValue(p))

                Case eColumnTypes.MPAAll
                    For i As Integer = 1 To cCore.N_MONTHS
                        mi.IsOpen(i) = Not CBool(cell.GetValue(p))
                    Next
                    Me.UpdateRow(p.Row)

                Case eColumnTypes.MPAJan To eColumnTypes.MPADec
                    mi.IsOpen(p.Column + 1 - CInt(eColumnTypes.MPAJan)) = Not CBool(cell.GetValue(p))
                    Me.UpdateRow(p.Row)

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

            Dim mi As MPAInfo = DirectCast(Me.m_alMPAs(p.Row - 1), MPAInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.MPAIndex
                    ' Not possible

                Case eColumnTypes.MPAName
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iMPA As Integer = 0 To Me.m_alMPAs.Count - 1
                        Dim giTemp As MPAInfo = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)
                        ' Does name already exist?
                        If (Not Object.ReferenceEquals(giTemp, mi)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    mi.Name = strName

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

            Dim iMPA As Integer = iRow - iFIRSTMPAROW
            Dim mi As MPAInfo = Nothing

            ' Validate
            If iMPA < 0 Then Return

            mi = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)
            ' Toggle 'flagged for deletion' flag
            mi.FlaggedForDeletion = Not mi.FlaggedForDeletion

            ' Check to see what is to happen to the MPA now
            Select Case mi.Status

                Case AddRemoveItemStatus.Original
                    ' Clear removed status of the MPA
                    Me.m_alMPAsRemoved.Remove(Me.m_alMPAs(iMPA))

                Case AddRemoveItemStatus.Added
                    ' Clear removed status of the MPA
                    Me.m_alMPAsRemoved.Remove(Me.m_alMPAs(iMPA))

                Case AddRemoveItemStatus.Removed
                    ' Set removed status
                    Me.m_alMPAsRemoved.Add(Me.m_alMPAs(iMPA))

                Case AddRemoveItemStatus.Invalid
                    ' Set removed status
                    Me.m_alMPAs.RemoveAt(iMPA)

            End Select

            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a MPA.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsMPARow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTMPAROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the MPA on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsMPARow(iRow) Then Return False

            Dim iMPA As Integer = iRow - iFIRSTMPAROW
            Dim mi As MPAInfo = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)

            Return mi.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new MPA.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateMPA()
        End Sub

        ''' <summary>
        ''' Create a new MPA.
        ''' </summary>
        Private Sub CreateMPA()
            Dim iRow As Integer = -1
            Dim iMPA As Integer = -1
            Dim mi As MPAInfo = Nothing
            Dim lstrMPAs As New List(Of String)

            ' Make fit
            iRow = Math.Max(iFIRSTMPAROW, Me.RowsCount)
            iMPA = iRow - iFIRSTMPAROW

            ' Validate
            If iMPA < 0 Then Return

            ' Collect all current MPA names
            For Each mi In Me.m_alMPAs
                lstrMPAs.Add(mi.Name)
            Next

            mi = New MPAInfo(String.Format(My.Resources.DEFAULT_NEWMPA_NUM, _
                    cStringUtils.GetNextNumber(lstrMPAs.ToArray(), My.Resources.DEFAULT_NEWMPA_NUM)))
            Me.m_alMPAs.Insert(iMPA, mi)

            Me.UpdateGrid()
            Me.SelectRow(mi)
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

        Private Sub SelectRow(ByVal mi As MPAInfo)
            For iMPA As Integer = 0 To Me.m_alMPAs.Count - 1
                If Object.ReferenceEquals(Me.m_alMPAs(iMPA), mi) Then
                    Me.SelectRow(iMPA + iFIRSTMPAROW)
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
        ''' MPA configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean

            '' Check if the user is about to delete all fleets - one should remain
            'If Me.m_alMPAsRemoved.Count = Me.m_alMPAs.Count Then
            '    MsgBox(My.Resources.ECOPATH_EDITMPA_PROMPT_CANNOTDELETEALL, _
            '            MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly, _
            '            My.Resources.ECOPATH_EDITMPA_CONFIRMDELETE_CAPTION)
            '    Return False
            'End If

            Return True

        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bMPAsChanged As Boolean = False
            Dim mi As MPAInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim MPA As cEcospaceMPA = Nothing
            Dim iMPA As Integer = 0
            Dim iDeleteCount As Integer = 0
            Dim bSuccess As Boolean = True

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess MPA changes
            For iMPA = 0 To Me.m_alMPAs.Count - 1
                mi = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)
                ' Check if this MPA is newly added
                bConfigurationChanged = bConfigurationChanged Or mi.IsNew()
                ' Check if this MPA has been changed
                bMPAsChanged = bMPAsChanged Or mi.IsChanged()
            Next iMPA

            ' Assess MPAs to remove
            iDeleteCount = 0
            For iMPA = 0 To Me.m_alMPAsRemoved.Count - 1
                mi = DirectCast(Me.m_alMPAsRemoved(iMPA), MPAInfo)
                If (Not mi.IsNew()) Then iDeleteCount += 1
            Next iMPA

            If (iDeleteCount > 0) Then

                strPrompt = String.Format(My.Resources.ECOSPACE_EDITMPA_CONFIRMDELETE_PROMPT, iDeleteCount)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.No
                        ' Do not delete MPAs
                        iDeleteCount = 0
                    Case MsgBoxResult.Yes
                        ' Delete MPAs
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.m_core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

                cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

                ' Add new MPAs
                For iMPA = 0 To Me.m_alMPAs.Count - 1
                    mi = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)
                    If (mi.IsNew()) Then
                        bSuccess = bSuccess And Me.m_core.AddEcospaceMPA(mi.Name, mi.MPAMonths, iDBID)
                    End If
                Next

                ' Remove MPAs
                If iDeleteCount > 0 Then
                    For iMPA = 0 To Me.m_alMPAsRemoved.Count - 1
                        mi = DirectCast(Me.m_alMPAsRemoved(iMPA), MPAInfo)
                        If (Not mi.IsNew()) Then
                            If (Me.m_core.RemoveEcospaceMPA(mi.MPA)) Then
                                Me.m_alMPAs.Remove(mi)
                            Else
                                bSuccess = False
                            End If
                        End If
                    Next iMPA
                    If bSuccess Then Me.m_alMPAsRemoved.Clear()
                End If

                ' The core will reload now
                If bSuccess Then
                    bSuccess = Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace, True)
                    ' Test whether new MPAs were loaded correctly
                    Debug.Assert(Me.m_alMPAs.Count = Me.m_core.nMPAs, ">> Internal panic: Dialog and core out of sync on MPAs")
                Else
                    Me.m_core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)
                End If
                cApplicationStatusNotifier.SetStatusText("", TriState.False)
            End If

                ' Update core objects
            If (bMPAsChanged) Then
                ' For each local MPA admin unit
                For iMPA = 0 To Me.m_alMPAs.Count - 1
                    ' Get local admin unit
                    mi = DirectCast(Me.m_alMPAs(iMPA), MPAInfo)
                    ' Has it changed?
                    If mi.IsChanged() Then
                        ' Find core MPA with same BDID (cannot use cached cEcospaceMPA instances since the core has reloaded)
                        Dim bFound As Boolean = False
                        ' For every core MPA instance
                        For iMPATest As Integer = 1 To Me.m_core.nMPAs
                            ' Get core MPA instance
                            Dim MPATest As cEcospaceMPA = Me.m_core.EcospaceMPAs(iMPATest)
                            ' Has matching ID?
                            If (MPATest.getID = mi.MPA.getID) Then
                                ' #Yes: Update
                                MPATest.Name = mi.Name
                                For iMonth As Integer = 1 To cCore.N_MONTHS
                                    MPATest.MPAMonth(iMonth) = mi.IsOpen(iMonth)
                                Next
                                ' Happy, happy, happy
                                bFound = True
                            End If
                        Next
                        ' All went well?
                        If Not bFound Then
                            ' #No?! Uh oh...
                            Debug.Assert(False, ">> Internal panic: Unable to apply changes to MPA id " & mi.MPA.getID)
                        End If
                    End If
                Next

                '' Apply all changes
                'Me.m_core.SaveEcospaceScenario()
            End If

            Return bSuccess

        End Function


#End Region ' Apply changes

    End Class

End Namespace
