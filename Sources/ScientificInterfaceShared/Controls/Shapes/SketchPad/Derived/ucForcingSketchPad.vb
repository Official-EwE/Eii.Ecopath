'==============================================================================
'
' $Log: ucForcingSketchPad.vb,v $
' Revision 1.1  2008/12/15 15:36:40  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:37  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared.Definitions

#End Region ' Imports

Namespace Controls

    <CLSCompliant(True)> _
    Public Class ucForcingSketchPad

        Private m_AxisYMarks As eAxisTickmarkDisplayModeTypes

        Public WriteOnly Property AxisTickMarkDisplayMode() As eAxisTickmarkDisplayModeTypes
            Set(ByVal value As eAxisTickmarkDisplayModeTypes)
                m_AxisYMarks = value
            End Set
        End Property

        Public Sub New()

            ' This call is required by the Windows Form Designer.
            InitializeComponent()

            'Default display as Absolute value
            m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Absolute

        End Sub

        ''' <summary>
        ''' This method returns the marks displayed on the Axis X
        ''' </summary>
        Private Function GetAxisX() As String()

            Dim lstrAxis As New List(Of String)

            If Me.Shape Is Nothing Then Return lstrAxis.ToArray()

            If Not Me.IsSeasonal Then
                Dim iStepSize As Integer
                Dim iYMax As Integer = CInt(Me.Shape.XMax / cCore.N_MONTHS)

                iStepSize = (iYMax + 9) \ 10
                For i As Integer = 0 To iYMax Step iStepSize
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

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            Dim strLabel As String = ""
            Dim sXMax As Single = 0.0!
            Dim sLabelXPos As Single = 0.0!
            Dim astrXMarks As String() = Nothing
            Dim sfmt As StringFormat = Nothing
            Dim sBtnSpace As Single = Me.Font.Height
            Dim brTmp As SolidBrush = Nothing
            Dim penTmp As Pen = Nothing
            Dim tmpFont As Font = Nothing
            Dim yStep As Integer = 0

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            If Not bDrawLabels Then Return
            If Me.Shape Is Nothing Then Return

            'Draw the line with y's value equal to 1
            g.DrawLine(Pens.Black, _
                ShapeImage.toImagePoint(New PointF(0, 1), Me.ClientRectangle, Me.Shape.XMax, sYMax), _
                ShapeImage.toImagePoint(New PointF(Me.Shape.XMax, 1), Me.ClientRectangle, Me.Shape.XMax, sYMax))

            ' Draw the axis when this mode is on
            If m_AxisDisplayMode = eAxisDisplayModeTypes.Show Then

                'Draw Axis
                g.DrawLine(Pens.Gray, New PointF(rcImage.Left, rcImage.Bottom), New PointF(rcImage.Right, rcImage.Bottom))
                g.DrawLine(Pens.Gray, New PointF(rcImage.Left, rcImage.Top), New PointF(rcImage.Left, rcImage.Bottom))

                ' Draw Axis X marks
                astrXMarks = GetAxisX()

                sfmt = New StringFormat
                sfmt.Alignment = StringAlignment.Center
                sfmt.LineAlignment = StringAlignment.Center

                sBtnSpace = Me.Font.Height
                brTmp = New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                penTmp = New Pen(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                tmpFont = New Font(Me.Font.FontFamily, Me.Font.Size + 2)

                For i As Integer = 0 To astrXMarks.Length - 1
                    If Me.Shape.IsSeasonal Then
                        sLabelXPos = CSng((i + 0.5!) * rcImage.Width / (astrXMarks.Length - 1))
                    Else
                        sLabelXPos = CSng(i * rcImage.Width / (astrXMarks.Length - 1))
                    End If
                    g.DrawString(astrXMarks(i), Me.Font, brTmp, rcImage.Left + sLabelXPos, rcImage.Bottom - sBtnSpace, sfmt)
                    g.DrawLine(penTmp, rcImage.Left + sLabelXPos, rcImage.Bottom, rcImage.Left + sLabelXPos, rcImage.Bottom - sBtnSpace / 2)
                Next

                'Draw Axis Y marks
                yStep = CInt(sYMax / 3)
                If yStep = 0 Then yStep = 1

                For j As Double = 0 To sYMax Step yStep * 0.5
                    ' JS 21nov07: calc proper label Y position
                    'Dim yPos As Integer = CInt(rcImage.Bottom - rcImage.Height * j / sYMax)
                    Dim yPos As Integer = CInt(ShapeImage.toImagePoint(New PointF(0, CSng(j)), rcImage, 0, sYMax).Y)

                    strLabel = j.ToString
                    If m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Relative Then
                        strLabel = String.Format("x{0}", strLabel)
                    End If
                    g.DrawString(strLabel, Me.Font, brTmp, rcImage.Left + 5, yPos)
                    g.DrawLine(penTmp, rcImage.Left, yPos, rcImage.Left + 3, yPos)
                Next

                If sYMax < 0.5 And sYMax >= 0.01 Then
                    For j As Integer = 0 To 2
                        Dim yPos As Integer = CInt(rcImage.Bottom - rcImage.Height * (3 - j) / 3)
                        strLabel = String.Format("{0:f3}", sYMax * (3 - j) / 3)
                        If m_AxisYMarks = eAxisTickmarkDisplayModeTypes.Relative Then
                            strLabel = String.Format("x{0}", strLabel)
                        End If
                        g.DrawString(strLabel, Me.Font, brTmp, rcImage.Left + 5, yPos)
                        g.DrawLine(penTmp, rcImage.Left, yPos, rcImage.Left + 3, yPos)
                    Next
                End If

                ' Display shape ID (=index in manager list) + 1
                strLabel = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (DirectCast(Me.Shape, cForcingFunction).ID + 1), Me.Shape.Name)
                g.DrawString(strLabel, tmpFont, brTmp, CSng(rcImage.Width / 2), rcImage.Top + 15, sfmt)

                ' Dispose the pen, brush and font we created and let the system garbage collect them.
                brTmp.Dispose()
                penTmp.Dispose()
                tmpFont.Dispose()

            End If

        End Sub

    End Class

End Namespace
