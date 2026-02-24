' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells displaying the actual cell value number,
    ''' and scaling the cell background colour across a colour gradient based
    ''' on the cell value in relation to the layer min/max value range.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererDepth
        Inherits cLayerRendererValue

        Public Sub New(uic As cUIContext, vs As cVisualStyle)
            MyBase.New(uic, vs)
        End Sub

        'Public Overrides Sub RenderPreview(g As Graphics, _
        '                                   rc As Rectangle)
        '    MyBase.RenderPreview(g, rc)
        '    If Me.ForeBrush Is Nothing Then Me.Update()
        '    g.DrawString("#", Me.Font, Me.ForeBrush, rc)
        'End Sub

        Public Overrides Sub RenderCell(g As System.Drawing.Graphics,
                                        rc As System.Drawing.RectangleF,
                                        layer As cEcospaceLayer,
                                        value As Object,
                                        style As cStyleGuide.eStyleFlags)

            Try
                Dim sValue As Single = CSng(value)
                Dim sValueMax As Single = layer.MaxValue
                Dim sValueMin As Single = layer.MinValue

                ' Is non-water cell?
                If (sValue <= 0) Then
                    ' #Yes: draw in black
                    g.FillRectangle(Brushes.Gray, rc)
                Else
                    ' #No: only draw colours when highlighted

                    ' Highlighted? draw values in colour + value on top
                    If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Then

                        If (Me.ForeBrush Is Nothing) Then Me.Update()

                        If (value IsNot Nothing) And (Me.Font IsNot Nothing) Then
                            ' Calculate the cell color based on the cell value RELATIVE TO [0, sValueMax),
                            Using br As New SolidBrush(Me.ColorRamp.GetColor(sValue - sValueMin, sValueMax - sValueMin))
                                g.FillRectangle(br, rc)
                            End Using
                        End If
                    End If
                End If

            Catch ex As Exception
                ' Boom
            End Try
        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            Return cStringUtils.FormatNumber(value)
        End Function

    End Class

End Namespace
