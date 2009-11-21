#Region " Imports "

Option Strict On

Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports System.ComponentModel

#End Region ' Imports

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Listbox devived class meant for showing colour-coded Ecopath groups.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cGroupListBox
        Inherits ListBox

#Region " Public enums "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, indicates sort styles for a cGroupListBox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eSortType As Byte
            GroupIndexAsc = 0
            GroupIndexDesc
            GroupNameAsc
            GroupNameDesc
            ValueAsc
            ValueDesc
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, indicates which groups are automatically added
        ''' to a cGroupListBox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eGroupTrackingType As Integer
            ''' <summary>Groups are added manually, not automatically by tracking the style guide.</summary>
            Manual = 0
            ''' <summary>Only living groups are added, and tracked by the style guide visibility settings.</summary>
            LivingGroups
            ''' <summary>Only detritus groups are added, and tracked by the style guide visibility settings.</summary>
            DetritusGroups
            ''' <summary>All groups are added, and tracked by the style guide visibility settings.</summary>
            AllGroups
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, indicates how groups are displayed according to
        ''' StyleGuide visibility settings.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eGroupDisplayStyleTypes As Integer
            ''' <summary>Always show groups.</summary>
            DisplayAlways = 0
            ''' <summary>Show styleguide hidden groups as 'hidden'.</summary>
            DisplayAsHidden
            ''' <summary>Do not show styleguide hidden groups.</summary>
            DisplayVisibleOnly
        End Enum

#End Region ' Public enums

#Region " cGroupItem "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' An item for a cGroupListBox
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Class cGroupItem

            ''' <summary>Group to show.</summary>
            Private m_group As cCoreGroupBase = Nothing
            ''' <summary>A value to sort by.</summary>
            Private m_sValue As Single = 0.0

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Creates a new item for usage in the GroupListBox.
            ''' </summary>
            ''' <param name="group">Group to link to.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal group As cCoreGroupBase)
                Me.m_group = group
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Formats the item text for display in the GroupListBox.
            ''' </summary>
            ''' <returns>The formatted item text.</returns>
            ''' ---------------------------------------------------------------
            Public Overrides Function ToString() As String
                If Me.m_group IsNot Nothing Then Return Me.m_group.Name
                Return My.Resources.GENERIC_VALUE_ALL
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the group linked to the item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Group() As cCoreGroupBase
                Get
                    Return Me.m_group
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get/set the sort value for this item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Property SortValue() As Single
                Get
                    Return Me.m_sValue
                End Get
                Set(ByVal sSortValue As Single)
                    Me.m_sValue = sSortValue
                End Set
            End Property

        End Class

#End Region ' cGroupItem

#Region " Privates "

        Private m_core As cCore = Nothing
        Private m_sg As cStyleGuide = Nothing
        Private m_sortType As eSortType = eSortType.GroupIndexAsc
        Private m_sSortThreshold As Single = cCore.NULL_VALUE
        Private m_bShowAllItem As Boolean = True
        Private m_groupdisplaystyle As eGroupDisplayStyleTypes = eGroupDisplayStyleTypes.DisplayAlways
        Private m_grouptrackingtype As eGroupTrackingType = eGroupTrackingType.AllGroups

