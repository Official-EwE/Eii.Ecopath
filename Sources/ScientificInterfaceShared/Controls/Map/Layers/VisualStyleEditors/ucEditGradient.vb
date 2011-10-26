#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports System.Drawing.Drawing2D
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    Public Class ucEditGradient

#Region " Private parts "

        Private m_uic As cUIContext = Nothing
        Private m_clrStart As Color = Color.White
        Private m_clrEnd As Color = Color.White
        Private m_iSelectedColor As Integer = 0

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------

#End Region ' Private parts

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, _
                       ByVal vs As cVisualStyle, _
                       ByVal style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(vs, style)

            Me.InitializeComponent()

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            If (vs.GradientBreaks IsNot Nothing And vs.GradientColors IsNot Nothing) Then
                Me.m_clrStart = vs.GradientColors(0)
                Me.m_clrEnd = vs.GradientColors(1)
                Me.m_rbCustomGradient.Checked = True
            Else
                Me.m_rbDefaultGradient.Checked = True
            End If

            Me.UpdateControls()

        End Sub

#End Region ' Constructor

#Region " Overrides "

        Public Overrides Function Apply(ByVal vs As cVisualStyle) As Boolean

            If Me.m_rbDefaultGradient.Checked Then
                vs.GradientBreaks = Nothing
                vs.GradientColors = Nothing
            Else
                vs.GradientBreaks = New Double() {0, 1}
                vs.GradientColors = New Color() {Me.m_clrStart, Me.m_clrEnd}
            End If

            Return True

        End Function

#End Region ' Overrides

#Region " Internals "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property SelectedForeColor() As Color
            Get
                Return Me.m_clrStart
            End Get
            Set(ByVal value As Color)
                Me.m_clrStart = value
                Me.UpdateControls()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property SelectedBackColor() As Color
            Get
                Return Me.m_clrEnd
            End Get
            Set(ByVal value As Color)
                Me.m_clrEnd = value
                Me.UpdateControls()
            End Set
        End Property

#End Region ' Internals

#Region " Events "

        Private Sub OnGradOptionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_rbDefaultGradient.CheckedChanged, m_rbCustomGradient.CheckedChanged

            Me.UpdateControls()

        End Sub

        Private Sub pbForeColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_plStart.Click
            Me.PickColor(0)
        End Sub

        Private Sub pbBackColor_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_plEnd.Click
            Me.PickColor(1)
        End Sub

        Private Sub nud_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudRed.ValueChanged, m_nudGreen.ValueChanged, m_nudBlue.ValueChanged, m_nudAlpha.ValueChanged
            Dim clr As Color = Color.FromArgb(CInt(m_nudAlpha.Value), CInt(m_nudRed.Value), CInt(m_nudGreen.Value), CInt(m_nudBlue.Value))

            Select Case Me.m_iSelectedColor
                Case 0
                    Me.m_clrStart = clr
                Case 1
                    Me.m_clrEnd = clr
            End Select

            Me.UpdateColors()
        End Sub

        Private Sub tb_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_slRed.ValueChanged, m_slBlue.ValueChanged, m_slGreen.ValueChanged, m_slAlpha.ValueChanged

            Dim clr As Color = Color.FromArgb(CInt(m_slAlpha.Value), CInt(m_slRed.Value), CInt(m_slGreen.Value), CInt(m_slBlue.Value))

            Select Case Me.m_iSelectedColor
                Case 0
                    Me.m_clrStart = clr
                Case 1
                    Me.m_clrEnd = clr
            End Select

            Me.UpdateColors()
        End Sub

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.UpdateControls()
        End Sub

        Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)

            MyBase.OnPaintBackground(e)

            Dim ramp As New cARGBColorRamp(New Color() {Me.m_clrStart, Me.m_clrEnd}, New Double() {0, 1})
            Dim rc As Rectangle = Me.m_plPreview.ClientRectangle
            rc.X = Me.m_plPreview.Location.X
            rc.Y = Me.m_plPreview.Location.Y

            e.Graphics.FillRectangle(New SolidBrush(Color.White), rc)
            cColorRampIndicator.DrawColorRamp(e.Graphics, ramp, rc)
        End Sub

        Private Sub pbForeColor_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_plStart.Paint

            Dim rcOuter As New Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)
            Dim rcInner As New Rectangle(e.ClipRectangle.X + 3, e.ClipRectangle.Y + 3, e.ClipRectangle.Width - 6, e.ClipRectangle.Height - 6)

            If m_plStart.Enabled Then
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
                Using br As New SolidBrush(Me.m_clrStart)
                    e.Graphics.FillRectangle(br, rcInner)
                End Using
                If Me.m_iSelectedColor = 0 Then
                    e.Graphics.DrawRectangle(Pens.Black, rcOuter)
                End If
            Else
                e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle)
            End If

        End Sub

        Private Sub pbBackColor_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) _
            Handles m_plEnd.Paint

            Dim rcOuter As New Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)
            Dim rcInner As New Rectangle(e.ClipRectangle.X + 3, e.ClipRectangle.Y + 3, e.ClipRectangle.Width - 6, e.ClipRectangle.Height - 6)

            If m_plEnd.Enabled Then
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
                Using br As New SolidBrush(Me.m_clrEnd)
                    e.Graphics.FillRectangle(br, rcInner)
                End Using
                If Me.m_iSelectedColor = 1 Then
                    e.Graphics.DrawRectangle(Pens.Black, rcOuter)
                End If
            Else
                e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle)
            End If

        End Sub

