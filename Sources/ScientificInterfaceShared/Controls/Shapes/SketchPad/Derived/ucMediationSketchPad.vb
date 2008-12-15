'==============================================================================
'
' $Log: ucMediationSketchPad.vb,v $
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
Imports ScientificInterfaceShared.Definitions

#End Region

Namespace Controls

    Public Class ucMediationSketchPad

        Public Sub New()

            Me.InitializeComponent()
            'No axis info in the mediation sketchpad right now. 
            AxisToolStripMenuItem.Visible = False
        End Sub

        Private Sub MediationSketchPad_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint
            Me.DrawShape(Me.Shape, Me.ClientRectangle, e.Graphics, Me.Color, True, Me.SketchDrawMode, Me.YAxisMaxValue)
        End Sub

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                ByVal rcImage As System.Drawing.Rectangle, _
                ByVal g As System.Drawing.Graphics, _
                ByVal clr As System.Drawing.Color, _
                ByVal bDrawLabels As Boolean, _
                ByVal drawMode As eSketchDrawModeTypes, _
                ByVal sYMax As Single)

            Dim iXMax As Integer = 0
            Dim tmpFont As Font = Nothing
            Dim tmpBrush As SolidBrush = Nothing
            Dim sfmt As StringFormat = Nothing
            Dim strCaption As String = ""

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            If Not bDrawLabels Then Return

            ' Sanity ceck
            If Me.Shape Is Nothing Then Return

            iXMax = CInt(IIf(Me.Shape.IsSeasonal, cCore.N_MONTHS, Me.Shape.XMax))
            sYMax = Me.YAxisMaxValue

            'g.DrawLine(Pens.Black, _
            '        ShapeImage.toScreenPoint(New PointF(0, 0.5), rcImage, iXMax, Me.Shape.YMax), _
            '        ShapeImage.toScreenPoint(New PointF(iXMax, 0.5), rcImage, iXMax, Me.Shape.YMax))

            sfmt = New StringFormat
            sfmt.Alignment = StringAlignment.Center
            sfmt.LineAlignment = StringAlignment.Center

            tmpFont = New Font(Me.Font.FontFamily, Me.Font.Size + 2)
            tmpBrush = New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))

            strCaption = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (DirectCast(Me.Shape, cMediationFunction).ID + 1), Me.Shape.Name)
            g.DrawString(strCaption, tmpFont, tmpBrush, CSng(rcImage.Width / 2), rcImage.Top + 15, sfmt)

            g.DrawString(My.Resources.MEDIATION_X_AXIS_LABEL, tmpFont, tmpBrush, CSng(rcImage.Width / 2), rcImage.Bottom - 15, sfmt)

            tmpFont.Dispose()
            tmpBrush.Dispose()

        End Sub

    End Class

End Namespace
