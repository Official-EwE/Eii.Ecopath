' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.ComponentModel
Imports EwEUtils.UserInterface
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for editing a <see cref="cARGBColorRamp"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucEditGradient

#Region " Private parts "

        Private m_lColors As New List(Of VisualColor)
        Private m_ramp As cColorRamp = Nothing

#End Region ' Private parts

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Sub New()

            MyBase.New()
            Me.InitializeComponent()

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            Me.UpdateControls()

        End Sub

#End Region ' Constructor

#Region " Overrides "

        <Browsable(False)>
        Public Property ColorRamp As cColorRamp
            Get
                Return Me.m_ramp
            End Get
            Set(value As cColorRamp)

                Dim bClear As Boolean = True

                Debug.Assert(Not Me.m_bInUpdate)
                Me.m_bInUpdate = True

                Me.m_tbxName.Text = ""
                Me.m_ramp = value

                If (value IsNot Nothing) Then
                    Me.m_tbxName.Text = value.Name
                    If TypeOf value Is cARGBColorRamp Then
                        Dim ramp As cARGBColorRamp = DirectCast(value, cARGBColorRamp)
                        Me.SetARGBGradient(ramp.GradientBreaks, ramp.GradientColors)
                        bClear = False
                    End If
                End If

                If bClear Then Me.SetARGBGradient(Nothing, Nothing)
                Me.m_tbxName.ReadOnly = Not Me.IsEditable()
                Me.UpdateControls()

                Me.m_bInUpdate = False

            End Set
        End Property

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            Me.ColorRamp = Nothing

        End Sub

        ''' <summary>
        ''' Paint the control background to render the current gradient.
        ''' </summary>
        Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)

            MyBase.OnPaintBackground(e)

            Dim rc As Rectangle = Me.m_plGradient.ClientRectangle
            Dim ramp As cColorRamp = Me.m_ramp

            rc.X = Me.m_plGradient.Location.X
            rc.Y = Me.m_plGradient.Location.Y

            e.Graphics.FillRectangle(SystemBrushes.Window, rc)

            cColorRampIndicator.DrawColorRamp(e.Graphics, ramp, rc)
            ControlPaint.DrawBorder3D(e.Graphics, rc)

        End Sub

#End Region ' Overrides

#Region " Events "

        Public Event OnColorRampChanged(sender As Object, args As cColorRamp)

        ''' <summary>
        ''' User clicked CurrentColor box to pick a colour
        ''' </summary>
        Private Sub OnPickColour(sender As System.Object, e As System.EventArgs) _
            Handles m_pbCurrentColor.Click
            Try
                Me.PickColor()
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' User altered a colour value via a numeric up/down control.
        ''' </summary>
        Private Sub OnColorValueChanged(sender As Object, e As System.EventArgs) _
            Handles m_nudRed.ValueChanged, m_nudGreen.ValueChanged, m_nudBlue.ValueChanged, m_nudAlpha.ValueChanged

            If (Me.m_bInUpdate) Then Return

            Dim clr As VisualColor = VisualColor.FromArgb(CInt(m_nudAlpha.Value), CInt(m_nudRed.Value), CInt(m_nudGreen.Value), CInt(m_nudBlue.Value))
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = clr
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User altered a colour value via a slider.
        ''' </summary>
        Private Sub OnColourSliderChanged(sender As System.Object, e As System.EventArgs) _
            Handles m_slRed.ValueChanged, m_slBlue.ValueChanged, m_slGreen.ValueChanged, m_slAlpha.ValueChanged

            If (Me.m_bInUpdate) Then Return

            Dim clr As VisualColor = VisualColor.FromArgb(CInt(m_slAlpha.Value), CInt(m_slRed.Value), CInt(m_slGreen.Value), CInt(m_slBlue.Value))
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = clr
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User added a gradient break.
        ''' </summary>
        Private Sub OnAddBreak(sender As System.Object, e As System.EventArgs) _
            Handles m_btnAdd.Click

            Dim grad As cARGBColorRamp = DirectCast(Me.m_ramp, cARGBColorRamp)
            Me.m_slGradient.Add()
            Me.m_lColors.Add(grad.GetColorInvariant(Me.m_slGradient.Value(0) / Me.m_slGradient.Maximum))
            Me.UpdateARGBGradient(grad)
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' User removed a gradient break.
        ''' </summary>
        Private Sub OnRemoveBreak(sender As System.Object, e As System.EventArgs) _
            Handles m_btnRemove.Click

            Dim iKnob As Integer = Me.m_slGradient.CurrentKnob
            If Me.m_lColors.Count > 2 Then
                Me.m_slGradient.Remove(iKnob)
                Me.m_lColors.RemoveAt(iKnob)
                Dim grad As cARGBColorRamp = DirectCast(Me.m_ramp, cARGBColorRamp)
                Me.UpdateARGBGradient(grad)
            End If
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' User selected a different knob in the gradient slider.
        ''' </summary>
        Private Sub OnGradientSliderCurrentKnobChanged(sender As Object, e As SliderKnobChangedEventArgs) _
            Handles m_slGradient.CurrentKnobChanged

            If (Me.m_bInUpdate) Then Return
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User selected a different value in the gradient slider.
        ''' </summary>
        Private Sub OnGradientSliderValueChanged(sender As Object, e As System.EventArgs) _
            Handles m_slGradient.ValueChanged

            If (Me.m_bInUpdate) Then Return
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' Flip gradient
        ''' </summary>
        Private Sub OnFlipGradient(sender As System.Object, e As System.EventArgs) _
            Handles m_btnFlip.Click

            Me.m_bInUpdate = True
            Try
                For i As Integer = 0 To Me.m_slGradient.NumKnobs - 1
                    Me.m_slGradient.Value(i) = (100 - Me.m_slGradient.Value(i))
                Next
                Me.Invalidate(True)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OnFlipGradient() Exception " & ex.Message)
                System.Console.WriteLine(Me.ToString & ".OnFlipGradient() Exception " & ex.Message)
            End Try

            Me.m_bInUpdate = False
            Me.ApplyColorsToGradient()

        End Sub

        Private Sub OnNameChanged(sender As Object, e As EventArgs) Handles m_tbxName.TextChanged
            Try
                If (Me.m_bInUpdate) Then Return
                If (Me.IsEditable) Then Me.m_ramp.Name = Me.m_tbxName.Text
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events 

#Region " Internals "

        Private ReadOnly Property IsEditable As Boolean
            Get
                If (Me.m_ramp Is Nothing) Then Return False
                Return (Me.m_ramp.IsEditable)
            End Get
        End Property

        Private Sub SetARGBGradient(breaks() As Double, colors() As VisualColor)

            Me.m_bInUpdate = True

            Me.m_lColors.Clear()
            Me.m_slGradient.NumKnobs = 0

            If (breaks IsNot Nothing) And (colors IsNot Nothing) Then

                Me.m_slGradient.NumKnobs = breaks.Length

                Dim iPos As Integer = 0
                For i As Integer = 0 To breaks.Length - 1
                    iPos += CInt(breaks(i) * 100)
                    Me.m_lColors.Add(colors(i))
                    Me.m_slGradient.Value(i) = iPos
                Next
                Me.ApplyColorsToGradient()
            End If

            Me.m_bInUpdate = False
            Me.Invalidate(True)

            Me.UpdateControls()

        End Sub

        Private Sub UpdateControls()

            Dim bIsEditableGradient As Boolean = Me.IsEditable
            Dim iNumKnobs As Integer = Me.m_slGradient.NumKnobs

            Me.m_tbxName.Enabled = bIsEditableGradient

            Me.m_slRed.Enabled = bIsEditableGradient
            Me.m_nudRed.Enabled = bIsEditableGradient

            Me.m_slGreen.Enabled = bIsEditableGradient
            Me.m_nudGreen.Enabled = bIsEditableGradient

            Me.m_slBlue.Enabled = bIsEditableGradient
            Me.m_nudBlue.Enabled = bIsEditableGradient

            Me.m_slAlpha.Enabled = bIsEditableGradient
            Me.m_nudAlpha.Enabled = bIsEditableGradient

            Me.m_slGradient.Enabled = bIsEditableGradient
            Me.m_pbCurrentColor.Enabled = bIsEditableGradient

            Me.m_btnAdd.Enabled = (iNumKnobs < 8) And bIsEditableGradient
            Me.m_btnRemove.Enabled = (iNumKnobs > 2) And bIsEditableGradient
            Me.m_btnFlip.Enabled = bIsEditableGradient

            ' Hide it, and draw on the area of this control instead... wow, that's awful!
            Me.m_plGradient.Visible = False

        End Sub

        ''' <summary>Loop prevention flag.</summary>
        Private m_bInUpdate As Boolean = False

        Private Sub ApplyColorsToGradient()

            If Me.m_bInUpdate Then Return
            Me.m_bInUpdate = True

            Try
                If Me.IsEditable Then
                    Dim clr As VisualColor = Me.m_lColors(Me.m_slGradient.CurrentKnob)
                    Me.m_pbCurrentColor.BackColor = cStyleGuide.FromVisualColor(clr)

                    Me.m_slRed.Value = clr.R
                    Me.m_nudRed.Value = clr.R

                    Me.m_slGreen.Value = clr.G
                    Me.m_nudGreen.Value = clr.G

                    Me.m_slBlue.Value = clr.B
                    Me.m_nudBlue.Value = clr.B

                    Me.m_slAlpha.Value = clr.A
                    Me.m_nudAlpha.Value = clr.A

                    Me.UpdateARGBGradient(DirectCast(Me.m_ramp, cARGBColorRamp))
                    RaiseEvent OnColorRampChanged(Me, Me.m_ramp)
                Else
                    Me.m_slRed.Value = 0
                    Me.m_nudRed.Value = 0

                    Me.m_slGreen.Value = 0
                    Me.m_nudGreen.Value = 0

                    Me.m_slBlue.Value = 0
                    Me.m_nudBlue.Value = 0

                    Me.m_slAlpha.Value = 0
                    Me.m_nudAlpha.Value = 0

                End If

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
                System.Console.WriteLine(Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
            End Try

            Me.m_bInUpdate = False

            Me.Refresh()

        End Sub

        Private Sub PickColor()

            Dim dlg As New cEwEColorDialog()
            dlg.Color = cStyleGuide.FromVisualColor(Me.m_lColors(Me.m_slGradient.CurrentKnob))
            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = cStyleGuide.ToVisualColor(dlg.Color)

            Me.ApplyColorsToGradient()

        End Sub

        Private Sub UpdateARGBGradient(argb As cARGBColorRamp)

            ' Sort knobs indexes in ascending order by knob value. This will be the basis 
            ' for creating the gradient positions and corresponding colours.
            Dim lKnobsSorted As New List(Of Integer)

            ' For all knobs:
            For i As Integer = 0 To Me.m_lColors.Count - 1
                ' Find position for a knob in the lKnobsSorted list
                Dim iPos As Integer = -1
                Dim j As Integer = 0
                While (j <= lKnobsSorted.Count - 1) And (iPos = -1)
                    ' Does knob at this position represent a smaller value
                    If Me.m_slGradient.Value(i) < Me.m_slGradient.Value(lKnobsSorted(j)) Then
                        iPos = j
                    End If
                    j += 1
                End While
                If iPos = -1 Then
                    lKnobsSorted.Add(i)
                Else
                    lKnobsSorted.Insert(iPos, i)
                End If
            Next

            ' Create breaks and colours arrays for gradient from sorted knob list
            Dim lPos As New List(Of Double)
            Dim lColor As New List(Of VisualColor)
            Dim iLast As Integer = 0

            For i As Integer = 0 To lKnobsSorted.Count - 1
                Dim iValue As Integer = Me.m_slGradient.Value(lKnobsSorted(i))
                lPos.Add((iValue - iLast) / Me.m_slGradient.Maximum)
                iLast = iValue
                lColor.Add(Me.m_lColors(lKnobsSorted(i)))
            Next

            ' Update gradient
            argb.GradientColors = lColor.ToArray
            argb.GradientBreaks = lPos.ToArray

            argb.Name = Me.m_tbxName.Text

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
