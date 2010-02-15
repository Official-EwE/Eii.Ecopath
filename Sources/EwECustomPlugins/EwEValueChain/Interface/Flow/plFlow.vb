#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports EwECore
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Style

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Flow editor control, including flow area, relevant toolbar(s) and 
''' property grid(s).
''' </summary>
''' ===========================================================================
Public Class plFlow
    Inherits Panel

#Region " Private variables "

    ''' <summary>The one reference to underlying data for manipulating objects.</summary>
    Private m_data As cData = Nothing
    ''' <summary>The one reference to underlying diagram data.</summary>
    Private m_diagram As cFlowDiagram = Nothing
    ''' <summary>Bitmap to test for click hits.</summary>
    Private m_bmpClickDetection As Bitmap = Nothing
    ''' <summary>List of cUnit representations.</summary>
    Private m_dtControls As New Dictionary(Of cUnit, plUnitControl)
    ''' <summary>List of cOutputLink representations.</summary>
    Private m_dtLinks As New Dictionary(Of cLink, UnitConnector)
    ''' <summary>Current interaction mode.</summary>
    Private m_editMode As eEditMode = eEditMode.Move
    ''' <summary>PropertyGrid</summary>
    Private m_pg As PropertyGrid = Nothing
    ''' <summary>Style guide</summary>
    Private m_sg As cStyleGuide = Nothing

    ''' <summary>Fleet to filter for in the flow, if any.</summary>
    Private m_fleetFilter As cFleetInput = Nothing

    ''' <summary>Selected flow element.</summary>
    Private m_selection As Object = Nothing
    Private m_hover As Object = Nothing

    '' ToDo: get rid of cUnitControl, render all in this graph
    '' ToDo: make graph scalable, zoomable.

    ''' <summary>Drag/drop mouse offset.</summary>
    ''' <remarks>The (x,y) distance from a control's origin during a drag/drop operation.</remarks>
    Private m_ptMouseOffset As Point = Nothing
    ''' <summary>Unit control being dragged.</summary>
    Private m_ucDrag As plUnitControl = Nothing
    Private m_sScale As Single = 1.0!

    ' Grid bits
    Private m_iCellWidth As Integer = 80
    Private m_iCellHeight As Integer = 40
    Private m_sGridMarginRatio As Single = 0.25 ' top/bottom and left/right margin
    Private m_bShowGrid As Boolean = False

    Private m_iNumControls As Integer = 0

#End Region ' Private variables

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Enumerated type stating the current edit mode.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Enum eEditMode As Byte
        ''' <summary>
        ''' Units are moved when clicked.
        ''' </summary>
        Move = 0
        ''' <summary>
        ''' Units are linked when clicked.
        ''' </summary>
        Link
        ''' <summary>
        ''' Units are deleted when clicked.
        ''' </summary>
        Delete
        ''' <summary>
        ''' The flow is not editable.
        ''' </summary>
        [ReadOnly]
    End Enum

    Public Event EditModeChanged(ByVal sender As plFlow, ByVal mode As eEditMode)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Constructor.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub New()
        Me.AutoScroll = True
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.UserPaint, True)

        Me.m_sg = cStyleGuide.GetInstance()
        AddHandler m_sg.StyleGuideChanged, AddressOf OnStyleguideChanged
    End Sub

#Region " Public interfaces "

#Region " Scale "

    Public Property ZoomFactor() As Single
        Get
            Return Me.m_sScale
        End Get
        Set(ByVal value As Single)
            Me.m_sScale = value
            For Each uc As plUnitControl In Me.m_dtControls.Values
                uc.ZoomFactor = value
            Next
            Me.Invalidate(True)
        End Set
    End Property

#End Region ' Scale

