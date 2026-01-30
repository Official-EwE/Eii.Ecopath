' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Utilities



Namespace Controls

    ''' <summary>
    ''' <para>TreeMap renderer, based on https://pascallaurin42.blogspot.com/2013/12/implementing-treemap-in-c.html</para>
    ''' <para>Changes:
    ''' <list type=" bullet">
    ''' <item>Abolished templated logic</item>
    ''' <item>Added colors</item>
    ''' <item>Added rendering customization flags</item>
    ''' <item>Reduced rendering logic to one public call, hidden all internal logic and data types</item>
    ''' </list>
    ''' </para>
    ''' </summary>
    Public Class cTreeMapRenderer

        Private m_uic As cUIContext = Nothing

        Public Class cTreeMapElement
            Public Property Label As String = ""
            Public Property Value As Single = 1
            Public Property Color As Color
        End Class

        Public Sub New(uic As cUIContext)
            Me.m_uic = uic
        End Sub

        Public Property MinSliceRatio As Double = 0.35
        Public Property DrawBorders As Boolean = True
        Public Property DrawCaptions As Boolean = True
        Public Property DrawDataLabels As Boolean = True

        Public Sub DrawTreemap(elements As IEnumerable(Of cTreeMapElement), caption As String, gfx As Graphics, rect As Rectangle)

            If (Me.m_uic Is Nothing) Then Return

            Dim fmt As New StringFormat()
            fmt.Alignment = StringAlignment.Center
            fmt.LineAlignment = StringAlignment.Center

            If (Me.DrawCaptions) Then
                Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.SubTitle)
                    Dim rcCaption As New Rectangle(rect.X, rect.Y, rect.Width, ft.Height + 3)
                    gfx.DrawString(caption, ft, Brushes.Black, rcCaption, fmt)
                    rect.Y += rcCaption.Height
                    rect.Height -= rcCaption.Height
                End Using
            End If

            If (elements.Count = 0) Then Return

            Dim slice As cSlice = Me.GetSlice(elements, 1, Me.MinSliceRatio)
            Dim rectangles As IEnumerable(Of cSliceRectangle) = Me.GetRectangles(slice, rect.Width, rect.Height)

            gfx.FillRectangle(Brushes.White, rect)

            For Each r As cSliceRectangle In rectangles
                Dim rc As New Rectangle(rect.X + r.X, rect.Y + r.Y, r.Width - 1, r.Height - 1)
                Dim clrBack As Color = r.Slice.Elements.First().Color
                Dim clrText As Color = If(cColorUtils.IsLight(clrBack), Color.Black, Color.White)

                Using br As New SolidBrush(clrBack)
                    gfx.FillRectangle(br, rc)
                End Using

                If (Me.DrawDataLabels) Then
                    Using ft As Font = Me.m_uic.StyleGuide.Font(cStyleGuide.eApplicationFontType.Scale)
                        Using brText As New SolidBrush(clrText)
                            gfx.DrawString(r.Slice.Elements.First().Label, ft, brText, rc, fmt)
                        End Using
                    End Using
                End If
            Next

            If (Me.DrawBorders) Then
                gfx.DrawRectangle(Pens.Black, rect)
            End If

        End Sub

#Region " Internals "

        Private Function GetSlice(elements As IEnumerable(Of cTreeMapElement), totalSize As Double, sliceWidth As Double) As cSlice

            Dim slice As cSlice = Nothing

            If (Not elements.Any()) Then Return slice

            If (elements.Count() = 1) Then
                slice = New cSlice()
                slice.Elements = elements
                slice.Size = totalSize
            Else
                Dim sliceResult As cSliceResult = Me.GetElementsForSlice(elements, sliceWidth)
                slice = New cSlice()
                slice.Elements = elements
                slice.Size = totalSize
                slice.SubSlices = {Me.GetSlice(sliceResult.Elements, sliceResult.ElementsSize, sliceWidth),
                                   Me.GetSlice(sliceResult.RemainingElements, 1 - sliceResult.ElementsSize, sliceWidth)}
            End If
            Return slice

        End Function

        Private Function GetElementsForSlice(elements As IEnumerable(Of cTreeMapElement), sliceWidth As Double) As cSliceResult

            Dim elementsInSlice As New List(Of cTreeMapElement)()
            Dim remainingElements As New List(Of cTreeMapElement)()
            Dim current As Double = 0
            Dim total As Single = elements.Sum(Function(x) x.Value)

            For Each element As cTreeMapElement In elements
                If current > sliceWidth Then
                    remainingElements.Add(element)
                Else
                    elementsInSlice.Add(element)
                    current += (element.Value / total)
                End If
            Next

            Dim result As New cSliceResult()
            result.Elements = elementsInSlice
            result.ElementsSize = current
            result.RemainingElements = remainingElements
            Return result

        End Function

        Private Class cSliceResult
            Public Property Elements As IEnumerable(Of cTreeMapElement)
            Public Property ElementsSize As Double
            Public Property RemainingElements As IEnumerable(Of cTreeMapElement)
        End Class

        Private Class cSlice
            Public Property Size As Double
            Public Property Elements As IEnumerable(Of cTreeMapElement)
            Public Property SubSlices As IEnumerable(Of cSlice)
        End Class

        Private Class cSliceRectangle
            Public Property Slice As cSlice
            Public Property X As Integer
            Public Property Y As Integer
            Public Property Width As Integer
            Public Property Height As Integer
        End Class

        Private Function GetRectangles(slice As cSlice, width As Integer, height As Integer) As IEnumerable(Of cSliceRectangle)

            Dim results As New List(Of cSliceRectangle)
            Dim area As New cSliceRectangle()
            area.Slice = slice
            area.Width = width
            area.Height = height

            If slice.SubSlices Is Nothing Then
                results.Add(area)
            Else
                For Each rect As cSliceRectangle In Me.GetRectangles(area)
                    If rect.X + rect.Width > area.Width Then rect.Width = area.Width - rect.X
                    If rect.Y + rect.Height > area.Height Then rect.Height = area.Height - rect.Y
                    results.Add(rect)
                Next
            End If

            Return results.ToArray()

        End Function

        Private Function GetRectangles(sliceRectangle As cSliceRectangle) As IEnumerable(Of cSliceRectangle)

            Dim isHorizontalSplit As Boolean = sliceRectangle.Width >= sliceRectangle.Height
            Dim currentPos As Integer = 0
            Dim results As New List(Of cSliceRectangle)

            For Each subSlice As cSlice In sliceRectangle.Slice.SubSlices
                Dim rectSize As Integer
                Dim subRect As New cSliceRectangle()
                subRect.Slice = subSlice

                If isHorizontalSplit Then
                    rectSize = CInt(Math.Ceiling(sliceRectangle.Width * subSlice.Size))
                    subRect.X = sliceRectangle.X + currentPos
                    subRect.Y = sliceRectangle.Y
                    subRect.Width = rectSize
                    subRect.Height = sliceRectangle.Height
                Else
                    rectSize = CInt(Math.Ceiling(sliceRectangle.Height * subSlice.Size))
                    subRect.X = sliceRectangle.X
                    subRect.Y = sliceRectangle.Y + currentPos
                    subRect.Width = sliceRectangle.Width
                    subRect.Height = rectSize
                End If

                currentPos += rectSize

                If subSlice.Elements.Count() > 1 Then
                    results.AddRange(Me.GetRectangles(subRect))
                ElseIf subSlice.Elements.Count() = 1 Then
                    results.Add(subRect)
                End If
            Next
            Return results

        End Function

#End Region ' Internals

    End Class

End Namespace
