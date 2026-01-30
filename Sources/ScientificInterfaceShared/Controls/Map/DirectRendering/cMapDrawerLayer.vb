' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Option Explicit On
Imports EwEUtils.Utilities
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Controls.Map

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' <see cref="cMapDrawerBase"/> for rendering data from Ecospace layers.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cMapDrawerLayer
        Inherits cMapDrawerBase

        Private m_vn As eVarNameFlags = eVarNameFlags.NotSet

        Public Sub New(core As cCore, sg As cStyleGuide, vn As eVarNameFlags)
            MyBase.New(core, sg)
            Me.m_vn = vn
        End Sub

        Overloads Property Map As eVarNameFlags

        Public Overrides Sub DrawMap(iItem As Integer, rcPos As Rectangle, Args As cMapDrawerArgs)

            Dim bm As cEcospaceBasemap = Me.m_core.EcospaceBasemap
            Dim excl As cEcospaceLayerExclusion = bm.LayerExclusion
            Dim map As cEcospaceLayer = bm.Layer(Me.Map, iItem)
            Dim brExcluded As New Drawing2D.HatchBrush(Drawing2D.HatchStyle.DiagonalCross, Color.Red, Color.FromArgb(&H88FF4500))
            Using br As New SolidBrush(Me.m_sg.ApplicationColor(cStyleGuide.eApplicationColorType.MAP_BACKGROUND))
                Me.Graphics.FillRectangle(br, rcPos)
            End Using
            Dim font As Font = Me.m_sg.Font(cStyleGuide.eApplicationFontType.SubTitle)

            map.Invalidate()
            Dim min As Single = map.MinValue
            Dim max As Single = map.MaxValue
            Dim range As Single = max - min

            Try
                For i As Integer = 1 To Me.InRow
                    For j As Integer = 1 To Me.InCol

                        Dim rcfCell As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / Me.InCol),
                                                                           CSng(rcPos.Top + (i - 1) * rcPos.Height() / Me.InRow),
                                                                           CSng(rcPos.Width() / Me.InCol),
                                                                           CSng(rcPos.Height() / Me.InRow))
                        Dim brCell As Brush = Nothing

                        'If it is water
                        If m_core.EcospaceBasemap.LayerDepth.IsWaterCell(i, j) Then
                            ' Is not excluded
                            If (Not excl.IsExcludedCell(i, j)) Then
                                ' Water Cell
                                Dim sMapValue As Single = 0
                                Dim icc As Single

                                If (range = 0) Then
                                    sMapValue = 1 ' CSng(map.Cell(i, j))
                                Else
                                    sMapValue = (CSng(map.Cell(i, j)) - min) / range
                                End If

                                If (sMapValue > 10.0!) Or Single.IsPositiveInfinity(sMapValue) Then
                                    icc = Me.Colors.Count
                                ElseIf (sMapValue < 0.1!) Or Single.IsNegativeInfinity(sMapValue) Or Single.IsNaN(sMapValue) Then
                                    icc = 1
                                Else
                                    icc = Me.Colors.Count * sMapValue / (sMapValue + 1)
                                End If

                                'Boundary check
                                icc = Math.Max(Math.Min(Me.Colors.Count - 1, icc), 1)
                                brCell = New SolidBrush(Me.Colors(CInt(icc)))
                            End If
                        ElseIf Me.ShowLand Then
                            ' #Land
                            brCell = New SolidBrush(Color.Gray)
                        End If

                        If (brCell IsNot Nothing) Then
                            Me.Graphics.FillRectangle(brCell, rcfCell)
                            brCell.Dispose()
                        End If

                        ' Always show excluded?
                        If Me.ShowExcluded And excl.IsExcludedCell(i, j) = True Then
                            ' Draw excluded Cell 
                            Me.Graphics.FillRectangle(brExcluded, rcfCell)
                        End If
                    Next
                Next

            Catch ex As Exception
                'Debug.Assert(False, ex.Message)
                Exit Sub
            End Try

            brExcluded.Dispose()
            brExcluded = Nothing

            MyBase.DrawMap(iItem, rcPos, Args)

            If Me.m_sg.ShowMapLabels Then

                Dim strLabel As String = ""
                Dim strName As String = ""

                If (Me.m_sg.ShowMapsIndexInLabels) Then
                    strName = cStringUtils.Localize(My.Resources.GENERIC_LABEL_INDEXED, iItem, bm.Layer(Me.m_vn, iItem).Name)
                Else
                    strName = bm.Layer(Me.m_vn, iItem).Name
                End If
                If Me.m_sg.ShowMapsDateInLabels Then
                    strLabel = String.Format(SharedResources.Resources.GENERIC_LABEL_DOUBLE, strName, Me.Date)
                Else
                    strLabel = strName
                End If
                Dim br As Brush = Brushes.Black
                Dim fmt As New StringFormat()

                fmt.Alignment = Me.m_sg.MapLabelPosHorizontal
                fmt.LineAlignment = Me.m_sg.MapLabelPosVertical

                If Me.m_sg.InvertMapLabelColor Then br = Brushes.White

                Me.Graphics.DrawString(strLabel, font, br, rcPos, fmt)
            End If

            font.Dispose()

        End Sub

    End Class

End Namespace