#Region " Filters "

    ''' <summary>
    ''' Get/set the fleet to filter flow layout by.
    ''' </summary>
    Public Property FleetFilter() As cFleetInput
        Get
            Return Me.m_fleetFilter
        End Get
        Set(ByVal value As cFleetInput)
            Me.m_fleetFilter = value
        End Set
    End Property

#End Region ' Filters

#Region " Flow management "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initialize the flow diagram with existing data.
    ''' </summary>
    ''' <param name="fd">The <see cref="cFlowDiagram">data</see> to connect the flow to.</param>
    ''' <param name="pg">The <see cref="PropertyGrid">PropertyGrid</see> that will reflect unit values.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Init(ByVal data As cData, _
                    ByVal fd As cFlowDiagram, ByVal pg As PropertyGrid)

        If (Not Me.m_data Is Nothing) Then
            ' Init only once!
            Debug.Assert(False, "Already initialized!")
            Return
        End If

        ' Store references
        Me.m_data = data
        Me.m_diagram = fd
        Me.m_pg = pg

        ' Load the layout of the flow (this will re-position created units to their saved positions)
        Me.RebuildFlow()

        ' Create click detection bitmap
        Me.CreateClickDetectionBitmap()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the <see cref="eEditMode">edit mode</see> of the panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property EditMode() As eEditMode
        Get
            Return Me.m_editMode
        End Get
        Set(ByVal value As eEditMode)
            If Me.m_editMode <> value Then
                Me.m_editMode = value
                Me.Selection = Nothing
                Me.Hover = Nothing
                RaiseEvent EditModeChanged(Me, Me.m_editMode)
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Auto-arrange the units in the flow panel.
    ''' </summary>
    ''' <remarks>
    ''' The initial version of this algorithm is pretty blunt and should be 
    ''' seriously refined.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Sub Arrange()

        Dim ptUnitMargin As New Point(CInt(Me.m_iCellWidth * m_sGridMarginRatio * 0.5), CInt(Me.m_iCellHeight * m_sGridMarginRatio * 0.5))
        Dim uc As plUnitControl = Nothing
        Dim iUnitColumn As Integer = 0
        Dim aiUnitCount([Enum].GetValues(GetType(cUnitFactory.eUnitType)).Length) As Integer

        ' Align each unit in its column, where row position is based on unit index
        '    ToDo: include branches, merges into algorithm
        For Each unit As cUnit In Me.m_dtControls.Keys
            iUnitColumn = CInt(unit.UnitType) - 1
            uc = Me.m_dtControls(unit)
            With uc.FlowPos
                .AllowEvents = False
                .Xpos = CInt(ptUnitMargin.X + iUnitColumn * Me.m_iCellWidth)
                .Ypos = CInt(ptUnitMargin.Y + aiUnitCount(iUnitColumn) * Me.m_iCellHeight)
                .AllowEvents = True
            End With
            aiUnitCount(iUnitColumn) += 1
        Next

        ' Switch to 'move' mode upon arranging if NOT readonly
        If (Me.EditMode <> eEditMode.ReadOnly) Then
            Me.EditMode = eEditMode.Move
        End If

        Me.Refresh()

    End Sub

    Public Property ShowGrid() As Boolean
        Get
            Return Me.m_bShowGrid
        End Get
        Set(ByVal value As Boolean)
            If value <> Me.m_bShowGrid Then
                Me.m_bShowGrid = value
                Me.Refresh()
            End If
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Rebuild a flow with all units that match present filter settings.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub RebuildFlow()

        ' ToDo: use either unit filter or fleet/group filter

        Dim bInclude As Boolean = False
        Dim lUnits As New List(Of cUnit)

        ' Has unit filter?
        ' #No: has fleet and/or group filter?
        If (Me.FleetFilter IsNot Nothing) Then
            ' #Yes: grab flow operating on the requested fleet/group
            lUnits = Me.m_data.GetUnits(Me.FleetFilter)
        Else
            ' #No: grab all units
            lUnits = Me.m_data.GetUnits(cUnitFactory.eUnitType.All)
        End If

        Me.ClearFlow()

        ' Generate unit elements for all allowed units
        For Each unit As cUnit In lUnits
            ' Must filter by diagram?
            If (Me.m_diagram IsNot Nothing) Then
                ' #Yes: only include when a flow position is available for this unit/diagram
                bInclude = (Me.m_data.FlowPosition(unit, Me.m_diagram) IsNot Nothing)
            Else
                ' #No: always include
                bInclude = True
            End If

            ' Include in diagram?
            If bInclude Then
                ' #Yes: whoohoo
                Me.AddUnit(unit)
            End If
        Next

        ' Generate link elements for unit in the flow
        For Each unit As cUnit In Me.m_dtControls.Keys
            For j As Integer = 0 To unit.LinkOutCount - 1
                Me.AddLink(unit.LinkOut(j), False)
            Next j
        Next unit

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, get all the units currently in the flow.
    ''' </summary>
    ''' <returns>A list with all units currently in the flow.</returns>
    ''' -----------------------------------------------------------------------
    Public Function GetFlowUnits() As List(Of cUnit)
        Dim lUnits As New List(Of cUnit)
        lUnits.AddRange(Me.m_dtControls.Keys)
        Return lUnits
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear the flow
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ClearFlow()

        Dim llinks As New List(Of cLink)
        Dim lunits As New List(Of cUnit)

        ' Get links
        For Each l As cLink In Me.m_dtLinks.Keys
            llinks.Add(l)
        Next
        ' Remove 'em all
        For Each l As cLink In llinks
            Me.RemoveLink(l)
        Next

        ' Get all units
        For Each u As cUnit In Me.m_dtControls.Keys
            lunits.Add(u)
        Next
        ' Remove 'em all
        For Each u As cUnit In lunits
            Me.RemoveUnit(u)
        Next

        Debug.Assert(Me.Controls.Count = 0)
        Debug.Assert(Me.m_dtControls.Count = 0)
        Debug.Assert(Me.m_dtLinks.Count = 0)

    End Sub

#End Region ' Flow management

#End Region ' Public interfaces

#Region " Event handling "

    Private Sub plFlow_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Disposed

        Me.ClearFlow()

        RemoveHandler m_sg.StyleGuideChanged, AddressOf OnStyleguideChanged
        Me.m_sg = Nothing

        Me.m_pg = Nothing
        Me.m_data = Nothing
        Me.m_dtControls = Nothing
        Me.m_dtLinks = Nothing
        Me.m_bmpClickDetection.Dispose()
        Me.m_bmpClickDetection = Nothing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, processes mouse clicks to operate on UnitConnectors.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub HandleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles Me.MouseClick

        Me.ProcessConnectorClick(e.Location)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler, processes mouse movement to provide cursor feedback.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub HandleMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
        Handles Me.MouseMove

        Dim uc As UnitConnector = ConnectorFromPoint(e.Location)
        If (uc IsNot Nothing) Then
            Me.Cursor = Cursors.Hand
            Me.Hover = uc.Link
        Else
            Me.Cursor = Cursors.Default
            Me.Hover = Nothing
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Paint the panel and all unit connectors
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub DoPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
        Handles Me.Paint

        Dim g As Graphics = Graphics.FromImage(Me.m_bmpClickDetection)
        Dim ctrlSource As plUnitControl = Nothing
        Dim ctrlTarget As plUnitControl = Nothing
        Dim clrFore As Color = Color.Black
        Dim clrBack As Color = Color.Black

        ' Draw form background
        Using br As New SolidBrush(Me.BackColor)
            e.Graphics.FillRectangle(br, e.ClipRectangle)
        End Using

        ' Need to draw grid?
        If Me.ShowGrid Then
            ' #Yes: Let's draw that grid then
            ' Use a subtle colour variation on the background by inverting the third bit of its RGB values
            Using p As New Pen(Color.FromArgb(255, Me.BackColor.R Xor 16, Me.BackColor.G Xor 16, Me.BackColor.B Xor 16), 1)
                For i As Integer = CInt(Me.m_sScale * Me.m_iCellHeight) To Me.Height Step CInt(Me.m_sScale * Me.m_iCellHeight)
                    e.Graphics.DrawLine(p, 0, i, Me.Width, i)
                Next
                For i As Integer = CInt(Me.m_sScale * Me.m_iCellWidth) To Me.Width Step CInt(Me.m_sScale * Me.m_iCellWidth)
                    e.Graphics.DrawLine(p, i, 0, i, Me.Height)
                Next
            End Using
        End If

        ' Draw hit detection bitmap
        g.FillRectangle(Brushes.White, 0, 0, Me.Width, Me.Height)

        For Each c As UnitConnector In Me.m_dtLinks.Values
            Try
                ctrlSource = Me.m_dtControls(c.Link.Source)
                ctrlTarget = Me.m_dtControls(c.Link.Target)

                ' Paint link on visible canvas
                If Object.ReferenceEquals(Me.Selection, c) Then
                    clrFore = Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT)
                ElseIf Object.ReferenceEquals(Hover, ctrlSource.Unit) Or _
                       Object.ReferenceEquals(Hover, ctrlTarget.Unit) Or _
                       Object.ReferenceEquals(Hover, c.Link) Then
                    clrFore = Color.RoyalBlue
                Else
                    Me.m_sg.GetStyleColors(c.Link.Style, clrFore, clrBack)
                End If
                PaintLink(e.Graphics, ctrlSource.Center, ctrlTarget.Center, clrFore, c.Link.BiomassRatio, c.Link.External)

                ' Paint detection link on detection bitmap with a fixed width to make the link better clickable
                PaintLink(g, ctrlSource.Center, ctrlTarget.Center, c.Color, 5)

            Catch ex As Exception
                Console.WriteLine("Link {0} not correctly configured", c.Link.Name)
            End Try
        Next

    End Sub

    Protected Overrides Sub OnPaintBackground(ByVal arg As PaintEventArgs)
    End Sub

    Private Sub HandleResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) _
        Handles Me.Resize
        ' When panel is resized, the link detection bitmap needs to be resized accordingly
        Me.CreateClickDetectionBitmap()
    End Sub

    Private Sub OnStyleguideChanged(ByVal changeFlags As cStyleGuide.eChangeType)
        If ((changeFlags And cStyleGuide.eChangeType.Colours) > 0) Then
            Me.Invalidate(True)
        End If
    End Sub

#End Region ' Event handling

#Region " Selection "

    Private Property Selection() As Object
        Get
            Return Me.m_selection
        End Get

        Set(ByVal value As Object)
            ' Optimization
            If Object.ReferenceEquals(Me.m_selection, value) Then Return

            If TypeOf (Me.m_selection) Is plUnitControl Then
                DirectCast(Me.m_selection, plUnitControl).Selected = False
            End If

            ' Assign
            Me.m_selection = value

            If TypeOf (Me.m_selection) Is UnitConnector Then
                If Me.m_pg IsNot Nothing Then
                    ' Update property grid
                    Me.m_pg.SelectedObject = DirectCast(Me.m_selection, UnitConnector).Link
                End If
            ElseIf TypeOf (Me.m_selection) Is plUnitControl Then
                If Me.m_pg IsNot Nothing Then
                    ' Update property grid
                    Me.m_pg.SelectedObject = DirectCast(Me.m_selection, plUnitControl).Unit
                End If
                ' Update selected state
                DirectCast(Me.m_selection, plUnitControl).Selected = True
            End If

            ' Redraw
            Me.Invalidate()

        End Set
    End Property

    Private Property Hover() As Object
        Get
            If Me.m_editMode = eEditMode.Link Then
                Return Me.m_hover
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As Object)
            Me.m_hover = value
            Me.Invalidate()
        End Set
    End Property

#End Region ' Selection

#Region " Item admin "

#Region " Unit creation, deletion, modification "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a producer for every Ecopath landing (fleet x group).
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub CreateProducersByLandings()

        ' For all landings (in Ecopath fleet x group):
        ' Find if metier exists
        '    If not: create it, assign fleet and group

        Dim lUnits As List(Of cUnit) = Me.m_data.GetUnits(cUnitFactory.eUnitType.Producer)
        Dim core As cCore = Me.m_data.Core
        Dim fleet As cFleetInput = Nothing
        Dim group As cEcoPathGroupInput = Nothing
        Dim pu As cProducerUnit = Nothing
        Dim bProducerExists As Boolean = False

        For iFleet As Integer = 1 To core.nFleets
            For iGroup As Integer = 1 To core.nGroups
                fleet = core.FleetInputs(iFleet)
                If fleet.Landings(iGroup) > 0 Then
                    ' Get group
                    group = core.EcoPathGroupInputs(iGroup)
                    ' Find unit
                    bProducerExists = False
                    For Each unit As cUnit In lUnits
                        pu = DirectCast(unit, cProducerUnit)
                        ' Has fleet object?
                        If (Object.ReferenceEquals(fleet, pu.Fleet)) Then
                            If (Object.ReferenceEquals(group, pu.Group) Or (pu.Group Is Nothing)) Then
                                bProducerExists = True
                                Exit For
                            End If
                        End If
                    Next unit
                    ' Not found?
                    If Not bProducerExists Then
                        ' #Yes: create it
                        pu = DirectCast(Me.CreateUnit(cUnitFactory.eUnitType.Producer), cProducerUnit)
                        pu.AllowEvents = False
                        pu.Fleet = fleet
                        pu.Group = group
                        pu.AllowEvents = True
                    End If
                End If
            Next iGroup
        Next iFleet
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a producer for every Ecopath fleet.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub CreateProducersByFleet()

        ' For all Ecopath fleets:
        ' Find if group linking to a fleet already exists
        '    If not: create it, assign fleet and DO NOT assign group

        Dim lUnits As List(Of cUnit) = Me.m_data.GetUnits(cUnitFactory.eUnitType.Producer)
        Dim core As cCore = Me.m_data.Core
        Dim fleet As cFleetInput = Nothing
        Dim pu As cProducerUnit = Nothing
        Dim bProducerExists As Boolean = False

        For iFleet As Integer = 1 To core.nFleets
            ' Find unit
            bProducerExists = False
            fleet = core.FleetInputs(iFleet)
            For Each unit As cUnit In lUnits
                pu = DirectCast(unit, cProducerUnit)
                If (Object.ReferenceEquals(fleet, pu.Fleet)) Then
                    bProducerExists = True
                    Exit For
                End If
            Next unit
            ' Not found?
            If Not bProducerExists Then
                ' #Yes: create it
                pu = DirectCast(Me.CreateUnit(cUnitFactory.eUnitType.Producer), cProducerUnit)
                pu.AllowEvents = False
                pu.Fleet = fleet
                pu.Group = Nothing
                pu.AllowEvents = True
            End If
        Next iFleet
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create a new unit
    ''' </summary>
    ''' <param name="unitType"></param>
    ''' -----------------------------------------------------------------------
    Public Function CreateUnit(ByVal unitType As cUnitFactory.eUnitType) As cUnit

        Dim unit As cUnit = Nothing
        Dim lstrNames As New List(Of String)
        Dim strMask As String = ""
        Dim strName As String = ""

        ' Gather names
        For Each unit In Me.m_data.GetUnits(unitType)
            lstrNames.Add(unit.Name)
        Next

        ' Determine mask name
        Select Case unitType
            Case cUnitFactory.eUnitType.Market
                strMask = "Market {0}"
            Case cUnitFactory.eUnitType.Processing
                strMask = "Processing {0}"
            Case cUnitFactory.eUnitType.Producer
                strMask = ""
            Case cUnitFactory.eUnitType.Distribution
                strMask = "Distribution {0}"
            Case cUnitFactory.eUnitType.Consumer
                strMask = "Consumer {0}"
                'Case cUnitFactory.eUnitType.NonExtractive
                '    strMask = "Non-extractive {0}"
        End Select

        ' Has a mask?
        If Not String.IsNullOrEmpty(strMask) Then
            ' #Yes: concoct a name with an autonumber
            strName = String.Format(strMask, EwEUtils.Utilities.cStringUtils.GetNextNumber(lstrNames.ToArray, strMask))
        End If

        ' (try to) create unit
        unit = Me.m_data.CreateUnit(unitType, strName)

        ' Successfully created?
        If unit IsNot Nothing Then
            ' #Yes: add unit 
            Me.AddUnit(unit, True)
            ' Switch to move mode
            Me.EditMode = eEditMode.Move
        End If

        Return unit

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an existing unit to the flow.
    ''' </summary>
    ''' <param name="unit">The unit to add.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AddUnit(ByVal unit As cUnit, Optional ByVal bSelect As Boolean = False) As plUnitControl

        Dim fp As cFlowPosition = Nothing

        ' Try to get existing flow position for registered diagrams
        If (Me.m_diagram IsNot Nothing) Then
            fp = Me.m_data.FlowPosition(unit, Me.m_diagram)
        End If

        ' No flow position available?
        If fp Is Nothing Then
            ' #Yes: create one
            fp = New cFlowPosition()
            fp.Unit = unit
            fp.Diagram = Me.m_diagram
            ' Need to store new flow position for registered diagram?
            If (Me.m_diagram IsNot Nothing) Then
                ' #Yes: store it
                Me.m_data.AddFlowPosition(fp)
            End If
   
            fp.AllowEvents = False
            fp.Xpos = (10 + m_iNumControls * 10)
            fp.Ypos = fp.Xpos
            fp.Width = CInt(Me.m_iCellWidth * (1 - Me.m_sGridMarginRatio))
            fp.Height = CInt(Me.m_iCellHeight * (1 - Me.m_sGridMarginRatio))
            fp.AllowEvents = True

        End If

        Dim uc As New plUnitControl(fp)

        uc.ZoomFactor = Me.ZoomFactor

        Me.Controls.Add(uc)
        Me.m_dtControls(fp.Unit) = uc
        uc.BringToFront()

        If bSelect Then Me.Selection = uc

        Me.m_iNumControls += 1

        ' Start listening for unit changes
        AddHandler fp.Unit.OnChanged, AddressOf OnElementChanged

        Return uc

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a flow element from this control.
    ''' </summary>
    ''' <param name="unit"></param>
    ''' -----------------------------------------------------------------------
    Public Sub RemoveUnit(ByVal unit As cUnit)

        Dim uc As plUnitControl = Me.m_dtControls(unit)

        ' Detach event handlers
        RemoveHandler unit.OnChanged, AddressOf OnElementChanged

        ' Remove all source links
        For i As Integer = 0 To unit.LinkInCount - 1
            Me.RemoveLink(unit.LinkIn(i))
        Next
        ' Remove all target links
        For i As Integer = 0 To unit.LinkOutCount - 1
            Me.RemoveLink(unit.LinkOut(i))
        Next

        ' Clear selection if neccesary
        If Object.ReferenceEquals(Me.m_selection, unit) Then Me.m_selection = Nothing
        ' Clear dragged object if neccesary
        If Object.ReferenceEquals(Me.m_ucDrag, uc) Then Me.m_ucDrag = Nothing

        ' Remove control
        Me.m_dtControls.Remove(unit)
        Me.Controls.Remove(uc)
        ' Manually clear this
        uc.Dispose()

        ' Rerender
        Me.Invalidate()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove an existing unit from the flow, and remove it from the underlying data
    ''' </summary>
    ''' <param name="unit"></param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function DeleteUnit(ByVal unit As cUnit, ByVal fp As cFlowPosition) As Boolean

        'Select Case MessageBox.Show("Do you wish to entirely delete the unit? Click 'Yes' to delete the unit from the plugin, or 'No' to only remove the unit from this diagram", _
        '                   "Delete unit", MessageBoxButtons.YesNoCancel)
        '    Case DialogResult.Yes
        '        Me.m_data.DeleteUnit(unit)
        '    Case DialogResult.No
        '        Me.m_data.DeleteFlowPosition(fp)

        '    Case DialogResult.Cancel
        '        ' NOP
        'End Select

        Return Me.m_data.DeleteUnit(unit)

    End Function

#End Region ' Unit creation, deletion, modification 

#Region " Link creation, deletion, modification "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add an existing link to the flow
    ''' </summary>
    ''' <param name="link"></param>
    ''' <param name="bRefresh"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AddLink(ByVal link As cLink, Optional ByVal bRefresh As Boolean = True) As UnitConnector
        If link Is Nothing Then Return Nothing
        AddHandler link.OnChanged, AddressOf OnElementChanged

        Dim uc As New UnitConnector(link)
        Me.m_dtLinks(link) = uc
        If bRefresh Then Me.Invalidate()
        Return uc
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a link from the flow
    ''' </summary>
    ''' <param name="link"></param>
    ''' <remarks></remarks>
    ''' -----------------------------------------------------------------------
    Public Sub RemoveLink(ByVal link As cLink)
        If link Is Nothing Then Return
        RemoveHandler link.OnChanged, AddressOf OnElementChanged

        ' Clear selection if neccesary
        If Object.ReferenceEquals(Me.m_selection, link) Then Me.m_selection = Nothing

        Me.m_dtLinks.Remove(link)
        Me.Invalidate()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Remove a link from the flow, and delete it in the underlying data.
    ''' </summary>
    ''' <param name="link"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function DeleteLink(ByVal link As cLink) As Boolean
        Me.RemoveLink(link)
        Me.m_data.DeleteLink(link)
        Return True
    End Function

