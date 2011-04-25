Option Strict On
Option Explicit On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.ComponentModel
Imports ScientificInterfaceShared.Style

Public Class ArrayGraph

    Private m_nodes As Boolean

    ''' <summary>Spacer between cells, expressed in cell size</summary>
    ''' <remarks></remarks>
    Private Const cCELL_PADDING_RATIO As Single = 0.2!

    Public Enum eRenderStyle As Integer
        Circles
        Bars
    End Enum

    Public Sub New()
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g">Graphics to write onto.</param>
    ''' <param name="rcRender">Area the rendition is allowed to use.</param>
    ''' <param name="asData">Array (NxN) of single values to render.</param>
    ''' <param name="strTitleX">Title to render along the X axis.</param>
    ''' <param name="astrLabelsX">Labels to render along the X axis.</param>
    ''' <param name="strTitleY">Title to render along the Y axis.</param>
    ''' <param name="astrLabelsY">Labels to render along the Y axis. If this parameter is omitted,
    ''' the X-axis labels are plotted along the Y axis and the grid is presumed to be square.</param>
    ''' <param name="astrLegends">Legends to render</param>
    ''' -----------------------------------------------------------------------
    Public Sub Draw(ByVal sg As cStyleGuide, _
                    ByVal g As Graphics, ByVal rcRender As Rectangle, _
                    ByVal style As eRenderStyle, _
                    ByVal asData As Single(,), ByVal strTitleX As String, ByVal astrLabelsX As String(), _
                    Optional ByVal strTitleY As String = Nothing, Optional ByVal astrLabelsY As String() = Nothing, _
                    Optional ByVal astrLegends As String() = Nothing, _
                    Optional ByVal bShowGrid As Boolean = False, _
                    Optional ByVal sLabelAngle As Single = 0)

        ' Use system fonts
        Dim ftScale As Font = sg.Font(cStyleGuide.eApplicationFontType.Scale)
        Dim ftLegend As Font = sg.Font(cStyleGuide.eApplicationFontType.Legend)
        Dim ftSubtitle As Font = sg.Font(cStyleGuide.eApplicationFontType.SubTitle)

        ' ToDo: take right-to-left reading order into account
        ' ToDo: allow side label positioning (left or right)

        ' == Fix defaults ==
        ' Use X-axis labels for both axis if Y-axis labels are omitted
        If astrLabelsY Is Nothing Then astrLabelsY = astrLabelsX

        ' == Sanity checks ==
        ' Make sure data and label dimensions fit
        Debug.Assert(asData.GetUpperBound(0) = astrLabelsX.Length - 1, "Data dimension not compatible with X-axis labels")
        Debug.Assert(asData.GetUpperBound(1) = astrLabelsY.Length - 1, "Data dimension not compatible with Y-axis labels")

        ' Measure max label sizes
        Dim szLabelTopMaxSize As Size = Me.CalcLabelMaxSize(g, ftScale, astrLabelsX)
        Dim szLabelSideMaxSize As Size = Me.CalcLabelMaxSize(g, ftScale, astrLabelsY)
        Dim szLegendTop As Size = Me.CalcLegendMaxSize(g, ftLegend, strTitleX)
        Dim szLegendSide As Size = Me.CalcLegendMaxSize(g, ftLegend, strTitleY)
        Dim szCellSize As Size

        ' Graph layout explanation:
        '
        '   / Area 1:        / Area 4:
        '  / Slanted labels / Graph legends
        ' +----------------+-------------
        ' |                |
        ' | Area 2:        | Area 3:
        ' | Grid           | Horz. labels
        ' |                |
        ' +----------------+-------------
        Dim iArea3Width As Integer = szLabelSideMaxSize.Width + szLegendSide.Height * 2
        Dim iArea1Height As Integer = szLabelTopMaxSize.Width + szLegendTop.Height * 2
        Dim iArea3Height As Integer = rcRender.Height - iArea1Height
        Dim iArea2Width As Integer = rcRender.Width - iArea3Width
        Dim iArea1Width As Integer = iArea2Width
        Dim iArea2Height As Integer = iArea3Height

        Dim rcArea1 As Rectangle = New Rectangle(rcRender.X, rcRender.Y, _
                                                 iArea1Width, iArea1Height)
        Dim rcArea2 As Rectangle = New Rectangle(rcRender.X, rcRender.Y + iArea1Height, _
                                                 iArea2Width, iArea2Height)
        Dim rcArea3 As Rectangle = New Rectangle(rcRender.Width - iArea3Width, iArea1Height, _
                                                 iArea3Width, iArea3Height)
        Dim rcArea4 As Rectangle = New Rectangle(rcRender.Width - iArea3Width, rcRender.Y, _
                                                 iArea3Width, iArea1Height)

        ' Figure out where to draw the graphs
        szCellSize = CalcGridSize(rcArea2, asData.GetUpperBound(0) - 1, asData.GetUpperBound(1) - 1)
        ' Top text
        DrawLabelsTop(g, ftScale, ftSubtitle, rcArea1, szCellSize, strTitleX, astrLabelsX, szLabelTopMaxSize, -90 + sLabelAngle)
        ' Side text
        DrawLabelsSide(g, ftScale, ftSubtitle, rcArea3, szCellSize, strTitleY, astrLabelsY)
        ' Graph legends
        DrawLegends(g, ftLegend, rcArea4, szCellSize, astrLegends, style)
        ' Grid
        If bShowGrid Then DrawGrid(g, rcArea2, szCellSize, asData)
        ' Graph
        DrawGraph(g, rcArea2, szCellSize, asData, style)

        ftScale.Dispose()
        ftScale = Nothing

        ftLegend.Dispose()
        ftLegend = Nothing

        ftSubtitle.Dispose()
        ftSubtitle = Nothing

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate square grid cell size.
    ''' </summary>
    ''' <param name="rect"></param>
    ''' <param name="iNumItemsOnXAxis"></param>
    ''' <param name="iNumItemsOnYAxis"></param>
    ''' <returns>Size of a single grid cell (in pixels).</returns>
    ''' -----------------------------------------------------------------------
    Private Function CalcGridSize(ByVal rect As Rectangle, _
                                  ByVal iNumItemsOnXAxis As Integer, _
                                  ByVal iNumItemsOnYAxis As Integer) As Size

        Return New Size(CInt(rect.Width / (iNumItemsOnXAxis + (1 / cCELL_PADDING_RATIO))), _
                        CInt(rect.Height / (iNumItemsOnYAxis + (1 / cCELL_PADDING_RATIO))))

    End Function

#Region " Internals "

#Region " Calculations "

    Private Function GetNormalizedArray(ByRef asData As Single(,)) As Single(,)

        ' Normalize a copy of the data, do not affect the incoming array
        Dim asNomalized(asData.GetUpperBound(0), asData.GetUpperBound(1)) As Single
        Dim sMaxValue As Single = 0.0

        For x As Integer = 0 To asData.GetUpperBound(0)
            For y As Integer = 0 To asData.GetUpperBound(1)
                ' Find data maximum to normalize to
                sMaxValue = Math.Max(sMaxValue, Math.Abs(asData(x, y)))
            Next y
        Next x

        ' Sanity check
        If sMaxValue = 0.0 Then sMaxValue = 1.0

        For x As Integer = 0 To asData.GetUpperBound(0)
            For y As Integer = 0 To asData.GetUpperBound(1)
                asNomalized(x, y) = asData(x, y) / sMaxValue
            Next y
        Next x

        Return asNomalized

    End Function

#End Region ' Calculations

#Region " Measurements "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Return the max label length and height, in pixels, when rendered with 
    ''' the selected font.
    ''' </summary>
    ''' <param name="astrLabels">Labels to check.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function CalcLabelMaxSize(ByVal g As Graphics, ByVal ft As Font, ByVal astrLabels As String()) As Size

        Dim szMax As New Size(0, 0)
        Dim szfLabel As SizeF = Nothing

        For Each strLabel As String In astrLabels
            szfLabel = g.MeasureString(strLabel, ft)
            szMax.Width = Math.Max(szMax.Width, CInt(Math.Ceiling(szfLabel.Width)))
            szMax.Height = Math.Max(szMax.Height, CInt(Math.Ceiling(szfLabel.Height)))
        Next

        Return szMax
    End Function

    Private Function CalcLegendMaxSize(ByVal g As Graphics, ByVal ft As Font, ByVal strLegend As String) As Size

        Dim szLegend As New Size(0, 0)
        Dim szfLegend As SizeF = Nothing

        szfLegend = g.MeasureString(strLegend, ft)
        szLegend.Width = CInt(Math.Ceiling(szfLegend.Width))
        szLegend.Height = CInt(Math.Ceiling(szfLegend.Height))

        Return szLegend
    End Function

