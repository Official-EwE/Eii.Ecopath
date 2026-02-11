' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwEUtils.Utilities

Namespace Controls.Map.Layers

    Public MustInherit Class cLayerEditorRaster
        Inherits cLayerEditor

#Region " Private vars "

        ''' <summary>The current value 'under the cursor'.</summary>
        Private m_sValue As Single = 0
        ''' <summary>Draw helper flag: previous draw point.</summary>
        Private m_ptScreenPrevious As Point = Nothing

        ' === FEEDBACK SUPPORT ===
        Private Shared s_iCursorSize As Integer = 1

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(typeGUI As Type)
            MyBase.New(typeGUI)
        End Sub

#End Region ' Constructor

#Region " Initialization "

        Public Overrides Sub Initialize(uic As cUIContext, layer As cDisplayLayer)
            MyBase.Initialize(uic, layer)

            If (layer IsNot Nothing) Then
                Dim rl As cDisplayLayerRaster = DirectCast(layer, cDisplayLayerRaster)
                If (rl.Data IsNot Nothing) Then
                    Dim md As cVariableMetaData = rl.Data.MetadataCell
                    If (md IsNot Nothing) Then
                        Me.CellValueMin = md.Min
                        Me.CellValueMax = md.Max
                        Me.CellValue = md.NullValue
                    End If
                End If
            End If
        End Sub

#End Region ' Initialization

#Region " Properties "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value for the next cell that is to be edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property CellValue() As Object
            Get
                Return Me.m_sValue
            End Get
            Set(value As Object)
                Dim sValue As Single = Math.Max(Math.Min(CSng(value), Me.CellValueMax), Me.CellValueMin)
                If (sValue <> Me.m_sValue) Then
                    Me.m_sValue = sValue
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick up the cell value at a given point, and store this value in the
        ''' layer editor as the next value that will be set.
        ''' </summary>
        ''' <param name="pt">The cell location to pick up a value from.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Pickup(pt As Point)

            Try
                Me.CellValue = CDec(Me.Layer.Value(pt.Y, pt.X))
                If (Me.GUI IsNot Nothing) Then
                    Me.GUI.UpdateContent(CType(Me, cLayerEditorRaster))
                End If
            Catch ex As Exception
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the max value allowed in a cell.
        ''' </summary>
        ''' <remarks>
        ''' Ideally, this value would be obtained from core meta data. For now,
        ''' the UI is required to manually control this property.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property CellValueMax() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the min value allowed in a cell.
        ''' </summary>
        ''' <remarks>
        ''' Ideally, this value would be obtained from core meta data. For now,
        ''' the UI is required to manually control this property.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property CellValueMin() As Single

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the layer to attach to this Editor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overloads Property Layer() As cDisplayLayerRaster
            Get
                Return CType(MyBase.Layer, cDisplayLayerRaster)
            End Get
            Protected Set(value As cDisplayLayerRaster)
                MyBase.Layer = value
            End Set
        End Property

#End Region ' Properties

#Region " Cursor feedback "

        Protected Shared s_cursor As Cursor = Nothing
        Protected Shared s_lastsize As Single = -1.0!

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the size of the cursor.
        ''' </summary>
        ''' <remarks>
        ''' This value is persistent across layer editors.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Property CursorSize() As Integer
            Get
                Return s_iCursorSize
            End Get
            Set(iCursorSize As Integer)
                s_iCursorSize = iCursorSize
                If (s_cursor IsNot Nothing) Then
                    s_cursor.Dispose()
                    s_cursor = Nothing
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cursor feedback for the current location of the cursor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Function Cursor(ptMouse As Point, map As ucMap) As Cursor

            If (map.GetCellSize.Width <> s_lastsize) Then
                s_lastsize = map.GetCellSize.Width
                Me.CursorSize = Me.CursorSize
            End If

            If (s_cursor Is Nothing) Then
                Dim szCell As SizeF = map.GetCellSize()
                s_cursor = cLayerEditorRaster.EditorCursor(Me.CursorSize, szCell)
            End If
            Return s_cursor

        End Function

        Public Shared Function EditorCursor(iCursorSize As Integer, szCell As SizeF) As Cursor

            Dim ptIconSize As New Size(CInt(szCell.Width * iCursorSize), CInt(szCell.Height * iCursorSize))
            Dim cursor As Cursor = Cursors.Hand

            If (iCursorSize > 0) Then
                Try
                    Using bmp As New Bitmap(ptIconSize.Width + 1, ptIconSize.Height + 1)
                        Using g As Graphics = Graphics.FromImage(bmp)
                            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                            g.FillRectangle(Brushes.Transparent, New Rectangle(0, 0, bmp.Width, bmp.Height))
                            g.DrawEllipse(Pens.White, 1, 1, ptIconSize.Width - 2, ptIconSize.Height - 2)
                            g.DrawEllipse(Pens.Black, 0, 0, ptIconSize.Width, ptIconSize.Height)
                            Using br As New SolidBrush(Color.FromArgb(45, 0, 0, 0))
                                g.FillEllipse(br, 0, 0, ptIconSize.Width, ptIconSize.Height)
                            End Using
                            cursor = New Cursor(bmp.GetHicon())
                        End Using
                    End Using

                Catch e As Exception
                    Debug.WriteLine(e.Message)
                End Try
            End If
            Return cursor
        End Function

