'==============================================================================
'
' $Log: cEwEBrushProvider.vb,v $
' Revision 1.1  2008/09/26 07:31:26  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/06/04 00:59:00  jeroens
' Moved
'
' Revision 1.2  2008/05/07 19:16:35  jeroens
' Added a whack of glyps
'
' Revision 1.1  2008/01/06 09:12:19  jeroens
' Moved
'
'==============================================================================

#Region "Imports Directives"

Option Strict On

Imports EwECore
Imports SAUPUtil.SAUPData
Imports SAUPUtil.SAUPData.Mapping
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D

#End Region

Namespace Controls

    Public Class cEwEBrushProvider

        Private m_abrGlyphsDefaults As Image() = { _
                My.Resources.glyph_blue1, _
                My.Resources.glyph_blue2, _
                My.Resources.glyph_blue3, _
                My.Resources.glyph_blue4, _
                My.Resources.glyph_blue5, _
                My.Resources.glyph_blue6, _
                My.Resources.glyph_blue7, _
                My.Resources.glyph_blue8, _
                My.Resources.glyph_blue9, _
                My.Resources.glyph_blue10, _
                My.Resources.glyph_deep_beige, _
                My.Resources.glyph_deep_blue, _
                My.Resources.glyph_deep_brown, _
                My.Resources.glyph_deep_bw, _
                My.Resources.glyph_deep_green, _
                My.Resources.glyph_muddy_blue, _
                My.Resources.glyph_muddy_brown, _
                My.Resources.glyph_muddy_bw, _
                My.Resources.glyph_muddy_green, _
                My.Resources.glyph_rubble_blue, _
                My.Resources.glyph_rubble_lightblue, _
                My.Resources.glyph_rubble_brown, _
                My.Resources.glyph_rubble_bw, _
                My.Resources.glyph_rubble_sand, _
                My.Resources.glyph_rubble_green, _
                My.Resources.glyph_seagrass_brown, _
                My.Resources.glyph_seagrass_bw, _
                My.Resources.glyph_seagrass_dark, _
                My.Resources.glyph_seagrass_red, _
                My.Resources.glyph_arrows_down, _
                My.Resources.glyph_arrows_up, _
                My.Resources.glyph_blocks_large, _
                My.Resources.glyph_blocks_small, _
                My.Resources.glyph_squares_large, _
                My.Resources.glyph_squares_small, _
                My.Resources.glyph_dots_large, _
                My.Resources.glyph_dots_small, _
                My.Resources.glyph_circles_large, _
                My.Resources.glyph_circles_small}

        Private m_abrPatternDefaults As HatchStyle() = {HatchStyle.DiagonalCross, _
                                                  HatchStyle.Cross, _
                                                  HatchStyle.DiagonalBrick, _
                                                  HatchStyle.Divot, _
                                                  HatchStyle.LightHorizontal, _
                                                  HatchStyle.Shingle, _
                                                  HatchStyle.ZigZag, _
                                                  HatchStyle.SmallGrid, _
                                                  HatchStyle.DashedVertical, _
                                                  HatchStyle.Plaid}

        Private m_brHightLightDefault As Brush = Brushes.Red

        Enum eBrushType As Integer
            Color
            Pattern
            Glyphs
        End Enum

        Public Function GetVisualStyles(ByVal nBrushes As Integer, Optional ByVal brushType As eBrushType = cEwEBrushProvider.eBrushType.Color) As cVisualStyle()
            Dim avs As cVisualStyle() = Nothing

            Select Case brushType
                Case cEwEBrushProvider.eBrushType.Color
                    Debug.Assert(nBrushes >= 0)
                    ReDim avs(nBrushes)
                    GetColors(avs)
                Case cEwEBrushProvider.eBrushType.Glyphs
                    If (nBrushes < 0) Then nBrushes = m_abrGlyphsDefaults.Length
                    ReDim avs(nBrushes)
                    GetGlyphs(avs)
                Case cEwEBrushProvider.eBrushType.Pattern
                    If (nBrushes < 0) Then nBrushes = m_abrPatternDefaults.Length
                    ReDim avs(nBrushes)
                    GetPatterns(avs)
            End Select

            ' ok, Done
            Return avs
        End Function

#Region " Internal implementation "

        Private Sub GetColors(ByVal avs() As cVisualStyle)
            Dim vs As cVisualStyle = Nothing
            Dim lgnd As Legend = LegendPicker.CreateLegend(LegendScaleFactory.LegendScaleType.Linear, avs.Length, avs.Length, 0, LegendScale.eFriendlyValueType.Original)
            lgnd.ColorRamp = New SAUPColorRamp()

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1
                ' store the Visual Style
                vs = New cVisualStyle()
                vs.ForeColour = lgnd.GetColor(i)
                avs(i) = vs
            Next i
        End Sub

        Private Sub GetGlyphs(ByVal avs() As cVisualStyle)
            Dim vs As cVisualStyle = Nothing
            Dim iGlyphIndex As Integer = 0

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1

                vs = New cVisualStyle()
                vs.ForeColour = Color.Gray
                vs.BackColour = Color.Transparent
                vs.Image = m_abrGlyphsDefaults(iGlyphIndex)

                avs(i) = vs

                ' increment counter
                iGlyphIndex += 1
                If iGlyphIndex = m_abrGlyphsDefaults.Length Then iGlyphIndex = 0
            Next i
        End Sub

        Private Sub GetPatterns(ByVal avs() As cVisualStyle)
            Dim vs As cVisualStyle = Nothing
            Dim iPatternIndex As Integer = 0

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1

                vs = New cVisualStyle()
                vs.ForeColour = Color.Gray
                vs.BackColour = Color.Transparent
                vs.HatchStyle = m_abrPatternDefaults(iPatternIndex)

                avs(i) = vs

                ' increment counter
                iPatternIndex += 1
                If iPatternIndex = m_abrPatternDefaults.Length Then iPatternIndex = 0
            Next i
            ' Yay done, no need to return anything coz passed by ref
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace ' Controls