#End Region ' Measurement calculations

#Region " Rendering "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="ftScale">Font to render scales with.</param>
    ''' <param name="ftSubtitle">Font to render subtitles with.</param>
    ''' <param name="rect"></param>
    ''' <param name="szCellSize">Size of a cell</param>
    ''' <param name="strLegend"></param>
    ''' <param name="astrLabels"></param>
    ''' <param name="szLabelMaxSize">Max dimensions (in pixels) of a label.</param>
    ''' <param name="sAngle">Text rotation angle.</param>
    ''' -----------------------------------------------------------------------
    Private Sub DrawLabelsTop(ByVal g As Graphics, ByVal ftScale As Font, ByVal ftSubtitle As Font, _
                              ByVal rect As Rectangle, ByVal szCellSize As Size, _
                              ByVal strLegend As String, ByVal astrLabels As String(), ByVal szLabelMaxSize As Size, _
                              Optional ByVal sAngle As Single = 0.0!)
        Dim szLegendTop As Size = Me.CalcLegendMaxSize(g, ftScale, strLegend)

        ' Label
        g.DrawString(strLegend, ftSubtitle, SystemBrushes.WindowText, _
             CInt(((rect.Width - szLegendTop.Width) / 2.0) + rect.X), rect.Y, StringFormat.GenericDefault)

        ' Why the cosine bit in the label positioning logic? 
        '
        ' Consider the following diagram of a label rendered at an angle roughly 
        ' 30 degress off of -90 (thus at -60):
        '
        '    /   
        '   /     /
        '  /     /
        ' 1._   /
        '    ^-2
        '
        ' Here, pt (1) is the top-left origin at which the label will be drawn. However, point (2), the
        ' bottom-left corner of the label, is rotated below the origin. (2) should thus move up to ensure
        ' that the label is not rendered inside the graph area. Therefore, the Y-position of the label
        ' must be moved by {szLabelMaxSize.height} * Math.Cos(sAngle)

        For i As Integer = 0 To astrLabels.GetUpperBound(0)
            'DrawAngledText(g, astrLabels(i), _
            '            CInt(i * sCellSize + sCellSize * cCELL_PADDING_RATIO), _
            '            CInt(rect.Height + Math.Cos(sAngle) * szLabelMaxSize.Height), sAngle)
            DrawAngledText(g, ftScale, astrLabels(i), _
                 CInt(i * szCellSize.Width), _
                 CInt(rect.Height + Math.Cos(sAngle) * szLabelMaxSize.Height), sAngle)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="ftScale">Font to render scales with.</param>
    ''' <param name="ftSubtitle">Font to render subtitles with.</param>
    ''' <param name="rect"></param>
    ''' <param name="szCellSize">Size of a cell</param>
    ''' <param name="strLegend"></param>
    ''' <param name="astrLabels"></param>
    ''' <param name="sAngle">Text rotation angle.</param>
    ''' -----------------------------------------------------------------------
    Private Sub DrawLabelsSide(ByVal g As Graphics, ByVal ftScale As Font, ByVal ftSubtitle As Font, _
                               ByVal rect As Rectangle, ByVal szCellSize As Size, _
                               ByVal strLegend As String, ByVal astrLabels As String(), _
                               Optional ByVal sAngle As Single = 0.0!)
        Dim szLegendSide As Size = Me.CalcLegendMaxSize(g, ftScale, strLegend)

        DrawAngledText(g, ftScale, strLegend, rect.Width - szLegendSide.Height + rect.X, _
                       CInt(rect.Height - (rect.Height - szLegendSide.Width) / 2) + rect.Y, _
                       -90)

        For i As Integer = 0 To astrLabels.GetUpperBound(0)
            g.DrawString(astrLabels(i), ftScale, SystemBrushes.WindowText, _
                         rect.X, i * szCellSize.Height + rect.Y, StringFormat.GenericDefault)
        Next
    End Sub

    Private Sub DrawLegends(ByVal g As Graphics, ByVal ft As Font, ByVal rect As Rectangle, _
                            ByVal szCellSize As Size, _
                            ByVal astrLegends As String(), _
                            ByVal style As eRenderStyle, _
                            Optional ByVal sAngle As Single = 0.0!)

        For i As Integer = 0 To astrLegends.GetUpperBound(0)
            ' Area to render a single circle into
            Dim rcItem As Rectangle = Nothing

            'Render circle
            rcItem = New Rectangle(rect.X, _
                                     CInt(rect.Height / 2 + 1.5 * i * szCellSize.Height), _
                                     CInt(1 * szCellSize.Width), _
                                     CInt(1 * szCellSize.Height))

            Select Case style
                Case eRenderStyle.Bars
                    g.DrawRectangle(Pens.Black, rcItem)
                    If i = 1 Then g.FillRectangle(Brushes.Black, rcItem)
                Case eRenderStyle.Circles
                    g.DrawEllipse(Pens.Black, rcItem)
                    If i = 1 Then g.FillEllipse(Brushes.Black, rcItem)
            End Select

            'Render legend
            g.DrawString(astrLegends(i), ft, SystemBrushes.WindowText, _
                         rect.X + CInt(2 * szCellSize.Width), _
                         CInt(rect.Height / 2 + 1.5 * i * szCellSize.Height), _
                         StringFormat.GenericDefault)
        Next
    End Sub

    Private Sub DrawGrid(ByVal g As Graphics, _
                          ByVal rect As Rectangle, _
                          ByVal szCellSize As Size, _
                          ByVal asData As Single(,))
        For y As Integer = 0 To asData.GetUpperBound(1)
            g.DrawLine(Pens.LightGray, _
                       rect.X, rect.Y + CInt(y * szCellSize.Height), _
                       rect.X + rect.Width, rect.Y + CInt(y * szCellSize.Height))
        Next y
        For x As Integer = 0 To asData.GetUpperBound(0)
            g.DrawLine(Pens.LightGray, _
                       rect.X + CInt(x * szCellSize.Width), rect.Y, _
                       rect.X + CInt(x * szCellSize.Width), rect.Y + rect.Height)
        Next x
    End Sub

    Private Sub DrawGraph(ByVal g As Graphics, _
                          ByVal rect As Rectangle, _
                          ByVal szCellSize As Size, _
                          ByVal asData As Single(,), _
                          ByVal style As eRenderStyle)

        ' Normalize the data
        asData = Me.GetNormalizedArray(asData)

        ' Render the graph
        For y As Integer = 0 To asData.GetUpperBound(1)

            Select Case style
                Case eRenderStyle.Circles
                    ' NOP
                Case eRenderStyle.Bars
                    g.DrawLine(Pens.Black, rect.X, rect.Y + CInt((y + 0.5) * szCellSize.Height), _
                               rect.X + rect.Width, rect.Y + CInt((y + 0.5) * szCellSize.Height))

            End Select

            For x As Integer = 0 To asData.GetUpperBound(0)
                Select Case style

                    Case eRenderStyle.Circles
                        Dim rcCircle As New Rectangle(rect.X + CInt(x * szCellSize.Width + (szCellSize.Width - CInt(asData(x, y) * szCellSize.Width)) / 2), _
                                                 rect.Y + CInt(y * szCellSize.Height + (szCellSize.Height - CInt(asData(x, y) * szCellSize.Height)) / 2), _
                                                 CInt(asData(x, y) * szCellSize.Width), _
                                                 CInt(asData(x, y) * szCellSize.Height))
                        If asData(x, y) > 0.0 Then
                            g.FillEllipse(Brushes.Black, rcCircle)
                        Else
                            g.FillEllipse(Brushes.White, rcCircle)
                        End If
                        g.DrawEllipse(Pens.Black, rcCircle)

                    Case eRenderStyle.Bars
                        Dim iBarHeight As Integer = Math.Abs(CInt(asData(x, y) * szCellSize.Height / 2))
                        Dim rcRect As New Rectangle(rect.X + CInt(x * szCellSize.Width), _
                                                    rect.Y + CInt((y + 0.5) * szCellSize.Height), _
                                                    CInt(szCellSize.Width), _
                                                    iBarHeight)
                        If asData(x, y) > 0.0 Then
                            rcRect.Y -= iBarHeight
                            g.FillRectangle(Brushes.White, rcRect)
                        Else
                            g.FillRectangle(Brushes.Black, rcRect)
                        End If
                        g.DrawRectangle(Pens.Black, rcRect)

                End Select
            Next x
        Next y

    End Sub

    Private Sub DrawAngledText(ByVal g As Graphics, ByVal ft As Font, _
            ByVal strLabel As String, ByVal x As Integer, ByVal y As Integer, _
            ByVal sAngleDegrees As Single)

        g.TranslateTransform(x, y)
        g.RotateTransform(sAngleDegrees)
        g.DrawString(strLabel, ft, SystemBrushes.WindowText, 0, 0)
        g.ResetTransform()

    End Sub

#End Region ' Rendering

#End Region ' Internals

End Class
