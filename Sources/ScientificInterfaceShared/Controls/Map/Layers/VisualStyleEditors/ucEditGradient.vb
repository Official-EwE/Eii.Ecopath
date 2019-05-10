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
Imports EwECore.Auxiliary
Imports EwEUtils
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for editing the gradient part of a <see cref="cVisualStyle"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucEditGradient

#Region " Private parts "

        Private m_lColors As New List(Of Color)
        Private m_bReady As Boolean = False

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
        Public Sub New(ByVal uic As cUIContext,
                       ByVal vs As cVisualStyle,
                       ByVal style As cVisualStyle.eVisualStyleTypes)

            MyBase.New(uic, vs, style)
            Me.InitializeComponent()

            Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
            Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
            Me.SetStyle(ControlStyles.ResizeRedraw, True)

            Me.UpdateControls()

        End Sub

#End Region ' Constructor

#Region " Overrides "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_bInUpdate = True

            Me.m_bReady = True
            Me.VisualStyle = Me.VisualStyle

            Me.UpdateControls()

            Me.m_bInUpdate = False
        End Sub

        ''' <summary>
        ''' Paint the control background to render the current gradient.
        ''' </summary>
        Protected Overrides Sub OnPaintBackground(ByVal e As System.Windows.Forms.PaintEventArgs)

            MyBase.OnPaintBackground(e)

            Dim rc As Rectangle = Me.m_plGradient.ClientRectangle
            Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)

            rc.X = Me.m_plGradient.Location.X
            rc.Y = Me.m_plGradient.Location.Y
            e.Graphics.FillRectangle(New SolidBrush(Color.White), rc)

            cColorRampIndicator.DrawColorRamp(e.Graphics, ramp, rc)
            ControlPaint.DrawBorder3D(e.Graphics, rc)

        End Sub

        Public Overrides Function Apply(ByVal vs As cVisualStyle) As Boolean

            Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)

            vs.ColorRampID = ramp.ID
            If (TypeOf ramp Is cARGBColorRamp) Then
                Dim argb As cARGBColorRamp = DirectCast(ramp, cARGBColorRamp)
                vs.ColorRampBreaks = argb.GradientBreaks
                vs.ColorRampColors = argb.GradientColors
            Else
                vs.ColorRampBreaks = Nothing
                vs.ColorRampColors = Nothing
            End If

            Return True

        End Function

        Public Overrides Property VisualStyle As cVisualStyle
            Get
                Return MyBase.VisualStyle
            End Get
            Set(value As cVisualStyle)
                MyBase.VisualStyle = value
                If Not Me.m_bReady Then Return

                Dim aGrads() As cVisualStyle = Me.UIContext.StyleGuide.GetVisualStyles(-1, cStyleGuide.eBrushType.Gradient)
                Dim iSel As Integer = 0
                Me.m_cmbGradient.Items.Clear()

                Me.m_cmbGradient.Items.Add(New cEwEColorRamp())
                Me.m_cmbGradient.Items.Add(New cViridisColorRamp(cViridisColorRamp.eViridisOptions.A))
                Me.m_cmbGradient.Items.Add(New cViridisColorRamp(cViridisColorRamp.eViridisOptions.B))
                Me.m_cmbGradient.Items.Add(New cViridisColorRamp(cViridisColorRamp.eViridisOptions.C))
                Me.m_cmbGradient.Items.Add(New cViridisColorRamp(cViridisColorRamp.eViridisOptions.D))
                Me.m_cmbGradient.Items.Add(New cViridisColorRamp(cViridisColorRamp.eViridisOptions.E))

                For i As Integer = 0 To aGrads.Length - 1
                    Dim vs As cVisualStyle = aGrads(i)
                    Me.m_cmbGradient.Items.Add(New cARGBColorRamp(vs.ColorRampColors, vs.ColorRampBreaks))
                Next i

                ' Set selection
                For i As Integer = 0 To Me.m_cmbGradient.Items.Count - 1
                    Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.Items(i), cColorRamp)
                    If (Me.VisualStyle.ColorRampID = ramp.ID) Then
                        If (TypeOf ramp Is cARGBColorRamp) Then
                            Dim argb As cARGBColorRamp = DirectCast(ramp, cARGBColorRamp)
                            If argb.GradientBreaks.EqualsArray(Me.VisualStyle.ColorRampBreaks) And
                                argb.GradientColors.EqualsArray(Me.VisualStyle.ColorRampColors) Then
                                iSel = i
                            End If
                        Else
                            iSel = i
                        End If
                    End If
                Next

                Me.m_cmbGradient.SelectedIndex = iSel

            End Set
        End Property

#End Region ' Overrides

#Region " Events "

        'Private Sub OnGradientTypeChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
        '    Handles m_rbDefaultGradient.CheckedChanged, m_rbCustomGradient.CheckedChanged
        '    Me.UpdateControls()
        'End Sub

        ''' <summary>
        ''' User clicked CurrentColor box to pick a colour
        ''' </summary>
        Private Sub OnPickColour(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_pbCurrentColor.Click
            Try
                Me.PickColor()
            Catch ex As Exception
            End Try
        End Sub

        ''' <summary>
        ''' User altered a colour value via a numeric up/down control.
        ''' </summary>
        Private Sub OnColorValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_nudRed.ValueChanged, m_nudGreen.ValueChanged, m_nudBlue.ValueChanged, m_nudAlpha.ValueChanged

            If (Me.m_bInUpdate) Then Return

            Dim clr As Color = Color.FromArgb(CInt(m_nudAlpha.Value), CInt(m_nudRed.Value), CInt(m_nudGreen.Value), CInt(m_nudBlue.Value))
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = clr
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User altered a colour value via a slider.
        ''' </summary>
        Private Sub OnColourSliderChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_slRed.ValueChanged, m_slBlue.ValueChanged, m_slGreen.ValueChanged, m_slAlpha.ValueChanged

            If (Me.m_bInUpdate) Then Return

            Dim clr As Color = Color.FromArgb(CInt(m_slAlpha.Value), CInt(m_slRed.Value), CInt(m_slGreen.Value), CInt(m_slBlue.Value))
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = clr
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User added a gradient break.
        ''' </summary>
        Private Sub OnAddBreak(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnAdd.Click

            Dim grad As cARGBColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cARGBColorRamp)
            Me.m_slGradient.Add()
            Me.m_lColors.Add(grad.GetColor(Me.m_slGradient.Value(0) / Me.m_slGradient.Maximum))
            Me.UpdateARGBGradient(grad)
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' User removed a gradient break.
        ''' </summary>
        Private Sub OnRemoveBreak(ByVal sender As System.Object, ByVal e As System.EventArgs) _
            Handles m_btnRemove.Click

            Dim iKnob As Integer = Me.m_slGradient.CurrentKnob
            If Me.m_lColors.Count > 2 Then
                Me.m_slGradient.Remove(iKnob)
                Me.m_lColors.RemoveAt(iKnob)
                Dim grad As cARGBColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cARGBColorRamp)
                Me.UpdateARGBGradient(grad)
            End If
            Me.UpdateControls()

        End Sub

        ''' <summary>
        ''' User selected a different knob in the gradient slider.
        ''' </summary>
        Private Sub OnGradientSliderCurrentKnobChanged(ByVal sender As Object, ByVal e As SliderKnobChangedEventArgs) _
            Handles m_slGradient.CurrentKnobChanged

            If (Me.m_bInUpdate) Then Return
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' User selected a different value in the gradient slider.
        ''' </summary>
        Private Sub OnGradientSliderValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_slGradient.ValueChanged

            If (Me.m_bInUpdate) Then Return
            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>
        ''' Draw an item in the gradient combo box.
        ''' </summary>
        Private Sub OnDrawGradientComboBoxItem(ByVal sender As Object, ByVal e As System.Windows.Forms.DrawItemEventArgs) _
            Handles m_cmbGradient.DrawItem

            ' Sanity check
            If (e.Index < 0) Then Return

            Try

                Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.Items(e.Index), cColorRamp)
                Dim rc As Rectangle = e.Bounds

                If (e.Index < 0) Then
                    e.DrawBackground()
                    e.DrawFocusRectangle()
                    Return
                End If

                e.DrawBackground()
                rc.Inflate(-2, -2)
                cColorRampIndicator.DrawColorRamp(e.Graphics, ramp, rc)
                e.DrawFocusRectangle()

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
                System.Console.WriteLine(Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' User selected a gradient from the combo box.
        ''' </summary>
        Private Sub OnGradientSelected(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles m_cmbGradient.SelectedIndexChanged

            If (Me.m_bInUpdate) Then Return

            Try

                Dim grad As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)
                If (TypeOf (grad) Is cARGBColorRamp) Then
                    Dim argb As cARGBColorRamp = DirectCast(grad, cARGBColorRamp)
                    Me.SetARGBGradient(argb.GradientBreaks, argb.GradientColors)
                Else
                    Me.SetARGBGradient(Nothing, Nothing)
                End If
                Me.m_tbxName.Text = grad.Name
                Me.m_tbxName.Enabled = Not grad.IsSystemRamp()

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OnGradientSelected() Exception " & ex.Message)
                System.Console.WriteLine(Me.ToString & ".OnGradientSelected() Exception " & ex.Message)
            End Try

        End Sub

        ''' <summary>
        ''' Flip gradient
        ''' </summary>
        Private Sub OnFlipGradient(ByVal sender As System.Object, ByVal e As System.EventArgs) _
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
                Dim grad As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)
                If (Not grad.IsSystemRamp) Then
                    grad.Name = Me.m_tbxName.Text
                End If
            Catch ex As Exception

            End Try
        End Sub

