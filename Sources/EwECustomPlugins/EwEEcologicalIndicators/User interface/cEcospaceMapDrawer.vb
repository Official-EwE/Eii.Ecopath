' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports System.Drawing
Imports EwECore
Imports ScientificInterfaceShared.Controls.Map
Imports ScientificInterfaceShared.Style

''' <summary>
''' Renderer for Ecospace-derived indicators
''' </summary>
Public Class cEcospaceMapDrawer
    Inherits cMapDrawerBase

    Public Sub New(core As cCore, sg As cStyleGuide, ind As cIndicatorInfo)
        MyBase.New(core, sg)
        Me.Indicator = ind
    End Sub

    ''' <summary>The labels to use for each plot.</summary>
    Public Property Labels As String()
    ''' <summary>Descriptions to use for each plot.</summary>
    ''' <remarks>Can be used in tooltips.</remarks>
    Public Property Descriptions As String()

    Public Property Indicator As cIndicatorInfo

    Public Overrides Sub DrawMap(iItem As Integer, rcPos As Rectangle, Args As cMapDrawerArgs)

        If Me.Map Is Nothing Then Return

        Dim maptype As cMapDrawerBase.eMapType = Args.MapType
        Dim RelScaler() As Single = Args.RelMapScaler
        Dim excl As cEcospaceLayerExclusion = Me.m_core.EcospaceBasemap.LayerExclusion

        Using font As Font = Me.m_sg.Font(cStyleGuide.eApplicationFontType.SubTitle)

            For i As Integer = 1 To Me.InRow
                For j As Integer = 1 To Me.InCol
                    If CBool(excl.Cell(i, j)) = False Then

                        Try
                            Dim sMapValue As Single = 1.0E-20
                            Dim icc As Single
                            Dim rcfCell As RectangleF = New RectangleF(CSng(rcPos.Left + (j - 1) * rcPos.Width() / Me.InCol),
                                                                   CSng(rcPos.Top + (i - 1) * rcPos.Height() / Me.InRow),
                                                                   CSng(rcPos.Width() / Me.InCol),
                                                                   CSng(rcPos.Height() / Me.InRow))
                            Dim brCell As Brush = Nothing

                            'If it is water
                            If Me.m_core.EcospaceBasemap.LayerDepth.IsWaterCell(i, j) Then
                                ' Water Cell
                                sMapValue = Me.Map(i, j, iItem) / RelScaler(iItem)

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

                            ElseIf Me.ShowLand Then
                                ' #Land
                                brCell = New SolidBrush(Color.Gray)
                            Else
                                brCell = New SolidBrush(Color.Transparent)
                            End If

                            Me.Graphics.FillRectangle(brCell, rcfCell)
                            brCell.Dispose()

                        Catch ex As Exception
                            'Debug.Assert(False, ex.Message)
                            Exit Sub
                        End Try
                    End If

                Next
            Next

            Try
                MyBase.DrawMap(iItem, rcPos, Args)
            Catch ex As Exception
                ' This can happen during disposal. Catch and cleanly move on
            End Try

            Try

                If Me.m_sg.ShowMapLabels And Me.Labels IsNot Nothing Then
                    Dim br As Brush = Brushes.Black
                    Dim fmt As New StringFormat()

                    fmt.Alignment = Me.m_sg.MapLabelPosHorizontal
                    fmt.LineAlignment = Me.m_sg.MapLabelPosVertical

                    If Me.m_sg.InvertMapLabelColor Then br = Brushes.White

                    Me.Graphics.DrawString(Me.Labels(iItem), font, br, rcPos, fmt)
                End If
            Catch ex As Exception
                ' Catch and cleanly move on
            End Try

        End Using

    End Sub

End Class
