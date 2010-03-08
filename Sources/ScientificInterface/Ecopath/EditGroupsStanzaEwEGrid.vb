#Region " Imports "

Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Utilities
Imports SourceGrid2
Imports ScientificInterface.Other
Imports EwEUtils.Drawing

#End Region ' Imports

''' -----------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' -----------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class EditGroupsStanzaEwEGrid
    Inherits EwEGrid

#Region " Private vars "

    ''' <summary>String to clear a group stanza allocation.</summary>
    ''' <remarks>This is a rather hokey solution to represent empty stanza groups.
    ''' Instead, the grid should display "(no stanza)" in stanza combo dropdowns 
    ''' which should be translated to a non-assignment.</remarks>
    Private Const sNO_STANZA As String = ""
    ''' <summary>A number representing the row that contains the first group</summary>
    Private Const iFIRSTGROUPROW As Integer = 1
    ''' <summary>Default VBK</summary>
    Private Const sVBK As Single = 0.3!

    ''' <summary>List of active groups.</summary>
    Private m_lgiGroups As New List(Of GroupInfo)
    ''' <summary>List of removed groups.</summary>
    Private m_lgiGroupsRemoved As New List(Of GroupInfo)
    ''' <summary>List of active stanza configurations.</summary>
    Private m_lsiStanza As New List(Of StanzaInfo)
    ''' <summary>List of removed stanza configurations.</summary>
    Private m_lsiStanzaRemoved As New List(Of StanzaInfo)
    ''' <summary>Custom <see cref="BehaviorModels.IBehaviorModel">behaviour model</see>
    ''' to trap cell edit events locally in this grid. These events are essential
    ''' for keeping the local group/stanza administration up to date.</summary>
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
        GroupIndex = 0
        GroupName
        GroupColor
        GroupPPConsumer
        GroupPPProducer
        GroupPPDetritus
        GroupPP
        StanzaName
        StanzaAge
        GroupStatus
    End Enum

#End Region ' Private vars

