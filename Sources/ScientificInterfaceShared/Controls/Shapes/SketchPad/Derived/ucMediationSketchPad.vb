#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.ComponentModel
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Definitions

#End Region

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control, implements a control for sketching Ecosim mediation shapes.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucMediationSketchPad

        Private m_strXAxisLabel As String

        Public Sub New()
            Me.InitializeComponent()
            ''No axis info in the mediation sketchpad right now. 
            'Me.m_tsmiShowMarks.Visible = False
        End Sub

        Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
            Me.DrawShape(Me.Shape, Me.ClientRectangle, e.Graphics, Me.ShapeColor, True, Me.SketchDrawMode, Me.YAxisMaxValue)
        End Sub

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            If (Me.UIContext Is Nothing) Then Return

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            ' Sanity checks
            If (shape Is Nothing) Then Return
            If (Not TypeOf (shape) Is cMediationBaseFunction) Then Return
            If (Not bDrawLabels) Then Return

            Dim iXMax As Integer = 0
            Dim sfmt As StringFormat = Nothing
            Dim strCaption As String = ""
            Dim strLabel As String = ""
            Dim iYStep As Integer = 0
            Dim iYPos As Integer = 0
            Dim sYScale As Single = 1
            Dim sg As cStyleGuide = Me.UIContext.StyleGuide

            'sYMax = Me.YAxisMaxValue
            iXMax = Me.Shape.XMax
            sYScale = Me.YMarkValue

            If (sYScale = 0) Then sYScale = 1

            sfmt = New StringFormat()
            sfmt.Alignment = StringAlignment.Center
            sfmt.LineAlignment = StringAlignment.Center

            ' Write mediation caption
            strCaption = String.Format(My.Resources.GENERIC_LABEL_INDEXED, _
                                       (DirectCast(Me.Shape, cMediationBaseFunction).ID + 1), _
                                       Me.Shape.Name)

            Using br As New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                Using ft As Font = sg.Font(cStyleGuide.eApplicationFontType.Scale)
                    Using pn As New Pen(Color.FromArgb(128, 0, 0, 0))

                        g.DrawString(strCaption, ft, br, CSng(rcImage.Width / 2), rcImage.Top + 15, sfmt)
                        g.DrawString(Me.XAxisLabel, ft, br, CSng(rcImage.Width / 2), rcImage.Bottom - 15, sfmt)

                        ' Scale sYMax to the value at XMark
                        sYMax /= sYScale

                        'Draw Axis Y marks
                        iYStep = CInt(sYMax / 3)
                        If iYStep = 0 Then iYStep = 1

                        ' Draw Y-axis scale
                        For j As Double = 0 To sYMax Step iYStep * 0.5
                            iYPos = CInt(cShapeImage.ToImagePoint(New PointF(0, CSng(j)), rcImage, 0, sYMax).Y)

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

        <Browsable(True), _
         Category("Mediation")> _
        Public Property XAxisLabel() As String
            Get
                Return Me.m_strXAxisLabel
            End Get
            Set(ByVal value As String)
                Me.m_strXAxisLabel = value
            End Set
        End Property

        Public Overrides ReadOnly Property XMarkLabel() As String
            Get
                If (Not TypeOf (Shape) Is cMediationBaseFunction) Then Return MyBase.XMarkLabel
                Dim medfn As cMediationBaseFunction = DirectCast(Me.Shape, cMediationBaseFunction)
                Return Me.UIContext.StyleGuide.FormatNumber(medfn.XBase)
            End Get
        End Property

    End Class

End Namespace
