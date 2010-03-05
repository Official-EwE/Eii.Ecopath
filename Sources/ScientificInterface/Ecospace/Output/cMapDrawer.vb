#Region " Imports "

Option Strict On
Option Explicit On

Imports System
Imports System.Threading
Imports EwECore
Imports System.Drawing
Imports ScientificInterface.Other

#End Region ' Imports

''' <summary>
''' Helper class for rendering data for a series of groups onto a
''' graphics area.
''' </summary>
''' <remarks>
''' Need to write explanation on parms, usage
''' </remarks>
Public Class cMapDrawer

#Region " Private vars "

    Public SignalState As New ManualResetEvent(True)

    Private m_map(,,) As Single
    Private m_mapIBMPackets(,,) As Boolean
    Private m_core As cCore = Nothing

    Private m_lGroups As New List(Of Integer)
    Private m_lLocations As New List(Of Integer)
    Private m_iInCol As Integer
    Private m_iInRow As Integer
    Private m_iMonth As Integer
    Private m_lColors As List(Of Color)
    Private m_bShowMPA As Boolean = False

    Private m_graphics As Graphics
    Private m_font As System.Drawing.Font

    Private m_iFirst As Integer
    Private m_iLast As Integer

    Private m_threadID As Integer
    Public m_bAllowedToRun As Boolean

    Private m_lptOrigin As List(Of PointF)
    Private m_lrc As List(Of Rectangle)

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(ByVal iThreadID As Integer, ByVal core As cCore)
        Me.m_threadID = iThreadID
        Me.m_core = core
        Me.m_bAllowedToRun = True
    End Sub

#End Region ' Constructor

#Region " Public properties "

    Public Property AllowedToRun() As Boolean
        Get
            Return Me.m_bAllowedToRun
        End Get
        Set(ByVal value As Boolean)
            Me.m_bAllowedToRun = value
        End Set
    End Property

    Public Property ShowMPA() As Boolean
        Get
            Return Me.m_bShowMPA
        End Get
        Set(ByVal value As Boolean)
            Me.m_bShowMPA = value
        End Set
    End Property

    Public Property Map() As Single(,,)
        Get
            Return Me.m_map
        End Get
        Set(ByVal value As Single(,,))
            Me.m_map = value
        End Set
    End Property

    Public Property MapIBMPackets() As Boolean(,,)
        Get
            Return Me.m_mapIBMPackets
        End Get
        Set(ByVal value As Boolean(,,))
            Me.m_mapIBMPackets = value
        End Set
    End Property

    Public Property InRow() As Integer
        Get
            Return Me.m_iInRow
        End Get
        Set(ByVal value As Integer)
            Me.m_iInRow = value
        End Set
    End Property

    Public Property InCol() As Integer
        Get
            Return Me.m_iInCol
        End Get
        Set(ByVal value As Integer)
            Me.m_iInCol = value
        End Set
    End Property

    Public Property Month() As Integer
        Get
            Return Me.m_iMonth
        End Get
        Set(ByVal value As Integer)
            Me.m_iMonth = value
        End Set
    End Property

    Public Property GroupColors() As List(Of Color)
        Get
            Return Me.m_lColors
        End Get
        Set(ByVal lColors As List(Of Color))
            Me.m_lColors = lColors
        End Set
    End Property

    Public Property OriginList() As List(Of PointF)
        Get
            Return Me.m_lptOrigin
        End Get
        Set(ByVal value As List(Of PointF))
            Me.m_lptOrigin = value
        End Set
    End Property

    Public Property RectList() As List(Of Rectangle)
        Get
            Return Me.m_lrc
        End Get
        Set(ByVal value As List(Of Rectangle))
            Me.m_lrc = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the font to render labels with.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Font() As Font
        Get
            Return Me.m_font
        End Get
        Set(ByVal value As Font)
            Me.m_font = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the graphics to render onto.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Graphics() As Graphics
        Get
            Return Me.m_graphics
        End Get
        Set(ByVal value As Graphics)
            Me.m_graphics = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Clear all groups associated with this map drawer.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub ClearGroups()
        Me.m_lGroups.Clear()
        Me.m_lLocations.Clear()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a group to draw to this rendering drawer.
    ''' </summary>
    ''' <param name="iGroup">The group to add.</param>
    ''' <param name="iLocation">The location to show this group at.</param>
    ''' -----------------------------------------------------------------------
    Public Sub AddGroup(ByVal iGroup As Integer, ByVal iLocation As Integer)
        If Not Me.m_lGroups.Contains(iGroup) Then
            Me.m_lGroups.Add(iGroup)
            Me.m_lLocations.Add(iLocation)
        End If
    End Sub

#End Region ' Public properties

#Region " Public access "

    Public Sub Draw(ByVal obParam As Object)
        m_bAllowedToRun = False
        Try
            Dim i As Integer
            Dim iGroup As Integer
            Dim iLocation As Integer
            'SignalState.Reset()
            For i = 0 To Me.m_lGroups.Count - 1
                iGroup = Me.m_lGroups(i)
                iLocation = Me.m_lLocations(i)
                DrawBiomassBaseMap(iGroup, m_lrc(iLocation))
            Next

            m_bAllowedToRun = True
            SignalState.Set()

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            SignalState.Set()
        End Try
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="iGroup"></param>
    ''' <param name="rcPos"></param>
    ''' <remarks></remarks>
    Public Sub DrawBiomassBaseMap(ByVal iGroup As Integer, ByVal rcPos As Rectangle)
        If m_map Is Nothing Then Return
        For i As Integer = 1 To m_iInRow
            For j As Integer = 1 To m_iInCol
                Try
                    Dim sMapValue As Single = 1.0E-20
                    Dim icc As Single
                    Dim rcfCell As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / m_iInCol), _
                                                               CSng(rcPos.Top + (i - 1) * rcPos.Height() / m_iInRow), _
                                                               CSng(rcPos.Width() / m_iInCol), _
                                                               CSng(rcPos.Height() / m_iInRow))
                    Dim rcTemp As Rectangle = Nothing
                    Dim brCell As Brush = Nothing
                    Dim brBlack As New SolidBrush(Color.Black)
                    'If ConShow And ConcMax(ip) > 0 Then
                    '    If ConShowType = 0 Then
                    '        MapValue = 11 * p_baseMap(i, j, ip) / ConcMax(ip)
                    '    Else
                    '        If Bcell(i, j, ip) > 0 And (PrefHab(ip, HabType(i, j)) Or PrefHab(ip, 0)) Then MapValue = 11 * (p_baseMap(i, j, ip) / Bcell(i, j, ip)) / ConcMax(ip) Else MapValue = 1.0E-20
                    '    End If
                    'Else
                    '    MapValue = p_baseMap(i, j, ip) / StartBiomass(ip)
                    'End If

                    sMapValue = m_map(i, j, iGroup) / m_core.StartBiomass(iGroup)
                    If (sMapValue > 10.0!) Or Single.IsPositiveInfinity(sMapValue) Then
                        icc = m_lColors.Count
                    ElseIf (sMapValue < 0.1!) Or Single.IsNegativeInfinity(sMapValue) Then
                        icc = 1
                    Else
                        'icc = m_ColorNum * 1 / (MapValue + 1)
                        icc = m_lColors.Count * sMapValue / (sMapValue + 1)
                    End If

                    'Boundary check
                    icc = Math.Max(Math.Min(m_lColors.Count - 1, icc), 1)

                    'If it is water
                    If CInt(m_core.EcospaceBasemap.LayerDepth.Cell(i, j)) > 0 Then
                        ' #Water
                        brCell = New SolidBrush(m_lColors(CInt(icc)))
                    Else
                        ' #Land
                        brCell = New SolidBrush(Color.Gray)
                    End If
                    m_graphics.FillRectangle(brCell, rcfCell)
                    brCell.Dispose()

                    rcTemp = New Rectangle(CInt(rcfCell.X), CInt(rcfCell.Y), CInt(rcfCell.Width), CInt(rcfCell.Height))

                    ' Draw MPA
                    If Me.m_bShowMPA Then
                        Dim iMPA As Integer = CInt(m_core.EcospaceBasemap.LayerMPA.Cell(i, j))
                        ' Is MPA cell?
                        If iMPA > 0 Then
                            If Me.m_core.EcospaceMPAs(iMPA).MPAMonth(Me.Month) Then
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.LightGray, Color.Transparent)
                            Else
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.Black, Color.Transparent)
                            End If
                            m_graphics.FillRectangle(brCell, rcfCell)
                            brCell.Dispose()
                        End If
                    End If

                    If Me.m_mapIBMPackets IsNot Nothing Then
                        If Me.MapIBMPackets(i, j, iGroup) Then
                            m_graphics.FillEllipse(brBlack, Rectangle.Inflate(rcTemp, -CInt(rcTemp.Width / 2.5), -CInt(rcTemp.Height / 2.5)))
                            ' m_graphics.DrawEllipse(Pens.Black, _
                            '                       Rectangle.Inflate(rcTemp, -CInt(rcTemp.Width / 2.5), -CInt(rcTemp.Height / 2.5)))
                        End If
                    End If

                Catch ex As Exception
                    'Debug.Assert(False, ex.Message)
                    Exit Sub
                End Try
            Next
        Next

        'Draw the black frame of base map
        m_graphics.DrawRectangle(Pens.Black, rcPos)

        'Display the group name
        Dim grpName As String = m_core.EcospaceGroups(iGroup).Name
        m_graphics.DrawString(grpName, m_font, Brushes.Black, rcPos)
    End Sub

#End Region ' Public access

End Class
