'==============================================================================
'
' $Log: cLayerRendererDepth.vb,v $
' Revision 1.2  2009/05/28 12:37:16  jeroens
' Properly named utility classes StyleGuide and ZedGraphHelper
'
' Revision 1.1  2009/01/17 03:23:51  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterface.Other
Imports SAUPUtil.Misc.Colours
Imports System.Drawing.Drawing2D
Imports System.Reflection

#End Region 'Imports

Namespace Ecospace.Basemap.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Layer renderer that draws cells displaying the actual cell value number,
    ''' and scaling the cell background colour across a colour gradient based
    ''' on the cell value in relation to the layer min/max value range.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerRendererDepth
        Inherits cLayerRenderer

        Private m_brFore As Brush = Nothing
        Private m_ft As Font = Nothing
        Private m_sValueMax As Single = Nothing

        Private m_colorRamp As New SAUPColorRamp

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor Or _
                    cVisualStyle.eVisualStyleTypes.Font Or _
                    cVisualStyle.eVisualStyleTypes.Gradient)
        End Sub

        Public Overrides Sub SetValueRange(ByVal sMin As Object, ByVal sMax As Object)
            ' Min ignored, is always 0. We don't do mountains.
            Me.m_sValueMax = CSng(sMax)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, ByVal rc As Rectangle)
            If Me.m_brFore Is Nothing Then Me.Update()
            g.FillRectangle(Brushes.Black, rc)
        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, ByVal rc As System.Drawing.Rectangle, ByVal value As Object, _
                ByVal style As cStyleGuide.eStyleFlags)

            Try
                Dim sValue As Single = CSng(value)

                ' Is non-water cell?
                If (sValue <= 0) Then
                    ' #Yes: draw in black
                    g.FillRectangle(Brushes.Black, rc)
                Else
                    ' #No: only draw colours when highlighted

                    ' Highlighted? draw values in colour + value on top
                    If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Then

                        If (Me.m_brFore Is Nothing) Then Me.Update()

                        If (value IsNot Nothing) And (Me.m_ft IsNot Nothing) Then
                            ' Calculate the cell color based on the cell value RELATIVE TO [1, sValueMax),
                            Using br As New SolidBrush(m_colorRamp.GetColor(sValue - 1, Me.m_sValueMax))
                                g.FillRectangle(br, rc)
                            End Using
                        End If
                        ' Draw value
                        g.DrawString(String.Format("{0}", value), Me.m_ft, Me.m_brFore, rc)
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
