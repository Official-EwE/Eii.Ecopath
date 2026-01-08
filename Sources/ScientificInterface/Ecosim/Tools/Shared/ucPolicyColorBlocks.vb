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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On
Imports System.ComponentModel
Imports EwEUtils.Utilities
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region

Namespace Ecosim

    ''' =======================================================================
    ''' <summary>
    ''' Control implementing the policy blocks sketch user interface.
    ''' </summary>
    ''' =======================================================================
    Public Class ucPolicyColorBlocks
        Implements IUIElement

#Region " Private vars "

        Private m_bIsSketching As Boolean
        Private m_bShowTooltip As Boolean = True

        Private m_iRows As Integer
        Private m_iCols As Integer
        Private m_sFirstColWidth As Single
        Private m_sRowHeight As Single
        Private m_sColWidth As Single

        Private m_bIsFirstTimeLoaded As Boolean = True

        Private m_PropBaseYear As cProperty = Nothing
        Private m_PropEcosimNYears As cProperty = Nothing

        Private m_DataSource As IPolicyColorBlockDataSource = Nothing
        Private m_bInit As Boolean

        Private m_ptLast As Point = Nothing

#End Region ' Private vars

#Region " Contruction and destruction "

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

        Protected Overrides Sub Dispose(disposing As Boolean)
            Try

                Me.Detach()
                If disposing AndAlso Me.components IsNot Nothing Then
                    Me.components.Dispose()
                End If
                MyBase.Dispose(disposing)
            Catch ex As Exception
                Debug.Assert(True, ex.Message)
            End Try

        End Sub

#End Region ' Contruction and destruction

#Region " Public interfaces "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Attach a data source <see cref="IPolicyColorBlockDataSource">IPolicyColorBlockDataSource</see> 
        ''' and a block selector control <see cref="IBlockSelector">IBlockSelector</see> to a PolicyColorBlock control.
        ''' </summary>
        ''' <param name="DataSource">Implementation of IPolicyColorBlockDataSource</param>
        ''' <param name="BlockSelector">Implementation of IBlockSelector</param>
        ''' <remarks>
        ''' <para>PolicyColorBlocks can be attached to different data sources and block selectors.</para>
        ''' <para>Remember to correctly <see cref="Detach">detach</see> the control when no longer needed.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub Attach(DataSource As IPolicyColorBlockDataSource, BlockSelector As IBlockSelector)

            If Me.m_bInit Then Me.Detach()

            Me.m_DataSource = DataSource
            Me.ParmBlockCodes = BlockSelector
            Me.ParmBlockCodes.UIContext = Me.UIContext

            Try

                Dim selector As Control = DirectCast(Me.ParmBlockCodes, Control)
                selector.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Top
                selector.Size = Me.m_plBlocks.ClientSize ' Ugh
                Me.m_plBlocks.Controls.Clear()
                Me.m_plBlocks.Controls.Add(selector)

                ' datasource decides if the control panel is visible
                ' JS 22Apr2010: Now panel auto-sizes there is no need for tinkering with column widths.
                '               Added ControlPanelVisible to provide user with design-time control.
                Me.ControlPanelVisible = Me.m_DataSource.isControlPanelVisible

                AddHandler BlockSelector.OnValueChanged, AddressOf Me.onCVValuesChanged

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".Attach() Exception: " & ex.Message)
            End Try

            Me.m_DataSource.Attach(Me.ParmBlockCodes)

            Me.m_PropBaseYear = DirectCast(Me.UIContext.PropertyManager.GetProperty(Me.UIContext.Core.FishingPolicyManager.ObjectiveParameters, eVarNameFlags.SearchBaseYear), cIntegerProperty)
            AddHandler Me.m_PropBaseYear.PropertyChanged, AddressOf Me.OnPropChanged

            Me.m_PropEcosimNYears = DirectCast(Me.UIContext.PropertyManager.GetProperty(Me.UIContext.Core.EcosimModelParameters, eVarNameFlags.EcoSimNYears), cIntegerProperty)
            AddHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf Me.OnPropChanged

            Me.m_bInit = True

            Me.ReloadData()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Detaches the datasource from the control that was previously
        ''' <see cref="Attach">attached</see>.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub Detach()

            If (Me.m_bInit) Then

                RemoveHandler Me.ParmBlockCodes.OnValueChanged, AddressOf Me.onCVValuesChanged
                RemoveHandler Me.m_PropBaseYear.PropertyChanged, AddressOf Me.OnPropChanged
                Me.m_PropBaseYear = Nothing

                RemoveHandler Me.m_PropEcosimNYears.PropertyChanged, AddressOf Me.OnPropChanged
                Me.m_PropEcosimNYears = Nothing

                Me.m_DataSource = Nothing
                Me.ParmBlockCodes = Nothing

            End If

            Me.m_bInit = False
            Me.UIContext = Nothing

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext() As cUIContext Implements IUIElement.UIContext

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the color of the currently selected block.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property CurrentColor() As Color

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the <see cref="IBlockSelector">colour block selector</see>
        ''' to collaborate with this control.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ParmBlockCodes() As IBlockSelector

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether this control should show its controls panel.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property ControlPanelVisible() As Boolean
            Get
                Return Me.m_pnlControls.Visible
            End Get
            Set(value As Boolean)
                Me.m_pnlControls.Visible = value
                Me.m_hdrControls.Visible = value
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set whether the block editor should show tooltips.
        ''' </summary>
        ''' -------------------------------------------------------------------
        <Browsable(True)>
        Public Property ShowTooltip() As Boolean
            Get
                Return Me.m_bShowTooltip
            End Get
            Set(value As Boolean)
                Me.m_bShowTooltip = value
                Me.ProcessMouseHover(Cursor.Position)
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Refresh the content of the control. This will also trigger blocks control
        ''' to reload its datasource and colour scheme.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub Refresh()
            Me.ReloadData()
            Me.UpdateControls()
            MyBase.Refresh()
        End Sub

#End Region ' Public interfaces

#Region " Events handlers "

        Private Sub btnSetEveryGear_Click(sender As System.Object, e As System.EventArgs) _
            Handles m_btnSetGear.Click

            Me.SetSeqColorCodes(2, Me.m_DataSource.TotalBlocks, CInt(Me.m_nudNumYearsPerBlock.Value))

        End Sub

        Private Sub nupSeqStartYear_ValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudSeqStartYear.ValueChanged

            Dim startYear As Integer = CInt(Me.m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(Me.m_nudSeqEndYear.Value)

            Me.m_nudSeqEndYear.Minimum = Me.m_nudSeqStartYear.Value

            If Me.m_bInit Then
                Me.SetSeqColorCodes(startYear, endYear, Me.m_DataSource.TotalBlocks)
            End If
        End Sub

        Private Sub nupSeqEndYear_ValueChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_nudSeqEndYear.ValueChanged

            Dim startYear As Integer = CInt(Me.m_nudSeqStartYear.Value)
            Dim endYear As Integer = CInt(Me.m_nudSeqEndYear.Value)

            Me.m_nudSeqStartYear.Maximum = Me.m_nudSeqEndYear.Value

            If Me.m_bInit Then
                Me.SetSeqColorCodes(startYear, endYear, Me.m_DataSource.TotalBlocks)
            End If

        End Sub

        Private Sub OnPaintBlocks(sender As System.Object, e As PaintEventArgs) _
            Handles m_pbFishingBlocks.Paint

            If (Me.UIContext Is Nothing) Then Return

            Try
                Me.DrawBlocks(e.Graphics)
            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".Paint() Exception: " & ex.Message)
            End Try

        End Sub

        Private Sub OnMouseDownBlocks(sender As System.Object, e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseDown

            If (e.Button And System.Windows.Forms.MouseButtons.Right) > 0 Then
                Me.ProcessMousePickup(e.Location)
            End If

            If (e.Button And System.Windows.Forms.MouseButtons.Left) > 0 Then
                Me.m_bIsSketching = True
                Me.m_DataSource.BatchEdit = True
                Me.m_ptLast = e.Location
                Me.ProcessMouseSketch(e.Location)
            End If

        End Sub

        Private Sub OnMouseMoveBlocks(sender As System.Object, e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseMove

            If Me.m_bIsSketching Then
                Me.ProcessMouseSketch(e.Location)
            End If

            Me.ProcessMouseHover(e.Location)

        End Sub

        Private Sub OnMouseUpBlocks(sender As System.Object, e As MouseEventArgs) _
            Handles m_pbFishingBlocks.MouseUp

            Me.m_bIsSketching = False
            Me.m_DataSource.BatchEdit = False
            Me.m_ptLast = Nothing

        End Sub

        Private Sub OnScrollAreaResized(sender As Object, e As EventArgs) _
            Handles m_plScroll.SizeChanged

            If Not Me.m_bInit Then Return
            Me.ReloadData()

        End Sub

#End Region ' Events handlers

#Region " Callbacks "

        Private Sub OnPropChanged(p As cProperty, cf As cProperty.eChangeFlags)
            Me.Refresh()
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Callback, invoked when values in the grid selector have changed.
        ''' </summary>
        ''' <param name="newValue"></param>
        ''' <param name="Index"></param>
        ''' <remarks>Only the CV grid selector sends out this event.</remarks>
        ''' -------------------------------------------------------------------
        Private Sub onCVValuesChanged(newValue As Single, Index As Integer)
            Try
                ' Update the datasource
                Me.m_DataSource.Update()
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Callbacks