#End Region ' Link creation, deletion, modification

#End Region ' Item admin

#Region " Item UI "

#Region " Units "

    Public Sub OnUnitMouseDown(ByVal uc As plUnitControl)

        Select Case Me.EditMode

            Case eEditMode.Move
                Me.StartUnitDrag(uc)
                Me.Selection = uc

            Case eEditMode.Link
                If TypeOf Me.Selection Is plUnitControl Then
                    Dim link As cLink = Me.m_data.CreateLink(DirectCast(Me.Selection, plUnitControl).Unit, uc.Unit)
                    If link IsNot Nothing Then
                        Me.AddLink(link)
                    End If
                    ' Clear selection
                    Me.Selection = Nothing
                Else
                    Me.Selection = uc
                End If

            Case eEditMode.Delete
                If Me.DeleteUnit(uc.Unit, uc.FlowPos) Then
                    ' Do not delete again
                    Me.EditMode = eEditMode.Move
                    ' Yeah!
                    Me.RebuildFlow()
                End If

        End Select

    End Sub

    Public Sub OnUnitMouseHover(ByVal uc As plUnitControl, ByVal bHover As Boolean)
        If bHover Then
            Me.Hover = uc.Unit
        Else
            If Object.ReferenceEquals(uc.Unit, Me.Hover) Then
                Me.Hover = Nothing
            End If
        End If
    End Sub

