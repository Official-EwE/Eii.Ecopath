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
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports EwECore
Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Style

#End Region 'Imports

Namespace Controls.Map.Layers

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

        Private m_colorRamp As New cEwEColorRamp()

        Public Sub New(ByVal vs As cVisualStyle)
            MyBase.New(vs, cVisualStyle.eVisualStyleTypes.ForeColor Or _
                    cVisualStyle.eVisualStyleTypes.Font Or _
                    cVisualStyle.eVisualStyleTypes.Gradient)
        End Sub

        Public Overrides Sub RenderPreview(ByVal g As Graphics, _
                                           ByVal rc As Rectangle, _
                                           ByVal layer As cEcospaceLayer)

            If Me.m_brFore Is Nothing Then Me.Update()
            g.FillRectangle(Brushes.Gray, rc)
            g.DrawString("#", Me.m_ft, Me.m_brFore, rc)
        End Sub

        Public Overrides Sub RenderCell(ByVal g As System.Drawing.Graphics, _
                                        ByVal rc As System.Drawing.Rectangle, _
                                        ByVal layer As cEcospaceLayer, _
                                        ByVal value As Object, _
                                        ByVal style As cStyleGuide.eStyleFlags)

            Try
                Dim sValue As Single = CSng(value)
                Dim sValueMax As Single = layer.MaxValue

                ' Is non-water cell?
                If (sValue <= 0) Then
                    ' #Yes: draw in black
                    g.FillRectangle(Brushes.Gray, rc)
                Else
                    ' #No: only draw colours when highlighted

                    ' Highlighted? draw values in colour + value on top
                    If ((style And cStyleGuide.eStyleFlags.Highlight) = cStyleGuide.eStyleFlags.Highlight) Then

                        If (Me.m_brFore Is Nothing) Then Me.Update()

                        If (value IsNot Nothing) And (Me.m_ft IsNot Nothing) Then
                            ' Calculate the cell color based on the cell value RELATIVE TO [1, sValueMax),
                            Using br As New SolidBrush(m_colorRamp.GetColor(sValue - 1, sValueMax))
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
