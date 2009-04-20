'==============================================================================
'
' $Log: ucMediationSketchPad.vb,v $
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
            Dim sg As StyleGuide = StyleGuide.GetInstance()

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            ' Sanity checks
            If Me.Shape Is Nothing Then Return
            If Not bDrawLabels Then Return

            'sYMax = Me.YAxisMaxValue
            iXMax = Me.Shape.XMax

            sfmt = New StringFormat()
            sfmt.Alignment = StringAlignment.Center
            sfmt.LineAlignment = StringAlignment.Center

            strCaption = String.Format(My.Resources.GENERIC_LABEL_INDEXEDLABEL, (DirectCast(Me.Shape, cMediationFunction).ID + 1), Me.Shape.Name)

            Using br As New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
                Using ft As New Font(sg.GraphFontFamilyName, sg.GraphAxisScaleFontSize)
                    g.DrawString(strCaption, ft, br, CSng(rcImage.Width / 2), rcImage.Top + 15, sfmt)
                    g.DrawString(My.Resources.MEDIATION_X_AXIS_LABEL, ft, br, CSng(rcImage.Width / 2), rcImage.Bottom - 15, sfmt)
                End Using
            End Using

        End Sub

    End Class

End Namespace
