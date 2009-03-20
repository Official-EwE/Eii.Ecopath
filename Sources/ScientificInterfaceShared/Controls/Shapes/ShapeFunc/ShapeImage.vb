'==============================================================================
'
' $Log: ShapeImage.vb,v $
' Revision 1.3  2009/03/20 17:50:12  jeroens
' Removed warning icon
'
' Revision 1.2  2009/02/12 15:32:20  jeroens
' Can add labels to XMark, YMark lines
'
' Revision 1.1  2008/12/15 15:36:39  jeroens
' Moved from ScInt
'
' Revision 1.3  2008/10/08 17:45:15  jeroens
' Sanity check on sYMax
'
' Revision 1.2  2008/10/01 16:50:29  villyc
' Ecosim monte carlo updates, plus ecosim plot bug fix
'
' Revision 1.1  2008/09/26 07:31:41  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Globalization
Imports System.Threading
Imports EwEUtils.Core
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ShapeImage

        Public Const cDOT_SIZE As Integer = 6
        Public Const cICON_WIDTH As Integer = 48
        Public Const cICON_HEIGHT As Integer = 48

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' This helper method converts the coordinates of model point to those of the image point
        ''' </summary>
        ''' <param name="ptModel">Data point to convert</param>
        ''' <param name="rcClip">Clip rectangle to convert point to.</param>
        ''' <param name="sXMax">Clip rectangle horz. axis corresponds to [0, sxMax].</param>
        ''' <param name="sYMax">Clip rectangle vert. axis corresponds to [0, syMax].</param>
        ''' <returns>A point in the clip rectangle that corresponds to ptModel.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function ToImagePoint(ByVal ptModel As PointF, _
                                    ByVal rcClip As Rectangle, _
                                    ByVal sXMax As Single, ByVal sYMax As Single) As PointF

            Dim ptImage As PointF = Nothing

            ' Division by zero prevention
            If (sXMax = 0.0!) Then sXMax = 1.0!
            If (sYMax = 0.0!) Then sYMax = 1.0!

            ptImage = New PointF(ptModel.X * rcClip.Width / sXMax, rcClip.Height - ptModel.Y * rcClip.Height / sYMax)
            Return ptImage

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' This helper method transforms the underlying point value to the screen point value
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Function ToScreenPoint(ByVal ptModel As PointF, _
                                    ByVal rcClip As Rectangle, _
                                    ByVal sXMax As Single, ByVal sYMax As Single) As PointF

            Dim ptScreen As New PointF(ptModel.X * rcClip.Width / sXMax + rcClip.Left, _
                            rcClip.Height + rcClip.Top - ptModel.Y * rcClip.Height / sYMax)
            Return ptScreen

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' This method transforms the screen point to the underlying model point
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Shared Function ToModelPoint(ByVal ptImage As PointF, _
                                    ByVal rcClip As Rectangle, _
                                    ByVal sXMax As Single, ByVal sYMax As Single) As PointF

            Dim ptModel As New PointF(CInt(Math.Ceiling((ptImage.X - rcClip.Left) * sXMax / rcClip.Width)), _
                                (rcClip.Height + rcClip.Top - ptImage.Y) * sYMax / rcClip.Height)

            ptModel.X = Math.Min(Math.Max(0, ptModel.X), sXMax)
            ptModel.Y = Math.Max(0, ptModel.Y)

            Return ptModel

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Draws a <see cref="cForcingFunction">Forcing Function</see>.
        ''' </summary>
        ''' <param name="shape">The shape to draw.</param>
        ''' <param name="rcImage">The dimensions of the area to render the shape onto.</param>
        ''' <param name="g">The graphics to draw the image onto.</param>
        ''' <param name="clr">The colour to use rendering the image.</param>
        ''' <param name="drawMode">The <see cref="eSketchDrawModeTypes">mode</see> to render the shape with.</param>
        ''' <param name="sYMax">The max Y value to scale the shape to.</param>
        ''' -------------------------------------------------------------------
        Public Shared Sub DrawShape(ByVal shape As cShapeData, _
                                ByVal rcImage As Rectangle, _
                                ByVal g As Graphics, _
                                ByVal clr As Color, _
                                ByVal drawMode As eSketchDrawModeTypes, _
                                Optional ByVal sYMax As Single = cCore.NULL_VALUE, _
                                Optional ByVal sYMark As Single = cCore.NULL_VALUE, _
                                Optional ByVal sXMark As Single = cCore.NULL_VALUE, _
                                Optional ByVal strYMarkLabel As String = "", _
                                Optional ByVal strXMarkLabel As String = "")

            If shape Is Nothing Then Return
            If (sYMax = cCore.NULL_VALUE) Then sYMax = shape.YMax(True)
            If (sYMark = cCore.NULL_VALUE) Then sYMark = CSng(IIf(shape.DataType = eDataTypes.Mediation, 0.5!, 1.0!))

            ShapeImage.DrawShapeDirect(shape.ShapeData, shape.XMax, shape.IsSeasonal, _
                    rcImage, g, clr, _
                    drawMode, _
                    sYMax, _
                    sYMark, sXMark, strYMarkLabel, strXMarkLabel)

        End Sub

        Public Shared Sub DrawShapeDirect(ByVal asData As Single(), ByVal nPoints As Integer, ByVal bIsSeasonal As Boolean, _
                                ByVal rcImage As Rectangle, _
                                ByVal g As Graphics, _
                                ByVal clr As Color, _
                                ByVal drawMode As eSketchDrawModeTypes, _
                                ByVal sYMax As Single, _
                                ByVal sYMark As Single, _
                                ByVal sXMark As Single, _
                                Optional ByVal strYMarkLabel As String = "", _
                                Optional ByVal strXMarkLabel As String = "")


            Dim brShape As New SolidBrush(clr)
            Dim pnShape As New Pen(clr, 1)
            Dim sg As StyleGuide = StyleGuide.GetInstance()

            ' No max specified? Calc it.
            If (sYMax <> sYMax) Then Return

            Select Case drawMode

                Case eSketchDrawModeTypes.Line, eSketchDrawModeTypes.Fill

                    Dim gp As New GraphicsPath
                    Dim pt1 As PointF = Nothing
                    Dim pt2 As PointF = Nothing

                    If bIsSeasonal Then

                        nPoints = cCore.N_MONTHS

                        pt2 = ShapeImage.ToImagePoint(New PointF(0, 0), rcImage, nPoints, sYMax)
                        For i As Integer = 1 To nPoints
                            pt1 = pt2
                            pt2 = ShapeImage.ToImagePoint(New PointF(i - 1.0!, asData(i)), rcImage, nPoints, sYMax)
                            gp.AddLine(pt1, pt2)

                            pt1 = pt2
                            pt2 = ShapeImage.ToImagePoint(New PointF(i, asData(i)), rcImage, nPoints, sYMax)
                            gp.AddLine(pt1, pt2)
                        Next

                        pt1 = pt2
                        pt2 = ShapeImage.ToImagePoint(New PointF(13.0!, 0), rcImage, nPoints, sYMax)
                        gp.AddLine(pt1, pt2)

                    Else

                        pt2 = ShapeImage.ToImagePoint(New PointF(0, 0), rcImage, nPoints, sYMax)
                        For i As Integer = 1 To nPoints
                            pt1 = pt2
                            pt2 = ShapeImage.ToImagePoint(New PointF(i - 1, asData(i)), rcImage, nPoints, sYMax)
                            gp.AddLine(pt1, pt2)
                        Next
                        pt1 = pt2
                        pt2 = ShapeImage.ToImagePoint(New PointF(nPoints, 0), rcImage, nPoints, sYMax)
                        gp.AddLine(pt1, pt2)

                    End If

                    Try
                        Select Case drawMode
                            Case eSketchDrawModeTypes.Line
                                g.DrawPath(pnShape, gp)
                            Case eSketchDrawModeTypes.Fill
                                g.FillPath(brShape, gp)
                            Case Else
                                Debug.Assert(False)
                        End Select
                    Catch ex As Exception

                    End Try

                    gp.Dispose()

                Case eSketchDrawModeTypes.Dots

                    Dim pt As PointF = Nothing

                    If bIsSeasonal Then

                        nPoints = cCore.N_MONTHS

                        For i As Integer = 1 To nPoints
                            If asData(i) > 0.0! Then
                                pt = ShapeImage.ToImagePoint(New PointF(i - 0.5!, asData(i)), rcImage, nPoints, sYMax)
                                g.FillEllipse(brShape, _
                                        CSng(pt.X - cDOT_SIZE / 2), CSng(pt.Y - cDOT_SIZE / 2), _
                                        CSng(cDOT_SIZE), CSng(cDOT_SIZE))
                            End If
                        Next

                    Else
                        For i As Integer = 1 To nPoints
                            If asData(i) > 0.0! Then
                                pt = ShapeImage.ToImagePoint(New PointF(i - 1.0!, asData(i)), rcImage, nPoints, sYMax)
                                g.FillEllipse(brShape, _
                                        CSng(pt.X - cDOT_SIZE / 2), CSng(pt.Y - cDOT_SIZE / 2), _
                                        CSng(cDOT_SIZE), CSng(cDOT_SIZE))
                            End If
                        Next
                    End If

                Case eSketchDrawModeTypes.LineSelective

                    Dim pt1 As PointF = Nothing
                    Dim pt2 As PointF = Nothing
                    Dim iNumPoints As Integer = 0

                    If bIsSeasonal Then
                        For i As Integer = 1 To 12
                            If asData(i) > 0.0! Then
                                pt1 = ShapeImage.ToImagePoint(New PointF(i - 0.5!, asData(i)), rcImage, nPoints, sYMax)
                                g.FillEllipse(brShape, _
                                        CSng(pt1.X - cDOT_SIZE / 2), CSng(pt1.Y - cDOT_SIZE / 2), _
                                        CSng(cDOT_SIZE), CSng(cDOT_SIZE))
                            End If
                        Next
                    Else
                        For i As Integer = 1 To nPoints
                            If asData(i) > 0.0! Then
                                pt2 = pt1
                                pt1 = ShapeImage.ToImagePoint(New PointF(i - 1.0!, asData(i)), rcImage, nPoints, sYMax)
                                iNumPoints += 1

                                If (iNumPoints >= 2) Then g.DrawLine(pnShape, pt1, pt2)
                            Else
                                ' Only one point last found?
                                If (iNumPoints = 1) Then
                                    ' #Yes: render this point
                                    g.DrawLine(pnShape, pt1.X, pt1.Y - 1, pt1.X, pt1.Y)
                                End If
                                iNumPoints = 0
                            End If
                        Next

                    End If

            End Select

            ' Draw YMark
            If (sYMark > 0) Then
                Try
                    Dim ptfFrom As PointF = ShapeImage.ToImagePoint(New PointF(0, sYMark), rcImage, nPoints, sYMax)
                    Dim ptfTo As PointF = ShapeImage.ToImagePoint(New PointF(nPoints, sYMark), rcImage, nPoints, sYMax)

                    g.DrawLine(Pens.Gray, ptfFrom, ptfTo)

                    ' Draw Ymark label, if any
                    If Not String.IsNullOrEmpty(strYMarkLabel) Then
                        Using ft As New Font(sg.GraphFontFamilyName, sg.GraphAxisScaleFontSize, sg.GraphAxisLabelFontStyle)
                            Using br As New SolidBrush(sg.ApplicationColor(StyleGuide.eApplicationColorType.DEFAULT_TEXT))
                                ' Position label on the right end of the graph
                                ptfTo.X -= g.MeasureString(strYMarkLabel, ft).Width
                                g.DrawString(strYMarkLabel, ft, br, ptfTo)
                            End Using
                        End Using
                    End If

                Catch ex As Exception
                    ' Error drawing a point out of range
                End Try
            End If

            ' Draw axis
            g.DrawLine(Pens.Gray, New PointF(0, 0), New PointF(0, rcImage.Height))
            g.DrawLine(Pens.Gray, New PointF(0, rcImage.Height), New PointF(rcImage.Width, rcImage.Height))

            ' Draw XMark
            If (sXMark > 0) Then
                Using p As New Pen(Color.Blue, 1)

                    Dim ptfTmp As PointF = ShapeImage.ToImagePoint(New PointF(sXMark, 0), rcImage, nPoints, sYMax)
                    Dim ptfFrom As New PointF(ptfTmp.X, 0)
                    Dim ptfTo As New PointF(ptfTmp.X, rcImage.Height)

                    p.DashStyle = DashStyle.Dash
                    g.DrawLine(p, ptfFrom, ptfTo)

                    ' Draw Xmark label, if any
                    If Not String.IsNullOrEmpty(strXMarkLabel) Then
                        Using ft As New Font(sg.GraphFontFamilyName, sg.GraphAxisScaleFontSize, sg.GraphAxisLabelFontStyle)
                            Using br As New SolidBrush(Color.Blue)
                                Dim szfText As SizeF = g.MeasureString(strXMarkLabel, ft)
                                ' Position label on the top of the graph, left of the line
                                ptfFrom.X -= szfText.Width
                                g.DrawString(strXMarkLabel, ft, br, ptfFrom)
                            End Using
                        End Using
                    End If

                End Using
            End If

            pnShape.Dispose()
            brShape.Dispose()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Return a thumbnail image of a given shape.
        ''' </summary>
        ''' <param name="shape">
        ''' The shape to obtain an image for. If this parameter is not specified,
        ''' a thumbnail image will be returned for the current shape.
        ''' </param>
        ''' <param name="clr">Colour to render the thumbnail image with.</param>
        ''' <param name="sYMax">Y-scale to use for rendering the image.</param>
        ''' <param name="bShowWarning">Flag stating whether a warning icon
        ''' should be displayed in the lower left corner of the shape
        ''' (or lower right, depending on locale reading order).</param>
        ''' -------------------------------------------------------------------
        Public Shared Function IconImage(ByVal shape As cShapeData, _
                ByVal clr As Color, _
                Optional ByVal sYMax As Single = cCore.NULL_VALUE, _
                Optional ByVal bShowWarning As Boolean = False) As System.Drawing.Image

            Dim ci As CultureInfo = Nothing
            Dim dm As eSketchDrawModeTypes = eSketchDrawModeTypes.Line
            Dim bmp As New Bitmap(cICON_WIDTH, cICON_HEIGHT)
            Dim g As Graphics = Graphics.FromImage(bmp)
            Dim img As Image = Nothing

            ' JS 06sep07: pragmatic hack, this belongs elsewhere
            If TypeOf shape Is cTimeSeries Then dm = eSketchDrawModeTypes.LineSelective : sYMax = shape.YMax

            Try
                DrawShape(shape, New Rectangle(New Point(0, 0), bmp.Size), g, clr, dm, sYMax, cCore.NULL_VALUE)
            Catch ex As Exception
                ' Draw error image
                g.FillRectangle(Brushes.White, New Rectangle(New Point(0, 0), bmp.Size))
                g.DrawLine(Pens.Red, 0, 0, bmp.Width, bmp.Height)
                g.DrawLine(Pens.Red, 0, bmp.Height, bmp.Width, 0)
            End Try

            ' Draw warning icon, if neccessary
            If bShowWarning Then
                ' Try to get image from resources
                img = My.Resources.WarningHS
                If img IsNot Nothing Then
                    ' Get current locale info to see whether image should be drawn on left or right lower corner
                    ci = Thread.CurrentThread.CurrentUICulture
                    If ci.TextInfo.IsRightToLeft Then
                        ' RtoL reading order: draw image in lower right corner
                        g.DrawImage(img, bmp.Width - img.Width - 2, bmp.Height - img.Height - 2)
                    Else
                        ' LtoR reading order: draw image in lower left corner
                        g.DrawImage(img, 1, bmp.Height - img.Height - 2)
                    End If
                End If
            End If

            g.Dispose()
            Return bmp

        End Function

    End Class

End Namespace
