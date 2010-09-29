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
    ''' <summary>Dictionary, per variable, of pedigree levels.</summary>
    Private m_dictConfigs As New Dictionary(Of eVarNameFlags, cPedigreeManagerInfo)
    ''' <summary>Variab.</summary>
    Private m_vnActive As eVarNameFlags = eVarNameFlags.NotSet

    ''' <summary>Update lock, used to distinguish between code updates and
    ''' user updates of grid cells. When grid cells are updated from within
    ''' the code, an update lock should be active to prevent edit/update recursion.</summary>
    Private m_iUpdateLock As Integer = 0

    ''' <summary>Enumerated type defining the columns in this grid.</summary>
    Private Enum eColumnTypes As Integer
        LevelIndex = 0
        LevelName
        LevelColor
        LevelIndexValue
        LevelConfidenceInterval
        LevelStatus
    End Enum

#End Region ' Private vars

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for sorting a list of pedigree levels info bits.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cPedigreeInfoListSorter
        Implements IComparer(Of cPedigreeLevelInfo)

        Public Function Compare(ByVal x As cPedigreeLevelInfo, ByVal y As cPedigreeLevelInfo) As Integer _
            Implements IComparer(Of cPedigreeLevelInfo).Compare
            ' Sort by index value ascending
            If x.IndexValue < y.IndexValue Then Return -1
            If x.IndexValue > y.IndexValue Then Return 1
            ' Sort by confidence interval descending
            If x.ConfidenceInterval > y.ConfidenceInterval Then Return -1
            If x.ConfidenceInterval < y.ConfidenceInterval Then Return 1
            ' Last resort - sort by name
            Return String.Compare(x.Name, y.Name)
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

            ' Add info objects for all existing levels in the manager
            For iLevel As Integer = 0 To Me.m_man.NumLevels - 1
                Dim level As cPedigreeLevel = Me.m_man.Level(iLevel)
                Dim lvlInfo As cPedigreeLevelInfo = New cPedigreeLevelInfo(level)
                Me.m_lfiLevels.Add(lvlInfo)
            Next

        End Sub

        Public Function AssessChanges() As Boolean

            Dim iLevel As Integer = 0
            Dim lvlInfo As cPedigreeLevelInfo = Nothing
            Dim strPrompt As String = ""

            Me.m_bConfigChanged = (Me.m_lfiLevelsRemoved.Count > 0)
            Me.m_bLevelsChanged = False

            ' Assess Level changes
            For iLevel = 0 To Me.m_lfiLevels.Count - 1
                lvlInfo = DirectCast(Me.m_lfiLevels(iLevel), cPedigreeLevelInfo)
                ' Check this Level is newly added
                If Object.ReferenceEquals(lvlInfo.Level, Nothing) Then
                    Me.m_bConfigChanged = True
                Else
                    Me.m_bConfigChanged = Me.m_bConfigChanged Or (lvlInfo.Index <> iLevel)
                End If
                Me.m_bLevelsChanged = Me.m_bLevelsChanged Or lvlInfo.IsChanged()
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
            ' Sort the list
            Me.m_lfiLevels.Sort(New cPedigreeInfoListSorter)
            ' Invalidate all index positions, regardless if sort changed anything. This can be improved one day.
            For Each lvlInfo As cPedigreeLevelInfo In Me.m_lfiLevels
                ' Reset indices
                lvlInfo.Index = -1
            Next
        End Sub

        Public Function ApplyConfigChanges() As Boolean

            Dim lvlInfo As cPedigreeLevelInfo = Nothing
            Dim level As cPedigreeLevel = Nothing
            Dim iLevel As Integer = 0
            Dim bSuccess As Boolean = True

            ' Handle added and removed items
            If (Me.m_bConfigChanged) Then

                Dim iDBID As Integer = Nothing

                ' Add new Levels
                For iLevel = 0 To Me.m_lfiLevels.Count - 1

                    lvlInfo = DirectCast(Me.m_lfiLevels(iLevel), cPedigreeLevelInfo)
                    If (Object.ReferenceEquals(lvlInfo.Level, Nothing)) Then
                        bSuccess = bSuccess And Me.m_man.AddLevel(lvlInfo.Name, _
                                                                  lvlInfo.Description, _
                                                                  iLevel, _
                                                                  Me.m_vn, _
                                                                  lvlInfo.IndexValue, _
                                                                  lvlInfo.ConfidenceInterval, _
                                                                  iDBID)
                    Else
                        If (iLevel <> lvlInfo.Index) Then
                            bSuccess = bSuccess And Me.m_core.MovePedigreeLevel(lvlInfo.Level.Index, iLevel)
                        End If
                    End If
                Next

                ' Remove deleted (and confirmed) Levels
                For iLevel = 0 To Me.m_lfiLevelsRemoved.Count - 1
                    lvlInfo = DirectCast(Me.m_lfiLevelsRemoved(iLevel), cPedigreeLevelInfo)
                    If (Not Object.ReferenceEquals(lvlInfo.Level, Nothing)) Then
                        bSuccess = bSuccess and  (Me.m_man.RemoveLevel(lvlInfo.Level)) 
                    End If
                Next

                If bSuccess Then
                    Me.m_lfiLevelsRemoved.Clear()
                End If

            End If
            Return bSuccess

        End Function

        Public Function ApplyLevelChanges() As Boolean

            Dim level As cPedigreeLevel = Nothing
            Dim bUpdated As Boolean = False
            Dim bSuccess As Boolean = True

            ' Levels may have been reloaded
            If (Me.m_bLevelsChanged) Then
                For Each lvlInfo As cPedigreeLevelInfo In Me.m_lfiLevels
                    If lvlInfo.IsChanged() Then
                        bUpdated = False
                        ' Find (possibly reloaded) level that matches this lvlInfo
                        For iLevel As Integer = 0 To Me.m_man.NumLevels - 1
                            level = Me.m_man.Level(iLevel)
                            If level.DBID = lvlInfo.Level.DBID Then
                                level.Name = lvlInfo.Name
                                level.PoolColor = lvlInfo.PoolColor
                                level.Description = lvlInfo.Description
                                level.IndexValue = lvlInfo.IndexValue
                                level.ConfidenceInterval = lvlInfo.ConfidenceInterval
                                bUpdated = True
                            End If
                        Next
                        bSuccess = bSuccess And bUpdated
                    End If
                Next
            End If

            Me.m_man.UpdatePedigreeLevels()
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
        ''' <summary>Level color.</summary>
        Private m_iColor As Integer = 0
        ''' <summary>Description for this level.</summary>
        Private m_strDescription As String = ""
        ''' <summary>Index value for this level [0, 1].</summary>
        Private m_sIndexValue As Single = 0.0!
        ''' <summary>Confidence interval for this level [0, 100].</summary>
        Private m_iConfidenceInterval As Integer = 0
        ''' <summary>The status of a Level in the interface.</summary>
        Private m_status As eItemStatusTypes = eItemStatusTypes.Original
        ''' <summary>Index of the pedigree level in its manager.</summary>
        Private m_ID As Integer = 0

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
            Me.m_iColor = level.PoolColor
            Me.m_strDescription = level.Description
            Me.m_sIndexValue = level.IndexValue
            Me.m_iConfidenceInterval = level.ConfidenceInterval
            Me.m_status = eItemStatusTypes.Original
            Me.m_ID = level.ID
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String, _
                       Optional ByVal sIndexValue As Single = 0.0!, _
                       Optional ByVal iConfidenceInterval As Integer = 0)

            Me.m_level = Nothing
            If strName.IndexOf("|"c) > -1 Then
                Dim astrSplit As String() = strName.Split("|"c)
                Me.m_strName = astrSplit(0)
                Me.m_strDescription = astrSplit(1)
            Else
                Me.m_strName = strName
                Me.m_strDescription = ""
            End If
            Me.m_iColor = 0
            Me.m_sIndexValue = sIndexValue
            Me.m_iConfidenceInterval = iConfidenceInterval
            Me.m_status = eItemStatusTypes.Added
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of a level.
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
        ''' Get/set the Color value of a level.
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
        ''' Get/set the description of a level.
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
        ''' Get the <see cref="cPedigreeLevel">EwE Level</see> associated
        ''' with a level.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Level() As cPedigreeLevel
            Get
                Return Me.m_level
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the uncertainty index value of a level expressed as a ratio
        ''' [0, 1].
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IndexValue() As Single
            Get
                Return Me.m_sIndexValue
            End Get
            Set(ByVal value As Single)
                Me.m_sIndexValue = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the uncertainty confidence interval of a level, expressed
        ''' as a percentage [0, 100].
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ConfidenceInterval() As Integer
            Get
                Return Me.m_iConfidenceInterval
            End Get
            Set(ByVal value As Integer)
                Me.m_iConfidenceInterval = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the list index of a level in the local manager.
        ''' </summary>
        ''' <remarks>
        ''' This is a LOCAL value for use in the EditPedigree grid context only,
        ''' and bears little relationship to <see cref="cPedigreeLevel.Index">cPedigreeLevel.Index</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property Index() As Integer
            Get
                Return Me.m_ID
            End Get
            Set(ByVal value As Integer)
                Me.m_ID = value
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
            Return (Me.m_level.Name <> Me.Name) Or _
                   (Me.m_level.PoolColor <> Me.m_iColor) Or _
                   (Me.m_level.Description <> Me.Description) Or _
                   (Me.m_level.IndexValue <> Me.IndexValue) Or _
                   (Me.m_level.ConfidenceInterval <> Me.ConfidenceInterval)
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
                If Me.RowsCount > iFIRSTDATAROW Then
                    Me.SelectRow(iFIRSTDATAROW)
                End If
            End If
        End Set
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create default pedigree levels for the current variable
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Sub CreateDefaults()

        ' Remove all current rows
        For Each lvlInfo As cPedigreeLevelInfo In Me.ActiveConfig.Levels
            lvlInfo.FlaggedForDeletion = True

            ' Check to see what is to happen to the Level now
            Select Case lvlInfo.Status

                Case eItemStatusTypes.Removed
                    ' Set removed status
                    Me.ActiveConfig.LevelsRemoved.Add(lvlInfo)

                Case eItemStatusTypes.Invalid
                    ' Set removed status
                    Me.ActiveConfig.Levels.Remove(lvlInfo)

            End Select
        Next
        Me.ActiveConfig.Levels.AddRange(Me.DefaultLevels())
        Me.UpdateGrid()

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

        Me.ContextMenu = Nothing
        Me.FixedColumnWidths = False

        ' Redim columns
        Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

        ' Level index cell
        Me(0, eColumnTypes.LevelIndex) = New EwEColumnHeaderCell()
        ' Level name cell, editable this time
        Me(0, eColumnTypes.LevelName) = New EwEColumnHeaderCell(My.Resources.HEADER_NAME)
        ' Color
        Me(0, eColumnTypes.LevelColor) = New EwEColumnHeaderCell(My.Resources.HEADER_COLOR)
        ' Index value
        Me(0, eColumnTypes.LevelIndexValue) = New EwEColumnHeaderCell(My.Resources.HEADER_INDEXVALUE)
        ' Confidence interval
        Me(0, eColumnTypes.LevelConfidenceInterval) = New EwEColumnHeaderCell(My.Resources.HEADER_CONFIDENCEINTERVAL)
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

        If (Me.m_dictConfigs.Count <> Me.Core.nPedigreeVariables) Then
            For iVariable As Integer = 1 To Me.Core.nPedigreeVariables
                Dim vn As eVarNameFlags = Me.Core.PedigreeVariable(iVariable)
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

        Dim lvlInfo As cPedigreeLevelInfo = Nothing
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

            Me(iRow, eColumnTypes.LevelName) = New Cells.Real.Cell("", GetType(String))
            Me(iRow, eColumnTypes.LevelName).Behaviors.Add(Me.EwEEditHandler)
            Me(iRow, eColumnTypes.LevelName).DataModel.EditableMode = EditableMode.Default

            Me(iRow, eColumnTypes.LevelColor) = New Cells.Real.Cell()
            Me(iRow, eColumnTypes.LevelColor).VisualModel = New cColorCellVisualizer()
            Me(iRow, eColumnTypes.LevelColor).Behaviors.Add(Me.EwEEditHandler)

            Me(iRow, eColumnTypes.LevelIndexValue) = New EwECell(0.0!, GetType(Single))
            Me(iRow, eColumnTypes.LevelIndexValue).Behaviors.Add(Me.EwEEditHandler)

            ewec = New EwECell(0, GetType(Integer))
            ewec.SuppressZero = True
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

        Me.Columns(eColumnTypes.LevelIndex).Width = 24
        Me.Columns(eColumnTypes.LevelIndex).AutoSizeMode = SourceGrid2.AutoSizeMode.None
        Me.Columns(eColumnTypes.LevelName).Width = 120
        Me.Columns(eColumnTypes.LevelName).AutoSizeMode = SourceGrid2.AutoSizeMode.EnableStretch
        Me.Columns(eColumnTypes.LevelColor).Width = 80

        For i As Integer = 2 To Me.ColumnsCount - 1
            Me(0, i).VisualModel.TextAlignment = ContentAlignment.MiddleLeft
        Next

        Me.AutoSize = True
        Me.StretchColumnsToFitWidth()

    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)
        If Me.ColumnsCount > 0 Then
            Me.Columns(eColumnTypes.LevelName).Width = 100
            Me.StretchColumnsToFitWidth()
        End If
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refresh the content of the Row with the given index.
    ''' </summary>
    ''' <param name="iRow">The index of the row to refresh.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateRow(ByVal iRow As Integer)

        Dim lvlInfo As cPedigreeLevelInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.IVisualModel = Nothing
        Dim strText As String = ""
        Dim ewec As ICell = Nothing
        Dim clr As Color

        Me.AllowUpdates = False

        lvlInfo = DirectCast(Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW), cPedigreeLevelInfo)
        ri = Me.Rows(iRow)

        clr = cColorUtils.IntToColor(lvlInfo.PoolColor)
        If clr.A = 0 Then clr = Me.StyleGuide.PedigreeColorDefault(iRow - iFIRSTDATAROW, Me.RowsCount - iFIRSTDATAROW - 1)

        ri.Tag = lvlInfo

        Me(iRow, eColumnTypes.LevelName).Value = lvlInfo.Name
        Me(iRow, eColumnTypes.LevelColor).Value = clr
        Me(iRow, eColumnTypes.LevelIndexValue).Value = lvlInfo.IndexValue
        Me(iRow, eColumnTypes.LevelConfidenceInterval).Value = lvlInfo.ConfidenceInterval

        Select Case lvlInfo.Status
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

        Me(iRow, eColumnTypes.LevelStatus).VisualModel = vm
        Me(iRow, eColumnTypes.LevelStatus).Value = strText

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

        Dim lvlInfo As cPedigreeLevelInfo = DirectCast(Me.ActiveConfig.Levels(p.Row - 1), cPedigreeLevelInfo)

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.LevelIndex
                ' Not possible

            Case eColumnTypes.LevelName
                Dim strName As String = CStr(cell.GetValue(p))
                ' Check if name is unique
                For iLevel As Integer = 0 To Me.ActiveConfig.Levels.Count - 1
                    Dim giTemp As cPedigreeLevelInfo = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)
                    ' Does name already exist?
                    If (Not Object.ReferenceEquals(giTemp, lvlInfo)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                        ' Change is not allowed
                        Me.UpdateRow(p.Row)
                        ' Report failure
                        Return False
                    End If
                Next
                ' Allow name change
                lvlInfo.Name = strName

            Case eColumnTypes.LevelIndexValue
                lvlInfo.IndexValue = CSng(cell.GetValue(p))

            Case eColumnTypes.LevelConfidenceInterval
                ' Store value, truncated to [0, 100]
                lvlInfo.ConfidenceInterval = Math.Min(100, Math.Max(0, CInt(cell.GetValue(p))))

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
            Case eColumnTypes.LevelColor
                Me.SelectCustomColor(p.Row)
        End Select

    End Sub

#End Region ' Grid interaction

#Region " Row manipulation "

    Public Sub SelectLevel(ByVal Level As cPedigreeLevel)

        Dim lvlInfo As cPedigreeLevelInfo = Nothing

        If (Level Is Nothing) Then Return

        For iRow As Integer = iFIRSTDATAROW To Me.RowsCount - 1
            lvlInfo = DirectCast(Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW), cPedigreeLevelInfo)
            If (Object.ReferenceEquals(lvlInfo.Level, Level)) Then
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
    ''' <param name="tsDelete">Flag stating the new row state. <see cref="TriState.UseDefault">UseDefault</see> 
    ''' will perform a true toggle, <see cref="TriState.[True]">True</see> will delete the row,
    ''' and <see cref="TriState.[False]">False</see> will undelete the row.
    ''' </param>
    ''' -----------------------------------------------------------------------
    Public Sub ToggleDeleteRow(Optional ByVal iRow As Integer = -1, _
                               Optional ByVal tsDelete As TriState = TriState.UseDefault)

        If iRow = -1 Then iRow = Me.SelectedRow

        Dim iLevel As Integer = iRow - iFIRSTDATAROW
        Dim lvlInfo As cPedigreeLevelInfo = Nothing
        Dim strPrompt As String = ""

        ' Validate
        If iLevel < 0 Then Return

        lvlInfo = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)

        Select Case tsDelete
            Case TriState.True
                lvlInfo.FlaggedForDeletion = True
            Case TriState.False
                lvlInfo.FlaggedForDeletion = False
            Case TriState.UseDefault
                ' Toggle 'flagged for deletion' flag
                lvlInfo.FlaggedForDeletion = Not lvlInfo.FlaggedForDeletion
        End Select

        ' Check to see what is to happen to the Level now
        Select Case lvlInfo.Status

            Case eItemStatusTypes.Original
                ' Clear removed status of the Level
                Me.ActiveConfig.LevelsRemoved.Remove(lvlInfo)

            Case eItemStatusTypes.Added
                ' Clear removed status of the Level
                Me.ActiveConfig.LevelsRemoved.Remove(lvlInfo)

            Case eItemStatusTypes.Removed
                ' Set removed status
                Me.ActiveConfig.LevelsRemoved.Add(lvlInfo)

            Case eItemStatusTypes.Invalid
                ' Destroy new entry
                Me.ActiveConfig.Levels.Remove(lvlInfo)

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
        Dim lvlInfo As cPedigreeLevelInfo = Nothing
        Dim strPrompt As String = ""

        lvlInfo = DirectCast(Me.ActiveConfig.Levels(iLevel), cPedigreeLevelInfo)
        Return lvlInfo.FlaggedForDeletion
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
        Dim lvlInfo As cPedigreeLevelInfo = Nothing
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

        lvlInfo = New cPedigreeLevelInfo(String.Format("Estimate type {0}", _
                cStringUtils.GetNextNumber(lstrLevelNames.ToArray, "Estimate type {0}")))
        Me.ActiveConfig.Levels.Insert(iLevel, lvlInfo)

        Me.UpdateGrid()
        Me.SelectRow(lvlInfo)
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

    Public Property SelectedLevelDescription() As String
        Get
            Dim iRow As Integer = Me.SelectedRow - iFIRSTDATAROW
            If (iRow < 0) Then Return ""
            Return Me.ActiveConfig.Levels(iRow).Description
        End Get
        Set(ByVal value As String)
            Dim iRow As Integer = Me.SelectedRow - iFIRSTDATAROW
            If (iRow < 0) Then Return
            Me.ActiveConfig.Levels(iRow).Description = value
        End Set
    End Property

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

    Private Overloads Sub SelectRow(ByVal info As cPedigreeLevelInfo)
        For iLevel As Integer = 0 To Me.ActiveConfig.Levels.Count - 1
            If Object.ReferenceEquals(Me.ActiveConfig.Levels(iLevel), info) Then
                Me.SelectRow(iLevel + iFIRSTDATAROW)
            End If
        Next
    End Sub

#End Region ' Selection extension

    Private Function DefaultLevels() As cPedigreeLevelInfo()

        Dim lLevels As New List(Of cPedigreeLevelInfo)

        Select Case Me.m_vnActive

            Case eVarNameFlags.Biomass

                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_ESTIMATED, 0, 0))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_OTHERMODEL, 0.1, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_GUESSTIMATE, 0.2, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_APPROX_INDIRECT, 0.7, 40))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SAMPLING_LOW, 0.7, 40))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SAMPLING_HIGH, 1.0, 20))

            Case eVarNameFlags.PBInput, eVarNameFlags.QBInput

                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_ESTIMATED, 0, 0))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_GUESSTIMATE, 0.2, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_OTHERMODEL, 0.2, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_EMPERICAL, 0.5, 50))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SIM_SIM, 0.6, 40))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SIM_SAME, 0.7, 30))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SAME_SIM, 0.8, 20))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_SAME_SAME, 0.9, 10))

            Case eVarNameFlags.DietComp

                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_GENERAL_SIM, 0.2, 0))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_OTHERMODEL, 0.2, 0))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_GENERAL_SAME, 0.2, 0))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_QUALDC, 0.5, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_QUANDC_LIM, 0.7, 40))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_QUANDC_DET, 1.0, 30))

            Case eVarNameFlags.TCatchInput

                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_GUESSTIMATE, 0.1, 90))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_OTHERMODEL, 0.1, 90))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_FAO, 0.2, 80))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_NATIONAL, 0.5, 50))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_LOCAL_LOW, 0.7, 30))
                lLevels.Add(New cPedigreeLevelInfo(My.Resources.PEDIGREE_DEFAULT_LOCAL_HIGH, 1.0, 10))

        End Select
        Return lLevels.ToArray

    End Function

#End Region ' Admin

#Region " Colors "

    Public Sub SetDefaultFleetColors()
        For iRow As Integer = iFIRSTDATAROW To Me.RowsCount - 1
            Me.SetDefaultFleetColor(iRow)
        Next
    End Sub

    Public Sub SetDefaultFleetColor(Optional ByVal iRow As Integer = -1)
        If iRow = -1 Then iRow = Me.SelectedRow
        Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW).PoolColor = 0
        Me.UpdateRow(iRow)
    End Sub

    Public Sub SelectCustomColor(Optional ByVal iRow As Integer = -1)

        Dim lvlInfo As cPedigreeLevelInfo = Nothing
        Dim dlgColor As cEwEColorDialog = Nothing

        If iRow = -1 Then iRow = Me.SelectedRow

        If Not Me.IsDataRow(iRow) Then Return

        lvlInfo = Me.ActiveConfig.Levels(iRow - iFIRSTDATAROW)

        dlgColor = New cEwEColorDialog()
        dlgColor.Color = cColorUtils.IntToColor(lvlInfo.PoolColor)
        If dlgColor.ShowDialog() = DialogResult.OK Then
            lvlInfo.PoolColor = cColorUtils.ColorToInt(dlgColor.Color)
            Me.UpdateRow(iRow)
        End If

    End Sub

#End Region ' Colors

#Region " Apply changes "

    Public Function Apply() As Boolean

        Dim bLevelsChanged As Boolean = False
        Dim bConfigChanged As Boolean = False
        Dim bSucces As Boolean = True

        For Each manInfo As cPedigreeManagerInfo In Me.m_dictConfigs.Values
            manInfo.AssessChanges()
            bLevelsChanged = bLevelsChanged Or manInfo.LevelsChanged
            bConfigChanged = bConfigChanged Or manInfo.ConfigChanged
        Next

        If bConfigChanged Then

            ' Ooh!
            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then Return False
            cApplicationStatusNotifier.SetStatusText(My.Resources.GENERIC_STATUS_APPLYCHANGES, TriState.True)

            For Each manInfo As cPedigreeManagerInfo In Me.m_dictConfigs.Values
                bSucces = bSucces And manInfo.ApplyConfigChanges()
            Next

            ' The core will reload now
            Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath)
            cApplicationStatusNotifier.SetStatusText("", TriState.False)

        End If

        If bLevelsChanged Then

            For Each manInfo As cPedigreeManagerInfo In Me.m_dictConfigs.Values
                bSucces = bSucces And manInfo.ApplyLevelChanges()
            Next

        End If

        Return bSucces

    End Function

#End Region ' Apply changes

End Class
