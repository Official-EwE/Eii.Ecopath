' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing.Drawing2D
Imports EwECore.Common
Imports EwECore.SpatialData
Imports ScientificInterfaceShared.Utilities



Namespace Ecospace.Controls

    Public Class ucSpatialTimeSeriesToolbox
        Implements IUIElement

#Region " Private classes "

        ''' <summary>
        ''' Helper administration class for a data set in the toolbox
        ''' </summary>
        Private Class cDatasetInfo
            Public Property Dataset As ISpatialDataSet
            Public Property VarName As eVarNameFlags
            Public Property Guid As Guid
            Public Property AppliedDataStart As Integer = 0
            Public Property DataStart As Integer = 0
            Public Property DataEnd As Integer = 0
            Public Property AppliedDataEnd As Integer = 0
            Public Property PosVert As Integer = 0
            ''' <summary>Time steps with data from the dataset.</summary>
            Public Property DataPoint As New List(Of Integer)
            ''' <summary>Time steps with repeated data from the dataset.</summary>
            Public Property BorrowedDataPoint As New List(Of Integer)
            ''' <summary>The original data the point borrows from.</summary>
            Public Property BorrowedDataFrom As New List(Of Integer)

        End Class

#End Region ' Private classes

#Region " Private vars "

        ' Formatting constants
        Private Const c_headerheight As Integer = 18
        Private Const c_barheight As Integer = 24
        Private Const c_barlabelheight As Integer = 18
        Private Const c_barmargin As Integer = 3
        Private Const c_dotradius As Integer = 2
        Private Const c_imgradius As Integer = 4

        Private m_uic As cUIContext = Nothing
        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_lInfo As New List(Of cDatasetInfo)
        Private m_iTimestepSize As Integer = 0 ' Will be calculated, should perhaps be configurable
        Private m_iSelectedIndex As Integer = -1

        Private m_mhPath As cMessageHandler = Nothing
        Private m_mhSim As cMessageHandler = Nothing
        Private m_mhSpace As cMessageHandler = Nothing

        Private m_manSets As cSpatialDataSetManager = Nothing

        Private m_bmpError As Bitmap
        Private m_bmpWarning As Bitmap

#End Region ' Private vars

#Region " Construction / destruction "

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, True)
            Me.m_bmpError = New Bitmap(ScientificInterfaceShared.My.Resources.Critical, c_imgradius * 2, c_imgradius * 2)
            Me.m_bmpWarning = New Bitmap(ScientificInterfaceShared.My.Resources.Warning, c_imgradius * 2, c_imgradius * 2)
        End Sub

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso Me.components IsNot Nothing Then
                    Me.components.Dispose()
                    Me.m_bmpError.Dispose()
                    Me.m_bmpError = Nothing
                    Me.m_bmpWarning.Dispose()
                    Me.m_bmpWarning = Nothing
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Construction / destruction

