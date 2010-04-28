#Region " Imports "

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.ComponentModel
Imports SourceGrid2
Imports SourceGrid2.Cells.Real
Imports SourceLibrary
Imports ScientificInterfaceShared.Properties
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports System.IO
Imports SourceGrid2.Cells
Imports System.Globalization
Imports System.Threading
Imports ScientificInterfaceShared.Style

#End Region

Namespace Controls.EwEGrid

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' This class provides a <see cref="SourceGrid2.Grid">SourceGrid2.Grid</see>
    ''' derived grid for displaying EwE6 data. Basic interaction and presentation
    ''' styles are defined, and key points of interaction must be overridden to
    ''' complete an customized grid.
    ''' </summary>
    ''' <example>
    ''' The following code illustrates how to create and populate a custom EwE6 grid:
    ''' <code>
    ''' Public Class BasicInputEwEGrid
    '''    : Inherits EwEGrid
    '''
    '''    Protected Overrides Sub InitStyle()
    '''    
    '''        MyBase.InitStyle()
    '''    
    '''        Me.Redim(1, 10)
    '''        Me(0, 0) = New EwEColumnHeaderCell("")
    '''        Me(0, 1) = New EwEColumnHeaderCell(My.Resources.ECOPATH_HEADER_GROUPNAME)
    '''        Me(0, 2) = New EwEColumnHeaderCell(My.Resources.HEADER_AREA)
    '''        Me(0, 3) = New EwEColumnHeaderCell(My.Resources.ECOPATH_HEADER_BIOMASSAREA)
    '''        Me.FixedColumns = 2
    '''    
    '''    End Sub
    '''    
    '''    Protected Overrides Sub FillData()
    '''    
    '''    Dim core As cCore = cCore.GetInstance()
    '''    Dim source As cCoreInputOutputBase = Nothing
    '''    
    '''       Me.Rows.Clear()
    ''' 
    '''       For groupIndex As Integer = 1 To core.nGroups
    '''           Me.Rows.Insert(groupIndex)
    '''    
    '''           source = core.EcoPathGroupInputs(groupIndex)
    '''    
    '''           Me(groupIndex, 0) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Index)
    '''           Me(groupIndex, 1) = New PropertyRowHeaderCell(Me.PropertyManager, source, eVarNameFlags.Name)
    '''           Me(groupIndex, 2) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.Area)
    '''           Me(groupIndex, 3) = New PropertyCell(Me.PropertyManager, source, eVarNameFlags.BiomassAreaInput)
    '''    
    '''       Next groupIndex
    '''    
    '''    End Sub
    '''    
    ''' End Class
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public MustInherit Class EwEGrid
        Inherits SourceGrid2.Grid
        Implements IUIElement

#Region " Public helper classes "

        Public Class EndEditHandler
            Implements BehaviorModels.IBehaviorModel

            ''' <summary></summary>
            Private m_grid As EwEGrid = Nothing

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="grid"></param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal grid As EwEGrid)
                Me.m_grid = grid
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property CanReceiveFocus() As Boolean Implements SourceGrid2.BehaviorModels.IBehaviorModel.CanReceiveFocus
                Get
                    Return True
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnClick(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnClick
                Me.m_grid.OnCellClicked(e.Position, e.Cell)
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnContextMenuPopUp(ByVal e As SourceGrid2.PositionContextMenuEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnContextMenuPopUp
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnDoubleClick(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnDoubleClick
            End Sub

            Public Sub OnEditEnded(ByVal e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnEditEnded
                e.Cancel = Not Me.m_grid.OnCellEdited(e.Position, e.Cell)
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnEditStarting(ByVal e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnEditStarting
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnFocusEntered(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusEntered
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnFocusEntering(ByVal e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusEntering
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnFocusLeaving(ByVal e As SourceGrid2.PositionCancelEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusLeaving
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnFocusLeft(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnFocusLeft
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnKeyDown(ByVal e As SourceGrid2.PositionKeyEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyDown
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnKeyPress(ByVal e As SourceGrid2.PositionKeyPressEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyPress
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnKeyUp(ByVal e As SourceGrid2.PositionKeyEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnKeyUp
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnMouseDown(ByVal e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseDown
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnMouseEnter(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseEnter
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnMouseLeave(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseLeave
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnMouseMove(ByVal e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseMove
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnMouseUp(ByVal e As SourceGrid2.PositionMouseEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnMouseUp
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' 
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Sub OnValueChanged(ByVal e As SourceGrid2.PositionEventArgs) Implements SourceGrid2.BehaviorModels.IBehaviorModel.OnValueChanged
                Me.m_grid.OnCellValueChanged(e.Position, e.Cell)
            End Sub

        End Class

#End Region ' Public helper classes

#Region " Variables "

        ''' <summary>The UI context for this grid.</summary>
        Private m_uic As cUIContext = Nothing

        ''' <summary>Position event handler for trapping top-left cell clicks.</summary>
        Private m_pehTLCell As SourceGrid2.PositionEventHandler = Nothing
        ''' <summary>Cell click behaviour model.</summary>
        Private m_ceCellClick As New BehaviorModels.CustomEvents

        ''' <summary>Position event handler for trapping row header clicks.</summary>
        Private m_pehRowHeader As SourceGrid2.PositionEventHandler = Nothing
        ''' <summary>Row click behaviour model.</summary>
        Private m_ceRowSelect As New BehaviorModels.CustomEvents

        ''' <summary>Position event handler for trapping column header clicks.</summary>
        Private m_pehColHeader As SourceGrid2.PositionEventHandler = Nothing
        ''' <summary>Column click behaviour model.</summary>
        Private m_ceColSelect As New BehaviorModels.CustomEvents

        ''' <summary>Flag stating if this grid should track and distribute property selections.</summary>
        Private m_bTrackPropertySelection As Boolean = False

        ''' <summary>List of selected properties in the grid, if any.</summary>
        Private m_lpropertySelected As New List(Of cProperty)

        ''' <summary>Flag stating to use fixed col widths and heights.</summary>
        Private m_bFixedColumnWidths As Boolean = True

#End Region ' Variables

#Region " Constructor / destructor "

        Public Sub New()
            MyBase.New()

            Me.m_pehTLCell = New SourceGrid2.PositionEventHandler(AddressOf bm_tlCellClick)
            AddHandler m_ceCellClick.Click, Me.m_pehTLCell
            Me.m_pehRowHeader = New SourceGrid2.PositionEventHandler(AddressOf bm_rowSelectClick)
            AddHandler m_ceRowSelect.Click, Me.m_pehRowHeader
            Me.m_pehColHeader = New SourceGrid2.PositionEventHandler(AddressOf bm_colSelectClick)
            AddHandler m_ceColSelect.Click, Me.m_pehColHeader

            AddHandler Me.Selection.ClipboardCopy, AddressOf OnClipboardCopy
            AddHandler Me.Selection.ClipboardCut, AddressOf OnClipboardCut
            AddHandler Me.Selection.ClipboardPaste, AddressOf OnClipboardPaste
            AddHandler Me.Selection.ClearCells, AddressOf OnClearCells

            Me.TrackPropertySelection = True
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)

            Me.UIContext = Nothing

            If Me.m_pehTLCell IsNot Nothing Then

                RemoveHandler m_ceCellClick.Click, Me.m_pehTLCell
                Me.m_pehTLCell = Nothing
                RemoveHandler m_ceRowSelect.Click, Me.m_pehRowHeader
                Me.m_pehRowHeader = Nothing
                RemoveHandler m_ceColSelect.Click, Me.m_pehColHeader
                Me.m_pehColHeader = Nothing

                RemoveHandler Me.Selection.ClipboardCopy, AddressOf OnClipboardCopy
                RemoveHandler Me.Selection.ClipboardCut, AddressOf OnClipboardCut
                RemoveHandler Me.Selection.ClipboardPaste, AddressOf OnClipboardPaste
                RemoveHandler Me.Selection.ClearCells, AddressOf OnClearCells
                RemoveHandler Me.Selection.SelectionChange, AddressOf OnSelectionChange

                Me.TrackPropertySelection = False

            End If

        End Sub

#End Region ' Constructor / destructor

#Region " IUIElement implementation "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cUIContext">UI Context</see> for this grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)

                ' Clean-up
                If (Me.m_uic IsNot Nothing) Then
                    Me.ClearData()
                End If

                ' Store UIC
                Me.m_uic = value

                ' Refresh when setting
                If (Me.m_uic IsNot Nothing) Then
                    Me.RefreshContent()
                End If

            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCore">core</see> that this grid connects to.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Core() As cCore
            Get
                If Me.UIContext Is Nothing Then Return Nothing
                Return Me.UIContext.Core
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cStyleGuide">style guide</see> that this grid 
        ''' connects to.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property StyleGuide() As cStyleGuide
            Get
                If Me.UIContext Is Nothing Then Return Nothing
                Return Me.UIContext.StyleGuide
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cPropertyManager">property manager</see> that 
        ''' this grid can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property PropertyManager() As cPropertyManager
            Get
                If Me.UIContext Is Nothing Then Return Nothing
                Return Me.UIContext.PropertyManager
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cCommandHandler">command handler</see> that 
        ''' this grid can interact with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ComandHandler() As cCommandHandler
            Get
                If Me.UIContext Is Nothing Then Return Nothing
                Return Me.UIContext.CommandHandler
            End Get
        End Property

#End Region ' IUIElement implementation

#Region " EwE events "

        Public Event OnSelectionChanged(ByVal selection As SourceGrid2.CellVirtualCollection)

        Protected Sub RaiseSelectionChangeEvent()
            If Me.UIContext IsNot Nothing Then
                RaiseEvent OnSelectionChanged(Me.Selection.GetCells())
            End If
        End Sub

#End Region ' EwE events

#Region " Appearance "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to trigger the EwE process of <see cref="SourceGrid2.Grid.InitLayout">initializing</see>, 
        ''' <see cref="InitStyle">styling</see>, <see cref="FillData">populating</see> and
        ''' <see cref="FinishStyle">finalizing</see> the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub InitLayout()
            MyBase.InitLayout()

            Dim bIsUIContent As Boolean = (Me.UIContext IsNot Nothing)
            Dim bIsDesigning As Boolean = (Me.DesignMode = True)
            Dim bIsLife As Boolean = bIsUIContent And Not bIsDesigning

            Me.SuspendLayoutGrid()

            Try
                ' Clear grid of any remaining data
                Me.ClearData()
            Catch ex As Exception
                Debug.Assert(False, "Exception " & ex.Message & " in ClearData")
            End Try

            Try
                ' Style the grid only when designing OR fully live
                If bIsDesigning Or bIsLife Then
                    Me.InitStyle()
                End If
            Catch ex As Exception
                Debug.Assert(False, "Exception " & ex.Message & " in InitStyle: check if grid is using a missing UI context")
            End Try

            If (bIsLife) Then
                Try
                    Me.FillData()
                Catch ex As Exception
                    Debug.Assert(False, "Exception " & ex.Message & " in FillData")
                End Try
            End If

            Try
                ' Style the grid only when designing OR fully live
                If bIsDesigning Or bIsLife Then
                    Me.FinishStyle()
                End If
            Catch ex As Exception
                Debug.Assert(False, "Exception " & ex.Message & " in FinishStyle")
            End Try

            Me.ResumeLayoutGrid()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Provides a grid with starndard EwE appearances and behaviours.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub InitStyle()

            Me.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            Me.BackColor = Color.White
            Me.FixedColumns = 2
            Me.FixedRows = 1
            Me.GridToolTipActive = True
            Me.ContextMenuStyle = SourceGrid2.ContextMenuStyle.CellContextMenu Or _
                                  SourceGrid2.ContextMenuStyle.CopyPasteSelection Or _
                                  SourceGrid2.ContextMenuStyle.ColumnResize Or _
                                  SourceGrid2.ContextMenuStyle.AutoSize

            Me.AutoStretchRowsToFitHeight = False

            ' JS 05aug07: this flag controls whether selections can be made with cell nav keys and [ctrl] and/or [shift]
            '             It does not seem to work well though; when set to True it is impossible to select a range w
            '             [shift] and [ctrl] pressed. This is different from Excel and other grids. Let this be a known
            '             issue but let's not waste time on this issue right now.
            Me.Selection.EnableMultiSelection = True

            ' JS 06aug07: taking care of copy/paste ourselves
            Me.Selection.AutoCopyPaste = False

            Me.Selection.AutoClear = False

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Finalizes the grid by formatting the grid header and column widths to indicated sizes after data has been provided.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub FinishStyle()

            Dim cell As ICell = Nothing

            Me.AutoSizeAll()

            'Add the selection of whole grid.
            If (Me.RowsCount > 0) And (Me.ColumnsCount > 0) Then
                cell = Me(0, 0)
                If cell IsNot Nothing Then cell.Behaviors.Add(Me.m_ceCellClick)
            End If

            'Add the selection of whole row while clicking first column
            For i As Integer = 1 To Me.RowsCount - 1
                cell = Me(i, 0)
                If cell IsNot Nothing Then cell.Behaviors.Add(Me.m_ceRowSelect)
            Next

            'Add the selection of whole column while clicking first row 
            For i As Integer = 1 To Me.ColumnsCount - 1
                cell = Me(0, i)
                If cell IsNot Nothing Then cell.Behaviors.Add(Me.m_ceColSelect)
            Next

            Me.FixedColumnWidths = Me.m_bFixedColumnWidths

            ' Sanity checks
            If (Me.FocusStyle <> SourceGrid2.FocusStyle.None) Then
                Console.WriteLine("Warning: grid {0} ({1}) focus style may cause problems", Me.Name, Me.GetType().FullName)
            End If

        End Sub

        Protected Overridable Function OnCellEdited(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean
            Return True
        End Function

        Protected Overridable Function OnCellValueChanged(ByVal p As Position, ByVal cell As Cells.ICellVirtual) As Boolean
            Return True
        End Function

        Protected Overridable Sub OnCellClicked(ByVal p As Position, ByVal cell As Cells.ICellVirtual)
            ' NOP
        End Sub

        ''' <summary>
        ''' Flag, states whether columns are fixed in width and height.
        ''' </summary>
        ''' <remarks>
        ''' When True, the header row is set to a fixed height of 45 (shudder)
        ''' </remarks>
        <Browsable(True), Description("States whether columns are fixed in width and height")> _
        Public Property FixedColumnWidths() As Boolean
            Get
                Return m_bFixedColumnWidths
            End Get
            Set(ByVal bFixedColumnWidths As Boolean)
                'If (m_bFixedColumnWidths = bFixedColumnWidths) Then Return

                Me.m_bFixedColumnWidths = bFixedColumnWidths
                If (Me.RowsCount > 0) And (Me.ColumnsCount > 0) Then
                    If (Me.m_bFixedColumnWidths = True) Then
                        For i As Integer = 2 To Me.ColumnsCount - 1
                            Me.Columns(i).Width = 80
                        Next
                        Me.Rows(0).Height = 45
                        Me.AutoStretchColumnsToFitWidth = False
                    Else
                        For i As Integer = 2 To Me.ColumnsCount - 1
                            Me.Columns(i).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                        Next
                        Me.Rows(0).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableAutoSize
                        Me.AutoSizeAll()
                    End If
                End If
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Flag, states whether the grid will maintain a list of 
        ''' <see cref="SelectedProperties">selected properties</see>.
        ''' </summary>
        ''' <remarks>
        ''' It is advised to set this setting to False for larger grids.
        ''' </remarks>
        <Browsable(True), Description("States whether the grid maintains a list of selected cProperty instances.")> _
      Public Property TrackPropertySelection() As Boolean
            Get
                Return Me.m_bTrackPropertySelection
            End Get
            Set(ByVal value As Boolean)
                If Me.m_bTrackPropertySelection <> value Then
                    If Me.m_bTrackPropertySelection Then
                        RemoveHandler Me.Selection.SelectionChange, AddressOf OnSelectionChange
                    End If
                    Me.m_bTrackPropertySelection = value
                    If Me.m_bTrackPropertySelection Then
                        AddHandler Me.Selection.SelectionChange, AddressOf OnSelectionChange
                    End If
                End If
            End Set
        End Property

#End Region ' Appearance

#Region " Data "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the entire content of the grid.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub RefreshContent()
            Me.InitLayout()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Override this to fill the grid with data.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected MustOverride Sub FillData()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Properly releases all EwE cells in the grid.
        ''' </summary>
        ''' <note_js>Method does not require UI context to be present.</note_js>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub ClearData()
            For iRow As Integer = 0 To Me.RowsCount - 1
                Me.ClearRow(iRow)
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Properly releases all EwE cells in a row.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub ClearRow(ByVal iRow As Integer)
            Dim cell As SourceGrid2.Cells.ICell = Nothing
            For iCol As Integer = 0 To Me.ColumnsCount - 1
                cell = Me(iRow, iCol)
                If cell IsNot Nothing Then
                    If TypeOf (cell) Is EwECellBase Then
                        ' Clear the cell
                        DirectCast(cell, EwECellBase).Dispose()
                        ' ..and get rid of it
                        Me(iRow, iCol) = Nothing
                    End If
                End If
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a row to the grid.
        ''' </summary>
        ''' <param name="iRowIndex"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Protected Function AddRow(Optional ByVal iRowIndex As Integer = -1) As Integer
            If (-1 = iRowIndex) Then iRowIndex = Me.Rows.Count
            Me.Rows.Insert(iRowIndex)
            Return iRowIndex
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constant; returns the default text for 'value not available'.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected ReadOnly Property DataNotAvailable() As String
            Get
                Return My.Resources.GENERIC_VALUE_NOTAVAILABLE
            End Get
        End Property

        Public Overridable ReadOnly Property MessageSource() As eCoreComponentType
            Get
                Return eCoreComponentType.NotSet
            End Get
        End Property

        Public Overridable ReadOnly Property MessageSources() As eCoreComponentType()
            Get
                Return New eCoreComponentType() {Me.MessageSource}
            End Get
        End Property

        Public Overridable Sub OnCoreMessage(ByVal msg As cMessage)
            If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                Me.RefreshContent()
            End If
        End Sub

#End Region ' Data

#Region " Selection behavior "

        ' ToDo_JS 05aug07: fix [SHIFT]+key nav selection logic to select a range, not just select a cell

        Protected Overridable Sub bm_tlCellClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
            ' JS 05aug07: no need to process keys here; shift and ctrl modifiers behave just fine
            ' JS 05aug07: on second thought: it doesn't. [SHIFT]+[CTRL] click should ADD to a selection, not replace it
            Me.Selection.AddRange(New Range(0, 0, Me.RowsCount - 1, Me.ColumnsCount - 1))
        End Sub

        Protected Overridable Sub bm_rowSelectClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
            ' JS 05aug 07: select range of rows if shift pressed
            Dim iFirstRow As Integer = e.Position.Row
            Dim iLastRow As Integer = e.Position.Row

            If ((Control.ModifierKeys And Keys.Shift) = Keys.Shift) Then
                iFirstRow = Math.Min(Selection.GetRange.Start.Row, iFirstRow)
                iLastRow = Math.Min(Selection.GetRange.End.Row, iLastRow)
            End If

            Me.Selection.AddRange(New Range(iFirstRow, 0, iLastRow, Me.ColumnsCount - 1))
        End Sub

        Protected Overridable Sub bm_colSelectClick(ByVal sender As Object, ByVal e As SourceGrid2.PositionEventArgs)
            ' JS 05aug07: select range of columns if shift pressed
            Dim iFirstCol As Integer = e.Position.Column
            Dim iLastCol As Integer = e.Position.Column

            If ((Control.ModifierKeys And Keys.Shift) = Keys.Shift) Then
                iFirstCol = Math.Min(Selection.GetRange.Start.Column, iFirstCol)
                iLastCol = Math.Min(Selection.GetRange.End.Column, iLastCol)
            End If

            Me.Selection.AddRange(New Range(0, iFirstCol, Me.RowsCount - 1, iLastCol))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnClearCells(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim cell As SourceGrid2.Cells.ICell = Nothing

            For Each pos As Position In Me.Selection.GetCellsPositions()
                cell = Me(pos.Row, pos.Column)
                If cell.DataModel.EditableMode <> EditableMode.None And cell.DataModel.EnableEdit = True Then
                    cell.SetValue(pos, Nothing)
                End If
            Next
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clipboard copy, implemented to return actual property cell values and
        ''' style-masked values in the clipboard text.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnClipboardCopy(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim r As Range = Me.Selection.GetRange()
            Dim pos As Position = Nothing
            Dim prop As cProperty = Nothing
            Dim sbClipText As New StringBuilder
            Dim strValue As String = ""
            Dim bIgnoreSelection As Boolean = False
            Dim cell As Cells.ICell = Nothing

            ' Empty or near-empty range?
            If (r.IsEmpty) Then
                ' Select remaining grid
                r = New Range(r.Start.Row, r.Start.Column, r.Start.Row, r.Start.Column)
                ' Ignore selected cells
                bIgnoreSelection = True
            End If

            For iRow As Integer = r.Start.Row To r.End.Row
                If iRow > r.Start.Row Then sbClipText.Append(vbCr + vbLf)
                For iCol As Integer = r.Start.Column To r.End.Column
                    pos = New Position(iRow, iCol)
                    strValue = ""

                    If (Me.Selection.Contains(pos) Or bIgnoreSelection) Then
                        cell = Me(iRow, iCol)
                        If cell IsNot Nothing Then
                            If TypeOf cell Is PropertyCell Then
                                prop = DirectCast(cell, PropertyCell).GetProperty()
                                strValue = CStr(prop.GetValue(False))
                            Else
                                strValue = CStr(Me(iRow, iCol).GetValue(pos))
                            End If
                        End If
                    End If

                    If String.Compare(strValue, CStr(cCore.NULL_VALUE)) = 0 Then strValue = ""

                    ' Add to clip text
                    If iCol > r.Start.Column Then sbClipText.Append(vbTab)
                    sbClipText.Append(strValue)
                Next iCol
            Next iRow

            Dim dobj As New DataObject()
            dobj.SetData(DataFormats.Text, sbClipText.ToString())
            Clipboard.SetDataObject(dobj, True)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnClipboardCut(ByVal sender As Object, ByVal e As System.EventArgs)

            Me.OnClipboardCopy(sender, e)
            Me.OnClearCells(sender, e)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clipboard paste
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnClipboardPaste(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim dtObj As IDataObject = Clipboard.GetDataObject()

            If dtObj.GetDataPresent(DataFormats.Text, True) = False Then Return

            Dim strData As String = CStr(dtObj.GetData(DataFormats.Text)).Replace(CStr(vbCr + vbLf), CStr(vbLf))
            Dim astrLines() As String = strData.Split(New Char() {CChar(vbCr), CChar(vbLf)})
            Dim r As Range = Me.Selection.GetRange()
            Dim pos As Position = Nothing
            Dim cell As SourceGrid2.Cells.ICell = Nothing
            Dim strValue As String = ""

            ' Empty or near-empty range?
            If (r.IsEmpty) Then
                ' Select remaining grid
                r = New Range(r.Start.Row, r.Start.Column, Me.RowsCount - r.Start.Row, Me.ColumnsCount - r.Start.Column)
            End If

            ' JS 29aug09: paste behaviour changed to imitate Excel. Do not only paste in selected cells,
            '             but paste 'all the way through'
            For iRow As Integer = r.Start.Row To Math.Min(r.Start.Row + astrLines.Length - 1, Me.RowsCount - 1)
                If Not String.IsNullOrEmpty(astrLines(iRow - r.Start.Row)) Then
                    Dim astrCols() As String = astrLines(iRow - r.Start.Row).Split(CChar(vbTab))

                    For iCol As Integer = r.Start.Column To Math.Min(r.Start.Column + astrCols.Length - 1, Me.ColumnsCount - 1)
                        pos = New Position(iRow, iCol)
                        cell = Me(iRow, iCol)

                        ' Prevent from crashing on irregular grids
                        If cell IsNot Nothing Then
                            ' Is cell enabled for editing?
                            If (cell.DataModel.EnableEdit) Then
                                ' #Yes: attempt to set value
                                strValue = astrCols(iCol - r.Start.Column)

                                If (String.Compare(strValue, "") = 0) And _
                                    ((cell.DataModel.ValueType Is GetType(Single) Or cell.DataModel.ValueType Is GetType(Double) Or cell.DataModel.ValueType Is GetType(Integer))) Then
                                    strValue = cCore.NULL_VALUE.ToString()
                                End If

                                cell.SetValue(pos, strValue)
                            End If
                        End If
                    Next iCol
                End If
            Next iRow
            ' Redraw later
            Me.InvalidateCells()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Seclection change event handler; implemented to fire a 
        ''' <see cref="cPropertySelectionCommand">property select command</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub OnSelectionChange(ByVal sender As Object, ByVal e As SourceGrid2.SelectionChangeEventArgs)

            Dim cmdh As cCommandHandler = Me.ComandHandler
            Dim cmd As cCommand = cmdh.GetCommand(cPropertySelectionCommand.COMMAND_NAME)
            Dim sc As cPropertySelectionCommand = Nothing
            Dim c As SourceGrid2.Cells.ICell = Nothing

            Me.m_lpropertySelected.Clear()

            If e.EventType <> SelectionChangeEventType.Clear Then

                ' Get properties from selected cells
                For Each p As Position In Me.Selection.GetCellsPositions
                    c = Me(p.Row, p.Column)
                    If c IsNot Nothing Then
                        ' Is property cell?
                        If TypeOf c Is PropertyCell Then
                            ' #Yes: add to list of selected cells
                            Me.m_lpropertySelected.Add(DirectCast(c, PropertyCell).GetProperty())
                        End If
                    End If
                Next

            End If

            If cmd IsNot Nothing Then
                If (TypeOf cmd Is cPropertySelectionCommand) Then
                    sc = DirectCast(cmd, cPropertySelectionCommand)
                    sc.Invoke(Me.m_lpropertySelected)
                End If
            End If

            Try
                Me.RaiseSelectionChangeEvent()
            Catch ex As Exception
                ' Woops
            End Try

        End Sub

        Public Function SelectedProperties() As cProperty()
            Return m_lpropertySelected.ToArray()
        End Function

        Public Function ReadContent(ByVal sr As StreamReader) As Boolean

            Dim strLine As String = ""
            Dim astrCells As String()
            Dim cell As ICell = Nothing
            Dim cellValue As Object = Nothing
            Dim iRow As Integer = 0
            Dim iCol As Integer = 0
            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Dim nfi As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            nfi.NumberDecimalSeparator = "."

            Try
                While Not sr.EndOfStream And iRow < Me.RowsCount
                    strLine = sr.ReadLine()
                    astrCells = strLine.Split(","c)
                    For iCol = 0 To Math.Min(Me.ColumnsCount, astrCells.Length) - 1
                        cell = Me(iRow, iCol)
                        If cell IsNot Nothing Then
                            If (cell.DataModel.EnableEdit = True) And _
                               (cell.DataModel.EditableMode <> SourceGrid2.EditableMode.None) Then
                                If (cell.DataModel.ValueType Is GetType(String)) Then
                                    cell.Value = astrCells(iCol)
                                Else
                                    astrCells(iCol) = astrCells(iCol).Trim()
                                    If String.IsNullOrEmpty(astrCells(iCol)) Then
                                        astrCells(iCol) = CStr(cCore.NULL_VALUE)
                                    End If
                                    If (cell.DataModel.ValueType Is GetType(Single)) Then
                                        ' Parse using UI default number formatting
                                        cell.Value = Single.Parse(astrCells(iCol), nfi)
                                    ElseIf (cell.DataModel.ValueType Is GetType(Double)) Then
                                        ' Parse using UI default number formatting
                                        cell.Value = Double.Parse(astrCells(iCol), nfi)
                                    ElseIf (cell.DataModel.ValueType Is GetType(Integer)) Then
                                        ' Parse using UI default number formatting
                                        cell.Value = Integer.Parse(astrCells(iCol), nfi)
                                    ElseIf (cell.DataModel.ValueType Is GetType(Boolean)) Then
                                        ' Parse using UI default number formatting
                                        cell.Value = Boolean.Parse(astrCells(iCol))
                                    End If
                                End If
                            End If
                        End If
                    Next
                    iRow += 1
                End While
            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

        Public Function WriteContent(ByVal sw As StreamWriter) As Boolean

            Dim cell As ICell = Nothing
            Dim cellValue As Object = Nothing
            Dim strValue As String = ""
            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Dim nfi As NumberFormatInfo = DirectCast(ci.NumberFormat.Clone(), NumberFormatInfo)

            nfi.NumberDecimalSeparator = "."

            Try
                For iRow As Integer = 0 To Me.RowsCount - 1
                    For iCol As Integer = 0 To Me.ColumnsCount - 1
                        cell = Me(iRow, iCol)

                        If (cell IsNot Nothing) Then
                            cellValue = cell.Value
                            If (cellValue IsNot Nothing) Then
                                If TypeOf (cellValue) Is String Then
                                    sw.Write(cell.DisplayText)
                                Else
                                    strValue = Convert.ToString(cell.GetValue(New SourceGrid2.Position(iRow, iCol)), nfi)
                                    sw.Write(strValue)
                                End If
                            End If
                        End If
                        sw.Write(",")
                    Next
                    sw.WriteLine()
                Next

            Catch ex As Exception
                Return False
            End Try

            Return True

        End Function

#End Region ' Selection behavior

    End Class

End Namespace
