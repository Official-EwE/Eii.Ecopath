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
Imports System.Drawing.Drawing2D
Imports EwECore
Imports EwECore.SpatialData
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEPlugin

#End Region ' Imports

Namespace Ecospace.Controls

    Public Class ucSpatialTimeSeriesToolbox
        Implements IUIElement

#Region " Private classes "

        Private Class cDatasetPos
            Public m_ds As ISpatialDataSet
            Public m_iTimeStart As Integer = 0
            Public m_iTimeEnd As Integer = 0
            Public m_iPosVert As Integer = 0
            Public m_liData As New List(Of Integer) ' Time steps with data
        End Class

#End Region ' Private classes

#Region " Private vars "

        Private Const c_headerheight As Integer = 18
        Private Const c_barheight As Integer = 24
        Private Const c_barlabelheight As Integer = 18
        Private Const c_barmargin As Integer = 3
        Private Const c_dotradius As Integer = 2

        Private m_uic As cUIContext = Nothing
        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_lPos As New List(Of cDatasetPos)
        Private m_iTimestepSize As Integer = 1
        Private m_iSelectedIndex As Integer = -1
        Private m_iSelectedTimeStep As Integer = -1

#End Region ' Private vars

#Region " Construction "

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
        End Sub

#End Region ' Construction

#Region " Properties "

        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Public Property VarName As eVarNameFlags
            Get
                Return Me.m_varname
            End Get
            Set(value As eVarNameFlags)
                If (Me.m_varname = value) Then Return
                Me.m_varname = value
                Me.RecalcLayout()
            End Set
        End Property

        Public Event OnSelectedDatasetChanged(owner As Object, ds As ISpatialDataSet)

        Public Property SelectedIndex As Integer
            Get
                Return Me.m_iSelectedIndex
            End Get
            Set(value As Integer)
                Me.m_iSelectedIndex = value
                Me.Invalidate()

                If (Me.UIContext Is Nothing) Then Return

                Dim ds As ISpatialDataSet = Nothing
                If (Me.m_iSelectedIndex >= 0) Then ds = Me.m_lPos(Me.m_iSelectedIndex).m_ds
                Try
                    RaiseEvent OnSelectedDatasetChanged(Me, ds)
                Catch ex As Exception

                End Try
            End Set
        End Property

        Public Event OnSelectedTimestepChanged(owner As Object, iTimeStep As Integer, dt As DateTime)

        Public Property SelectedTimeStep As Integer
            Get
                Return Me.m_iSelectedTimeStep
            End Get
            Set(value As Integer)
                Me.m_iSelectedTimeStep = value
                Me.Invalidate()

                If (Me.UIContext Is Nothing) Then Return

                Try
                    RaiseEvent OnSelectedTimestepChanged(Me, Me.m_iSelectedTimeStep, Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(Me.m_iSelectedTimeStep))
                Catch ex As Exception

                End Try
            End Set
        End Property

        Public Sub RefreshContent()
            Me.RecalcSize()
            Me.RecalcLayout()
            Me.Invalidate()
        End Sub

#End Region ' Properties

#Region " Form overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            If (Me.m_uic Is Nothing) Then Return
            Me.RefreshContent()
        End Sub

        Protected Overrides Sub OnResize(e As System.EventArgs)
            MyBase.OnResize(e)
            Me.RecalcSize()
            Me.Invalidate(True)
        End Sub

        Protected Overrides Sub OnScroll(se As System.Windows.Forms.ScrollEventArgs)
            Me.Invalidate()
            MyBase.OnScroll(se)
        End Sub

        Protected Overrides Sub OnMouseClick(e As System.Windows.Forms.MouseEventArgs)
            Dim pos As cDatasetPos = Me.DatasetFromPoint(e.Location)
            If (pos IsNot Nothing) Then
                Me.SelectedIndex = pos.m_iPosVert
            End If
            Me.SelectedTimeStep = TimestepFromPoint(e.Location)
            MyBase.OnMouseClick(e)
        End Sub

        Protected Overrides Sub OnMouseDoubleClick(e As System.Windows.Forms.MouseEventArgs)
            Dim pos As cDatasetPos = Me.DatasetFromPoint(e.Location)
            If (pos IsNot Nothing) Then
                Dim ds As ISpatialDataSet = pos.m_ds
                If (TypeOf ds Is IConfigurablePlugin) Then
                    Dim dsConf As IConfigurablePlugin = DirectCast(ds, IConfigurablePlugin)
                    Dim ctrl As Control = dsConf.GetConfigUI()

                    If (ctrl IsNot Nothing) Then
                        Dim dlg As New dlgConfig()
                        dlg.ShowDialog(Me.FindForm, My.Resources.CAPTION_EXTERNAL_DATASET_CONFIGURE, ctrl)
                    End If

                    Me.RecalcLayout()
                End If
            End If
            MyBase.OnMouseDoubleClick(e)
        End Sub

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Dim rmp As New cEwEColorRamp()
            rmp.ColorOffsetStart = 0.2

            e.Graphics.Clear(Me.BackColor)

            Try

                ' Paint matrix shifted to x and Y scroll position
                e.Graphics.Transform = New Matrix(1, 0, 0, 1, AutoScrollPosition.X, AutoScrollPosition.Y)
                Me.PaintGrid(e.Graphics, New Rectangle(0, c_headerheight, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, Me.ClientRectangle.Height - c_headerheight))
                For i As Integer = 0 To Me.m_lPos.Count - 1
                    Me.PaintDataset(e.Graphics, Me.m_lPos(i), i = Me.m_iSelectedIndex, rmp.GetColor(i / Me.m_lPos.Count))
                Next
                e.Graphics.ResetTransform()

                ' Paint header at the top of the visible scroll area
                e.Graphics.Transform = New Matrix(1, 0, 0, 1, AutoScrollPosition.X, 0)
                Me.PaintHeader(e.Graphics, New Rectangle(0, 0, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, c_headerheight))
                e.Graphics.ResetTransform()

            Catch ex As Exception
                Debug.Assert(False)
            End Try

        End Sub