#Region "Private methods"

        Private Sub ReloadData()

            If Not Me.m_bInit Then Return

            Me.m_DataSource.Init()
            Me.ResizeBlocks()

        End Sub

        Private Sub ResizeBlocks()

            If Not Me.m_bInit Then Return

            Dim g As Graphics = Me.m_pbFishingBlocks.CreateGraphics
            Dim szf As SizeF = g.MeasureString("1", Me.Font)

            Try
                ' Determine row height
                Me.m_iRows = Me.m_DataSource.nRows + 1
                Me.m_sRowHeight = szf.Height + 2 ' CSng(Me.m_plScroll.Height / Me.m_iRows)

                Dim sLenMax As Single = -1
                For i As Integer = 0 To Me.m_DataSource.nRows - 1
                    Dim tmpWidth As Single = g.MeasureString(Me.m_DataSource.RowLabel(i + 1), Me.m_pbFishingBlocks.Font).Width
                    If sLenMax < tmpWidth Then sLenMax = tmpWidth
                Next

                ' Determine column widths
                Me.m_sFirstColWidth = sLenMax + 10
                Me.m_iCols = Me.m_DataSource.TotalBlocks

                ' -6 to make this look good when zoomed out all the way. Not sure which padding/margin props to subtract for real
                Dim sMinColWidth As Single = CSng((Me.m_plScroll.ClientRectangle.Width - Me.m_sFirstColWidth - 6) / Me.m_DataSource.TotalBlocks)
                Dim sMaxColWidth As Single = g.MeasureString(CStr(Math.Pow(10, CInt(Math.Log10(Me.m_iCols)) + 1) - 1), Me.Font).Width

                Me.m_sColWidth = sMinColWidth + (sMaxColWidth - sMinColWidth)


            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
                Throw New ApplicationException(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
            End Try

            g.Dispose()
            g = Nothing

            Me.m_pbFishingBlocks.Width = CInt(Math.Ceiling(Me.m_sFirstColWidth + Me.m_iCols * Me.m_sColWidth))
            Me.m_pbFishingBlocks.Height = CInt(Math.Ceiling(Me.m_iRows * Me.m_sRowHeight))
            Me.m_plScroll.Invalidate(True)

            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' Update the enabled state of controls
        ''' </summary>
        Private Sub UpdateControls()

            Me.m_bIsSketching = False

            Me.m_nudSeqEndYear.Maximum = Me.m_DataSource.TotalBlocks
            Me.m_nudNumYearsPerBlock.Maximum = Me.m_DataSource.TotalBlocks

            If Me.CurrentColor = Nothing Then
                Me.CurrentColor = Color.Green
            End If

            Me.m_nudNumYearsPerBlock.Value = CDec(Me.m_DataSource.TotalBlocks)
            Me.m_nudSeqStartYear.Value = CDec(Math.Min(2, Me.m_DataSource.TotalBlocks))
            Me.m_nudSeqEndYear.Value = CDec(Me.m_DataSource.TotalBlocks)
            Me.m_bIsFirstTimeLoaded = False

        End Sub

        Private Sub DrawBlocks(g As Graphics)

            If Not Me.m_bInit Then Return

            Try

                'Draw the blocks first
                For i As Integer = 1 To Me.m_iRows - 1
                    For j As Integer = 1 To Me.m_iCols
                        Dim yPos As Single = i * Me.m_sRowHeight
                        Dim xPos As Single = Me.m_sFirstColWidth + (j - 1) * Me.m_sColWidth
                        ' Ensure proper disposal
                        Using tmpBrush As New SolidBrush(Me.ParmBlockCodes.BlockColor(Me.m_DataSource.BlockCells(i, j)))
                            g.FillRectangle(tmpBrush, New RectangleF(xPos, yPos, Me.m_sColWidth, Me.m_sRowHeight))
                        End Using
                    Next
                Next

                ' Draw names area in correct style guide colour
                Using br As New SolidBrush(Me.UIContext.StyleGuide.ApplicationColor(cStyleGuide.eApplicationColorType.NAMES_BACKGROUND))
                    g.FillRectangle(br, 0, Me.m_sRowHeight, Me.m_sFirstColWidth, Me.m_pbFishingBlocks.Height - Me.m_sRowHeight)
                End Using

                'Now draw the grid lines on top of the blocks, so they show up
                'Rows
                Dim tSize As SizeF = g.MeasureString("T", Me.Font)
                Dim gridPen As Pen = SystemPens.ControlDark

                For i As Integer = 1 To Me.m_iRows - 1
                    Dim yPos As Single = i * Me.m_sRowHeight
                    g.DrawLine(gridPen, 0, yPos, Me.m_pbFishingBlocks.Width, yPos)
                    g.DrawLine(gridPen, Me.m_sFirstColWidth, yPos, Me.m_pbFishingBlocks.Width, yPos)
                    'draw the label in the middle
                    g.DrawString(Me.m_DataSource.RowLabel(i), Me.m_pbFishingBlocks.Font, Brushes.Black, 1, yPos + Me.m_sRowHeight * 0.5F - tSize.Height * 0.5F)
                Next
                g.DrawLine(gridPen, 0, Me.m_iRows * Me.m_sRowHeight, Me.m_pbFishingBlocks.Width, Me.m_iRows * Me.m_sRowHeight)

                'Cols
                For j As Integer = 1 To Me.m_iCols
                    Dim xPos As Single = Me.m_sFirstColWidth + (j - 1) * Me.m_sColWidth
                    g.DrawLine(gridPen, xPos, 0, xPos, Me.m_sRowHeight)
                    g.DrawLine(gridPen, xPos, Me.m_sRowHeight, xPos, Me.m_pbFishingBlocks.Height)
                    Dim txt As String = j.ToString
                    g.DrawString(txt, Me.m_pbFishingBlocks.Font, Brushes.Black, New Rectangle(CInt(xPos), 0, CInt(xPos + Me.m_sColWidth), CInt(Me.m_sRowHeight)))
                Next
                g.DrawLine(gridPen, Me.m_sFirstColWidth + Me.m_iCols * Me.m_sColWidth, 0, Me.m_sFirstColWidth + Me.m_iCols * Me.m_sColWidth, Me.m_pbFishingBlocks.Height)

            Catch ex As Exception
                System.Console.WriteLine(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
                Throw New ApplicationException(Me.ToString & ".DrawRowCols() Exception: " & ex.Message)
            End Try

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Pick a block colour from the current mouse location.
        ''' </summary>
        ''' <param name="ptCursor"></param>
        ''' -------------------------------------------------------------------
        Private Sub ProcessMousePickup(ptCursor As Point)

            Dim ptBlock As Point = Me.CursorToBlock(ptCursor)
            If ptBlock.Y < 0 Or ptBlock.Y > Me.m_iRows - 1 Then Return
            If ptBlock.X > Me.m_iCols Then Return

            Dim iBlock As Integer = Me.m_DataSource.BlockCells(ptBlock.Y, ptBlock.X)
            Me.ParmBlockCodes.SelectedBlock = iBlock

        End Sub

        Private Sub ProcessMouseSketch(ptCursor As Point)

            If Not Me.m_bInit Then Return

            Dim ptFrom As Point = Me.CursorToBlock(Me.m_ptLast)
            Dim ptTo As Point = Me.CursorToBlock(ptCursor)
            Dim dx As Integer = ptTo.X - ptFrom.X
            Dim dy As Integer = ptTo.Y - ptFrom.Y
            Dim dCLick As Integer = CInt(Math.Sqrt(dx * dx + dy * dy))

            If dCLick = 0 Then dCLick = 1

            For hmm As Integer = 0 To dCLick

                Dim ptBlock As New Point(CInt(ptFrom.X + (hmm * dx) / dCLick), CInt(ptFrom.Y + (hmm * dy) / dCLick))

                If ptBlock.Y >= 0 And ptBlock.Y <= Me.m_iRows - 1 Then
                    If ptBlock.X <= Me.m_iCols Then

                        ' Is row header clicked?
                        If (ptBlock.X < 1) Then

                            ' #Yes: is column header clicked? If so: cannot fill block row
                            If ptBlock.Y < 1 Then Return

                            For i As Integer = 1 To Me.m_DataSource.BlockCells.GetLength(1) - 1
                                Me.FillBlock(ptBlock.Y, i)
                            Next
                        Else
                            ' Is column header clicked?
                            If (ptBlock.Y < 1) Then
                                ' #Yes: is row header clicked? If so: cannot fill block column
                                If (ptBlock.X < 1) Then Return
                                For i As Integer = 1 To Me.m_DataSource.BlockCells.GetLength(0) - 1
                                    Me.FillBlock(i, ptBlock.X)
                                Next
                            Else
                                Me.FillBlock(ptBlock.Y, ptBlock.X)
                            End If
                        End If
                    End If
                End If
            Next

            Me.m_ptLast = ptCursor
            Me.m_pbFishingBlocks.Invalidate()

        End Sub

        Private Sub ProcessMouseHover(ptCursor As Point)

            Dim ptBlock As Point = Me.CursorToBlock(ptCursor)

            If (ptBlock.Y < 1 Or ptBlock.Y >= Me.m_iRows) Then Return
            If (ptBlock.X < 1 Or ptBlock.X > Me.m_iCols) Then Return

            If ptCursor.X = Me.m_ptLast.X And ptCursor.Y = Me.m_ptLast.Y Then Return

            Dim iBlock As Integer = Me.m_DataSource.BlockCells(ptBlock.Y, ptBlock.X)
            Dim strValue As String = ""

            ' Is a block defined for this position?
            If (iBlock > 0) Then
                ' #Yes: get block value
                strValue = cStringUtils.FormatSingle(Me.m_DataSource.BlockToValue(iBlock))
            Else
                ' #No: get 'not used' value
                strValue = SharedResources.GENERIC_VALUE_NOTUSED
            End If

            ' Format tooltip as as "value (x, y)"
            strValue = String.Format(SharedResources.GENERIC_LABEL_POINT, strValue, _
                                     Me.m_DataSource.RowLabel(ptBlock.Y), ptBlock.X)

            cToolTipShared.GetInstance().SetToolTip(Me.m_pbFishingBlocks, strValue)

            Me.m_ptLast = ptCursor

        End Sub

        Private Sub FillBlock(iRow As Integer, iCol As Integer)

            If Not Me.m_bInit Then Return
            Me.m_DataSource.FillBlock(iRow, iCol)

        End Sub

        Private Sub SetSeqColorCodes(startYear As Integer, endYear As Integer, yearPerBlock As Integer)

            If Not Me.m_bInit Then Return
            If Me.m_bIsFirstTimeLoaded Then Return

            Me.m_DataSource.SetSeqColorCodes(startYear, endYear, yearPerBlock)
            Me.m_pbFishingBlocks.Invalidate()

        End Sub

        Private Function CursorToBlock(ptCursor As Point) As Point
            Try
                If Me.m_sRowHeight > 0 And Me.m_sColWidth > 0 Then
                    Dim iRow As Integer = CInt(Math.Floor(ptCursor.Y / Me.m_sRowHeight))
                    Dim iCol As Integer = CInt(Math.Floor((ptCursor.X - Me.m_sFirstColWidth) / Me.m_sColWidth) + 1)

                    Return New Point(iCol, iRow)
                End If
            Catch ex As Exception

            End Try
            Return New Point(-1, -1)
        End Function

#End Region

    End Class

End Namespace

