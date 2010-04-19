#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Windows.Forms
Imports ScientificInterface.Ecospace.Basemap.Layers

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer editor base class that supports manual modification of Ecospace 
    ''' layers.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cLayerEditor

#Region " Private vars "

        ' === LAYER SUPPORT ===
        ''' <summary>The layer to operate on.</summary>
        Private m_layer As cLayer = Nothing
        ''' <summary>Flag stating whether the layer is editable.</summary>
        ''' <remarks></remarks>
        Private m_bEditable As Boolean = True
        ''' <summary>Flag stating whether the layer is read-only.</summary>
        ''' <remarks></remarks>
        Private m_bReadOnly As Boolean = False
        ''' <summary>The current value 'under the cursor'.</summary>
        Private Shared s_decValue As Decimal = Nothing
        Private m_decValueMax As Decimal = Decimal.MaxValue
        Private m_decValueMin As Decimal = 0

        ' === GUI SUPPORT ===
        ''' <summary>Runtime type of the <see cref="ucLayerEditor">layer editor GUI</see>
        ''' that implements the user interface controls to configure the editor.</summary>
        Private m_typeGUI As Type = Nothing
        ''' <summary>An instantiated GUI, if any.</summary>
        Private m_gui As ucLayerEditor = Nothing
        Private m_uic As cUIContext = Nothing

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
                                          ByVal layer As cLayer)
            Me.Layer = layer
            Me.UIContext = uic
        End Sub

        Protected Overrides Sub Finalize()

            If (Me.m_gui IsNot Nothing) Then
                Me.m_gui.Detach()
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

        Private Sub OnLayerChanged(ByVal layer As cLayer, ByVal cf As cLayer.eChangeFlags)
            If Me.GUI IsNot Nothing Then
                Me.GUI.UpdateContent()
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
                    g.DrawEllipse(Pens.Gray, 0, 0, ptIconSize.Width, ptIconSize.Height)
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
        ''' Optional GUI to allow the user to parameterize the edit process.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Function CreateEditorControl() As ucLayerEditor

            Debug.Assert(Me.m_gui Is Nothing)

            Try
                Dim obj As Object = Activator.CreateInstance(Me.m_typeGUI, New Object() {})
                ' Sanity check
                Debug.Assert(TypeOf obj Is ucLayerEditor)
                ' Remember GUI
                Me.m_gui = DirectCast(obj, ucLayerEditor)
                Me.m_gui.Attach(Me.m_uic, Me)
            Catch ex As Exception
                Debug.Assert(False, "Failed to create layer editor interface")
            End Try

            Return Me.m_gui
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Optional GUI to allow the user to parameterize the edit process.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub DestroyEditorControl()

            Debug.Assert(Me.m_gui IsNot Nothing)

            Me.m_gui.Detach()
            Me.m_gui.Dispose()
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

#End Region ' GUI feedback

#Region " Editing "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User has started editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub StartEdit(ByVal ptClick As Point)
            ' Notify the editor GUI, if any
            If Me.GUI IsNot Nothing Then
                Me.GUI.StartEdit()
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Edit the layer from one point to a next.
        ''' </summary>
        ''' <param name="ptFrom"></param>
        ''' <param name="ptTo"></param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Edit(ByVal ptFrom As Point, ByVal ptTo As Point, _
                                    ByRef ptUpdateMin As Point, ByRef ptUpdateMax As Point)

            ' Calc positions between current and last draw point
            Dim iNumSteps As Integer = Math.Max(1, Math.Max(Math.Abs(ptFrom.X - ptTo.X), Math.Abs(ptFrom.Y - ptTo.Y)))
            Dim dDX As Double = (ptTo.X - ptFrom.X) / iNumSteps
            Dim dX As Double = ptFrom.X
            Dim dDY As Double = (ptTo.Y - ptFrom.Y) / iNumSteps
            Dim dY As Double = ptFrom.Y

            Dim ptDraw As Point = Nothing
            Dim ptCell As Point = Nothing

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
                            Me.SetCellValue(Layer, ptCell, New Point(iX, iY))

                            ptUpdateMin.X = Math.Min(ptCell.X, ptUpdateMin.X)
                            ptUpdateMin.Y = Math.Min(ptCell.Y, ptUpdateMin.Y)
                            ptUpdateMax.X = Math.Max(ptCell.X, ptUpdateMax.X)
                            ptUpdateMax.Y = Math.Max(ptCell.Y, ptUpdateMax.Y)

                        End If

                    Next iY
                Next iX

            Next iStep

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick up the cell value at a given point, and store this value in the
        ''' layer editor as the next value that will be set.
        ''' </summary>
        ''' <param name="pt"></param>
        ''' -------------------------------------------------------------------
        Public Overridable Sub Pickup(ByVal pt As Point)

            Try
                Me.CellValue = CDec(Layer.Value(pt.Y, pt.X))
            Catch ex As Exception
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' User is done editing the layer.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub EndEdit()
            ' Notify the editor GUI, if any
            If Me.GUI IsNot Nothing Then
                Me.GUI.EndEdit()
            End If
        End Sub

        Protected Overridable Sub SetCellValue(ByVal layer As cLayer, _
                                           ByVal ptSet As Point, _
                                           ByVal ptClick As Point)
            layer.Value(ptSet.Y, ptSet.X) = Me.CellValue
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
                'If (Me.m_propName IsNot Nothing) Then
                '    bEditable = bEditable And ((m_propName.GetStyle() And StyleGuide.eStyleFlags.NotEditable) = 0)
                'End If
                Return bEditable
            End Get
            Set(ByVal value As Boolean)
                Me.m_bEditable = value And Not Me.IsReadOnly
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer can be made editable at all
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Property IsReadOnly() As Boolean
            Get
                Return m_bReadOnly
            End Get
            Set(ByVal value As Boolean)
                Me.m_bReadOnly = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the size of the cursor.
        ''' </summary>
        ''' <remarks>
        ''' This value is persistent across layer editors.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property CursorSize() As Integer
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
        ''' <remarks>
        ''' This value is persistent across layer editors.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property CellValue() As Decimal
            Get
                Return cLayerEditor.s_decValue
            End Get
            Set(ByVal value As Decimal)
                cLayerEditor.s_decValue = Math.Max(Math.Min(value, Me.m_decValueMax), Me.m_decValueMin)
                If (Me.m_gui IsNot Nothing) Then
                    Me.m_gui.UpdateContent()
                End If
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the max value allowed in a cell.
        ''' </summary>
        ''' <remarks>
        ''' Ideally, this value would be obtained from core meta data. For now,
        ''' the UI is required to manually control this property.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Property CellValueMax() As Decimal
            Get
                Return Me.m_decValueMax
            End Get
            Set(ByVal value As Decimal)
                Me.m_decValueMax = value
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
        Public Property CellValueMin() As Decimal
            Get
                Return Me.m_decValueMin
            End Get
            Set(ByVal value As Decimal)
                Me.m_decValueMin = value
            End Set
        End Property

#End Region ' Properties

#Region " Internals "

        Public Property Layer() As cLayer
            Get
                Return Me.m_layer
            End Get
            Private Set(ByVal value As cLayer)
                If Object.ReferenceEquals(value, Me.m_layer) Then Return

                If Me.m_layer IsNot Nothing Then
                    RemoveHandler Me.m_layer.LayerChanged, AddressOf OnLayerChanged
                End If
                Me.m_layer = value
                If Me.m_layer IsNot Nothing Then
                    AddHandler Me.m_layer.LayerChanged, AddressOf OnLayerChanged
                End If

            End Set
        End Property

        Public Property UIContext() As cUIContext
            Get
                Return Me.m_uic
            End Get
            Protected Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the editor user interface.
        ''' </summary>
        ''' <returns>A <see cref="ucLayerEditor">ucLayerEditor</see>-inherited
        ''' user control.</returns>
        ''' -----------------------------------------------------------------------
        Protected Function GUI() As ucLayerEditor
            Return Me.m_gui
        End Function

#End Region 'Internals 

    End Class

End Namespace
