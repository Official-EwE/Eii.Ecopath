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

Imports System.Threading
Imports EwECore

#End Region ' Imports

Namespace Ecospace

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Blunt class for rendering map data onto a graphics area.
    ''' </summary>
    ''' <remarks>
    ''' This class needs some thorough revisioning!
    ''' - Cannot draw fleet information yet. Suggest to split cMapDrawer in cGroupMapDrawer, cFleetMapDrawer; make cMapDrawer abstract class
    ''' - Drawer class should be cleaned up
    ''' </remarks>
    ''' ---------------------------------------------------------------------------
    Public MustInherit Class cMapDrawerBase

        Public Enum eMapType As Integer
            RelBiomass = 0
            RelCatch
            FishingMortRate
            RelContam
            ContamRate
        End Enum

#Region " Private vars "

        Protected m_SignalState As New ManualResetEvent(True)
        Protected Const MAX_FISH_MORT As Single = 2
        Protected m_core As cCore = Nothing
        Protected m_lItems As New List(Of cCoreInputOutputBase)
        Protected m_lLocations As New List(Of Integer)
        Protected m_labelposHorz As StringAlignment = StringAlignment.Near
        Protected m_labelposVert As StringAlignment = StringAlignment.Near

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore)
            Me.m_core = core
            Me.AllowedToRun = True
            Me.ShowLand = True
            Me.ShowBorder = True
        End Sub

#End Region ' Constructor

#Region " Public properties "

        Public Property AllowedToRun() As Boolean
        Public Property ShowMPA() As Boolean
        Public Property ShowLand() As Boolean
        Public Property ShowBorder() As Boolean
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
        ''' Clear all items associated with this map drawer.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub ClearItems()
            Me.m_lItems.Clear()
            Me.m_lLocations.Clear()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add a <see cref="cCoreInputOutputBase">item</see> to draw.
        ''' </summary>
        ''' <param name="item">The <see cref="cCoreInputOutputBase"/> to add.</param>
        ''' <param name="iLocation">The location to show this item at.</param>
        ''' -----------------------------------------------------------------------
        Public Sub AddItem(ByVal item As cCoreInputOutputBase, ByVal iLocation As Integer)
            If Not Me.m_lItems.Contains(item) Then
                Me.m_lItems.Add(item)
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

        Public ReadOnly Property SignalState As ManualResetEvent
            Get
                Return Me.m_SignalState
            End Get
        End Property

#End Region ' Public properties

#Region " Public access "

        Public Sub Draw(ByVal obParam As Object)

            Me.AllowedToRun = False
            Dim args As cMapDrawerArgs = DirectCast(obParam, cMapDrawerArgs)
            Try
                Dim i As Integer
                Dim iGroup As Integer
                Dim iLocation As Integer
                For i = 0 To Me.m_lItems.Count - 1
                    iGroup = Me.m_lItems(i).Index
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
        ''' Draw the map. The base class implementation just renderers MPA data. 
        ''' </summary>
        ''' <param name="iItem"></param>
        ''' <param name="rcPos"></param>
        ''' <param name="Args"></param>
        Public Overridable Sub DrawMap(ByVal iItem As Integer, ByVal rcPos As Rectangle, ByVal Args As cMapDrawerArgs)

            If (Me.Map Is Nothing) Then Return

            Dim iMPA As Integer = 0
            Dim rcfCell As RectangleF = Nothing
            Dim brCell As Brush = Nothing
            Dim mpa As cEcospaceLayerMPA = m_core.EcospaceBasemap.LayerMPA
            Dim excl As cEcospaceLayerExclusion = Me.m_core.EcospaceBasemap.LayerExclusion

            Try
                If Me.ShowMPA Then
                    For i As Integer = 1 To Me.InRow
                        For j As Integer = 1 To Me.InCol
                            iMPA = CInt(mpa.Cell(i, j))
                            If iMPA > 0 Then
                                If CBool(excl.Cell(i, j)) = False Then
                                    rcfCell = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / Me.InCol), _
                                                                 CSng(rcPos.Top + (i - 1) * rcPos.Height() / Me.InRow), _
                                                                 CSng(rcPos.Width() / Me.InCol), _
                                                                 CSng(rcPos.Height() / Me.InRow))
                                    If Me.m_core.EcospaceMPAs(iMPA).MPAMonth(Me.Month) Then
                                        brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.LightGray, Color.Transparent)
                                    Else
                                        brCell = New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.Black, Color.Transparent)
                                    End If
                                    Me.Graphics.FillRectangle(brCell, rcfCell)
                                    brCell.Dispose()
                                End If
                            End If
                        Next
                    Next
                End If
            Catch ex As Exception
                'Debug.Assert(False, ex.Message)
                Exit Sub
            End Try

            If (Me.ShowBorder) Then
                ' Draw the black frame of base map
                Me.Graphics.DrawRectangle(Pens.Black, rcPos)
            End If

        End Sub

#End Region ' Public access

    End Class

End Namespace
