'==============================================================================
'
' $Log: ucSketchPad.vb,v $
' Revision 1.10  2009/04/19 14:14:29  jeroens
' Update on style change
'
' Revision 1.9  2009/04/19 13:48:56  jeroens
' Added Style
'
' Revision 1.8  2009/03/21 00:30:34  jeroens
' Fixed unclear parameter names
'
' Revision 1.7  2009/03/20 22:31:30  jeroens
' Backcolour responds to selection
'
' Revision 1.6  2009/03/20 17:55:42  jeroens
' Shape controls are multiple selection
'
' Revision 1.5  2009/03/19 16:13:43  jeroens
' X mark can be suppressed
'
' Revision 1.4  2009/03/02 17:43:41  jeroens
' Cleaned up
'
' Revision 1.3  2009/03/02 02:05:05  jeroens
' Properly named handlers
' XMark can be dragged
' Removed right-click scaling option
'
' Revision 1.2  2009/02/12 15:32:21  jeroens
' Can add labels to XMark, YMark lines
'
' Revision 1.1  2008/12/15 15:36:41  jeroens
' Moved from ScInt
'
' Revision 1.2  2008/10/07 22:04:25  jeroens
' Added Ymax sanity checks on drawing
'
' Revision 1.1  2008/09/26 07:31:44  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.IO
Imports System.Globalization
Imports System.Threading
Imports System.Drawing.Imaging
Imports EwECore
Imports EwEUtils.Win32Api
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Definitions
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' This Sketchpad control class is used to render the shape and support
    ''' mouse interaction. It can be used as the base class for both forcing functions and 
    ''' mediation functions.
    ''' </summary>
    ''' <remarks>
    ''' This code is a mess and deserves some serious lobotomy
    ''' </remarks>
    <CLSCompliant(True)> _
    Public Class ucSketchPad

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Enum eMouseInteractionMode As Integer
            ''' <summary>Not drawing.</summary>
            None = 0
            ''' <summary>User is drawing the shape.</summary>
            DrawShape = 1
            ''' <summary>User is dragging the X mark line.</summary>
            DragXMark = 2
            ''' <summary>User is dragging the Y mark line.</summary>
            ''' <remarks>JS 02Mar09: This feature is not implemented yet.</remarks>
            DragYMark = 4
        End Enum

        Private Const cCLICK_TOLERANCE As Integer = 4

#Region " Variables "

        ''' <summary>The one core</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Styleguide to listen to.</summary>
        Private m_sg As StyleGuide = Nothing
        ''' <summary>The manager of this control.</summary>
        Private m_handler As cShapeGUIHandler = Nothing
        ''' <summary>The shape shown in this control.</summary>
        Private m_shape As cShapeData = Nothing
        ''' <summary>Last known mouse location when drawing.</summary>
        Private m_ptPosPrevious As Point = Nothing

        ''' <summary></summary>
        Private m_scalemodeYAxis As eAxisAutoScaleModeTypes = eAxisAutoScaleModeTypes.Auto
        ''' <summary></summary>
        Protected m_color As Color = Drawing.Color.AliceBlue
        ''' <summary></summary>
        Protected m_bShowAxis As Boolean = True
        ''' <summary></summary>
        Protected m_sketchDrawMode As eSketchDrawModeTypes = eSketchDrawModeTypes.Fill
        ''' <summary></summary>
        Protected m_shapeType As eShapeCategoryTypes = eShapeCategoryTypes.NotSet

        ''' <summary></summary>
        Private m_sYMax As Single = cCore.NULL_VALUE
        ''' <summary></summary>
        Private m_sYMin As Single = cCore.NULL_VALUE

        ''' <summary>Horizontal mark line.</summary>
        Private m_sYMarkValue As Single = cCore.NULL_VALUE
        ''' <summary>Horizontal mark line.</summary>
        Private m_strYMarkLabel As String = ""
        ''' <summary>Vertical mark line.</summary>
        Private m_sXMarkValue As Single = cCore.NULL_VALUE
        ''' <summary>Vertical mark line label.</summary>
        Private m_strXMarkLabel As String = ""
        ''' <summary></summary>
        Private m_bShowXMark As Boolean = False
        ''' <summary></summary>
        Private m_editMode As eMouseInteractionMode = eMouseInteractionMode.None
        ''' <summary>Style of the control.</summary>
        Private m_style As StyleGuide.eStyleFlags = StyleGuide.eStyleFlags.OK

        ''' <summary></summary>
        Public Delegate Sub ShapeChangedDelegate(ByVal shape As cShapeData)
        ''' <summary></summary>
        Public Event ShapeChanged As ShapeChangedDelegate

        ''' <summary></summary>
        Public Delegate Sub ShapeFinalizedDelegate(ByVal shape As cShapeData, ByVal sketchpad As ucSketchPad)
        ''' <summary></summary>
        Public Event ShapeFinalized As ShapeFinalizedDelegate

#End Region ' Variables

#Region " Constructor "

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

            ' Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            Me.Dock = DockStyle.Fill

            Me.m_core = cCore.GetInstance()
            Me.m_sg = StyleGuide.GetInstance()

            ' Default rendering mode
            Me.m_sketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_scalemodeYAxis = eAxisAutoScaleModeTypes.Auto

        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the handler that manages this sketch pad.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Handler() As cShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal value As cShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateMenuItemStates()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the style of the control to override data styles.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Style() As StyleGuide.eStyleFlags
            Get
                Return Me.m_style
            End Get
            Set(ByVal value As StyleGuide.eStyleFlags)
                Me.m_style = value
                Me.UpdateControl()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the shape to display in the sketch pad.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overridable Property Shape() As cShapeData

            Get
                Return Me.m_shape
            End Get

            Set(ByVal value As cShapeData)
                ' Store new shape ref
                Me.m_shape = value
                ' Respond to this major event
                Me.UpdateControl()
                ' Broadcast change
                Me.OnShapeChanged()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether to display the shape as 12-month seasonal data or
        ''' across the full length of time.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property IsSeasonal() As Boolean

            Get
                If Me.m_shape Is Nothing Then Return False
                Return Me.m_shape.IsSeasonal
            End Get

            Set(ByVal value As Boolean)
                If Me.m_shape IsNot Nothing Then
                    Me.m_shape.IsSeasonal = value
                    If Me.m_shape.IsSeasonal Then RepeatSeasonalPattern()
                    Me.OnShapeChanged()
                End If
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the line style used to render the graph.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property SketchDrawMode() As eSketchDrawModeTypes

            Get
                Return Me.m_sketchDrawMode
            End Get

            Set(ByVal value As eSketchDrawModeTypes)
                Me.m_sketchDrawMode = value
                Me.Invalidate()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the sketch pad should display an X and Y axis.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property DisplayAxis() As Boolean

            Get
                Return Me.m_bShowAxis
            End Get

            Set(ByVal value As Boolean)
                Me.m_bShowAxis = value
                Me.Invalidate()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the sketch pad should automatically scale the Y axis
        ''' to the range of data in the current shape.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YAxisAutoScaleMode() As eAxisAutoScaleModeTypes

            Get
                Return Me.m_scalemodeYAxis
            End Get

            Set(ByVal value As eAxisAutoScaleModeTypes)
                Me.m_scalemodeYAxis = value
                Me.Invalidate()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YAxisMaxValue() As Single
            Get
                ' Locked for drawing?
                If Me.m_sYMaxLock > 0.0! Then
                    Return Me.m_sYMaxLock
                End If

                Select Case Me.YAxisAutoScaleMode
                    Case eAxisAutoScaleModeTypes.Auto
                        If Me.Shape Is Nothing Then Return 0.0
                        Return Math.Max(Me.Shape.YMax * 1.25!, Me.YAxisMinValue)
                    Case eAxisAutoScaleModeTypes.Fixed
                        Return Math.Max(0, Me.m_sYMax * 1.25!)
                End Select
            End Get
            Set(ByVal sValue As Single)
                Me.m_sYMax = sValue
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YAxisMinValue() As Single
            Get
                Return Me.m_sYMin
            End Get
            Set(ByVal value As Single)
                Me.m_sYMin = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Value for horizontal (Y mark) line
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YMarkValue() As Single
            Get
                Return Me.m_sYMarkValue
            End Get
            Set(ByVal value As Single)
                Me.m_sYMarkValue = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Label for horizontal (Y mark) line
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property YMarkLabel() As String
            Get
                Return Me.m_strYMarkLabel
            End Get
            Set(ByVal value As String)
                Me.m_strYMarkLabel = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowXMark() As Boolean
            Get
                Return Me.m_bShowXMark
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowXMark = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Value for vertical (X mark) line
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property XMarkValue() As Single
            Get
                If Not Me.ShowXMark Then Return cCore.NULL_VALUE
                Return Me.m_sXMarkValue
            End Get
            Set(ByVal value As Single)
                Me.m_sXMarkValue = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Label for vertical (X mark) line
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property XMarkLabel() As String
            Get
                Return Me.m_strXMarkLabel
            End Get
            Set(ByVal value As String)
                Me.m_strXMarkLabel = value
                Me.Invalidate()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the colour used to draw the shape.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShapeColor() As Color

            Get
                Return Me.m_color
            End Get

            Set(ByVal value As Color)
                Me.m_color = value
                Me.Invalidate()
            End Set

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get the <see cref="eShapeCategoryTypes">category</see> of the shape.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public ReadOnly Property ShapeCategory() As eShapeCategoryTypes

            Get
                If Me.m_shape Is Nothing Then Return eShapeCategoryTypes.NotSet
                Select Case m_shape.DataType
                    Case eDataTypes.Forcing
                        Return eShapeCategoryTypes.Forcing
                    Case eDataTypes.EggProd
                        Return eShapeCategoryTypes.EggProduction
                    Case eDataTypes.Mediation
                        Return eShapeCategoryTypes.Mediation
                    Case Else
                        Debug.Assert(False)
                End Select
                Return m_shapeType
            End Get

        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the current shape to an image file.
        ''' </summary>
        ''' <param name="shape"></param>
        ''' <param name="strFileName"></param>
        ''' <param name="imgFormat"></param>
        ''' <param name="strError"></param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Overridable Function SaveAsImage(ByVal shape As cShapeData, ByVal strFileName As String, _
                                                ByVal imgFormat As ImageFormat, _
                                                ByRef strError As String) As Boolean

            Dim rcClient As Rectangle = Me.ClientRectangle()
            Dim bmp As New Bitmap(rcClient.Width, rcClient.Height, Imaging.PixelFormat.Format32bppArgb)
            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim fs As IO.FileStream = Nothing
            Dim bSucces As Boolean = True

            ' Render the shape
            g.Clear(Me.BackColor)

            Try
                Me.DrawShape(shape, rcClient, g, Me.ShapeColor, True, Me.SketchDrawMode, Me.YAxisMaxValue)
            Catch ex As Exception
                bSucces = False
            End Try

            ' Try to open the stream
            Try
                fs = New FileStream(strFileName, FileMode.Create)
                bmp.Save(fs, imgFormat)
                fs.Close()
            Catch ex As Exception
                ' An error occurred
                strError = ex.Message
                bSucces = False
            End Try
            Return bSucces

        End Function

        Private m_bEditable As Boolean = True

        Public Overridable Property Editable() As Boolean
            Get
                Return Me.m_bEditable
            End Get
            Set(ByVal bEditable As Boolean)
                Me.m_bEditable = bEditable
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draws a <see cref="cForcingFunction">Forcing Function</see>.
        ''' </summary>
        ''' <param name="shape">The shape to draw.</param>
        ''' <param name="rcImage">The dimensions of the area to render the shape onto.</param>
        ''' <param name="g">The graphics to draw the image onto.</param>
        ''' <param name="clr">The colour to use rendering the image.</param>
        ''' <param name="drawMode">The <see cref="SketchDrawMode">Mode</see> to render the shape with.</param>
        ''' <param name="sYMax">The max Y value to scale the shape to.</param>
        ''' -------------------------------------------------------------------
        Protected Overridable Sub DrawShape(ByVal shape As cShapeData, _
                                ByVal rcImage As Rectangle, _
                                ByVal g As Graphics, _
                                ByVal clr As Color, _
                                ByVal bDrawLabels As Boolean, _
                                ByVal drawMode As eSketchDrawModeTypes, _
                                ByVal sYMax As Single)

            ' Draw default
            ShapeImage.DrawShape(shape, rcImage, g, clr, drawMode, sYMax, _
                                 Me.YMarkValue, Me.XMarkValue, Me.YMarkLabel, Me.XMarkLabel)

        End Sub

