#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ScientificInterface.Ecospace.Basemap
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Win32Api
Imports EwEUtils.Core
Imports System.Drawing.Imaging

#End Region ' Imports

Namespace Ecospace

    Public Class cLegend

#Region " Private vars "

        Private Enum eLayerRenderStyle As Integer
            Symbol
            Element
            Gradient
        End Enum

        Private m_map As ucMap = Nothing
        Private m_uic As cUIContext = Nothing
        Private m_lLayers As New List(Of cLayer)

        Private m_bShowTitle As Boolean = True
        Private m_iTitleVSpacing As Integer = 8

        Private m_iLayerBoxWidth As Integer = 20
        Private m_iLayerBoxHeight As Integer = 12
        Private m_iLayerBoxHSpacing As Integer = 5
        Private m_iLayerBoxVSpacing As Integer = 4

#End Region ' Private vars

        Private Sub New(ByVal map As ucMap)

            Me.m_map = map
            Me.m_uic = map.UIContext

            Dim al As cLayer() = Me.m_map.Layers
            Dim l As cLayer = Nothing
            Dim r As cLayerRenderer = Nothing

            For i As Integer = 0 To al.Length - 1
                l = al(i)
                If (l IsNot Nothing) Then
                    r = l.Renderer
                    If (r IsNot Nothing) Then
                        If r.IsVisible Then
                            Me.m_lLayers.Add(l)
                        End If
                    End If
                End If
            Next

        End Sub

#Region " Shared interfaces "

        Public Shared Function FromMap(ByVal map As ucMap) As cLegend
            Return New cLegend(map)
        End Function

#End Region ' Shared interfaces

#Region " Public interfaces "

        Public Function SaveAsBitmap(ByVal strFileName As String, ByVal format As ImageFormat) As Boolean

            If Me.m_uic Is Nothing Then Return False

            Dim ftTitle As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
            Dim ftGroup As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)

            ' Measure size of legend
            Dim iWidth As Integer = 0
            Dim iHeight As Integer = 0
            Dim szfItem As SizeF = Nothing

            Dim bSuccess As Boolean = True

            Using bmpTmp As New Bitmap(1000, 300, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmpTmp)

                    If Me.m_bShowTitle Then
                        szfItem = Me.RenderTitleSize(g, ftTitle)
                        iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                        iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.m_iTitleVSpacing
                    End If

                    For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                        szfItem = Me.RenderLayerSize(g, ftLayer, Me.m_lLayers(iLayer))
                        iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                        iHeight += CInt(Math.Ceiling(szfItem.Height))
                        If iLayer > 0 Then iHeight += Me.m_iLayerBoxVSpacing
                    Next iLayer

                End Using ' g
            End Using ' bmp

            Using bmp As New Bitmap(iWidth, iHeight, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)

                    If format Is ImageFormat.Png Then
                        g.FillRectangle(Brushes.Transparent, 0, 0, iWidth, iHeight)
                    Else
                        Using br As New SolidBrush(Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND))
                            g.FillRectangle(br, 0, 0, iWidth, iHeight)
                        End Using
                    End If

                    iWidth = 0 : iHeight = 0

                    If Me.m_bShowTitle Then
                        szfItem = Me.RenderTitleSize(g, ftTitle)
                        iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                        Me.RenderTitle(g, ftTitle, New Point(0, iHeight))
                        iHeight += CInt(Math.Ceiling(szfItem.Height)) + Me.m_iTitleVSpacing
                    End If

                    For iLayer As Integer = 0 To Me.m_lLayers.Count - 1
                        szfItem = Me.RenderLayerSize(g, ftLayer, Me.m_lLayers(iLayer))
                        Me.RenderLayer(g, ftTitle, Me.m_lLayers(iLayer), New Point(0, iHeight))
                        iWidth = Math.Max(iWidth, CInt(Math.Ceiling(szfItem.Width)))
                        iHeight += CInt(Math.Ceiling(szfItem.Height))
                        If iLayer > 0 Then iHeight += Me.m_iLayerBoxVSpacing
                    Next iLayer

                End Using ' g

                Try
                    bmp.Save(strFileName, format)
                Catch ex As Exception
                    bSuccess = False
                End Try

            End Using ' bmp

            ftTitle.Dispose()
            ftGroup.Dispose()
            ftLayer.Dispose()

            Return bSuccess

        End Function

#End Region ' Public interfaces

#Region " Internals "

        Private Function RenderTitleSize(ByVal g As Graphics, ByVal ft As Font) As SizeF
            Return g.MeasureString(Me.m_map.Title, ft)
        End Function

        Private Sub RenderTitle(ByVal g As Graphics, ByVal ft As Font, ByVal ptLocation As Point)
            g.DrawString(Me.m_map.Title, ft, Brushes.Black, ptLocation)
        End Sub

        Private Function RenderLayerSize(ByVal g As Graphics, ByVal ft As Font, ByVal l As cLayer) As SizeF

            Dim sLayerBox As SizeF = g.MeasureString(l.Name, ft)
            Select Case Me.GetRenderStyle(l)
                Case eLayerRenderStyle.Element, eLayerRenderStyle.Symbol
                    ' NOP
                Case eLayerRenderStyle.Gradient
                    sLayerBox.Height *= 3
            End Select
            sLayerBox.Width = Math.Max(Me.m_iLayerBoxWidth + Me.m_iLayerBoxHSpacing, sLayerBox.Width)
            sLayerBox.Height = Math.Max(Me.m_iLayerBoxHeight, sLayerBox.Height)
            Return sLayerBox

        End Function

        Private Sub RenderLayer(ByVal g As Graphics, ByVal ft As Font, ByVal l As cLayer, ByVal ptLocation As Point)

            Dim szfBox As SizeF = Me.RenderLayerSize(g, ft, l)
            Dim rcPreview As Rectangle = New Rectangle(ptLocation.X, ptLocation.Y, 20, CInt(szfBox.Height))

            Select Case Me.GetRenderStyle(l)

                Case eLayerRenderStyle.Element
                    l.Renderer.RenderPreview(g, rcPreview, l.Data)
                    g.DrawRectangle(Pens.Black, rcPreview)
                    g.DrawString(l.Name, ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y)

                Case eLayerRenderStyle.Symbol
                    l.Renderer.RenderPreview(g, rcPreview, l.Data)
                    g.DrawString(l.Name, ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y)

                Case eLayerRenderStyle.Gradient
                    l.Renderer.RenderPreview(g, rcPreview, l.Data)
                    g.DrawRectangle(Pens.Black, rcPreview)
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(l.Data.MaxValue), _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y)
                    g.DrawString(l.Name, _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y + (szfBox.Height / 3))
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(0), _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y + (szfBox.Height * 2 / 3))
            End Select
        End Sub

        Private Function GetRenderStyle(ByVal l As cLayer) As eLayerRenderStyle
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
