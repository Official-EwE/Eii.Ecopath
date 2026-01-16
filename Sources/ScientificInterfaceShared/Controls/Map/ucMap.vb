' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'



Option Explicit On


Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Security.Permissions
Imports EwECore.Style
Imports EwEUtils.Logging
Imports EwEUtils.Utilities
Imports Microsoft.Extensions.Logging
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style
Imports Debug = System.Diagnostics.Debug




#Const DRAW_THREADED = 0

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
        ''' <summary>Map title.</summary>
        Private m_strTitle As String = ""
        ''' <summary>List of layers.</summary>
        Private m_layers As New List(Of cDisplayLayer)
        ''' <summary>Selected layer</summary>
        Private m_layerSelected As cDisplayLayer = Nothing

        ' JS New map logic Dec 18--
        Private m_zoom As Single = 1
        Private m_maprect As Rectangle
        Private m_cellsize As Single = 0
        Private ReadOnly m_logger As ILogger = LoggingContext.CreateLogger(Of ucMap)()

        Public Event OnMapScrolled(sender As Object, args As EventArgs)
        ' -- JS New map logic Dec 18

        Public Sub New()

            Me.InitializeComponent()

            ' Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)
            Me.SetStyle(ControlStyles.UserPaint, True)

            Me.BackColor = Color.White

        End Sub

        ''' <inheritdocs cref="IUIElement.UIContext"/>
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(uic As cUIContext)
                If (Me.m_uic IsNot Nothing) Then
                    RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
                Me.m_uic = uic
                If (Me.m_uic IsNot Nothing) Then
                    AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf Me.OnStyleGuideChanged
                End If
                Me.Clear()
            End Set
        End Property

#Region " Public interfaces "

        Public Function SaveToBitmap(strFileName As String, format As System.Drawing.Imaging.ImageFormat) As Boolean

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Dim bm As cEcospaceBasemap = Me.Basemap
            Dim szCellSize As SizeF = Me.GetCellSize()
            Dim rc As Rectangle = Me.ClientRectangle
            Dim strFilenameLegend As String = ""

            Try
                Using bmp As Bitmap = sg.GetImage(rc.Width, rc.Height, format, strFileName)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        Me.DrawMap(g, rc)
                        Using extr As Bitmap = bmp.Clone(Rectangle.Intersect(rc, Me.m_maprect), bmp.PixelFormat)
                            extr.Save(strFileName, format)
                        End Using
                    End Using
                End Using

                Dim lgd As cLegend = cLegend.FromMap(Me)
                Dim strExt As String = Path.GetExtension(strFileName)

                strFilenameLegend = Path.Combine(Path.GetDirectoryName(strFileName), Path.GetFileNameWithoutExtension(strFileName) & "_legend" & strExt)
                lgd.Save(strFilenameLegend, format)

                ' ToDo: globalize this
                Dim msg As New cMessage(String.Format("Map image has been saved to {0}, legend to {1}", strFileName, strFilenameLegend),
                                        eMessageType.DataExport, eCoreComponentType.Ecospace, eMessageImportance.Information)
                msg.Hyperlink = Path.GetDirectoryName(strFileName)
                Me.m_uic.Core.Messages.SendMessage(msg)
            Catch ex As Exception
                m_logger.LogError(ex, "ucMap(" & Me.Name & ")::SaveToBitmap(" & strFileName & ")")
            End Try

            Return True

        End Function

#End Region ' Public interfaces

