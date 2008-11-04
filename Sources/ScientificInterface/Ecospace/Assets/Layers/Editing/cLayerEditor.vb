'==============================================================================
'
' $Log: cLayerEditor.vb,v $
' Revision 1.1  2008/11/04 04:40:16  jeroens
' Split into separate files, moved
'
'==============================================================================

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
        Private m_objValue As Object = Nothing

        ' === GUI SUPPORT ===
        ''' <summary>Runtime type of the <see cref="ucLayerEditor">layer editor GUI</see>
        ''' that implements the user interface controls to configure the editor.</summary>
        Private m_typeGUI As Type = Nothing
        ''' <summary>An instantiated GUI, if any.</summary>
        Private m_gui As ucLayerEditor = Nothing

        ' === FEEDBACK SUPPORT ===
        Private m_iCursorSize As Integer = 1

#End Region ' Private vars

#Region " Construction "

        Public Sub New(ByVal typeGUI As Type)
            If typeGUI Is Nothing Then typeGUI = GetType(ucLayerEditorDefault)
            Me.m_typeGUI = typeGUI
        End Sub

        Public Sub Initialize(ByVal layer As cLayer)
            Me.Layer = layer
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            Me.Layer = Nothing
        End Sub

#End Region ' Construction

#Region " Events "

        Private Sub OnLayerChanged(ByVal layer As cLayer, ByVal cf As cLayer.eChangeFlags)
            If Me.GUI IsNot Nothing Then
                Me.GUI.UpdateControls()
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
        Public Overridable Function GetEditorControl() As ucLayerEditor
            Return Me.GUI
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Optional GUI to allow the user to parameterize the edit process.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Sub ReleaseEditorControl()
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
            layer.Value(ptSet) = Me.CellValue
        End Sub

#End Region ' Editing

#Region " Properties "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer is editable.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsEditable() As Boolean
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
        Public Property IsReadOnly() As Boolean
            Get
                Return m_bReadOnly
            End Get
            Set(ByVal value As Boolean)
                Me.m_bReadOnly = value
            End Set
        End Property

        Public Property CursorSize() As Integer
            Get
                Return Me.m_iCursorSize
            End Get
            Set(ByVal iCursorSize As Integer)
                Me.m_iCursorSize = iCursorSize
            End Set
        End Property

        Public Property CellValue() As Object
            Get
                Return Me.m_objValue
            End Get
            Set(ByVal value As Object)
                Me.m_objValue = value
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the editor user interface.
        ''' </summary>
        ''' <returns>A <see cref="ucLayerEditor">ucLayerEditor</see>-inherited
        ''' user control.</returns>
        ''' -----------------------------------------------------------------------
        Protected Function GUI() As ucLayerEditor
            ' Create editor GUI if not done
            If (Me.m_gui Is Nothing) Then
                Try
                    Dim obj As Object = Activator.CreateInstance(Me.m_typeGUI, New Object() {})
                    ' Sanity check
                    Debug.Assert(TypeOf obj Is ucLayerEditor)
                    ' Remember GUI
                    Me.m_gui = DirectCast(obj, ucLayerEditor)
                    Me.m_gui.Editor = Me
                Catch ex As Exception
                    Debug.Assert(False, "Failed to create layer editor interface")
                End Try
            End If
            Return Me.m_gui
        End Function

#End Region 'Internals 

    End Class

End Namespace
