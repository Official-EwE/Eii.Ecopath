'==============================================================================
'
' $Log: GroupListBox.vb,v $
' Revision 1.7  2009/05/20 16:39:52  jeroens
' Added smart group selection interfaces
'
' Revision 1.6  2009/04/09 01:34:22  jeroens
' Not sorted by default
'
' Revision 1.5  2009/04/08 17:42:28  jeroens
' Optimized sort behaviour
' Overridden Refresh()
' Items below sort threshold are rendered with hatched legend
'
' Revision 1.4  2009/04/08 16:01:52  jeroens
' Made sortable
'
' Revision 1.3  2009/04/08 15:09:10  jeroens
' GroupListBox -> cGroupListBox
'
' Revision 1.2  2009/03/23 18:44:46  jeroens
' Localized
'
' Revision 1.1  2009/03/19 16:54:10  jeroens
' Moved
'
' Revision 1.4  2008/12/15 15:37:28  jeroens
' no message
'
' Revision 1.3  2008/12/04 06:35:29  sherman
' Fixed Show/Hide refresh bug
' Fixed disposed bug
'
' Revision 1.2  2008/11/29 19:00:11  sherman
' Updated bugs and rescaling in RunEcosim plot
'
' Revision 1.1  2008/09/26 07:31:17  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On

Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style

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
        ''' Supported sort styles for a cGroupListBox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eSortType As Byte
            GroupIndexAsc = 0
            GroupIndexDesc
            GroupNameAsc
            GroupNameDesc
            ValueAsc
            ValueDesc
            [Default] = GroupIndexAsc
        End Enum

#End Region ' Public enums

#Region " cGroupItem "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' An item for a cGroupListBox
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Class cGroupItem

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
        Private m_sg As StyleGuide = Nothing
        Private m_sortType As eSortType = eSortType.Default
        Private m_sSortThreshold As Single = cCore.NULL_VALUE

#End Region ' Privates

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Creates a new cGroupListBox
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()

            ' Connect
            Me.m_core = cCore.GetInstance()
            Me.m_sg = StyleGuide.GetInstance()
            AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

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

            If (Me.m_core IsNot Nothing) Then
                RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
                Me.m_sg = Nothing
                Me.m_core = Nothing
            End If
        End Sub

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get/set how to sort the data in this list box.
        ''' </summary>
        ''' ---------------------------------------------------------------
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

        Public Overrides Sub Refresh()
            If Me.Sorted Then Me.Sort()
            MyBase.Refresh()
        End Sub

        Public Function GetGroupSelected(ByVal iGroup As Integer) As Boolean
            Dim iItem As Integer = Me.GroupIndex(iGroup)
            Return Me.GetSelected(iItem)
        End Function

        Public Sub SetGroupSelected(ByVal iGroup As Integer, ByVal bSelected As Boolean)
            Dim iItem As Integer = Me.GroupIndex(iGroup)
            Me.SetSelected(iItem, bSelected)
        End Sub

        Public Function GetGroupSelected(ByVal group As cCoreGroupBase) As Boolean
            Return Me.GetGroupSelected(group.Index)
        End Function

        Public Sub SetGroupSelected(ByVal group As cCoreGroupBase, ByVal bSelected As Boolean)
            Me.SetGroupSelected(group.Index, bSelected)
        End Sub

        Public ReadOnly Property GroupIndex(ByVal iGroup As Integer) As Integer
            Get
                Dim gi As cGroupItem = Nothing
                Dim item As Object = Nothing
                Dim group As cCoreGroupBase = Nothing

                For i As Integer = 0 To Me.Items.Count - 1
                    item = Me.Items(i)
                    If (TypeOf (item) Is cGroupItem) Then
                        group = DirectCast(item, cGroupItem).Group
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

        Public ReadOnly Property GroupIndex(ByVal group As cCoreGroupBase) As Integer
            Get
                Return Me.GroupIndex(group.Index)
            End Get
        End Property

#Region " Internals "

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

        Private Sub OnStyleGuideChanged(ByVal ct As StyleGuide.eChangeType)
            Me.Invalidate()
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