#Region " Public properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the map title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True)>
        <Category("Appearance")>
        <Description("Title of the map to display")>
        Public Property Title() As String
            Get
                Return Me.m_strTitle
            End Get
            Set(strTitle As String)
                Me.m_strTitle = strTitle
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the map.
        ''' </summary>
        ''' <remarks>Redrawing the map entirely may be slow!</remarks>
        ''' -------------------------------------------------------------------
        Public Overloads Sub Refresh()
            Me.UpdateMap()
        End Sub

        Public Property Editable() As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Update the entire map image.
        ''' </summary>
        ''' <remarks>
        ''' This will invalidate the entire map screen area.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub UpdateMap()
            ' Invalidate entirely
            Me.Invalidate()
        End Sub

        Public Sub UpdateMap(ptCellFrom As Point, ptCellTo As Point)

            If (ptCellFrom = ptCellTo) Then Return

            Dim ptTL As Point = Me.ColRowToPoint(ptCellFrom)
            Dim ptBR As Point = Me.ColRowToPoint(ptCellTo)
            ' ToDO: Invalidate selectively
            'Me.Invalidate(New Rectangle(ptTL.X, ptTL.Y, ptBR.X - ptTL.X, ptBR.Y - ptTL.Y))
            Me.Invalidate()
        End Sub

#End Region ' Public properties