#End Region ' Events 

#Region " Internal implementation "

        Private Sub UpdateControls()
            Me.m_plPreview.Visible = False
            Me.UpdateColors()
        End Sub

        ''' <summary>Loop prevention flag.</summary>
        Private m_bInUpdate As Boolean = False

        Private Sub UpdateColors()

            If m_bInUpdate Then Return

            Me.m_bInUpdate = True

            Dim clr As Color = DirectCast(IIf(Me.m_iSelectedColor = 0, Me.m_clrStart, Me.m_clrEnd), Color)
            Dim bEnabled As Boolean = (Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.Gradient) > 0

            Me.m_slRed.Value = clr.R
            Me.m_slRed.Enabled = bEnabled
            Me.m_nudRed.Value = clr.R
            Me.m_nudRed.Enabled = bEnabled

            Me.m_slGreen.Value = clr.G
            Me.m_slGreen.Enabled = bEnabled
            Me.m_nudGreen.Value = clr.G
            Me.m_nudGreen.Enabled = bEnabled

            Me.m_slBlue.Value = clr.B
            Me.m_slBlue.Enabled = bEnabled
            Me.m_nudBlue.Value = clr.B
            Me.m_nudBlue.Enabled = bEnabled

            Me.m_slAlpha.Value = clr.A
            Me.m_slAlpha.Enabled = bEnabled
            Me.m_nudAlpha.Value = clr.A
            Me.m_nudAlpha.Enabled = bEnabled

            Me.m_plEnd.Refresh()
            Me.m_plStart.Refresh()
            Me.m_plPreview.Refresh()

            Me.FireStyleChangedEvent()

            Me.m_bInUpdate = False

        End Sub

        Private Sub PickColor(ByVal iSel As Integer)

            If (Me.m_iSelectedColor <> iSel) Then
                Me.m_iSelectedColor = iSel
                Me.UpdateControls()
                Return
            End If

            Dim dlg As New ColorDialog()

            Select Case Me.m_iSelectedColor
                Case 0 : dlg.Color = Me.m_clrStart
                Case 1 : dlg.Color = Me.m_clrEnd
            End Select

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            Select Case Me.m_iSelectedColor
                Case 0 : Me.m_clrStart = dlg.Color
                Case 1 : Me.m_clrEnd = dlg.Color
            End Select

            Me.UpdateControls()

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