#End Region ' Cursor feedback

#Region " Mouse input handling "

        Public Overrides Sub ProcessMouseClick(e As MouseEventArgs, map As ucMap)

            If Not Me.IsEditable Then Return

            If ((e.Button And MouseButtons.Right) > 0) Then
                Dim ptfClick As PointF = map.PointToColRowExact(e.Location)
                Me.Pickup(map.PointToColRow(e.Location))
            ElseIf ((e.Button And MouseButtons.Left) > 0) Then
                Me.StartEdit(e, map)
            End If

        End Sub

        Public Overrides Sub ProcessMouseDraw(e As MouseEventArgs, map As ucMap)

            If Not Me.IsEditing Then Return

            Dim ptScreenCur As Point = e.Location '  New Point(e.X, e.Y)
            Dim size As Integer = Me.CursorSize + 1

            If (Me.m_ptScreenPrevious = Nothing) Then Me.m_ptScreenPrevious = ptScreenCur

            Dim ptCellFrom As Point = map.PointToColRow(Me.m_ptScreenPrevious)
            Dim ptCellTo As Point = map.PointToColRow(ptScreenCur)

            Dim ptUpdateMin As New Point(Math.Min(ptCellFrom.X, ptCellTo.X) - size, Math.Min(ptCellFrom.Y, ptCellTo.Y) + size)
            Dim ptUpdateMax As New Point(Math.Max(ptCellFrom.X, ptCellTo.X) - size, Math.Max(ptCellFrom.Y, ptCellTo.Y) + size)

            If (Me.Edit(ptCellFrom, ptCellTo,
                        New Point(ptScreenCur.X - Me.m_ptScreenPrevious.X, ptScreenCur.Y - Me.m_ptScreenPrevious.Y),
                        map.GetCellSize(),
                        e,
                        ptUpdateMin, ptUpdateMax)) Then

                ' Flag layer as changed
                DirectCast(Me.m_layer, cDisplayLayerRaster).IsModified = True

                map.UpdateMap(ptUpdateMin, ptUpdateMax)
            End If

            Me.m_ptScreenPrevious = ptScreenCur

        End Sub

        Public Overrides Sub ProcessMouseUp()
            If Not Me.IsEditing Then Return
            Me.EndEdit()
        End Sub

#End Region ' Mouse input handling

