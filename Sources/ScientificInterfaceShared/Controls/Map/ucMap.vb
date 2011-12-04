#Region " Imports "

Option Explicit On
Option Strict On

Imports System.ComponentModel
Imports System.IO
Imports EwECore
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls.Map

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Control that provides an interface to a series core data map layers.
    ''' </summary>
    ''' <remarks>
    ''' To provide zoom functionality, use <see cref="ucMapZoom">ucMapZoom</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class ucMap
        Implements IUIElement

        ''' <summary>UI context to work against.</summary>
        Private m_uic As cUIContext = Nothing
        ''' <summary>The bitmap to draw on.</summary>
        Private m_bmp As Bitmap = Nothing
         ''' <summary>Map title.</summary>
        Private m_strTitle As String = ""
        ''' <summary>List of layers.</summary>
        Private m_layers As New List(Of cLayer)
        ''' <summary>Selected layer</summary>
        Private m_layerSelected As cLayer = Nothing

        Public Sub New()

            Me.InitializeComponent()

            ' Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.UserPaint, True)

            Me.BackColor = Color.White
            Me.BorderStyle = Windows.Forms.BorderStyle.FixedSingle

        End Sub

        ''' <inheritdocs cref="IUIElement.UIContext"/>
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal uic As cUIContext)
                Me.m_uic = uic
                Me.Clear()
            End Set
        End Property

#Region " Public interfaces "

        Public Function SaveToBitmap(ByVal strFileName As String, ByVal format As System.Drawing.Imaging.ImageFormat) As Boolean

            Dim szCellSize As SizeF = Me.GetCellSize()
            Try
                Dim bmp As New Bitmap(CInt(Me.Basemap.InCol * szCellSize.Width), CInt(Me.Basemap.InRow * szCellSize.Height))
                Me.UpdateMap(bmp, New Point(1, 1), New Point(Me.Basemap.InCol, Me.Basemap.InRow))
                bmp.Save(strFileName, format)
            Catch ex As Exception
                Return False
            End Try

            Dim lgd As cLegend = cLegend.FromMap(Me)
            Dim strExt As String = Path.GetExtension(strFileName)

            strFileName = Path.Combine(Path.GetDirectoryName(strFileName), Path.GetFileNameWithoutExtension(strFileName) & "_legend" & strExt)
            lgd.SaveAsBitmap(strFileName, format)

            Return True

        End Function

#End Region ' Public interfaces

#Region " Public properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the map title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True)> _
        <Category("Appearance")> _
        <Description("Title of the map to display")> _
        Public Property Title() As String
            Get
                Return Me.m_strTitle
            End Get
            Set(ByVal strTitle As String)
                Me.m_strTitle = strTitle
            End Set
        End Property

        ' ''' -------------------------------------------------------------------
        ' ''' <summary>
        ' ''' Get a legend for the current map.
        ' ''' </summary>
        ' ''' -------------------------------------------------------------------
        'Public ReadOnly Property Legend() As cLegend
        '    Get
        '        Return cLegend.FromMap(Me, Me.m_uic)
        '    End Get
        'End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the map.
        ''' </summary>
        ''' <remarks>Redrawing the map entirely may be slow!</remarks>
        ''' -------------------------------------------------------------------
        Public Overloads Sub Refresh()
            Me.UpdateMap()
            Me.UpdateCursorFeedback()
        End Sub

        Private m_bEditable As Boolean = False

        Public Property Editable() As Boolean
            Get
                Return Me.m_bEditable
            End Get
            Set(ByVal value As Boolean)
                Me.m_bEditable = value
                Me.UpdateCursorFeedback()
            End Set
        End Property

        Public ReadOnly Property NumCols() As Integer
            Get
                If (Me.Basemap Is Nothing) Then Return 20
                Return Me.Basemap.InCol
            End Get
        End Property

        Public ReadOnly Property NumRows() As Integer
            Get
                If (Me.Basemap Is Nothing) Then Return 20
                Return Me.Basemap.InRow
            End Get
        End Property

#End Region ' Public properties

#Region " Event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.OnResized(Me, e)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clean-up.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ucBaseMap_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed
            Me.Clear()
        End Sub

#If DRAW_THREADED Then
        Private m_thread As Threading.Thread
#End If

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Paint handler; selectively redraws the bitmap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            ' If Object.ReferenceEquals(Me.m_bmp, Nothing) Then Return

            ' Needs new bitmap?
            If (Me.m_bmp Is Nothing) Then
                ' #Yes: create new bitmap
                Me.m_bmp = New Bitmap(Me.Width, Me.Height)
                Me.BackgroundImage = Me.m_bmp

#If DRAW_THREADED Then
                If Me.m_thread IsNot Nothing Then
                    If Me.m_thread.IsAlive Then
                        Me.m_thread.Abort()
                    End If
                    Me.m_thread = Nothing
                End If

                Me.m_thread = New Threading.Thread(AddressOf RedrawMapThreaded)
                Me.m_thread.Start()
#Else
 
                Me.UpdateMap(Me.m_bmp, New Point(1, 1), New Point(Me.Basemap.InCol, Me.Basemap.InRow))
#End If
            End If

            MyBase.OnPaint(e)

        End Sub

#If DRAW_THREADED Then
        Private Sub RedrawMapThreaded()
            Me.UpdateMap(Me.m_bmp, New Point(1, 1), New Point(Me.m_basemap.InCol, Me.m_basemap.InRow))
            Me.Invalidate()
            Me.m_thread = Nothing
        End Sub
#End If

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse down handler; intializes map drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)

            Dim bShiftPressed As Boolean = (Control.ModifierKeys = Keys.Shift)
            Dim ptCellCur As Point = Me.GetCellIndex(New Point(e.X, e.Y))

            If (Me.CanEdit = False) Then Return

            If ((e.Button And Windows.Forms.MouseButtons.Right) > 0) Then

                Me.m_layerSelected.Editor.Pickup(Me.GetCellIndex(e.Location))
                Me.Capture = False

            ElseIf ((e.Button And MouseButtons.Left) > 0) Then

                Me.Capture = True

                ' If NOT Shift key pressed, release the last mouse pos
                If Not bShiftPressed Then Me.m_ptScreenPrevious = Nothing

                ' Start editing
                Me.m_layerSelected.Editor.StartEdit(ptCellCur, e)

                Me.ProcessMouseInput(e)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse move handler; performs a map drawing step.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)

            If (Me.CanEdit = False) Then Return
            If (Me.Capture = False) Then Return

            Me.ProcessMouseInput(e)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse up handler; finalizes map drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)

            If (Me.CanEdit = False) Then Return
            If (Me.Capture = False) Then Return

            Me.m_layerSelected.Editor.EndEdit()

            ' Process pending layer changes
            For Each l As cLayer In m_layers
                If l.IsModified Then l.Update(cLayer.eChangeFlags.Map) : l.IsModified = False
            Next

            Me.Capture = False

        End Sub

        Private Sub OnResized(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Resize

            If (Me.m_bmp IsNot Nothing) Then
                Me.BackgroundImage = Nothing
                Me.m_bmp.Dispose()
                Me.m_bmp = Nothing
            End If

            ' Update cursor
            Me.UpdateCursorFeedback()
            ' Schedule paint job
            Me.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Layer changed event
        ''' </summary>
        ''' <param name="l">The layer that changed</param>
        ''' -------------------------------------------------------------------
        Private Sub OnLayerChanged(ByVal l As cLayer, ByVal cf As cLayer.eChangeFlags)

            ' Ignore sole descriptive layer changes
            If (cf = cLayer.eChangeFlags.Descriptive) Then Return

            ' Handle selection changes
            If ((cf And cLayer.eChangeFlags.Selected) > 0) Then
                ' Update selection
                Me.UpdateSelection(l)
            End If

            If ((cf And (cLayer.eChangeFlags.Map Or _
                                 cLayer.eChangeFlags.Visibility Or _
                                 cLayer.eChangeFlags.VisualStyle Or _
                                 cLayer.eChangeFlags.Selected)) > 0) Then
                ' Update Map
                Me.UpdateMap()
            End If

            If ((cf And (cLayer.eChangeFlags.Editable Or cLayer.eChangeFlags.Selected)) > 0) Then
                ' Refresh edit environment
                Me.UpdateCursorFeedback()
            End If

        End Sub

