' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991- 
'    UBC Institute for the Oceans and Fisheries, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Utilities
Imports EwEUtils.Core

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor base class that supports manual modification of Ecospace 
    ''' layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cLayerEditor

#Region " Private vars "

        ' === LAYER SUPPORT ===
        ''' <summary>The raster layer to operate on.</summary>
        Private m_layer As cDisplayRasterLayer = Nothing
        ''' <summary>Flag stating whether the layer is editable.</summary>
        Private m_bEditable As Boolean = True
        ''' <summary>The current value 'under the cursor'.</summary>
        Private m_sValue As Single = Nothing
        ''' <summary>Max value for cursor.</summary>
        Private m_sValueMax As Single = Single.MaxValue
        ''' <summary>Min value for cursor.</summary>
        Private m_sValueMin As Single = 0

        ' === GUI SUPPORT ===
        ''' <summary>Runtime type of the <see cref="ucLayerEditor">layer editor GUI</see>
        ''' that implements the user interface controls to configure the editor.</summary>
        Private m_typeGUI As Type = Nothing
        ''' <summary>A GUI, if any.</summary>
        Private m_gui As ILayerEditorGUI = Nothing

        ' === FEEDBACK SUPPORT ===
        Private Shared s_iCursorSize As Integer = 1

#End Region ' Private vars

#Region " Construction "

        Public Sub New(ByVal typeGUI As Type)
            If typeGUI Is Nothing Then typeGUI = GetType(ucLayerEditorDefault)
            Me.m_typeGUI = typeGUI
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the layer editor.
        ''' </summary>
        ''' <param name="uic">UI context to attach.</param>
        ''' <param name="layer">Layer to attach.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Initialize(ByVal uic As cUIContext, _
                                          ByVal layer As cDisplayRasterLayer)
            Me.UIContext = uic
            Me.Layer = layer
        End Sub

        Protected Overrides Sub Finalize()

            If (Me.m_gui IsNot Nothing) Then
                If (TypeOf Me.m_gui Is ucLayerEditor) Then
                    DirectCast(Me.m_gui, ucLayerEditor).Detach()
                End If
            End If

            Me.Layer = Nothing
            Me.UIContext = Nothing

            MyBase.Finalize()

        End Sub

        Public Overridable Function Clone() As cLayerEditor
            Dim minime As cLayerEditor = Nothing

            ' Clone without GUI type
            minime = DirectCast(Activator.CreateInstance(Me.GetType(), New Object() {}), cLayerEditor)
            minime.IsEditable = Me.IsEditable
            minime.IsReadOnly = Me.IsReadOnly

            Return minime
        End Function

#End Region ' Construction

#Region " Events "

        Private Sub OnLayerChanged(ByVal layer As cDisplayLayer, ByVal cf As cDisplayLayer.eChangeFlags)
            If Me.GUI IsNot Nothing Then
                Me.GUI.UpdateContent(Me)
            End If
        End Sub

#End Region ' Events