#Region " Editing "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User has started editing the layer.
        ''' </summary>
        ''' <param name="args">Click <see cref="MouseEventArgs">mouse state</see>
        ''' information.</param>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub StartEdit(args As MouseEventArgs, map As ucMap)

            Me.IsEditing = True

            ' If NOT Shift key pressed, release the last mouse pos
            If Not (Control.ModifierKeys = Keys.Shift) Then Me.m_ptScreenPrevious = Nothing

            If (Not Me.IsEditable) Then Return

            If (Me.GUI IsNot Nothing) Then ' Notify the editor GUI, if any
                Me.GUI.StartEdit(Me)
            End If

            ' Perform edit
            Me.ProcessMouseDraw(args, map)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User is done editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Overrides Sub EndEdit()

            ' Last-minute abort
            If (Not Me.IsEditable) Then Return
            ' Notify the editor GUI, if any
            If (Me.GUI IsNot Nothing) Then Me.GUI.EndEdit(Me)
            ' Update layer
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

            Me.IsEditing = False

        End Sub

        Public ReadOnly Property CanSmooth() As Boolean
            Get
                Return Me.Layer.ValueType Is GetType(Single)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Smooth layer data across water cells.
        ''' </summary>
        ''' <remarks>
        ''' Cells with NoData values are not considered in the smoothing, and
        ''' will not receive new values.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Smooth()

            If (Not Me.IsEditable) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim cnew(,) As Single, i As Integer, j As Integer
            Dim t As Single
            Dim n As Integer

            ReDim cnew(bm.InRow, bm.InCol)

            For i = 1 To bm.InRow
                For j = 1 To bm.InCol
                    t = 0
                    n = 0
                    For ii As Integer = i - 1 To i + 1
                        For jj As Integer = j - 1 To j + 1
                            If Not (ii = 0 Or jj = 0 Or ii = bm.InRow + 1 Or jj = bm.InCol + 1) And (layerDepth.IsWaterCell(ii, jj)) Then
                                Dim v As Single = CSng(Me.Layer.Value(ii, jj))
                                If (v <> cCore.NULL_VALUE) Then
                                    t += CSng(Me.Layer.Value(ii, jj))
                                    n += 1
                                End If
                            End If
                        Next jj
                    Next ii
                    If n > 0 Then cnew(i, j) = t / n
                Next j
            Next i

            For i = 1 To bm.InRow
                For j = 1 To bm.InCol
                    If layerDepth.IsWaterCell(i, j) Then
                        Dim v As Single = CSng(Me.Layer.Value(i, j))
                        If (v <> cCore.NULL_VALUE) Then
                            Me.Layer.Value(i, j) = cnew(i, j)
                        End If
                    End If
                Next
            Next
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the layer with the current <see cref="CellValue"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Reset()

            If (Not Me.IsEditable) Then Return

            Dim v As Object = Me.CellValue

            ' ToDo: globalize this
            Dim msg As New cFeedbackMessage(cStringUtils.Localize("Are you sure you want to set all cells in this map to {0}?", v),
                                            eCoreComponentType.External, eMessageType.Any, eMessageImportance.Question)
            msg.ReplyStyle = eMessageReplyStyle.YES_NO
            msg.Reply = eMessageReply.YES

            Me.UIContext.Core.Messages.SendMessage(msg)

            If (msg.Reply <> eMessageReply.YES) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth

            For i As Integer = 1 To bm.InRow
                For j As Integer = 1 To bm.InCol
                    If layerDepth.IsWaterCell(i, j) Then
                        Me.Layer.Value(i, j) = v
                    End If
                Next j
            Next i
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub

#End Region ' Editing

#Region " Cloning "

        Public Overridable ReadOnly Property CanDuplicate() As Boolean
            Get
                Return (Me.Layer.Data.SecundaryIndexCounter <> eCoreCounterTypes.NotSet)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Duplicate layer data across indexed layers.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Duplicate(iFrom As Integer)

            If (Not Me.IsEditable) Then Return

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap
            Dim layerDepth As cEcospaceLayerDepth = bm.LayerDepth
            Dim cc As Integer = Me.UIContext.Core.GetCoreCounter(Me.Layer.Data.SecundaryIndexCounter)
            Dim val As Object = Nothing

            For i As Integer = 1 To bm.InRow
                For j As Integer = 1 To bm.InCol
                    If (layerDepth.IsWaterCell(i, j)) Then
                        val = Me.Layer.Data.Cell(i, j, iFrom)
                        For k As Integer = 1 To cc
                            If (k <> iFrom) Then
                                Me.Layer.Data.Cell(i, j, k) = val
                            End If
                        Next k
                    End If
                Next j
            Next i

            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub

#End Region ' Cloning

#Region " Internals "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the value of a cell in the current layer with the designated 
        ''' <see cref="CellValue">set value</see>.
        ''' </summary>
        ''' <param name="ptSet">The cell location (Col, Row) to set.</param>
        ''' <param name="ptClick">The cell location (Col, Row) in the cursor.</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Function SetCellValue(ptSet As Point,
                                                    value As Object,
                                                    e As MouseEventArgs,
                                                    ptClick As Point) As Boolean
            If (Not Me.IsEditable) Then Return False
            If Not Convert.Equals(Me.Layer.Value(ptSet.Y, ptSet.X), value) Then
                Me.Layer.Value(ptSet.Y, ptSet.X) = value
                Return True
            End If
            Return False
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Edit the layer from one point to a next.
        ''' </summary>
        ''' <param name="ptFrom">The mouse location to edit from.</param>
        ''' <param name="ptTo">The mouse location to edit to.</param>
        ''' <param name="ptDelta">Mouse distance travelled since the last edit operation.</param>
        ''' <param name="szfCell">Size of a single cell.</param>
        ''' <param name="args">Click <see cref="MouseEventArgs">mouse state</see>
        ''' information.</param>
        ''' <param name="ptUpdateMin">Top-left cell position affected by
        ''' the edit operation.</param>
        ''' <param name="ptUpdateMax">Bottom-right cell position affected by
        ''' the edit operation.</param>
        ''' <returns>True if map values have changed due to the edit.</returns>
        ''' -------------------------------------------------------------------
        Protected Overridable Function Edit(ptFrom As Point,
                                    ptTo As Point,
                                    ptDelta As Point,
                                    szfCell As SizeF,
                                    args As MouseEventArgs,
                                    ByRef ptUpdateMin As Point,
                                    ByRef ptUpdateMax As Point) As Boolean

            ' Calc positions between current and last draw point
            Dim iNumSteps As Integer = Math.Max(1, Math.Max(Math.Abs(ptFrom.X - ptTo.X), Math.Abs(ptFrom.Y - ptTo.Y)))
            Dim dDX As Double = (ptTo.X - ptFrom.X) / iNumSteps
            Dim dX As Double = ptFrom.X
            Dim dDY As Double = (ptTo.Y - ptFrom.Y) / iNumSteps
            Dim dY As Double = ptFrom.Y

            Dim ptDraw As Point = Nothing
            Dim ptCell As Point = Nothing
            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap

            Dim bChanged As Boolean = False

            ' Draw every step between the two draw points
            For iStep As Integer = 1 To iNumSteps

                dX += dDX
                dY += dDY

                For iX As Integer = 0 To Me.CursorSize - 1
                    For iY As Integer = 0 To Me.CursorSize - 1

                        Dim ptfCursor As New PointF(CSng(iX - (Me.CursorSize - 1) / 2),
                                                    CSng(iY - (Me.CursorSize - 1) / 2))

                        If (Math.Sqrt(ptfCursor.X * ptfCursor.X + ptfCursor.Y * ptfCursor.Y) <= (Me.CursorSize / 2)) Then

                            ptCell = New Point(CInt(Math.Floor(dX + ptfCursor.X)), CInt(Math.Floor(dY + ptfCursor.Y)))

                            ' JS 26Feb15: This is the only spot to protect for invalid row/col access.
                            '             Should this check not have been here ages ago?!
                            If (bm.IsValidCellPosition(ptCell.Y, ptCell.X)) Then
                                bChanged = bChanged Or Me.SetCellValue(ptCell, Me.CellValue, args, New Point(iX, iY))

                                ptUpdateMin.X = Math.Min(ptCell.X, ptUpdateMin.X)
                                ptUpdateMin.Y = Math.Min(ptCell.Y, ptUpdateMin.Y)
                                ptUpdateMax.X = Math.Max(ptCell.X, ptUpdateMax.X)
                                ptUpdateMax.Y = Math.Max(ptCell.Y, ptUpdateMax.Y)
                            End If
                        End If
                    Next iY
                Next iX

            Next iStep

            Return bChanged

        End Function

#End Region ' Internals

    End Class

End Namespace
