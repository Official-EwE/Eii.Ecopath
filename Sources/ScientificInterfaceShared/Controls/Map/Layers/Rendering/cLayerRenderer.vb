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
'    UBC Fisheries Centre, Vancouver BC, Canada, and 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports 

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Base class for rendering a <see cref="cDisplayLayer">display layer</see>
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
            Me.VisualStyleFlags = layerStyleFlags
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Update any cached data for this layer representation.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub Update()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">The area to render into.</param>
        ''' <param name="iSymbol">The <see cref="nExtraSymbols">symbol</see> to render.
        ''' If left at 0 the default cell value should be drawn.</param>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub RenderPreview(ByVal g As Graphics, _
                                              ByVal rc As Rectangle, _
                                              Optional iSymbol As Integer = 0)

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
                                       ByVal layer As cDisplayLayer, _
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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell in error.
        ''' </summary>
        ''' <param name="g">Graphics device to render onto.</param>
        ''' <param name="rc">Area to render to.</param>
        ''' -----------------------------------------------------------------------
        Protected Sub RenderError(ByVal g As Graphics, ByVal rc As Rectangle)
            'g.FillRectangle(Brushes.White, rc)
            g.DrawLine(Pens.Red, rc.Left, rc.Top, rc.Right, rc.Bottom)
            g.DrawLine(Pens.Red, rc.Left, rc.Bottom, rc.Right, rc.Top)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Create a shallow copy.
        ''' </summary>
        ''' <returns>A shallow copy.</returns>
        ''' -----------------------------------------------------------------------
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
        Public Property IsVisible() As Boolean = True

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the scale max value to render to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ScaleMax() As Single = cCore.NULL_VALUE

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the scale min value to render to.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property ScaleMin() As Single = cCore.NULL_VALUE

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the display text for a given cell in the underlying data.
        ''' </summary>
        ''' <returns>The display text for a given cell in the underlying data.</returns>
        ''' -----------------------------------------------------------------------
        Public MustOverride Function GetDisplayText(value As Object) As String

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set how the <see cref="eLayerRenderType">layer should be drawn</see>.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property RenderMode As eLayerRenderType = eLayerRenderType.Selected

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the number of extra symbols, beyond the regular cell value, that 
        ''' this renderer uses and will need displaying in legends.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable ReadOnly Property nExtraSymbols As Integer
            Get
                Return 0
            End Get
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the name of all <see cref="nExtraSymbols">extra symbols</see> that need 
        ''' displaying in legends.
        ''' </summary>
        ''' <param name="iSymbol">The one-based symbol index.</param>
        ''' -----------------------------------------------------------------------
        Public Overridable ReadOnly Property SymbolName(iSymbol As Integer) As String
            Get
                Return ""
            End Get
        End Property

    End Class

End Namespace
