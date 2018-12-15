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
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports EwECore

#End Region ' Imports

Namespace Controls.Map

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' User control for implementing a <see cref="ucMap">EwE map</see> that
    ''' can be zoomed onto.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class ucMapZoom
        Implements IUIElement

#Region " Private vars "

        ''' <summary>UI context to connect to.</summary>
        Private m_uic As cUIContext = Nothing

        Private m_bInUpdate As Boolean = False

#End Region ' Private vars

#Region " Constructor "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Public access "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Enumerated types defining zoom modes for displaying the map.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Enum eZoomTypes As Byte
            ''' <summary>Increase zoom level.</summary>
            ZoomIn
            ''' <summary>Decrease zoom level.</summary>
            ZoomOut
            ''' <summary>Resets zoom level to exactly fit the zoom area.</summary>
            ZoomReset
        End Enum

        Public Event OnPositionChanged(ByVal sender As ucMapZoom)

        ''' <summary>
        ''' Zoom and position to the location of another map
        ''' </summary>
        ''' <param name="src"></param>
        Public Sub UpdatePosition(ByVal src As ucMapZoom)
            Me.m_bInUpdate = True

            Me.ZoomScale = src.ZoomScale
            'Me.ScaleMap()

            Me.m_bInUpdate = False
        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdoc cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        <Browsable(False)>
        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                Me.m_uic = value
                Me.m_map.UIContext = value
            End Set
        End Property

        Public ReadOnly Property Map() As ucMap
            Get
                Return Me.m_map
            End Get
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the zoom percentage for displaying the map.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(False)>
        Public Property ZoomScale(Optional bZoomToCursor As Boolean = False) As Single
            Get
                Return Me.m_map.Zoom
            End Get
            Set(ByVal value As Single)
                Me.m_map.Zoom = value
            End Set
        End Property

        Public ReadOnly Property CanZoomIn As Boolean
            Get
                Return (Me.ZoomScale < Me.m_map.MaxZoom)
            End Get
        End Property

        Public ReadOnly Property CanZoomOut As Boolean
            Get
                Return (Me.ZoomScale > 1)
            End Get
        End Property

#End Region ' Public access

#Region " Events "

#Region " Form events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
        End Sub

        Protected Sub OnMapScrolled(sender As Object, args As EventArgs) Handles m_map.OnMapScrolled

            If (Me.m_bInUpdate) Then Return
            Me.m_bInUpdate = True
            Try
                Dim szRange As Size = Me.m_map.ScrollRange
                Dim szSize As Size = Me.m_map.ScrollSize
                Dim ptPos As Point = Me.m_map.ScrollPos

                Me.m_sbHorz.LargeChange = CInt(szRange.Width / 10)
                Me.m_sbHorz.SmallChange = CInt(szRange.Width / 20)
                ' https://stackoverflow.com/questions/12369994/how-to-get-a-scroll-bar-to-reach-the-maximum-in-vb-net
                Me.m_sbHorz.Maximum = szSize.Width + Me.m_sbHorz.LargeChange + 1
                Me.m_sbHorz.Value = szSize.Width - ptPos.X

                Me.m_sbVert.LargeChange = CInt(szRange.Height / 10)
                Me.m_sbVert.SmallChange = CInt(szRange.Height / 20)
                ' https://stackoverflow.com/questions/12369994/how-to-get-a-scroll-bar-to-reach-the-maximum-in-vb-net
                Me.m_sbVert.Maximum = szSize.Height + Me.m_sbVert.LargeChange + 1
                Me.m_sbVert.Value = szSize.Height - ptPos.Y

                RaiseEvent OnPositionChanged(Me)
            Catch ex As Exception
                ' Plop
            End Try
            Me.m_bInUpdate = False

        End Sub

        Private Sub OnScrolled(sender As Object, e As ScrollEventArgs) _
        Handles m_sbHorz.Scroll, m_sbVert.Scroll

            If Me.m_bInUpdate Then Return

            Me.m_bInUpdate = True
            Try
                ' https://stackoverflow.com/questions/12369994/how-to-get-a-scroll-bar-to-reach-the-maximum-in-vb-net
                Me.m_map.ScrollPos = New Point(Me.m_sbHorz.Maximum - Me.m_sbHorz.Value - Me.m_sbHorz.LargeChange - 1, Me.m_sbVert.Maximum - Me.m_sbVert.Value - Me.m_sbVert.LargeChange - 1)
            Catch ex As Exception

            End Try
            Me.m_bInUpdate = False

        End Sub

#End Region ' Form events

#End Region ' Events

    End Class

End Namespace
