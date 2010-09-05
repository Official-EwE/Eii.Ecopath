#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ScientificInterface.Ecospace.Basemap
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Win32Api
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Control that provides an interface to a series core data map layers.
    ''' </summary>
    ''' <remarks>
    ''' To provide zoom functionality, use <see cref="ucMapZoom">ucMapZoom</see>.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Public Class ucMap

        ''' <summary>The bitmap to draw on.</summary>
        Private m_bmp As Bitmap
        ''' <summary>The basemap.</summary>
        Private m_basemap As cEcospaceBasemap = Nothing
        ''' <summary>List of layers.</summary>
        Private m_layers As New List(Of cLayer)
        ''' <summary>Selected layer</summary>
        Private m_layerSelected As cLayer = Nothing
        ''' <summary>Flag stating that map needs updating on next redraw.</summary>
        Private m_bNeedsUpdate As Boolean = False

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

#Region " Public interfaces "

        Public Function SaveToBitmap(ByVal strFileName As String, ByVal format As System.Drawing.Imaging.ImageFormat) As Boolean

            Dim szCellSize As SizeF = Me.GetCellSize()
            Try
                Dim bmp As New Bitmap(CInt(Me.m_basemap.InCol * szCellSize.Width), CInt(Me.m_basemap.InRow * szCellSize.Height))
                Me.UpdateMap(bmp, New Point(1, 1), New Point(Me.m_basemap.InCol, Me.m_basemap.InRow))
                bmp.Save(strFileName, format)
            Catch ex As Exception
                Return False
            End Try
            Return True

        End Function

#End Region ' Public interfaces

#Region " Public properties "

        Public Property Basemap() As cEcospaceBasemap
            Get
                Return Me.m_basemap
            End Get
            Set(ByVal value As cEcospaceBasemap)

                If (Me.m_basemap IsNot Nothing) Then
                End If

                Me.m_basemap = value

                If (Me.m_basemap IsNot Nothing) Then
                End If

                Me.Refresh()
            End Set
        End Property

        Public Overrides Sub Refresh()
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
                If (Me.m_basemap Is Nothing) Then Return 20
                Return Me.m_basemap.InCol
            End Get
        End Property

        Public ReadOnly Property NumRows() As Integer
            Get
                If (Me.m_basemap Is Nothing) Then Return 20
                Return Me.m_basemap.InRow
            End Get
        End Property

#End Region ' Public properties

#Region " Event handlers "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clean-up.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ucBaseMap_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Disposed
            Me.Clear()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Paint handler; selectively redraws the bitmap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)

            If Object.ReferenceEquals(Me.m_bmp, Nothing) Then Return

            If (Me.m_bNeedsUpdate = True) Then
                Me.m_bNeedsUpdate = False
                Me.UpdateMap(Me.m_bmp, New Point(1, 1), New Point(Me.m_basemap.InCol, Me.m_basemap.InRow))
            End If

            ' Draw only invalidated area
            e.Graphics.DrawImage(Me.m_bmp, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse down handler; intializes map drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)

            Dim bShiftPressed As Boolean = (User32.GetAsyncKeyState(&H10) < 0)
            Dim ptCellCur As Point = Me.GetCellIndex(New Point(e.X, e.Y))

            If (Me.CanEdit = False) Then Return

            Me.Capture = True

            ' If NOT Shift key pressed, release the last mouse pos
            If Not bShiftPressed Then Me.m_ptScreenPrevious = Nothing

            ' Start editing
            Me.m_layerSelected.Editor.StartEdit(ptCellCur, e)

            Me.ProcessMouseInput(e)

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

            ' Create new bitmap
            Me.m_bmp = New Bitmap(Me.Width, Me.Height)

            ' Sanity check
            If Object.ReferenceEquals(Me.m_basemap, Nothing) Then Return

            ' Redraw it entirely
            Me.UpdateMap()
            ' Update cursor
            Me.UpdateCursorFeedback()
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
                Me.UpdateMap(True)
            End If

            If ((cf And (cLayer.eChangeFlags.Editable Or cLayer.eChangeFlags.Selected)) > 0) Then
                ' Refresh edit environment
                Me.UpdateCursorFeedback()
            End If

        End Sub

