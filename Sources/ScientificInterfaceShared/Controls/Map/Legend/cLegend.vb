' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On


Imports System.Drawing.Imaging
Imports EwEUtils.SystemUtilities
Imports EwEUtils.UserInterface
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style



Namespace Controls.Map

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Simple map legend rendererfor <see cref="cDisplayLayer">display layers</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLegend

#Region " Private classes "

        ''' <summary>
        ''' An entry in the legend.
        ''' </summary>
        Private MustInherit Class cLegendEntry
            MustOverride ReadOnly Property Label As String
            MustOverride ReadOnly Property Renderer As cLayerRenderer
            MustOverride ReadOnly Property LabelMax As String
            MustOverride ReadOnly Property LabelMin As String
        End Class

        ''' <summary>
        ''' A static legend entry - one that does not vary.
        ''' </summary>
        Private Class cStaticEntry
            Inherits cLegendEntry

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="strName"></param>
            ''' <param name="strUnits"></param>
            ''' <param name="strLabelMin"></param>
            ''' <param name="strLabelMax"></param>
            ''' <param name="breaks">Optional gradient color breaks</param>
            ''' <param name="colors">Optional gradient color breaks</param>
            Public Sub New(uic As cUIContext, strName As String, strUnits As String, strLabelMin As String, strLabelMax As String,
                           Optional breaks As Double() = Nothing, Optional colors() As Color = Nothing)

                ' Bwech
                Dim l As New List(Of VisualColor)
                If (colors IsNot Nothing) Then
                    For i As Integer = 0 To colors.Length
                        l.Add(cStyleGuide.ToVisualColor(colors(i)))
                    Next
                End If

                If String.IsNullOrWhiteSpace(strUnits) Then
                    Me.Label = strName
                Else
                    Me.Label = cStringUtils.Localize(My.Resources.GENERIC_LABEL_DETAILED, strName, strUnits)
                End If
                Me.LabelMin = strLabelMin
                Me.LabelMax = strLabelMax

                Dim vs As New Auxiliary.cVisualStyle()
                If (breaks IsNot Nothing And colors IsNot Nothing) Then
                    vs.ColorRampBreaks = breaks
                    vs.ColorRampColors = l.ToArray()
                End If
                Me.Renderer = New cLayerRendererValue(uic, vs)
                Me.Renderer.LabelMin = strLabelMin
                Me.Renderer.LabelMax = strLabelMax

            End Sub

            Public Overrides ReadOnly Property Renderer As Layers.cLayerRenderer

            Public Overrides ReadOnly Property Label As String

            Public Overrides ReadOnly Property LabelMax As String

            Public Overrides ReadOnly Property LabelMin As String

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' A legend entry for a single <see cref="cDisplayLayer">display layer</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Class cLayerEntry
            Inherits cLegendEntry

            Private m_layer As cDisplayLayer

            Public Sub New(layer As cDisplayLayer)
                Me.m_layer = layer
            End Sub

            Public Overrides ReadOnly Property Renderer As Layers.cLayerRenderer
                Get
                    Return Me.m_layer.Renderer
                End Get
            End Property

            Public Overrides ReadOnly Property Label As String
                Get
                    If String.IsNullOrWhiteSpace(Me.m_layer.Units) Then
                        Return Me.m_layer.Name
                    Else
                        Return cStringUtils.Localize(My.Resources.GENERIC_LABEL_DETAILED, Me.m_layer.Name, Me.m_layer.Units)
                    End If
                End Get
            End Property

            Public Overrides ReadOnly Property LabelMax As String
                Get
                    If (TypeOf Me.m_layer Is cDisplayLayerRaster) Then
                        Dim l As cDisplayLayerRaster = DirectCast(Me.m_layer, cDisplayLayerRaster)
                        Dim strMax As String = l.Renderer.LabelMax
                        If Not String.IsNullOrWhiteSpace(strMax) Then Return strMax
                        Return cStringUtils.FormatNumber(l.Data.MaxValue)
                    End If
                    Return ""
                End Get
            End Property

            Public Overrides ReadOnly Property LabelMin As String
                Get
                    If (TypeOf Me.m_layer Is cDisplayLayerRaster) Then
                        Dim l As cDisplayLayerRaster = DirectCast(Me.m_layer, cDisplayLayerRaster)
                        Dim strMin As String = l.Renderer.LabelMin
                        If Not String.IsNullOrWhiteSpace(strMin) Then Return strMin
                        Return cStringUtils.FormatNumber(l.Data.MinValue)
                    End If
                    Return ""
                End Get
            End Property

        End Class

#End Region ' Private helper classes

#Region " Private vars "

        Private Enum eLayerRenderStyle As Integer
            Symbol
            Element
            Gradient
        End Enum

        Private m_uic As cUIContext = Nothing
        Private m_lLayers As New List(Of cLegendEntry)
        Private m_fmt As New StringFormat(StringFormat.GenericTypographic)

#End Region ' Private vars

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a legend for all the current layers in a map.
        ''' </summary>
        ''' <param name="map">The map to populate the legend from.</param>
        ''' -------------------------------------------------------------------
        Private Sub New(map As ucMap)

            Me.New(map.UIContext, map.Title)

            Dim al As cDisplayLayer() = map.Layers
            Dim l As cDisplayLayer = Nothing
            Dim r As cLayerRenderer = Nothing

            For i As Integer = 0 To al.Length - 1
                l = al(i)
                If (l IsNot Nothing) Then
                    r = l.Renderer
                    If (r IsNot Nothing) Then
                        If r.IsVisible Then
                            ' Only add layers with a value range
                            Me.m_lLayers.Add(New cLayerEntry(l))
                        End If
                    End If
                End If
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, create a new legend.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext"/> to use.</param>
        ''' <param name="strTitle">Legend title.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, Optional strTitle As String = "")
            Me.m_uic = uic
            Me.Title = strTitle
            Me.m_fmt.Alignment = If(cSystemUtils.IsRightToLeft, StringAlignment.Far, StringAlignment.Near)
            Me.m_fmt.LineAlignment = StringAlignment.Center
        End Sub

#End Region ' Constructors

#Region " Shared interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Construct a new legend for a map.
        ''' </summary>
        ''' <param name="map">The map to populate the legend from.</param>
        ''' <returns>A <see cref="cLegend">legend</see>.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function FromMap(map As ucMap) As cLegend
            Debug.Assert(map IsNot Nothing)
            Return New cLegend(map)
        End Function