#End Region ' Event handlers

#Region " Internals "

        Protected ReadOnly Property Basemap As cEcospaceBasemap
            Get
                If (Me.m_uic Is Nothing) Then Return Nothing
                Return Me.m_uic.Core.EcospaceBasemap
            End Get
        End Property

        ''' <summary>Draw helper flag: previous draw point.</summary>
        Private m_ptScreenPrevious As Point = Nothing

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Performs a draw step by updating the memory bitmap.
        ''' </summary>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Private Sub ProcessMouseInput(ByVal e As MouseEventArgs)

            If (Me.CanEdit = False) Then Return
            If (Me.Capture = False) Then Return

            Dim ptScreenCur As Point = New Point(e.X, e.Y)

            If (Me.m_ptScreenPrevious = Nothing) Then Me.m_ptScreenPrevious = ptScreenCur

            Dim ptCellFrom As Point = Me.GetCellIndex(Me.m_ptScreenPrevious)
            Dim ptCellTo As Point = Me.GetCellIndex(ptScreenCur)
            Dim ptUpdateMin As New Point(Math.Min(ptCellFrom.X, ptCellTo.X), Math.Min(ptCellFrom.Y, ptCellTo.Y))
            Dim ptUpdateMax As New Point(Math.Max(ptCellFrom.X, ptCellTo.X), Math.Max(ptCellFrom.Y, ptCellTo.Y))

            Me.m_layerSelected.Editor.Edit(ptCellFrom, ptCellTo, _
                                           New Point(ptScreenCur.X - Me.m_ptScreenPrevious.X, ptScreenCur.Y - Me.m_ptScreenPrevious.Y), _
                                           Me.GetCellSize(), _
                                           e, _
                                           ptUpdateMin, ptUpdateMax)

            ' Flag layer as changed
            Me.m_layerSelected.IsModified = True

            Me.UpdateMap(Me.m_bmp, ptUpdateMin, ptUpdateMax)

            Me.m_ptScreenPrevious = ptScreenCur

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the entire map image.
        ''' </summary>
        ''' <remarks>
        ''' This will invalidate the entire map screen area.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub UpdateMap()

            ' Sanity check
            If Object.ReferenceEquals(Me.Basemap, Nothing) Then Return

            If (Me.m_bmp IsNot Nothing) Then
                Me.BackgroundImage = Nothing
                Me.m_bmp.Dispose()
                Me.m_bmp = Nothing
            End If

            Me.Invalidate()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update a range of cells in the map image.
        ''' </summary>
        ''' <remarks>
        ''' This will invalidate the map screen area encompassing the range 
        ''' of indicated cells.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub UpdateMap(ByVal bmp As Bitmap, ByVal ptCellFrom As Point, ByVal ptCellTo As Point)

            ' Sanity check
            If Object.ReferenceEquals(Me.Basemap, Nothing) Then Return

            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim l As cLayer = Nothing
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            Dim ldDepth As cEcospaceLayer = Me.Basemap.LayerDepth()
            Dim szCell As SizeF = Me.GetCellSize()
            Dim ptCell As Point = Nothing
            Dim rcScreen As Rectangle = Nothing
            Dim bDrawCell As Boolean = False

            ' Calc area to invalidate
            Dim p1 As Point = Me.GetCellPos(ptCellFrom)
            Dim p2 As Point = Me.GetCellPos(ptCellTo)

            ' Sort coords
            Dim iXFrom As Integer = Math.Min(p1.X, p2.X)
            Dim iXTo As Integer = Math.Max(p1.X, p2.X)
            Dim iYFrom As Integer = Math.Min(p1.Y, p2.Y)
            Dim iYTo As Integer = Math.Max(p1.Y, p2.Y)

            ' Clear and invalidate the area
            rcScreen = New Rectangle(iXFrom, iYFrom, iXTo - iXFrom + CInt(szCell.Width), iYTo - iYFrom + CInt(szCell.Height))
            g.FillRectangle(New SolidBrush(Me.BackColor), rcScreen)
            Me.Invalidate(rcScreen)

            ' Draw surrounding cells as well to avoid anomalies
            iXFrom = Math.Max(0, Math.Min(ptCellFrom.X, ptCellTo.X) - 1)
            iYFrom = Math.Max(0, Math.Min(ptCellFrom.Y, ptCellTo.Y) - 1)
            iXTo = Math.Min(Me.Basemap.InCol, Math.Max(ptCellFrom.X, ptCellTo.X) + 1)
            iYTo = Math.Min(Me.Basemap.InRow, Math.Max(ptCellFrom.Y, ptCellTo.Y) + 1)

            For X As Integer = iXFrom To iXTo
                For Y As Integer = iYFrom To iYTo

                    ptCell = New Point(X, Y)
                    rcScreen = Me.GetCellRect(ptCell)

                    ' Draw layers in reverse order
                    For iLayer As Integer = Me.m_layers.Count - 1 To 0 Step -1

                        ' Get layer
                        l = Me.m_layers(iLayer)
                        ' Reset style flag
                        style = cStyleGuide.eStyleFlags.OK

                        Select Case l.Data.DataType
                            Case eDataTypes.EcospaceLayerDepth, eDataTypes.EcospaceLayerPort
                                bDrawCell = True
                            Case Else
                                bDrawCell = (CInt(ldDepth.Cell(Y, X)) > 0)
                        End Select

                        If l.Renderer.IsVisible And bDrawCell Then
                            If l.HasValue(ptCell.Y, ptCell.X) Then
                                ' Build style flags
                                If l.IsSelected Then
                                    style = (style Or cStyleGuide.eStyleFlags.Highlight)
                                End If
                                ' Render cell
                                l.Renderer.RenderCell(g, rcScreen, l.Data, l.Value(ptCell.Y, ptCell.X), style)
                            End If
                        End If

                    Next iLayer

                Next Y
            Next X

            g.Dispose()

        End Sub

        Private Sub UpdateSelection(ByVal l As cLayer)

            ' Sanity check
            If Object.ReferenceEquals(Me.Basemap, Nothing) Then Return

            ' New selection?
            If l.IsSelected Then
                ' #Yes: set selected layer
                Me.m_layerSelected = l
            Else
                ' #No: current selection being cleared?
                If Object.ReferenceEquals(Me.m_layerSelected, l) Then
                    ' #Yes: clear selection
                    Me.m_layerSelected = Nothing
                End If
            End If
            ' Reflect this
            Me.UpdateCursorFeedback()
        End Sub

        Public Sub UpdateCursorFeedback()

            ' Sanity check
            If Object.ReferenceEquals(Me.Basemap, Nothing) Then Return

            If Me.CanEdit Then
                Me.Cursor = Me.m_layerSelected.Editor.Cursor(Me.GetCellSize())
            Else
                Me.Cursor = Cursors.Default
            End If
        End Sub

        Private Sub SetBrushCursor(ByVal iBrushSize As Integer)

            Dim szCell As SizeF = Me.GetCellSize()
            Dim ptIconSize As New Size(CInt(szCell.Width * iBrushSize), CInt(szCell.Height * iBrushSize))

            If iBrushSize = 0 Then
                Me.Cursor = Cursors.Default
            Else
                Try
                    Dim bm As New Bitmap(ptIconSize.Width + 1, ptIconSize.Height + 1)
                    Dim g As Graphics = Graphics.FromImage(bm)

                    g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    g.FillRectangle(Brushes.Transparent, New Rectangle(0, 0, bm.Width, bm.Height))
                    g.DrawEllipse(Pens.Gray, 0, 0, ptIconSize.Width, ptIconSize.Height)
                    Me.Cursor = New Cursor(bm.GetHicon())
                    g.Dispose()
                    bm.Dispose()

                Catch e As Exception
                    Debug.WriteLine(e.Message)
                End Try
            End If

        End Sub

        Private Function CanEdit() As Boolean
            If (Me.Editable = False) Then Return False
            If (Me.m_layerSelected Is Nothing) Then Return False
            If (Me.m_layerSelected.Editor.IsEditable = False) Then Return False
            If (Me.m_layerSelected.Renderer.IsVisible = False) Then Return False
            Return True
        End Function

#End Region ' Internals

#Region " Layers "

        Public Event LayerAdded(sender As ucMap, layer As cLayer)
        Public Event LayerRemoved(sender As ucMap, layer As cLayer)

        Public Sub Clear()

            ' Unplug background image
            If (Me.m_bmp IsNot Nothing) Then
                Me.BackgroundImage = Nothing
                Me.m_bmp.Dispose()
                Me.m_bmp = Nothing
            End If

            ' Clean up layers to prevent dangling event handlers, which in turn keep disposed objects alive.
            Dim alayers As cLayer() = Me.m_layers.ToArray()
            For iLayer As Integer = 0 To alayers.Length - 1
                Me.RemoveLayer(alayers(iLayer))
            Next
            ' Should be neatly cleaned out
            Debug.Assert(m_layers.Count = 0)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer to the basemap.
        ''' </summary>
        ''' <param name="layer">The layer to add.</param>
        ''' <param name="layerPosition">The layer to add the layer before, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(ByVal layer As cLayer, _
                            Optional ByVal layerPosition As cLayer = Nothing)

            ' Sanity check
            If (layer Is Nothing) Then Return

            If layerPosition IsNot Nothing Then
                Me.m_layers.Insert(Me.m_layers.IndexOf(layerPosition), layer)
            Else
                Me.m_layers.Add(layer)
            End If

            AddHandler layer.LayerChanged, AddressOf Me.OnLayerChanged

            ' Manually update selected state on new layers
            If layer.IsSelected Then Me.UpdateSelection(layer)

            Try
                RaiseEvent LayerAdded(Me, layer)
            Catch ex As Exception
                cLog.Write(ex, "ucMap " & Me.Name & "::AddLayer")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a layer from the basemap.
        ''' </summary>
        ''' <param name="layer">The layer to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveLayer(ByVal layer As cLayer)

            ' Sanity check
            If (layer Is Nothing) Then Return

            RemoveHandler layer.LayerChanged, AddressOf Me.OnLayerChanged

            ' Clear selection
            If Object.ReferenceEquals(layer, Me.m_layerSelected) Then
                Me.m_layerSelected = Nothing
                Me.UpdateCursorFeedback()
            End If

            Me.m_layers.Remove(layer)

            Try
                RaiseEvent LayerRemoved(Me, layer)
            Catch ex As Exception
                cLog.Write(ex, "ucMap " & Me.Name & "::RemoveLayer")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get all layers currently active in the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Layers() As cLayer()
            Get
                Return Me.m_layers.ToArray
            End Get
        End Property