#Region " Helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for sorting GroupInfo objects by 
    ''' <see cref="GroupInfo.StanzaAge">Stanza age</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StanzaAgeComparer
        Implements IComparer(Of GroupInfo)

        Public Function Compare(ByVal x As GroupInfo, ByVal y As GroupInfo) As Integer _
                Implements System.Collections.Generic.IComparer(Of GroupInfo).Compare
            If x.StanzaAge < y.StanzaAge Then Return -1
            If x.StanzaAge = y.StanzaAge Then Return 0
            Return 1
        End Function

    End Class

#Region " Group info "
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Administrative unit representing a <see cref="cEcoPathGroupInput">group</see>
    ''' in the EwE model.
    ''' </summary>
    ''' <remarks>
    ''' This class can represent existing and new groups. If this class has its
    ''' <see cref="GroupInfo.Group">Group</see> parameter set, a real live
    ''' group is represented. If this parameter is not set, a new group is
    ''' represented.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Class GroupInfo

        ''' <summary><see cref="cEcoPathGroupInput">Group</see> associated with this group, if any.</summary>
        Private m_group As cEcoPathGroupInput = Nothing
        ''' <summary>PP value for this group.</summary>
        Private m_sPP As Single = 0.0
        ''' <summary>Name for this group.</summary>
        Private m_strName As String = ""
        ''' <summary>Group color.</summary>
        Private m_iColor As Integer = 0
        ''' <summary>Stanza configuration this group belongs to, if any.</summary>
        Private m_stanza As StanzaInfo = Nothing
        ''' <summary>Age of this group within a stanza configuration.</summary>
        Private m_iStanzaAge As Integer = 0
        ''' <summary>Mortality of this group within a stanza configuration.</summary>
        Private m_sMortality As Single = 0.0
        ''' <summary>Flag stating whether a user action is confirmed</summary>
        Private m_bConfirmed As Boolean = True
        ''' <summary>The status of a group in the interface.</summary>
        Private m_status As AddRemoveItemStatus = AddRemoveItemStatus.Original

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="group">The <see cref="cEcoPathGroupInput">group</see> to
        ''' initialize this instance from. If set, this instance represents a
        ''' group currently active in the EwE model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal group As cEcoPathGroupInput)
            Debug.Assert(group IsNot Nothing)
            Me.m_group = group
            Me.m_strName = group.Name
            Me.m_sPP = group.PP
            Me.m_iColor = group.PoolColor
            Me.m_status = AddRemoveItemStatus.Original
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String)
            Me.m_group = Nothing
            Me.m_strName = strName
            Me.m_sPP = CSng(ePrimaryProductionTypes.Consumer)
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
        ''' Get/set the <see cref="cEcopathDataStructures.PP">PP</see> value of
        ''' this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property PP() As Single
            Get
                Return Me.m_sPP
            End Get
            Set(ByVal value As Single)
                Me.m_sPP = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="cEcopathDataStructures.GroupColor">Color</see> value of
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
        ''' Get the <see cref="cEcoPathGroupInput">EwE group</see> associated
        ''' with this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Group() As cEcoPathGroupInput
            Get
                Return Me.m_group
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the stanza configuration this administrative unit is part of,
        ''' if any. Set this property to Nothing to clear a stanza assigment.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Stanza() As StanzaInfo
            Get
                Return Me.m_stanza
            End Get
            Set(ByVal value As StanzaInfo)
                Me.m_stanza = value
            End Set
        End Property

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
        ''' Get/set the age of this group within a stanza configuration.
        ''' </summary>
        ''' <remarks>
        ''' Although this value would be better placed in either 
        ''' <see cref="StanzaInfo">StanzaInfo</see> or in a not yet existing
        ''' StanzaLifeStage wrapper, it has been opted to embed this value in
        ''' the group for ease of use in the group-based grid.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property StanzaAge() As Integer
            Get
                Return m_iStanzaAge
            End Get
            Set(ByVal value As Integer)
                Me.m_iStanzaAge = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the mortality of this group within a stanza configuration.
        ''' </summary>
        ''' <remarks>
        ''' Although this value would be better placed in either 
        ''' <see cref="StanzaInfo">StanzaInfo</see> or in a not yet existing
        ''' StanzaLifeStage wrapper, it has been opted to embed this value in
        ''' the group for ease of use in the group-based grid.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property StanzaMortality() As Single
            Get
                Return m_sMortality
            End Get
            Set(ByVal value As Single)
                Me.m_sMortality = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the group has changed.
        ''' </summary>
        ''' <returns>
        ''' True when either <see cref="Name">Name</see>, <see cref="PP">PP</see>
        ''' values or <see cref="Color">Color </see> has changed.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged() As Boolean

            If (Me.IsNew()) Then Return False

            Return (Me.m_group.Name <> Me.m_strName) Or _
                   (Me.m_group.PP <> Me.m_sPP) Or _
                   (Me.m_group.PoolColor <> Me.m_iColor)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the group is to be created.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.m_group Is Nothing)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function IsLifeStageChanged() As Boolean

            Dim sg As cStanzaGroup = Nothing
            Dim iStanza As Integer = -1

            ' Not a stanza group: no lifestage changes
            If Me.Stanza Is Nothing Then Return False
            ' No group? New but part of a stanza: lifestage changed
            If Me.Group Is Nothing Then Return True

            sg = Me.Stanza.StanzaGroup

            ' No previous stanza group? changed
            If sg Is Nothing Then Return True

            ' Find pos of this group in the original stanza group
            For i As Integer = 0 To sg.NStanzas - 1
                If sg.iGroups(i) = (Me.Group.Index - 1) Then
                    iStanza = i
                End If
            Next

            ' Must exist
            Debug.Assert(iStanza >= 0)

            ' Check if age has changed
            Return (sg.StartAge(iStanza) <> Me.StanzaAge)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VBK() As Single
            Get
                If (Me.m_stanza Is Nothing) Then
                    If (Me.m_group Is Nothing) Then
                        Return EditGroupsStanzaEwEGrid.sVBK
                    Else
                        Return Me.m_group.VBK
                    End If
                Else
                    Return Me.m_stanza.VBK
                End If
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property FlaggedForDeletion() As Boolean
            Get
                Return Me.m_status = AddRemoveItemStatus.Removed
            End Get
            Set(ByVal bDelete As Boolean)
                If Me.m_group IsNot Nothing Then
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

#End Region ' Group Info

#Region " Stanza info "
    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Administrative unit representing a <see cref="cStanzaGroup">stanza configuration</see>
    ''' in the EwE model.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class StanzaInfo

        ''' <summary>Core stanza group, if any.</summary>
        Private m_sg As cStanzaGroup = Nothing
        ''' <summary>Name of this stanza configuration.</summary>
        Private m_strName As String = ""
        ''' <summary>List of groups in this stanza configuration.</summary>
        Private m_alGroups As New List(Of GroupInfo)
        ''' <summary>VBK value of a stanza group.</summary>
        Private m_sVBK As Single = EditGroupsStanzaEwEGrid.sVBK

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="sg">The <see cref="cStanzaGroup">stanza configuration</see>
        ''' to initialize this instance from. If set, this instance represents a
        ''' stanza configuration currently active in the EwE model.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal sg As cStanzaGroup, ByVal sVBK As Single)

            ' Sanity check
            Debug.Assert(sg IsNot Nothing)

            Me.m_sg = sg
            Me.m_strName = sg.Name
            Me.m_sVBK = sVBK

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instanze of this class.
        ''' </summary>
        ''' <param name="strName">Name to assign to this administrative unit.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal strName As String)
            Me.m_sg = Nothing
            Me.m_strName = strName
            Me.m_sVBK = EditGroupsStanzaEwEGrid.sVBK
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Adds a group to this administrative unit.
        ''' </summary>
        ''' <param name="gi"><see cref="GroupInfo">Group</see> to add.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddGroup(ByVal gi As GroupInfo)
            Dim iPos As Integer = Me.FindGroups(gi)
            ' Group not in this SI yet
            Debug.Assert(iPos = -1)
            If (gi.Stanza IsNot Nothing) Then
                gi.Stanza.RemoveGroup(gi)
            End If
            Debug.Assert(gi.Stanza Is Nothing)
            Me.m_alGroups.Add(gi)
            gi.Stanza = Me
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Removes a group from this administrative unit.
        ''' </summary>
        ''' <param name="gi"><see cref="GroupInfo">Group</see> to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveGroup(ByVal gi As GroupInfo)
            Dim iPos As Integer = Me.FindGroups(gi)

            Debug.Assert(iPos <> -1)
            Debug.Assert(Object.ReferenceEquals(gi.Stanza, Me))

            Me.m_alGroups.RemoveAt(iPos)
            gi.Stanza = Nothing
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Moves a group to a different position in the life cycle list.
        ''' </summary>
        ''' <param name="giMove"></param>
        ''' <param name="iTo"></param>
        ''' -------------------------------------------------------------------
        Public Sub MoveGroup(ByVal giMove As GroupInfo, ByVal iTo As Integer)

            Dim objTemp As GroupInfo = Nothing
            Dim iFrom As Integer = Me.FindGroups(giMove)

            ' Found group?
            If iFrom = -1 Then Return
            ' Truncate iTo
            iTo = Math.Min(Math.Max(0, iTo), Me.m_alGroups.Count - 1)

            objTemp = Me.m_alGroups(iTo)
            Me.m_alGroups(iTo) = giMove
            Me.m_alGroups(iFrom) = objTemp
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the list of groups for this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GroupList() As List(Of GroupInfo)
            Return Me.m_alGroups
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the number of groups for this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function NumGroups() As Integer
            Return Me.m_alGroups.Count
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the name of this administrative unit.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Property Name() As String
            Get
                Return Me.m_strName
            End Get
            Set(ByVal value As String)
                Me.m_strName = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="cStanzaGroup">cStanzaGroup</see> associated with
        ''' this StanzaInfo, if anything.
        ''' </summary>
        ''' -------------------------------------------------------------------
        ReadOnly Property StanzaGroup() As cStanzaGroup
            Get
                Return Me.m_sg
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this stanza configuration has any groups assigned.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function HasGroups() As Boolean
            Return (Me.NumGroups > 0)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Finds the zero-based index of a group in this stanza configuration.
        ''' </summary>
        ''' <param name="gi">The <see cref="GroupInfo">group</see> to find.</param>
        ''' <returns>A zero-based index or -1 if the group was not found.</returns>
        ''' -------------------------------------------------------------------
        Public Function FindGroups(ByVal gi As GroupInfo) As Integer
            For i As Integer = 0 To Me.m_alGroups.Count - 1
                If Object.ReferenceEquals(Me.m_alGroups(i), gi) Then
                    Return i
                End If
            Next
            Return -1
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the user has modified the stanza group administrative
        ''' unit.
        ''' </summary>
        ''' <returns>True if changed.</returns>
        ''' -------------------------------------------------------------------
        Public Function IsChanged(ByVal core As cCore) As Boolean
            Dim gi As GroupInfo = Nothing
            Dim group As cEcoPathGroupInput = Nothing

            ' Not associated with an existing stanza group? Flag as changed.
            If Object.ReferenceEquals(Me.m_sg, Nothing) Then Return True
            ' Name modified? Flag as changed.
            If Me.m_sg.Name <> Me.m_strName Then Return True
            ' Different number of groups? Flag as changed.
            If Me.m_sg.NStanzas <> Me.NumGroups Then Return True

            ' No changes detected yet? Try to find if group assignment and
            ' group order has changed.
            For i As Integer = 0 To Me.NumGroups - 1
                ' Get group info admin unit
                gi = Me.m_alGroups(i)
                ' Is a new group? Flag as changed
                If Object.ReferenceEquals(gi.Group, Nothing) Then Return True
                ' Is an existing group. Now check if group order has changed.
                group = core.EcoPathGroupInputs(Me.StanzaGroup.iGroups(i + 1))
                If Not Object.ReferenceEquals(gi.Group, group) Then Return True
                ' Check if stanza age has changed
                If gi.StanzaAge <> Me.StanzaGroup.StartAge(i + 1) Then Return True
            Next i
            ' No changes detected.
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this stanza configuration is to be created.
        ''' </summary>
        ''' <returns>True if new.</returns>
        ''' -------------------------------------------------------------------
        Public Function IsNew() As Boolean
            Return (Me.m_sg Is Nothing)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sort the stanza config by Age in ascending order.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Sort()
            Me.m_alGroups.Sort(New StanzaAgeComparer())
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the vbK value for a stanza configuration.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property VBK() As Single
            Get
                Return Me.m_sVBK
            End Get
        End Property

    End Class

#End Region ' Stanza info

