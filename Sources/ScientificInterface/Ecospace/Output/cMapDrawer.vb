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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

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

    Public Enum eMapType As Integer
        RelBiomass = 0
        RelCatch
        FishingMortRate
        RelContam
        ContamRate
    End Enum

#Region " Private vars "

    Private Const MAX_FISH_MORT As Single = 2

    Public SignalState As New ManualResetEvent(True)

    Private m_core As cCore = Nothing

    Private m_lGroups As New List(Of Integer)
    Private m_lLocations As New List(Of Integer)
    Private m_labelposHorz As StringAlignment = StringAlignment.Near
    Private m_labelposVert As StringAlignment = StringAlignment.Near

    Private m_threadID As Integer

#End Region ' Private vars

#Region " Constructor "

    Public Sub New(ByVal iThreadID As Integer, ByVal core As cCore)
        Me.m_threadID = iThreadID
        Me.m_core = core
        Me.AllowedToRun = True
        Me.ShowLand = True
    End Sub

#End Region ' Constructor

#Region " Public properties "

    Public Property AllowedToRun() As Boolean

    Public Property ShowMPA() As Boolean
    Public Property ShowLand() As Boolean
       
    Public Property Map() As Single(,,)

    Public Property StanzaDS() As cStanzaDatastructures

    Public Property InRow() As Integer
    
    Public Property InCol() As Integer

    Public Property Month() As Integer

    Public Property Colors() As List(Of Color)

    Public Property OriginList() As List(Of PointF)

    Public Property RectList() As List(Of Rectangle)

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the font to render labels with.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Font() As Font

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the graphics to render onto.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property Graphics() As Graphics

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

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether labels should be shown.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property ShowLabels() As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether labels should be rendered with inverse colors.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property InvertLabelColors() As Boolean

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Set the position of labels
    ''' </summary>
    ''' <param name="horz">Horizontal label alignment.</param>
    ''' <param name="vert">Vertical label alignment.</param>
    ''' -----------------------------------------------------------------------
    Public Sub SetLabelPosition(ByVal horz As StringAlignment, ByVal vert As StringAlignment)
        Me.m_labelposHorz = horz
        Me.m_labelposVert = vert
    End Sub

#End Region ' Public properties