#Region " Properties "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="IUIElement.UIContext"/>
        ''' -------------------------------------------------------------------
        Public Property UIContext As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(value As ScientificInterfaceShared.Controls.cUIContext)

                ' Clean up
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhPath)
                    Me.m_mhPath = Nothing
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhSim)
                    Me.m_mhSim = Nothing
                    Me.m_uic.Core.Messages.RemoveMessageHandler(Me.m_mhSpace)
                    Me.m_mhSpace = Nothing
                End If

                ' Update
                Me.m_uic = value

                ' Config
                If (Me.m_uic IsNot Nothing) Then
                    Me.m_mhPath = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecopath, eMessageType.Any, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhPath)
                    Me.m_mhSim = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecosim, eMessageType.Any, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhSim)
                    Me.m_mhSpace = New cMessageHandler(AddressOf Me.OnCoreMessage, eCoreComponentType.Ecospace, eMessageType.Any, Me.m_uic.SyncObject)
                    Me.m_uic.Core.Messages.AddMessageHandler(Me.m_mhSpace)
#If DEBUG Then
                    Me.m_mhPath.Name = "ucSpatialTimeSeriesToolbox::m_mhPath"
                    Me.m_mhSim.Name = "ucSpatialTimeSeriesToolbox::m_mhSim"
                    Me.m_mhSpace.Name = "ucSpatialTimeSeriesToolbox::m_mhSpace"
#End If
                End If
            End Set
        End Property

        Public Property Filter As eVarNameFlags
            Get
                Return Me.m_varname
            End Get
            Set(value As eVarNameFlags)
                If (Me.m_varname = value) Then Return
                Me.m_varname = value
                Me.RecalcLayout()
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Selection changed notification event
        ''' </summary>
        ''' <param name="owner">The sender of this event</param>
        ''' <param name="ds">The selected datasets</param>
        ''' -------------------------------------------------------------------
        Public Event OnSelectedDatasetChanged(owner As Object, ds As ISpatialDataSet)

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the zero-based index of the selected <see cref="ISpatialDataSet"/>. 
        ''' This value cannot be equal to or exceed <see cref="cSpatialDataSetManager.Count"/>
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property SelectedDatasetIndex As Integer
            Get
                Return Me.m_iSelectedIndex
            End Get
            Set(value As Integer)

                Me.m_iSelectedIndex = Math.Min(Me.m_lInfo.Count - 1, Math.Max(-1, value))
                Me.Invalidate()

                If (Me.UIContext Is Nothing) Then Return

                Dim ds As ISpatialDataSet = Nothing
                If (Me.m_iSelectedIndex >= 0) Then ds = Me.m_lInfo(Me.m_iSelectedIndex).Dataset
                Try
                    RaiseEvent OnSelectedDatasetChanged(Me, ds)
                Catch ex As Exception

                End Try
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Entirely refresh the content of the toolbox.
        ''' </summary>
        ''' -------------------------------------------------------------------
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
            Me.m_manSets = Me.m_uic.Core.SpatialDataConnectionManager.DatasetManager
            Me.RefreshContent()

        End Sub

        Protected Overrides Sub OnResize(e As System.EventArgs)
            MyBase.OnResize(e)
            Me.RecalcSize()
            Me.Invalidate(True)
        End Sub

        Protected Overrides Sub OnMouseDoubleClick(e As System.Windows.Forms.MouseEventArgs)

            'MyBase.OnMouseDoubleClick(e)

            Dim info As cDatasetInfo = Me.DatasetFromPoint(e.Location)
            If (info Is Nothing) Then Return

            Dim cmd As cEditSpatialDatasetCommand = CType(Me.UIContext.CommandHandler.GetCommand(cEditSpatialDatasetCommand.COMMAND_NAME), cEditSpatialDatasetCommand)
            cmd.Invoke(info.Dataset)

        End Sub

        Protected Overrides Sub OnScroll(se As System.Windows.Forms.ScrollEventArgs)
            Me.Invalidate()
            MyBase.OnScroll(se)
        End Sub

        Protected Overrides Sub OnMouseClick(e As System.Windows.Forms.MouseEventArgs)
            Dim ptClick As New Point(e.Location.X - Me.AutoScrollPosition.X, e.Location.Y - Me.AutoScrollPosition.Y)
            Dim pos As cDatasetInfo = Me.DatasetFromPoint(ptClick)
            If (pos IsNot Nothing) Then
                Me.SelectedDatasetIndex = pos.PosVert
            End If
            MyBase.OnMouseClick(e)
        End Sub

        Private m_strTipText As String = ""

        Protected Overrides Sub OnMouseMove(e As System.Windows.Forms.MouseEventArgs)

            Dim ptClick As New Point(e.Location.X - Me.AutoScrollPosition.X, e.Location.Y - Me.AutoScrollPosition.Y)
            Dim pos As cDatasetInfo = Me.DatasetFromPoint(ptClick)
            Dim strText As String = ""

            If (pos IsNot Nothing) Then
                Dim comp As cDatasetCompatilibity = Me.m_manSets.Compatibility(pos.Dataset)
                Dim iStep As Integer = Me.TimestepFromPoint(ptClick)
                Dim dtStep As Date = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iStep)
                Dim strDate As String = dtStep.ToShortDateString

                Dim iBorrowed As Integer = pos.BorrowedDataPoint.IndexOf(iStep)
                If (iBorrowed > -1) Then
                    dtStep = Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iBorrowed)
                    strText = String.Format("Data for {0} borrowed from {1}", strDate, Me.m_uic.Core.EcospaceTimestepToAbsoluteTime(iBorrowed).ToShortDateString)
                Else
                    Select Case comp.CompatibilityAt(iStep)

                        Case cDatasetCompatilibity.eCompatibilityTypes.NoTemporal,
                             cDatasetCompatilibity.eCompatibilityTypes.NotSet
                            strText = pos.Dataset.CustomName

                        Case cDatasetCompatilibity.eCompatibilityTypes.Errors
                            strText = String.Format(My.Resources.SPATIALTEMP_STATUS_T_MISSING, pos.Dataset.CustomName, iStep, strDate)

                        Case cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
                            strText = String.Format(My.Resources.SPATIALTEMP_STATUS_T_NOSPATIAL, pos.Dataset.CustomName, iStep, strDate)

                        Case cDatasetCompatilibity.eCompatibilityTypes.PartialSpatial
                            strText = String.Format(My.Resources.SPATIALTEMP_STATUS_T_PARTIALSPATIAL, pos.Dataset.CustomName, iStep, strDate)

                        Case cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap
                            strText = String.Format(My.Resources.SPATIALTEMP_STATUS_T_FULLSPATIAL, pos.Dataset.CustomName, iStep, strDate)

                        Case cDatasetCompatilibity.eCompatibilityTypes.TemporalNotIndexed
                            strText = String.Format(My.Resources.SPATIALTEMP_STATUS_T_UNKNOWN, pos.Dataset.CustomName, iStep, strDate)

                    End Select
                End If
            End If

            ' Async update to prevent flickering
            If (strText <> Me.m_strTipText) Then
                Me.m_strTipText = strText
                Me.BeginInvoke(New MethodInvoker(AddressOf Me.UpdateTooltip))
            End If

        End Sub

        Private Sub UpdateTooltip()
            cToolTipShared.GetInstance().SetToolTip(Me, Me.m_strTipText)
        End Sub

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)

            MyBase.OnPaint(e)

            If (Me.m_uic Is Nothing) Then Return

            e.Graphics.Clear(Me.BackColor)
            Try

                ' Paint matrix shifted to X and Y scroll position
                e.Graphics.Transform = New Matrix(1, 0, 0, 1, Me.AutoScrollPosition.X, Me.AutoScrollPosition.Y)
                For i As Integer = 0 To Me.m_lInfo.Count - 1
                    Me.DrawDatasetIndicator(e.Graphics, Me.m_lInfo(i), i = Me.m_iSelectedIndex)
                Next
                Me.DrawGrid(e.Graphics, New Rectangle(0, c_headerheight, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, Me.ClientRectangle.Height - c_headerheight))
                For i As Integer = 0 To Me.m_lInfo.Count - 1
                    Me.DrawDataset(e.Graphics, Me.m_lInfo(i), i = Me.m_iSelectedIndex)
                Next
                e.Graphics.ResetTransform()

                ' Paint header at the top of the visible scroll area
                e.Graphics.Transform = New Matrix(1, 0, 0, 1, Me.AutoScrollPosition.X, 0)
                Me.DrawGridHeader(e.Graphics, New Rectangle(0, 0, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, c_headerheight))
                e.Graphics.ResetTransform()

            Catch ex As Exception
                Debug.Assert(False)
            End Try

        End Sub