#End Region ' Public access

#Region " Private Methods "

        Private Sub UpdateControl()

            If ((Me.Style And StyleGuide.eStyleFlags.NotEditable) = 0) And (Me.m_shape IsNot Nothing) Then
                Me.Enabled = True
                Me.BackColor = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.IMAGE_BACKGROUND)
            Else
                Me.Enabled = False
                Me.BackColor = Me.m_sg.ApplicationColor(StyleGuide.eApplicationColorType.READONLY_BACKGROUND)
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, states whether a given point lies inside the drawing 
        ''' region.
        ''' </summary>
        ''' <returns>
        ''' True if the given point lies inside the drawing region.
        ''' </returns>
        ''' -------------------------------------------------------------------
        Private Function PointInRegion(ByVal p As Point, ByVal rcImage As Rectangle) As Boolean
            Return (p.X >= rcImage.Left) And (p.X <= rcImage.Right) And _
                   (p.Y >= rcImage.Top) And (p.Y <= rcImage.Bottom)
        End Function

        Private Sub DragXMark(ByVal ptPrev As Point, ByVal ptCur As Point)
            Dim sYMax As Single = Me.YAxisMaxValue
            Dim iXMax As Integer = CInt(IIf(Me.Shape.IsSeasonal, cCore.N_MONTHS, Me.Shape.XMax))
            Dim ptfCur As PointF = ShapeImage.ToModelPoint(ptCur, Me.ClientRectangle, iXMax, sYMax)
            Me.XMarkValue = ptfCur.X
            Me.Refresh()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draws a shape between two click points.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub DrawShape(ByVal ptPrev As Point, ByVal ptCur As Point)

            Dim sYMax As Single = Me.YAxisMaxValue
            Dim iXMax As Integer = CInt(IIf(Me.Shape.IsSeasonal, cCore.N_MONTHS, Me.Shape.XMax))
            Dim ptfPrev As PointF = ShapeImage.ToModelPoint(ptPrev, Me.ClientRectangle, iXMax, sYMax)
            Dim ptfCur As PointF = ShapeImage.ToModelPoint(ptCur, Me.ClientRectangle, iXMax, sYMax)

            Dim iStart As Integer = CInt(Math.Min(ptfPrev.X, ptfCur.X))
            Dim iEnd As Integer = CInt(Math.Max(ptfPrev.X, ptfCur.X))

            'If iStart = iEnd Or iEnd > Me.Shape.XData.XMax Or iStart < 1 Then
            If (iStart < 0) Or (iEnd > Me.Shape.XMax) Then
                Return
            End If

            ' Single click?
            If iStart = iEnd Then
                Me.Shape.ShapeData(iStart) = (ptfCur.Y + ptfPrev.Y) / 2
            Else
                For i As Integer = iStart To iEnd
                    Dim sYTmp As Single = (ptfCur.Y - ptfPrev.Y) * (i - ptfPrev.X) / (ptfCur.X - ptfPrev.X) + ptfPrev.Y
                    Me.Shape.ShapeData(i) = sYTmp
                Next
            End If

            If iEnd = Math.Round((Me.ClientRectangle.Width - 1) * Me.Shape.XMax / Me.ClientRectangle.Width) Then
                For i As Integer = iEnd To Me.Shape.XMax - 1
                    Me.Shape.ShapeData(i) = Me.Shape.ShapeData(iEnd)
                Next
            End If

        End Sub

        Public Sub RepeatSeasonalPattern()

            If Not Me.m_shape.IsSeasonal Then Return

            Dim asValues(Me.Shape.XMax - 1) As Single
            Dim j As Integer = 0

            For i As Integer = 1 To Me.Shape.XMax - 1
                asValues(i) = Me.Shape.ShapeData(j + 1)
                j += 1
                If j = cCore.N_MONTHS Then j = 0
            Next
            Me.Shape.ShapeData = asValues

        End Sub

        Private Function IsNearXMark(ByVal sX As Single) As Boolean

            If Not m_bShowXMark Then Return False

            ' Check if x value is near x mark
            Dim sYMax As Single = Me.YAxisMaxValue
            Dim iXMax As Integer = CInt(IIf(Me.Shape.IsSeasonal, cCore.N_MONTHS, Me.Shape.XMax))
            Dim ptfMouseL As PointF = ShapeImage.ToModelPoint(New PointF(sX - cCLICK_TOLERANCE, 0), Me.ClientRectangle, iXMax, sYMax)
            Dim ptfMouseR As PointF = ShapeImage.ToModelPoint(New PointF(sX + cCLICK_TOLERANCE, 0), Me.ClientRectangle, iXMax, sYMax)

            Return (ptfMouseL.X <= Me.XMarkValue) And (ptfMouseR.X >= Me.XMarkValue)

        End Function

        Private Function IsNearYMark(ByVal ptMouse As PointF) As Boolean
            Return False
        End Function