#Region " Event handlers "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.CalcMapSize()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Clean-up.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub ucBaseMap_Disposed(sender As Object, e As System.EventArgs) _
            Handles Me.Disposed
            Me.Clear()
        End Sub

        Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
            MyBase.OnMouseWheel(e)
            Me.Zoom(e.Location) += e.Delta / 300.0!
        End Sub

        Protected Overrides Sub OnSizeChanged(e As System.EventArgs)
            MyBase.OnSizeChanged(e)
            Me.CalcMapSize()
            Me.Invalidate()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Paint handler; selectively redraws the bitmap.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnPaint(e As PaintEventArgs)

            If (Me.Basemap Is Nothing) Then Return

            Try
                Me.DrawMap(e.Graphics, e.ClipRectangle)
            Catch ex As Exception
                Me.ResetExceptionState(Me)
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse down handler; intializes map drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseDown(e As MouseEventArgs)

            Dim bm As cEcospaceBasemap = Me.Basemap

            If (Me.CanEdit = False) Then Return
            If (Not Me.m_maprect.Contains(e.Location)) Then Return

            Dim edt As cLayerEditor = Me.m_layerSelected.Editor

            ' It's up to the editor to start editing
            edt.ProcessMouseClick(e, Me)

            If (edt.IsEditing) Then
                Me.Capture = True
            End If

            Me.UpdateMap()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse move handler; performs a map drawing step.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseMove(e As MouseEventArgs)

            If (Me.UIContext Is Nothing) Then Return

            ' Get value in selected layer
            Dim bm As cEcospaceBasemap = Me.Basemap
            Dim l As cDisplayLayer = Me.m_layerSelected

            If (Me.CanEdit) Then
                Me.Cursor = Me.m_layerSelected.Editor.Cursor(e.Location, Me)
            Else
                Me.Cursor = Cursors.Default
            End If

            If (Me.CanEdit And Me.Capture) Then
                Me.ProcessMouseMove(e)
            End If

            Dim strVal As String = ""
            Dim strFeedback As String = ""
            Dim ptCell As Point = Me.PointToColRow(e.Location)
            Dim ptCoord As PointF = Me.PointToLonLat(e.Location)

            If (l IsNot Nothing) Then
                ' Only show values for water cells or actual land cells for the land layer
                If (TypeOf l Is cDisplayLayerRaster) Then
                    Dim dl As cDisplayLayerRaster = DirectCast(l, cDisplayLayerRaster)
                    If (dl.VarName = eVarNameFlags.LayerDepth) Or (bm.IsModelledCell(ptCell.Y, ptCell.X)) Then
                        strVal = l.Renderer.GetDisplayText(dl.Value(ptCell.Y, ptCell.X))
                    End If
                End If
            End If

            Dim strLat As String = Me.UIContext.StyleGuide.FormatNumber(ptCoord.Y)
            Dim strLon As String = Me.UIContext.StyleGuide.FormatNumber(ptCoord.X)
            Dim fmt As New cMapUnitFormatter()
            Dim strUnit As String = If(bm.AssumeSquareCells,
                                       fmt.ToString(eUnitMapRefType.m, eDescriptorTypes.Symbol),
                                       fmt.ToString(eUnitMapRefType.dd, eDescriptorTypes.Symbol))

            If Not String.IsNullOrWhiteSpace(strVal) Then
                strFeedback = String.Format(My.Resources.GENERIC_VALUE_MAPPOS_VALUE,
                                            strLon, strLat, strUnit,
                                            ptCell.Y, ptCell.X, strVal)
            Else
                strFeedback = String.Format(My.Resources.GENERIC_VALUE_MAPPOS,
                                            strLon, strLat, strUnit,
                                            ptCell.Y, ptCell.X)
            End If

            cApplicationStatusNotifier.UpdateStatus(Me.m_uic.Core, strFeedback)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Mouse up handler; finalizes map drawing.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub OnMouseUp(e As MouseEventArgs)

            If (Me.UIContext Is Nothing) Then Return

            If (Me.CanEdit = False) Then Return
            If (Me.Capture = False) Then Return

            Me.m_layerSelected.Editor.ProcessMouseUp()

            ' Process pending layer changes
            For Each l As cDisplayLayer In Me.m_layers
                If (TypeOf l Is cDisplayLayerRaster) Then
                    Dim rl As cDisplayLayerRaster = DirectCast(l, cDisplayLayerRaster)
                    If rl.IsModified Then rl.Update(cDisplayLayer.eChangeFlags.Map) : rl.IsModified = False
                End If
            Next

            Me.Capture = False

        End Sub

        Protected Overrides Sub OnMouseLeave(e As System.EventArgs)
            MyBase.OnMouseLeave(e)

            If (Me.UIContext Is Nothing) Then Return
            cApplicationStatusNotifier.UpdateStatus(Me.m_uic.Core, "")

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Layer changed event
        ''' </summary>
        ''' <param name="l">The layer that changed</param>
        ''' -------------------------------------------------------------------
        Private Sub OnLayerChanged(l As cDisplayLayer, cf As cDisplayLayer.eChangeFlags)

            ' Ignore sole descriptive layer changes
            If (cf = cDisplayLayer.eChangeFlags.Descriptive) Then Return

            ' Handle selection changes
            If ((cf And cDisplayLayer.eChangeFlags.Selected) > 0) Then
                ' Update selection
                Me.UpdateSelection(l)
            End If

            If ((cf And (cDisplayLayer.eChangeFlags.Map Or
                                 cDisplayLayer.eChangeFlags.Visibility Or
                                 cDisplayLayer.eChangeFlags.VisualStyle Or
                                 cDisplayLayer.eChangeFlags.Selected)) > 0) Then
                ' Update Map
                Me.UpdateMap()
            End If

            If ((cf And (cDisplayLayer.eChangeFlags.Editable Or cDisplayLayer.eChangeFlags.Selected)) > 0) Then
                ' Refresh edit environment
                ' Nothing to do right now...
            End If

        End Sub

        Private Sub OnStyleGuideChanged(ct As cStyleGuide.eChangeType)
            If (ct And cStyleGuide.eChangeType.Colours) > 0 Then
                Me.UpdateMap()
            End If
        End Sub

#End Region ' Event handlers

#Region " Internals "

        ''' <summary>
        ''' May be needed from the outside (by layer editors, for instance)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Basemap As cEcospaceBasemap
            Get
                If (Me.m_uic Is Nothing) Then Return Nothing
                Return Me.m_uic.Core.EcospaceBasemap
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Performs a draw step by updating the memory bitmap.
        ''' </summary>
        ''' <param name="e"></param>
        ''' -------------------------------------------------------------------
        Private Sub ProcessMouseMove(e As MouseEventArgs)

            If (Me.CanEdit = False) Then Return
            If (Me.Capture = False) Then Return

            Me.m_layerSelected.Editor.ProcessMouseDraw(e, Me)

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
        Private Sub DrawMap(g As Graphics, rcClip As Rectangle)

            ' Sanity check
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm Is Nothing) Then Return

            Dim l As cDisplayLayer = Nothing
            Dim style As cStyleGuide.eStyleFlags = cStyleGuide.eStyleFlags.OK
            Dim layDepth As cEcospaceLayerDepth = Me.Basemap.LayerDepth()
            Dim layExcl As cEcospaceLayerExclusion = Me.Basemap.LayerExclusion()
            Dim szCell As SizeF = Me.GetCellSize()
            Dim ptCell As Point = Nothing
            Dim bRenderCell As Boolean = False

            ' Clear the area! Nothing to see here! Move on now, be a good lad.
            Using br As New SolidBrush(Me.BackColor)
                g.FillRectangle(br, rcClip)
            End Using

            ' Clear the area! Nothing to see here! Move on now, be a good lad.
            Using br As New SolidBrush(Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND))
                g.FillRectangle(br, Me.m_maprect)
            End Using

            Dim ptfTL As PointF = Me.PointToColRowExact(rcClip.Location)
            Dim ptfBR As PointF = Me.PointToColRowExact(New Point(rcClip.Right, rcClip.Bottom))
            Dim ptDrawTL As New Point(CInt(Math.Floor(Math.Max(1, ptfTL.X))), CInt(Math.Floor(Math.Max(1, ptfTL.Y))))
            Dim ptDrawBR As New Point(CInt(Math.Ceiling(Math.Min(bm.InCol, ptfBR.X))), CInt(Math.Ceiling(Math.Min(bm.InRow, ptfBR.Y))))

            Dim layers As New List(Of cDisplayLayer)
            Dim displayDepth As cDisplayLayer = Nothing
            Dim dtGroup As eDataTypes = Nothing

            If (Me.m_layerSelected IsNot Nothing) Then
                If (Me.m_layerSelected.RenderMode = Definitions.eLayerRenderType.Grouped) Then
                    If (TypeOf Me.m_layerSelected Is cDisplayLayerRaster) Then
                        dtGroup = DirectCast(Me.m_layerSelected, cDisplayLayerRaster).Data.DataType
                    End If
                End If
            End If

            For Each l In Me.m_layers

                Dim bDrawLayer As Boolean = (l.Renderer.IsVisible)

                If (TypeOf l Is cDisplayLayerRaster) Then

                    Dim rl As cDisplayLayerRaster = DirectCast(l, cDisplayLayerRaster)
                    Dim dt As eDataTypes = rl.Data.DataType

                    Select Case rl.RenderMode
                        Case Definitions.eLayerRenderType.Always
                            ' NOP
                        Case Definitions.eLayerRenderType.Selected
                            bDrawLayer = bDrawLayer And rl.IsSelected()
                        Case Definitions.eLayerRenderType.Grouped
                            If (rl.Data.DataType = dtGroup) And (dtGroup <> eDataTypes.NotSet) Then
                                ' NOP
                            Else
                                bDrawLayer = False
                            End If
                    End Select

                    ' Special cases
                    If (dt = eDataTypes.EcospaceLayerExclusion And Me.UIContext.StyleGuide.ShowMapsExcludedCells) Then
                        bDrawLayer = True
                    End If

                    If (dt = eDataTypes.EcospaceLayerDepth) Then
                        displayDepth = rl
                        bDrawLayer = True
                    End If

                End If

                If (l.RenderMode = Definitions.eLayerRenderType.Always) Then
                    bDrawLayer = True
                End If

                If bDrawLayer Then layers.Add(l)
            Next

            ' Draw raster layers in reverse order
            For iLayer As Integer = layers.Count - 1 To 0 Step -1

                l = layers(iLayer)
                If (l.Renderer.IsVisible) Then

                    If (TypeOf l Is cDisplayLayerRaster) Then

                        Dim rl As cDisplayLayerRaster = DirectCast(l, cDisplayLayerRaster)
                        Dim dt As eDataTypes = rl.Data.DataType

                        If (rl.HasData) Then

                            For X As Integer = ptDrawTL.X To ptDrawBR.X
                                For Y As Integer = ptDrawTL.Y To ptDrawBR.Y

                                    ptCell = New Point(X, Y)
                                    Dim rcCell As RectangleF = Me.GetCellRect(ptCell)

                                    Select Case dt
                                        Case eDataTypes.EcospaceLayerExclusion,
                                             eDataTypes.EcospaceLayerDepth,
                                             eDataTypes.EcospaceLayerPort
                                            bRenderCell = True
                                        Case Else
                                            bRenderCell = layDepth.IsWaterCell(Y, X) And CBool(layExcl.Cell(Y, X)) = False
                                    End Select

                                    If bRenderCell Then
                                        Dim objValue As Object = rl.Value(ptCell.Y, ptCell.X)
                                        If rl.IsValue(objValue) Then
                                            ' Build style flags
                                            style = cStyleGuide.eStyleFlags.OK
                                            If l.IsSelected Or l.RenderMode = Definitions.eLayerRenderType.Always Then
                                                style = (style Or cStyleGuide.eStyleFlags.Highlight)
                                            End If
                                            ' Render cell
                                            DirectCast(l.Renderer, cRasterLayerRenderer).RenderCell(g, rcCell, rl.Data, objValue, style)
                                        End If
                                    End If

                                Next Y
                            Next X
                        End If

                    ElseIf (TypeOf l.Renderer Is cVectorLayerRenderer) Then
                        style = cStyleGuide.eStyleFlags.OK
                        If l.IsSelected Then style = (style Or cStyleGuide.eStyleFlags.Highlight)

                        DirectCast(l.Renderer, cVectorLayerRenderer).Render(g, l, Me.m_maprect, Me.Basemap.PosTopLeft, Me.Basemap.PosBottomRight, style)

                    End If
                End If
            Next iLayer

            ' Draw map outer border
            g.DrawRectangle(Pens.LightGray, Me.m_maprect)

        End Sub

        Private Sub UpdateSelection(l As cDisplayLayer)

            ' Sanity check
            If Me.Basemap Is Nothing Then Return

            ' New selection?
            If l.IsSelected Then
                ' #Yes: set selected layer
                Me.m_layerSelected = l
            Else
                ' #No: current selection being cleared?
                If ReferenceEquals(Me.m_layerSelected, l) Then
                    ' #Yes: clear selection
                    Me.m_layerSelected = Nothing
                End If
            End If

        End Sub

        Private Function CanEdit() As Boolean

            If (Me.Editable = False) Then Return False
            If (Me.m_layerSelected Is Nothing) Then Return False
            If (Me.m_layerSelected.Renderer.IsVisible = False) Then Return False

            If (Me.m_layerSelected.Editor IsNot Nothing) Then
                Return Me.m_layerSelected.Editor.IsEditable
            End If

            Return False

        End Function

        <ReflectionPermission(SecurityAction.Demand, MemberAccess:=True)>
        Private Sub ResetExceptionState(control As Control)
            ' Reset exception state on drawing errors
            Dim args() As Object = {&H400000, False}
            GetType(Control).InvokeMember("SetState",
                                          BindingFlags.NonPublic Or BindingFlags.InvokeMethod Or BindingFlags.Instance,
                                          Nothing, control, args)
        End Sub

