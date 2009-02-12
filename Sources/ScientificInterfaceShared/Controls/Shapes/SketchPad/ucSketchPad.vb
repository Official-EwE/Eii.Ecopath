'==============================================================================
'
' $Log: ucSketchPad.vb,v $
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

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' This Sketchpad control class is used to render the shape and support
    ''' mouse interaction. It can be used as the base class for both forcing functions and 
    ''' mediation functions.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Class ucSketchPad

#Region " Variables "

        ''' <summary></summary>
        Private m_core As cCore = Nothing
        ''' <summary>The manager of this control.</summary>
        Private m_handler As ShapeGUIHandler = Nothing
        ''' <summary>The shape shown in this control.</summary>
        Private m_shape As cShapeData = Nothing
        ''' <summary>Last known mouse location when drawing.</summary>
        Private m_ptPosPrevious As Point = Nothing

        ' The variables for toggling addtional features on and off
        ''' <summary></summary>
        Private m_YAxisAutoScaleMode As eAxisAutoScaleModeTypes = eAxisAutoScaleModeTypes.Auto
        ''' <summary></summary>
        Private m_RightClickAutoScaleMode As eRightClickAutoScaleModeTypes = eRightClickAutoScaleModeTypes.Auto

        ''' <summary></summary>
        Protected m_color As Color = Drawing.Color.AliceBlue
        ''' <summary></summary>
        Protected m_AxisDisplayMode As eAxisDisplayModeTypes = eAxisDisplayModeTypes.Show
        ''' <summary></summary>
        Protected m_SketchDrawMode As eSketchDrawModeTypes = eSketchDrawModeTypes.Fill
        ''' <summary></summary>
        Protected m_ShapeType As eShapeCategoryTypes = eShapeCategoryTypes.NotSet

        ''' <summary></summary>
        Private m_sYMax As Single = cCore.NULL_VALUE
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
        Public Delegate Sub ShapeChangedEventHandler(ByVal shape As cShapeData)
        ''' <summary></summary>
        Public Delegate Sub ShapeFinalizedEventHandler(ByVal shape As cShapeData)
        ''' <summary></summary>
        Public Event ShapeChanged As ShapeChangedEventHandler
        ''' <summary></summary>
        Public Event ShapeFinalized As ShapeFinalizedEventHandler

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

            ' Default rendering mode
            Me.m_SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.m_AxisDisplayMode = eAxisDisplayModeTypes.Show
            Me.m_YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Me.m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto

        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the handler 
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Handler() As ShapeGUIHandler
            Get
                Return Me.m_handler
            End Get
            Set(ByVal value As ShapeGUIHandler)
                Me.m_handler = value
                Me.UpdateMenuItemStates()
            End Set
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Public Overridable Property Shape() As cShapeData

            Get
                Return Me.m_shape
            End Get

            Set(ByVal value As cShapeData)

                ' Enable mouse input when a shape is selected
                Me.Enabled = Not Object.ReferenceEquals(value, Nothing)

                ' Store new shape ref
                Me.m_shape = value

                ' Broadcast change
                Me.OnShapeChanged()
            End Set

        End Property

        ''' <summary>
        ''' 
        ''' </summary>
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

        ''' <summary>
        ''' This draw mode lets the user choose to display it as a filled graph or a line.
        ''' </summary>
        Public Property SketchDrawMode() As eSketchDrawModeTypes

            Get
                Return Me.m_SketchDrawMode
            End Get

            Set(ByVal value As eSketchDrawModeTypes)
                Me.m_SketchDrawMode = value
                Me.Invalidate()
            End Set

        End Property

        ''' <summary>
        ''' This mode lets the user to hide and show the axis.
        ''' </summary>
        Public Property AxisDisplayMode() As eAxisDisplayModeTypes

            Get
                Return Me.m_AxisDisplayMode
            End Get

            Set(ByVal value As eAxisDisplayModeTypes)
                Me.m_AxisDisplayMode = value
                Me.Invalidate()
            End Set

        End Property

        ''' <summary>
        ''' This mode lets the user to enable and disable the auto scale using the mouse
        ''' </summary>
        Public Property YAxisAutoScaleMode() As eAxisAutoScaleModeTypes

            Get
                Return Me.m_YAxisAutoScaleMode
            End Get

            Set(ByVal value As eAxisAutoScaleModeTypes)
                Me.m_YAxisAutoScaleMode = value
                Me.Invalidate()
            End Set

        End Property

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

        Public Property YAxisMinValue() As Single
            Get
                Return Me.m_sYMin
            End Get
            Set(ByVal value As Single)
                Me.m_sYMin = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Value for horizontal (Y mark) line
        ''' </summary>
        Public Property YMarkValue() As Single
            Get
                Return Me.m_sYMarkValue
            End Get
            Set(ByVal value As Single)
                Me.m_sYMarkValue = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Label for horizontal (Y mark) line
        ''' </summary>
        Public Property YMarkLabel() As String
            Get
                Return Me.m_strYMarkLabel
            End Get
            Set(ByVal value As String)
                Me.m_strYMarkLabel = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Value for vertical (X mark) line
        ''' </summary>
        Public Property XMarkValue() As Single
            Get
                Return Me.m_sXMarkValue
            End Get
            Set(ByVal value As Single)
                Me.m_sXMarkValue = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' Label for vertical (X mark) line
        ''' </summary>
        Public Property XMarkLabel() As String
            Get
                Return Me.m_strXMarkLabel
            End Get
            Set(ByVal value As String)
                Me.m_strXMarkLabel = value
                Me.Invalidate()
            End Set
        End Property

        ''' <summary>
        ''' This mode lets the user to enable and disable the auto scale by mouse right click
        ''' </summary>
        Public Property RightClickAutoScaleMode() As eRightClickAutoScaleModeTypes

            Get
                Return Me.m_RightClickAutoScaleMode
            End Get

            Set(ByVal value As eRightClickAutoScaleModeTypes)
                Me.m_RightClickAutoScaleMode = value
                Me.Invalidate()
            End Set

        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Public Property Color() As Color

            Get
                Return Me.m_color
            End Get

            Set(ByVal value As Color)
                Me.m_color = value
                Me.Invalidate()
            End Set

        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        Public ReadOnly Property ShapeType() As eShapeCategoryTypes

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
                Return m_ShapeType
            End Get

        End Property

        Public Overridable Function SaveAsImage(ByVal shape As cShapeData, ByVal strFileName As String, ByVal imgFormat As ImageFormat, _
                ByRef strError As String) As Boolean

            Dim rcClient As Rectangle = Me.ClientRectangle()
            Dim bmp As New Bitmap(rcClient.Width, rcClient.Height, Imaging.PixelFormat.Format32bppArgb)
            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim fs As IO.FileStream = Nothing
            Dim bSucces As Boolean = True

            ' Render the shape
            g.Clear(Me.BackColor)

            Try
                Me.DrawShape(shape, rcClient, g, Me.Color, True, Me.SketchDrawMode, Me.YAxisMaxValue)
            Catch ex As Exception
                bSucces = False
            End Try

            ' Try to open the stream
            Try
                fs = New FileStream(strFileName, FileMode.Create)
                bmp.Save(fs, imgFormat)
                fs.Close()
            Catch ex As UnauthorizedAccessException
                ' File cannot be written
                strError = ex.Message
                'MsgBox(String.Format("File {0} cannot be written", strFileName), MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
                bSucces = False
            Catch ex As Exception
                ' An error occurred
                'MsgBox(String.Format("An internal error occurred while attempting to write file {0}", strFileName), MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
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

        ''' <summary>
        ''' The method is to test if the point is inside the drawing region. Return true if it is. 
        ''' </summary>
        Private Function PointInRegion(ByVal p As Point, ByVal rcImage As Rectangle) As Boolean
            Return (p.X >= rcImage.Left) And (p.X <= rcImage.Right) And _
                   (p.Y >= rcImage.Top) And (p.Y <= rcImage.Bottom)
        End Function

        ''' <summary>
        ''' This method does the integration when mouse moves.
        ''' </summary>
        Private Sub IntegrateShape(ByVal ptPrev As Point, ByVal ptCur As Point)

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
        Private Sub SketchPad_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint

            Dim sYMax As Single = Me.YAxisMaxValue
            ' Avoid division by zero
            If (sYMax <= 0.0!) Then sYMax = 1.0!

            ' Check for invalid values
            If (Single.IsNaN(sYMax)) Then Return
            If (Single.IsNegativeInfinity(sYMax)) Then Return
            If (Single.IsPositiveInfinity(sYMax)) Then Return

            Try
                ' Draw
                Me.DrawShape(Me.Shape, Me.ClientRectangle, e.Graphics, Me.Color, True, Me.SketchDrawMode, sYMax)
            Catch ex As Exception
                ' Woops
            End Try

        End Sub

        Private Sub ProcessMouseInput(ByVal e As System.Windows.Forms.MouseEventArgs)

            If Not Me.Editable Then Return
            If Not Me.Capture Then Return

            Dim bLeftBtnDown As Boolean = (e.Button = MouseButtons.Left)
            Dim bRightButtonDown As Boolean = (e.Button = MouseButtons.Right)
            Dim ptPosCurrent As Point = New Point(e.X, e.Y)
            Dim sYPrev As Single = Me.m_shape.YMax
            Dim sYNew As Single = 0.0
            Dim rcImage As Rectangle = Me.ClientRectangle

            If (Me.m_ptPosPrevious = Nothing) Then m_ptPosPrevious = ptPosCurrent

            If bLeftBtnDown Then

                Me.IntegrateShape(Me.m_ptPosPrevious, ptPosCurrent)

            ElseIf bRightButtonDown Then

                If m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto Then
                    If PointInRegion(Me.m_ptPosPrevious, rcImage) And PointInRegion(ptPosCurrent, rcImage) Then
                        Dim sYMaxDrag As Single = Me.m_sYMaxLock + (Me.m_ptPosPrevious.Y - ptPosCurrent.Y) * Me.YAxisMaxValue / rcImage.Height
                        Me.m_sYMaxLock = Math.Max(sYMaxDrag, Me.m_sYMaxLock)
                    End If
                End If
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

            'Console.WriteLine(bShiftPressed)

            ' If NOT Shift key pressed release the last mouse pos
            If Not bShiftPressed Then Me.m_ptPosPrevious = Nothing

            Me.Shape.LockUpdates()

            Me.m_sYMaxLock = Me.YAxisMaxValue
            If Me.m_sYMaxLock = 0 Then Me.m_sYMaxLock = 2.0

            Me.ProcessMouseInput(e)

        End Sub

        ''' <summary>
        ''' Mouse move handler; draws the shape when the mouse input is captured.
        ''' </summary>
        Private Sub SketchPad_MouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove

            If Not Me.Editable Then Return
            If Not Me.Capture Then Return

            Me.ProcessMouseInput(e)

        End Sub

        ''' <summary>
        ''' Mouse up handler; finalizes the shape when the mouse is captured.
        ''' </summary>
        Private Sub SketchPad_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp

            If Not Me.Editable Then Return
            If Not Me.Capture Then Return

            ' Clear lock
            Me.m_sYMaxLock = 0.0!
            Me.Capture = False

            ' Test auto scale
            If (Me.m_YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto) Then
                Dim sYNew As Single = Me.m_shape.YMax(True)
                If (sYNew <> Me.YAxisMaxValue) Then
                    Me.YAxisMaxValue = Math.Max(Me.YAxisMinValue, sYNew)
                    'Me.YAxisMaxValue += ((Me.m_ptPosPrevious.Y - ptPosCurrent.Y) * Me.YAxisMaxValue / rcImage.Height)
                End If
            End If

            Me.RepeatSeasonalPattern()
            ' Unlock quietly; OnShapeFinalized will inform the world
            Me.Shape.UnlockUpdates(False)


            Me.OnShapeFinalized()

            'Me.Rescale()
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
            RaiseEvent ShapeFinalized(Me.Shape)
        End Sub

#End Region ' Local events

#Region " Context menu handlers "

        Private Sub UpdateMenuItemStates()

            LineToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Line)
            FillToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Fill)
            DotsToolStripMenuItem.Checked = (Me.SketchDrawMode = eSketchDrawModeTypes.Dots)

            If Me.Handler IsNot Nothing Then
                Me.OptionsToolStripMenuItem.Visible = Me.Handler.SupportCommand(ShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
                Me.OptionsToolStripMenuItem.Enabled = Me.Handler.EnableCommand(ShapeGUIHandler.eShapeCommandTypes.DisplayOptions)

                Me.LoadToolStripMenuItem.Visible = Me.Handler.SupportCommand(ShapeGUIHandler.eShapeCommandTypes.Load)
                Me.LoadToolStripMenuItem.Enabled = Me.Handler.EnableCommand(ShapeGUIHandler.eShapeCommandTypes.Load)

                Me.ValueToolStripMenuItem.Visible = Me.Handler.SupportCommand(ShapeGUIHandler.eShapeCommandTypes.Modify)
                Me.ValueToolStripMenuItem.Enabled = Me.Handler.EnableCommand(ShapeGUIHandler.eShapeCommandTypes.Modify)

                Me.ResetToolStripMenuItem.Visible = Me.Handler.SupportCommand(ShapeGUIHandler.eShapeCommandTypes.Reset)
                Me.ResetToolStripMenuItem.Enabled = Me.Handler.EnableCommand(ShapeGUIHandler.eShapeCommandTypes.Reset)

                Me.SaveToolStripMenuItem.Visible = Me.Handler.SupportCommand(ShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
                Me.SaveToolStripMenuItem.Enabled = Me.Handler.EnableCommand(ShapeGUIHandler.eShapeCommandTypes.SaveAsImage)
            End If

        End Sub

        Private Sub LineOnlyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LineToolStripMenuItem.Click
            Me.SketchDrawMode = eSketchDrawModeTypes.Line
            Me.UpdateMenuItemStates()
        End Sub

        Private Sub FillToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FillToolStripMenuItem.Click
            Me.SketchDrawMode = eSketchDrawModeTypes.Fill
            Me.UpdateMenuItemStates()
        End Sub

        Private Sub DotsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DotsToolStripMenuItem.Click
            Me.SketchDrawMode = eSketchDrawModeTypes.Dots
            Me.UpdateMenuItemStates()
        End Sub

        Private Sub AxisToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AxisToolStripMenuItem.Click
            AxisToolStripMenuItem.Checked = Not AxisToolStripMenuItem.Checked

            If AxisToolStripMenuItem.Checked Then
                Me.AxisDisplayMode = eAxisDisplayModeTypes.Show
            Else
                Me.AxisDisplayMode = eAxisDisplayModeTypes.Hide
            End If
        End Sub

        Private Sub AutoScaleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoScaleToolStripMenuItem.Click

            AutoScaleToolStripMenuItem.Checked = Not AutoScaleToolStripMenuItem.Checked
            If AutoScaleToolStripMenuItem.Checked Then
                m_YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto
            Else
                m_YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Fixed
            End If

        End Sub

        Private Sub RightMouseButtonToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RightMouseButtonToolStripMenuItem.Click
            RightMouseButtonToolStripMenuItem.Checked = Not RightMouseButtonToolStripMenuItem.Checked
            If RightMouseButtonToolStripMenuItem.Checked Then
                m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto
            Else
                m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Fixed
            End If
        End Sub

        Private Sub spContextMenuStrip_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles spContextMenuStrip.Opening
            If m_AxisDisplayMode = eAxisDisplayModeTypes.Show Then
                AxisToolStripMenuItem.Checked = True
            Else
                AxisToolStripMenuItem.Checked = False
            End If

            If m_RightClickAutoScaleMode = eRightClickAutoScaleModeTypes.Auto Then
                RightMouseButtonToolStripMenuItem.Checked = True
            Else
                RightMouseButtonToolStripMenuItem.Checked = False
            End If

            If m_YAxisAutoScaleMode = eAxisAutoScaleModeTypes.Auto Then
                AutoScaleToolStripMenuItem.Checked = True
            Else
                AutoScaleToolStripMenuItem.Checked = False
            End If

            Me.UpdateMenuItemStates()
        End Sub

        ''' <summary>
        ''' The event handler; handles a Reset toolstrip button click.
        ''' </summary>
        Private Sub OnResetShapeClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResetToolStripMenuItem.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Reset)
        End Sub

        ''' <summary>
        ''' Event handler; handles a Options toolstrip menu click 
        ''' </summary>
        Private Sub OnOptionClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionsToolStripMenuItem.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.DisplayOptions)
        End Sub

        ''' <summary>
        ''' Event handler; handles a Save image value toolstrip item click
        ''' </summary>
        Private Sub OnSaveImageClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.SaveAsImage, Me.Shape, Me)
        End Sub

        ''' <summary>
        ''' Event handler; handles a Shape value toolstrip item click
        ''' </summary>
        Private Sub OnShapeValueClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ValueToolStripMenuItem.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Modify)
        End Sub

        ''' <summary>
        ''' Event handler; handles a Load shape toolstrip item click
        ''' </summary>
        Private Sub OnLoadShapeClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadToolStripMenuItem.Click
            If Me.Handler IsNot Nothing Then Me.Handler.ExecuteCommand(ShapeGUIHandler.eShapeCommandTypes.Load)
        End Sub

#End Region ' Context menu handlers

#End Region 'Event handling

    End Class

End Namespace



