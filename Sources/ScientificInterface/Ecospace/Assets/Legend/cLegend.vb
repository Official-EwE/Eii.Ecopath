#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports ScientificInterface.Other
Imports ScientificInterface.Ecospace.Basemap
Imports ScientificInterface.Ecospace.Basemap.Layers
Imports EwEUtils.Win32Api
Imports EwEUtils.Core

#End Region ' Imports

Namespace Ecospace

    Public Class cLegend

#Region " Private vars "

        Private Enum eLayerRenderStyle As Integer
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

        Private Sub New(ByVal map As ucMap, ByVal uic As cUIContext)

            Me.m_map = map
            Me.m_uic = uic

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

        Public Shared Function FromMap(ByVal map As ucMap, ByVal uic As cUIContext) As cLegend
            Return New cLegend(map, uic)
        End Function

#End Region ' Shared interfaces

#Region " Public interfaces "

        Public Function SaveAsBitmap(ByVal strFileName As String, ByVal format As System.Drawing.Imaging.ImageFormat) As Boolean


            Dim ftTitle As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Title)
            Dim ftGroup As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
            Dim ftLayer As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Legend)

            ' Measure size of legend
            Dim iWidth As Integer = 0
            Dim iHeight As Integer = 0
            Dim szfItem As SizeF = Nothing

            Using bmpTmp As New Bitmap(100, 100, Imaging.PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmpTmp)

                    If Me.m_bShowTitle Then
                        'szfItem = Me.RenderTitleSize(GetRenderStyle, 
                    End If
                End Using
            End Using

            ftTitle.Dispose()
            ftGroup.Dispose()
            ftLayer.Dispose()

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
                Case eLayerRenderStyle.Element
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
            l.Renderer.RenderPreview(g, rcPreview, l.Data)

            Select Case Me.GetRenderStyle(l)
                Case eLayerRenderStyle.Element
                    g.DrawString(l.Name, ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y)
                Case eLayerRenderStyle.Gradient
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(l.Data.MaxValue), _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y)
                    g.DrawString(l.Name, _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y + (szfBox.Height / 3))
                    g.DrawString(Me.m_uic.StyleGuide.FormatNumber(0), _
                                 ft, Brushes.Black, ptLocation.X + Me.m_iLayerBoxHSpacing + Me.m_iLayerBoxWidth, ptLocation.Y + (szfBox.Height * 2 / 3))
            End Select
        End Sub

        Private Function GetRenderStyle(ByVal l As cLayer) As eLayerRenderStyle
            If TypeOf (l.Renderer) Is cLayerRendererValue Then
                Return eLayerRenderStyle.Gradient
            Else
                Return eLayerRenderStyle.Element
            End If
        End Function

#End Region ' Internals

    End Class

End Namespace