#End Region ' Internals

#Region " Layers "

        Public Event LayerAdded(sender As ucMap, layer As cDisplayLayer)
        Public Event LayerRemoved(sender As ucMap, layer As cDisplayLayer)

        Public Sub Clear()

            ' Clean up layers to prevent dangling event handlers, which in turn keep disposed objects alive.
            Dim alayers As cDisplayLayer() = Me.m_layers.ToArray()
            For iLayer As Integer = 0 To alayers.Length - 1
                Me.RemoveLayer(alayers(iLayer))
            Next

            ' Should be neatly cleaned out
            Debug.Assert(Me.m_layers.Count = 0)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer to the basemap.
        ''' </summary>
        ''' <param name="layer">The layer to add.</param>
        ''' <param name="layerPosition">The layer to add the layer before, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(layer As cDisplayLayer,
                            Optional layerPosition As cDisplayLayer = Nothing)

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
                m_logger.LogError(ex, "ucMap " & Me.Name & "::AddLayer")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Remove a layer from the basemap.
        ''' </summary>
        ''' <param name="layer">The layer to remove.</param>
        ''' -------------------------------------------------------------------
        Public Sub RemoveLayer(layer As cDisplayLayer)

            ' Sanity check
            If (layer Is Nothing) Then Return

            RemoveHandler layer.LayerChanged, AddressOf Me.OnLayerChanged

            ' Clear selection
            If ReferenceEquals(layer, Me.m_layerSelected) Then
                Me.m_layerSelected = Nothing
            End If

            Me.m_layers.Remove(layer)

            Try
                RaiseEvent LayerRemoved(Me, layer)
            Catch ex As Exception
                m_logger.LogError(ex, "ucMap " & Me.Name & "::RemoveLayer")
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get all layers currently active in the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property Layers() As cDisplayLayer()
            Get
                Return Me.m_layers.ToArray
            End Get
        End Property