#End Region ' Layers

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the width and height of a cell.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetCellSize() As SizeF
            Return New SizeF(CSng(Me.Width / Me.Basemap.InCol), CSng(Me.Height / Me.Basemap.InRow))
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the cell screen rectangle of a cell, given its index.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetCellRect(ByVal ptCellIndex As Point) As Rectangle

            Dim ptCell As Point = Me.GetCellPos(ptCellIndex)
            Dim szCell As SizeF = Me.GetCellSize()

            Return New Rectangle( _
                    ptCell.X, _
                    ptCell.Y, _
                    CInt(Math.Ceiling(szCell.Width)), _
                    CInt(Math.Ceiling(szCell.Height)) _
            )

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the top left screen coordinates of a cell, given its index.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetCellPos(ByVal ptCellIndex As Point) As Point

            Dim szCell As SizeF = Me.GetCellSize()
            Return New Point( _
                    CInt(Math.Floor((ptCellIndex.X - 1) * szCell.Width)), _
                    CInt(Math.Floor((ptCellIndex.Y - 1) * szCell.Height)) _
            )

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the index of a cell, based on a given screen point.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetCellIndex(ByVal ptScreen As Point) As Point

            Dim szCell As SizeF = Me.GetCellSize()
            Dim iColIndex As Integer = CInt((ptScreen.X + 0.5 * szCell.Width) / szCell.Width)
            Dim iRowIndex As Integer = CInt((ptScreen.Y + 0.5 * szCell.Height) / szCell.Height)

            ' Truncate
            iRowIndex = Math.Max(Math.Min(iRowIndex, Me.Basemap.InRow), 1)
            iColIndex = Math.Max(Math.Min(iColIndex, Me.Basemap.InCol), 1)

            Return New Point(iColIndex, iRowIndex)

        End Function

#End Region ' Helper methods

    End Class

End Namespace

