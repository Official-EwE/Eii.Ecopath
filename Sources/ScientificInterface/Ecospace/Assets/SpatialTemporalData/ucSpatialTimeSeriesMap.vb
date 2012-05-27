' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwEUtils.SpatialData

#End Region ' Imports

Namespace Ecospace

    Public Class ucSpatialTimeSeriesMap
        Implements IUIElement

        Public Enum eZoomLevel As Integer
            Both = 0
            Map
            Data
        End Enum

        Private m_ds As ISpatialDataSet = Nothing
        Private m_iTimeStep As Integer = -1
        Private m_uic As cUIContext = Nothing
        Private m_rcfEcospaceExtent As RectangleF
        Private m_lExternalDataMapExtents As New List(Of RectangleF)
        Private m_zoomlevel As eZoomLevel = eZoomLevel.Both
        Private m_bShowRefMap As Boolean = False

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        End Sub

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                Me.m_uic = value
                Me.RefreshContent()
            End Set
        End Property

        Public Property SelectedDataset As ISpatialDataSet
            Get
                Return Me.m_ds
            End Get
            Set(value As ISpatialDataSet)
                Me.m_ds = value
                Try
                    Me.RefreshContent()
                Catch ex As Exception
                End Try
                Me.Invalidate()
            End Set
        End Property

        Public Property SelectedTimeStep As Integer
            Get
                Return Me.m_iTimeStep
            End Get
            Set(value As Integer)
                Me.m_iTimeStep = value
                Me.Invalidate()
            End Set
        End Property

        Public Property ZoomLevel As eZoomLevel
            Get
                Return Me.m_zoomlevel
            End Get
            Set(value As eZoomLevel)
                Me.m_zoomlevel = value
                Me.Invalidate()
            End Set
        End Property

        Public Property ShowReferenceMap As Boolean
            Get
                Return Me.m_bShowRefMap
            End Get
            Set(value As Boolean)
                Me.m_bShowRefMap = value
                Me.Invalidate()
            End Set
        End Property

        Public Sub RefreshContent()

            If (Me.m_uic Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            Me.m_rcfEcospaceExtent = Me.ToDisplayRect(bm.PosTopLeft, bm.PosBottomRight)
            Me.m_lExternalDataMapExtents.Clear()

            If (Me.m_ds Is Nothing) Then Return

            Dim iTimeStart As Integer = 1
            Dim iTimeEnd As Integer = Me.m_uic.Core.nEcospaceTimeSteps
            Dim ptfTL As PointF = Nothing
            Dim ptfBR As PointF = Nothing

            If Me.m_ds.TimeStart <> Date.MinValue Then
                iTimeStart = Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(Me.m_ds.TimeStart)
            End If

            If Me.m_ds.TimeEnd < Date.MaxValue Then
                iTimeEnd = Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(Me.m_ds.TimeEnd)
            End If

            For iStep As Integer = iTimeStart To iTimeEnd
                If Me.m_ds.GetExtentAtT(Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iStep), ptfTL, ptfBR) Then
                    Me.m_lExternalDataMapExtents.Add(ToDisplayRect(ptfTL, ptfBR))
                End If
            Next

            Me.BackColor = sg.ApplicationColor(cStyleGuide.eApplicationColorType.PLOT_BACKGROUND)
            Me.Invalidate()

        End Sub

        Private Function ToDisplayRect(ptfTL As PointF, ptfBR As PointF) As RectangleF
            Return New RectangleF(ptfTL.X, -ptfTL.Y, ptfBR.X - ptfTL.X, ptfTL.Y - ptfBR.Y)
        End Function

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)
            Me.DoPaint(e.Graphics, Me.ClientRectangle)
        End Sub

        Private Sub DoPaint(g As Graphics, rc As Rectangle)

            If (Me.m_uic Is Nothing) Then Return

            Dim rcfViewExtent As RectangleF = Me.m_rcfEcospaceExtent
            Dim sg As cStyleGuide = Me.m_uic.StyleGuide

            ' Is data zoom involved?
            If (Me.m_zoomlevel = eZoomLevel.Both) Or (Me.m_zoomlevel = eZoomLevel.Data) Then
                ' #Yes: only zoom to data? (and has data to zoom to)
                If (Me.m_zoomlevel = eZoomLevel.Data) And (Me.m_lExternalDataMapExtents.Count > 0) Then
                    ' #Yes: ok, then base zoom exclusively on data
                    rcfViewExtent = New Rectangle(180, 90, -360, -180)
                End If

                ' Find biggest display rect
                For Each rcf As RectangleF In Me.m_lExternalDataMapExtents
                    Dim ptfTL As New PointF(Math.Min(rcf.Left, rcfViewExtent.Left), Math.Min(rcf.Top, rcfViewExtent.Top))
                    Dim ptfBR As New PointF(Math.Max(rcf.Right, rcfViewExtent.Right), Math.Max(rcf.Bottom, rcfViewExtent.Bottom))
                    rcfViewExtent = New RectangleF(ptfTL.X, ptfTL.Y, ptfBR.X - ptfTL.X, ptfBR.Y - ptfTL.Y)
                Next
            End If

            ' Abort if nothing to display
            If (rcfViewExtent.Width = 0 Or rcfViewExtent.Height = 0) Then Return

            Dim sScale As Single = CSng(Math.Min(rc.Height / (rcfViewExtent.Height * 3), rc.Width / (rcfViewExtent.Width * 3)))
            Dim dx As Single = rc.Width / 2.0! - (rcfViewExtent.X + rcfViewExtent.Width / 2.0!) * sScale
            Dim dy As Single = rc.Height / 2.0! - (rcfViewExtent.Y + rcfViewExtent.Height / 2.0!) * sScale
            g.TranslateTransform(dx, dy)
            g.ScaleTransform(sScale, sScale)

            ' Draw background
            If (Me.ShowReferenceMap) Then

                Dim img As Image = sg.MapReferenceImage
                If (img IsNot Nothing) Then
                    Try
                        g.DrawImage(img, _
                                    sg.MapReferenceLayerTL.X, -sg.MapReferenceLayerTL.Y, _
                                    (sg.MapReferenceLayerBR.X - sg.MapReferenceLayerTL.X), (sg.MapReferenceLayerTL.Y - sg.MapReferenceLayerBR.Y))
                    Catch ex As Exception
                        Debug.Assert(False, ex.Message)
                    End Try
                End If
            End If

            Try
                If (Me.m_lExternalDataMapExtents.Count > 0) Then
                    g.FillRectangles(Brushes.LightBlue, Me.m_lExternalDataMapExtents.ToArray)
                    Using p As New Pen(Brushes.Blue, 0.001)
                        g.DrawRectangles(p, Me.m_lExternalDataMapExtents.ToArray)
                    End Using
                End If

                g.FillRectangles(Brushes.LightGreen, New RectangleF() {Me.m_rcfEcospaceExtent})
                Using p As New Pen(Brushes.Green, 0.001)
                    g.DrawRectangles(p, New RectangleF() {Me.m_rcfEcospaceExtent})
                End Using
            Catch ex As Exception
                Debug.Assert(False, ex.Message)
            End Try

            g.ResetTransform()

            ' Draw labels
            Dim tmpFont As Font = sg.Font(cStyleGuide.eApplicationFontType.Scale)
            Dim brTmp As New SolidBrush(System.Drawing.Color.FromArgb(128, 0, 0, 0))
            Dim penTmp As New Pen(System.Drawing.Color.FromArgb(128, 0, 0, 0))
            Dim strLabel As String = ""
            Dim fmt As New StringFormat

            fmt.Alignment = StringAlignment.Center
            fmt.LineAlignment = StringAlignment.Center

            strLabel = String.Format("Time step {0} ({1})", Me.m_iTimeStep, Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(Me.m_iTimeStep).ToShortDateString())
            g.DrawString(strLabel, tmpFont, brTmp, rc.Width / 2.0!, 15, fmt)

            If (Me.m_ds IsNot Nothing) Then
                strLabel = String.Format("Dataset: '{0}'", Me.m_ds.DisplayName)
                g.DrawString(strLabel, tmpFont, brTmp, rc.Width / 2.0!, 33, fmt)
            End If

        End Sub

    End Class

End Namespace