#End Region ' Private Methods

#Region " Rendering "

        ''' <summary>
        ''' Locked Y scale while drawing
        ''' </summary>
        Private m_sYMaxLock As Single = 0.0!

        ''' <summary>
        ''' This method handls the Paint event and does the actual drawing routine
        ''' It only draws the graph with no other additional info like caption, axises..eg..Those will be drawn in the inherited class if needed
        ''' </summary>
        Private Sub SketchPad_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles MyBase.Paint

            Dim sYMax As Single = Me.YAxisMaxValue
            ' Avoid division by zero
            If (sYMax <= 0.0!) Then sYMax = 1.0!

            ' Check for invalid values
            If (Single.IsNaN(sYMax)) Then Return
            If (Single.IsNegativeInfinity(sYMax)) Then Return
            If (Single.IsPositiveInfinity(sYMax)) Then Return

            Try
                ' Draw
                Me.DrawShape(Me.Shape, Me.ClientRectangle, e.Graphics, Me.ShapeColor, True, Me.SketchDrawMode, sYMax)
            Catch ex As Exception
                ' Woops
            End Try

        End Sub

        Private Sub ProcessMouseInput(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Not Me.Editable Then Return
            If Not Me.Capture Then Return

            Dim bLeftBtnDown As Boolean = (e.Button = MouseButtons.Left)
            'Dim bRightButtonDown As Boolean = (e.Button = MouseButtons.Right)
            Dim ptPosCurrent As Point = New Point(e.X, e.Y)
            Dim sYPrev As Single = Me.m_shape.YMax
            Dim sYNew As Single = 0.0
            Dim rcImage As Rectangle = Me.ClientRectangle

            If (Me.m_ptPosPrevious = Nothing) Then m_ptPosPrevious = ptPosCurrent

            If bLeftBtnDown Then

                Select Case Me.m_editMode
                    Case eMouseInteractionMode.DrawShape
                        Me.DrawShape(Me.m_ptPosPrevious, ptPosCurrent)

                    Case eMouseInteractionMode.DragXMark
                        Me.DragXMark(Me.m_ptPosPrevious, ptPosCurrent)

                    Case eMouseInteractionMode.DragYMark
                    Case eMouseInteractionMode.None

                End Select

                'ElseIf bRightButtonDown Then

                '    If m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto Then
                '        If PointInRegion(Me.m_ptPosPrevious, rcImage) And PointInRegion(ptPosCurrent, rcImage) Then
                '            Dim sYMaxDrag As Single = Me.m_sYMaxLock + (Me.m_ptPosPrevious.Y - ptPosCurrent.Y) * Me.YAxisMaxValue / rcImage.Height
                '            Me.m_sYMaxLock = Math.Max(sYMaxDrag, Me.m_sYMaxLock)
                '        End If
                '    End If
            End If

            Me.m_ptPosPrevious = ptPosCurrent

            Me.Refresh()

            Me.OnShapeChanged()

        End Sub

#End Region ' Rendering

#Region " Event handling "

#Region " Mouse events "

        ''' <summary>
        ''' Mouse click handler; starts mouse capture and initiates shape drawing.
        ''' </summary>
        Private Sub SketchPad_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown

            Dim bShiftPressed As Boolean = (User32.GetAsyncKeyState(&H10) < 0)
            If Not Me.Editable Then Return

            Me.Capture = True

            ' If NOT Shift key pressed release the last mouse pos
            If Not bShiftPressed Then Me.m_ptPosPrevious = Nothing

            Me.Shape.LockUpdates()

            Me.m_sYMaxLock = Me.YAxisMaxValue
            If Me.m_sYMaxLock = 0 Then Me.m_sYMaxLock = 2.0

            If Me.IsNearXMark(e.X) Then
                Me.m_editMode = eMouseInteractionMode.DragXMark
            Else
                Me.m_editMode = eMouseInteractionMode.DrawShape
            End If

            Me.ProcessMouseInput(e)

        End Sub

        ''' <summary>
        ''' Mouse move handler; draws the shape when the mouse input is captured.
        ''' </summary>
        Private Sub SketchPad_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove

            If Not Me.Editable Then Return

            If Me.IsNearXMark(e.X) Then
                Me.Cursor = Cursors.SizeWE
            Else
                Me.Cursor = Cursors.Default
            End If

            If Not Me.Capture Then Return

            Me.ProcessMouseInput(e)

        End Sub

        ''' <summary>
        ''' Mouse up handler; finalizes the shape when the mouse is captured.
        ''' </summary>
        Private Sub SketchPad_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
            Handles MyBase.MouseUp

            If Not Me.Editable Then Return
            If Not Me.Capture Then Return

            ' Clear lock
            Me.m_sYMaxLock = 0.0!
            Me.Capture = False
            Me.m_editMode = eMouseInteractionMode.None

            ' Test auto scale
            If (Me.m_scalemodeYAxis = eAxisAutoScaleModeTypes.Auto) Then
                Dim sYNew As Single = Me.m_shape.YMax(True)
                If (sYNew <> Me.YAxisMaxValue) Then
                    Me.YAxisMaxValue = Math.Max(Me.YAxisMinValue, sYNew)
                End If
            End If

            Me.RepeatSeasonalPattern()

            ' Unlock quietly; OnShapeFinalized will inform the world
            Me.Shape.UnlockUpdates(False)
            Me.OnShapeFinalized()

            Me.Invalidate()

        End Sub

