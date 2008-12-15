'==============================================================================
'
' $Log: EcosimOutputLineGraph.vb,v $
' Revision 1.3  2008/12/15 15:53:25  jeroens
' no message
'
' Revision 1.2  2008/11/27 03:10:43  jeroens
' Group visible flags maintained by style guide, no longer by AppLauncher
'
' Revision 1.1  2008/09/26 07:31:49  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.24  2008/09/23 16:14:56  jeroens
' TS 'Apply' -> 'Enable'
'
' Revision 1.23  2008/08/12 16:46:07  jeroens
' Added sanity checks
'
' Revision 1.22  2008/08/12 16:11:24  jeroens
' Grr
'
' Revision 1.21  2008/08/12 16:10:08  jeroens
' Fixed crash
'
' Revision 1.20  2008/05/30 23:50:22  jeroens
' Added Yscale sanity checks
'
' Revision 1.19  2008/05/20 14:36:59  jeroens
' Fixed issue 317
'
' Revision 1.18  2008/05/14 18:45:53  jeroens
' Fixed autoscale issue
'
' Revision 1.17  2008/05/07 01:39:03  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.16  2008/05/05 08:34:28  jeroens
' Uses Styleguide group colors instead of group PoolColorArgb
'
' Revision 1.15  2008/03/28 17:18:18  jeroens
' Added comments
'
' Revision 1.14  2008/02/13 21:37:34  jeroens
' Fixed issue 330
'
' Revision 1.13  2008/02/08 01:17:23  jeroens
' TS years reflected in graph
'
' Revision 1.12  2008/01/21 04:06:38  jeroens
' Fixed shape max scale issues, once and for all
'
' Revision 1.11  2007/12/17 16:29:41  jeroens
' * Added sanity check
'
' Revision 1.10  2007/12/15 02:40:31  jeroens
' * Fixed autoscale vs user def. scale
'
' Revision 1.9  2007/11/21 17:39:43  jeroens
' * Minor cleanup
'
' Revision 1.8  2007/11/10 17:55:40  jeroens
' * Summary line dragging disabled when lines not visible (fixes bug 330)
'
' Revision 1.7  2007/11/02 16:32:49  joeb
' added some comments to code
'
' Revision 1.6  2007/11/02 16:23:58  joeb
' Public interface to redraw summary graph lines
'
' Revision 1.5  2007/10/18 22:07:00  joeb
' Plot reference data in middle of year
'
' Revision 1.4  2007/10/15 16:48:42  joeb
' Fixed indexing bug in line selection code
'
' Revision 1.3  2007/10/14 20:14:25  jeroens
' * Fixed potentially dangerous mybase-linked handlers
' * Fixed line lookup logic
'
' Revision 1.2  2007/10/14 19:16:26  jeroens
' * Responds to styleguide changes
'
' Revision 1.1  2007/10/11 18:14:33  jeroens
' Initial version, after renaming and reworking
'
' Revision 1.1  2007/10/10 18:34:56  jeroens
' * Prepared for live colours
'
' Revision 1.28  2007/10/09 18:58:23  joeb
' Fixed Scale and Progress bar
'
' Revision 1.27  2007/10/05 18:15:49  joeb
' Fixed Scale and Overlay interations
'
' Revision 1.26  2007/09/28 19:32:41  joeb
' Scaling of XAxis
'
' Revision 1.25  2007/09/19 18:32:21  fgao
' Reset tab orders and add user-set scale Y value to Biomass drawing..
'
' Revision 1.24  2007/09/07 00:09:15  fgao
' mouse hover for Ecosim biomass output..
'
' Revision 1.23  2007/09/06 19:04:26  fgao
' Plot both Biomass abs and rel TS data..
'
' Revision 1.22  2007/09/06 18:45:10  fgao
' Add the option of setting Y axis value by user.
'
' Revision 1.21  2007/08/29 05:07:22  fgao
' Fixed summary line bug...caused by StatusStrip's update --> Resize method from SetVariable method..
'
' Revision 1.20  2007/08/29 00:01:10  fgao
' Working on Setting summary time value back... -> A bug too..
'
' Revision 1.19  2007/08/24 22:54:43  fgao
' Add a progress bar ... Temporary test for incremental drawing..
'
' Revision 1.18  2007/08/10 23:23:40  fgao
' Finish ucBiomassPlot, make them work for both MCRun and RunEcosim UI,
' Add annual plot options etc.
'
' Revision 1.17  2007/08/10 00:38:05  fgao
' More refined drawings..
'
' Revision 1.16  2007/08/09 21:22:32  fgao
' Add Clear bkg method..
'
' Revision 1.15  2007/08/09 00:29:51  fgao
' Add debugging code and fix the run scale value to 5.0F for now..
'
' Revision 1.14  2007/08/07 16:41:56  jeroens
' * Fixed coding guidelines
'
' Revision 1.13  2007/08/07 03:00:02  jeroens
' + Added header
' * Fixed line drag cursor
' + Added localization ToDo's
'
'==============================================================================

