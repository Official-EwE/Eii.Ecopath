' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 3 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see https://www.gnu.org/licenses/gpl-3.0.html>. 
'
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'




Imports System.Drawing.Drawing2D
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style



Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for editing the hatch part of a <see cref="cVisualStyle"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucEditHatch

#Region " Private parts "

        Private m_uic As cUIContext = Nothing
        Private m_hbs As HatchStyle = HatchStyle.DottedDiamond
        Private m_clrFore As Color = Color.Black
        Private m_clrBack As Color = Color.Transparent
        Private m_selectionType As eSelectionType = eSelectionType.ForeColor
        Private m_form As Form = Nothing
        Private m_control As ucHatchSelect = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Enum eSelectionType As Byte
            ForeColor
            BackColor
        End Enum

#End Region ' Private parts

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="uic">UIContext to operate onto.</param>
        ''' <param name="vs">The <see cref="cVisualStyle"/> to create the editor for.</param>
        ''' <param name="style">Aspect of the style that needs editing.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext,
                         vs As cVisualStyle,
                         style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(uic, vs, style)

            Me.InitializeComponent()

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            Me.m_control = New ucHatchSelect(Me)

            Me.m_form = New Form()
            Me.m_form.StartPosition = FormStartPosition.Manual
            Me.m_form.FormBorderStyle = FormBorderStyle.None
            Me.m_form.Hide()
            Me.m_form.ShowInTaskbar = False
            Me.m_form.Controls.Add(Me.m_control)
            Me.m_form.Width = Me.m_control.Width + 24
            Me.m_form.Height = Me.m_control.Height + 10

            Me.SelectedForeColor = cStyleGuide.FromVisualColor(vs.ForeColour)
            Me.SelectedBackColor = cStyleGuide.FromVisualColor(vs.BackColour)
            Me.SelectedHatchStyle = cStyleGuide.FromVisualHatch(vs.HatchStyle)

            Me.UpdateControls()

        End Sub

#End Region ' Constructor

#Region " Overrides "

        Public Overrides Property VisualStyle As cVisualStyle
            Get
                Return MyBase.VisualStyle
            End Get
            Set(value As cVisualStyle)
                MyBase.VisualStyle = value
                If (MyBase.VisualStyle IsNot Nothing) And (Me.m_control IsNot Nothing) Then
                    Me.SelectedForeColor = cStyleGuide.FromVisualColor(MyBase.VisualStyle.ForeColour)
                    Me.SelectedBackColor = cStyleGuide.FromVisualColor(MyBase.VisualStyle.BackColour)
                    Me.SelectedHatchStyle = cStyleGuide.FromVisualHatch(MyBase.VisualStyle.HatchStyle)
                End If
            End Set
        End Property

        Public Overrides Function Apply(vs As cVisualStyle) As Boolean
            vs.ForeColour = cStyleGuide.ToVisualColor(Me.SelectedForeColor)
            vs.BackColour = cStyleGuide.ToVisualColor(Me.SelectedBackColor)
            vs.HatchStyle = cStyleGuide.ToVisualHatch(Me.SelectedHatchStyle)
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
                Return Me.m_clrFore
            End Get
            Set(value As Color)
                Me.m_clrFore = value
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
                Return Me.m_clrBack
            End Get
            Set(value As Color)
                Me.m_clrBack = value
                Me.UpdateControls()
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property SelectedHatchStyle() As HatchStyle
            Get
                Return Me.m_hbs
            End Get
            Set(value As HatchStyle)
                Me.m_hbs = value
                Me.UpdateColors()
            End Set
        End Property

#End Region ' Internals

