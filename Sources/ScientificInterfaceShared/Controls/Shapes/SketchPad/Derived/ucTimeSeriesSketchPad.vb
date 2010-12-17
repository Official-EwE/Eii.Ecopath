#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Utilities

#End Region

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control, implements a control for viewing (not sketchng) Ecosim
    ''' time series shapes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucTimeSeriesSketchPad
        Implements IUIElement

        Private m_AxisYMarks As eAxisTickmarkDisplayModeTypes

        Public Sub New()

            Me.InitializeComponent()
            Me.m_sketchDrawMode = eSketchDrawModeTypes.Dots
            Me.m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Absolute

        End Sub

        Public WriteOnly Property AxisXMark() As eAxisTickmarkDisplayModeTypes
            Set(ByVal value As eAxisTickmarkDisplayModeTypes)
                m_AxisYMarks = value
            End Set
        End Property

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            ' ToDo: Globalize this method

            If Me.UIContext Is Nothing Then Return

            Dim sg As cStyleGuide = Me.UIContext.StyleGuide
            Dim core As cCore = Me.UIContext.Core
            Dim strType As String = ""
            Dim strName As String = ""
            Dim strLabel As String = ""
            Dim fmt As StringFormat = Nothing

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
            If Me.m_bShowAxis Then

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
                Dim tmpFont As Font = sg.Font(cStyleGuide.eApplicationFontType.Scale)
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
                        strLabel = String.Format(SharedResources.GENERIC_VALUE_TIMES, cStringUtils.FormatDouble(j))
                    Else
                        strLabel = cStringUtils.FormatDouble(j)
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
                            strLabel = String.Format(SharedResources.GENERIC_VALUE_TIMES, strLabel)
                        End If

                        g.DrawString(strLabel, Me.Font, brTmp, rcImage.Left + 5, yPos)
                        g.DrawLine(penTmp, rcImage.Left, yPos, rcImage.Left + 3, yPos)
                    Next
                End If

                ' Draw time series name
                strLabel = String.Format(My.Resources.GENERIC_LABEL_INDEXED, Me.Shape.Index, Me.Shape.Name)
                g.DrawString(strLabel, tmpFont, brTmp, CSng(rcImage.Width / 2), rcImage.Top + 15, fmt)

                ' Draw time series type
                If TypeOf shape Is cGroupTimeSeries Then

                    Dim gts As cGroupTimeSeries = DirectCast(shape, cGroupTimeSeries)
                    Dim igroup As Integer = gts.GroupIndex

                    If (igroup > 0) Then
                        strName = core.EcoPathGroupInputs(igroup).Name
                    Else
                        strName = My.Resources.TIMESERIES_WARNING_NOGROUP
                    End If
                    strType = cTimeSeriesShapeGUIHandler.GetTimeSeriesTypeName(gts.TimeSeriesType)

                ElseIf TypeOf shape Is cFleetTimeSeries Then

                    Dim fts As cFleetTimeSeries = DirectCast(shape, cFleetTimeSeries)
                    Dim ifleet As Integer = fts.FleetIndex

                    If (ifleet > 0) Then
                        strName = core.FleetInputs(fts.FleetIndex).Name
                    Else
                        strName = My.Resources.TIMESERIES_WARNING_NOFLEET
                    End If
                    strType = cTimeSeriesShapeGUIHandler.GetTimeSeriesTypeName(fts.TimeSeriesType)

                End If

                strLabel = String.Format(My.Resources.GENERIC_LABEL_DETAILED, strType, strName)
                g.DrawString(strLabel, tmpFont, brTmp, CSng(rcImage.Width / 2), rcImage.Top + 33, fmt)

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

            Dim iDS As Integer = Me.UIContext.Core.ActiveTimeSeriesDatasetIndex
            Dim ds As cTimeSeriesDataset = Nothing
            Dim ts As cTimeSeries = DirectCast(Me.Shape, cTimeSeries)
            Dim iTSFinalYear As Integer = 0
            Dim iTSFirstYear As Integer = 0
            Dim iStepSize As Integer = 1
            Dim lstrAxis As New List(Of String)

            If Me.Shape Is Nothing Then Return lstrAxis.ToArray()
            If Not (TypeOf Me.Shape Is cTimeSeries) Then Return lstrAxis.ToArray()

            If Not Me.IsSeasonal Then

                If iDS >= 0 Then ds = Me.UIContext.Core.TimeSeriesDataset(iDS)

                If ds IsNot Nothing Then
                    iTSFirstYear = ds.FirstYear
                Else
                    iTSFirstYear = 1
                End If

                iTSFinalYear = iTSFirstYear + Me.Shape.XMax
                iStepSize = Math.Max(1, CInt((iTSFinalYear - iTSFirstYear) / 10))
                For i As Integer = iTSFirstYear To iTSFinalYear Step iStepSize
                    lstrAxis.Add(i.ToString)
                Next
                Return lstrAxis.ToArray()

            Else

                Dim lstrMonths As New List(Of String)
                For i As Integer = 1 To 12
                    lstrMonths.Add(New Date(1, i, 1).ToString("MMM"))
                Next

                ' Hack: one extra to center labels under value ranges
                lstrMonths.Add("")
                Return lstrMonths.ToArray()

            End If

        End Function

        Protected Overrides Sub OnShapeChanged()
            MyBase.OnShapeChanged()
        End Sub

        Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
            MyBase.OnResize(e)
            Me.OnShapeChanged()
        End Sub

    End Class

End Namespace