#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports ScientificInterface.Other

#End Region

Namespace Ecosim

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' <para>The performance of this graph is abyssimal. All LineTo methods should
    ''' be replaced by GraphPaths, scaled via transformation matrix.</para>
    ''' <para>GraphPaths can be appended to when new TS data arrives.</para>
    ''' <para>GraphPaths can be drawn in the appropriate group colours.</para>
    ''' <para>GraphPaths can be auto-scaled via a transformation matrix.</para>
    ''' </remarks>
    Public Class EcosimOutputLineGraph

        Private m_core As cCore
        Private m_EcosimModelParams As cEcoSimModelParameters

        Private m_aLayers As New List(Of cLayer)
        Private m_bmpBackBuffer As Bitmap
        Private m_sMaxXAxis As Single
        Private m_sMaxYAxis As Single
        Private m_asTSData As Single(,)
        Private m_abHasTSData() As Boolean

        Private m_sXStart As Single = 0.0!
        Private m_sXEnd As Single = 0.0!
        Private m_bMoveTimeStart As Boolean = False
        Private m_bMoveTimeEnd As Boolean = False

        Private m_bGrayOut As Boolean = False
        Private m_bShowSummaryLines As Boolean = False
        Private m_bShowTSData As Boolean = True
        Private m_bIsOverlay As Boolean
        Private m_bIsShowAnnualOutput As Boolean
        Private m_bFixedScale As Boolean

        Private Const cMOUSE_TOLERANCE As Integer = 3
        Private Const cDEFAULT_YSCALE As Single = 2.0F

        Private m_sFixedYScale As Single = 0.0

        Private m_sg As StyleGuide = Nothing

#Region " Helper classes "

#Region " Layer "

        Public Class cLayer

            Private m_Lines As New List(Of cGroupLine)
            Private m_IsShown As Boolean
            Private m_MaxY As Single

            Public Sub New()
                m_Lines = Nothing
                m_IsShown = False
                m_MaxY = Single.MinValue
            End Sub

            Public Sub New(ByRef lines As List(Of cGroupLine), _
                            Optional ByVal isShown As Boolean = True)
                m_Lines = lines
                m_IsShown = isShown
                m_MaxY = Single.MinValue

            End Sub

            Public Sub New(ByRef v(,) As Single, Optional ByVal isShown As Boolean = True)

                Dim core As cCore = cCore.GetInstance()
                Dim group As cEcoPathGroupInput = Nothing
                Dim line As cGroupLine = Nothing
                Dim sg As StyleGuide = StyleGuide.GetInstance()

                For i As Integer = 1 To v.GetLength(0) - 1
                    Dim pt(v.GetLength(1) - 1) As PointF

                    pt(0) = New Point(0, 1)

                    ' SL: Need to have core supply with 1 at TS 0, 
                    ' this should be changed back.

                    For j As Integer = 1 To v.GetLength(1) - 1
                        pt(j) = New PointF(j - 1, v(i, j))
                    Next

                    group = core.EcoPathGroupInputs(i)
                    line = New cGroupLine(pt, group, sg.GroupVisible(i))
                    m_Lines.Add(line)
                Next

                m_IsShown = isShown
                m_MaxY = Single.MinValue

            End Sub

            Public Property IsShown() As Boolean
                Get
                    Return m_IsShown
                End Get
                Set(ByVal value As Boolean)
                    m_IsShown = value
                End Set
            End Property

            Public ReadOnly Property Lines() As List(Of cGroupLine)
                Get
                    Return m_Lines
                End Get
            End Property

            Public ReadOnly Property MaxY() As Single

                Get
                    If m_MaxY = Single.MinValue Then
                        m_MaxY = CalculateMaxY()
                    End If
                    Return m_MaxY
                End Get

            End Property

            Private Function CalculateMaxY() As Single

                Dim ret As Single = -1
                If Not m_Lines Is Nothing Then
                    ret = m_Lines(0).MaxY
                    For i As Integer = 1 To m_Lines.Count - 1
                        If ret < m_Lines(i).MaxY Then ret = m_Lines(i).MaxY
                    Next
                End If

                Return ret

            End Function

        End Class

#End Region ' Layer

#Region " Line "

        Public Class cGroupLine

            Private m_aPoints() As PointF = Nothing
            Private m_sWidth As Single = 0.0!
            Private m_bShown As Boolean = False
            Private m_bGrayedOut As Boolean = False
            Private m_sMaxY As Single = 0.0!
            Private m_group As cEcoPathGroupInput = Nothing

            Public Sub New(ByVal points() As PointF, _
                    ByVal group As cEcoPathGroupInput, _
                    Optional ByVal isShown As Boolean = True, _
                    Optional ByVal isGrayOut As Boolean = False, _
                    Optional ByVal width As Single = 1)

                ' Sanity checks
                Debug.Assert(group IsNot Nothing)

                Me.m_aPoints = points
                Me.m_group = group
                Me.m_bShown = isShown
                Me.m_bGrayedOut = isGrayOut
                Me.m_sWidth = width
                Me.m_sMaxY = Single.MinValue

            End Sub

            ''' <summary>
            ''' Number of points in the line
            ''' </summary>
            Public ReadOnly Property NPoints() As Integer
                Get
                    Return m_aPoints.Length - 1 '????
                End Get
            End Property

            Public Property IsShown() As Boolean
                Get
                    Return m_bShown
                End Get
                Set(ByVal value As Boolean)
                    m_bShown = value
                End Set
            End Property

            Public Property IsGrayOut() As Boolean
                Get
                    Return m_bGrayedOut
                End Get
                Set(ByVal value As Boolean)
                    m_bGrayedOut = value
                End Set
            End Property

            Public Property Width() As Single
                Get
                    Return m_sWidth
                End Get
                Set(ByVal value As Single)
                    If value <= 0 Then
                        m_sWidth = 1
                    Else
                        m_sWidth = value
                    End If
                End Set
            End Property

            Public ReadOnly Property Name() As String
                Get
                    Return Me.m_group.Name
                End Get
            End Property

            Public ReadOnly Property Group() As cEcoPathGroupInput
                Get
                    Return Me.m_group
                End Get
            End Property

            ''' <summary>
            ''' Returns Point given an index
            ''' </summary>
            ''' <param name="i">Index that is 0 Based</param>
            ''' <returns>The Value</returns>
            ''' <remarks>Converted to 0 Based</remarks>
            Public Property Point(ByVal i As Integer) As PointF
                Get
                    If i < m_aPoints.Length And i >= 0 Then
                        Return m_aPoints(i)
                    End If
                End Get
                Set(ByVal value As PointF)
                    If i < m_aPoints.Length And i >= 0 Then
                        m_aPoints(i) = value
                    End If
                End Set
            End Property

            Public ReadOnly Property MaxY() As Single
                Get
                    If m_sMaxY = Single.MinValue Then
                        m_sMaxY = CalculateMaxY()
                    End If
                    Return m_sMaxY
                End Get
            End Property

            Private Function CalculateMaxY() As Single
                Dim ret As Single = -1
                If Not m_aPoints Is Nothing Then
                    ret = m_aPoints(1).Y

                    ' Starts with 1 because it skips the very first value
                    For i As Integer = 1 To m_aPoints.Length - 1
                        If ret < m_aPoints(i).Y Then ret = m_aPoints(i).Y
                    Next
                End If

                Return ret

            End Function

        End Class