#End Region ' Units

#Region " Links "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, creates the bitmap for detecting line clicks
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub CreateClickDetectionBitmap()
        Dim bmp As Bitmap = Me.m_bmpClickDetection
        If bmp IsNot Nothing Then bmp.Dispose() : bmp = Nothing
        Me.m_bmpClickDetection = New Bitmap(Me.ClientRectangle.Width + 1, Me.ClientRectangle.Height + 1, Imaging.PixelFormat.Format32bppArgb)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, returns the color of a pixel in the click detection bitmap.
    ''' </summary>
    ''' <param name="pt">The location of the pixel to return the color for.</param>
    ''' <returns>The color of the indicated pixel in the detection bitmap.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ColorAtPoint(ByVal pt As Point) As Color
        Try
            Return Me.m_bmpClickDetection.GetPixel(pt.X, pt.Y)
        Catch ex As Exception
            Return Color.Transparent
        End Try
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether a color value is a valid line color in the click detection bitmap.
    ''' </summary>
    ''' <param name="clr">The color to test.</param>
    ''' <returns>True if the color could be used for a line</returns>
    ''' -----------------------------------------------------------------------
    Private Function IsLineColor(ByVal clr As Color) As Boolean
        Return clr.R <> 255 Or clr.G <> 255 Or clr.B <> 255
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, attempts to return the unit connector for a given color value.
    ''' </summary>
    ''' <param name="clr">The color to find the unit connector for.</param>
    ''' <returns>A unit connector instance, or nothing if this connector could not be found.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ConnectorFromColor(ByVal clr As Color) As UnitConnector
        If IsLineColor(clr) Then
            For Each uc As UnitConnector In Me.m_dtLinks.Values
                If uc.Color = clr Then
                    Return uc
                End If
            Next
        End If
        Return Nothing
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, attempts to return the unit connector for a given color value.
    ''' </summary>
    ''' <param name="pt">The location to test.</param>
    ''' <returns>A unit connector instance, or nothing if this connector could not be found.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ConnectorFromPoint(ByVal pt As Point) As UnitConnector
        Return Me.ConnectorFromColor(Me.ColorAtPoint(pt))
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether the point under the mouse cursor belongs to a connector.
    ''' </summary>
    ''' <param name="pt">The location to test.</param>
    ''' <returns>True if there is a unit connector at (or very near to) this location.</returns>
    ''' -----------------------------------------------------------------------
    Private Function HasConnectorUnderCursor(ByVal pt As Point) As Boolean
        Dim conn As UnitConnector = Me.ConnectorFromPoint(pt)
        Return (conn IsNot Nothing)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Process a mouse click to find and operate on a UnitConnector.
    ''' </summary>
    ''' <param name="pt">The location that was clicked.</param>
    ''' -----------------------------------------------------------------------
    Private Sub ProcessConnectorClick(ByVal pt As Point)

        Dim conn As UnitConnector = Me.ConnectorFromColor(Me.ColorAtPoint(pt))
        If (conn IsNot Nothing) Then

            Select Case Me.EditMode
                Case eEditMode.Delete
                    Me.DeleteLink(conn.Link)
                    Me.Refresh()
                Case eEditMode.Move
                    Me.Selection = conn
                Case eEditMode.Link
                    Me.Selection = Nothing
            End Select

        Else
            Me.Selection = Nothing
        End If

    End Sub