#End Region ' Form overrides

#Region " Events "

        Private Sub OnCoreMessage(ByRef msg As cMessage)

            Select Case msg.DataType
                Case eDataTypes.EwEModel
                    ' Respond to Ecopath first year changes
                    If (msg.Type = eMessageType.DataValidation) Then
                        Me.RefreshContent()
                    End If
                Case eDataTypes.TimeSeriesDataset
                    ' Respond to Ecosim start year changes
                    If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                        Me.Invalidate()
                    End If
                Case eDataTypes.EcospaceSpatialDataConnection
                    If (msg.Type = eMessageType.Progress) Then
                        Me.Invalidate()
                    Else
                        Me.RefreshContent()
                    End If
                Case eDataTypes.EcoSpaceScenario, eDataTypes.EcospaceModelParameter
                    Me.RefreshContent()
                Case eDataTypes.EcospaceSpatialDataSource
                    Me.RefreshContent()
            End Select

        End Sub

#End Region ' Events

#Region " Internals "

        Protected Sub RecalcSize()

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return
            ' Calc number of pixels per time step
            Me.m_iTimestepSize = CInt(Math.Max(4, Math.Floor(Me.Width / Me.m_uic.Core.nEcospaceTimeSteps)))

            Me.AutoScroll = True
            Me.AutoScrollMinSize = New Size(Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, (Me.m_lInfo.Count * (c_barheight + 2 * c_barmargin) + c_headerheight))
            Me.AutoScrollMargin = New Size(0, 0)

        End Sub

        ''' <summary>
        ''' Calculate dataset display rectangles
        ''' </summary>
        Protected Sub RecalcLayout()

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Dim core As cCore = Me.m_uic.Core
            Dim bm As cEcospaceBasemap = core.EcospaceBasemap
            Dim man As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager()
            Dim lAdt As New List(Of cSpatialDataAdapter)
            Dim iRow As Integer = 0
            Dim ptfTL As PointF = bm.PosTopLeft
            Dim ptfBR As PointF = bm.PosBottomRight

            ' Resolve varname
            If (Me.m_varname = eVarNameFlags.NotSet) Then
                lAdt.AddRange(man.Adapters)
            Else
                lAdt.Add(man.Adapter(Me.m_varname))
            End If

            ' Try to preserve selection
            Dim var As eVarNameFlags = eVarNameFlags.NotSet
            Dim guid As Guid
            Dim iSel As Integer = 0

            If (Me.m_iSelectedIndex > 0) Then
                var = Me.m_lInfo(Me.m_iSelectedIndex).VarName
                guid = Me.m_lInfo(Me.m_iSelectedIndex).Guid
            End If

            Me.m_lInfo.Clear()

            Dim dicConn As New Dictionary(Of ISpatialDataSet, cSpatialDataConnection)
            For Each adt As cSpatialDataAdapter In lAdt
                For Each conn As cSpatialDataConnection In adt.Connections()
                    Debug.Assert(conn IsNot Nothing)
                    If (adt.IsEnabled(conn.iLayer) And conn.IsConfigured) Then
                        ' Only show datasets overlapping with run period
                        If Me.OverlapsWithRunPeriod(conn) Then
                            dicConn(conn.Dataset) = conn
                        End If
                    End If
                Next conn
            Next

            For Each conn As cSpatialDataConnection In dicConn.Values

                Dim pos As New cDatasetInfo()
                pos.Dataset = conn.Dataset
                pos.VarName = conn.Adapter.VarName
                pos.Guid = conn.Dataset.GUID
                pos.PosVert = iRow

                If conn.Dataset.TimeStart = Date.MinValue Then
                    pos.DataStart = 1
                    pos.AppliedDataStart = pos.DataStart
                Else
                    pos.DataStart = core.AbsoluteTimeToEcospaceTimestep(conn.Dataset.TimeStart)
                    pos.AppliedDataStart = core.AbsoluteTimeToEcospaceTimestep(conn.TimeStart)
                End If

                If conn.Dataset.TimeEnd = Date.MaxValue Then
                    pos.DataEnd = core.nEcospaceTimeSteps
                    pos.AppliedDataStart = pos.DataEnd
                Else
                    pos.DataEnd = core.AbsoluteTimeToEcospaceTimestep(conn.Dataset.TimeEnd)
                    pos.AppliedDataEnd = core.AbsoluteTimeToEcospaceTimestep(conn.TimeEnd)
                End If

                For iStep As Integer = pos.AppliedDataStart To pos.AppliedDataEnd
                    Dim bIsBorrowed As Boolean = False

                    Dim tm As DateTime = core.EcospaceTimestepToAbsoluteTime(iStep)
                    Dim tmData As DateTime = conn.ToDataTime(core, tm)

                    If conn.Dataset.HasDataAtT(tmData) Then
                        If tm = tmData Then
                            pos.DataPoint.Add(iStep)
                        Else
                            pos.BorrowedDataPoint.Add(iStep)
                            pos.BorrowedDataFrom.Add(core.AbsoluteTimeToEcospaceTimestep(tmData))
                        End If
                    End If
                Next

                Me.m_lInfo.Add(pos)

                If (pos.VarName = var) And (pos.Guid = guid) Then
                    iSel = iRow
                End If

                iRow += 1
            Next conn

            Me.SelectedDatasetIndex = iSel

        End Sub

        ''' <summary>
        ''' Paint the header row
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        Private Sub DrawGridHeader(g As Graphics, rc As Rectangle)

            g.FillRectangle(SystemBrushes.Control, rc)

            Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
            Dim core As cCore = Me.m_uic.Core
            Dim sStepsPerYear As Single = CSng(Me.m_uic.Core.nEcospaceTimeSteps / Math.Max(1, Me.m_uic.Core.nEcospaceYears))

            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                For i As Integer = 0 To Me.m_uic.Core.nEcospaceYears Step 5
                    Dim sx As Single = i * sStepsPerYear * Me.m_iTimestepSize
                    g.DrawString(CStr(iYear + i), ft, SystemBrushes.ControlText, sx, 0.0!)
                    Using p As New Pen(SystemColors.ControlDarkDark, 1)
                        p.DashStyle = DashStyle.Dot
                        g.DrawLine(p, rc.X + sx, rc.Y, rc.X + sx, rc.Y + rc.Height)
                    End Using
                Next
            End Using

        End Sub

        ''' <summary>
        ''' Draw a grid of vertical lines for every 5 years
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc"></param>
        Private Sub DrawGrid(g As Graphics, rc As Rectangle)

            Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
            Dim core As cCore = Me.m_uic.Core
            Dim sStepsPerYear As Single = CSng(Me.m_uic.Core.nEcospaceTimeSteps / Math.Max(1, Me.m_uic.Core.nEcospaceYears))

            Using p As New Pen(SystemColors.ControlDarkDark, 1)
                p.DashStyle = DashStyle.Dot
                For i As Integer = 0 To Me.m_uic.Core.nEcospaceYears
                    Dim sx As Single = i * sStepsPerYear * Me.m_iTimestepSize
                    g.DrawLine(p, rc.X + sx, rc.Y, sx, rc.Y + rc.Height)
                Next
            End Using

        End Sub

        ''' <summary>
        ''' Draw a shaded area to indicate the presence of a dataset. This area falls below the time step grid
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="pos"></param>
        Private Sub DrawDatasetIndicator(g As Graphics,
                                         pos As cDatasetInfo,
                                         bSelected As Boolean)

            Dim rcBar As Rectangle = Me.DatasetArea(pos)
            Dim rcBack As Rectangle = New Rectangle(-Me.AutoScrollPosition.X, rcBar.Y - c_barmargin, Me.ClientRectangle.Width, rcBar.Height + 2 * c_barmargin)
            Dim comp As cDatasetCompatilibity = Me.m_manSets.Compatibility(pos.Dataset)

            ' Fill back bar
            Using br As New SolidBrush(cColorUtils.GetVariant(cStyleGuide.GetColor(comp), 0.75))
                g.FillRectangle(br, rcBack)
            End Using
            g.DrawLine(Pens.White, rcBack.X, rcBack.Y, rcBack.X + rcBack.Width, rcBack.Y)

        End Sub


        ''' <summary>
        ''' Draw the actual dataset bar, data points and labels
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="pos"></param>
        Private Sub DrawDataset(g As Graphics,
                                pos As cDatasetInfo,
                                bSelected As Boolean)

            Dim rcBar As Rectangle = Me.DatasetArea(pos)
            Dim rcBack As Rectangle = New Rectangle(-Me.AutoScrollPosition.X, rcBar.Y - c_barmargin, Me.ClientRectangle.Width, rcBar.Height + 2 * c_barmargin)
            Dim rcLabel As New Rectangle(rcBar.X, rcBar.Y, rcBar.Width, c_barlabelheight)
            Dim rcDot As New Rectangle(rcBar.X, rcBar.Y + c_barheight - CInt((c_barheight - c_barlabelheight) / 2) - c_dotradius, 2 * c_dotradius, 2 * c_dotradius)
            Dim rcImg As New Rectangle(rcBar.X, CInt(rcBar.Y + c_barheight - CInt((c_barheight - c_barlabelheight) / 2) - c_imgradius), 2 * c_imgradius, 2 * c_imgradius)

            Dim comp As cDatasetCompatilibity = Me.m_manSets.Compatibility(pos.Dataset)
            Dim clrBar As Color = cStyleGuide.GetColor(comp)
            Dim clrText As Color = SystemColors.ControlText
            Dim clrOutline As Color
            Dim iWidthOutline As Integer = 1

            ' Is off-screen?
            Dim bOutRight As Boolean = (rcBar.X > Me.AutoScrollPosition.X + Me.ClientRectangle.Width)
            Dim bOutLeft As Boolean = ((rcBar.X + rcBar.Width) < Me.AutoScrollPosition.X)

            If bSelected Then
                clrOutline = SystemColors.Highlight
                iWidthOutline = 2
            Else
                clrOutline = cColorUtils.GetVariant(clrBar, -0.5)
            End If

            ' Draw borrowed data points, if any
            For i As Integer = 0 To pos.BorrowedDataPoint.Count - 1
                Dim iStep As Integer = pos.BorrowedDataPoint(i)
                rcDot.X = rcBar.X + (iStep - pos.DataStart) * Me.m_iTimestepSize - c_dotradius
                g.FillEllipse(Brushes.White, rcDot)
                g.DrawEllipse(Pens.Gray, rcDot)
            Next

            ' Fill area bar
            Using br As New SolidBrush(clrBar)
                g.FillRectangle(br, rcBar)
            End Using

            ' Draw outline
            Using p As New Pen(clrOutline, iWidthOutline)
                g.DrawRectangle(p, rcBar)
            End Using

            ' Draw text within bar area, but as much on-screen as possible
            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                rcLabel.Width = rcBack.Width
                g.DrawString(pos.Dataset.CustomName, ft, SystemBrushes.ControlText, Math.Max(rcBack.X, rcLabel.X), rcLabel.Y)
            End Using


            For i As Integer = 0 To pos.DataPoint.Count - 1
                Dim iStep As Integer = pos.DataPoint(i)
                rcDot.X = rcBar.X + (iStep - pos.DataStart) * Me.m_iTimestepSize - c_dotradius
                rcImg.X = rcBar.X + (iStep - pos.DataStart) * Me.m_iTimestepSize - c_imgradius

                Select Case comp.CompatibilityAt(iStep)
                    Case cDatasetCompatilibity.eCompatibilityTypes.NotSet,
                         cDatasetCompatilibity.eCompatibilityTypes.TemporalNotIndexed
                        g.FillEllipse(Brushes.White, rcDot)
                        g.DrawEllipse(Pens.Black, rcDot)

                    Case cDatasetCompatilibity.eCompatibilityTypes.Errors
                        g.DrawImage(Me.m_bmpError, rcImg.Location)

                    Case cDatasetCompatilibity.eCompatibilityTypes.NoSpatial
                        g.DrawImage(Me.m_bmpWarning, rcImg.Location)

                    Case cDatasetCompatilibity.eCompatibilityTypes.NoTemporal
                        'NOP

                    Case cDatasetCompatilibity.eCompatibilityTypes.PartialSpatial
                        g.DrawImage(Me.m_bmpWarning, rcImg.Location)

                    Case cDatasetCompatilibity.eCompatibilityTypes.TotalOverlap
                        g.FillEllipse(Brushes.Green, rcDot)
                        g.DrawEllipse(Pens.Black, rcDot)

                End Select
            Next


        End Sub

        Private Function DatasetPos(ds As ISpatialDataSet) As cDatasetInfo
            If ds Is Nothing Then Return Nothing
            For Each pos As cDatasetInfo In Me.m_lInfo
                If ReferenceEquals(pos.Dataset, ds) Then Return pos
            Next
            Return Nothing
        End Function

        Private Function DatasetArea(pos As cDatasetInfo) As Rectangle
            Dim iStart As Integer = pos.DataStart * Me.m_iTimestepSize
            Dim iEnd As Integer = (pos.DataEnd + 1) * Me.m_iTimestepSize - 1
            Return New Rectangle(iStart, c_headerheight + pos.PosVert * (c_barheight + 2 * c_barmargin) + c_barmargin, iEnd - iStart, c_barheight)
        End Function


        Private Function TimestepFromPoint(pt As Point) As Integer
            If (Me.m_iTimestepSize = 0) Then Return -1
            Return CInt(Math.Round(pt.X / Me.m_iTimestepSize))
        End Function

        Private Function DatasetFromPoint(pt As Point) As cDatasetInfo
            If (pt.Y < c_headerheight) Then Return Nothing
            For Each pos As cDatasetInfo In Me.m_lInfo
                ' JS 30Nov14: only test Y-coordinate fit
                'If Me.DatasetArea(pos).Contains(pt) Then Return pos
                Dim area As Rectangle = Me.DatasetArea(pos)
                If ((pt.Y >= area.Y) And (pt.Y <= (area.Y + area.Height))) Then Return pos
            Next
            Return Nothing
        End Function

        Private Function OverlapsWithRunPeriod(conn As cSpatialDataConnection) As Boolean

            Dim ds As ISpatialDataSet = conn.Dataset

            If (ds.TimeStart = Date.MinValue) Then Return True
            If (ds.TimeEnd = Date.MaxValue) Then Return True

            Dim core As cCore = Me.m_uic.Core
            Dim dtStart As Date = core.EcospaceTimestepToAbsoluteTime(1)
            Dim dtEnd As Date = core.EcospaceTimestepToAbsoluteTime(core.nEcospaceTimeSteps)

            Return dtStart < conn.TimeEnd And dtEnd > conn.TimeStart

        End Function

#End Region ' Internals

    End Class

End Namespace ' Ecospace.Controls