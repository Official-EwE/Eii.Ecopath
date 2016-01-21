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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing.Imaging
Imports ScientificInterfaceShared.Controls.Map.Layers
Imports ScientificInterfaceShared.Style
Imports EwECore
Imports System.IO

#End Region ' Imports

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
            MustOverride ReadOnly Property Name As String
            MustOverride ReadOnly Property Renderer As cLayerRenderer
            MustOverride ReadOnly Property Max As Single
            MustOverride ReadOnly Property Min As Single
        End Class

        ''' <summary>
        ''' A static legend entry - one that does not vary.
        ''' </summary>
        Private Class cStaticEntry
            Inherits cLegendEntry

            Private m_sMin As Single
            Private m_sMax As Single
            Private m_strName As String

            Public Sub New(strName As String, sMin As Single, sMax As Single)
                Me.m_strName = strName
                Me.m_sMin = sMin
                Me.m_sMax = sMax
            End Sub

            Public Overrides ReadOnly Property Renderer As Layers.cLayerRenderer
                Get
                    Dim rv As New cLayerRendererValue(New EwECore.Auxiliary.cVisualStyle())
                    rv.ScaleMin = Me.m_sMin
                    rv.ScaleMax = Me.m_sMax
                    Return rv
                End Get
            End Property

            Public Overrides ReadOnly Property Name As String
                Get
                    Return Me.m_strName
                End Get
            End Property

            Public Overrides ReadOnly Property Max As Single
                Get
                    Return Me.m_sMax
                End Get
            End Property

            Public Overrides ReadOnly Property Min As Single
                Get
                    Return Me.m_sMin
                End Get
            End Property
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

            Public Overrides ReadOnly Property Name As String
                Get
                    Return Me.m_layer.Name
                End Get
            End Property

            Public Overrides ReadOnly Property Max As Single
                Get
                    If (TypeOf Me.m_layer Is cDisplayRasterLayer) Then
                        Return DirectCast(Me.m_layer, cDisplayRasterLayer).Data.MaxValue
                    End If
                    Return cCore.NULL_VALUE
                End Get
            End Property

            Public Overrides ReadOnly Property Min As Single
                Get
                    If (TypeOf Me.m_layer Is cDisplayRasterLayer) Then
                        Return DirectCast(Me.m_layer, cDisplayRasterLayer).Data.MinValue
                    End If
                    Return cCore.NULL_VALUE
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
        Private m_strTitle As String = ""
        Private m_lLayers As New List(Of cLegendEntry)

#End Region ' Private vars

#Region " Constructors "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Create a legend for all the current layers in a map.
        ''' </summary>
        ''' <param name="map">The map to populate the legend from.</param>
        ''' -------------------------------------------------------------------
        Private Sub New(ByVal map As ucMap)

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
                            Me.m_lLayers.Add(New cLayerEntry(l))
                        End If
                    End If
                End If
            Next

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, create a new legend with a fixed name.
        ''' </summary>
        ''' <param name="uic"><see cref="cUIContext"/> to use.</param>
        ''' <param name="strTitle">Map title.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext, strTitle As String)
            Me.m_uic = uic
            Me.m_strTitle = strTitle
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
        Public Shared Function FromMap(ByVal map As ucMap) As cLegend
            Debug.Assert(map IsNot Nothing)
            Return New cLegend(map)
        End Function

#End Region ' Shared interfaces

#Region " Public interfaces "

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
        Public Property LayerBoxWidth As Integer = 20

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the height of a layer box.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxHeight As Integer = 12

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the horizontal spacing between a layer box and its label.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxHSpacing As Integer = 5

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the vertical spacing between two layer boxes.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property LayerBoxVSpacing As Integer = 4

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a layer to the legend.
        ''' </summary>
        ''' <param name="l"></param>
        ''' -------------------------------------------------------------------
        Public Sub AddLayer(l As cDisplayLayer)
            Me.m_lLayers.Add(New cLayerEntry(l))
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a static value range to the legend, that will be displayed as a gradient.
        ''' </summary>
        ''' <param name="strName"></param>
        ''' <param name="sMin"></param>
        ''' <param name="sMax"></param>
        ''' -------------------------------------------------------------------
        Public Sub AddGradient(strName As String, sMin As Single, sMax As Single)
            Me.m_lLayers.Add(New cStaticEntry(strName, sMin, sMax))
        End Sub

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
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)
            Dim szfItem As SizeF = Nothing
            Dim iWidth As Integer
            Dim iHeight As Integer
            Dim bSuccess As Boolean = True

            Try

                If Me.ShowTitle Then
                    szfItem = Me.RenderTitleSize(g, ftTitle)
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    Me.RenderTitle(g, ftTitle, New Point(ptOrigin.X, ptOrigin.Y + iHeight))
                    iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.TitleVSpacing
                End If

                For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                    szfItem = Me.RenderLayerSize(g, ftLayer, Me.m_lLayers(iLayer))
                    If iLayer > 0 Then iHeight += Me.LayerBoxVSpacing
                    Me.RenderLayer(g, ftLayer, Me.m_lLayers(iLayer), New Point(ptOrigin.X, ptOrigin.Y + iHeight))
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    iHeight += CInt(Math.Ceiling(szfItem.Height))
                Next iLayer

            Catch ex As Exception
                bSuccess = False
            End Try

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
        Public Function Save(ByVal strFileName As String, ByVal format As ImageFormat) As Boolean

            If (Me.m_uic Is Nothing) Then Return False

            Dim sg As cStyleGuide = Me.m_uic.StyleGuide
            Dim szLegend As Size = Nothing
            Dim bSuccess As Boolean = True

            Using bmp As New Bitmap(1000, 300, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    szLegend = Me.Size(g)
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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Calculate the size of the legend, when rendered with the current
        ''' <see cref="cStyleGuide.Font">styleguide font settings</see> and
        ''' content. 
        ''' </summary>
        ''' <param name="g">The graphics to calculate for.</param>
        ''' <returns>A size.</returns>
        ''' -------------------------------------------------------------------
        Public Function Size(g As Graphics) As Size

            Dim ftTitle As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)

            ' Measure size of legend
            Dim iWidth As Integer = 0
            Dim iHeight As Integer = 0
            Dim szfItem As SizeF = Nothing

            Try
                If Me.ShowTitle Then
                    szfItem = Me.RenderTitleSize(g, ftTitle)
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.TitleVSpacing
                End If

                For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                    szfItem = Me.RenderLayerSize(g, ftLayer, Me.m_lLayers(iLayer))
                    iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                    If iLayer > 0 Then iHeight += Me.LayerBoxVSpacing
                    iHeight += CInt(Math.Ceiling(szfItem.Height))
                Next iLayer
            Catch ex As Exception

            End Try

            iHeight += 1
            iWidth += 1

            ftTitle.Dispose()
            ftLayer.Dispose()

            Return New Size(iWidth, iHeight)

        End Function

#End Region ' Public interfaces

#Region " Internals "

        Private Function RenderTitleSize(ByVal g As Graphics, ByVal ft As Font) As SizeF
            Return g.MeasureString(Me.m_strTitle, ft)
        End Function

        Private Sub RenderTitle(ByVal g As Graphics, ByVal ft As Font, ByVal ptLocation As Point)
            g.DrawString(Me.m_strTitle, ft, Brushes.Black, ptLocation)
        End Sub

        Private Function RenderLayerSize(ByVal g As Graphics, ByVal ft As Font, ByVal l As cLegendEntry) As SizeF

            Dim strText As String = l.Name
            If String.IsNullOrWhiteSpace(strText) Then strText = "X"

            Dim sLayerBox As SizeF = g.MeasureString(strText, ft)
            Select Case Me.GetRenderStyle(l)
                Case eLayerRenderStyle.Element, eLayerRenderStyle.Symbol
                    ' NOP
                Case eLayerRenderStyle.Gradient
                    sLayerBox.Height *= 3
            End Select
            sLayerBox.Width += (Me.LayerBoxWidth + Me.LayerBoxHSpacing * 3)
            sLayerBox.Height = Math.Max(Me.LayerBoxHeight, sLayerBox.Height)

            Return sLayerBox

        End Function

        Private Sub RenderLayer(ByVal g As Graphics, ByVal ft As Font, ByVal l As cLegendEntry, ByVal ptLocation As Point)

            Dim szfBox As SizeF = Me.RenderLayerSize(g, ft, l)
            Dim rcPreview As Rectangle = New Rectangle(ptLocation.X, ptLocation.Y, 20, CInt(szfBox.Height))

            Select Case Me.GetRenderStyle(l)

                Case eLayerRenderStyle.Element
                    l.Renderer.RenderPreview(g, rcPreview)
                    g.DrawRectangle(Pens.Black, rcPreview)
                    g.DrawString(l.Name, ft, Brushes.Black, ptLocation.X + Me.LayerBoxHSpacing + Me.LayerBoxWidth, ptLocation.Y)

                Case eLayerRenderStyle.Symbol
                    l.Renderer.RenderPreview(g, rcPreview)
                    g.DrawString(l.Name, ft, Brushes.Black, ptLocation.X + Me.LayerBoxHSpacing + Me.LayerBoxWidth, ptLocation.Y)

                Case eLayerRenderStyle.Gradient
                    l.Renderer.RenderPreview(g, rcPreview)
                    g.DrawRectangle(Pens.Black, rcPreview)
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(l.Max), _
                                 ft, Brushes.Black, ptLocation.X + Me.LayerBoxHSpacing + Me.LayerBoxWidth, ptLocation.Y)
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(l.Min), _
                                 ft, Brushes.Black, ptLocation.X + Me.LayerBoxHSpacing + Me.LayerBoxWidth, ptLocation.Y + (szfBox.Height * 2 / 3))
                    g.DrawString(l.Name, _
                                 ft, Brushes.Black, ptLocation.X + Me.LayerBoxHSpacing + Me.LayerBoxWidth, ptLocation.Y + (szfBox.Height / 3))
            End Select
        End Sub

        Private Function GetRenderStyle(ByVal l As cLegendEntry) As eLayerRenderStyle
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