#End Region ' Event handlers

#Region " Internals "

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

            If ((e.Button And Windows.Forms.MouseButtons.Right) > 0) Then
                Me.m_layerSelected.Editor.Pickup(Me.GetCellIndex(e.Location))
            End If

            If ((e.Button And MouseButtons.Left) > 0) Then

                Dim ptCellFrom As Point = Me.GetCellIndex(Me.m_ptScreenPrevious)
                Dim ptCellTo As Point = Me.GetCellIndex(ptScreenCur)
                Dim ptUpdateMin As New Point(Math.Min(ptCellFrom.X, ptCellTo.X), Math.Min(ptCellFrom.Y, ptCellTo.Y))
                Dim ptUpdateMax As New Point(Math.Max(ptCellFrom.X, ptCellTo.X), Math.Max(ptCellFrom.Y, ptCellTo.Y))

                Me.m_layerSelected.Editor.Edit(ptCellFrom, ptCellTo, e, ptUpdateMin, ptUpdateMax)

                ' Flag layer as changed
                Me.m_layerSelected.IsModified = True

                Me.UpdateMap(Me.m_bmp, ptUpdateMin, ptUpdateMax)

            End If

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
        Private Sub UpdateMap(Optional ByVal bInvalidateOnly As Boolean = True)

            ' Sanity check
            If Object.ReferenceEquals(Me.m_basemap, Nothing) Then Return

            If (bInvalidateOnly = True) Then
                ' Only invalidate map
                Me.m_bNeedsUpdate = True
                Me.Invalidate()
            Else
                ' Update entire map
                Me.UpdateMap(Me.m_bmp, New Point(1, 1), New Point(Me.m_basemap.InCol, Me.m_basemap.InRow))
            End If

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
            If Object.ReferenceEquals(Me.m_basemap, Nothing) Then Return

            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim l As cLayer = Nothing
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            Dim ldDepth As cEcospaceLayer = Me.m_basemap.LayerDepth()
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
            iXTo = Math.Min(Me.m_basemap.InCol, Math.Max(ptCellFrom.X, ptCellTo.X) + 1)
            iYTo = Math.Min(Me.m_basemap.InRow, Math.Max(ptCellFrom.Y, ptCellTo.Y) + 1)

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
            If Object.ReferenceEquals(Me.m_basemap, Nothing) Then Return

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
            If Object.ReferenceEquals(Me.m_basemap, Nothing) Then Return

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

        Public Sub Clear()
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
            If layer Is Nothing Then Debug.Assert(False, "Need valid layer")

            If layerPosition IsNot Nothing Then
                Me.m_layers.Insert(Me.m_layers.IndexOf(layerPosition), layer)
            Else
                Me.m_layers.Add(layer)
            End If

            AddHandler layer.LayerChanged, AddressOf Me.OnLayerChanged
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a layer from the basemap.
        ''' </summary>
        ''' <param name="layer">The layer to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveLayer(ByVal layer As cLayer)

            ' Sanity check
            If layer Is Nothing Then Debug.Assert(False, "Need valid layer")

            RemoveHandler layer.LayerChanged, AddressOf Me.OnLayerChanged

            ' Clear selection
            If Object.ReferenceEquals(layer, Me.m_layerSelected) Then
                Me.m_layerSelected = Nothing
                Me.UpdateCursorFeedback()
            End If

            Me.m_layers.Remove(layer)

        End Sub

#End Region ' Layers

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the width and height of a cell.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Function GetCellSize() As SizeF
            Return New SizeF(CSng(Me.Width / Me.m_basemap.InCol), CSng(Me.Height / Me.m_basemap.InRow))
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
            iRowIndex = Math.Max(Math.Min(iRowIndex, Me.m_basemap.InRow), 1)
            iColIndex = Math.Max(Math.Min(iColIndex, Me.m_basemap.InCol), 1)

            Return New Point(iColIndex, iRowIndex)

        End Function

#End Region ' Helper methods

    End Class

End Namespace

