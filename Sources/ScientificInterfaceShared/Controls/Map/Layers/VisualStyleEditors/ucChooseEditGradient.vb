' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports EwEUtils
Imports ScientificInterfaceShared.Style

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for editing the gradient part of a <see cref="cVisualStyle"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucChooseEditGradient

#Region " Private vars "

        Private m_bReady As Boolean = False

#End Region ' Private vars

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

        End Sub

#End Region ' Constructor

#Region " Overrides "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Me.m_bReady = True

            Me.VisualStyle = Me.VisualStyle

        End Sub

        Public Overrides Function Apply(vs As cVisualStyle) As Boolean

            Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)

            If (ramp IsNot Nothing) Then
                vs.ColorRampID = ramp.ID
                If (TypeOf ramp Is cARGBColorRamp) Then
                    Dim argb As cARGBColorRamp = DirectCast(ramp, cARGBColorRamp)
                    vs.ColorRampBreaks = argb.GradientBreaks
                    vs.ColorRampColors = argb.GradientColors
                Else
                    vs.ColorRampBreaks = Nothing
                    vs.ColorRampColors = Nothing
                End If
            End If

            Return True

        End Function

        Public Overrides Property VisualStyle As cVisualStyle
            Get
                Return MyBase.VisualStyle
            End Get
            Set(value As cVisualStyle)
                MyBase.VisualStyle = value

                If (Not Me.m_bReady) Then Return

                Dim iSel As Integer = -1
                Me.m_cmbGradient.Items.Clear()
                Me.m_cmbGradient.Items.AddRange(Me.UIContext.StyleGuide.ColorRamps)

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

                If (iSel = -1) Then
                    If (value IsNot Nothing) Then
                        If (value.ColorRampID > 0) Then
                            iSel = Me.m_cmbGradient.Items.Add(New cARGBColorRamp("Custom", value.ColorRampColors, value.ColorRampBreaks))
                        Else
                            iSel = 0
                        End If
                    End If
                End If
                Me.m_cmbGradient.SelectedIndex = iSel

            End Set
        End Property

#End Region ' Overrides

#Region " Events "

        ''' <summary>
        ''' Draw an item in the gradient combo box.
        ''' </summary>
        Private Sub OnDrawGradientComboBoxItem(sender As Object, e As System.Windows.Forms.DrawItemEventArgs) _
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
        Private Sub OnGradientSelected(sender As Object, e As System.EventArgs) _
            Handles m_cmbGradient.SelectedIndexChanged

            Try
                Me.m_editor.ColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)
                FireStyleChangedEvent()

            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".OnGradientSelected() Exception " & ex.Message)
                System.Console.WriteLine(Me.ToString & ".OnGradientSelected() Exception " & ex.Message)
            End Try

        End Sub

        Private Sub OnColorRampEdited(sender As Object, args As cColorRamp) Handles m_editor.OnColorRampChanged

            Dim ramp As cColorRamp = DirectCast(Me.m_cmbGradient.SelectedItem, cColorRamp)
            If (ramp Is Nothing) Then Return
            If (ramp.IsSystemRamp) Then Return
            If (Not TypeOf ramp Is cARGBColorRamp) Then Return
            If (Not TypeOf args Is cARGBColorRamp) Then Return

            Dim out As cARGBColorRamp = DirectCast(ramp, cARGBColorRamp)
            Dim [in] As cARGBColorRamp = DirectCast(args, cARGBColorRamp)

            out.GradientBreaks = [in].GradientBreaks
            out.GradientColors = [in].GradientColors
            out.Name = [in].Name

            Me.m_cmbGradient.Invalidate()

            FireStyleChangedEvent()

        End Sub

#End Region ' Events 

    End Class

End Namespace
