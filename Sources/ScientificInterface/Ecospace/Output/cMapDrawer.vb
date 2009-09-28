Option Strict On
Option Explicit On

Imports System
Imports System.Threading
Imports EwECore
Imports System.Drawing
Imports ScientificInterface.Other

Public Class cMapDrawer

    Public SignalState As New ManualResetEvent(True)

    Private m_map(,,) As Single
    Private m_core As cCore = Nothing

    Private m_iGroupFirst As Integer
    Private m_iGroupLast As Integer
    Private m_iInCol As Integer
    Private m_iInRow As Integer
    Private m_lColors As List(Of Color)
    Private m_bShowMPA As Boolean = False
    Private m_bShowPackets As Boolean = False

    Private m_graphics As Graphics
    Private m_font As System.Drawing.Font

    Private m_iFirst As Integer
    Private m_iLast As Integer

    Private m_threadID As Integer
    Public m_bAllowedToRun As Boolean

    Private m_lptOrigin As List(Of PointF)
    Private m_lrc As List(Of Rectangle)

    Public Sub New(ByVal iThreadID As Integer, ByVal core As cCore)
        Me.m_bAllowedToRun = True
        Me.m_threadID = iThreadID
        Me.m_core = core
    End Sub

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

    Public Property ShowIBMPackets() As Boolean
        Get
            Return Me.m_bShowPackets
        End Get
        Set(ByVal value As Boolean)
            Me.m_bShowPackets = value
        End Set
    End Property

    Public Property Map() As Single(,,)
        Get
            Return Me.m_map
        End Get
        Set(ByVal as3Map As Single(,,))
            Me.m_map = as3Map
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

    ' ToDo: use styleguide font here
    Public Property Font() As Font
        Get
            Return Me.m_font
        End Get
        Set(ByVal value As Font)
            Me.m_font = value
        End Set
    End Property

    Public Property Graphics() As Graphics
        Get
            Return Me.m_graphics
        End Get
        Set(ByVal value As Graphics)
            Me.m_graphics = value
        End Set
    End Property

    Public Property GroupFirst() As Integer
        Get
            Return Me.m_iGroupFirst
        End Get
        Set(ByVal value As Integer)
            Me.m_iGroupFirst = value
        End Set
    End Property

    Public Property GroupLast() As Integer
        Get
            Return Me.m_iGroupLast
        End Get
        Set(ByVal value As Integer)
            Me.m_iGroupLast = value
        End Set
    End Property

#End Region ' Public properties

    Public Sub Draw(ByVal obParam As Object)
        m_bAllowedToRun = False
        Try
            Dim i As Integer
            'SignalState.Reset()
            For i = m_iGroupFirst To m_iGroupLast
                DrawBiomassBaseMap(i, m_lrc(i - 1))
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
    Public Sub DrawBiomassBaseMap(ByVal iGroup As Integer, ByVal rcPos As Rectangle, Optional ByVal iStanza As Integer = cCore.NULL_VALUE)
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
                    Dim brCell As Brush = Nothing

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

                    ' Draw MPA
                    If Me.m_bShowMPA Then
                        Dim iMPA As Integer = CInt(m_core.EcospaceBasemap.LayerMPA.Cell(i, j))
                        ' Is MPA cell?
                        If iMPA > 0 Then
                            ' ToDo: check if MPA is closed this month
                            brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.Cross, Color.Black)
                            m_graphics.FillRectangle(brCell, rcfCell)
                            brCell.Dispose()
                        End If
                    End If

                    ' Draw IBM packet
                    If Me.m_bShowPackets Then
                        ' Is Stanza group?

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

End Class
