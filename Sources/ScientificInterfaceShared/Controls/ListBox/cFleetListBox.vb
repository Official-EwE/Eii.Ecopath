#Region " Imports "

Option Strict On

Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports System.ComponentModel
Imports EwEUtils.Commands

#End Region ' Imports

Namespace Controls

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Listbox devived class meant for showing colour-coded Ecopath fleets.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cFleetListBox
        Inherits ListBox
        Implements IUIElement

#Region " Public enums "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, indicates sort styles for a cFleetListBox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eSortType As Byte
            FleetIndexAsc = 0
            FleetIndexDesc
            FleetNameAsc
            FleetNameDesc
            ValueAsc
            ValueDesc
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated type, indicates which fleets are automatically added
        ''' to a cFleetListBox.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Enum eFleetTrackingType As Integer

            Manual
            ''' <summary>Only fleets, except for the all fleet.</summary>
            Fleets
            ''' <summary>All fleets are added.</summary>
            AllFleets
        End Enum

#End Region ' Public enums

#Region " cFleetItem "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' An item for a cFleetListBox
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Protected Class cFleetItem
            Inherits cCoreInputOutputListboxItem

            ''' <summary>A value to sort by.</summary>
            Private m_sValue As Single = 0.0

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Creates a new item for usage in the list box.
            ''' </summary>
            ''' <param name="fleet">Fleet to link to.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal fleet As cFleetInput)
                MyBase.New(fleet)
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Formats the item text for display in the FleetListBox.
            ''' </summary>
            ''' <returns>The formatted item text.</returns>
            ''' ---------------------------------------------------------------
            Public Overrides Function ToString() As String
                Return String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, Me.Source.Index, Me.Source.Name)
            End Function

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Get the fleet linked to the item.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public Shadows ReadOnly Property Source() As cFleetInput
                Get
                    Return DirectCast(MyBase.Source, cFleetInput)
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

#End Region ' cFleetItem

#Region " Privates "

        Private m_uic As cUIContext = Nothing
        Private m_sortType As eSortType = eSortType.FleetIndexAsc
        Private m_sSortThreshold As Single = cCore.NULL_VALUE
        Private m_fleettrackingtype As eFleetTrackingType = eFleetTrackingType.AllFleets

#End Region ' Privates

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of a cFleetListBox.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New()
            ' This box draws its own items
            Me.DrawMode = DrawMode.OwnerDrawFixed
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Destroys a cFleetListBox.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Me.Detach()
            MyBase.Dispose(disposing)
        End Sub

#End Region ' Construction / destruction

#Region " Interface "

        Public Property UIContext() As cUIContext Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Private Set(ByVal uic As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                End If
                Me.m_uic = uic
                If (Me.m_uic IsNot Nothing) Then
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
                End If
            End Set
        End Property

#End Region ' Interface

#Region " Attach / detach "

        Public Sub Attach(ByVal uic As cUIContext)

            ' Sanity check
            Debug.Assert(uic IsNot Nothing)
            Debug.Assert(Not Me.IsInitialized())

            Me.UIContext = uic
            Me.Populate()

        End Sub

        Public Sub Detach()

            If (Me.IsInitialized()) Then
                Me.UIContext = Nothing
                Me.Items.Clear()
            End If

        End Sub

#End Region ' Attach / detach

#Region " Overrides "

        Protected Overrides Sub OnMouseDoubleClick(ByVal e As MouseEventArgs)

            If Not Me.IsInitialized() Then Return

            Dim cmd As cCommand = Nothing
            cmd = Me.m_uic.CommandHandler.GetCommand("EditFleets")

            If cmd Is Nothing Then
                MyBase.OnMouseDoubleClick(e)
            Else
                cmd.Tag = Me.SelectedFleet
                cmd.Invoke()
            End If

        End Sub

#End Region ' Overrides

#Region " Sorting "

        ''' ---------------------------------------------------------------
        ''' <summary>
        ''' Get/set how to sort the data in this list box.
        ''' </summary>
        ''' ---------------------------------------------------------------
        <Browsable(True), _
         Description("The EwE6 sort method to use"), _
         Category("EwE6"), _
         DefaultValue(eSortType.FleetIndexAsc)> _
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
        ''' Get/set a sort value for a given fleet.
        ''' </summary>
        ''' <param name="fleet">The fleet to get/set the sort value for.</param>
        ''' -------------------------------------------------------------------
        Public Property SortValue(ByVal fleet As cFleetInput) As Single
            Get
                Dim gi As cFleetItem = Me.FleetItem(fleet)
                If (gi Is Nothing) Then Return cCore.NULL_VALUE
                Return gi.SortValue
            End Get
            Set(ByVal value As Single)
                Dim gi As cFleetItem = Me.FleetItem(fleet)
                If (gi IsNot Nothing) Then
                    gi.SortValue = value
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a sort value for a given fleet index.
        ''' </summary>
        ''' <param name="iFleet">The fleet index to get/set the sort value for.</param>
        ''' -------------------------------------------------------------------
        Public Property SortValue(ByVal iFleet As Integer) As Single
            Get
                Dim gi As cFleetItem = Me.FleetItem(iFleet)
                If (gi Is Nothing) Then Return cCore.NULL_VALUE
                Return gi.SortValue
            End Get
            Set(ByVal value As Single)
                Dim gi As cFleetItem = Me.FleetItem(iFleet)
                If (gi IsNot Nothing) Then
                    gi.SortValue = value
                End If
            End Set
        End Property

#End Region ' Sorting

#Region " Behaviour "


        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether non-living fleets should be shown in the list box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True), _
         Description("Determine how fleets are tracked and added"), _
         Category("EwE6"), _
         DefaultValue(eFleetTrackingType.AllFleets)> _
        Public Property FleetListTracking() As eFleetTrackingType
            Get
                Return Me.m_fleettrackingtype
            End Get
            Set(ByVal value As eFleetTrackingType)
                If value <> Me.m_fleettrackingtype Then
                    Me.m_fleettrackingtype = value
                    Me.Populate()
                End If
            End Set
        End Property

#End Region ' Behaviour

#Region " Fleet / index access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a given fleet index is selected.
        ''' </summary>
        ''' <param name="iFleet">The one-based fleet index to test.</param>
        ''' -------------------------------------------------------------------
        Public Property IsFleetSelected(ByVal iFleet As Integer) As Boolean
            Get
                Dim iItem As Integer = Me.FleetIndex(iFleet)
                Return Me.GetSelected(iItem)
            End Get
            Set(ByVal bSelected As Boolean)
                If (Not Me.IsInitialized()) Then Return
                Dim iItem As Integer = Me.FleetIndex(iFleet)
                Me.SetSelected(iItem, bSelected)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether a given fleet is selected.
        ''' </summary>
        ''' <param name="fleet">The fleet to test.</param>
        ''' -------------------------------------------------------------------
        Public Property IsFleetSelected(ByVal fleet As cFleetInput) As Boolean
            Get
                Return Me.IsFleetSelected(fleet.Index)
            End Get
            Set(ByVal bSelected As Boolean)
                If (Not Me.IsInitialized()) Then Return
                Me.IsFleetSelected(fleet) = bSelected
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected fleet index (single selection listboxes only).
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property SelectedFleetIndex() As Integer
            Get
                Dim gi As cFleetItem = Me.FleetItem(Me.SelectedIndex)
                If (gi Is Nothing) Then Return -1
                If (gi.Source Is Nothing) Then Return -1
                Return gi.Source.Index
            End Get
            Set(ByVal iFleet As Integer)
                If (Not Me.IsInitialized()) Then Return
                If (iFleet < 0 Or iFleet >= Me.m_uic.Core.nFleets) Then
                    Me.SelectedIndex = -1
                    Return
                End If
                Me.SelectedIndex = Me.FleetIndex(iFleet)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the selected fleet (single selection listboxes only).
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)> _
        Public Property SelectedFleet() As cFleetInput
            Get
                Dim gi As cFleetItem = DirectCast(Me.SelectedItem, cFleetItem)
                If gi Is Nothing Then Return Nothing
                Return gi.Source
            End Get
            Set(ByVal fleet As cFleetInput)
                If (Not Me.IsInitialized()) Then Return
                If (fleet Is Nothing) Then
                    Me.SelectedIndex = -1
                    Return
                End If
                Me.SelectedIndex = Me.FleetIndex(fleet.Index)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the listbox item for a given fleet index.
        ''' </summary>
        ''' <param name="iFleet">The fleet index to translate into an item index.</param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property FleetIndex(ByVal iFleet As Integer) As Integer
            Get
                Dim gi As cFleetItem = Nothing
                Dim item As Object = Nothing
                Dim fleet As cFleetInput = Nothing

                For i As Integer = 0 To Me.Items.Count - 1
                    item = Me.Items(i)
                    gi = Me.FleetItem(i)
                    If (gi IsNot Nothing) Then
                        fleet = gi.Source
                        If (fleet IsNot Nothing) Then
                            If (fleet.Index = iFleet) Then
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
        ''' Get the listbox item for a given fleet.
        ''' </summary>
        ''' <param name="fleet">The fleet to translate into an item index.</param>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property FleetIndex(ByVal fleet As cFleetInput) As Integer
            Get
                Return Me.FleetIndex(fleet.Index)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the fleet at a given listbox item index.
        ''' </summary>
        ''' <param name="iIndex">The index to return the fleet for.</param>
        ''' <remarks>
        ''' Returns Nothing if no fleet was found at the given index.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GetFleetAt(ByVal iIndex As Integer) As cFleetInput
            Get
                If (iIndex <= 0 Or iIndex >= Me.Items.Count) Then Return Nothing
                Return DirectCast(Me.Items(iIndex), cFleetItem).Source
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the index of a fleet at a given listbox item index.
        ''' </summary>
        ''' <param name="iIndex">The index to return the fleet index for.</param>
        ''' <remarks>
        ''' Returns <see cref="cCore.NULL_VALUE">core NULL</see> if no fleet was
        ''' found at the given index.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property GetFleetIndexAt(ByVal iIndex As Integer) As Integer
            Get
                Dim gi As cFleetItem = Me.FleetItem(iIndex)

                If gi Is Nothing Then Return cCore.NULL_VALUE
                If gi.Source Is Nothing Then Return cCore.NULL_VALUE
                Return gi.Source.Index

            End Get
        End Property

#End Region ' Fleet / index access

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
        ''' <param name="aiFleets">
        ''' An optional list of fleet indexes to populate the listbox with.
        ''' </param>
        ''' -------------------------------------------------------------------
        Public Sub Populate(Optional ByVal aiFleets() As Integer = Nothing)

            Dim iFleetStart As Integer = 1
            Dim iFleetEnd As Integer = 1

            If (Not Me.IsInitialized()) Then Return

            ' Stop automatic tracking if a manual list is provided
            If aiFleets IsNot Nothing Then Me.m_fleettrackingtype = eFleetTrackingType.Manual

            ' ToDo_JS: Preserve fleet selection
            Me.SuspendLayout()

            ' Clear items
            Me.Items.Clear()

            ' (Re)populate listbox
            Select Case Me.m_fleettrackingtype

                Case eFleetTrackingType.Manual
                    If aiFleets IsNot Nothing Then
                        For i As Integer = 0 To aiFleets.Length - 1
                            Me.Items.Add(New cFleetItem(Me.m_uic.Core.FleetInputs(aiFleets(i))))
                        Next
                    End If

                Case eFleetTrackingType.AllFleets
                    iFleetStart = 0 : iFleetEnd = Me.m_uic.Core.nFleets
                    For i As Integer = iFleetStart To iFleetEnd
                        Me.Items.Add(New cFleetItem(Me.m_uic.Core.FleetInputs(i)))
                    Next

                Case eFleetTrackingType.Fleets
                    iFleetStart = 1 : iFleetEnd = Me.m_uic.Core.nFleets
                    For i As Integer = iFleetStart To iFleetEnd
                        Me.Items.Add(New cFleetItem(Me.m_uic.Core.FleetInputs(i)))
                    Next

                Case Else
                    Debug.Assert(False, "eFleetTrackingType not supported")

            End Select

            Me.ResumeLayout()

        End Sub

#End Region ' Refresh

#Region " Internals "

        Protected Function IsInitialized() As Boolean
            Return (Me.m_uic IsNot Nothing)
        End Function

        Protected Function FleetItem(ByVal iIndex As Integer) As cFleetItem
            If (iIndex >= 0 And iIndex < Me.Items.Count) Then Return DirectCast(Me.Items(iIndex), cFleetItem)
            Return Nothing
        End Function

        Protected Function FleetItem(ByVal fleet As cFleetInput) As cFleetItem
            If fleet Is Nothing Then Return Nothing
            Return Me.FleetItem(Me.FleetIndex(fleet.Index))
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
            Dim gi As cFleetListBox.cFleetItem = Nothing
            Dim clrFleet As Color = Color.Transparent
            Dim clrText As Color = e.ForeColor

            ' Attempt to get item colour if it is a cFleetItem
            If (TypeOf item Is cFleetListBox.cFleetItem) Then
                ' Get item fleet
                gi = DirectCast(item, cFleetListBox.cFleetItem)
                ' Has a fleet attached?
                If (gi.Source IsNot Nothing) Then
                    ' #Yes: use dimmed colours
                    clrFleet = Me.m_uic.StyleGuide.FleetColor(Me.m_uic.Core, gi.Source.Index)
                    clrText = e.ForeColor
                End If
            End If

            ' TODO: Take current culture into consideration here. Right-to-left reading order cultures
            ' will need the colour box to be displayed on the right-hand side of the text.

            ' Render default background 
            e.DrawBackground()
            ' Render default text, bumped to the right by 22 pixels
            e.Graphics.DrawString(item.ToString(), e.Font, New SolidBrush(clrText), e.Bounds.X + 22, e.Bounds.Y)

            ' Render colour fill
            Using br As New SolidBrush(clrFleet)
                e.Graphics.FillRectangle(br, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
            End Using
            ' Render colour box border
            Using p As New Pen(clrText, 1)
                e.Graphics.DrawRectangle(p, e.Bounds.X + 2, e.Bounds.Y + 2, 18, e.Bounds.Height - 4)
            End Using

            ' Render default focus rectangle
            e.DrawFocusRectangle()

        End Sub

        Private Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
            If ((ct And cStyleGuide.eChangeType.Colours) > 0) Then
                Me.Invalidate()
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
            Dim gi1 As cFleetItem = Nothing
            Dim fleet1 As cFleetInput = Nothing
            Dim gi2 As cFleetItem = Nothing
            Dim fleet2 As cFleetInput = Nothing

            ' Get sortable items
            If TypeOf (i1) Is cFleetItem Then gi1 = DirectCast(i1, cFleetItem) : fleet1 = gi1.Source
            If TypeOf (i2) Is cFleetItem Then gi2 = DirectCast(i2, cFleetItem) : fleet2 = gi2.Source

            ' Weed out any incompatible item comparisons
            If (gi1 Is Nothing) Then
                If (gi2 Is Nothing) Then
                    ' Not sortable
                    Return 0
                Else
                    ' Non-fleet item sorts before fleet item
                    Return 1
                End If
            Else
                If (gi2 Is Nothing) Then
                    ' Non-fleet item sorts before fleet item
                    Return -1
                End If
            End If

            ' Ok, two cFleetItems to compare
            ' Do both have fleets attached?
            If (fleet1 Is Nothing) Then
                If (fleet2 Is Nothing) Then
                    ' Not sortable
                    Return 0
                Else
                    ' Non-fleet item sorts before fleet item
                    Return 1
                End If
            Else
                If (fleet2 Is Nothing) Then
                    ' Non-fleet item sorts before fleet item
                    Return -1
                End If
            End If

            ' Ok, we have two valid fleets to compare!
            Select Case Me.m_sortType

                Case eSortType.FleetIndexAsc
                    If fleet1.Index < fleet2.Index Then Return -1
                    If fleet1.Index = fleet2.Index Then Return 0
                    Return 1

                Case eSortType.FleetIndexDesc
                    If fleet1.Index > fleet2.Index Then Return -1
                    If fleet1.Index = fleet2.Index Then Return 0
                    Return 1

                Case eSortType.FleetNameAsc
                    Return String.Compare(fleet1.Name, fleet2.Name)

                Case eSortType.FleetNameDesc
                    Return String.Compare(fleet2.Name, fleet1.Name)

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