#Region " GUI feedback "

        Public Shared Function EditorCursor(ByVal iCursorSize As Integer, ByVal szCell As SizeF) As Cursor

            Dim ptIconSize As New Size(CInt(szCell.Width * iCursorSize), CInt(szCell.Height * iCursorSize))
            Dim cursor As Cursor = Cursors.Hand

            If (iCursorSize > 0) Then
                Try
                    Dim bm As New Bitmap(ptIconSize.Width + 1, ptIconSize.Height + 1)
                    Dim g As Graphics = Graphics.FromImage(bm)

                    g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    g.FillRectangle(Brushes.Transparent, New Rectangle(0, 0, bm.Width, bm.Height))
                    g.DrawEllipse(Pens.White, 1, 1, ptIconSize.Width - 2, ptIconSize.Height - 2)
                    g.DrawEllipse(Pens.Black, 0, 0, ptIconSize.Width, ptIconSize.Height)
                    Using br As New SolidBrush(Color.FromArgb(45, 0, 0, 0))
                        g.FillEllipse(br, 0, 0, ptIconSize.Width, ptIconSize.Height)
                    End Using
                    cursor = New Cursor(bm.GetHicon())
                    g.Dispose()
                    bm.Dispose()

                Catch e As Exception
                    Debug.WriteLine(e.Message)
                End Try
            End If
            Return cursor
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a <see cref="ucLayerEditor">layer editor control</see> to 
        ''' allow a user to parameterize the edit process.
        ''' </summary>
        ''' <remarks>
        ''' Do not forget to destroy any control created with this method via 
        ''' <see cref="DestroyEditorControl">DestroyEditorControl</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateEditorControl() As ucLayerEditor

            Dim gui As ucLayerEditor = Nothing

            Debug.Assert(Me.m_gui Is Nothing)

            Try
                Dim obj As Object = Activator.CreateInstance(Me.m_typeGUI, New Object() {})
                ' Sanity check
                Debug.Assert(TypeOf obj Is ucLayerEditor)

                gui = DirectCast(obj, ucLayerEditor)
                gui.Attach(Me.UIContext, Me, Me.m_layer)
                gui.Initialize(Me)

                ' Remember GUI
                Me.m_gui = gui

            Catch ex As Exception
                Debug.Assert(False, "Failed to create layer editor interface")
            End Try

            Return gui
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Destroys a <see cref="ucLayerEditor">layer editor control</see>.
        ''' </summary>
        ''' <remarks>
        ''' Only use this method on controls created with 
        ''' <see cref="CreateEditorControl">CreateEditorControl</see>.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overridable Sub DestroyEditorControl()

            Debug.Assert(Me.m_gui IsNot Nothing)

            If (TypeOf Me.m_gui Is ucLayerEditor) Then
                DirectCast(Me.m_gui, ucLayerEditor).Detach()
                DirectCast(Me.m_gui, ucLayerEditor).Dispose()
            End If
            Me.m_gui = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Cursor feedback for the current location of the cursor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Function Cursor(ByVal szCell As SizeF) As Cursor
            Return cLayerEditor.EditorCursor(Me.CursorSize, szCell)
        End Function

        Public Property GUI() As ILayerEditorGUI
            Get
                Return Me.m_gui
            End Get
            Set(ByVal value As ILayerEditorGUI)
                Me.m_gui = value
                If Me.m_gui IsNot Nothing Then
                    Me.m_gui.Initialize(Me)
                End If
            End Set
        End Property

#End Region ' GUI feedback

#Region " Editing "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User has started editing the layer.
        ''' </summary>
        ''' <param name="ptClick">The cell position that was clicked.</param>
        ''' <param name="args">Click <see cref="MouseEventArgs">mouse state</see>
        ''' information.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub StartEdit(ByVal ptClick As Point, ByVal args As MouseEventArgs)

            If (Me.GUI Is Nothing) Or (Not Me.IsEditable) Then Return
            ' Notify the editor GUI, if any
            Me.GUI.StartEdit(Me)

        End Sub

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
        ''' -------------------------------------------------------------------
        Public Overridable Sub Edit(ByVal ptFrom As Point, _
                                    ByVal ptTo As Point, _
                                    ByVal ptDelta As Point, _
                                    ByVal szfCell As SizeF, _
                                    ByVal args As MouseEventArgs, _
                                    ByRef ptUpdateMin As Point, _
                                    ByRef ptUpdateMax As Point)

            If (Not Me.IsEditable) Then Return

            ' Calc positions between current and last draw point
            Dim iNumSteps As Integer = Math.Max(1, Math.Max(Math.Abs(ptFrom.X - ptTo.X), Math.Abs(ptFrom.Y - ptTo.Y)))
            Dim dDX As Double = (ptTo.X - ptFrom.X) / iNumSteps
            Dim dX As Double = ptFrom.X
            Dim dDY As Double = (ptTo.Y - ptFrom.Y) / iNumSteps
            Dim dY As Double = ptFrom.Y

            Dim ptDraw As Point = Nothing
            Dim ptCell As Point = Nothing

            Dim bm As cEcospaceBasemap = Me.UIContext.Core.EcospaceBasemap

            ' Draw every step between the two draw points
            For iStep As Integer = 1 To iNumSteps

                dX += dDX
                dY += dDY

                For iX As Integer = 0 To Me.CursorSize - 1
                    For iY As Integer = 0 To Me.CursorSize - 1

                        Dim ptfCursor As New PointF(CSng(iX - (Me.CursorSize - 1) / 2), _
                                                    CSng(iY - (Me.CursorSize - 1) / 2))

                        If (Math.Sqrt(ptfCursor.X * ptfCursor.X + ptfCursor.Y * ptfCursor.Y) <= (Me.CursorSize / 2)) Then

                            ptCell = New Point(CInt(Math.Floor(dX + ptfCursor.X)), CInt(Math.Floor(dY + ptfCursor.Y)))

                            ' JS 26Feb15: This is the only spot to protect for invalid row/col access.
                            '             Should this check not have been here ages ago?!
                            If (bm.IsValidCellPosition(ptCell.Y, ptCell.X)) Then
                                Me.SetCellValue(ptCell, Me.CellValue, args, New Point(iX, iY))

                                ptUpdateMin.X = Math.Min(ptCell.X, ptUpdateMin.X)
                                ptUpdateMin.Y = Math.Min(ptCell.Y, ptUpdateMin.Y)
                                ptUpdateMax.X = Math.Max(ptCell.X, ptUpdateMax.X)
                                ptUpdateMax.Y = Math.Max(ptCell.Y, ptUpdateMax.Y)
                            End If
                        End If
                    Next iY
                Next iX

            Next iStep

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User is done editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndEdit()
            ' Last-minute abort
            If (Not Me.IsEditable) Then Return
            ' Notify the editor GUI, if any
            If (Me.GUI IsNot Nothing) Then Me.GUI.EndEdit(Me)
            ' Update layer
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick up the cell value at a given point, and store this value in the
        ''' layer editor as the next value that will be set.
        ''' </summary>
        ''' <param name="pt">The cell location to pick up a value from.</param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Pickup(ByVal pt As Point)

            Try
                Me.CellValue = CDec(Layer.Value(pt.Y, pt.X))
            Catch ex As Exception
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Set the value of a cell in the current layer with the designated 
        ''' <see cref="CellValue">set value</see>.
        ''' </summary>
        ''' <param name="ptSet">The cell location (Col, Row) to set.</param>
        ''' <param name="ptClick">The cell location (Col, Row) in the cursor.</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub SetCellValue(ByVal ptSet As Point, _
                                               ByVal value As Object, _
                                               ByVal e As MouseEventArgs, _
                                               ByVal ptClick As Point)
            If (Not Me.IsEditable) Then Return
            Me.Layer.Value(ptSet.Y, ptSet.X) = value
        End Sub

        Public ReadOnly Property CanSmooth() As Boolean
            Get
                Return Me.m_layer.ValueType Is GetType(Single)
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Smooth layer data across water cells.
        ''' </summary>
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
                                t += CSng(Me.Layer.Value(ii, jj))
                                n += 1
                            End If
                        Next jj
                    Next ii
                    If n > 0 Then cnew(i, j) = t / n
                Next j
            Next i

            For i = 1 To bm.InRow
                For j = 1 To bm.InCol
                    If layerDepth.IsWaterCell(i, j) Then
                        Me.Layer.Value(i, j) = cnew(i, j)
                    End If
                Next
            Next
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub

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
        Public Overridable Sub Duplicate(ByVal iFrom As Integer)

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Fill the layer with the current <see cref="CellValue"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Reset()

            If (Not Me.IsEditable) Then Return

            ' ToDo: globalize this
            Dim msg As New cFeedbackMessage(cStringUtils.Localize("Are you sure you want to set all cells in this map to {0}?", Me.CellValue), _
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
                        Me.Layer.Value(i, j) = Me.CellValue
                    End If
                Next j
            Next i
            Me.Layer.Update(cDisplayLayer.eChangeFlags.Map)

        End Sub

#End Region ' Editing

#Region " Properties "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer is editable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Property IsEditable() As Boolean
            Get
                Dim bEditable As Boolean = (Me.m_bEditable = True) And (Me.IsReadOnly = False)

                If (Me.m_layer IsNot Nothing) Then
                    ' JS 22Nov14: this makes it hard to play with data. Removed constraint
                    '' External data cannot be edited
                    'bEditable = bEditable And (Not Me.m_layer.IsExternal)
                    ' Invisible data cannot be edited
                    If (Me.m_layer.Renderer IsNot Nothing) Then bEditable = bEditable And Me.m_layer.Renderer.IsVisible
                Else
                    ' No need to edit a layer that does not exist, no?
                    bEditable = False
                End If
                Return bEditable
            End Get
            Set(ByVal value As Boolean)
                Dim bEditable As Boolean = value
                If (bEditable <> Me.m_bEditable) Then
                    Me.m_bEditable = bEditable
                    ' Send out change notification
                    If (Me.m_layer IsNot Nothing) Then
                        Me.m_layer.Update(cDisplayLayer.eChangeFlags.Editable)
                    End If
                End If
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer can be made editable at all.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Property IsReadOnly() As Boolean

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
                Return cLayerEditor.s_iCursorSize
            End Get
            Set(ByVal iCursorSize As Integer)
                cLayerEditor.s_iCursorSize = iCursorSize
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the value for the next cell that is to be edited.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property CellValue() As Object
            Get
                Return Me.m_sValue
            End Get
            Set(ByVal value As Object)
                Dim sValue As Single = Math.Max(Math.Min(CSng(value), Me.m_sValueMax), Me.m_sValueMin)
                If (sValue <> Me.m_sValue) Then
                    Me.m_sValue = sValue
                    'Me.UpdateGUI()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Configure the editor to adhere to given <see cref="cVariableMetaData">variable meta data</see>.
        ''' </summary>
        ''' <param name="md">
        ''' The metadata to apply. If Nothing/Null this editor will need to be
        ''' manually configured via <see cref="CellValueMax">CellValueMax</see> 
        ''' and <see cref="CellValueMin">CellValueMin</see>.
        ''' </param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub ApplyMetadata(ByVal md As cVariableMetaData)
            If (md IsNot Nothing) Then
                Me.m_sValueMin = md.Min
                Me.m_sValueMax = md.Max
            End If
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
            Get
                Return Me.m_sValueMax
            End Get
            Set(ByVal value As Single)
                Me.m_sValueMax = value
            End Set
        End Property

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
            Get
                Return Me.m_sValueMin
            End Get
            Set(ByVal value As Single)
                Me.m_sValueMin = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the layer to attach to this Editor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Layer() As cDisplayRasterLayer
            Get
                Return Me.m_layer
            End Get
            Private Set(ByVal value As cDisplayRasterLayer)
                If Object.ReferenceEquals(value, Me.m_layer) Then Return

                ' Already has a layer?
                If Me.m_layer IsNot Nothing Then
                    ' #Yes: stop listening to layer changes
                    RemoveHandler Me.m_layer.LayerChanged, AddressOf OnLayerChanged
                End If

                ' Store new layer
                Me.m_layer = value

                ' Has a new layer?
                If Me.m_layer IsNot Nothing Then
                    ' #Yes: start listening to layer changes
                    AddHandler Me.m_layer.LayerChanged, AddressOf OnLayerChanged
                    ' Set metadata
                    Dim d As cEcospaceLayer = Me.m_layer.Data
                    Dim md As cVariableMetaData = Nothing

                    If (d IsNot Nothing) Then md = d.MetadataCell
                    Me.ApplyMetadata(md)

                End If

            End Set
        End Property

        Public Property UIContext() As cUIContext

        Protected Sub UpdateGUI()
            If (Me.m_gui IsNot Nothing) Then
                Me.m_gui.UpdateContent(Me)
            End If
        End Sub

#End Region ' Properties

    End Class

End Namespace
