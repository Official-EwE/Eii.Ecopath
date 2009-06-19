'==============================================================================
'
' $Log: ucMediationSketchPad.vb,v $
' Revision 1.10  2009/06/19 16:45:42  jeroens
' Y-axis labels scaled to XMark value
'
' Revision 1.9  2009/06/19 06:31:12  jeroens
' Added Y-axis labels to Med shapes
'
' Revision 1.8  2009/05/28 12:37:53  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.7  2009/04/20 13:57:13  jeroens
' no message
'
' Revision 1.6  2009/03/24 02:00:27  jeroens
' Fixed crash when no shape selected
'
' Revision 1.5  2009/03/21 00:30:34  jeroens
' Fixed unclear parameter names
'
' Revision 1.4  2009/03/02 17:43:52  jeroens
' Cleaned up
'
' Revision 1.3  2009/03/02 02:03:42  jeroens
' Simplified
'
' Revision 1.2  2009/02/12 15:32:21  jeroens
' Can add labels to XMark, YMark lines
'
' Revision 1.1  2008/12/15 15:36:40  jeroens
' Moved from ScInt
'
' Revision 1.1  2008/09/26 07:31:38  sherman
' --== DELETED HISTORY ==--
'
'==============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region

Namespace Controls

    Public Class ucMediationSketchPad

        Public Sub New()

            Me.InitializeComponent()
            'No axis info in the mediation sketchpad right now. 
            m_tsmiShowMarks.Visible = False
        End Sub

        Private Sub MediationSketchPad_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles MyBase.Paint
            Me.DrawShape(Me.Shape, Me.ClientRectangle, e.Graphics, Me.ShapeColor, True, Me.SketchDrawMode, Me.YAxisMaxValue)
        End Sub

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            Dim iXMax As Integer = 0
            Dim sfmt As StringFormat = Nothing
            Dim strCaption As String = ""
            Dim strLabel As String = ""
            Dim iYStep As Integer = 0
            Dim iYPos As Integer = 0
            Dim sYScale As Single = 1
            Dim sg As cStyleGuide = cStyleGuide.GetInstance()

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            ' Sanity checks
            If Me.Shape Is Nothing Then Return
            If Not bDrawLabels Then Return

            'sYMax = Me.YAxisMaxValue
            iXMax = Me.Shape.XMax
            sYScale = Me.YMarkValue

            If (sYScale = 0) Then sYScale = 1

            sfmt = New StringFormat()
            sfmt.Alignment = StringAlignment.Center
            sfmt.LineAlignment = StringAlignment.Center

            strCaption = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, _
                                       (DirectCast(Me.Shape, cMediationFunction).ID + 1), _
                                       Me.Shape.Name)

            Using br As New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                Using ft As New Font(sg.GraphFontFamilyName, sg.GraphAxisScaleFontSize)
                    Using pn As New Pen(Color.FromArgb(128, 0, 0, 0))

                        g.DrawString(strCaption, ft, br, CSng(rcImage.Width / 2), rcImage.Top + 15, sfmt)
                        g.DrawString(My.Resources.MEDIATION_X_AXIS_LABEL, ft, br, CSng(rcImage.Width / 2), rcImage.Bottom - 15, sfmt)

                        ' Scale sYMax to the value at XMark
                        sYMax /= sYScale

                        'Draw Axis Y marks
                        iYStep = CInt(sYMax / 3)
                        If iYStep = 0 Then iYStep = 1

                        For j As Double = 0 To sYMax Step iYStep * 0.5
                            iYPos = CInt(ShapeImage.ToImagePoint(New PointF(0, CSng(j)), rcImage, 0, sYMax).Y)

                            strLabel = j.ToString
                            g.DrawString(strLabel, Me.Font, br, rcImage.Left + 5, iYPos)
                            g.DrawLine(pn, rcImage.Left, iYPos, rcImage.Left + 3, iYPos)
                        Next

                        If sYMax < 0.5 And sYMax >= 0.01 Then
                            For j As Integer = 0 To 2
                                iYPos = CInt(rcImage.Bottom - rcImage.Height * (3 - j) / 3)
                                strLabel = sg.FormatNumber(sYMax * (3 - j) / 3)
                                g.DrawString(strLabel, Me.Font, br, rcImage.Left + 5, iYPos)
                                g.DrawLine(pn, rcImage.Left, iYPos, rcImage.Left + 3, iYPos)
                            Next
                        End If
                    End Using
                End Using
            End Using
        End Sub

    End Class

End Namespace
