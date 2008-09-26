'==============================================================================
'
' $Log: cUpwellingIndicator.vb,v $
' Revision 1.1  2008/09/26 07:31:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:09  jeroens
' Separated from Scientific Interface
'
' Revision 1.1  2008/03/29 21:19:59  jeroens
' Initial version
'
'==============================================================================

#Region " Imports directive "

Option Strict On
Imports System.Drawing
Imports System.Drawing.Drawing2D

#End Region ' Imports directive

Public Class cUpwellingIndicator

    Public Shared Sub DrawUpwelling(ByVal g As Graphics, ByVal clr As Color, ByVal rc As Rectangle, ByVal sVelocity As Single)

        'EwE5:
        'Up.Circle (j + 0.5, i + 0.5 - UpVel(i, j) / UpMax), 0.1
        'Up.Line (j + 0.5, i + 0.5)-Step(0, -UpVel(i, j) / UpMax)

        ' Determine diameter of circle (min of on quarter of the height OR rectangle width)
        Dim sSymbolSize As Single = CSng(Math.Min(rc.Height / 4, rc.Width))
        ' Determine relative center of the cell
        Dim ptfCenterRel As New PointF(CSng(rc.Width / 2), CSng(rc.Height / 2))
        ' Determine circle center
        Dim ptfCircle As New PointF(rc.X + ptfCenterRel.X, rc.Y + ptfCenterRel.Y - (ptfCenterRel.Y - (sSymbolSize / 2)) * sVelocity)

        ' Draw line from circle center to cell center
        Using p As New Pen(clr, Math.Max(1, sSymbolSize / 4))
            g.DrawLine(p, _
                ptfCircle.X, ptfCircle.Y, _
                ptfCircle.X, rc.Y + ptfCenterRel.Y)
        End Using
        ' Craw circle around cirlce center
        Using b As New SolidBrush(clr)
            g.FillEllipse(b, _
                ptfCircle.X - (sSymbolSize / 2), ptfCircle.Y - (sSymbolSize / 2), _
                sSymbolSize, sSymbolSize)
        End Using

    End Sub

End Class
