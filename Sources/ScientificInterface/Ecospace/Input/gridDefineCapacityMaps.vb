#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Core

#End Region

Namespace Ecospace

    ''' <summary>
    ''' Grid catered to defining <see cref="IEnviroInputMap">environmental input maps</see>.
    ''' </summary>
    <CLSCompliant(False)> _
    Public Class gridDefineCapacityMaps
        : Inherits EwEGrid

        ''' <summary>A number representing the row that contains the first Map</summary>
        Private Const iFIRSTMAPROW As Integer = 1

        Private m_manager As cMapResponseInteractionManager = Nothing
        Private m_editorVariable As EwEComboBoxCellEditor = Nothing

        ''' <summary>List of active Maps.</summary>
        Private m_alMaps As New List(Of cMapInfo)
        ''' <summary>List of removed Maps.</summary>
        Private m_alMapsRemoved As New List(Of cMapInfo)

        ''' <summary>Update lock, used to distinguish between code updates and
        ''' user updates of grid cells. When grid cells are updated from within
        ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
        Private m_iUpdateLock As Integer = 0

        ''' <summary>Enumerated type defining the columns in this grid.</summary>
        Private Enum eColumnTypes
            MapIndex = 0
            MapName
            MapTargetVariable
            MapStatus
        End Enum

        Private m_aSupportedVars() As eVarNameFlags = New eVarNameFlags() {eVarNameFlags.LayerDepth, _
                                                                           eVarNameFlags.LayerRelPP, _
                                                                           eVarNameFlags.LayerRelCin}

#Region " Helper classes "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Administrative unit representing a <see cref="IEnviroInputMap">Map</see>
        ''' in the EwE model.
        ''' </summary>
        ''' <remarks>
        ''' This class can represent existing and new Maps.
        ''' </remarks>
        ''' -----------------------------------------------------------------------
        Private Class cMapInfo

            ''' <summary>DBID of associated map.</summary>
            Private m_iMapDBID As Integer = cCore.NULL_VALUE
            Private m_iMapIndex As Integer = cCore.NULL_VALUE
            Private m_varname As eVarNameFlags = eVarNameFlags.LayerDepth
            ''' <summary>Name for this Map.</summary>
            Private m_strName As String = ""
            ''' <summary>Flag stating whether a user action is confirmed</summary>
            Private m_bConfirmed As Boolean = True
            ''' <summary>The status of a Map in the interface.</summary>
            Private m_status As eItemStatusTypes = eItemStatusTypes.Original

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="Map">The <see cref="IEnviroInputMap">IEnviroInputMap</see> to
            ''' initialize this instance from. If set, this instance represents a
            ''' Map currently active in the EwE model.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal map As IEnviroInputMap)
                Debug.Assert(map IsNot Nothing)
                Dim cio As cCoreInputOutputBase = DirectCast(map, cCoreInputOutputBase)
                Me.m_iMapDBID = cio.DBID
                Me.m_iMapIndex = cio.Index
                Me.m_strName = map.Name
                Me.m_varname = map.Variable
                Me.m_status = eItemStatusTypes.Original
            End Sub

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instanze of this class.
            ''' </summary>
            ''' <param name="strName">Name to assign to this administrative unit.</param>
            ''' -------------------------------------------------------------------
            Public Sub New(ByVal strName As String)
                Me.m_strName = strName
                Me.m_status = eItemStatusTypes.Added
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
            ''' Get the <see cref="IEnviroInputMap.Variable">variable</see> of the 
            ''' map associated with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property Variable() As eVarNameFlags
                Get
                    Return Me.m_varname
                End Get
                Set(ByVal value As eVarNameFlags)
                    Me.m_varname = value
                End Set
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cCoreInputOutputBase.DBID"/> of the map associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property MapDBID() As Integer
                Get
                    Return Me.m_iMapDBID
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="cCoreInputOutputBase.Index"/> of the map associated
            ''' with this administrative unit.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public ReadOnly Property MapIndex() As Integer
                Get
                    Return Me.m_iMapIndex
                End Get
            End Property

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get the <see cref="eItemStatusTypes">item status</see>
            ''' for the map object.
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
            ''' States whether the Map has changed.
            ''' </summary>
            ''' <returns>
            ''' True when Map <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsChanged(ByVal map As IEnviroInputMap) As Boolean
                If (Me.IsNew()) Then Return False

                Dim cio As cCoreInputOutputBase = DirectCast(map, cCoreInputOutputBase)

                If (cio.DBID <> Me.m_iMapDBID) Then Return True
                If (cio.Name <> Me.m_strName) Then Return True
                If (map.Variable <> Me.m_varname) Then Return True
                Return False

            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' States whether the Map is to be created.
            ''' </summary>
            ''' <returns>
            ''' True when Map <see cref="Name">Name</see> value has changed.
            ''' </returns>
            ''' -------------------------------------------------------------------
            Public Function IsNew() As Boolean
                Return (Me.m_iMapDBID = cCore.NULL_VALUE)
            End Function

            ''' -------------------------------------------------------------------
            ''' <summary>
            ''' Get/set whether this map is flagged for deletion. Toggling this flag
            ''' will update the <see cref="Status">Status</see> of the item.
            ''' </summary>
            ''' -------------------------------------------------------------------
            Public Property FlaggedForDeletion() As Boolean
                Get
                    Return Me.m_status = eItemStatusTypes.Removed
                End Get
                Set(ByVal bDelete As Boolean)
                    If Not Me.IsNew() Then
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

        End Class

#End Region ' Helper classes

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create the grid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()

            MyBase.New()

            ' Prepare editor
            Me.m_editorVariable = New EwEComboBoxCellEditor(New cVarnameTypeFormatter())
            Me.m_editorVariable.StandardValues = Me.m_aSupportedVars

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

            Me(0, eColumnTypes.MapIndex) = New EwEColumnHeaderCell()
            Me(0, eColumnTypes.MapName) = New EwEColumnHeaderCell(SharedResources.HEADER_NAME)
            Me(0, eColumnTypes.MapTargetVariable) = New EwEColumnHeaderCell(SharedResources.HEADER_TARGET)
            Me(0, eColumnTypes.MapStatus) = New EwEColumnHeaderCell(SharedResources.HEADER_STATUS)

            ' Fix index column only; Map name column cannot be fixed because it must be editable
            Me.FixedColumns = 1

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to first create a snapshot of the Map/stanza configuration
        ''' in the current EwE model. The grid will be populated from this local
        ''' administration.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub FillData()

            ' Get the core reference
            Dim map As IEnviroInputMap = Nothing
            Dim info As cMapInfo = Nothing

            Me.m_manager = Me.Core.CapacitMapInteractionManager

            ' Populate local administration from a snapshot of the live data

            ' Make snapshot of Map configuration
            For iMap As Integer = 1 To Me.Core.nCapacityMaps
                map = Me.m_manager.Map(iMap)
                info = New cMapInfo(map)
                Me.m_alMaps.Add(info)
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

            Dim info As cMapInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim cells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.Common = Nothing
            Dim ewec As EwECell = Nothing

            ' Create missing rows
            For iRow As Integer = Me.Rows.Count To Me.m_alMaps.Count
                Me.AddRow()

                ewec = New EwECell(0, GetType(Integer))
                ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
                Me(iRow, eColumnTypes.MapIndex) = ewec

                Me(iRow, eColumnTypes.MapName) = New Cells.Real.Cell("", GetType(String))
                Me(iRow, eColumnTypes.MapName).Behaviors.Add(Me.EwEEditHandler)

                ' ToDo: use proper formatting here
                Me(iRow, eColumnTypes.MapTargetVariable) = New SourceGrid2.Cells.Real.Cell(eVarNameFlags.NotSet, Me.m_editorVariable)
                Me(iRow, eColumnTypes.MapTargetVariable).Behaviors.Add(Me.EwEEditHandler)

                ' Status
                vm = New VisualModels.Common()
                vm.ImageAlignment = ContentAlignment.MiddleCenter
                Me(iRow, eColumnTypes.MapStatus) = New Cells.Real.Cell()
                Dim dm As New DataModels.DataModelBase(GetType(String))
                dm.EditableMode = EditableMode.None
                Me(iRow, eColumnTypes.MapStatus).DataModel = dm
            Next

            ' Delete obsolete rows
            While Me.Rows.Count > Me.m_alMaps.Count + 1
                Me.Rows.Remove(Me.Rows.Count - iFIRSTMAPROW)
            End While

            ' Sanity check whether grid can accomodate all Maps + header
            Debug.Assert(Me.Rows.Count = Me.m_alMaps.Count + 1)

            ' Populate rows
            For iRow As Integer = 1 To Me.m_alMaps.Count
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

            Dim info As cMapInfo = Nothing
            Dim ri As RowInfo = Nothing
            Dim aCells() As Cells.ICellVirtual = Nothing
            Dim pos As SourceGrid2.Position = Nothing
            Dim vm As VisualModels.IVisualModel = Nothing
            Dim strText As String = ""

            Me.AllowUpdates = False

            info = DirectCast(Me.m_alMaps(iRow - iFIRSTMAPROW), cMapInfo)
            ri = Me.Rows(iRow)

            ri.Tag = info
            aCells = ri.GetCells()

            pos = New Position(iRow, eColumnTypes.MapIndex)
            aCells(eColumnTypes.MapIndex).SetValue(pos, CInt(iRow))

            pos = New Position(iRow, eColumnTypes.MapName)
            aCells(eColumnTypes.MapName).SetValue(pos, CStr(info.Name))

            pos = New Position(iRow, eColumnTypes.MapTargetVariable)
            aCells(eColumnTypes.MapTargetVariable).SetValue(pos, info.Variable)

            Select Case info.Status
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

            pos = New Position(iRow, eColumnTypes.MapStatus)
            aCells(eColumnTypes.MapStatus).VisualModel = vm
            aCells(eColumnTypes.MapStatus).SetValue(pos, strText)

            Me.AllowUpdates = True

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

            If Not Me.AllowUpdates Then Return True

            Dim info As cMapInfo = DirectCast(Me.m_alMaps(p.Row - 1), cMapInfo)

            Select Case DirectCast(p.Column, eColumnTypes)
                Case eColumnTypes.MapIndex
                    ' Not possible

                Case eColumnTypes.MapName
                    Dim strName As String = CStr(cell.GetValue(p))
                    ' Check if name is unique
                    For iMap As Integer = 0 To Me.m_alMaps.Count - 1
                        Dim giTemp As cMapInfo = DirectCast(Me.m_alMaps(iMap), cMapInfo)
                        ' Does name already exist?
                        If (Not Object.ReferenceEquals(giTemp, info)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                            ' Change is not allowed
                            Me.UpdateRow(p.Row)
                            ' Report failure
                            Return False
                        End If
                    Next
                    ' Allow name change
                    info.Name = strName

                Case eColumnTypes.MapTargetVariable
                    Dim var As eVarNameFlags = DirectCast(cell.GetValue(p), eVarNameFlags)
                    info.Variable = var

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

            Dim iMap As Integer = iRow - iFIRSTMAPROW
            Dim info As cMapInfo = Nothing
            Dim strPrompt As String = ""

            ' Validate
            If iMap < 0 Then Return

            info = DirectCast(Me.m_alMaps(iMap), cMapInfo)
            ' Toggle 'flagged for deletion' flag
            info.FlaggedForDeletion = Not info.FlaggedForDeletion

            ' Check to see what is to happen to the Map now
            Select Case info.Status

                Case eItemStatusTypes.Original
                    ' Clear removed status of the Map
                    Me.m_alMapsRemoved.Remove(Me.m_alMaps(iMap))

                Case eItemStatusTypes.Added
                    ' Clear removed status of the Map
                    Me.m_alMapsRemoved.Remove(Me.m_alMaps(iMap))

                Case eItemStatusTypes.Removed
                    ' Set removed status
                    Me.m_alMapsRemoved.Add(Me.m_alMaps(iMap))

                Case eItemStatusTypes.Invalid
                    ' Set removed status
                    Me.m_alMaps.RemoveAt(iMap)

            End Select

            Me.UpdateGrid()

        End Sub

        ''' <summary>
        ''' States whether a row holds a map.
        ''' </summary>
        ''' <param name="iRow"></param>
        ''' <returns></returns>
        Public Function IsMapRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            Return (iRow >= iFIRSTMAPROW) And (iRow < Me.RowsCount)
        End Function

        ''' <summary>
        ''' States whether the map on a row is flagged for deletion.
        ''' </summary>
        Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
            If iRow = -1 Then iRow = Me.SelectedRow()
            If Not IsMapRow(iRow) Then Return False

            Dim iMap As Integer = iRow - iFIRSTMAPROW
            Dim info As cMapInfo = Nothing
            Dim strPrompt As String = ""

            info = DirectCast(Me.m_alMaps(iMap), cMapInfo)
            Return info.FlaggedForDeletion
        End Function

        ''' <summary>
        ''' Add a row by creating a new map.
        ''' </summary>
        Public Sub InsertRow()
            If Not Me.CanAddRow() Then Return
            Me.CreateMap()
        End Sub

        ''' <summary>
        ''' Create a new map.
        ''' </summary>
        Private Sub CreateMap()
            Dim iRow As Integer = -1
            Dim iMap As Integer = -1
            Dim info As cMapInfo = Nothing
            Dim lstrMaps As New List(Of String)

            ' Make fit
            iRow = Math.Max(iFIRSTMAPROW, Me.RowsCount)
            iMap = iRow - iFIRSTMAPROW

            ' Validate
            If iMap < 0 Then Return

            ' Collect all current map names
            For Each info In Me.m_alMaps
                lstrMaps.Add(info.Name)
            Next

            ' Format new map with an autonumber value based on existing names
            info = New cMapInfo(String.Format(SharedResources.DEFAULT_NEWMAP_NUM, _
                    cStringUtils.GetNextNumber(lstrMaps.ToArray(), SharedResources.DEFAULT_NEWMAP_NUM)))
            Me.m_alMaps.Insert(iMap, info)

            Me.UpdateGrid()
            Me.SelectRow(info)
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

        Private Overloads Sub SelectRow(ByVal info As cMapInfo)
            For iMap As Integer = 0 To Me.m_alMaps.Count - 1
                If Object.ReferenceEquals(Me.m_alMaps(iMap), info) Then
                    Me.SelectRow(iMap + iFIRSTMAPROW)
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
        ''' Map configuration for a model.</returns>
        ''' -----------------------------------------------------------------------
        Public Function ValidateContent() As Boolean

            Return Me.ValidateNames

        End Function

        Private Function ValidateNames() As Boolean

            Dim fmsg As New cFeedbackMessage(My.Resources.PROMPT_DUPLICATE_NAMES, eCoreComponentType.External, eMessageType.DataValidation, eMessageImportance.Question, cFeedbackMessage.eReplyStyle.YES_NO, eDataTypes.NotSet, cFeedbackMessage.eReply.NO)
            Dim bHasDuplicates As Boolean = False
            Dim bHasBlank As Boolean = False
            Dim lstrHandled As New List(Of String)

            For Each info As cMapInfo In Me.m_alMaps
                If String.IsNullOrEmpty(info.Name) Then
                    bHasBlank = True
                ElseIf Not Me.IsNameUnique(info.Name, info) Then
                    If Not lstrHandled.Contains(info.Name) Then
                        fmsg.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation, _
                                                             String.Format(My.Resources.PROMPT_DUPLICATE_NAME, info.Name), _
                                                             eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, cCore.NULL_VALUE))
                        lstrHandled.Add(info.Name)
                    End If
                    bHasDuplicates = True
                End If
            Next

            If bHasBlank Then
                Me.Core.Messages.SendMessage(New cMessage(My.Resources.PROMPT_EMPTY_NAMES, eMessageType.DataValidation, eCoreComponentType.External, eMessageImportance.Warning))
                Return False
            End If

            If bHasDuplicates Then
                Me.Core.Messages.SendMessage(fmsg)
                Return fmsg.Reply = cFeedbackMessage.eReply.YES
            End If

            Return True

        End Function

        Private Function IsNameUnique(ByVal strName As String, ByVal info As cMapInfo) As Boolean

            ' Check if name is unique
            For i As Integer = 0 To Me.m_alMaps.Count - 1
                Dim infoTmp As cMapInfo = DirectCast(Me.m_alMaps(i), cMapInfo)
                ' Does name already exist?
                If (Not Object.ReferenceEquals(infoTmp, info)) And (String.Compare(strName, infoTmp.Name, True) = 0) Then
                    ' Report failure
                    Return False
                End If
            Next
            Return True

        End Function

#End Region ' Validation

#Region " Apply changes "

        Public Function Apply() As Boolean

            Dim strPrompt As String = ""
            Dim bConfigurationChanged As Boolean = False
            Dim bMapsChanged As Boolean = False
            Dim info As cMapInfo = Nothing
            Dim iDBID As Integer = Nothing
            Dim map As IEnviroInputMap = Nothing
            Dim iMap As Integer = 0
            Dim bSuccess As Boolean = True

            ' Validate content of the grid
            If Not Me.ValidateContent() Then Return False

            ' Assess Map changes
            For iMap = 0 To Me.m_alMaps.Count - 1
                info = DirectCast(Me.m_alMaps(iMap), cMapInfo)

                If info.IsNew Then
                    bConfigurationChanged = True
                Else
                    ' Check if this map has been modified
                    bMapsChanged = bMapsChanged Or info.IsChanged(Me.m_manager.Map(info.MapIndex))
                End If
            Next iMap

            If Me.m_alMapsRemoved.Count > 5 Then

                strPrompt = String.Format(My.Resources.ECOSPACE_EDITHABITAT_CONFIRMDELETENUM_PROMPT, Me.m_alMapsRemoved.Count)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.Yes
                        ' Confirm all regions
                        For Each info In Me.m_alMapsRemoved
                            info.Confirmed = True
                        Next
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select

            Else
                ' Assess Maps to remove
                For iMap = 0 To Me.m_alMapsRemoved.Count - 1
                    info = DirectCast(Me.m_alMapsRemoved(iMap), cMapInfo)
                    If (Not info.IsNew()) Then

                        strPrompt = String.Format(My.Resources.ECOSPACE_EDITHABITAT_CONFIRMDELETE_PROMPT, info.Name)

                        Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                            Case MsgBoxResult.Cancel
                                ' Abort Apply process
                                Return False
                            Case MsgBoxResult.No
                                ' Do not delete this Map
                                info.Confirmed = False
                            Case MsgBoxResult.Yes
                                ' Delete this Map
                                info.Confirmed = True
                                bConfigurationChanged = True
                            Case Else
                                ' Unexpected anwer: assert
                                Debug.Assert(False)
                        End Select

                    End If
                Next iMap
            End If

            ' Handle added and removed items
            If (bConfigurationChanged) Then

                If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False

                cApplicationStatusNotifier.StartProgress(Me.Core, My.Resources.GENERIC_STATUS_APPLYCHANGES)

                ' Add new Maps
                For iMap = 0 To Me.m_alMaps.Count - 1
                    info = DirectCast(Me.m_alMaps(iMap), cMapInfo)
                    If (info.IsNew()) Then
                        bSuccess = bSuccess And Me.m_manager.AddMap(info.Name, info.Variable)
                    End If
                Next

                ' Remove deleted (and confirmed) Maps
                Dim iMapRemove As Integer = 0
                For iMap = 0 To Me.m_alMapsRemoved.Count - 1
                    info = DirectCast(Me.m_alMapsRemoved(iMapRemove), cMapInfo)

                    ' Sanity check
                    Debug.Assert(Not info.IsNew())

                    If (info.Confirmed()) Then
                        If (Me.m_manager.RemoveMap(info.MapDBID)) Then
                            Me.m_alMaps.Remove(info)
                            Me.m_alMapsRemoved.Remove(info)
                        Else
                            bSuccess = False
                            iMapRemove += 1
                        End If
                    End If
                Next

                ' The core will reload now
                Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecospace)
                cApplicationStatusNotifier.EndProgress(Me.Core)

                ' Test whether new Maps were loaded correctly 
                ' !! taking into account that this dialog does NOT contain the All map, hence the '-1'
                Debug.Assert(Me.m_alMaps.Count = Me.Core.nCapacityMaps, ">> Internal panic: Dialog and core out of sync on Maps")
            End If

            ' Update core objects
            If (bMapsChanged) Then

                ' Build quick map lookup
                Dim dtMaps As New Dictionary(Of Integer, IEnviroInputMap)
                For iMap = 1 To Me.m_manager.nMaps
                    map = Me.m_manager.Map(iMap)
                    dtMaps(DirectCast(map, cCoreInputOutputBase).DBID) = map
                Next

                ' For each local map admin unit
                For iMap = 0 To Me.m_alMaps.Count - 1
                    ' Get local admin unit
                    info = DirectCast(Me.m_alMaps(iMap), cMapInfo)
                    If Not info.IsNew() Then
                        map = dtMaps(info.MapDBID)
                        ' Has it changed?
                        If (info.IsChanged(map)) Then
                            ' #Yes: Update
                            map.Name = info.Name
                            map.Variable = info.Variable
                        End If
                    End If
                Next
            End If

            Return bSuccess

        End Function

#End Region ' Apply changes

    End Class

End Namespace