#End Region ' Shared interfaces

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the legend title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Title As String = ""

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the legend should show its title.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ShowTitle As Boolean = True

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the vertical spacing between the legend title and the first 
        ''' layer box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property TitleVSpacing As Integer = 8

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the width of a layer box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxWidth As Integer = 24

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the height of a layer box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxHeight As Integer = 16

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the horizontal spacing between a layer box and its label.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxHSpacing As Integer = 4

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the vertical spacing between two layer boxes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxVSpacing As Integer = 6

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a <see cref="cDisplayLayer"/> to the legend. Note that the layer
        ''' needs to be <see cref="cLayerRenderer.IsVisible">visible</see> to be included in the legend.
        ''' </summary>
        ''' <param name="l"></param>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(l As cDisplayLayer)
            If l.Renderer.IsVisible Then
                Me.m_lLayers.Add(New cLayerEntry(l))
            End If
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a static value range to the legend, that will be displayed as a gradient.
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="strLabelMin"></param>
        ''' <param name="strLabelMax"></param>
        ''' <param name="strUnits">Units to show.</param>
        ''' <param name="breaks">Gradient breaks, if any.</param>
        ''' <param name="colors">Gradient colors, if any.</param>
        ''' -------------------------------------------------------------------
        Public Sub AddGradient(strName As String, strLabelMin As String, strLabelMax As String,
                               Optional strUnits As String = "",
                               Optional breaks As Double() = Nothing, Optional colors() As Color = Nothing)
            Me.m_lLayers.Add(New cStaticEntry(Me.m_uic, strName, strUnits, strLabelMin, strLabelMax, breaks, colors))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the size of the legend, when rendered with the current
        ''' <see cref="cStyleGuide.Font">styleguide font settings</see> and
        ''' content. 
        ''' </summary>
        ''' <param name="g">The graphics to calculate for.</param>
        ''' <returns>A size.</returns>
        ''' -------------------------------------------------------------------
        Private Function Measure(g As Graphics) As Size

            Dim ftTitle As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            Dim ftScale As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)

            ' Measure size of legend
            Dim iWidth As Integer = 0
            Dim iHeight As Integer = 0
            Dim szfItem As SizeF = Nothing

            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            Try
                If Me.ShowTitle Then
                    szfItem = Me.RenderTitleSize(g, ftTitle)
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.TitleVSpacing
                End If

                For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                    szfItem = Me.MeasureLayer(g, ftLayer, ftScale, Me.m_lLayers(iLayer))
                    If iLayer > 0 Then iHeight += 2 * Me.LayerBoxVSpacing
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    iHeight += CInt(Math.Ceiling(szfItem.Height))
                Next iLayer
            Catch ex As Exception

            End Try

            iHeight += 1 + 2 * Me.LayerBoxVSpacing
            iWidth += 1 + 2 * Me.LayerBoxHSpacing

            ftScale.Dispose()
            ftTitle.Dispose()
            ftLayer.Dispose()

            Return New Size(iWidth, iHeight)

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draw the legend on a graphics device.
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="ptOrigin">Top-left location to draw the legend.</param>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public Function Draw(g As Graphics, ptOrigin As Point) As Boolean

            Dim ftTitle As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            Dim ftScale As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
            Dim szfItem As SizeF = Nothing
            Dim iHeight As Integer
            Dim bSuccess As Boolean = True

            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            g.TextRenderingHint = Drawing.Text.TextRenderingHint.AntiAlias

            ptOrigin.X += Me.LayerBoxHSpacing
            ptOrigin.Y += Me.LayerBoxHSpacing

            Try

                If Me.ShowTitle Then
                    szfItem = Me.RenderTitleSize(g, ftTitle)
                    Me.DrawTitle(g, ftTitle, New Point(ptOrigin.X, ptOrigin.Y + iHeight))
                    iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.TitleVSpacing
                End If

                For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                    szfItem = Me.MeasureLayer(g, ftLayer, ftScale, Me.m_lLayers(iLayer))
                    If iLayer > 0 Then iHeight += 2 * Me.LayerBoxVSpacing
                    Me.DrawLayer(g, ftLayer, ftScale, Me.m_lLayers(iLayer), New Point(ptOrigin.X, ptOrigin.Y + iHeight))
                    iHeight += CInt(Math.Ceiling(szfItem.Height))
                Next iLayer

            Catch ex As Exception
                bSuccess = False
            End Try

            ftScale.Dispose()
            ftTitle.Dispose()
            ftLayer.Dispose()

            Return bSuccess
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Save the legend image to a file.
        ''' </summary>
        ''' <param name="strFileName"></param>
        ''' <param name="format"></param>
        ''' <returns>True if successful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Save(strFileName As String, format As ImageFormat) As Boolean

            If (Me.m_uic Is Nothing) Then Return False

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim szLegend As Size = Nothing
            Dim bSuccess As Boolean = True

            Using bmp As Bitmap = sg.GetImage(1000, 300, format)
                Using g As Graphics = Graphics.FromImage(bmp)
                    szLegend = Me.Measure(g)
                End Using ' g
            End Using ' bmp

            Try
                Using bmp As Bitmap = sg.GetImage(szLegend.Width, szLegend.Height, format, strFileName)
                    Using g As Graphics = Graphics.FromImage(bmp)
                        bSuccess = Me.Draw(g, New Point(0, 0))
                    End Using ' g
                    bmp.Save(strFileName, format)
                End Using ' bmp
            Catch ex As Exception
                bSuccess = False
            End Try

            Return bSuccess

        End Function

#End Region ' Public interfaces

#Region " Internals "

        Private Function RenderTitleSize(g As Graphics, ft As Font) As SizeF
            Return g.MeasureString(Me.Title, ft, 10000, Me.m_fmt)
        End Function

        Private Sub DrawTitle(g As Graphics, ft As Font, pt As Point)
            g.DrawString(Me.Title, ft, Brushes.Black, pt)
        End Sub

        Private Function MeasureLayer(g As Graphics, ftLabel As Font, ftScale As Font, l As cLegendEntry) As SizeF

            Dim style As eLayerRenderStyle = Me.GetRenderStyle(l)
            Dim szBox As New SizeF(0, 0)
            Dim strText As String = ""

            For i As Integer = 0 To l.Renderer.nSymbols

                If (i = 0) Then
                    strText = l.Label
                Else
                    strText = l.Renderer.SymbolName(i)
                    style = eLayerRenderStyle.Element
                    szBox.Height += Me.LayerBoxVSpacing
                End If

                Dim szName As SizeF = g.MeasureString(If(String.IsNullOrWhiteSpace(strText), "XXXXXXXXX", strText), ftLabel, 10000, Me.m_fmt)

                Select Case style
                    Case eLayerRenderStyle.Element, eLayerRenderStyle.Symbol
                        szName.Height = Math.Max(Me.LayerBoxHeight, szName.Height)
                    Case eLayerRenderStyle.Gradient
                        szName.Height = Math.Max(Me.LayerBoxHeight, szName.Height * 3)
                End Select

                szBox.Width = Me.LayerBoxWidth + Me.LayerBoxHSpacing + szName.Width
                szBox.Height += szName.Height

            Next

            Return szBox

        End Function

        Private Sub DrawLayer(g As Graphics, ftLabel As Font, ftScale As Font, l As cLegendEntry, pt As Point)

            Dim style As eLayerRenderStyle = Me.GetRenderStyle(l)
            Dim strText As String = ""

            For i As Integer = 0 To l.Renderer.nSymbols

                If (i = 0) Then
                    strText = l.Label
                Else
                    strText = l.Renderer.SymbolName(i)
                    style = eLayerRenderStyle.Element
                End If

                Dim szName As SizeF = g.MeasureString(If(String.IsNullOrWhiteSpace(strText), "XXXXXXXXX", strText), ftLabel, 10000, Me.m_fmt)
                Dim rcPreview As Rectangle = New Rectangle(pt.X, pt.Y, Me.LayerBoxWidth, CInt(Math.Max(Me.LayerBoxHeight, szName.Height)))
                Dim iSymbolKey As Integer = l.Renderer.SymbolKey(i)
                Dim fmt As StringFormat = CType(Me.m_fmt.Clone(), StringFormat)
                Dim rcItem As New RectangleF(pt.X + Me.LayerBoxHSpacing + rcPreview.Width, pt.Y, szName.Width, szName.Height)

                Select Case style

                    Case eLayerRenderStyle.Element
                        l.Renderer.RenderPreview(g, rcPreview, iSymbolKey)
                        g.DrawRectangle(Pens.Black, rcPreview)
                        g.DrawString(strText, ftLabel, Brushes.Black, rcItem, fmt)

                    Case eLayerRenderStyle.Symbol
                        l.Renderer.RenderPreview(g, rcPreview, iSymbolKey)
                        g.DrawString(strText, ftLabel, Brushes.Black, rcItem, fmt)

                    Case eLayerRenderStyle.Gradient
                        If (Not String.IsNullOrWhiteSpace(l.LabelMax)) Then
                            fmt.LineAlignment = StringAlignment.Near
                            g.DrawString(l.LabelMax, ftScale, Brushes.Black, rcItem, fmt)
                        End If

                        rcItem.Y += rcPreview.Height
                        If (Not String.IsNullOrWhiteSpace(l.Label)) Then
                            fmt.LineAlignment = StringAlignment.Center
                            g.DrawString(l.Label, ftLabel, Brushes.Black, rcItem, fmt)
                        End If

                        rcItem.Y += rcPreview.Height
                        If (Not String.IsNullOrWhiteSpace(l.LabelMin)) Then
                            fmt.LineAlignment = StringAlignment.Far
                            g.DrawString(l.LabelMin, ftScale, Brushes.Black, rcItem, fmt)
                        End If

                        rcPreview.Height *= 3
                        l.Renderer.RenderPreview(g, rcPreview)
                        g.DrawRectangle(Pens.Black, rcPreview)

                End Select

                pt.Y += Me.LayerBoxHSpacing + rcPreview.Height

            Next

        End Sub

        Private Function GetRenderStyle(l As cLegendEntry) As eLayerRenderStyle
            If (TypeOf (l.Renderer) Is cLayerRendererValue) Then
                Return eLayerRenderStyle.Gradient
            ElseIf (TypeOf (l.Renderer) Is cLayerRendererSymbol) Then
                Return eLayerRenderStyle.Symbol
            Else
                Return eLayerRenderStyle.Element
            End If
        End Function

#End Region ' Internals

    End Class

End Namespace