#End Region ' Privates

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new cGroupListBox
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            ' This box draws its own items
            Me.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Thrash me.
        ''' </summary>
        ''' <param name="bDisposing"></param>
        ''' ---------------------------------------------------------------
        Protected Overrides Sub Dispose(ByVal bDisposing As Boolean)
            MyBase.Dispose(bDisposing)
            Me.Detach()
        End Sub

#Region " Attach / detach "

        Public Sub Attach(ByVal core As cCore, ByVal sg As cStyleGuide)

            ' Connect
            Me.m_core = core
            Me.m_sg = sg

            AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
            Me.Populate()

        End Sub

        Public Sub Detach()
            If (Me.IsInitialized()) Then
                RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
                Me.m_sg = Nothing
                Me.m_core = Nothing
            End If

        End Sub

#End Region ' Attach / detach

#Region " Sorting "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get/set how to sort the data in this list box.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(True), _
         Description("The EwE6 sort method to use"), _
         Category("EwE6"), _
         DefaultValue(eSortType.GroupIndexAsc)> _
       Public Property SortType() As eSortType
            Get
                Return Me.m_sortType
            End Get
            Set(ByVal sortType As eSortType)
                If (Me.m_sortType <> sortType) Then
                    Me.m_sortType = sortType
                    Me.Refresh()
                End If
            End Set
        End Property

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value that sort values have to exceed.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(True), _
        Description("The EwE6 sort threshold value to use"), _
        Category("EwE6"), _
        DefaultValue(cCore.NULL_VALUE)> _
      Public Property SortThreshold() As Single
            Get
                Return Me.m_sSortThreshold
            End Get
            Set(ByVal sSortThreshold As Single)
                If (Me.m_sSortThreshold <> sSortThreshold) Then
                    Me.m_sSortThreshold = sSortThreshold
                    Me.Refresh()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a sort value for a given group.
        ''' </summary>
        ''' <param name="group">The group to get/set the sort value for.</param>
        ''' -------------------------------------------------------------------
        Public Property SortValue(ByVal group As cCoreGroupBase) As Single
            Get
                Dim gi As cGroupItem = Me.GroupItem(group)
                If (gi Is Nothing) Then Return cCore.NULL_VALUE
                Return gi.SortValue
            End Get
            Set(ByVal value As Single)
                Dim gi As cGroupItem = Me.GroupItem(group)
                If (gi IsNot Nothing) Then
                    gi.SortValue = value
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a sort value for a given group index.
        ''' </summary>
        ''' <param name="iGroup">The group index to get/set the sort value for.</param>
        ''' -------------------------------------------------------------------
        Public Property SortValue(ByVal iGroup As Integer) As Single
            Get
                Dim gi As cGroupItem = Me.GroupItem(iGroup)
                If (gi Is Nothing) Then Return cCore.NULL_VALUE
                Return gi.SortValue
            End Get
            Set(ByVal value As Single)
                Dim gi As cGroupItem = Me.GroupItem(iGroup)
                If (gi IsNot Nothing) Then
                    gi.SortValue = value
                End If
            End Set
        End Property

#End Region ' Sorting

#Region " Behaviour "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set group styleguide visibility tracking.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Description("State how to reflect styleguide hidden groups"), _
         Category("EwE6"), _
         DefaultValue(eGroupDisplayStyleTypes.DisplayAlways)> _
        Public Property GroupDisplayStyle() As eGroupDisplayStyleTypes
            Get
                Return Me.m_groupdisplaystyle
            End Get
            Set(ByVal value As eGroupDisplayStyleTypes)
                If value <> Me.m_groupdisplaystyle Then
                    Me.m_groupdisplaystyle = value
                    Me.Populate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the listbox should include an 'all groups' item.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Description("Include an 'all groups' item"), _
         Category("EwE6"), _
         DefaultValue(True)> _
        Public Property ShowAllGroupsItem() As Boolean
            Get
                Return Me.m_bShowAllItem
            End Get
            Set(ByVal value As Boolean)
                If value <> Me.m_bShowAllItem Then
                    Me.m_bShowAllItem = value
                    Me.Populate()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether non-living groups should be shown in the list box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Description("Determine how groups are tracked and added"), _
         Category("EwE6"), _
         DefaultValue(eGroupTrackingType.AllGroups)> _
        Public Property GroupListTracking() As eGroupTrackingType
            Get
                Return Me.m_grouptrackingtype
            End Get
            Set(ByVal value As eGroupTrackingType)
                If value <> Me.m_grouptrackingtype Then
                    Me.m_grouptrackingtype = value
                    Me.Populate()
                End If
            End Set
        End Property

#End Region ' Behaviour

#Region " Group / index access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a given group index is selected.
        ''' </summary>
        ''' <param name="iGroup">The one-based group index to test.</param>
        ''' -------------------------------------------------------------------
        Public Property IsGroupSelected(ByVal iGroup As Integer) As Boolean
            Get
                Dim iItem As Integer = Me.GroupIndex(iGroup)
                Return Me.GetSelected(iItem)
            End Get
            Set(ByVal bSelected As Boolean)
                If (Not Me.IsInitialized()) Then Return
                Dim iItem As Integer = Me.GroupIndex(iGroup)
                Me.SetSelected(iItem, bSelected)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a given group is selected.
        ''' </summary>
        ''' <param name="group">The group to test.</param>
        ''' -------------------------------------------------------------------
        Public Property IsGroupSelected(ByVal group As cCoreGroupBase) As Boolean
            Get
                Return Me.IsGroupSelected(group.Index)
            End Get
            Set(ByVal bSelected As Boolean)
                If (Not Me.IsInitialized()) Then Return
                Me.IsGroupSelected(group) = bSelected
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected group index (single selection listboxes only).
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property SelectedGroupIndex() As Integer
            Get
                Dim gi As cGroupItem = Me.GroupItem(Me.SelectedIndex)
                If (gi Is Nothing) Then Return -1
                If (gi.Group Is Nothing) Then Return -1
                Return gi.Group.Index
            End Get
            Set(ByVal iGroup As Integer)
                If (Not Me.IsInitialized()) Then Return
                If (iGroup < 1 Or iGroup >= Me.m_core.nGroups) Then
                    Me.SelectedIndex = CInt(IIf(Me.m_bShowAllItem, 0, -1))
                    Return
                End If
                Me.SelectedIndex = Me.GroupIndex(iGroup)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected group (single selection listboxes only).
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property SelectedGroup() As cCoreGroupBase
            Get
                Dim gi As cGroupItem = DirectCast(Me.SelectedItem, cGroupItem)
                If gi Is Nothing Then Return Nothing
                Return gi.Group
            End Get
            Set(ByVal group As cCoreGroupBase)
                If (Not Me.IsInitialized()) Then Return
                If (group Is Nothing) Then
                    Me.SelectedIndex = CInt(IIf(Me.m_bShowAllItem, 0, -1))
                    Return
                End If
                Me.SelectedIndex = Me.GroupIndex(group.Index)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the listbox item for a given group index.
        ''' </summary>
        ''' <param name="iGroup">The group index to translate into an item index.</param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupIndex(ByVal iGroup As Integer) As Integer
            Get
                Dim gi As cGroupItem = Nothing
                Dim item As Object = Nothing
                Dim group As cCoreGroupBase = Nothing

                For i As Integer = 0 To Me.Items.Count - 1
                    item = Me.Items(i)
                    gi = Me.GroupItem(i)
                    If (gi IsNot Nothing) Then
                        group = gi.Group
                        If (group IsNot Nothing) Then
                            If (group.Index = iGroup) Then
                                Return i
                            End If
                        End If
                    End If
                Next
                Return -1
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the listbox item for a given group.
        ''' </summary>
        ''' <param name="group">The group to translate into an item index.</param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GroupIndex(ByVal group As cCoreGroupBase) As Integer
            Get
                Return Me.GroupIndex(group.Index)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the group at a given listbox item index.
        ''' </summary>
        ''' <param name="iIndex">The index to return the group for.</param>
        ''' <remarks>
        ''' Returns Nothing if no group was found at the given index.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GetGroupAt(ByVal iIndex As Integer) As cCoreGroupBase
            Get
                If (iIndex <= 0 Or iIndex >= Me.Items.Count) Then Return Nothing
                Return DirectCast(Me.Items(iIndex), cGroupItem).Group
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the index of a group at a given listbox item index.
        ''' </summary>
        ''' <param name="iIndex">The index to return the group index for.</param>
        ''' <remarks>
        ''' Returns <see cref="cCore.NULL_VALUE">core NULL</see> if no group was
        ''' found at the given index.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GetGroupIndexAt(ByVal iIndex As Integer) As Integer
            Get
                Dim gi As cGroupItem = Me.GroupItem(iIndex)

                If gi Is Nothing Then Return cCore.NULL_VALUE
                If gi.Group Is Nothing Then Return cCore.NULL_VALUE
                Return gi.Group.Index

            End Get
        End Property

