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
Imports System.Linq

#End Region ' Imports

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

    Public Property MinSliceRatio As Double = 0.35
    Public Property DrawLabels As Boolean = True

    Public Class cTreeMapElement
        Public Property Label As String = ""
        Public Property Value As Double = 1
        Public Property Color As Color
    End Class

    Public Sub DrawTreemap(elements As IEnumerable(Of cTreeMapElement), gfx As Graphics, rect As Rectangle, font As Font)

        Dim slice As cSlice = Me.GetSlice(elements, 1, Me.MinSliceRatio)
        Dim rectangles As IEnumerable(Of cSliceRectangle) = GetRectangles(slice, rect.Width, rect.Height)

        gfx.FillRectangle(Brushes.White, rect)

        For Each r As cSliceRectangle In rectangles
            Dim rc As New Rectangle(rect.X + r.X, rect.Y + r.Y, r.Width - 1, r.Height - 1)
            Using br As New SolidBrush(r.Slice.Elements.First().Color)
                gfx.FillRectangle(br, rc)
            End Using
            gfx.DrawRectangle(Pens.Black, rc)

            If Me.DrawLabels Then
                gfx.DrawString(r.Slice.Elements.First().Label, font, Brushes.Black, rc)
            End If
        Next

    End Sub

#Region " Internals "

    Private Function GetSlice(elements As IEnumerable(Of cTreeMapElement), totalSize As Double, sliceWidth As Double) As cSlice

        If Not elements.Any() Then Return Nothing
        If elements.Count() = 1 Then Return New cSlice With {
            .Elements = elements,
            .Size = totalSize
        }
        Dim sliceResult As cSliceResult = GetElementsForSlice(elements, sliceWidth)
        Return New cSlice With {
            .Elements = elements,
            .Size = totalSize,
            .SubSlices = {GetSlice(sliceResult.Elements, sliceResult.ElementsSize, sliceWidth), GetSlice(sliceResult.RemainingElements, 1 - sliceResult.ElementsSize, sliceWidth)}
        }

    End Function

    Private Function GetElementsForSlice(elements As IEnumerable(Of cTreeMapElement), sliceWidth As Double) As cSliceResult
        Dim elementsInSlice As New List(Of cTreeMapElement)()
        Dim remainingElements As New List(Of cTreeMapElement)()
        Dim current As Double = 0
        Dim total As Double = elements.Sum(Function(x) x.Value)

        For Each element As cTreeMapElement In elements

            If current > sliceWidth Then
                remainingElements.Add(element)
            Else
                elementsInSlice.Add(element)
                current += element.Value / total
            End If
        Next

        Return New cSliceResult With {
            .Elements = elementsInSlice,
            .ElementsSize = current,
            .RemainingElements = remainingElements
        }
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

    Private Iterator Function GetRectangles(slice As cSlice, width As Integer, height As Integer) As IEnumerable(Of cSliceRectangle)

        Dim area As New cSliceRectangle With {
            .Slice = slice,
            .Width = width,
            .Height = height
        }
        For Each rect As cSliceRectangle In GetRectangles(area)
            If rect.X + rect.Width > area.Width Then rect.Width = area.Width - rect.X
            If rect.Y + rect.Height > area.Height Then rect.Height = area.Height - rect.Y
            Yield rect
        Next

    End Function

    Private Iterator Function GetRectangles(sliceRectangle As cSliceRectangle) As IEnumerable(Of cSliceRectangle)

        Dim isHorizontalSplit As Boolean = sliceRectangle.Width >= sliceRectangle.Height
        Dim currentPos As Integer = 0

        For Each subSlice As cSlice In sliceRectangle.Slice.SubSlices
            Dim subRect As New cSliceRectangle With {
                .Slice = subSlice
            }
            Dim rectSize As Integer

            If isHorizontalSplit Then
                rectSize = CInt(Math.Round(sliceRectangle.Width * subSlice.Size))
                subRect.X = sliceRectangle.X + currentPos
                subRect.Y = sliceRectangle.Y
                subRect.Width = rectSize
                subRect.Height = sliceRectangle.Height
            Else
                rectSize = CInt(Math.Round(sliceRectangle.Height * subSlice.Size))
                subRect.X = sliceRectangle.X
                subRect.Y = sliceRectangle.Y + currentPos
                subRect.Width = sliceRectangle.Width
                subRect.Height = rectSize
            End If

            currentPos += rectSize

            If subSlice.Elements.Count() > 1 Then
                For Each rc As cSliceRectangle In GetRectangles(subRect)
                    Yield rc
                Next
            ElseIf subSlice.Elements.Count() = 1 Then
                Yield subRect
            End If
        Next

    End Function

#End Region ' Internals

End Class
