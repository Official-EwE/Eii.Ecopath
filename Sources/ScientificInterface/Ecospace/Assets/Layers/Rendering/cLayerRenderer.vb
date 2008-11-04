'==============================================================================
'
' $Log: cLayerRenderer.vb,v $
' Revision 1.1  2008/11/04 04:41:34  jeroens
' Split into separate files, moved
'
' Revision 1.3  2008/11/02 22:13:11  jeroens
' Added StringRenderer
' Fixed crash on Clone
'
' Revision 1.2  2008/10/15 23:56:56  jeroens
' Simplified ValueRenderer construction
'
' Revision 1.1  2008/10/10 18:03:21  jeroens
' Renamed
'
' Revision 1.1  2008/09/26 07:31:58  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D
Imports System.Reflection

#End Region ' Imports 

Namespace Ecospace.Basemap.Layers

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
        Public MustOverride Sub RenderPreview(ByVal g As Graphics, ByVal rc As Rectangle)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Render a cell of a layer.
        ''' </summary>
        ''' <param name="g">The graphics to render onto.</param>
        ''' <param name="rc">Device area to render cell onto.</param>
        ''' <param name="value">The value to render.</param>
        ''' -----------------------------------------------------------------------
        Public MustOverride Sub RenderCell(ByVal g As Graphics, ByVal rc As Rectangle, ByVal value As Object, ByVal style As StyleGuide.eStyleFlags)

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

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' The range of values in the underlying data has changed.
        ''' </summary>
        ''' <param name="objMin">Minimum value(s) in the layer.</param>
        ''' <param name="objMax">Maximum value(s) in the layer.</param>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub SetValueRange(ByVal objMin As Object, ByVal objMax As Object)
            ' NOP
        End Sub

        Public Overridable Function Clone() As cLayerRenderer
            Dim minime As cLayerRenderer = Nothing
            Dim vs As cVisualStyle = Me.VisualStyle.Clone()

            minime = DirectCast(Activator.CreateInstance(Me.GetType(), New Object() {vs}), cLayerRenderer)
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

    End Class

End Namespace