#End Region ' Layers

#Region " Helper methods "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the width and height of a cell in pixels, as drawn in the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetCellSize() As SizeF
            Return New SizeF(Me.m_cellsize, Me.m_cellsize)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the control rectangle of a cell in pixels, given its index.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function GetCellRect(ptCellIndex As Point) As RectangleF
            Return New RectangleF(Me.m_maprect.X + Me.m_cellsize * (ptCellIndex.X - 1),
                                  Me.m_maprect.Y + Me.m_cellsize * (ptCellIndex.Y - 1), Me.m_cellsize, Me.m_cellsize)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the map column and row at a given point in the map contol
        ''' </summary>
        ''' <param name="ptScreen"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function PointToColRow(ptScreen As Point) As Point

            Dim pt As New Point(0, 0)
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm IsNot Nothing) Then
                pt.X = CInt(Math.Floor(Math.Min(bm.InCol, Math.Max(1, 1 + (ptScreen.X - Me.m_maprect.X) / Me.m_cellsize))))
                pt.Y = CInt(Math.Floor(Math.Min(bm.InRow, Math.Max(1, 1 + (ptScreen.Y - Me.m_maprect.Y) / Me.m_cellsize))))
            End If
            Return pt
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the exact map column and row position at a given map contrl location.
        ''' </summary>
        ''' <param name="ptScreen"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function PointToColRowExact(ptScreen As Point) As PointF

            Dim ptf As New PointF(0, 0)
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm IsNot Nothing) Then
                ptf.X = Math.Min(bm.InCol + 1, Math.Max(1, 1 + (ptScreen.X - Me.m_maprect.X) / Me.m_cellsize))
                ptf.Y = Math.Min(bm.InRow + 1, Math.Max(1, 1 + (ptScreen.Y - Me.m_maprect.Y) / Me.m_cellsize))
            End If
            Return ptf

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Returns the map control location represented by a basemap location.
        ''' </summary>
        ''' <param name="ptfMap"></param>
        ''' -------------------------------------------------------------------
        Public Function ColRowToPoint(ptfMap As PointF) As Point

            Return New Point(CInt((ptfMap.X - 1) * Me.m_cellsize) + Me.m_maprect.X,
                             CInt((ptfMap.Y - 1) * Me.m_cellsize) + Me.m_maprect.Y)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a map location (col, row) to a georeferenced coordinate (lon, lat) 
        ''' <seealso cref="cEcospaceBasemap.ColToLon(Single)"/>
        ''' <seealso cref="cEcospaceBasemap.RowToLat(Single)"/>
        ''' <seealso cref="LonLatToColRow"/>
        ''' </summary>
        ''' <param name="ptf"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function ColRowToLonLat(ptf As PointF) As PointF
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm IsNot Nothing) Then
                ptf.X = bm.ColToLon(ptf.X)
                ptf.Y = bm.RowToLat(ptf.Y)
            End If
            Return ptf
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a map location (col, row) to a georeferenced coordinate (lon, lat) 
        ''' <seealso cref="cEcospaceBasemap.LonToCol(Single)"/>
        ''' <seealso cref="cEcospaceBasemap.LatToRow(Single)"/>
        ''' <seealso cref="ColRowToLonLat"/>
        ''' </summary>
        ''' <param name="ptf"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function LonLatToColRow(ptf As PointF) As PointF
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm IsNot Nothing) Then
                ptf.X = bm.LonToCol(ptf.X)
                ptf.Y = bm.LatToRow(ptf.Y)
            End If
            Return ptf
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Convert a map control point (in pixels) to a lon, lat coordinate.
        ''' <seealso cref="PointToColRowExact(Point)"/>
        ''' <seealso cref="PointToColRow(Point)"/>
        ''' <seealso cref="ColRowToLonLat(PointF)"/>
        ''' </summary>
        ''' <param name="ptScreen"></param>
        ''' -------------------------------------------------------------------
        Public Function PointToLonLat(ptScreen As Point) As PointF
            Return Me.ColRowToLonLat(Me.PointToColRowExact(ptScreen))
        End Function

        ''' <summary>
        ''' Smack!
        ''' </summary>
        ''' <param name="ptfLonLat"></param>
        ''' <returns></returns>
        Public Function LonLatToPoint(ptfLonLat As PointF) As Point
            Return Me.ColRowToPoint(Me.LonLatToColRow(ptfLonLat))
        End Function

        Public Function MapUnits() As String
            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm IsNot Nothing) Then
                Return bm.Units
            End If
            Return ""
        End Function

        Public Property Zoom(Optional ptLocation As Point = Nothing) As Single
            Get
                Return Me.m_zoom
            End Get
            Set(value As Single)

                Dim bm As cEcospaceBasemap = Me.Basemap
                If (bm Is Nothing) Then Return

                If (ptLocation = Nothing) Then ptLocation = Me.CenterPoint

                Dim ptf1 As PointF = Me.PointToColRowExact(ptLocation)
                Me.m_zoom = Math.Max(Me.MinZoom, Math.Min(Me.MaxZoom, value))
                Me.CalcMapSize()
                Dim ptf2 As PointF = Me.PointToColRowExact(ptLocation)
                Dim dx As New Point(CInt(Me.m_cellsize * (ptf2.X - ptf1.X)), CInt(Me.m_cellsize * (ptf2.Y - ptf1.Y)))
                Me.ScrollBy(dx)

            End Set
        End Property

        Public ReadOnly Property MinZoom As Single = 1.0!

        Public ReadOnly Property MaxZoom As Single
            Get
                Dim bm As cEcospaceBasemap = Me.Basemap
                If (bm Is Nothing) Then Return -1
                Return Math.Min(CInt(bm.InCol / 10), CInt(bm.InRow / 10))
            End Get
        End Property