#End Region ' Helper classes

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

        'Me.Selection.SelectionMode = GridSelectionMode.Row
        Me.Selection.EnableMultiSelection = False

        ' Redim columns
        Me.Redim(1, System.Enum.GetValues(GetType(eColumnTypes)).Length)

        ' Group index cell
        Me(0, eColumnTypes.GroupIndex) = New EwEColumnHeaderCell()
        ' Group name cell, editable this time
        Me(0, eColumnTypes.GroupName) = New EwEColumnHeaderCell(My.Resources.HEADER_GROUPNAME)

        ' PP cells
        Me(0, eColumnTypes.GroupPPConsumer) = New EwEColumnHeaderCell(My.Resources.HEADER_CONSUMER)
        Me(0, eColumnTypes.GroupPPProducer) = New EwEColumnHeaderCell(My.Resources.HEADER_PRODUCER)
        Me(0, eColumnTypes.GroupPPDetritus) = New EwEColumnHeaderCell(My.Resources.HEADER_DETRITUS)
        Me(0, eColumnTypes.GroupPP) = New EwEColumnHeaderCell(My.Resources.HEADER_PP)

        ' Color
        Me(0, eColumnTypes.GroupColor) = New EwEColumnHeaderCell(My.Resources.HEADER_COLOR)

        ' Stanza cells
        Me(0, eColumnTypes.StanzaName) = New EwEColumnHeaderCell(My.Resources.HEADER_STANZAGROUP_NAME)
        Me(0, eColumnTypes.StanzaAge) = New EwEColumnHeaderCell(My.Resources.HEADER_STANZA_AGE)

        ' Group index cell
        Me(0, eColumnTypes.GroupStatus) = New EwEColumnHeaderCell(My.Resources.HEADER_STATUS)

        ' Fix index column only; group name column cannot be fixed because it must be editable
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

        ' Get the core reference
        Dim group As cEcoPathGroupInput = Nothing
        Dim gi As GroupInfo = Nothing
        Dim stanza As cStanzaGroup = Nothing
        Dim si As StanzaInfo = Nothing

        ' Populate local administration from a snapshot of the live data
        Me.m_lgiGroups.Clear()

        ' Make snapshot of group configuration
        For iGroup As Integer = 1 To Core.nGroups
            group = Core.EcoPathGroupInputs(iGroup)
            gi = New GroupInfo(group)
            Me.m_lgiGroups.Add(gi)
        Next

        ' Make snapshot of stanza configuration
        For iStanza As Integer = 0 To Core.nStanzas - 1
            stanza = Core.StanzaGroups(iStanza)
            si = New StanzaInfo(stanza, Core.EcoPathGroupInputs(stanza.iGroups(1)).VBK)
            Me.m_lsiStanza.Add(si)

            ' Stanza group list is a one-based array
            For iGroup As Integer = 1 To stanza.NStanzas
                ' ..while local groupinfo admin is a zero-based array. Whoohoo.
                Dim isg As Integer = stanza.iGroups(iGroup)
                gi = DirectCast(Me.m_lgiGroups(isg - iFIRSTGROUPROW), GroupInfo)
                ' Copy start age
                gi.StanzaAge = stanza.StartAge(iGroup)
                gi.StanzaMortality = stanza.Mortality(iGroup)
                si.AddGroup(gi)
            Next
        Next

        ' This might have resulted in some cleaning-up
        Dim aStanza As StanzaInfo() = Me.m_lsiStanza.ToArray()
        For iStanza As Integer = 0 To aStanza.Length - 1
            si = aStanza(iStanza)
            If si.NumGroups = 0 Then
                Me.m_lsiStanza.Remove(si)
            End If
        Next

        ' Brute-force update grid
        UpdateGrid()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to configure column widths.
    ''' </summary>
    Protected Overrides Sub FinishStyle()
        MyBase.FinishStyle()
        Me.AutoSizeColumnRange(1, Me.ColumnsCount - 1, 1, Me.RowsCount - 1)
    End Sub

    Protected Overrides Function DefaultDockStyle() As System.Windows.Forms.DockStyle
        Return DockStyle.None
    End Function

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

        Dim gi As GroupInfo = Nothing
        Dim si As StanzaInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim astrStanzaNames As String() = Me.GetStanzaNames()
        Dim cells() As Cells.ICellVirtual = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim ewec As EwECell = Nothing

        ' Create missing rows
        For iRow As Integer = Me.Rows.Count To Me.m_lgiGroups.Count
            Me.AddRow()

            ewec = New EwECell(0, GetType(Integer))
            ewec.Style = cStyleGuide.eStyleFlags.Names Or cStyleGuide.eStyleFlags.NotEditable
            Me(iRow, eColumnTypes.GroupIndex) = ewec

            Me(iRow, eColumnTypes.GroupName) = New Cells.Real.Cell("", GetType(String))
            Me(iRow, eColumnTypes.GroupName).Behaviors.Add(m_bm)
            Me(iRow, eColumnTypes.GroupName).DataModel.EditableMode = EditableMode.Default

            Me(iRow, eColumnTypes.GroupPPConsumer) = New Cells.Real.CheckBox(False)
            Me(iRow, eColumnTypes.GroupPPConsumer).Behaviors.Add(m_bm)
            Me(iRow, eColumnTypes.GroupPPProducer) = New Cells.Real.CheckBox(False)
            Me(iRow, eColumnTypes.GroupPPProducer).Behaviors.Add(m_bm)
            Me(iRow, eColumnTypes.GroupPPDetritus) = New Cells.Real.CheckBox(False)
            Me(iRow, eColumnTypes.GroupPPDetritus).Behaviors.Add(m_bm)

            Me(iRow, eColumnTypes.GroupPP) = New EwECell(0, GetType(Single))
            Me(iRow, eColumnTypes.GroupPP).Behaviors.Add(m_bm)

            Me(iRow, eColumnTypes.GroupColor) = New Cells.Real.Cell()
            Me(iRow, eColumnTypes.GroupColor).VisualModel = New cColorCellVisualizer()
            Me(iRow, eColumnTypes.GroupColor).Behaviors.Add(m_bm)

            Dim cmb As Cells.Real.ComboBox = New Cells.Real.ComboBox("", GetType(String), astrStanzaNames, False)
            cmb.EditableMode = EditableMode.SingleClick
            Me(iRow, eColumnTypes.StanzaName) = cmb
            Me(iRow, eColumnTypes.StanzaName).Behaviors.Add(m_bm)
            Me(iRow, eColumnTypes.StanzaAge) = New Cells.Real.Cell(0)
            Me(iRow, eColumnTypes.StanzaAge).Behaviors.Add(m_bm)

            ' Status
            vm = New VisualModels.Common()
            vm.ImageAlignment = ContentAlignment.MiddleCenter
            Me(iRow, eColumnTypes.GroupStatus) = New Cells.Real.Cell()
            Dim dm As New DataModels.DataModelBase(GetType(String))
            dm.EditableMode = EditableMode.None
            Me(iRow, eColumnTypes.GroupStatus).DataModel = dm
        Next

        ' Delete obsolete rows
        While Me.Rows.Count > Me.m_lgiGroups.Count + 1
            Me.Rows.Remove(Me.Rows.Count - iFIRSTGROUPROW)
        End While

        ' Sanity check whether grid can accomodate all groups + header
        Debug.Assert(Me.Rows.Count = Me.m_lgiGroups.Count + 1)

        ' Populate rows
        For iRow As Integer = 1 To Me.m_lgiGroups.Count
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

        Dim gi As GroupInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim ewec As EwECell = Nothing
        Dim pos As SourceGrid2.Position = Nothing
        Dim vm As VisualModels.Common = Nothing
        Dim strText As String = ""

        Me.AllowUpdates = False

        gi = DirectCast(Me.m_lgiGroups(iRow - 1), GroupInfo)
        ri = Me.Rows(iRow)

        ri.Tag = gi

        pos = New Position(iRow, eColumnTypes.GroupIndex)
        Me(iRow, eColumnTypes.GroupIndex).SetValue(pos, CInt(iRow))

        pos = New Position(iRow, eColumnTypes.GroupName)
        Me(iRow, eColumnTypes.GroupName).SetValue(pos, CStr(gi.Name))

        ' Change to radio buttons or edit box + slider + icon?
        pos = New Position(iRow, eColumnTypes.GroupPPConsumer)
        Me(iRow, eColumnTypes.GroupPPConsumer).SetValue(pos, CBool(gi.PP = 0.0))
        pos = New Position(iRow, eColumnTypes.GroupPPProducer)
        Me(iRow, eColumnTypes.GroupPPProducer).SetValue(pos, CBool(gi.PP = 1.0))
        pos = New Position(iRow, eColumnTypes.GroupPPDetritus)
        Me(iRow, eColumnTypes.GroupPPDetritus).SetValue(pos, CBool(gi.PP = 2.0))
        pos = New Position(iRow, eColumnTypes.GroupPP)
        ewec = DirectCast(Me(iRow, eColumnTypes.GroupPP), EwECell)
        ewec.SetValue(pos, gi.PP)
        If (gi.PP > 1.0!) Then
            ewec.Style = ewec.Style Or cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable
        Else
            ewec.Style = ewec.Style And Not (cStyleGuide.eStyleFlags.Null Or cStyleGuide.eStyleFlags.NotEditable)
        End If

        pos = New Position(iRow, eColumnTypes.GroupColor)
        Dim clr As Color = cStyleGuide.IntToColor(gi.PoolColor)
        If clr.A = 0 Then clr = Me.StyleGuide.GroupColorDefault(iRow, Me.m_lgiGroups.Count)
        Me(iRow, eColumnTypes.GroupColor).SetValue(pos, clr)

        Select Case gi.Status
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

        pos = New Position(iRow, eColumnTypes.GroupStatus)
        Me(iRow, eColumnTypes.GroupStatus).VisualModel = vm
        Me(iRow, eColumnTypes.GroupStatus).SetValue(pos, strText)

        Me.UpdateStanzaCells(iRow)

        Me.AllowUpdates = True

    End Sub

    Private Sub UpdateColorColumn()

        Dim clr As Color = Color.Transparent

        Me.AllowUpdates = False
        For iGroup As Integer = 0 To Me.m_lgiGroups.Count - 1
            clr = cStyleGuide.IntToColor(Me.m_lgiGroups(iGroup).PoolColor)
            If clr.A = 0 Then clr = Me.StyleGuide.GroupColorDefault(iGroup + 1, Me.m_lgiGroups.Count)
            Me(iGroup + iFIRSTGROUPROW, eColumnTypes.GroupColor).Value = clr
        Next iGroup
        Me.AllowUpdates = True

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Refreshes the content of the stanza columns in the grid.
    ''' </summary>
    ''' <param name="astrStanzaNames">Optional array of stanza names.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateStanzaColumns(Optional ByVal astrStanzaNames As String() = Nothing)

        Me.AllowUpdates = False

        ' Populate rows
        For iRow As Integer = 1 To Me.m_lgiGroups.Count
            UpdateStanzaCells(iRow, astrStanzaNames)
        Next iRow

        Me.AllowUpdates = True

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update stanza cells in a single row.
    ''' </summary>
    ''' <param name="iRow">Index of the Row to update.</param>
    ''' <param name="astrStanzaNames">Optional array of stanza names.</param>
    ''' -----------------------------------------------------------------------
    Private Sub UpdateStanzaCells(ByVal iRow As Integer, Optional ByVal astrStanzaNames As String() = Nothing)

        Dim pos As SourceGrid2.Position = Nothing
        Dim aCells() As Cells.ICellVirtual = Nothing
        Dim gi As GroupInfo = Nothing
        Dim ri As RowInfo = Nothing
        Dim bHasStanza As Boolean = False
        Dim bCanEditStanza As Boolean = False

        Me.AllowUpdates = False

        If Not Object.ReferenceEquals(astrStanzaNames, Nothing) Then
            Dim cmb As Cells.Real.ComboBox = DirectCast(Me(iRow, eColumnTypes.StanzaName), Cells.Real.ComboBox)
            cmb.DataModel.StandardValues = astrStanzaNames
        End If

        gi = DirectCast(Me.m_lgiGroups(iRow - iFIRSTGROUPROW), GroupInfo)
        ri = Me.Rows(iRow)
        aCells = ri.GetCells()

        ' Determine if group has stanza info
        bHasStanza = (gi.Stanza IsNot Nothing)
        ' Can only edit stanza groups that are active and are no detritus groups
        bCanEditStanza = (gi.Status <> AddRemoveItemStatus.Removed) And (gi.PP <> ePrimaryProductionTypes.Detritus)

        ' Sanity check
        Debug.Assert(aCells IsNot Nothing)

        If bHasStanza And bCanEditStanza Then
            pos = New Position(iRow, eColumnTypes.StanzaName)
            aCells(eColumnTypes.StanzaName).SetValue(pos, gi.Stanza.Name)
            pos = New Position(iRow, eColumnTypes.StanzaAge)

            aCells(eColumnTypes.StanzaAge).DataModel = New SourceGrid2.DataModels.EditorTextBoxNumeric(GetType(Integer))
            aCells(eColumnTypes.StanzaAge).SetValue(pos, gi.StanzaAge)
            aCells(eColumnTypes.StanzaAge).DataModel.EnableEdit = True
            aCells(eColumnTypes.StanzaAge).DataModel.EditableMode = EditableMode.Default
        Else
            pos = New Position(iRow, eColumnTypes.StanzaName)
            aCells(eColumnTypes.StanzaName).SetValue(pos, CStr(sNO_STANZA))
            pos = New Position(iRow, eColumnTypes.StanzaAge)

            aCells(eColumnTypes.StanzaAge).DataModel = New SourceGrid2.DataModels.EditorTextBox(GetType(String))
            aCells(eColumnTypes.StanzaAge).SetValue(pos, "")
            aCells(eColumnTypes.StanzaAge).DataModel.EnableEdit = False
            aCells(eColumnTypes.StanzaAge).DataModel.EditableMode = EditableMode.None
        End If

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

        Dim gi As GroupInfo = DirectCast(Me.m_lgiGroups(p.Row - 1), GroupInfo)
        Dim si As StanzaInfo = DirectCast(gi.Stanza, StanzaInfo)

        Select Case DirectCast(p.Column, eColumnTypes)

            Case eColumnTypes.GroupName
                Dim strName As String = CStr(cell.GetValue(p))
                ' Check if name is unique
                If Me.IsNameUnique(strName, gi) Then
                    ' Allow name change
                    gi.Name = strName
                End If
                Me.UpdateRow(p.Row)

            Case eColumnTypes.GroupPPConsumer
                gi.PP = ePrimaryProductionTypes.Consumer
                Me.UpdateRow(p.Row)

            Case eColumnTypes.GroupPPProducer
                gi.PP = ePrimaryProductionTypes.Producer
                Me.UpdateRow(p.Row)

            Case eColumnTypes.GroupPPDetritus
                gi.PP = ePrimaryProductionTypes.Detritus
                ' Detritus group cannot be part of a stanza configuation
                If gi.Stanza Is Nothing Then
                    Me.UpdateRow(p.Row)
                Else
                    gi.Stanza.RemoveGroup(gi)
                    Me.UpdateRow(p.Row)
                    Me.UpdateStanzaColumns()
                End If

            Case eColumnTypes.GroupPP
                Dim sPP As Single = Single.Parse(CStr(cell.GetValue(p)))
                ' truncate
                sPP = Math.Max(0.0!, Math.Min(2.0!, sPP))
                If (sPP > 1.0) Then sPP = CSng(Math.Round(sPP))
                gi.PP = sPP
                Me.UpdateRow(p.Row)

            Case eColumnTypes.StanzaAge
                ' Update stanza age in group
                gi.StanzaAge = CInt(cell.GetValue(p))
                ' Sort the stanza config
                si.Sort()
                ' Bluntly update entire stanza columns
                Me.UpdateStanzaColumns()

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

        Dim gi As GroupInfo = DirectCast(Me.m_lgiGroups(p.Row - 1), GroupInfo)
        Dim si As StanzaInfo = DirectCast(gi.Stanza, StanzaInfo)

        Select Case DirectCast(p.Column, eColumnTypes)
            Case eColumnTypes.GroupIndex
                ' Not possible

            Case eColumnTypes.GroupName
                Dim strName As String = CStr(cell.GetValue(p))
                ' Check if name is unique
                If Not Me.IsNameUnique(strName, gi) Then
                    ' Change is not allowed
                    Me.UpdateRow(p.Row)
                    ' Report failure
                    Return False
                End If
                ' Allow name change
                gi.Name = strName

            Case eColumnTypes.StanzaName
                Dim strStanzaName As String = CStr(cell.GetValue(p))
                ' Was part of stanza config?
                If Not Object.ReferenceEquals(si, Nothing) Then
                    ' Remove group from this stanza config
                    si.RemoveGroup(gi)
                    gi.Stanza = Nothing

                    ' Stanza group empty?
                    If (Not si.HasGroups()) Then
                        ' #Yes: remove it
                        Me.m_lsiStanzaRemoved.Add(si)
                        Me.m_lsiStanza.Remove(si)
                    End If
                End If

                If (strStanzaName = sNO_STANZA) Then strStanzaName = ""

                ' New stanza assigned?
                If Not String.IsNullOrEmpty(strStanzaName) Then
                    ' Try to find existing stanza group
                    si = Me.FindStanzaInfo(strStanzaName)
                    ' Existing group?
                    If (Object.ReferenceEquals(si, Nothing)) Then
                        si = New StanzaInfo(strStanzaName)
                        Me.m_lsiStanza.Add(si)
                    Else
                    End If
                    ' Add group to this stanza
                    si.AddGroup(gi)
                    gi.Stanza = si
                End If

                ' Reflect changes
                Me.UpdateStanzaColumns(Me.GetStanzaNames())

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
            Case eColumnTypes.GroupColor
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

        Dim iGroup As Integer = iRow - iFIRSTGROUPROW
        Dim gi As GroupInfo = Nothing
        Dim si As StanzaInfo = Nothing
        Dim strPrompt As String = ""

        ' Validate
        If iGroup < 0 Then Return

        ' Check if need to update a stanza configuration
        gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
        ' Toggle 'flagged for deletion' flag
        gi.FlaggedForDeletion = Not gi.FlaggedForDeletion

        ' Check to see what is to happen to the group now
        Select Case gi.Status

            Case AddRemoveItemStatus.Original, AddRemoveItemStatus.Added
                ' Clear removed status of the group, if any
                Me.m_lgiGroupsRemoved.Remove(gi)

            Case AddRemoveItemStatus.Removed, AddRemoveItemStatus.Invalid
                ' Get connected stanza info, if any
                si = gi.Stanza
                ' Part of a stanza config?
                If si IsNot Nothing Then
                    ' #Yes: Remove the group from stanza config
                    si.RemoveGroup(gi)
                    gi.Stanza = Nothing
                    ' Stanza config empty?
                    If (si.HasGroups = False) Then
                        ' #Yes: Remove stanza config
                        Me.m_lsiStanza.Remove(si)
                        ' Flag stanza config for deletion if not new
                        If (Not si.IsNew()) Then Me.m_lsiStanzaRemoved.Add(si)
                    End If
                End If

                ' Remove group from org group list if group is New, there is no need to preserve it.
                If gi.IsNew() Then Me.m_lgiGroups.Remove(gi)
                ' Add to removed group list if group is not new
                If Not gi.IsNew() Then Me.m_lgiGroupsRemoved.Add(gi)

        End Select

        Me.UpdateGrid()

    End Sub

    Public Function IsGroupRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (iRow >= iFIRSTGROUPROW) And (iRow < Me.RowsCount)
    End Function

    Public Function IsFlaggedForDeletionRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not IsGroupRow(iRow) Then Return False

        Dim iGroup As Integer = iRow - iFIRSTGROUPROW
        Dim gi As GroupInfo = Nothing
        Dim strPrompt As String = ""

        ' Check if need to update a stanza configuration
        gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
        Return gi.FlaggedForDeletion
    End Function

    Public Sub InsertRow(Optional ByVal iRow As Integer = -1)

        Dim iGroup As Integer = -1
        Dim gi As GroupInfo = Nothing
        Dim lstrGroupNames As New List(Of String)

        If iRow = -1 Then iRow = Math.Max(iFIRSTGROUPROW, Me.SelectedRow())
        If Not Me.CanInsertRow(iRow) Then Return

        ' Make fit
        iRow = Math.Max(iFIRSTGROUPROW, iRow)
        iGroup = iRow - iFIRSTGROUPROW

        ' Validate
        If iGroup < 0 Then Return

        ' Gather group names for generating new number
        For i As Integer = 0 To Me.m_lgiGroups.Count - 1
            lstrGroupNames.Add(Me.m_lgiGroups(i).Name)
        Next i
        gi = New GroupInfo( _
            String.Format(My.Resources.DEFAULT_NEWGROUP_NUM, cStringUtils.GetNextNumber(lstrGroupNames.ToArray, My.Resources.DEFAULT_NEWGROUP_NUM)))
        Me.m_lgiGroups.Insert(iGroup, gi)

        Me.UpdateGrid()
    End Sub

    Public Function CanInsertRow(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Math.Max(iFIRSTGROUPROW, Me.SelectedRow())
        Return (iRow >= iFIRSTGROUPROW) And (iRow < Me.RowsCount)
    End Function

    Public Sub MoveRowUp(Optional ByVal iRow As Integer = -1)
        Dim bMoveSelection As Boolean = (iRow = -1)

        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not CanMoveRowUp(iRow) Then Return
        Me.MoveRow(iRow, iRow - 1)

        If bMoveSelection Then
            Me.SelectRow(iRow - 1)
        End If
    End Sub

    Public Function CanMoveRowUp(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (Me.RowsCount > (iFIRSTGROUPROW + 1)) And (iRow > iFIRSTGROUPROW)
    End Function

    Public Sub MoveRowDown(Optional ByVal iRow As Integer = -1)
        Dim bMoveSelection As Boolean = (iRow = -1)

        If iRow = -1 Then iRow = Me.SelectedRow()
        If Not CanMoveRowDown(iRow) Then Return
        Me.MoveRow(iRow, iRow + 1)

        If bMoveSelection Then
            Me.SelectRow(iRow + 1)
        End If
    End Sub

    Public Function CanMoveRowDown(Optional ByVal iRow As Integer = -1) As Boolean
        If iRow = -1 Then iRow = Me.SelectedRow()
        Return (Me.RowsCount > (iFIRSTGROUPROW + 1)) And (iRow >= iFIRSTGROUPROW) And (iRow < Me.RowsCount - 1)
    End Function

    Private Sub MoveRow(ByVal iFromRow As Integer, ByVal iToRow As Integer)

        Dim objTemp As GroupInfo = Nothing
        Dim iStep As Integer = 1
        Dim iFromGroup As Integer = iFromRow - iFIRSTGROUPROW
        Dim iToGroup As Integer = iToRow - iFIRSTGROUPROW

        ' Truncate
        iFromGroup = Math.Max(0, Math.Min(Me.m_lgiGroups.Count - 1, iFromGroup))
        iToGroup = Math.Max(0, Math.Min(Me.m_lgiGroups.Count - 1, iToGroup))

        ' Nothing to do? abort
        If iFromGroup = iToGroup Then Return
        ' Determine direction of movement
        If iFromGroup < iToGroup Then iStep = 1 Else iStep = -1

        ' Swap groups (but do not swap the group at iTo because then we've gone 1 too far)
        For iGroup As Integer = iFromGroup To iToGroup - iStep Step iStep
            objTemp = Me.m_lgiGroups(iGroup + iStep)
            Me.m_lgiGroups(iGroup + iStep) = Me.m_lgiGroups(iGroup)
            Me.m_lgiGroups(iGroup) = objTemp
            Me.UpdateRow(iGroup + iFIRSTGROUPROW)
            Me.UpdateRow(iGroup + iFIRSTGROUPROW + iStep)
        Next iGroup

    End Sub

#End Region ' Row manipulation 

#Region " Colors "

    ''' <summary>
    ''' Helper method to load alternating colours for all groups.
    ''' </summary>
    Public Sub SetAlternatingGroupColors()

        Dim lGroupLists As New List(Of List(Of GroupInfo))
        Dim lGroups As List(Of GroupInfo)
        Dim hsvGroup As HSV = Nothing
        Dim hsvLifeStage As HSV = Nothing
        Dim si As StanzaInfo = Nothing
        Dim gi As GroupInfo = Nothing
        Dim bAdded(Me.m_lgiGroups.Count - 1) As Boolean

        For iGroup As Integer = 0 To Me.m_lgiGroups.Count - 1
            gi = Me.m_lgiGroups(iGroup)

            If Not bAdded(iGroup) Then
                bAdded(iGroup) = True
                si = gi.Stanza

                lGroups = New List(Of GroupInfo)
                If si IsNot Nothing Then
                    For iLifeStage As Integer = 1 To si.GroupList.Count - 1
                        bAdded(iLifeStage) = True
                        lGroups.Add(gi)
                    Next
                Else
                    lGroups.Add(gi)
                End If
                lGroupLists.Add(lGroups)
            End If
        Next

        For i As Integer = 0 To lGroupLists.Count - 1
            hsvGroup = cStyleGuide.CalculateAlternatingGroupColor(i + 1, lGroupLists.Count)
            lGroups = lGroupLists(i)
            If lGroups.Count > 1 Then
                For iLifeStage As Integer = 0 To lGroups.Count - 1
                    gi = lGroups(iLifeStage)
                    hsvLifeStage = cStyleGuide.CalculateAlternatingStanzaGroupColor(hsvGroup, iLifeStage, lGroups.Count)
                    gi.PoolColor = cStyleGuide.ColorToInt(HSV.ToColor(hsvLifeStage))
                Next
            Else 'Non stanza group
                gi = lGroups(0)
                gi.PoolColor = cStyleGuide.ColorToInt(HSV.ToColor(hsvGroup))
            End If
        Next

        Me.UpdateColorColumn()
    End Sub

    Public Sub SetScaleGroupColors()

        For iGroup As Integer = 0 To Me.m_lgiGroups.Count - 1
            Me.m_lgiGroups(iGroup).PoolColor = cStyleGuide.ColorToInt(Me.StyleGuide.GroupColorDefault(iGroup + 1, Me.m_lgiGroups.Count))
            Me.UpdateRow(iGroup + iFIRSTGROUPROW)
        Next

        Me.UpdateColorColumn()
    End Sub

    Public Sub SelectCustomColor(Optional ByVal iRow As Integer = -1)

        Dim gi As GroupInfo = Nothing
        Dim dlgColor As ColorDialog = Nothing

        If iRow = -1 Then iRow = Me.SelectedRow

        If Not Me.IsGroupRow(iRow) Then Return

        gi = Me.m_lgiGroups(iRow - iFIRSTGROUPROW)

        dlgColor = New ColorDialog()
        dlgColor.Color = cStyleGuide.IntToColor(gi.PoolColor)
        If dlgColor.ShowDialog() = DialogResult.OK Then
            gi.PoolColor = cStyleGuide.ColorToInt(dlgColor.Color)
            Me.UpdateRow(iRow)
        End If

    End Sub