#End Region ' Line

#End Region ' Helper classes

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

        End Sub

        'jb added this to set the size of the xaxis in response to a user changing the number of years
        'keep your finger crossed
        Public Property XAxis() As Single
            Get
                Return m_sMaxXAxis
            End Get
            Set(ByVal value As Single)
                m_sMaxXAxis = value
            End Set
        End Property

        Public ReadOnly Property Layers() As List(Of cLayer)
            Get
                Return m_aLayers
            End Get
        End Property

        Public Property IsGrayOut() As Boolean
            Get
                Return m_bGrayOut
            End Get
            Set(ByVal value As Boolean)
                m_bGrayOut = value
            End Set
        End Property

        Public Property IsTSDataShown() As Boolean
            Get
                Return m_bShowTSData
            End Get
            Set(ByVal value As Boolean)
                m_bShowTSData = value
            End Set
        End Property

        Public Property IsSummaryLinesShown() As Boolean
            Get
                Return m_bShowSummaryLines
            End Get
            Set(ByVal value As Boolean)
                m_bShowSummaryLines = value
            End Set
        End Property

        Public Property IsOverlay() As Boolean
            Get
                Return m_bIsOverlay
            End Get
            Set(ByVal value As Boolean)
                m_bIsOverlay = value
                'If m_bIsOverlay Then
                '    m_sMaxYAxis = DEFAULT_YSCALE 'Fixed scale during running
                '    ClearPlot()
                '    GenerateOutputImage()
                'End If

            End Set
        End Property

        Public Property IsShowAnnualOutput() As Boolean
            Get
                Return m_bIsShowAnnualOutput
            End Get
            Set(ByVal value As Boolean)
                m_bIsShowAnnualOutput = value
            End Set
        End Property

        Public Property FixedYAxisScaleMax() As Single
            Get
                Return m_sMaxYAxis
            End Get
            Set(ByVal value As Single)

                If (value < 0.0!) Then
                    Debug.Assert(False, "Scale must be larger than 0")
                    Return
                End If

                If (value <> Me.m_sFixedYScale) Then
                    Me.m_sFixedYScale = value

                    If (IsFixedYAxisScale = False) Or (Me.m_sFixedYScale <> Me.m_sMaxYAxis) Then
                        Me.GenerateOutputImage()
                    End If
                End If

            End Set
        End Property

        Public Property IsFixedYAxisScale() As Boolean
            Get
                Return m_bFixedScale
            End Get
            Set(ByVal value As Boolean)
                Me.m_bFixedScale = value
                'only redraw if the user scale in different then the graph scale
                If (IsFixedYAxisScale = False) Or (Me.m_sFixedYScale <> Me.m_sMaxYAxis) Then
                    Me.GenerateOutputImage()
                End If
            End Set
        End Property


        Public Sub Reset()
            m_aLayers.Clear()
            If m_bmpBackBuffer IsNot Nothing Then
                ' ToDo: use colour from styleguide
                Graphics.FromImage(m_bmpBackBuffer).Clear(Color.White)
            End If
            Me.Invalidate()
        End Sub

        Public Sub AddValues(ByRef v(,) As Single, ByVal drawIncrem As Boolean)

            If Not m_bIsOverlay Then
                m_aLayers.Clear()
            End If
            'Add 2-d values into the line graph
            Dim nLayer As New cLayer(v)
            m_aLayers.Add(nLayer)

            If Not m_bIsOverlay Then
                '  maxYValue = m_aLayers(m_aLayers.Count - 1).MaxY
                '    ScaleAxis()
            End If

            GenerateOutputImage(drawIncrem)

        End Sub

        Private Sub ScaleYAxis()

            Dim sYMax As Single = Me.m_sMaxYAxis

            If Me.m_bFixedScale Then
                sYMax = Me.m_sFixedYScale
            Else
                sYMax = CSng(1.05! * CalculateMaxYValue())
                If (sYMax < 0.0!) Then
                    sYMax = cDEFAULT_YSCALE
                End If
            End If

            If sYMax <> Me.m_sMaxYAxis Then
                Console.WriteLine("Adjusting YScale from {0} to {1}", Me.m_sMaxYAxis, sYMax)
                Me.m_sMaxYAxis = sYMax

                Debug.Assert(Me.m_sMaxYAxis > 0)
            End If

        End Sub

        Private Sub OnHandleLoad(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load

            Dim ptf As PointF = Nothing

            ' Enable double buffering
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.UserPaint, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            Me.Dock = DockStyle.Fill

            Me.m_core = cCore.GetInstance()
            Me.m_EcosimModelParams = Me.m_core.EcoSimModelParameters()

            ' Create backbuffer bitmap from the drawing area, ie. the picturebox used here. 
            Me.m_bmpBackBuffer = New Bitmap(Me.Width, Me.Height, Me.CreateGraphics())

            ptf = toScreenPoint(New PointF(Me.m_EcosimModelParams.StartSummaryTime * cCore.N_MONTHS, 0))
            Me.m_sXStart = ptf.X
            ptf = toScreenPoint(New PointF(Me.m_EcosimModelParams.EndSummaryTime * cCore.N_MONTHS, 0))
            Me.m_sXEnd = ptf.X

            Me.m_bMoveTimeStart = False
            Me.m_bMoveTimeEnd = False

            Me.m_sMaxXAxis = m_core.nEcosimTimeSteps
            ' The maximum Y Axis value. 
            Me.m_sMaxYAxis = 3.0
            Me.m_sFixedYScale = m_sMaxYAxis
            Me.m_bShowSummaryLines = True

            Me.ttLineGraph.AutoPopDelay = 1000
            Me.ttLineGraph.InitialDelay = 500
            Me.ttLineGraph.ReshowDelay = 5000

            Me.m_sg = StyleGuide.GetInstance()
            AddHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged

        End Sub

        Private Sub OnHandleDisposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
            RemoveHandler Me.m_sg.StyleGuideChanged, AddressOf OnStyleGuideChanged
        End Sub

        Private Sub OnHandlePaint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

            Dim g As Graphics = e.Graphics

            If (Me.m_bmpBackBuffer Is Nothing) Then
                Me.GenerateOutputImage()
            End If

            g.DrawImage(m_bmpBackBuffer, 0, 0, Me.Width, Me.Height)
            If m_bShowSummaryLines Then
                DrawSummaryLines(g, Color.Red, m_sXStart, m_sXEnd)
            End If

        End Sub

        Public Sub GenerateOutputImage(Optional ByVal bDrawIncrem As Boolean = False)

            If (Me.m_bmpBackBuffer Is Nothing) Then
                Me.m_bmpBackBuffer = New Bitmap(Me.Width, Me.Height, Me.CreateGraphics())
            End If

            If m_sMaxXAxis <= 0 Or m_sMaxYAxis <= 0 Then Return
            Me.m_sXStart = toScreenPoint(New PointF(m_EcosimModelParams.StartSummaryTime * cCore.N_MONTHS, 0)).X
            Me.m_sXEnd = toScreenPoint(New PointF(m_EcosimModelParams.EndSummaryTime * cCore.N_MONTHS, 0)).X

            ScaleYAxis()

            m_asTSData = GetTSData()

            Dim dArea As Graphics = Graphics.FromImage(m_bmpBackBuffer)

            'Only draw incrementally if the scale is fixed
            'if the scale is not fixed then all the data needs to be redrawn at the new scale 
            'or the data will be draw at different scales on the plot
            If bDrawIncrem And m_bFixedScale Then
                DrawOutputIncrem(dArea)
            Else
                DrawOutput(dArea)
            End If
            DrawLabels(dArea, m_bmpBackBuffer)

            Me.Invalidate()

        End Sub

        Private Sub DrawOutput(ByRef g As Graphics)
            ' ToDo: use colour from styleguide
            g.Clear(Color.White)
            If m_bGrayOut Then
                DrawData(g, True)
            End If
            DrawData(g, False)

        End Sub

        Private Sub DrawData(ByRef g As Graphics, ByVal isGrayOut As Boolean)

            For i As Integer = 0 To m_aLayers.Count - 1
                If m_aLayers(i).IsShown Then
                    DrawOneLayer(g, i, isGrayOut, m_bIsShowAnnualOutput)
                End If
            Next

        End Sub

        Private Sub DrawOutputIncrem(ByRef g As Graphics)

            If m_bGrayOut Then
                DrawDataIncrem(g, True)
            End If
            DrawDataIncrem(g, False)

        End Sub

        Public Sub Clear()

            If m_bmpBackBuffer IsNot Nothing Then
                ' ToDo: use colour from styleguide
                Dim g As Graphics = Graphics.FromImage(m_bmpBackBuffer)
                g.Clear(Color.White)
                DrawLabels(g, m_bmpBackBuffer)
                Me.Invalidate()
            End If

        End Sub

        Private Sub DrawDataIncrem(ByRef g As Graphics, ByVal isGrayOut As Boolean)
            If m_aLayers.Count > 0 Then
                DrawOneLayer(g, m_aLayers.Count - 1, isGrayOut, m_bIsShowAnnualOutput)
            End If
        End Sub

        Private Sub DrawOneLayer(ByRef g As Graphics, ByVal i As Integer, ByVal isGrayOut As Boolean, ByVal isShowYearly As Boolean)
            Try

                Dim mnthOffset As Integer
                For j As Integer = 0 To m_aLayers(i).Lines.Count - 1
                    Dim line As cGroupLine = m_aLayers(i).Lines(j)
                    If line.IsShown And (line.IsGrayOut = isGrayOut) Then
                        Dim tmpPen As Pen
                        If isGrayOut Then
                            tmpPen = New Pen(Color.FromArgb(255, 230, 230, 230), line.Width)
                        Else
                            tmpPen = New Pen(Me.m_sg.GroupColor(Me.m_core, line.Group.Index), line.Width)
                        End If

                        Dim n As Integer
                        If isShowYearly Then
                            n = Math.Min(line.NPoints, m_EcosimModelParams.NumberYears - 1)
                            mnthOffset = 12
                        Else
                            n = Math.Min(line.NPoints - 1, CInt(m_sMaxXAxis - 1))
                            mnthOffset = 1
                        End If

                        For k As Integer = 0 To n
                            Dim p1 As PointF = toScreenPoint(line.Point(k * mnthOffset))
                            Dim p2 As PointF = toScreenPoint(line.Point((k + 1) * mnthOffset))
                            g.DrawLine(tmpPen, p1, p2)
                        Next


                        'If Not isShowYearly Then
                        '    For k As Integer = 0 To CInt(m_sMaxXAxis - 1)

                        '        Dim p1 As PointF = toScreenPoint(line.Point(k))
                        '        Dim p2 As PointF = toScreenPoint(line.Point(k + 1))
                        '        g.DrawLine(tmpPen, p1, p2)
                        '    Next
                        'Else
                        '    For k As Integer = 0 To m_EcosimModelParams.NumberYears - 1
                        '        Dim p1 As PointF = toScreenPoint(line.Point(k * 12))
                        '        Dim p2 As PointF = toScreenPoint(line.Point((k + 1) * 12))
                        '        g.DrawLine(tmpPen, p1, p2)
                        '    Next
                        'End If

                        If m_bShowTSData AndAlso Not m_asTSData Is Nothing Then
                            If m_abHasTSData(j) Then
                                For k As Integer = 1 To m_asTSData.GetLength(1) - 1
                                    If m_asTSData(j, k) > 0 And Not Single.IsNaN(m_asTSData(j, k)) Then
                                        'jb place the X point in the middle of the year consistent with EwE5
                                        Dim p As PointF = toScreenPoint(New PointF(k * cCore.N_MONTHS - 6, m_asTSData(j, k)))
                                        ' If k < 5 Then Console.WriteLine(i.ToString & " " & m_asTSData(j, k) & " " & p.ToString)
                                        g.DrawEllipse(tmpPen, New RectangleF(p.X, p.Y, line.Width + 4, line.Width + 4))
                                    End If
                                Next
                            End If
                        End If
                        tmpPen.Dispose()
                    End If
                Next
            Catch ex As Exception

            End Try

        End Sub

        Private Sub DrawSummaryLines(ByRef g As Graphics, ByVal c As Color, ByVal x1 As Single, ByVal x2 As Single)

            Dim linePen As New Pen(c, 2)
            linePen.DashStyle = Drawing2D.DashStyle.Dot
            g.DrawLine(linePen, New PointF(x1, 0), New PointF(x1, Me.Height))
            g.DrawLine(linePen, New PointF(x2, 0), New PointF(x2, Me.Height))

        End Sub

        ''' <summary>
        ''' Pulbic interface to draw the summary lines
        ''' </summary>
        ''' <param name="xStartPosInYears"></param>
        ''' <param name="xEndPosInYears"></param>
        ''' <remarks>This allows the interface to set the summary lines in response to an edit from some other part of the interface or core</remarks>
        Friend Sub DrawSummaryLines(ByVal xStartPosInYears As Single, ByVal xEndPosInYears As Single)

            'only redraw the lines if its not me moving the points
            If Not m_bMoveTimeStart And Not m_bMoveTimeEnd Then
                'convert from years to screen points
                Me.m_sXStart = toScreenPoint(New PointF(xStartPosInYears * cCore.N_MONTHS, 0)).X
                Me.m_sXEnd = toScreenPoint(New PointF(xEndPosInYears * cCore.N_MONTHS, 0)).X
                Me.Invalidate()
            End If

        End Sub

        Private Sub DrawLabels(ByRef g As Graphics, ByRef m As Bitmap)

            'Draw Axis
            g.DrawLine(Pens.Gray, New PointF(0, m.Height), New PointF(m.Width, m.Height))
            g.DrawLine(Pens.Gray, New PointF(0, 0), New PointF(0, m.Width))
            ' Draw Axis X marks
            Dim years As String() = GetAxisX(m_sMaxXAxis / cCore.N_MONTHS)
            Dim sp As Single = CSng(m.Width * cCore.N_MONTHS / m_sMaxXAxis)

            Dim strFormat As New StringFormat
            strFormat.Alignment = StringAlignment.Center
            strFormat.LineAlignment = StringAlignment.Center

            Dim btnSpace As Single = Me.Font.Height
            Dim tmpBrush As New SolidBrush(Color.Gray)
            Dim tmpPen As New Pen(Color.Gray)

            For i As Integer = 1 To years.Length - 1
                ' JS06Feb08: OMG, this is too hack to be true
                g.DrawString(years(i), Me.Font, tmpBrush, _
                        (CSng(years(i)) - Me.m_core.EcosimFirstYear) * sp, m.Height - btnSpace, strFormat)
                g.DrawLine(tmpPen, CInt(CSng(years(i)) * sp), _
                        m.Height, CInt(CSng(years(i)) * sp), CInt(m.Height - btnSpace / 2))
            Next

            'Draw Axis Y marks
            Dim yStep As Integer = CInt(m_sMaxYAxis / 3)
            If yStep = 0 Then yStep = 1

            For j As Double = 0 To m_sMaxYAxis Step yStep * 0.5
                Dim yPos As Integer = CInt(m.Height - m.Height * j / m_sMaxYAxis)
                g.DrawString(j.ToString, Me.Font, tmpBrush, 5, yPos)
                g.DrawLine(tmpPen, 0, yPos, 3, yPos)
            Next

            If m_sMaxYAxis < 0.5 And m_sMaxYAxis >= 0.01 Then
                For j As Integer = 0 To 2
                    Dim yPos As Integer = CInt(m.Height - m.Height * (3 - j) / 3)
                    g.DrawString(String.Format("{0:f3}", m_sMaxYAxis * (3 - j) / 3), Me.Font, tmpBrush, 5, yPos)
                    g.DrawLine(tmpPen, 0, yPos, 3, yPos)
                Next
            End If

            tmpPen.Dispose()
            tmpBrush.Dispose()

        End Sub

        Private Function GetTSData() As Single(,)

            ReDim m_abHasTSData(m_core.nGroups)
            If Not m_bShowTSData Then Return Nothing

            Dim ret(m_core.nGroups, m_EcosimModelParams.NumberYears) As Single

            Dim ts As cTimeSeries = Nothing

            For i As Integer = 1 To m_core.nTimeSeries
                ts = m_core.EcosimTimeSeries(i)
                If ts.TimeSeriesType = eTimeSeriesType.BiomassRel Or ts.TimeSeriesType = eTimeSeriesType.BiomassAbs Then
                    If TypeOf ts Is cGroupTimeSeries Then
                        Dim gts As cGroupTimeSeries = CType(ts, cGroupTimeSeries)
                        If gts.Enabled() Then
                            m_abHasTSData(gts.GroupIndex) = True
                            Dim da() As Single = gts.ShapeData()
                            For j As Integer = 1 To m_EcosimModelParams.NumberYears
                                If j < da.Length Then
                                    If da(j) > 0 Then
                                        ret(gts.GroupIndex, j) = (da(j) / CSng(Math.Exp(gts.DataQ))) / m_core.StartBiomass(gts.GroupIndex)
                                        'If j < 5 Then Console.WriteLine(String.Format("da(j)={0}, gts.DataQ={1}, SB={2}, gIndex={3}", da(j), gts.DataQ, m_Core.StartBiomass(gts.GroupIndex), gts.GroupIndex))
                                    End If
                                Else
                                    'Assign a NULL value
                                    ret(gts.GroupIndex, j) = Single.NaN
                                End If
                            Next
                        End If

                    Else
                        Debug.Assert(True, "Relative Biomass TS should be cGroupTimeSeries object, check for import")
                    End If
                End If

            Next

            Return ret

        End Function

        Private Sub OnHandleResize(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Resize

            ' Invalidate back buffer
            If Me.Width > 0 And Me.Height > 0 Then
                Me.m_bmpBackBuffer = Nothing
            End If

        End Sub

        Private Function toScreenPoint(ByVal p As PointF) As PointF
            ' Transforms the output value to the screen point value
            Dim screenPt As New PointF(p.X * m_bmpBackBuffer.Width / m_sMaxXAxis, _
                            m_bmpBackBuffer.Height - p.Y * m_bmpBackBuffer.Height / m_sMaxYAxis)

            Return screenPt

        End Function

        Private Function toModelPoint(ByVal sp As PointF) As PointF

            Dim mdlPt As New PointF(sp.X * m_sMaxXAxis / m_bmpBackBuffer.Width, _
                    (m_bmpBackBuffer.Height - sp.Y) * m_sMaxYAxis / m_bmpBackBuffer.Height)

            Return mdlPt

        End Function

        Private Function CalculateMaxYValue() As Single

            Dim ret As Single = -1

            If Not m_aLayers Is Nothing Then

                For i As Integer = 0 To m_aLayers.Count - 1
                    If m_aLayers(i).IsShown Then

                        Dim maxYValue As Single = m_aLayers(i).MaxY
                        If ret < maxYValue Then
                            ret = maxYValue
                        End If
                    End If
                Next

            End If

            Return ret

        End Function

        ''' <summary>
        ''' This method returns the marks displayed on the Axis X
        ''' </summary>
        Private Function GetAxisX(ByVal maxX As Single) As String()

            Dim ret As New List(Of String)
            Dim s As Integer

            s = (CInt(maxX) + 9) \ 10
            For i As Integer = 0 To CInt(maxX) Step s
                ret.Add(CStr(i + Me.m_core.EcosimFirstYear))
            Next

            Return ret.ToArray

        End Function

#Region " Events "

        Private Sub OnHandleMouseDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown

            If Not m_bShowSummaryLines Then Return

            If (e.Button = Windows.Forms.MouseButtons.Left) Then

                Dim xPos As Integer = e.X

                If m_bShowSummaryLines Then

                    If xPos >= Math.Floor(m_sXStart) - cMOUSE_TOLERANCE And xPos <= Math.Ceiling(m_sXStart) + cMOUSE_TOLERANCE Then
                        m_bMoveTimeStart = True
                        Cursor = Cursors.SizeWE
                    End If

                    If xPos >= Math.Floor(m_sXEnd) - cMOUSE_TOLERANCE And xPos <= Math.Ceiling(m_sXEnd) + cMOUSE_TOLERANCE Then
                        m_bMoveTimeEnd = True
                        Cursor = Cursors.SizeWE
                    End If
                End If

            End If

        End Sub

        Private Sub OnHandleMouseMove(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove

            Dim xPos As Single = e.X

            If e.X <= 0 Or e.X >= Me.Width Then lblGrpName.Visible = False : Return
            If e.Y <= 0 Or e.Y >= Me.Height Then lblGrpName.Visible = False : Return

            If Me.m_bmpBackBuffer Is Nothing Then Return

            Dim clr As Color = m_bmpBackBuffer.GetPixel(e.X, e.Y)
            Dim iGroup As Integer = FindGroupIndexFromColor(clr)

            If iGroup = -1 Then lblGrpName.Visible = False : Return
            lblGrpName.Visible = True
            lblGrpName.ForeColor = clr
            'jb cLayer.Lines is zero based iGroup returned by FindGroupIndexFromColor is one based
            lblGrpName.Text = m_aLayers(0).Lines(iGroup - 1).Name

            If Not m_bShowSummaryLines Then Return

            If (e.Button = Windows.Forms.MouseButtons.Left) And (m_bMoveTimeStart Or m_bMoveTimeEnd) Then

                If m_bMoveTimeStart And m_bMoveTimeEnd Then
                    If xPos <= m_sXStart Then
                        m_bMoveTimeEnd = False
                    ElseIf xPos >= m_sXEnd Then
                        m_bMoveTimeStart = False
                    End If
                End If

                If m_bMoveTimeStart Then
                    If xPos >= m_sXEnd Then
                        m_sXStart = m_sXEnd

                    ElseIf xPos <= Me.Left Then
                        m_sXStart = Me.Left
                    Else
                        m_sXStart = xPos
                    End If
                End If

                If m_bMoveTimeEnd Then
                    If xPos <= m_sXStart Then
                        m_sXEnd = m_sXStart
                    ElseIf xPos >= Me.Right Then
                        m_sXEnd = Me.Right
                    Else
                        m_sXEnd = xPos
                    End If
                End If

                Me.Invalidate()
            Else
                ' Is cursor is near a summary line?
                If (xPos >= Math.Floor(m_sXStart) - cMOUSE_TOLERANCE And xPos <= Math.Ceiling(m_sXStart) + cMOUSE_TOLERANCE) _
                    Or (xPos >= Math.Floor(m_sXEnd) - cMOUSE_TOLERANCE And xPos <= Math.Ceiling(m_sXEnd) + cMOUSE_TOLERANCE) Then
                    ' #Yes: show resize cursor
                    Cursor = Cursors.SizeWE
                Else
                    ' #No: show default cursor
                    Cursor = Cursors.Default
                End If

            End If


        End Sub

        Private Sub OnHandleMouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp

            If Not m_bShowSummaryLines Then Return

            'jb reset the m_bMoveTimeStart or m_bMoveTimeEnd flag after setting the new summary period
            'so that the container does not redraw the line n response to a message from the core
            If m_bMoveTimeStart Then
                Dim sMdlPt As PointF = toModelPoint(New PointF(m_sXStart, 0))
                m_EcosimModelParams.StartSummaryTime = sMdlPt.X / cCore.N_MONTHS
                m_bMoveTimeStart = False
            ElseIf m_bMoveTimeEnd Then
                Dim eMdlPt As PointF = toModelPoint(New PointF(m_sXEnd, 0))
                m_EcosimModelParams.EndSummaryTime = eMdlPt.X / cCore.N_MONTHS
                m_bMoveTimeEnd = False
            End If

            Cursor = Cursors.Default

        End Sub

        Private Function FindGroupIndexFromColor(ByVal clr As Color) As Integer

            Dim layer As cLayer = Nothing
            Dim line As cGroupLine = Nothing
            Dim group As cEcoPathGroupInput = Nothing

            ' Scan all layers
            For iLayer As Integer = 0 To m_aLayers.Count - 1
                layer = m_aLayers(iLayer)
                ' Is line visible?
                If layer.IsShown Then
                    ' #Yes: scan all lines in this layer
                    For iLine As Integer = 0 To layer.Lines.Count - 1
                        line = layer.Lines(iLine)
                        group = line.Group
                        ' Do colours match?
                        If Me.m_sg.GroupColor(Me.m_core, group.Index) = clr Then
                            ' #Yes: return group index
                            Return group.Index
                        End If
                    Next
                End If
            Next
            Return -1

        End Function

        Private Sub OnStyleGuideChanged(ByVal change As StyleGuide.eChangeType)
            If ((change And StyleGuide.eChangeType.Colours) = StyleGuide.eChangeType.Colours) Then
                Me.GenerateOutputImage()
            End If
        End Sub

#End Region ' Events

    End Class

End Namespace

