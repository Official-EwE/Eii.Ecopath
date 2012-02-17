Imports EwEUtils.SpatialData
Imports EwEUtils.Core
Imports EwECore.SpatialData
Imports EwECore

Namespace Ecospace.Controls

    Public Class ucExternalDataConnections
        Implements IUIElement

        Private Const c_barheight As Integer = 18

        Private Class cDatasetPos
            Public m_ds As ISpatialDataSet
            Public m_iTimeStart As Integer = 0
            Public m_iTimeEnd As Integer = 0
            Public m_iPosVert As Integer = 0
        End Class

        Private m_uic As cUIContext = Nothing
        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet
        Private m_lPos As New List(Of cDatasetPos)
        Private m_iTimestepSize As Integer = 1

        Public Sub New()
            Me.InitializeComponent()
            Me.SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer, True)
        End Sub

        Public Property UIContext As ScientificInterfaceShared.Controls.cUIContext Implements ScientificInterfaceShared.Controls.IUIElement.UIContext
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

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            MyBase.OnLoad(e)

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Me.RecalcSize()
            Me.RecalcLayout()

        End Sub

        ' ToDo: respond to core messages to update ecospace run time, dataset changes

        Protected Sub RecalcSize()
            ' Safety check
            If (Me.m_uic Is Nothing) Then Return
            ' Calc number of pixels per time step
            Me.m_iTimestepSize = CInt(Math.Max(4, Math.Floor(Me.Width / Me.m_uic.Core.nEcospaceTimeSteps)))
            Me.AutoScrollMinSize = New Size(Me.m_iTimestepSize, (Me.m_lPos.Count + 1) * 18)
            'Me.Invalidate()
        End Sub

        ''' <summary>
        ''' Calculate dataset display rectangles
        ''' </summary>
        Protected Sub RecalcLayout()

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Dim conn As cSpatialDataConnectionManager = Me.m_uic.Core.SpatialDataConnectionManager()
            Dim lAdt As New List(Of cSpatialDataAdapter)
            Dim ds As ISpatialDataSet = Nothing
            Dim iRow As Integer = 0

            ' Resolve varname
            If Me.m_varname = eVarNameFlags.NotSet Then
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
                            pos.m_iTimeStart = 0
                        Else
                            pos.m_iTimeStart = Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(ds.TimeStart)
                        End If

                        If ds.TimeEnd = Date.MinValue Then
                            pos.m_iTimeEnd = 0
                        Else
                            pos.m_iTimeEnd = Me.m_uic.Core.AbsoluteTimeToEcospaceTimestep(ds.TimeEnd)
                        End If

                        Me.m_lPos.Add(pos)
                        iRow += 1
                    End If
                Next
            Next

        End Sub

        Protected Overrides Sub OnResize(e As System.EventArgs)
            MyBase.OnResize(e)
            Me.RecalcSize()
            Me.Invalidate(True)
        End Sub

        Protected Overrides Sub OnMouseClick(e As System.Windows.Forms.MouseEventArgs)
            MyBase.OnMouseClick(e)
        End Sub

        Protected Overrides Sub OnMouseHover(e As System.EventArgs)
            MyBase.OnMouseHover(e)
        End Sub

        Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
            MyBase.OnPaint(e)

            ' Safety check
            If (Me.m_uic Is Nothing) Then Return

            Me.PaintTimeGrid(e.Graphics, New Rectangle(0, 0, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, c_barheight))

            For Each pos As cDatasetPos In Me.m_lPos
                Me.PaintDataset(e.Graphics, New Rectangle(0, c_barheight + pos.m_iPosVert * c_barheight, Me.m_iTimestepSize * Me.m_uic.Core.nEcospaceTimeSteps, c_barheight), pos)
            Next

        End Sub

        Private Sub PaintTimeGrid(g As Graphics, rc As Rectangle)

            g.FillRectangle(SystemBrushes.Control, rc)

            Dim iYear As Integer = Me.m_uic.Core.EcosimFirstYear
            Dim core As cCore = Me.m_uic.Core
            Dim sStepsPerYear As Single = CSng(Me.m_uic.Core.nEcospaceTimeSteps / Math.Max(1, Me.m_uic.Core.nEcospaceYears))

            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                For i As Integer = 0 To Me.m_uic.Core.nEcospaceYears Step 5
                    Dim sx As Single = i * sStepsPerYear * Me.m_iTimestepSize
                    g.DrawString(CStr(iYear + i), ft, SystemBrushes.ControlText, sx, 0.0!)
                    g.DrawLine(SystemPens.ControlLightLight, sx, 0, sx, c_barheight)
                    g.DrawLine(SystemPens.Control, sx, c_barheight, sx, Me.ClientRectangle.Height - c_barheight)
                Next
            End Using

        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="rc">Fitted rectangle to draw the dataset</param>
        ''' <param name="pos"></param>
        ''' <remarks></remarks>
        Private Sub PaintDataset(g As Graphics, rc As Rectangle, pos As cDatasetPos)

            Dim iStart As Integer = rc.X + pos.m_iTimeStart * Me.m_iTimestepSize
            Dim iEnd As Integer = rc.X + pos.m_iTimeEnd * Me.m_iTimestepSize
            Dim rcBar As New Rectangle(rc.X + iStart, rc.Y + 2, iEnd - iStart, c_barheight - 4)
            Dim fmt As New StringFormat()

            fmt.LineAlignment = StringAlignment.Far

            Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                Using br As New SolidBrush(Color.FromArgb(255, 100, 140, 250))
                    g.FillRectangle(br, rcBar)
                    g.DrawString(pos.m_ds.DisplayName, ft, SystemBrushes.ControlText, rcBar, fmt)
                End Using
            End Using

        End Sub

    End Class

End Namespace ' Ecospace.Controls