#Region " Events "

        Private Sub pbBrush_Click(sender As System.Object, e As System.EventArgs) Handles pbBrush.Click
            Me.DisplayDropdown()
        End Sub

        Private Sub pbForeColor_Click(sender As System.Object, e As System.EventArgs) Handles plForeColor.Click
            Me.SelectCustomControl(eSelectionType.ForeColor)
        End Sub

        Private Sub pbForeColor_DoubleClick(sender As System.Object, e As System.EventArgs) Handles plForeColor.DoubleClick
            Me.SelectCustomControl(eSelectionType.ForeColor)
            Me.DisplayDropdown()
        End Sub

        Private Sub pbBackColor_Click(sender As System.Object, e As System.EventArgs) Handles plBackColor.Click
            Me.SelectCustomControl(eSelectionType.BackColor)
        End Sub

        Private Sub pbBackColor_DoubleClick(sender As System.Object, e As System.EventArgs) Handles plBackColor.DoubleClick
            Me.SelectCustomControl(eSelectionType.BackColor)
            Me.DisplayDropdown()
        End Sub

        Private Sub nud_ValueChanged(sender As Object, e As System.EventArgs) Handles nudRed.ValueChanged, nudGreen.ValueChanged, nudBlue.ValueChanged, nudAlpha.ValueChanged
            Dim clr As Color = Color.FromArgb(CInt(Me.nudAlpha.Value), CInt(Me.nudRed.Value), CInt(Me.nudGreen.Value), CInt(Me.nudBlue.Value))

            Select Case Me.m_selectionType
                Case eSelectionType.ForeColor
                    Me.m_clrFore = clr
                Case eSelectionType.BackColor
                    Me.m_clrBack = clr
            End Select

            Me.UpdateColors()
        End Sub

        Private Sub tb_Scroll(sender As System.Object, e As System.EventArgs) Handles tbRed.ValueChanged, tbBlue.ValueChanged, tbGreen.ValueChanged, tbAlpha.ValueChanged
            Dim clr As Color = Color.FromArgb(CInt(Me.tbAlpha.Value), CInt(Me.tbRed.Value), CInt(Me.tbGreen.Value), CInt(Me.tbBlue.Value))

            Select Case Me.m_selectionType
                Case eSelectionType.ForeColor
                    Me.m_clrFore = clr
                Case eSelectionType.BackColor
                    Me.m_clrBack = clr
            End Select

            Me.UpdateColors()
        End Sub

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)
            Me.UpdateControls()
        End Sub

        Private Sub pbBrush_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles pbBrush.Paint

            Dim br As Brush = Nothing

            e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
            If ((Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.Hatch) > 0) Then
                br = New HatchBrush(Me.m_hbs, Me.m_clrFore, Me.m_clrBack)
            Else
                br = New SolidBrush(Me.m_clrFore)
            End If
            e.Graphics.FillRectangle(br, e.ClipRectangle)
            br.Dispose()
            br = Nothing

        End Sub

        Private Sub pbForeColor_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) _
            Handles plForeColor.Paint

            Dim rcOuter As New Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)
            Dim rcInner As New Rectangle(e.ClipRectangle.X + 3, e.ClipRectangle.Y + 3, e.ClipRectangle.Width - 6, e.ClipRectangle.Height - 6)

            If Me.plForeColor.Enabled Then
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
                Using br As New SolidBrush(Me.m_clrFore)
                    e.Graphics.FillRectangle(br, rcInner)
                End Using
                If Me.m_selectionType = eSelectionType.ForeColor Then
                    e.Graphics.DrawRectangle(Pens.Black, rcOuter)
                End If
            Else
                e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle)
            End If

        End Sub

        Private Sub pbBackColor_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) _
            Handles plBackColor.Paint

            Dim rcOuter As New Rectangle(e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)
            Dim rcInner As New Rectangle(e.ClipRectangle.X + 3, e.ClipRectangle.Y + 3, e.ClipRectangle.Width - 6, e.ClipRectangle.Height - 6)

            If Me.plBackColor.Enabled Then
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle)
                Using br As New SolidBrush(Me.m_clrBack)
                    e.Graphics.FillRectangle(br, rcInner)
                End Using
                If Me.m_selectionType = eSelectionType.BackColor Then
                    e.Graphics.DrawRectangle(Pens.Black, rcOuter)
                End If
            Else
                e.Graphics.FillRectangle(SystemBrushes.Control, e.ClipRectangle)
            End If

        End Sub

        Private Sub OnDropDownLostFocus(sender As Object, e As EventArgs)
            Me.HideDropdown()
        End Sub

        Private Sub OnDropDownDoubleClick(sender As Object, e As EventArgs)
            Me.HideDropdown()
        End Sub

#End Region ' Events 