#End Region ' Colors

#Region " Admin "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Finds <see cref="StanzaInfo">StanzaInfo</see> with the given name in 
    ''' the local admin.
    ''' </summary>
    ''' <param name="strName">The name of the Stanza to look for.</param>
    ''' <returns>A StanzaInfo instance or Nothing if this could not be found.</returns>
    ''' <remarks>The name search is case independent.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function FindStanzaInfo(ByVal strName As String) As StanzaInfo
        Dim si As StanzaInfo = Nothing
        For i As Integer = 0 To Me.m_lsiStanza.Count - 1
            si = DirectCast(Me.m_lsiStanza(i), StanzaInfo)
            If String.Compare(si.Name, strName, True) = 0 Then Return si
        Next
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the list of stanza names currently in the active list.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Function GetStanzaNames() As String()

        Dim astrStanzaNames(Me.m_lsiStanza.Count) As String
        Dim si As StanzaInfo = Nothing

        astrStanzaNames(0) = sNO_STANZA
        For i As Integer = 0 To Me.m_lsiStanza.Count - 1
            si = DirectCast(Me.m_lsiStanza(i), StanzaInfo)
            astrStanzaNames(i + 1) = si.Name
        Next
        Return astrStanzaNames

    End Function

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Move all Detritus groups to the end of the list
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub FixDetritusOrder()
        Dim iNextDetritusPos As Integer = Me.m_lgiGroups.Count - 1
        Dim gi As GroupInfo = Nothing

        ' Move detritus groups automatically.
        ' First next position to place a detritus group
        gi = DirectCast(Me.m_lgiGroups(iNextDetritusPos), GroupInfo)
        While gi.PP = ePrimaryProductionTypes.Detritus
            iNextDetritusPos -= 1
            gi = DirectCast(Me.m_lgiGroups(iNextDetritusPos), GroupInfo)
        End While

        ' Now scan the list for other detritus groups to move
        For iGroup As Integer = iNextDetritusPos To 0 Step -1
            gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
            ' Is this a detritus group?
            If gi.PP = ePrimaryProductionTypes.Detritus Then
                ' #Yes: move it to the end of the list
                Me.MoveRow(iGroup + iFIRSTGROUPROW, iNextDetritusPos + iFIRSTGROUPROW)
                iNextDetritusPos -= 1
            End If
        Next iGroup
    End Sub

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

        Dim r As SourceGrid2.Range = Me.Selection.Item(0)

        If r.Start.Row = iRow Then Return

        Me.Selection.AddRange(New SourceGrid2.Range(iRow, r.Start.Column, iRow, r.End.Column))
        Me.Selection.RemoveRange(r)

        ' Make sure selected row is visible
        Me.ShowCell(New Position(iRow, 0))

        Me.RaiseSelectionChangeEvent()

    End Sub

#End Region ' Selection extension

#End Region ' Admin

#Region " Validation "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; validates the content of the grid.
    ''' </summary>
    ''' <returns>True when the content of the grid depicts a valid
    ''' group/stanza configuration for a model.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ValidateContent() As Boolean

        Dim bSucces As Boolean = True

        ' Perform all validation
        bSucces = bSucces And Me.ValidateStanzaAssignments()
        bSucces = bSucces And Me.ValidateStanzaAges()
        bSucces = bSucces And Me.ValidateDetritusContent()

        Return bSucces

    End Function

    Private Function ValidateStanzaAssignments() As Boolean

        Dim si As StanzaInfo = Nothing
        Dim gi As GroupInfo = Nothing
        Dim bValid As Boolean = True

        For iStanza As Integer = 0 To Me.m_lsiStanza.Count - 1
            si = Me.m_lsiStanza(iStanza)
            ' Zero groups and somehow not flagged for removal?
            If ((si.NumGroups = 0) And (Me.m_lsiStanzaRemoved.IndexOf(si) = -1)) Then
                ' Remove stanza
                Me.m_lsiStanzaRemoved.Add(si)
            Else
                ' For all groups
                For iGroup As Integer = 0 To si.NumGroups - 1
                    gi = si.GroupList(iGroup)
                    bValid = bValid And (Object.ReferenceEquals(gi.Stanza, si))
                Next iGroup
            End If
        Next iStanza

        Return bValid
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; validates if the grid contains a valid set of stanza 
    ''' age settings.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ValidateStanzaAges() As Boolean

        Dim si As StanzaInfo = Nothing
        Dim gi As GroupInfo = Nothing
        Dim giPrev As GroupInfo = Nothing
        Dim strPrompt As String = ""

        For iStanza As Integer = 0 To Me.m_lsiStanza.Count - 1

            si = Me.m_lsiStanza(iStanza)
            giPrev = Nothing

            si.Sort()

            For iGroup As Integer = 0 To si.GroupList().Count - 1
                gi = si.GroupList(iGroup)
                If giPrev Is Nothing Then
                    ' Youngest group must have age 0
                    If gi.StanzaAge <> 0 Then
                        gi.StanzaAge = 0
                        strPrompt = String.Format(My.Resources.ECOPATH_EDITGROUPSSTANZA_STANZAAGECORRECTED, si.Name)
                        MsgBox(strPrompt, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly)
                        Me.UpdateStanzaColumns()
                    End If
                Else
                    ' Cannot have two groups of the same age within a stanza
                    If gi.StanzaAge = giPrev.StanzaAge Then
                        strPrompt = String.Format(My.Resources.ECOPATH_EDITGROUPSSTANZA_AGECONFLICT, giPrev.Name, gi.Name, si.Name)
                        MsgBox(strPrompt, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                        Return False
                    End If
                End If
                giPrev = gi
            Next iGroup

        Next iStanza
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; validates if the grid contains a valid set of detritus
    ''' settings for a model.
    ''' </summary>
    ''' <returns>True if valid.</returns>
    ''' <remarks>
    ''' <para>This method validates the following conditions:</para>
    ''' <list type="bullet">
    ''' <item><description>There must be at least ONE detritus group;</description></item>
    ''' <item><description>All detritus groups must be located at the end of the group list.
    ''' If this is not the case, the user will be prompted whether the ditritus groups may
    ''' be moved automatically.</description></item>
    ''' </list>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function ValidateDetritusContent() As Boolean

        Dim nDetritus As Integer = 0
        Dim bAllDetritusAtEnd As Boolean = True
        Dim gi As GroupInfo = Nothing
        Dim msg As cMessage = Nothing

        ' Check detritus config
        For iGroup As Integer = 0 To Me.m_lgiGroups.Count - 1
            gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
            ' Is this a detritus group?
            If gi.PP = ePrimaryProductionTypes.Detritus Then
                ' #Yes: count it
                nDetritus += 1
            Else
                ' #No. Check if there were any detritus groups encountered previously.
                ' If so, then not all detritus groups are positioned at the end of the list.
                If nDetritus > 0 Then
                    ' #Yes: this means 
                    bAllDetritusAtEnd = False
                End If
            End If
        Next iGroup

        ' Process results
        ' 1. Must be at least 1 detritus group found
        If nDetritus < 1 Then
            ' Display warning that requires user action
            MsgBox(My.Resources.ECOPATH_EDITGROUPSSTANZA_NEEDDETRITUS, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly Or MsgBoxStyle.Exclamation)
            Return False
        End If

        ' 2. All detritus groups must be at the end of the list
        If Not bAllDetritusAtEnd Then
            ' Display information message to the user, allow intervention
            If MsgBox(My.Resources.PROMPT_EDITGROUPSSTANZA_WILLMOVEDETRITUSTOEND, _
                    MsgBoxStyle.OkCancel Or MsgBoxStyle.Information) <> MsgBoxResult.Ok Then
                Return False
            End If
            ' Fix it
            Me.FixDetritusOrder()
        End If

        ' Detritus configuration is valid
        Return True
    End Function

    Private Function IsNameUnique(ByVal strName As String, ByVal gi As GroupInfo) As Boolean

        ' Check if name is unique
        For iGroup As Integer = 0 To Me.m_lgiGroups.Count - 1
            Dim giTemp As GroupInfo = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
            ' Does name already exist?
            If (Not Object.ReferenceEquals(giTemp, gi)) And (String.Compare(strName, giTemp.Name, True) = 0) Then
                ' Report failure
                Return False
            End If
        Next
        Return True

    End Function