#End Region ' Mouse events

#Region " Local events "

        ''' <summary>
        ''' This generated the shape changed event so its thumbnail image can synchronize with it.
        ''' </summary>
        Protected Overridable Sub OnShapeChanged()
            RaiseEvent ShapeChanged(Me.Shape)
            Me.Invalidate()
        End Sub

        ''' <summary>
        ''' This generated the shape finalized event so the underlying data can be synchronized
        ''' </summary>
        Protected Overridable Sub OnShapeFinalized()
            RaiseEvent ShapeFinalized(Me.Shape, Me)
        End Sub

#End Region ' Local events

#Region " Context menu handlers "

        Private Sub UpdateMenuItemStates()

            Me.LineToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Line)
            Me.FillToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Fill)
            Me.DotsToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Dots)

            If Me.Handler IsNot Nothing Then
                Me.OptionsToolStripMenuItem.Visible = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
                Me.OptionsToolStripMenuItem.Enabled = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions)

                Me.LoadToolStripMenuItem.Visible = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.Load)
                Me.LoadToolStripMenuItem.Enabled = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.Load)

                Me.ValueToolStripMenuItem.Visible = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.Modify)
                Me.ValueToolStripMenuItem.Enabled = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.Modify)

                Me.ResetToolStripMenuItem.Visible = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.Reset)
                Me.ResetToolStripMenuItem.Enabled = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.Reset)

                Me.SaveToolStripMenuItem.Visible = Me.Handler.SupportCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
                Me.SaveToolStripMenuItem.Enabled = Me.Handler.EnableCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
            End If

        End Sub

        Private Sub LineOnlyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles LineToolStripMenuItem.Click

            Me.SketchDrawMode = eSketchDrawModeTypes.Line
            Me.UpdateMenuItemStates()

        End Sub

        Private Sub FillToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles FillToolStripMenuItem.Click

            Me.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.UpdateMenuItemStates()

        End Sub

        Private Sub DotsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles DotsToolStripMenuItem.Click

            Me.SketchDrawMode = eSketchDrawModeTypes.Dots
            Me.UpdateMenuItemStates()

        End Sub

        Private Sub AxisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles AxisToolStripMenuItem.Click

            AxisToolStripMenuItem.Checked = Not AxisToolStripMenuItem.Checked
            Me.m_bShowAxis = AxisToolStripMenuItem.Checked

        End Sub

        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles AutoScaleToolStripMenuItem.Click

            AutoScaleToolStripMenuItem.Checked = Not AutoScaleToolStripMenuItem.Checked
            If AutoScaleToolStripMenuItem.Checked Then
                Me.m_scalemodeYAxis = eAxisAutoScaleModeTypes.Auto
            Else
                Me.m_scalemodeYAxis = eAxisAutoScaleModeTypes.Fixed
            End If

        End Sub

        Private Sub spContextMenuStrip_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) _
            Handles spContextMenuStrip.Opening

            Me.AxisToolStripMenuItem.Checked = Me.m_bShowAxis
            Me.AutoScaleToolStripMenuItem.Checked = (Me.m_scalemodeYAxis = eAxisAutoScaleModeTypes.Auto)

            Me.UpdateMenuItemStates()
        End Sub

        ''' <summary>
        ''' The event handler; handles a Reset toolstrip button click.
        ''' </summary>
        Private Sub OnResetShapeClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles ResetToolStripMenuItem.Click

            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Reset)
            End If
        End Sub

        ''' <summary>
        ''' Event handler; handles a Options toolstrip menu click 
        ''' </summary>
        Private Sub OnOptionClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles OptionsToolStripMenuItem.Click

            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
            End If
        End Sub

        ''' <summary>
        ''' Event handler; handles a Save image value toolstrip item click
        ''' </summary>
        Private Sub OnSaveImageClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles SaveToolStripMenuItem.Click

            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.SaveAsImage, _
                        New cShapeData() {Me.Shape}, Me)
            End If
        End Sub

        ''' <summary>
        ''' Event handler; handles a Shape value toolstrip item click
        ''' </summary>
        Private Sub OnShapeValueClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles ValueToolStripMenuItem.Click

            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Modify)
            End If
        End Sub

        ''' <summary>
        ''' Event handler; handles a Load shape toolstrip item click
        ''' </summary>
        Private Sub OnLoadShapeClick(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles LoadToolStripMenuItem.Click

            If Me.Handler IsNot Nothing Then
                Me.Handler.ExecuteCommand(cShapeGUIHandler.eShapeCommandTypes.Load)
            End If
        End Sub

#End Region ' Context menu handlers

#End Region 'Event handling

    End Class

End Namespace