#End Region ' Helper methods

#Region " JS New map logic dec18 "

        ''' <summary>
        ''' Get the scroll range for large and small change
        ''' </summary>
        ''' <remarks>
        ''' Small change typically range / 20
        ''' Large change typically range / 10
        ''' </remarks>
        ''' <returns></returns>
        Public ReadOnly Property ScrollRange As Size
            Get
                Dim sz As New Size(0, 0)
                Dim bm As cEcospaceBasemap = Me.Basemap

                If (bm IsNot Nothing) Then
                    sz.Width = Me.m_maprect.Width
                    sz.Height = Me.m_maprect.Height
                End If
                Return sz
            End Get
        End Property

        ''' <summary>
        ''' Get the scroll maximum values
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ScrollSize As Size
            Get
                Dim sz As New Size(0, 0)
                Dim bm As cEcospaceBasemap = Me.Basemap

                If (bm IsNot Nothing) Then
                    Dim rc As Rectangle = Me.ClientRectangle()
                    sz.Width = Math.Max(0, Me.m_maprect.Width - rc.Width)
                    sz.Height = Math.Max(0, Me.m_maprect.Height - rc.Height)
                End If
                Return sz
            End Get
        End Property

        ''' <summary>
        ''' Get the map scroll position
        ''' </summary>
        Public Property ScrollPos As Point
            Get
                Dim pt As New Point(0, 0)
                Dim bm As cEcospaceBasemap = Me.Basemap

                If (bm IsNot Nothing) Then
                    Dim rc As Rectangle = Me.ClientRectangle()
                    Dim sz As Size = Me.ScrollSize
                    pt.X = Math.Min(sz.Width, Me.m_maprect.X + sz.Width)
                    pt.Y = Math.Min(sz.Height, Me.m_maprect.Y + sz.Height)
                End If
                Return pt
            End Get
            Set(value As Point)
                Dim bm As cEcospaceBasemap = Me.Basemap

                If (bm IsNot Nothing) Then
                    Dim rc As Rectangle = Me.ClientRectangle()
                    Dim sz As Size = Me.ScrollSize

                    Dim x As Integer = value.X - sz.Width
                    Dim y As Integer = value.Y - sz.Height

                    Dim dx As Integer = Math.Min(Me.m_maprect.Width, rc.Width)
                    Dim dy As Integer = Math.Min(Me.m_maprect.Height, rc.Height)

                    Dim ptcenter As New Point(CInt(rc.X + rc.Width / 2), CInt(rc.Y + rc.Height / 2))
                    If (x > 0) Then
                        x = CInt(Math.Max(0, ptcenter.X - dx / 2))
                    End If
                    If (x < rc.Width - Me.m_maprect.Width) Then
                        x = CInt(Math.Min(rc.Width - Me.m_maprect.Width, ptcenter.X - dx / 2))
                    End If

                    If (y > 0) Then
                        y = CInt(Math.Max(0, ptcenter.Y - dy / 2))
                    End If
                    If (y < rc.Height - Me.m_maprect.Height) Then
                        y = CInt(Math.Min(rc.Height - Me.m_maprect.Height, ptcenter.Y - dy / 2))
                    End If

                    Me.m_maprect.X = x
                    Me.m_maprect.Y = y
                    Me.Invalidate()

                End If
            End Set
        End Property

        Private Sub CalcMapSize()

            Dim bm As cEcospaceBasemap = Me.Basemap
            If (bm Is Nothing) Then Return

            Dim rc As Rectangle = Me.ClientRectangle()
            Dim dx As Single = CSng(rc.Width / bm.InCol)
            Dim dy As Single = CSng(rc.Height / bm.InRow)

            Me.m_cellsize = Math.Min(dx, dy) * Me.m_zoom

            ' Position focus point at the center of the map
            Dim ptcenter As New Point(CInt(rc.X + rc.Width / 2), CInt(rc.Y + rc.Height / 2))

            ' Center map
            Me.m_maprect = New Rectangle(CInt(ptcenter.X - bm.InCol * Me.m_cellsize / 2.0!),
                                         CInt(ptcenter.Y - bm.InRow * Me.m_cellsize / 2.0!),
                                         CInt(bm.InCol * Me.m_cellsize),
                                         CInt(bm.InRow * Me.m_cellsize))

            Me.Invalidate()

        End Sub

        Private Sub ScrollBy(pt As Point)

            ' ToDo: limit to visible area
            Dim pts As Point = Me.ScrollPos()
            pts.X += pt.X
            pts.Y += pt.Y
            Me.ScrollPos = pts

            Try
                RaiseEvent OnMapScrolled(Me, New EventArgs())
            Catch ex As Exception

            End Try
        End Sub

        Private Function CenterPoint() As Point
            Dim rc As Rectangle = Me.ClientRectangle()
            Return New Point(CInt(rc.X + rc.Width / 2), CInt(rc.Y + rc.Height / 2))
        End Function

#End Region ' JS New map logic dec18

    End Class

End Namespace