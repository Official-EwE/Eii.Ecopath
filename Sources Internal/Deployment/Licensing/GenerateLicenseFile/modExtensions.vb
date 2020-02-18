' ===============================================================================
' This file is part of the Safenet toolkit.
'
' To use Safenet tools please contact Marta Coll or Jeroen Steenbeek at
' ecopathinternational@gmail.com
'
' Copyright 2017- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Drawing

<HideModuleName()>
Public Module modExtensions

    <Extension()>
    Public Function Value(o As Object, strField As String) As String
        Dim t As Type = o.GetType()
        For Each pi As PropertyInfo In t.GetProperties(BindingFlags.Public Or BindingFlags.Instance)
            If (String.Compare(pi.Name, strField, True) = 0) Then
                Return CStr(pi.GetValue(o, Nothing))
            End If
        Next
        Debug.Assert(False, "Object does not support the indicated field")
        Return ""
    End Function

    Private Class PointSort
        Implements IComparer(Of PointF)

        Public Function Compare(x As PointF, y As PointF) As Integer Implements IComparer(Of PointF).Compare
            If (x.X < y.X) Then Return -1
            If (x.X = y.X) Then Return 0
            Return 1
        End Function

    End Class

    <Extension()>
    Public Function Stretch(pts() As Single, iNewLength As Integer, Optional sZero As Single = 0.0!, Optional sNoData As Single = -9999.0!) As Single()

        If (pts Is Nothing) Then Return pts
        If (pts.Length < 2) Then Return pts

        iNewLength = Math.Max(iNewLength, pts.Length) - 1

        Dim ptsOut(iNewLength) As Single
        Dim dx As Decimal = CDec(iNewLength / (pts.Length - 1))

        Dim iTgtLast As Integer = 0
        Dim sValLast As Single = 0

        For iOrg As Integer = 0 To pts.Length - 1

            Dim iTgtNew As Integer = CInt(Math.Round(iOrg * dx))
            Dim sValNew As Single = pts(iOrg)

            For j As Integer = iTgtLast + 1 To iTgtNew
                If (sValLast = sNoData) Or (sValNew = sNoData) Then
                    ptsOut(j) = sNoData
                ElseIf (sValLast = sZero) Or (sValNew = sZero) Then
                    ptsOut(j) = sZero
                Else
                    ptsOut(j) = sValLast + (j - iTgtLast) * ((sValNew - sValLast) / (iTgtNew - iTgtLast))
                End If
            Next

            iTgtLast = iTgtNew
            sValLast = sValNew

            ptsOut(iTgtNew) = sValLast
        Next
        Return ptsOut

    End Function

    <Extension()>
    Public Function Shrink(pts() As Single, iNewLength As Integer, Optional sZero As Single = 0.0!, Optional sNoData As Single = -9999.0!) As Single()

        If (pts Is Nothing) Then Return pts
        If (pts.Length < 2) Then Return pts

        iNewLength = Math.Min(iNewLength, pts.Length) - 1

        Dim ptsOut(iNewLength) As Single
        Dim dx As Decimal = CDec((pts.Length - 1) / iNewLength)

        For iNew As Integer = 0 To iNewLength
            Dim sOrg As Single = iNew * dx
            If (sOrg) = CInt(sOrg) Then
                ptsOut(iNew) = pts(CInt(sOrg))
            Else
                ' Interpolate
                Dim iPrev As Integer = CInt(Math.Max(0, Math.Floor(iNew * dx)))
                Dim iNext As Integer = CInt(Math.Min(pts.Length - 1, Math.Ceiling(iNew * dx)))
                Dim sValPrev As Single = pts(iPrev)
                Dim sValNext As Single = pts(iNext)

                If (sValPrev = sNoData) Or (sValNext = sNoData) Then
                    ptsOut(iNew) = sNoData
                ElseIf (sValPrev = sZero) Or (sValNext = sZero) Then
                    ptsOut(iNew) = sZero
                Else
                    ptsOut(iNew) = sValPrev + (sOrg - CInt(sOrg)) * ((sValNext - sValPrev) / dx)
                End If
            End If
        Next
        Return ptsOut

    End Function

    ''' <summary>
    ''' Interpolate points 
    ''' </summary>
    ''' <param name="pts"></param>
    ''' <param name="iNewLength"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function Interpolate(pts() As PointF, iNewLength As Integer, Optional ByRef Xmin As Single = 0, Optional ByRef XMax As Single = 0) As PointF()

        If (pts Is Nothing) Then Return pts
        If (pts.Length < 2) Then Return pts

        Array.Sort(pts, New PointSort())

        Xmin = pts(0).X
        XMax = pts(pts.Length - 1).X

        Dim ptsOut(iNewLength - 1) As PointF
        Dim dxData As Decimal = CDec((XMax - Xmin) / iNewLength)

        Dim iPtFrom As Integer = 0
        Dim iPtTo As Integer = 0

        For i As Integer = 0 To iNewLength - 1
            ' Interpolated X value at the new array
            Dim xAt As Single = Xmin + dxData * i
            ' Find source array points that surround this value
            ' For now, do this stupidly in a loop. Can be an incremental change though
            iPtFrom = 0 : iPtTo = 0
            For j As Integer = 0 To pts.Length - 1
                If pts(j).X <= xAt Then
                    iPtFrom = j
                ElseIf (pts(j).X >= xAt) And (iPtTo = 0) Then
                    iPtTo = j
                End If
            Next

            If (pts(iPtTo).Y = pts(iPtFrom).Y) Then
                ptsOut(i) = New PointF(xAt, pts(iPtFrom).Y)
            Else
                Dim dx As Single = (xAt - pts(iPtFrom).X) / (pts(iPtTo).X - pts(iPtFrom).X)
                ptsOut(i) = New PointF(xAt, pts(iPtFrom).Y + dx * (pts(iPtTo).Y - pts(iPtFrom).Y))
            End If

        Next
        Return ptsOut

    End Function

    <Extension()>
    Public Sub ScientificNameToGenusSpecies(name As String, ByRef genus As String, ByRef species As String)
        name = name.Trim()
        Dim i As Integer = name.IndexOf(" "c)
        If i > 1 Then
            genus = name.Substring(0, i)
            species = name.Substring(i + 1).Trim
        Else
            genus = name
            species = ""
        End If
    End Sub

End Module
