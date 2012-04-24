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
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports EwEUtils.SpatialData
Imports EwECore.SpatialData
Imports EwECore

#End Region ' Imports

Namespace Ecospace

    Public Class ucSpatialTimeSeriesMap
        Implements IUIElement

        Private m_ds As ISpatialDataSet = Nothing
        Private m_uic As cUIContext = Nothing
        Private m_lDataRects As New List(Of RectangleF)
        Private m_rcMap As RectangleF

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As cUIContext)
                Me.m_uic = value
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

        Public Sub RefreshContent()

            If (Me.m_uic Is Nothing) Then Return

            Dim bm As cEcospaceBasemap = Me.m_uic.Core.EcospaceBasemap

            Me.m_rcMap = Me.ToRect(bm.PosTopLeft, bm.PosBottomRight)
            Me.m_lDataRects.Clear()

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
                    Me.m_lDataRects.Add(ToRect(ptfTL, ptfBR))
                End If
            Next

        End Sub

        Private Function ToRect(ptfTL As PointF, ptfBR As PointF) As RectangleF
            Return New RectangleF(ptfTL.X, ptfTL.Y, ptfBR.X - ptfTL.X, ptfTL.Y - ptfBR.Y)
        End Function

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            If (Me.m_uic Is Nothing) Then Return

            Dim rcfView As RectangleF = Me.m_rcMap
            For Each rcf As RectangleF In Me.m_lDataRects
                Dim ptfTL As New PointF(Math.Min(rcf.Left, rcfView.Left), Math.Max(rcf.Top, rcfView.Top))
                Dim ptfBR As New PointF(Math.Max(rcf.Right, rcfView.Right), Math.Min(rcf.Bottom, rcfView.Bottom))
                rcfView = ToRect(ptfTL, ptfBR)
            Next

            If (rcfView.Width = 0 Or rcfView.Height = 0) Then Return

            Dim sScale As Single = CSng(Math.Min(Me.Height / rcfView.Height, Me.Width / rcfView.Width))
            e.Graphics.TranslateTransform(rcfView.X, -rcfView.Y)
            e.Graphics.ScaleTransform(sScale, -sScale)

            Using p As New Pen(Brushes.Red, 0.001)
                e.Graphics.DrawRectangles(p, New RectangleF() {Me.m_rcMap})
            End Using

            If (Me.m_lDataRects.Count > 0) Then
                Using p As New Pen(Brushes.Gray, 0.001)
                    e.Graphics.DrawRectangles(p, Me.m_lDataRects.ToArray)
                End Using
            End If

        End Sub

    End Class

End Namespace