#Region " Internal implementation "

        Private Sub UpdateControls()

            ' Brush picker
            Me.pbBrush.Enabled = ((Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.Hatch) > 0)
            Me.pbBrush.BorderStyle = If(Me.pbBrush.Enabled, BorderStyle.Fixed3D, BorderStyle.FixedSingle)

            Me.plBackColor.Enabled = ((Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.BackColor) > 0)
            Me.plBackColor.BorderStyle = If(Me.plBackColor.Enabled, BorderStyle.Fixed3D, BorderStyle.FixedSingle)

            Me.plForeColor.Enabled = ((Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.ForeColor) > 0)
            Me.plForeColor.BorderStyle = If(Me.plForeColor.Enabled, BorderStyle.Fixed3D, BorderStyle.FixedSingle)

            Me.UpdateColors()
        End Sub

        ''' <summary>Loop prevention flag.</summary>
        Private m_bInUpdate As Boolean = False

        Private Sub UpdateColors()

            If Me.m_bInUpdate Then Return

            Me.m_bInUpdate = True

            Dim clr As Color = If(Me.m_selectionType = eSelectionType.ForeColor, Me.m_clrFore, Me.m_clrBack)
            Dim bEnabled As Boolean = (Me.RepresentationStyles And (cVisualStyle.eVisualStyleTypes.BackColor Or cVisualStyle.eVisualStyleTypes.ForeColor)) > 0

            Me.tbRed.Value = clr.R
            Me.tbRed.Enabled = bEnabled
            Me.nudRed.Value = clr.R
            Me.nudRed.Enabled = bEnabled

            Me.tbGreen.Value = clr.G
            Me.tbGreen.Enabled = bEnabled
            Me.nudGreen.Value = clr.G
            Me.nudGreen.Enabled = bEnabled

            Me.tbBlue.Value = clr.B
            Me.tbBlue.Enabled = bEnabled
            Me.nudBlue.Value = clr.B
            Me.nudBlue.Enabled = bEnabled

            Me.tbAlpha.Value = clr.A
            Me.tbAlpha.Enabled = bEnabled
            Me.nudAlpha.Value = clr.A
            Me.nudAlpha.Enabled = bEnabled

            Me.plBackColor.Refresh()
            Me.plForeColor.Refresh()
            Me.pbBrush.Refresh()

            Me.FireStyleChangedEvent()

            Me.m_bInUpdate = False

        End Sub

        Private Sub SelectCustomControl(selType As eSelectionType)

            Dim dlg As New ColorDialog()
            If (Me.m_selectionType <> selType) Then
                Me.m_selectionType = selType
                Me.UpdateControls()
                Return
            End If

            Select Case selType
                Case eSelectionType.BackColor
                    dlg.Color = Me.m_clrBack

                Case eSelectionType.ForeColor
                    dlg.Color = Me.m_clrFore
            End Select

            Dim a As Byte = dlg.Color.A

            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return

            ' JS 06Oct19: retain alpha when switching colours
            Select Case selType
                Case eSelectionType.BackColor
                    Me.m_clrBack = Color.FromArgb(a, dlg.Color.R, dlg.Color.G, dlg.Color.B)

                Case eSelectionType.ForeColor
                    Me.m_clrFore = Color.FromArgb(a, dlg.Color.R, dlg.Color.G, dlg.Color.B)
            End Select

            Me.UpdateControls()

        End Sub

        Friend Sub DisplayDropdown()
            Dim loc As Point = Me.PointToScreen(Point.Empty)
            loc.Y += Me.pbBrush.Height + Me.pbBrush.Location.Y
            loc.X += Me.pbBrush.Location.X

            Me.m_control.Colours(Me.m_clrFore, Me.m_clrBack)

            Me.m_form.Location = loc
            Me.m_form.Show()
            Me.m_form.Focus()

            AddHandler Me.m_control.LostFocus, AddressOf Me.OnDropDownLostFocus
            AddHandler Me.m_control.DoubleClick, AddressOf Me.OnDropDownDoubleClick
        End Sub

        Friend Sub HideDropdown()
            RemoveHandler Me.m_control.LostFocus, AddressOf Me.OnDropDownLostFocus
            RemoveHandler Me.m_control.DoubleClick, AddressOf Me.OnDropDownDoubleClick
            Me.m_form.Hide()
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
