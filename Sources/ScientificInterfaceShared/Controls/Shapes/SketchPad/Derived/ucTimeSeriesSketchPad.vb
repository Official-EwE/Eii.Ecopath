'==============================================================================
'
' $Log: ucTimeSeriesSketchPad.vb,v $
' Revision 1.2  2009/02/12 15:32:21  jeroens
' Can add labels to XMark, YMark lines
'
' Revision 1.1  2008/12/15 15:36:40  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:45  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region

Namespace Controls

    Public Class ucTimeSeriesSketchPad

        Private m_AxisYMarks As eAxisTickmarkDisplayModeTypes

        Public WriteOnly Property AxisXMark() As eAxisTickmarkDisplayModeTypes
            Set(ByVal value As eAxisTickmarkDisplayModeTypes)
                m_AxisYMarks = value
            End Set
        End Property

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            Me.m_SketchDrawMode = eSketchDrawModeTypes.Dots

            'Default display as Absolute value
            m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Absolute

        End Sub

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            Dim strLabel As String = ""
            Dim fmt As StringFormat = Nothing
            Dim sg As StyleGuide = StyleGuide.GetInstance()

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            If Me.Shape Is Nothing Then Return

            'Draw the line with y's value equal to 1
            ' JS 30Jan08: Only draw this line if it is going to be visible
            If (sYMax >= 1.0) Then
                g.DrawLine(Pens.Black, _
                    ShapeImage.ToImagePoint(New PointF(0, 1), Me.ClientRectangle, Me.Shape.XMax, sYMax), _
                    ShapeImage.ToImagePoint(New PointF(Me.Shape.XMax, 1), Me.ClientRectangle, Me.Shape.XMax, sYMax))
            End If

            ' Draw the axis when this mode is on
            If m_AxisDisplayMode = eAxisDisplayModeTypes.Show Then

                ' Draw Axis
                g.DrawLine(Pens.Gray, New PointF(rcImage.Left, rcImage.Bottom), New PointF(rcImage.Right, rcImage.Bottom))
                g.DrawLine(Pens.Gray, New PointF(rcImage.Left, rcImage.Top), New PointF(rcImage.Left, rcImage.Bottom))

                ' Draw Axis X marks
                Dim astrXMarks As String() = GetAxisX()
                fmt = New StringFormat
                fmt.Alignment = StringAlignment.Center
                fmt.LineAlignment = StringAlignment.Center

                Dim brTmp As New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                Dim penTmp As New Pen(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                Dim sLabelXPos As Single = 0.0
                Dim tmpFont As New Font(sg.GraphFontFamilyName, sg.GraphAxisScaleFontSize)
                Dim sBtnSpace As Single = tmpFont.Height

                For i As Integer = 0 To astrXMarks.Length - 1
                    If Me.Shape.IsSeasonal Then
                        sLabelXPos = CSng((i + 0.5!) * rcImage.Width / Math.Max(1, astrXMarks.Length - 1))
                    Else
                        sLabelXPos = CSng(i * rcImage.Width / Math.Max(1, astrXMarks.Length - 1))
                    End If
                    g.DrawString(astrXMarks(i), Me.Font, brTmp, _
                            rcImage.Left + sLabelXPos, rcImage.Bottom - sBtnSpace, fmt)
                    g.DrawLine(penTmp, rcImage.Left + sLabelXPos, _
                            rcImage.Bottom, rcImage.Left + sLabelXPos, rcImage.Bottom - sBtnSpace / 2)
                Next

                Dim yStep As Integer
                If CInt(sYMax / 10) = 0 Then
                    yStep = 1
                Else
                    yStep = CInt(10 ^ (CStr(CInt(sYMax)).Length - 1)) * 2
                End If

                For j As Double = 0 To sYMax Step yStep * 0.5
                    Dim yPos As Integer = CInt(rcImage.Bottom - rcImage.Height * j / sYMax)
                    If m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Relative Then
                        ' ToDo_JS: Localize this
                        strLabel = String.Format("x{0}", j)
                    Else
                        strLabel = String.Format("{0}", j)
                    End If
                    g.DrawString(strLabel, Me.Font, brTmp, rcImage.Left + 5, yPos)
                    g.DrawLine(penTmp, rcImage.Left, yPos, rcImage.Left + 3, yPos)
                Next

                ' AAAAAAAAAAAAAAARGH!
                If sYMax < 0.5 And sYMax >= 0.01 Then
                    For j As Integer = 0 To 2
                        Dim yPos As Integer = CInt(rcImage.Bottom - rcImage.Height * (3 - j) / 3)

                        strLabel = sg.FormatNumber(sYMax * (3 - j) / 3)
                        If m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Relative Then
                            strLabel = String.Format("x{0}", strLabel)
                        End If

                        g.DrawString(strLabel, Me.Font, brTmp, rcImage.Left + 5, yPos)
                        g.DrawLine(penTmp, rcImage.Left, yPos, rcImage.Left + 3, yPos)
                    Next
                End If

                strLabel = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, Me.Shape.Index, Me.Shape.Name)
                g.DrawString(strLabel, tmpFont, brTmp, CSng(rcImage.Width / 2), rcImage.Top + 15, fmt)

                ' Dispose the pen, brush and font we created and let the system garbage collect them.
                brTmp.Dispose()
                penTmp.Dispose()
                tmpFont.Dispose()

            End If

        End Sub

        ''' <summary>
        ''' This method returns the marks displayed on the Axis X
        ''' </summary>
        Private Function GetAxisX() As String()

            Dim core As cCore = cCore.GetInstance
            Dim ds As cTimeSeriesDataset = core.TimeSeriesDataset(core.ActiveTimeSeriesDatasetIndex)
            Dim lstrAxis As New List(Of String)

            If Me.Shape Is Nothing Then Return lstrAxis.ToArray()
            If Not (TypeOf Me.Shape Is cTimeSeries) Then Return lstrAxis.ToArray()

            If Not Me.IsSeasonal Then

                Dim iTSFinalYear As Integer
                Dim iStepSize As Integer
                Dim ts As cTimeSeries = DirectCast(Me.Shape, cTimeSeries)

                iTSFinalYear = ds.FirstYear + Me.Shape.XMax
                iStepSize = Math.Max(1, CInt((iTSFinalYear - ds.FirstYear) / 10))
                For i As Integer = ds.FirstYear To iTSFinalYear Step iStepSize
                    lstrAxis.Add(i.ToString)
                Next
                Return lstrAxis.ToArray()
            Else
                Return New String() {My.Resources.GENERIC_MONTH_ABBR_JANUARY, _
                                    My.Resources.GENERIC_MONTH_ABBR_FEBRUARY, _
                                    My.Resources.GENERIC_MONTH_ABBR_MARCH, _
                                    My.Resources.GENERIC_MONTH_ABBR_APRIL, _
                                    My.Resources.GENERIC_MONTH_ABBR_MAY, _
                                    My.Resources.GENERIC_MONTH_ABBR_JUNE, _
                                    My.Resources.GENERIC_MONTH_ABBR_JULY, _
                                    My.Resources.GENERIC_MONTH_ABBR_AUGUST, _
                                    My.Resources.GENERIC_MONTH_ABBR_SEPTEMBER, _
                                    My.Resources.GENERIC_MONTH_ABBR_OCTOBER, _
                                    My.Resources.GENERIC_MONTH_ABBR_NOVEMBER, _
                                    My.Resources.GENERIC_MONTH_ABBR_DECEMBER, _
                                    ""} ' Hack: one extra to center labels under value ranges
            End If

        End Function

        Protected Overrides Sub OnShapeChanged()
            MyBase.OnShapeChanged()
        End Sub

        Private Sub ForcingSketchPad_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
            Me.OnShapeChanged()
        End Sub

    End Class

End Namespace