#End Region ' Links

#Region " Drag/drop "

    Private Sub StartUnitDrag(ByVal uc As plUnitControl)
        Dim ptMouse As Point = Cursor.Position
        Dim ptControl As Point = uc.Location

        If (Me.m_ucDrag Is Nothing) Then
            Me.m_ucDrag = uc
            Me.m_ucDrag.BringToFront()
            Me.m_ptMouseOffset = New Point(ptMouse.X - ptControl.X, ptMouse.Y - ptControl.Y)

            AddHandler Me.m_ucDrag.MouseMove, AddressOf TrackMouseMove
            AddHandler Me.m_ucDrag.MouseUp, AddressOf TrackMouseUp
        End If
    End Sub

    Private Sub EndUnitDrag()
        If (Me.m_ucDrag IsNot Nothing) Then
            RemoveHandler Me.m_ucDrag.MouseMove, AddressOf TrackMouseMove
            RemoveHandler Me.m_ucDrag.MouseUp, AddressOf TrackMouseUp
        End If
        Me.m_ucDrag = Nothing
    End Sub

    Private Sub TrackMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

        Dim ptUnitMargin As New Point(CInt(Me.m_iCellWidth * Me.m_sGridMarginRatio * 0.5), _
                                      CInt(Me.m_iCellHeight * Me.m_sGridMarginRatio * 0.5))
        Dim ptMouse As Point = Cursor.Position

        ' Drag via flow position
        With Me.m_ucDrag.FlowPos
            .AllowEvents = False
            .Xpos = CInt((ptMouse.X - Me.m_ptMouseOffset.X) / Me.m_sScale)
            .Ypos = CInt((ptMouse.Y - Me.m_ptMouseOffset.Y) / Me.m_sScale)
            If Me.m_bShowGrid Then
                ' Truncate pos
                .Xpos = ptUnitMargin.X + CInt(Me.m_iCellWidth * Math.Round(.Xpos / Me.m_iCellWidth))
                .Ypos = ptUnitMargin.Y + CInt(Me.m_iCellHeight * Math.Round(.Ypos / Me.m_iCellHeight))
            End If

            .AllowEvents = True
        End With
        Me.Refresh()
    End Sub

    Private Sub TrackMouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Me.EndUnitDrag()
    End Sub