#End Region ' Form overrides

#Region " Internals "

        ' ToDo: respond to core messages to update ecospace run time, dataset changes

        Protected Sub RecalcSize()
            ' Safety check
            If (Me.m_uic Is Nothing) Then Return
            ' Calc number of pixels per time step
            Me.m_iTimestepSize = CInt(Math.Max(2, Math.Floor(Me.Width / Me.m_uic.Core.nEcospaceTimeSteps)))

            ' ToDo: put vert scrollbar UNDER header panel, not beside header panel

            Me.AutoScroll = True
            Me.AutoScrollMinSize = New Size(Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, (Me.m_lPos.Count * (c_barheight + 2 * c_barmargin) + c_headerheight))
            Me.AutoScrollMargin = New Size(0, 0)
            'Me.Invalidate()
        End Sub

        ''' <summary>
        ''' Calculate dataset display rectangles
        ''' </summary>
        Protected Sub RecalcLayout()

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim conn As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager()
            Dim lAdt As New List(Of cSpatialDataAdapter)
            Dim ds As ISpatialDataSet = Nothing
            Dim iRow As Integer = 0
            Dim ptfTL As PointF = bm.PosTopLeft
            Dim ptfBR As PointF = bm.PosBottomRight

            ' Resolve varname
            If (Me.m_varname = eVarNameFlags.NotSet) Then
                lAdt.AddRange(conn.Adapters)
            Else
                lAdt.Add(conn.Adapter(Me.m_varname))
            End If

            Me.m_lPos.Clear()

            For Each adt As cSpatialDataAdapter In lAdt
                For i As Integer = 0 To adt.Length - 1
                    If adt.IsConnected(i) Then

                        ds = adt.Dataset(i)

                        Dim pos As New cDatasetPos()
                        pos.m_ds = ds
                        pos.m_iPosVert = iRow

                        If ds.TimeStart = Date.MinValue Then
                            pos.m_iTimeStart = 1
                        Else
                            pos.m_iTimeStart = core.AbsoluteTimeToEcospaceTimestep(ds.TimeStart)
                        End If

                        If ds.TimeEnd = Date.MaxValue Then
                            pos.m_iTimeEnd = core.nEcospaceTimeSteps
                        Else
                            pos.m_iTimeEnd = Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(ds.TimeEnd)
                        End If

                        For iStep As Integer = pos.m_iTimeStart To pos.m_iTimeEnd
                            If ds.HasDataAtT(core.EcospaceTimestepToAbsoluteTime(iStep), ptfTL, ptfBR) Then
                                pos.m_liData.Add(iStep)
                            End If
                        Next

                        Me.m_lPos.Add(pos)
                        iRow += 1
                    End If
                Next
            Next
            Me.Invalidate()

        End Sub

        ''' <summary>
        ''' Paint the header row
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        ''' <remarks></remarks>
        Private Sub PaintHeader(g As Graphics, rc As Rectangle)

            g.FillRectangle(SystemBrushes.Control, rc)

            Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
            Dim core As cCore = Me.m_uic.Core
            Dim sStepsPerYear As Single = CSng(Me.m_uic.Core.nEcospaceTimeSteps / Math.Max(1, Me.m_uic.Core.nEcospaceYears))

            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                For i As Integer = 0 To Me.m_uic.Core.nEcospaceYears Step 5
                    Dim sx As Single = i * sStepsPerYear * Me.m_iTimestepSize
                    g.DrawString(CStr(iYear + i), ft, SystemBrushes.ControlText, sx, 0.0!)
                    g.DrawLine(SystemPens.ControlLightLight, rc.X + sx, rc.Y, rc.X + sx, rc.Y + rc.Height)
                Next
            End Using

        End Sub

        ''' <summary>
        ''' Draw a grid of vertical lines for every 5 years
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        Private Sub PaintGrid(g As Graphics, rc As Rectangle)

            Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
            Dim core As cCore = Me.m_uic.Core
            Dim sStepsPerYear As Single = CSng(Me.m_uic.Core.nEcospaceTimeSteps / Math.Max(1, Me.m_uic.Core.nEcospaceYears))

            Using p As New Pen(SystemColors.ControlDarkDark, 1)
                p.DashStyle = DashStyle.Dot
                For i As Integer = 0 To Me.m_uic.Core.nEcospaceYears Step 5
                    Dim sx As Single = i * sStepsPerYear * Me.m_iTimestepSize
                    g.DrawLine(p, rc.X + sx, rc.Y, sx, rc.Y + rc.Height)
                Next
            End Using

            Using p As New Pen(Me.m_uic.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.HIGHLIGHT), 2)
                Dim sx As Single = Me.m_iSelectedTimeStep * Me.m_iTimestepSize
                g.DrawLine(p, rc.X + sx, rc.Y, sx, rc.Y + rc.Height)
            End Using

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="pos"></param>
        ''' <remarks></remarks>
        Private Sub PaintDataset(ByVal g As Graphics, _
                                 ByVal pos As cDatasetPos, _
                                 ByVal bSelected As Boolean, _
                                 ByVal clr As Color)

            Dim rcBar As Rectangle = Me.DatasetArea(pos)
            Dim rcBack As Rectangle = New Rectangle(0, rcBar.Y - c_barmargin, Me.ClientRectangle.Width, rcBar.Height + 2 * c_barmargin)
            Dim rcLabel As New Rectangle(rcBar.X, rcBar.Y, rcBar.Width, c_barlabelheight)
            Dim rcTimeStep As New Rectangle(rcBar.X - c_dotradius, rcBar.Y + c_barheight - CInt((c_barheight - c_barlabelheight) / 2) - c_dotradius, 2 * c_dotradius, 2 * c_dotradius)
            Dim clrFill As Color = EwEUtils.Utilities.cColorUtils.GetVariant(clr, 0.5)
            Dim clrData As Color = EwEUtils.Utilities.cColorUtils.GetVariant(clr, -0.5)
            Dim clrText As Color = SystemColors.ControlText
            Dim clrTextFill As Color = clr

            Dim fmt As New StringFormat(StringFormatFlags.NoWrap)
            fmt.LineAlignment = StringAlignment.Center

            If bSelected Then
                clrText = SystemColors.HighlightText
                clrTextFill = SystemColors.Highlight
                Using br As New SolidBrush(SystemColors.MenuHighlight)
                    g.FillRectangle(br, rcBar)
                End Using
            End If

            ' Fill area bar
            Using br As New SolidBrush(clrFill)
                g.FillRectangle(br, rcBar)
            End Using
            Using p As New Pen(clr)
                g.DrawRectangle(p, rcBar)
            End Using
            Using br As New SolidBrush(clrTextFill)
                g.FillRectangle(br, rcLabel)
            End Using
            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                rcLabel.Width = rcBack.Width
                g.DrawString(pos.m_ds.DisplayName, ft, SystemBrushes.ControlText, rcLabel, fmt)
            End Using
            Using br As New SolidBrush(clrData)
                For Each iStep As Integer In pos.m_liData
                    rcTimeStep.X = rcBar.X + (iStep - pos.m_iTimeStart) * Me.m_iTimestepSize
                    g.FillEllipse(br, rcTimeStep)
                Next
            End Using

        End Sub

        Private Function DatasetArea(pos As cDatasetPos) As Rectangle
            Dim iStart As Integer = pos.m_iTimeStart * Me.m_iTimestepSize
            Dim iEnd As Integer = (pos.m_iTimeEnd + 1) * Me.m_iTimestepSize - 1
            Return New Rectangle(iStart, c_headerheight + pos.m_iPosVert * (c_barheight + 2 * c_barmargin) + c_barmargin, iEnd - iStart, c_barheight)
        End Function

        Private Function TimestepFromPoint(pt As Point) As Integer
            If (Me.m_iTimestepSize = 0) Then Return -1
            Return CInt(Math.Round(pt.X / Me.m_iTimestepSize))
        End Function

        Private Function DatasetFromPoint(pt As Point) As cDatasetPos
            If (pt.Y < c_headerheight) Then Return Nothing
            For Each pos As cDatasetPos In Me.m_lPos
                If Me.DatasetArea(pos).Contains(pt) Then Return pos
            Next
            Return Nothing
        End Function

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls