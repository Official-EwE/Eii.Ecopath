'==============================================================================
'
' $Log: ucAnomalySearchSketchPad.vb,v $
' Revision 1.1  2008/09/26 07:31:54  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.3  2008/08/05 15:38:26  jeroens
' TS year limit grayed out
'
' Revision 1.2  2008/06/02 00:01:40  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.1  2007/11/11 16:49:18  jeroens
' Initial verision
'
'==============================================================================

Option Strict On
Imports EwECore
Imports System.Drawing.Drawing2D

Namespace Ecosim

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="ucSketchPad">Sketchpad-derived</see> control that renders
    ''' a forcing function for use in the Anomaly search panel.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucAnomalySearchSketchPad

#Region " Private bits "

        Private m_iYearStartDragPos As Integer = -1
        Private m_iYearEndDragPos As Integer = -1
        Private m_iFirstYear As Integer = 0
        Private m_iLastYear As Integer = 0
        Private m_iNumSplinePoints As Integer = 0
        Private m_iNumTSYears As Integer = 0

#End Region ' Private bits

#Region " Public properties "

        Public Property FirstYear() As Integer
            Get
                Return Me.m_iFirstYear
            End Get
            Set(ByVal value As Integer)
                Me.m_iFirstYear = value
                Me.Invalidate()
            End Set
        End Property

        Public Property LastYear() As Integer
            Get
                Return Me.m_iLastYear
            End Get
            Set(ByVal value As Integer)
                Me.m_iLastYear = value
                Me.Invalidate()
            End Set
        End Property

        Public Property NumSplinePoints() As Integer
            Get
                Return Me.m_iNumSplinePoints
            End Get
            Set(ByVal value As Integer)
                Me.m_iNumSplinePoints = value
                Me.Invalidate()
            End Set
        End Property

        Public Property NumTSYears() As Integer
            Get
                Return Me.m_iNumTSYears
            End Get
            Set(ByVal value As Integer)
                Me.m_iNumTSYears = value
                Me.Invalidate()
            End Set
        End Property

#End Region ' Public properties

#Region " Public events "

        Public Event OnYearRangeChanged(ByVal sender As ucAnomalySearchSketchPad)

#End Region ' Public events

#Region " Internal implementation "

        Protected Overrides Sub DrawShape(ByVal shape As EwECore.cShapeData, _
                 ByVal rcImage As System.Drawing.Rectangle, _
                 ByVal g As System.Drawing.Graphics, _
                 ByVal clr As System.Drawing.Color, _
                 ByVal bDrawLabels As Boolean, _
                 ByVal drawMode As eSketchDrawModeTypes, _
                 ByVal sYMax As Single)

            Dim iYear1 As Integer = 0
            Dim iYear2 As Integer = 0
            Dim iSpline As Integer = 0

            ' Designer mode test
            If (Me.Shape Is Nothing) Then Return

            MyBase.DrawShape(shape, rcImage, g, clr, bDrawLabels, drawMode, sYMax)

            ' Draw gray area to block out areas past TS
            Me.DrawYearLimit(g, Me.YearToX(Me.NumTSYears, rcImage.Width))

            ' Draw year line 1
            If (Me.m_iYearStartDragPos >= 0) Then
                iYear1 = Me.m_iYearStartDragPos
            Else
                iYear1 = Me.YearToX(Me.m_iFirstYear, rcImage.Width)
            End If
            Me.DrawYearLine(g, iYear1)

            ' Draw year line 2
            If (Me.m_iYearEndDragPos >= 0) Then
                iYear2 = Me.m_iYearEndDragPos
            Else
                iYear2 = Me.YearToX(Me.m_iLastYear, rcImage.Width)
            End If
            Me.DrawYearLine(g, iYear2)

            ' Draw spline points
            If (iYear2 = 0) Then iYear2 = rcImage.Width
            For i As Integer = 1 To Me.m_iNumSplinePoints - 2
                iSpline = CInt(iYear1 + Math.Round(i * (iYear2 - iYear1) / (Me.m_iNumSplinePoints - 1)))
                Me.DrawSplineLine(g, iSpline)
            Next

        End Sub

        Private Function YearToX(ByVal iYear As Integer, ByVal iWidth As Integer) As Integer
            Return CInt(Math.Round((iYear * iWidth * cCore.N_MONTHS) / Me.Shape.XMax))
        End Function

        Private Function XToYear(ByVal x As Integer, ByVal iWidth As Integer) As Integer
            Dim iYear As Integer = CInt(Math.Round(x * Me.Shape.XMax / (cCore.N_MONTHS * iWidth)))
            Return Math.Min(Math.Max(0, iYear), CInt(Math.Floor(Me.Shape.XMax / cCore.N_MONTHS)))
        End Function

        Private Sub DrawYearLine(ByRef g As Graphics, ByVal x As Integer)
            Using penLine As New Pen(Drawing.Color.Black, 2)
                penLine.DashStyle = Drawing2D.DashStyle.Dot
                g.DrawLine(penLine, New Point(x, 0), New Point(x, Me.Height))
            End Using
        End Sub

        Private Sub DrawSplineLine(ByRef g As Graphics, ByVal x As Integer)
            Using penLine As New Pen(Drawing.Color.Orange, 1)
                penLine.DashStyle = Drawing2D.DashStyle.Dot
                g.DrawLine(penLine, New Point(x, 0), New Point(x, Me.Height))
            End Using
        End Sub

        Private Sub DrawYearLimit(ByRef g As Graphics, ByVal x As Integer)
            Using br As New HatchBrush(HatchStyle.DiagonalCross, Drawing.Color.LightGray, Drawing.Color.Transparent)
                g.FillRectangle(br, New Rectangle(x, 0, Me.Width, Me.Height))
            End Using
        End Sub

        Private cMOUSE_TOLERANCE As Integer = 3

        Private Sub ucAnomalySearchSketchPad_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
            If (Math.Abs(e.X - Me.YearToX(Me.m_iFirstYear, Me.ClientRectangle.Width)) <= cMOUSE_TOLERANCE) Then
                Me.m_iYearStartDragPos = e.X
                Me.Capture = True
            ElseIf (Math.Abs(e.X - Me.YearToX(Me.m_iLastYear, Me.ClientRectangle.Width)) <= cMOUSE_TOLERANCE) Then
                Me.m_iYearEndDragPos = e.X
                Me.Capture = True
            End If
        End Sub

        Private Sub ucAnomalySearchSketchPad_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove

            If Me.Capture Then
                If (Me.m_iYearStartDragPos >= 0) Then
                    Me.m_iYearStartDragPos = e.X
                    Me.Invalidate()
                ElseIf (Me.m_iYearEndDragPos >= 0) Then
                    Me.m_iYearEndDragPos = e.X
                    Me.Invalidate()
                End If
            Else
                If (Math.Abs(e.X - Me.YearToX(Me.m_iFirstYear, Me.ClientRectangle.Width)) <= cMOUSE_TOLERANCE) Or _
                   (Math.Abs(e.X - Me.YearToX(Me.m_iLastYear, Me.ClientRectangle.Width)) <= cMOUSE_TOLERANCE) Then
                    Me.Cursor = Cursors.SizeWE
                Else
                    Me.Cursor = Cursors.Default
                End If
            End If

        End Sub

        Private Sub ucAnomalySearchSketchPad_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp

            If Me.Capture Then

                ' Calc resulting years
                If (Me.m_iYearStartDragPos >= 0) Then
                    Me.m_iFirstYear = Me.XToYear(Me.m_iYearStartDragPos, Me.ClientRectangle.Width)
                ElseIf (Me.m_iYearEndDragPos >= 0) Then
                    Me.m_iLastYear = Me.XToYear(Me.m_iYearEndDragPos, Me.ClientRectangle.Width)
                End If

                ' Sort resulting years
                If (Me.m_iFirstYear > Me.m_iLastYear) Then
                    Dim i As Integer = Me.m_iFirstYear
                    Me.m_iFirstYear = Me.m_iLastYear
                    Me.m_iLastYear = i
                End If

                ' Notify the world
                RaiseEvent OnYearRangeChanged(Me)

                ' Refresh
                Me.Invalidate()

                ' Stop drag
                Me.Capture = False
                Me.m_iYearStartDragPos = -1
                Me.m_iYearEndDragPos = -1

            End If

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
