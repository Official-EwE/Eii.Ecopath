' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells values directly as text. The cell 
    ''' background colour is obtained from the visual style.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererText
        Inherits cRasterLayerRenderer

        Private m_brFore As Brush = Nothing
        Private m_ft As Font = Nothing

        Public Sub New(uic As cUIContext, vs As cVisualStyle)
            MyBase.New(uic, vs, cVisualStyle.eVisualStyleTypes.ForeColor Or
                    cVisualStyle.eVisualStyleTypes.Font Or
                    cVisualStyle.eVisualStyleTypes.Gradient)
        End Sub

        Public Overrides Sub RenderPreview(g As Graphics,
                                           rc As RectangleF,
                                           Optional iSymbol As Integer = 0)

            If Me.m_brFore Is Nothing Then Me.Update()

            If Me.IsStyleValid Then
                g.FillRectangle(Brushes.White, rc)
                g.DrawString("Aa", Me.m_ft, Me.m_brFore, rc)
            Else
                Me.RenderError(g, rc)
            End If

        End Sub

        Public Overrides Sub RenderCell(g As System.Drawing.Graphics,
                                        rc As System.Drawing.RectangleF,
                                        layer As cEcospaceLayer,
                                        value As Object,
                                        style As cStyleGuide.eStyleFlags)

            Try
                If Me.m_brFore Is Nothing Then Me.Update()

                ' Draw background
                ' Render value on top for highlighted layers
                If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Then

                    If (value IsNot Nothing) And (Me.m_ft IsNot Nothing) Then
                        Dim strValue As String = CStr(value)
                        Using br As New SolidBrush(cStyleGuide.FromVisualColor(Me.VisualStyle.BackColour))
                            g.FillRectangle(br, rc)
                        End Using
                        ' Draw value
                        g.DrawString(strValue, Me.m_ft, Me.m_brFore, rc)

                    End If
                End If
            Catch ex As Exception
                ' Boom
            End Try
        End Sub

        Public Overrides Sub Update()
            If Me.VisualStyle Is Nothing Then
                Me.m_brFore = cRasterLayerRenderer.brDEFAULT
            Else
                Me.m_brFore = New SolidBrush(cStyleGuide.FromVisualColor(Me.VisualStyle.ForeColour))
                Me.m_ft = New Font(Me.VisualStyle.FontName, Me.VisualStyle.FontSize, cStyleGuide.FromVisualFontStyle(Me.VisualStyle.FontStyle))
            End If
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            If Not MyBase.IsStyleValid() Then Return False
            Return (Not String.IsNullOrEmpty(Me.VisualStyle.FontName) Or (Me.VisualStyle.FontSize > 1))
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            Me.m_ft.Dispose()
            Me.m_ft = Nothing
            Me.m_brFore.Dispose()
            Me.m_brFore = Nothing
        End Sub

        Public Overrides Function GetDisplayText(value As Object) As String
            If String.IsNullOrWhiteSpace(CStr(value)) Then Return ""
            Return CStr(value)
        End Function

    End Class

End Namespace