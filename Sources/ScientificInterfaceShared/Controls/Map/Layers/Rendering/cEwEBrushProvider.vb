#Region " Imports "

Option Strict On

Imports System.Drawing.Drawing2D
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    Public Class cEwEBrushProvider

        Private m_abrDefaultGlyphs As Image() = { _
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

        Private m_abrDefaultHatchPatterns As HatchStyle() = {HatchStyle.DiagonalCross, _
                                                  HatchStyle.Cross, _
                                                  HatchStyle.DiagonalBrick, _
                                                  HatchStyle.Divot, _
                                                  HatchStyle.LightHorizontal, _
                                                  HatchStyle.Shingle, _
                                                  HatchStyle.ZigZag, _
                                                  HatchStyle.SmallGrid, _
                                                  HatchStyle.DashedVertical, _
                                                  HatchStyle.Plaid}

        Private m_agrads As cARGBColorRamp() = { _
            New cARGBColorRamp(New Color() {Color.LightSkyBlue, Color.DarkBlue}, New Double() {0, 1}), _
            New cARGBColorRamp(New Color() {Color.LightSeaGreen, Color.DarkGreen}, New Double() {0, 1}), _
            New cARGBColorRamp(New Color() {Color.LightSteelBlue, Color.DarkGreen}, New Double() {0, 1}), _
            New cARGBColorRamp(New Color() {Color.LightYellow, Color.DarkRed}, New Double() {0, 1}), _
            New cARGBColorRamp(New Color() {Color.SandyBrown, Color.SaddleBrown}, New Double() {0, 1}) _
        }

        Private m_brHightLightDefault As Brush = Brushes.Red

        ''' <summary>Enumerated type providing supported types of brushes.</summary>
        Enum eBrushType As Integer
            ''' <summary>Items are rendered as a single colour.</summary>
            Color
            ''' <summary>Items are rendered as a hatch pattern.</summary>
            HatchPattern
            ''' <summary>Items are rendered as an image.</summary>
            Glyphs
            ''' <summary>Items are rendered as gradients.</summary>
            Gradient
        End Enum

        Public Function GetVisualStyles(ByVal nBrushes As Integer, _
                                        Optional ByVal brushType As eBrushType = cEwEBrushProvider.eBrushType.Color) As cVisualStyle()
            Dim avs As cVisualStyle() = Nothing

            Select Case brushType
                Case cEwEBrushProvider.eBrushType.Color
                    Debug.Assert(nBrushes >= 0)
                    ReDim avs(nBrushes)
                    Me.GetColors(avs)

                Case cEwEBrushProvider.eBrushType.Glyphs
                    If (nBrushes <= 0) Then nBrushes = m_abrDefaultGlyphs.Length
                    ReDim avs(nBrushes)
                    Me.GetGlyphs(avs, m_abrDefaultGlyphs)

                Case cEwEBrushProvider.eBrushType.HatchPattern
                    If (nBrushes <= 0) Then nBrushes = m_abrDefaultHatchPatterns.Length
                    ReDim avs(nBrushes)
                    Me.GetPatterns(avs, m_abrDefaultHatchPatterns)

                Case eBrushType.Gradient
                    If (nBrushes <= 0) Then nBrushes = m_agrads.Length
                    ReDim avs(nBrushes)
                    Me.GetGradients(avs, m_agrads)

            End Select

            ' ok, done
            Return avs
        End Function

#Region " Internal implementation "

        Private Sub GetColors(ByVal avs() As cVisualStyle)

            Dim vs As cVisualStyle = Nothing
            Dim clrramp As New cEwEColorRamp()

            ' Loop through number of requested visual styles
            For i As Integer = 0 To avs.Length - 1
                ' Build visual style
                vs = New cVisualStyle()
                vs.ForeColour = clrramp.GetColor(i, avs.Length - 1)
                ' Store
                avs(i) = vs
            Next i
        End Sub

        Private Sub GetGlyphs(ByVal avs() As cVisualStyle, ByVal images() As Image)

            Dim vs As cVisualStyle = Nothing
            Dim iGlyphIndex As Integer = 0

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1

                vs = New cVisualStyle()
                vs.ForeColour = Color.Gray
                vs.BackColour = Color.Transparent
                vs.Image = images(iGlyphIndex)

                avs(i) = vs

                ' increment counter
                iGlyphIndex += 1
                If iGlyphIndex = images.Length Then iGlyphIndex = 0
            Next i

        End Sub

        Private Sub GetPatterns(ByVal avs() As cVisualStyle, ByVal hatches As HatchStyle())

            Dim vs As cVisualStyle = Nothing
            Dim iPatternIndex As Integer = 0

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1

                vs = New cVisualStyle()
                vs.HatchStyle = hatches(iPatternIndex)
                avs(i) = vs

                ' increment counter
                iPatternIndex += 1
                If iPatternIndex = hatches.Length Then iPatternIndex = 0
            Next i

        End Sub

        Private Sub GetGradients(ByVal avs() As cVisualStyle, ByVal ramps As cARGBColorRamp())

            Dim vs As cVisualStyle = Nothing
            Dim iPatternIndex As Integer = 0

            ' Loop through number of brushes
            For i As Integer = 0 To avs.Length - 1

                vs = New cVisualStyle()
                vs.GradientColors = ramps(i).GradientColors
                vs.GradientBreaks = ramps(i).GradientBreaks

                avs(i) = vs

                ' increment counter
                iPatternIndex += 1
                If iPatternIndex = ramps.Length Then iPatternIndex = 0
            Next i

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace ' Controls