#End Region ' Validation

#Region " Apply changes "

    Public Function Apply() As Boolean

        ' ToDo: globalize this method

        Dim strPrompt As String = ""
        Dim bConfigurationChanged As Boolean = False
        Dim bGroupsChanged As Boolean = False
        Dim bStanzaChanged As Boolean = False
        Dim gi As GroupInfo = Nothing
        Dim group As cEcoPathGroupInput = Nothing
        Dim iGroup As Integer = 0
        Dim si As StanzaInfo = Nothing
        Dim stanza As cStanzaGroup = Nothing
        Dim iStanza As Integer = 0
        Dim bSuccess As Boolean = True
        Dim sb As New System.Text.StringBuilder

        ' =================
        ' Validation
        ' =================

        ' Validate content of the grid
        If Not Me.ValidateContent() Then Return False

        ' =================
        ' Change assessment
        ' =================

        ' Assess group changes
        For iGroup = 0 To Me.m_lgiGroups.Count - 1
            gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
            ' Check this group is newly added
            If gi.IsNew() Then
                ' Yes, config has changed
                bConfigurationChanged = True
            Else
                ' Check if this existing group has moved
                bConfigurationChanged = bConfigurationChanged Or ((iGroup + 1) <> gi.Group.Index)
                ' Check if this exisitng group has been modified
                bGroupsChanged = bGroupsChanged Or gi.IsChanged()
            End If
        Next iGroup

        ' Assess groups to remove
        For iGroup = 0 To Me.m_lgiGroupsRemoved.Count - 1
            gi = DirectCast(Me.m_lgiGroupsRemoved(iGroup), GroupInfo)
            ' Only prompt for removal of pre-existing groups
            If (Not gi.IsNew()) Then

                strPrompt = String.Format(My.Resources.ECOPATH_EDITGROUPSSTANZA_CONFIRMGROUPDELETE_PROMPT, gi.Name)

                Select Case MsgBox(strPrompt, MsgBoxStyle.Question Or MsgBoxStyle.YesNoCancel)
                    Case MsgBoxResult.Cancel
                        ' Abort Apply process
                        Return False
                    Case MsgBoxResult.No
                        ' Do not delete this group
                        gi.Confirmed = False
                    Case MsgBoxResult.Yes
                        ' Delete this group
                        gi.Confirmed = True
                        bConfigurationChanged = True
                    Case Else
                        ' Unexpected anwer: assert
                        Debug.Assert(False)
                End Select
            End If
        Next iGroup

        ' Assess stanza changes
        For iStanza = 0 To Me.m_lsiStanza.Count - 1
            si = DirectCast(Me.m_lsiStanza(iStanza), StanzaInfo)
            bConfigurationChanged = bConfigurationChanged Or si.IsNew()
            bConfigurationChanged = bConfigurationChanged Or si.IsChanged(Me.Core)
        Next iStanza

        ' Assess stanza to remove
        ' JS14Apr07: Stanza are deleted when groups no longer use them. There will be no delete confirmation prompt
        '            on Stanza groups.
        For iStanza = 0 To Me.m_lsiStanzaRemoved.Count - 1
            si = DirectCast(Me.m_lsiStanzaRemoved(iStanza), StanzaInfo)
            bConfigurationChanged = bConfigurationChanged Or (Not si.IsNew())
        Next iStanza

        ' =================
        ' Application
        ' =================

        cApplicationStatusNotifier.SetStatusText(My.Resources.STATUS_APPLYVALUES, TriState.True)

        ' Handle added and removed items
        If (bConfigurationChanged) Then

            If Not Me.Core.SetBatchLock(cCore.eBatchLockType.Restructure) Then
                cApplicationStatusNotifier.SetStatusText("", TriState.False)
                Return False
            End If

            Dim htGroupID As New Dictionary(Of GroupInfo, Integer)
            Dim iDBID As Integer = Nothing

            ' Add new groups
            iGroup = 0
            While (bSuccess = True) And (iGroup < Me.m_lgiGroups.Count)
                gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
                If (gi.IsNew()) Then
                    Dim igt As Integer = iGroup + 1
                    bSuccess = bSuccess And Me.Core.AddGroup(gi.Name, gi.PP, gi.VBK, igt, iDBID)
                    Debug.Assert(igt = iGroup + 1)
                    ' Map this new ID during update
                    htGroupID.Add(gi, iDBID)
                Else
                    If ((iGroup + 1) <> gi.Group.Index) Then
                        If Not Me.Core.MoveGroup(gi.Group.Index, iGroup + 1) Then
                            sb.AppendLine("Failed to move group " & gi.Name)
                            bSuccess = False
                        End If
                    End If
                End If
                iGroup += 1
            End While

            ' Remove deleted (and confirmed) groups
            Dim agi() As GroupInfo = Me.m_lgiGroupsRemoved.ToArray()
            iGroup = 0
            While (bSuccess = True) And (iGroup < agi.Length)
                gi = agi(iGroup)
                If (Not gi.IsNew()) And (gi.Confirmed = True) Then
                    If (Me.Core.RemoveGroup(gi.Group.Index)) Then
                        Me.m_lgiGroups.Remove(gi)
                        Me.m_lgiGroupsRemoved.Remove(gi)
                    Else
                        sb.AppendLine("Failed to delete group " & gi.Name)
                        bSuccess = False
                    End If
                End If
                iGroup += 1
            End While

            ' Remove deleted stanza configurations from internal admin
            Dim asiRemove() As StanzaInfo = Me.m_lsiStanzaRemoved.ToArray()
            iStanza = 0
            While (bSuccess = True) And (iStanza < asiRemove.Length)
                si = asiRemove(iStanza)
                If (Not si.IsNew()) Then
                    If Me.Core.RemoveStanza(si.StanzaGroup.Index) = True Then
                        Me.m_lsiStanza.Remove(si)
                        Me.m_lsiStanzaRemoved.Remove(si)
                    Else
                        sb.AppendLine("Failed to delete stanza configuration " & si.Name)
                        bSuccess = False
                    End If
                End If
                iStanza += 1
            End While

            ' Add new stanza configurations
            iStanza = 0
            While (bSuccess = True) And (iStanza < Me.m_lsiStanza.Count)
                si = DirectCast(Me.m_lsiStanza(iStanza), StanzaInfo)
                If (si.IsNew()) Then
                    Dim iStanzaID As Integer = -1
                    Dim aiGroupID() As Integer
                    Dim aiStartAge() As Integer

                    ReDim aiGroupID(si.GroupList.Count - 1)
                    ReDim aiStartAge(si.GroupList.Count - 1)

                    For i As Integer = 0 To si.GroupList.Count - 1
                        ' Get the first group
                        gi = si.GroupList(i)
                        ' Is an existing group?
                        If gi.Group IsNot Nothing Then
                            iDBID = CInt(gi.Group.GetVariable(eVarNameFlags.DBID))
                        Else
                            iDBID = htGroupID(gi)
                        End If
                        aiGroupID(i) = iDBID
                        aiStartAge(i) = gi.StanzaAge
                    Next
                    If Not Me.Core.AppendStanza(si.Name, aiGroupID, aiStartAge, iStanzaID) Then
                        sb.AppendLine("Failed to add stanza configuration " & si.Name)
                        bSuccess = False
                    End If
                End If
                iStanza += 1
            End While

            ' Update modified stanza configurations
            iStanza = 0
            While (bSuccess = True) And (iStanza < Me.m_lsiStanza.Count)
                si = DirectCast(Me.m_lsiStanza(iStanza), StanzaInfo)
                If (Not si.IsNew()) Then
                    If si.IsChanged(Me.Core) Then
                        Dim sg As cStanzaGroup = si.StanzaGroup
                        ' Remove all current groups
                        For iLifestage As Integer = 1 To si.StanzaGroup.NStanzas
                            group = Me.Core.EcoPathGroupInputs(sg.iGroups(iLifestage))
                            If Not Me.Core.RemoveStanzaLifestage(sg.Index, CInt(group.GetVariable(eVarNameFlags.DBID))) Then
                                bSuccess = False
                            End If
                        Next
                        ' Add newly assigned groups
                        For iLifestage As Integer = 0 To si.NumGroups - 1
                            gi = si.GroupList(iLifestage)
                            If gi.Group IsNot Nothing Then
                                iDBID = CInt(gi.Group.GetVariable(eVarNameFlags.DBID))
                            Else
                                iDBID = htGroupID(gi)
                            End If
                            If Not Me.Core.AddStanzaLifestage(si.StanzaGroup.Index, iDBID, gi.StanzaAge, gi.StanzaMortality) Then
                                bSuccess = False
                            End If
                        Next

                        If bSuccess = False Then
                            sb.AppendLine("Failed to update stanza '" & si.Name & "' life stages")
                        End If
                    End If
                End If
                iStanza += 1
            End While

            ' The core will reload now
            Try
                Me.Core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.Ecopath, bSuccess)
            Catch ex As Exception
                ' this is to catch a core assertion when SumB=0
            End Try

        End If

        If Not bSuccess Then
            MsgBox(sb.ToString, MsgBoxStyle.Exclamation And MsgBoxStyle.OkOnly)
        End If

        ' Update core objects when previous operations were succesful
        If (bSuccess And bGroupsChanged) Then
            Dim bColorsChanged As Boolean = False
            For iGroup = 0 To Me.m_lgiGroups.Count - 1
                gi = DirectCast(Me.m_lgiGroups(iGroup), GroupInfo)
                If gi.IsChanged() Then
                    ' Find groups by ID; the core has reloaded
                    For iGrpTmp As Integer = 1 To Me.Core.nGroups
                        group = Me.Core.EcoPathGroupInputs(iGrpTmp)
                        If group.getID = gi.Group.getID Then
                            If (group.Name <> gi.Name) Then group.Name = gi.Name
                            If (group.PP <> gi.PP) Then group.PP = gi.PP
                            If (group.VBK <> gi.VBK) Then group.VBK = gi.VBK
                            If (group.PoolColor <> gi.PoolColor) Then
                                ' Is gi.poolcolor the default color? 
                                If gi.PoolColor = cStyleGuide.ColorToInt(Me.StyleGuide.GroupColorDefault(iGrpTmp, Me.m_lgiGroups.Count)) Then
                                    ' #Yes: Set color to transparent to allow group to show up as true default colour
                                    group.PoolColor = 0
                                Else
                                    ' #No: Assign new color
                                    group.PoolColor = gi.PoolColor
                                End If
                                bColorsChanged = True
                            End If
                            Exit For
                        End If
                    Next
                End If
            Next
            If bColorsChanged Then Me.StyleGuide.ColorsChanged()
        End If

        cApplicationStatusNotifier.SetStatusText("", TriState.False)

        Return bSuccess

    End Function

#End Region ' Apply changes

End Class
