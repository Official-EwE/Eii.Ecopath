#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D
Imports System.Reflection
Imports EwECore.Auxiliary

#End Region 'Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells values directly as text. The cell 
    ''' background colour is obtained from the visual style.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererText
        Inherits cLayerRenderer

        Private m_brFore As Brush = Nothing
        Private m_ft As Font = Nothing

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor Or _
                    cVisualStyle.eVisualStyleTypes.Font Or _
                    cVisualStyle.eVisualStyleTypes.Gradient)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)

            If Me.m_brFore Is Nothing Then Me.Update()

            If Me.IsStyleValid Then
                g.FillRectangle(Brushes.White, rc)
                g.DrawString("Aa", Me.m_ft, Me.m_brFore, rc)
            Else
                Me.RenderError(g, rc)
            End If

        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, _
                                        ByVal rc As System.Drawing.Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)

            Try
                If Me.m_brFore Is Nothing Then Me.Update()

                ' Draw background
                ' Render value on top for highlighted layers
                If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Then

                    If (value IsNot Nothing) And (Me.m_ft IsNot Nothing) Then
                        Dim strValue As String = CStr(value)
                        Using br As New SolidBrush(Me.VisualStyle.BackColour)
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
                Me.m_brFore = cLayerRenderer.brDEFAULT
            Else
                Me.m_brFore = New SolidBrush(Me.VisualStyle.ForeColour)
                Me.m_ft = New Font(Me.VisualStyle.FontName, Me.VisualStyle.FontSize, Me.VisualStyle.FontStyle)
            End If
        End Sub

        Protected Overrides Function IsStyleValid() As Boolean
            If Not MyBase.IsStyleValid() Then Return False
            Return (Not String.IsNullOrEmpty(Me.VisualStyle.FontName) Or (Me.VisualStyle.FontSize > 1))
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            MyBase.Dispose(disposing)
            Me.m_ft.Dispose()
            Me.m_ft = Nothing
            Me.m_brFore.Dispose()
            Me.m_brFore = Nothing
        End Sub

    End Class

End Namespace