#End Region ' Group / index access

#Region " Refresh "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Sort (if applicable) and redraw the listbox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Refresh()
            If Me.Sorted Then Me.Sort()
            MyBase.Refresh()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Populate the listbox
        ''' </summary>
        ''' <param name="aiGroups">
        ''' An optional list of group indexes to populate the listbox with.
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub Populate(Optional ByVal aiGroups() As Integer = Nothing)

            Dim bIncludeGroup As Boolean = False
            Dim iGroupStart As Integer = 1
            Dim iGroupEnd As Integer = 1

            If (Not Me.IsInitialized()) Then Return

            ' Stop automatic tracking if a manual list is provided
            If aiGroups IsNot Nothing Then Me.m_grouptrackingtype = eGroupTrackingType.Manual

            ' ToDo_JS: Preserve group selection
            Me.SuspendLayout()

            ' Clear items
            Me.Items.Clear()

            ' Add 'all groups' item
            If Me.m_bShowAllItem Then
                Me.Items.Add(New cGroupItem(Nothing))
            End If

            ' Determine #groups to show
            Select Case Me.m_grouptrackingtype

                Case eGroupTrackingType.Manual, _
                     eGroupTrackingType.AllGroups
                    iGroupStart = 1 : iGroupEnd = Me.m_core.nGroups

                Case eGroupTrackingType.LivingGroups
                    iGroupStart = 1 : iGroupEnd = Me.m_core.nLivingGroups

                Case eGroupTrackingType.DetritusGroups
                    iGroupEnd = Me.m_core.nLivingGroups + 1 : iGroupEnd = Me.m_core.nGroups

                Case Else
                    Debug.Assert(False, "inclusion type not supported")

            End Select

            ' (Re)populate listbox
            For i As Integer = iGroupStart To iGroupEnd

                ' Hard list given?
                If (aiGroups IsNot Nothing) Then
                    bIncludeGroup = (Array.IndexOf(aiGroups, i) >= 0)
                Else
                    Select Case Me.m_groupdisplaystyle
                        Case eGroupDisplayStyleTypes.DisplayAlways
                            bIncludeGroup = True
                        Case eGroupDisplayStyleTypes.DisplayAsHidden
                            bIncludeGroup = True
                        Case eGroupDisplayStyleTypes.DisplayVisibleOnly
                            bIncludeGroup = Me.m_sg.GroupVisible(i)
                    End Select
                End If

                If (bIncludeGroup = True) Then
                    Me.Items.Add(New cGroupItem(Me.m_core.EcoPathGroupInputs(i)))
                End If
            Next

            Me.ResumeLayout()

        End Sub

#End Region ' Refresh

#Region " Internals "

        Protected Function IsInitialized() As Boolean
            Return (Me.m_core IsNot Nothing) And _
                   (Me.m_sg IsNot Nothing)
        End Function

        Protected Function GroupItem(ByVal iIndex As Integer) As cGroupItem
            If (iIndex >= 0 And iIndex < Me.Items.Count) Then Return DirectCast(Me.Items(iIndex), cGroupItem)
            Return Nothing
        End Function

        Protected Function GroupItem(ByVal group As cCoreGroupBase) As cGroupItem
            If group Is Nothing Then Return Nothing
            Return Me.GroupItem(Me.GroupIndex(group.Index))
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Overridden to render an item
        ''' </summary>
        ''' <param name="e">Event parameters</param>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub OnDrawItem(ByVal e As System.Windows.Forms.DrawItemEventArgs)

            If (e.Index >= Me.Items.Count Or e.Index < 0) Then Return

            Dim item As Object = Me.Items(e.Index)
            Dim gi As cGroupListBox.cGroupItem = Nothing
            Dim clrLegend As Color = Color.Transparent
            Dim clrText As Color = e.ForeColor
            Dim bItemValid As Boolean = True

            ' Attempt to get item colour if it is a cGroupItem
            If (TypeOf item Is cGroupListBox.cGroupItem) Then
                ' Get item group
                gi = DirectCast(item, cGroupListBox.cGroupItem)
                ' Has a group attached?
                If (gi.Group IsNot Nothing) Then
                    ' #Yes: use dimmed colours
                    clrLegend = Me.m_sg.GroupColor(Me.m_core, gi.Group.Index)
                    ' Allowed to display and colour group?
                    If Me.m_sg.GroupVisible(gi.Group.Index) And gi.SortValue >= Me.SortThreshold Then
                        ' #Yes: display group in full color
                        clrText = e.ForeColor
                    Else
                        ' #No: use dimmed text colour
                        clrText = SystemColors.ControlDark
                        bItemValid = False
                    End If
                End If
            End If

            ' TODO: Take current culture into consideration here. Right-to-left reading order cultures
            ' will need the colour box to be displayed on the right-hand side of the text.

            ' Render default background 
            e.DrawBackground()
            ' Render default text, bumped to the right by 22 pixels
            e.Graphics.DrawString(item.ToString(), e.Font, New SolidBrush(clrText), e.Bounds.X + 22, e.Bounds.Y)

            If (clrLegend.A > 0) Then
                ' Render colour fill
                If bItemValid Then
                    Using br As New SolidBrush(clrLegend)
                        e.Graphics.FillRectangle(br, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                    End Using
                Else
                    Using br As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.BackwardDiagonal, clrLegend, Color.Transparent)
                        e.Graphics.FillRectangle(br, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                    End Using
                End If
                ' Render colour box border
                Using p As New Pen(clrText, 1)
                    e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
                End Using
            End If

            ' Render default focus rectangle
            e.DrawFocusRectangle()

        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If ((ct And cStyleGuide.eChangeType.GroupVisibility) > 0) Then
                Select Case Me.GroupDisplayStyle
                    Case eGroupDisplayStyleTypes.DisplayAlways
                        Return
                    Case eGroupDisplayStyleTypes.DisplayAsHidden
                        Me.Invalidate()
                    Case eGroupDisplayStyleTypes.DisplayVisibleOnly
                        Me.Populate()
                    Case Else
                        Debug.Assert(False)
                End Select
            End If
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Sort me!
        ''' </summary>
        ''' ---------------------------------------------------------------
        Protected Overrides Sub Sort()

            Dim objSwap As Object = Nothing
            Dim iCounter As Integer = 0
            Dim bSwapped As Boolean = False

            If (Items.Count > 1) Then
                ' Bubble away
                Do
                    ' Reset bubble loop
                    iCounter = Items.Count - 1
                    bSwapped = False
                    ' Bubble deeper
                    While ((iCounter - 1) > 0)

                        ' Need to swap items based on current sort order?
                        If Me.Compare(Items(iCounter - 1), Items(iCounter)) = 1 Then
                            ' #Yes: swap the items.
                            objSwap = Items(iCounter)
                            Items(iCounter) = Items(iCounter - 1)
                            Items(iCounter - 1) = objSwap
                            bSwapped = True
                        End If
                        ' Decrement the counter.
                        iCounter -= 1
                    End While
                Loop While (bSwapped = True)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Compare two items in the listbox for sorting.
        ''' </summary>
        ''' <param name="i1"></param>
        ''' <param name="i2"></param>
        ''' <returns>
        ''' <list>
        ''' <item><description>-1 if i1 is less than i2</description></item>
        ''' <item><description>0 if i1 equals i2</description></item>
        ''' <item><description>1 if i1 is greater than i2</description></item>
        ''' </list>
        ''' </returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function Compare(ByVal i1 As Object, ByVal i2 As Object) As Integer
            Dim gi1 As cGroupItem = Nothing
            Dim group1 As cCoreGroupBase = Nothing
            Dim gi2 As cGroupItem = Nothing
            Dim group2 As cCoreGroupBase = Nothing

            ' Get sortable items
            If TypeOf (i1) Is cGroupItem Then gi1 = DirectCast(i1, cGroupItem) : group1 = gi1.Group
            If TypeOf (i2) Is cGroupItem Then gi2 = DirectCast(i2, cGroupItem) : group2 = gi2.Group

            ' Weed out any incompatible item comparisons
            If (gi1 Is Nothing) Then
                If (gi2 Is Nothing) Then
                    ' Not sortable
                    Return 0
                Else
                    ' Non-group item sorts before group item
                    Return 1
                End If
            Else
                If (gi2 Is Nothing) Then
                    ' Non-group item sorts before group item
                    Return -1
                End If
            End If

            ' Ok, two cGroupItems to compare
            ' Do both have groups attached?
            If (group1 Is Nothing) Then
                If (group2 Is Nothing) Then
                    ' Not sortable
                    Return 0
                Else
                    ' Non-group item sorts before group item
                    Return 1
                End If
            Else
                If (group2 Is Nothing) Then
                    ' Non-group item sorts before group item
                    Return -1
                End If
            End If

            ' Ok, we have two valid groups to compare!
            Select Case Me.m_sortType

                Case eSortType.GroupIndexAsc
                    If group1.Index < group2.Index Then Return -1
                    If group1.Index = group2.Index Then Return 0
                    Return 1

                Case eSortType.GroupIndexDesc
                    If group1.Index > group2.Index Then Return -1
                    If group1.Index = group2.Index Then Return 0
                    Return 1

                Case eSortType.GroupNameAsc
                    Return String.Compare(group1.Name, group2.Name)

                Case eSortType.GroupNameDesc
                    Return String.Compare(group2.Name, group1.Name)

                Case eSortType.ValueAsc
                    If gi1.SortValue < gi2.SortValue Then Return -1
                    If gi1.SortValue = gi2.SortValue Then Return 0
                    Return 1

                Case eSortType.ValueDesc
                    If gi1.SortValue > gi2.SortValue Then Return -1
                    If gi1.SortValue = gi2.SortValue Then Return 0
                    Return 1

            End Select

            Return 0

        End Function

#End Region ' Internals

    End Class

End Namespace