#End Region ' Events 

#Region " Internals "

        Private Sub SetARGBGradient(ByVal breaks() As Double, ByVal colors() As Color)

            Me.m_bInUpdate = True

            If (breaks IsNot Nothing) And (colors IsNot Nothing) Then

                Me.m_lColors.Clear()
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

            Dim bIsEditableGradient As Boolean = (TypeOf Me.m_cmbGradient.SelectedItem Is cARGBColorRamp) And (Me.RepresentationStyles And cVisualStyle.eVisualStyleTypes.Gradient) > 0
            Dim iNumKnobs As Integer = Me.m_slGradient.NumKnobs

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

            Me.ApplyColorsToGradient()

        End Sub

        ''' <summary>Loop prevention flag.</summary>
        Private m_bInUpdate As Boolean = False

        Private Sub ApplyColorsToGradient()

            If m_bInUpdate Then Return

            Me.m_bInUpdate = True

            If (TypeOf Me.m_cmbGradient.SelectedItem Is cARGBColorRamp) Then

                Try

                    Dim clr As Color = Me.m_lColors(Me.m_slGradient.CurrentKnob)
                    Me.m_pbCurrentColor.BackColor = clr

                    Me.m_slRed.Value = clr.R
                    Me.m_nudRed.Value = clr.R

                    Me.m_slGreen.Value = clr.G
                    Me.m_nudGreen.Value = clr.G

                    Me.m_slBlue.Value = clr.B
                    Me.m_nudBlue.Value = clr.B

                    Me.m_slAlpha.Value = clr.A
                    Me.m_nudAlpha.Value = clr.A

                    Dim grad As cARGBColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cARGBColorRamp)
                    Me.UpdateARGBGradient(grad)

                    Me.VisualStyle.ColorRampBreaks = grad.GradientBreaks
                    Me.VisualStyle.ColorRampColors = grad.GradientColors

                Catch ex As Exception
                    Debug.Assert(False, Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
                    System.Console.WriteLine(Me.ToString & ".OnDrawGradientComboBoxItem() Exception " & ex.Message)
                End Try
            End If

            Me.FireStyleChangedEvent()
            Me.m_bInUpdate = False

            Me.Refresh()

        End Sub

        Private Sub PickColor()

            Dim dlg As New cEwEColorDialog()
            dlg.Color = Me.m_lColors(Me.m_slGradient.CurrentKnob)
            If dlg.ShowDialog(Me) <> DialogResult.OK Then Return
            Me.m_lColors(Me.m_slGradient.CurrentKnob) = dlg.Color

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
            Dim lColor As New List(Of Color)
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

        End Sub

#End Region ' Internal implementation

    End Class

End Namespace
