#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2

#End Region

Namespace Ecospace

    <CLSCompliant(False)> _
    Public Class gridEditImportanceLayers
        Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first Layer</summary>
        Private Const iFIRSTDATAROW As Integer = 1

        ''' <summary>List of active Layers.</summary>
        Private m_alLayers As New List(Of LayerInfo)
        ''' <summary>List of removed Layers.</summary>
        Private m_alLayersRemoved As New List(Of LayerInfo)
        ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
        ''' to trap cell edit events locally in this grid. These events are essential
        ''' for keeping the local Layer administration up to date.</summary>
        Private m_bm As BehaviorModels.IBehaviorModel = New EndEditHandler(Me)
        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Visual model to display original Layers.</summary>
        Private m_vmOriginal As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display newly created Layers.</summary>
        Private m_vmAdded As VisualModels.Common = New VisualModels.Common(False)
        ''' <summary>Visual model to display Layers that are about be deleted.</summary>
        Private m_vmRemoved As VisualModels.Common = New VisualModels.Common(False)

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            LayerIndex = 0
            LayerName
            LayerWeight
            LayerDescription
            LayerStatus
        End Enum

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="cEcospaceBasemap">Importance layer</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new Layers. If this class has its
        ''' <see cref="LayerInfo.Layer">Layer</see> parameter set, a real live
        ''' Layer is represented. If this parameter is not set, a new Layer is
        ''' represented.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class LayerInfo

            ''' <summary><see cref="cEcospaceBasemap">cEcospaceBasemap</see> associated with this Layer, if any.</summary>
            Private m_Layer As cEcospaceLayerImportance = Nothing
            ''' <summary>Name for this Layer.</summary>
            Private m_strName As String = ""
            ''' <summary>Description for this Layer.</summary>
            Private m_strDescription As String = ""
            ''' <summary>Weight for this Layer.</summary>
            Private m_sWeight As Single = 0.0
            ''' <summary>Flag stating whether a user action is confirmed</summary>
            Private m_bConfirmed As Boolean = True
            ''' <summary>The status of a Layer in the interface.</summary>
            Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="Layer">The <see cref="cEcospaceBasemap">cEcospaceBasemap</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' Layer currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal Layer As cEcospaceLayerImportance)
                Debug.Assert(Layer IsNot Nothing)
                Me.m_Layer = Layer
                Me.m_strName = Layer.Name
                Me.m_strDescription = Layer.Description
                Me.m_sWeight = Layer.Weight
                Me.m_status = AddRemoveItemStatus.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single)
                Me.m_Layer = Nothing
                Me.m_strName = strName
                Me.m_strDescription = strDescription
                Me.m_sWeight = sWeight
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
            ''' Get/set the description of this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Description() As String
                Get
                    Return Me.m_strDescription
                End Get
                Set(ByVal value As String)
                    Me.m_strDescription = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set the weight of this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Weight() As Single
                Get
                    Return Me.m_sWeight
                End Get
                Set(ByVal value As Single)
                    Me.m_sWeight = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cEcospaceBasemap">EwE Layer</see> associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property Layer() As cEcospaceLayerImportance
                Get
                    Return Me.m_Layer
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="AddRemoveItemStatus">add/remove item status</see>
            ''' for the layer object.
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
            ''' States whether the Layer has changed.
            ''' </summary>
            ''' <returns>
            ''' True when Layer <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged() As Boolean
                If (Me.IsNew()) Then Return False
                Return (Me.m_Layer.Name <> Me.m_strName) Or _
                       (Me.Layer.Description <> Me.m_strDescription) Or _
                       (Me.Layer.Weight <> Me.m_sWeight)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the Layer is to be created.
            ''' </summary>
            ''' <returns>
            ''' True when Layer <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsNew() As Boolean
                Return (Me.m_Layer Is Nothing)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this layer is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = AddRemoveItemStatus.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Me.m_Layer IsNot Nothing Then
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
            Me.FixedColumnWidths = False

            ' Set up visual models for reflecting Layer modification status
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

            ' Layer index cell
            Me(0, eColumnTypes.LayerIndex) = New EwEColumnHeaderCell()
            ' Layer name cell, editable this time
            Me(0, eColumnTypes.LayerName) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
            Me(0, eColumnTypes.LayerWeight) = New EwEColumnHeaderCell(My.Resources.HEADER_WEIGHT)
            Me(0, eColumnTypes.LayerDescription) = New EwEColumnHeaderCell(My.Resources.HEADER_DESCRIPTION)

            ' Layer index cell
            Me(0, eColumnTypes.LayerStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

            ' Fix index column only; Layer name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the Layer/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            Dim Layer As cEcospaceLayerImportance = Nothing
            Dim li As LayerInfo = Nothing

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of Layer configuration
            For iLayer As Integer = 1 To Me.Core.nImportanceLayers
                Layer = Me.Core.EcospaceBasemap.LayerImportance(iLayer)
                li = New LayerInfo(Layer)
                Me.m_alLayers.Add(li)
            Next

            ' Brute-force update grid
            UpdateGrid()

        End Sub

        Protected Overrides Sub FinishStyle()
            MyBase.FinishStyle()

            Me.Columns(eColumnTypes.LayerIndex).Width = 40
            Me.Columns(eColumnTypes.LayerName).Width = 120
            Me.Columns(eColumnTypes.LayerWeight).Width = 60
            Me.Columns(eColumnTypes.LayerDescription).Width = 278

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Brute-force resize the gird if necessary, and repopulate with data from 
        ''' the local administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub UpdateGrid()

            Dim li As LayerInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_alLayers.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.LayerIndex) = ewec

                Me(iRow, eColumnTypes.LayerName) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.LayerName).Behaviors.Add(m_bm)

                Me(iRow, eColumnTypes.LayerDescription) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.LayerDescription).Behaviors.Add(m_bm)

                Me(iRow, eColumnTypes.LayerWeight) = New Cells.Real.Cell(0.0!, GetType(Single))
                Me(iRow, eColumnTypes.LayerWeight).Behaviors.Add(m_bm)

                ' Status
                vm = New VisualModels.Common()
                vm.ImageAlignment = ContentAlignment.MiddleCenter
                Me(iRow, eColumnTypes.LayerStatus) = New Cells.Real.Cell()
                Dim dm As New DataModels.DataModelBase(GetType(String))
                dm.EditableMode = EditableMode.None
                Me(iRow, eColumnTypes.LayerStatus).DataModel = dm
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_alLayers.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTDATAROW)
            End While

            ' Sanity check whether grid can accomodate all Layers + header
            Debug.Assert(Me.Rows.Count = Me.m_alLayers.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_alLayers.Count
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

            Dim li As LayerInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim strText As String = ""

            Me.AllowUpdates = False

            li = DirectCast(Me.m_alLayers(iRow - iFIRSTDATAROW), LayerInfo)
            ri = Me.Rows(iRow)

            ri.Tag = li
            aCells = ri.GetCells()

            pos = New Position(iRow, eColumnTypes.LayerIndex)
            aCells(eColumnTypes.LayerIndex).SetValue(pos, CInt(iRow))

            pos = New Position(iRow, eColumnTypes.LayerName)
            aCells(eColumnTypes.LayerName).SetValue(pos, CStr(li.Name))

            pos = New Position(iRow, eColumnTypes.LayerDescription)
            aCells(eColumnTypes.LayerDescription).SetValue(pos, CStr(li.Description))

            pos = New Position(iRow, eColumnTypes.LayerWeight)
            aCells(eColumnTypes.LayerWeight).SetValue(pos, CSng(li.Weight))

            Select Case li.Status
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

            pos = New Position(iRow, eColumnTypes.LayerStatus)
            aCells(eColumnTypes.LayerStatus).VisualModel = vm
            aCells(eColumnTypes.LayerStatus).SetValue(pos, strText)

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

            Dim li As LayerInfo = DirectCast(Me.m_alLayers(p.Row - 1), LayerInfo)

            Select Case DirectCast(p.Column, eColumnTypes)

                Case eColumnTypes.LayerName
                    ' JS: Handled in OnCellEdited()
                    ' li.Name = CStr(cell.GetValue(p))

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

            Dim li As LayerInfo = DirectCast(Me.m_alLayers(p.Row - 1), LayerInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.LayerIndex
                    ' Not possible

                Case eColumnTypes.LayerName
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iLayer As Integer = 0 To Me.m_alLayers.Count - 1
                        Dim giTemp As LayerInfo = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
                        ' Does name already exist?
                        If (Not Object.ReferenceEquals(giTemp, li)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    li.Name = strName

                Case eColumnTypes.LayerDescription
                    li.Description = CStr(cell.GetValue(p))

                Case eColumnTypes.LayerWeight
                    Dim sWeight As Single = CSng(cell.GetValue(p))
                    If sWeight < 0 Then Me.UpdateRow(p.Row) : Return False
                    li.Weight = sWeight

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

            Dim iLayer As Integer = iRow - iFIRSTDATAROW
            Dim li As LayerInfo = Nothing
            Dim strPrompt As String = ""

            ' Validate
            If iLayer < 0 Then Return

            li = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
            ' Toggle 'flagged for deletion' flag
            li.FlaggedForDeletion = Not li.FlaggedForDeletion

            ' Check to see what is to happen to the Layer now
            Select Case li.Status

                Case AddRemoveItemStatus.Original
                    ' Clear removed status of the Layer
                    Me.m_alLayersRemoved.Remove(Me.m_alLayers(iLayer))

                Case AddRemoveItemStatus.Added
                    ' Clear removed status of the Layer
                    Me.m_alLayersRemoved.Remove(Me.m_alLayers(iLayer))

                Case AddRemoveItemStatus.Removed
                    ' Set removed status
                    Me.m_alLayersRemoved.Add(Me.m_alLayers(iLayer))

                Case AddRemoveItemStatus.Invalid
                    ' Set removed status
                    Me.m_alLayers.RemoveAt(iLayer)

            End Select

            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a layer.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsLayerRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTDATAROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the layer on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsLayerRow(iRow) Then Return False

            Dim iLayer As Integer = iRow - iFIRSTDATAROW
            Dim li As LayerInfo = Nothing
            Dim strPrompt As String = ""

            li = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
            Return li.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new layer.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateLayer()
        End Sub

        ''' <summary>
        ''' Create a new layer.
        ''' </summary>
        Private Sub CreateLayer()
            Dim iRow As Integer = -1
            Dim iLayer As Integer = -1
            Dim li As LayerInfo = Nothing
            Dim lstrLayers As New List(Of String)

            ' Make fit
            iRow = Math.Max(iFIRSTDATAROW, Me.RowsCount)
            iLayer = iRow - iFIRSTDATAROW

            ' Validate
            If iLayer < 0 Then Return

            ' Collect all current layer names
            For Each li In Me.m_alLayers
                lstrLayers.Add(li.Name)
            Next

            ' Format new layer with an autonumber value based on existing names
            Dim iNextNum As Integer = cStringUtils.GetNextNumber(lstrLayers.ToArray(), My.Resources.DEFAULT_NEWLAYER_NUM)
            Dim strName As String = String.Format(My.Resources.DEFAULT_NEWLAYER_NUM, iNextNum)

            li = New LayerInfo(strName, "", 1.0!)
            Me.m_alLayers.Insert(iLayer, li)

            Me.UpdateGrid()
            Me.SelectRow(li)
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

        Private Sub SelectRow(ByVal li As LayerInfo)
            For iLayer As Integer = 0 To Me.m_alLayers.Count - 1
                If Object.ReferenceEquals(Me.m_alLayers(iLayer), li) Then
                    Me.SelectRow(iLayer + iFIRSTDATAROW)
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
        ''' Layer configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean
            Return True
        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bLayersChanged As Boolean = False
            Dim li As LayerInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim Layer As cEcospaceLayerImportance = Nothing
            Dim iLayer As Integer = 0
            Dim bSuccess As Boolean = True

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess Layer changes
            For iLayer = 0 To Me.m_alLayers.Count - 1
                li = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
                ' Check if this layer is newly added
                bConfigurationChanged = bConfigurationChanged Or li.IsNew()
                ' Check if this layer has been modified
                bLayersChanged = bLayersChanged Or li.IsChanged()
            Next iLayer

            If Me.m_alLayersRemoved.Count > 5 Then

                ' ToDo_JS: Globalize this
                strPrompt = String.Format(My.Resources.ECOSPACE_EDITLAYER_CONFIRMDELETENUM_PROMPT, Me.m_alLayersRemoved.Count)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.Yes
                        ' Confirm all regions
                        For Each li In Me.m_alLayersRemoved
                            li.Confirmed = True
                        Next
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            Else
                ' Assess Layers to remove
                For iLayer = 0 To Me.m_alLayersRemoved.Count - 1
                    li = DirectCast(Me.m_alLayersRemoved(iLayer), LayerInfo)
                    If (Not li.IsNew()) Then

                        strPrompt = String.Format(My.Resources.ECOSPACE_EDITLAYER_CONFIRMDELETE_PROMPT, li.Name)

                        Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                            Case MsgBoxResult.Cancel
                                ' Abort Apply process
                                Return False
                            Case MsgBoxResult.No
                                ' Do not delete this Layer
                                li.Confirmed = False
                            Case MsgBoxResult.Yes
                                ' Delete this Layer
                                li.Confirmed = True
                                bConfigurationChanged = True
                            Case Else
                                ' Unexpected anwer: assert
                                Debug.Assert(False)
                        End Select

                    End If
                Next iLayer
            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

                cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

                ' Add new Layers
                For iLayer = 0 To Me.m_alLayers.Count - 1
                    li = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
                    If (li.IsNew()) Then
                        bSuccess = bSuccess And Me.Core.AddEcospaceImportanceLayer(li.Name, li.Description, li.Weight, iDBID)
                    End If
                Next

                ' Remove deleted (and confirmed) Layers
                Dim iLayerRemove As Integer = 0
                For iLayer = 0 To Me.m_alLayersRemoved.Count - 1
                    li = DirectCast(Me.m_alLayersRemoved(iLayerRemove), LayerInfo)

                    ' Sanity check
                    Debug.Assert(Not li.IsNew())

                    If (li.Confirmed()) Then
                        If (Me.Core.RemoveEcospaceImportanceLayer(li.Layer)) Then
                            Me.m_alLayers.Remove(li)
                            Me.m_alLayersRemoved.Remove(li)
                        Else
                            bSuccess = False
                            iLayerRemove += 1
                        End If
                    End If
                Next

                ' The core will reload now
                Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)
                cApplicationStatusNotifier.SetStatusText("", TriState.False)

                ' Test whether new Layers were loaded correctly 
                Debug.Assert(Me.m_alLayers.Count = Me.Core.nImportanceLayers, ">> Internal panic: Dialog and core out of sync on Layers")
            End If

            ' Update core objects
            If (bLayersChanged) Then
                ' For each local layer admin unit
                For iLayer = 0 To Me.m_alLayers.Count - 1
                    ' Get local admin unit
                    li = DirectCast(Me.m_alLayers(iLayer), LayerInfo)
                    ' Has it changed?
                    If (li.IsChanged()) Then
                        ' Find core layer with same BDID (cannot use cached cEcospaceBasemap instances since the core has reloaded)
                        Dim bFound As Boolean = False
                        ' For every core layer instance (and yes, this array is one-based)
                        For iLayTest As Integer = 1 To Me.Core.nImportanceLayers
                            ' Get core layer instance
                            Dim layTest As cEcospaceLayerImportance = Me.Core.EcospaceBasemap.LayerImportance(iLayTest)
                            ' Has matching ID?
                            If (layTest.getID = li.Layer.getID) Then
                                ' #Yes: Update
                                layTest.Name = li.Name
                                layTest.Description = li.Description
                                layTest.Weight = li.Weight
                                ' Are we relieved or what!
                                bFound = True
                            End If
                        Next
                        ' All went well?
                        If Not bFound Then
                            ' #No?! Uh oh...
                            Debug.Assert(False, ">> Internal panic: Unable to apply changes to layer id " & li.Layer.getID)
                        End If
                    End If
                Next
            End If

            Return bSuccess

        End Function

#End Region ' Apply changes

    End Class

End Namespace


