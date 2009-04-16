'==============================================================================
'
' $Log: ArrayGraph.vb,v $
' Revision 1.4  2009/04/16 21:52:36  joeh
' Add Legends to the MTI plot
'
' Revision 1.3  2008/12/05 19:45:20  joeh
' Add 'Legend' to be plotted on top of labels
'
' Revision 1.2  2008/11/28 20:20:44  joeh
' Change the label topaxis angle to 0
'
' Revision 1.1  2008/11/28 01:58:32  joeh
' Implement new MTI plot and save MTI plot as emf file
'
' Revision 1.5  2008/11/24 16:06:13  jeroens
' Header!
'
'==============================================================================

Option Strict On
Option Explicit On
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.ComponentModel

Public Class ArrayGraph

    Private m_font As Font = SystemFonts.DefaultFont
    Private m_nodes As Boolean

    ''' <summary>Spacer between cells, expressed in cell size</summary>
    ''' <remarks></remarks>
    Private Const cCELL_PADDING_RATIO As Single = 0.3333!
    ''' <summary>Angle of top labels, specied in degress, off of the vertical axis</summary>
    Private Const cLABEL_TOPAXIS_ANGLE As Integer = 0 '30

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
    Public Sub Draw(ByVal g As Graphics, ByVal rcRender As Rectangle, _
                    ByVal asData As Single(,), ByVal strTitleX As String, ByVal astrLabelsX As String(), _
                    Optional ByVal strTitleY As String = Nothing, Optional ByVal astrLabelsY As String() = Nothing, _
                    Optional ByVal astrLegends As String() = Nothing)

        ' ToDo: take right-to-left reading order into account
        ' ToDo: allow side label positioning (left or right)

        ' == Fix defaults ==
        ' Use X-axis labels for both axis if Y-axis labels are omitted
        If astrLabelsY Is Nothing Then astrLabelsY = astrLabelsX

        ' == Sanity checks ==
        ' Make sure data and label dimensions fit
        Debug.Assert(asData.GetUpperBound(0) = astrLabelsX.Length - 1, "Data dimension {0} not compatible with X-axis labels")
        Debug.Assert(asData.GetUpperBound(1) = astrLabelsY.Length - 1, "Data dimension not compatible with Y-axis labels")

        ' Measure max label sizes
        Dim szLabelTopMaxSize As Size = Me.CalcLabelMaxSize(g, astrLabelsX)
        Dim szLabelSideMaxSize As Size = Me.CalcLabelMaxSize(g, astrLabelsY)
        Dim szLegendTop As Size = Me.CalcLegendMaxSize(g, strTitleX)
        Dim szLegendSide As Size = Me.CalcLegendMaxSize(g, strTitleY)
        Dim sCellSize As Single = 0.0!

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
        'Dim rcArea1 As Rectangle = New Rectangle(rcRender.X, rcRender.Y, _
        '                                          rcRender.Width - szLabelTopMaxSize.Width, szLabelTopMaxSize.Width)
        'Dim rcArea2 As Rectangle = New Rectangle(rcRender.X, rcRender.Y + szLabelTopMaxSize.Width, _
        '                                         rcRender.Width - szLabelSideMaxSize.Width, rcRender.Height - szLabelTopMaxSize.Width)
        'Dim rcArea3 As Rectangle = New Rectangle(rcRender.Width - szLabelSideMaxSize.Width, szLabelTopMaxSize.Width, _
        '                                         szLabelSideMaxSize.Width, rcRender.Height - szLabelTopMaxSize.Width)
        Dim intArea1Width As Integer = rcRender.Width - szLabelSideMaxSize.Width - szLegendSide.Height * 2
        Dim intArea1Height As Integer = szLabelTopMaxSize.Width + szLegendTop.Height * 2
        Dim intArea3Width As Integer = szLabelSideMaxSize.Width + szLegendSide.Height * 2
        Dim intArea3Height As Integer = rcRender.Height - intArea1Height
        Dim intArea2Width As Integer = rcRender.Width - intArea3Width
        Dim intArea2Height As Integer = rcRender.Height - intArea1Height

        Dim rcArea1 As Rectangle = New Rectangle(rcRender.X, rcRender.Y, _
                                                 intArea1Width, intArea1Height)
        Dim rcArea2 As Rectangle = New Rectangle(rcRender.X, rcRender.Y + intArea1Height, _
                                                 intArea2Width, intArea2Height)
        Dim rcArea3 As Rectangle = New Rectangle(rcRender.Width - intArea3Width, intArea1Height, _
                                                 intArea3Width, intArea3Height)
        Dim rcArea4 As Rectangle = New Rectangle(rcRender.Width - intArea3Width, rcRender.Y, _
                                                 intArea3Width, intArea1Height)

        ' Figure out where to draw the graphs
        sCellSize = CalcGridSize(rcArea2, asData.GetUpperBound(0), asData.GetUpperBound(1))
        ' Top text
        DrawLabelsTop(g, rcArea1, sCellSize, strTitleX, astrLabelsX, szLabelTopMaxSize, -90 + cLABEL_TOPAXIS_ANGLE)
        ' Side text
        DrawLabelsSide(g, rcArea3, sCellSize, strTitleY, astrLabelsY)
        ' Graph legends
        DrawLegends(g, rcArea4, sCellSize, astrLegends)
        ' Graph
        DrawGraph(g, rcArea2, sCellSize, asData)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Calculate square grid cell size.
    ''' </summary>
    ''' <param name="rect"></param>
    ''' <param name="iNumItemsOnXAxis"></param>
    ''' <param name="iNumItemsOnYAxis"></param>
    ''' <returns>Size of a single square grid cell (in pixels).</returns>
    ''' -----------------------------------------------------------------------
    Private Function CalcGridSize(ByVal rect As Rectangle, ByVal iNumItemsOnXAxis As Integer, ByVal iNumItemsOnYAxis As Integer) As Single

        Return CInt(Math.Min(rect.Width / (iNumItemsOnXAxis + (1 / cCELL_PADDING_RATIO)), _
                             rect.Height / (iNumItemsOnYAxis + (1 / cCELL_PADDING_RATIO))))

    End Function

#Region " Properties "

    Public Property Font() As Font
        Get
            Return Me.m_font
        End Get
        Set(ByVal value As Font)
            Me.m_font = value
        End Set
    End Property

#End Region ' Properties

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
    Private Function CalcLabelMaxSize(ByVal g As Graphics, ByVal astrLabels As String()) As Size

        Dim szMax As New Size(0, 0)
        Dim szfLabel As SizeF = Nothing

        For Each strLabel As String In astrLabels
            szfLabel = g.MeasureString(strLabel, Me.Font)
            szMax.Width = Math.Max(szMax.Width, CInt(Math.Ceiling(szfLabel.Width)))
            szMax.Height = Math.Max(szMax.Height, CInt(Math.Ceiling(szfLabel.Height)))
        Next

        Return szMax
    End Function

    Private Function CalcLegendMaxSize(ByVal g As Graphics, ByVal strLegend As String) As Size

        Dim szLegend As New Size(0, 0)
        Dim szfLegend As SizeF = Nothing

        szfLegend = g.MeasureString(strLegend, Me.Font)
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
    ''' <param name="rect"></param>
    ''' <param name="sCellSize">Size of a square cell</param>
    ''' <param name="strLegend"></param>
    ''' <param name="astrLabels"></param>
    ''' <param name="szLabelMaxSize">Max dimensions (in pixels) of a label.</param>
    ''' <param name="sAngle">Text rotation angle.</param>
    ''' -----------------------------------------------------------------------
    Private Sub DrawLabelsTop(ByVal g As Graphics, ByVal rect As Rectangle, ByVal sCellSize As Single, _
                              ByVal strLegend As String, ByVal astrLabels As String(), ByVal szLabelMaxSize As Size, _
                              Optional ByVal sAngle As Single = 0.0!)
        Dim szLegendTop As Size = Me.CalcLegendMaxSize(g, strLegend)

        g.DrawString(strLegend, Me.Font, SystemBrushes.WindowText, _
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
        '    ^,2
        '
        ' Here, pt (1) is the top-left origin at which the label will be drawn. However, point (2), the
        ' bottom-left corner of the label, is rotated below the origin. (2) should thus move up to ensure
        ' that the label is not rendered inside the graph area. Therefore, the Y-position of the label
        ' must be moved by {szLabelMaxSize.height} * Math.Cos(sAngle)

        For i As Integer = 0 To astrLabels.GetUpperBound(0)
            'DrawAngledText(g, astrLabels(i), _
            '            CInt(i * sCellSize + sCellSize * cCELL_PADDING_RATIO), _
            '            CInt(rect.Height + Math.Cos(sAngle) * szLabelMaxSize.Height), sAngle)
            DrawAngledText(g, astrLabels(i), _
                 CInt(i * sCellSize), _
                 CInt(rect.Height + Math.Cos(sAngle) * szLabelMaxSize.Height), sAngle)
        Next

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="g"></param>
    ''' <param name="rect"></param>
    ''' <param name="sCellSize">Size of a square cell</param>
    ''' <param name="strLegend"></param>
    ''' <param name="astrLabels"></param>
    ''' <param name="sAngle">Text rotation angle.</param>
    ''' -----------------------------------------------------------------------
    Private Sub DrawLabelsSide(ByVal g As Graphics, ByVal rect As Rectangle, ByVal sCellSize As Single, _
                               ByVal strLegend As String, ByVal astrLabels As String(), _
                               Optional ByVal sAngle As Single = 0.0!)
        Dim szLegendSide As Size = Me.CalcLegendMaxSize(g, strLegend)

        DrawAngledText(g, strLegend, rect.Width - szLegendSide.Height + rect.X, _
                       CInt(rect.Height - (rect.Height - szLegendSide.Width) / 2) + rect.Y, _
                       -90)

        For i As Integer = 0 To astrLabels.GetUpperBound(0)
            'g.DrawString(astrLabels(i), Me.Font, SystemBrushes.WindowText, _
            '             rect.X, CInt(i * sCellSize + sCellSize * cCELL_PADDING_RATIO) + rect.Y, StringFormat.GenericDefault)
            g.DrawString(astrLabels(i), Me.Font, SystemBrushes.WindowText, _
                        rect.X, i * sCellSize + rect.Y, StringFormat.GenericDefault)
        Next
    End Sub

    Private Sub DrawLegends(ByVal g As Graphics, ByVal rect As Rectangle, ByVal sCellSize As Single, _
                            ByVal astrLegends As String(), Optional ByVal sAngle As Single = 0.0!)
        For i As Integer = 0 To astrLegends.GetUpperBound(0)
            ' Area to render a single circle into
            Dim rcCircle As Rectangle = Nothing

            'Render circle
            'CInt(rect.Height / 2 + 1.5 * i * sCellSize) gives half cell size between legends
            rcCircle = New Rectangle(rect.X, _
                                     CInt(rect.Height / 2 + 1.5 * i * sCellSize), _
                                     CInt(1 * sCellSize), _
                                     CInt(1 * sCellSize))
            g.DrawEllipse(Pens.Black, rcCircle)
            If i = 0 Then g.FillEllipse(Brushes.Black, rcCircle)

            'Render legend
            g.DrawString(astrLegends(i), Me.Font, SystemBrushes.WindowText, _
                         rect.X + CInt(2 * sCellSize), _
                         CInt(rect.Height / 2 + 1.5 * i * sCellSize), _
                         StringFormat.GenericDefault)
        Next
    End Sub

    Private Sub DrawGraph(ByVal g As Graphics, ByVal rect As Rectangle, ByVal sCellSize As Single, ByVal asData As Single(,))

        ' Area to render a single circle into
        Dim rcCircle As Rectangle = Nothing

        ' Normalize the data
        asData = Me.GetNormalizedArray(asData)

        ' Render the graph
        For x As Integer = 0 To asData.GetUpperBound(0)
            For y As Integer = 0 To asData.GetUpperBound(1)
                rcCircle = New Rectangle(rect.X + CInt(x * sCellSize + (sCellSize - CInt(asData(x, y) * sCellSize)) / 2), _
                                         rect.Y + CInt(y * sCellSize + (sCellSize - CInt(asData(x, y) * sCellSize)) / 2), _
                                         CInt(asData(x, y) * sCellSize), _
                                         CInt(asData(x, y) * sCellSize))
                g.DrawEllipse(Pens.Black, rcCircle)
                If asData(x, y) > 0.0 Then g.FillEllipse(Brushes.Black, rcCircle)
            Next y
        Next x

    End Sub

    Private Sub DrawAngledText(ByVal g As Graphics, _
            ByVal strLabel As String, ByVal x As Integer, ByVal y As Integer, _
            ByVal sAngleDegrees As Single)

        g.TranslateTransform(x, y)
        g.RotateTransform(sAngleDegrees)
        g.DrawString(strLabel, Me.Font, SystemBrushes.WindowText, 0, 0)
        g.ResetTransform()

    End Sub

#End Region ' Rendering

#End Region ' Internals

End Class
