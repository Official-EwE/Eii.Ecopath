#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports ScientificInterface.Other
Imports EwEUtils.Core
Imports SourceGrid2.Cells

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' Grid class for the Edit Pedigree Levels interface.
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
   Public Class gridEditPedigree
    : Inherits EwEGrid

#Region " Private vars "

    ''' <summary>A number representing the row that contains the first Level</summary>
    Private Const iFIRSTDATAROW As Integer = 1

    Private m_dictConfigs As New Dictionary(Of eVarNameFlags, cPedigreeManagerInfo)
    Private m_vnActive As eVarNameFlags = eVarNameFlags.NotSet

    ''' <summary>Update lock, used to distinguish between code updates and
    ''' user updates of grid cells. When grid cells are updated from within
    ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
    Private m_iUpdateLock As Integer = 0

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes
        LevelIndex = 0
        LevelName
        LevelIndexValue
        LevelConfidenceInterval
        LevelStatus
    End Enum

#End Region ' Private vars

#Region " Helper classes "

    Private Class cPedigreeInfoListSorter
        Implements IComparer(Of cPedigreeLevelInfo)

        Public Function Compare(ByVal x As cPedigreeLevelInfo, ByVal y As cPedigreeLevelInfo) As Integer _
            Implements IComparer(Of cPedigreeLevelInfo).Compare
            If x.IndexValue < y.IndexValue Then Return -1
            If x.IndexValue > y.IndexValue Then Return 1
            If x.ConfidenceInterval < y.ConfidenceInterval Then Return -1
            If x.ConfidenceInterval > y.ConfidenceInterval Then Return 1
            Return 0
        End Function

    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class wrapping a single <see cref="cPedigreeManager">pedigree manager</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Class cPedigreeManagerInfo

        ''' <summary>List of active levels.</summary>
        Private m_lfiLevels As New List(Of cPedigreeLevelInfo)
        ''' <summary>List of removed Levels.</summary>
        Private m_lfiLevelsRemoved As New List(Of cPedigreeLevelInfo)

        Private m_vn As eVarNameFlags = eVarNameFlags.NotSet
        Private m_core As cCore = Nothing
        Private m_man As cPedigreeManager = Nothing
        Private m_bConfigChanged As Boolean = False
        Private m_bLevelsChanged As Boolean = False

        Public Sub New(ByVal core As cCore, ByVal vn As eVarNameFlags)

            Me.m_core = core
            Me.m_vn = vn
            Me.m_man = Me.m_core.GetPedigreeManager(Me.m_vn)

            For iLevel As Integer = 0 To Me.m_man.NumLevels - 1
                Me.m_lfiLevels.Add(New cPedigreeLevelInfo(Me.m_man.Level(iLevel)))
            Next

        End Sub

        Public Function AssessChanges() As Boolean

            Dim iLevel As Integer = 0
            Dim fi As cPedigreeLevelInfo = Nothing
            Dim strPrompt As String = ""

            Me.m_bConfigChanged = False
            Me.m_bLevelsChanged = False

            ' Assess Level changes
            For iLevel = 0 To Me.m_lfiLevels.Count - 1
                fi = DirectCast(Me.m_lfiLevels(iLevel), cPedigreeLevelInfo)
                ' Check this Level is newly added
                If Object.ReferenceEquals(fi.Level, Nothing) Then
                    Me.m_bConfigChanged = True
                End If
                ' Check if this Level is an existing Level that has been moved
                If Not Object.ReferenceEquals(fi.Level, Nothing) Then
                    If ((iLevel + 1) <> fi.Level.Index) Then
                        Me.m_bConfigChanged = True
                    End If
                End If
                Me.m_bLevelsChanged = Me.m_bLevelsChanged Or fi.IsChanged()
            Next iLevel

            ' Assess Levels to remove
            strPrompt = ""
            For iLevel = 0 To Me.m_lfiLevelsRemoved.Count - 1
                fi = DirectCast(Me.m_lfiLevelsRemoved(iLevel), cPedigreeLevelInfo)
                If (Not Object.ReferenceEquals(fi.Level, Nothing)) Then

                    strPrompt = String.Format("?", fi.Name)

                    Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                        Case MsgBoxResult.Cancel
                            ' Abort Apply process
                            Return False
                        Case MsgBoxResult.No
                            ' Do not delete this Level
                            fi.Confirmed = False
                        Case MsgBoxResult.Yes
                            ' Delete this Level
                            fi.Confirmed = True
                            Me.m_bConfigChanged = True
                        Case Else
                            ' Unexpected answer: assert
                            Debug.Assert(False)
                    End Select

                End If
            Next iLevel
            Return True

        End Function

        Public ReadOnly Property LevelsChanged() As Boolean
            Get
                Return Me.m_bLevelsChanged
            End Get
        End Property

        Public ReadOnly Property ConfigChanged() As Boolean
            Get
                Return Me.m_bConfigChanged
            End Get
        End Property

        Public ReadOnly Property Levels() As List(Of cPedigreeLevelInfo)
            Get
                Return Me.m_lfiLevels
            End Get
        End Property

        Public ReadOnly Property LevelsRemoved() As List(Of cPedigreeLevelInfo)
            Get
                Return Me.m_lfiLevelsRemoved
            End Get
        End Property

        Public Sub Sort()
            Me.m_lfiLevels.Sort(New cPedigreeInfoListSorter)
        End Sub

        Public Function ApplyConfigChanges() As Boolean

            Dim fi As cPedigreeLevelInfo = Nothing
            Dim Level As cPedigreeLevel = Nothing
            Dim iLevel As Integer = 0
            Dim bSuccess As Boolean = True

            ' Handle added and removed items
            If (Me.m_bConfigChanged) Then

                Dim iDBID As Integer = Nothing

                ' Add new Levels
                For iLevel = 0 To Me.m_lfiLevels.Count - 1

                    fi = DirectCast(Me.m_lfiLevels(iLevel), cPedigreeLevelInfo)
                    If (Object.ReferenceEquals(fi.Level, Nothing)) Then
                        Dim igt As Integer = iLevel + 1
                        bSuccess = bSuccess And Me.m_man.AddLevel(fi.Name, igt, Me.m_vn, fi.IndexValue, fi.ConfidenceInterval, iDBID)
                    Else
                        If ((iLevel + 1) <> fi.Level.Index) Then
                            'bSuccess = bSuccess And Me.m_man.MoveLevel(fi.Level.Index, iLevel + 1)
                        End If
                    End If
                Next

                ' Remove deleted (and confirmed) Levels
                Dim iLevelRemove As Integer = 0
                For iLevel = 0 To Me.m_lfiLevelsRemoved.Count - 1
                    fi = DirectCast(Me.m_lfiLevelsRemoved(iLevelRemove), cPedigreeLevelInfo)
                    If (Not Object.ReferenceEquals(fi.Level, Nothing)) And (fi.Confirmed = True) Then
                        If (Me.m_man.RemoveLevel(fi.Level)) Then
                            Me.m_lfiLevels.Remove(fi)
                            Me.m_lfiLevelsRemoved.Remove(fi)
                        Else
                            bSuccess = False
                            iLevelRemove += 1
                        End If
                    End If
                Next

                '' Test whether new Levels were loaded correctly
                'Debug.Assert(Me.m_lfiLevels.Count = Me.m_man.NumLevels, "Dialog and core out of sync on Levels")
            End If
            Return bSuccess

        End Function

        Public Function ApplyLevelChanges() As Boolean

            Dim level As cPedigreeLevel = Nothing
            Dim bUpdated As Boolean = False
            Dim bSuccess As Boolean = True

            ' Levels may have been reloaded
            If (Me.m_bLevelsChanged) Then
                For Each pi As cPedigreeLevelInfo In Me.m_lfiLevels
                    If pi.IsChanged() Then
                        bUpdated = False
                        ' Find (possibly reloaded) level that matches this pi
                        For iLevel As Integer = 0 To Me.m_man.NumLevels - 1
                            level = Me.m_man.Level(iLevel)
                            If level.DBID = pi.Level.DBID Then
                                level.Name = pi.Name
                                level.IndexValue = pi.IndexValue
                                level.ConfidenceInterval = pi.ConfidenceInterval
                                bUpdated = True
                            End If
                        Next
                        bSuccess = bSuccess And bUpdated
                    End If
                Next
            End If

            Me.m_man.Update()
            Return bSuccess

        End Function
    End Class

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Administrative unit representing a <see cref="cPedigreeLevel">Level</see>
    ''' in the EwE model.
    ''' </summary>
    ''' <remarks>
    ''' This class can represent existing and new Levels. If this class has its
    ''' <see cref="cPedigreeLevelInfo.Level">Level</see> parameter set, a real live
    ''' Level is represented. If this parameter is not set, a new Level is
    ''' represented.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Protected Class cPedigreeLevelInfo

        ''' <summary><see cref="cPedigreeLevel">cPedigreeLevel</see> associated with this info, if any.</summary>
        Private m_level As cPedigreeLevel = Nothing
        ''' <summary>Name for this level.</summary>
        Private m_strName As String = ""
        Private m_sIndex As Single = 0.0!
        Private m_sConfidenceInterval As Single = 0.0!
        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a Level in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="level">The <see cref="cPedigreeLevel">cPedigreeLevel</see> to
        ''' initialize this instance from. If set, this instance represents a
        ''' Level currently active in the EwE model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal level As cPedigreeLevel)
            Debug.Assert(level IsNot Nothing)
            Me.m_level = level
            Me.m_strName = level.Name
            Me.m_sIndex = level.IndexValue
            Me.m_sConfidenceInterval = level.ConfidenceInterval
            Me.m_status = eItemStatusTypes.Original
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String)
            Me.m_level = Nothing
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
        ''' Get the <see cref="cPedigreeLevel">EwE Level</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Level() As cPedigreeLevel
            Get
                Return Me.m_level
            End Get
        End Property

        Public Property IndexValue() As Single
            Get
                Return Me.m_sIndex
            End Get
            Set(ByVal value As Single)
                Me.m_sIndex = value
            End Set
        End Property

        Public Property ConfidenceInterval() As Single
            Get
                Return Me.m_sConfidenceInterval
            End Get
            Set(ByVal value As Single)
                Me.m_sConfidenceInterval = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eItemStatusTypes">item status</see> for the Level 
        ''' object.
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
        ''' States whether the underlying <see cref="cPedigreeLevel">pedigree level</see>
        ''' has been changed.
        ''' </summary>
        ''' <returns>
        ''' True if the underlying <see cref="cPedigreeLevel">pedigree level</see> 
        ''' has been changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged() As Boolean
            If Me.m_level Is Nothing Then Return False
            Return (Me.m_level.Name <> Me.m_strName) Or _
                   (Me.m_level.IndexValue <> Me.m_sIndex) Or _
                   (Me.m_level.ConfidenceInterval <> Me.m_sConfidenceInterval)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this Level is flagged for deletion. Toggling this flag
        ''' will update the <see cref="Status">Status</see> of the item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = eItemStatusTypes.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Me.m_level IsNot Nothing Then
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

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the varname to show in the grid.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property VarName() As eVarNameFlags
        Get
            Return Me.m_vnActive
        End Get
        Set(ByVal value As eVarNameFlags)
            If (value <> Me.m_vnActive) Then
                Me.m_vnActive = value
                Me.RefreshContent()
            End If
        End Set
    End Property

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

        ' Level index cell
        Me(0, eColumnTypes.LevelIndex) = New EwEColumnHeaderCell()
        ' Level name cell, editable this time
        Me(0, eColumnTypes.LevelName) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
        ' Index value
        Me(0, eColumnTypes.LevelIndexValue) = New EwEColumnHeaderCell("Index value")
        ' Confidence interval
        Me(0, eColumnTypes.LevelConfidenceInterval) = New EwEColumnHeaderCell("Conf. interv. (+/-%)")
        ' Status
        Me(0, eColumnTypes.LevelStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)
        ' Fix index column only; Level name column cannot be fixed because it must be editable
        Me.FixedColumns = 1

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to first create a snapshot of the Level/stanza configuration
    ''' in the current EwE model. The grid will be populated from this local
    ''' administration.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Protected Overrides Sub FillData()

        If (Me.m_dictConfigs.Count = 0) Then
            For Each vn As eVarNameFlags In cPedigreeManager.SupportVariables
                Me.m_dictConfigs(vn) = New cPedigreeManagerInfo(Me.Core, vn)
            Next
        End If

        If Me.m_vnActive = eVarNameFlags.NotSet Then Return

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

        Dim fi As cPedigreeLevelInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim ewec As EwECell = Nothing

        ' Create missing rows
        For iRow As Integer = Me.Rows.Count To Me.ActiveConfig.Levels.Count
            Me.AddRow()

            ewec = New EwECell(iRow, GetType(Integer))
            ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.LevelIndex) = ewec

            ewec = New EwECell("", GetType(String))
            ewec.Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.LevelName) = ewec

            ewec = New EwECell(1.0!, GetType(Single))
            ewec.NumDigits = 2
            ewec.Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.LevelIndexValue) = ewec

            ewec = New EwECell(75, GetType(Integer))
            ewec.Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.LevelConfidenceInterval) = ewec

            ' Status
            vm = New VisualModels.Common()
            vm.ImageAlignment = ContentAlignment.MiddleCenter
            Me(iRow, eColumnTypes.LevelStatus) = New Cells.Real.Cell()
            Dim dm As New DataModels.DataModelBase(GetType(String))
            dm.EditableMode = EditableMode.None
            Me(iRow, eColumnTypes.LevelStatus).DataModel = dm
        Next

        ' Delete obsolete rows
        While Me.Rows.Count > Me.ActiveConfig.Levels.Count + 1
            Me.Rows.Remove(Me.Rows.Count - iFIRSTDATAROW)
        End While

        ' Sanity check whether grid can accomodate all Levels + header
        Debug.Assert(Me.Rows.Count = Me.ActiveConfig.Levels.Count + 1)

        ' Populate rows
        For iRow As Integer = 1 To Me.ActiveConfig.Levels.Count
            UpdateRow(iRow)
        Next iRow

    End Sub

    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        ' Should size to fit header
        Me.Columns(eColumnTypes.LevelIndexValue).Width = 80
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)

        Dim fi As cPedigreeLevelInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim aCells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.IVisualModel = Nothing
        Dim strText As String = ""
        Dim ewec As ICell = Nothing

        Me.AllowUpdates = False

        fi = DirectCast(Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW), cPedigreeLevelInfo)
        ri = Me.Rows(iRow)

        ri.Tag = fi
        aCells = ri.GetCells()

        ' No need to update since row number will not change
        'pos = New Position(iRow, eColumnTypes.LevelIndex)
        'aCells(eColumnTypes.LevelIndex).SetValue(pos, CInt(iRow))

        'pos = New Position(iRow, eColumnTypes.LevelName)
        'aCells(eColumnTypes.LevelName).SetValue(pos, fi.Name)
        Me(iRow, CInt(eColumnTypes.LevelName)).Value = fi.Name

        'pos = New Position(iRow, eColumnTypes.LevelIndexValue)
        'aCells(eColumnTypes.LevelIndexValue).SetValue(pos, fi.IndexValue)
        Me(iRow, CInt(eColumnTypes.LevelIndexValue)).Value = fi.IndexValue

        'pos = New Position(iRow, eColumnTypes.LevelConfidenceInterval)
        'aCells(eColumnTypes.LevelConfidenceInterval).SetValue(pos, CInt(100 * fi.ConfidenceInterval))
        ewec = Me(iRow, CInt(eColumnTypes.LevelConfidenceInterval))
        ewec.Value = fi.ConfidenceInterval

        Select Case fi.Status
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

        Me(iRow, CInt(eColumnTypes.LevelStatus)).VisualModel = vm
        Me(iRow, CInt(eColumnTypes.LevelStatus)).Value = strText

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

        Dim fi As cPedigreeLevelInfo = DirectCast(Me.ActiveConfig.Levels(p.Row - 1), cPedigreeLevelInfo)

        Select Case DirectCast(p.Column, eColumnTypes)

            Case eColumnTypes.LevelIndexValue
                fi.IndexValue = CSng(cell.GetValue(p))

            Case eColumnTypes.LevelConfidenceInterval
                fi.ConfidenceInterval = CSng(CInt(cell.GetValue(p)) / 100.0!)

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

        Dim fi As cPedigreeLevelInfo = DirectCast(Me.ActiveConfig.Levels(p.Row - 1), cPedigreeLevelInfo)

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.LevelIndex
                ' Not possible

            Case eColumnTypes.LevelName
                Dim strName As String = CStr(cell.GetValue(p))
                ' Check if name is unique
                For iLevel As Integer = 0 To Me.ActiveConfig.Levels.Count - 1
                    Dim giTemp As cPedigreeLevelInfo = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)
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
        End Select

    End Sub

#End Region ' Grid interaction

#Region " Row manipulation "

    Public Sub SelectLevel(ByVal Level As cPedigreeLevel)

        Dim fi As cPedigreeLevelInfo = Nothing

        If (Level Is Nothing) Then Return

        For iRow As Integer = iFIRSTDATAROW To Me.RowsCount - 1
            fi = DirectCast(Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW), cPedigreeLevelInfo)
            If (Object.ReferenceEquals(fi.Level, Level)) Then
                Me.SelectRow(iRow)
                Return
            End If
        Next iRow

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Delete a row from the grid
    ''' </summary>
    ''' <param name="iRow">The index of the row to delete.</param>
    ''' -----------------------------------------------------------------------
    Public Sub ToggleDeleteRow(Optional ByVal iRow As Integer = -1)

        If iRow = -1 Then iRow = Me.SelectedRow

        Dim iLevel As Integer = iRow - iFIRSTDATAROW
        Dim fi As cPedigreeLevelInfo = Nothing
        Dim strPrompt As String = ""

        ' Validate
        If iLevel < 0 Then Return

        fi = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)
        ' Toggle 'flagged for deletion' flag
        fi.FlaggedForDeletion = Not fi.FlaggedForDeletion

        ' Check to see what is to happen to the Level now
        Select Case fi.Status

            Case eItemStatusTypes.Original
                ' Clear removed status of the Level
                Me.ActiveConfig.LevelsRemoved.Remove(Me.ActiveConfig.Levels(iLevel))

            Case eItemStatusTypes.Added
                ' Clear removed status of the Level
                Me.ActiveConfig.LevelsRemoved.Remove(Me.ActiveConfig.Levels(iLevel))

            Case eItemStatusTypes.Removed
                ' Set removed status
                Me.ActiveConfig.LevelsRemoved.Add(Me.ActiveConfig.Levels(iLevel))

            Case eItemStatusTypes.Invalid
                ' Set removed status
                Me.ActiveConfig.Levels.RemoveAt(iLevel)

        End Select

        Me.UpdateGrid()

    End Sub

    ''' <summary>
    ''' States whether a row holds a Level.
    ''' </summary>
    ''' <param name="iRow"></param>
    ''' <returns></returns>
    Public Function IsDataRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (iRow >= iFIRSTDATAROW) And (iRow < Me.RowsCount)
    End Function

    ''' <summary>
    ''' States whether the Level on a row is flagged for deletion.
    ''' </summary>
    Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not IsDataRow(iRow) Then Return False

        Dim iLevel As Integer = iRow - iFIRSTDATAROW
        Dim fi As cPedigreeLevelInfo = Nothing
        Dim strPrompt As String = ""

        fi = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)
        Return fi.FlaggedForDeletion
    End Function

    ''' <summary>
    ''' Insert a row by creating a new Level.
    ''' </summary>
    Public Sub InsertRow(Optional ByVal iRow As Integer = -1)
        If iRow = -1 Then iRow = Me.SelectedRow()
        If iRow = -1 Then iRow = Math.Max(iFIRSTDATAROW, Me.RowsCount)
        If Not Me.CanInsertRow(iRow) Then Return
        Me.CreateLevel(iRow)
    End Sub

    ''' <summary>
    ''' Create a new Level.
    ''' </summary>
    Private Sub CreateLevel(ByVal iRow As Integer)

        Dim iLevel As Integer = -1
        Dim fi As cPedigreeLevelInfo = Nothing
        Dim lstrLevelNames As New List(Of String)

        ' Make fit
        iRow = Math.Max(iFIRSTDATAROW, iRow)
        iLevel = iRow - iFIRSTDATAROW

        ' Validate
        If iLevel < 0 Then Return

        ' Gather Level names for generating new number
        For i As Integer = 0 To Me.ActiveConfig.Levels.Count - 1
            lstrLevelNames.Add(Me.ActiveConfig.Levels(i).Name)
        Next i

        fi = New cPedigreeLevelInfo(String.Format("Estimate type {0}", _
                cStringUtils.GetNextNumber(lstrLevelNames.ToArray, "Estimate type {0}")))
        Me.ActiveConfig.Levels.Insert(iLevel, fi)

        Me.UpdateGrid()
        Me.SelectRow(fi)
    End Sub

    ''' <summary>
    ''' States whether a row can be inserted at the indicated position.
    ''' </summary>
    Public Function CanInsertRow(Optional ByVal iRow As Integer = -1) As Boolean
        Return True
    End Function

    Public Sub Sort()
        Me.ActiveConfig.Sort()
        Me.UpdateGrid()
    End Sub

    Public Function CanSort() As Boolean
        Return (Me.ActiveConfig.Levels.Count >= 2)
    End Function

#End Region ' Row manipulation 

#Region " Admin "

    Private ReadOnly Property ActiveConfig() As cPedigreeManagerInfo
        Get
            Return Me.m_dictConfigs(Me.m_vnActive)
        End Get
    End Property

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

    Private Overloads Sub SelectRow(ByVal fi As cPedigreeLevelInfo)
        For iLevel As Integer = 0 To Me.ActiveConfig.Levels.Count - 1
            If Object.ReferenceEquals(Me.ActiveConfig.Levels(iLevel), fi) Then
                Me.SelectRow(iLevel + iFIRSTDATAROW)
            End If
        Next
    End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Apply changes "

    Public Function Apply() As Boolean

        Dim bLevelsChanged As Boolean = False
        Dim bConfigChanged As Boolean = False
        Dim bSucces As Boolean = True

        For Each mi As cPedigreeManagerInfo In Me.m_dictConfigs.Values
            mi.AssessChanges()
            bLevelsChanged = bLevelsChanged Or mi.LevelsChanged
            bConfigChanged = bConfigChanged Or mi.ConfigChanged
        Next

        If bConfigChanged Then

            ' Ooh!
            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False
            cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

            For Each mi As cPedigreeManagerInfo In Me.m_dictConfigs.Values
                bSucces = bSucces And mi.ApplyConfigChanges()
            Next

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        End If

        If bLevelsChanged Then

            For Each mi As cPedigreeManagerInfo In Me.m_dictConfigs.Values
                bSucces = bSucces And mi.ApplyLevelChanges()
            Next

        End If

        Return bSucces

    End Function

#End Region ' Apply changes

End Class
