Imports System.Text

Public Class cDiffChecker

    Private m_diffs As New List(Of cRowDifference)

    ''' <summary>
    ''' Checks all the rows in <paramref name="dt1"/> against the rows in <paramref name="dt2"/>
    ''' </summary>
    ''' <param name="dt1"></param>
    ''' <param name="dt2"></param>
    ''' <returns>True of there were differences</returns>
    Public Function GetDifferences(dt1 As DataTable, dt2 As DataTable) As Boolean

        Me.Clear()

        Dim keys As New List(Of String)
        For iCol As Integer = 0 To Math.Min(4, dt1.Columns.Count - 1)
            Dim col As DataColumn = dt1.Columns(iCol)
            If (col.DataType Is GetType(Int16)) Or (col.DataType Is GetType(Int32)) Or (col.DataType Is GetType(Int64)) Or (col.DataType = GetType(Byte)) Then
                keys.Add(col.ColumnName)
            End If
        Next

        If (keys.Count = 0) Then
            ' Skip table
            Return False
        End If

        For Each drow1 As DataRow In dt1.Rows

            ' Build filter
            Dim sbFilter As New StringBuilder()
            For i As Integer = 0 To keys.Count - 1
                If (i) Then sbFilter.Append(" AND ")
                sbFilter.Append(String.Format("{0}={1}", keys(i), drow1(keys(i))))
            Next
            Dim filter As String = sbFilter.ToString()

            Dim drows2 As DataRow() = dt2.Select(filter)

            Select Case drows2.Length
                Case 0
                    Me.m_diffs.Add(New cRowDifference(filter, "row missing", "", "", cRowDifference.eRowDifference.Missing))
                Case 1
                    Me.Compare(filter, dt1, drow1, drows2(0))
                Case Else
                    Debug.Assert(False)
            End Select

        Next
        Return m_diffs.Count > 0

    End Function

    Private Sub Clear()
        Me.m_diffs.Clear()
    End Sub

    Public ReadOnly Property Differences As cRowDifference()
        Get
            Return Me.m_diffs.ToArray()
        End Get
    End Property

    Private Enum eIsThere As Integer
        Present
        Missing
    End Enum

    Private Sub Compare(name As String, dt As DataTable, drow1 As DataRow, drow2 As DataRow)

        For Each col As DataColumn In dt.Columns
            Dim v1 As Object = Nothing
            Dim v2 As Object = Nothing
            Try
                v1 = drow1(col.ColumnName)
                v2 = drow2(col.ColumnName)
            Catch ex As Exception
                Debug.Assert(False)
            End Try

            Dim bMissing1 As eIsThere = If(Convert.IsDBNull(v1), eIsThere.Missing, eIsThere.Present)
            Dim bMissing2 As eIsThere = If(Convert.IsDBNull(v2), eIsThere.Missing, eIsThere.Present)

            If bMissing1 <> bMissing2 Then
                Me.m_diffs.Add(New cRowDifference(name, col.ColumnName, bMissing1.ToString, bMissing2.ToString, cRowDifference.eRowDifference.Missing))
            Else
                If Not Convert.Equals(v1, v2) Then
                    Dim str1 As String = ""
                    Dim str2 As String = ""
                    Me.m_diffs.Add(New cRowDifference(name, col.ColumnName, ToValue(v1), ToValue(v2), cRowDifference.eRowDifference.Changed))
                End If
            End If
        Next
    End Sub

    Private Function ToValue(v As Object) As String
        Dim t As Type = v.GetType()
        If t.IsArray Then Return "(array)"
        If t.IsGenericType Then Return "(other)"
        Return CStr(v)
    End Function

End Class