#Region " Public access "

    Public Sub Draw(ByVal obParam As Object)
        Me.AllowedToRun = False
        Dim args As cMapDrawerArgs = DirectCast(obParam, cMapDrawerArgs)
        Try
            Dim i As Integer
            Dim iGroup As Integer
            Dim iLocation As Integer
            For i = 0 To Me.m_lGroups.Count - 1
                iGroup = Me.m_lGroups(i)
                iLocation = Me.m_lLocations(i)
                Try
                    DrawMap(iGroup, Me.RectList(iLocation), args)
                Catch ex As Exception

                End Try
            Next

            Me.AllowedToRun = True
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
    Public Sub DrawMap(ByVal iGroup As Integer, ByVal rcPos As Rectangle, ByVal Args As cMapDrawerArgs)
        If Me.Map Is Nothing Then Return
        Dim FScaler As Single
        Dim maptype As cMapDrawer.eMapType = Args.MapType
        Dim RelScaler() As Single = Args.RelMapScaler

        If MapType = eMapType.FishingMortRate Then
            FScaler = Me.Colors.Count / Args.FishingMortLegendMax
        End If

        For i As Integer = 1 To Me.InRow
            For j As Integer = 1 To Me.InCol
                Try
                    Dim sMapValue As Single = 1.0E-20
                    Dim icc As Single
                    Dim rcfCell As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / Me.InCol), _
                                                               CSng(rcPos.Top + (i - 1) * rcPos.Height() / Me.InRow), _
                                                               CSng(rcPos.Width() / Me.InCol), _
                                                               CSng(rcPos.Height() / Me.InRow))
                    Dim rcTemp As Rectangle = Nothing
                    Dim brCell As Brush = Nothing

                    'If it is water
                    If CInt(m_core.EcospaceBasemap.LayerDepth.Cell(i, j)) > 0 Then
                        ' Water Cell
                        sMapValue = Me.Map(i, j, iGroup) / RelScaler(iGroup)

                        ' Old EwE5:    icc = m_ColorNum * 1 / (MapValue + 1)
                        ' Latest EwE5: icc = MaxColorsInGrad * MapValue / (MaxColorsInGrad / ColorScaling - 1 + MapValue)
                        '              ColorScaling is MaxColorsInGrad / 2

                        Select Case maptype
                            Case eMapType.FishingMortRate
                                'Only Fishing mort map has it's own color binning 
                                icc = sMapValue * FScaler
                            Case Else
                                If (sMapValue > 10.0!) Or Single.IsPositiveInfinity(sMapValue) Then
                                    icc = Me.Colors.Count
                                ElseIf (sMapValue < 0.1!) Or Single.IsNegativeInfinity(sMapValue) Or Single.IsNaN(sMapValue) Then
                                    icc = 1
                                Else
                                    icc = Me.Colors.Count * sMapValue / (sMapValue + 1)
                                End If
                        End Select

                        'Boundary check
                        icc = Math.Max(Math.Min(Me.Colors.Count - 1, icc), 1)
                        brCell = New SolidBrush(Me.Colors(CInt(icc)))

                    ElseIf Me.ShowLand Then
                        ' #Land
                        brCell = New SolidBrush(Color.Gray)
                    Else
                        brCell = New SolidBrush(Color.Transparent)
                    End If

                    Me.Graphics.FillRectangle(brCell, rcfCell)
                    brCell.Dispose()

                    rcTemp = New Rectangle(CInt(rcfCell.X), CInt(rcfCell.Y), CInt(rcfCell.Width), CInt(rcfCell.Height))

                    ' Draw MPA
                    If Me.ShowMPA Then
                        Dim iMPA As Integer = CInt(m_core.EcospaceBasemap.LayerMPA.Cell(i, j))
                        ' Is MPA cell?
                        If iMPA > 0 Then
                            If Me.m_core.EcospaceMPAs(iMPA).MPAMonth(Me.Month) Then
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.LightGray, Color.Transparent)
                            Else
                                brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.Black, Color.Transparent)
                            End If
                            Me.Graphics.FillRectangle(brCell, rcfCell)
                            brCell.Dispose()
                        End If
                    End If

                Catch ex As Exception
                    'Debug.Assert(False, ex.Message)
                    Exit Sub
                End Try

            Next
        Next

        If (Me.StanzaDS IsNot Nothing) Then

            Dim isp As Integer = -1

            For ispTmp As Integer = 1 To StanzaDS.Nsplit
                For ist As Integer = 1 To StanzaDS.Nstanza(ispTmp)
                    If iGroup = StanzaDS.EcopathCode(ispTmp, ist) Then
                        If (isp = -1) Then isp = ispTmp
                    End If
                Next ist
            Next ispTmp

            Try
                If isp > -1 Then

                    For iaa As Integer = 0 To StanzaDS.MaxAgeSpecies(isp)
                        Dim ia As Integer = StanzaDS.AgeIndex1(isp) + iaa : If ia > StanzaDS.MaxAgeSpecies(isp) Then ia = ia - StanzaDS.MaxAgeSpecies(isp) - 1
                        Dim ist As Integer = StanzaDS.StanzaNo(isp, ia)
                        Dim ieco As Integer = StanzaDS.EcopathCode(isp, ist)

                        If ieco = iGroup Then
                            For ipkt As Integer = 1 To StanzaDS.Npackets

                                Dim sy As Single = StanzaDS.iPacket(isp, iaa, ipkt)
                                Dim sx As Single = StanzaDS.jPacket(isp, iaa, ipkt)
                                Dim ptfCell As New PointF(CSng(rcPos.Left + (sx - 1) * rcPos.Width() / Me.InCol), _
                                                          CSng(rcPos.Top + (sy - 1) * rcPos.Height() / Me.InRow))
                                Dim rcF As New RectangleF(ptfCell.X, ptfCell.Y, 1, 1)

                                Me.Graphics.DrawEllipse(Pens.Black, rcF)

                            Next ipkt

                        End If
                    Next iaa

                End If

            Catch ex As Exception

            End Try

        End If

        'Draw the black frame of base map
        Me.Graphics.DrawRectangle(Pens.Black, rcPos)

        If Me.ShowLabels Then
            'Display the group name
            Dim grpName As String = m_core.EcospaceGroups(iGroup).Name
            Dim br As Brush = Brushes.Black
            Dim fmt As New StringFormat()

            fmt.Alignment = Me.m_labelposHorz
            fmt.LineAlignment = Me.m_labelposVert

            If Me.InvertLabelColors Then br = Brushes.White

            Me.Graphics.DrawString(grpName, Me.Font, br, rcPos, fmt)
        End If

    End Sub

#End Region ' Public access

End Class

Public Class cMapDrawerArgs
    Public MapType As cMapDrawer.eMapType
    Public RelMapScaler() As Single
    Public FishingMortLegendMax As Single

    Public Sub New(ByVal theMapType As cMapDrawer.eMapType, ByVal theRelScaler() As Single, ByVal MaxLegendF As Single)
        Me.MapType = theMapType
        Me.RelMapScaler = theRelScaler
        Me.FishingMortLegendMax = MaxLegendF
    End Sub
End Class
