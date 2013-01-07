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
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for rendering a Scientific Interface <see cref="cLayer">layer</see>
    ''' onto the base map.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public MustInherit Class cLayerRenderer
        : Implements IDisposable

#Region " Private vars "

        ''' <summary>Default brush to render the cell with.</summary>
        Protected Shared brDEFAULT As Brush = Brushes.Transparent
        ''' <summary><see cref="cVisualStyle">Style</see> describing what colours
        ''' and font to use for rendering.</summary>
        Private m_vs As cVisualStyle = Nothing
        ''' <summary>Core style flags to use when rendering.</summary>
        Private m_styleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet
        ''' <summary>States whether the underlying layer is visible.</summary>
        Private m_bVisible As Boolean = True

        Private m_sScaleMax As Single = cCore.NULL_VALUE
        Private m_sScaleMin As Single = cCore.NULL_VALUE

#End Region ' Private vars

#Region " Construction / destruction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="vs"></param>
        ''' <param name="layerStyleFlags"></param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal vs As cVisualStyle, _
                       Optional ByVal layerStyleFlags As cVisualStyle.eVisualStyleTypes = cVisualStyle.eVisualStyleTypes.NotSet)
            Me.m_vs = vs
            Me.m_styleFlags = layerStyleFlags
            Me.Update()
        End Sub

        Protected Overridable Sub Dispose(ByVal bDisposing As Boolean)
            Me.m_vs = Nothing
        End Sub

#Region " IDisposable support "

        ' To detect redundant calls
        Private m_bDisposed As Boolean = False

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Haha I modified it
            If m_bDisposed = False Then
                Dispose(True)
                GC.SuppressFinalize(Me)
                Me.m_bDisposed = True
            End If
        End Sub

#End Region ' IDisposable support

#End Region ' Construction / destruction

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the visual style for this layer representation.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property VisualStyle() As cVisualStyle
            Get
                Return Me.m_vs
            End Get
            Set(ByVal value As cVisualStyle)
                Me.m_vs = value
                Me.Update()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the flags describing which visual style flags apply to a given 
        ''' layer representation.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property VisualStyleFlags() As cVisualStyle.eVisualStyleTypes
            Get
                Return Me.m_styleFlags
            End Get
            Set(ByVal styleFlags As cVisualStyle.eVisualStyleTypes)
                Me.m_styleFlags = styleFlags
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update any cached data for this layer representation.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub Update()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Provide brush for rendering sample panes.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub RenderPreview(ByVal g As Graphics, _
                                              ByVal rc As Rectangle)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a layer onto a graphics context
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">Device area to render cell onto.</param>
        ''' <param name="layer">The layer to render.</param>
        ''' <param name="ptfTL">Top-left coordinate represented by the device area.</param>
        ''' <param name="ptfBR">Bottom-right coordinate represented by the device area.</param>
        ''' <param name="style">Layer style to use when rendering/</param>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub Render(ByVal g As Graphics, _
                                       ByVal layer As cLayer, _
                                       ByVal rc As Rectangle, _
                                       ByVal ptfTL As PointF, _
                                       ByVal ptfBR As PointF, _
                                       ByVal style As cStyleGuide.eStyleFlags)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' States whether the current visual style is valid
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Protected Overridable Function IsStyleValid() As Boolean
            Return (Me.VisualStyle IsNot Nothing)
        End Function

        Protected Sub RenderError(ByVal g As Graphics, ByVal rc As Rectangle)
            g.FillRectangle(Brushes.White, rc)
            g.DrawLine(Pens.Red, rc.Left, rc.Top, rc.Right, rc.Bottom)
            g.DrawLine(Pens.Red, rc.Left, rc.Bottom, rc.Right, rc.Top)
        End Sub

        Public Overridable Function Clone() As cRasterLayerRenderer
            Dim minime As cRasterLayerRenderer = Nothing
            Dim vs As cVisualStyle = Me.VisualStyle.Clone()

            minime = DirectCast(Activator.CreateInstance(Me.GetType(), New Object() {vs}), cRasterLayerRenderer)
            minime.VisualStyleFlags = Me.VisualStyleFlags

            Return minime
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the layer is visible.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property IsVisible() As Boolean
            Get
                Return Me.m_bVisible
            End Get
            Set(ByVal value As Boolean)
                Me.m_bVisible = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the scale max value to render to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ScaleMax() As Single
            Get
                Return Me.m_sScaleMax
            End Get
            Set(ByVal value As Single)
                Me.m_sScaleMax = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the scale min value to render to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ScaleMin() As Single
            Get
                Return Me.m_sScaleMin
            End Get
            Set(ByVal value As Single)
                Me.m_sScaleMin = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the display text for a given cell in the underlying data.
        ''' </summary>
        ''' <returns>The display text for a given cell in the underlying data.</returns>
        ''' -----------------------------------------------------------------------
        Public MustOverride Function GetDisplayText(value As Object) As String

    End Class

End Namespace
