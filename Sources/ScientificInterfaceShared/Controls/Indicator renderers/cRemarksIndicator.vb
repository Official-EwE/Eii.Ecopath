'==============================================================================
'
' $Log: cRemarksIndicator.vb,v $
' Revision 1.2  2009/05/28 12:37:18  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2008/09/26 07:31:17  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:09  jeroens
' Separated from Scientific Interface
'
' Revision 1.8  2008/05/07 01:39:04  jeroens
' Fixed bugs 281, 378, 470
'
' Revision 1.7  2008/01/31 03:08:03  jeroens
' Added header, *finally*
'
'==============================================================================

Option Strict On
Imports System.Globalization
Imports System.Threading
Imports System.Drawing
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, renders remarks a remarks indicator into a control.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cRemarksIndicator

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns the remarks indicator corner points for a given cell boundary. The current UI culture
        ''' reading order is evaluated to determine the position and layout of the remarks indicator.
        ''' </summary>
        ''' <param name="rcClip">Clip boundary to calculate the remarks indicator for</param>
        ''' <returns>A series of <see cref="Point">points</see></returns>
        ''' -----------------------------------------------------------------------
        Private Shared Function GetPoints(ByVal rcClip As Rectangle) As Point()
            Dim ci As CultureInfo = Thread.CurrentThread.CurrentUICulture
            Dim nSize As Integer = CInt(Math.Floor(rcClip.Height / 2.5))
            Dim pt(2) As Point

            If (ci.TextInfo.IsRightToLeft) Then
                ' 0--1---
                ' | /       
                ' 2
                ' |
                pt(0) = New Point(rcClip.X + 1, rcClip.Y)
                pt(1) = New Point(rcClip.X + 1 + nSize, rcClip.Y)
                pt(2) = New Point(rcClip.X + 1, rcClip.Y + nSize)
            Else
                ' ---1--0
                '     \ |
                '       2
                '       |
                pt(0) = New Point(rcClip.X + rcClip.Width - 1, rcClip.Y)
                pt(1) = New Point(rcClip.X + rcClip.Width - 1 - nSize, rcClip.Y)
                pt(2) = New Point(rcClip.X + rcClip.Width - 1, rcClip.Y + nSize)
            End If
            Return pt
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Renders a remarks indicator onto a given canvas
        ''' </summary>
        ''' <param name="rcClip">Clip boundary to fit the remarks indicator in</param>
        ''' <param name="g">The canvas to render onto</param>
        ''' <param name="bHasRemarks">States whether the indicator is rendered as having remarks (true) or
        ''' as ready for receiving remarks (false)</param>
        ''' -----------------------------------------------------------------------
        Public Shared Sub Paint(ByVal rcClip As Rectangle, ByVal g As Graphics, ByVal bHasRemarks As Boolean)

            Dim sg As cStyleGuide = cStyleGuide.GetInstance()
            Dim pt() As Point = GetPoints(rcClip)
            Dim clrFill As Color = Nothing

            If (bHasRemarks) Then
                clrFill = sg.ApplicationColor(cStyleGuide.eApplicationColorType.REMARKS_BACKGROUND)
                Using br As New SolidBrush(clrFill)
                    g.FillPolygon(br, pt)
                End Using
                g.DrawLine(SystemPens.ControlDark, pt(1), pt(2))
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, determines the bounding box for a remarks indicator. This method could be handy when
        ''' testing for remarks indicator mouse hits.
        ''' </summary>
        ''' <param name="rcClip">Coordinates of the area to get the remarks indicator bounding box for.</param>
        ''' <returns>The bounding box that fully encapsulates the Remarks indicator.</returns>
        ''' -----------------------------------------------------------------------
        Public Shared Function GetBounds(ByVal rcClip As Rectangle) As Rectangle
            Dim pt As Point() = GetPoints(rcClip)
            Return New Rectangle(Math.Min(pt(0).X, pt(1).X), pt(0).Y, Math.Abs(pt(1).X - pt(0).X), pt(2).Y - pt(0).Y)
        End Function

    End Class

End Namespace ' Controls
