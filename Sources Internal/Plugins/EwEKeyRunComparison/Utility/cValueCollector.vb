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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
Imports System.Text
Imports EwECore
Imports EwECore.ValueWrapper
Imports EwECore.SpatialData

Public Class cValueCollector

    Private m_lData As New List(Of Byte)

    Public Sub New()

    End Sub

    ' -- Primitives --

    Public Function Add(value As Boolean) As Boolean
        Me.m_lData.AddRange(BitConverter.GetBytes(value))
        Return True
    End Function

    Public Function Add(value As Integer) As Boolean
        Me.m_lData.AddRange(BitConverter.GetBytes(value))
        Return True
    End Function

    Public Function Add(value As Single) As Boolean
        Me.m_lData.AddRange(BitConverter.GetBytes(value))
        Return True
    End Function

    Public Function Add(value As String) As Boolean
        Me.m_lData.AddRange(System.Text.Encoding.ASCII.GetBytes(value))
        Return True
    End Function

    ' -- Bigger blocks --

    Public Function Add(value As cValue) As Boolean

        If (value Is Nothing) Then Return False

        Select Case value.varType
            Case eValueTypes.Bool
                Me.m_lData.AddRange(BitConverter.GetBytes(CBool(value.Value)))
            Case eValueTypes.Int
                Me.m_lData.AddRange(BitConverter.GetBytes(CInt(value.Value)))
            Case eValueTypes.Sng
                Me.m_lData.AddRange(BitConverter.GetBytes(CSng(value.Value)))
            Case eValueTypes.BoolArray
                For i As Integer = 0 To value.Length
                    Me.m_lData.AddRange(BitConverter.GetBytes(CBool(value.Value(i))))
                Next
            Case eValueTypes.IntArray
                For i As Integer = 0 To value.Length
                    Me.m_lData.AddRange(BitConverter.GetBytes(CInt(value.Value(i))))
                Next
            Case eValueTypes.SingleArray
                For i As Integer = 0 To value.Length
                    Me.m_lData.AddRange(BitConverter.GetBytes(CSng(value.Value(i))))
                Next
            Case eValueTypes.Str
                Me.m_lData.AddRange(Encoding.Unicode.GetBytes(CStr(value.Value)))
            Case Else
                Debug.Assert(False)
        End Select

        Return False

    End Function

    Public Function Add(value As cShapeData) As Boolean

        If (value Is Nothing) Then Return False

        Dim pts As Single() = value.ShapeData ' Note that ShapeData returns a copy!
        For i As Integer = 0 To value.nPoints - 1
            Me.m_lData.AddRange(BitConverter.GetBytes(pts(i)))
        Next
        Return True

    End Function

    Public Function Add(nRow As Integer, nCol As Integer, value As cEcospaceLayerInteger) As Boolean
        For i As Integer = 1 To nRow
            For j As Integer = 1 To nCol
                Me.Add(CInt(value.Cell(i, j)))
            Next
        Next
        Return True
    End Function

    Public Function Add(nRow As Integer, nCol As Integer, value As cEcospaceLayerSingle) As Boolean
        For i As Integer = 1 To nRow
            For j As Integer = 1 To nCol
                Me.Add(CSng(value.Cell(i, j)))
            Next
        Next
        Return True
    End Function

    Public Function Add(nRow As Integer, nCol As Integer, value As cEcospaceLayerBoolean) As Boolean
        For i As Integer = 1 To nRow
            For j As Integer = 1 To nCol
                Me.Add(CBool(value.Cell(i, j)))
            Next
        Next
        Return True
    End Function

    Public Function Add(nRow As Integer, nCol As Integer, value As cEcospaceLayerVector) As Boolean
        For i As Integer = 1 To nRow
            For j As Integer = 1 To nCol
                Me.Add(value.XVelocity(i, j))
                Me.Add(value.YVelocity(i, j))
            Next
        Next
        Return True
    End Function

    Public Function Bytes() As Byte()
        Return Me.m_lData.ToArray()
    End Function

    Public Sub Clear()
        Me.m_lData.Clear()
    End Sub

End Class