#End Region ' Drag/drop

#End Region ' Item UI

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event handler; trapped to redraw live data changes.
    ''' </summary>
    ''' <param name="obj">The item that changed.</param>
    ''' -----------------------------------------------------------------------
    Private Sub OnElementChanged(ByVal obj As cOOPStorable)
        Me.Invalidate()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Paint a link between two units.
    ''' </summary>
    ''' <param name="g">Graphics to draw onto.</param>
    ''' <param name="ptStart">Start point to draw from.</param>
    ''' <param name="ptEnd">End point to draw to.</param>
    ''' <param name="clr">Colour to use for the link.</param>
    ''' <param name="sWeight">Weight of the link [0, 1]. 
    ''' If 0, the link is rendered in gray.</param>
    ''' <param name="bExternal">Flag stating whether this link represents an 
    ''' 'External' connection.</param>
    ''' -----------------------------------------------------------------------
    Private Sub PaintLink(ByVal g As Graphics, ByVal ptStart As Point, ByVal ptEnd As Point, ByVal clr As Color, ByVal sWeight As Single, _
            Optional ByVal bExternal As Boolean = False)

        Dim p As Pen = Nothing
        Dim ptStartScaled As New Point(CInt(ptStart.X * Me.m_sScale), CInt(ptStart.Y * Me.m_sScale))
        Dim ptEndScaled As New Point(CInt(ptEnd.X * Me.m_sScale), CInt(ptEnd.Y * Me.m_sScale))

        Dim dx As Integer = ptEndScaled.X - ptStartScaled.X
        Dim dy As Integer = ptEndScaled.Y - ptStartScaled.Y
        Dim ptIndicatorStartScaled As New Point(ptStartScaled.X + CInt(dx * 0.6), ptStartScaled.Y + CInt(dy * 0.6))
        Dim ptIndicatorEndScaled As New Point(ptIndicatorStartScaled.X, ptIndicatorStartScaled.Y)
        Dim sIndicatorLength As Single = Me.m_sScale * 5
        Dim sAngle As Single = Me.Angle(dx, dy) - CSng(Math.PI / 2)

        ptIndicatorEndScaled.X += CInt(sIndicatorLength * Math.Sin(sAngle))
        ptIndicatorEndScaled.Y -= CInt(sIndicatorLength * Math.Cos(sAngle))

        ' Get pen to draw with. Zero weight?
        If sWeight = 0 Then
            ' #Yes: render a dotted, thin line
            p = New Pen(clr, 1)
            p.DashStyle = Drawing2D.DashStyle.Dot
        Else
            ' 'No: Render a line of a width representing this weight. Weight is a value between
            '      0 and 1. Pen sizes of this magnitude do not show up well, therefore the actual
            '      pen width is an arbitrary 3 * sWeight to make it look better.
            p = New Pen(clr, sWeight * 3)
        End If

        ' External link?
        If bExternal Then
            ' #Yes: render the line dashed
            p.DashStyle = Drawing2D.DashStyle.Dash
        End If

        ' Finally draw
        g.DrawLine(p, ptStartScaled, ptEndScaled)
        g.DrawLine(p, ptIndicatorStartScaled, ptIndicatorEndScaled)

        p.Dispose()

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Paint a unit
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="pt"></param>
    ''' <param name="unit"></param>
    ''' <param name="bSelected"></param>
    ''' <remarks>
    ''' JS 11mar09: method not used, painting still handled by cUnitControl
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Sub PaintUnit(ByVal g As Graphics, ByVal pt As Point, ByVal unit As cUnit, ByVal bSelected As Boolean, ByVal bHover As Boolean)

        Dim rc As Rectangle = Me.ClientRectangle
        rc.Width -= 1
        rc.Height -= 1

        g.FillRectangle(SystemBrushes.Window, rc)
        If bSelected Then
            Using p As New Pen(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT))
                g.DrawRectangle(p, rc)
            End Using
        Else
            Using p As New Pen(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.DEFAULT_TEXT))
                g.DrawRectangle(Pens.Black, rc)
            End Using
        End If
        Using ft As New Font(Me.Font.FontFamily, CSng(Me.Font.Size * Me.Width / 80), FontStyle.Regular, Me.Font.Unit)
            g.DrawString(unit.Name, ft, SystemBrushes.WindowText, Me.ClientRectangle)
        End Using

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns the angle between two points in radians. 0 degrees is straigt up,
    ''' angle rotates clockwise.
    ''' </summary>
    ''' <param name="dx"></param>
    ''' <param name="dy"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function Angle(ByVal dx As Integer, ByVal dy As Integer) As Single

        Dim sHalfPI As Single = CSng(Math.PI / 2)
        Dim sAngle As Single = 0

        If dy = 0 Then
            If dx >= 0 Then
                sAngle = sHalfPI
            Else
                sAngle = 3 * sHalfPI
            End If
        Else
            sAngle = CSng(Math.Atan(dx / -dy))

            ' Find quadrant
            If dy > 0 Then
                sAngle += 2 * sHalfPI
            Else
                If dx < 0 Then
                    sAngle += 4 * sHalfPI
                End If
            End If
        End If

        Return sAngle

    End Function

#End Region ' Internals

End Class
